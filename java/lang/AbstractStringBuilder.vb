Imports Microsoft.VisualBasic
Imports System

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

Namespace java.lang


	''' <summary>
	''' A mutable sequence of characters.
	''' <p>
	''' Implements a modifiable string. At any point in time it contains some
	''' particular sequence of characters, but the length and content of the
	''' sequence can be changed through certain method calls.
	''' 
	''' <p>Unless otherwise noted, passing a {@code null} argument to a constructor
	''' or method in this class will cause a <seealso cref="NullPointerException"/> to be
	''' thrown.
	''' 
	''' @author      Michael McCloskey
	''' @author      Martin Buchholz
	''' @author      Ulf Zibis
	''' @since       1.5
	''' </summary>
	Friend MustInherit Class AbstractStringBuilder
		Implements Appendable, CharSequence

'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			Public MustOverride Return intStream(() -> java.util.Spliterators.spliteratorUnknownSize(New CodePointIterator, java.util.Spliterator.ORDERED), java.util.Spliterator.ORDERED, False);
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			Public MustOverride Return toCodePoint(c1, c2);
			Public Function [if](Character.isHighSurrogate(c1) AndAlso cur <   length As ) As [MustOverride]
			Public Function [if](cur >=   length As ) As [MustOverride]
			Public Function accept(Character.toCodePoint(c1, c2)   As ) As [MustOverride]
			Public Function [if](Character.isLowSurrogate(c2)   As ) As [MustOverride]
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			Public MustOverride  accept(c1);
			Public Function [if]((Not Character.isHighSurrogate(c1)) OrElse i >=   length As ) As [MustOverride]
			Public Function [while](i <   length As ) As [MustOverride]
			Public MustOverride Function codePoints() As default
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			Public MustOverride Return intStream(() -> java.util.Spliterators.spliterator(New CharIterator, length(), java.util.Spliterator.ORDERED), java.util.Spliterator.SUBSIZED | java.util.Spliterator.SIZED | java.util.Spliterator.ORDERED, False);
			Public Function accept(charAt(cur)   As ) As [MustOverride]
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
			Public Function [for](; cur < length(); cur +=   1 As ) As [MustOverride]
			Public MustOverride Sub forEachRemaining(  block As java.util.function.IntConsumer) Implements CharSequence.forEachRemaining
			Public MustOverride Function NoSuchElementException() As throw
				Friend   += 1
				Public MustOverride Function charAt(cur   As ) As [Return] Implements CharSequence.charAt
			Public MustOverride [if](hasNext())
			Public MustOverride Integer nextInt()
			Public MustOverride Boolean hasNext()
			Public MustOverride default chars()
			Public MustOverride Appendable append(CharSequence csq, Integer start, Integer end)
			Public MustOverride Appendable append(CharSequence csq)
		''' <summary>
		''' The value is used for character storage.
		''' </summary>
		Dim value_Renamed As Char()

		''' <summary>
		''' The count is the number of characters used.
		''' </summary>
		Dim count As Integer

		''' <summary>
		''' This no-arg constructor is necessary for serialization of subclasses.
		''' </summary>
		AbstractStringBuilder()

		''' <summary>
		''' Creates an AbstractStringBuilder of the specified capacity.
		''' </summary>
		AbstractStringBuilder(Integer capacity)
			value_Renamed = New Char(capacity - 1){}

		''' <summary>
		''' Returns the length (character count).
		''' </summary>
		''' <returns>  the length of the sequence of characters currently
		'''          represented by this object </returns>
		public Integer length()
			Return count

		''' <summary>
		''' Returns the current capacity. The capacity is the amount of storage
		''' available for newly inserted characters, beyond which an allocation
		''' will occur.
		''' </summary>
		''' <returns>  the current capacity </returns>
		public Integer capacity()
			Return value_Renamed.Length

		''' <summary>
		''' Ensures that the capacity is at least equal to the specified minimum.
		''' If the current capacity is less than the argument, then a new internal
		''' array is allocated with greater capacity. The new capacity is the
		''' larger of:
		''' <ul>
		''' <li>The {@code minimumCapacity} argument.
		''' <li>Twice the old capacity, plus {@code 2}.
		''' </ul>
		''' If the {@code minimumCapacity} argument is nonpositive, this
		''' method takes no action and simply returns.
		''' Note that subsequent operations on this object can reduce the
		''' actual capacity below that requested here.
		''' </summary>
		''' <param name="minimumCapacity">   the minimum desired capacity. </param>
		public  Sub  ensureCapacity(Integer minimumCapacity)
			If minimumCapacity > 0 Then ensureCapacityInternal(minimumCapacity)

		''' <summary>
		''' This method has the same contract as ensureCapacity, but is
		''' never synchronized.
		''' </summary>
		private  Sub  ensureCapacityInternal(Integer minimumCapacity)
			' overflow-conscious code
			If minimumCapacity - value_Renamed.Length > 0 Then expandCapacity(minimumCapacity)

		''' <summary>
		''' This implements the expansion semantics of ensureCapacity with no
		''' size check or synchronization.
		''' </summary>
		void expandCapacity(Integer minimumCapacity)
			Dim newCapacity As Integer = value_Renamed.Length * 2 + 2
			If newCapacity - minimumCapacity < 0 Then newCapacity = minimumCapacity
			If newCapacity < 0 Then
				If minimumCapacity < 0 Then ' overflow Throw New OutOfMemoryError
				newCapacity =  java.lang.[Integer].Max_Value
			End If
			value_Renamed = java.util.Arrays.copyOf(value_Renamed, newCapacity)

		''' <summary>
		''' Attempts to reduce storage used for the character sequence.
		''' If the buffer is larger than necessary to hold its current sequence of
		''' characters, then it may be resized to become more space efficient.
		''' Calling this method may, but is not required to, affect the value
		''' returned by a subsequent call to the <seealso cref="#capacity()"/> method.
		''' </summary>
		public  Sub  trimToSize()
			If count < value_Renamed.Length Then value_Renamed = java.util.Arrays.copyOf(value_Renamed, count)

		''' <summary>
		''' Sets the length of the character sequence.
		''' The sequence is changed to a new character sequence
		''' whose length is specified by the argument. For every nonnegative
		''' index <i>k</i> less than {@code newLength}, the character at
		''' index <i>k</i> in the new character sequence is the same as the
		''' character at index <i>k</i> in the old sequence if <i>k</i> is less
		''' than the length of the old character sequence; otherwise, it is the
		''' null character {@code '\u005Cu0000'}.
		''' 
		''' In other words, if the {@code newLength} argument is less than
		''' the current length, the length is changed to the specified length.
		''' <p>
		''' If the {@code newLength} argument is greater than or equal
		''' to the current length, sufficient null characters
		''' ({@code '\u005Cu0000'}) are appended so that
		''' length becomes the {@code newLength} argument.
		''' <p>
		''' The {@code newLength} argument must be greater than or equal
		''' to {@code 0}.
		''' </summary>
		''' <param name="newLength">   the new length </param>
		''' <exception cref="IndexOutOfBoundsException">  if the
		'''               {@code newLength} argument is negative. </exception>
		public  Sub  lengthgth(Integer newLength)
			If newLength < 0 Then Throw New StringIndexOutOfBoundsException(newLength)
			ensureCapacityInternal(newLength)

			If count < newLength Then java.util.Arrays.fill(value_Renamed, count, newLength, ControlChars.NullChar)

			count = newLength

		''' <summary>
		''' Returns the {@code char} value in this sequence at the specified index.
		''' The first {@code char} value is at index {@code 0}, the next at index
		''' {@code 1}, and so on, as in array indexing.
		''' <p>
		''' The index argument must be greater than or equal to
		''' {@code 0}, and less than the length of this sequence.
		''' 
		''' <p>If the {@code char} value specified by the index is a
		''' <a href="Character.html#unicode">surrogate</a>, the surrogate
		''' value is returned.
		''' </summary>
		''' <param name="index">   the index of the desired {@code char} value. </param>
		''' <returns>     the {@code char} value at the specified index. </returns>
		''' <exception cref="IndexOutOfBoundsException">  if {@code index} is
		'''             negative or greater than or equal to {@code length()}. </exception>
		public Char charAt(Integer index)
			If (index < 0) OrElse (index >= count) Then Throw New StringIndexOutOfBoundsException(index)
			Return value_Renamed(index)

		''' <summary>
		''' Returns the character (Unicode code point) at the specified
		''' index. The index refers to {@code char} values
		''' (Unicode code units) and ranges from {@code 0} to
		''' <seealso cref="#length()"/>{@code  - 1}.
		''' 
		''' <p> If the {@code char} value specified at the given index
		''' is in the high-surrogate range, the following index is less
		''' than the length of this sequence, and the
		''' {@code char} value at the following index is in the
		''' low-surrogate range, then the supplementary code point
		''' corresponding to this surrogate pair is returned. Otherwise,
		''' the {@code char} value at the given index is returned.
		''' </summary>
		''' <param name="index"> the index to the {@code char} values </param>
		''' <returns>     the code point value of the character at the
		'''             {@code index} </returns>
		''' <exception cref="IndexOutOfBoundsException">  if the {@code index}
		'''             argument is negative or not less than the length of this
		'''             sequence. </exception>
		public Integer codePointAt(Integer index)
			If (index < 0) OrElse (index >= count) Then Throw New StringIndexOutOfBoundsException(index)
			Return Character.codePointAtImpl(value_Renamed, index, count)

		''' <summary>
		''' Returns the character (Unicode code point) before the specified
		''' index. The index refers to {@code char} values
		''' (Unicode code units) and ranges from {@code 1} to {@link
		''' #length()}.
		''' 
		''' <p> If the {@code char} value at {@code (index - 1)}
		''' is in the low-surrogate range, {@code (index - 2)} is not
		''' negative, and the {@code char} value at {@code (index -
		''' 2)} is in the high-surrogate range, then the
		''' supplementary code point value of the surrogate pair is
		''' returned. If the {@code char} value at {@code index -
		''' 1} is an unpaired low-surrogate or a high-surrogate, the
		''' surrogate value is returned.
		''' </summary>
		''' <param name="index"> the index following the code point that should be returned </param>
		''' <returns>    the Unicode code point value before the given index. </returns>
		''' <exception cref="IndexOutOfBoundsException"> if the {@code index}
		'''            argument is less than 1 or greater than the length
		'''            of this sequence. </exception>
		public Integer codePointBefore(Integer index)
			Dim i As Integer = index - 1
			If (i < 0) OrElse (i >= count) Then Throw New StringIndexOutOfBoundsException(index)
			Return Character.codePointBeforeImpl(value_Renamed, index, 0)

		''' <summary>
		''' Returns the number of Unicode code points in the specified text
		''' range of this sequence. The text range begins at the specified
		''' {@code beginIndex} and extends to the {@code char} at
		''' index {@code endIndex - 1}. Thus the length (in
		''' {@code char}s) of the text range is
		''' {@code endIndex-beginIndex}. Unpaired surrogates within
		''' this sequence count as one code point each.
		''' </summary>
		''' <param name="beginIndex"> the index to the first {@code char} of
		''' the text range. </param>
		''' <param name="endIndex"> the index after the last {@code char} of
		''' the text range. </param>
		''' <returns> the number of Unicode code points in the specified text
		''' range </returns>
		''' <exception cref="IndexOutOfBoundsException"> if the
		''' {@code beginIndex} is negative, or {@code endIndex}
		''' is larger than the length of this sequence, or
		''' {@code beginIndex} is larger than {@code endIndex}. </exception>
		public Integer codePointCount(Integer beginIndex, Integer endIndex)
			If beginIndex < 0 OrElse endIndex > count OrElse beginIndex > endIndex Then Throw New IndexOutOfBoundsException
			Return Character.codePointCountImpl(value_Renamed, beginIndex, endIndex-beginIndex)

		''' <summary>
		''' Returns the index within this sequence that is offset from the
		''' given {@code index} by {@code codePointOffset} code
		''' points. Unpaired surrogates within the text range given by
		''' {@code index} and {@code codePointOffset} count as
		''' one code point each.
		''' </summary>
		''' <param name="index"> the index to be offset </param>
		''' <param name="codePointOffset"> the offset in code points </param>
		''' <returns> the index within this sequence </returns>
		''' <exception cref="IndexOutOfBoundsException"> if {@code index}
		'''   is negative or larger then the length of this sequence,
		'''   or if {@code codePointOffset} is positive and the subsequence
		'''   starting with {@code index} has fewer than
		'''   {@code codePointOffset} code points,
		'''   or if {@code codePointOffset} is negative and the subsequence
		'''   before {@code index} has fewer than the absolute value of
		'''   {@code codePointOffset} code points. </exception>
		public Integer offsetByCodePoints(Integer index, Integer codePointOffset)
			If index < 0 OrElse index > count Then Throw New IndexOutOfBoundsException
			Return Character.offsetByCodePointsImpl(value_Renamed, 0, count, index, codePointOffset)

		''' <summary>
		''' Characters are copied from this sequence into the
		''' destination character array {@code dst}. The first character to
		''' be copied is at index {@code srcBegin}; the last character to
		''' be copied is at index {@code srcEnd-1}. The total number of
		''' characters to be copied is {@code srcEnd-srcBegin}. The
		''' characters are copied into the subarray of {@code dst} starting
		''' at index {@code dstBegin} and ending at index:
		''' <pre>{@code
		''' dstbegin + (srcEnd-srcBegin) - 1
		''' }</pre>
		''' </summary>
		''' <param name="srcBegin">   start copying at this offset. </param>
		''' <param name="srcEnd">     stop copying at this offset. </param>
		''' <param name="dst">        the array to copy the data into. </param>
		''' <param name="dstBegin">   offset into {@code dst}. </param>
		''' <exception cref="IndexOutOfBoundsException">  if any of the following is true:
		'''             <ul>
		'''             <li>{@code srcBegin} is negative
		'''             <li>{@code dstBegin} is negative
		'''             <li>the {@code srcBegin} argument is greater than
		'''             the {@code srcEnd} argument.
		'''             <li>{@code srcEnd} is greater than
		'''             {@code this.length()}.
		'''             <li>{@code dstBegin+srcEnd-srcBegin} is greater than
		'''             {@code dst.length}
		'''             </ul> </exception>
		public  Sub  getChars(Integer srcBegin, Integer srcEnd, Char() dst, Integer dstBegin)
			If srcBegin < 0 Then Throw New StringIndexOutOfBoundsException(srcBegin)
			If (srcEnd < 0) OrElse (srcEnd > count) Then Throw New StringIndexOutOfBoundsException(srcEnd)
			If srcBegin > srcEnd Then Throw New StringIndexOutOfBoundsException("srcBegin > srcEnd")
			Array.Copy(value_Renamed, srcBegin, dst, dstBegin, srcEnd - srcBegin)

		''' <summary>
		''' The character at the specified index is set to {@code ch}. This
		''' sequence is altered to represent a new character sequence that is
		''' identical to the old character sequence, except that it contains the
		''' character {@code ch} at position {@code index}.
		''' <p>
		''' The index argument must be greater than or equal to
		''' {@code 0}, and less than the length of this sequence.
		''' </summary>
		''' <param name="index">   the index of the character to modify. </param>
		''' <param name="ch">      the new character. </param>
		''' <exception cref="IndexOutOfBoundsException">  if {@code index} is
		'''             negative or greater than or equal to {@code length()}. </exception>
		public  Sub  charAtrAt(Integer index, Char ch)
			If (index < 0) OrElse (index >= count) Then Throw New StringIndexOutOfBoundsException(index)
			value_Renamed(index) = ch

		''' <summary>
		''' Appends the string representation of the {@code Object} argument.
		''' <p>
		''' The overall effect is exactly as if the argument were converted
		''' to a string by the method <seealso cref="String#valueOf(Object)"/>,
		''' and the characters of that string were then
		''' <seealso cref="#append(String) appended"/> to this character sequence.
		''' </summary>
		''' <param name="obj">   an {@code Object}. </param>
		''' <returns>  a reference to this object. </returns>
		public AbstractStringBuilder append(Object obj)
			Return append(Convert.ToString(obj))

		''' <summary>
		''' Appends the specified string to this character sequence.
		''' <p>
		''' The characters of the {@code String} argument are appended, in
		''' order, increasing the length of this sequence by the length of the
		''' argument. If {@code str} is {@code null}, then the four
		''' characters {@code "null"} are appended.
		''' <p>
		''' Let <i>n</i> be the length of this character sequence just prior to
		''' execution of the {@code append} method. Then the character at
		''' index <i>k</i> in the new character sequence is equal to the character
		''' at index <i>k</i> in the old character sequence, if <i>k</i> is less
		''' than <i>n</i>; otherwise, it is equal to the character at index
		''' <i>k-n</i> in the argument {@code str}.
		''' </summary>
		''' <param name="str">   a string. </param>
		''' <returns>  a reference to this object. </returns>
		public AbstractStringBuilder append(String str)
			If str Is Nothing Then Return appendNull()
			Dim len As Integer = str.length()
			ensureCapacityInternal(count + len)
			str.getChars(0, len, value_Renamed, count)
			count += len
			Return Me

		' Documentation in subclasses because of synchro difference
		public AbstractStringBuilder append(StringBuffer sb)
			If sb Is Nothing Then Return appendNull()
			Dim len As Integer = sb.length()
			ensureCapacityInternal(count + len)
			sb.getChars(0, len, value_Renamed, count)
			count += len
			Return Me

		''' <summary>
		''' @since 1.8
		''' </summary>
		AbstractStringBuilder append(AbstractStringBuilder asb)
			If asb Is Nothing Then Return appendNull()
			Dim len As Integer = asb.length()
			ensureCapacityInternal(count + len)
			asb.getChars(0, len, value_Renamed, count)
			count += len
			Return Me

		' Documentation in subclasses because of synchro difference
		public AbstractStringBuilder append(CharSequence s)
			If s Is Nothing Then Return appendNull()
			If TypeOf s Is String Then Return Me.append(CStr(s))
			If TypeOf s Is AbstractStringBuilder Then Return Me.append(CType(s, AbstractStringBuilder))

			Return Me.append(s, 0, s.length())

		private AbstractStringBuilder appendNull()
			Dim c As Integer = count
			ensureCapacityInternal(c + 4)
			Dim value_Renamed As Char() = Me.value
				value_Renamed(c) = "n"c
				c += 1
				value_Renamed(c) = "u"c
				c += 1
				value_Renamed(c) = "l"c
				c += 1
				value_Renamed(c) = "l"c
				c += 1
			count = c
			Return Me

		''' <summary>
		''' Appends a subsequence of the specified {@code CharSequence} to this
		''' sequence.
		''' <p>
		''' Characters of the argument {@code s}, starting at
		''' index {@code start}, are appended, in order, to the contents of
		''' this sequence up to the (exclusive) index {@code end}. The length
		''' of this sequence is increased by the value of {@code end - start}.
		''' <p>
		''' Let <i>n</i> be the length of this character sequence just prior to
		''' execution of the {@code append} method. Then the character at
		''' index <i>k</i> in this character sequence becomes equal to the
		''' character at index <i>k</i> in this sequence, if <i>k</i> is less than
		''' <i>n</i>; otherwise, it is equal to the character at index
		''' <i>k+start-n</i> in the argument {@code s}.
		''' <p>
		''' If {@code s} is {@code null}, then this method appends
		''' characters as if the s parameter was a sequence containing the four
		''' characters {@code "null"}.
		''' </summary>
		''' <param name="s"> the sequence to append. </param>
		''' <param name="start">   the starting index of the subsequence to be appended. </param>
		''' <param name="end">     the end index of the subsequence to be appended. </param>
		''' <returns>  a reference to this object. </returns>
		''' <exception cref="IndexOutOfBoundsException"> if
		'''             {@code start} is negative, or
		'''             {@code start} is greater than {@code end} or
		'''             {@code end} is greater than {@code s.length()} </exception>
		public AbstractStringBuilder append(CharSequence s, Integer start, Integer end)
			If s Is Nothing Then s = "null"
			If (start < 0) OrElse (start > end) OrElse (end > s.length()) Then Throw New IndexOutOfBoundsException("start " & start & ", end " & end & ", s.length() " & s.length())
			Dim len As Integer = end - start
			ensureCapacityInternal(count + len)
			Dim i As Integer = start
			Dim j As Integer = count
			Do While i < end
				value_Renamed(j) = s.Chars(i)
				i += 1
				j += 1
			Loop
			count += len
			Return Me

		''' <summary>
		''' Appends the string representation of the {@code char} array
		''' argument to this sequence.
		''' <p>
		''' The characters of the array argument are appended, in order, to
		''' the contents of this sequence. The length of this sequence
		''' increases by the length of the argument.
		''' <p>
		''' The overall effect is exactly as if the argument were converted
		''' to a string by the method <seealso cref="String#valueOf(char[])"/>,
		''' and the characters of that string were then
		''' <seealso cref="#append(String) appended"/> to this character sequence.
		''' </summary>
		''' <param name="str">   the characters to be appended. </param>
		''' <returns>  a reference to this object. </returns>
		public AbstractStringBuilder append(Char() str)
			Dim len As Integer = str.length
			ensureCapacityInternal(count + len)
			Array.Copy(str, 0, value_Renamed, count, len)
			count += len
			Return Me

		''' <summary>
		''' Appends the string representation of a subarray of the
		''' {@code char} array argument to this sequence.
		''' <p>
		''' Characters of the {@code char} array {@code str}, starting at
		''' index {@code offset}, are appended, in order, to the contents
		''' of this sequence. The length of this sequence increases
		''' by the value of {@code len}.
		''' <p>
		''' The overall effect is exactly as if the arguments were converted
		''' to a string by the method <seealso cref="String#valueOf(char[],int,int)"/>,
		''' and the characters of that string were then
		''' <seealso cref="#append(String) appended"/> to this character sequence.
		''' </summary>
		''' <param name="str">      the characters to be appended. </param>
		''' <param name="offset">   the index of the first {@code char} to append. </param>
		''' <param name="len">      the number of {@code char}s to append. </param>
		''' <returns>  a reference to this object. </returns>
		''' <exception cref="IndexOutOfBoundsException">
		'''         if {@code offset < 0} or {@code len < 0}
		'''         or {@code offset+len > str.length} </exception>
		public AbstractStringBuilder append(Char str() , Integer offset, Integer len)
			If len > 0 Then ' let arraycopy report AIOOBE for len < 0 ensureCapacityInternal(count + len)
			Array.Copy(str, offset, value_Renamed, count, len)
			count += len
			Return Me

		''' <summary>
		''' Appends the string representation of the {@code boolean}
		''' argument to the sequence.
		''' <p>
		''' The overall effect is exactly as if the argument were converted
		''' to a string by the method <seealso cref="String#valueOf(boolean)"/>,
		''' and the characters of that string were then
		''' <seealso cref="#append(String) appended"/> to this character sequence.
		''' </summary>
		''' <param name="b">   a {@code boolean}. </param>
		''' <returns>  a reference to this object. </returns>
		public AbstractStringBuilder append(Boolean b)
			If b Then
				ensureCapacityInternal(count + 4)
				value_Renamed(count) = "t"c
				count += 1
				value_Renamed(count) = "r"c
				count += 1
				value_Renamed(count) = "u"c
				count += 1
				value_Renamed(count) = "e"c
				count += 1
			Else
				ensureCapacityInternal(count + 5)
				value_Renamed(count) = "f"c
				count += 1
				value_Renamed(count) = "a"c
				count += 1
				value_Renamed(count) = "l"c
				count += 1
				value_Renamed(count) = "s"c
				count += 1
				value_Renamed(count) = "e"c
				count += 1
			End If
			Return Me

		''' <summary>
		''' Appends the string representation of the {@code char}
		''' argument to this sequence.
		''' <p>
		''' The argument is appended to the contents of this sequence.
		''' The length of this sequence increases by {@code 1}.
		''' <p>
		''' The overall effect is exactly as if the argument were converted
		''' to a string by the method <seealso cref="String#valueOf(char)"/>,
		''' and the character in that string were then
		''' <seealso cref="#append(String) appended"/> to this character sequence.
		''' </summary>
		''' <param name="c">   a {@code char}. </param>
		''' <returns>  a reference to this object. </returns>
		public AbstractStringBuilder append(Char c)
			ensureCapacityInternal(count + 1)
			value_Renamed(count) = c
			count += 1
			Return Me

		''' <summary>
		''' Appends the string representation of the {@code int}
		''' argument to this sequence.
		''' <p>
		''' The overall effect is exactly as if the argument were converted
		''' to a string by the method <seealso cref="String#valueOf(int)"/>,
		''' and the characters of that string were then
		''' <seealso cref="#append(String) appended"/> to this character sequence.
		''' </summary>
		''' <param name="i">   an {@code int}. </param>
		''' <returns>  a reference to this object. </returns>
		public AbstractStringBuilder append(Integer i)
			If i =  java.lang.[Integer].MIN_VALUE Then
				append("-2147483648")
				Return Me
			End If
			Dim appendedLength As Integer = If(i < 0,  java.lang.[Integer].stringSize(-i) + 1,  java.lang.[Integer].stringSize(i))
			Dim spaceNeeded As Integer = count + appendedLength
			ensureCapacityInternal(spaceNeeded)
			 java.lang.[Integer].getChars(i, spaceNeeded, value_Renamed)
			count = spaceNeeded
			Return Me

		''' <summary>
		''' Appends the string representation of the {@code long}
		''' argument to this sequence.
		''' <p>
		''' The overall effect is exactly as if the argument were converted
		''' to a string by the method <seealso cref="String#valueOf(long)"/>,
		''' and the characters of that string were then
		''' <seealso cref="#append(String) appended"/> to this character sequence.
		''' </summary>
		''' <param name="l">   a {@code long}. </param>
		''' <returns>  a reference to this object. </returns>
		public AbstractStringBuilder append(Long l)
			If l = java.lang.[Long].MIN_VALUE Then
				append("-9223372036854775808")
				Return Me
			End If
			Dim appendedLength As Integer = If(l < 0, java.lang.[Long].stringSize(-l) + 1, java.lang.[Long].stringSize(l))
			Dim spaceNeeded As Integer = count + appendedLength
			ensureCapacityInternal(spaceNeeded)
			Long.getChars(l, spaceNeeded, value_Renamed)
			count = spaceNeeded
			Return Me

		''' <summary>
		''' Appends the string representation of the {@code float}
		''' argument to this sequence.
		''' <p>
		''' The overall effect is exactly as if the argument were converted
		''' to a string by the method <seealso cref="String#valueOf(float)"/>,
		''' and the characters of that string were then
		''' <seealso cref="#append(String) appended"/> to this character sequence.
		''' </summary>
		''' <param name="f">   a {@code float}. </param>
		''' <returns>  a reference to this object. </returns>
		public AbstractStringBuilder append(Single f)
			sun.misc.FloatingDecimal.appendTo(f,Me)
			Return Me

		''' <summary>
		''' Appends the string representation of the {@code double}
		''' argument to this sequence.
		''' <p>
		''' The overall effect is exactly as if the argument were converted
		''' to a string by the method <seealso cref="String#valueOf(double)"/>,
		''' and the characters of that string were then
		''' <seealso cref="#append(String) appended"/> to this character sequence.
		''' </summary>
		''' <param name="d">   a {@code double}. </param>
		''' <returns>  a reference to this object. </returns>
		public AbstractStringBuilder append(Double d)
			sun.misc.FloatingDecimal.appendTo(d,Me)
			Return Me

		''' <summary>
		''' Removes the characters in a substring of this sequence.
		''' The substring begins at the specified {@code start} and extends to
		''' the character at index {@code end - 1} or to the end of the
		''' sequence if no such character exists. If
		''' {@code start} is equal to {@code end}, no changes are made.
		''' </summary>
		''' <param name="start">  The beginning index, inclusive. </param>
		''' <param name="end">    The ending index, exclusive. </param>
		''' <returns>     This object. </returns>
		''' <exception cref="StringIndexOutOfBoundsException">  if {@code start}
		'''             is negative, greater than {@code length()}, or
		'''             greater than {@code end}. </exception>
		public AbstractStringBuilder delete(Integer start, Integer end)
			If start < 0 Then Throw New StringIndexOutOfBoundsException(start)
			If end > count Then end = count
			If start > end Then Throw New StringIndexOutOfBoundsException
			Dim len As Integer = end - start
			If len > 0 Then
				Array.Copy(value_Renamed, start+len, value_Renamed, start, count-end)
				count -= len
			End If
			Return Me

		''' <summary>
		''' Appends the string representation of the {@code codePoint}
		''' argument to this sequence.
		''' 
		''' <p> The argument is appended to the contents of this sequence.
		''' The length of this sequence increases by
		''' <seealso cref="Character#charCount(int) Character.charCount(codePoint)"/>.
		''' 
		''' <p> The overall effect is exactly as if the argument were
		''' converted to a {@code char} array by the method
		''' <seealso cref="Character#toChars(int)"/> and the character in that array
		''' were then <seealso cref="#append(char[]) appended"/> to this character
		''' sequence.
		''' </summary>
		''' <param name="codePoint">   a Unicode code point </param>
		''' <returns>  a reference to this object. </returns>
		''' <exception cref="IllegalArgumentException"> if the specified
		''' {@code codePoint} isn't a valid Unicode code point </exception>
		public AbstractStringBuilder appendCodePoint(Integer codePoint)
			Dim count As Integer = Me.count

			If Character.isBmpCodePoint(codePoint) Then
				ensureCapacityInternal(count + 1)
				value_Renamed(count) = CChar(codePoint)
				Me.count = count + 1
			ElseIf Character.isValidCodePoint(codePoint) Then
				ensureCapacityInternal(count + 2)
				Character.toSurrogates(codePoint, value_Renamed, count)
				Me.count = count + 2
			Else
				Throw New IllegalArgumentException
			End If
			Return Me

		''' <summary>
		''' Removes the {@code char} at the specified position in this
		''' sequence. This sequence is shortened by one {@code char}.
		''' 
		''' <p>Note: If the character at the given index is a supplementary
		''' character, this method does not remove the entire character. If
		''' correct handling of supplementary characters is required,
		''' determine the number of {@code char}s to remove by calling
		''' {@code Character.charCount(thisSequence.codePointAt(index))},
		''' where {@code thisSequence} is this sequence.
		''' </summary>
		''' <param name="index">  Index of {@code char} to remove </param>
		''' <returns>      This object. </returns>
		''' <exception cref="StringIndexOutOfBoundsException">  if the {@code index}
		'''              is negative or greater than or equal to
		'''              {@code length()}. </exception>
		public AbstractStringBuilder deleteCharAt(Integer index)
			If (index < 0) OrElse (index >= count) Then Throw New StringIndexOutOfBoundsException(index)
			Array.Copy(value_Renamed, index+1, value_Renamed, index, count-index-1)
			count -= 1
			Return Me

		''' <summary>
		''' Replaces the characters in a substring of this sequence
		''' with characters in the specified {@code String}. The substring
		''' begins at the specified {@code start} and extends to the character
		''' at index {@code end - 1} or to the end of the
		''' sequence if no such character exists. First the
		''' characters in the substring are removed and then the specified
		''' {@code String} is inserted at {@code start}. (This
		''' sequence will be lengthened to accommodate the
		''' specified String if necessary.)
		''' </summary>
		''' <param name="start">    The beginning index, inclusive. </param>
		''' <param name="end">      The ending index, exclusive. </param>
		''' <param name="str">   String that will replace previous contents. </param>
		''' <returns>     This object. </returns>
		''' <exception cref="StringIndexOutOfBoundsException">  if {@code start}
		'''             is negative, greater than {@code length()}, or
		'''             greater than {@code end}. </exception>
		public AbstractStringBuilder replace(Integer start, Integer end, String str)
			If start < 0 Then Throw New StringIndexOutOfBoundsException(start)
			If start > count Then Throw New StringIndexOutOfBoundsException("start > length()")
			If start > end Then Throw New StringIndexOutOfBoundsException("start > end")

			If end > count Then end = count
			Dim len As Integer = str.length()
			Dim newCount As Integer = count + len - (end - start)
			ensureCapacityInternal(newCount)

			Array.Copy(value_Renamed, end, value_Renamed, start + len, count - end)
			str.getChars(value_Renamed, start)
			count = newCount
			Return Me

		''' <summary>
		''' Returns a new {@code String} that contains a subsequence of
		''' characters currently contained in this character sequence. The
		''' substring begins at the specified index and extends to the end of
		''' this sequence.
		''' </summary>
		''' <param name="start">    The beginning index, inclusive. </param>
		''' <returns>     The new string. </returns>
		''' <exception cref="StringIndexOutOfBoundsException">  if {@code start} is
		'''             less than zero, or greater than the length of this object. </exception>
		public String Substring(Integer start)
			Return Substring(start, count)

		''' <summary>
		''' Returns a new character sequence that is a subsequence of this sequence.
		''' 
		''' <p> An invocation of this method of the form
		''' 
		''' <pre>{@code
		''' sb.subSequence(begin,&nbsp;end)}</pre>
		''' 
		''' behaves in exactly the same way as the invocation
		''' 
		''' <pre>{@code
		''' sb.substring(begin,&nbsp;end)}</pre>
		''' 
		''' This method is provided so that this class can
		''' implement the <seealso cref="CharSequence"/> interface.
		''' </summary>
		''' <param name="start">   the start index, inclusive. </param>
		''' <param name="end">     the end index, exclusive. </param>
		''' <returns>     the specified subsequence.
		''' </returns>
		''' <exception cref="IndexOutOfBoundsException">
		'''          if {@code start} or {@code end} are negative,
		'''          if {@code end} is greater than {@code length()},
		'''          or if {@code start} is greater than {@code end}
		''' @spec JSR-51 </exception>
		public CharSequence subSequence(Integer start, Integer end)
			Return Substring(start, end)

		''' <summary>
		''' Returns a new {@code String} that contains a subsequence of
		''' characters currently contained in this sequence. The
		''' substring begins at the specified {@code start} and
		''' extends to the character at index {@code end - 1}.
		''' </summary>
		''' <param name="start">    The beginning index, inclusive. </param>
		''' <param name="end">      The ending index, exclusive. </param>
		''' <returns>     The new string. </returns>
		''' <exception cref="StringIndexOutOfBoundsException">  if {@code start}
		'''             or {@code end} are negative or greater than
		'''             {@code length()}, or {@code start} is
		'''             greater than {@code end}. </exception>
		public String Substring(Integer start, Integer end)
			If start < 0 Then Throw New StringIndexOutOfBoundsException(start)
			If end > count Then Throw New StringIndexOutOfBoundsException(end)
			If start > end Then Throw New StringIndexOutOfBoundsException(end - start)
			Return New String(value_Renamed, start, end - start)

		''' <summary>
		''' Inserts the string representation of a subarray of the {@code str}
		''' array argument into this sequence. The subarray begins at the
		''' specified {@code offset} and extends {@code len} {@code char}s.
		''' The characters of the subarray are inserted into this sequence at
		''' the position indicated by {@code index}. The length of this
		''' sequence increases by {@code len} {@code char}s.
		''' </summary>
		''' <param name="index">    position at which to insert subarray. </param>
		''' <param name="str">       A {@code char} array. </param>
		''' <param name="offset">   the index of the first {@code char} in subarray to
		'''             be inserted. </param>
		''' <param name="len">      the number of {@code char}s in the subarray to
		'''             be inserted. </param>
		''' <returns>     This object </returns>
		''' <exception cref="StringIndexOutOfBoundsException">  if {@code index}
		'''             is negative or greater than {@code length()}, or
		'''             {@code offset} or {@code len} are negative, or
		'''             {@code (offset+len)} is greater than
		'''             {@code str.length}. </exception>
		public AbstractStringBuilder insert(Integer index, Char() str, Integer offset, Integer len)
			If (index < 0) OrElse (index > length()) Then Throw New StringIndexOutOfBoundsException(index)
			If (offset < 0) OrElse (len < 0) OrElse (offset > str.length - len) Then Throw New StringIndexOutOfBoundsException("offset " & offset & ", len " & len & ", str.length " & str.length)
			ensureCapacityInternal(count + len)
			Array.Copy(value_Renamed, index, value_Renamed, index + len, count - index)
			Array.Copy(str, offset, value_Renamed, index, len)
			count += len
			Return Me

		''' <summary>
		''' Inserts the string representation of the {@code Object}
		''' argument into this character sequence.
		''' <p>
		''' The overall effect is exactly as if the second argument were
		''' converted to a string by the method <seealso cref="String#valueOf(Object)"/>,
		''' and the characters of that string were then
		''' <seealso cref="#insert(int,String) inserted"/> into this character
		''' sequence at the indicated offset.
		''' <p>
		''' The {@code offset} argument must be greater than or equal to
		''' {@code 0}, and less than or equal to the <seealso cref="#length() length"/>
		''' of this sequence.
		''' </summary>
		''' <param name="offset">   the offset. </param>
		''' <param name="obj">      an {@code Object}. </param>
		''' <returns>     a reference to this object. </returns>
		''' <exception cref="StringIndexOutOfBoundsException">  if the offset is invalid. </exception>
		public AbstractStringBuilder insert(Integer offset, Object obj)
			Return insert(offset, Convert.ToString(obj))

		''' <summary>
		''' Inserts the string into this character sequence.
		''' <p>
		''' The characters of the {@code String} argument are inserted, in
		''' order, into this sequence at the indicated offset, moving up any
		''' characters originally above that position and increasing the length
		''' of this sequence by the length of the argument. If
		''' {@code str} is {@code null}, then the four characters
		''' {@code "null"} are inserted into this sequence.
		''' <p>
		''' The character at index <i>k</i> in the new character sequence is
		''' equal to:
		''' <ul>
		''' <li>the character at index <i>k</i> in the old character sequence, if
		''' <i>k</i> is less than {@code offset}
		''' <li>the character at index <i>k</i>{@code -offset} in the
		''' argument {@code str}, if <i>k</i> is not less than
		''' {@code offset} but is less than {@code offset+str.length()}
		''' <li>the character at index <i>k</i>{@code -str.length()} in the
		''' old character sequence, if <i>k</i> is not less than
		''' {@code offset+str.length()}
		''' </ul><p>
		''' The {@code offset} argument must be greater than or equal to
		''' {@code 0}, and less than or equal to the <seealso cref="#length() length"/>
		''' of this sequence.
		''' </summary>
		''' <param name="offset">   the offset. </param>
		''' <param name="str">      a string. </param>
		''' <returns>     a reference to this object. </returns>
		''' <exception cref="StringIndexOutOfBoundsException">  if the offset is invalid. </exception>
		public AbstractStringBuilder insert(Integer offset, String str)
			If (offset < 0) OrElse (offset > length()) Then Throw New StringIndexOutOfBoundsException(offset)
			If str Is Nothing Then str = "null"
			Dim len As Integer = str.length()
			ensureCapacityInternal(count + len)
			Array.Copy(value_Renamed, offset, value_Renamed, offset + len, count - offset)
			str.getChars(value_Renamed, offset)
			count += len
			Return Me

		''' <summary>
		''' Inserts the string representation of the {@code char} array
		''' argument into this sequence.
		''' <p>
		''' The characters of the array argument are inserted into the
		''' contents of this sequence at the position indicated by
		''' {@code offset}. The length of this sequence increases by
		''' the length of the argument.
		''' <p>
		''' The overall effect is exactly as if the second argument were
		''' converted to a string by the method <seealso cref="String#valueOf(char[])"/>,
		''' and the characters of that string were then
		''' <seealso cref="#insert(int,String) inserted"/> into this character
		''' sequence at the indicated offset.
		''' <p>
		''' The {@code offset} argument must be greater than or equal to
		''' {@code 0}, and less than or equal to the <seealso cref="#length() length"/>
		''' of this sequence.
		''' </summary>
		''' <param name="offset">   the offset. </param>
		''' <param name="str">      a character array. </param>
		''' <returns>     a reference to this object. </returns>
		''' <exception cref="StringIndexOutOfBoundsException">  if the offset is invalid. </exception>
		public AbstractStringBuilder insert(Integer offset, Char() str)
			If (offset < 0) OrElse (offset > length()) Then Throw New StringIndexOutOfBoundsException(offset)
			Dim len As Integer = str.length
			ensureCapacityInternal(count + len)
			Array.Copy(value_Renamed, offset, value_Renamed, offset + len, count - offset)
			Array.Copy(str, 0, value_Renamed, offset, len)
			count += len
			Return Me

		''' <summary>
		''' Inserts the specified {@code CharSequence} into this sequence.
		''' <p>
		''' The characters of the {@code CharSequence} argument are inserted,
		''' in order, into this sequence at the indicated offset, moving up
		''' any characters originally above that position and increasing the length
		''' of this sequence by the length of the argument s.
		''' <p>
		''' The result of this method is exactly the same as if it were an
		''' invocation of this object's
		''' <seealso cref="#insert(int,CharSequence,int,int) insert"/>(dstOffset, s, 0, s.length())
		''' method.
		''' 
		''' <p>If {@code s} is {@code null}, then the four characters
		''' {@code "null"} are inserted into this sequence.
		''' </summary>
		''' <param name="dstOffset">   the offset. </param>
		''' <param name="s"> the sequence to be inserted </param>
		''' <returns>     a reference to this object. </returns>
		''' <exception cref="IndexOutOfBoundsException">  if the offset is invalid. </exception>
		public AbstractStringBuilder insert(Integer dstOffset, CharSequence s)
			If s Is Nothing Then s = "null"
			If TypeOf s Is String Then Return Me.insert(dstOffset, CStr(s))
			Return Me.insert(dstOffset, s, 0, s.length())

		''' <summary>
		''' Inserts a subsequence of the specified {@code CharSequence} into
		''' this sequence.
		''' <p>
		''' The subsequence of the argument {@code s} specified by
		''' {@code start} and {@code end} are inserted,
		''' in order, into this sequence at the specified destination offset, moving
		''' up any characters originally above that position. The length of this
		''' sequence is increased by {@code end - start}.
		''' <p>
		''' The character at index <i>k</i> in this sequence becomes equal to:
		''' <ul>
		''' <li>the character at index <i>k</i> in this sequence, if
		''' <i>k</i> is less than {@code dstOffset}
		''' <li>the character at index <i>k</i>{@code +start-dstOffset} in
		''' the argument {@code s}, if <i>k</i> is greater than or equal to
		''' {@code dstOffset} but is less than {@code dstOffset+end-start}
		''' <li>the character at index <i>k</i>{@code -(end-start)} in this
		''' sequence, if <i>k</i> is greater than or equal to
		''' {@code dstOffset+end-start}
		''' </ul><p>
		''' The {@code dstOffset} argument must be greater than or equal to
		''' {@code 0}, and less than or equal to the <seealso cref="#length() length"/>
		''' of this sequence.
		''' <p>The start argument must be nonnegative, and not greater than
		''' {@code end}.
		''' <p>The end argument must be greater than or equal to
		''' {@code start}, and less than or equal to the length of s.
		''' 
		''' <p>If {@code s} is {@code null}, then this method inserts
		''' characters as if the s parameter was a sequence containing the four
		''' characters {@code "null"}.
		''' </summary>
		''' <param name="dstOffset">   the offset in this sequence. </param>
		''' <param name="s">       the sequence to be inserted. </param>
		''' <param name="start">   the starting index of the subsequence to be inserted. </param>
		''' <param name="end">     the end index of the subsequence to be inserted. </param>
		''' <returns>     a reference to this object. </returns>
		''' <exception cref="IndexOutOfBoundsException">  if {@code dstOffset}
		'''             is negative or greater than {@code this.length()}, or
		'''              {@code start} or {@code end} are negative, or
		'''              {@code start} is greater than {@code end} or
		'''              {@code end} is greater than {@code s.length()} </exception>
		 public AbstractStringBuilder insert(Integer dstOffset, CharSequence s, Integer start, Integer end)
			If s Is Nothing Then s = "null"
			If (dstOffset < 0) OrElse (dstOffset > Me.length()) Then Throw New IndexOutOfBoundsException("dstOffset " & dstOffset)
			If (start < 0) OrElse (end < 0) OrElse (start > end) OrElse (end > s.length()) Then Throw New IndexOutOfBoundsException("start " & start & ", end " & end & ", s.length() " & s.length())
			Dim len As Integer = end - start
			ensureCapacityInternal(count + len)
			Array.Copy(value_Renamed, dstOffset, value_Renamed, dstOffset + len, count - dstOffset)
			For i As Integer = start To end - 1
				value_Renamed(dstOffset) = s.Chars(i)
				dstOffset += 1
			Next i
			count += len
			Return Me

		''' <summary>
		''' Inserts the string representation of the {@code boolean}
		''' argument into this sequence.
		''' <p>
		''' The overall effect is exactly as if the second argument were
		''' converted to a string by the method <seealso cref="String#valueOf(boolean)"/>,
		''' and the characters of that string were then
		''' <seealso cref="#insert(int,String) inserted"/> into this character
		''' sequence at the indicated offset.
		''' <p>
		''' The {@code offset} argument must be greater than or equal to
		''' {@code 0}, and less than or equal to the <seealso cref="#length() length"/>
		''' of this sequence.
		''' </summary>
		''' <param name="offset">   the offset. </param>
		''' <param name="b">        a {@code boolean}. </param>
		''' <returns>     a reference to this object. </returns>
		''' <exception cref="StringIndexOutOfBoundsException">  if the offset is invalid. </exception>
		public AbstractStringBuilder insert(Integer offset, Boolean b)
			Return insert(offset, Convert.ToString(b))

		''' <summary>
		''' Inserts the string representation of the {@code char}
		''' argument into this sequence.
		''' <p>
		''' The overall effect is exactly as if the second argument were
		''' converted to a string by the method <seealso cref="String#valueOf(char)"/>,
		''' and the character in that string were then
		''' <seealso cref="#insert(int,String) inserted"/> into this character
		''' sequence at the indicated offset.
		''' <p>
		''' The {@code offset} argument must be greater than or equal to
		''' {@code 0}, and less than or equal to the <seealso cref="#length() length"/>
		''' of this sequence.
		''' </summary>
		''' <param name="offset">   the offset. </param>
		''' <param name="c">        a {@code char}. </param>
		''' <returns>     a reference to this object. </returns>
		''' <exception cref="IndexOutOfBoundsException">  if the offset is invalid. </exception>
		public AbstractStringBuilder insert(Integer offset, Char c)
			ensureCapacityInternal(count + 1)
			Array.Copy(value_Renamed, offset, value_Renamed, offset + 1, count - offset)
			value_Renamed(offset) = c
			count += 1
			Return Me

		''' <summary>
		''' Inserts the string representation of the second {@code int}
		''' argument into this sequence.
		''' <p>
		''' The overall effect is exactly as if the second argument were
		''' converted to a string by the method <seealso cref="String#valueOf(int)"/>,
		''' and the characters of that string were then
		''' <seealso cref="#insert(int,String) inserted"/> into this character
		''' sequence at the indicated offset.
		''' <p>
		''' The {@code offset} argument must be greater than or equal to
		''' {@code 0}, and less than or equal to the <seealso cref="#length() length"/>
		''' of this sequence.
		''' </summary>
		''' <param name="offset">   the offset. </param>
		''' <param name="i">        an {@code int}. </param>
		''' <returns>     a reference to this object. </returns>
		''' <exception cref="StringIndexOutOfBoundsException">  if the offset is invalid. </exception>
		public AbstractStringBuilder insert(Integer offset, Integer i)
			Return insert(offset, Convert.ToString(i))

		''' <summary>
		''' Inserts the string representation of the {@code long}
		''' argument into this sequence.
		''' <p>
		''' The overall effect is exactly as if the second argument were
		''' converted to a string by the method <seealso cref="String#valueOf(long)"/>,
		''' and the characters of that string were then
		''' <seealso cref="#insert(int,String) inserted"/> into this character
		''' sequence at the indicated offset.
		''' <p>
		''' The {@code offset} argument must be greater than or equal to
		''' {@code 0}, and less than or equal to the <seealso cref="#length() length"/>
		''' of this sequence.
		''' </summary>
		''' <param name="offset">   the offset. </param>
		''' <param name="l">        a {@code long}. </param>
		''' <returns>     a reference to this object. </returns>
		''' <exception cref="StringIndexOutOfBoundsException">  if the offset is invalid. </exception>
		public AbstractStringBuilder insert(Integer offset, Long l)
			Return insert(offset, Convert.ToString(l))

		''' <summary>
		''' Inserts the string representation of the {@code float}
		''' argument into this sequence.
		''' <p>
		''' The overall effect is exactly as if the second argument were
		''' converted to a string by the method <seealso cref="String#valueOf(float)"/>,
		''' and the characters of that string were then
		''' <seealso cref="#insert(int,String) inserted"/> into this character
		''' sequence at the indicated offset.
		''' <p>
		''' The {@code offset} argument must be greater than or equal to
		''' {@code 0}, and less than or equal to the <seealso cref="#length() length"/>
		''' of this sequence.
		''' </summary>
		''' <param name="offset">   the offset. </param>
		''' <param name="f">        a {@code float}. </param>
		''' <returns>     a reference to this object. </returns>
		''' <exception cref="StringIndexOutOfBoundsException">  if the offset is invalid. </exception>
		public AbstractStringBuilder insert(Integer offset, Single f)
			Return insert(offset, Convert.ToString(f))

		''' <summary>
		''' Inserts the string representation of the {@code double}
		''' argument into this sequence.
		''' <p>
		''' The overall effect is exactly as if the second argument were
		''' converted to a string by the method <seealso cref="String#valueOf(double)"/>,
		''' and the characters of that string were then
		''' <seealso cref="#insert(int,String) inserted"/> into this character
		''' sequence at the indicated offset.
		''' <p>
		''' The {@code offset} argument must be greater than or equal to
		''' {@code 0}, and less than or equal to the <seealso cref="#length() length"/>
		''' of this sequence.
		''' </summary>
		''' <param name="offset">   the offset. </param>
		''' <param name="d">        a {@code double}. </param>
		''' <returns>     a reference to this object. </returns>
		''' <exception cref="StringIndexOutOfBoundsException">  if the offset is invalid. </exception>
		public AbstractStringBuilder insert(Integer offset, Double d)
			Return insert(offset, Convert.ToString(d))

		''' <summary>
		''' Returns the index within this string of the first occurrence of the
		''' specified substring. The integer returned is the smallest value
		''' <i>k</i> such that:
		''' <pre>{@code
		''' this.toString().startsWith(str, <i>k</i>)
		''' }</pre>
		''' is {@code true}.
		''' </summary>
		''' <param name="str">   any string. </param>
		''' <returns>  if the string argument occurs as a substring within this
		'''          object, then the index of the first character of the first
		'''          such substring is returned; if it does not occur as a
		'''          substring, {@code -1} is returned. </returns>
		public Integer IndexOf(String str)
			Return IndexOf(str, 0)

		''' <summary>
		''' Returns the index within this string of the first occurrence of the
		''' specified substring, starting at the specified index.  The integer
		''' returned is the smallest value {@code k} for which:
		''' <pre>{@code
		'''     k >= System.Math.min(fromIndex, this.length()) &&
		'''                   this.toString().startsWith(str, k)
		''' }</pre>
		''' If no such value of <i>k</i> exists, then -1 is returned.
		''' </summary>
		''' <param name="str">         the substring for which to search. </param>
		''' <param name="fromIndex">   the index from which to start the search. </param>
		''' <returns>  the index within this string of the first occurrence of the
		'''          specified substring, starting at the specified index. </returns>
		public Integer IndexOf(String str, Integer fromIndex)
			Return String.IndexOf(value_Renamed, 0, count, str, fromIndex)

		''' <summary>
		''' Returns the index within this string of the rightmost occurrence
		''' of the specified substring.  The rightmost empty string "" is
		''' considered to occur at the index value {@code this.length()}.
		''' The returned index is the largest value <i>k</i> such that
		''' <pre>{@code
		''' this.toString().startsWith(str, k)
		''' }</pre>
		''' is true.
		''' </summary>
		''' <param name="str">   the substring to search for. </param>
		''' <returns>  if the string argument occurs one or more times as a substring
		'''          within this object, then the index of the first character of
		'''          the last such substring is returned. If it does not occur as
		'''          a substring, {@code -1} is returned. </returns>
		public Integer LastIndexOf(String str)
			Return LastIndexOf(str, count)

		''' <summary>
		''' Returns the index within this string of the last occurrence of the
		''' specified substring. The integer returned is the largest value <i>k</i>
		''' such that:
		''' <pre>{@code
		'''     k <= System.Math.min(fromIndex, this.length()) &&
		'''                   this.toString().startsWith(str, k)
		''' }</pre>
		''' If no such value of <i>k</i> exists, then -1 is returned.
		''' </summary>
		''' <param name="str">         the substring to search for. </param>
		''' <param name="fromIndex">   the index to start the search from. </param>
		''' <returns>  the index within this sequence of the last occurrence of the
		'''          specified substring. </returns>
		public Integer LastIndexOf(String str, Integer fromIndex)
			Return String.LastIndexOf(value_Renamed, 0, count, str, fromIndex)

		''' <summary>
		''' Causes this character sequence to be replaced by the reverse of
		''' the sequence. If there are any surrogate pairs included in the
		''' sequence, these are treated as single characters for the
		''' reverse operation. Thus, the order of the high-low surrogates
		''' is never reversed.
		''' 
		''' Let <i>n</i> be the character length of this character sequence
		''' (not the length in {@code char} values) just prior to
		''' execution of the {@code reverse} method. Then the
		''' character at index <i>k</i> in the new character sequence is
		''' equal to the character at index <i>n-k-1</i> in the old
		''' character sequence.
		''' 
		''' <p>Note that the reverse operation may result in producing
		''' surrogate pairs that were unpaired low-surrogates and
		''' high-surrogates before the operation. For example, reversing
		''' "\u005CuDC00\u005CuD800" produces "\u005CuD800\u005CuDC00" which is
		''' a valid surrogate pair.
		''' </summary>
		''' <returns>  a reference to this object. </returns>
		public AbstractStringBuilder reverse()
			Dim hasSurrogates As Boolean = False
			Dim n As Integer = count - 1
			For j As Integer = (n-1) >> 1 To 0 Step -1
				Dim k As Integer = n - j
				Dim cj As Char = value_Renamed(j)
				Dim ck As Char = value_Renamed(k)
				value_Renamed(j) = ck
				value_Renamed(k) = cj
				If Character.isSurrogate(cj) OrElse Character.isSurrogate(ck) Then hasSurrogates = True
			Next j
			If hasSurrogates Then reverseAllValidSurrogatePairs()
			Return Me

		''' <summary>
		''' Outlined helper method for reverse() </summary>
		private  Sub  reverseAllValidSurrogatePairs()
			For i As Integer = 0 To count - 2
				Dim c2 As Char = value_Renamed(i)
				If Char.IsLowSurrogate(c2) Then
					Dim c1 As Char = value_Renamed(i + 1)
					If Char.IsHighSurrogate(c1) Then
						value_Renamed(i) = c1
						i += 1
						value_Renamed(i) = c2
					End If
				End If
			Next i

		''' <summary>
		''' Returns a string representing the data in this sequence.
		''' A new {@code String} object is allocated and initialized to
		''' contain the character sequence currently represented by this
		''' object. This {@code String} is then returned. Subsequent
		''' changes to this sequence do not affect the contents of the
		''' {@code String}.
		''' </summary>
		''' <returns>  a string representation of this sequence of characters. </returns>
		Public MustOverride String ToString()

		''' <summary>
		''' Needed by {@code String} for the contentEquals method.
		''' </summary>
		final Char() value
			Return value_Renamed

	End Class

End Namespace
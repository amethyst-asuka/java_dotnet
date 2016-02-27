Imports System

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
	''' A <code>ChoiceFormat</code> allows you to attach a format to a range of numbers.
	''' It is generally used in a <code>MessageFormat</code> for handling plurals.
	''' The choice is specified with an ascending list of doubles, where each item
	''' specifies a half-open interval up to the next item:
	''' <blockquote>
	''' <pre>
	''' X matches j if and only if limit[j] &le; X &lt; limit[j+1]
	''' </pre>
	''' </blockquote>
	''' If there is no match, then either the first or last index is used, depending
	''' on whether the number (X) is too low or too high.  If the limit array is not
	''' in ascending order, the results of formatting will be incorrect.  ChoiceFormat
	''' also accepts <code>&#92;u221E</code> as equivalent to infinity(INF).
	''' 
	''' <p>
	''' <strong>Note:</strong>
	''' <code>ChoiceFormat</code> differs from the other <code>Format</code>
	''' classes in that you create a <code>ChoiceFormat</code> object with a
	''' constructor (not with a <code>getInstance</code> style factory
	''' method). The factory methods aren't necessary because <code>ChoiceFormat</code>
	''' doesn't require any complex setup for a given locale. In fact,
	''' <code>ChoiceFormat</code> doesn't implement any locale specific behavior.
	''' 
	''' <p>
	''' When creating a <code>ChoiceFormat</code>, you must specify an array of formats
	''' and an array of limits. The length of these arrays must be the same.
	''' For example,
	''' <ul>
	''' <li>
	'''     <em>limits</em> = {1,2,3,4,5,6,7}<br>
	'''     <em>formats</em> = {"Sun","Mon","Tue","Wed","Thur","Fri","Sat"}
	''' <li>
	'''     <em>limits</em> = {0, 1, ChoiceFormat.nextDouble(1)}<br>
	'''     <em>formats</em> = {"no files", "one file", "many files"}<br>
	'''     (<code>nextDouble</code> can be used to get the next higher double, to
	'''     make the half-open interval.)
	''' </ul>
	''' 
	''' <p>
	''' Here is a simple example that shows formatting and parsing:
	''' <blockquote>
	''' <pre>{@code
	''' double[] limits = {1,2,3,4,5,6,7};
	''' String[] dayOfWeekNames = {"Sun","Mon","Tue","Wed","Thur","Fri","Sat"};
	''' ChoiceFormat form = new ChoiceFormat(limits, dayOfWeekNames);
	''' ParsePosition status = new ParsePosition(0);
	''' for (double i = 0.0; i <= 8.0; ++i) {
	'''     status.setIndex(0);
	'''     System.out.println(i + " -> " + form.format(i) + " -> "
	'''                              + form.parse(form.format(i),status));
	''' }
	''' }</pre>
	''' </blockquote>
	''' Here is a more complex example, with a pattern format:
	''' <blockquote>
	''' <pre>{@code
	''' double[] filelimits = {0,1,2};
	''' String[] filepart = {"are no files","is one file","are {2} files"};
	''' ChoiceFormat fileform = new ChoiceFormat(filelimits, filepart);
	''' Format[] testFormats = {fileform, null, NumberFormat.getInstance()};
	''' MessageFormat pattform = new MessageFormat("There {0} on {1}");
	''' pattform.setFormats(testFormats);
	''' Object[] testArgs = {null, "ADisk", null};
	''' for (int i = 0; i < 4; ++i) {
	'''     testArgs[0] = new Integer(i);
	'''     testArgs[2] = testArgs[0];
	'''     System.out.println(pattform.format(testArgs));
	''' }
	''' }</pre>
	''' </blockquote>
	''' <p>
	''' Specifying a pattern for ChoiceFormat objects is fairly straightforward.
	''' For example:
	''' <blockquote>
	''' <pre>{@code
	''' ChoiceFormat fmt = new ChoiceFormat(
	'''      "-1#is negative| 0#is zero or fraction | 1#is one |1.0<is 1+ |2#is two |2<is more than 2.");
	''' System.out.println("Formatter Pattern : " + fmt.toPattern());
	''' 
	''' System.out.println("Format with -INF : " + fmt.format(Double.NEGATIVE_INFINITY));
	''' System.out.println("Format with -1.0 : " + fmt.format(-1.0));
	''' System.out.println("Format with 0 : " + fmt.format(0));
	''' System.out.println("Format with 0.9 : " + fmt.format(0.9));
	''' System.out.println("Format with 1.0 : " + fmt.format(1));
	''' System.out.println("Format with 1.5 : " + fmt.format(1.5));
	''' System.out.println("Format with 2 : " + fmt.format(2));
	''' System.out.println("Format with 2.1 : " + fmt.format(2.1));
	''' System.out.println("Format with NaN : " + fmt.format(Double.NaN));
	''' System.out.println("Format with +INF : " + fmt.format(Double.POSITIVE_INFINITY));
	''' }</pre>
	''' </blockquote>
	''' And the output result would be like the following:
	''' <blockquote>
	''' <pre>{@code
	''' Format with -INF : is negative
	''' Format with -1.0 : is negative
	''' Format with 0 : is zero or fraction
	''' Format with 0.9 : is zero or fraction
	''' Format with 1.0 : is one
	''' Format with 1.5 : is 1+
	''' Format with 2 : is two
	''' Format with 2.1 : is more than 2.
	''' Format with NaN : is negative
	''' Format with +INF : is more than 2.
	''' }</pre>
	''' </blockquote>
	''' 
	''' <h3><a name="synchronization">Synchronization</a></h3>
	''' 
	''' <p>
	''' Choice formats are not synchronized.
	''' It is recommended to create separate format instances for each thread.
	''' If multiple threads access a format concurrently, it must be synchronized
	''' externally.
	''' 
	''' </summary>
	''' <seealso cref=          DecimalFormat </seealso>
	''' <seealso cref=          MessageFormat
	''' @author       Mark Davis </seealso>
	Public Class ChoiceFormat
		Inherits NumberFormat

		' Proclaim serial compatibility with 1.1 FCS
		Private Shadows Const serialVersionUID As Long = 1795184449645032964L

		''' <summary>
		''' Sets the pattern. </summary>
		''' <param name="newPattern"> See the class description. </param>
		Public Overridable Sub applyPattern(ByVal newPattern As String)
			Dim segments As StringBuffer() = New StringBuffer(1){}
			For i As Integer = 0 To segments.Length - 1
				segments(i) = New StringBuffer
			Next i
			Dim newChoiceLimits As Double() = New Double(29){}
			Dim newChoiceFormats As String() = New String(29){}
			Dim count As Integer = 0
			Dim part As Integer = 0
			Dim startValue As Double = 0
			Dim oldStartValue As Double = java.lang.[Double].NaN
			Dim inQuote As Boolean = False
			For i As Integer = 0 To newPattern.length() - 1
				Dim ch As Char = newPattern.Chars(i)
				If ch="'"c Then
					' Check for "''" indicating a literal quote
					If (i+1)<newPattern.length() AndAlso newPattern.Chars(i+1)=ch Then
						segments(part).append(ch)
						i += 1
					Else
						inQuote = Not inQuote
					End If
				ElseIf inQuote Then
					segments(part).append(ch)
				ElseIf ch = "<"c OrElse ch = "#"c OrElse ch = ChrW(&H2264) Then
					If segments(0).length() = 0 Then Throw New IllegalArgumentException
					Try
						Dim tempBuffer As String = segments(0).ToString()
						If tempBuffer.Equals(ChrW(&H221E).ToString()) Then
							startValue = java.lang.[Double].PositiveInfinity
						ElseIf tempBuffer.Equals("-" & ChrW(&H221E).ToString()) Then
							startValue = java.lang.[Double].NegativeInfinity
						Else
							startValue = Convert.ToDouble(segments(0).ToString())
						End If
					Catch e As Exception
						Throw New IllegalArgumentException
					End Try
					If ch = "<"c AndAlso startValue <> java.lang.[Double].PositiveInfinity AndAlso startValue <> java.lang.[Double].NegativeInfinity Then startValue = nextDouble(startValue)
					If startValue <= oldStartValue Then Throw New IllegalArgumentException
					segments(0).length = 0
					part = 1
				ElseIf ch = "|"c Then
					If count = newChoiceLimits.Length Then
						newChoiceLimits = doubleArraySize(newChoiceLimits)
						newChoiceFormats = doubleArraySize(newChoiceFormats)
					End If
					newChoiceLimits(count) = startValue
					newChoiceFormats(count) = segments(1).ToString()
					count += 1
					oldStartValue = startValue
					segments(1).length = 0
					part = 0
				Else
					segments(part).append(ch)
				End If
			Next i
			' clean up last one
			If part = 1 Then
				If count = newChoiceLimits.Length Then
					newChoiceLimits = doubleArraySize(newChoiceLimits)
					newChoiceFormats = doubleArraySize(newChoiceFormats)
				End If
				newChoiceLimits(count) = startValue
				newChoiceFormats(count) = segments(1).ToString()
				count += 1
			End If
			choiceLimits = New Double(count - 1){}
			Array.Copy(newChoiceLimits, 0, choiceLimits, 0, count)
			choiceFormats = New String(count - 1){}
			Array.Copy(newChoiceFormats, 0, choiceFormats, 0, count)
		End Sub

		''' <summary>
		''' Gets the pattern.
		''' </summary>
		''' <returns> the pattern string </returns>
		Public Overridable Function toPattern() As String
			Dim result As New StringBuffer
			For i As Integer = 0 To choiceLimits.Length - 1
				If i <> 0 Then result.append("|"c)
				' choose based upon which has less precision
				' approximate that by choosing the closest one to an  java.lang.[Integer].
				' could do better, but it's not worth it.
				Dim less As Double = previousDouble(choiceLimits(i))
				Dim tryLessOrEqual As Double = System.Math.Abs (System.Math.IEEERemainder(choiceLimits(i), 1.0R))
				Dim tryLess As Double = System.Math.Abs (System.Math.IEEERemainder(less, 1.0R))

				If tryLessOrEqual < tryLess Then
					result.append("" & choiceLimits(i))
					result.append("#"c)
				Else
					If choiceLimits(i) = java.lang.[Double].PositiveInfinity Then
						result.append(ChrW(&H221E).ToString())
					ElseIf choiceLimits(i) = java.lang.[Double].NegativeInfinity Then
						result.append("-" & ChrW(&H221E).ToString())
					Else
						result.append("" & less)
					End If
					result.append("<"c)
				End If
				' Append choiceFormats[i], using quotes if there are special characters.
				' Single quotes themselves must be escaped in either case.
				Dim text As String = choiceFormats(i)
				Dim needQuote As Boolean = text.IndexOf("<"c) >= 0 OrElse text.IndexOf("#"c) >= 0 OrElse text.IndexOf(ChrW(&H2264)) >= 0 OrElse text.IndexOf("|"c) >= 0
				If needQuote Then result.append("'"c)
				If text.IndexOf("'"c) < 0 Then
					result.append(text)
				Else
					For j As Integer = 0 To text.length() - 1
						Dim c As Char = text.Chars(j)
						result.append(c)
						If c = "'"c Then result.append(c)
					Next j
				End If
				If needQuote Then result.append("'"c)
			Next i
			Return result.ToString()
		End Function

		''' <summary>
		''' Constructs with limits and corresponding formats based on the pattern.
		''' </summary>
		''' <param name="newPattern"> the new pattern string </param>
		''' <seealso cref= #applyPattern </seealso>
		Public Sub New(ByVal newPattern As String)
			applyPattern(newPattern)
		End Sub

		''' <summary>
		''' Constructs with the limits and the corresponding formats.
		''' </summary>
		''' <param name="limits"> limits in ascending order </param>
		''' <param name="formats"> corresponding format strings </param>
		''' <seealso cref= #setChoices </seealso>
		Public Sub New(ByVal limits As Double(), ByVal formats As String())
			choicesces(limits, formats)
		End Sub

		''' <summary>
		''' Set the choices to be used in formatting. </summary>
		''' <param name="limits"> contains the top value that you want
		''' parsed with that format, and should be in ascending sorted order. When
		''' formatting X, the choice will be the i, where
		''' limit[i] &le; X {@literal <} limit[i+1].
		''' If the limit array is not in ascending order, the results of formatting
		''' will be incorrect. </param>
		''' <param name="formats"> are the formats you want to use for each limit.
		''' They can be either Format objects or Strings.
		''' When formatting with object Y,
		''' if the object is a NumberFormat, then ((NumberFormat) Y).format(X)
		''' is called. Otherwise Y.toString() is called. </param>
		Public Overridable Sub setChoices(ByVal limits As Double(), ByVal formats As String())
			If limits.Length <> formats.Length Then Throw New IllegalArgumentException("Array and limit arrays must be of the same length.")
			choiceLimits = java.util.Arrays.copyOf(limits, limits.Length)
			choiceFormats = java.util.Arrays.copyOf(formats, formats.Length)
		End Sub

		''' <summary>
		''' Get the limits passed in the constructor. </summary>
		''' <returns> the limits. </returns>
		Public Overridable Property limits As Double()
			Get
				Dim newLimits As Double() = java.util.Arrays.copyOf(choiceLimits, choiceLimits.Length)
				Return newLimits
			End Get
		End Property

		''' <summary>
		''' Get the formats passed in the constructor. </summary>
		''' <returns> the formats. </returns>
		Public Overridable Property formats As Object()
			Get
				Dim newFormats As Object() = java.util.Arrays.copyOf(choiceFormats, choiceFormats.Length)
				Return newFormats
			End Get
		End Property

		' Overrides

		''' <summary>
		''' Specialization of format. This method really calls
		''' <code>format(double, StringBuffer, FieldPosition)</code>
		''' thus the range of longs that are supported is only equal to
		''' the range that can be stored by java.lang.[Double]. This will never be
		''' a practical limitation.
		''' </summary>
		Public Overrides Function format(ByVal number As Long, ByVal toAppendTo As StringBuffer, ByVal status As FieldPosition) As StringBuffer
			Return format(CDbl(number), toAppendTo, status)
		End Function

		''' <summary>
		''' Returns pattern with formatted java.lang.[Double]. </summary>
		''' <param name="number"> number to be formatted and substituted. </param>
		''' <param name="toAppendTo"> where text is appended. </param>
		''' <param name="status"> ignore no useful status is returned. </param>
	   Public Overrides Function format(ByVal number As Double, ByVal toAppendTo As StringBuffer, ByVal status As FieldPosition) As StringBuffer
			' find the number
			Dim i As Integer
			For i = 0 To choiceLimits.Length - 1
				If Not(number >= choiceLimits(i)) Then Exit For
			Next i
			i -= 1
			If i < 0 Then i = 0
			' return either a formatted number, or a string
			Return toAppendTo.append(choiceFormats(i))
	   End Function

		''' <summary>
		''' Parses a Number from the input text. </summary>
		''' <param name="text"> the source text. </param>
		''' <param name="status"> an input-output parameter.  On input, the
		''' status.index field indicates the first character of the
		''' source text that should be parsed.  On exit, if no error
		''' occurred, status.index is set to the first unparsed character
		''' in the source text.  On exit, if an error did occur,
		''' status.index is unchanged and status.errorIndex is set to the
		''' first index of the character that caused the parse to fail. </param>
		''' <returns> A Number representing the value of the number parsed. </returns>
		Public Overrides Function parse(ByVal text As String, ByVal status As ParsePosition) As Number
			' find the best number (defined as the one with the longest parse)
			Dim start As Integer = status.index
			Dim furthest As Integer = start
			Dim bestNumber As Double = java.lang.[Double].NaN
			Dim tempNumber As Double = 0.0
			For i As Integer = 0 To choiceFormats.Length - 1
				Dim tempString As String = choiceFormats(i)
				If text.regionMatches(start, tempString, 0, tempString.length()) Then
					status.index = start + tempString.length()
					tempNumber = choiceLimits(i)
					If status.index > furthest Then
						furthest = status.index
						bestNumber = tempNumber
						If furthest = text.length() Then Exit For
					End If
				End If
			Next i
			status.index = furthest
			If status.index = start Then status.errorIndex = furthest
			Return New Double?(bestNumber)
		End Function

		''' <summary>
		''' Finds the least double greater than {@code d}.
		''' If {@code NaN}, returns same value.
		''' <p>Used to make half-open intervals.
		''' </summary>
		''' <param name="d"> the reference value </param>
		''' <returns> the least double value greather than {@code d} </returns>
		''' <seealso cref= #previousDouble </seealso>
		Public Shared Function nextDouble(ByVal d As Double) As Double
			Return nextDouble(d,True)
		End Function

		''' <summary>
		''' Finds the greatest double less than {@code d}.
		''' If {@code NaN}, returns same value.
		''' </summary>
		''' <param name="d"> the reference value </param>
		''' <returns> the greatest double value less than {@code d} </returns>
		''' <seealso cref= #nextDouble </seealso>
		Public Shared Function previousDouble(ByVal d As Double) As Double
			Return nextDouble(d,False)
		End Function

		''' <summary>
		''' Overrides Cloneable
		''' </summary>
		Public Overrides Function clone() As Object
			Dim other As ChoiceFormat = CType(MyBase.clone(), ChoiceFormat)
			' for primitives or immutables, shallow clone is enough
			other.choiceLimits = choiceLimits.clone()
			other.choiceFormats = choiceFormats.clone()
			Return other
		End Function

		''' <summary>
		''' Generates a hash code for the message format object.
		''' </summary>
		Public Overrides Function GetHashCode() As Integer
			Dim result As Integer = choiceLimits.Length
			If choiceFormats.Length > 0 Then result = result Xor choiceFormats(choiceFormats.Length-1).GetHashCode()
			Return result
		End Function

		''' <summary>
		''' Equality comparision between two
		''' </summary>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			If obj Is Nothing Then Return False
			If Me Is obj Then ' quick check Return True
			If Me.GetType() IsNot obj.GetType() Then Return False
			Dim other As ChoiceFormat = CType(obj, ChoiceFormat)
			Return (java.util.Arrays.Equals(choiceLimits, other.choiceLimits) AndAlso java.util.Arrays.Equals(choiceFormats, other.choiceFormats))
		End Function

		''' <summary>
		''' After reading an object from the input stream, do a simple verification
		''' to maintain class invariants. </summary>
		''' <exception cref="InvalidObjectException"> if the objects read from the stream is invalid. </exception>
		Private Sub readObject(ByVal [in] As java.io.ObjectInputStream)
			[in].defaultReadObject()
			If choiceLimits.Length <> choiceFormats.Length Then Throw New java.io.InvalidObjectException("limits and format arrays of different length.")
		End Sub

		' ===============privates===========================

		''' <summary>
		''' A list of lower bounds for the choices.  The formatter will return
		''' <code>choiceFormats[i]</code> if the number being formatted is greater than or equal to
		''' <code>choiceLimits[i]</code> and less than <code>choiceLimits[i+1]</code>.
		''' @serial
		''' </summary>
		Private choiceLimits As Double()

		''' <summary>
		''' A list of choice strings.  The formatter will return
		''' <code>choiceFormats[i]</code> if the number being formatted is greater than or equal to
		''' <code>choiceLimits[i]</code> and less than <code>choiceLimits[i+1]</code>.
		''' @serial
		''' </summary>
		Private choiceFormats As String()

	'    
	'    static final long SIGN          = 0x8000000000000000L;
	'    static final long EXPONENT      = 0x7FF0000000000000L;
	'    static final long SIGNIFICAND   = 0x000FFFFFFFFFFFFFL;
	'
	'    private static double nextDouble (double d, boolean positive) {
	'        if (Double.isNaN(d) || java.lang.[Double].isInfinite(d)) {
	'                return d;
	'            }
	'        long bits = java.lang.[Double].doubleToLongBits(d);
	'        long significand = bits & SIGNIFICAND;
	'        if (bits < 0) {
	'            significand |= (SIGN | EXPONENT);
	'        }
	'        long exponent = bits & EXPONENT;
	'        if (positive) {
	'            significand += 1;
	'            // FIXME fix overflow & underflow
	'        } else {
	'            significand -= 1;
	'            // FIXME fix overflow & underflow
	'        }
	'        bits = exponent | (significand & ~EXPONENT);
	'        return java.lang.[Double].longBitsToDouble(bits);
	'    }
	'    

		Friend Const SIGN As Long = &H8000000000000000L
		Friend Const EXPONENT As Long = &H7FF0000000000000L
		Friend Const POSITIVEINFINITY As Long = &H7FF0000000000000L

		''' <summary>
		''' Finds the least double greater than {@code d} (if {@code positive} is
		''' {@code true}), or the greatest double less than {@code d} (if
		''' {@code positive} is {@code false}).
		''' If {@code NaN}, returns same value.
		''' 
		''' Does not affect floating-point flags,
		''' provided these member functions do not:
		'''          java.lang.[Double].longBitsToDouble(long)
		'''          java.lang.[Double].doubleToLongBits(double)
		'''          java.lang.[Double].isNaN(double)
		''' </summary>
		''' <param name="d">        the reference value </param>
		''' <param name="positive"> {@code true} if the least double is desired;
		'''                 {@code false} otherwise </param>
		''' <returns> the least or greater double value </returns>
		Public Shared Function nextDouble(ByVal d As Double, ByVal positive As Boolean) As Double

			' filter out NaN's 
			If java.lang.[Double].IsNaN(d) Then Return d

			' zero's are also a special case 
			If d = 0.0 Then
				Dim smallestPositiveDouble As Double = java.lang.[Double].longBitsToDouble(1L)
				If positive Then
					Return smallestPositiveDouble
				Else
					Return -smallestPositiveDouble
				End If
			End If

			' if entering here, d is a nonzero value 

			' hold all bits in a long for later use 
			Dim bits As Long = java.lang.[Double].doubleToLongBits(d)

			' strip off the sign bit 
			Dim magnitude As Long = bits And Not SIGN

			' if next double away from zero, increase magnitude 
			If (bits > 0) = positive Then
				If magnitude <> POSITIVEINFINITY Then magnitude += 1
			' else decrease magnitude 
			Else
				magnitude -= 1
			End If

			' restore sign bit and return 
			Dim signbit As Long = bits And SIGN
			Return java.lang.[Double].longBitsToDouble(magnitude Or signbit)
		End Function

		Private Shared Function doubleArraySize(ByVal array As Double()) As Double()
			Dim oldSize As Integer = array.Length
			Dim newArray As Double() = New Double(oldSize * 2 - 1){}
			Array.Copy(array, 0, newArray, 0, oldSize)
			Return newArray
		End Function

		Private Function doubleArraySize(ByVal array As String()) As String()
			Dim oldSize As Integer = array.Length
			Dim newArray As String() = New String(oldSize * 2 - 1){}
			Array.Copy(array, 0, newArray, 0, oldSize)
			Return newArray
		End Function

	End Class

End Namespace
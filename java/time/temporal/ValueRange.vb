Imports Microsoft.VisualBasic
Imports System

'
' * Copyright (c) 2012, 2013, Oracle and/or its affiliates. All rights reserved.
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
' *
' *
' *
' *
' *
' * Copyright (c) 2011-2012, Stephen Colebourne & Michael Nascimento Santos
' *
' * All rights reserved.
' *
' * Redistribution and use in source and binary forms, with or without
' * modification, are permitted provided that the following conditions are met:
' *
' *  * Redistributions of source code must retain the above copyright notice,
' *    this list of conditions and the following disclaimer.
' *
' *  * Redistributions in binary form must reproduce the above copyright notice,
' *    this list of conditions and the following disclaimer in the documentation
' *    and/or other materials provided with the distribution.
' *
' *  * Neither the name of JSR-310 nor the names of its contributors
' *    may be used to endorse or promote products derived from this software
' *    without specific prior written permission.
' *
' * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
' * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
' * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
' * A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
' * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
' * EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
' * PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
' * PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
' * LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
' * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
' * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
' 
Namespace java.time.temporal


	''' <summary>
	''' The range of valid values for a date-time field.
	''' <p>
	''' All <seealso cref="TemporalField"/> instances have a valid range of values.
	''' For example, the ISO day-of-month runs from 1 to somewhere between 28 and 31.
	''' This class captures that valid range.
	''' <p>
	''' It is important to be aware of the limitations of this class.
	''' Only the minimum and maximum values are provided.
	''' It is possible for there to be invalid values within the outer range.
	''' For example, a weird field may have valid values of 1, 2, 4, 6, 7, thus
	''' have a range of '1 - 7', despite that fact that values 3 and 5 are invalid.
	''' <p>
	''' Instances of this class are not tied to a specific field.
	''' 
	''' @implSpec
	''' This class is immutable and thread-safe.
	''' 
	''' @since 1.8
	''' </summary>
	<Serializable> _
	Public NotInheritable Class ValueRange

		''' <summary>
		''' Serialization version.
		''' </summary>
		Private Const serialVersionUID As Long = -7317881728594519368L

		''' <summary>
		''' The smallest minimum value.
		''' </summary>
		Private ReadOnly minSmallest As Long
		''' <summary>
		''' The largest minimum value.
		''' </summary>
		Private ReadOnly minLargest As Long
		''' <summary>
		''' The smallest maximum value.
		''' </summary>
		Private ReadOnly maxSmallest As Long
		''' <summary>
		''' The largest maximum value.
		''' </summary>
		Private ReadOnly maxLargest As Long

		''' <summary>
		''' Obtains a fixed value range.
		''' <p>
		''' This factory obtains a range where the minimum and maximum values are fixed.
		''' For example, the ISO month-of-year always runs from 1 to 12.
		''' </summary>
		''' <param name="min">  the minimum value </param>
		''' <param name="max">  the maximum value </param>
		''' <returns> the ValueRange for min, max, not null </returns>
		''' <exception cref="IllegalArgumentException"> if the minimum is greater than the maximum </exception>
		Public Shared Function [of](ByVal min As Long, ByVal max As Long) As ValueRange
			If min > max Then Throw New IllegalArgumentException("Minimum value must be less than maximum value")
			Return New ValueRange(min, min, max, max)
		End Function

		''' <summary>
		''' Obtains a variable value range.
		''' <p>
		''' This factory obtains a range where the minimum value is fixed and the maximum value may vary.
		''' For example, the ISO day-of-month always starts at 1, but ends between 28 and 31.
		''' </summary>
		''' <param name="min">  the minimum value </param>
		''' <param name="maxSmallest">  the smallest maximum value </param>
		''' <param name="maxLargest">  the largest maximum value </param>
		''' <returns> the ValueRange for min, smallest max, largest max, not null </returns>
		''' <exception cref="IllegalArgumentException"> if
		'''     the minimum is greater than the smallest maximum,
		'''  or the smallest maximum is greater than the largest maximum </exception>
		Public Shared Function [of](ByVal min As Long, ByVal maxSmallest As Long, ByVal maxLargest As Long) As ValueRange
			Return [of](min, min, maxSmallest, maxLargest)
		End Function

		''' <summary>
		''' Obtains a fully variable value range.
		''' <p>
		''' This factory obtains a range where both the minimum and maximum value may vary.
		''' </summary>
		''' <param name="minSmallest">  the smallest minimum value </param>
		''' <param name="minLargest">  the largest minimum value </param>
		''' <param name="maxSmallest">  the smallest maximum value </param>
		''' <param name="maxLargest">  the largest maximum value </param>
		''' <returns> the ValueRange for smallest min, largest min, smallest max, largest max, not null </returns>
		''' <exception cref="IllegalArgumentException"> if
		'''     the smallest minimum is greater than the smallest maximum,
		'''  or the smallest maximum is greater than the largest maximum
		'''  or the largest minimum is greater than the largest maximum </exception>
		Public Shared Function [of](ByVal minSmallest As Long, ByVal minLargest As Long, ByVal maxSmallest As Long, ByVal maxLargest As Long) As ValueRange
			If minSmallest > minLargest Then Throw New IllegalArgumentException("Smallest minimum value must be less than largest minimum value")
			If maxSmallest > maxLargest Then Throw New IllegalArgumentException("Smallest maximum value must be less than largest maximum value")
			If minLargest > maxLargest Then Throw New IllegalArgumentException("Minimum value must be less than maximum value")
			Return New ValueRange(minSmallest, minLargest, maxSmallest, maxLargest)
		End Function

		''' <summary>
		''' Restrictive constructor.
		''' </summary>
		''' <param name="minSmallest">  the smallest minimum value </param>
		''' <param name="minLargest">  the largest minimum value </param>
		''' <param name="maxSmallest">  the smallest minimum value </param>
		''' <param name="maxLargest">  the largest minimum value </param>
		Private Sub New(ByVal minSmallest As Long, ByVal minLargest As Long, ByVal maxSmallest As Long, ByVal maxLargest As Long)
			Me.minSmallest = minSmallest
			Me.minLargest = minLargest
			Me.maxSmallest = maxSmallest
			Me.maxLargest = maxLargest
		End Sub

		'-----------------------------------------------------------------------
		''' <summary>
		''' Is the value range fixed and fully known.
		''' <p>
		''' For example, the ISO day-of-month runs from 1 to between 28 and 31.
		''' Since there is uncertainty about the maximum value, the range is not fixed.
		''' However, for the month of January, the range is always 1 to 31, thus it is fixed.
		''' </summary>
		''' <returns> true if the set of values is fixed </returns>
		Public Property fixed As Boolean
			Get
				Return minSmallest = minLargest AndAlso maxSmallest = maxLargest
			End Get
		End Property

		'-----------------------------------------------------------------------
		''' <summary>
		''' Gets the minimum value that the field can take.
		''' <p>
		''' For example, the ISO day-of-month always starts at 1.
		''' The minimum is therefore 1.
		''' </summary>
		''' <returns> the minimum value for this field </returns>
		Public Property minimum As Long
			Get
				Return minSmallest
			End Get
		End Property

		''' <summary>
		''' Gets the largest possible minimum value that the field can take.
		''' <p>
		''' For example, the ISO day-of-month always starts at 1.
		''' The largest minimum is therefore 1.
		''' </summary>
		''' <returns> the largest possible minimum value for this field </returns>
		Public Property largestMinimum As Long
			Get
				Return minLargest
			End Get
		End Property

		''' <summary>
		''' Gets the smallest possible maximum value that the field can take.
		''' <p>
		''' For example, the ISO day-of-month runs to between 28 and 31 days.
		''' The smallest maximum is therefore 28.
		''' </summary>
		''' <returns> the smallest possible maximum value for this field </returns>
		Public Property smallestMaximum As Long
			Get
				Return maxSmallest
			End Get
		End Property

		''' <summary>
		''' Gets the maximum value that the field can take.
		''' <p>
		''' For example, the ISO day-of-month runs to between 28 and 31 days.
		''' The maximum is therefore 31.
		''' </summary>
		''' <returns> the maximum value for this field </returns>
		Public Property maximum As Long
			Get
				Return maxLargest
			End Get
		End Property

		'-----------------------------------------------------------------------
		''' <summary>
		''' Checks if all values in the range fit in an {@code int}.
		''' <p>
		''' This checks that all valid values are within the bounds of an {@code int}.
		''' <p>
		''' For example, the ISO month-of-year has values from 1 to 12, which fits in an {@code int}.
		''' By comparison, ISO nano-of-day runs from 1 to 86,400,000,000,000 which does not fit in an {@code int}.
		''' <p>
		''' This implementation uses <seealso cref="#getMinimum()"/> and <seealso cref="#getMaximum()"/>.
		''' </summary>
		''' <returns> true if a valid value always fits in an {@code int} </returns>
		Public Property intValue As Boolean
			Get
				Return minimum >= Integer.MinValue AndAlso maximum <= Integer.MaxValue
			End Get
		End Property

		''' <summary>
		''' Checks if the value is within the valid range.
		''' <p>
		''' This checks that the value is within the stored range of values.
		''' </summary>
		''' <param name="value">  the value to check </param>
		''' <returns> true if the value is valid </returns>
		Public Function isValidValue(ByVal value As Long) As Boolean
			Return (value >= minimum AndAlso value <= maximum)
		End Function

		''' <summary>
		''' Checks if the value is within the valid range and that all values
		''' in the range fit in an {@code int}.
		''' <p>
		''' This method combines <seealso cref="#isIntValue()"/> and <seealso cref="#isValidValue(long)"/>.
		''' </summary>
		''' <param name="value">  the value to check </param>
		''' <returns> true if the value is valid and fits in an {@code int} </returns>
		Public Function isValidIntValue(ByVal value As Long) As Boolean
			Return intValue AndAlso isValidValue(value)
		End Function

		''' <summary>
		''' Checks that the specified value is valid.
		''' <p>
		''' This validates that the value is within the valid range of values.
		''' The field is only used to improve the error message.
		''' </summary>
		''' <param name="value">  the value to check </param>
		''' <param name="field">  the field being checked, may be null </param>
		''' <returns> the value that was passed in </returns>
		''' <seealso cref= #isValidValue(long) </seealso>
		Public Function checkValidValue(ByVal value As Long, ByVal field As TemporalField) As Long
			If isValidValue(value) = False Then Throw New java.time.DateTimeException(genInvalidFieldMessage(field, value))
			Return value
		End Function

		''' <summary>
		''' Checks that the specified value is valid and fits in an {@code int}.
		''' <p>
		''' This validates that the value is within the valid range of values and that
		''' all valid values are within the bounds of an {@code int}.
		''' The field is only used to improve the error message.
		''' </summary>
		''' <param name="value">  the value to check </param>
		''' <param name="field">  the field being checked, may be null </param>
		''' <returns> the value that was passed in </returns>
		''' <seealso cref= #isValidIntValue(long) </seealso>
		Public Function checkValidIntValue(ByVal value As Long, ByVal field As TemporalField) As Integer
			If isValidIntValue(value) = False Then Throw New java.time.DateTimeException(genInvalidFieldMessage(field, value))
			Return CInt(value)
		End Function

		Private Function genInvalidFieldMessage(ByVal field As TemporalField, ByVal value As Long) As String
			If field IsNot Nothing Then
				Return "Invalid value for " & field & " (valid values " & Me & "): " & value
			Else
				Return "Invalid value (valid values " & Me & "): " & value
			End If
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Restore the state of an ValueRange from the stream.
		''' Check that the values are valid.
		''' </summary>
		''' <param name="s"> the stream to read </param>
		''' <exception cref="InvalidObjectException"> if
		'''     the smallest minimum is greater than the smallest maximum,
		'''  or the smallest maximum is greater than the largest maximum
		'''  or the largest minimum is greater than the largest maximum </exception>
		''' <exception cref="ClassNotFoundException"> if a class cannot be resolved </exception>
		Private Sub readObject(ByVal s As java.io.ObjectInputStream)
			s.defaultReadObject()
			If minSmallest > minLargest Then Throw New java.io.InvalidObjectException("Smallest minimum value must be less than largest minimum value")
			If maxSmallest > maxLargest Then Throw New java.io.InvalidObjectException("Smallest maximum value must be less than largest maximum value")
			If minLargest > maxLargest Then Throw New java.io.InvalidObjectException("Minimum value must be less than maximum value")
		End Sub

		'-----------------------------------------------------------------------
		''' <summary>
		''' Checks if this range is equal to another range.
		''' <p>
		''' The comparison is based on the four values, minimum, largest minimum,
		''' smallest maximum and maximum.
		''' Only objects of type {@code ValueRange} are compared, other types return false.
		''' </summary>
		''' <param name="obj">  the object to check, null returns false </param>
		''' <returns> true if this is equal to the other range </returns>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			If obj Is Me Then Return True
			If TypeOf obj Is ValueRange Then
				Dim other As ValueRange = CType(obj, ValueRange)
			   Return minSmallest = other.minSmallest AndAlso minLargest = other.minLargest AndAlso maxSmallest = other.maxSmallest AndAlso maxLargest = other.maxLargest
			End If
			Return False
		End Function

		''' <summary>
		''' A hash code for this range.
		''' </summary>
		''' <returns> a suitable hash code </returns>
		Public Overrides Function GetHashCode() As Integer
			Dim hash As Long = minSmallest + minLargest << 16 + minLargest >> 48 + maxSmallest << 32 + maxSmallest >> 32 + maxLargest << 48 + maxLargest >> 16
			Return CInt(Fix(hash Xor (CLng(CULng(hash) >> 32))))
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Outputs this range as a {@code String}.
		''' <p>
		''' The format will be '{min}/{largestMin} - {smallestMax}/{max}',
		''' where the largestMin or smallestMax sections may be omitted, together
		''' with associated slash, if they are the same as the min or max.
		''' </summary>
		''' <returns> a string representation of this range, not null </returns>
		Public Overrides Function ToString() As String
			Dim buf As New StringBuilder
			buf.append(minSmallest)
			If minSmallest <> minLargest Then buf.append("/"c).append(minLargest)
			buf.append(" - ").append(maxSmallest)
			If maxSmallest <> maxLargest Then buf.append("/"c).append(maxLargest)
			Return buf.ToString()
		End Function

	End Class

End Namespace
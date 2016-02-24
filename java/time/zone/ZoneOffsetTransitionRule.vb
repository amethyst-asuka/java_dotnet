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
' * Copyright (c) 2009-2012, Stephen Colebourne & Michael Nascimento Santos
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
Namespace java.time.zone



	''' <summary>
	''' A rule expressing how to create a transition.
	''' <p>
	''' This class allows rules for identifying future transitions to be expressed.
	''' A rule might be written in many forms:
	''' <ul>
	''' <li>the 16th March
	''' <li>the Sunday on or after the 16th March
	''' <li>the Sunday on or before the 16th March
	''' <li>the last Sunday in February
	''' </ul>
	''' These different rule types can be expressed and queried.
	''' 
	''' @implSpec
	''' This class is immutable and thread-safe.
	''' 
	''' @since 1.8
	''' </summary>
	<Serializable> _
	Public NotInheritable Class ZoneOffsetTransitionRule

		''' <summary>
		''' Serialization version.
		''' </summary>
		Private Const serialVersionUID As Long = 6889046316657758795L

		''' <summary>
		''' The month of the month-day of the first day of the cutover week.
		''' The actual date will be adjusted by the dowChange field.
		''' </summary>
		Private ReadOnly month As java.time.Month
		''' <summary>
		''' The day-of-month of the month-day of the cutover week.
		''' If positive, it is the start of the week where the cutover can occur.
		''' If negative, it represents the end of the week where cutover can occur.
		''' The value is the number of days from the end of the month, such that
		''' {@code -1} is the last day of the month, {@code -2} is the second
		''' to last day, and so on.
		''' </summary>
		Private ReadOnly dom As SByte
		''' <summary>
		''' The cutover day-of-week, null to retain the day-of-month.
		''' </summary>
		Private ReadOnly dow As java.time.DayOfWeek
		''' <summary>
		''' The cutover time in the 'before' offset.
		''' </summary>
		Private ReadOnly time As java.time.LocalTime
		''' <summary>
		''' Whether the cutover time is midnight at the end of day.
		''' </summary>
		Private ReadOnly timeEndOfDay As Boolean
		''' <summary>
		''' The definition of how the local time should be interpreted.
		''' </summary>
		Private ReadOnly timeDefinition As TimeDefinition
		''' <summary>
		''' The standard offset at the cutover.
		''' </summary>
		Private ReadOnly standardOffset As java.time.ZoneOffset
		''' <summary>
		''' The offset before the cutover.
		''' </summary>
		Private ReadOnly offsetBefore As java.time.ZoneOffset
		''' <summary>
		''' The offset after the cutover.
		''' </summary>
		Private ReadOnly offsetAfter As java.time.ZoneOffset

		''' <summary>
		''' Obtains an instance defining the yearly rule to create transitions between two offsets.
		''' <p>
		''' Applications should normally obtain an instance from <seealso cref="ZoneRules"/>.
		''' This factory is only intended for use when creating <seealso cref="ZoneRules"/>.
		''' </summary>
		''' <param name="month">  the month of the month-day of the first day of the cutover week, not null </param>
		''' <param name="dayOfMonthIndicator">  the day of the month-day of the cutover week, positive if the week is that
		'''  day or later, negative if the week is that day or earlier, counting from the last day of the month,
		'''  from -28 to 31 excluding 0 </param>
		''' <param name="dayOfWeek">  the required day-of-week, null if the month-day should not be changed </param>
		''' <param name="time">  the cutover time in the 'before' offset, not null </param>
		''' <param name="timeEndOfDay">  whether the time is midnight at the end of day </param>
		''' <param name="timeDefnition">  how to interpret the cutover </param>
		''' <param name="standardOffset">  the standard offset in force at the cutover, not null </param>
		''' <param name="offsetBefore">  the offset before the cutover, not null </param>
		''' <param name="offsetAfter">  the offset after the cutover, not null </param>
		''' <returns> the rule, not null </returns>
		''' <exception cref="IllegalArgumentException"> if the day of month indicator is invalid </exception>
		''' <exception cref="IllegalArgumentException"> if the end of day flag is true when the time is not midnight </exception>
		Public Shared Function [of](ByVal month As java.time.Month, ByVal dayOfMonthIndicator As Integer, ByVal dayOfWeek As java.time.DayOfWeek, ByVal time As java.time.LocalTime, ByVal timeEndOfDay As Boolean, ByVal timeDefnition As TimeDefinition, ByVal standardOffset As java.time.ZoneOffset, ByVal offsetBefore As java.time.ZoneOffset, ByVal offsetAfter As java.time.ZoneOffset) As ZoneOffsetTransitionRule
			java.util.Objects.requireNonNull(month, "month")
			java.util.Objects.requireNonNull(time, "time")
			java.util.Objects.requireNonNull(timeDefnition, "timeDefnition")
			java.util.Objects.requireNonNull(standardOffset, "standardOffset")
			java.util.Objects.requireNonNull(offsetBefore, "offsetBefore")
			java.util.Objects.requireNonNull(offsetAfter, "offsetAfter")
			If dayOfMonthIndicator < -28 OrElse dayOfMonthIndicator > 31 OrElse dayOfMonthIndicator = 0 Then Throw New IllegalArgumentException("Day of month indicator must be between -28 and 31 inclusive excluding zero")
			If timeEndOfDay AndAlso time.Equals(java.time.LocalTime.MIDNIGHT) = False Then Throw New IllegalArgumentException("Time must be midnight when end of day flag is true")
			Return New ZoneOffsetTransitionRule(month, dayOfMonthIndicator, dayOfWeek, time, timeEndOfDay, timeDefnition, standardOffset, offsetBefore, offsetAfter)
		End Function

		''' <summary>
		''' Creates an instance defining the yearly rule to create transitions between two offsets.
		''' </summary>
		''' <param name="month">  the month of the month-day of the first day of the cutover week, not null </param>
		''' <param name="dayOfMonthIndicator">  the day of the month-day of the cutover week, positive if the week is that
		'''  day or later, negative if the week is that day or earlier, counting from the last day of the month,
		'''  from -28 to 31 excluding 0 </param>
		''' <param name="dayOfWeek">  the required day-of-week, null if the month-day should not be changed </param>
		''' <param name="time">  the cutover time in the 'before' offset, not null </param>
		''' <param name="timeEndOfDay">  whether the time is midnight at the end of day </param>
		''' <param name="timeDefnition">  how to interpret the cutover </param>
		''' <param name="standardOffset">  the standard offset in force at the cutover, not null </param>
		''' <param name="offsetBefore">  the offset before the cutover, not null </param>
		''' <param name="offsetAfter">  the offset after the cutover, not null </param>
		''' <exception cref="IllegalArgumentException"> if the day of month indicator is invalid </exception>
		''' <exception cref="IllegalArgumentException"> if the end of day flag is true when the time is not midnight </exception>
		Friend Sub New(ByVal month As java.time.Month, ByVal dayOfMonthIndicator As Integer, ByVal dayOfWeek As java.time.DayOfWeek, ByVal time As java.time.LocalTime, ByVal timeEndOfDay As Boolean, ByVal timeDefnition As TimeDefinition, ByVal standardOffset As java.time.ZoneOffset, ByVal offsetBefore As java.time.ZoneOffset, ByVal offsetAfter As java.time.ZoneOffset)
			Me.month = month
			Me.dom = CByte(dayOfMonthIndicator)
			Me.dow = dayOfWeek
			Me.time = time
			Me.timeEndOfDay = timeEndOfDay
			Me.timeDefinition = timeDefnition
			Me.standardOffset = standardOffset
			Me.offsetBefore = offsetBefore
			Me.offsetAfter = offsetAfter
		End Sub

		'-----------------------------------------------------------------------
		''' <summary>
		''' Defend against malicious streams.
		''' </summary>
		''' <param name="s"> the stream to read </param>
		''' <exception cref="InvalidObjectException"> always </exception>
		Private Sub readObject(ByVal s As java.io.ObjectInputStream)
			Throw New java.io.InvalidObjectException("Deserialization via serialization delegate")
		End Sub

		''' <summary>
		''' Writes the object using a
		''' <a href="../../../serialized-form.html#java.time.zone.Ser">dedicated serialized form</a>.
		''' @serialData
		''' Refer to the serialized form of
		''' <a href="../../../serialized-form.html#java.time.zone.ZoneRules">ZoneRules.writeReplace</a>
		''' for the encoding of epoch seconds and offsets.
		''' <pre style="font-size:1.0em">{@code
		''' 
		'''      out.writeByte(3);                // identifies a ZoneOffsetTransition
		'''      final int timeSecs = (timeEndOfDay ? 86400 : time.toSecondOfDay());
		'''      final int stdOffset = standardOffset.getTotalSeconds();
		'''      final int beforeDiff = offsetBefore.getTotalSeconds() - stdOffset;
		'''      final int afterDiff = offsetAfter.getTotalSeconds() - stdOffset;
		'''      final int timeByte = (timeSecs % 3600 == 0 ? (timeEndOfDay ? 24 : time.getHour()) : 31);
		'''      final int stdOffsetByte = (stdOffset % 900 == 0 ? stdOffset / 900 + 128 : 255);
		'''      final int beforeByte = (beforeDiff == 0 || beforeDiff == 1800 || beforeDiff == 3600 ? beforeDiff / 1800 : 3);
		'''      final int afterByte = (afterDiff == 0 || afterDiff == 1800 || afterDiff == 3600 ? afterDiff / 1800 : 3);
		'''      final int dowByte = (dow == null ? 0 : dow.getValue());
		'''      int b = (month.getValue() << 28) +          // 4 bits
		'''              ((dom + 32) << 22) +                // 6 bits
		'''              (dowByte << 19) +                   // 3 bits
		'''              (timeByte << 14) +                  // 5 bits
		'''              (timeDefinition.ordinal() << 12) +  // 2 bits
		'''              (stdOffsetByte << 4) +              // 8 bits
		'''              (beforeByte << 2) +                 // 2 bits
		'''              afterByte;                          // 2 bits
		'''      out.writeInt(b);
		'''      if (timeByte == 31) {
		'''          out.writeInt(timeSecs);
		'''      }
		'''      if (stdOffsetByte == 255) {
		'''          out.writeInt(stdOffset);
		'''      }
		'''      if (beforeByte == 3) {
		'''          out.writeInt(offsetBefore.getTotalSeconds());
		'''      }
		'''      if (afterByte == 3) {
		'''          out.writeInt(offsetAfter.getTotalSeconds());
		'''      }
		''' }
		''' </pre>
		''' </summary>
		''' <returns> the replacing object, not null </returns>
		Private Function writeReplace() As Object
			Return New Ser(Ser.ZOTRULE, Me)
		End Function

		''' <summary>
		''' Writes the state to the stream.
		''' </summary>
		''' <param name="out">  the output stream, not null </param>
		''' <exception cref="IOException"> if an error occurs </exception>
		Friend Sub writeExternal(ByVal out As java.io.DataOutput)
			Dim timeSecs As Integer = (If(timeEndOfDay, 86400, time.toSecondOfDay()))
			Dim stdOffset As Integer = standardOffset.totalSeconds
			Dim beforeDiff As Integer = offsetBefore.totalSeconds - stdOffset
			Dim afterDiff As Integer = offsetAfter.totalSeconds - stdOffset
			Dim timeByte As Integer = (If(timeSecs Mod 3600 = 0, (If(timeEndOfDay, 24, time.hour)), 31))
			Dim stdOffsetByte As Integer = (If(stdOffset Mod 900 = 0, stdOffset \ 900 + 128, 255))
			Dim beforeByte As Integer = (If(beforeDiff = 0 OrElse beforeDiff = 1800 OrElse beforeDiff = 3600, beforeDiff \ 1800, 3))
			Dim afterByte As Integer = (If(afterDiff = 0 OrElse afterDiff = 1800 OrElse afterDiff = 3600, afterDiff \ 1800, 3))
			Dim dowByte As Integer = (If(dow Is Nothing, 0, dow.value))
			Dim b As Integer = (month.value << 28) + ((dom + 32) << 22) + (dowByte << 19) + (timeByte << 14) + (timeDefinition.ordinal() << 12) + (stdOffsetByte << 4) + (beforeByte << 2) + afterByte ' 2 bits -  2 bits -  8 bits -  2 bits -  5 bits -  3 bits -  6 bits -  4 bits
			out.writeInt(b)
			If timeByte = 31 Then out.writeInt(timeSecs)
			If stdOffsetByte = 255 Then out.writeInt(stdOffset)
			If beforeByte = 3 Then out.writeInt(offsetBefore.totalSeconds)
			If afterByte = 3 Then out.writeInt(offsetAfter.totalSeconds)
		End Sub

		''' <summary>
		''' Reads the state from the stream.
		''' </summary>
		''' <param name="in">  the input stream, not null </param>
		''' <returns> the created object, not null </returns>
		''' <exception cref="IOException"> if an error occurs </exception>
		Shared Function readExternal(ByVal [in] As java.io.DataInput) As ZoneOffsetTransitionRule
			Dim data As Integer = [in].readInt()
			Dim month_Renamed As java.time.Month = java.time.Month.of(CInt(CUInt(data) >> 28))
			Dim dom As Integer = (CInt(CUInt((data And (63 << 22))) >> 22)) - 32
			Dim dowByte As Integer = CInt(CUInt((data And (7 << 19))) >> 19)
			Dim dow As java.time.DayOfWeek = If(dowByte = 0, Nothing, java.time.DayOfWeek.of(dowByte))
			Dim timeByte As Integer = CInt(CUInt((data And (31 << 14))) >> 14)
			Dim defn As TimeDefinition = System.Enum.GetValues(GetType(TimeDefinition))(CInt(CUInt((data And (3 << 12))) >> 12))
			Dim stdByte As Integer = CInt(CUInt((data And (255 << 4))) >> 4)
			Dim beforeByte As Integer = CInt(CUInt((data And (3 << 2))) >> 2)
			Dim afterByte As Integer = (data And 3)
			Dim time As java.time.LocalTime = (If(timeByte = 31, java.time.LocalTime.ofSecondOfDay([in].readInt()), java.time.LocalTime.of(timeByte Mod 24, 0)))
			Dim std As java.time.ZoneOffset = (If(stdByte = 255, java.time.ZoneOffset.ofTotalSeconds([in].readInt()), java.time.ZoneOffset.ofTotalSeconds((stdByte - 128) * 900)))
			Dim before As java.time.ZoneOffset = (If(beforeByte = 3, java.time.ZoneOffset.ofTotalSeconds([in].readInt()), java.time.ZoneOffset.ofTotalSeconds(std.totalSeconds + beforeByte * 1800)))
			Dim after As java.time.ZoneOffset = (If(afterByte = 3, java.time.ZoneOffset.ofTotalSeconds([in].readInt()), java.time.ZoneOffset.ofTotalSeconds(std.totalSeconds + afterByte * 1800)))
			Return ZoneOffsetTransitionRule.of(month_Renamed, dom, dow, time, timeByte = 24, defn, std, before, after)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Gets the month of the transition.
		''' <p>
		''' If the rule defines an exact date then the month is the month of that date.
		''' <p>
		''' If the rule defines a week where the transition might occur, then the month
		''' if the month of either the earliest or latest possible date of the cutover.
		''' </summary>
		''' <returns> the month of the transition, not null </returns>
		Public Property month As java.time.Month
			Get
				Return month
			End Get
		End Property

		''' <summary>
		''' Gets the indicator of the day-of-month of the transition.
		''' <p>
		''' If the rule defines an exact date then the day is the month of that date.
		''' <p>
		''' If the rule defines a week where the transition might occur, then the day
		''' defines either the start of the end of the transition week.
		''' <p>
		''' If the value is positive, then it represents a normal day-of-month, and is the
		''' earliest possible date that the transition can be.
		''' The date may refer to 29th February which should be treated as 1st March in non-leap years.
		''' <p>
		''' If the value is negative, then it represents the number of days back from the
		''' end of the month where {@code -1} is the last day of the month.
		''' In this case, the day identified is the latest possible date that the transition can be.
		''' </summary>
		''' <returns> the day-of-month indicator, from -28 to 31 excluding 0 </returns>
		Public Property dayOfMonthIndicator As Integer
			Get
				Return dom
			End Get
		End Property

		''' <summary>
		''' Gets the day-of-week of the transition.
		''' <p>
		''' If the rule defines an exact date then this returns null.
		''' <p>
		''' If the rule defines a week where the cutover might occur, then this method
		''' returns the day-of-week that the month-day will be adjusted to.
		''' If the day is positive then the adjustment is later.
		''' If the day is negative then the adjustment is earlier.
		''' </summary>
		''' <returns> the day-of-week that the transition occurs, null if the rule defines an exact date </returns>
		Public Property dayOfWeek As java.time.DayOfWeek
			Get
				Return dow
			End Get
		End Property

		''' <summary>
		''' Gets the local time of day of the transition which must be checked with
		''' <seealso cref="#isMidnightEndOfDay()"/>.
		''' <p>
		''' The time is converted into an instant using the time definition.
		''' </summary>
		''' <returns> the local time of day of the transition, not null </returns>
		Public Property localTime As java.time.LocalTime
			Get
				Return time
			End Get
		End Property

		''' <summary>
		''' Is the transition local time midnight at the end of day.
		''' <p>
		''' The transition may be represented as occurring at 24:00.
		''' </summary>
		''' <returns> whether a local time of midnight is at the start or end of the day </returns>
		Public Property midnightEndOfDay As Boolean
			Get
				Return timeEndOfDay
			End Get
		End Property

		''' <summary>
		''' Gets the time definition, specifying how to convert the time to an instant.
		''' <p>
		''' The local time can be converted to an instant using the standard offset,
		''' the wall offset or UTC.
		''' </summary>
		''' <returns> the time definition, not null </returns>
		Public Property timeDefinition As TimeDefinition
			Get
				Return timeDefinition
			End Get
		End Property

		''' <summary>
		''' Gets the standard offset in force at the transition.
		''' </summary>
		''' <returns> the standard offset, not null </returns>
		Public Property standardOffset As java.time.ZoneOffset
			Get
				Return standardOffset
			End Get
		End Property

		''' <summary>
		''' Gets the offset before the transition.
		''' </summary>
		''' <returns> the offset before, not null </returns>
		Public Property offsetBefore As java.time.ZoneOffset
			Get
				Return offsetBefore
			End Get
		End Property

		''' <summary>
		''' Gets the offset after the transition.
		''' </summary>
		''' <returns> the offset after, not null </returns>
		Public Property offsetAfter As java.time.ZoneOffset
			Get
				Return offsetAfter
			End Get
		End Property

		'-----------------------------------------------------------------------
		''' <summary>
		''' Creates a transition instance for the specified year.
		''' <p>
		''' Calculations are performed using the ISO-8601 chronology.
		''' </summary>
		''' <param name="year">  the year to create a transition for, not null </param>
		''' <returns> the transition instance, not null </returns>
		Public Function createTransition(ByVal year_Renamed As Integer) As ZoneOffsetTransition
			Dim date_Renamed As java.time.LocalDate
			If dom < 0 Then
				date_Renamed = java.time.LocalDate.of(year_Renamed, month, month.length(java.time.chrono.IsoChronology.INSTANCE.isLeapYear(year_Renamed)) + 1 + dom)
				If dow IsNot Nothing Then date_Renamed = date_Renamed.with(previousOrSame(dow))
			Else
				date_Renamed = java.time.LocalDate.of(year_Renamed, month, dom)
				If dow IsNot Nothing Then date_Renamed = date_Renamed.with(nextOrSame(dow))
			End If
			If timeEndOfDay Then date_Renamed = date_Renamed.plusDays(1)
			Dim localDT As java.time.LocalDateTime = java.time.LocalDateTime.of(date_Renamed, time)
			Dim transition As java.time.LocalDateTime = timeDefinition.createDateTime(localDT, standardOffset, offsetBefore)
			Return New ZoneOffsetTransition(transition, offsetBefore, offsetAfter)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Checks if this object equals another.
		''' <p>
		''' The entire state of the object is compared.
		''' </summary>
		''' <param name="otherRule">  the other object to compare to, null returns false </param>
		''' <returns> true if equal </returns>
		Public Overrides Function Equals(ByVal otherRule As Object) As Boolean
			If otherRule Is Me Then Return True
			If TypeOf otherRule Is ZoneOffsetTransitionRule Then
				Dim other As ZoneOffsetTransitionRule = CType(otherRule, ZoneOffsetTransitionRule)
				Return month Is other.month AndAlso dom = other.dom AndAlso dow Is other.dow AndAlso timeDefinition = other.timeDefinition AndAlso time.Equals(other.time) AndAlso timeEndOfDay = other.timeEndOfDay AndAlso standardOffset.Equals(other.standardOffset) AndAlso offsetBefore.Equals(other.offsetBefore) AndAlso offsetAfter.Equals(other.offsetAfter)
			End If
			Return False
		End Function

		''' <summary>
		''' Returns a suitable hash code.
		''' </summary>
		''' <returns> the hash code </returns>
		Public Overrides Function GetHashCode() As Integer
			Dim hash As Integer = ((time.toSecondOfDay() + (If(timeEndOfDay, 1, 0))) << 15) + (month.ordinal() << 11) + ((dom + 32) << 5) + ((If(dow Is Nothing, 7, dow.ordinal())) << 2) + (timeDefinition.ordinal())
			Return hash Xor standardOffset.GetHashCode() Xor offsetBefore.GetHashCode() Xor offsetAfter.GetHashCode()
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Returns a string describing this object.
		''' </summary>
		''' <returns> a string for debugging, not null </returns>
		Public Overrides Function ToString() As String
			Dim buf As New StringBuilder
			buf.append("TransitionRule[").append(If(offsetBefore.CompareTo(offsetAfter) > 0, "Gap ", "Overlap ")).append(offsetBefore).append(" to ").append(offsetAfter).append(", ")
			If dow IsNot Nothing Then
				If dom = -1 Then
					buf.append(dow.name()).append(" on or before last day of ").append(month.name())
				ElseIf dom < 0 Then
					buf.append(dow.name()).append(" on or before last day minus ").append(-dom - 1).append(" of ").append(month.name())
				Else
					buf.append(dow.name()).append(" on or after ").append(month.name()).append(" "c).append(dom)
				End If
			Else
				buf.append(month.name()).append(" "c).append(dom)
			End If
			buf.append(" at ").append(If(timeEndOfDay, "24:00", time.ToString())).append(" ").append(timeDefinition).append(", standard offset ").append(standardOffset).append("]"c)
			Return buf.ToString()
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' A definition of the way a local time can be converted to the actual
		''' transition date-time.
		''' <p>
		''' Time zone rules are expressed in one of three ways:
		''' <ul>
		''' <li>Relative to UTC</li>
		''' <li>Relative to the standard offset in force</li>
		''' <li>Relative to the wall offset (what you would see on a clock on the wall)</li>
		''' </ul>
		''' </summary>
		Public Enum TimeDefinition
			''' <summary>
			''' The local date-time is expressed in terms of the UTC offset. </summary>
			UTC
			''' <summary>
			''' The local date-time is expressed in terms of the wall offset. </summary>
			WALL
			''' <summary>
			''' The local date-time is expressed in terms of the standard offset. </summary>
			STANDARD

			''' <summary>
			''' Converts the specified local date-time to the local date-time actually
			''' seen on a wall clock.
			''' <p>
			''' This method converts using the type of this enum.
			''' The output is defined relative to the 'before' offset of the transition.
			''' <p>
			''' The UTC type uses the UTC offset.
			''' The STANDARD type uses the standard offset.
			''' The WALL type returns the input date-time.
			''' The result is intended for use with the wall-offset.
			''' </summary>
			''' <param name="dateTime">  the local date-time, not null </param>
			''' <param name="standardOffset">  the standard offset, not null </param>
			''' <param name="wallOffset">  the wall offset, not null </param>
			''' <returns> the date-time relative to the wall/before offset, not null </returns>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public java.time.LocalDateTime createDateTime(java.time.LocalDateTime dateTime, java.time.ZoneOffset standardOffset, java.time.ZoneOffset wallOffset)
	'		{
	'			switch (Me)
	'			{
	'				case UTC:
	'				{
	'					int difference = wallOffset.getTotalSeconds() - ZoneOffset.UTC.getTotalSeconds();
	'					Return dateTime.plusSeconds(difference);
	'				}
	'				case STANDARD:
	'				{
	'					int difference = wallOffset.getTotalSeconds() - standardOffset.getTotalSeconds();
	'					Return dateTime.plusSeconds(difference);
	'				}
	'				default: ' WALL
	'					Return dateTime;
	'			}
	'		}
		End Enum

	End Class

End Namespace
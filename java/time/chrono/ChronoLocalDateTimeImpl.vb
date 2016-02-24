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
' * Copyright (c) 2007-2012, Stephen Colebourne & Michael Nascimento Santos
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
Namespace java.time.chrono



	''' <summary>
	''' A date-time without a time-zone for the calendar neutral API.
	''' <p>
	''' {@code ChronoLocalDateTime} is an immutable date-time object that represents a date-time, often
	''' viewed as year-month-day-hour-minute-second. This object can also access other
	''' fields such as day-of-year, day-of-week and week-of-year.
	''' <p>
	''' This class stores all date and time fields, to a precision of nanoseconds.
	''' It does not store or represent a time-zone. For example, the value
	''' "2nd October 2007 at 13:45.30.123456789" can be stored in an {@code ChronoLocalDateTime}.
	''' 
	''' @implSpec
	''' This class is immutable and thread-safe.
	''' @serial </summary>
	''' @param <D> the concrete type for the date of this date-time
	''' @since 1.8 </param>
	<Serializable> _
	Friend NotInheritable Class ChronoLocalDateTimeImpl(Of D As ChronoLocalDate)
		Implements ChronoLocalDateTime(Of D), java.time.temporal.Temporal, java.time.temporal.TemporalAdjuster

		''' <summary>
		''' Serialization version.
		''' </summary>
		Private Const serialVersionUID As Long = 4556003607393004514L
		''' <summary>
		''' Hours per day.
		''' </summary>
		Friend Const HOURS_PER_DAY As Integer = 24
		''' <summary>
		''' Minutes per hour.
		''' </summary>
		Friend Const MINUTES_PER_HOUR As Integer = 60
		''' <summary>
		''' Minutes per day.
		''' </summary>
		Friend Shared ReadOnly MINUTES_PER_DAY As Integer = MINUTES_PER_HOUR * HOURS_PER_DAY
		''' <summary>
		''' Seconds per minute.
		''' </summary>
		Friend Const SECONDS_PER_MINUTE As Integer = 60
		''' <summary>
		''' Seconds per hour.
		''' </summary>
		Friend Shared ReadOnly SECONDS_PER_HOUR As Integer = SECONDS_PER_MINUTE * MINUTES_PER_HOUR
		''' <summary>
		''' Seconds per day.
		''' </summary>
		Friend Shared ReadOnly SECONDS_PER_DAY As Integer = SECONDS_PER_HOUR * HOURS_PER_DAY
		''' <summary>
		''' Milliseconds per day.
		''' </summary>
		Friend Shared ReadOnly MILLIS_PER_DAY As Long = SECONDS_PER_DAY * 1000L
		''' <summary>
		''' Microseconds per day.
		''' </summary>
		Friend Shared ReadOnly MICROS_PER_DAY As Long = SECONDS_PER_DAY * 1000_000L
		''' <summary>
		''' Nanos per second.
		''' </summary>
		Friend Shared ReadOnly NANOS_PER_SECOND As Long = 1000_000_000L
		''' <summary>
		''' Nanos per minute.
		''' </summary>
		Friend Shared ReadOnly NANOS_PER_MINUTE As Long = NANOS_PER_SECOND * SECONDS_PER_MINUTE
		''' <summary>
		''' Nanos per hour.
		''' </summary>
		Friend Shared ReadOnly NANOS_PER_HOUR As Long = NANOS_PER_MINUTE * MINUTES_PER_HOUR
		''' <summary>
		''' Nanos per day.
		''' </summary>
		Friend Shared ReadOnly NANOS_PER_DAY As Long = NANOS_PER_HOUR * HOURS_PER_DAY

		''' <summary>
		''' The date part.
		''' </summary>
		<NonSerialized> _
		Private ReadOnly [date] As D
		''' <summary>
		''' The time part.
		''' </summary>
		<NonSerialized> _
		Private ReadOnly time As java.time.LocalTime

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains an instance of {@code ChronoLocalDateTime} from a date and time.
		''' </summary>
		''' <param name="date">  the local date, not null </param>
		''' <param name="time">  the local time, not null </param>
		''' <returns> the local date-time, not null </returns>
		Shared Function [of](Of R As ChronoLocalDate)(ByVal [date] As R, ByVal time As java.time.LocalTime) As ChronoLocalDateTimeImpl(Of R)
			Return New ChronoLocalDateTimeImpl(Of )(date_Renamed, time)
		End Function

		''' <summary>
		''' Casts the {@code Temporal} to {@code ChronoLocalDateTime} ensuring it bas the specified chronology.
		''' </summary>
		''' <param name="chrono">  the chronology to check for, not null </param>
		''' <param name="temporal">   a date-time to cast, not null </param>
		''' <returns> the date-time checked and cast to {@code ChronoLocalDateTime}, not null </returns>
		''' <exception cref="ClassCastException"> if the date-time cannot be cast to ChronoLocalDateTimeImpl
		'''  or the chronology is not equal this Chronology </exception>
		Shared Function ensureValid(Of R As ChronoLocalDate)(ByVal chrono As Chronology, ByVal temporal As java.time.temporal.Temporal) As ChronoLocalDateTimeImpl(Of R)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Dim other As ChronoLocalDateTimeImpl(Of R) = CType(temporal, ChronoLocalDateTimeImpl(Of R))
			If chrono.Equals(other.chronology) = False Then Throw New ClassCastException("Chronology mismatch, required: " & chrono.id & ", actual: " & other.chronology.id)
			Return other
		End Function

		''' <summary>
		''' Constructor.
		''' </summary>
		''' <param name="date">  the date part of the date-time, not null </param>
		''' <param name="time">  the time part of the date-time, not null </param>
		Private Sub New(ByVal [date] As D, ByVal time As java.time.LocalTime)
			java.util.Objects.requireNonNull(date_Renamed, "date")
			java.util.Objects.requireNonNull(time, "time")
			Me.date = date_Renamed
			Me.time = time
		End Sub

		''' <summary>
		''' Returns a copy of this date-time with the new date and time, checking
		''' to see if a new object is in fact required.
		''' </summary>
		''' <param name="newDate">  the date of the new date-time, not null </param>
		''' <param name="newTime">  the time of the new date-time, not null </param>
		''' <returns> the date-time, not null </returns>
		Private Function [with](ByVal newDate As java.time.temporal.Temporal, ByVal newTime As java.time.LocalTime) As ChronoLocalDateTimeImpl(Of D)
			If [date] Is newDate AndAlso time Is newTime Then Return Me
			' Validate that the new Temporal is a ChronoLocalDate (and not something else)
			Dim cd As D = ChronoLocalDateImpl.ensureValid([date].chronology, newDate)
			Return New ChronoLocalDateTimeImpl(Of )(cd, newTime)
		End Function

		'-----------------------------------------------------------------------
		Public Overrides Function toLocalDate() As D Implements ChronoLocalDateTime(Of D).toLocalDate
			Return [date]
		End Function

		Public Overrides Function toLocalTime() As java.time.LocalTime Implements ChronoLocalDateTime(Of D).toLocalTime
			Return time
		End Function

		'-----------------------------------------------------------------------
		Public Overrides Function isSupported(ByVal field As java.time.temporal.TemporalField) As Boolean Implements ChronoLocalDateTime(Of D).isSupported
			If TypeOf field Is java.time.temporal.ChronoField Then
				Dim f As java.time.temporal.ChronoField = CType(field, java.time.temporal.ChronoField)
				Return f.dateBased OrElse f.timeBased
			End If
			Return field IsNot Nothing AndAlso field.isSupportedBy(Me)
		End Function

		Public Overrides Function range(ByVal field As java.time.temporal.TemporalField) As java.time.temporal.ValueRange
			If TypeOf field Is java.time.temporal.ChronoField Then
				Dim f As java.time.temporal.ChronoField = CType(field, java.time.temporal.ChronoField)
				Return (If(f.timeBased, time.range(field), [date].range(field)))
			End If
			Return field.rangeRefinedBy(Me)
		End Function

		Public Overrides Function [get](ByVal field As java.time.temporal.TemporalField) As Integer
			If TypeOf field Is java.time.temporal.ChronoField Then
				Dim f As java.time.temporal.ChronoField = CType(field, java.time.temporal.ChronoField)
				Return (If(f.timeBased, time.get(field), [date].get(field)))
			End If
			Return range(field).checkValidIntValue(getLong(field), field)
		End Function

		Public Overrides Function getLong(ByVal field As java.time.temporal.TemporalField) As Long
			If TypeOf field Is java.time.temporal.ChronoField Then
				Dim f As java.time.temporal.ChronoField = CType(field, java.time.temporal.ChronoField)
				Return (If(f.timeBased, time.getLong(field), [date].getLong(field)))
			End If
			Return field.getFrom(Me)
		End Function

		'-----------------------------------------------------------------------
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overrides Function [with](ByVal adjuster As java.time.temporal.TemporalAdjuster) As ChronoLocalDateTimeImpl(Of D)
			If TypeOf adjuster Is ChronoLocalDate Then
				' The Chronology is checked in with(date,time)
				Return [with](CType(adjuster, ChronoLocalDate), time)
			ElseIf TypeOf adjuster Is java.time.LocalTime Then
				Return [with]([date], CType(adjuster, java.time.LocalTime))
			ElseIf TypeOf adjuster Is ChronoLocalDateTimeImpl Then
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Return ChronoLocalDateTimeImpl.ensureValid([date].chronology, CType(adjuster, ChronoLocalDateTimeImpl(Of ?)))
			End If
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Return ChronoLocalDateTimeImpl.ensureValid([date].chronology, CType(adjuster.adjustInto(Me), ChronoLocalDateTimeImpl(Of ?)))
		End Function

		Public Overrides Function [with](ByVal field As java.time.temporal.TemporalField, ByVal newValue As Long) As ChronoLocalDateTimeImpl(Of D)
			If TypeOf field Is java.time.temporal.ChronoField Then
				Dim f As java.time.temporal.ChronoField = CType(field, java.time.temporal.ChronoField)
				If f.timeBased Then
					Return [with]([date], time.with(field, newValue))
				Else
					Return [with]([date].with(field, newValue), time)
				End If
			End If
			Return ChronoLocalDateTimeImpl.ensureValid([date].chronology, field.adjustInto(Me, newValue))
		End Function

		'-----------------------------------------------------------------------
		Public Overrides Function plus(ByVal amountToAdd As Long, ByVal unit As java.time.temporal.TemporalUnit) As ChronoLocalDateTimeImpl(Of D)
			If TypeOf unit Is java.time.temporal.ChronoUnit Then
				Dim f As java.time.temporal.ChronoUnit = CType(unit, java.time.temporal.ChronoUnit)
				Select Case f
					Case NANOS
						Return plusNanos(amountToAdd)
					Case MICROS
						Return plusDays(amountToAdd \ MICROS_PER_DAY).plusNanos((amountToAdd Mod MICROS_PER_DAY) * 1000)
					Case MILLIS
						Return plusDays(amountToAdd \ MILLIS_PER_DAY).plusNanos((amountToAdd Mod MILLIS_PER_DAY) * 1000000)
					Case SECONDS
						Return plusSeconds(amountToAdd)
					Case MINUTES
						Return plusMinutes(amountToAdd)
					Case HOURS
						Return plusHours(amountToAdd)
					Case HALF_DAYS ' no overflow (256 is multiple of 2)
						Return plusDays(amountToAdd \ 256).plusHours((amountToAdd Mod 256) * 12)
				End Select
				Return [with]([date].plus(amountToAdd, unit), time)
			End If
			Return ChronoLocalDateTimeImpl.ensureValid([date].chronology, unit.addTo(Me, amountToAdd))
		End Function

		Private Function plusDays(ByVal days As Long) As ChronoLocalDateTimeImpl(Of D)
			Return [with]([date].plus(days, java.time.temporal.ChronoUnit.DAYS), time)
		End Function

		Private Function plusHours(ByVal hours As Long) As ChronoLocalDateTimeImpl(Of D)
			Return plusWithOverflow([date], hours, 0, 0, 0)
		End Function

		Private Function plusMinutes(ByVal minutes As Long) As ChronoLocalDateTimeImpl(Of D)
			Return plusWithOverflow([date], 0, minutes, 0, 0)
		End Function

		Friend Function plusSeconds(ByVal seconds As Long) As ChronoLocalDateTimeImpl(Of D)
			Return plusWithOverflow([date], 0, 0, seconds, 0)
		End Function

		Private Function plusNanos(ByVal nanos As Long) As ChronoLocalDateTimeImpl(Of D)
			Return plusWithOverflow([date], 0, 0, 0, nanos)
		End Function

		'-----------------------------------------------------------------------
		Private Function plusWithOverflow(ByVal newDate As D, ByVal hours As Long, ByVal minutes As Long, ByVal seconds As Long, ByVal nanos As Long) As ChronoLocalDateTimeImpl(Of D)
			' 9223372036854775808 long, 2147483648 int
			If (hours Or minutes Or seconds Or nanos) = 0 Then Return [with](newDate, time)
			Dim totDays As Long = nanos \ NANOS_PER_DAY + seconds \ SECONDS_PER_DAY + minutes \ MINUTES_PER_DAY + hours \ HOURS_PER_DAY '   max/24 -    max/24*60 -    max/24*60*60 -    max/24*60*60*1B
			Dim totNanos As Long = nanos Mod NANOS_PER_DAY + (seconds Mod SECONDS_PER_DAY) * NANOS_PER_SECOND + (minutes Mod MINUTES_PER_DAY) * NANOS_PER_MINUTE + (hours Mod HOURS_PER_DAY) * NANOS_PER_HOUR '   max  86400000000000 -    max  86400000000000 -    max  86400000000000 -    max  86400000000000
			Dim curNoD As Long = time.toNanoOfDay() '   max  86400000000000
			totNanos = totNanos + curNoD ' total 432000000000000
			totDays += Math.floorDiv(totNanos, NANOS_PER_DAY)
			Dim newNoD As Long = Math.floorMod(totNanos, NANOS_PER_DAY)
			Dim newTime As java.time.LocalTime = (If(newNoD = curNoD, time, java.time.LocalTime.ofNanoOfDay(newNoD)))
			Return [with](newDate.plus(totDays, java.time.temporal.ChronoUnit.DAYS), newTime)
		End Function

		'-----------------------------------------------------------------------
		Public Overrides Function atZone(ByVal zone As java.time.ZoneId) As ChronoZonedDateTime(Of D) Implements ChronoLocalDateTime(Of D).atZone
			Return ChronoZonedDateTimeImpl.ofBest(Me, zone, Nothing)
		End Function

		'-----------------------------------------------------------------------
		Public Overrides Function [until](ByVal endExclusive As java.time.temporal.Temporal, ByVal unit As java.time.temporal.TemporalUnit) As Long
			java.util.Objects.requireNonNull(endExclusive, "endExclusive")
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Dim [end] As ChronoLocalDateTime(Of D) = CType(chronology.localDateTime(endExclusive), ChronoLocalDateTime(Of D))
			If TypeOf unit Is java.time.temporal.ChronoUnit Then
				If unit.timeBased Then
					Dim amount As Long = [end].getLong(EPOCH_DAY) - [date].getLong(EPOCH_DAY)
					Select Case CType(unit, java.time.temporal.ChronoUnit)
						Case NANOS
							amount = Math.multiplyExact(amount, NANOS_PER_DAY)
						Case MICROS
							amount = Math.multiplyExact(amount, MICROS_PER_DAY)
						Case MILLIS
							amount = Math.multiplyExact(amount, MILLIS_PER_DAY)
						Case SECONDS
							amount = Math.multiplyExact(amount, SECONDS_PER_DAY)
						Case MINUTES
							amount = Math.multiplyExact(amount, MINUTES_PER_DAY)
						Case HOURS
							amount = Math.multiplyExact(amount, HOURS_PER_DAY)
						Case HALF_DAYS
							amount = Math.multiplyExact(amount, 2)
					End Select
					Return Math.addExact(amount, time.until([end].toLocalTime(), unit))
				End If
				Dim endDate As ChronoLocalDate = [end].toLocalDate()
				If [end].toLocalTime().isBefore(time) Then endDate = endDate.minus(1, java.time.temporal.ChronoUnit.DAYS)
				Return [date].until(endDate, unit)
			End If
			java.util.Objects.requireNonNull(unit, "unit")
			Return unit.between(Me, [end])
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Writes the ChronoLocalDateTime using a
		''' <a href="../../../serialized-form.html#java.time.chrono.Ser">dedicated serialized form</a>.
		''' @serialData
		''' <pre>
		'''  out.writeByte(2);              // identifies a ChronoLocalDateTime
		'''  out.writeObject(toLocalDate());
		'''  out.witeObject(toLocalTime());
		''' </pre>
		''' </summary>
		''' <returns> the instance of {@code Ser}, not null </returns>
		Private Function writeReplace() As Object
			Return New Ser(Ser.CHRONO_LOCAL_DATE_TIME_TYPE, Me)
		End Function

		''' <summary>
		''' Defend against malicious streams.
		''' </summary>
		''' <param name="s"> the stream to read </param>
		''' <exception cref="InvalidObjectException"> always </exception>
		Private Sub readObject(ByVal s As java.io.ObjectInputStream)
			Throw New java.io.InvalidObjectException("Deserialization via serialization delegate")
		End Sub

		Friend Sub writeExternal(ByVal out As java.io.ObjectOutput)
			out.writeObject([date])
			out.writeObject(time)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Friend Shared Function readExternal(ByVal [in] As java.io.ObjectInput) As ChronoLocalDateTime(Of ?)
			Dim date_Renamed As ChronoLocalDate = CType([in].readObject(), ChronoLocalDate)
			Dim time As java.time.LocalTime = CType([in].readObject(), java.time.LocalTime)
			Return date_Renamed.atTime(time)
		End Function

		'-----------------------------------------------------------------------
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			If Me Is obj Then Return True
			If TypeOf obj Is ChronoLocalDateTime Then Return compareTo(CType(obj, ChronoLocalDateTime(Of ?))) = 0
			Return False
		End Function

		Public Overrides Function GetHashCode() As Integer
			Return toLocalDate().GetHashCode() Xor toLocalTime().GetHashCode()
		End Function

		Public Overrides Function ToString() As String
			Return toLocalDate().ToString() & AscW("T"c) + toLocalTime().ToString()
		End Function

	End Class

End Namespace
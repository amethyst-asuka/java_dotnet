Imports System

'
' * Copyright (c) 2009, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.nio.file.attribute


	''' <summary>
	''' Represents the value of a file's time stamp attribute. For example, it may
	''' represent the time that the file was last
	''' <seealso cref="BasicFileAttributes#lastModifiedTime() modified"/>,
	''' <seealso cref="BasicFileAttributes#lastAccessTime() accessed"/>,
	''' or <seealso cref="BasicFileAttributes#creationTime() created"/>.
	''' 
	''' <p> Instances of this class are immutable.
	''' 
	''' @since 1.7 </summary>
	''' <seealso cref= java.nio.file.Files#setLastModifiedTime </seealso>
	''' <seealso cref= java.nio.file.Files#getLastModifiedTime </seealso>

	Public NotInheritable Class FileTime
		Implements Comparable(Of FileTime)

		''' <summary>
		''' The unit of granularity to interpret the value. Null if
		''' this {@code FileTime} is converted from an {@code Instant},
		''' the {@code value} and {@code unit} pair will not be used
		''' in this scenario.
		''' </summary>
		Private ReadOnly unit As java.util.concurrent.TimeUnit

		''' <summary>
		''' The value since the epoch; can be negative.
		''' </summary>
		Private ReadOnly value As Long

		''' <summary>
		''' The value as Instant (created lazily, if not from an instant)
		''' </summary>
		Private instant As java.time.Instant

		''' <summary>
		''' The value return by toString (created lazily)
		''' </summary>
		Private valueAsString As String

		''' <summary>
		''' Initializes a new instance of this class.
		''' </summary>
		Private Sub New(ByVal value As Long, ByVal unit As java.util.concurrent.TimeUnit, ByVal instant As java.time.Instant)
			Me.value = value
			Me.unit = unit
			Me.instant = instant
		End Sub

		''' <summary>
		''' Returns a {@code FileTime} representing a value at the given unit of
		''' granularity.
		''' </summary>
		''' <param name="value">
		'''          the value since the epoch (1970-01-01T00:00:00Z); can be
		'''          negative </param>
		''' <param name="unit">
		'''          the unit of granularity to interpret the value
		''' </param>
		''' <returns>  a {@code FileTime} representing the given value </returns>
		Public Shared Function [from](ByVal value As Long, ByVal unit As java.util.concurrent.TimeUnit) As FileTime
			java.util.Objects.requireNonNull(unit, "unit")
			Return New FileTime(value, unit, Nothing)
		End Function

		''' <summary>
		''' Returns a {@code FileTime} representing the given value in milliseconds.
		''' </summary>
		''' <param name="value">
		'''          the value, in milliseconds, since the epoch
		'''          (1970-01-01T00:00:00Z); can be negative
		''' </param>
		''' <returns>  a {@code FileTime} representing the given value </returns>
		Public Shared Function fromMillis(ByVal value As Long) As FileTime
			Return New FileTime(value, java.util.concurrent.TimeUnit.MILLISECONDS, Nothing)
		End Function

		''' <summary>
		''' Returns a {@code FileTime} representing the same point of time value
		''' on the time-line as the provided {@code Instant} object.
		''' </summary>
		''' <param name="instant">
		'''          the instant to convert </param>
		''' <returns>  a {@code FileTime} representing the same point on the time-line
		'''          as the provided instant
		''' @since 1.8 </returns>
		Public Shared Function [from](ByVal instant As java.time.Instant) As FileTime
			java.util.Objects.requireNonNull(instant, "instant")
			Return New FileTime(0, Nothing, instant)
		End Function

		''' <summary>
		''' Returns the value at the given unit of granularity.
		''' 
		''' <p> Conversion from a coarser granularity that would numerically overflow
		''' saturate to {@code Long.MIN_VALUE} if negative or {@code Long.MAX_VALUE}
		''' if positive.
		''' </summary>
		''' <param name="unit">
		'''          the unit of granularity for the return value
		''' </param>
		''' <returns>  value in the given unit of granularity, since the epoch
		'''          since the epoch (1970-01-01T00:00:00Z); can be negative </returns>
		Public Function [to](ByVal unit As java.util.concurrent.TimeUnit) As Long
			java.util.Objects.requireNonNull(unit, "unit")
			If Me.unit IsNot Nothing Then
				Return unit.convert(Me.value, Me.unit)
			Else
				Dim secs As Long = unit.convert(instant.epochSecond, java.util.concurrent.TimeUnit.SECONDS)
				If secs = Long.MinValue OrElse secs = Long.MaxValue Then Return secs
				Dim nanos As Long = unit.convert(instant.nano, java.util.concurrent.TimeUnit.NANOSECONDS)
				Dim r As Long = secs + nanos
				' Math.addExact() variant
				If ((secs Xor r) And (nanos Xor r)) < 0 Then Return If(secs < 0, Long.MinValue, Long.MaxValue)
				Return r
			End If
		End Function

		''' <summary>
		''' Returns the value in milliseconds.
		''' 
		''' <p> Conversion from a coarser granularity that would numerically overflow
		''' saturate to {@code Long.MIN_VALUE} if negative or {@code Long.MAX_VALUE}
		''' if positive.
		''' </summary>
		''' <returns>  the value in milliseconds, since the epoch (1970-01-01T00:00:00Z) </returns>
		Public Function toMillis() As Long
			If unit IsNot Nothing Then
				Return unit.toMillis(value)
			Else
				Dim secs As Long = instant.epochSecond
				Dim nanos As Integer = instant.nano
				' Math.multiplyExact() variant
				Dim r As Long = secs * 1000
				Dim ax As Long = Math.Abs(secs)
				If (CInt(CUInt((ax Or 1000)) >> 31 <> 0)) Then
					If (r \ 1000) <> secs Then Return If(secs < 0, Long.MinValue, Long.MaxValue)
				End If
				Return r + nanos \ 1000000
			End If
		End Function

		''' <summary>
		''' Time unit constants for conversion.
		''' </summary>
		Private Const HOURS_PER_DAY As Long = 24L
		Private Const MINUTES_PER_HOUR As Long = 60L
		Private Const SECONDS_PER_MINUTE As Long = 60L
		Private Shared ReadOnly SECONDS_PER_HOUR As Long = SECONDS_PER_MINUTE * MINUTES_PER_HOUR
		Private Shared ReadOnly SECONDS_PER_DAY As Long = SECONDS_PER_HOUR * HOURS_PER_DAY
		Private Const MILLIS_PER_SECOND As Long = 1000L
		Private Shared ReadOnly MICROS_PER_SECOND As Long = 1000_000L
		Private Shared ReadOnly NANOS_PER_SECOND As Long = 1000_000_000L
		Private Const NANOS_PER_MILLI As Integer = 1000000
		Private Const NANOS_PER_MICRO As Integer = 1000
		' The epoch second of Instant.MIN.
		Private Const MIN_SECOND As Long = -31557014167219200L
		' The epoch second of Instant.MAX.
		Private Const MAX_SECOND As Long = 31556889864403199L

	'    
	'     * Scale d by m, checking for overflow.
	'     
		Private Shared Function scale(ByVal d As Long, ByVal m As Long, ByVal over As Long) As Long
			If d > over Then Return Long.MaxValue
			If d < -over Then Return Long.MinValue
			Return d * m
		End Function

		''' <summary>
		''' Converts this {@code FileTime} object to an {@code Instant}.
		''' 
		''' <p> The conversion creates an {@code Instant} that represents the
		''' same point on the time-line as this {@code FileTime}.
		''' 
		''' <p> {@code FileTime} can store points on the time-line further in the
		''' future and further in the past than {@code Instant}. Conversion
		''' from such further time points saturates to <seealso cref="Instant#MIN"/> if
		''' earlier than {@code Instant.MIN} or <seealso cref="Instant#MAX"/> if later
		''' than {@code Instant.MAX}.
		''' </summary>
		''' <returns>  an instant representing the same point on the time-line as
		'''          this {@code FileTime} object
		''' @since 1.8 </returns>
		Public Function toInstant() As java.time.Instant
			If instant Is Nothing Then
				Dim secs As Long = 0L
				Dim nanos As Integer = 0
				Select Case unit
					Case java.util.concurrent.TimeUnit.DAYS
						secs = scale(value, SECONDS_PER_DAY, Long.MaxValue/SECONDS_PER_DAY)
					Case java.util.concurrent.TimeUnit.HOURS
						secs = scale(value, SECONDS_PER_HOUR, Long.MaxValue/SECONDS_PER_HOUR)
					Case java.util.concurrent.TimeUnit.MINUTES
						secs = scale(value, SECONDS_PER_MINUTE, Long.MaxValue/SECONDS_PER_MINUTE)
					Case java.util.concurrent.TimeUnit.SECONDS
						secs = value
					Case java.util.concurrent.TimeUnit.MILLISECONDS
						secs = Math.floorDiv(value, MILLIS_PER_SECOND)
						nanos = CInt(Math.floorMod(value, MILLIS_PER_SECOND)) * NANOS_PER_MILLI
					Case java.util.concurrent.TimeUnit.MICROSECONDS
						secs = Math.floorDiv(value, MICROS_PER_SECOND)
						nanos = CInt(Math.floorMod(value, MICROS_PER_SECOND)) * NANOS_PER_MICRO
					Case java.util.concurrent.TimeUnit.NANOSECONDS
						secs = Math.floorDiv(value, NANOS_PER_SECOND)
						nanos = CInt(Math.floorMod(value, NANOS_PER_SECOND))
					Case Else
						Throw New AssertionError("Unit not handled")
				End Select
				If secs <= MIN_SECOND Then
					instant = java.time.Instant.MIN
				ElseIf secs >= MAX_SECOND Then
					instant = java.time.Instant.MAX
				Else
					instant = java.time.Instant.ofEpochSecond(secs, nanos)
				End If
			End If
			Return instant
		End Function

		''' <summary>
		''' Tests this {@code FileTime} for equality with the given object.
		''' 
		''' <p> The result is {@code true} if and only if the argument is not {@code
		''' null} and is a {@code FileTime} that represents the same time. This
		''' method satisfies the general contract of the {@code Object.equals} method.
		''' </summary>
		''' <param name="obj">
		'''          the object to compare with
		''' </param>
		''' <returns>  {@code true} if, and only if, the given object is a {@code
		'''          FileTime} that represents the same time </returns>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			Return If(TypeOf obj Is FileTime, compareTo(CType(obj, FileTime)) = 0, False)
		End Function

		''' <summary>
		''' Computes a hash code for this file time.
		''' 
		''' <p> The hash code is based upon the value represented, and satisfies the
		''' general contract of the <seealso cref="Object#hashCode"/> method.
		''' </summary>
		''' <returns>  the hash-code value </returns>
		Public Overrides Function GetHashCode() As Integer
			' hashcode of instant representation to satisfy contract with equals
			Return toInstant().GetHashCode()
		End Function

		Private Function toDays() As Long
			If unit IsNot Nothing Then
				Return unit.toDays(value)
			Else
				Return java.util.concurrent.TimeUnit.SECONDS.toDays(toInstant().epochSecond)
			End If
		End Function

		Private Function toExcessNanos(ByVal days As Long) As Long
			If unit IsNot Nothing Then
				Return unit.toNanos(value - unit.convert(days, java.util.concurrent.TimeUnit.DAYS))
			Else
				Return java.util.concurrent.TimeUnit.SECONDS.toNanos(toInstant().epochSecond - java.util.concurrent.TimeUnit.DAYS.toSeconds(days))
			End If
		End Function

		''' <summary>
		''' Compares the value of two {@code FileTime} objects for order.
		''' </summary>
		''' <param name="other">
		'''          the other {@code FileTime} to be compared
		''' </param>
		''' <returns>  {@code 0} if this {@code FileTime} is equal to {@code other}, a
		'''          value less than 0 if this {@code FileTime} represents a time
		'''          that is before {@code other}, and a value greater than 0 if this
		'''          {@code FileTime} represents a time that is after {@code other} </returns>
		Public Overrides Function compareTo(ByVal other As FileTime) As Integer Implements Comparable(Of FileTime).compareTo
			' same granularity
			If unit IsNot Nothing AndAlso unit = other.unit Then
				Return Long.Compare(value, other.value)
			Else
				' compare using instant representation when unit differs
				Dim secs As Long = toInstant().epochSecond
				Dim secsOther As Long = other.toInstant().epochSecond
				Dim cmp As Integer = Long.Compare(secs, secsOther)
				If cmp <> 0 Then Return cmp
				cmp = Long.Compare(toInstant().nano, other.toInstant().nano)
				If cmp <> 0 Then Return cmp
				If secs <> MAX_SECOND AndAlso secs <> MIN_SECOND Then Return 0
				' if both this and other's Instant reps are MIN/MAX,
				' use daysSinceEpoch and nanosOfDays, which will not
				' saturate during calculation.
				Dim days As Long = toDays()
				Dim daysOther As Long = other.toDays()
				If days = daysOther Then Return Long.Compare(toExcessNanos(days), other.toExcessNanos(daysOther))
				Return Long.Compare(days, daysOther)
			End If
		End Function

		' days in a 400 year cycle = 146097
		' days in a 10,000 year cycle = 146097 * 25
		' seconds per day = 86400
		Private Shared ReadOnly DAYS_PER_10000_YEARS As Long = 146097L * 25L
		Private Shared ReadOnly SECONDS_PER_10000_YEARS As Long = 146097L * 25L * 86400L
		Private Shared ReadOnly SECONDS_0000_TO_1970 As Long = ((146097L * 5L) - (30L * 365L + 7L)) * 86400L

		' append year/month/day/hour/minute/second/nano with width and 0 padding
		Private Function append(ByVal sb As StringBuilder, ByVal w As Integer, ByVal d As Integer) As StringBuilder
			Do While w > 0
				sb.append(ChrW(d\w + AscW("0"c)))
				d = d Mod w
				w \= 10
			Loop
			Return sb
		End Function

		''' <summary>
		''' Returns the string representation of this {@code FileTime}. The string
		''' is returned in the <a
		''' href="http://www.w3.org/TR/NOTE-datetime">ISO&nbsp;8601</a> format:
		''' <pre>
		'''     YYYY-MM-DDThh:mm:ss[.s+]Z
		''' </pre>
		''' where "{@code [.s+]}" represents a dot followed by one of more digits
		''' for the decimal fraction of a second. It is only present when the decimal
		''' fraction of a second is not zero. For example, {@code
		''' FileTime.fromMillis(1234567890000L).toString()} yields {@code
		''' "2009-02-13T23:31:30Z"}, and {@code FileTime.fromMillis(1234567890123L).toString()}
		''' yields {@code "2009-02-13T23:31:30.123Z"}.
		''' 
		''' <p> A {@code FileTime} is primarily intended to represent the value of a
		''' file's time stamp. Where used to represent <i>extreme values</i>, where
		''' the year is less than "{@code 0001}" or greater than "{@code 9999}" then
		''' this method deviates from ISO 8601 in the same manner as the
		''' <a href="http://www.w3.org/TR/xmlschema-2/#deviantformats">XML Schema
		''' language</a>. That is, the year may be expanded to more than four digits
		''' and may be negative-signed. If more than four digits then leading zeros
		''' are not present. The year before "{@code 0001}" is "{@code -0001}".
		''' </summary>
		''' <returns>  the string representation of this file time </returns>
		Public Overrides Function ToString() As String
			If valueAsString Is Nothing Then
				Dim secs As Long = 0L
				Dim nanos As Integer = 0
				If instant Is Nothing AndAlso unit.CompareTo(java.util.concurrent.TimeUnit.SECONDS) >= 0 Then
					secs = unit.toSeconds(value)
				Else
					secs = toInstant().epochSecond
					nanos = toInstant().nano
				End If
				Dim ldt As java.time.LocalDateTime
				Dim year As Integer = 0
				If secs >= -SECONDS_0000_TO_1970 Then
					' current era
					Dim zeroSecs As Long = secs - SECONDS_PER_10000_YEARS + SECONDS_0000_TO_1970
					Dim hi As Long = Math.floorDiv(zeroSecs, SECONDS_PER_10000_YEARS) + 1
					Dim lo As Long = Math.floorMod(zeroSecs, SECONDS_PER_10000_YEARS)
					ldt = java.time.LocalDateTime.ofEpochSecond(lo - SECONDS_0000_TO_1970, nanos, java.time.ZoneOffset.UTC)
					year = ldt.year + CInt(hi) * 10000
				Else
					' before current era
					Dim zeroSecs As Long = secs + SECONDS_0000_TO_1970
					Dim hi As Long = zeroSecs \ SECONDS_PER_10000_YEARS
					Dim lo As Long = zeroSecs Mod SECONDS_PER_10000_YEARS
					ldt = java.time.LocalDateTime.ofEpochSecond(lo - SECONDS_0000_TO_1970, nanos, java.time.ZoneOffset.UTC)
					year = ldt.year + CInt(hi) * 10000
				End If
				If year <= 0 Then year = year - 1
				Dim fraction As Integer = ldt.nano
				Dim sb As New StringBuilder(64)
				sb.append(If(year < 0, "-", ""))
				year = Math.Abs(year)
				If year < 10000 Then
					append(sb, 1000, Math.Abs(year))
				Else
					sb.append(Convert.ToString(year))
				End If
				sb.append("-"c)
				append(sb, 10, ldt.monthValue)
				sb.append("-"c)
				append(sb, 10, ldt.dayOfMonth)
				sb.append("T"c)
				append(sb, 10, ldt.hour)
				sb.append(":"c)
				append(sb, 10, ldt.minute)
				sb.append(":"c)
				append(sb, 10, ldt.second)
				If fraction <> 0 Then
					sb.append("."c)
					' adding leading zeros and stripping any trailing zeros
					Dim w As Integer = 100000000
					Do While fraction Mod 10 = 0
						fraction \= 10
						w \= 10
					Loop
					append(sb, w, fraction)
				End If
				sb.append("Z"c)
				valueAsString = sb.ToString()
			End If
			Return valueAsString
		End Function
	End Class

End Namespace
Imports Microsoft.VisualBasic
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

Namespace java.sql


	''' <summary>
	''' <P>A thin wrapper around <code>java.util.Date</code> that allows
	''' the JDBC API to identify this as an SQL <code>TIMESTAMP</code> value.
	''' It adds the ability
	''' to hold the SQL <code>TIMESTAMP</code> fractional seconds value, by allowing
	''' the specification of fractional seconds to a precision of nanoseconds.
	''' A Timestamp also provides formatting and
	''' parsing operations to support the JDBC escape syntax for timestamp values.
	''' 
	''' <p>The precision of a Timestamp object is calculated to be either:
	''' <ul>
	''' <li><code>19 </code>, which is the number of characters in yyyy-mm-dd hh:mm:ss
	''' <li> <code> 20 + s </code>, which is the number
	''' of characters in the yyyy-mm-dd hh:mm:ss.[fff...] and <code>s</code> represents  the scale of the given Timestamp,
	''' its fractional seconds precision.
	''' </ul>
	''' 
	''' <P><B>Note:</B> This type is a composite of a <code>java.util.Date</code> and a
	''' separate nanoseconds value. Only integral seconds are stored in the
	''' <code>java.util.Date</code> component. The fractional seconds - the nanos - are
	''' separate.  The <code>Timestamp.equals(Object)</code> method never returns
	''' <code>true</code> when passed an object
	''' that isn't an instance of <code>java.sql.Timestamp</code>,
	''' because the nanos component of a date is unknown.
	''' As a result, the <code>Timestamp.equals(Object)</code>
	''' method is not symmetric with respect to the
	''' <code>java.util.Date.equals(Object)</code>
	''' method.  Also, the <code>hashCode</code> method uses the underlying
	''' <code>java.util.Date</code>
	''' implementation and therefore does not include nanos in its computation.
	''' <P>
	''' Due to the differences between the <code>Timestamp</code> class
	''' and the <code>java.util.Date</code>
	''' class mentioned above, it is recommended that code not view
	''' <code>Timestamp</code> values generically as an instance of
	''' <code>java.util.Date</code>.  The
	''' inheritance relationship between <code>Timestamp</code>
	''' and <code>java.util.Date</code> really
	''' denotes implementation inheritance, and not type inheritance.
	''' </summary>
	Public Class Timestamp
		Inherits DateTime?

		''' <summary>
		''' Constructs a <code>Timestamp</code> object initialized
		''' with the given values.
		''' </summary>
		''' <param name="year"> the year minus 1900 </param>
		''' <param name="month"> 0 to 11 </param>
		''' <param name="date"> 1 to 31 </param>
		''' <param name="hour"> 0 to 23 </param>
		''' <param name="minute"> 0 to 59 </param>
		''' <param name="second"> 0 to 59 </param>
		''' <param name="nano"> 0 to 999,999,999 </param>
		''' @deprecated instead use the constructor <code>Timestamp(long millis)</code> 
		''' <exception cref="IllegalArgumentException"> if the nano argument is out of bounds </exception>
		<Obsolete("instead use the constructor <code>Timestamp(long millis)</code>")> _
		Public Sub New(  year As Integer,   month As Integer,   [date] As Integer,   hour As Integer,   minute As Integer,   second As Integer,   nano As Integer)
			MyBase.New(year, month, date_Renamed, hour, minute, second)
			If nano > 999999999 OrElse nano < 0 Then Throw New IllegalArgumentException("nanos > 999999999 or < 0")
			nanos = nano
		End Sub

		''' <summary>
		''' Constructs a <code>Timestamp</code> object
		''' using a milliseconds time value. The
		''' integral seconds are stored in the underlying date value; the
		''' fractional seconds are stored in the <code>nanos</code> field of
		''' the <code>Timestamp</code> object.
		''' </summary>
		''' <param name="time"> milliseconds since January 1, 1970, 00:00:00 GMT.
		'''        A negative number is the number of milliseconds before
		'''         January 1, 1970, 00:00:00 GMT. </param>
		''' <seealso cref= java.util.Calendar </seealso>
		Public Sub New(  time_Renamed As Long)
			MyBase.New((time_Renamed\1000)*1000)
			nanos = CInt(Fix((time Mod 1000) * 1000000))
			If nanos < 0 Then
				nanos = 1000000000 + nanos
				MyBase.time = ((time_Renamed\1000)-1)*1000
			End If
		End Sub

		''' <summary>
		''' Sets this <code>Timestamp</code> object to represent a point in time that is
		''' <tt>time</tt> milliseconds after January 1, 1970 00:00:00 GMT.
		''' </summary>
		''' <param name="time">   the number of milliseconds. </param>
		''' <seealso cref= #getTime </seealso>
		''' <seealso cref= #Timestamp(long time) </seealso>
		''' <seealso cref= java.util.Calendar </seealso>
		Public Overrides Property time As Long
			Set(  time_Renamed As Long)
				MyBase.time = (time_Renamed\1000)*1000
				nanos = CInt(Fix((time Mod 1000) * 1000000))
				If nanos < 0 Then
					nanos = 1000000000 + nanos
					MyBase.time = ((time_Renamed\1000)-1)*1000
				End If
			End Set
			Get
				Dim time_Renamed As Long = MyBase.time
				Return (time_Renamed + (nanos \ 1000000))
			End Get
		End Property



		''' <summary>
		''' @serial
		''' </summary>
		Private nanos As Integer

		''' <summary>
		''' Converts a <code>String</code> object in JDBC timestamp escape format to a
		''' <code>Timestamp</code> value.
		''' </summary>
		''' <param name="s"> timestamp in format <code>yyyy-[m]m-[d]d hh:mm:ss[.f...]</code>.  The
		''' fractional seconds may be omitted. The leading zero for <code>mm</code>
		''' and <code>dd</code> may also be omitted.
		''' </param>
		''' <returns> corresponding <code>Timestamp</code> value </returns>
		''' <exception cref="java.lang.IllegalArgumentException"> if the given argument
		''' does not have the format <code>yyyy-[m]m-[d]d hh:mm:ss[.f...]</code> </exception>
		Public Shared Function valueOf(  s As String) As Timestamp
			Const YEAR_LENGTH As Integer = 4
			Const MONTH_LENGTH As Integer = 2
			Const DAY_LENGTH As Integer = 2
			Const MAX_MONTH As Integer = 12
			Const MAX_DAY As Integer = 31
			Dim date_s As String
			Dim time_s As String
			Dim nanos_s As String
			Dim year_Renamed As Integer = 0
			Dim month_Renamed As Integer = 0
			Dim day_Renamed As Integer = 0
			Dim hour As Integer
			Dim minute As Integer
			Dim second As Integer
			Dim a_nanos As Integer = 0
			Dim firstDash As Integer
			Dim secondDash As Integer
			Dim dividingSpace As Integer
			Dim firstColon As Integer = 0
			Dim secondColon As Integer = 0
			Dim period As Integer = 0
			Dim formatError As String = "Timestamp format must be yyyy-mm-dd hh:mm:ss[.fffffffff]"
			Dim zeros As String = "000000000"
			Dim delimiterDate As String = "-"
			Dim delimiterTime As String = ":"

			If s Is Nothing Then Throw New System.ArgumentException("null string")

			' Split the string into date and time components
			s = s.Trim()
			dividingSpace = s.IndexOf(" "c)
			If dividingSpace > 0 Then
				date_s = s.Substring(0,dividingSpace)
				time_s = s.Substring(dividingSpace+1)
			Else
				Throw New System.ArgumentException(formatError)
			End If

			' Parse the date
			firstDash = date_s.IndexOf("-"c)
			secondDash = date_s.IndexOf("-"c, firstDash+1)

			' Parse the time
			If time_s Is Nothing Then Throw New System.ArgumentException(formatError)
			firstColon = time_s.IndexOf(":"c)
			secondColon = time_s.IndexOf(":"c, firstColon+1)
			period = time_s.IndexOf("."c, secondColon+1)

			' Convert the date
			Dim parsedDate As Boolean = False
			If (firstDash > 0) AndAlso (secondDash > 0) AndAlso (secondDash < date_s.length() - 1) Then
				Dim yyyy As String = date_s.Substring(0, firstDash)
				Dim mm As String = date_s.Substring(firstDash + 1, secondDash - (firstDash + 1))
				Dim dd As String = date_s.Substring(secondDash + 1)
				If yyyy.length() = YEAR_LENGTH AndAlso (mm.length() >= 1 AndAlso mm.length() <= MONTH_LENGTH) AndAlso (dd.length() >= 1 AndAlso dd.length() <= DAY_LENGTH) Then
					 year_Renamed = Convert.ToInt32(yyyy)
					 month_Renamed = Convert.ToInt32(mm)
					 day_Renamed = Convert.ToInt32(dd)

					If (month_Renamed >= 1 AndAlso month_Renamed <= MAX_MONTH) AndAlso (day_Renamed >= 1 AndAlso day_Renamed <= MAX_DAY) Then parsedDate = True
				End If
			End If
			If Not parsedDate Then Throw New System.ArgumentException(formatError)

			' Convert the time; default missing nanos
			If (firstColon > 0) And (secondColon > 0) And (secondColon < time_s.length()-1) Then
				hour = Convert.ToInt32(time_s.Substring(0, firstColon))
				minute = Convert.ToInt32(time_s.Substring(firstColon+1, secondColon - (firstColon+1)))
				If (period > 0) And (period < time_s.length()-1) Then
					second = Convert.ToInt32(time_s.Substring(secondColon+1, period - (secondColon+1)))
					nanos_s = time_s.Substring(period+1)
					If nanos_s.length() > 9 Then Throw New System.ArgumentException(formatError)
					If Not Char.IsDigit(nanos_s.Chars(0)) Then Throw New System.ArgumentException(formatError)
					nanos_s = nanos_s + zeros.Substring(0,9-nanos_s.length())
					a_nanos = Convert.ToInt32(nanos_s)
				ElseIf period > 0 Then
					Throw New System.ArgumentException(formatError)
				Else
					second = Convert.ToInt32(time_s.Substring(secondColon+1))
				End If
			Else
				Throw New System.ArgumentException(formatError)
			End If

			Return New Timestamp(year_Renamed - 1900, month_Renamed - 1, day_Renamed, hour, minute, second, a_nanos)
		End Function

		''' <summary>
		''' Formats a timestamp in JDBC timestamp escape format.
		'''         <code>yyyy-mm-dd hh:mm:ss.fffffffff</code>,
		''' where <code>ffffffffff</code> indicates nanoseconds.
		''' <P> </summary>
		''' <returns> a <code>String</code> object in
		'''           <code>yyyy-mm-dd hh:mm:ss.fffffffff</code> format </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overrides Function ToString() As String

			Dim year_Renamed As Integer = MyBase.year + 1900
			Dim month_Renamed As Integer = MyBase.month + 1
			Dim day_Renamed As Integer = MyBase.date
			Dim hour As Integer = MyBase.hours
			Dim minute As Integer = MyBase.minutes
			Dim second As Integer = MyBase.seconds
			Dim yearString As String
			Dim monthString As String
			Dim dayString As String
			Dim hourString As String
			Dim minuteString As String
			Dim secondString As String
			Dim nanosString As String
			Dim zeros As String = "000000000"
			Dim yearZeros As String = "0000"
			Dim timestampBuf As StringBuffer

			If year_Renamed < 1000 Then
				' Add leading zeros
				yearString = "" & year_Renamed
				yearString = yearZeros.Substring(0, (4-yearString.length())) + yearString
			Else
				yearString = "" & year_Renamed
			End If
			If month_Renamed < 10 Then
				monthString = "0" & month_Renamed
			Else
				monthString = Convert.ToString(month_Renamed)
			End If
			If day_Renamed < 10 Then
				dayString = "0" & day_Renamed
			Else
				dayString = Convert.ToString(day_Renamed)
			End If
			If hour < 10 Then
				hourString = "0" & hour
			Else
				hourString = Convert.ToString(hour)
			End If
			If minute < 10 Then
				minuteString = "0" & minute
			Else
				minuteString = Convert.ToString(minute)
			End If
			If second < 10 Then
				secondString = "0" & second
			Else
				secondString = Convert.ToString(second)
			End If
			If nanos = 0 Then
				nanosString = "0"
			Else
				nanosString = Convert.ToString(nanos)

				' Add leading zeros
				nanosString = zeros.Substring(0, (9-nanosString.length())) + nanosString

				' Truncate trailing zeros
				Dim nanosChar As Char() = New Char(nanosString.length() - 1){}
				nanosString.getChars(0, nanosString.length(), nanosChar, 0)
				Dim truncIndex As Integer = 8
				Do While nanosChar(truncIndex) = "0"c
					truncIndex -= 1
				Loop

				nanosString = New String(nanosChar, 0, truncIndex + 1)
			End If

			' do a string buffer here instead.
			timestampBuf = New StringBuffer(20+nanosString.length())
			timestampBuf.append(yearString)
			timestampBuf.append("-")
			timestampBuf.append(monthString)
			timestampBuf.append("-")
			timestampBuf.append(dayString)
			timestampBuf.append(" ")
			timestampBuf.append(hourString)
			timestampBuf.append(":")
			timestampBuf.append(minuteString)
			timestampBuf.append(":")
			timestampBuf.append(secondString)
			timestampBuf.append(".")
			timestampBuf.append(nanosString)

			Return (timestampBuf.ToString())
		End Function

		''' <summary>
		''' Gets this <code>Timestamp</code> object's <code>nanos</code> value.
		''' </summary>
		''' <returns> this <code>Timestamp</code> object's fractional seconds component </returns>
		''' <seealso cref= #setNanos </seealso>
		Public Overridable Property nanos As Integer
			Get
				Return nanos
			End Get
			Set(  n As Integer)
				If n > 999999999 OrElse n < 0 Then Throw New IllegalArgumentException("nanos > 999999999 or < 0")
				nanos = n
			End Set
		End Property


		''' <summary>
		''' Tests to see if this <code>Timestamp</code> object is
		''' equal to the given <code>Timestamp</code> object.
		''' </summary>
		''' <param name="ts"> the <code>Timestamp</code> value to compare with </param>
		''' <returns> <code>true</code> if the given <code>Timestamp</code>
		'''         object is equal to this <code>Timestamp</code> object;
		'''         <code>false</code> otherwise </returns>
		Public Overrides Function Equals(  ts As Timestamp) As Boolean
			If MyBase.Equals(ts) Then
				If nanos = ts.nanos Then
					Return True
				Else
					Return False
				End If
			Else
				Return False
			End If
		End Function

		''' <summary>
		''' Tests to see if this <code>Timestamp</code> object is
		''' equal to the given object.
		''' 
		''' This version of the method <code>equals</code> has been added
		''' to fix the incorrect
		''' signature of <code>Timestamp.equals(Timestamp)</code> and to preserve backward
		''' compatibility with existing class files.
		''' 
		''' Note: This method is not symmetric with respect to the
		''' <code>equals(Object)</code> method in the base class.
		''' </summary>
		''' <param name="ts"> the <code>Object</code> value to compare with </param>
		''' <returns> <code>true</code> if the given <code>Object</code> is an instance
		'''         of a <code>Timestamp</code> that
		'''         is equal to this <code>Timestamp</code> object;
		'''         <code>false</code> otherwise </returns>
		Public Overrides Function Equals(  ts As Object) As Boolean
		  If TypeOf ts Is Timestamp Then
			Return Me.Equals(CType(ts, Timestamp))
		  Else
			Return False
		  End If
		End Function

		''' <summary>
		''' Indicates whether this <code>Timestamp</code> object is
		''' earlier than the given <code>Timestamp</code> object.
		''' </summary>
		''' <param name="ts"> the <code>Timestamp</code> value to compare with </param>
		''' <returns> <code>true</code> if this <code>Timestamp</code> object is earlier;
		'''        <code>false</code> otherwise </returns>
		Public Overridable Function before(  ts As Timestamp) As Boolean
			Return compareTo(ts) < 0
		End Function

		''' <summary>
		''' Indicates whether this <code>Timestamp</code> object is
		''' later than the given <code>Timestamp</code> object.
		''' </summary>
		''' <param name="ts"> the <code>Timestamp</code> value to compare with </param>
		''' <returns> <code>true</code> if this <code>Timestamp</code> object is later;
		'''        <code>false</code> otherwise </returns>
		Public Overridable Function after(  ts As Timestamp) As Boolean
			Return compareTo(ts) > 0
		End Function

		''' <summary>
		''' Compares this <code>Timestamp</code> object to the given
		''' <code>Timestamp</code> object.
		''' </summary>
		''' <param name="ts">   the <code>Timestamp</code> object to be compared to
		'''                this <code>Timestamp</code> object </param>
		''' <returns>  the value <code>0</code> if the two <code>Timestamp</code>
		'''          objects are equal; a value less than <code>0</code> if this
		'''          <code>Timestamp</code> object is before the given argument;
		'''          and a value greater than <code>0</code> if this
		'''          <code>Timestamp</code> object is after the given argument.
		''' @since   1.4 </returns>
		Public Overridable Function compareTo(  ts As Timestamp) As Integer
			Dim thisTime As Long = Me.time
			Dim anotherTime As Long = ts.time
			Dim i As Integer = (If(thisTime<anotherTime, -1, (If(thisTime=anotherTime, 0, 1))))
			If i = 0 Then
				If nanos > ts.nanos Then
						Return 1
				ElseIf nanos < ts.nanos Then
					Return -1
				End If
			End If
			Return i
		End Function

		''' <summary>
		''' Compares this <code>Timestamp</code> object to the given
		''' <code>Date</code> object.
		''' </summary>
		''' <param name="o"> the <code>Date</code> to be compared to
		'''          this <code>Timestamp</code> object </param>
		''' <returns>  the value <code>0</code> if this <code>Timestamp</code> object
		'''          and the given object are equal; a value less than <code>0</code>
		'''          if this  <code>Timestamp</code> object is before the given argument;
		'''          and a value greater than <code>0</code> if this
		'''          <code>Timestamp</code> object is after the given argument.
		''' 
		''' @since   1.5 </returns>
		Public Overridable Function compareTo(  o As DateTime?) As Integer
		   If TypeOf o Is Timestamp Then
				' When Timestamp instance compare it with a Timestamp
				' Hence it is basically calling this.compareTo((Timestamp))o);
				' Note typecasting is safe because o is instance of Timestamp
			   Return compareTo(CType(o, Timestamp))
		  Else
				' When Date doing a o.compareTo(this)
				' will give wrong results.
			  Dim ts As New Timestamp(o.Value.time)
			  Return Me.CompareTo(ts)
		  End If
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' 
		''' The {@code hashCode} method uses the underlying {@code java.util.Date}
		''' implementation and therefore does not include nanos in its computation.
		''' 
		''' </summary>
		Public Overrides Function GetHashCode() As Integer
			Return MyBase.GetHashCode()
		End Function

		Friend Const serialVersionUID As Long = 2745179027874758501L

		Private Const MILLIS_PER_SECOND As Integer = 1000

		''' <summary>
		''' Obtains an instance of {@code Timestamp} from a {@code LocalDateTime}
		''' object, with the same year, month, day of month, hours, minutes,
		''' seconds and nanos date-time value as the provided {@code LocalDateTime}.
		''' <p>
		''' The provided {@code LocalDateTime} is interpreted as the local
		''' date-time in the local time zone.
		''' </summary>
		''' <param name="dateTime"> a {@code LocalDateTime} to convert </param>
		''' <returns> a {@code Timestamp} object </returns>
		''' <exception cref="NullPointerException"> if {@code dateTime} is null.
		''' @since 1.8 </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Shared Function valueOf(  dateTime As java.time.LocalDateTime) As Timestamp
			Return New Timestamp(dateTime.year - 1900, dateTime.monthValue - 1, dateTime.dayOfMonth, dateTime.hour, dateTime.minute, dateTime.second, dateTime.nano)
		End Function

		''' <summary>
		''' Converts this {@code Timestamp} object to a {@code LocalDateTime}.
		''' <p>
		''' The conversion creates a {@code LocalDateTime} that represents the
		''' same year, month, day of month, hours, minutes, seconds and nanos
		''' date-time value as this {@code Timestamp} in the local time zone.
		''' </summary>
		''' <returns> a {@code LocalDateTime} object representing the same date-time value
		''' @since 1.8 </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Function toLocalDateTime() As java.time.LocalDateTime
			Return java.time.LocalDateTime.of(year + 1900, month + 1, [date], hours, minutes, seconds, nanos)
		End Function

		''' <summary>
		''' Obtains an instance of {@code Timestamp} from an <seealso cref="Instant"/> object.
		''' <p>
		''' {@code Instant} can store points on the time-line further in the future
		''' and further in the past than {@code Date}. In this scenario, this method
		''' will throw an exception.
		''' </summary>
		''' <param name="instant">  the instant to convert </param>
		''' <returns> an {@code Timestamp} representing the same point on the time-line as
		'''  the provided instant </returns>
		''' <exception cref="NullPointerException"> if {@code instant} is null. </exception>
		''' <exception cref="IllegalArgumentException"> if the instant is too large to
		'''  represent as a {@code Timesamp}
		''' @since 1.8 </exception>
		Public Shared Function [from](  instant As java.time.Instant) As Timestamp
			Try
				Dim stamp As New Timestamp(instant.epochSecond * MILLIS_PER_SECOND)
				stamp.nanos = instant.nano
				Return stamp
			Catch ex As ArithmeticException
				Throw New IllegalArgumentException(ex)
			End Try
		End Function

		''' <summary>
		''' Converts this {@code Timestamp} object to an {@code Instant}.
		''' <p>
		''' The conversion creates an {@code Instant} that represents the same
		''' point on the time-line as this {@code Timestamp}.
		''' </summary>
		''' <returns> an instant representing the same point on the time-line
		''' @since 1.8 </returns>
		Public Overrides Function toInstant() As java.time.Instant
			Return java.time.Instant.ofEpochSecond(MyBase.time / MILLIS_PER_SECOND, nanos)
		End Function
	End Class

End Namespace
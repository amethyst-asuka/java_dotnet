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
	''' <P>A thin wrapper around a millisecond value that allows
	''' JDBC to identify this as an SQL <code>DATE</code> value.  A
	''' milliseconds value represents the number of milliseconds that
	''' have passed since January 1, 1970 00:00:00.000 GMT.
	''' <p>
	''' To conform with the definition of SQL <code>DATE</code>, the
	''' millisecond values wrapped by a <code>java.sql.Date</code> instance
	''' must be 'normalized' by setting the
	''' hours, minutes, seconds, and milliseconds to zero in the particular
	''' time zone with which the instance is associated.
	''' </summary>
	Public Class [Date]
		Inherits DateTime?

		''' <summary>
		''' Constructs a <code>Date</code> object initialized with the given
		''' year, month, and day.
		''' <P>
		''' The result is undefined if a given argument is out of bounds.
		''' </summary>
		''' <param name="year"> the year minus 1900; must be 0 to 8099. (Note that
		'''        8099 is 9999 minus 1900.) </param>
		''' <param name="month"> 0 to 11 </param>
		''' <param name="day"> 1 to 31 </param>
		''' @deprecated instead use the constructor <code>Date(long date)</code> 
		<Obsolete("instead use the constructor <code>Date(long date)</code>")> _
		Public Sub New(ByVal year As Integer, ByVal month As Integer, ByVal day As Integer)
			MyBase.New(year, month, day)
		End Sub

		''' <summary>
		''' Constructs a <code>Date</code> object using the given milliseconds
		''' time value.  If the given milliseconds value contains time
		''' information, the driver will set the time components to the
		''' time in the default time zone (the time zone of the Java virtual
		''' machine running the application) that corresponds to zero GMT.
		''' </summary>
		''' <param name="date"> milliseconds since January 1, 1970, 00:00:00 GMT not
		'''        to exceed the milliseconds representation for the year 8099.
		'''        A negative number indicates the number of milliseconds
		'''        before January 1, 1970, 00:00:00 GMT. </param>
		Public Sub New(ByVal [date] As Long)
			' If the millisecond date value contains time info, mask it out.
			MyBase.New(date_Renamed)

		End Sub

		''' <summary>
		''' Sets an existing <code>Date</code> object
		''' using the given milliseconds time value.
		''' If the given milliseconds value contains time information,
		''' the driver will set the time components to the
		''' time in the default time zone (the time zone of the Java virtual
		''' machine running the application) that corresponds to zero GMT.
		''' </summary>
		''' <param name="date"> milliseconds since January 1, 1970, 00:00:00 GMT not
		'''        to exceed the milliseconds representation for the year 8099.
		'''        A negative number indicates the number of milliseconds
		'''        before January 1, 1970, 00:00:00 GMT. </param>
		Public Overrides Property time As Long
			Set(ByVal [date] As Long)
				' If the millisecond date value contains time info, mask it out.
				MyBase.time = date_Renamed
			End Set
		End Property

		''' <summary>
		''' Converts a string in JDBC date escape format to
		''' a <code>Date</code> value.
		''' </summary>
		''' <param name="s"> a <code>String</code> object representing a date in
		'''        in the format "yyyy-[m]m-[d]d". The leading zero for <code>mm</code>
		''' and <code>dd</code> may also be omitted. </param>
		''' <returns> a <code>java.sql.Date</code> object representing the
		'''         given date </returns>
		''' <exception cref="IllegalArgumentException"> if the date given is not in the
		'''         JDBC date escape format (yyyy-[m]m-[d]d) </exception>
		Public Shared Function valueOf(ByVal s As String) As Date
			Const YEAR_LENGTH As Integer = 4
			Const MONTH_LENGTH As Integer = 2
			Const DAY_LENGTH As Integer = 2
			Const MAX_MONTH As Integer = 12
			Const MAX_DAY As Integer = 31
			Dim firstDash As Integer
			Dim secondDash As Integer
			Dim d As Date = Nothing
			If s Is Nothing Then Throw New System.ArgumentException

			firstDash = s.IndexOf("-"c)
			secondDash = s.IndexOf("-"c, firstDash + 1)

			If (firstDash > 0) AndAlso (secondDash > 0) AndAlso (secondDash < s.length() - 1) Then
				Dim yyyy As String = s.Substring(0, firstDash)
				Dim mm As String = s.Substring(firstDash + 1, secondDash - (firstDash + 1))
				Dim dd As String = s.Substring(secondDash + 1)
				If yyyy.length() = YEAR_LENGTH AndAlso (mm.length() >= 1 AndAlso mm.length() <= MONTH_LENGTH) AndAlso (dd.length() >= 1 AndAlso dd.length() <= DAY_LENGTH) Then
					Dim year As Integer = Convert.ToInt32(yyyy)
					Dim month As Integer = Convert.ToInt32(mm)
					Dim day As Integer = Convert.ToInt32(dd)

					If (month >= 1 AndAlso month <= MAX_MONTH) AndAlso (day >= 1 AndAlso day <= MAX_DAY) Then d = New Date(year - 1900, month - 1, day)
				End If
			End If
			If d Is Nothing Then Throw New System.ArgumentException

			Return d

		End Function


		''' <summary>
		''' Formats a date in the date escape format yyyy-mm-dd.
		''' <P> </summary>
		''' <returns> a String in yyyy-mm-dd format </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overrides Function ToString() As String
			Dim year As Integer = MyBase.year + 1900
			Dim month As Integer = MyBase.month + 1
			Dim day As Integer = MyBase.date

			Dim buf As Char() = "2000-00-00".ToCharArray()
			buf(0) = Character.forDigit(year\1000,10)
			buf(1) = Character.forDigit((year\100) Mod 10,10)
			buf(2) = Character.forDigit((year\10) Mod 10,10)
			buf(3) = Character.forDigit(year Mod 10,10)
			buf(5) = Character.forDigit(month\10,10)
			buf(6) = Character.forDigit(month Mod 10,10)
			buf(8) = Character.forDigit(day\10,10)
			buf(9) = Character.forDigit(day Mod 10,10)

			Return New String(buf)
		End Function

		' Override all the time operations inherited from java.util.Date;

	   ''' <summary>
	   ''' This method is deprecated and should not be used because SQL Date
	   ''' values do not have a time component.
	   ''' 
	   ''' @deprecated </summary>
	   ''' <exception cref="java.lang.IllegalArgumentException"> if this method is invoked </exception>
	   ''' <seealso cref= #setHours </seealso>
		<Obsolete> _
		Public  Overrides ReadOnly Property  hours As Integer
			Get
				Throw New System.ArgumentException
			End Get
			Set(ByVal i As Integer)
				Throw New System.ArgumentException
			End Set
		End Property

	   ''' <summary>
	   ''' This method is deprecated and should not be used because SQL Date
	   ''' values do not have a time component.
	   ''' 
	   ''' @deprecated </summary>
	   ''' <exception cref="java.lang.IllegalArgumentException"> if this method is invoked </exception>
	   ''' <seealso cref= #setMinutes </seealso>
		<Obsolete> _
		Public  Overrides ReadOnly Property  minutes As Integer
			Get
				Throw New System.ArgumentException
			End Get
			Set(ByVal i As Integer)
				Throw New System.ArgumentException
			End Set
		End Property

	   ''' <summary>
	   ''' This method is deprecated and should not be used because SQL Date
	   ''' values do not have a time component.
	   ''' 
	   ''' @deprecated </summary>
	   ''' <exception cref="java.lang.IllegalArgumentException"> if this method is invoked </exception>
	   ''' <seealso cref= #setSeconds </seealso>
		<Obsolete> _
		Public  Overrides ReadOnly Property  seconds As Integer
			Get
				Throw New System.ArgumentException
			End Get
			Set(ByVal i As Integer)
				Throw New System.ArgumentException
			End Set
		End Property




	   ''' <summary>
	   ''' Private serial version unique ID to ensure serialization
	   ''' compatibility.
	   ''' </summary>
		Friend Const serialVersionUID As Long = 1511598038487230103L

		''' <summary>
		''' Obtains an instance of {@code Date} from a <seealso cref="LocalDate"/> object
		''' with the same year, month and day of month value as the given
		''' {@code LocalDate}.
		''' <p>
		''' The provided {@code LocalDate} is interpreted as the local date
		''' in the local time zone.
		''' </summary>
		''' <param name="date"> a {@code LocalDate} to convert </param>
		''' <returns> a {@code Date} object </returns>
		''' <exception cref="NullPointerException"> if {@code date} is null
		''' @since 1.8 </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Shared Function valueOf(ByVal [date] As java.time.LocalDate) As Date
			Return New Date(date_Renamed.year - 1900, date_Renamed.monthValue -1, date_Renamed.dayOfMonth)
		End Function

		''' <summary>
		''' Converts this {@code Date} object to a {@code LocalDate}
		''' <p>
		''' The conversion creates a {@code LocalDate} that represents the same
		''' date value as this {@code Date} in local time zone
		''' </summary>
		''' <returns> a {@code LocalDate} object representing the same date value
		''' 
		''' @since 1.8 </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Function toLocalDate() As java.time.LocalDate
			Return java.time.LocalDate.of(year + 1900, month + 1, [date])
		End Function

	   ''' <summary>
	   ''' This method always throws an UnsupportedOperationException and should
	   ''' not be used because SQL {@code Date} values do not have a time
	   ''' component.
	   ''' </summary>
	   ''' <exception cref="java.lang.UnsupportedOperationException"> if this method is invoked </exception>
		Public Overrides Function toInstant() As java.time.Instant
			Throw New System.NotSupportedException
		End Function
	End Class

End Namespace
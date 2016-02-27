Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic

'
' * Copyright (c) 2012, 2015, Oracle and/or its affiliates. All rights reserved.
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
' * Copyright (c) 2008-2012, Stephen Colebourne & Michael Nascimento Santos
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
Namespace java.time



	''' <summary>
	''' A date-based amount of time in the ISO-8601 calendar system,
	''' such as '2 years, 3 months and 4 days'.
	''' <p>
	''' This class models a quantity or amount of time in terms of years, months and days.
	''' See <seealso cref="Duration"/> for the time-based equivalent to this class.
	''' <p>
	''' Durations and periods differ in their treatment of daylight savings time
	''' when added to <seealso cref="ZonedDateTime"/>. A {@code Duration} will add an exact
	''' number of seconds, thus a duration of one day is always exactly 24 hours.
	''' By contrast, a {@code Period} will add a conceptual day, trying to maintain
	''' the local time.
	''' <p>
	''' For example, consider adding a period of one day and a duration of one day to
	''' 18:00 on the evening before a daylight savings gap. The {@code Period} will add
	''' the conceptual day and result in a {@code ZonedDateTime} at 18:00 the following day.
	''' By contrast, the {@code Duration} will add exactly 24 hours, resulting in a
	''' {@code ZonedDateTime} at 19:00 the following day (assuming a one hour DST gap).
	''' <p>
	''' The supported units of a period are <seealso cref="ChronoUnit#YEARS YEARS"/>,
	''' <seealso cref="ChronoUnit#MONTHS MONTHS"/> and <seealso cref="ChronoUnit#DAYS DAYS"/>.
	''' All three fields are always present, but may be set to zero.
	''' <p>
	''' The ISO-8601 calendar system is the modern civil calendar system used today
	''' in most of the world. It is equivalent to the proleptic Gregorian calendar
	''' system, in which today's rules for leap years are applied for all time.
	''' <p>
	''' The period is modeled as a directed amount of time, meaning that individual parts of the
	''' period may be negative.
	''' 
	''' <p>
	''' This is a <a href="{@docRoot}/java/lang/doc-files/ValueBased.html">value-based</a>
	''' class; use of identity-sensitive operations (including reference equality
	''' ({@code ==}), identity hash code, or synchronization) on instances of
	''' {@code Period} may have unpredictable results and should be avoided.
	''' The {@code equals} method should be used for comparisons.
	''' 
	''' @implSpec
	''' This class is immutable and thread-safe.
	''' 
	''' @since 1.8
	''' </summary>
	<Serializable> _
	Public NotInheritable Class Period
		Implements java.time.chrono.ChronoPeriod

		''' <summary>
		''' A constant for a period of zero.
		''' </summary>
		Public Shared ReadOnly ZERO As New Period(0, 0, 0)
		''' <summary>
		''' Serialization version.
		''' </summary>
		Private Const serialVersionUID As Long = -3587258372562876L
		''' <summary>
		''' The pattern for parsing.
		''' </summary>
		Private Shared ReadOnly PATTERN As java.util.regex.Pattern = java.util.regex.Pattern.compile("([-+]?)P(?:([-+]?[0-9]+)Y)?(?:([-+]?[0-9]+)M)?(?:([-+]?[0-9]+)W)?(?:([-+]?[0-9]+)D)?", java.util.regex.Pattern.CASE_INSENSITIVE)

		''' <summary>
		''' The set of supported units.
		''' </summary>
		Private Shared ReadOnly SUPPORTED_UNITS As IList(Of java.time.temporal.TemporalUnit) = java.util.Collections.unmodifiableList(java.util.Arrays.asList(Of java.time.temporal.TemporalUnit)(YEARS, MONTHS, DAYS))

		''' <summary>
		''' The number of years.
		''' </summary>
		Private ReadOnly years As Integer
		''' <summary>
		''' The number of months.
		''' </summary>
		Private ReadOnly months As Integer
		''' <summary>
		''' The number of days.
		''' </summary>
		Private ReadOnly days As Integer

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains a {@code Period} representing a number of years.
		''' <p>
		''' The resulting period will have the specified years.
		''' The months and days units will be zero.
		''' </summary>
		''' <param name="years">  the number of years, positive or negative </param>
		''' <returns> the period of years, not null </returns>
		Public Shared Function ofYears(ByVal years As Integer) As Period
			Return create(years, 0, 0)
		End Function

		''' <summary>
		''' Obtains a {@code Period} representing a number of months.
		''' <p>
		''' The resulting period will have the specified months.
		''' The years and days units will be zero.
		''' </summary>
		''' <param name="months">  the number of months, positive or negative </param>
		''' <returns> the period of months, not null </returns>
		Public Shared Function ofMonths(ByVal months As Integer) As Period
			Return create(0, months, 0)
		End Function

		''' <summary>
		''' Obtains a {@code Period} representing a number of weeks.
		''' <p>
		''' The resulting period will be day-based, with the amount of days
		''' equal to the number of weeks multiplied by 7.
		''' The years and months units will be zero.
		''' </summary>
		''' <param name="weeks">  the number of weeks, positive or negative </param>
		''' <returns> the period, with the input weeks converted to days, not null </returns>
		Public Shared Function ofWeeks(ByVal weeks As Integer) As Period
			Return create(0, 0, System.Math.multiplyExact(weeks, 7))
		End Function

		''' <summary>
		''' Obtains a {@code Period} representing a number of days.
		''' <p>
		''' The resulting period will have the specified days.
		''' The years and months units will be zero.
		''' </summary>
		''' <param name="days">  the number of days, positive or negative </param>
		''' <returns> the period of days, not null </returns>
		Public Shared Function ofDays(ByVal days As Integer) As Period
			Return create(0, 0, days)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains a {@code Period} representing a number of years, months and days.
		''' <p>
		''' This creates an instance based on years, months and days.
		''' </summary>
		''' <param name="years">  the amount of years, may be negative </param>
		''' <param name="months">  the amount of months, may be negative </param>
		''' <param name="days">  the amount of days, may be negative </param>
		''' <returns> the period of years, months and days, not null </returns>
		Public Shared Function [of](ByVal years As Integer, ByVal months As Integer, ByVal days As Integer) As Period
			Return create(years, months, days)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains an instance of {@code Period} from a temporal amount.
		''' <p>
		''' This obtains a period based on the specified amount.
		''' A {@code TemporalAmount} represents an  amount of time, which may be
		''' date-based or time-based, which this factory extracts to a {@code Period}.
		''' <p>
		''' The conversion loops around the set of units from the amount and uses
		''' the <seealso cref="ChronoUnit#YEARS YEARS"/>, <seealso cref="ChronoUnit#MONTHS MONTHS"/>
		''' and <seealso cref="ChronoUnit#DAYS DAYS"/> units to create a period.
		''' If any other units are found then an exception is thrown.
		''' <p>
		''' If the amount is a {@code ChronoPeriod} then it must use the ISO chronology.
		''' </summary>
		''' <param name="amount">  the temporal amount to convert, not null </param>
		''' <returns> the equivalent period, not null </returns>
		''' <exception cref="DateTimeException"> if unable to convert to a {@code Period} </exception>
		''' <exception cref="ArithmeticException"> if the amount of years, months or days exceeds an int </exception>
		Public Shared Function [from](ByVal amount As java.time.temporal.TemporalAmount) As Period
			If TypeOf amount Is Period Then Return CType(amount, Period)
			If TypeOf amount Is java.time.chrono.ChronoPeriod Then
				If java.time.chrono.IsoChronology.INSTANCE.Equals(CType(amount, java.time.chrono.ChronoPeriod).chronology) = False Then Throw New DateTimeException("Period requires ISO chronology: " & amount)
			End If
			java.util.Objects.requireNonNull(amount, "amount")
			Dim years_Renamed As Integer = 0
			Dim months_Renamed As Integer = 0
			Dim days_Renamed As Integer = 0
			For Each unit As java.time.temporal.TemporalUnit In amount.units
				Dim unitAmount As Long = amount.get(unit)
				If unit = java.time.temporal.ChronoUnit.YEARS Then
					years_Renamed = System.Math.toIntExact(unitAmount)
				ElseIf unit = java.time.temporal.ChronoUnit.MONTHS Then
					months_Renamed = System.Math.toIntExact(unitAmount)
				ElseIf unit = java.time.temporal.ChronoUnit.DAYS Then
					days_Renamed = System.Math.toIntExact(unitAmount)
				Else
					Throw New DateTimeException("Unit must be Years, Months or Days, but was " & unit)
				End If
			Next unit
			Return create(years_Renamed, months_Renamed, days_Renamed)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains a {@code Period} from a text string such as {@code PnYnMnD}.
		''' <p>
		''' This will parse the string produced by {@code toString()} which is
		''' based on the ISO-8601 period formats {@code PnYnMnD} and {@code PnW}.
		''' <p>
		''' The string starts with an optional sign, denoted by the ASCII negative
		''' or positive symbol. If negative, the whole period is negated.
		''' The ASCII letter "P" is next in upper or lower case.
		''' There are then four sections, each consisting of a number and a suffix.
		''' At least one of the four sections must be present.
		''' The sections have suffixes in ASCII of "Y", "M", "W" and "D" for
		''' years, months, weeks and days, accepted in upper or lower case.
		''' The suffixes must occur in order.
		''' The number part of each section must consist of ASCII digits.
		''' The number may be prefixed by the ASCII negative or positive symbol.
		''' The number must parse to an {@code int}.
		''' <p>
		''' The leading plus/minus sign, and negative values for other units are
		''' not part of the ISO-8601 standard. In addition, ISO-8601 does not
		''' permit mixing between the {@code PnYnMnD} and {@code PnW} formats.
		''' Any week-based input is multiplied by 7 and treated as a number of days.
		''' <p>
		''' For example, the following are valid inputs:
		''' <pre>
		'''   "P2Y"             -- Period.ofYears(2)
		'''   "P3M"             -- Period.ofMonths(3)
		'''   "P4W"             -- Period.ofWeeks(4)
		'''   "P5D"             -- Period.ofDays(5)
		'''   "P1Y2M3D"         -- Period.of(1, 2, 3)
		'''   "P1Y2M3W4D"       -- Period.of(1, 2, 25)
		'''   "P-1Y2M"          -- Period.of(-1, 2, 0)
		'''   "-P1Y2M"          -- Period.of(-1, -2, 0)
		''' </pre>
		''' </summary>
		''' <param name="text">  the text to parse, not null </param>
		''' <returns> the parsed period, not null </returns>
		''' <exception cref="DateTimeParseException"> if the text cannot be parsed to a period </exception>
		Public Shared Function parse(ByVal text As CharSequence) As Period
			java.util.Objects.requireNonNull(text, "text")
			Dim matcher As java.util.regex.Matcher = PATTERN.matcher(text)
			If matcher.matches() Then
				Dim negate As Integer = (If("-".Equals(matcher.group(1)), -1, 1))
				Dim yearMatch As String = matcher.group(2)
				Dim monthMatch As String = matcher.group(3)
				Dim weekMatch As String = matcher.group(4)
				Dim dayMatch As String = matcher.group(5)
				If yearMatch IsNot Nothing OrElse monthMatch IsNot Nothing OrElse dayMatch IsNot Nothing OrElse weekMatch IsNot Nothing Then
					Try
						Dim years_Renamed As Integer = parseNumber(text, yearMatch, negate)
						Dim months_Renamed As Integer = parseNumber(text, monthMatch, negate)
						Dim weeks As Integer = parseNumber(text, weekMatch, negate)
						Dim days_Renamed As Integer = parseNumber(text, dayMatch, negate)
						days_Renamed = System.Math.addExact(days_Renamed, System.Math.multiplyExact(weeks, 7))
						Return create(years_Renamed, months_Renamed, days_Renamed)
					Catch ex As NumberFormatException
						Throw New java.time.format.DateTimeParseException("Text cannot be parsed to a Period", text, 0, ex)
					End Try
				End If
			End If
			Throw New java.time.format.DateTimeParseException("Text cannot be parsed to a Period", text, 0)
		End Function

		Private Shared Function parseNumber(ByVal text As CharSequence, ByVal str As String, ByVal negate As Integer) As Integer
			If str Is Nothing Then Return 0
			Dim val As Integer = Convert.ToInt32(str)
			Try
				Return System.Math.multiplyExact(val, negate)
			Catch ex As ArithmeticException
				Throw New java.time.format.DateTimeParseException("Text cannot be parsed to a Period", text, 0, ex)
			End Try
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains a {@code Period} consisting of the number of years, months,
		''' and days between two dates.
		''' <p>
		''' The start date is included, but the end date is not.
		''' The period is calculated by removing complete months, then calculating
		''' the remaining number of days, adjusting to ensure that both have the same sign.
		''' The number of months is then split into years and months based on a 12 month year.
		''' A month is considered if the end day-of-month is greater than or equal to the start day-of-month.
		''' For example, from {@code 2010-01-15} to {@code 2011-03-18} is one year, two months and three days.
		''' <p>
		''' The result of this method can be a negative period if the end is before the start.
		''' The negative sign will be the same in each of year, month and day.
		''' </summary>
		''' <param name="startDateInclusive">  the start date, inclusive, not null </param>
		''' <param name="endDateExclusive">  the end date, exclusive, not null </param>
		''' <returns> the period between this date and the end date, not null </returns>
		''' <seealso cref= ChronoLocalDate#until(ChronoLocalDate) </seealso>
		Public Shared Function between(ByVal startDateInclusive As LocalDate, ByVal endDateExclusive As LocalDate) As Period
			Return startDateInclusive.until(endDateExclusive)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Creates an instance.
		''' </summary>
		''' <param name="years">  the amount </param>
		''' <param name="months">  the amount </param>
		''' <param name="days">  the amount </param>
		Private Shared Function create(ByVal years As Integer, ByVal months As Integer, ByVal days As Integer) As Period
			If (years Or months Or days) = 0 Then Return ZERO
			Return New Period(years, months, days)
		End Function

		''' <summary>
		''' Constructor.
		''' </summary>
		''' <param name="years">  the amount </param>
		''' <param name="months">  the amount </param>
		''' <param name="days">  the amount </param>
		Private Sub New(ByVal years As Integer, ByVal months As Integer, ByVal days As Integer)
			Me.years = years
			Me.months = months
			Me.days = days
		End Sub

		'-----------------------------------------------------------------------
		''' <summary>
		''' Gets the value of the requested unit.
		''' <p>
		''' This returns a value for each of the three supported units,
		''' <seealso cref="ChronoUnit#YEARS YEARS"/>, <seealso cref="ChronoUnit#MONTHS MONTHS"/> and
		''' <seealso cref="ChronoUnit#DAYS DAYS"/>.
		''' All other units throw an exception.
		''' </summary>
		''' <param name="unit"> the {@code TemporalUnit} for which to return the value </param>
		''' <returns> the long value of the unit </returns>
		''' <exception cref="DateTimeException"> if the unit is not supported </exception>
		''' <exception cref="UnsupportedTemporalTypeException"> if the unit is not supported </exception>
		Public Overrides Function [get](ByVal unit As java.time.temporal.TemporalUnit) As Long
			If unit = java.time.temporal.ChronoUnit.YEARS Then
				Return years
			ElseIf unit = java.time.temporal.ChronoUnit.MONTHS Then
				Return months
			ElseIf unit = java.time.temporal.ChronoUnit.DAYS Then
				Return days
			Else
				Throw New java.time.temporal.UnsupportedTemporalTypeException("Unsupported unit: " & unit)
			End If
		End Function

		''' <summary>
		''' Gets the set of units supported by this period.
		''' <p>
		''' The supported units are <seealso cref="ChronoUnit#YEARS YEARS"/>,
		''' <seealso cref="ChronoUnit#MONTHS MONTHS"/> and <seealso cref="ChronoUnit#DAYS DAYS"/>.
		''' They are returned in the order years, months, days.
		''' <p>
		''' This set can be used in conjunction with <seealso cref="#get(TemporalUnit)"/>
		''' to access the entire state of the period.
		''' </summary>
		''' <returns> a list containing the years, months and days units, not null </returns>
		Public Property Overrides units As IList(Of java.time.temporal.TemporalUnit)
			Get
				Return SUPPORTED_UNITS
			End Get
		End Property

		''' <summary>
		''' Gets the chronology of this period, which is the ISO calendar system.
		''' <p>
		''' The {@code Chronology} represents the calendar system in use.
		''' The ISO-8601 calendar system is the modern civil calendar system used today
		''' in most of the world. It is equivalent to the proleptic Gregorian calendar
		''' system, in which today's rules for leap years are applied for all time.
		''' </summary>
		''' <returns> the ISO chronology, not null </returns>
		Public Property Overrides chronology As java.time.chrono.IsoChronology
			Get
				Return java.time.chrono.IsoChronology.INSTANCE
			End Get
		End Property

		'-----------------------------------------------------------------------
		''' <summary>
		''' Checks if all three units of this period are zero.
		''' <p>
		''' A zero period has the value zero for the years, months and days units.
		''' </summary>
		''' <returns> true if this period is zero-length </returns>
		Public Property zero As Boolean
			Get
				Return (Me Is ZERO)
			End Get
		End Property

		''' <summary>
		''' Checks if any of the three units of this period are negative.
		''' <p>
		''' This checks whether the years, months or days units are less than zero.
		''' </summary>
		''' <returns> true if any unit of this period is negative </returns>
		Public Property negative As Boolean
			Get
				Return years < 0 OrElse months < 0 OrElse days < 0
			End Get
		End Property

		'-----------------------------------------------------------------------
		''' <summary>
		''' Gets the amount of years of this period.
		''' <p>
		''' This returns the years unit.
		''' <p>
		''' The months unit is not automatically normalized with the years unit.
		''' This means that a period of "15 months" is different to a period
		''' of "1 year and 3 months".
		''' </summary>
		''' <returns> the amount of years of this period, may be negative </returns>
		Public Property years As Integer
			Get
				Return years
			End Get
		End Property

		''' <summary>
		''' Gets the amount of months of this period.
		''' <p>
		''' This returns the months unit.
		''' <p>
		''' The months unit is not automatically normalized with the years unit.
		''' This means that a period of "15 months" is different to a period
		''' of "1 year and 3 months".
		''' </summary>
		''' <returns> the amount of months of this period, may be negative </returns>
		Public Property months As Integer
			Get
				Return months
			End Get
		End Property

		''' <summary>
		''' Gets the amount of days of this period.
		''' <p>
		''' This returns the days unit.
		''' </summary>
		''' <returns> the amount of days of this period, may be negative </returns>
		Public Property days As Integer
			Get
				Return days
			End Get
		End Property

		'-----------------------------------------------------------------------
		''' <summary>
		''' Returns a copy of this period with the specified amount of years.
		''' <p>
		''' This sets the amount of the years unit in a copy of this period.
		''' The months and days units are unaffected.
		''' <p>
		''' The months unit is not automatically normalized with the years unit.
		''' This means that a period of "15 months" is different to a period
		''' of "1 year and 3 months".
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="years">  the years to represent, may be negative </param>
		''' <returns> a {@code Period} based on this period with the requested years, not null </returns>
		Public Function withYears(ByVal years As Integer) As Period
			If years = Me.years Then Return Me
			Return create(years, months, days)
		End Function

		''' <summary>
		''' Returns a copy of this period with the specified amount of months.
		''' <p>
		''' This sets the amount of the months unit in a copy of this period.
		''' The years and days units are unaffected.
		''' <p>
		''' The months unit is not automatically normalized with the years unit.
		''' This means that a period of "15 months" is different to a period
		''' of "1 year and 3 months".
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="months">  the months to represent, may be negative </param>
		''' <returns> a {@code Period} based on this period with the requested months, not null </returns>
		Public Function withMonths(ByVal months As Integer) As Period
			If months = Me.months Then Return Me
			Return create(years, months, days)
		End Function

		''' <summary>
		''' Returns a copy of this period with the specified amount of days.
		''' <p>
		''' This sets the amount of the days unit in a copy of this period.
		''' The years and months units are unaffected.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="days">  the days to represent, may be negative </param>
		''' <returns> a {@code Period} based on this period with the requested days, not null </returns>
		Public Function withDays(ByVal days As Integer) As Period
			If days = Me.days Then Return Me
			Return create(years, months, days)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Returns a copy of this period with the specified period added.
		''' <p>
		''' This operates separately on the years, months and days.
		''' No normalization is performed.
		''' <p>
		''' For example, "1 year, 6 months and 3 days" plus "2 years, 2 months and 2 days"
		''' returns "3 years, 8 months and 5 days".
		''' <p>
		''' The specified amount is typically an instance of {@code Period}.
		''' Other types are interpreted using <seealso cref="Period#from(TemporalAmount)"/>.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="amountToAdd">  the amount to add, not null </param>
		''' <returns> a {@code Period} based on this period with the requested period added, not null </returns>
		''' <exception cref="DateTimeException"> if the specified amount has a non-ISO chronology or
		'''  contains an invalid unit </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Function plus(ByVal amountToAdd As java.time.temporal.TemporalAmount) As Period
			Dim isoAmount As Period = Period.from(amountToAdd)
			Return create (System.Math.addExact(years, isoAmount.years), System.Math.addExact(months, isoAmount.months), System.Math.addExact(days, isoAmount.days))
		End Function

		''' <summary>
		''' Returns a copy of this period with the specified years added.
		''' <p>
		''' This adds the amount to the years unit in a copy of this period.
		''' The months and days units are unaffected.
		''' For example, "1 year, 6 months and 3 days" plus 2 years returns "3 years, 6 months and 3 days".
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="yearsToAdd">  the years to add, positive or negative </param>
		''' <returns> a {@code Period} based on this period with the specified years added, not null </returns>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Function plusYears(ByVal yearsToAdd As Long) As Period
			If yearsToAdd = 0 Then Return Me
			Return create (System.Math.toIntExact (System.Math.addExact(years, yearsToAdd)), months, days)
		End Function

		''' <summary>
		''' Returns a copy of this period with the specified months added.
		''' <p>
		''' This adds the amount to the months unit in a copy of this period.
		''' The years and days units are unaffected.
		''' For example, "1 year, 6 months and 3 days" plus 2 months returns "1 year, 8 months and 3 days".
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="monthsToAdd">  the months to add, positive or negative </param>
		''' <returns> a {@code Period} based on this period with the specified months added, not null </returns>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Function plusMonths(ByVal monthsToAdd As Long) As Period
			If monthsToAdd = 0 Then Return Me
			Return create(years, System.Math.toIntExact (System.Math.addExact(months, monthsToAdd)), days)
		End Function

		''' <summary>
		''' Returns a copy of this period with the specified days added.
		''' <p>
		''' This adds the amount to the days unit in a copy of this period.
		''' The years and months units are unaffected.
		''' For example, "1 year, 6 months and 3 days" plus 2 days returns "1 year, 6 months and 5 days".
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="daysToAdd">  the days to add, positive or negative </param>
		''' <returns> a {@code Period} based on this period with the specified days added, not null </returns>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Function plusDays(ByVal daysToAdd As Long) As Period
			If daysToAdd = 0 Then Return Me
			Return create(years, months, System.Math.toIntExact (System.Math.addExact(days, daysToAdd)))
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Returns a copy of this period with the specified period subtracted.
		''' <p>
		''' This operates separately on the years, months and days.
		''' No normalization is performed.
		''' <p>
		''' For example, "1 year, 6 months and 3 days" minus "2 years, 2 months and 2 days"
		''' returns "-1 years, 4 months and 1 day".
		''' <p>
		''' The specified amount is typically an instance of {@code Period}.
		''' Other types are interpreted using <seealso cref="Period#from(TemporalAmount)"/>.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="amountToSubtract">  the amount to subtract, not null </param>
		''' <returns> a {@code Period} based on this period with the requested period subtracted, not null </returns>
		''' <exception cref="DateTimeException"> if the specified amount has a non-ISO chronology or
		'''  contains an invalid unit </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Function minus(ByVal amountToSubtract As java.time.temporal.TemporalAmount) As Period
			Dim isoAmount As Period = Period.from(amountToSubtract)
			Return create (System.Math.subtractExact(years, isoAmount.years), System.Math.subtractExact(months, isoAmount.months), System.Math.subtractExact(days, isoAmount.days))
		End Function

		''' <summary>
		''' Returns a copy of this period with the specified years subtracted.
		''' <p>
		''' This subtracts the amount from the years unit in a copy of this period.
		''' The months and days units are unaffected.
		''' For example, "1 year, 6 months and 3 days" minus 2 years returns "-1 years, 6 months and 3 days".
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="yearsToSubtract">  the years to subtract, positive or negative </param>
		''' <returns> a {@code Period} based on this period with the specified years subtracted, not null </returns>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Function minusYears(ByVal yearsToSubtract As Long) As Period
			Return (If(yearsToSubtract = java.lang.[Long].MIN_VALUE, plusYears(Long.Max_Value).plusYears(1), plusYears(-yearsToSubtract)))
		End Function

		''' <summary>
		''' Returns a copy of this period with the specified months subtracted.
		''' <p>
		''' This subtracts the amount from the months unit in a copy of this period.
		''' The years and days units are unaffected.
		''' For example, "1 year, 6 months and 3 days" minus 2 months returns "1 year, 4 months and 3 days".
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="monthsToSubtract">  the years to subtract, positive or negative </param>
		''' <returns> a {@code Period} based on this period with the specified months subtracted, not null </returns>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Function minusMonths(ByVal monthsToSubtract As Long) As Period
			Return (If(monthsToSubtract = java.lang.[Long].MIN_VALUE, plusMonths(Long.Max_Value).plusMonths(1), plusMonths(-monthsToSubtract)))
		End Function

		''' <summary>
		''' Returns a copy of this period with the specified days subtracted.
		''' <p>
		''' This subtracts the amount from the days unit in a copy of this period.
		''' The years and months units are unaffected.
		''' For example, "1 year, 6 months and 3 days" minus 2 days returns "1 year, 6 months and 1 day".
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="daysToSubtract">  the months to subtract, positive or negative </param>
		''' <returns> a {@code Period} based on this period with the specified days subtracted, not null </returns>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Function minusDays(ByVal daysToSubtract As Long) As Period
			Return (If(daysToSubtract = java.lang.[Long].MIN_VALUE, plusDays(Long.Max_Value).plusDays(1), plusDays(-daysToSubtract)))
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Returns a new instance with each element in this period multiplied
		''' by the specified scalar.
		''' <p>
		''' This returns a period with each of the years, months and days units
		''' individually multiplied.
		''' For example, a period of "2 years, -3 months and 4 days" multiplied by
		''' 3 will return "6 years, -9 months and 12 days".
		''' No normalization is performed.
		''' </summary>
		''' <param name="scalar">  the scalar to multiply by, not null </param>
		''' <returns> a {@code Period} based on this period with the amounts multiplied by the scalar, not null </returns>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Function multipliedBy(ByVal scalar As Integer) As Period
			If Me Is ZERO OrElse scalar = 1 Then Return Me
			Return create (System.Math.multiplyExact(years, scalar), System.Math.multiplyExact(months, scalar), System.Math.multiplyExact(days, scalar))
		End Function

		''' <summary>
		''' Returns a new instance with each amount in this period negated.
		''' <p>
		''' This returns a period with each of the years, months and days units
		''' individually negated.
		''' For example, a period of "2 years, -3 months and 4 days" will be
		''' negated to "-2 years, 3 months and -4 days".
		''' No normalization is performed.
		''' </summary>
		''' <returns> a {@code Period} based on this period with the amounts negated, not null </returns>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs, which only happens if
		'''  one of the units has the value {@code java.lang.[Long].MIN_VALUE} </exception>
		Public Function negated() As Period
			Return multipliedBy(-1)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Returns a copy of this period with the years and months normalized.
		''' <p>
		''' This normalizes the years and months units, leaving the days unit unchanged.
		''' The months unit is adjusted to have an absolute value less than 11,
		''' with the years unit being adjusted to compensate. For example, a period of
		''' "1 Year and 15 months" will be normalized to "2 years and 3 months".
		''' <p>
		''' The sign of the years and months units will be the same after normalization.
		''' For example, a period of "1 year and -25 months" will be normalized to
		''' "-1 year and -1 month".
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <returns> a {@code Period} based on this period with excess months normalized to years, not null </returns>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Function normalized() As Period
			Dim totalMonths As Long = toTotalMonths()
			Dim splitYears As Long = totalMonths \ 12
			Dim splitMonths As Integer = CInt(Fix(totalMonths Mod 12)) ' no overflow
			If splitYears = years AndAlso splitMonths = months Then Return Me
			Return create (System.Math.toIntExact(splitYears), splitMonths, days)
		End Function

		''' <summary>
		''' Gets the total number of months in this period.
		''' <p>
		''' This returns the total number of months in the period by multiplying the
		''' number of years by 12 and adding the number of months.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <returns> the total number of months in the period, may be negative </returns>
		Public Function toTotalMonths() As Long
			Return years * 12L + months ' no overflow
		End Function

		'-------------------------------------------------------------------------
		''' <summary>
		''' Adds this period to the specified temporal object.
		''' <p>
		''' This returns a temporal object of the same observable type as the input
		''' with this period added.
		''' If the temporal has a chronology, it must be the ISO chronology.
		''' <p>
		''' In most cases, it is clearer to reverse the calling pattern by using
		''' <seealso cref="Temporal#plus(TemporalAmount)"/>.
		''' <pre>
		'''   // these two lines are equivalent, but the second approach is recommended
		'''   dateTime = thisPeriod.addTo(dateTime);
		'''   dateTime = dateTime.plus(thisPeriod);
		''' </pre>
		''' <p>
		''' The calculation operates as follows.
		''' First, the chronology of the temporal is checked to ensure it is ISO chronology or null.
		''' Second, if the months are zero, the years are added if non-zero, otherwise
		''' the combination of years and months is added if non-zero.
		''' Finally, any days are added.
		''' <p>
		''' This approach ensures that a partial period can be added to a partial date.
		''' For example, a period of years and/or months can be added to a {@code YearMonth},
		''' but a period including days cannot.
		''' The approach also adds years and months together when necessary, which ensures
		''' correct behaviour at the end of the month.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="temporal">  the temporal object to adjust, not null </param>
		''' <returns> an object of the same type with the adjustment made, not null </returns>
		''' <exception cref="DateTimeException"> if unable to add </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Overrides Function addTo(ByVal temporal As java.time.temporal.Temporal) As java.time.temporal.Temporal
			validateChrono(temporal)
			If months = 0 Then
				If years <> 0 Then temporal = temporal.plus(years, YEARS)
			Else
				Dim totalMonths As Long = toTotalMonths()
				If totalMonths <> 0 Then temporal = temporal.plus(totalMonths, MONTHS)
			End If
			If days <> 0 Then temporal = temporal.plus(days, DAYS)
			Return temporal
		End Function

		''' <summary>
		''' Subtracts this period from the specified temporal object.
		''' <p>
		''' This returns a temporal object of the same observable type as the input
		''' with this period subtracted.
		''' If the temporal has a chronology, it must be the ISO chronology.
		''' <p>
		''' In most cases, it is clearer to reverse the calling pattern by using
		''' <seealso cref="Temporal#minus(TemporalAmount)"/>.
		''' <pre>
		'''   // these two lines are equivalent, but the second approach is recommended
		'''   dateTime = thisPeriod.subtractFrom(dateTime);
		'''   dateTime = dateTime.minus(thisPeriod);
		''' </pre>
		''' <p>
		''' The calculation operates as follows.
		''' First, the chronology of the temporal is checked to ensure it is ISO chronology or null.
		''' Second, if the months are zero, the years are subtracted if non-zero, otherwise
		''' the combination of years and months is subtracted if non-zero.
		''' Finally, any days are subtracted.
		''' <p>
		''' This approach ensures that a partial period can be subtracted from a partial date.
		''' For example, a period of years and/or months can be subtracted from a {@code YearMonth},
		''' but a period including days cannot.
		''' The approach also subtracts years and months together when necessary, which ensures
		''' correct behaviour at the end of the month.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="temporal">  the temporal object to adjust, not null </param>
		''' <returns> an object of the same type with the adjustment made, not null </returns>
		''' <exception cref="DateTimeException"> if unable to subtract </exception>
		''' <exception cref="ArithmeticException"> if numeric overflow occurs </exception>
		Public Overrides Function subtractFrom(ByVal temporal As java.time.temporal.Temporal) As java.time.temporal.Temporal
			validateChrono(temporal)
			If months = 0 Then
				If years <> 0 Then temporal = temporal.minus(years, YEARS)
			Else
				Dim totalMonths As Long = toTotalMonths()
				If totalMonths <> 0 Then temporal = temporal.minus(totalMonths, MONTHS)
			End If
			If days <> 0 Then temporal = temporal.minus(days, DAYS)
			Return temporal
		End Function

		''' <summary>
		''' Validates that the temporal has the correct chronology.
		''' </summary>
		Private Sub validateChrono(ByVal temporal As java.time.temporal.TemporalAccessor)
			java.util.Objects.requireNonNull(temporal, "temporal")
			Dim temporalChrono As java.time.chrono.Chronology = temporal.query(java.time.temporal.TemporalQueries.chronology())
			If temporalChrono IsNot Nothing AndAlso java.time.chrono.IsoChronology.INSTANCE.Equals(temporalChrono) = False Then Throw New DateTimeException("Chronology mismatch, expected: ISO, actual: " & temporalChrono.id)
		End Sub

		'-----------------------------------------------------------------------
		''' <summary>
		''' Checks if this period is equal to another period.
		''' <p>
		''' The comparison is based on the type {@code Period} and each of the three amounts.
		''' To be equal, the years, months and days units must be individually equal.
		''' Note that this means that a period of "15 Months" is not equal to a period
		''' of "1 Year and 3 Months".
		''' </summary>
		''' <param name="obj">  the object to check, null returns false </param>
		''' <returns> true if this is equal to the other period </returns>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			If Me Is obj Then Return True
			If TypeOf obj Is Period Then
				Dim other As Period = CType(obj, Period)
				Return years = other.years AndAlso months = other.months AndAlso days = other.days
			End If
			Return False
		End Function

		''' <summary>
		''' A hash code for this period.
		''' </summary>
		''' <returns> a suitable hash code </returns>
		Public Overrides Function GetHashCode() As Integer
			Return years +  java.lang.[Integer].rotateLeft(months, 8) +  java.lang.[Integer].rotateLeft(days, 16)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Outputs this period as a {@code String}, such as {@code P6Y3M1D}.
		''' <p>
		''' The output will be in the ISO-8601 period format.
		''' A zero period will be represented as zero days, 'P0D'.
		''' </summary>
		''' <returns> a string representation of this period, not null </returns>
		Public Overrides Function ToString() As String
			If Me Is ZERO Then
				Return "P0D"
			Else
				Dim buf As New StringBuilder
				buf.append("P"c)
				If years <> 0 Then buf.append(years).append("Y"c)
				If months <> 0 Then buf.append(months).append("M"c)
				If days <> 0 Then buf.append(days).append("D"c)
				Return buf.ToString()
			End If
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Writes the object using a
		''' <a href="../../serialized-form.html#java.time.Ser">dedicated serialized form</a>.
		''' @serialData
		''' <pre>
		'''  out.writeByte(14);  // identifies a Period
		'''  out.writeInt(years);
		'''  out.writeInt(months);
		'''  out.writeInt(days);
		''' </pre>
		''' </summary>
		''' <returns> the instance of {@code Ser}, not null </returns>
		Private Function writeReplace() As Object
			Return New Ser(Ser.PERIOD_TYPE, Me)
		End Function

		''' <summary>
		''' Defend against malicious streams.
		''' </summary>
		''' <param name="s"> the stream to read </param>
		''' <exception cref="java.io.InvalidObjectException"> always </exception>
		Private Sub readObject(ByVal s As java.io.ObjectInputStream)
			Throw New java.io.InvalidObjectException("Deserialization via serialization delegate")
		End Sub

		Friend Sub writeExternal(ByVal out As java.io.DataOutput)
			out.writeInt(years)
			out.writeInt(months)
			out.writeInt(days)
		End Sub

		Shared Function readExternal(ByVal [in] As java.io.DataInput) As Period
			Dim years_Renamed As Integer = [in].readInt()
			Dim months_Renamed As Integer = [in].readInt()
			Dim days_Renamed As Integer = [in].readInt()
			Return Period.of(years_Renamed, months_Renamed, days_Renamed)
		End Function

	End Class

End Namespace
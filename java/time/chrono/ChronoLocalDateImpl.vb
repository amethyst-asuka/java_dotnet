Imports Microsoft.VisualBasic
Imports System

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
' * Copyright (c) 2012, Stephen Colebourne & Michael Nascimento Santos
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
	''' A date expressed in terms of a standard year-month-day calendar system.
	''' <p>
	''' This class is used by applications seeking to handle dates in non-ISO calendar systems.
	''' For example, the Japanese, Minguo, Thai Buddhist and others.
	''' <p>
	''' {@code ChronoLocalDate} is built on the generic concepts of year, month and day.
	''' The calendar system, represented by a <seealso cref="java.time.chrono.Chronology"/>, expresses the relationship between
	''' the fields and this class allows the resulting date to be manipulated.
	''' <p>
	''' Note that not all calendar systems are suitable for use with this class.
	''' For example, the Mayan calendar uses a system that bears no relation to years, months and days.
	''' <p>
	''' The API design encourages the use of {@code LocalDate} for the majority of the application.
	''' This includes code to read and write from a persistent data store, such as a database,
	''' and to send dates and times across a network. The {@code ChronoLocalDate} instance is then used
	''' at the user interface level to deal with localized input/output.
	''' 
	''' <P>Example: </p>
	''' <pre>
	'''        System.out.printf("Example()%n");
	'''        // Enumerate the list of available calendars and print today for each
	'''        Set&lt;Chronology&gt; chronos = Chronology.getAvailableChronologies();
	'''        for (Chronology chrono : chronos) {
	'''            ChronoLocalDate date = chrono.dateNow();
	'''            System.out.printf("   %20s: %s%n", chrono.getID(), date.toString());
	'''        }
	''' 
	'''        // Print the Hijrah date and calendar
	'''        ChronoLocalDate date = Chronology.of("Hijrah").dateNow();
	'''        int day = date.get(ChronoField.DAY_OF_MONTH);
	'''        int dow = date.get(ChronoField.DAY_OF_WEEK);
	'''        int month = date.get(ChronoField.MONTH_OF_YEAR);
	'''        int year = date.get(ChronoField.YEAR);
	'''        System.out.printf("  Today is %s %s %d-%s-%d%n", date.getChronology().getID(),
	'''                dow, day, month, year);
	''' 
	'''        // Print today's date and the last day of the year
	'''        ChronoLocalDate now1 = Chronology.of("Hijrah").dateNow();
	'''        ChronoLocalDate first = now1.with(ChronoField.DAY_OF_MONTH, 1)
	'''                .with(ChronoField.MONTH_OF_YEAR, 1);
	'''        ChronoLocalDate last = first.plus(1, ChronoUnit.YEARS)
	'''                .minus(1, ChronoUnit.DAYS);
	'''        System.out.printf("  Today is %s: start: %s; end: %s%n", last.getChronology().getID(),
	'''                first, last);
	''' </pre>
	''' 
	''' <h3>Adding Calendars</h3>
	''' <p> The set of calendars is extensible by defining a subclass of <seealso cref="ChronoLocalDate"/>
	''' to represent a date instance and an implementation of {@code Chronology}
	''' to be the factory for the ChronoLocalDate subclass.
	''' </p>
	''' <p> To permit the discovery of the additional calendar types the implementation of
	''' {@code Chronology} must be registered as a Service implementing the {@code Chronology} interface
	''' in the {@code META-INF/Services} file as per the specification of <seealso cref="java.util.ServiceLoader"/>.
	''' The subclass must function according to the {@code Chronology} class description and must provide its
	''' <seealso cref="java.time.chrono.Chronology#getId() chronlogy ID"/> and <seealso cref="Chronology#getCalendarType() calendar type"/>. </p>
	''' 
	''' @implSpec
	''' This abstract class must be implemented with care to ensure other classes operate correctly.
	''' All implementations that can be instantiated must be final, immutable and thread-safe.
	''' Subclasses should be Serializable wherever possible.
	''' </summary>
	''' @param <D> the ChronoLocalDate of this date-time
	''' @since 1.8 </param>
	<Serializable> _
	Friend MustInherit Class ChronoLocalDateImpl(Of D As ChronoLocalDate)
		Implements ChronoLocalDate, java.time.temporal.Temporal, java.time.temporal.TemporalAdjuster

			Public MustOverride Function adjustInto(ByVal temporal As java.time.temporal.Temporal) As java.time.temporal.Temporal Implements ChronoLocalDate.adjustInto
			Public MustOverride Function [until](ByVal endExclusive As java.time.temporal.Temporal, ByVal unit As java.time.temporal.TemporalUnit) As Long Implements ChronoLocalDate.until
			Public Function [Return](amountToSubtract = java.lang.[Long].MIN_VALUE ? plus(Long.MAX_VALUE, unit).plus(1, unit) : plus(-amountToSubtract, unit) ByVal As ) As [MustOverride]
			Public MustOverride Function minus(ByVal amountToSubtract As Long, ByVal unit As java.time.temporal.TemporalUnit) As default
			Public MustOverride Function minus(ByVal amount As java.time.temporal.TemporalAmount) As default
			Public MustOverride Function plus(ByVal amountToAdd As Long, ByVal unit As java.time.temporal.TemporalUnit) As java.time.temporal.Temporal
			Public MustOverride Function plus(ByVal amount As java.time.temporal.TemporalAmount) As default
			Public MustOverride Function [with](ByVal field As java.time.temporal.TemporalField, ByVal newValue As Long) As java.time.temporal.Temporal
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			Public MustOverride Return adjustInto(Me);
			Public MustOverride Function [with](ByVal adjuster As java.time.temporal.TemporalAdjuster) As default
			Public MustOverride Function isSupported(ByVal unit As java.time.temporal.TemporalUnit) As Boolean Implements ChronoLocalDate.isSupported
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			Public MustOverride Return subtractFrom(Me);
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			Public MustOverride Return addTo(Me);
			Public MustOverride Function isEqual(ByVal other As ChronoLocalDate) As default
			Public MustOverride Function isBefore(ByVal other As ChronoLocalDate) As default
			Public MustOverride Function isAfter(ByVal other As ChronoLocalDate) As default
			Public Function [if](cmp = ByVal 0 As ) As [MustOverride]
			Public MustOverride Function compareTo(ByVal other As ChronoLocalDate) As default
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			Public MustOverride Return getLong(EPOCH_DAY);
			Public MustOverride Function toEpochDay() As default
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			Public MustOverride Return of(Me, localTime);
			Public MustOverride Function atTime(ByVal localTime As java.time.LocalTime) As default
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			Public MustOverride Return format(Me);
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			Public MustOverride  requireNonNull(formatter, "formatter");
			Public MustOverride Function format(ByVal formatter As java.time.format.DateTimeFormatter) As default
			Public MustOverride Function [until](ByVal endDateExclusive As ChronoLocalDate) As ChronoPeriod Implements ChronoLocalDate.until
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			Public MustOverride Return with(EPOCH_DAY, toEpochDay());
			Public MustOverride Function adjustInto(ByVal temporal As java.time.temporal.Temporal) As default
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			Public MustOverride Return queryFrom(Me);
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			Public MustOverride  Return(R);
			Public Function [Return]() As [MustOverride]
			Public Function [if](query = java.time.temporal.TemporalQueries.zoneId() OrElse query = java.time.temporal.TemporalQueries.zone() OrElse query = java.time.temporal.TemporalQueries.offset() ByVal As ) As [MustOverride]
			Public MustOverride Function query(ByVal query As java.time.temporal.TemporalQuery(Of R)) As default(Of R)
			Public MustOverride Function ensureValid(getChronology() ByVal As , outerInstance.minus(amountToSubtract, unit) ByVal As ) As [Return]
			Public MustOverride Function ensureValid(getChronology() ByVal As , outerInstance.minus(amount) ByVal As ) As [Return]
			Public MustOverride Function ensureValid(getChronology() ByVal As , unit.addTo(Me, amountToAdd) ByVal As ) As [Return]
			Public MustOverride Function java.time.temporal.UnsupportedTemporalTypeException("Unsupported unit: " & ByVal unit As ) As throw
			Public MustOverride Function ensureValid(getChronology() ByVal As , outerInstance.plus(amount) ByVal As ) As [Return]
			Public MustOverride Function ensureValid(getChronology() ByVal As , field.adjustInto(Me, newValue) ByVal As ) As [Return]
			Public MustOverride Function java.time.temporal.UnsupportedTemporalTypeException("Unsupported field: " & ByVal field As ) As throw
			Public MustOverride Function [with](ByVal field As java.time.temporal.TemporalField, ByVal newValue As Long) As default
			Public MustOverride Function ensureValid(getChronology() ByVal As , outerInstance.with(adjuster) ByVal As ) As [Return]
			Public Function [if](unit ByVal java.time.temporal.ChronoUnit As instanceof) As [MustOverride]
			Public MustOverride Function isSupported(ByVal unit As java.time.temporal.TemporalUnit) As default
			Public MustOverride ReadOnly Property dateBased As [Return]
			Public Function [if](field ByVal java.time.temporal.ChronoField As instanceof) As [MustOverride]
			Public MustOverride Function isSupported(ByVal field As java.time.temporal.TemporalField) As default
			Public Function [Return](isLeapYear() ? 366 : ByVal 365 As ) As [MustOverride]
			Public MustOverride Function lengthOfYear() As default
			Public MustOverride Function lengthOfMonth() As Integer Implements ChronoLocalDate.lengthOfMonth
			Public MustOverride Function getChronology(getLong(YEAR) ByVal As ) As [Return]
			Public MustOverride ReadOnly Property leapYear As default
			Public MustOverride Function getChronology(get(ERA) ByVal As ) As [Return]
			Public MustOverride ReadOnly Property era As default
			Public MustOverride ReadOnly Property chronology As Chronology Implements ChronoLocalDate.getChronology
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			Public MustOverride Return date(temporal);
			Public MustOverride Function java.time.DateTimeException("Unable to obtain ChronoLocalDate from TemporalAccessor: " & temporal.GetType() ByVal As ) As throw
			Public Function [if](chrono = ByVal [Nothing] As ) As [MustOverride]
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			Public MustOverride  requireNonNull(temporal, "temporal");
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
			Public MustOverride  Return(ChronoLocalDate);
			Public Function [if](temporal ByVal ChronoLocalDate As instanceof) As [MustOverride]
			Public MustOverride Function [from](ByVal temporal As java.time.temporal.TemporalAccessor) As ChronoLocalDate Implements ChronoLocalDate.from
			Public MustOverride Function timeLineOrder() As IComparer(Of ChronoLocalDate) Implements ChronoLocalDate.timeLineOrder

		''' <summary>
		''' Serialization version.
		''' </summary>
		Private Const serialVersionUID As Long = 6282433883239719096L

		''' <summary>
		''' Casts the {@code Temporal} to {@code ChronoLocalDate} ensuring it bas the specified chronology.
		''' </summary>
		''' <param name="chrono">  the chronology to check for, not null </param>
		''' <param name="temporal">  a date-time to cast, not null </param>
		''' <returns> the date-time checked and cast to {@code ChronoLocalDate}, not null </returns>
		''' <exception cref="ClassCastException"> if the date-time cannot be cast to ChronoLocalDate
		'''  or the chronology is not equal this Chronology </exception>
		Friend Shared Function ensureValid(Of D As ChronoLocalDate)(ByVal chrono As Chronology, ByVal temporal As java.time.temporal.Temporal) As D
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Dim other As D = CType(temporal, D)
			If chrono.Equals(other.chronology) = False Then Throw New ClassCastException("Chronology mismatch, expected: " & chrono.id & ", actual: " & other.chronology.id)
			Return other
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Creates an instance.
		''' </summary>
		Friend Sub New()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overrides Function [with](ByVal adjuster As java.time.temporal.TemporalAdjuster) As D
			Return CType(ChronoLocalDate.this.with(adjuster), D)
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overrides Function [with](ByVal field As java.time.temporal.TemporalField, ByVal value As Long) As D
			Return CType(ChronoLocalDate.this.with(field, value), D)
		End Function

		'-----------------------------------------------------------------------
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overrides Function plus(ByVal amount As java.time.temporal.TemporalAmount) As D
			Return CType(ChronoLocalDate.this.plus(amount), D)
		End Function

		'-----------------------------------------------------------------------
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overrides Function plus(ByVal amountToAdd As Long, ByVal unit As java.time.temporal.TemporalUnit) As D
			If TypeOf unit Is java.time.temporal.ChronoUnit Then
				Dim f As java.time.temporal.ChronoUnit = CType(unit, java.time.temporal.ChronoUnit)
				Select Case f
					Case DAYS
						Return plusDays(amountToAdd)
					Case WEEKS
						Return plusDays (System.Math.multiplyExact(amountToAdd, 7))
					Case MONTHS
						Return plusMonths(amountToAdd)
					Case YEARS
						Return plusYears(amountToAdd)
					Case DECADES
						Return plusYears (System.Math.multiplyExact(amountToAdd, 10))
					Case CENTURIES
						Return plusYears (System.Math.multiplyExact(amountToAdd, 100))
					Case MILLENNIA
						Return plusYears (System.Math.multiplyExact(amountToAdd, 1000))
					Case ERAS
						Return [with](ERA, System.Math.addExact(getLong(ERA), amountToAdd))
				End Select
				Throw New java.time.temporal.UnsupportedTemporalTypeException("Unsupported unit: " & unit)
			End If
			Return CType(ChronoLocalDate.this.plus(amountToAdd, unit), D)
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overrides Function minus(ByVal amount As java.time.temporal.TemporalAmount) As D
			Return CType(ChronoLocalDate.this.minus(amount), D)
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overrides Function minus(ByVal amountToSubtract As Long, ByVal unit As java.time.temporal.TemporalUnit) As D
			Return CType(ChronoLocalDate.this.minus(amountToSubtract, unit), D)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Returns a copy of this date with the specified number of years added.
		''' <p>
		''' This adds the specified period in years to the date.
		''' In some cases, adding years can cause the resulting date to become invalid.
		''' If this occurs, then other fields, typically the day-of-month, will be adjusted to ensure
		''' that the result is valid. Typically this will select the last valid day of the month.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="yearsToAdd">  the years to add, may be negative </param>
		''' <returns> a date based on this one with the years added, not null </returns>
		''' <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		Friend MustOverride Function plusYears(ByVal yearsToAdd As Long) As D

		''' <summary>
		''' Returns a copy of this date with the specified number of months added.
		''' <p>
		''' This adds the specified period in months to the date.
		''' In some cases, adding months can cause the resulting date to become invalid.
		''' If this occurs, then other fields, typically the day-of-month, will be adjusted to ensure
		''' that the result is valid. Typically this will select the last valid day of the month.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="monthsToAdd">  the months to add, may be negative </param>
		''' <returns> a date based on this one with the months added, not null </returns>
		''' <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		Friend MustOverride Function plusMonths(ByVal monthsToAdd As Long) As D

		''' <summary>
		''' Returns a copy of this date with the specified number of weeks added.
		''' <p>
		''' This adds the specified period in weeks to the date.
		''' In some cases, adding weeks can cause the resulting date to become invalid.
		''' If this occurs, then other fields will be adjusted to ensure that the result is valid.
		''' <p>
		''' The default implementation uses <seealso cref="#plusDays(long)"/> using a 7 day week.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="weeksToAdd">  the weeks to add, may be negative </param>
		''' <returns> a date based on this one with the weeks added, not null </returns>
		''' <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		Friend Overridable Function plusWeeks(ByVal weeksToAdd As Long) As D
			Return plusDays (System.Math.multiplyExact(weeksToAdd, 7))
		End Function

		''' <summary>
		''' Returns a copy of this date with the specified number of days added.
		''' <p>
		''' This adds the specified period in days to the date.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="daysToAdd">  the days to add, may be negative </param>
		''' <returns> a date based on this one with the days added, not null </returns>
		''' <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
		Friend MustOverride Function plusDays(ByVal daysToAdd As Long) As D

		'-----------------------------------------------------------------------
		''' <summary>
		''' Returns a copy of this date with the specified number of years subtracted.
		''' <p>
		''' This subtracts the specified period in years to the date.
		''' In some cases, subtracting years can cause the resulting date to become invalid.
		''' If this occurs, then other fields, typically the day-of-month, will be adjusted to ensure
		''' that the result is valid. Typically this will select the last valid day of the month.
		''' <p>
		''' The default implementation uses <seealso cref="#plusYears(long)"/>.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="yearsToSubtract">  the years to subtract, may be negative </param>
		''' <returns> a date based on this one with the years subtracted, not null </returns>
		''' <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend Overridable Function minusYears(ByVal yearsToSubtract As Long) As D
			[Return] (If(yearsToSubtract = java.lang.[Long].MIN_VALUE, CType(plusYears(Long.Max_Value), ChronoLocalDateImpl(Of D)).plusYears(1), plusYears(-yearsToSubtract)))
		End Function

		''' <summary>
		''' Returns a copy of this date with the specified number of months subtracted.
		''' <p>
		''' This subtracts the specified period in months to the date.
		''' In some cases, subtracting months can cause the resulting date to become invalid.
		''' If this occurs, then other fields, typically the day-of-month, will be adjusted to ensure
		''' that the result is valid. Typically this will select the last valid day of the month.
		''' <p>
		''' The default implementation uses <seealso cref="#plusMonths(long)"/>.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="monthsToSubtract">  the months to subtract, may be negative </param>
		''' <returns> a date based on this one with the months subtracted, not null </returns>
		''' <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend Overridable Function minusMonths(ByVal monthsToSubtract As Long) As D
			[Return] (If(monthsToSubtract = java.lang.[Long].MIN_VALUE, CType(plusMonths(Long.Max_Value), ChronoLocalDateImpl(Of D)).plusMonths(1), plusMonths(-monthsToSubtract)))
		End Function

		''' <summary>
		''' Returns a copy of this date with the specified number of weeks subtracted.
		''' <p>
		''' This subtracts the specified period in weeks to the date.
		''' In some cases, subtracting weeks can cause the resulting date to become invalid.
		''' If this occurs, then other fields will be adjusted to ensure that the result is valid.
		''' <p>
		''' The default implementation uses <seealso cref="#plusWeeks(long)"/>.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="weeksToSubtract">  the weeks to subtract, may be negative </param>
		''' <returns> a date based on this one with the weeks subtracted, not null </returns>
		''' <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend Overridable Function minusWeeks(ByVal weeksToSubtract As Long) As D
			[Return] (If(weeksToSubtract = java.lang.[Long].MIN_VALUE, CType(plusWeeks(Long.Max_Value), ChronoLocalDateImpl(Of D)).plusWeeks(1), plusWeeks(-weeksToSubtract)))
		End Function

		''' <summary>
		''' Returns a copy of this date with the specified number of days subtracted.
		''' <p>
		''' This subtracts the specified period in days to the date.
		''' <p>
		''' The default implementation uses <seealso cref="#plusDays(long)"/>.
		''' <p>
		''' This instance is immutable and unaffected by this method call.
		''' </summary>
		''' <param name="daysToSubtract">  the days to subtract, may be negative </param>
		''' <returns> a date based on this one with the days subtracted, not null </returns>
		''' <exception cref="DateTimeException"> if the result exceeds the supported date range </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend Overridable Function minusDays(ByVal daysToSubtract As Long) As D
			[Return] (If(daysToSubtract = java.lang.[Long].MIN_VALUE, CType(plusDays(Long.Max_Value), ChronoLocalDateImpl(Of D)).plusDays(1), plusDays(-daysToSubtract)))
		End Function

		'-----------------------------------------------------------------------
		Public Overrides Function [until](ByVal endExclusive As java.time.temporal.Temporal, ByVal unit As java.time.temporal.TemporalUnit) As Long Implements ChronoLocalDate.until
			java.util.Objects.requireNonNull(endExclusive, "endExclusive")
			Dim [end] As ChronoLocalDate = chronology.date(endExclusive)
			If TypeOf unit Is java.time.temporal.ChronoUnit Then
				Select Case CType(unit, java.time.temporal.ChronoUnit)
					Case DAYS
						Return daysUntil([end])
					Case WEEKS
						Return daysUntil([end]) \ 7
					Case MONTHS
						Return monthsUntil([end])
					Case YEARS
						Return monthsUntil([end]) \ 12
					Case DECADES
						Return monthsUntil([end]) \ 120
					Case CENTURIES
						Return monthsUntil([end]) \ 1200
					Case MILLENNIA
						Return monthsUntil([end]) \ 12000
					Case ERAS
						Return [end].getLong(ERA) - getLong(ERA)
				End Select
				Throw New java.time.temporal.UnsupportedTemporalTypeException("Unsupported unit: " & unit)
			End If
			java.util.Objects.requireNonNull(unit, "unit")
			Return unit.between(Me, [end])
		End Function

		Private Function daysUntil(ByVal [end] As ChronoLocalDate) As Long
			Return [end].toEpochDay() - toEpochDay() ' no overflow
		End Function

		Private Function monthsUntil(ByVal [end] As ChronoLocalDate) As Long
			Dim range As java.time.temporal.ValueRange = chronology.range(MONTH_OF_YEAR)
			If range.maximum <> 12 Then Throw New IllegalStateException("ChronoLocalDateImpl only supports Chronologies with 12 months per year")
			Dim packed1 As Long = getLong(PROLEPTIC_MONTH) * 32L + get(DAY_OF_MONTH) ' no overflow
			Dim packed2 As Long = [end].getLong(PROLEPTIC_MONTH) * 32L + [end].get(DAY_OF_MONTH) ' no overflow
			[Return] (packed2 - packed1) / 32
		End Function

		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			If Me Is obj Then Return True
			If TypeOf obj Is ChronoLocalDate Then Return compareTo(CType(obj, ChronoLocalDate)) = 0
			Return False
		End Function

		Public Overrides Function GetHashCode() As Integer
			Dim epDay As Long = toEpochDay()
			Return chronology.GetHashCode() Xor (CInt(Fix(epDay Xor (CLng(CULng(epDay) >> 32)))))
		End Function

		Public Overrides Function ToString() As String
			' getLong() reduces chances of exceptions in toString()
			Dim yoe As Long = getLong(YEAR_OF_ERA)
			Dim moy As Long = getLong(MONTH_OF_YEAR)
			Dim dom As Long = getLong(DAY_OF_MONTH)
			Dim buf As New StringBuilder(30)
			buf.append(chronology.ToString()).append(" ").append(era).append(" ").append(yoe).append(If(moy < 10, "-0", "-")).append(moy).append(If(dom < 10, "-0", "-")).append(dom)
			Return buf.ToString()
		End Function

	End Class

End Namespace
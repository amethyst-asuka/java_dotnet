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
Namespace java.time.temporal


	''' <summary>
	''' A standard set of date periods units.
	''' <p>
	''' This set of units provide unit-based access to manipulate a date, time or date-time.
	''' The standard set of units can be extended by implementing <seealso cref="TemporalUnit"/>.
	''' <p>
	''' These units are intended to be applicable in multiple calendar systems.
	''' For example, most non-ISO calendar systems define units of years, months and days,
	''' just with slightly different rules.
	''' The documentation of each unit explains how it operates.
	''' 
	''' @implSpec
	''' This is a final, immutable and thread-safe enum.
	''' 
	''' @since 1.8
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot implement interfaces in .NET:
	Public Enum ChronoUnit

		''' <summary>
		''' Unit that represents the concept of a nanosecond, the smallest supported unit of time.
		''' For the ISO calendar system, it is equal to the 1,000,000,000th part of the second unit.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Enum values must be single integer values in .NET:
		NANOS("Nanos", java.time.Duration.ofNanos(1)),
		''' <summary>
		''' Unit that represents the concept of a microsecond.
		''' For the ISO calendar system, it is equal to the 1,000,000th part of the second unit.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Enum values must be single integer values in .NET:
		MICROS("Micros", java.time.Duration.ofNanos(1000)),
		''' <summary>
		''' Unit that represents the concept of a millisecond.
		''' For the ISO calendar system, it is equal to the 1000th part of the second unit.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Enum values must be single integer values in .NET:
		MILLIS("Millis", java.time.Duration.ofNanos(1000000)),
		''' <summary>
		''' Unit that represents the concept of a second.
		''' For the ISO calendar system, it is equal to the second in the SI system
		''' of units, except around a leap-second.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Enum values must be single integer values in .NET:
		SECONDS("Seconds", java.time.Duration.ofSeconds(1)),
		''' <summary>
		''' Unit that represents the concept of a minute.
		''' For the ISO calendar system, it is equal to 60 seconds.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Enum values must be single integer values in .NET:
		MINUTES("Minutes", java.time.Duration.ofSeconds(60)),
		''' <summary>
		''' Unit that represents the concept of an hour.
		''' For the ISO calendar system, it is equal to 60 minutes.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Enum values must be single integer values in .NET:
		HOURS("Hours", java.time.Duration.ofSeconds(3600)),
		''' <summary>
		''' Unit that represents the concept of half a day, as used in AM/PM.
		''' For the ISO calendar system, it is equal to 12 hours.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Enum values must be single integer values in .NET:
		HALF_DAYS("HalfDays", java.time.Duration.ofSeconds(43200)),
		''' <summary>
		''' Unit that represents the concept of a day.
		''' For the ISO calendar system, it is the standard day from midnight to midnight.
		''' The estimated duration of a day is {@code 24 Hours}.
		''' <p>
		''' When used with other calendar systems it must correspond to the day defined by
		''' the rising and setting of the Sun on Earth. It is not required that days begin
		''' at midnight - when converting between calendar systems, the date should be
		''' equivalent at midday.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Enum values must be single integer values in .NET:
		DAYS("Days", java.time.Duration.ofSeconds(86400)),
		''' <summary>
		''' Unit that represents the concept of a week.
		''' For the ISO calendar system, it is equal to 7 days.
		''' <p>
		''' When used with other calendar systems it must correspond to an integral number of days.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Enum values must be single integer values in .NET:
		WEEKS("Weeks", java.time.Duration.ofSeconds(7 * 86400L)),
		''' <summary>
		''' Unit that represents the concept of a month.
		''' For the ISO calendar system, the length of the month varies by month-of-year.
		''' The estimated duration of a month is one twelfth of {@code 365.2425 Days}.
		''' <p>
		''' When used with other calendar systems it must correspond to an integral number of days.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Enum values must be single integer values in .NET:
		MONTHS("Months", java.time.Duration.ofSeconds(31556952L / 12)),
		''' <summary>
		''' Unit that represents the concept of a year.
		''' For the ISO calendar system, it is equal to 12 months.
		''' The estimated duration of a year is {@code 365.2425 Days}.
		''' <p>
		''' When used with other calendar systems it must correspond to an integral number of days
		''' or months roughly equal to a year defined by the passage of the Earth around the Sun.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Enum values must be single integer values in .NET:
		YEARS("Years", java.time.Duration.ofSeconds(31556952L)),
		''' <summary>
		''' Unit that represents the concept of a decade.
		''' For the ISO calendar system, it is equal to 10 years.
		''' <p>
		''' When used with other calendar systems it must correspond to an integral number of days
		''' and is normally an integral number of years.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Enum values must be single integer values in .NET:
		DECADES("Decades", java.time.Duration.ofSeconds(31556952L * 10L)),
		''' <summary>
		''' Unit that represents the concept of a century.
		''' For the ISO calendar system, it is equal to 100 years.
		''' <p>
		''' When used with other calendar systems it must correspond to an integral number of days
		''' and is normally an integral number of years.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Enum values must be single integer values in .NET:
		CENTURIES("Centuries", java.time.Duration.ofSeconds(31556952L * 100L)),
		''' <summary>
		''' Unit that represents the concept of a millennium.
		''' For the ISO calendar system, it is equal to 1000 years.
		''' <p>
		''' When used with other calendar systems it must correspond to an integral number of days
		''' and is normally an integral number of years.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Enum values must be single integer values in .NET:
		MILLENNIA("Millennia", java.time.Duration.ofSeconds(31556952L * 1000L)),
		''' <summary>
		''' Unit that represents the concept of an era.
		''' The ISO calendar system doesn't have eras thus it is impossible to add
		''' an era to a date or date-time.
		''' The estimated duration of the era is artificially defined as {@code 1,000,000,000 Years}.
		''' <p>
		''' When used with other calendar systems there are no restrictions on the unit.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Enum values must be single integer values in .NET:
		ERAS("Eras", java.time.Duration.ofSeconds(31556952L * 1000_000_000L)),
		''' <summary>
		''' Artificial unit that represents the concept of forever.
		''' This is primarily used with <seealso cref="TemporalField"/> to represent unbounded fields
		''' such as the year or era.
		''' The estimated duration of the era is artificially defined as the largest duration
		''' supported by {@code Duration}.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Enum values must be single integer values in .NET:
		FOREVER("Forever", java.time.Duration.ofSeconds(Long.MAX_VALUE, 999999999));

'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'		private final String name;
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'		private final java.time.Duration duration;

'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		private ChronoUnit(String name, java.time.Duration estimatedDuration)
	'	{
	'		Me.name = name;
	'		Me.duration = estimatedDuration;
	'	}

		'-----------------------------------------------------------------------
		''' <summary>
		''' Gets the estimated duration of this unit in the ISO calendar system.
		''' <p>
		''' All of the units in this class have an estimated duration.
		''' Days vary due to daylight saving time, while months have different lengths.
		''' </summary>
		''' <returns> the estimated duration of this unit, not null </returns>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		public java.time.Duration getDuration()
	'	{
	'		Return duration;
	'	}

		''' <summary>
		''' Checks if the duration of the unit is an estimate.
		''' <p>
		''' All time units in this class are considered to be accurate, while all date
		''' units in this class are considered to be estimated.
		''' <p>
		''' This definition ignores leap seconds, but considers that Days vary due to
		''' daylight saving time and months have different lengths.
		''' </summary>
		''' <returns> true if the duration is estimated, false if accurate </returns>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		public boolean isDurationEstimated()
	'	{
	'		Return Me.compareTo(DAYS) >= 0;
	'	}

		'-----------------------------------------------------------------------
		''' <summary>
		''' Checks if this unit is a date unit.
		''' <p>
		''' All units from days to eras inclusive are date-based.
		''' Time-based units and {@code FOREVER} return false.
		''' </summary>
		''' <returns> true if a date unit, false if a time unit </returns>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		public boolean isDateBased()
	'	{
	'		Return Me.compareTo(DAYS) >= 0 && Me != FOREVER;
	'	}

		''' <summary>
		''' Checks if this unit is a time unit.
		''' <p>
		''' All units from nanos to half-days inclusive are time-based.
		''' Date-based units and {@code FOREVER} return false.
		''' </summary>
		''' <returns> true if a time unit, false if a date unit </returns>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		public boolean isTimeBased()
	'	{
	'		Return Me.compareTo(DAYS) < 0;
	'	}

		'-----------------------------------------------------------------------
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		public boolean isSupportedBy(Temporal temporal)
	'	{
	'		Return temporal.isSupported(Me);
	'	}

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		public (Of R As Temporal) R addTo(R temporal, long amount)
	'	{
	'		Return (R) temporal.plus(amount, Me);
	'	}

		'-----------------------------------------------------------------------
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		public long between(Temporal temporal1Inclusive, Temporal temporal2Exclusive)
	'	{
	'		Return temporal1Inclusive.until(temporal2Exclusive, Me);
	'	}

		'-----------------------------------------------------------------------
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		public String toString()
	'	{
	'		Return name;
	'	}

	End Enum

End Namespace
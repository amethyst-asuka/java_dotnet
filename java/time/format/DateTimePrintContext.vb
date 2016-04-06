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
Namespace java.time.format



	''' <summary>
	''' Context object used during date and time printing.
	''' <p>
	''' This class provides a single wrapper to items used in the format.
	''' 
	''' @implSpec
	''' This class is a mutable context intended for use from a single thread.
	''' Usage of the class is thread-safe within standard printing as the framework creates
	''' a new instance of the class for each format and printing is single-threaded.
	''' 
	''' @since 1.8
	''' </summary>
	Friend NotInheritable Class DateTimePrintContext

		''' <summary>
		''' The temporal being output.
		''' </summary>
		Private temporal As java.time.temporal.TemporalAccessor
		''' <summary>
		''' The formatter, not null.
		''' </summary>
		Private formatter As DateTimeFormatter
		''' <summary>
		''' Whether the current formatter is optional.
		''' </summary>
		Private [optional] As Integer

		''' <summary>
		''' Creates a new instance of the context.
		''' </summary>
		''' <param name="temporal">  the temporal object being output, not null </param>
		''' <param name="formatter">  the formatter controlling the format, not null </param>
		Friend Sub New(  temporal As java.time.temporal.TemporalAccessor,   formatter As DateTimeFormatter)
			MyBase.New()
			Me.temporal = adjust(temporal, formatter)
			Me.formatter = formatter
		End Sub

		Private Shared Function adjust(  temporal As java.time.temporal.TemporalAccessor,   formatter As DateTimeFormatter) As java.time.temporal.TemporalAccessor
			' normal case first (early return is an optimization)
			Dim overrideChrono As java.time.chrono.Chronology = formatter.chronology
			Dim overrideZone As java.time.ZoneId = formatter.zone
			If overrideChrono Is Nothing AndAlso overrideZone Is Nothing Then Return temporal

			' ensure minimal change (early return is an optimization)
			Dim temporalChrono As java.time.chrono.Chronology = temporal.query(java.time.temporal.TemporalQueries.chronology())
			Dim temporalZone As java.time.ZoneId = temporal.query(java.time.temporal.TemporalQueries.zoneId())
			If java.util.Objects.Equals(overrideChrono, temporalChrono) Then overrideChrono = Nothing
			If java.util.Objects.Equals(overrideZone, temporalZone) Then overrideZone = Nothing
			If overrideChrono Is Nothing AndAlso overrideZone Is Nothing Then Return temporal

			' make adjustment
			Dim effectiveChrono As java.time.chrono.Chronology = (If(overrideChrono IsNot Nothing, overrideChrono, temporalChrono))
			If overrideZone IsNot Nothing Then
				' if have zone and instant, calculation is simple, defaulting chrono if necessary
				If temporal.isSupported(INSTANT_SECONDS) Then
					Dim chrono As java.time.chrono.Chronology = (If(effectiveChrono IsNot Nothing, effectiveChrono, java.time.chrono.IsoChronology.INSTANCE))
					Return chrono.zonedDateTime(java.time.Instant.from(temporal), overrideZone)
				End If
				' block changing zone on OffsetTime, and similar problem cases
				If TypeOf overrideZone.normalized() Is java.time.ZoneOffset AndAlso temporal.isSupported(OFFSET_SECONDS) AndAlso temporal.get(OFFSET_SECONDS) IsNot overrideZone.rules.getOffset(java.time.Instant.EPOCH).totalSeconds Then Throw New java.time.DateTimeException("Unable to apply override zone '" & overrideZone & "' because the temporal object being formatted has a different offset but" & " does not represent an instant: " & temporal)
			End If
			Dim effectiveZone As java.time.ZoneId = (If(overrideZone IsNot Nothing, overrideZone, temporalZone))
			Dim effectiveDate As java.time.chrono.ChronoLocalDate
			If overrideChrono IsNot Nothing Then
				If temporal.isSupported(EPOCH_DAY) Then
					effectiveDate = effectiveChrono.date(temporal)
				Else
					' check for date fields other than epoch-day, ignoring case of converting null to ISO
					If Not(overrideChrono Is java.time.chrono.IsoChronology.INSTANCE AndAlso temporalChrono Is Nothing) Then
						For Each f As java.time.temporal.ChronoField In java.time.temporal.ChronoField.values()
							If f.dateBased AndAlso temporal.isSupported(f) Then Throw New java.time.DateTimeException("Unable to apply override chronology '" & overrideChrono & "' because the temporal object being formatted contains date fields but" & " does not represent a whole date: " & temporal)
						Next f
					End If
					effectiveDate = Nothing
				End If
			Else
				effectiveDate = Nothing
			End If

			' combine available data
			' this is a non-standard temporal that is almost a pure delegate
			' this better handles map-like underlying temporal instances
			Return New TemporalAccessorAnonymousInnerClassHelper
		End Function

		Private Class TemporalAccessorAnonymousInnerClassHelper
			Implements java.time.temporal.TemporalAccessor

			Public Overrides Function isSupported(  field As java.time.temporal.TemporalField) As Boolean
				If effectiveDate IsNot Nothing AndAlso field.dateBased Then Return effectiveDate.isSupported(field)
				Return outerInstance.temporal.isSupported(field)
			End Function
			Public Overrides Function range(  field As java.time.temporal.TemporalField) As java.time.temporal.ValueRange
				If effectiveDate IsNot Nothing AndAlso field.dateBased Then Return effectiveDate.range(field)
				Return outerInstance.temporal.range(field)
			End Function
			Public Overrides Function getLong(  field As java.time.temporal.TemporalField) As Long
				If effectiveDate IsNot Nothing AndAlso field.dateBased Then Return effectiveDate.getLong(field)
				Return outerInstance.temporal.getLong(field)
			End Function
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Public Overrides Function query(Of R)(  query_Renamed As java.time.temporal.TemporalQuery(Of R)) As R
				If query_Renamed Is java.time.temporal.TemporalQueries.chronology() Then Return CType(effectiveChrono, R)
				If query_Renamed Is java.time.temporal.TemporalQueries.zoneId() Then Return CType(effectiveZone, R)
				If query_Renamed Is java.time.temporal.TemporalQueries.precision() Then Return outerInstance.temporal.query(query_Renamed)
				Return query_Renamed.queryFrom(Me)
			End Function
		End Class

		'-----------------------------------------------------------------------
		''' <summary>
		''' Gets the temporal object being output.
		''' </summary>
		''' <returns> the temporal object, not null </returns>
		Friend Property temporal As java.time.temporal.TemporalAccessor
			Get
				Return temporal
			End Get
		End Property

		''' <summary>
		''' Gets the locale.
		''' <p>
		''' This locale is used to control localization in the format output except
		''' where localization is controlled by the DecimalStyle.
		''' </summary>
		''' <returns> the locale, not null </returns>
		Friend Property locale As java.util.Locale
			Get
				Return formatter.locale
			End Get
		End Property

		''' <summary>
		''' Gets the DecimalStyle.
		''' <p>
		''' The DecimalStyle controls the localization of numeric output.
		''' </summary>
		''' <returns> the DecimalStyle, not null </returns>
		Friend Property decimalStyle As DecimalStyle
			Get
				Return formatter.decimalStyle
			End Get
		End Property

		'-----------------------------------------------------------------------
		''' <summary>
		''' Starts the printing of an optional segment of the input.
		''' </summary>
		Friend Sub startOptional()
			Me.optional += 1
		End Sub

		''' <summary>
		''' Ends the printing of an optional segment of the input.
		''' </summary>
		Friend Sub endOptional()
			Me.optional -= 1
		End Sub

		''' <summary>
		''' Gets a value using a query.
		''' </summary>
		''' <param name="query">  the query to use, not null </param>
		''' <returns> the result, null if not found and optional is true </returns>
		''' <exception cref="DateTimeException"> if the type is not available and the section is not optional </exception>
		 Friend Function getValue(Of R)(  query As java.time.temporal.TemporalQuery(Of R)) As R
			Dim result As R = temporal.query(query)
			If result Is Nothing AndAlso [optional] = 0 Then Throw New java.time.DateTimeException("Unable to extract value: " & temporal.GetType())
			Return result
		 End Function

		''' <summary>
		''' Gets the value of the specified field.
		''' <p>
		''' This will return the value for the specified field.
		''' </summary>
		''' <param name="field">  the field to find, not null </param>
		''' <returns> the value, null if not found and optional is true </returns>
		''' <exception cref="DateTimeException"> if the field is not available and the section is not optional </exception>
		Friend Function getValue(  field As java.time.temporal.TemporalField) As Long?
			Try
				Return temporal.getLong(field)
			Catch ex As java.time.DateTimeException
				If [optional] > 0 Then Return Nothing
				Throw ex
			End Try
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Returns a string version of the context for debugging.
		''' </summary>
		''' <returns> a string representation of the context, not null </returns>
		Public Overrides Function ToString() As String
			Return temporal.ToString()
		End Function

	End Class

End Namespace
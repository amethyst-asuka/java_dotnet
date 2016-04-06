Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic

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
' * Copyright (c) 2008-2013, Stephen Colebourne & Michael Nascimento Santos
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
	''' A store of parsed data.
	''' <p>
	''' This class is used during parsing to collect the data. Part of the parsing process
	''' involves handling optional blocks and multiple copies of the data get created to
	''' support the necessary backtracking.
	''' <p>
	''' Once parsing is completed, this class can be used as the resultant {@code TemporalAccessor}.
	''' In most cases, it is only exposed once the fields have been resolved.
	''' 
	''' @implSpec
	''' This class is a mutable context intended for use from a single thread.
	''' Usage of the class is thread-safe within standard parsing as a new instance of this class
	''' is automatically created for each parse and parsing is single-threaded
	''' 
	''' @since 1.8
	''' </summary>
	Friend NotInheritable Class Parsed
		Implements java.time.temporal.TemporalAccessor

		' some fields are accessed using package scope from DateTimeParseContext

		''' <summary>
		''' The parsed fields.
		''' </summary>
		Friend ReadOnly fieldValues As IDictionary(Of java.time.temporal.TemporalField, Long?) = New Dictionary(Of java.time.temporal.TemporalField, Long?)
		''' <summary>
		''' The parsed zone.
		''' </summary>
		Friend zone As java.time.ZoneId
		''' <summary>
		''' The parsed chronology.
		''' </summary>
		Friend chrono As java.time.chrono.Chronology
		''' <summary>
		''' Whether a leap-second is parsed.
		''' </summary>
		Friend leapSecond As Boolean
		''' <summary>
		''' The resolver style to use.
		''' </summary>
		Private resolverStyle As ResolverStyle
		''' <summary>
		''' The resolved date.
		''' </summary>
		Private [date] As java.time.chrono.ChronoLocalDate
		''' <summary>
		''' The resolved time.
		''' </summary>
		Private time As java.time.LocalTime
		''' <summary>
		''' The excess period from time-only parsing.
		''' </summary>
		Friend excessDays As java.time.Period = java.time.Period.ZERO

		''' <summary>
		''' Creates an instance.
		''' </summary>
		Friend Sub New()
		End Sub

		''' <summary>
		''' Creates a copy.
		''' </summary>
		Friend Function copy() As Parsed
			' only copy fields used in parsing stage
			Dim cloned As New Parsed
'JAVA TO VB CONVERTER TODO TASK: There is no .NET Dictionary equivalent to the Java 'putAll' method:
			cloned.fieldValues.putAll(Me.fieldValues)
			cloned.zone = Me.zone
			cloned.chrono = Me.chrono
			cloned.leapSecond = Me.leapSecond
			Return cloned
		End Function

		'-----------------------------------------------------------------------
		Public Overrides Function isSupported(  field As java.time.temporal.TemporalField) As Boolean
			If fieldValues.ContainsKey(field) OrElse ([date] IsNot Nothing AndAlso [date].isSupported(field)) OrElse (time IsNot Nothing AndAlso time.isSupported(field)) Then Return True
			Return field IsNot Nothing AndAlso (TypeOf field Is java.time.temporal.ChronoField = False) AndAlso field.isSupportedBy(Me)
		End Function

		Public Overrides Function getLong(  field As java.time.temporal.TemporalField) As Long
			java.util.Objects.requireNonNull(field, "field")
			Dim value As Long? = fieldValues(field)
			If value IsNot Nothing Then Return value
			If [date] IsNot Nothing AndAlso [date].isSupported(field) Then Return [date].getLong(field)
			If time IsNot Nothing AndAlso time.isSupported(field) Then Return time.getLong(field)
			If TypeOf field Is java.time.temporal.ChronoField Then Throw New java.time.temporal.UnsupportedTemporalTypeException("Unsupported field: " & field)
			Return field.getFrom(Me)
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overrides Function query(Of R)(  query_Renamed As java.time.temporal.TemporalQuery(Of R)) As R
			If query_Renamed Is java.time.temporal.TemporalQueries.zoneId() Then
				Return CType(zone, R)
			ElseIf query_Renamed Is java.time.temporal.TemporalQueries.chronology() Then
				Return CType(chrono, R)
			ElseIf query_Renamed Is java.time.temporal.TemporalQueries.localDate() Then
				Return CType(If([date] IsNot Nothing, java.time.LocalDate.from([date]), Nothing), R)
			ElseIf query_Renamed Is java.time.temporal.TemporalQueries.localTime() Then
				Return CType(time, R)
			ElseIf query_Renamed Is java.time.temporal.TemporalQueries.zone() OrElse query_Renamed Is java.time.temporal.TemporalQueries.offset() Then
				Return query_Renamed.queryFrom(Me)
			ElseIf query_Renamed Is java.time.temporal.TemporalQueries.precision() Then
				Return Nothing ' not a complete date/time
			End If
			' inline TemporalAccessor.super.query(query) as an optimization
			' non-JDK classes are not permitted to make this optimization
			Return query_Renamed.queryFrom(Me)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Resolves the fields in this context.
		''' </summary>
		''' <param name="resolverStyle">  the resolver style, not null </param>
		''' <param name="resolverFields">  the fields to use for resolving, null for all fields </param>
		''' <returns> this, for method chaining </returns>
		''' <exception cref="DateTimeException"> if resolving one field results in a value for
		'''  another field that is in conflict </exception>
		Friend Function resolve(  resolverStyle As ResolverStyle,   resolverFields As java.util.Set(Of java.time.temporal.TemporalField)) As java.time.temporal.TemporalAccessor
			If resolverFields IsNot Nothing Then fieldValues.Keys.retainAll(resolverFields)
			Me.resolverStyle = resolverStyle
			resolveFields()
			resolveTimeLenient()
			crossCheck()
			resolvePeriod()
			resolveFractional()
			resolveInstant()
			Return Me
		End Function

		'-----------------------------------------------------------------------
		Private Sub resolveFields()
			' resolve ChronoField
			resolveInstantFields()
			resolveDateFields()
			resolveTimeFields()

			' if any other fields, handle them
			' any lenient date resolution should return epoch-day
			If fieldValues.Count > 0 Then
				Dim changedCount As Integer = 0
				outer:
				Do While changedCount < 50
					For Each entry As KeyValuePair(Of java.time.temporal.TemporalField, Long?) In fieldValues
						Dim targetField As java.time.temporal.TemporalField = entry.Key
						Dim resolvedObject As java.time.temporal.TemporalAccessor = targetField.resolve(fieldValues, Me, resolverStyle)
						If resolvedObject IsNot Nothing Then
							If TypeOf resolvedObject Is java.time.chrono.ChronoZonedDateTime Then
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
								Dim czdt As java.time.chrono.ChronoZonedDateTime(Of ?) = CType(resolvedObject, java.time.chrono.ChronoZonedDateTime(Of ?))
								If zone Is Nothing Then
									zone = czdt.zone
								ElseIf zone.Equals(czdt.zone) = False Then
									Throw New java.time.DateTimeException("ChronoZonedDateTime must use the effective parsed zone: " & zone)
								End If
								resolvedObject = czdt.toLocalDateTime()
							End If
							If TypeOf resolvedObject Is java.time.chrono.ChronoLocalDateTime Then
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
								Dim cldt As java.time.chrono.ChronoLocalDateTime(Of ?) = CType(resolvedObject, java.time.chrono.ChronoLocalDateTime(Of ?))
								updateCheckConflict(cldt.toLocalTime(), java.time.Period.ZERO)
								updateCheckConflict(cldt.toLocalDate())
								changedCount += 1
								GoTo outer ' have to restart to avoid concurrent modification
							End If
							If TypeOf resolvedObject Is java.time.chrono.ChronoLocalDate Then
								updateCheckConflict(CType(resolvedObject, java.time.chrono.ChronoLocalDate))
								changedCount += 1
								GoTo outer ' have to restart to avoid concurrent modification
							End If
							If TypeOf resolvedObject Is java.time.LocalTime Then
								updateCheckConflict(CType(resolvedObject, java.time.LocalTime), java.time.Period.ZERO)
								changedCount += 1
								GoTo outer ' have to restart to avoid concurrent modification
							End If
							Throw New java.time.DateTimeException("Method resolve() can only return ChronoZonedDateTime, " & "ChronoLocalDateTime, ChronoLocalDate or LocalTime")
						ElseIf fieldValues.ContainsKey(targetField) = False Then
							changedCount += 1
							GoTo outer ' have to restart to avoid concurrent modification
						End If
					Next entry
					Exit Do
				Loop
				If changedCount = 50 Then ' catch infinite loops Throw New java.time.DateTimeException("One of the parsed fields has an incorrectly implemented resolve method")
				' if something changed then have to redo ChronoField resolve
				If changedCount > 0 Then
					resolveInstantFields()
					resolveDateFields()
					resolveTimeFields()
				End If
			End If
		End Sub

		Private Sub updateCheckConflict(  targetField As java.time.temporal.TemporalField,   changeField As java.time.temporal.TemporalField,   changeValue As Long?)
				fieldValues(changeField) = changeValue
				Dim old As Long? = fieldValues(changeField)
			If old IsNot Nothing AndAlso old <> changeValue Then Throw New java.time.DateTimeException("Conflict found: " & changeField & " " & old & " differs from " & changeField & " " & changeValue & " while resolving  " & targetField)
		End Sub

		'-----------------------------------------------------------------------
		Private Sub resolveInstantFields()
			' resolve parsed instant seconds to date and time if zone available
			If fieldValues.ContainsKey(INSTANT_SECONDS) Then
				If zone IsNot Nothing Then
					resolveInstantFields0(zone)
				Else
					Dim offsetSecs As Long? = fieldValues(OFFSET_SECONDS)
					If offsetSecs IsNot Nothing Then
						Dim offset As java.time.ZoneOffset = java.time.ZoneOffset.ofTotalSeconds(offsetSecs)
						resolveInstantFields0(offset)
					End If
				End If
			End If
		End Sub

		Private Sub resolveInstantFields0(  selectedZone As java.time.ZoneId)
			Dim instant_Renamed As java.time.Instant = java.time.Instant.ofEpochSecond(fieldValues.Remove(INSTANT_SECONDS))
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim zdt As java.time.chrono.ChronoZonedDateTime(Of ?) = chrono.zonedDateTime(instant_Renamed, selectedZone)
			updateCheckConflict(zdt.toLocalDate())
			updateCheckConflict(INSTANT_SECONDS, SECOND_OF_DAY, CLng(Fix(zdt.toLocalTime().toSecondOfDay())))
		End Sub

		'-----------------------------------------------------------------------
		Private Sub resolveDateFields()
			updateCheckConflict(chrono.resolveDate(fieldValues, resolverStyle))
		End Sub

		Private Sub updateCheckConflict(  cld As java.time.chrono.ChronoLocalDate)
			If [date] IsNot Nothing Then
				If cld IsNot Nothing AndAlso [date].Equals(cld) = False Then Throw New java.time.DateTimeException("Conflict found: Fields resolved to two different dates: " & [date] & " " & cld)
			ElseIf cld IsNot Nothing Then
				If chrono.Equals(cld.chronology) = False Then Throw New java.time.DateTimeException("ChronoLocalDate must use the effective parsed chronology: " & chrono)
				[date] = cld
			End If
		End Sub

		'-----------------------------------------------------------------------
		Private Sub resolveTimeFields()
			' simplify fields
			If fieldValues.ContainsKey(CLOCK_HOUR_OF_DAY) Then
				' lenient allows anything, smart allows 0-24, strict allows 1-24
				Dim ch As Long = fieldValues.Remove(CLOCK_HOUR_OF_DAY)
				If resolverStyle = ResolverStyle.STRICT OrElse (resolverStyle = ResolverStyle.SMART AndAlso ch <> 0) Then CLOCK_HOUR_OF_DAY.checkValidValue(ch)
				updateCheckConflict(CLOCK_HOUR_OF_DAY, HOUR_OF_DAY,If(ch = 24, 0, ch))
			End If
			If fieldValues.ContainsKey(CLOCK_HOUR_OF_AMPM) Then
				' lenient allows anything, smart allows 0-12, strict allows 1-12
				Dim ch As Long = fieldValues.Remove(CLOCK_HOUR_OF_AMPM)
				If resolverStyle = ResolverStyle.STRICT OrElse (resolverStyle = ResolverStyle.SMART AndAlso ch <> 0) Then CLOCK_HOUR_OF_AMPM.checkValidValue(ch)
				updateCheckConflict(CLOCK_HOUR_OF_AMPM, HOUR_OF_AMPM,If(ch = 12, 0, ch))
			End If
			If fieldValues.ContainsKey(AMPM_OF_DAY) AndAlso fieldValues.ContainsKey(HOUR_OF_AMPM) Then
				Dim ap As Long = fieldValues.Remove(AMPM_OF_DAY)
				Dim hap As Long = fieldValues.Remove(HOUR_OF_AMPM)
				If resolverStyle = ResolverStyle.LENIENT Then
					updateCheckConflict(AMPM_OF_DAY, HOUR_OF_DAY, System.Math.addExact (System.Math.multiplyExact(ap, 12), hap)) ' STRICT or SMART
				Else
					AMPM_OF_DAY.checkValidValue(ap)
					HOUR_OF_AMPM.checkValidValue(ap)
					updateCheckConflict(AMPM_OF_DAY, HOUR_OF_DAY, ap * 12 + hap)
				End If
			End If
			If fieldValues.ContainsKey(NANO_OF_DAY) Then
				Dim nod As Long = fieldValues.Remove(NANO_OF_DAY)
				If resolverStyle <> ResolverStyle.LENIENT Then NANO_OF_DAY.checkValidValue(nod)
				updateCheckConflict(NANO_OF_DAY, HOUR_OF_DAY, nod \ 3600_000_000_000L)
				updateCheckConflict(NANO_OF_DAY, MINUTE_OF_HOUR, (nod \ 60_000_000_000L) Mod 60)
				updateCheckConflict(NANO_OF_DAY, SECOND_OF_MINUTE, (nod \ 1_000_000_000L) Mod 60)
				updateCheckConflict(NANO_OF_DAY, NANO_OF_SECOND, nod Mod 1_000_000_000L)
			End If
			If fieldValues.ContainsKey(MICRO_OF_DAY) Then
				Dim cod As Long = fieldValues.Remove(MICRO_OF_DAY)
				If resolverStyle <> ResolverStyle.LENIENT Then MICRO_OF_DAY.checkValidValue(cod)
				updateCheckConflict(MICRO_OF_DAY, SECOND_OF_DAY, cod \ 1_000_000L)
				updateCheckConflict(MICRO_OF_DAY, MICRO_OF_SECOND, cod Mod 1_000_000L)
			End If
			If fieldValues.ContainsKey(MILLI_OF_DAY) Then
				Dim lod As Long = fieldValues.Remove(MILLI_OF_DAY)
				If resolverStyle <> ResolverStyle.LENIENT Then MILLI_OF_DAY.checkValidValue(lod)
				updateCheckConflict(MILLI_OF_DAY, SECOND_OF_DAY, lod \ 1000)
				updateCheckConflict(MILLI_OF_DAY, MILLI_OF_SECOND, lod Mod 1000)
			End If
			If fieldValues.ContainsKey(SECOND_OF_DAY) Then
				Dim sod As Long = fieldValues.Remove(SECOND_OF_DAY)
				If resolverStyle <> ResolverStyle.LENIENT Then SECOND_OF_DAY.checkValidValue(sod)
				updateCheckConflict(SECOND_OF_DAY, HOUR_OF_DAY, sod \ 3600)
				updateCheckConflict(SECOND_OF_DAY, MINUTE_OF_HOUR, (sod \ 60) Mod 60)
				updateCheckConflict(SECOND_OF_DAY, SECOND_OF_MINUTE, sod Mod 60)
			End If
			If fieldValues.ContainsKey(MINUTE_OF_DAY) Then
				Dim [mod] As Long = fieldValues.Remove(MINUTE_OF_DAY)
				If resolverStyle <> ResolverStyle.LENIENT Then MINUTE_OF_DAY.checkValidValue([mod])
				updateCheckConflict(MINUTE_OF_DAY, HOUR_OF_DAY, [mod] \ 60)
				updateCheckConflict(MINUTE_OF_DAY, MINUTE_OF_HOUR, mod Mod 60)
			End If

			' combine partial second fields strictly, leaving lenient expansion to later
			If fieldValues.ContainsKey(NANO_OF_SECOND) Then
				Dim nos As Long = fieldValues(NANO_OF_SECOND)
				If resolverStyle <> ResolverStyle.LENIENT Then NANO_OF_SECOND.checkValidValue(nos)
				If fieldValues.ContainsKey(MICRO_OF_SECOND) Then
					Dim cos As Long = fieldValues.Remove(MICRO_OF_SECOND)
					If resolverStyle <> ResolverStyle.LENIENT Then MICRO_OF_SECOND.checkValidValue(cos)
					nos = cos * 1000 + (nos Mod 1000)
					updateCheckConflict(MICRO_OF_SECOND, NANO_OF_SECOND, nos)
				End If
				If fieldValues.ContainsKey(MILLI_OF_SECOND) Then
					Dim los As Long = fieldValues.Remove(MILLI_OF_SECOND)
					If resolverStyle <> ResolverStyle.LENIENT Then MILLI_OF_SECOND.checkValidValue(los)
					updateCheckConflict(MILLI_OF_SECOND, NANO_OF_SECOND, los * 1_000_000L + (nos Mod 1_000_000L))
				End If
			End If

			' convert to time if all four fields available (optimization)
			If fieldValues.ContainsKey(HOUR_OF_DAY) AndAlso fieldValues.ContainsKey(MINUTE_OF_HOUR) AndAlso fieldValues.ContainsKey(SECOND_OF_MINUTE) AndAlso fieldValues.ContainsKey(NANO_OF_SECOND) Then
				Dim hod As Long = fieldValues.Remove(HOUR_OF_DAY)
				Dim moh As Long = fieldValues.Remove(MINUTE_OF_HOUR)
				Dim som As Long = fieldValues.Remove(SECOND_OF_MINUTE)
				Dim nos As Long = fieldValues.Remove(NANO_OF_SECOND)
				resolveTime(hod, moh, som, nos)
			End If
		End Sub

		Private Sub resolveTimeLenient()
			' leniently create a time from incomplete information
			' done after everything else as it creates information from nothing
			' which would break updateCheckConflict(field)

			If time Is Nothing Then
				' NANO_OF_SECOND merged with MILLI/MICRO above
				If fieldValues.ContainsKey(MILLI_OF_SECOND) Then
					Dim los As Long = fieldValues.Remove(MILLI_OF_SECOND)
					If fieldValues.ContainsKey(MICRO_OF_SECOND) Then
						' merge milli-of-second and micro-of-second for better error message
						Dim cos As Long = los * 1000 + (fieldValues(MICRO_OF_SECOND) Mod 1000)
						updateCheckConflict(MILLI_OF_SECOND, MICRO_OF_SECOND, cos)
						fieldValues.Remove(MICRO_OF_SECOND)
						fieldValues(NANO_OF_SECOND) = cos * 1_000L
					Else
						' convert milli-of-second to nano-of-second
						fieldValues(NANO_OF_SECOND) = los * 1_000_000L
					End If
				ElseIf fieldValues.ContainsKey(MICRO_OF_SECOND) Then
					' convert micro-of-second to nano-of-second
					Dim cos As Long = fieldValues.Remove(MICRO_OF_SECOND)
					fieldValues(NANO_OF_SECOND) = cos * 1_000L
				End If

				' merge hour/minute/second/nano leniently
				Dim hod As Long? = fieldValues(HOUR_OF_DAY)
				If hod IsNot Nothing Then
					Dim moh As Long? = fieldValues(MINUTE_OF_HOUR)
					Dim som As Long? = fieldValues(SECOND_OF_MINUTE)
					Dim nos As Long? = fieldValues(NANO_OF_SECOND)

					' check for invalid combinations that cannot be defaulted
					If (moh Is Nothing AndAlso (som IsNot Nothing OrElse nos IsNot Nothing)) OrElse (moh IsNot Nothing AndAlso som Is Nothing AndAlso nos IsNot Nothing) Then Return

					' default as necessary and build time
					Dim mohVal As Long = (If(moh IsNot Nothing, moh, 0))
					Dim somVal As Long = (If(som IsNot Nothing, som, 0))
					Dim nosVal As Long = (If(nos IsNot Nothing, nos, 0))
					resolveTime(hod, mohVal, somVal, nosVal)
					fieldValues.Remove(HOUR_OF_DAY)
					fieldValues.Remove(MINUTE_OF_HOUR)
					fieldValues.Remove(SECOND_OF_MINUTE)
					fieldValues.Remove(NANO_OF_SECOND)
				End If
			End If

			' validate remaining
			If resolverStyle <> ResolverStyle.LENIENT AndAlso fieldValues.Count > 0 Then
				For Each entry As KeyValuePair(Of java.time.temporal.TemporalField, Long?) In fieldValues
					Dim field As java.time.temporal.TemporalField = entry.Key
					If TypeOf field Is java.time.temporal.ChronoField AndAlso field.timeBased Then CType(field, java.time.temporal.ChronoField).checkValidValue(entry.Value)
				Next entry
			End If
		End Sub

		Private Sub resolveTime(  hod As Long,   moh As Long,   som As Long,   nos As Long)
			If resolverStyle = ResolverStyle.LENIENT Then
				Dim totalNanos As Long = System.Math.multiplyExact(hod, 3600_000_000_000L)
				totalNanos = System.Math.addExact(totalNanos, System.Math.multiplyExact(moh, 60_000_000_000L))
				totalNanos = System.Math.addExact(totalNanos, System.Math.multiplyExact(som, 1_000_000_000L))
				totalNanos = System.Math.addExact(totalNanos, nos)
				Dim excessDays As Integer = CInt (System.Math.floorDiv(totalNanos, 86400_000_000_000L)) ' safe int cast
				Dim nod As Long = System.Math.floorMod(totalNanos, 86400_000_000_000L)
				updateCheckConflict(java.time.LocalTime.ofNanoOfDay(nod), java.time.Period.ofDays(excessDays)) ' STRICT or SMART
			Else
				Dim mohVal As Integer = MINUTE_OF_HOUR.checkValidIntValue(moh)
				Dim nosVal As Integer = NANO_OF_SECOND.checkValidIntValue(nos)
				' handle 24:00 end of day
				If resolverStyle = ResolverStyle.SMART AndAlso hod = 24 AndAlso mohVal = 0 AndAlso som = 0 AndAlso nosVal = 0 Then
					updateCheckConflict(java.time.LocalTime.MIDNIGHT, java.time.Period.ofDays(1))
				Else
					Dim hodVal As Integer = HOUR_OF_DAY.checkValidIntValue(hod)
					Dim somVal As Integer = SECOND_OF_MINUTE.checkValidIntValue(som)
					updateCheckConflict(java.time.LocalTime.of(hodVal, mohVal, somVal, nosVal), java.time.Period.ZERO)
				End If
			End If
		End Sub

		Private Sub resolvePeriod()
			' add whole days if we have both date and time
			If [date] IsNot Nothing AndAlso time IsNot Nothing AndAlso excessDays.zero = False Then
				[date] = [date].plus(excessDays)
				excessDays = java.time.Period.ZERO
			End If
		End Sub

		Private Sub resolveFractional()
			' ensure fractional seconds available as ChronoField requires
			' resolveTimeLenient() will have merged MICRO_OF_SECOND/MILLI_OF_SECOND to NANO_OF_SECOND
			If time Is Nothing AndAlso (fieldValues.ContainsKey(INSTANT_SECONDS) OrElse fieldValues.ContainsKey(SECOND_OF_DAY) OrElse fieldValues.ContainsKey(SECOND_OF_MINUTE)) Then
				If fieldValues.ContainsKey(NANO_OF_SECOND) Then
					Dim nos As Long = fieldValues(NANO_OF_SECOND)
					fieldValues(MICRO_OF_SECOND) = nos \ 1000
					fieldValues(MILLI_OF_SECOND) = nos \ 1000000
				Else
					fieldValues(NANO_OF_SECOND) = 0L
					fieldValues(MICRO_OF_SECOND) = 0L
					fieldValues(MILLI_OF_SECOND) = 0L
				End If
			End If
		End Sub

		Private Sub resolveInstant()
			' add instant seconds if we have date, time and zone
			If [date] IsNot Nothing AndAlso time IsNot Nothing Then
				If zone IsNot Nothing Then
					Dim instant_Renamed As Long = [date].atTime(time).atZone(zone).getLong(java.time.temporal.ChronoField.INSTANT_SECONDS)
					fieldValues(INSTANT_SECONDS) = instant_Renamed
				Else
					Dim offsetSecs As Long? = fieldValues(OFFSET_SECONDS)
					If offsetSecs IsNot Nothing Then
						Dim offset As java.time.ZoneOffset = java.time.ZoneOffset.ofTotalSeconds(offsetSecs)
						Dim instant_Renamed As Long = [date].atTime(time).atZone(offset).getLong(java.time.temporal.ChronoField.INSTANT_SECONDS)
						fieldValues(INSTANT_SECONDS) = instant_Renamed
					End If
				End If
			End If
		End Sub

		Private Sub updateCheckConflict(  timeToSet As java.time.LocalTime,   periodToSet As java.time.Period)
			If time IsNot Nothing Then
				If time.Equals(timeToSet) = False Then Throw New java.time.DateTimeException("Conflict found: Fields resolved to different times: " & time & " " & timeToSet)
				If excessDays.zero = False AndAlso periodToSet.zero = False AndAlso excessDays.Equals(periodToSet) = False Then
					Throw New java.time.DateTimeException("Conflict found: Fields resolved to different excess periods: " & excessDays & " " & periodToSet)
				Else
					excessDays = periodToSet
				End If
			Else
				time = timeToSet
				excessDays = periodToSet
			End If
		End Sub

		'-----------------------------------------------------------------------
		Private Sub crossCheck()
			' only cross-check date, time and date-time
			' avoid object creation if possible
			If [date] IsNot Nothing Then crossCheck([date])
			If time IsNot Nothing Then
				crossCheck(time)
				If [date] IsNot Nothing AndAlso fieldValues.Count > 0 Then crossCheck([date].atTime(time))
			End If
		End Sub

		Private Sub crossCheck(  target As java.time.temporal.TemporalAccessor)
			Dim it As IEnumerator(Of KeyValuePair(Of java.time.temporal.TemporalField, Long?)) = fieldValues.GetEnumerator()
			Do While it.MoveNext()
				Dim entry As KeyValuePair(Of java.time.temporal.TemporalField, Long?) = it.Current
				Dim field As java.time.temporal.TemporalField = entry.Key
				If target.isSupported(field) Then
					Dim val1 As Long
					Try
						val1 = target.getLong(field)
					Catch ex As RuntimeException
						Continue Do
					End Try
					Dim val2 As Long = entry.Value
					If val1 <> val2 Then Throw New java.time.DateTimeException("Conflict found: Field " & field & " " & val1 & " differs from " & field & " " & val2 & " derived from " & target)
					it.remove()
				End If
			Loop
		End Sub

		'-----------------------------------------------------------------------
		Public Overrides Function ToString() As String
			Dim buf As New StringBuilder(64)
			buf.append(fieldValues).append(","c).append(chrono)
			If zone IsNot Nothing Then buf.append(","c).append(zone)
			If [date] IsNot Nothing OrElse time IsNot Nothing Then
				buf.append(" resolved to ")
				If [date] IsNot Nothing Then
					buf.append([date])
					If time IsNot Nothing Then buf.append("T"c).append(time)
				Else
					buf.append(time)
				End If
			End If
			Return buf.ToString()
		End Function

	End Class

End Namespace
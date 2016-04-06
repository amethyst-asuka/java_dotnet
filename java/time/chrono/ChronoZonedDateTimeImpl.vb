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
	''' A date-time with a time-zone in the calendar neutral API.
	''' <p>
	''' {@code ZoneChronoDateTime} is an immutable representation of a date-time with a time-zone.
	''' This class stores all date and time fields, to a precision of nanoseconds,
	''' as well as a time-zone and zone offset.
	''' <p>
	''' The purpose of storing the time-zone is to distinguish the ambiguous case where
	''' the local time-line overlaps, typically as a result of the end of daylight time.
	''' Information about the local-time can be obtained using methods on the time-zone.
	''' 
	''' @implSpec
	''' This class is immutable and thread-safe.
	''' 
	''' @serial Document the delegation of this class in the serialized-form specification. </summary>
	''' @param <D> the concrete type for the date of this date-time
	''' @since 1.8 </param>
	<Serializable> _
	Friend NotInheritable Class ChronoZonedDateTimeImpl(Of D As ChronoLocalDate)
		Implements ChronoZonedDateTime(Of D)

		''' <summary>
		''' Serialization version.
		''' </summary>
		Private Const serialVersionUID As Long = -5261813987200935591L

		''' <summary>
		''' The local date-time.
		''' </summary>
		<NonSerialized> _
		Private ReadOnly dateTime As ChronoLocalDateTimeImpl(Of D)
		''' <summary>
		''' The zone offset.
		''' </summary>
		<NonSerialized> _
		Private ReadOnly offset As java.time.ZoneOffset
		''' <summary>
		''' The zone ID.
		''' </summary>
		<NonSerialized> _
		Private ReadOnly zone As java.time.ZoneId

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains an instance from a local date-time using the preferred offset if possible.
		''' </summary>
		''' <param name="localDateTime">  the local date-time, not null </param>
		''' <param name="zone">  the zone identifier, not null </param>
		''' <param name="preferredOffset">  the zone offset, null if no preference </param>
		''' <returns> the zoned date-time, not null </returns>
		Friend Shared Function ofBest(Of R As ChronoLocalDate)(  localDateTime_Renamed As ChronoLocalDateTimeImpl(Of R),   zone As java.time.ZoneId,   preferredOffset As java.time.ZoneOffset) As ChronoZonedDateTime(Of R)
			java.util.Objects.requireNonNull(localDateTime_Renamed, "localDateTime")
			java.util.Objects.requireNonNull(zone, "zone")
			If TypeOf zone Is java.time.ZoneOffset Then Return New ChronoZonedDateTimeImpl(Of )(localDateTime_Renamed, CType(zone, java.time.ZoneOffset), zone)
			Dim rules As java.time.zone.ZoneRules = zone.rules
			Dim isoLDT As java.time.LocalDateTime = java.time.LocalDateTime.from(localDateTime_Renamed)
			Dim validOffsets As IList(Of java.time.ZoneOffset) = rules.getValidOffsets(isoLDT)
			Dim offset_Renamed As java.time.ZoneOffset
			If validOffsets.Count = 1 Then
				offset_Renamed = validOffsets(0)
			ElseIf validOffsets.Count = 0 Then
				Dim trans As java.time.zone.ZoneOffsetTransition = rules.getTransition(isoLDT)
				localDateTime_Renamed = localDateTime_Renamed.plusSeconds(trans.duration.seconds)
				offset_Renamed = trans.offsetAfter
			Else
				If preferredOffset IsNot Nothing AndAlso validOffsets.Contains(preferredOffset) Then
					offset_Renamed = preferredOffset
				Else
					offset_Renamed = validOffsets(0)
				End If
			End If
			java.util.Objects.requireNonNull(offset_Renamed, "offset") ' protect against bad ZoneRules
			Return New ChronoZonedDateTimeImpl(Of )(localDateTime_Renamed, offset_Renamed, zone)
		End Function

		''' <summary>
		''' Obtains an instance from an instant using the specified time-zone.
		''' </summary>
		''' <param name="chrono">  the chronology, not null </param>
		''' <param name="instant">  the instant, not null </param>
		''' <param name="zone">  the zone identifier, not null </param>
		''' <returns> the zoned date-time, not null </returns>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Shared Function ofInstant(  chrono As Chronology,   instant_Renamed As java.time.Instant,   zone As java.time.ZoneId) As ChronoZonedDateTimeImpl(Of ?)
			Dim rules As java.time.zone.ZoneRules = zone.rules
			Dim offset_Renamed As java.time.ZoneOffset = rules.getOffset(instant_Renamed)
			java.util.Objects.requireNonNull(offset_Renamed, "offset") ' protect against bad ZoneRules
			Dim ldt As java.time.LocalDateTime = java.time.LocalDateTime.ofEpochSecond(instant_Renamed.epochSecond, instant_Renamed.nano, offset_Renamed)
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim cldt As ChronoLocalDateTimeImpl(Of ?) = CType(chrono.localDateTime(ldt), ChronoLocalDateTimeImpl(Of ?))
			Return New ChronoZonedDateTimeImpl(Of )(cldt, offset_Renamed, zone)
		End Function

		''' <summary>
		''' Obtains an instance from an {@code Instant}.
		''' </summary>
		''' <param name="instant">  the instant to create the date-time from, not null </param>
		''' <param name="zone">  the time-zone to use, validated not null </param>
		''' <returns> the zoned date-time, validated not null </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Private Function create(  instant_Renamed As java.time.Instant,   zone As java.time.ZoneId) As ChronoZonedDateTimeImpl(Of D)
			Return CType(ofInstant(chronology, instant_Renamed, zone), ChronoZonedDateTimeImpl(Of D))
		End Function

		''' <summary>
		''' Casts the {@code Temporal} to {@code ChronoZonedDateTimeImpl} ensuring it bas the specified chronology.
		''' </summary>
		''' <param name="chrono">  the chronology to check for, not null </param>
		''' <param name="temporal">  a date-time to cast, not null </param>
		''' <returns> the date-time checked and cast to {@code ChronoZonedDateTimeImpl}, not null </returns>
		''' <exception cref="ClassCastException"> if the date-time cannot be cast to ChronoZonedDateTimeImpl
		'''  or the chronology is not equal this Chronology </exception>
		Shared Function ensureValid(Of R As ChronoLocalDate)(  chrono As Chronology,   temporal As java.time.temporal.Temporal) As ChronoZonedDateTimeImpl(Of R)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Dim other As ChronoZonedDateTimeImpl(Of R) = CType(temporal, ChronoZonedDateTimeImpl(Of R))
			If chrono.Equals(other.chronology) = False Then Throw New ClassCastException("Chronology mismatch, required: " & chrono.id & ", actual: " & other.chronology.id)
			Return other
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Constructor.
		''' </summary>
		''' <param name="dateTime">  the date-time, not null </param>
		''' <param name="offset">  the zone offset, not null </param>
		''' <param name="zone">  the zone ID, not null </param>
		Private Sub New(  dateTime As ChronoLocalDateTimeImpl(Of D),   offset As java.time.ZoneOffset,   zone As java.time.ZoneId)
			Me.dateTime = java.util.Objects.requireNonNull(dateTime, "dateTime")
			Me.offset = java.util.Objects.requireNonNull(offset, "offset")
			Me.zone = java.util.Objects.requireNonNull(zone, "zone")
		End Sub

		'-----------------------------------------------------------------------
		Public  Overrides ReadOnly Property  offset As java.time.ZoneOffset Implements ChronoZonedDateTime(Of D).getOffset
			Get
				Return offset
			End Get
		End Property

		Public Overrides Function withEarlierOffsetAtOverlap() As ChronoZonedDateTime(Of D) Implements ChronoZonedDateTime(Of D).withEarlierOffsetAtOverlap
			Dim trans As java.time.zone.ZoneOffsetTransition = zone.rules.getTransition(java.time.LocalDateTime.from(Me))
			If trans IsNot Nothing AndAlso trans.overlap Then
				Dim earlierOffset As java.time.ZoneOffset = trans.offsetBefore
				If earlierOffset.Equals(offset) = False Then Return New ChronoZonedDateTimeImpl(Of )(dateTime, earlierOffset, zone)
			End If
			Return Me
		End Function

		Public Overrides Function withLaterOffsetAtOverlap() As ChronoZonedDateTime(Of D) Implements ChronoZonedDateTime(Of D).withLaterOffsetAtOverlap
			Dim trans As java.time.zone.ZoneOffsetTransition = zone.rules.getTransition(java.time.LocalDateTime.from(Me))
			If trans IsNot Nothing Then
				Dim offset_Renamed As java.time.ZoneOffset = trans.offsetAfter
				If offset_Renamed.Equals(offset) = False Then Return New ChronoZonedDateTimeImpl(Of )(dateTime, offset_Renamed, zone)
			End If
			Return Me
		End Function

		'-----------------------------------------------------------------------
		Public Overrides Function toLocalDateTime() As ChronoLocalDateTime(Of D) Implements ChronoZonedDateTime(Of D).toLocalDateTime
			Return dateTime
		End Function

		Public  Overrides ReadOnly Property  zone As java.time.ZoneId Implements ChronoZonedDateTime(Of D).getZone
			Get
				Return zone
			End Get
		End Property

		Public Overrides Function withZoneSameLocal(  zone As java.time.ZoneId) As ChronoZonedDateTime(Of D) Implements ChronoZonedDateTime(Of D).withZoneSameLocal
			Return ofBest(dateTime, zone, offset)
		End Function

		Public Overrides Function withZoneSameInstant(  zone As java.time.ZoneId) As ChronoZonedDateTime(Of D) Implements ChronoZonedDateTime(Of D).withZoneSameInstant
			java.util.Objects.requireNonNull(zone, "zone")
			Return If(Me.zone.Equals(zone), Me, create(dateTime.toInstant(offset), zone))
		End Function

		'-----------------------------------------------------------------------
		Public Overrides Function isSupported(  field As java.time.temporal.TemporalField) As Boolean Implements ChronoZonedDateTime(Of D).isSupported
			Return TypeOf field Is java.time.temporal.ChronoField OrElse (field IsNot Nothing AndAlso field.isSupportedBy(Me))
		End Function

		'-----------------------------------------------------------------------
		Public Overrides Function [with](  field As java.time.temporal.TemporalField,   newValue As Long) As ChronoZonedDateTime(Of D) Implements ChronoZonedDateTime(Of D).with
			If TypeOf field Is java.time.temporal.ChronoField Then
				Dim f As java.time.temporal.ChronoField = CType(field, java.time.temporal.ChronoField)
				Select Case f
					Case INSTANT_SECONDS
						Return plus(newValue - toEpochSecond(), SECONDS)
					Case OFFSET_SECONDS
						Dim offset_Renamed As java.time.ZoneOffset = java.time.ZoneOffset.ofTotalSeconds(f.checkValidIntValue(newValue))
						Return create(dateTime.toInstant(offset_Renamed), zone)
				End Select
				Return ofBest(dateTime.with(field, newValue), zone, offset)
			End If
			Return ChronoZonedDateTimeImpl.ensureValid(chronology, field.adjustInto(Me, newValue))
		End Function

		'-----------------------------------------------------------------------
		Public Overrides Function plus(  amountToAdd As Long,   unit As java.time.temporal.TemporalUnit) As ChronoZonedDateTime(Of D) Implements ChronoZonedDateTime(Of D).plus
			If TypeOf unit Is java.time.temporal.ChronoUnit Then Return with(dateTime.plus(amountToAdd, unit))
			Return ChronoZonedDateTimeImpl.ensureValid(chronology, unit.addTo(Me, amountToAdd)) '/ TODO: Generics replacement Risk!
		End Function

		'-----------------------------------------------------------------------
		Public Overrides Function [until](  endExclusive As java.time.temporal.Temporal,   unit As java.time.temporal.TemporalUnit) As Long
			java.util.Objects.requireNonNull(endExclusive, "endExclusive")
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Dim [end] As ChronoZonedDateTime(Of D) = CType(chronology.zonedDateTime(endExclusive), ChronoZonedDateTime(Of D))
			If TypeOf unit Is java.time.temporal.ChronoUnit Then
				[end] = [end].withZoneSameInstant(offset)
				Return dateTime.until([end].toLocalDateTime(), unit)
			End If
			java.util.Objects.requireNonNull(unit, "unit")
			Return unit.between(Me, [end])
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Writes the ChronoZonedDateTime using a
		''' <a href="../../../serialized-form.html#java.time.chrono.Ser">dedicated serialized form</a>.
		''' @serialData
		''' <pre>
		'''  out.writeByte(3);                  // identifies a ChronoZonedDateTime
		'''  out.writeObject(toLocalDateTime());
		'''  out.writeObject(getOffset());
		'''  out.writeObject(getZone());
		''' </pre>
		''' </summary>
		''' <returns> the instance of {@code Ser}, not null </returns>
		Private Function writeReplace() As Object
			Return New Ser(Ser.CHRONO_ZONE_DATE_TIME_TYPE, Me)
		End Function

		''' <summary>
		''' Defend against malicious streams.
		''' </summary>
		''' <param name="s"> the stream to read </param>
		''' <exception cref="InvalidObjectException"> always </exception>
		Private Sub readObject(  s As java.io.ObjectInputStream)
			Throw New java.io.InvalidObjectException("Deserialization via serialization delegate")
		End Sub

		Friend Sub writeExternal(  out As java.io.ObjectOutput)
			out.writeObject(dateTime)
			out.writeObject(offset)
			out.writeObject(zone)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Friend Shared Function readExternal(  [in] As java.io.ObjectInput) As ChronoZonedDateTime(Of ?)
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim dateTime As ChronoLocalDateTime(Of ?) = CType([in].readObject(), ChronoLocalDateTime(Of ?))
			Dim offset_Renamed As java.time.ZoneOffset = CType([in].readObject(), java.time.ZoneOffset)
			Dim zone_Renamed As java.time.ZoneId = CType([in].readObject(), java.time.ZoneId)
			Return dateTime.atZone(offset_Renamed).withZoneSameLocal(zone_Renamed)
			' TODO: ZDT uses ofLenient()
		End Function

		'-------------------------------------------------------------------------
		Public Overrides Function Equals(  obj As Object) As Boolean
			If Me Is obj Then Return True
			If TypeOf obj Is ChronoZonedDateTime Then Return compareTo(CType(obj, ChronoZonedDateTime(Of ?))) = 0
			Return False
		End Function

		Public Overrides Function GetHashCode() As Integer
			Return toLocalDateTime().GetHashCode() Xor offset.GetHashCode() Xor  java.lang.[Integer].rotateLeft(zone.GetHashCode(), 3)
		End Function

		Public Overrides Function ToString() As String
			Dim str As String = toLocalDateTime().ToString() & offset.ToString()
			If offset IsNot zone Then str += AscW("["c) + zone.ToString() & AscW("]"c)
			Return str
		End Function


	End Class

End Namespace
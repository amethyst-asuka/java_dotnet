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
Namespace java.time


	''' <summary>
	''' The shared serialization delegate for this package.
	''' 
	''' @implNote
	''' This class wraps the object being serialized, and takes a byte representing the type of the class to
	''' be serialized.  This byte can also be used for versioning the serialization format.  In this case another
	''' byte flag would be used in order to specify an alternative version of the type format.
	''' For example {@code LOCAL_DATE_TYPE_VERSION_2 = 21}.
	''' <p>
	''' In order to serialize the object it writes its byte and then calls back to the appropriate class where
	''' the serialization is performed.  In order to deserialize the object it read in the type byte, switching
	''' in order to select which class to call back into.
	''' <p>
	''' The serialization format is determined on a per class basis.  In the case of field based classes each
	''' of the fields is written out with an appropriate size format in descending order of the field's size.  For
	''' example in the case of <seealso cref="LocalDate"/> year is written before month.  Composite classes, such as
	''' <seealso cref="LocalDateTime"/> are serialized as one object.
	''' <p>
	''' This class is mutable and should be created once per serialization.
	''' 
	''' @serial include
	''' @since 1.8
	''' </summary>
	Friend NotInheritable Class Ser
		Implements java.io.Externalizable

		''' <summary>
		''' Serialization version.
		''' </summary>
		Private Const serialVersionUID As Long = -7683839454370182990L

		Friend Const DURATION_TYPE As SByte = 1
		Friend Const INSTANT_TYPE As SByte = 2
		Friend Const LOCAL_DATE_TYPE As SByte = 3
		Friend Const LOCAL_TIME_TYPE As SByte = 4
		Friend Const LOCAL_DATE_TIME_TYPE As SByte = 5
		Friend Const ZONE_DATE_TIME_TYPE As SByte = 6
		Friend Const ZONE_REGION_TYPE As SByte = 7
		Friend Const ZONE_OFFSET_TYPE As SByte = 8
		Friend Const OFFSET_TIME_TYPE As SByte = 9
		Friend Const OFFSET_DATE_TIME_TYPE As SByte = 10
		Friend Const YEAR_TYPE As SByte = 11
		Friend Const YEAR_MONTH_TYPE As SByte = 12
		Friend Const MONTH_DAY_TYPE As SByte = 13
		Friend Const PERIOD_TYPE As SByte = 14

		''' <summary>
		''' The type being serialized. </summary>
		Private type As SByte
		''' <summary>
		''' The object being serialized. </summary>
		Private object_Renamed As Object

		''' <summary>
		''' Constructor for deserialization.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Creates an instance for serialization.
		''' </summary>
		''' <param name="type">  the type </param>
		''' <param name="object">  the object </param>
		Friend Sub New(  type As SByte,   [object] As Object)
			Me.type = type
			Me.object_Renamed = object_Renamed
		End Sub

		'-----------------------------------------------------------------------
		''' <summary>
		''' Implements the {@code Externalizable} interface to write the object.
		''' @serialData
		''' 
		''' Each serializable class is mapped to a type that is the first byte
		''' in the stream.  Refer to each class {@code writeReplace}
		''' serialized form for the value of the type and sequence of values for the type.
		''' <ul>
		''' <li><a href="../../serialized-form.html#java.time.Duration">Duration.writeReplace</a>
		''' <li><a href="../../serialized-form.html#java.time.Instant">Instant.writeReplace</a>
		''' <li><a href="../../serialized-form.html#java.time.LocalDate">LocalDate.writeReplace</a>
		''' <li><a href="../../serialized-form.html#java.time.LocalDateTime">LocalDateTime.writeReplace</a>
		''' <li><a href="../../serialized-form.html#java.time.LocalTime">LocalTime.writeReplace</a>
		''' <li><a href="../../serialized-form.html#java.time.MonthDay">MonthDay.writeReplace</a>
		''' <li><a href="../../serialized-form.html#java.time.OffsetTime">OffsetTime.writeReplace</a>
		''' <li><a href="../../serialized-form.html#java.time.OffsetDateTime">OffsetDateTime.writeReplace</a>
		''' <li><a href="../../serialized-form.html#java.time.Period">Period.writeReplace</a>
		''' <li><a href="../../serialized-form.html#java.time.Year">Year.writeReplace</a>
		''' <li><a href="../../serialized-form.html#java.time.YearMonth">YearMonth.writeReplace</a>
		''' <li><a href="../../serialized-form.html#java.time.ZoneId">ZoneId.writeReplace</a>
		''' <li><a href="../../serialized-form.html#java.time.ZoneOffset">ZoneOffset.writeReplace</a>
		''' <li><a href="../../serialized-form.html#java.time.ZonedDateTime">ZonedDateTime.writeReplace</a>
		''' </ul>
		''' </summary>
		''' <param name="out">  the data stream to write to, not null </param>
		Public Overrides Sub writeExternal(  out As java.io.ObjectOutput)
			writeInternal(type, object_Renamed, out)
		End Sub

		Friend Shared Sub writeInternal(  type As SByte,   [object] As Object,   out As java.io.ObjectOutput)
			out.writeByte(type)
			Select Case type
				Case DURATION_TYPE
					CType(object_Renamed, Duration).writeExternal(out)
				Case INSTANT_TYPE
					CType(object_Renamed, Instant).writeExternal(out)
				Case LOCAL_DATE_TYPE
					CType(object_Renamed, LocalDate).writeExternal(out)
				Case LOCAL_DATE_TIME_TYPE
					CType(object_Renamed, LocalDateTime).writeExternal(out)
				Case LOCAL_TIME_TYPE
					CType(object_Renamed, LocalTime).writeExternal(out)
				Case ZONE_REGION_TYPE
					CType(object_Renamed, ZoneRegion).writeExternal(out)
				Case ZONE_OFFSET_TYPE
					CType(object_Renamed, ZoneOffset).writeExternal(out)
				Case ZONE_DATE_TIME_TYPE
					CType(object_Renamed, ZonedDateTime).writeExternal(out)
				Case OFFSET_TIME_TYPE
					CType(object_Renamed, OffsetTime).writeExternal(out)
				Case OFFSET_DATE_TIME_TYPE
					CType(object_Renamed, OffsetDateTime).writeExternal(out)
				Case YEAR_TYPE
					CType(object_Renamed, Year).writeExternal(out)
				Case YEAR_MONTH_TYPE
					CType(object_Renamed, YearMonth).writeExternal(out)
				Case MONTH_DAY_TYPE
					CType(object_Renamed, MonthDay).writeExternal(out)
				Case PERIOD_TYPE
					CType(object_Renamed, Period).writeExternal(out)
				Case Else
					Throw New java.io.InvalidClassException("Unknown serialized type")
			End Select
		End Sub

		'-----------------------------------------------------------------------
		''' <summary>
		''' Implements the {@code Externalizable} interface to read the object.
		''' @serialData
		''' 
		''' The streamed type and parameters defined by the type's {@code writeReplace}
		''' method are read and passed to the corresponding static factory for the type
		''' to create a new instance.  That instance is returned as the de-serialized
		''' {@code Ser} object.
		''' 
		''' <ul>
		''' <li><a href="../../serialized-form.html#java.time.Duration">Duration</a> - {@code Duration.ofSeconds(seconds, nanos);}
		''' <li><a href="../../serialized-form.html#java.time.Instant">Instant</a> - {@code Instant.ofEpochSecond(seconds, nanos);}
		''' <li><a href="../../serialized-form.html#java.time.LocalDate">LocalDate</a> - {@code LocalDate.of(year, month, day);}
		''' <li><a href="../../serialized-form.html#java.time.LocalDateTime">LocalDateTime</a> - {@code LocalDateTime.of(date, time);}
		''' <li><a href="../../serialized-form.html#java.time.LocalTime">LocalTime</a> - {@code LocalTime.of(hour, minute, second, nano);}
		''' <li><a href="../../serialized-form.html#java.time.MonthDay">MonthDay</a> - {@code MonthDay.of(month, day);}
		''' <li><a href="../../serialized-form.html#java.time.OffsetTime">OffsetTime</a> - {@code OffsetTime.of(time, offset);}
		''' <li><a href="../../serialized-form.html#java.time.OffsetDateTime">OffsetDateTime</a> - {@code OffsetDateTime.of(dateTime, offset);}
		''' <li><a href="../../serialized-form.html#java.time.Period">Period</a> - {@code Period.of(years, months, days);}
		''' <li><a href="../../serialized-form.html#java.time.Year">Year</a> - {@code Year.of(year);}
		''' <li><a href="../../serialized-form.html#java.time.YearMonth">YearMonth</a> - {@code YearMonth.of(year, month);}
		''' <li><a href="../../serialized-form.html#java.time.ZonedDateTime">ZonedDateTime</a> - {@code ZonedDateTime.ofLenient(dateTime, offset, zone);}
		''' <li><a href="../../serialized-form.html#java.time.ZoneId">ZoneId</a> - {@code ZoneId.of(id);}
		''' <li><a href="../../serialized-form.html#java.time.ZoneOffset">ZoneOffset</a> - {@code (offsetByte == 127 ? ZoneOffset.ofTotalSeconds(in.readInt()) : ZoneOffset.ofTotalSeconds(offsetByte * 900));}
		''' </ul>
		''' </summary>
		''' <param name="in">  the data to read, not null </param>
		Public Sub readExternal(  [in] As java.io.ObjectInput)
			type = [in].readByte()
			object_Renamed = readInternal(type, [in])
		End Sub

		Friend Shared Function read(  [in] As java.io.ObjectInput) As Object
			Dim type As SByte = [in].readByte()
			Return readInternal(type, [in])
		End Function

		Private Shared Function readInternal(  type As SByte,   [in] As java.io.ObjectInput) As Object
			Select Case type
				Case DURATION_TYPE
					Return Duration.readExternal([in])
				Case INSTANT_TYPE
					Return Instant.readExternal([in])
				Case LOCAL_DATE_TYPE
					Return LocalDate.readExternal([in])
				Case LOCAL_DATE_TIME_TYPE
					Return LocalDateTime.readExternal([in])
				Case LOCAL_TIME_TYPE
					Return LocalTime.readExternal([in])
				Case ZONE_DATE_TIME_TYPE
					Return ZonedDateTime.readExternal([in])
				Case ZONE_OFFSET_TYPE
					Return ZoneOffset.readExternal([in])
				Case ZONE_REGION_TYPE
					Return ZoneRegion.readExternal([in])
				Case OFFSET_TIME_TYPE
					Return OffsetTime.readExternal([in])
				Case OFFSET_DATE_TIME_TYPE
					Return OffsetDateTime.readExternal([in])
				Case YEAR_TYPE
					Return Year.readExternal([in])
				Case YEAR_MONTH_TYPE
					Return YearMonth.readExternal([in])
				Case MONTH_DAY_TYPE
					Return MonthDay.readExternal([in])
				Case PERIOD_TYPE
					Return Period.readExternal([in])
				Case Else
					Throw New java.io.StreamCorruptedException("Unknown serialized type")
			End Select
		End Function

		''' <summary>
		''' Returns the object that will replace this one.
		''' </summary>
		''' <returns> the read object, should never be null </returns>
		Private Function readResolve() As Object
			 Return object_Renamed
		End Function

	End Class

End Namespace
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
Namespace java.time.chrono


	''' <summary>
	''' The shared serialization delegate for this package.
	''' 
	''' @implNote
	''' This class wraps the object being serialized, and takes a byte representing the type of the class to
	''' be serialized.  This byte can also be used for versioning the serialization format.  In this case another
	''' byte flag would be used in order to specify an alternative version of the type format.
	''' For example {@code CHRONO_TYPE_VERSION_2 = 21}
	''' <p>
	''' In order to serialize the object it writes its byte and then calls back to the appropriate class where
	''' the serialization is performed.  In order to deserialize the object it read in the type byte, switching
	''' in order to select which class to call back into.
	''' <p>
	''' The serialization format is determined on a per class basis.  In the case of field based classes each
	''' of the fields is written out with an appropriate size format in descending order of the field's size.  For
	''' example in the case of <seealso cref="LocalDate"/> year is written before month.  Composite classes, such as
	''' <seealso cref="LocalDateTime"/> are serialized as one object.  Enum classes are serialized using the index of their
	''' element.
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
		Private Const serialVersionUID As Long = -6103370247208168577L

		Friend Const CHRONO_TYPE As SByte = 1
		Friend Const CHRONO_LOCAL_DATE_TIME_TYPE As SByte = 2
		Friend Const CHRONO_ZONE_DATE_TIME_TYPE As SByte = 3
		Friend Const JAPANESE_DATE_TYPE As SByte = 4
		Friend Const JAPANESE_ERA_TYPE As SByte = 5
		Friend Const HIJRAH_DATE_TYPE As SByte = 6
		Friend Const MINGUO_DATE_TYPE As SByte = 7
		Friend Const THAIBUDDHIST_DATE_TYPE As SByte = 8
		Friend Const CHRONO_PERIOD_TYPE As SByte = 9

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
		''' Each serializable class is mapped to a type that is the first byte
		''' in the stream.  Refer to each class {@code writeReplace}
		''' serialized form for the value of the type and sequence of values for the type.
		''' <ul>
		''' <li><a href="../../../serialized-form.html#java.time.chrono.HijrahChronology">HijrahChronology.writeReplace</a>
		''' <li><a href="../../../serialized-form.html#java.time.chrono.IsoChronology">IsoChronology.writeReplace</a>
		''' <li><a href="../../../serialized-form.html#java.time.chrono.JapaneseChronology">JapaneseChronology.writeReplace</a>
		''' <li><a href="../../../serialized-form.html#java.time.chrono.MinguoChronology">MinguoChronology.writeReplace</a>
		''' <li><a href="../../../serialized-form.html#java.time.chrono.ThaiBuddhistChronology">ThaiBuddhistChronology.writeReplace</a>
		''' <li><a href="../../../serialized-form.html#java.time.chrono.ChronoLocalDateTimeImpl">ChronoLocalDateTime.writeReplace</a>
		''' <li><a href="../../../serialized-form.html#java.time.chrono.ChronoZonedDateTimeImpl">ChronoZonedDateTime.writeReplace</a>
		''' <li><a href="../../../serialized-form.html#java.time.chrono.JapaneseDate">JapaneseDate.writeReplace</a>
		''' <li><a href="../../../serialized-form.html#java.time.chrono.JapaneseEra">JapaneseEra.writeReplace</a>
		''' <li><a href="../../../serialized-form.html#java.time.chrono.HijrahDate">HijrahDate.writeReplace</a>
		''' <li><a href="../../../serialized-form.html#java.time.chrono.MinguoDate">MinguoDate.writeReplace</a>
		''' <li><a href="../../../serialized-form.html#java.time.chrono.ThaiBuddhistDate">ThaiBuddhistDate.writeReplace</a>
		''' </ul>
		''' </summary>
		''' <param name="out">  the data stream to write to, not null </param>
		Public Overrides Sub writeExternal(  out As java.io.ObjectOutput)
			writeInternal(type, object_Renamed, out)
		End Sub

		Private Shared Sub writeInternal(  type As SByte,   [object] As Object,   out As java.io.ObjectOutput)
			out.writeByte(type)
			Select Case type
				Case CHRONO_TYPE
					CType(object_Renamed, AbstractChronology).writeExternal(out)
				Case CHRONO_LOCAL_DATE_TIME_TYPE
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					CType(object_Renamed, ChronoLocalDateTimeImpl(Of ?)).writeExternal(out)
				Case CHRONO_ZONE_DATE_TIME_TYPE
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
					CType(object_Renamed, ChronoZonedDateTimeImpl(Of ?)).writeExternal(out)
				Case JAPANESE_DATE_TYPE
					CType(object_Renamed, JapaneseDate).writeExternal(out)
				Case JAPANESE_ERA_TYPE
					CType(object_Renamed, JapaneseEra).writeExternal(out)
				Case HIJRAH_DATE_TYPE
					CType(object_Renamed, HijrahDate).writeExternal(out)
				Case MINGUO_DATE_TYPE
					CType(object_Renamed, MinguoDate).writeExternal(out)
				Case THAIBUDDHIST_DATE_TYPE
					CType(object_Renamed, ThaiBuddhistDate).writeExternal(out)
				Case CHRONO_PERIOD_TYPE
					CType(object_Renamed, ChronoPeriodImpl).writeExternal(out)
				Case Else
					Throw New java.io.InvalidClassException("Unknown serialized type")
			End Select
		End Sub

		'-----------------------------------------------------------------------
		''' <summary>
		''' Implements the {@code Externalizable} interface to read the object.
		''' @serialData
		''' The streamed type and parameters defined by the type's {@code writeReplace}
		''' method are read and passed to the corresponding static factory for the type
		''' to create a new instance.  That instance is returned as the de-serialized
		''' {@code Ser} object.
		''' 
		''' <ul>
		''' <li><a href="../../../serialized-form.html#java.time.chrono.HijrahChronology">HijrahChronology</a> - Chronology.of(id)
		''' <li><a href="../../../serialized-form.html#java.time.chrono.IsoChronology">IsoChronology</a> - Chronology.of(id)
		''' <li><a href="../../../serialized-form.html#java.time.chrono.JapaneseChronology">JapaneseChronology</a> - Chronology.of(id)
		''' <li><a href="../../../serialized-form.html#java.time.chrono.MinguoChronology">MinguoChronology</a> - Chronology.of(id)
		''' <li><a href="../../../serialized-form.html#java.time.chrono.ThaiBuddhistChronology">ThaiBuddhistChronology</a> - Chronology.of(id)
		''' <li><a href="../../../serialized-form.html#java.time.chrono.ChronoLocalDateTimeImpl">ChronoLocalDateTime</a> - date.atTime(time)
		''' <li><a href="../../../serialized-form.html#java.time.chrono.ChronoZonedDateTimeImpl">ChronoZonedDateTime</a> - dateTime.atZone(offset).withZoneSameLocal(zone)
		''' <li><a href="../../../serialized-form.html#java.time.chrono.JapaneseDate">JapaneseDate</a> - JapaneseChronology.INSTANCE.date(year, month, dayOfMonth)
		''' <li><a href="../../../serialized-form.html#java.time.chrono.JapaneseEra">JapaneseEra</a> - JapaneseEra.of(eraValue)
		''' <li><a href="../../../serialized-form.html#java.time.chrono.HijrahDate">HijrahDate</a> - HijrahChronology chrono.date(year, month, dayOfMonth)
		''' <li><a href="../../../serialized-form.html#java.time.chrono.MinguoDate">MinguoDate</a> - MinguoChronology.INSTANCE.date(year, month, dayOfMonth)
		''' <li><a href="../../../serialized-form.html#java.time.chrono.ThaiBuddhistDate">ThaiBuddhistDate</a> - ThaiBuddhistChronology.INSTANCE.date(year, month, dayOfMonth)
		''' </ul>
		''' </summary>
		''' <param name="in">  the data stream to read from, not null </param>
		Public Overrides Sub readExternal(  [in] As java.io.ObjectInput)
			type = [in].readByte()
			object_Renamed = readInternal(type, [in])
		End Sub

		Friend Shared Function read(  [in] As java.io.ObjectInput) As Object
			Dim type As SByte = [in].readByte()
			Return readInternal(type, [in])
		End Function

		Private Shared Function readInternal(  type As SByte,   [in] As java.io.ObjectInput) As Object
			Select Case type
				Case CHRONO_TYPE
					Return AbstractChronology.readExternal([in])
				Case CHRONO_LOCAL_DATE_TIME_TYPE
					Return ChronoLocalDateTimeImpl.readExternal([in])
				Case CHRONO_ZONE_DATE_TIME_TYPE
					Return ChronoZonedDateTimeImpl.readExternal([in])
				Case JAPANESE_DATE_TYPE
					Return JapaneseDate.readExternal([in])
				Case JAPANESE_ERA_TYPE
					Return JapaneseEra.readExternal([in])
				Case HIJRAH_DATE_TYPE
					Return HijrahDate.readExternal([in])
				Case MINGUO_DATE_TYPE
					Return MinguoDate.readExternal([in])
				Case THAIBUDDHIST_DATE_TYPE
					Return ThaiBuddhistDate.readExternal([in])
				Case CHRONO_PERIOD_TYPE
					Return ChronoPeriodImpl.readExternal([in])
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
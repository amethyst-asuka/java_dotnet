Imports System

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
	''' An era in the Japanese Imperial calendar system.
	''' <p>
	''' This class defines the valid eras for the Japanese chronology.
	''' Japan introduced the Gregorian calendar starting with Meiji 6.
	''' Only Meiji and later eras are supported;
	''' dates before Meiji 6, January 1 are not supported.
	''' 
	''' @implSpec
	''' This class is immutable and thread-safe.
	''' 
	''' @since 1.8
	''' </summary>
	<Serializable> _
	Public NotInheritable Class JapaneseEra
		Implements Era

		' The offset value to 0-based index from the era value.
		' i.e., getValue() + ERA_OFFSET == 0-based index
		Friend Const ERA_OFFSET As Integer = 2

		Friend Shared ReadOnly ERA_CONFIG As sun.util.calendar.Era()

		''' <summary>
		''' The singleton instance for the 'Meiji' era (1868-01-01 - 1912-07-29)
		''' which has the value -1.
		''' </summary>
		Public Shared ReadOnly MEIJI As New JapaneseEra(-1, java.time.LocalDate.of(1868, 1, 1))
		''' <summary>
		''' The singleton instance for the 'Taisho' era (1912-07-30 - 1926-12-24)
		''' which has the value 0.
		''' </summary>
		Public Shared ReadOnly TAISHO As New JapaneseEra(0, java.time.LocalDate.of(1912, 7, 30))
		''' <summary>
		''' The singleton instance for the 'Showa' era (1926-12-25 - 1989-01-07)
		''' which has the value 1.
		''' </summary>
		Public Shared ReadOnly SHOWA As New JapaneseEra(1, java.time.LocalDate.of(1926, 12, 25))
		''' <summary>
		''' The singleton instance for the 'Heisei' era (1989-01-08 - current)
		''' which has the value 2.
		''' </summary>
		Public Shared ReadOnly HEISEI As New JapaneseEra(2, java.time.LocalDate.of(1989, 1, 8))

		' the number of defined JapaneseEra constants.
		' There could be an extra era defined in its configuration.
		Private Shared ReadOnly N_ERA_CONSTANTS As Integer = HEISEI.value + ERA_OFFSET

		''' <summary>
		''' Serialization version.
		''' </summary>
		Private Const serialVersionUID As Long = 1466499369062886794L

		' array for the singleton JapaneseEra instances
		Private Shared ReadOnly KNOWN_ERAS As JapaneseEra()

		Shared Sub New()
			ERA_CONFIG = JapaneseChronology.JCAL.eras

			KNOWN_ERAS = New JapaneseEra(ERA_CONFIG.Length - 1){}
			KNOWN_ERAS(0) = MEIJI
			KNOWN_ERAS(1) = TAISHO
			KNOWN_ERAS(2) = SHOWA
			KNOWN_ERAS(3) = HEISEI
			For i As Integer = N_ERA_CONSTANTS To ERA_CONFIG.Length - 1
				Dim date_Renamed As sun.util.calendar.CalendarDate = ERA_CONFIG(i).sinceDate
				Dim isoDate As java.time.LocalDate = java.time.LocalDate.of(date_Renamed.year, date_Renamed.month, date_Renamed.dayOfMonth)
				KNOWN_ERAS(i) = New JapaneseEra(i - ERA_OFFSET + 1, isoDate)
			Next i
		End Sub

		''' <summary>
		''' The era value.
		''' @serial
		''' </summary>
		<NonSerialized> _
		Private ReadOnly eraValue As Integer

		' the first day of the era
		<NonSerialized> _
		Private ReadOnly since As java.time.LocalDate

		''' <summary>
		''' Creates an instance.
		''' </summary>
		''' <param name="eraValue">  the era value, validated </param>
		''' <param name="since">  the date representing the first date of the era, validated not null </param>
		Private Sub New(ByVal eraValue As Integer, ByVal since As java.time.LocalDate)
			Me.eraValue = eraValue
			Me.since = since
		End Sub

		'-----------------------------------------------------------------------
		''' <summary>
		''' Returns the Sun private Era instance corresponding to this {@code JapaneseEra}.
		''' </summary>
		''' <returns> the Sun private Era instance for this {@code JapaneseEra}. </returns>
		Friend Property privateEra As sun.util.calendar.Era
			Get
				Return ERA_CONFIG(ordinal(eraValue))
			End Get
		End Property

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains an instance of {@code JapaneseEra} from an {@code int} value.
		''' <p>
		''' The <seealso cref="#SHOWA"/> era that contains 1970-01-01 (ISO calendar system) has the value 1
		''' Later era is numbered 2 (<seealso cref="#HEISEI"/>). Earlier eras are numbered 0 (<seealso cref="#TAISHO"/>),
		''' -1 (<seealso cref="#MEIJI"/>), only Meiji and later eras are supported.
		''' </summary>
		''' <param name="japaneseEra">  the era to represent </param>
		''' <returns> the {@code JapaneseEra} singleton, not null </returns>
		''' <exception cref="DateTimeException"> if the value is invalid </exception>
		Public Shared Function [of](ByVal japaneseEra_Renamed As Integer) As JapaneseEra
			If japaneseEra_Renamed < MEIJI.eraValue OrElse japaneseEra_Renamed + ERA_OFFSET > KNOWN_ERAS.Length Then Throw New java.time.DateTimeException("Invalid era: " & japaneseEra_Renamed)
			Return KNOWN_ERAS(ordinal(japaneseEra_Renamed))
		End Function

		''' <summary>
		''' Returns the {@code JapaneseEra} with the name.
		''' <p>
		''' The string must match exactly the name of the era.
		''' (Extraneous whitespace characters are not permitted.)
		''' </summary>
		''' <param name="japaneseEra">  the japaneseEra name; non-null </param>
		''' <returns> the {@code JapaneseEra} singleton, never null </returns>
		''' <exception cref="IllegalArgumentException"> if there is not JapaneseEra with the specified name </exception>
		Public Shared Function valueOf(ByVal japaneseEra_Renamed As String) As JapaneseEra
			java.util.Objects.requireNonNull(japaneseEra_Renamed, "japaneseEra")
			For Each era As JapaneseEra In KNOWN_ERAS
				If era.name.Equals(japaneseEra_Renamed) Then Return era
			Next era
			Throw New IllegalArgumentException("japaneseEra is invalid")
		End Function

		''' <summary>
		''' Returns an array of JapaneseEras.
		''' <p>
		''' This method may be used to iterate over the JapaneseEras as follows:
		''' <pre>
		''' for (JapaneseEra c : JapaneseEra.values())
		'''     System.out.println(c);
		''' </pre>
		''' </summary>
		''' <returns> an array of JapaneseEras </returns>
		Public Shared Function values() As JapaneseEra()
			Return java.util.Arrays.copyOf(KNOWN_ERAS, KNOWN_ERAS.Length)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Obtains an instance of {@code JapaneseEra} from a date.
		''' </summary>
		''' <param name="date">  the date, not null </param>
		''' <returns> the Era singleton, never null </returns>
		Shared Function [from](ByVal [date] As java.time.LocalDate) As JapaneseEra
			If date_Renamed.isBefore(MEIJI_6_ISODATE) Then Throw New java.time.DateTimeException("JapaneseDate before Meiji 6 are not supported")
			For i As Integer = KNOWN_ERAS.Length - 1 To 1 Step -1
				Dim era As JapaneseEra = KNOWN_ERAS(i)
				If date_Renamed.CompareTo(era.since) >= 0 Then Return era
			Next i
			Return Nothing
		End Function

		Shared Function toJapaneseEra(ByVal privateEra As sun.util.calendar.Era) As JapaneseEra
			For i As Integer = ERA_CONFIG.Length - 1 To 0 Step -1
				If ERA_CONFIG(i).Equals(privateEra) Then Return KNOWN_ERAS(i)
			Next i
			Return Nothing
		End Function

		Friend Shared Function privateEraFrom(ByVal isoDate As java.time.LocalDate) As sun.util.calendar.Era
			For i As Integer = KNOWN_ERAS.Length - 1 To 1 Step -1
				Dim era As JapaneseEra = KNOWN_ERAS(i)
				If isoDate.CompareTo(era.since) >= 0 Then Return ERA_CONFIG(i)
			Next i
			Return Nothing
		End Function

		''' <summary>
		''' Returns the index into the arrays from the Era value.
		''' the eraValue is a valid Era number, -1..2.
		''' </summary>
		''' <param name="eraValue">  the era value to convert to the index </param>
		''' <returns> the index of the current Era </returns>
		Private Shared Function ordinal(ByVal eraValue As Integer) As Integer
			Return eraValue + ERA_OFFSET - 1
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Gets the numeric era {@code int} value.
		''' <p>
		''' The <seealso cref="#SHOWA"/> era that contains 1970-01-01 (ISO calendar system) has the value 1.
		''' Later eras are numbered from 2 (<seealso cref="#HEISEI"/>).
		''' Earlier eras are numbered 0 (<seealso cref="#TAISHO"/>), -1 (<seealso cref="#MEIJI"/>)).
		''' </summary>
		''' <returns> the era value </returns>
		Public  Overrides ReadOnly Property  value As Integer Implements Era.getValue
			Get
				Return eraValue
			End Get
		End Property

		'-----------------------------------------------------------------------
		''' <summary>
		''' Gets the range of valid values for the specified field.
		''' <p>
		''' The range object expresses the minimum and maximum valid values for a field.
		''' This era is used to enhance the accuracy of the returned range.
		''' If it is not possible to return the range, because the field is not supported
		''' or for some other reason, an exception is thrown.
		''' <p>
		''' If the field is a <seealso cref="ChronoField"/> then the query is implemented here.
		''' The {@code ERA} field returns the range.
		''' All other {@code ChronoField} instances will throw an {@code UnsupportedTemporalTypeException}.
		''' <p>
		''' If the field is not a {@code ChronoField}, then the result of this method
		''' is obtained by invoking {@code TemporalField.rangeRefinedBy(TemporalAccessor)}
		''' passing {@code this} as the argument.
		''' Whether the range can be obtained is determined by the field.
		''' <p>
		''' The range of valid Japanese eras can change over time due to the nature
		''' of the Japanese calendar system.
		''' </summary>
		''' <param name="field">  the field to query the range for, not null </param>
		''' <returns> the range of valid values for the field, not null </returns>
		''' <exception cref="DateTimeException"> if the range for the field cannot be obtained </exception>
		''' <exception cref="UnsupportedTemporalTypeException"> if the unit is not supported </exception>
		Public Overrides Function range(ByVal field As java.time.temporal.TemporalField) As java.time.temporal.ValueRange Implements Era.range ' override as super would return range from 0 to 1
			If field = ERA Then Return JapaneseChronology.INSTANCE.range(ERA)
			Return outerInstance.range(field)
		End Function

		'-----------------------------------------------------------------------
		Friend Property abbreviation As String
			Get
				Dim index As Integer = ordinal(value)
				If index = 0 Then Return ""
				Return ERA_CONFIG(index).abbreviation
			End Get
		End Property

		Friend Property name As String
			Get
				Return ERA_CONFIG(ordinal(value)).name
			End Get
		End Property

		Public Overrides Function ToString() As String
			Return name
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Defend against malicious streams.
		''' </summary>
		''' <param name="s"> the stream to read </param>
		''' <exception cref="InvalidObjectException"> always </exception>
		Private Sub readObject(ByVal s As java.io.ObjectInputStream)
			Throw New java.io.InvalidObjectException("Deserialization via serialization delegate")
		End Sub

		'-----------------------------------------------------------------------
		''' <summary>
		''' Writes the object using a
		''' <a href="../../../serialized-form.html#java.time.chrono.Ser">dedicated serialized form</a>.
		''' @serialData
		''' <pre>
		'''  out.writeByte(5);        // identifies a JapaneseEra
		'''  out.writeInt(getValue());
		''' </pre>
		''' </summary>
		''' <returns> the instance of {@code Ser}, not null </returns>
		Private Function writeReplace() As Object
			Return New Ser(Ser.JAPANESE_ERA_TYPE, Me)
		End Function

		Friend Sub writeExternal(ByVal out As java.io.DataOutput)
			out.writeByte(Me.value)
		End Sub

		Shared Function readExternal(ByVal [in] As java.io.DataInput) As JapaneseEra
			Dim eraValue As SByte = [in].readByte()
			Return JapaneseEra.of(eraValue)
		End Function

	End Class

End Namespace
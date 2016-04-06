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
Namespace java.time.format


	''' <summary>
	''' Context object used during date and time parsing.
	''' <p>
	''' This class represents the current state of the parse.
	''' It has the ability to store and retrieve the parsed values and manage optional segments.
	''' It also provides key information to the parsing methods.
	''' <p>
	''' Once parsing is complete, the <seealso cref="#toUnresolved()"/> is used to obtain the unresolved
	''' result data. The <seealso cref="#toResolved()"/> is used to obtain the resolved result.
	''' 
	''' @implSpec
	''' This class is a mutable context intended for use from a single thread.
	''' Usage of the class is thread-safe within standard parsing as a new instance of this class
	''' is automatically created for each parse and parsing is single-threaded
	''' 
	''' @since 1.8
	''' </summary>
	Friend NotInheritable Class DateTimeParseContext

		''' <summary>
		''' The formatter, not null.
		''' </summary>
		Private formatter As DateTimeFormatter
		''' <summary>
		''' Whether to parse using case sensitively.
		''' </summary>
		Private caseSensitive As Boolean = True
		''' <summary>
		''' Whether to parse using strict rules.
		''' </summary>
		Private [strict] As Boolean = True
		''' <summary>
		''' The list of parsed data.
		''' </summary>
		Private ReadOnly parsed As New List(Of Parsed)
		''' <summary>
		''' List of Consumers<Chronology> to be notified if the Chronology changes.
		''' </summary>
		Private chronoListeners As List(Of java.util.function.Consumer(Of java.time.chrono.Chronology)) = Nothing

		''' <summary>
		''' Creates a new instance of the context.
		''' </summary>
		''' <param name="formatter">  the formatter controlling the parse, not null </param>
		Friend Sub New(  formatter As DateTimeFormatter)
			MyBase.New()
			Me.formatter = formatter
			parsed.Add(New Parsed)
		End Sub

		''' <summary>
		''' Creates a copy of this context.
		''' This retains the case sensitive and strict flags.
		''' </summary>
		Friend Function copy() As DateTimeParseContext
			Dim newContext As New DateTimeParseContext(formatter)
			newContext.caseSensitive = caseSensitive
			newContext.strict = [strict]
			Return newContext
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Gets the locale.
		''' <p>
		''' This locale is used to control localization in the parse except
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
		''' The DecimalStyle controls the numeric parsing.
		''' </summary>
		''' <returns> the DecimalStyle, not null </returns>
		Friend Property decimalStyle As DecimalStyle
			Get
				Return formatter.decimalStyle
			End Get
		End Property

		''' <summary>
		''' Gets the effective chronology during parsing.
		''' </summary>
		''' <returns> the effective parsing chronology, not null </returns>
		Friend Property effectiveChronology As java.time.chrono.Chronology
			Get
				Dim chrono As java.time.chrono.Chronology = currentParsed().chrono
				If chrono Is Nothing Then
					chrono = formatter.chronology
					If chrono Is Nothing Then chrono = java.time.chrono.IsoChronology.INSTANCE
				End If
				Return chrono
			End Get
		End Property

		'-----------------------------------------------------------------------
		''' <summary>
		''' Checks if parsing is case sensitive.
		''' </summary>
		''' <returns> true if parsing is case sensitive, false if case insensitive </returns>
		Friend Property caseSensitive As Boolean
			Get
				Return caseSensitive
			End Get
			Set(  caseSensitive As Boolean)
				Me.caseSensitive = caseSensitive
			End Set
		End Property


		'-----------------------------------------------------------------------
		''' <summary>
		''' Helper to compare two {@code CharSequence} instances.
		''' This uses <seealso cref="#isCaseSensitive()"/>.
		''' </summary>
		''' <param name="cs1">  the first character sequence, not null </param>
		''' <param name="offset1">  the offset into the first sequence, valid </param>
		''' <param name="cs2">  the second character sequence, not null </param>
		''' <param name="offset2">  the offset into the second sequence, valid </param>
		''' <param name="length">  the length to check, valid </param>
		''' <returns> true if equal </returns>
		Friend Function subSequenceEquals(  cs1 As CharSequence,   offset1 As Integer,   cs2 As CharSequence,   offset2 As Integer,   length As Integer) As Boolean
			If offset1 + length > cs1.length() OrElse offset2 + length > cs2.length() Then Return False
			If caseSensitive Then
				For i As Integer = 0 To length - 1
					Dim ch1 As Char = cs1.Chars(offset1 + i)
					Dim ch2 As Char = cs2.Chars(offset2 + i)
					If ch1 <> ch2 Then Return False
				Next i
			Else
				For i As Integer = 0 To length - 1
					Dim ch1 As Char = cs1.Chars(offset1 + i)
					Dim ch2 As Char = cs2.Chars(offset2 + i)
					If ch1 <> ch2 AndAlso Char.ToUpper(ch1) <> Char.ToUpper(ch2) AndAlso Char.ToLower(ch1) <> Char.ToLower(ch2) Then Return False
				Next i
			End If
			Return True
		End Function

		''' <summary>
		''' Helper to compare two {@code char}.
		''' This uses <seealso cref="#isCaseSensitive()"/>.
		''' </summary>
		''' <param name="ch1">  the first character </param>
		''' <param name="ch2">  the second character </param>
		''' <returns> true if equal </returns>
		Friend Function charEquals(  ch1 As Char,   ch2 As Char) As Boolean
			If caseSensitive Then Return ch1 = ch2
			Return charEqualsIgnoreCase(ch1, ch2)
		End Function

		''' <summary>
		''' Compares two characters ignoring case.
		''' </summary>
		''' <param name="c1">  the first </param>
		''' <param name="c2">  the second </param>
		''' <returns> true if equal </returns>
		Friend Shared Function charEqualsIgnoreCase(  c1 As Char,   c2 As Char) As Boolean
			Return c1 = c2 OrElse Char.ToUpper(c1) = Char.ToUpper(c2) OrElse Char.ToLower(c1) = Char.ToLower(c2)
		End Function

		'-----------------------------------------------------------------------
		''' <summary>
		''' Checks if parsing is strict.
		''' <p>
		''' Strict parsing requires exact matching of the text and sign styles.
		''' </summary>
		''' <returns> true if parsing is strict, false if lenient </returns>
		Friend Property [strict] As Boolean
			Get
				Return [strict]
			End Get
			Set(  [strict] As Boolean)
				Me.strict = [strict]
			End Set
		End Property


		'-----------------------------------------------------------------------
		''' <summary>
		''' Starts the parsing of an optional segment of the input.
		''' </summary>
		Friend Sub startOptional()
			parsed.Add(currentParsed().copy())
		End Sub

		''' <summary>
		''' Ends the parsing of an optional segment of the input.
		''' </summary>
		''' <param name="successful">  whether the optional segment was successfully parsed </param>
		Friend Sub endOptional(  successful As Boolean)
			If successful Then
				parsed.Remove(parsed.Count - 2)
			Else
				parsed.Remove(parsed.Count - 1)
			End If
		End Sub

		'-----------------------------------------------------------------------
		''' <summary>
		''' Gets the currently active temporal objects.
		''' </summary>
		''' <returns> the current temporal objects, not null </returns>
		Private Function currentParsed() As Parsed
			Return parsed(parsed.Count - 1)
		End Function

		''' <summary>
		''' Gets the unresolved result of the parse.
		''' </summary>
		''' <returns> the result of the parse, not null </returns>
		Friend Function toUnresolved() As Parsed
			Return currentParsed()
		End Function

		''' <summary>
		''' Gets the resolved result of the parse.
		''' </summary>
		''' <returns> the result of the parse, not null </returns>
		Friend Function toResolved(  resolverStyle As ResolverStyle,   resolverFields As java.util.Set(Of java.time.temporal.TemporalField)) As java.time.temporal.TemporalAccessor
			Dim parsed_Renamed As Parsed = currentParsed()
			parsed_Renamed.chrono = effectiveChronology
			parsed_Renamed.zone = (If(parsed_Renamed.zone IsNot Nothing, parsed_Renamed.zone, formatter.zone))
			Return parsed_Renamed.resolve(resolverStyle, resolverFields)
		End Function


		'-----------------------------------------------------------------------
		''' <summary>
		''' Gets the first value that was parsed for the specified field.
		''' <p>
		''' This searches the results of the parse, returning the first value found
		''' for the specified field. No attempt is made to derive a value.
		''' The field may have an out of range value.
		''' For example, the day-of-month might be set to 50, or the hour to 1000.
		''' </summary>
		''' <param name="field">  the field to query from the map, null returns null </param>
		''' <returns> the value mapped to the specified field, null if field was not parsed </returns>
		Friend Function getParsed(  field As java.time.temporal.TemporalField) As Long?
			Return currentParsed().fieldValues(field)
		End Function

		''' <summary>
		''' Stores the parsed field.
		''' <p>
		''' This stores a field-value pair that has been parsed.
		''' The value stored may be out of range for the field - no checks are performed.
		''' </summary>
		''' <param name="field">  the field to set in the field-value map, not null </param>
		''' <param name="value">  the value to set in the field-value map </param>
		''' <param name="errorPos">  the position of the field being parsed </param>
		''' <param name="successPos">  the position after the field being parsed </param>
		''' <returns> the new position </returns>
		Friend Function setParsedField(  field As java.time.temporal.TemporalField,   value As Long,   errorPos As Integer,   successPos As Integer) As Integer
			java.util.Objects.requireNonNull(field, "field")
				currentParsed().fieldValues(field) = value
				Dim old As Long? = currentParsed().fieldValues(field)
			Return If(old IsNot Nothing AndAlso old <> value, (Not errorPos), successPos)
		End Function

		''' <summary>
		''' Stores the parsed chronology.
		''' <p>
		''' This stores the chronology that has been parsed.
		''' No validation is performed other than ensuring it is not null.
		''' <p>
		''' The list of listeners is copied and cleared so that each
		''' listener is called only once.  A listener can add itself again
		''' if it needs to be notified of future changes.
		''' </summary>
		''' <param name="chrono">  the parsed chronology, not null </param>
		Friend Property parsed As java.time.chrono.Chronology
			Set(  chrono As java.time.chrono.Chronology)
				java.util.Objects.requireNonNull(chrono, "chrono")
				currentParsed().chrono = chrono
				If chronoListeners IsNot Nothing AndAlso chronoListeners.Count > 0 Then
	'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
					Dim tmp As java.util.function.Consumer(Of java.time.chrono.Chronology)() = New java.util.function.Consumer(0){}
					Dim listeners As java.util.function.Consumer(Of java.time.chrono.Chronology)() = chronoListeners.ToArray(tmp)
					chronoListeners.Clear()
					For Each l As java.util.function.Consumer(Of java.time.chrono.Chronology) In listeners
						l.accept(chrono)
					Next l
				End If
			End Set
		End Property

		''' <summary>
		''' Adds a Consumer<Chronology> to the list of listeners to be notified
		''' if the Chronology changes. </summary>
		''' <param name="listener"> a Consumer<Chronology> to be called when Chronology changes </param>
		Friend Sub addChronoChangedListener(  listener As java.util.function.Consumer(Of java.time.chrono.Chronology))
			If chronoListeners Is Nothing Then chronoListeners = New List(Of java.util.function.Consumer(Of java.time.chrono.Chronology))
			chronoListeners.Add(listener)
		End Sub

		''' <summary>
		''' Stores the parsed zone.
		''' <p>
		''' This stores the zone that has been parsed.
		''' No validation is performed other than ensuring it is not null.
		''' </summary>
		''' <param name="zone">  the parsed zone, not null </param>
		Friend Property parsed As java.time.ZoneId
			Set(  zone As java.time.ZoneId)
				java.util.Objects.requireNonNull(zone, "zone")
				currentParsed().zone = zone
			End Set
		End Property

		''' <summary>
		''' Stores the parsed leap second.
		''' </summary>
		Friend Sub setParsedLeapSecond()
			currentParsed().leapSecond = True
		End Sub

		'-----------------------------------------------------------------------
		''' <summary>
		''' Returns a string version of the context for debugging.
		''' </summary>
		''' <returns> a string representation of the context data, not null </returns>
		Public Overrides Function ToString() As String
			Return currentParsed().ToString()
		End Function

	End Class

End Namespace
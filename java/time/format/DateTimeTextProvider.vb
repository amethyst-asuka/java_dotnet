Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Collections.Concurrent

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
	''' A provider to obtain the textual form of a date-time field.
	''' 
	''' @implSpec
	''' Implementations must be thread-safe.
	''' Implementations should cache the textual information.
	''' 
	''' @since 1.8
	''' </summary>
	Friend Class DateTimeTextProvider

		''' <summary>
		''' Cache. </summary>
		Private Shared ReadOnly CACHE As java.util.concurrent.ConcurrentMap(Of KeyValuePair(Of java.time.temporal.TemporalField, java.util.Locale), Object) = New ConcurrentDictionary(Of KeyValuePair(Of java.time.temporal.TemporalField, java.util.Locale), Object)(16, 0.75f, 2)
		''' <summary>
		''' Comparator. </summary>
		Private Shared ReadOnly COMPARATOR As IComparer(Of KeyValuePair(Of String, Long?)) = New ComparatorAnonymousInnerClassHelper(Of T)

		Private Class ComparatorAnonymousInnerClassHelper(Of T)
			Implements IComparer(Of T)

			Public Overrides Function compare(  obj1 As KeyValuePair(Of String, Long?),   obj2 As KeyValuePair(Of String, Long?)) As Integer
				Return obj2.Key.length() - obj1.Key.length() ' longest to shortest
			End Function
		End Class

		Friend Sub New()
		End Sub

		''' <summary>
		''' Gets the provider of text.
		''' </summary>
		''' <returns> the provider, not null </returns>
		Shared instance As DateTimeTextProvider
			Get
				Return New DateTimeTextProvider
			End Get
		End Property

		''' <summary>
		''' Gets the text for the specified field, locale and style
		''' for the purpose of formatting.
		''' <p>
		''' The text associated with the value is returned.
		''' The null return value should be used if there is no applicable text, or
		''' if the text would be a numeric representation of the value.
		''' </summary>
		''' <param name="field">  the field to get text for, not null </param>
		''' <param name="value">  the field value to get text for, not null </param>
		''' <param name="style">  the style to get text for, not null </param>
		''' <param name="locale">  the locale to get text for, not null </param>
		''' <returns> the text for the field value, null if no text found </returns>
		Public Overridable Function getText(  field As java.time.temporal.TemporalField,   value As Long,   style As TextStyle,   locale As java.util.Locale) As String
			Dim store As Object = findStore(field, locale)
			If TypeOf store Is LocaleStore Then Return CType(store, LocaleStore).getText(value, style)
			Return Nothing
		End Function

		''' <summary>
		''' Gets the text for the specified chrono, field, locale and style
		''' for the purpose of formatting.
		''' <p>
		''' The text associated with the value is returned.
		''' The null return value should be used if there is no applicable text, or
		''' if the text would be a numeric representation of the value.
		''' </summary>
		''' <param name="chrono">  the Chronology to get text for, not null </param>
		''' <param name="field">  the field to get text for, not null </param>
		''' <param name="value">  the field value to get text for, not null </param>
		''' <param name="style">  the style to get text for, not null </param>
		''' <param name="locale">  the locale to get text for, not null </param>
		''' <returns> the text for the field value, null if no text found </returns>
		Public Overridable Function getText(  chrono As java.time.chrono.Chronology,   field As java.time.temporal.TemporalField,   value As Long,   style As TextStyle,   locale As java.util.Locale) As String
			If chrono Is java.time.chrono.IsoChronology.INSTANCE OrElse Not(TypeOf field Is java.time.temporal.ChronoField) Then Return getText(field, value, style, locale)

			Dim fieldIndex As Integer
			Dim fieldValue As Integer
			If field = ERA Then
				fieldIndex = DateTime.ERA
				If chrono Is java.time.chrono.JapaneseChronology.INSTANCE Then
					If value = -999 Then
						fieldValue = 0
					Else
						fieldValue = CInt(value) + 2
					End If
				Else
					fieldValue = CInt(value)
				End If
			ElseIf field = MONTH_OF_YEAR Then
				fieldIndex = DateTime.MONTH
				fieldValue = CInt(value) - 1
			ElseIf field = DAY_OF_WEEK Then
				fieldIndex = DateTime.DAY_OF_WEEK
				fieldValue = CInt(value) + 1
				If fieldValue > 7 Then fieldValue = DayOfWeek.Sunday
			ElseIf field = AMPM_OF_DAY Then
				fieldIndex = DateTime.AM_PM
				fieldValue = CInt(value)
			Else
				Return Nothing
			End If
			Return sun.util.locale.provider.CalendarDataUtility.retrieveJavaTimeFieldValueName(chrono.calendarType, fieldIndex, fieldValue, style.toCalendarStyle(), locale)
		End Function

		''' <summary>
		''' Gets an iterator of text to field for the specified field, locale and style
		''' for the purpose of parsing.
		''' <p>
		''' The iterator must be returned in order from the longest text to the shortest.
		''' <p>
		''' The null return value should be used if there is no applicable parsable text, or
		''' if the text would be a numeric representation of the value.
		''' Text can only be parsed if all the values for that field-style-locale combination are unique.
		''' </summary>
		''' <param name="field">  the field to get text for, not null </param>
		''' <param name="style">  the style to get text for, null for all parsable text </param>
		''' <param name="locale">  the locale to get text for, not null </param>
		''' <returns> the iterator of text to field pairs, in order from longest text to shortest text,
		'''  null if the field or style is not parsable </returns>
		Public Overridable Function getTextIterator(  field As java.time.temporal.TemporalField,   style As TextStyle,   locale As java.util.Locale) As IEnumerator(Of KeyValuePair(Of String, Long?))
			Dim store As Object = findStore(field, locale)
			If TypeOf store Is LocaleStore Then Return CType(store, LocaleStore).getTextIterator(style)
			Return Nothing
		End Function

		''' <summary>
		''' Gets an iterator of text to field for the specified chrono, field, locale and style
		''' for the purpose of parsing.
		''' <p>
		''' The iterator must be returned in order from the longest text to the shortest.
		''' <p>
		''' The null return value should be used if there is no applicable parsable text, or
		''' if the text would be a numeric representation of the value.
		''' Text can only be parsed if all the values for that field-style-locale combination are unique.
		''' </summary>
		''' <param name="chrono">  the Chronology to get text for, not null </param>
		''' <param name="field">  the field to get text for, not null </param>
		''' <param name="style">  the style to get text for, null for all parsable text </param>
		''' <param name="locale">  the locale to get text for, not null </param>
		''' <returns> the iterator of text to field pairs, in order from longest text to shortest text,
		'''  null if the field or style is not parsable </returns>
		Public Overridable Function getTextIterator(  chrono As java.time.chrono.Chronology,   field As java.time.temporal.TemporalField,   style As TextStyle,   locale As java.util.Locale) As IEnumerator(Of KeyValuePair(Of String, Long?))
			If chrono Is java.time.chrono.IsoChronology.INSTANCE OrElse Not(TypeOf field Is java.time.temporal.ChronoField) Then Return getTextIterator(field, style, locale)

			Dim fieldIndex As Integer
			Select Case CType(field, java.time.temporal.ChronoField)
			Case ERA
				fieldIndex = DateTime.ERA
			Case MONTH_OF_YEAR
				fieldIndex = DateTime.MONTH
			Case DAY_OF_WEEK
				fieldIndex = DateTime.DAY_OF_WEEK
			Case AMPM_OF_DAY
				fieldIndex = DateTime.AM_PM
			Case Else
				Return Nothing
			End Select

			Dim calendarStyle As Integer = If(style Is Nothing, DateTime.ALL_STYLES, style.toCalendarStyle())
			Dim map As IDictionary(Of String, Integer?) = sun.util.locale.provider.CalendarDataUtility.retrieveJavaTimeFieldValueNames(chrono.calendarType, fieldIndex, calendarStyle, locale)
			If map Is Nothing Then Return Nothing
			Dim list As IList(Of KeyValuePair(Of String, Long?)) = New List(Of KeyValuePair(Of String, Long?))(map.Count)
			Select Case fieldIndex
			Case DateTime.ERA
				For Each entry As KeyValuePair(Of String, Integer?) In map
					Dim era As Integer = entry.Value
					If chrono Is java.time.chrono.JapaneseChronology.INSTANCE Then
						If era = 0 Then
							era = -999
						Else
							era -= 2
						End If
					End If
					list.Add(createEntry(entry.Key, CLng(era)))
				Next entry
			Case DateTime.MONTH
				For Each entry As KeyValuePair(Of String, Integer?) In map
					list.Add(createEntry(entry.Key, CLng(Fix(entry.Value + 1))))
				Next entry
			Case DateTime.DAY_OF_WEEK
				For Each entry As KeyValuePair(Of String, Integer?) In map
					list.Add(createEntry(entry.Key, CLng(toWeekDay(entry.Value))))
				Next entry
			Case Else
				For Each entry As KeyValuePair(Of String, Integer?) In map
					list.Add(createEntry(entry.Key, CLng(Fix(entry.Value))))
				Next entry
			End Select
			Return list.GetEnumerator()
		End Function

		Private Function findStore(  field As java.time.temporal.TemporalField,   locale As java.util.Locale) As Object
			Dim key As KeyValuePair(Of java.time.temporal.TemporalField, java.util.Locale) = createEntry(field, locale)
			Dim store As Object = CACHE.get(key)
			If store Is Nothing Then
				store = createStore(field, locale)
				CACHE.putIfAbsent(key, store)
				store = CACHE.get(key)
			End If
			Return store
		End Function

		Private Shared Function toWeekDay(  calWeekDay As Integer) As Integer
			If calWeekDay = DayOfWeek.Sunday Then
				Return 7
			Else
				Return calWeekDay - 1
			End If
		End Function

		Private Function createStore(  field As java.time.temporal.TemporalField,   locale As java.util.Locale) As Object
			Dim styleMap As IDictionary(Of TextStyle, IDictionary(Of Long?, String)) = New Dictionary(Of TextStyle, IDictionary(Of Long?, String))
			If field = ERA Then
				For Each textStyle As TextStyle In System.Enum.GetValues(GetType(TextStyle))
					If textStyle.standalone Then Continue For
					Dim displayNames As IDictionary(Of String, Integer?) = sun.util.locale.provider.CalendarDataUtility.retrieveJavaTimeFieldValueNames("gregory", DateTime.ERA, textStyle.toCalendarStyle(), locale)
					If displayNames IsNot Nothing Then
						Dim map As IDictionary(Of Long?, String) = New Dictionary(Of Long?, String)
						For Each entry As KeyValuePair(Of String, Integer?) In displayNames
							map(CLng(Fix(entry.Value))) = entry.Key
						Next entry
						If map.Count > 0 Then styleMap(textStyle) = map
					End If
				Next textStyle
				Return New LocaleStore(styleMap)
			End If

			If field = MONTH_OF_YEAR Then
				For Each textStyle As TextStyle In System.Enum.GetValues(GetType(TextStyle))
					Dim displayNames As IDictionary(Of String, Integer?) = sun.util.locale.provider.CalendarDataUtility.retrieveJavaTimeFieldValueNames("gregory", DateTime.MONTH, textStyle.toCalendarStyle(), locale)
					Dim map As IDictionary(Of Long?, String) = New Dictionary(Of Long?, String)
					If displayNames IsNot Nothing Then
						For Each entry As KeyValuePair(Of String, Integer?) In displayNames
							map(CLng(Fix(entry.Value + 1))) = entry.Key
						Next entry

					Else
						' Narrow names may have duplicated names, such as "J" for January, Jun, July.
						' Get names one by one in that case.
						For month As Integer = 1 To 12
							Dim name As String
							name = sun.util.locale.provider.CalendarDataUtility.retrieveJavaTimeFieldValueName("gregory", DateTime.MONTH, month, textStyle.toCalendarStyle(), locale)
							If name Is Nothing Then Exit For
							map(CLng(month + 1)) = name
						Next month
					End If
					If map.Count > 0 Then styleMap(textStyle) = map
				Next textStyle
				Return New LocaleStore(styleMap)
			End If

			If field = DAY_OF_WEEK Then
				For Each textStyle As TextStyle In System.Enum.GetValues(GetType(TextStyle))
					Dim displayNames As IDictionary(Of String, Integer?) = sun.util.locale.provider.CalendarDataUtility.retrieveJavaTimeFieldValueNames("gregory", DateTime.DAY_OF_WEEK, textStyle.toCalendarStyle(), locale)
					Dim map As IDictionary(Of Long?, String) = New Dictionary(Of Long?, String)
					If displayNames IsNot Nothing Then
						For Each entry As KeyValuePair(Of String, Integer?) In displayNames
							map(CLng(toWeekDay(entry.Value))) = entry.Key
						Next entry

					Else
						' Narrow names may have duplicated names, such as "S" for Sunday and Saturday.
						' Get names one by one in that case.
						For wday As Integer = DayOfWeek.Sunday To DayOfWeek.Saturday
							Dim name As String
							name = sun.util.locale.provider.CalendarDataUtility.retrieveJavaTimeFieldValueName("gregory", DateTime.DAY_OF_WEEK, wday, textStyle.toCalendarStyle(), locale)
							If name Is Nothing Then Exit For
							map(CLng(toWeekDay(wday))) = name
						Next wday
					End If
					If map.Count > 0 Then styleMap(textStyle) = map
				Next textStyle
				Return New LocaleStore(styleMap)
			End If

			If field = AMPM_OF_DAY Then
				For Each textStyle As TextStyle In System.Enum.GetValues(GetType(TextStyle))
					If textStyle.standalone Then Continue For
					Dim displayNames As IDictionary(Of String, Integer?) = sun.util.locale.provider.CalendarDataUtility.retrieveJavaTimeFieldValueNames("gregory", DateTime.AM_PM, textStyle.toCalendarStyle(), locale)
					If displayNames IsNot Nothing Then
						Dim map As IDictionary(Of Long?, String) = New Dictionary(Of Long?, String)
						For Each entry As KeyValuePair(Of String, Integer?) In displayNames
							map(CLng(Fix(entry.Value))) = entry.Key
						Next entry
						If map.Count > 0 Then styleMap(textStyle) = map
					End If
				Next textStyle
				Return New LocaleStore(styleMap)
			End If

			If field = java.time.temporal.IsoFields.QUARTER_OF_YEAR Then
				' The order of keys must correspond to the TextStyle.values() order.
				Dim keys As String() = { "QuarterNames", "standalone.QuarterNames", "QuarterAbbreviations", "standalone.QuarterAbbreviations", "QuarterNarrows", "standalone.QuarterNarrows" }
				For i As Integer = 0 To keys.Length - 1
					Dim names As String() = getLocalizedResource(keys(i), locale)
					If names IsNot Nothing Then
						Dim map As IDictionary(Of Long?, String) = New Dictionary(Of Long?, String)
						For q As Integer = 0 To names.Length - 1
							map(CLng(q + 1)) = names(q)
						Next q
						styleMap(System.Enum.GetValues(GetType(TextStyle))(i)) = map
					End If
				Next i
				Return New LocaleStore(styleMap)
			End If

			Return "" ' null marker for map
		End Function

		''' <summary>
		''' Helper method to create an immutable entry.
		''' </summary>
		''' <param name="text">  the text, not null </param>
		''' <param name="field">  the field, not null </param>
		''' <returns> the entry, not null </returns>
		Private Shared Function createEntry(Of A, B)(  text As A,   field As B) As KeyValuePair(Of A, B)
			Return New java.util.AbstractMap.SimpleImmutableEntry(Of )(text, field)
		End Function

		''' <summary>
		''' Returns the localized resource of the given key and locale, or null
		''' if no localized resource is available.
		''' </summary>
		''' <param name="key">  the key of the localized resource, not null </param>
		''' <param name="locale">  the locale, not null </param>
		''' <returns> the localized resource, or null if not available </returns>
		''' <exception cref="NullPointerException"> if key or locale is null </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend Shared Function getLocalizedResource(Of T)(  key As String,   locale As java.util.Locale) As T
			Dim lr As sun.util.locale.provider.LocaleResources = sun.util.locale.provider.LocaleProviderAdapter.resourceBundleBased.getLocaleResources(locale)
			Dim rb As java.util.ResourceBundle = lr.javaTimeFormatData
			Return If(rb.containsKey(key), CType(rb.getObject(key), T), Nothing)
		End Function

		''' <summary>
		''' Stores the text for a single locale.
		''' <p>
		''' Some fields have a textual representation, such as day-of-week or month-of-year.
		''' These textual representations can be captured in this class for printing
		''' and parsing.
		''' <p>
		''' This class is immutable and thread-safe.
		''' </summary>
		Friend NotInheritable Class LocaleStore
			''' <summary>
			''' Map of value to text.
			''' </summary>
			Private ReadOnly valueTextMap As IDictionary(Of TextStyle, IDictionary(Of Long?, String))
			''' <summary>
			''' Parsable data.
			''' </summary>
			Private ReadOnly parsable As IDictionary(Of TextStyle, IList(Of KeyValuePair(Of String, Long?)))

			''' <summary>
			''' Constructor.
			''' </summary>
			''' <param name="valueTextMap">  the map of values to text to store, assigned and not altered, not null </param>
			Friend Sub New(  valueTextMap As IDictionary(Of TextStyle, IDictionary(Of Long?, String)))
				Me.valueTextMap = valueTextMap
				Dim map As IDictionary(Of TextStyle, IList(Of KeyValuePair(Of String, Long?))) = New Dictionary(Of TextStyle, IList(Of KeyValuePair(Of String, Long?)))
				Dim allList As IList(Of KeyValuePair(Of String, Long?)) = New List(Of KeyValuePair(Of String, Long?))
				For Each vtmEntry As KeyValuePair(Of TextStyle, IDictionary(Of Long?, String)) In valueTextMap
					Dim reverse As IDictionary(Of String, KeyValuePair(Of String, Long?)) = New Dictionary(Of String, KeyValuePair(Of String, Long?))
					For Each entry As KeyValuePair(Of Long?, String) In vtmEntry.Value.entrySet()
						If reverse.put(entry.Value, createEntry(entry.Value, entry.Key)) IsNot Nothing Then Continue For ' not parsable, try next style
					Next entry
					Dim list As IList(Of KeyValuePair(Of String, Long?)) = New List(Of KeyValuePair(Of String, Long?))(reverse.Values)
					java.util.Collections.sort(list, COMPARATOR)
					map(vtmEntry.Key) = list
					allList.AddRange(list)
					map(Nothing) = allList
				Next vtmEntry
				java.util.Collections.sort(allList, COMPARATOR)
				Me.parsable = map
			End Sub

			''' <summary>
			''' Gets the text for the specified field value, locale and style
			''' for the purpose of printing.
			''' </summary>
			''' <param name="value">  the value to get text for, not null </param>
			''' <param name="style">  the style to get text for, not null </param>
			''' <returns> the text for the field value, null if no text found </returns>
			Friend Function getText(  value As Long,   style As TextStyle) As String
				Dim map As IDictionary(Of Long?, String) = valueTextMap(style)
				Return If(map IsNot Nothing, map(value), Nothing)
			End Function

			''' <summary>
			''' Gets an iterator of text to field for the specified style for the purpose of parsing.
			''' <p>
			''' The iterator must be returned in order from the longest text to the shortest.
			''' </summary>
			''' <param name="style">  the style to get text for, null for all parsable text </param>
			''' <returns> the iterator of text to field pairs, in order from longest text to shortest text,
			'''  null if the style is not parsable </returns>
			Friend Function getTextIterator(  style As TextStyle) As IEnumerator(Of KeyValuePair(Of String, Long?))
				Dim list As IList(Of KeyValuePair(Of String, Long?)) = parsable(style)
				Return If(list IsNot Nothing, list.GetEnumerator(), Nothing)
			End Function
		End Class
	End Class

End Namespace
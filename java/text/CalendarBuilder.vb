Imports System

'
' * Copyright (c) 2010, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.text


	''' <summary>
	''' {@code CalendarBuilder} keeps field-value pairs for setting
	''' the calendar fields of the given {@code Calendar}. It has the
	''' <seealso cref="Calendar#FIELD_COUNT FIELD_COUNT"/>-th field for the week year
	''' support. Also {@code ISO_DAY_OF_WEEK} is used to specify
	''' {@code DAY_OF_WEEK} in the ISO day of week numbering.
	''' 
	''' <p>{@code CalendarBuilder} retains the semantic of the pseudo
	''' timestamp for fields. {@code CalendarBuilder} uses a single
	''' int array combining fields[] and stamp[] of {@code Calendar}.
	''' 
	''' @author Masayoshi Okutsu
	''' </summary>
	Friend Class CalendarBuilder
	'    
	'     * Pseudo time stamp constants used in java.util.Calendar
	'     
		Private Const UNSET As Integer = 0
		Private Const COMPUTED As Integer = 1
		Private Const MINIMUM_USER_STAMP As Integer = 2

		Private Shared ReadOnly MAX_FIELD As Integer = FIELD_COUNT + 1

		Public Shared ReadOnly WEEK_YEAR As Integer = FIELD_COUNT
		Public Const ISO_DAY_OF_WEEK As Integer = 1000 ' pseudo field index

		' stamp[] (lower half) and field[] (upper half) combined
		Private ReadOnly field As Integer()
		Private nextStamp As Integer
		Private maxFieldIndex As Integer

		Friend Sub New()
			field = New Integer(MAX_FIELD * 2 - 1){}
			nextStamp = MINIMUM_USER_STAMP
			maxFieldIndex = -1
		End Sub

		Friend Overridable Function [set](ByVal index As Integer, ByVal value As Integer) As CalendarBuilder
			If index = ISO_DAY_OF_WEEK Then
				index = DAY_OF_WEEK
				value = toCalendarDayOfWeek(value)
			End If
			field(index) = nextStamp
			nextStamp += 1
			field(MAX_FIELD + index) = value
			If index > maxFieldIndex AndAlso index < FIELD_COUNT Then maxFieldIndex = index
			Return Me
		End Function

		Friend Overridable Function addYear(ByVal value As Integer) As CalendarBuilder
			field(MAX_FIELD + YEAR) += value
			field(MAX_FIELD + WEEK_YEAR) += value
			Return Me
		End Function

		Friend Overridable Function isSet(ByVal index As Integer) As Boolean
			If index = ISO_DAY_OF_WEEK Then index = DAY_OF_WEEK
			Return field(index) > UNSET
		End Function

		Friend Overridable Function clear(ByVal index As Integer) As CalendarBuilder
			If index = ISO_DAY_OF_WEEK Then index = DAY_OF_WEEK
			field(index) = UNSET
			field(MAX_FIELD + index) = 0
			Return Me
		End Function

		Friend Overridable Function establish(ByVal cal As DateTime?) As DateTime?
			Dim weekDate As Boolean = isSet(WEEK_YEAR) AndAlso field(WEEK_YEAR) > field(YEAR)
			If weekDate AndAlso (Not cal.Value.weekDateSupported) Then
				' Use YEAR instead
				If Not isSet(YEAR) Then [set](YEAR, field(MAX_FIELD + WEEK_YEAR))
				weekDate = False
			End If

			cal.Value.clear()
			' Set the fields from the min stamp to the max stamp so that
			' the field resolution works in the Calendar.
			For stamp As Integer = MINIMUM_USER_STAMP To nextStamp - 1
				For index As Integer = 0 To maxFieldIndex
					If field(index) = stamp Then
						cal.Value.set(index, field(MAX_FIELD + index))
						Exit For
					End If
				Next index
			Next stamp

			If weekDate Then
				Dim weekOfYear As Integer = If(isSet(WEEK_OF_YEAR), field(MAX_FIELD + WEEK_OF_YEAR), 1)
				Dim dayOfWeek As Integer = If(isSet(DAY_OF_WEEK), field(MAX_FIELD + DAY_OF_WEEK), cal.Value.firstDayOfWeek)
				If (Not isValidDayOfWeek(dayOfWeek)) AndAlso cal.Value.lenient Then
					If dayOfWeek >= 8 Then
						dayOfWeek -= 1
						weekOfYear += dayOfWeek \ 7
						dayOfWeek = (dayOfWeek Mod 7) + 1
					Else
						Do While dayOfWeek <= 0
							dayOfWeek += 7
							weekOfYear -= 1
						Loop
					End If
					dayOfWeek = toCalendarDayOfWeek(dayOfWeek)
				End If
				cal.Value.weekDateate(field(MAX_FIELD + WEEK_YEAR), weekOfYear, dayOfWeek)
			End If
			Return cal
		End Function

		Public Overrides Function ToString() As String
			Dim sb As New StringBuilder
			sb.append("CalendarBuilder:[")
			For i As Integer = 0 To field.Length - 1
				If isSet(i) Then sb.append(i).append("="c).append(field(MAX_FIELD + i)).append(","c)
			Next i
			Dim lastIndex As Integer = sb.length() - 1
			If sb.Chars(lastIndex) Is ","c Then sb.length = lastIndex
			sb.append("]"c)
			Return sb.ToString()
		End Function

		Friend Shared Function toISODayOfWeek(ByVal calendarDayOfWeek As Integer) As Integer
			Return If(calendarDayOfWeek = SUNDAY, 7, calendarDayOfWeek - 1)
		End Function

		Friend Shared Function toCalendarDayOfWeek(ByVal isoDayOfWeek As Integer) As Integer
			If Not isValidDayOfWeek(isoDayOfWeek) Then Return isoDayOfWeek
			Return If(isoDayOfWeek = 7, SUNDAY, isoDayOfWeek + 1)
		End Function

		Friend Shared Function isValidDayOfWeek(ByVal dayOfWeek As Integer) As Boolean
			Return dayOfWeek > 0 AndAlso dayOfWeek <= 7
		End Function
	End Class

End Namespace
Imports System
Imports System.Collections
Imports javax.swing
Imports javax.swing.text

'
' * Copyright (c) 2000, 2013, Oracle and/or its affiliates. All rights reserved.
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
Namespace javax.swing.text

	''' <summary>
	''' DateFormatter is an <code>InternationalFormatter</code> that does its
	''' formatting by way of an instance of <code>java.text.DateFormat</code>.
	''' <p>
	''' <strong>Warning:</strong>
	''' Serialized objects of this class will not be compatible with
	''' future Swing releases. The current serialization support is
	''' appropriate for short term storage or RMI between applications running
	''' the same version of Swing.  As of 1.4, support for long term storage
	''' of all JavaBeans&trade;
	''' has been added to the <code>java.beans</code> package.
	''' Please see <seealso cref="java.beans.XMLEncoder"/>.
	''' </summary>
	''' <seealso cref= java.text.DateFormat
	''' 
	''' @since 1.4 </seealso>
	Public Class DateFormatter
		Inherits InternationalFormatter

		''' <summary>
		''' This is shorthand for
		''' <code>new DateFormatter(DateFormat.getDateInstance())</code>.
		''' </summary>
		Public Sub New()
			Me.New(DateFormat.dateInstance)
		End Sub

		''' <summary>
		''' Returns a DateFormatter configured with the specified
		''' <code>Format</code> instance.
		''' </summary>
		''' <param name="format"> Format used to dictate legal values </param>
		Public Sub New(ByVal format As DateFormat)
			MyBase.New(format)
			format = format
		End Sub

		''' <summary>
		''' Sets the format that dictates the legal values that can be edited
		''' and displayed.
		''' <p>
		''' If you have used the nullary constructor the value of this property
		''' will be determined for the current locale by way of the
		''' <code>Dateformat.getDateInstance()</code> method.
		''' </summary>
		''' <param name="format"> DateFormat instance used for converting from/to Strings </param>
		Public Overridable Property format As DateFormat
			Set(ByVal format As DateFormat)
				MyBase.format = format
			End Set
		End Property

		''' <summary>
		''' Returns the Calendar that <code>DateFormat</code> is associated with,
		''' or if the <code>Format</code> is not a <code>DateFormat</code>
		''' <code>Calendar.getInstance</code> is returned.
		''' </summary>
		Private Property calendar As DateTime
			Get
				Dim f As Format = format
    
				If TypeOf f Is DateFormat Then Return CType(f, DateFormat).calendar
				Return New DateTime
			End Get
		End Property


		''' <summary>
		''' Returns true, as DateFormatterFilter will support
		''' incrementing/decrementing of the value.
		''' </summary>
		Friend Property Overrides supportsIncrement As Boolean
			Get
				Return True
			End Get
		End Property

		''' <summary>
		''' Returns the field that will be adjusted by adjustValue.
		''' </summary>
		Friend Overrides Function getAdjustField(ByVal start As Integer, ByVal attributes As IDictionary) As Object
			Dim attrs As IEnumerator = attributes.Keys.GetEnumerator()

			Do While attrs.hasNext()
				Dim key As Object = attrs.next()

				If (TypeOf key Is DateFormat.Field) AndAlso (key Is DateFormat.Field.HOUR1 OrElse CType(key, DateFormat.Field).calendarField <> -1) Then Return key
			Loop
			Return Nothing
		End Function

		''' <summary>
		''' Adjusts the Date if FieldPosition identifies a known calendar
		''' field.
		''' </summary>
		Friend Overrides Function adjustValue(ByVal value As Object, ByVal attributes As IDictionary, ByVal key As Object, ByVal direction As Integer) As Object
			If key IsNot Nothing Then
				Dim field As Integer

				' HOUR1 has no corresponding calendar field, thus, map
				' it to HOUR0 which will give the correct behavior.
				If key Is DateFormat.Field.HOUR1 Then key = DateFormat.Field.HOUR0
				field = CType(key, DateFormat.Field).calendarField

				Dim ___calendar As DateTime = calendar

				If ___calendar IsNot Nothing Then
					___calendar = CDate(value)

					Dim fieldValue As Integer = ___calendar.get(field)

					Try
						___calendar.add(field, direction)
						value = ___calendar
					Catch th As Exception
						value = Nothing
					End Try
					Return value
				End If
			End If
			Return Nothing
		End Function
	End Class

End Namespace
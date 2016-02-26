Imports System
Imports System.Collections
Imports System.Text

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
	''' <code>NumberFormatter</code> subclasses <code>InternationalFormatter</code>
	''' adding special behavior for numbers. Among the specializations are
	''' (these are only used if the <code>NumberFormatter</code> does not display
	''' invalid numbers, for example, <code>setAllowsInvalid(false)</code>):
	''' <ul>
	'''   <li>Pressing +/- (- is determined from the
	'''       <code>DecimalFormatSymbols</code> associated with the
	'''       <code>DecimalFormat</code>) in any field but the exponent
	'''       field will attempt to change the sign of the number to
	'''       positive/negative.
	'''   <li>Pressing +/- (- is determined from the
	'''       <code>DecimalFormatSymbols</code> associated with the
	'''       <code>DecimalFormat</code>) in the exponent field will
	'''       attempt to change the sign of the exponent to positive/negative.
	''' </ul>
	''' <p>
	''' If you are displaying scientific numbers, you may wish to turn on
	''' overwrite mode, <code>setOverwriteMode(true)</code>. For example:
	''' <pre>
	''' DecimalFormat decimalFormat = new DecimalFormat("0.000E0");
	''' NumberFormatter textFormatter = new NumberFormatter(decimalFormat);
	''' textFormatter.setOverwriteMode(true);
	''' textFormatter.setAllowsInvalid(false);
	''' </pre>
	''' <p>
	''' If you are going to allow the user to enter decimal
	''' values, you should either force the DecimalFormat to contain at least
	''' one decimal (<code>#.0###</code>), or allow the value to be invalid
	''' <code>setAllowsInvalid(true)</code>. Otherwise users may not be able to
	''' input decimal values.
	''' <p>
	''' <code>NumberFormatter</code> provides slightly different behavior to
	''' <code>stringToValue</code> than that of its superclass. If you have
	''' specified a Class for values, <seealso cref="#setValueClass"/>, that is one of
	''' of <code>Integer</code>, <code>Long</code>, <code>Float</code>,
	''' <code>Double</code>, <code>Byte</code> or <code>Short</code> and
	''' the Format's <code>parseObject</code> returns an instance of
	''' <code>Number</code>, the corresponding instance of the value class
	''' will be created using the constructor appropriate for the primitive
	''' type the value class represents. For example:
	''' <code>setValueClass(Integer.class)</code> will cause the resulting
	''' value to be created via
	''' <code>new Integer(((Number)formatter.parseObject(string)).intValue())</code>.
	''' This is typically useful if you
	''' wish to set a min/max value as the various <code>Number</code>
	''' implementations are generally not comparable to each other. This is also
	''' useful if for some reason you need a specific <code>Number</code>
	''' implementation for your values.
	''' <p>
	''' <strong>Warning:</strong>
	''' Serialized objects of this class will not be compatible with
	''' future Swing releases. The current serialization support is
	''' appropriate for short term storage or RMI between applications running
	''' the same version of Swing.  As of 1.4, support for long term storage
	''' of all JavaBeans&trade;
	''' has been added to the <code>java.beans</code> package.
	''' Please see <seealso cref="java.beans.XMLEncoder"/>.
	''' 
	''' @since 1.4
	''' </summary>
	Public Class NumberFormatter
		Inherits InternationalFormatter

		''' <summary>
		''' The special characters from the Format instance. </summary>
		Private specialChars As String

		''' <summary>
		''' Creates a <code>NumberFormatter</code> with the a default
		''' <code>NumberFormat</code> instance obtained from
		''' <code>NumberFormat.getNumberInstance()</code>.
		''' </summary>
		Public Sub New()
			Me.New(NumberFormat.numberInstance)
		End Sub

		''' <summary>
		''' Creates a NumberFormatter with the specified Format instance.
		''' </summary>
		''' <param name="format"> Format used to dictate legal values </param>
		Public Sub New(ByVal format As NumberFormat)
			MyBase.New(format)
			format = format
			allowsInvalid = True
			commitsOnValidEdit = False
			overwriteMode = False
		End Sub

		''' <summary>
		''' Sets the format that dictates the legal values that can be edited
		''' and displayed.
		''' <p>
		''' If you have used the nullary constructor the value of this property
		''' will be determined for the current locale by way of the
		''' <code>NumberFormat.getNumberInstance()</code> method.
		''' </summary>
		''' <param name="format"> NumberFormat instance used to dictate legal values </param>
		Public Overrides Property format As Format
			Set(ByVal format As Format)
				MyBase.format = format
    
				Dim dfs As DecimalFormatSymbols = decimalFormatSymbols
    
				If dfs IsNot Nothing Then
					Dim sb As New StringBuilder
    
					sb.Append(dfs.currencySymbol)
					sb.Append(dfs.decimalSeparator)
					sb.Append(dfs.groupingSeparator)
					sb.Append(dfs.infinity)
					sb.Append(dfs.internationalCurrencySymbol)
					sb.Append(dfs.minusSign)
					sb.Append(dfs.monetaryDecimalSeparator)
					sb.Append(dfs.naN)
					sb.Append(dfs.percent)
					sb.Append("+"c)
					specialChars = sb.ToString()
				Else
					specialChars = ""
				End If
			End Set
		End Property

		''' <summary>
		''' Invokes <code>parseObject</code> on <code>f</code>, returning
		''' its value.
		''' </summary>
		Friend Overrides Function stringToValue(ByVal text As String, ByVal f As Format) As Object
			If f Is Nothing Then Return text
			Dim value As Object = f.parseObject(text)

			Return convertValueToValueClass(value, valueClass)
		End Function

		''' <summary>
		''' Converts the passed in value to the passed in class. This only
		''' works if <code>valueClass</code> is one of <code>Integer</code>,
		''' <code>Long</code>, <code>Float</code>, <code>Double</code>,
		''' <code>Byte</code> or <code>Short</code> and <code>value</code>
		''' is an instanceof <code>Number</code>.
		''' </summary>
		Private Function convertValueToValueClass(ByVal value As Object, ByVal valueClass As Type) As Object
			If valueClass IsNot Nothing AndAlso (TypeOf value Is Number) Then
				Dim numberValue As Number = CType(value, Number)
				If valueClass Is GetType(Integer) Then
					Return Convert.ToInt32(numberValue)
				ElseIf valueClass Is GetType(Long) Then
					Return Convert.ToInt64(numberValue)
				ElseIf valueClass Is GetType(Single?) Then
					Return Convert.ToSingle(numberValue)
				ElseIf valueClass Is GetType(Double) Then
					Return Convert.ToDouble(numberValue)
				ElseIf valueClass Is GetType(SByte?) Then
					Return Convert.ToByte(numberValue)
				ElseIf valueClass Is GetType(Short) Then
					Return Convert.ToInt16(numberValue)
				End If
			End If
			Return value
		End Function

		''' <summary>
		''' Returns the character that is used to toggle to positive values.
		''' </summary>
		Private Property positiveSign As Char
			Get
				Return "+"c
			End Get
		End Property

		''' <summary>
		''' Returns the character that is used to toggle to negative values.
		''' </summary>
		Private Property minusSign As Char
			Get
				Dim dfs As DecimalFormatSymbols = decimalFormatSymbols
    
				If dfs IsNot Nothing Then Return dfs.minusSign
				Return "-"c
			End Get
		End Property

		''' <summary>
		''' Returns the character that is used to toggle to negative values.
		''' </summary>
		Private Property decimalSeparator As Char
			Get
				Dim dfs As DecimalFormatSymbols = decimalFormatSymbols
    
				If dfs IsNot Nothing Then Return dfs.decimalSeparator
				Return "."c
			End Get
		End Property

		''' <summary>
		''' Returns the DecimalFormatSymbols from the Format instance.
		''' </summary>
		Private Property decimalFormatSymbols As DecimalFormatSymbols
			Get
				Dim f As Format = format
    
				If TypeOf f Is DecimalFormat Then Return CType(f, DecimalFormat).decimalFormatSymbols
				Return Nothing
			End Get
		End Property

		''' <summary>
		''' Subclassed to return false if <code>text</code> contains in an invalid
		''' character to insert, that is, it is not a digit
		''' (<code>Character.isDigit()</code>) and
		''' not one of the characters defined by the DecimalFormatSymbols.
		''' </summary>
		Friend Overrides Function isLegalInsertText(ByVal text As String) As Boolean
			If allowsInvalid Then Return True
			For counter As Integer = text.Length - 1 To 0 Step -1
				Dim aChar As Char = text.Chars(counter)

				If (Not Char.IsDigit(aChar)) AndAlso specialChars.IndexOf(aChar) = -1 Then Return False
			Next counter
			Return True
		End Function

		''' <summary>
		''' Subclassed to treat the decimal separator, grouping separator,
		''' exponent symbol, percent, permille, currency and sign as literals.
		''' </summary>
		Friend Overrides Function isLiteral(ByVal attrs As IDictionary) As Boolean
			If Not MyBase.isLiteral(attrs) Then
				If attrs Is Nothing Then Return False
				Dim size As Integer = attrs.Count

				If attrs(NumberFormat.Field.GROUPING_SEPARATOR) IsNot Nothing Then
					size -= 1
					If attrs(NumberFormat.Field.INTEGER) IsNot Nothing Then size -= 1
				End If
				If attrs(NumberFormat.Field.EXPONENT_SYMBOL) IsNot Nothing Then size -= 1
				If attrs(NumberFormat.Field.PERCENT) IsNot Nothing Then size -= 1
				If attrs(NumberFormat.Field.PERMILLE) IsNot Nothing Then size -= 1
				If attrs(NumberFormat.Field.CURRENCY) IsNot Nothing Then size -= 1
				If attrs(NumberFormat.Field.SIGN) IsNot Nothing Then size -= 1
				Return size = 0
			End If
			Return True
		End Function

		''' <summary>
		''' Subclassed to make the decimal separator navigable, as well
		''' as making the character between the integer field and the next
		''' field navigable.
		''' </summary>
		Friend Overrides Function isNavigatable(ByVal index As Integer) As Boolean
			If Not MyBase.isNavigatable(index) Then Return getBufferedChar(index) = decimalSeparator
			Return True
		End Function

		''' <summary>
		''' Returns the first <code>NumberFormat.Field</code> starting
		''' <code>index</code> incrementing by <code>direction</code>.
		''' </summary>
		Private Function getFieldFrom(ByVal index As Integer, ByVal direction As Integer) As NumberFormat.Field
			If validMask Then
				Dim max As Integer = formattedTextField.document.length
				Dim ___iterator As AttributedCharacterIterator = [iterator]

				If index >= max Then index += direction
				Do While index >= 0 AndAlso index < max
					___iterator.index = index

					Dim attrs As IDictionary = ___iterator.attributes

					If attrs IsNot Nothing AndAlso attrs.Count > 0 Then
						For Each key As Object In attrs.Keys
							If TypeOf key Is NumberFormat.Field Then Return CType(key, NumberFormat.Field)
						Next key
					End If
					index += direction
				Loop
			End If
			Return Nothing
		End Function

		''' <summary>
		''' Overriden to toggle the value if the positive/minus sign
		''' is inserted.
		''' </summary>
		Friend Overrides Sub replace(ByVal fb As DocumentFilter.FilterBypass, ByVal offset As Integer, ByVal length As Integer, ByVal [string] As String, ByVal attr As AttributeSet)
			If (Not allowsInvalid) AndAlso length = 0 AndAlso [string] IsNot Nothing AndAlso [string].Length = 1 AndAlso toggleSignIfNecessary(fb, offset, [string].Chars(0)) Then Return
			MyBase.replace(fb, offset, length, [string], attr)
		End Sub

		''' <summary>
		''' Will change the sign of the integer or exponent field if
		''' <code>aChar</code> is the positive or minus sign. Returns
		''' true if a sign change was attempted.
		''' </summary>
		Private Function toggleSignIfNecessary(ByVal fb As DocumentFilter.FilterBypass, ByVal offset As Integer, ByVal aChar As Char) As Boolean
			If aChar = minusSign OrElse aChar = positiveSign Then
				Dim field As NumberFormat.Field = getFieldFrom(offset, -1)
				Dim newValue As Object

				Try
					If field Is Nothing OrElse (field IsNot NumberFormat.Field.EXPONENT AndAlso field IsNot NumberFormat.Field.EXPONENT_SYMBOL AndAlso field IsNot NumberFormat.Field.EXPONENT_SIGN) Then
						newValue = toggleSign((aChar = positiveSign))
					Else
						' exponent
						newValue = toggleExponentSign(offset, aChar)
					End If
					If newValue IsNot Nothing AndAlso isValidValue(newValue, False) Then
						Dim lc As Integer = getLiteralCountTo(offset)
						Dim [string] As String = valueToString(newValue)

						fb.remove(0, fb.document.length)
						fb.insertString(0, [string], Nothing)
						updateValue(newValue)
						repositionCursor(getLiteralCountTo(offset) - lc + offset, 1)
						Return True
					End If
				Catch pe As ParseException
					invalidEdit()
				End Try
			End If
			Return False
		End Function

		''' <summary>
		''' Invoked to toggle the sign. For this to work the value class
		''' must have a single arg constructor that takes a String.
		''' </summary>
		Private Function toggleSign(ByVal positive As Boolean) As Object
			Dim value As Object = stringToValue(formattedTextField.text)

			If value IsNot Nothing Then
				' toString isn't localized, so that using +/- should work
				' correctly.
				Dim [string] As String = value.ToString()

				If [string] IsNot Nothing AndAlso [string].Length > 0 Then
					If positive Then
						If [string].Chars(0) = "-"c Then [string] = [string].Substring(1)
					Else
						If [string].Chars(0) = "+"c Then [string] = [string].Substring(1)
						If [string].Length > 0 AndAlso [string].Chars(0) <> "-"c Then [string] = "-" & [string]
					End If
					If [string] IsNot Nothing Then
						Dim ___valueClass As Type = valueClass

						If ___valueClass Is Nothing Then ___valueClass = value.GetType()
						Try
							sun.reflect.misc.ReflectUtil.checkPackageAccess(___valueClass)
							sun.swing.SwingUtilities2.checkAccess(___valueClass.modifiers)
							Dim cons As Constructor = ___valueClass.GetConstructor(New Type() { GetType(String) })
							If cons IsNot Nothing Then
								sun.swing.SwingUtilities2.checkAccess(cons.modifiers)
								Return cons.newInstance(New Object(){[string]})
							End If
						Catch ex As Exception
						End Try
					End If
				End If
			End If
			Return Nothing
		End Function

		''' <summary>
		''' Invoked to toggle the sign of the exponent (for scientific
		''' numbers).
		''' </summary>
		Private Function toggleExponentSign(ByVal offset As Integer, ByVal aChar As Char) As Object
			Dim [string] As String = formattedTextField.text
			Dim replaceLength As Integer = 0
			Dim loc As Integer = getAttributeStart(NumberFormat.Field.EXPONENT_SIGN)

			If loc >= 0 Then
				replaceLength = 1
				offset = loc
			End If
			If aChar = positiveSign Then
				[string] = getReplaceString(offset, replaceLength, Nothing)
			Else
				[string] = getReplaceString(offset, replaceLength, New String(New Char() { aChar }))
			End If
			Return stringToValue([string])
		End Function
	End Class

End Namespace
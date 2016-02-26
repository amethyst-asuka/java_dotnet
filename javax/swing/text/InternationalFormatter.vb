Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports javax.swing

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
	''' <code>InternationalFormatter</code> extends <code>DefaultFormatter</code>,
	''' using an instance of <code>java.text.Format</code> to handle the
	''' conversion to a String, and the conversion from a String.
	''' <p>
	''' If <code>getAllowsInvalid()</code> is false, this will ask the
	''' <code>Format</code> to format the current text on every edit.
	''' <p>
	''' You can specify a minimum and maximum value by way of the
	''' <code>setMinimum</code> and <code>setMaximum</code> methods. In order
	''' for this to work the values returned from <code>stringToValue</code> must be
	''' comparable to the min/max values by way of the <code>Comparable</code>
	''' interface.
	''' <p>
	''' Be careful how you configure the <code>Format</code> and the
	''' <code>InternationalFormatter</code>, as it is possible to create a
	''' situation where certain values can not be input. Consider the date
	''' format 'M/d/yy', an <code>InternationalFormatter</code> that is always
	''' valid (<code>setAllowsInvalid(false)</code>), is in overwrite mode
	''' (<code>setOverwriteMode(true)</code>) and the date 7/1/99. In this
	''' case the user will not be able to enter a two digit month or day of
	''' month. To avoid this, the format should be 'MM/dd/yy'.
	''' <p>
	''' If <code>InternationalFormatter</code> is configured to only allow valid
	''' values (<code>setAllowsInvalid(false)</code>), every valid edit will result
	''' in the text of the <code>JFormattedTextField</code> being completely reset
	''' from the <code>Format</code>.
	''' The cursor position will also be adjusted as literal characters are
	''' added/removed from the resulting String.
	''' <p>
	''' <code>InternationalFormatter</code>'s behavior of
	''' <code>stringToValue</code> is  slightly different than that of
	''' <code>DefaultTextFormatter</code>, it does the following:
	''' <ol>
	'''   <li><code>parseObject</code> is invoked on the <code>Format</code>
	'''       specified by <code>setFormat</code>
	'''   <li>If a Class has been set for the values (<code>setValueClass</code>),
	'''       supers implementation is invoked to convert the value returned
	'''       from <code>parseObject</code> to the appropriate class.
	'''   <li>If a <code>ParseException</code> has not been thrown, and the value
	'''       is outside the min/max a <code>ParseException</code> is thrown.
	'''   <li>The value is returned.
	''' </ol>
	''' <code>InternationalFormatter</code> implements <code>stringToValue</code>
	''' in this manner so that you can specify an alternate Class than
	''' <code>Format</code> may return.
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
	''' <seealso cref= java.text.Format </seealso>
	''' <seealso cref= java.lang.Comparable
	''' 
	''' @since 1.4 </seealso>
	Public Class InternationalFormatter
		Inherits DefaultFormatter

		''' <summary>
		''' Used by <code>getFields</code>.
		''' </summary>
		Private Shared ReadOnly EMPTY_FIELD_ARRAY As Format.Field() = New Format.Field(){}

		''' <summary>
		''' Object used to handle the conversion.
		''' </summary>
		Private format As Format
		''' <summary>
		''' Can be used to impose a maximum value.
		''' </summary>
		Private max As IComparable
		''' <summary>
		''' Can be used to impose a minimum value.
		''' </summary>
		Private min As IComparable

		''' <summary>
		''' <code>InternationalFormatter</code>'s behavior is dicatated by a
		''' <code>AttributedCharacterIterator</code> that is obtained from
		''' the <code>Format</code>. On every edit, assuming
		''' allows invalid is false, the <code>Format</code> instance is invoked
		''' with <code>formatToCharacterIterator</code>. A <code>BitSet</code> is
		''' also kept upto date with the non-literal characters, that is
		''' for every index in the <code>AttributedCharacterIterator</code> an
		''' entry in the bit set is updated based on the return value from
		''' <code>isLiteral(Map)</code>. <code>isLiteral(int)</code> then uses
		''' this cached information.
		''' <p>
		''' If allowsInvalid is false, every edit results in resetting the complete
		''' text of the JTextComponent.
		''' <p>
		''' InternationalFormatterFilter can also provide two actions suitable for
		''' incrementing and decrementing. To enable this a subclass must
		''' override <code>getSupportsIncrement</code> to return true, and
		''' override <code>adjustValue</code> to handle the changing of the
		''' value. If you want to support changing the value outside of
		''' the valid FieldPositions, you will need to override
		''' <code>canIncrement</code>.
		''' </summary>
		''' <summary>
		''' A bit is set for every index identified in the
		''' AttributedCharacterIterator that is not considered decoration.
		''' This should only be used if validMask is true.
		''' </summary>
		<NonSerialized> _
		Private literalMask As BitArray
		''' <summary>
		''' Used to iterate over characters.
		''' </summary>
		<NonSerialized> _
		Private [iterator] As AttributedCharacterIterator
		''' <summary>
		''' True if the Format was able to convert the value to a String and
		''' back.
		''' </summary>
		<NonSerialized> _
		Private validMask As Boolean
		''' <summary>
		''' Current value being displayed.
		''' </summary>
		<NonSerialized> _
		Private [string] As String
		''' <summary>
		''' If true, DocumentFilter methods are unconditionally allowed,
		''' and no checking is done on their values. This is used when
		''' incrementing/decrementing via the actions.
		''' </summary>
		<NonSerialized> _
		Private ignoreDocumentMutate As Boolean


		''' <summary>
		''' Creates an <code>InternationalFormatter</code> with no
		''' <code>Format</code> specified.
		''' </summary>
		Public Sub New()
			overwriteMode = False
		End Sub

		''' <summary>
		''' Creates an <code>InternationalFormatter</code> with the specified
		''' <code>Format</code> instance.
		''' </summary>
		''' <param name="format"> Format instance used for converting from/to Strings </param>
		Public Sub New(ByVal format As Format)
			Me.New()
			format = format
		End Sub

		''' <summary>
		''' Sets the format that dictates the legal values that can be edited
		''' and displayed.
		''' </summary>
		''' <param name="format"> <code>Format</code> instance used for converting
		''' from/to Strings </param>
		Public Overridable Property format As Format
			Set(ByVal format As Format)
				Me.format = format
			End Set
			Get
				Return format
			End Get
		End Property


		''' <summary>
		''' Sets the minimum permissible value. If the <code>valueClass</code> has
		''' not been specified, and <code>minimum</code> is non null, the
		''' <code>valueClass</code> will be set to that of the class of
		''' <code>minimum</code>.
		''' </summary>
		''' <param name="minimum"> Minimum legal value that can be input </param>
		''' <seealso cref= #setValueClass </seealso>
		Public Overridable Property minimum As IComparable
			Set(ByVal minimum As IComparable)
				If valueClass Is Nothing AndAlso minimum IsNot Nothing Then valueClass = minimum.GetType()
				min = minimum
			End Set
			Get
				Return min
			End Get
		End Property


		''' <summary>
		''' Sets the maximum permissible value. If the <code>valueClass</code> has
		''' not been specified, and <code>max</code> is non null, the
		''' <code>valueClass</code> will be set to that of the class of
		''' <code>max</code>.
		''' </summary>
		''' <param name="max"> Maximum legal value that can be input </param>
		''' <seealso cref= #setValueClass </seealso>
		Public Overridable Property maximum As IComparable
			Set(ByVal max As IComparable)
				If valueClass Is Nothing AndAlso max IsNot Nothing Then valueClass = max.GetType()
				Me.max = max
			End Set
			Get
				Return max
			End Get
		End Property


		''' <summary>
		''' Installs the <code>DefaultFormatter</code> onto a particular
		''' <code>JFormattedTextField</code>.
		''' This will invoke <code>valueToString</code> to convert the
		''' current value from the <code>JFormattedTextField</code> to
		''' a String. This will then install the <code>Action</code>s from
		''' <code>getActions</code>, the <code>DocumentFilter</code>
		''' returned from <code>getDocumentFilter</code> and the
		''' <code>NavigationFilter</code> returned from
		''' <code>getNavigationFilter</code> onto the
		''' <code>JFormattedTextField</code>.
		''' <p>
		''' Subclasses will typically only need to override this if they
		''' wish to install additional listeners on the
		''' <code>JFormattedTextField</code>.
		''' <p>
		''' If there is a <code>ParseException</code> in converting the
		''' current value to a String, this will set the text to an empty
		''' String, and mark the <code>JFormattedTextField</code> as being
		''' in an invalid state.
		''' <p>
		''' While this is a public method, this is typically only useful
		''' for subclassers of <code>JFormattedTextField</code>.
		''' <code>JFormattedTextField</code> will invoke this method at
		''' the appropriate times when the value changes, or its internal
		''' state changes.
		''' </summary>
		''' <param name="ftf"> JFormattedTextField to format for, may be null indicating
		'''            uninstall from current JFormattedTextField. </param>
		Public Overrides Sub install(ByVal ftf As JFormattedTextField)
			MyBase.install(ftf)
			updateMaskIfNecessary()
			' invoked again as the mask should now be valid.
			positionCursorAtInitialLocation()
		End Sub

		''' <summary>
		''' Returns a String representation of the Object <code>value</code>.
		''' This invokes <code>format</code> on the current <code>Format</code>.
		''' </summary>
		''' <exception cref="ParseException"> if there is an error in the conversion </exception>
		''' <param name="value"> Value to convert </param>
		''' <returns> String representation of value </returns>
		Public Overrides Function valueToString(ByVal value As Object) As String
			If value Is Nothing Then Return ""
			Dim f As Format = format

			If f Is Nothing Then Return value.ToString()
			Return f.format(value)
		End Function

		''' <summary>
		''' Returns the <code>Object</code> representation of the
		''' <code>String</code> <code>text</code>.
		''' </summary>
		''' <param name="text"> <code>String</code> to convert </param>
		''' <returns> <code>Object</code> representation of text </returns>
		''' <exception cref="ParseException"> if there is an error in the conversion </exception>
		Public Overrides Function stringToValue(ByVal text As String) As Object
			Dim value As Object = stringToValue(text, format)

			' Convert to the value class if the Value returned from the
			' Format does not match.
			If value IsNot Nothing AndAlso valueClass IsNot Nothing AndAlso (Not valueClass.IsInstanceOfType(value)) Then value = MyBase.stringToValue(value.ToString())
			Try
				If Not isValidValue(value, True) Then Throw New ParseException("Value not within min/max range", 0)
			Catch cce As ClassCastException
				Throw New ParseException("Class cast exception comparing values: " & cce, 0)
			End Try
			Return value
		End Function

		''' <summary>
		''' Returns the <code>Format.Field</code> constants associated with
		''' the text at <code>offset</code>. If <code>offset</code> is not
		''' a valid location into the current text, this will return an
		''' empty array.
		''' </summary>
		''' <param name="offset"> offset into text to be examined </param>
		''' <returns> Format.Field constants associated with the text at the
		'''         given position. </returns>
		Public Overridable Function getFields(ByVal offset As Integer) As Format.Field()
			If allowsInvalid Then updateMask()

			Dim attrs As IDictionary(Of java.text.AttributedCharacterIterator.Attribute, Object) = getAttributes(offset)

			If attrs IsNot Nothing AndAlso attrs.Count > 0 Then
				Dim al As New List(Of java.text.AttributedCharacterIterator.Attribute)

				al.AddRange(attrs.Keys)
				Return al.ToArray(EMPTY_FIELD_ARRAY)
			End If
			Return EMPTY_FIELD_ARRAY
		End Function

		''' <summary>
		''' Creates a copy of the DefaultFormatter.
		''' </summary>
		''' <returns> copy of the DefaultFormatter </returns>
		Public Overrides Function clone() As Object
			Dim formatter As InternationalFormatter = CType(MyBase.clone(), InternationalFormatter)

			formatter.literalMask = Nothing
			formatter.iterator = Nothing
			formatter.validMask = False
			formatter.string = Nothing
			Return formatter
		End Function

		''' <summary>
		''' If <code>getSupportsIncrement</code> returns true, this returns
		''' two Actions suitable for incrementing/decrementing the value.
		''' </summary>
		Protected Friend Property Overrides actions As Action()
			Get
				If supportsIncrement Then Return New Action() { New IncrementAction(Me, "increment", 1), New IncrementAction(Me, "decrement", -1) }
				Return Nothing
			End Get
		End Property

		''' <summary>
		''' Invokes <code>parseObject</code> on <code>f</code>, returning
		''' its value.
		''' </summary>
		Friend Overridable Function stringToValue(ByVal text As String, ByVal f As Format) As Object
			If f Is Nothing Then Return text
			Return f.parseObject(text)
		End Function

		''' <summary>
		''' Returns true if <code>value</code> is between the min/max.
		''' </summary>
		''' <param name="wantsCCE"> If false, and a ClassCastException is thrown in
		'''                 comparing the values, the exception is consumed and
		'''                 false is returned. </param>
		Friend Overridable Function isValidValue(ByVal value As Object, ByVal wantsCCE As Boolean) As Boolean
			Dim min As IComparable = minimum

			Try
				If min IsNot Nothing AndAlso min.CompareTo(value) > 0 Then Return False
			Catch cce As ClassCastException
				If wantsCCE Then Throw cce
				Return False
			End Try

			Dim max As IComparable = maximum
			Try
				If max IsNot Nothing AndAlso max.CompareTo(value) < 0 Then Return False
			Catch cce As ClassCastException
				If wantsCCE Then Throw cce
				Return False
			End Try
			Return True
		End Function

		''' <summary>
		''' Returns a Set of the attribute identifiers at <code>index</code>.
		''' </summary>
		Friend Overridable Function getAttributes(ByVal index As Integer) As IDictionary(Of java.text.AttributedCharacterIterator.Attribute, Object)
			If validMask Then
				Dim ___iterator As AttributedCharacterIterator = [iterator]

				If index >= 0 AndAlso index <= ___iterator.endIndex Then
					___iterator.index = index
					Return ___iterator.attributes
				End If
			End If
			Return Nothing
		End Function


		''' <summary>
		''' Returns the start of the first run that contains the attribute
		''' <code>id</code>. This will return <code>-1</code> if the attribute
		''' can not be found.
		''' </summary>
		Friend Overridable Function getAttributeStart(ByVal id As AttributedCharacterIterator.Attribute) As Integer
			If validMask Then
				Dim ___iterator As AttributedCharacterIterator = [iterator]

				___iterator.first()
				Do While ___iterator.current() <> CharacterIterator.DONE
					If ___iterator.getAttribute(id) IsNot Nothing Then Return ___iterator.index
					___iterator.next()
				Loop
			End If
			Return -1
		End Function

		''' <summary>
		''' Returns the <code>AttributedCharacterIterator</code> used to
		''' format the last value.
		''' </summary>
		Friend Overridable Property [iterator] As AttributedCharacterIterator
			Get
				Return [iterator]
			End Get
		End Property

		''' <summary>
		''' Updates the AttributedCharacterIterator and bitset, if necessary.
		''' </summary>
		Friend Overridable Sub updateMaskIfNecessary()
			If (Not allowsInvalid) AndAlso (format IsNot Nothing) Then
				If Not validMask Then
					updateMask()
				Else
					Dim newString As String = formattedTextField.text

					If Not newString.Equals([string]) Then updateMask()
				End If
			End If
		End Sub

		''' <summary>
		''' Updates the AttributedCharacterIterator by invoking
		''' <code>formatToCharacterIterator</code> on the <code>Format</code>.
		''' If this is successful,
		''' <code>updateMask(AttributedCharacterIterator)</code>
		''' is then invoked to update the internal bitmask.
		''' </summary>
		Friend Overridable Sub updateMask()
			If format IsNot Nothing Then
				Dim doc As Document = formattedTextField.document

				validMask = False
				If doc IsNot Nothing Then
					Try
						[string] = doc.getText(0, doc.length)
					Catch ble As BadLocationException
						[string] = Nothing
					End Try
					If [string] IsNot Nothing Then
						Try
							Dim value As Object = stringToValue([string])
							Dim ___iterator As AttributedCharacterIterator = format.formatToCharacterIterator(value)

							updateMask(___iterator)
						Catch pe As ParseException
						Catch iae As System.ArgumentException
						Catch npe As NullPointerException
						End Try
					End If
				End If
			End If
		End Sub

		''' <summary>
		''' Returns the number of literal characters before <code>index</code>.
		''' </summary>
		Friend Overridable Function getLiteralCountTo(ByVal index As Integer) As Integer
			Dim lCount As Integer = 0

			For counter As Integer = 0 To index - 1
				If isLiteral(counter) Then lCount += 1
			Next counter
			Return lCount
		End Function

		''' <summary>
		''' Returns true if the character at index is a literal, that is
		''' not editable.
		''' </summary>
		Friend Overridable Function isLiteral(ByVal index As Integer) As Boolean
			If validMask AndAlso index < [string].Length Then Return literalMask.Get(index)
			Return False
		End Function

		''' <summary>
		''' Returns the literal character at index.
		''' </summary>
		Friend Overridable Function getLiteral(ByVal index As Integer) As Char
			If validMask AndAlso [string] IsNot Nothing AndAlso index < [string].Length Then Return [string].Chars(index)
			Return ChrW(0)
		End Function

		''' <summary>
		''' Returns true if the character at offset is navigable too. This
		''' is implemented in terms of <code>isLiteral</code>, subclasses
		''' may wish to provide different behavior.
		''' </summary>
		Friend Overrides Function isNavigatable(ByVal offset As Integer) As Boolean
			Return Not isLiteral(offset)
		End Function

		''' <summary>
		''' Overriden to update the mask after invoking supers implementation.
		''' </summary>
		Friend Overrides Sub updateValue(ByVal value As Object)
			MyBase.updateValue(value)
			updateMaskIfNecessary()
		End Sub

		''' <summary>
		''' Overriden to unconditionally allow the replace if
		''' ignoreDocumentMutate is true.
		''' </summary>
		Friend Overrides Sub replace(ByVal fb As DocumentFilter.FilterBypass, ByVal offset As Integer, ByVal length As Integer, ByVal text As String, ByVal attrs As AttributeSet)
			If ignoreDocumentMutate Then
				fb.replace(offset, length, text, attrs)
				Return
			End If
			MyBase.replace(fb, offset, length, text, attrs)
		End Sub

		''' <summary>
		''' Returns the index of the next non-literal character starting at
		''' index. If index is not a literal, it will be returned.
		''' </summary>
		''' <param name="direction"> Amount to increment looking for non-literal </param>
		Private Function getNextNonliteralIndex(ByVal index As Integer, ByVal direction As Integer) As Integer
			Dim max As Integer = formattedTextField.document.length

			Do While index >= 0 AndAlso index < max
				If isLiteral(index) Then
					index += direction
				Else
					Return index
				End If
			Loop
			Return If(direction = -1, 0, max)
		End Function

		''' <summary>
		''' Overriden in an attempt to honor the literals.
		''' <p>If we do not allow invalid values and are in overwrite mode, this
		''' {@code rh.length} is corrected as to preserve trailing literals.
		''' If not in overwrite mode, and there is text to insert it is
		''' inserted at the next non literal index going forward.  If there
		''' is only text to remove, it is removed from the next non literal
		''' index going backward.
		''' </summary>
		Friend Overrides Function canReplace(ByVal rh As ReplaceHolder) As Boolean
			If Not allowsInvalid Then
				Dim text As String = rh.text
				Dim tl As Integer = If(text IsNot Nothing, text.Length, 0)
				Dim c As JTextComponent = formattedTextField

				If tl = 0 AndAlso rh.length = 1 AndAlso c.selectionStart <> rh.offset Then
					' Backspace, adjust to actually delete next non-literal.
					rh.offset = getNextNonliteralIndex(rh.offset, -1)
				ElseIf overwriteMode Then
					Dim pos As Integer = rh.offset
					Dim textPos As Integer = pos
					Dim overflown As Boolean = False

					For i As Integer = 0 To rh.length - 1
						Do While isLiteral(pos)
							pos += 1
						Loop
						If pos >= [string].Length Then
							pos = textPos
							overflown = True
							Exit For
						End If
						pos += 1
						textPos = pos
					Next i
					If overflown OrElse c.selectedText Is Nothing Then rh.length = pos - rh.offset
				ElseIf tl > 0 Then
					' insert (or insert and remove)
					rh.offset = getNextNonliteralIndex(rh.offset, 1)
				Else
					' remove only
					rh.offset = getNextNonliteralIndex(rh.offset, -1)
				End If
				CType(rh, ExtendedReplaceHolder).endOffset = rh.offset
				CType(rh, ExtendedReplaceHolder).endTextLength = If(rh.text IsNot Nothing, rh.text.Length, 0)
			Else
				CType(rh, ExtendedReplaceHolder).endOffset = rh.offset
				CType(rh, ExtendedReplaceHolder).endTextLength = If(rh.text IsNot Nothing, rh.text.Length, 0)
			End If
			Dim can As Boolean = MyBase.canReplace(rh)
			If can AndAlso (Not allowsInvalid) Then CType(rh, ExtendedReplaceHolder).resetFromValue(Me)
			Return can
		End Function

		''' <summary>
		''' When in !allowsInvalid mode the text is reset on every edit, thus
		''' supers implementation will position the cursor at the wrong position.
		''' As such, this invokes supers implementation and then invokes
		''' <code>repositionCursor</code> to correctly reset the cursor.
		''' </summary>
		Friend Overrides Function replace(ByVal rh As ReplaceHolder) As Boolean
			Dim start As Integer = -1
			Dim direction As Integer = 1
			Dim literalCount As Integer = -1

			If rh.length > 0 AndAlso (rh.text Is Nothing OrElse rh.text.Length = 0) AndAlso (formattedTextField.selectionStart <> rh.offset OrElse rh.length > 1) Then direction = -1
			If Not allowsInvalid Then
				If (rh.text Is Nothing OrElse rh.text.Length = 0) AndAlso rh.length > 0 Then
					' remove
					start = formattedTextField.selectionStart
				Else
					start = rh.offset
				End If
				literalCount = getLiteralCountTo(start)
			End If
			If MyBase.replace(rh) Then
				If start <> -1 Then
					Dim [end] As Integer = CType(rh, ExtendedReplaceHolder).endOffset

					[end] += CType(rh, ExtendedReplaceHolder).endTextLength
					repositionCursor(literalCount, [end], direction)
				Else
					start = CType(rh, ExtendedReplaceHolder).endOffset
					If direction = 1 Then start += CType(rh, ExtendedReplaceHolder).endTextLength
					repositionCursor(start, direction)
				End If
				Return True
			End If
			Return False
		End Function

		''' <summary>
		''' Repositions the cursor. <code>startLiteralCount</code> gives
		''' the number of literals to the start of the deleted range, end
		''' gives the ending location to adjust from, direction gives
		''' the direction relative to <code>end</code> to position the
		''' cursor from.
		''' </summary>
		Private Sub repositionCursor(ByVal startLiteralCount As Integer, ByVal [end] As Integer, ByVal direction As Integer)
			Dim endLiteralCount As Integer = getLiteralCountTo([end])

			If endLiteralCount <> [end] Then
				[end] -= startLiteralCount
				Dim counter As Integer = 0
				Do While counter < [end]
					If isLiteral(counter) Then [end] += 1
					counter += 1
				Loop
			End If
			repositionCursor([end], 1) 'direction
		End Sub

		''' <summary>
		''' Returns the character from the mask that has been buffered
		''' at <code>index</code>.
		''' </summary>
		Friend Overridable Function getBufferedChar(ByVal index As Integer) As Char
			If validMask Then
				If [string] IsNot Nothing AndAlso index < [string].Length Then Return [string].Chars(index)
			End If
			Return ChrW(0)
		End Function

		''' <summary>
		''' Returns true if the current mask is valid.
		''' </summary>
		Friend Overridable Property validMask As Boolean
			Get
				Return validMask
			End Get
		End Property

		''' <summary>
		''' Returns true if <code>attributes</code> is null or empty.
		''' </summary>
		Friend Overridable Function isLiteral(ByVal attributes As IDictionary) As Boolean
			Return ((attributes Is Nothing) OrElse attributes.Count = 0)
		End Function

		''' <summary>
		''' Updates the interal bitset from <code>iterator</code>. This will
		''' set <code>validMask</code> to true if <code>iterator</code> is
		''' non-null.
		''' </summary>
		Private Sub updateMask(ByVal [iterator] As AttributedCharacterIterator)
			If [iterator] IsNot Nothing Then
				validMask = True
				Me.iterator = [iterator]

				' Update the literal mask
				If literalMask Is Nothing Then
					literalMask = New BitArray
				Else
					For counter As Integer = literalMask.length() - 1 To 0 Step -1
						literalMask.Set(counter, False)
					Next counter
				End If

				[iterator].first()
				Do While [iterator].current() <> CharacterIterator.DONE
					Dim ___attributes As IDictionary = [iterator].attributes
					Dim [set] As Boolean = isLiteral(___attributes)
					Dim start As Integer = [iterator].index
					Dim [end] As Integer = [iterator].runLimit

					Do While start < [end]
						If [set] Then
							literalMask.Set(start, True)
						Else
							literalMask.Set(start, False)
						End If
						start += 1
					Loop
					[iterator].index = start
				Loop
			End If
		End Sub

		''' <summary>
		''' Returns true if <code>field</code> is non-null.
		''' Subclasses that wish to allow incrementing to happen outside of
		''' the known fields will need to override this.
		''' </summary>
		Friend Overridable Function canIncrement(ByVal field As Object, ByVal cursorPosition As Integer) As Boolean
			Return (field IsNot Nothing)
		End Function

		''' <summary>
		''' Selects the fields identified by <code>attributes</code>.
		''' </summary>
		Friend Overridable Sub selectField(ByVal f As Object, ByVal count As Integer)
			Dim ___iterator As AttributedCharacterIterator = [iterator]

			If ___iterator IsNot Nothing AndAlso (TypeOf f Is AttributedCharacterIterator.Attribute) Then
				Dim field As AttributedCharacterIterator.Attribute = CType(f, AttributedCharacterIterator.Attribute)

				___iterator.first()
				Do While ___iterator.current() <> CharacterIterator.DONE
					Do While ___iterator.getAttribute(field) Is Nothing AndAlso ___iterator.next() <> CharacterIterator.DONE

					Loop
					If ___iterator.current() <> CharacterIterator.DONE Then
						Dim limit As Integer = ___iterator.getRunLimit(field)

						count -= 1
						If count <= 0 Then
							formattedTextField.select(___iterator.index, limit)
							Exit Do
						End If
						___iterator.index = limit
						___iterator.next()
					End If
				Loop
			End If
		End Sub

		''' <summary>
		''' Returns the field that will be adjusted by adjustValue.
		''' </summary>
		Friend Overridable Function getAdjustField(ByVal start As Integer, ByVal attributes As IDictionary) As Object
			Return Nothing
		End Function

		''' <summary>
		''' Returns the number of occurrences of <code>f</code> before
		''' the location <code>start</code> in the current
		''' <code>AttributedCharacterIterator</code>.
		''' </summary>
		Private Function getFieldTypeCountTo(ByVal f As Object, ByVal start As Integer) As Integer
			Dim ___iterator As AttributedCharacterIterator = [iterator]
			Dim count As Integer = 0

			If ___iterator IsNot Nothing AndAlso (TypeOf f Is AttributedCharacterIterator.Attribute) Then
				Dim field As AttributedCharacterIterator.Attribute = CType(f, AttributedCharacterIterator.Attribute)

				___iterator.first()
				Do While ___iterator.index < start
					Do While ___iterator.getAttribute(field) Is Nothing AndAlso ___iterator.next() <> CharacterIterator.DONE

					Loop
					If ___iterator.current() <> CharacterIterator.DONE Then
						___iterator.index = ___iterator.getRunLimit(field)
						___iterator.next()
						count += 1
					Else
						Exit Do
					End If
				Loop
			End If
			Return count
		End Function

		''' <summary>
		''' Subclasses supporting incrementing must override this to handle
		''' the actual incrementing. <code>value</code> is the current value,
		''' <code>attributes</code> gives the field the cursor is in (may be
		''' null depending upon <code>canIncrement</code>) and
		''' <code>direction</code> is the amount to increment by.
		''' </summary>
		Friend Overridable Function adjustValue(ByVal value As Object, ByVal attributes As IDictionary, ByVal field As Object, ByVal direction As Integer) As Object
			Return Nothing
		End Function

		''' <summary>
		''' Returns false, indicating InternationalFormatter does not allow
		''' incrementing of the value. Subclasses that wish to support
		''' incrementing/decrementing the value should override this and
		''' return true. Subclasses should also override
		''' <code>adjustValue</code>.
		''' </summary>
		Friend Overridable Property supportsIncrement As Boolean
			Get
				Return False
			End Get
		End Property

		''' <summary>
		''' Resets the value of the JFormattedTextField to be
		''' <code>value</code>.
		''' </summary>
		Friend Overridable Sub resetValue(ByVal value As Object)
			Dim doc As Document = formattedTextField.document
			Dim [string] As String = valueToString(value)

			Try
				ignoreDocumentMutate = True
				doc.remove(0, doc.length)
				doc.insertString(0, [string], Nothing)
			Finally
				ignoreDocumentMutate = False
			End Try
			updateValue(value)
		End Sub

		''' <summary>
		''' Subclassed to update the internal representation of the mask after
		''' the default read operation has completed.
		''' </summary>
		Private Sub readObject(ByVal s As ObjectInputStream)
			s.defaultReadObject()
			updateMaskIfNecessary()
		End Sub


		''' <summary>
		''' Overriden to return an instance of <code>ExtendedReplaceHolder</code>.
		''' </summary>
		Friend Overrides Function getReplaceHolder(ByVal fb As DocumentFilter.FilterBypass, ByVal offset As Integer, ByVal length As Integer, ByVal text As String, ByVal attrs As AttributeSet) As ReplaceHolder
			If replaceHolder Is Nothing Then replaceHolder = New ExtendedReplaceHolder
			Return MyBase.getReplaceHolder(fb, offset, length, text, attrs)
		End Function


		''' <summary>
		''' As InternationalFormatter replaces the complete text on every edit,
		''' ExtendedReplaceHolder keeps track of the offset and length passed
		''' into canReplace.
		''' </summary>
		Friend Class ExtendedReplaceHolder
			Inherits ReplaceHolder

			''' <summary>
			''' Offset of the insert/remove. This may differ from offset in
			''' that if !allowsInvalid the text is replaced on every edit. 
			''' </summary>
			Friend endOffset As Integer
			''' <summary>
			''' Length of the text. This may differ from text.length in
			''' that if !allowsInvalid the text is replaced on every edit. 
			''' </summary>
			Friend endTextLength As Integer

			''' <summary>
			''' Resets the region to delete to be the complete document and
			''' the text from invoking valueToString on the current value.
			''' </summary>
			Friend Overridable Sub resetFromValue(ByVal formatter As InternationalFormatter)
				' Need to reset the complete string as Format's result can
				' be completely different.
				offset = 0
				Try
					text = formatter.valueToString(value)
				Catch pe As ParseException
					' Should never happen, otherwise canReplace would have
					' returned value.
					text = ""
				End Try
				length = fb.document.length
			End Sub
		End Class


		''' <summary>
		''' IncrementAction is used to increment the value by a certain amount.
		''' It calls into <code>adjustValue</code> to handle the actual
		''' incrementing of the value.
		''' </summary>
		Private Class IncrementAction
			Inherits AbstractAction

			Private ReadOnly outerInstance As InternationalFormatter

			Private direction As Integer

			Friend Sub New(ByVal outerInstance As InternationalFormatter, ByVal name As String, ByVal direction As Integer)
					Me.outerInstance = outerInstance
				MyBase.New(name)
				Me.direction = direction
			End Sub

			Public Overridable Sub actionPerformed(ByVal ae As java.awt.event.ActionEvent)

				If outerInstance.formattedTextField.editable Then
					If outerInstance.allowsInvalid Then outerInstance.updateMask()

					Dim validEdit As Boolean = False

					If outerInstance.validMask Then
						Dim start As Integer = outerInstance.formattedTextField.selectionStart

						If start <> -1 Then
							Dim [iterator] As AttributedCharacterIterator = outerInstance.iterator

							[iterator].index = start

							Dim attributes As IDictionary = [iterator].attributes
							Dim field As Object = outerInstance.getAdjustField(start, attributes)

							If outerInstance.canIncrement(field, start) Then
								Try
									Dim ___value As Object = outerInstance.stringToValue(outerInstance.formattedTextField.text)
									Dim fieldTypeCount As Integer = outerInstance.getFieldTypeCountTo(field, start)

									___value = outerInstance.adjustValue(___value, attributes, field, direction)
									If ___value IsNot Nothing AndAlso outerInstance.isValidValue(___value, False) Then
										outerInstance.resetValue(___value)
										outerInstance.updateMask()

										If outerInstance.validMask Then outerInstance.selectField(field, fieldTypeCount)
										validEdit = True
									End If
								Catch pe As ParseException
								Catch ble As BadLocationException
								End Try
							End If
						End If
					End If
					If Not validEdit Then outerInstance.invalidEdit()
				End If
			End Sub
		End Class
	End Class

End Namespace
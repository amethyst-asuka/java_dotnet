Imports System
Imports System.Collections.Generic
Imports System.Text
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
	''' <code>MaskFormatter</code> is used to format and edit strings. The behavior
	''' of a <code>MaskFormatter</code> is controlled by way of a String mask
	''' that specifies the valid characters that can be contained at a particular
	''' location in the <code>Document</code> model. The following characters can
	''' be specified:
	''' 
	''' <table border=1 summary="Valid characters and their descriptions">
	''' <tr>
	'''    <th>Character&nbsp;</th>
	'''    <th><p style="text-align:left">Description</p></th>
	''' </tr>
	''' <tr>
	'''    <td>#</td>
	'''    <td>Any valid number, uses <code>Character.isDigit</code>.</td>
	''' </tr>
	''' <tr>
	'''    <td>'</td>
	'''    <td>Escape character, used to escape any of the
	'''       special formatting characters.</td>
	''' </tr>
	''' <tr>
	'''    <td>U</td><td>Any character (<code>Character.isLetter</code>). All
	'''        lowercase letters are mapped to upper case.</td>
	''' </tr>
	''' <tr><td>L</td><td>Any character (<code>Character.isLetter</code>). All
	'''        upper case letters are mapped to lower case.</td>
	''' </tr>
	''' <tr><td>A</td><td>Any character or number (<code>Character.isLetter</code>
	'''       or <code>Character.isDigit</code>)</td>
	''' </tr>
	''' <tr><td>?</td><td>Any character
	'''        (<code>Character.isLetter</code>).</td>
	''' </tr>
	''' <tr><td>*</td><td>Anything.</td></tr>
	''' <tr><td>H</td><td>Any hex character (0-9, a-f or A-F).</td></tr>
	''' </table>
	''' 
	''' <p>
	''' Typically characters correspond to one char, but in certain languages this
	''' is not the case. The mask is on a per character basis, and will thus
	''' adjust to fit as many chars as are needed.
	''' <p>
	''' You can further restrict the characters that can be input by the
	''' <code>setInvalidCharacters</code> and <code>setValidCharacters</code>
	''' methods. <code>setInvalidCharacters</code> allows you to specify
	''' which characters are not legal. <code>setValidCharacters</code> allows
	''' you to specify which characters are valid. For example, the following
	''' code block is equivalent to a mask of '0xHHH' with no invalid/valid
	''' characters:
	''' <pre>
	''' MaskFormatter formatter = new MaskFormatter("0x***");
	''' formatter.setValidCharacters("0123456789abcdefABCDEF");
	''' </pre>
	''' <p>
	''' When initially formatting a value if the length of the string is
	''' less than the length of the mask, two things can happen. Either
	''' the placeholder string will be used, or the placeholder character will
	''' be used. Precedence is given to the placeholder string. For example:
	''' <pre>
	'''   MaskFormatter formatter = new MaskFormatter("###-####");
	'''   formatter.setPlaceholderCharacter('_');
	'''   formatter.getDisplayValue(tf, "123");
	''' </pre>
	''' <p>
	''' Would result in the string '123-____'. If
	''' <code>setPlaceholder("555-1212")</code> was invoked '123-1212' would
	''' result. The placeholder String is only used on the initial format,
	''' on subsequent formats only the placeholder character will be used.
	''' <p>
	''' If a <code>MaskFormatter</code> is configured to only allow valid characters
	''' (<code>setAllowsInvalid(false)</code>) literal characters will be skipped as
	''' necessary when editing. Consider a <code>MaskFormatter</code> with
	''' the mask "###-####" and current value "555-1212". Using the right
	''' arrow key to navigate through the field will result in (| indicates the
	''' position of the caret):
	''' <pre>
	'''   |555-1212
	'''   5|55-1212
	'''   55|5-1212
	'''   555-|1212
	'''   555-1|212
	''' </pre>
	''' The '-' is a literal (non-editable) character, and is skipped.
	''' <p>
	''' Similar behavior will result when editing. Consider inserting the string
	''' '123-45' and '12345' into the <code>MaskFormatter</code> in the
	''' previous example. Both inserts will result in the same String,
	''' '123-45__'. When <code>MaskFormatter</code>
	''' is processing the insert at character position 3 (the '-'), two things can
	''' happen:
	''' <ol>
	'''   <li>If the inserted character is '-', it is accepted.
	'''   <li>If the inserted character matches the mask for the next non-literal
	'''       character, it is accepted at the new location.
	'''   <li>Anything else results in an invalid edit
	''' </ol>
	''' <p>
	''' By default <code>MaskFormatter</code> will not allow invalid edits, you can
	''' change this with the <code>setAllowsInvalid</code> method, and will
	''' commit edits on valid edits (use the <code>setCommitsOnValidEdit</code> to
	''' change this).
	''' <p>
	''' By default, <code>MaskFormatter</code> is in overwrite mode. That is as
	''' characters are typed a new character is not inserted, rather the character
	''' at the current location is replaced with the newly typed character. You
	''' can change this behavior by way of the method <code>setOverwriteMode</code>.
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
	Public Class MaskFormatter
		Inherits DefaultFormatter

		' Potential values in mask.
		Private Const DIGIT_KEY As Char = "#"c
		Private Const LITERAL_KEY As Char = "'"c
		Private Const UPPERCASE_KEY As Char = "U"c
		Private Const LOWERCASE_KEY As Char = "L"c
		Private Const ALPHA_NUMERIC_KEY As Char = "A"c
		Private Const CHARACTER_KEY As Char = "?"c
		Private Const ANYTHING_KEY As Char = "*"c
		Private Const HEX_KEY As Char = "H"c

		Private Shared ReadOnly EmptyMaskChars As MaskCharacter() = New MaskCharacter(){}

		''' <summary>
		''' The user specified mask. </summary>
		Private mask As String

		<NonSerialized> _
		Private maskChars As MaskCharacter()

		''' <summary>
		''' List of valid characters. </summary>
		Private validCharacters As String

		''' <summary>
		''' List of invalid characters. </summary>
		Private invalidCharacters As String

		''' <summary>
		''' String used for the passed in value if it does not completely
		''' fill the mask. 
		''' </summary>
		Private placeholderString As String

		''' <summary>
		''' String used to represent characters not present. </summary>
		Private placeholder As Char

		''' <summary>
		''' Indicates if the value contains the literal characters. </summary>
		Private containsLiteralChars As Boolean


		''' <summary>
		''' Creates a MaskFormatter with no mask.
		''' </summary>
		Public Sub New()
			allowsInvalid = False
			containsLiteralChars = True
			maskChars = EmptyMaskChars
			placeholder = " "c
		End Sub

		''' <summary>
		''' Creates a <code>MaskFormatter</code> with the specified mask.
		''' A <code>ParseException</code>
		''' will be thrown if <code>mask</code> is an invalid mask.
		''' </summary>
		''' <exception cref="ParseException"> if mask does not contain valid mask characters </exception>
		Public Sub New(ByVal mask As String)
			Me.New()
			mask = mask
		End Sub

		''' <summary>
		''' Sets the mask dictating the legal characters.
		''' This will throw a <code>ParseException</code> if <code>mask</code> is
		''' not valid.
		''' </summary>
		''' <exception cref="ParseException"> if mask does not contain valid mask characters </exception>
		Public Overridable Property mask As String
			Set(ByVal mask As String)
				Me.mask = mask
				updateInternalMask()
			End Set
			Get
				Return mask
			End Get
		End Property


		''' <summary>
		''' Allows for further restricting of the characters that can be input.
		''' Only characters specified in the mask, not in the
		''' <code>invalidCharacters</code>, and in
		''' <code>validCharacters</code> will be allowed to be input. Passing
		''' in null (the default) implies the valid characters are only bound
		''' by the mask and the invalid characters.
		''' </summary>
		''' <param name="validCharacters"> If non-null, specifies legal characters. </param>
		Public Overridable Property validCharacters As String
			Set(ByVal validCharacters As String)
				Me.validCharacters = validCharacters
			End Set
			Get
				Return validCharacters
			End Get
		End Property


		''' <summary>
		''' Allows for further restricting of the characters that can be input.
		''' Only characters specified in the mask, not in the
		''' <code>invalidCharacters</code>, and in
		''' <code>validCharacters</code> will be allowed to be input. Passing
		''' in null (the default) implies the valid characters are only bound
		''' by the mask and the valid characters.
		''' </summary>
		''' <param name="invalidCharacters"> If non-null, specifies illegal characters. </param>
		Public Overridable Property invalidCharacters As String
			Set(ByVal invalidCharacters As String)
				Me.invalidCharacters = invalidCharacters
			End Set
			Get
				Return invalidCharacters
			End Get
		End Property


		''' <summary>
		''' Sets the string to use if the value does not completely fill in
		''' the mask. A null value implies the placeholder char should be used.
		''' </summary>
		''' <param name="placeholder"> String used when formatting if the value does not
		'''        completely fill the mask </param>
		Public Overridable Property placeholder As String
			Set(ByVal placeholder As String)
				Me.placeholderString = placeholder
			End Set
			Get
				Return placeholderString
			End Get
		End Property


		''' <summary>
		''' Sets the character to use in place of characters that are not present
		''' in the value, ie the user must fill them in. The default value is
		''' a space.
		''' <p>
		''' This is only applicable if the placeholder string has not been
		''' specified, or does not completely fill in the mask.
		''' </summary>
		''' <param name="placeholder"> Character used when formatting if the value does not
		'''        completely fill the mask </param>
		Public Overridable Property placeholderCharacter As Char
			Set(ByVal placeholder As Char)
				Me.placeholder = placeholder
			End Set
			Get
				Return placeholder
			End Get
		End Property


		''' <summary>
		''' If true, the returned value and set value will also contain the literal
		''' characters in mask.
		''' <p>
		''' For example, if the mask is <code>'(###) ###-####'</code>, the
		''' current value is <code>'(415) 555-1212'</code>, and
		''' <code>valueContainsLiteralCharacters</code> is
		''' true <code>stringToValue</code> will return
		''' <code>'(415) 555-1212'</code>. On the other hand, if
		''' <code>valueContainsLiteralCharacters</code> is false,
		''' <code>stringToValue</code> will return <code>'4155551212'</code>.
		''' </summary>
		''' <param name="containsLiteralChars"> Used to indicate if literal characters in
		'''        mask should be returned in stringToValue </param>
		Public Overridable Property valueContainsLiteralCharacters As Boolean
			Set(ByVal containsLiteralChars As Boolean)
				Me.containsLiteralChars = containsLiteralChars
			End Set
			Get
				Return containsLiteralChars
			End Get
		End Property


		''' <summary>
		''' Parses the text, returning the appropriate Object representation of
		''' the String <code>value</code>. This strips the literal characters as
		''' necessary and invokes supers <code>stringToValue</code>, so that if
		''' you have specified a value class (<code>setValueClass</code>) an
		''' instance of it will be created. This will throw a
		''' <code>ParseException</code> if the value does not match the current
		''' mask.  Refer to <seealso cref="#setValueContainsLiteralCharacters"/> for details
		''' on how literals are treated.
		''' </summary>
		''' <exception cref="ParseException"> if there is an error in the conversion </exception>
		''' <param name="value"> String to convert </param>
		''' <seealso cref= #setValueContainsLiteralCharacters </seealso>
		''' <returns> Object representation of text </returns>
		Public Overrides Function stringToValue(ByVal value As String) As Object
			Return stringToValue(value, True)
		End Function

		''' <summary>
		''' Returns a String representation of the Object <code>value</code>
		''' based on the mask.  Refer to
		''' <seealso cref="#setValueContainsLiteralCharacters"/> for details
		''' on how literals are treated.
		''' </summary>
		''' <exception cref="ParseException"> if there is an error in the conversion </exception>
		''' <param name="value"> Value to convert </param>
		''' <seealso cref= #setValueContainsLiteralCharacters </seealso>
		''' <returns> String representation of value </returns>
		Public Overrides Function valueToString(ByVal value As Object) As String
			Dim sValue As String = If(value Is Nothing, "", value.ToString())
			Dim result As New StringBuilder
			Dim ___placeholder As String = placeholder
			Dim valueCounter As Integer() = { 0 }

			append(result, sValue, valueCounter, ___placeholder, maskChars)
			Return result.ToString()
		End Function

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
			' valueToString doesn't throw, but stringToValue does, need to
			' update the editValid state appropriately
			If ftf IsNot Nothing Then
				Dim value As Object = ftf.value

				Try
					stringToValue(valueToString(value))
				Catch pe As ParseException
					editValid = False
				End Try
			End If
		End Sub

		''' <summary>
		''' Actual <code>stringToValue</code> implementation.
		''' If <code>completeMatch</code> is true, the value must exactly match
		''' the mask, on the other hand if <code>completeMatch</code> is false
		''' the string must match the mask or the placeholder string.
		''' </summary>
		Private Function stringToValue(ByVal value As String, ByVal completeMatch As Boolean) As Object
			Dim errorOffset As Integer

			errorOffset = getInvalidOffset(value, completeMatch)
			If errorOffset = -1 Then
				If Not valueContainsLiteralCharacters Then value = stripLiteralChars(value)
				Return MyBase.stringToValue(value)
			End If
			Throw New ParseException("stringToValue passed invalid value", errorOffset)
		End Function

		''' <summary>
		''' Returns -1 if the passed in string is valid, otherwise the index of
		''' the first bogus character is returned.
		''' </summary>
		Private Function getInvalidOffset(ByVal [string] As String, ByVal completeMatch As Boolean) As Integer
			Dim iLength As Integer = [string].Length

			If iLength <> maxLength Then Return iLength
			Dim counter As Integer = 0
			Dim max As Integer = [string].Length
			Do While counter < max
				Dim aChar As Char = [string].Chars(counter)

				If (Not isValidCharacter(counter, aChar)) AndAlso (completeMatch OrElse (Not isPlaceholder(counter, aChar))) Then Return counter
				counter += 1
			Loop
			Return -1
		End Function

		''' <summary>
		''' Invokes <code>append</code> on the mask characters in
		''' <code>mask</code>.
		''' </summary>
		Private Sub append(ByVal result As StringBuilder, ByVal value As String, ByVal index As Integer(), ByVal placeholder As String, ByVal mask As MaskCharacter())
			Dim counter As Integer = 0
			Dim maxCounter As Integer = mask.Length
			Do While counter < maxCounter
				mask(counter).append(result, value, index, placeholder)
				counter += 1
			Loop
		End Sub

		''' <summary>
		''' Updates the internal representation of the mask.
		''' </summary>
		Private Sub updateInternalMask()
			Dim ___mask As String = mask
			Dim fixed As New List(Of MaskCharacter)
			Dim temp As List(Of MaskCharacter) = fixed

			If ___mask IsNot Nothing Then
				Dim counter As Integer = 0
				Dim maxCounter As Integer = ___mask.Length
				Do While counter < maxCounter
					Dim maskChar As Char = ___mask.Chars(counter)

					Select Case maskChar
					Case DIGIT_KEY
						temp.Add(New DigitMaskCharacter(Me))
					Case LITERAL_KEY
						counter += 1
						If counter < maxCounter Then
							maskChar = ___mask.Chars(counter)
							temp.Add(New LiteralCharacter(Me, maskChar))
						End If
						' else: Could actually throw if else
					Case UPPERCASE_KEY
						temp.Add(New UpperCaseCharacter(Me))
					Case LOWERCASE_KEY
						temp.Add(New LowerCaseCharacter(Me))
					Case ALPHA_NUMERIC_KEY
						temp.Add(New AlphaNumericCharacter(Me))
					Case CHARACTER_KEY
						temp.Add(New CharCharacter(Me))
					Case ANYTHING_KEY
						temp.Add(New MaskCharacter(Me))
					Case HEX_KEY
						temp.Add(New HexCharacter(Me))
					Case Else
						temp.Add(New LiteralCharacter(Me, maskChar))
					End Select
					counter += 1
				Loop
			End If
			If fixed.Count = 0 Then
				maskChars = EmptyMaskChars
			Else
				maskChars = New MaskCharacter(fixed.Count - 1){}
				fixed.ToArray(maskChars)
			End If
		End Sub

		''' <summary>
		''' Returns the MaskCharacter at the specified location.
		''' </summary>
		Private Function getMaskCharacter(ByVal index As Integer) As MaskCharacter
			If index >= maskChars.Length Then Return Nothing
			Return maskChars(index)
		End Function

		''' <summary>
		''' Returns true if the placeholder character matches aChar.
		''' </summary>
		Private Function isPlaceholder(ByVal index As Integer, ByVal aChar As Char) As Boolean
			Return (placeholderCharacter = aChar)
		End Function

		''' <summary>
		''' Returns true if the passed in character matches the mask at the
		''' specified location.
		''' </summary>
		Private Function isValidCharacter(ByVal index As Integer, ByVal aChar As Char) As Boolean
			Return getMaskCharacter(index).isValidCharacter(aChar)
		End Function

		''' <summary>
		''' Returns true if the character at the specified location is a literal,
		''' that is it can not be edited.
		''' </summary>
		Private Function isLiteral(ByVal index As Integer) As Boolean
			Return getMaskCharacter(index).literal
		End Function

		''' <summary>
		''' Returns the maximum length the text can be.
		''' </summary>
		Private Property maxLength As Integer
			Get
				Return maskChars.Length
			End Get
		End Property

		''' <summary>
		''' Returns the literal character at the specified location.
		''' </summary>
		Private Function getLiteral(ByVal index As Integer) As Char
			Return getMaskCharacter(index).getChar(ChrW(0))
		End Function

		''' <summary>
		''' Returns the character to insert at the specified location based on
		''' the passed in character.  This provides a way to map certain sets
		''' of characters to alternative values (lowercase to
		''' uppercase...).
		''' </summary>
		Private Function getCharacter(ByVal index As Integer, ByVal aChar As Char) As Char
			Return getMaskCharacter(index).getChar(aChar)
		End Function

		''' <summary>
		''' Removes the literal characters from the passed in string.
		''' </summary>
		Private Function stripLiteralChars(ByVal [string] As String) As String
			Dim sb As StringBuilder = Nothing
			Dim last As Integer = 0

			Dim counter As Integer = 0
			Dim max As Integer = [string].Length
			Do While counter < max
				If isLiteral(counter) Then
					If sb Is Nothing Then
						sb = New StringBuilder
						If counter > 0 Then sb.Append([string].Substring(0, counter))
						last = counter + 1
					ElseIf last <> counter Then
						sb.Append([string].Substring(last, counter - last))
					End If
					last = counter + 1
				End If
				counter += 1
			Loop
			If sb Is Nothing Then
				' Assume the mask isn't all literals.
				Return [string]
			ElseIf last <> [string].Length Then
				If sb Is Nothing Then Return [string].Substring(last)
				sb.Append([string].Substring(last))
			End If
			Return sb.ToString()
		End Function


		''' <summary>
		''' Subclassed to update the internal representation of the mask after
		''' the default read operation has completed.
		''' </summary>
		Private Sub readObject(ByVal s As ObjectInputStream)
			s.defaultReadObject()
			Try
				updateInternalMask()
			Catch pe As ParseException
				' assert();
			End Try
		End Sub

		''' <summary>
		''' Returns true if the MaskFormatter allows invalid, or
		''' the offset is less than the max length and the character at
		''' <code>offset</code> is a literal.
		''' </summary>
		Friend Overrides Function isNavigatable(ByVal offset As Integer) As Boolean
			If Not allowsInvalid Then Return (offset < maxLength AndAlso (Not isLiteral(offset)))
			Return True
		End Function

	'    
	'     * Returns true if the operation described by <code>rh</code> will
	'     * result in a legal edit.  This may set the <code>value</code>
	'     * field of <code>rh</code>.
	'     * <p>
	'     * This is overriden to return true for a partial match.
	'     
		Friend Overrides Function isValidEdit(ByVal rh As ReplaceHolder) As Boolean
			If Not allowsInvalid Then
				Dim newString As String = getReplaceString(rh.offset, rh.length, rh.text)

				Try
					rh.value = stringToValue(newString, False)

					Return True
				Catch pe As ParseException
					Return False
				End Try
			End If
			Return True
		End Function

		''' <summary>
		''' This method does the following (assuming !getAllowsInvalid()):
		''' iterate over the max of the deleted region or the text length, for
		''' each character:
		''' <ol>
		''' <li>If it is valid (matches the mask at the particular position, or
		'''     matches the literal character at the position), allow it
		''' <li>Else if the position identifies a literal character, add it. This
		'''     allows for the user to paste in text that may/may not contain
		'''     the literals.  For example, in pasing in 5551212 into ###-####
		'''     when the 1 is evaluated it is illegal (by the first test), but there
		'''     is a literal at this position (-), so it is used.  NOTE: This has
		'''     a problem that you can't tell (without looking ahead) if you should
		'''     eat literals in the text. For example, if you paste '555' into
		'''     #5##, should it result in '5555' or '555 '? The current code will
		'''     result in the latter, which feels a little better as selecting
		'''     text than pasting will always result in the same thing.
		''' <li>Else if at the end of the inserted text, the replace the item with
		'''     the placeholder
		''' <li>Otherwise the insert is bogus and false is returned.
		''' </ol>
		''' </summary>
		Friend Overrides Function canReplace(ByVal rh As ReplaceHolder) As Boolean
			' This method is rather long, but much of the burden is in
			' maintaining a String and swapping to a StringBuilder only if
			' absolutely necessary.
			If Not allowsInvalid Then
				Dim replace As StringBuilder = Nothing
				Dim text As String = rh.text
				Dim tl As Integer = If(text IsNot Nothing, text.Length, 0)

				If tl = 0 AndAlso rh.length = 1 AndAlso formattedTextField.selectionStart <> rh.offset Then
					' Backspace, adjust to actually delete next non-literal.
					Do While rh.offset > 0 AndAlso isLiteral(rh.offset)
						rh.offset -= 1
					Loop
				End If
				Dim max As Integer = Math.Min(maxLength - rh.offset, Math.Max(tl, rh.length))
				Dim counter As Integer = 0
				Dim textIndex As Integer = 0
				Do While counter < max
					If textIndex < tl AndAlso isValidCharacter(rh.offset + counter, text.Chars(textIndex)) Then
						Dim aChar As Char = text.Chars(textIndex)
						If aChar <> getCharacter(rh.offset + counter, aChar) Then
							If replace Is Nothing Then
								replace = New StringBuilder
								If textIndex > 0 Then replace.Append(text.Substring(0, textIndex))
							End If
						End If
						If replace IsNot Nothing Then replace.Append(getCharacter(rh.offset + counter, aChar))
						textIndex += 1
					ElseIf isLiteral(rh.offset + counter) Then
						If replace IsNot Nothing Then
							replace.Append(getLiteral(rh.offset + counter))
							If textIndex < tl Then max = Math.Min(max + 1, maxLength - rh.offset)
						ElseIf textIndex > 0 Then
							replace = New StringBuilder(max)
							replace.Append(text.Substring(0, textIndex))
							replace.Append(getLiteral(rh.offset + counter))
							If textIndex < tl Then
								' Evaluate the character in text again.
								max = Math.Min(max + 1, maxLength - rh.offset)
							ElseIf rh.cursorPosition = -1 Then
								rh.cursorPosition = rh.offset + counter
							End If
						Else
							rh.offset += 1
							rh.length -= 1
							counter -= 1
							max -= 1
						End If
					ElseIf textIndex >= tl Then
						' placeholder
						If replace Is Nothing Then
							replace = New StringBuilder
							If text IsNot Nothing Then replace.Append(text)
						End If
						replace.Append(placeholderCharacter)
						If tl > 0 AndAlso rh.cursorPosition = -1 Then rh.cursorPosition = rh.offset + counter
					Else
						' Bogus character.
						Return False
					End If
					counter += 1
				Loop
				If replace IsNot Nothing Then
					rh.text = replace.ToString()
				ElseIf text IsNot Nothing AndAlso rh.offset + tl > maxLength Then
					rh.text = text.Substring(0, maxLength - rh.offset)
				End If
				If overwriteMode AndAlso rh.text IsNot Nothing Then rh.length = rh.text.Length
			End If
			Return MyBase.canReplace(rh)
		End Function


		'
		' Interal classes used to represent the mask.
		'
		Private Class MaskCharacter
			Private ReadOnly outerInstance As MaskFormatter

			Public Sub New(ByVal outerInstance As MaskFormatter)
				Me.outerInstance = outerInstance
			End Sub

			''' <summary>
			''' Subclasses should override this returning true if the instance
			''' represents a literal character. The default implementation
			''' returns false.
			''' </summary>
			Public Overridable Property literal As Boolean
				Get
					Return False
				End Get
			End Property

			''' <summary>
			''' Returns true if <code>aChar</code> is a valid reprensentation of
			''' the receiver. The default implementation returns true if the
			''' receiver represents a literal character and <code>getChar</code>
			''' == aChar. Otherwise, this will return true is <code>aChar</code>
			''' is contained in the valid characters and not contained
			''' in the invalid characters.
			''' </summary>
			Public Overridable Function isValidCharacter(ByVal aChar As Char) As Boolean
				If literal Then Return (getChar(aChar) = aChar)

				aChar = getChar(aChar)

				Dim filter As String = outerInstance.validCharacters

				If filter IsNot Nothing AndAlso filter.IndexOf(aChar) = -1 Then Return False
				filter = outerInstance.invalidCharacters
				If filter IsNot Nothing AndAlso filter.IndexOf(aChar) <> -1 Then Return False
				Return True
			End Function

			''' <summary>
			''' Returns the character to insert for <code>aChar</code>. The
			''' default implementation returns <code>aChar</code>. Subclasses
			''' that wish to do some sort of mapping, perhaps lower case to upper
			''' case should override this and do the necessary mapping.
			''' </summary>
			Public Overridable Function getChar(ByVal aChar As Char) As Char
				Return aChar
			End Function

			''' <summary>
			''' Appends the necessary character in <code>formatting</code> at
			''' <code>index</code> to <code>buff</code>.
			''' </summary>
			Public Overridable Sub append(ByVal buff As StringBuilder, ByVal formatting As String, ByVal index As Integer(), ByVal placeholder As String)
				Dim inString As Boolean = index(0) < formatting.Length
				Dim aChar As Char = If(inString, formatting.Chars(index(0)), 0)

				If literal Then
					buff.Append(getChar(aChar))
					If outerInstance.valueContainsLiteralCharacters Then
						If inString AndAlso aChar <> getChar(aChar) Then Throw New ParseException("Invalid character: " & AscW(aChar), index(0))
						index(0) = index(0) + 1
					End If
				ElseIf index(0) >= formatting.Length Then
					If placeholder IsNot Nothing AndAlso index(0) < placeholder.Length Then
						buff.Append(placeholder.Chars(index(0)))
					Else
						buff.Append(outerInstance.placeholderCharacter)
					End If
					index(0) = index(0) + 1
				ElseIf isValidCharacter(aChar) Then
					buff.Append(getChar(aChar))
					index(0) = index(0) + 1
				Else
					Throw New ParseException("Invalid character: " & AscW(aChar), index(0))
				End If
			End Sub
		End Class


		''' <summary>
		''' Used to represent a fixed character in the mask.
		''' </summary>
		Private Class LiteralCharacter
			Inherits MaskCharacter

			Private ReadOnly outerInstance As MaskFormatter

			Private fixedChar As Char

			Public Sub New(ByVal outerInstance As MaskFormatter, ByVal fixedChar As Char)
					Me.outerInstance = outerInstance
				Me.fixedChar = fixedChar
			End Sub

			Public Property Overrides literal As Boolean
				Get
					Return True
				End Get
			End Property

			Public Overrides Function getChar(ByVal aChar As Char) As Char
				Return fixedChar
			End Function
		End Class


		''' <summary>
		''' Represents a number, uses <code>Character.isDigit</code>.
		''' </summary>
		Private Class DigitMaskCharacter
			Inherits MaskCharacter

			Private ReadOnly outerInstance As MaskFormatter

			Public Sub New(ByVal outerInstance As MaskFormatter)
				Me.outerInstance = outerInstance
			End Sub

			Public Overrides Function isValidCharacter(ByVal aChar As Char) As Boolean
				Return (Char.IsDigit(aChar) AndAlso MyBase.isValidCharacter(aChar))
			End Function
		End Class


		''' <summary>
		''' Represents a character, lower case letters are mapped to upper case
		''' using <code>Character.toUpperCase</code>.
		''' </summary>
		Private Class UpperCaseCharacter
			Inherits MaskCharacter

			Private ReadOnly outerInstance As MaskFormatter

			Public Sub New(ByVal outerInstance As MaskFormatter)
				Me.outerInstance = outerInstance
			End Sub

			Public Overrides Function isValidCharacter(ByVal aChar As Char) As Boolean
				Return (Char.IsLetter(aChar) AndAlso MyBase.isValidCharacter(aChar))
			End Function

			Public Overrides Function getChar(ByVal aChar As Char) As Char
				Return Char.ToUpper(aChar)
			End Function
		End Class


		''' <summary>
		''' Represents a character, upper case letters are mapped to lower case
		''' using <code>Character.toLowerCase</code>.
		''' </summary>
		Private Class LowerCaseCharacter
			Inherits MaskCharacter

			Private ReadOnly outerInstance As MaskFormatter

			Public Sub New(ByVal outerInstance As MaskFormatter)
				Me.outerInstance = outerInstance
			End Sub

			Public Overrides Function isValidCharacter(ByVal aChar As Char) As Boolean
				Return (Char.IsLetter(aChar) AndAlso MyBase.isValidCharacter(aChar))
			End Function

			Public Overrides Function getChar(ByVal aChar As Char) As Char
				Return Char.ToLower(aChar)
			End Function
		End Class


		''' <summary>
		''' Represents either a character or digit, uses
		''' <code>Character.isLetterOrDigit</code>.
		''' </summary>
		Private Class AlphaNumericCharacter
			Inherits MaskCharacter

			Private ReadOnly outerInstance As MaskFormatter

			Public Sub New(ByVal outerInstance As MaskFormatter)
				Me.outerInstance = outerInstance
			End Sub

			Public Overrides Function isValidCharacter(ByVal aChar As Char) As Boolean
				Return (Char.IsLetterOrDigit(aChar) AndAlso MyBase.isValidCharacter(aChar))
			End Function
		End Class


		''' <summary>
		''' Represents a letter, uses <code>Character.isLetter</code>.
		''' </summary>
		Private Class CharCharacter
			Inherits MaskCharacter

			Private ReadOnly outerInstance As MaskFormatter

			Public Sub New(ByVal outerInstance As MaskFormatter)
				Me.outerInstance = outerInstance
			End Sub

			Public Overrides Function isValidCharacter(ByVal aChar As Char) As Boolean
				Return (Char.IsLetter(aChar) AndAlso MyBase.isValidCharacter(aChar))
			End Function
		End Class


		''' <summary>
		''' Represents a hex character, 0-9a-fA-F. a-f is mapped to A-F
		''' </summary>
		Private Class HexCharacter
			Inherits MaskCharacter

			Private ReadOnly outerInstance As MaskFormatter

			Public Sub New(ByVal outerInstance As MaskFormatter)
				Me.outerInstance = outerInstance
			End Sub

			Public Overrides Function isValidCharacter(ByVal aChar As Char) As Boolean
				Return ((aChar = "0"c OrElse aChar = "1"c OrElse aChar = "2"c OrElse aChar = "3"c OrElse aChar = "4"c OrElse aChar = "5"c OrElse aChar = "6"c OrElse aChar = "7"c OrElse aChar = "8"c OrElse aChar = "9"c OrElse aChar = "a"c OrElse aChar = "A"c OrElse aChar = "b"c OrElse aChar = "B"c OrElse aChar = "c"c OrElse aChar = "C"c OrElse aChar = "d"c OrElse aChar = "D"c OrElse aChar = "e"c OrElse aChar = "E"c OrElse aChar = "f"c OrElse aChar = "F"c) AndAlso MyBase.isValidCharacter(aChar))
			End Function

			Public Overrides Function getChar(ByVal aChar As Char) As Char
				If Char.IsDigit(aChar) Then Return aChar
				Return Char.ToUpper(aChar)
			End Function
		End Class
	End Class

End Namespace
Imports System
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
	''' <code>DefaultFormatter</code> formats arbitrary objects. Formatting is done
	''' by invoking the <code>toString</code> method. In order to convert the
	''' value back to a String, your class must provide a constructor that
	''' takes a String argument. If no single argument constructor that takes a
	''' String is found, the returned value will be the String passed into
	''' <code>stringToValue</code>.
	''' <p>
	''' Instances of <code>DefaultFormatter</code> can not be used in multiple
	''' instances of <code>JFormattedTextField</code>. To obtain a copy of
	''' an already configured <code>DefaultFormatter</code>, use the
	''' <code>clone</code> method.
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
	''' <seealso cref= javax.swing.JFormattedTextField.AbstractFormatter
	''' 
	''' @since 1.4 </seealso>
	<Serializable> _
	Public Class DefaultFormatter
		Inherits JFormattedTextField.AbstractFormatter
		Implements ICloneable

		''' <summary>
		''' Indicates if the value being edited must match the mask. </summary>
		Private allowsInvalid As Boolean

		''' <summary>
		''' If true, editing mode is in overwrite (or strikethough). </summary>
		Private overwriteMode As Boolean

		''' <summary>
		''' If true, any time a valid edit happens commitEdit is invoked. </summary>
		Private commitOnEdit As Boolean

		''' <summary>
		''' Class used to create new instances. </summary>
		Private valueClass As Type

		''' <summary>
		''' NavigationFilter that forwards calls back to DefaultFormatter. </summary>
		Private navigationFilter As NavigationFilter

		''' <summary>
		''' DocumentFilter that forwards calls back to DefaultFormatter. </summary>
		Private documentFilter As DocumentFilter

		''' <summary>
		''' Used during replace to track the region to replace. </summary>
		<NonSerialized> _
		Friend replaceHolder As ReplaceHolder


		''' <summary>
		''' Creates a DefaultFormatter.
		''' </summary>
		Public Sub New()
			overwriteMode = True
			allowsInvalid = True
		End Sub

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
		Public Overridable Sub install(ByVal ftf As JFormattedTextField)
			MyBase.install(ftf)
			positionCursorAtInitialLocation()
		End Sub

		''' <summary>
		''' Sets when edits are published back to the
		''' <code>JFormattedTextField</code>. If true, <code>commitEdit</code>
		''' is invoked after every valid edit (any time the text is edited). On
		''' the other hand, if this is false than the <code>DefaultFormatter</code>
		''' does not publish edits back to the <code>JFormattedTextField</code>.
		''' As such, the only time the value of the <code>JFormattedTextField</code>
		''' will change is when <code>commitEdit</code> is invoked on
		''' <code>JFormattedTextField</code>, typically when enter is pressed
		''' or focus leaves the <code>JFormattedTextField</code>.
		''' </summary>
		''' <param name="commit"> Used to indicate when edits are committed back to the
		'''               JTextComponent </param>
		Public Overridable Property commitsOnValidEdit As Boolean
			Set(ByVal commit As Boolean)
				commitOnEdit = commit
			End Set
			Get
				Return commitOnEdit
			End Get
		End Property


		''' <summary>
		''' Configures the behavior when inserting characters. If
		''' <code>overwriteMode</code> is true (the default), new characters
		''' overwrite existing characters in the model.
		''' </summary>
		''' <param name="overwriteMode"> Indicates if overwrite or overstrike mode is used </param>
		Public Overridable Property overwriteMode As Boolean
			Set(ByVal overwriteMode As Boolean)
				Me.overwriteMode = overwriteMode
			End Set
			Get
				Return overwriteMode
			End Get
		End Property


		''' <summary>
		''' Sets whether or not the value being edited is allowed to be invalid
		''' for a length of time (that is, <code>stringToValue</code> throws
		''' a <code>ParseException</code>).
		''' It is often convenient to allow the user to temporarily input an
		''' invalid value.
		''' </summary>
		''' <param name="allowsInvalid"> Used to indicate if the edited value must always
		'''        be valid </param>
		Public Overridable Property allowsInvalid As Boolean
			Set(ByVal allowsInvalid As Boolean)
				Me.allowsInvalid = allowsInvalid
			End Set
			Get
				Return allowsInvalid
			End Get
		End Property


		''' <summary>
		''' Sets that class that is used to create new Objects. If the
		''' passed in class does not have a single argument constructor that
		''' takes a String, String values will be used.
		''' </summary>
		''' <param name="valueClass"> Class used to construct return value from
		'''        stringToValue </param>
		Public Overridable Property valueClass As Type
			Set(ByVal valueClass As Type)
				Me.valueClass = valueClass
			End Set
			Get
				Return valueClass
			End Get
		End Property


		''' <summary>
		''' Converts the passed in String into an instance of
		''' <code>getValueClass</code> by way of the constructor that
		''' takes a String argument. If <code>getValueClass</code>
		''' returns null, the Class of the current value in the
		''' <code>JFormattedTextField</code> will be used. If this is null, a
		''' String will be returned. If the constructor throws an exception, a
		''' <code>ParseException</code> will be thrown. If there is no single
		''' argument String constructor, <code>string</code> will be returned.
		''' </summary>
		''' <exception cref="ParseException"> if there is an error in the conversion </exception>
		''' <param name="string"> String to convert </param>
		''' <returns> Object representation of text </returns>
		Public Overridable Function stringToValue(ByVal [string] As String) As Object
			Dim vc As Type = valueClass
			Dim ftf As JFormattedTextField = formattedTextField

			If vc Is Nothing AndAlso ftf IsNot Nothing Then
				Dim value As Object = ftf.value

				If value IsNot Nothing Then vc = value.GetType()
			End If
			If vc IsNot Nothing Then
				Dim cons As Constructor

				Try
					sun.reflect.misc.ReflectUtil.checkPackageAccess(vc)
					sun.swing.SwingUtilities2.checkAccess(vc.modifiers)
					cons = vc.GetConstructor(New Type(){GetType(String)})

				Catch nsme As NoSuchMethodException
					cons = Nothing
				End Try

				If cons IsNot Nothing Then
					Try
						sun.swing.SwingUtilities2.checkAccess(cons.modifiers)
						Return cons.newInstance(New Object() { [string] })
					Catch ex As Exception
						Throw New java.text.ParseException("Error creating instance", 0)
					End Try
				End If
			End If
			Return [string]
		End Function

		''' <summary>
		''' Converts the passed in Object into a String by way of the
		''' <code>toString</code> method.
		''' </summary>
		''' <exception cref="ParseException"> if there is an error in the conversion </exception>
		''' <param name="value"> Value to convert </param>
		''' <returns> String representation of value </returns>
		Public Overridable Function valueToString(ByVal value As Object) As String
			If value Is Nothing Then Return ""
			Return value.ToString()
		End Function

		''' <summary>
		''' Returns the <code>DocumentFilter</code> used to restrict the characters
		''' that can be input into the <code>JFormattedTextField</code>.
		''' </summary>
		''' <returns> DocumentFilter to restrict edits </returns>
		Protected Friend Overridable Property documentFilter As DocumentFilter
			Get
				If documentFilter Is Nothing Then documentFilter = New DefaultDocumentFilter(Me)
				Return documentFilter
			End Get
		End Property

		''' <summary>
		''' Returns the <code>NavigationFilter</code> used to restrict where the
		''' cursor can be placed.
		''' </summary>
		''' <returns> NavigationFilter to restrict navigation </returns>
		Protected Friend Overridable Property navigationFilter As NavigationFilter
			Get
				If navigationFilter Is Nothing Then navigationFilter = New DefaultNavigationFilter(Me)
				Return navigationFilter
			End Get
		End Property

		''' <summary>
		''' Creates a copy of the DefaultFormatter.
		''' </summary>
		''' <returns> copy of the DefaultFormatter </returns>
		Public Overridable Function clone() As Object
			Dim formatter As DefaultFormatter = CType(MyBase.clone(), DefaultFormatter)

			formatter.navigationFilter = Nothing
			formatter.documentFilter = Nothing
			formatter.replaceHolder = Nothing
			Return formatter
		End Function


		''' <summary>
		''' Positions the cursor at the initial location.
		''' </summary>
		Friend Overridable Sub positionCursorAtInitialLocation()
			Dim ftf As JFormattedTextField = formattedTextField
			If ftf IsNot Nothing Then ftf.caretPosition = initialVisualPosition
		End Sub

		''' <summary>
		''' Returns the initial location to position the cursor at. This forwards
		''' the call to <code>getNextNavigatableChar</code>.
		''' </summary>
		Friend Overridable Property initialVisualPosition As Integer
			Get
				Return getNextNavigatableChar(0, 1)
			End Get
		End Property

		''' <summary>
		''' Subclasses should override this if they want cursor navigation
		''' to skip certain characters. A return value of false indicates
		''' the character at <code>offset</code> should be skipped when
		''' navigating throught the field.
		''' </summary>
		Friend Overridable Function isNavigatable(ByVal offset As Integer) As Boolean
			Return True
		End Function

		''' <summary>
		''' Returns true if the text in <code>text</code> can be inserted.  This
		''' does not mean the text will ultimately be inserted, it is used if
		''' text can trivially reject certain characters.
		''' </summary>
		Friend Overridable Function isLegalInsertText(ByVal text As String) As Boolean
			Return True
		End Function

		''' <summary>
		''' Returns the next editable character starting at offset incrementing
		''' the offset by <code>direction</code>.
		''' </summary>
		Private Function getNextNavigatableChar(ByVal offset As Integer, ByVal direction As Integer) As Integer
			Dim max As Integer = formattedTextField.document.length

			Do While offset >= 0 AndAlso offset < max
				If isNavigatable(offset) Then Return offset
				offset += direction
			Loop
			Return offset
		End Function

		''' <summary>
		''' A convenience methods to return the result of deleting
		''' <code>deleteLength</code> characters at <code>offset</code>
		''' and inserting <code>replaceString</code> at <code>offset</code>
		''' in the current text field.
		''' </summary>
		Friend Overridable Function getReplaceString(ByVal offset As Integer, ByVal deleteLength As Integer, ByVal replaceString As String) As String
			Dim [string] As String = formattedTextField.text
			Dim result As String

			result = [string].Substring(0, offset)
			If replaceString IsNot Nothing Then result += replaceString
			If offset + deleteLength < [string].Length Then result += [string].Substring(offset + deleteLength)
			Return result
		End Function

	'    
	'     * Returns true if the operation described by <code>rh</code> will
	'     * result in a legal edit.  This may set the <code>value</code>
	'     * field of <code>rh</code>.
	'     
		Friend Overridable Function isValidEdit(ByVal rh As ReplaceHolder) As Boolean
			If Not allowsInvalid Then
				Dim newString As String = getReplaceString(rh.offset, rh.length, rh.text)

				Try
					rh.value = stringToValue(newString)

					Return True
				Catch pe As java.text.ParseException
					Return False
				End Try
			End If
			Return True
		End Function

		''' <summary>
		''' Invokes <code>commitEdit</code> on the JFormattedTextField.
		''' </summary>
		Friend Overridable Sub commitEdit()
			Dim ftf As JFormattedTextField = formattedTextField

			If ftf IsNot Nothing Then ftf.commitEdit()
		End Sub

		''' <summary>
		''' Pushes the value to the JFormattedTextField if the current value
		''' is valid and invokes <code>setEditValid</code> based on the
		''' validity of the value.
		''' </summary>
		Friend Overridable Sub updateValue()
			updateValue(Nothing)
		End Sub

		''' <summary>
		''' Pushes the <code>value</code> to the editor if we are to
		''' commit on edits. If <code>value</code> is null, the current value
		''' will be obtained from the text component.
		''' </summary>
		Friend Overridable Sub updateValue(ByVal value As Object)
			Try
				If value Is Nothing Then
					Dim [string] As String = formattedTextField.text

					value = stringToValue([string])
				End If

				If commitsOnValidEdit Then commitEdit()
				editValid = True
			Catch pe As java.text.ParseException
				editValid = False
			End Try
		End Sub

		''' <summary>
		''' Returns the next cursor position from offset by incrementing
		''' <code>direction</code>. This uses
		''' <code>getNextNavigatableChar</code>
		''' as well as constraining the location to the max position.
		''' </summary>
		Friend Overridable Function getNextCursorPosition(ByVal offset As Integer, ByVal direction As Integer) As Integer
			Dim newOffset As Integer = getNextNavigatableChar(offset, direction)
			Dim max As Integer = formattedTextField.document.length

			If Not allowsInvalid Then
				If direction = -1 AndAlso offset = newOffset Then
					' Case where hit backspace and only characters before
					' offset are fixed.
					newOffset = getNextNavigatableChar(newOffset, 1)
					If newOffset >= max Then newOffset = offset
				ElseIf direction = 1 AndAlso newOffset >= max Then
					' Don't go beyond last editable character.
					newOffset = getNextNavigatableChar(max - 1, -1)
					If newOffset < max Then newOffset += 1
				End If
			End If
			Return newOffset
		End Function

		''' <summary>
		''' Resets the cursor by using getNextCursorPosition.
		''' </summary>
		Friend Overridable Sub repositionCursor(ByVal offset As Integer, ByVal direction As Integer)
			formattedTextField.caret.dot = getNextCursorPosition(offset, direction)
		End Sub


		''' <summary>
		''' Finds the next navigable character.
		''' </summary>
		Friend Overridable Function getNextVisualPositionFrom(ByVal text As JTextComponent, ByVal pos As Integer, ByVal bias As Position.Bias, ByVal direction As Integer, ByVal biasRet As Position.Bias()) As Integer
			Dim value As Integer = text.uI.getNextVisualPositionFrom(text, pos, bias, direction, biasRet)

			If value = -1 Then Return -1
			If (Not allowsInvalid) AndAlso (direction = SwingConstants.EAST OrElse direction = SwingConstants.WEST) Then
				Dim last As Integer = -1

				Do While (Not isNavigatable(value)) AndAlso value <> last
					last = value
					value = text.uI.getNextVisualPositionFrom(text, value, bias, direction,biasRet)
				Loop
				Dim max As Integer = formattedTextField.document.length
				If last = value OrElse value = max Then
					If value = 0 Then
						biasRet(0) = Position.Bias.Forward
						value = initialVisualPosition
					End If
					If value >= max AndAlso max > 0 Then
						' Pending: should not assume forward!
						biasRet(0) = Position.Bias.Forward
						value = getNextNavigatableChar(max - 1, -1) + 1
					End If
				End If
			End If
			Return value
		End Function

		''' <summary>
		''' Returns true if the edit described by <code>rh</code> will result
		''' in a legal value.
		''' </summary>
		Friend Overridable Function canReplace(ByVal rh As ReplaceHolder) As Boolean
			Return isValidEdit(rh)
		End Function

		''' <summary>
		''' DocumentFilter method, funnels into <code>replace</code>.
		''' </summary>
		Friend Overridable Sub replace(ByVal fb As DocumentFilter.FilterBypass, ByVal offset As Integer, ByVal length As Integer, ByVal text As String, ByVal attrs As AttributeSet)
			Dim rh As ReplaceHolder = getReplaceHolder(fb, offset, length, text, attrs)

			replace(rh)
		End Sub

		''' <summary>
		''' If the edit described by <code>rh</code> is legal, this will
		''' return true, commit the edit (if necessary) and update the cursor
		''' position.  This forwards to <code>canReplace</code> and
		''' <code>isLegalInsertText</code> as necessary to determine if
		''' the edit is in fact legal.
		''' <p>
		''' All of the DocumentFilter methods funnel into here, you should
		''' generally only have to override this.
		''' </summary>
		Friend Overridable Function replace(ByVal rh As ReplaceHolder) As Boolean
			Dim valid As Boolean = True
			Dim direction As Integer = 1

			If rh.length > 0 AndAlso (rh.text Is Nothing OrElse rh.text.Length = 0) AndAlso (formattedTextField.selectionStart <> rh.offset OrElse rh.length > 1) Then direction = -1

			If overwriteMode AndAlso rh.text IsNot Nothing AndAlso formattedTextField.selectedText Is Nothing Then rh.length = Math.Min(Math.Max(rh.length, rh.text.Length), rh.fb.document.length - rh.offset)
			If (rh.text IsNot Nothing AndAlso (Not isLegalInsertText(rh.text))) OrElse (Not canReplace(rh)) OrElse (rh.length = 0 AndAlso (rh.text Is Nothing OrElse rh.text.Length = 0)) Then valid = False
			If valid Then
				Dim cursor As Integer = rh.cursorPosition

				rh.fb.replace(rh.offset, rh.length, rh.text, rh.attrs)
				If cursor = -1 Then
					cursor = rh.offset
					If direction = 1 AndAlso rh.text IsNot Nothing Then cursor = rh.offset + rh.text.Length
				End If
				updateValue(rh.value)
				repositionCursor(cursor, direction)
				Return True
			Else
				invalidEdit()
			End If
			Return False
		End Function

		''' <summary>
		''' NavigationFilter method, subclasses that wish finer control should
		''' override this.
		''' </summary>
		Friend Overridable Sub setDot(ByVal fb As NavigationFilter.FilterBypass, ByVal dot As Integer, ByVal bias As Position.Bias)
			fb.dotDot(dot, bias)
		End Sub

		''' <summary>
		''' NavigationFilter method, subclasses that wish finer control should
		''' override this.
		''' </summary>
		Friend Overridable Sub moveDot(ByVal fb As NavigationFilter.FilterBypass, ByVal dot As Integer, ByVal bias As Position.Bias)
			fb.moveDot(dot, bias)
		End Sub


		''' <summary>
		''' Returns the ReplaceHolder to track the replace of the specified
		''' text.
		''' </summary>
		Friend Overridable Function getReplaceHolder(ByVal fb As DocumentFilter.FilterBypass, ByVal offset As Integer, ByVal length As Integer, ByVal text As String, ByVal attrs As AttributeSet) As ReplaceHolder
			If replaceHolder Is Nothing Then replaceHolder = New ReplaceHolder
			replaceHolder.reset(fb, offset, length, text, attrs)
			Return replaceHolder
		End Function


		''' <summary>
		''' ReplaceHolder is used to track where insert/remove/replace is
		''' going to happen.
		''' </summary>
		Friend Class ReplaceHolder
			''' <summary>
			''' The FilterBypass that was passed to the DocumentFilter method. </summary>
			Friend fb As DocumentFilter.FilterBypass
			''' <summary>
			''' Offset where the remove/insert is going to occur. </summary>
			Friend offset As Integer
			''' <summary>
			''' Length of text to remove. </summary>
			Friend length As Integer
			''' <summary>
			''' The text to insert, may be null. </summary>
			Friend text As String
			''' <summary>
			''' AttributeSet to attach to text, may be null. </summary>
			Friend attrs As AttributeSet
			''' <summary>
			''' The resulting value, this may never be set. </summary>
			Friend value As Object
			''' <summary>
			''' Position the cursor should be adjusted from.  If this is -1
			''' the cursor position will be adjusted based on the direction of
			''' the replace (-1: offset, 1: offset + text.length()), otherwise
			''' the cursor position is adusted from this position.
			''' </summary>
			Friend cursorPosition As Integer

			Friend Overridable Sub reset(ByVal fb As DocumentFilter.FilterBypass, ByVal offset As Integer, ByVal length As Integer, ByVal text As String, ByVal attrs As AttributeSet)
				Me.fb = fb
				Me.offset = offset
				Me.length = length
				Me.text = text
				Me.attrs = attrs
				Me.value = Nothing
				cursorPosition = -1
			End Sub
		End Class


		''' <summary>
		''' NavigationFilter implementation that calls back to methods with
		''' same name in DefaultFormatter.
		''' </summary>
		<Serializable> _
		Private Class DefaultNavigationFilter
			Inherits NavigationFilter

			Private ReadOnly outerInstance As DefaultFormatter

			Public Sub New(ByVal outerInstance As DefaultFormatter)
				Me.outerInstance = outerInstance
			End Sub

			Public Overrides Sub setDot(ByVal fb As FilterBypass, ByVal dot As Integer, ByVal bias As Position.Bias)
				Dim tc As JTextComponent = outerInstance.formattedTextField
				If tc.composedTextExists() Then
					' bypass the filter
					fb.dotDot(dot, bias)
				Else
					outerInstance.dotDot(fb, dot, bias)
				End If
			End Sub

			Public Overrides Sub moveDot(ByVal fb As FilterBypass, ByVal dot As Integer, ByVal bias As Position.Bias)
				Dim tc As JTextComponent = outerInstance.formattedTextField
				If tc.composedTextExists() Then
					' bypass the filter
					fb.moveDot(dot, bias)
				Else
					outerInstance.moveDot(fb, dot, bias)
				End If
			End Sub

			Public Overrides Function getNextVisualPositionFrom(ByVal text As JTextComponent, ByVal pos As Integer, ByVal bias As Position.Bias, ByVal direction As Integer, ByVal biasRet As Position.Bias()) As Integer
				If text.composedTextExists() Then
					' forward the call to the UI directly
					Return text.uI.getNextVisualPositionFrom(text, pos, bias, direction, biasRet)
				Else
					Return outerInstance.getNextVisualPositionFrom(text, pos, bias, direction, biasRet)
				End If
			End Function
		End Class


		''' <summary>
		''' DocumentFilter implementation that calls back to the replace
		''' method of DefaultFormatter.
		''' </summary>
		<Serializable> _
		Private Class DefaultDocumentFilter
			Inherits DocumentFilter

			Private ReadOnly outerInstance As DefaultFormatter

			Public Sub New(ByVal outerInstance As DefaultFormatter)
				Me.outerInstance = outerInstance
			End Sub

			Public Overrides Sub remove(ByVal fb As FilterBypass, ByVal offset As Integer, ByVal length As Integer)
				Dim tc As JTextComponent = outerInstance.formattedTextField
				If tc.composedTextExists() Then
					' bypass the filter
					fb.remove(offset, length)
				Else
					outerInstance.replace(fb, offset, length, Nothing, Nothing)
				End If
			End Sub

			Public Overrides Sub insertString(ByVal fb As FilterBypass, ByVal offset As Integer, ByVal [string] As String, ByVal attr As AttributeSet)
				Dim tc As JTextComponent = outerInstance.formattedTextField
				If tc.composedTextExists() OrElse Utilities.isComposedTextAttributeDefined(attr) Then
					' bypass the filter
					fb.insertString(offset, [string], attr)
				Else
					outerInstance.replace(fb, offset, 0, [string], attr)
				End If
			End Sub

			Public Overrides Sub replace(ByVal fb As FilterBypass, ByVal offset As Integer, ByVal length As Integer, ByVal text As String, ByVal attr As AttributeSet)
				Dim tc As JTextComponent = outerInstance.formattedTextField
				If tc.composedTextExists() OrElse Utilities.isComposedTextAttributeDefined(attr) Then
					' bypass the filter
					fb.replace(offset, length, text, attr)
				Else
					outerInstance.replace(fb, offset, length, text, attr)
				End If
			End Sub
		End Class
	End Class

End Namespace
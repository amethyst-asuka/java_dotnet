Imports System
Imports javax.swing.text
Imports javax.swing.plaf
Imports javax.accessibility

'
' * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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
Namespace javax.swing


	''' <summary>
	''' <code>JPasswordField</code> is a lightweight component that allows
	''' the editing of a single line of text where the view indicates
	''' something was typed, but does not show the original characters.
	''' You can find further information and examples in
	''' <a href="https://docs.oracle.com/javase/tutorial/uiswing/components/textfield.html">How to Use Text Fields</a>,
	''' a section in <em>The Java Tutorial.</em>
	''' <p>
	''' <code>JPasswordField</code> is intended
	''' to be source-compatible with <code>java.awt.TextField</code>
	''' used with <code>echoChar</code> set.  It is provided separately
	''' to make it easier to safely change the UI for the
	''' <code>JTextField</code> without affecting password entries.
	''' <p>
	''' <strong>NOTE:</strong>
	''' By default, JPasswordField disables input methods; otherwise, input
	''' characters could be visible while they were composed using input methods.
	''' If an application needs the input methods support, please use the
	''' inherited method, <code>enableInputMethods(true)</code>.
	''' <p>
	''' <strong>Warning:</strong> Swing is not thread safe. For more
	''' information see <a
	''' href="package-summary.html#threading">Swing's Threading
	''' Policy</a>.
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
	''' @beaninfo
	'''  attribute: isContainer false
	''' description: Allows the editing of a line of text but doesn't show the characters.
	''' 
	''' @author  Timothy Prinzing
	''' </summary>
	Public Class JPasswordField
		Inherits JTextField

		''' <summary>
		''' Constructs a new <code>JPasswordField</code>,
		''' with a default document, <code>null</code> starting
		''' text string, and 0 column width.
		''' </summary>
		Public Sub New()
			Me.New(Nothing,Nothing,0)
		End Sub

		''' <summary>
		''' Constructs a new <code>JPasswordField</code> initialized
		''' with the specified text.  The document model is set to the
		''' default, and the number of columns to 0.
		''' </summary>
		''' <param name="text"> the text to be displayed, <code>null</code> if none </param>
		Public Sub New(ByVal text As String)
			Me.New(Nothing, text, 0)
		End Sub

		''' <summary>
		''' Constructs a new empty <code>JPasswordField</code> with the specified
		''' number of columns.  A default model is created, and the initial string
		''' is set to <code>null</code>.
		''' </summary>
		''' <param name="columns"> the number of columns &gt;= 0 </param>
		Public Sub New(ByVal columns As Integer)
			Me.New(Nothing, Nothing, columns)
		End Sub

		''' <summary>
		''' Constructs a new <code>JPasswordField</code> initialized with
		''' the specified text and columns.  The document model is set to
		''' the default.
		''' </summary>
		''' <param name="text"> the text to be displayed, <code>null</code> if none </param>
		''' <param name="columns"> the number of columns &gt;= 0 </param>
		Public Sub New(ByVal text As String, ByVal columns As Integer)
			Me.New(Nothing, text, columns)
		End Sub

		''' <summary>
		''' Constructs a new <code>JPasswordField</code> that uses the
		''' given text storage model and the given number of columns.
		''' This is the constructor through which the other constructors feed.
		''' The echo character is set to '*', but may be changed by the current
		''' Look and Feel.  If the document model is
		''' <code>null</code>, a default one will be created.
		''' </summary>
		''' <param name="doc">  the text storage to use </param>
		''' <param name="txt"> the text to be displayed, <code>null</code> if none </param>
		''' <param name="columns">  the number of columns to use to calculate
		'''   the preferred width &gt;= 0; if columns is set to zero, the
		'''   preferred width will be whatever naturally results from
		'''   the component implementation </param>
		Public Sub New(ByVal doc As Document, ByVal txt As String, ByVal columns As Integer)
			MyBase.New(doc, txt, columns)
			' We could either leave this on, which wouldn't be secure,
			' or obscure the composted text, which essentially makes displaying
			' it useless. Therefore, we turn off input methods.
			enableInputMethods(False)
		End Sub

		''' <summary>
		''' Returns the name of the L&amp;F class that renders this component.
		''' </summary>
		''' <returns> the string "PasswordFieldUI" </returns>
		''' <seealso cref= JComponent#getUIClassID </seealso>
		''' <seealso cref= UIDefaults#getUI </seealso>
		Public Property Overrides uIClassID As String
			Get
				Return uiClassID
			End Get
		End Property


		''' <summary>
		''' {@inheritDoc}
		''' @since 1.6
		''' </summary>
		Public Overrides Sub updateUI()
			If Not echoCharSet Then echoChar = "*"c
			MyBase.updateUI()
		End Sub

		''' <summary>
		''' Returns the character to be used for echoing.  The default is '*'.
		''' The default may be different depending on the currently running Look
		''' and Feel. For example, Metal/Ocean's default is a bullet character.
		''' </summary>
		''' <returns> the echo character, 0 if unset </returns>
		''' <seealso cref= #setEchoChar </seealso>
		''' <seealso cref= #echoCharIsSet </seealso>
		Public Overridable Property echoChar As Char
			Get
				Return echoChar
			End Get
			Set(ByVal c As Char)
				echoChar = c
				echoCharSet = True
				repaint()
				revalidate()
			End Set
		End Property


		''' <summary>
		''' Returns true if this <code>JPasswordField</code> has a character
		''' set for echoing.  A character is considered to be set if the echo
		''' character is not 0.
		''' </summary>
		''' <returns> true if a character is set for echoing </returns>
		''' <seealso cref= #setEchoChar </seealso>
		''' <seealso cref= #getEchoChar </seealso>
		Public Overridable Function echoCharIsSet() As Boolean
			Return AscW(echoChar) <> 0
		End Function

		' --- JTextComponent methods ----------------------------------

		''' <summary>
		''' Invokes <code>provideErrorFeedback</code> on the current
		''' look and feel, which typically initiates an error beep.
		''' The normal behavior of transferring the
		''' currently selected range in the associated text model
		''' to the system clipboard, and removing the contents from
		''' the model, is not acceptable for a password field.
		''' </summary>
		Public Overrides Sub cut()
			If getClientProperty("JPasswordField.cutCopyAllowed") IsNot Boolean.TRUE Then
				UIManager.lookAndFeel.provideErrorFeedback(Me)
			Else
				MyBase.cut()
			End If
		End Sub

		''' <summary>
		''' Invokes <code>provideErrorFeedback</code> on the current
		''' look and feel, which typically initiates an error beep.
		''' The normal behavior of transferring the
		''' currently selected range in the associated text model
		''' to the system clipboard, and leaving the contents from
		''' the model, is not acceptable for a password field.
		''' </summary>
		Public Overrides Sub copy()
			If getClientProperty("JPasswordField.cutCopyAllowed") IsNot Boolean.TRUE Then
				UIManager.lookAndFeel.provideErrorFeedback(Me)
			Else
				MyBase.copy()
			End If
		End Sub

		''' <summary>
		''' Returns the text contained in this <code>TextComponent</code>.
		''' If the underlying document is <code>null</code>, will give a
		''' <code>NullPointerException</code>.
		''' <p>
		''' For security reasons, this method is deprecated.  Use the
		''' <code>* getPassword</code> method instead. </summary>
		''' @deprecated As of Java 2 platform v1.2,
		''' replaced by <code>getPassword</code>. 
		''' <returns> the text </returns>
		<Obsolete("As of Java 2 platform v1.2,")> _
		Public Property Overrides text As String
			Get
				Return MyBase.text
			End Get
		End Property

		''' <summary>
		''' Fetches a portion of the text represented by the
		''' component.  Returns an empty string if length is 0.
		''' <p>
		''' For security reasons, this method is deprecated.  Use the
		''' <code>getPassword</code> method instead. </summary>
		''' @deprecated As of Java 2 platform v1.2,
		''' replaced by <code>getPassword</code>. 
		''' <param name="offs"> the offset &gt;= 0 </param>
		''' <param name="len"> the length &gt;= 0 </param>
		''' <returns> the text </returns>
		''' <exception cref="BadLocationException"> if the offset or length are invalid </exception>
		<Obsolete("As of Java 2 platform v1.2,")> _
		Public Overrides Function getText(ByVal offs As Integer, ByVal len As Integer) As String
			Return MyBase.getText(offs, len)
		End Function

		''' <summary>
		''' Returns the text contained in this <code>TextComponent</code>.
		''' If the underlying document is <code>null</code>, will give a
		''' <code>NullPointerException</code>.  For stronger
		''' security, it is recommended that the returned character array be
		''' cleared after use by setting each character to zero.
		''' </summary>
		''' <returns> the text </returns>
		Public Overridable Property password As Char()
			Get
				Dim doc As Document = document
				Dim txt As New Segment
				Try
					doc.getText(0, doc.length, txt) ' use the non-String API
				Catch e As BadLocationException
					Return Nothing
				End Try
				Dim retValue As Char() = New Char(txt.count - 1){}
				Array.Copy(txt.array, txt.offset, retValue, 0, txt.count)
				Return retValue
			End Get
		End Property

		''' <summary>
		''' See readObject() and writeObject() in JComponent for more
		''' information about serialization in Swing.
		''' </summary>
		Private Sub writeObject(ByVal s As java.io.ObjectOutputStream)
			s.defaultWriteObject()
			If uIClassID.Equals(uiClassID) Then
				Dim count As SByte = JComponent.getWriteObjCounter(Me)
				count -= 1
				JComponent.writeObjCounterter(Me, count)
				If count = 0 AndAlso ui IsNot Nothing Then ui.installUI(Me)
			End If
		End Sub

		' --- variables -----------------------------------------------

		''' <seealso cref= #getUIClassID </seealso>
		''' <seealso cref= #readObject </seealso>
		Private Const uiClassID As String = "PasswordFieldUI"

		Private echoChar As Char

		Private echoCharSet As Boolean = False


		''' <summary>
		''' Returns a string representation of this <code>JPasswordField</code>.
		''' This method is intended to be used only for debugging purposes, and the
		''' content and format of the returned string may vary between
		''' implementations. The returned string may be empty but may not
		''' be <code>null</code>.
		''' </summary>
		''' <returns>  a string representation of this <code>JPasswordField</code> </returns>
		Protected Friend Overrides Function paramString() As String
			Return MyBase.paramString() & ",echoChar=" & AscW(echoChar)
		End Function


		''' <summary>
		''' This method is a hack to get around the fact that we cannot
		''' directly override setUIProperty because part of the inheritance hierarchy
		''' goes outside of the javax.swing package, and therefore calling a package
		''' private method isn't allowed. This method should return true if the property
		''' was handled, and false otherwise.
		''' </summary>
		Friend Overridable Function customSetUIProperty(ByVal propertyName As String, ByVal value As Object) As Boolean
			If propertyName = "echoChar" Then
				If Not echoCharSet Then
					echoChar = CChar(value)
					echoCharSet = False
				End If
				Return True
			End If
			Return False
		End Function

	'///////////////
	' Accessibility support
	'//////////////


		''' <summary>
		''' Returns the <code>AccessibleContext</code> associated with this
		''' <code>JPasswordField</code>. For password fields, the
		''' <code>AccessibleContext</code> takes the form of an
		''' <code>AccessibleJPasswordField</code>.
		''' A new <code>AccessibleJPasswordField</code> instance is created
		''' if necessary.
		''' </summary>
		''' <returns> an <code>AccessibleJPasswordField</code> that serves as the
		'''         <code>AccessibleContext</code> of this
		'''         <code>JPasswordField</code> </returns>
		Public Property Overrides accessibleContext As AccessibleContext
			Get
				If accessibleContext Is Nothing Then accessibleContext = New AccessibleJPasswordField(Me)
				Return accessibleContext
			End Get
		End Property

		''' <summary>
		''' This class implements accessibility support for the
		''' <code>JPasswordField</code> class.  It provides an implementation of the
		''' Java Accessibility API appropriate to password field user-interface
		''' elements.
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
		Protected Friend Class AccessibleJPasswordField
			Inherits AccessibleJTextField

			Private ReadOnly outerInstance As JPasswordField

			Public Sub New(ByVal outerInstance As JPasswordField)
				Me.outerInstance = outerInstance
			End Sub


			''' <summary>
			''' Gets the role of this object.
			''' </summary>
			''' <returns> an instance of AccessibleRole describing the role of the
			'''   object (AccessibleRole.PASSWORD_TEXT) </returns>
			''' <seealso cref= AccessibleRole </seealso>
			Public Overridable Property accessibleRole As AccessibleRole
				Get
					Return AccessibleRole.PASSWORD_TEXT
				End Get
			End Property

			''' <summary>
			''' Gets the <code>AccessibleText</code> for the <code>JPasswordField</code>.
			''' The returned object also implements the
			''' <code>AccessibleExtendedText</code> interface.
			''' </summary>
			''' <returns> <code>AccessibleText</code> for the JPasswordField </returns>
			''' <seealso cref= javax.accessibility.AccessibleContext </seealso>
			''' <seealso cref= javax.accessibility.AccessibleContext#getAccessibleText </seealso>
			''' <seealso cref= javax.accessibility.AccessibleText </seealso>
			''' <seealso cref= javax.accessibility.AccessibleExtendedText
			''' 
			''' @since 1.6 </seealso>
			Public Overridable Property accessibleText As AccessibleText
				Get
					Return Me
				End Get
			End Property

	'        
	'         * Returns a String filled with password echo characters. The String
	'         * contains one echo character for each character (including whitespace)
	'         * that the user entered in the JPasswordField.
	'         
			Private Function getEchoString(ByVal str As String) As String
				If str Is Nothing Then Return Nothing
				Dim buffer As Char() = New Char(str.Length - 1){}
				java.util.Arrays.fill(buffer, outerInstance.echoChar)
				Return New String(buffer)
			End Function

			''' <summary>
			''' Returns the <code>String</code> at a given <code>index</code>.
			''' </summary>
			''' <param name="part"> the <code>CHARACTER</code>, <code>WORD</code> or
			''' <code>SENTENCE</code> to retrieve </param>
			''' <param name="index"> an index within the text </param>
			''' <returns> a <code>String</code> if <code>part</code> and
			''' <code>index</code> are valid.
			''' Otherwise, <code>null</code> is returned
			''' </returns>
			''' <seealso cref= javax.accessibility.AccessibleText#CHARACTER </seealso>
			''' <seealso cref= javax.accessibility.AccessibleText#WORD </seealso>
			''' <seealso cref= javax.accessibility.AccessibleText#SENTENCE
			''' 
			''' @since 1.6 </seealso>
			Public Overridable Function getAtIndex(ByVal part As Integer, ByVal index As Integer) As String
			   Dim str As String = Nothing
				If part = AccessibleText.CHARACTER Then
					str = MyBase.getAtIndex(part, index)
				Else
					' Treat the text displayed in the JPasswordField
					' as one word and sentence.
					Dim password As Char() = outerInstance.password
					If password Is Nothing OrElse index < 0 OrElse index >= password.Length Then Return Nothing
					str = New String(password)
				End If
				Return getEchoString(str)
			End Function

			''' <summary>
			''' Returns the <code>String</code> after a given <code>index</code>.
			''' </summary>
			''' <param name="part"> the <code>CHARACTER</code>, <code>WORD</code> or
			''' <code>SENTENCE</code> to retrieve </param>
			''' <param name="index"> an index within the text </param>
			''' <returns> a <code>String</code> if <code>part</code> and
			''' <code>index</code> are valid.
			''' Otherwise, <code>null</code> is returned
			''' </returns>
			''' <seealso cref= javax.accessibility.AccessibleText#CHARACTER </seealso>
			''' <seealso cref= javax.accessibility.AccessibleText#WORD </seealso>
			''' <seealso cref= javax.accessibility.AccessibleText#SENTENCE
			''' 
			''' @since 1.6 </seealso>
			Public Overridable Function getAfterIndex(ByVal part As Integer, ByVal index As Integer) As String
				If part = AccessibleText.CHARACTER Then
					Dim str As String = MyBase.getAfterIndex(part, index)
					Return getEchoString(str)
				Else
					' There is no word or sentence after the text
					' displayed in the JPasswordField.
					Return Nothing
				End If
			End Function

			''' <summary>
			''' Returns the <code>String</code> before a given <code>index</code>.
			''' </summary>
			''' <param name="part"> the <code>CHARACTER</code>, <code>WORD</code> or
			''' <code>SENTENCE</code> to retrieve </param>
			''' <param name="index"> an index within the text </param>
			''' <returns> a <code>String</code> if <code>part</code> and
			''' <code>index</code> are valid.
			''' Otherwise, <code>null</code> is returned
			''' </returns>
			''' <seealso cref= javax.accessibility.AccessibleText#CHARACTER </seealso>
			''' <seealso cref= javax.accessibility.AccessibleText#WORD </seealso>
			''' <seealso cref= javax.accessibility.AccessibleText#SENTENCE
			''' 
			''' @since 1.6 </seealso>
			Public Overridable Function getBeforeIndex(ByVal part As Integer, ByVal index As Integer) As String
				If part = AccessibleText.CHARACTER Then
					Dim str As String = MyBase.getBeforeIndex(part, index)
					Return getEchoString(str)
				Else
					' There is no word or sentence before the text
					' displayed in the JPasswordField.
					Return Nothing
				End If
			End Function

			''' <summary>
			''' Returns the text between two <code>indices</code>.
			''' </summary>
			''' <param name="startIndex"> the start index in the text </param>
			''' <param name="endIndex"> the end index in the text </param>
			''' <returns> the text string if the indices are valid.
			''' Otherwise, <code>null</code> is returned
			''' 
			''' @since 1.6 </returns>
			Public Overridable Function getTextRange(ByVal startIndex As Integer, ByVal endIndex As Integer) As String
				Dim str As String = MyBase.getTextRange(startIndex, endIndex)
				Return getEchoString(str)
			End Function


			''' <summary>
			''' Returns the <code>AccessibleTextSequence</code> at a given
			''' <code>index</code>.
			''' </summary>
			''' <param name="part"> the <code>CHARACTER</code>, <code>WORD</code>,
			''' <code>SENTENCE</code>, <code>LINE</code> or <code>ATTRIBUTE_RUN</code> to
			''' retrieve </param>
			''' <param name="index"> an index within the text </param>
			''' <returns> an <code>AccessibleTextSequence</code> specifying the text if
			''' <code>part</code> and <code>index</code> are valid.  Otherwise,
			''' <code>null</code> is returned
			''' </returns>
			''' <seealso cref= javax.accessibility.AccessibleText#CHARACTER </seealso>
			''' <seealso cref= javax.accessibility.AccessibleText#WORD </seealso>
			''' <seealso cref= javax.accessibility.AccessibleText#SENTENCE </seealso>
			''' <seealso cref= javax.accessibility.AccessibleExtendedText#LINE </seealso>
			''' <seealso cref= javax.accessibility.AccessibleExtendedText#ATTRIBUTE_RUN
			''' 
			''' @since 1.6 </seealso>
			Public Overridable Function getTextSequenceAt(ByVal part As Integer, ByVal index As Integer) As AccessibleTextSequence
				If part = AccessibleText.CHARACTER Then
					Dim seq As AccessibleTextSequence = MyBase.getTextSequenceAt(part, index)
					If seq Is Nothing Then Return Nothing
					Return New AccessibleTextSequence(seq.startIndex, seq.endIndex, getEchoString(seq.text))
				Else
					' Treat the text displayed in the JPasswordField
					' as one word, sentence, line and attribute run
					Dim password As Char() = outerInstance.password
					If password Is Nothing OrElse index < 0 OrElse index >= password.Length Then Return Nothing
					Dim text As New String(password)
					Return New AccessibleTextSequence(0, password.Length - 1, getEchoString(text))
				End If
			End Function

			''' <summary>
			''' Returns the <code>AccessibleTextSequence</code> after a given
			''' <code>index</code>.
			''' </summary>
			''' <param name="part"> the <code>CHARACTER</code>, <code>WORD</code>,
			''' <code>SENTENCE</code>, <code>LINE</code> or <code>ATTRIBUTE_RUN</code> to
			''' retrieve </param>
			''' <param name="index"> an index within the text </param>
			''' <returns> an <code>AccessibleTextSequence</code> specifying the text if
			''' <code>part</code> and <code>index</code> are valid.  Otherwise,
			''' <code>null</code> is returned
			''' </returns>
			''' <seealso cref= javax.accessibility.AccessibleText#CHARACTER </seealso>
			''' <seealso cref= javax.accessibility.AccessibleText#WORD </seealso>
			''' <seealso cref= javax.accessibility.AccessibleText#SENTENCE </seealso>
			''' <seealso cref= javax.accessibility.AccessibleExtendedText#LINE </seealso>
			''' <seealso cref= javax.accessibility.AccessibleExtendedText#ATTRIBUTE_RUN
			''' 
			''' @since 1.6 </seealso>
			Public Overridable Function getTextSequenceAfter(ByVal part As Integer, ByVal index As Integer) As AccessibleTextSequence
				If part = AccessibleText.CHARACTER Then
					Dim seq As AccessibleTextSequence = MyBase.getTextSequenceAfter(part, index)
					If seq Is Nothing Then Return Nothing
					Return New AccessibleTextSequence(seq.startIndex, seq.endIndex, getEchoString(seq.text))
				Else
					' There is no word, sentence, line or attribute run
					' after the text displayed in the JPasswordField.
					Return Nothing
				End If
			End Function

			''' <summary>
			''' Returns the <code>AccessibleTextSequence</code> before a given
			''' <code>index</code>.
			''' </summary>
			''' <param name="part"> the <code>CHARACTER</code>, <code>WORD</code>,
			''' <code>SENTENCE</code>, <code>LINE</code> or <code>ATTRIBUTE_RUN</code> to
			''' retrieve </param>
			''' <param name="index"> an index within the text </param>
			''' <returns> an <code>AccessibleTextSequence</code> specifying the text if
			''' <code>part</code> and <code>index</code> are valid.  Otherwise,
			''' <code>null</code> is returned
			''' </returns>
			''' <seealso cref= javax.accessibility.AccessibleText#CHARACTER </seealso>
			''' <seealso cref= javax.accessibility.AccessibleText#WORD </seealso>
			''' <seealso cref= javax.accessibility.AccessibleText#SENTENCE </seealso>
			''' <seealso cref= javax.accessibility.AccessibleExtendedText#LINE </seealso>
			''' <seealso cref= javax.accessibility.AccessibleExtendedText#ATTRIBUTE_RUN
			''' 
			''' @since 1.6 </seealso>
			Public Overridable Function getTextSequenceBefore(ByVal part As Integer, ByVal index As Integer) As AccessibleTextSequence
				If part = AccessibleText.CHARACTER Then
					Dim seq As AccessibleTextSequence = MyBase.getTextSequenceBefore(part, index)
					If seq Is Nothing Then Return Nothing
					Return New AccessibleTextSequence(seq.startIndex, seq.endIndex, getEchoString(seq.text))
				Else
					' There is no word, sentence, line or attribute run
					' before the text displayed in the JPasswordField.
					Return Nothing
				End If
			End Function
		End Class
	End Class

End Namespace
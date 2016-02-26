Imports System
Imports javax.swing.event
Imports javax.swing.text

'
' * Copyright (c) 2000, 2015, Oracle and/or its affiliates. All rights reserved.
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
	''' <code>JFormattedTextField</code> extends <code>JTextField</code> adding
	''' support for formatting arbitrary values, as well as retrieving a particular
	''' object once the user has edited the text. The following illustrates
	''' configuring a <code>JFormattedTextField</code> to edit dates:
	''' <pre>
	'''   JFormattedTextField ftf = new JFormattedTextField();
	'''   ftf.setValue(new Date());
	''' </pre>
	''' <p>
	''' Once a <code>JFormattedTextField</code> has been created, you can
	''' listen for editing changes by way of adding
	''' a <code>PropertyChangeListener</code> and listening for
	''' <code>PropertyChangeEvent</code>s with the property name <code>value</code>.
	''' <p>
	''' <code>JFormattedTextField</code> allows
	''' configuring what action should be taken when focus is lost. The possible
	''' configurations are:
	''' <table summary="Possible JFormattedTextField configurations and their descriptions">
	''' <tr><th><p style="text-align:left">Value</p></th><th><p style="text-align:left">Description</p></th></tr>
	''' <tr><td>JFormattedTextField.REVERT
	'''            <td>Revert the display to match that of <code>getValue</code>,
	'''                possibly losing the current edit.
	'''        <tr><td>JFormattedTextField.COMMIT
	'''            <td>Commits the current value. If the value being edited
	'''                isn't considered a legal value by the
	'''                <code>AbstractFormatter</code> that is, a
	'''                <code>ParseException</code> is thrown, then the value
	'''                will not change, and then edited value will persist.
	'''        <tr><td>JFormattedTextField.COMMIT_OR_REVERT
	'''            <td>Similar to <code>COMMIT</code>, but if the value isn't
	'''                legal, behave like <code>REVERT</code>.
	'''        <tr><td>JFormattedTextField.PERSIST
	'''            <td>Do nothing, don't obtain a new
	'''                <code>AbstractFormatter</code>, and don't update the value.
	''' </table>
	''' The default is <code>JFormattedTextField.COMMIT_OR_REVERT</code>,
	''' refer to <seealso cref="#setFocusLostBehavior"/> for more information on this.
	''' <p>
	''' <code>JFormattedTextField</code> allows the focus to leave, even if
	''' the currently edited value is invalid. To lock the focus down while the
	''' <code>JFormattedTextField</code> is an invalid edit state
	''' you can attach an <code>InputVerifier</code>. The following code snippet
	''' shows a potential implementation of such an <code>InputVerifier</code>:
	''' <pre>
	''' public class FormattedTextFieldVerifier extends InputVerifier {
	'''     public boolean verify(JComponent input) {
	'''         if (input instanceof JFormattedTextField) {
	'''             JFormattedTextField ftf = (JFormattedTextField)input;
	'''             AbstractFormatter formatter = ftf.getFormatter();
	'''             if (formatter != null) {
	'''                 String text = ftf.getText();
	'''                 try {
	'''                      formatter.stringToValue(text);
	'''                      return true;
	'''                  } catch (ParseException pe) {
	'''                      return false;
	'''                  }
	'''              }
	'''          }
	'''          return true;
	'''      }
	'''      public boolean shouldYieldFocus(JComponent input) {
	'''          return verify(input);
	'''      }
	'''  }
	''' </pre>
	''' <p>
	''' Alternatively, you could invoke <code>commitEdit</code>, which would also
	''' commit the value.
	''' <p>
	''' <code>JFormattedTextField</code> does not do the formatting it self,
	''' rather formatting is done through an instance of
	''' <code>JFormattedTextField.AbstractFormatter</code> which is obtained from
	''' an instance of <code>JFormattedTextField.AbstractFormatterFactory</code>.
	''' Instances of <code>JFormattedTextField.AbstractFormatter</code> are
	''' notified when they become active by way of the
	''' <code>install</code> method, at which point the
	''' <code>JFormattedTextField.AbstractFormatter</code> can install whatever
	''' it needs to, typically a <code>DocumentFilter</code>. Similarly when
	''' <code>JFormattedTextField</code> no longer
	''' needs the <code>AbstractFormatter</code>, it will invoke
	''' <code>uninstall</code>.
	''' <p>
	''' <code>JFormattedTextField</code> typically
	''' queries the <code>AbstractFormatterFactory</code> for an
	''' <code>AbstractFormat</code> when it gains or loses focus. Although this
	''' can change based on the focus lost policy. If the focus lost
	''' policy is <code>JFormattedTextField.PERSIST</code>
	''' and the <code>JFormattedTextField</code> has been edited, the
	''' <code>AbstractFormatterFactory</code> will not be queried until the
	''' value has been committed. Similarly if the focus lost policy is
	''' <code>JFormattedTextField.COMMIT</code> and an exception
	''' is thrown from <code>stringToValue</code>, the
	''' <code>AbstractFormatterFactory</code> will not be queried when focus is
	''' lost or gained.
	''' <p>
	''' <code>JFormattedTextField.AbstractFormatter</code>
	''' is also responsible for determining when values are committed to
	''' the <code>JFormattedTextField</code>. Some
	''' <code>JFormattedTextField.AbstractFormatter</code>s will make new values
	''' available on every edit, and others will never commit the value. You can
	''' force the current value to be obtained
	''' from the current <code>JFormattedTextField.AbstractFormatter</code>
	''' by way of invoking <code>commitEdit</code>. <code>commitEdit</code> will
	''' be invoked whenever return is pressed in the
	''' <code>JFormattedTextField</code>.
	''' <p>
	''' If an <code>AbstractFormatterFactory</code> has not been explicitly
	''' set, one will be set based on the <code>Class</code> of the value type after
	''' <code>setValue</code> has been invoked (assuming value is non-null).
	''' For example, in the following code an appropriate
	''' <code>AbstractFormatterFactory</code> and <code>AbstractFormatter</code>
	''' will be created to handle formatting of numbers:
	''' <pre>
	'''   JFormattedTextField tf = new JFormattedTextField();
	'''   tf.setValue(100);
	''' </pre>
	''' <p>
	''' <strong>Warning:</strong> As the <code>AbstractFormatter</code> will
	''' typically install a <code>DocumentFilter</code> on the
	''' <code>Document</code>, and a <code>NavigationFilter</code> on the
	''' <code>JFormattedTextField</code> you should not install your own. If you do,
	''' you are likely to see odd behavior in that the editing policy of the
	''' <code>AbstractFormatter</code> will not be enforced.
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
	''' @since 1.4
	''' </summary>
	Public Class JFormattedTextField
		Inherits JTextField

		Private Const uiClassID As String = "FormattedTextFieldUI"
		Private Shared ReadOnly defaultActions As Action() = { New CommitAction, New CancelAction }

		''' <summary>
		''' Constant identifying that when focus is lost,
		''' <code>commitEdit</code> should be invoked. If in committing the
		''' new value a <code>ParseException</code> is thrown, the invalid
		''' value will remain.
		''' </summary>
		''' <seealso cref= #setFocusLostBehavior </seealso>
		Public Const COMMIT As Integer = 0

		''' <summary>
		''' Constant identifying that when focus is lost,
		''' <code>commitEdit</code> should be invoked. If in committing the new
		''' value a <code>ParseException</code> is thrown, the value will be
		''' reverted.
		''' </summary>
		''' <seealso cref= #setFocusLostBehavior </seealso>
		Public Const COMMIT_OR_REVERT As Integer = 1

		''' <summary>
		''' Constant identifying that when focus is lost, editing value should
		''' be reverted to current value set on the
		''' <code>JFormattedTextField</code>.
		''' </summary>
		''' <seealso cref= #setFocusLostBehavior </seealso>
		Public Const REVERT As Integer = 2

		''' <summary>
		''' Constant identifying that when focus is lost, the edited value
		''' should be left.
		''' </summary>
		''' <seealso cref= #setFocusLostBehavior </seealso>
		Public Const PERSIST As Integer = 3


		''' <summary>
		''' Factory used to obtain an instance of AbstractFormatter.
		''' </summary>
		Private factory As AbstractFormatterFactory
		''' <summary>
		''' Object responsible for formatting the current value.
		''' </summary>
		Private format As AbstractFormatter
		''' <summary>
		''' Last valid value.
		''' </summary>
		Private value As Object
		''' <summary>
		''' True while the value being edited is valid.
		''' </summary>
		Private editValid As Boolean
		''' <summary>
		''' Behavior when focus is lost.
		''' </summary>
		Private focusLostBehavior As Integer
		''' <summary>
		''' Indicates the current value has been edited.
		''' </summary>
		Private edited As Boolean
		''' <summary>
		''' Used to set the dirty state.
		''' </summary>
		Private documentListener As DocumentListener
		''' <summary>
		''' Masked used to set the AbstractFormatterFactory.
		''' </summary>
		Private mask As Object
		''' <summary>
		''' ActionMap that the TextFormatter Actions are added to.
		''' </summary>
		Private textFormatterActionMap As ActionMap
		''' <summary>
		''' Indicates the input method composed text is in the document
		''' </summary>
		Private composedTextExists As Boolean = False
		''' <summary>
		''' A handler for FOCUS_LOST event
		''' </summary>
		Private focusLostHandler As FocusLostHandler


		''' <summary>
		''' Creates a <code>JFormattedTextField</code> with no
		''' <code>AbstractFormatterFactory</code>. Use <code>setMask</code> or
		''' <code>setFormatterFactory</code> to configure the
		''' <code>JFormattedTextField</code> to edit a particular type of
		''' value.
		''' </summary>
		Public Sub New()
			MyBase.New()
			enableEvents(AWTEvent.FOCUS_EVENT_MASK)
			focusLostBehavior = COMMIT_OR_REVERT
		End Sub

		''' <summary>
		''' Creates a JFormattedTextField with the specified value. This will
		''' create an <code>AbstractFormatterFactory</code> based on the
		''' type of <code>value</code>.
		''' </summary>
		''' <param name="value"> Initial value for the JFormattedTextField </param>
		Public Sub New(ByVal value As Object)
			Me.New()
			value = value
		End Sub

		''' <summary>
		''' Creates a <code>JFormattedTextField</code>. <code>format</code> is
		''' wrapped in an appropriate <code>AbstractFormatter</code> which is
		''' then wrapped in an <code>AbstractFormatterFactory</code>.
		''' </summary>
		''' <param name="format"> Format used to look up an AbstractFormatter </param>
		Public Sub New(ByVal format As java.text.Format)
			Me.New()
			formatterFactory = getDefaultFormatterFactory(format)
		End Sub

		''' <summary>
		''' Creates a <code>JFormattedTextField</code> with the specified
		''' <code>AbstractFormatter</code>. The <code>AbstractFormatter</code>
		''' is placed in an <code>AbstractFormatterFactory</code>.
		''' </summary>
		''' <param name="formatter"> AbstractFormatter to use for formatting. </param>
		Public Sub New(ByVal formatter As AbstractFormatter)
			Me.New(New DefaultFormatterFactory(formatter))
		End Sub

		''' <summary>
		''' Creates a <code>JFormattedTextField</code> with the specified
		''' <code>AbstractFormatterFactory</code>.
		''' </summary>
		''' <param name="factory"> AbstractFormatterFactory used for formatting. </param>
		Public Sub New(ByVal factory As AbstractFormatterFactory)
			Me.New()
			formatterFactory = factory
		End Sub

		''' <summary>
		''' Creates a <code>JFormattedTextField</code> with the specified
		''' <code>AbstractFormatterFactory</code> and initial value.
		''' </summary>
		''' <param name="factory"> <code>AbstractFormatterFactory</code> used for
		'''        formatting. </param>
		''' <param name="currentValue"> Initial value to use </param>
		Public Sub New(ByVal factory As AbstractFormatterFactory, ByVal currentValue As Object)
			Me.New(currentValue)
			formatterFactory = factory
		End Sub

		''' <summary>
		''' Sets the behavior when focus is lost. This will be one of
		''' <code>JFormattedTextField.COMMIT_OR_REVERT</code>,
		''' <code>JFormattedTextField.REVERT</code>,
		''' <code>JFormattedTextField.COMMIT</code> or
		''' <code>JFormattedTextField.PERSIST</code>
		''' Note that some <code>AbstractFormatter</code>s may push changes as
		''' they occur, so that the value of this will have no effect.
		''' <p>
		''' This will throw an <code>IllegalArgumentException</code> if the object
		''' passed in is not one of the afore mentioned values.
		''' <p>
		''' The default value of this property is
		''' <code>JFormattedTextField.COMMIT_OR_REVERT</code>.
		''' </summary>
		''' <param name="behavior"> Identifies behavior when focus is lost </param>
		''' <exception cref="IllegalArgumentException"> if behavior is not one of the known
		'''         values
		''' @beaninfo
		'''  enum: COMMIT         JFormattedTextField.COMMIT
		'''        COMMIT_OR_REVERT JFormattedTextField.COMMIT_OR_REVERT
		'''        REVERT         JFormattedTextField.REVERT
		'''        PERSIST        JFormattedTextField.PERSIST
		'''  description: Behavior when component loses focus </exception>
		Public Overridable Property focusLostBehavior As Integer
			Set(ByVal behavior As Integer)
				If behavior <> COMMIT AndAlso behavior <> COMMIT_OR_REVERT AndAlso behavior <> PERSIST AndAlso behavior <> REVERT Then Throw New System.ArgumentException("setFocusLostBehavior must be one of: JFormattedTextField.COMMIT, JFormattedTextField.COMMIT_OR_REVERT, JFormattedTextField.PERSIST or JFormattedTextField.REVERT")
				focusLostBehavior = behavior
			End Set
			Get
				Return focusLostBehavior
			End Get
		End Property


		''' <summary>
		''' Sets the <code>AbstractFormatterFactory</code>.
		''' <code>AbstractFormatterFactory</code> is
		''' able to return an instance of <code>AbstractFormatter</code> that is
		''' used to format a value for display, as well an enforcing an editing
		''' policy.
		''' <p>
		''' If you have not explicitly set an <code>AbstractFormatterFactory</code>
		''' by way of this method (or a constructor) an
		''' <code>AbstractFormatterFactory</code> and consequently an
		''' <code>AbstractFormatter</code> will be used based on the
		''' <code>Class</code> of the value. <code>NumberFormatter</code> will
		''' be used for <code>Number</code>s, <code>DateFormatter</code> will
		''' be used for <code>Dates</code>, otherwise <code>DefaultFormatter</code>
		''' will be used.
		''' <p>
		''' This is a JavaBeans bound property.
		''' </summary>
		''' <param name="tf"> <code>AbstractFormatterFactory</code> used to lookup
		'''          instances of <code>AbstractFormatter</code>
		''' @beaninfo
		'''       bound: true
		'''   attribute: visualUpdate true
		''' description: AbstractFormatterFactory, responsible for returning an
		'''              AbstractFormatter that can format the current value. </param>
		Public Overridable Property formatterFactory As AbstractFormatterFactory
			Set(ByVal tf As AbstractFormatterFactory)
				Dim oldFactory As AbstractFormatterFactory = factory
    
				factory = tf
				firePropertyChange("formatterFactory", oldFactory, tf)
				valuelue(value, True, False)
			End Set
			Get
				Return factory
			End Get
		End Property


		''' <summary>
		''' Sets the current <code>AbstractFormatter</code>.
		''' <p>
		''' You should not normally invoke this, instead set the
		''' <code>AbstractFormatterFactory</code> or set the value.
		''' <code>JFormattedTextField</code> will
		''' invoke this as the state of the <code>JFormattedTextField</code>
		''' changes and requires the value to be reset.
		''' <code>JFormattedTextField</code> passes in the
		''' <code>AbstractFormatter</code> obtained from the
		''' <code>AbstractFormatterFactory</code>.
		''' <p>
		''' This is a JavaBeans bound property.
		''' </summary>
		''' <seealso cref= #setFormatterFactory </seealso>
		''' <param name="format"> AbstractFormatter to use for formatting
		''' @beaninfo
		'''       bound: true
		'''   attribute: visualUpdate true
		''' description: TextFormatter, responsible for formatting the current value </param>
		Protected Friend Overridable Property formatter As AbstractFormatter
			Set(ByVal format As AbstractFormatter)
				Dim oldFormat As AbstractFormatter = Me.format
    
				If oldFormat IsNot Nothing Then oldFormat.uninstall()
				editValid = True
				Me.format = format
				If format IsNot Nothing Then format.install(Me)
				edited = False
				firePropertyChange("textFormatter", oldFormat, format)
			End Set
			Get
				Return format
			End Get
		End Property


		''' <summary>
		''' Sets the value that will be formatted by an
		''' <code>AbstractFormatter</code> obtained from the current
		''' <code>AbstractFormatterFactory</code>. If no
		''' <code>AbstractFormatterFactory</code> has been specified, this will
		''' attempt to create one based on the type of <code>value</code>.
		''' <p>
		''' The default value of this property is null.
		''' <p>
		''' This is a JavaBeans bound property.
		''' </summary>
		''' <param name="value"> Current value to display
		''' @beaninfo
		'''       bound: true
		'''   attribute: visualUpdate true
		''' description: The value to be formatted. </param>
		Public Overridable Property value As Object
			Set(ByVal value As Object)
				If value IsNot Nothing AndAlso formatterFactory Is Nothing Then formatterFactory = getDefaultFormatterFactory(value)
				valuelue(value, True, True)
			End Set
			Get
				Return value
			End Get
		End Property


		''' <summary>
		''' Forces the current value to be taken from the
		''' <code>AbstractFormatter</code> and set as the current value.
		''' This has no effect if there is no current
		''' <code>AbstractFormatter</code> installed.
		''' </summary>
		''' <exception cref="ParseException"> if the <code>AbstractFormatter</code> is not able
		'''         to format the current value </exception>
		Public Overridable Sub commitEdit()
			Dim format As AbstractFormatter = formatter

			If format IsNot Nothing Then valuelue(format.stringToValue(text), False, True)
		End Sub

		''' <summary>
		''' Sets the validity of the edit on the receiver. You should not normally
		''' invoke this. This will be invoked by the
		''' <code>AbstractFormatter</code> as the user edits the value.
		''' <p>
		''' Not all formatters will allow the component to get into an invalid
		''' state, and thus this may never be invoked.
		''' <p>
		''' Based on the look and feel this may visually change the state of
		''' the receiver.
		''' </summary>
		''' <param name="isValid"> boolean indicating if the currently edited value is
		'''        valid.
		''' @beaninfo
		'''       bound: true
		'''   attribute: visualUpdate true
		''' description: True indicates the edited value is valid </param>
		Private Property editValid As Boolean
			Set(ByVal isValid As Boolean)
				If isValid <> editValid Then
					editValid = isValid
					firePropertyChange("editValid", Convert.ToBoolean((Not isValid)), Convert.ToBoolean(isValid))
				End If
			End Set
			Get
				Return editValid
			End Get
		End Property


		''' <summary>
		''' Invoked when the user inputs an invalid value. This gives the
		''' component a chance to provide feedback. The default
		''' implementation beeps.
		''' </summary>
		Protected Friend Overridable Sub invalidEdit()
			UIManager.lookAndFeel.provideErrorFeedback(JFormattedTextField.this)
		End Sub

		''' <summary>
		''' Processes any input method events, such as
		''' <code>InputMethodEvent.INPUT_METHOD_TEXT_CHANGED</code> or
		''' <code>InputMethodEvent.CARET_POSITION_CHANGED</code>.
		''' </summary>
		''' <param name="e"> the <code>InputMethodEvent</code> </param>
		''' <seealso cref= InputMethodEvent </seealso>
		Protected Friend Overrides Sub processInputMethodEvent(ByVal e As InputMethodEvent)
			Dim ___text As AttributedCharacterIterator = e.text
			Dim commitCount As Integer = e.committedCharacterCount

			' Keep track of the composed text
			If ___text IsNot Nothing Then
				Dim begin As Integer = ___text.beginIndex
				Dim [end] As Integer = ___text.endIndex
				composedTextExists = (([end] - begin) > commitCount)
			Else
				composedTextExists = False
			End If

			MyBase.processInputMethodEvent(e)
		End Sub

		''' <summary>
		''' Processes any focus events, such as
		''' <code>FocusEvent.FOCUS_GAINED</code> or
		''' <code>FocusEvent.FOCUS_LOST</code>.
		''' </summary>
		''' <param name="e"> the <code>FocusEvent</code> </param>
		''' <seealso cref= FocusEvent </seealso>
		Protected Friend Overridable Sub processFocusEvent(ByVal e As FocusEvent)
			MyBase.processFocusEvent(e)

			' ignore temporary focus event
			If e.temporary Then Return

			If edited AndAlso e.iD = FocusEvent.FOCUS_LOST Then
				Dim ic As java.awt.im.InputContext = inputContext
				If focusLostHandler Is Nothing Then focusLostHandler = New FocusLostHandler(Me)

				' if there is a composed text, process it first
				If (ic IsNot Nothing) AndAlso composedTextExists Then
					ic.endComposition()
					EventQueue.invokeLater(focusLostHandler)
				Else
					focusLostHandler.run()
				End If
			ElseIf Not edited Then
				' reformat
				valuelue(value, True, True)
			End If
		End Sub

		''' <summary>
		''' FOCUS_LOST behavior implementation
		''' </summary>
		<Serializable> _
		Private Class FocusLostHandler
			Implements Runnable

			Private ReadOnly outerInstance As JFormattedTextField

			Public Sub New(ByVal outerInstance As JFormattedTextField)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Sub run()
				Dim fb As Integer = outerInstance.focusLostBehavior
				If fb = JFormattedTextField.COMMIT OrElse fb = JFormattedTextField.COMMIT_OR_REVERT Then
					Try
						outerInstance.commitEdit()
						' Give it a chance to reformat.
						outerInstance.valuelue(outerInstance.value, True, True)
					Catch pe As ParseException
						If fb = outerInstance.COMMIT_OR_REVERT Then outerInstance.valuelue(outerInstance.value, True, True)
					End Try
				ElseIf fb = JFormattedTextField.REVERT Then
					outerInstance.valuelue(outerInstance.value, True, True)
				End If
			End Sub
		End Class

		''' <summary>
		''' Fetches the command list for the editor.  This is
		''' the list of commands supported by the plugged-in UI
		''' augmented by the collection of commands that the
		''' editor itself supports.  These are useful for binding
		''' to events, such as in a keymap.
		''' </summary>
		''' <returns> the command list </returns>
		Public Property Overrides actions As Action()
			Get
				Return TextAction.augmentList(MyBase.actions, defaultActions)
			End Get
		End Property

		''' <summary>
		''' Gets the class ID for a UI.
		''' </summary>
		''' <returns> the string "FormattedTextFieldUI" </returns>
		''' <seealso cref= JComponent#getUIClassID </seealso>
		Public Property Overrides uIClassID As String
			Get
				Return uiClassID
			End Get
		End Property

		''' <summary>
		''' Associates the editor with a text document.
		''' The currently registered factory is used to build a view for
		''' the document, which gets displayed by the editor after revalidation.
		''' A PropertyChange event ("document") is propagated to each listener.
		''' </summary>
		''' <param name="doc">  the document to display/edit </param>
		''' <seealso cref= #getDocument
		''' @beaninfo
		'''  description: the text document model
		'''        bound: true
		'''       expert: true </seealso>
		Public Overrides Property document As Document
			Set(ByVal doc As Document)
				If documentListener IsNot Nothing AndAlso document IsNot Nothing Then document.removeDocumentListener(documentListener)
				MyBase.document = doc
				If documentListener Is Nothing Then documentListener = New DocumentHandler(Me)
				doc.addDocumentListener(documentListener)
			End Set
		End Property

	'    
	'     * See readObject and writeObject in JComponent for more
	'     * information about serialization in Swing.
	'     *
	'     * @param s Stream to write to
	'     
		Private Sub writeObject(ByVal s As ObjectOutputStream)
			s.defaultWriteObject()
			If uIClassID.Equals(uiClassID) Then
				Dim count As SByte = JComponent.getWriteObjCounter(Me)
				count -= 1
				JComponent.writeObjCounterter(Me, count)
				If count = 0 AndAlso ui IsNot Nothing Then ui.installUI(Me)
			End If
		End Sub

		''' <summary>
		''' Resets the Actions that come from the TextFormatter to
		''' <code>actions</code>.
		''' </summary>
		Private Property formatterActions As Action()
			Set(ByVal actions As Action())
				If actions Is Nothing Then
					If textFormatterActionMap IsNot Nothing Then textFormatterActionMap.clear()
				Else
					If textFormatterActionMap Is Nothing Then
						Dim map As ActionMap = actionMap
    
						textFormatterActionMap = New ActionMap
						Do While map IsNot Nothing
							Dim parent As ActionMap = map.parent
    
							If TypeOf parent Is javax.swing.plaf.UIResource OrElse parent Is Nothing Then
								map.parent = textFormatterActionMap
								textFormatterActionMap.parent = parent
								Exit Do
							End If
							map = parent
						Loop
					End If
					For counter As Integer = actions.Length - 1 To 0 Step -1
						Dim key As Object = actions(counter).getValue(Action.NAME)
    
						If key IsNot Nothing Then textFormatterActionMap.put(key, actions(counter))
					Next counter
				End If
			End Set
		End Property

		''' <summary>
		''' Does the setting of the value. If <code>createFormat</code> is true,
		''' this will also obtain a new <code>AbstractFormatter</code> from the
		''' current factory. The property change event will be fired if
		''' <code>firePC</code> is true.
		''' </summary>
		Private Sub setValue(ByVal value As Object, ByVal createFormat As Boolean, ByVal firePC As Boolean)
			Dim oldValue As Object = Me.value

			Me.value = value

			If createFormat Then
				Dim factory As AbstractFormatterFactory = formatterFactory
				Dim atf As AbstractFormatter

				If factory IsNot Nothing Then
					atf = factory.getFormatter(Me)
				Else
					atf = Nothing
				End If
				formatter = atf
			Else
				' Assumed to be valid
				editValid = True
			End If

			edited = False

			If firePC Then firePropertyChange("value", oldValue, value)
		End Sub

		''' <summary>
		''' Sets the edited state of the receiver.
		''' </summary>
		Private Property edited As Boolean
			Set(ByVal edited As Boolean)
				Me.edited = edited
			End Set
			Get
				Return edited
			End Get
		End Property


		''' <summary>
		''' Returns an AbstractFormatterFactory suitable for the passed in
		''' Object type.
		''' </summary>
		Private Function getDefaultFormatterFactory(ByVal type As Object) As AbstractFormatterFactory
			If TypeOf type Is DateFormat Then Return New DefaultFormatterFactory(New DateFormatter(CType(type, DateFormat)))
			If TypeOf type Is NumberFormat Then Return New DefaultFormatterFactory(New NumberFormatter(CType(type, NumberFormat)))
			If TypeOf type Is Format Then Return New DefaultFormatterFactory(New InternationalFormatter(CType(type, Format)))
			If TypeOf type Is DateTime Then Return New DefaultFormatterFactory(New DateFormatter)
			If TypeOf type Is Number Then
				Dim displayFormatter As AbstractFormatter = New NumberFormatter
				CType(displayFormatter, NumberFormatter).valueClass = type.GetType()
				Dim editFormatter As AbstractFormatter = New NumberFormatter(New DecimalFormat("#.#"))
				CType(editFormatter, NumberFormatter).valueClass = type.GetType()

				Return New DefaultFormatterFactory(displayFormatter, displayFormatter,editFormatter)
			End If
			Return New DefaultFormatterFactory(New DefaultFormatter)
		End Function


		''' <summary>
		''' Instances of <code>AbstractFormatterFactory</code> are used by
		''' <code>JFormattedTextField</code> to obtain instances of
		''' <code>AbstractFormatter</code> which in turn are used to format values.
		''' <code>AbstractFormatterFactory</code> can return different
		''' <code>AbstractFormatter</code>s based on the state of the
		''' <code>JFormattedTextField</code>, perhaps returning different
		''' <code>AbstractFormatter</code>s when the
		''' <code>JFormattedTextField</code> has focus vs when it
		''' doesn't have focus.
		''' @since 1.4
		''' </summary>
		Public MustInherit Class AbstractFormatterFactory
			''' <summary>
			''' Returns an <code>AbstractFormatter</code> that can handle formatting
			''' of the passed in <code>JFormattedTextField</code>.
			''' </summary>
			''' <param name="tf"> JFormattedTextField requesting AbstractFormatter </param>
			''' <returns> AbstractFormatter to handle formatting duties, a null
			'''         return value implies the JFormattedTextField should behave
			'''         like a normal JTextField </returns>
			Public MustOverride Function getFormatter(ByVal tf As JFormattedTextField) As AbstractFormatter
		End Class


		''' <summary>
		''' Instances of <code>AbstractFormatter</code> are used by
		''' <code>JFormattedTextField</code> to handle the conversion both
		''' from an Object to a String, and back from a String to an Object.
		''' <code>AbstractFormatter</code>s can also enforce editing policies,
		''' or navigation policies, or manipulate the
		''' <code>JFormattedTextField</code> in any way it sees fit to
		''' enforce the desired policy.
		''' <p>
		''' An <code>AbstractFormatter</code> can only be active in
		''' one <code>JFormattedTextField</code> at a time.
		''' <code>JFormattedTextField</code> invokes
		''' <code>install</code> when it is ready to use it followed
		''' by <code>uninstall</code> when done. Subclasses
		''' that wish to install additional state should override
		''' <code>install</code> and message super appropriately.
		''' <p>
		''' Subclasses must override the conversion methods
		''' <code>stringToValue</code> and <code>valueToString</code>. Optionally
		''' they can override <code>getActions</code>,
		''' <code>getNavigationFilter</code> and <code>getDocumentFilter</code>
		''' to restrict the <code>JFormattedTextField</code> in a particular
		''' way.
		''' <p>
		''' Subclasses that allow the <code>JFormattedTextField</code> to be in
		''' a temporarily invalid state should invoke <code>setEditValid</code>
		''' at the appropriate times.
		''' @since 1.4
		''' </summary>
		<Serializable> _
		Public MustInherit Class AbstractFormatter
			Private ftf As JFormattedTextField

			''' <summary>
			''' Installs the <code>AbstractFormatter</code> onto a particular
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
			''' state changes.  You will only need to invoke this yourself if
			''' you are subclassing <code>JFormattedTextField</code> and
			''' installing/uninstalling <code>AbstractFormatter</code> at a
			''' different time than <code>JFormattedTextField</code> does.
			''' </summary>
			''' <param name="ftf"> JFormattedTextField to format for, may be null indicating
			'''            uninstall from current JFormattedTextField. </param>
			Public Overridable Sub install(ByVal ftf As JFormattedTextField)
				If Me.ftf IsNot Nothing Then uninstall()
				Me.ftf = ftf
				If ftf IsNot Nothing Then
					Try
						ftf.text = valueToString(ftf.value)
					Catch pe As ParseException
						ftf.text = ""
						editValid = False
					End Try
					installDocumentFilter(documentFilter)
					ftf.navigationFilter = navigationFilter
					ftf.formatterActions = actions
				End If
			End Sub

			''' <summary>
			''' Uninstalls any state the <code>AbstractFormatter</code> may have
			''' installed on the <code>JFormattedTextField</code>. This resets the
			''' <code>DocumentFilter</code>, <code>NavigationFilter</code>
			''' and additional <code>Action</code>s installed on the
			''' <code>JFormattedTextField</code>.
			''' </summary>
			Public Overridable Sub uninstall()
				If Me.ftf IsNot Nothing Then
					installDocumentFilter(Nothing)
					Me.ftf.navigationFilter = Nothing
					Me.ftf.formatterActions = Nothing
				End If
			End Sub

			''' <summary>
			''' Parses <code>text</code> returning an arbitrary Object. Some
			''' formatters may return null.
			''' </summary>
			''' <exception cref="ParseException"> if there is an error in the conversion </exception>
			''' <param name="text"> String to convert </param>
			''' <returns> Object representation of text </returns>
			Public MustOverride Function stringToValue(ByVal text As String) As Object

			''' <summary>
			''' Returns the string value to display for <code>value</code>.
			''' </summary>
			''' <exception cref="ParseException"> if there is an error in the conversion </exception>
			''' <param name="value"> Value to convert </param>
			''' <returns> String representation of value </returns>
			Public MustOverride Function valueToString(ByVal value As Object) As String

			''' <summary>
			''' Returns the current <code>JFormattedTextField</code> the
			''' <code>AbstractFormatter</code> is installed on.
			''' </summary>
			''' <returns> JFormattedTextField formatting for. </returns>
			Protected Friend Overridable Property formattedTextField As JFormattedTextField
				Get
					Return ftf
				End Get
			End Property

			''' <summary>
			''' This should be invoked when the user types an invalid character.
			''' This forwards the call to the current JFormattedTextField.
			''' </summary>
			Protected Friend Overridable Sub invalidEdit()
				Dim ftf As JFormattedTextField = formattedTextField

				If ftf IsNot Nothing Then ftf.invalidEdit()
			End Sub

			''' <summary>
			''' Invoke this to update the <code>editValid</code> property of the
			''' <code>JFormattedTextField</code>. If you an enforce a policy
			''' such that the <code>JFormattedTextField</code> is always in a
			''' valid state, you will never need to invoke this.
			''' </summary>
			''' <param name="valid"> Valid state of the JFormattedTextField </param>
			Protected Friend Overridable Property editValid As Boolean
				Set(ByVal valid As Boolean)
					Dim ftf As JFormattedTextField = formattedTextField
    
					If ftf IsNot Nothing Then ftf.editValid = valid
				End Set
			End Property

			''' <summary>
			''' Subclass and override if you wish to provide a custom set of
			''' <code>Action</code>s. <code>install</code> will install these
			''' on the <code>JFormattedTextField</code>'s <code>ActionMap</code>.
			''' </summary>
			''' <returns> Array of Actions to install on JFormattedTextField </returns>
			Protected Friend Overridable Property actions As Action()
				Get
					Return Nothing
				End Get
			End Property

			''' <summary>
			''' Subclass and override if you wish to provide a
			''' <code>DocumentFilter</code> to restrict what can be input.
			''' <code>install</code> will install the returned value onto
			''' the <code>JFormattedTextField</code>.
			''' </summary>
			''' <returns> DocumentFilter to restrict edits </returns>
			Protected Friend Overridable Property documentFilter As DocumentFilter
				Get
					Return Nothing
				End Get
			End Property

			''' <summary>
			''' Subclass and override if you wish to provide a filter to restrict
			''' where the user can navigate to.
			''' <code>install</code> will install the returned value onto
			''' the <code>JFormattedTextField</code>.
			''' </summary>
			''' <returns> NavigationFilter to restrict navigation </returns>
			Protected Friend Overridable Property navigationFilter As NavigationFilter
				Get
					Return Nothing
				End Get
			End Property

			''' <summary>
			''' Clones the <code>AbstractFormatter</code>. The returned instance
			''' is not associated with a <code>JFormattedTextField</code>.
			''' </summary>
			''' <returns> Copy of the AbstractFormatter </returns>
			Protected Friend Overridable Function clone() As Object
				Dim formatter As AbstractFormatter = CType(MyBase.clone(), AbstractFormatter)

				formatter.ftf = Nothing
				Return formatter
			End Function

			''' <summary>
			''' Installs the <code>DocumentFilter</code> <code>filter</code>
			''' onto the current <code>JFormattedTextField</code>.
			''' </summary>
			''' <param name="filter"> DocumentFilter to install on the Document. </param>
			Private Sub installDocumentFilter(ByVal filter As DocumentFilter)
				Dim ftf As JFormattedTextField = formattedTextField

				If ftf IsNot Nothing Then
					Dim doc As Document = ftf.document

					If TypeOf doc Is AbstractDocument Then CType(doc, AbstractDocument).documentFilter = filter
					doc.putProperty(GetType(DocumentFilter), Nothing)
				End If
			End Sub
		End Class


		''' <summary>
		''' Used to commit the edit. This extends JTextField.NotifyAction
		''' so that <code>isEnabled</code> is true while a JFormattedTextField
		''' has focus, and extends <code>actionPerformed</code> to invoke
		''' commitEdit.
		''' </summary>
		Friend Class CommitAction
			Inherits JTextField.NotifyAction

			Public Overridable Sub actionPerformed(ByVal e As ActionEvent)
				Dim target As JTextComponent = focusedComponent

				If TypeOf target Is JFormattedTextField Then
					' Attempt to commit the value
					Try
						CType(target, JFormattedTextField).commitEdit()
					Catch pe As ParseException
						CType(target, JFormattedTextField).invalidEdit()
						' value not committed, don't notify ActionListeners
						Return
					End Try
				End If
				' Super behavior.
				MyBase.actionPerformed(e)
			End Sub

			Public Overridable Property enabled As Boolean
				Get
					Dim target As JTextComponent = focusedComponent
					If TypeOf target Is JFormattedTextField Then
						Dim ftf As JFormattedTextField = CType(target, JFormattedTextField)
						If Not ftf.edited Then Return False
						Return True
					End If
					Return MyBase.enabled
				End Get
			End Property
		End Class


		''' <summary>
		''' CancelAction will reset the value in the JFormattedTextField when
		''' <code>actionPerformed</code> is invoked. It will only be
		''' enabled if the focused component is an instance of
		''' JFormattedTextField.
		''' </summary>
		Private Class CancelAction
			Inherits TextAction

			Public Sub New()
				MyBase.New("reset-field-edit")
			End Sub

			Public Overridable Sub actionPerformed(ByVal e As ActionEvent)
				Dim target As JTextComponent = focusedComponent

				If TypeOf target Is JFormattedTextField Then
					Dim ftf As JFormattedTextField = CType(target, JFormattedTextField)
					ftf.value = ftf.value
				End If
			End Sub

			Public Property Overrides enabled As Boolean
				Get
					Dim target As JTextComponent = focusedComponent
					If TypeOf target Is JFormattedTextField Then
						Dim ftf As JFormattedTextField = CType(target, JFormattedTextField)
						If Not ftf.edited Then Return False
						Return True
					End If
					Return MyBase.enabled
				End Get
			End Property
		End Class


		''' <summary>
		''' Sets the dirty state as the document changes.
		''' </summary>
		<Serializable> _
		Private Class DocumentHandler
			Implements DocumentListener

			Private ReadOnly outerInstance As JFormattedTextField

			Public Sub New(ByVal outerInstance As JFormattedTextField)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Sub insertUpdate(ByVal e As DocumentEvent) Implements DocumentListener.insertUpdate
				outerInstance.edited = True
			End Sub
			Public Overridable Sub removeUpdate(ByVal e As DocumentEvent) Implements DocumentListener.removeUpdate
				outerInstance.edited = True
			End Sub
			Public Overridable Sub changedUpdate(ByVal e As DocumentEvent) Implements DocumentListener.changedUpdate
			End Sub
		End Class
	End Class

End Namespace
Imports System
Imports System.Runtime.CompilerServices
Imports javax.swing.text
Imports javax.swing.plaf
Imports javax.swing.event
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
	''' <code>JTextField</code> is a lightweight component that allows the editing
	''' of a single line of text.
	''' For information on and examples of using text fields,
	''' see
	''' <a href="https://docs.oracle.com/javase/tutorial/uiswing/components/textfield.html">How to Use Text Fields</a>
	''' in <em>The Java Tutorial.</em>
	''' 
	''' <p>
	''' <code>JTextField</code> is intended to be source-compatible
	''' with <code>java.awt.TextField</code> where it is reasonable to do so.  This
	''' component has capabilities not found in the <code>java.awt.TextField</code>
	''' class.  The superclass should be consulted for additional capabilities.
	''' <p>
	''' <code>JTextField</code> has a method to establish the string used as the
	''' command string for the action event that gets fired.  The
	''' <code>java.awt.TextField</code> used the text of the field as the command
	''' string for the <code>ActionEvent</code>.
	''' <code>JTextField</code> will use the command
	''' string set with the <code>setActionCommand</code> method if not <code>null</code>,
	''' otherwise it will use the text of the field as a compatibility with
	''' <code>java.awt.TextField</code>.
	''' <p>
	''' The method <code>setEchoChar</code> and <code>getEchoChar</code>
	''' are not provided directly to avoid a new implementation of a
	''' pluggable look-and-feel inadvertently exposing password characters.
	''' To provide password-like services a separate class <code>JPasswordField</code>
	''' extends <code>JTextField</code> to provide this service with an independently
	''' pluggable look-and-feel.
	''' <p>
	''' The <code>java.awt.TextField</code> could be monitored for changes by adding
	''' a <code>TextListener</code> for <code>TextEvent</code>'s.
	''' In the <code>JTextComponent</code> based
	''' components, changes are broadcasted from the model via a
	''' <code>DocumentEvent</code> to <code>DocumentListeners</code>.
	''' The <code>DocumentEvent</code> gives
	''' the location of the change and the kind of change if desired.
	''' The code fragment might look something like:
	''' <pre><code>
	''' &nbsp;   DocumentListener myListener = ??;
	''' &nbsp;   JTextField myArea = ??;
	''' &nbsp;   myArea.getDocument().addDocumentListener(myListener);
	''' </code></pre>
	''' <p>
	''' The horizontal alignment of <code>JTextField</code> can be set to be left
	''' justified, leading justified, centered, right justified or trailing justified.
	''' Right/trailing justification is useful if the required size
	''' of the field text is smaller than the size allocated to it.
	''' This is determined by the <code>setHorizontalAlignment</code>
	''' and <code>getHorizontalAlignment</code> methods.  The default
	''' is to be leading justified.
	''' <p>
	''' How the text field consumes VK_ENTER events depends
	''' on whether the text field has any action listeners.
	''' If so, then VK_ENTER results in the listeners
	''' getting an ActionEvent,
	''' and the VK_ENTER event is consumed.
	''' This is compatible with how AWT text fields handle VK_ENTER events.
	''' If the text field has no action listeners, then as of v 1.3 the VK_ENTER
	''' event is not consumed.  Instead, the bindings of ancestor components
	''' are processed, which enables the default button feature of
	''' JFC/Swing to work.
	''' <p>
	''' Customized fields can easily be created by extending the model and
	''' changing the default model provided.  For example, the following piece
	''' of code will create a field that holds only upper case characters.  It
	''' will work even if text is pasted into from the clipboard or it is altered via
	''' programmatic changes.
	''' <pre><code>
	''' 
	''' &nbsp;public class UpperCaseField extends JTextField {
	''' &nbsp;
	''' &nbsp;    public UpperCaseField(int cols) {
	''' &nbsp;        super(cols);
	''' &nbsp;    }
	''' &nbsp;
	''' &nbsp;    protected Document createDefaultModel() {
	''' &nbsp;        return new UpperCaseDocument();
	''' &nbsp;    }
	''' &nbsp;
	''' &nbsp;    static class UpperCaseDocument extends PlainDocument {
	''' &nbsp;
	''' &nbsp;        public void insertString(int offs, String str, AttributeSet a)
	''' &nbsp;            throws BadLocationException {
	''' &nbsp;
	''' &nbsp;            if (str == null) {
	''' &nbsp;                return;
	''' &nbsp;            }
	''' &nbsp;            char[] upper = str.toCharArray();
	''' &nbsp;            for (int i = 0; i &lt; upper.length; i++) {
	''' &nbsp;                upper[i] = Character.toUpperCase(upper[i]);
	''' &nbsp;            }
	''' &nbsp;            super.insertString(offs, new String(upper), a);
	''' &nbsp;        }
	''' &nbsp;    }
	''' &nbsp;}
	''' 
	''' </code></pre>
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
	'''   attribute: isContainer false
	''' description: A component which allows for the editing of a single line of text.
	''' 
	''' @author  Timothy Prinzing </summary>
	''' <seealso cref= #setActionCommand </seealso>
	''' <seealso cref= JPasswordField </seealso>
	''' <seealso cref= #addActionListener </seealso>
	Public Class JTextField
		Inherits JTextComponent
		Implements SwingConstants

		''' <summary>
		''' Constructs a new <code>TextField</code>.  A default model is created,
		''' the initial string is <code>null</code>,
		''' and the number of columns is set to 0.
		''' </summary>
		Public Sub New()
			Me.New(Nothing, Nothing, 0)
		End Sub

		''' <summary>
		''' Constructs a new <code>TextField</code> initialized with the
		''' specified text. A default model is created and the number of
		''' columns is 0.
		''' </summary>
		''' <param name="text"> the text to be displayed, or <code>null</code> </param>
		Public Sub New(ByVal text As String)
			Me.New(Nothing, text, 0)
		End Sub

		''' <summary>
		''' Constructs a new empty <code>TextField</code> with the specified
		''' number of columns.
		''' A default model is created and the initial string is set to
		''' <code>null</code>.
		''' </summary>
		''' <param name="columns">  the number of columns to use to calculate
		'''   the preferred width; if columns is set to zero, the
		'''   preferred width will be whatever naturally results from
		'''   the component implementation </param>
		Public Sub New(ByVal columns As Integer)
			Me.New(Nothing, Nothing, columns)
		End Sub

		''' <summary>
		''' Constructs a new <code>TextField</code> initialized with the
		''' specified text and columns.  A default model is created.
		''' </summary>
		''' <param name="text"> the text to be displayed, or <code>null</code> </param>
		''' <param name="columns">  the number of columns to use to calculate
		'''   the preferred width; if columns is set to zero, the
		'''   preferred width will be whatever naturally results from
		'''   the component implementation </param>
		Public Sub New(ByVal text As String, ByVal columns As Integer)
			Me.New(Nothing, text, columns)
		End Sub

		''' <summary>
		''' Constructs a new <code>JTextField</code> that uses the given text
		''' storage model and the given number of columns.
		''' This is the constructor through which the other constructors feed.
		''' If the document is <code>null</code>, a default model is created.
		''' </summary>
		''' <param name="doc">  the text storage to use; if this is <code>null</code>,
		'''          a default will be provided by calling the
		'''          <code>createDefaultModel</code> method </param>
		''' <param name="text">  the initial string to display, or <code>null</code> </param>
		''' <param name="columns">  the number of columns to use to calculate
		'''   the preferred width &gt;= 0; if <code>columns</code>
		'''   is set to zero, the preferred width will be whatever
		'''   naturally results from the component implementation </param>
		''' <exception cref="IllegalArgumentException"> if <code>columns</code> &lt; 0 </exception>
		Public Sub New(ByVal doc As Document, ByVal text As String, ByVal columns As Integer)
			If columns < 0 Then Throw New System.ArgumentException("columns less than zero.")
			visibility = New DefaultBoundedRangeModel
			visibility.addChangeListener(New ScrollRepainter(Me))
			Me.columns = columns
			If doc Is Nothing Then doc = createDefaultModel()
			document = doc
			If text IsNot Nothing Then text = text
		End Sub

		''' <summary>
		''' Gets the class ID for a UI.
		''' </summary>
		''' <returns> the string "TextFieldUI" </returns>
		''' <seealso cref= JComponent#getUIClassID </seealso>
		''' <seealso cref= UIDefaults#getUI </seealso>
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
				If doc IsNot Nothing Then doc.putProperty("filterNewlines", Boolean.TRUE)
				MyBase.document = doc
			End Set
		End Property

		''' <summary>
		''' Calls to <code>revalidate</code> that come from within the
		''' textfield itself will
		''' be handled by validating the textfield, unless the textfield
		''' is contained within a <code>JViewport</code>,
		''' in which case this returns false.
		''' </summary>
		''' <returns> if the parent of this textfield is a <code>JViewPort</code>
		'''          return false, otherwise return true
		''' </returns>
		''' <seealso cref= JComponent#revalidate </seealso>
		''' <seealso cref= JComponent#isValidateRoot </seealso>
		''' <seealso cref= java.awt.Container#isValidateRoot </seealso>
		Public Property Overrides validateRoot As Boolean
			Get
				Return Not(TypeOf SwingUtilities.getUnwrappedParent(Me) Is JViewport)
			End Get
		End Property


		''' <summary>
		''' Returns the horizontal alignment of the text.
		''' Valid keys are:
		''' <ul>
		''' <li><code>JTextField.LEFT</code>
		''' <li><code>JTextField.CENTER</code>
		''' <li><code>JTextField.RIGHT</code>
		''' <li><code>JTextField.LEADING</code>
		''' <li><code>JTextField.TRAILING</code>
		''' </ul>
		''' </summary>
		''' <returns> the horizontal alignment </returns>
		Public Overridable Property horizontalAlignment As Integer
			Get
				Return horizontalAlignment
			End Get
			Set(ByVal alignment As Integer)
				If alignment = horizontalAlignment Then Return
				Dim oldValue As Integer = horizontalAlignment
				If (alignment = LEFT) OrElse (alignment = CENTER) OrElse (alignment = RIGHT) OrElse (alignment = LEADING) OrElse (alignment = TRAILING) Then
					horizontalAlignment = alignment
				Else
					Throw New System.ArgumentException("horizontalAlignment")
				End If
				firePropertyChange("horizontalAlignment", oldValue, horizontalAlignment)
				invalidate()
				repaint()
			 End Set
		End Property


		''' <summary>
		''' Creates the default implementation of the model
		''' to be used at construction if one isn't explicitly
		''' given.  An instance of <code>PlainDocument</code> is returned.
		''' </summary>
		''' <returns> the default model implementation </returns>
		Protected Friend Overridable Function createDefaultModel() As Document
			Return New PlainDocument
		End Function

		''' <summary>
		''' Returns the number of columns in this <code>TextField</code>.
		''' </summary>
		''' <returns> the number of columns &gt;= 0 </returns>
		Public Overridable Property columns As Integer
			Get
				Return columns
			End Get
			Set(ByVal columns As Integer)
				Dim oldVal As Integer = Me.columns
				If columns < 0 Then Throw New System.ArgumentException("columns less than zero.")
				If columns <> oldVal Then
					Me.columns = columns
					invalidate()
				End If
			End Set
		End Property


		''' <summary>
		''' Returns the column width.
		''' The meaning of what a column is can be considered a fairly weak
		''' notion for some fonts.  This method is used to define the width
		''' of a column.  By default this is defined to be the width of the
		''' character <em>m</em> for the font used.  This method can be
		''' redefined to be some alternative amount
		''' </summary>
		''' <returns> the column width &gt;= 1 </returns>
		Protected Friend Overridable Property columnWidth As Integer
			Get
				If columnWidth = 0 Then
					Dim metrics As FontMetrics = getFontMetrics(font)
					columnWidth = metrics.charWidth("m"c)
				End If
				Return columnWidth
			End Get
		End Property

		''' <summary>
		''' Returns the preferred size <code>Dimensions</code> needed for this
		''' <code>TextField</code>.  If a non-zero number of columns has been
		''' set, the width is set to the columns multiplied by
		''' the column width.
		''' </summary>
		''' <returns> the dimension of this textfield </returns>
		Public Property Overrides preferredSize As Dimension
			Get
				Dim ___size As Dimension = MyBase.preferredSize
				If columns <> 0 Then
					Dim ___insets As Insets = insets
					___size.width = columns * columnWidth + ___insets.left + ___insets.right
				End If
				Return ___size
			End Get
		End Property

		''' <summary>
		''' Sets the current font.  This removes cached row height and column
		''' width so the new font will be reflected.
		''' <code>revalidate</code> is called after setting the font.
		''' </summary>
		''' <param name="f"> the new font </param>
		Public Overrides Property font As Font
			Set(ByVal f As Font)
				MyBase.font = f
				columnWidth = 0
			End Set
		End Property

		''' <summary>
		''' Adds the specified action listener to receive
		''' action events from this textfield.
		''' </summary>
		''' <param name="l"> the action listener to be added </param>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub addActionListener(ByVal l As ActionListener)
			listenerList.add(GetType(ActionListener), l)
		End Sub

		''' <summary>
		''' Removes the specified action listener so that it no longer
		''' receives action events from this textfield.
		''' </summary>
		''' <param name="l"> the action listener to be removed </param>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub removeActionListener(ByVal l As ActionListener)
			If (l IsNot Nothing) AndAlso (action Is l) Then
				action = Nothing
			Else
				listenerList.remove(GetType(ActionListener), l)
			End If
		End Sub

		''' <summary>
		''' Returns an array of all the <code>ActionListener</code>s added
		''' to this JTextField with addActionListener().
		''' </summary>
		''' <returns> all of the <code>ActionListener</code>s added or an empty
		'''         array if no listeners have been added
		''' @since 1.4 </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property actionListeners As ActionListener()
			Get
				Return listenerList.getListeners(GetType(ActionListener))
			End Get
		End Property

		''' <summary>
		''' Notifies all listeners that have registered interest for
		''' notification on this event type.  The event instance
		''' is lazily created.
		''' The listener list is processed in last to
		''' first order. </summary>
		''' <seealso cref= EventListenerList </seealso>
		Protected Friend Overridable Sub fireActionPerformed()
			' Guaranteed to return a non-null array
			Dim ___listeners As Object() = listenerList.listenerList
			Dim modifiers As Integer = 0
			Dim currentEvent As AWTEvent = EventQueue.currentEvent
			If TypeOf currentEvent Is InputEvent Then
				modifiers = CType(currentEvent, InputEvent).modifiers
			ElseIf TypeOf currentEvent Is ActionEvent Then
				modifiers = CType(currentEvent, ActionEvent).modifiers
			End If
			Dim e As New ActionEvent(Me, ActionEvent.ACTION_PERFORMED,If(command IsNot Nothing, command, text), EventQueue.mostRecentEventTime, modifiers)

			' Process the listeners last to first, notifying
			' those that are interested in this event
			For i As Integer = ___listeners.Length-2 To 0 Step -2
				If ___listeners(i) Is GetType(ActionListener) Then CType(___listeners(i+1), ActionListener).actionPerformed(e)
			Next i
		End Sub

		''' <summary>
		''' Sets the command string used for action events.
		''' </summary>
		''' <param name="command"> the command string </param>
		Public Overridable Property actionCommand As String
			Set(ByVal command As String)
				Me.command = command
			End Set
		End Property

		Private action As Action
		Private actionPropertyChangeListener As PropertyChangeListener

		''' <summary>
		''' Sets the <code>Action</code> for the <code>ActionEvent</code> source.
		''' The new <code>Action</code> replaces
		''' any previously set <code>Action</code> but does not affect
		''' <code>ActionListeners</code> independently
		''' added with <code>addActionListener</code>.
		''' If the <code>Action</code> is already a registered
		''' <code>ActionListener</code>
		''' for the <code>ActionEvent</code> source, it is not re-registered.
		''' <p>
		''' Setting the <code>Action</code> results in immediately changing
		''' all the properties described in <a href="Action.html#buttonActions">
		''' Swing Components Supporting <code>Action</code></a>.
		''' Subsequently, the textfield's properties are automatically updated
		''' as the <code>Action</code>'s properties change.
		''' <p>
		''' This method uses three other methods to set
		''' and help track the <code>Action</code>'s property values.
		''' It uses the <code>configurePropertiesFromAction</code> method
		''' to immediately change the textfield's properties.
		''' To track changes in the <code>Action</code>'s property values,
		''' this method registers the <code>PropertyChangeListener</code>
		''' returned by <code>createActionPropertyChangeListener</code>. The
		''' default {@code PropertyChangeListener} invokes the
		''' {@code actionPropertyChanged} method when a property in the
		''' {@code Action} changes.
		''' </summary>
		''' <param name="a"> the <code>Action</code> for the <code>JTextField</code>,
		'''          or <code>null</code>
		''' @since 1.3 </param>
		''' <seealso cref= Action </seealso>
		''' <seealso cref= #getAction </seealso>
		''' <seealso cref= #configurePropertiesFromAction </seealso>
		''' <seealso cref= #createActionPropertyChangeListener </seealso>
		''' <seealso cref= #actionPropertyChanged
		''' @beaninfo
		'''        bound: true
		'''    attribute: visualUpdate true
		'''  description: the Action instance connected with this ActionEvent source </seealso>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Public Overridable Sub setAction(ByVal a As Action) 'JavaToDotNetTempPropertySetaction
		Public Overridable Property action As Action
			Set(ByVal a As Action)
				Dim oldValue As Action = action
				If action Is Nothing OrElse (Not action.Equals(a)) Then
					action = a
					If oldValue IsNot Nothing Then
						removeActionListener(oldValue)
						oldValue.removePropertyChangeListener(actionPropertyChangeListener)
						actionPropertyChangeListener = Nothing
					End If
					configurePropertiesFromAction(action)
					If action IsNot Nothing Then
						' Don't add if it is already a listener
						If Not isListener(GetType(ActionListener), action) Then addActionListener(action)
						' Reverse linkage:
						actionPropertyChangeListener = createActionPropertyChangeListener(action)
						action.addPropertyChangeListener(actionPropertyChangeListener)
					End If
					firePropertyChange("action", oldValue, action)
				End If
			End Set
			Get
		End Property

		Private Function isListener(ByVal c As Type, ByVal a As ActionListener) As Boolean
			Dim ___isListener As Boolean = False
			Dim ___listeners As Object() = listenerList.listenerList
			For i As Integer = ___listeners.Length-2 To 0 Step -2
				If ___listeners(i) Is c AndAlso ___listeners(i+1) Is a Then ___isListener=True
			Next i
			Return ___isListener
		End Function

			Return action
		End Function

		''' <summary>
		''' Sets the properties on this textfield to match those in the specified
		''' <code>Action</code>.  Refer to <a href="Action.html#buttonActions">
		''' Swing Components Supporting <code>Action</code></a> for more
		''' details as to which properties this sets.
		''' </summary>
		''' <param name="a"> the <code>Action</code> from which to get the properties,
		'''          or <code>null</code>
		''' @since 1.3 </param>
		''' <seealso cref= Action </seealso>
		''' <seealso cref= #setAction </seealso>
		Protected Friend Overridable Sub configurePropertiesFromAction(ByVal a As Action)
			AbstractAction.enabledFromActionion(Me, a)
			AbstractAction.toolTipTextFromActionion(Me, a)
			actionCommandFromAction = a
		End Sub

		''' <summary>
		''' Updates the textfield's state in response to property changes in
		''' associated action. This method is invoked from the
		''' {@code PropertyChangeListener} returned from
		''' {@code createActionPropertyChangeListener}. Subclasses do not normally
		''' need to invoke this. Subclasses that support additional {@code Action}
		''' properties should override this and
		''' {@code configurePropertiesFromAction}.
		''' <p>
		''' Refer to the table at <a href="Action.html#buttonActions">
		''' Swing Components Supporting <code>Action</code></a> for a list of
		''' the properties this method sets.
		''' </summary>
		''' <param name="action"> the <code>Action</code> associated with this textfield </param>
		''' <param name="propertyName"> the name of the property that changed
		''' @since 1.6 </param>
		''' <seealso cref= Action </seealso>
		''' <seealso cref= #configurePropertiesFromAction </seealso>
		Protected Friend Overridable Sub actionPropertyChanged(ByVal action As Action, ByVal propertyName As String)
			If propertyName = Action.ACTION_COMMAND_KEY Then
				actionCommandFromAction = action
			ElseIf propertyName = "enabled" Then
				AbstractAction.enabledFromActionion(Me, action)
			ElseIf propertyName = Action.SHORT_DESCRIPTION Then
				AbstractAction.toolTipTextFromActionion(Me, action)
			End If
		End Sub

		Private Property actionCommandFromAction As Action
			Set(ByVal action As Action)
				actionCommand = If(action Is Nothing, Nothing, CStr(action.getValue(Action.ACTION_COMMAND_KEY)))
			End Set
		End Property

		''' <summary>
		''' Creates and returns a <code>PropertyChangeListener</code> that is
		''' responsible for listening for changes from the specified
		''' <code>Action</code> and updating the appropriate properties.
		''' <p>
		''' <b>Warning:</b> If you subclass this do not create an anonymous
		''' inner class.  If you do the lifetime of the textfield will be tied to
		''' that of the <code>Action</code>.
		''' </summary>
		''' <param name="a"> the textfield's action
		''' @since 1.3 </param>
		''' <seealso cref= Action </seealso>
		''' <seealso cref= #setAction </seealso>
		Protected Friend Overridable Function createActionPropertyChangeListener(ByVal a As Action) As PropertyChangeListener
			Return New TextFieldActionPropertyChangeListener(Me, a)
		End Function

		Private Class TextFieldActionPropertyChangeListener
			Inherits ActionPropertyChangeListener(Of JTextField)

			Friend Sub New(ByVal tf As JTextField, ByVal a As Action)
				MyBase.New(tf, a)
			End Sub

			Protected Friend Overridable Sub actionPropertyChanged(ByVal textField As JTextField, ByVal action As Action, ByVal e As PropertyChangeEvent)
				If AbstractAction.shouldReconfigure(e) Then
					textField.configurePropertiesFromAction(action)
				Else
					textField.actionPropertyChanged(action, e.propertyName)
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
		''' Processes action events occurring on this textfield by
		''' dispatching them to any registered <code>ActionListener</code> objects.
		''' This is normally called by the controller registered with
		''' textfield.
		''' </summary>
		Public Overridable Sub postActionEvent()
			fireActionPerformed()
		End Sub

		' --- Scrolling support -----------------------------------

		''' <summary>
		''' Gets the visibility of the text field.  This can
		''' be adjusted to change the location of the visible
		''' area if the size of the field is greater than
		''' the area that was allocated to the field.
		''' 
		''' <p>
		''' The fields look-and-feel implementation manages
		''' the values of the minimum, maximum, and extent
		''' properties on the <code>BoundedRangeModel</code>.
		''' </summary>
		''' <returns> the visibility </returns>
		''' <seealso cref= BoundedRangeModel </seealso>
		Public Overridable Property horizontalVisibility As BoundedRangeModel
			Get
				Return visibility
			End Get
		End Property

		''' <summary>
		''' Gets the scroll offset, in pixels.
		''' </summary>
		''' <returns> the offset &gt;= 0 </returns>
		Public Overridable Property scrollOffset As Integer
			Get
				Return visibility.value
			End Get
			Set(ByVal scrollOffset As Integer)
				visibility.value = scrollOffset
			End Set
		End Property


		''' <summary>
		''' Scrolls the field left or right.
		''' </summary>
		''' <param name="r"> the region to scroll </param>
		Public Overrides Sub scrollRectToVisible(ByVal r As Rectangle)
			' convert to coordinate system of the bounded range
			Dim i As Insets = insets
			Dim x0 As Integer = r.x + visibility.value - i.left
			Dim x1 As Integer = x0 + r.width
			If x0 < visibility.value Then
				' Scroll to the left
				visibility.value = x0
			ElseIf x1 > visibility.value + visibility.extent Then
				' Scroll to the right
				visibility.value = x1 - visibility.extent
			End If
		End Sub

		''' <summary>
		''' Returns true if the receiver has an <code>ActionListener</code>
		''' installed.
		''' </summary>
		Friend Overridable Function hasActionListener() As Boolean
			' Guaranteed to return a non-null array
			Dim ___listeners As Object() = listenerList.listenerList
			' Process the listeners last to first, notifying
			' those that are interested in this event
			For i As Integer = ___listeners.Length-2 To 0 Step -2
				If ___listeners(i) Is GetType(ActionListener) Then Return True
			Next i
			Return False
		End Function

		' --- variables -------------------------------------------

		''' <summary>
		''' Name of the action to send notification that the
		''' contents of the field have been accepted.  Typically
		''' this is bound to a carriage-return.
		''' </summary>
		Public Const notifyAction As String = "notify-field-accept"

		Private visibility As BoundedRangeModel
		Private horizontalAlignment As Integer = LEADING
		Private columns As Integer
		Private columnWidth As Integer
		Private command As String

		Private Shared ReadOnly defaultActions As Action() = { New NotifyAction }

		''' <seealso cref= #getUIClassID </seealso>
		''' <seealso cref= #readObject </seealso>
		Private Const uiClassID As String = "TextFieldUI"

		' --- Action implementations -----------------------------------

		' Note that JFormattedTextField.CommitAction extends this
		Friend Class NotifyAction
			Inherits TextAction

			Friend Sub New()
				MyBase.New(notifyAction)
			End Sub

			Public Overridable Sub actionPerformed(ByVal e As ActionEvent)
				Dim target As JTextComponent = focusedComponent
				If TypeOf target Is JTextField Then
					Dim field As JTextField = CType(target, JTextField)
					field.postActionEvent()
				End If
			End Sub

			Public Property Overrides enabled As Boolean
				Get
					Dim target As JTextComponent = focusedComponent
					If TypeOf target Is JTextField Then Return CType(target, JTextField).hasActionListener()
					Return False
				End Get
			End Property
		End Class

		<Serializable> _
		Friend Class ScrollRepainter
			Implements ChangeListener

			Private ReadOnly outerInstance As JTextField

			Public Sub New(ByVal outerInstance As JTextField)
				Me.outerInstance = outerInstance
			End Sub


			Public Overridable Sub stateChanged(ByVal e As ChangeEvent) Implements ChangeListener.stateChanged
				repaint()
			End Sub

		End Class


		''' <summary>
		''' See <code>readObject</code> and <code>writeObject</code> in
		''' <code>JComponent</code> for more
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


		''' <summary>
		''' Returns a string representation of this <code>JTextField</code>.
		''' This method is intended to be used only for debugging purposes,
		''' and the content and format of the returned string may vary between
		''' implementations. The returned string may be empty but may not
		''' be <code>null</code>.
		''' </summary>
		''' <returns>  a string representation of this <code>JTextField</code> </returns>
		Protected Friend Overrides Function paramString() As String
			Dim horizontalAlignmentString As String
			If horizontalAlignment = LEFT Then
				horizontalAlignmentString = "LEFT"
			ElseIf horizontalAlignment = CENTER Then
				horizontalAlignmentString = "CENTER"
			ElseIf horizontalAlignment = RIGHT Then
				horizontalAlignmentString = "RIGHT"
			ElseIf horizontalAlignment = LEADING Then
				horizontalAlignmentString = "LEADING"
			ElseIf horizontalAlignment = TRAILING Then
				horizontalAlignmentString = "TRAILING"
			Else
				horizontalAlignmentString = ""
			End If
			Dim commandString As String = (If(command IsNot Nothing, command, ""))

			Return MyBase.paramString() & ",columns=" & columns & ",columnWidth=" & columnWidth & ",command=" & commandString & ",horizontalAlignment=" & horizontalAlignmentString
		End Function


	'///////////////
	' Accessibility support
	'//////////////


		''' <summary>
		''' Gets the <code>AccessibleContext</code> associated with this
		''' <code>JTextField</code>. For <code>JTextFields</code>,
		''' the <code>AccessibleContext</code> takes the form of an
		''' <code>AccessibleJTextField</code>.
		''' A new <code>AccessibleJTextField</code> instance is created
		''' if necessary.
		''' </summary>
		''' <returns> an <code>AccessibleJTextField</code> that serves as the
		'''         <code>AccessibleContext</code> of this <code>JTextField</code> </returns>
		Public Property Overrides accessibleContext As AccessibleContext
			Get
				If accessibleContext Is Nothing Then accessibleContext = New AccessibleJTextField(Me)
				Return accessibleContext
			End Get
		End Property

		''' <summary>
		''' This class implements accessibility support for the
		''' <code>JTextField</code> class.  It provides an implementation of the
		''' Java Accessibility API appropriate to text field user-interface
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
		Protected Friend Class AccessibleJTextField
			Inherits AccessibleJTextComponent

			Private ReadOnly outerInstance As JTextField

			Public Sub New(ByVal outerInstance As JTextField)
				Me.outerInstance = outerInstance
			End Sub


			''' <summary>
			''' Gets the state set of this object.
			''' </summary>
			''' <returns> an instance of AccessibleStateSet describing the states
			''' of the object </returns>
			''' <seealso cref= AccessibleState </seealso>
			Public Overridable Property accessibleStateSet As AccessibleStateSet
				Get
					Dim states As AccessibleStateSet = MyBase.accessibleStateSet
					states.add(AccessibleState.SINGLE_LINE)
					Return states
				End Get
			End Property
		End Class
	End Class

End Namespace
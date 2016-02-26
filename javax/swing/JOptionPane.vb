Imports Microsoft.VisualBasic
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Windows.Forms
Imports javax.accessibility
import static javax.swing.ClientPropertyKey.PopupFactory_FORCE_HEAVYWEIGHT_POPUP

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
	''' <code>JOptionPane</code> makes it easy to pop up a standard dialog box that
	''' prompts users for a value or informs them of something.
	''' For information about using <code>JOptionPane</code>, see
	''' <a
	''' href="https://docs.oracle.com/javase/tutorial/uiswing/components/dialog.html">How to Make Dialogs</a>,
	''' a section in <em>The Java Tutorial</em>.
	''' 
	''' <p>
	''' 
	''' While the <code>JOptionPane</code>
	''' class may appear complex because of the large number of methods, almost
	''' all uses of this class are one-line calls to one of the static
	''' <code>showXxxDialog</code> methods shown below:
	''' <blockquote>
	''' 
	''' 
	''' <table border=1 summary="Common JOptionPane method names and their descriptions">
	''' <tr>
	'''    <th>Method Name</th>
	'''    <th>Description</th>
	''' </tr>
	''' <tr>
	'''    <td>showConfirmDialog</td>
	'''    <td>Asks a confirming question, like yes/no/cancel.</td>
	''' </tr>
	''' <tr>
	'''    <td>showInputDialog</td>
	'''    <td>Prompt for some input.</td>
	''' </tr>
	''' <tr>
	'''   <td>showMessageDialog</td>
	'''   <td>Tell the user about something that has happened.</td>
	''' </tr>
	''' <tr>
	'''   <td>showOptionDialog</td>
	'''   <td>The Grand Unification of the above three.</td>
	''' </tr>
	''' </table>
	''' 
	''' </blockquote>
	''' Each of these methods also comes in a <code>showInternalXXX</code>
	''' flavor, which uses an internal frame to hold the dialog box (see
	''' <seealso cref="JInternalFrame"/>).
	''' Multiple convenience methods have also been defined -- overloaded
	''' versions of the basic methods that use different parameter lists.
	''' <p>
	''' All dialogs are modal. Each <code>showXxxDialog</code> method blocks
	''' the caller until the user's interaction is complete.
	''' 
	''' <table cellspacing=6 cellpadding=4 border=0 style="float:right" summary="layout">
	''' <tr>
	'''  <td style="background-color:#FFe0d0" rowspan=2>icon</td>
	'''  <td style="background-color:#FFe0d0">message</td>
	''' </tr>
	''' <tr>
	'''  <td style="background-color:#FFe0d0">input value</td>
	''' </tr>
	''' <tr>
	'''   <td style="background-color:#FFe0d0" colspan=2>option buttons</td>
	''' </tr>
	''' </table>
	''' 
	''' The basic appearance of one of these dialog boxes is generally
	''' similar to the picture at the right, although the various
	''' look-and-feels are
	''' ultimately responsible for the final result.  In particular, the
	''' look-and-feels will adjust the layout to accommodate the option pane's
	''' <code>ComponentOrientation</code> property.
	''' <br style="clear:all">
	''' <p>
	''' <b>Parameters:</b><br>
	''' The parameters to these methods follow consistent patterns:
	''' <blockquote>
	''' <dl compact>
	''' <dt>parentComponent<dd>
	''' Defines the <code>Component</code> that is to be the parent of this
	''' dialog box.
	''' It is used in two ways: the <code>Frame</code> that contains
	''' it is used as the <code>Frame</code>
	''' parent for the dialog box, and its screen coordinates are used in
	''' the placement of the dialog box. In general, the dialog box is placed
	''' just below the component. This parameter may be <code>null</code>,
	''' in which case a default <code>Frame</code> is used as the parent,
	''' and the dialog will be
	''' centered on the screen (depending on the {@literal L&F}).
	''' <dt><a name=message>message</a><dd>
	''' A descriptive message to be placed in the dialog box.
	''' In the most common usage, message is just a <code>String</code> or
	''' <code>String</code> constant.
	''' However, the type of this parameter is actually <code>Object</code>. Its
	''' interpretation depends on its type:
	''' <dl compact>
	''' <dt>Object[]<dd>An array of objects is interpreted as a series of
	'''                 messages (one per object) arranged in a vertical stack.
	'''                 The interpretation is recursive -- each object in the
	'''                 array is interpreted according to its type.
	''' <dt>Component<dd>The <code>Component</code> is displayed in the dialog.
	''' <dt>Icon<dd>The <code>Icon</code> is wrapped in a <code>JLabel</code>
	'''               and displayed in the dialog.
	''' <dt>others<dd>The object is converted to a <code>String</code> by calling
	'''               its <code>toString</code> method. The result is wrapped in a
	'''               <code>JLabel</code> and displayed.
	''' </dl>
	''' <dt>messageType<dd>Defines the style of the message. The Look and Feel
	''' manager may lay out the dialog differently depending on this value, and
	''' will often provide a default icon. The possible values are:
	''' <ul>
	''' <li><code>ERROR_MESSAGE</code>
	''' <li><code>INFORMATION_MESSAGE</code>
	''' <li><code>WARNING_MESSAGE</code>
	''' <li><code>QUESTION_MESSAGE</code>
	''' <li><code>PLAIN_MESSAGE</code>
	''' </ul>
	''' <dt>optionType<dd>Defines the set of option buttons that appear at
	''' the bottom of the dialog box:
	''' <ul>
	''' <li><code>DEFAULT_OPTION</code>
	''' <li><code>YES_NO_OPTION</code>
	''' <li><code>YES_NO_CANCEL_OPTION</code>
	''' <li><code>OK_CANCEL_OPTION</code>
	''' </ul>
	''' You aren't limited to this set of option buttons.  You can provide any
	''' buttons you want using the options parameter.
	''' <dt>options<dd>A more detailed description of the set of option buttons
	''' that will appear at the bottom of the dialog box.
	''' The usual value for the options parameter is an array of
	''' <code>String</code>s. But
	''' the parameter type is an array of <code>Objects</code>.
	''' A button is created for each object depending on its type:
	''' <dl compact>
	''' <dt>Component<dd>The component is added to the button row directly.
	''' <dt>Icon<dd>A <code>JButton</code> is created with this as its label.
	''' <dt>other<dd>The <code>Object</code> is converted to a string using its
	'''              <code>toString</code> method and the result is used to
	'''              label a <code>JButton</code>.
	''' </dl>
	''' <dt>icon<dd>A decorative icon to be placed in the dialog box. A default
	''' value for this is determined by the <code>messageType</code> parameter.
	''' <dt>title<dd>The title for the dialog box.
	''' <dt>initialValue<dd>The default selection (input value).
	''' </dl>
	''' </blockquote>
	''' <p>
	''' When the selection is changed, <code>setValue</code> is invoked,
	''' which generates a <code>PropertyChangeEvent</code>.
	''' <p>
	''' If a <code>JOptionPane</code> has configured to all input
	''' <code>setWantsInput</code>
	''' the bound property <code>JOptionPane.INPUT_VALUE_PROPERTY</code>
	'''  can also be listened
	''' to, to determine when the user has input or selected a value.
	''' <p>
	''' When one of the <code>showXxxDialog</code> methods returns an integer,
	''' the possible values are:
	''' <ul>
	''' <li><code>YES_OPTION</code>
	''' <li><code>NO_OPTION</code>
	''' <li><code>CANCEL_OPTION</code>
	''' <li><code>OK_OPTION</code>
	''' <li><code>CLOSED_OPTION</code>
	''' </ul>
	''' <b>Examples:</b>
	''' <dl>
	''' <dt>Show an error dialog that displays the message, 'alert':
	''' <dd><code>
	''' JOptionPane.showMessageDialog(null, "alert", "alert", JOptionPane.ERROR_MESSAGE);
	''' </code>
	''' <dt>Show an internal information dialog with the message, 'information':
	''' <dd><pre>
	''' JOptionPane.showInternalMessageDialog(frame, "information",
	'''             "information", JOptionPane.INFORMATION_MESSAGE);
	''' </pre>
	''' <dt>Show an information panel with the options yes/no and message 'choose one':
	''' <dd><pre>JOptionPane.showConfirmDialog(null,
	'''             "choose one", "choose one", JOptionPane.YES_NO_OPTION);
	''' </pre>
	''' <dt>Show an internal information dialog with the options yes/no/cancel and
	''' message 'please choose one' and title information:
	''' <dd><pre>JOptionPane.showInternalConfirmDialog(frame,
	'''             "please choose one", "information",
	'''             JOptionPane.YES_NO_CANCEL_OPTION, JOptionPane.INFORMATION_MESSAGE);
	''' </pre>
	''' <dt>Show a warning dialog with the options OK, CANCEL, title 'Warning', and
	''' message 'Click OK to continue':
	''' <dd><pre>
	''' Object[] options = { "OK", "CANCEL" };
	''' JOptionPane.showOptionDialog(null, "Click OK to continue", "Warning",
	'''             JOptionPane.DEFAULT_OPTION, JOptionPane.WARNING_MESSAGE,
	'''             null, options, options[0]);
	''' </pre>
	''' <dt>Show a dialog asking the user to type in a String:
	''' <dd><code>
	''' String inputValue = JOptionPane.showInputDialog("Please input a value");
	''' </code>
	''' <dt>Show a dialog asking the user to select a String:
	''' <dd><pre>
	''' Object[] possibleValues = { "First", "Second", "Third" };<br>
	''' Object selectedValue = JOptionPane.showInputDialog(null,
	'''             "Choose one", "Input",
	'''             JOptionPane.INFORMATION_MESSAGE, null,
	'''             possibleValues, possibleValues[0]);
	''' </pre><p>
	''' </dl>
	''' <b>Direct Use:</b><br>
	''' To create and use an <code>JOptionPane</code> directly, the
	''' standard pattern is roughly as follows:
	''' <pre>
	'''     JOptionPane pane = new JOptionPane(<i>arguments</i>);
	'''     pane.set<i>.Xxxx(...); // Configure</i>
	'''     JDialog dialog = pane.createDialog(<i>parentComponent, title</i>);
	'''     dialog.show();
	'''     Object selectedValue = pane.getValue();
	'''     if(selectedValue == null)
	'''       return CLOSED_OPTION;
	'''     <i>//If there is <b>not</b> an array of option buttons:</i>
	'''     if(options == null) {
	'''       if(selectedValue instanceof Integer)
	'''          return ((Integer)selectedValue).intValue();
	'''       return CLOSED_OPTION;
	'''     }
	'''     <i>//If there is an array of option buttons:</i>
	'''     for(int counter = 0, maxCounter = options.length;
	'''        counter &lt; maxCounter; counter++) {
	'''        if(options[counter].equals(selectedValue))
	'''        return counter;
	'''     }
	'''     return CLOSED_OPTION;
	''' </pre>
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
	''' </summary>
	''' <seealso cref= JInternalFrame
	''' 
	''' @beaninfo
	'''      attribute: isContainer true
	'''    description: A component which implements standard dialog box controls.
	''' 
	''' @author James Gosling
	''' @author Scott Violet </seealso>
	Public Class JOptionPane
		Inherits JComponent
		Implements Accessible

		''' <seealso cref= #getUIClassID </seealso>
		''' <seealso cref= #readObject </seealso>
		Private Const uiClassID As String = "OptionPaneUI"

		''' <summary>
		''' Indicates that the user has not yet selected a value.
		''' </summary>
		Public Const UNINITIALIZED_VALUE As Object = "uninitializedValue"

		'
		' Option types
		'

		''' <summary>
		''' Type meaning Look and Feel should not supply any options -- only
		''' use the options from the <code>JOptionPane</code>.
		''' </summary>
		Public Const DEFAULT_OPTION As Integer = -1
		''' <summary>
		''' Type used for <code>showConfirmDialog</code>. </summary>
		Public Const YES_NO_OPTION As Integer = 0
		''' <summary>
		''' Type used for <code>showConfirmDialog</code>. </summary>
		Public Const YES_NO_CANCEL_OPTION As Integer = 1
		''' <summary>
		''' Type used for <code>showConfirmDialog</code>. </summary>
		Public Const OK_CANCEL_OPTION As Integer = 2

		'
		' Return values.
		'
		''' <summary>
		''' Return value from class method if YES is chosen. </summary>
		Public Const YES_OPTION As Integer = 0
		''' <summary>
		''' Return value from class method if NO is chosen. </summary>
		Public Const NO_OPTION As Integer = 1
		''' <summary>
		''' Return value from class method if CANCEL is chosen. </summary>
		Public Const CANCEL_OPTION As Integer = 2
		''' <summary>
		''' Return value form class method if OK is chosen. </summary>
		Public Const OK_OPTION As Integer = 0
		''' <summary>
		''' Return value from class method if user closes window without selecting
		''' anything, more than likely this should be treated as either a
		''' <code>CANCEL_OPTION</code> or <code>NO_OPTION</code>. 
		''' </summary>
		Public Const CLOSED_OPTION As Integer = -1

		'
		' Message types. Used by the UI to determine what icon to display,
		' and possibly what behavior to give based on the type.
		'
		''' <summary>
		''' Used for error messages. </summary>
		Public Const ERROR_MESSAGE As Integer = 0
		''' <summary>
		''' Used for information messages. </summary>
		Public Const INFORMATION_MESSAGE As Integer = 1
		''' <summary>
		''' Used for warning messages. </summary>
		Public Const WARNING_MESSAGE As Integer = 2
		''' <summary>
		''' Used for questions. </summary>
		Public Const QUESTION_MESSAGE As Integer = 3
		''' <summary>
		''' No icon is used. </summary>
		Public Const PLAIN_MESSAGE As Integer = -1

		''' <summary>
		''' Bound property name for <code>icon</code>. </summary>
		Public Const ICON_PROPERTY As String = "icon"
		''' <summary>
		''' Bound property name for <code>message</code>. </summary>
		Public Const MESSAGE_PROPERTY As String = "message"
		''' <summary>
		''' Bound property name for <code>value</code>. </summary>
		Public Const VALUE_PROPERTY As String = "value"
		''' <summary>
		''' Bound property name for <code>option</code>. </summary>
		Public Const OPTIONS_PROPERTY As String = "options"
		''' <summary>
		''' Bound property name for <code>initialValue</code>. </summary>
		Public Const INITIAL_VALUE_PROPERTY As String = "initialValue"
		''' <summary>
		''' Bound property name for <code>type</code>. </summary>
		Public Const MESSAGE_TYPE_PROPERTY As String = "messageType"
		''' <summary>
		''' Bound property name for <code>optionType</code>. </summary>
		Public Const OPTION_TYPE_PROPERTY As String = "optionType"
		''' <summary>
		''' Bound property name for <code>selectionValues</code>. </summary>
		Public Const SELECTION_VALUES_PROPERTY As String = "selectionValues"
		''' <summary>
		''' Bound property name for <code>initialSelectionValue</code>. </summary>
		Public Const INITIAL_SELECTION_VALUE_PROPERTY As String = "initialSelectionValue"
		''' <summary>
		''' Bound property name for <code>inputValue</code>. </summary>
		Public Const INPUT_VALUE_PROPERTY As String = "inputValue"
		''' <summary>
		''' Bound property name for <code>wantsInput</code>. </summary>
		Public Const WANTS_INPUT_PROPERTY As String = "wantsInput"

		''' <summary>
		''' Icon used in pane. </summary>
		<NonSerialized> _
		Protected Friend icon As Icon
		''' <summary>
		''' Message to display. </summary>
		<NonSerialized> _
		Protected Friend message As Object
		''' <summary>
		''' Options to display to the user. </summary>
		<NonSerialized> _
		Protected Friend options As Object()
		''' <summary>
		''' Value that should be initially selected in <code>options</code>. </summary>
		<NonSerialized> _
		Protected Friend initialValue As Object
		''' <summary>
		''' Message type. </summary>
		Protected Friend messageType As Integer
		''' <summary>
		''' Option type, one of <code>DEFAULT_OPTION</code>,
		''' <code>YES_NO_OPTION</code>,
		''' <code>YES_NO_CANCEL_OPTION</code> or
		''' <code>OK_CANCEL_OPTION</code>.
		''' </summary>
		Protected Friend optionType As Integer
		''' <summary>
		''' Currently selected value, will be a valid option, or
		''' <code>UNINITIALIZED_VALUE</code> or <code>null</code>. 
		''' </summary>
		<NonSerialized> _
		Protected Friend value As Object
		''' <summary>
		''' Array of values the user can choose from. Look and feel will
		''' provide the UI component to choose this from. 
		''' </summary>
		<NonSerialized> _
		Protected Friend selectionValues As Object()
		''' <summary>
		''' Value the user has input. </summary>
		<NonSerialized> _
		Protected Friend inputValue As Object
		''' <summary>
		''' Initial value to select in <code>selectionValues</code>. </summary>
		<NonSerialized> _
		Protected Friend initialSelectionValue As Object
		''' <summary>
		''' If true, a UI widget will be provided to the user to get input. </summary>
		Protected Friend wantsInput As Boolean


		''' <summary>
		''' Shows a question-message dialog requesting input from the user. The
		''' dialog uses the default frame, which usually means it is centered on
		''' the screen.
		''' </summary>
		''' <param name="message"> the <code>Object</code> to display </param>
		''' <exception cref="HeadlessException"> if
		'''   <code>GraphicsEnvironment.isHeadless</code> returns
		'''   <code>true</code> </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		Public Shared Function showInputDialog(ByVal message As Object) As String
			Return showInputDialog(Nothing, message)
		End Function

		''' <summary>
		''' Shows a question-message dialog requesting input from the user, with
		''' the input value initialized to <code>initialSelectionValue</code>. The
		''' dialog uses the default frame, which usually means it is centered on
		''' the screen.
		''' </summary>
		''' <param name="message"> the <code>Object</code> to display </param>
		''' <param name="initialSelectionValue"> the value used to initialize the input
		'''                 field
		''' @since 1.4 </param>
		Public Shared Function showInputDialog(ByVal message As Object, ByVal initialSelectionValue As Object) As String
			Return showInputDialog(Nothing, message, initialSelectionValue)
		End Function

		''' <summary>
		''' Shows a question-message dialog requesting input from the user
		''' parented to <code>parentComponent</code>.
		''' The dialog is displayed on top of the <code>Component</code>'s
		''' frame, and is usually positioned below the <code>Component</code>.
		''' </summary>
		''' <param name="parentComponent">  the parent <code>Component</code> for the
		'''          dialog </param>
		''' <param name="message">  the <code>Object</code> to display </param>
		''' <exception cref="HeadlessException"> if
		'''    <code>GraphicsEnvironment.isHeadless</code> returns
		'''    <code>true</code> </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		Public Shared Function showInputDialog(ByVal parentComponent As java.awt.Component, ByVal message As Object) As String
			Return showInputDialog(parentComponent, message, UIManager.getString("OptionPane.inputDialogTitle", parentComponent), QUESTION_MESSAGE)
		End Function

		''' <summary>
		''' Shows a question-message dialog requesting input from the user and
		''' parented to <code>parentComponent</code>. The input value will be
		''' initialized to <code>initialSelectionValue</code>.
		''' The dialog is displayed on top of the <code>Component</code>'s
		''' frame, and is usually positioned below the <code>Component</code>.
		''' </summary>
		''' <param name="parentComponent">  the parent <code>Component</code> for the
		'''          dialog </param>
		''' <param name="message"> the <code>Object</code> to display </param>
		''' <param name="initialSelectionValue"> the value used to initialize the input
		'''                 field
		''' @since 1.4 </param>
		Public Shared Function showInputDialog(ByVal parentComponent As java.awt.Component, ByVal message As Object, ByVal initialSelectionValue As Object) As String
			Return CStr(showInputDialog(parentComponent, message, UIManager.getString("OptionPane.inputDialogTitle", parentComponent), QUESTION_MESSAGE, Nothing, Nothing, initialSelectionValue))
		End Function

		''' <summary>
		''' Shows a dialog requesting input from the user parented to
		''' <code>parentComponent</code> with the dialog having the title
		''' <code>title</code> and message type <code>messageType</code>.
		''' </summary>
		''' <param name="parentComponent">  the parent <code>Component</code> for the
		'''                  dialog </param>
		''' <param name="message">  the <code>Object</code> to display </param>
		''' <param name="title">    the <code>String</code> to display in the dialog
		'''                  title bar </param>
		''' <param name="messageType"> the type of message that is to be displayed:
		'''                  <code>ERROR_MESSAGE</code>,
		'''                  <code>INFORMATION_MESSAGE</code>,
		'''                  <code>WARNING_MESSAGE</code>,
		'''                  <code>QUESTION_MESSAGE</code>,
		'''                  or <code>PLAIN_MESSAGE</code> </param>
		''' <exception cref="HeadlessException"> if
		'''   <code>GraphicsEnvironment.isHeadless</code> returns
		'''   <code>true</code> </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		Public Shared Function showInputDialog(ByVal parentComponent As java.awt.Component, ByVal message As Object, ByVal title As String, ByVal messageType As Integer) As String
			Return CStr(showInputDialog(parentComponent, message, title, messageType, Nothing, Nothing, Nothing))
		End Function

		''' <summary>
		''' Prompts the user for input in a blocking dialog where the
		''' initial selection, possible selections, and all other options can
		''' be specified. The user will able to choose from
		''' <code>selectionValues</code>, where <code>null</code> implies the
		''' user can input
		''' whatever they wish, usually by means of a <code>JTextField</code>.
		''' <code>initialSelectionValue</code> is the initial value to prompt
		''' the user with. It is up to the UI to decide how best to represent
		''' the <code>selectionValues</code>, but usually a
		''' <code>JComboBox</code>, <code>JList</code>, or
		''' <code>JTextField</code> will be used.
		''' </summary>
		''' <param name="parentComponent">  the parent <code>Component</code> for the
		'''                  dialog </param>
		''' <param name="message">  the <code>Object</code> to display </param>
		''' <param name="title">    the <code>String</code> to display in the
		'''                  dialog title bar </param>
		''' <param name="messageType"> the type of message to be displayed:
		'''                  <code>ERROR_MESSAGE</code>,
		'''                  <code>INFORMATION_MESSAGE</code>,
		'''                  <code>WARNING_MESSAGE</code>,
		'''                  <code>QUESTION_MESSAGE</code>,
		'''                  or <code>PLAIN_MESSAGE</code> </param>
		''' <param name="icon">     the <code>Icon</code> image to display </param>
		''' <param name="selectionValues"> an array of <code>Object</code>s that
		'''                  gives the possible selections </param>
		''' <param name="initialSelectionValue"> the value used to initialize the input
		'''                 field </param>
		''' <returns> user's input, or <code>null</code> meaning the user
		'''                  canceled the input </returns>
		''' <exception cref="HeadlessException"> if
		'''   <code>GraphicsEnvironment.isHeadless</code> returns
		'''   <code>true</code> </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		Public Shared Function showInputDialog(ByVal parentComponent As java.awt.Component, ByVal message As Object, ByVal title As String, ByVal messageType As Integer, ByVal icon As Icon, ByVal selectionValues As Object(), ByVal initialSelectionValue As Object) As Object
			Dim pane As New JOptionPane(message, messageType, OK_CANCEL_OPTION, icon, Nothing, Nothing)

			pane.wantsInput = True
			pane.selectionValues = selectionValues
			pane.initialSelectionValue = initialSelectionValue
			pane.componentOrientation = (If(parentComponent Is Nothing, rootFrame, parentComponent)).componentOrientation

			Dim style As Integer = styleFromMessageType(messageType)
			Dim dialog As JDialog = pane.createDialog(parentComponent, title, style)

			pane.selectInitialValue()
			dialog.show()
			dialog.Dispose()

			Dim ___value As Object = pane.inputValue

			If ___value Is UNINITIALIZED_VALUE Then Return Nothing
			Return ___value
		End Function

		''' <summary>
		''' Brings up an information-message dialog titled "Message".
		''' </summary>
		''' <param name="parentComponent"> determines the <code>Frame</code> in
		'''          which the dialog is displayed; if <code>null</code>,
		'''          or if the <code>parentComponent</code> has no
		'''          <code>Frame</code>, a default <code>Frame</code> is used </param>
		''' <param name="message">   the <code>Object</code> to display </param>
		''' <exception cref="HeadlessException"> if
		'''   <code>GraphicsEnvironment.isHeadless</code> returns
		'''   <code>true</code> </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		Public Shared Sub showMessageDialog(ByVal parentComponent As java.awt.Component, ByVal message As Object)
			showMessageDialog(parentComponent, message, UIManager.getString("OptionPane.messageDialogTitle", parentComponent), INFORMATION_MESSAGE)
		End Sub

		''' <summary>
		''' Brings up a dialog that displays a message using a default
		''' icon determined by the <code>messageType</code> parameter.
		''' </summary>
		''' <param name="parentComponent"> determines the <code>Frame</code>
		'''          in which the dialog is displayed; if <code>null</code>,
		'''          or if the <code>parentComponent</code> has no
		'''          <code>Frame</code>, a default <code>Frame</code> is used </param>
		''' <param name="message">   the <code>Object</code> to display </param>
		''' <param name="title">     the title string for the dialog </param>
		''' <param name="messageType"> the type of message to be displayed:
		'''                  <code>ERROR_MESSAGE</code>,
		'''                  <code>INFORMATION_MESSAGE</code>,
		'''                  <code>WARNING_MESSAGE</code>,
		'''                  <code>QUESTION_MESSAGE</code>,
		'''                  or <code>PLAIN_MESSAGE</code> </param>
		''' <exception cref="HeadlessException"> if
		'''   <code>GraphicsEnvironment.isHeadless</code> returns
		'''   <code>true</code> </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		Public Shared Sub showMessageDialog(ByVal parentComponent As java.awt.Component, ByVal message As Object, ByVal title As String, ByVal messageType As Integer)
			showMessageDialog(parentComponent, message, title, messageType, Nothing)
		End Sub

		''' <summary>
		''' Brings up a dialog displaying a message, specifying all parameters.
		''' </summary>
		''' <param name="parentComponent"> determines the <code>Frame</code> in which the
		'''                  dialog is displayed; if <code>null</code>,
		'''                  or if the <code>parentComponent</code> has no
		'''                  <code>Frame</code>, a
		'''                  default <code>Frame</code> is used </param>
		''' <param name="message">   the <code>Object</code> to display </param>
		''' <param name="title">     the title string for the dialog </param>
		''' <param name="messageType"> the type of message to be displayed:
		'''                  <code>ERROR_MESSAGE</code>,
		'''                  <code>INFORMATION_MESSAGE</code>,
		'''                  <code>WARNING_MESSAGE</code>,
		'''                  <code>QUESTION_MESSAGE</code>,
		'''                  or <code>PLAIN_MESSAGE</code> </param>
		''' <param name="icon">      an icon to display in the dialog that helps the user
		'''                  identify the kind of message that is being displayed </param>
		''' <exception cref="HeadlessException"> if
		'''   <code>GraphicsEnvironment.isHeadless</code> returns
		'''   <code>true</code> </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		Public Shared Sub showMessageDialog(ByVal parentComponent As java.awt.Component, ByVal message As Object, ByVal title As String, ByVal messageType As Integer, ByVal icon As Icon)
			showOptionDialog(parentComponent, message, title, DEFAULT_OPTION, messageType, icon, Nothing, Nothing)
		End Sub

		''' <summary>
		''' Brings up a dialog with the options <i>Yes</i>,
		''' <i>No</i> and <i>Cancel</i>; with the
		''' title, <b>Select an Option</b>.
		''' </summary>
		''' <param name="parentComponent"> determines the <code>Frame</code> in which the
		'''                  dialog is displayed; if <code>null</code>,
		'''                  or if the <code>parentComponent</code> has no
		'''                  <code>Frame</code>, a
		'''                  default <code>Frame</code> is used </param>
		''' <param name="message">   the <code>Object</code> to display </param>
		''' <returns> an integer indicating the option selected by the user </returns>
		''' <exception cref="HeadlessException"> if
		'''   <code>GraphicsEnvironment.isHeadless</code> returns
		'''   <code>true</code> </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		Public Shared Function showConfirmDialog(ByVal parentComponent As java.awt.Component, ByVal message As Object) As Integer
			Return MessageBox.Show(parentComponent, message, UIManager.getString("OptionPane.titleText"), MessageBoxButtons.YesNoCancel)
		End Function

		''' <summary>
		''' Brings up a dialog where the number of choices is determined
		''' by the <code>optionType</code> parameter.
		''' </summary>
		''' <param name="parentComponent"> determines the <code>Frame</code> in which the
		'''                  dialog is displayed; if <code>null</code>,
		'''                  or if the <code>parentComponent</code> has no
		'''                  <code>Frame</code>, a
		'''                  default <code>Frame</code> is used </param>
		''' <param name="message">   the <code>Object</code> to display </param>
		''' <param name="title">     the title string for the dialog </param>
		''' <param name="optionType"> an int designating the options available on the dialog:
		'''                  <code>YES_NO_OPTION</code>,
		'''                  <code>YES_NO_CANCEL_OPTION</code>,
		'''                  or <code>OK_CANCEL_OPTION</code> </param>
		''' <returns> an int indicating the option selected by the user </returns>
		''' <exception cref="HeadlessException"> if
		'''   <code>GraphicsEnvironment.isHeadless</code> returns
		'''   <code>true</code> </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		Public Shared Function showConfirmDialog(ByVal parentComponent As java.awt.Component, ByVal message As Object, ByVal title As String, ByVal optionType As Integer) As Integer
			Return MessageBox.Show(parentComponent, message, title, , MessageBoxIcon.Question)
		End Function

		''' <summary>
		''' Brings up a dialog where the number of choices is determined
		''' by the <code>optionType</code> parameter, where the
		''' <code>messageType</code>
		''' parameter determines the icon to display.
		''' The <code>messageType</code> parameter is primarily used to supply
		''' a default icon from the Look and Feel.
		''' </summary>
		''' <param name="parentComponent"> determines the <code>Frame</code> in
		'''                  which the dialog is displayed; if <code>null</code>,
		'''                  or if the <code>parentComponent</code> has no
		'''                  <code>Frame</code>, a
		'''                  default <code>Frame</code> is used. </param>
		''' <param name="message">   the <code>Object</code> to display </param>
		''' <param name="title">     the title string for the dialog </param>
		''' <param name="optionType"> an integer designating the options available
		'''                   on the dialog: <code>YES_NO_OPTION</code>,
		'''                  <code>YES_NO_CANCEL_OPTION</code>,
		'''                  or <code>OK_CANCEL_OPTION</code> </param>
		''' <param name="messageType"> an integer designating the kind of message this is;
		'''                  primarily used to determine the icon from the pluggable
		'''                  Look and Feel: <code>ERROR_MESSAGE</code>,
		'''                  <code>INFORMATION_MESSAGE</code>,
		'''                  <code>WARNING_MESSAGE</code>,
		'''                  <code>QUESTION_MESSAGE</code>,
		'''                  or <code>PLAIN_MESSAGE</code> </param>
		''' <returns> an integer indicating the option selected by the user </returns>
		''' <exception cref="HeadlessException"> if
		'''   <code>GraphicsEnvironment.isHeadless</code> returns
		'''   <code>true</code> </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		Public Shared Function showConfirmDialog(ByVal parentComponent As java.awt.Component, ByVal message As Object, ByVal title As String, ByVal optionType As Integer, ByVal messageType As Integer) As Integer
			Return showConfirmDialog(parentComponent, message, title, optionType, messageType, Nothing)
		End Function

		''' <summary>
		''' Brings up a dialog with a specified icon, where the number of
		''' choices is determined by the <code>optionType</code> parameter.
		''' The <code>messageType</code> parameter is primarily used to supply
		''' a default icon from the look and feel.
		''' </summary>
		''' <param name="parentComponent"> determines the <code>Frame</code> in which the
		'''                  dialog is displayed; if <code>null</code>,
		'''                  or if the <code>parentComponent</code> has no
		'''                  <code>Frame</code>, a
		'''                  default <code>Frame</code> is used </param>
		''' <param name="message">   the Object to display </param>
		''' <param name="title">     the title string for the dialog </param>
		''' <param name="optionType"> an int designating the options available on the dialog:
		'''                  <code>YES_NO_OPTION</code>,
		'''                  <code>YES_NO_CANCEL_OPTION</code>,
		'''                  or <code>OK_CANCEL_OPTION</code> </param>
		''' <param name="messageType"> an int designating the kind of message this is,
		'''                  primarily used to determine the icon from the pluggable
		'''                  Look and Feel: <code>ERROR_MESSAGE</code>,
		'''                  <code>INFORMATION_MESSAGE</code>,
		'''                  <code>WARNING_MESSAGE</code>,
		'''                  <code>QUESTION_MESSAGE</code>,
		'''                  or <code>PLAIN_MESSAGE</code> </param>
		''' <param name="icon">      the icon to display in the dialog </param>
		''' <returns> an int indicating the option selected by the user </returns>
		''' <exception cref="HeadlessException"> if
		'''   <code>GraphicsEnvironment.isHeadless</code> returns
		'''   <code>true</code> </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		Public Shared Function showConfirmDialog(ByVal parentComponent As java.awt.Component, ByVal message As Object, ByVal title As String, ByVal optionType As Integer, ByVal messageType As Integer, ByVal icon As Icon) As Integer
			Return showOptionDialog(parentComponent, message, title, optionType, messageType, icon, Nothing, Nothing)
		End Function

		''' <summary>
		''' Brings up a dialog with a specified icon, where the initial
		''' choice is determined by the <code>initialValue</code> parameter and
		''' the number of choices is determined by the <code>optionType</code>
		''' parameter.
		''' <p>
		''' If <code>optionType</code> is <code>YES_NO_OPTION</code>,
		''' or <code>YES_NO_CANCEL_OPTION</code>
		''' and the <code>options</code> parameter is <code>null</code>,
		''' then the options are
		''' supplied by the look and feel.
		''' <p>
		''' The <code>messageType</code> parameter is primarily used to supply
		''' a default icon from the look and feel.
		''' </summary>
		''' <param name="parentComponent"> determines the <code>Frame</code>
		'''                  in which the dialog is displayed;  if
		'''                  <code>null</code>, or if the
		'''                  <code>parentComponent</code> has no
		'''                  <code>Frame</code>, a
		'''                  default <code>Frame</code> is used </param>
		''' <param name="message">   the <code>Object</code> to display </param>
		''' <param name="title">     the title string for the dialog </param>
		''' <param name="optionType"> an integer designating the options available on the
		'''                  dialog: <code>DEFAULT_OPTION</code>,
		'''                  <code>YES_NO_OPTION</code>,
		'''                  <code>YES_NO_CANCEL_OPTION</code>,
		'''                  or <code>OK_CANCEL_OPTION</code> </param>
		''' <param name="messageType"> an integer designating the kind of message this is,
		'''                  primarily used to determine the icon from the
		'''                  pluggable Look and Feel: <code>ERROR_MESSAGE</code>,
		'''                  <code>INFORMATION_MESSAGE</code>,
		'''                  <code>WARNING_MESSAGE</code>,
		'''                  <code>QUESTION_MESSAGE</code>,
		'''                  or <code>PLAIN_MESSAGE</code> </param>
		''' <param name="icon">      the icon to display in the dialog </param>
		''' <param name="options">   an array of objects indicating the possible choices
		'''                  the user can make; if the objects are components, they
		'''                  are rendered properly; non-<code>String</code>
		'''                  objects are
		'''                  rendered using their <code>toString</code> methods;
		'''                  if this parameter is <code>null</code>,
		'''                  the options are determined by the Look and Feel </param>
		''' <param name="initialValue"> the object that represents the default selection
		'''                  for the dialog; only meaningful if <code>options</code>
		'''                  is used; can be <code>null</code> </param>
		''' <returns> an integer indicating the option chosen by the user,
		'''                  or <code>CLOSED_OPTION</code> if the user closed
		'''                  the dialog </returns>
		''' <exception cref="HeadlessException"> if
		'''   <code>GraphicsEnvironment.isHeadless</code> returns
		'''   <code>true</code> </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		Public Shared Function showOptionDialog(ByVal parentComponent As java.awt.Component, ByVal message As Object, ByVal title As String, ByVal optionType As Integer, ByVal messageType As Integer, ByVal icon As Icon, ByVal options As Object(), ByVal initialValue As Object) As Integer
			Dim pane As New JOptionPane(message, messageType, optionType, icon, options, initialValue)

			pane.initialValue = initialValue
			pane.componentOrientation = (If(parentComponent Is Nothing, rootFrame, parentComponent)).componentOrientation

			Dim style As Integer = styleFromMessageType(messageType)
			Dim dialog As JDialog = pane.createDialog(parentComponent, title, style)

			pane.selectInitialValue()
			dialog.show()
			dialog.Dispose()

			Dim selectedValue As Object = pane.value

			If selectedValue Is Nothing Then Return CLOSED_OPTION
			If options Is Nothing Then
				If TypeOf selectedValue Is Integer? Then Return CInt(Fix(selectedValue))
				Return CLOSED_OPTION
			End If
			Dim counter As Integer = 0
			Dim maxCounter As Integer = options.Length
			Do While counter < maxCounter
				If options(counter).Equals(selectedValue) Then Return counter
				counter += 1
			Loop
			Return CLOSED_OPTION
		End Function

		''' <summary>
		''' Creates and returns a new <code>JDialog</code> wrapping
		''' <code>this</code> centered on the <code>parentComponent</code>
		''' in the <code>parentComponent</code>'s frame.
		''' <code>title</code> is the title of the returned dialog.
		''' The returned <code>JDialog</code> will not be resizable by the
		''' user, however programs can invoke <code>setResizable</code> on
		''' the <code>JDialog</code> instance to change this property.
		''' The returned <code>JDialog</code> will be set up such that
		''' once it is closed, or the user clicks on one of the buttons,
		''' the optionpane's value property will be set accordingly and
		''' the dialog will be closed.  Each time the dialog is made visible,
		''' it will reset the option pane's value property to
		''' <code>JOptionPane.UNINITIALIZED_VALUE</code> to ensure the
		''' user's subsequent action closes the dialog properly.
		''' </summary>
		''' <param name="parentComponent"> determines the frame in which the dialog
		'''          is displayed; if the <code>parentComponent</code> has
		'''          no <code>Frame</code>, a default <code>Frame</code> is used </param>
		''' <param name="title">     the title string for the dialog </param>
		''' <returns> a new <code>JDialog</code> containing this instance </returns>
		''' <exception cref="HeadlessException"> if
		'''   <code>GraphicsEnvironment.isHeadless</code> returns
		'''   <code>true</code> </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		Public Overridable Function createDialog(ByVal parentComponent As java.awt.Component, ByVal title As String) As JDialog
			Dim style As Integer = styleFromMessageType(messageType)
			Return createDialog(parentComponent, title, style)
		End Function

		''' <summary>
		''' Creates and returns a new parentless <code>JDialog</code>
		''' with the specified title.
		''' The returned <code>JDialog</code> will not be resizable by the
		''' user, however programs can invoke <code>setResizable</code> on
		''' the <code>JDialog</code> instance to change this property.
		''' The returned <code>JDialog</code> will be set up such that
		''' once it is closed, or the user clicks on one of the buttons,
		''' the optionpane's value property will be set accordingly and
		''' the dialog will be closed.  Each time the dialog is made visible,
		''' it will reset the option pane's value property to
		''' <code>JOptionPane.UNINITIALIZED_VALUE</code> to ensure the
		''' user's subsequent action closes the dialog properly.
		''' </summary>
		''' <param name="title">     the title string for the dialog </param>
		''' <returns> a new <code>JDialog</code> containing this instance </returns>
		''' <exception cref="HeadlessException"> if
		'''   <code>GraphicsEnvironment.isHeadless</code> returns
		'''   <code>true</code> </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless
		''' @since 1.6 </seealso>
		Public Overridable Function createDialog(ByVal title As String) As JDialog
			Dim style As Integer = styleFromMessageType(messageType)
			Dim dialog As New JDialog(CType(Nothing, java.awt.Dialog), title, True)
			initDialog(dialog, style, Nothing)
			Return dialog
		End Function

		Private Function createDialog(ByVal parentComponent As java.awt.Component, ByVal title As String, ByVal style As Integer) As JDialog

			Dim dialog As JDialog

			Dim window As java.awt.Window = JOptionPane.getWindowForComponent(parentComponent)
			If TypeOf window Is java.awt.Frame Then
				dialog = New JDialog(CType(window, java.awt.Frame), title, True)
			Else
				dialog = New JDialog(CType(window, java.awt.Dialog), title, True)
			End If
			If TypeOf window Is SwingUtilities.SharedOwnerFrame Then
				Dim ownerShutdownListener As java.awt.event.WindowListener = SwingUtilities.sharedOwnerFrameShutdownListener
				dialog.addWindowListener(ownerShutdownListener)
			End If
			initDialog(dialog, style, parentComponent)
			Return dialog
		End Function

		Private Sub initDialog(ByVal dialog As JDialog, ByVal style As Integer, ByVal parentComponent As java.awt.Component)
			dialog.componentOrientation = Me.componentOrientation
			Dim contentPane As java.awt.Container = dialog.contentPane

			contentPane.layout = New java.awt.BorderLayout
			contentPane.add(Me, java.awt.BorderLayout.CENTER)
			dialog.resizable = False
			If JDialog.defaultLookAndFeelDecorated Then
				Dim supportsWindowDecorations As Boolean = UIManager.lookAndFeel.supportsWindowDecorations
				If supportsWindowDecorations Then
					dialog.undecorated = True
					rootPane.windowDecorationStyle = style
				End If
			End If
			dialog.pack()
			dialog.locationRelativeTo = parentComponent

'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			final java.beans.PropertyChangeListener listener = New java.beans.PropertyChangeListener()
	'		{
	'			public void propertyChange(PropertyChangeEvent event)
	'			{
	'				' Let the defaultCloseOperation handle the closing
	'				' if the user closed the window without selecting a button
	'				' (newValue = null in that case).  Otherwise, close the dialog.
	'				if (dialog.isVisible() && event.getSource() == JOptionPane.this && (event.getPropertyName().equals(VALUE_PROPERTY)) && event.getNewValue() != Nothing && event.getNewValue() != JOptionPane.UNINITIALIZED_VALUE)
	'				{
	'					dialog.setVisible(False);
	'				}
	'			}
	'		};

'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			java.awt.event.WindowAdapter adapter = New java.awt.event.WindowAdapter()
	'		{
	'			private boolean gotFocus = False;
	'			public void windowClosing(WindowEvent we)
	'			{
	'				setValue(Nothing);
	'			}
	'
	'			public void windowClosed(WindowEvent e)
	'			{
	'				removePropertyChangeListener(listener);
	'				dialog.getContentPane().removeAll();
	'			}
	'
	'			public void windowGainedFocus(WindowEvent we)
	'			{
	'				' Once window gets focus, set initial focus
	'				if (!gotFocus)
	'				{
	'					selectInitialValue();
	'					gotFocus = True;
	'				}
	'			}
	'		};
			dialog.addWindowListener(adapter)
			dialog.addWindowFocusListener(adapter)
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			dialog.addComponentListener(New java.awt.event.ComponentAdapter()
	'		{
	'			public void componentShown(ComponentEvent ce)
	'			{
	'				' reset value to ensure closing works properly
	'				setValue(JOptionPane.UNINITIALIZED_VALUE);
	'			}
	'		});

			addPropertyChangeListener(listener)
		End Sub


		''' <summary>
		''' Brings up an internal confirmation dialog panel. The dialog
		''' is a information-message dialog titled "Message".
		''' </summary>
		''' <param name="parentComponent"> determines the <code>Frame</code>
		'''          in which the dialog is displayed; if <code>null</code>,
		'''          or if the <code>parentComponent</code> has no
		'''          <code>Frame</code>, a default <code>Frame</code> is used </param>
		''' <param name="message">   the object to display </param>
		Public Shared Sub showInternalMessageDialog(ByVal parentComponent As java.awt.Component, ByVal message As Object)
			showInternalMessageDialog(parentComponent, message, UIManager.getString("OptionPane.messageDialogTitle", parentComponent), INFORMATION_MESSAGE)
		End Sub

		''' <summary>
		''' Brings up an internal dialog panel that displays a message
		''' using a default icon determined by the <code>messageType</code>
		''' parameter.
		''' </summary>
		''' <param name="parentComponent"> determines the <code>Frame</code>
		'''          in which the dialog is displayed; if <code>null</code>,
		'''          or if the <code>parentComponent</code> has no
		'''          <code>Frame</code>, a default <code>Frame</code> is used </param>
		''' <param name="message">   the <code>Object</code> to display </param>
		''' <param name="title">     the title string for the dialog </param>
		''' <param name="messageType"> the type of message to be displayed:
		'''                  <code>ERROR_MESSAGE</code>,
		'''                  <code>INFORMATION_MESSAGE</code>,
		'''                  <code>WARNING_MESSAGE</code>,
		'''                  <code>QUESTION_MESSAGE</code>,
		'''                  or <code>PLAIN_MESSAGE</code> </param>
		Public Shared Sub showInternalMessageDialog(ByVal parentComponent As java.awt.Component, ByVal message As Object, ByVal title As String, ByVal messageType As Integer)
			showInternalMessageDialog(parentComponent, message, title, messageType,Nothing)
		End Sub

		''' <summary>
		''' Brings up an internal dialog panel displaying a message,
		''' specifying all parameters.
		''' </summary>
		''' <param name="parentComponent"> determines the <code>Frame</code>
		'''          in which the dialog is displayed; if <code>null</code>,
		'''          or if the <code>parentComponent</code> has no
		'''          <code>Frame</code>, a default <code>Frame</code> is used </param>
		''' <param name="message">   the <code>Object</code> to display </param>
		''' <param name="title">     the title string for the dialog </param>
		''' <param name="messageType"> the type of message to be displayed:
		'''                  <code>ERROR_MESSAGE</code>,
		'''                  <code>INFORMATION_MESSAGE</code>,
		'''                  <code>WARNING_MESSAGE</code>,
		'''                  <code>QUESTION_MESSAGE</code>,
		'''                  or <code>PLAIN_MESSAGE</code> </param>
		''' <param name="icon">      an icon to display in the dialog that helps the user
		'''                  identify the kind of message that is being displayed </param>
		Public Shared Sub showInternalMessageDialog(ByVal parentComponent As java.awt.Component, ByVal message As Object, ByVal title As String, ByVal messageType As Integer, ByVal icon As Icon)
			showInternalOptionDialog(parentComponent, message, title, DEFAULT_OPTION, messageType, icon, Nothing, Nothing)
		End Sub

		''' <summary>
		''' Brings up an internal dialog panel with the options <i>Yes</i>, <i>No</i>
		''' and <i>Cancel</i>; with the title, <b>Select an Option</b>.
		''' </summary>
		''' <param name="parentComponent"> determines the <code>Frame</code> in
		'''          which the dialog is displayed; if <code>null</code>,
		'''          or if the <code>parentComponent</code> has no
		'''          <code>Frame</code>, a default <code>Frame</code> is used </param>
		''' <param name="message">   the <code>Object</code> to display </param>
		''' <returns> an integer indicating the option selected by the user </returns>
		Public Shared Function showInternalConfirmDialog(ByVal parentComponent As java.awt.Component, ByVal message As Object) As Integer
			Return showInternalConfirmDialog(parentComponent, message, UIManager.getString("OptionPane.titleText"), YES_NO_CANCEL_OPTION)
		End Function

		''' <summary>
		''' Brings up a internal dialog panel where the number of choices
		''' is determined by the <code>optionType</code> parameter.
		''' </summary>
		''' <param name="parentComponent"> determines the <code>Frame</code>
		'''          in which the dialog is displayed; if <code>null</code>,
		'''          or if the <code>parentComponent</code> has no
		'''          <code>Frame</code>, a default <code>Frame</code> is used </param>
		''' <param name="message">   the object to display in the dialog; a
		'''          <code>Component</code> object is rendered as a
		'''          <code>Component</code>; a <code>String</code>
		'''          object is rendered as a string; other objects
		'''          are converted to a <code>String</code> using the
		'''          <code>toString</code> method </param>
		''' <param name="title">     the title string for the dialog </param>
		''' <param name="optionType"> an integer designating the options
		'''          available on the dialog: <code>YES_NO_OPTION</code>,
		'''          or <code>YES_NO_CANCEL_OPTION</code> </param>
		''' <returns> an integer indicating the option selected by the user </returns>
		Public Shared Function showInternalConfirmDialog(ByVal parentComponent As java.awt.Component, ByVal message As Object, ByVal title As String, ByVal optionType As Integer) As Integer
			Return showInternalConfirmDialog(parentComponent, message, title, optionType, QUESTION_MESSAGE)
		End Function

		''' <summary>
		''' Brings up an internal dialog panel where the number of choices
		''' is determined by the <code>optionType</code> parameter, where
		''' the <code>messageType</code> parameter determines the icon to display.
		''' The <code>messageType</code> parameter is primarily used to supply
		''' a default icon from the Look and Feel.
		''' </summary>
		''' <param name="parentComponent"> determines the <code>Frame</code> in
		'''          which the dialog is displayed; if <code>null</code>,
		'''          or if the <code>parentComponent</code> has no
		'''          <code>Frame</code>, a default <code>Frame</code> is used </param>
		''' <param name="message">   the object to display in the dialog; a
		'''          <code>Component</code> object is rendered as a
		'''          <code>Component</code>; a <code>String</code>
		'''          object is rendered as a string; other objects are
		'''          converted to a <code>String</code> using the
		'''          <code>toString</code> method </param>
		''' <param name="title">     the title string for the dialog </param>
		''' <param name="optionType"> an integer designating the options
		'''          available on the dialog:
		'''          <code>YES_NO_OPTION</code>, or <code>YES_NO_CANCEL_OPTION</code> </param>
		''' <param name="messageType"> an integer designating the kind of message this is,
		'''          primarily used to determine the icon from the
		'''          pluggable Look and Feel: <code>ERROR_MESSAGE</code>,
		'''          <code>INFORMATION_MESSAGE</code>,
		'''          <code>WARNING_MESSAGE</code>, <code>QUESTION_MESSAGE</code>,
		'''          or <code>PLAIN_MESSAGE</code> </param>
		''' <returns> an integer indicating the option selected by the user </returns>
		Public Shared Function showInternalConfirmDialog(ByVal parentComponent As java.awt.Component, ByVal message As Object, ByVal title As String, ByVal optionType As Integer, ByVal messageType As Integer) As Integer
			Return showInternalConfirmDialog(parentComponent, message, title, optionType, messageType, Nothing)
		End Function

		''' <summary>
		''' Brings up an internal dialog panel with a specified icon, where
		''' the number of choices is determined by the <code>optionType</code>
		''' parameter.
		''' The <code>messageType</code> parameter is primarily used to supply
		''' a default icon from the look and feel.
		''' </summary>
		''' <param name="parentComponent"> determines the <code>Frame</code>
		'''          in which the dialog is displayed; if <code>null</code>,
		'''          or if the parentComponent has no Frame, a
		'''          default <code>Frame</code> is used </param>
		''' <param name="message">   the object to display in the dialog; a
		'''          <code>Component</code> object is rendered as a
		'''          <code>Component</code>; a <code>String</code>
		'''          object is rendered as a string; other objects are
		'''          converted to a <code>String</code> using the
		'''          <code>toString</code> method </param>
		''' <param name="title">     the title string for the dialog </param>
		''' <param name="optionType"> an integer designating the options available
		'''          on the dialog:
		'''          <code>YES_NO_OPTION</code>, or
		'''          <code>YES_NO_CANCEL_OPTION</code>. </param>
		''' <param name="messageType"> an integer designating the kind of message this is,
		'''          primarily used to determine the icon from the pluggable
		'''          Look and Feel: <code>ERROR_MESSAGE</code>,
		'''          <code>INFORMATION_MESSAGE</code>,
		'''          <code>WARNING_MESSAGE</code>, <code>QUESTION_MESSAGE</code>,
		'''          or <code>PLAIN_MESSAGE</code> </param>
		''' <param name="icon">      the icon to display in the dialog </param>
		''' <returns> an integer indicating the option selected by the user </returns>
		Public Shared Function showInternalConfirmDialog(ByVal parentComponent As java.awt.Component, ByVal message As Object, ByVal title As String, ByVal optionType As Integer, ByVal messageType As Integer, ByVal icon As Icon) As Integer
			Return showInternalOptionDialog(parentComponent, message, title, optionType, messageType, icon, Nothing, Nothing)
		End Function

		''' <summary>
		''' Brings up an internal dialog panel with a specified icon, where
		''' the initial choice is determined by the <code>initialValue</code>
		''' parameter and the number of choices is determined by the
		''' <code>optionType</code> parameter.
		''' <p>
		''' If <code>optionType</code> is <code>YES_NO_OPTION</code>, or
		''' <code>YES_NO_CANCEL_OPTION</code>
		''' and the <code>options</code> parameter is <code>null</code>,
		''' then the options are supplied by the Look and Feel.
		''' <p>
		''' The <code>messageType</code> parameter is primarily used to supply
		''' a default icon from the look and feel.
		''' </summary>
		''' <param name="parentComponent"> determines the <code>Frame</code>
		'''          in which the dialog is displayed; if <code>null</code>,
		'''          or if the <code>parentComponent</code> has no
		'''          <code>Frame</code>, a default <code>Frame</code> is used </param>
		''' <param name="message">   the object to display in the dialog; a
		'''          <code>Component</code> object is rendered as a
		'''          <code>Component</code>; a <code>String</code>
		'''          object is rendered as a string. Other objects are
		'''          converted to a <code>String</code> using the
		'''          <code>toString</code> method </param>
		''' <param name="title">     the title string for the dialog </param>
		''' <param name="optionType"> an integer designating the options available
		'''          on the dialog: <code>YES_NO_OPTION</code>,
		'''          or <code>YES_NO_CANCEL_OPTION</code> </param>
		''' <param name="messageType"> an integer designating the kind of message this is;
		'''          primarily used to determine the icon from the
		'''          pluggable Look and Feel: <code>ERROR_MESSAGE</code>,
		'''          <code>INFORMATION_MESSAGE</code>,
		'''          <code>WARNING_MESSAGE</code>, <code>QUESTION_MESSAGE</code>,
		'''          or <code>PLAIN_MESSAGE</code> </param>
		''' <param name="icon">      the icon to display in the dialog </param>
		''' <param name="options">   an array of objects indicating the possible choices
		'''          the user can make; if the objects are components, they
		'''          are rendered properly; non-<code>String</code>
		'''          objects are rendered using their <code>toString</code>
		'''          methods; if this parameter is <code>null</code>,
		'''          the options are determined by the Look and Feel </param>
		''' <param name="initialValue"> the object that represents the default selection
		'''          for the dialog; only meaningful if <code>options</code>
		'''          is used; can be <code>null</code> </param>
		''' <returns> an integer indicating the option chosen by the user,
		'''          or <code>CLOSED_OPTION</code> if the user closed the Dialog </returns>
		Public Shared Function showInternalOptionDialog(ByVal parentComponent As java.awt.Component, ByVal message As Object, ByVal title As String, ByVal optionType As Integer, ByVal messageType As Integer, ByVal icon As Icon, ByVal options As Object(), ByVal initialValue As Object) As Integer
			Dim pane As New JOptionPane(message, messageType, optionType, icon, options, initialValue)
			pane.putClientProperty(PopupFactory_FORCE_HEAVYWEIGHT_POPUP, Boolean.TRUE)
			Dim fo As java.awt.Component = java.awt.KeyboardFocusManager.currentKeyboardFocusManager.focusOwner

			pane.initialValue = initialValue

			Dim dialog As JInternalFrame = pane.createInternalFrame(parentComponent, title)
			pane.selectInitialValue()
			dialog.visible = True

	'         Since all input will be blocked until this dialog is dismissed,
	'         * make sure its parent containers are visible first (this component
	'         * is tested below).  This is necessary for JApplets, because
	'         * because an applet normally isn't made visible until after its
	'         * start() method returns -- if this method is called from start(),
	'         * the applet will appear to hang while an invisible modal frame
	'         * waits for input.
	'         
			If dialog.visible AndAlso (Not dialog.showing) Then
				Dim parent As java.awt.Container = dialog.parent
				Do While parent IsNot Nothing
					If parent.visible = False Then parent.visible = True
					parent = parent.parent
				Loop
			End If

			' Use reflection to get Container.startLWModal.
			Try
				Dim method As Method = java.security.AccessController.doPrivileged(New ModalPrivilegedAction(GetType(java.awt.Container), "startLWModal"))
				If method IsNot Nothing Then method.invoke(dialog, CType(Nothing, Object()))
			Catch ex As IllegalAccessException
			Catch ex As System.ArgumentException
			Catch ex As InvocationTargetException
			End Try

			If TypeOf parentComponent Is JInternalFrame Then
				Try
					CType(parentComponent, JInternalFrame).selected = True
				Catch e As java.beans.PropertyVetoException
				End Try
			End If

			Dim selectedValue As Object = pane.value

			If fo IsNot Nothing AndAlso fo.showing Then fo.requestFocus()
			If selectedValue Is Nothing Then Return CLOSED_OPTION
			If options Is Nothing Then
				If TypeOf selectedValue Is Integer? Then Return CInt(Fix(selectedValue))
				Return CLOSED_OPTION
			End If
			Dim counter As Integer = 0
			Dim maxCounter As Integer = options.Length
			Do While counter < maxCounter
				If options(counter).Equals(selectedValue) Then Return counter
				counter += 1
			Loop
			Return CLOSED_OPTION
		End Function

		''' <summary>
		''' Shows an internal question-message dialog requesting input from
		''' the user parented to <code>parentComponent</code>. The dialog
		''' is displayed in the <code>Component</code>'s frame,
		''' and is usually positioned below the <code>Component</code>.
		''' </summary>
		''' <param name="parentComponent">  the parent <code>Component</code>
		'''          for the dialog </param>
		''' <param name="message">  the <code>Object</code> to display </param>
		Public Shared Function showInternalInputDialog(ByVal parentComponent As java.awt.Component, ByVal message As Object) As String
			Return showInternalInputDialog(parentComponent, message, UIManager.getString("OptionPane.inputDialogTitle", parentComponent), QUESTION_MESSAGE)
		End Function

		''' <summary>
		''' Shows an internal dialog requesting input from the user parented
		''' to <code>parentComponent</code> with the dialog having the title
		''' <code>title</code> and message type <code>messageType</code>.
		''' </summary>
		''' <param name="parentComponent"> the parent <code>Component</code> for the dialog </param>
		''' <param name="message">  the <code>Object</code> to display </param>
		''' <param name="title">    the <code>String</code> to display in the
		'''          dialog title bar </param>
		''' <param name="messageType"> the type of message that is to be displayed:
		'''                    ERROR_MESSAGE, INFORMATION_MESSAGE, WARNING_MESSAGE,
		'''                    QUESTION_MESSAGE, or PLAIN_MESSAGE </param>
		Public Shared Function showInternalInputDialog(ByVal parentComponent As java.awt.Component, ByVal message As Object, ByVal title As String, ByVal messageType As Integer) As String
			Return CStr(showInternalInputDialog(parentComponent, message, title, messageType, Nothing, Nothing, Nothing))
		End Function

		''' <summary>
		''' Prompts the user for input in a blocking internal dialog where
		''' the initial selection, possible selections, and all other
		''' options can be specified. The user will able to choose from
		''' <code>selectionValues</code>, where <code>null</code>
		''' implies the user can input
		''' whatever they wish, usually by means of a <code>JTextField</code>.
		''' <code>initialSelectionValue</code> is the initial value to prompt
		''' the user with. It is up to the UI to decide how best to represent
		''' the <code>selectionValues</code>, but usually a
		''' <code>JComboBox</code>, <code>JList</code>, or
		''' <code>JTextField</code> will be used.
		''' </summary>
		''' <param name="parentComponent"> the parent <code>Component</code> for the dialog </param>
		''' <param name="message">  the <code>Object</code> to display </param>
		''' <param name="title">    the <code>String</code> to display in the dialog
		'''          title bar </param>
		''' <param name="messageType"> the type of message to be displayed:
		'''                  <code>ERROR_MESSAGE</code>, <code>INFORMATION_MESSAGE</code>,
		'''                  <code>WARNING_MESSAGE</code>,
		'''                  <code>QUESTION_MESSAGE</code>, or <code>PLAIN_MESSAGE</code> </param>
		''' <param name="icon">     the <code>Icon</code> image to display </param>
		''' <param name="selectionValues"> an array of <code>Objects</code> that
		'''                  gives the possible selections </param>
		''' <param name="initialSelectionValue"> the value used to initialize the input
		'''                  field </param>
		''' <returns> user's input, or <code>null</code> meaning the user
		'''          canceled the input </returns>
		Public Shared Function showInternalInputDialog(ByVal parentComponent As java.awt.Component, ByVal message As Object, ByVal title As String, ByVal messageType As Integer, ByVal icon As Icon, ByVal selectionValues As Object(), ByVal initialSelectionValue As Object) As Object
			Dim pane As New JOptionPane(message, messageType, OK_CANCEL_OPTION, icon, Nothing, Nothing)
			pane.putClientProperty(PopupFactory_FORCE_HEAVYWEIGHT_POPUP, Boolean.TRUE)
			Dim fo As java.awt.Component = java.awt.KeyboardFocusManager.currentKeyboardFocusManager.focusOwner

			pane.wantsInput = True
			pane.selectionValues = selectionValues
			pane.initialSelectionValue = initialSelectionValue

			Dim dialog As JInternalFrame = pane.createInternalFrame(parentComponent, title)

			pane.selectInitialValue()
			dialog.visible = True

	'         Since all input will be blocked until this dialog is dismissed,
	'         * make sure its parent containers are visible first (this component
	'         * is tested below).  This is necessary for JApplets, because
	'         * because an applet normally isn't made visible until after its
	'         * start() method returns -- if this method is called from start(),
	'         * the applet will appear to hang while an invisible modal frame
	'         * waits for input.
	'         
			If dialog.visible AndAlso (Not dialog.showing) Then
				Dim parent As java.awt.Container = dialog.parent
				Do While parent IsNot Nothing
					If parent.visible = False Then parent.visible = True
					parent = parent.parent
				Loop
			End If

			' Use reflection to get Container.startLWModal.
			Try
				Dim method As Method = java.security.AccessController.doPrivileged(New ModalPrivilegedAction(GetType(java.awt.Container), "startLWModal"))
				If method IsNot Nothing Then method.invoke(dialog, CType(Nothing, Object()))
			Catch ex As IllegalAccessException
			Catch ex As System.ArgumentException
			Catch ex As InvocationTargetException
			End Try

			If TypeOf parentComponent Is JInternalFrame Then
				Try
					CType(parentComponent, JInternalFrame).selected = True
				Catch e As java.beans.PropertyVetoException
				End Try
			End If

			If fo IsNot Nothing AndAlso fo.showing Then fo.requestFocus()
			Dim ___value As Object = pane.inputValue

			If ___value Is UNINITIALIZED_VALUE Then Return Nothing
			Return ___value
		End Function

		''' <summary>
		''' Creates and returns an instance of <code>JInternalFrame</code>.
		''' The internal frame is created with the specified title,
		''' and wrapping the <code>JOptionPane</code>.
		''' The returned <code>JInternalFrame</code> is
		''' added to the <code>JDesktopPane</code> ancestor of
		''' <code>parentComponent</code>, or components
		''' parent if one its ancestors isn't a <code>JDesktopPane</code>,
		''' or if <code>parentComponent</code>
		''' doesn't have a parent then a <code>RuntimeException</code> is thrown.
		''' </summary>
		''' <param name="parentComponent">  the parent <code>Component</code> for
		'''          the internal frame </param>
		''' <param name="title">    the <code>String</code> to display in the
		'''          frame's title bar </param>
		''' <returns> a <code>JInternalFrame</code> containing a
		'''          <code>JOptionPane</code> </returns>
		''' <exception cref="RuntimeException"> if <code>parentComponent</code> does
		'''          not have a valid parent </exception>
		Public Overridable Function createInternalFrame(ByVal parentComponent As java.awt.Component, ByVal title As String) As JInternalFrame
			Dim parent As java.awt.Container = JOptionPane.getDesktopPaneForComponent(parentComponent)

			parent = parentComponent.parent
			If parent Is Nothing AndAlso (parentComponent Is Nothing OrElse parent Is Nothing) Then Throw New Exception("JOptionPane: parentComponent does " & "not have a valid parent")

			' Option dialogs should be closable only
			Dim iFrame As New JInternalFrame(title, False, True, False, False)

			iFrame.putClientProperty("JInternalFrame.frameType", "optionDialog")
			iFrame.putClientProperty("JInternalFrame.messageType", Convert.ToInt32(messageType))

			iFrame.addInternalFrameListener(New InternalFrameAdapterAnonymousInnerClassHelper
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			addPropertyChangeListener(New java.beans.PropertyChangeListener()
	'		{
	'			public void propertyChange(PropertyChangeEvent event)
	'			{
	'				' Let the defaultCloseOperation handle the closing
	'				' if the user closed the iframe without selecting a button
	'				' (newValue = null in that case).  Otherwise, close the dialog.
	'				if (iFrame.isVisible() && event.getSource() == JOptionPane.this && event.getPropertyName().equals(VALUE_PROPERTY))
	'				{
	'				' Use reflection to get Container.stopLWModal().
	'				try
	'				{
	'					Method method = AccessController.doPrivileged(New ModalPrivilegedAction(Container.class, "stopLWModal"));
	'					if (method != Nothing)
	'					{
	'						method.invoke(iFrame, (Object[])Nothing);
	'					}
	'				}
	'				catch (IllegalAccessException ex)
	'				{
	'				}
	'				catch (IllegalArgumentException ex)
	'				{
	'				}
	'				catch (InvocationTargetException ex)
	'				{
	'				}
	'
	'				try
	'				{
	'					iFrame.setClosed(True);
	'				}
	'				catch (java.beans.PropertyVetoException e)
	'				{
	'				}
	'
	'				iFrame.setVisible(False);
	'				}
	'			}
	'		});
			iFrame.contentPane.add(Me, java.awt.BorderLayout.CENTER)
			If TypeOf parent Is JDesktopPane Then
				parent.add(iFrame, JLayeredPane.MODAL_LAYER)
			Else
				parent.add(iFrame, java.awt.BorderLayout.CENTER)
			End If
			Dim iFrameSize As java.awt.Dimension = iFrame.preferredSize
			Dim rootSize As java.awt.Dimension = parent.size
			Dim parentSize As java.awt.Dimension = parentComponent.size

			iFrame.boundsnds((rootSize.width - iFrameSize.width) / 2, (rootSize.height - iFrameSize.height) / 2, iFrameSize.width, iFrameSize.height)
			' We want dialog centered relative to its parent component
			Dim iFrameCoord As java.awt.Point = SwingUtilities.convertPoint(parentComponent, 0, 0, parent)
			Dim ___x As Integer = (parentSize.width - iFrameSize.width) / 2 + iFrameCoord.x
			Dim ___y As Integer = (parentSize.height - iFrameSize.height) / 2 + iFrameCoord.y

			' If possible, dialog should be fully visible
			Dim ovrx As Integer = ___x + iFrameSize.width - rootSize.width
			Dim ovry As Integer = ___y + iFrameSize.height - rootSize.height
			___x = Math.Max((If(ovrx > 0, ___x - ovrx, ___x)), 0)
			___y = Math.Max((If(ovry > 0, ___y - ovry, ___y)), 0)
			iFrame.boundsnds(___x, ___y, iFrameSize.width, iFrameSize.height)

			parent.validate()
			Try
				iFrame.selected = True
			Catch e As java.beans.PropertyVetoException
			End Try

			Return iFrame
		End Function

		Private Class InternalFrameAdapterAnonymousInnerClassHelper
			Inherits javax.swing.event.InternalFrameAdapter

			Public Overridable Sub internalFrameClosing(ByVal e As javax.swing.event.InternalFrameEvent)
				If outerInstance.value Is UNINITIALIZED_VALUE Then outerInstance.value = Nothing
			End Sub
		End Class

		''' <summary>
		''' Returns the specified component's <code>Frame</code>.
		''' </summary>
		''' <param name="parentComponent"> the <code>Component</code> to check for a
		'''          <code>Frame</code> </param>
		''' <returns> the <code>Frame</code> that contains the component,
		'''          or <code>getRootFrame</code>
		'''          if the component is <code>null</code>,
		'''          or does not have a valid <code>Frame</code> parent </returns>
		''' <exception cref="HeadlessException"> if
		'''   <code>GraphicsEnvironment.isHeadless</code> returns
		'''   <code>true</code> </exception>
		''' <seealso cref= #getRootFrame </seealso>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		Public Shared Function getFrameForComponent(ByVal parentComponent As java.awt.Component) As java.awt.Frame
			If parentComponent Is Nothing Then Return rootFrame
			If TypeOf parentComponent Is java.awt.Frame Then Return CType(parentComponent, java.awt.Frame)
			Return JOptionPane.getFrameForComponent(parentComponent.parent)
		End Function

		''' <summary>
		''' Returns the specified component's toplevel <code>Frame</code> or
		''' <code>Dialog</code>.
		''' </summary>
		''' <param name="parentComponent"> the <code>Component</code> to check for a
		'''          <code>Frame</code> or <code>Dialog</code> </param>
		''' <returns> the <code>Frame</code> or <code>Dialog</code> that
		'''          contains the component, or the default
		'''          frame if the component is <code>null</code>,
		'''          or does not have a valid
		'''          <code>Frame</code> or <code>Dialog</code> parent </returns>
		''' <exception cref="HeadlessException"> if
		'''   <code>GraphicsEnvironment.isHeadless</code> returns
		'''   <code>true</code> </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		Friend Shared Function getWindowForComponent(ByVal parentComponent As java.awt.Component) As java.awt.Window
			If parentComponent Is Nothing Then Return rootFrame
			If TypeOf parentComponent Is java.awt.Frame OrElse TypeOf parentComponent Is java.awt.Dialog Then Return CType(parentComponent, java.awt.Window)
			Return JOptionPane.getWindowForComponent(parentComponent.parent)
		End Function


		''' <summary>
		''' Returns the specified component's desktop pane.
		''' </summary>
		''' <param name="parentComponent"> the <code>Component</code> to check for a
		'''          desktop </param>
		''' <returns> the <code>JDesktopPane</code> that contains the component,
		'''          or <code>null</code> if the component is <code>null</code>
		'''          or does not have an ancestor that is a
		'''          <code>JInternalFrame</code> </returns>
		Public Shared Function getDesktopPaneForComponent(ByVal parentComponent As java.awt.Component) As JDesktopPane
			If parentComponent Is Nothing Then Return Nothing
			If TypeOf parentComponent Is JDesktopPane Then Return CType(parentComponent, JDesktopPane)
			Return getDesktopPaneForComponent(parentComponent.parent)
		End Function

		Private Shared ReadOnly sharedFrameKey As Object = GetType(JOptionPane)

		''' <summary>
		''' Sets the frame to use for class methods in which a frame is
		''' not provided.
		''' <p>
		''' <strong>Note:</strong>
		''' It is recommended that rather than using this method you supply a valid parent.
		''' </summary>
		''' <param name="newRootFrame"> the default <code>Frame</code> to use </param>
		Public Shared Property rootFrame As java.awt.Frame
			Set(ByVal newRootFrame As java.awt.Frame)
				If newRootFrame IsNot Nothing Then
					SwingUtilities.appContextPut(sharedFrameKey, newRootFrame)
				Else
					SwingUtilities.appContextRemove(sharedFrameKey)
				End If
			End Set
			Get
				Dim sharedFrame As java.awt.Frame = CType(SwingUtilities.appContextGet(sharedFrameKey), java.awt.Frame)
				If sharedFrame Is Nothing Then
					sharedFrame = SwingUtilities.sharedOwnerFrame
					SwingUtilities.appContextPut(sharedFrameKey, sharedFrame)
				End If
				Return sharedFrame
			End Get
		End Property


		''' <summary>
		''' Creates a <code>JOptionPane</code> with a test message.
		''' </summary>
		Public Sub New()
			Me.New("JOptionPane message")
		End Sub

		''' <summary>
		''' Creates a instance of <code>JOptionPane</code> to display a
		''' message using the
		''' plain-message message type and the default options delivered by
		''' the UI.
		''' </summary>
		''' <param name="message"> the <code>Object</code> to display </param>
		Public Sub New(ByVal message As Object)
			Me.New(message, PLAIN_MESSAGE)
		End Sub

		''' <summary>
		''' Creates an instance of <code>JOptionPane</code> to display a message
		''' with the specified message type and the default options,
		''' </summary>
		''' <param name="message"> the <code>Object</code> to display </param>
		''' <param name="messageType"> the type of message to be displayed:
		'''                  <code>ERROR_MESSAGE</code>,
		'''                  <code>INFORMATION_MESSAGE</code>,
		'''                  <code>WARNING_MESSAGE</code>,
		'''                  <code>QUESTION_MESSAGE</code>,
		'''                  or <code>PLAIN_MESSAGE</code> </param>
		Public Sub New(ByVal message As Object, ByVal messageType As Integer)
			Me.New(message, messageType, DEFAULT_OPTION)
		End Sub

		''' <summary>
		''' Creates an instance of <code>JOptionPane</code> to display a message
		''' with the specified message type and options.
		''' </summary>
		''' <param name="message"> the <code>Object</code> to display </param>
		''' <param name="messageType"> the type of message to be displayed:
		'''                  <code>ERROR_MESSAGE</code>,
		'''                  <code>INFORMATION_MESSAGE</code>,
		'''                  <code>WARNING_MESSAGE</code>,
		'''                  <code>QUESTION_MESSAGE</code>,
		'''                  or <code>PLAIN_MESSAGE</code> </param>
		''' <param name="optionType"> the options to display in the pane:
		'''                  <code>DEFAULT_OPTION</code>, <code>YES_NO_OPTION</code>,
		'''                  <code>YES_NO_CANCEL_OPTION</code>,
		'''                  <code>OK_CANCEL_OPTION</code> </param>
		Public Sub New(ByVal message As Object, ByVal messageType As Integer, ByVal optionType As Integer)
			Me.New(message, messageType, optionType, Nothing)
		End Sub

		''' <summary>
		''' Creates an instance of <code>JOptionPane</code> to display a message
		''' with the specified message type, options, and icon.
		''' </summary>
		''' <param name="message"> the <code>Object</code> to display </param>
		''' <param name="messageType"> the type of message to be displayed:
		'''                  <code>ERROR_MESSAGE</code>,
		'''                  <code>INFORMATION_MESSAGE</code>,
		'''                  <code>WARNING_MESSAGE</code>,
		'''                  <code>QUESTION_MESSAGE</code>,
		'''                  or <code>PLAIN_MESSAGE</code> </param>
		''' <param name="optionType"> the options to display in the pane:
		'''                  <code>DEFAULT_OPTION</code>, <code>YES_NO_OPTION</code>,
		'''                  <code>YES_NO_CANCEL_OPTION</code>,
		'''                  <code>OK_CANCEL_OPTION</code> </param>
		''' <param name="icon"> the <code>Icon</code> image to display </param>
		Public Sub New(ByVal message As Object, ByVal messageType As Integer, ByVal optionType As Integer, ByVal icon As Icon)
			Me.New(message, messageType, optionType, icon, Nothing)
		End Sub

		''' <summary>
		''' Creates an instance of <code>JOptionPane</code> to display a message
		''' with the specified message type, icon, and options.
		''' None of the options is initially selected.
		''' <p>
		''' The options objects should contain either instances of
		''' <code>Component</code>s, (which are added directly) or
		''' <code>Strings</code> (which are wrapped in a <code>JButton</code>).
		''' If you provide <code>Component</code>s, you must ensure that when the
		''' <code>Component</code> is clicked it messages <code>setValue</code>
		''' in the created <code>JOptionPane</code>.
		''' </summary>
		''' <param name="message"> the <code>Object</code> to display </param>
		''' <param name="messageType"> the type of message to be displayed:
		'''                  <code>ERROR_MESSAGE</code>,
		'''                  <code>INFORMATION_MESSAGE</code>,
		'''                  <code>WARNING_MESSAGE</code>,
		'''                  <code>QUESTION_MESSAGE</code>,
		'''                  or <code>PLAIN_MESSAGE</code> </param>
		''' <param name="optionType"> the options to display in the pane:
		'''                  <code>DEFAULT_OPTION</code>,
		'''                  <code>YES_NO_OPTION</code>,
		'''                  <code>YES_NO_CANCEL_OPTION</code>,
		'''                  <code>OK_CANCEL_OPTION</code> </param>
		''' <param name="icon"> the <code>Icon</code> image to display </param>
		''' <param name="options">  the choices the user can select </param>
		Public Sub New(ByVal message As Object, ByVal messageType As Integer, ByVal optionType As Integer, ByVal icon As Icon, ByVal options As Object())
			Me.New(message, messageType, optionType, icon, options, Nothing)
		End Sub

		''' <summary>
		''' Creates an instance of <code>JOptionPane</code> to display a message
		''' with the specified message type, icon, and options, with the
		''' initially-selected option specified.
		''' </summary>
		''' <param name="message"> the <code>Object</code> to display </param>
		''' <param name="messageType"> the type of message to be displayed:
		'''                  <code>ERROR_MESSAGE</code>,
		'''                  <code>INFORMATION_MESSAGE</code>,
		'''                  <code>WARNING_MESSAGE</code>,
		'''                  <code>QUESTION_MESSAGE</code>,
		'''                  or <code>PLAIN_MESSAGE</code> </param>
		''' <param name="optionType"> the options to display in the pane:
		'''                  <code>DEFAULT_OPTION</code>,
		'''                  <code>YES_NO_OPTION</code>,
		'''                  <code>YES_NO_CANCEL_OPTION</code>,
		'''                  <code>OK_CANCEL_OPTION</code> </param>
		''' <param name="icon"> the Icon image to display </param>
		''' <param name="options">  the choices the user can select </param>
		''' <param name="initialValue"> the choice that is initially selected; if
		'''                  <code>null</code>, then nothing will be initially selected;
		'''                  only meaningful if <code>options</code> is used </param>
		Public Sub New(ByVal message As Object, ByVal messageType As Integer, ByVal optionType As Integer, ByVal icon As Icon, ByVal options As Object(), ByVal initialValue As Object)

			Me.message = message
			Me.options = options
			Me.initialValue = initialValue
			Me.icon = icon
			messageType = messageType
			optionType = optionType
			value = UNINITIALIZED_VALUE
			inputValue = UNINITIALIZED_VALUE
			updateUI()
		End Sub

		''' <summary>
		''' Sets the UI object which implements the {@literal L&F} for this component.
		''' </summary>
		''' <param name="ui">  the <code>OptionPaneUI</code> {@literal L&F} object </param>
		''' <seealso cref= UIDefaults#getUI
		''' @beaninfo
		'''       bound: true
		'''      hidden: true
		''' description: The UI object that implements the optionpane's LookAndFeel </seealso>
		Public Overridable Property uI As javax.swing.plaf.OptionPaneUI
			Set(ByVal ui As javax.swing.plaf.OptionPaneUI)
				If Me.ui IsNot ui Then
					MyBase.uI = ui
					invalidate()
				End If
			End Set
			Get
				Return CType(ui, javax.swing.plaf.OptionPaneUI)
			End Get
		End Property


		''' <summary>
		''' Notification from the <code>UIManager</code> that the {@literal L&F} has changed.
		''' Replaces the current UI object with the latest version from the
		''' <code>UIManager</code>.
		''' </summary>
		''' <seealso cref= JComponent#updateUI </seealso>
		Public Overrides Sub updateUI()
			uI = CType(UIManager.getUI(Me), javax.swing.plaf.OptionPaneUI)
		End Sub


		''' <summary>
		''' Returns the name of the UI class that implements the
		''' {@literal L&F} for this component.
		''' </summary>
		''' <returns> the string "OptionPaneUI" </returns>
		''' <seealso cref= JComponent#getUIClassID </seealso>
		''' <seealso cref= UIDefaults#getUI </seealso>
		Public Property Overrides uIClassID As String
			Get
				Return uiClassID
			End Get
		End Property


		''' <summary>
		''' Sets the option pane's message-object. </summary>
		''' <param name="newMessage"> the <code>Object</code> to display </param>
		''' <seealso cref= #getMessage
		''' 
		''' @beaninfo
		'''   preferred: true
		'''   bound: true
		''' description: The optionpane's message object. </seealso>
		Public Overridable Property message As Object
			Set(ByVal newMessage As Object)
				Dim oldMessage As Object = message
    
				message = newMessage
				firePropertyChange(MESSAGE_PROPERTY, oldMessage, message)
			End Set
			Get
				Return message
			End Get
		End Property


		''' <summary>
		''' Sets the icon to display. If non-<code>null</code>, the look and feel
		''' does not provide an icon. </summary>
		''' <param name="newIcon"> the <code>Icon</code> to display
		''' </param>
		''' <seealso cref= #getIcon
		''' @beaninfo
		'''   preferred: true
		'''       bound: true
		''' description: The option pane's type icon. </seealso>
		Public Overridable Property icon As Icon
			Set(ByVal newIcon As Icon)
				Dim oldIcon As Object = icon
    
				icon = newIcon
				firePropertyChange(ICON_PROPERTY, oldIcon, icon)
			End Set
			Get
				Return icon
			End Get
		End Property


		''' <summary>
		''' Sets the value the user has chosen. </summary>
		''' <param name="newValue">  the chosen value
		''' </param>
		''' <seealso cref= #getValue
		''' @beaninfo
		'''   preferred: true
		'''       bound: true
		''' description: The option pane's value object. </seealso>
		Public Overridable Property value As Object
			Set(ByVal newValue As Object)
				Dim oldValue As Object = value
    
				value = newValue
				firePropertyChange(VALUE_PROPERTY, oldValue, value)
			End Set
			Get
				Return value
			End Get
		End Property


		''' <summary>
		''' Sets the options this pane displays. If an element in
		''' <code>newOptions</code> is a <code>Component</code>
		''' it is added directly to the pane,
		''' otherwise a button is created for the element.
		''' </summary>
		''' <param name="newOptions"> an array of <code>Objects</code> that create the
		'''          buttons the user can click on, or arbitrary
		'''          <code>Components</code> to add to the pane
		''' </param>
		''' <seealso cref= #getOptions
		''' @beaninfo
		'''       bound: true
		''' description: The option pane's options objects. </seealso>
		Public Overridable Property options As Object()
			Set(ByVal newOptions As Object())
				Dim oldOptions As Object() = options
    
				options = newOptions
				firePropertyChange(OPTIONS_PROPERTY, oldOptions, options)
			End Set
			Get
				If options IsNot Nothing Then
					Dim optionCount As Integer = options.Length
					Dim retOptions As Object() = New Object(optionCount - 1){}
    
					Array.Copy(options, 0, retOptions, 0, optionCount)
					Return retOptions
				End If
				Return options
			End Get
		End Property


		''' <summary>
		''' Sets the initial value that is to be enabled -- the
		''' <code>Component</code>
		''' that has the focus when the pane is initially displayed.
		''' </summary>
		''' <param name="newInitialValue"> the <code>Object</code> that gets the initial
		'''                         keyboard focus
		''' </param>
		''' <seealso cref= #getInitialValue
		''' @beaninfo
		'''   preferred: true
		'''       bound: true
		''' description: The option pane's initial value object. </seealso>
		Public Overridable Property initialValue As Object
			Set(ByVal newInitialValue As Object)
				Dim oldIV As Object = initialValue
    
				initialValue = newInitialValue
				firePropertyChange(INITIAL_VALUE_PROPERTY, oldIV, initialValue)
			End Set
			Get
				Return initialValue
			End Get
		End Property


		''' <summary>
		''' Sets the option pane's message type.
		''' The message type is used by the Look and Feel to determine the
		''' icon to display (if not supplied) as well as potentially how to
		''' lay out the <code>parentComponent</code>. </summary>
		''' <param name="newType"> an integer specifying the kind of message to display:
		'''                <code>ERROR_MESSAGE</code>, <code>INFORMATION_MESSAGE</code>,
		'''                <code>WARNING_MESSAGE</code>,
		'''                <code>QUESTION_MESSAGE</code>, or <code>PLAIN_MESSAGE</code> </param>
		''' <exception cref="RuntimeException"> if <code>newType</code> is not one of the
		'''          legal values listed above
		''' </exception>
		''' <seealso cref= #getMessageType
		''' @beaninfo
		'''   preferred: true
		'''       bound: true
		''' description: The option pane's message type. </seealso>
		Public Overridable Property messageType As Integer
			Set(ByVal newType As Integer)
				If newType <> ERROR_MESSAGE AndAlso newType <> INFORMATION_MESSAGE AndAlso newType <> WARNING_MESSAGE AndAlso newType <> QUESTION_MESSAGE AndAlso newType <> PLAIN_MESSAGE Then Throw New Exception("JOptionPane: type must be one of JOptionPane.ERROR_MESSAGE, JOptionPane.INFORMATION_MESSAGE, JOptionPane.WARNING_MESSAGE, JOptionPane.QUESTION_MESSAGE or JOptionPane.PLAIN_MESSAGE")
    
				Dim oldType As Integer = messageType
    
				messageType = newType
				firePropertyChange(MESSAGE_TYPE_PROPERTY, oldType, messageType)
			End Set
			Get
				Return messageType
			End Get
		End Property


		''' <summary>
		''' Sets the options to display.
		''' The option type is used by the Look and Feel to
		''' determine what buttons to show (unless options are supplied). </summary>
		''' <param name="newType"> an integer specifying the options the {@literal L&F} is to display:
		'''                  <code>DEFAULT_OPTION</code>,
		'''                  <code>YES_NO_OPTION</code>,
		'''                  <code>YES_NO_CANCEL_OPTION</code>,
		'''                  or <code>OK_CANCEL_OPTION</code> </param>
		''' <exception cref="RuntimeException"> if <code>newType</code> is not one of
		'''          the legal values listed above
		''' </exception>
		''' <seealso cref= #getOptionType </seealso>
		''' <seealso cref= #setOptions
		''' @beaninfo
		'''   preferred: true
		'''       bound: true
		''' description: The option pane's option type. </seealso>
		Public Overridable Property optionType As Integer
			Set(ByVal newType As Integer)
				If newType <> DEFAULT_OPTION AndAlso newType <> YES_NO_OPTION AndAlso newType <> YES_NO_CANCEL_OPTION AndAlso newType <> OK_CANCEL_OPTION Then Throw New Exception("JOptionPane: option type must be one of JOptionPane.DEFAULT_OPTION, JOptionPane.YES_NO_OPTION, JOptionPane.YES_NO_CANCEL_OPTION or JOptionPane.OK_CANCEL_OPTION")
    
				Dim oldType As Integer = optionType
    
				optionType = newType
				firePropertyChange(OPTION_TYPE_PROPERTY, oldType, optionType)
			End Set
			Get
				Return optionType
			End Get
		End Property


		''' <summary>
		''' Sets the input selection values for a pane that provides the user
		''' with a list of items to choose from. (The UI provides a widget
		''' for choosing one of the values.)  A <code>null</code> value
		''' implies the user can input whatever they wish, usually by means
		''' of a <code>JTextField</code>.
		''' <p>
		''' Sets <code>wantsInput</code> to true. Use
		''' <code>setInitialSelectionValue</code> to specify the initially-chosen
		''' value. After the pane as been enabled, <code>inputValue</code> is
		''' set to the value the user has selected. </summary>
		''' <param name="newValues"> an array of <code>Objects</code> the user to be
		'''                  displayed
		'''                  (usually in a list or combo-box) from which
		'''                  the user can make a selection </param>
		''' <seealso cref= #setWantsInput </seealso>
		''' <seealso cref= #setInitialSelectionValue </seealso>
		''' <seealso cref= #getSelectionValues
		''' @beaninfo
		'''       bound: true
		''' description: The option pane's selection values. </seealso>
		Public Overridable Property selectionValues As Object()
			Set(ByVal newValues As Object())
				Dim oldValues As Object() = selectionValues
    
				selectionValues = newValues
				firePropertyChange(SELECTION_VALUES_PROPERTY, oldValues, newValues)
				If selectionValues IsNot Nothing Then wantsInput = True
			End Set
			Get
				Return selectionValues
			End Get
		End Property


		''' <summary>
		''' Sets the input value that is initially displayed as selected to the user.
		''' Only used if <code>wantsInput</code> is true. </summary>
		''' <param name="newValue"> the initially selected value </param>
		''' <seealso cref= #setSelectionValues </seealso>
		''' <seealso cref= #getInitialSelectionValue
		''' @beaninfo
		'''       bound: true
		''' description: The option pane's initial selection value object. </seealso>
		Public Overridable Property initialSelectionValue As Object
			Set(ByVal newValue As Object)
				Dim oldValue As Object = initialSelectionValue
    
				initialSelectionValue = newValue
				firePropertyChange(INITIAL_SELECTION_VALUE_PROPERTY, oldValue, newValue)
			End Set
			Get
				Return initialSelectionValue
			End Get
		End Property


		''' <summary>
		''' Sets the input value that was selected or input by the user.
		''' Only used if <code>wantsInput</code> is true.  Note that this method
		''' is invoked internally by the option pane (in response to user action)
		''' and should generally not be called by client programs.  To set the
		''' input value initially displayed as selected to the user, use
		''' <code>setInitialSelectionValue</code>.
		''' </summary>
		''' <param name="newValue"> the <code>Object</code> used to set the
		'''          value that the user specified (usually in a text field) </param>
		''' <seealso cref= #setSelectionValues </seealso>
		''' <seealso cref= #setInitialSelectionValue </seealso>
		''' <seealso cref= #setWantsInput </seealso>
		''' <seealso cref= #getInputValue
		''' @beaninfo
		'''   preferred: true
		'''       bound: true
		''' description: The option pane's input value object. </seealso>
		Public Overridable Property inputValue As Object
			Set(ByVal newValue As Object)
				Dim oldValue As Object = inputValue
    
				inputValue = newValue
				firePropertyChange(INPUT_VALUE_PROPERTY, oldValue, newValue)
			End Set
			Get
				Return inputValue
			End Get
		End Property


		''' <summary>
		''' Returns the maximum number of characters to place on a line in a
		''' message. Default is to return <code>Integer.MAX_VALUE</code>.
		''' The value can be
		''' changed by overriding this method in a subclass.
		''' </summary>
		''' <returns> an integer giving the maximum number of characters on a line </returns>
		Public Overridable Property maxCharactersPerLineCount As Integer
			Get
				Return Integer.MaxValue
			End Get
		End Property

		''' <summary>
		''' Sets the <code>wantsInput</code> property.
		''' If <code>newValue</code> is true, an input component
		''' (such as a text field or combo box) whose parent is
		''' <code>parentComponent</code> is provided to
		''' allow the user to input a value. If <code>getSelectionValues</code>
		''' returns a non-<code>null</code> array, the input value is one of the
		''' objects in that array. Otherwise the input value is whatever
		''' the user inputs.
		''' <p>
		''' This is a bound property.
		''' </summary>
		''' <seealso cref= #setSelectionValues </seealso>
		''' <seealso cref= #setInputValue
		''' @beaninfo
		'''   preferred: true
		'''       bound: true
		''' description: Flag which allows the user to input a value. </seealso>
		Public Overridable Property wantsInput As Boolean
			Set(ByVal newValue As Boolean)
				Dim oldValue As Boolean = wantsInput
    
				wantsInput = newValue
				firePropertyChange(WANTS_INPUT_PROPERTY, oldValue, newValue)
			End Set
			Get
				Return wantsInput
			End Get
		End Property


		''' <summary>
		''' Requests that the initial value be selected, which will set
		''' focus to the initial value. This method
		''' should be invoked after the window containing the option pane
		''' is made visible.
		''' </summary>
		Public Overridable Sub selectInitialValue()
			Dim ___ui As javax.swing.plaf.OptionPaneUI = uI
			If ___ui IsNot Nothing Then ___ui.selectInitialValue(Me)
		End Sub


		Private Shared Function styleFromMessageType(ByVal messageType As Integer) As Integer
			Select Case messageType
			Case ERROR_MESSAGE
				Return JRootPane.ERROR_DIALOG
			Case QUESTION_MESSAGE
				Return JRootPane.QUESTION_DIALOG
			Case WARNING_MESSAGE
				Return JRootPane.WARNING_DIALOG
			Case INFORMATION_MESSAGE
				Return JRootPane.INFORMATION_DIALOG
			Case Else
				Return JRootPane.PLAIN_DIALOG
			End Select
		End Function

		' Serialization support.
		Private Sub writeObject(ByVal s As java.io.ObjectOutputStream)
			Dim values As New List(Of Object)

			s.defaultWriteObject()
			' Save the icon, if its Serializable.
			If icon IsNot Nothing AndAlso TypeOf icon Is java.io.Serializable Then
				values.Add("icon")
				values.Add(icon)
			End If
			' Save the message, if its Serializable.
			If message IsNot Nothing AndAlso TypeOf message Is java.io.Serializable Then
				values.Add("message")
				values.Add(message)
			End If
			' Save the treeModel, if its Serializable.
			If options IsNot Nothing Then
				Dim serOptions As New List(Of Object)

				Dim counter As Integer = 0
				Dim maxCounter As Integer = options.Length
				Do While counter < maxCounter
					If TypeOf options(counter) Is java.io.Serializable Then serOptions.Add(options(counter))
					counter += 1
				Loop
				If serOptions.Count > 0 Then
					Dim optionCount As Integer = serOptions.Count
					Dim arrayOptions As Object() = New Object(optionCount - 1){}

					serOptions.CopyTo(arrayOptions)
					values.Add("options")
					values.Add(arrayOptions)
				End If
			End If
			' Save the initialValue, if its Serializable.
			If initialValue IsNot Nothing AndAlso TypeOf initialValue Is java.io.Serializable Then
				values.Add("initialValue")
				values.Add(initialValue)
			End If
			' Save the value, if its Serializable.
			If value IsNot Nothing AndAlso TypeOf value Is java.io.Serializable Then
				values.Add("value")
				values.Add(value)
			End If
			' Save the selectionValues, if its Serializable.
			If selectionValues IsNot Nothing Then
				Dim serialize As Boolean = True

				Dim counter As Integer = 0
				Dim maxCounter As Integer = selectionValues.Length
				Do While counter < maxCounter
					If selectionValues(counter) IsNot Nothing AndAlso Not(TypeOf selectionValues(counter) Is java.io.Serializable) Then
						serialize = False
						Exit Do
					End If
					counter += 1
				Loop
				If serialize Then
					values.Add("selectionValues")
					values.Add(selectionValues)
				End If
			End If
			' Save the inputValue, if its Serializable.
			If inputValue IsNot Nothing AndAlso TypeOf inputValue Is java.io.Serializable Then
				values.Add("inputValue")
				values.Add(inputValue)
			End If
			' Save the initialSelectionValue, if its Serializable.
			If initialSelectionValue IsNot Nothing AndAlso TypeOf initialSelectionValue Is java.io.Serializable Then
				values.Add("initialSelectionValue")
				values.Add(initialSelectionValue)
			End If
			s.writeObject(values)
		End Sub

		Private Sub readObject(ByVal s As java.io.ObjectInputStream)
			s.defaultReadObject()

			Dim values As ArrayList = CType(s.readObject(), ArrayList)
			Dim indexCounter As Integer = 0
			Dim maxCounter As Integer = values.Count

			If indexCounter < maxCounter AndAlso values(indexCounter).Equals("icon") Then
				indexCounter += 1
				icon = CType(values(indexCounter), Icon)
				indexCounter += 1
			End If
			If indexCounter < maxCounter AndAlso values(indexCounter).Equals("message") Then
				indexCounter += 1
				message = values(indexCounter)
				indexCounter += 1
			End If
			If indexCounter < maxCounter AndAlso values(indexCounter).Equals("options") Then
				indexCounter += 1
				options = CType(values(indexCounter), Object())
				indexCounter += 1
			End If
			If indexCounter < maxCounter AndAlso values(indexCounter).Equals("initialValue") Then
				indexCounter += 1
				initialValue = values(indexCounter)
				indexCounter += 1
			End If
			If indexCounter < maxCounter AndAlso values(indexCounter).Equals("value") Then
				indexCounter += 1
				value = values(indexCounter)
				indexCounter += 1
			End If
			If indexCounter < maxCounter AndAlso values(indexCounter).Equals("selectionValues") Then
				indexCounter += 1
				selectionValues = CType(values(indexCounter), Object())
				indexCounter += 1
			End If
			If indexCounter < maxCounter AndAlso values(indexCounter).Equals("inputValue") Then
				indexCounter += 1
				inputValue = values(indexCounter)
				indexCounter += 1
			End If
			If indexCounter < maxCounter AndAlso values(indexCounter).Equals("initialSelectionValue") Then
				indexCounter += 1
				initialSelectionValue = values(indexCounter)
				indexCounter += 1
			End If
			If uIClassID.Equals(uiClassID) Then
				Dim count As SByte = JComponent.getWriteObjCounter(Me)
				count -= 1
				JComponent.writeObjCounterter(Me, count)
				If count = 0 AndAlso ui IsNot Nothing Then ui.installUI(Me)
			End If
		End Sub


		''' <summary>
		''' Returns a string representation of this <code>JOptionPane</code>.
		''' This method
		''' is intended to be used only for debugging purposes, and the
		''' content and format of the returned string may vary between
		''' implementations. The returned string may be empty but may not
		''' be <code>null</code>.
		''' </summary>
		''' <returns>  a string representation of this <code>JOptionPane</code> </returns>
		Protected Friend Overrides Function paramString() As String
			Dim iconString As String = (If(icon IsNot Nothing, icon.ToString(), ""))
			Dim initialValueString As String = (If(initialValue IsNot Nothing, initialValue.ToString(), ""))
			Dim messageString As String = (If(message IsNot Nothing, message.ToString(), ""))
			Dim messageTypeString As String
			If messageType = ERROR_MESSAGE Then
				messageTypeString = "ERROR_MESSAGE"
			ElseIf messageType = INFORMATION_MESSAGE Then
				messageTypeString = "INFORMATION_MESSAGE"
			ElseIf messageType = WARNING_MESSAGE Then
				messageTypeString = "WARNING_MESSAGE"
			ElseIf messageType = QUESTION_MESSAGE Then
				messageTypeString = "QUESTION_MESSAGE"
			ElseIf messageType = PLAIN_MESSAGE Then
				messageTypeString = "PLAIN_MESSAGE"
			Else
				messageTypeString = ""
			End If
			Dim optionTypeString As String
			If optionType = DEFAULT_OPTION Then
				optionTypeString = "DEFAULT_OPTION"
			ElseIf optionType = YES_NO_OPTION Then
				optionTypeString = "YES_NO_OPTION"
			ElseIf optionType = YES_NO_CANCEL_OPTION Then
				optionTypeString = "YES_NO_CANCEL_OPTION"
			ElseIf optionType = OK_CANCEL_OPTION Then
				optionTypeString = "OK_CANCEL_OPTION"
			Else
				optionTypeString = ""
			End If
			Dim wantsInputString As String = (If(wantsInput, "true", "false"))

			Return MyBase.paramString() & ",icon=" & iconString & ",initialValue=" & initialValueString & ",message=" & messageString & ",messageType=" & messageTypeString & ",optionType=" & optionTypeString & ",wantsInput=" & wantsInputString
		End Function

		''' <summary>
		''' Retrieves a method from the provided class and makes it accessible.
		''' </summary>
		Private Class ModalPrivilegedAction
			Implements java.security.PrivilegedAction(Of Method)

			Private clazz As Type
			Private methodName As String

			Public Sub New(ByVal clazz As Type, ByVal methodName As String)
				Me.clazz = clazz
				Me.methodName = methodName
			End Sub

			Public Overridable Function run() As Method
				Dim method As Method = Nothing
				Try
					method = clazz.getDeclaredMethod(methodName, CType(Nothing, Type()))
				Catch ex As NoSuchMethodException
				End Try
				If method IsNot Nothing Then method.accessible = True
				Return method
			End Function
		End Class



	'/////////////////
	' Accessibility support
	'/////////////////

		''' <summary>
		''' Returns the <code>AccessibleContext</code> associated with this JOptionPane.
		''' For option panes, the <code>AccessibleContext</code> takes the form of an
		''' <code>AccessibleJOptionPane</code>.
		''' A new <code>AccessibleJOptionPane</code> instance is created if necessary.
		''' </summary>
		''' <returns> an AccessibleJOptionPane that serves as the
		'''         AccessibleContext of this AccessibleJOptionPane
		''' @beaninfo
		'''       expert: true
		'''  description: The AccessibleContext associated with this option pane </returns>
		Public Overridable Property accessibleContext As AccessibleContext Implements Accessible.getAccessibleContext
			Get
				If accessibleContext Is Nothing Then accessibleContext = New AccessibleJOptionPane(Me)
				Return accessibleContext
			End Get
		End Property

		''' <summary>
		''' This class implements accessibility support for the
		''' <code>JOptionPane</code> class.  It provides an implementation of the
		''' Java Accessibility API appropriate to option pane user-interface
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
		Protected Friend Class AccessibleJOptionPane
			Inherits AccessibleJComponent

			Private ReadOnly outerInstance As JOptionPane

			Public Sub New(ByVal outerInstance As JOptionPane)
				Me.outerInstance = outerInstance
			End Sub


			''' <summary>
			''' Get the role of this object.
			''' </summary>
			''' <returns> an instance of AccessibleRole describing the role of the object </returns>
			''' <seealso cref= AccessibleRole </seealso>
			Public Overridable Property accessibleRole As AccessibleRole
				Get
					Select Case outerInstance.messageType
					Case ERROR_MESSAGE, INFORMATION_MESSAGE, WARNING_MESSAGE
						Return AccessibleRole.ALERT
    
					Case Else
						Return AccessibleRole.OPTION_PANE
					End Select
				End Get
			End Property

		End Class ' inner class AccessibleJOptionPane
	End Class

End Namespace
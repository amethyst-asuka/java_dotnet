Imports Microsoft.VisualBasic
Imports System
Imports System.Windows.Forms
Imports javax.swing
Imports javax.swing.event

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

Namespace javax.swing.plaf.basic




	''' <summary>
	''' Provides the basic look and feel for a <code>JOptionPane</code>.
	''' <code>BasicMessagePaneUI</code> provides a means to place an icon,
	''' message and buttons into a <code>Container</code>.
	''' Generally, the layout will look like:
	''' <pre>
	'''        ------------------
	'''        | i | message    |
	'''        | c | message    |
	'''        | o | message    |
	'''        | n | message    |
	'''        ------------------
	'''        |     buttons    |
	'''        |________________|
	''' </pre>
	''' icon is an instance of <code>Icon</code> that is wrapped inside a
	''' <code>JLabel</code>.  The message is an opaque object and is tested
	''' for the following: if the message is a <code>Component</code> it is
	''' added to the <code>Container</code>, if it is an <code>Icon</code>
	''' it is wrapped inside a <code>JLabel</code> and added to the
	''' <code>Container</code> otherwise it is wrapped inside a <code>JLabel</code>.
	''' <p>
	''' The above layout is used when the option pane's
	''' <code>ComponentOrientation</code> property is horizontal, left-to-right.
	''' The layout will be adjusted appropriately for other orientations.
	''' <p>
	''' The <code>Container</code>, message, icon, and buttons are all
	''' determined from abstract methods.
	''' 
	''' @author James Gosling
	''' @author Scott Violet
	''' @author Amy Fowler
	''' </summary>
	Public Class BasicOptionPaneUI
		Inherits javax.swing.plaf.OptionPaneUI

		Public Const MinimumWidth As Integer = 262
		Public Const MinimumHeight As Integer = 90

		Private Shared newline As String

		''' <summary>
		''' <code>JOptionPane</code> that the receiver is providing the
		''' look and feel for.
		''' </summary>
		Protected Friend optionPane As JOptionPane

		Protected Friend minimumSize As Dimension

		''' <summary>
		''' JComponent provide for input if optionPane.getWantsInput() returns
		''' true. 
		''' </summary>
		Protected Friend inputComponent As JComponent

		''' <summary>
		''' Component to receive focus when messaged with selectInitialValue. </summary>
		Protected Friend initialFocusComponent As Component

		''' <summary>
		''' This is set to true in validateComponent if a Component is contained
		''' in either the message or the buttons. 
		''' </summary>
		Protected Friend hasCustomComponents As Boolean

		Protected Friend propertyChangeListener As java.beans.PropertyChangeListener

		Private handler As Handler


		Shared Sub New()
			newline = java.security.AccessController.doPrivileged(New sun.security.action.GetPropertyAction("line.separator"))
			If newline Is Nothing Then newline = vbLf
		End Sub

		Friend Shared Sub loadActionMap(ByVal map As LazyActionMap)
			map.put(New Actions(Actions.CLOSE))
			BasicLookAndFeel.installAudioActionMap(map)
		End Sub



		''' <summary>
		''' Creates a new BasicOptionPaneUI instance.
		''' </summary>
		Public Shared Function createUI(ByVal x As JComponent) As javax.swing.plaf.ComponentUI
			Return New BasicOptionPaneUI
		End Function

		''' <summary>
		''' Installs the receiver as the L&amp;F for the passed in
		''' <code>JOptionPane</code>.
		''' </summary>
		Public Overridable Sub installUI(ByVal c As JComponent)
			optionPane = CType(c, JOptionPane)
			installDefaults()
			optionPane.layout = createLayoutManager()
			installComponents()
			installListeners()
			installKeyboardActions()
		End Sub

		''' <summary>
		''' Removes the receiver from the L&amp;F controller of the passed in split
		''' pane.
		''' </summary>
		Public Overridable Sub uninstallUI(ByVal c As JComponent)
			uninstallComponents()
			optionPane.layout = Nothing
			uninstallKeyboardActions()
			uninstallListeners()
			uninstallDefaults()
			optionPane = Nothing
		End Sub

		Protected Friend Overridable Sub installDefaults()
			LookAndFeel.installColorsAndFont(optionPane, "OptionPane.background", "OptionPane.foreground", "OptionPane.font")
			LookAndFeel.installBorder(optionPane, "OptionPane.border")
			minimumSize = UIManager.getDimension("OptionPane.minimumSize")
			LookAndFeel.installProperty(optionPane, "opaque", Boolean.TRUE)
		End Sub

		Protected Friend Overridable Sub uninstallDefaults()
			LookAndFeel.uninstallBorder(optionPane)
		End Sub

		Protected Friend Overridable Sub installComponents()
			optionPane.add(createMessageArea())

			Dim separator As Container = createSeparator()
			If separator IsNot Nothing Then optionPane.add(separator)
			optionPane.add(createButtonArea())
			optionPane.applyComponentOrientation(optionPane.componentOrientation)
		End Sub

		Protected Friend Overridable Sub uninstallComponents()
			hasCustomComponents = False
			inputComponent = Nothing
			initialFocusComponent = Nothing
			optionPane.removeAll()
		End Sub

		Protected Friend Overridable Function createLayoutManager() As LayoutManager
			Return New BoxLayout(optionPane, BoxLayout.Y_AXIS)
		End Function

		Protected Friend Overridable Sub installListeners()
			propertyChangeListener = createPropertyChangeListener()
			If propertyChangeListener IsNot Nothing Then optionPane.addPropertyChangeListener(propertyChangeListener)
		End Sub

		Protected Friend Overridable Sub uninstallListeners()
			If propertyChangeListener IsNot Nothing Then
				optionPane.removePropertyChangeListener(propertyChangeListener)
				propertyChangeListener = Nothing
			End If
			handler = Nothing
		End Sub

		Protected Friend Overridable Function createPropertyChangeListener() As java.beans.PropertyChangeListener
			Return handler
		End Function

		Private Property handler As Handler
			Get
				If handler Is Nothing Then handler = New Handler(Me)
				Return handler
			End Get
		End Property

		Protected Friend Overridable Sub installKeyboardActions()
			Dim map As InputMap = getInputMap(JComponent.WHEN_IN_FOCUSED_WINDOW)

			SwingUtilities.replaceUIInputMap(optionPane, JComponent.WHEN_IN_FOCUSED_WINDOW, map)

			LazyActionMap.installLazyActionMap(optionPane, GetType(BasicOptionPaneUI), "OptionPane.actionMap")
		End Sub

		Protected Friend Overridable Sub uninstallKeyboardActions()
			SwingUtilities.replaceUIInputMap(optionPane, JComponent.WHEN_IN_FOCUSED_WINDOW, Nothing)
			SwingUtilities.replaceUIActionMap(optionPane, Nothing)
		End Sub

		Friend Overridable Function getInputMap(ByVal condition As Integer) As InputMap
			If condition = JComponent.WHEN_IN_FOCUSED_WINDOW Then
				Dim bindings As Object() = CType(sun.swing.DefaultLookup.get(optionPane, Me, "OptionPane.windowBindings"), Object())
				If bindings IsNot Nothing Then Return LookAndFeel.makeComponentInputMap(optionPane, bindings)
			End If
			Return Nothing
		End Function

		''' <summary>
		''' Returns the minimum size the option pane should be. Primarily
		''' provided for subclassers wishing to offer a different minimum size.
		''' </summary>
		Public Overridable Property minimumOptionPaneSize As Dimension
			Get
				If minimumSize Is Nothing Then Return New Dimension(MinimumWidth, MinimumHeight)
				Return New Dimension(minimumSize.width, minimumSize.height)
			End Get
		End Property

		''' <summary>
		''' If <code>c</code> is the <code>JOptionPane</code> the receiver
		''' is contained in, the preferred
		''' size that is returned is the maximum of the preferred size of
		''' the <code>LayoutManager</code> for the <code>JOptionPane</code>, and
		''' <code>getMinimumOptionPaneSize</code>.
		''' </summary>
		Public Overridable Function getPreferredSize(ByVal c As JComponent) As Dimension
			If c Is optionPane Then
				Dim ourMin As Dimension = minimumOptionPaneSize
				Dim lm As LayoutManager = c.layout

				If lm IsNot Nothing Then
					Dim lmSize As Dimension = lm.preferredLayoutSize(c)

					If ourMin IsNot Nothing Then Return New Dimension(Math.Max(lmSize.width, ourMin.width), Math.Max(lmSize.height, ourMin.height))
					Return lmSize
				End If
				Return ourMin
			End If
			Return Nothing
		End Function

		''' <summary>
		''' Messaged from installComponents to create a Container containing the
		''' body of the message. The icon is the created by calling
		''' <code>addIcon</code>.
		''' </summary>
		Protected Friend Overridable Function createMessageArea() As Container
			Dim top As New JPanel
			Dim topBorder As javax.swing.border.Border = CType(sun.swing.DefaultLookup.get(optionPane, Me, "OptionPane.messageAreaBorder"), javax.swing.border.Border)
			If topBorder IsNot Nothing Then top.border = topBorder
			top.layout = New BorderLayout

			' Fill the body. 
			Dim body As Container = New JPanel(New GridBagLayout)
			Dim realBody As Container = New JPanel(New BorderLayout)

			body.name = "OptionPane.body"
			realBody.name = "OptionPane.realBody"

			If icon IsNot Nothing Then
				Dim sep As New JPanel
				sep.name = "OptionPane.separator"
				sep.preferredSize = New Dimension(15, 1)
				realBody.add(sep, BorderLayout.BEFORE_LINE_BEGINS)
			End If
			realBody.add(body, BorderLayout.CENTER)

			Dim cons As New GridBagConstraints
				cons.gridy = 0
				cons.gridx = cons.gridy
			cons.gridwidth = GridBagConstraints.REMAINDER
			cons.gridheight = 1
			cons.anchor = sun.swing.DefaultLookup.getInt(optionPane, Me, "OptionPane.messageAnchor", GridBagConstraints.CENTER)
			cons.insets = New Insets(0,0,3,0)

			addMessageComponents(body, cons, message, maxCharactersPerLineCount, False)
			top.add(realBody, BorderLayout.CENTER)

			addIcon(top)
			Return top
		End Function

		''' <summary>
		''' Creates the appropriate object to represent <code>msg</code> and
		''' places it into <code>container</code>. If <code>msg</code> is an
		''' instance of Component, it is added directly, if it is an Icon,
		''' a JLabel is created to represent it, otherwise a JLabel is
		''' created for the string, if <code>d</code> is an Object[], this
		''' method will be recursively invoked for the children.
		''' <code>internallyCreated</code> is true if Objc is an instance
		''' of Component and was created internally by this method (this is
		''' used to correctly set hasCustomComponents only if !internallyCreated).
		''' </summary>
		Protected Friend Overridable Sub addMessageComponents(ByVal container As Container, ByVal cons As GridBagConstraints, ByVal msg As Object, ByVal maxll As Integer, ByVal internallyCreated As Boolean)
			If msg Is Nothing Then Return
			If TypeOf msg Is Component Then
				' To workaround problem where Gridbad will set child
				' to its minimum size if its preferred size will not fit
				' within allocated cells
				If TypeOf msg Is JScrollPane OrElse TypeOf msg Is JPanel Then
					cons.fill = GridBagConstraints.BOTH
					cons.weighty = 1
				Else
					cons.fill = GridBagConstraints.HORIZONTAL
				End If
				cons.weightx = 1

				container.add(CType(msg, Component), cons)
				cons.weightx = 0
				cons.weighty = 0
				cons.fill = GridBagConstraints.NONE
				cons.gridy += 1
				If Not internallyCreated Then hasCustomComponents = True

			ElseIf TypeOf msg Is Object() Then
				Dim msgs As Object() = CType(msg, Object())
				For Each o As Object In msgs
					addMessageComponents(container, cons, o, maxll, False)
				Next o

			ElseIf TypeOf msg Is Icon Then
				Dim label As New JLabel(CType(msg, Icon), SwingConstants.CENTER)
				configureMessageLabel(label)
				addMessageComponents(container, cons, label, maxll, True)

			Else
				Dim s As String = msg.ToString()
				Dim len As Integer = s.Length
				If len <= 0 Then Return
				Dim nl As Integer
				Dim nll As Integer = 0

				nl = s.IndexOf(newline)
				If nl >= 0 Then
					nll = newline.Length
				Else
					nl = s.IndexOf(vbCrLf)
					If nl >= 0 Then
						nll = 2
					Else
						nl = s.IndexOf(ControlChars.Lf)
						If nl >= 0 Then nll = 1
						End If
					End If
				If nl >= 0 Then
					' break up newlines
					If nl = 0 Then
						Dim breakPanel As JPanel = New JPanelAnonymousInnerClassHelper
						breakPanel.name = "OptionPane.break"
						addMessageComponents(container, cons, breakPanel, maxll, True)
					Else
						addMessageComponents(container, cons, s.Substring(0, nl), maxll, False)
					End If
					addMessageComponents(container, cons, s.Substring(nl + nll), maxll, False)

				ElseIf len > maxll Then
					Dim c As Container = Box.createVerticalBox()
					c.name = "OptionPane.verticalBox"
					burstStringInto(c, s, maxll)
					addMessageComponents(container, cons, c, maxll, True)

				Else
					Dim label As JLabel
					label = New JLabel(s, JLabel.LEADING)
					label.name = "OptionPane.label"
					configureMessageLabel(label)
					addMessageComponents(container, cons, label, maxll, True)
				End If
			End If
		End Sub

		Private Class JPanelAnonymousInnerClassHelper
			Inherits JPanel

			Public Property Overrides preferredSize As Dimension
				Get
					Dim f As Font = font
    
					If f IsNot Nothing Then Return New Dimension(1, f.size + 2)
					Return New Dimension(0, 0)
				End Get
			End Property
		End Class

		''' <summary>
		''' Returns the message to display from the JOptionPane the receiver is
		''' providing the look and feel for.
		''' </summary>
		Protected Friend Overridable Property message As Object
			Get
				inputComponent = Nothing
				If optionPane IsNot Nothing Then
					If optionPane.wantsInput Then
		'                 Create a user component to capture the input. If the
		'                   selectionValues are non null the component and there
		'                   are < 20 values it'll be a combobox, if non null and
		'                   >= 20, it'll be a list, otherwise it'll be a textfield. 
						Dim ___message As Object = optionPane.message
						Dim sValues As Object() = optionPane.selectionValues
						Dim inputValue As Object = optionPane.initialSelectionValue
						Dim toAdd As JComponent
    
						If sValues IsNot Nothing Then
							If sValues.Length < 20 Then
								Dim cBox As New JComboBox
    
								cBox.name = "OptionPane.comboBox"
								Dim counter As Integer = 0
								Dim maxCounter As Integer = sValues.Length
								Do While counter < maxCounter
									cBox.addItem(sValues(counter))
									counter += 1
								Loop
								If inputValue IsNot Nothing Then cBox.selectedItem = inputValue
								inputComponent = cBox
								toAdd = cBox
    
							Else
								Dim list As New JList(sValues)
								Dim sp As New JScrollPane(list)
    
								sp.name = "OptionPane.scrollPane"
								list.name = "OptionPane.list"
								list.visibleRowCount = 10
								list.selectionMode = ListSelectionModel.SINGLE_SELECTION
								If inputValue IsNot Nothing Then list.selectedValuelue(inputValue, True)
								list.addMouseListener(handler)
								toAdd = sp
								inputComponent = list
							End If
    
						Else
							Dim tf As New MultiplexingTextField(20)
    
							tf.name = "OptionPane.textField"
							tf.keyStrokes = New KeyStroke() { KeyStroke.getKeyStroke("ENTER") }
							If inputValue IsNot Nothing Then
								Dim inputString As String = inputValue.ToString()
								tf.text = inputString
								tf.selectionStart = 0
								tf.selectionEnd = inputString.Length
							End If
							tf.addActionListener(handler)
								inputComponent = tf
								toAdd = inputComponent
						End If
    
						Dim newMessage As Object()
    
						If ___message Is Nothing Then
							newMessage = New Object(0){}
							newMessage(0) = toAdd
    
						Else
							newMessage = New Object(1){}
							newMessage(0) = ___message
							newMessage(1) = toAdd
						End If
						Return newMessage
					End If
					Return optionPane.message
				End If
				Return Nothing
			End Get
		End Property

		''' <summary>
		''' Creates and adds a JLabel representing the icon returned from
		''' <code>getIcon</code> to <code>top</code>. This is messaged from
		''' <code>createMessageArea</code>
		''' </summary>
		Protected Friend Overridable Sub addIcon(ByVal top As Container)
			' Create the icon. 
			Dim sideIcon As Icon = icon

			If sideIcon IsNot Nothing Then
				Dim iconLabel As New JLabel(sideIcon)

				iconLabel.name = "OptionPane.iconLabel"
				iconLabel.verticalAlignment = SwingConstants.TOP
				top.add(iconLabel, BorderLayout.BEFORE_LINE_BEGINS)
			End If
		End Sub

		''' <summary>
		''' Returns the icon from the JOptionPane the receiver is providing
		''' the look and feel for, or the default icon as returned from
		''' <code>getDefaultIcon</code>.
		''' </summary>
		Protected Friend Overridable Property icon As Icon
			Get
				Dim mIcon As Icon = (If(optionPane Is Nothing, Nothing, optionPane.icon))
    
				If mIcon Is Nothing AndAlso optionPane IsNot Nothing Then mIcon = getIconForType(optionPane.messageType)
				Return mIcon
			End Get
		End Property

		''' <summary>
		''' Returns the icon to use for the passed in type.
		''' </summary>
		Protected Friend Overridable Function getIconForType(ByVal messageType As Integer) As Icon
			If messageType < 0 OrElse messageType > 3 Then Return Nothing
			Dim propertyName As String = Nothing
			Select Case messageType
			Case 0
				propertyName = "OptionPane.errorIcon"
			Case 1
				propertyName = "OptionPane.informationIcon"
			Case 2
				propertyName = "OptionPane.warningIcon"
			Case 3
				propertyName = "OptionPane.questionIcon"
			End Select
			If propertyName IsNot Nothing Then Return CType(sun.swing.DefaultLookup.get(optionPane, Me, propertyName), Icon)
			Return Nothing
		End Function

		''' <summary>
		''' Returns the maximum number of characters to place on a line.
		''' </summary>
		Protected Friend Overridable Property maxCharactersPerLineCount As Integer
			Get
				Return optionPane.maxCharactersPerLineCount
			End Get
		End Property

	   ''' <summary>
	   ''' Recursively creates new JLabel instances to represent <code>d</code>.
	   ''' Each JLabel instance is added to <code>c</code>.
	   ''' </summary>
		Protected Friend Overridable Sub burstStringInto(ByVal c As Container, ByVal d As String, ByVal maxll As Integer)
			' Primitive line wrapping
			Dim len As Integer = d.Length
			If len <= 0 Then Return
			If len > maxll Then
				Dim p As Integer = d.LastIndexOf(" "c, maxll)
				If p <= 0 Then p = d.IndexOf(" "c, maxll)
				If p > 0 AndAlso p < len Then
					burstStringInto(c, d.Substring(0, p), maxll)
					burstStringInto(c, d.Substring(p + 1), maxll)
					Return
				End If
			End If
			Dim label As New JLabel(d, JLabel.LEFT)
			label.name = "OptionPane.label"
			configureMessageLabel(label)
			c.add(label)
		End Sub

		Protected Friend Overridable Function createSeparator() As Container
			Return Nothing
		End Function

		''' <summary>
		''' Creates and returns a Container containing the buttons. The buttons
		''' are created by calling <code>getButtons</code>.
		''' </summary>
		Protected Friend Overridable Function createButtonArea() As Container
			Dim bottom As New JPanel
			Dim border As javax.swing.border.Border = CType(sun.swing.DefaultLookup.get(optionPane, Me, "OptionPane.buttonAreaBorder"), javax.swing.border.Border)
			bottom.name = "OptionPane.buttonArea"
			If border IsNot Nothing Then bottom.border = border
			bottom.layout = New ButtonAreaLayout(sun.swing.DefaultLookup.getBoolean(optionPane, Me, "OptionPane.sameSizeButtons", True), sun.swing.DefaultLookup.getInt(optionPane, Me, "OptionPane.buttonPadding", 6), sun.swing.DefaultLookup.getInt(optionPane, Me, "OptionPane.buttonOrientation", SwingConstants.CENTER), sun.swing.DefaultLookup.getBoolean(optionPane, Me, "OptionPane.isYesLast", False))
			addButtonComponents(bottom, buttons, initialValueIndex)
			Return bottom
		End Function

		''' <summary>
		''' Creates the appropriate object to represent each of the objects in
		''' <code>buttons</code> and adds it to <code>container</code>. This
		''' differs from addMessageComponents in that it will recurse on
		''' <code>buttons</code> and that if button is not a Component
		''' it will create an instance of JButton.
		''' </summary>
		Protected Friend Overridable Sub addButtonComponents(ByVal container As Container, ByVal buttons As Object(), ByVal initialIndex As Integer)
			If buttons IsNot Nothing AndAlso buttons.Length > 0 Then
				Dim sizeButtonsToSame As Boolean = sizeButtonsToSameWidth
				Dim createdAll As Boolean = True
				Dim numButtons As Integer = buttons.Length
				Dim createdButtons As JButton() = Nothing
				Dim maxWidth As Integer = 0

				If sizeButtonsToSame Then createdButtons = New JButton(numButtons - 1){}

				For counter As Integer = 0 To numButtons - 1
					Dim button As Object = buttons(counter)
					Dim newComponent As Component

					If TypeOf button Is Component Then
						createdAll = False
						newComponent = CType(button, Component)
						container.add(newComponent)
						hasCustomComponents = True

					Else
						Dim aButton As JButton

						If TypeOf button Is ButtonFactory Then
							aButton = CType(button, ButtonFactory).createButton()
						ElseIf TypeOf button Is Icon Then
							aButton = New JButton(CType(button, Icon))
						Else
							aButton = New JButton(button.ToString())
						End If

						aButton.name = "OptionPane.button"
						aButton.multiClickThreshhold = sun.swing.DefaultLookup.getInt(optionPane, Me, "OptionPane.buttonClickThreshhold", 0)
						configureButton(aButton)

						container.add(aButton)

						Dim buttonListener As ActionListener = createButtonActionListener(counter)
						If buttonListener IsNot Nothing Then aButton.addActionListener(buttonListener)
						newComponent = aButton
					End If
					If sizeButtonsToSame AndAlso createdAll AndAlso (TypeOf newComponent Is JButton) Then
						createdButtons(counter) = CType(newComponent, JButton)
						maxWidth = Math.Max(maxWidth, newComponent.minimumSize.width)
					End If
					If counter = initialIndex Then
						initialFocusComponent = newComponent
						If TypeOf initialFocusComponent Is JButton Then Dim defaultB As JButton = CType(initialFocusComponent, JButton)
					End If
				Next counter
				CType(container.layout, ButtonAreaLayout).syncAllWidths = (sizeButtonsToSame AndAlso createdAll)
	'             Set the padding, windows seems to use 8 if <= 2 components,
	'               otherwise 4 is used. It may actually just be the size of the
	'               buttons is always the same, not sure. 
				If sun.swing.DefaultLookup.getBoolean(optionPane, Me, "OptionPane.setButtonMargin", True) AndAlso sizeButtonsToSame AndAlso createdAll Then
					Dim aButton As JButton
					Dim padSize As Integer

					padSize = (If(numButtons <= 2, 8, 4))

					For counter As Integer = 0 To numButtons - 1
						aButton = createdButtons(counter)
						aButton.margin = New Insets(2, padSize, 2, padSize)
					Next counter
				End If
			End If
		End Sub

		Protected Friend Overridable Function createButtonActionListener(ByVal buttonIndex As Integer) As ActionListener
			Return New ButtonActionListener(Me, buttonIndex)
		End Function

		''' <summary>
		''' Returns the buttons to display from the JOptionPane the receiver is
		''' providing the look and feel for. If the JOptionPane has options
		''' set, they will be provided, otherwise if the optionType is
		''' YES_NO_OPTION, yesNoOptions is returned, if the type is
		''' YES_NO_CANCEL_OPTION yesNoCancelOptions is returned, otherwise
		''' defaultButtons are returned.
		''' </summary>
		Protected Friend Overridable Property buttons As Object()
			Get
				If optionPane IsNot Nothing Then
					Dim suppliedOptions As Object() = optionPane.options
    
					If suppliedOptions Is Nothing Then
						Dim defaultOptions As Object()
						Dim type As Integer = optionPane.optionType
						Dim l As java.util.Locale = optionPane.locale
						Dim ___minimumWidth As Integer = sun.swing.DefaultLookup.getInt(optionPane, Me, "OptionPane.buttonMinimumWidth",-1)
						If type = JOptionPane.YES_NO_OPTION Then
							defaultOptions = New ButtonFactory(1){}
							defaultOptions(0) = New ButtonFactory(UIManager.getString("OptionPane.yesButtonText", l), getMnemonic("OptionPane.yesButtonMnemonic", l), CType(sun.swing.DefaultLookup.get(optionPane, Me, "OptionPane.yesIcon"), Icon), ___minimumWidth)
							defaultOptions(1) = New ButtonFactory(UIManager.getString("OptionPane.noButtonText", l), getMnemonic("OptionPane.noButtonMnemonic", l), CType(sun.swing.DefaultLookup.get(optionPane, Me, "OptionPane.noIcon"), Icon), ___minimumWidth)
						ElseIf type = JOptionPane.YES_NO_CANCEL_OPTION Then
							defaultOptions = New ButtonFactory(2){}
							defaultOptions(0) = New ButtonFactory(UIManager.getString("OptionPane.yesButtonText", l), getMnemonic("OptionPane.yesButtonMnemonic", l), CType(sun.swing.DefaultLookup.get(optionPane, Me, "OptionPane.yesIcon"), Icon), ___minimumWidth)
							defaultOptions(1) = New ButtonFactory(UIManager.getString("OptionPane.noButtonText",l), getMnemonic("OptionPane.noButtonMnemonic", l), CType(sun.swing.DefaultLookup.get(optionPane, Me, "OptionPane.noIcon"), Icon), ___minimumWidth)
							defaultOptions(2) = New ButtonFactory(UIManager.getString("OptionPane.cancelButtonText",l), getMnemonic("OptionPane.cancelButtonMnemonic", l), CType(sun.swing.DefaultLookup.get(optionPane, Me, "OptionPane.cancelIcon"), Icon), ___minimumWidth)
						ElseIf type = JOptionPane.OK_CANCEL_OPTION Then
							defaultOptions = New ButtonFactory(1){}
							defaultOptions(0) = New ButtonFactory(UIManager.getString("OptionPane.okButtonText",l), getMnemonic("OptionPane.okButtonMnemonic", l), CType(sun.swing.DefaultLookup.get(optionPane, Me, "OptionPane.okIcon"), Icon), ___minimumWidth)
							defaultOptions(1) = New ButtonFactory(UIManager.getString("OptionPane.cancelButtonText",l), getMnemonic("OptionPane.cancelButtonMnemonic", l), CType(sun.swing.DefaultLookup.get(optionPane, Me, "OptionPane.cancelIcon"), Icon), ___minimumWidth)
						Else
							defaultOptions = New ButtonFactory(0){}
							defaultOptions(0) = New ButtonFactory(UIManager.getString("OptionPane.okButtonText",l), getMnemonic("OptionPane.okButtonMnemonic", l), CType(sun.swing.DefaultLookup.get(optionPane, Me, "OptionPane.okIcon"), Icon), ___minimumWidth)
						End If
						Return defaultOptions
    
					End If
					Return suppliedOptions
				End If
				Return Nothing
			End Get
		End Property

		Private Function getMnemonic(ByVal key As String, ByVal l As java.util.Locale) As Integer
			Dim value As String = CStr(UIManager.get(key, l))

			If value Is Nothing Then Return 0
			Try
				Return Convert.ToInt32(value)
			Catch nfe As NumberFormatException
			End Try
			Return 0
		End Function

		''' <summary>
		''' Returns true, basic L&amp;F wants all the buttons to have the same
		''' width.
		''' </summary>
		Protected Friend Overridable Property sizeButtonsToSameWidth As Boolean
			Get
				Return True
			End Get
		End Property

		''' <summary>
		''' Returns the initial index into the buttons to select. The index
		''' is calculated from the initial value from the JOptionPane and
		''' options of the JOptionPane or 0.
		''' </summary>
		Protected Friend Overridable Property initialValueIndex As Integer
			Get
				If optionPane IsNot Nothing Then
					Dim iv As Object = optionPane.initialValue
					Dim options As Object() = optionPane.options
    
					If options Is Nothing Then
						Return 0
					ElseIf iv IsNot Nothing Then
						For counter As Integer = options.Length - 1 To 0 Step -1
							If options(counter).Equals(iv) Then Return counter
						Next counter
					End If
				End If
				Return -1
			End Get
		End Property

		''' <summary>
		''' Sets the input value in the option pane the receiver is providing
		''' the look and feel for based on the value in the inputComponent.
		''' </summary>
		Protected Friend Overridable Sub resetInputValue()
			If inputComponent IsNot Nothing AndAlso (TypeOf inputComponent Is JTextField) Then
				optionPane.inputValue = CType(inputComponent, JTextField).text

			ElseIf inputComponent IsNot Nothing AndAlso (TypeOf inputComponent Is JComboBox) Then
				optionPane.inputValue = CType(inputComponent, JComboBox).selectedItem
			ElseIf inputComponent IsNot Nothing Then
				optionPane.inputValue = CType(inputComponent, JList).selectedValue
			End If
		End Sub


		''' <summary>
		''' If inputComponent is non-null, the focus is requested on that,
		''' otherwise request focus on the default value
		''' </summary>
		Public Overridable Sub selectInitialValue(ByVal op As JOptionPane)
			If inputComponent IsNot Nothing Then
				inputComponent.requestFocus()
			Else
				If initialFocusComponent IsNot Nothing Then initialFocusComponent.requestFocus()

				If TypeOf initialFocusComponent Is JButton Then
					Dim root As JRootPane = SwingUtilities.getRootPane(initialFocusComponent)
					If root IsNot Nothing Then root.defaultButton = CType(initialFocusComponent, JButton)
				End If
			End If
		End Sub

		''' <summary>
		''' Returns true if in the last call to validateComponent the message
		''' or buttons contained a subclass of Component.
		''' </summary>
		Public Overridable Function containsCustomComponents(ByVal op As JOptionPane) As Boolean
			Return hasCustomComponents
		End Function


		''' <summary>
		''' <code>ButtonAreaLayout</code> behaves in a similar manner to
		''' <code>FlowLayout</code>. It lays out all components from left to
		''' right. If <code>syncAllWidths</code> is true, the widths of each
		''' component will be set to the largest preferred size width.
		''' 
		''' This class should be treated as a &quot;protected&quot; inner class.
		''' Instantiate it only within subclasses of {@code BasicOptionPaneUI}.
		''' </summary>
		Public Class ButtonAreaLayout
			Implements LayoutManager

			Protected Friend syncAllWidths As Boolean
			Protected Friend padding As Integer
			''' <summary>
			''' If true, children are lumped together in parent. </summary>
			Protected Friend centersChildren As Boolean
			Private orientation As Integer
			Private reverseButtons As Boolean
			''' <summary>
			''' Indicates whether or not centersChildren should be used vs
			''' the orientation. This is done for backward compatibility
			''' for subclassers.
			''' </summary>
			Private useOrientation As Boolean

			Public Sub New(ByVal syncAllWidths As Boolean, ByVal padding As Integer)
				Me.syncAllWidths = syncAllWidths
				Me.padding = padding
				centersChildren = True
				useOrientation = False
			End Sub

			Friend Sub New(ByVal syncAllSizes As Boolean, ByVal padding As Integer, ByVal orientation As Integer, ByVal reverseButtons As Boolean)
				Me.New(syncAllSizes, padding)
				useOrientation = True
				Me.orientation = orientation
				Me.reverseButtons = reverseButtons
			End Sub

			Public Overridable Property syncAllWidths As Boolean
				Set(ByVal newValue As Boolean)
					syncAllWidths = newValue
				End Set
				Get
					Return syncAllWidths
				End Get
			End Property


			Public Overridable Property padding As Integer
				Set(ByVal newPadding As Integer)
					Me.padding = newPadding
				End Set
				Get
					Return padding
				End Get
			End Property


			Public Overridable Property centersChildren As Boolean
				Set(ByVal newValue As Boolean)
					centersChildren = newValue
					useOrientation = False
				End Set
				Get
					Return centersChildren
				End Get
			End Property


			Private Function getOrientation(ByVal container As Container) As Integer
				If Not useOrientation Then Return SwingConstants.CENTER
				If container.componentOrientation.leftToRight Then Return orientation
				Select Case orientation
				Case SwingConstants.LEFT
					Return SwingConstants.RIGHT
				Case SwingConstants.RIGHT
					Return SwingConstants.LEFT
				Case SwingConstants.CENTER
					Return SwingConstants.CENTER
				End Select
				Return SwingConstants.LEFT
			End Function

			Public Overridable Sub addLayoutComponent(ByVal [string] As String, ByVal comp As Component)
			End Sub

			Public Overridable Sub layoutContainer(ByVal container As Container)
				Dim children As Component() = container.components

				If children IsNot Nothing AndAlso children.Length > 0 Then
					Dim numChildren As Integer = children.Length
					Dim insets As Insets = container.insets
					Dim maxWidth As Integer = 0
					Dim maxHeight As Integer = 0
					Dim totalButtonWidth As Integer = 0
					Dim x As Integer = 0
					Dim xOffset As Integer = 0
					Dim ltr As Boolean = container.componentOrientation.leftToRight
					Dim reverse As Boolean = If(ltr, reverseButtons, (Not reverseButtons))

					For counter As Integer = 0 To numChildren - 1
						Dim pref As Dimension = children(counter).preferredSize
						maxWidth = Math.Max(maxWidth, pref.width)
						maxHeight = Math.Max(maxHeight, pref.height)
						totalButtonWidth += pref.width
					Next counter
					If syncAllWidths Then totalButtonWidth = maxWidth * numChildren
					totalButtonWidth += (numChildren - 1) * padding

					Select Case getOrientation(container)
					Case SwingConstants.LEFT
						x = insets.left
					Case SwingConstants.RIGHT
						x = container.width - insets.right - totalButtonWidth
					Case SwingConstants.CENTER
						If centersChildren OrElse numChildren < 2 Then
							x = (container.width - totalButtonWidth) / 2
						Else
							x = insets.left
							If syncAllWidths Then
								xOffset = (container.width - insets.left - insets.right - totalButtonWidth) / (numChildren - 1) + maxWidth
							Else
								xOffset = (container.width - insets.left - insets.right - totalButtonWidth) / (numChildren - 1)
							End If
						End If
					End Select

					For counter As Integer = 0 To numChildren - 1
						Dim index As Integer = If(reverse, numChildren - counter - 1, counter)
						Dim pref As Dimension = children(index).preferredSize

						If syncAllWidths Then
							children(index).boundsnds(x, insets.top, maxWidth, maxHeight)
						Else
							children(index).boundsnds(x, insets.top, pref.width, pref.height)
						End If
						If xOffset <> 0 Then
							x += xOffset
						Else
							x += children(index).width + padding
						End If
					Next counter
				End If
			End Sub

			Public Overridable Function minimumLayoutSize(ByVal c As Container) As Dimension
				If c IsNot Nothing Then
					Dim children As Component() = c.components

					If children IsNot Nothing AndAlso children.Length > 0 Then
						Dim aSize As Dimension
						Dim numChildren As Integer = children.Length
						Dim height As Integer = 0
						Dim cInsets As Insets = c.insets
						Dim extraHeight As Integer = cInsets.top + cInsets.bottom
						Dim extraWidth As Integer = cInsets.left + cInsets.right

						If syncAllWidths Then
							Dim maxWidth As Integer = 0

							For counter As Integer = 0 To numChildren - 1
								aSize = children(counter).preferredSize
								height = Math.Max(height, aSize.height)
								maxWidth = Math.Max(maxWidth, aSize.width)
							Next counter
							Return New Dimension(extraWidth + (maxWidth * numChildren) + (numChildren - 1) * padding, extraHeight + height)
						Else
							Dim totalWidth As Integer = 0

							For counter As Integer = 0 To numChildren - 1
								aSize = children(counter).preferredSize
								height = Math.Max(height, aSize.height)
								totalWidth += aSize.width
							Next counter
							totalWidth += ((numChildren - 1) * padding)
							Return New Dimension(extraWidth + totalWidth, extraHeight + height)
						End If
					End If
				End If
				Return New Dimension(0, 0)
			End Function

			Public Overridable Function preferredLayoutSize(ByVal c As Container) As Dimension
				Return minimumLayoutSize(c)
			End Function

			Public Overridable Sub removeLayoutComponent(ByVal c As Component)
			End Sub
		End Class


		''' <summary>
		''' This class should be treated as a &quot;protected&quot; inner class.
		''' Instantiate it only within subclasses of {@code BasicOptionPaneUI}.
		''' </summary>
		Public Class PropertyChangeHandler
			Implements java.beans.PropertyChangeListener

			Private ReadOnly outerInstance As BasicOptionPaneUI

			Public Sub New(ByVal outerInstance As BasicOptionPaneUI)
				Me.outerInstance = outerInstance
			End Sub

			''' <summary>
			''' If the source of the PropertyChangeEvent <code>e</code> equals the
			''' optionPane and is one of the ICON_PROPERTY, MESSAGE_PROPERTY,
			''' OPTIONS_PROPERTY or INITIAL_VALUE_PROPERTY,
			''' validateComponent is invoked.
			''' </summary>
			Public Overridable Sub propertyChange(ByVal e As java.beans.PropertyChangeEvent)
				outerInstance.handler.propertyChange(e)
			End Sub
		End Class

		''' <summary>
		''' Configures any necessary colors/fonts for the specified label
		''' used representing the message.
		''' </summary>
		Private Sub configureMessageLabel(ByVal label As JLabel)
			Dim color As Color = CType(sun.swing.DefaultLookup.get(optionPane, Me, "OptionPane.messageForeground"), Color)
			If color IsNot Nothing Then label.foreground = color
			Dim messageFont As Font = CType(sun.swing.DefaultLookup.get(optionPane, Me, "OptionPane.messageFont"), Font)
			If messageFont IsNot Nothing Then label.font = messageFont
		End Sub

		''' <summary>
		''' Configures any necessary colors/fonts for the specified button
		''' used representing the button portion of the optionpane.
		''' </summary>
		Private Sub configureButton(ByVal button As JButton)
			Dim buttonFont As Font = CType(sun.swing.DefaultLookup.get(optionPane, Me, "OptionPane.buttonFont"), Font)
			If buttonFont IsNot Nothing Then button.font = buttonFont
		End Sub

		''' <summary>
		''' This class should be treated as a &quot;protected&quot; inner class.
		''' Instantiate it only within subclasses of {@code BasicOptionPaneUI}.
		''' </summary>
		Public Class ButtonActionListener
			Implements ActionListener

			Private ReadOnly outerInstance As BasicOptionPaneUI

			Protected Friend buttonIndex As Integer

			Public Sub New(ByVal outerInstance As BasicOptionPaneUI, ByVal buttonIndex As Integer)
					Me.outerInstance = outerInstance
				Me.buttonIndex = buttonIndex
			End Sub

			Public Overridable Sub actionPerformed(ByVal e As ActionEvent)
				If outerInstance.optionPane IsNot Nothing Then
					Dim optionType As Integer = outerInstance.optionPane.optionType
					Dim options As Object() = outerInstance.optionPane.options

	'                 If the option pane takes input, then store the input value
	'                 * if custom options were specified, if the option type is
	'                 * DEFAULT_OPTION, OR if option type is set to a predefined
	'                 * one and the user chose the affirmative answer.
	'                 
					If outerInstance.inputComponent IsNot Nothing Then
						If options IsNot Nothing OrElse optionType = JOptionPane.DEFAULT_OPTION OrElse ((optionType = JOptionPane.YES_NO_OPTION OrElse optionType = JOptionPane.YES_NO_CANCEL_OPTION OrElse optionType = JOptionPane.OK_CANCEL_OPTION) AndAlso buttonIndex = 0) Then outerInstance.resetInputValue()
					End If
					If options Is Nothing Then
						If optionType = JOptionPane.OK_CANCEL_OPTION AndAlso buttonIndex = 1 Then
							outerInstance.optionPane.value = Convert.ToInt32(2)

						Else
							outerInstance.optionPane.value = Convert.ToInt32(buttonIndex)
						End If
					Else
						outerInstance.optionPane.value = options(buttonIndex)
					End If
				End If
			End Sub
		End Class


		Private Class Handler
			Implements ActionListener, MouseListener, java.beans.PropertyChangeListener

			Private ReadOnly outerInstance As BasicOptionPaneUI

			Public Sub New(ByVal outerInstance As BasicOptionPaneUI)
				Me.outerInstance = outerInstance
			End Sub

			'
			' ActionListener
			'
			Public Overridable Sub actionPerformed(ByVal e As ActionEvent)
				outerInstance.optionPane.inputValue = CType(e.source, JTextField).text
			End Sub


			'
			' MouseListener
			'
			Public Overridable Sub mouseClicked(ByVal e As MouseEvent)
			End Sub

			Public Overridable Sub mouseReleased(ByVal e As MouseEvent)
			End Sub

			Public Overridable Sub mouseEntered(ByVal e As MouseEvent)
			End Sub

			Public Overridable Sub mouseExited(ByVal e As MouseEvent)
			End Sub

			Public Overridable Sub mousePressed(ByVal e As MouseEvent)
				If e.clickCount = 2 Then
					Dim list As JList = CType(e.source, JList)
					Dim index As Integer = list.locationToIndex(e.point)

					outerInstance.optionPane.inputValue = list.model.getElementAt(index)
					outerInstance.optionPane.value = DialogResult.OK
				End If
			End Sub

			'
			' PropertyChangeListener
			'
			Public Overridable Sub propertyChange(ByVal e As java.beans.PropertyChangeEvent)
				If e.source Is outerInstance.optionPane Then
					' Option Pane Auditory Cue Activation
					' only respond to "ancestor" changes
					' the idea being that a JOptionPane gets a JDialog when it is
					' set to appear and loses it's JDialog when it is dismissed.
					If "ancestor" = e.propertyName Then
						Dim op As JOptionPane = CType(e.source, JOptionPane)
						Dim isComingUp As Boolean

						' if the old value is null, then the JOptionPane is being
						' created since it didn't previously have an ancestor.
						If e.oldValue Is Nothing Then
							isComingUp = True
						Else
							isComingUp = False
						End If

						' figure out what to do based on the message type
						Select Case op.messageType
						Case JOptionPane.PLAIN_MESSAGE
							If isComingUp Then BasicLookAndFeel.playSound(outerInstance.optionPane, "OptionPane.informationSound")
						Case JOptionPane.QUESTION_MESSAGE
							If isComingUp Then BasicLookAndFeel.playSound(outerInstance.optionPane, "OptionPane.questionSound")
						Case JOptionPane.INFORMATION_MESSAGE
							If isComingUp Then BasicLookAndFeel.playSound(outerInstance.optionPane, "OptionPane.informationSound")
						Case JOptionPane.WARNING_MESSAGE
							If isComingUp Then BasicLookAndFeel.playSound(outerInstance.optionPane, "OptionPane.warningSound")
						Case JOptionPane.ERROR_MESSAGE
							If isComingUp Then BasicLookAndFeel.playSound(outerInstance.optionPane, "OptionPane.errorSound")
						Case Else
							Console.Error.WriteLine("Undefined JOptionPane type: " & op.messageType)
						End Select
					End If
					' Visual activity
					Dim changeName As String = e.propertyName

					If changeName = JOptionPane.OPTIONS_PROPERTY OrElse changeName = JOptionPane.INITIAL_VALUE_PROPERTY OrElse changeName = JOptionPane.ICON_PROPERTY OrElse changeName = JOptionPane.MESSAGE_TYPE_PROPERTY OrElse changeName = JOptionPane.OPTION_TYPE_PROPERTY OrElse changeName = JOptionPane.MESSAGE_PROPERTY OrElse changeName = JOptionPane.SELECTION_VALUES_PROPERTY OrElse changeName = JOptionPane.INITIAL_SELECTION_VALUE_PROPERTY OrElse changeName = JOptionPane.WANTS_INPUT_PROPERTY Then
					   outerInstance.uninstallComponents()
					   outerInstance.installComponents()
					   outerInstance.optionPane.validate()
					ElseIf changeName = "componentOrientation" Then
						Dim o As ComponentOrientation = CType(e.newValue, ComponentOrientation)
						Dim op As JOptionPane = CType(e.source, JOptionPane)
						If o IsNot e.oldValue Then op.applyComponentOrientation(o)
					End If
				End If
			End Sub
		End Class


		'
		' Classes used when optionPane.getWantsInput returns true.
		'

		''' <summary>
		''' A JTextField that allows you to specify an array of KeyStrokes that
		''' that will have their bindings processed regardless of whether or
		''' not they are registered on the JTextField. This is used as we really
		''' want the ActionListener to be notified so that we can push the
		''' change to the JOptionPane, but we also want additional bindings
		''' (those of the JRootPane) to be processed as well.
		''' </summary>
		Private Class MultiplexingTextField
			Inherits JTextField

			Private strokes As KeyStroke()

			Friend Sub New(ByVal cols As Integer)
				MyBase.New(cols)
			End Sub

			''' <summary>
			''' Sets the KeyStrokes that will be additional processed for
			''' ancestor bindings.
			''' </summary>
			Friend Overridable Property keyStrokes As KeyStroke()
				Set(ByVal strokes As KeyStroke())
					Me.strokes = strokes
				End Set
			End Property

			Protected Friend Overrides Function processKeyBinding(ByVal ks As KeyStroke, ByVal e As KeyEvent, ByVal condition As Integer, ByVal pressed As Boolean) As Boolean
				Dim processed As Boolean = MyBase.processKeyBinding(ks, e, condition, pressed)

				If processed AndAlso condition <> JComponent.WHEN_IN_FOCUSED_WINDOW Then
					For counter As Integer = strokes.Length - 1 To 0 Step -1
						If strokes(counter).Equals(ks) Then Return False
					Next counter
				End If
				Return processed
			End Function
		End Class



		''' <summary>
		''' Registered in the ActionMap. Sets the value of the option pane
		''' to <code>JOptionPane.CLOSED_OPTION</code>.
		''' </summary>
		Private Class Actions
			Inherits sun.swing.UIAction

			Private Const CLOSE As String = "close"

			Friend Sub New(ByVal key As String)
				MyBase.New(key)
			End Sub

			Public Overridable Sub actionPerformed(ByVal e As ActionEvent)
				If name = CLOSE Then
					Dim optionPane As JOptionPane = CType(e.source, JOptionPane)

					optionPane.value = Convert.ToInt32(JOptionPane.CLOSED_OPTION)
				End If
			End Sub
		End Class


		''' <summary>
		''' This class is used to create the default buttons. This indirection is
		''' used so that addButtonComponents can tell which Buttons were created
		''' by us vs subclassers or from the JOptionPane itself.
		''' </summary>
		Private Class ButtonFactory
			Private text As String
			Private mnemonic As Integer
			Private icon As Icon
			Private minimumWidth As Integer = -1

			Friend Sub New(ByVal text As String, ByVal mnemonic As Integer, ByVal icon As Icon, ByVal minimumWidth As Integer)
				Me.text = text
				Me.mnemonic = mnemonic
				Me.icon = icon
				Me.minimumWidth = minimumWidth
			End Sub

			Friend Overridable Function createButton() As JButton
				Dim button As JButton

				If minimumWidth > 0 Then
					button = New ConstrainedButton(text, minimumWidth)
				Else
					button = New JButton(text)
				End If
				If icon IsNot Nothing Then button.icon = icon
				If mnemonic <> 0 Then button.mnemonic = mnemonic
				Return button
			End Function

			Private Class ConstrainedButton
				Inherits JButton

				Friend minimumWidth As Integer

				Friend Sub New(ByVal text As String, ByVal minimumWidth As Integer)
					MyBase.New(text)
					Me.minimumWidth = minimumWidth
				End Sub

				Public Property Overrides minimumSize As Dimension
					Get
						Dim min As Dimension = MyBase.minimumSize
						min.width = Math.Max(min.width, minimumWidth)
						Return min
					End Get
				End Property

				Public Property Overrides preferredSize As Dimension
					Get
						Dim pref As Dimension = MyBase.preferredSize
						pref.width = Math.Max(pref.width, minimumWidth)
						Return pref
					End Get
				End Property
			End Class
		End Class
	End Class

End Namespace
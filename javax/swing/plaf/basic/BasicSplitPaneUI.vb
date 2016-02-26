Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports javax.swing

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
	''' A Basic L&amp;F implementation of the SplitPaneUI.
	''' 
	''' @author Scott Violet
	''' @author Steve Wilson
	''' @author Ralph Kar
	''' </summary>
	Public Class BasicSplitPaneUI
		Inherits javax.swing.plaf.SplitPaneUI

		''' <summary>
		''' The divider used for non-continuous layout is added to the split pane
		''' with this object.
		''' </summary>
		Protected Friend Const NON_CONTINUOUS_DIVIDER As String = "nonContinuousDivider"


		''' <summary>
		''' How far (relative) the divider does move when it is moved around by
		''' the cursor keys on the keyboard.
		''' </summary>
		Protected Friend Shared KEYBOARD_DIVIDER_MOVE_OFFSET As Integer = 3


		''' <summary>
		''' JSplitPane instance this instance is providing
		''' the look and feel for.
		''' </summary>
		Protected Friend splitPane As JSplitPane


		''' <summary>
		''' LayoutManager that is created and placed into the split pane.
		''' </summary>
		Protected Friend layoutManager As BasicHorizontalLayoutManager


		''' <summary>
		''' Instance of the divider for this JSplitPane.
		''' </summary>
		Protected Friend divider As BasicSplitPaneDivider


		''' <summary>
		''' Instance of the PropertyChangeListener for this JSplitPane.
		''' </summary>
		Protected Friend propertyChangeListener As PropertyChangeListener


		''' <summary>
		''' Instance of the FocusListener for this JSplitPane.
		''' </summary>
		Protected Friend focusListener As FocusListener

		Private handler As Handler


		''' <summary>
		''' Keys to use for forward focus traversal when the JComponent is
		''' managing focus.
		''' </summary>
		Private managingFocusForwardTraversalKeys As [Set](Of KeyStroke)

		''' <summary>
		''' Keys to use for backward focus traversal when the JComponent is
		''' managing focus.
		''' </summary>
		Private managingFocusBackwardTraversalKeys As [Set](Of KeyStroke)


		''' <summary>
		''' The size of the divider while the dragging session is valid.
		''' </summary>
		Protected Friend dividerSize As Integer


		''' <summary>
		''' Instance for the shadow of the divider when non continuous layout
		''' is being used.
		''' </summary>
		Protected Friend nonContinuousLayoutDivider As Component


		''' <summary>
		''' Set to true in startDragging if any of the children
		''' (not including the nonContinuousLayoutDivider) are heavy weights.
		''' </summary>
		Protected Friend draggingHW As Boolean


		''' <summary>
		''' Location of the divider when the dragging session began.
		''' </summary>
		Protected Friend beginDragDividerLocation As Integer


		''' <summary>
		''' As of Java 2 platform v1.3 this previously undocumented field is no
		''' longer used.
		''' Key bindings are now defined by the LookAndFeel, please refer to
		''' the key bindings specification for further details.
		''' </summary>
		''' @deprecated As of Java 2 platform v1.3. 
		<Obsolete("As of Java 2 platform v1.3.")> _
		Protected Friend upKey As KeyStroke
		''' <summary>
		''' As of Java 2 platform v1.3 this previously undocumented field is no
		''' longer used.
		''' Key bindings are now defined by the LookAndFeel, please refer to
		''' the key bindings specification for further details.
		''' </summary>
		''' @deprecated As of Java 2 platform v1.3. 
		<Obsolete("As of Java 2 platform v1.3.")> _
		Protected Friend downKey As KeyStroke
		''' <summary>
		''' As of Java 2 platform v1.3 this previously undocumented field is no
		''' longer used.
		''' Key bindings are now defined by the LookAndFeel, please refer to
		''' the key bindings specification for further details.
		''' </summary>
		''' @deprecated As of Java 2 platform v1.3. 
		<Obsolete("As of Java 2 platform v1.3.")> _
		Protected Friend leftKey As KeyStroke
		''' <summary>
		''' As of Java 2 platform v1.3 this previously undocumented field is no
		''' longer used.
		''' Key bindings are now defined by the LookAndFeel, please refer to
		''' the key bindings specification for further details.
		''' </summary>
		''' @deprecated As of Java 2 platform v1.3. 
		<Obsolete("As of Java 2 platform v1.3.")> _
		Protected Friend rightKey As KeyStroke
		''' <summary>
		''' As of Java 2 platform v1.3 this previously undocumented field is no
		''' longer used.
		''' Key bindings are now defined by the LookAndFeel, please refer to
		''' the key bindings specification for further details.
		''' </summary>
		''' @deprecated As of Java 2 platform v1.3. 
		<Obsolete("As of Java 2 platform v1.3.")> _
		Protected Friend homeKey As KeyStroke
		''' <summary>
		''' As of Java 2 platform v1.3 this previously undocumented field is no
		''' longer used.
		''' Key bindings are now defined by the LookAndFeel, please refer to
		''' the key bindings specification for further details.
		''' </summary>
		''' @deprecated As of Java 2 platform v1.3. 
		<Obsolete("As of Java 2 platform v1.3.")> _
		Protected Friend endKey As KeyStroke
		''' <summary>
		''' As of Java 2 platform v1.3 this previously undocumented field is no
		''' longer used.
		''' Key bindings are now defined by the LookAndFeel, please refer to
		''' the key bindings specification for further details.
		''' </summary>
		''' @deprecated As of Java 2 platform v1.3. 
		<Obsolete("As of Java 2 platform v1.3.")> _
		Protected Friend dividerResizeToggleKey As KeyStroke

		''' <summary>
		''' As of Java 2 platform v1.3 this previously undocumented field is no
		''' longer used.
		''' Key bindings are now defined by the LookAndFeel, please refer to
		''' the key bindings specification for further details.
		''' </summary>
		''' @deprecated As of Java 2 platform v1.3. 
		<Obsolete("As of Java 2 platform v1.3.")> _
		Protected Friend keyboardUpLeftListener As ActionListener
		''' <summary>
		''' As of Java 2 platform v1.3 this previously undocumented field is no
		''' longer used.
		''' Key bindings are now defined by the LookAndFeel, please refer to
		''' the key bindings specification for further details.
		''' </summary>
		''' @deprecated As of Java 2 platform v1.3. 
		<Obsolete("As of Java 2 platform v1.3.")> _
		Protected Friend keyboardDownRightListener As ActionListener
		''' <summary>
		''' As of Java 2 platform v1.3 this previously undocumented field is no
		''' longer used.
		''' Key bindings are now defined by the LookAndFeel, please refer to
		''' the key bindings specification for further details.
		''' </summary>
		''' @deprecated As of Java 2 platform v1.3. 
		<Obsolete("As of Java 2 platform v1.3.")> _
		Protected Friend keyboardHomeListener As ActionListener
		''' <summary>
		''' As of Java 2 platform v1.3 this previously undocumented field is no
		''' longer used.
		''' Key bindings are now defined by the LookAndFeel, please refer to
		''' the key bindings specification for further details.
		''' </summary>
		''' @deprecated As of Java 2 platform v1.3. 
		<Obsolete("As of Java 2 platform v1.3.")> _
		Protected Friend keyboardEndListener As ActionListener
		''' <summary>
		''' As of Java 2 platform v1.3 this previously undocumented field is no
		''' longer used.
		''' Key bindings are now defined by the LookAndFeel, please refer to
		''' the key bindings specification for further details.
		''' </summary>
		''' @deprecated As of Java 2 platform v1.3. 
		<Obsolete("As of Java 2 platform v1.3.")> _
		Protected Friend keyboardResizeToggleListener As ActionListener


		' Private data of the instance
		Private orientation As Integer
		Private lastDragLocation As Integer
		Private continuousLayout As Boolean
		Private dividerKeyboardResize As Boolean
		Private dividerLocationIsSet As Boolean ' needed for tracking
												   ' the first occurrence of
												   ' setDividerLocation()
		Private dividerDraggingColor As Color
		Private rememberPaneSizes As Boolean

		' Indicates whether the one of splitpane sides is expanded
		Private keepHidden As Boolean = False

		''' <summary>
		''' Indicates that we have painted once. </summary>
		' This is used by the LayoutManager to determine when it should use
		' the divider location provided by the JSplitPane. This is used as there
		' is no way to determine when the layout process has completed.
		Friend painted As Boolean
		''' <summary>
		''' If true, setDividerLocation does nothing. </summary>
		Friend ignoreDividerLocationChange As Boolean


		''' <summary>
		''' Creates a new BasicSplitPaneUI instance
		''' </summary>
		Public Shared Function createUI(ByVal x As JComponent) As javax.swing.plaf.ComponentUI
			Return New BasicSplitPaneUI
		End Function

		Friend Shared Sub loadActionMap(ByVal map As LazyActionMap)
			map.put(New Actions(Actions.NEGATIVE_INCREMENT))
			map.put(New Actions(Actions.POSITIVE_INCREMENT))
			map.put(New Actions(Actions.SELECT_MIN))
			map.put(New Actions(Actions.SELECT_MAX))
			map.put(New Actions(Actions.START_RESIZE))
			map.put(New Actions(Actions.TOGGLE_FOCUS))
			map.put(New Actions(Actions.FOCUS_OUT_FORWARD))
			map.put(New Actions(Actions.FOCUS_OUT_BACKWARD))
		End Sub



		''' <summary>
		''' Installs the UI.
		''' </summary>
		Public Overridable Sub installUI(ByVal c As JComponent)
			splitPane = CType(c, JSplitPane)
			dividerLocationIsSet = False
			dividerKeyboardResize = False
			keepHidden = False
			installDefaults()
			installListeners()
			installKeyboardActions()
			lastDragLocation = -1
		End Sub


		''' <summary>
		''' Installs the UI defaults.
		''' </summary>
		Protected Friend Overridable Sub installDefaults()
			LookAndFeel.installBorder(splitPane, "SplitPane.border")
			LookAndFeel.installColors(splitPane, "SplitPane.background", "SplitPane.foreground")
			LookAndFeel.installProperty(splitPane, "opaque", Boolean.TRUE)

			If divider Is Nothing Then divider = createDefaultDivider()
			divider.basicSplitPaneUI = Me

			Dim b As javax.swing.border.Border = divider.border

			If b Is Nothing OrElse Not(TypeOf b Is javax.swing.plaf.UIResource) Then divider.border = UIManager.getBorder("SplitPaneDivider.border")

			dividerDraggingColor = UIManager.getColor("SplitPaneDivider.draggingColor")

			orientation = splitPane.orientation

			' note: don't rename this temp variable to dividerSize
			' since it will conflict with "this.dividerSize" field
			Dim temp As Integer? = CInt(Fix(UIManager.get("SplitPane.dividerSize")))
			LookAndFeel.installProperty(splitPane, "dividerSize",If(temp Is Nothing, 10, temp))

			divider.dividerSize = splitPane.dividerSize
			dividerSize = divider.dividerSize
			splitPane.add(divider, JSplitPane.DIVIDER)

			continuousLayout = splitPane.continuousLayout

			resetLayoutManager()

	'         Install the nonContinuousLayoutDivider here to avoid having to
	'        add/remove everything later. 
			If nonContinuousLayoutDivider Is Nothing Then
				nonContinuousLayoutDividerder(createDefaultNonContinuousLayoutDivider(), True)
			Else
				nonContinuousLayoutDividerder(nonContinuousLayoutDivider, True)
			End If

			' focus forward traversal key
			If managingFocusForwardTraversalKeys Is Nothing Then
				managingFocusForwardTraversalKeys = New HashSet(Of KeyStroke)
				managingFocusForwardTraversalKeys.add(KeyStroke.getKeyStroke(KeyEvent.VK_TAB, 0))
			End If
			splitPane.focusTraversalKeyseys(KeyboardFocusManager.FORWARD_TRAVERSAL_KEYS, managingFocusForwardTraversalKeys)
			' focus backward traversal key
			If managingFocusBackwardTraversalKeys Is Nothing Then
				managingFocusBackwardTraversalKeys = New HashSet(Of KeyStroke)
				managingFocusBackwardTraversalKeys.add(KeyStroke.getKeyStroke(KeyEvent.VK_TAB, InputEvent.SHIFT_MASK))
			End If
			splitPane.focusTraversalKeyseys(KeyboardFocusManager.BACKWARD_TRAVERSAL_KEYS, managingFocusBackwardTraversalKeys)
		End Sub


		''' <summary>
		''' Installs the event listeners for the UI.
		''' </summary>
		Protected Friend Overridable Sub installListeners()
			propertyChangeListener = createPropertyChangeListener()
			If propertyChangeListener IsNot Nothing Then splitPane.addPropertyChangeListener(propertyChangeListener)

			focusListener = createFocusListener()
			If focusListener IsNot Nothing Then splitPane.addFocusListener(focusListener)
		End Sub


		''' <summary>
		''' Installs the keyboard actions for the UI.
		''' </summary>
		Protected Friend Overridable Sub installKeyboardActions()
			Dim km As InputMap = getInputMap(JComponent.WHEN_ANCESTOR_OF_FOCUSED_COMPONENT)

			SwingUtilities.replaceUIInputMap(splitPane, JComponent.WHEN_ANCESTOR_OF_FOCUSED_COMPONENT, km)
			LazyActionMap.installLazyActionMap(splitPane, GetType(BasicSplitPaneUI), "SplitPane.actionMap")
		End Sub

		Friend Overridable Function getInputMap(ByVal condition As Integer) As InputMap
			If condition = JComponent.WHEN_ANCESTOR_OF_FOCUSED_COMPONENT Then Return CType(sun.swing.DefaultLookup.get(splitPane, Me, "SplitPane.ancestorInputMap"), InputMap)
			Return Nothing
		End Function

		''' <summary>
		''' Uninstalls the UI.
		''' </summary>
		Public Overridable Sub uninstallUI(ByVal c As JComponent)
			uninstallKeyboardActions()
			uninstallListeners()
			uninstallDefaults()
			dividerLocationIsSet = False
			dividerKeyboardResize = False
			splitPane = Nothing
		End Sub


		''' <summary>
		''' Uninstalls the UI defaults.
		''' </summary>
		Protected Friend Overridable Sub uninstallDefaults()
			If splitPane.layout Is layoutManager Then splitPane.layout = Nothing

			If nonContinuousLayoutDivider IsNot Nothing Then splitPane.remove(nonContinuousLayoutDivider)

			LookAndFeel.uninstallBorder(splitPane)

			Dim b As javax.swing.border.Border = divider.border

			If TypeOf b Is javax.swing.plaf.UIResource Then divider.border = Nothing

			splitPane.remove(divider)
			divider.basicSplitPaneUI = Nothing
			layoutManager = Nothing
			divider = Nothing
			nonContinuousLayoutDivider = Nothing

			nonContinuousLayoutDivider = Nothing

			' sets the focus forward and backward traversal keys to null
			' to restore the defaults
			splitPane.focusTraversalKeyseys(KeyboardFocusManager.FORWARD_TRAVERSAL_KEYS, Nothing)
			splitPane.focusTraversalKeyseys(KeyboardFocusManager.BACKWARD_TRAVERSAL_KEYS, Nothing)
		End Sub


		''' <summary>
		''' Uninstalls the event listeners for the UI.
		''' </summary>
		Protected Friend Overridable Sub uninstallListeners()
			If propertyChangeListener IsNot Nothing Then
				splitPane.removePropertyChangeListener(propertyChangeListener)
				propertyChangeListener = Nothing
			End If
			If focusListener IsNot Nothing Then
				splitPane.removeFocusListener(focusListener)
				focusListener = Nothing
			End If

			keyboardUpLeftListener = Nothing
			keyboardDownRightListener = Nothing
			keyboardHomeListener = Nothing
			keyboardEndListener = Nothing
			keyboardResizeToggleListener = Nothing
			handler = Nothing
		End Sub


		''' <summary>
		''' Uninstalls the keyboard actions for the UI.
		''' </summary>
		Protected Friend Overridable Sub uninstallKeyboardActions()
			SwingUtilities.replaceUIActionMap(splitPane, Nothing)
			SwingUtilities.replaceUIInputMap(splitPane, JComponent.WHEN_ANCESTOR_OF_FOCUSED_COMPONENT, Nothing)
		End Sub


		''' <summary>
		''' Creates a PropertyChangeListener for the JSplitPane UI.
		''' </summary>
		Protected Friend Overridable Function createPropertyChangeListener() As PropertyChangeListener
			Return handler
		End Function

		Private Property handler As Handler
			Get
				If handler Is Nothing Then handler = New Handler(Me)
				Return handler
			End Get
		End Property


		''' <summary>
		''' Creates a FocusListener for the JSplitPane UI.
		''' </summary>
		Protected Friend Overridable Function createFocusListener() As FocusListener
			Return handler
		End Function


		''' <summary>
		''' As of Java 2 platform v1.3 this method is no
		''' longer used. Subclassers previously using this method should
		''' instead create an Action wrapping the ActionListener, and register
		''' that Action by overriding <code>installKeyboardActions</code> and
		''' placing the Action in the SplitPane's ActionMap. Please refer to
		''' the key bindings specification for further details.
		''' <p>
		''' Creates a ActionListener for the JSplitPane UI that listens for
		''' specific key presses.
		''' </summary>
		''' @deprecated As of Java 2 platform v1.3. 
		<Obsolete("As of Java 2 platform v1.3.")> _
		Protected Friend Overridable Function createKeyboardUpLeftListener() As ActionListener
			Return New KeyboardUpLeftHandler(Me)
		End Function


		''' <summary>
		''' As of Java 2 platform v1.3 this method is no
		''' longer used. Subclassers previously using this method should
		''' instead create an Action wrapping the ActionListener, and register
		''' that Action by overriding <code>installKeyboardActions</code> and
		''' placing the Action in the SplitPane's ActionMap. Please refer to
		''' the key bindings specification for further details.
		''' <p>
		''' Creates a ActionListener for the JSplitPane UI that listens for
		''' specific key presses.
		''' </summary>
		''' @deprecated As of Java 2 platform v1.3. 
		<Obsolete("As of Java 2 platform v1.3.")> _
		Protected Friend Overridable Function createKeyboardDownRightListener() As ActionListener
			Return New KeyboardDownRightHandler(Me)
		End Function


		''' <summary>
		''' As of Java 2 platform v1.3 this method is no
		''' longer used. Subclassers previously using this method should
		''' instead create an Action wrapping the ActionListener, and register
		''' that Action by overriding <code>installKeyboardActions</code> and
		''' placing the Action in the SplitPane's ActionMap. Please refer to
		''' the key bindings specification for further details.
		''' <p>
		''' Creates a ActionListener for the JSplitPane UI that listens for
		''' specific key presses.
		''' </summary>
		''' @deprecated As of Java 2 platform v1.3. 
		<Obsolete("As of Java 2 platform v1.3.")> _
		Protected Friend Overridable Function createKeyboardHomeListener() As ActionListener
			Return New KeyboardHomeHandler(Me)
		End Function


		''' <summary>
		''' As of Java 2 platform v1.3 this method is no
		''' longer used. Subclassers previously using this method should
		''' instead create an Action wrapping the ActionListener, and register
		''' that Action by overriding <code>installKeyboardActions</code> and
		''' placing the Action in the SplitPane's ActionMap. Please refer to
		''' the key bindings specification for further details.
		''' <p>
		''' Creates a ActionListener for the JSplitPane UI that listens for
		''' specific key presses.
		''' </summary>
		''' @deprecated As of Java 2 platform v1.3. 
		<Obsolete("As of Java 2 platform v1.3.")> _
		Protected Friend Overridable Function createKeyboardEndListener() As ActionListener
			Return New KeyboardEndHandler(Me)
		End Function


		''' <summary>
		''' As of Java 2 platform v1.3 this method is no
		''' longer used. Subclassers previously using this method should
		''' instead create an Action wrapping the ActionListener, and register
		''' that Action by overriding <code>installKeyboardActions</code> and
		''' placing the Action in the SplitPane's ActionMap. Please refer to
		''' the key bindings specification for further details.
		''' <p>
		''' Creates a ActionListener for the JSplitPane UI that listens for
		''' specific key presses.
		''' </summary>
		''' @deprecated As of Java 2 platform v1.3. 
		<Obsolete("As of Java 2 platform v1.3.")> _
		Protected Friend Overridable Function createKeyboardResizeToggleListener() As ActionListener
			Return New KeyboardResizeToggleHandler(Me)
		End Function


		''' <summary>
		''' Returns the orientation for the JSplitPane.
		''' </summary>
		Public Overridable Property orientation As Integer
			Get
				Return orientation
			End Get
			Set(ByVal orientation As Integer)
				Me.orientation = orientation
			End Set
		End Property




		''' <summary>
		''' Determines whether the JSplitPane is set to use a continuous layout.
		''' </summary>
		Public Overridable Property continuousLayout As Boolean
			Get
				Return continuousLayout
			End Get
			Set(ByVal b As Boolean)
				continuousLayout = b
			End Set
		End Property




		''' <summary>
		''' Returns the last drag location of the JSplitPane.
		''' </summary>
		Public Overridable Property lastDragLocation As Integer
			Get
				Return lastDragLocation
			End Get
			Set(ByVal l As Integer)
				lastDragLocation = l
			End Set
		End Property



		''' <returns> increment via keyboard methods. </returns>
		Friend Overridable Property keyboardMoveIncrement As Integer
			Get
				Return 3
			End Get
		End Property

		''' <summary>
		''' Implementation of the PropertyChangeListener
		''' that the JSplitPane UI uses.
		''' <p>
		''' This class should be treated as a &quot;protected&quot; inner class.
		''' Instantiate it only within subclasses of BasicSplitPaneUI.
		''' </summary>
		Public Class PropertyHandler
			Implements PropertyChangeListener

			Private ReadOnly outerInstance As BasicSplitPaneUI

			Public Sub New(ByVal outerInstance As BasicSplitPaneUI)
				Me.outerInstance = outerInstance
			End Sub

			' NOTE: This class exists only for backward compatibility. All
			' its functionality has been moved into Handler. If you need to add
			' new functionality add it to the Handler, but make sure this
			' class calls into the Handler.

			''' <summary>
			''' Messaged from the <code>JSplitPane</code> the receiver is
			''' contained in.  May potentially reset the layout manager and cause a
			''' <code>validate</code> to be sent.
			''' </summary>
			Public Overridable Sub propertyChange(ByVal e As PropertyChangeEvent)
				outerInstance.handler.propertyChange(e)
			End Sub
		End Class


		''' <summary>
		''' Implementation of the FocusListener that the JSplitPane UI uses.
		''' <p>
		''' This class should be treated as a &quot;protected&quot; inner class.
		''' Instantiate it only within subclasses of BasicSplitPaneUI.
		''' </summary>
		Public Class FocusHandler
			Inherits FocusAdapter

			Private ReadOnly outerInstance As BasicSplitPaneUI

			Public Sub New(ByVal outerInstance As BasicSplitPaneUI)
				Me.outerInstance = outerInstance
			End Sub

			' NOTE: This class exists only for backward compatibility. All
			' its functionality has been moved into Handler. If you need to add
			' new functionality add it to the Handler, but make sure this
			' class calls into the Handler.
			Public Overridable Sub focusGained(ByVal ev As FocusEvent)
				outerInstance.handler.focusGained(ev)
			End Sub

			Public Overridable Sub focusLost(ByVal ev As FocusEvent)
				outerInstance.handler.focusLost(ev)
			End Sub
		End Class


		''' <summary>
		''' Implementation of an ActionListener that the JSplitPane UI uses for
		''' handling specific key presses.
		''' <p>
		''' This class should be treated as a &quot;protected&quot; inner class.
		''' Instantiate it only within subclasses of BasicSplitPaneUI.
		''' </summary>
		Public Class KeyboardUpLeftHandler
			Implements ActionListener

			Private ReadOnly outerInstance As BasicSplitPaneUI

			Public Sub New(ByVal outerInstance As BasicSplitPaneUI)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Sub actionPerformed(ByVal ev As ActionEvent)
				If outerInstance.dividerKeyboardResize Then outerInstance.splitPane.dividerLocation = Math.Max(0,outerInstance.getDividerLocation(outerInstance.splitPane) - outerInstance.keyboardMoveIncrement)
			End Sub
		End Class

		''' <summary>
		''' Implementation of an ActionListener that the JSplitPane UI uses for
		''' handling specific key presses.
		''' <p>
		''' This class should be treated as a &quot;protected&quot; inner class.
		''' Instantiate it only within subclasses of BasicSplitPaneUI.
		''' </summary>
		Public Class KeyboardDownRightHandler
			Implements ActionListener

			Private ReadOnly outerInstance As BasicSplitPaneUI

			Public Sub New(ByVal outerInstance As BasicSplitPaneUI)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Sub actionPerformed(ByVal ev As ActionEvent)
				If outerInstance.dividerKeyboardResize Then outerInstance.splitPane.dividerLocation = outerInstance.getDividerLocation(outerInstance.splitPane) + outerInstance.keyboardMoveIncrement
			End Sub
		End Class


		''' <summary>
		''' Implementation of an ActionListener that the JSplitPane UI uses for
		''' handling specific key presses.
		''' <p>
		''' This class should be treated as a &quot;protected&quot; inner class.
		''' Instantiate it only within subclasses of BasicSplitPaneUI.
		''' </summary>
		Public Class KeyboardHomeHandler
			Implements ActionListener

			Private ReadOnly outerInstance As BasicSplitPaneUI

			Public Sub New(ByVal outerInstance As BasicSplitPaneUI)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Sub actionPerformed(ByVal ev As ActionEvent)
				If outerInstance.dividerKeyboardResize Then outerInstance.splitPane.dividerLocation = 0
			End Sub
		End Class


		''' <summary>
		''' Implementation of an ActionListener that the JSplitPane UI uses for
		''' handling specific key presses.
		''' <p>
		''' This class should be treated as a &quot;protected&quot; inner class.
		''' Instantiate it only within subclasses of BasicSplitPaneUI.
		''' </summary>
		Public Class KeyboardEndHandler
			Implements ActionListener

			Private ReadOnly outerInstance As BasicSplitPaneUI

			Public Sub New(ByVal outerInstance As BasicSplitPaneUI)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Sub actionPerformed(ByVal ev As ActionEvent)
				If outerInstance.dividerKeyboardResize Then
					Dim insets As Insets = outerInstance.splitPane.insets
					Dim bottomI As Integer = If(insets IsNot Nothing, insets.bottom, 0)
					Dim rightI As Integer = If(insets IsNot Nothing, insets.right, 0)

					If outerInstance.orientation = JSplitPane.VERTICAL_SPLIT Then
						outerInstance.splitPane.dividerLocation = outerInstance.splitPane.height - bottomI
					Else
						outerInstance.splitPane.dividerLocation = outerInstance.splitPane.width - rightI
					End If
				End If
			End Sub
		End Class


		''' <summary>
		''' Implementation of an ActionListener that the JSplitPane UI uses for
		''' handling specific key presses.
		''' <p>
		''' This class should be treated as a &quot;protected&quot; inner class.
		''' Instantiate it only within subclasses of BasicSplitPaneUI.
		''' </summary>
		Public Class KeyboardResizeToggleHandler
			Implements ActionListener

			Private ReadOnly outerInstance As BasicSplitPaneUI

			Public Sub New(ByVal outerInstance As BasicSplitPaneUI)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Sub actionPerformed(ByVal ev As ActionEvent)
				If Not outerInstance.dividerKeyboardResize Then outerInstance.splitPane.requestFocus()
			End Sub
		End Class

		''' <summary>
		''' Returns the divider between the top Components.
		''' </summary>
		Public Overridable Property divider As BasicSplitPaneDivider
			Get
				Return divider
			End Get
		End Property


		''' <summary>
		''' Returns the default non continuous layout divider, which is an
		''' instance of {@code Canvas} that fills in the background with dark gray.
		''' </summary>
		Protected Friend Overridable Function createDefaultNonContinuousLayoutDivider() As Component
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			Return New Canvas()
	'		{
	'			public void paint(Graphics g)
	'			{
	'				if(!isContinuousLayout() && getLastDragLocation() != -1)
	'				{
	'					Dimension size = splitPane.getSize();
	'
	'					g.setColor(dividerDraggingColor);
	'					if(orientation == JSplitPane.HORIZONTAL_SPLIT)
	'					{
	'						g.fillRect(0, 0, dividerSize - 1, size.height - 1);
	'					}
	'					else
	'					{
	'						g.fillRect(0, 0, size.width - 1, dividerSize - 1);
	'					}
	'				}
	'			}
	'		};
		End Function


		''' <summary>
		''' Sets the divider to use when the splitPane is configured to
		''' not continuously layout. This divider will only be used during a
		''' dragging session. It is recommended that the passed in component
		''' be a heavy weight.
		''' </summary>
		Protected Friend Overridable Property nonContinuousLayoutDivider As Component
			Set(ByVal newDivider As Component)
				nonContinuousLayoutDividerder(newDivider, True)
			End Set
			Get
				Return nonContinuousLayoutDivider
			End Get
		End Property


		''' <summary>
		''' Sets the divider to use.
		''' </summary>
		Protected Friend Overridable Sub setNonContinuousLayoutDivider(ByVal newDivider As Component, ByVal rememberSizes As Boolean)
			rememberPaneSizes = rememberSizes
			If nonContinuousLayoutDivider IsNot Nothing AndAlso splitPane IsNot Nothing Then splitPane.remove(nonContinuousLayoutDivider)
			nonContinuousLayoutDivider = newDivider
		End Sub

		Private Sub addHeavyweightDivider()
			If nonContinuousLayoutDivider IsNot Nothing AndAlso splitPane IsNot Nothing Then

				' Needs to remove all the components and re-add them! YECK! 
				' This is all done so that the nonContinuousLayoutDivider will
				' be drawn on top of the other components, without this, one
				' of the heavyweights will draw over the divider!
				Dim leftC As Component = splitPane.leftComponent
				Dim rightC As Component = splitPane.rightComponent
				Dim lastLocation As Integer = splitPane.dividerLocation

				If leftC IsNot Nothing Then splitPane.leftComponent = Nothing
				If rightC IsNot Nothing Then splitPane.rightComponent = Nothing
				splitPane.remove(divider)
				splitPane.add(nonContinuousLayoutDivider, BasicSplitPaneUI.NON_CONTINUOUS_DIVIDER, splitPane.componentCount)
				splitPane.leftComponent = leftC
				splitPane.rightComponent = rightC
				splitPane.add(divider, JSplitPane.DIVIDER)
				If rememberPaneSizes Then splitPane.dividerLocation = lastLocation
			End If

		End Sub




		''' <summary>
		''' Returns the splitpane this instance is currently contained
		''' in.
		''' </summary>
		Public Overridable Property splitPane As JSplitPane
			Get
				Return splitPane
			End Get
		End Property


		''' <summary>
		''' Creates the default divider.
		''' </summary>
		Public Overridable Function createDefaultDivider() As BasicSplitPaneDivider
			Return New BasicSplitPaneDivider(Me)
		End Function


		''' <summary>
		''' Messaged to reset the preferred sizes.
		''' </summary>
		Public Overridable Sub resetToPreferredSizes(ByVal jc As JSplitPane)
			If splitPane IsNot Nothing Then
				layoutManager.resetToPreferredSizes()
				splitPane.revalidate()
				splitPane.repaint()
			End If
		End Sub


		''' <summary>
		''' Sets the location of the divider to location.
		''' </summary>
		Public Overridable Sub setDividerLocation(ByVal jc As JSplitPane, ByVal location As Integer)
			If Not ignoreDividerLocationChange Then
				dividerLocationIsSet = True
				splitPane.revalidate()
				splitPane.repaint()

				If keepHidden Then
					Dim ___insets As Insets = splitPane.insets
					Dim ___orientation As Integer = splitPane.orientation
					If (___orientation = JSplitPane.VERTICAL_SPLIT AndAlso location <> ___insets.top AndAlso location <> splitPane.height-divider.height-___insets.top) OrElse (___orientation = JSplitPane.HORIZONTAL_SPLIT AndAlso location <> ___insets.left AndAlso location <> splitPane.width-divider.width-___insets.left) Then keepHidden = False
				End If
			Else
				ignoreDividerLocationChange = False
			End If
		End Sub


		''' <summary>
		''' Returns the location of the divider, which may differ from what
		''' the splitpane thinks the location of the divider is.
		''' </summary>
		Public Overridable Function getDividerLocation(ByVal jc As JSplitPane) As Integer
			If orientation = JSplitPane.HORIZONTAL_SPLIT Then Return divider.location.x
			Return divider.location.y
		End Function


		''' <summary>
		''' Gets the minimum location of the divider.
		''' </summary>
		Public Overridable Function getMinimumDividerLocation(ByVal jc As JSplitPane) As Integer
			Dim minLoc As Integer = 0
			Dim leftC As Component = splitPane.leftComponent

			If (leftC IsNot Nothing) AndAlso (leftC.visible) Then
				Dim ___insets As Insets = splitPane.insets
				Dim minSize As Dimension = leftC.minimumSize
				If orientation = JSplitPane.HORIZONTAL_SPLIT Then
					minLoc = minSize.width
				Else
					minLoc = minSize.height
				End If
				If ___insets IsNot Nothing Then
					If orientation = JSplitPane.HORIZONTAL_SPLIT Then
						minLoc += ___insets.left
					Else
						minLoc += ___insets.top
					End If
				End If
			End If
			Return minLoc
		End Function


		''' <summary>
		''' Gets the maximum location of the divider.
		''' </summary>
		Public Overridable Function getMaximumDividerLocation(ByVal jc As JSplitPane) As Integer
			Dim splitPaneSize As Dimension = splitPane.size
			Dim maxLoc As Integer = 0
			Dim rightC As Component = splitPane.rightComponent

			If rightC IsNot Nothing Then
				Dim ___insets As Insets = splitPane.insets
				Dim minSize As New Dimension(0, 0)
				If rightC.visible Then minSize = rightC.minimumSize
				If orientation = JSplitPane.HORIZONTAL_SPLIT Then
					maxLoc = splitPaneSize.width - minSize.width
				Else
					maxLoc = splitPaneSize.height - minSize.height
				End If
				maxLoc -= dividerSize
				If ___insets IsNot Nothing Then
					If orientation = JSplitPane.HORIZONTAL_SPLIT Then
						maxLoc -= ___insets.right
					Else
						maxLoc -= ___insets.top
					End If
				End If
			End If
			Return Math.Max(getMinimumDividerLocation(splitPane), maxLoc)
		End Function


		''' <summary>
		''' Called when the specified split pane has finished painting
		''' its children.
		''' </summary>
		Public Overridable Sub finishedPaintingChildren(ByVal sp As JSplitPane, ByVal g As Graphics)
			If sp Is splitPane AndAlso lastDragLocation <> -1 AndAlso (Not continuousLayout) AndAlso (Not draggingHW) Then
				Dim size As Dimension = splitPane.size

				g.color = dividerDraggingColor
				If orientation = JSplitPane.HORIZONTAL_SPLIT Then
					g.fillRect(lastDragLocation, 0, dividerSize - 1, size.height - 1)
				Else
					g.fillRect(0, lastDragLocation, size.width - 1, dividerSize - 1)
				End If
			End If
		End Sub


		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overridable Sub paint(ByVal g As Graphics, ByVal jc As JComponent)
			If (Not painted) AndAlso splitPane.dividerLocation<0 Then
				ignoreDividerLocationChange = True
				splitPane.dividerLocation = getDividerLocation(splitPane)
			End If
			painted = True
		End Sub


		''' <summary>
		''' Returns the preferred size for the passed in component,
		''' This is passed off to the current layout manager.
		''' </summary>
		Public Overridable Function getPreferredSize(ByVal jc As JComponent) As Dimension
			If splitPane IsNot Nothing Then Return layoutManager.preferredLayoutSize(splitPane)
			Return New Dimension(0, 0)
		End Function


		''' <summary>
		''' Returns the minimum size for the passed in component,
		''' This is passed off to the current layout manager.
		''' </summary>
		Public Overridable Function getMinimumSize(ByVal jc As JComponent) As Dimension
			If splitPane IsNot Nothing Then Return layoutManager.minimumLayoutSize(splitPane)
			Return New Dimension(0, 0)
		End Function


		''' <summary>
		''' Returns the maximum size for the passed in component,
		''' This is passed off to the current layout manager.
		''' </summary>
		Public Overridable Function getMaximumSize(ByVal jc As JComponent) As Dimension
			If splitPane IsNot Nothing Then Return layoutManager.maximumLayoutSize(splitPane)
			Return New Dimension(0, 0)
		End Function


		''' <summary>
		''' Returns the insets. The insets are returned from the border insets
		''' of the current border.
		''' </summary>
		Public Overridable Function getInsets(ByVal jc As JComponent) As Insets
			Return Nothing
		End Function


		''' <summary>
		''' Resets the layout manager based on orientation and messages it
		''' with invalidateLayout to pull in appropriate Components.
		''' </summary>
		Protected Friend Overridable Sub resetLayoutManager()
			If orientation = JSplitPane.HORIZONTAL_SPLIT Then
				layoutManager = New BasicHorizontalLayoutManager(Me, 0)
			Else
				layoutManager = New BasicHorizontalLayoutManager(Me, 1)
			End If
			splitPane.layout = layoutManager
			layoutManager.updateComponents()
			splitPane.revalidate()
			splitPane.repaint()
		End Sub

		''' <summary>
		''' Set the value to indicate if one of the splitpane sides is expanded.
		''' </summary>
		Friend Overridable Property keepHidden As Boolean
			Set(ByVal keepHidden As Boolean)
				Me.keepHidden = keepHidden
			End Set
			Get
				Return keepHidden
			End Get
		End Property


		''' <summary>
		''' Should be messaged before the dragging session starts, resets
		''' lastDragLocation and dividerSize.
		''' </summary>
		Protected Friend Overridable Sub startDragging()
			Dim leftC As Component = splitPane.leftComponent
			Dim rightC As Component = splitPane.rightComponent
			Dim cPeer As java.awt.peer.ComponentPeer

			beginDragDividerLocation = getDividerLocation(splitPane)
			draggingHW = False
			cPeer = leftC.peer
			If leftC IsNot Nothing AndAlso cPeer IsNot Nothing AndAlso Not(TypeOf cPeer Is java.awt.peer.LightweightPeer) Then
				draggingHW = True
			Else
				cPeer = rightC.peer
				If rightC IsNot Nothing AndAlso cPeer IsNot Nothing AndAlso Not(TypeOf cPeer Is java.awt.peer.LightweightPeer) Then draggingHW = True
				End If
			If orientation = JSplitPane.HORIZONTAL_SPLIT Then
				lastDragLocation = divider.bounds.x
				dividerSize = divider.size.width
				If (Not continuousLayout) AndAlso draggingHW Then
					nonContinuousLayoutDivider.boundsnds(lastDragLocation, 0, dividerSize, splitPane.height)
						  addHeavyweightDivider()
				End If
			Else
				lastDragLocation = divider.bounds.y
				dividerSize = divider.size.height
				If (Not continuousLayout) AndAlso draggingHW Then
					nonContinuousLayoutDivider.boundsnds(0, lastDragLocation, splitPane.width, dividerSize)
						  addHeavyweightDivider()
				End If
			End If
		End Sub


		''' <summary>
		''' Messaged during a dragging session to move the divider to the
		''' passed in location. If continuousLayout is true the location is
		''' reset and the splitPane validated.
		''' </summary>
		Protected Friend Overridable Sub dragDividerTo(ByVal location As Integer)
			If lastDragLocation <> location Then
				If continuousLayout Then
					splitPane.dividerLocation = location
					lastDragLocation = location
				Else
					Dim lastLoc As Integer = lastDragLocation

					lastDragLocation = location
					If orientation = JSplitPane.HORIZONTAL_SPLIT Then
						If draggingHW Then
							nonContinuousLayoutDivider.locationion(lastDragLocation, 0)
						Else
							Dim splitHeight As Integer = splitPane.height
							splitPane.repaint(lastLoc, 0, dividerSize, splitHeight)
							splitPane.repaint(location, 0, dividerSize, splitHeight)
						End If
					Else
						If draggingHW Then
							nonContinuousLayoutDivider.locationion(0, lastDragLocation)
						Else
							Dim splitWidth As Integer = splitPane.width

							splitPane.repaint(0, lastLoc, splitWidth, dividerSize)
							splitPane.repaint(0, location, splitWidth, dividerSize)
						End If
					End If
				End If
			End If
		End Sub


		''' <summary>
		''' Messaged to finish the dragging session. If not continuous display
		''' the dividers location will be reset.
		''' </summary>
		Protected Friend Overridable Sub finishDraggingTo(ByVal location As Integer)
			dragDividerTo(location)
			lastDragLocation = -1
			If Not continuousLayout Then
				Dim leftC As Component = splitPane.leftComponent
				Dim leftBounds As Rectangle = leftC.bounds

				If draggingHW Then
					If orientation = JSplitPane.HORIZONTAL_SPLIT Then
						nonContinuousLayoutDivider.locationion(-dividerSize, 0)
					Else
						nonContinuousLayoutDivider.locationion(0, -dividerSize)
					End If
					splitPane.remove(nonContinuousLayoutDivider)
				End If
				splitPane.dividerLocation = location
			End If
		End Sub


		''' <summary>
		''' As of Java 2 platform v1.3 this method is no longer used. Instead
		''' you should set the border on the divider.
		''' <p>
		''' Returns the width of one side of the divider border.
		''' </summary>
		''' @deprecated As of Java 2 platform v1.3, instead set the border on the
		''' divider. 
		<Obsolete("As of Java 2 platform v1.3, instead set the border on the")> _
		Protected Friend Overridable Property dividerBorderSize As Integer
			Get
				Return 1
			End Get
		End Property


		''' <summary>
		''' LayoutManager for JSplitPanes that have an orientation of
		''' HORIZONTAL_SPLIT.
		''' </summary>
		Public Class BasicHorizontalLayoutManager
			Implements LayoutManager2

			Private ReadOnly outerInstance As BasicSplitPaneUI

			' left, right, divider. (in this exact order) 
			Protected Friend sizes As Integer()
			Protected Friend components As Component()
			''' <summary>
			''' Size of the splitpane the last time laid out. </summary>
			Private lastSplitPaneSize As Integer
			''' <summary>
			''' True if resetToPreferredSizes has been invoked. </summary>
			Private doReset As Boolean
			''' <summary>
			''' Axis, 0 for horizontal, or 1 for veritcal. </summary>
			Private axis As Integer


			Friend Sub New(ByVal outerInstance As BasicSplitPaneUI)
					Me.outerInstance = outerInstance
				Me.New(0)
			End Sub

			Friend Sub New(ByVal outerInstance As BasicSplitPaneUI, ByVal axis As Integer)
					Me.outerInstance = outerInstance
				Me.axis = axis
				components = New Component(2){}
					components(2) = Nothing
						components(1) = components(2)
						components(0) = components(1)
				sizes = New Integer(2){}
			End Sub

			'
			' LayoutManager
			'

			''' <summary>
			''' Does the actual layout.
			''' </summary>
			Public Overridable Sub layoutContainer(ByVal container As Container)
				Dim containerSize As Dimension = container.size

				' If the splitpane has a zero size then no op out of here.
				' If we execute this function now, we're going to cause ourselves
				' much grief.
				If containerSize.height <= 0 OrElse containerSize.width <= 0 Then
					lastSplitPaneSize = 0
					Return
				End If

				Dim spDividerLocation As Integer = outerInstance.splitPane.dividerLocation
				Dim insets As Insets = outerInstance.splitPane.insets
				Dim ___availableSize As Integer = getAvailableSize(containerSize, insets)
				Dim newSize As Integer = getSizeForPrimaryAxis(containerSize)
				Dim beginLocation As Integer = outerInstance.getDividerLocation(outerInstance.splitPane)
				Dim dOffset As Integer = getSizeForPrimaryAxis(insets, True)
				Dim dSize As Dimension = If(components(2) Is Nothing, Nothing, components(2).preferredSize)

				If (doReset AndAlso (Not outerInstance.dividerLocationIsSet)) OrElse spDividerLocation < 0 Then
					resetToPreferredSizes(___availableSize)
				ElseIf lastSplitPaneSize <= 0 OrElse ___availableSize = lastSplitPaneSize OrElse (Not outerInstance.painted) OrElse (dSize IsNot Nothing AndAlso getSizeForPrimaryAxis(dSize) <> sizes(2)) Then
					If dSize IsNot Nothing Then
						sizes(2) = getSizeForPrimaryAxis(dSize)
					Else
						sizes(2) = 0
					End If
					dividerLocationion(spDividerLocation - dOffset, ___availableSize)
					outerInstance.dividerLocationIsSet = False
				ElseIf ___availableSize <> lastSplitPaneSize Then
					distributeSpace(___availableSize - lastSplitPaneSize, outerInstance.keepHidden)
				End If
				doReset = False
				outerInstance.dividerLocationIsSet = False
				lastSplitPaneSize = ___availableSize

				' Reset the bounds of each component
				Dim nextLocation As Integer = getInitialLocation(insets)
				Dim counter As Integer = 0

				Do While counter < 3
					If components(counter) IsNot Nothing AndAlso components(counter).visible Then
						componentToSizeize(components(counter), sizes(counter), nextLocation, insets, containerSize)
						nextLocation += sizes(counter)
					End If
					Select Case counter
					Case 0
						counter = 2
					Case 2
						counter = 1
					Case 1
						counter = 3
					End Select
				Loop
				If outerInstance.painted Then
					' This is tricky, there is never a good time for us
					' to push the value to the splitpane, painted appears to
					' the best time to do it. What is really needed is
					' notification that layout has completed.
					Dim newLocation As Integer = outerInstance.getDividerLocation(outerInstance.splitPane)

					If newLocation <> (spDividerLocation - dOffset) Then
						Dim lastLocation As Integer = outerInstance.splitPane.lastDividerLocation

						outerInstance.ignoreDividerLocationChange = True
						Try
							outerInstance.splitPane.dividerLocation = newLocation
							' This is not always needed, but is rather tricky
							' to determine when... The case this is needed for
							' is if the user sets the divider location to some
							' bogus value, say 0, and the actual value is 1, the
							' call to setDividerLocation(1) will preserve the
							' old value of 0, when we really want the divider
							' location value  before the call. This is needed for
							' the one touch buttons.
							outerInstance.splitPane.lastDividerLocation = lastLocation
						Finally
							outerInstance.ignoreDividerLocationChange = False
						End Try
					End If
				End If
			End Sub


			''' <summary>
			''' Adds the component at place.  Place must be one of
			''' JSplitPane.LEFT, RIGHT, TOP, BOTTOM, or null (for the
			''' divider).
			''' </summary>
			Public Overridable Sub addLayoutComponent(ByVal place As String, ByVal component As Component)
				Dim isValid As Boolean = True

				If place IsNot Nothing Then
					If place.Equals(JSplitPane.DIVIDER) Then
						' Divider. 
						components(2) = component
						sizes(2) = getSizeForPrimaryAxis(component.preferredSize)
					ElseIf place.Equals(JSplitPane.LEFT) OrElse place.Equals(JSplitPane.TOP) Then
						components(0) = component
						sizes(0) = 0
					ElseIf place.Equals(JSplitPane.RIGHT) OrElse place.Equals(JSplitPane.BOTTOM) Then
						components(1) = component
						sizes(1) = 0
					ElseIf Not place.Equals(BasicSplitPaneUI.NON_CONTINUOUS_DIVIDER) Then
						isValid = False
					End If
				Else
					isValid = False
				End If
				If Not isValid Then Throw New System.ArgumentException("cannot add to layout: " & "unknown constraint: " & place)
				doReset = True
			End Sub


			''' <summary>
			''' Returns the minimum size needed to contain the children.
			''' The width is the sum of all the children's min widths and
			''' the height is the largest of the children's minimum heights.
			''' </summary>
			Public Overridable Function minimumLayoutSize(ByVal container As Container) As Dimension
				Dim minPrimary As Integer = 0
				Dim minSecondary As Integer = 0
				Dim insets As Insets = outerInstance.splitPane.insets

				For counter As Integer = 0 To 2
					If components(counter) IsNot Nothing Then
						Dim minSize As Dimension = components(counter).minimumSize
						Dim secSize As Integer = getSizeForSecondaryAxis(minSize)

						minPrimary += getSizeForPrimaryAxis(minSize)
						If secSize > minSecondary Then minSecondary = secSize
					End If
				Next counter
				If insets IsNot Nothing Then
					minPrimary += getSizeForPrimaryAxis(insets, True) + getSizeForPrimaryAxis(insets, False)
					minSecondary += getSizeForSecondaryAxis(insets, True) + getSizeForSecondaryAxis(insets, False)
				End If
				If axis = 0 Then Return New Dimension(minPrimary, minSecondary)
				Return New Dimension(minSecondary, minPrimary)
			End Function


			''' <summary>
			''' Returns the preferred size needed to contain the children.
			''' The width is the sum of all the preferred widths of the children and
			''' the height is the largest preferred height of the children.
			''' </summary>
			Public Overridable Function preferredLayoutSize(ByVal container As Container) As Dimension
				Dim prePrimary As Integer = 0
				Dim preSecondary As Integer = 0
				Dim insets As Insets = outerInstance.splitPane.insets

				For counter As Integer = 0 To 2
					If components(counter) IsNot Nothing Then
						Dim preSize As Dimension = components(counter).preferredSize
						Dim secSize As Integer = getSizeForSecondaryAxis(preSize)

						prePrimary += getSizeForPrimaryAxis(preSize)
						If secSize > preSecondary Then preSecondary = secSize
					End If
				Next counter
				If insets IsNot Nothing Then
					prePrimary += getSizeForPrimaryAxis(insets, True) + getSizeForPrimaryAxis(insets, False)
					preSecondary += getSizeForSecondaryAxis(insets, True) + getSizeForSecondaryAxis(insets, False)
				End If
				If axis = 0 Then Return New Dimension(prePrimary, preSecondary)
				Return New Dimension(preSecondary, prePrimary)
			End Function


			''' <summary>
			''' Removes the specified component from our knowledge.
			''' </summary>
			Public Overridable Sub removeLayoutComponent(ByVal component As Component)
				For counter As Integer = 0 To 2
					If components(counter) Is component Then
						components(counter) = Nothing
						sizes(counter) = 0
						doReset = True
					End If
				Next counter
			End Sub


			'
			' LayoutManager2
			'


			''' <summary>
			''' Adds the specified component to the layout, using the specified
			''' constraint object. </summary>
			''' <param name="comp"> the component to be added </param>
			''' <param name="constraints">  where/how the component is added to the layout. </param>
			Public Overridable Sub addLayoutComponent(ByVal comp As Component, ByVal constraints As Object)
				If (constraints Is Nothing) OrElse (TypeOf constraints Is String) Then
					addLayoutComponent(CStr(constraints), comp)
				Else
					Throw New System.ArgumentException("cannot add to layout: " & "constraint must be a " & "string (or null)")
				End If
			End Sub


			''' <summary>
			''' Returns the alignment along the x axis.  This specifies how
			''' the component would like to be aligned relative to other
			''' components.  The value should be a number between 0 and 1
			''' where 0 represents alignment along the origin, 1 is aligned
			''' the furthest away from the origin, 0.5 is centered, etc.
			''' </summary>
			Public Overridable Function getLayoutAlignmentX(ByVal target As Container) As Single
				Return 0.0f
			End Function


			''' <summary>
			''' Returns the alignment along the y axis.  This specifies how
			''' the component would like to be aligned relative to other
			''' components.  The value should be a number between 0 and 1
			''' where 0 represents alignment along the origin, 1 is aligned
			''' the furthest away from the origin, 0.5 is centered, etc.
			''' </summary>
			Public Overridable Function getLayoutAlignmentY(ByVal target As Container) As Single
				Return 0.0f
			End Function


			''' <summary>
			''' Does nothing. If the developer really wants to change the
			''' size of one of the views JSplitPane.resetToPreferredSizes should
			''' be messaged.
			''' </summary>
			Public Overridable Sub invalidateLayout(ByVal c As Container)
			End Sub


			''' <summary>
			''' Returns the maximum layout size, which is Integer.MAX_VALUE
			''' in both directions.
			''' </summary>
			Public Overridable Function maximumLayoutSize(ByVal target As Container) As Dimension
				Return New Dimension(Integer.MaxValue, Integer.MaxValue)
			End Function


			'
			' New methods.
			'

			''' <summary>
			''' Marks the receiver so that the next time this instance is
			''' laid out it'll ask for the preferred sizes.
			''' </summary>
			Public Overridable Sub resetToPreferredSizes()
				doReset = True
			End Sub

			''' <summary>
			''' Resets the size of the Component at the passed in location.
			''' </summary>
			Protected Friend Overridable Sub resetSizeAt(ByVal index As Integer)
				sizes(index) = 0
				doReset = True
			End Sub


			''' <summary>
			''' Sets the sizes to <code>newSizes</code>.
			''' </summary>
			Protected Friend Overridable Property sizes As Integer()
				Set(ByVal newSizes As Integer())
					Array.Copy(newSizes, 0, sizes, 0, 3)
				End Set
				Get
					Dim retSizes As Integer() = New Integer(2){}
    
					Array.Copy(sizes, 0, retSizes, 0, 3)
					Return retSizes
				End Get
			End Property




			''' <summary>
			''' Returns the width of the passed in Components preferred size.
			''' </summary>
			Protected Friend Overridable Function getPreferredSizeOfComponent(ByVal c As Component) As Integer
				Return getSizeForPrimaryAxis(c.preferredSize)
			End Function


			''' <summary>
			''' Returns the width of the passed in Components minimum size.
			''' </summary>
			Friend Overridable Function getMinimumSizeOfComponent(ByVal c As Component) As Integer
				Return getSizeForPrimaryAxis(c.minimumSize)
			End Function


			''' <summary>
			''' Returns the width of the passed in component.
			''' </summary>
			Protected Friend Overridable Function getSizeOfComponent(ByVal c As Component) As Integer
				Return getSizeForPrimaryAxis(c.size)
			End Function


			''' <summary>
			''' Returns the available width based on the container size and
			''' Insets.
			''' </summary>
			Protected Friend Overridable Function getAvailableSize(ByVal containerSize As Dimension, ByVal insets As Insets) As Integer
				If insets Is Nothing Then Return getSizeForPrimaryAxis(containerSize)
				Return (getSizeForPrimaryAxis(containerSize) - (getSizeForPrimaryAxis(insets, True) + getSizeForPrimaryAxis(insets, False)))
			End Function


			''' <summary>
			''' Returns the left inset, unless the Insets are null in which case
			''' 0 is returned.
			''' </summary>
			Protected Friend Overridable Function getInitialLocation(ByVal insets As Insets) As Integer
				If insets IsNot Nothing Then Return getSizeForPrimaryAxis(insets, True)
				Return 0
			End Function


			''' <summary>
			''' Sets the width of the component c to be size, placing its
			''' x location at location, y to the insets.top and height
			''' to the containersize.height less the top and bottom insets.
			''' </summary>
			Protected Friend Overridable Sub setComponentToSize(ByVal c As Component, ByVal size As Integer, ByVal location As Integer, ByVal insets As Insets, ByVal containerSize As Dimension)
				If insets IsNot Nothing Then
					If axis = 0 Then
						c.boundsnds(location, insets.top, size, containerSize.height - (insets.top + insets.bottom))
					Else
						c.boundsnds(insets.left, location, containerSize.width - (insets.left + insets.right), size)
					End If
				Else
					If axis = 0 Then
						c.boundsnds(location, 0, size, containerSize.height)
					Else
						c.boundsnds(0, location, containerSize.width, size)
					End If
				End If
			End Sub

			''' <summary>
			''' If the axis == 0, the width is returned, otherwise the height.
			''' </summary>
			Friend Overridable Function getSizeForPrimaryAxis(ByVal size As Dimension) As Integer
				If axis = 0 Then Return size.width
				Return size.height
			End Function

			''' <summary>
			''' If the axis == 0, the width is returned, otherwise the height.
			''' </summary>
			Friend Overridable Function getSizeForSecondaryAxis(ByVal size As Dimension) As Integer
				If axis = 0 Then Return size.height
				Return size.width
			End Function

			''' <summary>
			''' Returns a particular value of the inset identified by the
			''' axis and <code>isTop</code><p>
			'''   axis isTop
			'''    0    true    - left
			'''    0    false   - right
			'''    1    true    - top
			'''    1    false   - bottom
			''' </summary>
			Friend Overridable Function getSizeForPrimaryAxis(ByVal insets As Insets, ByVal isTop As Boolean) As Integer
				If axis = 0 Then
					If isTop Then Return insets.left
					Return insets.right
				End If
				If isTop Then Return insets.top
				Return insets.bottom
			End Function

			''' <summary>
			''' Returns a particular value of the inset identified by the
			''' axis and <code>isTop</code><p>
			'''   axis isTop
			'''    0    true    - left
			'''    0    false   - right
			'''    1    true    - top
			'''    1    false   - bottom
			''' </summary>
			Friend Overridable Function getSizeForSecondaryAxis(ByVal insets As Insets, ByVal isTop As Boolean) As Integer
				If axis = 0 Then
					If isTop Then Return insets.top
					Return insets.bottom
				End If
				If isTop Then Return insets.left
				Return insets.right
			End Function

			''' <summary>
			''' Determines the components. This should be called whenever
			''' a new instance of this is installed into an existing
			''' SplitPane.
			''' </summary>
			Protected Friend Overridable Sub updateComponents()
				Dim comp As Component

				comp = outerInstance.splitPane.leftComponent
				If components(0) IsNot comp Then
					components(0) = comp
					If comp Is Nothing Then
						sizes(0) = 0
					Else
						sizes(0) = -1
					End If
				End If

				comp = outerInstance.splitPane.rightComponent
				If components(1) IsNot comp Then
					components(1) = comp
					If comp Is Nothing Then
						sizes(1) = 0
					Else
						sizes(1) = -1
					End If
				End If

				' Find the divider. 
				Dim children As Component() = outerInstance.splitPane.components
				Dim oldDivider As Component = components(2)

				components(2) = Nothing
				For counter As Integer = children.Length - 1 To 0 Step -1
					If children(counter) IsNot components(0) AndAlso children(counter) IsNot components(1) AndAlso children(counter) IsNot outerInstance.nonContinuousLayoutDivider Then
						If oldDivider IsNot children(counter) Then
							components(2) = children(counter)
						Else
							components(2) = oldDivider
						End If
						Exit For
					End If
				Next counter
				If components(2) Is Nothing Then
					sizes(2) = 0
				Else
					sizes(2) = getSizeForPrimaryAxis(components(2).preferredSize)
				End If
			End Sub

			''' <summary>
			''' Resets the size of the first component to <code>leftSize</code>,
			''' and the right component to the remainder of the space.
			''' </summary>
			Friend Overridable Sub setDividerLocation(ByVal leftSize As Integer, ByVal availableSize As Integer)
				Dim lValid As Boolean = (components(0) IsNot Nothing AndAlso components(0).visible)
				Dim rValid As Boolean = (components(1) IsNot Nothing AndAlso components(1).visible)
				Dim dValid As Boolean = (components(2) IsNot Nothing AndAlso components(2).visible)
				Dim max As Integer = availableSize

				If dValid Then max -= sizes(2)
				leftSize = Math.Max(0, Math.Min(leftSize, max))
				If lValid Then
					If rValid Then
						sizes(0) = leftSize
						sizes(1) = max - leftSize
					Else
						sizes(0) = max
						sizes(1) = 0
					End If
				ElseIf rValid Then
					sizes(1) = max
					sizes(0) = 0
				End If
			End Sub

			''' <summary>
			''' Returns an array of the minimum sizes of the components.
			''' </summary>
			Friend Overridable Property preferredSizes As Integer()
				Get
					Dim retValue As Integer() = New Integer(2){}
    
					For counter As Integer = 0 To 2
						If components(counter) IsNot Nothing AndAlso components(counter).visible Then
							retValue(counter) = getPreferredSizeOfComponent(components(counter))
						Else
							retValue(counter) = -1
						End If
					Next counter
					Return retValue
				End Get
			End Property

			''' <summary>
			''' Returns an array of the minimum sizes of the components.
			''' </summary>
			Friend Overridable Property minimumSizes As Integer()
				Get
					Dim retValue As Integer() = New Integer(2){}
    
					For counter As Integer = 0 To 1
						If components(counter) IsNot Nothing AndAlso components(counter).visible Then
							retValue(counter) = getMinimumSizeOfComponent(components(counter))
						Else
							retValue(counter) = -1
						End If
					Next counter
					retValue(2) = If(components(2) IsNot Nothing, getMinimumSizeOfComponent(components(2)), -1)
					Return retValue
				End Get
			End Property

			''' <summary>
			''' Resets the components to their preferred sizes.
			''' </summary>
			Friend Overridable Sub resetToPreferredSizes(ByVal availableSize As Integer)
				' Set the sizes to the preferred sizes (if fits), otherwise
				' set to min sizes and distribute any extra space.
				Dim testSizes As Integer() = preferredSizes
				Dim totalSize As Integer = 0

				For counter As Integer = 0 To 2
					If testSizes(counter) <> -1 Then totalSize += testSizes(counter)
				Next counter
				If totalSize > availableSize Then
					testSizes = minimumSizes

					totalSize = 0
					For counter As Integer = 0 To 2
						If testSizes(counter) <> -1 Then totalSize += testSizes(counter)
					Next counter
				End If
				sizes = testSizes
				distributeSpace(availableSize - totalSize, False)
			End Sub

			''' <summary>
			''' Distributes <code>space</code> between the two components
			''' (divider won't get any extra space) based on the weighting. This
			''' attempts to honor the min size of the components.
			''' </summary>
			''' <param name="keepHidden"> if true and one of the components is 0x0
			'''                   it gets none of the extra space </param>
			Friend Overridable Sub distributeSpace(ByVal space As Integer, ByVal keepHidden As Boolean)
				Dim lValid As Boolean = (components(0) IsNot Nothing AndAlso components(0).visible)
				Dim rValid As Boolean = (components(1) IsNot Nothing AndAlso components(1).visible)

				If keepHidden Then
					If lValid AndAlso getSizeForPrimaryAxis(components(0).size) = 0 Then
						lValid = False
						If rValid AndAlso getSizeForPrimaryAxis(components(1).size) = 0 Then lValid = True
					ElseIf rValid AndAlso getSizeForPrimaryAxis(components(1).size) = 0 Then
						rValid = False
					End If
				End If
				If lValid AndAlso rValid Then
					Dim weight As Double = outerInstance.splitPane.resizeWeight
					Dim lExtra As Integer = CInt(Fix(weight * CDbl(space)))
					Dim rExtra As Integer = (space - lExtra)

					sizes(0) += lExtra
					sizes(1) += rExtra

					Dim lMin As Integer = getMinimumSizeOfComponent(components(0))
					Dim rMin As Integer = getMinimumSizeOfComponent(components(1))
					Dim lMinValid As Boolean = (sizes(0) >= lMin)
					Dim rMinValid As Boolean = (sizes(1) >= rMin)

					If (Not lMinValid) AndAlso (Not rMinValid) Then
						If sizes(0) < 0 Then
							sizes(1) += sizes(0)
							sizes(0) = 0
						ElseIf sizes(1) < 0 Then
							sizes(0) += sizes(1)
							sizes(1) = 0
						End If
					ElseIf Not lMinValid Then
						If sizes(1) - (lMin - sizes(0)) < rMin Then
							' both below min, just make sure > 0
							If sizes(0) < 0 Then
								sizes(1) += sizes(0)
								sizes(0) = 0
							End If
						Else
							sizes(1) -= (lMin - sizes(0))
							sizes(0) = lMin
						End If
					ElseIf Not rMinValid Then
						If sizes(0) - (rMin - sizes(1)) < lMin Then
							' both below min, just make sure > 0
							If sizes(1) < 0 Then
								sizes(0) += sizes(1)
								sizes(1) = 0
							End If
						Else
							sizes(0) -= (rMin - sizes(1))
							sizes(1) = rMin
						End If
					End If
					If sizes(0) < 0 Then sizes(0) = 0
					If sizes(1) < 0 Then sizes(1) = 0
				ElseIf lValid Then
					sizes(0) = Math.Max(0, sizes(0) + space)
				ElseIf rValid Then
					sizes(1) = Math.Max(0, sizes(1) + space)
				End If
			End Sub
		End Class


		''' <summary>
		''' LayoutManager used for JSplitPanes with an orientation of
		''' VERTICAL_SPLIT.
		''' 
		''' </summary>
		Public Class BasicVerticalLayoutManager
			Inherits BasicHorizontalLayoutManager

			Private ReadOnly outerInstance As BasicSplitPaneUI

			Public Sub New(ByVal outerInstance As BasicSplitPaneUI)
					Me.outerInstance = outerInstance
				MyBase.New(1)
			End Sub
		End Class


		Private Class Handler
			Implements FocusListener, PropertyChangeListener

			Private ReadOnly outerInstance As BasicSplitPaneUI

			Public Sub New(ByVal outerInstance As BasicSplitPaneUI)
				Me.outerInstance = outerInstance
			End Sub

			'
			' PropertyChangeListener
			'
			''' <summary>
			''' Messaged from the <code>JSplitPane</code> the receiver is
			''' contained in.  May potentially reset the layout manager and cause a
			''' <code>validate</code> to be sent.
			''' </summary>
			Public Overridable Sub propertyChange(ByVal e As PropertyChangeEvent)
				If e.source Is outerInstance.splitPane Then
					Dim changeName As String = e.propertyName

					If changeName = JSplitPane.ORIENTATION_PROPERTY Then
						outerInstance.orientation = outerInstance.splitPane.orientation
						outerInstance.resetLayoutManager()
					ElseIf changeName = JSplitPane.CONTINUOUS_LAYOUT_PROPERTY Then
						outerInstance.continuousLayout = outerInstance.splitPane.continuousLayout
						If Not outerInstance.continuousLayout Then
							If outerInstance.nonContinuousLayoutDivider Is Nothing Then
								outerInstance.nonContinuousLayoutDividerder(outerInstance.createDefaultNonContinuousLayoutDivider(), True)
							ElseIf outerInstance.nonContinuousLayoutDivider.parent Is Nothing Then
								outerInstance.nonContinuousLayoutDividerder(outerInstance.nonContinuousLayoutDivider, True)
							End If
						End If
					ElseIf changeName = JSplitPane.DIVIDER_SIZE_PROPERTY Then
						outerInstance.divider.dividerSize = outerInstance.splitPane.dividerSize
						outerInstance.dividerSize = outerInstance.divider.dividerSize
						outerInstance.splitPane.revalidate()
						outerInstance.splitPane.repaint()
					End If
				End If
			End Sub

			'
			' FocusListener
			'
			Public Overridable Sub focusGained(ByVal ev As FocusEvent)
				outerInstance.dividerKeyboardResize = True
				outerInstance.splitPane.repaint()
			End Sub

			Public Overridable Sub focusLost(ByVal ev As FocusEvent)
				outerInstance.dividerKeyboardResize = False
				outerInstance.splitPane.repaint()
			End Sub
		End Class


		Private Class Actions
			Inherits sun.swing.UIAction

			Private Const NEGATIVE_INCREMENT As String = "negativeIncrement"
			Private Const POSITIVE_INCREMENT As String = "positiveIncrement"
			Private Const SELECT_MIN As String = "selectMin"
			Private Const SELECT_MAX As String = "selectMax"
			Private Const START_RESIZE As String = "startResize"
			Private Const TOGGLE_FOCUS As String = "toggleFocus"
			Private Const FOCUS_OUT_FORWARD As String = "focusOutForward"
			Private Const FOCUS_OUT_BACKWARD As String = "focusOutBackward"

			Friend Sub New(ByVal key As String)
				MyBase.New(key)
			End Sub

			Public Overridable Sub actionPerformed(ByVal ev As ActionEvent)
				Dim splitPane As JSplitPane = CType(ev.source, JSplitPane)
				Dim ui As BasicSplitPaneUI = CType(BasicLookAndFeel.getUIOfType(splitPane.uI, GetType(BasicSplitPaneUI)), BasicSplitPaneUI)

				If ui Is Nothing Then Return
				Dim key As String = name
				If key = NEGATIVE_INCREMENT Then
					If ui.dividerKeyboardResize Then splitPane.dividerLocation = Math.Max(0, ui.getDividerLocation(splitPane) - ui.keyboardMoveIncrement)
				ElseIf key = POSITIVE_INCREMENT Then
					If ui.dividerKeyboardResize Then splitPane.dividerLocation = ui.getDividerLocation(splitPane) + ui.keyboardMoveIncrement
				ElseIf key = SELECT_MIN Then
					If ui.dividerKeyboardResize Then splitPane.dividerLocation = 0
				ElseIf key = SELECT_MAX Then
					If ui.dividerKeyboardResize Then
						Dim insets As Insets = splitPane.insets
						Dim bottomI As Integer = If(insets IsNot Nothing, insets.bottom, 0)
						Dim rightI As Integer = If(insets IsNot Nothing, insets.right, 0)

						If ui.orientation = JSplitPane.VERTICAL_SPLIT Then
							splitPane.dividerLocation = splitPane.height - bottomI
						Else
							splitPane.dividerLocation = splitPane.width - rightI
						End If
					End If
				ElseIf key = START_RESIZE Then
					If Not ui.dividerKeyboardResize Then
						splitPane.requestFocus()
					Else
						Dim parentSplitPane As JSplitPane = CType(SwingUtilities.getAncestorOfClass(GetType(JSplitPane), splitPane), JSplitPane)
						If parentSplitPane IsNot Nothing Then parentSplitPane.requestFocus()
					End If
				ElseIf key = TOGGLE_FOCUS Then
					toggleFocus(splitPane)
				ElseIf key = FOCUS_OUT_FORWARD Then
					moveFocus(splitPane, 1)
				ElseIf key = FOCUS_OUT_BACKWARD Then
					moveFocus(splitPane, -1)
				End If
			End Sub

			Private Sub moveFocus(ByVal splitPane As JSplitPane, ByVal direction As Integer)
				Dim rootAncestor As Container = splitPane.focusCycleRootAncestor
				Dim policy As FocusTraversalPolicy = rootAncestor.focusTraversalPolicy
				Dim focusOn As Component = If(direction > 0, policy.getComponentAfter(rootAncestor, splitPane), policy.getComponentBefore(rootAncestor, splitPane))
				Dim focusFrom As New HashSet(Of Component)
				If splitPane.isAncestorOf(focusOn) Then
					Do
						focusFrom.Add(focusOn)
						rootAncestor = focusOn.focusCycleRootAncestor
						policy = rootAncestor.focusTraversalPolicy
						focusOn = If(direction > 0, policy.getComponentAfter(rootAncestor, focusOn), policy.getComponentBefore(rootAncestor, focusOn))
					Loop While splitPane.isAncestorOf(focusOn) AndAlso Not focusFrom.Contains(focusOn)
				End If
				If focusOn IsNot Nothing AndAlso (Not splitPane.isAncestorOf(focusOn)) Then focusOn.requestFocus()
			End Sub

			Private Sub toggleFocus(ByVal splitPane As JSplitPane)
				Dim left As Component = splitPane.leftComponent
				Dim right As Component = splitPane.rightComponent

				Dim manager As KeyboardFocusManager = KeyboardFocusManager.currentKeyboardFocusManager
				Dim focus As Component = manager.focusOwner
				Dim focusOn As Component = getNextSide(splitPane, focus)
				If focusOn IsNot Nothing Then
					' don't change the focus if the new focused component belongs
					' to the same splitpane and the same side
					If focus IsNot Nothing AndAlso ((SwingUtilities.isDescendingFrom(focus, left) AndAlso SwingUtilities.isDescendingFrom(focusOn, left)) OrElse (SwingUtilities.isDescendingFrom(focus, right) AndAlso SwingUtilities.isDescendingFrom(focusOn, right))) Then Return
					sun.swing.SwingUtilities2.compositeRequestFocus(focusOn)
				End If
			End Sub

			Private Function getNextSide(ByVal splitPane As JSplitPane, ByVal focus As Component) As Component
				Dim left As Component = splitPane.leftComponent
				Dim right As Component = splitPane.rightComponent
				Dim [next] As Component
				If focus IsNot Nothing AndAlso SwingUtilities.isDescendingFrom(focus, left) AndAlso right IsNot Nothing Then
					[next] = getFirstAvailableComponent(right)
					If [next] IsNot Nothing Then Return [next]
				End If
				Dim parentSplitPane As JSplitPane = CType(SwingUtilities.getAncestorOfClass(GetType(JSplitPane), splitPane), JSplitPane)
				If parentSplitPane IsNot Nothing Then
					' focus next side of the parent split pane
					[next] = getNextSide(parentSplitPane, focus)
				Else
					[next] = getFirstAvailableComponent(left)
					If [next] Is Nothing Then [next] = getFirstAvailableComponent(right)
				End If
				Return [next]
			End Function

			Private Function getFirstAvailableComponent(ByVal c As Component) As Component
				If c IsNot Nothing AndAlso TypeOf c Is JSplitPane Then
					Dim sp As JSplitPane = CType(c, JSplitPane)
					Dim left As Component = getFirstAvailableComponent(sp.leftComponent)
					If left IsNot Nothing Then
						c = left
					Else
						c = getFirstAvailableComponent(sp.rightComponent)
					End If
				End If
				Return c
			End Function
		End Class
	End Class

End Namespace
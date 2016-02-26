Imports javax.swing
Imports javax.swing.plaf
Imports javax.swing.event
Imports javax.swing.plaf.basic

'
' * Copyright (c) 2002, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.swing.plaf.synth


	''' <summary>
	''' Provides the Synth L&amp;F UI delegate for
	''' <seealso cref="javax.swing.JComboBox"/>.
	''' 
	''' @author Scott Violet
	''' @since 1.7
	''' </summary>
	Public Class SynthComboBoxUI
		Inherits BasicComboBoxUI
		Implements java.beans.PropertyChangeListener, SynthUI

		Private style As SynthStyle
		Private useListColors As Boolean

		''' <summary>
		''' Used to adjust the location and size of the popup. Very useful for
		''' situations such as we find in Nimbus where part of the border is used
		''' to paint the focus. In such cases, the border is empty space, and not
		''' part of the "visual" border, and in these cases, you'd like the popup
		''' to be adjusted such that it looks as if it were next to the visual border.
		''' You may want to use negative insets to get the right look.
		''' </summary>
		Friend popupInsets As Insets

		''' <summary>
		''' This flag may be set via UIDefaults. By default, it is false, to
		''' preserve backwards compatibility. If true, then the combo will
		''' "act as a button" when it is not editable.
		''' </summary>
		Private buttonWhenNotEditable As Boolean

		''' <summary>
		''' A flag to indicate that the combo box and combo box button should
		''' remain in the PRESSED state while the combo popup is visible.
		''' </summary>
		Private pressedWhenPopupVisible As Boolean

		''' <summary>
		''' When buttonWhenNotEditable is true, this field is used to help make
		''' the combo box appear and function as a button when the combo box is
		''' not editable. In such a state, you can click anywhere on the button
		''' to get it to open the popup. Also, anywhere you hover over the combo
		''' will cause the entire combo to go into "rollover" state, and anywhere
		''' you press will go into "pressed" state. This also keeps in sync the
		''' state of the combo and the arrowButton.
		''' </summary>
		Private buttonHandler As ButtonHandler

		''' <summary>
		''' Handler for repainting combo when editor component gains/looses focus
		''' </summary>
		Private editorFocusHandler As EditorFocusHandler

		''' <summary>
		''' If true, then the cell renderer will be forced to be non-opaque when
		''' used for rendering the selected item in the combo box (not in the list),
		''' and forced to opaque after rendering the selected value.
		''' </summary>
		Private forceOpaque As Boolean = False

		''' <summary>
		''' Creates a new UI object for the given component.
		''' </summary>
		''' <param name="c"> component to create UI object for </param>
		''' <returns> the UI object </returns>
		Public Shared Function createUI(ByVal c As JComponent) As ComponentUI
			Return New SynthComboBoxUI
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' 
		''' Overridden to ensure that ButtonHandler is created prior to any of
		''' the other installXXX methods, since several of them reference
		''' buttonHandler.
		''' </summary>
		Public Overrides Sub installUI(ByVal c As JComponent)
			buttonHandler = New ButtonHandler(Me)
			MyBase.installUI(c)
		End Sub

		Protected Friend Overrides Sub installDefaults()
			updateStyle(comboBox)
		End Sub

		Private Sub updateStyle(ByVal comboBox As JComboBox)
			Dim oldStyle As SynthStyle = style
			Dim ___context As SynthContext = getContext(comboBox, ENABLED)

			style = SynthLookAndFeel.updateStyle(___context, Me)
			If style IsNot oldStyle Then
				padding = CType(style.get(___context, "ComboBox.padding"), Insets)
				popupInsets = CType(style.get(___context, "ComboBox.popupInsets"), Insets)
				useListColors = style.getBoolean(___context, "ComboBox.rendererUseListColors", True)
				buttonWhenNotEditable = style.getBoolean(___context, "ComboBox.buttonWhenNotEditable", False)
				pressedWhenPopupVisible = style.getBoolean(___context, "ComboBox.pressedWhenPopupVisible", False)
				squareButton = style.getBoolean(___context, "ComboBox.squareButton", True)

				If oldStyle IsNot Nothing Then
					uninstallKeyboardActions()
					installKeyboardActions()
				End If
				forceOpaque = style.getBoolean(___context, "ComboBox.forceOpaque", False)
			End If
			___context.Dispose()

			If listBox IsNot Nothing Then SynthLookAndFeel.updateStyles(listBox)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub installListeners()
			comboBox.addPropertyChangeListener(Me)
			comboBox.addMouseListener(buttonHandler)
			editorFocusHandler = New EditorFocusHandler(comboBox)
			MyBase.installListeners()
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Sub uninstallUI(ByVal c As JComponent)
			If TypeOf popup Is SynthComboPopup Then CType(popup, SynthComboPopup).removePopupMenuListener(buttonHandler)
			MyBase.uninstallUI(c)
			buttonHandler = Nothing
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub uninstallDefaults()
			Dim ___context As SynthContext = getContext(comboBox, ENABLED)

			style.uninstallDefaults(___context)
			___context.Dispose()
			style = Nothing
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub uninstallListeners()
			editorFocusHandler.unregister()
			comboBox.removePropertyChangeListener(Me)
			comboBox.removeMouseListener(buttonHandler)
			buttonHandler.pressed = False
			buttonHandler.over = False
			MyBase.uninstallListeners()
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Function getContext(ByVal c As JComponent) As SynthContext
			Return getContext(c, getComponentState(c))
		End Function

		Private Function getContext(ByVal c As JComponent, ByVal state As Integer) As SynthContext
			Return SynthContext.getContext(c, style, state)
		End Function

		Private Function getComponentState(ByVal c As JComponent) As Integer
			' currently we have a broken situation where if a developer
			' takes the border from a JComboBox and sets it on a JTextField
			' then the codepath will eventually lead back to this method
			' but pass in a JTextField instead of JComboBox! In case this
			' happens, we just return the normal synth state for the component
			' instead of doing anything special
			If Not(TypeOf c Is JComboBox) Then Return SynthLookAndFeel.getComponentState(c)

			Dim ___box As JComboBox = CType(c, JComboBox)
			If shouldActLikeButton() Then
				Dim state As Integer = ENABLED
				If ((Not c.enabled)) Then state = DISABLED
				If buttonHandler.pressed Then state = state Or PRESSED
				If buttonHandler.rollover Then state = state Or MOUSE_OVER
				If ___box.focusOwner Then state = state Or FOCUSED
				Return state
			Else
				' for editable combos the editor component has the focus not the
				' combo box its self, so we should make the combo paint focused
				' when its editor has focus
				Dim basicState As Integer = SynthLookAndFeel.getComponentState(c)
				If ___box.editable AndAlso ___box.editor.editorComponent.focusOwner Then basicState = basicState Or FOCUSED
				Return basicState
			End If
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Function createPopup() As ComboPopup
			Dim p As New SynthComboPopup(comboBox)
			p.addPopupMenuListener(buttonHandler)
			Return p
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Function createRenderer() As ListCellRenderer
			Return New SynthComboBoxRenderer(Me)
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Function createEditor() As ComboBoxEditor
			Return New SynthComboBoxEditor
		End Function

		'
		' end UI Initialization
		'======================

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Sub propertyChange(ByVal e As java.beans.PropertyChangeEvent)
			If SynthLookAndFeel.shouldUpdateStyle(e) Then updateStyle(comboBox)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Function createArrowButton() As JButton
			Dim button As New SynthArrowButton(SwingConstants.SOUTH)
			button.name = "ComboBox.arrowButton"
			button.model = buttonHandler
			Return button
		End Function

		'=================================
		' begin ComponentUI Implementation

		''' <summary>
		''' Notifies this UI delegate to repaint the specified component.
		''' This method paints the component background, then calls
		''' the <seealso cref="#paint(SynthContext,Graphics)"/> method.
		''' 
		''' <p>In general, this method does not need to be overridden by subclasses.
		''' All Look and Feel rendering code should reside in the {@code paint} method.
		''' </summary>
		''' <param name="g"> the {@code Graphics} object used for painting </param>
		''' <param name="c"> the component being painted </param>
		''' <seealso cref= #paint(SynthContext,Graphics) </seealso>
		Public Overrides Sub update(ByVal g As Graphics, ByVal c As JComponent)
			Dim ___context As SynthContext = getContext(c)

			SynthLookAndFeel.update(___context, g)
			___context.painter.paintComboBoxBackground(___context, g, 0, 0, c.width, c.height)
			paint(___context, g)
			___context.Dispose()
		End Sub

		''' <summary>
		''' Paints the specified component according to the Look and Feel.
		''' <p>This method is not used by Synth Look and Feel.
		''' Painting is handled by the <seealso cref="#paint(SynthContext,Graphics)"/> method.
		''' </summary>
		''' <param name="g"> the {@code Graphics} object used for painting </param>
		''' <param name="c"> the component being painted </param>
		''' <seealso cref= #paint(SynthContext,Graphics) </seealso>
		Public Overrides Sub paint(ByVal g As Graphics, ByVal c As JComponent)
			Dim ___context As SynthContext = getContext(c)

			paint(___context, g)
			___context.Dispose()
		End Sub

		''' <summary>
		''' Paints the specified component.
		''' </summary>
		''' <param name="context"> context for the component being painted </param>
		''' <param name="g"> the {@code Graphics} object used for painting </param>
		''' <seealso cref= #update(Graphics,JComponent) </seealso>
		Protected Friend Overridable Sub paint(ByVal context As SynthContext, ByVal g As Graphics)
			hasFocus = comboBox.hasFocus()
			If Not comboBox.editable Then
				Dim r As Rectangle = rectangleForCurrentValue()
				paintCurrentValue(g,r,hasFocus)
			End If
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Sub paintBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			context.painter.paintComboBoxBorder(context, g, x, y, w, h)
		End Sub

		''' <summary>
		''' Paints the currently selected item.
		''' </summary>
		Public Overrides Sub paintCurrentValue(ByVal g As Graphics, ByVal bounds As Rectangle, ByVal hasFocus As Boolean)
			Dim renderer As ListCellRenderer = comboBox.renderer
			Dim c As Component

			c = renderer.getListCellRendererComponent(listBox, comboBox.selectedItem, -1, False, False)

			' Fix for 4238829: should lay out the JPanel.
			Dim shouldValidate As Boolean = False
			If TypeOf c Is JPanel Then shouldValidate = True

			If TypeOf c Is UIResource Then c.name = "ComboBox.renderer"

			Dim force As Boolean = forceOpaque AndAlso TypeOf c Is JComponent
			If force Then CType(c, JComponent).opaque = False

			Dim x As Integer = bounds.x, y As Integer = bounds.y, w As Integer = bounds.width, h As Integer = bounds.height
			If padding IsNot Nothing Then
				x = bounds.x + padding.left
				y = bounds.y + padding.top
				w = bounds.width - (padding.left + padding.right)
				h = bounds.height - (padding.top + padding.bottom)
			End If

			currentValuePane.paintComponent(g, c, comboBox, x, y, w, h, shouldValidate)

			If force Then CType(c, JComponent).opaque = True
		End Sub

		''' <returns> true if this combo box should act as one big button. Typically
		''' only happens when buttonWhenNotEditable is true, and comboBox.isEditable
		''' is false. </returns>
		Private Function shouldActLikeButton() As Boolean
			Return buttonWhenNotEditable AndAlso Not comboBox.editable
		End Function

		''' <summary>
		''' Returns the default size of an empty display area of the combo box using
		''' the current renderer and font.
		''' 
		''' This method was overridden to use SynthComboBoxRenderer instead of
		''' DefaultListCellRenderer as the default renderer when calculating the
		''' size of the combo box. This is used in the case of the combo not having
		''' any data.
		''' </summary>
		''' <returns> the size of an empty display area </returns>
		''' <seealso cref= #getDisplaySize </seealso>
		Protected Friend Property Overrides defaultSize As Dimension
			Get
				Dim r As New SynthComboBoxRenderer(Me)
				Dim d As Dimension = getSizeForComponent(r.getListCellRendererComponent(listBox, " ", -1, False, False))
				Return New Dimension(d.width, d.height)
			End Get
		End Property

		''' <summary>
		''' From BasicComboBoxRenderer v 1.18.
		''' 
		''' Be aware that SynthFileChooserUIImpl relies on the fact that the default
		''' renderer installed on a Synth combo box is a JLabel. If this is changed,
		''' then an assert will fail in SynthFileChooserUIImpl
		''' </summary>
		Private Class SynthComboBoxRenderer
			Inherits JLabel
			Implements ListCellRenderer(Of Object), UIResource

			Private ReadOnly outerInstance As SynthComboBoxUI

			Public Sub New(ByVal outerInstance As SynthComboBoxUI)
					Me.outerInstance = outerInstance
				MyBase.New()
				text = " "
			End Sub

			Public Property Overrides name As String
				Get
					' SynthComboBoxRenderer should have installed Name while constructor is working.
					' The setName invocation in the SynthComboBoxRenderer() constructor doesn't work
					' because of the opaque property is installed in the constructor based on the
					' component name (see GTKStyle.isOpaque())
					Dim ___name As String = MyBase.name
    
					Return If(___name Is Nothing, "ComboBox.renderer", ___name)
				End Get
			End Property

			Public Overrides Function getListCellRendererComponent(Of T1)(ByVal list As JList(Of T1), ByVal value As Object, ByVal index As Integer, ByVal isSelected As Boolean, ByVal cellHasFocus As Boolean) As Component
				name = "ComboBox.listRenderer"
				SynthLookAndFeel.resetSelectedUI()
				If isSelected Then
					background = list.selectionBackground
					foreground = list.selectionForeground
					If Not outerInstance.useListColors Then SynthLookAndFeel.selectedUIdUI(CType(SynthLookAndFeel.getUIOfType(uI, GetType(SynthLabelUI)), SynthLabelUI), isSelected, cellHasFocus, list.enabled, False)
				Else
					background = list.background
					foreground = list.foreground
				End If

				font = list.font

				If TypeOf value Is Icon Then
					icon = CType(value, Icon)
					text = ""
				Else
					Dim ___text As String = If(value Is Nothing, " ", value.ToString())

					If "".Equals(___text) Then ___text = " "
					text = ___text
				End If

				' The renderer component should inherit the enabled and
				' orientation state of its parent combobox.  This is
				' especially needed for GTK comboboxes, where the
				' ListCellRenderer's state determines the visual state
				' of the combobox.
				If outerInstance.comboBox IsNot Nothing Then
					enabled = outerInstance.comboBox.enabled
					componentOrientation = outerInstance.comboBox.componentOrientation
				End If

				Return Me
			End Function

			Public Overrides Sub paint(ByVal g As Graphics)
				MyBase.paint(g)
				SynthLookAndFeel.resetSelectedUI()
			End Sub
		End Class


		Private Class SynthComboBoxEditor
			Inherits BasicComboBoxEditor.UIResource

			Public Overrides Function createEditorComponent() As JTextField
				Dim f As New JTextField("", 9)
				f.name = "ComboBox.textField"
				Return f
			End Function
		End Class


		''' <summary>
		''' Handles all the logic for treating the combo as a button when it is
		''' not editable, and when shouldActLikeButton() is true. This class is a
		''' special ButtonModel, and installed on the arrowButton when appropriate.
		''' It also is installed as a mouse listener and mouse motion listener on
		''' the combo box. In this way, the state between the button and combo
		''' are in sync. Whenever one is "over" both are. Whenever one is pressed,
		''' both are.
		''' </summary>
		Private NotInheritable Class ButtonHandler
			Inherits DefaultButtonModel
			Implements MouseListener, PopupMenuListener

			Private ReadOnly outerInstance As SynthComboBoxUI

			Public Sub New(ByVal outerInstance As SynthComboBoxUI)
				Me.outerInstance = outerInstance
			End Sub

			''' <summary>
			''' Indicates that the mouse is over the combo or the arrow button.
			''' This field only has meaning if buttonWhenNotEnabled is true.
			''' </summary>
			Private over As Boolean
			''' <summary>
			''' Indicates that the combo or arrow button has been pressed. This
			''' field only has meaning if buttonWhenNotEnabled is true.
			''' </summary>
			Private pressed As Boolean

			'------------------------------------------------------------------
			' State Methods
			'------------------------------------------------------------------

			''' <summary>
			''' <p>Updates the internal "pressed" state. If shouldActLikeButton()
			''' is true, and if this method call will change the internal state,
			''' then the combo and button will be repainted.</p>
			''' 
			''' <p>Note that this method is called either when a press event
			''' occurs on the combo box, or on the arrow button.</p>
			''' </summary>
			Private Sub updatePressed(ByVal p As Boolean)
				Me.pressed = p AndAlso enabled
				If outerInstance.shouldActLikeButton() Then outerInstance.comboBox.repaint()
			End Sub

			''' <summary>
			''' <p>Updates the internal "over" state. If shouldActLikeButton()
			''' is true, and if this method call will change the internal state,
			''' then the combo and button will be repainted.</p>
			''' 
			''' <p>Note that this method is called either when a mouseover/mouseoff event
			''' occurs on the combo box, or on the arrow button.</p>
			''' </summary>
			Private Sub updateOver(ByVal o As Boolean)
				Dim old As Boolean = rollover
				Me.over = o AndAlso enabled
				Dim newo As Boolean = rollover
				If outerInstance.shouldActLikeButton() AndAlso old <> newo Then outerInstance.comboBox.repaint()
			End Sub

			'------------------------------------------------------------------
			' DefaultButtonModel Methods
			'------------------------------------------------------------------

			''' <summary>
			''' @inheritDoc
			''' 
			''' Ensures that isPressed() will return true if the combo is pressed,
			''' or the arrowButton is pressed, <em>or</em> if the combo popup is
			''' visible. This is the case because a combo box looks pressed when
			''' the popup is visible, and so should the arrow button.
			''' </summary>
			Public Property Overrides pressed As Boolean
				Get
					Dim b As Boolean = If(outerInstance.shouldActLikeButton(), pressed, MyBase.pressed)
					Return b OrElse (outerInstance.pressedWhenPopupVisible AndAlso outerInstance.comboBox.popupVisible)
				End Get
				Set(ByVal b As Boolean)
					MyBase.pressed = b
					updatePressed(b)
				End Set
			End Property

			''' <summary>
			''' @inheritDoc
			''' 
			''' Ensures that the armed state is in sync with the pressed state
			''' if shouldActLikeButton is true. Without this method, the arrow
			''' button will not look pressed when the popup is open, regardless
			''' of the result of isPressed() alone.
			''' </summary>
			Public Property Overrides armed As Boolean
				Get
					Dim b As Boolean = outerInstance.shouldActLikeButton() OrElse (outerInstance.pressedWhenPopupVisible AndAlso outerInstance.comboBox.popupVisible)
					Return If(b, pressed, MyBase.armed)
				End Get
			End Property

			''' <summary>
			''' @inheritDoc
			''' 
			''' Ensures that isRollover() will return true if the combo is
			''' rolled over, or the arrowButton is rolled over.
			''' </summary>
			Public Property Overrides rollover As Boolean
				Get
					Return If(outerInstance.shouldActLikeButton(), over, MyBase.rollover)
				End Get
				Set(ByVal b As Boolean)
					MyBase.rollover = b
					updateOver(b)
				End Set
			End Property



			'------------------------------------------------------------------
			' MouseListener/MouseMotionListener Methods
			'------------------------------------------------------------------

			Public Overrides Sub mouseEntered(ByVal mouseEvent As MouseEvent)
				updateOver(True)
			End Sub

			Public Overrides Sub mouseExited(ByVal mouseEvent As MouseEvent)
				updateOver(False)
			End Sub

			Public Overrides Sub mousePressed(ByVal mouseEvent As MouseEvent)
				updatePressed(True)
			End Sub

			Public Overrides Sub mouseReleased(ByVal mouseEvent As MouseEvent)
				updatePressed(False)
			End Sub

			Public Overrides Sub mouseClicked(ByVal e As MouseEvent)
			'------------------------------------------------------------------
			' PopupMenuListener Methods
			'------------------------------------------------------------------
			''' <summary>
			''' @inheritDoc
			''' 
			''' Ensures that the combo box is repainted when the popup is closed.
			''' This avoids a bug where clicking off the combo wasn't causing a repaint,
			''' and thus the combo box still looked pressed even when it was not.
			''' 
			''' This bug was only noticed when acting as a button, but may be generally
			''' present. If so, remove the if() block
			''' </summary>
			End Sub
			Public Overrides Sub popupMenuCanceled(ByVal e As PopupMenuEvent) Implements PopupMenuListener.popupMenuCanceled
				If outerInstance.shouldActLikeButton() OrElse outerInstance.pressedWhenPopupVisible Then outerInstance.comboBox.repaint()
			End Sub

			Public Overrides Sub popupMenuWillBecomeVisible(ByVal e As PopupMenuEvent) Implements PopupMenuListener.popupMenuWillBecomeVisible
			End Sub
			Public Overrides Sub popupMenuWillBecomeInvisible(ByVal e As PopupMenuEvent) Implements PopupMenuListener.popupMenuWillBecomeInvisible
			End Sub
		End Class

		''' <summary>
		''' Handler for repainting combo when editor component gains/looses focus
		''' </summary>
		Private Class EditorFocusHandler
			Implements FocusListener, java.beans.PropertyChangeListener

			Private comboBox As JComboBox
			Private editor As ComboBoxEditor = Nothing
			Private editorComponent As Component = Nothing

			Private Sub New(ByVal comboBox As JComboBox)
				Me.comboBox = comboBox
				editor = comboBox.editor
				If editor IsNot Nothing Then
					editorComponent = editor.editorComponent
					If editorComponent IsNot Nothing Then editorComponent.addFocusListener(Me)
				End If
				comboBox.addPropertyChangeListener("editor",Me)
			End Sub

			Public Overridable Sub unregister()
				comboBox.removePropertyChangeListener(Me)
				If editorComponent IsNot Nothing Then editorComponent.removeFocusListener(Me)
			End Sub

			''' <summary>
			''' Invoked when a component gains the keyboard focus. </summary>
			Public Overridable Sub focusGained(ByVal e As FocusEvent)
				' repaint whole combo on focus gain
				comboBox.repaint()
			End Sub

			''' <summary>
			''' Invoked when a component loses the keyboard focus. </summary>
			Public Overridable Sub focusLost(ByVal e As FocusEvent)
				' repaint whole combo on focus loss
				comboBox.repaint()
			End Sub

			''' <summary>
			''' Called when the combos editor changes
			''' </summary>
			''' <param name="evt"> A PropertyChangeEvent object describing the event source and
			'''            the property that has changed. </param>
			Public Overridable Sub propertyChange(ByVal evt As java.beans.PropertyChangeEvent)
				Dim newEditor As ComboBoxEditor = comboBox.editor
				If editor IsNot newEditor Then
					If editorComponent IsNot Nothing Then editorComponent.removeFocusListener(Me)
					editor = newEditor
					If editor IsNot Nothing Then
						editorComponent = editor.editorComponent
						If editorComponent IsNot Nothing Then editorComponent.addFocusListener(Me)
					End If
				End If
			End Sub
		End Class
	End Class

End Namespace
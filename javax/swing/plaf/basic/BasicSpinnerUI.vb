Imports System
Imports System.Collections
Imports javax.swing
Imports javax.swing.border
Imports javax.swing.event
Imports javax.swing.plaf
Imports javax.swing.text

'
' * Copyright (c) 2000, 2010, Oracle and/or its affiliates. All rights reserved.
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
	''' The default Spinner UI delegate.
	''' 
	''' @author Hans Muller
	''' @since 1.4
	''' </summary>
	Public Class BasicSpinnerUI
		Inherits SpinnerUI

		''' <summary>
		''' The spinner that we're a UI delegate for.  Initialized by
		''' the <code>installUI</code> method, and reset to null
		''' by <code>uninstallUI</code>.
		''' </summary>
		''' <seealso cref= #installUI </seealso>
		''' <seealso cref= #uninstallUI </seealso>
		Protected Friend spinner As JSpinner
		Private handler As Handler


		''' <summary>
		''' The mouse/action listeners that are added to the spinner's
		''' arrow buttons.  These listeners are shared by all
		''' spinner arrow buttons.
		''' </summary>
		''' <seealso cref= #createNextButton </seealso>
		''' <seealso cref= #createPreviousButton </seealso>
		Private Shared ReadOnly nextButtonHandler As New ArrowButtonHandler("increment", True)
		Private Shared ReadOnly previousButtonHandler As New ArrowButtonHandler("decrement", False)
		Private propertyChangeListener As PropertyChangeListener


		''' <summary>
		''' Used by the default LayoutManager class - SpinnerLayout for
		''' missing (null) editor/nextButton/previousButton children.
		''' </summary>
		Private Shared ReadOnly zeroSize As New Dimension(0, 0)


		''' <summary>
		''' Returns a new instance of BasicSpinnerUI.  SpinnerListUI
		''' delegates are allocated one per JSpinner.
		''' </summary>
		''' <param name="c"> the JSpinner (not used) </param>
		''' <seealso cref= ComponentUI#createUI </seealso>
		''' <returns> a new BasicSpinnerUI object </returns>
		Public Shared Function createUI(ByVal c As JComponent) As ComponentUI
			Return New BasicSpinnerUI
		End Function


		Private Sub maybeAdd(ByVal c As Component, ByVal s As String)
			If c IsNot Nothing Then spinner.add(c, s)
		End Sub


		''' <summary>
		''' Calls <code>installDefaults</code>, <code>installListeners</code>,
		''' and then adds the components returned by <code>createNextButton</code>,
		''' <code>createPreviousButton</code>, and <code>createEditor</code>.
		''' </summary>
		''' <param name="c"> the JSpinner </param>
		''' <seealso cref= #installDefaults </seealso>
		''' <seealso cref= #installListeners </seealso>
		''' <seealso cref= #createNextButton </seealso>
		''' <seealso cref= #createPreviousButton </seealso>
		''' <seealso cref= #createEditor </seealso>
		Public Overridable Sub installUI(ByVal c As JComponent)
			Me.spinner = CType(c, JSpinner)
			installDefaults()
			installListeners()
			maybeAdd(createNextButton(), "Next")
			maybeAdd(createPreviousButton(), "Previous")
			maybeAdd(createEditor(), "Editor")
			updateEnabledState()
			installKeyboardActions()
		End Sub


		''' <summary>
		''' Calls <code>uninstallDefaults</code>, <code>uninstallListeners</code>,
		''' and then removes all of the spinners children.
		''' </summary>
		''' <param name="c"> the JSpinner (not used) </param>
		Public Overridable Sub uninstallUI(ByVal c As JComponent)
			uninstallDefaults()
			uninstallListeners()
			Me.spinner = Nothing
			c.removeAll()
		End Sub


		''' <summary>
		''' Initializes <code>PropertyChangeListener</code> with
		''' a shared object that delegates interesting PropertyChangeEvents
		''' to protected methods.
		''' <p>
		''' This method is called by <code>installUI</code>.
		''' </summary>
		''' <seealso cref= #replaceEditor </seealso>
		''' <seealso cref= #uninstallListeners </seealso>
		Protected Friend Overridable Sub installListeners()
			propertyChangeListener = createPropertyChangeListener()
			spinner.addPropertyChangeListener(propertyChangeListener)
			If sun.swing.DefaultLookup.getBoolean(spinner, Me, "Spinner.disableOnBoundaryValues", False) Then spinner.addChangeListener(handler)
			Dim editor As JComponent = spinner.editor
			If editor IsNot Nothing AndAlso TypeOf editor Is JSpinner.DefaultEditor Then
				Dim tf As JTextField = CType(editor, JSpinner.DefaultEditor).textField
				If tf IsNot Nothing Then
					tf.addFocusListener(nextButtonHandler)
					tf.addFocusListener(previousButtonHandler)
				End If
			End If
		End Sub


		''' <summary>
		''' Removes the <code>PropertyChangeListener</code> added
		''' by installListeners.
		''' <p>
		''' This method is called by <code>uninstallUI</code>.
		''' </summary>
		''' <seealso cref= #installListeners </seealso>
		Protected Friend Overridable Sub uninstallListeners()
			spinner.removePropertyChangeListener(propertyChangeListener)
			spinner.removeChangeListener(handler)
			Dim editor As JComponent = spinner.editor
			removeEditorBorderListener(editor)
			If TypeOf editor Is JSpinner.DefaultEditor Then
				Dim tf As JTextField = CType(editor, JSpinner.DefaultEditor).textField
				If tf IsNot Nothing Then
					tf.removeFocusListener(nextButtonHandler)
					tf.removeFocusListener(previousButtonHandler)
				End If
			End If
			propertyChangeListener = Nothing
			handler = Nothing
		End Sub


		''' <summary>
		''' Initialize the <code>JSpinner</code> <code>border</code>,
		''' <code>foreground</code>, and <code>background</code>, properties
		''' based on the corresponding "Spinner.*" properties from defaults table.
		''' The <code>JSpinners</code> layout is set to the value returned by
		''' <code>createLayout</code>.  This method is called by <code>installUI</code>.
		''' </summary>
		''' <seealso cref= #uninstallDefaults </seealso>
		''' <seealso cref= #installUI </seealso>
		''' <seealso cref= #createLayout </seealso>
		''' <seealso cref= LookAndFeel#installBorder </seealso>
		''' <seealso cref= LookAndFeel#installColors </seealso>
		Protected Friend Overridable Sub installDefaults()
			spinner.layout = createLayout()
			LookAndFeel.installBorder(spinner, "Spinner.border")
			LookAndFeel.installColorsAndFont(spinner, "Spinner.background", "Spinner.foreground", "Spinner.font")
			LookAndFeel.installProperty(spinner, "opaque", Boolean.TRUE)
		End Sub


		''' <summary>
		''' Sets the <code>JSpinner's</code> layout manager to null.  This
		''' method is called by <code>uninstallUI</code>.
		''' </summary>
		''' <seealso cref= #installDefaults </seealso>
		''' <seealso cref= #uninstallUI </seealso>
		Protected Friend Overridable Sub uninstallDefaults()
			spinner.layout = Nothing
		End Sub


		Private Property handler As Handler
			Get
				If handler Is Nothing Then handler = New Handler
				Return handler
			End Get
		End Property


		''' <summary>
		''' Installs the necessary listeners on the next button, <code>c</code>,
		''' to update the <code>JSpinner</code> in response to a user gesture.
		''' </summary>
		''' <param name="c"> Component to install the listeners on </param>
		''' <exception cref="NullPointerException"> if <code>c</code> is null. </exception>
		''' <seealso cref= #createNextButton
		''' @since 1.5 </seealso>
		Protected Friend Overridable Sub installNextButtonListeners(ByVal c As Component)
			installButtonListeners(c, nextButtonHandler)
		End Sub

		''' <summary>
		''' Installs the necessary listeners on the previous button, <code>c</code>,
		''' to update the <code>JSpinner</code> in response to a user gesture.
		''' </summary>
		''' <param name="c"> Component to install the listeners on. </param>
		''' <exception cref="NullPointerException"> if <code>c</code> is null. </exception>
		''' <seealso cref= #createPreviousButton
		''' @since 1.5 </seealso>
		Protected Friend Overridable Sub installPreviousButtonListeners(ByVal c As Component)
			installButtonListeners(c, previousButtonHandler)
		End Sub

		Private Sub installButtonListeners(ByVal c As Component, ByVal handler As ArrowButtonHandler)
			If TypeOf c Is JButton Then CType(c, JButton).addActionListener(handler)
			c.addMouseListener(handler)
		End Sub

		''' <summary>
		''' Creates a <code>LayoutManager</code> that manages the <code>editor</code>,
		''' <code>nextButton</code>, and <code>previousButton</code>
		''' children of the JSpinner.  These three children must be
		''' added with a constraint that identifies their role:
		''' "Editor", "Next", and "Previous". The default layout manager
		''' can handle the absence of any of these children.
		''' </summary>
		''' <returns> a LayoutManager for the editor, next button, and previous button. </returns>
		''' <seealso cref= #createNextButton </seealso>
		''' <seealso cref= #createPreviousButton </seealso>
		''' <seealso cref= #createEditor </seealso>
		Protected Friend Overridable Function createLayout() As LayoutManager
			Return handler
		End Function


		''' <summary>
		''' Creates a <code>PropertyChangeListener</code> that can be
		''' added to the JSpinner itself.  Typically, this listener
		''' will call replaceEditor when the "editor" property changes,
		''' since it's the <code>SpinnerUI's</code> responsibility to
		''' add the editor to the JSpinner (and remove the old one).
		''' This method is called by <code>installListeners</code>.
		''' </summary>
		''' <returns> A PropertyChangeListener for the JSpinner itself </returns>
		''' <seealso cref= #installListeners </seealso>
		Protected Friend Overridable Function createPropertyChangeListener() As PropertyChangeListener
			Return handler
		End Function


		''' <summary>
		''' Creates a decrement button, i.e. component that replaces the spinner
		''' value with the object returned by <code>spinner.getPreviousValue</code>.
		''' By default the <code>previousButton</code> is a {@code JButton}. If the
		''' decrement button is not needed this method should return {@code null}.
		''' </summary>
		''' <returns> a component that will replace the spinner's value with the
		'''     previous value in the sequence, or {@code null} </returns>
		''' <seealso cref= #installUI </seealso>
		''' <seealso cref= #createNextButton </seealso>
		''' <seealso cref= #installPreviousButtonListeners </seealso>
		Protected Friend Overridable Function createPreviousButton() As Component
			Dim c As Component = createArrowButton(SwingConstants.SOUTH)
			c.name = "Spinner.previousButton"
			installPreviousButtonListeners(c)
			Return c
		End Function


		''' <summary>
		''' Creates an increment button, i.e. component that replaces the spinner
		''' value with the object returned by <code>spinner.getNextValue</code>.
		''' By default the <code>nextButton</code> is a {@code JButton}. If the
		''' increment button is not needed this method should return {@code null}.
		''' </summary>
		''' <returns> a component that will replace the spinner's value with the
		'''     next value in the sequence, or {@code null} </returns>
		''' <seealso cref= #installUI </seealso>
		''' <seealso cref= #createPreviousButton </seealso>
		''' <seealso cref= #installNextButtonListeners </seealso>
		Protected Friend Overridable Function createNextButton() As Component
			Dim c As Component = createArrowButton(SwingConstants.NORTH)
			c.name = "Spinner.nextButton"
			installNextButtonListeners(c)
			Return c
		End Function

		Private Function createArrowButton(ByVal direction As Integer) As Component
			Dim b As JButton = New BasicArrowButton(direction)
			Dim buttonBorder As Border = UIManager.getBorder("Spinner.arrowButtonBorder")
			If TypeOf buttonBorder Is UIResource Then
				' Wrap the border to avoid having the UIResource be replaced by
				' the ButtonUI. This is the opposite of using BorderUIResource.
				b.border = New CompoundBorder(buttonBorder, Nothing)
			Else
				b.border = buttonBorder
			End If
			b.inheritsPopupMenu = True
			Return b
		End Function


		''' <summary>
		''' This method is called by installUI to get the editor component
		''' of the <code>JSpinner</code>.  By default it just returns
		''' <code>JSpinner.getEditor()</code>.  Subclasses can override
		''' <code>createEditor</code> to return a component that contains
		''' the spinner's editor or null, if they're going to handle adding
		''' the editor to the <code>JSpinner</code> in an
		''' <code>installUI</code> override.
		''' <p>
		''' Typically this method would be overridden to wrap the editor
		''' with a container with a custom border, since one can't assume
		''' that the editors border can be set directly.
		''' <p>
		''' The <code>replaceEditor</code> method is called when the spinners
		''' editor is changed with <code>JSpinner.setEditor</code>.  If you've
		''' overriden this method, then you'll probably want to override
		''' <code>replaceEditor</code> as well.
		''' </summary>
		''' <returns> the JSpinners editor JComponent, spinner.getEditor() by default </returns>
		''' <seealso cref= #installUI </seealso>
		''' <seealso cref= #replaceEditor </seealso>
		''' <seealso cref= JSpinner#getEditor </seealso>
		Protected Friend Overridable Function createEditor() As JComponent
			Dim editor As JComponent = spinner.editor
			maybeRemoveEditorBorder(editor)
			installEditorBorderListener(editor)
			editor.inheritsPopupMenu = True
			updateEditorAlignment(editor)
			Return editor
		End Function


		''' <summary>
		''' Called by the <code>PropertyChangeListener</code> when the
		''' <code>JSpinner</code> editor property changes.  It's the responsibility
		''' of this method to remove the old editor and add the new one.  By
		''' default this operation is just:
		''' <pre>
		''' spinner.remove(oldEditor);
		''' spinner.add(newEditor, "Editor");
		''' </pre>
		''' The implementation of <code>replaceEditor</code> should be coordinated
		''' with the <code>createEditor</code> method.
		''' </summary>
		''' <seealso cref= #createEditor </seealso>
		''' <seealso cref= #createPropertyChangeListener </seealso>
		Protected Friend Overridable Sub replaceEditor(ByVal oldEditor As JComponent, ByVal newEditor As JComponent)
			spinner.remove(oldEditor)
			maybeRemoveEditorBorder(newEditor)
			installEditorBorderListener(newEditor)
			newEditor.inheritsPopupMenu = True
			spinner.add(newEditor, "Editor")
		End Sub

		Private Sub updateEditorAlignment(ByVal editor As JComponent)
			If TypeOf editor Is JSpinner.DefaultEditor Then
				' if editor alignment isn't set in LAF, we get 0 (CENTER) here
				Dim alignment As Integer = UIManager.getInt("Spinner.editorAlignment")
				Dim text As JTextField = CType(editor, JSpinner.DefaultEditor).textField
				text.horizontalAlignment = alignment
			End If
		End Sub

		''' <summary>
		''' Remove the border around the inner editor component for LaFs
		''' that install an outside border around the spinner,
		''' </summary>
		Private Sub maybeRemoveEditorBorder(ByVal editor As JComponent)
			If Not UIManager.getBoolean("Spinner.editorBorderPainted") Then
				If TypeOf editor Is JPanel AndAlso editor.border Is Nothing AndAlso editor.componentCount > 0 Then editor = CType(editor.getComponent(0), JComponent)

				If editor IsNot Nothing AndAlso TypeOf editor.border Is UIResource Then editor.border = Nothing
			End If
		End Sub

		''' <summary>
		''' Remove the border around the inner editor component for LaFs
		''' that install an outside border around the spinner,
		''' </summary>
		Private Sub installEditorBorderListener(ByVal editor As JComponent)
			If Not UIManager.getBoolean("Spinner.editorBorderPainted") Then
				If TypeOf editor Is JPanel AndAlso editor.border Is Nothing AndAlso editor.componentCount > 0 Then editor = CType(editor.getComponent(0), JComponent)
				If editor IsNot Nothing AndAlso (editor.border Is Nothing OrElse TypeOf editor.border Is UIResource) Then editor.addPropertyChangeListener(handler)
			End If
		End Sub

		Private Sub removeEditorBorderListener(ByVal editor As JComponent)
			If Not UIManager.getBoolean("Spinner.editorBorderPainted") Then
				If TypeOf editor Is JPanel AndAlso editor.componentCount > 0 Then editor = CType(editor.getComponent(0), JComponent)
				If editor IsNot Nothing Then editor.removePropertyChangeListener(handler)
			End If
		End Sub


		''' <summary>
		''' Updates the enabled state of the children Components based on the
		''' enabled state of the <code>JSpinner</code>.
		''' </summary>
		Private Sub updateEnabledState()
			updateEnabledState(spinner, spinner.enabled)
		End Sub


		''' <summary>
		''' Recursively updates the enabled state of the child
		''' <code>Component</code>s of <code>c</code>.
		''' </summary>
		Private Sub updateEnabledState(ByVal c As Container, ByVal enabled As Boolean)
			For counter As Integer = c.componentCount - 1 To 0 Step -1
				Dim child As Component = c.getComponent(counter)

				If sun.swing.DefaultLookup.getBoolean(spinner, Me, "Spinner.disableOnBoundaryValues", False) Then
					Dim model As SpinnerModel = spinner.model
					If child.name = "Spinner.nextButton" AndAlso model.nextValue Is Nothing Then
						child.enabled = False
					ElseIf child.name = "Spinner.previousButton" AndAlso model.previousValue Is Nothing Then
						child.enabled = False
					Else
						child.enabled = enabled
					End If
				Else
					child.enabled = enabled
				End If
				If TypeOf child Is Container Then updateEnabledState(CType(child, Container), enabled)
			Next counter
		End Sub


		''' <summary>
		''' Installs the keyboard Actions onto the JSpinner.
		''' 
		''' @since 1.5
		''' </summary>
		Protected Friend Overridable Sub installKeyboardActions()
			Dim iMap As InputMap = getInputMap(JComponent.WHEN_ANCESTOR_OF_FOCUSED_COMPONENT)

			SwingUtilities.replaceUIInputMap(spinner, JComponent.WHEN_ANCESTOR_OF_FOCUSED_COMPONENT, iMap)

			LazyActionMap.installLazyActionMap(spinner, GetType(BasicSpinnerUI), "Spinner.actionMap")
		End Sub

		''' <summary>
		''' Returns the InputMap to install for <code>condition</code>.
		''' </summary>
		Private Function getInputMap(ByVal condition As Integer) As InputMap
			If condition = JComponent.WHEN_ANCESTOR_OF_FOCUSED_COMPONENT Then Return CType(sun.swing.DefaultLookup.get(spinner, Me, "Spinner.ancestorInputMap"), InputMap)
			Return Nothing
		End Function

		Friend Shared Sub loadActionMap(ByVal map As LazyActionMap)
			map.put("increment", nextButtonHandler)
			map.put("decrement", previousButtonHandler)
		End Sub

		''' <summary>
		''' Returns the baseline.
		''' </summary>
		''' <exception cref="NullPointerException"> {@inheritDoc} </exception>
		''' <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
		''' <seealso cref= javax.swing.JComponent#getBaseline(int, int)
		''' @since 1.6 </seealso>
		Public Overridable Function getBaseline(ByVal c As JComponent, ByVal width As Integer, ByVal height As Integer) As Integer
			MyBase.getBaseline(c, width, height)
			Dim editor As JComponent = spinner.editor
			Dim insets As Insets = spinner.insets
			width = width - insets.left - insets.right
			height = height - insets.top - insets.bottom
			If width >= 0 AndAlso height >= 0 Then
				Dim ___baseline As Integer = editor.getBaseline(width, height)
				If ___baseline >= 0 Then Return insets.top + ___baseline
			End If
			Return -1
		End Function

		''' <summary>
		''' Returns an enum indicating how the baseline of the component
		''' changes as the size changes.
		''' </summary>
		''' <exception cref="NullPointerException"> {@inheritDoc} </exception>
		''' <seealso cref= javax.swing.JComponent#getBaseline(int, int)
		''' @since 1.6 </seealso>
		Public Overridable Function getBaselineResizeBehavior(ByVal c As JComponent) As Component.BaselineResizeBehavior
			MyBase.getBaselineResizeBehavior(c)
			Return spinner.editor.baselineResizeBehavior
		End Function

		''' <summary>
		''' A handler for spinner arrow button mouse and action events.  When
		''' a left mouse pressed event occurs we look up the (enabled) spinner
		''' that's the source of the event and start the autorepeat timer.  The
		''' timer fires action events until any button is released at which
		''' point the timer is stopped and the reference to the spinner cleared.
		''' The timer doesn't start until after a 300ms delay, so often the
		''' source of the initial (and final) action event is just the button
		''' logic for mouse released - which means that we're relying on the fact
		''' that our mouse listener runs after the buttons mouse listener.
		''' <p>
		''' Note that one instance of this handler is shared by all slider previous
		''' arrow buttons and likewise for all of the next buttons,
		''' so it doesn't have any state that persists beyond the limits
		''' of a single button pressed/released gesture.
		''' </summary>
		Private Class ArrowButtonHandler
			Inherits AbstractAction
			Implements FocusListener, MouseListener, UIResource

			Friend ReadOnly autoRepeatTimer As javax.swing.Timer
			Friend ReadOnly isNext As Boolean
			Friend spinner As JSpinner = Nothing
			Friend arrowButton As JButton = Nothing

			Friend Sub New(ByVal name As String, ByVal isNext As Boolean)
				MyBase.New(name)
				Me.isNext = isNext
				autoRepeatTimer = New javax.swing.Timer(60, Me)
				autoRepeatTimer.initialDelay = 300
			End Sub

			Private Function eventToSpinner(ByVal e As AWTEvent) As JSpinner
				Dim src As Object = e.source
				Do While (TypeOf src Is Component) AndAlso Not(TypeOf src Is JSpinner)
					src = CType(src, Component).parent
				Loop
				Return If(TypeOf src Is JSpinner, CType(src, JSpinner), Nothing)
			End Function

			Public Overridable Sub actionPerformed(ByVal e As ActionEvent)
				Dim spinner As JSpinner = Me.spinner

				If Not(TypeOf e.source Is javax.swing.Timer) Then
					' Most likely resulting from being in ActionMap.
					spinner = eventToSpinner(e)
					If TypeOf e.source Is JButton Then arrowButton = CType(e.source, JButton)
				Else
					If arrowButton IsNot Nothing AndAlso (Not arrowButton.model.pressed) AndAlso autoRepeatTimer.running Then
						autoRepeatTimer.stop()
						spinner = Nothing
						arrowButton = Nothing
					End If
				End If
				If spinner IsNot Nothing Then
					Try
						Dim ___calendarField As Integer = getCalendarField(spinner)
						spinner.commitEdit()
						If ___calendarField <> -1 Then CType(spinner.model, SpinnerDateModel).calendarField = ___calendarField
						Dim ___value As Object = If(isNext, spinner.nextValue, spinner.previousValue)
						If ___value IsNot Nothing Then
							spinner.value = ___value
							[select](spinner)
						End If
					Catch iae As System.ArgumentException
						UIManager.lookAndFeel.provideErrorFeedback(spinner)
					Catch pe As java.text.ParseException
						UIManager.lookAndFeel.provideErrorFeedback(spinner)
					End Try
				End If
			End Sub

			''' <summary>
			''' If the spinner's editor is a DateEditor, this selects the field
			''' associated with the value that is being incremented.
			''' </summary>
			Private Sub [select](ByVal spinner As JSpinner)
				Dim editor As JComponent = spinner.editor

				If TypeOf editor Is JSpinner.DateEditor Then
					Dim dateEditor As JSpinner.DateEditor = CType(editor, JSpinner.DateEditor)
					Dim ftf As JFormattedTextField = dateEditor.textField
					Dim format As Format = dateEditor.format
					Dim ___value As Object

					___value = spinner.value
					If format IsNot Nothing AndAlso ___value IsNot Nothing Then
						Dim model As SpinnerDateModel = dateEditor.model
						Dim field As DateFormat.Field = DateFormat.Field.ofCalendarField(model.calendarField)

						If field IsNot Nothing Then
							Try
								Dim [iterator] As AttributedCharacterIterator = format.formatToCharacterIterator(___value)
								If (Not [select](ftf, [iterator], field)) AndAlso field Is DateFormat.Field.HOUR0 Then [select](ftf, [iterator], DateFormat.Field.HOUR1)
							Catch iae As System.ArgumentException
							End Try
						End If
					End If
				End If
			End Sub

			''' <summary>
			''' Selects the passed in field, returning true if it is found,
			''' false otherwise.
			''' </summary>
			Private Function [select](ByVal ftf As JFormattedTextField, ByVal [iterator] As AttributedCharacterIterator, ByVal field As DateFormat.Field) As Boolean
				Dim max As Integer = ftf.document.length

				[iterator].first()
				Do
					Dim attrs As IDictionary = [iterator].attributes

					If attrs IsNot Nothing AndAlso attrs.Contains(field) Then
						Dim start As Integer = [iterator].getRunStart(field)
						Dim [end] As Integer = [iterator].getRunLimit(field)

						If start <> -1 AndAlso [end] <> -1 AndAlso start <= max AndAlso [end] <= max Then ftf.select(start, [end])
						Return True
					End If
				Loop While [iterator].next() <> CharacterIterator.DONE
				Return False
			End Function

			''' <summary>
			''' Returns the calendarField under the start of the selection, or
			''' -1 if there is no valid calendar field under the selection (or
			''' the spinner isn't editing dates.
			''' </summary>
			Private Function getCalendarField(ByVal spinner As JSpinner) As Integer
				Dim editor As JComponent = spinner.editor

				If TypeOf editor Is JSpinner.DateEditor Then
					Dim dateEditor As JSpinner.DateEditor = CType(editor, JSpinner.DateEditor)
					Dim ftf As JFormattedTextField = dateEditor.textField
					Dim start As Integer = ftf.selectionStart
					Dim formatter As JFormattedTextField.AbstractFormatter = ftf.formatter

					If TypeOf formatter Is InternationalFormatter Then
						Dim fields As Format.Field() = CType(formatter, InternationalFormatter).getFields(start)

						For counter As Integer = 0 To fields.Length - 1
							If TypeOf fields(counter) Is DateFormat.Field Then
								Dim ___calendarField As Integer

								If fields(counter) Is DateFormat.Field.HOUR1 Then
									___calendarField = DateTime.HOUR
								Else
									___calendarField = CType(fields(counter), DateFormat.Field).calendarField
								End If
								If ___calendarField <> -1 Then Return ___calendarField
							End If
						Next counter
					End If
				End If
				Return -1
			End Function

			Public Overridable Sub mousePressed(ByVal e As MouseEvent)
				If SwingUtilities.isLeftMouseButton(e) AndAlso e.component.enabled Then
					spinner = eventToSpinner(e)
					autoRepeatTimer.start()

					focusSpinnerIfNecessary()
				End If
			End Sub

			Public Overridable Sub mouseReleased(ByVal e As MouseEvent)
				autoRepeatTimer.stop()
				arrowButton = Nothing
				spinner = Nothing
			End Sub

			Public Overridable Sub mouseClicked(ByVal e As MouseEvent)
			End Sub

			Public Overridable Sub mouseEntered(ByVal e As MouseEvent)
				If spinner IsNot Nothing AndAlso (Not autoRepeatTimer.running) AndAlso spinner Is eventToSpinner(e) Then autoRepeatTimer.start()
			End Sub

			Public Overridable Sub mouseExited(ByVal e As MouseEvent)
				If autoRepeatTimer.running Then autoRepeatTimer.stop()
			End Sub

			''' <summary>
			''' Requests focus on a child of the spinner if the spinner doesn't
			''' have focus.
			''' </summary>
			Private Sub focusSpinnerIfNecessary()
				Dim fo As Component = KeyboardFocusManager.currentKeyboardFocusManager.focusOwner
				If spinner.requestFocusEnabled AndAlso (fo Is Nothing OrElse (Not SwingUtilities.isDescendingFrom(fo, spinner))) Then
					Dim root As Container = spinner

					If Not root.focusCycleRoot Then root = root.focusCycleRootAncestor
					If root IsNot Nothing Then
						Dim ftp As FocusTraversalPolicy = root.focusTraversalPolicy
						Dim child As Component = ftp.getComponentAfter(root, spinner)

						If child IsNot Nothing AndAlso SwingUtilities.isDescendingFrom(child, spinner) Then child.requestFocus()
					End If
				End If
			End Sub

			Public Overridable Sub focusGained(ByVal e As FocusEvent)
			End Sub

			Public Overridable Sub focusLost(ByVal e As FocusEvent)
				If spinner Is eventToSpinner(e) Then
					If autoRepeatTimer.running Then autoRepeatTimer.stop()
					spinner = Nothing
					If arrowButton IsNot Nothing Then
						Dim model As ButtonModel = arrowButton.model
						model.pressed = False
						model.armed = False
						arrowButton = Nothing
					End If
				End If
			End Sub
		End Class


		Private Class Handler
			Implements LayoutManager, PropertyChangeListener, ChangeListener

			'
			' LayoutManager
			'
			Private nextButton As Component = Nothing
			Private previousButton As Component = Nothing
			Private editor As Component = Nothing

			Public Overridable Sub addLayoutComponent(ByVal name As String, ByVal c As Component)
				If "Next".Equals(name) Then
					nextButton = c
				ElseIf "Previous".Equals(name) Then
					previousButton = c
				ElseIf "Editor".Equals(name) Then
					editor = c
				End If
			End Sub

			Public Overridable Sub removeLayoutComponent(ByVal c As Component)
				If c Is nextButton Then
					nextButton = Nothing
				ElseIf c Is previousButton Then
					previousButton = Nothing
				ElseIf c Is editor Then
					editor = Nothing
				End If
			End Sub

			Private Function preferredSize(ByVal c As Component) As Dimension
				Return If(c Is Nothing, zeroSize, c.preferredSize)
			End Function

			Public Overridable Function preferredLayoutSize(ByVal parent As Container) As Dimension
				Dim nextD As Dimension = preferredSize(nextButton)
				Dim previousD As Dimension = preferredSize(previousButton)
				Dim editorD As Dimension = preferredSize(editor)

	'             Force the editors height to be a multiple of 2
	'             
				editorD.height = ((editorD.height + 1) / 2) * 2

				Dim size As New Dimension(editorD.width, editorD.height)
				size.width += Math.Max(nextD.width, previousD.width)
				Dim insets As Insets = parent.insets
				size.width += insets.left + insets.right
				size.height += insets.top + insets.bottom
				Return size
			End Function

			Public Overridable Function minimumLayoutSize(ByVal parent As Container) As Dimension
				Return preferredLayoutSize(parent)
			End Function

			Private Sub setBounds(ByVal c As Component, ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer)
				If c IsNot Nothing Then c.boundsnds(x, y, width, height)
			End Sub

			Public Overridable Sub layoutContainer(ByVal parent As Container)
				Dim width As Integer = parent.width
				Dim height As Integer = parent.height

				Dim insets As Insets = parent.insets

				If nextButton Is Nothing AndAlso previousButton Is Nothing Then
					boundsnds(editor, insets.left, insets.top, width - insets.left - insets.right, height - insets.top - insets.bottom)

					Return
				End If

				Dim nextD As Dimension = preferredSize(nextButton)
				Dim previousD As Dimension = preferredSize(previousButton)
				Dim buttonsWidth As Integer = Math.Max(nextD.width, previousD.width)
				Dim editorHeight As Integer = height - (insets.top + insets.bottom)

				' The arrowButtonInsets value is used instead of the JSpinner's
				' insets if not null. Defining this to be (0, 0, 0, 0) causes the
				' buttons to be aligned with the outer edge of the spinner's
				' border, and leaving it as "null" places the buttons completely
				' inside the spinner's border.
				Dim buttonInsets As Insets = UIManager.getInsets("Spinner.arrowButtonInsets")
				If buttonInsets Is Nothing Then buttonInsets = insets

	'             Deal with the spinner's componentOrientation property.
	'             
				Dim editorX, editorWidth, buttonsX As Integer
				If parent.componentOrientation.leftToRight Then
					editorX = insets.left
					editorWidth = width - insets.left - buttonsWidth - buttonInsets.right
					buttonsX = width - buttonsWidth - buttonInsets.right
				Else
					buttonsX = buttonInsets.left
					editorX = buttonsX + buttonsWidth
					editorWidth = width - buttonInsets.left - buttonsWidth - insets.right
				End If

				Dim nextY As Integer = buttonInsets.top
				Dim nextHeight As Integer = (height \ 2) + (height Mod 2) - nextY
				Dim previousY As Integer = buttonInsets.top + nextHeight
				Dim previousHeight As Integer = height - previousY - buttonInsets.bottom

				boundsnds(editor, editorX, insets.top, editorWidth, editorHeight)
				boundsnds(nextButton, buttonsX, nextY, buttonsWidth, nextHeight)
				boundsnds(previousButton, buttonsX, previousY, buttonsWidth, previousHeight)
			End Sub


			'
			' PropertyChangeListener
			'
			Public Overridable Sub propertyChange(ByVal e As PropertyChangeEvent)
				Dim propertyName As String = e.propertyName
				If TypeOf e.source Is JSpinner Then
					Dim spinner As JSpinner = CType(e.source, JSpinner)
					Dim spinnerUI As SpinnerUI = spinner.uI

					If TypeOf spinnerUI Is BasicSpinnerUI Then
						Dim ui As BasicSpinnerUI = CType(spinnerUI, BasicSpinnerUI)

						If "editor".Equals(propertyName) Then
							Dim oldEditor As JComponent = CType(e.oldValue, JComponent)
							Dim newEditor As JComponent = CType(e.newValue, JComponent)
							ui.replaceEditor(oldEditor, newEditor)
							ui.updateEnabledState()
							If TypeOf oldEditor Is JSpinner.DefaultEditor Then
								Dim tf As JTextField = CType(oldEditor, JSpinner.DefaultEditor).textField
								If tf IsNot Nothing Then
									tf.removeFocusListener(nextButtonHandler)
									tf.removeFocusListener(previousButtonHandler)
								End If
							End If
							If TypeOf newEditor Is JSpinner.DefaultEditor Then
								Dim tf As JTextField = CType(newEditor, JSpinner.DefaultEditor).textField
								If tf IsNot Nothing Then
									If TypeOf tf.font Is UIResource Then tf.font = spinner.font
									tf.addFocusListener(nextButtonHandler)
									tf.addFocusListener(previousButtonHandler)
								End If
							End If
						ElseIf "enabled".Equals(propertyName) OrElse "model".Equals(propertyName) Then
							ui.updateEnabledState()
					ElseIf "font".Equals(propertyName) Then
						Dim editor As JComponent = spinner.editor
						If editor IsNot Nothing AndAlso TypeOf editor Is JSpinner.DefaultEditor Then
							Dim tf As JTextField = CType(editor, JSpinner.DefaultEditor).textField
							If tf IsNot Nothing Then
								If TypeOf tf.font Is UIResource Then tf.font = spinner.font
							End If
						End If
					ElseIf JComponent.TOOL_TIP_TEXT_KEY.Equals(propertyName) Then
						updateToolTipTextForChildren(spinner)
					End If
					End If
				ElseIf TypeOf e.source Is JComponent Then
					Dim c As JComponent = CType(e.source, JComponent)
					If (TypeOf c.parent Is JPanel) AndAlso (TypeOf c.parent.parent Is JSpinner) AndAlso "border".Equals(propertyName) Then

						Dim spinner As JSpinner = CType(c.parent.parent, JSpinner)
						Dim spinnerUI As SpinnerUI = spinner.uI
						If TypeOf spinnerUI Is BasicSpinnerUI Then
							Dim ui As BasicSpinnerUI = CType(spinnerUI, BasicSpinnerUI)
							ui.maybeRemoveEditorBorder(c)
						End If
					End If
				End If
			End Sub

			' Syncronizes the ToolTip text for the components within the spinner
			' to be the same value as the spinner ToolTip text.
			Private Sub updateToolTipTextForChildren(ByVal spinner As JComponent)
				Dim toolTipText As String = spinner.toolTipText
				Dim children As Component() = spinner.components
				For i As Integer = 0 To children.Length - 1
					If TypeOf children(i) Is JSpinner.DefaultEditor Then
						Dim tf As JTextField = CType(children(i), JSpinner.DefaultEditor).textField
						If tf IsNot Nothing Then tf.toolTipText = toolTipText
					ElseIf TypeOf children(i) Is JComponent Then
						CType(children(i), JComponent).toolTipText = spinner.toolTipText
					End If
				Next i
			End Sub

			Public Overridable Sub stateChanged(ByVal e As ChangeEvent) Implements ChangeListener.stateChanged
				If TypeOf e.source Is JSpinner Then
					Dim spinner As JSpinner = CType(e.source, JSpinner)
					Dim spinnerUI As SpinnerUI = spinner.uI
					If sun.swing.DefaultLookup.getBoolean(spinner, spinnerUI, "Spinner.disableOnBoundaryValues", False) AndAlso TypeOf spinnerUI Is BasicSpinnerUI Then
						Dim ui As BasicSpinnerUI = CType(spinnerUI, BasicSpinnerUI)
						ui.updateEnabledState()
					End If
				End If
			End Sub
		End Class
	End Class

End Namespace
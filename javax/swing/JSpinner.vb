Imports System
Imports javax.swing.event
Imports javax.swing.text
Imports javax.accessibility

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
	''' A single line input field that lets the user select a
	''' number or an object value from an ordered sequence. Spinners typically
	''' provide a pair of tiny arrow buttons for stepping through the elements
	''' of the sequence. The keyboard up/down arrow keys also cycle through the
	''' elements. The user may also be allowed to type a (legal) value directly
	''' into the spinner. Although combo boxes provide similar functionality,
	''' spinners are sometimes preferred because they don't require a drop down list
	''' that can obscure important data.
	''' <p>
	''' A <code>JSpinner</code>'s sequence value is defined by its
	''' <code>SpinnerModel</code>.
	''' The <code>model</code> can be specified as a constructor argument and
	''' changed with the <code>model</code> property.  <code>SpinnerModel</code>
	''' classes for some common types are provided: <code>SpinnerListModel</code>,
	''' <code>SpinnerNumberModel</code>, and <code>SpinnerDateModel</code>.
	''' <p>
	''' A <code>JSpinner</code> has a single child component that's
	''' responsible for displaying
	''' and potentially changing the current element or <i>value</i> of
	''' the model, which is called the <code>editor</code>.  The editor is created
	''' by the <code>JSpinner</code>'s constructor and can be changed with the
	''' <code>editor</code> property.  The <code>JSpinner</code>'s editor stays
	''' in sync with the model by listening for <code>ChangeEvent</code>s. If the
	''' user has changed the value displayed by the <code>editor</code> it is
	''' possible for the <code>model</code>'s value to differ from that of
	''' the <code>editor</code>. To make sure the <code>model</code> has the same
	''' value as the editor use the <code>commitEdit</code> method, eg:
	''' <pre>
	'''   try {
	'''       spinner.commitEdit();
	'''   }
	'''   catch (ParseException pe) {
	'''       // Edited value is invalid, spinner.getValue() will return
	'''       // the last valid value, you could revert the spinner to show that:
	'''       JComponent editor = spinner.getEditor();
	'''       if (editor instanceof DefaultEditor) {
	'''           ((DefaultEditor)editor).getTextField().setValue(spinner.getValue());
	'''       }
	'''       // reset the value to some known value:
	'''       spinner.setValue(fallbackValue);
	'''       // or treat the last valid value as the current, in which
	'''       // case you don't need to do anything.
	'''   }
	'''   return spinner.getValue();
	''' </pre>
	''' <p>
	''' For information and examples of using spinner see
	''' <a href="https://docs.oracle.com/javase/tutorial/uiswing/components/spinner.html">How to Use Spinners</a>,
	''' a section in <em>The Java Tutorial.</em>
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
	''' description: A single line input field that lets the user select a
	'''     number or an object value from an ordered set.
	''' </summary>
	''' <seealso cref= SpinnerModel </seealso>
	''' <seealso cref= AbstractSpinnerModel </seealso>
	''' <seealso cref= SpinnerListModel </seealso>
	''' <seealso cref= SpinnerNumberModel </seealso>
	''' <seealso cref= SpinnerDateModel </seealso>
	''' <seealso cref= JFormattedTextField
	''' 
	''' @author Hans Muller
	''' @author Lynn Monsanto (accessibility)
	''' @since 1.4 </seealso>
	Public Class JSpinner
		Inherits JComponent
		Implements Accessible

		''' <seealso cref= #getUIClassID </seealso>
		''' <seealso cref= #readObject </seealso>
		Private Const uiClassID As String = "SpinnerUI"

		Private Shared ReadOnly DISABLED_ACTION As Action = New DisabledAction

		Private model As SpinnerModel
		Private editor As JComponent
		Private modelListener As ChangeListener
		<NonSerialized> _
		Private changeEvent As ChangeEvent
		Private editorExplicitlySet As Boolean = False


		''' <summary>
		''' Constructs a spinner for the given model. The spinner has
		''' a set of previous/next buttons, and an editor appropriate
		''' for the model.
		''' </summary>
		''' <exception cref="NullPointerException"> if the model is {@code null} </exception>
		Public Sub New(ByVal model As SpinnerModel)
			If model Is Nothing Then Throw New NullPointerException("model cannot be null")
			Me.model = model
			Me.editor = createEditor(model)
			uIPropertyrty("opaque",True)
			updateUI()
		End Sub


		''' <summary>
		''' Constructs a spinner with an <code>Integer SpinnerNumberModel</code>
		''' with initial value 0 and no minimum or maximum limits.
		''' </summary>
		Public Sub New()
			Me.New(New SpinnerNumberModel)
		End Sub


		''' <summary>
		''' Returns the look and feel (L&amp;F) object that renders this component.
		''' </summary>
		''' <returns> the <code>SpinnerUI</code> object that renders this component </returns>
		Public Overridable Property uI As javax.swing.plaf.SpinnerUI
			Get
				Return CType(ui, javax.swing.plaf.SpinnerUI)
			End Get
			Set(ByVal ui As javax.swing.plaf.SpinnerUI)
				MyBase.uI = ui
			End Set
		End Property




		''' <summary>
		''' Returns the suffix used to construct the name of the look and feel
		''' (L&amp;F) class used to render this component.
		''' </summary>
		''' <returns> the string "SpinnerUI" </returns>
		''' <seealso cref= JComponent#getUIClassID </seealso>
		''' <seealso cref= UIDefaults#getUI </seealso>
		Public Property Overrides uIClassID As String
			Get
				Return uiClassID
			End Get
		End Property



		''' <summary>
		''' Resets the UI property with the value from the current look and feel.
		''' </summary>
		''' <seealso cref= UIManager#getUI </seealso>
		Public Overrides Sub updateUI()
			uI = CType(UIManager.getUI(Me), javax.swing.plaf.SpinnerUI)
			invalidate()
		End Sub


		''' <summary>
		''' This method is called by the constructors to create the
		''' <code>JComponent</code>
		''' that displays the current value of the sequence.  The editor may
		''' also allow the user to enter an element of the sequence directly.
		''' An editor must listen for <code>ChangeEvents</code> on the
		''' <code>model</code> and keep the value it displays
		''' in sync with the value of the model.
		''' <p>
		''' Subclasses may override this method to add support for new
		''' <code>SpinnerModel</code> classes.  Alternatively one can just
		''' replace the editor created here with the <code>setEditor</code>
		''' method.  The default mapping from model type to editor is:
		''' <ul>
		''' <li> <code>SpinnerNumberModel =&gt; JSpinner.NumberEditor</code>
		''' <li> <code>SpinnerDateModel =&gt; JSpinner.DateEditor</code>
		''' <li> <code>SpinnerListModel =&gt; JSpinner.ListEditor</code>
		''' <li> <i>all others</i> =&gt; <code>JSpinner.DefaultEditor</code>
		''' </ul>
		''' </summary>
		''' <returns> a component that displays the current value of the sequence </returns>
		''' <param name="model"> the value of getModel </param>
		''' <seealso cref= #getModel </seealso>
		''' <seealso cref= #setEditor </seealso>
		Protected Friend Overridable Function createEditor(ByVal model As SpinnerModel) As JComponent
			If TypeOf model Is SpinnerDateModel Then
				Return New DateEditor(Me)
			ElseIf TypeOf model Is SpinnerListModel Then
				Return New ListEditor(Me)
			ElseIf TypeOf model Is SpinnerNumberModel Then
				Return New NumberEditor(Me)
			Else
				Return New DefaultEditor(Me)
			End If
		End Function


		''' <summary>
		''' Changes the model that represents the value of this spinner.
		''' If the editor property has not been explicitly set,
		''' the editor property is (implicitly) set after the <code>"model"</code>
		''' <code>PropertyChangeEvent</code> has been fired.  The editor
		''' property is set to the value returned by <code>createEditor</code>,
		''' as in:
		''' <pre>
		''' setEditor(createEditor(model));
		''' </pre>
		''' </summary>
		''' <param name="model"> the new <code>SpinnerModel</code> </param>
		''' <seealso cref= #getModel </seealso>
		''' <seealso cref= #getEditor </seealso>
		''' <seealso cref= #setEditor </seealso>
		''' <exception cref="IllegalArgumentException"> if model is <code>null</code>
		''' 
		''' @beaninfo
		'''        bound: true
		'''    attribute: visualUpdate true
		'''  description: Model that represents the value of this spinner. </exception>
		Public Overridable Property model As SpinnerModel
			Set(ByVal model As SpinnerModel)
				If model Is Nothing Then Throw New System.ArgumentException("null model")
				If Not model.Equals(Me.model) Then
					Dim oldModel As SpinnerModel = Me.model
					Me.model = model
					If modelListener IsNot Nothing Then
						oldModel.removeChangeListener(modelListener)
						Me.model.addChangeListener(modelListener)
					End If
					firePropertyChange("model", oldModel, model)
					If Not editorExplicitlySet Then
						editor = createEditor(model) ' sets editorExplicitlySet true
						editorExplicitlySet = False
					End If
					repaint()
					revalidate()
				End If
			End Set
			Get
				Return model
			End Get
		End Property




		''' <summary>
		''' Returns the current value of the model, typically
		''' this value is displayed by the <code>editor</code>. If the
		''' user has changed the value displayed by the <code>editor</code> it is
		''' possible for the <code>model</code>'s value to differ from that of
		''' the <code>editor</code>, refer to the class level javadoc for examples
		''' of how to deal with this.
		''' <p>
		''' This method simply delegates to the <code>model</code>.
		''' It is equivalent to:
		''' <pre>
		''' getModel().getValue()
		''' </pre>
		''' </summary>
		''' <seealso cref= #setValue </seealso>
		''' <seealso cref= SpinnerModel#getValue </seealso>
		Public Overridable Property value As Object
			Get
				Return model.value
			End Get
			Set(ByVal value As Object)
				model.value = value
			End Set
		End Property




		''' <summary>
		''' Returns the object in the sequence that comes after the object returned
		''' by <code>getValue()</code>. If the end of the sequence has been reached
		''' then return <code>null</code>.
		''' Calling this method does not effect <code>value</code>.
		''' <p>
		''' This method simply delegates to the <code>model</code>.
		''' It is equivalent to:
		''' <pre>
		''' getModel().getNextValue()
		''' </pre>
		''' </summary>
		''' <returns> the next legal value or <code>null</code> if one doesn't exist </returns>
		''' <seealso cref= #getValue </seealso>
		''' <seealso cref= #getPreviousValue </seealso>
		''' <seealso cref= SpinnerModel#getNextValue </seealso>
		Public Overridable Property nextValue As Object
			Get
				Return model.nextValue
			End Get
		End Property


		''' <summary>
		''' We pass <code>Change</code> events along to the listeners with the
		''' the slider (instead of the model itself) as the event source.
		''' </summary>
		<Serializable> _
		Private Class ModelListener
			Implements ChangeListener

			Private ReadOnly outerInstance As JSpinner

			Public Sub New(ByVal outerInstance As JSpinner)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Sub stateChanged(ByVal e As ChangeEvent) Implements ChangeListener.stateChanged
				outerInstance.fireStateChanged()
			End Sub
		End Class


		''' <summary>
		''' Adds a listener to the list that is notified each time a change
		''' to the model occurs.  The source of <code>ChangeEvents</code>
		''' delivered to <code>ChangeListeners</code> will be this
		''' <code>JSpinner</code>.  Note also that replacing the model
		''' will not affect listeners added directly to JSpinner.
		''' Applications can add listeners to  the model directly.  In that
		''' case is that the source of the event would be the
		''' <code>SpinnerModel</code>.
		''' </summary>
		''' <param name="listener"> the <code>ChangeListener</code> to add </param>
		''' <seealso cref= #removeChangeListener </seealso>
		''' <seealso cref= #getModel </seealso>
		Public Overridable Sub addChangeListener(ByVal listener As ChangeListener)
			If modelListener Is Nothing Then
				modelListener = New ModelListener(Me)
				model.addChangeListener(modelListener)
			End If
			listenerList.add(GetType(ChangeListener), listener)
		End Sub



		''' <summary>
		''' Removes a <code>ChangeListener</code> from this spinner.
		''' </summary>
		''' <param name="listener"> the <code>ChangeListener</code> to remove </param>
		''' <seealso cref= #fireStateChanged </seealso>
		''' <seealso cref= #addChangeListener </seealso>
		Public Overridable Sub removeChangeListener(ByVal listener As ChangeListener)
			listenerList.remove(GetType(ChangeListener), listener)
		End Sub


		''' <summary>
		''' Returns an array of all the <code>ChangeListener</code>s added
		''' to this JSpinner with addChangeListener().
		''' </summary>
		''' <returns> all of the <code>ChangeListener</code>s added or an empty
		'''         array if no listeners have been added
		''' @since 1.4 </returns>
		Public Overridable Property changeListeners As ChangeListener()
			Get
				Return listenerList.getListeners(GetType(ChangeListener))
			End Get
		End Property


		''' <summary>
		''' Sends a <code>ChangeEvent</code>, whose source is this
		''' <code>JSpinner</code>, to each <code>ChangeListener</code>.
		''' When a <code>ChangeListener</code> has been added
		''' to the spinner, this method method is called each time
		''' a <code>ChangeEvent</code> is received from the model.
		''' </summary>
		''' <seealso cref= #addChangeListener </seealso>
		''' <seealso cref= #removeChangeListener </seealso>
		''' <seealso cref= EventListenerList </seealso>
		Protected Friend Overridable Sub fireStateChanged()
			Dim ___listeners As Object() = listenerList.listenerList
			For i As Integer = ___listeners.Length - 2 To 0 Step -2
				If ___listeners(i) Is GetType(ChangeListener) Then
					If changeEvent Is Nothing Then changeEvent = New ChangeEvent(Me)
					CType(___listeners(i+1), ChangeListener).stateChanged(changeEvent)
				End If
			Next i
		End Sub


		''' <summary>
		''' Returns the object in the sequence that comes
		''' before the object returned by <code>getValue()</code>.
		''' If the end of the sequence has been reached then
		''' return <code>null</code>. Calling this method does
		''' not effect <code>value</code>.
		''' <p>
		''' This method simply delegates to the <code>model</code>.
		''' It is equivalent to:
		''' <pre>
		''' getModel().getPreviousValue()
		''' </pre>
		''' </summary>
		''' <returns> the previous legal value or <code>null</code>
		'''   if one doesn't exist </returns>
		''' <seealso cref= #getValue </seealso>
		''' <seealso cref= #getNextValue </seealso>
		''' <seealso cref= SpinnerModel#getPreviousValue </seealso>
		Public Overridable Property previousValue As Object
			Get
				Return model.previousValue
			End Get
		End Property


		''' <summary>
		''' Changes the <code>JComponent</code> that displays the current value
		''' of the <code>SpinnerModel</code>.  It is the responsibility of this
		''' method to <i>disconnect</i> the old editor from the model and to
		''' connect the new editor.  This may mean removing the
		''' old editors <code>ChangeListener</code> from the model or the
		''' spinner itself and adding one for the new editor.
		''' </summary>
		''' <param name="editor"> the new editor </param>
		''' <seealso cref= #getEditor </seealso>
		''' <seealso cref= #createEditor </seealso>
		''' <seealso cref= #getModel </seealso>
		''' <exception cref="IllegalArgumentException"> if editor is <code>null</code>
		''' 
		''' @beaninfo
		'''        bound: true
		'''    attribute: visualUpdate true
		'''  description: JComponent that displays the current value of the model </exception>
		Public Overridable Property editor As JComponent
			Set(ByVal editor As JComponent)
				If editor Is Nothing Then Throw New System.ArgumentException("null editor")
				If Not editor.Equals(Me.editor) Then
					Dim oldEditor As JComponent = Me.editor
					Me.editor = editor
					If TypeOf oldEditor Is DefaultEditor Then CType(oldEditor, DefaultEditor).dismiss(Me)
					editorExplicitlySet = True
					firePropertyChange("editor", oldEditor, editor)
					revalidate()
					repaint()
				End If
			End Set
			Get
				Return editor
			End Get
		End Property




		''' <summary>
		''' Commits the currently edited value to the <code>SpinnerModel</code>.
		''' <p>
		''' If the editor is an instance of <code>DefaultEditor</code>, the
		''' call if forwarded to the editor, otherwise this does nothing.
		''' </summary>
		''' <exception cref="ParseException"> if the currently edited value couldn't
		'''         be committed. </exception>
		Public Overridable Sub commitEdit()
			Dim ___editor As JComponent = editor
			If TypeOf ___editor Is DefaultEditor Then CType(___editor, DefaultEditor).commitEdit()
		End Sub


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
		''' A simple base class for more specialized editors
		''' that displays a read-only view of the model's current
		''' value with a <code>JFormattedTextField</code>.  Subclasses
		''' can configure the <code>JFormattedTextField</code> to create
		''' an editor that's appropriate for the type of model they
		''' support and they may want to override
		''' the <code>stateChanged</code> and <code>propertyChanged</code>
		''' methods, which keep the model and the text field in sync.
		''' <p>
		''' This class defines a <code>dismiss</code> method that removes the
		''' editors <code>ChangeListener</code> from the <code>JSpinner</code>
		''' that it's part of.   The <code>setEditor</code> method knows about
		''' <code>DefaultEditor.dismiss</code>, so if the developer
		''' replaces an editor that's derived from <code>JSpinner.DefaultEditor</code>
		''' its <code>ChangeListener</code> connection back to the
		''' <code>JSpinner</code> will be removed.  However after that,
		''' it's up to the developer to manage their editor listeners.
		''' Similarly, if a subclass overrides <code>createEditor</code>,
		''' it's up to the subclasser to deal with their editor
		''' subsequently being replaced (with <code>setEditor</code>).
		''' We expect that in most cases, and in editor installed
		''' with <code>setEditor</code> or created by a <code>createEditor</code>
		''' override, will not be replaced anyway.
		''' <p>
		''' This class is the <code>LayoutManager</code> for it's single
		''' <code>JFormattedTextField</code> child.   By default the
		''' child is just centered with the parents insets.
		''' @since 1.4
		''' </summary>
		Public Class DefaultEditor
			Inherits JPanel
			Implements ChangeListener, PropertyChangeListener, LayoutManager

			''' <summary>
			''' Constructs an editor component for the specified <code>JSpinner</code>.
			''' This <code>DefaultEditor</code> is it's own layout manager and
			''' it is added to the spinner's <code>ChangeListener</code> list.
			''' The constructor creates a single <code>JFormattedTextField</code> child,
			''' initializes it's value to be the spinner model's current value
			''' and adds it to <code>this</code> <code>DefaultEditor</code>.
			''' </summary>
			''' <param name="spinner"> the spinner whose model <code>this</code> editor will monitor </param>
			''' <seealso cref= #getTextField </seealso>
			''' <seealso cref= JSpinner#addChangeListener </seealso>
			Public Sub New(ByVal spinner As JSpinner)
				MyBase.New(Nothing)

				Dim ftf As New JFormattedTextField
				ftf.name = "Spinner.formattedTextField"
				ftf.value = spinner.value
				ftf.addPropertyChangeListener(Me)
				ftf.editable = False
				ftf.inheritsPopupMenu = True

				Dim ___toolTipText As String = spinner.toolTipText
				If ___toolTipText IsNot Nothing Then ftf.toolTipText = ___toolTipText

				add(ftf)

				layout = Me
				spinner.addChangeListener(Me)

				' We want the spinner's increment/decrement actions to be
				' active vs those of the JFormattedTextField. As such we
				' put disabled actions in the JFormattedTextField's actionmap.
				' A binding to a disabled action is treated as a nonexistant
				' binding.
				Dim ftfMap As ActionMap = ftf.actionMap

				If ftfMap IsNot Nothing Then
					ftfMap.put("increment", DISABLED_ACTION)
					ftfMap.put("decrement", DISABLED_ACTION)
				End If
			End Sub


			''' <summary>
			''' Disconnect <code>this</code> editor from the specified
			''' <code>JSpinner</code>.  By default, this method removes
			''' itself from the spinners <code>ChangeListener</code> list.
			''' </summary>
			''' <param name="spinner"> the <code>JSpinner</code> to disconnect this
			'''    editor from; the same spinner as was passed to the constructor. </param>
			Public Overridable Sub dismiss(ByVal spinner As JSpinner)
				spinner.removeChangeListener(Me)
			End Sub


			''' <summary>
			''' Returns the <code>JSpinner</code> ancestor of this editor or
			''' <code>null</code> if none of the ancestors are a
			''' <code>JSpinner</code>.
			''' Typically the editor's parent is a <code>JSpinner</code> however
			''' subclasses of <code>JSpinner</code> may override the
			''' the <code>createEditor</code> method and insert one or more containers
			''' between the <code>JSpinner</code> and it's editor.
			''' </summary>
			''' <returns> <code>JSpinner</code> ancestor; <code>null</code>
			'''         if none of the ancestors are a <code>JSpinner</code>
			''' </returns>
			''' <seealso cref= JSpinner#createEditor </seealso>
			Public Overridable Property spinner As JSpinner
				Get
					Dim c As Component = Me
					Do While c IsNot Nothing
						If TypeOf c Is JSpinner Then Return CType(c, JSpinner)
						c = c.parent
					Loop
					Return Nothing
				End Get
			End Property


			''' <summary>
			''' Returns the <code>JFormattedTextField</code> child of this
			''' editor.  By default the text field is the first and only
			''' child of editor.
			''' </summary>
			''' <returns> the <code>JFormattedTextField</code> that gives the user
			'''     access to the <code>SpinnerDateModel's</code> value. </returns>
			''' <seealso cref= #getSpinner </seealso>
			''' <seealso cref= #getModel </seealso>
			Public Overridable Property textField As JFormattedTextField
				Get
					Return CType(getComponent(0), JFormattedTextField)
				End Get
			End Property


			''' <summary>
			''' This method is called when the spinner's model's state changes.
			''' It sets the <code>value</code> of the text field to the current
			''' value of the spinners model.
			''' </summary>
			''' <param name="e"> the <code>ChangeEvent</code> whose source is the
			''' <code>JSpinner</code> whose model has changed. </param>
			''' <seealso cref= #getTextField </seealso>
			''' <seealso cref= JSpinner#getValue </seealso>
			Public Overridable Sub stateChanged(ByVal e As ChangeEvent) Implements ChangeListener.stateChanged
				Dim ___spinner As JSpinner = CType(e.source, JSpinner)
				textField.value = ___spinner.value
			End Sub


			''' <summary>
			''' Called by the <code>JFormattedTextField</code>
			''' <code>PropertyChangeListener</code>.  When the <code>"value"</code>
			''' property changes, which implies that the user has typed a new
			''' number, we set the value of the spinners model.
			''' <p>
			''' This class ignores <code>PropertyChangeEvents</code> whose
			''' source is not the <code>JFormattedTextField</code>, so subclasses
			''' may safely make <code>this</code> <code>DefaultEditor</code> a
			''' <code>PropertyChangeListener</code> on other objects.
			''' </summary>
			''' <param name="e"> the <code>PropertyChangeEvent</code> whose source is
			'''    the <code>JFormattedTextField</code> created by this class. </param>
			''' <seealso cref= #getTextField </seealso>
			Public Overridable Sub propertyChange(ByVal e As PropertyChangeEvent)
				Dim ___spinner As JSpinner = spinner

				If ___spinner Is Nothing Then Return

				Dim source As Object = e.source
				Dim name As String = e.propertyName
				If (TypeOf source Is JFormattedTextField) AndAlso "value".Equals(name) Then
					Dim lastValue As Object = ___spinner.value

					' Try to set the new value
					Try
						___spinner.value = textField.value
					Catch iae As System.ArgumentException
						' SpinnerModel didn't like new value, reset
						Try
							CType(source, JFormattedTextField).value = lastValue
						Catch iae2 As System.ArgumentException
							' Still bogus, nothing else we can do, the
							' SpinnerModel and JFormattedTextField are now out
							' of sync.
						End Try
					End Try
				End If
			End Sub


			''' <summary>
			''' This <code>LayoutManager</code> method does nothing.  We're
			''' only managing a single child and there's no support
			''' for layout constraints.
			''' </summary>
			''' <param name="name"> ignored </param>
			''' <param name="child"> ignored </param>
			Public Overridable Sub addLayoutComponent(ByVal name As String, ByVal child As Component)
			End Sub


			''' <summary>
			''' This <code>LayoutManager</code> method does nothing.  There
			''' isn't any per-child state.
			''' </summary>
			''' <param name="child"> ignored </param>
			Public Overridable Sub removeLayoutComponent(ByVal child As Component)
			End Sub


			''' <summary>
			''' Returns the size of the parents insets.
			''' </summary>
			Private Function insetSize(ByVal parent As Container) As Dimension
				Dim ___insets As Insets = parent.insets
				Dim w As Integer = ___insets.left + ___insets.right
				Dim h As Integer = ___insets.top + ___insets.bottom
				Return New Dimension(w, h)
			End Function


			''' <summary>
			''' Returns the preferred size of first (and only) child plus the
			''' size of the parents insets.
			''' </summary>
			''' <param name="parent"> the Container that's managing the layout </param>
			''' <returns> the preferred dimensions to lay out the subcomponents
			'''          of the specified container. </returns>
			Public Overridable Function preferredLayoutSize(ByVal parent As Container) As Dimension
				Dim ___preferredSize As Dimension = insetSize(parent)
				If parent.componentCount > 0 Then
					Dim childSize As Dimension = getComponent(0).preferredSize
					___preferredSize.width += childSize.width
					___preferredSize.height += childSize.height
				End If
				Return ___preferredSize
			End Function


			''' <summary>
			''' Returns the minimum size of first (and only) child plus the
			''' size of the parents insets.
			''' </summary>
			''' <param name="parent"> the Container that's managing the layout </param>
			''' <returns>  the minimum dimensions needed to lay out the subcomponents
			'''          of the specified container. </returns>
			Public Overridable Function minimumLayoutSize(ByVal parent As Container) As Dimension
				Dim ___minimumSize As Dimension = insetSize(parent)
				If parent.componentCount > 0 Then
					Dim childSize As Dimension = getComponent(0).minimumSize
					___minimumSize.width += childSize.width
					___minimumSize.height += childSize.height
				End If
				Return ___minimumSize
			End Function


			''' <summary>
			''' Resize the one (and only) child to completely fill the area
			''' within the parents insets.
			''' </summary>
			Public Overridable Sub layoutContainer(ByVal parent As Container)
				If parent.componentCount > 0 Then
					Dim ___insets As Insets = parent.insets
					Dim w As Integer = parent.width - (___insets.left + ___insets.right)
					Dim h As Integer = parent.height - (___insets.top + ___insets.bottom)
					getComponent(0).boundsnds(___insets.left, ___insets.top, w, h)
				End If
			End Sub

			''' <summary>
			''' Pushes the currently edited value to the <code>SpinnerModel</code>.
			''' <p>
			''' The default implementation invokes <code>commitEdit</code> on the
			''' <code>JFormattedTextField</code>.
			''' </summary>
			''' <exception cref="ParseException"> if the edited value is not legal </exception>
			Public Overridable Sub commitEdit()
				' If the value in the JFormattedTextField is legal, this will have
				' the result of pushing the value to the SpinnerModel
				' by way of the <code>propertyChange</code> method.
				Dim ftf As JFormattedTextField = textField

				ftf.commitEdit()
			End Sub

			''' <summary>
			''' Returns the baseline.
			''' </summary>
			''' <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
			''' <seealso cref= javax.swing.JComponent#getBaseline(int,int) </seealso>
			''' <seealso cref= javax.swing.JComponent#getBaselineResizeBehavior()
			''' @since 1.6 </seealso>
			Public Overrides Function getBaseline(ByVal width As Integer, ByVal height As Integer) As Integer
				' check size.
				MyBase.getBaseline(width, height)
				Dim ___insets As Insets = insets
				width = width - ___insets.left - ___insets.right
				height = height - ___insets.top - ___insets.bottom
				Dim ___baseline As Integer = getComponent(0).getBaseline(width, height)
				If ___baseline >= 0 Then Return ___baseline + ___insets.top
				Return -1
			End Function

			''' <summary>
			''' Returns an enum indicating how the baseline of the component
			''' changes as the size changes.
			''' </summary>
			''' <exception cref="NullPointerException"> {@inheritDoc} </exception>
			''' <seealso cref= javax.swing.JComponent#getBaseline(int, int)
			''' @since 1.6 </seealso>
			Public Property Overrides baselineResizeBehavior As BaselineResizeBehavior
				Get
					Return getComponent(0).baselineResizeBehavior
				End Get
			End Property
		End Class




		''' <summary>
		''' This subclass of javax.swing.DateFormatter maps the minimum/maximum
		''' properties to te start/end properties of a SpinnerDateModel.
		''' </summary>
		Private Class DateEditorFormatter
			Inherits DateFormatter

			Private ReadOnly model As SpinnerDateModel

			Friend Sub New(ByVal model As SpinnerDateModel, ByVal format As DateFormat)
				MyBase.New(format)
				Me.model = model
			End Sub

			Public Overrides Property minimum As IComparable
				Set(ByVal min As IComparable)
					model.start = min
				End Set
				Get
					Return model.start
				End Get
			End Property


			Public Overrides Property maximum As IComparable
				Set(ByVal max As IComparable)
					model.end = max
				End Set
				Get
					Return model.end
				End Get
			End Property

		End Class


		''' <summary>
		''' An editor for a <code>JSpinner</code> whose model is a
		''' <code>SpinnerDateModel</code>.  The value of the editor is
		''' displayed with a <code>JFormattedTextField</code> whose format
		''' is defined by a <code>DateFormatter</code> instance whose
		''' <code>minimum</code> and <code>maximum</code> properties
		''' are mapped to the <code>SpinnerDateModel</code>.
		''' @since 1.4
		''' </summary>
		' PENDING(hmuller): more example javadoc
		Public Class DateEditor
			Inherits DefaultEditor

			' This is here until SimpleDateFormat gets a constructor that
			' takes a Locale: 4923525
			Private Shared Function getDefaultPattern(ByVal loc As Locale) As String
				Dim adapter As sun.util.locale.provider.LocaleProviderAdapter = sun.util.locale.provider.LocaleProviderAdapter.getAdapter(GetType(java.text.spi.DateFormatProvider), loc)
				Dim lr As sun.util.locale.provider.LocaleResources = adapter.getLocaleResources(loc)
				If lr Is Nothing Then lr = sun.util.locale.provider.LocaleProviderAdapter.forJRE().getLocaleResources(loc)
				Return lr.getDateTimePattern(DateFormat.SHORT, DateFormat.SHORT, Nothing)
			End Function

			''' <summary>
			''' Construct a <code>JSpinner</code> editor that supports displaying
			''' and editing the value of a <code>SpinnerDateModel</code>
			''' with a <code>JFormattedTextField</code>.  <code>This</code>
			''' <code>DateEditor</code> becomes both a <code>ChangeListener</code>
			''' on the spinners model and a <code>PropertyChangeListener</code>
			''' on the new <code>JFormattedTextField</code>.
			''' </summary>
			''' <param name="spinner"> the spinner whose model <code>this</code> editor will monitor </param>
			''' <exception cref="IllegalArgumentException"> if the spinners model is not
			'''     an instance of <code>SpinnerDateModel</code>
			''' </exception>
			''' <seealso cref= #getModel </seealso>
			''' <seealso cref= #getFormat </seealso>
			''' <seealso cref= SpinnerDateModel </seealso>
			Public Sub New(ByVal spinner As JSpinner)
				Me.New(spinner, getDefaultPattern(spinner.locale))
			End Sub


			''' <summary>
			''' Construct a <code>JSpinner</code> editor that supports displaying
			''' and editing the value of a <code>SpinnerDateModel</code>
			''' with a <code>JFormattedTextField</code>.  <code>This</code>
			''' <code>DateEditor</code> becomes both a <code>ChangeListener</code>
			''' on the spinner and a <code>PropertyChangeListener</code>
			''' on the new <code>JFormattedTextField</code>.
			''' </summary>
			''' <param name="spinner"> the spinner whose model <code>this</code> editor will monitor </param>
			''' <param name="dateFormatPattern"> the initial pattern for the
			'''     <code>SimpleDateFormat</code> object that's used to display
			'''     and parse the value of the text field. </param>
			''' <exception cref="IllegalArgumentException"> if the spinners model is not
			'''     an instance of <code>SpinnerDateModel</code>
			''' </exception>
			''' <seealso cref= #getModel </seealso>
			''' <seealso cref= #getFormat </seealso>
			''' <seealso cref= SpinnerDateModel </seealso>
			''' <seealso cref= java.text.SimpleDateFormat </seealso>
			Public Sub New(ByVal spinner As JSpinner, ByVal dateFormatPattern As String)
				Me.New(spinner, New SimpleDateFormat(dateFormatPattern, spinner.locale))
			End Sub

			''' <summary>
			''' Construct a <code>JSpinner</code> editor that supports displaying
			''' and editing the value of a <code>SpinnerDateModel</code>
			''' with a <code>JFormattedTextField</code>.  <code>This</code>
			''' <code>DateEditor</code> becomes both a <code>ChangeListener</code>
			''' on the spinner and a <code>PropertyChangeListener</code>
			''' on the new <code>JFormattedTextField</code>.
			''' </summary>
			''' <param name="spinner"> the spinner whose model <code>this</code> editor
			'''        will monitor </param>
			''' <param name="format"> <code>DateFormat</code> object that's used to display
			'''     and parse the value of the text field. </param>
			''' <exception cref="IllegalArgumentException"> if the spinners model is not
			'''     an instance of <code>SpinnerDateModel</code>
			''' </exception>
			''' <seealso cref= #getModel </seealso>
			''' <seealso cref= #getFormat </seealso>
			''' <seealso cref= SpinnerDateModel </seealso>
			''' <seealso cref= java.text.SimpleDateFormat </seealso>
			Private Sub New(ByVal spinner As JSpinner, ByVal format As DateFormat)
				MyBase.New(spinner)
				If Not(TypeOf spinner.model Is SpinnerDateModel) Then Throw New System.ArgumentException("model not a SpinnerDateModel")

				Dim ___model As SpinnerDateModel = CType(spinner.model, SpinnerDateModel)
				Dim formatter As DateFormatter = New DateEditorFormatter(___model, format)
				Dim factory As New DefaultFormatterFactory(formatter)
				Dim ftf As JFormattedTextField = textField
				ftf.editable = True
				ftf.formatterFactory = factory

	'             TBD - initializing the column width of the text field
	'             * is imprecise and doing it here is tricky because
	'             * the developer may configure the formatter later.
	'             
				Try
					Dim maxString As String = formatter.valueToString(___model.start)
					Dim minString As String = formatter.valueToString(___model.end)
					ftf.columns = Math.Max(maxString.Length, minString.Length)
				Catch e As ParseException
					' PENDING: hmuller
				End Try
			End Sub

			''' <summary>
			''' Returns the <code>java.text.SimpleDateFormat</code> object the
			''' <code>JFormattedTextField</code> uses to parse and format
			''' numbers.
			''' </summary>
			''' <returns> the value of <code>getTextField().getFormatter().getFormat()</code>. </returns>
			''' <seealso cref= #getTextField </seealso>
			''' <seealso cref= java.text.SimpleDateFormat </seealso>
			Public Overridable Property format As SimpleDateFormat
				Get
					Return CType(CType(textField.formatter, DateFormatter).format, SimpleDateFormat)
				End Get
			End Property


			''' <summary>
			''' Return our spinner ancestor's <code>SpinnerDateModel</code>.
			''' </summary>
			''' <returns> <code>getSpinner().getModel()</code> </returns>
			''' <seealso cref= #getSpinner </seealso>
			''' <seealso cref= #getTextField </seealso>
			Public Overridable Property model As SpinnerDateModel
				Get
					Return CType(spinner.model, SpinnerDateModel)
				End Get
			End Property
		End Class


		''' <summary>
		''' This subclass of javax.swing.NumberFormatter maps the minimum/maximum
		''' properties to a SpinnerNumberModel and initializes the valueClass
		''' of the NumberFormatter to match the type of the initial models value.
		''' </summary>
		Private Class NumberEditorFormatter
			Inherits NumberFormatter

			Private ReadOnly model As SpinnerNumberModel

			Friend Sub New(ByVal model As SpinnerNumberModel, ByVal format As NumberFormat)
				MyBase.New(format)
				Me.model = model
				valueClass = model.value.GetType()
			End Sub

			Public Overrides Property minimum As IComparable
				Set(ByVal min As IComparable)
					model.minimum = min
				End Set
				Get
					Return model.minimum
				End Get
			End Property


			Public Overrides Property maximum As IComparable
				Set(ByVal max As IComparable)
					model.maximum = max
				End Set
				Get
					Return model.maximum
				End Get
			End Property

		End Class



		''' <summary>
		''' An editor for a <code>JSpinner</code> whose model is a
		''' <code>SpinnerNumberModel</code>.  The value of the editor is
		''' displayed with a <code>JFormattedTextField</code> whose format
		''' is defined by a <code>NumberFormatter</code> instance whose
		''' <code>minimum</code> and <code>maximum</code> properties
		''' are mapped to the <code>SpinnerNumberModel</code>.
		''' @since 1.4
		''' </summary>
		' PENDING(hmuller): more example javadoc
		Public Class NumberEditor
			Inherits DefaultEditor

			' This is here until DecimalFormat gets a constructor that
			' takes a Locale: 4923525
			Private Shared Function getDefaultPattern(ByVal locale As Locale) As String
				' Get the pattern for the default locale.
				Dim adapter As sun.util.locale.provider.LocaleProviderAdapter
				adapter = sun.util.locale.provider.LocaleProviderAdapter.getAdapter(GetType(java.text.spi.NumberFormatProvider), locale)
				Dim lr As sun.util.locale.provider.LocaleResources = adapter.getLocaleResources(locale)
				If lr Is Nothing Then lr = sun.util.locale.provider.LocaleProviderAdapter.forJRE().getLocaleResources(locale)
				Dim all As String() = lr.numberPatterns
				Return all(0)
			End Function

			''' <summary>
			''' Construct a <code>JSpinner</code> editor that supports displaying
			''' and editing the value of a <code>SpinnerNumberModel</code>
			''' with a <code>JFormattedTextField</code>.  <code>This</code>
			''' <code>NumberEditor</code> becomes both a <code>ChangeListener</code>
			''' on the spinner and a <code>PropertyChangeListener</code>
			''' on the new <code>JFormattedTextField</code>.
			''' </summary>
			''' <param name="spinner"> the spinner whose model <code>this</code> editor will monitor </param>
			''' <exception cref="IllegalArgumentException"> if the spinners model is not
			'''     an instance of <code>SpinnerNumberModel</code>
			''' </exception>
			''' <seealso cref= #getModel </seealso>
			''' <seealso cref= #getFormat </seealso>
			''' <seealso cref= SpinnerNumberModel </seealso>
			Public Sub New(ByVal spinner As JSpinner)
				Me.New(spinner, getDefaultPattern(spinner.locale))
			End Sub

			''' <summary>
			''' Construct a <code>JSpinner</code> editor that supports displaying
			''' and editing the value of a <code>SpinnerNumberModel</code>
			''' with a <code>JFormattedTextField</code>.  <code>This</code>
			''' <code>NumberEditor</code> becomes both a <code>ChangeListener</code>
			''' on the spinner and a <code>PropertyChangeListener</code>
			''' on the new <code>JFormattedTextField</code>.
			''' </summary>
			''' <param name="spinner"> the spinner whose model <code>this</code> editor will monitor </param>
			''' <param name="decimalFormatPattern"> the initial pattern for the
			'''     <code>DecimalFormat</code> object that's used to display
			'''     and parse the value of the text field. </param>
			''' <exception cref="IllegalArgumentException"> if the spinners model is not
			'''     an instance of <code>SpinnerNumberModel</code> or if
			'''     <code>decimalFormatPattern</code> is not a legal
			'''     argument to <code>DecimalFormat</code>
			''' </exception>
			''' <seealso cref= #getTextField </seealso>
			''' <seealso cref= SpinnerNumberModel </seealso>
			''' <seealso cref= java.text.DecimalFormat </seealso>
			Public Sub New(ByVal spinner As JSpinner, ByVal decimalFormatPattern As String)
				Me.New(spinner, New DecimalFormat(decimalFormatPattern))
			End Sub


			''' <summary>
			''' Construct a <code>JSpinner</code> editor that supports displaying
			''' and editing the value of a <code>SpinnerNumberModel</code>
			''' with a <code>JFormattedTextField</code>.  <code>This</code>
			''' <code>NumberEditor</code> becomes both a <code>ChangeListener</code>
			''' on the spinner and a <code>PropertyChangeListener</code>
			''' on the new <code>JFormattedTextField</code>.
			''' </summary>
			''' <param name="spinner"> the spinner whose model <code>this</code> editor will monitor </param>
			''' <param name="decimalFormatPattern"> the initial pattern for the
			'''     <code>DecimalFormat</code> object that's used to display
			'''     and parse the value of the text field. </param>
			''' <exception cref="IllegalArgumentException"> if the spinners model is not
			'''     an instance of <code>SpinnerNumberModel</code>
			''' </exception>
			''' <seealso cref= #getTextField </seealso>
			''' <seealso cref= SpinnerNumberModel </seealso>
			''' <seealso cref= java.text.DecimalFormat </seealso>
			Private Sub New(ByVal spinner As JSpinner, ByVal format As DecimalFormat)
				MyBase.New(spinner)
				If Not(TypeOf spinner.model Is SpinnerNumberModel) Then Throw New System.ArgumentException("model not a SpinnerNumberModel")

				Dim ___model As SpinnerNumberModel = CType(spinner.model, SpinnerNumberModel)
				Dim formatter As NumberFormatter = New NumberEditorFormatter(___model, format)
				Dim factory As New DefaultFormatterFactory(formatter)
				Dim ftf As JFormattedTextField = textField
				ftf.editable = True
				ftf.formatterFactory = factory
				ftf.horizontalAlignment = JTextField.RIGHT

	'             TBD - initializing the column width of the text field
	'             * is imprecise and doing it here is tricky because
	'             * the developer may configure the formatter later.
	'             
				Try
					Dim maxString As String = formatter.valueToString(___model.minimum)
					Dim minString As String = formatter.valueToString(___model.maximum)
					ftf.columns = Math.Max(maxString.Length, minString.Length)
				Catch e As ParseException
					' TBD should throw a chained error here
				End Try

			End Sub


			''' <summary>
			''' Returns the <code>java.text.DecimalFormat</code> object the
			''' <code>JFormattedTextField</code> uses to parse and format
			''' numbers.
			''' </summary>
			''' <returns> the value of <code>getTextField().getFormatter().getFormat()</code>. </returns>
			''' <seealso cref= #getTextField </seealso>
			''' <seealso cref= java.text.DecimalFormat </seealso>
			Public Overridable Property format As DecimalFormat
				Get
					Return CType(CType(textField.formatter, NumberFormatter).format, DecimalFormat)
				End Get
			End Property


			''' <summary>
			''' Return our spinner ancestor's <code>SpinnerNumberModel</code>.
			''' </summary>
			''' <returns> <code>getSpinner().getModel()</code> </returns>
			''' <seealso cref= #getSpinner </seealso>
			''' <seealso cref= #getTextField </seealso>
			Public Overridable Property model As SpinnerNumberModel
				Get
					Return CType(spinner.model, SpinnerNumberModel)
				End Get
			End Property
		End Class


		''' <summary>
		''' An editor for a <code>JSpinner</code> whose model is a
		''' <code>SpinnerListModel</code>.
		''' @since 1.4
		''' </summary>
		Public Class ListEditor
			Inherits DefaultEditor

			''' <summary>
			''' Construct a <code>JSpinner</code> editor that supports displaying
			''' and editing the value of a <code>SpinnerListModel</code>
			''' with a <code>JFormattedTextField</code>.  <code>This</code>
			''' <code>ListEditor</code> becomes both a <code>ChangeListener</code>
			''' on the spinner and a <code>PropertyChangeListener</code>
			''' on the new <code>JFormattedTextField</code>.
			''' </summary>
			''' <param name="spinner"> the spinner whose model <code>this</code> editor will monitor </param>
			''' <exception cref="IllegalArgumentException"> if the spinners model is not
			'''     an instance of <code>SpinnerListModel</code>
			''' </exception>
			''' <seealso cref= #getModel </seealso>
			''' <seealso cref= SpinnerListModel </seealso>
			Public Sub New(ByVal spinner As JSpinner)
				MyBase.New(spinner)
				If Not(TypeOf spinner.model Is SpinnerListModel) Then Throw New System.ArgumentException("model not a SpinnerListModel")
				textField.editable = True
				textField.formatterFactory = New DefaultFormatterFactory(New ListFormatter(Me))
			End Sub

			''' <summary>
			''' Return our spinner ancestor's <code>SpinnerNumberModel</code>.
			''' </summary>
			''' <returns> <code>getSpinner().getModel()</code> </returns>
			''' <seealso cref= #getSpinner </seealso>
			''' <seealso cref= #getTextField </seealso>
			Public Overridable Property model As SpinnerListModel
				Get
					Return CType(spinner.model, SpinnerListModel)
				End Get
			End Property


			''' <summary>
			''' ListFormatter provides completion while text is being input
			''' into the JFormattedTextField. Completion is only done if the
			''' user is inserting text at the end of the document. Completion
			''' is done by way of the SpinnerListModel method findNextMatch.
			''' </summary>
			Private Class ListFormatter
				Inherits JFormattedTextField.AbstractFormatter

				Private ReadOnly outerInstance As JSpinner.ListEditor

				Public Sub New(ByVal outerInstance As JSpinner.ListEditor)
					Me.outerInstance = outerInstance
				End Sub

				Private filter As DocumentFilter

				Public Overridable Function valueToString(ByVal value As Object) As String
					If value Is Nothing Then Return ""
					Return value.ToString()
				End Function

				Public Overridable Function stringToValue(ByVal [string] As String) As Object
					Return [string]
				End Function

				Protected Friend Overridable Property documentFilter As DocumentFilter
					Get
						If filter Is Nothing Then filter = New Filter(Me)
						Return filter
					End Get
				End Property


				Private Class Filter
					Inherits DocumentFilter

					Private ReadOnly outerInstance As JSpinner.ListEditor.ListFormatter

					Public Sub New(ByVal outerInstance As JSpinner.ListEditor.ListFormatter)
						Me.outerInstance = outerInstance
					End Sub

					Public Overrides Sub replace(ByVal fb As FilterBypass, ByVal offset As Integer, ByVal length As Integer, ByVal [string] As String, ByVal attrs As AttributeSet)
						If [string] IsNot Nothing AndAlso (offset + length) = fb.document.length Then
							Dim [next] As Object = model.findNextMatch(fb.document.getText(0, offset) + [string])
							Dim value As String = If([next] IsNot Nothing, [next].ToString(), Nothing)

							If value IsNot Nothing Then
								fb.remove(0, offset + length)
								fb.insertString(0, value, Nothing)
								outerInstance.formattedTextField.select(offset + [string].Length, value.Length)
								Return
							End If
						End If
						MyBase.replace(fb, offset, length, [string], attrs)
					End Sub

					Public Overrides Sub insertString(ByVal fb As FilterBypass, ByVal offset As Integer, ByVal [string] As String, ByVal attr As AttributeSet)
						replace(fb, offset, 0, [string], attr)
					End Sub
				End Class
			End Class
		End Class


		''' <summary>
		''' An Action implementation that is always disabled.
		''' </summary>
		Private Class DisabledAction
			Implements Action

			Public Overridable Function getValue(ByVal key As String) As Object Implements Action.getValue
				Return Nothing
			End Function
			Public Overridable Sub putValue(ByVal key As String, ByVal value As Object) Implements Action.putValue
			End Sub
			Public Overridable Property enabled Implements Action.setEnabled As Boolean
				Set(ByVal b As Boolean)
				End Set
				Get
					Return False
				End Get
			End Property
			Public Overridable Sub addPropertyChangeListener(ByVal l As PropertyChangeListener) Implements Action.addPropertyChangeListener
			End Sub
			Public Overridable Sub removePropertyChangeListener(ByVal l As PropertyChangeListener) Implements Action.removePropertyChangeListener
			End Sub
			Public Overridable Sub actionPerformed(ByVal ae As ActionEvent)
			End Sub
		End Class

		'///////////////
		' Accessibility support
		'//////////////

		''' <summary>
		''' Gets the <code>AccessibleContext</code> for the <code>JSpinner</code>
		''' </summary>
		''' <returns> the <code>AccessibleContext</code> for the <code>JSpinner</code>
		''' @since 1.5 </returns>
		Public Overridable Property accessibleContext As AccessibleContext Implements Accessible.getAccessibleContext
			Get
				If accessibleContext Is Nothing Then accessibleContext = New AccessibleJSpinner(Me)
				Return accessibleContext
			End Get
		End Property

		''' <summary>
		''' <code>AccessibleJSpinner</code> implements accessibility
		''' support for the <code>JSpinner</code> class.
		''' @since 1.5
		''' </summary>
		Protected Friend Class AccessibleJSpinner
			Inherits AccessibleJComponent
			Implements AccessibleValue, AccessibleAction, AccessibleText, AccessibleEditableText, ChangeListener

			Private ReadOnly outerInstance As JSpinner


			Private oldModelValue As Object = Nothing

			''' <summary>
			''' AccessibleJSpinner constructor
			''' </summary>
			Protected Friend Sub New(ByVal outerInstance As JSpinner)
					Me.outerInstance = outerInstance
				' model is guaranteed to be non-null
				oldModelValue = outerInstance.model.value
				outerInstance.addChangeListener(Me)
			End Sub

			''' <summary>
			''' Invoked when the target of the listener has changed its state.
			''' </summary>
			''' <param name="e">  a <code>ChangeEvent</code> object. Must not be null. </param>
			''' <exception cref="NullPointerException"> if the parameter is null. </exception>
			Public Overridable Sub stateChanged(ByVal e As ChangeEvent) Implements ChangeListener.stateChanged
				If e Is Nothing Then Throw New NullPointerException
				Dim newModelValue As Object = outerInstance.model.value
				outerInstance.firePropertyChange(ACCESSIBLE_VALUE_PROPERTY, oldModelValue, newModelValue)
				outerInstance.firePropertyChange(ACCESSIBLE_TEXT_PROPERTY, Nothing, 0) ' entire text may have changed

				oldModelValue = newModelValue
			End Sub

			' ===== Begin AccessibleContext methods ===== 

			''' <summary>
			''' Gets the role of this object.  The role of the object is the generic
			''' purpose or use of the class of this object.  For example, the role
			''' of a push button is AccessibleRole.PUSH_BUTTON.  The roles in
			''' AccessibleRole are provided so component developers can pick from
			''' a set of predefined roles.  This enables assistive technologies to
			''' provide a consistent interface to various tweaked subclasses of
			''' components (e.g., use AccessibleRole.PUSH_BUTTON for all components
			''' that act like a push button) as well as distinguish between subclasses
			''' that behave differently (e.g., AccessibleRole.CHECK_BOX for check boxes
			''' and AccessibleRole.RADIO_BUTTON for radio buttons).
			''' <p>Note that the AccessibleRole class is also extensible, so
			''' custom component developers can define their own AccessibleRole's
			''' if the set of predefined roles is inadequate.
			''' </summary>
			''' <returns> an instance of AccessibleRole describing the role of the object </returns>
			''' <seealso cref= AccessibleRole </seealso>
			Public Overridable Property accessibleRole As AccessibleRole
				Get
					Return AccessibleRole.SPIN_BOX
				End Get
			End Property

			''' <summary>
			''' Returns the number of accessible children of the object.
			''' </summary>
			''' <returns> the number of accessible children of the object. </returns>
			Public Overridable Property accessibleChildrenCount As Integer
				Get
					' the JSpinner has one child, the editor
					If outerInstance.editor.accessibleContext IsNot Nothing Then Return 1
					Return 0
				End Get
			End Property

			''' <summary>
			''' Returns the specified Accessible child of the object.  The Accessible
			''' children of an Accessible object are zero-based, so the first child
			''' of an Accessible child is at index 0, the second child is at index 1,
			''' and so on.
			''' </summary>
			''' <param name="i"> zero-based index of child </param>
			''' <returns> the Accessible child of the object </returns>
			''' <seealso cref= #getAccessibleChildrenCount </seealso>
			Public Overridable Function getAccessibleChild(ByVal i As Integer) As Accessible
				' the JSpinner has one child, the editor
				If i <> 0 Then Return Nothing
				If outerInstance.editor.accessibleContext IsNot Nothing Then Return CType(outerInstance.editor, Accessible)
				Return Nothing
			End Function

			' ===== End AccessibleContext methods ===== 

			''' <summary>
			''' Gets the AccessibleAction associated with this object that supports
			''' one or more actions.
			''' </summary>
			''' <returns> AccessibleAction if supported by object; else return null </returns>
			''' <seealso cref= AccessibleAction </seealso>
			Public Overridable Property accessibleAction As AccessibleAction
				Get
					Return Me
				End Get
			End Property

			''' <summary>
			''' Gets the AccessibleText associated with this object presenting
			''' text on the display.
			''' </summary>
			''' <returns> AccessibleText if supported by object; else return null </returns>
			''' <seealso cref= AccessibleText </seealso>
			Public Overridable Property accessibleText As AccessibleText
				Get
					Return Me
				End Get
			End Property

	'        
	'         * Returns the AccessibleContext for the JSpinner editor
	'         
			Private Property editorAccessibleContext As AccessibleContext
				Get
					If TypeOf outerInstance.editor Is DefaultEditor Then
						Dim textField As JTextField = CType(outerInstance.editor, DefaultEditor).textField
						If textField IsNot Nothing Then Return textField.accessibleContext
					ElseIf TypeOf outerInstance.editor Is Accessible Then
						Return outerInstance.editor.accessibleContext
					End If
					Return Nothing
				End Get
			End Property

	'        
	'         * Returns the AccessibleText for the JSpinner editor
	'         
			Private Property editorAccessibleText As AccessibleText
				Get
					Dim ac As AccessibleContext = editorAccessibleContext
					If ac IsNot Nothing Then Return ac.accessibleText
					Return Nothing
				End Get
			End Property

	'        
	'         * Returns the AccessibleEditableText for the JSpinner editor
	'         
			Private Property editorAccessibleEditableText As AccessibleEditableText
				Get
					Dim at As AccessibleText = editorAccessibleText
					If TypeOf at Is AccessibleEditableText Then Return CType(at, AccessibleEditableText)
					Return Nothing
				End Get
			End Property

			''' <summary>
			''' Gets the AccessibleValue associated with this object.
			''' </summary>
			''' <returns> AccessibleValue if supported by object; else return null </returns>
			''' <seealso cref= AccessibleValue
			'''  </seealso>
			Public Overridable Property accessibleValue As AccessibleValue
				Get
					Return Me
				End Get
			End Property

			' ===== Begin AccessibleValue impl ===== 

			''' <summary>
			''' Get the value of this object as a Number.  If the value has not been
			''' set, the return value will be null.
			''' </summary>
			''' <returns> value of the object </returns>
			''' <seealso cref= #setCurrentAccessibleValue </seealso>
			Public Overridable Property currentAccessibleValue As Number Implements AccessibleValue.getCurrentAccessibleValue
				Get
					Dim o As Object = outerInstance.model.value
					If TypeOf o Is Number Then Return CType(o, Number)
					Return Nothing
				End Get
			End Property

			''' <summary>
			''' Set the value of this object as a Number.
			''' </summary>
			''' <param name="n"> the value to set for this object </param>
			''' <returns> true if the value was set; else False </returns>
			''' <seealso cref= #getCurrentAccessibleValue </seealso>
			Public Overridable Function setCurrentAccessibleValue(ByVal n As Number) As Boolean Implements AccessibleValue.setCurrentAccessibleValue
				' try to set the new value
				Try
					outerInstance.model.value = n
					Return True
				Catch iae As System.ArgumentException
					' SpinnerModel didn't like new value
				End Try
				Return False
			End Function

			''' <summary>
			''' Get the minimum value of this object as a Number.
			''' </summary>
			''' <returns> Minimum value of the object; null if this object does not
			''' have a minimum value </returns>
			''' <seealso cref= #getMaximumAccessibleValue </seealso>
			Public Overridable Property minimumAccessibleValue As Number Implements AccessibleValue.getMinimumAccessibleValue
				Get
					If TypeOf outerInstance.model Is SpinnerNumberModel Then
						Dim numberModel As SpinnerNumberModel = CType(outerInstance.model, SpinnerNumberModel)
						Dim o As Object = numberModel.minimum
						If TypeOf o Is Number Then Return CType(o, Number)
					End If
					Return Nothing
				End Get
			End Property

			''' <summary>
			''' Get the maximum value of this object as a Number.
			''' </summary>
			''' <returns> Maximum value of the object; null if this object does not
			''' have a maximum value </returns>
			''' <seealso cref= #getMinimumAccessibleValue </seealso>
			Public Overridable Property maximumAccessibleValue As Number Implements AccessibleValue.getMaximumAccessibleValue
				Get
					If TypeOf outerInstance.model Is SpinnerNumberModel Then
						Dim numberModel As SpinnerNumberModel = CType(outerInstance.model, SpinnerNumberModel)
						Dim o As Object = numberModel.maximum
						If TypeOf o Is Number Then Return CType(o, Number)
					End If
					Return Nothing
				End Get
			End Property

			' ===== End AccessibleValue impl ===== 

			' ===== Begin AccessibleAction impl ===== 

			''' <summary>
			''' Returns the number of accessible actions available in this object
			''' If there are more than one, the first one is considered the "default"
			''' action of the object.
			''' 
			''' Two actions are supported: AccessibleAction.INCREMENT which
			''' increments the spinner value and AccessibleAction.DECREMENT
			''' which decrements the spinner value
			''' </summary>
			''' <returns> the zero-based number of Actions in this object </returns>
			Public Overridable Property accessibleActionCount As Integer Implements AccessibleAction.getAccessibleActionCount
				Get
					Return 2
				End Get
			End Property

			''' <summary>
			''' Returns a description of the specified action of the object.
			''' </summary>
			''' <param name="i"> zero-based index of the actions </param>
			''' <returns> a String description of the action </returns>
			''' <seealso cref= #getAccessibleActionCount </seealso>
			Public Overridable Function getAccessibleActionDescription(ByVal i As Integer) As String Implements AccessibleAction.getAccessibleActionDescription
				If i = 0 Then
					Return AccessibleAction.INCREMENT
				ElseIf i = 1 Then
					Return AccessibleAction.DECREMENT
				End If
				Return Nothing
			End Function

			''' <summary>
			''' Performs the specified Action on the object
			''' </summary>
			''' <param name="i"> zero-based index of actions. The first action
			''' (index 0) is AccessibleAction.INCREMENT and the second
			''' action (index 1) is AccessibleAction.DECREMENT. </param>
			''' <returns> true if the action was performed; otherwise false. </returns>
			''' <seealso cref= #getAccessibleActionCount </seealso>
			Public Overridable Function doAccessibleAction(ByVal i As Integer) As Boolean Implements AccessibleAction.doAccessibleAction
				If i < 0 OrElse i > 1 Then Return False
				Dim o As Object
				If i = 0 Then
					o = outerInstance.nextValue ' AccessibleAction.INCREMENT
				Else
					o = outerInstance.previousValue ' AccessibleAction.DECREMENT
				End If
				' try to set the new value
				Try
					outerInstance.model.value = o
					Return True
				Catch iae As System.ArgumentException
					' SpinnerModel didn't like new value
				End Try
				Return False
			End Function

			' ===== End AccessibleAction impl ===== 

			' ===== Begin AccessibleText impl ===== 

	'        
	'         * Returns whether source and destination components have the
	'         * same window ancestor
	'         
			Private Function sameWindowAncestor(ByVal src As Component, ByVal dest As Component) As Boolean
				If src Is Nothing OrElse dest Is Nothing Then Return False
				Return SwingUtilities.getWindowAncestor(src) Is SwingUtilities.getWindowAncestor(dest)
			End Function

			''' <summary>
			''' Given a point in local coordinates, return the zero-based index
			''' of the character under that Point.  If the point is invalid,
			''' this method returns -1.
			''' </summary>
			''' <param name="p"> the Point in local coordinates </param>
			''' <returns> the zero-based index of the character under Point p; if
			''' Point is invalid return -1. </returns>
			Public Overridable Function getIndexAtPoint(ByVal p As Point) As Integer Implements AccessibleText.getIndexAtPoint
				Dim at As AccessibleText = editorAccessibleText
				If at IsNot Nothing AndAlso sameWindowAncestor(JSpinner.this, outerInstance.editor) Then
					' convert point from the JSpinner bounds (source) to
					' editor bounds (destination)
					Dim editorPoint As Point = SwingUtilities.convertPoint(JSpinner.this, p, outerInstance.editor)
					If editorPoint IsNot Nothing Then Return at.getIndexAtPoint(editorPoint)
				End If
				Return -1
			End Function

			''' <summary>
			''' Determines the bounding box of the character at the given
			''' index into the string.  The bounds are returned in local
			''' coordinates.  If the index is invalid an empty rectangle is
			''' returned.
			''' </summary>
			''' <param name="i"> the index into the String </param>
			''' <returns> the screen coordinates of the character's bounding box,
			''' if index is invalid return an empty rectangle. </returns>
			Public Overridable Function getCharacterBounds(ByVal i As Integer) As Rectangle Implements AccessibleText.getCharacterBounds
				Dim at As AccessibleText = editorAccessibleText
				If at IsNot Nothing Then
					Dim editorRect As Rectangle = at.getCharacterBounds(i)
					If editorRect IsNot Nothing AndAlso sameWindowAncestor(JSpinner.this, outerInstance.editor) Then Return SwingUtilities.convertRectangle(outerInstance.editor, editorRect, JSpinner.this)
				End If
				Return Nothing
			End Function

			''' <summary>
			''' Returns the number of characters (valid indicies)
			''' </summary>
			''' <returns> the number of characters </returns>
			Public Overridable Property charCount As Integer Implements AccessibleText.getCharCount
				Get
					Dim at As AccessibleText = editorAccessibleText
					If at IsNot Nothing Then Return at.charCount
					Return -1
				End Get
			End Property

			''' <summary>
			''' Returns the zero-based offset of the caret.
			''' 
			''' Note: That to the right of the caret will have the same index
			''' value as the offset (the caret is between two characters). </summary>
			''' <returns> the zero-based offset of the caret. </returns>
			Public Overridable Property caretPosition As Integer Implements AccessibleText.getCaretPosition
				Get
					Dim at As AccessibleText = editorAccessibleText
					If at IsNot Nothing Then Return at.caretPosition
					Return -1
				End Get
			End Property

			''' <summary>
			''' Returns the String at a given index.
			''' </summary>
			''' <param name="part"> the CHARACTER, WORD, or SENTENCE to retrieve </param>
			''' <param name="index"> an index within the text </param>
			''' <returns> the letter, word, or sentence </returns>
			Public Overridable Function getAtIndex(ByVal part As Integer, ByVal index As Integer) As String Implements AccessibleText.getAtIndex
				Dim at As AccessibleText = editorAccessibleText
				If at IsNot Nothing Then Return at.getAtIndex(part, index)
				Return Nothing
			End Function

			''' <summary>
			''' Returns the String after a given index.
			''' </summary>
			''' <param name="part"> the CHARACTER, WORD, or SENTENCE to retrieve </param>
			''' <param name="index"> an index within the text </param>
			''' <returns> the letter, word, or sentence </returns>
			Public Overridable Function getAfterIndex(ByVal part As Integer, ByVal index As Integer) As String Implements AccessibleText.getAfterIndex
				Dim at As AccessibleText = editorAccessibleText
				If at IsNot Nothing Then Return at.getAfterIndex(part, index)
				Return Nothing
			End Function

			''' <summary>
			''' Returns the String before a given index.
			''' </summary>
			''' <param name="part"> the CHARACTER, WORD, or SENTENCE to retrieve </param>
			''' <param name="index"> an index within the text </param>
			''' <returns> the letter, word, or sentence </returns>
			Public Overridable Function getBeforeIndex(ByVal part As Integer, ByVal index As Integer) As String Implements AccessibleText.getBeforeIndex
				Dim at As AccessibleText = editorAccessibleText
				If at IsNot Nothing Then Return at.getBeforeIndex(part, index)
				Return Nothing
			End Function

			''' <summary>
			''' Returns the AttributeSet for a given character at a given index
			''' </summary>
			''' <param name="i"> the zero-based index into the text </param>
			''' <returns> the AttributeSet of the character </returns>
			Public Overridable Function getCharacterAttribute(ByVal i As Integer) As AttributeSet Implements AccessibleText.getCharacterAttribute
				Dim at As AccessibleText = editorAccessibleText
				If at IsNot Nothing Then Return at.getCharacterAttribute(i)
				Return Nothing
			End Function

			''' <summary>
			''' Returns the start offset within the selected text.
			''' If there is no selection, but there is
			''' a caret, the start and end offsets will be the same.
			''' </summary>
			''' <returns> the index into the text of the start of the selection </returns>
			Public Overridable Property selectionStart As Integer Implements AccessibleText.getSelectionStart
				Get
					Dim at As AccessibleText = editorAccessibleText
					If at IsNot Nothing Then Return at.selectionStart
					Return -1
				End Get
			End Property

			''' <summary>
			''' Returns the end offset within the selected text.
			''' If there is no selection, but there is
			''' a caret, the start and end offsets will be the same.
			''' </summary>
			''' <returns> the index into the text of the end of the selection </returns>
			Public Overridable Property selectionEnd As Integer Implements AccessibleText.getSelectionEnd
				Get
					Dim at As AccessibleText = editorAccessibleText
					If at IsNot Nothing Then Return at.selectionEnd
					Return -1
				End Get
			End Property

			''' <summary>
			''' Returns the portion of the text that is selected.
			''' </summary>
			''' <returns> the String portion of the text that is selected </returns>
			Public Overridable Property selectedText As String Implements AccessibleText.getSelectedText
				Get
					Dim at As AccessibleText = editorAccessibleText
					If at IsNot Nothing Then Return at.selectedText
					Return Nothing
				End Get
			End Property

			' ===== End AccessibleText impl ===== 


			' ===== Begin AccessibleEditableText impl ===== 

			''' <summary>
			''' Sets the text contents to the specified string.
			''' </summary>
			''' <param name="s"> the string to set the text contents </param>
			Public Overridable Property textContents Implements AccessibleEditableText.setTextContents As String
				Set(ByVal s As String)
					Dim at As AccessibleEditableText = editorAccessibleEditableText
					If at IsNot Nothing Then at.textContents = s
				End Set
			End Property

			''' <summary>
			''' Inserts the specified string at the given index/
			''' </summary>
			''' <param name="index"> the index in the text where the string will
			''' be inserted </param>
			''' <param name="s"> the string to insert in the text </param>
			Public Overridable Sub insertTextAtIndex(ByVal index As Integer, ByVal s As String) Implements AccessibleEditableText.insertTextAtIndex
				Dim at As AccessibleEditableText = editorAccessibleEditableText
				If at IsNot Nothing Then at.insertTextAtIndex(index, s)
			End Sub

			''' <summary>
			''' Returns the text string between two indices.
			''' </summary>
			''' <param name="startIndex"> the starting index in the text </param>
			''' <param name="endIndex"> the ending index in the text </param>
			''' <returns> the text string between the indices </returns>
			Public Overridable Function getTextRange(ByVal startIndex As Integer, ByVal endIndex As Integer) As String Implements AccessibleEditableText.getTextRange
				Dim at As AccessibleEditableText = editorAccessibleEditableText
				If at IsNot Nothing Then Return at.getTextRange(startIndex, endIndex)
				Return Nothing
			End Function

			''' <summary>
			''' Deletes the text between two indices
			''' </summary>
			''' <param name="startIndex"> the starting index in the text </param>
			''' <param name="endIndex"> the ending index in the text </param>
			Public Overridable Sub delete(ByVal startIndex As Integer, ByVal endIndex As Integer) Implements AccessibleEditableText.delete
				Dim at As AccessibleEditableText = editorAccessibleEditableText
				If at IsNot Nothing Then at.delete(startIndex, endIndex)
			End Sub

			''' <summary>
			''' Cuts the text between two indices into the system clipboard.
			''' </summary>
			''' <param name="startIndex"> the starting index in the text </param>
			''' <param name="endIndex"> the ending index in the text </param>
			Public Overridable Sub cut(ByVal startIndex As Integer, ByVal endIndex As Integer) Implements AccessibleEditableText.cut
				Dim at As AccessibleEditableText = editorAccessibleEditableText
				If at IsNot Nothing Then at.cut(startIndex, endIndex)
			End Sub

			''' <summary>
			''' Pastes the text from the system clipboard into the text
			''' starting at the specified index.
			''' </summary>
			''' <param name="startIndex"> the starting index in the text </param>
			Public Overridable Sub paste(ByVal startIndex As Integer) Implements AccessibleEditableText.paste
				Dim at As AccessibleEditableText = editorAccessibleEditableText
				If at IsNot Nothing Then at.paste(startIndex)
			End Sub

			''' <summary>
			''' Replaces the text between two indices with the specified
			''' string.
			''' </summary>
			''' <param name="startIndex"> the starting index in the text </param>
			''' <param name="endIndex"> the ending index in the text </param>
			''' <param name="s"> the string to replace the text between two indices </param>
			Public Overridable Sub replaceText(ByVal startIndex As Integer, ByVal endIndex As Integer, ByVal s As String) Implements AccessibleEditableText.replaceText
				Dim at As AccessibleEditableText = editorAccessibleEditableText
				If at IsNot Nothing Then at.replaceText(startIndex, endIndex, s)
			End Sub

			''' <summary>
			''' Selects the text between two indices.
			''' </summary>
			''' <param name="startIndex"> the starting index in the text </param>
			''' <param name="endIndex"> the ending index in the text </param>
			Public Overridable Sub selectText(ByVal startIndex As Integer, ByVal endIndex As Integer) Implements AccessibleEditableText.selectText
				Dim at As AccessibleEditableText = editorAccessibleEditableText
				If at IsNot Nothing Then at.selectText(startIndex, endIndex)
			End Sub

			''' <summary>
			''' Sets attributes for the text between two indices.
			''' </summary>
			''' <param name="startIndex"> the starting index in the text </param>
			''' <param name="endIndex"> the ending index in the text </param>
			''' <param name="as"> the attribute set </param>
			''' <seealso cref= AttributeSet </seealso>
			Public Overridable Sub setAttributes(ByVal startIndex As Integer, ByVal endIndex As Integer, ByVal [as] As AttributeSet) Implements AccessibleEditableText.setAttributes
				Dim at As AccessibleEditableText = editorAccessibleEditableText
				If at IsNot Nothing Then at.attributestes(startIndex, endIndex, [as])
			End Sub
		End Class ' End AccessibleJSpinner
	End Class

End Namespace
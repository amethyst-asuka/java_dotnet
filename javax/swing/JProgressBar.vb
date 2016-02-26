Imports System
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
	''' A component that visually displays the progress of some task.  As the task
	''' progresses towards completion, the progress bar displays the
	''' task's percentage of completion.
	''' This percentage is typically represented visually by a rectangle which
	''' starts out empty and gradually becomes filled in as the task progresses.
	''' In addition, the progress bar can display a textual representation of this
	''' percentage.
	''' <p>
	''' {@code JProgressBar} uses a {@code BoundedRangeModel} as its data model,
	''' with the {@code value} property representing the "current" state of the task,
	''' and the {@code minimum} and {@code maximum} properties representing the
	''' beginning and end points, respectively.
	''' <p>
	''' To indicate that a task of unknown length is executing,
	''' you can put a progress bar into indeterminate mode.
	''' While the bar is in indeterminate mode,
	''' it animates constantly to show that work is occurring.
	''' As soon as you can determine the task's length and amount of progress,
	''' you should update the progress bar's value
	''' and switch it back to determinate mode.
	''' 
	''' <p>
	''' 
	''' Here is an example of creating a progress bar,
	''' where <code>task</code> is an object (representing some piece of work)
	''' which returns information about the progress of the task:
	''' 
	''' <pre>
	''' progressBar = new JProgressBar(0, task.getLengthOfTask());
	''' progressBar.setValue(0);
	''' progressBar.setStringPainted(true);
	''' </pre>
	''' 
	''' Here is an example of querying the current state of the task, and using
	''' the returned value to update the progress bar:
	''' 
	''' <pre>
	''' progressBar.setValue(task.getCurrent());
	''' </pre>
	''' 
	''' Here is an example of putting a progress bar into
	''' indeterminate mode,
	''' and then switching back to determinate mode
	''' once the length of the task is known:
	''' 
	''' <pre>
	''' progressBar = new JProgressBar();
	''' <em>...//when the task of (initially) unknown length begins:</em>
	''' progressBar.setIndeterminate(true);
	''' <em>...//do some work; get length of task...</em>
	''' progressBar.setMaximum(newLength);
	''' progressBar.setValue(newValue);
	''' progressBar.setIndeterminate(false);
	''' </pre>
	''' 
	''' <p>
	''' 
	''' For complete examples and further documentation see
	''' <a href="https://docs.oracle.com/javase/tutorial/uiswing/components/progress.html" target="_top">How to Monitor Progress</a>,
	''' a section in <em>The Java Tutorial.</em>
	''' 
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
	''' <seealso cref= javax.swing.plaf.basic.BasicProgressBarUI </seealso>
	''' <seealso cref= javax.swing.BoundedRangeModel </seealso>
	''' <seealso cref= javax.swing.SwingWorker
	''' 
	''' @beaninfo
	'''      attribute: isContainer false
	'''    description: A component that displays an integer value.
	''' 
	''' @author Michael C. Albers
	''' @author Kathy Walrath </seealso>
	Public Class JProgressBar
		Inherits JComponent
		Implements SwingConstants, Accessible

		''' <seealso cref= #getUIClassID </seealso>
		Private Const uiClassID As String = "ProgressBarUI"

		''' <summary>
		''' Whether the progress bar is horizontal or vertical.
		''' The default is <code>HORIZONTAL</code>.
		''' </summary>
		''' <seealso cref= #setOrientation </seealso>
		Protected Friend orientation As Integer

		''' <summary>
		''' Whether to display a border around the progress bar.
		''' The default is <code>true</code>.
		''' </summary>
		''' <seealso cref= #setBorderPainted </seealso>
		Protected Friend ___paintBorder As Boolean

		''' <summary>
		''' The object that holds the data for the progress bar.
		''' </summary>
		''' <seealso cref= #setModel </seealso>
		Protected Friend model As BoundedRangeModel

		''' <summary>
		''' An optional string that can be displayed on the progress bar.
		''' The default is <code>null</code>. Setting this to a non-<code>null</code>
		''' value does not imply that the string will be displayed.
		''' To display the string, {@code paintString} must be {@code true}.
		''' </summary>
		''' <seealso cref= #setString </seealso>
		''' <seealso cref= #setStringPainted </seealso>
		Protected Friend progressString As String

		''' <summary>
		''' Whether to display a string of text on the progress bar.
		''' The default is <code>false</code>.
		''' Setting this to <code>true</code> causes a textual
		''' display of the progress to be rendered on the progress bar. If
		''' the <code>progressString</code> is <code>null</code>,
		''' the percentage of completion is displayed on the progress bar.
		''' Otherwise, the <code>progressString</code> is
		''' rendered on the progress bar.
		''' </summary>
		''' <seealso cref= #setStringPainted </seealso>
		''' <seealso cref= #setString </seealso>
		Protected Friend paintString As Boolean

		''' <summary>
		''' The default minimum for a progress bar is 0.
		''' </summary>
		Private Const defaultMinimum As Integer = 0
		''' <summary>
		''' The default maximum for a progress bar is 100.
		''' </summary>
		Private Const defaultMaximum As Integer = 100
		''' <summary>
		''' The default orientation for a progress bar is <code>HORIZONTAL</code>.
		''' </summary>
		Private Shared ReadOnly defaultOrientation As Integer = HORIZONTAL

		''' <summary>
		''' Only one <code>ChangeEvent</code> is needed per instance since the
		''' event's only interesting property is the immutable source, which
		''' is the progress bar.
		''' The event is lazily created the first time that an
		''' event notification is fired.
		''' </summary>
		''' <seealso cref= #fireStateChanged </seealso>
		<NonSerialized> _
		Protected Friend changeEvent As ChangeEvent = Nothing

		''' <summary>
		''' Listens for change events sent by the progress bar's model,
		''' redispatching them
		''' to change-event listeners registered upon
		''' this progress bar.
		''' </summary>
		''' <seealso cref= #createChangeListener </seealso>
		Protected Friend changeListener As ChangeListener = Nothing

		''' <summary>
		''' Format used when displaying percent complete.
		''' </summary>
		<NonSerialized> _
		Private format As java.text.Format

		''' <summary>
		''' Whether the progress bar is indeterminate (<code>true</code>) or
		''' normal (<code>false</code>); the default is <code>false</code>.
		''' </summary>
		''' <seealso cref= #setIndeterminate
		''' @since 1.4 </seealso>
		Private indeterminate As Boolean


	   ''' <summary>
	   ''' Creates a horizontal progress bar
	   ''' that displays a border but no progress string.
	   ''' The initial and minimum values are 0,
	   ''' and the maximum is 100.
	   ''' </summary>
	   ''' <seealso cref= #setOrientation </seealso>
	   ''' <seealso cref= #setBorderPainted </seealso>
	   ''' <seealso cref= #setStringPainted </seealso>
	   ''' <seealso cref= #setString </seealso>
	   ''' <seealso cref= #setIndeterminate </seealso>
		Public Sub New()
			Me.New(defaultOrientation)
		End Sub

	   ''' <summary>
	   ''' Creates a progress bar with the specified orientation,
	   ''' which can be
	   ''' either {@code SwingConstants.VERTICAL} or
	   ''' {@code SwingConstants.HORIZONTAL}.
	   ''' By default, a border is painted but a progress string is not.
	   ''' The initial and minimum values are 0,
	   ''' and the maximum is 100.
	   ''' </summary>
	   ''' <param name="orient">  the desired orientation of the progress bar </param>
	   ''' <exception cref="IllegalArgumentException"> if {@code orient} is an illegal value
	   ''' </exception>
	   ''' <seealso cref= #setOrientation </seealso>
	   ''' <seealso cref= #setBorderPainted </seealso>
	   ''' <seealso cref= #setStringPainted </seealso>
	   ''' <seealso cref= #setString </seealso>
	   ''' <seealso cref= #setIndeterminate </seealso>
		Public Sub New(ByVal orient As Integer)
			Me.New(orient, defaultMinimum, defaultMaximum)
		End Sub


		''' <summary>
		''' Creates a horizontal progress bar
		''' with the specified minimum and maximum.
		''' Sets the initial value of the progress bar to the specified minimum.
		''' By default, a border is painted but a progress string is not.
		''' <p>
		''' The <code>BoundedRangeModel</code> that holds the progress bar's data
		''' handles any issues that may arise from improperly setting the
		''' minimum, initial, and maximum values on the progress bar.
		''' See the {@code BoundedRangeModel} documentation for details.
		''' </summary>
		''' <param name="min">  the minimum value of the progress bar </param>
		''' <param name="max">  the maximum value of the progress bar
		''' </param>
		''' <seealso cref= BoundedRangeModel </seealso>
		''' <seealso cref= #setOrientation </seealso>
		''' <seealso cref= #setBorderPainted </seealso>
		''' <seealso cref= #setStringPainted </seealso>
		''' <seealso cref= #setString </seealso>
		''' <seealso cref= #setIndeterminate </seealso>
		Public Sub New(ByVal min As Integer, ByVal max As Integer)
			Me.New(defaultOrientation, min, max)
		End Sub


		''' <summary>
		''' Creates a progress bar using the specified orientation,
		''' minimum, and maximum.
		''' By default, a border is painted but a progress string is not.
		''' Sets the initial value of the progress bar to the specified minimum.
		''' <p>
		''' The <code>BoundedRangeModel</code> that holds the progress bar's data
		''' handles any issues that may arise from improperly setting the
		''' minimum, initial, and maximum values on the progress bar.
		''' See the {@code BoundedRangeModel} documentation for details.
		''' </summary>
		''' <param name="orient">  the desired orientation of the progress bar </param>
		''' <param name="min">  the minimum value of the progress bar </param>
		''' <param name="max">  the maximum value of the progress bar </param>
		''' <exception cref="IllegalArgumentException"> if {@code orient} is an illegal value
		''' </exception>
		''' <seealso cref= BoundedRangeModel </seealso>
		''' <seealso cref= #setOrientation </seealso>
		''' <seealso cref= #setBorderPainted </seealso>
		''' <seealso cref= #setStringPainted </seealso>
		''' <seealso cref= #setString </seealso>
		''' <seealso cref= #setIndeterminate </seealso>
		Public Sub New(ByVal orient As Integer, ByVal min As Integer, ByVal max As Integer)
			' Creating the model this way is a bit simplistic, but
			'  I believe that it is the the most common usage of this
			'  component - it's what people will expect.
			model = New DefaultBoundedRangeModel(min, 0, min, max)
			updateUI()

			orientation = orient ' documented with set/getOrientation()
			borderPainted = True ' documented with is/setBorderPainted()
			stringPainted = False ' see setStringPainted
			[string] = Nothing ' see getString
			indeterminate = False ' see setIndeterminate
		End Sub


		''' <summary>
		''' Creates a horizontal progress bar
		''' that uses the specified model
		''' to hold the progress bar's data.
		''' By default, a border is painted but a progress string is not.
		''' </summary>
		''' <param name="newModel">  the data model for the progress bar
		''' </param>
		''' <seealso cref= #setOrientation </seealso>
		''' <seealso cref= #setBorderPainted </seealso>
		''' <seealso cref= #setStringPainted </seealso>
		''' <seealso cref= #setString </seealso>
		''' <seealso cref= #setIndeterminate </seealso>
		Public Sub New(ByVal newModel As BoundedRangeModel)
			model = newModel
			updateUI()

			orientation = defaultOrientation ' see setOrientation()
			borderPainted = True ' see setBorderPainted()
			stringPainted = False ' see setStringPainted
			[string] = Nothing ' see getString
			indeterminate = False ' see setIndeterminate
		End Sub


		''' <summary>
		''' Returns {@code SwingConstants.VERTICAL} or
		''' {@code SwingConstants.HORIZONTAL}, depending on the orientation
		''' of the progress bar. The default orientation is
		''' {@code SwingConstants.HORIZONTAL}.
		''' </summary>
		''' <returns> <code>HORIZONTAL</code> or <code>VERTICAL</code> </returns>
		''' <seealso cref= #setOrientation </seealso>
		Public Overridable Property orientation As Integer
			Get
				Return orientation
			End Get
			Set(ByVal newOrientation As Integer)
				If orientation <> newOrientation Then
					Select Case newOrientation
					Case VERTICAL, HORIZONTAL
						Dim oldOrientation As Integer = orientation
						orientation = newOrientation
						firePropertyChange("orientation", oldOrientation, newOrientation)
						If accessibleContext IsNot Nothing Then accessibleContext.firePropertyChange(AccessibleContext.ACCESSIBLE_STATE_PROPERTY, (If(oldOrientation = VERTICAL, AccessibleState.VERTICAL, AccessibleState.HORIZONTAL)), (If(orientation = VERTICAL, AccessibleState.VERTICAL, AccessibleState.HORIZONTAL)))
					Case Else
						Throw New System.ArgumentException(newOrientation & " is not a legal orientation")
					End Select
					revalidate()
				End If
			End Set
		End Property




		''' <summary>
		''' Returns the value of the <code>stringPainted</code> property.
		''' </summary>
		''' <returns> the value of the <code>stringPainted</code> property </returns>
		''' <seealso cref=    #setStringPainted </seealso>
		''' <seealso cref=    #setString </seealso>
		Public Overridable Property stringPainted As Boolean
			Get
				Return paintString
			End Get
			Set(ByVal b As Boolean)
				'PENDING: specify that string not painted when in indeterminate mode?
				'         or just leave that to the L&F?
				Dim oldValue As Boolean = paintString
				paintString = b
				firePropertyChange("stringPainted", oldValue, paintString)
				If paintString <> oldValue Then
					revalidate()
					repaint()
				End If
			End Set
		End Property




		''' <summary>
		''' Returns a {@code String} representation of the current progress.
		''' By default, this returns a simple percentage {@code String} based on
		''' the value returned from {@code getPercentComplete}.  An example
		''' would be the "42%".  You can change this by calling {@code setString}.
		''' </summary>
		''' <returns> the value of the progress string, or a simple percentage string
		'''         if the progress string is {@code null} </returns>
		''' <seealso cref=    #setString </seealso>
		Public Overridable Property [string] As String
			Get
				If progressString IsNot Nothing Then
					Return progressString
				Else
					If format Is Nothing Then format = java.text.NumberFormat.percentInstance
					Return format.format(New Double?(percentComplete))
				End If
			End Get
			Set(ByVal s As String)
				Dim oldValue As String = progressString
				progressString = s
				firePropertyChange("string", oldValue, progressString)
				If progressString Is Nothing OrElse oldValue Is Nothing OrElse (Not progressString.Equals(oldValue)) Then repaint()
			End Set
		End Property


		''' <summary>
		''' Returns the percent complete for the progress bar.
		''' Note that this number is between 0.0 and 1.0.
		''' </summary>
		''' <returns> the percent complete for this progress bar </returns>
		Public Overridable Property percentComplete As Double
			Get
				Dim span As Long = model.maximum - model.minimum
				Dim currentValue As Double = model.value
				Dim pc As Double = (currentValue - model.minimum) / span
				Return pc
			End Get
		End Property

		''' <summary>
		''' Returns the <code>borderPainted</code> property.
		''' </summary>
		''' <returns> the value of the <code>borderPainted</code> property </returns>
		''' <seealso cref=    #setBorderPainted
		''' @beaninfo
		'''  description: Does the progress bar paint its border </seealso>
		Public Overridable Property borderPainted As Boolean
			Get
				Return ___paintBorder
			End Get
			Set(ByVal b As Boolean)
				Dim oldValue As Boolean = ___paintBorder
				___paintBorder = b
				firePropertyChange("borderPainted", oldValue, ___paintBorder)
				If ___paintBorder <> oldValue Then repaint()
			End Set
		End Property


		''' <summary>
		''' Paints the progress bar's border if the <code>borderPainted</code>
		''' property is <code>true</code>.
		''' </summary>
		''' <param name="g">  the <code>Graphics</code> context within which to paint the border </param>
		''' <seealso cref= #paint </seealso>
		''' <seealso cref= #setBorder </seealso>
		''' <seealso cref= #isBorderPainted </seealso>
		''' <seealso cref= #setBorderPainted </seealso>
		Protected Friend Overridable Sub paintBorder(ByVal g As java.awt.Graphics)
			If borderPainted Then MyBase.paintBorder(g)
		End Sub


		''' <summary>
		''' Returns the look-and-feel object that renders this component.
		''' </summary>
		''' <returns> the <code>ProgressBarUI</code> object that renders this component </returns>
		Public Overridable Property uI As javax.swing.plaf.ProgressBarUI
			Get
				Return CType(ui, javax.swing.plaf.ProgressBarUI)
			End Get
			Set(ByVal ui As javax.swing.plaf.ProgressBarUI)
				MyBase.uI = ui
			End Set
		End Property



		''' <summary>
		''' Resets the UI property to a value from the current look and feel.
		''' </summary>
		''' <seealso cref= JComponent#updateUI </seealso>
		Public Overrides Sub updateUI()
			uI = CType(UIManager.getUI(Me), javax.swing.plaf.ProgressBarUI)
		End Sub


		''' <summary>
		''' Returns the name of the look-and-feel class that renders this component.
		''' </summary>
		''' <returns> the string "ProgressBarUI" </returns>
		''' <seealso cref= JComponent#getUIClassID </seealso>
		''' <seealso cref= UIDefaults#getUI
		''' @beaninfo
		'''        expert: true
		'''   description: A string that specifies the name of the look-and-feel class. </seealso>
		Public Property Overrides uIClassID As String
			Get
				Return uiClassID
			End Get
		End Property


	'     We pass each Change event to the listeners with the
	'     * the progress bar as the event source.
	'     * <p>
	'     * <strong>Warning:</strong>
	'     * Serialized objects of this class will not be compatible with
	'     * future Swing releases. The current serialization support is
	'     * appropriate for short term storage or RMI between applications running
	'     * the same version of Swing.  As of 1.4, support for long term storage
	'     * of all JavaBeans&trade;
	'     * has been added to the <code>java.beans</code> package.
	'     * Please see {@link java.beans.XMLEncoder}.
	'     
		<Serializable> _
		Private Class ModelListener
			Implements ChangeListener

			Private ReadOnly outerInstance As JProgressBar

			Public Sub New(ByVal outerInstance As JProgressBar)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Sub stateChanged(ByVal e As ChangeEvent) Implements ChangeListener.stateChanged
				outerInstance.fireStateChanged()
			End Sub
		End Class

		''' <summary>
		''' Subclasses that want to handle change events
		''' from the model differently
		''' can override this to return
		''' an instance of a custom <code>ChangeListener</code> implementation.
		''' The default {@code ChangeListener} simply calls the
		''' {@code fireStateChanged} method to forward {@code ChangeEvent}s
		''' to the {@code ChangeListener}s that have been added directly to the
		''' progress bar.
		''' </summary>
		''' <seealso cref= #changeListener </seealso>
		''' <seealso cref= #fireStateChanged </seealso>
		''' <seealso cref= javax.swing.event.ChangeListener </seealso>
		''' <seealso cref= javax.swing.BoundedRangeModel </seealso>
		Protected Friend Overridable Function createChangeListener() As ChangeListener
			Return New ModelListener(Me)
		End Function

		''' <summary>
		''' Adds the specified <code>ChangeListener</code> to the progress bar.
		''' </summary>
		''' <param name="l"> the <code>ChangeListener</code> to add </param>
		Public Overridable Sub addChangeListener(ByVal l As ChangeListener)
			listenerList.add(GetType(ChangeListener), l)
		End Sub

		''' <summary>
		''' Removes a <code>ChangeListener</code> from the progress bar.
		''' </summary>
		''' <param name="l"> the <code>ChangeListener</code> to remove </param>
		Public Overridable Sub removeChangeListener(ByVal l As ChangeListener)
			listenerList.remove(GetType(ChangeListener), l)
		End Sub

		''' <summary>
		''' Returns an array of all the <code>ChangeListener</code>s added
		''' to this progress bar with <code>addChangeListener</code>.
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
		''' Send a {@code ChangeEvent}, whose source is this {@code JProgressBar}, to
		''' all {@code ChangeListener}s that have registered interest in
		''' {@code ChangeEvent}s.
		''' This method is called each time a {@code ChangeEvent} is received from
		''' the model.
		''' <p>
		''' 
		''' The event instance is created if necessary, and stored in
		''' {@code changeEvent}.
		''' </summary>
		''' <seealso cref= #addChangeListener </seealso>
		''' <seealso cref= EventListenerList </seealso>
		Protected Friend Overridable Sub fireStateChanged()
			' Guaranteed to return a non-null array
			Dim ___listeners As Object() = listenerList.listenerList
			' Process the listeners last to first, notifying
			' those that are interested in this event
			For i As Integer = ___listeners.Length-2 To 0 Step -2
				If ___listeners(i) Is GetType(ChangeListener) Then
					' Lazily create the event:
					If changeEvent Is Nothing Then changeEvent = New ChangeEvent(Me)
					CType(___listeners(i+1), ChangeListener).stateChanged(changeEvent)
				End If
			Next i
		End Sub

		''' <summary>
		''' Returns the data model used by this progress bar.
		''' </summary>
		''' <returns> the <code>BoundedRangeModel</code> currently in use </returns>
		''' <seealso cref= #setModel </seealso>
		''' <seealso cref=    BoundedRangeModel </seealso>
		Public Overridable Property model As BoundedRangeModel
			Get
				Return model
			End Get
			Set(ByVal newModel As BoundedRangeModel)
				' PENDING(???) setting the same model to multiple bars is broken; listeners
				Dim oldModel As BoundedRangeModel = model
    
				If newModel IsNot oldModel Then
					If oldModel IsNot Nothing Then
						oldModel.removeChangeListener(changeListener)
						changeListener = Nothing
					End If
    
					model = newModel
    
					If newModel IsNot Nothing Then
						changeListener = createChangeListener()
						newModel.addChangeListener(changeListener)
					End If
    
					If accessibleContext IsNot Nothing Then accessibleContext.firePropertyChange(AccessibleContext.ACCESSIBLE_VALUE_PROPERTY, (If(oldModel Is Nothing, Nothing, Convert.ToInt32(oldModel.value))), (If(newModel Is Nothing, Nothing, Convert.ToInt32(newModel.value))))
    
					If model IsNot Nothing Then model.extent = 0
					repaint()
				End If
			End Set
		End Property



		' All of the model methods are implemented by delegation. 

		''' <summary>
		''' Returns the progress bar's current {@code value}
		''' from the <code>BoundedRangeModel</code>.
		''' The value is always between the
		''' minimum and maximum values, inclusive.
		''' </summary>
		''' <returns>  the current value of the progress bar </returns>
		''' <seealso cref=     #setValue </seealso>
		''' <seealso cref=     BoundedRangeModel#getValue </seealso>
		Public Overridable Property value As Integer
			Get
				Return model.value
			End Get
			Set(ByVal n As Integer)
				Dim brm As BoundedRangeModel = model
				Dim oldValue As Integer = brm.value
				brm.value = n
    
				If accessibleContext IsNot Nothing Then accessibleContext.firePropertyChange(AccessibleContext.ACCESSIBLE_VALUE_PROPERTY, Convert.ToInt32(oldValue), Convert.ToInt32(brm.value))
			End Set
		End Property

		''' <summary>
		''' Returns the progress bar's {@code minimum} value
		''' from the <code>BoundedRangeModel</code>.
		''' </summary>
		''' <returns>  the progress bar's minimum value </returns>
		''' <seealso cref=     #setMinimum </seealso>
		''' <seealso cref=     BoundedRangeModel#getMinimum </seealso>
		Public Overridable Property minimum As Integer
			Get
				Return model.minimum
			End Get
			Set(ByVal n As Integer)
				model.minimum = n
			End Set
		End Property

		''' <summary>
		''' Returns the progress bar's {@code maximum} value
		''' from the <code>BoundedRangeModel</code>.
		''' </summary>
		''' <returns>  the progress bar's maximum value </returns>
		''' <seealso cref=     #setMaximum </seealso>
		''' <seealso cref=     BoundedRangeModel#getMaximum </seealso>
		Public Overridable Property maximum As Integer
			Get
				Return model.maximum
			End Get
			Set(ByVal n As Integer)
				model.maximum = n
			End Set
		End Property




		''' <summary>
		''' Sets the <code>indeterminate</code> property of the progress bar,
		''' which determines whether the progress bar is in determinate
		''' or indeterminate mode.
		''' An indeterminate progress bar continuously displays animation
		''' indicating that an operation of unknown length is occurring.
		''' By default, this property is <code>false</code>.
		''' Some look and feels might not support indeterminate progress bars;
		''' they will ignore this property.
		''' 
		''' <p>
		''' 
		''' See
		''' <a href="https://docs.oracle.com/javase/tutorial/uiswing/components/progress.html" target="_top">How to Monitor Progress</a>
		''' for examples of using indeterminate progress bars.
		''' </summary>
		''' <param name="newValue">  <code>true</code> if the progress bar
		'''                  should change to indeterminate mode;
		'''                  <code>false</code> if it should revert to normal.
		''' </param>
		''' <seealso cref= #isIndeterminate </seealso>
		''' <seealso cref= javax.swing.plaf.basic.BasicProgressBarUI
		''' 
		''' @since 1.4
		''' 
		''' @beaninfo
		'''        bound: true
		'''    attribute: visualUpdate true
		'''  description: Set whether the progress bar is indeterminate (true)
		'''               or normal (false). </seealso>
		Public Overridable Property indeterminate As Boolean
			Set(ByVal newValue As Boolean)
				Dim oldValue As Boolean = indeterminate
				indeterminate = newValue
				firePropertyChange("indeterminate", oldValue, indeterminate)
			End Set
			Get
				Return indeterminate
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


		''' <summary>
		''' Returns a string representation of this <code>JProgressBar</code>.
		''' This method is intended to be used only for debugging purposes. The
		''' content and format of the returned string may vary between
		''' implementations. The returned string may be empty but may not
		''' be <code>null</code>.
		''' </summary>
		''' <returns>  a string representation of this <code>JProgressBar</code> </returns>
		Protected Friend Overrides Function paramString() As String
			Dim orientationString As String = (If(orientation = HORIZONTAL, "HORIZONTAL", "VERTICAL"))
			Dim paintBorderString As String = (If(___paintBorder, "true", "false"))
			Dim progressStringString As String = (If(progressString IsNot Nothing, progressString, ""))
			Dim paintStringString As String = (If(paintString, "true", "false"))
			Dim indeterminateString As String = (If(indeterminate, "true", "false"))

			Return MyBase.paramString() & ",orientation=" & orientationString & ",paintBorder=" & paintBorderString & ",paintString=" & paintStringString & ",progressString=" & progressStringString & ",indeterminateString=" & indeterminateString
		End Function

	'///////////////
	' Accessibility support
	'//////////////

		''' <summary>
		''' Gets the <code>AccessibleContext</code> associated with this
		''' <code>JProgressBar</code>. For progress bars, the
		''' <code>AccessibleContext</code> takes the form of an
		''' <code>AccessibleJProgressBar</code>.
		''' A new <code>AccessibleJProgressBar</code> instance is created if necessary.
		''' </summary>
		''' <returns> an <code>AccessibleJProgressBar</code> that serves as the
		'''         <code>AccessibleContext</code> of this <code>JProgressBar</code>
		''' @beaninfo
		'''       expert: true
		'''  description: The AccessibleContext associated with this ProgressBar. </returns>
		Public Overridable Property accessibleContext As AccessibleContext Implements Accessible.getAccessibleContext
			Get
				If accessibleContext Is Nothing Then accessibleContext = New AccessibleJProgressBar(Me)
				Return accessibleContext
			End Get
		End Property

		''' <summary>
		''' This class implements accessibility support for the
		''' <code>JProgressBar</code> class.  It provides an implementation of the
		''' Java Accessibility API appropriate to progress bar user-interface
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
		Protected Friend Class AccessibleJProgressBar
			Inherits AccessibleJComponent
			Implements AccessibleValue

			Private ReadOnly outerInstance As JProgressBar

			Public Sub New(ByVal outerInstance As JProgressBar)
				Me.outerInstance = outerInstance
			End Sub


			''' <summary>
			''' Gets the state set of this object.
			''' </summary>
			''' <returns> an instance of AccessibleState containing the current state
			''' of the object </returns>
			''' <seealso cref= AccessibleState </seealso>
			Public Overridable Property accessibleStateSet As AccessibleStateSet
				Get
					Dim states As AccessibleStateSet = MyBase.accessibleStateSet
					If outerInstance.model.valueIsAdjusting Then states.add(AccessibleState.BUSY)
					If outerInstance.orientation = VERTICAL Then
						states.add(AccessibleState.VERTICAL)
					Else
						states.add(AccessibleState.HORIZONTAL)
					End If
					Return states
				End Get
			End Property

			''' <summary>
			''' Gets the role of this object.
			''' </summary>
			''' <returns> an instance of AccessibleRole describing the role of the
			''' object </returns>
			Public Overridable Property accessibleRole As AccessibleRole
				Get
					Return AccessibleRole.PROGRESS_BAR
				End Get
			End Property

			''' <summary>
			''' Gets the <code>AccessibleValue</code> associated with this object.  In the
			''' implementation of the Java Accessibility API for this class,
			''' returns this object, which is responsible for implementing the
			''' <code>AccessibleValue</code> interface on behalf of itself.
			''' </summary>
			''' <returns> this object </returns>
			Public Overridable Property accessibleValue As AccessibleValue
				Get
					Return Me
				End Get
			End Property

			''' <summary>
			''' Gets the accessible value of this object.
			''' </summary>
			''' <returns> the current value of this object </returns>
			Public Overridable Property currentAccessibleValue As Number Implements AccessibleValue.getCurrentAccessibleValue
				Get
					Return Convert.ToInt32(outerInstance.value)
				End Get
			End Property

			''' <summary>
			''' Sets the value of this object as a <code>Number</code>.
			''' </summary>
			''' <returns> <code>true</code> if the value was set </returns>
			Public Overridable Function setCurrentAccessibleValue(ByVal n As Number) As Boolean Implements AccessibleValue.setCurrentAccessibleValue
				' TIGER- 4422535
				If n Is Nothing Then Return False
				outerInstance.value = n
				Return True
			End Function

			''' <summary>
			''' Gets the minimum accessible value of this object.
			''' </summary>
			''' <returns> the minimum value of this object </returns>
			Public Overridable Property minimumAccessibleValue As Number Implements AccessibleValue.getMinimumAccessibleValue
				Get
					Return Convert.ToInt32(outerInstance.minimum)
				End Get
			End Property

			''' <summary>
			''' Gets the maximum accessible value of this object.
			''' </summary>
			''' <returns> the maximum value of this object </returns>
			Public Overridable Property maximumAccessibleValue As Number Implements AccessibleValue.getMaximumAccessibleValue
				Get
					' TIGER - 4422362
					Return Convert.ToInt32(outerInstance.model.maximum - outerInstance.model.extent)
				End Get
			End Property

		End Class ' AccessibleJProgressBar
	End Class

End Namespace
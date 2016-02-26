Imports System
Imports System.Collections
Imports javax.swing.event
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
	''' A component that lets the user graphically select a value by sliding
	''' a knob within a bounded interval. The knob is always positioned
	''' at the points that match integer values within the specified interval.
	''' <p>
	''' The slider can show both
	''' major tick marks, and minor tick marks between the major ones.  The number of
	''' values between the tick marks is controlled with
	''' <code>setMajorTickSpacing</code> and <code>setMinorTickSpacing</code>.
	''' Painting of tick marks is controlled by {@code setPaintTicks}.
	''' <p>
	''' Sliders can also print text labels at regular intervals (or at
	''' arbitrary locations) along the slider track.  Painting of labels is
	''' controlled by {@code setLabelTable} and {@code setPaintLabels}.
	''' <p>
	''' For further information and examples see
	''' <a
	''' href="https://docs.oracle.com/javase/tutorial/uiswing/components/slider.html">How to Use Sliders</a>,
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
	'''      attribute: isContainer false
	'''    description: A component that supports selecting a integer value from a range.
	''' 
	''' @author David Kloba
	''' </summary>
	Public Class JSlider
		Inherits JComponent
		Implements SwingConstants, Accessible

		''' <seealso cref= #getUIClassID </seealso>
		''' <seealso cref= #readObject </seealso>
		Private Const uiClassID As String = "SliderUI"

		Private paintTicks As Boolean = False
		Private paintTrack As Boolean = True
		Private paintLabels As Boolean = False
		Private isInverted As Boolean = False

		''' <summary>
		''' The data model that handles the numeric maximum value,
		''' minimum value, and current-position value for the slider.
		''' </summary>
		Protected Friend sliderModel As BoundedRangeModel

		''' <summary>
		''' The number of values between the major tick marks -- the
		''' larger marks that break up the minor tick marks.
		''' </summary>
		Protected Friend majorTickSpacing As Integer

		''' <summary>
		''' The number of values between the minor tick marks -- the
		''' smaller marks that occur between the major tick marks. </summary>
		''' <seealso cref= #setMinorTickSpacing </seealso>
		Protected Friend minorTickSpacing As Integer

		''' <summary>
		''' If true, the knob (and the data value it represents)
		''' resolve to the closest tick mark next to where the user
		''' positioned the knob.  The default is false. </summary>
		''' <seealso cref= #setSnapToTicks </seealso>
		Protected Friend snapToTicks As Boolean = False

		''' <summary>
		''' If true, the knob (and the data value it represents)
		''' resolve to the closest slider value next to where the user
		''' positioned the knob.
		''' </summary>
		Friend snapToValue As Boolean = True

		''' <summary>
		''' Whether the slider is horizontal or vertical
		''' The default is horizontal.
		''' </summary>
		''' <seealso cref= #setOrientation </seealso>
		Protected Friend orientation As Integer


		''' <summary>
		''' {@code Dictionary} of what labels to draw at which values
		''' </summary>
		Private labelTable As Dictionary


		''' <summary>
		''' The changeListener (no suffix) is the listener we add to the
		''' slider's model.  This listener is initialized to the
		''' {@code ChangeListener} returned from {@code createChangeListener},
		''' which by default just forwards events
		''' to {@code ChangeListener}s (if any) added directly to the slider.
		''' </summary>
		''' <seealso cref= #addChangeListener </seealso>
		''' <seealso cref= #createChangeListener </seealso>
		Protected Friend changeListener As ChangeListener = createChangeListener()


		''' <summary>
		''' Only one <code>ChangeEvent</code> is needed per slider instance since the
		''' event's only (read-only) state is the source property.  The source
		''' of events generated here is always "this". The event is lazily
		''' created the first time that an event notification is fired.
		''' </summary>
		''' <seealso cref= #fireStateChanged </seealso>
		<NonSerialized> _
		Protected Friend changeEvent As ChangeEvent = Nothing


		Private Sub checkOrientation(ByVal orientation As Integer)
			Select Case orientation
			Case VERTICAL, HORIZONTAL
			Case Else
				Throw New System.ArgumentException("orientation must be one of: VERTICAL, HORIZONTAL")
			End Select
		End Sub


		''' <summary>
		''' Creates a horizontal slider with the range 0 to 100 and
		''' an initial value of 50.
		''' </summary>
		Public Sub New()
			Me.New(HORIZONTAL, 0, 100, 50)
		End Sub


		''' <summary>
		''' Creates a slider using the specified orientation with the
		''' range {@code 0} to {@code 100} and an initial value of {@code 50}.
		''' The orientation can be
		''' either <code>SwingConstants.VERTICAL</code> or
		''' <code>SwingConstants.HORIZONTAL</code>.
		''' </summary>
		''' <param name="orientation">  the orientation of the slider </param>
		''' <exception cref="IllegalArgumentException"> if orientation is not one of {@code VERTICAL}, {@code HORIZONTAL} </exception>
		''' <seealso cref= #setOrientation </seealso>
		Public Sub New(ByVal orientation As Integer)
			Me.New(orientation, 0, 100, 50)
		End Sub


		''' <summary>
		''' Creates a horizontal slider using the specified min and max
		''' with an initial value equal to the average of the min plus max.
		''' <p>
		''' The <code>BoundedRangeModel</code> that holds the slider's data
		''' handles any issues that may arise from improperly setting the
		''' minimum and maximum values on the slider.  See the
		''' {@code BoundedRangeModel} documentation for details.
		''' </summary>
		''' <param name="min">  the minimum value of the slider </param>
		''' <param name="max">  the maximum value of the slider
		''' </param>
		''' <seealso cref= BoundedRangeModel </seealso>
		''' <seealso cref= #setMinimum </seealso>
		''' <seealso cref= #setMaximum </seealso>
		Public Sub New(ByVal min As Integer, ByVal max As Integer)
			Me.New(HORIZONTAL, min, max, (min + max) \ 2)
		End Sub


		''' <summary>
		''' Creates a horizontal slider using the specified min, max and value.
		''' <p>
		''' The <code>BoundedRangeModel</code> that holds the slider's data
		''' handles any issues that may arise from improperly setting the
		''' minimum, initial, and maximum values on the slider.  See the
		''' {@code BoundedRangeModel} documentation for details.
		''' </summary>
		''' <param name="min">  the minimum value of the slider </param>
		''' <param name="max">  the maximum value of the slider </param>
		''' <param name="value">  the initial value of the slider
		''' </param>
		''' <seealso cref= BoundedRangeModel </seealso>
		''' <seealso cref= #setMinimum </seealso>
		''' <seealso cref= #setMaximum </seealso>
		''' <seealso cref= #setValue </seealso>
		Public Sub New(ByVal min As Integer, ByVal max As Integer, ByVal value As Integer)
			Me.New(HORIZONTAL, min, max, value)
		End Sub


		''' <summary>
		''' Creates a slider with the specified orientation and the
		''' specified minimum, maximum, and initial values.
		''' The orientation can be
		''' either <code>SwingConstants.VERTICAL</code> or
		''' <code>SwingConstants.HORIZONTAL</code>.
		''' <p>
		''' The <code>BoundedRangeModel</code> that holds the slider's data
		''' handles any issues that may arise from improperly setting the
		''' minimum, initial, and maximum values on the slider.  See the
		''' {@code BoundedRangeModel} documentation for details.
		''' </summary>
		''' <param name="orientation">  the orientation of the slider </param>
		''' <param name="min">  the minimum value of the slider </param>
		''' <param name="max">  the maximum value of the slider </param>
		''' <param name="value">  the initial value of the slider
		''' </param>
		''' <exception cref="IllegalArgumentException"> if orientation is not one of {@code VERTICAL}, {@code HORIZONTAL}
		''' </exception>
		''' <seealso cref= BoundedRangeModel </seealso>
		''' <seealso cref= #setOrientation </seealso>
		''' <seealso cref= #setMinimum </seealso>
		''' <seealso cref= #setMaximum </seealso>
		''' <seealso cref= #setValue </seealso>
		Public Sub New(ByVal orientation As Integer, ByVal min As Integer, ByVal max As Integer, ByVal value As Integer)
			checkOrientation(orientation)
			Me.orientation = orientation
			model = New DefaultBoundedRangeModel(value, 0, min, max)
			updateUI()
		End Sub


		''' <summary>
		''' Creates a horizontal slider using the specified
		''' BoundedRangeModel.
		''' </summary>
		Public Sub New(ByVal brm As BoundedRangeModel)
			Me.orientation = JSlider.HORIZONTAL
			model = brm
			updateUI()
		End Sub


		''' <summary>
		''' Gets the UI object which implements the L&amp;F for this component.
		''' </summary>
		''' <returns> the SliderUI object that implements the Slider L&amp;F </returns>
		Public Overridable Property uI As SliderUI
			Get
				Return CType(ui, SliderUI)
			End Get
			Set(ByVal ui As SliderUI)
				MyBase.uI = ui
			End Set
		End Property




		''' <summary>
		''' Resets the UI property to a value from the current look and feel.
		''' </summary>
		''' <seealso cref= JComponent#updateUI </seealso>
		Public Overrides Sub updateUI()
			uI = CType(UIManager.getUI(Me), SliderUI)
			' The labels preferred size may be derived from the font
			' of the slider, so we must update the UI of the slider first, then
			' that of labels.  This way when setSize is called the right
			' font is used.
			updateLabelUIs()
		End Sub


		''' <summary>
		''' Returns the name of the L&amp;F class that renders this component.
		''' </summary>
		''' <returns> "SliderUI" </returns>
		''' <seealso cref= JComponent#getUIClassID </seealso>
		''' <seealso cref= UIDefaults#getUI </seealso>
		Public Property Overrides uIClassID As String
			Get
				Return uiClassID
			End Get
		End Property


		''' <summary>
		''' We pass Change events along to the listeners with the
		''' the slider (instead of the model itself) as the event source.
		''' </summary>
		<Serializable> _
		Private Class ModelListener
			Implements ChangeListener

			Private ReadOnly outerInstance As JSlider

			Public Sub New(ByVal outerInstance As JSlider)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Sub stateChanged(ByVal e As ChangeEvent) Implements ChangeListener.stateChanged
				outerInstance.fireStateChanged()
			End Sub
		End Class


		''' <summary>
		''' Subclasses that want to handle {@code ChangeEvent}s
		''' from the model differently
		''' can override this to return
		''' an instance of a custom <code>ChangeListener</code> implementation.
		''' The default {@code ChangeListener} simply calls the
		''' {@code fireStateChanged} method to forward {@code ChangeEvent}s
		''' to the {@code ChangeListener}s that have been added directly to the
		''' slider. </summary>
		''' <seealso cref= #changeListener </seealso>
		''' <seealso cref= #fireStateChanged </seealso>
		''' <seealso cref= javax.swing.event.ChangeListener </seealso>
		''' <seealso cref= javax.swing.BoundedRangeModel </seealso>
		Protected Friend Overridable Function createChangeListener() As ChangeListener
			Return New ModelListener(Me)
		End Function


		''' <summary>
		''' Adds a ChangeListener to the slider.
		''' </summary>
		''' <param name="l"> the ChangeListener to add </param>
		''' <seealso cref= #fireStateChanged </seealso>
		''' <seealso cref= #removeChangeListener </seealso>
		Public Overridable Sub addChangeListener(ByVal l As ChangeListener)
			listenerList.add(GetType(ChangeListener), l)
		End Sub


		''' <summary>
		''' Removes a ChangeListener from the slider.
		''' </summary>
		''' <param name="l"> the ChangeListener to remove </param>
		''' <seealso cref= #fireStateChanged </seealso>
		''' <seealso cref= #addChangeListener
		'''  </seealso>
		Public Overridable Sub removeChangeListener(ByVal l As ChangeListener)
			listenerList.remove(GetType(ChangeListener), l)
		End Sub


		''' <summary>
		''' Returns an array of all the <code>ChangeListener</code>s added
		''' to this JSlider with addChangeListener().
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
		''' Send a {@code ChangeEvent}, whose source is this {@code JSlider}, to
		''' all {@code ChangeListener}s that have registered interest in
		''' {@code ChangeEvent}s.
		''' This method is called each time a {@code ChangeEvent} is received from
		''' the model.
		''' <p>
		''' The event instance is created if necessary, and stored in
		''' {@code changeEvent}.
		''' </summary>
		''' <seealso cref= #addChangeListener </seealso>
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
		''' Returns the {@code BoundedRangeModel} that handles the slider's three
		''' fundamental properties: minimum, maximum, value.
		''' </summary>
		''' <returns> the data model for this component </returns>
		''' <seealso cref= #setModel </seealso>
		''' <seealso cref=    BoundedRangeModel </seealso>
		Public Overridable Property model As BoundedRangeModel
			Get
				Return sliderModel
			End Get
			Set(ByVal newModel As BoundedRangeModel)
				Dim oldModel As BoundedRangeModel = model
    
				If oldModel IsNot Nothing Then oldModel.removeChangeListener(changeListener)
    
				sliderModel = newModel
    
				If newModel IsNot Nothing Then newModel.addChangeListener(changeListener)
    
				If accessibleContext IsNot Nothing Then accessibleContext.firePropertyChange(AccessibleContext.ACCESSIBLE_VALUE_PROPERTY, (If(oldModel Is Nothing, Nothing, Convert.ToInt32(oldModel.value))), (If(newModel Is Nothing, Nothing, Convert.ToInt32(newModel.value))))
    
				firePropertyChange("model", oldModel, sliderModel)
			End Set
		End Property




		''' <summary>
		''' Returns the slider's current value
		''' from the {@code BoundedRangeModel}.
		''' </summary>
		''' <returns>  the current value of the slider </returns>
		''' <seealso cref=     #setValue </seealso>
		''' <seealso cref=     BoundedRangeModel#getValue </seealso>
		Public Overridable Property value As Integer
			Get
				Return model.value
			End Get
			Set(ByVal n As Integer)
				Dim m As BoundedRangeModel = model
				Dim oldValue As Integer = m.value
				If oldValue = n Then Return
				m.value = n
    
				If accessibleContext IsNot Nothing Then accessibleContext.firePropertyChange(AccessibleContext.ACCESSIBLE_VALUE_PROPERTY, Convert.ToInt32(oldValue), Convert.ToInt32(m.value))
			End Set
		End Property



		''' <summary>
		''' Returns the minimum value supported by the slider
		''' from the <code>BoundedRangeModel</code>.
		''' </summary>
		''' <returns> the value of the model's minimum property </returns>
		''' <seealso cref= #setMinimum </seealso>
		''' <seealso cref=     BoundedRangeModel#getMinimum </seealso>
		Public Overridable Property minimum As Integer
			Get
				Return model.minimum
			End Get
			Set(ByVal minimum As Integer)
				Dim oldMin As Integer = model.minimum
				model.minimum = minimum
				firePropertyChange("minimum", Convert.ToInt32(oldMin), Convert.ToInt32(minimum))
			End Set
		End Property




		''' <summary>
		''' Returns the maximum value supported by the slider
		''' from the <code>BoundedRangeModel</code>.
		''' </summary>
		''' <returns> the value of the model's maximum property </returns>
		''' <seealso cref= #setMaximum </seealso>
		''' <seealso cref= BoundedRangeModel#getMaximum </seealso>
		Public Overridable Property maximum As Integer
			Get
				Return model.maximum
			End Get
			Set(ByVal maximum As Integer)
				Dim oldMax As Integer = model.maximum
				model.maximum = maximum
				firePropertyChange("maximum", Convert.ToInt32(oldMax), Convert.ToInt32(maximum))
			End Set
		End Property




		''' <summary>
		''' Returns the {@code valueIsAdjusting} property from the model.  For
		''' details on how this is used, see the {@code setValueIsAdjusting}
		''' documentation.
		''' </summary>
		''' <returns> the value of the model's {@code valueIsAdjusting} property </returns>
		''' <seealso cref= #setValueIsAdjusting </seealso>
		Public Overridable Property valueIsAdjusting As Boolean
			Get
				Return model.valueIsAdjusting
			End Get
			Set(ByVal b As Boolean)
				Dim m As BoundedRangeModel = model
				Dim oldValue As Boolean = m.valueIsAdjusting
				m.valueIsAdjusting = b
    
				If (oldValue <> b) AndAlso (accessibleContext IsNot Nothing) Then accessibleContext.firePropertyChange(AccessibleContext.ACCESSIBLE_STATE_PROPERTY, (If(oldValue, AccessibleState.BUSY, Nothing)), (If(b, AccessibleState.BUSY, Nothing)))
			End Set
		End Property




		''' <summary>
		''' Returns the "extent" from the <code>BoundedRangeModel</code>.
		''' This represents the range of values "covered" by the knob.
		''' </summary>
		''' <returns> an int representing the extent </returns>
		''' <seealso cref= #setExtent </seealso>
		''' <seealso cref= BoundedRangeModel#getExtent </seealso>
		Public Overridable Property extent As Integer
			Get
				Return model.extent
			End Get
			Set(ByVal extent As Integer)
				model.extent = extent
			End Set
		End Property




		''' <summary>
		''' Return this slider's vertical or horizontal orientation. </summary>
		''' <returns> {@code SwingConstants.VERTICAL} or
		'''  {@code SwingConstants.HORIZONTAL} </returns>
		''' <seealso cref= #setOrientation </seealso>
		Public Overridable Property orientation As Integer
			Get
				Return orientation
			End Get
			Set(ByVal orientation As Integer)
				checkOrientation(orientation)
				Dim oldValue As Integer = Me.orientation
				Me.orientation = orientation
				firePropertyChange("orientation", oldValue, orientation)
    
				If (oldValue <> orientation) AndAlso (accessibleContext IsNot Nothing) Then accessibleContext.firePropertyChange(AccessibleContext.ACCESSIBLE_STATE_PROPERTY, (If(oldValue = VERTICAL, AccessibleState.VERTICAL, AccessibleState.HORIZONTAL)), (If(orientation = VERTICAL, AccessibleState.VERTICAL, AccessibleState.HORIZONTAL)))
				If orientation <> oldValue Then revalidate()
			End Set
		End Property




		''' <summary>
		''' {@inheritDoc}
		''' 
		''' @since 1.6
		''' </summary>
		Public Overrides Property font As Font
			Set(ByVal font As Font)
				MyBase.font = font
				updateLabelSizes()
			End Set
		End Property

		''' <summary>
		''' {@inheritDoc}
		''' @since 1.7
		''' </summary>
		Public Overridable Function imageUpdate(ByVal img As Image, ByVal infoflags As Integer, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer) As Boolean
			If Not showing Then Return False

			' Check that there is a label with such image
			Dim elements As System.Collections.IEnumerator = labelTable.elements()

			Do While elements.hasMoreElements()
				Dim component As Component = CType(elements.nextElement(), Component)

				If TypeOf component Is JLabel Then
					Dim label As JLabel = CType(component, JLabel)

					If SwingUtilities.doesIconReferenceImage(label.icon, img) OrElse SwingUtilities.doesIconReferenceImage(label.disabledIcon, img) Then Return MyBase.imageUpdate(img, infoflags, x, y, w, h)
				End If
			Loop

			Return False
		End Function

		''' <summary>
		''' Returns the dictionary of what labels to draw at which values.
		''' </summary>
		''' <returns> the <code>Dictionary</code> containing labels and
		'''    where to draw them </returns>
		Public Overridable Property labelTable As Dictionary
			Get
		'
		'        if ( labelTable == null && getMajorTickSpacing() > 0 ) {
		'            setLabelTable( createStandardLabels( getMajorTickSpacing() ) );
		'        }
		'
				Return labelTable
			End Get
			Set(ByVal labels As Dictionary)
				Dim oldTable As Dictionary = labelTable
				labelTable = labels
				updateLabelUIs()
				firePropertyChange("labelTable", oldTable, labelTable)
				If labels IsNot oldTable Then
					revalidate()
					repaint()
				End If
			End Set
		End Property




		''' <summary>
		''' Updates the UIs for the labels in the label table by calling
		''' {@code updateUI} on each label.  The UIs are updated from
		''' the current look and feel.  The labels are also set to their
		''' preferred size.
		''' </summary>
		''' <seealso cref= #setLabelTable </seealso>
		''' <seealso cref= JComponent#updateUI </seealso>
		Protected Friend Overridable Sub updateLabelUIs()
			Dim ___labelTable As Dictionary = labelTable

			If ___labelTable Is Nothing Then Return
			Dim labels As System.Collections.IEnumerator = ___labelTable.keys()
			Do While labels.hasMoreElements()
				Dim component As JComponent = CType(___labelTable.get(labels.nextElement()), JComponent)
				component.updateUI()
				component.size = component.preferredSize
			Loop
		End Sub

		Private Sub updateLabelSizes()
			Dim ___labelTable As Dictionary = labelTable
			If ___labelTable IsNot Nothing Then
				Dim labels As System.Collections.IEnumerator = ___labelTable.elements()
				Do While labels.hasMoreElements()
					Dim component As JComponent = CType(labels.nextElement(), JComponent)
					component.size = component.preferredSize
				Loop
			End If
		End Sub


		''' <summary>
		''' Creates a {@code Hashtable} of numerical text labels, starting at the
		''' slider minimum, and using the increment specified.
		''' For example, if you call <code>createStandardLabels( 10 )</code>
		''' and the slider minimum is zero,
		''' then labels will be created for the values 0, 10, 20, 30, and so on.
		''' <p>
		''' For the labels to be drawn on the slider, the returned {@code Hashtable}
		''' must be passed into {@code setLabelTable}, and {@code setPaintLabels}
		''' must be set to {@code true}.
		''' <p>
		''' For further details on the makeup of the returned {@code Hashtable}, see
		''' the {@code setLabelTable} documentation.
		''' </summary>
		''' <param name="increment">  distance between labels in the generated hashtable </param>
		''' <returns> a new {@code Hashtable} of labels </returns>
		''' <seealso cref= #setLabelTable </seealso>
		''' <seealso cref= #setPaintLabels </seealso>
		''' <exception cref="IllegalArgumentException"> if {@code increment} is less than or
		'''          equal to zero </exception>
		Public Overridable Function createStandardLabels(ByVal increment As Integer) As Hashtable
			Return createStandardLabels(increment, minimum)
		End Function


		''' <summary>
		''' Creates a {@code Hashtable} of numerical text labels, starting at the
		''' starting point specified, and using the increment specified.
		''' For example, if you call
		''' <code>createStandardLabels( 10, 2 )</code>,
		''' then labels will be created for the values 2, 12, 22, 32, and so on.
		''' <p>
		''' For the labels to be drawn on the slider, the returned {@code Hashtable}
		''' must be passed into {@code setLabelTable}, and {@code setPaintLabels}
		''' must be set to {@code true}.
		''' <p>
		''' For further details on the makeup of the returned {@code Hashtable}, see
		''' the {@code setLabelTable} documentation.
		''' </summary>
		''' <param name="increment">  distance between labels in the generated hashtable </param>
		''' <param name="start">      value at which the labels will begin </param>
		''' <returns> a new {@code Hashtable} of labels </returns>
		''' <seealso cref= #setLabelTable </seealso>
		''' <seealso cref= #setPaintLabels </seealso>
		''' <exception cref="IllegalArgumentException"> if {@code start} is
		'''          out of range, or if {@code increment} is less than or equal
		'''          to zero </exception>
		Public Overridable Function createStandardLabels(ByVal increment As Integer, ByVal start As Integer) As Hashtable
			If start > maximum OrElse start < minimum Then Throw New System.ArgumentException("Slider label start point out of range.")

			If increment <= 0 Then Throw New System.ArgumentException("Label incremement must be > 0")

'JAVA TO VB CONVERTER TODO TASK: Local classes are not converted by Java to VB Converter:
'			class SmartHashtable extends Hashtable(Of Object, Object) implements PropertyChangeListener
	'		{
	'			int increment = 0;
	'			int start = 0;
	'			boolean startAtMin = False;
	'
	'			class LabelUIResource extends JLabel implements UIResource
	'			{
	'				public LabelUIResource(String text, int alignment)
	'				{
	'					MyBase(text, alignment);
	'					setName("Slider.label");
	'				}
	'
	'				public Font getFont()
	'				{
	'					Font font = MyBase.getFont();
	'					if (font != Nothing && !(font instanceof UIResource))
	'					{
	'						Return font;
	'					}
	'					Return outerInstance.getFont();
	'				}
	'
	'				public Color getForeground()
	'				{
	'					Color fg = MyBase.getForeground();
	'					if (fg != Nothing && !(fg instanceof UIResource))
	'					{
	'						Return fg;
	'					}
	'					if (!(outerInstance.getForeground() instanceof UIResource))
	'					{
	'						Return outerInstance.getForeground();
	'					}
	'					Return fg;
	'				}
	'			}
	'
	'			public SmartHashtable(int increment, int start)
	'			{
	'				MyBase();
	'				Me.increment = increment;
	'				Me.start = start;
	'				startAtMin = start == getMinimum();
	'				createLabels();
	'			}
	'
	'			public void propertyChange(PropertyChangeEvent e)
	'			{
	'				if (e.getPropertyName().equals("minimum") && startAtMin)
	'				{
	'					start = getMinimum();
	'				}
	'
	'				if (e.getPropertyName().equals("minimum") || e.getPropertyName().equals("maximum"))
	'				{
	'
	'					Enumeration keys = getLabelTable().keys();
	'					Hashtable<Object, Object> hashtable = New Hashtable<Object, Object>();
	'
	'					' Save the labels that were added by the developer
	'					while (keys.hasMoreElements())
	'					{
	'						Object key = keys.nextElement();
	'						Object value = labelTable.get(key);
	'						if (!(value instanceof LabelUIResource))
	'						{
	'							hashtable.put(key, value);
	'						}
	'					}
	'
	'					clear();
	'					createLabels();
	'
	'					' Add the saved labels
	'					keys = hashtable.keys();
	'					while (keys.hasMoreElements())
	'					{
	'						Object key = keys.nextElement();
	'						put(key, hashtable.get(key));
	'					}
	'
	'					((JSlider)e.getSource()).setLabelTable(Me);
	'				}
	'			}
	'
	'			void createLabels()
	'			{
	'				for (int labelIndex = start; labelIndex <= getMaximum(); labelIndex += increment)
	'				{
	'					put(Integer.valueOf(labelIndex), New LabelUIResource(""+labelIndex, JLabel.CENTER));
	'				}
	'			}
	'		}

			Dim table As New SmartHashtable(increment, start)

			Dim ___labelTable As Dictionary = labelTable

			If ___labelTable IsNot Nothing AndAlso (TypeOf ___labelTable Is PropertyChangeListener) Then removePropertyChangeListener(CType(___labelTable, PropertyChangeListener))

			addPropertyChangeListener(table)

			Return table
		End Function


		''' <summary>
		''' Returns true if the value-range shown for the slider is reversed,
		''' </summary>
		''' <returns> true if the slider values are reversed from their normal order </returns>
		''' <seealso cref= #setInverted </seealso>
		Public Overridable Property inverted As Boolean
			Get
				Return isInverted
			End Get
			Set(ByVal b As Boolean)
				Dim oldValue As Boolean = isInverted
				isInverted = b
				firePropertyChange("inverted", oldValue, isInverted)
				If b <> oldValue Then repaint()
			End Set
		End Property




		''' <summary>
		''' This method returns the major tick spacing.  The number that is returned
		''' represents the distance, measured in values, between each major tick mark.
		''' If you have a slider with a range from 0 to 50 and the major tick spacing
		''' is set to 10, you will get major ticks next to the following values:
		''' 0, 10, 20, 30, 40, 50.
		''' </summary>
		''' <returns> the number of values between major ticks </returns>
		''' <seealso cref= #setMajorTickSpacing </seealso>
		Public Overridable Property majorTickSpacing As Integer
			Get
				Return majorTickSpacing
			End Get
			Set(ByVal n As Integer)
				Dim oldValue As Integer = majorTickSpacing
				majorTickSpacing = n
				If labelTable Is Nothing AndAlso majorTickSpacing > 0 AndAlso paintLabels Then labelTable = createStandardLabels(majorTickSpacing)
				firePropertyChange("majorTickSpacing", oldValue, majorTickSpacing)
				If majorTickSpacing <> oldValue AndAlso paintTicks Then repaint()
			End Set
		End Property





		''' <summary>
		''' This method returns the minor tick spacing.  The number that is returned
		''' represents the distance, measured in values, between each minor tick mark.
		''' If you have a slider with a range from 0 to 50 and the minor tick spacing
		''' is set to 10, you will get minor ticks next to the following values:
		''' 0, 10, 20, 30, 40, 50.
		''' </summary>
		''' <returns> the number of values between minor ticks </returns>
		''' <seealso cref= #getMinorTickSpacing </seealso>
		Public Overridable Property minorTickSpacing As Integer
			Get
				Return minorTickSpacing
			End Get
			Set(ByVal n As Integer)
				Dim oldValue As Integer = minorTickSpacing
				minorTickSpacing = n
				firePropertyChange("minorTickSpacing", oldValue, minorTickSpacing)
				If minorTickSpacing <> oldValue AndAlso paintTicks Then repaint()
			End Set
		End Property




		''' <summary>
		''' Returns true if the knob (and the data value it represents)
		''' resolve to the closest tick mark next to where the user
		''' positioned the knob.
		''' </summary>
		''' <returns> true if the value snaps to the nearest tick mark, else false </returns>
		''' <seealso cref= #setSnapToTicks </seealso>
		Public Overridable Property snapToTicks As Boolean
			Get
				Return snapToTicks
			End Get
			Set(ByVal b As Boolean)
				Dim oldValue As Boolean = snapToTicks
				snapToTicks = b
				firePropertyChange("snapToTicks", oldValue, snapToTicks)
			End Set
		End Property


		''' <summary>
		''' Returns true if the knob (and the data value it represents)
		''' resolve to the closest slider value next to where the user
		''' positioned the knob.
		''' </summary>
		''' <returns> true if the value snaps to the nearest slider value, else false </returns>
		''' <seealso cref= #setSnapToValue </seealso>
		Friend Overridable Property snapToValue As Boolean
			Get
				Return snapToValue
			End Get
			Set(ByVal b As Boolean)
				Dim oldValue As Boolean = snapToValue
				snapToValue = b
				firePropertyChange("snapToValue", oldValue, snapToValue)
			End Set
		End Property






		''' <summary>
		''' Tells if tick marks are to be painted. </summary>
		''' <returns> true if tick marks are painted, else false </returns>
		''' <seealso cref= #setPaintTicks </seealso>
		Public Overridable Property paintTicks As Boolean
			Get
				Return paintTicks
			End Get
			Set(ByVal b As Boolean)
				Dim oldValue As Boolean = paintTicks
				paintTicks = b
				firePropertyChange("paintTicks", oldValue, paintTicks)
				If paintTicks <> oldValue Then
					revalidate()
					repaint()
				End If
			End Set
		End Property



		''' <summary>
		''' Tells if the track (area the slider slides in) is to be painted. </summary>
		''' <returns> true if track is painted, else false </returns>
		''' <seealso cref= #setPaintTrack </seealso>
		Public Overridable Property paintTrack As Boolean
			Get
				Return paintTrack
			End Get
			Set(ByVal b As Boolean)
				Dim oldValue As Boolean = paintTrack
				paintTrack = b
				firePropertyChange("paintTrack", oldValue, paintTrack)
				If paintTrack <> oldValue Then repaint()
			End Set
		End Property




		''' <summary>
		''' Tells if labels are to be painted. </summary>
		''' <returns> true if labels are painted, else false </returns>
		''' <seealso cref= #setPaintLabels </seealso>
		Public Overridable Property paintLabels As Boolean
			Get
				Return paintLabels
			End Get
			Set(ByVal b As Boolean)
				Dim oldValue As Boolean = paintLabels
				paintLabels = b
				If labelTable Is Nothing AndAlso majorTickSpacing > 0 Then labelTable = createStandardLabels(majorTickSpacing)
				firePropertyChange("paintLabels", oldValue, paintLabels)
				If paintLabels <> oldValue Then
					revalidate()
					repaint()
				End If
			End Set
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
		''' Returns a string representation of this JSlider. This method
		''' is intended to be used only for debugging purposes, and the
		''' content and format of the returned string may vary between
		''' implementations. The returned string may be empty but may not
		''' be <code>null</code>.
		''' </summary>
		''' <returns>  a string representation of this JSlider. </returns>
		Protected Friend Overrides Function paramString() As String
			Dim paintTicksString As String = (If(paintTicks, "true", "false"))
			Dim paintTrackString As String = (If(paintTrack, "true", "false"))
			Dim paintLabelsString As String = (If(paintLabels, "true", "false"))
			Dim isInvertedString As String = (If(isInverted, "true", "false"))
			Dim snapToTicksString As String = (If(snapToTicks, "true", "false"))
			Dim snapToValueString As String = (If(snapToValue, "true", "false"))
			Dim orientationString As String = (If(orientation = HORIZONTAL, "HORIZONTAL", "VERTICAL"))

			Return MyBase.paramString() & ",isInverted=" & isInvertedString & ",majorTickSpacing=" & majorTickSpacing & ",minorTickSpacing=" & minorTickSpacing & ",orientation=" & orientationString & ",paintLabels=" & paintLabelsString & ",paintTicks=" & paintTicksString & ",paintTrack=" & paintTrackString & ",snapToTicks=" & snapToTicksString & ",snapToValue=" & snapToValueString
		End Function


	'///////////////
	' Accessibility support
	'//////////////

		''' <summary>
		''' Gets the AccessibleContext associated with this JSlider.
		''' For sliders, the AccessibleContext takes the form of an
		''' AccessibleJSlider.
		''' A new AccessibleJSlider instance is created if necessary.
		''' </summary>
		''' <returns> an AccessibleJSlider that serves as the
		'''         AccessibleContext of this JSlider </returns>
		Public Overridable Property accessibleContext As AccessibleContext Implements Accessible.getAccessibleContext
			Get
				If accessibleContext Is Nothing Then accessibleContext = New AccessibleJSlider(Me)
				Return accessibleContext
			End Get
		End Property

		''' <summary>
		''' This class implements accessibility support for the
		''' <code>JSlider</code> class.  It provides an implementation of the
		''' Java Accessibility API appropriate to slider user-interface elements.
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
		Protected Friend Class AccessibleJSlider
			Inherits AccessibleJComponent
			Implements AccessibleValue

			Private ReadOnly outerInstance As JSlider

			Public Sub New(ByVal outerInstance As JSlider)
				Me.outerInstance = outerInstance
			End Sub


			''' <summary>
			''' Get the state set of this object.
			''' </summary>
			''' <returns> an instance of AccessibleState containing the current state
			''' of the object </returns>
			''' <seealso cref= AccessibleState </seealso>
			Public Overridable Property accessibleStateSet As AccessibleStateSet
				Get
					Dim states As AccessibleStateSet = MyBase.accessibleStateSet
					If outerInstance.valueIsAdjusting Then states.add(AccessibleState.BUSY)
					If outerInstance.orientation = VERTICAL Then
						states.add(AccessibleState.VERTICAL)
					Else
						states.add(AccessibleState.HORIZONTAL)
					End If
					Return states
				End Get
			End Property

			''' <summary>
			''' Get the role of this object.
			''' </summary>
			''' <returns> an instance of AccessibleRole describing the role of the object </returns>
			Public Overridable Property accessibleRole As AccessibleRole
				Get
					Return AccessibleRole.SLIDER
				End Get
			End Property

			''' <summary>
			''' Get the AccessibleValue associated with this object.  In the
			''' implementation of the Java Accessibility API for this class,
			''' return this object, which is responsible for implementing the
			''' AccessibleValue interface on behalf of itself.
			''' </summary>
			''' <returns> this object </returns>
			Public Overridable Property accessibleValue As AccessibleValue
				Get
					Return Me
				End Get
			End Property

			''' <summary>
			''' Get the accessible value of this object.
			''' </summary>
			''' <returns> The current value of this object. </returns>
			Public Overridable Property currentAccessibleValue As Number Implements AccessibleValue.getCurrentAccessibleValue
				Get
					Return Convert.ToInt32(outerInstance.value)
				End Get
			End Property

			''' <summary>
			''' Set the value of this object as a Number.
			''' </summary>
			''' <returns> True if the value was set. </returns>
			Public Overridable Function setCurrentAccessibleValue(ByVal n As Number) As Boolean Implements AccessibleValue.setCurrentAccessibleValue
				' TIGER - 4422535
				If n Is Nothing Then Return False
				outerInstance.value = n
				Return True
			End Function

			''' <summary>
			''' Get the minimum accessible value of this object.
			''' </summary>
			''' <returns> The minimum value of this object. </returns>
			Public Overridable Property minimumAccessibleValue As Number Implements AccessibleValue.getMinimumAccessibleValue
				Get
					Return Convert.ToInt32(outerInstance.minimum)
				End Get
			End Property

			''' <summary>
			''' Get the maximum accessible value of this object.
			''' </summary>
			''' <returns> The maximum value of this object. </returns>
			Public Overridable Property maximumAccessibleValue As Number Implements AccessibleValue.getMaximumAccessibleValue
				Get
					' TIGER - 4422362
					Dim model As BoundedRangeModel = outerInstance.model
					Return Convert.ToInt32(model.maximum - model.extent)
				End Get
			End Property
		End Class ' AccessibleJSlider
	End Class

End Namespace
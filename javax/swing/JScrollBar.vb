Imports System
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
	''' An implementation of a scrollbar. The user positions the knob in the
	''' scrollbar to determine the contents of the viewing area. The
	''' program typically adjusts the display so that the end of the
	''' scrollbar represents the end of the displayable contents, or 100%
	''' of the contents. The start of the scrollbar is the beginning of the
	''' displayable contents, or 0%. The position of the knob within
	''' those bounds then translates to the corresponding percentage of
	''' the displayable contents.
	''' <p>
	''' Typically, as the position of the knob in the scrollbar changes
	''' a corresponding change is made to the position of the JViewport on
	''' the underlying view, changing the contents of the JViewport.
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
	''' <seealso cref= JScrollPane
	''' @beaninfo
	'''      attribute: isContainer false
	'''    description: A component that helps determine the visible content range of an area.
	''' 
	''' @author David Kloba </seealso>
	Public Class JScrollBar
		Inherits JComponent
		Implements java.awt.Adjustable, Accessible

		''' <seealso cref= #getUIClassID </seealso>
		''' <seealso cref= #readObject </seealso>
		Private Const uiClassID As String = "ScrollBarUI"

		''' <summary>
		''' All changes from the model are treated as though the user moved
		''' the scrollbar knob.
		''' </summary>
		Private fwdAdjustmentEvents As ChangeListener = New ModelListener(Me)


		''' <summary>
		''' The model that represents the scrollbar's minimum, maximum, extent
		''' (aka "visibleAmount") and current value. </summary>
		''' <seealso cref= #setModel </seealso>
		Protected Friend model As BoundedRangeModel


		''' <seealso cref= #setOrientation </seealso>
		Protected Friend orientation As Integer


		''' <seealso cref= #setUnitIncrement </seealso>
		Protected Friend unitIncrement As Integer


		''' <seealso cref= #setBlockIncrement </seealso>
		Protected Friend blockIncrement As Integer


		Private Sub checkOrientation(ByVal orientation As Integer)
			Select Case orientation
			Case VERTICAL, HORIZONTAL
			Case Else
				Throw New System.ArgumentException("orientation must be one of: VERTICAL, HORIZONTAL")
			End Select
		End Sub


		''' <summary>
		''' Creates a scrollbar with the specified orientation,
		''' value, extent, minimum, and maximum.
		''' The "extent" is the size of the viewable area. It is also known
		''' as the "visible amount".
		''' <p>
		''' Note: Use <code>setBlockIncrement</code> to set the block
		''' increment to a size slightly smaller than the view's extent.
		''' That way, when the user jumps the knob to an adjacent position,
		''' one or two lines of the original contents remain in view.
		''' </summary>
		''' <exception cref="IllegalArgumentException"> if orientation is not one of VERTICAL, HORIZONTAL
		''' </exception>
		''' <seealso cref= #setOrientation </seealso>
		''' <seealso cref= #setValue </seealso>
		''' <seealso cref= #setVisibleAmount </seealso>
		''' <seealso cref= #setMinimum </seealso>
		''' <seealso cref= #setMaximum </seealso>
		Public Sub New(ByVal orientation As Integer, ByVal value As Integer, ByVal extent As Integer, ByVal min As Integer, ByVal max As Integer)
			checkOrientation(orientation)
			Me.unitIncrement = 1
			Me.blockIncrement = If(extent = 0, 1, extent)
			Me.orientation = orientation
			Me.model = New DefaultBoundedRangeModel(value, extent, min, max)
			Me.model.addChangeListener(fwdAdjustmentEvents)
			requestFocusEnabled = False
			updateUI()
		End Sub


		''' <summary>
		''' Creates a scrollbar with the specified orientation
		''' and the following initial values:
		''' <pre>
		''' minimum = 0
		''' maximum = 100
		''' value = 0
		''' extent = 10
		''' </pre>
		''' </summary>
		Public Sub New(ByVal orientation As Integer)
			Me.New(orientation, 0, 10, 0, 100)
		End Sub


		''' <summary>
		''' Creates a vertical scrollbar with the following initial values:
		''' <pre>
		''' minimum = 0
		''' maximum = 100
		''' value = 0
		''' extent = 10
		''' </pre>
		''' </summary>
		Public Sub New()
			Me.New(VERTICAL)
		End Sub


		''' <summary>
		''' Sets the {@literal L&F} object that renders this component.
		''' </summary>
		''' <param name="ui">  the <code>ScrollBarUI</code> {@literal L&F} object </param>
		''' <seealso cref= UIDefaults#getUI
		''' @since 1.4
		''' @beaninfo
		'''        bound: true
		'''       hidden: true
		'''    attribute: visualUpdate true
		'''  description: The UI object that implements the Component's LookAndFeel </seealso>
		Public Overridable Property uI As ScrollBarUI
			Set(ByVal ui As ScrollBarUI)
				MyBase.uI = ui
			End Set
			Get
				Return CType(ui, ScrollBarUI)
			End Get
		End Property




		''' <summary>
		''' Overrides <code>JComponent.updateUI</code>. </summary>
		''' <seealso cref= JComponent#updateUI </seealso>
		Public Overrides Sub updateUI()
			uI = CType(UIManager.getUI(Me), ScrollBarUI)
		End Sub


		''' <summary>
		''' Returns the name of the LookAndFeel class for this component.
		''' </summary>
		''' <returns> "ScrollBarUI" </returns>
		''' <seealso cref= JComponent#getUIClassID </seealso>
		''' <seealso cref= UIDefaults#getUI </seealso>
		Public Property Overrides uIClassID As String
			Get
				Return uiClassID
			End Get
		End Property



		''' <summary>
		''' Returns the component's orientation (horizontal or vertical).
		''' </summary>
		''' <returns> VERTICAL or HORIZONTAL </returns>
		''' <seealso cref= #setOrientation </seealso>
		''' <seealso cref= java.awt.Adjustable#getOrientation </seealso>
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
		''' Returns data model that handles the scrollbar's four
		''' fundamental properties: minimum, maximum, value, extent.
		''' </summary>
		''' <seealso cref= #setModel </seealso>
		Public Overridable Property model As BoundedRangeModel
			Get
				Return model
			End Get
			Set(ByVal newModel As BoundedRangeModel)
				Dim oldValue As Integer? = Nothing
				Dim oldModel As BoundedRangeModel = model
				If model IsNot Nothing Then
					model.removeChangeListener(fwdAdjustmentEvents)
					oldValue = Convert.ToInt32(model.value)
				End If
				model = newModel
				If model IsNot Nothing Then model.addChangeListener(fwdAdjustmentEvents)
    
				firePropertyChange("model", oldModel, model)
    
				If accessibleContext IsNot Nothing Then accessibleContext.firePropertyChange(AccessibleContext.ACCESSIBLE_VALUE_PROPERTY, oldValue, New Integer?(model.value))
			End Set
		End Property




		''' <summary>
		''' Returns the amount to change the scrollbar's value by,
		''' given a unit up/down request.  A ScrollBarUI implementation
		''' typically calls this method when the user clicks on a scrollbar
		''' up/down arrow and uses the result to update the scrollbar's
		''' value.   Subclasses my override this method to compute
		''' a value, e.g. the change required to scroll up or down one
		''' (variable height) line text or one row in a table.
		''' <p>
		''' The JScrollPane component creates scrollbars (by default)
		''' that override this method and delegate to the viewports
		''' Scrollable view, if it has one.  The Scrollable interface
		''' provides a more specialized version of this method.
		''' <p>
		''' Some look and feels implement custom scrolling behavior
		''' and ignore this property.
		''' </summary>
		''' <param name="direction"> is -1 or 1 for up/down respectively </param>
		''' <returns> the value of the unitIncrement property </returns>
		''' <seealso cref= #setUnitIncrement </seealso>
		''' <seealso cref= #setValue </seealso>
		''' <seealso cref= Scrollable#getScrollableUnitIncrement </seealso>
		Public Overridable Function getUnitIncrement(ByVal direction As Integer) As Integer
			Return unitIncrement
		End Function


		''' <summary>
		''' Sets the unitIncrement property.
		''' <p>
		''' Note, that if the argument is equal to the value of Integer.MIN_VALUE,
		''' the most look and feels will not provide the scrolling to the right/down.
		''' <p>
		''' Some look and feels implement custom scrolling behavior
		''' and ignore this property.
		''' </summary>
		''' <seealso cref= #getUnitIncrement
		''' @beaninfo
		'''   preferred: true
		'''       bound: true
		''' description: The scrollbar's unit increment. </seealso>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Public Overridable Sub setUnitIncrement(ByVal unitIncrement As Integer) 'JavaToDotNetTempPropertySetunitIncrement
		Public Overridable Property unitIncrement As Integer
			Set(ByVal unitIncrement As Integer)
				Dim oldValue As Integer = Me.unitIncrement
				Me.unitIncrement = unitIncrement
				firePropertyChange("unitIncrement", oldValue, unitIncrement)
			End Set
			Get
		End Property


		''' <summary>
		''' Returns the amount to change the scrollbar's value by,
		''' given a block (usually "page") up/down request.  A ScrollBarUI
		''' implementation typically calls this method when the user clicks
		''' above or below the scrollbar "knob" to change the value
		''' up or down by large amount.  Subclasses my override this
		''' method to compute a value, e.g. the change required to scroll
		''' up or down one paragraph in a text document.
		''' <p>
		''' The JScrollPane component creates scrollbars (by default)
		''' that override this method and delegate to the viewports
		''' Scrollable view, if it has one.  The Scrollable interface
		''' provides a more specialized version of this method.
		''' <p>
		''' Some look and feels implement custom scrolling behavior
		''' and ignore this property.
		''' </summary>
		''' <param name="direction"> is -1 or 1 for up/down respectively </param>
		''' <returns> the value of the blockIncrement property </returns>
		''' <seealso cref= #setBlockIncrement </seealso>
		''' <seealso cref= #setValue </seealso>
		''' <seealso cref= Scrollable#getScrollableBlockIncrement </seealso>
		Public Overridable Function getBlockIncrement(ByVal direction As Integer) As Integer
			Return blockIncrement
		End Function


		''' <summary>
		''' Sets the blockIncrement property.
		''' <p>
		''' Note, that if the argument is equal to the value of Integer.MIN_VALUE,
		''' the most look and feels will not provide the scrolling to the right/down.
		''' <p>
		''' Some look and feels implement custom scrolling behavior
		''' and ignore this property.
		''' </summary>
		''' <seealso cref= #getBlockIncrement()
		''' @beaninfo
		'''   preferred: true
		'''       bound: true
		''' description: The scrollbar's block increment. </seealso>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Public Overridable Sub setBlockIncrement(ByVal blockIncrement As Integer) 'JavaToDotNetTempPropertySetblockIncrement
		Public Overridable Property blockIncrement As Integer
			Set(ByVal blockIncrement As Integer)
				Dim oldValue As Integer = Me.blockIncrement
				Me.blockIncrement = blockIncrement
				firePropertyChange("blockIncrement", oldValue, blockIncrement)
			End Set
			Get
		End Property


			Return unitIncrement
		End Function


			Return blockIncrement
		End Function


		''' <summary>
		''' Returns the scrollbar's value. </summary>
		''' <returns> the model's value property </returns>
		''' <seealso cref= #setValue </seealso>
		Public Overridable Property value As Integer
			Get
				Return model.value
			End Get
			Set(ByVal value As Integer)
				Dim m As BoundedRangeModel = model
				Dim oldValue As Integer = m.value
				m.value = value
    
				If accessibleContext IsNot Nothing Then accessibleContext.firePropertyChange(AccessibleContext.ACCESSIBLE_VALUE_PROPERTY, Convert.ToInt32(oldValue), Convert.ToInt32(m.value))
			End Set
		End Property




		''' <summary>
		''' Returns the scrollbar's extent, aka its "visibleAmount".  In many
		''' scrollbar look and feel implementations the size of the
		''' scrollbar "knob" or "thumb" is proportional to the extent.
		''' </summary>
		''' <returns> the value of the model's extent property </returns>
		''' <seealso cref= #setVisibleAmount </seealso>
		Public Overridable Property visibleAmount As Integer
			Get
				Return model.extent
			End Get
			Set(ByVal extent As Integer)
				model.extent = extent
			End Set
		End Property




		''' <summary>
		''' Returns the minimum value supported by the scrollbar
		''' (usually zero).
		''' </summary>
		''' <returns> the value of the model's minimum property </returns>
		''' <seealso cref= #setMinimum </seealso>
		Public Overridable Property minimum As Integer
			Get
				Return model.minimum
			End Get
			Set(ByVal minimum As Integer)
				model.minimum = minimum
			End Set
		End Property




		''' <summary>
		''' The maximum value of the scrollbar is maximum - extent.
		''' </summary>
		''' <returns> the value of the model's maximum property </returns>
		''' <seealso cref= #setMaximum </seealso>
		Public Overridable Property maximum As Integer
			Get
				Return model.maximum
			End Get
			Set(ByVal maximum As Integer)
				model.maximum = maximum
			End Set
		End Property




		''' <summary>
		''' True if the scrollbar knob is being dragged.
		''' </summary>
		''' <returns> the value of the model's valueIsAdjusting property </returns>
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
		''' Sets the four BoundedRangeModel properties after forcing
		''' the arguments to obey the usual constraints:
		''' <pre>
		''' minimum &le; value &le; value+extent &le; maximum
		''' </pre>
		''' 
		''' </summary>
		''' <seealso cref= BoundedRangeModel#setRangeProperties </seealso>
		''' <seealso cref= #setValue </seealso>
		''' <seealso cref= #setVisibleAmount </seealso>
		''' <seealso cref= #setMinimum </seealso>
		''' <seealso cref= #setMaximum </seealso>
		Public Overridable Sub setValues(ByVal newValue As Integer, ByVal newExtent As Integer, ByVal newMin As Integer, ByVal newMax As Integer)
			Dim m As BoundedRangeModel = model
			Dim oldValue As Integer = m.value
			m.rangePropertiesies(newValue, newExtent, newMin, newMax, m.valueIsAdjusting)

			If accessibleContext IsNot Nothing Then accessibleContext.firePropertyChange(AccessibleContext.ACCESSIBLE_VALUE_PROPERTY, Convert.ToInt32(oldValue), Convert.ToInt32(m.value))
		End Sub


		''' <summary>
		''' Adds an AdjustmentListener.  Adjustment listeners are notified
		''' each time the scrollbar's model changes.  Adjustment events are
		''' provided for backwards compatibility with java.awt.Scrollbar.
		''' <p>
		''' Note that the AdjustmentEvents type property will always have a
		''' placeholder value of AdjustmentEvent.TRACK because all changes
		''' to a BoundedRangeModels value are considered equivalent.  To change
		''' the value of a BoundedRangeModel one just sets its value property,
		''' i.e. model.setValue(123).  No information about the origin of the
		''' change, e.g. it's a block decrement, is provided.  We don't try
		''' fabricate the origin of the change here.
		''' </summary>
		''' <param name="l"> the AdjustmentLister to add </param>
		''' <seealso cref= #removeAdjustmentListener </seealso>
		''' <seealso cref= BoundedRangeModel#addChangeListener </seealso>
		Public Overridable Sub addAdjustmentListener(ByVal l As java.awt.event.AdjustmentListener)
			listenerList.add(GetType(java.awt.event.AdjustmentListener), l)
		End Sub


		''' <summary>
		''' Removes an AdjustmentEvent listener.
		''' </summary>
		''' <param name="l"> the AdjustmentLister to remove </param>
		''' <seealso cref= #addAdjustmentListener </seealso>
		Public Overridable Sub removeAdjustmentListener(ByVal l As java.awt.event.AdjustmentListener)
			listenerList.remove(GetType(java.awt.event.AdjustmentListener), l)
		End Sub


		''' <summary>
		''' Returns an array of all the <code>AdjustmentListener</code>s added
		''' to this JScrollBar with addAdjustmentListener().
		''' </summary>
		''' <returns> all of the <code>AdjustmentListener</code>s added or an empty
		'''         array if no listeners have been added
		''' @since 1.4 </returns>
		Public Overridable Property adjustmentListeners As java.awt.event.AdjustmentListener()
			Get
				Return listenerList.getListeners(GetType(java.awt.event.AdjustmentListener))
			End Get
		End Property


		''' <summary>
		''' Notify listeners that the scrollbar's model has changed.
		''' </summary>
		''' <seealso cref= #addAdjustmentListener </seealso>
		''' <seealso cref= EventListenerList </seealso>
		Protected Friend Overridable Sub fireAdjustmentValueChanged(ByVal id As Integer, ByVal type As Integer, ByVal value As Integer)
			fireAdjustmentValueChanged(id, type, value, valueIsAdjusting)
		End Sub

		''' <summary>
		''' Notify listeners that the scrollbar's model has changed.
		''' </summary>
		''' <seealso cref= #addAdjustmentListener </seealso>
		''' <seealso cref= EventListenerList </seealso>
		Private Sub fireAdjustmentValueChanged(ByVal id As Integer, ByVal type As Integer, ByVal value As Integer, ByVal isAdjusting As Boolean)
			Dim ___listeners As Object() = listenerList.listenerList
			Dim e As java.awt.event.AdjustmentEvent = Nothing
			For i As Integer = ___listeners.Length - 2 To 0 Step -2
				If ___listeners(i) Is GetType(java.awt.event.AdjustmentListener) Then
					If e Is Nothing Then e = New java.awt.event.AdjustmentEvent(Me, id, type, value, isAdjusting)
					CType(___listeners(i+1), java.awt.event.AdjustmentListener).adjustmentValueChanged(e)
				End If
			Next i
		End Sub


		''' <summary>
		''' This class listens to ChangeEvents on the model and forwards
		''' AdjustmentEvents for the sake of backwards compatibility.
		''' Unfortunately there's no way to determine the proper
		''' type of the AdjustmentEvent as all updates to the model's
		''' value are considered equivalent.
		''' </summary>
		<Serializable> _
		Private Class ModelListener
			Implements ChangeListener

			Private ReadOnly outerInstance As JScrollBar

			Public Sub New(ByVal outerInstance As JScrollBar)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Sub stateChanged(ByVal e As ChangeEvent) Implements ChangeListener.stateChanged
				Dim obj As Object = e.source
				If TypeOf obj Is BoundedRangeModel Then
					Dim id As Integer = java.awt.event.AdjustmentEvent.ADJUSTMENT_VALUE_CHANGED
					Dim type As Integer = java.awt.event.AdjustmentEvent.TRACK
					Dim model As BoundedRangeModel = CType(obj, BoundedRangeModel)
					Dim value As Integer = model.value
					Dim isAdjusting As Boolean = model.valueIsAdjusting
					outerInstance.fireAdjustmentValueChanged(id, type, value, isAdjusting)
				End If
			End Sub
		End Class

		' PENDING(hmuller) - the next three methods should be removed

		''' <summary>
		''' The scrollbar is flexible along it's scrolling axis and
		''' rigid along the other axis.
		''' </summary>
		Public Property Overrides minimumSize As java.awt.Dimension
			Get
				Dim pref As java.awt.Dimension = preferredSize
				If orientation = VERTICAL Then
					Return New java.awt.Dimension(pref.width, 5)
				Else
					Return New java.awt.Dimension(5, pref.height)
				End If
			End Get
		End Property

		''' <summary>
		''' The scrollbar is flexible along it's scrolling axis and
		''' rigid along the other axis.
		''' </summary>
		Public Property Overrides maximumSize As java.awt.Dimension
			Get
				Dim pref As java.awt.Dimension = preferredSize
				If orientation = VERTICAL Then
					Return New java.awt.Dimension(pref.width, Short.MaxValue)
				Else
					Return New java.awt.Dimension(Short.MaxValue, pref.height)
				End If
			End Get
		End Property

		''' <summary>
		''' Enables the component so that the knob position can be changed.
		''' When the disabled, the knob position cannot be changed.
		''' </summary>
		''' <param name="x"> a boolean value, where true enables the component and
		'''          false disables it </param>
		Public Overrides Property enabled As Boolean
			Set(ByVal x As Boolean)
				MyBase.enabled = x
				Dim children As java.awt.Component() = components
				For Each child As java.awt.Component In children
					child.enabled = x
				Next child
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
		''' Returns a string representation of this JScrollBar. This method
		''' is intended to be used only for debugging purposes, and the
		''' content and format of the returned string may vary between
		''' implementations. The returned string may be empty but may not
		''' be <code>null</code>.
		''' </summary>
		''' <returns>  a string representation of this JScrollBar. </returns>
		Protected Friend Overrides Function paramString() As String
			Dim orientationString As String = (If(orientation = HORIZONTAL, "HORIZONTAL", "VERTICAL"))

			Return MyBase.paramString() & ",blockIncrement=" & blockIncrement & ",orientation=" & orientationString & ",unitIncrement=" & unitIncrement
		End Function

	'///////////////
	' Accessibility support
	'//////////////

		''' <summary>
		''' Gets the AccessibleContext associated with this JScrollBar.
		''' For JScrollBar, the AccessibleContext takes the form of an
		''' AccessibleJScrollBar.
		''' A new AccessibleJScrollBar instance is created if necessary.
		''' </summary>
		''' <returns> an AccessibleJScrollBar that serves as the
		'''         AccessibleContext of this JScrollBar </returns>
		Public Overridable Property accessibleContext As AccessibleContext Implements Accessible.getAccessibleContext
			Get
				If accessibleContext Is Nothing Then accessibleContext = New AccessibleJScrollBar(Me)
				Return accessibleContext
			End Get
		End Property

		''' <summary>
		''' This class implements accessibility support for the
		''' <code>JScrollBar</code> class.  It provides an implementation of the
		''' Java Accessibility API appropriate to scroll bar user-interface
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
		Protected Friend Class AccessibleJScrollBar
			Inherits AccessibleJComponent
			Implements AccessibleValue

			Private ReadOnly outerInstance As JScrollBar

			Public Sub New(ByVal outerInstance As JScrollBar)
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
			''' <returns> an instance of AccessibleRole describing the role of the
			''' object </returns>
			Public Overridable Property accessibleRole As AccessibleRole
				Get
					Return AccessibleRole.SCROLL_BAR
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
					Return New Integer?(outerInstance.model.maximum - outerInstance.model.extent)
				End Get
			End Property

		End Class ' AccessibleJScrollBar
	End Class

End Namespace
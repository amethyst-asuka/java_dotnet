Imports Microsoft.VisualBasic
Imports System
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
	''' <code>JSplitPane</code> is used to divide two (and only two)
	''' <code>Component</code>s. The two <code>Component</code>s
	''' are graphically divided based on the look and feel
	''' implementation, and the two <code>Component</code>s can then be
	''' interactively resized by the user.
	''' Information on using <code>JSplitPane</code> is in
	''' <a
	''' href="https://docs.oracle.com/javase/tutorial/uiswing/components/splitpane.html">How to Use Split Panes</a> in
	''' <em>The Java Tutorial</em>.
	''' <p>
	''' The two <code>Component</code>s in a split pane can be aligned
	''' left to right using
	''' <code>JSplitPane.HORIZONTAL_SPLIT</code>, or top to bottom using
	''' <code>JSplitPane.VERTICAL_SPLIT</code>.
	''' The preferred way to change the size of the <code>Component</code>s
	''' is to invoke
	''' <code>setDividerLocation</code> where <code>location</code> is either
	''' the new x or y position, depending on the orientation of the
	''' <code>JSplitPane</code>.
	''' <p>
	''' To resize the <code>Component</code>s to their preferred sizes invoke
	''' <code>resetToPreferredSizes</code>.
	''' <p>
	''' When the user is resizing the <code>Component</code>s the minimum
	''' size of the <code>Components</code> is used to determine the
	''' maximum/minimum position the <code>Component</code>s
	''' can be set to. If the minimum size of the two
	''' components is greater than the size of the split pane the divider
	''' will not allow you to resize it. To alter the minimum size of a
	''' <code>JComponent</code>, see <seealso cref="JComponent#setMinimumSize"/>.
	''' <p>
	''' When the user resizes the split pane the new space is distributed between
	''' the two components based on the <code>resizeWeight</code> property.
	''' A value of 0,
	''' the default, indicates the right/bottom component gets all the space,
	''' where as a value of 1 indicates the left/top component gets all the space.
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
	''' <seealso cref= #setDividerLocation </seealso>
	''' <seealso cref= #resetToPreferredSizes
	''' 
	''' @author Scott Violet </seealso>
	Public Class JSplitPane
		Inherits JComponent
		Implements Accessible

		''' <seealso cref= #getUIClassID </seealso>
		''' <seealso cref= #readObject </seealso>
		Private Const uiClassID As String = "SplitPaneUI"

		''' <summary>
		''' Vertical split indicates the <code>Component</code>s are
		''' split along the y axis.  For example the two
		''' <code>Component</code>s will be split one on top of the other.
		''' </summary>
		Public Const VERTICAL_SPLIT As Integer = 0

		''' <summary>
		''' Horizontal split indicates the <code>Component</code>s are
		''' split along the x axis.  For example the two
		''' <code>Component</code>s will be split one to the left of the
		''' other.
		''' </summary>
		Public Const HORIZONTAL_SPLIT As Integer = 1

		''' <summary>
		''' Used to add a <code>Component</code> to the left of the other
		''' <code>Component</code>.
		''' </summary>
		Public Const LEFT As String = "left"

		''' <summary>
		''' Used to add a <code>Component</code> to the right of the other
		''' <code>Component</code>.
		''' </summary>
		Public Const RIGHT As String = "right"

		''' <summary>
		''' Used to add a <code>Component</code> above the other
		''' <code>Component</code>.
		''' </summary>
		Public Const TOP As String = "top"

		''' <summary>
		''' Used to add a <code>Component</code> below the other
		''' <code>Component</code>.
		''' </summary>
		Public Const BOTTOM As String = "bottom"

		''' <summary>
		''' Used to add a <code>Component</code> that will represent the divider.
		''' </summary>
		Public Const DIVIDER As String = "divider"

		''' <summary>
		''' Bound property name for orientation (horizontal or vertical).
		''' </summary>
		Public Const ORIENTATION_PROPERTY As String = "orientation"

		''' <summary>
		''' Bound property name for continuousLayout.
		''' </summary>
		Public Const CONTINUOUS_LAYOUT_PROPERTY As String = "continuousLayout"

		''' <summary>
		''' Bound property name for border.
		''' </summary>
		Public Const DIVIDER_SIZE_PROPERTY As String = "dividerSize"

		''' <summary>
		''' Bound property for oneTouchExpandable.
		''' </summary>
		Public Const ONE_TOUCH_EXPANDABLE_PROPERTY As String = "oneTouchExpandable"

		''' <summary>
		''' Bound property for lastLocation.
		''' </summary>
		Public Const LAST_DIVIDER_LOCATION_PROPERTY As String = "lastDividerLocation"

		''' <summary>
		''' Bound property for the dividerLocation.
		''' @since 1.3
		''' </summary>
		Public Const DIVIDER_LOCATION_PROPERTY As String = "dividerLocation"

		''' <summary>
		''' Bound property for weight.
		''' @since 1.3
		''' </summary>
		Public Const RESIZE_WEIGHT_PROPERTY As String = "resizeWeight"

		''' <summary>
		''' How the views are split.
		''' </summary>
		Protected Friend orientation As Integer

		''' <summary>
		''' Whether or not the views are continuously redisplayed while
		''' resizing.
		''' </summary>
		Protected Friend continuousLayout As Boolean

		''' <summary>
		''' The left or top component.
		''' </summary>
		Protected Friend leftComponent As Component

		''' <summary>
		''' The right or bottom component.
		''' </summary>
		Protected Friend rightComponent As Component

		''' <summary>
		''' Size of the divider.
		''' </summary>
		Protected Friend dividerSize As Integer
		Private dividerSizeSet As Boolean = False

		''' <summary>
		''' Is a little widget provided to quickly expand/collapse the
		''' split pane?
		''' </summary>
		Protected Friend oneTouchExpandable As Boolean
		Private oneTouchExpandableSet As Boolean

		''' <summary>
		''' Previous location of the split pane.
		''' </summary>
		Protected Friend lastDividerLocation As Integer

		''' <summary>
		''' How to distribute extra space.
		''' </summary>
		Private resizeWeight As Double

		''' <summary>
		''' Location of the divider, at least the value that was set, the UI may
		''' have a different value.
		''' </summary>
		Private dividerLocation As Integer


		''' <summary>
		''' Creates a new <code>JSplitPane</code> configured to arrange the child
		''' components side-by-side horizontally, using two buttons for the components.
		''' </summary>
		Public Sub New()
			Me.New(JSplitPane.HORIZONTAL_SPLIT, UIManager.getBoolean("SplitPane.continuousLayout"), New JButton(UIManager.getString("SplitPane.leftButtonText")), New JButton(UIManager.getString("SplitPane.rightButtonText")))
		End Sub


		''' <summary>
		''' Creates a new <code>JSplitPane</code> configured with the
		''' specified orientation.
		''' </summary>
		''' <param name="newOrientation">  <code>JSplitPane.HORIZONTAL_SPLIT</code> or
		'''                        <code>JSplitPane.VERTICAL_SPLIT</code> </param>
		''' <exception cref="IllegalArgumentException"> if <code>orientation</code>
		'''          is not one of HORIZONTAL_SPLIT or VERTICAL_SPLIT. </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Sub New(ByVal newOrientation As Integer)
			Me.New(newOrientation, UIManager.getBoolean("SplitPane.continuousLayout"))
		End Sub


		''' <summary>
		''' Creates a new <code>JSplitPane</code> with the specified
		''' orientation and redrawing style.
		''' </summary>
		''' <param name="newOrientation">  <code>JSplitPane.HORIZONTAL_SPLIT</code> or
		'''                        <code>JSplitPane.VERTICAL_SPLIT</code> </param>
		''' <param name="newContinuousLayout">  a boolean, true for the components to
		'''        redraw continuously as the divider changes position, false
		'''        to wait until the divider position stops changing to redraw </param>
		''' <exception cref="IllegalArgumentException"> if <code>orientation</code>
		'''          is not one of HORIZONTAL_SPLIT or VERTICAL_SPLIT </exception>
		Public Sub New(ByVal newOrientation As Integer, ByVal newContinuousLayout As Boolean)
			Me.New(newOrientation, newContinuousLayout, Nothing, Nothing)
		End Sub


		''' <summary>
		''' Creates a new <code>JSplitPane</code> with the specified
		''' orientation and the specified components.
		''' </summary>
		''' <param name="newOrientation">  <code>JSplitPane.HORIZONTAL_SPLIT</code> or
		'''                        <code>JSplitPane.VERTICAL_SPLIT</code> </param>
		''' <param name="newLeftComponent"> the <code>Component</code> that will
		'''          appear on the left
		'''          of a horizontally-split pane, or at the top of a
		'''          vertically-split pane </param>
		''' <param name="newRightComponent"> the <code>Component</code> that will
		'''          appear on the right
		'''          of a horizontally-split pane, or at the bottom of a
		'''          vertically-split pane </param>
		''' <exception cref="IllegalArgumentException"> if <code>orientation</code>
		'''          is not one of: HORIZONTAL_SPLIT or VERTICAL_SPLIT </exception>
		Public Sub New(ByVal newOrientation As Integer, ByVal newLeftComponent As Component, ByVal newRightComponent As Component)
			Me.New(newOrientation, UIManager.getBoolean("SplitPane.continuousLayout"), newLeftComponent, newRightComponent)
		End Sub


		''' <summary>
		''' Creates a new <code>JSplitPane</code> with the specified
		''' orientation and
		''' redrawing style, and with the specified components.
		''' </summary>
		''' <param name="newOrientation">  <code>JSplitPane.HORIZONTAL_SPLIT</code> or
		'''                        <code>JSplitPane.VERTICAL_SPLIT</code> </param>
		''' <param name="newContinuousLayout">  a boolean, true for the components to
		'''        redraw continuously as the divider changes position, false
		'''        to wait until the divider position stops changing to redraw </param>
		''' <param name="newLeftComponent"> the <code>Component</code> that will
		'''          appear on the left
		'''          of a horizontally-split pane, or at the top of a
		'''          vertically-split pane </param>
		''' <param name="newRightComponent"> the <code>Component</code> that will
		'''          appear on the right
		'''          of a horizontally-split pane, or at the bottom of a
		'''          vertically-split pane </param>
		''' <exception cref="IllegalArgumentException"> if <code>orientation</code>
		'''          is not one of HORIZONTAL_SPLIT or VERTICAL_SPLIT </exception>
		Public Sub New(ByVal newOrientation As Integer, ByVal newContinuousLayout As Boolean, ByVal newLeftComponent As Component, ByVal newRightComponent As Component)
			MyBase.New()

			dividerLocation = -1
			layout = Nothing
			uIPropertyrty("opaque", Boolean.TRUE)
			orientation = newOrientation
			If orientation <> HORIZONTAL_SPLIT AndAlso orientation <> VERTICAL_SPLIT Then Throw New System.ArgumentException("cannot create JSplitPane, " & "orientation must be one of " & "JSplitPane.HORIZONTAL_SPLIT " & "or JSplitPane.VERTICAL_SPLIT")
			continuousLayout = newContinuousLayout
			If newLeftComponent IsNot Nothing Then leftComponent = newLeftComponent
			If newRightComponent IsNot Nothing Then rightComponent = newRightComponent
			updateUI()

		End Sub


		''' <summary>
		''' Sets the L&amp;F object that renders this component.
		''' </summary>
		''' <param name="ui">  the <code>SplitPaneUI</code> L&amp;F object </param>
		''' <seealso cref= UIDefaults#getUI
		''' @beaninfo
		'''        bound: true
		'''       hidden: true
		'''    attribute: visualUpdate true
		'''  description: The UI object that implements the Component's LookAndFeel. </seealso>
		Public Overridable Property uI As SplitPaneUI
			Set(ByVal ui As SplitPaneUI)
				If CType(Me.ui, SplitPaneUI) IsNot ui Then
					MyBase.uI = ui
					revalidate()
				End If
			End Set
			Get
				Return CType(ui, SplitPaneUI)
			End Get
		End Property




		''' <summary>
		''' Notification from the <code>UIManager</code> that the L&amp;F has changed.
		''' Replaces the current UI object with the latest version from the
		''' <code>UIManager</code>.
		''' </summary>
		''' <seealso cref= JComponent#updateUI </seealso>
		Public Overrides Sub updateUI()
			uI = CType(UIManager.getUI(Me), SplitPaneUI)
			revalidate()
		End Sub


		''' <summary>
		''' Returns the name of the L&amp;F class that renders this component.
		''' </summary>
		''' <returns> the string "SplitPaneUI" </returns>
		''' <seealso cref= JComponent#getUIClassID </seealso>
		''' <seealso cref= UIDefaults#getUI
		''' @beaninfo
		'''       expert: true
		'''  description: A string that specifies the name of the L&amp;F class. </seealso>
		Public Property Overrides uIClassID As String
			Get
				Return uiClassID
			End Get
		End Property


		''' <summary>
		''' Sets the size of the divider.
		''' </summary>
		''' <param name="newSize"> an integer giving the size of the divider in pixels
		''' @beaninfo
		'''        bound: true
		'''  description: The size of the divider. </param>
		Public Overridable Property dividerSize As Integer
			Set(ByVal newSize As Integer)
				Dim oldSize As Integer = dividerSize
    
				dividerSizeSet = True
				If oldSize <> newSize Then
					dividerSize = newSize
					firePropertyChange(DIVIDER_SIZE_PROPERTY, oldSize, newSize)
				End If
			End Set
			Get
				Return dividerSize
			End Get
		End Property




		''' <summary>
		''' Sets the component to the left (or above) the divider.
		''' </summary>
		''' <param name="comp"> the <code>Component</code> to display in that position </param>
		Public Overridable Property leftComponent As Component
			Set(ByVal comp As Component)
				If comp Is Nothing Then
					If leftComponent IsNot Nothing Then
						remove(leftComponent)
						leftComponent = Nothing
					End If
				Else
					add(comp, JSplitPane.LEFT)
				End If
			End Set
			Get
				Return leftComponent
			End Get
		End Property




		''' <summary>
		''' Sets the component above, or to the left of the divider.
		''' </summary>
		''' <param name="comp"> the <code>Component</code> to display in that position
		''' @beaninfo
		'''  description: The component above, or to the left of the divider. </param>
		Public Overridable Property topComponent As Component
			Set(ByVal comp As Component)
				leftComponent = comp
			End Set
			Get
				Return leftComponent
			End Get
		End Property




		''' <summary>
		''' Sets the component to the right (or below) the divider.
		''' </summary>
		''' <param name="comp"> the <code>Component</code> to display in that position
		''' @beaninfo
		'''    preferred: true
		'''  description: The component to the right (or below) the divider. </param>
		Public Overridable Property rightComponent As Component
			Set(ByVal comp As Component)
				If comp Is Nothing Then
					If rightComponent IsNot Nothing Then
						remove(rightComponent)
						rightComponent = Nothing
					End If
				Else
					add(comp, JSplitPane.RIGHT)
				End If
			End Set
			Get
				Return rightComponent
			End Get
		End Property




		''' <summary>
		''' Sets the component below, or to the right of the divider.
		''' </summary>
		''' <param name="comp"> the <code>Component</code> to display in that position
		''' @beaninfo
		'''  description: The component below, or to the right of the divider. </param>
		Public Overridable Property bottomComponent As Component
			Set(ByVal comp As Component)
				rightComponent = comp
			End Set
			Get
				Return rightComponent
			End Get
		End Property




		''' <summary>
		''' Sets the value of the <code>oneTouchExpandable</code> property,
		''' which must be <code>true</code> for the
		''' <code>JSplitPane</code> to provide a UI widget
		''' on the divider to quickly expand/collapse the divider.
		''' The default value of this property is <code>false</code>.
		''' Some look and feels might not support one-touch expanding;
		''' they will ignore this property.
		''' </summary>
		''' <param name="newValue"> <code>true</code> to specify that the split pane should provide a
		'''        collapse/expand widget
		''' @beaninfo
		'''        bound: true
		'''  description: UI widget on the divider to quickly
		'''               expand/collapse the divider.
		''' </param>
		''' <seealso cref= #isOneTouchExpandable </seealso>
		Public Overridable Property oneTouchExpandable As Boolean
			Set(ByVal newValue As Boolean)
				Dim oldValue As Boolean = oneTouchExpandable
    
				oneTouchExpandable = newValue
				oneTouchExpandableSet = True
				firePropertyChange(ONE_TOUCH_EXPANDABLE_PROPERTY, oldValue, newValue)
				repaint()
			End Set
			Get
				Return oneTouchExpandable
			End Get
		End Property




		''' <summary>
		''' Sets the last location the divider was at to
		''' <code>newLastLocation</code>.
		''' </summary>
		''' <param name="newLastLocation"> an integer specifying the last divider location
		'''        in pixels, from the left (or upper) edge of the pane to the
		'''        left (or upper) edge of the divider
		''' @beaninfo
		'''        bound: true
		'''  description: The last location the divider was at. </param>
		Public Overridable Property lastDividerLocation As Integer
			Set(ByVal newLastLocation As Integer)
				Dim oldLocation As Integer = lastDividerLocation
    
				lastDividerLocation = newLastLocation
				firePropertyChange(LAST_DIVIDER_LOCATION_PROPERTY, oldLocation, newLastLocation)
			End Set
			Get
				Return lastDividerLocation
			End Get
		End Property




		''' <summary>
		''' Sets the orientation, or how the splitter is divided. The options
		''' are:<ul>
		''' <li>JSplitPane.VERTICAL_SPLIT  (above/below orientation of components)
		''' <li>JSplitPane.HORIZONTAL_SPLIT  (left/right orientation of components)
		''' </ul>
		''' </summary>
		''' <param name="orientation"> an integer specifying the orientation </param>
		''' <exception cref="IllegalArgumentException"> if orientation is not one of:
		'''        HORIZONTAL_SPLIT or VERTICAL_SPLIT.
		''' @beaninfo
		'''        bound: true
		'''  description: The orientation, or how the splitter is divided.
		'''         enum: HORIZONTAL_SPLIT JSplitPane.HORIZONTAL_SPLIT
		'''               VERTICAL_SPLIT   JSplitPane.VERTICAL_SPLIT </exception>
		Public Overridable Property orientation As Integer
			Set(ByVal orientation As Integer)
				If (orientation <> VERTICAL_SPLIT) AndAlso (orientation <> HORIZONTAL_SPLIT) Then Throw New System.ArgumentException("JSplitPane: orientation must " & "be one of " & "JSplitPane.VERTICAL_SPLIT or " & "JSplitPane.HORIZONTAL_SPLIT")
    
				Dim oldOrientation As Integer = Me.orientation
    
				Me.orientation = orientation
				firePropertyChange(ORIENTATION_PROPERTY, oldOrientation, orientation)
			End Set
			Get
				Return orientation
			End Get
		End Property




		''' <summary>
		''' Sets the value of the <code>continuousLayout</code> property,
		''' which must be <code>true</code> for the child components
		''' to be continuously
		''' redisplayed and laid out during user intervention.
		''' The default value of this property is look and feel dependent.
		''' Some look and feels might not support continuous layout;
		''' they will ignore this property.
		''' </summary>
		''' <param name="newContinuousLayout">  <code>true</code> if the components
		'''        should continuously be redrawn as the divider changes position
		''' @beaninfo
		'''        bound: true
		'''  description: Whether the child components are
		'''               continuously redisplayed and laid out during
		'''               user intervention. </param>
		''' <seealso cref= #isContinuousLayout </seealso>
		Public Overridable Property continuousLayout As Boolean
			Set(ByVal newContinuousLayout As Boolean)
				Dim oldCD As Boolean = continuousLayout
    
				continuousLayout = newContinuousLayout
				firePropertyChange(CONTINUOUS_LAYOUT_PROPERTY, oldCD, newContinuousLayout)
			End Set
			Get
				Return continuousLayout
			End Get
		End Property



		''' <summary>
		''' Specifies how to distribute extra space when the size of the split pane
		''' changes. A value of 0, the default,
		''' indicates the right/bottom component gets all the extra space (the
		''' left/top component acts fixed), where as a value of 1 specifies the
		''' left/top component gets all the extra space (the right/bottom component
		''' acts fixed). Specifically, the left/top component gets (weight * diff)
		''' extra space and the right/bottom component gets (1 - weight) * diff
		''' extra space.
		''' </summary>
		''' <param name="value"> as described above </param>
		''' <exception cref="IllegalArgumentException"> if <code>value</code> is &lt; 0 or &gt; 1
		''' @since 1.3
		''' @beaninfo
		'''        bound: true
		'''  description: Specifies how to distribute extra space when the split pane
		'''               resizes. </exception>
		Public Overridable Property resizeWeight As Double
			Set(ByVal value As Double)
				If value < 0 OrElse value > 1 Then Throw New System.ArgumentException("JSplitPane weight must be between 0 and 1")
				Dim oldWeight As Double = resizeWeight
    
				resizeWeight = value
				firePropertyChange(RESIZE_WEIGHT_PROPERTY, oldWeight, value)
			End Set
			Get
				Return resizeWeight
			End Get
		End Property


		''' <summary>
		''' Lays out the <code>JSplitPane</code> layout based on the preferred size
		''' of the children components. This will likely result in changing
		''' the divider location.
		''' </summary>
		Public Overridable Sub resetToPreferredSizes()
			Dim ___ui As SplitPaneUI = uI

			If ___ui IsNot Nothing Then ___ui.resetToPreferredSizes(Me)
		End Sub


		''' <summary>
		''' Sets the divider location as a percentage of the
		''' <code>JSplitPane</code>'s size.
		''' <p>
		''' This method is implemented in terms of
		''' <code>setDividerLocation(int)</code>.
		''' This method immediately changes the size of the split pane based on
		''' its current size. If the split pane is not correctly realized and on
		''' screen, this method will have no effect (new divider location will
		''' become (current size * proportionalLocation) which is 0).
		''' </summary>
		''' <param name="proportionalLocation">  a double-precision floating point value
		'''        that specifies a percentage, from zero (top/left) to 1.0
		'''        (bottom/right) </param>
		''' <exception cref="IllegalArgumentException"> if the specified location is &lt; 0
		'''            or &gt; 1.0
		''' @beaninfo
		'''  description: The location of the divider. </exception>
		Public Overridable Property dividerLocation As Double
			Set(ByVal proportionalLocation As Double)
				If proportionalLocation < 0.0 OrElse proportionalLocation > 1.0 Then Throw New System.ArgumentException("proportional location must " & "be between 0.0 and 1.0.")
				If orientation = VERTICAL_SPLIT Then
					dividerLocation = CInt(Fix(CDbl(height - dividerSize) * proportionalLocation))
				Else
					dividerLocation = CInt(Fix(CDbl(width - dividerSize) * proportionalLocation))
				End If
			End Set
			Get
				Return dividerLocation
			End Get
		End Property


		''' <summary>
		''' Sets the location of the divider. This is passed off to the
		''' look and feel implementation, and then listeners are notified. A value
		''' less than 0 implies the divider should be reset to a value that
		''' attempts to honor the preferred size of the left/top component.
		''' After notifying the listeners, the last divider location is updated,
		''' via <code>setLastDividerLocation</code>.
		''' </summary>
		''' <param name="location"> an int specifying a UI-specific value (typically a
		'''        pixel count)
		''' @beaninfo
		'''        bound: true
		'''  description: The location of the divider. </param>
		Public Overridable Property dividerLocation As Integer
			Set(ByVal location As Integer)
				Dim oldValue As Integer = dividerLocation
    
				dividerLocation = location
    
				' Notify UI.
				Dim ___ui As SplitPaneUI = uI
    
				If ___ui IsNot Nothing Then ___ui.dividerLocationion(Me, location)
    
				' Then listeners
				firePropertyChange(DIVIDER_LOCATION_PROPERTY, oldValue, location)
    
				' And update the last divider location.
				lastDividerLocation = oldValue
			End Set
		End Property




		''' <summary>
		''' Returns the minimum location of the divider from the look and feel
		''' implementation.
		''' </summary>
		''' <returns> an integer specifying a UI-specific value for the minimum
		'''          location (typically a pixel count); or -1 if the UI is
		'''          <code>null</code>
		''' @beaninfo
		'''  description: The minimum location of the divider from the L&amp;F. </returns>
		Public Overridable Property minimumDividerLocation As Integer
			Get
				Dim ___ui As SplitPaneUI = uI
    
				If ___ui IsNot Nothing Then Return ___ui.getMinimumDividerLocation(Me)
				Return -1
			End Get
		End Property


		''' <summary>
		''' Returns the maximum location of the divider from the look and feel
		''' implementation.
		''' </summary>
		''' <returns> an integer specifying a UI-specific value for the maximum
		'''          location (typically a pixel count); or -1 if the  UI is
		'''          <code>null</code> </returns>
		Public Overridable Property maximumDividerLocation As Integer
			Get
				Dim ___ui As SplitPaneUI = uI
    
				If ___ui IsNot Nothing Then Return ___ui.getMaximumDividerLocation(Me)
				Return -1
			End Get
		End Property


		''' <summary>
		''' Removes the child component, <code>component</code> from the
		''' pane. Resets the <code>leftComponent</code> or
		''' <code>rightComponent</code> instance variable, as necessary.
		''' </summary>
		''' <param name="component"> the <code>Component</code> to remove </param>
		Public Overridable Sub remove(ByVal component As Component)
			If component Is leftComponent Then
				leftComponent = Nothing
			ElseIf component Is rightComponent Then
				rightComponent = Nothing
			End If
			MyBase.remove(component)

			' Update the JSplitPane on the screen
			revalidate()
			repaint()
		End Sub


		''' <summary>
		''' Removes the <code>Component</code> at the specified index.
		''' Updates the <code>leftComponent</code> and <code>rightComponent</code>
		''' instance variables as necessary, and then messages super.
		''' </summary>
		''' <param name="index"> an integer specifying the component to remove, where
		'''        1 specifies the left/top component and 2 specifies the
		'''        bottom/right component </param>
		Public Overridable Sub remove(ByVal index As Integer)
			Dim comp As Component = getComponent(index)

			If comp Is leftComponent Then
				leftComponent = Nothing
			ElseIf comp Is rightComponent Then
				rightComponent = Nothing
			End If
			MyBase.remove(index)

			' Update the JSplitPane on the screen
			revalidate()
			repaint()
		End Sub


		''' <summary>
		''' Removes all the child components from the split pane. Resets the
		''' <code>leftComonent</code> and <code>rightComponent</code>
		''' instance variables.
		''' </summary>
		Public Overridable Sub removeAll()
				rightComponent = Nothing
				leftComponent = rightComponent
			MyBase.removeAll()

			' Update the JSplitPane on the screen
			revalidate()
			repaint()
		End Sub


		''' <summary>
		''' Returns true, so that calls to <code>revalidate</code>
		''' on any descendant of this <code>JSplitPane</code>
		''' will cause a request to be queued that
		''' will validate the <code>JSplitPane</code> and all its descendants.
		''' </summary>
		''' <returns> true </returns>
		''' <seealso cref= JComponent#revalidate </seealso>
		''' <seealso cref= java.awt.Container#isValidateRoot
		''' 
		''' @beaninfo
		'''    hidden: true </seealso>
		Public Property Overrides validateRoot As Boolean
			Get
				Return True
			End Get
		End Property


		''' <summary>
		''' Adds the specified component to this split pane.
		''' If <code>constraints</code> identifies the left/top or
		''' right/bottom child component, and a component with that identifier
		''' was previously added, it will be removed and then <code>comp</code>
		''' will be added in its place. If <code>constraints</code> is not
		''' one of the known identifiers the layout manager may throw an
		''' <code>IllegalArgumentException</code>.
		''' <p>
		''' The possible constraints objects (Strings) are:
		''' <ul>
		''' <li>JSplitPane.TOP
		''' <li>JSplitPane.LEFT
		''' <li>JSplitPane.BOTTOM
		''' <li>JSplitPane.RIGHT
		''' </ul>
		''' If the <code>constraints</code> object is <code>null</code>,
		''' the component is added in the
		''' first available position (left/top if open, else right/bottom).
		''' </summary>
		''' <param name="comp">        the component to add </param>
		''' <param name="constraints"> an <code>Object</code> specifying the
		'''                    layout constraints
		'''                    (position) for this component </param>
		''' <param name="index">       an integer specifying the index in the container's
		'''                    list. </param>
		''' <exception cref="IllegalArgumentException">  if the <code>constraints</code>
		'''          object does not match an existing component </exception>
		''' <seealso cref= java.awt.Container#addImpl(Component, Object, int) </seealso>
		Protected Friend Overridable Sub addImpl(ByVal comp As Component, ByVal constraints As Object, ByVal index As Integer)
			Dim toRemove As Component

			If constraints IsNot Nothing AndAlso Not(TypeOf constraints Is String) Then Throw New System.ArgumentException("cannot add to layout: " & "constraint must be a string " & "(or null)")

	'         If the constraints are null and the left/right component is
	'           invalid, add it at the left/right component. 
			If constraints Is Nothing Then
				If leftComponent Is Nothing Then
					constraints = JSplitPane.LEFT
				ElseIf rightComponent Is Nothing Then
					constraints = JSplitPane.RIGHT
				End If
			End If

			' Find the Component that already exists and remove it. 
			If constraints IsNot Nothing AndAlso (constraints.Equals(JSplitPane.LEFT) OrElse constraints.Equals(JSplitPane.TOP)) Then
				toRemove = leftComponent
				If toRemove IsNot Nothing Then remove(toRemove)
				leftComponent = comp
				index = -1
			ElseIf constraints IsNot Nothing AndAlso (constraints.Equals(JSplitPane.RIGHT) OrElse constraints.Equals(JSplitPane.BOTTOM)) Then
				toRemove = rightComponent
				If toRemove IsNot Nothing Then remove(toRemove)
				rightComponent = comp
				index = -1
			ElseIf constraints IsNot Nothing AndAlso constraints.Equals(JSplitPane.DIVIDER) Then
				index = -1
			End If
			' LayoutManager should raise for else condition here. 

			MyBase.addImpl(comp, constraints, index)

			' Update the JSplitPane on the screen
			revalidate()
			repaint()
		End Sub


		''' <summary>
		''' Subclassed to message the UI with <code>finishedPaintingChildren</code>
		''' after super has been messaged, as well as painting the border.
		''' </summary>
		''' <param name="g"> the <code>Graphics</code> context within which to paint </param>
		Protected Friend Overrides Sub paintChildren(ByVal g As Graphics)
			MyBase.paintChildren(g)

			Dim ___ui As SplitPaneUI = uI

			If ___ui IsNot Nothing Then
				Dim tempG As Graphics = g.create()
				___ui.finishedPaintingChildren(Me, tempG)
				tempG.Dispose()
			End If
		End Sub


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

		Friend Overrides Sub setUIProperty(ByVal propertyName As String, ByVal value As Object)
			If propertyName = "dividerSize" Then
				If Not dividerSizeSet Then
					dividerSize = CType(value, Number)
					dividerSizeSet = False
				End If
			ElseIf propertyName = "oneTouchExpandable" Then
				If Not oneTouchExpandableSet Then
					oneTouchExpandable = CBool(value)
					oneTouchExpandableSet = False
				End If
			Else
				MyBase.uIPropertyrty(propertyName, value)
			End If
		End Sub


		''' <summary>
		''' Returns a string representation of this <code>JSplitPane</code>.
		''' This method
		''' is intended to be used only for debugging purposes, and the
		''' content and format of the returned string may vary between
		''' implementations. The returned string may be empty but may not
		''' be <code>null</code>.
		''' </summary>
		''' <returns>  a string representation of this <code>JSplitPane</code>. </returns>
		Protected Friend Overrides Function paramString() As String
			Dim orientationString As String = (If(orientation = HORIZONTAL_SPLIT, "HORIZONTAL_SPLIT", "VERTICAL_SPLIT"))
			Dim continuousLayoutString As String = (If(continuousLayout, "true", "false"))
			Dim oneTouchExpandableString As String = (If(oneTouchExpandable, "true", "false"))

			Return MyBase.paramString() & ",continuousLayout=" & continuousLayoutString & ",dividerSize=" & dividerSize & ",lastDividerLocation=" & lastDividerLocation & ",oneTouchExpandable=" & oneTouchExpandableString & ",orientation=" & orientationString
		End Function



		'/////////////////////////
		' Accessibility support //
		'/////////////////////////


		''' <summary>
		''' Gets the AccessibleContext associated with this JSplitPane.
		''' For split panes, the AccessibleContext takes the form of an
		''' AccessibleJSplitPane.
		''' A new AccessibleJSplitPane instance is created if necessary.
		''' </summary>
		''' <returns> an AccessibleJSplitPane that serves as the
		'''         AccessibleContext of this JSplitPane
		''' @beaninfo
		'''       expert: true
		'''  description: The AccessibleContext associated with this SplitPane. </returns>
		Public Overridable Property accessibleContext As AccessibleContext Implements Accessible.getAccessibleContext
			Get
				If accessibleContext Is Nothing Then accessibleContext = New AccessibleJSplitPane(Me)
				Return accessibleContext
			End Get
		End Property


		''' <summary>
		''' This class implements accessibility support for the
		''' <code>JSplitPane</code> class.  It provides an implementation of the
		''' Java Accessibility API appropriate to split pane user-interface elements.
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
		Protected Friend Class AccessibleJSplitPane
			Inherits AccessibleJComponent
			Implements AccessibleValue

			Private ReadOnly outerInstance As JSplitPane

			Public Sub New(ByVal outerInstance As JSplitPane)
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
					' FIXME: [[[WDW - Should also add BUSY if this implements
					' Adjustable at some point.  If this happens, we probably
					' should also add actions.]]]
					If outerInstance.orientation = VERTICAL_SPLIT Then
						states.add(AccessibleState.VERTICAL)
					Else
						states.add(AccessibleState.HORIZONTAL)
					End If
					Return states
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
			''' Gets the accessible value of this object.
			''' </summary>
			''' <returns> a localized String describing the value of this object </returns>
			Public Overridable Property currentAccessibleValue As Number Implements AccessibleValue.getCurrentAccessibleValue
				Get
					Return Convert.ToInt32(outerInstance.dividerLocation)
				End Get
			End Property


			''' <summary>
			''' Sets the value of this object as a Number.
			''' </summary>
			''' <returns> True if the value was set. </returns>
			Public Overridable Function setCurrentAccessibleValue(ByVal n As Number) As Boolean Implements AccessibleValue.setCurrentAccessibleValue
				' TIGER - 4422535
				If n Is Nothing Then Return False
				outerInstance.dividerLocation = n
				Return True
			End Function


			''' <summary>
			''' Gets the minimum accessible value of this object.
			''' </summary>
			''' <returns> The minimum value of this object. </returns>
			Public Overridable Property minimumAccessibleValue As Number Implements AccessibleValue.getMinimumAccessibleValue
				Get
					Return Convert.ToInt32(outerInstance.uI.getMinimumDividerLocation(JSplitPane.this))
				End Get
			End Property


			''' <summary>
			''' Gets the maximum accessible value of this object.
			''' </summary>
			''' <returns> The maximum value of this object. </returns>
			Public Overridable Property maximumAccessibleValue As Number Implements AccessibleValue.getMaximumAccessibleValue
				Get
					Return Convert.ToInt32(outerInstance.uI.getMaximumDividerLocation(JSplitPane.this))
				End Get
			End Property


			''' <summary>
			''' Gets the role of this object.
			''' </summary>
			''' <returns> an instance of AccessibleRole describing the role of
			''' the object </returns>
			''' <seealso cref= AccessibleRole </seealso>
			Public Overridable Property accessibleRole As AccessibleRole
				Get
					Return AccessibleRole.SPLIT_PANE
				End Get
			End Property
		End Class ' inner class AccessibleJSplitPane
	End Class

End Namespace
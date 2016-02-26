Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Threading
Imports javax.swing.event
Imports javax.swing.border
Imports javax.swing.plaf
Imports javax.accessibility
Imports javax.swing.text
Imports javax.swing.text.html
Imports javax.swing.plaf.basic

'
' * Copyright (c) 1997, 2015, Oracle and/or its affiliates. All rights reserved.
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
	''' Defines common behaviors for buttons and menu items.
	''' <p>
	''' Buttons can be configured, and to some degree controlled, by
	''' <code><a href="Action.html">Action</a></code>s.  Using an
	''' <code>Action</code> with a button has many benefits beyond directly
	''' configuring a button.  Refer to <a href="Action.html#buttonActions">
	''' Swing Components Supporting <code>Action</code></a> for more
	''' details, and you can find more information in <a
	''' href="https://docs.oracle.com/javase/tutorial/uiswing/misc/action.html">How
	''' to Use Actions</a>, a section in <em>The Java Tutorial</em>.
	''' <p>
	''' For further information see
	''' <a
	''' href="https://docs.oracle.com/javase/tutorial/uiswing/components/button.html">How to Use Buttons, Check Boxes, and Radio Buttons</a>,
	''' a section in <em>The Java Tutorial</em>.
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
	''' @author Jeff Dinkins
	''' </summary>
	Public MustInherit Class AbstractButton
		Inherits JComponent
		Implements ItemSelectable, SwingConstants

		' *********************************
		' ******* Button properties *******
		' *********************************

		''' <summary>
		''' Identifies a change in the button model. </summary>
		Public Const MODEL_CHANGED_PROPERTY As String = "model"
		''' <summary>
		''' Identifies a change in the button's text. </summary>
		Public Const TEXT_CHANGED_PROPERTY As String = "text"
		''' <summary>
		''' Identifies a change to the button's mnemonic. </summary>
		Public Const MNEMONIC_CHANGED_PROPERTY As String = "mnemonic"

		' Text positioning and alignment
		''' <summary>
		''' Identifies a change in the button's margins. </summary>
		Public Const MARGIN_CHANGED_PROPERTY As String = "margin"
		''' <summary>
		''' Identifies a change in the button's vertical alignment. </summary>
		Public Const VERTICAL_ALIGNMENT_CHANGED_PROPERTY As String = "verticalAlignment"
		''' <summary>
		''' Identifies a change in the button's horizontal alignment. </summary>
		Public Const HORIZONTAL_ALIGNMENT_CHANGED_PROPERTY As String = "horizontalAlignment"

		''' <summary>
		''' Identifies a change in the button's vertical text position. </summary>
		Public Const VERTICAL_TEXT_POSITION_CHANGED_PROPERTY As String = "verticalTextPosition"
		''' <summary>
		''' Identifies a change in the button's horizontal text position. </summary>
		Public Const HORIZONTAL_TEXT_POSITION_CHANGED_PROPERTY As String = "horizontalTextPosition"

		' Paint options
		''' <summary>
		''' Identifies a change to having the border drawn,
		''' or having it not drawn.
		''' </summary>
		Public Const BORDER_PAINTED_CHANGED_PROPERTY As String = "borderPainted"
		''' <summary>
		''' Identifies a change to having the border highlighted when focused,
		''' or not.
		''' </summary>
		Public Const FOCUS_PAINTED_CHANGED_PROPERTY As String = "focusPainted"
		''' <summary>
		''' Identifies a change from rollover enabled to disabled or back
		''' to enabled.
		''' </summary>
		Public Const ROLLOVER_ENABLED_CHANGED_PROPERTY As String = "rolloverEnabled"
		''' <summary>
		''' Identifies a change to having the button paint the content area.
		''' </summary>
		Public Const CONTENT_AREA_FILLED_CHANGED_PROPERTY As String = "contentAreaFilled"

		' Icons
		''' <summary>
		''' Identifies a change to the icon that represents the button. </summary>
		Public Const ICON_CHANGED_PROPERTY As String = "icon"

		''' <summary>
		''' Identifies a change to the icon used when the button has been
		''' pressed.
		''' </summary>
		Public Const PRESSED_ICON_CHANGED_PROPERTY As String = "pressedIcon"
		''' <summary>
		''' Identifies a change to the icon used when the button has
		''' been selected.
		''' </summary>
		Public Const SELECTED_ICON_CHANGED_PROPERTY As String = "selectedIcon"

		''' <summary>
		''' Identifies a change to the icon used when the cursor is over
		''' the button.
		''' </summary>
		Public Const ROLLOVER_ICON_CHANGED_PROPERTY As String = "rolloverIcon"
		''' <summary>
		''' Identifies a change to the icon used when the cursor is
		''' over the button and it has been selected.
		''' </summary>
		Public Const ROLLOVER_SELECTED_ICON_CHANGED_PROPERTY As String = "rolloverSelectedIcon"

		''' <summary>
		''' Identifies a change to the icon used when the button has
		''' been disabled.
		''' </summary>
		Public Const DISABLED_ICON_CHANGED_PROPERTY As String = "disabledIcon"
		''' <summary>
		''' Identifies a change to the icon used when the button has been
		''' disabled and selected.
		''' </summary>
		Public Const DISABLED_SELECTED_ICON_CHANGED_PROPERTY As String = "disabledSelectedIcon"


		''' <summary>
		''' The data model that determines the button's state. </summary>
		Protected Friend model As ButtonModel = Nothing

		Private text As String = "" ' for BeanBox
		Private margin As Insets = Nothing
		Private defaultMargin As Insets = Nothing

		' Button icons
		' PENDING(jeff) - hold icons in an array
		Private defaultIcon As Icon = Nothing
		Private pressedIcon As Icon = Nothing
		Private disabledIcon As Icon = Nothing

		Private selectedIcon As Icon = Nothing
		Private disabledSelectedIcon As Icon = Nothing

		Private rolloverIcon As Icon = Nothing
		Private rolloverSelectedIcon As Icon = Nothing

		' Display properties
		Private ___paintBorder As Boolean = True
		Private paintFocus As Boolean = True
		Private rolloverEnabled As Boolean = False
		Private contentAreaFilled As Boolean = True

		' Icon/Label Alignment
		Private verticalAlignment As Integer = CENTER
		Private horizontalAlignment As Integer = CENTER

		Private verticalTextPosition As Integer = CENTER
		Private horizontalTextPosition As Integer = TRAILING

		Private iconTextGap As Integer = 4

		Private mnemonic As Integer
		Private mnemonicIndex As Integer = -1

		Private multiClickThreshhold As Long = 0

		Private borderPaintedSet As Boolean = False
		Private rolloverEnabledSet As Boolean = False
		Private iconTextGapSet As Boolean = False
		Private contentAreaFilledSet As Boolean = False

		' Whether or not we've set the LayoutManager.
		Private ___setLayout As Boolean = False

		' This is only used by JButton, promoted to avoid an extra
		' boolean field in JButton
		Friend defaultCapable As Boolean = True

		''' <summary>
		''' Combined listeners: ActionListener, ChangeListener, ItemListener.
		''' </summary>
		Private handler As Handler

		''' <summary>
		''' The button model's <code>changeListener</code>.
		''' </summary>
		Protected Friend changeListener As ChangeListener = Nothing
		''' <summary>
		''' The button model's <code>ActionListener</code>.
		''' </summary>
		Protected Friend actionListener As ActionListener = Nothing
		''' <summary>
		''' The button model's <code>ItemListener</code>.
		''' </summary>
		Protected Friend itemListener As ItemListener = Nothing

		''' <summary>
		''' Only one <code>ChangeEvent</code> is needed per button
		''' instance since the
		''' event's only state is the source property.  The source of events
		''' generated is always "this".
		''' </summary>
		<NonSerialized> _
		Protected Friend changeEvent As ChangeEvent

		Private hideActionText As Boolean = False

		''' <summary>
		''' Sets the <code>hideActionText</code> property, which determines
		''' whether the button displays text from the <code>Action</code>.
		''' This is useful only if an <code>Action</code> has been
		''' installed on the button.
		''' </summary>
		''' <param name="hideActionText"> <code>true</code> if the button's
		'''                       <code>text</code> property should not reflect
		'''                       that of the <code>Action</code>; the default is
		'''                       <code>false</code> </param>
		''' <seealso cref= <a href="Action.html#buttonActions">Swing Components Supporting
		'''      <code>Action</code></a>
		''' @since 1.6
		''' @beaninfo
		'''        bound: true
		'''    expert: true
		'''  description: Whether the text of the button should come from
		'''               the <code>Action</code>. </seealso>
		Public Overridable Property hideActionText As Boolean
			Set(ByVal hideActionText As Boolean)
				If hideActionText <> Me.hideActionText Then
					Me.hideActionText = hideActionText
					If action IsNot Nothing Then textFromActionion(action, False)
					firePropertyChange("hideActionText", (Not hideActionText), hideActionText)
				End If
			End Set
			Get
				Return hideActionText
			End Get
		End Property


		''' <summary>
		''' Returns the button's text. </summary>
		''' <returns> the buttons text </returns>
		''' <seealso cref= #setText </seealso>
		Public Overridable Property text As String
			Get
				Return text
			End Get
			Set(ByVal text As String)
				Dim oldValue As String = Me.text
				Me.text = text
				firePropertyChange(TEXT_CHANGED_PROPERTY, oldValue, text)
				updateDisplayedMnemonicIndex(text, mnemonic)
    
				If accessibleContext IsNot Nothing Then accessibleContext.firePropertyChange(AccessibleContext.ACCESSIBLE_VISIBLE_DATA_PROPERTY, oldValue, text)
				If text Is Nothing OrElse oldValue Is Nothing OrElse (Not text.Equals(oldValue)) Then
					revalidate()
					repaint()
				End If
			End Set
		End Property



		''' <summary>
		''' Returns the state of the button. True if the
		''' toggle button is selected, false if it's not. </summary>
		''' <returns> true if the toggle button is selected, otherwise false </returns>
		Public Overridable Property selected As Boolean
			Get
				Return model.selected
			End Get
			Set(ByVal b As Boolean)
				Dim oldValue As Boolean = selected
    
				' TIGER - 4840653
				' Removed code which fired an AccessibleState.SELECTED
				' PropertyChangeEvent since this resulted in two
				' identical events being fired since
				' AbstractButton.fireItemStateChanged also fires the
				' same event. This caused screen readers to speak the
				' name of the item twice.
    
				model.selected = b
			End Set
		End Property


		''' <summary>
		''' Programmatically perform a "click". This does the same
		''' thing as if the user had pressed and released the button.
		''' </summary>
		Public Overridable Sub doClick()
			doClick(68)
		End Sub

		''' <summary>
		''' Programmatically perform a "click". This does the same
		''' thing as if the user had pressed and released the button.
		''' The button stays visually "pressed" for <code>pressTime</code>
		'''  milliseconds.
		''' </summary>
		''' <param name="pressTime"> the time to "hold down" the button, in milliseconds </param>
		Public Overridable Sub doClick(ByVal pressTime As Integer)
			Dim ___size As Dimension = size
			model.armed = True
			model.pressed = True
			paintImmediately(New Rectangle(0,0, ___size.width, ___size.height))
			Try
				Thread.CurrentThread.sleep(pressTime)
			Catch ie As InterruptedException
			End Try
			model.pressed = False
			model.armed = False
		End Sub

		''' <summary>
		''' Sets space for margin between the button's border and
		''' the label. Setting to <code>null</code> will cause the button to
		''' use the default margin.  The button's default <code>Border</code>
		''' object will use this value to create the proper margin.
		''' However, if a non-default border is set on the button,
		''' it is that <code>Border</code> object's responsibility to create the
		''' appropriate margin space (else this property will
		''' effectively be ignored).
		''' </summary>
		''' <param name="m"> the space between the border and the label
		''' 
		''' @beaninfo
		'''        bound: true
		'''    attribute: visualUpdate true
		'''  description: The space between the button's border and the label. </param>
		Public Overridable Property margin As Insets
			Set(ByVal m As Insets)
				' Cache the old margin if it comes from the UI
				If TypeOf m Is UIResource Then
					defaultMargin = m
				ElseIf TypeOf margin Is UIResource Then
					defaultMargin = margin
				End If
    
				' If the client passes in a null insets, restore the margin
				' from the UI if possible
				If m Is Nothing AndAlso defaultMargin IsNot Nothing Then m = defaultMargin
    
				Dim old As Insets = margin
				margin = m
				firePropertyChange(MARGIN_CHANGED_PROPERTY, old, m)
				If old Is Nothing OrElse (Not old.Equals(m)) Then
					revalidate()
					repaint()
				End If
			End Set
			Get
				Return If(margin Is Nothing, Nothing, CType(margin.clone(), Insets))
			End Get
		End Property


		''' <summary>
		''' Returns the default icon. </summary>
		''' <returns> the default <code>Icon</code> </returns>
		''' <seealso cref= #setIcon </seealso>
		Public Overridable Property icon As Icon
			Get
				Return defaultIcon
			End Get
			Set(ByVal defaultIcon As Icon)
				Dim oldValue As Icon = Me.defaultIcon
				Me.defaultIcon = defaultIcon
    
		'         If the default icon has really changed and we had
		'         * generated the disabled icon for this component,
		'         * (i.e. setDisabledIcon() was never called) then
		'         * clear the disabledIcon field.
		'         
				If defaultIcon IsNot oldValue AndAlso (TypeOf disabledIcon Is UIResource) Then disabledIcon = Nothing
    
				firePropertyChange(ICON_CHANGED_PROPERTY, oldValue, defaultIcon)
				If accessibleContext IsNot Nothing Then accessibleContext.firePropertyChange(AccessibleContext.ACCESSIBLE_VISIBLE_DATA_PROPERTY, oldValue, defaultIcon)
				If defaultIcon IsNot oldValue Then
					If defaultIcon Is Nothing OrElse oldValue Is Nothing OrElse defaultIcon.iconWidth <> oldValue.iconWidth OrElse defaultIcon.iconHeight <> oldValue.iconHeight Then revalidate()
					repaint()
				End If
			End Set
		End Property


		''' <summary>
		''' Returns the pressed icon for the button. </summary>
		''' <returns> the <code>pressedIcon</code> property </returns>
		''' <seealso cref= #setPressedIcon </seealso>
		Public Overridable Property pressedIcon As Icon
			Get
				Return pressedIcon
			End Get
			Set(ByVal pressedIcon As Icon)
				Dim oldValue As Icon = Me.pressedIcon
				Me.pressedIcon = pressedIcon
				firePropertyChange(PRESSED_ICON_CHANGED_PROPERTY, oldValue, pressedIcon)
				If accessibleContext IsNot Nothing Then accessibleContext.firePropertyChange(AccessibleContext.ACCESSIBLE_VISIBLE_DATA_PROPERTY, oldValue, pressedIcon)
				If pressedIcon IsNot oldValue Then
					If model.pressed Then repaint()
				End If
			End Set
		End Property


		''' <summary>
		''' Returns the selected icon for the button. </summary>
		''' <returns> the <code>selectedIcon</code> property </returns>
		''' <seealso cref= #setSelectedIcon </seealso>
		Public Overridable Property selectedIcon As Icon
			Get
				Return selectedIcon
			End Get
			Set(ByVal selectedIcon As Icon)
				Dim oldValue As Icon = Me.selectedIcon
				Me.selectedIcon = selectedIcon
    
		'         If the default selected icon has really changed and we had
		'         * generated the disabled selected icon for this component,
		'         * (i.e. setDisabledSelectedIcon() was never called) then
		'         * clear the disabledSelectedIcon field.
		'         
				If selectedIcon IsNot oldValue AndAlso TypeOf disabledSelectedIcon Is UIResource Then disabledSelectedIcon = Nothing
    
				firePropertyChange(SELECTED_ICON_CHANGED_PROPERTY, oldValue, selectedIcon)
				If accessibleContext IsNot Nothing Then accessibleContext.firePropertyChange(AccessibleContext.ACCESSIBLE_VISIBLE_DATA_PROPERTY, oldValue, selectedIcon)
				If selectedIcon IsNot oldValue Then
					If selected Then repaint()
				End If
			End Set
		End Property


		''' <summary>
		''' Returns the rollover icon for the button. </summary>
		''' <returns> the <code>rolloverIcon</code> property </returns>
		''' <seealso cref= #setRolloverIcon </seealso>
		Public Overridable Property rolloverIcon As Icon
			Get
				Return rolloverIcon
			End Get
			Set(ByVal rolloverIcon As Icon)
				Dim oldValue As Icon = Me.rolloverIcon
				Me.rolloverIcon = rolloverIcon
				firePropertyChange(ROLLOVER_ICON_CHANGED_PROPERTY, oldValue, rolloverIcon)
				If accessibleContext IsNot Nothing Then accessibleContext.firePropertyChange(AccessibleContext.ACCESSIBLE_VISIBLE_DATA_PROPERTY, oldValue, rolloverIcon)
				rolloverEnabled = True
				If rolloverIcon IsNot oldValue Then repaint()
    
			End Set
		End Property


		''' <summary>
		''' Returns the rollover selection icon for the button. </summary>
		''' <returns> the <code>rolloverSelectedIcon</code> property </returns>
		''' <seealso cref= #setRolloverSelectedIcon </seealso>
		Public Overridable Property rolloverSelectedIcon As Icon
			Get
				Return rolloverSelectedIcon
			End Get
			Set(ByVal rolloverSelectedIcon As Icon)
				Dim oldValue As Icon = Me.rolloverSelectedIcon
				Me.rolloverSelectedIcon = rolloverSelectedIcon
				firePropertyChange(ROLLOVER_SELECTED_ICON_CHANGED_PROPERTY, oldValue, rolloverSelectedIcon)
				If accessibleContext IsNot Nothing Then accessibleContext.firePropertyChange(AccessibleContext.ACCESSIBLE_VISIBLE_DATA_PROPERTY, oldValue, rolloverSelectedIcon)
				rolloverEnabled = True
				If rolloverSelectedIcon IsNot oldValue Then
					' No way to determine whether we are currently in
					' a rollover state, so repaint regardless
					If selected Then repaint()
				End If
			End Set
		End Property


		''' <summary>
		''' Returns the icon used by the button when it's disabled.
		''' If no disabled icon has been set this will forward the call to
		''' the look and feel to construct an appropriate disabled Icon.
		''' <p>
		''' Some look and feels might not render the disabled Icon, in which
		''' case they will ignore this.
		''' </summary>
		''' <returns> the <code>disabledIcon</code> property </returns>
		''' <seealso cref= #getPressedIcon </seealso>
		''' <seealso cref= #setDisabledIcon </seealso>
		''' <seealso cref= javax.swing.LookAndFeel#getDisabledIcon </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Property disabledIcon As Icon
			Get
				If disabledIcon Is Nothing Then
					disabledIcon = UIManager.lookAndFeel.getDisabledIcon(Me, icon)
					If disabledIcon IsNot Nothing Then firePropertyChange(DISABLED_ICON_CHANGED_PROPERTY, Nothing, disabledIcon)
				End If
				Return disabledIcon
			End Get
			Set(ByVal disabledIcon As Icon)
				Dim oldValue As Icon = Me.disabledIcon
				Me.disabledIcon = disabledIcon
				firePropertyChange(DISABLED_ICON_CHANGED_PROPERTY, oldValue, disabledIcon)
				If accessibleContext IsNot Nothing Then accessibleContext.firePropertyChange(AccessibleContext.ACCESSIBLE_VISIBLE_DATA_PROPERTY, oldValue, disabledIcon)
				If disabledIcon IsNot oldValue Then
					If Not enabled Then repaint()
				End If
			End Set
		End Property


		''' <summary>
		''' Returns the icon used by the button when it's disabled and selected.
		''' If no disabled selection icon has been set, this will forward
		''' the call to the LookAndFeel to construct an appropriate disabled
		''' Icon from the selection icon if it has been set and to
		''' <code>getDisabledIcon()</code> otherwise.
		''' <p>
		''' Some look and feels might not render the disabled selected Icon, in
		''' which case they will ignore this.
		''' </summary>
		''' <returns> the <code>disabledSelectedIcon</code> property </returns>
		''' <seealso cref= #getDisabledIcon </seealso>
		''' <seealso cref= #setDisabledSelectedIcon </seealso>
		''' <seealso cref= javax.swing.LookAndFeel#getDisabledSelectedIcon </seealso>
		Public Overridable Property disabledSelectedIcon As Icon
			Get
				If disabledSelectedIcon Is Nothing Then
					 If selectedIcon IsNot Nothing Then
						 disabledSelectedIcon = UIManager.lookAndFeel.getDisabledSelectedIcon(Me, selectedIcon)
					 Else
						 Return disabledIcon
					 End If
				End If
				Return disabledSelectedIcon
			End Get
			Set(ByVal disabledSelectedIcon As Icon)
				Dim oldValue As Icon = Me.disabledSelectedIcon
				Me.disabledSelectedIcon = disabledSelectedIcon
				firePropertyChange(DISABLED_SELECTED_ICON_CHANGED_PROPERTY, oldValue, disabledSelectedIcon)
				If accessibleContext IsNot Nothing Then accessibleContext.firePropertyChange(AccessibleContext.ACCESSIBLE_VISIBLE_DATA_PROPERTY, oldValue, disabledSelectedIcon)
				If disabledSelectedIcon IsNot oldValue Then
					If disabledSelectedIcon Is Nothing OrElse oldValue Is Nothing OrElse disabledSelectedIcon.iconWidth <> oldValue.iconWidth OrElse disabledSelectedIcon.iconHeight <> oldValue.iconHeight Then revalidate()
					If (Not enabled) AndAlso selected Then repaint()
				End If
			End Set
		End Property


		''' <summary>
		''' Returns the vertical alignment of the text and icon.
		''' </summary>
		''' <returns> the <code>verticalAlignment</code> property, one of the
		'''          following values:
		''' <ul>
		''' <li>{@code SwingConstants.CENTER} (the default)
		''' <li>{@code SwingConstants.TOP}
		''' <li>{@code SwingConstants.BOTTOM}
		''' </ul> </returns>
		Public Overridable Property verticalAlignment As Integer
			Get
				Return verticalAlignment
			End Get
			Set(ByVal alignment As Integer)
				If alignment = verticalAlignment Then Return
				Dim oldValue As Integer = verticalAlignment
				verticalAlignment = checkVerticalKey(alignment, "verticalAlignment")
				firePropertyChange(VERTICAL_ALIGNMENT_CHANGED_PROPERTY, oldValue, verticalAlignment)
				repaint()
			End Set
		End Property


		''' <summary>
		''' Returns the horizontal alignment of the icon and text.
		''' {@code AbstractButton}'s default is {@code SwingConstants.CENTER},
		''' but subclasses such as {@code JCheckBox} may use a different default.
		''' </summary>
		''' <returns> the <code>horizontalAlignment</code> property,
		'''             one of the following values:
		''' <ul>
		'''   <li>{@code SwingConstants.RIGHT}
		'''   <li>{@code SwingConstants.LEFT}
		'''   <li>{@code SwingConstants.CENTER}
		'''   <li>{@code SwingConstants.LEADING}
		'''   <li>{@code SwingConstants.TRAILING}
		''' </ul> </returns>
		Public Overridable Property horizontalAlignment As Integer
			Get
				Return horizontalAlignment
			End Get
			Set(ByVal alignment As Integer)
				If alignment = horizontalAlignment Then Return
				Dim oldValue As Integer = horizontalAlignment
				horizontalAlignment = checkHorizontalKey(alignment, "horizontalAlignment")
				firePropertyChange(HORIZONTAL_ALIGNMENT_CHANGED_PROPERTY, oldValue, horizontalAlignment)
				repaint()
			End Set
		End Property



		''' <summary>
		''' Returns the vertical position of the text relative to the icon. </summary>
		''' <returns> the <code>verticalTextPosition</code> property,
		'''          one of the following values:
		''' <ul>
		''' <li>{@code SwingConstants.CENTER} (the default)
		''' <li>{@code SwingConstants.TOP}
		''' <li>{@code SwingConstants.BOTTOM}
		''' </ul> </returns>
		Public Overridable Property verticalTextPosition As Integer
			Get
				Return verticalTextPosition
			End Get
			Set(ByVal textPosition As Integer)
				If textPosition = verticalTextPosition Then Return
				Dim oldValue As Integer = verticalTextPosition
				verticalTextPosition = checkVerticalKey(textPosition, "verticalTextPosition")
				firePropertyChange(VERTICAL_TEXT_POSITION_CHANGED_PROPERTY, oldValue, verticalTextPosition)
				revalidate()
				repaint()
			End Set
		End Property


		''' <summary>
		''' Returns the horizontal position of the text relative to the icon. </summary>
		''' <returns> the <code>horizontalTextPosition</code> property,
		'''          one of the following values:
		''' <ul>
		''' <li>{@code SwingConstants.RIGHT}
		''' <li>{@code SwingConstants.LEFT}
		''' <li>{@code SwingConstants.CENTER}
		''' <li>{@code SwingConstants.LEADING}
		''' <li>{@code SwingConstants.TRAILING} (the default)
		''' </ul> </returns>
		Public Overridable Property horizontalTextPosition As Integer
			Get
				Return horizontalTextPosition
			End Get
			Set(ByVal textPosition As Integer)
				If textPosition = horizontalTextPosition Then Return
				Dim oldValue As Integer = horizontalTextPosition
				horizontalTextPosition = checkHorizontalKey(textPosition, "horizontalTextPosition")
				firePropertyChange(HORIZONTAL_TEXT_POSITION_CHANGED_PROPERTY, oldValue, horizontalTextPosition)
				revalidate()
				repaint()
			End Set
		End Property


		''' <summary>
		''' Returns the amount of space between the text and the icon
		''' displayed in this button.
		''' </summary>
		''' <returns> an int equal to the number of pixels between the text
		'''         and the icon.
		''' @since 1.4 </returns>
		''' <seealso cref= #setIconTextGap </seealso>
		Public Overridable Property iconTextGap As Integer
			Get
				Return iconTextGap
			End Get
			Set(ByVal iconTextGap As Integer)
				Dim oldValue As Integer = Me.iconTextGap
				Me.iconTextGap = iconTextGap
				iconTextGapSet = True
				firePropertyChange("iconTextGap", oldValue, iconTextGap)
				If iconTextGap <> oldValue Then
					revalidate()
					repaint()
				End If
			End Set
		End Property


		''' <summary>
		''' Verify that the {@code key} argument is a legal value for the
		''' {@code horizontalAlignment} and {@code horizontalTextPosition}
		''' properties. Valid values are:
		''' <ul>
		'''   <li>{@code SwingConstants.RIGHT}
		'''   <li>{@code SwingConstants.LEFT}
		'''   <li>{@code SwingConstants.CENTER}
		'''   <li>{@code SwingConstants.LEADING}
		'''   <li>{@code SwingConstants.TRAILING}
		''' </ul>
		''' </summary>
		''' <param name="key"> the property value to check </param>
		''' <param name="exception"> the message to use in the
		'''        {@code IllegalArgumentException} that is thrown for an invalid
		'''        value </param>
		''' <returns> the {@code key} argument </returns>
		''' <exception cref="IllegalArgumentException"> if key is not one of the legal
		'''            values listed above </exception>
		''' <seealso cref= #setHorizontalTextPosition </seealso>
		''' <seealso cref= #setHorizontalAlignment </seealso>
		Protected Friend Overridable Function checkHorizontalKey(ByVal key As Integer, ByVal exception As String) As Integer
			If (key = LEFT) OrElse (key = CENTER) OrElse (key = RIGHT) OrElse (key = LEADING) OrElse (key = TRAILING) Then
				Return key
			Else
				Throw New System.ArgumentException(exception)
			End If
		End Function

		''' <summary>
		''' Verify that the {@code key} argument is a legal value for the
		''' vertical properties. Valid values are:
		''' <ul>
		'''   <li>{@code SwingConstants.CENTER}
		'''   <li>{@code SwingConstants.TOP}
		'''   <li>{@code SwingConstants.BOTTOM}
		''' </ul>
		''' </summary>
		''' <param name="key"> the property value to check </param>
		''' <param name="exception"> the message to use in the
		'''        {@code IllegalArgumentException} that is thrown for an invalid
		'''        value </param>
		''' <returns> the {@code key} argument </returns>
		''' <exception cref="IllegalArgumentException"> if key is not one of the legal
		'''            values listed above </exception>
		Protected Friend Overridable Function checkVerticalKey(ByVal key As Integer, ByVal exception As String) As Integer
			If (key = TOP) OrElse (key = CENTER) OrElse (key = BOTTOM) Then
				Return key
			Else
				Throw New System.ArgumentException(exception)
			End If
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' 
		''' @since 1.6
		''' </summary>
		Public Overrides Sub removeNotify()
			MyBase.removeNotify()
			If rolloverEnabled Then model.rollover = False
		End Sub

		''' <summary>
		''' Sets the action command for this button. </summary>
		''' <param name="actionCommand"> the action command for this button </param>
		Public Overridable Property actionCommand As String
			Set(ByVal actionCommand As String)
				model.actionCommand = actionCommand
			End Set
			Get
				Dim ac As String = model.actionCommand
				If ac Is Nothing Then ac = text
				Return ac
			End Get
		End Property


		Private action As Action
		Private actionPropertyChangeListener As java.beans.PropertyChangeListener

		''' <summary>
		''' Sets the <code>Action</code>.
		''' The new <code>Action</code> replaces any previously set
		''' <code>Action</code> but does not affect <code>ActionListeners</code>
		''' independently added with <code>addActionListener</code>.
		''' If the <code>Action</code> is already a registered
		''' <code>ActionListener</code> for the button, it is not re-registered.
		''' <p>
		''' Setting the <code>Action</code> results in immediately changing
		''' all the properties described in <a href="Action.html#buttonActions">
		''' Swing Components Supporting <code>Action</code></a>.
		''' Subsequently, the button's properties are automatically updated
		''' as the <code>Action</code>'s properties change.
		''' <p>
		''' This method uses three other methods to set
		''' and help track the <code>Action</code>'s property values.
		''' It uses the <code>configurePropertiesFromAction</code> method
		''' to immediately change the button's properties.
		''' To track changes in the <code>Action</code>'s property values,
		''' this method registers the <code>PropertyChangeListener</code>
		''' returned by <code>createActionPropertyChangeListener</code>. The
		''' default {@code PropertyChangeListener} invokes the
		''' {@code actionPropertyChanged} method when a property in the
		''' {@code Action} changes.
		''' </summary>
		''' <param name="a"> the <code>Action</code> for the <code>AbstractButton</code>,
		'''          or <code>null</code>
		''' @since 1.3 </param>
		''' <seealso cref= Action </seealso>
		''' <seealso cref= #getAction </seealso>
		''' <seealso cref= #configurePropertiesFromAction </seealso>
		''' <seealso cref= #createActionPropertyChangeListener </seealso>
		''' <seealso cref= #actionPropertyChanged
		''' @beaninfo
		'''        bound: true
		'''    attribute: visualUpdate true
		'''  description: the Action instance connected with this ActionEvent source </seealso>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Public Overridable Sub setAction(ByVal a As Action) 'JavaToDotNetTempPropertySetaction
		Public Overridable Property action As Action
			Set(ByVal a As Action)
				Dim oldValue As Action = action
				If action Is Nothing OrElse (Not action.Equals(a)) Then
					action = a
					If oldValue IsNot Nothing Then
						removeActionListener(oldValue)
						oldValue.removePropertyChangeListener(actionPropertyChangeListener)
						actionPropertyChangeListener = Nothing
					End If
					configurePropertiesFromAction(action)
					If action IsNot Nothing Then
						' Don't add if it is already a listener
						If Not isListener(GetType(ActionListener), action) Then addActionListener(action)
						' Reverse linkage:
						actionPropertyChangeListener = createActionPropertyChangeListener(action)
						action.addPropertyChangeListener(actionPropertyChangeListener)
					End If
					firePropertyChange("action", oldValue, action)
				End If
			End Set
			Get
		End Property

		Private Function isListener(ByVal c As Type, ByVal a As ActionListener) As Boolean
			Dim ___isListener As Boolean = False
			Dim ___listeners As Object() = listenerList.listenerList
			For i As Integer = ___listeners.Length-2 To 0 Step -2
				If ___listeners(i) Is c AndAlso ___listeners(i+1) Is a Then ___isListener=True
			Next i
			Return ___isListener
		End Function

			Return action
		End Function

		''' <summary>
		''' Sets the properties on this button to match those in the specified
		''' <code>Action</code>.  Refer to <a href="Action.html#buttonActions">
		''' Swing Components Supporting <code>Action</code></a> for more
		''' details as to which properties this sets.
		''' </summary>
		''' <param name="a"> the <code>Action</code> from which to get the properties,
		'''          or <code>null</code>
		''' @since 1.3 </param>
		''' <seealso cref= Action </seealso>
		''' <seealso cref= #setAction </seealso>
		Protected Friend Overridable Sub configurePropertiesFromAction(ByVal a As Action)
			mnemonicFromAction = a
			textFromActionion(a, False)
			AbstractAction.toolTipTextFromActionion(Me, a)
			iconFromAction = a
			actionCommandFromAction = a
			AbstractAction.enabledFromActionion(Me, a)
			If AbstractAction.hasSelectedKey(a) AndAlso shouldUpdateSelectedStateFromAction() Then selectedFromAction = a
			displayedMnemonicIndexFromActionion(a, False)
		End Sub

		Friend Overrides Sub clientPropertyChanged(ByVal key As Object, ByVal oldValue As Object, ByVal newValue As Object)
			If key Is "hideActionText" Then
				Dim current As Boolean = If(TypeOf newValue Is Boolean?, CBool(newValue), False)
				If hideActionText <> current Then hideActionText = current
			End If
		End Sub

		''' <summary>
		''' Button subclasses that support mirroring the selected state from
		''' the action should override this to return true.  AbstractButton's
		''' implementation returns false.
		''' </summary>
		Friend Overridable Function shouldUpdateSelectedStateFromAction() As Boolean
			Return False
		End Function

		''' <summary>
		''' Updates the button's state in response to property changes in the
		''' associated action. This method is invoked from the
		''' {@code PropertyChangeListener} returned from
		''' {@code createActionPropertyChangeListener}. Subclasses do not normally
		''' need to invoke this. Subclasses that support additional {@code Action}
		''' properties should override this and
		''' {@code configurePropertiesFromAction}.
		''' <p>
		''' Refer to the table at <a href="Action.html#buttonActions">
		''' Swing Components Supporting <code>Action</code></a> for a list of
		''' the properties this method sets.
		''' </summary>
		''' <param name="action"> the <code>Action</code> associated with this button </param>
		''' <param name="propertyName"> the name of the property that changed
		''' @since 1.6 </param>
		''' <seealso cref= Action </seealso>
		''' <seealso cref= #configurePropertiesFromAction </seealso>
		Protected Friend Overridable Sub actionPropertyChanged(ByVal action As Action, ByVal propertyName As String)
			If propertyName = Action.NAME Then
				textFromActionion(action, True)
			ElseIf propertyName = "enabled" Then
				AbstractAction.enabledFromActionion(Me, action)
			ElseIf propertyName = Action.SHORT_DESCRIPTION Then
				AbstractAction.toolTipTextFromActionion(Me, action)
			ElseIf propertyName = Action.SMALL_ICON Then
				smallIconChanged(action)
			ElseIf propertyName = Action.MNEMONIC_KEY Then
				mnemonicFromAction = action
			ElseIf propertyName = Action.ACTION_COMMAND_KEY Then
				actionCommandFromAction = action
			ElseIf propertyName = Action.SELECTED_KEY AndAlso AbstractAction.hasSelectedKey(action) AndAlso shouldUpdateSelectedStateFromAction() Then
				selectedFromAction = action
			ElseIf propertyName = Action.DISPLAYED_MNEMONIC_INDEX_KEY Then
				displayedMnemonicIndexFromActionion(action, True)
			ElseIf propertyName = Action.LARGE_ICON_KEY Then
				largeIconChanged(action)
			End If
		End Sub

		Private Sub setDisplayedMnemonicIndexFromAction(ByVal a As Action, ByVal fromPropertyChange As Boolean)
			Dim iValue As Integer? = If(a Is Nothing, Nothing, CInt(Fix(a.getValue(Action.DISPLAYED_MNEMONIC_INDEX_KEY))))
			If fromPropertyChange OrElse iValue IsNot Nothing Then
				Dim value As Integer
				If iValue Is Nothing Then
					value = -1
				Else
					value = iValue
					Dim ___text As String = text
					If ___text Is Nothing OrElse value >= ___text.Length Then value = -1
				End If
				displayedMnemonicIndex = value
			End If
		End Sub

		Private Property mnemonicFromAction As Action
			Set(ByVal a As Action)
				Dim n As Integer? = If(a Is Nothing, Nothing, CInt(Fix(a.getValue(Action.MNEMONIC_KEY))))
				mnemonic = If(n Is Nothing, ControlChars.NullChar, n)
			End Set
		End Property

		Private Sub setTextFromAction(ByVal a As Action, ByVal propertyChange As Boolean)
			Dim hideText As Boolean = hideActionText
			If Not propertyChange Then
				text = If(a IsNot Nothing AndAlso (Not hideText), CStr(a.getValue(Action.NAME)), Nothing)
			ElseIf Not hideText Then
				text = CStr(a.getValue(Action.NAME))
			End If
		End Sub

		Friend Overridable Property iconFromAction As Action
			Set(ByVal a As Action)
				Dim ___icon As Icon = Nothing
				If a IsNot Nothing Then
					___icon = CType(a.getValue(Action.LARGE_ICON_KEY), Icon)
					If ___icon Is Nothing Then ___icon = CType(a.getValue(Action.SMALL_ICON), Icon)
				End If
				icon = ___icon
			End Set
		End Property

		Friend Overridable Sub smallIconChanged(ByVal a As Action)
			If a.getValue(Action.LARGE_ICON_KEY) Is Nothing Then iconFromAction = a
		End Sub

		Friend Overridable Sub largeIconChanged(ByVal a As Action)
			iconFromAction = a
		End Sub

		Private Property actionCommandFromAction As Action
			Set(ByVal a As Action)
				actionCommand = If(a IsNot Nothing, CStr(a.getValue(Action.ACTION_COMMAND_KEY)), Nothing)
			End Set
		End Property

		''' <summary>
		''' Sets the seleted state of the button from the action.  This is defined
		''' here, but not wired up.  Subclasses like JToggleButton and
		''' JCheckBoxMenuItem make use of it.
		''' </summary>
		''' <param name="a"> the Action </param>
		Private Property selectedFromAction As Action
			Set(ByVal a As Action)
				Dim ___selected As Boolean = False
				If a IsNot Nothing Then ___selected = AbstractAction.isSelected(a)
				If ___selected <> selected Then
					' This won't notify ActionListeners, but that should be
					' ok as the change is coming from the Action.
					selected = ___selected
					' Make sure the change actually took effect
					If (Not ___selected) AndAlso selected Then
						If TypeOf model Is DefaultButtonModel Then
							Dim group As ButtonGroup = CType(model, DefaultButtonModel).group
							If group IsNot Nothing Then group.clearSelection()
						End If
					End If
				End If
			End Set
		End Property

		''' <summary>
		''' Creates and returns a <code>PropertyChangeListener</code> that is
		''' responsible for listening for changes from the specified
		''' <code>Action</code> and updating the appropriate properties.
		''' <p>
		''' <b>Warning:</b> If you subclass this do not create an anonymous
		''' inner class.  If you do the lifetime of the button will be tied to
		''' that of the <code>Action</code>.
		''' </summary>
		''' <param name="a"> the button's action
		''' @since 1.3 </param>
		''' <seealso cref= Action </seealso>
		''' <seealso cref= #setAction </seealso>
		Protected Friend Overridable Function createActionPropertyChangeListener(ByVal a As Action) As java.beans.PropertyChangeListener
			Return createActionPropertyChangeListener0(a)
		End Function


		Friend Overridable Function createActionPropertyChangeListener0(ByVal a As Action) As java.beans.PropertyChangeListener
			Return New ButtonActionPropertyChangeListener(Me, a)
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Private Class ButtonActionPropertyChangeListener
			Inherits ActionPropertyChangeListener(Of AbstractButton)

			Friend Sub New(ByVal b As AbstractButton, ByVal a As Action)
				MyBase.New(b, a)
			End Sub
			Protected Friend Overridable Sub actionPropertyChanged(ByVal button As AbstractButton, ByVal action As Action, ByVal e As java.beans.PropertyChangeEvent)
				If AbstractAction.shouldReconfigure(e) Then
					button.configurePropertiesFromAction(action)
				Else
					button.actionPropertyChanged(action, e.propertyName)
				End If
			End Sub
		End Class

		''' <summary>
		''' Gets the <code>borderPainted</code> property.
		''' </summary>
		''' <returns> the value of the <code>borderPainted</code> property </returns>
		''' <seealso cref= #setBorderPainted </seealso>
		Public Overridable Property borderPainted As Boolean
			Get
				Return ___paintBorder
			End Get
			Set(ByVal b As Boolean)
				Dim oldValue As Boolean = ___paintBorder
				___paintBorder = b
				borderPaintedSet = True
				firePropertyChange(BORDER_PAINTED_CHANGED_PROPERTY, oldValue, ___paintBorder)
				If b <> oldValue Then
					revalidate()
					repaint()
				End If
			End Set
		End Property


		''' <summary>
		''' Paint the button's border if <code>BorderPainted</code>
		''' property is true and the button has a border. </summary>
		''' <param name="g"> the <code>Graphics</code> context in which to paint
		''' </param>
		''' <seealso cref= #paint </seealso>
		''' <seealso cref= #setBorder </seealso>
		Protected Friend Overrides Sub paintBorder(ByVal g As Graphics)
			If borderPainted Then MyBase.paintBorder(g)
		End Sub

		''' <summary>
		''' Gets the <code>paintFocus</code> property.
		''' </summary>
		''' <returns> the <code>paintFocus</code> property </returns>
		''' <seealso cref= #setFocusPainted </seealso>
		Public Overridable Property focusPainted As Boolean
			Get
				Return paintFocus
			End Get
			Set(ByVal b As Boolean)
				Dim oldValue As Boolean = paintFocus
				paintFocus = b
				firePropertyChange(FOCUS_PAINTED_CHANGED_PROPERTY, oldValue, paintFocus)
				If b <> oldValue AndAlso focusOwner Then
					revalidate()
					repaint()
				End If
			End Set
		End Property


		''' <summary>
		''' Gets the <code>contentAreaFilled</code> property.
		''' </summary>
		''' <returns> the <code>contentAreaFilled</code> property </returns>
		''' <seealso cref= #setContentAreaFilled </seealso>
		Public Overridable Property contentAreaFilled As Boolean
			Get
				Return contentAreaFilled
			End Get
			Set(ByVal b As Boolean)
				Dim oldValue As Boolean = contentAreaFilled
				contentAreaFilled = b
				contentAreaFilledSet = True
				firePropertyChange(CONTENT_AREA_FILLED_CHANGED_PROPERTY, oldValue, contentAreaFilled)
				If b <> oldValue Then repaint()
			End Set
		End Property


		''' <summary>
		''' Gets the <code>rolloverEnabled</code> property.
		''' </summary>
		''' <returns> the value of the <code>rolloverEnabled</code> property </returns>
		''' <seealso cref= #setRolloverEnabled </seealso>
		Public Overridable Property rolloverEnabled As Boolean
			Get
				Return rolloverEnabled
			End Get
			Set(ByVal b As Boolean)
				Dim oldValue As Boolean = rolloverEnabled
				rolloverEnabled = b
				rolloverEnabledSet = True
				firePropertyChange(ROLLOVER_ENABLED_CHANGED_PROPERTY, oldValue, rolloverEnabled)
				If b <> oldValue Then repaint()
			End Set
		End Property


		''' <summary>
		''' Returns the keyboard mnemonic from the the current model. </summary>
		''' <returns> the keyboard mnemonic from the model </returns>
		Public Overridable Property mnemonic As Integer
			Get
				Return mnemonic
			End Get
			Set(ByVal mnemonic As Integer)
				Dim oldValue As Integer = mnemonic
				model.mnemonic = mnemonic
				updateMnemonicProperties()
			End Set
		End Property


		''' <summary>
		''' This method is now obsolete, please use <code>setMnemonic(int)</code>
		''' to set the mnemonic for a button.  This method is only designed
		''' to handle character values which fall between 'a' and 'z' or
		''' 'A' and 'Z'.
		''' </summary>
		''' <param name="mnemonic">  a char specifying the mnemonic value </param>
		''' <seealso cref= #setMnemonic(int)
		''' @beaninfo
		'''        bound: true
		'''    attribute: visualUpdate true
		'''  description: the keyboard character mnemonic </seealso>
		Public Overridable Property mnemonic As Char
			Set(ByVal mnemonic As Char)
				Dim vk As Integer = AscW(mnemonic)
				If vk >= "a"c AndAlso vk <="z"c Then vk -= (AscW("a"c) - AscW("A"c))
				mnemonic = vk
			End Set
		End Property

		''' <summary>
		''' Provides a hint to the look and feel as to which character in the
		''' text should be decorated to represent the mnemonic. Not all look and
		''' feels may support this. A value of -1 indicates either there is no
		''' mnemonic, the mnemonic character is not contained in the string, or
		''' the developer does not wish the mnemonic to be displayed.
		''' <p>
		''' The value of this is updated as the properties relating to the
		''' mnemonic change (such as the mnemonic itself, the text...).
		''' You should only ever have to call this if
		''' you do not wish the default character to be underlined. For example, if
		''' the text was 'Save As', with a mnemonic of 'a', and you wanted the 'A'
		''' to be decorated, as 'Save <u>A</u>s', you would have to invoke
		''' <code>setDisplayedMnemonicIndex(5)</code> after invoking
		''' <code>setMnemonic(KeyEvent.VK_A)</code>.
		''' 
		''' @since 1.4 </summary>
		''' <param name="index"> Index into the String to underline </param>
		''' <exception cref="IllegalArgumentException"> will be thrown if <code>index</code>
		'''            is &gt;= length of the text, or &lt; -1 </exception>
		''' <seealso cref= #getDisplayedMnemonicIndex
		''' 
		''' @beaninfo
		'''        bound: true
		'''    attribute: visualUpdate true
		'''  description: the index into the String to draw the keyboard character
		'''               mnemonic at </seealso>
		Public Overridable Property displayedMnemonicIndex As Integer
			Set(ByVal index As Integer)
				Dim oldValue As Integer = mnemonicIndex
				If index = -1 Then
					mnemonicIndex = -1
				Else
					Dim ___text As String = text
					Dim textLength As Integer = If(___text Is Nothing, 0, ___text.Length)
					If index < -1 OrElse index >= textLength Then ' index out of range Throw New System.ArgumentException("index == " & index)
				End If
				mnemonicIndex = index
				firePropertyChange("displayedMnemonicIndex", oldValue, index)
				If index <> oldValue Then
					revalidate()
					repaint()
				End If
			End Set
			Get
				Return mnemonicIndex
			End Get
		End Property


		''' <summary>
		''' Update the displayedMnemonicIndex property. This method
		''' is called when either text or mnemonic changes. The new
		''' value of the displayedMnemonicIndex property is the index
		''' of the first occurrence of mnemonic in text.
		''' </summary>
		Private Sub updateDisplayedMnemonicIndex(ByVal text As String, ByVal mnemonic As Integer)
			displayedMnemonicIndex = SwingUtilities.findDisplayedMnemonicIndex(text, mnemonic)
		End Sub

		''' <summary>
		''' Brings the mnemonic property in accordance with model's mnemonic.
		''' This is called when model's mnemonic changes. Also updates the
		''' displayedMnemonicIndex property.
		''' </summary>
		Private Sub updateMnemonicProperties()
			Dim newMnemonic As Integer = model.mnemonic
			If mnemonic <> newMnemonic Then
				Dim oldValue As Integer = mnemonic
				mnemonic = newMnemonic
				firePropertyChange(MNEMONIC_CHANGED_PROPERTY, oldValue, mnemonic)
				updateDisplayedMnemonicIndex(text, mnemonic)
				revalidate()
				repaint()
			End If
		End Sub

		''' <summary>
		''' Sets the amount of time (in milliseconds) required between
		''' mouse press events for the button to generate the corresponding
		''' action events.  After the initial mouse press occurs (and action
		''' event generated) any subsequent mouse press events which occur
		''' on intervals less than the threshhold will be ignored and no
		''' corresponding action event generated.  By default the threshhold is 0,
		''' which means that for each mouse press, an action event will be
		''' fired, no matter how quickly the mouse clicks occur.  In buttons
		''' where this behavior is not desirable (for example, the "OK" button
		''' in a dialog), this threshhold should be set to an appropriate
		''' positive value.
		''' </summary>
		''' <seealso cref= #getMultiClickThreshhold </seealso>
		''' <param name="threshhold"> the amount of time required between mouse
		'''        press events to generate corresponding action events </param>
		''' <exception cref="IllegalArgumentException"> if threshhold &lt; 0
		''' @since 1.4 </exception>
		Public Overridable Property multiClickThreshhold As Long
			Set(ByVal threshhold As Long)
				If threshhold < 0 Then Throw New System.ArgumentException("threshhold must be >= 0")
				Me.multiClickThreshhold = threshhold
			End Set
			Get
				Return multiClickThreshhold
			End Get
		End Property


		''' <summary>
		''' Returns the model that this button represents. </summary>
		''' <returns> the <code>model</code> property </returns>
		''' <seealso cref= #setModel </seealso>
		Public Overridable Property model As ButtonModel
			Get
				Return model
			End Get
			Set(ByVal newModel As ButtonModel)
    
				Dim oldModel As ButtonModel = model
    
				If oldModel IsNot Nothing Then
					oldModel.removeChangeListener(changeListener)
					oldModel.removeActionListener(actionListener)
					oldModel.removeItemListener(itemListener)
					changeListener = Nothing
					actionListener = Nothing
					itemListener = Nothing
				End If
    
				model = newModel
    
				If newModel IsNot Nothing Then
					changeListener = createChangeListener()
					actionListener = createActionListener()
					itemListener = createItemListener()
					newModel.addChangeListener(changeListener)
					newModel.addActionListener(actionListener)
					newModel.addItemListener(itemListener)
    
					updateMnemonicProperties()
					'We invoke setEnabled() from JComponent
					'because setModel() can be called from a constructor
					'when the button is not fully initialized
					MyBase.enabled = newModel.enabled
    
				Else
					mnemonic = ControlChars.NullChar
				End If
    
				updateDisplayedMnemonicIndex(text, mnemonic)
    
				firePropertyChange(MODEL_CHANGED_PROPERTY, oldModel, newModel)
				If newModel IsNot oldModel Then
					revalidate()
					repaint()
				End If
			End Set
		End Property



		''' <summary>
		''' Returns the L&amp;F object that renders this component. </summary>
		''' <returns> the ButtonUI object </returns>
		''' <seealso cref= #setUI </seealso>
		Public Overridable Property uI As ButtonUI
			Get
				Return CType(ui, ButtonUI)
			End Get
			Set(ByVal ui As ButtonUI)
				MyBase.uI = ui
				' disabled icons are generated by the LF so they should be unset here
				If TypeOf disabledIcon Is UIResource Then disabledIcon = Nothing
				If TypeOf disabledSelectedIcon Is UIResource Then disabledSelectedIcon = Nothing
			End Set
		End Property




		''' <summary>
		''' Resets the UI property to a value from the current look
		''' and feel.  Subtypes of <code>AbstractButton</code>
		''' should override this to update the UI. For
		''' example, <code>JButton</code> might do the following:
		''' <pre>
		'''      setUI((ButtonUI)UIManager.getUI(
		'''          "ButtonUI", "javax.swing.plaf.basic.BasicButtonUI", this));
		''' </pre>
		''' </summary>
		Public Overrides Sub updateUI()
		End Sub

		''' <summary>
		''' Adds the specified component to this container at the specified
		''' index, refer to
		''' <seealso cref="java.awt.Container#addImpl(Component, Object, int)"/>
		''' for a complete description of this method.
		''' </summary>
		''' <param name="comp"> the component to be added </param>
		''' <param name="constraints"> an object expressing layout constraints
		'''                 for this component </param>
		''' <param name="index"> the position in the container's list at which to
		'''                 insert the component, where <code>-1</code>
		'''                 means append to the end </param>
		''' <exception cref="IllegalArgumentException"> if <code>index</code> is invalid </exception>
		''' <exception cref="IllegalArgumentException"> if adding the container's parent
		'''                  to itself </exception>
		''' <exception cref="IllegalArgumentException"> if adding a window to a container
		''' @since 1.5 </exception>
		Protected Friend Overridable Sub addImpl(ByVal comp As Component, ByVal constraints As Object, ByVal index As Integer)
			If Not ___setLayout Then layout = New OverlayLayout(Me)
			MyBase.addImpl(comp, constraints, index)
		End Sub

		''' <summary>
		''' Sets the layout manager for this container, refer to
		''' <seealso cref="java.awt.Container#setLayout(LayoutManager)"/>
		''' for a complete description of this method.
		''' </summary>
		''' <param name="mgr"> the specified layout manager
		''' @since 1.5 </param>
		Public Overridable Property layout As LayoutManager
			Set(ByVal mgr As LayoutManager)
				___setLayout = True
				MyBase.layout = mgr
			End Set
		End Property

		''' <summary>
		''' Adds a <code>ChangeListener</code> to the button. </summary>
		''' <param name="l"> the listener to be added </param>
		Public Overridable Sub addChangeListener(ByVal l As ChangeListener)
			listenerList.add(GetType(ChangeListener), l)
		End Sub

		''' <summary>
		''' Removes a ChangeListener from the button. </summary>
		''' <param name="l"> the listener to be removed </param>
		Public Overridable Sub removeChangeListener(ByVal l As ChangeListener)
			listenerList.remove(GetType(ChangeListener), l)
		End Sub

		''' <summary>
		''' Returns an array of all the <code>ChangeListener</code>s added
		''' to this AbstractButton with addChangeListener().
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
		''' Notifies all listeners that have registered interest for
		''' notification on this event type.  The event instance
		''' is lazily created. </summary>
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
		''' Adds an <code>ActionListener</code> to the button. </summary>
		''' <param name="l"> the <code>ActionListener</code> to be added </param>
		Public Overridable Sub addActionListener(ByVal l As ActionListener)
			listenerList.add(GetType(ActionListener), l)
		End Sub

		''' <summary>
		''' Removes an <code>ActionListener</code> from the button.
		''' If the listener is the currently set <code>Action</code>
		''' for the button, then the <code>Action</code>
		''' is set to <code>null</code>.
		''' </summary>
		''' <param name="l"> the listener to be removed </param>
		Public Overridable Sub removeActionListener(ByVal l As ActionListener)
			If (l IsNot Nothing) AndAlso (action Is l) Then
				action = Nothing
			Else
				listenerList.remove(GetType(ActionListener), l)
			End If
		End Sub

		''' <summary>
		''' Returns an array of all the <code>ActionListener</code>s added
		''' to this AbstractButton with addActionListener().
		''' </summary>
		''' <returns> all of the <code>ActionListener</code>s added or an empty
		'''         array if no listeners have been added
		''' @since 1.4 </returns>
		Public Overridable Property actionListeners As ActionListener()
			Get
				Return listenerList.getListeners(GetType(ActionListener))
			End Get
		End Property

		''' <summary>
		''' Subclasses that want to handle <code>ChangeEvents</code> differently
		''' can override this to return another <code>ChangeListener</code>
		''' implementation.
		''' </summary>
		''' <returns> the new <code>ChangeListener</code> </returns>
		Protected Friend Overridable Function createChangeListener() As ChangeListener
			Return handler
		End Function

		''' <summary>
		''' Extends <code>ChangeListener</code> to be serializable.
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
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		<Serializable> _
		Protected Friend Class ButtonChangeListener
			Implements ChangeListener

			Private ReadOnly outerInstance As AbstractButton

			' NOTE: This class is NOT used, instead the functionality has
			' been moved to Handler.
			Friend Sub New(ByVal outerInstance As AbstractButton)
					Me.outerInstance = outerInstance
			End Sub

			Public Overridable Sub stateChanged(ByVal e As ChangeEvent) Implements ChangeListener.stateChanged
				outerInstance.handler.stateChanged(e)
			End Sub
		End Class


		''' <summary>
		''' Notifies all listeners that have registered interest for
		''' notification on this event type.  The event instance
		''' is lazily created using the <code>event</code>
		''' parameter.
		''' </summary>
		''' <param name="event">  the <code>ActionEvent</code> object </param>
		''' <seealso cref= EventListenerList </seealso>
		Protected Friend Overridable Sub fireActionPerformed(ByVal [event] As ActionEvent)
			' Guaranteed to return a non-null array
			Dim ___listeners As Object() = listenerList.listenerList
			Dim e As ActionEvent = Nothing
			' Process the listeners last to first, notifying
			' those that are interested in this event
			For i As Integer = ___listeners.Length-2 To 0 Step -2
				If ___listeners(i) Is GetType(ActionListener) Then
					' Lazily create the event:
					If e Is Nothing Then
						  Dim ___actionCommand As String = [event].actionCommand
						  If ___actionCommand Is Nothing Then ___actionCommand = actionCommand
						  e = New ActionEvent(AbstractButton.this, ActionEvent.ACTION_PERFORMED, ___actionCommand, [event].when, [event].modifiers)
					End If
					CType(___listeners(i+1), ActionListener).actionPerformed(e)
				End If
			Next i
		End Sub

		''' <summary>
		''' Notifies all listeners that have registered interest for
		''' notification on this event type.  The event instance
		''' is lazily created using the <code>event</code> parameter.
		''' </summary>
		''' <param name="event">  the <code>ItemEvent</code> object </param>
		''' <seealso cref= EventListenerList </seealso>
		Protected Friend Overridable Sub fireItemStateChanged(ByVal [event] As ItemEvent)
			' Guaranteed to return a non-null array
			Dim ___listeners As Object() = listenerList.listenerList
			Dim e As ItemEvent = Nothing
			' Process the listeners last to first, notifying
			' those that are interested in this event
			For i As Integer = ___listeners.Length-2 To 0 Step -2
				If ___listeners(i) Is GetType(ItemListener) Then
					' Lazily create the event:
					If e Is Nothing Then e = New ItemEvent(AbstractButton.this, ItemEvent.ITEM_STATE_CHANGED, AbstractButton.this, [event].stateChange)
					CType(___listeners(i+1), ItemListener).itemStateChanged(e)
				End If
			Next i
			If accessibleContext IsNot Nothing Then
				If [event].stateChange = ItemEvent.SELECTED Then
					accessibleContext.firePropertyChange(AccessibleContext.ACCESSIBLE_STATE_PROPERTY, Nothing, AccessibleState.SELECTED)
					accessibleContext.firePropertyChange(AccessibleContext.ACCESSIBLE_VALUE_PROPERTY, Convert.ToInt32(0), Convert.ToInt32(1))
				Else
					accessibleContext.firePropertyChange(AccessibleContext.ACCESSIBLE_STATE_PROPERTY, AccessibleState.SELECTED, Nothing)
					accessibleContext.firePropertyChange(AccessibleContext.ACCESSIBLE_VALUE_PROPERTY, Convert.ToInt32(1), Convert.ToInt32(0))
				End If
			End If
		End Sub


		Protected Friend Overridable Function createActionListener() As ActionListener
			Return handler
		End Function


		Protected Friend Overridable Function createItemListener() As ItemListener
			Return handler
		End Function


		''' <summary>
		''' Enables (or disables) the button. </summary>
		''' <param name="b">  true to enable the button, otherwise false </param>
		Public Overrides Property enabled As Boolean
			Set(ByVal b As Boolean)
				If (Not b) AndAlso model.rollover Then model.rollover = False
				MyBase.enabled = b
				model.enabled = b
			End Set
		End Property

		' *** Deprecated java.awt.Button APIs below *** //

		''' <summary>
		''' Returns the label text.
		''' </summary>
		''' <returns> a <code>String</code> containing the label </returns>
		''' @deprecated - Replaced by <code>getText</code> 
		<Obsolete("- Replaced by <code>getText</code>")> _
		Public Overridable Property label As String
			Get
				Return text
			End Get
			Set(ByVal label As String)
				text = label
			End Set
		End Property


		''' <summary>
		''' Adds an <code>ItemListener</code> to the <code>checkbox</code>. </summary>
		''' <param name="l">  the <code>ItemListener</code> to be added </param>
		Public Overridable Sub addItemListener(ByVal l As ItemListener)
			listenerList.add(GetType(ItemListener), l)
		End Sub

		''' <summary>
		''' Removes an <code>ItemListener</code> from the button. </summary>
		''' <param name="l"> the <code>ItemListener</code> to be removed </param>
		Public Overridable Sub removeItemListener(ByVal l As ItemListener)
			listenerList.remove(GetType(ItemListener), l)
		End Sub

		''' <summary>
		''' Returns an array of all the <code>ItemListener</code>s added
		''' to this AbstractButton with addItemListener().
		''' </summary>
		''' <returns> all of the <code>ItemListener</code>s added or an empty
		'''         array if no listeners have been added
		''' @since 1.4 </returns>
		Public Overridable Property itemListeners As ItemListener()
			Get
				Return listenerList.getListeners(GetType(ItemListener))
			End Get
		End Property

	   ''' <summary>
	   ''' Returns an array (length 1) containing the label or
	   ''' <code>null</code> if the button is not selected.
	   ''' </summary>
	   ''' <returns> an array containing 1 Object: the text of the button,
	   '''         if the item is selected; otherwise <code>null</code> </returns>
		Public Overridable Property selectedObjects As Object()
			Get
				If selected = False Then Return Nothing
				Dim ___selectedObjects As Object() = New Object(0){}
				___selectedObjects(0) = text
				Return ___selectedObjects
			End Get
		End Property

		Protected Friend Overridable Sub init(ByVal text As String, ByVal icon As Icon)
			If text IsNot Nothing Then text = text

			If icon IsNot Nothing Then icon = icon

			' Set the UI
			updateUI()

			alignmentX = LEFT_ALIGNMENT
			alignmentY = CENTER_ALIGNMENT
		End Sub


		''' <summary>
		''' This is overridden to return false if the current <code>Icon</code>'s
		''' <code>Image</code> is not equal to the
		''' passed in <code>Image</code> <code>img</code>.
		''' </summary>
		''' <param name="img">  the <code>Image</code> to be compared </param>
		''' <param name="infoflags"> flags used to repaint the button when the image
		'''          is updated and which determine how much is to be painted </param>
		''' <param name="x">  the x coordinate </param>
		''' <param name="y">  the y coordinate </param>
		''' <param name="w">  the width </param>
		''' <param name="h">  the height </param>
		''' <seealso cref=     java.awt.image.ImageObserver </seealso>
		''' <seealso cref=     java.awt.Component#imageUpdate(java.awt.Image, int, int, int, int, int) </seealso>
		Public Overridable Function imageUpdate(ByVal img As Image, ByVal infoflags As Integer, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer) As Boolean
			Dim iconDisplayed As Icon = Nothing

			If Not model.enabled Then
				If model.selected Then
					iconDisplayed = disabledSelectedIcon
				Else
					iconDisplayed = disabledIcon
				End If
			ElseIf model.pressed AndAlso model.armed Then
				iconDisplayed = pressedIcon
			ElseIf rolloverEnabled AndAlso model.rollover Then
				If model.selected Then
					iconDisplayed = rolloverSelectedIcon
				Else
					iconDisplayed = rolloverIcon
				End If
			ElseIf model.selected Then
				iconDisplayed = selectedIcon
			End If

			If iconDisplayed Is Nothing Then iconDisplayed = icon

			If iconDisplayed Is Nothing OrElse (Not SwingUtilities.doesIconReferenceImage(iconDisplayed, img)) Then Return False
			Return MyBase.imageUpdate(img, infoflags, x, y, w, h)
		End Function

		Friend Overrides Sub setUIProperty(ByVal propertyName As String, ByVal value As Object)
			If propertyName = "borderPainted" Then
				If Not borderPaintedSet Then
					borderPainted = CBool(value)
					borderPaintedSet = False
				End If
			ElseIf propertyName = "rolloverEnabled" Then
				If Not rolloverEnabledSet Then
					rolloverEnabled = CBool(value)
					rolloverEnabledSet = False
				End If
			ElseIf propertyName = "iconTextGap" Then
				If Not iconTextGapSet Then
					iconTextGap = CType(value, Number)
					iconTextGapSet = False
				End If
			ElseIf propertyName = "contentAreaFilled" Then
				If Not contentAreaFilledSet Then
					contentAreaFilled = CBool(value)
					contentAreaFilledSet = False
				End If
			Else
				MyBase.uIPropertyrty(propertyName, value)
			End If
		End Sub

		''' <summary>
		''' Returns a string representation of this <code>AbstractButton</code>.
		''' This method
		''' is intended to be used only for debugging purposes, and the
		''' content and format of the returned string may vary between
		''' implementations. The returned string may be empty but may not
		''' be <code>null</code>.
		''' <P>
		''' Overriding <code>paramString</code> to provide information about the
		''' specific new aspects of the JFC components.
		''' </summary>
		''' <returns>  a string representation of this <code>AbstractButton</code> </returns>
		Protected Friend Overrides Function paramString() As String
			Dim defaultIconString As String = (If((defaultIcon IsNot Nothing) AndAlso (defaultIcon IsNot Me), defaultIcon.ToString(), ""))
			Dim pressedIconString As String = (If((pressedIcon IsNot Nothing) AndAlso (pressedIcon IsNot Me), pressedIcon.ToString(), ""))
			Dim disabledIconString As String = (If((disabledIcon IsNot Nothing) AndAlso (disabledIcon IsNot Me), disabledIcon.ToString(), ""))
			Dim selectedIconString As String = (If((selectedIcon IsNot Nothing) AndAlso (selectedIcon IsNot Me), selectedIcon.ToString(), ""))
			Dim disabledSelectedIconString As String = (If((disabledSelectedIcon IsNot Nothing) AndAlso (disabledSelectedIcon IsNot Me), disabledSelectedIcon.ToString(), ""))
			Dim rolloverIconString As String = (If((rolloverIcon IsNot Nothing) AndAlso (rolloverIcon IsNot Me), rolloverIcon.ToString(), ""))
			Dim rolloverSelectedIconString As String = (If((rolloverSelectedIcon IsNot Nothing) AndAlso (rolloverSelectedIcon IsNot Me), rolloverSelectedIcon.ToString(), ""))
			Dim paintBorderString As String = (If(___paintBorder, "true", "false"))
			Dim paintFocusString As String = (If(paintFocus, "true", "false"))
			Dim rolloverEnabledString As String = (If(rolloverEnabled, "true", "false"))

			Return MyBase.paramString() & ",defaultIcon=" & defaultIconString & ",disabledIcon=" & disabledIconString & ",disabledSelectedIcon=" & disabledSelectedIconString & ",margin=" & margin & ",paintBorder=" & paintBorderString & ",paintFocus=" & paintFocusString & ",pressedIcon=" & pressedIconString & ",rolloverEnabled=" & rolloverEnabledString & ",rolloverIcon=" & rolloverIconString & ",rolloverSelectedIcon=" & rolloverSelectedIconString & ",selectedIcon=" & selectedIconString & ",text=" & text
		End Function


		Private Property handler As Handler
			Get
				If handler Is Nothing Then handler = New Handler(Me)
				Return handler
			End Get
		End Property


		'
		' Listeners that are added to model
		'
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		<Serializable> _
		Friend Class Handler
			Implements ActionListener, ChangeListener, ItemListener

			Private ReadOnly outerInstance As AbstractButton

			Public Sub New(ByVal outerInstance As AbstractButton)
				Me.outerInstance = outerInstance
			End Sub

			'
			' ChangeListener
			'
			Public Overridable Sub stateChanged(ByVal e As ChangeEvent) Implements ChangeListener.stateChanged
				Dim source As Object = e.source

				outerInstance.updateMnemonicProperties()
				If enabled <> outerInstance.model.enabled Then outerInstance.enabled = outerInstance.model.enabled
				outerInstance.fireStateChanged()
				repaint()
			End Sub

			'
			' ActionListener
			'
			Public Overridable Sub actionPerformed(ByVal [event] As ActionEvent)
				outerInstance.fireActionPerformed([event])
			End Sub

			'
			' ItemListener
			'
			Public Overridable Sub itemStateChanged(ByVal [event] As ItemEvent)
				outerInstance.fireItemStateChanged([event])
				If outerInstance.shouldUpdateSelectedStateFromAction() Then
					Dim action As Action = outerInstance.action
					If action IsNot Nothing AndAlso AbstractAction.hasSelectedKey(action) Then
						Dim selected As Boolean = outerInstance.selected
						Dim isActionSelected As Boolean = AbstractAction.isSelected(action)
						If isActionSelected <> selected Then action.putValue(Action.SELECTED_KEY, selected)
					End If
				End If
			End Sub
		End Class

	'/////////////////
	' Accessibility support
	'/////////////////
		''' <summary>
		''' This class implements accessibility support for the
		''' <code>AbstractButton</code> class.  It provides an implementation of the
		''' Java Accessibility API appropriate to button and menu item
		''' user-interface elements.
		''' <p>
		''' <strong>Warning:</strong>
		''' Serialized objects of this class will not be compatible with
		''' future Swing releases. The current serialization support is
		''' appropriate for short term storage or RMI between applications running
		''' the same version of Swing.  As of 1.4, support for long term storage
		''' of all JavaBeans&trade;
		''' has been added to the <code>java.beans</code> package.
		''' Please see <seealso cref="java.beans.XMLEncoder"/>.
		''' @since 1.4
		''' </summary>
		Protected Friend MustInherit Class AccessibleAbstractButton
			Inherits AccessibleJComponent
			Implements AccessibleAction, AccessibleValue, AccessibleText, AccessibleExtendedComponent

			Private ReadOnly outerInstance As AbstractButton

			Public Sub New(ByVal outerInstance As AbstractButton)
				Me.outerInstance = outerInstance
			End Sub


			''' <summary>
			''' Returns the accessible name of this object.
			''' </summary>
			''' <returns> the localized name of the object -- can be
			'''              <code>null</code> if this
			'''              object does not have a name </returns>
			Public Overridable Property accessibleName As String
				Get
					Dim name As String = accessibleName
    
					If name Is Nothing Then name = CStr(outerInstance.getClientProperty(AccessibleContext.ACCESSIBLE_NAME_PROPERTY))
					If name Is Nothing Then name = outerInstance.text
					If name Is Nothing Then name = MyBase.accessibleName
					Return name
				End Get
			End Property

			''' <summary>
			''' Get the AccessibleIcons associated with this object if one
			''' or more exist.  Otherwise return null.
			''' @since 1.3
			''' </summary>
			Public Overridable Property accessibleIcon As AccessibleIcon()
				Get
					Dim defaultIcon As Icon = outerInstance.icon
    
					If TypeOf defaultIcon Is Accessible Then
						Dim ac As AccessibleContext = CType(defaultIcon, Accessible).accessibleContext
						If ac IsNot Nothing AndAlso TypeOf ac Is AccessibleIcon Then Return New AccessibleIcon() { CType(ac, AccessibleIcon) }
					End If
					Return Nothing
				End Get
			End Property

			''' <summary>
			''' Get the state set of this object.
			''' </summary>
			''' <returns> an instance of AccessibleState containing the current state
			''' of the object </returns>
			''' <seealso cref= AccessibleState </seealso>
			Public Overridable Property accessibleStateSet As AccessibleStateSet
				Get
				Dim states As AccessibleStateSet = MyBase.accessibleStateSet
					If outerInstance.model.armed Then states.add(AccessibleState.ARMED)
					If focusOwner Then states.add(AccessibleState.FOCUSED)
					If outerInstance.model.pressed Then states.add(AccessibleState.PRESSED)
					If outerInstance.selected Then states.add(AccessibleState.CHECKED)
					Return states
				End Get
			End Property

			''' <summary>
			''' Get the AccessibleRelationSet associated with this object if one
			''' exists.  Otherwise return null. </summary>
			''' <seealso cref= AccessibleRelation
			''' @since 1.3 </seealso>
			Public Overridable Property accessibleRelationSet As AccessibleRelationSet
				Get
    
					' Check where the AccessibleContext's relation
					' set already contains a MEMBER_OF relation.
					Dim relationSet As AccessibleRelationSet = MyBase.accessibleRelationSet
    
					If Not relationSet.contains(AccessibleRelation.MEMBER_OF) Then
						' get the members of the button group if one exists
						Dim model As ButtonModel = outerInstance.model
						If model IsNot Nothing AndAlso TypeOf model Is DefaultButtonModel Then
							Dim group As ButtonGroup = CType(model, DefaultButtonModel).group
							If group IsNot Nothing Then
								' set the target of the MEMBER_OF relation to be
								' the members of the button group.
								Dim len As Integer = group.buttonCount
								Dim target As Object() = New Object(len - 1){}
								Dim elem As System.Collections.IEnumerator(Of AbstractButton) = group.elements
								For i As Integer = 0 To len - 1
	'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
									If elem.hasMoreElements() Then target(i) = elem.nextElement()
								Next i
								Dim relation As New AccessibleRelation(AccessibleRelation.MEMBER_OF)
								relation.target = target
								relationSet.add(relation)
							End If
						End If
					End If
					Return relationSet
				End Get
			End Property

			''' <summary>
			''' Get the AccessibleAction associated with this object.  In the
			''' implementation of the Java Accessibility API for this class,
			''' return this object, which is responsible for implementing the
			''' AccessibleAction interface on behalf of itself.
			''' </summary>
			''' <returns> this object </returns>
			Public Overridable Property accessibleAction As AccessibleAction
				Get
					Return Me
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
			''' Returns the number of Actions available in this object.  The
			''' default behavior of a button is to have one action - toggle
			''' the button.
			''' </summary>
			''' <returns> 1, the number of Actions in this object </returns>
			Public Overridable Property accessibleActionCount As Integer Implements AccessibleAction.getAccessibleActionCount
				Get
					Return 1
				End Get
			End Property

			''' <summary>
			''' Return a description of the specified action of the object.
			''' </summary>
			''' <param name="i"> zero-based index of the actions </param>
			Public Overridable Function getAccessibleActionDescription(ByVal i As Integer) As String Implements AccessibleAction.getAccessibleActionDescription
				If i = 0 Then
					Return UIManager.getString("AbstractButton.clickText")
				Else
					Return Nothing
				End If
			End Function

			''' <summary>
			''' Perform the specified Action on the object
			''' </summary>
			''' <param name="i"> zero-based index of actions </param>
			''' <returns> true if the the action was performed; else false. </returns>
			Public Overridable Function doAccessibleAction(ByVal i As Integer) As Boolean Implements AccessibleAction.doAccessibleAction
				If i = 0 Then
					outerInstance.doClick()
					Return True
				Else
					Return False
				End If
			End Function

			''' <summary>
			''' Get the value of this object as a Number.
			''' </summary>
			''' <returns> An Integer of 0 if this isn't selected or an Integer of 1 if
			''' this is selected. </returns>
			''' <seealso cref= AbstractButton#isSelected </seealso>
			Public Overridable Property currentAccessibleValue As Number Implements AccessibleValue.getCurrentAccessibleValue
				Get
					If outerInstance.selected Then
						Return Convert.ToInt32(1)
					Else
						Return Convert.ToInt32(0)
					End If
				End Get
			End Property

			''' <summary>
			''' Set the value of this object as a Number.
			''' </summary>
			''' <returns> True if the value was set. </returns>
			Public Overridable Function setCurrentAccessibleValue(ByVal n As Number) As Boolean Implements AccessibleValue.setCurrentAccessibleValue
				' TIGER - 4422535
				If n Is Nothing Then Return False
				Dim i As Integer = n
				If i = 0 Then
					outerInstance.selected = False
				Else
					outerInstance.selected = True
				End If
				Return True
			End Function

			''' <summary>
			''' Get the minimum value of this object as a Number.
			''' </summary>
			''' <returns> an Integer of 0. </returns>
			Public Overridable Property minimumAccessibleValue As Number Implements AccessibleValue.getMinimumAccessibleValue
				Get
					Return Convert.ToInt32(0)
				End Get
			End Property

			''' <summary>
			''' Get the maximum value of this object as a Number.
			''' </summary>
			''' <returns> An Integer of 1. </returns>
			Public Overridable Property maximumAccessibleValue As Number Implements AccessibleValue.getMaximumAccessibleValue
				Get
					Return Convert.ToInt32(1)
				End Get
			End Property


			' AccessibleText ---------- 

			Public Overridable Property accessibleText As AccessibleText
				Get
					Dim ___view As View = CType(AbstractButton.this.getClientProperty("html"), View)
					If ___view IsNot Nothing Then
						Return Me
					Else
						Return Nothing
					End If
				End Get
			End Property

			''' <summary>
			''' Given a point in local coordinates, return the zero-based index
			''' of the character under that Point.  If the point is invalid,
			''' this method returns -1.
			''' 
			''' Note: the AbstractButton must have a valid size (e.g. have
			''' been added to a parent container whose ancestor container
			''' is a valid top-level window) for this method to be able
			''' to return a meaningful value.
			''' </summary>
			''' <param name="p"> the Point in local coordinates </param>
			''' <returns> the zero-based index of the character under Point p; if
			''' Point is invalid returns -1.
			''' @since 1.3 </returns>
			Public Overridable Function getIndexAtPoint(ByVal p As Point) As Integer Implements AccessibleText.getIndexAtPoint
				Dim ___view As View = CType(AbstractButton.this.getClientProperty("html"), View)
				If ___view IsNot Nothing Then
					Dim r As Rectangle = textRectangle
					If r Is Nothing Then Return -1
					Dim shape As New Rectangle2D.Float(r.x, r.y, r.width, r.height)
					Dim bias As Position.Bias() = New Position.Bias(0){}
					Return ___view.viewToModel(p.x, p.y, shape, bias)
				Else
					Return -1
				End If
			End Function

			''' <summary>
			''' Determine the bounding box of the character at the given
			''' index into the string.  The bounds are returned in local
			''' coordinates.  If the index is invalid an empty rectangle is
			''' returned.
			''' 
			''' Note: the AbstractButton must have a valid size (e.g. have
			''' been added to a parent container whose ancestor container
			''' is a valid top-level window) for this method to be able
			''' to return a meaningful value.
			''' </summary>
			''' <param name="i"> the index into the String </param>
			''' <returns> the screen coordinates of the character's the bounding box,
			''' if index is invalid returns an empty rectangle.
			''' @since 1.3 </returns>
			Public Overridable Function getCharacterBounds(ByVal i As Integer) As Rectangle Implements AccessibleText.getCharacterBounds
				Dim ___view As View = CType(AbstractButton.this.getClientProperty("html"), View)
				If ___view IsNot Nothing Then
					Dim r As Rectangle = textRectangle
					If r Is Nothing Then Return Nothing
					Dim shape As New Rectangle2D.Float(r.x, r.y, r.width, r.height)
					Try
						Dim charShape As Shape = ___view.modelToView(i, shape, Position.Bias.Forward)
						Return charShape.bounds
					Catch e As BadLocationException
						Return Nothing
					End Try
				Else
					Return Nothing
				End If
			End Function

			''' <summary>
			''' Return the number of characters (valid indicies)
			''' </summary>
			''' <returns> the number of characters
			''' @since 1.3 </returns>
			Public Overridable Property charCount As Integer Implements AccessibleText.getCharCount
				Get
					Dim ___view As View = CType(AbstractButton.this.getClientProperty("html"), View)
					If ___view IsNot Nothing Then
						Dim d As Document = ___view.document
						If TypeOf d Is StyledDocument Then
							Dim doc As StyledDocument = CType(d, StyledDocument)
							Return doc.length
						End If
					End If
					Return accessibleContext.accessibleName.length()
				End Get
			End Property

			''' <summary>
			''' Return the zero-based offset of the caret.
			''' 
			''' Note: That to the right of the caret will have the same index
			''' value as the offset (the caret is between two characters). </summary>
			''' <returns> the zero-based offset of the caret.
			''' @since 1.3 </returns>
			Public Overridable Property caretPosition As Integer Implements AccessibleText.getCaretPosition
				Get
					' There is no caret.
					Return -1
				End Get
			End Property

			''' <summary>
			''' Returns the String at a given index.
			''' </summary>
			''' <param name="part"> the AccessibleText.CHARACTER, AccessibleText.WORD,
			''' or AccessibleText.SENTENCE to retrieve </param>
			''' <param name="index"> an index within the text &gt;= 0 </param>
			''' <returns> the letter, word, or sentence,
			'''   null for an invalid index or part
			''' @since 1.3 </returns>
			Public Overridable Function getAtIndex(ByVal part As Integer, ByVal index As Integer) As String Implements AccessibleText.getAtIndex
				If index < 0 OrElse index >= charCount Then Return Nothing
				Select Case part
				Case AccessibleText.CHARACTER
					Try
						Return getText(index, 1)
					Catch e As BadLocationException
						Return Nothing
					End Try
				Case AccessibleText.WORD
					Try
						Dim s As String = getText(0, charCount)
						Dim words As BreakIterator = BreakIterator.getWordInstance(locale)
						words.text = s
						Dim [end] As Integer = words.following(index)
						Return s.Substring(words.previous(), [end] - (words.previous()))
					Catch e As BadLocationException
						Return Nothing
					End Try
				Case AccessibleText.SENTENCE
					Try
						Dim s As String = getText(0, charCount)
						Dim sentence As BreakIterator = BreakIterator.getSentenceInstance(locale)
						sentence.text = s
						Dim [end] As Integer = sentence.following(index)
						Return s.Substring(sentence.previous(), [end] - (sentence.previous()))
					Catch e As BadLocationException
						Return Nothing
					End Try
				Case Else
					Return Nothing
				End Select
			End Function

			''' <summary>
			''' Returns the String after a given index.
			''' </summary>
			''' <param name="part"> the AccessibleText.CHARACTER, AccessibleText.WORD,
			''' or AccessibleText.SENTENCE to retrieve </param>
			''' <param name="index"> an index within the text &gt;= 0 </param>
			''' <returns> the letter, word, or sentence, null for an invalid
			'''  index or part
			''' @since 1.3 </returns>
			Public Overridable Function getAfterIndex(ByVal part As Integer, ByVal index As Integer) As String Implements AccessibleText.getAfterIndex
				If index < 0 OrElse index >= charCount Then Return Nothing
				Select Case part
				Case AccessibleText.CHARACTER
					If index+1 >= charCount Then Return Nothing
					Try
						Return getText(index+1, 1)
					Catch e As BadLocationException
						Return Nothing
					End Try
				Case AccessibleText.WORD
					Try
						Dim s As String = getText(0, charCount)
						Dim words As BreakIterator = BreakIterator.getWordInstance(locale)
						words.text = s
						Dim start As Integer = words.following(index)
						If start = BreakIterator.DONE OrElse start >= s.Length Then Return Nothing
						Dim [end] As Integer = words.following(start)
						If [end] = BreakIterator.DONE OrElse [end] >= s.Length Then Return Nothing
						Return s.Substring(start, [end] - start)
					Catch e As BadLocationException
						Return Nothing
					End Try
				Case AccessibleText.SENTENCE
					Try
						Dim s As String = getText(0, charCount)
						Dim sentence As BreakIterator = BreakIterator.getSentenceInstance(locale)
						sentence.text = s
						Dim start As Integer = sentence.following(index)
						If start = BreakIterator.DONE OrElse start > s.Length Then Return Nothing
						Dim [end] As Integer = sentence.following(start)
						If [end] = BreakIterator.DONE OrElse [end] > s.Length Then Return Nothing
						Return s.Substring(start, [end] - start)
					Catch e As BadLocationException
						Return Nothing
					End Try
				Case Else
					Return Nothing
				End Select
			End Function

			''' <summary>
			''' Returns the String before a given index.
			''' </summary>
			''' <param name="part"> the AccessibleText.CHARACTER, AccessibleText.WORD,
			'''   or AccessibleText.SENTENCE to retrieve </param>
			''' <param name="index"> an index within the text &gt;= 0 </param>
			''' <returns> the letter, word, or sentence, null for an invalid index
			'''  or part
			''' @since 1.3 </returns>
			Public Overridable Function getBeforeIndex(ByVal part As Integer, ByVal index As Integer) As String Implements AccessibleText.getBeforeIndex
				If index < 0 OrElse index > charCount-1 Then Return Nothing
				Select Case part
				Case AccessibleText.CHARACTER
					If index = 0 Then Return Nothing
					Try
						Return getText(index-1, 1)
					Catch e As BadLocationException
						Return Nothing
					End Try
				Case AccessibleText.WORD
					Try
						Dim s As String = getText(0, charCount)
						Dim words As BreakIterator = BreakIterator.getWordInstance(locale)
						words.text = s
						Dim [end] As Integer = words.following(index)
						[end] = words.previous()
						Dim start As Integer = words.previous()
						If start = BreakIterator.DONE Then Return Nothing
						Return s.Substring(start, [end] - start)
					Catch e As BadLocationException
						Return Nothing
					End Try
				Case AccessibleText.SENTENCE
					Try
						Dim s As String = getText(0, charCount)
						Dim sentence As BreakIterator = BreakIterator.getSentenceInstance(locale)
						sentence.text = s
						Dim [end] As Integer = sentence.following(index)
						[end] = sentence.previous()
						Dim start As Integer = sentence.previous()
						If start = BreakIterator.DONE Then Return Nothing
						Return s.Substring(start, [end] - start)
					Catch e As BadLocationException
						Return Nothing
					End Try
				Case Else
					Return Nothing
				End Select
			End Function

			''' <summary>
			''' Return the AttributeSet for a given character at a given index
			''' </summary>
			''' <param name="i"> the zero-based index into the text </param>
			''' <returns> the AttributeSet of the character
			''' @since 1.3 </returns>
			Public Overridable Function getCharacterAttribute(ByVal i As Integer) As AttributeSet Implements AccessibleText.getCharacterAttribute
				Dim ___view As View = CType(AbstractButton.this.getClientProperty("html"), View)
				If ___view IsNot Nothing Then
					Dim d As Document = ___view.document
					If TypeOf d Is StyledDocument Then
						Dim doc As StyledDocument = CType(d, StyledDocument)
						Dim elem As Element = doc.getCharacterElement(i)
						If elem IsNot Nothing Then Return elem.attributes
					End If
				End If
				Return Nothing
			End Function

			''' <summary>
			''' Returns the start offset within the selected text.
			''' If there is no selection, but there is
			''' a caret, the start and end offsets will be the same.
			''' </summary>
			''' <returns> the index into the text of the start of the selection
			''' @since 1.3 </returns>
			Public Overridable Property selectionStart As Integer Implements AccessibleText.getSelectionStart
				Get
					' Text cannot be selected.
					Return -1
				End Get
			End Property

			''' <summary>
			''' Returns the end offset within the selected text.
			''' If there is no selection, but there is
			''' a caret, the start and end offsets will be the same.
			''' </summary>
			''' <returns> the index into the text of the end of the selection
			''' @since 1.3 </returns>
			Public Overridable Property selectionEnd As Integer Implements AccessibleText.getSelectionEnd
				Get
					' Text cannot be selected.
					Return -1
				End Get
			End Property

			''' <summary>
			''' Returns the portion of the text that is selected.
			''' </summary>
			''' <returns> the String portion of the text that is selected
			''' @since 1.3 </returns>
			Public Overridable Property selectedText As String Implements AccessibleText.getSelectedText
				Get
					' Text cannot be selected.
					Return Nothing
				End Get
			End Property

	'        
	'         * Returns the text substring starting at the specified
	'         * offset with the specified length.
	'         
			Private Function getText(ByVal offset As Integer, ByVal length As Integer) As String

				Dim ___view As View = CType(AbstractButton.this.getClientProperty("html"), View)
				If ___view IsNot Nothing Then
					Dim d As Document = ___view.document
					If TypeOf d Is StyledDocument Then
						Dim doc As StyledDocument = CType(d, StyledDocument)
						Return doc.getText(offset, length)
					End If
				End If
				Return Nothing
			End Function

	'        
	'         * Returns the bounding rectangle for the component text.
	'         
			Private Property textRectangle As Rectangle
				Get
    
					Dim ___text As String = outerInstance.text
					Dim icon As Icon = If(outerInstance.enabled, outerInstance.icon, outerInstance.disabledIcon)
    
					If (icon Is Nothing) AndAlso (___text Is Nothing) Then Return Nothing
    
					Dim paintIconR As New Rectangle
					Dim paintTextR As New Rectangle
					Dim paintViewR As New Rectangle
					Dim paintViewInsets As New Insets(0, 0, 0, 0)
    
					paintViewInsets = outerInstance.getInsets(paintViewInsets)
					paintViewR.x = paintViewInsets.left
					paintViewR.y = paintViewInsets.top
					paintViewR.width = outerInstance.width - (paintViewInsets.left + paintViewInsets.right)
					paintViewR.height = outerInstance.height - (paintViewInsets.top + paintViewInsets.bottom)
    
					Dim clippedText As String = SwingUtilities.layoutCompoundLabel(AbstractButton.this, outerInstance.getFontMetrics(font), ___text, icon, outerInstance.verticalAlignment, outerInstance.horizontalAlignment, outerInstance.verticalTextPosition, outerInstance.horizontalTextPosition, paintViewR, paintIconR, paintTextR, 0)
    
					Return paintTextR
				End Get
			End Property

			' ----- AccessibleExtendedComponent

			''' <summary>
			''' Returns the AccessibleExtendedComponent
			''' </summary>
			''' <returns> the AccessibleExtendedComponent </returns>
			Friend Overridable Property accessibleExtendedComponent As AccessibleExtendedComponent
				Get
					Return Me
				End Get
			End Property

			''' <summary>
			''' Returns the tool tip text
			''' </summary>
			''' <returns> the tool tip text, if supported, of the object;
			''' otherwise, null
			''' @since 1.4 </returns>
			Public Overridable Property toolTipText As String Implements AccessibleExtendedComponent.getToolTipText
				Get
					Return outerInstance.toolTipText
				End Get
			End Property

			''' <summary>
			''' Returns the titled border text
			''' </summary>
			''' <returns> the titled border text, if supported, of the object;
			''' otherwise, null
			''' @since 1.4 </returns>
			Public Overridable Property titledBorderText As String Implements AccessibleExtendedComponent.getTitledBorderText
				Get
					Return MyBase.titledBorderText
				End Get
			End Property

			''' <summary>
			''' Returns key bindings associated with this object
			''' </summary>
			''' <returns> the key bindings, if supported, of the object;
			''' otherwise, null </returns>
			''' <seealso cref= AccessibleKeyBinding
			''' @since 1.4 </seealso>
			Public Overridable Property accessibleKeyBinding As AccessibleKeyBinding Implements AccessibleExtendedComponent.getAccessibleKeyBinding
				Get
					Dim mnemonic As Integer = outerInstance.mnemonic
					If mnemonic = 0 Then Return Nothing
					Return New ButtonKeyBinding(Me, mnemonic)
				End Get
			End Property

			Friend Class ButtonKeyBinding
				Implements AccessibleKeyBinding

				Private ReadOnly outerInstance As AbstractButton.AccessibleAbstractButton

				Friend mnemonic As Integer

				Friend Sub New(ByVal outerInstance As AbstractButton.AccessibleAbstractButton, ByVal mnemonic As Integer)
						Me.outerInstance = outerInstance
					Me.mnemonic = mnemonic
				End Sub

				''' <summary>
				''' Returns the number of key bindings for this object
				''' </summary>
				''' <returns> the zero-based number of key bindings for this object </returns>
				Public Overridable Property accessibleKeyBindingCount As Integer Implements AccessibleKeyBinding.getAccessibleKeyBindingCount
					Get
						Return 1
					End Get
				End Property

				''' <summary>
				''' Returns a key binding for this object.  The value returned is an
				''' java.lang.Object which must be cast to appropriate type depending
				''' on the underlying implementation of the key.  For example, if the
				''' Object returned is a javax.swing.KeyStroke, the user of this
				''' method should do the following:
				''' <nf><code>
				''' Component c = <get the component that has the key bindings>
				''' AccessibleContext ac = c.getAccessibleContext();
				''' AccessibleKeyBinding akb = ac.getAccessibleKeyBinding();
				''' for (int i = 0; i < akb.getAccessibleKeyBindingCount(); i++) {
				'''     Object o = akb.getAccessibleKeyBinding(i);
				'''     if (o instanceof javax.swing.KeyStroke) {
				'''         javax.swing.KeyStroke keyStroke = (javax.swing.KeyStroke)o;
				'''         <do something with the key binding>
				'''     }
				''' }
				''' </code></nf>
				''' </summary>
				''' <param name="i"> zero-based index of the key bindings </param>
				''' <returns> a javax.lang.Object which specifies the key binding </returns>
				''' <exception cref="IllegalArgumentException"> if the index is
				''' out of bounds </exception>
				''' <seealso cref= #getAccessibleKeyBindingCount </seealso>
				Public Overridable Function getAccessibleKeyBinding(ByVal i As Integer) As Object Implements AccessibleKeyBinding.getAccessibleKeyBinding
					If i <> 0 Then Throw New System.ArgumentException
					Return KeyStroke.getKeyStroke(mnemonic, 0)
				End Function
			End Class
		End Class
	End Class

End Namespace
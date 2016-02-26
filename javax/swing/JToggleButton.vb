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
	''' An implementation of a two-state button.
	''' The <code>JRadioButton</code> and <code>JCheckBox</code> classes
	''' are subclasses of this class.
	''' For information on using them see
	''' <a
	''' href="https://docs.oracle.com/javase/tutorial/uiswing/components/button.html">How to Use Buttons, Check Boxes, and Radio Buttons</a>,
	''' a section in <em>The Java Tutorial</em>.
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
	''' description: An implementation of a two-state button.
	''' </summary>
	''' <seealso cref= JRadioButton </seealso>
	''' <seealso cref= JCheckBox
	''' @author Jeff Dinkins </seealso>
	Public Class JToggleButton
		Inherits AbstractButton
		Implements Accessible

		''' <seealso cref= #getUIClassID </seealso>
		''' <seealso cref= #readObject </seealso>
		Private Const uiClassID As String = "ToggleButtonUI"

		''' <summary>
		''' Creates an initially unselected toggle button
		''' without setting the text or image.
		''' </summary>
		Public Sub New()
			Me.New(Nothing, Nothing, False)
		End Sub

		''' <summary>
		''' Creates an initially unselected toggle button
		''' with the specified image but no text.
		''' </summary>
		''' <param name="icon">  the image that the button should display </param>
		Public Sub New(ByVal icon As Icon)
			Me.New(Nothing, icon, False)
		End Sub

		''' <summary>
		''' Creates a toggle button with the specified image
		''' and selection state, but no text.
		''' </summary>
		''' <param name="icon">  the image that the button should display </param>
		''' <param name="selected">  if true, the button is initially selected;
		'''                  otherwise, the button is initially unselected </param>
		Public Sub New(ByVal icon As Icon, ByVal selected As Boolean)
			Me.New(Nothing, icon, selected)
		End Sub

		''' <summary>
		''' Creates an unselected toggle button with the specified text.
		''' </summary>
		''' <param name="text">  the string displayed on the toggle button </param>
		Public Sub New(ByVal text As String)
			Me.New(text, Nothing, False)
		End Sub

		''' <summary>
		''' Creates a toggle button with the specified text
		''' and selection state.
		''' </summary>
		''' <param name="text">  the string displayed on the toggle button </param>
		''' <param name="selected">  if true, the button is initially selected;
		'''                  otherwise, the button is initially unselected </param>
		Public Sub New(ByVal text As String, ByVal selected As Boolean)
			Me.New(text, Nothing, selected)
		End Sub

		''' <summary>
		''' Creates a toggle button where properties are taken from the
		''' Action supplied.
		''' 
		''' @since 1.3
		''' </summary>
		Public Sub New(ByVal a As Action)
			Me.New()
			action = a
		End Sub

		''' <summary>
		''' Creates a toggle button that has the specified text and image,
		''' and that is initially unselected.
		''' </summary>
		''' <param name="text"> the string displayed on the button </param>
		''' <param name="icon">  the image that the button should display </param>
		Public Sub New(ByVal text As String, ByVal icon As Icon)
			Me.New(text, icon, False)
		End Sub

		''' <summary>
		''' Creates a toggle button with the specified text, image, and
		''' selection state.
		''' </summary>
		''' <param name="text"> the text of the toggle button </param>
		''' <param name="icon">  the image that the button should display </param>
		''' <param name="selected">  if true, the button is initially selected;
		'''                  otherwise, the button is initially unselected </param>
		Public Sub New(ByVal text As String, ByVal icon As Icon, ByVal selected As Boolean)
			' Create the model
			model = New ToggleButtonModel

			model.selected = selected

			' initialize
			init(text, icon)
		End Sub

		''' <summary>
		''' Resets the UI property to a value from the current look and feel.
		''' </summary>
		''' <seealso cref= JComponent#updateUI </seealso>
		Public Overrides Sub updateUI()
			uI = CType(UIManager.getUI(Me), ButtonUI)
		End Sub

		''' <summary>
		''' Returns a string that specifies the name of the l&amp;f class
		''' that renders this component.
		''' </summary>
		''' <returns> String "ToggleButtonUI" </returns>
		''' <seealso cref= JComponent#getUIClassID </seealso>
		''' <seealso cref= UIDefaults#getUI
		''' @beaninfo
		'''  description: A string that specifies the name of the L&amp;F class </seealso>
		Public Property Overrides uIClassID As String
			Get
				Return uiClassID
			End Get
		End Property


		''' <summary>
		''' Overriden to return true, JToggleButton supports
		''' the selected state.
		''' </summary>
		Friend Overrides Function shouldUpdateSelectedStateFromAction() As Boolean
			Return True
		End Function

		' *********************************************************************

		''' <summary>
		''' The ToggleButton model
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
		Public Class ToggleButtonModel
			Inherits DefaultButtonModel

			''' <summary>
			''' Creates a new ToggleButton Model
			''' </summary>
			Public Sub New()
			End Sub

			''' <summary>
			''' Checks if the button is selected.
			''' </summary>
			Public Property Overrides selected As Boolean
				Get
		'              if(getGroup() != null) {
		'                  return getGroup().isSelected(this);
		'              } else {
						Return (stateMask And SELECTED) <> 0
		'              }
				End Get
				Set(ByVal b As Boolean)
					Dim ___group As ButtonGroup = group
					If ___group IsNot Nothing Then
						' use the group model instead
						___group.selectedted(Me, b)
						b = ___group.isSelected(Me)
					End If
    
					If selected = b Then Return
    
					If b Then
						stateMask = stateMask Or SELECTED
					Else
						stateMask = stateMask And Not SELECTED
					End If
    
					' Send ChangeEvent
					fireStateChanged()
    
					' Send ItemEvent
					fireItemStateChanged(New ItemEvent(Me, ItemEvent.ITEM_STATE_CHANGED, Me,If(Me.selected, ItemEvent.SELECTED, ItemEvent.DESELECTED)))
    
				End Set
			End Property



			''' <summary>
			''' Sets the pressed state of the toggle button.
			''' </summary>
			Public Overrides Property pressed As Boolean
				Set(ByVal b As Boolean)
					If (pressed = b) OrElse (Not enabled) Then Return
    
					If b = False AndAlso armed Then selected = (Not Me.selected)
    
					If b Then
						stateMask = stateMask Or PRESSED
					Else
						stateMask = stateMask And Not PRESSED
					End If
    
					fireStateChanged()
    
					If (Not pressed) AndAlso armed Then
						Dim modifiers As Integer = 0
						Dim currentEvent As AWTEvent = EventQueue.currentEvent
						If TypeOf currentEvent Is InputEvent Then
							modifiers = CType(currentEvent, InputEvent).modifiers
						ElseIf TypeOf currentEvent Is ActionEvent Then
							modifiers = CType(currentEvent, ActionEvent).modifiers
						End If
						fireActionPerformed(New ActionEvent(Me, ActionEvent.ACTION_PERFORMED, actionCommand, EventQueue.mostRecentEventTime, modifiers))
					End If
    
				End Set
			End Property
		End Class


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
		''' Returns a string representation of this JToggleButton. This method
		''' is intended to be used only for debugging purposes, and the
		''' content and format of the returned string may vary between
		''' implementations. The returned string may be empty but may not
		''' be <code>null</code>.
		''' </summary>
		''' <returns>  a string representation of this JToggleButton. </returns>
		Protected Friend Overrides Function paramString() As String
			Return MyBase.paramString()
		End Function


	'///////////////
	' Accessibility support
	'//////////////

		''' <summary>
		''' Gets the AccessibleContext associated with this JToggleButton.
		''' For toggle buttons, the AccessibleContext takes the form of an
		''' AccessibleJToggleButton.
		''' A new AccessibleJToggleButton instance is created if necessary.
		''' </summary>
		''' <returns> an AccessibleJToggleButton that serves as the
		'''         AccessibleContext of this JToggleButton
		''' @beaninfo
		'''       expert: true
		'''  description: The AccessibleContext associated with this ToggleButton. </returns>
		Public Overridable Property accessibleContext As AccessibleContext Implements Accessible.getAccessibleContext
			Get
				If accessibleContext Is Nothing Then accessibleContext = New AccessibleJToggleButton(Me)
				Return accessibleContext
			End Get
		End Property

		''' <summary>
		''' This class implements accessibility support for the
		''' <code>JToggleButton</code> class.  It provides an implementation of the
		''' Java Accessibility API appropriate to toggle button user-interface
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
		Protected Friend Class AccessibleJToggleButton
			Inherits AccessibleAbstractButton
			Implements ItemListener

			Private ReadOnly outerInstance As JToggleButton


			Public Sub New(ByVal outerInstance As JToggleButton)
					Me.outerInstance = outerInstance
				MyBase.New()
				outerInstance.addItemListener(Me)
			End Sub

			''' <summary>
			''' Fire accessible property change events when the state of the
			''' toggle button changes.
			''' </summary>
			Public Overridable Sub itemStateChanged(ByVal e As ItemEvent)
				Dim tb As JToggleButton = CType(e.source, JToggleButton)
				If outerInstance.accessibleContext IsNot Nothing Then
					If tb.selected Then
						outerInstance.accessibleContext.firePropertyChange(AccessibleContext.ACCESSIBLE_STATE_PROPERTY, Nothing, AccessibleState.CHECKED)
					Else
						outerInstance.accessibleContext.firePropertyChange(AccessibleContext.ACCESSIBLE_STATE_PROPERTY, AccessibleState.CHECKED, Nothing)
					End If
				End If
			End Sub

			''' <summary>
			''' Get the role of this object.
			''' </summary>
			''' <returns> an instance of AccessibleRole describing the role of the
			''' object </returns>
			Public Overridable Property accessibleRole As AccessibleRole
				Get
					Return AccessibleRole.TOGGLE_BUTTON
				End Get
			End Property
		End Class ' inner class AccessibleJToggleButton
	End Class

End Namespace
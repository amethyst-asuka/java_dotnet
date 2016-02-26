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
	''' An implementation of a radio button -- an item that can be selected or
	''' deselected, and which displays its state to the user.
	''' Used with a <seealso cref="ButtonGroup"/> object to create a group of buttons
	''' in which only one button at a time can be selected. (Create a ButtonGroup
	''' object and use its <code>add</code> method to include the JRadioButton objects
	''' in the group.)
	''' <blockquote>
	''' <strong>Note:</strong>
	''' The ButtonGroup object is a logical grouping -- not a physical grouping.
	''' To create a button panel, you should still create a <seealso cref="JPanel"/> or similar
	''' container-object and add a <seealso cref="javax.swing.border.Border"/> to it to set it off from surrounding
	''' components.
	''' </blockquote>
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
	''' See <a href="https://docs.oracle.com/javase/tutorial/uiswing/components/button.html">How to Use Buttons, Check Boxes, and Radio Buttons</a>
	''' in <em>The Java Tutorial</em>
	''' for further documentation.
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
	''' description: A component which can display it's state as selected or deselected.
	''' </summary>
	''' <seealso cref= ButtonGroup </seealso>
	''' <seealso cref= JCheckBox
	''' @author Jeff Dinkins </seealso>
	Public Class JRadioButton
		Inherits JToggleButton
		Implements Accessible

		''' <seealso cref= #getUIClassID </seealso>
		''' <seealso cref= #readObject </seealso>
		Private Const uiClassID As String = "RadioButtonUI"


		''' <summary>
		''' Creates an initially unselected radio button
		''' with no set text.
		''' </summary>
		Public Sub New()
			Me.New(Nothing, Nothing, False)
		End Sub

		''' <summary>
		''' Creates an initially unselected radio button
		''' with the specified image but no text.
		''' </summary>
		''' <param name="icon">  the image that the button should display </param>
		Public Sub New(ByVal icon As Icon)
			Me.New(Nothing, icon, False)
		End Sub

		''' <summary>
		''' Creates a radiobutton where properties are taken from the
		''' Action supplied.
		''' 
		''' @since 1.3
		''' </summary>
		Public Sub New(ByVal a As Action)
			Me.New()
			action = a
		End Sub

		''' <summary>
		''' Creates a radio button with the specified image
		''' and selection state, but no text.
		''' </summary>
		''' <param name="icon">  the image that the button should display </param>
		''' <param name="selected">  if true, the button is initially selected;
		'''                  otherwise, the button is initially unselected </param>
		Public Sub New(ByVal icon As Icon, ByVal selected As Boolean)
			Me.New(Nothing, icon, selected)
		End Sub

		''' <summary>
		''' Creates an unselected radio button with the specified text.
		''' </summary>
		''' <param name="text">  the string displayed on the radio button </param>
		Public Sub New(ByVal text As String)
			Me.New(text, Nothing, False)
		End Sub

		''' <summary>
		''' Creates a radio button with the specified text
		''' and selection state.
		''' </summary>
		''' <param name="text">  the string displayed on the radio button </param>
		''' <param name="selected">  if true, the button is initially selected;
		'''                  otherwise, the button is initially unselected </param>
		Public Sub New(ByVal text As String, ByVal selected As Boolean)
			Me.New(text, Nothing, selected)
		End Sub

		''' <summary>
		''' Creates a radio button that has the specified text and image,
		''' and that is initially unselected.
		''' </summary>
		''' <param name="text">  the string displayed on the radio button </param>
		''' <param name="icon">  the image that the button should display </param>
		Public Sub New(ByVal text As String, ByVal icon As Icon)
			Me.New(text, icon, False)
		End Sub

		''' <summary>
		''' Creates a radio button that has the specified text, image,
		''' and selection state.
		''' </summary>
		''' <param name="text">  the string displayed on the radio button </param>
		''' <param name="icon">  the image that the button should display </param>
		Public Sub New(ByVal text As String, ByVal icon As Icon, ByVal selected As Boolean)
			MyBase.New(text, icon, selected)
			borderPainted = False
			horizontalAlignment = LEADING
		End Sub


		''' <summary>
		''' Resets the UI property to a value from the current look and feel.
		''' </summary>
		''' <seealso cref= JComponent#updateUI </seealso>
		Public Overrides Sub updateUI()
			uI = CType(UIManager.getUI(Me), ButtonUI)
		End Sub


		''' <summary>
		''' Returns the name of the L&amp;F class
		''' that renders this component.
		''' </summary>
		''' <returns> String "RadioButtonUI" </returns>
		''' <seealso cref= JComponent#getUIClassID </seealso>
		''' <seealso cref= UIDefaults#getUI
		''' @beaninfo
		'''        expert: true
		'''   description: A string that specifies the name of the L&amp;F class. </seealso>
		Public Property Overrides uIClassID As String
			Get
				Return uiClassID
			End Get
		End Property


		''' <summary>
		''' The icon for radio buttons comes from the look and feel,
		''' not the Action.
		''' </summary>
		Friend Overrides Property iconFromAction As Action
			Set(ByVal a As Action)
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
		''' Returns a string representation of this JRadioButton. This method
		''' is intended to be used only for debugging purposes, and the
		''' content and format of the returned string may vary between
		''' implementations. The returned string may be empty but may not
		''' be <code>null</code>.
		''' </summary>
		''' <returns>  a string representation of this JRadioButton. </returns>
		Protected Friend Overrides Function paramString() As String
			Return MyBase.paramString()
		End Function


	'///////////////
	' Accessibility support
	'//////////////


		''' <summary>
		''' Gets the AccessibleContext associated with this JRadioButton.
		''' For JRadioButtons, the AccessibleContext takes the form of an
		''' AccessibleJRadioButton.
		''' A new AccessibleJRadioButton instance is created if necessary.
		''' </summary>
		''' <returns> an AccessibleJRadioButton that serves as the
		'''         AccessibleContext of this JRadioButton
		''' @beaninfo
		'''       expert: true
		'''  description: The AccessibleContext associated with this Button </returns>
		Public Property Overrides accessibleContext As AccessibleContext Implements Accessible.getAccessibleContext
			Get
				If accessibleContext Is Nothing Then accessibleContext = New AccessibleJRadioButton(Me)
				Return accessibleContext
			End Get
		End Property

		''' <summary>
		''' This class implements accessibility support for the
		''' <code>JRadioButton</code> class.  It provides an implementation of the
		''' Java Accessibility API appropriate to radio button
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
		''' </summary>
		Protected Friend Class AccessibleJRadioButton
			Inherits AccessibleJToggleButton

			Private ReadOnly outerInstance As JRadioButton

			Public Sub New(ByVal outerInstance As JRadioButton)
				Me.outerInstance = outerInstance
			End Sub


			''' <summary>
			''' Get the role of this object.
			''' </summary>
			''' <returns> an instance of AccessibleRole describing the role of the object </returns>
			''' <seealso cref= AccessibleRole </seealso>
			Public Overridable Property accessibleRole As AccessibleRole
				Get
					Return AccessibleRole.RADIO_BUTTON
				End Get
			End Property

		End Class ' inner class AccessibleJRadioButton
	End Class

End Namespace
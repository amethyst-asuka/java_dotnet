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
	''' An implementation of a radio button menu item.
	''' A <code>JRadioButtonMenuItem</code> is
	''' a menu item that is part of a group of menu items in which only one
	''' item in the group can be selected. The selected item displays its
	''' selected state. Selecting it causes any other selected item to
	''' switch to the unselected state.
	''' To control the selected state of a group of radio button menu items,
	''' use a <code>ButtonGroup</code> object.
	''' <p>
	''' Menu items can be configured, and to some degree controlled, by
	''' <code><a href="Action.html">Action</a></code>s.  Using an
	''' <code>Action</code> with a menu item has many benefits beyond directly
	''' configuring a menu item.  Refer to <a href="Action.html#buttonActions">
	''' Swing Components Supporting <code>Action</code></a> for more
	''' details, and you can find more information in <a
	''' href="https://docs.oracle.com/javase/tutorial/uiswing/misc/action.html">How
	''' to Use Actions</a>, a section in <em>The Java Tutorial</em>.
	''' <p>
	''' For further documentation and examples see
	''' <a
	''' href="https://docs.oracle.com/javase/tutorial/uiswing/components/menu.html">How to Use Menus</a>,
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
	''' description: A component within a group of menu items which can be selected.
	''' 
	''' @author Georges Saab
	''' @author David Karlton </summary>
	''' <seealso cref= ButtonGroup </seealso>
	Public Class JRadioButtonMenuItem
		Inherits JMenuItem
		Implements Accessible

		''' <seealso cref= #getUIClassID </seealso>
		''' <seealso cref= #readObject </seealso>
		Private Const uiClassID As String = "RadioButtonMenuItemUI"

		''' <summary>
		''' Creates a <code>JRadioButtonMenuItem</code> with no set text or icon.
		''' </summary>
		Public Sub New()
			Me.New(Nothing, Nothing, False)
		End Sub

		''' <summary>
		''' Creates a <code>JRadioButtonMenuItem</code> with an icon.
		''' </summary>
		''' <param name="icon"> the <code>Icon</code> to display on the
		'''          <code>JRadioButtonMenuItem</code> </param>
		Public Sub New(ByVal icon As Icon)
			Me.New(Nothing, icon, False)
		End Sub

		''' <summary>
		''' Creates a <code>JRadioButtonMenuItem</code> with text.
		''' </summary>
		''' <param name="text"> the text of the <code>JRadioButtonMenuItem</code> </param>
		Public Sub New(ByVal text As String)
			Me.New(text, Nothing, False)
		End Sub

		''' <summary>
		''' Creates a radio button menu item whose properties are taken from the
		''' <code>Action</code> supplied.
		''' </summary>
		''' <param name="a"> the <code>Action</code> on which to base the radio
		'''          button menu item
		''' 
		''' @since 1.3 </param>
		Public Sub New(ByVal a As Action)
			Me.New()
			action = a
		End Sub

		''' <summary>
		''' Creates a radio button menu item with the specified text
		''' and <code>Icon</code>.
		''' </summary>
		''' <param name="text"> the text of the <code>JRadioButtonMenuItem</code> </param>
		''' <param name="icon"> the icon to display on the <code>JRadioButtonMenuItem</code> </param>
		Public Sub New(ByVal text As String, ByVal icon As Icon)
			Me.New(text, icon, False)
		End Sub

		''' <summary>
		''' Creates a radio button menu item with the specified text
		''' and selection state.
		''' </summary>
		''' <param name="text"> the text of the <code>CheckBoxMenuItem</code> </param>
		''' <param name="selected"> the selected state of the <code>CheckBoxMenuItem</code> </param>
		Public Sub New(ByVal text As String, ByVal selected As Boolean)
			Me.New(text)
			selected = selected
		End Sub

		''' <summary>
		''' Creates a radio button menu item with the specified image
		''' and selection state, but no text.
		''' </summary>
		''' <param name="icon">  the image that the button should display </param>
		''' <param name="selected">  if true, the button is initially selected;
		'''                  otherwise, the button is initially unselected </param>
		Public Sub New(ByVal icon As Icon, ByVal selected As Boolean)
			Me.New(Nothing, icon, selected)
		End Sub

		''' <summary>
		''' Creates a radio button menu item that has the specified
		''' text, image, and selection state.  All other constructors
		''' defer to this one.
		''' </summary>
		''' <param name="text">  the string displayed on the radio button </param>
		''' <param name="icon">  the image that the button should display </param>
		Public Sub New(ByVal text As String, ByVal icon As Icon, ByVal selected As Boolean)
			MyBase.New(text, icon)
			model = New JToggleButton.ToggleButtonModel
			selected = selected
			focusable = False
		End Sub

		''' <summary>
		''' Returns the name of the L&amp;F class that renders this component.
		''' </summary>
		''' <returns> the string "RadioButtonMenuItemUI" </returns>
		''' <seealso cref= JComponent#getUIClassID </seealso>
		''' <seealso cref= UIDefaults#getUI </seealso>
		Public Property Overrides uIClassID As String
			Get
				Return uiClassID
			End Get
		End Property

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


		''' <summary>
		''' Returns a string representation of this
		''' <code>JRadioButtonMenuItem</code>.  This method
		''' is intended to be used only for debugging purposes, and the
		''' content and format of the returned string may vary between
		''' implementations. The returned string may be empty but may not
		''' be <code>null</code>.
		''' </summary>
		''' <returns>  a string representation of this
		'''          <code>JRadioButtonMenuItem</code> </returns>
		Protected Friend Overrides Function paramString() As String
			Return MyBase.paramString()
		End Function

		''' <summary>
		''' Overriden to return true, JRadioButtonMenuItem supports
		''' the selected state.
		''' </summary>
		Friend Overrides Function shouldUpdateSelectedStateFromAction() As Boolean
			Return True
		End Function

	'///////////////
	' Accessibility support
	'//////////////

		''' <summary>
		''' Gets the AccessibleContext associated with this JRadioButtonMenuItem.
		''' For JRadioButtonMenuItems, the AccessibleContext takes the form of an
		''' AccessibleJRadioButtonMenuItem.
		''' A new AccessibleJRadioButtonMenuItem instance is created if necessary.
		''' </summary>
		''' <returns> an AccessibleJRadioButtonMenuItem that serves as the
		'''         AccessibleContext of this JRadioButtonMenuItem </returns>
		Public Property Overrides accessibleContext As AccessibleContext Implements Accessible.getAccessibleContext
			Get
				If accessibleContext Is Nothing Then accessibleContext = New AccessibleJRadioButtonMenuItem(Me)
				Return accessibleContext
			End Get
		End Property

		''' <summary>
		''' This class implements accessibility support for the
		''' <code>JRadioButtonMenuItem</code> class.  It provides an
		''' implementation of the Java Accessibility API appropriate to
		''' <code>JRadioButtonMenuItem</code> user-interface elements.
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
		Protected Friend Class AccessibleJRadioButtonMenuItem
			Inherits AccessibleJMenuItem

			Private ReadOnly outerInstance As JRadioButtonMenuItem

			Public Sub New(ByVal outerInstance As JRadioButtonMenuItem)
				Me.outerInstance = outerInstance
			End Sub

			''' <summary>
			''' Get the role of this object.
			''' </summary>
			''' <returns> an instance of AccessibleRole describing the role of the
			''' object </returns>
			Public Overridable Property accessibleRole As AccessibleRole
				Get
					Return AccessibleRole.RADIO_BUTTON
				End Get
			End Property
		End Class ' inner class AccessibleJRadioButtonMenuItem
	End Class

End Namespace
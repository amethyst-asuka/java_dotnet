Imports System.Runtime.CompilerServices
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
	''' A menu item that can be selected or deselected. If selected, the menu
	''' item typically appears with a checkmark next to it. If unselected or
	''' deselected, the menu item appears without a checkmark. Like a regular
	''' menu item, a check box menu item can have either text or a graphic
	''' icon associated with it, or both.
	''' <p>
	''' Either <code>isSelected</code>/<code>setSelected</code> or
	''' <code>getState</code>/<code>setState</code> can be used
	''' to determine/specify the menu item's selection state. The
	''' preferred methods are <code>isSelected</code> and
	''' <code>setSelected</code>, which work for all menus and buttons.
	''' The <code>getState</code> and <code>setState</code> methods exist for
	''' compatibility with other component sets.
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
	''' For further information and examples of using check box menu items,
	''' see <a
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
	''' description: A menu item which can be selected or deselected.
	''' 
	''' @author Georges Saab
	''' @author David Karlton
	''' </summary>
	Public Class JCheckBoxMenuItem
		Inherits JMenuItem
		Implements SwingConstants, Accessible

		''' <seealso cref= #getUIClassID </seealso>
		''' <seealso cref= #readObject </seealso>
		Private Const uiClassID As String = "CheckBoxMenuItemUI"

		''' <summary>
		''' Creates an initially unselected check box menu item with no set text or icon.
		''' </summary>
		Public Sub New()
			Me.New(Nothing, Nothing, False)
		End Sub

		''' <summary>
		''' Creates an initially unselected check box menu item with an icon.
		''' </summary>
		''' <param name="icon"> the icon of the CheckBoxMenuItem. </param>
		Public Sub New(ByVal icon As Icon)
			Me.New(Nothing, icon, False)
		End Sub

		''' <summary>
		''' Creates an initially unselected check box menu item with text.
		''' </summary>
		''' <param name="text"> the text of the CheckBoxMenuItem </param>
		Public Sub New(ByVal text As String)
			Me.New(text, Nothing, False)
		End Sub

		''' <summary>
		''' Creates a menu item whose properties are taken from the
		''' Action supplied.
		''' 
		''' @since 1.3
		''' </summary>
		Public Sub New(ByVal a As Action)
			Me.New()
			action = a
		End Sub

		''' <summary>
		''' Creates an initially unselected check box menu item with the specified text and icon.
		''' </summary>
		''' <param name="text"> the text of the CheckBoxMenuItem </param>
		''' <param name="icon"> the icon of the CheckBoxMenuItem </param>
		Public Sub New(ByVal text As String, ByVal icon As Icon)
			Me.New(text, icon, False)
		End Sub

		''' <summary>
		''' Creates a check box menu item with the specified text and selection state.
		''' </summary>
		''' <param name="text"> the text of the check box menu item. </param>
		''' <param name="b"> the selected state of the check box menu item </param>
		Public Sub New(ByVal text As String, ByVal b As Boolean)
			Me.New(text, Nothing, b)
		End Sub

		''' <summary>
		''' Creates a check box menu item with the specified text, icon, and selection state.
		''' </summary>
		''' <param name="text"> the text of the check box menu item </param>
		''' <param name="icon"> the icon of the check box menu item </param>
		''' <param name="b"> the selected state of the check box menu item </param>
		Public Sub New(ByVal text As String, ByVal icon As Icon, ByVal b As Boolean)
			MyBase.New(text, icon)
			model = New JToggleButton.ToggleButtonModel
			selected = b
			focusable = False
		End Sub

		''' <summary>
		''' Returns the name of the L&amp;F class
		''' that renders this component.
		''' </summary>
		''' <returns> "CheckBoxMenuItemUI" </returns>
		''' <seealso cref= JComponent#getUIClassID </seealso>
		''' <seealso cref= UIDefaults#getUI </seealso>
		Public Property Overrides uIClassID As String
			Get
				Return uiClassID
			End Get
		End Property

		 ''' <summary>
		 ''' Returns the selected-state of the item. This method
		 ''' exists for AWT compatibility only.  New code should
		 ''' use isSelected() instead.
		 ''' </summary>
		 ''' <returns> true  if the item is selected </returns>
		Public Overridable Property state As Boolean
			Get
				Return selected
			End Get
			Set(ByVal b As Boolean)
				selected = b
			End Set
		End Property



		''' <summary>
		''' Returns an array (length 1) containing the check box menu item
		''' label or null if the check box is not selected.
		''' </summary>
		''' <returns> an array containing one Object -- the text of the menu item
		'''         -- if the item is selected; otherwise null </returns>
		Public Property Overrides selectedObjects As Object()
			Get
				If selected = False Then Return Nothing
				Dim ___selectedObjects As Object() = New Object(0){}
				___selectedObjects(0) = text
				Return ___selectedObjects
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
		''' Returns a string representation of this JCheckBoxMenuItem. This method
		''' is intended to be used only for debugging purposes, and the
		''' content and format of the returned string may vary between
		''' implementations. The returned string may be empty but may not
		''' be <code>null</code>.
		''' </summary>
		''' <returns>  a string representation of this JCheckBoxMenuItem. </returns>
		Protected Friend Overrides Function paramString() As String
			Return MyBase.paramString()
		End Function

		''' <summary>
		''' Overriden to return true, JCheckBoxMenuItem supports
		''' the selected state.
		''' </summary>
		Friend Overrides Function shouldUpdateSelectedStateFromAction() As Boolean
			Return True
		End Function

	'///////////////
	' Accessibility support
	'//////////////

		''' <summary>
		''' Gets the AccessibleContext associated with this JCheckBoxMenuItem.
		''' For JCheckBoxMenuItems, the AccessibleContext takes the form of an
		''' AccessibleJCheckBoxMenuItem.
		''' A new AccessibleJCheckBoxMenuItem instance is created if necessary.
		''' </summary>
		''' <returns> an AccessibleJCheckBoxMenuItem that serves as the
		'''         AccessibleContext of this AccessibleJCheckBoxMenuItem </returns>
		Public Property Overrides accessibleContext As AccessibleContext Implements Accessible.getAccessibleContext
			Get
				If accessibleContext Is Nothing Then accessibleContext = New AccessibleJCheckBoxMenuItem(Me)
				Return accessibleContext
			End Get
		End Property

		''' <summary>
		''' This class implements accessibility support for the
		''' <code>JCheckBoxMenuItem</code> class.  It provides an implementation
		''' of the Java Accessibility API appropriate to checkbox menu item
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
		Protected Friend Class AccessibleJCheckBoxMenuItem
			Inherits AccessibleJMenuItem

			Private ReadOnly outerInstance As JCheckBoxMenuItem

			Public Sub New(ByVal outerInstance As JCheckBoxMenuItem)
				Me.outerInstance = outerInstance
			End Sub

			''' <summary>
			''' Get the role of this object.
			''' </summary>
			''' <returns> an instance of AccessibleRole describing the role of the
			''' object </returns>
			Public Overridable Property accessibleRole As AccessibleRole
				Get
					Return AccessibleRole.CHECK_BOX
				End Get
			End Property
		End Class ' inner class AccessibleJCheckBoxMenuItem
	End Class

End Namespace
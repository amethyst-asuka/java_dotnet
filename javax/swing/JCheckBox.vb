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
	''' An implementation of a check box -- an item that can be selected or
	''' deselected, and which displays its state to the user.
	''' By convention, any number of check boxes in a group can be selected.
	''' See <a href="https://docs.oracle.com/javase/tutorial/uiswing/components/button.html">How to Use Buttons, Check Boxes, and Radio Buttons</a>
	''' in <em>The Java Tutorial</em>
	''' for examples and information on using check boxes.
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
	''' </summary>
	''' <seealso cref= JRadioButton
	''' 
	''' @beaninfo
	'''   attribute: isContainer false
	''' description: A component which can be selected or deselected.
	''' 
	''' @author Jeff Dinkins </seealso>
	Public Class JCheckBox
		Inherits JToggleButton
		Implements Accessible

		''' <summary>
		''' Identifies a change to the flat property. </summary>
		Public Const BORDER_PAINTED_FLAT_CHANGED_PROPERTY As String = "borderPaintedFlat"

		Private flat As Boolean = False

		''' <seealso cref= #getUIClassID </seealso>
		''' <seealso cref= #readObject </seealso>
		Private Const uiClassID As String = "CheckBoxUI"


		''' <summary>
		''' Creates an initially unselected check box button with no text, no icon.
		''' </summary>
		Public Sub New()
			Me.New(Nothing, Nothing, False)
		End Sub

		''' <summary>
		''' Creates an initially unselected check box with an icon.
		''' </summary>
		''' <param name="icon">  the Icon image to display </param>
		Public Sub New(ByVal icon As Icon)
			Me.New(Nothing, icon, False)
		End Sub

		''' <summary>
		''' Creates a check box with an icon and specifies whether
		''' or not it is initially selected.
		''' </summary>
		''' <param name="icon">  the Icon image to display </param>
		''' <param name="selected"> a boolean value indicating the initial selection
		'''        state. If <code>true</code> the check box is selected </param>
		Public Sub New(ByVal icon As Icon, ByVal selected As Boolean)
			Me.New(Nothing, icon, selected)
		End Sub

		''' <summary>
		''' Creates an initially unselected check box with text.
		''' </summary>
		''' <param name="text"> the text of the check box. </param>
		Public Sub New(ByVal text As String)
			Me.New(text, Nothing, False)
		End Sub

		''' <summary>
		''' Creates a check box where properties are taken from the
		''' Action supplied.
		''' 
		''' @since 1.3
		''' </summary>
		Public Sub New(ByVal a As Action)
			Me.New()
			action = a
		End Sub


		''' <summary>
		''' Creates a check box with text and specifies whether
		''' or not it is initially selected.
		''' </summary>
		''' <param name="text"> the text of the check box. </param>
		''' <param name="selected"> a boolean value indicating the initial selection
		'''        state. If <code>true</code> the check box is selected </param>
		Public Sub New(ByVal text As String, ByVal selected As Boolean)
			Me.New(text, Nothing, selected)
		End Sub

		''' <summary>
		''' Creates an initially unselected check box with
		''' the specified text and icon.
		''' </summary>
		''' <param name="text"> the text of the check box. </param>
		''' <param name="icon">  the Icon image to display </param>
		Public Sub New(ByVal text As String, ByVal icon As Icon)
			Me.New(text, icon, False)
		End Sub

		''' <summary>
		''' Creates a check box with text and icon,
		''' and specifies whether or not it is initially selected.
		''' </summary>
		''' <param name="text"> the text of the check box. </param>
		''' <param name="icon">  the Icon image to display </param>
		''' <param name="selected"> a boolean value indicating the initial selection
		'''        state. If <code>true</code> the check box is selected </param>
		Public Sub New(ByVal text As String, ByVal icon As Icon, ByVal selected As Boolean)
			MyBase.New(text, icon, selected)
			uIPropertyrty("borderPainted", Boolean.FALSE)
			horizontalAlignment = LEADING
		End Sub

		''' <summary>
		''' Sets the <code>borderPaintedFlat</code> property,
		''' which gives a hint to the look and feel as to the
		''' appearance of the check box border.
		''' This is usually set to <code>true</code> when a
		''' <code>JCheckBox</code> instance is used as a
		''' renderer in a component such as a <code>JTable</code> or
		''' <code>JTree</code>.  The default value for the
		''' <code>borderPaintedFlat</code> property is <code>false</code>.
		''' This method fires a property changed event.
		''' Some look and feels might not implement flat borders;
		''' they will ignore this property.
		''' </summary>
		''' <param name="b"> <code>true</code> requests that the border be painted flat;
		'''          <code>false</code> requests normal borders </param>
		''' <seealso cref= #isBorderPaintedFlat
		''' @beaninfo
		'''        bound: true
		'''    attribute: visualUpdate true
		'''  description: Whether the border is painted flat.
		''' @since 1.3 </seealso>
		Public Overridable Property borderPaintedFlat As Boolean
			Set(ByVal b As Boolean)
				Dim oldValue As Boolean = flat
				flat = b
				firePropertyChange(BORDER_PAINTED_FLAT_CHANGED_PROPERTY, oldValue, flat)
				If b <> oldValue Then
					revalidate()
					repaint()
				End If
			End Set
			Get
				Return flat
			End Get
		End Property


		''' <summary>
		''' Resets the UI property to a value from the current look and feel.
		''' </summary>
		''' <seealso cref= JComponent#updateUI </seealso>
		Public Overrides Sub updateUI()
			uI = CType(UIManager.getUI(Me), ButtonUI)
		End Sub


		''' <summary>
		''' Returns a string that specifies the name of the L&amp;F class
		''' that renders this component.
		''' </summary>
		''' <returns> the string "CheckBoxUI" </returns>
		''' <seealso cref= JComponent#getUIClassID </seealso>
		''' <seealso cref= UIDefaults#getUI
		''' @beaninfo
		'''        expert: true
		'''   description: A string that specifies the name of the L&amp;F class </seealso>
		Public Property Overrides uIClassID As String
			Get
				Return uiClassID
			End Get
		End Property


		''' <summary>
		''' The icon for checkboxs comes from the look and feel,
		''' not the Action; this is overriden to do nothing.
		''' </summary>
		Friend Overrides Property iconFromAction As Action
			Set(ByVal a As Action)
			End Set
		End Property

	'     
	'      * See readObject and writeObject in JComponent for more
	'      * information about serialization in Swing.
	'      
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
		''' See JComponent.readObject() for information about serialization
		''' in Swing.
		''' </summary>
		Private Sub readObject(ByVal s As java.io.ObjectInputStream)
			s.defaultReadObject()
			If uIClassID.Equals(uiClassID) Then updateUI()
		End Sub


		''' <summary>
		''' Returns a string representation of this JCheckBox. This method
		''' is intended to be used only for debugging purposes, and the
		''' content and format of the returned string may vary between
		''' implementations. The returned string may be empty but may not
		''' be <code>null</code>.
		''' specific new aspects of the JFC components.
		''' </summary>
		''' <returns>  a string representation of this JCheckBox. </returns>
		Protected Friend Overrides Function paramString() As String
			Return MyBase.paramString()
		End Function

	'///////////////
	' Accessibility support
	'//////////////

		''' <summary>
		''' Gets the AccessibleContext associated with this JCheckBox.
		''' For JCheckBoxes, the AccessibleContext takes the form of an
		''' AccessibleJCheckBox.
		''' A new AccessibleJCheckBox instance is created if necessary.
		''' </summary>
		''' <returns> an AccessibleJCheckBox that serves as the
		'''         AccessibleContext of this JCheckBox
		''' @beaninfo
		'''       expert: true
		'''  description: The AccessibleContext associated with this CheckBox. </returns>
		Public Property Overrides accessibleContext As AccessibleContext Implements Accessible.getAccessibleContext
			Get
				If accessibleContext Is Nothing Then accessibleContext = New AccessibleJCheckBox(Me)
				Return accessibleContext
			End Get
		End Property

		''' <summary>
		''' This class implements accessibility support for the
		''' <code>JCheckBox</code> class.  It provides an implementation of the
		''' Java Accessibility API appropriate to check box user-interface
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
		Protected Friend Class AccessibleJCheckBox
			Inherits AccessibleJToggleButton

			Private ReadOnly outerInstance As JCheckBox

			Public Sub New(ByVal outerInstance As JCheckBox)
				Me.outerInstance = outerInstance
			End Sub


			''' <summary>
			''' Get the role of this object.
			''' </summary>
			''' <returns> an instance of AccessibleRole describing the role of the object </returns>
			''' <seealso cref= AccessibleRole </seealso>
			Public Overridable Property accessibleRole As AccessibleRole
				Get
					Return AccessibleRole.CHECK_BOX
				End Get
			End Property

		End Class ' inner class AccessibleJCheckBox
	End Class

End Namespace
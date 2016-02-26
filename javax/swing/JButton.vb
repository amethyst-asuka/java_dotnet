Imports javax.swing.plaf
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
	''' An implementation of a "push" button.
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
	''' for information and examples of using buttons.
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
	''' description: An implementation of a \"push\" button.
	''' 
	''' @author Jeff Dinkins
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	Public Class JButton
		Inherits AbstractButton
		Implements Accessible

		''' <seealso cref= #getUIClassID </seealso>
		''' <seealso cref= #readObject </seealso>
		Private Const uiClassID As String = "ButtonUI"

		''' <summary>
		''' Creates a button with no set text or icon.
		''' </summary>
		Public Sub New()
			Me.New(Nothing, Nothing)
		End Sub

		''' <summary>
		''' Creates a button with an icon.
		''' </summary>
		''' <param name="icon">  the Icon image to display on the button </param>
		Public Sub New(ByVal icon As Icon)
			Me.New(Nothing, icon)
		End Sub

		''' <summary>
		''' Creates a button with text.
		''' </summary>
		''' <param name="text">  the text of the button </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Sub New(ByVal text As String)
			Me.New(text, Nothing)
		End Sub

		''' <summary>
		''' Creates a button where properties are taken from the
		''' <code>Action</code> supplied.
		''' </summary>
		''' <param name="a"> the <code>Action</code> used to specify the new button
		''' 
		''' @since 1.3 </param>
		Public Sub New(ByVal a As Action)
			Me.New()
			action = a
		End Sub

		''' <summary>
		''' Creates a button with initial text and an icon.
		''' </summary>
		''' <param name="text">  the text of the button </param>
		''' <param name="icon">  the Icon image to display on the button </param>
		Public Sub New(ByVal text As String, ByVal icon As Icon)
			' Create the model
			model = New DefaultButtonModel

			' initialize
			init(text, icon)
		End Sub

		''' <summary>
		''' Resets the UI property to a value from the current look and
		''' feel.
		''' </summary>
		''' <seealso cref= JComponent#updateUI </seealso>
		Public Overrides Sub updateUI()
			uI = CType(UIManager.getUI(Me), ButtonUI)
		End Sub


		''' <summary>
		''' Returns a string that specifies the name of the L&amp;F class
		''' that renders this component.
		''' </summary>
		''' <returns> the string "ButtonUI" </returns>
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
		''' Gets the value of the <code>defaultButton</code> property,
		''' which if <code>true</code> means that this button is the current
		''' default button for its <code>JRootPane</code>.
		''' Most look and feels render the default button
		''' differently, and may potentially provide bindings
		''' to access the default button.
		''' </summary>
		''' <returns> the value of the <code>defaultButton</code> property </returns>
		''' <seealso cref= JRootPane#setDefaultButton </seealso>
		''' <seealso cref= #isDefaultCapable
		''' @beaninfo
		'''  description: Whether or not this button is the default button </seealso>
		Public Overridable Property defaultButton As Boolean
			Get
				Dim root As JRootPane = SwingUtilities.getRootPane(Me)
				If root IsNot Nothing Then Return root.defaultButton Is Me
				Return False
			End Get
		End Property

		''' <summary>
		''' Gets the value of the <code>defaultCapable</code> property.
		''' </summary>
		''' <returns> the value of the <code>defaultCapable</code> property </returns>
		''' <seealso cref= #setDefaultCapable </seealso>
		''' <seealso cref= #isDefaultButton </seealso>
		''' <seealso cref= JRootPane#setDefaultButton </seealso>
		Public Overridable Property defaultCapable As Boolean
			Get
				Return defaultCapable
			End Get
			Set(ByVal defaultCapable As Boolean)
				Dim oldDefaultCapable As Boolean = Me.defaultCapable
				Me.defaultCapable = defaultCapable
				firePropertyChange("defaultCapable", oldDefaultCapable, defaultCapable)
			End Set
		End Property


		''' <summary>
		''' Overrides <code>JComponent.removeNotify</code> to check if
		''' this button is currently set as the default button on the
		''' <code>RootPane</code>, and if so, sets the <code>RootPane</code>'s
		''' default button to <code>null</code> to ensure the
		''' <code>RootPane</code> doesn't hold onto an invalid button reference.
		''' </summary>
		Public Overrides Sub removeNotify()
			Dim root As JRootPane = SwingUtilities.getRootPane(Me)
			If root IsNot Nothing AndAlso root.defaultButton Is Me Then root.defaultButton = Nothing
			MyBase.removeNotify()
		End Sub

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
		''' Returns a string representation of this <code>JButton</code>.
		''' This method is intended to be used only for debugging purposes, and the
		''' content and format of the returned string may vary between
		''' implementations. The returned string may be empty but may not
		''' be <code>null</code>.
		''' </summary>
		''' <returns>  a string representation of this <code>JButton</code> </returns>
		Protected Friend Overrides Function paramString() As String
			Dim defaultCapableString As String = (If(defaultCapable, "true", "false"))

			Return MyBase.paramString() & ",defaultCapable=" & defaultCapableString
		End Function


	'///////////////
	' Accessibility support
	'//////////////

		''' <summary>
		''' Gets the <code>AccessibleContext</code> associated with this
		''' <code>JButton</code>. For <code>JButton</code>s,
		''' the <code>AccessibleContext</code> takes the form of an
		''' <code>AccessibleJButton</code>.
		''' A new <code>AccessibleJButton</code> instance is created if necessary.
		''' </summary>
		''' <returns> an <code>AccessibleJButton</code> that serves as the
		'''         <code>AccessibleContext</code> of this <code>JButton</code>
		''' @beaninfo
		'''       expert: true
		'''  description: The AccessibleContext associated with this Button. </returns>
		Public Overridable Property accessibleContext As AccessibleContext Implements Accessible.getAccessibleContext
			Get
				If accessibleContext Is Nothing Then accessibleContext = New AccessibleJButton(Me)
				Return accessibleContext
			End Get
		End Property

		''' <summary>
		''' This class implements accessibility support for the
		''' <code>JButton</code> class.  It provides an implementation of the
		''' Java Accessibility API appropriate to button user-interface
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
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Protected Friend Class AccessibleJButton
			Inherits AccessibleAbstractButton

			Private ReadOnly outerInstance As JButton

			Public Sub New(ByVal outerInstance As JButton)
				Me.outerInstance = outerInstance
			End Sub


			''' <summary>
			''' Get the role of this object.
			''' </summary>
			''' <returns> an instance of AccessibleRole describing the role of the
			''' object </returns>
			''' <seealso cref= AccessibleRole </seealso>
			Public Overridable Property accessibleRole As AccessibleRole
				Get
					Return AccessibleRole.PUSH_BUTTON
				End Get
			End Property
		End Class ' inner class AccessibleJButton
	End Class

End Namespace
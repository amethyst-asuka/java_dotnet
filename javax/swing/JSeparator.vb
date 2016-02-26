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
	''' <code>JSeparator</code> provides a general purpose component for
	''' implementing divider lines - most commonly used as a divider
	''' between menu items that breaks them up into logical groupings.
	''' Instead of using <code>JSeparator</code> directly,
	''' you can use the <code>JMenu</code> or <code>JPopupMenu</code>
	''' <code>addSeparator</code> method to create and add a separator.
	''' <code>JSeparator</code>s may also be used elsewhere in a GUI
	''' wherever a visual divider is useful.
	''' 
	''' <p>
	''' 
	''' For more information and examples see
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
	'''      attribute: isContainer false
	'''    description: A divider between menu items.
	''' 
	''' @author Georges Saab
	''' @author Jeff Shapiro
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	Public Class JSeparator
		Inherits JComponent
		Implements SwingConstants, Accessible

		''' <seealso cref= #getUIClassID </seealso>
		''' <seealso cref= #readObject </seealso>
		Private Const uiClassID As String = "SeparatorUI"

		Private orientation As Integer = HORIZONTAL

		''' <summary>
		''' Creates a new horizontal separator. </summary>
		Public Sub New()
			Me.New(HORIZONTAL)
		End Sub

		''' <summary>
		''' Creates a new separator with the specified horizontal or
		''' vertical orientation.
		''' </summary>
		''' <param name="orientation"> an integer specifying
		'''          <code>SwingConstants.HORIZONTAL</code> or
		'''          <code>SwingConstants.VERTICAL</code> </param>
		''' <exception cref="IllegalArgumentException"> if <code>orientation</code>
		'''          is neither <code>SwingConstants.HORIZONTAL</code> nor
		'''          <code>SwingConstants.VERTICAL</code> </exception>
		Public Sub New(ByVal orientation As Integer)
			checkOrientation(orientation)
			Me.orientation = orientation
			focusable = False
			updateUI()
		End Sub

		''' <summary>
		''' Returns the L&amp;F object that renders this component.
		''' </summary>
		''' <returns> the SeparatorUI object that renders this component </returns>
		Public Overridable Property uI As SeparatorUI
			Get
				Return CType(ui, SeparatorUI)
			End Get
			Set(ByVal ui As SeparatorUI)
				MyBase.uI = ui
			End Set
		End Property


		''' <summary>
		''' Resets the UI property to a value from the current look and feel.
		''' </summary>
		''' <seealso cref= JComponent#updateUI </seealso>
		Public Overrides Sub updateUI()
			uI = CType(UIManager.getUI(Me), SeparatorUI)
		End Sub


		''' <summary>
		''' Returns the name of the L&amp;F class that renders this component.
		''' </summary>
		''' <returns> the string "SeparatorUI" </returns>
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
		''' Returns the orientation of this separator.
		''' </summary>
		''' <returns>   The value of the orientation property, one of the
		'''           following constants defined in <code>SwingConstants</code>:
		'''           <code>VERTICAL</code>, or
		'''           <code>HORIZONTAL</code>.
		''' </returns>
		''' <seealso cref= SwingConstants </seealso>
		''' <seealso cref= #setOrientation </seealso>
		Public Overridable Property orientation As Integer
			Get
				Return Me.orientation
			End Get
			Set(ByVal orientation As Integer)
				If Me.orientation = orientation Then Return
				Dim oldValue As Integer = Me.orientation
				checkOrientation(orientation)
				Me.orientation = orientation
				firePropertyChange("orientation", oldValue, orientation)
				revalidate()
				repaint()
			End Set
		End Property


		Private Sub checkOrientation(ByVal orientation As Integer)
			Select Case orientation
				Case VERTICAL, HORIZONTAL
				Case Else
					Throw New System.ArgumentException("orientation must be one of: VERTICAL, HORIZONTAL")
			End Select
		End Sub


		''' <summary>
		''' Returns a string representation of this <code>JSeparator</code>.
		''' This method
		''' is intended to be used only for debugging purposes, and the
		''' content and format of the returned string may vary between
		''' implementations. The returned string may be empty but may not
		''' be <code>null</code>.
		''' </summary>
		''' <returns>  a string representation of this <code>JSeparator</code> </returns>
		Protected Friend Overrides Function paramString() As String
			Dim orientationString As String = (If(orientation = HORIZONTAL, "HORIZONTAL", "VERTICAL"))

			Return MyBase.paramString() & ",orientation=" & orientationString
		End Function

	'///////////////
	' Accessibility support
	'//////////////

		''' <summary>
		''' Gets the AccessibleContext associated with this JSeparator.
		''' For separators, the AccessibleContext takes the form of an
		''' AccessibleJSeparator.
		''' A new AccessibleJSeparator instance is created if necessary.
		''' </summary>
		''' <returns> an AccessibleJSeparator that serves as the
		'''         AccessibleContext of this JSeparator </returns>
		Public Overridable Property accessibleContext As AccessibleContext Implements Accessible.getAccessibleContext
			Get
				If accessibleContext Is Nothing Then accessibleContext = New AccessibleJSeparator(Me)
				Return accessibleContext
			End Get
		End Property

		''' <summary>
		''' This class implements accessibility support for the
		''' <code>JSeparator</code> class.  It provides an implementation of the
		''' Java Accessibility API appropriate to separator user-interface elements.
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
		Protected Friend Class AccessibleJSeparator
			Inherits AccessibleJComponent

			Private ReadOnly outerInstance As JSeparator

			Public Sub New(ByVal outerInstance As JSeparator)
				Me.outerInstance = outerInstance
			End Sub


			''' <summary>
			''' Get the role of this object.
			''' </summary>
			''' <returns> an instance of AccessibleRole describing the role of the
			''' object </returns>
			Public Overridable Property accessibleRole As AccessibleRole
				Get
					Return AccessibleRole.SEPARATOR
				End Get
			End Property
		End Class
	End Class

End Namespace
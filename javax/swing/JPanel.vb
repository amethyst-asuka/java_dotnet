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
	''' <code>JPanel</code> is a generic lightweight container.
	''' For examples and task-oriented documentation for JPanel, see
	''' <a
	''' href="https://docs.oracle.com/javase/tutorial/uiswing/components/panel.html">How to Use Panels</a>,
	''' a section in <em>The Java Tutorial</em>.
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
	''' description: A generic lightweight container.
	''' 
	''' @author Arnaud Weber
	''' @author Steve Wilson
	''' </summary>
	Public Class JPanel
		Inherits JComponent
		Implements Accessible

		''' <seealso cref= #getUIClassID </seealso>
		''' <seealso cref= #readObject </seealso>
		Private Const uiClassID As String = "PanelUI"

		''' <summary>
		''' Creates a new JPanel with the specified layout manager and buffering
		''' strategy.
		''' </summary>
		''' <param name="layout">  the LayoutManager to use </param>
		''' <param name="isDoubleBuffered">  a boolean, true for double-buffering, which
		'''        uses additional memory space to achieve fast, flicker-free
		'''        updates </param>
		Public Sub New(ByVal layout As LayoutManager, ByVal isDoubleBuffered As Boolean)
			layout = layout
			doubleBuffered = isDoubleBuffered
			uIPropertyrty("opaque", Boolean.TRUE)
			updateUI()
		End Sub

		''' <summary>
		''' Create a new buffered JPanel with the specified layout manager
		''' </summary>
		''' <param name="layout">  the LayoutManager to use </param>
		Public Sub New(ByVal layout As LayoutManager)
			Me.New(layout, True)
		End Sub

		''' <summary>
		''' Creates a new <code>JPanel</code> with <code>FlowLayout</code>
		''' and the specified buffering strategy.
		''' If <code>isDoubleBuffered</code> is true, the <code>JPanel</code>
		''' will use a double buffer.
		''' </summary>
		''' <param name="isDoubleBuffered">  a boolean, true for double-buffering, which
		'''        uses additional memory space to achieve fast, flicker-free
		'''        updates </param>
		Public Sub New(ByVal isDoubleBuffered As Boolean)
			Me.New(New FlowLayout, isDoubleBuffered)
		End Sub

		''' <summary>
		''' Creates a new <code>JPanel</code> with a double buffer
		''' and a flow layout.
		''' </summary>
		Public Sub New()
			Me.New(True)
		End Sub

		''' <summary>
		''' Resets the UI property with a value from the current look and feel.
		''' </summary>
		''' <seealso cref= JComponent#updateUI </seealso>
		Public Overrides Sub updateUI()
			uI = CType(UIManager.getUI(Me), PanelUI)
		End Sub

		''' <summary>
		''' Returns the look and feel (L&amp;amp;F) object that renders this component.
		''' </summary>
		''' <returns> the PanelUI object that renders this component
		''' @since 1.4 </returns>
		Public Overridable Property uI As PanelUI
			Get
				Return CType(ui, PanelUI)
			End Get
			Set(ByVal ui As PanelUI)
				MyBase.uI = ui
			End Set
		End Property



		''' <summary>
		''' Returns a string that specifies the name of the L&amp;F class
		''' that renders this component.
		''' </summary>
		''' <returns> "PanelUI" </returns>
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
		''' Returns a string representation of this JPanel. This method
		''' is intended to be used only for debugging purposes, and the
		''' content and format of the returned string may vary between
		''' implementations. The returned string may be empty but may not
		''' be <code>null</code>.
		''' </summary>
		''' <returns>  a string representation of this JPanel. </returns>
		Protected Friend Overrides Function paramString() As String
			Return MyBase.paramString()
		End Function

	'///////////////
	' Accessibility support
	'//////////////

		''' <summary>
		''' Gets the AccessibleContext associated with this JPanel.
		''' For JPanels, the AccessibleContext takes the form of an
		''' AccessibleJPanel.
		''' A new AccessibleJPanel instance is created if necessary.
		''' </summary>
		''' <returns> an AccessibleJPanel that serves as the
		'''         AccessibleContext of this JPanel </returns>
		Public Overridable Property accessibleContext As AccessibleContext Implements Accessible.getAccessibleContext
			Get
				If accessibleContext Is Nothing Then accessibleContext = New AccessibleJPanel(Me)
				Return accessibleContext
			End Get
		End Property

		''' <summary>
		''' This class implements accessibility support for the
		''' <code>JPanel</code> class.  It provides an implementation of the
		''' Java Accessibility API appropriate to panel user-interface
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
		Protected Friend Class AccessibleJPanel
			Inherits AccessibleJComponent

			Private ReadOnly outerInstance As JPanel

			Public Sub New(ByVal outerInstance As JPanel)
				Me.outerInstance = outerInstance
			End Sub


			''' <summary>
			''' Get the role of this object.
			''' </summary>
			''' <returns> an instance of AccessibleRole describing the role of the
			''' object </returns>
			Public Overridable Property accessibleRole As AccessibleRole
				Get
					Return AccessibleRole.PANEL
				End Get
			End Property
		End Class
	End Class

End Namespace
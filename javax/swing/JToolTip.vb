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
	''' Used to display a "Tip" for a Component. Typically components provide api
	''' to automate the process of using <code>ToolTip</code>s.
	''' For example, any Swing component can use the <code>JComponent</code>
	''' <code>setToolTipText</code> method to specify the text
	''' for a standard tooltip. A component that wants to create a custom
	''' <code>ToolTip</code>
	''' display can override <code>JComponent</code>'s <code>createToolTip</code>
	''' method and use a subclass of this class.
	''' <p>
	''' See <a href="https://docs.oracle.com/javase/tutorial/uiswing/components/tooltip.html">How to Use Tool Tips</a>
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
	''' </summary>
	''' <seealso cref= JComponent#setToolTipText </seealso>
	''' <seealso cref= JComponent#createToolTip
	''' @author Dave Moore
	''' @author Rich Shiavi </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	Public Class JToolTip
		Inherits JComponent
		Implements Accessible

		''' <seealso cref= #getUIClassID </seealso>
		''' <seealso cref= #readObject </seealso>
		Private Const uiClassID As String = "ToolTipUI"

		Friend tipText As String
		Friend component As JComponent

		''' <summary>
		''' Creates a tool tip. </summary>
		Public Sub New()
			opaque = True
			updateUI()
		End Sub

		''' <summary>
		''' Returns the L&amp;F object that renders this component.
		''' </summary>
		''' <returns> the <code>ToolTipUI</code> object that renders this component </returns>
		Public Overridable Property uI As ToolTipUI
			Get
				Return CType(ui, ToolTipUI)
			End Get
		End Property

		''' <summary>
		''' Resets the UI property to a value from the current look and feel.
		''' </summary>
		''' <seealso cref= JComponent#updateUI </seealso>
		Public Overrides Sub updateUI()
			uI = CType(UIManager.getUI(Me), ToolTipUI)
		End Sub


		''' <summary>
		''' Returns the name of the L&amp;F class that renders this component.
		''' </summary>
		''' <returns> the string "ToolTipUI" </returns>
		''' <seealso cref= JComponent#getUIClassID </seealso>
		''' <seealso cref= UIDefaults#getUI </seealso>
		Public Property Overrides uIClassID As String
			Get
				Return uiClassID
			End Get
		End Property


		''' <summary>
		''' Sets the text to show when the tool tip is displayed.
		''' The string <code>tipText</code> may be <code>null</code>.
		''' </summary>
		''' <param name="tipText"> the <code>String</code> to display
		''' @beaninfo
		'''    preferred: true
		'''        bound: true
		'''  description: Sets the text of the tooltip </param>
		Public Overridable Property tipText As String
			Set(ByVal tipText As String)
				Dim oldValue As String = Me.tipText
				Me.tipText = tipText
				firePropertyChange("tiptext", oldValue, tipText)
    
				If Not java.util.Objects.Equals(oldValue, tipText) Then
					revalidate()
					repaint()
				End If
			End Set
			Get
				Return tipText
			End Get
		End Property


		''' <summary>
		''' Specifies the component that the tooltip describes.
		''' The component <code>c</code> may be <code>null</code>
		''' and will have no effect.
		''' <p>
		''' This is a bound property.
		''' </summary>
		''' <param name="c"> the <code>JComponent</code> being described </param>
		''' <seealso cref= JComponent#createToolTip
		''' @beaninfo
		'''       bound: true
		''' description: Sets the component that the tooltip describes. </seealso>
		Public Overridable Property component As JComponent
			Set(ByVal c As JComponent)
				Dim oldValue As JComponent = Me.component
    
				component = c
				firePropertyChange("component", oldValue, c)
			End Set
			Get
				Return component
			End Get
		End Property


		''' <summary>
		''' Always returns true since tooltips, by definition,
		''' should always be on top of all other windows.
		''' </summary>
		' package private
		Friend Overrides Function alwaysOnTop() As Boolean
			Return True
		End Function


		''' <summary>
		''' See <code>readObject</code> and <code>writeObject</code>
		''' in <code>JComponent</code> for more
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
		''' Returns a string representation of this <code>JToolTip</code>.
		''' This method
		''' is intended to be used only for debugging purposes, and the
		''' content and format of the returned string may vary between
		''' implementations. The returned string may be empty but may not
		''' be <code>null</code>.
		''' </summary>
		''' <returns>  a string representation of this <code>JToolTip</code> </returns>
		Protected Friend Overrides Function paramString() As String
			Dim tipTextString As String = (If(tipText IsNot Nothing, tipText, ""))

			Return MyBase.paramString() & ",tipText=" & tipTextString
		End Function


	'///////////////
	' Accessibility support
	'//////////////

		''' <summary>
		''' Gets the AccessibleContext associated with this JToolTip.
		''' For tool tips, the AccessibleContext takes the form of an
		''' AccessibleJToolTip.
		''' A new AccessibleJToolTip instance is created if necessary.
		''' </summary>
		''' <returns> an AccessibleJToolTip that serves as the
		'''         AccessibleContext of this JToolTip </returns>
		Public Overridable Property accessibleContext As AccessibleContext Implements Accessible.getAccessibleContext
			Get
				If accessibleContext Is Nothing Then accessibleContext = New AccessibleJToolTip(Me)
				Return accessibleContext
			End Get
		End Property

		''' <summary>
		''' This class implements accessibility support for the
		''' <code>JToolTip</code> class.  It provides an implementation of the
		''' Java Accessibility API appropriate to tool tip user-interface elements.
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
		Protected Friend Class AccessibleJToolTip
			Inherits AccessibleJComponent

			Private ReadOnly outerInstance As JToolTip

			Public Sub New(ByVal outerInstance As JToolTip)
				Me.outerInstance = outerInstance
			End Sub


			''' <summary>
			''' Get the accessible description of this object.
			''' </summary>
			''' <returns> a localized String describing this object. </returns>
			Public Overridable Property accessibleDescription As String
				Get
					Dim description As String = accessibleDescription
    
					' fallback to client property
					If description Is Nothing Then description = CStr(outerInstance.getClientProperty(AccessibleContext.ACCESSIBLE_DESCRIPTION_PROPERTY))
					If description Is Nothing Then description = outerInstance.tipText
					Return description
				End Get
			End Property

			''' <summary>
			''' Get the role of this object.
			''' </summary>
			''' <returns> an instance of AccessibleRole describing the role of the
			''' object </returns>
			Public Overridable Property accessibleRole As AccessibleRole
				Get
					Return AccessibleRole.TOOL_TIP
				End Get
			End Property
		End Class
	End Class

End Namespace
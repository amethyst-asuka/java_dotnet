Imports Microsoft.VisualBasic
Imports System
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
	''' <code>JToolBar</code> provides a component that is useful for
	''' displaying commonly used <code>Action</code>s or controls.
	''' For examples and information on using tool bars see
	''' <a href="https://docs.oracle.com/javase/tutorial/uiswing/components/toolbar.html">How to Use Tool Bars</a>,
	''' a section in <em>The Java Tutorial</em>.
	''' 
	''' <p>
	''' With most look and feels,
	''' the user can drag out a tool bar into a separate window
	''' (unless the <code>floatable</code> property is set to <code>false</code>).
	''' For drag-out to work correctly, it is recommended that you add
	''' <code>JToolBar</code> instances to one of the four "sides" of a
	''' container whose layout manager is a <code>BorderLayout</code>,
	''' and do not add children to any of the other four "sides".
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
	'''   attribute: isContainer true
	''' description: A component which displays commonly used controls or Actions.
	''' 
	''' @author Georges Saab
	''' @author Jeff Shapiro </summary>
	''' <seealso cref= Action </seealso>
	Public Class JToolBar
		Inherits JComponent
		Implements SwingConstants, Accessible

		''' <seealso cref= #getUIClassID </seealso>
		''' <seealso cref= #readObject </seealso>
		Private Const uiClassID As String = "ToolBarUI"

		Private ___paintBorder As Boolean = True
		Private margin As java.awt.Insets = Nothing
		Private floatable As Boolean = True
		Private orientation As Integer = HORIZONTAL

		''' <summary>
		''' Creates a new tool bar; orientation defaults to <code>HORIZONTAL</code>.
		''' </summary>
		Public Sub New()
			Me.New(HORIZONTAL)
		End Sub

		''' <summary>
		''' Creates a new tool bar with the specified <code>orientation</code>.
		''' The <code>orientation</code> must be either <code>HORIZONTAL</code>
		''' or <code>VERTICAL</code>.
		''' </summary>
		''' <param name="orientation">  the orientation desired </param>
		Public Sub New(ByVal orientation As Integer)
			Me.New(Nothing, orientation)
		End Sub

		''' <summary>
		''' Creates a new tool bar with the specified <code>name</code>.  The
		''' name is used as the title of the undocked tool bar.  The default
		''' orientation is <code>HORIZONTAL</code>.
		''' </summary>
		''' <param name="name"> the name of the tool bar
		''' @since 1.3 </param>
		Public Sub New(ByVal name As String)
			Me.New(name, HORIZONTAL)
		End Sub

		''' <summary>
		''' Creates a new tool bar with a specified <code>name</code> and
		''' <code>orientation</code>.
		''' All other constructors call this constructor.
		''' If <code>orientation</code> is an invalid value, an exception will
		''' be thrown.
		''' </summary>
		''' <param name="name">  the name of the tool bar </param>
		''' <param name="orientation">  the initial orientation -- it must be
		'''          either <code>HORIZONTAL</code> or <code>VERTICAL</code> </param>
		''' <exception cref="IllegalArgumentException"> if orientation is neither
		'''          <code>HORIZONTAL</code> nor <code>VERTICAL</code>
		''' @since 1.3 </exception>
		Public Sub New(ByVal name As String, ByVal orientation As Integer)
			name = name
			checkOrientation(orientation)

			Me.orientation = orientation
			Dim ___layout As New DefaultToolBarLayout(Me, orientation)
			layout = ___layout

			addPropertyChangeListener(___layout)

			updateUI()
		End Sub

		''' <summary>
		''' Returns the tool bar's current UI. </summary>
		''' <seealso cref= #setUI </seealso>
		Public Overridable Property uI As ToolBarUI
			Get
				Return CType(ui, ToolBarUI)
			End Get
			Set(ByVal ui As ToolBarUI)
				MyBase.uI = ui
			End Set
		End Property


		''' <summary>
		''' Notification from the <code>UIFactory</code> that the L&amp;F has changed.
		''' Called to replace the UI with the latest version from the
		''' <code>UIFactory</code>.
		''' </summary>
		''' <seealso cref= JComponent#updateUI </seealso>
		Public Overrides Sub updateUI()
			uI = CType(UIManager.getUI(Me), ToolBarUI)
			' GTKLookAndFeel installs a different LayoutManager, and sets it
			' to null after changing the look and feel, so, install the default
			' if the LayoutManager is null.
			If layout Is Nothing Then layout = New DefaultToolBarLayout(Me, orientation)
			invalidate()
		End Sub



		''' <summary>
		''' Returns the name of the L&amp;F class that renders this component.
		''' </summary>
		''' <returns> the string "ToolBarUI" </returns>
		''' <seealso cref= JComponent#getUIClassID </seealso>
		''' <seealso cref= UIDefaults#getUI </seealso>
		Public Property Overrides uIClassID As String
			Get
				Return uiClassID
			End Get
		End Property


		''' <summary>
		''' Returns the index of the specified component.
		''' (Note: Separators occupy index positions.)
		''' </summary>
		''' <param name="c">  the <code>Component</code> to find </param>
		''' <returns> an integer indicating the component's position,
		'''          where 0 is first </returns>
		Public Overridable Function getComponentIndex(ByVal c As java.awt.Component) As Integer
			Dim ncomponents As Integer = Me.componentCount
			Dim component As java.awt.Component() = Me.components
			For i As Integer = 0 To ncomponents - 1
				Dim comp As java.awt.Component = component(i)
				If comp Is c Then Return i
			Next i
			Return -1
		End Function

		''' <summary>
		''' Returns the component at the specified index.
		''' </summary>
		''' <param name="i">  the component's position, where 0 is first </param>
		''' <returns>   the <code>Component</code> at that position,
		'''          or <code>null</code> for an invalid index
		'''  </returns>
		Public Overridable Function getComponentAtIndex(ByVal i As Integer) As java.awt.Component
			Dim ncomponents As Integer = Me.componentCount
			If i >= 0 AndAlso i < ncomponents Then
				Dim component As java.awt.Component() = Me.components
				Return component(i)
			End If
			Return Nothing
		End Function

		 ''' <summary>
		 ''' Sets the margin between the tool bar's border and
		 ''' its buttons. Setting to <code>null</code> causes the tool bar to
		 ''' use the default margins. The tool bar's default <code>Border</code>
		 ''' object uses this value to create the proper margin.
		 ''' However, if a non-default border is set on the tool bar,
		 ''' it is that <code>Border</code> object's responsibility to create the
		 ''' appropriate margin space (otherwise this property will
		 ''' effectively be ignored).
		 ''' </summary>
		 ''' <param name="m"> an <code>Insets</code> object that defines the space
		 '''         between the border and the buttons </param>
		 ''' <seealso cref= Insets
		 ''' @beaninfo
		 ''' description: The margin between the tool bar's border and contents
		 '''       bound: true
		 '''      expert: true </seealso>
		 Public Overridable Property margin As java.awt.Insets
			 Set(ByVal m As java.awt.Insets)
				 Dim old As java.awt.Insets = margin
				 margin = m
				 firePropertyChange("margin", old, m)
				 revalidate()
				 repaint()
			 End Set
			 Get
				 If margin Is Nothing Then
					 Return New java.awt.Insets(0,0,0,0)
				 Else
					 Return margin
				 End If
			 End Get
		 End Property


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
				 If ___paintBorder <> b Then
					 Dim old As Boolean = ___paintBorder
					 ___paintBorder = b
					 firePropertyChange("borderPainted", old, b)
					 revalidate()
					 repaint()
				 End If
			 End Set
		 End Property



		 ''' <summary>
		 ''' Paints the tool bar's border if the <code>borderPainted</code> property
		 ''' is <code>true</code>.
		 ''' </summary>
		 ''' <param name="g">  the <code>Graphics</code> context in which the painting
		 '''         is done </param>
		 ''' <seealso cref= JComponent#paint </seealso>
		 ''' <seealso cref= JComponent#setBorder </seealso>
		 Protected Friend Overridable Sub paintBorder(ByVal g As java.awt.Graphics)
			 If borderPainted Then MyBase.paintBorder(g)
		 End Sub

		''' <summary>
		''' Gets the <code>floatable</code> property.
		''' </summary>
		''' <returns> the value of the <code>floatable</code> property
		''' </returns>
		''' <seealso cref= #setFloatable </seealso>
		Public Overridable Property floatable As Boolean
			Get
				Return floatable
			End Get
			Set(ByVal b As Boolean)
				If floatable <> b Then
					Dim old As Boolean = floatable
					floatable = b
    
					firePropertyChange("floatable", old, b)
					revalidate()
					repaint()
				End If
			End Set
		End Property


		''' <summary>
		''' Returns the current orientation of the tool bar.  The value is either
		''' <code>HORIZONTAL</code> or <code>VERTICAL</code>.
		''' </summary>
		''' <returns> an integer representing the current orientation -- either
		'''          <code>HORIZONTAL</code> or <code>VERTICAL</code> </returns>
		''' <seealso cref= #setOrientation </seealso>
		Public Overridable Property orientation As Integer
			Get
				Return Me.orientation
			End Get
			Set(ByVal o As Integer)
				checkOrientation(o)
    
				If orientation <> o Then
					Dim old As Integer = orientation
					orientation = o
    
					firePropertyChange("orientation", old, o)
					revalidate()
					repaint()
				End If
			End Set
		End Property


		''' <summary>
		''' Sets the rollover state of this toolbar. If the rollover state is true
		''' then the border of the toolbar buttons will be drawn only when the
		''' mouse pointer hovers over them. The default value of this property
		''' is false.
		''' <p>
		''' The implementation of a look and feel may choose to ignore this
		''' property.
		''' </summary>
		''' <param name="rollover"> true for rollover toolbar buttons; otherwise false
		''' @since 1.4
		''' @beaninfo
		'''        bound: true
		'''    preferred: true
		'''    attribute: visualUpdate true
		'''  description: Will draw rollover button borders in the toolbar. </param>
		Public Overridable Property rollover As Boolean
			Set(ByVal rollover As Boolean)
				putClientProperty("JToolBar.isRollover",If(rollover, Boolean.TRUE, Boolean.FALSE))
			End Set
			Get
				Dim ___rollover As Boolean? = CBool(getClientProperty("JToolBar.isRollover"))
				If ___rollover IsNot Nothing Then Return ___rollover
				Return False
			End Get
		End Property


		Private Sub checkOrientation(ByVal orientation As Integer)
			Select Case orientation
				Case VERTICAL, HORIZONTAL
				Case Else
					Throw New System.ArgumentException("orientation must be one of: VERTICAL, HORIZONTAL")
			End Select
		End Sub

		''' <summary>
		''' Appends a separator of default size to the end of the tool bar.
		''' The default size is determined by the current look and feel.
		''' </summary>
		Public Overridable Sub addSeparator()
			addSeparator(Nothing)
		End Sub

		''' <summary>
		''' Appends a separator of a specified size to the end
		''' of the tool bar.
		''' </summary>
		''' <param name="size"> the <code>Dimension</code> of the separator </param>
		Public Overridable Sub addSeparator(ByVal size As java.awt.Dimension)
			Dim s As New JToolBar.Separator(size)
			add(s)
		End Sub

		''' <summary>
		''' Adds a new <code>JButton</code> which dispatches the action.
		''' </summary>
		''' <param name="a"> the <code>Action</code> object to add as a new menu item </param>
		''' <returns> the new button which dispatches the action </returns>
		Public Overridable Function add(ByVal a As Action) As JButton
			Dim b As JButton = createActionComponent(a)
			b.action = a
			add(b)
			Return b
		End Function

		''' <summary>
		''' Factory method which creates the <code>JButton</code> for
		''' <code>Action</code>s added to the <code>JToolBar</code>.
		''' The default name is empty if a <code>null</code> action is passed.
		''' </summary>
		''' <param name="a"> the <code>Action</code> for the button to be added </param>
		''' <returns> the newly created button </returns>
		''' <seealso cref= Action
		''' @since 1.3 </seealso>
		Protected Friend Overridable Function createActionComponent(ByVal a As Action) As JButton
			Dim b As JButton = New JButtonAnonymousInnerClassHelper
			If a IsNot Nothing AndAlso (a.getValue(Action.SMALL_ICON) IsNot Nothing OrElse a.getValue(Action.LARGE_ICON_KEY) IsNot Nothing) Then b.hideActionText = True
			b.horizontalTextPosition = JButton.CENTER
			b.verticalTextPosition = JButton.BOTTOM
			Return b
		End Function

		Private Class JButtonAnonymousInnerClassHelper
			Inherits JButton

			Protected Friend Overrides Function createActionPropertyChangeListener(ByVal a As Action) As PropertyChangeListener
				Dim pcl As PropertyChangeListener = outerInstance.createActionChangeListener(Me)
				If pcl Is Nothing Then pcl = MyBase.createActionPropertyChangeListener(a)
				Return pcl
			End Function
		End Class

		''' <summary>
		''' Returns a properly configured <code>PropertyChangeListener</code>
		''' which updates the control as changes to the <code>Action</code> occur,
		''' or <code>null</code> if the default
		''' property change listener for the control is desired.
		''' </summary>
		''' <returns> <code>null</code> </returns>
		Protected Friend Overridable Function createActionChangeListener(ByVal b As JButton) As PropertyChangeListener
			Return Nothing
		End Function

		''' <summary>
		''' If a <code>JButton</code> is being added, it is initially
		''' set to be disabled.
		''' </summary>
		''' <param name="comp">  the component to be enhanced </param>
		''' <param name="constraints">  the constraints to be enforced on the component </param>
		''' <param name="index"> the index of the component
		'''  </param>
		Protected Friend Overridable Sub addImpl(ByVal comp As java.awt.Component, ByVal constraints As Object, ByVal index As Integer)
			If TypeOf comp Is Separator Then
				If orientation = VERTICAL Then
					CType(comp, Separator).orientation = JSeparator.HORIZONTAL
				Else
					CType(comp, Separator).orientation = JSeparator.VERTICAL
				End If
			End If
			MyBase.addImpl(comp, constraints, index)
			If TypeOf comp Is JButton Then CType(comp, JButton).defaultCapable = False
		End Sub


		''' <summary>
		''' A toolbar-specific separator. An object with dimension but
		''' no contents used to divide buttons on a tool bar into groups.
		''' </summary>
		Public Class Separator
			Inherits JSeparator

			Private separatorSize As java.awt.Dimension

			''' <summary>
			''' Creates a new toolbar separator with the default size
			''' as defined by the current look and feel.
			''' </summary>
			Public Sub New()
				Me.New(Nothing) ' let the UI define the default size
			End Sub

			''' <summary>
			''' Creates a new toolbar separator with the specified size.
			''' </summary>
			''' <param name="size"> the <code>Dimension</code> of the separator </param>
			Public Sub New(ByVal size As java.awt.Dimension)
				MyBase.New(JSeparator.HORIZONTAL)
				separatorSize = size
			End Sub

			''' <summary>
			''' Returns the name of the L&amp;F class that renders this component.
			''' </summary>
			''' <returns> the string "ToolBarSeparatorUI" </returns>
			''' <seealso cref= JComponent#getUIClassID </seealso>
			''' <seealso cref= UIDefaults#getUI </seealso>
			Public Property Overrides uIClassID As String
				Get
					Return "ToolBarSeparatorUI"
				End Get
			End Property

			''' <summary>
			''' Sets the size of the separator.
			''' </summary>
			''' <param name="size"> the new <code>Dimension</code> of the separator </param>
			Public Overridable Property separatorSize As java.awt.Dimension
				Set(ByVal size As java.awt.Dimension)
					If size IsNot Nothing Then
						separatorSize = size
					Else
						MyBase.updateUI()
					End If
					Me.invalidate()
				End Set
				Get
					Return separatorSize
				End Get
			End Property


			''' <summary>
			''' Returns the minimum size for the separator.
			''' </summary>
			''' <returns> the <code>Dimension</code> object containing the separator's
			'''         minimum size </returns>
			Public Property Overrides minimumSize As java.awt.Dimension
				Get
					If separatorSize IsNot Nothing Then
						Return separatorSize.size
					Else
						Return MyBase.minimumSize
					End If
				End Get
			End Property

			''' <summary>
			''' Returns the maximum size for the separator.
			''' </summary>
			''' <returns> the <code>Dimension</code> object containing the separator's
			'''         maximum size </returns>
			Public Property Overrides maximumSize As java.awt.Dimension
				Get
					If separatorSize IsNot Nothing Then
						Return separatorSize.size
					Else
						Return MyBase.maximumSize
					End If
				End Get
			End Property

			''' <summary>
			''' Returns the preferred size for the separator.
			''' </summary>
			''' <returns> the <code>Dimension</code> object containing the separator's
			'''         preferred size </returns>
			Public Property Overrides preferredSize As java.awt.Dimension
				Get
					If separatorSize IsNot Nothing Then
						Return separatorSize.size
					Else
						Return MyBase.preferredSize
					End If
				End Get
			End Property
		End Class


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
		''' Returns a string representation of this <code>JToolBar</code>.
		''' This method
		''' is intended to be used only for debugging purposes, and the
		''' content and format of the returned string may vary between
		''' implementations. The returned string may be empty but may not
		''' be <code>null</code>.
		''' </summary>
		''' <returns>  a string representation of this <code>JToolBar</code>. </returns>
		Protected Friend Overrides Function paramString() As String
			Dim paintBorderString As String = (If(___paintBorder, "true", "false"))
			Dim marginString As String = (If(margin IsNot Nothing, margin.ToString(), ""))
			Dim floatableString As String = (If(floatable, "true", "false"))
			Dim orientationString As String = (If(orientation = HORIZONTAL, "HORIZONTAL", "VERTICAL"))

			Return MyBase.paramString() & ",floatable=" & floatableString & ",margin=" & marginString & ",orientation=" & orientationString & ",paintBorder=" & paintBorderString
		End Function


		<Serializable> _
		Private Class DefaultToolBarLayout
			Implements java.awt.LayoutManager2, PropertyChangeListener, UIResource

			Private ReadOnly outerInstance As JToolBar


			Friend lm As BoxLayout

			Friend Sub New(ByVal outerInstance As JToolBar, ByVal orientation As Integer)
					Me.outerInstance = outerInstance
				If orientation = JToolBar.VERTICAL Then
					lm = New BoxLayout(JToolBar.this, BoxLayout.PAGE_AXIS)
				Else
					lm = New BoxLayout(JToolBar.this, BoxLayout.LINE_AXIS)
				End If
			End Sub

			Public Overridable Sub addLayoutComponent(ByVal name As String, ByVal comp As java.awt.Component)
				lm.addLayoutComponent(name, comp)
			End Sub

			Public Overridable Sub addLayoutComponent(ByVal comp As java.awt.Component, ByVal constraints As Object)
				lm.addLayoutComponent(comp, constraints)
			End Sub

			Public Overridable Sub removeLayoutComponent(ByVal comp As java.awt.Component)
				lm.removeLayoutComponent(comp)
			End Sub

			Public Overridable Function preferredLayoutSize(ByVal target As java.awt.Container) As java.awt.Dimension
				Return lm.preferredLayoutSize(target)
			End Function

			Public Overridable Function minimumLayoutSize(ByVal target As java.awt.Container) As java.awt.Dimension
				Return lm.minimumLayoutSize(target)
			End Function

			Public Overridable Function maximumLayoutSize(ByVal target As java.awt.Container) As java.awt.Dimension
				Return lm.maximumLayoutSize(target)
			End Function

			Public Overridable Sub layoutContainer(ByVal target As java.awt.Container)
				lm.layoutContainer(target)
			End Sub

			Public Overridable Function getLayoutAlignmentX(ByVal target As java.awt.Container) As Single
				Return lm.getLayoutAlignmentX(target)
			End Function

			Public Overridable Function getLayoutAlignmentY(ByVal target As java.awt.Container) As Single
				Return lm.getLayoutAlignmentY(target)
			End Function

			Public Overridable Sub invalidateLayout(ByVal target As java.awt.Container)
				lm.invalidateLayout(target)
			End Sub

			Public Overridable Sub propertyChange(ByVal e As PropertyChangeEvent)
				Dim name As String = e.propertyName
				If name.Equals("orientation") Then
					Dim o As Integer = CInt(Fix(e.newValue))

					If o = JToolBar.VERTICAL Then
						lm = New BoxLayout(JToolBar.this, BoxLayout.PAGE_AXIS)
					Else
						lm = New BoxLayout(JToolBar.this, BoxLayout.LINE_AXIS)
					End If
				End If
			End Sub
		End Class


		Public Overridable Property layout As java.awt.LayoutManager
			Set(ByVal mgr As java.awt.LayoutManager)
				Dim oldMgr As java.awt.LayoutManager = layout
				If TypeOf oldMgr Is PropertyChangeListener Then removePropertyChangeListener(CType(oldMgr, PropertyChangeListener))
				MyBase.layout = mgr
			End Set
		End Property

	'///////////////
	' Accessibility support
	'//////////////

		''' <summary>
		''' Gets the AccessibleContext associated with this JToolBar.
		''' For tool bars, the AccessibleContext takes the form of an
		''' AccessibleJToolBar.
		''' A new AccessibleJToolBar instance is created if necessary.
		''' </summary>
		''' <returns> an AccessibleJToolBar that serves as the
		'''         AccessibleContext of this JToolBar </returns>
		Public Overridable Property accessibleContext As AccessibleContext Implements Accessible.getAccessibleContext
			Get
				If accessibleContext Is Nothing Then accessibleContext = New AccessibleJToolBar(Me)
				Return accessibleContext
			End Get
		End Property

		''' <summary>
		''' This class implements accessibility support for the
		''' <code>JToolBar</code> class.  It provides an implementation of the
		''' Java Accessibility API appropriate to toolbar user-interface elements.
		''' </summary>
		Protected Friend Class AccessibleJToolBar
			Inherits AccessibleJComponent

			Private ReadOnly outerInstance As JToolBar

			Public Sub New(ByVal outerInstance As JToolBar)
				Me.outerInstance = outerInstance
			End Sub


			''' <summary>
			''' Get the state of this object.
			''' </summary>
			''' <returns> an instance of AccessibleStateSet containing the current
			''' state set of the object </returns>
			''' <seealso cref= AccessibleState </seealso>
			Public Overridable Property accessibleStateSet As AccessibleStateSet
				Get
					Dim states As AccessibleStateSet = MyBase.accessibleStateSet
					' FIXME:  [[[WDW - need to add orientation from BoxLayout]]]
					' FIXME:  [[[WDW - need to do SELECTABLE if SelectionModel is added]]]
					Return states
				End Get
			End Property

			''' <summary>
			''' Get the role of this object.
			''' </summary>
			''' <returns> an instance of AccessibleRole describing the role of the object </returns>
			Public Overridable Property accessibleRole As AccessibleRole
				Get
					Return AccessibleRole.TOOL_BAR
				End Get
			End Property
		End Class ' inner class AccessibleJToolBar
	End Class

End Namespace
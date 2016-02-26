Imports System
Imports System.Collections.Generic
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
	''' An implementation of a menu bar. You add <code>JMenu</code> objects to the
	''' menu bar to construct a menu. When the user selects a <code>JMenu</code>
	''' object, its associated <code>JPopupMenu</code> is displayed, allowing the
	''' user to select one of the <code>JMenuItems</code> on it.
	''' <p>
	''' For information and examples of using menu bars see
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
	''' <p>
	''' <strong>Warning:</strong>
	''' By default, pressing the Tab key does not transfer focus from a <code>
	''' JMenuBar</code> which is added to a container together with other Swing
	''' components, because the <code>focusTraversalKeysEnabled</code> property
	''' of <code>JMenuBar</code> is set to <code>false</code>. To resolve this,
	''' you should call the <code>JMenuBar.setFocusTraversalKeysEnabled(true)</code>
	''' method.
	''' @beaninfo
	'''   attribute: isContainer true
	''' description: A container for holding and displaying menus.
	''' 
	''' @author Georges Saab
	''' @author David Karlton
	''' @author Arnaud Weber </summary>
	''' <seealso cref= JMenu </seealso>
	''' <seealso cref= JPopupMenu </seealso>
	''' <seealso cref= JMenuItem </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	Public Class JMenuBar
		Inherits JComponent
		Implements Accessible, MenuElement

		''' <seealso cref= #getUIClassID </seealso>
		''' <seealso cref= #readObject </seealso>
		Private Const uiClassID As String = "MenuBarUI"

	'    
	'     * Model for the selected subcontrol.
	'     
		<NonSerialized> _
		Private selectionModel As SingleSelectionModel

		Private ___paintBorder As Boolean = True
		Private margin As java.awt.Insets = Nothing

		' diagnostic aids -- should be false for production builds. 
		Private Const TRACE As Boolean = False ' trace creates and disposes
		Private Const VERBOSE As Boolean = False ' show reuse hits/misses
		Private Const DEBUG As Boolean = False ' show bad params, misc.

		''' <summary>
		''' Creates a new menu bar.
		''' </summary>
		Public Sub New()
			MyBase.New()
			focusTraversalKeysEnabled = False
			selectionModel = New DefaultSingleSelectionModel
			updateUI()
		End Sub

		''' <summary>
		''' Returns the menubar's current UI. </summary>
		''' <seealso cref= #setUI </seealso>
		Public Overridable Property uI As MenuBarUI
			Get
				Return CType(ui, MenuBarUI)
			End Get
			Set(ByVal ui As MenuBarUI)
				MyBase.uI = ui
			End Set
		End Property


		''' <summary>
		''' Resets the UI property with a value from the current look and feel.
		''' </summary>
		''' <seealso cref= JComponent#updateUI </seealso>
		Public Overrides Sub updateUI()
			uI = CType(UIManager.getUI(Me), MenuBarUI)
		End Sub


		''' <summary>
		''' Returns the name of the L&amp;F class that renders this component.
		''' </summary>
		''' <returns> the string "MenuBarUI" </returns>
		''' <seealso cref= JComponent#getUIClassID </seealso>
		''' <seealso cref= UIDefaults#getUI </seealso>
		Public Property Overrides uIClassID As String
			Get
				Return uiClassID
			End Get
		End Property


		''' <summary>
		''' Returns the model object that handles single selections.
		''' </summary>
		''' <returns> the <code>SingleSelectionModel</code> property </returns>
		''' <seealso cref= SingleSelectionModel </seealso>
		Public Overridable Property selectionModel As SingleSelectionModel
			Get
				Return selectionModel
			End Get
			Set(ByVal model As SingleSelectionModel)
				Dim oldValue As SingleSelectionModel = selectionModel
				Me.selectionModel = model
				firePropertyChange("selectionModel", oldValue, selectionModel)
			End Set
		End Property



		''' <summary>
		''' Appends the specified menu to the end of the menu bar.
		''' </summary>
		''' <param name="c"> the <code>JMenu</code> component to add </param>
		''' <returns> the menu component </returns>
		Public Overridable Function add(ByVal c As JMenu) As JMenu
			MyBase.add(c)
			Return c
		End Function

		''' <summary>
		''' Returns the menu at the specified position in the menu bar.
		''' </summary>
		''' <param name="index">  an integer giving the position in the menu bar, where
		'''               0 is the first position </param>
		''' <returns> the <code>JMenu</code> at that position, or <code>null</code> if
		'''          if there is no <code>JMenu</code> at that position (ie. if
		'''          it is a <code>JMenuItem</code>) </returns>
		Public Overridable Function getMenu(ByVal index As Integer) As JMenu
			Dim c As java.awt.Component = getComponentAtIndex(index)
			If TypeOf c Is JMenu Then Return CType(c, JMenu)
			Return Nothing
		End Function

		''' <summary>
		''' Returns the number of items in the menu bar.
		''' </summary>
		''' <returns> the number of items in the menu bar </returns>
		Public Overridable Property menuCount As Integer
			Get
				Return componentCount
			End Get
		End Property

		''' <summary>
		''' Sets the help menu that appears when the user selects the
		''' "help" option in the menu bar. This method is not yet implemented
		''' and will throw an exception.
		''' </summary>
		''' <param name="menu"> the JMenu that delivers help to the user </param>
		Public Overridable Property helpMenu As JMenu
			Set(ByVal menu As JMenu)
				Throw New Exception("setHelpMenu() not yet implemented.")
			End Set
			Get
				Throw New Exception("getHelpMenu() not yet implemented.")
			End Get
		End Property

		''' <summary>
		''' Gets the help menu for the menu bar.  This method is not yet
		''' implemented and will throw an exception.
		''' </summary>
		''' <returns> the <code>JMenu</code> that delivers help to the user </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:

		''' <summary>
		''' Returns the component at the specified index.
		''' </summary>
		''' <param name="i"> an integer specifying the position, where 0 is first </param>
		''' <returns> the <code>Component</code> at the position,
		'''          or <code>null</code> for an invalid index </returns>
		''' @deprecated replaced by <code>getComponent(int i)</code> 
		<Obsolete("replaced by <code>getComponent(int i)</code>")> _
		Public Overridable Function getComponentAtIndex(ByVal i As Integer) As java.awt.Component
			If i < 0 OrElse i >= componentCount Then Return Nothing
			Return getComponent(i)
		End Function

		''' <summary>
		''' Returns the index of the specified component.
		''' </summary>
		''' <param name="c">  the <code>Component</code> to find </param>
		''' <returns> an integer giving the component's position, where 0 is first;
		'''          or -1 if it can't be found </returns>
		Public Overridable Function getComponentIndex(ByVal c As java.awt.Component) As Integer
			Dim ncomponents As Integer = Me.componentCount
			Dim ___component As java.awt.Component() = Me.components
			For i As Integer = 0 To ncomponents - 1
				Dim comp As java.awt.Component = ___component(i)
				If comp Is c Then Return i
			Next i
			Return -1
		End Function

		''' <summary>
		''' Sets the currently selected component, producing a
		''' a change to the selection model.
		''' </summary>
		''' <param name="sel"> the <code>Component</code> to select </param>
		Public Overridable Property selected As java.awt.Component
			Set(ByVal sel As java.awt.Component)
				Dim model As SingleSelectionModel = selectionModel
				Dim index As Integer = getComponentIndex(sel)
				model.selectedIndex = index
			End Set
			Get
				Return selectionModel.selected
			End Get
		End Property


		''' <summary>
		''' Returns true if the menu bars border should be painted.
		''' </summary>
		''' <returns>  true if the border should be painted, else false </returns>
		Public Overridable Property borderPainted As Boolean
			Get
				Return ___paintBorder
			End Get
			Set(ByVal b As Boolean)
				Dim oldValue As Boolean = ___paintBorder
				___paintBorder = b
				firePropertyChange("borderPainted", oldValue, ___paintBorder)
				If b <> oldValue Then
					revalidate()
					repaint()
				End If
			End Set
		End Property


		''' <summary>
		''' Paints the menubar's border if <code>BorderPainted</code>
		''' property is true.
		''' </summary>
		''' <param name="g"> the <code>Graphics</code> context to use for painting </param>
		''' <seealso cref= JComponent#paint </seealso>
		''' <seealso cref= JComponent#setBorder </seealso>
		Protected Friend Overridable Sub paintBorder(ByVal g As java.awt.Graphics)
			If borderPainted Then MyBase.paintBorder(g)
		End Sub

		''' <summary>
		''' Sets the margin between the menubar's border and
		''' its menus. Setting to <code>null</code> will cause the menubar to
		''' use the default margins.
		''' </summary>
		''' <param name="m"> an Insets object containing the margin values </param>
		''' <seealso cref= Insets
		''' @beaninfo
		'''        bound: true
		'''    attribute: visualUpdate true
		'''  description: The space between the menubar's border and its contents </seealso>
		Public Overridable Property margin As java.awt.Insets
			Set(ByVal m As java.awt.Insets)
				Dim old As java.awt.Insets = margin
				Me.margin = m
				firePropertyChange("margin", old, m)
				If old Is Nothing OrElse (Not old.Equals(m)) Then
					revalidate()
					repaint()
				End If
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
		''' Implemented to be a <code>MenuElement</code> -- does nothing.
		''' </summary>
		''' <seealso cref= #getSubElements </seealso>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		public void processMouseEvent(MouseEvent event,MenuElement path() ,MenuSelectionManager manager)

		''' <summary>
		''' Implemented to be a <code>MenuElement</code> -- does nothing.
		''' </summary>
		''' <seealso cref= #getSubElements </seealso>
		public void processKeyEvent(KeyEvent e,MenuElement path() ,MenuSelectionManager manager)

		''' <summary>
		''' Implemented to be a <code>MenuElement</code> -- does nothing.
		''' </summary>
		''' <seealso cref= #getSubElements </seealso>
		public void menuSelectionChanged(Boolean isIncluded)

		''' <summary>
		''' Implemented to be a <code>MenuElement</code> -- returns the
		''' menus in this menu bar.
		''' This is the reason for implementing the <code>MenuElement</code>
		''' interface -- so that the menu bar can be treated the same as
		''' other menu elements. </summary>
		''' <returns> an array of menu items in the menu bar. </returns>
		public MenuElement() subElements
			Dim result As MenuElement()
			Dim tmp As New List(Of MenuElement)
			Dim c As Integer = componentCount
			Dim i As Integer
			Dim m As java.awt.Component

			For i = 0 To c - 1
				m = getComponent(i)
				If TypeOf m Is MenuElement Then tmp.Add(CType(m, MenuElement))
			Next i

			result = New MenuElement(tmp.Count - 1){}
			i=0
			c=tmp.Count
			Do While i < c
				result(i) = tmp(i)
				i += 1
			Loop
			Return result

		''' <summary>
		''' Implemented to be a <code>MenuElement</code>. Returns this object.
		''' </summary>
		''' <returns> the current <code>Component</code> (this) </returns>
		''' <seealso cref= #getSubElements </seealso>
		public java.awt.Component component
			Return Me


		''' <summary>
		''' Returns a string representation of this <code>JMenuBar</code>.
		''' This method
		''' is intended to be used only for debugging purposes, and the
		''' content and format of the returned string may vary between
		''' implementations. The returned string may be empty but may not
		''' be <code>null</code>.
		''' </summary>
		''' <returns>  a string representation of this <code>JMenuBar</code> </returns>
		protected String paramString()
			Dim paintBorderString As String = (If(___paintBorder, "true", "false"))
			Dim marginString As String = (If(margin IsNot Nothing, margin.ToString(), ""))

			Return MyBase.paramString() & ",margin=" & marginString & ",paintBorder=" & paintBorderString

	'///////////////
	' Accessibility support
	'//////////////

		''' <summary>
		''' Gets the AccessibleContext associated with this JMenuBar.
		''' For JMenuBars, the AccessibleContext takes the form of an
		''' AccessibleJMenuBar.
		''' A new AccessibleJMenuBar instance is created if necessary.
		''' </summary>
		''' <returns> an AccessibleJMenuBar that serves as the
		'''         AccessibleContext of this JMenuBar </returns>
		public AccessibleContext accessibleContext
			If accessibleContext Is Nothing Then accessibleContext = New AccessibleJMenuBar(Me)
			Return accessibleContext

		''' <summary>
		''' This class implements accessibility support for the
		''' <code>JMenuBar</code> class.  It provides an implementation of the
		''' Java Accessibility API appropriate to menu bar user-interface
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
		protected class AccessibleJMenuBar extends AccessibleJComponent implements AccessibleSelection

			''' <summary>
			''' Get the accessible state set of this object.
			''' </summary>
			''' <returns> an instance of AccessibleState containing the current state
			'''         of the object </returns>
			public AccessibleStateSet accessibleStateSet
				Dim states As AccessibleStateSet = MyBase.accessibleStateSet
				Return states

			''' <summary>
			''' Get the role of this object.
			''' </summary>
			''' <returns> an instance of AccessibleRole describing the role of the
			''' object </returns>
			public AccessibleRole accessibleRole
				Return AccessibleRole.MENU_BAR

			''' <summary>
			''' Get the AccessibleSelection associated with this object.  In the
			''' implementation of the Java Accessibility API for this class,
			''' return this object, which is responsible for implementing the
			''' AccessibleSelection interface on behalf of itself.
			''' </summary>
			''' <returns> this object </returns>
			public AccessibleSelection accessibleSelection
				Return Me

			''' <summary>
			''' Returns 1 if a menu is currently selected in this menu bar.
			''' </summary>
			''' <returns> 1 if a menu is currently selected, else 0 </returns>
			 public Integer accessibleSelectionCount
				If selected Then
					Return 1
				Else
					Return 0
				End If

			''' <summary>
			''' Returns the currently selected menu if one is selected,
			''' otherwise null.
			''' </summary>
			 public Accessible getAccessibleSelection(Integer i)
				If selected Then
					If i <> 0 Then ' single selection model for JMenuBar Return Nothing
					Dim j As Integer = selectionModel.selectedIndex
					If TypeOf getComponentAtIndex(j) Is Accessible Then Return CType(getComponentAtIndex(j), Accessible)
				End If
				Return Nothing

			''' <summary>
			''' Returns true if the current child of this object is selected.
			''' </summary>
			''' <param name="i"> the zero-based index of the child in this Accessible
			''' object. </param>
			''' <seealso cref= AccessibleContext#getAccessibleChild </seealso>
			public Boolean isAccessibleChildSelected(Integer i)
				Return (i = selectionModel.selectedIndex)

			''' <summary>
			''' Selects the nth menu in the menu bar, forcing it to
			''' pop up.  If another menu is popped up, this will force
			''' it to close.  If the nth menu is already selected, this
			''' method has no effect.
			''' </summary>
			''' <param name="i"> the zero-based index of selectable items </param>
			''' <seealso cref= #getAccessibleStateSet </seealso>
			public void addAccessibleSelection(Integer i)
				' first close up any open menu
				Dim j As Integer = selectionModel.selectedIndex
				If i = j Then Return
				If j >= 0 AndAlso j < menuCount Then
					Dim ___menu As JMenu = getMenu(j)
					If ___menu IsNot Nothing Then MenuSelectionManager.defaultManager().selectedPath = Nothing
				End If
				' now popup the new menu
				selectionModel.selectedIndex = i
				Dim ___menu As JMenu = getMenu(i)
				If ___menu IsNot Nothing Then
					Dim [me] As MenuElement() = New MenuElement(2){}
					[me](0) = JMenuBar.this
					[me](1) = ___menu
					[me](2) = ___menu.popupMenu
					MenuSelectionManager.defaultManager().selectedPath = [me]
	'              menu.setPopupMenuVisible(true);
				End If

			''' <summary>
			''' Removes the nth selected item in the object from the object's
			''' selection.  If the nth item isn't currently selected, this
			''' method has no effect.  Otherwise, it closes the popup menu.
			''' </summary>
			''' <param name="i"> the zero-based index of selectable items </param>
			public void removeAccessibleSelection(Integer i)
				If i >= 0 AndAlso i < menuCount Then
					Dim ___menu As JMenu = getMenu(i)
					If ___menu IsNot Nothing Then MenuSelectionManager.defaultManager().selectedPath = Nothing
					selectionModel.selectedIndex = -1
				End If

			''' <summary>
			''' Clears the selection in the object, so that nothing in the
			''' object is selected.  This will close any open menu.
			''' </summary>
			public void clearAccessibleSelection()
				Dim i As Integer = selectionModel.selectedIndex
				If i >= 0 AndAlso i < menuCount Then
					Dim ___menu As JMenu = getMenu(i)
					If ___menu IsNot Nothing Then MenuSelectionManager.defaultManager().selectedPath = Nothing
				End If
				selectionModel.selectedIndex = -1

			''' <summary>
			''' Normally causes every selected item in the object to be selected
			''' if the object supports multiple selections.  This method
			''' makes no sense in a menu bar, and so does nothing.
			''' </summary>
			public void selectAllAccessibleSelection() ' internal class AccessibleJMenuBar


		''' <summary>
		''' Subclassed to check all the child menus.
		''' @since 1.3
		''' </summary>
		protected Boolean processKeyBinding(KeyStroke ks, KeyEvent e, Integer condition, Boolean pressed)
			' See if we have a local binding.
			Dim retValue As Boolean = MyBase.processKeyBinding(ks, e, condition, pressed)
			If Not retValue Then
				Dim ___subElements As MenuElement() = subElements
				For Each subElement As MenuElement In ___subElements
					If processBindingForKeyStrokeRecursive(subElement, ks, e, condition, pressed) Then Return True
				Next subElement
			End If
			Return retValue

		static Boolean processBindingForKeyStrokeRecursive(MenuElement elem, KeyStroke ks, KeyEvent e, Integer condition, Boolean pressed)
			If elem Is Nothing Then Return False

			Dim c As java.awt.Component = elem.component

			If Not(c.visible OrElse (TypeOf c Is JPopupMenu)) OrElse (Not c.enabled) Then Return False

			If c IsNot Nothing AndAlso TypeOf c Is JComponent AndAlso CType(c, JComponent).processKeyBinding(ks, e, condition, pressed) Then Return True

			Dim ___subElements As MenuElement() = elem.subElements
			For Each subElement As MenuElement In ___subElements
				If processBindingForKeyStrokeRecursive(subElement, ks, e, condition, pressed) Then Return True
			Next subElement
			Return False

		''' <summary>
		''' Overrides <code>JComponent.addNotify</code> to register this
		''' menu bar with the current keyboard manager.
		''' </summary>
		public void addNotify()
			MyBase.addNotify()
			KeyboardManager.currentManager.registerMenuBar(Me)

		''' <summary>
		''' Overrides <code>JComponent.removeNotify</code> to unregister this
		''' menu bar with the current keyboard manager.
		''' </summary>
		public void removeNotify()
			MyBase.removeNotify()
			KeyboardManager.currentManager.unregisterMenuBar(Me)


		private void writeObject(java.io.ObjectOutputStream s) throws java.io.IOException
			s.defaultWriteObject()
			If uIClassID.Equals(uiClassID) Then
				Dim count As SByte = JComponent.getWriteObjCounter(Me)
				count -= 1
				JComponent.writeObjCounterter(Me, count)
				If count = 0 AndAlso ui IsNot Nothing Then ui.installUI(Me)
			End If

			Dim kvData As Object() = New Object(3){}
			Dim n As Integer = 0

			If TypeOf selectionModel Is java.io.Serializable Then
				kvData(n) = "selectionModel"
				n += 1
				kvData(n) = selectionModel
				n += 1
			End If

			s.writeObject(kvData)


		''' <summary>
		''' See JComponent.readObject() for information about serialization
		''' in Swing.
		''' </summary>
		private void readObject(java.io.ObjectInputStream s) throws java.io.IOException, ClassNotFoundException
			s.defaultReadObject()
			Dim kvData As Object() = CType(s.readObject(), Object())

			For i As Integer = 0 To kvData.Length - 1 Step 2
				If kvData(i) Is Nothing Then
					Exit For
				ElseIf kvData(i).Equals("selectionModel") Then
					selectionModel = CType(kvData(i + 1), SingleSelectionModel)
				End If
			Next i

	End Class

End Namespace
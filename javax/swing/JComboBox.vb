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
	''' A component that combines a button or editable field and a drop-down list.
	''' The user can select a value from the drop-down list, which appears at the
	''' user's request. If you make the combo box editable, then the combo box
	''' includes an editable field into which the user can type a value.
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
	''' <p>
	''' See <a href="https://docs.oracle.com/javase/tutorial/uiswing/components/combobox.html">How to Use Combo Boxes</a>
	''' in <a href="https://docs.oracle.com/javase/tutorial/"><em>The Java Tutorial</em></a>
	''' for further information.
	''' <p> </summary>
	''' <seealso cref= ComboBoxModel </seealso>
	''' <seealso cref= DefaultComboBoxModel
	''' </seealso>
	''' @param <E> the type of the elements of this combo box
	''' 
	''' @beaninfo
	'''   attribute: isContainer false
	''' description: A combination of a text field and a drop-down list.
	''' 
	''' @author Arnaud Weber
	''' @author Mark Davidson </param>
	Public Class JComboBox(Of E)
		Inherits JComponent
		Implements ItemSelectable, ListDataListener, ActionListener, Accessible

		''' <seealso cref= #getUIClassID </seealso>
		''' <seealso cref= #readObject </seealso>
		Private Const uiClassID As String = "ComboBoxUI"

		''' <summary>
		''' This protected field is implementation specific. Do not access directly
		''' or override. Use the accessor methods instead.
		''' </summary>
		''' <seealso cref= #getModel </seealso>
		''' <seealso cref= #setModel </seealso>
		Protected Friend dataModel As ComboBoxModel(Of E)
		''' <summary>
		''' This protected field is implementation specific. Do not access directly
		''' or override. Use the accessor methods instead.
		''' </summary>
		''' <seealso cref= #getRenderer </seealso>
		''' <seealso cref= #setRenderer </seealso>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Protected Friend renderer As ListCellRenderer(Of ?)
		''' <summary>
		''' This protected field is implementation specific. Do not access directly
		''' or override. Use the accessor methods instead.
		''' </summary>
		''' <seealso cref= #getEditor </seealso>
		''' <seealso cref= #setEditor </seealso>
		Protected Friend editor As ComboBoxEditor
		''' <summary>
		''' This protected field is implementation specific. Do not access directly
		''' or override. Use the accessor methods instead.
		''' </summary>
		''' <seealso cref= #getMaximumRowCount </seealso>
		''' <seealso cref= #setMaximumRowCount </seealso>
		Protected Friend maximumRowCount As Integer = 8

		''' <summary>
		''' This protected field is implementation specific. Do not access directly
		''' or override. Use the accessor methods instead.
		''' </summary>
		''' <seealso cref= #isEditable </seealso>
		''' <seealso cref= #setEditable </seealso>
		Protected Friend ___isEditable As Boolean = False
		''' <summary>
		''' This protected field is implementation specific. Do not access directly
		''' or override. Use the accessor methods instead.
		''' </summary>
		''' <seealso cref= #setKeySelectionManager </seealso>
		''' <seealso cref= #getKeySelectionManager </seealso>
		Protected Friend keySelectionManager As KeySelectionManager = Nothing
		''' <summary>
		''' This protected field is implementation specific. Do not access directly
		''' or override. Use the accessor methods instead.
		''' </summary>
		''' <seealso cref= #setActionCommand </seealso>
		''' <seealso cref= #getActionCommand </seealso>
		Protected Friend actionCommand As String = "comboBoxChanged"
		''' <summary>
		''' This protected field is implementation specific. Do not access directly
		''' or override. Use the accessor methods instead.
		''' </summary>
		''' <seealso cref= #setLightWeightPopupEnabled </seealso>
		''' <seealso cref= #isLightWeightPopupEnabled </seealso>
		Protected Friend lightWeightPopupEnabled As Boolean = JPopupMenu.defaultLightWeightPopupEnabled

		''' <summary>
		''' This protected field is implementation specific. Do not access directly
		''' or override.
		''' </summary>
		Protected Friend selectedItemReminder As Object = Nothing

		Private prototypeDisplayValue As E

		' Flag to ensure that infinite loops do not occur with ActionEvents.
		Private firingActionEvent As Boolean = False

		' Flag to ensure the we don't get multiple ActionEvents on item selection.
		Private selectingItem As Boolean = False

		''' <summary>
		''' Creates a <code>JComboBox</code> that takes its items from an
		''' existing <code>ComboBoxModel</code>.  Since the
		''' <code>ComboBoxModel</code> is provided, a combo box created using
		''' this constructor does not create a default combo box model and
		''' may impact how the insert, remove and add methods behave.
		''' </summary>
		''' <param name="aModel"> the <code>ComboBoxModel</code> that provides the
		'''          displayed list of items </param>
		''' <seealso cref= DefaultComboBoxModel </seealso>
		Public Sub New(ByVal aModel As ComboBoxModel(Of E))
			MyBase.New()
			model = aModel
			init()
		End Sub

		''' <summary>
		''' Creates a <code>JComboBox</code> that contains the elements
		''' in the specified array.  By default the first item in the array
		''' (and therefore the data model) becomes selected.
		''' </summary>
		''' <param name="items">  an array of objects to insert into the combo box </param>
		''' <seealso cref= DefaultComboBoxModel </seealso>
		Public Sub New(ByVal items As E())
			MyBase.New()
			model = New DefaultComboBoxModel(Of E)(items)
			init()
		End Sub

		''' <summary>
		''' Creates a <code>JComboBox</code> that contains the elements
		''' in the specified Vector.  By default the first item in the vector
		''' (and therefore the data model) becomes selected.
		''' </summary>
		''' <param name="items">  an array of vectors to insert into the combo box </param>
		''' <seealso cref= DefaultComboBoxModel </seealso>
		Public Sub New(ByVal items As List(Of E))
			MyBase.New()
			model = New DefaultComboBoxModel(Of E)(items)
			init()
		End Sub

		''' <summary>
		''' Creates a <code>JComboBox</code> with a default data model.
		''' The default data model is an empty list of objects.
		''' Use <code>addItem</code> to add items.  By default the first item
		''' in the data model becomes selected.
		''' </summary>
		''' <seealso cref= DefaultComboBoxModel </seealso>
		Public Sub New()
			MyBase.New()
			model = New DefaultComboBoxModel(Of E)
			init()
		End Sub

		Private Sub init()
			installAncestorListener()
			uIPropertyrty("opaque",True)
			updateUI()
		End Sub

		Protected Friend Overridable Sub installAncestorListener()
			addAncestorListener(New AncestorListenerAnonymousInnerClassHelper
		End Sub

		Private Class AncestorListenerAnonymousInnerClassHelper
			Implements AncestorListener

			Public Overridable Sub ancestorAdded(ByVal [event] As AncestorEvent) Implements AncestorListener.ancestorAdded
				outerInstance.hidePopup()
			End Sub
			Public Overridable Sub ancestorRemoved(ByVal [event] As AncestorEvent) Implements AncestorListener.ancestorRemoved
				outerInstance.hidePopup()
			End Sub
			Public Overridable Sub ancestorMoved(ByVal [event] As AncestorEvent) Implements AncestorListener.ancestorMoved
				If [event].source <> JComboBox.this Then outerInstance.hidePopup()
			End Sub
		End Class

		''' <summary>
		''' Sets the L&amp;F object that renders this component.
		''' </summary>
		''' <param name="ui">  the <code>ComboBoxUI</code> L&amp;F object </param>
		''' <seealso cref= UIDefaults#getUI
		''' 
		''' @beaninfo
		'''        bound: true
		'''       hidden: true
		'''    attribute: visualUpdate true
		'''  description: The UI object that implements the Component's LookAndFeel. </seealso>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Public Overridable Sub setUI(ByVal ui As ComboBoxUI) 'JavaToDotNetTempPropertySetuI
		Public Overridable Property uI As ComboBoxUI
			Set(ByVal ui As ComboBoxUI)
				MyBase.uI = ui
			End Set
			Get
		End Property

		''' <summary>
		''' Resets the UI property to a value from the current look and feel.
		''' </summary>
		''' <seealso cref= JComponent#updateUI </seealso>
		Public Overrides Sub updateUI()
			uI = CType(UIManager.getUI(Me), ComboBoxUI)

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim ___renderer As ListCellRenderer(Of ?) = renderer
			If TypeOf ___renderer Is Component Then SwingUtilities.updateComponentTreeUI(CType(___renderer, Component))
		End Sub


		''' <summary>
		''' Returns the name of the L&amp;F class that renders this component.
		''' </summary>
		''' <returns> the string "ComboBoxUI" </returns>
		''' <seealso cref= JComponent#getUIClassID </seealso>
		''' <seealso cref= UIDefaults#getUI </seealso>
		Public Property Overrides uIClassID As String
			Get
				Return uiClassID
			End Get
		End Property


			Return CType(ui, ComboBoxUI)
		End Function

		''' <summary>
		''' Sets the data model that the <code>JComboBox</code> uses to obtain
		''' the list of items.
		''' </summary>
		''' <param name="aModel"> the <code>ComboBoxModel</code> that provides the
		'''  displayed list of items
		''' 
		''' @beaninfo
		'''        bound: true
		'''  description: Model that the combo box uses to get data to display. </param>
		Public Overridable Property model As ComboBoxModel(Of E)
			Set(ByVal aModel As ComboBoxModel(Of E))
				Dim oldModel As ComboBoxModel(Of E) = dataModel
				If oldModel IsNot Nothing Then oldModel.removeListDataListener(Me)
				dataModel = aModel
				dataModel.addListDataListener(Me)
    
				' set the current selected item.
				selectedItemReminder = dataModel.selectedItem
    
				firePropertyChange("model", oldModel, dataModel)
			End Set
			Get
				Return dataModel
			End Get
		End Property


	'    
	'     * Properties
	'     

		''' <summary>
		''' Sets the <code>lightWeightPopupEnabled</code> property, which
		''' provides a hint as to whether or not a lightweight
		''' <code>Component</code> should be used to contain the
		''' <code>JComboBox</code>, versus a heavyweight
		''' <code>Component</code> such as a <code>Panel</code>
		''' or a <code>Window</code>.  The decision of lightweight
		''' versus heavyweight is ultimately up to the
		''' <code>JComboBox</code>.  Lightweight windows are more
		''' efficient than heavyweight windows, but lightweight
		''' and heavyweight components do not mix well in a GUI.
		''' If your application mixes lightweight and heavyweight
		''' components, you should disable lightweight popups.
		''' The default value for the <code>lightWeightPopupEnabled</code>
		''' property is <code>true</code>, unless otherwise specified
		''' by the look and feel.  Some look and feels always use
		''' heavyweight popups, no matter what the value of this property.
		''' <p>
		''' See the article <a href="http://www.oracle.com/technetwork/articles/java/mixing-components-433992.html">Mixing Heavy and Light Components</a>
		''' This method fires a property changed event.
		''' </summary>
		''' <param name="aFlag"> if <code>true</code>, lightweight popups are desired
		''' 
		''' @beaninfo
		'''        bound: true
		'''       expert: true
		'''  description: Set to <code>false</code> to require heavyweight popups. </param>
		Public Overridable Property lightWeightPopupEnabled As Boolean
			Set(ByVal aFlag As Boolean)
				Dim oldFlag As Boolean = lightWeightPopupEnabled
				lightWeightPopupEnabled = aFlag
				firePropertyChange("lightWeightPopupEnabled", oldFlag, lightWeightPopupEnabled)
			End Set
			Get
				Return lightWeightPopupEnabled
			End Get
		End Property


		''' <summary>
		''' Determines whether the <code>JComboBox</code> field is editable.
		''' An editable <code>JComboBox</code> allows the user to type into the
		''' field or selected an item from the list to initialize the field,
		''' after which it can be edited. (The editing affects only the field,
		''' the list item remains intact.) A non editable <code>JComboBox</code>
		''' displays the selected item in the field,
		''' but the selection cannot be modified.
		''' </summary>
		''' <param name="aFlag"> a boolean value, where true indicates that the
		'''                  field is editable
		''' 
		''' @beaninfo
		'''        bound: true
		'''    preferred: true
		'''  description: If true, the user can type a new value in the combo box. </param>
		Public Overridable Property editable As Boolean
			Set(ByVal aFlag As Boolean)
				Dim oldFlag As Boolean = ___isEditable
				___isEditable = aFlag
				firePropertyChange("editable", oldFlag, ___isEditable)
			End Set
			Get
				Return ___isEditable
			End Get
		End Property


		''' <summary>
		''' Sets the maximum number of rows the <code>JComboBox</code> displays.
		''' If the number of objects in the model is greater than count,
		''' the combo box uses a scrollbar.
		''' </summary>
		''' <param name="count"> an integer specifying the maximum number of items to
		'''              display in the list before using a scrollbar
		''' @beaninfo
		'''        bound: true
		'''    preferred: true
		'''  description: The maximum number of rows the popup should have </param>
		Public Overridable Property maximumRowCount As Integer
			Set(ByVal count As Integer)
				Dim oldCount As Integer = maximumRowCount
				maximumRowCount = count
				firePropertyChange("maximumRowCount", oldCount, maximumRowCount)
			End Set
			Get
				Return maximumRowCount
			End Get
		End Property


		''' <summary>
		''' Sets the renderer that paints the list items and the item selected from the list in
		''' the JComboBox field. The renderer is used if the JComboBox is not
		''' editable. If it is editable, the editor is used to render and edit
		''' the selected item.
		''' <p>
		''' The default renderer displays a string or an icon.
		''' Other renderers can handle graphic images and composite items.
		''' <p>
		''' To display the selected item,
		''' <code>aRenderer.getListCellRendererComponent</code>
		''' is called, passing the list object and an index of -1.
		''' </summary>
		''' <param name="aRenderer">  the <code>ListCellRenderer</code> that
		'''                  displays the selected item </param>
		''' <seealso cref= #setEditor
		''' @beaninfo
		'''      bound: true
		'''     expert: true
		'''  description: The renderer that paints the item selected in the list. </seealso>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
		Public Overridable Property renderer(Of T1) As ListCellRenderer(Of T1)
			Set(ByVal aRenderer As ListCellRenderer(Of T1))
	'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
	'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim oldRenderer As ListCellRenderer(Of ?) = renderer
				renderer = aRenderer
				firePropertyChange("renderer", oldRenderer, renderer)
				invalidate()
			End Set
			Get
				Return renderer
			End Get
		End Property

		''' <summary>
		''' Returns the renderer used to display the selected item in the
		''' <code>JComboBox</code> field.
		''' </summary>
		''' <returns>  the <code>ListCellRenderer</code> that displays
		'''                  the selected item. </returns>
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:

		''' <summary>
		''' Sets the editor used to paint and edit the selected item in the
		''' <code>JComboBox</code> field.  The editor is used only if the
		''' receiving <code>JComboBox</code> is editable. If not editable,
		''' the combo box uses the renderer to paint the selected item.
		''' </summary>
		''' <param name="anEditor">  the <code>ComboBoxEditor</code> that
		'''                  displays the selected item </param>
		''' <seealso cref= #setRenderer
		''' @beaninfo
		'''     bound: true
		'''    expert: true
		'''  description: The editor that combo box uses to edit the current value </seealso>
		Public Overridable Property editor As ComboBoxEditor
			Set(ByVal anEditor As ComboBoxEditor)
				Dim oldEditor As ComboBoxEditor = editor
    
				If editor IsNot Nothing Then editor.removeActionListener(Me)
				editor = anEditor
				If editor IsNot Nothing Then editor.addActionListener(Me)
				firePropertyChange("editor", oldEditor, editor)
			End Set
			Get
				Return editor
			End Get
		End Property


		'
		' Selection
		'

		''' <summary>
		''' Sets the selected item in the combo box display area to the object in
		''' the argument.
		''' If <code>anObject</code> is in the list, the display area shows
		''' <code>anObject</code> selected.
		''' <p>
		''' If <code>anObject</code> is <i>not</i> in the list and the combo box is
		''' uneditable, it will not change the current selection. For editable
		''' combo boxes, the selection will change to <code>anObject</code>.
		''' <p>
		''' If this constitutes a change in the selected item,
		''' <code>ItemListener</code>s added to the combo box will be notified with
		''' one or two <code>ItemEvent</code>s.
		''' If there is a current selected item, an <code>ItemEvent</code> will be
		''' fired and the state change will be <code>ItemEvent.DESELECTED</code>.
		''' If <code>anObject</code> is in the list and is not currently selected
		''' then an <code>ItemEvent</code> will be fired and the state change will
		''' be <code>ItemEvent.SELECTED</code>.
		''' <p>
		''' <code>ActionListener</code>s added to the combo box will be notified
		''' with an <code>ActionEvent</code> when this method is called.
		''' </summary>
		''' <param name="anObject">  the list object to select; use <code>null</code> to
		'''                    clear the selection
		''' @beaninfo
		'''    preferred:   true
		'''    description: Sets the selected item in the JComboBox. </param>
		Public Overridable Property selectedItem As Object
			Set(ByVal anObject As Object)
				Dim oldSelection As Object = selectedItemReminder
				Dim objectToSelect As Object = anObject
				If oldSelection Is Nothing OrElse (Not oldSelection.Equals(anObject)) Then
    
					If anObject IsNot Nothing AndAlso (Not editable) Then
						' For non editable combo boxes, an invalid selection
						' will be rejected.
						Dim found As Boolean = False
						For i As Integer = 0 To dataModel.size - 1
							Dim element As E = dataModel.getElementAt(i)
							If anObject.Equals(element) Then
								found = True
								objectToSelect = element
								Exit For
							End If
						Next i
						If Not found Then Return
					End If
    
					' Must toggle the state of this flag since this method
					' call may result in ListDataEvents being fired.
					selectingItem = True
					dataModel.selectedItem = objectToSelect
					selectingItem = False
    
					If selectedItemReminder IsNot dataModel.selectedItem Then selectedItemChanged()
				End If
				fireActionEvent()
			End Set
			Get
				Return dataModel.selectedItem
			End Get
		End Property


		''' <summary>
		''' Selects the item at index <code>anIndex</code>.
		''' </summary>
		''' <param name="anIndex"> an integer specifying the list item to select,
		'''                  where 0 specifies the first item in the list and -1 indicates no selection </param>
		''' <exception cref="IllegalArgumentException"> if <code>anIndex</code> &lt; -1 or
		'''                  <code>anIndex</code> is greater than or equal to size
		''' @beaninfo
		'''   preferred: true
		'''  description: The item at index is selected. </exception>
		Public Overridable Property selectedIndex As Integer
			Set(ByVal anIndex As Integer)
				Dim ___size As Integer = dataModel.size
    
				If anIndex = -1 Then
					selectedItem = Nothing
				ElseIf anIndex < -1 OrElse anIndex >= ___size Then
					Throw New System.ArgumentException("setSelectedIndex: " & anIndex & " out of bounds")
				Else
					selectedItem = dataModel.getElementAt(anIndex)
				End If
			End Set
			Get
				Dim sObject As Object = dataModel.selectedItem
				Dim i, c As Integer
				Dim obj As E
    
				i=0
				c=dataModel.size
				Do While i<c
					obj = dataModel.getElementAt(i)
					If obj IsNot Nothing AndAlso obj.Equals(sObject) Then Return i
					i += 1
				Loop
				Return -1
			End Get
		End Property

		''' <summary>
		''' Returns the first item in the list that matches the given item.
		''' The result is not always defined if the <code>JComboBox</code>
		''' allows selected items that are not in the list.
		''' Returns -1 if there is no selected item or if the user specified
		''' an item which is not in the list.
		''' </summary>
		''' <returns> an integer specifying the currently selected list item,
		'''                  where 0 specifies
		'''                  the first item in the list;
		'''                  or -1 if no item is selected or if
		'''                  the currently selected item is not in the list </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:

		''' <summary>
		''' Returns the "prototypical display" value - an Object used
		''' for the calculation of the display height and width.
		''' </summary>
		''' <returns> the value of the <code>prototypeDisplayValue</code> property </returns>
		''' <seealso cref= #setPrototypeDisplayValue
		''' @since 1.4 </seealso>
		Public Overridable Property prototypeDisplayValue As E
			Get
				Return prototypeDisplayValue
			End Get
			Set(ByVal prototypeDisplayValue As E)
				Dim oldValue As Object = Me.prototypeDisplayValue
				Me.prototypeDisplayValue = prototypeDisplayValue
				firePropertyChange("prototypeDisplayValue", oldValue, prototypeDisplayValue)
			End Set
		End Property


		''' <summary>
		''' Adds an item to the item list.
		''' This method works only if the <code>JComboBox</code> uses a
		''' mutable data model.
		''' <p>
		''' <strong>Warning:</strong>
		''' Focus and keyboard navigation problems may arise if you add duplicate
		''' String objects. A workaround is to add new objects instead of String
		''' objects and make sure that the toString() method is defined.
		''' For example:
		''' <pre>
		'''   comboBox.addItem(makeObj("Item 1"));
		'''   comboBox.addItem(makeObj("Item 1"));
		'''   ...
		'''   private Object makeObj(final String item)  {
		'''     return new Object() { public String toString() { return item; } };
		'''   }
		''' </pre>
		''' </summary>
		''' <param name="item"> the item to add to the list </param>
		''' <seealso cref= MutableComboBoxModel </seealso>
		Public Overridable Sub addItem(ByVal item As E)
			checkMutableComboBoxModel()
			CType(dataModel, MutableComboBoxModel(Of E)).addElement(item)
		End Sub

		''' <summary>
		''' Inserts an item into the item list at a given index.
		''' This method works only if the <code>JComboBox</code> uses a
		''' mutable data model.
		''' </summary>
		''' <param name="item"> the item to add to the list </param>
		''' <param name="index">    an integer specifying the position at which
		'''                  to add the item </param>
		''' <seealso cref= MutableComboBoxModel </seealso>
		Public Overridable Sub insertItemAt(ByVal item As E, ByVal index As Integer)
			checkMutableComboBoxModel()
			CType(dataModel, MutableComboBoxModel(Of E)).insertElementAt(item,index)
		End Sub

		''' <summary>
		''' Removes an item from the item list.
		''' This method works only if the <code>JComboBox</code> uses a
		''' mutable data model.
		''' </summary>
		''' <param name="anObject">  the object to remove from the item list </param>
		''' <seealso cref= MutableComboBoxModel </seealso>
		Public Overridable Sub removeItem(ByVal anObject As Object)
			checkMutableComboBoxModel()
			CType(dataModel, MutableComboBoxModel).removeElement(anObject)
		End Sub

		''' <summary>
		''' Removes the item at <code>anIndex</code>
		''' This method works only if the <code>JComboBox</code> uses a
		''' mutable data model.
		''' </summary>
		''' <param name="anIndex">  an int specifying the index of the item to remove,
		'''                  where 0
		'''                  indicates the first item in the list </param>
		''' <seealso cref= MutableComboBoxModel </seealso>
		Public Overridable Sub removeItemAt(ByVal anIndex As Integer)
			checkMutableComboBoxModel()
			CType(dataModel, MutableComboBoxModel(Of E)).removeElementAt(anIndex)
		End Sub

		''' <summary>
		''' Removes all items from the item list.
		''' </summary>
		Public Overridable Sub removeAllItems()
			checkMutableComboBoxModel()
			Dim ___model As MutableComboBoxModel(Of E) = CType(dataModel, MutableComboBoxModel(Of E))
			Dim ___size As Integer = ___model.size

			If TypeOf ___model Is DefaultComboBoxModel Then
				CType(___model, DefaultComboBoxModel).removeAllElements()
			Else
				For i As Integer = 0 To ___size - 1
					Dim element As E = ___model.getElementAt(0)
					___model.removeElement(element)
				Next i
			End If
			selectedItemReminder = Nothing
			If editable Then editor.item = Nothing
		End Sub

		''' <summary>
		''' Checks that the <code>dataModel</code> is an instance of
		''' <code>MutableComboBoxModel</code>.  If not, it throws an exception. </summary>
		''' <exception cref="RuntimeException"> if <code>dataModel</code> is not an
		'''          instance of <code>MutableComboBoxModel</code>. </exception>
		Friend Overridable Sub checkMutableComboBoxModel()
			If Not(TypeOf dataModel Is MutableComboBoxModel) Then Throw New Exception("Cannot use this method with a non-Mutable data model.")
		End Sub

		''' <summary>
		''' Causes the combo box to display its popup window. </summary>
		''' <seealso cref= #setPopupVisible </seealso>
		Public Overridable Sub showPopup()
			popupVisible = True
		End Sub

		''' <summary>
		''' Causes the combo box to close its popup window. </summary>
		''' <seealso cref= #setPopupVisible </seealso>
		Public Overridable Sub hidePopup()
			popupVisible = False
		End Sub

		''' <summary>
		''' Sets the visibility of the popup.
		''' </summary>
		Public Overridable Property popupVisible As Boolean
			Set(ByVal v As Boolean)
				uI.popupVisibleble(Me, v)
			End Set
			Get
				Return uI.isPopupVisible(Me)
			End Get
		End Property


		''' <summary>
		''' Selection * </summary>

		''' <summary>
		''' Adds an <code>ItemListener</code>.
		''' <p>
		''' <code>aListener</code> will receive one or two <code>ItemEvent</code>s when
		''' the selected item changes.
		''' </summary>
		''' <param name="aListener"> the <code>ItemListener</code> that is to be notified </param>
		''' <seealso cref= #setSelectedItem </seealso>
		Public Overridable Sub addItemListener(ByVal aListener As ItemListener)
			listenerList.add(GetType(ItemListener),aListener)
		End Sub

		''' <summary>
		''' Removes an <code>ItemListener</code>.
		''' </summary>
		''' <param name="aListener">  the <code>ItemListener</code> to remove </param>
		Public Overridable Sub removeItemListener(ByVal aListener As ItemListener)
			listenerList.remove(GetType(ItemListener),aListener)
		End Sub

		''' <summary>
		''' Returns an array of all the <code>ItemListener</code>s added
		''' to this JComboBox with addItemListener().
		''' </summary>
		''' <returns> all of the <code>ItemListener</code>s added or an empty
		'''         array if no listeners have been added
		''' @since 1.4 </returns>
		Public Overridable Property itemListeners As ItemListener()
			Get
				Return listenerList.getListeners(GetType(ItemListener))
			End Get
		End Property

		''' <summary>
		''' Adds an <code>ActionListener</code>.
		''' <p>
		''' The <code>ActionListener</code> will receive an <code>ActionEvent</code>
		''' when a selection has been made. If the combo box is editable, then
		''' an <code>ActionEvent</code> will be fired when editing has stopped.
		''' </summary>
		''' <param name="l">  the <code>ActionListener</code> that is to be notified </param>
		''' <seealso cref= #setSelectedItem </seealso>
		Public Overridable Sub addActionListener(ByVal l As ActionListener)
			listenerList.add(GetType(ActionListener),l)
		End Sub

		''' <summary>
		''' Removes an <code>ActionListener</code>.
		''' </summary>
		''' <param name="l">  the <code>ActionListener</code> to remove </param>
		Public Overridable Sub removeActionListener(ByVal l As ActionListener)
			If (l IsNot Nothing) AndAlso (action Is l) Then
				action = Nothing
			Else
				listenerList.remove(GetType(ActionListener), l)
			End If
		End Sub

		''' <summary>
		''' Returns an array of all the <code>ActionListener</code>s added
		''' to this JComboBox with addActionListener().
		''' </summary>
		''' <returns> all of the <code>ActionListener</code>s added or an empty
		'''         array if no listeners have been added
		''' @since 1.4 </returns>
		Public Overridable Property actionListeners As ActionListener()
			Get
				Return listenerList.getListeners(GetType(ActionListener))
			End Get
		End Property

		''' <summary>
		''' Adds a <code>PopupMenu</code> listener which will listen to notification
		''' messages from the popup portion of the combo box.
		''' <p>
		''' For all standard look and feels shipped with Java, the popup list
		''' portion of combo box is implemented as a <code>JPopupMenu</code>.
		''' A custom look and feel may not implement it this way and will
		''' therefore not receive the notification.
		''' </summary>
		''' <param name="l">  the <code>PopupMenuListener</code> to add
		''' @since 1.4 </param>
		Public Overridable Sub addPopupMenuListener(ByVal l As PopupMenuListener)
			listenerList.add(GetType(PopupMenuListener),l)
		End Sub

		''' <summary>
		''' Removes a <code>PopupMenuListener</code>.
		''' </summary>
		''' <param name="l">  the <code>PopupMenuListener</code> to remove </param>
		''' <seealso cref= #addPopupMenuListener
		''' @since 1.4 </seealso>
		Public Overridable Sub removePopupMenuListener(ByVal l As PopupMenuListener)
			listenerList.remove(GetType(PopupMenuListener),l)
		End Sub

		''' <summary>
		''' Returns an array of all the <code>PopupMenuListener</code>s added
		''' to this JComboBox with addPopupMenuListener().
		''' </summary>
		''' <returns> all of the <code>PopupMenuListener</code>s added or an empty
		'''         array if no listeners have been added
		''' @since 1.4 </returns>
		Public Overridable Property popupMenuListeners As PopupMenuListener()
			Get
				Return listenerList.getListeners(GetType(PopupMenuListener))
			End Get
		End Property

		''' <summary>
		''' Notifies <code>PopupMenuListener</code>s that the popup portion of the
		''' combo box will become visible.
		''' <p>
		''' This method is public but should not be called by anything other than
		''' the UI delegate. </summary>
		''' <seealso cref= #addPopupMenuListener
		''' @since 1.4 </seealso>
		Public Overridable Sub firePopupMenuWillBecomeVisible()
			Dim ___listeners As Object() = listenerList.listenerList
			Dim e As PopupMenuEvent=Nothing
			For i As Integer = ___listeners.Length-2 To 0 Step -2
				If ___listeners(i) Is GetType(PopupMenuListener) Then
					If e Is Nothing Then e = New PopupMenuEvent(Me)
					CType(___listeners(i+1), PopupMenuListener).popupMenuWillBecomeVisible(e)
				End If
			Next i
		End Sub

		''' <summary>
		''' Notifies <code>PopupMenuListener</code>s that the popup portion of the
		''' combo box has become invisible.
		''' <p>
		''' This method is public but should not be called by anything other than
		''' the UI delegate. </summary>
		''' <seealso cref= #addPopupMenuListener
		''' @since 1.4 </seealso>
		Public Overridable Sub firePopupMenuWillBecomeInvisible()
			Dim ___listeners As Object() = listenerList.listenerList
			Dim e As PopupMenuEvent=Nothing
			For i As Integer = ___listeners.Length-2 To 0 Step -2
				If ___listeners(i) Is GetType(PopupMenuListener) Then
					If e Is Nothing Then e = New PopupMenuEvent(Me)
					CType(___listeners(i+1), PopupMenuListener).popupMenuWillBecomeInvisible(e)
				End If
			Next i
		End Sub

		''' <summary>
		''' Notifies <code>PopupMenuListener</code>s that the popup portion of the
		''' combo box has been canceled.
		''' <p>
		''' This method is public but should not be called by anything other than
		''' the UI delegate. </summary>
		''' <seealso cref= #addPopupMenuListener
		''' @since 1.4 </seealso>
		Public Overridable Sub firePopupMenuCanceled()
			Dim ___listeners As Object() = listenerList.listenerList
			Dim e As PopupMenuEvent=Nothing
			For i As Integer = ___listeners.Length-2 To 0 Step -2
				If ___listeners(i) Is GetType(PopupMenuListener) Then
					If e Is Nothing Then e = New PopupMenuEvent(Me)
					CType(___listeners(i+1), PopupMenuListener).popupMenuCanceled(e)
				End If
			Next i
		End Sub

		''' <summary>
		''' Sets the action command that should be included in the event
		''' sent to action listeners.
		''' </summary>
		''' <param name="aCommand">  a string containing the "command" that is sent
		'''                  to action listeners; the same listener can then
		'''                  do different things depending on the command it
		'''                  receives </param>
		Public Overridable Property actionCommand As String
			Set(ByVal aCommand As String)
				actionCommand = aCommand
			End Set
			Get
				Return actionCommand
			End Get
		End Property


		Private action As Action
		Private actionPropertyChangeListener As java.beans.PropertyChangeListener

		''' <summary>
		''' Sets the <code>Action</code> for the <code>ActionEvent</code> source.
		''' The new <code>Action</code> replaces any previously set
		''' <code>Action</code> but does not affect <code>ActionListeners</code>
		''' independently added with <code>addActionListener</code>.
		''' If the <code>Action</code> is already a registered
		''' <code>ActionListener</code> for the <code>ActionEvent</code> source,
		''' it is not re-registered.
		''' <p>
		''' Setting the <code>Action</code> results in immediately changing
		''' all the properties described in <a href="Action.html#buttonActions">
		''' Swing Components Supporting <code>Action</code></a>.
		''' Subsequently, the combobox's properties are automatically updated
		''' as the <code>Action</code>'s properties change.
		''' <p>
		''' This method uses three other methods to set
		''' and help track the <code>Action</code>'s property values.
		''' It uses the <code>configurePropertiesFromAction</code> method
		''' to immediately change the combobox's properties.
		''' To track changes in the <code>Action</code>'s property values,
		''' this method registers the <code>PropertyChangeListener</code>
		''' returned by <code>createActionPropertyChangeListener</code>. The
		''' default {@code PropertyChangeListener} invokes the
		''' {@code actionPropertyChanged} method when a property in the
		''' {@code Action} changes.
		''' </summary>
		''' <param name="a"> the <code>Action</code> for the <code>JComboBox</code>,
		'''                  or <code>null</code>.
		''' @since 1.3 </param>
		''' <seealso cref= Action </seealso>
		''' <seealso cref= #getAction </seealso>
		''' <seealso cref= #configurePropertiesFromAction </seealso>
		''' <seealso cref= #createActionPropertyChangeListener </seealso>
		''' <seealso cref= #actionPropertyChanged
		''' @beaninfo
		'''        bound: true
		'''    attribute: visualUpdate true
		'''  description: the Action instance connected with this ActionEvent source </seealso>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Public Overridable Sub setAction(ByVal a As Action) 'JavaToDotNetTempPropertySetaction
		Public Overridable Property action As Action
			Set(ByVal a As Action)
				Dim oldValue As Action = action
				If action Is Nothing OrElse (Not action.Equals(a)) Then
					action = a
					If oldValue IsNot Nothing Then
						removeActionListener(oldValue)
						oldValue.removePropertyChangeListener(actionPropertyChangeListener)
						actionPropertyChangeListener = Nothing
					End If
					configurePropertiesFromAction(action)
					If action IsNot Nothing Then
						' Don't add if it is already a listener
						If Not isListener(GetType(ActionListener), action) Then addActionListener(action)
						' Reverse linkage:
						actionPropertyChangeListener = createActionPropertyChangeListener(action)
						action.addPropertyChangeListener(actionPropertyChangeListener)
					End If
					firePropertyChange("action", oldValue, action)
				End If
			End Set
			Get
		End Property

		Private Function isListener(ByVal c As Type, ByVal a As ActionListener) As Boolean
			Dim ___isListener As Boolean = False
			Dim ___listeners As Object() = listenerList.listenerList
			For i As Integer = ___listeners.Length-2 To 0 Step -2
				If ___listeners(i) Is c AndAlso ___listeners(i+1) Is a Then ___isListener=True
			Next i
			Return ___isListener
		End Function

			Return action
		End Function

		''' <summary>
		''' Sets the properties on this combobox to match those in the specified
		''' <code>Action</code>.  Refer to <a href="Action.html#buttonActions">
		''' Swing Components Supporting <code>Action</code></a> for more
		''' details as to which properties this sets.
		''' </summary>
		''' <param name="a"> the <code>Action</code> from which to get the properties,
		'''          or <code>null</code>
		''' @since 1.3 </param>
		''' <seealso cref= Action </seealso>
		''' <seealso cref= #setAction </seealso>
		Protected Friend Overridable Sub configurePropertiesFromAction(ByVal a As Action)
			AbstractAction.enabledFromActionion(Me, a)
			AbstractAction.toolTipTextFromActionion(Me, a)
			actionCommandFromAction = a
		End Sub

		''' <summary>
		''' Creates and returns a <code>PropertyChangeListener</code> that is
		''' responsible for listening for changes from the specified
		''' <code>Action</code> and updating the appropriate properties.
		''' <p>
		''' <b>Warning:</b> If you subclass this do not create an anonymous
		''' inner class.  If you do the lifetime of the combobox will be tied to
		''' that of the <code>Action</code>.
		''' </summary>
		''' <param name="a"> the combobox's action
		''' @since 1.3 </param>
		''' <seealso cref= Action </seealso>
		''' <seealso cref= #setAction </seealso>
		Protected Friend Overridable Function createActionPropertyChangeListener(ByVal a As Action) As java.beans.PropertyChangeListener
			Return New ComboBoxActionPropertyChangeListener(Me, a)
		End Function

		''' <summary>
		''' Updates the combobox's state in response to property changes in
		''' associated action. This method is invoked from the
		''' {@code PropertyChangeListener} returned from
		''' {@code createActionPropertyChangeListener}. Subclasses do not normally
		''' need to invoke this. Subclasses that support additional {@code Action}
		''' properties should override this and
		''' {@code configurePropertiesFromAction}.
		''' <p>
		''' Refer to the table at <a href="Action.html#buttonActions">
		''' Swing Components Supporting <code>Action</code></a> for a list of
		''' the properties this method sets.
		''' </summary>
		''' <param name="action"> the <code>Action</code> associated with this combobox </param>
		''' <param name="propertyName"> the name of the property that changed
		''' @since 1.6 </param>
		''' <seealso cref= Action </seealso>
		''' <seealso cref= #configurePropertiesFromAction </seealso>
		Protected Friend Overridable Sub actionPropertyChanged(ByVal action As Action, ByVal propertyName As String)
			If propertyName = Action.ACTION_COMMAND_KEY Then
				actionCommandFromAction = action
			ElseIf propertyName = "enabled" Then
				AbstractAction.enabledFromActionion(Me, action)
			ElseIf Action.SHORT_DESCRIPTION = propertyName Then
				AbstractAction.toolTipTextFromActionion(Me, action)
			End If
		End Sub

		Private Property actionCommandFromAction As Action
			Set(ByVal a As Action)
				actionCommand = If(a IsNot Nothing, CStr(a.getValue(Action.ACTION_COMMAND_KEY)), Nothing)
			End Set
		End Property


		Private Class ComboBoxActionPropertyChangeListener
			Inherits ActionPropertyChangeListener(Of JComboBox(Of JavaToDotNetGenericWildcard))

			Friend Sub New(Of T1)(ByVal b As JComboBox(Of T1), ByVal a As Action)
				MyBase.New(b, a)
			End Sub
			Protected Friend Overridable Sub actionPropertyChanged(Of T1)(ByVal cb As JComboBox(Of T1), ByVal action As Action, ByVal e As java.beans.PropertyChangeEvent)
				If AbstractAction.shouldReconfigure(e) Then
					cb.configurePropertiesFromAction(action)
				Else
					cb.actionPropertyChanged(action, e.propertyName)
				End If
			End Sub
		End Class

		''' <summary>
		''' Notifies all listeners that have registered interest for
		''' notification on this event type. </summary>
		''' <param name="e">  the event of interest
		''' </param>
		''' <seealso cref= EventListenerList </seealso>
		Protected Friend Overridable Sub fireItemStateChanged(ByVal e As ItemEvent)
			' Guaranteed to return a non-null array
			Dim ___listeners As Object() = listenerList.listenerList
			' Process the listeners last to first, notifying
			' those that are interested in this event
			For i As Integer = ___listeners.Length-2 To 0 Step -2
				If ___listeners(i) Is GetType(ItemListener) Then CType(___listeners(i+1), ItemListener).itemStateChanged(e)
			Next i
		End Sub

		''' <summary>
		''' Notifies all listeners that have registered interest for
		''' notification on this event type.
		''' </summary>
		''' <seealso cref= EventListenerList </seealso>
		Protected Friend Overridable Sub fireActionEvent()
			If Not firingActionEvent Then
				' Set flag to ensure that an infinite loop is not created
				firingActionEvent = True
				Dim e As ActionEvent = Nothing
				' Guaranteed to return a non-null array
				Dim ___listeners As Object() = listenerList.listenerList
				Dim mostRecentEventTime As Long = EventQueue.mostRecentEventTime
				Dim modifiers As Integer = 0
				Dim currentEvent As AWTEvent = EventQueue.currentEvent
				If TypeOf currentEvent Is InputEvent Then
					modifiers = CType(currentEvent, InputEvent).modifiers
				ElseIf TypeOf currentEvent Is ActionEvent Then
					modifiers = CType(currentEvent, ActionEvent).modifiers
				End If
				' Process the listeners last to first, notifying
				' those that are interested in this event
				For i As Integer = ___listeners.Length-2 To 0 Step -2
					If ___listeners(i) Is GetType(ActionListener) Then
						' Lazily create the event:
						If e Is Nothing Then e = New ActionEvent(Me,ActionEvent.ACTION_PERFORMED, actionCommand, mostRecentEventTime, modifiers)
						CType(___listeners(i+1), ActionListener).actionPerformed(e)
					End If
				Next i
				firingActionEvent = False
			End If
		End Sub

		''' <summary>
		''' This protected method is implementation specific. Do not access directly
		''' or override.
		''' </summary>
		Protected Friend Overridable Sub selectedItemChanged()
			If selectedItemReminder IsNot Nothing Then fireItemStateChanged(New ItemEvent(Me,ItemEvent.ITEM_STATE_CHANGED, selectedItemReminder, ItemEvent.DESELECTED))

			' set the new selected item.
			selectedItemReminder = dataModel.selectedItem

			If selectedItemReminder IsNot Nothing Then fireItemStateChanged(New ItemEvent(Me,ItemEvent.ITEM_STATE_CHANGED, selectedItemReminder, ItemEvent.SELECTED))
		End Sub

		''' <summary>
		''' Returns an array containing the selected item.
		''' This method is implemented for compatibility with
		''' <code>ItemSelectable</code>.
		''' </summary>
		''' <returns> an array of <code>Objects</code> containing one
		'''          element -- the selected item </returns>
		Public Overridable Property selectedObjects As Object()
			Get
				Dim selectedObject As Object = selectedItem
				If selectedObject Is Nothing Then
					Return New Object(){}
				Else
					Dim result As Object() = New Object(0){}
					result(0) = selectedObject
					Return result
				End If
			End Get
		End Property

		''' <summary>
		''' This method is public as an implementation side effect.
		''' do not call or override.
		''' </summary>
		Public Overridable Sub actionPerformed(ByVal e As ActionEvent)
			Dim ___editor As ComboBoxEditor = editor
			If (___editor IsNot Nothing) AndAlso (e IsNot Nothing) AndAlso (___editor Is e.source OrElse ___editor.editorComponent Is e.source) Then
				popupVisible = False
				model.selectedItem = ___editor.item
				Dim oldCommand As String = actionCommand
				actionCommand = "comboBoxEdited"
				fireActionEvent()
				actionCommand = oldCommand
			End If
		End Sub

		''' <summary>
		''' This method is public as an implementation side effect.
		''' do not call or override.
		''' </summary>
		Public Overridable Sub contentsChanged(ByVal e As ListDataEvent) Implements ListDataListener.contentsChanged
			Dim oldSelection As Object = selectedItemReminder
			Dim newSelection As Object = dataModel.selectedItem
			If oldSelection Is Nothing OrElse (Not oldSelection.Equals(newSelection)) Then
				selectedItemChanged()
				If Not selectingItem Then fireActionEvent()
			End If
		End Sub

		''' <summary>
		''' This method is public as an implementation side effect.
		''' do not call or override.
		''' </summary>
		Public Overridable Sub intervalAdded(ByVal e As ListDataEvent) Implements ListDataListener.intervalAdded
			If selectedItemReminder IsNot dataModel.selectedItem Then selectedItemChanged()
		End Sub

		''' <summary>
		''' This method is public as an implementation side effect.
		''' do not call or override.
		''' </summary>
		Public Overridable Sub intervalRemoved(ByVal e As ListDataEvent) Implements ListDataListener.intervalRemoved
			contentsChanged(e)
		End Sub

		''' <summary>
		''' Selects the list item that corresponds to the specified keyboard
		''' character and returns true, if there is an item corresponding
		''' to that character.  Otherwise, returns false.
		''' </summary>
		''' <param name="keyChar"> a char, typically this is a keyboard key
		'''                  typed by the user </param>
		Public Overridable Function selectWithKeyChar(ByVal keyChar As Char) As Boolean
			Dim index As Integer

			If keySelectionManager Is Nothing Then keySelectionManager = createDefaultKeySelectionManager()

			index = keySelectionManager.selectionForKey(keyChar,model)
			If index <> -1 Then
				selectedIndex = index
				Return True
			Else
				Return False
			End If
		End Function

		''' <summary>
		''' Enables the combo box so that items can be selected. When the
		''' combo box is disabled, items cannot be selected and values
		''' cannot be typed into its field (if it is editable).
		''' </summary>
		''' <param name="b"> a boolean value, where true enables the component and
		'''          false disables it
		''' @beaninfo
		'''        bound: true
		'''    preferred: true
		'''  description: Whether the combo box is enabled. </param>
		Public Overrides Property enabled As Boolean
			Set(ByVal b As Boolean)
				MyBase.enabled = b
				firePropertyChange("enabled", (Not enabled), enabled)
			End Set
		End Property

		''' <summary>
		''' Initializes the editor with the specified item.
		''' </summary>
		''' <param name="anEditor"> the <code>ComboBoxEditor</code> that displays
		'''                  the list item in the
		'''                  combo box field and allows it to be edited </param>
		''' <param name="anItem">   the object to display and edit in the field </param>
		Public Overridable Sub configureEditor(ByVal anEditor As ComboBoxEditor, ByVal anItem As Object)
			anEditor.item = anItem
		End Sub

		''' <summary>
		''' Handles <code>KeyEvent</code>s, looking for the Tab key.
		''' If the Tab key is found, the popup window is closed.
		''' </summary>
		''' <param name="e">  the <code>KeyEvent</code> containing the keyboard
		'''          key that was pressed </param>
		Public Overrides Sub processKeyEvent(ByVal e As KeyEvent)
			If e.keyCode = KeyEvent.VK_TAB Then hidePopup()
			MyBase.processKeyEvent(e)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Function processKeyBinding(ByVal ks As KeyStroke, ByVal e As KeyEvent, ByVal condition As Integer, ByVal pressed As Boolean) As Boolean
			If MyBase.processKeyBinding(ks, e, condition, pressed) Then Return True

			If (Not editable) OrElse condition <> WHEN_FOCUSED OrElse editor Is Nothing OrElse (Not Boolean.TRUE.Equals(getClientProperty("JComboBox.isTableCellEditor"))) Then Return False

			Dim editorComponent As Component = editor.editorComponent
			If TypeOf editorComponent Is JComponent Then
				Dim component As JComponent = CType(editorComponent, JComponent)
				Return component.processKeyBinding(ks, e, WHEN_FOCUSED, pressed)
			End If
			Return False
		End Function

		''' <summary>
		''' Sets the object that translates a keyboard character into a list
		''' selection. Typically, the first selection with a matching first
		''' character becomes the selected item.
		''' 
		''' @beaninfo
		'''       expert: true
		'''  description: The objects that changes the selection when a key is pressed.
		''' </summary>
		Public Overridable Property keySelectionManager As KeySelectionManager
			Set(ByVal aManager As KeySelectionManager)
				keySelectionManager = aManager
			End Set
			Get
				Return keySelectionManager
			End Get
		End Property


		' Accessing the model 
		''' <summary>
		''' Returns the number of items in the list.
		''' </summary>
		''' <returns> an integer equal to the number of items in the list </returns>
		Public Overridable Property itemCount As Integer
			Get
				Return dataModel.size
			End Get
		End Property

		''' <summary>
		''' Returns the list item at the specified index.  If <code>index</code>
		''' is out of range (less than zero or greater than or equal to size)
		''' it will return <code>null</code>.
		''' </summary>
		''' <param name="index">  an integer indicating the list position, where the first
		'''               item starts at zero </param>
		''' <returns> the item at that list position; or
		'''                  <code>null</code> if out of range </returns>
		Public Overridable Function getItemAt(ByVal index As Integer) As E
			Return dataModel.getElementAt(index)
		End Function

		''' <summary>
		''' Returns an instance of the default key-selection manager.
		''' </summary>
		''' <returns> the <code>KeySelectionManager</code> currently used by the list </returns>
		''' <seealso cref= #setKeySelectionManager </seealso>
		Protected Friend Overridable Function createDefaultKeySelectionManager() As KeySelectionManager
			Return New DefaultKeySelectionManager(Me)
		End Function


		''' <summary>
		''' The interface that defines a <code>KeySelectionManager</code>.
		''' To qualify as a <code>KeySelectionManager</code>,
		''' the class needs to implement the method
		''' that identifies the list index given a character and the
		''' combo box data model.
		''' </summary>
		Public Interface KeySelectionManager
			''' <summary>
			''' Given <code>aKey</code> and the model, returns the row
			'''  that should become selected. Return -1 if no match was
			'''  found.
			''' </summary>
			''' <param name="aKey">  a char value, usually indicating a keyboard key that
			'''               was pressed </param>
			''' <param name="aModel"> a ComboBoxModel -- the component's data model, containing
			'''               the list of selectable items </param>
			''' <returns> an int equal to the selected row, where 0 is the
			'''         first item and -1 is none. </returns>
			Function selectionForKey(ByVal aKey As Char, ByVal aModel As ComboBoxModel) As Integer
		End Interface

		<Serializable> _
		Friend Class DefaultKeySelectionManager
			Implements KeySelectionManager

			Private ReadOnly outerInstance As JComboBox

			Public Sub New(ByVal outerInstance As JComboBox)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Function selectionForKey(ByVal aKey As Char, ByVal aModel As ComboBoxModel) As Integer
				Dim i, c As Integer
				Dim currentSelection As Integer = -1
				Dim selectedItem As Object = aModel.selectedItem
				Dim v As String
				Dim pattern As String

				If selectedItem IsNot Nothing Then
					i=0
					c=aModel.size
					Do While i<c
						If selectedItem Is aModel.getElementAt(i) Then
							currentSelection = i
							Exit Do
						End If
						i += 1
					Loop
				End If

				pattern = ("" & AscW(aKey)).ToLower()
				aKey = pattern.Chars(0)

				currentSelection += 1
				i = currentSelection
				c = aModel.size
				Do While i < c
					Dim elem As Object = aModel.getElementAt(i)
					If elem IsNot Nothing AndAlso elem.ToString() IsNot Nothing Then
						v = elem.ToString().ToLower()
						If v.Length > 0 AndAlso v.Chars(0) = aKey Then Return i
					End If
					i += 1
				Loop

				For i = 0 To currentSelection - 1
					Dim elem As Object = aModel.getElementAt(i)
					If elem IsNot Nothing AndAlso elem.ToString() IsNot Nothing Then
						v = elem.ToString().ToLower()
						If v.Length > 0 AndAlso v.Chars(0) = aKey Then Return i
					End If
				Next i
				Return -1
			End Function
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
		''' Returns a string representation of this <code>JComboBox</code>.
		''' This method is intended to be used only for debugging purposes,
		''' and the content and format of the returned string may vary between
		''' implementations. The returned string may be empty but may not
		''' be <code>null</code>.
		''' </summary>
		''' <returns>  a string representation of this <code>JComboBox</code> </returns>
		Protected Friend Overrides Function paramString() As String
			Dim selectedItemReminderString As String = (If(selectedItemReminder IsNot Nothing, selectedItemReminder.ToString(), ""))
			Dim isEditableString As String = (If(___isEditable, "true", "false"))
			Dim lightWeightPopupEnabledString As String = (If(lightWeightPopupEnabled, "true", "false"))

			Return MyBase.paramString() & ",isEditable=" & isEditableString & ",lightWeightPopupEnabled=" & lightWeightPopupEnabledString & ",maximumRowCount=" & maximumRowCount & ",selectedItemReminder=" & selectedItemReminderString
		End Function


	'/////////////////
	' Accessibility support
	'/////////////////

		''' <summary>
		''' Gets the AccessibleContext associated with this JComboBox.
		''' For combo boxes, the AccessibleContext takes the form of an
		''' AccessibleJComboBox.
		''' A new AccessibleJComboBox instance is created if necessary.
		''' </summary>
		''' <returns> an AccessibleJComboBox that serves as the
		'''         AccessibleContext of this JComboBox </returns>
		Public Overridable Property accessibleContext As AccessibleContext Implements Accessible.getAccessibleContext
			Get
				If accessibleContext Is Nothing Then accessibleContext = New AccessibleJComboBox(Me)
				Return accessibleContext
			End Get
		End Property

		''' <summary>
		''' This class implements accessibility support for the
		''' <code>JComboBox</code> class.  It provides an implementation of the
		''' Java Accessibility API appropriate to Combo Box user-interface elements.
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
		Protected Friend Class AccessibleJComboBox
			Inherits AccessibleJComponent
			Implements AccessibleAction, AccessibleSelection

			Private ReadOnly outerInstance As JComboBox



			Private popupList As JList ' combo box popup list
			Private previousSelectedAccessible As Accessible = Nothing

			''' <summary>
			''' Returns an AccessibleJComboBox instance
			''' @since 1.4
			''' </summary>
			Public Sub New(ByVal outerInstance As JComboBox)
					Me.outerInstance = outerInstance
				' set the combo box editor's accessible name and description
				outerInstance.addPropertyChangeListener(New AccessibleJComboBoxPropertyChangeListener(Me))
				editorNameAndDescriptionion()

				' Get the popup list
				Dim a As Accessible = outerInstance.uI.getAccessibleChild(JComboBox.this, 0)
				If TypeOf a Is javax.swing.plaf.basic.ComboPopup Then
					' Listen for changes to the popup menu selection.
					popupList = CType(a, javax.swing.plaf.basic.ComboPopup).list
					popupList.addListSelectionListener(New AccessibleJComboBoxListSelectionListener(Me))
				End If
				' Listen for popup menu show/hide events
				outerInstance.addPopupMenuListener(New AccessibleJComboBoxPopupMenuListener(Me))
			End Sub

	'        
	'         * JComboBox PropertyChangeListener
	'         
			Private Class AccessibleJComboBoxPropertyChangeListener
				Implements java.beans.PropertyChangeListener

				Private ReadOnly outerInstance As JComboBox.AccessibleJComboBox

				Public Sub New(ByVal outerInstance As JComboBox.AccessibleJComboBox)
					Me.outerInstance = outerInstance
				End Sub


				Public Overridable Sub propertyChange(ByVal e As java.beans.PropertyChangeEvent)
					If e.propertyName = "editor" Then outerInstance.editorNameAndDescriptionion()
				End Sub
			End Class

	'        
	'         * Sets the combo box editor's accessible name and descripton
	'         
			Private Sub setEditorNameAndDescription()
				Dim editor As ComboBoxEditor = outerInstance.editor
				If editor IsNot Nothing Then
					Dim comp As Component = editor.editorComponent
					If TypeOf comp Is Accessible Then
						Dim ac As AccessibleContext = comp.accessibleContext
						If ac IsNot Nothing Then ' may be null
							ac.accessibleName = accessibleName
							ac.accessibleDescription = accessibleDescription
						End If
					End If
				End If
			End Sub

	'        
	'         * Listener for combo box popup menu
	'         * TIGER - 4669379 4894434
	'         
			Private Class AccessibleJComboBoxPopupMenuListener
				Implements PopupMenuListener

				Private ReadOnly outerInstance As JComboBox.AccessibleJComboBox

				Public Sub New(ByVal outerInstance As JComboBox.AccessibleJComboBox)
					Me.outerInstance = outerInstance
				End Sub


				''' <summary>
				'''  This method is called before the popup menu becomes visible
				''' </summary>
				Public Overridable Sub popupMenuWillBecomeVisible(ByVal e As PopupMenuEvent) Implements PopupMenuListener.popupMenuWillBecomeVisible
					' save the initial selection
					If outerInstance.popupList Is Nothing Then Return
					Dim selectedIndex As Integer = outerInstance.popupList.selectedIndex
					If selectedIndex < 0 Then Return
					outerInstance.previousSelectedAccessible = outerInstance.popupList.accessibleContext.getAccessibleChild(selectedIndex)
				End Sub

				''' <summary>
				''' This method is called before the popup menu becomes invisible
				''' Note that a JPopupMenu can become invisible any time
				''' </summary>
				Public Overridable Sub popupMenuWillBecomeInvisible(ByVal e As PopupMenuEvent) Implements PopupMenuListener.popupMenuWillBecomeInvisible
					' ignore
				End Sub

				''' <summary>
				''' This method is called when the popup menu is canceled
				''' </summary>
				Public Overridable Sub popupMenuCanceled(ByVal e As PopupMenuEvent) Implements PopupMenuListener.popupMenuCanceled
					' ignore
				End Sub
			End Class

	'        
	'         * Handles changes to the popup list selection.
	'         * TIGER - 4669379 4894434 4933143
	'         
			Private Class AccessibleJComboBoxListSelectionListener
				Implements ListSelectionListener

				Private ReadOnly outerInstance As JComboBox.AccessibleJComboBox

				Public Sub New(ByVal outerInstance As JComboBox.AccessibleJComboBox)
					Me.outerInstance = outerInstance
				End Sub


				Public Overridable Sub valueChanged(ByVal e As ListSelectionEvent) Implements ListSelectionListener.valueChanged
					If outerInstance.popupList Is Nothing Then Return

					' Get the selected popup list item.
					Dim selectedIndex As Integer = outerInstance.popupList.selectedIndex
					If selectedIndex < 0 Then Return
					Dim selectedAccessible As Accessible = outerInstance.popupList.accessibleContext.getAccessibleChild(selectedIndex)
					If selectedAccessible Is Nothing Then Return

					' Fire a FOCUSED lost PropertyChangeEvent for the
					' previously selected list item.
					Dim pce As java.beans.PropertyChangeEvent

					If outerInstance.previousSelectedAccessible IsNot Nothing Then
						pce = New java.beans.PropertyChangeEvent(outerInstance.previousSelectedAccessible, AccessibleContext.ACCESSIBLE_STATE_PROPERTY, AccessibleState.FOCUSED, Nothing)
						firePropertyChange(AccessibleContext.ACCESSIBLE_STATE_PROPERTY, Nothing, pce)
					End If
					' Fire a FOCUSED gained PropertyChangeEvent for the
					' currently selected list item.
					pce = New java.beans.PropertyChangeEvent(selectedAccessible, AccessibleContext.ACCESSIBLE_STATE_PROPERTY, Nothing, AccessibleState.FOCUSED)
					firePropertyChange(AccessibleContext.ACCESSIBLE_STATE_PROPERTY, Nothing, pce)

					' Fire the ACCESSIBLE_ACTIVE_DESCENDANT_PROPERTY event
					' for the combo box.
					firePropertyChange(AccessibleContext.ACCESSIBLE_ACTIVE_DESCENDANT_PROPERTY, outerInstance.previousSelectedAccessible, selectedAccessible)

					' Save the previous selection.
					outerInstance.previousSelectedAccessible = selectedAccessible
				End Sub
			End Class


			''' <summary>
			''' Returns the number of accessible children in the object.  If all
			''' of the children of this object implement Accessible, than this
			''' method should return the number of children of this object.
			''' </summary>
			''' <returns> the number of accessible children in the object. </returns>
			Public Overridable Property accessibleChildrenCount As Integer
				Get
					' Always delegate to the UI if it exists
					If outerInstance.ui IsNot Nothing Then
						Return outerInstance.ui.getAccessibleChildrenCount(JComboBox.this)
					Else
						Return MyBase.accessibleChildrenCount
					End If
				End Get
			End Property

			''' <summary>
			''' Returns the nth Accessible child of the object.
			''' The child at index zero represents the popup.
			''' If the combo box is editable, the child at index one
			''' represents the editor.
			''' </summary>
			''' <param name="i"> zero-based index of child </param>
			''' <returns> the nth Accessible child of the object </returns>
			Public Overridable Function getAccessibleChild(ByVal i As Integer) As Accessible
				' Always delegate to the UI if it exists
				If outerInstance.ui IsNot Nothing Then
					Return outerInstance.ui.getAccessibleChild(JComboBox.this, i)
				Else
				   Return MyBase.getAccessibleChild(i)
				End If
			End Function

			''' <summary>
			''' Get the role of this object.
			''' </summary>
			''' <returns> an instance of AccessibleRole describing the role of the
			''' object </returns>
			''' <seealso cref= AccessibleRole </seealso>
			Public Overridable Property accessibleRole As AccessibleRole
				Get
					Return AccessibleRole.COMBO_BOX
				End Get
			End Property

			''' <summary>
			''' Gets the state set of this object.  The AccessibleStateSet of
			''' an object is composed of a set of unique AccessibleStates.
			''' A change in the AccessibleStateSet of an object will cause a
			''' PropertyChangeEvent to be fired for the ACCESSIBLE_STATE_PROPERTY
			''' property.
			''' </summary>
			''' <returns> an instance of AccessibleStateSet containing the
			''' current state set of the object </returns>
			''' <seealso cref= AccessibleStateSet </seealso>
			''' <seealso cref= AccessibleState </seealso>
			''' <seealso cref= #addPropertyChangeListener
			'''  </seealso>
			Public Overridable Property accessibleStateSet As AccessibleStateSet
				Get
					' TIGER - 4489748
					Dim ass As AccessibleStateSet = MyBase.accessibleStateSet
					If ass Is Nothing Then ass = New AccessibleStateSet
					If outerInstance.popupVisible Then
						ass.add(AccessibleState.EXPANDED)
					Else
						ass.add(AccessibleState.COLLAPSED)
					End If
					Return ass
				End Get
			End Property

			''' <summary>
			''' Get the AccessibleAction associated with this object.  In the
			''' implementation of the Java Accessibility API for this class,
			''' return this object, which is responsible for implementing the
			''' AccessibleAction interface on behalf of itself.
			''' </summary>
			''' <returns> this object </returns>
			Public Overridable Property accessibleAction As AccessibleAction
				Get
					Return Me
				End Get
			End Property

			''' <summary>
			''' Return a description of the specified action of the object.
			''' </summary>
			''' <param name="i"> zero-based index of the actions </param>
			Public Overridable Function getAccessibleActionDescription(ByVal i As Integer) As String Implements AccessibleAction.getAccessibleActionDescription
				If i = 0 Then
					Return UIManager.getString("ComboBox.togglePopupText")
				Else
					Return Nothing
				End If
			End Function

			''' <summary>
			''' Returns the number of Actions available in this object.  The
			''' default behavior of a combo box is to have one action.
			''' </summary>
			''' <returns> 1, the number of Actions in this object </returns>
			Public Overridable Property accessibleActionCount As Integer Implements AccessibleAction.getAccessibleActionCount
				Get
					Return 1
				End Get
			End Property

			''' <summary>
			''' Perform the specified Action on the object
			''' </summary>
			''' <param name="i"> zero-based index of actions </param>
			''' <returns> true if the the action was performed; else false. </returns>
			Public Overridable Function doAccessibleAction(ByVal i As Integer) As Boolean Implements AccessibleAction.doAccessibleAction
				If i = 0 Then
					outerInstance.popupVisible = (Not outerInstance.popupVisible)
					Return True
				Else
					Return False
				End If
			End Function


			''' <summary>
			''' Get the AccessibleSelection associated with this object.  In the
			''' implementation of the Java Accessibility API for this class,
			''' return this object, which is responsible for implementing the
			''' AccessibleSelection interface on behalf of itself.
			''' </summary>
			''' <returns> this object </returns>
			Public Overridable Property accessibleSelection As AccessibleSelection
				Get
					Return Me
				End Get
			End Property

			''' <summary>
			''' Returns the number of Accessible children currently selected.
			''' If no children are selected, the return value will be 0.
			''' </summary>
			''' <returns> the number of items currently selected.
			''' @since 1.3 </returns>
			Public Overridable Property accessibleSelectionCount As Integer Implements AccessibleSelection.getAccessibleSelectionCount
				Get
					Dim o As Object = outerInstance.selectedItem
					If o IsNot Nothing Then
						Return 1
					Else
						Return 0
					End If
				End Get
			End Property

			''' <summary>
			''' Returns an Accessible representing the specified selected child
			''' in the popup.  If there isn't a selection, or there are
			''' fewer children selected than the integer passed in, the return
			''' value will be null.
			''' <p>Note that the index represents the i-th selected child, which
			''' is different from the i-th child.
			''' </summary>
			''' <param name="i"> the zero-based index of selected children </param>
			''' <returns> the i-th selected child </returns>
			''' <seealso cref= #getAccessibleSelectionCount
			''' @since 1.3 </seealso>
			Public Overridable Function getAccessibleSelection(ByVal i As Integer) As Accessible Implements AccessibleSelection.getAccessibleSelection
				' Get the popup
				Dim a As Accessible = outerInstance.uI.getAccessibleChild(JComboBox.this, 0)
				If a IsNot Nothing AndAlso TypeOf a Is javax.swing.plaf.basic.ComboPopup Then

					' get the popup list
					Dim list As JList = CType(a, javax.swing.plaf.basic.ComboPopup).list

					' return the i-th selection in the popup list
					Dim ac As AccessibleContext = list.accessibleContext
					If ac IsNot Nothing Then
						Dim [as] As AccessibleSelection = ac.accessibleSelection
						If [as] IsNot Nothing Then Return [as].getAccessibleSelection(i)
					End If
				End If
				Return Nothing
			End Function

			''' <summary>
			''' Determines if the current child of this object is selected.
			''' </summary>
			''' <returns> true if the current child of this object is selected;
			'''              else false </returns>
			''' <param name="i"> the zero-based index of the child in this Accessible
			''' object. </param>
			''' <seealso cref= AccessibleContext#getAccessibleChild
			''' @since 1.3 </seealso>
			Public Overridable Function isAccessibleChildSelected(ByVal i As Integer) As Boolean Implements AccessibleSelection.isAccessibleChildSelected
				Return outerInstance.selectedIndex = i
			End Function

			''' <summary>
			''' Adds the specified Accessible child of the object to the object's
			''' selection.  If the object supports multiple selections,
			''' the specified child is added to any existing selection, otherwise
			''' it replaces any existing selection in the object.  If the
			''' specified child is already selected, this method has no effect.
			''' </summary>
			''' <param name="i"> the zero-based index of the child </param>
			''' <seealso cref= AccessibleContext#getAccessibleChild
			''' @since 1.3 </seealso>
			Public Overridable Sub addAccessibleSelection(ByVal i As Integer) Implements AccessibleSelection.addAccessibleSelection
				' TIGER - 4856195
				clearAccessibleSelection()
				outerInstance.selectedIndex = i
			End Sub

			''' <summary>
			''' Removes the specified child of the object from the object's
			''' selection.  If the specified item isn't currently selected, this
			''' method has no effect.
			''' </summary>
			''' <param name="i"> the zero-based index of the child </param>
			''' <seealso cref= AccessibleContext#getAccessibleChild
			''' @since 1.3 </seealso>
			Public Overridable Sub removeAccessibleSelection(ByVal i As Integer) Implements AccessibleSelection.removeAccessibleSelection
				If outerInstance.selectedIndex = i Then clearAccessibleSelection()
			End Sub

			''' <summary>
			''' Clears the selection in the object, so that no children in the
			''' object are selected.
			''' @since 1.3
			''' </summary>
			Public Overridable Sub clearAccessibleSelection() Implements AccessibleSelection.clearAccessibleSelection
				outerInstance.selectedIndex = -1
			End Sub

			''' <summary>
			''' Causes every child of the object to be selected
			''' if the object supports multiple selections.
			''' @since 1.3
			''' </summary>
			Public Overridable Sub selectAllAccessibleSelection() Implements AccessibleSelection.selectAllAccessibleSelection
				' do nothing since multiple selection is not supported
			End Sub

	'        public Accessible getAccessibleAt(Point p) {
	'            Accessible a = getAccessibleChild(1);
	'            if ( a != null ) {
	'                return a; // the editor
	'            }
	'            else {
	'                return getAccessibleChild(0); // the list
	'            }
	'        }
			Private editorAccessibleContext As EditorAccessibleContext = Nothing

			Private Class AccessibleEditor
				Implements Accessible

				Private ReadOnly outerInstance As JComboBox.AccessibleJComboBox

				Public Sub New(ByVal outerInstance As JComboBox.AccessibleJComboBox)
					Me.outerInstance = outerInstance
				End Sub

				Public Overridable Property accessibleContext As AccessibleContext Implements Accessible.getAccessibleContext
					Get
						If outerInstance.editorAccessibleContext Is Nothing Then
							Dim c As Component = outerInstance.editor.editorComponent
							If TypeOf c Is Accessible Then outerInstance.editorAccessibleContext = New EditorAccessibleContext(CType(c, Accessible))
						End If
						Return outerInstance.editorAccessibleContext
					End Get
				End Property
			End Class

	'        
	'         * Wrapper class for the AccessibleContext implemented by the
	'         * combo box editor.  Delegates all method calls except
	'         * getAccessibleIndexInParent to the editor.  The
	'         * getAccessibleIndexInParent method returns the selected
	'         * index in the combo box.
	'         
			Private Class EditorAccessibleContext
				Inherits AccessibleContext

				Private ReadOnly outerInstance As JComboBox.AccessibleJComboBox


				Private ac As AccessibleContext

				Private Sub New(ByVal outerInstance As JComboBox.AccessibleJComboBox)
						Me.outerInstance = outerInstance
				End Sub

	'            
	'             * @param a the AccessibleContext implemented by the
	'             * combo box editor
	'             
				Friend Sub New(ByVal outerInstance As JComboBox.AccessibleJComboBox, ByVal a As Accessible)
						Me.outerInstance = outerInstance
					Me.ac = a.accessibleContext
				End Sub

				''' <summary>
				''' Gets the accessibleName property of this object.  The accessibleName
				''' property of an object is a localized String that designates the purpose
				''' of the object.  For example, the accessibleName property of a label
				''' or button might be the text of the label or button itself.  In the
				''' case of an object that doesn't display its name, the accessibleName
				''' should still be set.  For example, in the case of a text field used
				''' to enter the name of a city, the accessibleName for the en_US locale
				''' could be 'city.'
				''' </summary>
				''' <returns> the localized name of the object; null if this
				''' object does not have a name
				''' </returns>
				''' <seealso cref= #setAccessibleName </seealso>
				Public Property Overrides accessibleName As String
					Get
						Return ac.accessibleName
					End Get
					Set(ByVal s As String)
						ac.accessibleName = s
					End Set
				End Property


				''' <summary>
				''' Gets the accessibleDescription property of this object.  The
				''' accessibleDescription property of this object is a short localized
				''' phrase describing the purpose of the object.  For example, in the
				''' case of a 'Cancel' button, the accessibleDescription could be
				''' 'Ignore changes and close dialog box.'
				''' </summary>
				''' <returns> the localized description of the object; null if
				''' this object does not have a description
				''' </returns>
				''' <seealso cref= #setAccessibleDescription </seealso>
				Public Property Overrides accessibleDescription As String
					Get
						Return ac.accessibleDescription
					End Get
					Set(ByVal s As String)
						ac.accessibleDescription = s
					End Set
				End Property


				''' <summary>
				''' Gets the role of this object.  The role of the object is the generic
				''' purpose or use of the class of this object.  For example, the role
				''' of a push button is AccessibleRole.PUSH_BUTTON.  The roles in
				''' AccessibleRole are provided so component developers can pick from
				''' a set of predefined roles.  This enables assistive technologies to
				''' provide a consistent interface to various tweaked subclasses of
				''' components (e.g., use AccessibleRole.PUSH_BUTTON for all components
				''' that act like a push button) as well as distinguish between subclasses
				''' that behave differently (e.g., AccessibleRole.CHECK_BOX for check boxes
				''' and AccessibleRole.RADIO_BUTTON for radio buttons).
				''' <p>Note that the AccessibleRole class is also extensible, so
				''' custom component developers can define their own AccessibleRole's
				''' if the set of predefined roles is inadequate.
				''' </summary>
				''' <returns> an instance of AccessibleRole describing the role of the object </returns>
				''' <seealso cref= AccessibleRole </seealso>
				Public Property Overrides accessibleRole As AccessibleRole
					Get
						Return ac.accessibleRole
					End Get
				End Property

				''' <summary>
				''' Gets the state set of this object.  The AccessibleStateSet of an object
				''' is composed of a set of unique AccessibleStates.  A change in the
				''' AccessibleStateSet of an object will cause a PropertyChangeEvent to
				''' be fired for the ACCESSIBLE_STATE_PROPERTY property.
				''' </summary>
				''' <returns> an instance of AccessibleStateSet containing the
				''' current state set of the object </returns>
				''' <seealso cref= AccessibleStateSet </seealso>
				''' <seealso cref= AccessibleState </seealso>
				''' <seealso cref= #addPropertyChangeListener </seealso>
				Public Property Overrides accessibleStateSet As AccessibleStateSet
					Get
						Return ac.accessibleStateSet
					End Get
				End Property

				''' <summary>
				''' Gets the Accessible parent of this object.
				''' </summary>
				''' <returns> the Accessible parent of this object; null if this
				''' object does not have an Accessible parent </returns>
				Public Property Overrides accessibleParent As Accessible
					Get
						Return ac.accessibleParent
					End Get
					Set(ByVal a As Accessible)
						ac.accessibleParent = a
					End Set
				End Property


				''' <summary>
				''' Gets the 0-based index of this object in its accessible parent.
				''' </summary>
				''' <returns> the 0-based index of this object in its parent; -1 if this
				''' object does not have an accessible parent.
				''' </returns>
				''' <seealso cref= #getAccessibleParent </seealso>
				''' <seealso cref= #getAccessibleChildrenCount </seealso>
				''' <seealso cref= #getAccessibleChild </seealso>
				Public Property Overrides accessibleIndexInParent As Integer
					Get
						Return outerInstance.selectedIndex
					End Get
				End Property

				''' <summary>
				''' Returns the number of accessible children of the object.
				''' </summary>
				''' <returns> the number of accessible children of the object. </returns>
				Public Property Overrides accessibleChildrenCount As Integer
					Get
						Return ac.accessibleChildrenCount
					End Get
				End Property

				''' <summary>
				''' Returns the specified Accessible child of the object.  The Accessible
				''' children of an Accessible object are zero-based, so the first child
				''' of an Accessible child is at index 0, the second child is at index 1,
				''' and so on.
				''' </summary>
				''' <param name="i"> zero-based index of child </param>
				''' <returns> the Accessible child of the object </returns>
				''' <seealso cref= #getAccessibleChildrenCount </seealso>
				Public Overrides Function getAccessibleChild(ByVal i As Integer) As Accessible
					Return ac.getAccessibleChild(i)
				End Function

				''' <summary>
				''' Gets the locale of the component. If the component does not have a
				''' locale, then the locale of its parent is returned.
				''' </summary>
				''' <returns> this component's locale.  If this component does not have
				''' a locale, the locale of its parent is returned.
				''' </returns>
				''' <exception cref="IllegalComponentStateException">
				''' If the Component does not have its own locale and has not yet been
				''' added to a containment hierarchy such that the locale can be
				''' determined from the containing parent. </exception>
				Public Property Overrides locale As Locale
					Get
						Return ac.locale
					End Get
				End Property

				''' <summary>
				''' Adds a PropertyChangeListener to the listener list.
				''' The listener is registered for all Accessible properties and will
				''' be called when those properties change.
				''' </summary>
				''' <seealso cref= #ACCESSIBLE_NAME_PROPERTY </seealso>
				''' <seealso cref= #ACCESSIBLE_DESCRIPTION_PROPERTY </seealso>
				''' <seealso cref= #ACCESSIBLE_STATE_PROPERTY </seealso>
				''' <seealso cref= #ACCESSIBLE_VALUE_PROPERTY </seealso>
				''' <seealso cref= #ACCESSIBLE_SELECTION_PROPERTY </seealso>
				''' <seealso cref= #ACCESSIBLE_TEXT_PROPERTY </seealso>
				''' <seealso cref= #ACCESSIBLE_VISIBLE_DATA_PROPERTY
				''' </seealso>
				''' <param name="listener">  The PropertyChangeListener to be added </param>
				Public Overrides Sub addPropertyChangeListener(ByVal listener As java.beans.PropertyChangeListener)
					ac.addPropertyChangeListener(listener)
				End Sub

				''' <summary>
				''' Removes a PropertyChangeListener from the listener list.
				''' This removes a PropertyChangeListener that was registered
				''' for all properties.
				''' </summary>
				''' <param name="listener">  The PropertyChangeListener to be removed </param>
				Public Overrides Sub removePropertyChangeListener(ByVal listener As java.beans.PropertyChangeListener)
					ac.removePropertyChangeListener(listener)
				End Sub

				''' <summary>
				''' Gets the AccessibleAction associated with this object that supports
				''' one or more actions.
				''' </summary>
				''' <returns> AccessibleAction if supported by object; else return null </returns>
				''' <seealso cref= AccessibleAction </seealso>
				Public Property Overrides accessibleAction As AccessibleAction
					Get
						Return ac.accessibleAction
					End Get
				End Property

				''' <summary>
				''' Gets the AccessibleComponent associated with this object that has a
				''' graphical representation.
				''' </summary>
				''' <returns> AccessibleComponent if supported by object; else return null </returns>
				''' <seealso cref= AccessibleComponent </seealso>
				Public Property Overrides accessibleComponent As AccessibleComponent
					Get
						Return ac.accessibleComponent
					End Get
				End Property

				''' <summary>
				''' Gets the AccessibleSelection associated with this object which allows its
				''' Accessible children to be selected.
				''' </summary>
				''' <returns> AccessibleSelection if supported by object; else return null </returns>
				''' <seealso cref= AccessibleSelection </seealso>
				Public Property Overrides accessibleSelection As AccessibleSelection
					Get
						Return ac.accessibleSelection
					End Get
				End Property

				''' <summary>
				''' Gets the AccessibleText associated with this object presenting
				''' text on the display.
				''' </summary>
				''' <returns> AccessibleText if supported by object; else return null </returns>
				''' <seealso cref= AccessibleText </seealso>
				Public Property Overrides accessibleText As AccessibleText
					Get
						Return ac.accessibleText
					End Get
				End Property

				''' <summary>
				''' Gets the AccessibleEditableText associated with this object
				''' presenting editable text on the display.
				''' </summary>
				''' <returns> AccessibleEditableText if supported by object; else return null </returns>
				''' <seealso cref= AccessibleEditableText </seealso>
				Public Property Overrides accessibleEditableText As AccessibleEditableText
					Get
						Return ac.accessibleEditableText
					End Get
				End Property

				''' <summary>
				''' Gets the AccessibleValue associated with this object that supports a
				''' Numerical value.
				''' </summary>
				''' <returns> AccessibleValue if supported by object; else return null </returns>
				''' <seealso cref= AccessibleValue </seealso>
				Public Property Overrides accessibleValue As AccessibleValue
					Get
						Return ac.accessibleValue
					End Get
				End Property

				''' <summary>
				''' Gets the AccessibleIcons associated with an object that has
				''' one or more associated icons
				''' </summary>
				''' <returns> an array of AccessibleIcon if supported by object;
				''' otherwise return null </returns>
				''' <seealso cref= AccessibleIcon </seealso>
				Public Property Overrides accessibleIcon As AccessibleIcon()
					Get
						Return ac.accessibleIcon
					End Get
				End Property

				''' <summary>
				''' Gets the AccessibleRelationSet associated with an object
				''' </summary>
				''' <returns> an AccessibleRelationSet if supported by object;
				''' otherwise return null </returns>
				''' <seealso cref= AccessibleRelationSet </seealso>
				Public Property Overrides accessibleRelationSet As AccessibleRelationSet
					Get
						Return ac.accessibleRelationSet
					End Get
				End Property

				''' <summary>
				''' Gets the AccessibleTable associated with an object
				''' </summary>
				''' <returns> an AccessibleTable if supported by object;
				''' otherwise return null </returns>
				''' <seealso cref= AccessibleTable </seealso>
				Public Property Overrides accessibleTable As AccessibleTable
					Get
						Return ac.accessibleTable
					End Get
				End Property

				''' <summary>
				''' Support for reporting bound property changes.  If oldValue and
				''' newValue are not equal and the PropertyChangeEvent listener list
				''' is not empty, then fire a PropertyChange event to each listener.
				''' In general, this is for use by the Accessible objects themselves
				''' and should not be called by an application program. </summary>
				''' <param name="propertyName">  The programmatic name of the property that
				''' was changed. </param>
				''' <param name="oldValue">  The old value of the property. </param>
				''' <param name="newValue">  The new value of the property. </param>
				''' <seealso cref= java.beans.PropertyChangeSupport </seealso>
				''' <seealso cref= #addPropertyChangeListener </seealso>
				''' <seealso cref= #removePropertyChangeListener </seealso>
				''' <seealso cref= #ACCESSIBLE_NAME_PROPERTY </seealso>
				''' <seealso cref= #ACCESSIBLE_DESCRIPTION_PROPERTY </seealso>
				''' <seealso cref= #ACCESSIBLE_STATE_PROPERTY </seealso>
				''' <seealso cref= #ACCESSIBLE_VALUE_PROPERTY </seealso>
				''' <seealso cref= #ACCESSIBLE_SELECTION_PROPERTY </seealso>
				''' <seealso cref= #ACCESSIBLE_TEXT_PROPERTY </seealso>
				''' <seealso cref= #ACCESSIBLE_VISIBLE_DATA_PROPERTY </seealso>
				Public Overrides Sub firePropertyChange(ByVal propertyName As String, ByVal oldValue As Object, ByVal newValue As Object)
					ac.firePropertyChange(propertyName, oldValue, newValue)
				End Sub
			End Class

		End Class ' innerclass AccessibleJComboBox
	End Class

End Namespace
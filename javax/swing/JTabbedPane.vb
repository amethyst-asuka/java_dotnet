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
	''' A component that lets the user switch between a group of components by
	''' clicking on a tab with a given title and/or icon.
	''' For examples and information on using tabbed panes see
	''' <a href="https://docs.oracle.com/javase/tutorial/uiswing/components/tabbedpane.html">How to Use Tabbed Panes</a>,
	''' a section in <em>The Java Tutorial</em>.
	''' <p>
	''' Tabs/components are added to a <code>TabbedPane</code> object by using the
	''' <code>addTab</code> and <code>insertTab</code> methods.
	''' A tab is represented by an index corresponding
	''' to the position it was added in, where the first tab has an index equal to 0
	''' and the last tab has an index equal to the tab count minus 1.
	''' <p>
	''' The <code>TabbedPane</code> uses a <code>SingleSelectionModel</code>
	''' to represent the set
	''' of tab indices and the currently selected index.  If the tab count
	''' is greater than 0, then there will always be a selected index, which
	''' by default will be initialized to the first tab.  If the tab count is
	''' 0, then the selected index will be -1.
	''' <p>
	''' The tab title can be rendered by a <code>Component</code>.
	''' For example, the following produce similar results:
	''' <pre>
	''' // In this case the look and feel renders the title for the tab.
	''' tabbedPane.addTab("Tab", myComponent);
	''' // In this case the custom component is responsible for rendering the
	''' // title of the tab.
	''' tabbedPane.addTab(null, myComponent);
	''' tabbedPane.setTabComponentAt(0, new JLabel("Tab"));
	''' </pre>
	''' The latter is typically used when you want a more complex user interaction
	''' that requires custom components on the tab.  For example, you could
	''' provide a custom component that animates or one that has widgets for
	''' closing the tab.
	''' <p>
	''' If you specify a component for a tab, the <code>JTabbedPane</code>
	''' will not render any text or icon you have specified for the tab.
	''' <p>
	''' <strong>Note:</strong>
	''' Do not use <code>setVisible</code> directly on a tab component to make it visible,
	''' use <code>setSelectedComponent</code> or <code>setSelectedIndex</code> methods instead.
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
	'''      attribute: isContainer true
	'''    description: A component which provides a tab folder metaphor for
	'''                 displaying one component from a set of components.
	''' 
	''' @author Dave Moore
	''' @author Philip Milne
	''' @author Amy Fowler
	''' </summary>
	''' <seealso cref= SingleSelectionModel </seealso>
	<Serializable> _
	Public Class JTabbedPane
		Inherits JComponent
		Implements Accessible, SwingConstants

	   ''' <summary>
	   ''' The tab layout policy for wrapping tabs in multiple runs when all
	   ''' tabs will not fit within a single run.
	   ''' </summary>
		Public Const WRAP_TAB_LAYOUT As Integer = 0

	   ''' <summary>
	   ''' Tab layout policy for providing a subset of available tabs when all
	   ''' the tabs will not fit within a single run.  If all the tabs do
	   ''' not fit within a single run the look and feel will provide a way
	   ''' to navigate to hidden tabs.
	   ''' </summary>
		Public Const SCROLL_TAB_LAYOUT As Integer = 1


		''' <seealso cref= #getUIClassID </seealso>
		''' <seealso cref= #readObject </seealso>
		Private Const uiClassID As String = "TabbedPaneUI"

		''' <summary>
		''' Where the tabs are placed. </summary>
		''' <seealso cref= #setTabPlacement </seealso>
		Protected Friend tabPlacement As Integer = TOP

		Private tabLayoutPolicy As Integer

		''' <summary>
		''' The default selection model </summary>
		Protected Friend model As SingleSelectionModel

		Private haveRegistered As Boolean

		''' <summary>
		''' The <code>changeListener</code> is the listener we add to the
		''' model.
		''' </summary>
		Protected Friend changeListener As ChangeListener = Nothing

		Private ReadOnly pages As IList(Of Page)

		' The component that is currently visible 
		Private visComp As Component = Nothing

		''' <summary>
		''' Only one <code>ChangeEvent</code> is needed per <code>TabPane</code>
		''' instance since the
		''' event's only (read-only) state is the source property.  The source
		''' of events generated here is always "this".
		''' </summary>
		<NonSerialized> _
		Protected Friend changeEvent As ChangeEvent = Nothing

		''' <summary>
		''' Creates an empty <code>TabbedPane</code> with a default
		''' tab placement of <code>JTabbedPane.TOP</code>. </summary>
		''' <seealso cref= #addTab </seealso>
		Public Sub New()
			Me.New(TOP, WRAP_TAB_LAYOUT)
		End Sub

		''' <summary>
		''' Creates an empty <code>TabbedPane</code> with the specified tab placement
		''' of either: <code>JTabbedPane.TOP</code>, <code>JTabbedPane.BOTTOM</code>,
		''' <code>JTabbedPane.LEFT</code>, or <code>JTabbedPane.RIGHT</code>.
		''' </summary>
		''' <param name="tabPlacement"> the placement for the tabs relative to the content </param>
		''' <seealso cref= #addTab </seealso>
		Public Sub New(ByVal tabPlacement As Integer)
			Me.New(tabPlacement, WRAP_TAB_LAYOUT)
		End Sub

		''' <summary>
		''' Creates an empty <code>TabbedPane</code> with the specified tab placement
		''' and tab layout policy.  Tab placement may be either:
		''' <code>JTabbedPane.TOP</code>, <code>JTabbedPane.BOTTOM</code>,
		''' <code>JTabbedPane.LEFT</code>, or <code>JTabbedPane.RIGHT</code>.
		''' Tab layout policy may be either: <code>JTabbedPane.WRAP_TAB_LAYOUT</code>
		''' or <code>JTabbedPane.SCROLL_TAB_LAYOUT</code>.
		''' </summary>
		''' <param name="tabPlacement"> the placement for the tabs relative to the content </param>
		''' <param name="tabLayoutPolicy"> the policy for laying out tabs when all tabs will not fit on one run </param>
		''' <exception cref="IllegalArgumentException"> if tab placement or tab layout policy are not
		'''            one of the above supported values </exception>
		''' <seealso cref= #addTab
		''' @since 1.4 </seealso>
		Public Sub New(ByVal tabPlacement As Integer, ByVal tabLayoutPolicy As Integer)
			tabPlacement = tabPlacement
			tabLayoutPolicy = tabLayoutPolicy
			pages = New List(Of Page)(1)
			model = New DefaultSingleSelectionModel
			updateUI()
		End Sub

		''' <summary>
		''' Returns the UI object which implements the L&amp;F for this component.
		''' </summary>
		''' <returns> a <code>TabbedPaneUI</code> object </returns>
		''' <seealso cref= #setUI </seealso>
		Public Overridable Property uI As TabbedPaneUI
			Get
				Return CType(ui, TabbedPaneUI)
			End Get
			Set(ByVal ui As TabbedPaneUI)
				MyBase.uI = ui
				' disabled icons are generated by LF so they should be unset here
				For i As Integer = 0 To tabCount - 1
					Dim icon As Icon = pages(i).disabledIcon
					If TypeOf icon Is UIResource Then disabledIconAtnAt(i, Nothing)
				Next i
			End Set
		End Property


		''' <summary>
		''' Resets the UI property to a value from the current look and feel.
		''' </summary>
		''' <seealso cref= JComponent#updateUI </seealso>
		Public Overrides Sub updateUI()
			uI = CType(UIManager.getUI(Me), TabbedPaneUI)
		End Sub


		''' <summary>
		''' Returns the name of the UI class that implements the
		''' L&amp;F for this component.
		''' </summary>
		''' <returns> the string "TabbedPaneUI" </returns>
		''' <seealso cref= JComponent#getUIClassID </seealso>
		''' <seealso cref= UIDefaults#getUI </seealso>
		Public Property Overrides uIClassID As String
			Get
				Return uiClassID
			End Get
		End Property


		''' <summary>
		''' We pass <code>ModelChanged</code> events along to the listeners with
		''' the tabbedpane (instead of the model itself) as the event source.
		''' </summary>
		<Serializable> _
		Protected Friend Class ModelListener
			Implements ChangeListener

			Private ReadOnly outerInstance As JTabbedPane

			Public Sub New(ByVal outerInstance As JTabbedPane)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Sub stateChanged(ByVal e As ChangeEvent) Implements ChangeListener.stateChanged
				outerInstance.fireStateChanged()
			End Sub
		End Class

		''' <summary>
		''' Subclasses that want to handle <code>ChangeEvents</code> differently
		''' can override this to return a subclass of <code>ModelListener</code> or
		''' another <code>ChangeListener</code> implementation.
		''' </summary>
		''' <seealso cref= #fireStateChanged </seealso>
		Protected Friend Overridable Function createChangeListener() As ChangeListener
			Return New ModelListener(Me)
		End Function

		''' <summary>
		''' Adds a <code>ChangeListener</code> to this tabbedpane.
		''' </summary>
		''' <param name="l"> the <code>ChangeListener</code> to add </param>
		''' <seealso cref= #fireStateChanged </seealso>
		''' <seealso cref= #removeChangeListener </seealso>
		Public Overridable Sub addChangeListener(ByVal l As ChangeListener)
			listenerList.add(GetType(ChangeListener), l)
		End Sub

		''' <summary>
		''' Removes a <code>ChangeListener</code> from this tabbedpane.
		''' </summary>
		''' <param name="l"> the <code>ChangeListener</code> to remove </param>
		''' <seealso cref= #fireStateChanged </seealso>
		''' <seealso cref= #addChangeListener </seealso>
		Public Overridable Sub removeChangeListener(ByVal l As ChangeListener)
			listenerList.remove(GetType(ChangeListener), l)
		End Sub

	   ''' <summary>
	   ''' Returns an array of all the <code>ChangeListener</code>s added
	   ''' to this <code>JTabbedPane</code> with <code>addChangeListener</code>.
	   ''' </summary>
	   ''' <returns> all of the <code>ChangeListener</code>s added or an empty
	   '''         array if no listeners have been added
	   ''' @since 1.4 </returns>
		Public Overridable Property changeListeners As ChangeListener()
			Get
				Return listenerList.getListeners(GetType(ChangeListener))
			End Get
		End Property

		''' <summary>
		''' Sends a {@code ChangeEvent}, with this {@code JTabbedPane} as the source,
		''' to each registered listener. This method is called each time there is
		''' a change to either the selected index or the selected tab in the
		''' {@code JTabbedPane}. Usually, the selected index and selected tab change
		''' together. However, there are some cases, such as tab addition, where the
		''' selected index changes and the same tab remains selected. There are other
		''' cases, such as deleting the selected tab, where the index remains the
		''' same, but a new tab moves to that index. Events are fired for all of
		''' these cases.
		''' </summary>
		''' <seealso cref= #addChangeListener </seealso>
		''' <seealso cref= EventListenerList </seealso>
		Protected Friend Overridable Sub fireStateChanged()
			' --- Begin code to deal with visibility --- 

	'         This code deals with changing the visibility of components to
	'         * hide and show the contents for the selected tab. It duplicates
	'         * logic already present in BasicTabbedPaneUI, logic that is
	'         * processed during the layout pass. This code exists to allow
	'         * developers to do things that are quite difficult to accomplish
	'         * with the previous model of waiting for the layout pass to process
	'         * visibility changes; such as requesting focus on the new visible
	'         * component.
	'         *
	'         * For the average code, using the typical JTabbedPane methods,
	'         * all visibility changes will now be processed here. However,
	'         * the code in BasicTabbedPaneUI still exists, for the purposes
	'         * of backward compatibility. Therefore, when making changes to
	'         * this code, ensure that the BasicTabbedPaneUI code is kept in
	'         * synch.
	'         

			Dim selIndex As Integer = selectedIndex

			' if the selection is now nothing 
			If selIndex < 0 Then
				' if there was a previous visible component 
				If visComp IsNot Nothing AndAlso visComp.visible Then visComp.visible = False

				' now there's no visible component 
				visComp = Nothing

			' else - the selection is now something 
			Else
				' Fetch the component for the new selection 
				Dim newComp As Component = getComponentAt(selIndex)

				' if the new component is non-null and different 
				If newComp IsNot Nothing AndAlso newComp IsNot visComp Then
					Dim shouldChangeFocus As Boolean = False

	'                 Note: the following (clearing of the old visible component)
	'                 * is inside this if-statement for good reason: Tabbed pane
	'                 * should continue to show the previously visible component
	'                 * if there is no component for the chosen tab.
	'                 

					' if there was a previous visible component 
					If visComp IsNot Nothing Then
						shouldChangeFocus = (SwingUtilities.findFocusOwner(visComp) IsNot Nothing)

						' if it's still visible 
						If visComp.visible Then visComp.visible = False
					End If

					If Not newComp.visible Then newComp.visible = True

					If shouldChangeFocus Then sun.swing.SwingUtilities2.tabbedPaneChangeFocusTo(newComp)

					visComp = newComp
				End If ' else - the visible component shouldn't changed
			End If

			' --- End code to deal with visibility --- 

			' Guaranteed to return a non-null array
			Dim ___listeners As Object() = listenerList.listenerList
			' Process the listeners last to first, notifying
			' those that are interested in this event
			For i As Integer = ___listeners.Length-2 To 0 Step -2
				If ___listeners(i) Is GetType(ChangeListener) Then
					' Lazily create the event:
					If changeEvent Is Nothing Then changeEvent = New ChangeEvent(Me)
					CType(___listeners(i+1), ChangeListener).stateChanged(changeEvent)
				End If
			Next i
		End Sub

		''' <summary>
		''' Returns the model associated with this tabbedpane.
		''' </summary>
		''' <seealso cref= #setModel </seealso>
		Public Overridable Property model As SingleSelectionModel
			Get
				Return model
			End Get
			Set(ByVal model As SingleSelectionModel)
				Dim oldModel As SingleSelectionModel = model
    
				If oldModel IsNot Nothing Then
					oldModel.removeChangeListener(changeListener)
					changeListener = Nothing
				End If
    
				Me.model = model
    
				If model IsNot Nothing Then
					changeListener = createChangeListener()
					model.addChangeListener(changeListener)
				End If
    
				firePropertyChange("model", oldModel, model)
				repaint()
			End Set
		End Property


		''' <summary>
		''' Returns the placement of the tabs for this tabbedpane. </summary>
		''' <seealso cref= #setTabPlacement </seealso>
		Public Overridable Property tabPlacement As Integer
			Get
				Return tabPlacement
			End Get
			Set(ByVal tabPlacement As Integer)
				If tabPlacement <> TOP AndAlso tabPlacement <> LEFT AndAlso tabPlacement <> BOTTOM AndAlso tabPlacement <> RIGHT Then Throw New System.ArgumentException("illegal tab placement: must be TOP, BOTTOM, LEFT, or RIGHT")
				If Me.tabPlacement <> tabPlacement Then
					Dim oldValue As Integer = Me.tabPlacement
					Me.tabPlacement = tabPlacement
					firePropertyChange("tabPlacement", oldValue, tabPlacement)
					revalidate()
					repaint()
				End If
			End Set
		End Property


		''' <summary>
		''' Returns the policy used by the tabbedpane to layout the tabs when all the
		''' tabs will not fit within a single run. </summary>
		''' <seealso cref= #setTabLayoutPolicy
		''' @since 1.4 </seealso>
		Public Overridable Property tabLayoutPolicy As Integer
			Get
				Return tabLayoutPolicy
			End Get
			Set(ByVal tabLayoutPolicy As Integer)
				If tabLayoutPolicy <> WRAP_TAB_LAYOUT AndAlso tabLayoutPolicy <> SCROLL_TAB_LAYOUT Then Throw New System.ArgumentException("illegal tab layout policy: must be WRAP_TAB_LAYOUT or SCROLL_TAB_LAYOUT")
				If Me.tabLayoutPolicy <> tabLayoutPolicy Then
					Dim oldValue As Integer = Me.tabLayoutPolicy
					Me.tabLayoutPolicy = tabLayoutPolicy
					firePropertyChange("tabLayoutPolicy", oldValue, tabLayoutPolicy)
					revalidate()
					repaint()
				End If
			End Set
		End Property


		''' <summary>
		''' Returns the currently selected index for this tabbedpane.
		''' Returns -1 if there is no currently selected tab.
		''' </summary>
		''' <returns> the index of the selected tab </returns>
		''' <seealso cref= #setSelectedIndex </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Property selectedIndex As Integer
			Get
				Return model.selectedIndex
			End Get
			Set(ByVal index As Integer)
				If index <> -1 Then checkIndex(index)
				selectedIndexImplmpl(index, True)
			End Set
		End Property



		Private Sub setSelectedIndexImpl(ByVal index As Integer, ByVal doAccessibleChanges As Boolean)
			Dim oldIndex As Integer = model.selectedIndex
			Dim oldPage As Page = Nothing, newPage As Page = Nothing
			Dim oldName As String = Nothing

			doAccessibleChanges = doAccessibleChanges AndAlso (oldIndex <> index)

			If doAccessibleChanges Then
				If accessibleContext IsNot Nothing Then oldName = accessibleContext.accessibleName

				If oldIndex >= 0 Then oldPage = pages(oldIndex)

				If index >= 0 Then newPage = pages(index)
			End If

			model.selectedIndex = index

			If doAccessibleChanges Then changeAccessibleSelection(oldPage, oldName, newPage)
		End Sub

		Private Sub changeAccessibleSelection(ByVal oldPage As Page, ByVal oldName As String, ByVal newPage As Page)
			If accessibleContext Is Nothing Then Return

			If oldPage IsNot Nothing Then oldPage.firePropertyChange(AccessibleContext.ACCESSIBLE_STATE_PROPERTY, AccessibleState.SELECTED, Nothing)

			If newPage IsNot Nothing Then newPage.firePropertyChange(AccessibleContext.ACCESSIBLE_STATE_PROPERTY, Nothing, AccessibleState.SELECTED)

			accessibleContext.firePropertyChange(AccessibleContext.ACCESSIBLE_NAME_PROPERTY, oldName, accessibleContext.accessibleName)
		End Sub

		''' <summary>
		''' Returns the currently selected component for this tabbedpane.
		''' Returns <code>null</code> if there is no currently selected tab.
		''' </summary>
		''' <returns> the component corresponding to the selected tab </returns>
		''' <seealso cref= #setSelectedComponent </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Overridable Property selectedComponent As Component
			Get
				Dim index As Integer = selectedIndex
				If index = -1 Then Return Nothing
				Return getComponentAt(index)
			End Get
			Set(ByVal c As Component)
				Dim index As Integer = indexOfComponent(c)
				If index <> -1 Then
					selectedIndex = index
				Else
					Throw New System.ArgumentException("component not found in tabbed pane")
				End If
			End Set
		End Property


		''' <summary>
		''' Inserts a new tab for the given component, at the given index,
		''' represented by the given title and/or icon, either of which may
		''' be {@code null}.
		''' </summary>
		''' <param name="title"> the title to be displayed on the tab </param>
		''' <param name="icon"> the icon to be displayed on the tab </param>
		''' <param name="component"> the component to be displayed when this tab is clicked. </param>
		''' <param name="tip"> the tooltip to be displayed for this tab </param>
		''' <param name="index"> the position to insert this new tab
		'''       ({@code > 0 and <= getTabCount()})
		''' </param>
		''' <exception cref="IndexOutOfBoundsException"> if the index is out of range
		'''         ({@code < 0 or > getTabCount()})
		''' </exception>
		''' <seealso cref= #addTab </seealso>
		''' <seealso cref= #removeTabAt </seealso>
		Public Overridable Sub insertTab(ByVal title As String, ByVal icon As Icon, ByVal component As Component, ByVal tip As String, ByVal index As Integer)
			Dim newIndex As Integer = index

			' If component already exists, remove corresponding
			' tab so that new tab gets added correctly
			' Note: we are allowing component=null because of compatibility,
			' but we really should throw an exception because much of the
			' rest of the JTabbedPane implementation isn't designed to deal
			' with null components for tabs.
			Dim removeIndex As Integer = indexOfComponent(component)
			If component IsNot Nothing AndAlso removeIndex <> -1 Then
				removeTabAt(removeIndex)
				If newIndex > removeIndex Then newIndex -= 1
			End If

			Dim ___selectedIndex As Integer = selectedIndex

			pages.Insert(newIndex, New Page(Me, Me,If(title IsNot Nothing, title, ""), icon, Nothing, component, tip))


			If component IsNot Nothing Then
				addImpl(component, Nothing, -1)
				component.visible = False
			Else
				firePropertyChange("indexForNullComponent", -1, index)
			End If

			If pages.Count = 1 Then selectedIndex = 0

			If ___selectedIndex >= newIndex Then selectedIndexImplmpl(___selectedIndex + 1, False)

			If (Not haveRegistered) AndAlso tip IsNot Nothing Then
				ToolTipManager.sharedInstance().registerComponent(Me)
				haveRegistered = True
			End If

			If accessibleContext IsNot Nothing Then accessibleContext.firePropertyChange(AccessibleContext.ACCESSIBLE_VISIBLE_DATA_PROPERTY, Nothing, component)
			revalidate()
			repaint()
		End Sub

		''' <summary>
		''' Adds a <code>component</code> and <code>tip</code>
		''' represented by a <code>title</code> and/or <code>icon</code>,
		''' either of which can be <code>null</code>.
		''' Cover method for <code>insertTab</code>.
		''' </summary>
		''' <param name="title"> the title to be displayed in this tab </param>
		''' <param name="icon"> the icon to be displayed in this tab </param>
		''' <param name="component"> the component to be displayed when this tab is clicked </param>
		''' <param name="tip"> the tooltip to be displayed for this tab
		''' </param>
		''' <seealso cref= #insertTab </seealso>
		''' <seealso cref= #removeTabAt </seealso>
		Public Overridable Sub addTab(ByVal title As String, ByVal icon As Icon, ByVal component As Component, ByVal tip As String)
			insertTab(title, icon, component, tip, pages.Count)
		End Sub

		''' <summary>
		''' Adds a <code>component</code> represented by a <code>title</code>
		''' and/or <code>icon</code>, either of which can be <code>null</code>.
		''' Cover method for <code>insertTab</code>.
		''' </summary>
		''' <param name="title"> the title to be displayed in this tab </param>
		''' <param name="icon"> the icon to be displayed in this tab </param>
		''' <param name="component"> the component to be displayed when this tab is clicked
		''' </param>
		''' <seealso cref= #insertTab </seealso>
		''' <seealso cref= #removeTabAt </seealso>
		Public Overridable Sub addTab(ByVal title As String, ByVal icon As Icon, ByVal component As Component)
			insertTab(title, icon, component, Nothing, pages.Count)
		End Sub

		''' <summary>
		''' Adds a <code>component</code> represented by a <code>title</code>
		''' and no icon.
		''' Cover method for <code>insertTab</code>.
		''' </summary>
		''' <param name="title"> the title to be displayed in this tab </param>
		''' <param name="component"> the component to be displayed when this tab is clicked
		''' </param>
		''' <seealso cref= #insertTab </seealso>
		''' <seealso cref= #removeTabAt </seealso>
		Public Overridable Sub addTab(ByVal title As String, ByVal component As Component)
			insertTab(title, Nothing, component, Nothing, pages.Count)
		End Sub

		''' <summary>
		''' Adds a <code>component</code> with a tab title defaulting to
		''' the name of the component which is the result of calling
		''' <code>component.getName</code>.
		''' Cover method for <code>insertTab</code>.
		''' </summary>
		''' <param name="component"> the component to be displayed when this tab is clicked </param>
		''' <returns> the component
		''' </returns>
		''' <seealso cref= #insertTab </seealso>
		''' <seealso cref= #removeTabAt </seealso>
		Public Overridable Function add(ByVal component As Component) As Component
			If Not(TypeOf component Is UIResource) Then
				addTab(component.name, component)
			Else
				MyBase.add(component)
			End If
			Return component
		End Function

		''' <summary>
		''' Adds a <code>component</code> with the specified tab title.
		''' Cover method for <code>insertTab</code>.
		''' </summary>
		''' <param name="title"> the title to be displayed in this tab </param>
		''' <param name="component"> the component to be displayed when this tab is clicked </param>
		''' <returns> the component
		''' </returns>
		''' <seealso cref= #insertTab </seealso>
		''' <seealso cref= #removeTabAt </seealso>
		Public Overridable Function add(ByVal title As String, ByVal component As Component) As Component
			If Not(TypeOf component Is UIResource) Then
				addTab(title, component)
			Else
				MyBase.add(title, component)
			End If
			Return component
		End Function

		''' <summary>
		''' Adds a <code>component</code> at the specified tab index with a tab
		''' title defaulting to the name of the component.
		''' Cover method for <code>insertTab</code>.
		''' </summary>
		''' <param name="component"> the component to be displayed when this tab is clicked </param>
		''' <param name="index"> the position to insert this new tab </param>
		''' <returns> the component
		''' </returns>
		''' <seealso cref= #insertTab </seealso>
		''' <seealso cref= #removeTabAt </seealso>
		Public Overridable Function add(ByVal component As Component, ByVal index As Integer) As Component
			If Not(TypeOf component Is UIResource) Then
				' Container.add() interprets -1 as "append", so convert
				' the index appropriately to be handled by the vector
				insertTab(component.name, Nothing, component, Nothing,If(index = -1, tabCount, index))
			Else
				MyBase.add(component, index)
			End If
			Return component
		End Function

		''' <summary>
		''' Adds a <code>component</code> to the tabbed pane.
		''' If <code>constraints</code> is a <code>String</code> or an
		''' <code>Icon</code>, it will be used for the tab title,
		''' otherwise the component's name will be used as the tab title.
		''' Cover method for <code>insertTab</code>.
		''' </summary>
		''' <param name="component"> the component to be displayed when this tab is clicked </param>
		''' <param name="constraints"> the object to be displayed in the tab
		''' </param>
		''' <seealso cref= #insertTab </seealso>
		''' <seealso cref= #removeTabAt </seealso>
		Public Overridable Sub add(ByVal component As Component, ByVal constraints As Object)
			If Not(TypeOf component Is UIResource) Then
				If TypeOf constraints Is String Then
					addTab(CStr(constraints), component)
				ElseIf TypeOf constraints Is Icon Then
					addTab(Nothing, CType(constraints, Icon), component)
				Else
					add(component)
				End If
			Else
				MyBase.add(component, constraints)
			End If
		End Sub

		''' <summary>
		''' Adds a <code>component</code> at the specified tab index.
		''' If <code>constraints</code> is a <code>String</code> or an
		''' <code>Icon</code>, it will be used for the tab title,
		''' otherwise the component's name will be used as the tab title.
		''' Cover method for <code>insertTab</code>.
		''' </summary>
		''' <param name="component"> the component to be displayed when this tab is clicked </param>
		''' <param name="constraints"> the object to be displayed in the tab </param>
		''' <param name="index"> the position to insert this new tab
		''' </param>
		''' <seealso cref= #insertTab </seealso>
		''' <seealso cref= #removeTabAt </seealso>
		Public Overridable Sub add(ByVal component As Component, ByVal constraints As Object, ByVal index As Integer)
			If Not(TypeOf component Is UIResource) Then

				Dim icon As Icon = If(TypeOf constraints Is Icon, CType(constraints, Icon), Nothing)
				Dim title As String = If(TypeOf constraints Is String, CStr(constraints), Nothing)
				' Container.add() interprets -1 as "append", so convert
				' the index appropriately to be handled by the vector
				insertTab(title, icon, component, Nothing,If(index = -1, tabCount, index))
			Else
				MyBase.add(component, constraints, index)
			End If
		End Sub

		''' <summary>
		''' Removes the tab at <code>index</code>.
		''' After the component associated with <code>index</code> is removed,
		''' its visibility is reset to true to ensure it will be visible
		''' if added to other containers. </summary>
		''' <param name="index"> the index of the tab to be removed </param>
		''' <exception cref="IndexOutOfBoundsException"> if index is out of range
		'''            {@code (index < 0 || index >= tab count)}
		''' </exception>
		''' <seealso cref= #addTab </seealso>
		''' <seealso cref= #insertTab </seealso>
		Public Overridable Sub removeTabAt(ByVal index As Integer)
			checkIndex(index)

			Dim component As Component = getComponentAt(index)
			Dim shouldChangeFocus As Boolean = False
			Dim selected As Integer = selectedIndex
			Dim oldName As String = Nothing

			' if we're about to remove the visible component 
			If component Is visComp Then
				shouldChangeFocus = (SwingUtilities.findFocusOwner(visComp) IsNot Nothing)
				visComp = Nothing
			End If

			If accessibleContext IsNot Nothing Then
				' if we're removing the selected page 
				If index = selected Then
					' fire an accessible notification that it's unselected 
					pages(index).firePropertyChange(AccessibleContext.ACCESSIBLE_STATE_PROPERTY, AccessibleState.SELECTED, Nothing)

					oldName = accessibleContext.accessibleName
				End If

				accessibleContext.firePropertyChange(AccessibleContext.ACCESSIBLE_VISIBLE_DATA_PROPERTY, component, Nothing)
			End If

			' Force the tabComponent to be cleaned up.
			tabComponentAttAt(index, Nothing)
			pages.RemoveAt(index)

			' NOTE 4/15/2002 (joutwate):
			' This fix is implemented using client properties since there is
			' currently no IndexPropertyChangeEvent.  Once
			' IndexPropertyChangeEvents have been added this code should be
			' modified to use it.
			putClientProperty("__index_to_remove__", Convert.ToInt32(index))

			' if the selected tab is after the removal 
			If selected > index Then
				selectedIndexImplmpl(selected - 1, False)

			' if the selected tab is the last tab 
			ElseIf selected >= tabCount Then
				selectedIndexImplmpl(selected - 1, False)
				Dim newSelected As Page = If(selected <> 0, pages(selected - 1), Nothing)

				changeAccessibleSelection(Nothing, oldName, newSelected)

			' selected index hasn't changed, but the associated tab has 
			ElseIf index = selected Then
				fireStateChanged()
				changeAccessibleSelection(Nothing, oldName, pages(index))
			End If

			' We can't assume the tab indices correspond to the
			' container's children array indices, so make sure we
			' remove the correct child!
			If component IsNot Nothing Then
				Dim components As Component() = components
				Dim i As Integer = components.Length
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
				Do While i -= 1 >= 0
					If components(i) Is component Then
						MyBase.remove(i)
						component.visible = True
						Exit Do
					End If
				Loop
			End If

			If shouldChangeFocus Then sun.swing.SwingUtilities2.tabbedPaneChangeFocusTo(selectedComponent)

			revalidate()
			repaint()
		End Sub

		''' <summary>
		''' Removes the specified <code>Component</code> from the
		''' <code>JTabbedPane</code>. The method does nothing
		''' if the <code>component</code> is null.
		''' </summary>
		''' <param name="component"> the component to remove from the tabbedpane </param>
		''' <seealso cref= #addTab </seealso>
		''' <seealso cref= #removeTabAt </seealso>
		Public Overridable Sub remove(ByVal component As Component)
			Dim index As Integer = indexOfComponent(component)
			If index <> -1 Then
				removeTabAt(index)
			Else
				' Container#remove(comp) invokes Container#remove(int)
				' so make sure JTabbedPane#remove(int) isn't called here
				Dim children As Component() = components
				For i As Integer = 0 To children.Length - 1
					If component Is children(i) Then
						MyBase.remove(i)
						Exit For
					End If
				Next i
			End If
		End Sub

		''' <summary>
		''' Removes the tab and component which corresponds to the specified index.
		''' </summary>
		''' <param name="index"> the index of the component to remove from the
		'''          <code>tabbedpane</code> </param>
		''' <exception cref="IndexOutOfBoundsException"> if index is out of range
		'''            {@code (index < 0 || index >= tab count)} </exception>
		''' <seealso cref= #addTab </seealso>
		''' <seealso cref= #removeTabAt </seealso>
		Public Overridable Sub remove(ByVal index As Integer)
			removeTabAt(index)
		End Sub

		''' <summary>
		''' Removes all the tabs and their corresponding components
		''' from the <code>tabbedpane</code>.
		''' </summary>
		''' <seealso cref= #addTab </seealso>
		''' <seealso cref= #removeTabAt </seealso>
		Public Overridable Sub removeAll()
			selectedIndexImplmpl(-1, True)

			Dim ___tabCount As Integer = tabCount
			' We invoke removeTabAt for each tab, otherwise we may end up
			' removing Components added by the UI.
			Dim tempVar As Boolean = ___tabCount > 0
			___tabCount -= 1
			Do While tempVar
				removeTabAt(___tabCount)
				tempVar = ___tabCount > 0
				___tabCount -= 1
			Loop
		End Sub

		''' <summary>
		''' Returns the number of tabs in this <code>tabbedpane</code>.
		''' </summary>
		''' <returns> an integer specifying the number of tabbed pages </returns>
		Public Overridable Property tabCount As Integer
			Get
				Return pages.Count
			End Get
		End Property

		''' <summary>
		''' Returns the number of tab runs currently used to display
		''' the tabs. </summary>
		''' <returns> an integer giving the number of rows if the
		'''          <code>tabPlacement</code>
		'''          is <code>TOP</code> or <code>BOTTOM</code>
		'''          and the number of columns if
		'''          <code>tabPlacement</code>
		'''          is <code>LEFT</code> or <code>RIGHT</code>,
		'''          or 0 if there is no UI set on this <code>tabbedpane</code> </returns>
		Public Overridable Property tabRunCount As Integer
			Get
				If ui IsNot Nothing Then Return CType(ui, TabbedPaneUI).getTabRunCount(Me)
				Return 0
			End Get
		End Property


	' Getters for the Pages

		''' <summary>
		''' Returns the tab title at <code>index</code>.
		''' </summary>
		''' <param name="index">  the index of the item being queried </param>
		''' <returns> the title at <code>index</code> </returns>
		''' <exception cref="IndexOutOfBoundsException"> if index is out of range
		'''            {@code (index < 0 || index >= tab count)} </exception>
		''' <seealso cref= #setTitleAt </seealso>
		Public Overridable Function getTitleAt(ByVal index As Integer) As String
			Return pages(index).title
		End Function

		''' <summary>
		''' Returns the tab icon at <code>index</code>.
		''' </summary>
		''' <param name="index">  the index of the item being queried </param>
		''' <returns> the icon at <code>index</code> </returns>
		''' <exception cref="IndexOutOfBoundsException"> if index is out of range
		'''            {@code (index < 0 || index >= tab count)}
		''' </exception>
		''' <seealso cref= #setIconAt </seealso>
		Public Overridable Function getIconAt(ByVal index As Integer) As Icon
			Return pages(index).icon
		End Function

		''' <summary>
		''' Returns the tab disabled icon at <code>index</code>.
		''' If the tab disabled icon doesn't exist at <code>index</code>
		''' this will forward the call to the look and feel to construct
		''' an appropriate disabled Icon from the corresponding enabled
		''' Icon. Some look and feels might not render the disabled Icon,
		''' in which case it won't be created.
		''' </summary>
		''' <param name="index">  the index of the item being queried </param>
		''' <returns> the icon at <code>index</code> </returns>
		''' <exception cref="IndexOutOfBoundsException"> if index is out of range
		'''            {@code (index < 0 || index >= tab count)}
		''' </exception>
		''' <seealso cref= #setDisabledIconAt </seealso>
		Public Overridable Function getDisabledIconAt(ByVal index As Integer) As Icon
			Dim page As Page = pages(index)
			If page.disabledIcon Is Nothing Then page.disabledIcon = UIManager.lookAndFeel.getDisabledIcon(Me, page.icon)
			Return page.disabledIcon
		End Function

		''' <summary>
		''' Returns the tab tooltip text at <code>index</code>.
		''' </summary>
		''' <param name="index">  the index of the item being queried </param>
		''' <returns> a string containing the tool tip text at <code>index</code> </returns>
		''' <exception cref="IndexOutOfBoundsException"> if index is out of range
		'''            {@code (index < 0 || index >= tab count)}
		''' </exception>
		''' <seealso cref= #setToolTipTextAt
		''' @since 1.3 </seealso>
		Public Overridable Function getToolTipTextAt(ByVal index As Integer) As String
			Return pages(index).tip
		End Function

		''' <summary>
		''' Returns the tab background color at <code>index</code>.
		''' </summary>
		''' <param name="index">  the index of the item being queried </param>
		''' <returns> the <code>Color</code> of the tab background at
		'''          <code>index</code> </returns>
		''' <exception cref="IndexOutOfBoundsException"> if index is out of range
		'''            {@code (index < 0 || index >= tab count)}
		''' </exception>
		''' <seealso cref= #setBackgroundAt </seealso>
		Public Overridable Function getBackgroundAt(ByVal index As Integer) As Color
			Return pages(index).background
		End Function

		''' <summary>
		''' Returns the tab foreground color at <code>index</code>.
		''' </summary>
		''' <param name="index">  the index of the item being queried </param>
		''' <returns> the <code>Color</code> of the tab foreground at
		'''          <code>index</code> </returns>
		''' <exception cref="IndexOutOfBoundsException"> if index is out of range
		'''            {@code (index < 0 || index >= tab count)}
		''' </exception>
		''' <seealso cref= #setForegroundAt </seealso>
		Public Overridable Function getForegroundAt(ByVal index As Integer) As Color
			Return pages(index).foreground
		End Function

		''' <summary>
		''' Returns whether or not the tab at <code>index</code> is
		''' currently enabled.
		''' </summary>
		''' <param name="index">  the index of the item being queried </param>
		''' <returns> true if the tab at <code>index</code> is enabled;
		'''          false otherwise </returns>
		''' <exception cref="IndexOutOfBoundsException"> if index is out of range
		'''            {@code (index < 0 || index >= tab count)}
		''' </exception>
		''' <seealso cref= #setEnabledAt </seealso>
		Public Overridable Function isEnabledAt(ByVal index As Integer) As Boolean
			Return pages(index).enabled
		End Function

		''' <summary>
		''' Returns the component at <code>index</code>.
		''' </summary>
		''' <param name="index">  the index of the item being queried </param>
		''' <returns> the <code>Component</code> at <code>index</code> </returns>
		''' <exception cref="IndexOutOfBoundsException"> if index is out of range
		'''            {@code (index < 0 || index >= tab count)}
		''' </exception>
		''' <seealso cref= #setComponentAt </seealso>
		Public Overridable Function getComponentAt(ByVal index As Integer) As Component
			Return pages(index).component
		End Function

		''' <summary>
		''' Returns the keyboard mnemonic for accessing the specified tab.
		''' The mnemonic is the key which when combined with the look and feel's
		''' mouseless modifier (usually Alt) will activate the specified
		''' tab.
		''' 
		''' @since 1.4 </summary>
		''' <param name="tabIndex"> the index of the tab that the mnemonic refers to </param>
		''' <returns> the key code which represents the mnemonic;
		'''         -1 if a mnemonic is not specified for the tab </returns>
		''' <exception cref="IndexOutOfBoundsException"> if index is out of range
		'''            (<code>tabIndex</code> &lt; 0 ||
		'''              <code>tabIndex</code> &gt;= tab count) </exception>
		''' <seealso cref= #setDisplayedMnemonicIndexAt(int,int) </seealso>
		''' <seealso cref= #setMnemonicAt(int,int) </seealso>
		Public Overridable Function getMnemonicAt(ByVal tabIndex As Integer) As Integer
			checkIndex(tabIndex)

			Dim page As Page = pages(tabIndex)
			Return page.mnemonic
		End Function

		''' <summary>
		''' Returns the character, as an index, that the look and feel should
		''' provide decoration for as representing the mnemonic character.
		''' 
		''' @since 1.4 </summary>
		''' <param name="tabIndex"> the index of the tab that the mnemonic refers to </param>
		''' <returns> index representing mnemonic character if one exists;
		'''    otherwise returns -1 </returns>
		''' <exception cref="IndexOutOfBoundsException"> if index is out of range
		'''            (<code>tabIndex</code> &lt; 0 ||
		'''              <code>tabIndex</code> &gt;= tab count) </exception>
		''' <seealso cref= #setDisplayedMnemonicIndexAt(int,int) </seealso>
		''' <seealso cref= #setMnemonicAt(int,int) </seealso>
		Public Overridable Function getDisplayedMnemonicIndexAt(ByVal tabIndex As Integer) As Integer
			checkIndex(tabIndex)

			Dim page As Page = pages(tabIndex)
			Return page.displayedMnemonicIndex
		End Function

		''' <summary>
		''' Returns the tab bounds at <code>index</code>.  If the tab at
		''' this index is not currently visible in the UI, then returns
		''' <code>null</code>.
		''' If there is no UI set on this <code>tabbedpane</code>,
		''' then returns <code>null</code>.
		''' </summary>
		''' <param name="index"> the index to be queried </param>
		''' <returns> a <code>Rectangle</code> containing the tab bounds at
		'''          <code>index</code>, or <code>null</code> if tab at
		'''          <code>index</code> is not currently visible in the UI,
		'''          or if there is no UI set on this <code>tabbedpane</code> </returns>
		''' <exception cref="IndexOutOfBoundsException"> if index is out of range
		'''            {@code (index < 0 || index >= tab count)} </exception>
		Public Overridable Function getBoundsAt(ByVal index As Integer) As Rectangle
			checkIndex(index)
			If ui IsNot Nothing Then Return CType(ui, TabbedPaneUI).getTabBounds(Me, index)
			Return Nothing
		End Function


	' Setters for the Pages

		''' <summary>
		''' Sets the title at <code>index</code> to <code>title</code> which
		''' can be <code>null</code>.
		''' The title is not shown if a tab component for this tab was specified.
		''' An internal exception is raised if there is no tab at that index.
		''' </summary>
		''' <param name="index"> the tab index where the title should be set </param>
		''' <param name="title"> the title to be displayed in the tab </param>
		''' <exception cref="IndexOutOfBoundsException"> if index is out of range
		'''            {@code (index < 0 || index >= tab count)}
		''' </exception>
		''' <seealso cref= #getTitleAt </seealso>
		''' <seealso cref= #setTabComponentAt
		''' @beaninfo
		'''    preferred: true
		'''    attribute: visualUpdate true
		'''  description: The title at the specified tab index. </seealso>
		Public Overridable Sub setTitleAt(ByVal index As Integer, ByVal title As String)
			Dim page As Page = pages(index)
			Dim oldTitle As String =page.title
			page.title = title

			If oldTitle <> title Then firePropertyChange("indexForTitle", -1, index)
			page.updateDisplayedMnemonicIndex()
			If (oldTitle <> title) AndAlso (accessibleContext IsNot Nothing) Then accessibleContext.firePropertyChange(AccessibleContext.ACCESSIBLE_VISIBLE_DATA_PROPERTY, oldTitle, title)
			If title Is Nothing OrElse oldTitle Is Nothing OrElse (Not title.Equals(oldTitle)) Then
				revalidate()
				repaint()
			End If
		End Sub

		''' <summary>
		''' Sets the icon at <code>index</code> to <code>icon</code> which can be
		''' <code>null</code>. This does not set disabled icon at <code>icon</code>.
		''' If the new Icon is different than the current Icon and disabled icon
		''' is not explicitly set, the LookAndFeel will be asked to generate a disabled
		''' Icon. To explicitly set disabled icon, use <code>setDisableIconAt()</code>.
		''' The icon is not shown if a tab component for this tab was specified.
		''' An internal exception is raised if there is no tab at that index.
		''' </summary>
		''' <param name="index"> the tab index where the icon should be set </param>
		''' <param name="icon"> the icon to be displayed in the tab </param>
		''' <exception cref="IndexOutOfBoundsException"> if index is out of range
		'''            {@code (index < 0 || index >= tab count)}
		''' </exception>
		''' <seealso cref= #setDisabledIconAt </seealso>
		''' <seealso cref= #getIconAt </seealso>
		''' <seealso cref= #getDisabledIconAt </seealso>
		''' <seealso cref= #setTabComponentAt
		''' @beaninfo
		'''    preferred: true
		'''    attribute: visualUpdate true
		'''  description: The icon at the specified tab index. </seealso>
		Public Overridable Sub setIconAt(ByVal index As Integer, ByVal icon As Icon)
			Dim page As Page = pages(index)
			Dim oldIcon As Icon = page.icon
			If icon IsNot oldIcon Then
				page.icon = icon

	'             If the default icon has really changed and we had
	'             * generated the disabled icon for this page, then
	'             * clear the disabledIcon field of the page.
	'             
				If TypeOf page.disabledIcon Is UIResource Then page.disabledIcon = Nothing

				' Fire the accessibility Visible data change
				If accessibleContext IsNot Nothing Then accessibleContext.firePropertyChange(AccessibleContext.ACCESSIBLE_VISIBLE_DATA_PROPERTY, oldIcon, icon)
				revalidate()
				repaint()
			End If
		End Sub

		''' <summary>
		''' Sets the disabled icon at <code>index</code> to <code>icon</code>
		''' which can be <code>null</code>.
		''' An internal exception is raised if there is no tab at that index.
		''' </summary>
		''' <param name="index"> the tab index where the disabled icon should be set </param>
		''' <param name="disabledIcon"> the icon to be displayed in the tab when disabled </param>
		''' <exception cref="IndexOutOfBoundsException"> if index is out of range
		'''            {@code (index < 0 || index >= tab count)}
		''' </exception>
		''' <seealso cref= #getDisabledIconAt
		''' @beaninfo
		'''    preferred: true
		'''    attribute: visualUpdate true
		'''  description: The disabled icon at the specified tab index. </seealso>
		Public Overridable Sub setDisabledIconAt(ByVal index As Integer, ByVal disabledIcon As Icon)
			Dim oldIcon As Icon = pages(index).disabledIcon
			pages(index).disabledIcon = disabledIcon
			If disabledIcon IsNot oldIcon AndAlso (Not isEnabledAt(index)) Then
				revalidate()
				repaint()
			End If
		End Sub

		''' <summary>
		''' Sets the tooltip text at <code>index</code> to <code>toolTipText</code>
		''' which can be <code>null</code>.
		''' An internal exception is raised if there is no tab at that index.
		''' </summary>
		''' <param name="index"> the tab index where the tooltip text should be set </param>
		''' <param name="toolTipText"> the tooltip text to be displayed for the tab </param>
		''' <exception cref="IndexOutOfBoundsException"> if index is out of range
		'''            {@code (index < 0 || index >= tab count)}
		''' </exception>
		''' <seealso cref= #getToolTipTextAt
		''' @beaninfo
		'''    preferred: true
		'''  description: The tooltip text at the specified tab index.
		''' @since 1.3 </seealso>
		Public Overridable Sub setToolTipTextAt(ByVal index As Integer, ByVal toolTipText As String)
			Dim oldToolTipText As String = pages(index).tip
			pages(index).tip = toolTipText

			If (oldToolTipText <> toolTipText) AndAlso (accessibleContext IsNot Nothing) Then accessibleContext.firePropertyChange(AccessibleContext.ACCESSIBLE_VISIBLE_DATA_PROPERTY, oldToolTipText, toolTipText)
			If (Not haveRegistered) AndAlso toolTipText IsNot Nothing Then
				ToolTipManager.sharedInstance().registerComponent(Me)
				haveRegistered = True
			End If
		End Sub

		''' <summary>
		''' Sets the background color at <code>index</code> to
		''' <code>background</code>
		''' which can be <code>null</code>, in which case the tab's background color
		''' will default to the background color of the <code>tabbedpane</code>.
		''' An internal exception is raised if there is no tab at that index.
		''' <p>
		''' It is up to the look and feel to honor this property, some may
		''' choose to ignore it.
		''' </summary>
		''' <param name="index"> the tab index where the background should be set </param>
		''' <param name="background"> the color to be displayed in the tab's background </param>
		''' <exception cref="IndexOutOfBoundsException"> if index is out of range
		'''            {@code (index < 0 || index >= tab count)}
		''' </exception>
		''' <seealso cref= #getBackgroundAt
		''' @beaninfo
		'''    preferred: true
		'''    attribute: visualUpdate true
		'''  description: The background color at the specified tab index. </seealso>
		Public Overridable Sub setBackgroundAt(ByVal index As Integer, ByVal background As Color)
			Dim oldBg As Color = pages(index).background
			pages(index).background = background
			If background Is Nothing OrElse oldBg Is Nothing OrElse (Not background.Equals(oldBg)) Then
				Dim tabBounds As Rectangle = getBoundsAt(index)
				If tabBounds IsNot Nothing Then repaint(tabBounds)
			End If
		End Sub

		''' <summary>
		''' Sets the foreground color at <code>index</code> to
		''' <code>foreground</code> which can be
		''' <code>null</code>, in which case the tab's foreground color
		''' will default to the foreground color of this <code>tabbedpane</code>.
		''' An internal exception is raised if there is no tab at that index.
		''' <p>
		''' It is up to the look and feel to honor this property, some may
		''' choose to ignore it.
		''' </summary>
		''' <param name="index"> the tab index where the foreground should be set </param>
		''' <param name="foreground"> the color to be displayed as the tab's foreground </param>
		''' <exception cref="IndexOutOfBoundsException"> if index is out of range
		'''            {@code (index < 0 || index >= tab count)}
		''' </exception>
		''' <seealso cref= #getForegroundAt
		''' @beaninfo
		'''    preferred: true
		'''    attribute: visualUpdate true
		'''  description: The foreground color at the specified tab index. </seealso>
		Public Overridable Sub setForegroundAt(ByVal index As Integer, ByVal foreground As Color)
			Dim oldFg As Color = pages(index).foreground
			pages(index).foreground = foreground
			If foreground Is Nothing OrElse oldFg Is Nothing OrElse (Not foreground.Equals(oldFg)) Then
				Dim tabBounds As Rectangle = getBoundsAt(index)
				If tabBounds IsNot Nothing Then repaint(tabBounds)
			End If
		End Sub

		''' <summary>
		''' Sets whether or not the tab at <code>index</code> is enabled.
		''' An internal exception is raised if there is no tab at that index.
		''' </summary>
		''' <param name="index"> the tab index which should be enabled/disabled </param>
		''' <param name="enabled"> whether or not the tab should be enabled </param>
		''' <exception cref="IndexOutOfBoundsException"> if index is out of range
		'''            {@code (index < 0 || index >= tab count)}
		''' </exception>
		''' <seealso cref= #isEnabledAt </seealso>
		Public Overridable Sub setEnabledAt(ByVal index As Integer, ByVal enabled As Boolean)
			Dim oldEnabled As Boolean = pages(index).enabled
			pages(index).enabled = enabled
			If enabled <> oldEnabled Then
				revalidate()
				repaint()
			End If
		End Sub

		''' <summary>
		''' Sets the component at <code>index</code> to <code>component</code>.
		''' An internal exception is raised if there is no tab at that index.
		''' </summary>
		''' <param name="index"> the tab index where this component is being placed </param>
		''' <param name="component"> the component for the tab </param>
		''' <exception cref="IndexOutOfBoundsException"> if index is out of range
		'''            {@code (index < 0 || index >= tab count)}
		''' </exception>
		''' <seealso cref= #getComponentAt
		''' @beaninfo
		'''    attribute: visualUpdate true
		'''  description: The component at the specified tab index. </seealso>
		Public Overridable Sub setComponentAt(ByVal index As Integer, ByVal component As Component)
			Dim page As Page = pages(index)
			If component IsNot page.component Then
				Dim shouldChangeFocus As Boolean = False

				If page.component IsNot Nothing Then
					shouldChangeFocus = (SwingUtilities.findFocusOwner(page.component) IsNot Nothing)

					' REMIND(aim): this is really silly;
					' why not if (page.component.getParent() == this) remove(component)
					SyncLock treeLock
						Dim count As Integer = componentCount
						Dim children As Component() = components
						For i As Integer = 0 To count - 1
							If children(i) Is page.component Then MyBase.remove(i)
						Next i
					End SyncLock
				End If

				page.component = component
				Dim selectedPage As Boolean = (selectedIndex = index)

				If selectedPage Then Me.visComp = component

				If component IsNot Nothing Then
					component.visible = selectedPage
					addImpl(component, Nothing, -1)

					If shouldChangeFocus Then sun.swing.SwingUtilities2.tabbedPaneChangeFocusTo(component)
				Else
					repaint()
				End If

				revalidate()
			End If
		End Sub

		''' <summary>
		''' Provides a hint to the look and feel as to which character in the
		''' text should be decorated to represent the mnemonic. Not all look and
		''' feels may support this. A value of -1 indicates either there is
		''' no mnemonic for this tab, or you do not wish the mnemonic to be
		''' displayed for this tab.
		''' <p>
		''' The value of this is updated as the properties relating to the
		''' mnemonic change (such as the mnemonic itself, the text...).
		''' You should only ever have to call this if
		''' you do not wish the default character to be underlined. For example, if
		''' the text at tab index 3 was 'Apple Price', with a mnemonic of 'p',
		''' and you wanted the 'P'
		''' to be decorated, as 'Apple <u>P</u>rice', you would have to invoke
		''' <code>setDisplayedMnemonicIndex(3, 6)</code> after invoking
		''' <code>setMnemonicAt(3, KeyEvent.VK_P)</code>.
		''' <p>Note that it is the programmer's responsibility to ensure
		''' that each tab has a unique mnemonic or unpredictable results may
		''' occur.
		''' 
		''' @since 1.4 </summary>
		''' <param name="tabIndex"> the index of the tab that the mnemonic refers to </param>
		''' <param name="mnemonicIndex"> index into the <code>String</code> to underline </param>
		''' <exception cref="IndexOutOfBoundsException"> if <code>tabIndex</code> is
		'''            out of range ({@code tabIndex < 0 || tabIndex >= tab
		'''            count}) </exception>
		''' <exception cref="IllegalArgumentException"> will be thrown if
		'''            <code>mnemonicIndex</code> is &gt;= length of the tab
		'''            title , or &lt; -1 </exception>
		''' <seealso cref= #setMnemonicAt(int,int) </seealso>
		''' <seealso cref= #getDisplayedMnemonicIndexAt(int)
		''' 
		''' @beaninfo
		'''        bound: true
		'''    attribute: visualUpdate true
		'''  description: the index into the String to draw the keyboard character
		'''               mnemonic at </seealso>
		Public Overridable Sub setDisplayedMnemonicIndexAt(ByVal tabIndex As Integer, ByVal mnemonicIndex As Integer)
			checkIndex(tabIndex)

			Dim page As Page = pages(tabIndex)

			page.displayedMnemonicIndex = mnemonicIndex
		End Sub

		''' <summary>
		''' Sets the keyboard mnemonic for accessing the specified tab.
		''' The mnemonic is the key which when combined with the look and feel's
		''' mouseless modifier (usually Alt) will activate the specified
		''' tab.
		''' <p>
		''' A mnemonic must correspond to a single key on the keyboard
		''' and should be specified using one of the <code>VK_XXX</code>
		''' keycodes defined in <code>java.awt.event.KeyEvent</code>
		''' or one of the extended keycodes obtained through
		''' <code>java.awt.event.KeyEvent.getExtendedKeyCodeForChar</code>.
		''' Mnemonics are case-insensitive, therefore a key event
		''' with the corresponding keycode would cause the button to be
		''' activated whether or not the Shift modifier was pressed.
		''' <p>
		''' This will update the displayed mnemonic property for the specified
		''' tab.
		''' 
		''' @since 1.4 </summary>
		''' <param name="tabIndex"> the index of the tab that the mnemonic refers to </param>
		''' <param name="mnemonic"> the key code which represents the mnemonic </param>
		''' <exception cref="IndexOutOfBoundsException"> if <code>tabIndex</code> is out
		'''            of range ({@code tabIndex < 0 || tabIndex >= tab count}) </exception>
		''' <seealso cref= #getMnemonicAt(int) </seealso>
		''' <seealso cref= #setDisplayedMnemonicIndexAt(int,int)
		''' 
		''' @beaninfo
		'''        bound: true
		'''    attribute: visualUpdate true
		'''  description: The keyboard mnenmonic, as a KeyEvent VK constant,
		'''               for the specified tab </seealso>
		Public Overridable Sub setMnemonicAt(ByVal tabIndex As Integer, ByVal mnemonic As Integer)
			checkIndex(tabIndex)

			Dim page As Page = pages(tabIndex)
			page.mnemonic = mnemonic

			firePropertyChange("mnemonicAt", Nothing, Nothing)
		End Sub

	' end of Page setters

		''' <summary>
		''' Returns the first tab index with a given <code>title</code>,  or
		''' -1 if no tab has this title.
		''' </summary>
		''' <param name="title"> the title for the tab </param>
		''' <returns> the first tab index which matches <code>title</code>, or
		'''          -1 if no tab has this title </returns>
		Public Overridable Function indexOfTab(ByVal title As String) As Integer
			For i As Integer = 0 To tabCount - 1
				If getTitleAt(i).Equals(If(title Is Nothing, "", title)) Then Return i
			Next i
			Return -1
		End Function

		''' <summary>
		''' Returns the first tab index with a given <code>icon</code>,
		''' or -1 if no tab has this icon.
		''' </summary>
		''' <param name="icon"> the icon for the tab </param>
		''' <returns> the first tab index which matches <code>icon</code>,
		'''          or -1 if no tab has this icon </returns>
		Public Overridable Function indexOfTab(ByVal icon As Icon) As Integer
			For i As Integer = 0 To tabCount - 1
				Dim tabIcon As Icon = getIconAt(i)
				If (tabIcon IsNot Nothing AndAlso tabIcon.Equals(icon)) OrElse (tabIcon Is Nothing AndAlso tabIcon Is icon) Then Return i
			Next i
			Return -1
		End Function

		''' <summary>
		''' Returns the index of the tab for the specified component.
		''' Returns -1 if there is no tab for this component.
		''' </summary>
		''' <param name="component"> the component for the tab </param>
		''' <returns> the first tab which matches this component, or -1
		'''          if there is no tab for this component </returns>
		Public Overridable Function indexOfComponent(ByVal component As Component) As Integer
			For i As Integer = 0 To tabCount - 1
				Dim c As Component = getComponentAt(i)
				If (c IsNot Nothing AndAlso c.Equals(component)) OrElse (c Is Nothing AndAlso c Is component) Then Return i
			Next i
			Return -1
		End Function

		''' <summary>
		''' Returns the tab index corresponding to the tab whose bounds
		''' intersect the specified location.  Returns -1 if no tab
		''' intersects the location.
		''' </summary>
		''' <param name="x"> the x location relative to this tabbedpane </param>
		''' <param name="y"> the y location relative to this tabbedpane </param>
		''' <returns> the tab index which intersects the location, or
		'''         -1 if no tab intersects the location
		''' @since 1.4 </returns>
		Public Overridable Function indexAtLocation(ByVal x As Integer, ByVal y As Integer) As Integer
			If ui IsNot Nothing Then Return CType(ui, TabbedPaneUI).tabForCoordinate(Me, x, y)
			Return -1
		End Function


		''' <summary>
		''' Returns the tooltip text for the component determined by the
		''' mouse event location.
		''' </summary>
		''' <param name="event">  the <code>MouseEvent</code> that tells where the
		'''          cursor is lingering </param>
		''' <returns> the <code>String</code> containing the tooltip text </returns>
		Public Overrides Function getToolTipText(ByVal [event] As MouseEvent) As String
			If ui IsNot Nothing Then
				Dim index As Integer = CType(ui, TabbedPaneUI).tabForCoordinate(Me, [event].x, [event].y)

				If index <> -1 Then Return pages(index).tip
			End If
			Return MyBase.getToolTipText([event])
		End Function

		Private Sub checkIndex(ByVal index As Integer)
			If index < 0 OrElse index >= pages.Count Then Throw New System.IndexOutOfRangeException("Index: " & index & ", Tab count: " & pages.Count)
		End Sub


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

	'     Called from the <code>JComponent</code>'s
	'     * <code>EnableSerializationFocusListener</code> to
	'     * do any Swing-specific pre-serialization configuration.
	'     
		Friend Overrides Sub compWriteObjectNotify()
			MyBase.compWriteObjectNotify()
			' If ToolTipText != null, then the tooltip has already been
			' unregistered by JComponent.compWriteObjectNotify()
			If toolTipText Is Nothing AndAlso haveRegistered Then ToolTipManager.sharedInstance().unregisterComponent(Me)
		End Sub

		''' <summary>
		''' See <code>readObject</code> and <code>writeObject</code> in
		''' <code>JComponent</code> for more
		''' information about serialization in Swing.
		''' </summary>
		Private Sub readObject(ByVal s As java.io.ObjectInputStream)
			s.defaultReadObject()
			If (ui IsNot Nothing) AndAlso (uIClassID.Equals(uiClassID)) Then ui.installUI(Me)
			' If ToolTipText != null, then the tooltip has already been
			' registered by JComponent.readObject()
			If toolTipText Is Nothing AndAlso haveRegistered Then ToolTipManager.sharedInstance().registerComponent(Me)
		End Sub


		''' <summary>
		''' Returns a string representation of this <code>JTabbedPane</code>.
		''' This method
		''' is intended to be used only for debugging purposes, and the
		''' content and format of the returned string may vary between
		''' implementations. The returned string may be empty but may not
		''' be <code>null</code>.
		''' </summary>
		''' <returns>  a string representation of this JTabbedPane. </returns>
		Protected Friend Overrides Function paramString() As String
			Dim tabPlacementString As String
			If tabPlacement = TOP Then
				tabPlacementString = "TOP"
			ElseIf tabPlacement = BOTTOM Then
				tabPlacementString = "BOTTOM"
			ElseIf tabPlacement = LEFT Then
				tabPlacementString = "LEFT"
			ElseIf tabPlacement = RIGHT Then
				tabPlacementString = "RIGHT"
			Else
				tabPlacementString = ""
			End If
			Dim haveRegisteredString As String = (If(haveRegistered, "true", "false"))

			Return MyBase.paramString() & ",haveRegistered=" & haveRegisteredString & ",tabPlacement=" & tabPlacementString
		End Function

	'///////////////
	' Accessibility support
	'//////////////

		''' <summary>
		''' Gets the AccessibleContext associated with this JTabbedPane.
		''' For tabbed panes, the AccessibleContext takes the form of an
		''' AccessibleJTabbedPane.
		''' A new AccessibleJTabbedPane instance is created if necessary.
		''' </summary>
		''' <returns> an AccessibleJTabbedPane that serves as the
		'''         AccessibleContext of this JTabbedPane </returns>
		Public Overridable Property accessibleContext As AccessibleContext Implements Accessible.getAccessibleContext
			Get
				If accessibleContext Is Nothing Then
					accessibleContext = New AccessibleJTabbedPane(Me)
    
					' initialize AccessibleContext for the existing pages
					Dim count As Integer = tabCount
					For i As Integer = 0 To count - 1
						pages(i).initAccessibleContext()
					Next i
				End If
				Return accessibleContext
			End Get
		End Property

		''' <summary>
		''' This class implements accessibility support for the
		''' <code>JTabbedPane</code> class.  It provides an implementation of the
		''' Java Accessibility API appropriate to tabbed pane user-interface
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
		Protected Friend Class AccessibleJTabbedPane
			Inherits AccessibleJComponent
			Implements AccessibleSelection, ChangeListener

			Private ReadOnly outerInstance As JTabbedPane


			''' <summary>
			''' Returns the accessible name of this object, or {@code null} if
			''' there is no accessible name.
			''' </summary>
			''' <returns> the accessible name of this object, nor {@code null}.
			''' @since 1.6 </returns>
			Public Overridable Property accessibleName As String
				Get
					If accessibleName IsNot Nothing Then Return accessibleName
    
					Dim cp As String = CStr(outerInstance.getClientProperty(AccessibleContext.ACCESSIBLE_NAME_PROPERTY))
    
					If cp IsNot Nothing Then Return cp
    
					Dim index As Integer = outerInstance.selectedIndex
    
					If index >= 0 Then Return outerInstance.pages(index).accessibleName
    
					Return MyBase.accessibleName
				End Get
			End Property

			''' <summary>
			'''  Constructs an AccessibleJTabbedPane
			''' </summary>
			Public Sub New(ByVal outerInstance As JTabbedPane)
					Me.outerInstance = outerInstance
				MyBase.New()
				outerInstance.model.addChangeListener(Me)
			End Sub

			Public Overridable Sub stateChanged(ByVal e As ChangeEvent) Implements ChangeListener.stateChanged
				Dim o As Object = e.source
				outerInstance.firePropertyChange(AccessibleContext.ACCESSIBLE_SELECTION_PROPERTY, Nothing, o)
			End Sub

			''' <summary>
			''' Get the role of this object.
			''' </summary>
			''' <returns> an instance of AccessibleRole describing the role of
			'''          the object </returns>
			Public Overridable Property accessibleRole As AccessibleRole
				Get
					Return AccessibleRole.PAGE_TAB_LIST
				End Get
			End Property

			''' <summary>
			''' Returns the number of accessible children in the object.
			''' </summary>
			''' <returns> the number of accessible children in the object. </returns>
			Public Overridable Property accessibleChildrenCount As Integer
				Get
					Return outerInstance.tabCount
				End Get
			End Property

			''' <summary>
			''' Return the specified Accessible child of the object.
			''' </summary>
			''' <param name="i"> zero-based index of child </param>
			''' <returns> the Accessible child of the object </returns>
			''' <exception cref="IllegalArgumentException"> if index is out of bounds </exception>
			Public Overridable Function getAccessibleChild(ByVal i As Integer) As Accessible
				If i < 0 OrElse i >= outerInstance.tabCount Then Return Nothing
				Return outerInstance.pages(i)
			End Function

			''' <summary>
			''' Gets the <code>AccessibleSelection</code> associated with
			''' this object.  In the implementation of the Java
			''' Accessibility API for this class,
			''' returns this object, which is responsible for implementing the
			''' <code>AccessibleSelection</code> interface on behalf of itself.
			''' </summary>
			''' <returns> this object </returns>
			Public Overridable Property accessibleSelection As AccessibleSelection
				Get
				   Return Me
				End Get
			End Property

			''' <summary>
			''' Returns the <code>Accessible</code> child contained at
			''' the local coordinate <code>Point</code>, if one exists.
			''' Otherwise returns the currently selected tab.
			''' </summary>
			''' <returns> the <code>Accessible</code> at the specified
			'''    location, if it exists </returns>
			Public Overridable Function getAccessibleAt(ByVal p As Point) As Accessible
				Dim tab As Integer = CType(outerInstance.ui, TabbedPaneUI).tabForCoordinate(JTabbedPane.this, p.x, p.y)
				If tab = -1 Then tab = outerInstance.selectedIndex
				Return getAccessibleChild(tab)
			End Function

			Public Overridable Property accessibleSelectionCount As Integer Implements AccessibleSelection.getAccessibleSelectionCount
				Get
					Return 1
				End Get
			End Property

			Public Overridable Function getAccessibleSelection(ByVal i As Integer) As Accessible Implements AccessibleSelection.getAccessibleSelection
				Dim index As Integer = outerInstance.selectedIndex
				If index = -1 Then Return Nothing
				Return outerInstance.pages(index)
			End Function

			Public Overridable Function isAccessibleChildSelected(ByVal i As Integer) As Boolean Implements AccessibleSelection.isAccessibleChildSelected
				Return (i = outerInstance.selectedIndex)
			End Function

			Public Overridable Sub addAccessibleSelection(ByVal i As Integer) Implements AccessibleSelection.addAccessibleSelection
			   outerInstance.selectedIndex = i
			End Sub

			Public Overridable Sub removeAccessibleSelection(ByVal i As Integer) Implements AccessibleSelection.removeAccessibleSelection
			   ' can't do
			End Sub

			Public Overridable Sub clearAccessibleSelection() Implements AccessibleSelection.clearAccessibleSelection
			   ' can't do
			End Sub

			Public Overridable Sub selectAllAccessibleSelection() Implements AccessibleSelection.selectAllAccessibleSelection
			   ' can't do
			End Sub
		End Class

		<Serializable> _
		Private Class Page
			Inherits AccessibleContext
			Implements Accessible, AccessibleComponent

			Private ReadOnly outerInstance As JTabbedPane

			Friend title As String
			Friend background As Color
			Friend foreground As Color
			Friend icon As Icon
			Friend disabledIcon As Icon
			Friend parent As JTabbedPane
			Friend component As Component
			Friend tip As String
			Friend enabled As Boolean = True
			Friend needsUIUpdate As Boolean
			Friend mnemonic As Integer = -1
			Friend mnemonicIndex As Integer = -1
			Friend tabComponent As Component

			Friend Sub New(ByVal outerInstance As JTabbedPane, ByVal parent As JTabbedPane, ByVal title As String, ByVal icon As Icon, ByVal disabledIcon As Icon, ByVal component As Component, ByVal tip As String)
					Me.outerInstance = outerInstance
				Me.title = title
				Me.icon = icon
				Me.disabledIcon = disabledIcon
				Me.parent = parent
				Me.accessibleParent = parent
				Me.component = component
				Me.tip = tip

				initAccessibleContext()
			End Sub

	'        
	'         * initializes the AccessibleContext for the page
	'         
			Friend Overridable Sub initAccessibleContext()
				If outerInstance.accessibleContext IsNot Nothing AndAlso TypeOf component Is Accessible Then
	'                
	'                 * Do initialization if the AccessibleJTabbedPane
	'                 * has been instantiated. We do not want to load
	'                 * Accessibility classes unnecessarily.
	'                 
					Dim ac As AccessibleContext
					ac = component.accessibleContext
					If ac IsNot Nothing Then ac.accessibleParent = Me
				End If
			End Sub

			Friend Overridable Property mnemonic As Integer
				Set(ByVal mnemonic As Integer)
					Me.mnemonic = mnemonic
					updateDisplayedMnemonicIndex()
				End Set
				Get
					Return mnemonic
				End Get
			End Property


	'        
	'         * Sets the page displayed mnemonic index
	'         
			Friend Overridable Property displayedMnemonicIndex As Integer
				Set(ByVal mnemonicIndex As Integer)
					If Me.mnemonicIndex <> mnemonicIndex Then
						If mnemonicIndex <> -1 AndAlso (title Is Nothing OrElse mnemonicIndex < 0 OrElse mnemonicIndex >= title.Length) Then Throw New System.ArgumentException("Invalid mnemonic index: " & mnemonicIndex)
						Me.mnemonicIndex = mnemonicIndex
						outerInstance.firePropertyChange("displayedMnemonicIndexAt", Nothing, Nothing)
					End If
				End Set
				Get
					Return Me.mnemonicIndex
				End Get
			End Property

	'        
	'         * Returns the page displayed mnemonic index
	'         

			Friend Overridable Sub updateDisplayedMnemonicIndex()
				displayedMnemonicIndex = SwingUtilities.findDisplayedMnemonicIndex(title, mnemonic)
			End Sub

			'///////////////
			' Accessibility support
			'//////////////

			Public Overridable Property accessibleContext As AccessibleContext Implements Accessible.getAccessibleContext
				Get
					Return Me
				End Get
			End Property


			' AccessibleContext methods

			Public Property Overrides accessibleName As String
				Get
					If accessibleName IsNot Nothing Then
						Return accessibleName
					ElseIf title IsNot Nothing Then
						Return title
					End If
					Return Nothing
				End Get
			End Property

			Public Property Overrides accessibleDescription As String
				Get
					If accessibleDescription IsNot Nothing Then
						Return accessibleDescription
					ElseIf tip IsNot Nothing Then
						Return tip
					End If
					Return Nothing
				End Get
			End Property

			Public Property Overrides accessibleRole As AccessibleRole
				Get
					Return AccessibleRole.PAGE_TAB
				End Get
			End Property

			Public Property Overrides accessibleStateSet As AccessibleStateSet
				Get
					Dim states As AccessibleStateSet
					states = parent.accessibleContext.accessibleStateSet
					states.add(AccessibleState.SELECTABLE)
					Dim i As Integer = parent.indexOfTab(title)
					If i = parent.selectedIndex Then states.add(AccessibleState.SELECTED)
					Return states
				End Get
			End Property

			Public Property Overrides accessibleIndexInParent As Integer
				Get
					Return parent.indexOfTab(title)
				End Get
			End Property

			Public Property Overrides accessibleChildrenCount As Integer
				Get
					If TypeOf component Is Accessible Then
						Return 1
					Else
						Return 0
					End If
				End Get
			End Property

			Public Overrides Function getAccessibleChild(ByVal i As Integer) As Accessible
				If TypeOf component Is Accessible Then
					Return CType(component, Accessible)
				Else
					Return Nothing
				End If
			End Function

			Public Property Overrides locale As Locale
				Get
					Return parent.locale
				End Get
			End Property

			Public Property Overrides accessibleComponent As AccessibleComponent
				Get
					Return Me
				End Get
			End Property


			' AccessibleComponent methods

			Public Overridable Property background As Color Implements AccessibleComponent.getBackground
				Get
					Return If(background IsNot Nothing, background, parent.background)
				End Get
				Set(ByVal c As Color)
					background = c
				End Set
			End Property


			Public Overridable Property foreground As Color Implements AccessibleComponent.getForeground
				Get
					Return If(foreground IsNot Nothing, foreground, parent.foreground)
				End Get
				Set(ByVal c As Color)
					foreground = c
				End Set
			End Property


			Public Overridable Property cursor As Cursor Implements AccessibleComponent.getCursor
				Get
					Return parent.cursor
				End Get
				Set(ByVal c As Cursor)
					parent.cursor = c
				End Set
			End Property


			Public Overridable Property font As Font Implements AccessibleComponent.getFont
				Get
					Return parent.font
				End Get
				Set(ByVal f As Font)
					parent.font = f
				End Set
			End Property


			Public Overridable Function getFontMetrics(ByVal f As Font) As FontMetrics Implements AccessibleComponent.getFontMetrics
				Return parent.getFontMetrics(f)
			End Function

			Public Overridable Property enabled As Boolean Implements AccessibleComponent.isEnabled
				Get
					Return enabled
				End Get
				Set(ByVal b As Boolean)
					enabled = b
				End Set
			End Property


			Public Overridable Property visible As Boolean Implements AccessibleComponent.isVisible
				Get
					Return parent.visible
				End Get
				Set(ByVal b As Boolean)
					parent.visible = b
				End Set
			End Property


			Public Overridable Property showing As Boolean Implements AccessibleComponent.isShowing
				Get
					Return parent.showing
				End Get
			End Property

			Public Overridable Function contains(ByVal p As Point) As Boolean Implements AccessibleComponent.contains
				Dim r As Rectangle = bounds
				Return r.contains(p)
			End Function

			Public Overridable Property locationOnScreen As Point Implements AccessibleComponent.getLocationOnScreen
				Get
					 Dim parentLocation As Point = parent.locationOnScreen
					 Dim componentLocation As Point = location
					 componentLocation.translate(parentLocation.x, parentLocation.y)
					 Return componentLocation
				End Get
			End Property

			Public Overridable Property location As Point Implements AccessibleComponent.getLocation
				Get
					 Dim r As Rectangle = bounds
					 Return New Point(r.x, r.y)
				End Get
				Set(ByVal p As Point)
					' do nothing
				End Set
			End Property


			Public Overridable Property bounds As Rectangle Implements AccessibleComponent.getBounds
				Get
					Return parent.uI.getTabBounds(parent, parent.indexOfTab(title))
				End Get
				Set(ByVal r As Rectangle)
					' do nothing
				End Set
			End Property


			Public Overridable Property size As Dimension Implements AccessibleComponent.getSize
				Get
					Dim r As Rectangle = bounds
					Return New Dimension(r.width, r.height)
				End Get
				Set(ByVal d As Dimension)
					' do nothing
				End Set
			End Property


			Public Overridable Function getAccessibleAt(ByVal p As Point) As Accessible Implements AccessibleComponent.getAccessibleAt
				If TypeOf component Is Accessible Then
					Return CType(component, Accessible)
				Else
					Return Nothing
				End If
			End Function

			Public Overridable Property focusTraversable As Boolean Implements AccessibleComponent.isFocusTraversable
				Get
					Return False
				End Get
			End Property

			Public Overridable Sub requestFocus() Implements AccessibleComponent.requestFocus
				' do nothing
			End Sub

			Public Overridable Sub addFocusListener(ByVal l As FocusListener) Implements AccessibleComponent.addFocusListener
				' do nothing
			End Sub

			Public Overridable Sub removeFocusListener(ByVal l As FocusListener) Implements AccessibleComponent.removeFocusListener
				' do nothing
			End Sub

			' TIGER - 4732339
			''' <summary>
			''' Returns an AccessibleIcon
			''' </summary>
			''' <returns> the enabled icon if one exists and the page
			''' is enabled. Otherwise, returns the disabled icon if
			''' one exists and the page is disabled.  Otherwise, null
			''' is returned. </returns>
			Public Property Overrides accessibleIcon As AccessibleIcon()
				Get
					Dim ___accessibleIcon As AccessibleIcon = Nothing
					If enabled AndAlso TypeOf icon Is ImageIcon Then
						Dim ac As AccessibleContext = CType(icon, ImageIcon).accessibleContext
						___accessibleIcon = CType(ac, AccessibleIcon)
					ElseIf (Not enabled) AndAlso TypeOf disabledIcon Is ImageIcon Then
						Dim ac As AccessibleContext = CType(disabledIcon, ImageIcon).accessibleContext
						___accessibleIcon = CType(ac, AccessibleIcon)
					End If
					If ___accessibleIcon IsNot Nothing Then
						Dim returnIcons As AccessibleIcon() = New AccessibleIcon(0){}
						returnIcons(0) = ___accessibleIcon
						Return returnIcons
					Else
						Return Nothing
					End If
				End Get
			End Property
		End Class

		''' <summary>
		''' Sets the component that is responsible for rendering the
		''' title for the specified tab.  A null value means
		''' <code>JTabbedPane</code> will render the title and/or icon for
		''' the specified tab.  A non-null value means the component will
		''' render the title and <code>JTabbedPane</code> will not render
		''' the title and/or icon.
		''' <p>
		''' Note: The component must not be one that the developer has
		'''       already added to the tabbed pane.
		''' </summary>
		''' <param name="index"> the tab index where the component should be set </param>
		''' <param name="component"> the component to render the title for the
		'''                  specified tab </param>
		''' <exception cref="IndexOutOfBoundsException"> if index is out of range
		'''            {@code (index < 0 || index >= tab count)} </exception>
		''' <exception cref="IllegalArgumentException"> if component has already been
		'''            added to this <code>JTabbedPane</code>
		''' </exception>
		''' <seealso cref= #getTabComponentAt
		''' @beaninfo
		'''    preferred: true
		'''    attribute: visualUpdate true
		'''  description: The tab component at the specified tab index.
		''' @since 1.6 </seealso>
		Public Overridable Sub setTabComponentAt(ByVal index As Integer, ByVal component As Component)
			If component IsNot Nothing AndAlso indexOfComponent(component) <> -1 Then Throw New System.ArgumentException("Component is already added to this JTabbedPane")
			Dim oldValue As Component = getTabComponentAt(index)
			If component IsNot oldValue Then
				Dim tabComponentIndex As Integer = indexOfTabComponent(component)
				If tabComponentIndex <> -1 Then tabComponentAttAt(tabComponentIndex, Nothing)
				pages(index).tabComponent = component
				firePropertyChange("indexForTabComponent", -1, index)
			End If
		End Sub

		''' <summary>
		''' Returns the tab component at <code>index</code>.
		''' </summary>
		''' <param name="index">  the index of the item being queried </param>
		''' <returns> the tab component at <code>index</code> </returns>
		''' <exception cref="IndexOutOfBoundsException"> if index is out of range
		'''            {@code (index < 0 || index >= tab count)}
		''' </exception>
		''' <seealso cref= #setTabComponentAt
		''' @since 1.6 </seealso>
		Public Overridable Function getTabComponentAt(ByVal index As Integer) As Component
			Return pages(index).tabComponent
		End Function

		''' <summary>
		''' Returns the index of the tab for the specified tab component.
		''' Returns -1 if there is no tab for this tab component.
		''' </summary>
		''' <param name="tabComponent"> the tab component for the tab </param>
		''' <returns> the first tab which matches this tab component, or -1
		'''          if there is no tab for this tab component </returns>
		''' <seealso cref= #setTabComponentAt </seealso>
		''' <seealso cref= #getTabComponentAt
		''' @since 1.6 </seealso>
		 Public Overridable Function indexOfTabComponent(ByVal tabComponent As Component) As Integer
			For i As Integer = 0 To tabCount - 1
				Dim c As Component = getTabComponentAt(i)
				If c Is tabComponent Then Return i
			Next i
			Return -1
		 End Function
	End Class

End Namespace
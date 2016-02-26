Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports javax.swing
Imports javax.swing.event
Imports javax.swing.plaf

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

Namespace javax.swing.plaf.basic





	''' <summary>
	''' A Basic L&amp;F implementation of TabbedPaneUI.
	''' 
	''' @author Amy Fowler
	''' @author Philip Milne
	''' @author Steve Wilson
	''' @author Tom Santos
	''' @author Dave Moore
	''' </summary>
	Public Class BasicTabbedPaneUI
		Inherits TabbedPaneUI
		Implements SwingConstants


	' Instance variables initialized at installation

		Protected Friend tabPane As JTabbedPane

		Protected Friend highlight As Color
		Protected Friend lightHighlight As Color
		Protected Friend shadow As Color
		Protected Friend darkShadow As Color
		Protected Friend focus As Color
		Private selectedColor As Color

		Protected Friend textIconGap As Integer

		Protected Friend tabRunOverlay As Integer

		Protected Friend tabInsets As Insets
		Protected Friend selectedTabPadInsets As Insets
		Protected Friend tabAreaInsets As Insets
		Protected Friend contentBorderInsets As Insets
		Private tabsOverlapBorder As Boolean
		Private tabsOpaque As Boolean = True
		Private contentOpaque As Boolean = True

		''' <summary>
		''' As of Java 2 platform v1.3 this previously undocumented field is no
		''' longer used.
		''' Key bindings are now defined by the LookAndFeel, please refer to
		''' the key bindings specification for further details.
		''' </summary>
		''' @deprecated As of Java 2 platform v1.3. 
		<Obsolete("As of Java 2 platform v1.3.")> _
		Protected Friend upKey As KeyStroke
		''' <summary>
		''' As of Java 2 platform v1.3 this previously undocumented field is no
		''' longer used.
		''' Key bindings are now defined by the LookAndFeel, please refer to
		''' the key bindings specification for further details.
		''' </summary>
		''' @deprecated As of Java 2 platform v1.3. 
		<Obsolete("As of Java 2 platform v1.3.")> _
		Protected Friend downKey As KeyStroke
		''' <summary>
		''' As of Java 2 platform v1.3 this previously undocumented field is no
		''' longer used.
		''' Key bindings are now defined by the LookAndFeel, please refer to
		''' the key bindings specification for further details.
		''' </summary>
		''' @deprecated As of Java 2 platform v1.3. 
		<Obsolete("As of Java 2 platform v1.3.")> _
		Protected Friend leftKey As KeyStroke
		''' <summary>
		''' As of Java 2 platform v1.3 this previously undocumented field is no
		''' longer used.
		''' Key bindings are now defined by the LookAndFeel, please refer to
		''' the key bindings specification for further details.
		''' </summary>
		''' @deprecated As of Java 2 platform v1.3. 
		<Obsolete("As of Java 2 platform v1.3.")> _
		Protected Friend rightKey As KeyStroke


	' Transient variables (recalculated each time TabbedPane is layed out)

		Protected Friend tabRuns As Integer() = New Integer(9){}
		Protected Friend runCount As Integer = 0
		Protected Friend selectedRun As Integer = -1
		Protected Friend rects As Rectangle() = New Rectangle(){}
		Protected Friend maxTabHeight As Integer
		Protected Friend maxTabWidth As Integer

	' Listeners

		Protected Friend tabChangeListener As ChangeListener
		Protected Friend propertyChangeListener As java.beans.PropertyChangeListener
		Protected Friend mouseListener As MouseListener
		Protected Friend focusListener As FocusListener

	' Private instance data

		Private currentPadInsets As New Insets(0,0,0,0)
		Private currentTabAreaInsets As New Insets(0,0,0,0)

		Private visibleComponent As Component
		' PENDING(api): See comment for ContainerHandler
		Private htmlViews As List(Of javax.swing.text.View)

		Private mnemonicToIndexMap As Dictionary(Of Integer?, Integer?)

		''' <summary>
		''' InputMap used for mnemonics. Only non-null if the JTabbedPane has
		''' mnemonics associated with it. Lazily created in initMnemonics.
		''' </summary>
		Private mnemonicInputMap As InputMap

		' For use when tabLayoutPolicy = SCROLL_TAB_LAYOUT
		Private tabScroller As ScrollableTabSupport

		Private tabContainer As TabContainer

		''' <summary>
		''' A rectangle used for general layout calculations in order
		''' to avoid constructing many new Rectangles on the fly.
		''' </summary>
		<NonSerialized> _
		Protected Friend calcRect As New Rectangle(0,0,0,0)

		''' <summary>
		''' Tab that has focus.
		''' </summary>
		Private focusIndex As Integer

		''' <summary>
		''' Combined listeners.
		''' </summary>
		Private handler As Handler

		''' <summary>
		''' Index of the tab the mouse is over.
		''' </summary>
		Private rolloverTabIndex As Integer

		''' <summary>
		''' This is set to true when a component is added/removed from the tab
		''' pane and set to false when layout happens.  If true it indicates that
		''' tabRuns is not valid and shouldn't be used.
		''' </summary>
		Private isRunsDirty As Boolean

		Private calculatedBaseline As Boolean
		Private baseline As Integer

	' UI creation

		Public Shared Function createUI(ByVal c As JComponent) As ComponentUI
			Return New BasicTabbedPaneUI
		End Function

		Friend Shared Sub loadActionMap(ByVal map As LazyActionMap)
			map.put(New Actions(Actions.NEXT))
			map.put(New Actions(Actions.PREVIOUS))
			map.put(New Actions(Actions.RIGHT))
			map.put(New Actions(Actions.LEFT))
			map.put(New Actions(Actions.UP))
			map.put(New Actions(Actions.DOWN))
			map.put(New Actions(Actions.PAGE_UP))
			map.put(New Actions(Actions.PAGE_DOWN))
			map.put(New Actions(Actions.REQUEST_FOCUS))
			map.put(New Actions(Actions.REQUEST_FOCUS_FOR_VISIBLE))
			map.put(New Actions(Actions.SET_SELECTED))
			map.put(New Actions(Actions.SELECT_FOCUSED))
			map.put(New Actions(Actions.SCROLL_FORWARD))
			map.put(New Actions(Actions.SCROLL_BACKWARD))
		End Sub

	' UI Installation/De-installation

		Public Overridable Sub installUI(ByVal c As JComponent)
			Me.tabPane = CType(c, JTabbedPane)

			calculatedBaseline = False
			rolloverTabIndex = -1
			focusIndex = -1
			c.layout = createLayoutManager()
			installComponents()
			installDefaults()
			installListeners()
			installKeyboardActions()
		End Sub

		Public Overridable Sub uninstallUI(ByVal c As JComponent)
			uninstallKeyboardActions()
			uninstallListeners()
			uninstallDefaults()
			uninstallComponents()
			c.layout = Nothing

			Me.tabPane = Nothing
		End Sub

		''' <summary>
		''' Invoked by <code>installUI</code> to create
		''' a layout manager object to manage
		''' the <code>JTabbedPane</code>.
		''' </summary>
		''' <returns> a layout manager object
		''' </returns>
		''' <seealso cref= TabbedPaneLayout </seealso>
		''' <seealso cref= javax.swing.JTabbedPane#getTabLayoutPolicy </seealso>
		Protected Friend Overridable Function createLayoutManager() As LayoutManager
			If tabPane.tabLayoutPolicy = JTabbedPane.SCROLL_TAB_LAYOUT Then
				Return New TabbedPaneScrollLayout(Me) ' WRAP_TAB_LAYOUT
			Else
				Return New TabbedPaneLayout(Me)
			End If
		End Function

	'     In an attempt to preserve backward compatibility for programs
	'     * which have extended BasicTabbedPaneUI to do their own layout, the
	'     * UI uses the installed layoutManager (and not tabLayoutPolicy) to
	'     * determine if scrollTabLayout is enabled.
	'     
		Private Function scrollableTabLayoutEnabled() As Boolean
			Return (TypeOf tabPane.layout Is TabbedPaneScrollLayout)
		End Function

		''' <summary>
		''' Creates and installs any required subcomponents for the JTabbedPane.
		''' Invoked by installUI.
		''' 
		''' @since 1.4
		''' </summary>
		Protected Friend Overridable Sub installComponents()
			If scrollableTabLayoutEnabled() Then
				If tabScroller Is Nothing Then
					tabScroller = New ScrollableTabSupport(Me, tabPane.tabPlacement)
					tabPane.add(tabScroller.viewport)
				End If
			End If
			installTabContainer()
		End Sub

		Private Sub installTabContainer()
			 For i As Integer = 0 To tabPane.tabCount - 1
				 Dim tabComponent As Component = tabPane.getTabComponentAt(i)
				 If tabComponent IsNot Nothing Then
					 If tabContainer Is Nothing Then tabContainer = New TabContainer(Me)
					 tabContainer.add(tabComponent)
				 End If
			 Next i
			 If tabContainer Is Nothing Then Return
			 If scrollableTabLayoutEnabled() Then
				 tabScroller.tabPanel.add(tabContainer)
			 Else
				 tabPane.add(tabContainer)
			 End If
		End Sub

		''' <summary>
		''' Creates and returns a JButton that will provide the user
		''' with a way to scroll the tabs in a particular direction. The
		''' returned JButton must be instance of UIResource.
		''' </summary>
		''' <param name="direction"> One of the SwingConstants constants:
		''' SOUTH, NORTH, EAST or WEST </param>
		''' <returns> Widget for user to </returns>
		''' <seealso cref= javax.swing.JTabbedPane#setTabPlacement </seealso>
		''' <seealso cref= javax.swing.SwingConstants </seealso>
		''' <exception cref="IllegalArgumentException"> if direction is not one of
		'''         NORTH, SOUTH, EAST or WEST
		''' @since 1.5 </exception>
		Protected Friend Overridable Function createScrollButton(ByVal direction As Integer) As JButton
			If direction <> SOUTH AndAlso direction <> NORTH AndAlso direction <> EAST AndAlso direction <> WEST Then Throw New System.ArgumentException("Direction must be one of: " & "SOUTH, NORTH, EAST or WEST")
			Return New ScrollableTabButton(Me, direction)
		End Function

		''' <summary>
		''' Removes any installed subcomponents from the JTabbedPane.
		''' Invoked by uninstallUI.
		''' 
		''' @since 1.4
		''' </summary>
		Protected Friend Overridable Sub uninstallComponents()
			uninstallTabContainer()
			If scrollableTabLayoutEnabled() Then
				tabPane.remove(tabScroller.viewport)
				tabPane.remove(tabScroller.scrollForwardButton)
				tabPane.remove(tabScroller.scrollBackwardButton)
				tabScroller = Nothing
			End If
		End Sub

		Private Sub uninstallTabContainer()
			 If tabContainer Is Nothing Then Return
			 ' Remove all the tabComponents, making sure not to notify
			 ' the tabbedpane.
			 tabContainer.notifyTabbedPane = False
			 tabContainer.removeAll()
			 If scrollableTabLayoutEnabled() Then
				 tabContainer.remove(tabScroller.croppedEdge)
				 tabScroller.tabPanel.remove(tabContainer)
			 Else
			   tabPane.remove(tabContainer)
			 End If
			 tabContainer = Nothing
		End Sub

		Protected Friend Overridable Sub installDefaults()
			LookAndFeel.installColorsAndFont(tabPane, "TabbedPane.background", "TabbedPane.foreground", "TabbedPane.font")
			highlight = UIManager.getColor("TabbedPane.light")
			lightHighlight = UIManager.getColor("TabbedPane.highlight")
			shadow = UIManager.getColor("TabbedPane.shadow")
			darkShadow = UIManager.getColor("TabbedPane.darkShadow")
			focus = UIManager.getColor("TabbedPane.focus")
			selectedColor = UIManager.getColor("TabbedPane.selected")

			textIconGap = UIManager.getInt("TabbedPane.textIconGap")
			tabInsets = UIManager.getInsets("TabbedPane.tabInsets")
			selectedTabPadInsets = UIManager.getInsets("TabbedPane.selectedTabPadInsets")
			tabAreaInsets = UIManager.getInsets("TabbedPane.tabAreaInsets")
			tabsOverlapBorder = UIManager.getBoolean("TabbedPane.tabsOverlapBorder")
			contentBorderInsets = UIManager.getInsets("TabbedPane.contentBorderInsets")
			tabRunOverlay = UIManager.getInt("TabbedPane.tabRunOverlay")
			tabsOpaque = UIManager.getBoolean("TabbedPane.tabsOpaque")
			contentOpaque = UIManager.getBoolean("TabbedPane.contentOpaque")
			Dim opaque As Object = UIManager.get("TabbedPane.opaque")
			If opaque Is Nothing Then opaque = Boolean.FALSE
			LookAndFeel.installProperty(tabPane, "opaque", opaque)

			' Fix for 6711145 BasicTabbedPanuUI should not throw a NPE if these
			' keys are missing. So we are setting them to there default values here
			' if the keys are missing.
			If tabInsets Is Nothing Then tabInsets = New Insets(0,4,1,4)
			If selectedTabPadInsets Is Nothing Then selectedTabPadInsets = New Insets(2,2,2,1)
			If tabAreaInsets Is Nothing Then tabAreaInsets = New Insets(3,2,0,2)
			If contentBorderInsets Is Nothing Then contentBorderInsets = New Insets(2,2,3,3)
		End Sub

		Protected Friend Overridable Sub uninstallDefaults()
			highlight = Nothing
			lightHighlight = Nothing
			shadow = Nothing
			darkShadow = Nothing
			focus = Nothing
			tabInsets = Nothing
			selectedTabPadInsets = Nothing
			tabAreaInsets = Nothing
			contentBorderInsets = Nothing
		End Sub

		Protected Friend Overridable Sub installListeners()
			propertyChangeListener = createPropertyChangeListener()
			If propertyChangeListener IsNot Nothing Then tabPane.addPropertyChangeListener(propertyChangeListener)
			tabChangeListener = createChangeListener()
			If tabChangeListener IsNot Nothing Then tabPane.addChangeListener(tabChangeListener)
			mouseListener = createMouseListener()
			If mouseListener IsNot Nothing Then tabPane.addMouseListener(mouseListener)
			tabPane.addMouseMotionListener(handler)
			focusListener = createFocusListener()
			If focusListener IsNot Nothing Then tabPane.addFocusListener(focusListener)
			tabPane.addContainerListener(handler)
			If tabPane.tabCount>0 Then htmlViews = createHTMLVector()
		End Sub

		Protected Friend Overridable Sub uninstallListeners()
			If mouseListener IsNot Nothing Then
				tabPane.removeMouseListener(mouseListener)
				mouseListener = Nothing
			End If
			tabPane.removeMouseMotionListener(handler)
			If focusListener IsNot Nothing Then
				tabPane.removeFocusListener(focusListener)
				focusListener = Nothing
			End If

			tabPane.removeContainerListener(handler)
			If htmlViews IsNot Nothing Then
				htmlViews.Clear()
				htmlViews = Nothing
			End If
			If tabChangeListener IsNot Nothing Then
				tabPane.removeChangeListener(tabChangeListener)
				tabChangeListener = Nothing
			End If
			If propertyChangeListener IsNot Nothing Then
				tabPane.removePropertyChangeListener(propertyChangeListener)
				propertyChangeListener = Nothing
			End If
			handler = Nothing
		End Sub

		Protected Friend Overridable Function createMouseListener() As MouseListener
			Return handler
		End Function

		Protected Friend Overridable Function createFocusListener() As FocusListener
			Return handler
		End Function

		Protected Friend Overridable Function createChangeListener() As ChangeListener
			Return handler
		End Function

		Protected Friend Overridable Function createPropertyChangeListener() As java.beans.PropertyChangeListener
			Return handler
		End Function

		Private Property handler As Handler
			Get
				If handler Is Nothing Then handler = New Handler(Me)
				Return handler
			End Get
		End Property

		Protected Friend Overridable Sub installKeyboardActions()
			Dim km As InputMap = getInputMap(JComponent.WHEN_ANCESTOR_OF_FOCUSED_COMPONENT)

			SwingUtilities.replaceUIInputMap(tabPane, JComponent.WHEN_ANCESTOR_OF_FOCUSED_COMPONENT, km)
			km = getInputMap(JComponent.WHEN_FOCUSED)
			SwingUtilities.replaceUIInputMap(tabPane, JComponent.WHEN_FOCUSED, km)

			LazyActionMap.installLazyActionMap(tabPane, GetType(BasicTabbedPaneUI), "TabbedPane.actionMap")
			updateMnemonics()
		End Sub

		Friend Overridable Function getInputMap(ByVal condition As Integer) As InputMap
			If condition = JComponent.WHEN_ANCESTOR_OF_FOCUSED_COMPONENT Then
				Return CType(sun.swing.DefaultLookup.get(tabPane, Me, "TabbedPane.ancestorInputMap"), InputMap)
			ElseIf condition = JComponent.WHEN_FOCUSED Then
				Return CType(sun.swing.DefaultLookup.get(tabPane, Me, "TabbedPane.focusInputMap"), InputMap)
			End If
			Return Nothing
		End Function

		Protected Friend Overridable Sub uninstallKeyboardActions()
			SwingUtilities.replaceUIActionMap(tabPane, Nothing)
			SwingUtilities.replaceUIInputMap(tabPane, JComponent.WHEN_ANCESTOR_OF_FOCUSED_COMPONENT, Nothing)
			SwingUtilities.replaceUIInputMap(tabPane, JComponent.WHEN_FOCUSED, Nothing)
			SwingUtilities.replaceUIInputMap(tabPane, JComponent.WHEN_IN_FOCUSED_WINDOW, Nothing)
			mnemonicToIndexMap = Nothing
			mnemonicInputMap = Nothing
		End Sub

		''' <summary>
		''' Reloads the mnemonics. This should be invoked when a memonic changes,
		''' when the title of a mnemonic changes, or when tabs are added/removed.
		''' </summary>
		Private Sub updateMnemonics()
			resetMnemonics()
			For counter As Integer = tabPane.tabCount - 1 To 0 Step -1
				Dim mnemonic As Integer = tabPane.getMnemonicAt(counter)

				If mnemonic > 0 Then addMnemonic(counter, mnemonic)
			Next counter
		End Sub

		''' <summary>
		''' Resets the mnemonics bindings to an empty state.
		''' </summary>
		Private Sub resetMnemonics()
			If mnemonicToIndexMap IsNot Nothing Then
				mnemonicToIndexMap.Clear()
				mnemonicInputMap.clear()
			End If
		End Sub

		''' <summary>
		''' Adds the specified mnemonic at the specified index.
		''' </summary>
		Private Sub addMnemonic(ByVal index As Integer, ByVal mnemonic As Integer)
			If mnemonicToIndexMap Is Nothing Then initMnemonics()
			mnemonicInputMap.put(KeyStroke.getKeyStroke(mnemonic, BasicLookAndFeel.focusAcceleratorKeyMask), "setSelectedIndex")
			mnemonicToIndexMap(Convert.ToInt32(mnemonic)) = Convert.ToInt32(index)
		End Sub

		''' <summary>
		''' Installs the state needed for mnemonics.
		''' </summary>
		Private Sub initMnemonics()
			mnemonicToIndexMap = New Dictionary(Of Integer?, Integer?)
			mnemonicInputMap = New ComponentInputMapUIResource(tabPane)
			mnemonicInputMap.parent = SwingUtilities.getUIInputMap(tabPane, JComponent.WHEN_IN_FOCUSED_WINDOW)
			SwingUtilities.replaceUIInputMap(tabPane, JComponent.WHEN_IN_FOCUSED_WINDOW, mnemonicInputMap)
		End Sub

		''' <summary>
		''' Sets the tab the mouse is over by location. This is a cover method
		''' for <code>setRolloverTab(tabForCoordinate(x, y, false))</code>.
		''' </summary>
		Private Sub setRolloverTab(ByVal x As Integer, ByVal y As Integer)
			' NOTE:
			' This calls in with false otherwise it could trigger a validate,
			' which should NOT happen if the user is only dragging the
			' mouse around.
			rolloverTab = tabForCoordinate(tabPane, x, y, False)
		End Sub

		''' <summary>
		''' Sets the tab the mouse is currently over to <code>index</code>.
		''' <code>index</code> will be -1 if the mouse is no longer over any
		''' tab. No checking is done to ensure the passed in index identifies a
		''' valid tab.
		''' </summary>
		''' <param name="index"> Index of the tab the mouse is over.
		''' @since 1.5 </param>
		Protected Friend Overridable Property rolloverTab As Integer
			Set(ByVal index As Integer)
				rolloverTabIndex = index
			End Set
			Get
				Return rolloverTabIndex
			End Get
		End Property


		Public Overridable Function getMinimumSize(ByVal c As JComponent) As Dimension
			' Default to LayoutManager's minimumLayoutSize
			Return Nothing
		End Function

		Public Overridable Function getMaximumSize(ByVal c As JComponent) As Dimension
			' Default to LayoutManager's maximumLayoutSize
			Return Nothing
		End Function

		''' <summary>
		''' Returns the baseline.
		''' </summary>
		''' <exception cref="NullPointerException"> {@inheritDoc} </exception>
		''' <exception cref="IllegalArgumentException"> {@inheritDoc} </exception>
		''' <seealso cref= javax.swing.JComponent#getBaseline(int, int)
		''' @since 1.6 </seealso>
		Public Overridable Function getBaseline(ByVal c As JComponent, ByVal width As Integer, ByVal height As Integer) As Integer
			MyBase.getBaseline(c, width, height)
			Dim ___baseline As Integer = calculateBaselineIfNecessary()
			If ___baseline <> -1 Then
				Dim placement As Integer = tabPane.tabPlacement
				Dim insets As Insets = tabPane.insets
				Dim ___tabAreaInsets As Insets = getTabAreaInsets(placement)
				Select Case placement
				Case JTabbedPane.TOP
					___baseline += insets.top + ___tabAreaInsets.top
					Return ___baseline
				Case JTabbedPane.BOTTOM
					___baseline = height - insets.bottom - ___tabAreaInsets.bottom - maxTabHeight + ___baseline
					Return ___baseline
				Case JTabbedPane.LEFT, JTabbedPane.RIGHT
					___baseline += insets.top + ___tabAreaInsets.top
					Return ___baseline
				End Select
			End If
			Return -1
		End Function

		''' <summary>
		''' Returns an enum indicating how the baseline of the component
		''' changes as the size changes.
		''' </summary>
		''' <exception cref="NullPointerException"> {@inheritDoc} </exception>
		''' <seealso cref= javax.swing.JComponent#getBaseline(int, int)
		''' @since 1.6 </seealso>
		Public Overridable Function getBaselineResizeBehavior(ByVal c As JComponent) As Component.BaselineResizeBehavior
			MyBase.getBaselineResizeBehavior(c)
			Select Case tabPane.tabPlacement
			Case JTabbedPane.LEFT, JTabbedPane.RIGHT, JTabbedPane.TOP
				Return Component.BaselineResizeBehavior.CONSTANT_ASCENT
			Case JTabbedPane.BOTTOM
				Return Component.BaselineResizeBehavior.CONSTANT_DESCENT
			End Select
			Return Component.BaselineResizeBehavior.OTHER
		End Function

		''' <summary>
		''' Returns the baseline for the specified tab.
		''' </summary>
		''' <param name="tab"> index of tab to get baseline for </param>
		''' <exception cref="IndexOutOfBoundsException"> if index is out of range
		'''            (index &lt; 0 || index &gt;= tab count) </exception>
		''' <returns> baseline or a value &lt; 0 indicating there is no reasonable
		'''                  baseline
		''' @since 1.6 </returns>
		Protected Friend Overridable Function getBaseline(ByVal tab As Integer) As Integer
			If tabPane.getTabComponentAt(tab) IsNot Nothing Then
				Dim offset As Integer = baselineOffset
				If offset <> 0 Then Return -1
				Dim c As Component = tabPane.getTabComponentAt(tab)
				Dim pref As Dimension = c.preferredSize
				Dim ___tabInsets As Insets = getTabInsets(tabPane.tabPlacement, tab)
				Dim cellHeight As Integer = maxTabHeight - ___tabInsets.top - ___tabInsets.bottom
				Return c.getBaseline(pref.width, pref.height) + (cellHeight - pref.height) / 2 + ___tabInsets.top
			Else
				Dim view As javax.swing.text.View = getTextViewForTab(tab)
				If view IsNot Nothing Then
					Dim viewHeight As Integer = CInt(Fix(view.getPreferredSpan(javax.swing.text.View.Y_AXIS)))
					Dim ___baseline As Integer = BasicHTML.getHTMLBaseline(view, CInt(Fix(view.getPreferredSpan(javax.swing.text.View.X_AXIS))), viewHeight)
					If ___baseline >= 0 Then Return maxTabHeight \ 2 - viewHeight \ 2 + ___baseline + baselineOffset
					Return -1
				End If
			End If
			Dim metrics As FontMetrics = fontMetrics
			Dim fontHeight As Integer = metrics.height
			Dim fontBaseline As Integer = metrics.ascent
			Return maxTabHeight \ 2 - fontHeight \ 2 + fontBaseline + baselineOffset
		End Function

		''' <summary>
		''' Returns the amount the baseline is offset by.  This is typically
		''' the same as <code>getTabLabelShiftY</code>.
		''' </summary>
		''' <returns> amount to offset the baseline by
		''' @since 1.6 </returns>
		Protected Friend Overridable Property baselineOffset As Integer
			Get
				Select Case tabPane.tabPlacement
				Case JTabbedPane.TOP
					If tabPane.tabCount > 1 Then
						Return 1
					Else
						Return -1
					End If
				Case JTabbedPane.BOTTOM
					If tabPane.tabCount > 1 Then
						Return -1
					Else
						Return 1
					End If
				Case Else ' RIGHT|LEFT
					Return (maxTabHeight Mod 2)
				End Select
			End Get
		End Property

		Private Function calculateBaselineIfNecessary() As Integer
			If Not calculatedBaseline Then
				calculatedBaseline = True
				baseline = -1
				If tabPane.tabCount > 0 Then calculateBaseline()
			End If
			Return baseline
		End Function

		Private Sub calculateBaseline()
			Dim tabCount As Integer = tabPane.tabCount
			Dim tabPlacement As Integer = tabPane.tabPlacement
			maxTabHeight = calculateMaxTabHeight(tabPlacement)
			baseline = getBaseline(0)
			If horizontalTabPlacement Then
				For i As Integer = 1 To tabCount - 1
					If getBaseline(i) <> baseline Then
						baseline = -1
						Exit For
					End If
				Next i
			Else
				' left/right, tabs may be different sizes.
				Dim ___fontMetrics As FontMetrics = fontMetrics
				Dim fontHeight As Integer = ___fontMetrics.height
				Dim height As Integer = calculateTabHeight(tabPlacement, 0, fontHeight)
				For i As Integer = 1 To tabCount - 1
					Dim newHeight As Integer = calculateTabHeight(tabPlacement, i,fontHeight)
					If height <> newHeight Then
						' assume different baseline
						baseline = -1
						Exit For
					End If
				Next i
			End If
		End Sub

	' UI Rendering

		Public Overridable Sub paint(ByVal g As Graphics, ByVal c As JComponent)
			Dim selectedIndex As Integer = tabPane.selectedIndex
			Dim tabPlacement As Integer = tabPane.tabPlacement

			ensureCurrentLayout()

			' Paint content border and tab area
			If tabsOverlapBorder Then paintContentBorder(g, tabPlacement, selectedIndex)
			' If scrollable tabs are enabled, the tab area will be
			' painted by the scrollable tab panel instead.
			'
			If Not scrollableTabLayoutEnabled() Then ' WRAP_TAB_LAYOUT paintTabArea(g, tabPlacement, selectedIndex)
			If Not tabsOverlapBorder Then paintContentBorder(g, tabPlacement, selectedIndex)
		End Sub

		''' <summary>
		''' Paints the tabs in the tab area.
		''' Invoked by paint().
		''' The graphics parameter must be a valid <code>Graphics</code>
		''' object.  Tab placement may be either:
		''' <code>JTabbedPane.TOP</code>, <code>JTabbedPane.BOTTOM</code>,
		''' <code>JTabbedPane.LEFT</code>, or <code>JTabbedPane.RIGHT</code>.
		''' The selected index must be a valid tabbed pane tab index (0 to
		''' tab count - 1, inclusive) or -1 if no tab is currently selected.
		''' The handling of invalid parameters is unspecified.
		''' </summary>
		''' <param name="g"> the graphics object to use for rendering </param>
		''' <param name="tabPlacement"> the placement for the tabs within the JTabbedPane </param>
		''' <param name="selectedIndex"> the tab index of the selected component
		''' 
		''' @since 1.4 </param>
		Protected Friend Overridable Sub paintTabArea(ByVal g As Graphics, ByVal tabPlacement As Integer, ByVal selectedIndex As Integer)
			Dim tabCount As Integer = tabPane.tabCount

			Dim iconRect As New Rectangle, textRect As New Rectangle
			Dim clipRect As Rectangle = g.clipBounds

			' Paint tabRuns of tabs from back to front
			For i As Integer = runCount - 1 To 0 Step -1
				Dim start As Integer = tabRuns(i)
				Dim [next] As Integer = tabRuns(If(i = runCount - 1, 0, i + 1))
				Dim [end] As Integer = (If([next] <> 0, [next] - 1, tabCount - 1))
				For j As Integer = start To [end]
					If j <> selectedIndex AndAlso rects(j).intersects(clipRect) Then paintTab(g, tabPlacement, rects, j, iconRect, textRect)
				Next j
			Next i

			' Paint selected tab if its in the front run
			' since it may overlap other tabs
			If selectedIndex >= 0 AndAlso rects(selectedIndex).intersects(clipRect) Then paintTab(g, tabPlacement, rects, selectedIndex, iconRect, textRect)
		End Sub

		Protected Friend Overridable Sub paintTab(ByVal g As Graphics, ByVal tabPlacement As Integer, ByVal rects As Rectangle(), ByVal tabIndex As Integer, ByVal iconRect As Rectangle, ByVal textRect As Rectangle)
			Dim tabRect As Rectangle = rects(tabIndex)
			Dim selectedIndex As Integer = tabPane.selectedIndex
			Dim isSelected As Boolean = selectedIndex = tabIndex

			If tabsOpaque OrElse tabPane.opaque Then paintTabBackground(g, tabPlacement, tabIndex, tabRect.x, tabRect.y, tabRect.width, tabRect.height, isSelected)

			paintTabBorder(g, tabPlacement, tabIndex, tabRect.x, tabRect.y, tabRect.width, tabRect.height, isSelected)

			Dim title As String = tabPane.getTitleAt(tabIndex)
			Dim font As Font = tabPane.font
			Dim metrics As FontMetrics = sun.swing.SwingUtilities2.getFontMetrics(tabPane, g, font)
			Dim icon As Icon = getIconForTab(tabIndex)

			layoutLabel(tabPlacement, metrics, tabIndex, title, icon, tabRect, iconRect, textRect, isSelected)

			If tabPane.getTabComponentAt(tabIndex) Is Nothing Then
				Dim clippedTitle As String = title

				If scrollableTabLayoutEnabled() AndAlso tabScroller.croppedEdge.paramsSet AndAlso tabScroller.croppedEdge.tabIndex = tabIndex AndAlso horizontalTabPlacement Then
					Dim availTextWidth As Integer = tabScroller.croppedEdge.cropline - (textRect.x - tabRect.x) - tabScroller.croppedEdge.croppedSideWidth
					clippedTitle = sun.swing.SwingUtilities2.clipStringIfNecessary(Nothing, metrics, title, availTextWidth)
				ElseIf (Not scrollableTabLayoutEnabled()) AndAlso horizontalTabPlacement Then
					clippedTitle = sun.swing.SwingUtilities2.clipStringIfNecessary(Nothing, metrics, title, textRect.width)
				End If

				paintText(g, tabPlacement, font, metrics, tabIndex, clippedTitle, textRect, isSelected)

				paintIcon(g, tabPlacement, tabIndex, icon, iconRect, isSelected)
			End If
			paintFocusIndicator(g, tabPlacement, rects, tabIndex, iconRect, textRect, isSelected)
		End Sub

		Private Property horizontalTabPlacement As Boolean
			Get
				Return tabPane.tabPlacement = TOP OrElse tabPane.tabPlacement = BOTTOM
			End Get
		End Property

	'     This method will create and return a polygon shape for the given tab rectangle
	'     * which has been cropped at the specified cropline with a torn edge visual.
	'     * e.g. A "File" tab which has cropped been cropped just after the "i":
	'     *             -------------
	'     *             |  .....     |
	'     *             |  .          |
	'     *             |  ...  .    |
	'     *             |  .    .   |
	'     *             |  .    .    |
	'     *             |  .    .     |
	'     *             --------------
	'     *
	'     * The x, y arrays below define the pattern used to create a "torn" edge
	'     * segment which is repeated to fill the edge of the tab.
	'     * For tabs placed on TOP and BOTTOM, this righthand torn edge is created by
	'     * line segments which are defined by coordinates obtained by
	'     * subtracting xCropLen[i] from (tab.x + tab.width) and adding yCroplen[i]
	'     * to (tab.y).
	'     * For tabs placed on LEFT or RIGHT, the bottom torn edge is created by
	'     * subtracting xCropLen[i] from (tab.y + tab.height) and adding yCropLen[i]
	'     * to (tab.x).
	'     
		Private Shared xCropLen As Integer() = {1,1,0,0,1,1,2,2}
		Private Shared yCropLen As Integer() = {0,3,3,6,6,9,9,12}
		Private Const CROP_SEGMENT As Integer = 12

		Private Shared Function createCroppedTabShape(ByVal tabPlacement As Integer, ByVal tabRect As Rectangle, ByVal cropline As Integer) As Polygon
			Dim rlen As Integer
			Dim start As Integer
			Dim [end] As Integer
			Dim ostart As Integer

			Select Case tabPlacement
			  Case LEFT, RIGHT
				  rlen = tabRect.width
				  start = tabRect.x
				  [end] = tabRect.x + tabRect.width
				  ostart = tabRect.y + tabRect.height
			  Case Else
				 rlen = tabRect.height
				 start = tabRect.y
				 [end] = tabRect.y + tabRect.height
				 ostart = tabRect.x + tabRect.width
			End Select
			Dim rcnt As Integer = rlen\CROP_SEGMENT
			If rlen Mod CROP_SEGMENT > 0 Then rcnt += 1
			Dim npts As Integer = 2 + (rcnt*8)
			Dim xp As Integer() = New Integer(npts - 1){}
			Dim yp As Integer() = New Integer(npts - 1){}
			Dim pcnt As Integer = 0

			xp(pcnt) = ostart
			yp(pcnt) = [end]
			pcnt += 1
			xp(pcnt) = ostart
			yp(pcnt) = start
			pcnt += 1
			For i As Integer = 0 To rcnt - 1
				For j As Integer = 0 To xCropLen.Length - 1
					xp(pcnt) = cropline - xCropLen(j)
					yp(pcnt) = start + (i*CROP_SEGMENT) + yCropLen(j)
					If yp(pcnt) >= [end] Then
						yp(pcnt) = [end]
						pcnt += 1
						Exit For
					End If
					pcnt += 1
				Next j
			Next i
			If tabPlacement = JTabbedPane.TOP OrElse tabPlacement = JTabbedPane.BOTTOM Then
			   Return New Polygon(xp, yp, pcnt)
 ' LEFT or RIGHT
			Else
			   Return New Polygon(yp, xp, pcnt)
			End If
		End Function

	'     If tabLayoutPolicy == SCROLL_TAB_LAYOUT, this method will paint an edge
	'     * indicating the tab is cropped in the viewport display
	'     
		Private Sub paintCroppedTabEdge(ByVal g As Graphics)
			Dim tabIndex As Integer = tabScroller.croppedEdge.tabIndex
			Dim cropline As Integer = tabScroller.croppedEdge.cropline
			Dim x, y As Integer
			Select Case tabPane.tabPlacement
			  Case LEFT, RIGHT
				x = rects(tabIndex).x
				y = cropline
				Dim xx As Integer = x
				g.color = shadow
				Do While xx <= x+rects(tabIndex).width
					For i As Integer = 0 To xCropLen.Length - 1 Step 2
						g.drawLine(xx+yCropLen(i),y-xCropLen(i), xx+yCropLen(i+1)-1,y-xCropLen(i+1))
					Next i
					xx+=CROP_SEGMENT
				Loop
			  Case Else
				x = cropline
				y = rects(tabIndex).y
				Dim yy As Integer = y
				g.color = shadow
				Do While yy <= y+rects(tabIndex).height
					For i As Integer = 0 To xCropLen.Length - 1 Step 2
						g.drawLine(x-xCropLen(i),yy+yCropLen(i), x-xCropLen(i+1),yy+yCropLen(i+1)-1)
					Next i
					yy+=CROP_SEGMENT
				Loop
			End Select
		End Sub

		Protected Friend Overridable Sub layoutLabel(ByVal tabPlacement As Integer, ByVal metrics As FontMetrics, ByVal tabIndex As Integer, ByVal title As String, ByVal icon As Icon, ByVal tabRect As Rectangle, ByVal iconRect As Rectangle, ByVal textRect As Rectangle, ByVal isSelected As Boolean)
				iconRect.y = 0
					iconRect.x = iconRect.y
						textRect.y = iconRect.x
						textRect.x = textRect.y

			Dim v As javax.swing.text.View = getTextViewForTab(tabIndex)
			If v IsNot Nothing Then tabPane.putClientProperty("html", v)

			SwingUtilities.layoutCompoundLabel(tabPane, metrics, title, icon, SwingUtilities.CENTER, SwingUtilities.CENTER, SwingUtilities.CENTER, SwingUtilities.TRAILING, tabRect, iconRect, textRect, textIconGap)

			tabPane.putClientProperty("html", Nothing)

			Dim xNudge As Integer = getTabLabelShiftX(tabPlacement, tabIndex, isSelected)
			Dim yNudge As Integer = getTabLabelShiftY(tabPlacement, tabIndex, isSelected)
			iconRect.x += xNudge
			iconRect.y += yNudge
			textRect.x += xNudge
			textRect.y += yNudge
		End Sub

		Protected Friend Overridable Sub paintIcon(ByVal g As Graphics, ByVal tabPlacement As Integer, ByVal tabIndex As Integer, ByVal icon As Icon, ByVal iconRect As Rectangle, ByVal isSelected As Boolean)
			If icon IsNot Nothing Then icon.paintIcon(tabPane, g, iconRect.x, iconRect.y)
		End Sub

		Protected Friend Overridable Sub paintText(ByVal g As Graphics, ByVal tabPlacement As Integer, ByVal font As Font, ByVal metrics As FontMetrics, ByVal tabIndex As Integer, ByVal title As String, ByVal textRect As Rectangle, ByVal isSelected As Boolean)

			g.font = font

			Dim v As javax.swing.text.View = getTextViewForTab(tabIndex)
			If v IsNot Nothing Then
				' html
				v.paint(g, textRect)
			Else
				' plain text
				Dim mnemIndex As Integer = tabPane.getDisplayedMnemonicIndexAt(tabIndex)

				If tabPane.enabled AndAlso tabPane.isEnabledAt(tabIndex) Then
					Dim fg As Color = tabPane.getForegroundAt(tabIndex)
					If isSelected AndAlso (TypeOf fg Is UIResource) Then
						Dim selectedFG As Color = UIManager.getColor("TabbedPane.selectedForeground")
						If selectedFG IsNot Nothing Then fg = selectedFG
					End If
					g.color = fg
					sun.swing.SwingUtilities2.drawStringUnderlineCharAt(tabPane, g, title, mnemIndex, textRect.x, textRect.y + metrics.ascent)
 ' tab disabled
				Else
					g.color = tabPane.getBackgroundAt(tabIndex).brighter()
					sun.swing.SwingUtilities2.drawStringUnderlineCharAt(tabPane, g, title, mnemIndex, textRect.x, textRect.y + metrics.ascent)
					g.color = tabPane.getBackgroundAt(tabIndex).darker()
					sun.swing.SwingUtilities2.drawStringUnderlineCharAt(tabPane, g, title, mnemIndex, textRect.x - 1, textRect.y + metrics.ascent - 1)

				End If
			End If
		End Sub


		Protected Friend Overridable Function getTabLabelShiftX(ByVal tabPlacement As Integer, ByVal tabIndex As Integer, ByVal isSelected As Boolean) As Integer
			Dim tabRect As Rectangle = rects(tabIndex)
			Dim propKey As String = (If(isSelected, "selectedLabelShift", "labelShift"))
			Dim nudge As Integer = sun.swing.DefaultLookup.getInt(tabPane, Me, "TabbedPane." & propKey, 1)

			Select Case tabPlacement
				Case LEFT
					Return nudge
				Case RIGHT
					Return -nudge
				Case Else
					Return tabRect.width Mod 2
			End Select
		End Function

		Protected Friend Overridable Function getTabLabelShiftY(ByVal tabPlacement As Integer, ByVal tabIndex As Integer, ByVal isSelected As Boolean) As Integer
			Dim tabRect As Rectangle = rects(tabIndex)
			Dim nudge As Integer = (If(isSelected, sun.swing.DefaultLookup.getInt(tabPane, Me, "TabbedPane.selectedLabelShift", -1), sun.swing.DefaultLookup.getInt(tabPane, Me, "TabbedPane.labelShift", 1)))

			Select Case tabPlacement
				Case BOTTOM
					Return -nudge
				Case LEFT, RIGHT
					Return tabRect.height Mod 2
				Case Else
					Return nudge
			End Select
		End Function

		Protected Friend Overridable Sub paintFocusIndicator(ByVal g As Graphics, ByVal tabPlacement As Integer, ByVal rects As Rectangle(), ByVal tabIndex As Integer, ByVal iconRect As Rectangle, ByVal textRect As Rectangle, ByVal isSelected As Boolean)
			Dim tabRect As Rectangle = rects(tabIndex)
			If tabPane.hasFocus() AndAlso isSelected Then
				Dim x, y, w, h As Integer
				g.color = focus
				Select Case tabPlacement
				  Case LEFT
					  x = tabRect.x + 3
					  y = tabRect.y + 3
					  w = tabRect.width - 5
					  h = tabRect.height - 6
				  Case RIGHT
					  x = tabRect.x + 2
					  y = tabRect.y + 3
					  w = tabRect.width - 5
					  h = tabRect.height - 6
				  Case BOTTOM
					  x = tabRect.x + 3
					  y = tabRect.y + 2
					  w = tabRect.width - 6
					  h = tabRect.height - 5
				  Case Else
					  x = tabRect.x + 3
					  y = tabRect.y + 3
					  w = tabRect.width - 6
					  h = tabRect.height - 5
				End Select
				BasicGraphicsUtils.drawDashedRect(g, x, y, w, h)
			End If
		End Sub

		''' <summary>
		''' this function draws the border around each tab
		''' note that this function does now draw the background of the tab.
		''' that is done elsewhere
		''' </summary>
		Protected Friend Overridable Sub paintTabBorder(ByVal g As Graphics, ByVal tabPlacement As Integer, ByVal tabIndex As Integer, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal isSelected As Boolean)
			g.color = lightHighlight

			Select Case tabPlacement
			  Case LEFT
				  g.drawLine(x+1, y+h-2, x+1, y+h-2) ' bottom-left highlight
				  g.drawLine(x, y+2, x, y+h-3) ' left highlight
				  g.drawLine(x+1, y+1, x+1, y+1) ' top-left highlight
				  g.drawLine(x+2, y, x+w-1, y) ' top highlight

				  g.color = shadow
				  g.drawLine(x+2, y+h-2, x+w-1, y+h-2) ' bottom shadow

				  g.color = darkShadow
				  g.drawLine(x+2, y+h-1, x+w-1, y+h-1) ' bottom dark shadow
			  Case RIGHT
				  g.drawLine(x, y, x+w-3, y) ' top highlight

				  g.color = shadow
				  g.drawLine(x, y+h-2, x+w-3, y+h-2) ' bottom shadow
				  g.drawLine(x+w-2, y+2, x+w-2, y+h-3) ' right shadow

				  g.color = darkShadow
				  g.drawLine(x+w-2, y+1, x+w-2, y+1) ' top-right dark shadow
				  g.drawLine(x+w-2, y+h-2, x+w-2, y+h-2) ' bottom-right dark shadow
				  g.drawLine(x+w-1, y+2, x+w-1, y+h-3) ' right dark shadow
				  g.drawLine(x, y+h-1, x+w-3, y+h-1) ' bottom dark shadow
			  Case BOTTOM
				  g.drawLine(x, y, x, y+h-3) ' left highlight
				  g.drawLine(x+1, y+h-2, x+1, y+h-2) ' bottom-left highlight

				  g.color = shadow
				  g.drawLine(x+2, y+h-2, x+w-3, y+h-2) ' bottom shadow
				  g.drawLine(x+w-2, y, x+w-2, y+h-3) ' right shadow

				  g.color = darkShadow
				  g.drawLine(x+2, y+h-1, x+w-3, y+h-1) ' bottom dark shadow
				  g.drawLine(x+w-2, y+h-2, x+w-2, y+h-2) ' bottom-right dark shadow
				  g.drawLine(x+w-1, y, x+w-1, y+h-3) ' right dark shadow
			  Case Else
				  g.drawLine(x, y+2, x, y+h-1) ' left highlight
				  g.drawLine(x+1, y+1, x+1, y+1) ' top-left highlight
				  g.drawLine(x+2, y, x+w-3, y) ' top highlight

				  g.color = shadow
				  g.drawLine(x+w-2, y+2, x+w-2, y+h-1) ' right shadow

				  g.color = darkShadow
				  g.drawLine(x+w-1, y+2, x+w-1, y+h-1) ' right dark-shadow
				  g.drawLine(x+w-2, y+1, x+w-2, y+1) ' top-right shadow
			End Select
		End Sub

		Protected Friend Overridable Sub paintTabBackground(ByVal g As Graphics, ByVal tabPlacement As Integer, ByVal tabIndex As Integer, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal isSelected As Boolean)
			g.color = If((Not isSelected) OrElse selectedColor Is Nothing, tabPane.getBackgroundAt(tabIndex), selectedColor)
			Select Case tabPlacement
			  Case LEFT
				  g.fillRect(x+1, y+1, w-1, h-3)
			  Case RIGHT
				  g.fillRect(x, y+1, w-2, h-3)
			  Case BOTTOM
				  g.fillRect(x+1, y, w-3, h-1)
			  Case Else
				  g.fillRect(x+1, y+1, w-3, h-1)
			End Select
		End Sub

		Protected Friend Overridable Sub paintContentBorder(ByVal g As Graphics, ByVal tabPlacement As Integer, ByVal selectedIndex As Integer)
			Dim width As Integer = tabPane.width
			Dim height As Integer = tabPane.height
			Dim insets As Insets = tabPane.insets
			Dim ___tabAreaInsets As Insets = getTabAreaInsets(tabPlacement)

			Dim x As Integer = insets.left
			Dim y As Integer = insets.top
			Dim w As Integer = width - insets.right - insets.left
			Dim h As Integer = height - insets.top - insets.bottom

			Select Case tabPlacement
			  Case LEFT
				  x += calculateTabAreaWidth(tabPlacement, runCount, maxTabWidth)
				  If tabsOverlapBorder Then x -= ___tabAreaInsets.right
				  w -= (x - insets.left)
			  Case RIGHT
				  w -= calculateTabAreaWidth(tabPlacement, runCount, maxTabWidth)
				  If tabsOverlapBorder Then w += ___tabAreaInsets.left
			  Case BOTTOM
				  h -= calculateTabAreaHeight(tabPlacement, runCount, maxTabHeight)
				  If tabsOverlapBorder Then h += ___tabAreaInsets.top
			  Case Else
				  y += calculateTabAreaHeight(tabPlacement, runCount, maxTabHeight)
				  If tabsOverlapBorder Then y -= ___tabAreaInsets.bottom
				  h -= (y - insets.top)
			End Select

				If tabPane.tabCount > 0 AndAlso (contentOpaque OrElse tabPane.opaque) Then
				' Fill region behind content area
				Dim color As Color = UIManager.getColor("TabbedPane.contentAreaColor")
				If color IsNot Nothing Then
					g.color = color
				ElseIf selectedColor Is Nothing OrElse selectedIndex = -1 Then
					g.color = tabPane.background
				Else
					g.color = selectedColor
				End If
				g.fillRect(x,y,w,h)
				End If

			paintContentBorderTopEdge(g, tabPlacement, selectedIndex, x, y, w, h)
			paintContentBorderLeftEdge(g, tabPlacement, selectedIndex, x, y, w, h)
			paintContentBorderBottomEdge(g, tabPlacement, selectedIndex, x, y, w, h)
			paintContentBorderRightEdge(g, tabPlacement, selectedIndex, x, y, w, h)

		End Sub

		Protected Friend Overridable Sub paintContentBorderTopEdge(ByVal g As Graphics, ByVal tabPlacement As Integer, ByVal selectedIndex As Integer, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			Dim selRect As Rectangle = If(selectedIndex < 0, Nothing, getTabBounds(selectedIndex, calcRect))

			g.color = lightHighlight

			' Draw unbroken line if tabs are not on TOP, OR
			' selected tab is not in run adjacent to content, OR
			' selected tab is not visible (SCROLL_TAB_LAYOUT)
			'
			If tabPlacement <> TOP OrElse selectedIndex < 0 OrElse (selRect.y + selRect.height + 1 < y) OrElse (selRect.x < x OrElse selRect.x > x + w) Then
				g.drawLine(x, y, x+w-2, y)
			Else
				' Break line to show visual connection to selected tab
				g.drawLine(x, y, selRect.x - 1, y)
				If selRect.x + selRect.width < x + w - 2 Then
					g.drawLine(selRect.x + selRect.width, y, x+w-2, y)
				Else
					g.color = shadow
					g.drawLine(x+w-2, y, x+w-2, y)
				End If
			End If
		End Sub

		Protected Friend Overridable Sub paintContentBorderLeftEdge(ByVal g As Graphics, ByVal tabPlacement As Integer, ByVal selectedIndex As Integer, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			Dim selRect As Rectangle = If(selectedIndex < 0, Nothing, getTabBounds(selectedIndex, calcRect))

			g.color = lightHighlight

			' Draw unbroken line if tabs are not on LEFT, OR
			' selected tab is not in run adjacent to content, OR
			' selected tab is not visible (SCROLL_TAB_LAYOUT)
			'
			If tabPlacement <> LEFT OrElse selectedIndex < 0 OrElse (selRect.x + selRect.width + 1 < x) OrElse (selRect.y < y OrElse selRect.y > y + h) Then
				g.drawLine(x, y, x, y+h-2)
			Else
				' Break line to show visual connection to selected tab
				g.drawLine(x, y, x, selRect.y - 1)
				If selRect.y + selRect.height < y + h - 2 Then g.drawLine(x, selRect.y + selRect.height, x, y+h-2)
			End If
		End Sub

		Protected Friend Overridable Sub paintContentBorderBottomEdge(ByVal g As Graphics, ByVal tabPlacement As Integer, ByVal selectedIndex As Integer, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			Dim selRect As Rectangle = If(selectedIndex < 0, Nothing, getTabBounds(selectedIndex, calcRect))

			g.color = shadow

			' Draw unbroken line if tabs are not on BOTTOM, OR
			' selected tab is not in run adjacent to content, OR
			' selected tab is not visible (SCROLL_TAB_LAYOUT)
			'
			If tabPlacement <> BOTTOM OrElse selectedIndex < 0 OrElse (selRect.y - 1 > h) OrElse (selRect.x < x OrElse selRect.x > x + w) Then
				g.drawLine(x+1, y+h-2, x+w-2, y+h-2)
				g.color = darkShadow
				g.drawLine(x, y+h-1, x+w-1, y+h-1)
			Else
				' Break line to show visual connection to selected tab
				g.drawLine(x+1, y+h-2, selRect.x - 1, y+h-2)
				g.color = darkShadow
				g.drawLine(x, y+h-1, selRect.x - 1, y+h-1)
				If selRect.x + selRect.width < x + w - 2 Then
					g.color = shadow
					g.drawLine(selRect.x + selRect.width, y+h-2, x+w-2, y+h-2)
					g.color = darkShadow
					g.drawLine(selRect.x + selRect.width, y+h-1, x+w-1, y+h-1)
				End If
			End If

		End Sub

		Protected Friend Overridable Sub paintContentBorderRightEdge(ByVal g As Graphics, ByVal tabPlacement As Integer, ByVal selectedIndex As Integer, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			Dim selRect As Rectangle = If(selectedIndex < 0, Nothing, getTabBounds(selectedIndex, calcRect))

			g.color = shadow

			' Draw unbroken line if tabs are not on RIGHT, OR
			' selected tab is not in run adjacent to content, OR
			' selected tab is not visible (SCROLL_TAB_LAYOUT)
			'
			If tabPlacement <> RIGHT OrElse selectedIndex < 0 OrElse (selRect.x - 1 > w) OrElse (selRect.y < y OrElse selRect.y > y + h) Then
				g.drawLine(x+w-2, y+1, x+w-2, y+h-3)
				g.color = darkShadow
				g.drawLine(x+w-1, y, x+w-1, y+h-1)
			Else
				' Break line to show visual connection to selected tab
				g.drawLine(x+w-2, y+1, x+w-2, selRect.y - 1)
				g.color = darkShadow
				g.drawLine(x+w-1, y, x+w-1, selRect.y - 1)

				If selRect.y + selRect.height < y + h - 2 Then
					g.color = shadow
					g.drawLine(x+w-2, selRect.y + selRect.height, x+w-2, y+h-2)
					g.color = darkShadow
					g.drawLine(x+w-1, selRect.y + selRect.height, x+w-1, y+h-2)
				End If
			End If
		End Sub

		Private Sub ensureCurrentLayout()
			If Not tabPane.valid Then tabPane.validate()
	'         If tabPane doesn't have a peer yet, the validate() call will
	'         * silently fail.  We handle that by forcing a layout if tabPane
	'         * is still invalid.  See bug 4237677.
	'         
			If Not tabPane.valid Then
				Dim layout As TabbedPaneLayout = CType(tabPane.layout, TabbedPaneLayout)
				layout.calculateLayoutInfo()
			End If
		End Sub


	' TabbedPaneUI methods

		''' <summary>
		''' Returns the bounds of the specified tab index.  The bounds are
		''' with respect to the JTabbedPane's coordinate space.
		''' </summary>
		Public Overridable Function getTabBounds(ByVal pane As JTabbedPane, ByVal i As Integer) As Rectangle
			ensureCurrentLayout()
			Dim tabRect As New Rectangle
			Return getTabBounds(i, tabRect)
		End Function

		Public Overridable Function getTabRunCount(ByVal pane As JTabbedPane) As Integer
			ensureCurrentLayout()
			Return runCount
		End Function

		''' <summary>
		''' Returns the tab index which intersects the specified point
		''' in the JTabbedPane's coordinate space.
		''' </summary>
		Public Overridable Function tabForCoordinate(ByVal pane As JTabbedPane, ByVal x As Integer, ByVal y As Integer) As Integer
			Return tabForCoordinate(pane, x, y, True)
		End Function

		Private Function tabForCoordinate(ByVal pane As JTabbedPane, ByVal x As Integer, ByVal y As Integer, ByVal validateIfNecessary As Boolean) As Integer
			If validateIfNecessary Then ensureCurrentLayout()
			If isRunsDirty Then Return -1
			Dim p As New Point(x, y)

			If scrollableTabLayoutEnabled() Then
				translatePointToTabPanel(x, y, p)
				Dim viewRect As Rectangle = tabScroller.viewport.viewRect
				If Not viewRect.contains(p) Then Return -1
			End If
			Dim tabCount As Integer = tabPane.tabCount
			For i As Integer = 0 To tabCount - 1
				If rects(i).contains(p.x, p.y) Then Return i
			Next i
			Return -1
		End Function

		''' <summary>
		''' Returns the bounds of the specified tab in the coordinate space
		''' of the JTabbedPane component.  This is required because the tab rects
		''' are by default defined in the coordinate space of the component where
		''' they are rendered, which could be the JTabbedPane
		''' (for WRAP_TAB_LAYOUT) or a ScrollableTabPanel (SCROLL_TAB_LAYOUT).
		''' This method should be used whenever the tab rectangle must be relative
		''' to the JTabbedPane itself and the result should be placed in a
		''' designated Rectangle object (rather than instantiating and returning
		''' a new Rectangle each time). The tab index parameter must be a valid
		''' tabbed pane tab index (0 to tab count - 1, inclusive).  The destination
		''' rectangle parameter must be a valid <code>Rectangle</code> instance.
		''' The handling of invalid parameters is unspecified.
		''' </summary>
		''' <param name="tabIndex"> the index of the tab </param>
		''' <param name="dest"> the rectangle where the result should be placed </param>
		''' <returns> the resulting rectangle
		''' 
		''' @since 1.4 </returns>
		Protected Friend Overridable Function getTabBounds(ByVal tabIndex As Integer, ByVal dest As Rectangle) As Rectangle
			dest.width = rects(tabIndex).width
			dest.height = rects(tabIndex).height

			If scrollableTabLayoutEnabled() Then ' SCROLL_TAB_LAYOUT
				' Need to translate coordinates based on viewport location &
				' view position
				Dim vpp As Point = tabScroller.viewport.location
				Dim viewp As Point = tabScroller.viewport.viewPosition
				dest.x = rects(tabIndex).x + vpp.x - viewp.x
				dest.y = rects(tabIndex).y + vpp.y - viewp.y
 ' WRAP_TAB_LAYOUT
			Else
				dest.x = rects(tabIndex).x
				dest.y = rects(tabIndex).y
			End If
			Return dest
		End Function

		''' <summary>
		''' Returns the index of the tab closest to the passed in location, note
		''' that the returned tab may not contain the location x,y.
		''' </summary>
		Private Function getClosestTab(ByVal x As Integer, ByVal y As Integer) As Integer
			Dim min As Integer = 0
			Dim tabCount As Integer = Math.Min(rects.Length, tabPane.tabCount)
			Dim max As Integer = tabCount
			Dim tabPlacement As Integer = tabPane.tabPlacement
			Dim useX As Boolean = (tabPlacement = TOP OrElse tabPlacement = BOTTOM)
			Dim want As Integer = If(useX, x, y)

			Do While min <> max
				Dim current As Integer = (max + min) \ 2
				Dim minLoc As Integer
				Dim maxLoc As Integer

				If useX Then
					minLoc = rects(current).x
					maxLoc = minLoc + rects(current).width
				Else
					minLoc = rects(current).y
					maxLoc = minLoc + rects(current).height
				End If
				If want < minLoc Then
					max = current
					If min = max Then Return Math.Max(0, current - 1)
				ElseIf want >= maxLoc Then
					min = current
					If max - min <= 1 Then Return Math.Max(current + 1, tabCount - 1)
				Else
					Return current
				End If
			Loop
			Return min
		End Function

		''' <summary>
		''' Returns a point which is translated from the specified point in the
		''' JTabbedPane's coordinate space to the coordinate space of the
		''' ScrollableTabPanel.  This is used for SCROLL_TAB_LAYOUT ONLY.
		''' </summary>
		Private Function translatePointToTabPanel(ByVal srcx As Integer, ByVal srcy As Integer, ByVal dest As Point) As Point
			Dim vpp As Point = tabScroller.viewport.location
			Dim viewp As Point = tabScroller.viewport.viewPosition
			dest.x = srcx - vpp.x + viewp.x
			dest.y = srcy - vpp.y + viewp.y
			Return dest
		End Function

	' BasicTabbedPaneUI methods

		Protected Friend Overridable Property visibleComponent As Component
			Get
				Return visibleComponent
			End Get
			Set(ByVal component As Component)
				If visibleComponent IsNot Nothing AndAlso visibleComponent IsNot component AndAlso visibleComponent.parent Is tabPane AndAlso visibleComponent.visible Then visibleComponent.visible = False
				If component IsNot Nothing AndAlso (Not component.visible) Then component.visible = True
				visibleComponent = component
			End Set
		End Property


		Protected Friend Overridable Sub assureRectsCreated(ByVal tabCount As Integer)
			Dim rectArrayLen As Integer = rects.Length
			If tabCount <> rectArrayLen Then
				Dim tempRectArray As Rectangle() = New Rectangle(tabCount - 1){}
				Array.Copy(rects, 0, tempRectArray, 0, Math.Min(rectArrayLen, tabCount))
				rects = tempRectArray
				For rectIndex As Integer = rectArrayLen To tabCount - 1
					rects(rectIndex) = New Rectangle
				Next rectIndex
			End If

		End Sub

		Protected Friend Overridable Sub expandTabRunsArray()
			Dim rectLen As Integer = tabRuns.Length
			Dim newArray As Integer() = New Integer(rectLen+10 - 1){}
			Array.Copy(tabRuns, 0, newArray, 0, runCount)
			tabRuns = newArray
		End Sub

		Protected Friend Overridable Function getRunForTab(ByVal tabCount As Integer, ByVal tabIndex As Integer) As Integer
			For i As Integer = 0 To runCount - 1
				Dim first As Integer = tabRuns(i)
				Dim last As Integer = lastTabInRun(tabCount, i)
				If tabIndex >= first AndAlso tabIndex <= last Then Return i
			Next i
			Return 0
		End Function

		Protected Friend Overridable Function lastTabInRun(ByVal tabCount As Integer, ByVal run As Integer) As Integer
			If runCount = 1 Then Return tabCount - 1
			Dim nextRun As Integer = (If(run = runCount - 1, 0, run + 1))
			If tabRuns(nextRun) = 0 Then Return tabCount - 1
			Return tabRuns(nextRun)-1
		End Function

		Protected Friend Overridable Function getTabRunOverlay(ByVal tabPlacement As Integer) As Integer
			Return tabRunOverlay
		End Function

		Protected Friend Overridable Function getTabRunIndent(ByVal tabPlacement As Integer, ByVal run As Integer) As Integer
			Return 0
		End Function

		Protected Friend Overridable Function shouldPadTabRun(ByVal tabPlacement As Integer, ByVal run As Integer) As Boolean
			Return runCount > 1
		End Function

		Protected Friend Overridable Function shouldRotateTabRuns(ByVal tabPlacement As Integer) As Boolean
			Return True
		End Function

		Protected Friend Overridable Function getIconForTab(ByVal tabIndex As Integer) As Icon
			Return If((Not tabPane.enabled) OrElse (Not tabPane.isEnabledAt(tabIndex)), tabPane.getDisabledIconAt(tabIndex), tabPane.getIconAt(tabIndex))
		End Function

		''' <summary>
		''' Returns the text View object required to render stylized text (HTML) for
		''' the specified tab or null if no specialized text rendering is needed
		''' for this tab. This is provided to support html rendering inside tabs.
		''' </summary>
		''' <param name="tabIndex"> the index of the tab </param>
		''' <returns> the text view to render the tab's text or null if no
		'''         specialized rendering is required
		''' 
		''' @since 1.4 </returns>
		Protected Friend Overridable Function getTextViewForTab(ByVal tabIndex As Integer) As javax.swing.text.View
			If htmlViews IsNot Nothing Then Return htmlViews(tabIndex)
			Return Nothing
		End Function

		Protected Friend Overridable Function calculateTabHeight(ByVal tabPlacement As Integer, ByVal tabIndex As Integer, ByVal fontHeight As Integer) As Integer
			Dim height As Integer = 0
			Dim c As Component = tabPane.getTabComponentAt(tabIndex)
			If c IsNot Nothing Then
				height = c.preferredSize.height
			Else
				Dim v As javax.swing.text.View = getTextViewForTab(tabIndex)
				If v IsNot Nothing Then
					' html
					height += CInt(Fix(v.getPreferredSpan(javax.swing.text.View.Y_AXIS)))
				Else
					' plain text
					height += fontHeight
				End If
				Dim icon As Icon = getIconForTab(tabIndex)

				If icon IsNot Nothing Then height = Math.Max(height, icon.iconHeight)
			End If
			Dim ___tabInsets As Insets = getTabInsets(tabPlacement, tabIndex)
			height += ___tabInsets.top + ___tabInsets.bottom + 2
			Return height
		End Function

		Protected Friend Overridable Function calculateMaxTabHeight(ByVal tabPlacement As Integer) As Integer
			Dim metrics As FontMetrics = fontMetrics
			Dim tabCount As Integer = tabPane.tabCount
			Dim result As Integer = 0
			Dim fontHeight As Integer = metrics.height
			For i As Integer = 0 To tabCount - 1
				result = Math.Max(calculateTabHeight(tabPlacement, i, fontHeight), result)
			Next i
			Return result
		End Function

		Protected Friend Overridable Function calculateTabWidth(ByVal tabPlacement As Integer, ByVal tabIndex As Integer, ByVal metrics As FontMetrics) As Integer
			Dim ___tabInsets As Insets = getTabInsets(tabPlacement, tabIndex)
			Dim width As Integer = ___tabInsets.left + ___tabInsets.right + 3
			Dim tabComponent As Component = tabPane.getTabComponentAt(tabIndex)
			If tabComponent IsNot Nothing Then
				width += tabComponent.preferredSize.width
			Else
				Dim icon As Icon = getIconForTab(tabIndex)
				If icon IsNot Nothing Then width += icon.iconWidth + textIconGap
				Dim v As javax.swing.text.View = getTextViewForTab(tabIndex)
				If v IsNot Nothing Then
					' html
					width += CInt(Fix(v.getPreferredSpan(javax.swing.text.View.X_AXIS)))
				Else
					' plain text
					Dim title As String = tabPane.getTitleAt(tabIndex)
					width += sun.swing.SwingUtilities2.stringWidth(tabPane, metrics, title)
				End If
			End If
			Return width
		End Function

		Protected Friend Overridable Function calculateMaxTabWidth(ByVal tabPlacement As Integer) As Integer
			Dim metrics As FontMetrics = fontMetrics
			Dim tabCount As Integer = tabPane.tabCount
			Dim result As Integer = 0
			For i As Integer = 0 To tabCount - 1
				result = Math.Max(calculateTabWidth(tabPlacement, i, metrics), result)
			Next i
			Return result
		End Function

		Protected Friend Overridable Function calculateTabAreaHeight(ByVal tabPlacement As Integer, ByVal horizRunCount As Integer, ByVal maxTabHeight As Integer) As Integer
			Dim ___tabAreaInsets As Insets = getTabAreaInsets(tabPlacement)
			Dim ___tabRunOverlay As Integer = getTabRunOverlay(tabPlacement)
			Return (If(horizRunCount > 0, horizRunCount * (maxTabHeight-___tabRunOverlay) + ___tabRunOverlay + ___tabAreaInsets.top + ___tabAreaInsets.bottom, 0))
		End Function

		Protected Friend Overridable Function calculateTabAreaWidth(ByVal tabPlacement As Integer, ByVal vertRunCount As Integer, ByVal maxTabWidth As Integer) As Integer
			Dim ___tabAreaInsets As Insets = getTabAreaInsets(tabPlacement)
			Dim ___tabRunOverlay As Integer = getTabRunOverlay(tabPlacement)
			Return (If(vertRunCount > 0, vertRunCount * (maxTabWidth-___tabRunOverlay) + ___tabRunOverlay + ___tabAreaInsets.left + ___tabAreaInsets.right, 0))
		End Function

		Protected Friend Overridable Function getTabInsets(ByVal tabPlacement As Integer, ByVal tabIndex As Integer) As Insets
			Return tabInsets
		End Function

		Protected Friend Overridable Function getSelectedTabPadInsets(ByVal tabPlacement As Integer) As Insets
			rotateInsets(selectedTabPadInsets, currentPadInsets, tabPlacement)
			Return currentPadInsets
		End Function

		Protected Friend Overridable Function getTabAreaInsets(ByVal tabPlacement As Integer) As Insets
			rotateInsets(tabAreaInsets, currentTabAreaInsets, tabPlacement)
			Return currentTabAreaInsets
		End Function

		Protected Friend Overridable Function getContentBorderInsets(ByVal tabPlacement As Integer) As Insets
			Return contentBorderInsets
		End Function

		Protected Friend Overridable Property fontMetrics As FontMetrics
			Get
				Dim font As Font = tabPane.font
				Return tabPane.getFontMetrics(font)
			End Get
		End Property


	' Tab Navigation methods

		Protected Friend Overridable Sub navigateSelectedTab(ByVal direction As Integer)
			Dim tabPlacement As Integer = tabPane.tabPlacement
			Dim current As Integer = If(sun.swing.DefaultLookup.getBoolean(tabPane, Me, "TabbedPane.selectionFollowsFocus", True), tabPane.selectedIndex, focusIndex)
			Dim tabCount As Integer = tabPane.tabCount
			Dim leftToRight As Boolean = BasicGraphicsUtils.isLeftToRight(tabPane)

			' If we have no tabs then don't navigate.
			If tabCount <= 0 Then Return

			Dim offset As Integer
			Select Case tabPlacement
			  Case LEFT, RIGHT
				  Select Case direction
					 Case NEXT
						 selectNextTab(current)
					 Case PREVIOUS
						 selectPreviousTab(current)
					Case NORTH
						selectPreviousTabInRun(current)
					Case SOUTH
						selectNextTabInRun(current)
					Case WEST
						offset = getTabRunOffset(tabPlacement, tabCount, current, False)
						selectAdjacentRunTab(tabPlacement, current, offset)
					Case EAST
						offset = getTabRunOffset(tabPlacement, tabCount, current, True)
						selectAdjacentRunTab(tabPlacement, current, offset)
					Case Else
				  End Select
			  Case Else
				  Select Case direction
					Case NEXT
						selectNextTab(current)
					Case PREVIOUS
						selectPreviousTab(current)
					Case NORTH
						offset = getTabRunOffset(tabPlacement, tabCount, current, False)
						selectAdjacentRunTab(tabPlacement, current, offset)
					Case SOUTH
						offset = getTabRunOffset(tabPlacement, tabCount, current, True)
						selectAdjacentRunTab(tabPlacement, current, offset)
					Case EAST
						If leftToRight Then
							selectNextTabInRun(current)
						Else
							selectPreviousTabInRun(current)
						End If
					Case WEST
						If leftToRight Then
							selectPreviousTabInRun(current)
						Else
							selectNextTabInRun(current)
						End If
					Case Else
				  End Select
			End Select
		End Sub

		Protected Friend Overridable Sub selectNextTabInRun(ByVal current As Integer)
			Dim tabCount As Integer = tabPane.tabCount
			Dim tabIndex As Integer = getNextTabIndexInRun(tabCount, current)

			Do While tabIndex <> current AndAlso Not tabPane.isEnabledAt(tabIndex)
				tabIndex = getNextTabIndexInRun(tabCount, tabIndex)
			Loop
			navigateTo(tabIndex)
		End Sub

		Protected Friend Overridable Sub selectPreviousTabInRun(ByVal current As Integer)
			Dim tabCount As Integer = tabPane.tabCount
			Dim tabIndex As Integer = getPreviousTabIndexInRun(tabCount, current)

			Do While tabIndex <> current AndAlso Not tabPane.isEnabledAt(tabIndex)
				tabIndex = getPreviousTabIndexInRun(tabCount, tabIndex)
			Loop
			navigateTo(tabIndex)
		End Sub

		Protected Friend Overridable Sub selectNextTab(ByVal current As Integer)
			Dim tabIndex As Integer = getNextTabIndex(current)

			Do While tabIndex <> current AndAlso Not tabPane.isEnabledAt(tabIndex)
				tabIndex = getNextTabIndex(tabIndex)
			Loop
			navigateTo(tabIndex)
		End Sub

		Protected Friend Overridable Sub selectPreviousTab(ByVal current As Integer)
			Dim tabIndex As Integer = getPreviousTabIndex(current)

			Do While tabIndex <> current AndAlso Not tabPane.isEnabledAt(tabIndex)
				tabIndex = getPreviousTabIndex(tabIndex)
			Loop
			navigateTo(tabIndex)
		End Sub

		Protected Friend Overridable Sub selectAdjacentRunTab(ByVal tabPlacement As Integer, ByVal tabIndex As Integer, ByVal offset As Integer)
			If runCount < 2 Then Return
			Dim newIndex As Integer
			Dim r As Rectangle = rects(tabIndex)
			Select Case tabPlacement
			  Case LEFT, RIGHT
				  newIndex = tabForCoordinate(tabPane, r.x + r.width/2 + offset, r.y + r.height/2)
			  Case Else
				  newIndex = tabForCoordinate(tabPane, r.x + r.width/2, r.y + r.height/2 + offset)
			End Select
			If newIndex <> -1 Then
				Do While (Not tabPane.isEnabledAt(newIndex)) AndAlso newIndex <> tabIndex
					newIndex = getNextTabIndex(newIndex)
				Loop
				navigateTo(newIndex)
			End If
		End Sub

		Private Sub navigateTo(ByVal index As Integer)
			If sun.swing.DefaultLookup.getBoolean(tabPane, Me, "TabbedPane.selectionFollowsFocus", True) Then
				tabPane.selectedIndex = index
			Else
				' Just move focus (not selection)
				focusIndexdex(index, True)
			End If
		End Sub

		Friend Overridable Sub setFocusIndex(ByVal index As Integer, ByVal repaint As Boolean)
			If repaint AndAlso (Not isRunsDirty) Then
				repaintTab(focusIndex)
				focusIndex = index
				repaintTab(focusIndex)
			Else
				focusIndex = index
			End If
		End Sub

		''' <summary>
		''' Repaints the specified tab.
		''' </summary>
		Private Sub repaintTab(ByVal index As Integer)
			' If we're not valid that means we will shortly be validated and
			' painted, which means we don't have to do anything here.
			If (Not isRunsDirty) AndAlso index >= 0 AndAlso index < tabPane.tabCount Then tabPane.repaint(getTabBounds(tabPane, index))
		End Sub

		''' <summary>
		''' Makes sure the focusIndex is valid.
		''' </summary>
		Private Sub validateFocusIndex()
			If focusIndex >= tabPane.tabCount Then focusIndexdex(tabPane.selectedIndex, False)
		End Sub

		''' <summary>
		''' Returns the index of the tab that has focus.
		''' </summary>
		''' <returns> index of tab that has focus
		''' @since 1.5 </returns>
		Protected Friend Overridable Property focusIndex As Integer
			Get
				Return focusIndex
			End Get
		End Property

		Protected Friend Overridable Function getTabRunOffset(ByVal tabPlacement As Integer, ByVal tabCount As Integer, ByVal tabIndex As Integer, ByVal forward As Boolean) As Integer
			Dim run As Integer = getRunForTab(tabCount, tabIndex)
			Dim offset As Integer
			Select Case tabPlacement
			  Case LEFT
				  If run = 0 Then
					  offset = (If(forward, -(calculateTabAreaWidth(tabPlacement, runCount, maxTabWidth)-maxTabWidth), -maxTabWidth))

				  ElseIf run = runCount - 1 Then
					  offset = (If(forward, maxTabWidth, calculateTabAreaWidth(tabPlacement, runCount, maxTabWidth)-maxTabWidth))
				  Else
					  offset = (If(forward, maxTabWidth, -maxTabWidth))
				  End If
				  Exit Select
			  Case RIGHT
				  If run = 0 Then
					  offset = (If(forward, maxTabWidth, calculateTabAreaWidth(tabPlacement, runCount, maxTabWidth)-maxTabWidth))
				  ElseIf run = runCount - 1 Then
					  offset = (If(forward, -(calculateTabAreaWidth(tabPlacement, runCount, maxTabWidth)-maxTabWidth), -maxTabWidth))
				  Else
					  offset = (If(forward, maxTabWidth, -maxTabWidth))
				  End If
				  Exit Select
			  Case BOTTOM
				  If run = 0 Then
					  offset = (If(forward, maxTabHeight, calculateTabAreaHeight(tabPlacement, runCount, maxTabHeight)-maxTabHeight))
				  ElseIf run = runCount - 1 Then
					  offset = (If(forward, -(calculateTabAreaHeight(tabPlacement, runCount, maxTabHeight)-maxTabHeight), -maxTabHeight))
				  Else
					  offset = (If(forward, maxTabHeight, -maxTabHeight))
				  End If
				  Exit Select
			  Case Else
				  If run = 0 Then
					  offset = (If(forward, -(calculateTabAreaHeight(tabPlacement, runCount, maxTabHeight)-maxTabHeight), -maxTabHeight))
				  ElseIf run = runCount - 1 Then
					  offset = (If(forward, maxTabHeight, calculateTabAreaHeight(tabPlacement, runCount, maxTabHeight)-maxTabHeight))
				  Else
					  offset = (If(forward, maxTabHeight, -maxTabHeight))
				  End If
			End Select
			Return offset
		End Function

		Protected Friend Overridable Function getPreviousTabIndex(ByVal base As Integer) As Integer
			Dim tabIndex As Integer = (If(base - 1 >= 0, base - 1, tabPane.tabCount - 1))
			Return (If(tabIndex >= 0, tabIndex, 0))
		End Function

		Protected Friend Overridable Function getNextTabIndex(ByVal base As Integer) As Integer
			Return (base+1) Mod tabPane.tabCount
		End Function

		Protected Friend Overridable Function getNextTabIndexInRun(ByVal tabCount As Integer, ByVal base As Integer) As Integer
			If runCount < 2 Then Return getNextTabIndex(base)
			Dim currentRun As Integer = getRunForTab(tabCount, base)
			Dim [next] As Integer = getNextTabIndex(base)
			If [next] = tabRuns(getNextTabRun(currentRun)) Then Return tabRuns(currentRun)
			Return [next]
		End Function

		Protected Friend Overridable Function getPreviousTabIndexInRun(ByVal tabCount As Integer, ByVal base As Integer) As Integer
			If runCount < 2 Then Return getPreviousTabIndex(base)
			Dim currentRun As Integer = getRunForTab(tabCount, base)
			If base = tabRuns(currentRun) Then
				Dim previous As Integer = tabRuns(getNextTabRun(currentRun))-1
				Return (If(previous <> -1, previous, tabCount-1))
			End If
			Return getPreviousTabIndex(base)
		End Function

		Protected Friend Overridable Function getPreviousTabRun(ByVal baseRun As Integer) As Integer
			Dim runIndex As Integer = (If(baseRun - 1 >= 0, baseRun - 1, runCount - 1))
			Return (If(runIndex >= 0, runIndex, 0))
		End Function

		Protected Friend Overridable Function getNextTabRun(ByVal baseRun As Integer) As Integer
			Return (baseRun+1) Mod runCount
		End Function

		Protected Friend Shared Sub rotateInsets(ByVal topInsets As Insets, ByVal targetInsets As Insets, ByVal targetPlacement As Integer)

			Select Case targetPlacement
			  Case LEFT
				  targetInsets.top = topInsets.left
				  targetInsets.left = topInsets.top
				  targetInsets.bottom = topInsets.right
				  targetInsets.right = topInsets.bottom
			  Case BOTTOM
				  targetInsets.top = topInsets.bottom
				  targetInsets.left = topInsets.left
				  targetInsets.bottom = topInsets.top
				  targetInsets.right = topInsets.right
			  Case RIGHT
				  targetInsets.top = topInsets.left
				  targetInsets.left = topInsets.bottom
				  targetInsets.bottom = topInsets.right
				  targetInsets.right = topInsets.top
			  Case Else
				  targetInsets.top = topInsets.top
				  targetInsets.left = topInsets.left
				  targetInsets.bottom = topInsets.bottom
				  targetInsets.right = topInsets.right
			End Select
		End Sub

		' REMIND(aim,7/29/98): This method should be made
		' protected in the next release where
		' API changes are allowed
		Friend Overridable Function requestFocusForVisibleComponent() As Boolean
			Return sun.swing.SwingUtilities2.tabbedPaneChangeFocusTo(visibleComponent)
		End Function

		Private Class Actions
			Inherits sun.swing.UIAction

			Friend Const [NEXT] As String = "navigateNext"
			Friend Const PREVIOUS As String = "navigatePrevious"
			Friend Const RIGHT As String = "navigateRight"
			Friend Const LEFT As String = "navigateLeft"
			Friend Const UP As String = "navigateUp"
			Friend Const DOWN As String = "navigateDown"
			Friend Const PAGE_UP As String = "navigatePageUp"
			Friend Const PAGE_DOWN As String = "navigatePageDown"
			Friend Const REQUEST_FOCUS As String = "requestFocus"
			Friend Const REQUEST_FOCUS_FOR_VISIBLE As String = "requestFocusForVisibleComponent"
			Friend Const SET_SELECTED As String = "setSelectedIndex"
			Friend Const SELECT_FOCUSED As String = "selectTabWithFocus"
			Friend Const SCROLL_FORWARD As String = "scrollTabsForwardAction"
			Friend Const SCROLL_BACKWARD As String = "scrollTabsBackwardAction"

			Friend Sub New(ByVal key As String)
				MyBase.New(key)
			End Sub

			Public Overridable Sub actionPerformed(ByVal e As ActionEvent)
				Dim key As String = name
				Dim pane As JTabbedPane = CType(e.source, JTabbedPane)
				Dim ui As BasicTabbedPaneUI = CType(BasicLookAndFeel.getUIOfType(pane.uI, GetType(BasicTabbedPaneUI)), BasicTabbedPaneUI)

				If ui Is Nothing Then Return
				If key = [NEXT] Then
					ui.navigateSelectedTab(SwingConstants.NEXT)
				ElseIf key = PREVIOUS Then
					ui.navigateSelectedTab(SwingConstants.PREVIOUS)
				ElseIf key = RIGHT Then
					ui.navigateSelectedTab(SwingConstants.EAST)
				ElseIf key = LEFT Then
					ui.navigateSelectedTab(SwingConstants.WEST)
				ElseIf key = UP Then
					ui.navigateSelectedTab(SwingConstants.NORTH)
				ElseIf key = DOWN Then
					ui.navigateSelectedTab(SwingConstants.SOUTH)
				ElseIf key = PAGE_UP Then
					Dim tabPlacement As Integer = pane.tabPlacement
					If tabPlacement = TOP OrElse tabPlacement = BOTTOM Then
						ui.navigateSelectedTab(SwingConstants.WEST)
					Else
						ui.navigateSelectedTab(SwingConstants.NORTH)
					End If
				ElseIf key = PAGE_DOWN Then
					Dim tabPlacement As Integer = pane.tabPlacement
					If tabPlacement = TOP OrElse tabPlacement = BOTTOM Then
						ui.navigateSelectedTab(SwingConstants.EAST)
					Else
						ui.navigateSelectedTab(SwingConstants.SOUTH)
					End If
				ElseIf key = REQUEST_FOCUS Then
					pane.requestFocus()
				ElseIf key = REQUEST_FOCUS_FOR_VISIBLE Then
					ui.requestFocusForVisibleComponent()
				ElseIf key = SET_SELECTED Then
					Dim command As String = e.actionCommand

					If command IsNot Nothing AndAlso command.Length > 0 Then
						Dim mnemonic As Integer = AscW(e.actionCommand.Chars(0))
						If mnemonic >= "a"c AndAlso mnemonic <="z"c Then mnemonic -= (AscW("a"c) - AscW("A"c))
						Dim index As Integer? = ui.mnemonicToIndexMap.get(Convert.ToInt32(mnemonic))
						If index IsNot Nothing AndAlso pane.isEnabledAt(index) Then pane.selectedIndex = index
					End If
				ElseIf key = SELECT_FOCUSED Then
					Dim focusIndex As Integer = ui.focusIndex
					If focusIndex <> -1 Then pane.selectedIndex = focusIndex
				ElseIf key = SCROLL_FORWARD Then
					If ui.scrollableTabLayoutEnabled() Then ui.tabScroller.scrollForward(pane.tabPlacement)
				ElseIf key = SCROLL_BACKWARD Then
					If ui.scrollableTabLayoutEnabled() Then ui.tabScroller.scrollBackward(pane.tabPlacement)
				End If
			End Sub
		End Class

		''' <summary>
		''' This class should be treated as a &quot;protected&quot; inner class.
		''' Instantiate it only within subclasses of BasicTabbedPaneUI.
		''' </summary>
		Public Class TabbedPaneLayout
			Implements LayoutManager

			Private ReadOnly outerInstance As BasicTabbedPaneUI

			Public Sub New(ByVal outerInstance As BasicTabbedPaneUI)
				Me.outerInstance = outerInstance
			End Sub


			Public Overridable Sub addLayoutComponent(ByVal name As String, ByVal comp As Component)
			End Sub

			Public Overridable Sub removeLayoutComponent(ByVal comp As Component)
			End Sub

			Public Overridable Function preferredLayoutSize(ByVal parent As Container) As Dimension
				Return calculateSize(False)
			End Function

			Public Overridable Function minimumLayoutSize(ByVal parent As Container) As Dimension
				Return calculateSize(True)
			End Function

			Protected Friend Overridable Function calculateSize(ByVal minimum As Boolean) As Dimension
				Dim tabPlacement As Integer = outerInstance.tabPane.tabPlacement
				Dim insets As Insets = outerInstance.tabPane.insets
				Dim contentInsets As Insets = outerInstance.getContentBorderInsets(tabPlacement)
				Dim tabAreaInsets As Insets = outerInstance.getTabAreaInsets(tabPlacement)

				Dim zeroSize As New Dimension(0,0)
				Dim height As Integer = 0
				Dim width As Integer = 0
				Dim cWidth As Integer = 0
				Dim cHeight As Integer = 0

				' Determine minimum size required to display largest
				' child in each dimension
				'
				For i As Integer = 0 To outerInstance.tabPane.tabCount - 1
					Dim component As Component = outerInstance.tabPane.getComponentAt(i)
					If component IsNot Nothing Then
						Dim size As Dimension = If(minimum, component.minimumSize, component.preferredSize)

						If size IsNot Nothing Then
							cHeight = Math.Max(size.height, cHeight)
							cWidth = Math.Max(size.width, cWidth)
						End If
					End If
				Next i
				' Add content border insets to minimum size
				width += cWidth
				height += cHeight
				Dim tabExtent As Integer

				' Calculate how much space the tabs will need, based on the
				' minimum size required to display largest child + content border
				'
				Select Case tabPlacement
				  Case LEFT, RIGHT
					  height = Math.Max(height, outerInstance.calculateMaxTabHeight(tabPlacement))
					  tabExtent = preferredTabAreaWidth(tabPlacement, height - tabAreaInsets.top - tabAreaInsets.bottom)
					  width += tabExtent
				  Case Else
					  width = Math.Max(width, outerInstance.calculateMaxTabWidth(tabPlacement))
					  tabExtent = preferredTabAreaHeight(tabPlacement, width - tabAreaInsets.left - tabAreaInsets.right)
					  height += tabExtent
				End Select
				Return New Dimension(width + insets.left + insets.right + contentInsets.left + contentInsets.right, height + insets.bottom + insets.top + contentInsets.top + contentInsets.bottom)

			End Function

			Protected Friend Overridable Function preferredTabAreaHeight(ByVal tabPlacement As Integer, ByVal width As Integer) As Integer
				Dim metrics As FontMetrics = outerInstance.fontMetrics
				Dim tabCount As Integer = outerInstance.tabPane.tabCount
				Dim total As Integer = 0
				If tabCount > 0 Then
					Dim rows As Integer = 1
					Dim x As Integer = 0

					Dim maxTabHeight As Integer = outerInstance.calculateMaxTabHeight(tabPlacement)

					For i As Integer = 0 To tabCount - 1
						Dim tabWidth As Integer = outerInstance.calculateTabWidth(tabPlacement, i, metrics)

						If x <> 0 AndAlso x + tabWidth > width Then
							rows += 1
							x = 0
						End If
						x += tabWidth
					Next i
					total = outerInstance.calculateTabAreaHeight(tabPlacement, rows, maxTabHeight)
				End If
				Return total
			End Function

			Protected Friend Overridable Function preferredTabAreaWidth(ByVal tabPlacement As Integer, ByVal height As Integer) As Integer
				Dim metrics As FontMetrics = outerInstance.fontMetrics
				Dim tabCount As Integer = outerInstance.tabPane.tabCount
				Dim total As Integer = 0
				If tabCount > 0 Then
					Dim columns As Integer = 1
					Dim y As Integer = 0
					Dim fontHeight As Integer = metrics.height

					outerInstance.maxTabWidth = outerInstance.calculateMaxTabWidth(tabPlacement)

					For i As Integer = 0 To tabCount - 1
						Dim tabHeight As Integer = outerInstance.calculateTabHeight(tabPlacement, i, fontHeight)

						If y <> 0 AndAlso y + tabHeight > height Then
							columns += 1
							y = 0
						End If
						y += tabHeight
					Next i
					total = outerInstance.calculateTabAreaWidth(tabPlacement, columns, outerInstance.maxTabWidth)
				End If
				Return total
			End Function

			Public Overridable Sub layoutContainer(ByVal parent As Container)
	'             Some of the code in this method deals with changing the
	'            * visibility of components to hide and show the contents for the
	'            * selected tab. This is older code that has since been duplicated
	'            * in JTabbedPane.fireStateChanged(), so as to allow visibility
	'            * changes to happen sooner (see the note there). This code remains
	'            * for backward compatibility as there are some cases, such as
	'            * subclasses that don't fireStateChanged() where it may be used.
	'            * Any changes here need to be kept in synch with
	'            * JTabbedPane.fireStateChanged().
	'            

				outerInstance.rolloverTab = -1

				Dim tabPlacement As Integer = outerInstance.tabPane.tabPlacement
				Dim insets As Insets = outerInstance.tabPane.insets
				Dim selectedIndex As Integer = outerInstance.tabPane.selectedIndex
				Dim visibleComponent As Component = outerInstance.visibleComponent

				calculateLayoutInfo()

				Dim selectedComponent As Component = Nothing
				If selectedIndex < 0 Then
					If visibleComponent IsNot Nothing Then outerInstance.visibleComponent = Nothing
				Else
					selectedComponent = outerInstance.tabPane.getComponentAt(selectedIndex)
				End If
				Dim cx, cy, cw, ch As Integer
				Dim totalTabWidth As Integer = 0
				Dim totalTabHeight As Integer = 0
				Dim contentInsets As Insets = outerInstance.getContentBorderInsets(tabPlacement)

				Dim shouldChangeFocus As Boolean = False

				' In order to allow programs to use a single component
				' as the display for multiple tabs, we will not change
				' the visible compnent if the currently selected tab
				' has a null component.  This is a bit dicey, as we don't
				' explicitly state we support this in the spec, but since
				' programs are now depending on this, we're making it work.
				'
				If selectedComponent IsNot Nothing Then
					If selectedComponent IsNot visibleComponent AndAlso visibleComponent IsNot Nothing Then
						If SwingUtilities.findFocusOwner(visibleComponent) IsNot Nothing Then shouldChangeFocus = True
					End If
					outerInstance.visibleComponent = selectedComponent
				End If

				Dim bounds As Rectangle = outerInstance.tabPane.bounds
				Dim numChildren As Integer = outerInstance.tabPane.componentCount

				If numChildren > 0 Then

					Select Case tabPlacement
						Case LEFT
							totalTabWidth = outerInstance.calculateTabAreaWidth(tabPlacement, outerInstance.runCount, outerInstance.maxTabWidth)
							cx = insets.left + totalTabWidth + contentInsets.left
							cy = insets.top + contentInsets.top
						Case RIGHT
							totalTabWidth = outerInstance.calculateTabAreaWidth(tabPlacement, outerInstance.runCount, outerInstance.maxTabWidth)
							cx = insets.left + contentInsets.left
							cy = insets.top + contentInsets.top
						Case BOTTOM
							totalTabHeight = outerInstance.calculateTabAreaHeight(tabPlacement, outerInstance.runCount, outerInstance.maxTabHeight)
							cx = insets.left + contentInsets.left
							cy = insets.top + contentInsets.top
						Case Else
							totalTabHeight = outerInstance.calculateTabAreaHeight(tabPlacement, outerInstance.runCount, outerInstance.maxTabHeight)
							cx = insets.left + contentInsets.left
							cy = insets.top + totalTabHeight + contentInsets.top
					End Select

					cw = bounds.width - totalTabWidth - insets.left - insets.right - contentInsets.left - contentInsets.right
					ch = bounds.height - totalTabHeight - insets.top - insets.bottom - contentInsets.top - contentInsets.bottom

					For i As Integer = 0 To numChildren - 1
						Dim child As Component = outerInstance.tabPane.getComponent(i)
						If child Is outerInstance.tabContainer Then

							Dim tabContainerWidth As Integer = If(totalTabWidth = 0, bounds.width, totalTabWidth + insets.left + insets.right + contentInsets.left + contentInsets.right)
							Dim tabContainerHeight As Integer = If(totalTabHeight = 0, bounds.height, totalTabHeight + insets.top + insets.bottom + contentInsets.top + contentInsets.bottom)

							Dim tabContainerX As Integer = 0
							Dim tabContainerY As Integer = 0
							If tabPlacement = BOTTOM Then
								tabContainerY = bounds.height - tabContainerHeight
							ElseIf tabPlacement = RIGHT Then
								tabContainerX = bounds.width - tabContainerWidth
							End If
							child.boundsnds(tabContainerX, tabContainerY, tabContainerWidth, tabContainerHeight)
						Else
							child.boundsnds(cx, cy, cw, ch)
						End If
					Next i
				End If
				layoutTabComponents()
				If shouldChangeFocus Then
					If Not outerInstance.requestFocusForVisibleComponent() Then outerInstance.tabPane.requestFocus()
				End If
			End Sub

			Public Overridable Sub calculateLayoutInfo()
				Dim tabCount As Integer = outerInstance.tabPane.tabCount
				outerInstance.assureRectsCreated(tabCount)
				calculateTabRects(outerInstance.tabPane.tabPlacement, tabCount)
				outerInstance.isRunsDirty = False
			End Sub

			Private Sub layoutTabComponents()
				If outerInstance.tabContainer Is Nothing Then Return
				Dim rect As New Rectangle
				Dim delta As New Point(-outerInstance.tabContainer.x, -outerInstance.tabContainer.y)
				If outerInstance.scrollableTabLayoutEnabled() Then outerInstance.translatePointToTabPanel(0, 0, delta)
				For i As Integer = 0 To outerInstance.tabPane.tabCount - 1
					Dim c As Component = outerInstance.tabPane.getTabComponentAt(i)
					If c Is Nothing Then Continue For
					outerInstance.getTabBounds(i, rect)
					Dim preferredSize As Dimension = c.preferredSize
					Dim insets As Insets = outerInstance.getTabInsets(outerInstance.tabPane.tabPlacement, i)
					Dim outerX As Integer = rect.x + insets.left + delta.x
					Dim outerY As Integer = rect.y + insets.top + delta.y
					Dim outerWidth As Integer = rect.width - insets.left - insets.right
					Dim outerHeight As Integer = rect.height - insets.top - insets.bottom
					'centralize component
					Dim x As Integer = outerX + (outerWidth - preferredSize.width) / 2
					Dim y As Integer = outerY + (outerHeight - preferredSize.height) / 2
					Dim tabPlacement As Integer = outerInstance.tabPane.tabPlacement
					Dim isSeleceted As Boolean = i = outerInstance.tabPane.selectedIndex
					c.boundsnds(x + outerInstance.getTabLabelShiftX(tabPlacement, i, isSeleceted), y + outerInstance.getTabLabelShiftY(tabPlacement, i, isSeleceted), preferredSize.width, preferredSize.height)
				Next i
			End Sub

			Protected Friend Overridable Sub calculateTabRects(ByVal tabPlacement As Integer, ByVal tabCount As Integer)
				Dim metrics As FontMetrics = outerInstance.fontMetrics
				Dim size As Dimension = outerInstance.tabPane.size
				Dim insets As Insets = outerInstance.tabPane.insets
				Dim tabAreaInsets As Insets = outerInstance.getTabAreaInsets(tabPlacement)
				Dim fontHeight As Integer = metrics.height
				Dim selectedIndex As Integer = outerInstance.tabPane.selectedIndex
				Dim tabRunOverlay As Integer
				Dim i, j As Integer
				Dim x, y As Integer
				Dim returnAt As Integer
				Dim verticalTabRuns As Boolean = (tabPlacement = LEFT OrElse tabPlacement = RIGHT)
				Dim leftToRight As Boolean = BasicGraphicsUtils.isLeftToRight(outerInstance.tabPane)

				'
				' Calculate bounds within which a tab run must fit
				'
				Select Case tabPlacement
				  Case LEFT
					  outerInstance.maxTabWidth = outerInstance.calculateMaxTabWidth(tabPlacement)
					  x = insets.left + tabAreaInsets.left
					  y = insets.top + tabAreaInsets.top
					  returnAt = size.height - (insets.bottom + tabAreaInsets.bottom)
				  Case RIGHT
					  outerInstance.maxTabWidth = outerInstance.calculateMaxTabWidth(tabPlacement)
					  x = size.width - insets.right - tabAreaInsets.right - outerInstance.maxTabWidth
					  y = insets.top + tabAreaInsets.top
					  returnAt = size.height - (insets.bottom + tabAreaInsets.bottom)
				  Case BOTTOM
					  outerInstance.maxTabHeight = outerInstance.calculateMaxTabHeight(tabPlacement)
					  x = insets.left + tabAreaInsets.left
					  y = size.height - insets.bottom - tabAreaInsets.bottom - outerInstance.maxTabHeight
					  returnAt = size.width - (insets.right + tabAreaInsets.right)
				  Case Else
					  outerInstance.maxTabHeight = outerInstance.calculateMaxTabHeight(tabPlacement)
					  x = insets.left + tabAreaInsets.left
					  y = insets.top + tabAreaInsets.top
					  returnAt = size.width - (insets.right + tabAreaInsets.right)
				End Select

				tabRunOverlay = outerInstance.getTabRunOverlay(tabPlacement)

				outerInstance.runCount = 0
				outerInstance.selectedRun = -1

				If tabCount = 0 Then Return

				' Run through tabs and partition them into runs
				Dim rect As Rectangle
				For i = 0 To tabCount - 1
					rect = outerInstance.rects(i)

					If Not verticalTabRuns Then
						' Tabs on TOP or BOTTOM....
						If i > 0 Then
							rect.x = outerInstance.rects(i-1).x + outerInstance.rects(i-1).width
						Else
							outerInstance.tabRuns(0) = 0
							outerInstance.runCount = 1
							outerInstance.maxTabWidth = 0
							rect.x = x
						End If
						rect.width = outerInstance.calculateTabWidth(tabPlacement, i, metrics)
						outerInstance.maxTabWidth = Math.Max(outerInstance.maxTabWidth, rect.width)

						' Never move a TAB down a run if it is in the first column.
						' Even if there isn't enough room, moving it to a fresh
						' line won't help.
						If rect.x <> x AndAlso rect.x + rect.width > returnAt Then
							If outerInstance.runCount > outerInstance.tabRuns.Length - 1 Then outerInstance.expandTabRunsArray()
							outerInstance.tabRuns(outerInstance.runCount) = i
							outerInstance.runCount += 1
							rect.x = x
						End If
						' Initialize y position in case there's just one run
						rect.y = y
						rect.height = outerInstance.maxTabHeight ' - 2

					Else
						' Tabs on LEFT or RIGHT...
						If i > 0 Then
							rect.y = outerInstance.rects(i-1).y + outerInstance.rects(i-1).height
						Else
							outerInstance.tabRuns(0) = 0
							outerInstance.runCount = 1
							outerInstance.maxTabHeight = 0
							rect.y = y
						End If
						rect.height = outerInstance.calculateTabHeight(tabPlacement, i, fontHeight)
						outerInstance.maxTabHeight = Math.Max(outerInstance.maxTabHeight, rect.height)

						' Never move a TAB over a run if it is in the first run.
						' Even if there isn't enough room, moving it to a fresh
						' column won't help.
						If rect.y <> y AndAlso rect.y + rect.height > returnAt Then
							If outerInstance.runCount > outerInstance.tabRuns.Length - 1 Then outerInstance.expandTabRunsArray()
							outerInstance.tabRuns(outerInstance.runCount) = i
							outerInstance.runCount += 1
							rect.y = y
						End If
						' Initialize x position in case there's just one column
						rect.x = x
						rect.width = outerInstance.maxTabWidth ' - 2

					End If
					If i = selectedIndex Then outerInstance.selectedRun = outerInstance.runCount - 1
				Next i

				If outerInstance.runCount > 1 Then
					' Re-distribute tabs in case last run has leftover space
					normalizeTabRuns(tabPlacement, tabCount,If(verticalTabRuns, y, x), returnAt)

					outerInstance.selectedRun = outerInstance.getRunForTab(tabCount, selectedIndex)

					' Rotate run array so that selected run is first
					If outerInstance.shouldRotateTabRuns(tabPlacement) Then rotateTabRuns(tabPlacement, outerInstance.selectedRun)
				End If

				' Step through runs from back to front to calculate
				' tab y locations and to pad runs appropriately
				For i = outerInstance.runCount - 1 To 0 Step -1
					Dim start As Integer = outerInstance.tabRuns(i)
					Dim [next] As Integer = outerInstance.tabRuns(If(i = (outerInstance.runCount - 1), 0, i + 1))
					Dim [end] As Integer = (If([next] <> 0, [next] - 1, tabCount - 1))
					If Not verticalTabRuns Then
						For j = start To [end]
							rect = outerInstance.rects(j)
							rect.y = y
							rect.x += outerInstance.getTabRunIndent(tabPlacement, i)
						Next j
						If outerInstance.shouldPadTabRun(tabPlacement, i) Then padTabRun(tabPlacement, start, [end], returnAt)
						If tabPlacement = BOTTOM Then
							y -= (outerInstance.maxTabHeight - tabRunOverlay)
						Else
							y += (outerInstance.maxTabHeight - tabRunOverlay)
						End If
					Else
						For j = start To [end]
							rect = outerInstance.rects(j)
							rect.x = x
							rect.y += outerInstance.getTabRunIndent(tabPlacement, i)
						Next j
						If outerInstance.shouldPadTabRun(tabPlacement, i) Then padTabRun(tabPlacement, start, [end], returnAt)
						If tabPlacement = RIGHT Then
							x -= (outerInstance.maxTabWidth - tabRunOverlay)
						Else
							x += (outerInstance.maxTabWidth - tabRunOverlay)
						End If
					End If
				Next i

				' Pad the selected tab so that it appears raised in front
				padSelectedTab(tabPlacement, selectedIndex)

				' if right to left and tab placement on the top or
				' the bottom, flip x positions and adjust by widths
				If (Not leftToRight) AndAlso (Not verticalTabRuns) Then
					Dim rightMargin As Integer = size.width - (insets.right + tabAreaInsets.right)
					For i = 0 To tabCount - 1
						outerInstance.rects(i).x = rightMargin - outerInstance.rects(i).x - outerInstance.rects(i).width
					Next i
				End If
			End Sub


	'       
	'       * Rotates the run-index array so that the selected run is run[0]
	'       
			Protected Friend Overridable Sub rotateTabRuns(ByVal tabPlacement As Integer, ByVal selectedRun As Integer)
				For i As Integer = 0 To selectedRun - 1
					Dim save As Integer = outerInstance.tabRuns(0)
					For j As Integer = 1 To outerInstance.runCount - 1
						outerInstance.tabRuns(j - 1) = outerInstance.tabRuns(j)
					Next j
					outerInstance.tabRuns(outerInstance.runCount-1) = save
				Next i
			End Sub

			Protected Friend Overridable Sub normalizeTabRuns(ByVal tabPlacement As Integer, ByVal tabCount As Integer, ByVal start As Integer, ByVal max As Integer)
				Dim verticalTabRuns As Boolean = (tabPlacement = LEFT OrElse tabPlacement = RIGHT)
				Dim run As Integer = outerInstance.runCount - 1
				Dim keepAdjusting As Boolean = True
				Dim weight As Double = 1.25

				' At this point the tab runs are packed to fit as many
				' tabs as possible, which can leave the last run with a lot
				' of extra space (resulting in very fat tabs on the last run).
				' So we'll attempt to distribute this extra space more evenly
				' across the runs in order to make the runs look more consistent.
				'
				' Starting with the last run, determine whether the last tab in
				' the previous run would fit (generously) in this run; if so,
				' move tab to current run and shift tabs accordingly.  Cycle
				' through remaining runs using the same algorithm.
				'
				Do While keepAdjusting
					Dim last As Integer = outerInstance.lastTabInRun(tabCount, run)
					Dim prevLast As Integer = outerInstance.lastTabInRun(tabCount, run-1)
					Dim [end] As Integer
					Dim prevLastLen As Integer

					If Not verticalTabRuns Then
						[end] = outerInstance.rects(last).x + outerInstance.rects(last).width
						prevLastLen = CInt(Fix(outerInstance.maxTabWidth*weight))
					Else
						[end] = outerInstance.rects(last).y + outerInstance.rects(last).height
						prevLastLen = CInt(Fix(outerInstance.maxTabHeight*weight*2))
					End If

					' Check if the run has enough extra space to fit the last tab
					' from the previous row...
					If max - [end] > prevLastLen Then

						' Insert tab from previous row and shift rest over
						outerInstance.tabRuns(run) = prevLast
						If Not verticalTabRuns Then
							outerInstance.rects(prevLast).x = start
						Else
							outerInstance.rects(prevLast).y = start
						End If
						For i As Integer = prevLast+1 To last
							If Not verticalTabRuns Then
								outerInstance.rects(i).x = outerInstance.rects(i-1).x + outerInstance.rects(i-1).width
							Else
								outerInstance.rects(i).y = outerInstance.rects(i-1).y + outerInstance.rects(i-1).height
							End If
						Next i

					ElseIf run = outerInstance.runCount - 1 Then
						' no more room left in last run, so we're done!
						keepAdjusting = False
					End If
					If run - 1 > 0 Then
						' check previous run next...
						run -= 1
					Else
						' check last run again...but require a higher ratio
						' of extraspace-to-tabsize because we don't want to
						' end up with too many tabs on the last run!
						run = outerInstance.runCount - 1
						weight +=.25
					End If
				Loop
			End Sub

			Protected Friend Overridable Sub padTabRun(ByVal tabPlacement As Integer, ByVal start As Integer, ByVal [end] As Integer, ByVal max As Integer)
				Dim lastRect As Rectangle = outerInstance.rects([end])
				If tabPlacement = TOP OrElse tabPlacement = BOTTOM Then
					Dim runWidth As Integer = (lastRect.x + lastRect.width) - outerInstance.rects(start).x
					Dim deltaWidth As Integer = max - (lastRect.x + lastRect.width)
					Dim factor As Single = CSng(deltaWidth) / CSng(runWidth)

					For j As Integer = start To [end]
						Dim pastRect As Rectangle = outerInstance.rects(j)
						If j > start Then pastRect.x = outerInstance.rects(j-1).x + outerInstance.rects(j-1).width
						pastRect.width += Math.Round(CSng(pastRect.width) * factor)
					Next j
					lastRect.width = max - lastRect.x
				Else
					Dim runHeight As Integer = (lastRect.y + lastRect.height) - outerInstance.rects(start).y
					Dim deltaHeight As Integer = max - (lastRect.y + lastRect.height)
					Dim factor As Single = CSng(deltaHeight) / CSng(runHeight)

					For j As Integer = start To [end]
						Dim pastRect As Rectangle = outerInstance.rects(j)
						If j > start Then pastRect.y = outerInstance.rects(j-1).y + outerInstance.rects(j-1).height
						pastRect.height += Math.Round(CSng(pastRect.height) * factor)
					Next j
					lastRect.height = max - lastRect.y
				End If
			End Sub

			Protected Friend Overridable Sub padSelectedTab(ByVal tabPlacement As Integer, ByVal selectedIndex As Integer)

				If selectedIndex >= 0 Then
					Dim selRect As Rectangle = outerInstance.rects(selectedIndex)
					Dim padInsets As Insets = outerInstance.getSelectedTabPadInsets(tabPlacement)
					selRect.x -= padInsets.left
					selRect.width += (padInsets.left + padInsets.right)
					selRect.y -= padInsets.top
					selRect.height += (padInsets.top + padInsets.bottom)

					If Not outerInstance.scrollableTabLayoutEnabled() Then ' WRAP_TAB_LAYOUT
						' do not expand selected tab more then necessary
						Dim size As Dimension = outerInstance.tabPane.size
						Dim insets As Insets = outerInstance.tabPane.insets

						If (tabPlacement = LEFT) OrElse (tabPlacement = RIGHT) Then
							Dim top As Integer = insets.top - selRect.y
							If top > 0 Then
								selRect.y += top
								selRect.height -= top
							End If
							Dim bottom As Integer = (selRect.y + selRect.height) + insets.bottom - size.height
							If bottom > 0 Then selRect.height -= bottom
						Else
							Dim left As Integer = insets.left - selRect.x
							If left > 0 Then
								selRect.x += left
								selRect.width -= left
							End If
							Dim right As Integer = (selRect.x + selRect.width) + insets.right - size.width
							If right > 0 Then selRect.width -= right
						End If
					End If
				End If
			End Sub
		End Class

		Private Class TabbedPaneScrollLayout
			Inherits TabbedPaneLayout

			Private ReadOnly outerInstance As BasicTabbedPaneUI

			Public Sub New(ByVal outerInstance As BasicTabbedPaneUI)
				Me.outerInstance = outerInstance
			End Sub


			Protected Friend Overrides Function preferredTabAreaHeight(ByVal tabPlacement As Integer, ByVal width As Integer) As Integer
				Return outerInstance.calculateMaxTabHeight(tabPlacement)
			End Function

			Protected Friend Overrides Function preferredTabAreaWidth(ByVal tabPlacement As Integer, ByVal height As Integer) As Integer
				Return outerInstance.calculateMaxTabWidth(tabPlacement)
			End Function

			Public Overrides Sub layoutContainer(ByVal parent As Container)
	'             Some of the code in this method deals with changing the
	'             * visibility of components to hide and show the contents for the
	'             * selected tab. This is older code that has since been duplicated
	'             * in JTabbedPane.fireStateChanged(), so as to allow visibility
	'             * changes to happen sooner (see the note there). This code remains
	'             * for backward compatibility as there are some cases, such as
	'             * subclasses that don't fireStateChanged() where it may be used.
	'             * Any changes here need to be kept in synch with
	'             * JTabbedPane.fireStateChanged().
	'             

				outerInstance.rolloverTab = -1

				Dim tabPlacement As Integer = outerInstance.tabPane.tabPlacement
				Dim tabCount As Integer = outerInstance.tabPane.tabCount
				Dim insets As Insets = outerInstance.tabPane.insets
				Dim selectedIndex As Integer = outerInstance.tabPane.selectedIndex
				Dim visibleComponent As Component = outerInstance.visibleComponent

				calculateLayoutInfo()

				Dim selectedComponent As Component = Nothing
				If selectedIndex < 0 Then
					If visibleComponent IsNot Nothing Then outerInstance.visibleComponent = Nothing
				Else
					selectedComponent = outerInstance.tabPane.getComponentAt(selectedIndex)
				End If

				If outerInstance.tabPane.tabCount = 0 Then
					outerInstance.tabScroller.croppedEdge.resetParams()
					outerInstance.tabScroller.scrollForwardButton.visible = False
					outerInstance.tabScroller.scrollBackwardButton.visible = False
					Return
				End If

				Dim shouldChangeFocus As Boolean = False

				' In order to allow programs to use a single component
				' as the display for multiple tabs, we will not change
				' the visible compnent if the currently selected tab
				' has a null component.  This is a bit dicey, as we don't
				' explicitly state we support this in the spec, but since
				' programs are now depending on this, we're making it work.
				'
				If selectedComponent IsNot Nothing Then
					If selectedComponent IsNot visibleComponent AndAlso visibleComponent IsNot Nothing Then
						If SwingUtilities.findFocusOwner(visibleComponent) IsNot Nothing Then shouldChangeFocus = True
					End If
					outerInstance.visibleComponent = selectedComponent
				End If
				Dim tx, ty, tw, th As Integer ' tab area bounds
				Dim cx, cy, cw, ch As Integer ' content area bounds
				Dim contentInsets As Insets = outerInstance.getContentBorderInsets(tabPlacement)
				Dim bounds As Rectangle = outerInstance.tabPane.bounds
				Dim numChildren As Integer = outerInstance.tabPane.componentCount

				If numChildren > 0 Then
					Select Case tabPlacement
						Case LEFT
							' calculate tab area bounds
							tw = outerInstance.calculateTabAreaWidth(tabPlacement, outerInstance.runCount, outerInstance.maxTabWidth)
							th = bounds.height - insets.top - insets.bottom
							tx = insets.left
							ty = insets.top

							' calculate content area bounds
							cx = tx + tw + contentInsets.left
							cy = ty + contentInsets.top
							cw = bounds.width - insets.left - insets.right - tw - contentInsets.left - contentInsets.right
							ch = bounds.height - insets.top - insets.bottom - contentInsets.top - contentInsets.bottom
						Case RIGHT
							' calculate tab area bounds
							tw = outerInstance.calculateTabAreaWidth(tabPlacement, outerInstance.runCount, outerInstance.maxTabWidth)
							th = bounds.height - insets.top - insets.bottom
							tx = bounds.width - insets.right - tw
							ty = insets.top

							' calculate content area bounds
							cx = insets.left + contentInsets.left
							cy = insets.top + contentInsets.top
							cw = bounds.width - insets.left - insets.right - tw - contentInsets.left - contentInsets.right
							ch = bounds.height - insets.top - insets.bottom - contentInsets.top - contentInsets.bottom
						Case BOTTOM
							' calculate tab area bounds
							tw = bounds.width - insets.left - insets.right
							th = outerInstance.calculateTabAreaHeight(tabPlacement, outerInstance.runCount, outerInstance.maxTabHeight)
							tx = insets.left
							ty = bounds.height - insets.bottom - th

							' calculate content area bounds
							cx = insets.left + contentInsets.left
							cy = insets.top + contentInsets.top
							cw = bounds.width - insets.left - insets.right - contentInsets.left - contentInsets.right
							ch = bounds.height - insets.top - insets.bottom - th - contentInsets.top - contentInsets.bottom
						Case Else
							' calculate tab area bounds
							tw = bounds.width - insets.left - insets.right
							th = outerInstance.calculateTabAreaHeight(tabPlacement, outerInstance.runCount, outerInstance.maxTabHeight)
							tx = insets.left
							ty = insets.top

							' calculate content area bounds
							cx = tx + contentInsets.left
							cy = ty + th + contentInsets.top
							cw = bounds.width - insets.left - insets.right - contentInsets.left - contentInsets.right
							ch = bounds.height - insets.top - insets.bottom - th - contentInsets.top - contentInsets.bottom
					End Select

					For i As Integer = 0 To numChildren - 1
						Dim child As Component = outerInstance.tabPane.getComponent(i)

						If outerInstance.tabScroller IsNot Nothing AndAlso child Is outerInstance.tabScroller.viewport Then
							Dim viewport As JViewport = CType(child, JViewport)
							Dim viewRect As Rectangle = viewport.viewRect
							Dim vw As Integer = tw
							Dim vh As Integer = th
							Dim butSize As Dimension = outerInstance.tabScroller.scrollForwardButton.preferredSize
							Select Case tabPlacement
								Case LEFT, RIGHT
									Dim totalTabHeight As Integer = outerInstance.rects(tabCount - 1).y + outerInstance.rects(tabCount - 1).height
									If totalTabHeight > th Then
										' Allow space for scrollbuttons
										vh = If(th > 2 * butSize.height, th - 2 * butSize.height, 0)
										If totalTabHeight - viewRect.y <= vh Then vh = totalTabHeight - viewRect.y
									End If
								Case Else
									Dim totalTabWidth As Integer = outerInstance.rects(tabCount - 1).x + outerInstance.rects(tabCount - 1).width
									If totalTabWidth > tw Then
										' Need to allow space for scrollbuttons
										vw = If(tw > 2 * butSize.width, tw - 2 * butSize.width, 0)
										If totalTabWidth - viewRect.x <= vw Then vw = totalTabWidth - viewRect.x
									End If
							End Select
							child.boundsnds(tx, ty, vw, vh)

						ElseIf outerInstance.tabScroller IsNot Nothing AndAlso (child Is outerInstance.tabScroller.scrollForwardButton OrElse child Is outerInstance.tabScroller.scrollBackwardButton) Then
							Dim scrollbutton As Component = child
							Dim bsize As Dimension = scrollbutton.preferredSize
							Dim bx As Integer = 0
							Dim by As Integer = 0
							Dim bw As Integer = bsize.width
							Dim bh As Integer = bsize.height
							Dim visible As Boolean = False

							Select Case tabPlacement
								Case LEFT, RIGHT
									Dim totalTabHeight As Integer = outerInstance.rects(tabCount - 1).y + outerInstance.rects(tabCount - 1).height
									If totalTabHeight > th Then
										visible = True
										bx = (If(tabPlacement = LEFT, tx + tw - bsize.width, tx))
										by = If(child Is outerInstance.tabScroller.scrollForwardButton, bounds.height - insets.bottom - bsize.height, bounds.height - insets.bottom - 2 * bsize.height)
									End If

								Case Else
									Dim totalTabWidth As Integer = outerInstance.rects(tabCount - 1).x + outerInstance.rects(tabCount - 1).width

									If totalTabWidth > tw Then
										visible = True
										bx = If(child Is outerInstance.tabScroller.scrollForwardButton, bounds.width - insets.left - bsize.width, bounds.width - insets.left - 2 * bsize.width)
										by = (If(tabPlacement = TOP, ty + th - bsize.height, ty))
									End If
							End Select
							child.visible = visible
							If visible Then child.boundsnds(bx, by, bw, bh)

						Else
							' All content children...
							child.boundsnds(cx, cy, cw, ch)
						End If
					Next i
					MyBase.layoutTabComponents()
					layoutCroppedEdge()
					If shouldChangeFocus Then
						If Not outerInstance.requestFocusForVisibleComponent() Then outerInstance.tabPane.requestFocus()
					End If
				End If
			End Sub

			Private Sub layoutCroppedEdge()
				outerInstance.tabScroller.croppedEdge.resetParams()
				Dim viewRect As Rectangle = outerInstance.tabScroller.viewport.viewRect
				Dim cropline As Integer
				For i As Integer = 0 To outerInstance.rects.Length - 1
					Dim tabRect As Rectangle = outerInstance.rects(i)
					Select Case outerInstance.tabPane.tabPlacement
						Case LEFT, RIGHT
							cropline = viewRect.y + viewRect.height
							If (tabRect.y < cropline) AndAlso (tabRect.y + tabRect.height > cropline) Then outerInstance.tabScroller.croppedEdge.paramsams(i, cropline - tabRect.y - 1, -outerInstance.currentTabAreaInsets.left, 0)
						Case Else
							cropline = viewRect.x + viewRect.width
							If (tabRect.x < cropline - 1) AndAlso (tabRect.x + tabRect.width > cropline) Then outerInstance.tabScroller.croppedEdge.paramsams(i, cropline - tabRect.x - 1, 0, -outerInstance.currentTabAreaInsets.top)
					End Select
				Next i
			End Sub

			Protected Friend Overrides Sub calculateTabRects(ByVal tabPlacement As Integer, ByVal tabCount As Integer)
				Dim metrics As FontMetrics = outerInstance.fontMetrics
				Dim size As Dimension = outerInstance.tabPane.size
				Dim insets As Insets = outerInstance.tabPane.insets
				Dim tabAreaInsets As Insets = outerInstance.getTabAreaInsets(tabPlacement)
				Dim fontHeight As Integer = metrics.height
				Dim selectedIndex As Integer = outerInstance.tabPane.selectedIndex
				Dim i As Integer
				Dim verticalTabRuns As Boolean = (tabPlacement = LEFT OrElse tabPlacement = RIGHT)
				Dim leftToRight As Boolean = BasicGraphicsUtils.isLeftToRight(outerInstance.tabPane)
				Dim x As Integer = tabAreaInsets.left
				Dim y As Integer = tabAreaInsets.top
				Dim totalWidth As Integer = 0
				Dim totalHeight As Integer = 0

				'
				' Calculate bounds within which a tab run must fit
				'
				Select Case tabPlacement
				  Case LEFT, RIGHT
					  outerInstance.maxTabWidth = outerInstance.calculateMaxTabWidth(tabPlacement)
				  Case Else
					  outerInstance.maxTabHeight = outerInstance.calculateMaxTabHeight(tabPlacement)
				End Select

				outerInstance.runCount = 0
				outerInstance.selectedRun = -1

				If tabCount = 0 Then Return

				outerInstance.selectedRun = 0
				outerInstance.runCount = 1

				' Run through tabs and lay them out in a single run
				Dim rect As Rectangle
				For i = 0 To tabCount - 1
					rect = outerInstance.rects(i)

					If Not verticalTabRuns Then
						' Tabs on TOP or BOTTOM....
						If i > 0 Then
							rect.x = outerInstance.rects(i-1).x + outerInstance.rects(i-1).width
						Else
							outerInstance.tabRuns(0) = 0
							outerInstance.maxTabWidth = 0
							totalHeight += outerInstance.maxTabHeight
							rect.x = x
						End If
						rect.width = outerInstance.calculateTabWidth(tabPlacement, i, metrics)
						totalWidth = rect.x + rect.width
						outerInstance.maxTabWidth = Math.Max(outerInstance.maxTabWidth, rect.width)

						rect.y = y
						rect.height = outerInstance.maxTabHeight ' - 2

					Else
						' Tabs on LEFT or RIGHT...
						If i > 0 Then
							rect.y = outerInstance.rects(i-1).y + outerInstance.rects(i-1).height
						Else
							outerInstance.tabRuns(0) = 0
							outerInstance.maxTabHeight = 0
							totalWidth = outerInstance.maxTabWidth
							rect.y = y
						End If
						rect.height = outerInstance.calculateTabHeight(tabPlacement, i, fontHeight)
						totalHeight = rect.y + rect.height
						outerInstance.maxTabHeight = Math.Max(outerInstance.maxTabHeight, rect.height)

						rect.x = x
						rect.width = outerInstance.maxTabWidth ' - 2

					End If
				Next i

				If outerInstance.tabsOverlapBorder Then padSelectedTab(tabPlacement, selectedIndex)

				' if right to left and tab placement on the top or
				' the bottom, flip x positions and adjust by widths
				If (Not leftToRight) AndAlso (Not verticalTabRuns) Then
					Dim rightMargin As Integer = size.width - (insets.right + tabAreaInsets.right)
					For i = 0 To tabCount - 1
						outerInstance.rects(i).x = rightMargin - outerInstance.rects(i).x - outerInstance.rects(i).width
					Next i
				End If
				outerInstance.tabScroller.tabPanel.preferredSize = New Dimension(totalWidth, totalHeight)
				outerInstance.tabScroller.tabPanel.invalidate()
			End Sub
		End Class

		Private Class ScrollableTabSupport
			Implements ActionListener, ChangeListener

			Private ReadOnly outerInstance As BasicTabbedPaneUI

			Public viewport As ScrollableTabViewport
			Public tabPanel As ScrollableTabPanel
			Public scrollForwardButton As JButton
			Public scrollBackwardButton As JButton
			Public croppedEdge As CroppedEdge
			Public leadingTabIndex As Integer

			Private tabViewPosition As New Point(0,0)

			Friend Sub New(ByVal outerInstance As BasicTabbedPaneUI, ByVal tabPlacement As Integer)
					Me.outerInstance = outerInstance
				viewport = New ScrollableTabViewport
				tabPanel = New ScrollableTabPanel
				viewport.view = tabPanel
				viewport.addChangeListener(Me)
				croppedEdge = New CroppedEdge
				createButtons()
			End Sub

			''' <summary>
			''' Recreates the scroll buttons and adds them to the TabbedPane.
			''' </summary>
			Friend Overridable Sub createButtons()
				If scrollForwardButton IsNot Nothing Then
					outerInstance.tabPane.remove(scrollForwardButton)
					scrollForwardButton.removeActionListener(Me)
					outerInstance.tabPane.remove(scrollBackwardButton)
					scrollBackwardButton.removeActionListener(Me)
				End If
				Dim tabPlacement As Integer = outerInstance.tabPane.tabPlacement
				If tabPlacement = TOP OrElse tabPlacement = BOTTOM Then
					scrollForwardButton = outerInstance.createScrollButton(EAST)
					scrollBackwardButton = outerInstance.createScrollButton(WEST)
 ' tabPlacement = LEFT || RIGHT
				Else
					scrollForwardButton = outerInstance.createScrollButton(SOUTH)
					scrollBackwardButton = outerInstance.createScrollButton(NORTH)
				End If
				scrollForwardButton.addActionListener(Me)
				scrollBackwardButton.addActionListener(Me)
				outerInstance.tabPane.add(scrollForwardButton)
				outerInstance.tabPane.add(scrollBackwardButton)
			End Sub

			Public Overridable Sub scrollForward(ByVal tabPlacement As Integer)
				Dim viewSize As Dimension = viewport.viewSize
				Dim viewRect As Rectangle = viewport.viewRect

				If tabPlacement = TOP OrElse tabPlacement = BOTTOM Then
					If viewRect.width >= viewSize.width - viewRect.x Then
						Return ' no room left to scroll
					End If ' tabPlacement == LEFT || tabPlacement == RIGHT
				Else
					If viewRect.height >= viewSize.height - viewRect.y Then Return
				End If
				leadingTabIndexdex(tabPlacement, leadingTabIndex+1)
			End Sub

			Public Overridable Sub scrollBackward(ByVal tabPlacement As Integer)
				If leadingTabIndex = 0 Then Return ' no room left to scroll
				leadingTabIndexdex(tabPlacement, leadingTabIndex-1)
			End Sub

			Public Overridable Sub setLeadingTabIndex(ByVal tabPlacement As Integer, ByVal index As Integer)
				leadingTabIndex = index
				Dim viewSize As Dimension = viewport.viewSize
				Dim viewRect As Rectangle = viewport.viewRect

				Select Case tabPlacement
				  Case TOP, BOTTOM
					tabViewPosition.x = If(leadingTabIndex = 0, 0, outerInstance.rects(leadingTabIndex).x)

					If (viewSize.width - tabViewPosition.x) < viewRect.width Then
						' We've scrolled to the end, so adjust the viewport size
						' to ensure the view position remains aligned on a tab boundary
						Dim extentSize As New Dimension(viewSize.width - tabViewPosition.x, viewRect.height)
						viewport.extentSize = extentSize
					End If
				  Case LEFT, RIGHT
					tabViewPosition.y = If(leadingTabIndex = 0, 0, outerInstance.rects(leadingTabIndex).y)

					If (viewSize.height - tabViewPosition.y) < viewRect.height Then
					' We've scrolled to the end, so adjust the viewport size
					' to ensure the view position remains aligned on a tab boundary
						 Dim extentSize As New Dimension(viewRect.width, viewSize.height - tabViewPosition.y)
						 viewport.extentSize = extentSize
					End If
				End Select
				viewport.viewPosition = tabViewPosition
			End Sub

			Public Overridable Sub stateChanged(ByVal e As ChangeEvent) Implements ChangeListener.stateChanged
				updateView()
			End Sub

			Private Sub updateView()
				Dim tabPlacement As Integer = outerInstance.tabPane.tabPlacement
				Dim tabCount As Integer = outerInstance.tabPane.tabCount
				outerInstance.assureRectsCreated(tabCount)
				Dim vpRect As Rectangle = viewport.bounds
				Dim viewSize As Dimension = viewport.viewSize
				Dim viewRect As Rectangle = viewport.viewRect

				leadingTabIndex = outerInstance.getClosestTab(viewRect.x, viewRect.y)

				' If the tab isn't right aligned, adjust it.
				If leadingTabIndex + 1 < tabCount Then
					Select Case tabPlacement
					Case TOP, BOTTOM
						If outerInstance.rects(leadingTabIndex).x < viewRect.x Then leadingTabIndex += 1
					Case LEFT, RIGHT
						If outerInstance.rects(leadingTabIndex).y < viewRect.y Then leadingTabIndex += 1
					End Select
				End If
				Dim contentInsets As Insets = outerInstance.getContentBorderInsets(tabPlacement)
				Select Case tabPlacement
				  Case LEFT
					  outerInstance.tabPane.repaint(vpRect.x+vpRect.width, vpRect.y, contentInsets.left, vpRect.height)
					  scrollBackwardButton.enabled = viewRect.y > 0 AndAlso leadingTabIndex > 0
					  scrollForwardButton.enabled = leadingTabIndex < tabCount-1 AndAlso viewSize.height-viewRect.y > viewRect.height
				  Case RIGHT
					  outerInstance.tabPane.repaint(vpRect.x-contentInsets.right, vpRect.y, contentInsets.right, vpRect.height)
					  scrollBackwardButton.enabled = viewRect.y > 0 AndAlso leadingTabIndex > 0
					  scrollForwardButton.enabled = leadingTabIndex < tabCount-1 AndAlso viewSize.height-viewRect.y > viewRect.height
				  Case BOTTOM
					  outerInstance.tabPane.repaint(vpRect.x, vpRect.y-contentInsets.bottom, vpRect.width, contentInsets.bottom)
					  scrollBackwardButton.enabled = viewRect.x > 0 AndAlso leadingTabIndex > 0
					  scrollForwardButton.enabled = leadingTabIndex < tabCount-1 AndAlso viewSize.width-viewRect.x > viewRect.width
				  Case Else
					  outerInstance.tabPane.repaint(vpRect.x, vpRect.y+vpRect.height, vpRect.width, contentInsets.top)
					  scrollBackwardButton.enabled = viewRect.x > 0 AndAlso leadingTabIndex > 0
					  scrollForwardButton.enabled = leadingTabIndex < tabCount-1 AndAlso viewSize.width-viewRect.x > viewRect.width
				End Select
			End Sub

			''' <summary>
			''' ActionListener for the scroll buttons.
			''' </summary>
			Public Overridable Sub actionPerformed(ByVal e As ActionEvent)
				Dim map As ActionMap = outerInstance.tabPane.actionMap

				If map IsNot Nothing Then
					Dim actionKey As String

					If e.source Is scrollForwardButton Then
						actionKey = "scrollTabsForwardAction"
					Else
						actionKey = "scrollTabsBackwardAction"
					End If
					Dim action As Action = map.get(actionKey)

					If action IsNot Nothing AndAlso action.enabled Then action.actionPerformed(New ActionEvent(outerInstance.tabPane, ActionEvent.ACTION_PERFORMED, Nothing, e.when, e.modifiers))
				End If
			End Sub

			Public Overrides Function ToString() As String
				Return "viewport.viewSize=" & viewport.viewSize & vbLf & "viewport.viewRectangle=" & viewport.viewRect & vbLf & "leadingTabIndex=" & leadingTabIndex & vbLf & "tabViewPosition=" & tabViewPosition
			End Function

		End Class

		Private Class ScrollableTabViewport
			Inherits JViewport
			Implements UIResource

			Private ReadOnly outerInstance As BasicTabbedPaneUI

			Public Sub New(ByVal outerInstance As BasicTabbedPaneUI)
					Me.outerInstance = outerInstance
				MyBase.New()
				name = "TabbedPane.scrollableViewport"
				scrollMode = SIMPLE_SCROLL_MODE
				opaque = outerInstance.tabPane.opaque
				Dim bgColor As Color = UIManager.getColor("TabbedPane.tabAreaBackground")
				If bgColor Is Nothing Then bgColor = outerInstance.tabPane.background
				background = bgColor
			End Sub
		End Class

		Private Class ScrollableTabPanel
			Inherits JPanel
			Implements UIResource

			Private ReadOnly outerInstance As BasicTabbedPaneUI

			Public Sub New(ByVal outerInstance As BasicTabbedPaneUI)
					Me.outerInstance = outerInstance
				MyBase.New(Nothing)
				opaque = outerInstance.tabPane.opaque
				Dim bgColor As Color = UIManager.getColor("TabbedPane.tabAreaBackground")
				If bgColor Is Nothing Then bgColor = outerInstance.tabPane.background
				background = bgColor
			End Sub
			Public Overrides Sub paintComponent(ByVal g As Graphics)
				MyBase.paintComponent(g)
				outerInstance.paintTabArea(g, outerInstance.tabPane.tabPlacement, outerInstance.tabPane.selectedIndex)
				If outerInstance.tabScroller.croppedEdge.paramsSet AndAlso outerInstance.tabContainer Is Nothing Then
					Dim croppedRect As Rectangle = outerInstance.rects(outerInstance.tabScroller.croppedEdge.tabIndex)
					g.translate(croppedRect.x, croppedRect.y)
					outerInstance.tabScroller.croppedEdge.paintComponent(g)
					g.translate(-croppedRect.x, -croppedRect.y)
				End If
			End Sub

			Public Overridable Sub doLayout()
				If componentCount > 0 Then
					Dim child As Component = getComponent(0)
					child.boundsnds(0, 0, width, height)
				End If
			End Sub
		End Class

		Private Class ScrollableTabButton
			Inherits BasicArrowButton
			Implements UIResource, SwingConstants

			Private ReadOnly outerInstance As BasicTabbedPaneUI

			Public Sub New(ByVal outerInstance As BasicTabbedPaneUI, ByVal direction As Integer)
					Me.outerInstance = outerInstance
				MyBase.New(direction, UIManager.getColor("TabbedPane.selected"), UIManager.getColor("TabbedPane.shadow"), UIManager.getColor("TabbedPane.darkShadow"), UIManager.getColor("TabbedPane.highlight"))
			End Sub
		End Class


	' Controller: event listeners

		Private Class Handler
			Implements ChangeListener, ContainerListener, FocusListener, MouseListener, MouseMotionListener, java.beans.PropertyChangeListener

			Private ReadOnly outerInstance As BasicTabbedPaneUI

			Public Sub New(ByVal outerInstance As BasicTabbedPaneUI)
				Me.outerInstance = outerInstance
			End Sub

			'
			' PropertyChangeListener
			'
			Public Overridable Sub propertyChange(ByVal e As java.beans.PropertyChangeEvent)
				Dim pane As JTabbedPane = CType(e.source, JTabbedPane)
				Dim name As String = e.propertyName
				Dim isScrollLayout As Boolean = outerInstance.scrollableTabLayoutEnabled()
				If name = "mnemonicAt" Then
					outerInstance.updateMnemonics()
					pane.repaint()
				ElseIf name = "displayedMnemonicIndexAt" Then
					pane.repaint()
				ElseIf name ="indexForTitle" Then
					outerInstance.calculatedBaseline = False
					Dim index As Integer? = CInt(Fix(e.newValue))
					' remove the current index
					' to let updateHtmlViews() insert the correct one
					If outerInstance.htmlViews IsNot Nothing Then outerInstance.htmlViews.RemoveAt(index)
					updateHtmlViews(index)
				ElseIf name = "tabLayoutPolicy" Then
					outerInstance.uninstallUI(pane)
					outerInstance.installUI(pane)
					outerInstance.calculatedBaseline = False
				ElseIf name = "tabPlacement" Then
					If outerInstance.scrollableTabLayoutEnabled() Then outerInstance.tabScroller.createButtons()
					outerInstance.calculatedBaseline = False
				ElseIf name = "opaque" AndAlso isScrollLayout Then
					Dim newVal As Boolean = CBool(e.newValue)
					outerInstance.tabScroller.tabPanel.opaque = newVal
					outerInstance.tabScroller.viewport.opaque = newVal
				ElseIf name = "background" AndAlso isScrollLayout Then
					Dim newVal As Color = CType(e.newValue, Color)
					outerInstance.tabScroller.tabPanel.background = newVal
					outerInstance.tabScroller.viewport.background = newVal
					Dim newColor As Color = If(outerInstance.selectedColor Is Nothing, newVal, outerInstance.selectedColor)
					outerInstance.tabScroller.scrollForwardButton.background = newColor
					outerInstance.tabScroller.scrollBackwardButton.background = newColor
				ElseIf name = "indexForTabComponent" Then
					If outerInstance.tabContainer IsNot Nothing Then outerInstance.tabContainer.removeUnusedTabComponents()
					Dim c As Component = outerInstance.tabPane.getTabComponentAt(CInt(Fix(e.newValue)))
					If c IsNot Nothing Then
						If outerInstance.tabContainer Is Nothing Then
							outerInstance.installTabContainer()
						Else
							outerInstance.tabContainer.add(c)
						End If
					End If
					outerInstance.tabPane.revalidate()
					outerInstance.tabPane.repaint()
					outerInstance.calculatedBaseline = False
				ElseIf name = "indexForNullComponent" Then
					outerInstance.isRunsDirty = True
					updateHtmlViews(CInt(Fix(e.newValue)))
				ElseIf name = "font" Then
					outerInstance.calculatedBaseline = False
				End If
			End Sub

			Private Sub updateHtmlViews(ByVal index As Integer)
				Dim title As String = outerInstance.tabPane.getTitleAt(index)
				Dim isHTML As Boolean = BasicHTML.isHTMLString(title)
				If isHTML Then
					If outerInstance.htmlViews Is Nothing Then ' Initialize vector
						outerInstance.htmlViews = outerInstance.createHTMLVector() ' Vector already exists
					Else
						Dim v As javax.swing.text.View = BasicHTML.createHTMLView(outerInstance.tabPane, title)
						outerInstance.htmlViews.Insert(index, v)
					End If ' Not HTML
				Else
					If outerInstance.htmlViews IsNot Nothing Then ' Add placeholder
						outerInstance.htmlViews.Insert(index, Nothing)
					End If ' else nada!
				End If
				outerInstance.updateMnemonics()
			End Sub

			'
			' ChangeListener
			'
			Public Overridable Sub stateChanged(ByVal e As ChangeEvent) Implements ChangeListener.stateChanged
				Dim tabPane As JTabbedPane = CType(e.source, JTabbedPane)
				tabPane.revalidate()
				tabPane.repaint()

				outerInstance.focusIndexdex(tabPane.selectedIndex, False)

				If outerInstance.scrollableTabLayoutEnabled() Then
					outerInstance.ensureCurrentLayout()
					Dim index As Integer = tabPane.selectedIndex
					If index < outerInstance.rects.Length AndAlso index <> -1 Then outerInstance.tabScroller.tabPanel.scrollRectToVisible(CType(outerInstance.rects(index).clone(), Rectangle))
				End If
			End Sub

			'
			' MouseListener
			'
			Public Overridable Sub mouseClicked(ByVal e As MouseEvent)
			End Sub

			Public Overridable Sub mouseReleased(ByVal e As MouseEvent)
			End Sub

			Public Overridable Sub mouseEntered(ByVal e As MouseEvent)
				outerInstance.rolloverTabTab(e.x, e.y)
			End Sub

			Public Overridable Sub mouseExited(ByVal e As MouseEvent)
				outerInstance.rolloverTab = -1
			End Sub

			Public Overridable Sub mousePressed(ByVal e As MouseEvent)
				If Not outerInstance.tabPane.enabled Then Return
				Dim tabIndex As Integer = outerInstance.tabForCoordinate(outerInstance.tabPane, e.x, e.y)
				If tabIndex >= 0 AndAlso outerInstance.tabPane.isEnabledAt(tabIndex) Then
					If tabIndex <> outerInstance.tabPane.selectedIndex Then
						' Clicking on unselected tab, change selection, do NOT
						' request focus.
						' This will trigger the focusIndex to change by way
						' of stateChanged.
						outerInstance.tabPane.selectedIndex = tabIndex
					ElseIf outerInstance.tabPane.requestFocusEnabled Then
						' Clicking on selected tab, try and give the tabbedpane
						' focus.  Repaint will occur in focusGained.
						outerInstance.tabPane.requestFocus()
					End If
				End If
			End Sub

			'
			' MouseMotionListener
			'
			Public Overridable Sub mouseDragged(ByVal e As MouseEvent)
			End Sub

			Public Overridable Sub mouseMoved(ByVal e As MouseEvent)
				outerInstance.rolloverTabTab(e.x, e.y)
			End Sub

			'
			' FocusListener
			'
			Public Overridable Sub focusGained(ByVal e As FocusEvent)
			   outerInstance.focusIndexdex(outerInstance.tabPane.selectedIndex, True)
			End Sub
			Public Overridable Sub focusLost(ByVal e As FocusEvent)
			   outerInstance.repaintTab(outerInstance.focusIndex)
			End Sub


			'
			' ContainerListener
			'
	'     GES 2/3/99:
	'       The container listener code was added to support HTML
	'       rendering of tab titles.
	'
	'       Ideally, we would be able to listen for property changes
	'       when a tab is added or its text modified.  At the moment
	'       there are no such events because the Beans spec doesn't
	'       allow 'indexed' property changes (i.e. tab 2's text changed
	'       from A to B).
	'
	'       In order to get around this, we listen for tabs to be added
	'       or removed by listening for the container events.  we then
	'       queue up a runnable (so the component has a chance to complete
	'       the add) which checks the tab title of the new component to see
	'       if it requires HTML rendering.
	'
	'       The Views (one per tab title requiring HTML rendering) are
	'       stored in the htmlViews Vector, which is only allocated after
	'       the first time we run into an HTML tab.  Note that this vector
	'       is kept in step with the number of pages, and nulls are added
	'       for those pages whose tab title do not require HTML rendering.
	'
	'       This makes it easy for the paint and layout code to tell
	'       whether to invoke the HTML engine without having to check
	'       the string during time-sensitive operations.
	'
	'       When we have added a way to listen for tab additions and
	'       changes to tab text, this code should be removed and
	'       replaced by something which uses that.  

			Public Overridable Sub componentAdded(ByVal e As ContainerEvent)
				Dim tp As JTabbedPane = CType(e.container, JTabbedPane)
				Dim child As Component = e.child
				If TypeOf child Is UIResource Then Return
				outerInstance.isRunsDirty = True
				updateHtmlViews(tp.indexOfComponent(child))
			End Sub
			Public Overridable Sub componentRemoved(ByVal e As ContainerEvent)
				Dim tp As JTabbedPane = CType(e.container, JTabbedPane)
				Dim child As Component = e.child
				If TypeOf child Is UIResource Then Return

				' NOTE 4/15/2002 (joutwate):
				' This fix is implemented using client properties since there is
				' currently no IndexPropertyChangeEvent.  Once
				' IndexPropertyChangeEvents have been added this code should be
				' modified to use it.
				Dim indexObj As Integer? = CInt(Fix(tp.getClientProperty("__index_to_remove__")))
				If indexObj IsNot Nothing Then
					Dim index As Integer = indexObj
					If outerInstance.htmlViews IsNot Nothing AndAlso outerInstance.htmlViews.Count > index Then outerInstance.htmlViews.RemoveAt(index)
					tp.putClientProperty("__index_to_remove__", Nothing)
				End If
				outerInstance.isRunsDirty = True
				outerInstance.updateMnemonics()

				outerInstance.validateFocusIndex()
			End Sub
		End Class

		''' <summary>
		''' This class should be treated as a &quot;protected&quot; inner class.
		''' Instantiate it only within subclasses of BasicTabbedPaneUI.
		''' </summary>
		Public Class PropertyChangeHandler
			Implements java.beans.PropertyChangeListener

			Private ReadOnly outerInstance As BasicTabbedPaneUI

			Public Sub New(ByVal outerInstance As BasicTabbedPaneUI)
				Me.outerInstance = outerInstance
			End Sub

			' NOTE: This class exists only for backward compatibility. All
			' its functionality has been moved into Handler. If you need to add
			' new functionality add it to the Handler, but make sure this
			' class calls into the Handler.
			Public Overridable Sub propertyChange(ByVal e As java.beans.PropertyChangeEvent)
				outerInstance.handler.propertyChange(e)
			End Sub
		End Class

		''' <summary>
		''' This class should be treated as a &quot;protected&quot; inner class.
		''' Instantiate it only within subclasses of BasicTabbedPaneUI.
		''' </summary>
		Public Class TabSelectionHandler
			Implements ChangeListener

			Private ReadOnly outerInstance As BasicTabbedPaneUI

			Public Sub New(ByVal outerInstance As BasicTabbedPaneUI)
				Me.outerInstance = outerInstance
			End Sub

			' NOTE: This class exists only for backward compatibility. All
			' its functionality has been moved into Handler. If you need to add
			' new functionality add it to the Handler, but make sure this
			' class calls into the Handler.
			Public Overridable Sub stateChanged(ByVal e As ChangeEvent) Implements ChangeListener.stateChanged
				outerInstance.handler.stateChanged(e)
			End Sub
		End Class

		''' <summary>
		''' This class should be treated as a &quot;protected&quot; inner class.
		''' Instantiate it only within subclasses of BasicTabbedPaneUI.
		''' </summary>
		Public Class MouseHandler
			Inherits MouseAdapter

			Private ReadOnly outerInstance As BasicTabbedPaneUI

			Public Sub New(ByVal outerInstance As BasicTabbedPaneUI)
				Me.outerInstance = outerInstance
			End Sub

			' NOTE: This class exists only for backward compatibility. All
			' its functionality has been moved into Handler. If you need to add
			' new functionality add it to the Handler, but make sure this
			' class calls into the Handler.
			Public Overridable Sub mousePressed(ByVal e As MouseEvent)
				outerInstance.handler.mousePressed(e)
			End Sub
		End Class

		''' <summary>
		''' This class should be treated as a &quot;protected&quot; inner class.
		''' Instantiate it only within subclasses of BasicTabbedPaneUI.
		''' </summary>
		Public Class FocusHandler
			Inherits FocusAdapter

			Private ReadOnly outerInstance As BasicTabbedPaneUI

			Public Sub New(ByVal outerInstance As BasicTabbedPaneUI)
				Me.outerInstance = outerInstance
			End Sub

			' NOTE: This class exists only for backward compatibility. All
			' its functionality has been moved into Handler. If you need to add
			' new functionality add it to the Handler, but make sure this
			' class calls into the Handler.
			Public Overridable Sub focusGained(ByVal e As FocusEvent)
				outerInstance.handler.focusGained(e)
			End Sub
			Public Overridable Sub focusLost(ByVal e As FocusEvent)
				outerInstance.handler.focusLost(e)
			End Sub
		End Class

		Private Function createHTMLVector() As List(Of javax.swing.text.View)
			Dim htmlViews As New List(Of javax.swing.text.View)
			Dim count As Integer = tabPane.tabCount
			If count>0 Then
				For i As Integer = 0 To count - 1
					Dim title As String = tabPane.getTitleAt(i)
					If BasicHTML.isHTMLString(title) Then
						htmlViews.Add(BasicHTML.createHTMLView(tabPane, title))
					Else
						htmlViews.Add(Nothing)
					End If
				Next i
			End If
			Return htmlViews
		End Function

		Private Class TabContainer
			Inherits JPanel
			Implements UIResource

			Private ReadOnly outerInstance As BasicTabbedPaneUI

			Private notifyTabbedPane As Boolean = True

			Public Sub New(ByVal outerInstance As BasicTabbedPaneUI)
					Me.outerInstance = outerInstance
				MyBase.New(Nothing)
				opaque = False
			End Sub

			Public Overridable Sub remove(ByVal comp As Component)
				Dim index As Integer = outerInstance.tabPane.indexOfTabComponent(comp)
				MyBase.remove(comp)
				If notifyTabbedPane AndAlso index <> -1 Then outerInstance.tabPane.tabComponentAttAt(index, Nothing)
			End Sub

			Private Sub removeUnusedTabComponents()
				For Each c As Component In components
					If Not(TypeOf c Is UIResource) Then
						Dim index As Integer = outerInstance.tabPane.indexOfTabComponent(c)
						If index = -1 Then MyBase.remove(c)
					End If
				Next c
			End Sub

			Public Property Overrides optimizedDrawingEnabled As Boolean
				Get
					Return outerInstance.tabScroller IsNot Nothing AndAlso Not outerInstance.tabScroller.croppedEdge.paramsSet
				End Get
			End Property

			Public Overridable Sub doLayout()
				' We layout tabComponents in JTabbedPane's layout manager
				' and use this method as a hook for repainting tabs
				' to update tabs area e.g. when the size of tabComponent was changed
				If outerInstance.scrollableTabLayoutEnabled() Then
					outerInstance.tabScroller.tabPanel.repaint()
					outerInstance.tabScroller.updateView()
				Else
					outerInstance.tabPane.repaint(bounds)
				End If
			End Sub
		End Class

		Private Class CroppedEdge
			Inherits JPanel
			Implements UIResource

			Private ReadOnly outerInstance As BasicTabbedPaneUI

			Private shape As Shape
			Private tabIndex As Integer
			Private cropline As Integer
			Private cropx, cropy As Integer

			Public Sub New(ByVal outerInstance As BasicTabbedPaneUI)
					Me.outerInstance = outerInstance
				opaque = False
			End Sub

			Public Overridable Sub setParams(ByVal tabIndex As Integer, ByVal cropline As Integer, ByVal cropx As Integer, ByVal cropy As Integer)
				Me.tabIndex = tabIndex
				Me.cropline = cropline
				Me.cropx = cropx
				Me.cropy = cropy
				Dim tabRect As Rectangle = outerInstance.rects(tabIndex)
				bounds = tabRect
				shape = createCroppedTabShape(outerInstance.tabPane.tabPlacement, tabRect, cropline)
				If parent Is Nothing AndAlso outerInstance.tabContainer IsNot Nothing Then outerInstance.tabContainer.add(Me, 0)
			End Sub

			Public Overridable Sub resetParams()
				shape = Nothing
				If parent Is outerInstance.tabContainer AndAlso outerInstance.tabContainer IsNot Nothing Then outerInstance.tabContainer.remove(Me)
			End Sub

			Public Overridable Property paramsSet As Boolean
				Get
					Return shape IsNot Nothing
				End Get
			End Property

			Public Overridable Property tabIndex As Integer
				Get
					Return tabIndex
				End Get
			End Property

			Public Overridable Property cropline As Integer
				Get
					Return cropline
				End Get
			End Property

			Public Overridable Property croppedSideWidth As Integer
				Get
					Return 3
				End Get
			End Property

			Private Property bgColor As Color
				Get
					Dim parent As Component = outerInstance.tabPane.parent
					If parent IsNot Nothing Then
						Dim bg As Color = parent.background
						If bg IsNot Nothing Then Return bg
					End If
					Return UIManager.getColor("control")
				End Get
			End Property

			Protected Friend Overrides Sub paintComponent(ByVal g As Graphics)
				MyBase.paintComponent(g)
				If paramsSet AndAlso TypeOf g Is Graphics2D Then
					Dim g2 As Graphics2D = CType(g, Graphics2D)
					g2.clipRect(0, 0, width, height)
					g2.color = bgColor
					g2.translate(cropx, cropy)
					g2.fill(shape)
					outerInstance.paintCroppedTabEdge(g)
					g2.translate(-cropx, -cropy)
				End If
			End Sub
		End Class
	End Class

End Namespace
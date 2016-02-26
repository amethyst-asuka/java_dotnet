Imports Microsoft.VisualBasic
Imports System
Imports javax.swing
Imports javax.swing.plaf
Imports javax.swing.plaf.basic

'
' * Copyright (c) 2002, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.swing.plaf.synth



	''' <summary>
	''' Provides the Synth L&amp;F UI delegate for
	''' <seealso cref="javax.swing.JTabbedPane"/>.
	''' 
	''' <p>Looks up the {@code selectedTabPadInsets} property from the Style,
	''' which represents additional insets for the selected tab.
	''' 
	''' @author Scott Violet
	''' @since 1.7
	''' </summary>
	Public Class SynthTabbedPaneUI
		Inherits BasicTabbedPaneUI
		Implements java.beans.PropertyChangeListener, SynthUI

		''' <summary>
		''' <p>If non-zero, tabOverlap indicates the amount that the tab bounds
		''' should be altered such that they would overlap with a tab on either the
		''' leading or trailing end of a run (ie: in TOP, this would be on the left
		''' or right).</p>
		''' 
		''' <p>A positive overlap indicates that tabs should overlap right/down,
		''' while a negative overlap indicates tha tabs should overlap left/up.</p>
		''' 
		''' <p>When tabOverlap is specified, it both changes the x position and width
		''' of the tab if in TOP or BOTTOM placement, and changes the y position and
		''' height if in LEFT or RIGHT placement.</p>
		''' 
		''' <p>This is done for the following reason. Consider a run of 10 tabs.
		''' There are 9 gaps between these tabs. If you specified a tabOverlap of
		''' "-1", then each of the tabs "x" values will be shifted left. This leaves
		''' 9 pixels of space to the right of the right-most tab unpainted. So, each
		''' tab's width is also extended by 1 pixel to make up the difference.</p>
		''' 
		''' <p>This property respects the RTL component orientation.</p>
		''' </summary>
		Private tabOverlap As Integer = 0

		''' <summary>
		''' When a tabbed pane has multiple rows of tabs, this indicates whether
		''' the tabs in the upper row(s) should extend to the base of the tab area,
		''' or whether they should remain at their normal tab height. This does not
		''' affect the bounds of the tabs, only the bounds of area painted by the
		''' tabs. The text position does not change. The result is that the bottom
		''' border of the upper row of tabs becomes fully obscured by the lower tabs,
		''' resulting in a cleaner look.
		''' </summary>
		Private extendTabsToBase As Boolean = False

		Private tabAreaContext As SynthContext
		Private tabContext As SynthContext
		Private tabContentContext As SynthContext

		Private style As SynthStyle
		Private tabStyle As SynthStyle
		Private tabAreaStyle As SynthStyle
		Private tabContentStyle As SynthStyle

		Private textRect As New Rectangle
		Private iconRect As New Rectangle

		Private tabAreaBounds As New Rectangle

		'added for the Nimbus look and feel, where the tab area is painted differently depending on the
		'state for the selected tab
		Private tabAreaStatesMatchSelectedTab As Boolean = False
		'added for the Nimbus LAF to ensure that the labels don't move whether the tab is selected or not
		Private nudgeSelectedLabel As Boolean = True

		Private selectedTabIsPressed As Boolean = False

		''' <summary>
		''' Creates a new UI object for the given component.
		''' </summary>
		''' <param name="c"> component to create UI object for </param>
		''' <returns> the UI object </returns>
		Public Shared Function createUI(ByVal c As JComponent) As ComponentUI
			Return New SynthTabbedPaneUI
		End Function

		 Private Function scrollableTabLayoutEnabled() As Boolean
			Return (tabPane.tabLayoutPolicy = JTabbedPane.SCROLL_TAB_LAYOUT)
		 End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub installDefaults()
			updateStyle(tabPane)
		End Sub

		Private Sub updateStyle(ByVal c As JTabbedPane)
			Dim ___context As SynthContext = getContext(c, ENABLED)
			Dim oldStyle As SynthStyle = style
			style = SynthLookAndFeel.updateStyle(___context, Me)
			' Add properties other than JComponent colors, Borders and
			' opacity settings here:
			If style IsNot oldStyle Then
				tabRunOverlay = style.getInt(___context, "TabbedPane.tabRunOverlay", 0)
				tabOverlap = style.getInt(___context, "TabbedPane.tabOverlap", 0)
				extendTabsToBase = style.getBoolean(___context, "TabbedPane.extendTabsToBase", False)
				textIconGap = style.getInt(___context, "TabbedPane.textIconGap", 0)
				selectedTabPadInsets = CType(style.get(___context, "TabbedPane.selectedTabPadInsets"), Insets)
				If selectedTabPadInsets Is Nothing Then selectedTabPadInsets = New Insets(0, 0, 0, 0)
				tabAreaStatesMatchSelectedTab = style.getBoolean(___context, "TabbedPane.tabAreaStatesMatchSelectedTab", False)
				nudgeSelectedLabel = style.getBoolean(___context, "TabbedPane.nudgeSelectedLabel", True)
				If oldStyle IsNot Nothing Then
					uninstallKeyboardActions()
					installKeyboardActions()
				End If
			End If
			___context.Dispose()

			If tabContext IsNot Nothing Then tabContext.Dispose()
			tabContext = getContext(c, Region.TABBED_PANE_TAB, ENABLED)
			Me.tabStyle = SynthLookAndFeel.updateStyle(tabContext, Me)
			tabInsets = tabStyle.getInsets(tabContext, Nothing)


			If tabAreaContext IsNot Nothing Then tabAreaContext.Dispose()
			tabAreaContext = getContext(c, Region.TABBED_PANE_TAB_AREA, ENABLED)
			Me.tabAreaStyle = SynthLookAndFeel.updateStyle(tabAreaContext, Me)
			tabAreaInsets = tabAreaStyle.getInsets(tabAreaContext, Nothing)


			If tabContentContext IsNot Nothing Then tabContentContext.Dispose()
			tabContentContext = getContext(c, Region.TABBED_PANE_CONTENT, ENABLED)
			Me.tabContentStyle = SynthLookAndFeel.updateStyle(tabContentContext, Me)
			contentBorderInsets = tabContentStyle.getInsets(tabContentContext, Nothing)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub installListeners()
			MyBase.installListeners()
			tabPane.addPropertyChangeListener(Me)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub uninstallListeners()
			MyBase.uninstallListeners()
			tabPane.removePropertyChangeListener(Me)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub uninstallDefaults()
			Dim ___context As SynthContext = getContext(tabPane, ENABLED)
			style.uninstallDefaults(___context)
			___context.Dispose()
			style = Nothing

			tabStyle.uninstallDefaults(tabContext)
			tabContext.Dispose()
			tabContext = Nothing
			tabStyle = Nothing

			tabAreaStyle.uninstallDefaults(tabAreaContext)
			tabAreaContext.Dispose()
			tabAreaContext = Nothing
			tabAreaStyle = Nothing

			tabContentStyle.uninstallDefaults(tabContentContext)
			tabContentContext.Dispose()
			tabContentContext = Nothing
			tabContentStyle = Nothing
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Function getContext(ByVal c As JComponent) As SynthContext
			Return getContext(c, SynthLookAndFeel.getComponentState(c))
		End Function

		Private Function getContext(ByVal c As JComponent, ByVal state As Integer) As SynthContext
			Return SynthContext.getContext(c, style, state)
		End Function

		Private Function getContext(ByVal c As JComponent, ByVal subregion As Region, ByVal state As Integer) As SynthContext
			Dim style As SynthStyle = Nothing

			If subregion Is Region.TABBED_PANE_TAB Then
				style = tabStyle
			ElseIf subregion Is Region.TABBED_PANE_TAB_AREA Then
				style = tabAreaStyle
			ElseIf subregion Is Region.TABBED_PANE_CONTENT Then
				style = tabContentStyle
			End If
			Return SynthContext.getContext(c, subregion, style, state)
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Function createScrollButton(ByVal direction As Integer) As JButton
			' added for Nimbus LAF so that it can use the basic arrow buttons
			' UIManager is queried directly here because this is called before
			' updateStyle is called so the style can not be queried directly
			If UIManager.getBoolean("TabbedPane.useBasicArrows") Then
				Dim btn As JButton = MyBase.createScrollButton(direction)
				btn.border = BorderFactory.createEmptyBorder()
				Return btn
			End If
			Return New SynthScrollableTabButton(Me, direction)
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Sub propertyChange(ByVal e As java.beans.PropertyChangeEvent)
			If SynthLookAndFeel.shouldUpdateStyle(e) Then updateStyle(tabPane)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' 
		''' Overridden to keep track of whether the selected tab is also pressed.
		''' </summary>
		Protected Friend Overrides Function createMouseListener() As MouseListener
			Dim [delegate] As MouseListener = MyBase.createMouseListener()
			Dim delegate2 As MouseMotionListener = CType([delegate], MouseMotionListener)
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			Return New MouseListener()
	'		{
	'			public void mouseClicked(MouseEvent e)
	'			{
	'				delegate.mouseClicked(e);
	'			}
	'			public void mouseEntered(MouseEvent e)
	'			{
	'				delegate.mouseEntered(e);
	'			}
	'			public void mouseExited(MouseEvent e)
	'			{
	'				delegate.mouseExited(e);
	'			}
	'
	'			public void mousePressed(MouseEvent e)
	'			{
	'				if (!tabPane.isEnabled())
	'				{
	'					Return;
	'				}
	'
	'				int tabIndex = tabForCoordinate(tabPane, e.getX(), e.getY());
	'				if (tabIndex >= 0 && tabPane.isEnabledAt(tabIndex))
	'				{
	'					if (tabIndex == tabPane.getSelectedIndex())
	'					{
	'						' Clicking on selected tab
	'						selectedTabIsPressed = True;
	'						'TODO need to just repaint the tab area!
	'						tabPane.repaint();
	'					}
	'				}
	'
	'				'forward the event (this will set the selected index, or none at all
	'				delegate.mousePressed(e);
	'			}
	'
	'			public void mouseReleased(MouseEvent e)
	'			{
	'				if (selectedTabIsPressed)
	'				{
	'					selectedTabIsPressed = False;
	'					'TODO need to just repaint the tab area!
	'					tabPane.repaint();
	'				}
	'				'forward the event
	'				delegate.mouseReleased(e);
	'
	'				'hack: The super method *should* be setting the mouse-over property correctly
	'				'here, but it doesn't. That is, when the mouse is released, whatever tab is below the
	'				'released mouse should be in rollover state. But, if you select a tab and don't
	'				'move the mouse, this doesn't happen. Hence, forwarding the event.
	'				delegate2.mouseMoved(e);
	'			}
	'		};
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Function getTabLabelShiftX(ByVal tabPlacement As Integer, ByVal tabIndex As Integer, ByVal isSelected As Boolean) As Integer
			If nudgeSelectedLabel Then
				Return MyBase.getTabLabelShiftX(tabPlacement, tabIndex, isSelected)
			Else
				Return 0
			End If
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Function getTabLabelShiftY(ByVal tabPlacement As Integer, ByVal tabIndex As Integer, ByVal isSelected As Boolean) As Integer
			If nudgeSelectedLabel Then
				Return MyBase.getTabLabelShiftY(tabPlacement, tabIndex, isSelected)
			Else
				Return 0
			End If
		End Function

		''' <summary>
		''' Notifies this UI delegate to repaint the specified component.
		''' This method paints the component background, then calls
		''' the <seealso cref="#paint(SynthContext,Graphics)"/> method.
		''' 
		''' <p>In general, this method does not need to be overridden by subclasses.
		''' All Look and Feel rendering code should reside in the {@code paint} method.
		''' </summary>
		''' <param name="g"> the {@code Graphics} object used for painting </param>
		''' <param name="c"> the component being painted </param>
		''' <seealso cref= #paint(SynthContext,Graphics) </seealso>
		Public Overrides Sub update(ByVal g As Graphics, ByVal c As JComponent)
			Dim ___context As SynthContext = getContext(c)

			SynthLookAndFeel.update(___context, g)
			___context.painter.paintTabbedPaneBackground(___context, g, 0, 0, c.width, c.height)
			paint(___context, g)
			___context.Dispose()
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Function getBaseline(ByVal tab As Integer) As Integer
			If tabPane.getTabComponentAt(tab) IsNot Nothing OrElse getTextViewForTab(tab) IsNot Nothing Then Return MyBase.getBaseline(tab)
			Dim title As String = tabPane.getTitleAt(tab)
			Dim font As Font = tabContext.style.getFont(tabContext)
			Dim metrics As FontMetrics = getFontMetrics(font)
			Dim icon As Icon = getIconForTab(tab)
			textRect.boundsnds(0, 0, 0, 0)
			iconRect.boundsnds(0, 0, 0, 0)
			calcRect.boundsnds(0, 0, Short.MaxValue, maxTabHeight)
			tabContext.style.getGraphicsUtils(tabContext).layoutText(tabContext, metrics, title, icon, SwingUtilities.CENTER, SwingUtilities.CENTER, SwingUtilities.LEADING, SwingUtilities.CENTER, calcRect, iconRect, textRect, textIconGap)
			Return textRect.y + metrics.ascent + baselineOffset
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Sub paintBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			context.painter.paintTabbedPaneBorder(context, g, x, y, w, h)
		End Sub

		''' <summary>
		''' Paints the specified component according to the Look and Feel.
		''' <p>This method is not used by Synth Look and Feel.
		''' Painting is handled by the <seealso cref="#paint(SynthContext,Graphics)"/> method.
		''' </summary>
		''' <param name="g"> the {@code Graphics} object used for painting </param>
		''' <param name="c"> the component being painted </param>
		''' <seealso cref= #paint(SynthContext,Graphics) </seealso>
		Public Overrides Sub paint(ByVal g As Graphics, ByVal c As JComponent)
			Dim ___context As SynthContext = getContext(c)

			paint(___context, g)
			___context.Dispose()
		End Sub

		''' <summary>
		''' Paints the specified component.
		''' </summary>
		''' <param name="context"> context for the component being painted </param>
		''' <param name="g"> the {@code Graphics} object used for painting </param>
		''' <seealso cref= #update(Graphics,JComponent) </seealso>
		Protected Friend Overridable Sub paint(ByVal context As SynthContext, ByVal g As Graphics)
			Dim selectedIndex As Integer = tabPane.selectedIndex
			Dim tabPlacement As Integer = tabPane.tabPlacement

			ensureCurrentLayout()

			' Paint tab area
			' If scrollable tabs are enabled, the tab area will be
			' painted by the scrollable tab panel instead.
			'
			If Not scrollableTabLayoutEnabled() Then ' WRAP_TAB_LAYOUT
				Dim insets As Insets = tabPane.insets
				Dim x As Integer = insets.left
				Dim y As Integer = insets.top
				Dim width As Integer = tabPane.width - insets.left - insets.right
				Dim height As Integer = tabPane.height - insets.top - insets.bottom
				Dim size As Integer
				Select Case tabPlacement
				Case LEFT
					width = calculateTabAreaWidth(tabPlacement, runCount, maxTabWidth)
				Case RIGHT
					size = calculateTabAreaWidth(tabPlacement, runCount, maxTabWidth)
					x = x + width - size
					width = size
				Case BOTTOM
					size = calculateTabAreaHeight(tabPlacement, runCount, maxTabHeight)
					y = y + height - size
					height = size
				Case Else
					height = calculateTabAreaHeight(tabPlacement, runCount, maxTabHeight)
				End Select

				tabAreaBounds.boundsnds(x, y, width, height)

				If g.clipBounds.intersects(tabAreaBounds) Then paintTabArea(tabAreaContext, g, tabPlacement, selectedIndex, tabAreaBounds)
			End If

			' Paint content border
			paintContentBorder(tabContentContext, g, tabPlacement, selectedIndex)
		End Sub

		Protected Friend Overrides Sub paintTabArea(ByVal g As Graphics, ByVal tabPlacement As Integer, ByVal selectedIndex As Integer)
			' This can be invoked from ScrollabeTabPanel
			Dim insets As Insets = tabPane.insets
			Dim x As Integer = insets.left
			Dim y As Integer = insets.top
			Dim width As Integer = tabPane.width - insets.left - insets.right
			Dim height As Integer = tabPane.height - insets.top - insets.bottom

			paintTabArea(tabAreaContext, g, tabPlacement, selectedIndex, New Rectangle(x, y, width, height))
		End Sub

		Private Sub paintTabArea(ByVal ss As SynthContext, ByVal g As Graphics, ByVal tabPlacement As Integer, ByVal selectedIndex As Integer, ByVal tabAreaBounds As Rectangle)
			Dim clipRect As Rectangle = g.clipBounds

			'if the tab area's states should match that of the selected tab, then
			'first update the selected tab's states, then set the state
			'for the tab area to match
			'otherwise, restore the tab area's state to ENABLED (which is the
			'only supported state otherwise).
			If tabAreaStatesMatchSelectedTab AndAlso selectedIndex >= 0 Then
				updateTabContext(selectedIndex, True, selectedTabIsPressed, (rolloverTab = selectedIndex), (focusIndex = selectedIndex))
				ss.componentState = tabContext.componentState
			Else
				ss.componentState = SynthConstants.ENABLED
			End If

			' Paint the tab area.
			SynthLookAndFeel.updateSubregion(ss, g, tabAreaBounds)
			ss.painter.paintTabbedPaneTabAreaBackground(ss, g, tabAreaBounds.x, tabAreaBounds.y, tabAreaBounds.width, tabAreaBounds.height, tabPlacement)
			ss.painter.paintTabbedPaneTabAreaBorder(ss, g, tabAreaBounds.x, tabAreaBounds.y, tabAreaBounds.width, tabAreaBounds.height, tabPlacement)

			Dim tabCount As Integer = tabPane.tabCount

			iconRect.boundsnds(0, 0, 0, 0)
			textRect.boundsnds(0, 0, 0, 0)

			' Paint tabRuns of tabs from back to front
			For i As Integer = runCount - 1 To 0 Step -1
				Dim start As Integer = tabRuns(i)
				Dim [next] As Integer = tabRuns(If(i = runCount - 1, 0, i + 1))
				Dim [end] As Integer = (If([next] <> 0, [next] - 1, tabCount - 1))
				For j As Integer = start To [end]
					If rects(j).intersects(clipRect) AndAlso selectedIndex <> j Then paintTab(tabContext, g, tabPlacement, rects, j, iconRect, textRect)
				Next j
			Next i

			If selectedIndex >= 0 Then
				If rects(selectedIndex).intersects(clipRect) Then paintTab(tabContext, g, tabPlacement, rects, selectedIndex, iconRect, textRect)
			End If
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Property rolloverTab As Integer
			Set(ByVal index As Integer)
				Dim oldRolloverTab As Integer = rolloverTab
				MyBase.rolloverTab = index
    
				Dim r As Rectangle = Nothing
    
				If oldRolloverTab <> index AndAlso tabAreaStatesMatchSelectedTab Then
					'TODO need to just repaint the tab area!
					tabPane.repaint()
				Else
					If (oldRolloverTab >= 0) AndAlso (oldRolloverTab < tabPane.tabCount) Then
						r = getTabBounds(tabPane, oldRolloverTab)
						If r IsNot Nothing Then tabPane.repaint(r)
					End If
    
					If index >= 0 Then
						r = getTabBounds(tabPane, index)
						If r IsNot Nothing Then tabPane.repaint(r)
					End If
				End If
			End Set
		End Property

		Private Sub paintTab(ByVal ss As SynthContext, ByVal g As Graphics, ByVal tabPlacement As Integer, ByVal rects As Rectangle(), ByVal tabIndex As Integer, ByVal iconRect As Rectangle, ByVal textRect As Rectangle)
			Dim tabRect As Rectangle = rects(tabIndex)
			Dim selectedIndex As Integer = tabPane.selectedIndex
			Dim isSelected As Boolean = selectedIndex = tabIndex
			updateTabContext(tabIndex, isSelected, isSelected AndAlso selectedTabIsPressed, (rolloverTab = tabIndex), (focusIndex = tabIndex))

			SynthLookAndFeel.updateSubregion(ss, g, tabRect)
			Dim x As Integer = tabRect.x
			Dim y As Integer = tabRect.y
			Dim height As Integer = tabRect.height
			Dim width As Integer = tabRect.width
			Dim placement As Integer = tabPane.tabPlacement
			If extendTabsToBase AndAlso runCount > 1 Then
				'paint this tab such that its edge closest to the base is equal to
				'edge of the selected tab closest to the base. In terms of the TOP
				'tab placement, this will cause the bottom of each tab to be
				'painted even with the bottom of the selected tab. This is because
				'in each tab placement (TOP, LEFT, BOTTOM, RIGHT) the selected tab
				'is closest to the base.
				If selectedIndex >= 0 Then
					Dim r As Rectangle = rects(selectedIndex)
					Select Case placement
						Case TOP
							Dim bottomY As Integer = r.y + r.height
							height = bottomY - tabRect.y
						Case LEFT
							Dim rightX As Integer = r.x + r.width
							width = rightX - tabRect.x
						Case BOTTOM
							Dim topY As Integer = r.y
							height = (tabRect.y + tabRect.height) - topY
							y = topY
						Case RIGHT
							Dim leftX As Integer = r.x
							width = (tabRect.x + tabRect.width) - leftX
							x = leftX
					End Select
				End If
			End If
			tabContext.painter.paintTabbedPaneTabBackground(tabContext, g, x, y, width, height, tabIndex, placement)
			tabContext.painter.paintTabbedPaneTabBorder(tabContext, g, x, y, width, height, tabIndex, placement)

			If tabPane.getTabComponentAt(tabIndex) Is Nothing Then
				Dim title As String = tabPane.getTitleAt(tabIndex)
				Dim font As Font = ss.style.getFont(ss)
				Dim metrics As FontMetrics = sun.swing.SwingUtilities2.getFontMetrics(tabPane, g, font)
				Dim icon As Icon = getIconForTab(tabIndex)

				layoutLabel(ss, tabPlacement, metrics, tabIndex, title, icon, tabRect, iconRect, textRect, isSelected)

				paintText(ss, g, tabPlacement, font, metrics, tabIndex, title, textRect, isSelected)

				paintIcon(g, tabPlacement, tabIndex, icon, iconRect, isSelected)
			End If
		End Sub

		Private Sub layoutLabel(ByVal ss As SynthContext, ByVal tabPlacement As Integer, ByVal metrics As FontMetrics, ByVal tabIndex As Integer, ByVal title As String, ByVal icon As Icon, ByVal tabRect As Rectangle, ByVal iconRect As Rectangle, ByVal textRect As Rectangle, ByVal isSelected As Boolean)
			Dim v As javax.swing.text.View = getTextViewForTab(tabIndex)
			If v IsNot Nothing Then tabPane.putClientProperty("html", v)

				iconRect.y = 0
					iconRect.x = iconRect.y
						textRect.y = iconRect.x
						textRect.x = textRect.y

			ss.style.getGraphicsUtils(ss).layoutText(ss, metrics, title, icon, SwingUtilities.CENTER, SwingUtilities.CENTER, SwingUtilities.LEADING, SwingUtilities.CENTER, tabRect, iconRect, textRect, textIconGap)

			tabPane.putClientProperty("html", Nothing)

			Dim xNudge As Integer = getTabLabelShiftX(tabPlacement, tabIndex, isSelected)
			Dim yNudge As Integer = getTabLabelShiftY(tabPlacement, tabIndex, isSelected)
			iconRect.x += xNudge
			iconRect.y += yNudge
			textRect.x += xNudge
			textRect.y += yNudge
		End Sub

		Private Sub paintText(ByVal ss As SynthContext, ByVal g As Graphics, ByVal tabPlacement As Integer, ByVal font As Font, ByVal metrics As FontMetrics, ByVal tabIndex As Integer, ByVal title As String, ByVal textRect As Rectangle, ByVal isSelected As Boolean)
			g.font = font

			Dim v As javax.swing.text.View = getTextViewForTab(tabIndex)
			If v IsNot Nothing Then
				' html
				v.paint(g, textRect)
			Else
				' plain text
				Dim mnemIndex As Integer = tabPane.getDisplayedMnemonicIndexAt(tabIndex)

				g.color = ss.style.getColor(ss, ColorType.TEXT_FOREGROUND)
				ss.style.getGraphicsUtils(ss).paintText(ss, g, title, textRect, mnemIndex)
			End If
		End Sub


		Private Sub paintContentBorder(ByVal ss As SynthContext, ByVal g As Graphics, ByVal tabPlacement As Integer, ByVal selectedIndex As Integer)
			Dim width As Integer = tabPane.width
			Dim height As Integer = tabPane.height
			Dim insets As Insets = tabPane.insets

			Dim x As Integer = insets.left
			Dim y As Integer = insets.top
			Dim w As Integer = width - insets.right - insets.left
			Dim h As Integer = height - insets.top - insets.bottom

			Select Case tabPlacement
			  Case LEFT
				  x += calculateTabAreaWidth(tabPlacement, runCount, maxTabWidth)
				  w -= (x - insets.left)
			  Case RIGHT
				  w -= calculateTabAreaWidth(tabPlacement, runCount, maxTabWidth)
			  Case BOTTOM
				  h -= calculateTabAreaHeight(tabPlacement, runCount, maxTabHeight)
			  Case Else
				  y += calculateTabAreaHeight(tabPlacement, runCount, maxTabHeight)
				  h -= (y - insets.top)
			End Select
			SynthLookAndFeel.updateSubregion(ss, g, New Rectangle(x, y, w, h))
			ss.painter.paintTabbedPaneContentBackground(ss, g, x, y, w, h)
			ss.painter.paintTabbedPaneContentBorder(ss, g, x, y, w, h)
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

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Function calculateMaxTabHeight(ByVal tabPlacement As Integer) As Integer
			Dim metrics As FontMetrics = getFontMetrics(tabContext.style.getFont(tabContext))
			Dim tabCount As Integer = tabPane.tabCount
			Dim result As Integer = 0
			Dim fontHeight As Integer = metrics.height
			For i As Integer = 0 To tabCount - 1
				result = Math.Max(calculateTabHeight(tabPlacement, i, fontHeight), result)
			Next i
			Return result
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Function calculateTabWidth(ByVal tabPlacement As Integer, ByVal tabIndex As Integer, ByVal metrics As FontMetrics) As Integer
			Dim icon As Icon = getIconForTab(tabIndex)
			Dim ___tabInsets As Insets = getTabInsets(tabPlacement, tabIndex)
			Dim width As Integer = ___tabInsets.left + ___tabInsets.right
			Dim tabComponent As Component = tabPane.getTabComponentAt(tabIndex)
			If tabComponent IsNot Nothing Then
				width += tabComponent.preferredSize.width
			Else
				If icon IsNot Nothing Then width += icon.iconWidth + textIconGap
				Dim v As javax.swing.text.View = getTextViewForTab(tabIndex)
				If v IsNot Nothing Then
					' html
					width += CInt(Fix(v.getPreferredSpan(javax.swing.text.View.X_AXIS)))
				Else
					' plain text
					Dim title As String = tabPane.getTitleAt(tabIndex)
					width += tabContext.style.getGraphicsUtils(tabContext).computeStringWidth(tabContext, metrics.font, metrics, title)
				End If
			End If
			Return width
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Function calculateMaxTabWidth(ByVal tabPlacement As Integer) As Integer
			Dim metrics As FontMetrics = getFontMetrics(tabContext.style.getFont(tabContext))
			Dim tabCount As Integer = tabPane.tabCount
			Dim result As Integer = 0
			For i As Integer = 0 To tabCount - 1
				result = Math.Max(calculateTabWidth(tabPlacement, i, metrics), result)
			Next i
			Return result
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Function getTabInsets(ByVal tabPlacement As Integer, ByVal tabIndex As Integer) As Insets
			updateTabContext(tabIndex, False, False, False, (focusIndex = tabIndex))
			Return tabInsets
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Property Overrides fontMetrics As FontMetrics
			Get
				Return getFontMetrics(tabContext.style.getFont(tabContext))
			End Get
		End Property

		Private Function getFontMetrics(ByVal font As Font) As FontMetrics
			Return tabPane.getFontMetrics(font)
		End Function

		Private Sub updateTabContext(ByVal index As Integer, ByVal selected As Boolean, ByVal isMouseDown As Boolean, ByVal isMouseOver As Boolean, ByVal hasFocus As Boolean)
			Dim state As Integer = 0
			If (Not tabPane.enabled) OrElse (Not tabPane.isEnabledAt(index)) Then
				state = state Or SynthConstants.DISABLED
				If selected Then state = state Or SynthConstants.SELECTED
			ElseIf selected Then
				state = state Or (SynthConstants.ENABLED Or SynthConstants.SELECTED)
				If isMouseOver AndAlso UIManager.getBoolean("TabbedPane.isTabRollover") Then state = state Or SynthConstants.MOUSE_OVER
			ElseIf isMouseOver Then
				state = state Or (SynthConstants.ENABLED Or SynthConstants.MOUSE_OVER)
			Else
				state = SynthLookAndFeel.getComponentState(tabPane)
				state = state And Not SynthConstants.FOCUSED ' don't use tabbedpane focus state
			End If
			If hasFocus AndAlso tabPane.hasFocus() Then state = state Or SynthConstants.FOCUSED ' individual tab has focus
			If isMouseDown Then state = state Or SynthConstants.PRESSED

			tabContext.componentState = state
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' 
		''' Overridden to create a TabbedPaneLayout subclass which takes into
		''' account tabOverlap.
		''' </summary>
		Protected Friend Overrides Function createLayoutManager() As LayoutManager
			If tabPane.tabLayoutPolicy = JTabbedPane.SCROLL_TAB_LAYOUT Then
				Return MyBase.createLayoutManager() ' WRAP_TAB_LAYOUT
			Else
				Return New TabbedPaneLayoutAnonymousInnerClassHelper
			End If
		End Function

		Private Class TabbedPaneLayoutAnonymousInnerClassHelper
			Inherits TabbedPaneLayout

			Public Overrides Sub calculateLayoutInfo()
				MyBase.calculateLayoutInfo()
				'shift all the tabs, if necessary
				If outerInstance.tabOverlap <> 0 Then
					Dim tabCount As Integer = outerInstance.tabPane.tabCount
					'left-to-right/right-to-left only affects layout
					'when placement is TOP or BOTTOM
					Dim ltr As Boolean = outerInstance.tabPane.componentOrientation.leftToRight
					For i As Integer = outerInstance.runCount - 1 To 0 Step -1
						Dim start As Integer = outerInstance.tabRuns(i)
						Dim [next] As Integer = outerInstance.tabRuns(If(i = outerInstance.runCount - 1, 0, i + 1))
						Dim [end] As Integer = (If([next] <> 0, [next] - 1, tabCount - 1))
						For j As Integer = start+1 To [end]
							' xshift and yshift represent the amount &
							' direction to shift the tab in their
							' respective axis.
							Dim xshift As Integer = 0
							Dim yshift As Integer = 0
							' configure xshift and y shift based on tab
							' position and ltr/rtl
							Select Case outerInstance.tabPane.tabPlacement
								Case JTabbedPane.TOP, JTabbedPane.BOTTOM
									xshift = If(ltr, outerInstance.tabOverlap, -outerInstance.tabOverlap)
								Case JTabbedPane.LEFT, JTabbedPane.RIGHT
									yshift = outerInstance.tabOverlap
								Case Else 'do nothing
							End Select
							outerInstance.rects(j).x += xshift
							outerInstance.rects(j).y += yshift
							outerInstance.rects(j).width += Math.Abs(xshift)
							outerInstance.rects(j).height += Math.Abs(yshift)
						Next j
					Next i
				End If
			End Sub
		End Class

		Private Class SynthScrollableTabButton
			Inherits SynthArrowButton
			Implements UIResource

			Private ReadOnly outerInstance As SynthTabbedPaneUI

			Public Sub New(ByVal outerInstance As SynthTabbedPaneUI, ByVal direction As Integer)
					Me.outerInstance = outerInstance
				MyBase.New(direction)
				name = "TabbedPane.button"
			End Sub
		End Class
	End Class

End Namespace
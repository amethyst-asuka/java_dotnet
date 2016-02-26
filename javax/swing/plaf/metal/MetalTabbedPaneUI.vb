Imports javax.swing
Imports javax.swing.event
Imports javax.swing.plaf

'
' * Copyright (c) 1998, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.swing.plaf.metal


	''' <summary>
	''' The Metal subclass of BasicTabbedPaneUI.
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
	''' @author Tom Santos
	''' </summary>

	Public Class MetalTabbedPaneUI
		Inherits javax.swing.plaf.basic.BasicTabbedPaneUI

		Protected Friend minTabWidth As Integer = 40
		' Background color for unselected tabs that don't have an explicitly
		' set color.
		Private unselectedBackground As Color
		Protected Friend tabAreaBackground As Color
		Protected Friend selectColor As Color
		Protected Friend selectHighlight As Color
		Private tabsOpaque As Boolean = True

		' Whether or not we're using ocean. This is cached as it is used
		' extensively during painting.
		Private ocean As Boolean
		' Selected border color for ocean.
		Private oceanSelectedBorderColor As Color

		Public Shared Function createUI(ByVal x As JComponent) As ComponentUI
			Return New MetalTabbedPaneUI
		End Function

		Protected Friend Overrides Function createLayoutManager() As LayoutManager
			If tabPane.tabLayoutPolicy = JTabbedPane.SCROLL_TAB_LAYOUT Then Return MyBase.createLayoutManager()
			Return New TabbedPaneLayout(Me)
		End Function

		Protected Friend Overrides Sub installDefaults()
			MyBase.installDefaults()

			tabAreaBackground = UIManager.getColor("TabbedPane.tabAreaBackground")
			selectColor = UIManager.getColor("TabbedPane.selected")
			selectHighlight = UIManager.getColor("TabbedPane.selectHighlight")
			tabsOpaque = UIManager.getBoolean("TabbedPane.tabsOpaque")
			unselectedBackground = UIManager.getColor("TabbedPane.unselectedBackground")
			ocean = MetalLookAndFeel.usingOcean()
			If ocean Then oceanSelectedBorderColor = UIManager.getColor("TabbedPane.borderHightlightColor")
		End Sub


		Protected Friend Overrides Sub paintTabBorder(ByVal g As Graphics, ByVal tabPlacement As Integer, ByVal tabIndex As Integer, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal isSelected As Boolean)
			Dim bottom As Integer = y + (h-1)
			Dim right As Integer = x + (w-1)

			Select Case tabPlacement
			Case LEFT
				paintLeftTabBorder(tabIndex, g, x, y, w, h, bottom, right, isSelected)
			Case BOTTOM
				paintBottomTabBorder(tabIndex, g, x, y, w, h, bottom, right, isSelected)
			Case RIGHT
				paintRightTabBorder(tabIndex, g, x, y, w, h, bottom, right, isSelected)
			Case Else
				paintTopTabBorder(tabIndex, g, x, y, w, h, bottom, right, isSelected)
			End Select
		End Sub


		Protected Friend Overridable Sub paintTopTabBorder(ByVal tabIndex As Integer, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal btm As Integer, ByVal rght As Integer, ByVal isSelected As Boolean)
			Dim currentRun As Integer = getRunForTab(tabPane.tabCount, tabIndex)
			Dim lastIndex As Integer = lastTabInRun(tabPane.tabCount, currentRun)
			Dim firstIndex As Integer = tabRuns(currentRun)
			Dim leftToRight As Boolean = MetalUtils.isLeftToRight(tabPane)
			Dim selectedIndex As Integer = tabPane.selectedIndex
			Dim bottom As Integer = h - 1
			Dim right As Integer = w - 1

			'
			' Paint Gap
			'

			If shouldFillGap(currentRun, tabIndex, x, y) Then
				g.translate(x, y)

				If leftToRight Then
					g.color = getColorForGap(currentRun, x, y + 1)
					g.fillRect(1, 0, 5, 3)
					g.fillRect(1, 3, 2, 2)
				Else
					g.color = getColorForGap(currentRun, x + w - 1, y + 1)
					g.fillRect(right - 5, 0, 5, 3)
					g.fillRect(right - 2, 3, 2, 2)
				End If

				g.translate(-x, -y)
			End If

			g.translate(x, y)

			'
			' Paint Border
			'

			If ocean AndAlso isSelected Then
				g.color = oceanSelectedBorderColor
			Else
				g.color = darkShadow
			End If

			If leftToRight Then

				' Paint slant
				g.drawLine(1, 5, 6, 0)

				' Paint top
				g.drawLine(6, 0, right, 0)

				' Paint right
				If tabIndex=lastIndex Then g.drawLine(right, 1, right, bottom)

				If ocean AndAlso tabIndex - 1 = selectedIndex AndAlso currentRun = getRunForTab(tabPane.tabCount, selectedIndex) Then g.color = oceanSelectedBorderColor

				' Paint left
				If tabIndex <> tabRuns(runCount - 1) Then
					' not the first tab in the last run
					If ocean AndAlso isSelected Then
						g.drawLine(0, 6, 0, bottom)
						g.color = darkShadow
						g.drawLine(0, 0, 0, 5)
					Else
						g.drawLine(0, 0, 0, bottom)
					End If
				Else
					' the first tab in the last run
					g.drawLine(0, 6, 0, bottom)
				End If
			Else

				' Paint slant
				g.drawLine(right - 1, 5, right - 6, 0)

				' Paint top
				g.drawLine(right - 6, 0, 0, 0)

				' Paint left
				If tabIndex=lastIndex Then g.drawLine(0, 1, 0, bottom)

				' Paint right
				If ocean AndAlso tabIndex - 1 = selectedIndex AndAlso currentRun = getRunForTab(tabPane.tabCount, selectedIndex) Then
					g.color = oceanSelectedBorderColor
					g.drawLine(right, 0, right, bottom)
				ElseIf ocean AndAlso isSelected Then
					g.drawLine(right, 6, right, bottom)
					If tabIndex <> 0 Then
						g.color = darkShadow
						g.drawLine(right, 0, right, 5)
					End If
				Else
					If tabIndex <> tabRuns(runCount - 1) Then
						' not the first tab in the last run
						g.drawLine(right, 0, right, bottom)
					Else
						' the first tab in the last run
						g.drawLine(right, 6, right, bottom)
					End If
				End If
			End If

			'
			' Paint Highlight
			'

			g.color = If(isSelected, selectHighlight, highlight)

			If leftToRight Then

				' Paint slant
				g.drawLine(1, 6, 6, 1)

				' Paint top
				g.drawLine(6, 1,If(tabIndex = lastIndex, right - 1, right), 1)

				' Paint left
				g.drawLine(1, 6, 1, bottom)

				' paint highlight in the gap on tab behind this one
				' on the left end (where they all line up)
				If tabIndex=firstIndex AndAlso tabIndex<>tabRuns(runCount - 1) Then
					'  first tab in run but not first tab in last run
					If tabPane.selectedIndex=tabRuns(currentRun+1) Then
						' tab in front of selected tab
						g.color = selectHighlight
					Else
						' tab in front of normal tab
						g.color = highlight
					End If
					g.drawLine(1, 0, 1, 4)
				End If
			Else

				' Paint slant
				g.drawLine(right - 1, 6, right - 6, 1)

				' Paint top
				g.drawLine(right - 6, 1, 1, 1)

				' Paint left
				If tabIndex=lastIndex Then
					' last tab in run
					g.drawLine(1, 1, 1, bottom)
				Else
					g.drawLine(0, 1, 0, bottom)
				End If
			End If

			g.translate(-x, -y)
		End Sub

		Protected Friend Overridable Function shouldFillGap(ByVal currentRun As Integer, ByVal tabIndex As Integer, ByVal x As Integer, ByVal y As Integer) As Boolean
			Dim result As Boolean = False

			If Not tabsOpaque Then Return False

			If currentRun = runCount - 2 Then ' If it's the second to last row.
				Dim lastTabBounds As Rectangle = getTabBounds(tabPane, tabPane.tabCount - 1)
				Dim ___tabBounds As Rectangle = getTabBounds(tabPane, tabIndex)
				If MetalUtils.isLeftToRight(tabPane) Then
					Dim lastTabRight As Integer = lastTabBounds.x + lastTabBounds.width - 1

					' is the right edge of the last tab to the right
					' of the left edge of the current tab?
					If lastTabRight > ___tabBounds.x + 2 Then Return True
				Else
					Dim lastTabLeft As Integer = lastTabBounds.x
					Dim currentTabRight As Integer = ___tabBounds.x + ___tabBounds.width - 1

					' is the left edge of the last tab to the left
					' of the right edge of the current tab?
					If lastTabLeft < currentTabRight - 2 Then Return True
				End If
			Else
				' fill in gap for all other rows except last row
				result = currentRun <> runCount - 1
			End If

			Return result
		End Function

		Protected Friend Overridable Function getColorForGap(ByVal currentRun As Integer, ByVal x As Integer, ByVal y As Integer) As Color
			Const shadowWidth As Integer = 4
			Dim selectedIndex As Integer = tabPane.selectedIndex
			Dim startIndex As Integer = tabRuns(currentRun + 1)
			Dim endIndex As Integer = lastTabInRun(tabPane.tabCount, currentRun + 1)
			Dim tabOverGap As Integer = -1
			' Check each tab in the row that is 'on top' of this row
			For i As Integer = startIndex To endIndex
				Dim ___tabBounds As Rectangle = getTabBounds(tabPane, i)
				Dim tabLeft As Integer = ___tabBounds.x
				Dim tabRight As Integer = (___tabBounds.x + ___tabBounds.width) - 1
				' Check to see if this tab is over the gap
				If MetalUtils.isLeftToRight(tabPane) Then
					If tabLeft <= x AndAlso tabRight - shadowWidth > x Then Return If(selectedIndex = i, selectColor, getUnselectedBackgroundAt(i))
				Else
					If tabLeft + shadowWidth < x AndAlso tabRight >= x Then Return If(selectedIndex = i, selectColor, getUnselectedBackgroundAt(i))
				End If
			Next i

			Return tabPane.background
		End Function

		Protected Friend Overridable Sub paintLeftTabBorder(ByVal tabIndex As Integer, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal btm As Integer, ByVal rght As Integer, ByVal isSelected As Boolean)
			Dim tabCount As Integer = tabPane.tabCount
			Dim currentRun As Integer = getRunForTab(tabCount, tabIndex)
			Dim lastIndex As Integer = lastTabInRun(tabCount, currentRun)
			Dim firstIndex As Integer = tabRuns(currentRun)

			g.translate(x, y)

			Dim bottom As Integer = h - 1
			Dim right As Integer = w - 1

			'
			' Paint part of the tab above
			'

			If tabIndex <> firstIndex AndAlso tabsOpaque Then
				g.color = If(tabPane.selectedIndex = tabIndex - 1, selectColor, getUnselectedBackgroundAt(tabIndex - 1))
				g.fillRect(2, 0, 4, 3)
				g.drawLine(2, 3, 2, 3)
			End If


			'
			' Paint Highlight
			'

			If ocean Then
				g.color = If(isSelected, selectHighlight, MetalLookAndFeel.white)
			Else
				g.color = If(isSelected, selectHighlight, highlight)
			End If

			' Paint slant
			g.drawLine(1, 6, 6, 1)

			' Paint left
			g.drawLine(1, 6, 1, bottom)

			' Paint top
			g.drawLine(6, 1, right, 1)

			If tabIndex <> firstIndex Then
				If tabPane.selectedIndex = tabIndex - 1 Then
					g.color = selectHighlight
				Else
					g.color = If(ocean, MetalLookAndFeel.white, highlight)
				End If

				g.drawLine(1, 0, 1, 4)
			End If

			'
			' Paint Border
			'

			If ocean Then
				If isSelected Then
					g.color = oceanSelectedBorderColor
				Else
					g.color = darkShadow
				End If
			Else
				g.color = darkShadow
			End If

			' Paint slant
			g.drawLine(1, 5, 6, 0)

			' Paint top
			g.drawLine(6, 0, right, 0)

			' Paint bottom
			If tabIndex = lastIndex Then g.drawLine(0, bottom, right, bottom)

			' Paint left
			If ocean Then
				If tabPane.selectedIndex = tabIndex - 1 Then
					g.drawLine(0, 5, 0, bottom)
					g.color = oceanSelectedBorderColor
					g.drawLine(0, 0, 0, 5)
				ElseIf isSelected Then
					g.drawLine(0, 6, 0, bottom)
					If tabIndex <> 0 Then
						g.color = darkShadow
						g.drawLine(0, 0, 0, 5)
					End If
				ElseIf tabIndex <> firstIndex Then
					g.drawLine(0, 0, 0, bottom)
				Else
					g.drawLine(0, 6, 0, bottom)
				End If
			Else ' metal
				If tabIndex <> firstIndex Then
					g.drawLine(0, 0, 0, bottom)
				Else
					g.drawLine(0, 6, 0, bottom)
				End If
			End If

			g.translate(-x, -y)
		End Sub


		Protected Friend Overridable Sub paintBottomTabBorder(ByVal tabIndex As Integer, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal btm As Integer, ByVal rght As Integer, ByVal isSelected As Boolean)
			Dim tabCount As Integer = tabPane.tabCount
			Dim currentRun As Integer = getRunForTab(tabCount, tabIndex)
			Dim lastIndex As Integer = lastTabInRun(tabCount, currentRun)
			Dim firstIndex As Integer = tabRuns(currentRun)
			Dim leftToRight As Boolean = MetalUtils.isLeftToRight(tabPane)

			Dim bottom As Integer = h - 1
			Dim right As Integer = w - 1

			'
			' Paint Gap
			'

			If shouldFillGap(currentRun, tabIndex, x, y) Then
				g.translate(x, y)

				If leftToRight Then
					g.color = getColorForGap(currentRun, x, y)
					g.fillRect(1, bottom - 4, 3, 5)
					g.fillRect(4, bottom - 1, 2, 2)
				Else
					g.color = getColorForGap(currentRun, x + w - 1, y)
					g.fillRect(right - 3, bottom - 3, 3, 4)
					g.fillRect(right - 5, bottom - 1, 2, 2)
					g.drawLine(right - 1, bottom - 4, right - 1, bottom - 4)
				End If

				g.translate(-x, -y)
			End If

			g.translate(x, y)


			'
			' Paint Border
			'

			If ocean AndAlso isSelected Then
				g.color = oceanSelectedBorderColor
			Else
				g.color = darkShadow
			End If

			If leftToRight Then

				' Paint slant
				g.drawLine(1, bottom - 5, 6, bottom)

				' Paint bottom
				g.drawLine(6, bottom, right, bottom)

				' Paint right
				If tabIndex = lastIndex Then g.drawLine(right, 0, right, bottom)

				' Paint left
				If ocean AndAlso isSelected Then
					g.drawLine(0, 0, 0, bottom - 6)
					If (currentRun = 0 AndAlso tabIndex <> 0) OrElse (currentRun > 0 AndAlso tabIndex <> tabRuns(currentRun - 1)) Then
						g.color = darkShadow
						g.drawLine(0, bottom - 5, 0, bottom)
					End If
				Else
					If ocean AndAlso tabIndex = tabPane.selectedIndex + 1 Then g.color = oceanSelectedBorderColor
					If tabIndex <> tabRuns(runCount - 1) Then
						g.drawLine(0, 0, 0, bottom)
					Else
						g.drawLine(0, 0, 0, bottom - 6)
					End If
				End If
			Else

				' Paint slant
				g.drawLine(right - 1, bottom - 5, right - 6, bottom)

				' Paint bottom
				g.drawLine(right - 6, bottom, 0, bottom)

				' Paint left
				If tabIndex=lastIndex Then g.drawLine(0, 0, 0, bottom)

				' Paint right
				If ocean AndAlso tabIndex = tabPane.selectedIndex + 1 Then
					g.color = oceanSelectedBorderColor
					g.drawLine(right, 0, right, bottom)
				ElseIf ocean AndAlso isSelected Then
					g.drawLine(right, 0, right, bottom - 6)
					If tabIndex <> firstIndex Then
						g.color = darkShadow
						g.drawLine(right, bottom - 5, right, bottom)
					End If
				ElseIf tabIndex <> tabRuns(runCount - 1) Then
					' not the first tab in the last run
					g.drawLine(right, 0, right, bottom)
				Else
					' the first tab in the last run
					g.drawLine(right, 0, right, bottom - 6)
				End If
			End If

			'
			' Paint Highlight
			'

			g.color = If(isSelected, selectHighlight, highlight)

			If leftToRight Then

				' Paint slant
				g.drawLine(1, bottom - 6, 6, bottom - 1)

				' Paint left
				g.drawLine(1, 0, 1, bottom - 6)

				' paint highlight in the gap on tab behind this one
				' on the left end (where they all line up)
				If tabIndex=firstIndex AndAlso tabIndex<>tabRuns(runCount - 1) Then
					'  first tab in run but not first tab in last run
					If tabPane.selectedIndex=tabRuns(currentRun+1) Then
						' tab in front of selected tab
						g.color = selectHighlight
					Else
						' tab in front of normal tab
						g.color = highlight
					End If
					g.drawLine(1, bottom - 4, 1, bottom)
				End If
			Else

				' Paint left
				If tabIndex=lastIndex Then
					' last tab in run
					g.drawLine(1, 0, 1, bottom - 1)
				Else
					g.drawLine(0, 0, 0, bottom - 1)
				End If
			End If

			g.translate(-x, -y)
		End Sub

		Protected Friend Overridable Sub paintRightTabBorder(ByVal tabIndex As Integer, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal btm As Integer, ByVal rght As Integer, ByVal isSelected As Boolean)
			Dim tabCount As Integer = tabPane.tabCount
			Dim currentRun As Integer = getRunForTab(tabCount, tabIndex)
			Dim lastIndex As Integer = lastTabInRun(tabCount, currentRun)
			Dim firstIndex As Integer = tabRuns(currentRun)

			g.translate(x, y)

			Dim bottom As Integer = h - 1
			Dim right As Integer = w - 1

			'
			' Paint part of the tab above
			'

			If tabIndex <> firstIndex AndAlso tabsOpaque Then
				g.color = If(tabPane.selectedIndex = tabIndex - 1, selectColor, getUnselectedBackgroundAt(tabIndex - 1))
				g.fillRect(right - 5, 0, 5, 3)
				g.fillRect(right - 2, 3, 2, 2)
			End If


			'
			' Paint Highlight
			'

			g.color = If(isSelected, selectHighlight, highlight)

			' Paint slant
			g.drawLine(right - 6, 1, right - 1, 6)

			' Paint top
			g.drawLine(0, 1, right - 6, 1)

			' Paint left
			If Not isSelected Then g.drawLine(0, 1, 0, bottom)


			'
			' Paint Border
			'

			If ocean AndAlso isSelected Then
				g.color = oceanSelectedBorderColor
			Else
				g.color = darkShadow
			End If

			' Paint bottom
			If tabIndex = lastIndex Then g.drawLine(0, bottom, right, bottom)

			' Paint slant
			If ocean AndAlso tabPane.selectedIndex = tabIndex - 1 Then g.color = oceanSelectedBorderColor
			g.drawLine(right - 6, 0, right, 6)

			' Paint top
			g.drawLine(0, 0, right - 6, 0)

			' Paint right
			If ocean AndAlso isSelected Then
				g.drawLine(right, 6, right, bottom)
				If tabIndex <> firstIndex Then
					g.color = darkShadow
					g.drawLine(right, 0, right, 5)
				End If
			ElseIf ocean AndAlso tabPane.selectedIndex = tabIndex - 1 Then
				g.color = oceanSelectedBorderColor
				g.drawLine(right, 0, right, 6)
				g.color = darkShadow
				g.drawLine(right, 6, right, bottom)
			ElseIf tabIndex <> firstIndex Then
				g.drawLine(right, 0, right, bottom)
			Else
				g.drawLine(right, 6, right, bottom)
			End If

			g.translate(-x, -y)
		End Sub

		Public Overridable Sub update(ByVal g As Graphics, ByVal c As JComponent)
			If c.opaque Then
				g.color = tabAreaBackground
				g.fillRect(0, 0, c.width,c.height)
			End If
			paint(g, c)
		End Sub

		Protected Friend Overrides Sub paintTabBackground(ByVal g As Graphics, ByVal tabPlacement As Integer, ByVal tabIndex As Integer, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal isSelected As Boolean)
			Dim slantWidth As Integer = h \ 2
			If isSelected Then
				g.color = selectColor
			Else
				g.color = getUnselectedBackgroundAt(tabIndex)
			End If

			If MetalUtils.isLeftToRight(tabPane) Then
				Select Case tabPlacement
					Case LEFT
						g.fillRect(x + 5, y + 1, w - 5, h - 1)
						g.fillRect(x + 2, y + 4, 3, h - 4)
					Case BOTTOM
						g.fillRect(x + 2, y, w - 2, h - 4)
						g.fillRect(x + 5, y + (h - 1) - 3, w - 5, 3)
					Case RIGHT
						g.fillRect(x, y + 2, w - 4, h - 2)
						g.fillRect(x + (w - 1) - 3, y + 5, 3, h - 5)
					Case Else
						g.fillRect(x + 4, y + 2, (w - 1) - 3, (h - 1) - 1)
						g.fillRect(x + 2, y + 5, 2, h - 5)
				End Select
			Else
				Select Case tabPlacement
					Case LEFT
						g.fillRect(x + 5, y + 1, w - 5, h - 1)
						g.fillRect(x + 2, y + 4, 3, h - 4)
					Case BOTTOM
						g.fillRect(x, y, w - 5, h - 1)
						g.fillRect(x + (w - 1) - 4, y, 4, h - 5)
						g.fillRect(x + (w - 1) - 4, y + (h - 1) - 4, 2, 2)
					Case RIGHT
						g.fillRect(x + 1, y + 1, w - 5, h - 1)
						g.fillRect(x + (w - 1) - 3, y + 5, 3, h - 5)
					Case Else
						g.fillRect(x, y + 2, (w - 1) - 3, (h - 1) - 1)
						g.fillRect(x + (w - 1) - 3, y + 5, 3, h - 3)
				End Select
			End If
		End Sub

		''' <summary>
		''' Overridden to do nothing for the Java L&amp;F.
		''' </summary>
		Protected Friend Overrides Function getTabLabelShiftX(ByVal tabPlacement As Integer, ByVal tabIndex As Integer, ByVal isSelected As Boolean) As Integer
			Return 0
		End Function


		''' <summary>
		''' Overridden to do nothing for the Java L&amp;F.
		''' </summary>
		Protected Friend Overrides Function getTabLabelShiftY(ByVal tabPlacement As Integer, ByVal tabIndex As Integer, ByVal isSelected As Boolean) As Integer
			Return 0
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' 
		''' @since 1.6
		''' </summary>
		Protected Friend Property Overrides baselineOffset As Integer
			Get
				Return 0
			End Get
		End Property

		Public Overrides Sub paint(ByVal g As Graphics, ByVal c As JComponent)
			Dim tabPlacement As Integer = tabPane.tabPlacement

			Dim insets As Insets = c.insets
			Dim size As Dimension = c.size

			' Paint the background for the tab area
			If tabPane.opaque Then
				If (Not c.backgroundSet) AndAlso (tabAreaBackground IsNot Nothing) Then
					g.color = tabAreaBackground
				Else
					g.color = c.background
				End If
				Select Case tabPlacement
				Case LEFT
					g.fillRect(insets.left, insets.top, calculateTabAreaWidth(tabPlacement, runCount, maxTabWidth), size.height - insets.bottom - insets.top)
				Case BOTTOM
					Dim totalTabHeight As Integer = calculateTabAreaHeight(tabPlacement, runCount, maxTabHeight)
					g.fillRect(insets.left, size.height - insets.bottom - totalTabHeight, size.width - insets.left - insets.right, totalTabHeight)
				Case RIGHT
					Dim totalTabWidth As Integer = calculateTabAreaWidth(tabPlacement, runCount, maxTabWidth)
					g.fillRect(size.width - insets.right - totalTabWidth, insets.top, totalTabWidth, size.height - insets.top - insets.bottom)
				Case Else
					g.fillRect(insets.left, insets.top, size.width - insets.right - insets.left, calculateTabAreaHeight(tabPlacement, runCount, maxTabHeight))
					paintHighlightBelowTab()
				End Select
			End If

			MyBase.paint(g, c)
		End Sub

		Protected Friend Overridable Sub paintHighlightBelowTab()

		End Sub


		Protected Friend Overrides Sub paintFocusIndicator(ByVal g As Graphics, ByVal tabPlacement As Integer, ByVal rects As Rectangle(), ByVal tabIndex As Integer, ByVal iconRect As Rectangle, ByVal textRect As Rectangle, ByVal isSelected As Boolean)
			If tabPane.hasFocus() AndAlso isSelected Then
				Dim tabRect As Rectangle = rects(tabIndex)
				Dim ___lastInRun As Boolean = isLastInRun(tabIndex)
				g.color = focus
				g.translate(tabRect.x, tabRect.y)
				Dim right As Integer = tabRect.width - 1
				Dim bottom As Integer = tabRect.height - 1
				Dim leftToRight As Boolean = MetalUtils.isLeftToRight(tabPane)
				Select Case tabPlacement
				Case RIGHT
					g.drawLine(right - 6,2, right - 2,6) ' slant
					g.drawLine(1,2, right - 6,2) ' top
					g.drawLine(right - 2,6, right - 2,bottom) ' right
					g.drawLine(1,2, 1,bottom) ' left
					g.drawLine(1,bottom, right - 2,bottom) ' bottom
				Case BOTTOM
					If leftToRight Then
						g.drawLine(2, bottom - 6, 6, bottom - 2) ' slant
						g.drawLine(6, bottom - 2, right, bottom - 2) ' bottom
						g.drawLine(2, 0, 2, bottom - 6) ' left
						g.drawLine(2, 0, right, 0) ' top
						g.drawLine(right, 0, right, bottom - 2) ' right
					Else
						g.drawLine(right - 2, bottom - 6, right - 6, bottom - 2) ' slant
						g.drawLine(right - 2, 0, right - 2, bottom - 6) ' right
						If ___lastInRun Then
							' last tab in run
							g.drawLine(2, bottom - 2, right - 6, bottom - 2) ' bottom
							g.drawLine(2, 0, right - 2, 0) ' top
							g.drawLine(2, 0, 2, bottom - 2) ' left
						Else
							g.drawLine(1, bottom - 2, right - 6, bottom - 2) ' bottom
							g.drawLine(1, 0, right - 2, 0) ' top
							g.drawLine(1, 0, 1, bottom - 2) ' left
						End If
					End If
				Case LEFT
					g.drawLine(2, 6, 6, 2) ' slant
					g.drawLine(2, 6, 2, bottom - 1) ' left
					g.drawLine(6, 2, right, 2) ' top
					g.drawLine(right, 2, right, bottom - 1) ' right
					g.drawLine(2, bottom - 1, right, bottom - 1) ' bottom
				 Case Else
						If leftToRight Then
							g.drawLine(2, 6, 6, 2) ' slant
							g.drawLine(2, 6, 2, bottom - 1) ' left
							g.drawLine(6, 2, right, 2) ' top
							g.drawLine(right, 2, right, bottom - 1) ' right
							g.drawLine(2, bottom - 1, right, bottom - 1) ' bottom
						Else
							g.drawLine(right - 2, 6, right - 6, 2) ' slant
							g.drawLine(right - 2, 6, right - 2, bottom - 1) ' right
							If ___lastInRun Then
								' last tab in run
								g.drawLine(right - 6, 2, 2, 2) ' top
								g.drawLine(2, 2, 2, bottom - 1) ' left
								g.drawLine(right - 2, bottom - 1, 2, bottom - 1) ' bottom
							Else
								g.drawLine(right - 6, 2, 1, 2) ' top
								g.drawLine(1, 2, 1, bottom - 1) ' left
								g.drawLine(right - 2, bottom - 1, 1, bottom - 1) ' bottom
							End If
						End If
				End Select
				g.translate(-tabRect.x, -tabRect.y)
			End If
		End Sub

		Protected Friend Overrides Sub paintContentBorderTopEdge(ByVal g As Graphics, ByVal tabPlacement As Integer, ByVal selectedIndex As Integer, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			Dim leftToRight As Boolean = MetalUtils.isLeftToRight(tabPane)
			Dim right As Integer = x + w - 1
			Dim selRect As Rectangle = If(selectedIndex < 0, Nothing, getTabBounds(selectedIndex, calcRect))
			If ocean Then
				g.color = oceanSelectedBorderColor
			Else
				g.color = selectHighlight
			End If

			' Draw unbroken line if tabs are not on TOP, OR
			' selected tab is not in run adjacent to content, OR
			' selected tab is not visible (SCROLL_TAB_LAYOUT)
			'
			 If tabPlacement <> TOP OrElse selectedIndex < 0 OrElse (selRect.y + selRect.height + 1 < y) OrElse (selRect.x < x OrElse selRect.x > x + w) Then
				g.drawLine(x, y, x+w-2, y)
				If ocean AndAlso tabPlacement = TOP Then
					g.color = MetalLookAndFeel.white
					g.drawLine(x, y + 1, x+w-2, y + 1)
				End If
			Else
				' Break line to show visual connection to selected tab
				Dim ___lastInRun As Boolean = isLastInRun(selectedIndex)

				If leftToRight OrElse ___lastInRun Then
					g.drawLine(x, y, selRect.x + 1, y)
				Else
					g.drawLine(x, y, selRect.x, y)
				End If

				If selRect.x + selRect.width < right - 1 Then
					If leftToRight AndAlso (Not ___lastInRun) Then
						g.drawLine(selRect.x + selRect.width, y, right - 1, y)
					Else
						g.drawLine(selRect.x + selRect.width - 1, y, right - 1, y)
					End If
				Else
					g.color = shadow
					g.drawLine(x+w-2, y, x+w-2, y)
				End If

				If ocean Then
					g.color = MetalLookAndFeel.white

					If leftToRight OrElse ___lastInRun Then
						g.drawLine(x, y + 1, selRect.x + 1, y + 1)
					Else
						g.drawLine(x, y + 1, selRect.x, y + 1)
					End If

					If selRect.x + selRect.width < right - 1 Then
						If leftToRight AndAlso (Not ___lastInRun) Then
							g.drawLine(selRect.x + selRect.width, y + 1, right - 1, y + 1)
						Else
							g.drawLine(selRect.x + selRect.width - 1, y + 1, right - 1, y + 1)
						End If
					Else
						g.color = shadow
						g.drawLine(x+w-2, y + 1, x+w-2, y + 1)
					End If
				End If
			End If
		End Sub

		Protected Friend Overrides Sub paintContentBorderBottomEdge(ByVal g As Graphics, ByVal tabPlacement As Integer, ByVal selectedIndex As Integer, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			Dim leftToRight As Boolean = MetalUtils.isLeftToRight(tabPane)
			Dim bottom As Integer = y + h - 1
			Dim right As Integer = x + w - 1
			Dim selRect As Rectangle = If(selectedIndex < 0, Nothing, getTabBounds(selectedIndex, calcRect))

			g.color = darkShadow

			' Draw unbroken line if tabs are not on BOTTOM, OR
			' selected tab is not in run adjacent to content, OR
			' selected tab is not visible (SCROLL_TAB_LAYOUT)
			'
			If tabPlacement <> BOTTOM OrElse selectedIndex < 0 OrElse (selRect.y - 1 > h) OrElse (selRect.x < x OrElse selRect.x > x + w) Then
				If ocean AndAlso tabPlacement = BOTTOM Then g.color = oceanSelectedBorderColor
				g.drawLine(x, y+h-1, x+w-1, y+h-1)
			Else
				' Break line to show visual connection to selected tab
				Dim ___lastInRun As Boolean = isLastInRun(selectedIndex)

				If ocean Then g.color = oceanSelectedBorderColor

				If leftToRight OrElse ___lastInRun Then
					g.drawLine(x, bottom, selRect.x, bottom)
				Else
					g.drawLine(x, bottom, selRect.x - 1, bottom)
				End If

				If selRect.x + selRect.width < x + w - 2 Then
					If leftToRight AndAlso (Not ___lastInRun) Then
						g.drawLine(selRect.x + selRect.width, bottom, right, bottom)
					Else
						g.drawLine(selRect.x + selRect.width - 1, bottom, right, bottom)
					End If
				End If
			End If
		End Sub

		Protected Friend Overrides Sub paintContentBorderLeftEdge(ByVal g As Graphics, ByVal tabPlacement As Integer, ByVal selectedIndex As Integer, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			Dim selRect As Rectangle = If(selectedIndex < 0, Nothing, getTabBounds(selectedIndex, calcRect))
			If ocean Then
				g.color = oceanSelectedBorderColor
			Else
				g.color = selectHighlight
			End If

			' Draw unbroken line if tabs are not on LEFT, OR
			' selected tab is not in run adjacent to content, OR
			' selected tab is not visible (SCROLL_TAB_LAYOUT)
			'
			If tabPlacement <> LEFT OrElse selectedIndex < 0 OrElse (selRect.x + selRect.width + 1 < x) OrElse (selRect.y < y OrElse selRect.y > y + h) Then
				g.drawLine(x, y + 1, x, y+h-2)
				If ocean AndAlso tabPlacement = LEFT Then
					g.color = MetalLookAndFeel.white
					g.drawLine(x + 1, y, x + 1, y + h - 2)
				End If
			Else
				' Break line to show visual connection to selected tab
				g.drawLine(x, y, x, selRect.y + 1)
				If selRect.y + selRect.height < y + h - 2 Then g.drawLine(x, selRect.y + selRect.height + 1, x, y+h+2)
				If ocean Then
					g.color = MetalLookAndFeel.white
					g.drawLine(x + 1, y + 1, x + 1, selRect.y + 1)
					If selRect.y + selRect.height < y + h - 2 Then g.drawLine(x + 1, selRect.y + selRect.height + 1, x + 1, y+h+2)
				End If
			End If
		End Sub

		Protected Friend Overrides Sub paintContentBorderRightEdge(ByVal g As Graphics, ByVal tabPlacement As Integer, ByVal selectedIndex As Integer, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			Dim selRect As Rectangle = If(selectedIndex < 0, Nothing, getTabBounds(selectedIndex, calcRect))

			g.color = darkShadow
			' Draw unbroken line if tabs are not on RIGHT, OR
			' selected tab is not in run adjacent to content, OR
			' selected tab is not visible (SCROLL_TAB_LAYOUT)
			'
			If tabPlacement <> RIGHT OrElse selectedIndex < 0 OrElse (selRect.x - 1 > w) OrElse (selRect.y < y OrElse selRect.y > y + h) Then
				If ocean AndAlso tabPlacement = RIGHT Then g.color = oceanSelectedBorderColor
				g.drawLine(x+w-1, y, x+w-1, y+h-1)
			Else
				' Break line to show visual connection to selected tab
				If ocean Then g.color = oceanSelectedBorderColor
				g.drawLine(x+w-1, y, x+w-1, selRect.y)

				If selRect.y + selRect.height < y + h - 2 Then g.drawLine(x+w-1, selRect.y + selRect.height, x+w-1, y+h-2)
			End If
		End Sub

		Protected Friend Overrides Function calculateMaxTabHeight(ByVal tabPlacement As Integer) As Integer
			Dim metrics As FontMetrics = fontMetrics
			Dim height As Integer = metrics.height
			Dim tallerIcons As Boolean = False

			For i As Integer = 0 To tabPane.tabCount - 1
				Dim icon As Icon = tabPane.getIconAt(i)
				If icon IsNot Nothing Then
					If icon.iconHeight > height Then
						tallerIcons = True
						Exit For
					End If
				End If
			Next i
			Return MyBase.calculateMaxTabHeight(tabPlacement) - (If(tallerIcons, (tabInsets.top + tabInsets.bottom), 0))
		End Function


		Protected Friend Overrides Function getTabRunOverlay(ByVal tabPlacement As Integer) As Integer
			' Tab runs laid out vertically should overlap
			' at least as much as the largest slant
			If tabPlacement = LEFT OrElse tabPlacement = RIGHT Then
				Dim maxTabHeight As Integer = calculateMaxTabHeight(tabPlacement)
				Return maxTabHeight \ 2
			End If
			Return 0
		End Function

		' Don't rotate runs!
		Protected Friend Overridable Function shouldRotateTabRuns(ByVal tabPlacement As Integer, ByVal selectedRun As Integer) As Boolean
			Return False
		End Function

		' Don't pad last run
		Protected Friend Overrides Function shouldPadTabRun(ByVal tabPlacement As Integer, ByVal run As Integer) As Boolean
			Return runCount > 1 AndAlso run < runCount - 1
		End Function

		Private Function isLastInRun(ByVal tabIndex As Integer) As Boolean
			Dim run As Integer = getRunForTab(tabPane.tabCount, tabIndex)
			Dim lastIndex As Integer = lastTabInRun(tabPane.tabCount, run)
			Return tabIndex = lastIndex
		End Function

		''' <summary>
		''' Returns the color to use for the specified tab.
		''' </summary>
		Private Function getUnselectedBackgroundAt(ByVal index As Integer) As Color
			Dim color As Color = tabPane.getBackgroundAt(index)
			If TypeOf color Is UIResource Then
				If unselectedBackground IsNot Nothing Then Return unselectedBackground
			End If
			Return color
		End Function

		''' <summary>
		''' Returns the tab index of JTabbedPane the mouse is currently over
		''' </summary>
		Friend Overridable Property rolloverTabIndex As Integer
			Get
				Return rolloverTab
			End Get
		End Property

		''' <summary>
		''' This class should be treated as a &quot;protected&quot; inner class.
		''' Instantiate it only within subclasses of {@code MetalTabbedPaneUI}.
		''' </summary>
		Public Class TabbedPaneLayout
			Inherits javax.swing.plaf.basic.BasicTabbedPaneUI.TabbedPaneLayout

			Private ReadOnly outerInstance As MetalTabbedPaneUI


			Public Sub New(ByVal outerInstance As MetalTabbedPaneUI)
					Me.outerInstance = outerInstance
				outerInstance.super()
			End Sub

			Protected Friend Overrides Sub normalizeTabRuns(ByVal tabPlacement As Integer, ByVal tabCount As Integer, ByVal start As Integer, ByVal max As Integer)
				' Only normalize the runs for top & bottom;  normalizing
				' doesn't look right for Metal's vertical tabs
				' because the last run isn't padded and it looks odd to have
				' fat tabs in the first vertical runs, but slimmer ones in the
				' last (this effect isn't noticeable for horizontal tabs).
				If tabPlacement = TOP OrElse tabPlacement = BOTTOM Then MyBase.normalizeTabRuns(tabPlacement, tabCount, start, max)
			End Sub

			' Don't rotate runs!
			Protected Friend Overrides Sub rotateTabRuns(ByVal tabPlacement As Integer, ByVal selectedRun As Integer)
			End Sub

			' Don't pad selected tab
			Protected Friend Overrides Sub padSelectedTab(ByVal tabPlacement As Integer, ByVal selectedIndex As Integer)
			End Sub
		End Class

	End Class

End Namespace
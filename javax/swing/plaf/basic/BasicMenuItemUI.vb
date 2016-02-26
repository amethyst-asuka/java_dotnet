Imports System
Imports System.Threading
Imports javax.swing
Imports javax.swing.event
Imports javax.swing.plaf
Imports sun.swing

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
	''' BasicMenuItem implementation
	''' 
	''' @author Georges Saab
	''' @author David Karlton
	''' @author Arnaud Weber
	''' @author Fredrik Lagerblad
	''' </summary>
	Public Class BasicMenuItemUI
		Inherits MenuItemUI

		Protected Friend menuItem As JMenuItem = Nothing
		Protected Friend selectionBackground As Color
		Protected Friend selectionForeground As Color
		Protected Friend disabledForeground As Color
		Protected Friend acceleratorForeground As Color
		Protected Friend acceleratorSelectionForeground As Color

		''' <summary>
		''' Accelerator delimiter string, such as {@code '+'} in {@code 'Ctrl+C'}.
		''' @since 1.7
		''' </summary>
		Protected Friend acceleratorDelimiter As String

		Protected Friend defaultTextIconGap As Integer
		Protected Friend acceleratorFont As Font

		Protected Friend mouseInputListener As MouseInputListener
		Protected Friend menuDragMouseListener As MenuDragMouseListener
		Protected Friend menuKeyListener As MenuKeyListener
		''' <summary>
		''' <code>PropertyChangeListener</code> returned from
		''' <code>createPropertyChangeListener</code>. You should not
		''' need to access this field, rather if you want to customize the
		''' <code>PropertyChangeListener</code> override
		''' <code>createPropertyChangeListener</code>.
		''' 
		''' @since 1.6 </summary>
		''' <seealso cref= #createPropertyChangeListener </seealso>
		Protected Friend propertyChangeListener As java.beans.PropertyChangeListener
		' BasicMenuUI also uses this.
		Friend handler As Handler

		Protected Friend arrowIcon As Icon = Nothing
		Protected Friend checkIcon As Icon = Nothing

		Protected Friend oldBorderPainted As Boolean

		' diagnostic aids -- should be false for production builds. 
		Private Const TRACE As Boolean = False ' trace creates and disposes

		Private Const VERBOSE As Boolean = False ' show reuse hits/misses
		Private Const DEBUG As Boolean = False ' show bad params, misc.

		Friend Shared Sub loadActionMap(ByVal map As LazyActionMap)
			' NOTE: BasicMenuUI also calls into this method.
			map.put(New Actions(Actions.CLICK))
			BasicLookAndFeel.installAudioActionMap(map)
		End Sub

		Public Shared Function createUI(ByVal c As JComponent) As ComponentUI
			Return New BasicMenuItemUI
		End Function

		Public Overridable Sub installUI(ByVal c As JComponent)
			menuItem = CType(c, JMenuItem)

			installDefaults()
			installComponents(menuItem)
			installListeners()
			installKeyboardActions()
		End Sub


		Protected Friend Overridable Sub installDefaults()
			Dim prefix As String = propertyPrefix

			acceleratorFont = UIManager.getFont("MenuItem.acceleratorFont")
			' use default if missing so that BasicMenuItemUI can be used in other
			' LAFs like Nimbus
			If acceleratorFont Is Nothing Then acceleratorFont = UIManager.getFont("MenuItem.font")

			Dim opaque As Object = UIManager.get(propertyPrefix & ".opaque")
			If opaque IsNot Nothing Then
				LookAndFeel.installProperty(menuItem, "opaque", opaque)
			Else
				LookAndFeel.installProperty(menuItem, "opaque", Boolean.TRUE)
			End If
			If menuItem.margin Is Nothing OrElse (TypeOf menuItem.margin Is UIResource) Then menuItem.margin = UIManager.getInsets(prefix & ".margin")

			LookAndFeel.installProperty(menuItem, "iconTextGap", Convert.ToInt32(4))
			defaultTextIconGap = menuItem.iconTextGap

			LookAndFeel.installBorder(menuItem, prefix & ".border")
			oldBorderPainted = menuItem.borderPainted
			LookAndFeel.installProperty(menuItem, "borderPainted", UIManager.getBoolean(prefix & ".borderPainted"))
			LookAndFeel.installColorsAndFont(menuItem, prefix & ".background", prefix & ".foreground", prefix & ".font")

			' MenuItem specific defaults
			If selectionBackground Is Nothing OrElse TypeOf selectionBackground Is UIResource Then selectionBackground = UIManager.getColor(prefix & ".selectionBackground")
			If selectionForeground Is Nothing OrElse TypeOf selectionForeground Is UIResource Then selectionForeground = UIManager.getColor(prefix & ".selectionForeground")
			If disabledForeground Is Nothing OrElse TypeOf disabledForeground Is UIResource Then disabledForeground = UIManager.getColor(prefix & ".disabledForeground")
			If acceleratorForeground Is Nothing OrElse TypeOf acceleratorForeground Is UIResource Then acceleratorForeground = UIManager.getColor(prefix & ".acceleratorForeground")
			If acceleratorSelectionForeground Is Nothing OrElse TypeOf acceleratorSelectionForeground Is UIResource Then acceleratorSelectionForeground = UIManager.getColor(prefix & ".acceleratorSelectionForeground")
			' Get accelerator delimiter
			acceleratorDelimiter = UIManager.getString("MenuItem.acceleratorDelimiter")
			If acceleratorDelimiter Is Nothing Then acceleratorDelimiter = "+"
			' Icons
			If arrowIcon Is Nothing OrElse TypeOf arrowIcon Is UIResource Then arrowIcon = UIManager.getIcon(prefix & ".arrowIcon")
			If checkIcon Is Nothing OrElse TypeOf checkIcon Is UIResource Then
				checkIcon = UIManager.getIcon(prefix & ".checkIcon")
				'In case of column layout, .checkIconFactory is defined for this UI,
				'the icon is compatible with it and useCheckAndArrow() is true,
				'then the icon is handled by the checkIcon.
				Dim isColumnLayout As Boolean = MenuItemLayoutHelper.isColumnLayout(BasicGraphicsUtils.isLeftToRight(menuItem), menuItem)
				If isColumnLayout Then
					Dim iconFactory As MenuItemCheckIconFactory = CType(UIManager.get(prefix & ".checkIconFactory"), MenuItemCheckIconFactory)
					If iconFactory IsNot Nothing AndAlso MenuItemLayoutHelper.useCheckAndArrow(menuItem) AndAlso iconFactory.isCompatible(checkIcon, prefix) Then checkIcon = iconFactory.getIcon(menuItem)
				End If
			End If
		End Sub

		''' <summary>
		''' @since 1.3
		''' </summary>
		Protected Friend Overridable Sub installComponents(ByVal menuItem As JMenuItem)
			BasicHTML.updateRenderer(menuItem, menuItem.text)
		End Sub

		Protected Friend Overridable Property propertyPrefix As String
			Get
				Return "MenuItem"
			End Get
		End Property

		Protected Friend Overridable Sub installListeners()
			mouseInputListener = createMouseInputListener(menuItem)
			If mouseInputListener IsNot Nothing Then
				menuItem.addMouseListener(mouseInputListener)
				menuItem.addMouseMotionListener(mouseInputListener)
			End If
			menuDragMouseListener = createMenuDragMouseListener(menuItem)
			If menuDragMouseListener IsNot Nothing Then menuItem.addMenuDragMouseListener(menuDragMouseListener)
			menuKeyListener = createMenuKeyListener(menuItem)
			If menuKeyListener IsNot Nothing Then menuItem.addMenuKeyListener(menuKeyListener)
			propertyChangeListener = createPropertyChangeListener(menuItem)
			If propertyChangeListener IsNot Nothing Then menuItem.addPropertyChangeListener(propertyChangeListener)
		End Sub

		Protected Friend Overridable Sub installKeyboardActions()
			installLazyActionMap()
			updateAcceleratorBinding()
		End Sub

		Friend Overridable Sub installLazyActionMap()
			LazyActionMap.installLazyActionMap(menuItem, GetType(BasicMenuItemUI), propertyPrefix & ".actionMap")
		End Sub

		Public Overridable Sub uninstallUI(ByVal c As JComponent)
			menuItem = CType(c, JMenuItem)
			uninstallDefaults()
			uninstallComponents(menuItem)
			uninstallListeners()
			uninstallKeyboardActions()
			MenuItemLayoutHelper.clearUsedParentClientProperties(menuItem)
			menuItem = Nothing
		End Sub


		Protected Friend Overridable Sub uninstallDefaults()
			LookAndFeel.uninstallBorder(menuItem)
			LookAndFeel.installProperty(menuItem, "borderPainted", oldBorderPainted)
			If TypeOf menuItem.margin Is UIResource Then menuItem.margin = Nothing
			If TypeOf arrowIcon Is UIResource Then arrowIcon = Nothing
			If TypeOf checkIcon Is UIResource Then checkIcon = Nothing
		End Sub

		''' <summary>
		''' @since 1.3
		''' </summary>
		Protected Friend Overridable Sub uninstallComponents(ByVal menuItem As JMenuItem)
			BasicHTML.updateRenderer(menuItem, "")
		End Sub

		Protected Friend Overridable Sub uninstallListeners()
			If mouseInputListener IsNot Nothing Then
				menuItem.removeMouseListener(mouseInputListener)
				menuItem.removeMouseMotionListener(mouseInputListener)
			End If
			If menuDragMouseListener IsNot Nothing Then menuItem.removeMenuDragMouseListener(menuDragMouseListener)
			If menuKeyListener IsNot Nothing Then menuItem.removeMenuKeyListener(menuKeyListener)
			If propertyChangeListener IsNot Nothing Then menuItem.removePropertyChangeListener(propertyChangeListener)

			mouseInputListener = Nothing
			menuDragMouseListener = Nothing
			menuKeyListener = Nothing
			propertyChangeListener = Nothing
			handler = Nothing
		End Sub

		Protected Friend Overridable Sub uninstallKeyboardActions()
			SwingUtilities.replaceUIActionMap(menuItem, Nothing)
			SwingUtilities.replaceUIInputMap(menuItem, JComponent.WHEN_IN_FOCUSED_WINDOW, Nothing)
		End Sub

		Protected Friend Overridable Function createMouseInputListener(ByVal c As JComponent) As MouseInputListener
			Return handler
		End Function

		Protected Friend Overridable Function createMenuDragMouseListener(ByVal c As JComponent) As MenuDragMouseListener
			Return handler
		End Function

		Protected Friend Overridable Function createMenuKeyListener(ByVal c As JComponent) As MenuKeyListener
			Return Nothing
		End Function

		''' <summary>
		''' Creates a <code>PropertyChangeListener</code> which will be added to
		''' the menu item.
		''' If this method returns null then it will not be added to the menu item.
		''' </summary>
		''' <returns> an instance of a <code>PropertyChangeListener</code> or null
		''' @since 1.6 </returns>
		Protected Friend Overridable Function createPropertyChangeListener(ByVal c As JComponent) As java.beans.PropertyChangeListener
			Return handler
		End Function

		Friend Overridable Property handler As Handler
			Get
				If handler Is Nothing Then handler = New Handler(Me)
				Return handler
			End Get
		End Property

		Friend Overridable Function createInputMap(ByVal condition As Integer) As InputMap
			If condition = JComponent.WHEN_IN_FOCUSED_WINDOW Then Return New ComponentInputMapUIResource(menuItem)
			Return Nothing
		End Function

		Friend Overridable Sub updateAcceleratorBinding()
			Dim accelerator As KeyStroke = menuItem.accelerator
			Dim windowInputMap As InputMap = SwingUtilities.getUIInputMap(menuItem, JComponent.WHEN_IN_FOCUSED_WINDOW)

			If windowInputMap IsNot Nothing Then windowInputMap.clear()
			If accelerator IsNot Nothing Then
				If windowInputMap Is Nothing Then
					windowInputMap = createInputMap(JComponent.WHEN_IN_FOCUSED_WINDOW)
					SwingUtilities.replaceUIInputMap(menuItem, JComponent.WHEN_IN_FOCUSED_WINDOW, windowInputMap)
				End If
				windowInputMap.put(accelerator, "doClick")
			End If
		End Sub

		Public Overridable Function getMinimumSize(ByVal c As JComponent) As Dimension
			Dim d As Dimension = Nothing
			Dim v As javax.swing.text.View = CType(c.getClientProperty(BasicHTML.propertyKey), javax.swing.text.View)
			If v IsNot Nothing Then
				d = getPreferredSize(c)
				d.width -= v.getPreferredSpan(javax.swing.text.View.X_AXIS) - v.getMinimumSpan(javax.swing.text.View.X_AXIS)
			End If
			Return d
		End Function

		Public Overridable Function getPreferredSize(ByVal c As JComponent) As Dimension
			Return getPreferredMenuItemSize(c, checkIcon, arrowIcon, defaultTextIconGap)
		End Function

		Public Overridable Function getMaximumSize(ByVal c As JComponent) As Dimension
			Dim d As Dimension = Nothing
			Dim v As javax.swing.text.View = CType(c.getClientProperty(BasicHTML.propertyKey), javax.swing.text.View)
			If v IsNot Nothing Then
				d = getPreferredSize(c)
				d.width += v.getMaximumSpan(javax.swing.text.View.X_AXIS) - v.getPreferredSpan(javax.swing.text.View.X_AXIS)
			End If
			Return d
		End Function

		Protected Friend Overridable Function getPreferredMenuItemSize(ByVal c As JComponent, ByVal checkIcon As Icon, ByVal arrowIcon As Icon, ByVal defaultTextIconGap As Integer) As Dimension

			' The method also determines the preferred width of the
			' parent popup menu (through DefaultMenuLayout class).
			' The menu width equals to the maximal width
			' among child menu items.

			' Menu item width will be a sum of the widest check icon, label,
			' arrow icon and accelerator text among neighbor menu items.
			' For the latest menu item we will know the maximal widths exactly.
			' It will be the widest menu item and it will determine
			' the width of the parent popup menu.

			' !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
			' There is a conceptual problem: if user sets preferred size manually
			' for a menu item, this method won't be called for it
			' (see JComponent.getPreferredSize()),
			' maximal widths won't be calculated, other menu items won't be able
			' to take them into account and will be layouted in such a way,
			' as there is no the item with manual preferred size.
			' But after the first paint() method call, all maximal widths
			' will be correctly calculated and layout of some menu items
			' can be changed. For example, it can cause a shift of
			' the icon and text when user points a menu item by mouse.

			Dim mi As JMenuItem = CType(c, JMenuItem)
			Dim lh As New MenuItemLayoutHelper(mi, checkIcon, arrowIcon, MenuItemLayoutHelper.createMaxRect(), defaultTextIconGap, acceleratorDelimiter, BasicGraphicsUtils.isLeftToRight(mi), mi.font, acceleratorFont, MenuItemLayoutHelper.useCheckAndArrow(menuItem), propertyPrefix)

			Dim result As New Dimension

			' Calculate the result width
			result.width = lh.leadingGap
			MenuItemLayoutHelper.addMaxWidth(lh.checkSize, lh.afterCheckIconGap, result)
			' Take into account mimimal text offset.
			If ((Not lh.topLevelMenu)) AndAlso (lh.minTextOffset > 0) AndAlso (result.width < lh.minTextOffset) Then result.width = lh.minTextOffset
			MenuItemLayoutHelper.addMaxWidth(lh.labelSize, lh.gap, result)
			MenuItemLayoutHelper.addMaxWidth(lh.accSize, lh.gap, result)
			MenuItemLayoutHelper.addMaxWidth(lh.arrowSize, lh.gap, result)

			' Calculate the result height
			result.height = MenuItemLayoutHelper.max(lh.checkSize.height, lh.labelSize.height, lh.accSize.height, lh.arrowSize.height)

			' Take into account menu item insets
			Dim insets As Insets = lh.menuItem.insets
			If insets IsNot Nothing Then
				result.width += insets.left + insets.right
				result.height += insets.top + insets.bottom
			End If

			' if the width is even, bump it up one. This is critical
			' for the focus dash line to draw properly
			If result.width Mod 2 = 0 Then result.width += 1

			' if the height is even, bump it up one. This is critical
			' for the text to center properly
			If result.height Mod 2 = 0 AndAlso Boolean.TRUE IsNot UIManager.get(propertyPrefix & ".evenHeight") Then result.height += 1

			Return result
		End Function

		''' <summary>
		''' We draw the background in paintMenuItem()
		''' so override update (which fills the background of opaque
		''' components by default) to just call paint().
		''' 
		''' </summary>
		Public Overridable Sub update(ByVal g As Graphics, ByVal c As JComponent)
			paint(g, c)
		End Sub

		Public Overridable Sub paint(ByVal g As Graphics, ByVal c As JComponent)
			paintMenuItem(g, c, checkIcon, arrowIcon, selectionBackground, selectionForeground, defaultTextIconGap)
		End Sub

		Protected Friend Overridable Sub paintMenuItem(ByVal g As Graphics, ByVal c As JComponent, ByVal checkIcon As Icon, ByVal arrowIcon As Icon, ByVal background As Color, ByVal foreground As Color, ByVal defaultTextIconGap As Integer)
			' Save original graphics font and color
			Dim holdf As Font = g.font
			Dim holdc As Color = g.color

			Dim mi As JMenuItem = CType(c, JMenuItem)
			g.font = mi.font

			Dim viewRect As New Rectangle(0, 0, mi.width, mi.height)
			applyInsets(viewRect, mi.insets)

			Dim lh As New MenuItemLayoutHelper(mi, checkIcon, arrowIcon, viewRect, defaultTextIconGap, acceleratorDelimiter, BasicGraphicsUtils.isLeftToRight(mi), mi.font, acceleratorFont, MenuItemLayoutHelper.useCheckAndArrow(menuItem), propertyPrefix)
			Dim lr As MenuItemLayoutHelper.LayoutResult = lh.layoutMenuItem()

			paintBackground(g, mi, background)
			paintCheckIcon(g, lh, lr, holdc, foreground)
			paintIcon(g, lh, lr, holdc)
			paintText(g, lh, lr)
			paintAccText(g, lh, lr)
			paintArrowIcon(g, lh, lr, foreground)

			' Restore original graphics font and color
			g.color = holdc
			g.font = holdf
		End Sub

		Private Sub paintIcon(ByVal g As Graphics, ByVal lh As MenuItemLayoutHelper, ByVal lr As MenuItemLayoutHelper.LayoutResult, ByVal holdc As Color)
			If lh.icon IsNot Nothing Then
				Dim icon As Icon
				Dim model As ButtonModel = lh.menuItem.model
				If Not model.enabled Then
					icon = lh.menuItem.disabledIcon
				ElseIf model.pressed AndAlso model.armed Then
					icon = lh.menuItem.pressedIcon
					If icon Is Nothing Then icon = lh.menuItem.icon
				Else
					icon = lh.menuItem.icon
				End If

				If icon IsNot Nothing Then
					icon.paintIcon(lh.menuItem, g, lr.iconRect.x, lr.iconRect.y)
					g.color = holdc
				End If
			End If
		End Sub

		Private Sub paintCheckIcon(ByVal g As Graphics, ByVal lh As MenuItemLayoutHelper, ByVal lr As MenuItemLayoutHelper.LayoutResult, ByVal holdc As Color, ByVal foreground As Color)
			If lh.checkIcon IsNot Nothing Then
				Dim model As ButtonModel = lh.menuItem.model
				If model.armed OrElse (TypeOf lh.menuItem Is JMenu AndAlso model.selected) Then
					g.color = foreground
				Else
					g.color = holdc
				End If
				If lh.useCheckAndArrow() Then lh.checkIcon.paintIcon(lh.menuItem, g, lr.checkRect.x, lr.checkRect.y)
				g.color = holdc
			End If
		End Sub

		Private Sub paintAccText(ByVal g As Graphics, ByVal lh As MenuItemLayoutHelper, ByVal lr As MenuItemLayoutHelper.LayoutResult)
			If Not lh.accText.Equals("") Then
				Dim model As ButtonModel = lh.menuItem.model
				g.font = lh.accFontMetrics.font
				If Not model.enabled Then
					' *** paint the accText disabled
					If disabledForeground IsNot Nothing Then
						g.color = disabledForeground
						SwingUtilities2.drawString(lh.menuItem, g, lh.accText, lr.accRect.x, lr.accRect.y + lh.accFontMetrics.ascent)
					Else
						g.color = lh.menuItem.background.brighter()
						SwingUtilities2.drawString(lh.menuItem, g, lh.accText, lr.accRect.x, lr.accRect.y + lh.accFontMetrics.ascent)
						g.color = lh.menuItem.background.darker()
						SwingUtilities2.drawString(lh.menuItem, g, lh.accText, lr.accRect.x - 1, lr.accRect.y + lh.fontMetrics.ascent - 1)
					End If
				Else
					' *** paint the accText normally
					If model.armed OrElse (TypeOf lh.menuItem Is JMenu AndAlso model.selected) Then
						g.color = acceleratorSelectionForeground
					Else
						g.color = acceleratorForeground
					End If
					SwingUtilities2.drawString(lh.menuItem, g, lh.accText, lr.accRect.x, lr.accRect.y + lh.accFontMetrics.ascent)
				End If
			End If
		End Sub

		Private Sub paintText(ByVal g As Graphics, ByVal lh As MenuItemLayoutHelper, ByVal lr As MenuItemLayoutHelper.LayoutResult)
			If Not lh.text.Equals("") Then
				If lh.htmlView IsNot Nothing Then
					' Text is HTML
					lh.htmlView.paint(g, lr.textRect)
				Else
					' Text isn't HTML
					paintText(g, lh.menuItem, lr.textRect, lh.text)
				End If
			End If
		End Sub

		Private Sub paintArrowIcon(ByVal g As Graphics, ByVal lh As MenuItemLayoutHelper, ByVal lr As MenuItemLayoutHelper.LayoutResult, ByVal foreground As Color)
			If lh.arrowIcon IsNot Nothing Then
				Dim model As ButtonModel = lh.menuItem.model
				If model.armed OrElse (TypeOf lh.menuItem Is JMenu AndAlso model.selected) Then g.color = foreground
				If lh.useCheckAndArrow() Then lh.arrowIcon.paintIcon(lh.menuItem, g, lr.arrowRect.x, lr.arrowRect.y)
			End If
		End Sub

		Private Sub applyInsets(ByVal rect As Rectangle, ByVal insets As Insets)
			If insets IsNot Nothing Then
				rect.x += insets.left
				rect.y += insets.top
				rect.width -= (insets.right + rect.x)
				rect.height -= (insets.bottom + rect.y)
			End If
		End Sub

		''' <summary>
		''' Draws the background of the menu item.
		''' </summary>
		''' <param name="g"> the paint graphics </param>
		''' <param name="menuItem"> menu item to be painted </param>
		''' <param name="bgColor"> selection background color
		''' @since 1.4 </param>
		Protected Friend Overridable Sub paintBackground(ByVal g As Graphics, ByVal menuItem As JMenuItem, ByVal bgColor As Color)
			Dim model As ButtonModel = menuItem.model
			Dim oldColor As Color = g.color
			Dim menuWidth As Integer = menuItem.width
			Dim menuHeight As Integer = menuItem.height

			If menuItem.opaque Then
				If model.armed OrElse (TypeOf menuItem Is JMenu AndAlso model.selected) Then
					g.color = bgColor
					g.fillRect(0,0, menuWidth, menuHeight)
				Else
					g.color = menuItem.background
					g.fillRect(0,0, menuWidth, menuHeight)
				End If
				g.color = oldColor
			ElseIf model.armed OrElse (TypeOf menuItem Is JMenu AndAlso model.selected) Then
				g.color = bgColor
				g.fillRect(0,0, menuWidth, menuHeight)
				g.color = oldColor
			End If
		End Sub

		''' <summary>
		''' Renders the text of the current menu item.
		''' <p> </summary>
		''' <param name="g"> graphics context </param>
		''' <param name="menuItem"> menu item to render </param>
		''' <param name="textRect"> bounding rectangle for rendering the text </param>
		''' <param name="text"> string to render
		''' @since 1.4 </param>
		Protected Friend Overridable Sub paintText(ByVal g As Graphics, ByVal menuItem As JMenuItem, ByVal textRect As Rectangle, ByVal text As String)
			Dim model As ButtonModel = menuItem.model
			Dim fm As FontMetrics = SwingUtilities2.getFontMetrics(menuItem, g)
			Dim mnemIndex As Integer = menuItem.displayedMnemonicIndex

			If Not model.enabled Then
				' *** paint the text disabled
				If TypeOf UIManager.get("MenuItem.disabledForeground") Is Color Then
					g.color = UIManager.getColor("MenuItem.disabledForeground")
					SwingUtilities2.drawStringUnderlineCharAt(menuItem, g,text, mnemIndex, textRect.x, textRect.y + fm.ascent)
				Else
					g.color = menuItem.background.brighter()
					SwingUtilities2.drawStringUnderlineCharAt(menuItem, g, text, mnemIndex, textRect.x, textRect.y + fm.ascent)
					g.color = menuItem.background.darker()
					SwingUtilities2.drawStringUnderlineCharAt(menuItem, g,text, mnemIndex, textRect.x - 1, textRect.y + fm.ascent - 1)
				End If
			Else
				' *** paint the text normally
				If model.armed OrElse (TypeOf menuItem Is JMenu AndAlso model.selected) Then g.color = selectionForeground ' Uses protected field.
				SwingUtilities2.drawStringUnderlineCharAt(menuItem, g,text, mnemIndex, textRect.x, textRect.y + fm.ascent)
			End If
		End Sub

		Public Overridable Property path As MenuElement()
			Get
				Dim m As MenuSelectionManager = MenuSelectionManager.defaultManager()
				Dim oldPath As MenuElement() = m.selectedPath
				Dim newPath As MenuElement()
				Dim i As Integer = oldPath.Length
				If i = 0 Then Return New MenuElement(){}
				Dim parent As Component = menuItem.parent
				If oldPath(i-1).component Is parent Then
					' The parent popup menu is the last so far
					newPath = New MenuElement(i){}
					Array.Copy(oldPath, 0, newPath, 0, i)
					newPath(i) = menuItem
				Else
					' A sibling menuitem is the current selection
					'
					'  This probably needs to handle 'exit submenu into
					' a menu item.  Search backwards along the current
					' selection until you find the parent popup menu,
					' then copy up to that and add yourself...
					Dim j As Integer
					For j = oldPath.Length-1 To 0 Step -1
						If oldPath(j).component Is parent Then Exit For
					Next j
					newPath = New MenuElement(j+2 - 1){}
					Array.Copy(oldPath, 0, newPath, 0, j+1)
					newPath(j+1) = menuItem
		'            
		'            System.out.println("Sibling condition -- ");
		'            System.out.println("Old array : ");
		'            printMenuElementArray(oldPath, false);
		'            System.out.println("New array : ");
		'            printMenuElementArray(newPath, false);
		'            
				End If
				Return newPath
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
		void printMenuElementArray(MenuElement path() , boolean dumpStack)
			Console.WriteLine("Path is(")
			Dim i, j As Integer
			i=0
			j=path.length
			Do While i<j
				For k As Integer = 0 To i
					Console.Write("  ")
				Next k
				Dim [me] As MenuElement = path(i)
				If TypeOf [me] Is JMenuItem Then
					Console.WriteLine(CType([me], JMenuItem).text & ", ")
				ElseIf [me] Is Nothing Then
					Console.WriteLine("NULL , ")
				Else
					Console.WriteLine("" & [me] & ", ")
				End If
				i += 1
			Loop
			Console.WriteLine(")")

			If dumpStack = True Then Thread.dumpStack()
		protected class MouseInputHandler implements MouseInputListener
			' NOTE: This class exists only for backward compatibility. All
			' its functionality has been moved into Handler. If you need to add
			' new functionality add it to the Handler, but make sure this
			' class calls into the Handler.

			public void mouseClicked(MouseEvent e)
				handler.mouseClicked(e)
			public void mousePressed(MouseEvent e)
				handler.mousePressed(e)
			public void mouseReleased(MouseEvent e)
				handler.mouseReleased(e)
			public void mouseEntered(MouseEvent e)
				handler.mouseEntered(e)
			public void mouseExited(MouseEvent e)
				handler.mouseExited(e)
			public void mouseDragged(MouseEvent e)
				handler.mouseDragged(e)
			public void mouseMoved(MouseEvent e)
				handler.mouseMoved(e)


		private static class Actions extends UIAction
			private static final String CLICK = "doClick"

			Actions(String key)
				MyBase(key)

			public void actionPerformed(ActionEvent e)
				Dim mi As JMenuItem = CType(e.source, JMenuItem)
				MenuSelectionManager.defaultManager().clearSelectedPath()
				mi.doClick()

		''' <summary>
		''' Call this method when a menu item is to be activated.
		''' This method handles some of the details of menu item activation
		''' such as clearing the selected path and messaging the
		''' JMenuItem's doClick() method.
		''' </summary>
		''' <param name="msm">  A MenuSelectionManager. The visual feedback and
		'''             internal bookkeeping tasks are delegated to
		'''             this MenuSelectionManager. If <code>null</code> is
		'''             passed as this argument, the
		'''             <code>MenuSelectionManager.defaultManager</code> is
		'''             used. </param>
		''' <seealso cref= MenuSelectionManager </seealso>
		''' <seealso cref= JMenuItem#doClick(int)
		''' @since 1.4 </seealso>
		protected void doClick(MenuSelectionManager msm)
			' Auditory cue
			If Not internalFrameSystemMenu Then BasicLookAndFeel.playSound(menuItem, propertyPrefix & ".commandSound")
			' Visual feedback
			If msm Is Nothing Then msm = MenuSelectionManager.defaultManager()
			msm.clearSelectedPath()
			menuItem.doClick(0)

		''' <summary>
		''' This is to see if the menu item in question is part of the
		''' system menu on an internal frame.
		''' The Strings that are being checked can be found in
		''' MetalInternalFrameTitlePaneUI.java,
		''' WindowsInternalFrameTitlePaneUI.java, and
		''' MotifInternalFrameTitlePaneUI.java.
		''' 
		''' @since 1.4
		''' </summary>
		private Boolean internalFrameSystemMenu
			Dim actionCommand As String = menuItem.actionCommand
			If (actionCommand = "Close") OrElse (actionCommand = "Minimize") OrElse (actionCommand = "Restore") OrElse (actionCommand = "Maximize") Then
			  Return True
			Else
			  Return False
			End If


		' BasicMenuUI subclasses this.
'JAVA TO VB CONVERTER TODO TASK: Local classes are not converted by Java to VB Converter:
'		class Handler implements MenuDragMouseListener, MouseInputListener, java.beans.PropertyChangeListener
	'	{
	'		'
	'		' MouseInputListener
	'		'
	'		public void mouseClicked(MouseEvent e)
	'		{
	'		}
	'		public void mousePressed(MouseEvent e)
	'		{
	'		}
	'		public void mouseReleased(MouseEvent e)
	'		{
	'			if (!menuItem.isEnabled())
	'			{
	'				Return;
	'			}
	'			MenuSelectionManager manager = MenuSelectionManager.defaultManager();
	'			Point p = e.getPoint();
	'			if(p.x >= 0 && p.x < menuItem.getWidth() && p.y >= 0 && p.y < menuItem.getHeight())
	'			{
	'				doClick(manager);
	'			}
	'			else
	'			{
	'				manager.processMouseEvent(e);
	'			}
	'		}
	'		public void mouseEntered(MouseEvent e)
	'		{
	'			MenuSelectionManager manager = MenuSelectionManager.defaultManager();
	'			int modifiers = e.getModifiers();
	'			' 4188027: drag enter/exit added in JDK 1.1.7A, JDK1.2
	'			if ((modifiers & (InputEvent.BUTTON1_MASK | InputEvent.BUTTON2_MASK | InputEvent.BUTTON3_MASK)) !=0)
	'			{
	'				MenuSelectionManager.defaultManager().processMouseEvent(e);
	'			}
	'			else
	'			{
	'			manager.setSelectedPath(getPath());
	'			 }
	'		}
	'		public void mouseExited(MouseEvent e)
	'		{
	'			MenuSelectionManager manager = MenuSelectionManager.defaultManager();
	'
	'			int modifiers = e.getModifiers();
	'			' 4188027: drag enter/exit added in JDK 1.1.7A, JDK1.2
	'			if ((modifiers & (InputEvent.BUTTON1_MASK | InputEvent.BUTTON2_MASK | InputEvent.BUTTON3_MASK)) !=0)
	'			{
	'				MenuSelectionManager.defaultManager().processMouseEvent(e);
	'			}
	'			else
	'			{
	'
	'				MenuElement path[] = manager.getSelectedPath();
	'				if (path.length > 1 && path[path.length-1] == menuItem)
	'				{
	'					MenuElement newPath[] = New MenuElement[path.length-1];
	'					int i,c;
	'					for(i=0,c=path.length-1;i<c;i += 1)
	'						newPath[i] = path[i];
	'					manager.setSelectedPath(newPath);
	'				}
	'				}
	'		}
	'
	'		public void mouseDragged(MouseEvent e)
	'		{
	'			MenuSelectionManager.defaultManager().processMouseEvent(e);
	'		}
	'		public void mouseMoved(MouseEvent e)
	'		{
	'		}
	'
	'		'
	'		' MenuDragListener
	'		'
	'		public void menuDragMouseEntered(MenuDragMouseEvent e)
	'		{
	'			MenuSelectionManager manager = e.getMenuSelectionManager();
	'			MenuElement path[] = e.getPath();
	'			manager.setSelectedPath(path);
	'		}
	'		public void menuDragMouseDragged(MenuDragMouseEvent e)
	'		{
	'			MenuSelectionManager manager = e.getMenuSelectionManager();
	'			MenuElement path[] = e.getPath();
	'			manager.setSelectedPath(path);
	'		}
	'		public void menuDragMouseExited(MenuDragMouseEvent e)
	'		{
	'		}
	'		public void menuDragMouseReleased(MenuDragMouseEvent e)
	'		{
	'			if (!menuItem.isEnabled())
	'			{
	'				Return;
	'			}
	'			MenuSelectionManager manager = e.getMenuSelectionManager();
	'			MenuElement path[] = e.getPath();
	'			Point p = e.getPoint();
	'			if (p.x >= 0 && p.x < menuItem.getWidth() && p.y >= 0 && p.y < menuItem.getHeight())
	'			{
	'				doClick(manager);
	'			}
	'			else
	'			{
	'				manager.clearSelectedPath();
	'			}
	'		}
	'
	'
	'		'
	'		' PropertyChangeListener
	'		'
	'		public void propertyChange(PropertyChangeEvent e)
	'		{
	'			String name = e.getPropertyName();
	'
	'			if (name == "labelFor" || name == "displayedMnemonic" || name == "accelerator")
	'			{
	'				updateAcceleratorBinding();
	'			}
	'			else if (name == "text" || "font" == name || "foreground" == name)
	'			{
	'				' remove the old html view client property if one
	'				' existed, and install a new one if the text installed
	'				' into the JLabel is html source.
	'				JMenuItem lbl = ((JMenuItem) e.getSource());
	'				String text = lbl.getText();
	'				BasicHTML.updateRenderer(lbl, text);
	'			}
	'			else if (name == "iconTextGap")
	'			{
	'				defaultTextIconGap = ((Number)e.getNewValue()).intValue();
	'			}
	'		}
	'	}
	End Class

End Namespace
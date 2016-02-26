Imports System.Collections
Imports javax.swing
Imports javax.swing.plaf

'
' * Copyright (c) 2003, 2006, Oracle and/or its affiliates. All rights reserved.
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
	''' The default theme for the {@code MetalLookAndFeel}.
	''' <p>
	''' The designers
	''' of the Metal Look and Feel strive to keep the default look up to
	''' date, possibly through the use of new themes in the future.
	''' Therefore, developers should only use this class directly when they
	''' wish to customize the "Ocean" look, or force it to be the current
	''' theme, regardless of future updates.
	''' 
	''' <p>
	''' All colors returned by {@code OceanTheme} are completely
	''' opaque.
	''' 
	''' @since 1.5 </summary>
	''' <seealso cref= MetalLookAndFeel#setCurrentTheme </seealso>
	Public Class OceanTheme
		Inherits DefaultMetalTheme

		Private Shared ReadOnly PRIMARY1 As New ColorUIResource(&H6382BF)
		Private Shared ReadOnly PRIMARY2 As New ColorUIResource(&HA3B8CC)
		Private Shared ReadOnly PRIMARY3 As New ColorUIResource(&HB8CFE5)
		Private Shared ReadOnly SECONDARY1 As New ColorUIResource(&H7A8A99)
		Private Shared ReadOnly SECONDARY2 As New ColorUIResource(&HB8CFE5)
		Private Shared ReadOnly SECONDARY3 As New ColorUIResource(&HEEEEEE)

		Private Shared ReadOnly CONTROL_TEXT_COLOR As ColorUIResource = New sun.swing.PrintColorUIResource(&H333333, Color.BLACK)
		Private Shared ReadOnly INACTIVE_CONTROL_TEXT_COLOR As New ColorUIResource(&H999999)
		Private Shared ReadOnly MENU_DISABLED_FOREGROUND As New ColorUIResource(&H999999)
		Private Shared ReadOnly OCEAN_BLACK As ColorUIResource = New sun.swing.PrintColorUIResource(&H333333, Color.BLACK)

		Private Shared ReadOnly OCEAN_DROP As New ColorUIResource(&HD2E9FF)

		' ComponentOrientation Icon
		' Delegates to different icons based on component orientation
		Private Class COIcon
			Inherits IconUIResource

			Private rtl As Icon

			Public Sub New(ByVal ltr As Icon, ByVal rtl As Icon)
				MyBase.New(ltr)
				Me.rtl = rtl
			End Sub

			Public Overridable Sub paintIcon(ByVal c As Component, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer)
				If MetalUtils.isLeftToRight(c) Then
					MyBase.paintIcon(c, g, x, y)
				Else
					rtl.paintIcon(c, g, x, y)
				End If
			End Sub
		End Class

		' InternalFrame Icon
		' Delegates to different icons based on button state
		Private Class IFIcon
			Inherits IconUIResource

			Private pressed As Icon

			Public Sub New(ByVal normal As Icon, ByVal pressed As Icon)
				MyBase.New(normal)
				Me.pressed = pressed
			End Sub

			Public Overridable Sub paintIcon(ByVal c As Component, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer)
				Dim model As ButtonModel = CType(c, AbstractButton).model
				If model.pressed AndAlso model.armed Then
					pressed.paintIcon(c, g, x, y)
				Else
					MyBase.paintIcon(c, g, x, y)
				End If
			End Sub
		End Class

		''' <summary>
		''' Creates an instance of <code>OceanTheme</code>
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Add this theme's custom entries to the defaults table.
		''' </summary>
		''' <param name="table"> the defaults table, non-null </param>
		''' <exception cref="NullPointerException"> if {@code table} is {@code null} </exception>
		Public Overrides Sub addCustomEntriesToTable(ByVal table As UIDefaults)
			Dim focusBorder As Object = New sun.swing.SwingLazyValue("javax.swing.plaf.BorderUIResource$LineBorderUIResource", New Object() {primary1})
			' .30 0 DDE8F3 white secondary2
			Dim buttonGradient As IList = New Object() {New Single?(.3f), New Single?(0f), New ColorUIResource(&HDDE8F3), white, secondary2 }

			' Other possible properties that aren't defined:
			'
			' Used when generating the disabled Icons, provides the region to
			' constrain grays to.
			' Button.disabledGrayRange -> Object[] of Integers giving min/max
			' InternalFrame.inactiveTitleGradient -> Gradient when the
			'   internal frame is inactive.
			Dim cccccc As Color = New ColorUIResource(&HCCCCCC)
			Dim dadada As Color = New ColorUIResource(&HDADADA)
			Dim c8ddf2 As Color = New ColorUIResource(&HC8RDF2)
			Dim directoryIcon As Object = getIconResource("icons/ocean/directory.gif")
			Dim fileIcon As Object = getIconResource("icons/ocean/file.gif")
			Dim sliderGradient As IList = New Object() { New Single?(.3f), New Single?(.2f), c8ddf2, white, New ColorUIResource(SECONDARY2) }

			Dim defaults As Object() = { "Button.gradient", buttonGradient, "Button.rollover", Boolean.TRUE, "Button.toolBarBorderBackground", INACTIVE_CONTROL_TEXT_COLOR, "Button.disabledToolBarBorderBackground", cccccc, "Button.rolloverIconType", "ocean", "CheckBox.rollover", Boolean.TRUE, "CheckBox.gradient", buttonGradient, "CheckBoxMenuItem.gradient", buttonGradient, "FileChooser.homeFolderIcon", getIconResource("icons/ocean/homeFolder.gif"), "FileChooser.newFolderIcon", getIconResource("icons/ocean/newFolder.gif"), "FileChooser.upFolderIcon", getIconResource("icons/ocean/upFolder.gif"), "FileView.computerIcon", getIconResource("icons/ocean/computer.gif"), "FileView.directoryIcon", directoryIcon, "FileView.hardDriveIcon", getIconResource("icons/ocean/hardDrive.gif"), "FileView.fileIcon", fileIcon, "FileView.floppyDriveIcon", getIconResource("icons/ocean/floppy.gif"), "Label.disabledForeground", inactiveControlTextColor, "Menu.opaque", Boolean.FALSE, "MenuBar.gradient", New Object() { New Single?(1f), New Single?(0f), white, dadada, New ColorUIResource(dadada) }, "MenuBar.borderColor", cccccc, "InternalFrame.activeTitleGradient", buttonGradient, "InternalFrame.closeIcon", New UIDefaults.LazyValue() { public Object createValue(UIDefaults table) { Return New IFIcon(getHastenedIcon("icons/ocean/close.gif", table), getHastenedIcon("icons/ocean/close-pressed.gif", table)); } }, "InternalFrame.iconifyIcon", New UIDefaults.LazyValue() { public Object createValue(UIDefaults table) { Return New IFIcon(getHastenedIcon("icons/ocean/iconify.gif", table), getHastenedIcon("icons/ocean/iconify-pressed.gif", table)); } }, "InternalFrame.minimizeIcon", New UIDefaults.LazyValue() { public Object createValue(UIDefaults table) { Return New IFIcon(getHastenedIcon("icons/ocean/minimize.gif", table), getHastenedIcon("icons/ocean/minimize-pressed.gif", table)); } }, "InternalFrame.icon", getIconResource("icons/ocean/menu.gif"), "InternalFrame.maximizeIcon", New UIDefaults.LazyValue() { public Object createValue(UIDefaults table) { Return New IFIcon(getHastenedIcon("icons/ocean/maximize.gif", table), getHastenedIcon("icons/ocean/maximize-pressed.gif", table)); } }, "InternalFrame.paletteCloseIcon", New UIDefaults.LazyValue() { public Object createValue(UIDefaults table) { Return New IFIcon(getHastenedIcon("icons/ocean/paletteClose.gif", table), getHastenedIcon("icons/ocean/paletteClose-pressed.gif", table)); } }, "List.focusCellHighlightBorder", focusBorder, "MenuBarUI", "javax.swing.plaf.metal.MetalMenuBarUI", "OptionPane.errorIcon", getIconResource("icons/ocean/error.png"), "OptionPane.informationIcon", getIconResource("icons/ocean/info.png"), "OptionPane.questionIcon", getIconResource("icons/ocean/question.png"), "OptionPane.warningIcon", getIconResource("icons/ocean/warning.png"), "RadioButton.gradient", buttonGradient, "RadioButton.rollover", Boolean.TRUE, "RadioButtonMenuItem.gradient", buttonGradient, "ScrollBar.gradient", buttonGradient, "Slider.altTrackColor", New ColorUIResource(&HD2E2EF), "Slider.gradient", sliderGradient, "Slider.focusGradient", sliderGradient, "SplitPane.oneTouchButtonsOpaque", Boolean.FALSE, "SplitPane.dividerFocusColor", c8ddf2, "TabbedPane.borderHightlightColor", primary1, "TabbedPane.contentAreaColor", c8ddf2, "TabbedPane.contentBorderInsets", New Insets(4, 2, 3, 3), "TabbedPane.selected", c8ddf2, "TabbedPane.tabAreaBackground", dadada, "TabbedPane.tabAreaInsets", New Insets(2, 2, 0, 6), "TabbedPane.unselectedBackground", SECONDARY3, "Table.focusCellHighlightBorder", focusBorder, "Table.gridColor", SECONDARY1, "TableHeader.focusCellBackground", c8ddf2, "ToggleButton.gradient", buttonGradient, "ToolBar.borderColor", cccccc, "ToolBar.isRollover", Boolean.TRUE, "Tree.closedIcon", directoryIcon, "Tree.collapsedIcon", New UIDefaults.LazyValue() { public Object createValue(UIDefaults table) { Return New COIcon(getHastenedIcon("icons/ocean/collapsed.gif", table), getHastenedIcon("icons/ocean/collapsed-rtl.gif", table)); } }, "Tree.expandedIcon", getIconResource("icons/ocean/expanded.gif"), "Tree.leafIcon", fileIcon, "Tree.openIcon", directoryIcon, "Tree.selectionBorderColor", primary1, "Tree.dropLineColor", primary1, "Table.dropLineColor", primary1, "Table.dropLineShortColor", OCEAN_BLACK, "Table.dropCellBackground", OCEAN_DROP, "Tree.dropCellBackground", OCEAN_DROP, "List.dropCellBackground", OCEAN_DROP, "List.dropLineColor", primary1 }
			table.putDefaults(defaults)
		End Sub

		''' <summary>
		''' Overriden to enable picking up the system fonts, if applicable.
		''' </summary>
		Friend Property Overrides systemTheme As Boolean
			Get
				Return True
			End Get
		End Property

		''' <summary>
		''' Return the name of this theme, "Ocean".
		''' </summary>
		''' <returns> "Ocean" </returns>
		Public Property Overrides name As String
			Get
				Return "Ocean"
			End Get
		End Property

		''' <summary>
		''' Returns the primary 1 color. This returns a color with an rgb hex value
		''' of {@code 0x6382BF}.
		''' </summary>
		''' <returns> the primary 1 color </returns>
		''' <seealso cref= java.awt.Color#decode </seealso>
		Protected Friend Property Overrides primary1 As ColorUIResource
			Get
				Return PRIMARY1
			End Get
		End Property

		''' <summary>
		''' Returns the primary 2 color. This returns a color with an rgb hex value
		''' of {@code 0xA3B8CC}.
		''' </summary>
		''' <returns> the primary 2 color </returns>
		''' <seealso cref= java.awt.Color#decode </seealso>
		Protected Friend Property Overrides primary2 As ColorUIResource
			Get
				Return PRIMARY2
			End Get
		End Property

		''' <summary>
		''' Returns the primary 3 color. This returns a color with an rgb hex value
		''' of {@code 0xB8CFE5}.
		''' </summary>
		''' <returns> the primary 3 color </returns>
		''' <seealso cref= java.awt.Color#decode </seealso>
		Protected Friend Property Overrides primary3 As ColorUIResource
			Get
				Return PRIMARY3
			End Get
		End Property

		''' <summary>
		''' Returns the secondary 1 color. This returns a color with an rgb hex
		''' value of {@code 0x7A8A99}.
		''' </summary>
		''' <returns> the secondary 1 color </returns>
		''' <seealso cref= java.awt.Color#decode </seealso>
		Protected Friend Property Overrides secondary1 As ColorUIResource
			Get
				Return SECONDARY1
			End Get
		End Property

		''' <summary>
		''' Returns the secondary 2 color. This returns a color with an rgb hex
		''' value of {@code 0xB8CFE5}.
		''' </summary>
		''' <returns> the secondary 2 color </returns>
		''' <seealso cref= java.awt.Color#decode </seealso>
		Protected Friend Property Overrides secondary2 As ColorUIResource
			Get
				Return SECONDARY2
			End Get
		End Property

		''' <summary>
		''' Returns the secondary 3 color. This returns a color with an rgb hex
		''' value of {@code 0xEEEEEE}.
		''' </summary>
		''' <returns> the secondary 3 color </returns>
		''' <seealso cref= java.awt.Color#decode </seealso>
		Protected Friend Property Overrides secondary3 As ColorUIResource
			Get
				Return SECONDARY3
			End Get
		End Property

		''' <summary>
		''' Returns the black color. This returns a color with an rgb hex
		''' value of {@code 0x333333}.
		''' </summary>
		''' <returns> the black color </returns>
		''' <seealso cref= java.awt.Color#decode </seealso>
		Protected Friend Property Overrides black As ColorUIResource
			Get
				Return OCEAN_BLACK
			End Get
		End Property

		''' <summary>
		''' Returns the desktop color. This returns a color with an rgb hex
		''' value of {@code 0xFFFFFF}.
		''' </summary>
		''' <returns> the desktop color </returns>
		''' <seealso cref= java.awt.Color#decode </seealso>
		Public Property Overrides desktopColor As ColorUIResource
			Get
				Return MetalTheme.white
			End Get
		End Property

		''' <summary>
		''' Returns the inactive control text color. This returns a color with an
		''' rgb hex value of {@code 0x999999}.
		''' </summary>
		''' <returns> the inactive control text color </returns>
		Public Property Overrides inactiveControlTextColor As ColorUIResource
			Get
				Return INACTIVE_CONTROL_TEXT_COLOR
			End Get
		End Property

		''' <summary>
		''' Returns the control text color. This returns a color with an
		''' rgb hex value of {@code 0x333333}.
		''' </summary>
		''' <returns> the control text color </returns>
		Public Property Overrides controlTextColor As ColorUIResource
			Get
				Return CONTROL_TEXT_COLOR
			End Get
		End Property

		''' <summary>
		''' Returns the menu disabled foreground color. This returns a color with an
		''' rgb hex value of {@code 0x999999}.
		''' </summary>
		''' <returns> the menu disabled foreground color </returns>
		Public Property Overrides menuDisabledForeground As ColorUIResource
			Get
				Return MENU_DISABLED_FOREGROUND
			End Get
		End Property

		Private Function getIconResource(ByVal iconID As String) As Object
			Return sun.swing.SwingUtilities2.makeIcon(Me.GetType(), GetType(OceanTheme), iconID)
		End Function

		' makes use of getIconResource() to fetch an icon and then hastens it
		' - calls createValue() on it and returns the actual icon
		Private Function getHastenedIcon(ByVal iconID As String, ByVal table As UIDefaults) As Icon
			Dim res As Object = getIconResource(iconID)
			Return CType(CType(res, UIDefaults.LazyValue).createValue(table), Icon)
		End Function
	End Class

End Namespace
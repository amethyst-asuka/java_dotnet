Imports System
Imports System.Runtime.CompilerServices
Imports javax.swing.plaf
Imports javax.swing
Imports javax.swing.plaf.basic
Imports sun.awt

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
	''' The Java Look and Feel, otherwise known as Metal.
	''' <p>
	''' Each of the {@code ComponentUI}s provided by {@code
	''' MetalLookAndFeel} derives its behavior from the defaults
	''' table. Unless otherwise noted each of the {@code ComponentUI}
	''' implementations in this package document the set of defaults they
	''' use. Unless otherwise noted the defaults are installed at the time
	''' {@code installUI} is invoked, and follow the recommendations
	''' outlined in {@code LookAndFeel} for installing defaults.
	''' <p>
	''' {@code MetalLookAndFeel} derives it's color palette and fonts from
	''' {@code MetalTheme}. The default theme is {@code OceanTheme}. The theme
	''' can be changed using the {@code setCurrentTheme} method, refer to it
	''' for details on changing the theme. Prior to 1.5 the default
	''' theme was {@code DefaultMetalTheme}. The system property
	''' {@code "swing.metalTheme"} can be set to {@code "steel"} to indicate
	''' the default should be {@code DefaultMetalTheme}.
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
	''' <seealso cref= MetalTheme </seealso>
	''' <seealso cref= DefaultMetalTheme </seealso>
	''' <seealso cref= OceanTheme
	''' 
	''' @author Steve Wilson </seealso>
	Public Class MetalLookAndFeel
		Inherits BasicLookAndFeel

		Private Shared METAL_LOOK_AND_FEEL_INITED As Boolean = False


		''' <summary>
		''' True if checked for windows yet.
		''' </summary>
		Private Shared checkedWindows As Boolean
		''' <summary>
		''' True if running on Windows.
		''' </summary>
		Private Shared ___isWindows As Boolean

		''' <summary>
		''' Set to true first time we've checked swing.useSystemFontSettings.
		''' </summary>
		Private Shared checkedSystemFontSettings As Boolean

		''' <summary>
		''' True indicates we should use system fonts, unless the developer has
		''' specified otherwise with Application.useSystemFontSettings.
		''' </summary>
		Private Shared ___useSystemFonts As Boolean


		''' <summary>
		''' Returns true if running on Windows.
		''' </summary>
		Friend Property Shared windows As Boolean
			Get
				If Not checkedWindows Then
					Dim osType As OSInfo.OSType = java.security.AccessController.doPrivileged(OSInfo.oSTypeAction)
					If osType Is OSInfo.OSType.WINDOWS Then
						___isWindows = True
						Dim systemFonts As String = java.security.AccessController.doPrivileged(New sun.security.action.GetPropertyAction("swing.useSystemFontSettings"))
						___useSystemFonts = (systemFonts IsNot Nothing AndAlso (Convert.ToBoolean(systemFonts)))
					End If
					checkedWindows = True
				End If
				Return ___isWindows
			End Get
		End Property

		''' <summary>
		''' Returns true if system fonts should be used, this is only useful
		''' for windows.
		''' </summary>
		Friend Shared Function useSystemFonts() As Boolean
			If windows AndAlso ___useSystemFonts Then
				If METAL_LOOK_AND_FEEL_INITED Then
					Dim value As Object = UIManager.get("Application.useSystemFontSettings")

					Return (value Is Nothing OrElse Boolean.TRUE.Equals(value))
				End If
				' If an instanceof MetalLookAndFeel hasn't been inited yet, we
				' don't want to trigger loading of a UI by asking the UIManager
				' for a property, assume the user wants system fonts. This will
				' be properly adjusted when install is invoked on the
				' MetalTheme
				Return True
			End If
			Return False
		End Function

		''' <summary>
		''' Returns true if the high contrast theme should be used as the default
		''' theme.
		''' </summary>
		Private Shared Function useHighContrastTheme() As Boolean
			If windows AndAlso useSystemFonts() Then
				Dim highContrast As Boolean? = CBool(Toolkit.defaultToolkit.getDesktopProperty("win.highContrast.on"))

				Return If(highContrast Is Nothing, False, highContrast)
			End If
			Return False
		End Function

		''' <summary>
		''' Returns true if we're using the Ocean Theme.
		''' </summary>
		Friend Shared Function usingOcean() As Boolean
			Return (TypeOf currentTheme Is OceanTheme)
		End Function

		''' <summary>
		''' Returns the name of this look and feel. This returns
		''' {@code "Metal"}.
		''' </summary>
		''' <returns> the name of this look and feel </returns>
		Public Property Overrides name As String
			Get
				Return "Metal"
			End Get
		End Property

		''' <summary>
		''' Returns an identifier for this look and feel. This returns
		''' {@code "Metal"}.
		''' </summary>
		''' <returns> the identifier of this look and feel </returns>
		Public Property Overrides iD As String
			Get
				Return "Metal"
			End Get
		End Property

		''' <summary>
		''' Returns a short description of this look and feel. This returns
		''' {@code "The Java(tm) Look and Feel"}.
		''' </summary>
		''' <returns> a short description for the look and feel </returns>
		Public Property Overrides description As String
			Get
				Return "The Java(tm) Look and Feel"
			End Get
		End Property

		''' <summary>
		''' Returns {@code false}; {@code MetalLookAndFeel} is not a native
		''' look and feel.
		''' </summary>
		''' <returns> {@code false} </returns>
		Public Property Overrides nativeLookAndFeel As Boolean
			Get
				Return False
			End Get
		End Property

		''' <summary>
		''' Returns {@code true}; {@code MetalLookAndFeel} can be run on
		''' any platform.
		''' </summary>
		''' <returns> {@code true} </returns>
		Public Property Overrides supportedLookAndFeel As Boolean
			Get
				Return True
			End Get
		End Property

		''' <summary>
		''' Returns {@code true}; metal can provide {@code Window}
		''' decorations.
		''' </summary>
		''' <returns> {@code true}
		''' </returns>
		''' <seealso cref= JDialog#setDefaultLookAndFeelDecorated </seealso>
		''' <seealso cref= JFrame#setDefaultLookAndFeelDecorated </seealso>
		''' <seealso cref= JRootPane#setWindowDecorationStyle
		''' @since 1.4 </seealso>
		Public Property Overrides supportsWindowDecorations As Boolean
			Get
				Return True
			End Get
		End Property

		''' <summary>
		''' Populates {@code table} with mappings from {@code uiClassID} to
		''' the fully qualified name of the ui class. {@code
		''' MetalLookAndFeel} registers an entry for each of the classes in
		''' the package {@code javax.swing.plaf.metal} that are named
		''' MetalXXXUI. The string {@code XXX} is one of Swing's uiClassIDs. For
		''' the {@code uiClassIDs} that do not have a class in metal, the
		''' corresponding class in {@code javax.swing.plaf.basic} is
		''' used. For example, metal does not have a class named {@code
		''' "MetalColorChooserUI"}, as such, {@code
		''' javax.swing.plaf.basic.BasicColorChooserUI} is used.
		''' </summary>
		''' <param name="table"> the {@code UIDefaults} instance the entries are
		'''        added to </param>
		''' <exception cref="NullPointerException"> if {@code table} is {@code null}
		''' </exception>
		''' <seealso cref= javax.swing.plaf.basic.BasicLookAndFeel#initClassDefaults </seealso>
		Protected Friend Overridable Sub initClassDefaults(ByVal table As UIDefaults)
			MyBase.initClassDefaults(table)
			Const metalPackageName As String = "javax.swing.plaf.metal."

			Dim uiDefaults As Object() = { "ButtonUI", metalPackageName & "MetalButtonUI", "CheckBoxUI", metalPackageName & "MetalCheckBoxUI", "ComboBoxUI", metalPackageName & "MetalComboBoxUI", "DesktopIconUI", metalPackageName & "MetalDesktopIconUI", "FileChooserUI", metalPackageName & "MetalFileChooserUI", "InternalFrameUI", metalPackageName & "MetalInternalFrameUI", "LabelUI", metalPackageName & "MetalLabelUI", "PopupMenuSeparatorUI", metalPackageName & "MetalPopupMenuSeparatorUI", "ProgressBarUI", metalPackageName & "MetalProgressBarUI", "RadioButtonUI", metalPackageName & "MetalRadioButtonUI", "ScrollBarUI", metalPackageName & "MetalScrollBarUI", "ScrollPaneUI", metalPackageName & "MetalScrollPaneUI", "SeparatorUI", metalPackageName & "MetalSeparatorUI", "SliderUI", metalPackageName & "MetalSliderUI", "SplitPaneUI", metalPackageName & "MetalSplitPaneUI", "TabbedPaneUI", metalPackageName & "MetalTabbedPaneUI", "TextFieldUI", metalPackageName & "MetalTextFieldUI", "ToggleButtonUI", metalPackageName & "MetalToggleButtonUI", "ToolBarUI", metalPackageName & "MetalToolBarUI", "ToolTipUI", metalPackageName & "MetalToolTipUI", "TreeUI", metalPackageName & "MetalTreeUI", "RootPaneUI", metalPackageName & "MetalRootPaneUI" }

			table.putDefaults(uiDefaults)
		End Sub

		''' <summary>
		''' Populates {@code table} with system colors. The following values are
		''' added to {@code table}:
		''' <table border="1" cellpadding="1" cellspacing="0"
		'''         summary="Metal's system color mapping">
		'''  <tr valign="top"  align="left">
		'''    <th style="background-color:#CCCCFF" align="left">Key
		'''    <th style="background-color:#CCCCFF" align="left">Value
		'''  <tr valign="top"  align="left">
		'''    <td>"desktop"
		'''    <td>{@code theme.getDesktopColor()}
		'''  <tr valign="top"  align="left">
		'''    <td>"activeCaption"
		'''    <td>{@code theme.getWindowTitleBackground()}
		'''  <tr valign="top"  align="left">
		'''    <td>"activeCaptionText"
		'''    <td>{@code theme.getWindowTitleForeground()}
		'''  <tr valign="top"  align="left">
		'''    <td>"activeCaptionBorder"
		'''    <td>{@code theme.getPrimaryControlShadow()}
		'''  <tr valign="top"  align="left">
		'''    <td>"inactiveCaption"
		'''    <td>{@code theme.getWindowTitleInactiveBackground()}
		'''  <tr valign="top"  align="left">
		'''    <td>"inactiveCaptionText"
		'''    <td>{@code theme.getWindowTitleInactiveForeground()}
		'''  <tr valign="top"  align="left">
		'''    <td>"inactiveCaptionBorder"
		'''    <td>{@code theme.getControlShadow()}
		'''  <tr valign="top"  align="left">
		'''    <td>"window"
		'''    <td>{@code theme.getWindowBackground()}
		'''  <tr valign="top"  align="left">
		'''    <td>"windowBorder"
		'''    <td>{@code theme.getControl()}
		'''  <tr valign="top"  align="left">
		'''    <td>"windowText"
		'''    <td>{@code theme.getUserTextColor()}
		'''  <tr valign="top"  align="left">
		'''    <td>"menu"
		'''    <td>{@code theme.getMenuBackground()}
		'''  <tr valign="top"  align="left">
		'''    <td>"menuText"
		'''    <td>{@code theme.getMenuForeground()}
		'''  <tr valign="top"  align="left">
		'''    <td>"text"
		'''    <td>{@code theme.getWindowBackground()}
		'''  <tr valign="top"  align="left">
		'''    <td>"textText"
		'''    <td>{@code theme.getUserTextColor()}
		'''  <tr valign="top"  align="left">
		'''    <td>"textHighlight"
		'''    <td>{@code theme.getTextHighlightColor()}
		'''  <tr valign="top"  align="left">
		'''    <td>"textHighlightText"
		'''    <td>{@code theme.getHighlightedTextColor()}
		'''  <tr valign="top"  align="left">
		'''    <td>"textInactiveText"
		'''    <td>{@code theme.getInactiveSystemTextColor()}
		'''  <tr valign="top"  align="left">
		'''    <td>"control"
		'''    <td>{@code theme.getControl()}
		'''  <tr valign="top"  align="left">
		'''    <td>"controlText"
		'''    <td>{@code theme.getControlTextColor()}
		'''  <tr valign="top"  align="left">
		'''    <td>"controlHighlight"
		'''    <td>{@code theme.getControlHighlight()}
		'''  <tr valign="top"  align="left">
		'''    <td>"controlLtHighlight"
		'''    <td>{@code theme.getControlHighlight()}
		'''  <tr valign="top"  align="left">
		'''    <td>"controlShadow"
		'''    <td>{@code theme.getControlShadow()}
		'''  <tr valign="top"  align="left">
		'''    <td>"controlDkShadow"
		'''    <td>{@code theme.getControlDarkShadow()}
		'''  <tr valign="top"  align="left">
		'''    <td>"scrollbar"
		'''    <td>{@code theme.getControl()}
		'''  <tr valign="top"  align="left">
		'''    <td>"info"
		'''    <td>{@code theme.getPrimaryControl()}
		'''  <tr valign="top"  align="left">
		'''    <td>"infoText"
		'''    <td>{@code theme.getPrimaryControlInfo()}
		''' </table>
		''' The value {@code theme} corresponds to the current {@code MetalTheme}.
		''' </summary>
		''' <param name="table"> the {@code UIDefaults} object the values are added to </param>
		''' <exception cref="NullPointerException"> if {@code table} is {@code null} </exception>
		Protected Friend Overridable Sub initSystemColorDefaults(ByVal table As UIDefaults)
			Dim theme As MetalTheme = currentTheme
			Dim ___control As java.awt.Color = theme.control
			Dim systemColors As Object() = { "desktop", theme.desktopColor, "activeCaption", theme.windowTitleBackground, "activeCaptionText", theme.windowTitleForeground, "activeCaptionBorder", theme.primaryControlShadow, "inactiveCaption", theme.windowTitleInactiveBackground, "inactiveCaptionText", theme.windowTitleInactiveForeground, "inactiveCaptionBorder", theme.controlShadow, "window", theme.windowBackground, "windowBorder", ___control, "windowText", theme.userTextColor, "menu", theme.menuBackground, "menuText", theme.menuForeground, "text", theme.windowBackground, "textText", theme.userTextColor, "textHighlight", theme.textHighlightColor, "textHighlightText", theme.highlightedTextColor, "textInactiveText", theme.inactiveSystemTextColor, "control", ___control, "controlText", theme.controlTextColor, "controlHighlight", theme.controlHighlight, "controlLtHighlight", theme.controlHighlight, "controlShadow", theme.controlShadow, "controlDkShadow", theme.controlDarkShadow, "scrollbar", ___control, "info", theme.primaryControl, "infoText", theme.primaryControlInfo }

			table.putDefaults(systemColors)
		End Sub

		''' <summary>
		''' Initialize the defaults table with the name of the ResourceBundle
		''' used for getting localized defaults.
		''' </summary>
		Private Sub initResourceBundle(ByVal table As UIDefaults)
			table.addResourceBundle("com.sun.swing.internal.plaf.metal.resources.metal")
		End Sub

		''' <summary>
		''' Populates {@code table} with the defaults for metal.
		''' </summary>
		''' <param name="table"> the {@code UIDefaults} to add the values to </param>
		''' <exception cref="NullPointerException"> if {@code table} is {@code null} </exception>
		Protected Friend Overridable Sub initComponentDefaults(ByVal table As UIDefaults)
			MyBase.initComponentDefaults(table)

			initResourceBundle(table)

			Dim ___acceleratorForeground As java.awt.Color = acceleratorForeground
			Dim ___acceleratorSelectedForeground As java.awt.Color = acceleratorSelectedForeground
			Dim ___control As java.awt.Color = control
			Dim ___controlHighlight As java.awt.Color = controlHighlight
			Dim ___controlShadow As java.awt.Color = controlShadow
			Dim ___controlDarkShadow As java.awt.Color = controlDarkShadow
			Dim ___controlTextColor As java.awt.Color = controlTextColor
			Dim ___focusColor As java.awt.Color = focusColor
			Dim ___inactiveControlTextColor As java.awt.Color = inactiveControlTextColor
			Dim ___menuBackground As java.awt.Color = menuBackground
			Dim ___menuSelectedBackground As java.awt.Color = menuSelectedBackground
			Dim ___menuDisabledForeground As java.awt.Color = menuDisabledForeground
			Dim ___menuSelectedForeground As java.awt.Color = menuSelectedForeground
			Dim ___primaryControl As java.awt.Color = primaryControl
			Dim ___primaryControlDarkShadow As java.awt.Color = primaryControlDarkShadow
			Dim ___primaryControlShadow As java.awt.Color = primaryControlShadow
			Dim ___systemTextColor As java.awt.Color = systemTextColor

			Dim zeroInsets As Insets = New InsetsUIResource(0, 0, 0, 0)

			Dim zero As Integer? = Convert.ToInt32(0)

			Dim textFieldBorder As Object = New sun.swing.SwingLazyValue("javax.swing.plaf.metal.MetalBorders", "getTextFieldBorder")

			Dim dialogBorder As javax.swing.UIDefaults.LazyValue = t -> New MetalBorders.DialogBorder

			Dim questionDialogBorder As javax.swing.UIDefaults.LazyValue = t -> New MetalBorders.QuestionDialogBorder

			Dim fieldInputMap As Object = New UIDefaults.LazyInputMap(New Object() { "ctrl C", javax.swing.text.DefaultEditorKit.copyAction, "ctrl V", javax.swing.text.DefaultEditorKit.pasteAction, "ctrl X", javax.swing.text.DefaultEditorKit.cutAction, "COPY", javax.swing.text.DefaultEditorKit.copyAction, "PASTE", javax.swing.text.DefaultEditorKit.pasteAction, "CUT", javax.swing.text.DefaultEditorKit.cutAction, "control INSERT", javax.swing.text.DefaultEditorKit.copyAction, "shift INSERT", javax.swing.text.DefaultEditorKit.pasteAction, "shift DELETE", javax.swing.text.DefaultEditorKit.cutAction, "shift LEFT", javax.swing.text.DefaultEditorKit.selectionBackwardAction, "shift KP_LEFT", javax.swing.text.DefaultEditorKit.selectionBackwardAction, "shift RIGHT", javax.swing.text.DefaultEditorKit.selectionForwardAction, "shift KP_RIGHT", javax.swing.text.DefaultEditorKit.selectionForwardAction, "ctrl LEFT", javax.swing.text.DefaultEditorKit.previousWordAction, "ctrl KP_LEFT", javax.swing.text.DefaultEditorKit.previousWordAction, "ctrl RIGHT", javax.swing.text.DefaultEditorKit.nextWordAction, "ctrl KP_RIGHT", javax.swing.text.DefaultEditorKit.nextWordAction, "ctrl shift LEFT", javax.swing.text.DefaultEditorKit.selectionPreviousWordAction, "ctrl shift KP_LEFT", javax.swing.text.DefaultEditorKit.selectionPreviousWordAction, "ctrl shift RIGHT", javax.swing.text.DefaultEditorKit.selectionNextWordAction, "ctrl shift KP_RIGHT", javax.swing.text.DefaultEditorKit.selectionNextWordAction, "ctrl A", javax.swing.text.DefaultEditorKit.selectAllAction, "HOME", javax.swing.text.DefaultEditorKit.beginLineAction, "END", javax.swing.text.DefaultEditorKit.endLineAction, "shift HOME", javax.swing.text.DefaultEditorKit.selectionBeginLineAction, "shift END", javax.swing.text.DefaultEditorKit.selectionEndLineAction, "BACK_SPACE", javax.swing.text.DefaultEditorKit.deletePrevCharAction, "shift BACK_SPACE", javax.swing.text.DefaultEditorKit.deletePrevCharAction, "ctrl H", javax.swing.text.DefaultEditorKit.deletePrevCharAction, "DELETE", javax.swing.text.DefaultEditorKit.deleteNextCharAction, "ctrl DELETE", javax.swing.text.DefaultEditorKit.deleteNextWordAction, "ctrl BACK_SPACE", javax.swing.text.DefaultEditorKit.deletePrevWordAction, "RIGHT", javax.swing.text.DefaultEditorKit.forwardAction, "LEFT", javax.swing.text.DefaultEditorKit.backwardAction, "KP_RIGHT", javax.swing.text.DefaultEditorKit.forwardAction, "KP_LEFT", javax.swing.text.DefaultEditorKit.backwardAction, "ENTER", JTextField.notifyAction, "ctrl BACK_SLASH", "unselect", "control shift O", "toggle-componentOrientation" }) 'DefaultEditorKit.toggleComponentOrientation - DefaultEditorKit.unselectAction

			Dim passwordInputMap As Object = New UIDefaults.LazyInputMap(New Object() { "ctrl C", javax.swing.text.DefaultEditorKit.copyAction, "ctrl V", javax.swing.text.DefaultEditorKit.pasteAction, "ctrl X", javax.swing.text.DefaultEditorKit.cutAction, "COPY", javax.swing.text.DefaultEditorKit.copyAction, "PASTE", javax.swing.text.DefaultEditorKit.pasteAction, "CUT", javax.swing.text.DefaultEditorKit.cutAction, "control INSERT", javax.swing.text.DefaultEditorKit.copyAction, "shift INSERT", javax.swing.text.DefaultEditorKit.pasteAction, "shift DELETE", javax.swing.text.DefaultEditorKit.cutAction, "shift LEFT", javax.swing.text.DefaultEditorKit.selectionBackwardAction, "shift KP_LEFT", javax.swing.text.DefaultEditorKit.selectionBackwardAction, "shift RIGHT", javax.swing.text.DefaultEditorKit.selectionForwardAction, "shift KP_RIGHT", javax.swing.text.DefaultEditorKit.selectionForwardAction, "ctrl LEFT", javax.swing.text.DefaultEditorKit.beginLineAction, "ctrl KP_LEFT", javax.swing.text.DefaultEditorKit.beginLineAction, "ctrl RIGHT", javax.swing.text.DefaultEditorKit.endLineAction, "ctrl KP_RIGHT", javax.swing.text.DefaultEditorKit.endLineAction, "ctrl shift LEFT", javax.swing.text.DefaultEditorKit.selectionBeginLineAction, "ctrl shift KP_LEFT", javax.swing.text.DefaultEditorKit.selectionBeginLineAction, "ctrl shift RIGHT", javax.swing.text.DefaultEditorKit.selectionEndLineAction, "ctrl shift KP_RIGHT", javax.swing.text.DefaultEditorKit.selectionEndLineAction, "ctrl A", javax.swing.text.DefaultEditorKit.selectAllAction, "HOME", javax.swing.text.DefaultEditorKit.beginLineAction, "END", javax.swing.text.DefaultEditorKit.endLineAction, "shift HOME", javax.swing.text.DefaultEditorKit.selectionBeginLineAction, "shift END", javax.swing.text.DefaultEditorKit.selectionEndLineAction, "BACK_SPACE", javax.swing.text.DefaultEditorKit.deletePrevCharAction, "shift BACK_SPACE", javax.swing.text.DefaultEditorKit.deletePrevCharAction, "ctrl H", javax.swing.text.DefaultEditorKit.deletePrevCharAction, "DELETE", javax.swing.text.DefaultEditorKit.deleteNextCharAction, "RIGHT", javax.swing.text.DefaultEditorKit.forwardAction, "LEFT", javax.swing.text.DefaultEditorKit.backwardAction, "KP_RIGHT", javax.swing.text.DefaultEditorKit.forwardAction, "KP_LEFT", javax.swing.text.DefaultEditorKit.backwardAction, "ENTER", JTextField.notifyAction, "ctrl BACK_SLASH", "unselect", "control shift O", "toggle-componentOrientation" }) 'DefaultEditorKit.toggleComponentOrientation - DefaultEditorKit.unselectAction

			Dim multilineInputMap As Object = New UIDefaults.LazyInputMap(New Object() { "ctrl C", javax.swing.text.DefaultEditorKit.copyAction, "ctrl V", javax.swing.text.DefaultEditorKit.pasteAction, "ctrl X", javax.swing.text.DefaultEditorKit.cutAction, "COPY", javax.swing.text.DefaultEditorKit.copyAction, "PASTE", javax.swing.text.DefaultEditorKit.pasteAction, "CUT", javax.swing.text.DefaultEditorKit.cutAction, "control INSERT", javax.swing.text.DefaultEditorKit.copyAction, "shift INSERT", javax.swing.text.DefaultEditorKit.pasteAction, "shift DELETE", javax.swing.text.DefaultEditorKit.cutAction, "shift LEFT", javax.swing.text.DefaultEditorKit.selectionBackwardAction, "shift KP_LEFT", javax.swing.text.DefaultEditorKit.selectionBackwardAction, "shift RIGHT", javax.swing.text.DefaultEditorKit.selectionForwardAction, "shift KP_RIGHT", javax.swing.text.DefaultEditorKit.selectionForwardAction, "ctrl LEFT", javax.swing.text.DefaultEditorKit.previousWordAction, "ctrl KP_LEFT", javax.swing.text.DefaultEditorKit.previousWordAction, "ctrl RIGHT", javax.swing.text.DefaultEditorKit.nextWordAction, "ctrl KP_RIGHT", javax.swing.text.DefaultEditorKit.nextWordAction, "ctrl shift LEFT", javax.swing.text.DefaultEditorKit.selectionPreviousWordAction, "ctrl shift KP_LEFT", javax.swing.text.DefaultEditorKit.selectionPreviousWordAction, "ctrl shift RIGHT", javax.swing.text.DefaultEditorKit.selectionNextWordAction, "ctrl shift KP_RIGHT", javax.swing.text.DefaultEditorKit.selectionNextWordAction, "ctrl A", javax.swing.text.DefaultEditorKit.selectAllAction, "HOME", javax.swing.text.DefaultEditorKit.beginLineAction, "END", javax.swing.text.DefaultEditorKit.endLineAction, "shift HOME", javax.swing.text.DefaultEditorKit.selectionBeginLineAction, "shift END", javax.swing.text.DefaultEditorKit.selectionEndLineAction, "UP", javax.swing.text.DefaultEditorKit.upAction, "KP_UP", javax.swing.text.DefaultEditorKit.upAction, "DOWN", javax.swing.text.DefaultEditorKit.downAction, "KP_DOWN", javax.swing.text.DefaultEditorKit.downAction, "PAGE_UP", javax.swing.text.DefaultEditorKit.pageUpAction, "PAGE_DOWN", javax.swing.text.DefaultEditorKit.pageDownAction, "shift PAGE_UP", "selection-page-up", "shift PAGE_DOWN", "selection-page-down", "ctrl shift PAGE_UP", "selection-page-left", "ctrl shift PAGE_DOWN", "selection-page-right", "shift UP", javax.swing.text.DefaultEditorKit.selectionUpAction, "shift KP_UP", javax.swing.text.DefaultEditorKit.selectionUpAction, "shift DOWN", javax.swing.text.DefaultEditorKit.selectionDownAction, "shift KP_DOWN", javax.swing.text.DefaultEditorKit.selectionDownAction, "ENTER", javax.swing.text.DefaultEditorKit.insertBreakAction, "BACK_SPACE", javax.swing.text.DefaultEditorKit.deletePrevCharAction, "shift BACK_SPACE", javax.swing.text.DefaultEditorKit.deletePrevCharAction, "ctrl H", javax.swing.text.DefaultEditorKit.deletePrevCharAction, "DELETE", javax.swing.text.DefaultEditorKit.deleteNextCharAction, "ctrl DELETE", javax.swing.text.DefaultEditorKit.deleteNextWordAction, "ctrl BACK_SPACE", javax.swing.text.DefaultEditorKit.deletePrevWordAction, "RIGHT", javax.swing.text.DefaultEditorKit.forwardAction, "LEFT", javax.swing.text.DefaultEditorKit.backwardAction, "KP_RIGHT", javax.swing.text.DefaultEditorKit.forwardAction, "KP_LEFT", javax.swing.text.DefaultEditorKit.backwardAction, "TAB", javax.swing.text.DefaultEditorKit.insertTabAction, "ctrl BACK_SLASH", "unselect", "ctrl HOME", javax.swing.text.DefaultEditorKit.beginAction, "ctrl END", javax.swing.text.DefaultEditorKit.endAction, "ctrl shift HOME", javax.swing.text.DefaultEditorKit.selectionBeginAction, "ctrl shift END", javax.swing.text.DefaultEditorKit.selectionEndAction, "ctrl T", "next-link-action", "ctrl shift T", "previous-link-action", "ctrl SPACE", "activate-link-action", "control shift O", "toggle-componentOrientation" }) 'DefaultEditorKit.toggleComponentOrientation - DefaultEditorKit.unselectAction

			Dim scrollPaneBorder As Object = New sun.swing.SwingLazyValue("javax.swing.plaf.metal.MetalBorders$ScrollPaneBorder")
			Dim buttonBorder As Object = New sun.swing.SwingLazyValue("javax.swing.plaf.metal.MetalBorders", "getButtonBorder")

			Dim toggleButtonBorder As Object = New sun.swing.SwingLazyValue("javax.swing.plaf.metal.MetalBorders", "getToggleButtonBorder")

			Dim titledBorderBorder As Object = New sun.swing.SwingLazyValue("javax.swing.plaf.BorderUIResource$LineBorderUIResource", New Object() {___controlShadow})

			Dim desktopIconBorder As Object = New sun.swing.SwingLazyValue("javax.swing.plaf.metal.MetalBorders", "getDesktopIconBorder")

			Dim menuBarBorder As Object = New sun.swing.SwingLazyValue("javax.swing.plaf.metal.MetalBorders$MenuBarBorder")

			Dim popupMenuBorder As Object = New sun.swing.SwingLazyValue("javax.swing.plaf.metal.MetalBorders$PopupMenuBorder")
			Dim menuItemBorder As Object = New sun.swing.SwingLazyValue("javax.swing.plaf.metal.MetalBorders$MenuItemBorder")

			Dim menuItemAcceleratorDelimiter As Object = "-"
			Dim toolBarBorder As Object = New sun.swing.SwingLazyValue("javax.swing.plaf.metal.MetalBorders$ToolBarBorder")

			Dim progressBarBorder As Object = New sun.swing.SwingLazyValue("javax.swing.plaf.BorderUIResource$LineBorderUIResource", New Object() {___controlDarkShadow, New Integer?(1)})

			Dim toolTipBorder As Object = New sun.swing.SwingLazyValue("javax.swing.plaf.BorderUIResource$LineBorderUIResource", New Object() {___primaryControlDarkShadow})

			Dim toolTipBorderInactive As Object = New sun.swing.SwingLazyValue("javax.swing.plaf.BorderUIResource$LineBorderUIResource", New Object() {___controlDarkShadow})

			Dim focusCellHighlightBorder As Object = New sun.swing.SwingLazyValue("javax.swing.plaf.BorderUIResource$LineBorderUIResource", New Object() {___focusColor})

			Dim tabbedPaneTabAreaInsets As Object = New InsetsUIResource(4, 2, 0, 6)

			Dim tabbedPaneTabInsets As Object = New InsetsUIResource(0, 9, 1, 9)

			Dim internalFrameIconArgs As Object() = New Object(0){}
			internalFrameIconArgs(0) = New Integer?(16)

			Dim defaultCueList As Object() = { "OptionPane.errorSound", "OptionPane.informationSound", "OptionPane.questionSound", "OptionPane.warningSound" }

			Dim theme As MetalTheme = currentTheme
			Dim menuTextValue As Object = New FontActiveValue(theme, MetalTheme.MENU_TEXT_FONT)
			Dim controlTextValue As Object = New FontActiveValue(theme, MetalTheme.CONTROL_TEXT_FONT)
			Dim userTextValue As Object = New FontActiveValue(theme, MetalTheme.USER_TEXT_FONT)
			Dim windowTitleValue As Object = New FontActiveValue(theme, MetalTheme.WINDOW_TITLE_FONT)
			Dim subTextValue As Object = New FontActiveValue(theme, MetalTheme.SUB_TEXT_FONT)
			Dim systemTextValue As Object = New FontActiveValue(theme, MetalTheme.SYSTEM_TEXT_FONT)
			'
			' DEFAULTS TABLE
			'

			Dim ___defaults As Object() = { "AuditoryCues.defaultCueList", defaultCueList, "AuditoryCues.playList", Nothing, "TextField.border", textFieldBorder, "TextField.font", userTextValue, "PasswordField.border", textFieldBorder, "PasswordField.font", userTextValue, "PasswordField.echoChar", ChrW(&H2022), "TextArea.font", userTextValue, "TextPane.background", table("window"), "TextPane.font", userTextValue, "EditorPane.background", table("window"), "EditorPane.font", userTextValue, "TextField.focusInputMap", fieldInputMap, "PasswordField.focusInputMap", passwordInputMap, "TextArea.focusInputMap", multilineInputMap, "TextPane.focusInputMap", multilineInputMap, "EditorPane.focusInputMap", multilineInputMap, "FormattedTextField.border", textFieldBorder, "FormattedTextField.font", userTextValue, "FormattedTextField.focusInputMap", New UIDefaults.LazyInputMap(New Object() { "ctrl C", javax.swing.text.DefaultEditorKit.copyAction, "ctrl V", javax.swing.text.DefaultEditorKit.pasteAction, "ctrl X", javax.swing.text.DefaultEditorKit.cutAction, "COPY", javax.swing.text.DefaultEditorKit.copyAction, "PASTE", javax.swing.text.DefaultEditorKit.pasteAction, "CUT", javax.swing.text.DefaultEditorKit.cutAction, "control INSERT", javax.swing.text.DefaultEditorKit.copyAction, "shift INSERT", javax.swing.text.DefaultEditorKit.pasteAction, "shift DELETE", javax.swing.text.DefaultEditorKit.cutAction, "shift LEFT", javax.swing.text.DefaultEditorKit.selectionBackwardAction, "shift KP_LEFT", javax.swing.text.DefaultEditorKit.selectionBackwardAction, "shift RIGHT", javax.swing.text.DefaultEditorKit.selectionForwardAction, "shift KP_RIGHT", javax.swing.text.DefaultEditorKit.selectionForwardAction, "ctrl LEFT", javax.swing.text.DefaultEditorKit.previousWordAction, "ctrl KP_LEFT", javax.swing.text.DefaultEditorKit.previousWordAction, "ctrl RIGHT", javax.swing.text.DefaultEditorKit.nextWordAction, "ctrl KP_RIGHT", javax.swing.text.DefaultEditorKit.nextWordAction, "ctrl shift LEFT", javax.swing.text.DefaultEditorKit.selectionPreviousWordAction, "ctrl shift KP_LEFT", javax.swing.text.DefaultEditorKit.selectionPreviousWordAction, "ctrl shift RIGHT", javax.swing.text.DefaultEditorKit.selectionNextWordAction, "ctrl shift KP_RIGHT", javax.swing.text.DefaultEditorKit.selectionNextWordAction, "ctrl A", javax.swing.text.DefaultEditorKit.selectAllAction, "HOME", javax.swing.text.DefaultEditorKit.beginLineAction, "END", javax.swing.text.DefaultEditorKit.endLineAction, "shift HOME", javax.swing.text.DefaultEditorKit.selectionBeginLineAction, "shift END", javax.swing.text.DefaultEditorKit.selectionEndLineAction, "BACK_SPACE", javax.swing.text.DefaultEditorKit.deletePrevCharAction, "shift BACK_SPACE", javax.swing.text.DefaultEditorKit.deletePrevCharAction, "ctrl H", javax.swing.text.DefaultEditorKit.deletePrevCharAction, "DELETE", javax.swing.text.DefaultEditorKit.deleteNextCharAction, "ctrl DELETE", javax.swing.text.DefaultEditorKit.deleteNextWordAction, "ctrl BACK_SPACE", javax.swing.text.DefaultEditorKit.deletePrevWordAction, "RIGHT", javax.swing.text.DefaultEditorKit.forwardAction, "LEFT", javax.swing.text.DefaultEditorKit.backwardAction, "KP_RIGHT", javax.swing.text.DefaultEditorKit.forwardAction, "KP_LEFT", javax.swing.text.DefaultEditorKit.backwardAction, "ENTER", JTextField.notifyAction, "ctrl BACK_SLASH", "unselect", "control shift O", "toggle-componentOrientation", "ESCAPE", "reset-field-edit", "UP", "increment", "KP_UP", "increment", "DOWN", "decrement", "KP_DOWN", "decrement" }), "Button.defaultButtonFollowsFocus", Boolean.FALSE, "Button.disabledText", ___inactiveControlTextColor, "Button.select", ___controlShadow, "Button.border", buttonBorder, "Button.font", controlTextValue, "Button.focus", ___focusColor, "Button.focusInputMap", New UIDefaults.LazyInputMap(New Object() { "SPACE", "pressed", "released SPACE", "released" }), "CheckBox.disabledText", ___inactiveControlTextColor, "Checkbox.select", ___controlShadow, "CheckBox.font", controlTextValue, "CheckBox.focus", ___focusColor, "CheckBox.icon", New sun.swing.SwingLazyValue("javax.swing.plaf.metal.MetalIconFactory", "getCheckBoxIcon"), "CheckBox.focusInputMap", New UIDefaults.LazyInputMap(New Object() { "SPACE", "pressed", "released SPACE", "released" }), "CheckBox.totalInsets", New Insets(4, 4, 4, 4), "RadioButton.disabledText", ___inactiveControlTextColor, "RadioButton.select", ___controlShadow, "RadioButton.icon", New sun.swing.SwingLazyValue("javax.swing.plaf.metal.MetalIconFactory", "getRadioButtonIcon"), "RadioButton.font", controlTextValue, "RadioButton.focus", ___focusColor, "RadioButton.focusInputMap", New UIDefaults.LazyInputMap(New Object() { "SPACE", "pressed", "released SPACE", "released" }), "RadioButton.totalInsets", New Insets(4, 4, 4, 4), "ToggleButton.select", ___controlShadow, "ToggleButton.disabledText", ___inactiveControlTextColor, "ToggleButton.focus", ___focusColor, "ToggleButton.border", toggleButtonBorder, "ToggleButton.font", controlTextValue, "ToggleButton.focusInputMap", New UIDefaults.LazyInputMap(New Object() { "SPACE", "pressed", "released SPACE", "released" }), "FileView.directoryIcon", New sun.swing.SwingLazyValue("javax.swing.plaf.metal.MetalIconFactory", "getTreeFolderIcon"), "FileView.fileIcon", New sun.swing.SwingLazyValue("javax.swing.plaf.metal.MetalIconFactory", "getTreeLeafIcon"), "FileView.computerIcon", New sun.swing.SwingLazyValue("javax.swing.plaf.metal.MetalIconFactory", "getTreeComputerIcon"), "FileView.hardDriveIcon", New sun.swing.SwingLazyValue("javax.swing.plaf.metal.MetalIconFactory", "getTreeHardDriveIcon"), "FileView.floppyDriveIcon", New sun.swing.SwingLazyValue("javax.swing.plaf.metal.MetalIconFactory", "getTreeFloppyDriveIcon"), "FileChooser.detailsViewIcon", New sun.swing.SwingLazyValue("javax.swing.plaf.metal.MetalIconFactory", "getFileChooserDetailViewIcon"), "FileChooser.homeFolderIcon", New sun.swing.SwingLazyValue("javax.swing.plaf.metal.MetalIconFactory", "getFileChooserHomeFolderIcon"), "FileChooser.listViewIcon", New sun.swing.SwingLazyValue("javax.swing.plaf.metal.MetalIconFactory", "getFileChooserListViewIcon"), "FileChooser.newFolderIcon", New sun.swing.SwingLazyValue("javax.swing.plaf.metal.MetalIconFactory", "getFileChooserNewFolderIcon"), "FileChooser.upFolderIcon", New sun.swing.SwingLazyValue("javax.swing.plaf.metal.MetalIconFactory", "getFileChooserUpFolderIcon"), "FileChooser.usesSingleFilePane", Boolean.TRUE, "FileChooser.ancestorInputMap", New UIDefaults.LazyInputMap(New Object() { "ESCAPE", "cancelSelection", "F2", "editFileName", "F5", "refresh", "BACK_SPACE", "Go Up" }), "ToolTip.font", systemTextValue, "ToolTip.border", toolTipBorder, "ToolTip.borderInactive", toolTipBorderInactive, "ToolTip.backgroundInactive", ___control, "ToolTip.foregroundInactive", ___controlDarkShadow, "ToolTip.hideAccelerator", Boolean.FALSE, "ToolTipManager.enableToolTipMode", "activeApplication", "Slider.font", controlTextValue, "Slider.border", Nothing, "Slider.foreground", ___primaryControlShadow, "Slider.focus", ___focusColor, "Slider.focusInsets", zeroInsets, "Slider.trackWidth", New Integer?(7), "Slider.majorTickLength", New Integer?(6), "Slider.horizontalThumbIcon", New sun.swing.SwingLazyValue("javax.swing.plaf.metal.MetalIconFactory", "getHorizontalSliderThumbIcon"), "Slider.verticalThumbIcon", New sun.swing.SwingLazyValue("javax.swing.plaf.metal.MetalIconFactory", "getVerticalSliderThumbIcon"), "Slider.focusInputMap", New UIDefaults.LazyInputMap(New Object() { "RIGHT", "positiveUnitIncrement", "KP_RIGHT", "positiveUnitIncrement", "DOWN", "negativeUnitIncrement", "KP_DOWN", "negativeUnitIncrement", "PAGE_DOWN", "negativeBlockIncrement", "ctrl PAGE_DOWN", "negativeBlockIncrement", "LEFT", "negativeUnitIncrement", "KP_LEFT", "negativeUnitIncrement", "UP", "positiveUnitIncrement", "KP_UP", "positiveUnitIncrement", "PAGE_UP", "positiveBlockIncrement", "ctrl PAGE_UP", "positiveBlockIncrement", "HOME", "minScroll", "END", "maxScroll" }), "ProgressBar.font", controlTextValue, "ProgressBar.foreground", ___primaryControlShadow, "ProgressBar.selectionBackground", ___primaryControlDarkShadow, "ProgressBar.border", progressBarBorder, "ProgressBar.cellSpacing", zero, "ProgressBar.cellLength", Convert.ToInt32(1), "ComboBox.background", ___control, "ComboBox.foreground", ___controlTextColor, "ComboBox.selectionBackground", ___primaryControlShadow, "ComboBox.selectionForeground", ___controlTextColor, "ComboBox.font", controlTextValue, "ComboBox.ancestorInputMap", New UIDefaults.LazyInputMap(New Object() { "ESCAPE", "hidePopup", "PAGE_UP", "pageUpPassThrough", "PAGE_DOWN", "pageDownPassThrough", "HOME", "homePassThrough", "END", "endPassThrough", "DOWN", "selectNext", "KP_DOWN", "selectNext", "alt DOWN", "togglePopup", "alt KP_DOWN", "togglePopup", "alt UP", "togglePopup", "alt KP_UP", "togglePopup", "SPACE", "spacePopup", "ENTER", "enterPressed", "UP", "selectPrevious", "KP_UP", "selectPrevious" }), "InternalFrame.icon", New sun.swing.SwingLazyValue("javax.swing.plaf.metal.MetalIconFactory", "getInternalFrameDefaultMenuIcon"), "InternalFrame.border", New sun.swing.SwingLazyValue("javax.swing.plaf.metal.MetalBorders$InternalFrameBorder"), "InternalFrame.optionDialogBorder", New sun.swing.SwingLazyValue("javax.swing.plaf.metal.MetalBorders$OptionDialogBorder"), "InternalFrame.paletteBorder", New sun.swing.SwingLazyValue("javax.swing.plaf.metal.MetalBorders$PaletteBorder"), "InternalFrame.paletteTitleHeight", New Integer?(11), "InternalFrame.paletteCloseIcon", New sun.swing.SwingLazyValue("javax.swing.plaf.metal.MetalIconFactory$PaletteCloseIcon"), "InternalFrame.closeIcon", New sun.swing.SwingLazyValue("javax.swing.plaf.metal.MetalIconFactory", "getInternalFrameCloseIcon", internalFrameIconArgs), "InternalFrame.maximizeIcon", New sun.swing.SwingLazyValue("javax.swing.plaf.metal.MetalIconFactory", "getInternalFrameMaximizeIcon", internalFrameIconArgs), "InternalFrame.iconifyIcon", New sun.swing.SwingLazyValue("javax.swing.plaf.metal.MetalIconFactory", "getInternalFrameMinimizeIcon", internalFrameIconArgs), "InternalFrame.minimizeIcon", New sun.swing.SwingLazyValue("javax.swing.plaf.metal.MetalIconFactory", "getInternalFrameAltMaximizeIcon", internalFrameIconArgs), "InternalFrame.titleFont", windowTitleValue, "InternalFrame.windowBindings", Nothing, "InternalFrame.closeSound", "sounds/FrameClose.wav", "InternalFrame.maximizeSound", "sounds/FrameMaximize.wav", "InternalFrame.minimizeSound", "sounds/FrameMinimize.wav", "InternalFrame.restoreDownSound", "sounds/FrameRestoreDown.wav", "InternalFrame.restoreUpSound", "sounds/FrameRestoreUp.wav", "DesktopIcon.border", desktopIconBorder, "DesktopIcon.font", controlTextValue, "DesktopIcon.foreground", ___controlTextColor, "DesktopIcon.background", ___control, "DesktopIcon.width", Convert.ToInt32(160), "Desktop.ancestorInputMap", New UIDefaults.LazyInputMap(New Object() { "ctrl F5", "restore", "ctrl F4", "close", "ctrl F7", "move", "ctrl F8", "resize", "RIGHT", "right", "KP_RIGHT", "right", "shift RIGHT", "shrinkRight", "shift KP_RIGHT", "shrinkRight", "LEFT", "left", "KP_LEFT", "left", "shift LEFT", "shrinkLeft", "shift KP_LEFT", "shrinkLeft", "UP", "up", "KP_UP", "up", "shift UP", "shrinkUp", "shift KP_UP", "shrinkUp", "DOWN", "down", "KP_DOWN", "down", "shift DOWN", "shrinkDown", "shift KP_DOWN", "shrinkDown", "ESCAPE", "escape", "ctrl F9", "minimize", "ctrl F10", "maximize", "ctrl F6", "selectNextFrame", "ctrl TAB", "selectNextFrame", "ctrl alt F6", "selectNextFrame", "shift ctrl alt F6", "selectPreviousFrame", "ctrl F12", "navigateNext", "shift ctrl F12", "navigatePrevious" }), "TitledBorder.font", controlTextValue, "TitledBorder.titleColor", ___systemTextColor, "TitledBorder.border", titledBorderBorder, "Label.font", controlTextValue, "Label.foreground", ___systemTextColor, "Label.disabledForeground", inactiveSystemTextColor, "List.font", controlTextValue, "List.focusCellHighlightBorder", focusCellHighlightBorder, "List.focusInputMap", New UIDefaults.LazyInputMap(New Object() { "ctrl C", "copy", "ctrl V", "paste", "ctrl X", "cut", "COPY", "copy", "PASTE", "paste", "CUT", "cut", "control INSERT", "copy", "shift INSERT", "paste", "shift DELETE", "cut", "UP", "selectPreviousRow", "KP_UP", "selectPreviousRow", "shift UP", "selectPreviousRowExtendSelection", "shift KP_UP", "selectPreviousRowExtendSelection", "ctrl shift UP", "selectPreviousRowExtendSelection", "ctrl shift KP_UP", "selectPreviousRowExtendSelection", "ctrl UP", "selectPreviousRowChangeLead", "ctrl KP_UP", "selectPreviousRowChangeLead", "DOWN", "selectNextRow", "KP_DOWN", "selectNextRow", "shift DOWN", "selectNextRowExtendSelection", "shift KP_DOWN", "selectNextRowExtendSelection", "ctrl shift DOWN", "selectNextRowExtendSelection", "ctrl shift KP_DOWN", "selectNextRowExtendSelection", "ctrl DOWN", "selectNextRowChangeLead", "ctrl KP_DOWN", "selectNextRowChangeLead", "LEFT", "selectPreviousColumn", "KP_LEFT", "selectPreviousColumn", "shift LEFT", "selectPreviousColumnExtendSelection", "shift KP_LEFT", "selectPreviousColumnExtendSelection", "ctrl shift LEFT", "selectPreviousColumnExtendSelection", "ctrl shift KP_LEFT", "selectPreviousColumnExtendSelection", "ctrl LEFT", "selectPreviousColumnChangeLead", "ctrl KP_LEFT", "selectPreviousColumnChangeLead", "RIGHT", "selectNextColumn", "KP_RIGHT", "selectNextColumn", "shift RIGHT", "selectNextColumnExtendSelection", "shift KP_RIGHT", "selectNextColumnExtendSelection", "ctrl shift RIGHT", "selectNextColumnExtendSelection", "ctrl shift KP_RIGHT", "selectNextColumnExtendSelection", "ctrl RIGHT", "selectNextColumnChangeLead", "ctrl KP_RIGHT", "selectNextColumnChangeLead", "HOME", "selectFirstRow", "shift HOME", "selectFirstRowExtendSelection", "ctrl shift HOME", "selectFirstRowExtendSelection", "ctrl HOME", "selectFirstRowChangeLead", "END", "selectLastRow", "shift END", "selectLastRowExtendSelection", "ctrl shift END", "selectLastRowExtendSelection", "ctrl END", "selectLastRowChangeLead", "PAGE_UP", "scrollUp", "shift PAGE_UP", "scrollUpExtendSelection", "ctrl shift PAGE_UP", "scrollUpExtendSelection", "ctrl PAGE_UP", "scrollUpChangeLead", "PAGE_DOWN", "scrollDown", "shift PAGE_DOWN", "scrollDownExtendSelection", "ctrl shift PAGE_DOWN", "scrollDownExtendSelection", "ctrl PAGE_DOWN", "scrollDownChangeLead", "ctrl A", "selectAll", "ctrl SLASH", "selectAll", "ctrl BACK_SLASH", "clearSelection", "SPACE", "addToSelection", "ctrl SPACE", "toggleAndAnchor", "shift SPACE", "extendTo", "ctrl shift SPACE", "moveSelectionTo" }), "ScrollBar.background", ___control, "ScrollBar.highlight", ___controlHighlight, "ScrollBar.shadow", ___controlShadow, "ScrollBar.darkShadow", ___controlDarkShadow, "ScrollBar.thumb", ___primaryControlShadow, "ScrollBar.thumbShadow", ___primaryControlDarkShadow, "ScrollBar.thumbHighlight", ___primaryControl, "ScrollBar.width", New Integer?(17), "ScrollBar.allowsAbsolutePositioning", Boolean.TRUE, "ScrollBar.ancestorInputMap", New UIDefaults.LazyInputMap(New Object() { "RIGHT", "positiveUnitIncrement", "KP_RIGHT", "positiveUnitIncrement", "DOWN", "positiveUnitIncrement", "KP_DOWN", "positiveUnitIncrement", "PAGE_DOWN", "positiveBlockIncrement", "LEFT", "negativeUnitIncrement", "KP_LEFT", "negativeUnitIncrement", "UP", "negativeUnitIncrement", "KP_UP", "negativeUnitIncrement", "PAGE_UP", "negativeBlockIncrement", "HOME", "minScroll", "END", "maxScroll" }), "ScrollPane.border", scrollPaneBorder, "ScrollPane.ancestorInputMap", New UIDefaults.LazyInputMap(New Object() { "RIGHT", "unitScrollRight", "KP_RIGHT", "unitScrollRight", "DOWN", "unitScrollDown", "KP_DOWN", "unitScrollDown", "LEFT", "unitScrollLeft", "KP_LEFT", "unitScrollLeft", "UP", "unitScrollUp", "KP_UP", "unitScrollUp", "PAGE_UP", "scrollUp", "PAGE_DOWN", "scrollDown", "ctrl PAGE_UP", "scrollLeft", "ctrl PAGE_DOWN", "scrollRight", "ctrl HOME", "scrollHome", "ctrl END", "scrollEnd" }), "TabbedPane.font", controlTextValue, "TabbedPane.tabAreaBackground", ___control, "TabbedPane.background", ___controlShadow, "TabbedPane.light", ___control, "TabbedPane.focus", ___primaryControlDarkShadow, "TabbedPane.selected", ___control, "TabbedPane.selectHighlight", ___controlHighlight, "TabbedPane.tabAreaInsets", tabbedPaneTabAreaInsets, "TabbedPane.tabInsets", tabbedPaneTabInsets, "TabbedPane.focusInputMap", New UIDefaults.LazyInputMap(New Object() { "RIGHT", "navigateRight", "KP_RIGHT", "navigateRight", "LEFT", "navigateLeft", "KP_LEFT", "navigateLeft", "UP", "navigateUp", "KP_UP", "navigateUp", "DOWN", "navigateDown", "KP_DOWN", "navigateDown", "ctrl DOWN", "requestFocusForVisibleComponent", "ctrl KP_DOWN", "requestFocusForVisibleComponent" }), "TabbedPane.ancestorInputMap", New UIDefaults.LazyInputMap(New Object() { "ctrl PAGE_DOWN", "navigatePageDown", "ctrl PAGE_UP", "navigatePageUp", "ctrl UP", "requestFocus", "ctrl KP_UP", "requestFocus" }), "Table.font", userTextValue, "Table.focusCellHighlightBorder", focusCellHighlightBorder, "Table.scrollPaneBorder", scrollPaneBorder, "Table.dropLineColor", ___focusColor, "Table.dropLineShortColor", ___primaryControlDarkShadow, "Table.gridColor", ___controlShadow, "Table.ancestorInputMap", New UIDefaults.LazyInputMap(New Object() { "ctrl C", "copy", "ctrl V", "paste", "ctrl X", "cut", "COPY", "copy", "PASTE", "paste", "CUT", "cut", "control INSERT", "copy", "shift INSERT", "paste", "shift DELETE", "cut", "RIGHT", "selectNextColumn", "KP_RIGHT", "selectNextColumn", "shift RIGHT", "selectNextColumnExtendSelection", "shift KP_RIGHT", "selectNextColumnExtendSelection", "ctrl shift RIGHT", "selectNextColumnExtendSelection", "ctrl shift KP_RIGHT", "selectNextColumnExtendSelection", "ctrl RIGHT", "selectNextColumnChangeLead", "ctrl KP_RIGHT", "selectNextColumnChangeLead", "LEFT", "selectPreviousColumn", "KP_LEFT", "selectPreviousColumn", "shift LEFT", "selectPreviousColumnExtendSelection", "shift KP_LEFT", "selectPreviousColumnExtendSelection", "ctrl shift LEFT", "selectPreviousColumnExtendSelection", "ctrl shift KP_LEFT", "selectPreviousColumnExtendSelection", "ctrl LEFT", "selectPreviousColumnChangeLead", "ctrl KP_LEFT", "selectPreviousColumnChangeLead", "DOWN", "selectNextRow", "KP_DOWN", "selectNextRow", "shift DOWN", "selectNextRowExtendSelection", "shift KP_DOWN", "selectNextRowExtendSelection", "ctrl shift DOWN", "selectNextRowExtendSelection", "ctrl shift KP_DOWN", "selectNextRowExtendSelection", "ctrl DOWN", "selectNextRowChangeLead", "ctrl KP_DOWN", "selectNextRowChangeLead", "UP", "selectPreviousRow", "KP_UP", "selectPreviousRow", "shift UP", "selectPreviousRowExtendSelection", "shift KP_UP", "selectPreviousRowExtendSelection", "ctrl shift UP", "selectPreviousRowExtendSelection", "ctrl shift KP_UP", "selectPreviousRowExtendSelection", "ctrl UP", "selectPreviousRowChangeLead", "ctrl KP_UP", "selectPreviousRowChangeLead", "HOME", "selectFirstColumn", "shift HOME", "selectFirstColumnExtendSelection", "ctrl shift HOME", "selectFirstRowExtendSelection", "ctrl HOME", "selectFirstRow", "END", "selectLastColumn", "shift END", "selectLastColumnExtendSelection", "ctrl shift END", "selectLastRowExtendSelection", "ctrl END", "selectLastRow", "PAGE_UP", "scrollUpChangeSelection", "shift PAGE_UP", "scrollUpExtendSelection", "ctrl shift PAGE_UP", "scrollLeftExtendSelection", "ctrl PAGE_UP", "scrollLeftChangeSelection", "PAGE_DOWN", "scrollDownChangeSelection", "shift PAGE_DOWN", "scrollDownExtendSelection", "ctrl shift PAGE_DOWN", "scrollRightExtendSelection", "ctrl PAGE_DOWN", "scrollRightChangeSelection", "TAB", "selectNextColumnCell", "shift TAB", "selectPreviousColumnCell", "ENTER", "selectNextRowCell", "shift ENTER", "selectPreviousRowCell", "ctrl A", "selectAll", "ctrl SLASH", "selectAll", "ctrl BACK_SLASH", "clearSelection", "ESCAPE", "cancel", "F2", "startEditing", "SPACE", "addToSelection", "ctrl SPACE", "toggleAndAnchor", "shift SPACE", "extendTo", "ctrl shift SPACE", "moveSelectionTo", "F8", "focusHeader" }), "Table.ascendingSortIcon", sun.swing.SwingUtilities2.makeIcon(Me.GetType(), GetType(MetalLookAndFeel), "icons/sortUp.png"), "Table.descendingSortIcon", sun.swing.SwingUtilities2.makeIcon(Me.GetType(), GetType(MetalLookAndFeel), "icons/sortDown.png"), "TableHeader.font", userTextValue, "TableHeader.cellBorder", New sun.swing.SwingLazyValue("javax.swing.plaf.metal.MetalBorders$TableHeaderBorder"), "MenuBar.border", menuBarBorder, "MenuBar.font", menuTextValue, "MenuBar.windowBindings", New Object() { "F10", "takeFocus" }, "Menu.border", menuItemBorder, "Menu.borderPainted", Boolean.TRUE, "Menu.menuPopupOffsetX", zero, "Menu.menuPopupOffsetY", zero, "Menu.submenuPopupOffsetX", New Integer?(-4), "Menu.submenuPopupOffsetY", New Integer?(-3), "Menu.font", menuTextValue, "Menu.selectionForeground", ___menuSelectedForeground, "Menu.selectionBackground", ___menuSelectedBackground, "Menu.disabledForeground", ___menuDisabledForeground, "Menu.acceleratorFont", subTextValue, "Menu.acceleratorForeground", ___acceleratorForeground, "Menu.acceleratorSelectionForeground", ___acceleratorSelectedForeground, "Menu.checkIcon", New sun.swing.SwingLazyValue("javax.swing.plaf.metal.MetalIconFactory", "getMenuItemCheckIcon"), "Menu.arrowIcon", New sun.swing.SwingLazyValue("javax.swing.plaf.metal.MetalIconFactory", "getMenuArrowIcon"), "MenuItem.border", menuItemBorder, "MenuItem.borderPainted", Boolean.TRUE, "MenuItem.font", menuTextValue, "MenuItem.selectionForeground", ___menuSelectedForeground, "MenuItem.selectionBackground", ___menuSelectedBackground, "MenuItem.disabledForeground", ___menuDisabledForeground, "MenuItem.acceleratorFont", subTextValue, "MenuItem.acceleratorForeground", ___acceleratorForeground, "MenuItem.acceleratorSelectionForeground", ___acceleratorSelectedForeground, "MenuItem.acceleratorDelimiter", menuItemAcceleratorDelimiter, "MenuItem.checkIcon", New sun.swing.SwingLazyValue("javax.swing.plaf.metal.MetalIconFactory", "getMenuItemCheckIcon"), "MenuItem.arrowIcon", New sun.swing.SwingLazyValue("javax.swing.plaf.metal.MetalIconFactory", "getMenuItemArrowIcon"), "MenuItem.commandSound", "sounds/MenuItemCommand.wav", "OptionPane.windowBindings", New Object() { "ESCAPE", "close" }, "OptionPane.informationSound", "sounds/OptionPaneInformation.wav", "OptionPane.warningSound", "sounds/OptionPaneWarning.wav", "OptionPane.errorSound", "sounds/OptionPaneError.wav", "OptionPane.questionSound", "sounds/OptionPaneQuestion.wav", "OptionPane.errorDialog.border.background", New ColorUIResource(153, 51, 51), "OptionPane.errorDialog.titlePane.foreground", New ColorUIResource(51, 0, 0), "OptionPane.errorDialog.titlePane.background", New ColorUIResource(255, 153, 153), "OptionPane.errorDialog.titlePane.shadow", New ColorUIResource(204, 102, 102), "OptionPane.questionDialog.border.background", New ColorUIResource(51, 102, 51), "OptionPane.questionDialog.titlePane.foreground", New ColorUIResource(0, 51, 0), "OptionPane.questionDialog.titlePane.background", New ColorUIResource(153, 204, 153), "OptionPane.questionDialog.titlePane.shadow", New ColorUIResource(102, 153, 102), "OptionPane.warningDialog.border.background", New ColorUIResource(153, 102, 51), "OptionPane.warningDialog.titlePane.foreground", New ColorUIResource(102, 51, 0), "OptionPane.warningDialog.titlePane.background", New ColorUIResource(255, 204, 153), "OptionPane.warningDialog.titlePane.shadow", New ColorUIResource(204, 153, 102), "Separator.background", separatorBackground, "Separator.foreground", separatorForeground, "PopupMenu.border", popupMenuBorder, "PopupMenu.popupSound", "sounds/PopupMenuPopup.wav", "PopupMenu.font", menuTextValue, "CheckBoxMenuItem.border", menuItemBorder, "CheckBoxMenuItem.borderPainted", Boolean.TRUE, "CheckBoxMenuItem.font", menuTextValue, "CheckBoxMenuItem.selectionForeground", ___menuSelectedForeground, "CheckBoxMenuItem.selectionBackground", ___menuSelectedBackground, "CheckBoxMenuItem.disabledForeground", ___menuDisabledForeground, "CheckBoxMenuItem.acceleratorFont", subTextValue, "CheckBoxMenuItem.acceleratorForeground", ___acceleratorForeground, "CheckBoxMenuItem.acceleratorSelectionForeground", ___acceleratorSelectedForeground, "CheckBoxMenuItem.checkIcon", New sun.swing.SwingLazyValue("javax.swing.plaf.metal.MetalIconFactory", "getCheckBoxMenuItemIcon"), "CheckBoxMenuItem.arrowIcon", New sun.swing.SwingLazyValue("javax.swing.plaf.metal.MetalIconFactory", "getMenuItemArrowIcon"), "CheckBoxMenuItem.commandSound", "sounds/MenuItemCommand.wav", "RadioButtonMenuItem.border", menuItemBorder, "RadioButtonMenuItem.borderPainted", Boolean.TRUE, "RadioButtonMenuItem.font", menuTextValue, "RadioButtonMenuItem.selectionForeground", ___menuSelectedForeground, "RadioButtonMenuItem.selectionBackground", ___menuSelectedBackground, "RadioButtonMenuItem.disabledForeground", ___menuDisabledForeground, "RadioButtonMenuItem.acceleratorFont", subTextValue, "RadioButtonMenuItem.acceleratorForeground", ___acceleratorForeground, "RadioButtonMenuItem.acceleratorSelectionForeground", ___acceleratorSelectedForeground, "RadioButtonMenuItem.checkIcon", New sun.swing.SwingLazyValue("javax.swing.plaf.metal.MetalIconFactory", "getRadioButtonMenuItemIcon"), "RadioButtonMenuItem.arrowIcon", New sun.swing.SwingLazyValue("javax.swing.plaf.metal.MetalIconFactory", "getMenuItemArrowIcon"), "RadioButtonMenuItem.commandSound", "sounds/MenuItemCommand.wav", "Spinner.ancestorInputMap", New UIDefaults.LazyInputMap(New Object() { "UP", "increment", "KP_UP", "increment", "DOWN", "decrement", "KP_DOWN", "decrement" }), "Spinner.arrowButtonInsets", zeroInsets, "Spinner.border", textFieldBorder, "Spinner.arrowButtonBorder", buttonBorder, "Spinner.font", controlTextValue, "SplitPane.dividerSize", New Integer?(10), "SplitPane.ancestorInputMap", New UIDefaults.LazyInputMap(New Object() { "UP", "negativeIncrement", "DOWN", "positiveIncrement", "LEFT", "negativeIncrement", "RIGHT", "positiveIncrement", "KP_UP", "negativeIncrement", "KP_DOWN", "positiveIncrement", "KP_LEFT", "negativeIncrement", "KP_RIGHT", "positiveIncrement", "HOME", "selectMin", "END", "selectMax", "F8", "startResize", "F6", "toggleFocus", "ctrl TAB", "focusOutForward", "ctrl shift TAB", "focusOutBackward" }), "SplitPane.centerOneTouchButtons", Boolean.FALSE, "SplitPane.dividerFocusColor", ___primaryControl, "Tree.font", userTextValue, "Tree.textBackground", windowBackground, "Tree.selectionBorderColor", ___focusColor, "Tree.openIcon", New sun.swing.SwingLazyValue("javax.swing.plaf.metal.MetalIconFactory", "getTreeFolderIcon"), "Tree.closedIcon", New sun.swing.SwingLazyValue("javax.swing.plaf.metal.MetalIconFactory", "getTreeFolderIcon"), "Tree.leafIcon", New sun.swing.SwingLazyValue("javax.swing.plaf.metal.MetalIconFactory", "getTreeLeafIcon"), "Tree.expandedIcon", New sun.swing.SwingLazyValue("javax.swing.plaf.metal.MetalIconFactory", "getTreeControlIcon", New Object() {Convert.ToBoolean(MetalIconFactory.DARK)}), "Tree.collapsedIcon", New sun.swing.SwingLazyValue("javax.swing.plaf.metal.MetalIconFactory", "getTreeControlIcon", New Object() {Convert.ToBoolean(MetalIconFactory.LIGHT)}), "Tree.line", ___primaryControl, "Tree.hash", ___primaryControl, "Tree.rowHeight", zero, "Tree.focusInputMap", New UIDefaults.LazyInputMap(New Object() { "ADD", "expand", "SUBTRACT", "collapse", "ctrl C", "copy", "ctrl V", "paste", "ctrl X", "cut", "COPY", "copy", "PASTE", "paste", "CUT", "cut", "control INSERT", "copy", "shift INSERT", "paste", "shift DELETE", "cut", "UP", "selectPrevious", "KP_UP", "selectPrevious", "shift UP", "selectPreviousExtendSelection", "shift KP_UP", "selectPreviousExtendSelection", "ctrl shift UP", "selectPreviousExtendSelection", "ctrl shift KP_UP", "selectPreviousExtendSelection", "ctrl UP", "selectPreviousChangeLead", "ctrl KP_UP", "selectPreviousChangeLead", "DOWN", "selectNext", "KP_DOWN", "selectNext", "shift DOWN", "selectNextExtendSelection", "shift KP_DOWN", "selectNextExtendSelection", "ctrl shift DOWN", "selectNextExtendSelection", "ctrl shift KP_DOWN", "selectNextExtendSelection", "ctrl DOWN", "selectNextChangeLead", "ctrl KP_DOWN", "selectNextChangeLead", "RIGHT", "selectChild", "KP_RIGHT", "selectChild", "LEFT", "selectParent", "KP_LEFT", "selectParent", "PAGE_UP", "scrollUpChangeSelection", "shift PAGE_UP", "scrollUpExtendSelection", "ctrl shift PAGE_UP", "scrollUpExtendSelection", "ctrl PAGE_UP", "scrollUpChangeLead", "PAGE_DOWN", "scrollDownChangeSelection", "shift PAGE_DOWN", "scrollDownExtendSelection", "ctrl shift PAGE_DOWN", "scrollDownExtendSelection", "ctrl PAGE_DOWN", "scrollDownChangeLead", "HOME", "selectFirst", "shift HOME", "selectFirstExtendSelection", "ctrl shift HOME", "selectFirstExtendSelection", "ctrl HOME", "selectFirstChangeLead", "END", "selectLast", "shift END", "selectLastExtendSelection", "ctrl shift END", "selectLastExtendSelection", "ctrl END", "selectLastChangeLead", "F2", "startEditing", "ctrl A", "selectAll", "ctrl SLASH", "selectAll", "ctrl BACK_SLASH", "clearSelection", "ctrl LEFT", "scrollLeft", "ctrl KP_LEFT", "scrollLeft", "ctrl RIGHT", "scrollRight", "ctrl KP_RIGHT", "scrollRight", "SPACE", "addToSelection", "ctrl SPACE", "toggleAndAnchor", "shift SPACE", "extendTo", "ctrl shift SPACE", "moveSelectionTo" }), "Tree.ancestorInputMap", New UIDefaults.LazyInputMap(New Object() { "ESCAPE", "cancel" }), "ToolBar.border", toolBarBorder, "ToolBar.background", ___menuBackground, "ToolBar.foreground", menuForeground, "ToolBar.font", menuTextValue, "ToolBar.dockingBackground", ___menuBackground, "ToolBar.floatingBackground", ___menuBackground, "ToolBar.dockingForeground", ___primaryControlDarkShadow, "ToolBar.floatingForeground", ___primaryControl, "ToolBar.rolloverBorder", CType(t, javax.swing.UIDefaults.LazyValue) -> MetalBorders.toolBarRolloverBorder, "ToolBar.nonrolloverBorder", CType(t, javax.swing.UIDefaults.LazyValue) -> MetalBorders.toolBarNonrolloverBorder, "ToolBar.ancestorInputMap", New UIDefaults.LazyInputMap(New Object() { "UP", "navigateUp", "KP_UP", "navigateUp", "DOWN", "navigateDown", "KP_DOWN", "navigateDown", "LEFT", "navigateLeft", "KP_LEFT", "navigateLeft", "RIGHT", "navigateRight", "KP_RIGHT", "navigateRight" }), "RootPane.frameBorder", CType(t, javax.swing.UIDefaults.LazyValue) -> New MetalBorders.FrameBorder, "RootPane.plainDialogBorder", dialogBorder, "RootPane.informationDialogBorder", dialogBorder, "RootPane.errorDialogBorder", CType(t, javax.swing.UIDefaults.LazyValue) -> New MetalBorders.ErrorDialogBorder, "RootPane.colorChooserDialogBorder", questionDialogBorder, "RootPane.fileChooserDialogBorder", questionDialogBorder, "RootPane.questionDialogBorder", questionDialogBorder, "RootPane.warningDialogBorder", CType(t, javax.swing.UIDefaults.LazyValue) -> New MetalBorders.WarningDialogBorder, "RootPane.defaultButtonWindowKeyBindings", New Object() { "ENTER", "press", "released ENTER", "release", "ctrl ENTER", "press", "ctrl released ENTER", "release" } }

			table.putDefaults(___defaults)

			If windows AndAlso useSystemFonts() AndAlso theme.systemTheme Then
				Dim messageFont As Object = New MetalFontDesktopProperty("win.messagebox.font.height", MetalTheme.CONTROL_TEXT_FONT)

				___defaults = New Object() { "OptionPane.messageFont", messageFont, "OptionPane.buttonFont", messageFont }
				table.putDefaults(___defaults)
			End If

			flushUnreferenced() ' Remove old listeners

			Dim lafCond As Boolean = sun.swing.SwingUtilities2.localDisplay
			Dim aaTextInfo As Object = sun.swing.SwingUtilities2.AATextInfo.getAATextInfo(lafCond)
			table(sun.swing.SwingUtilities2.AA_TEXT_PROPERTY_KEY) = aaTextInfo
			Dim TempAATextListener As AATextListener = New AATextListener(Me)
		End Sub

		''' <summary>
		''' Ensures the current {@code MetalTheme} is {@code non-null}. This is
		''' a cover method for {@code getCurrentTheme}.
		''' </summary>
		''' <seealso cref= #getCurrentTheme </seealso>
		Protected Friend Overridable Sub createDefaultTheme()
			currentTheme
		End Sub

		''' <summary>
		''' Returns the look and feel defaults. This invokes, in order,
		''' {@code createDefaultTheme()}, {@code super.getDefaults()} and
		''' {@code getCurrentTheme().addCustomEntriesToTable(table)}.
		''' <p>
		''' While this method is public, it should only be invoked by the
		''' {@code UIManager} when the look and feel is set as the current
		''' look and feel and after {@code initialize} has been invoked.
		''' </summary>
		''' <returns> the look and feel defaults
		''' </returns>
		''' <seealso cref= #createDefaultTheme </seealso>
		''' <seealso cref= javax.swing.plaf.basic.BasicLookAndFeel#getDefaults() </seealso>
		''' <seealso cref= MetalTheme#addCustomEntriesToTable(UIDefaults) </seealso>
		Public Property Overrides defaults As UIDefaults
			Get
				' PENDING: move this to initialize when API changes are allowed
				METAL_LOOK_AND_FEEL_INITED = True
    
				createDefaultTheme()
				Dim table As UIDefaults = MyBase.defaults
				Dim ___currentTheme As MetalTheme = currentTheme
				___currentTheme.addCustomEntriesToTable(table)
				___currentTheme.install()
				Return table
			End Get
		End Property

		''' <summary>
		''' {@inheritDoc}
		''' 
		''' @since 1.4
		''' </summary>
		Public Overridable Sub provideErrorFeedback(ByVal component As Component)
			MyBase.provideErrorFeedback(component)
		End Sub

		''' <summary>
		''' Set the theme used by <code>MetalLookAndFeel</code>.
		''' <p>
		''' After the theme is set, {@code MetalLookAndFeel} needs to be
		''' re-installed and the uis need to be recreated. The following
		''' shows how to do this:
		''' <pre>
		'''   MetalLookAndFeel.setCurrentTheme(theme);
		''' 
		'''   // re-install the Metal Look and Feel
		'''   UIManager.setLookAndFeel(new MetalLookAndFeel());
		''' 
		'''   // Update the ComponentUIs for all Components. This
		'''   // needs to be invoked for all windows.
		'''   SwingUtilities.updateComponentTreeUI(rootComponent);
		''' </pre>
		''' If this is not done the results are undefined.
		''' </summary>
		''' <param name="theme"> the theme to use </param>
		''' <exception cref="NullPointerException"> if {@code theme} is {@code null} </exception>
		''' <seealso cref= #getCurrentTheme </seealso>
		Public Shared Property currentTheme As MetalTheme
			Set(ByVal theme As MetalTheme)
				' NOTE: because you need to recreate the look and feel after
				' this step, we don't bother blowing away any potential windows
				' values.
				If theme Is Nothing Then Throw New NullPointerException("Can't have null theme")
				AppContext.appContext.put("currentMetalTheme", theme)
			End Set
			Get
				Dim ___currentTheme As MetalTheme
				Dim context As AppContext = AppContext.appContext
				___currentTheme = CType(context.get("currentMetalTheme"), MetalTheme)
				If ___currentTheme Is Nothing Then
					' This will happen in two cases:
					' . When MetalLookAndFeel is first being initialized.
					' . When a new AppContext has been created that hasn't
					'   triggered UIManager to load a LAF. Rather than invoke
					'   a method on the UIManager, which would trigger the loading
					'   of a potentially different LAF, we directly set the
					'   Theme here.
					If useHighContrastTheme() Then
						___currentTheme = New MetalHighContrastTheme
					Else
						' Create the default theme. We prefer Ocean, but will
						' use DefaultMetalTheme if told to.
						Dim theme As String = java.security.AccessController.doPrivileged(New sun.security.action.GetPropertyAction("swing.metalTheme"))
						If "steel".Equals(theme) Then
							___currentTheme = New DefaultMetalTheme
						Else
							___currentTheme = New OceanTheme
						End If
					End If
					currentTheme = ___currentTheme
				End If
				Return ___currentTheme
			End Get
		End Property


		''' <summary>
		''' Returns an <code>Icon</code> with a disabled appearance.
		''' This method is used to generate a disabled <code>Icon</code> when
		''' one has not been specified.  For example, if you create a
		''' <code>JButton</code> and only specify an <code>Icon</code> via
		''' <code>setIcon</code> this method will be called to generate the
		''' disabled <code>Icon</code>. If null is passed as <code>icon</code>
		''' this method returns null.
		''' <p>
		''' Some look and feels might not render the disabled Icon, in which
		''' case they will ignore this.
		''' </summary>
		''' <param name="component"> JComponent that will display the Icon, may be null </param>
		''' <param name="icon"> Icon to generate disable icon from. </param>
		''' <returns> Disabled icon, or null if a suitable Icon can not be
		'''         generated.
		''' @since 1.5 </returns>
		Public Overrides Function getDisabledIcon(ByVal component As JComponent, ByVal icon As Icon) As Icon
			If (TypeOf icon Is ImageIcon) AndAlso MetalLookAndFeel.usingOcean() Then Return MetalUtils.getOceanDisabledButtonIcon(CType(icon, ImageIcon).image)
			Return MyBase.getDisabledIcon(component, icon)
		End Function

		''' <summary>
		''' Returns an <code>Icon</code> for use by disabled
		''' components that are also selected. This method is used to generate an
		''' <code>Icon</code> for components that are in both the disabled and
		''' selected states but do not have a specific <code>Icon</code> for this
		''' state.  For example, if you create a <code>JButton</code> and only
		''' specify an <code>Icon</code> via <code>setIcon</code> this method
		''' will be called to generate the disabled and selected
		''' <code>Icon</code>. If null is passed as <code>icon</code> this method
		''' returns null.
		''' <p>
		''' Some look and feels might not render the disabled and selected Icon,
		''' in which case they will ignore this.
		''' </summary>
		''' <param name="component"> JComponent that will display the Icon, may be null </param>
		''' <param name="icon"> Icon to generate disabled and selected icon from. </param>
		''' <returns> Disabled and Selected icon, or null if a suitable Icon can not
		'''         be generated.
		''' @since 1.5 </returns>
		Public Overrides Function getDisabledSelectedIcon(ByVal component As JComponent, ByVal icon As Icon) As Icon
			If (TypeOf icon Is ImageIcon) AndAlso MetalLookAndFeel.usingOcean() Then Return MetalUtils.getOceanDisabledButtonIcon(CType(icon, ImageIcon).image)
			Return MyBase.getDisabledSelectedIcon(component, icon)
		End Function

		''' <summary>
		''' Returns the control text font of the current theme. This is a
		''' cover method for {@code getCurrentTheme().getControlTextColor()}.
		''' </summary>
		''' <returns> the control text font
		''' </returns>
		''' <seealso cref= MetalTheme </seealso>
		Public Property Shared controlTextFont As FontUIResource
			Get
				Return currentTheme.controlTextFont
			End Get
		End Property

		''' <summary>
		''' Returns the system text font of the current theme. This is a
		''' cover method for {@code getCurrentTheme().getSystemTextFont()}.
		''' </summary>
		''' <returns> the system text font
		''' </returns>
		''' <seealso cref= MetalTheme </seealso>
		Public Property Shared systemTextFont As FontUIResource
			Get
				Return currentTheme.systemTextFont
			End Get
		End Property

		''' <summary>
		''' Returns the user text font of the current theme. This is a
		''' cover method for {@code getCurrentTheme().getUserTextFont()}.
		''' </summary>
		''' <returns> the user text font
		''' </returns>
		''' <seealso cref= MetalTheme </seealso>
		Public Property Shared userTextFont As FontUIResource
			Get
				Return currentTheme.userTextFont
			End Get
		End Property

		''' <summary>
		''' Returns the menu text font of the current theme. This is a
		''' cover method for {@code getCurrentTheme().getMenuTextFont()}.
		''' </summary>
		''' <returns> the menu text font
		''' </returns>
		''' <seealso cref= MetalTheme </seealso>
		Public Property Shared menuTextFont As FontUIResource
			Get
				Return currentTheme.menuTextFont
			End Get
		End Property

		''' <summary>
		''' Returns the window title font of the current theme. This is a
		''' cover method for {@code getCurrentTheme().getWindowTitleFont()}.
		''' </summary>
		''' <returns> the window title font
		''' </returns>
		''' <seealso cref= MetalTheme </seealso>
		Public Property Shared windowTitleFont As FontUIResource
			Get
				Return currentTheme.windowTitleFont
			End Get
		End Property

		''' <summary>
		''' Returns the sub-text font of the current theme. This is a
		''' cover method for {@code getCurrentTheme().getSubTextFont()}.
		''' </summary>
		''' <returns> the sub-text font
		''' </returns>
		''' <seealso cref= MetalTheme </seealso>
		Public Property Shared subTextFont As FontUIResource
			Get
				Return currentTheme.subTextFont
			End Get
		End Property

		''' <summary>
		''' Returns the desktop color of the current theme. This is a
		''' cover method for {@code getCurrentTheme().getDesktopColor()}.
		''' </summary>
		''' <returns> the desktop color
		''' </returns>
		''' <seealso cref= MetalTheme </seealso>
		Public Property Shared desktopColor As ColorUIResource
			Get
				Return currentTheme.desktopColor
			End Get
		End Property

		''' <summary>
		''' Returns the focus color of the current theme. This is a
		''' cover method for {@code getCurrentTheme().getFocusColor()}.
		''' </summary>
		''' <returns> the focus color
		''' </returns>
		''' <seealso cref= MetalTheme </seealso>
		Public Property Shared focusColor As ColorUIResource
			Get
				Return currentTheme.focusColor
			End Get
		End Property

		''' <summary>
		''' Returns the white color of the current theme. This is a
		''' cover method for {@code getCurrentTheme().getWhite()}.
		''' </summary>
		''' <returns> the white color
		''' </returns>
		''' <seealso cref= MetalTheme </seealso>
		Public Property Shared white As ColorUIResource
			Get
				Return currentTheme.white
			End Get
		End Property

		''' <summary>
		''' Returns the black color of the current theme. This is a
		''' cover method for {@code getCurrentTheme().getBlack()}.
		''' </summary>
		''' <returns> the black color
		''' </returns>
		''' <seealso cref= MetalTheme </seealso>
		Public Property Shared black As ColorUIResource
			Get
				Return currentTheme.black
			End Get
		End Property

		''' <summary>
		''' Returns the control color of the current theme. This is a
		''' cover method for {@code getCurrentTheme().getControl()}.
		''' </summary>
		''' <returns> the control color
		''' </returns>
		''' <seealso cref= MetalTheme </seealso>
		Public Property Shared control As ColorUIResource
			Get
				Return currentTheme.control
			End Get
		End Property

		''' <summary>
		''' Returns the control shadow color of the current theme. This is a
		''' cover method for {@code getCurrentTheme().getControlShadow()}.
		''' </summary>
		''' <returns> the control shadow color
		''' </returns>
		''' <seealso cref= MetalTheme </seealso>
		Public Property Shared controlShadow As ColorUIResource
			Get
				Return currentTheme.controlShadow
			End Get
		End Property

		''' <summary>
		''' Returns the control dark shadow color of the current theme. This is a
		''' cover method for {@code getCurrentTheme().getControlDarkShadow()}.
		''' </summary>
		''' <returns> the control dark shadow color
		''' </returns>
		''' <seealso cref= MetalTheme </seealso>
		Public Property Shared controlDarkShadow As ColorUIResource
			Get
				Return currentTheme.controlDarkShadow
			End Get
		End Property

		''' <summary>
		''' Returns the control info color of the current theme. This is a
		''' cover method for {@code getCurrentTheme().getControlInfo()}.
		''' </summary>
		''' <returns> the control info color
		''' </returns>
		''' <seealso cref= MetalTheme </seealso>
		Public Property Shared controlInfo As ColorUIResource
			Get
				Return currentTheme.controlInfo
			End Get
		End Property

		''' <summary>
		''' Returns the control highlight color of the current theme. This is a
		''' cover method for {@code getCurrentTheme().getControlHighlight()}.
		''' </summary>
		''' <returns> the control highlight color
		''' </returns>
		''' <seealso cref= MetalTheme </seealso>
		Public Property Shared controlHighlight As ColorUIResource
			Get
				Return currentTheme.controlHighlight
			End Get
		End Property

		''' <summary>
		''' Returns the control disabled color of the current theme. This is a
		''' cover method for {@code getCurrentTheme().getControlDisabled()}.
		''' </summary>
		''' <returns> the control disabled color
		''' </returns>
		''' <seealso cref= MetalTheme </seealso>
		Public Property Shared controlDisabled As ColorUIResource
			Get
				Return currentTheme.controlDisabled
			End Get
		End Property

		''' <summary>
		''' Returns the primary control color of the current theme. This is a
		''' cover method for {@code getCurrentTheme().getPrimaryControl()}.
		''' </summary>
		''' <returns> the primary control color
		''' </returns>
		''' <seealso cref= MetalTheme </seealso>
		Public Property Shared primaryControl As ColorUIResource
			Get
				Return currentTheme.primaryControl
			End Get
		End Property

		''' <summary>
		''' Returns the primary control shadow color of the current theme. This is a
		''' cover method for {@code getCurrentTheme().getPrimaryControlShadow()}.
		''' </summary>
		''' <returns> the primary control shadow color
		''' </returns>
		''' <seealso cref= MetalTheme </seealso>
		Public Property Shared primaryControlShadow As ColorUIResource
			Get
				Return currentTheme.primaryControlShadow
			End Get
		End Property

		''' <summary>
		''' Returns the primary control dark shadow color of the current
		''' theme. This is a cover method for {@code
		''' getCurrentTheme().getPrimaryControlDarkShadow()}.
		''' </summary>
		''' <returns> the primary control dark shadow color
		''' </returns>
		''' <seealso cref= MetalTheme </seealso>
		Public Property Shared primaryControlDarkShadow As ColorUIResource
			Get
				Return currentTheme.primaryControlDarkShadow
			End Get
		End Property

		''' <summary>
		''' Returns the primary control info color of the current theme. This is a
		''' cover method for {@code getCurrentTheme().getPrimaryControlInfo()}.
		''' </summary>
		''' <returns> the primary control info color
		''' </returns>
		''' <seealso cref= MetalTheme </seealso>
		Public Property Shared primaryControlInfo As ColorUIResource
			Get
				Return currentTheme.primaryControlInfo
			End Get
		End Property

		''' <summary>
		''' Returns the primary control highlight color of the current
		''' theme. This is a cover method for {@code
		''' getCurrentTheme().getPrimaryControlHighlight()}.
		''' </summary>
		''' <returns> the primary control highlight color
		''' </returns>
		''' <seealso cref= MetalTheme </seealso>
		Public Property Shared primaryControlHighlight As ColorUIResource
			Get
				Return currentTheme.primaryControlHighlight
			End Get
		End Property

		''' <summary>
		''' Returns the system text color of the current theme. This is a
		''' cover method for {@code getCurrentTheme().getSystemTextColor()}.
		''' </summary>
		''' <returns> the system text color
		''' </returns>
		''' <seealso cref= MetalTheme </seealso>
		Public Property Shared systemTextColor As ColorUIResource
			Get
				Return currentTheme.systemTextColor
			End Get
		End Property

		''' <summary>
		''' Returns the control text color of the current theme. This is a
		''' cover method for {@code getCurrentTheme().getControlTextColor()}.
		''' </summary>
		''' <returns> the control text color
		''' </returns>
		''' <seealso cref= MetalTheme </seealso>
		Public Property Shared controlTextColor As ColorUIResource
			Get
				Return currentTheme.controlTextColor
			End Get
		End Property

		''' <summary>
		''' Returns the inactive control text color of the current theme. This is a
		''' cover method for {@code
		''' getCurrentTheme().getInactiveControlTextColor()}.
		''' </summary>
		''' <returns> the inactive control text color
		''' </returns>
		''' <seealso cref= MetalTheme </seealso>
		Public Property Shared inactiveControlTextColor As ColorUIResource
			Get
				Return currentTheme.inactiveControlTextColor
			End Get
		End Property

		''' <summary>
		''' Returns the inactive system text color of the current theme. This is a
		''' cover method for {@code
		''' getCurrentTheme().getInactiveSystemTextColor()}.
		''' </summary>
		''' <returns> the inactive system text color
		''' </returns>
		''' <seealso cref= MetalTheme </seealso>
		Public Property Shared inactiveSystemTextColor As ColorUIResource
			Get
				Return currentTheme.inactiveSystemTextColor
			End Get
		End Property

		''' <summary>
		''' Returns the user text color of the current theme. This is a
		''' cover method for {@code getCurrentTheme().getUserTextColor()}.
		''' </summary>
		''' <returns> the user text color
		''' </returns>
		''' <seealso cref= MetalTheme </seealso>
		Public Property Shared userTextColor As ColorUIResource
			Get
				Return currentTheme.userTextColor
			End Get
		End Property

		''' <summary>
		''' Returns the text highlight color of the current theme. This is a
		''' cover method for {@code getCurrentTheme().getTextHighlightColor()}.
		''' </summary>
		''' <returns> the text highlight color
		''' </returns>
		''' <seealso cref= MetalTheme </seealso>
		Public Property Shared textHighlightColor As ColorUIResource
			Get
				Return currentTheme.textHighlightColor
			End Get
		End Property

		''' <summary>
		''' Returns the highlighted text color of the current theme. This is a
		''' cover method for {@code getCurrentTheme().getHighlightedTextColor()}.
		''' </summary>
		''' <returns> the highlighted text color
		''' </returns>
		''' <seealso cref= MetalTheme </seealso>
		Public Property Shared highlightedTextColor As ColorUIResource
			Get
				Return currentTheme.highlightedTextColor
			End Get
		End Property

		''' <summary>
		''' Returns the window background color of the current theme. This is a
		''' cover method for {@code getCurrentTheme().getWindowBackground()}.
		''' </summary>
		''' <returns> the window background color
		''' </returns>
		''' <seealso cref= MetalTheme </seealso>
		Public Property Shared windowBackground As ColorUIResource
			Get
				Return currentTheme.windowBackground
			End Get
		End Property

		''' <summary>
		''' Returns the window title background color of the current
		''' theme. This is a cover method for {@code
		''' getCurrentTheme().getWindowTitleBackground()}.
		''' </summary>
		''' <returns> the window title background color
		''' </returns>
		''' <seealso cref= MetalTheme </seealso>
		Public Property Shared windowTitleBackground As ColorUIResource
			Get
				Return currentTheme.windowTitleBackground
			End Get
		End Property

		''' <summary>
		''' Returns the window title foreground color of the current
		''' theme. This is a cover method for {@code
		''' getCurrentTheme().getWindowTitleForeground()}.
		''' </summary>
		''' <returns> the window title foreground color
		''' </returns>
		''' <seealso cref= MetalTheme </seealso>
		Public Property Shared windowTitleForeground As ColorUIResource
			Get
				Return currentTheme.windowTitleForeground
			End Get
		End Property

		''' <summary>
		''' Returns the window title inactive background color of the current
		''' theme. This is a cover method for {@code
		''' getCurrentTheme().getWindowTitleInactiveBackground()}.
		''' </summary>
		''' <returns> the window title inactive background color
		''' </returns>
		''' <seealso cref= MetalTheme </seealso>
		Public Property Shared windowTitleInactiveBackground As ColorUIResource
			Get
				Return currentTheme.windowTitleInactiveBackground
			End Get
		End Property

		''' <summary>
		''' Returns the window title inactive foreground color of the current
		''' theme. This is a cover method for {@code
		''' getCurrentTheme().getWindowTitleInactiveForeground()}.
		''' </summary>
		''' <returns> the window title inactive foreground color
		''' </returns>
		''' <seealso cref= MetalTheme </seealso>
		Public Property Shared windowTitleInactiveForeground As ColorUIResource
			Get
				Return currentTheme.windowTitleInactiveForeground
			End Get
		End Property

		''' <summary>
		''' Returns the menu background color of the current theme. This is
		''' a cover method for {@code getCurrentTheme().getMenuBackground()}.
		''' </summary>
		''' <returns> the menu background color
		''' </returns>
		''' <seealso cref= MetalTheme </seealso>
		Public Property Shared menuBackground As ColorUIResource
			Get
				Return currentTheme.menuBackground
			End Get
		End Property

		''' <summary>
		''' Returns the menu foreground color of the current theme. This is
		''' a cover method for {@code getCurrentTheme().getMenuForeground()}.
		''' </summary>
		''' <returns> the menu foreground color
		''' </returns>
		''' <seealso cref= MetalTheme </seealso>
		Public Property Shared menuForeground As ColorUIResource
			Get
				Return currentTheme.menuForeground
			End Get
		End Property

		''' <summary>
		''' Returns the menu selected background color of the current theme. This is
		''' a cover method for
		''' {@code getCurrentTheme().getMenuSelectedBackground()}.
		''' </summary>
		''' <returns> the menu selected background color
		''' </returns>
		''' <seealso cref= MetalTheme </seealso>
		Public Property Shared menuSelectedBackground As ColorUIResource
			Get
				Return currentTheme.menuSelectedBackground
			End Get
		End Property

		''' <summary>
		''' Returns the menu selected foreground color of the current theme. This is
		''' a cover method for
		''' {@code getCurrentTheme().getMenuSelectedForeground()}.
		''' </summary>
		''' <returns> the menu selected foreground color
		''' </returns>
		''' <seealso cref= MetalTheme </seealso>
		Public Property Shared menuSelectedForeground As ColorUIResource
			Get
				Return currentTheme.menuSelectedForeground
			End Get
		End Property

		''' <summary>
		''' Returns the menu disabled foreground color of the current theme. This is
		''' a cover method for
		''' {@code getCurrentTheme().getMenuDisabledForeground()}.
		''' </summary>
		''' <returns> the menu disabled foreground color
		''' </returns>
		''' <seealso cref= MetalTheme </seealso>
		Public Property Shared menuDisabledForeground As ColorUIResource
			Get
				Return currentTheme.menuDisabledForeground
			End Get
		End Property

		''' <summary>
		''' Returns the separator background color of the current theme. This is
		''' a cover method for {@code getCurrentTheme().getSeparatorBackground()}.
		''' </summary>
		''' <returns> the separator background color
		''' </returns>
		''' <seealso cref= MetalTheme </seealso>
		Public Property Shared separatorBackground As ColorUIResource
			Get
				Return currentTheme.separatorBackground
			End Get
		End Property

		''' <summary>
		''' Returns the separator foreground color of the current theme. This is
		''' a cover method for {@code getCurrentTheme().getSeparatorForeground()}.
		''' </summary>
		''' <returns> the separator foreground color
		''' </returns>
		''' <seealso cref= MetalTheme </seealso>
		Public Property Shared separatorForeground As ColorUIResource
			Get
				Return currentTheme.separatorForeground
			End Get
		End Property

		''' <summary>
		''' Returns the accelerator foreground color of the current theme. This is
		''' a cover method for {@code getCurrentTheme().getAcceleratorForeground()}.
		''' </summary>
		''' <returns> the separator accelerator foreground color
		''' </returns>
		''' <seealso cref= MetalTheme </seealso>
		Public Property Shared acceleratorForeground As ColorUIResource
			Get
				Return currentTheme.acceleratorForeground
			End Get
		End Property

		''' <summary>
		''' Returns the accelerator selected foreground color of the
		''' current theme. This is a cover method for {@code
		''' getCurrentTheme().getAcceleratorSelectedForeground()}.
		''' </summary>
		''' <returns> the accelerator selected foreground color
		''' </returns>
		''' <seealso cref= MetalTheme </seealso>
		Public Property Shared acceleratorSelectedForeground As ColorUIResource
			Get
				Return currentTheme.acceleratorSelectedForeground
			End Get
		End Property


		''' <summary>
		''' Returns a {@code LayoutStyle} implementing the Java look and feel
		''' design guidelines as specified at
		''' <a href="http://www.oracle.com/technetwork/java/hig-136467.html">http://www.oracle.com/technetwork/java/hig-136467.html</a>.
		''' </summary>
		''' <returns> LayoutStyle implementing the Java look and feel design
		'''         guidelines
		''' @since 1.6 </returns>
		Public Property Overrides layoutStyle As LayoutStyle
			Get
				Return MetalLayoutStyle.INSTANCE
			End Get
		End Property


		''' <summary>
		''' FontActiveValue redirects to the appropriate metal theme method.
		''' </summary>
		Private Class FontActiveValue
			Implements UIDefaults.ActiveValue

			Private type As Integer
			Private theme As MetalTheme

			Friend Sub New(ByVal theme As MetalTheme, ByVal type As Integer)
				Me.theme = theme
				Me.type = type
			End Sub

			Public Overridable Function createValue(ByVal table As UIDefaults) As Object Implements UIDefaults.ActiveValue.createValue
				Dim value As Object = Nothing
				Select Case type
				Case MetalTheme.CONTROL_TEXT_FONT
					value = theme.controlTextFont
				Case MetalTheme.SYSTEM_TEXT_FONT
					value = theme.systemTextFont
				Case MetalTheme.USER_TEXT_FONT
					value = theme.userTextFont
				Case MetalTheme.MENU_TEXT_FONT
					value = theme.menuTextFont
				Case MetalTheme.WINDOW_TITLE_FONT
					value = theme.windowTitleFont
				Case MetalTheme.SUB_TEXT_FONT
					value = theme.subTextFont
				End Select
				Return value
			End Function
		End Class

		Friend Shared queue As New ReferenceQueue(Of LookAndFeel)

		Friend Shared Sub flushUnreferenced()
			Dim aatl As AATextListener
			aatl = CType(queue.poll(), AATextListener)
			Do While aatl IsNot Nothing
				aatl.Dispose()
				aatl = CType(queue.poll(), AATextListener)
			Loop
		End Sub

		Friend Class AATextListener
			Inherits WeakReference(Of LookAndFeel)
			Implements java.beans.PropertyChangeListener

			Private key As String = SunToolkit.DESKTOPFONTHINTS

			Friend Sub New(ByVal laf As LookAndFeel)
				MyBase.New(laf, queue)
				Dim tk As Toolkit = Toolkit.defaultToolkit
				tk.addPropertyChangeListener(key, Me)
			End Sub

			Public Overridable Sub propertyChange(ByVal pce As java.beans.PropertyChangeEvent)
				Dim laf As LookAndFeel = get()
				If laf Is Nothing OrElse laf IsNot UIManager.lookAndFeel Then
					Dispose()
					Return
				End If
				Dim defaults As UIDefaults = UIManager.lookAndFeelDefaults
				Dim lafCond As Boolean = sun.swing.SwingUtilities2.localDisplay
				Dim aaTextInfo As Object = sun.swing.SwingUtilities2.AATextInfo.getAATextInfo(lafCond)
				defaults(sun.swing.SwingUtilities2.AA_TEXT_PROPERTY_KEY) = aaTextInfo
				updateUI()
			End Sub

			Friend Overridable Sub dispose()
				Dim tk As Toolkit = Toolkit.defaultToolkit
				tk.removePropertyChangeListener(key, Me)
			End Sub

			''' <summary>
			''' Updates the UI of the passed in window and all its children.
			''' </summary>
			Private Shared Sub updateWindowUI(ByVal window As Window)
				SwingUtilities.updateComponentTreeUI(window)
				Dim ownedWins As Window() = window.ownedWindows
				For Each w As Window In ownedWins
					updateWindowUI(w)
				Next w
			End Sub

			''' <summary>
			''' Updates the UIs of all the known Frames.
			''' </summary>
			Private Shared Sub updateAllUIs()
				Dim appFrames As Frame() = Frame.frames
				For Each frame As Frame In appFrames
					updateWindowUI(frame)
				Next frame
			End Sub

			''' <summary>
			''' Indicates if an updateUI call is pending.
			''' </summary>
			Private Shared updatePending As Boolean

			''' <summary>
			''' Sets whether or not an updateUI call is pending.
			''' </summary>
			<MethodImpl(MethodImplOptions.Synchronized)> _
			Private Shared Property updatePending As Boolean
				Set(ByVal update As Boolean)
					updatePending = update
				End Set
				Get
					Return updatePending
				End Get
			End Property


			Protected Friend Overridable Sub updateUI()
				If Not updatePending Then
					updatePending = True
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'					Runnable uiUpdater = New Runnable()
	'				{
	'						public void run()
	'						{
	'							updateAllUIs();
	'							setUpdatePending(False);
	'						}
	'					};
					SwingUtilities.invokeLater(uiUpdater)
				End If
			End Sub
		End Class

		' From the JLF Design Guidelines:
		' http://www.oracle.com/technetwork/java/jlf-135985.html
		Private Class MetalLayoutStyle
			Inherits sun.swing.DefaultLayoutStyle

			Private Shared INSTANCE As New MetalLayoutStyle

			Public Overrides Function getPreferredGap(ByVal component1 As JComponent, ByVal component2 As JComponent, ByVal type As ComponentPlacement, ByVal position As Integer, ByVal parent As Container) As Integer
				' Checks args
				MyBase.getPreferredGap(component1, component2, type, position, parent)

				Dim offset As Integer = 0

				Select Case type
				Case INDENT
					' Metal doesn't spec this.
					If position = SwingConstants.EAST OrElse position = SwingConstants.WEST Then
						Dim indent As Integer = getIndent(component1, position)
						If indent > 0 Then Return indent
						Return 12
					End If
					' Fall through to related.
				Case RELATED
					If component1.uIClassID = "ToggleButtonUI" AndAlso component2.uIClassID = "ToggleButtonUI" Then
						Dim sourceModel As ButtonModel = CType(component1, JToggleButton).model
						Dim targetModel As ButtonModel = CType(component2, JToggleButton).model
						If (TypeOf sourceModel Is DefaultButtonModel) AndAlso (TypeOf targetModel Is DefaultButtonModel) AndAlso (CType(sourceModel, DefaultButtonModel).group Is CType(targetModel, DefaultButtonModel).group) AndAlso CType(sourceModel, DefaultButtonModel).group IsNot Nothing Then Return 2
						' When toggle buttons are independent (like
						' checkboxes) and used outside a toolbar,
						' separate them with 5 pixels.
						If usingOcean() Then Return 6
						Return 5
					End If
					offset = 6
				Case UNRELATED
					offset = 12
				End Select
				If isLabelAndNonlabel(component1, component2, position) Then Return getButtonGap(component1, component2, position, offset + 6)
				Return getButtonGap(component1, component2, position, offset)
			End Function

			Public Overrides Function getContainerGap(ByVal component As JComponent, ByVal position As Integer, ByVal parent As Container) As Integer
				MyBase.getContainerGap(component, position, parent)
				' Include 11 pixels between the bottom and right
				' borders of a dialog box and its command
				' buttons. (To the eye, the 11-pixel spacing appears
				' to be 12 pixels because the white borders on the
				' lower and right edges of the button components are
				' not visually significant.)
				' NOTE: this last text was designed with Steel in mind,
				' not Ocean.
				'
				' Insert 12 pixels between the edges of the panel and the
				' titled border. Insert 11 pixels between the top of the
				' title and the component above the titled border. Insert 12
				' pixels between the bottom of the title and the top of the
				' first label in the panel. Insert 11 pixels between
				' component groups and between the bottom of the last
				' component and the lower border.
				Return getButtonGap(component, position, 12 - getButtonAdjustment(component, position))
			End Function

			Protected Friend Overrides Function getButtonGap(ByVal source As JComponent, ByVal target As JComponent, ByVal position As Integer, ByVal offset As Integer) As Integer
				offset = MyBase.getButtonGap(source, target, position, offset)
				If offset > 0 Then
					Dim ___buttonAdjustment As Integer = getButtonAdjustment(source, position)
					If ___buttonAdjustment = 0 Then ___buttonAdjustment = getButtonAdjustment(target, flipDirection(position))
					offset -= ___buttonAdjustment
				End If
				If offset < 0 Then Return 0
				Return offset
			End Function

			Private Function getButtonAdjustment(ByVal source As JComponent, ByVal edge As Integer) As Integer
				Dim classID As String = source.uIClassID
				If classID = "ButtonUI" OrElse classID = "ToggleButtonUI" Then
					If (Not usingOcean()) AndAlso (edge = SwingConstants.EAST OrElse edge = SwingConstants.SOUTH) Then
						If TypeOf source.border Is UIResource Then Return 1
					End If
				ElseIf edge = SwingConstants.SOUTH Then
					If (classID = "RadioButtonUI" OrElse classID = "CheckBoxUI") AndAlso (Not usingOcean()) Then Return 1
				End If
				Return 0
			End Function
		End Class
	End Class

End Namespace
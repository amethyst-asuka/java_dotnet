Imports System
Imports System.Collections.Generic
Imports System.Threading

'
' * Copyright (c) 2005, 2006, Oracle and/or its affiliates. All rights reserved.
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
Namespace javax.swing.plaf.nimbus


	''' <summary>
	''' This class contains all the implementation details related to
	''' Nimbus. It contains all the code for initializing the UIDefaults table,
	''' as well as for selecting
	''' a SynthStyle based on a JComponent/Region pair.
	''' 
	''' @author Richard Bair
	''' </summary>
	Friend NotInheritable Class NimbusDefaults
		''' <summary>
		''' The map of SynthStyles. This map is keyed by Region. Each Region maps
		''' to a List of LazyStyles. Each LazyStyle has a reference to the prefix
		''' that was registered with it. This reference can then be inspected to see
		''' if it is the proper lazy style.
		''' <p/>
		''' There can be more than one LazyStyle for a single Region if there is more
		''' than one prefix defined for a given region. For example, both Button and
		''' "MyButton" might be prefixes assigned to the Region.Button region.
		''' </summary>
		Private m As IDictionary(Of javax.swing.plaf.synth.Region, IList(Of LazyStyle))
		''' <summary>
		''' A map of regions which have been registered.
		''' This mapping is maintained so that the Region can be found based on
		''' prefix in a very fast manner. This is used in the "matches" method of
		''' LazyStyle.
		''' </summary>
		Private registeredRegions As IDictionary(Of String, javax.swing.plaf.synth.Region) = New Dictionary(Of String, javax.swing.plaf.synth.Region)

		Private overridesCache As IDictionary(Of javax.swing.JComponent, IDictionary(Of javax.swing.plaf.synth.Region, javax.swing.plaf.synth.SynthStyle)) = New java.util.WeakHashMap(Of javax.swing.JComponent, IDictionary(Of javax.swing.plaf.synth.Region, javax.swing.plaf.synth.SynthStyle))

		''' <summary>
		''' Our fallback style to avoid NPEs if the proper style cannot be found in
		''' this class. Not sure if relying on DefaultSynthStyle is the best choice.
		''' </summary>
		Private defaultStyle As sun.swing.plaf.synth.DefaultSynthStyle
		''' <summary>
		''' The default font that will be used. I store this value so that it can be
		''' set in the UIDefaults when requested.
		''' </summary>
		Private defaultFont As javax.swing.plaf.FontUIResource

		Private colorTree As New ColorTree(Me)

		''' <summary>
		''' Listener for changes to user defaults table </summary>
		Private defaultsListener As New DefaultsListener(Me)

		''' <summary>
		''' Called by UIManager when this look and feel is installed. </summary>
		Friend Sub initialize()
			' add listener for derived colors
			javax.swing.UIManager.addPropertyChangeListener(defaultsListener)
			javax.swing.UIManager.defaults.addPropertyChangeListener(colorTree)
		End Sub

		''' <summary>
		''' Called by UIManager when this look and feel is uninstalled. </summary>
		Friend Sub uninitialize()
			' remove listener for derived colors
			javax.swing.UIManager.removePropertyChangeListener(defaultsListener)
			javax.swing.UIManager.defaults.removePropertyChangeListener(colorTree)
		End Sub

		''' <summary>
		''' Create a new NimbusDefaults. This constructor is only called from
		''' within NimbusLookAndFeel.
		''' </summary>
		Friend Sub New()
			m = New Dictionary(Of javax.swing.plaf.synth.Region, IList(Of LazyStyle))

			'Create the default font and default style. Also register all of the
			'regions and their states that this class will use for later lookup.
			'Additional regions can be registered later by 3rd party components.
			'These are simply the default registrations.
			defaultFont = sun.font.FontUtilities.getFontConfigFUIR("sans", java.awt.Font.PLAIN, 12)
			defaultStyle = New sun.swing.plaf.synth.DefaultSynthStyle
			defaultStyle.font = defaultFont

			'initialize the map of styles
			register(javax.swing.plaf.synth.Region.ARROW_BUTTON, "ArrowButton")
			register(javax.swing.plaf.synth.Region.BUTTON, "Button")
			register(javax.swing.plaf.synth.Region.TOGGLE_BUTTON, "ToggleButton")
			register(javax.swing.plaf.synth.Region.RADIO_BUTTON, "RadioButton")
			register(javax.swing.plaf.synth.Region.CHECK_BOX, "CheckBox")
			register(javax.swing.plaf.synth.Region.COLOR_CHOOSER, "ColorChooser")
			register(javax.swing.plaf.synth.Region.PANEL, "ColorChooser:""ColorChooser.previewPanelHolder""")
			register(javax.swing.plaf.synth.Region.LABEL, "ColorChooser:""ColorChooser.previewPanelHolder"":""OptionPane.label""")
			register(javax.swing.plaf.synth.Region.COMBO_BOX, "ComboBox")
			register(javax.swing.plaf.synth.Region.TEXT_FIELD, "ComboBox:""ComboBox.textField""")
			register(javax.swing.plaf.synth.Region.ARROW_BUTTON, "ComboBox:""ComboBox.arrowButton""")
			register(javax.swing.plaf.synth.Region.LABEL, "ComboBox:""ComboBox.listRenderer""")
			register(javax.swing.plaf.synth.Region.LABEL, "ComboBox:""ComboBox.renderer""")
			register(javax.swing.plaf.synth.Region.SCROLL_PANE, """ComboBox.scrollPane""")
			register(javax.swing.plaf.synth.Region.FILE_CHOOSER, "FileChooser")
			register(javax.swing.plaf.synth.Region.INTERNAL_FRAME_TITLE_PANE, "InternalFrameTitlePane")
			register(javax.swing.plaf.synth.Region.INTERNAL_FRAME, "InternalFrame")
			register(javax.swing.plaf.synth.Region.INTERNAL_FRAME_TITLE_PANE, "InternalFrame:InternalFrameTitlePane")
			register(javax.swing.plaf.synth.Region.BUTTON, "InternalFrame:InternalFrameTitlePane:""InternalFrameTitlePane.menuButton""")
			register(javax.swing.plaf.synth.Region.BUTTON, "InternalFrame:InternalFrameTitlePane:""InternalFrameTitlePane.iconifyButton""")
			register(javax.swing.plaf.synth.Region.BUTTON, "InternalFrame:InternalFrameTitlePane:""InternalFrameTitlePane.maximizeButton""")
			register(javax.swing.plaf.synth.Region.BUTTON, "InternalFrame:InternalFrameTitlePane:""InternalFrameTitlePane.closeButton""")
			register(javax.swing.plaf.synth.Region.DESKTOP_ICON, "DesktopIcon")
			register(javax.swing.plaf.synth.Region.DESKTOP_PANE, "DesktopPane")
			register(javax.swing.plaf.synth.Region.LABEL, "Label")
			register(javax.swing.plaf.synth.Region.LIST, "List")
			register(javax.swing.plaf.synth.Region.LABEL, "List:""List.cellRenderer""")
			register(javax.swing.plaf.synth.Region.MENU_BAR, "MenuBar")
			register(javax.swing.plaf.synth.Region.MENU, "MenuBar:Menu")
			register(javax.swing.plaf.synth.Region.MENU_ITEM_ACCELERATOR, "MenuBar:Menu:MenuItemAccelerator")
			register(javax.swing.plaf.synth.Region.MENU_ITEM, "MenuItem")
			register(javax.swing.plaf.synth.Region.MENU_ITEM_ACCELERATOR, "MenuItem:MenuItemAccelerator")
			register(javax.swing.plaf.synth.Region.RADIO_BUTTON_MENU_ITEM, "RadioButtonMenuItem")
			register(javax.swing.plaf.synth.Region.MENU_ITEM_ACCELERATOR, "RadioButtonMenuItem:MenuItemAccelerator")
			register(javax.swing.plaf.synth.Region.CHECK_BOX_MENU_ITEM, "CheckBoxMenuItem")
			register(javax.swing.plaf.synth.Region.MENU_ITEM_ACCELERATOR, "CheckBoxMenuItem:MenuItemAccelerator")
			register(javax.swing.plaf.synth.Region.MENU, "Menu")
			register(javax.swing.plaf.synth.Region.MENU_ITEM_ACCELERATOR, "Menu:MenuItemAccelerator")
			register(javax.swing.plaf.synth.Region.POPUP_MENU, "PopupMenu")
			register(javax.swing.plaf.synth.Region.POPUP_MENU_SEPARATOR, "PopupMenuSeparator")
			register(javax.swing.plaf.synth.Region.OPTION_PANE, "OptionPane")
			register(javax.swing.plaf.synth.Region.SEPARATOR, "OptionPane:""OptionPane.separator""")
			register(javax.swing.plaf.synth.Region.PANEL, "OptionPane:""OptionPane.messageArea""")
			register(javax.swing.plaf.synth.Region.LABEL, "OptionPane:""OptionPane.messageArea"":""OptionPane.label""")
			register(javax.swing.plaf.synth.Region.PANEL, "Panel")
			register(javax.swing.plaf.synth.Region.PROGRESS_BAR, "ProgressBar")
			register(javax.swing.plaf.synth.Region.SEPARATOR, "Separator")
			register(javax.swing.plaf.synth.Region.SCROLL_BAR, "ScrollBar")
			register(javax.swing.plaf.synth.Region.ARROW_BUTTON, "ScrollBar:""ScrollBar.button""")
			register(javax.swing.plaf.synth.Region.SCROLL_BAR_THUMB, "ScrollBar:ScrollBarThumb")
			register(javax.swing.plaf.synth.Region.SCROLL_BAR_TRACK, "ScrollBar:ScrollBarTrack")
			register(javax.swing.plaf.synth.Region.SCROLL_PANE, "ScrollPane")
			register(javax.swing.plaf.synth.Region.VIEWPORT, "Viewport")
			register(javax.swing.plaf.synth.Region.SLIDER, "Slider")
			register(javax.swing.plaf.synth.Region.SLIDER_THUMB, "Slider:SliderThumb")
			register(javax.swing.plaf.synth.Region.SLIDER_TRACK, "Slider:SliderTrack")
			register(javax.swing.plaf.synth.Region.SPINNER, "Spinner")
			register(javax.swing.plaf.synth.Region.PANEL, "Spinner:""Spinner.editor""")
			register(javax.swing.plaf.synth.Region.FORMATTED_TEXT_FIELD, "Spinner:Panel:""Spinner.formattedTextField""")
			register(javax.swing.plaf.synth.Region.ARROW_BUTTON, "Spinner:""Spinner.previousButton""")
			register(javax.swing.plaf.synth.Region.ARROW_BUTTON, "Spinner:""Spinner.nextButton""")
			register(javax.swing.plaf.synth.Region.SPLIT_PANE, "SplitPane")
			register(javax.swing.plaf.synth.Region.SPLIT_PANE_DIVIDER, "SplitPane:SplitPaneDivider")
			register(javax.swing.plaf.synth.Region.TABBED_PANE, "TabbedPane")
			register(javax.swing.plaf.synth.Region.TABBED_PANE_TAB, "TabbedPane:TabbedPaneTab")
			register(javax.swing.plaf.synth.Region.TABBED_PANE_TAB_AREA, "TabbedPane:TabbedPaneTabArea")
			register(javax.swing.plaf.synth.Region.TABBED_PANE_CONTENT, "TabbedPane:TabbedPaneContent")
			register(javax.swing.plaf.synth.Region.TABLE, "Table")
			register(javax.swing.plaf.synth.Region.LABEL, "Table:""Table.cellRenderer""")
			register(javax.swing.plaf.synth.Region.TABLE_HEADER, "TableHeader")
			register(javax.swing.plaf.synth.Region.LABEL, "TableHeader:""TableHeader.renderer""")
			register(javax.swing.plaf.synth.Region.TEXT_FIELD, """Table.editor""")
			register(javax.swing.plaf.synth.Region.TEXT_FIELD, """Tree.cellEditor""")
			register(javax.swing.plaf.synth.Region.TEXT_FIELD, "TextField")
			register(javax.swing.plaf.synth.Region.FORMATTED_TEXT_FIELD, "FormattedTextField")
			register(javax.swing.plaf.synth.Region.PASSWORD_FIELD, "PasswordField")
			register(javax.swing.plaf.synth.Region.TEXT_AREA, "TextArea")
			register(javax.swing.plaf.synth.Region.TEXT_PANE, "TextPane")
			register(javax.swing.plaf.synth.Region.EDITOR_PANE, "EditorPane")
			register(javax.swing.plaf.synth.Region.TOOL_BAR, "ToolBar")
			register(javax.swing.plaf.synth.Region.BUTTON, "ToolBar:Button")
			register(javax.swing.plaf.synth.Region.TOGGLE_BUTTON, "ToolBar:ToggleButton")
			register(javax.swing.plaf.synth.Region.TOOL_BAR_SEPARATOR, "ToolBarSeparator")
			register(javax.swing.plaf.synth.Region.TOOL_TIP, "ToolTip")
			register(javax.swing.plaf.synth.Region.TREE, "Tree")
			register(javax.swing.plaf.synth.Region.TREE_CELL, "Tree:TreeCell")
			register(javax.swing.plaf.synth.Region.LABEL, "Tree:""Tree.cellRenderer""")
			register(javax.swing.plaf.synth.Region.ROOT_PANE, "RootPane")

		End Sub

		'--------------- Methods called by NimbusLookAndFeel

		''' <summary>
		''' Called from NimbusLookAndFeel to initialize the UIDefaults.
		''' </summary>
		''' <param name="d"> UIDefaults table to initialize. This will never be null.
		'''          If listeners are attached to <code>d</code>, then you will
		'''          only receive notification of LookAndFeel level defaults, not
		'''          all defaults on the UIManager. </param>
		Friend Sub initializeDefaults(ByVal d As javax.swing.UIDefaults)
			'Color palette
			addColor(d, "text", 0, 0, 0, 255)
			addColor(d, "control", 214, 217, 223, 255)
			addColor(d, "nimbusBase", 51, 98, 140, 255)
			addColor(d, "nimbusBlueGrey", "nimbusBase", 0.032459438f, -0.52518797f, 0.19607842f, 0)
			addColor(d, "nimbusOrange", 191, 98, 4, 255)
			addColor(d, "nimbusGreen", 176, 179, 50, 255)
			addColor(d, "nimbusRed", 169, 46, 34, 255)
			addColor(d, "nimbusBorder", "nimbusBlueGrey", 0.0f, -0.017358616f, -0.11372548f, 0)
			addColor(d, "nimbusSelection", "nimbusBase", -0.010750473f, -0.04875779f, -0.007843137f, 0)
			addColor(d, "nimbusInfoBlue", 47, 92, 180, 255)
			addColor(d, "nimbusAlertYellow", 255, 220, 35, 255)
			addColor(d, "nimbusFocus", 115, 164, 209, 255)
			addColor(d, "nimbusSelectedText", 255, 255, 255, 255)
			addColor(d, "nimbusSelectionBackground", 57, 105, 138, 255)
			addColor(d, "nimbusDisabledText", 142, 143, 145, 255)
			addColor(d, "nimbusLightBackground", 255, 255, 255, 255)
			addColor(d, "infoText", "text", 0.0f, 0.0f, 0.0f, 0)
			addColor(d, "info", 242, 242, 189, 255)
			addColor(d, "menuText", "text", 0.0f, 0.0f, 0.0f, 0)
			addColor(d, "menu", "nimbusBase", 0.021348298f, -0.6150531f, 0.39999998f, 0)
			addColor(d, "scrollbar", "nimbusBlueGrey", -0.006944418f, -0.07296763f, 0.09019607f, 0)
			addColor(d, "controlText", "text", 0.0f, 0.0f, 0.0f, 0)
			addColor(d, "controlHighlight", "nimbusBlueGrey", 0.0f, -0.07333623f, 0.20392156f, 0)
			addColor(d, "controlLHighlight", "nimbusBlueGrey", 0.0f, -0.098526314f, 0.2352941f, 0)
			addColor(d, "controlShadow", "nimbusBlueGrey", -0.0027777553f, -0.0212406f, 0.13333333f, 0)
			addColor(d, "controlDkShadow", "nimbusBlueGrey", -0.0027777553f, -0.0018306673f, -0.02352941f, 0)
			addColor(d, "textHighlight", "nimbusSelectionBackground", 0.0f, 0.0f, 0.0f, 0)
			addColor(d, "textHighlightText", "nimbusSelectedText", 0.0f, 0.0f, 0.0f, 0)
			addColor(d, "textInactiveText", "nimbusDisabledText", 0.0f, 0.0f, 0.0f, 0)
			addColor(d, "desktop", "nimbusBase", -0.009207249f, -0.13984653f, -0.07450983f, 0)
			addColor(d, "activeCaption", "nimbusBlueGrey", 0.0f, -0.049920253f, 0.031372547f, 0)
			addColor(d, "inactiveCaption", "nimbusBlueGrey", -0.00505054f, -0.055526316f, 0.039215684f, 0)

			'Font palette
			d("defaultFont") = New javax.swing.plaf.FontUIResource(defaultFont)
			d("InternalFrame.titleFont") = New DerivedFont("defaultFont", 1.0f, True, Nothing)

			'Border palette

			'The global style definition
			addColor(d, "textForeground", "text", 0.0f, 0.0f, 0.0f, 0)
			addColor(d, "textBackground", "nimbusSelectionBackground", 0.0f, 0.0f, 0.0f, 0)
			addColor(d, "background", "control", 0.0f, 0.0f, 0.0f, 0)
			d("TitledBorder.position") = "ABOVE_TOP"
			d("FileView.fullRowSelection") = Boolean.TRUE

			'Initialize ArrowButton
			d("ArrowButton.contentMargins") = New javax.swing.plaf.InsetsUIResource(0, 0, 0, 0)
			d("ArrowButton.size") = New Integer?(16)
			d("ArrowButton[Disabled].foregroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ArrowButtonPainter", ArrowButtonPainter.FOREGROUND_DISABLED, New java.awt.Insets(0, 0, 0, 0), New java.awt.Dimension(10, 10), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("ArrowButton[Enabled].foregroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ArrowButtonPainter", ArrowButtonPainter.FOREGROUND_ENABLED, New java.awt.Insets(0, 0, 0, 0), New java.awt.Dimension(10, 10), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)

			'Initialize Button
			d("Button.contentMargins") = New javax.swing.plaf.InsetsUIResource(6, 14, 6, 14)
			d("Button.defaultButtonFollowsFocus") = Boolean.FALSE
			d("Button[Default].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ButtonPainter", ButtonPainter.BACKGROUND_DEFAULT, New java.awt.Insets(7, 7, 7, 7), New java.awt.Dimension(104, 33), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, 2.0)
			d("Button[Default+Focused].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ButtonPainter", ButtonPainter.BACKGROUND_DEFAULT_FOCUSED, New java.awt.Insets(7, 7, 7, 7), New java.awt.Dimension(104, 33), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, 2.0)
			d("Button[Default+MouseOver].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ButtonPainter", ButtonPainter.BACKGROUND_MOUSEOVER_DEFAULT, New java.awt.Insets(7, 7, 7, 7), New java.awt.Dimension(104, 33), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, 2.0)
			d("Button[Default+Focused+MouseOver].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ButtonPainter", ButtonPainter.BACKGROUND_MOUSEOVER_DEFAULT_FOCUSED, New java.awt.Insets(7, 7, 7, 7), New java.awt.Dimension(104, 33), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, 2.0)
			addColor(d, "Button[Default+Pressed].textForeground", "nimbusSelectedText", 0.0f, 0.0f, 0.0f, 0)
			d("Button[Default+Pressed].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ButtonPainter", ButtonPainter.BACKGROUND_PRESSED_DEFAULT, New java.awt.Insets(7, 7, 7, 7), New java.awt.Dimension(104, 33), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, 2.0)
			d("Button[Default+Focused+Pressed].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ButtonPainter", ButtonPainter.BACKGROUND_PRESSED_DEFAULT_FOCUSED, New java.awt.Insets(7, 7, 7, 7), New java.awt.Dimension(104, 33), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, 2.0)
			addColor(d, "Button[Disabled].textForeground", "nimbusDisabledText", 0.0f, 0.0f, 0.0f, 0)
			d("Button[Disabled].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ButtonPainter", ButtonPainter.BACKGROUND_DISABLED, New java.awt.Insets(7, 7, 7, 7), New java.awt.Dimension(104, 33), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, 2.0)
			d("Button[Enabled].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ButtonPainter", ButtonPainter.BACKGROUND_ENABLED, New java.awt.Insets(7, 7, 7, 7), New java.awt.Dimension(104, 33), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, 2.0)
			d("Button[Focused].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ButtonPainter", ButtonPainter.BACKGROUND_FOCUSED, New java.awt.Insets(7, 7, 7, 7), New java.awt.Dimension(104, 33), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, 2.0)
			d("Button[MouseOver].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ButtonPainter", ButtonPainter.BACKGROUND_MOUSEOVER, New java.awt.Insets(7, 7, 7, 7), New java.awt.Dimension(104, 33), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, 2.0)
			d("Button[Focused+MouseOver].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ButtonPainter", ButtonPainter.BACKGROUND_MOUSEOVER_FOCUSED, New java.awt.Insets(7, 7, 7, 7), New java.awt.Dimension(104, 33), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, 2.0)
			d("Button[Pressed].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ButtonPainter", ButtonPainter.BACKGROUND_PRESSED, New java.awt.Insets(7, 7, 7, 7), New java.awt.Dimension(104, 33), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, 2.0)
			d("Button[Focused+Pressed].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ButtonPainter", ButtonPainter.BACKGROUND_PRESSED_FOCUSED, New java.awt.Insets(7, 7, 7, 7), New java.awt.Dimension(104, 33), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, 2.0)

			'Initialize ToggleButton
			d("ToggleButton.contentMargins") = New javax.swing.plaf.InsetsUIResource(6, 14, 6, 14)
			addColor(d, "ToggleButton[Disabled].textForeground", "nimbusDisabledText", 0.0f, 0.0f, 0.0f, 0)
			d("ToggleButton[Disabled].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ToggleButtonPainter", ToggleButtonPainter.BACKGROUND_DISABLED, New java.awt.Insets(7, 7, 7, 7), New java.awt.Dimension(104, 33), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, 2.0)
			d("ToggleButton[Enabled].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ToggleButtonPainter", ToggleButtonPainter.BACKGROUND_ENABLED, New java.awt.Insets(7, 7, 7, 7), New java.awt.Dimension(104, 33), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, 2.0)
			d("ToggleButton[Focused].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ToggleButtonPainter", ToggleButtonPainter.BACKGROUND_FOCUSED, New java.awt.Insets(7, 7, 7, 7), New java.awt.Dimension(104, 33), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, 2.0)
			d("ToggleButton[MouseOver].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ToggleButtonPainter", ToggleButtonPainter.BACKGROUND_MOUSEOVER, New java.awt.Insets(7, 7, 7, 7), New java.awt.Dimension(104, 33), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, 2.0)
			d("ToggleButton[Focused+MouseOver].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ToggleButtonPainter", ToggleButtonPainter.BACKGROUND_MOUSEOVER_FOCUSED, New java.awt.Insets(7, 7, 7, 7), New java.awt.Dimension(104, 33), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, 2.0)
			d("ToggleButton[Pressed].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ToggleButtonPainter", ToggleButtonPainter.BACKGROUND_PRESSED, New java.awt.Insets(7, 7, 7, 7), New java.awt.Dimension(104, 33), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, 2.0)
			d("ToggleButton[Focused+Pressed].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ToggleButtonPainter", ToggleButtonPainter.BACKGROUND_PRESSED_FOCUSED, New java.awt.Insets(7, 7, 7, 7), New java.awt.Dimension(104, 33), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, 2.0)
			d("ToggleButton[Selected].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ToggleButtonPainter", ToggleButtonPainter.BACKGROUND_SELECTED, New java.awt.Insets(7, 7, 7, 7), New java.awt.Dimension(72, 24), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, 2.0)
			d("ToggleButton[Focused+Selected].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ToggleButtonPainter", ToggleButtonPainter.BACKGROUND_SELECTED_FOCUSED, New java.awt.Insets(7, 7, 7, 7), New java.awt.Dimension(72, 24), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, 2.0)
			d("ToggleButton[Pressed+Selected].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ToggleButtonPainter", ToggleButtonPainter.BACKGROUND_PRESSED_SELECTED, New java.awt.Insets(7, 7, 7, 7), New java.awt.Dimension(72, 24), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, 2.0)
			d("ToggleButton[Focused+Pressed+Selected].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ToggleButtonPainter", ToggleButtonPainter.BACKGROUND_PRESSED_SELECTED_FOCUSED, New java.awt.Insets(7, 7, 7, 7), New java.awt.Dimension(72, 24), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, 2.0)
			d("ToggleButton[MouseOver+Selected].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ToggleButtonPainter", ToggleButtonPainter.BACKGROUND_MOUSEOVER_SELECTED, New java.awt.Insets(7, 7, 7, 7), New java.awt.Dimension(72, 24), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, 2.0)
			d("ToggleButton[Focused+MouseOver+Selected].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ToggleButtonPainter", ToggleButtonPainter.BACKGROUND_MOUSEOVER_SELECTED_FOCUSED, New java.awt.Insets(7, 7, 7, 7), New java.awt.Dimension(72, 24), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, 2.0)
			addColor(d, "ToggleButton[Disabled+Selected].textForeground", "nimbusDisabledText", 0.0f, 0.0f, 0.0f, 0)
			d("ToggleButton[Disabled+Selected].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ToggleButtonPainter", ToggleButtonPainter.BACKGROUND_DISABLED_SELECTED, New java.awt.Insets(7, 7, 7, 7), New java.awt.Dimension(72, 24), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, 2.0)

			'Initialize RadioButton
			d("RadioButton.contentMargins") = New javax.swing.plaf.InsetsUIResource(0, 0, 0, 0)
			addColor(d, "RadioButton[Disabled].textForeground", "nimbusDisabledText", 0.0f, 0.0f, 0.0f, 0)
			d("RadioButton[Disabled].iconPainter") = New LazyPainter("javax.swing.plaf.nimbus.RadioButtonPainter", RadioButtonPainter.ICON_DISABLED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(18, 18), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("RadioButton[Enabled].iconPainter") = New LazyPainter("javax.swing.plaf.nimbus.RadioButtonPainter", RadioButtonPainter.ICON_ENABLED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(18, 18), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("RadioButton[Focused].iconPainter") = New LazyPainter("javax.swing.plaf.nimbus.RadioButtonPainter", RadioButtonPainter.ICON_FOCUSED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(18, 18), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("RadioButton[MouseOver].iconPainter") = New LazyPainter("javax.swing.plaf.nimbus.RadioButtonPainter", RadioButtonPainter.ICON_MOUSEOVER, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(18, 18), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("RadioButton[Focused+MouseOver].iconPainter") = New LazyPainter("javax.swing.plaf.nimbus.RadioButtonPainter", RadioButtonPainter.ICON_MOUSEOVER_FOCUSED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(18, 18), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("RadioButton[Pressed].iconPainter") = New LazyPainter("javax.swing.plaf.nimbus.RadioButtonPainter", RadioButtonPainter.ICON_PRESSED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(18, 18), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("RadioButton[Focused+Pressed].iconPainter") = New LazyPainter("javax.swing.plaf.nimbus.RadioButtonPainter", RadioButtonPainter.ICON_PRESSED_FOCUSED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(18, 18), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("RadioButton[Selected].iconPainter") = New LazyPainter("javax.swing.plaf.nimbus.RadioButtonPainter", RadioButtonPainter.ICON_SELECTED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(18, 18), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("RadioButton[Focused+Selected].iconPainter") = New LazyPainter("javax.swing.plaf.nimbus.RadioButtonPainter", RadioButtonPainter.ICON_SELECTED_FOCUSED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(18, 18), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("RadioButton[Pressed+Selected].iconPainter") = New LazyPainter("javax.swing.plaf.nimbus.RadioButtonPainter", RadioButtonPainter.ICON_PRESSED_SELECTED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(18, 18), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("RadioButton[Focused+Pressed+Selected].iconPainter") = New LazyPainter("javax.swing.plaf.nimbus.RadioButtonPainter", RadioButtonPainter.ICON_PRESSED_SELECTED_FOCUSED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(18, 18), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("RadioButton[MouseOver+Selected].iconPainter") = New LazyPainter("javax.swing.plaf.nimbus.RadioButtonPainter", RadioButtonPainter.ICON_MOUSEOVER_SELECTED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(18, 18), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("RadioButton[Focused+MouseOver+Selected].iconPainter") = New LazyPainter("javax.swing.plaf.nimbus.RadioButtonPainter", RadioButtonPainter.ICON_MOUSEOVER_SELECTED_FOCUSED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(18, 18), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("RadioButton[Disabled+Selected].iconPainter") = New LazyPainter("javax.swing.plaf.nimbus.RadioButtonPainter", RadioButtonPainter.ICON_DISABLED_SELECTED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(18, 18), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("RadioButton.icon") = New NimbusIcon("RadioButton", "iconPainter", 18, 18)

			'Initialize CheckBox
			d("CheckBox.contentMargins") = New javax.swing.plaf.InsetsUIResource(0, 0, 0, 0)
			addColor(d, "CheckBox[Disabled].textForeground", "nimbusDisabledText", 0.0f, 0.0f, 0.0f, 0)
			d("CheckBox[Disabled].iconPainter") = New LazyPainter("javax.swing.plaf.nimbus.CheckBoxPainter", CheckBoxPainter.ICON_DISABLED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(18, 18), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("CheckBox[Enabled].iconPainter") = New LazyPainter("javax.swing.plaf.nimbus.CheckBoxPainter", CheckBoxPainter.ICON_ENABLED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(18, 18), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("CheckBox[Focused].iconPainter") = New LazyPainter("javax.swing.plaf.nimbus.CheckBoxPainter", CheckBoxPainter.ICON_FOCUSED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(18, 18), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("CheckBox[MouseOver].iconPainter") = New LazyPainter("javax.swing.plaf.nimbus.CheckBoxPainter", CheckBoxPainter.ICON_MOUSEOVER, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(18, 18), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("CheckBox[Focused+MouseOver].iconPainter") = New LazyPainter("javax.swing.plaf.nimbus.CheckBoxPainter", CheckBoxPainter.ICON_MOUSEOVER_FOCUSED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(18, 18), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("CheckBox[Pressed].iconPainter") = New LazyPainter("javax.swing.plaf.nimbus.CheckBoxPainter", CheckBoxPainter.ICON_PRESSED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(18, 18), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("CheckBox[Focused+Pressed].iconPainter") = New LazyPainter("javax.swing.plaf.nimbus.CheckBoxPainter", CheckBoxPainter.ICON_PRESSED_FOCUSED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(18, 18), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("CheckBox[Selected].iconPainter") = New LazyPainter("javax.swing.plaf.nimbus.CheckBoxPainter", CheckBoxPainter.ICON_SELECTED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(18, 18), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("CheckBox[Focused+Selected].iconPainter") = New LazyPainter("javax.swing.plaf.nimbus.CheckBoxPainter", CheckBoxPainter.ICON_SELECTED_FOCUSED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(18, 18), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("CheckBox[Pressed+Selected].iconPainter") = New LazyPainter("javax.swing.plaf.nimbus.CheckBoxPainter", CheckBoxPainter.ICON_PRESSED_SELECTED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(18, 18), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("CheckBox[Focused+Pressed+Selected].iconPainter") = New LazyPainter("javax.swing.plaf.nimbus.CheckBoxPainter", CheckBoxPainter.ICON_PRESSED_SELECTED_FOCUSED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(18, 18), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("CheckBox[MouseOver+Selected].iconPainter") = New LazyPainter("javax.swing.plaf.nimbus.CheckBoxPainter", CheckBoxPainter.ICON_MOUSEOVER_SELECTED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(18, 18), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("CheckBox[Focused+MouseOver+Selected].iconPainter") = New LazyPainter("javax.swing.plaf.nimbus.CheckBoxPainter", CheckBoxPainter.ICON_MOUSEOVER_SELECTED_FOCUSED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(18, 18), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("CheckBox[Disabled+Selected].iconPainter") = New LazyPainter("javax.swing.plaf.nimbus.CheckBoxPainter", CheckBoxPainter.ICON_DISABLED_SELECTED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(18, 18), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("CheckBox.icon") = New NimbusIcon("CheckBox", "iconPainter", 18, 18)

			'Initialize ColorChooser
			d("ColorChooser.contentMargins") = New javax.swing.plaf.InsetsUIResource(5, 0, 0, 0)
			addColor(d, "ColorChooser.swatchesDefaultRecentColor", 255, 255, 255, 255)
			d("ColorChooser:""ColorChooser.previewPanelHolder"".contentMargins") = New javax.swing.plaf.InsetsUIResource(0, 5, 10, 5)
			d("ColorChooser:""ColorChooser.previewPanelHolder"":""OptionPane.label"".contentMargins") = New javax.swing.plaf.InsetsUIResource(0, 10, 10, 10)

			'Initialize ComboBox
			d("ComboBox.contentMargins") = New javax.swing.plaf.InsetsUIResource(0, 0, 0, 0)
			d("ComboBox.States") = "Enabled,MouseOver,Pressed,Selected,Disabled,Focused,Editable"
			d("ComboBox.Editable") = New ComboBoxEditableState
			d("ComboBox.forceOpaque") = Boolean.TRUE
			d("ComboBox.buttonWhenNotEditable") = Boolean.TRUE
			d("ComboBox.rendererUseListColors") = Boolean.FALSE
			d("ComboBox.pressedWhenPopupVisible") = Boolean.TRUE
			d("ComboBox.squareButton") = Boolean.FALSE
			d("ComboBox.popupInsets") = New javax.swing.plaf.InsetsUIResource(-2, 2, 0, 2)
			d("ComboBox.padding") = New javax.swing.plaf.InsetsUIResource(3, 3, 3, 3)
			d("ComboBox[Disabled].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ComboBoxPainter", ComboBoxPainter.BACKGROUND_DISABLED, New java.awt.Insets(8, 9, 8, 19), New java.awt.Dimension(83, 24), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, 2.0)
			d("ComboBox[Disabled+Pressed].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ComboBoxPainter", ComboBoxPainter.BACKGROUND_DISABLED_PRESSED, New java.awt.Insets(8, 9, 8, 19), New java.awt.Dimension(83, 24), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, 2.0)
			d("ComboBox[Enabled].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ComboBoxPainter", ComboBoxPainter.BACKGROUND_ENABLED, New java.awt.Insets(8, 9, 8, 19), New java.awt.Dimension(83, 24), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, 2.0)
			d("ComboBox[Focused].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ComboBoxPainter", ComboBoxPainter.BACKGROUND_FOCUSED, New java.awt.Insets(8, 9, 8, 19), New java.awt.Dimension(83, 24), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, 2.0)
			d("ComboBox[Focused+MouseOver].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ComboBoxPainter", ComboBoxPainter.BACKGROUND_MOUSEOVER_FOCUSED, New java.awt.Insets(8, 9, 8, 19), New java.awt.Dimension(83, 24), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, 2.0)
			d("ComboBox[MouseOver].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ComboBoxPainter", ComboBoxPainter.BACKGROUND_MOUSEOVER, New java.awt.Insets(8, 9, 8, 19), New java.awt.Dimension(83, 24), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, 2.0)
			d("ComboBox[Focused+Pressed].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ComboBoxPainter", ComboBoxPainter.BACKGROUND_PRESSED_FOCUSED, New java.awt.Insets(8, 9, 8, 19), New java.awt.Dimension(83, 24), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, 2.0)
			d("ComboBox[Pressed].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ComboBoxPainter", ComboBoxPainter.BACKGROUND_PRESSED, New java.awt.Insets(8, 9, 8, 19), New java.awt.Dimension(83, 24), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, 2.0)
			d("ComboBox[Enabled+Selected].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ComboBoxPainter", ComboBoxPainter.BACKGROUND_ENABLED_SELECTED, New java.awt.Insets(8, 9, 8, 19), New java.awt.Dimension(83, 24), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, 2.0)
			d("ComboBox[Disabled+Editable].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ComboBoxPainter", ComboBoxPainter.BACKGROUND_DISABLED_EDITABLE, New java.awt.Insets(6, 5, 6, 17), New java.awt.Dimension(79, 21), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, 2.0)
			d("ComboBox[Editable+Enabled].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ComboBoxPainter", ComboBoxPainter.BACKGROUND_ENABLED_EDITABLE, New java.awt.Insets(6, 5, 6, 17), New java.awt.Dimension(79, 21), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, 2.0)
			d("ComboBox[Editable+Focused].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ComboBoxPainter", ComboBoxPainter.BACKGROUND_FOCUSED_EDITABLE, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(142, 27), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, 2.0)
			d("ComboBox[Editable+MouseOver].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ComboBoxPainter", ComboBoxPainter.BACKGROUND_MOUSEOVER_EDITABLE, New java.awt.Insets(4, 5, 5, 17), New java.awt.Dimension(79, 21), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, 2.0)
			d("ComboBox[Editable+Pressed].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ComboBoxPainter", ComboBoxPainter.BACKGROUND_PRESSED_EDITABLE, New java.awt.Insets(4, 5, 5, 17), New java.awt.Dimension(79, 21), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, 2.0)
			d("ComboBox:""ComboBox.textField"".contentMargins") = New javax.swing.plaf.InsetsUIResource(0, 6, 0, 3)
			addColor(d, "ComboBox:""ComboBox.textField""[Disabled].textForeground", "nimbusDisabledText", 0.0f, 0.0f, 0.0f, 0)
			d("ComboBox:""ComboBox.textField""[Disabled].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ComboBoxTextFieldPainter", ComboBoxTextFieldPainter.BACKGROUND_DISABLED, New java.awt.Insets(5, 3, 3, 1), New java.awt.Dimension(64, 24), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, 2.0)
			d("ComboBox:""ComboBox.textField""[Enabled].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ComboBoxTextFieldPainter", ComboBoxTextFieldPainter.BACKGROUND_ENABLED, New java.awt.Insets(5, 3, 3, 1), New java.awt.Dimension(64, 24), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, 2.0)
			addColor(d, "ComboBox:""ComboBox.textField""[Selected].textForeground", "nimbusSelectedText", 0.0f, 0.0f, 0.0f, 0)
			d("ComboBox:""ComboBox.textField""[Selected].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ComboBoxTextFieldPainter", ComboBoxTextFieldPainter.BACKGROUND_SELECTED, New java.awt.Insets(5, 3, 3, 1), New java.awt.Dimension(64, 24), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, 2.0)
			d("ComboBox:""ComboBox.arrowButton"".contentMargins") = New javax.swing.plaf.InsetsUIResource(0, 0, 0, 0)
			d("ComboBox:""ComboBox.arrowButton"".States") = "Enabled,MouseOver,Pressed,Disabled,Editable"
			d("ComboBox:""ComboBox.arrowButton"".Editable") = New ComboBoxArrowButtonEditableState
			d("ComboBox:""ComboBox.arrowButton"".size") = New Integer?(19)
			d("ComboBox:""ComboBox.arrowButton""[Disabled+Editable].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ComboBoxArrowButtonPainter", ComboBoxArrowButtonPainter.BACKGROUND_DISABLED_EDITABLE, New java.awt.Insets(8, 1, 8, 8), New java.awt.Dimension(20, 24), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, 2.0)
			d("ComboBox:""ComboBox.arrowButton""[Editable+Enabled].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ComboBoxArrowButtonPainter", ComboBoxArrowButtonPainter.BACKGROUND_ENABLED_EDITABLE, New java.awt.Insets(8, 1, 8, 8), New java.awt.Dimension(20, 24), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, 2.0)
			d("ComboBox:""ComboBox.arrowButton""[Editable+MouseOver].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ComboBoxArrowButtonPainter", ComboBoxArrowButtonPainter.BACKGROUND_MOUSEOVER_EDITABLE, New java.awt.Insets(8, 1, 8, 8), New java.awt.Dimension(20, 24), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, 2.0)
			d("ComboBox:""ComboBox.arrowButton""[Editable+Pressed].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ComboBoxArrowButtonPainter", ComboBoxArrowButtonPainter.BACKGROUND_PRESSED_EDITABLE, New java.awt.Insets(8, 1, 8, 8), New java.awt.Dimension(20, 24), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, 2.0)
			d("ComboBox:""ComboBox.arrowButton""[Editable+Selected].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ComboBoxArrowButtonPainter", ComboBoxArrowButtonPainter.BACKGROUND_SELECTED_EDITABLE, New java.awt.Insets(8, 1, 8, 8), New java.awt.Dimension(20, 24), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, 2.0)
			d("ComboBox:""ComboBox.arrowButton""[Enabled].foregroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ComboBoxArrowButtonPainter", ComboBoxArrowButtonPainter.FOREGROUND_ENABLED, New java.awt.Insets(6, 9, 6, 10), New java.awt.Dimension(24, 19), True, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, 2.0)
			d("ComboBox:""ComboBox.arrowButton""[MouseOver].foregroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ComboBoxArrowButtonPainter", ComboBoxArrowButtonPainter.FOREGROUND_MOUSEOVER, New java.awt.Insets(6, 9, 6, 10), New java.awt.Dimension(24, 19), True, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, 2.0)
			d("ComboBox:""ComboBox.arrowButton""[Disabled].foregroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ComboBoxArrowButtonPainter", ComboBoxArrowButtonPainter.FOREGROUND_DISABLED, New java.awt.Insets(6, 9, 6, 10), New java.awt.Dimension(24, 19), True, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, 2.0)
			d("ComboBox:""ComboBox.arrowButton""[Pressed].foregroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ComboBoxArrowButtonPainter", ComboBoxArrowButtonPainter.FOREGROUND_PRESSED, New java.awt.Insets(6, 9, 6, 10), New java.awt.Dimension(24, 19), True, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, 2.0)
			d("ComboBox:""ComboBox.arrowButton""[Selected].foregroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ComboBoxArrowButtonPainter", ComboBoxArrowButtonPainter.FOREGROUND_SELECTED, New java.awt.Insets(6, 9, 6, 10), New java.awt.Dimension(24, 19), True, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, 2.0)
			d("ComboBox:""ComboBox.listRenderer"".contentMargins") = New javax.swing.plaf.InsetsUIResource(2, 4, 2, 4)
			d("ComboBox:""ComboBox.listRenderer"".opaque") = Boolean.TRUE
			addColor(d, "ComboBox:""ComboBox.listRenderer"".background", "nimbusLightBackground", 0.0f, 0.0f, 0.0f, 0)
			addColor(d, "ComboBox:""ComboBox.listRenderer""[Disabled].textForeground", "nimbusDisabledText", 0.0f, 0.0f, 0.0f, 0)
			addColor(d, "ComboBox:""ComboBox.listRenderer""[Selected].textForeground", "nimbusSelectedText", 0.0f, 0.0f, 0.0f, 0)
			addColor(d, "ComboBox:""ComboBox.listRenderer""[Selected].background", "nimbusSelectionBackground", 0.0f, 0.0f, 0.0f, 0)
			d("ComboBox:""ComboBox.renderer"".contentMargins") = New javax.swing.plaf.InsetsUIResource(2, 4, 2, 4)
			addColor(d, "ComboBox:""ComboBox.renderer""[Disabled].textForeground", "nimbusDisabledText", 0.0f, 0.0f, 0.0f, 0)
			addColor(d, "ComboBox:""ComboBox.renderer""[Selected].textForeground", "nimbusSelectedText", 0.0f, 0.0f, 0.0f, 0)
			addColor(d, "ComboBox:""ComboBox.renderer""[Selected].background", "nimbusSelectionBackground", 0.0f, 0.0f, 0.0f, 0)

			'Initialize \"ComboBox.scrollPane\"
			d("""ComboBox.scrollPane"".contentMargins") = New javax.swing.plaf.InsetsUIResource(0, 0, 0, 0)

			'Initialize FileChooser
			d("FileChooser.contentMargins") = New javax.swing.plaf.InsetsUIResource(10, 10, 10, 10)
			d("FileChooser.opaque") = Boolean.TRUE
			d("FileChooser.usesSingleFilePane") = Boolean.TRUE
			d("FileChooser[Enabled].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.FileChooserPainter", FileChooserPainter.BACKGROUND_ENABLED, New java.awt.Insets(0, 0, 0, 0), New java.awt.Dimension(100, 30), False, AbstractRegionPainter.PaintContext.CacheMode.NO_CACHING, 1.0, 1.0)
			d("FileChooser[Enabled].fileIconPainter") = New LazyPainter("javax.swing.plaf.nimbus.FileChooserPainter", FileChooserPainter.FILEICON_ENABLED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(16, 16), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("FileChooser.fileIcon") = New NimbusIcon("FileChooser", "fileIconPainter", 16, 16)
			d("FileChooser[Enabled].directoryIconPainter") = New LazyPainter("javax.swing.plaf.nimbus.FileChooserPainter", FileChooserPainter.DIRECTORYICON_ENABLED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(16, 16), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("FileChooser.directoryIcon") = New NimbusIcon("FileChooser", "directoryIconPainter", 16, 16)
			d("FileChooser[Enabled].upFolderIconPainter") = New LazyPainter("javax.swing.plaf.nimbus.FileChooserPainter", FileChooserPainter.UPFOLDERICON_ENABLED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(16, 16), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("FileChooser.upFolderIcon") = New NimbusIcon("FileChooser", "upFolderIconPainter", 16, 16)
			d("FileChooser[Enabled].newFolderIconPainter") = New LazyPainter("javax.swing.plaf.nimbus.FileChooserPainter", FileChooserPainter.NEWFOLDERICON_ENABLED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(16, 16), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("FileChooser.newFolderIcon") = New NimbusIcon("FileChooser", "newFolderIconPainter", 16, 16)
			d("FileChooser[Enabled].hardDriveIconPainter") = New LazyPainter("javax.swing.plaf.nimbus.FileChooserPainter", FileChooserPainter.HARDDRIVEICON_ENABLED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(16, 16), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("FileChooser.hardDriveIcon") = New NimbusIcon("FileChooser", "hardDriveIconPainter", 16, 16)
			d("FileChooser[Enabled].floppyDriveIconPainter") = New LazyPainter("javax.swing.plaf.nimbus.FileChooserPainter", FileChooserPainter.FLOPPYDRIVEICON_ENABLED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(16, 16), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("FileChooser.floppyDriveIcon") = New NimbusIcon("FileChooser", "floppyDriveIconPainter", 16, 16)
			d("FileChooser[Enabled].homeFolderIconPainter") = New LazyPainter("javax.swing.plaf.nimbus.FileChooserPainter", FileChooserPainter.HOMEFOLDERICON_ENABLED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(16, 16), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("FileChooser.homeFolderIcon") = New NimbusIcon("FileChooser", "homeFolderIconPainter", 16, 16)
			d("FileChooser[Enabled].detailsViewIconPainter") = New LazyPainter("javax.swing.plaf.nimbus.FileChooserPainter", FileChooserPainter.DETAILSVIEWICON_ENABLED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(16, 16), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("FileChooser.detailsViewIcon") = New NimbusIcon("FileChooser", "detailsViewIconPainter", 16, 16)
			d("FileChooser[Enabled].listViewIconPainter") = New LazyPainter("javax.swing.plaf.nimbus.FileChooserPainter", FileChooserPainter.LISTVIEWICON_ENABLED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(16, 16), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("FileChooser.listViewIcon") = New NimbusIcon("FileChooser", "listViewIconPainter", 16, 16)

			'Initialize InternalFrameTitlePane
			d("InternalFrameTitlePane.contentMargins") = New javax.swing.plaf.InsetsUIResource(0, 0, 0, 0)
			d("InternalFrameTitlePane.maxFrameIconSize") = New javax.swing.plaf.DimensionUIResource(18, 18)

			'Initialize InternalFrame
			d("InternalFrame.contentMargins") = New javax.swing.plaf.InsetsUIResource(1, 6, 6, 6)
			d("InternalFrame.States") = "Enabled,WindowFocused"
			d("InternalFrame.WindowFocused") = New InternalFrameWindowFocusedState
			d("InternalFrame[Enabled].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.InternalFramePainter", InternalFramePainter.BACKGROUND_ENABLED, New java.awt.Insets(25, 6, 6, 6), New java.awt.Dimension(25, 36), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, Double.PositiveInfinity)
			d("InternalFrame[Enabled+WindowFocused].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.InternalFramePainter", InternalFramePainter.BACKGROUND_ENABLED_WINDOWFOCUSED, New java.awt.Insets(25, 6, 6, 6), New java.awt.Dimension(25, 36), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, Double.PositiveInfinity)
			d("InternalFrame:InternalFrameTitlePane.contentMargins") = New javax.swing.plaf.InsetsUIResource(3, 0, 3, 0)
			d("InternalFrame:InternalFrameTitlePane.States") = "Enabled,WindowFocused"
			d("InternalFrame:InternalFrameTitlePane.WindowFocused") = New InternalFrameTitlePaneWindowFocusedState
			d("InternalFrame:InternalFrameTitlePane.titleAlignment") = "CENTER"
			addColor(d, "InternalFrame:InternalFrameTitlePane[Enabled].textForeground", "nimbusDisabledText", 0.0f, 0.0f, 0.0f, 0)
			d("InternalFrame:InternalFrameTitlePane:""InternalFrameTitlePane.menuButton"".contentMargins") = New javax.swing.plaf.InsetsUIResource(0, 0, 0, 0)
			d("InternalFrame:InternalFrameTitlePane:""InternalFrameTitlePane.menuButton"".States") = "Enabled,MouseOver,Pressed,Disabled,Focused,Selected,WindowNotFocused"
			d("InternalFrame:InternalFrameTitlePane:""InternalFrameTitlePane.menuButton"".WindowNotFocused") = New InternalFrameTitlePaneMenuButtonWindowNotFocusedState
			d("InternalFrame:InternalFrameTitlePane:""InternalFrameTitlePane.menuButton"".test") = "am InternalFrameTitlePane.menuButton"
			d("InternalFrame:InternalFrameTitlePane:""InternalFrameTitlePane.menuButton""[Enabled].iconPainter") = New LazyPainter("javax.swing.plaf.nimbus.InternalFrameTitlePaneMenuButtonPainter", InternalFrameTitlePaneMenuButtonPainter.ICON_ENABLED, New java.awt.Insets(0, 0, 0, 0), New java.awt.Dimension(19, 18), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, Double.PositiveInfinity, Double.PositiveInfinity)
			d("InternalFrame:InternalFrameTitlePane:""InternalFrameTitlePane.menuButton""[Disabled].iconPainter") = New LazyPainter("javax.swing.plaf.nimbus.InternalFrameTitlePaneMenuButtonPainter", InternalFrameTitlePaneMenuButtonPainter.ICON_DISABLED, New java.awt.Insets(0, 0, 0, 0), New java.awt.Dimension(19, 18), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, Double.PositiveInfinity, Double.PositiveInfinity)
			d("InternalFrame:InternalFrameTitlePane:""InternalFrameTitlePane.menuButton""[MouseOver].iconPainter") = New LazyPainter("javax.swing.plaf.nimbus.InternalFrameTitlePaneMenuButtonPainter", InternalFrameTitlePaneMenuButtonPainter.ICON_MOUSEOVER, New java.awt.Insets(0, 0, 0, 0), New java.awt.Dimension(19, 18), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, Double.PositiveInfinity, Double.PositiveInfinity)
			d("InternalFrame:InternalFrameTitlePane:""InternalFrameTitlePane.menuButton""[Pressed].iconPainter") = New LazyPainter("javax.swing.plaf.nimbus.InternalFrameTitlePaneMenuButtonPainter", InternalFrameTitlePaneMenuButtonPainter.ICON_PRESSED, New java.awt.Insets(0, 0, 0, 0), New java.awt.Dimension(19, 18), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, Double.PositiveInfinity, Double.PositiveInfinity)
			d("InternalFrame:InternalFrameTitlePane:""InternalFrameTitlePane.menuButton""[Enabled+WindowNotFocused].iconPainter") = New LazyPainter("javax.swing.plaf.nimbus.InternalFrameTitlePaneMenuButtonPainter", InternalFrameTitlePaneMenuButtonPainter.ICON_ENABLED_WINDOWNOTFOCUSED, New java.awt.Insets(0, 0, 0, 0), New java.awt.Dimension(19, 18), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, Double.PositiveInfinity, Double.PositiveInfinity)
			d("InternalFrame:InternalFrameTitlePane:""InternalFrameTitlePane.menuButton""[MouseOver+WindowNotFocused].iconPainter") = New LazyPainter("javax.swing.plaf.nimbus.InternalFrameTitlePaneMenuButtonPainter", InternalFrameTitlePaneMenuButtonPainter.ICON_MOUSEOVER_WINDOWNOTFOCUSED, New java.awt.Insets(0, 0, 0, 0), New java.awt.Dimension(19, 18), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, Double.PositiveInfinity, Double.PositiveInfinity)
			d("InternalFrame:InternalFrameTitlePane:""InternalFrameTitlePane.menuButton""[Pressed+WindowNotFocused].iconPainter") = New LazyPainter("javax.swing.plaf.nimbus.InternalFrameTitlePaneMenuButtonPainter", InternalFrameTitlePaneMenuButtonPainter.ICON_PRESSED_WINDOWNOTFOCUSED, New java.awt.Insets(0, 0, 0, 0), New java.awt.Dimension(19, 18), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, Double.PositiveInfinity, Double.PositiveInfinity)
			d("InternalFrame:InternalFrameTitlePane:""InternalFrameTitlePane.menuButton"".icon") = New NimbusIcon("InternalFrame:InternalFrameTitlePane:""InternalFrameTitlePane.menuButton""", "iconPainter", 19, 18)
			d("InternalFrame:InternalFrameTitlePane:""InternalFrameTitlePane.iconifyButton"".contentMargins") = New javax.swing.plaf.InsetsUIResource(9, 9, 9, 9)
			d("InternalFrame:InternalFrameTitlePane:""InternalFrameTitlePane.iconifyButton"".States") = "Enabled,MouseOver,Pressed,Disabled,Focused,Selected,WindowNotFocused"
			d("InternalFrame:InternalFrameTitlePane:""InternalFrameTitlePane.iconifyButton"".WindowNotFocused") = New InternalFrameTitlePaneIconifyButtonWindowNotFocusedState
			d("InternalFrame:InternalFrameTitlePane:""InternalFrameTitlePane.iconifyButton""[Enabled].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.InternalFrameTitlePaneIconifyButtonPainter", InternalFrameTitlePaneIconifyButtonPainter.BACKGROUND_ENABLED, New java.awt.Insets(0, 0, 0, 0), New java.awt.Dimension(19, 18), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, Double.PositiveInfinity, Double.PositiveInfinity)
			d("InternalFrame:InternalFrameTitlePane:""InternalFrameTitlePane.iconifyButton""[Disabled].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.InternalFrameTitlePaneIconifyButtonPainter", InternalFrameTitlePaneIconifyButtonPainter.BACKGROUND_DISABLED, New java.awt.Insets(0, 0, 0, 0), New java.awt.Dimension(19, 18), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, Double.PositiveInfinity, Double.PositiveInfinity)
			d("InternalFrame:InternalFrameTitlePane:""InternalFrameTitlePane.iconifyButton""[MouseOver].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.InternalFrameTitlePaneIconifyButtonPainter", InternalFrameTitlePaneIconifyButtonPainter.BACKGROUND_MOUSEOVER, New java.awt.Insets(0, 0, 0, 0), New java.awt.Dimension(19, 18), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, Double.PositiveInfinity, Double.PositiveInfinity)
			d("InternalFrame:InternalFrameTitlePane:""InternalFrameTitlePane.iconifyButton""[Pressed].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.InternalFrameTitlePaneIconifyButtonPainter", InternalFrameTitlePaneIconifyButtonPainter.BACKGROUND_PRESSED, New java.awt.Insets(0, 0, 0, 0), New java.awt.Dimension(19, 18), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, Double.PositiveInfinity, Double.PositiveInfinity)
			d("InternalFrame:InternalFrameTitlePane:""InternalFrameTitlePane.iconifyButton""[Enabled+WindowNotFocused].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.InternalFrameTitlePaneIconifyButtonPainter", InternalFrameTitlePaneIconifyButtonPainter.BACKGROUND_ENABLED_WINDOWNOTFOCUSED, New java.awt.Insets(0, 0, 0, 0), New java.awt.Dimension(19, 18), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, Double.PositiveInfinity, Double.PositiveInfinity)
			d("InternalFrame:InternalFrameTitlePane:""InternalFrameTitlePane.iconifyButton""[MouseOver+WindowNotFocused].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.InternalFrameTitlePaneIconifyButtonPainter", InternalFrameTitlePaneIconifyButtonPainter.BACKGROUND_MOUSEOVER_WINDOWNOTFOCUSED, New java.awt.Insets(0, 0, 0, 0), New java.awt.Dimension(19, 18), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, Double.PositiveInfinity, Double.PositiveInfinity)
			d("InternalFrame:InternalFrameTitlePane:""InternalFrameTitlePane.iconifyButton""[Pressed+WindowNotFocused].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.InternalFrameTitlePaneIconifyButtonPainter", InternalFrameTitlePaneIconifyButtonPainter.BACKGROUND_PRESSED_WINDOWNOTFOCUSED, New java.awt.Insets(0, 0, 0, 0), New java.awt.Dimension(19, 18), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, Double.PositiveInfinity, Double.PositiveInfinity)
			d("InternalFrame:InternalFrameTitlePane:""InternalFrameTitlePane.maximizeButton"".contentMargins") = New javax.swing.plaf.InsetsUIResource(9, 9, 9, 9)
			d("InternalFrame:InternalFrameTitlePane:""InternalFrameTitlePane.maximizeButton"".States") = "Enabled,MouseOver,Pressed,Disabled,Focused,Selected,WindowNotFocused,WindowMaximized"
			d("InternalFrame:InternalFrameTitlePane:""InternalFrameTitlePane.maximizeButton"".WindowNotFocused") = New InternalFrameTitlePaneMaximizeButtonWindowNotFocusedState
			d("InternalFrame:InternalFrameTitlePane:""InternalFrameTitlePane.maximizeButton"".WindowMaximized") = New InternalFrameTitlePaneMaximizeButtonWindowMaximizedState
			d("InternalFrame:InternalFrameTitlePane:""InternalFrameTitlePane.maximizeButton""[Disabled+WindowMaximized].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.InternalFrameTitlePaneMaximizeButtonPainter", InternalFrameTitlePaneMaximizeButtonPainter.BACKGROUND_DISABLED_WINDOWMAXIMIZED, New java.awt.Insets(0, 0, 0, 0), New java.awt.Dimension(19, 18), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, Double.PositiveInfinity, Double.PositiveInfinity)
			d("InternalFrame:InternalFrameTitlePane:""InternalFrameTitlePane.maximizeButton""[Enabled+WindowMaximized].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.InternalFrameTitlePaneMaximizeButtonPainter", InternalFrameTitlePaneMaximizeButtonPainter.BACKGROUND_ENABLED_WINDOWMAXIMIZED, New java.awt.Insets(0, 0, 0, 0), New java.awt.Dimension(19, 18), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, Double.PositiveInfinity, Double.PositiveInfinity)
			d("InternalFrame:InternalFrameTitlePane:""InternalFrameTitlePane.maximizeButton""[MouseOver+WindowMaximized].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.InternalFrameTitlePaneMaximizeButtonPainter", InternalFrameTitlePaneMaximizeButtonPainter.BACKGROUND_MOUSEOVER_WINDOWMAXIMIZED, New java.awt.Insets(0, 0, 0, 0), New java.awt.Dimension(19, 18), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, Double.PositiveInfinity, Double.PositiveInfinity)
			d("InternalFrame:InternalFrameTitlePane:""InternalFrameTitlePane.maximizeButton""[Pressed+WindowMaximized].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.InternalFrameTitlePaneMaximizeButtonPainter", InternalFrameTitlePaneMaximizeButtonPainter.BACKGROUND_PRESSED_WINDOWMAXIMIZED, New java.awt.Insets(0, 0, 0, 0), New java.awt.Dimension(19, 18), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, Double.PositiveInfinity, Double.PositiveInfinity)
			d("InternalFrame:InternalFrameTitlePane:""InternalFrameTitlePane.maximizeButton""[Enabled+WindowMaximized+WindowNotFocused].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.InternalFrameTitlePaneMaximizeButtonPainter", InternalFrameTitlePaneMaximizeButtonPainter.BACKGROUND_ENABLED_WINDOWNOTFOCUSED_WINDOWMAXIMIZED, New java.awt.Insets(0, 0, 0, 0), New java.awt.Dimension(19, 18), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, Double.PositiveInfinity, Double.PositiveInfinity)
			d("InternalFrame:InternalFrameTitlePane:""InternalFrameTitlePane.maximizeButton""[MouseOver+WindowMaximized+WindowNotFocused].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.InternalFrameTitlePaneMaximizeButtonPainter", InternalFrameTitlePaneMaximizeButtonPainter.BACKGROUND_MOUSEOVER_WINDOWNOTFOCUSED_WINDOWMAXIMIZED, New java.awt.Insets(0, 0, 0, 0), New java.awt.Dimension(19, 18), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, Double.PositiveInfinity, Double.PositiveInfinity)
			d("InternalFrame:InternalFrameTitlePane:""InternalFrameTitlePane.maximizeButton""[Pressed+WindowMaximized+WindowNotFocused].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.InternalFrameTitlePaneMaximizeButtonPainter", InternalFrameTitlePaneMaximizeButtonPainter.BACKGROUND_PRESSED_WINDOWNOTFOCUSED_WINDOWMAXIMIZED, New java.awt.Insets(0, 0, 0, 0), New java.awt.Dimension(19, 18), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, Double.PositiveInfinity, Double.PositiveInfinity)
			d("InternalFrame:InternalFrameTitlePane:""InternalFrameTitlePane.maximizeButton""[Disabled].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.InternalFrameTitlePaneMaximizeButtonPainter", InternalFrameTitlePaneMaximizeButtonPainter.BACKGROUND_DISABLED, New java.awt.Insets(0, 0, 0, 0), New java.awt.Dimension(19, 18), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, Double.PositiveInfinity, Double.PositiveInfinity)
			d("InternalFrame:InternalFrameTitlePane:""InternalFrameTitlePane.maximizeButton""[Enabled].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.InternalFrameTitlePaneMaximizeButtonPainter", InternalFrameTitlePaneMaximizeButtonPainter.BACKGROUND_ENABLED, New java.awt.Insets(0, 0, 0, 0), New java.awt.Dimension(19, 18), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, Double.PositiveInfinity, Double.PositiveInfinity)
			d("InternalFrame:InternalFrameTitlePane:""InternalFrameTitlePane.maximizeButton""[MouseOver].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.InternalFrameTitlePaneMaximizeButtonPainter", InternalFrameTitlePaneMaximizeButtonPainter.BACKGROUND_MOUSEOVER, New java.awt.Insets(0, 0, 0, 0), New java.awt.Dimension(19, 18), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, Double.PositiveInfinity, Double.PositiveInfinity)
			d("InternalFrame:InternalFrameTitlePane:""InternalFrameTitlePane.maximizeButton""[Pressed].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.InternalFrameTitlePaneMaximizeButtonPainter", InternalFrameTitlePaneMaximizeButtonPainter.BACKGROUND_PRESSED, New java.awt.Insets(0, 0, 0, 0), New java.awt.Dimension(19, 18), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, Double.PositiveInfinity, Double.PositiveInfinity)
			d("InternalFrame:InternalFrameTitlePane:""InternalFrameTitlePane.maximizeButton""[Enabled+WindowNotFocused].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.InternalFrameTitlePaneMaximizeButtonPainter", InternalFrameTitlePaneMaximizeButtonPainter.BACKGROUND_ENABLED_WINDOWNOTFOCUSED, New java.awt.Insets(0, 0, 0, 0), New java.awt.Dimension(19, 18), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, Double.PositiveInfinity, Double.PositiveInfinity)
			d("InternalFrame:InternalFrameTitlePane:""InternalFrameTitlePane.maximizeButton""[MouseOver+WindowNotFocused].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.InternalFrameTitlePaneMaximizeButtonPainter", InternalFrameTitlePaneMaximizeButtonPainter.BACKGROUND_MOUSEOVER_WINDOWNOTFOCUSED, New java.awt.Insets(0, 0, 0, 0), New java.awt.Dimension(19, 18), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, Double.PositiveInfinity, Double.PositiveInfinity)
			d("InternalFrame:InternalFrameTitlePane:""InternalFrameTitlePane.maximizeButton""[Pressed+WindowNotFocused].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.InternalFrameTitlePaneMaximizeButtonPainter", InternalFrameTitlePaneMaximizeButtonPainter.BACKGROUND_PRESSED_WINDOWNOTFOCUSED, New java.awt.Insets(0, 0, 0, 0), New java.awt.Dimension(19, 18), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, Double.PositiveInfinity, Double.PositiveInfinity)
			d("InternalFrame:InternalFrameTitlePane:""InternalFrameTitlePane.closeButton"".contentMargins") = New javax.swing.plaf.InsetsUIResource(9, 9, 9, 9)
			d("InternalFrame:InternalFrameTitlePane:""InternalFrameTitlePane.closeButton"".States") = "Enabled,MouseOver,Pressed,Disabled,Focused,Selected,WindowNotFocused"
			d("InternalFrame:InternalFrameTitlePane:""InternalFrameTitlePane.closeButton"".WindowNotFocused") = New InternalFrameTitlePaneCloseButtonWindowNotFocusedState
			d("InternalFrame:InternalFrameTitlePane:""InternalFrameTitlePane.closeButton""[Disabled].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.InternalFrameTitlePaneCloseButtonPainter", InternalFrameTitlePaneCloseButtonPainter.BACKGROUND_DISABLED, New java.awt.Insets(0, 0, 0, 0), New java.awt.Dimension(19, 18), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, Double.PositiveInfinity, Double.PositiveInfinity)
			d("InternalFrame:InternalFrameTitlePane:""InternalFrameTitlePane.closeButton""[Enabled].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.InternalFrameTitlePaneCloseButtonPainter", InternalFrameTitlePaneCloseButtonPainter.BACKGROUND_ENABLED, New java.awt.Insets(0, 0, 0, 0), New java.awt.Dimension(19, 18), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, Double.PositiveInfinity, Double.PositiveInfinity)
			d("InternalFrame:InternalFrameTitlePane:""InternalFrameTitlePane.closeButton""[MouseOver].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.InternalFrameTitlePaneCloseButtonPainter", InternalFrameTitlePaneCloseButtonPainter.BACKGROUND_MOUSEOVER, New java.awt.Insets(0, 0, 0, 0), New java.awt.Dimension(19, 18), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, Double.PositiveInfinity, Double.PositiveInfinity)
			d("InternalFrame:InternalFrameTitlePane:""InternalFrameTitlePane.closeButton""[Pressed].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.InternalFrameTitlePaneCloseButtonPainter", InternalFrameTitlePaneCloseButtonPainter.BACKGROUND_PRESSED, New java.awt.Insets(0, 0, 0, 0), New java.awt.Dimension(19, 18), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, Double.PositiveInfinity, Double.PositiveInfinity)
			d("InternalFrame:InternalFrameTitlePane:""InternalFrameTitlePane.closeButton""[Enabled+WindowNotFocused].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.InternalFrameTitlePaneCloseButtonPainter", InternalFrameTitlePaneCloseButtonPainter.BACKGROUND_ENABLED_WINDOWNOTFOCUSED, New java.awt.Insets(0, 0, 0, 0), New java.awt.Dimension(19, 18), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, Double.PositiveInfinity, Double.PositiveInfinity)
			d("InternalFrame:InternalFrameTitlePane:""InternalFrameTitlePane.closeButton""[MouseOver+WindowNotFocused].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.InternalFrameTitlePaneCloseButtonPainter", InternalFrameTitlePaneCloseButtonPainter.BACKGROUND_MOUSEOVER_WINDOWNOTFOCUSED, New java.awt.Insets(0, 0, 0, 0), New java.awt.Dimension(19, 18), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, Double.PositiveInfinity, Double.PositiveInfinity)
			d("InternalFrame:InternalFrameTitlePane:""InternalFrameTitlePane.closeButton""[Pressed+WindowNotFocused].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.InternalFrameTitlePaneCloseButtonPainter", InternalFrameTitlePaneCloseButtonPainter.BACKGROUND_PRESSED_WINDOWNOTFOCUSED, New java.awt.Insets(0, 0, 0, 0), New java.awt.Dimension(19, 18), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, Double.PositiveInfinity, Double.PositiveInfinity)

			'Initialize DesktopIcon
			d("DesktopIcon.contentMargins") = New javax.swing.plaf.InsetsUIResource(4, 6, 5, 4)
			d("DesktopIcon[Enabled].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.DesktopIconPainter", DesktopIconPainter.BACKGROUND_ENABLED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(28, 26), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, Double.PositiveInfinity)

			'Initialize DesktopPane
			d("DesktopPane.contentMargins") = New javax.swing.plaf.InsetsUIResource(0, 0, 0, 0)
			d("DesktopPane.opaque") = Boolean.TRUE
			d("DesktopPane[Enabled].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.DesktopPanePainter", DesktopPanePainter.BACKGROUND_ENABLED, New java.awt.Insets(0, 0, 0, 0), New java.awt.Dimension(300, 232), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, Double.PositiveInfinity)

			'Initialize Label
			d("Label.contentMargins") = New javax.swing.plaf.InsetsUIResource(0, 0, 0, 0)
			addColor(d, "Label[Disabled].textForeground", "nimbusDisabledText", 0.0f, 0.0f, 0.0f, 0)

			'Initialize List
			d("List.contentMargins") = New javax.swing.plaf.InsetsUIResource(0, 0, 0, 0)
			d("List.opaque") = Boolean.TRUE
			addColor(d, "List.background", "nimbusLightBackground", 0.0f, 0.0f, 0.0f, 0)
			d("List.rendererUseListColors") = Boolean.TRUE
			d("List.rendererUseUIBorder") = Boolean.TRUE
			d("List.cellNoFocusBorder") = New javax.swing.plaf.BorderUIResource(javax.swing.BorderFactory.createEmptyBorder(2, 5, 2, 5))
			d("List.focusCellHighlightBorder") = New javax.swing.plaf.BorderUIResource(New PainterBorder("Tree:TreeCell[Enabled+Focused].backgroundPainter", New java.awt.Insets(2, 5, 2, 5)))
			addColor(d, "List.dropLineColor", "nimbusFocus", 0.0f, 0.0f, 0.0f, 0)
			addColor(d, "List[Selected].textForeground", "nimbusLightBackground", 0.0f, 0.0f, 0.0f, 0)
			addColor(d, "List[Selected].textBackground", "nimbusSelectionBackground", 0.0f, 0.0f, 0.0f, 0)
			addColor(d, "List[Disabled+Selected].textBackground", "nimbusSelectionBackground", 0.0f, 0.0f, 0.0f, 0)
			addColor(d, "List[Disabled].textForeground", "nimbusDisabledText", 0.0f, 0.0f, 0.0f, 0)
			d("List:""List.cellRenderer"".contentMargins") = New javax.swing.plaf.InsetsUIResource(0, 0, 0, 0)
			d("List:""List.cellRenderer"".opaque") = Boolean.TRUE
			addColor(d, "List:""List.cellRenderer""[Disabled].textForeground", "nimbusDisabledText", 0.0f, 0.0f, 0.0f, 0)
			addColor(d, "List:""List.cellRenderer""[Disabled].background", "nimbusSelectionBackground", 0.0f, 0.0f, 0.0f, 0)

			'Initialize MenuBar
			d("MenuBar.contentMargins") = New javax.swing.plaf.InsetsUIResource(2, 6, 2, 6)
			d("MenuBar[Enabled].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.MenuBarPainter", MenuBarPainter.BACKGROUND_ENABLED, New java.awt.Insets(1, 0, 0, 0), New java.awt.Dimension(18, 22), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, 2.0)
			d("MenuBar[Enabled].borderPainter") = New LazyPainter("javax.swing.plaf.nimbus.MenuBarPainter", MenuBarPainter.BORDER_ENABLED, New java.awt.Insets(0, 0, 1, 0), New java.awt.Dimension(30, 30), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, 2.0)
			d("MenuBar:Menu.contentMargins") = New javax.swing.plaf.InsetsUIResource(1, 4, 2, 4)
			addColor(d, "MenuBar:Menu[Disabled].textForeground", "nimbusDisabledText", 0.0f, 0.0f, 0.0f, 0)
			addColor(d, "MenuBar:Menu[Enabled].textForeground", 35, 35, 36, 255)
			addColor(d, "MenuBar:Menu[Selected].textForeground", 255, 255, 255, 255)
			d("MenuBar:Menu[Selected].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.MenuBarMenuPainter", MenuBarMenuPainter.BACKGROUND_SELECTED, New java.awt.Insets(0, 0, 0, 0), New java.awt.Dimension(100, 30), False, AbstractRegionPainter.PaintContext.CacheMode.NO_CACHING, 1.0, 1.0)
			d("MenuBar:Menu:MenuItemAccelerator.contentMargins") = New javax.swing.plaf.InsetsUIResource(0, 0, 0, 0)

			'Initialize MenuItem
			d("MenuItem.contentMargins") = New javax.swing.plaf.InsetsUIResource(1, 12, 2, 13)
			d("MenuItem.textIconGap") = New Integer?(5)
			addColor(d, "MenuItem[Disabled].textForeground", "nimbusDisabledText", 0.0f, 0.0f, 0.0f, 0)
			addColor(d, "MenuItem[Enabled].textForeground", 35, 35, 36, 255)
			addColor(d, "MenuItem[MouseOver].textForeground", 255, 255, 255, 255)
			d("MenuItem[MouseOver].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.MenuItemPainter", MenuItemPainter.BACKGROUND_MOUSEOVER, New java.awt.Insets(0, 0, 0, 0), New java.awt.Dimension(100, 3), False, AbstractRegionPainter.PaintContext.CacheMode.NO_CACHING, 1.0, 1.0)
			d("MenuItem:MenuItemAccelerator.contentMargins") = New javax.swing.plaf.InsetsUIResource(0, 0, 0, 0)
			addColor(d, "MenuItem:MenuItemAccelerator[Disabled].textForeground", "nimbusDisabledText", 0.0f, 0.0f, 0.0f, 0)
			addColor(d, "MenuItem:MenuItemAccelerator[MouseOver].textForeground", 255, 255, 255, 255)

			'Initialize RadioButtonMenuItem
			d("RadioButtonMenuItem.contentMargins") = New javax.swing.plaf.InsetsUIResource(1, 12, 2, 13)
			d("RadioButtonMenuItem.textIconGap") = New Integer?(5)
			addColor(d, "RadioButtonMenuItem[Disabled].textForeground", "nimbusDisabledText", 0.0f, 0.0f, 0.0f, 0)
			addColor(d, "RadioButtonMenuItem[Enabled].textForeground", 35, 35, 36, 255)
			addColor(d, "RadioButtonMenuItem[MouseOver].textForeground", 255, 255, 255, 255)
			d("RadioButtonMenuItem[MouseOver].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.RadioButtonMenuItemPainter", RadioButtonMenuItemPainter.BACKGROUND_MOUSEOVER, New java.awt.Insets(0, 0, 0, 0), New java.awt.Dimension(100, 3), False, AbstractRegionPainter.PaintContext.CacheMode.NO_CACHING, 1.0, 1.0)
			addColor(d, "RadioButtonMenuItem[MouseOver+Selected].textForeground", 255, 255, 255, 255)
			d("RadioButtonMenuItem[MouseOver+Selected].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.RadioButtonMenuItemPainter", RadioButtonMenuItemPainter.BACKGROUND_SELECTED_MOUSEOVER, New java.awt.Insets(0, 0, 0, 0), New java.awt.Dimension(100, 3), False, AbstractRegionPainter.PaintContext.CacheMode.NO_CACHING, 1.0, 1.0)
			d("RadioButtonMenuItem[Disabled+Selected].checkIconPainter") = New LazyPainter("javax.swing.plaf.nimbus.RadioButtonMenuItemPainter", RadioButtonMenuItemPainter.CHECKICON_DISABLED_SELECTED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(9, 10), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("RadioButtonMenuItem[Enabled+Selected].checkIconPainter") = New LazyPainter("javax.swing.plaf.nimbus.RadioButtonMenuItemPainter", RadioButtonMenuItemPainter.CHECKICON_ENABLED_SELECTED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(9, 10), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("RadioButtonMenuItem[MouseOver+Selected].checkIconPainter") = New LazyPainter("javax.swing.plaf.nimbus.RadioButtonMenuItemPainter", RadioButtonMenuItemPainter.CHECKICON_SELECTED_MOUSEOVER, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(9, 10), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("RadioButtonMenuItem.checkIcon") = New NimbusIcon("RadioButtonMenuItem", "checkIconPainter", 9, 10)
			d("RadioButtonMenuItem:MenuItemAccelerator.contentMargins") = New javax.swing.plaf.InsetsUIResource(0, 0, 0, 0)
			addColor(d, "RadioButtonMenuItem:MenuItemAccelerator[MouseOver].textForeground", 255, 255, 255, 255)

			'Initialize CheckBoxMenuItem
			d("CheckBoxMenuItem.contentMargins") = New javax.swing.plaf.InsetsUIResource(1, 12, 2, 13)
			d("CheckBoxMenuItem.textIconGap") = New Integer?(5)
			addColor(d, "CheckBoxMenuItem[Disabled].textForeground", "nimbusDisabledText", 0.0f, 0.0f, 0.0f, 0)
			addColor(d, "CheckBoxMenuItem[Enabled].textForeground", 35, 35, 36, 255)
			addColor(d, "CheckBoxMenuItem[MouseOver].textForeground", 255, 255, 255, 255)
			d("CheckBoxMenuItem[MouseOver].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.CheckBoxMenuItemPainter", CheckBoxMenuItemPainter.BACKGROUND_MOUSEOVER, New java.awt.Insets(0, 0, 0, 0), New java.awt.Dimension(100, 3), False, AbstractRegionPainter.PaintContext.CacheMode.NO_CACHING, 1.0, 1.0)
			addColor(d, "CheckBoxMenuItem[MouseOver+Selected].textForeground", 255, 255, 255, 255)
			d("CheckBoxMenuItem[MouseOver+Selected].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.CheckBoxMenuItemPainter", CheckBoxMenuItemPainter.BACKGROUND_SELECTED_MOUSEOVER, New java.awt.Insets(0, 0, 0, 0), New java.awt.Dimension(100, 3), False, AbstractRegionPainter.PaintContext.CacheMode.NO_CACHING, 1.0, 1.0)
			d("CheckBoxMenuItem[Disabled+Selected].checkIconPainter") = New LazyPainter("javax.swing.plaf.nimbus.CheckBoxMenuItemPainter", CheckBoxMenuItemPainter.CHECKICON_DISABLED_SELECTED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(9, 10), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("CheckBoxMenuItem[Enabled+Selected].checkIconPainter") = New LazyPainter("javax.swing.plaf.nimbus.CheckBoxMenuItemPainter", CheckBoxMenuItemPainter.CHECKICON_ENABLED_SELECTED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(9, 10), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("CheckBoxMenuItem[MouseOver+Selected].checkIconPainter") = New LazyPainter("javax.swing.plaf.nimbus.CheckBoxMenuItemPainter", CheckBoxMenuItemPainter.CHECKICON_SELECTED_MOUSEOVER, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(9, 10), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("CheckBoxMenuItem.checkIcon") = New NimbusIcon("CheckBoxMenuItem", "checkIconPainter", 9, 10)
			d("CheckBoxMenuItem:MenuItemAccelerator.contentMargins") = New javax.swing.plaf.InsetsUIResource(0, 0, 0, 0)
			addColor(d, "CheckBoxMenuItem:MenuItemAccelerator[MouseOver].textForeground", 255, 255, 255, 255)

			'Initialize Menu
			d("Menu.contentMargins") = New javax.swing.plaf.InsetsUIResource(1, 12, 2, 5)
			d("Menu.textIconGap") = New Integer?(5)
			addColor(d, "Menu[Disabled].textForeground", "nimbusDisabledText", 0.0f, 0.0f, 0.0f, 0)
			addColor(d, "Menu[Enabled].textForeground", 35, 35, 36, 255)
			addColor(d, "Menu[Enabled+Selected].textForeground", 255, 255, 255, 255)
			d("Menu[Enabled+Selected].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.MenuPainter", MenuPainter.BACKGROUND_ENABLED_SELECTED, New java.awt.Insets(0, 0, 0, 0), New java.awt.Dimension(100, 30), False, AbstractRegionPainter.PaintContext.CacheMode.NO_CACHING, 1.0, 1.0)
			d("Menu[Disabled].arrowIconPainter") = New LazyPainter("javax.swing.plaf.nimbus.MenuPainter", MenuPainter.ARROWICON_DISABLED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(9, 10), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("Menu[Enabled].arrowIconPainter") = New LazyPainter("javax.swing.plaf.nimbus.MenuPainter", MenuPainter.ARROWICON_ENABLED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(9, 10), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("Menu[Enabled+Selected].arrowIconPainter") = New LazyPainter("javax.swing.plaf.nimbus.MenuPainter", MenuPainter.ARROWICON_ENABLED_SELECTED, New java.awt.Insets(1, 1, 1, 1), New java.awt.Dimension(9, 10), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("Menu.arrowIcon") = New NimbusIcon("Menu", "arrowIconPainter", 9, 10)
			d("Menu:MenuItemAccelerator.contentMargins") = New javax.swing.plaf.InsetsUIResource(0, 0, 0, 0)
			addColor(d, "Menu:MenuItemAccelerator[MouseOver].textForeground", 255, 255, 255, 255)

			'Initialize PopupMenu
			d("PopupMenu.contentMargins") = New javax.swing.plaf.InsetsUIResource(6, 1, 6, 1)
			d("PopupMenu.opaque") = Boolean.TRUE
			d("PopupMenu.consumeEventOnClose") = Boolean.TRUE
			d("PopupMenu[Disabled].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.PopupMenuPainter", PopupMenuPainter.BACKGROUND_DISABLED, New java.awt.Insets(9, 0, 11, 0), New java.awt.Dimension(220, 313), False, AbstractRegionPainter.PaintContext.CacheMode.NO_CACHING, 1.0, 1.0)
			d("PopupMenu[Enabled].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.PopupMenuPainter", PopupMenuPainter.BACKGROUND_ENABLED, New java.awt.Insets(11, 2, 11, 2), New java.awt.Dimension(220, 313), False, AbstractRegionPainter.PaintContext.CacheMode.NO_CACHING, 1.0, 1.0)

			'Initialize PopupMenuSeparator
			d("PopupMenuSeparator.contentMargins") = New javax.swing.plaf.InsetsUIResource(1, 0, 2, 0)
			d("PopupMenuSeparator[Enabled].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.PopupMenuSeparatorPainter", PopupMenuSeparatorPainter.BACKGROUND_ENABLED, New java.awt.Insets(1, 1, 1, 1), New java.awt.Dimension(3, 3), True, AbstractRegionPainter.PaintContext.CacheMode.NO_CACHING, 1.0, 1.0)

			'Initialize OptionPane
			d("OptionPane.contentMargins") = New javax.swing.plaf.InsetsUIResource(15, 15, 15, 15)
			d("OptionPane.opaque") = Boolean.TRUE
			d("OptionPane.buttonOrientation") = New Integer?(4)
			d("OptionPane.messageAnchor") = New Integer?(17)
			d("OptionPane.separatorPadding") = New Integer?(0)
			d("OptionPane.sameSizeButtons") = Boolean.FALSE
			d("OptionPane:""OptionPane.separator"".contentMargins") = New javax.swing.plaf.InsetsUIResource(1, 0, 0, 0)
			d("OptionPane:""OptionPane.messageArea"".contentMargins") = New javax.swing.plaf.InsetsUIResource(0, 0, 10, 0)
			d("OptionPane:""OptionPane.messageArea"":""OptionPane.label"".contentMargins") = New javax.swing.plaf.InsetsUIResource(0, 10, 10, 10)
			d("OptionPane:""OptionPane.messageArea"":""OptionPane.label""[Enabled].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.OptionPaneMessageAreaOptionPaneLabelPainter", OptionPaneMessageAreaOptionPaneLabelPainter.BACKGROUND_ENABLED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(100, 30), False, AbstractRegionPainter.PaintContext.CacheMode.NO_CACHING, 1.0, 1.0)
			d("OptionPane[Enabled].errorIconPainter") = New LazyPainter("javax.swing.plaf.nimbus.OptionPanePainter", OptionPanePainter.ERRORICON_ENABLED, New java.awt.Insets(0, 0, 0, 0), New java.awt.Dimension(48, 48), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("OptionPane.errorIcon") = New NimbusIcon("OptionPane", "errorIconPainter", 48, 48)
			d("OptionPane[Enabled].informationIconPainter") = New LazyPainter("javax.swing.plaf.nimbus.OptionPanePainter", OptionPanePainter.INFORMATIONICON_ENABLED, New java.awt.Insets(0, 0, 0, 0), New java.awt.Dimension(48, 48), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("OptionPane.informationIcon") = New NimbusIcon("OptionPane", "informationIconPainter", 48, 48)
			d("OptionPane[Enabled].questionIconPainter") = New LazyPainter("javax.swing.plaf.nimbus.OptionPanePainter", OptionPanePainter.QUESTIONICON_ENABLED, New java.awt.Insets(0, 0, 0, 0), New java.awt.Dimension(48, 48), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("OptionPane.questionIcon") = New NimbusIcon("OptionPane", "questionIconPainter", 48, 48)
			d("OptionPane[Enabled].warningIconPainter") = New LazyPainter("javax.swing.plaf.nimbus.OptionPanePainter", OptionPanePainter.WARNINGICON_ENABLED, New java.awt.Insets(0, 0, 0, 0), New java.awt.Dimension(48, 48), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("OptionPane.warningIcon") = New NimbusIcon("OptionPane", "warningIconPainter", 48, 48)

			'Initialize Panel
			d("Panel.contentMargins") = New javax.swing.plaf.InsetsUIResource(0, 0, 0, 0)
			d("Panel.opaque") = Boolean.TRUE

			'Initialize ProgressBar
			d("ProgressBar.contentMargins") = New javax.swing.plaf.InsetsUIResource(0, 0, 0, 0)
			d("ProgressBar.States") = "Enabled,Disabled,Indeterminate,Finished"
			d("ProgressBar.Indeterminate") = New ProgressBarIndeterminateState
			d("ProgressBar.Finished") = New ProgressBarFinishedState
			d("ProgressBar.tileWhenIndeterminate") = Boolean.TRUE
			d("ProgressBar.tileWidth") = New Integer?(27)
			d("ProgressBar.paintOutsideClip") = Boolean.TRUE
			d("ProgressBar.rotateText") = Boolean.TRUE
			d("ProgressBar.vertictalSize") = New javax.swing.plaf.DimensionUIResource(19, 150)
			d("ProgressBar.horizontalSize") = New javax.swing.plaf.DimensionUIResource(150, 19)
			d("ProgressBar.cycleTime") = New Integer?(250)
			d("ProgressBar[Enabled].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ProgressBarPainter", ProgressBarPainter.BACKGROUND_ENABLED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(29, 19), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			addColor(d, "ProgressBar[Disabled].textForeground", "nimbusDisabledText", 0.0f, 0.0f, 0.0f, 0)
			d("ProgressBar[Disabled].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ProgressBarPainter", ProgressBarPainter.BACKGROUND_DISABLED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(29, 19), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("ProgressBar[Enabled].foregroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ProgressBarPainter", ProgressBarPainter.FOREGROUND_ENABLED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(27, 19), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, 2.0)
			d("ProgressBar[Enabled+Finished].foregroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ProgressBarPainter", ProgressBarPainter.FOREGROUND_ENABLED_FINISHED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(27, 19), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, 2.0)
			d("ProgressBar[Enabled+Indeterminate].progressPadding") = New Integer?(3)
			d("ProgressBar[Enabled+Indeterminate].foregroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ProgressBarPainter", ProgressBarPainter.FOREGROUND_ENABLED_INDETERMINATE, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(30, 13), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, 2.0)
			d("ProgressBar[Disabled].foregroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ProgressBarPainter", ProgressBarPainter.FOREGROUND_DISABLED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(27, 19), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, 2.0)
			d("ProgressBar[Disabled+Finished].foregroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ProgressBarPainter", ProgressBarPainter.FOREGROUND_DISABLED_FINISHED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(27, 19), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, 2.0)
			d("ProgressBar[Disabled+Indeterminate].progressPadding") = New Integer?(3)
			d("ProgressBar[Disabled+Indeterminate].foregroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ProgressBarPainter", ProgressBarPainter.FOREGROUND_DISABLED_INDETERMINATE, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(30, 13), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, 2.0)

			'Initialize Separator
			d("Separator.contentMargins") = New javax.swing.plaf.InsetsUIResource(0, 0, 0, 0)
			d("Separator[Enabled].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.SeparatorPainter", SeparatorPainter.BACKGROUND_ENABLED, New java.awt.Insets(0, 40, 0, 40), New java.awt.Dimension(100, 3), True, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)

			'Initialize ScrollBar
			d("ScrollBar.contentMargins") = New javax.swing.plaf.InsetsUIResource(0, 0, 0, 0)
			d("ScrollBar.opaque") = Boolean.TRUE
			d("ScrollBar.incrementButtonGap") = New Integer?(-8)
			d("ScrollBar.decrementButtonGap") = New Integer?(-8)
			d("ScrollBar.thumbHeight") = New Integer?(15)
			d("ScrollBar.minimumThumbSize") = New javax.swing.plaf.DimensionUIResource(29, 29)
			d("ScrollBar.maximumThumbSize") = New javax.swing.plaf.DimensionUIResource(1000, 1000)
			d("ScrollBar:""ScrollBar.button"".contentMargins") = New javax.swing.plaf.InsetsUIResource(0, 0, 0, 0)
			d("ScrollBar:""ScrollBar.button"".size") = New Integer?(25)
			d("ScrollBar:""ScrollBar.button""[Enabled].foregroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ScrollBarButtonPainter", ScrollBarButtonPainter.FOREGROUND_ENABLED, New java.awt.Insets(1, 1, 1, 1), New java.awt.Dimension(25, 15), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("ScrollBar:""ScrollBar.button""[Disabled].foregroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ScrollBarButtonPainter", ScrollBarButtonPainter.FOREGROUND_DISABLED, New java.awt.Insets(1, 1, 1, 1), New java.awt.Dimension(25, 15), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("ScrollBar:""ScrollBar.button""[MouseOver].foregroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ScrollBarButtonPainter", ScrollBarButtonPainter.FOREGROUND_MOUSEOVER, New java.awt.Insets(1, 1, 1, 1), New java.awt.Dimension(25, 15), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("ScrollBar:""ScrollBar.button""[Pressed].foregroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ScrollBarButtonPainter", ScrollBarButtonPainter.FOREGROUND_PRESSED, New java.awt.Insets(1, 1, 1, 1), New java.awt.Dimension(25, 15), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("ScrollBar:ScrollBarThumb.contentMargins") = New javax.swing.plaf.InsetsUIResource(0, 0, 0, 0)
			d("ScrollBar:ScrollBarThumb[Enabled].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ScrollBarThumbPainter", ScrollBarThumbPainter.BACKGROUND_ENABLED, New java.awt.Insets(0, 15, 0, 15), New java.awt.Dimension(38, 15), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, 2.0)
			d("ScrollBar:ScrollBarThumb[MouseOver].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ScrollBarThumbPainter", ScrollBarThumbPainter.BACKGROUND_MOUSEOVER, New java.awt.Insets(0, 15, 0, 15), New java.awt.Dimension(38, 15), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, 2.0)
			d("ScrollBar:ScrollBarThumb[Pressed].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ScrollBarThumbPainter", ScrollBarThumbPainter.BACKGROUND_PRESSED, New java.awt.Insets(0, 15, 0, 15), New java.awt.Dimension(38, 15), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, 2.0)
			d("ScrollBar:ScrollBarTrack.contentMargins") = New javax.swing.plaf.InsetsUIResource(0, 0, 0, 0)
			d("ScrollBar:ScrollBarTrack[Disabled].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ScrollBarTrackPainter", ScrollBarTrackPainter.BACKGROUND_DISABLED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(18, 15), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, 2.0)
			d("ScrollBar:ScrollBarTrack[Enabled].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ScrollBarTrackPainter", ScrollBarTrackPainter.BACKGROUND_ENABLED, New java.awt.Insets(5, 10, 5, 9), New java.awt.Dimension(34, 15), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)

			'Initialize ScrollPane
			d("ScrollPane.contentMargins") = New javax.swing.plaf.InsetsUIResource(3, 3, 3, 3)
			d("ScrollPane.useChildTextComponentFocus") = Boolean.TRUE
			d("ScrollPane[Enabled+Focused].borderPainter") = New LazyPainter("javax.swing.plaf.nimbus.ScrollPanePainter", ScrollPanePainter.BORDER_ENABLED_FOCUSED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(122, 24), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, Double.PositiveInfinity)
			d("ScrollPane[Enabled].borderPainter") = New LazyPainter("javax.swing.plaf.nimbus.ScrollPanePainter", ScrollPanePainter.BORDER_ENABLED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(122, 24), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, Double.PositiveInfinity)

			'Initialize Viewport
			d("Viewport.contentMargins") = New javax.swing.plaf.InsetsUIResource(0, 0, 0, 0)
			d("Viewport.opaque") = Boolean.TRUE

			'Initialize Slider
			d("Slider.contentMargins") = New javax.swing.plaf.InsetsUIResource(0, 0, 0, 0)
			d("Slider.States") = "Enabled,MouseOver,Pressed,Disabled,Focused,Selected,ArrowShape"
			d("Slider.ArrowShape") = New SliderArrowShapeState
			d("Slider.thumbWidth") = New Integer?(17)
			d("Slider.thumbHeight") = New Integer?(17)
			d("Slider.trackBorder") = New Integer?(0)
			d("Slider.paintValue") = Boolean.FALSE
			addColor(d, "Slider.tickColor", 35, 40, 48, 255)
			d("Slider:SliderThumb.contentMargins") = New javax.swing.plaf.InsetsUIResource(0, 0, 0, 0)
			d("Slider:SliderThumb.States") = "Enabled,MouseOver,Pressed,Disabled,Focused,Selected,ArrowShape"
			d("Slider:SliderThumb.ArrowShape") = New SliderThumbArrowShapeState
			d("Slider:SliderThumb[Disabled].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.SliderThumbPainter", SliderThumbPainter.BACKGROUND_DISABLED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(17, 17), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("Slider:SliderThumb[Enabled].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.SliderThumbPainter", SliderThumbPainter.BACKGROUND_ENABLED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(17, 17), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("Slider:SliderThumb[Focused].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.SliderThumbPainter", SliderThumbPainter.BACKGROUND_FOCUSED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(17, 17), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("Slider:SliderThumb[Focused+MouseOver].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.SliderThumbPainter", SliderThumbPainter.BACKGROUND_FOCUSED_MOUSEOVER, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(17, 17), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("Slider:SliderThumb[Focused+Pressed].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.SliderThumbPainter", SliderThumbPainter.BACKGROUND_FOCUSED_PRESSED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(17, 17), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("Slider:SliderThumb[MouseOver].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.SliderThumbPainter", SliderThumbPainter.BACKGROUND_MOUSEOVER, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(17, 17), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("Slider:SliderThumb[Pressed].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.SliderThumbPainter", SliderThumbPainter.BACKGROUND_PRESSED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(17, 17), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("Slider:SliderThumb[ArrowShape+Enabled].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.SliderThumbPainter", SliderThumbPainter.BACKGROUND_ENABLED_ARROWSHAPE, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(17, 17), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("Slider:SliderThumb[ArrowShape+Disabled].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.SliderThumbPainter", SliderThumbPainter.BACKGROUND_DISABLED_ARROWSHAPE, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(17, 17), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("Slider:SliderThumb[ArrowShape+MouseOver].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.SliderThumbPainter", SliderThumbPainter.BACKGROUND_MOUSEOVER_ARROWSHAPE, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(17, 17), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("Slider:SliderThumb[ArrowShape+Pressed].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.SliderThumbPainter", SliderThumbPainter.BACKGROUND_PRESSED_ARROWSHAPE, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(17, 17), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("Slider:SliderThumb[ArrowShape+Focused].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.SliderThumbPainter", SliderThumbPainter.BACKGROUND_FOCUSED_ARROWSHAPE, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(17, 17), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("Slider:SliderThumb[ArrowShape+Focused+MouseOver].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.SliderThumbPainter", SliderThumbPainter.BACKGROUND_FOCUSED_MOUSEOVER_ARROWSHAPE, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(17, 17), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("Slider:SliderThumb[ArrowShape+Focused+Pressed].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.SliderThumbPainter", SliderThumbPainter.BACKGROUND_FOCUSED_PRESSED_ARROWSHAPE, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(17, 17), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("Slider:SliderTrack.contentMargins") = New javax.swing.plaf.InsetsUIResource(0, 0, 0, 0)
			d("Slider:SliderTrack.States") = "Enabled,MouseOver,Pressed,Disabled,Focused,Selected,ArrowShape"
			d("Slider:SliderTrack.ArrowShape") = New SliderTrackArrowShapeState
			d("Slider:SliderTrack[Disabled].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.SliderTrackPainter", SliderTrackPainter.BACKGROUND_DISABLED, New java.awt.Insets(6, 5, 6, 5), New java.awt.Dimension(23, 17), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, 2.0)
			d("Slider:SliderTrack[Enabled].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.SliderTrackPainter", SliderTrackPainter.BACKGROUND_ENABLED, New java.awt.Insets(6, 5, 6, 5), New java.awt.Dimension(23, 17), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)

			'Initialize Spinner
			d("Spinner.contentMargins") = New javax.swing.plaf.InsetsUIResource(0, 0, 0, 0)
			d("Spinner:""Spinner.editor"".contentMargins") = New javax.swing.plaf.InsetsUIResource(0, 0, 0, 0)
			d("Spinner:Panel:""Spinner.formattedTextField"".contentMargins") = New javax.swing.plaf.InsetsUIResource(6, 6, 5, 6)
			addColor(d, "Spinner:Panel:""Spinner.formattedTextField""[Disabled].textForeground", "nimbusDisabledText", 0.0f, 0.0f, 0.0f, 0)
			d("Spinner:Panel:""Spinner.formattedTextField""[Disabled].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.SpinnerPanelSpinnerFormattedTextFieldPainter", SpinnerPanelSpinnerFormattedTextFieldPainter.BACKGROUND_DISABLED, New java.awt.Insets(5, 3, 3, 1), New java.awt.Dimension(64, 24), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, Double.PositiveInfinity)
			d("Spinner:Panel:""Spinner.formattedTextField""[Enabled].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.SpinnerPanelSpinnerFormattedTextFieldPainter", SpinnerPanelSpinnerFormattedTextFieldPainter.BACKGROUND_ENABLED, New java.awt.Insets(5, 3, 3, 1), New java.awt.Dimension(64, 24), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, Double.PositiveInfinity)
			d("Spinner:Panel:""Spinner.formattedTextField""[Focused].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.SpinnerPanelSpinnerFormattedTextFieldPainter", SpinnerPanelSpinnerFormattedTextFieldPainter.BACKGROUND_FOCUSED, New java.awt.Insets(5, 3, 3, 1), New java.awt.Dimension(64, 24), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, Double.PositiveInfinity)
			addColor(d, "Spinner:Panel:""Spinner.formattedTextField""[Selected].textForeground", "nimbusSelectedText", 0.0f, 0.0f, 0.0f, 0)
			d("Spinner:Panel:""Spinner.formattedTextField""[Selected].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.SpinnerPanelSpinnerFormattedTextFieldPainter", SpinnerPanelSpinnerFormattedTextFieldPainter.BACKGROUND_SELECTED, New java.awt.Insets(5, 3, 3, 1), New java.awt.Dimension(64, 24), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, Double.PositiveInfinity)
			addColor(d, "Spinner:Panel:""Spinner.formattedTextField""[Focused+Selected].textForeground", "nimbusSelectedText", 0.0f, 0.0f, 0.0f, 0)
			d("Spinner:Panel:""Spinner.formattedTextField""[Focused+Selected].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.SpinnerPanelSpinnerFormattedTextFieldPainter", SpinnerPanelSpinnerFormattedTextFieldPainter.BACKGROUND_SELECTED_FOCUSED, New java.awt.Insets(5, 3, 3, 1), New java.awt.Dimension(64, 24), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, Double.PositiveInfinity)
			d("Spinner:""Spinner.previousButton"".contentMargins") = New javax.swing.plaf.InsetsUIResource(0, 0, 0, 0)
			d("Spinner:""Spinner.previousButton"".size") = New Integer?(20)
			d("Spinner:""Spinner.previousButton""[Disabled].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.SpinnerPreviousButtonPainter", SpinnerPreviousButtonPainter.BACKGROUND_DISABLED, New java.awt.Insets(0, 1, 6, 7), New java.awt.Dimension(20, 12), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("Spinner:""Spinner.previousButton""[Enabled].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.SpinnerPreviousButtonPainter", SpinnerPreviousButtonPainter.BACKGROUND_ENABLED, New java.awt.Insets(0, 1, 6, 7), New java.awt.Dimension(20, 12), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("Spinner:""Spinner.previousButton""[Focused].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.SpinnerPreviousButtonPainter", SpinnerPreviousButtonPainter.BACKGROUND_FOCUSED, New java.awt.Insets(0, 1, 6, 7), New java.awt.Dimension(20, 12), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("Spinner:""Spinner.previousButton""[Focused+MouseOver].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.SpinnerPreviousButtonPainter", SpinnerPreviousButtonPainter.BACKGROUND_MOUSEOVER_FOCUSED, New java.awt.Insets(3, 1, 6, 7), New java.awt.Dimension(20, 12), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("Spinner:""Spinner.previousButton""[Focused+Pressed].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.SpinnerPreviousButtonPainter", SpinnerPreviousButtonPainter.BACKGROUND_PRESSED_FOCUSED, New java.awt.Insets(0, 1, 6, 7), New java.awt.Dimension(20, 12), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("Spinner:""Spinner.previousButton""[MouseOver].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.SpinnerPreviousButtonPainter", SpinnerPreviousButtonPainter.BACKGROUND_MOUSEOVER, New java.awt.Insets(0, 1, 6, 7), New java.awt.Dimension(20, 12), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("Spinner:""Spinner.previousButton""[Pressed].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.SpinnerPreviousButtonPainter", SpinnerPreviousButtonPainter.BACKGROUND_PRESSED, New java.awt.Insets(0, 1, 6, 7), New java.awt.Dimension(20, 12), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("Spinner:""Spinner.previousButton""[Disabled].foregroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.SpinnerPreviousButtonPainter", SpinnerPreviousButtonPainter.FOREGROUND_DISABLED, New java.awt.Insets(3, 6, 5, 9), New java.awt.Dimension(20, 12), True, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("Spinner:""Spinner.previousButton""[Enabled].foregroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.SpinnerPreviousButtonPainter", SpinnerPreviousButtonPainter.FOREGROUND_ENABLED, New java.awt.Insets(3, 6, 5, 9), New java.awt.Dimension(20, 12), True, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("Spinner:""Spinner.previousButton""[Focused].foregroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.SpinnerPreviousButtonPainter", SpinnerPreviousButtonPainter.FOREGROUND_FOCUSED, New java.awt.Insets(3, 6, 5, 9), New java.awt.Dimension(20, 12), True, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("Spinner:""Spinner.previousButton""[Focused+MouseOver].foregroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.SpinnerPreviousButtonPainter", SpinnerPreviousButtonPainter.FOREGROUND_MOUSEOVER_FOCUSED, New java.awt.Insets(3, 6, 5, 9), New java.awt.Dimension(20, 12), True, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("Spinner:""Spinner.previousButton""[Focused+Pressed].foregroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.SpinnerPreviousButtonPainter", SpinnerPreviousButtonPainter.FOREGROUND_PRESSED_FOCUSED, New java.awt.Insets(3, 6, 5, 9), New java.awt.Dimension(20, 12), True, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("Spinner:""Spinner.previousButton""[MouseOver].foregroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.SpinnerPreviousButtonPainter", SpinnerPreviousButtonPainter.FOREGROUND_MOUSEOVER, New java.awt.Insets(3, 6, 5, 9), New java.awt.Dimension(20, 12), True, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("Spinner:""Spinner.previousButton""[Pressed].foregroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.SpinnerPreviousButtonPainter", SpinnerPreviousButtonPainter.FOREGROUND_PRESSED, New java.awt.Insets(3, 6, 5, 9), New java.awt.Dimension(20, 12), True, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("Spinner:""Spinner.nextButton"".contentMargins") = New javax.swing.plaf.InsetsUIResource(0, 0, 0, 0)
			d("Spinner:""Spinner.nextButton"".size") = New Integer?(20)
			d("Spinner:""Spinner.nextButton""[Disabled].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.SpinnerNextButtonPainter", SpinnerNextButtonPainter.BACKGROUND_DISABLED, New java.awt.Insets(7, 1, 1, 7), New java.awt.Dimension(20, 12), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("Spinner:""Spinner.nextButton""[Enabled].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.SpinnerNextButtonPainter", SpinnerNextButtonPainter.BACKGROUND_ENABLED, New java.awt.Insets(7, 1, 1, 7), New java.awt.Dimension(20, 12), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("Spinner:""Spinner.nextButton""[Focused].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.SpinnerNextButtonPainter", SpinnerNextButtonPainter.BACKGROUND_FOCUSED, New java.awt.Insets(7, 1, 1, 7), New java.awt.Dimension(20, 12), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("Spinner:""Spinner.nextButton""[Focused+MouseOver].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.SpinnerNextButtonPainter", SpinnerNextButtonPainter.BACKGROUND_MOUSEOVER_FOCUSED, New java.awt.Insets(7, 1, 1, 7), New java.awt.Dimension(20, 12), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("Spinner:""Spinner.nextButton""[Focused+Pressed].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.SpinnerNextButtonPainter", SpinnerNextButtonPainter.BACKGROUND_PRESSED_FOCUSED, New java.awt.Insets(7, 1, 1, 7), New java.awt.Dimension(20, 12), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("Spinner:""Spinner.nextButton""[MouseOver].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.SpinnerNextButtonPainter", SpinnerNextButtonPainter.BACKGROUND_MOUSEOVER, New java.awt.Insets(7, 1, 1, 7), New java.awt.Dimension(20, 12), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("Spinner:""Spinner.nextButton""[Pressed].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.SpinnerNextButtonPainter", SpinnerNextButtonPainter.BACKGROUND_PRESSED, New java.awt.Insets(7, 1, 1, 7), New java.awt.Dimension(20, 12), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("Spinner:""Spinner.nextButton""[Disabled].foregroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.SpinnerNextButtonPainter", SpinnerNextButtonPainter.FOREGROUND_DISABLED, New java.awt.Insets(5, 6, 3, 9), New java.awt.Dimension(20, 12), True, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("Spinner:""Spinner.nextButton""[Enabled].foregroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.SpinnerNextButtonPainter", SpinnerNextButtonPainter.FOREGROUND_ENABLED, New java.awt.Insets(5, 6, 3, 9), New java.awt.Dimension(20, 12), True, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("Spinner:""Spinner.nextButton""[Focused].foregroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.SpinnerNextButtonPainter", SpinnerNextButtonPainter.FOREGROUND_FOCUSED, New java.awt.Insets(3, 6, 3, 9), New java.awt.Dimension(20, 12), True, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("Spinner:""Spinner.nextButton""[Focused+MouseOver].foregroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.SpinnerNextButtonPainter", SpinnerNextButtonPainter.FOREGROUND_MOUSEOVER_FOCUSED, New java.awt.Insets(3, 6, 3, 9), New java.awt.Dimension(20, 12), True, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("Spinner:""Spinner.nextButton""[Focused+Pressed].foregroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.SpinnerNextButtonPainter", SpinnerNextButtonPainter.FOREGROUND_PRESSED_FOCUSED, New java.awt.Insets(5, 6, 3, 9), New java.awt.Dimension(20, 12), True, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("Spinner:""Spinner.nextButton""[MouseOver].foregroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.SpinnerNextButtonPainter", SpinnerNextButtonPainter.FOREGROUND_MOUSEOVER, New java.awt.Insets(5, 6, 3, 9), New java.awt.Dimension(20, 12), True, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("Spinner:""Spinner.nextButton""[Pressed].foregroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.SpinnerNextButtonPainter", SpinnerNextButtonPainter.FOREGROUND_PRESSED, New java.awt.Insets(5, 6, 3, 9), New java.awt.Dimension(20, 12), True, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)

			'Initialize SplitPane
			d("SplitPane.contentMargins") = New javax.swing.plaf.InsetsUIResource(1, 1, 1, 1)
			d("SplitPane.States") = "Enabled,MouseOver,Pressed,Disabled,Focused,Selected,Vertical"
			d("SplitPane.Vertical") = New SplitPaneVerticalState
			d("SplitPane.size") = New Integer?(10)
			d("SplitPane.dividerSize") = New Integer?(10)
			d("SplitPane.centerOneTouchButtons") = Boolean.TRUE
			d("SplitPane.oneTouchButtonOffset") = New Integer?(30)
			d("SplitPane.oneTouchExpandable") = Boolean.FALSE
			d("SplitPane.continuousLayout") = Boolean.TRUE
			d("SplitPane:SplitPaneDivider.contentMargins") = New javax.swing.plaf.InsetsUIResource(0, 0, 0, 0)
			d("SplitPane:SplitPaneDivider.States") = "Enabled,MouseOver,Pressed,Disabled,Focused,Selected,Vertical"
			d("SplitPane:SplitPaneDivider.Vertical") = New SplitPaneDividerVerticalState
			d("SplitPane:SplitPaneDivider[Enabled].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.SplitPaneDividerPainter", SplitPaneDividerPainter.BACKGROUND_ENABLED, New java.awt.Insets(3, 0, 3, 0), New java.awt.Dimension(68, 10), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, Double.PositiveInfinity)
			d("SplitPane:SplitPaneDivider[Focused].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.SplitPaneDividerPainter", SplitPaneDividerPainter.BACKGROUND_FOCUSED, New java.awt.Insets(3, 0, 3, 0), New java.awt.Dimension(68, 10), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, Double.PositiveInfinity)
			d("SplitPane:SplitPaneDivider[Enabled].foregroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.SplitPaneDividerPainter", SplitPaneDividerPainter.FOREGROUND_ENABLED, New java.awt.Insets(0, 24, 0, 24), New java.awt.Dimension(68, 10), True, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, Double.PositiveInfinity)
			d("SplitPane:SplitPaneDivider[Enabled+Vertical].foregroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.SplitPaneDividerPainter", SplitPaneDividerPainter.FOREGROUND_ENABLED_VERTICAL, New java.awt.Insets(5, 0, 5, 0), New java.awt.Dimension(10, 38), True, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, Double.PositiveInfinity)

			'Initialize TabbedPane
			d("TabbedPane.contentMargins") = New javax.swing.plaf.InsetsUIResource(0, 0, 0, 0)
			d("TabbedPane.tabAreaStatesMatchSelectedTab") = Boolean.TRUE
			d("TabbedPane.nudgeSelectedLabel") = Boolean.FALSE
			d("TabbedPane.tabRunOverlay") = New Integer?(2)
			d("TabbedPane.tabOverlap") = New Integer?(-1)
			d("TabbedPane.extendTabsToBase") = Boolean.TRUE
			d("TabbedPane.useBasicArrows") = Boolean.TRUE
			addColor(d, "TabbedPane.shadow", "nimbusDisabledText", 0.0f, 0.0f, 0.0f, 0)
			addColor(d, "TabbedPane.darkShadow", "text", 0.0f, 0.0f, 0.0f, 0)
			addColor(d, "TabbedPane.highlight", "nimbusLightBackground", 0.0f, 0.0f, 0.0f, 0)
			d("TabbedPane:TabbedPaneTab.contentMargins") = New javax.swing.plaf.InsetsUIResource(2, 8, 3, 8)
			d("TabbedPane:TabbedPaneTab[Enabled].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.TabbedPaneTabPainter", TabbedPaneTabPainter.BACKGROUND_ENABLED, New java.awt.Insets(7, 7, 1, 7), New java.awt.Dimension(44, 20), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, Double.PositiveInfinity)
			d("TabbedPane:TabbedPaneTab[Enabled+MouseOver].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.TabbedPaneTabPainter", TabbedPaneTabPainter.BACKGROUND_ENABLED_MOUSEOVER, New java.awt.Insets(7, 7, 1, 7), New java.awt.Dimension(44, 20), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, Double.PositiveInfinity)
			d("TabbedPane:TabbedPaneTab[Enabled+Pressed].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.TabbedPaneTabPainter", TabbedPaneTabPainter.BACKGROUND_ENABLED_PRESSED, New java.awt.Insets(7, 6, 1, 7), New java.awt.Dimension(44, 20), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, Double.PositiveInfinity)
			addColor(d, "TabbedPane:TabbedPaneTab[Disabled].textForeground", "nimbusDisabledText", 0.0f, 0.0f, 0.0f, 0)
			d("TabbedPane:TabbedPaneTab[Disabled].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.TabbedPaneTabPainter", TabbedPaneTabPainter.BACKGROUND_DISABLED, New java.awt.Insets(6, 7, 1, 7), New java.awt.Dimension(44, 20), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, Double.PositiveInfinity)
			d("TabbedPane:TabbedPaneTab[Disabled+Selected].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.TabbedPaneTabPainter", TabbedPaneTabPainter.BACKGROUND_SELECTED_DISABLED, New java.awt.Insets(7, 7, 0, 7), New java.awt.Dimension(44, 20), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, Double.PositiveInfinity)
			d("TabbedPane:TabbedPaneTab[Selected].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.TabbedPaneTabPainter", TabbedPaneTabPainter.BACKGROUND_SELECTED, New java.awt.Insets(7, 7, 0, 7), New java.awt.Dimension(44, 20), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, Double.PositiveInfinity)
			d("TabbedPane:TabbedPaneTab[MouseOver+Selected].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.TabbedPaneTabPainter", TabbedPaneTabPainter.BACKGROUND_SELECTED_MOUSEOVER, New java.awt.Insets(7, 9, 0, 9), New java.awt.Dimension(44, 20), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, Double.PositiveInfinity)
			addColor(d, "TabbedPane:TabbedPaneTab[Pressed+Selected].textForeground", 255, 255, 255, 255)
			d("TabbedPane:TabbedPaneTab[Pressed+Selected].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.TabbedPaneTabPainter", TabbedPaneTabPainter.BACKGROUND_SELECTED_PRESSED, New java.awt.Insets(7, 9, 0, 9), New java.awt.Dimension(44, 20), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, Double.PositiveInfinity)
			d("TabbedPane:TabbedPaneTab[Focused+Selected].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.TabbedPaneTabPainter", TabbedPaneTabPainter.BACKGROUND_SELECTED_FOCUSED, New java.awt.Insets(7, 7, 3, 7), New java.awt.Dimension(44, 20), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, Double.PositiveInfinity)
			d("TabbedPane:TabbedPaneTab[Focused+MouseOver+Selected].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.TabbedPaneTabPainter", TabbedPaneTabPainter.BACKGROUND_SELECTED_MOUSEOVER_FOCUSED, New java.awt.Insets(7, 9, 3, 9), New java.awt.Dimension(44, 20), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, Double.PositiveInfinity)
			addColor(d, "TabbedPane:TabbedPaneTab[Focused+Pressed+Selected].textForeground", 255, 255, 255, 255)
			d("TabbedPane:TabbedPaneTab[Focused+Pressed+Selected].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.TabbedPaneTabPainter", TabbedPaneTabPainter.BACKGROUND_SELECTED_PRESSED_FOCUSED, New java.awt.Insets(7, 9, 3, 9), New java.awt.Dimension(44, 20), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, Double.PositiveInfinity)
			d("TabbedPane:TabbedPaneTabArea.contentMargins") = New javax.swing.plaf.InsetsUIResource(3, 10, 4, 10)
			d("TabbedPane:TabbedPaneTabArea[Enabled].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.TabbedPaneTabAreaPainter", TabbedPaneTabAreaPainter.BACKGROUND_ENABLED, New java.awt.Insets(0, 5, 6, 5), New java.awt.Dimension(5, 24), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, Double.PositiveInfinity)
			d("TabbedPane:TabbedPaneTabArea[Disabled].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.TabbedPaneTabAreaPainter", TabbedPaneTabAreaPainter.BACKGROUND_DISABLED, New java.awt.Insets(0, 5, 6, 5), New java.awt.Dimension(5, 24), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, Double.PositiveInfinity)
			d("TabbedPane:TabbedPaneTabArea[Enabled+MouseOver].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.TabbedPaneTabAreaPainter", TabbedPaneTabAreaPainter.BACKGROUND_ENABLED_MOUSEOVER, New java.awt.Insets(0, 5, 6, 5), New java.awt.Dimension(5, 24), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, Double.PositiveInfinity)
			d("TabbedPane:TabbedPaneTabArea[Enabled+Pressed].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.TabbedPaneTabAreaPainter", TabbedPaneTabAreaPainter.BACKGROUND_ENABLED_PRESSED, New java.awt.Insets(0, 5, 6, 5), New java.awt.Dimension(5, 24), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, Double.PositiveInfinity)
			d("TabbedPane:TabbedPaneContent.contentMargins") = New javax.swing.plaf.InsetsUIResource(0, 0, 0, 0)

			'Initialize Table
			d("Table.contentMargins") = New javax.swing.plaf.InsetsUIResource(0, 0, 0, 0)
			d("Table.opaque") = Boolean.TRUE
			addColor(d, "Table.textForeground", 35, 35, 36, 255)
			addColor(d, "Table.background", "nimbusLightBackground", 0.0f, 0.0f, 0.0f, 0)
			d("Table.showGrid") = Boolean.FALSE
			d("Table.intercellSpacing") = New javax.swing.plaf.DimensionUIResource(0, 0)
			addColor(d, "Table.alternateRowColor", "nimbusLightBackground", 0.0f, 0.0f, -0.05098039f, 0, False)
			d("Table.rendererUseTableColors") = Boolean.TRUE
			d("Table.rendererUseUIBorder") = Boolean.TRUE
			d("Table.cellNoFocusBorder") = New javax.swing.plaf.BorderUIResource(javax.swing.BorderFactory.createEmptyBorder(2, 5, 2, 5))
			d("Table.focusCellHighlightBorder") = New javax.swing.plaf.BorderUIResource(New PainterBorder("Tree:TreeCell[Enabled+Focused].backgroundPainter", New java.awt.Insets(2, 5, 2, 5)))
			addColor(d, "Table.dropLineColor", "nimbusFocus", 0.0f, 0.0f, 0.0f, 0)
			addColor(d, "Table.dropLineShortColor", "nimbusOrange", 0.0f, 0.0f, 0.0f, 0)
			addColor(d, "Table[Enabled+Selected].textForeground", "nimbusLightBackground", 0.0f, 0.0f, 0.0f, 0, False)
			addColor(d, "Table[Enabled+Selected].textBackground", "nimbusSelectionBackground", 0.0f, 0.0f, 0.0f, 0, False)
			addColor(d, "Table[Disabled+Selected].textBackground", "nimbusSelectionBackground", 0.0f, 0.0f, 0.0f, 0, False)
			d("Table:""Table.cellRenderer"".contentMargins") = New javax.swing.plaf.InsetsUIResource(0, 0, 0, 0)
			d("Table:""Table.cellRenderer"".opaque") = Boolean.TRUE
			addColor(d, "Table:""Table.cellRenderer"".background", "nimbusLightBackground", 0.0f, 0.0f, 0.0f, 0, False)

			'Initialize TableHeader
			d("TableHeader.contentMargins") = New javax.swing.plaf.InsetsUIResource(0, 0, 0, 0)
			d("TableHeader.opaque") = Boolean.TRUE
			d("TableHeader.rightAlignSortArrow") = Boolean.TRUE
			d("TableHeader[Enabled].ascendingSortIconPainter") = New LazyPainter("javax.swing.plaf.nimbus.TableHeaderPainter", TableHeaderPainter.ASCENDINGSORTICON_ENABLED, New java.awt.Insets(0, 0, 0, 2), New java.awt.Dimension(7, 7), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("Table.ascendingSortIcon") = New NimbusIcon("TableHeader", "ascendingSortIconPainter", 7, 7)
			d("TableHeader[Enabled].descendingSortIconPainter") = New LazyPainter("javax.swing.plaf.nimbus.TableHeaderPainter", TableHeaderPainter.DESCENDINGSORTICON_ENABLED, New java.awt.Insets(0, 0, 0, 0), New java.awt.Dimension(7, 7), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("Table.descendingSortIcon") = New NimbusIcon("TableHeader", "descendingSortIconPainter", 7, 7)
			d("TableHeader:""TableHeader.renderer"".contentMargins") = New javax.swing.plaf.InsetsUIResource(2, 5, 4, 5)
			d("TableHeader:""TableHeader.renderer"".opaque") = Boolean.TRUE
			d("TableHeader:""TableHeader.renderer"".States") = "Enabled,MouseOver,Pressed,Disabled,Focused,Selected,Sorted"
			d("TableHeader:""TableHeader.renderer"".Sorted") = New TableHeaderRendererSortedState
			d("TableHeader:""TableHeader.renderer""[Disabled].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.TableHeaderRendererPainter", TableHeaderRendererPainter.BACKGROUND_DISABLED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(22, 20), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, Double.PositiveInfinity)
			d("TableHeader:""TableHeader.renderer""[Enabled].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.TableHeaderRendererPainter", TableHeaderRendererPainter.BACKGROUND_ENABLED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(22, 20), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, Double.PositiveInfinity)
			d("TableHeader:""TableHeader.renderer""[Enabled+Focused].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.TableHeaderRendererPainter", TableHeaderRendererPainter.BACKGROUND_ENABLED_FOCUSED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(22, 20), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, Double.PositiveInfinity)
			d("TableHeader:""TableHeader.renderer""[MouseOver].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.TableHeaderRendererPainter", TableHeaderRendererPainter.BACKGROUND_MOUSEOVER, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(22, 20), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, Double.PositiveInfinity)
			d("TableHeader:""TableHeader.renderer""[Pressed].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.TableHeaderRendererPainter", TableHeaderRendererPainter.BACKGROUND_PRESSED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(22, 20), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, Double.PositiveInfinity)
			d("TableHeader:""TableHeader.renderer""[Enabled+Sorted].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.TableHeaderRendererPainter", TableHeaderRendererPainter.BACKGROUND_ENABLED_SORTED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(22, 20), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, Double.PositiveInfinity)
			d("TableHeader:""TableHeader.renderer""[Enabled+Focused+Sorted].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.TableHeaderRendererPainter", TableHeaderRendererPainter.BACKGROUND_ENABLED_FOCUSED_SORTED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(22, 20), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, Double.PositiveInfinity)
			d("TableHeader:""TableHeader.renderer""[Disabled+Sorted].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.TableHeaderRendererPainter", TableHeaderRendererPainter.BACKGROUND_DISABLED_SORTED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(22, 20), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, Double.PositiveInfinity)

			'Initialize \"Table.editor\"
			d("""Table.editor"".contentMargins") = New javax.swing.plaf.InsetsUIResource(3, 5, 3, 5)
			d("""Table.editor"".opaque") = Boolean.TRUE
			addColor(d, """Table.editor"".background", "nimbusLightBackground", 0.0f, 0.0f, 0.0f, 0)
			addColor(d, """Table.editor""[Disabled].textForeground", "nimbusDisabledText", 0.0f, 0.0f, 0.0f, 0)
			d("""Table.editor""[Enabled].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.TableEditorPainter", TableEditorPainter.BACKGROUND_ENABLED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(100, 30), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, Double.PositiveInfinity)
			d("""Table.editor""[Enabled+Focused].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.TableEditorPainter", TableEditorPainter.BACKGROUND_ENABLED_FOCUSED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(100, 30), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, Double.PositiveInfinity)
			addColor(d, """Table.editor""[Selected].textForeground", "nimbusSelectedText", 0.0f, 0.0f, 0.0f, 0)

			'Initialize \"Tree.cellEditor\"
			d("""Tree.cellEditor"".contentMargins") = New javax.swing.plaf.InsetsUIResource(2, 5, 2, 5)
			d("""Tree.cellEditor"".opaque") = Boolean.TRUE
			addColor(d, """Tree.cellEditor"".background", "nimbusLightBackground", 0.0f, 0.0f, 0.0f, 0)
			addColor(d, """Tree.cellEditor""[Disabled].textForeground", "nimbusDisabledText", 0.0f, 0.0f, 0.0f, 0)
			d("""Tree.cellEditor""[Enabled].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.TreeCellEditorPainter", TreeCellEditorPainter.BACKGROUND_ENABLED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(100, 30), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, Double.PositiveInfinity)
			d("""Tree.cellEditor""[Enabled+Focused].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.TreeCellEditorPainter", TreeCellEditorPainter.BACKGROUND_ENABLED_FOCUSED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(100, 30), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, Double.PositiveInfinity)
			addColor(d, """Tree.cellEditor""[Selected].textForeground", "nimbusSelectedText", 0.0f, 0.0f, 0.0f, 0)

			'Initialize TextField
			d("TextField.contentMargins") = New javax.swing.plaf.InsetsUIResource(6, 6, 6, 6)
			addColor(d, "TextField.background", "nimbusLightBackground", 0.0f, 0.0f, 0.0f, 0)
			addColor(d, "TextField[Disabled].textForeground", "nimbusDisabledText", 0.0f, 0.0f, 0.0f, 0)
			d("TextField[Disabled].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.TextFieldPainter", TextFieldPainter.BACKGROUND_DISABLED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(122, 24), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, Double.PositiveInfinity)
			d("TextField[Enabled].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.TextFieldPainter", TextFieldPainter.BACKGROUND_ENABLED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(122, 24), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, Double.PositiveInfinity)
			addColor(d, "TextField[Selected].textForeground", "nimbusSelectedText", 0.0f, 0.0f, 0.0f, 0)
			d("TextField[Selected].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.TextFieldPainter", TextFieldPainter.BACKGROUND_SELECTED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(122, 24), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, Double.PositiveInfinity)
			addColor(d, "TextField[Disabled].textForeground", "nimbusDisabledText", 0.0f, 0.0f, 0.0f, 0)
			d("TextField[Disabled].borderPainter") = New LazyPainter("javax.swing.plaf.nimbus.TextFieldPainter", TextFieldPainter.BORDER_DISABLED, New java.awt.Insets(5, 3, 3, 3), New java.awt.Dimension(122, 24), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, Double.PositiveInfinity)
			d("TextField[Focused].borderPainter") = New LazyPainter("javax.swing.plaf.nimbus.TextFieldPainter", TextFieldPainter.BORDER_FOCUSED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(122, 24), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, Double.PositiveInfinity)
			d("TextField[Enabled].borderPainter") = New LazyPainter("javax.swing.plaf.nimbus.TextFieldPainter", TextFieldPainter.BORDER_ENABLED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(122, 24), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, Double.PositiveInfinity)

			'Initialize FormattedTextField
			d("FormattedTextField.contentMargins") = New javax.swing.plaf.InsetsUIResource(6, 6, 6, 6)
			addColor(d, "FormattedTextField[Disabled].textForeground", "nimbusDisabledText", 0.0f, 0.0f, 0.0f, 0)
			d("FormattedTextField[Disabled].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.FormattedTextFieldPainter", FormattedTextFieldPainter.BACKGROUND_DISABLED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(122, 24), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, Double.PositiveInfinity)
			d("FormattedTextField[Enabled].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.FormattedTextFieldPainter", FormattedTextFieldPainter.BACKGROUND_ENABLED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(122, 24), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, Double.PositiveInfinity)
			addColor(d, "FormattedTextField[Selected].textForeground", "nimbusSelectedText", 0.0f, 0.0f, 0.0f, 0)
			d("FormattedTextField[Selected].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.FormattedTextFieldPainter", FormattedTextFieldPainter.BACKGROUND_SELECTED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(122, 24), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, Double.PositiveInfinity)
			addColor(d, "FormattedTextField[Disabled].textForeground", "nimbusDisabledText", 0.0f, 0.0f, 0.0f, 0)
			d("FormattedTextField[Disabled].borderPainter") = New LazyPainter("javax.swing.plaf.nimbus.FormattedTextFieldPainter", FormattedTextFieldPainter.BORDER_DISABLED, New java.awt.Insets(5, 3, 3, 3), New java.awt.Dimension(122, 24), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, Double.PositiveInfinity)
			d("FormattedTextField[Focused].borderPainter") = New LazyPainter("javax.swing.plaf.nimbus.FormattedTextFieldPainter", FormattedTextFieldPainter.BORDER_FOCUSED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(122, 24), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, Double.PositiveInfinity)
			d("FormattedTextField[Enabled].borderPainter") = New LazyPainter("javax.swing.plaf.nimbus.FormattedTextFieldPainter", FormattedTextFieldPainter.BORDER_ENABLED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(122, 24), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, Double.PositiveInfinity)

			'Initialize PasswordField
			d("PasswordField.contentMargins") = New javax.swing.plaf.InsetsUIResource(6, 6, 6, 6)
			addColor(d, "PasswordField[Disabled].textForeground", "nimbusDisabledText", 0.0f, 0.0f, 0.0f, 0)
			d("PasswordField[Disabled].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.PasswordFieldPainter", PasswordFieldPainter.BACKGROUND_DISABLED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(122, 24), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, Double.PositiveInfinity)
			d("PasswordField[Enabled].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.PasswordFieldPainter", PasswordFieldPainter.BACKGROUND_ENABLED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(122, 24), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, Double.PositiveInfinity)
			addColor(d, "PasswordField[Selected].textForeground", "nimbusSelectedText", 0.0f, 0.0f, 0.0f, 0)
			d("PasswordField[Selected].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.PasswordFieldPainter", PasswordFieldPainter.BACKGROUND_SELECTED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(122, 24), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, Double.PositiveInfinity)
			addColor(d, "PasswordField[Disabled].textForeground", "nimbusDisabledText", 0.0f, 0.0f, 0.0f, 0)
			d("PasswordField[Disabled].borderPainter") = New LazyPainter("javax.swing.plaf.nimbus.PasswordFieldPainter", PasswordFieldPainter.BORDER_DISABLED, New java.awt.Insets(5, 3, 3, 3), New java.awt.Dimension(122, 24), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, Double.PositiveInfinity)
			d("PasswordField[Focused].borderPainter") = New LazyPainter("javax.swing.plaf.nimbus.PasswordFieldPainter", PasswordFieldPainter.BORDER_FOCUSED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(122, 24), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, Double.PositiveInfinity)
			d("PasswordField[Enabled].borderPainter") = New LazyPainter("javax.swing.plaf.nimbus.PasswordFieldPainter", PasswordFieldPainter.BORDER_ENABLED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(122, 24), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, Double.PositiveInfinity)

			'Initialize TextArea
			d("TextArea.contentMargins") = New javax.swing.plaf.InsetsUIResource(6, 6, 6, 6)
			d("TextArea.States") = "Enabled,MouseOver,Pressed,Selected,Disabled,Focused,NotInScrollPane"
			d("TextArea.NotInScrollPane") = New TextAreaNotInScrollPaneState
			addColor(d, "TextArea[Disabled].textForeground", "nimbusDisabledText", 0.0f, 0.0f, 0.0f, 0)
			d("TextArea[Disabled].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.TextAreaPainter", TextAreaPainter.BACKGROUND_DISABLED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(122, 24), False, AbstractRegionPainter.PaintContext.CacheMode.NO_CACHING, 1.0, 1.0)
			d("TextArea[Enabled].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.TextAreaPainter", TextAreaPainter.BACKGROUND_ENABLED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(122, 24), False, AbstractRegionPainter.PaintContext.CacheMode.NO_CACHING, 1.0, 1.0)
			addColor(d, "TextArea[Disabled+NotInScrollPane].textForeground", "nimbusDisabledText", 0.0f, 0.0f, 0.0f, 0)
			d("TextArea[Disabled+NotInScrollPane].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.TextAreaPainter", TextAreaPainter.BACKGROUND_DISABLED_NOTINSCROLLPANE, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(122, 24), False, AbstractRegionPainter.PaintContext.CacheMode.NO_CACHING, 1.0, 1.0)
			d("TextArea[Enabled+NotInScrollPane].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.TextAreaPainter", TextAreaPainter.BACKGROUND_ENABLED_NOTINSCROLLPANE, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(122, 24), False, AbstractRegionPainter.PaintContext.CacheMode.NO_CACHING, 1.0, 1.0)
			addColor(d, "TextArea[Selected].textForeground", "nimbusSelectedText", 0.0f, 0.0f, 0.0f, 0)
			d("TextArea[Selected].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.TextAreaPainter", TextAreaPainter.BACKGROUND_SELECTED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(122, 24), False, AbstractRegionPainter.PaintContext.CacheMode.NO_CACHING, 1.0, 1.0)
			addColor(d, "TextArea[Disabled+NotInScrollPane].textForeground", "nimbusDisabledText", 0.0f, 0.0f, 0.0f, 0)
			d("TextArea[Disabled+NotInScrollPane].borderPainter") = New LazyPainter("javax.swing.plaf.nimbus.TextAreaPainter", TextAreaPainter.BORDER_DISABLED_NOTINSCROLLPANE, New java.awt.Insets(5, 3, 3, 3), New java.awt.Dimension(122, 24), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, Double.PositiveInfinity)
			d("TextArea[Focused+NotInScrollPane].borderPainter") = New LazyPainter("javax.swing.plaf.nimbus.TextAreaPainter", TextAreaPainter.BORDER_FOCUSED_NOTINSCROLLPANE, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(122, 24), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, Double.PositiveInfinity)
			d("TextArea[Enabled+NotInScrollPane].borderPainter") = New LazyPainter("javax.swing.plaf.nimbus.TextAreaPainter", TextAreaPainter.BORDER_ENABLED_NOTINSCROLLPANE, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(122, 24), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, Double.PositiveInfinity, Double.PositiveInfinity)

			'Initialize TextPane
			d("TextPane.contentMargins") = New javax.swing.plaf.InsetsUIResource(4, 6, 4, 6)
			d("TextPane.opaque") = Boolean.TRUE
			addColor(d, "TextPane[Disabled].textForeground", "nimbusDisabledText", 0.0f, 0.0f, 0.0f, 0)
			d("TextPane[Disabled].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.TextPanePainter", TextPanePainter.BACKGROUND_DISABLED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(100, 30), False, AbstractRegionPainter.PaintContext.CacheMode.NO_CACHING, 1.0, 1.0)
			d("TextPane[Enabled].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.TextPanePainter", TextPanePainter.BACKGROUND_ENABLED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(100, 30), False, AbstractRegionPainter.PaintContext.CacheMode.NO_CACHING, 1.0, 1.0)
			addColor(d, "TextPane[Selected].textForeground", "nimbusSelectedText", 0.0f, 0.0f, 0.0f, 0)
			d("TextPane[Selected].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.TextPanePainter", TextPanePainter.BACKGROUND_SELECTED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(100, 30), False, AbstractRegionPainter.PaintContext.CacheMode.NO_CACHING, 1.0, 1.0)

			'Initialize EditorPane
			d("EditorPane.contentMargins") = New javax.swing.plaf.InsetsUIResource(4, 6, 4, 6)
			d("EditorPane.opaque") = Boolean.TRUE
			addColor(d, "EditorPane[Disabled].textForeground", "nimbusDisabledText", 0.0f, 0.0f, 0.0f, 0)
			d("EditorPane[Disabled].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.EditorPanePainter", EditorPanePainter.BACKGROUND_DISABLED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(100, 30), False, AbstractRegionPainter.PaintContext.CacheMode.NO_CACHING, 1.0, 1.0)
			d("EditorPane[Enabled].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.EditorPanePainter", EditorPanePainter.BACKGROUND_ENABLED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(100, 30), False, AbstractRegionPainter.PaintContext.CacheMode.NO_CACHING, 1.0, 1.0)
			addColor(d, "EditorPane[Selected].textForeground", "nimbusSelectedText", 0.0f, 0.0f, 0.0f, 0)
			d("EditorPane[Selected].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.EditorPanePainter", EditorPanePainter.BACKGROUND_SELECTED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(100, 30), False, AbstractRegionPainter.PaintContext.CacheMode.NO_CACHING, 1.0, 1.0)

			'Initialize ToolBar
			d("ToolBar.contentMargins") = New javax.swing.plaf.InsetsUIResource(2, 2, 2, 2)
			d("ToolBar.opaque") = Boolean.TRUE
			d("ToolBar.States") = "North,East,West,South"
			d("ToolBar.North") = New ToolBarNorthState
			d("ToolBar.East") = New ToolBarEastState
			d("ToolBar.West") = New ToolBarWestState
			d("ToolBar.South") = New ToolBarSouthState
			d("ToolBar[North].borderPainter") = New LazyPainter("javax.swing.plaf.nimbus.ToolBarPainter", ToolBarPainter.BORDER_NORTH, New java.awt.Insets(0, 0, 1, 0), New java.awt.Dimension(30, 30), False, AbstractRegionPainter.PaintContext.CacheMode.NO_CACHING, 1.0, 1.0)
			d("ToolBar[South].borderPainter") = New LazyPainter("javax.swing.plaf.nimbus.ToolBarPainter", ToolBarPainter.BORDER_SOUTH, New java.awt.Insets(1, 0, 0, 0), New java.awt.Dimension(30, 30), False, AbstractRegionPainter.PaintContext.CacheMode.NO_CACHING, 1.0, 1.0)
			d("ToolBar[East].borderPainter") = New LazyPainter("javax.swing.plaf.nimbus.ToolBarPainter", ToolBarPainter.BORDER_EAST, New java.awt.Insets(1, 0, 0, 0), New java.awt.Dimension(30, 30), False, AbstractRegionPainter.PaintContext.CacheMode.NO_CACHING, 1.0, 1.0)
			d("ToolBar[West].borderPainter") = New LazyPainter("javax.swing.plaf.nimbus.ToolBarPainter", ToolBarPainter.BORDER_WEST, New java.awt.Insets(0, 0, 1, 0), New java.awt.Dimension(30, 30), False, AbstractRegionPainter.PaintContext.CacheMode.NO_CACHING, 1.0, 1.0)
			d("ToolBar[Enabled].handleIconPainter") = New LazyPainter("javax.swing.plaf.nimbus.ToolBarPainter", ToolBarPainter.HANDLEICON_ENABLED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(11, 38), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, 2.0, Double.PositiveInfinity)
			d("ToolBar.handleIcon") = New NimbusIcon("ToolBar", "handleIconPainter", 11, 38)
			d("ToolBar:Button.contentMargins") = New javax.swing.plaf.InsetsUIResource(4, 4, 4, 4)
			d("ToolBar:Button[Focused].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ToolBarButtonPainter", ToolBarButtonPainter.BACKGROUND_FOCUSED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(104, 33), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, 2.0, Double.PositiveInfinity)
			d("ToolBar:Button[MouseOver].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ToolBarButtonPainter", ToolBarButtonPainter.BACKGROUND_MOUSEOVER, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(104, 33), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, 2.0, Double.PositiveInfinity)
			d("ToolBar:Button[Focused+MouseOver].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ToolBarButtonPainter", ToolBarButtonPainter.BACKGROUND_MOUSEOVER_FOCUSED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(104, 33), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, 2.0, Double.PositiveInfinity)
			d("ToolBar:Button[Pressed].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ToolBarButtonPainter", ToolBarButtonPainter.BACKGROUND_PRESSED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(104, 33), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, 2.0, Double.PositiveInfinity)
			d("ToolBar:Button[Focused+Pressed].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ToolBarButtonPainter", ToolBarButtonPainter.BACKGROUND_PRESSED_FOCUSED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(104, 33), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, 2.0, Double.PositiveInfinity)
			d("ToolBar:ToggleButton.contentMargins") = New javax.swing.plaf.InsetsUIResource(4, 4, 4, 4)
			d("ToolBar:ToggleButton[Focused].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ToolBarToggleButtonPainter", ToolBarToggleButtonPainter.BACKGROUND_FOCUSED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(104, 34), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, 2.0, Double.PositiveInfinity)
			d("ToolBar:ToggleButton[MouseOver].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ToolBarToggleButtonPainter", ToolBarToggleButtonPainter.BACKGROUND_MOUSEOVER, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(104, 34), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, 2.0, Double.PositiveInfinity)
			d("ToolBar:ToggleButton[Focused+MouseOver].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ToolBarToggleButtonPainter", ToolBarToggleButtonPainter.BACKGROUND_MOUSEOVER_FOCUSED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(104, 34), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, 2.0, Double.PositiveInfinity)
			d("ToolBar:ToggleButton[Pressed].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ToolBarToggleButtonPainter", ToolBarToggleButtonPainter.BACKGROUND_PRESSED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(72, 25), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, 2.0, Double.PositiveInfinity)
			d("ToolBar:ToggleButton[Focused+Pressed].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ToolBarToggleButtonPainter", ToolBarToggleButtonPainter.BACKGROUND_PRESSED_FOCUSED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(72, 25), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, 2.0, Double.PositiveInfinity)
			d("ToolBar:ToggleButton[Selected].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ToolBarToggleButtonPainter", ToolBarToggleButtonPainter.BACKGROUND_SELECTED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(72, 25), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, 2.0, Double.PositiveInfinity)
			d("ToolBar:ToggleButton[Focused+Selected].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ToolBarToggleButtonPainter", ToolBarToggleButtonPainter.BACKGROUND_SELECTED_FOCUSED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(72, 25), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, 2.0, Double.PositiveInfinity)
			d("ToolBar:ToggleButton[Pressed+Selected].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ToolBarToggleButtonPainter", ToolBarToggleButtonPainter.BACKGROUND_PRESSED_SELECTED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(72, 25), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, 2.0, Double.PositiveInfinity)
			d("ToolBar:ToggleButton[Focused+Pressed+Selected].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ToolBarToggleButtonPainter", ToolBarToggleButtonPainter.BACKGROUND_PRESSED_SELECTED_FOCUSED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(72, 25), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, 2.0, Double.PositiveInfinity)
			d("ToolBar:ToggleButton[MouseOver+Selected].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ToolBarToggleButtonPainter", ToolBarToggleButtonPainter.BACKGROUND_MOUSEOVER_SELECTED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(72, 25), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, 2.0, Double.PositiveInfinity)
			d("ToolBar:ToggleButton[Focused+MouseOver+Selected].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ToolBarToggleButtonPainter", ToolBarToggleButtonPainter.BACKGROUND_MOUSEOVER_SELECTED_FOCUSED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(72, 25), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, 2.0, Double.PositiveInfinity)
			addColor(d, "ToolBar:ToggleButton[Disabled+Selected].textForeground", "nimbusDisabledText", 0.0f, 0.0f, 0.0f, 0)
			d("ToolBar:ToggleButton[Disabled+Selected].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ToolBarToggleButtonPainter", ToolBarToggleButtonPainter.BACKGROUND_DISABLED_SELECTED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(72, 25), False, AbstractRegionPainter.PaintContext.CacheMode.NINE_SQUARE_SCALE, 2.0, Double.PositiveInfinity)

			'Initialize ToolBarSeparator
			d("ToolBarSeparator.contentMargins") = New javax.swing.plaf.InsetsUIResource(2, 0, 3, 0)
			addColor(d, "ToolBarSeparator.textForeground", "nimbusBorder", 0.0f, 0.0f, 0.0f, 0)

			'Initialize ToolTip
			d("ToolTip.contentMargins") = New javax.swing.plaf.InsetsUIResource(4, 4, 4, 4)
			d("ToolTip[Enabled].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.ToolTipPainter", ToolTipPainter.BACKGROUND_ENABLED, New java.awt.Insets(1, 1, 1, 1), New java.awt.Dimension(10, 10), False, AbstractRegionPainter.PaintContext.CacheMode.NO_CACHING, 1.0, 1.0)

			'Initialize Tree
			d("Tree.contentMargins") = New javax.swing.plaf.InsetsUIResource(0, 0, 0, 0)
			d("Tree.opaque") = Boolean.TRUE
			addColor(d, "Tree.textForeground", "text", 0.0f, 0.0f, 0.0f, 0, False)
			addColor(d, "Tree.textBackground", "nimbusLightBackground", 0.0f, 0.0f, 0.0f, 0, False)
			addColor(d, "Tree.background", "nimbusLightBackground", 0.0f, 0.0f, 0.0f, 0)
			d("Tree.rendererFillBackground") = Boolean.FALSE
			d("Tree.leftChildIndent") = New Integer?(12)
			d("Tree.rightChildIndent") = New Integer?(4)
			d("Tree.drawHorizontalLines") = Boolean.FALSE
			d("Tree.drawVerticalLines") = Boolean.FALSE
			d("Tree.showRootHandles") = Boolean.FALSE
			d("Tree.rendererUseTreeColors") = Boolean.TRUE
			d("Tree.repaintWholeRow") = Boolean.TRUE
			d("Tree.rowHeight") = New Integer?(0)
			d("Tree.rendererMargins") = New javax.swing.plaf.InsetsUIResource(2, 0, 1, 5)
			addColor(d, "Tree.selectionForeground", "nimbusSelectedText", 0.0f, 0.0f, 0.0f, 0, False)
			addColor(d, "Tree.selectionBackground", "nimbusSelectionBackground", 0.0f, 0.0f, 0.0f, 0, False)
			addColor(d, "Tree.dropLineColor", "nimbusFocus", 0.0f, 0.0f, 0.0f, 0)
			d("Tree:TreeCell.contentMargins") = New javax.swing.plaf.InsetsUIResource(0, 0, 0, 0)
			addColor(d, "Tree:TreeCell[Enabled].background", "nimbusLightBackground", 0.0f, 0.0f, 0.0f, 0)
			addColor(d, "Tree:TreeCell[Enabled+Focused].background", "nimbusLightBackground", 0.0f, 0.0f, 0.0f, 0)
			d("Tree:TreeCell[Enabled+Focused].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.TreeCellPainter", TreeCellPainter.BACKGROUND_ENABLED_FOCUSED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(100, 30), False, AbstractRegionPainter.PaintContext.CacheMode.NO_CACHING, 1.0, 1.0)
			addColor(d, "Tree:TreeCell[Enabled+Selected].textForeground", 255, 255, 255, 255)
			d("Tree:TreeCell[Enabled+Selected].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.TreeCellPainter", TreeCellPainter.BACKGROUND_ENABLED_SELECTED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(100, 30), False, AbstractRegionPainter.PaintContext.CacheMode.NO_CACHING, 1.0, 1.0)
			addColor(d, "Tree:TreeCell[Focused+Selected].textForeground", 255, 255, 255, 255)
			d("Tree:TreeCell[Focused+Selected].backgroundPainter") = New LazyPainter("javax.swing.plaf.nimbus.TreeCellPainter", TreeCellPainter.BACKGROUND_SELECTED_FOCUSED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(100, 30), False, AbstractRegionPainter.PaintContext.CacheMode.NO_CACHING, 1.0, 1.0)
			d("Tree:""Tree.cellRenderer"".contentMargins") = New javax.swing.plaf.InsetsUIResource(0, 0, 0, 0)
			addColor(d, "Tree:""Tree.cellRenderer""[Disabled].textForeground", "nimbusDisabledText", 0.0f, 0.0f, 0.0f, 0)
			d("Tree[Enabled].leafIconPainter") = New LazyPainter("javax.swing.plaf.nimbus.TreePainter", TreePainter.LEAFICON_ENABLED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(16, 16), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("Tree.leafIcon") = New NimbusIcon("Tree", "leafIconPainter", 16, 16)
			d("Tree[Enabled].closedIconPainter") = New LazyPainter("javax.swing.plaf.nimbus.TreePainter", TreePainter.CLOSEDICON_ENABLED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(16, 16), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("Tree.closedIcon") = New NimbusIcon("Tree", "closedIconPainter", 16, 16)
			d("Tree[Enabled].openIconPainter") = New LazyPainter("javax.swing.plaf.nimbus.TreePainter", TreePainter.OPENICON_ENABLED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(16, 16), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("Tree.openIcon") = New NimbusIcon("Tree", "openIconPainter", 16, 16)
			d("Tree[Enabled].collapsedIconPainter") = New LazyPainter("javax.swing.plaf.nimbus.TreePainter", TreePainter.COLLAPSEDICON_ENABLED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(18, 7), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("Tree[Enabled+Selected].collapsedIconPainter") = New LazyPainter("javax.swing.plaf.nimbus.TreePainter", TreePainter.COLLAPSEDICON_ENABLED_SELECTED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(18, 7), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("Tree.collapsedIcon") = New NimbusIcon("Tree", "collapsedIconPainter", 18, 7)
			d("Tree[Enabled].expandedIconPainter") = New LazyPainter("javax.swing.plaf.nimbus.TreePainter", TreePainter.EXPANDEDICON_ENABLED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(18, 7), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("Tree[Enabled+Selected].expandedIconPainter") = New LazyPainter("javax.swing.plaf.nimbus.TreePainter", TreePainter.EXPANDEDICON_ENABLED_SELECTED, New java.awt.Insets(5, 5, 5, 5), New java.awt.Dimension(18, 7), False, AbstractRegionPainter.PaintContext.CacheMode.FIXED_SIZES, 1.0, 1.0)
			d("Tree.expandedIcon") = New NimbusIcon("Tree", "expandedIconPainter", 18, 7)

			'Initialize RootPane
			d("RootPane.contentMargins") = New javax.swing.plaf.InsetsUIResource(0, 0, 0, 0)
			d("RootPane.opaque") = Boolean.TRUE
			addColor(d, "RootPane.background", "control", 0.0f, 0.0f, 0.0f, 0)


		End Sub

		''' <summary>
		''' <p>Registers the given region and prefix. The prefix, if it contains
		''' quoted sections, refers to certain named components. If there are not
		''' quoted sections, then the prefix refers to a generic component type.</p>
		''' 
		''' <p>If the given region/prefix combo has already been registered, then
		''' it will not be registered twice. The second registration attempt will
		''' fail silently.</p>
		''' </summary>
		''' <param name="region"> The Synth Region that is being registered. Such as Button,
		'''        or ScrollBarThumb. </param>
		''' <param name="prefix"> The UIDefault prefix. For example, could be ComboBox, or if
		'''        a named components, "MyComboBox", or even something like
		'''        ToolBar:"MyComboBox":"ComboBox.arrowButton" </param>
		Friend Sub register(ByVal region As javax.swing.plaf.synth.Region, ByVal prefix As String)
			'validate the method arguments
			If region Is Nothing OrElse prefix Is Nothing Then Throw New System.ArgumentException("Neither Region nor Prefix may be null")

			'Add a LazyStyle for this region/prefix to m.
			Dim styles As IList(Of LazyStyle) = m(region)
			If styles Is Nothing Then
				styles = New LinkedList(Of LazyStyle)
				styles.Add(New LazyStyle(Me, prefix))
				m(region) = styles
			Else
				'iterate over all the current styles and see if this prefix has
				'already been registered. If not, then register it.
				For Each s As LazyStyle In styles
					If prefix.Equals(s.prefix) Then Return
				Next s
				styles.Add(New LazyStyle(Me, prefix))
			End If

			'add this region to the map of registered regions
			registeredRegions(region.name) = region
		End Sub

		''' <summary>
		''' <p>Locate the style associated with the given region, and component.
		''' This is called from NimbusLookAndFeel in the SynthStyleFactory
		''' implementation.</p>
		''' 
		''' <p>Lookup occurs as follows:<br/>
		''' Check the map of styles <code>m</code>. If the map contains no styles at
		''' all, then simply return the defaultStyle. If the map contains styles,
		''' then iterate over all of the styles for the Region <code>r</code> looking
		''' for the best match, based on prefix. If a match was made, then return
		''' that SynthStyle. Otherwise, return the defaultStyle.</p>
		''' </summary>
		''' <param name="comp"> The component associated with this region. For example, if
		'''        the Region is Region.Button then the component will be a JButton.
		'''        If the Region is a subregion, such as ScrollBarThumb, then the
		'''        associated component will be the component that subregion belongs
		'''        to, such as JScrollBar. The JComponent may be named. It may not be
		'''        null. </param>
		''' <param name="r"> The region we are looking for a style for. May not be null. </param>
		Friend Function getStyle(ByVal comp As javax.swing.JComponent, ByVal r As javax.swing.plaf.synth.Region) As javax.swing.plaf.synth.SynthStyle
			'validate method arguments
			If comp Is Nothing OrElse r Is Nothing Then Throw New System.ArgumentException("Neither comp nor r may be null")

			'if there are no lazy styles registered for the region r, then return
			'the default style
			Dim styles As IList(Of LazyStyle) = m(r)
			If styles Is Nothing OrElse styles.Count = 0 Then Return defaultStyle

			'Look for the best SynthStyle for this component/region pair.
			Dim foundStyle As LazyStyle = Nothing
			For Each s As LazyStyle In styles
				If s.matches(comp) Then
					'replace the foundStyle if foundStyle is null, or
					'if the new style "s" is more specific (ie, its path was
					'longer), or if the foundStyle was "simple" and the new style
					'was not (ie: the foundStyle was for something like Button and
					'the new style was for something like "MyButton", hence, being
					'more specific.) In all cases, favor the most specific style
					'found.
					If foundStyle Is Nothing OrElse (foundStyle.parts.length < s.parts.length) OrElse (foundStyle.parts.length = s.parts.length AndAlso foundStyle.simple AndAlso (Not s.simple)) Then foundStyle = s
				End If
			Next s

			'return the style, if found, or the default style if not found
			Return If(foundStyle Is Nothing, defaultStyle, foundStyle.getStyle(comp, r))
		End Function

		Public Sub clearOverridesCache(ByVal c As javax.swing.JComponent)
			overridesCache.Remove(c)
		End Sub

	'    
	'        Various public helper classes.
	'        These may be used to register 3rd party values into UIDefaults
	'    

		''' <summary>
		''' <p>Derives its font value based on a parent font and a set of offsets and
		''' attributes. This class is an ActiveValue, meaning that it will recompute
		''' its value each time it is requested from UIDefaults. It is therefore
		''' recommended to read this value once and cache it in the UI delegate class
		''' until asked to reinitialize.</p>
		''' 
		''' <p>To use this class, create an instance with the key of the font in the
		''' UI defaults table from which to derive this font, along with a size
		''' offset (if any), and whether it is to be bold, italic, or left in its
		''' default form.</p>
		''' </summary>
		Friend NotInheritable Class DerivedFont
			Implements javax.swing.UIDefaults.ActiveValue

			Private sizeOffset As Single
			Private bold As Boolean?
			Private italic As Boolean?
			Private parentKey As String

			''' <summary>
			''' Create a new DerivedFont.
			''' </summary>
			''' <param name="key"> The UIDefault key associated with this derived font's
			'''            parent or source. If this key leads to a null value, or a
			'''            value that is not a font, then null will be returned as
			'''            the derived font. The key must not be null. </param>
			''' <param name="sizeOffset"> The size offset, as a percentage, to use. For
			'''                   example, if the source font was a 12pt font and the
			'''                   sizeOffset were specified as .9, then the new font
			'''                   will be 90% of what the source font was, or, 10.8
			'''                   pts which is rounded to 11pts. This fractional
			'''                   based offset allows for proper font scaling in high
			'''                   DPI or large system font scenarios. </param>
			''' <param name="bold"> Whether the new font should be bold. If null, then this
			'''             new font will inherit the bold setting of the source
			'''             font. </param>
			''' <param name="italic"> Whether the new font should be italicized. If null,
			'''               then this new font will inherit the italic setting of
			'''               the source font. </param>
			Public Sub New(ByVal key As String, ByVal sizeOffset As Single, ByVal bold As Boolean?, ByVal italic As Boolean?)
				'validate the constructor arguments
				If key Is Nothing Then Throw New System.ArgumentException("You must specify a key")

				'set the values
				Me.parentKey = key
				Me.sizeOffset = sizeOffset
				Me.bold = bold
				Me.italic = italic
			End Sub

			''' <summary>
			''' @inheritDoc
			''' </summary>
			Public Overrides Function createValue(ByVal defaults As javax.swing.UIDefaults) As Object
				Dim f As java.awt.Font = defaults.getFont(parentKey)
				If f IsNot Nothing Then
					' always round size for now so we have exact int font size
					' (or we may have lame looking fonts)
					Dim size As Single = Math.Round(f.size2D * sizeOffset)
					Dim style As Integer = f.style
					If bold IsNot Nothing Then
						If bold Then
							style = style Or java.awt.Font.BOLD
						Else
							style = style And Not java.awt.Font.BOLD
						End If
					End If
					If italic IsNot Nothing Then
						If italic Then
							style = style Or java.awt.Font.ITALIC
						Else
							style = style And Not java.awt.Font.ITALIC
						End If
					End If
					Return f.deriveFont(style, size)
				Else
					Return Nothing
				End If
			End Function
		End Class


		''' <summary>
		''' This class is private because it relies on the constructor of the
		''' auto-generated AbstractRegionPainter subclasses. Hence, it is not
		''' generally useful, and is private.
		''' <p/>
		''' LazyPainter is a LazyValue class. It will create the
		''' AbstractRegionPainter lazily, when asked. It uses reflection to load the
		''' proper class and invoke its constructor.
		''' </summary>
		Private NotInheritable Class LazyPainter
			Implements javax.swing.UIDefaults.LazyValue

			Private which As Integer
			Private ctx As AbstractRegionPainter.PaintContext
			Private className As String

			Friend Sub New(ByVal className As String, ByVal which As Integer, ByVal insets As java.awt.Insets, ByVal canvasSize As java.awt.Dimension, ByVal inverted As Boolean)
				If className Is Nothing Then Throw New System.ArgumentException("The className must be specified")

				Me.className = className
				Me.which = which
				Me.ctx = New AbstractRegionPainter.PaintContext(insets, canvasSize, inverted)
			End Sub

			Friend Sub New(ByVal className As String, ByVal which As Integer, ByVal insets As java.awt.Insets, ByVal canvasSize As java.awt.Dimension, ByVal inverted As Boolean, ByVal cacheMode As AbstractRegionPainter.PaintContext.CacheMode, ByVal maxH As Double, ByVal maxV As Double)
				If className Is Nothing Then Throw New System.ArgumentException("The className must be specified")

				Me.className = className
				Me.which = which
				Me.ctx = New AbstractRegionPainter.PaintContext(insets, canvasSize, inverted, cacheMode, maxH, maxV)
			End Sub

			Public Overrides Function createValue(ByVal table As javax.swing.UIDefaults) As Object
				Try
					Dim c As Type
					Dim cl As Object
					' See if we should use a separate ClassLoader
					cl = table("ClassLoader")
					If table Is Nothing OrElse Not(TypeOf cl Is ClassLoader) Then
						cl = Thread.CurrentThread.contextClassLoader
						If cl Is Nothing Then cl = ClassLoader.systemClassLoader
					End If

					c = Type.GetType(className, True, CType(cl, ClassLoader))
					Dim constructor As Constructor = c.GetConstructor(GetType(AbstractRegionPainter.PaintContext), GetType(Integer))
					If constructor Is Nothing Then Throw New NullPointerException("Failed to find the constructor for the class: " & className)
					Return constructor.newInstance(ctx, which)
				Catch e As Exception
					Console.WriteLine(e.ToString())
					Console.Write(e.StackTrace)
					Return Nothing
				End Try
			End Function
		End Class

		''' <summary>
		''' A class which creates the NimbusStyle associated with it lazily, but also
		''' manages a lot more information about the style. It is less of a LazyValue
		''' type of class, and more of an Entry or Item type of class, as it
		''' represents an entry in the list of LazyStyles in the map m.
		''' 
		''' The primary responsibilities of this class include:
		''' <ul>
		'''   <li>Determining whether a given component/region pair matches this
		'''       style</li>
		'''   <li>Splitting the prefix specified in the constructor into its
		'''       constituent parts to facilitate quicker matching</li>
		'''   <li>Creating and vending a NimbusStyle lazily.</li>
		''' </ul>
		''' </summary>
		Private NotInheritable Class LazyStyle
			Private ReadOnly outerInstance As NimbusDefaults

			''' <summary>
			''' The prefix this LazyStyle was registered with. Something like
			''' Button or ComboBox:"ComboBox.arrowButton"
			''' </summary>
			Private prefix As String
			''' <summary>
			''' Whether or not this LazyStyle represents an unnamed component
			''' </summary>
			Private simple As Boolean = True
			''' <summary>
			''' The various parts, or sections, of the prefix. For example,
			''' the prefix:
			'''     ComboBox:"ComboBox.arrowButton"
			''' 
			''' will be broken into two parts,
			'''     ComboBox and "ComboBox.arrowButton"
			''' </summary>
			Private parts As Part()
			''' <summary>
			''' Cached shared style.
			''' </summary>
			Private style As NimbusStyle

			''' <summary>
			''' Create a new LazyStyle.
			''' </summary>
			''' <param name="prefix"> The prefix associated with this style. Cannot be null. </param>
			Private Sub New(ByVal outerInstance As NimbusDefaults, ByVal prefix As String)
					Me.outerInstance = outerInstance
				If prefix Is Nothing Then Throw New System.ArgumentException("The prefix must not be null")

				Me.prefix = prefix

				'there is one odd case that needs to be supported here: cell
				'renderers. A cell renderer is defined as a named internal
				'component, so for example:
				' List."List.cellRenderer"
				'The problem is that the component named List.cellRenderer is not a
				'child of a JList. Rather, it is treated more as a direct component
				'Thus, if the prefix ends with "cellRenderer", then remove all the
				'previous dotted parts of the prefix name so that it becomes, for
				'example: "List.cellRenderer"
				'Likewise, we have a hacked work around for cellRenderer, renderer,
				'and listRenderer.
				Dim temp As String = prefix
				If temp.EndsWith("cellRenderer""") OrElse temp.EndsWith("renderer""") OrElse temp.EndsWith("listRenderer""") Then temp = temp.Substring(temp.LastIndexOf(":""") + 1)

				'otherwise, normal code path
				Dim sparts As IList(Of String) = Split(temp)
				parts = New Part(sparts.Count - 1){}
				For i As Integer = 0 To parts.Length - 1
					parts(i) = New Part(Me, sparts(i))
					If parts(i).named Then simple = False
				Next i
			End Sub

			''' <summary>
			''' Gets the style. Creates it if necessary. </summary>
			''' <returns> the style </returns>
			Friend Function getStyle(ByVal c As javax.swing.JComponent, ByVal r As javax.swing.plaf.synth.Region) As javax.swing.plaf.synth.SynthStyle
				' if the component has overrides, it gets its own unique style
				' instead of the shared style.
				If c.getClientProperty("Nimbus.Overrides") IsNot Nothing Then
					Dim map As IDictionary(Of javax.swing.plaf.synth.Region, javax.swing.plaf.synth.SynthStyle) = outerInstance.overridesCache(c)
					Dim s As javax.swing.plaf.synth.SynthStyle = Nothing
					If map Is Nothing Then
						map = New Dictionary(Of javax.swing.plaf.synth.Region, javax.swing.plaf.synth.SynthStyle)
						outerInstance.overridesCache(c) = map
					Else
						s = map(r)
					End If
					If s Is Nothing Then
						s = New NimbusStyle(prefix, c)
						map(r) = s
					End If
					Return s
				End If

				' lazily create the style if necessary
				If style Is Nothing Then style = New NimbusStyle(prefix, Nothing)

				' return the style
				Return style
			End Function

			''' <summary>
			''' This LazyStyle is a match for the given component if, and only if,
			''' for each part of the prefix the component hierarchy matches exactly.
			''' That is, if given "a":something:"b", then:
			''' c.getName() must equals "b"
			''' c.getParent() can be anything
			''' c.getParent().getParent().getName() must equal "a".
			''' </summary>
			Friend Function matches(ByVal c As javax.swing.JComponent) As Boolean
				Return matches(c, parts.Length - 1)
			End Function

			Private Function matches(ByVal c As java.awt.Component, ByVal partIndex As Integer) As Boolean
				If partIndex < 0 Then Return True
				If c Is Nothing Then Return False
				'only get here if partIndex > 0 and c == null

				Dim name As String = c.name
				If parts(partIndex).named AndAlso parts(partIndex).s.Equals(name) Then
					'so far so good, recurse
					Return matches(c.parent, partIndex - 1)
				ElseIf Not parts(partIndex).named Then
					'if c is not named, and parts[partIndex] has an expected class
					'type registered, then check to make sure c is of the
					'right type;
					Dim clazz As Type = parts(partIndex).c
					If clazz IsNot Nothing AndAlso clazz.IsAssignableFrom(c.GetType()) Then
						'so far so good, recurse
						Return matches(c.parent, partIndex - 1)
					ElseIf clazz Is Nothing AndAlso outerInstance.registeredRegions.ContainsKey(parts(partIndex).s) Then
						Dim r As javax.swing.plaf.synth.Region = outerInstance.registeredRegions(parts(partIndex).s)
						Dim parent As java.awt.Component = If(r.subregion, c, c.parent)
						'special case the JInternalFrameTitlePane, because it
						'doesn't fit the mold. very, very funky.
						If r Is javax.swing.plaf.synth.Region.INTERNAL_FRAME_TITLE_PANE AndAlso parent IsNot Nothing AndAlso TypeOf parent Is javax.swing.JInternalFrame.JDesktopIcon Then
							Dim icon As javax.swing.JInternalFrame.JDesktopIcon = CType(parent, javax.swing.JInternalFrame.JDesktopIcon)
							parent = icon.internalFrame
						End If
						'it was the name of a region. So far, so good. Recurse.
						Return matches(parent, partIndex - 1)
					End If
				End If

				Return False
			End Function

			''' <summary>
			''' Given some dot separated prefix, split on the colons that are
			''' not within quotes, and not within brackets.
			''' </summary>
			''' <param name="prefix">
			''' @return </param>
			Private Function split(ByVal prefix As String) As IList(Of String)
				Dim parts As IList(Of String) = New List(Of String)
				Dim bracketCount As Integer = 0
				Dim inquotes As Boolean = False
				Dim lastIndex As Integer = 0
				For i As Integer = 0 To prefix.Length - 1
					Dim c As Char = prefix.Chars(i)

					If c = "["c Then
						bracketCount += 1
						Continue For
					ElseIf c = """"c Then
						inquotes = Not inquotes
						Continue For
					ElseIf c = "]"c Then
						bracketCount -= 1
						If bracketCount < 0 Then Throw New Exception("Malformed prefix: " & prefix)
						Continue For
					End If

					If c = ":"c AndAlso (Not inquotes) AndAlso bracketCount = 0 Then
						'found a character to split on.
						parts.Add(prefix.Substring(lastIndex, i - lastIndex))
						lastIndex = i + 1
					End If
				Next i
				If lastIndex < prefix.Length - 1 AndAlso (Not inquotes) AndAlso bracketCount = 0 Then parts.Add(prefix.Substring(lastIndex))
				Return parts

			End Function

			Private NotInheritable Class Part
				Private ReadOnly outerInstance As NimbusDefaults.LazyStyle

				Private s As String
				'true if this part represents a component name
				Private named As Boolean
				Private c As Type

				Friend Sub New(ByVal outerInstance As NimbusDefaults.LazyStyle, ByVal s As String)
						Me.outerInstance = outerInstance
					named = s.Chars(0) = """"c AndAlso s.Chars(s.Length - 1) = """"c
					If named Then
						Me.s = s.Substring(1, s.Length - 1 - 1)
					Else
						Me.s = s
						'TODO use a map of known regions for Synth and Swing, and
						'then use [classname] instead of org_class_name style
						Try
							c = Type.GetType("javax.swing.J" & s)
						Catch e As Exception
						End Try
						Try
							c = Type.GetType(s.Replace("_", "."))
						Catch e As Exception
						End Try
					End If
				End Sub
			End Class
		End Class

		Private Sub addColor(ByVal d As javax.swing.UIDefaults, ByVal uin As String, ByVal r As Integer, ByVal g As Integer, ByVal b As Integer, ByVal a As Integer)
			Dim color As java.awt.Color = New javax.swing.plaf.ColorUIResource(New java.awt.Color(r, g, b, a))
			colorTree.addColor(uin, color)
			d(uin) = color
		End Sub

		Private Sub addColor(ByVal d As javax.swing.UIDefaults, ByVal uin As String, ByVal parentUin As String, ByVal hOffset As Single, ByVal sOffset As Single, ByVal bOffset As Single, ByVal aOffset As Integer)
			addColor(d, uin, parentUin, hOffset, sOffset, bOffset, aOffset, True)
		End Sub

		Private Sub addColor(ByVal d As javax.swing.UIDefaults, ByVal uin As String, ByVal parentUin As String, ByVal hOffset As Single, ByVal sOffset As Single, ByVal bOffset As Single, ByVal aOffset As Integer, ByVal uiResource As Boolean)
			Dim color As java.awt.Color = getDerivedColor(uin, parentUin, hOffset, sOffset, bOffset, aOffset, uiResource)
			d(uin) = color
		End Sub

		''' <summary>
		''' Get a derived color, derived colors are shared instances and will be
		''' updated when its parent UIDefault color changes.
		''' </summary>
		''' <param name="uiDefaultParentName"> The parent UIDefault key </param>
		''' <param name="hOffset"> The hue offset </param>
		''' <param name="sOffset"> The saturation offset </param>
		''' <param name="bOffset"> The brightness offset </param>
		''' <param name="aOffset"> The alpha offset </param>
		''' <param name="uiResource"> True if the derived color should be a UIResource,
		'''        false if it should not be a UIResource </param>
		''' <returns> The stored derived color </returns>
		Public Function getDerivedColor(ByVal parentUin As String, ByVal hOffset As Single, ByVal sOffset As Single, ByVal bOffset As Single, ByVal aOffset As Integer, ByVal uiResource As Boolean) As DerivedColor
			Return getDerivedColor(Nothing, parentUin, hOffset, sOffset, bOffset, aOffset, uiResource)
		End Function

		Private Function getDerivedColor(ByVal uin As String, ByVal parentUin As String, ByVal hOffset As Single, ByVal sOffset As Single, ByVal bOffset As Single, ByVal aOffset As Integer, ByVal uiResource As Boolean) As DerivedColor
			Dim color As DerivedColor
			If uiResource Then
				color = New DerivedColor.UIResource(parentUin, hOffset, sOffset, bOffset, aOffset)
			Else
				color = New DerivedColor(parentUin, hOffset, sOffset, bOffset, aOffset)
			End If

			If derivedColors.ContainsKey(color) Then
				Return derivedColors(color)
			Else
				derivedColors(color) = color
				color.rederiveColor() '/ move to ARP.decodeColor() ?
				colorTree.addColor(uin, color)
				Return color
			End If
		End Function

		Private derivedColors As IDictionary(Of DerivedColor, DerivedColor) = New Dictionary(Of DerivedColor, DerivedColor)

		Private Class ColorTree
			Implements java.beans.PropertyChangeListener

			Private ReadOnly outerInstance As NimbusDefaults

			Public Sub New(ByVal outerInstance As NimbusDefaults)
				Me.outerInstance = outerInstance
			End Sub

			Private root As New Node(Me, Nothing, Nothing)
			Private nodes As IDictionary(Of String, Node) = New Dictionary(Of String, Node)

			Public Overridable Function getColor(ByVal uin As String) As java.awt.Color
				Return nodes(uin).color
			End Function

			Public Overridable Sub addColor(ByVal uin As String, ByVal color As java.awt.Color)
				Dim parent As Node = getParentNode(color)
				Dim node As New Node(Me, color, parent)
				parent.children.Add(node)
				If uin IsNot Nothing Then nodes(uin) = node
			End Sub

			Private Function getParentNode(ByVal color As java.awt.Color) As Node
				Dim parent As Node = root
				If TypeOf color Is DerivedColor Then
					Dim parentUin As String = CType(color, DerivedColor).uiDefaultParentName
					Dim p As Node = nodes(parentUin)
					If p IsNot Nothing Then parent = p
				End If
				Return parent
			End Function

			Public Overridable Sub update()
				root.update()
			End Sub

			Public Overrides Sub propertyChange(ByVal ev As java.beans.PropertyChangeEvent)
				Dim name As String = ev.propertyName
				Dim node As Node = nodes(name)
				If node IsNot Nothing Then
					' this is a registered color
					node.parent.children.Remove(node)
					Dim ___color As java.awt.Color = CType(ev.newValue, java.awt.Color)
					Dim parent As Node = getParentNode(___color)
					node.set(___color, parent)
					parent.children.Add(node)
					node.update()
				End If
			End Sub

			Friend Class Node
				Private ReadOnly outerInstance As NimbusDefaults.ColorTree

				Friend color As java.awt.Color
				Friend parent As Node
				Friend children As IList(Of Node) = New LinkedList(Of Node)

				Friend Sub New(ByVal outerInstance As NimbusDefaults.ColorTree, ByVal color As java.awt.Color, ByVal parent As Node)
						Me.outerInstance = outerInstance
					[set](color, parent)
				End Sub

				Public Overridable Sub [set](ByVal color As java.awt.Color, ByVal parent As Node)
					Me.color = color
					Me.parent = parent
				End Sub

				Public Overridable Sub update()
					If TypeOf color Is DerivedColor Then CType(color, DerivedColor).rederiveColor()
					For Each child As Node In children
						child.update()
					Next child
				End Sub
			End Class
		End Class

		''' <summary>
		''' Listener to update derived colors on UIManager Defaults changes
		''' </summary>
		Private Class DefaultsListener
			Implements java.beans.PropertyChangeListener

			Private ReadOnly outerInstance As NimbusDefaults

			Public Sub New(ByVal outerInstance As NimbusDefaults)
				Me.outerInstance = outerInstance
			End Sub

			Public Overrides Sub propertyChange(ByVal evt As java.beans.PropertyChangeEvent)
				If "lookAndFeel".Equals(evt.propertyName) Then outerInstance.colorTree.update()
			End Sub
		End Class

		Private NotInheritable Class PainterBorder
			Implements javax.swing.border.Border, javax.swing.plaf.UIResource

			Private insets As java.awt.Insets
			Private painter As javax.swing.Painter
			Private painterKey As String

			Friend Sub New(ByVal painterKey As String, ByVal insets As java.awt.Insets)
				Me.insets = insets
				Me.painterKey = painterKey
			End Sub

			Public Overrides Sub paintBorder(ByVal c As java.awt.Component, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
				If painter Is Nothing Then
					painter = CType(javax.swing.UIManager.get(painterKey), javax.swing.Painter)
					If painter Is Nothing Then Return
				End If

				g.translate(x, y)
				If TypeOf g Is java.awt.Graphics2D Then
					painter.paint(CType(g, java.awt.Graphics2D), c, w, h)
				Else
					Dim img As New java.awt.image.BufferedImage(w, h, TYPE_INT_ARGB)
					Dim gfx As java.awt.Graphics2D = img.createGraphics()
					painter.paint(gfx, c, w, h)
					gfx.Dispose()
					g.drawImage(img, x, y, Nothing)
					img = Nothing
				End If
				g.translate(-x, -y)
			End Sub

			Public Overrides Function getBorderInsets(ByVal c As java.awt.Component) As java.awt.Insets
				Return CType(insets.clone(), java.awt.Insets)
			End Function

			Public Property Overrides borderOpaque As Boolean
				Get
					Return False
				End Get
			End Property
		End Class
	End Class

End Namespace
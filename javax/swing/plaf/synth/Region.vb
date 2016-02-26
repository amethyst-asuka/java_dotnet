Imports System.Collections.Generic

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
	''' A distinct rendering area of a Swing component.  A component may
	''' support one or more regions.  Specific component regions are defined
	''' by the typesafe enumeration in this class.
	''' <p>
	''' Regions are typically used as a way to identify the <code>Component</code>s
	''' and areas a particular style is to apply to. Synth's file format allows you
	''' to bind styles based on the name of a <code>Region</code>.
	''' The name is derived from the field name of the constant:
	''' <ol>
	'''  <li>Map all characters to lowercase.
	'''  <li>Map the first character to uppercase.
	'''  <li>Map the first character after underscores to uppercase.
	'''  <li>Remove all underscores.
	''' </ol>
	''' For example, to identify the <code>SPLIT_PANE</code>
	''' <code>Region</code> you would use <code>SplitPane</code>.
	''' The following shows a custom <code>SynthStyleFactory</code>
	''' that returns a specific style for split panes:
	''' <pre>
	'''    public SynthStyle getStyle(JComponent c, Region id) {
	'''        if (id == Region.SPLIT_PANE) {
	'''            return splitPaneStyle;
	'''        }
	'''        ...
	'''    }
	''' </pre>
	''' The following <a href="doc-files/synthFileFormat.html">xml</a>
	''' accomplishes the same thing:
	''' <pre>
	''' &lt;style id="splitPaneStyle"&gt;
	'''   ...
	''' &lt;/style&gt;
	''' &lt;bind style="splitPaneStyle" type="region" key="SplitPane"/&gt;
	''' </pre>
	''' 
	''' @since 1.5
	''' @author Scott Violet
	''' </summary>
	Public Class Region
		Private Shared ReadOnly UI_TO_REGION_MAP_KEY As New Object
		Private Shared ReadOnly LOWER_CASE_NAME_MAP_KEY As New Object

		''' <summary>
		''' ArrowButton's are special types of buttons that also render a
		''' directional indicator, typically an arrow. ArrowButtons are used by
		''' composite components, for example ScrollBar's contain ArrowButtons.
		''' To bind a style to this <code>Region</code> use the name
		''' <code>ArrowButton</code>.
		''' </summary>
		Public Shared ReadOnly ARROW_BUTTON As New Region("ArrowButton", False)

		''' <summary>
		''' Button region. To bind a style to this <code>Region</code> use the name
		''' <code>Button</code>.
		''' </summary>
		Public Shared ReadOnly BUTTON As New Region("Button", False)

		''' <summary>
		''' CheckBox region. To bind a style to this <code>Region</code> use the name
		''' <code>CheckBox</code>.
		''' </summary>
		Public Shared ReadOnly CHECK_BOX As New Region("CheckBox", False)

		''' <summary>
		''' CheckBoxMenuItem region. To bind a style to this <code>Region</code> use
		''' the name <code>CheckBoxMenuItem</code>.
		''' </summary>
		Public Shared ReadOnly CHECK_BOX_MENU_ITEM As New Region("CheckBoxMenuItem", False)

		''' <summary>
		''' ColorChooser region. To bind a style to this <code>Region</code> use
		''' the name <code>ColorChooser</code>.
		''' </summary>
		Public Shared ReadOnly COLOR_CHOOSER As New Region("ColorChooser", False)

		''' <summary>
		''' ComboBox region. To bind a style to this <code>Region</code> use
		''' the name <code>ComboBox</code>.
		''' </summary>
		Public Shared ReadOnly COMBO_BOX As New Region("ComboBox", False)

		''' <summary>
		''' DesktopPane region. To bind a style to this <code>Region</code> use
		''' the name <code>DesktopPane</code>.
		''' </summary>
		Public Shared ReadOnly DESKTOP_PANE As New Region("DesktopPane", False)

		''' <summary>
		''' DesktopIcon region. To bind a style to this <code>Region</code> use
		''' the name <code>DesktopIcon</code>.
		''' </summary>
		Public Shared ReadOnly DESKTOP_ICON As New Region("DesktopIcon", False)

		''' <summary>
		''' EditorPane region. To bind a style to this <code>Region</code> use
		''' the name <code>EditorPane</code>.
		''' </summary>
		Public Shared ReadOnly EDITOR_PANE As New Region("EditorPane", False)

		''' <summary>
		''' FileChooser region. To bind a style to this <code>Region</code> use
		''' the name <code>FileChooser</code>.
		''' </summary>
		Public Shared ReadOnly FILE_CHOOSER As New Region("FileChooser", False)

		''' <summary>
		''' FormattedTextField region. To bind a style to this <code>Region</code> use
		''' the name <code>FormattedTextField</code>.
		''' </summary>
		Public Shared ReadOnly FORMATTED_TEXT_FIELD As New Region("FormattedTextField", False)

		''' <summary>
		''' InternalFrame region. To bind a style to this <code>Region</code> use
		''' the name <code>InternalFrame</code>.
		''' </summary>
		Public Shared ReadOnly INTERNAL_FRAME As New Region("InternalFrame", False)

		''' <summary>
		''' TitlePane of an InternalFrame. The TitlePane typically
		''' shows a menu, title, widgets to manipulate the internal frame.
		''' To bind a style to this <code>Region</code> use the name
		''' <code>InternalFrameTitlePane</code>.
		''' </summary>
		Public Shared ReadOnly INTERNAL_FRAME_TITLE_PANE As New Region("InternalFrameTitlePane", False)

		''' <summary>
		''' Label region. To bind a style to this <code>Region</code> use the name
		''' <code>Label</code>.
		''' </summary>
		Public Shared ReadOnly LABEL As New Region("Label", False)

		''' <summary>
		''' List region. To bind a style to this <code>Region</code> use the name
		''' <code>List</code>.
		''' </summary>
		Public Shared ReadOnly LIST As New Region("List", False)

		''' <summary>
		''' Menu region. To bind a style to this <code>Region</code> use the name
		''' <code>Menu</code>.
		''' </summary>
		Public Shared ReadOnly MENU As New Region("Menu", False)

		''' <summary>
		''' MenuBar region. To bind a style to this <code>Region</code> use the name
		''' <code>MenuBar</code>.
		''' </summary>
		Public Shared ReadOnly MENU_BAR As New Region("MenuBar", False)

		''' <summary>
		''' MenuItem region. To bind a style to this <code>Region</code> use the name
		''' <code>MenuItem</code>.
		''' </summary>
		Public Shared ReadOnly MENU_ITEM As New Region("MenuItem", False)

		''' <summary>
		''' Accelerator region of a MenuItem. To bind a style to this
		''' <code>Region</code> use the name <code>MenuItemAccelerator</code>.
		''' </summary>
		Public Shared ReadOnly MENU_ITEM_ACCELERATOR As New Region("MenuItemAccelerator", True)

		''' <summary>
		''' OptionPane region. To bind a style to this <code>Region</code> use
		''' the name <code>OptionPane</code>.
		''' </summary>
		Public Shared ReadOnly OPTION_PANE As New Region("OptionPane", False)

		''' <summary>
		''' Panel region. To bind a style to this <code>Region</code> use the name
		''' <code>Panel</code>.
		''' </summary>
		Public Shared ReadOnly PANEL As New Region("Panel", False)

		''' <summary>
		''' PasswordField region. To bind a style to this <code>Region</code> use
		''' the name <code>PasswordField</code>.
		''' </summary>
		Public Shared ReadOnly PASSWORD_FIELD As New Region("PasswordField", False)

		''' <summary>
		''' PopupMenu region. To bind a style to this <code>Region</code> use
		''' the name <code>PopupMenu</code>.
		''' </summary>
		Public Shared ReadOnly POPUP_MENU As New Region("PopupMenu", False)

		''' <summary>
		''' PopupMenuSeparator region. To bind a style to this <code>Region</code>
		''' use the name <code>PopupMenuSeparator</code>.
		''' </summary>
		Public Shared ReadOnly POPUP_MENU_SEPARATOR As New Region("PopupMenuSeparator", False)

		''' <summary>
		''' ProgressBar region. To bind a style to this <code>Region</code>
		''' use the name <code>ProgressBar</code>.
		''' </summary>
		Public Shared ReadOnly PROGRESS_BAR As New Region("ProgressBar", False)

		''' <summary>
		''' RadioButton region. To bind a style to this <code>Region</code>
		''' use the name <code>RadioButton</code>.
		''' </summary>
		Public Shared ReadOnly RADIO_BUTTON As New Region("RadioButton", False)

		''' <summary>
		''' RegionButtonMenuItem region. To bind a style to this <code>Region</code>
		''' use the name <code>RadioButtonMenuItem</code>.
		''' </summary>
		Public Shared ReadOnly RADIO_BUTTON_MENU_ITEM As New Region("RadioButtonMenuItem", False)

		''' <summary>
		''' RootPane region. To bind a style to this <code>Region</code> use
		''' the name <code>RootPane</code>.
		''' </summary>
		Public Shared ReadOnly ROOT_PANE As New Region("RootPane", False)

		''' <summary>
		''' ScrollBar region. To bind a style to this <code>Region</code> use
		''' the name <code>ScrollBar</code>.
		''' </summary>
		Public Shared ReadOnly SCROLL_BAR As New Region("ScrollBar", False)

		''' <summary>
		''' Track of the ScrollBar. To bind a style to this <code>Region</code> use
		''' the name <code>ScrollBarTrack</code>.
		''' </summary>
		Public Shared ReadOnly SCROLL_BAR_TRACK As New Region("ScrollBarTrack", True)

		''' <summary>
		''' Thumb of the ScrollBar. The thumb is the region of the ScrollBar
		''' that gives a graphical depiction of what percentage of the View is
		''' currently visible. To bind a style to this <code>Region</code> use
		''' the name <code>ScrollBarThumb</code>.
		''' </summary>
		Public Shared ReadOnly SCROLL_BAR_THUMB As New Region("ScrollBarThumb", True)

		''' <summary>
		''' ScrollPane region. To bind a style to this <code>Region</code> use
		''' the name <code>ScrollPane</code>.
		''' </summary>
		Public Shared ReadOnly SCROLL_PANE As New Region("ScrollPane", False)

		''' <summary>
		''' Separator region. To bind a style to this <code>Region</code> use
		''' the name <code>Separator</code>.
		''' </summary>
		Public Shared ReadOnly SEPARATOR As New Region("Separator", False)

		''' <summary>
		''' Slider region. To bind a style to this <code>Region</code> use
		''' the name <code>Slider</code>.
		''' </summary>
		Public Shared ReadOnly SLIDER As New Region("Slider", False)

		''' <summary>
		''' Track of the Slider. To bind a style to this <code>Region</code> use
		''' the name <code>SliderTrack</code>.
		''' </summary>
		Public Shared ReadOnly SLIDER_TRACK As New Region("SliderTrack", True)

		''' <summary>
		''' Thumb of the Slider. The thumb of the Slider identifies the current
		''' value. To bind a style to this <code>Region</code> use the name
		''' <code>SliderThumb</code>.
		''' </summary>
		Public Shared ReadOnly SLIDER_THUMB As New Region("SliderThumb", True)

		''' <summary>
		''' Spinner region. To bind a style to this <code>Region</code> use the name
		''' <code>Spinner</code>.
		''' </summary>
		Public Shared ReadOnly SPINNER As New Region("Spinner", False)

		''' <summary>
		''' SplitPane region. To bind a style to this <code>Region</code> use the name
		''' <code>SplitPane</code>.
		''' </summary>
		Public Shared ReadOnly SPLIT_PANE As New Region("SplitPane", False)

		''' <summary>
		''' Divider of the SplitPane. To bind a style to this <code>Region</code>
		''' use the name <code>SplitPaneDivider</code>.
		''' </summary>
		Public Shared ReadOnly SPLIT_PANE_DIVIDER As New Region("SplitPaneDivider", True)

		''' <summary>
		''' TabbedPane region. To bind a style to this <code>Region</code> use
		''' the name <code>TabbedPane</code>.
		''' </summary>
		Public Shared ReadOnly TABBED_PANE As New Region("TabbedPane", False)

		''' <summary>
		''' Region of a TabbedPane for one tab. To bind a style to this
		''' <code>Region</code> use the name <code>TabbedPaneTab</code>.
		''' </summary>
		Public Shared ReadOnly TABBED_PANE_TAB As New Region("TabbedPaneTab", True)

		''' <summary>
		''' Region of a TabbedPane containing the tabs. To bind a style to this
		''' <code>Region</code> use the name <code>TabbedPaneTabArea</code>.
		''' </summary>
		Public Shared ReadOnly TABBED_PANE_TAB_AREA As New Region("TabbedPaneTabArea", True)

		''' <summary>
		''' Region of a TabbedPane containing the content. To bind a style to this
		''' <code>Region</code> use the name <code>TabbedPaneContent</code>.
		''' </summary>
		Public Shared ReadOnly TABBED_PANE_CONTENT As New Region("TabbedPaneContent", True)

		''' <summary>
		''' Table region. To bind a style to this <code>Region</code> use
		''' the name <code>Table</code>.
		''' </summary>
		Public Shared ReadOnly TABLE As New Region("Table", False)

		''' <summary>
		''' TableHeader region. To bind a style to this <code>Region</code> use
		''' the name <code>TableHeader</code>.
		''' </summary>
		Public Shared ReadOnly TABLE_HEADER As New Region("TableHeader", False)

		''' <summary>
		''' TextArea region. To bind a style to this <code>Region</code> use
		''' the name <code>TextArea</code>.
		''' </summary>
		Public Shared ReadOnly TEXT_AREA As New Region("TextArea", False)

		''' <summary>
		''' TextField region. To bind a style to this <code>Region</code> use
		''' the name <code>TextField</code>.
		''' </summary>
		Public Shared ReadOnly TEXT_FIELD As New Region("TextField", False)

		''' <summary>
		''' TextPane region. To bind a style to this <code>Region</code> use
		''' the name <code>TextPane</code>.
		''' </summary>
		Public Shared ReadOnly TEXT_PANE As New Region("TextPane", False)

		''' <summary>
		''' ToggleButton region. To bind a style to this <code>Region</code> use
		''' the name <code>ToggleButton</code>.
		''' </summary>
		Public Shared ReadOnly TOGGLE_BUTTON As New Region("ToggleButton", False)

		''' <summary>
		''' ToolBar region. To bind a style to this <code>Region</code> use
		''' the name <code>ToolBar</code>.
		''' </summary>
		Public Shared ReadOnly TOOL_BAR As New Region("ToolBar", False)

		''' <summary>
		''' Region of the ToolBar containing the content. To bind a style to this
		''' <code>Region</code> use the name <code>ToolBarContent</code>.
		''' </summary>
		Public Shared ReadOnly TOOL_BAR_CONTENT As New Region("ToolBarContent", True)

		''' <summary>
		''' Region for the Window containing the ToolBar. To bind a style to this
		''' <code>Region</code> use the name <code>ToolBarDragWindow</code>.
		''' </summary>
		Public Shared ReadOnly TOOL_BAR_DRAG_WINDOW As New Region("ToolBarDragWindow", False)

		''' <summary>
		''' ToolTip region. To bind a style to this <code>Region</code> use
		''' the name <code>ToolTip</code>.
		''' </summary>
		Public Shared ReadOnly TOOL_TIP As New Region("ToolTip", False)

		''' <summary>
		''' ToolBar separator region. To bind a style to this <code>Region</code> use
		''' the name <code>ToolBarSeparator</code>.
		''' </summary>
		Public Shared ReadOnly TOOL_BAR_SEPARATOR As New Region("ToolBarSeparator", False)

		''' <summary>
		''' Tree region. To bind a style to this <code>Region</code> use the name
		''' <code>Tree</code>.
		''' </summary>
		Public Shared ReadOnly TREE As New Region("Tree", False)

		''' <summary>
		''' Region of the Tree for one cell. To bind a style to this
		''' <code>Region</code> use the name <code>TreeCell</code>.
		''' </summary>
		Public Shared ReadOnly TREE_CELL As New Region("TreeCell", True)

		''' <summary>
		''' Viewport region. To bind a style to this <code>Region</code> use
		''' the name <code>Viewport</code>.
		''' </summary>
		Public Shared ReadOnly VIEWPORT As New Region("Viewport", False)

		Private Property Shared uItoRegionMap As IDictionary(Of String, Region)
			Get
				Dim context As sun.awt.AppContext = sun.awt.AppContext.appContext
				Dim map As IDictionary(Of String, Region) = CType(context.get(UI_TO_REGION_MAP_KEY), IDictionary(Of String, Region))
				If map Is Nothing Then
					map = New Dictionary(Of String, Region)
					map("ArrowButtonUI") = ARROW_BUTTON
					map("ButtonUI") = BUTTON
					map("CheckBoxUI") = CHECK_BOX
					map("CheckBoxMenuItemUI") = CHECK_BOX_MENU_ITEM
					map("ColorChooserUI") = COLOR_CHOOSER
					map("ComboBoxUI") = COMBO_BOX
					map("DesktopPaneUI") = DESKTOP_PANE
					map("DesktopIconUI") = DESKTOP_ICON
					map("EditorPaneUI") = EDITOR_PANE
					map("FileChooserUI") = FILE_CHOOSER
					map("FormattedTextFieldUI") = FORMATTED_TEXT_FIELD
					map("InternalFrameUI") = INTERNAL_FRAME
					map("InternalFrameTitlePaneUI") = INTERNAL_FRAME_TITLE_PANE
					map("LabelUI") = LABEL
					map("ListUI") = LIST
					map("MenuUI") = MENU
					map("MenuBarUI") = MENU_BAR
					map("MenuItemUI") = MENU_ITEM
					map("OptionPaneUI") = OPTION_PANE
					map("PanelUI") = PANEL
					map("PasswordFieldUI") = PASSWORD_FIELD
					map("PopupMenuUI") = POPUP_MENU
					map("PopupMenuSeparatorUI") = POPUP_MENU_SEPARATOR
					map("ProgressBarUI") = PROGRESS_BAR
					map("RadioButtonUI") = RADIO_BUTTON
					map("RadioButtonMenuItemUI") = RADIO_BUTTON_MENU_ITEM
					map("RootPaneUI") = ROOT_PANE
					map("ScrollBarUI") = SCROLL_BAR
					map("ScrollPaneUI") = SCROLL_PANE
					map("SeparatorUI") = SEPARATOR
					map("SliderUI") = SLIDER
					map("SpinnerUI") = SPINNER
					map("SplitPaneUI") = SPLIT_PANE
					map("TabbedPaneUI") = TABBED_PANE
					map("TableUI") = TABLE
					map("TableHeaderUI") = TABLE_HEADER
					map("TextAreaUI") = TEXT_AREA
					map("TextFieldUI") = TEXT_FIELD
					map("TextPaneUI") = TEXT_PANE
					map("ToggleButtonUI") = TOGGLE_BUTTON
					map("ToolBarUI") = TOOL_BAR
					map("ToolTipUI") = TOOL_TIP
					map("ToolBarSeparatorUI") = TOOL_BAR_SEPARATOR
					map("TreeUI") = TREE
					map("ViewportUI") = VIEWPORT
					context.put(UI_TO_REGION_MAP_KEY, map)
				End If
				Return map
			End Get
		End Property

		Private Property Shared lowerCaseNameMap As IDictionary(Of Region, String)
			Get
				Dim context As sun.awt.AppContext = sun.awt.AppContext.appContext
				Dim map As IDictionary(Of Region, String) = CType(context.get(LOWER_CASE_NAME_MAP_KEY), IDictionary(Of Region, String))
				If map Is Nothing Then
					map = New Dictionary(Of Region, String)
					context.put(LOWER_CASE_NAME_MAP_KEY, map)
				End If
				Return map
			End Get
		End Property

		Shared Function getRegion(ByVal c As javax.swing.JComponent) As Region
			Return uItoRegionMap(c.uIClassID)
		End Function

		Friend Shared Sub registerUIs(ByVal table As javax.swing.UIDefaults)
			For Each key As Object In uItoRegionMap.Keys
				table(key) = "javax.swing.plaf.synth.SynthLookAndFeel"
			Next key
		End Sub

		Private ReadOnly name As String
		Private ReadOnly subregion As Boolean

		Private Sub New(ByVal name As String, ByVal subregion As Boolean)
			If name Is Nothing Then Throw New NullPointerException("You must specify a non-null name")
			Me.name = name
			Me.subregion = subregion
		End Sub

		''' <summary>
		''' Creates a Region with the specified name. This should only be
		''' used if you are creating your own <code>JComponent</code> subclass
		''' with a custom <code>ComponentUI</code> class.
		''' </summary>
		''' <param name="name"> Name of the region </param>
		''' <param name="ui"> String that will be returned from
		'''           <code>component.getUIClassID</code>. This will be null
		'''           if this is a subregion. </param>
		''' <param name="subregion"> Whether or not this is a subregion. </param>
		Protected Friend Sub New(ByVal name As String, ByVal ui As String, ByVal subregion As Boolean)
			Me.New(name, subregion)
			If ui IsNot Nothing Then uItoRegionMap(ui) = Me
		End Sub

		''' <summary>
		''' Returns true if the Region is a subregion of a Component, otherwise
		''' false. For example, <code>Region.BUTTON</code> corresponds do a
		''' <code>Component</code> so that <code>Region.BUTTON.isSubregion()</code>
		''' returns false.
		''' </summary>
		''' <returns> true if the Region is a subregion of a Component. </returns>
		Public Overridable Property subregion As Boolean
			Get
				Return subregion
			End Get
		End Property

		''' <summary>
		''' Returns the name of the region.
		''' </summary>
		''' <returns> name of the Region. </returns>
		Public Overridable Property name As String
			Get
				Return name
			End Get
		End Property

		''' <summary>
		''' Returns the name, in lowercase.
		''' </summary>
		''' <returns> lower case representation of the name of the Region </returns>
		Friend Overridable Property lowerCaseName As String
			Get
				Dim ___lowerCaseNameMap As IDictionary(Of Region, String) = lowerCaseNameMap
				Dim ___lowerCaseName As String = ___lowerCaseNameMap(Me)
				If ___lowerCaseName Is Nothing Then
					___lowerCaseName = name.ToLower(java.util.Locale.ENGLISH)
					___lowerCaseNameMap(Me) = ___lowerCaseName
				End If
				Return ___lowerCaseName
			End Get
		End Property

		''' <summary>
		''' Returns the name of the Region.
		''' </summary>
		''' <returns> name of the Region. </returns>
		Public Overrides Function ToString() As String
			Return name
		End Function
	End Class

End Namespace
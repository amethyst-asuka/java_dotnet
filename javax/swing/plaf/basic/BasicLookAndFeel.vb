Imports System
Imports System.Collections.Generic
Imports javax.sound.sampled
Imports javax.swing.border
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
	''' A base class to use in creating a look and feel for Swing.
	''' <p>
	''' Each of the {@code ComponentUI}s provided by {@code
	''' BasicLookAndFeel} derives its behavior from the defaults
	''' table. Unless otherwise noted each of the {@code ComponentUI}
	''' implementations in this package document the set of defaults they
	''' use. Unless otherwise noted the defaults are installed at the time
	''' {@code installUI} is invoked, and follow the recommendations
	''' outlined in {@code LookAndFeel} for installing defaults.
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
	''' @author unattributed
	''' </summary>
	<Serializable> _
	Public MustInherit Class BasicLookAndFeel
		Inherits javax.swing.LookAndFeel

		''' <summary>
		''' Whether or not the developer has created a JPopupMenu.
		''' </summary>
		Friend Shared needsEventHelper As Boolean

		''' <summary>
		''' Lock used when manipulating clipPlaying.
		''' </summary>
		<NonSerialized> _
		Private audioLock As New Object
		''' <summary>
		''' The Clip that is currently playing (set in AudioAction).
		''' </summary>
		Private clipPlaying As Clip

		Friend invocator As AWTEventHelper = Nothing

	'    
	'     * Listen for our AppContext being disposed
	'     
		Private disposer As java.beans.PropertyChangeListener = Nothing

		''' <summary>
		''' Returns the look and feel defaults. The returned {@code UIDefaults}
		''' is populated by invoking, in order, {@code initClassDefaults},
		''' {@code initSystemColorDefaults} and {@code initComponentDefaults}.
		''' <p>
		''' While this method is public, it should only be invoked by the
		''' {@code UIManager} when the look and feel is set as the current
		''' look and feel and after {@code initialize} has been invoked.
		''' </summary>
		''' <returns> the look and feel defaults
		''' </returns>
		''' <seealso cref= #initClassDefaults </seealso>
		''' <seealso cref= #initSystemColorDefaults </seealso>
		''' <seealso cref= #initComponentDefaults </seealso>
		Public Property Overrides defaults As javax.swing.UIDefaults
			Get
				Dim table As New javax.swing.UIDefaults(610, 0.75f)
    
				initClassDefaults(table)
				initSystemColorDefaults(table)
				initComponentDefaults(table)
    
				Return table
			End Get
		End Property

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Sub initialize()
			If needsEventHelper Then installAWTEventListener()
		End Sub

		Friend Overridable Sub installAWTEventListener()
			If invocator Is Nothing Then
				invocator = New AWTEventHelper(Me)
				needsEventHelper = True

				' Add a PropertyChangeListener to our AppContext so we're alerted
				' when the AppContext is disposed(), at which time this laf should
				' be uninitialize()d.
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				disposer = New java.beans.PropertyChangeListener()
	'			{
	'				public void propertyChange(PropertyChangeEvent prpChg)
	'				{
	'					uninitialize();
	'				}
	'			};
				sun.awt.AppContext.appContext.addPropertyChangeListener(sun.awt.AppContext.GUI_DISPOSED, disposer)
			End If
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Sub uninitialize()
			Dim context As sun.awt.AppContext = sun.awt.AppContext.appContext
			SyncLock BasicPopupMenuUI.MOUSE_GRABBER_KEY
				Dim grabber As Object = context.get(BasicPopupMenuUI.MOUSE_GRABBER_KEY)
				If grabber IsNot Nothing Then CType(grabber, BasicPopupMenuUI.MouseGrabber).uninstall()
			End SyncLock
			SyncLock BasicPopupMenuUI.MENU_KEYBOARD_HELPER_KEY
				Dim helper As Object = context.get(BasicPopupMenuUI.MENU_KEYBOARD_HELPER_KEY)
				If helper IsNot Nothing Then CType(helper, BasicPopupMenuUI.MenuKeyboardHelper).uninstall()
			End SyncLock

			If invocator IsNot Nothing Then
				java.security.AccessController.doPrivileged(invocator)
				invocator = Nothing
			End If

			If disposer IsNot Nothing Then
				' Note that we're likely calling removePropertyChangeListener()
				' during the course of AppContext.firePropertyChange().
				' However, EventListenerAggreggate has code to safely modify
				' the list under such circumstances.
				context.removePropertyChangeListener(sun.awt.AppContext.GUI_DISPOSED, disposer)
				disposer = Nothing
			End If
		End Sub

		''' <summary>
		''' Populates {@code table} with mappings from {@code uiClassID} to the
		''' fully qualified name of the ui class. The value for a
		''' particular {@code uiClassID} is {@code
		''' "javax.swing.plaf.basic.Basic + uiClassID"}. For example, the
		''' value for the {@code uiClassID} {@code TreeUI} is {@code
		''' "javax.swing.plaf.basic.BasicTreeUI"}.
		''' </summary>
		''' <param name="table"> the {@code UIDefaults} instance the entries are
		'''        added to </param>
		''' <exception cref="NullPointerException"> if {@code table} is {@code null}
		''' </exception>
		''' <seealso cref= javax.swing.LookAndFeel </seealso>
		''' <seealso cref= #getDefaults </seealso>
		Protected Friend Overridable Sub initClassDefaults(ByVal table As javax.swing.UIDefaults)
			Const basicPackageName As String = "javax.swing.plaf.basic."
			Dim uiDefaults As Object() = { "ButtonUI", basicPackageName & "BasicButtonUI", "CheckBoxUI", basicPackageName & "BasicCheckBoxUI", "ColorChooserUI", basicPackageName & "BasicColorChooserUI", "FormattedTextFieldUI", basicPackageName & "BasicFormattedTextFieldUI", "MenuBarUI", basicPackageName & "BasicMenuBarUI", "MenuUI", basicPackageName & "BasicMenuUI", "MenuItemUI", basicPackageName & "BasicMenuItemUI", "CheckBoxMenuItemUI", basicPackageName & "BasicCheckBoxMenuItemUI", "RadioButtonMenuItemUI", basicPackageName & "BasicRadioButtonMenuItemUI", "RadioButtonUI", basicPackageName & "BasicRadioButtonUI", "ToggleButtonUI", basicPackageName & "BasicToggleButtonUI", "PopupMenuUI", basicPackageName & "BasicPopupMenuUI", "ProgressBarUI", basicPackageName & "BasicProgressBarUI", "ScrollBarUI", basicPackageName & "BasicScrollBarUI", "ScrollPaneUI", basicPackageName & "BasicScrollPaneUI", "SplitPaneUI", basicPackageName & "BasicSplitPaneUI", "SliderUI", basicPackageName & "BasicSliderUI", "SeparatorUI", basicPackageName & "BasicSeparatorUI", "SpinnerUI", basicPackageName & "BasicSpinnerUI", "ToolBarSeparatorUI", basicPackageName & "BasicToolBarSeparatorUI", "PopupMenuSeparatorUI", basicPackageName & "BasicPopupMenuSeparatorUI", "TabbedPaneUI", basicPackageName & "BasicTabbedPaneUI", "TextAreaUI", basicPackageName & "BasicTextAreaUI", "TextFieldUI", basicPackageName & "BasicTextFieldUI", "PasswordFieldUI", basicPackageName & "BasicPasswordFieldUI", "TextPaneUI", basicPackageName & "BasicTextPaneUI", "EditorPaneUI", basicPackageName & "BasicEditorPaneUI", "TreeUI", basicPackageName & "BasicTreeUI", "LabelUI", basicPackageName & "BasicLabelUI", "ListUI", basicPackageName & "BasicListUI", "ToolBarUI", basicPackageName & "BasicToolBarUI", "ToolTipUI", basicPackageName & "BasicToolTipUI", "ComboBoxUI", basicPackageName & "BasicComboBoxUI", "TableUI", basicPackageName & "BasicTableUI", "TableHeaderUI", basicPackageName & "BasicTableHeaderUI", "InternalFrameUI", basicPackageName & "BasicInternalFrameUI", "DesktopPaneUI", basicPackageName & "BasicDesktopPaneUI", "DesktopIconUI", basicPackageName & "BasicDesktopIconUI", "FileChooserUI", basicPackageName & "BasicFileChooserUI", "OptionPaneUI", basicPackageName & "BasicOptionPaneUI", "PanelUI", basicPackageName & "BasicPanelUI", "ViewportUI", basicPackageName & "BasicViewportUI", "RootPaneUI", basicPackageName & "BasicRootPaneUI" }

			table.putDefaults(uiDefaults)
		End Sub

		''' <summary>
		''' Populates {@code table} with system colors. This creates an
		''' array of {@code name-color} pairs and invokes {@code
		''' loadSystemColors}.
		''' <p>
		''' The name is a {@code String} that corresponds to the name of
		''' one of the static {@code SystemColor} fields in the {@code
		''' SystemColor} class.  A name-color pair is created for every
		''' such {@code SystemColor} field.
		''' <p>
		''' The {@code color} corresponds to a hex {@code String} as
		''' understood by {@code Color.decode}. For example, one of the
		''' {@code name-color} pairs is {@code
		''' "desktop"-"#005C5C"}. This corresponds to the {@code
		''' SystemColor} field {@code desktop}, with a color value of
		''' {@code new Color(0x005C5C)}.
		''' <p>
		''' The following shows two of the {@code name-color} pairs:
		''' <pre>
		'''   String[] nameColorPairs = new String[] {
		'''          "desktop", "#005C5C",
		'''    "activeCaption", "#000080" };
		'''   loadSystemColors(table, nameColorPairs, isNativeLookAndFeel());
		''' </pre>
		''' 
		''' As previously stated, this invokes {@code loadSystemColors}
		''' with the supplied {@code table} and {@code name-color} pair
		''' array. The last argument to {@code loadSystemColors} indicates
		''' whether the value of the field in {@code SystemColor} should be
		''' used. This method passes the value of {@code
		''' isNativeLookAndFeel()} as the last argument to {@code loadSystemColors}.
		''' </summary>
		''' <param name="table"> the {@code UIDefaults} object the values are added to </param>
		''' <exception cref="NullPointerException"> if {@code table} is {@code null}
		''' </exception>
		''' <seealso cref= java.awt.SystemColor </seealso>
		''' <seealso cref= #getDefaults </seealso>
		''' <seealso cref= #loadSystemColors </seealso>
		Protected Friend Overridable Sub initSystemColorDefaults(ByVal table As javax.swing.UIDefaults)
			Dim defaultSystemColors As String() = { "desktop", "#005C5C", "activeCaption", "#000080", "activeCaptionText", "#FFFFFF", "activeCaptionBorder", "#C0C0C0", "inactiveCaption", "#808080", "inactiveCaptionText", "#C0C0C0", "inactiveCaptionBorder", "#C0C0C0", "window", "#FFFFFF", "windowBorder", "#000000", "windowText", "#000000", "menu", "#C0C0C0", "menuText", "#000000", "text", "#C0C0C0", "textText", "#000000", "textHighlight", "#000080", "textHighlightText", "#FFFFFF", "textInactiveText", "#808080", "control", "#C0C0C0", "controlText", "#000000", "controlHighlight", "#C0C0C0", "controlLtHighlight", "#FFFFFF", "controlShadow", "#808080", "controlDkShadow", "#000000", "scrollbar", "#E0E0E0", "info", "#FFFFE1", "infoText", "#000000" }

			loadSystemColors(table, defaultSystemColors, nativeLookAndFeel)
		End Sub


		''' <summary>
		''' Populates {@code table} with the {@code name-color} pairs in
		''' {@code systemColors}. Refer to
		''' <seealso cref="#initSystemColorDefaults(UIDefaults)"/> for details on
		''' the format of {@code systemColors}.
		''' <p>
		''' An entry is added to {@code table} for each of the {@code name-color}
		''' pairs in {@code systemColors}. The entry key is
		''' the {@code name} of the {@code name-color} pair.
		''' <p>
		''' The value of the entry corresponds to the {@code color} of the
		''' {@code name-color} pair.  The value of the entry is calculated
		''' in one of two ways. With either approach the value is always a
		''' {@code ColorUIResource}.
		''' <p>
		''' If {@code useNative} is {@code false}, the {@code color} is
		''' created by using {@code Color.decode} to convert the {@code
		''' String} into a {@code Color}. If {@code decode} can not convert
		''' the {@code String} into a {@code Color} ({@code
		''' NumberFormatException} is thrown) then a {@code
		''' ColorUIResource} of black is used.
		''' <p>
		''' If {@code useNative} is {@code true}, the {@code color} is the
		''' value of the field in {@code SystemColor} with the same name as
		''' the {@code name} of the {@code name-color} pair. If the field
		''' is not valid, a {@code ColorUIResource} of black is used.
		''' </summary>
		''' <param name="table"> the {@code UIDefaults} object the values are added to </param>
		''' <param name="systemColors"> array of {@code name-color} pairs as described
		'''        in <seealso cref="#initSystemColorDefaults(UIDefaults)"/> </param>
		''' <param name="useNative"> whether the color is obtained from {@code SystemColor}
		'''        or {@code Color.decode} </param>
		''' <exception cref="NullPointerException"> if {@code systemColors} is {@code null}; or
		'''         {@code systemColors} is not empty, and {@code table} is
		'''         {@code null}; or one of the
		'''         names of the {@code name-color} pairs is {@code null}; or
		'''         {@code useNative} is {@code false} and one of the
		'''         {@code colors} of the {@code name-color} pairs is {@code null} </exception>
		''' <exception cref="ArrayIndexOutOfBoundsException"> if {@code useNative} is
		'''         {@code false} and {@code systemColors.length} is odd
		''' </exception>
		''' <seealso cref= #initSystemColorDefaults(javax.swing.UIDefaults) </seealso>
		''' <seealso cref= java.awt.SystemColor </seealso>
		''' <seealso cref= java.awt.Color#decode(String) </seealso>
		Protected Friend Overridable Sub loadSystemColors(ByVal table As javax.swing.UIDefaults, ByVal systemColors As String(), ByVal useNative As Boolean)
	'         PENDING(hmuller) We don't load the system colors below because
	'         * they're not reliable.  Hopefully we'll be able to do better in
	'         * a future version of AWT.
	'         
			If useNative Then
				For i As Integer = 0 To systemColors.Length - 1 Step 2
					Dim color As java.awt.Color = java.awt.Color.black
					Try
						Dim ___name As String = systemColors(i)
						color = CType(GetType(java.awt.SystemColor).getField(___name).get(Nothing), java.awt.Color)
					Catch e As Exception
					End Try
					table(systemColors(i)) = New ColorUIResource(color)
				Next i
			Else
				For i As Integer = 0 To systemColors.Length - 1 Step 2
					Dim color As java.awt.Color = java.awt.Color.black
					Try
						color = java.awt.Color.decode(systemColors(i + 1))
					Catch e As NumberFormatException
						Console.WriteLine(e.ToString())
						Console.Write(e.StackTrace)
					End Try
					table(systemColors(i)) = New ColorUIResource(color)
				Next i
			End If
		End Sub
		''' <summary>
		''' Initialize the defaults table with the name of the ResourceBundle
		''' used for getting localized defaults.  Also initialize the default
		''' locale used when no locale is passed into UIDefaults.get().  The
		''' default locale should generally not be relied upon. It is here for
		''' compatibility with releases prior to 1.4.
		''' </summary>
		Private Sub initResourceBundle(ByVal table As javax.swing.UIDefaults)
			table.defaultLocale = Locale.default
			table.addResourceBundle("com.sun.swing.internal.plaf.basic.resources.basic")
		End Sub

		''' <summary>
		''' Populates {@code table} with the defaults for the basic look and
		''' feel.
		''' </summary>
		''' <param name="table"> the {@code UIDefaults} to add the values to </param>
		''' <exception cref="NullPointerException"> if {@code table} is {@code null} </exception>
		Protected Friend Overridable Sub initComponentDefaults(ByVal table As javax.swing.UIDefaults)

			initResourceBundle(table)

			' *** Shared Integers
			Dim fiveHundred As Integer? = New Integer?(500)

			' *** Shared Longs
			Dim oneThousand As Long? = New Long?(1000)

			' *** Shared Fonts
			Dim twelve As Integer? = New Integer?(12)
			Dim fontPlain As Integer? = New Integer?(java.awt.Font.PLAIN)
			Dim fontBold As Integer? = New Integer?(java.awt.Font.BOLD)
			Dim dialogPlain12 As Object = New sun.swing.SwingLazyValue("javax.swing.plaf.FontUIResource", Nothing, New Object() {java.awt.Font.DIALOG, fontPlain, twelve})
			Dim serifPlain12 As Object = New sun.swing.SwingLazyValue("javax.swing.plaf.FontUIResource", Nothing, New Object() {java.awt.Font.SERIF, fontPlain, twelve})
			Dim sansSerifPlain12 As Object = New sun.swing.SwingLazyValue("javax.swing.plaf.FontUIResource", Nothing, New Object() {java.awt.Font.SANS_SERIF, fontPlain, twelve})
			Dim monospacedPlain12 As Object = New sun.swing.SwingLazyValue("javax.swing.plaf.FontUIResource", Nothing, New Object() {java.awt.Font.MONOSPACED, fontPlain, twelve})
			Dim dialogBold12 As Object = New sun.swing.SwingLazyValue("javax.swing.plaf.FontUIResource", Nothing, New Object() {java.awt.Font.DIALOG, fontBold, twelve})


			' *** Shared Colors
			Dim red As New ColorUIResource(java.awt.Color.red)
			Dim black As New ColorUIResource(java.awt.Color.black)
			Dim white As New ColorUIResource(java.awt.Color.white)
			Dim yellow As New ColorUIResource(java.awt.Color.yellow)
			Dim gray As New ColorUIResource(java.awt.Color.gray)
			Dim lightGray As New ColorUIResource(java.awt.Color.lightGray)
			Dim darkGray As New ColorUIResource(java.awt.Color.darkGray)
			Dim scrollBarTrack As New ColorUIResource(224, 224, 224)

			Dim control As java.awt.Color = table.getColor("control")
			Dim controlDkShadow As java.awt.Color = table.getColor("controlDkShadow")
			Dim controlHighlight As java.awt.Color = table.getColor("controlHighlight")
			Dim controlLtHighlight As java.awt.Color = table.getColor("controlLtHighlight")
			Dim controlShadow As java.awt.Color = table.getColor("controlShadow")
			Dim controlText As java.awt.Color = table.getColor("controlText")
			Dim menu As java.awt.Color = table.getColor("menu")
			Dim menuText As java.awt.Color = table.getColor("menuText")
			Dim textHighlight As java.awt.Color = table.getColor("textHighlight")
			Dim textHighlightText As java.awt.Color = table.getColor("textHighlightText")
			Dim textInactiveText As java.awt.Color = table.getColor("textInactiveText")
			Dim textText As java.awt.Color = table.getColor("textText")
			Dim window As java.awt.Color = table.getColor("window")

			' *** Shared Insets
			Dim zeroInsets As New InsetsUIResource(0,0,0,0)
			Dim twoInsets As New InsetsUIResource(2,2,2,2)
			Dim threeInsets As New InsetsUIResource(3,3,3,3)

			' *** Shared Borders
			Dim marginBorder As Object = New sun.swing.SwingLazyValue("javax.swing.plaf.basic.BasicBorders$MarginBorder")
			Dim ___etchedBorder As Object = New sun.swing.SwingLazyValue("javax.swing.plaf.BorderUIResource", "getEtchedBorderUIResource")
			Dim loweredBevelBorder As Object = New sun.swing.SwingLazyValue("javax.swing.plaf.BorderUIResource", "getLoweredBevelBorderUIResource")

			Dim popupMenuBorder As Object = New sun.swing.SwingLazyValue("javax.swing.plaf.basic.BasicBorders", "getInternalFrameBorder")

			Dim blackLineBorder As Object = New sun.swing.SwingLazyValue("javax.swing.plaf.BorderUIResource", "getBlackLineBorderUIResource")
			Dim focusCellHighlightBorder As Object = New sun.swing.SwingLazyValue("javax.swing.plaf.BorderUIResource$LineBorderUIResource", Nothing, New Object() {yellow})

			Dim noFocusBorder As Object = New BorderUIResource.EmptyBorderUIResource(1,1,1,1)

			Dim tableHeaderBorder As Object = New sun.swing.SwingLazyValue("javax.swing.plaf.BorderUIResource$BevelBorderUIResource", Nothing, New Object() { New Integer?(BevelBorder.RAISED), controlLtHighlight, control, controlDkShadow, controlShadow })


			' *** Button value objects

			Dim buttonBorder As Object = New sun.swing.SwingLazyValue("javax.swing.plaf.basic.BasicBorders", "getButtonBorder")

			Dim buttonToggleBorder As Object = New sun.swing.SwingLazyValue("javax.swing.plaf.basic.BasicBorders", "getToggleButtonBorder")

			Dim radioButtonBorder As Object = New sun.swing.SwingLazyValue("javax.swing.plaf.basic.BasicBorders", "getRadioButtonBorder")

			' *** FileChooser / FileView value objects

			Dim newFolderIcon As Object = sun.swing.SwingUtilities2.makeIcon(Me.GetType(), GetType(BasicLookAndFeel), "icons/NewFolder.gif")
			Dim upFolderIcon As Object = sun.swing.SwingUtilities2.makeIcon(Me.GetType(), GetType(BasicLookAndFeel), "icons/UpFolder.gif")
			Dim homeFolderIcon As Object = sun.swing.SwingUtilities2.makeIcon(Me.GetType(), GetType(BasicLookAndFeel), "icons/HomeFolder.gif")
			Dim detailsViewIcon As Object = sun.swing.SwingUtilities2.makeIcon(Me.GetType(), GetType(BasicLookAndFeel), "icons/DetailsView.gif")
			Dim listViewIcon As Object = sun.swing.SwingUtilities2.makeIcon(Me.GetType(), GetType(BasicLookAndFeel), "icons/ListView.gif")
			Dim directoryIcon As Object = sun.swing.SwingUtilities2.makeIcon(Me.GetType(), GetType(BasicLookAndFeel), "icons/Directory.gif")
			Dim fileIcon As Object = sun.swing.SwingUtilities2.makeIcon(Me.GetType(), GetType(BasicLookAndFeel), "icons/File.gif")
			Dim computerIcon As Object = sun.swing.SwingUtilities2.makeIcon(Me.GetType(), GetType(BasicLookAndFeel), "icons/Computer.gif")
			Dim hardDriveIcon As Object = sun.swing.SwingUtilities2.makeIcon(Me.GetType(), GetType(BasicLookAndFeel), "icons/HardDrive.gif")
			Dim floppyDriveIcon As Object = sun.swing.SwingUtilities2.makeIcon(Me.GetType(), GetType(BasicLookAndFeel), "icons/FloppyDrive.gif")


			' *** InternalFrame value objects

			Dim internalFrameBorder As Object = New sun.swing.SwingLazyValue("javax.swing.plaf.basic.BasicBorders", "getInternalFrameBorder")

			' *** List value objects

			Dim listCellRendererActiveValue As Object = New ActiveValueAnonymousInnerClassHelper


			' *** Menus value objects

			Dim menuBarBorder As Object = New sun.swing.SwingLazyValue("javax.swing.plaf.basic.BasicBorders", "getMenuBarBorder")

			Dim menuItemCheckIcon As Object = New sun.swing.SwingLazyValue("javax.swing.plaf.basic.BasicIconFactory", "getMenuItemCheckIcon")

			Dim menuItemArrowIcon As Object = New sun.swing.SwingLazyValue("javax.swing.plaf.basic.BasicIconFactory", "getMenuItemArrowIcon")


			Dim menuArrowIcon As Object = New sun.swing.SwingLazyValue("javax.swing.plaf.basic.BasicIconFactory", "getMenuArrowIcon")

			Dim checkBoxIcon As Object = New sun.swing.SwingLazyValue("javax.swing.plaf.basic.BasicIconFactory", "getCheckBoxIcon")

			Dim radioButtonIcon As Object = New sun.swing.SwingLazyValue("javax.swing.plaf.basic.BasicIconFactory", "getRadioButtonIcon")

			Dim checkBoxMenuItemIcon As Object = New sun.swing.SwingLazyValue("javax.swing.plaf.basic.BasicIconFactory", "getCheckBoxMenuItemIcon")

			Dim radioButtonMenuItemIcon As Object = New sun.swing.SwingLazyValue("javax.swing.plaf.basic.BasicIconFactory", "getRadioButtonMenuItemIcon")

			Dim menuItemAcceleratorDelimiter As Object = "+"

			' *** OptionPane value objects

			Dim optionPaneMinimumSize As Object = New DimensionUIResource(262, 90)

			Dim zero As Integer? = New Integer?(0)
			Dim zeroBorder As Object = New sun.swing.SwingLazyValue("javax.swing.plaf.BorderUIResource$EmptyBorderUIResource", New Object() {zero, zero, zero, zero})

			Dim ten As Integer? = New Integer?(10)
			Dim optionPaneBorder As Object = New sun.swing.SwingLazyValue("javax.swing.plaf.BorderUIResource$EmptyBorderUIResource", New Object() {ten, ten, twelve, ten})

			Dim optionPaneButtonAreaBorder As Object = New sun.swing.SwingLazyValue("javax.swing.plaf.BorderUIResource$EmptyBorderUIResource", New Object() {New Integer?(6), zero, zero, zero})


			' *** ProgessBar value objects

			Dim progressBarBorder As Object = New sun.swing.SwingLazyValue("javax.swing.plaf.basic.BasicBorders", "getProgressBarBorder")

			' ** ScrollBar value objects

			Dim minimumThumbSize As Object = New DimensionUIResource(8,8)
			Dim maximumThumbSize As Object = New DimensionUIResource(4096,4096)

			' ** Slider value objects

			Dim sliderFocusInsets As Object = twoInsets

			Dim toolBarSeparatorSize As Object = New DimensionUIResource(10, 10)


			' *** SplitPane value objects

			Dim splitPaneBorder As Object = New sun.swing.SwingLazyValue("javax.swing.plaf.basic.BasicBorders", "getSplitPaneBorder")
			Dim splitPaneDividerBorder As Object = New sun.swing.SwingLazyValue("javax.swing.plaf.basic.BasicBorders", "getSplitPaneDividerBorder")

			' ** TabbedBane value objects

			Dim tabbedPaneTabInsets As Object = New InsetsUIResource(0, 4, 1, 4)

			Dim tabbedPaneTabPadInsets As Object = New InsetsUIResource(2, 2, 2, 1)

			Dim tabbedPaneTabAreaInsets As Object = New InsetsUIResource(3, 2, 0, 2)

			Dim tabbedPaneContentBorderInsets As Object = New InsetsUIResource(2, 2, 3, 3)


			' *** Text value objects

			Dim textFieldBorder As Object = New sun.swing.SwingLazyValue("javax.swing.plaf.basic.BasicBorders", "getTextFieldBorder")

			Dim editorMargin As Object = threeInsets

			Dim caretBlinkRate As Object = fiveHundred
			Dim four As Integer? = New Integer?(4)

			Dim allAuditoryCues As Object() = { "CheckBoxMenuItem.commandSound", "InternalFrame.closeSound", "InternalFrame.maximizeSound", "InternalFrame.minimizeSound", "InternalFrame.restoreDownSound", "InternalFrame.restoreUpSound", "MenuItem.commandSound", "OptionPane.errorSound", "OptionPane.informationSound", "OptionPane.questionSound", "OptionPane.warningSound", "PopupMenu.popupSound", "RadioButtonMenuItem.commandSound"}

			Dim noAuditoryCues As Object() = {"mute"}

			' *** Component Defaults

			Dim ___defaults As Object() = { "AuditoryCues.cueList", allAuditoryCues, "AuditoryCues.allAuditoryCues", allAuditoryCues, "AuditoryCues.noAuditoryCues", noAuditoryCues, "AuditoryCues.playList", Nothing, "Button.defaultButtonFollowsFocus", Boolean.TRUE, "Button.font", dialogPlain12, "Button.background", control, "Button.foreground", controlText, "Button.shadow", controlShadow, "Button.darkShadow", controlDkShadow, "Button.light", controlHighlight, "Button.highlight", controlLtHighlight, "Button.border", buttonBorder, "Button.margin", New InsetsUIResource(2, 14, 2, 14), "Button.textIconGap", four, "Button.textShiftOffset", zero, "Button.focusInputMap", New javax.swing.UIDefaults.LazyInputMap(New Object() { "SPACE", "pressed", "released SPACE", "released", "ENTER", "pressed", "released ENTER", "released" }), "ToggleButton.font", dialogPlain12, "ToggleButton.background", control, "ToggleButton.foreground", controlText, "ToggleButton.shadow", controlShadow, "ToggleButton.darkShadow", controlDkShadow, "ToggleButton.light", controlHighlight, "ToggleButton.highlight", controlLtHighlight, "ToggleButton.border", buttonToggleBorder, "ToggleButton.margin", New InsetsUIResource(2, 14, 2, 14), "ToggleButton.textIconGap", four, "ToggleButton.textShiftOffset", zero, "ToggleButton.focusInputMap", New javax.swing.UIDefaults.LazyInputMap(New Object() { "SPACE", "pressed", "released SPACE", "released" }), "RadioButton.font", dialogPlain12, "RadioButton.background", control, "RadioButton.foreground", controlText, "RadioButton.shadow", controlShadow, "RadioButton.darkShadow", controlDkShadow, "RadioButton.light", controlHighlight, "RadioButton.highlight", controlLtHighlight, "RadioButton.border", radioButtonBorder, "RadioButton.margin", twoInsets, "RadioButton.textIconGap", four, "RadioButton.textShiftOffset", zero, "RadioButton.icon", radioButtonIcon, "RadioButton.focusInputMap", New javax.swing.UIDefaults.LazyInputMap(New Object() { "SPACE", "pressed", "released SPACE", "released", "RETURN", "pressed" }), "CheckBox.font", dialogPlain12, "CheckBox.background", control, "CheckBox.foreground", controlText, "CheckBox.border", radioButtonBorder, "CheckBox.margin", twoInsets, "CheckBox.textIconGap", four, "CheckBox.textShiftOffset", zero, "CheckBox.icon", checkBoxIcon, "CheckBox.focusInputMap", New javax.swing.UIDefaults.LazyInputMap(New Object() { "SPACE", "pressed", "released SPACE", "released" }), "FileChooser.useSystemExtensionHiding", Boolean.FALSE, "ColorChooser.font", dialogPlain12, "ColorChooser.background", control, "ColorChooser.foreground", controlText, "ColorChooser.swatchesSwatchSize", New java.awt.Dimension(10, 10), "ColorChooser.swatchesRecentSwatchSize", New java.awt.Dimension(10, 10), "ColorChooser.swatchesDefaultRecentColor", control, "ComboBox.font", sansSerifPlain12, "ComboBox.background", window, "ComboBox.foreground", textText, "ComboBox.buttonBackground", control, "ComboBox.buttonShadow", controlShadow, "ComboBox.buttonDarkShadow", controlDkShadow, "ComboBox.buttonHighlight", controlLtHighlight, "ComboBox.selectionBackground", textHighlight, "ComboBox.selectionForeground", textHighlightText, "ComboBox.disabledBackground", control, "ComboBox.disabledForeground", textInactiveText, "ComboBox.timeFactor", oneThousand, "ComboBox.isEnterSelectablePopup", Boolean.FALSE, "ComboBox.ancestorInputMap", New javax.swing.UIDefaults.LazyInputMap(New Object() { "ESCAPE", "hidePopup", "PAGE_UP", "pageUpPassThrough", "PAGE_DOWN", "pageDownPassThrough", "HOME", "homePassThrough", "END", "endPassThrough", "ENTER", "enterPressed" }), "ComboBox.noActionOnKeyNavigation", Boolean.FALSE, "FileChooser.newFolderIcon", newFolderIcon, "FileChooser.upFolderIcon", upFolderIcon, "FileChooser.homeFolderIcon", homeFolderIcon, "FileChooser.detailsViewIcon", detailsViewIcon, "FileChooser.listViewIcon", listViewIcon, "FileChooser.readOnly", Boolean.FALSE, "FileChooser.usesSingleFilePane", Boolean.FALSE, "FileChooser.ancestorInputMap", New javax.swing.UIDefaults.LazyInputMap(New Object() { "ESCAPE", "cancelSelection", "F5", "refresh" }), "FileView.directoryIcon", directoryIcon, "FileView.fileIcon", fileIcon, "FileView.computerIcon", computerIcon, "FileView.hardDriveIcon", hardDriveIcon, "FileView.floppyDriveIcon", floppyDriveIcon, "InternalFrame.titleFont", dialogBold12, "InternalFrame.borderColor", control, "InternalFrame.borderShadow", controlShadow, "InternalFrame.borderDarkShadow", controlDkShadow, "InternalFrame.borderHighlight", controlLtHighlight, "InternalFrame.borderLight", controlHighlight, "InternalFrame.border", internalFrameBorder, "InternalFrame.icon", sun.swing.SwingUtilities2.makeIcon(Me.GetType(), GetType(BasicLookAndFeel), "icons/JavaCup16.png"), "InternalFrame.maximizeIcon", New sun.swing.SwingLazyValue("javax.swing.plaf.basic.BasicIconFactory", "createEmptyFrameIcon"), "InternalFrame.minimizeIcon", New sun.swing.SwingLazyValue("javax.swing.plaf.basic.BasicIconFactory", "createEmptyFrameIcon"), "InternalFrame.iconifyIcon", New sun.swing.SwingLazyValue("javax.swing.plaf.basic.BasicIconFactory", "createEmptyFrameIcon"), "InternalFrame.closeIcon", New sun.swing.SwingLazyValue("javax.swing.plaf.basic.BasicIconFactory", "createEmptyFrameIcon"), "InternalFrame.closeSound", Nothing, "InternalFrame.maximizeSound", Nothing, "InternalFrame.minimizeSound", Nothing, "InternalFrame.restoreDownSound", Nothing, "InternalFrame.restoreUpSound", Nothing, "InternalFrame.activeTitleBackground", table("activeCaption"), "InternalFrame.activeTitleForeground", table("activeCaptionText"), "InternalFrame.inactiveTitleBackground", table("inactiveCaption"), "InternalFrame.inactiveTitleForeground", table("inactiveCaptionText"), "InternalFrame.windowBindings", New Object() { "shift ESCAPE", "showSystemMenu", "ctrl SPACE", "showSystemMenu", "ESCAPE", "hideSystemMenu"}, "InternalFrameTitlePane.iconifyButtonOpacity", Boolean.TRUE, "InternalFrameTitlePane.maximizeButtonOpacity", Boolean.TRUE, "InternalFrameTitlePane.closeButtonOpacity", Boolean.TRUE, "DesktopIcon.border", internalFrameBorder, "Desktop.minOnScreenInsets", threeInsets, "Desktop.background", table("desktop"), "Desktop.ancestorInputMap", New javax.swing.UIDefaults.LazyInputMap(New Object() { "ctrl F5", "restore", "ctrl F4", "close", "ctrl F7", "move", "ctrl F8", "resize", "RIGHT", "right", "KP_RIGHT", "right", "shift RIGHT", "shrinkRight", "shift KP_RIGHT", "shrinkRight", "LEFT", "left", "KP_LEFT", "left", "shift LEFT", "shrinkLeft", "shift KP_LEFT", "shrinkLeft", "UP", "up", "KP_UP", "up", "shift UP", "shrinkUp", "shift KP_UP", "shrinkUp", "DOWN", "down", "KP_DOWN", "down", "shift DOWN", "shrinkDown", "shift KP_DOWN", "shrinkDown", "ESCAPE", "escape", "ctrl F9", "minimize", "ctrl F10", "maximize", "ctrl F6", "selectNextFrame", "ctrl TAB", "selectNextFrame", "ctrl alt F6", "selectNextFrame", "shift ctrl alt F6", "selectPreviousFrame", "ctrl F12", "navigateNext", "shift ctrl F12", "navigatePrevious" }), "Label.font", dialogPlain12, "Label.background", control, "Label.foreground", controlText, "Label.disabledForeground", white, "Label.disabledShadow", controlShadow, "Label.border", Nothing, "List.font", dialogPlain12, "List.background", window, "List.foreground", textText, "List.selectionBackground", textHighlight, "List.selectionForeground", textHighlightText, "List.noFocusBorder", noFocusBorder, "List.focusCellHighlightBorder", focusCellHighlightBorder, "List.dropLineColor", controlShadow, "List.border", Nothing, "List.cellRenderer", listCellRendererActiveValue, "List.timeFactor", oneThousand, "List.focusInputMap", New javax.swing.UIDefaults.LazyInputMap(New Object() { "ctrl C", "copy", "ctrl V", "paste", "ctrl X", "cut", "COPY", "copy", "PASTE", "paste", "CUT", "cut", "control INSERT", "copy", "shift INSERT", "paste", "shift DELETE", "cut", "UP", "selectPreviousRow", "KP_UP", "selectPreviousRow", "shift UP", "selectPreviousRowExtendSelection", "shift KP_UP", "selectPreviousRowExtendSelection", "ctrl shift UP", "selectPreviousRowExtendSelection", "ctrl shift KP_UP", "selectPreviousRowExtendSelection", "ctrl UP", "selectPreviousRowChangeLead", "ctrl KP_UP", "selectPreviousRowChangeLead", "DOWN", "selectNextRow", "KP_DOWN", "selectNextRow", "shift DOWN", "selectNextRowExtendSelection", "shift KP_DOWN", "selectNextRowExtendSelection", "ctrl shift DOWN", "selectNextRowExtendSelection", "ctrl shift KP_DOWN", "selectNextRowExtendSelection", "ctrl DOWN", "selectNextRowChangeLead", "ctrl KP_DOWN", "selectNextRowChangeLead", "LEFT", "selectPreviousColumn", "KP_LEFT", "selectPreviousColumn", "shift LEFT", "selectPreviousColumnExtendSelection", "shift KP_LEFT", "selectPreviousColumnExtendSelection", "ctrl shift LEFT", "selectPreviousColumnExtendSelection", "ctrl shift KP_LEFT", "selectPreviousColumnExtendSelection", "ctrl LEFT", "selectPreviousColumnChangeLead", "ctrl KP_LEFT", "selectPreviousColumnChangeLead", "RIGHT", "selectNextColumn", "KP_RIGHT", "selectNextColumn", "shift RIGHT", "selectNextColumnExtendSelection", "shift KP_RIGHT", "selectNextColumnExtendSelection", "ctrl shift RIGHT", "selectNextColumnExtendSelection", "ctrl shift KP_RIGHT", "selectNextColumnExtendSelection", "ctrl RIGHT", "selectNextColumnChangeLead", "ctrl KP_RIGHT", "selectNextColumnChangeLead", "HOME", "selectFirstRow", "shift HOME", "selectFirstRowExtendSelection", "ctrl shift HOME", "selectFirstRowExtendSelection", "ctrl HOME", "selectFirstRowChangeLead", "END", "selectLastRow", "shift END", "selectLastRowExtendSelection", "ctrl shift END", "selectLastRowExtendSelection", "ctrl END", "selectLastRowChangeLead", "PAGE_UP", "scrollUp", "shift PAGE_UP", "scrollUpExtendSelection", "ctrl shift PAGE_UP", "scrollUpExtendSelection", "ctrl PAGE_UP", "scrollUpChangeLead", "PAGE_DOWN", "scrollDown", "shift PAGE_DOWN", "scrollDownExtendSelection", "ctrl shift PAGE_DOWN", "scrollDownExtendSelection", "ctrl PAGE_DOWN", "scrollDownChangeLead", "ctrl A", "selectAll", "ctrl SLASH", "selectAll", "ctrl BACK_SLASH", "clearSelection", "SPACE", "addToSelection", "ctrl SPACE", "toggleAndAnchor", "shift SPACE", "extendTo", "ctrl shift SPACE", "moveSelectionTo" }), "List.focusInputMap.RightToLeft", New javax.swing.UIDefaults.LazyInputMap(New Object() { "LEFT", "selectNextColumn", "KP_LEFT", "selectNextColumn", "shift LEFT", "selectNextColumnExtendSelection", "shift KP_LEFT", "selectNextColumnExtendSelection", "ctrl shift LEFT", "selectNextColumnExtendSelection", "ctrl shift KP_LEFT", "selectNextColumnExtendSelection", "ctrl LEFT", "selectNextColumnChangeLead", "ctrl KP_LEFT", "selectNextColumnChangeLead", "RIGHT", "selectPreviousColumn", "KP_RIGHT", "selectPreviousColumn", "shift RIGHT", "selectPreviousColumnExtendSelection", "shift KP_RIGHT", "selectPreviousColumnExtendSelection", "ctrl shift RIGHT", "selectPreviousColumnExtendSelection", "ctrl shift KP_RIGHT", "selectPreviousColumnExtendSelection", "ctrl RIGHT", "selectPreviousColumnChangeLead", "ctrl KP_RIGHT", "selectPreviousColumnChangeLead" }), "MenuBar.font", dialogPlain12, "MenuBar.background", menu, "MenuBar.foreground", menuText, "MenuBar.shadow", controlShadow, "MenuBar.highlight", controlLtHighlight, "MenuBar.border", menuBarBorder, "MenuBar.windowBindings", New Object() { "F10", "takeFocus" }, "MenuItem.font", dialogPlain12, "MenuItem.acceleratorFont", dialogPlain12, "MenuItem.background", menu, "MenuItem.foreground", menuText, "MenuItem.selectionForeground", textHighlightText, "MenuItem.selectionBackground", textHighlight, "MenuItem.disabledForeground", Nothing, "MenuItem.acceleratorForeground", menuText, "MenuItem.acceleratorSelectionForeground", textHighlightText, "MenuItem.acceleratorDelimiter", menuItemAcceleratorDelimiter, "MenuItem.border", marginBorder, "MenuItem.borderPainted", Boolean.FALSE, "MenuItem.margin", twoInsets, "MenuItem.checkIcon", menuItemCheckIcon, "MenuItem.arrowIcon", menuItemArrowIcon, "MenuItem.commandSound", Nothing, "RadioButtonMenuItem.font", dialogPlain12, "RadioButtonMenuItem.acceleratorFont", dialogPlain12, "RadioButtonMenuItem.background", menu, "RadioButtonMenuItem.foreground", menuText, "RadioButtonMenuItem.selectionForeground", textHighlightText, "RadioButtonMenuItem.selectionBackground", textHighlight, "RadioButtonMenuItem.disabledForeground", Nothing, "RadioButtonMenuItem.acceleratorForeground", menuText, "RadioButtonMenuItem.acceleratorSelectionForeground", textHighlightText, "RadioButtonMenuItem.border", marginBorder, "RadioButtonMenuItem.borderPainted", Boolean.FALSE, "RadioButtonMenuItem.margin", twoInsets, "RadioButtonMenuItem.checkIcon", radioButtonMenuItemIcon, "RadioButtonMenuItem.arrowIcon", menuItemArrowIcon, "RadioButtonMenuItem.commandSound", Nothing, "CheckBoxMenuItem.font", dialogPlain12, "CheckBoxMenuItem.acceleratorFont", dialogPlain12, "CheckBoxMenuItem.background", menu, "CheckBoxMenuItem.foreground", menuText, "CheckBoxMenuItem.selectionForeground", textHighlightText, "CheckBoxMenuItem.selectionBackground", textHighlight, "CheckBoxMenuItem.disabledForeground", Nothing, "CheckBoxMenuItem.acceleratorForeground", menuText, "CheckBoxMenuItem.acceleratorSelectionForeground", textHighlightText, "CheckBoxMenuItem.border", marginBorder, "CheckBoxMenuItem.borderPainted", Boolean.FALSE, "CheckBoxMenuItem.margin", twoInsets, "CheckBoxMenuItem.checkIcon", checkBoxMenuItemIcon, "CheckBoxMenuItem.arrowIcon", menuItemArrowIcon, "CheckBoxMenuItem.commandSound", Nothing, "Menu.font", dialogPlain12, "Menu.acceleratorFont", dialogPlain12, "Menu.background", menu, "Menu.foreground", menuText, "Menu.selectionForeground", textHighlightText, "Menu.selectionBackground", textHighlight, "Menu.disabledForeground", Nothing, "Menu.acceleratorForeground", menuText, "Menu.acceleratorSelectionForeground", textHighlightText, "Menu.border", marginBorder, "Menu.borderPainted", Boolean.FALSE, "Menu.margin", twoInsets, "Menu.checkIcon", menuItemCheckIcon, "Menu.arrowIcon", menuArrowIcon, "Menu.menuPopupOffsetX", New Integer?(0), "Menu.menuPopupOffsetY", New Integer?(0), "Menu.submenuPopupOffsetX", New Integer?(0), "Menu.submenuPopupOffsetY", New Integer?(0), "Menu.shortcutKeys", New Integer(){ sun.swing.SwingUtilities2.systemMnemonicKeyMask }, "Menu.crossMenuMnemonic", Boolean.TRUE, "Menu.cancelMode", "hideLastSubmenu", "Menu.preserveTopLevelSelection", Boolean.FALSE, "PopupMenu.font", dialogPlain12, "PopupMenu.background", menu, "PopupMenu.foreground", menuText, "PopupMenu.border", popupMenuBorder, "PopupMenu.popupSound", Nothing, "PopupMenu.selectedWindowInputMapBindings", New Object() { "ESCAPE", "cancel", "DOWN", "selectNext", "KP_DOWN", "selectNext", "UP", "selectPrevious", "KP_UP", "selectPrevious", "LEFT", "selectParent", "KP_LEFT", "selectParent", "RIGHT", "selectChild", "KP_RIGHT", "selectChild", "ENTER", "return", "ctrl ENTER", "return", "SPACE", "return" }, "PopupMenu.selectedWindowInputMapBindings.RightToLeft", New Object() { "LEFT", "selectChild", "KP_LEFT", "selectChild", "RIGHT", "selectParent", "KP_RIGHT", "selectParent" }, "PopupMenu.consumeEventOnClose", Boolean.FALSE, "OptionPane.font", dialogPlain12, "OptionPane.background", control, "OptionPane.foreground", controlText, "OptionPane.messageForeground", controlText, "OptionPane.border", optionPaneBorder, "OptionPane.messageAreaBorder", zeroBorder, "OptionPane.buttonAreaBorder", optionPaneButtonAreaBorder, "OptionPane.minimumSize", optionPaneMinimumSize, "OptionPane.errorIcon", sun.swing.SwingUtilities2.makeIcon(Me.GetType(), GetType(BasicLookAndFeel), "icons/Error.gif"), "OptionPane.informationIcon", sun.swing.SwingUtilities2.makeIcon(Me.GetType(), GetType(BasicLookAndFeel), "icons/Inform.gif"), "OptionPane.warningIcon", sun.swing.SwingUtilities2.makeIcon(Me.GetType(), GetType(BasicLookAndFeel), "icons/Warn.gif"), "OptionPane.questionIcon", sun.swing.SwingUtilities2.makeIcon(Me.GetType(), GetType(BasicLookAndFeel), "icons/Question.gif"), "OptionPane.windowBindings", New Object() { "ESCAPE", "close" }, "OptionPane.errorSound", Nothing, "OptionPane.informationSound", Nothing, "OptionPane.questionSound", Nothing, "OptionPane.warningSound", Nothing, "OptionPane.buttonClickThreshhold", fiveHundred, "Panel.font", dialogPlain12, "Panel.background", control, "Panel.foreground", textText, "ProgressBar.font", dialogPlain12, "ProgressBar.foreground", textHighlight, "ProgressBar.background", control, "ProgressBar.selectionForeground", control, "ProgressBar.selectionBackground", textHighlight, "ProgressBar.border", progressBarBorder, "ProgressBar.cellLength", New Integer?(1), "ProgressBar.cellSpacing", zero, "ProgressBar.repaintInterval", New Integer?(50), "ProgressBar.cycleTime", New Integer?(3000), "ProgressBar.horizontalSize", New DimensionUIResource(146, 12), "ProgressBar.verticalSize", New DimensionUIResource(12, 146), "Separator.shadow", controlShadow, "Separator.highlight", controlLtHighlight, "Separator.background", controlLtHighlight, "Separator.foreground", controlShadow, "ScrollBar.background", scrollBarTrack, "ScrollBar.foreground", control, "ScrollBar.track", table("scrollbar"), "ScrollBar.trackHighlight", controlDkShadow, "ScrollBar.thumb", control, "ScrollBar.thumbHighlight", controlLtHighlight, "ScrollBar.thumbDarkShadow", controlDkShadow, "ScrollBar.thumbShadow", controlShadow, "ScrollBar.border", Nothing, "ScrollBar.minimumThumbSize", minimumThumbSize, "ScrollBar.maximumThumbSize", maximumThumbSize, "ScrollBar.ancestorInputMap", New javax.swing.UIDefaults.LazyInputMap(New Object() { "RIGHT", "positiveUnitIncrement", "KP_RIGHT", "positiveUnitIncrement", "DOWN", "positiveUnitIncrement", "KP_DOWN", "positiveUnitIncrement", "PAGE_DOWN", "positiveBlockIncrement", "LEFT", "negativeUnitIncrement", "KP_LEFT", "negativeUnitIncrement", "UP", "negativeUnitIncrement", "KP_UP", "negativeUnitIncrement", "PAGE_UP", "negativeBlockIncrement", "HOME", "minScroll", "END", "maxScroll" }), "ScrollBar.ancestorInputMap.RightToLeft", New javax.swing.UIDefaults.LazyInputMap(New Object() { "RIGHT", "negativeUnitIncrement", "KP_RIGHT", "negativeUnitIncrement", "LEFT", "positiveUnitIncrement", "KP_LEFT", "positiveUnitIncrement" }), "ScrollBar.width", New Integer?(16), "ScrollPane.font", dialogPlain12, "ScrollPane.background", control, "ScrollPane.foreground", controlText, "ScrollPane.border", textFieldBorder, "ScrollPane.viewportBorder", Nothing, "ScrollPane.ancestorInputMap", New javax.swing.UIDefaults.LazyInputMap(New Object() { "RIGHT", "unitScrollRight", "KP_RIGHT", "unitScrollRight", "DOWN", "unitScrollDown", "KP_DOWN", "unitScrollDown", "LEFT", "unitScrollLeft", "KP_LEFT", "unitScrollLeft", "UP", "unitScrollUp", "KP_UP", "unitScrollUp", "PAGE_UP", "scrollUp", "PAGE_DOWN", "scrollDown", "ctrl PAGE_UP", "scrollLeft", "ctrl PAGE_DOWN", "scrollRight", "ctrl HOME", "scrollHome", "ctrl END", "scrollEnd" }), "ScrollPane.ancestorInputMap.RightToLeft", New javax.swing.UIDefaults.LazyInputMap(New Object() { "ctrl PAGE_UP", "scrollRight", "ctrl PAGE_DOWN", "scrollLeft" }), "Viewport.font", dialogPlain12, "Viewport.background", control, "Viewport.foreground", textText, "Slider.font", dialogPlain12, "Slider.foreground", control, "Slider.background", control, "Slider.highlight", controlLtHighlight, "Slider.tickColor", java.awt.Color.black, "Slider.shadow", controlShadow, "Slider.focus", controlDkShadow, "Slider.border", Nothing, "Slider.horizontalSize", New java.awt.Dimension(200, 21), "Slider.verticalSize", New java.awt.Dimension(21, 200), "Slider.minimumHorizontalSize", New java.awt.Dimension(36, 21), "Slider.minimumVerticalSize", New java.awt.Dimension(21, 36), "Slider.focusInsets", sliderFocusInsets, "Slider.focusInputMap", New javax.swing.UIDefaults.LazyInputMap(New Object() { "RIGHT", "positiveUnitIncrement", "KP_RIGHT", "positiveUnitIncrement", "DOWN", "negativeUnitIncrement", "KP_DOWN", "negativeUnitIncrement", "PAGE_DOWN", "negativeBlockIncrement", "LEFT", "negativeUnitIncrement", "KP_LEFT", "negativeUnitIncrement", "UP", "positiveUnitIncrement", "KP_UP", "positiveUnitIncrement", "PAGE_UP", "positiveBlockIncrement", "HOME", "minScroll", "END", "maxScroll" }), "Slider.focusInputMap.RightToLeft", New javax.swing.UIDefaults.LazyInputMap(New Object() { "RIGHT", "negativeUnitIncrement", "KP_RIGHT", "negativeUnitIncrement", "LEFT", "positiveUnitIncrement", "KP_LEFT", "positiveUnitIncrement" }), "Slider.onlyLeftMouseButtonDrag", Boolean.TRUE, "Spinner.font", monospacedPlain12, "Spinner.background", control, "Spinner.foreground", control, "Spinner.border", textFieldBorder, "Spinner.arrowButtonBorder", Nothing, "Spinner.arrowButtonInsets", Nothing, "Spinner.arrowButtonSize", New java.awt.Dimension(16, 5), "Spinner.ancestorInputMap", New javax.swing.UIDefaults.LazyInputMap(New Object() { "UP", "increment", "KP_UP", "increment", "DOWN", "decrement", "KP_DOWN", "decrement" }), "Spinner.editorBorderPainted", Boolean.FALSE, "Spinner.editorAlignment", javax.swing.JTextField.TRAILING, "SplitPane.background", control, "SplitPane.highlight", controlLtHighlight, "SplitPane.shadow", controlShadow, "SplitPane.darkShadow", controlDkShadow, "SplitPane.border", splitPaneBorder, "SplitPane.dividerSize", New Integer?(7), "SplitPaneDivider.border", splitPaneDividerBorder, "SplitPaneDivider.draggingColor", darkGray, "SplitPane.ancestorInputMap", New javax.swing.UIDefaults.LazyInputMap(New Object() { "UP", "negativeIncrement", "DOWN", "positiveIncrement", "LEFT", "negativeIncrement", "RIGHT", "positiveIncrement", "KP_UP", "negativeIncrement", "KP_DOWN", "positiveIncrement", "KP_LEFT", "negativeIncrement", "KP_RIGHT", "positiveIncrement", "HOME", "selectMin", "END", "selectMax", "F8", "startResize", "F6", "toggleFocus", "ctrl TAB", "focusOutForward", "ctrl shift TAB", "focusOutBackward" }), "TabbedPane.font", dialogPlain12, "TabbedPane.background", control, "TabbedPane.foreground", controlText, "TabbedPane.highlight", controlLtHighlight, "TabbedPane.light", controlHighlight, "TabbedPane.shadow", controlShadow, "TabbedPane.darkShadow", controlDkShadow, "TabbedPane.selected", Nothing, "TabbedPane.focus", controlText, "TabbedPane.textIconGap", four, "TabbedPane.tabsOverlapBorder", Boolean.FALSE, "TabbedPane.selectionFollowsFocus", Boolean.TRUE, "TabbedPane.labelShift", 1, "TabbedPane.selectedLabelShift", -1, "TabbedPane.tabInsets", tabbedPaneTabInsets, "TabbedPane.selectedTabPadInsets", tabbedPaneTabPadInsets, "TabbedPane.tabAreaInsets", tabbedPaneTabAreaInsets, "TabbedPane.contentBorderInsets", tabbedPaneContentBorderInsets, "TabbedPane.tabRunOverlay", New Integer?(2), "TabbedPane.tabsOpaque", Boolean.TRUE, "TabbedPane.contentOpaque", Boolean.TRUE, "TabbedPane.focusInputMap", New javax.swing.UIDefaults.LazyInputMap(New Object() { "RIGHT", "navigateRight", "KP_RIGHT", "navigateRight", "LEFT", "navigateLeft", "KP_LEFT", "navigateLeft", "UP", "navigateUp", "KP_UP", "navigateUp", "DOWN", "navigateDown", "KP_DOWN", "navigateDown", "ctrl DOWN", "requestFocusForVisibleComponent", "ctrl KP_DOWN", "requestFocusForVisibleComponent" }), "TabbedPane.ancestorInputMap", New javax.swing.UIDefaults.LazyInputMap(New Object() { "ctrl PAGE_DOWN", "navigatePageDown", "ctrl PAGE_UP", "navigatePageUp", "ctrl UP", "requestFocus", "ctrl KP_UP", "requestFocus" }), "Table.font", dialogPlain12, "Table.foreground", controlText, "Table.background", window, "Table.selectionForeground", textHighlightText, "Table.selectionBackground", textHighlight, "Table.dropLineColor", controlShadow, "Table.dropLineShortColor", black, "Table.gridColor", gray, "Table.focusCellBackground", window, "Table.focusCellForeground", controlText, "Table.focusCellHighlightBorder", focusCellHighlightBorder, "Table.scrollPaneBorder", loweredBevelBorder, "Table.ancestorInputMap", New javax.swing.UIDefaults.LazyInputMap(New Object() { "ctrl C", "copy", "ctrl V", "paste", "ctrl X", "cut", "COPY", "copy", "PASTE", "paste", "CUT", "cut", "control INSERT", "copy", "shift INSERT", "paste", "shift DELETE", "cut", "RIGHT", "selectNextColumn", "KP_RIGHT", "selectNextColumn", "shift RIGHT", "selectNextColumnExtendSelection", "shift KP_RIGHT", "selectNextColumnExtendSelection", "ctrl shift RIGHT", "selectNextColumnExtendSelection", "ctrl shift KP_RIGHT", "selectNextColumnExtendSelection", "ctrl RIGHT", "selectNextColumnChangeLead", "ctrl KP_RIGHT", "selectNextColumnChangeLead", "LEFT", "selectPreviousColumn", "KP_LEFT", "selectPreviousColumn", "shift LEFT", "selectPreviousColumnExtendSelection", "shift KP_LEFT", "selectPreviousColumnExtendSelection", "ctrl shift LEFT", "selectPreviousColumnExtendSelection", "ctrl shift KP_LEFT", "selectPreviousColumnExtendSelection", "ctrl LEFT", "selectPreviousColumnChangeLead", "ctrl KP_LEFT", "selectPreviousColumnChangeLead", "DOWN", "selectNextRow", "KP_DOWN", "selectNextRow", "shift DOWN", "selectNextRowExtendSelection", "shift KP_DOWN", "selectNextRowExtendSelection", "ctrl shift DOWN", "selectNextRowExtendSelection", "ctrl shift KP_DOWN", "selectNextRowExtendSelection", "ctrl DOWN", "selectNextRowChangeLead", "ctrl KP_DOWN", "selectNextRowChangeLead", "UP", "selectPreviousRow", "KP_UP", "selectPreviousRow", "shift UP", "selectPreviousRowExtendSelection", "shift KP_UP", "selectPreviousRowExtendSelection", "ctrl shift UP", "selectPreviousRowExtendSelection", "ctrl shift KP_UP", "selectPreviousRowExtendSelection", "ctrl UP", "selectPreviousRowChangeLead", "ctrl KP_UP", "selectPreviousRowChangeLead", "HOME", "selectFirstColumn", "shift HOME", "selectFirstColumnExtendSelection", "ctrl shift HOME", "selectFirstRowExtendSelection", "ctrl HOME", "selectFirstRow", "END", "selectLastColumn", "shift END", "selectLastColumnExtendSelection", "ctrl shift END", "selectLastRowExtendSelection", "ctrl END", "selectLastRow", "PAGE_UP", "scrollUpChangeSelection", "shift PAGE_UP", "scrollUpExtendSelection", "ctrl shift PAGE_UP", "scrollLeftExtendSelection", "ctrl PAGE_UP", "scrollLeftChangeSelection", "PAGE_DOWN", "scrollDownChangeSelection", "shift PAGE_DOWN", "scrollDownExtendSelection", "ctrl shift PAGE_DOWN", "scrollRightExtendSelection", "ctrl PAGE_DOWN", "scrollRightChangeSelection", "TAB", "selectNextColumnCell", "shift TAB", "selectPreviousColumnCell", "ENTER", "selectNextRowCell", "shift ENTER", "selectPreviousRowCell", "ctrl A", "selectAll", "ctrl SLASH", "selectAll", "ctrl BACK_SLASH", "clearSelection", "ESCAPE", "cancel", "F2", "startEditing", "SPACE", "addToSelection", "ctrl SPACE", "toggleAndAnchor", "shift SPACE", "extendTo", "ctrl shift SPACE", "moveSelectionTo", "F8", "focusHeader" }), "Table.ancestorInputMap.RightToLeft", New javax.swing.UIDefaults.LazyInputMap(New Object() { "RIGHT", "selectPreviousColumn", "KP_RIGHT", "selectPreviousColumn", "shift RIGHT", "selectPreviousColumnExtendSelection", "shift KP_RIGHT", "selectPreviousColumnExtendSelection", "ctrl shift RIGHT", "selectPreviousColumnExtendSelection", "ctrl shift KP_RIGHT", "selectPreviousColumnExtendSelection", "ctrl RIGHT", "selectPreviousColumnChangeLead", "ctrl KP_RIGHT", "selectPreviousColumnChangeLead", "LEFT", "selectNextColumn", "KP_LEFT", "selectNextColumn", "shift LEFT", "selectNextColumnExtendSelection", "shift KP_LEFT", "selectNextColumnExtendSelection", "ctrl shift LEFT", "selectNextColumnExtendSelection", "ctrl shift KP_LEFT", "selectNextColumnExtendSelection", "ctrl LEFT", "selectNextColumnChangeLead", "ctrl KP_LEFT", "selectNextColumnChangeLead", "ctrl PAGE_UP", "scrollRightChangeSelection", "ctrl PAGE_DOWN", "scrollLeftChangeSelection", "ctrl shift PAGE_UP", "scrollRightExtendSelection", "ctrl shift PAGE_DOWN", "scrollLeftExtendSelection" }), "Table.ascendingSortIcon", New sun.swing.SwingLazyValue("sun.swing.icon.SortArrowIcon", Nothing, New Object() { Boolean.TRUE, "Table.sortIconColor" }), "Table.descendingSortIcon", New sun.swing.SwingLazyValue("sun.swing.icon.SortArrowIcon", Nothing, New Object() { Boolean.FALSE, "Table.sortIconColor" }), "Table.sortIconColor", controlShadow, "TableHeader.font", dialogPlain12, "TableHeader.foreground", controlText, "TableHeader.background", control, "TableHeader.cellBorder", tableHeaderBorder, "TableHeader.focusCellBackground", table.getColor("text"), "TableHeader.focusCellForeground", Nothing, "TableHeader.focusCellBorder", Nothing, "TableHeader.ancestorInputMap", New javax.swing.UIDefaults.LazyInputMap(New Object() { "SPACE", "toggleSortOrder", "LEFT", "selectColumnToLeft", "KP_LEFT", "selectColumnToLeft", "RIGHT", "selectColumnToRight", "KP_RIGHT", "selectColumnToRight", "alt LEFT", "moveColumnLeft", "alt KP_LEFT", "moveColumnLeft", "alt RIGHT", "moveColumnRight", "alt KP_RIGHT", "moveColumnRight", "alt shift LEFT", "resizeLeft", "alt shift KP_LEFT", "resizeLeft", "alt shift RIGHT", "resizeRight", "alt shift KP_RIGHT", "resizeRight", "ESCAPE", "focusTable" }), "TextField.font", sansSerifPlain12, "TextField.background", window, "TextField.foreground", textText, "TextField.shadow", controlShadow, "TextField.darkShadow", controlDkShadow, "TextField.light", controlHighlight, "TextField.highlight", controlLtHighlight, "TextField.inactiveForeground", textInactiveText, "TextField.inactiveBackground", control, "TextField.selectionBackground", textHighlight, "TextField.selectionForeground", textHighlightText, "TextField.caretForeground", textText, "TextField.caretBlinkRate", caretBlinkRate, "TextField.border", textFieldBorder, "TextField.margin", zeroInsets, "FormattedTextField.font", sansSerifPlain12, "FormattedTextField.background", window, "FormattedTextField.foreground", textText, "FormattedTextField.inactiveForeground", textInactiveText, "FormattedTextField.inactiveBackground", control, "FormattedTextField.selectionBackground", textHighlight, "FormattedTextField.selectionForeground", textHighlightText, "FormattedTextField.caretForeground", textText, "FormattedTextField.caretBlinkRate", caretBlinkRate, "FormattedTextField.border", textFieldBorder, "FormattedTextField.margin", zeroInsets, "FormattedTextField.focusInputMap", New javax.swing.UIDefaults.LazyInputMap(New Object() { "ctrl C", javax.swing.text.DefaultEditorKit.copyAction, "ctrl V", javax.swing.text.DefaultEditorKit.pasteAction, "ctrl X", javax.swing.text.DefaultEditorKit.cutAction, "COPY", javax.swing.text.DefaultEditorKit.copyAction, "PASTE", javax.swing.text.DefaultEditorKit.pasteAction, "CUT", javax.swing.text.DefaultEditorKit.cutAction, "control INSERT", javax.swing.text.DefaultEditorKit.copyAction, "shift INSERT", javax.swing.text.DefaultEditorKit.pasteAction, "shift DELETE", javax.swing.text.DefaultEditorKit.cutAction, "shift LEFT", javax.swing.text.DefaultEditorKit.selectionBackwardAction, "shift KP_LEFT", javax.swing.text.DefaultEditorKit.selectionBackwardAction, "shift RIGHT", javax.swing.text.DefaultEditorKit.selectionForwardAction, "shift KP_RIGHT", javax.swing.text.DefaultEditorKit.selectionForwardAction, "ctrl LEFT", javax.swing.text.DefaultEditorKit.previousWordAction, "ctrl KP_LEFT", javax.swing.text.DefaultEditorKit.previousWordAction, "ctrl RIGHT", javax.swing.text.DefaultEditorKit.nextWordAction, "ctrl KP_RIGHT", javax.swing.text.DefaultEditorKit.nextWordAction, "ctrl shift LEFT", javax.swing.text.DefaultEditorKit.selectionPreviousWordAction, "ctrl shift KP_LEFT", javax.swing.text.DefaultEditorKit.selectionPreviousWordAction, "ctrl shift RIGHT", javax.swing.text.DefaultEditorKit.selectionNextWordAction, "ctrl shift KP_RIGHT", javax.swing.text.DefaultEditorKit.selectionNextWordAction, "ctrl A", javax.swing.text.DefaultEditorKit.selectAllAction, "HOME", javax.swing.text.DefaultEditorKit.beginLineAction, "END", javax.swing.text.DefaultEditorKit.endLineAction, "shift HOME", javax.swing.text.DefaultEditorKit.selectionBeginLineAction, "shift END", javax.swing.text.DefaultEditorKit.selectionEndLineAction, "BACK_SPACE", javax.swing.text.DefaultEditorKit.deletePrevCharAction, "shift BACK_SPACE", javax.swing.text.DefaultEditorKit.deletePrevCharAction, "ctrl H", javax.swing.text.DefaultEditorKit.deletePrevCharAction, "DELETE", javax.swing.text.DefaultEditorKit.deleteNextCharAction, "ctrl DELETE", javax.swing.text.DefaultEditorKit.deleteNextWordAction, "ctrl BACK_SPACE", javax.swing.text.DefaultEditorKit.deletePrevWordAction, "RIGHT", javax.swing.text.DefaultEditorKit.forwardAction, "LEFT", javax.swing.text.DefaultEditorKit.backwardAction, "KP_RIGHT", javax.swing.text.DefaultEditorKit.forwardAction, "KP_LEFT", javax.swing.text.DefaultEditorKit.backwardAction, "ENTER", javax.swing.JTextField.notifyAction, "ctrl BACK_SLASH", "unselect", "control shift O", "toggle-componentOrientation", "ESCAPE", "reset-field-edit", "UP", "increment", "KP_UP", "increment", "DOWN", "decrement", "KP_DOWN", "decrement" }), "PasswordField.font", monospacedPlain12, "PasswordField.background", window, "PasswordField.foreground", textText, "PasswordField.inactiveForeground", textInactiveText, "PasswordField.inactiveBackground", control, "PasswordField.selectionBackground", textHighlight, "PasswordField.selectionForeground", textHighlightText, "PasswordField.caretForeground", textText, "PasswordField.caretBlinkRate", caretBlinkRate, "PasswordField.border", textFieldBorder, "PasswordField.margin", zeroInsets, "PasswordField.echoChar", "*"c, "TextArea.font", monospacedPlain12, "TextArea.background", window, "TextArea.foreground", textText, "TextArea.inactiveForeground", textInactiveText, "TextArea.selectionBackground", textHighlight, "TextArea.selectionForeground", textHighlightText, "TextArea.caretForeground", textText, "TextArea.caretBlinkRate", caretBlinkRate, "TextArea.border", marginBorder, "TextArea.margin", zeroInsets, "TextPane.font", serifPlain12, "TextPane.background", white, "TextPane.foreground", textText, "TextPane.selectionBackground", textHighlight, "TextPane.selectionForeground", textHighlightText, "TextPane.caretForeground", textText, "TextPane.caretBlinkRate", caretBlinkRate, "TextPane.inactiveForeground", textInactiveText, "TextPane.border", marginBorder, "TextPane.margin", editorMargin, "EditorPane.font", serifPlain12, "EditorPane.background", white, "EditorPane.foreground", textText, "EditorPane.selectionBackground", textHighlight, "EditorPane.selectionForeground", textHighlightText, "EditorPane.caretForeground", textText, "EditorPane.caretBlinkRate", caretBlinkRate, "EditorPane.inactiveForeground", textInactiveText, "EditorPane.border", marginBorder, "EditorPane.margin", editorMargin, "html.pendingImage", sun.swing.SwingUtilities2.makeIcon(Me.GetType(), GetType(BasicLookAndFeel), "icons/image-delayed.png"), "html.missingImage", sun.swing.SwingUtilities2.makeIcon(Me.GetType(), GetType(BasicLookAndFeel), "icons/image-failed.png"), "TitledBorder.font", dialogPlain12, "TitledBorder.titleColor", controlText, "TitledBorder.border", ___etchedBorder, "ToolBar.font", dialogPlain12, "ToolBar.background", control, "ToolBar.foreground", controlText, "ToolBar.shadow", controlShadow, "ToolBar.darkShadow", controlDkShadow, "ToolBar.light", controlHighlight, "ToolBar.highlight", controlLtHighlight, "ToolBar.dockingBackground", control, "ToolBar.dockingForeground", red, "ToolBar.floatingBackground", control, "ToolBar.floatingForeground", darkGray, "ToolBar.border", ___etchedBorder, "ToolBar.separatorSize", toolBarSeparatorSize, "ToolBar.ancestorInputMap", New javax.swing.UIDefaults.LazyInputMap(New Object() { "UP", "navigateUp", "KP_UP", "navigateUp", "DOWN", "navigateDown", "KP_DOWN", "navigateDown", "LEFT", "navigateLeft", "KP_LEFT", "navigateLeft", "RIGHT", "navigateRight", "KP_RIGHT", "navigateRight" }), "ToolTip.font", sansSerifPlain12, "ToolTip.background", table("info"), "ToolTip.foreground", table("infoText"), "ToolTip.border", blackLineBorder, "ToolTipManager.enableToolTipMode", "allWindows", "Tree.paintLines", Boolean.TRUE, "Tree.lineTypeDashed", Boolean.FALSE, "Tree.font", dialogPlain12, "Tree.background", window, "Tree.foreground", textText, "Tree.hash", gray, "Tree.textForeground", textText, "Tree.textBackground", table("text"), "Tree.selectionForeground", textHighlightText, "Tree.selectionBackground", textHighlight, "Tree.selectionBorderColor", black, "Tree.dropLineColor", controlShadow, "Tree.editorBorder", blackLineBorder, "Tree.leftChildIndent", New Integer?(7), "Tree.rightChildIndent", New Integer?(13), "Tree.rowHeight", New Integer?(16), "Tree.scrollsOnExpand", Boolean.TRUE, "Tree.openIcon", sun.swing.SwingUtilities2.makeIcon(Me.GetType(), GetType(BasicLookAndFeel), "icons/TreeOpen.gif"), "Tree.closedIcon", sun.swing.SwingUtilities2.makeIcon(Me.GetType(), GetType(BasicLookAndFeel), "icons/TreeClosed.gif"), "Tree.leafIcon", sun.swing.SwingUtilities2.makeIcon(Me.GetType(), GetType(BasicLookAndFeel), "icons/TreeLeaf.gif"), "Tree.expandedIcon", Nothing, "Tree.collapsedIcon", Nothing, "Tree.changeSelectionWithFocus", Boolean.TRUE, "Tree.drawsFocusBorderAroundIcon", Boolean.FALSE, "Tree.timeFactor", oneThousand, "Tree.focusInputMap", New javax.swing.UIDefaults.LazyInputMap(New Object() { "ctrl C", "copy", "ctrl V", "paste", "ctrl X", "cut", "COPY", "copy", "PASTE", "paste", "CUT", "cut", "control INSERT", "copy", "shift INSERT", "paste", "shift DELETE", "cut", "UP", "selectPrevious", "KP_UP", "selectPrevious", "shift UP", "selectPreviousExtendSelection", "shift KP_UP", "selectPreviousExtendSelection", "ctrl shift UP", "selectPreviousExtendSelection", "ctrl shift KP_UP", "selectPreviousExtendSelection", "ctrl UP", "selectPreviousChangeLead", "ctrl KP_UP", "selectPreviousChangeLead", "DOWN", "selectNext", "KP_DOWN", "selectNext", "shift DOWN", "selectNextExtendSelection", "shift KP_DOWN", "selectNextExtendSelection", "ctrl shift DOWN", "selectNextExtendSelection", "ctrl shift KP_DOWN", "selectNextExtendSelection", "ctrl DOWN", "selectNextChangeLead", "ctrl KP_DOWN", "selectNextChangeLead", "RIGHT", "selectChild", "KP_RIGHT", "selectChild", "LEFT", "selectParent", "KP_LEFT", "selectParent", "PAGE_UP", "scrollUpChangeSelection", "shift PAGE_UP", "scrollUpExtendSelection", "ctrl shift PAGE_UP", "scrollUpExtendSelection", "ctrl PAGE_UP", "scrollUpChangeLead", "PAGE_DOWN", "scrollDownChangeSelection", "shift PAGE_DOWN", "scrollDownExtendSelection", "ctrl shift PAGE_DOWN", "scrollDownExtendSelection", "ctrl PAGE_DOWN", "scrollDownChangeLead", "HOME", "selectFirst", "shift HOME", "selectFirstExtendSelection", "ctrl shift HOME", "selectFirstExtendSelection", "ctrl HOME", "selectFirstChangeLead", "END", "selectLast", "shift END", "selectLastExtendSelection", "ctrl shift END", "selectLastExtendSelection", "ctrl END", "selectLastChangeLead", "F2", "startEditing", "ctrl A", "selectAll", "ctrl SLASH", "selectAll", "ctrl BACK_SLASH", "clearSelection", "ctrl LEFT", "scrollLeft", "ctrl KP_LEFT", "scrollLeft", "ctrl RIGHT", "scrollRight", "ctrl KP_RIGHT", "scrollRight", "SPACE", "addToSelection", "ctrl SPACE", "toggleAndAnchor", "shift SPACE", "extendTo", "ctrl shift SPACE", "moveSelectionTo" }), "Tree.focusInputMap.RightToLeft", New javax.swing.UIDefaults.LazyInputMap(New Object() { "RIGHT", "selectParent", "KP_RIGHT", "selectParent", "LEFT", "selectChild", "KP_LEFT", "selectChild" }), "Tree.ancestorInputMap", New javax.swing.UIDefaults.LazyInputMap(New Object() { "ESCAPE", "cancel" }), "RootPane.ancestorInputMap", New javax.swing.UIDefaults.LazyInputMap(New Object() { "shift F10", "postPopup", "CONTEXT_MENU", "postPopup" }), "RootPane.defaultButtonWindowKeyBindings", New Object() { "ENTER", "press", "released ENTER", "release", "ctrl ENTER", "press", "ctrl released ENTER", "release" } }

			table.putDefaults(___defaults)
		End Sub

		Private Class ActiveValueAnonymousInnerClassHelper
			Implements ActiveValue

			Public Overridable Function createValue(ByVal table As javax.swing.UIDefaults) As Object
				Return New javax.swing.DefaultListCellRenderer.UIResource
			End Function
		End Class

		Friend Property Shared focusAcceleratorKeyMask As Integer
			Get
				Dim tk As java.awt.Toolkit = java.awt.Toolkit.defaultToolkit
				If TypeOf tk Is sun.awt.SunToolkit Then Return CType(tk, sun.awt.SunToolkit).focusAcceleratorKeyMask
				Return ActionEvent.ALT_MASK
			End Get
		End Property



		''' <summary>
		''' Returns the ui that is of type <code>klass</code>, or null if
		''' one can not be found.
		''' </summary>
		Friend Shared Function getUIOfType(ByVal ui As ComponentUI, ByVal klass As Type) As Object
			If klass.IsInstanceOfType(ui) Then Return ui
			Return Nothing
		End Function

		' ********* Auditory Cue support methods and objects *********
		' also see the "AuditoryCues" section of the defaults table

		''' <summary>
		''' Returns an <code>ActionMap</code> containing the audio actions
		''' for this look and feel.
		''' <P>
		''' The returned <code>ActionMap</code> contains <code>Actions</code> that
		''' embody the ability to render an auditory cue. These auditory
		''' cues map onto user and system activities that may be useful
		''' for an end user to know about (such as a dialog box appearing).
		''' <P>
		''' At the appropriate time,
		''' the {@code ComponentUI} is responsible for obtaining an
		''' <code>Action</code> out of the <code>ActionMap</code> and passing
		''' it to <code>playSound</code>.
		''' <P>
		''' This method first looks up the {@code ActionMap} from the
		''' defaults using the key {@code "AuditoryCues.actionMap"}.
		''' <p>
		''' If the value is {@code non-null}, it is returned. If the value
		''' of the default {@code "AuditoryCues.actionMap"} is {@code null}
		''' and the value of the default {@code "AuditoryCues.cueList"} is
		''' {@code non-null}, an {@code ActionMapUIResource} is created and
		''' populated. Population is done by iterating over each of the
		''' elements of the {@code "AuditoryCues.cueList"} array, and
		''' invoking {@code createAudioAction()} to create an {@code
		''' Action} for each element.  The resulting {@code Action} is
		''' placed in the {@code ActionMapUIResource}, using the array
		''' element as the key.  For example, if the {@code
		''' "AuditoryCues.cueList"} array contains a single-element, {@code
		''' "audioKey"}, the {@code ActionMapUIResource} is created, then
		''' populated by way of {@code actionMap.put(cueList[0],
		''' createAudioAction(cueList[0]))}.
		''' <p>
		''' If the value of the default {@code "AuditoryCues.actionMap"} is
		''' {@code null} and the value of the default
		''' {@code "AuditoryCues.cueList"} is {@code null}, an empty
		''' {@code ActionMapUIResource} is created.
		''' 
		''' </summary>
		''' <returns>      an ActionMap containing {@code Actions}
		'''              responsible for playing auditory cues </returns>
		''' <exception cref="ClassCastException"> if the value of the
		'''         default {@code "AuditoryCues.actionMap"} is not an
		'''         {@code ActionMap}, or the value of the default
		'''         {@code "AuditoryCues.cueList"} is not an {@code Object[]} </exception>
		''' <seealso cref= #createAudioAction </seealso>
		''' <seealso cref= #playSound(Action)
		''' @since 1.4 </seealso>
		Protected Friend Overridable Property audioActionMap As javax.swing.ActionMap
			Get
				Dim ___audioActionMap As javax.swing.ActionMap = CType(javax.swing.UIManager.get("AuditoryCues.actionMap"), javax.swing.ActionMap)
				If ___audioActionMap Is Nothing Then
					Dim acList As Object() = CType(javax.swing.UIManager.get("AuditoryCues.cueList"), Object())
					If acList IsNot Nothing Then
						___audioActionMap = New ActionMapUIResource
						For counter As Integer = acList.Length-1 To 0 Step -1
							___audioActionMap.put(acList(counter), createAudioAction(acList(counter)))
						Next counter
					End If
					javax.swing.UIManager.lookAndFeelDefaults("AuditoryCues.actionMap") = ___audioActionMap
				End If
				Return ___audioActionMap
			End Get
		End Property

		''' <summary>
		''' Creates and returns an {@code Action} used to play a sound.
		''' <p>
		''' If {@code key} is {@code non-null}, an {@code Action} is created
		''' using the value from the defaults with key {@code key}. The value
		''' identifies the sound resource to load when
		''' {@code actionPerformed} is invoked on the {@code Action}. The
		''' sound resource is loaded into a {@code byte[]} by way of
		''' {@code getClass().getResourceAsStream()}.
		''' </summary>
		''' <param name="key"> the key identifying the audio action </param>
		''' <returns>      an {@code Action} used to play the source, or {@code null}
		'''              if {@code key} is {@code null} </returns>
		''' <seealso cref= #playSound(Action)
		''' @since 1.4 </seealso>
		Protected Friend Overridable Function createAudioAction(ByVal key As Object) As javax.swing.Action
			If key IsNot Nothing Then
				Dim audioKey As String = CStr(key)
				Dim audioValue As String = CStr(javax.swing.UIManager.get(key))
				Return New AudioAction(Me, audioKey, audioValue)
			Else
				Return Nothing
			End If
		End Function

		''' <summary>
		''' Pass the name String to the super constructor. This is used
		''' later to identify the Action and decide whether to play it or
		''' not. Store the resource String. I is used to get the audio
		''' resource. In this case, the resource is an audio file.
		''' 
		''' @since 1.4
		''' </summary>
		Private Class AudioAction
			Inherits javax.swing.AbstractAction
			Implements LineListener

			Private ReadOnly outerInstance As BasicLookAndFeel

			' We strive to only play one sound at a time (other platforms
			' appear to do this). This is done by maintaining the field
			' clipPlaying. Every time a sound is to be played,
			' cancelCurrentSound is invoked to cancel any sound that may be
			' playing.
			Private audioResource As String
			Private audioBuffer As SByte()

			''' <summary>
			''' The String is the name of the Action and
			''' points to the audio resource.
			''' The byte[] is a buffer of the audio bits.
			''' </summary>
			Public Sub New(ByVal outerInstance As BasicLookAndFeel, ByVal name As String, ByVal resource As String)
					Me.outerInstance = outerInstance
				MyBase.New(name)
				audioResource = resource
			End Sub

			Public Overridable Sub actionPerformed(ByVal e As ActionEvent)
				If audioBuffer Is Nothing Then audioBuffer = outerInstance.loadAudioData(audioResource)
				If audioBuffer IsNot Nothing Then
					cancelCurrentSound(Nothing)
					Try
						Dim soundStream As AudioInputStream = AudioSystem.getAudioInputStream(New ByteArrayInputStream(audioBuffer))
						Dim info As New DataLine.Info(GetType(Clip), soundStream.format)
						Dim clip As Clip = CType(AudioSystem.getLine(info), Clip)
						clip.open(soundStream)
						clip.addLineListener(Me)

						SyncLock outerInstance.audioLock
							outerInstance.clipPlaying = clip
						End SyncLock

						clip.start()
					Catch ex As Exception
					End Try
				End If
			End Sub

			Public Overridable Sub update(ByVal [event] As LineEvent) Implements LineListener.update
				If [event].type Is LineEvent.Type.STOP Then cancelCurrentSound(CType([event].line, Clip))
			End Sub

			''' <summary>
			''' If the parameter is null, or equal to the currently
			''' playing sound, then cancel the currently playing sound.
			''' </summary>
			Private Sub cancelCurrentSound(ByVal clip As Clip)
				Dim lastClip As Clip = Nothing

				SyncLock outerInstance.audioLock
					If clip Is Nothing OrElse clip Is outerInstance.clipPlaying Then
						lastClip = outerInstance.clipPlaying
						outerInstance.clipPlaying = Nothing
					End If
				End SyncLock

				If lastClip IsNot Nothing Then
					lastClip.removeLineListener(Me)
					lastClip.close()
				End If
			End Sub
		End Class

		''' <summary>
		''' Utility method that loads audio bits for the specified
		''' <code>soundFile</code> filename. If this method is unable to
		''' build a viable path name from the <code>baseClass</code> and
		''' <code>soundFile</code> passed into this method, it will
		''' return <code>null</code>.
		''' </summary>
		''' <param name="soundFile">    the name of the audio file to be retrieved
		'''                     from disk </param>
		''' <returns>             A byte[] with audio data or null
		''' @since 1.4 </returns>
		Private Function loadAudioData(ByVal soundFile As String) As SByte()
			If soundFile Is Nothing Then Return Nothing
	'         Copy resource into a byte array.  This is
	'         * necessary because several browsers consider
	'         * Class.getResource a security risk since it
	'         * can be used to load additional classes.
	'         * Class.getResourceAsStream just returns raw
	'         * bytes, which we can convert to a sound.
	'         
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			byte[] buffer = java.security.AccessController.doPrivileged(New java.security.PrivilegedAction<byte[]>()
	'		{
	'				public byte[] run()
	'				{
	'					try
	'					{
	'						InputStream resource = outerInstance.getClass().getResourceAsStream(soundFile);
	'						if (resource == Nothing)
	'						{
	'							Return Nothing;
	'						}
	'						BufferedInputStream in = New BufferedInputStream(resource);
	'						ByteArrayOutputStream out = New ByteArrayOutputStream(1024);
	'						byte[] buffer = New byte[1024];
	'						int n;
	'						while ((n = in.read(buffer)) > 0)
	'						{
	'							out.write(buffer, 0, n);
	'						}
	'						in.close();
	'						out.flush();
	'						buffer = out.toByteArray();
	'						Return buffer;
	'					}
	'					catch (IOException ioe)
	'					{
	'						System.err.println(ioe.toString());
	'						Return Nothing;
	'					}
	'				}
	'			});
			If buffer Is Nothing Then
				Console.Error.WriteLine(Me.GetType().name & "/" & soundFile & " not found.")
				Return Nothing
			End If
			If buffer.length = 0 Then
				Console.Error.WriteLine("warning: " & soundFile & " is zero-length")
				Return Nothing
			End If
			Return buffer
		End Function

		''' <summary>
		''' If necessary, invokes {@code actionPerformed} on
		''' {@code audioAction} to play a sound.
		''' The {@code actionPerformed} method is invoked if the value of
		''' the {@code "AuditoryCues.playList"} default is a {@code
		''' non-null} {@code Object[]} containing a {@code String} entry
		''' equal to the name of the {@code audioAction}.
		''' </summary>
		''' <param name="audioAction"> an Action that knows how to render the audio
		'''                    associated with the system or user activity
		'''                    that is occurring; a value of {@code null}, is
		'''                    ignored </param>
		''' <exception cref="ClassCastException"> if {@code audioAction} is {@code non-null}
		'''         and the value of the default {@code "AuditoryCues.playList"}
		'''         is not an {@code Object[]}
		''' @since 1.4 </exception>
		Protected Friend Overridable Sub playSound(ByVal audioAction As javax.swing.Action)
			If audioAction IsNot Nothing Then
				Dim audioStrings As Object() = CType(javax.swing.UIManager.get("AuditoryCues.playList"), Object())
				If audioStrings IsNot Nothing Then
					' create a HashSet to help us decide to play or not
					Dim audioCues As New HashSet(Of Object)
					For Each audioString As Object In audioStrings
						audioCues.Add(audioString)
					Next audioString
					' get the name of the Action
					Dim actionName As String = CStr(audioAction.getValue(javax.swing.Action.NAME))
					' if the actionName is in the audioCues HashSet, play it.
					If audioCues.Contains(actionName) Then audioAction.actionPerformed(New ActionEvent(Me, ActionEvent.ACTION_PERFORMED, actionName))
				End If
			End If
		End Sub


		''' <summary>
		''' Sets the parent of the passed in ActionMap to be the audio action
		''' map.
		''' </summary>
		Friend Shared Sub installAudioActionMap(ByVal map As javax.swing.ActionMap)
			Dim laf As javax.swing.LookAndFeel = javax.swing.UIManager.lookAndFeel
			If TypeOf laf Is BasicLookAndFeel Then map.parent = CType(laf, BasicLookAndFeel).audioActionMap
		End Sub


		''' <summary>
		''' Helper method to play a named sound.
		''' </summary>
		''' <param name="c"> JComponent to play the sound for. </param>
		''' <param name="actionKey"> Key for the sound. </param>
		Friend Shared Sub playSound(ByVal c As javax.swing.JComponent, ByVal actionKey As Object)
			Dim laf As javax.swing.LookAndFeel = javax.swing.UIManager.lookAndFeel
			If TypeOf laf Is BasicLookAndFeel Then
				Dim map As javax.swing.ActionMap = c.actionMap
				If map IsNot Nothing Then
					Dim audioAction As javax.swing.Action = map.get(actionKey)
					If audioAction IsNot Nothing Then CType(laf, BasicLookAndFeel).playSound(audioAction)
				End If
			End If
		End Sub

		''' <summary>
		''' This class contains listener that watches for all the mouse
		''' events that can possibly invoke popup on the component
		''' </summary>
		Friend Class AWTEventHelper
			Implements AWTEventListener, java.security.PrivilegedAction(Of Object)

			Private ReadOnly outerInstance As BasicLookAndFeel

			Friend Sub New(ByVal outerInstance As BasicLookAndFeel)
					Me.outerInstance = outerInstance
				MyBase.New()
				java.security.AccessController.doPrivileged(Me)
			End Sub

			Public Overridable Function run() As Object
				Dim tk As java.awt.Toolkit = java.awt.Toolkit.defaultToolkit
				If outerInstance.invocator Is Nothing Then
					tk.addAWTEventListener(Me, java.awt.AWTEvent.MOUSE_EVENT_MASK)
				Else
					tk.removeAWTEventListener(outerInstance.invocator)
				End If
				' Return value not used.
				Return Nothing
			End Function

			Public Overridable Sub eventDispatched(ByVal ev As java.awt.AWTEvent)
				Dim eventID As Integer = ev.iD
				If (eventID And java.awt.AWTEvent.MOUSE_EVENT_MASK) <> 0 Then
					Dim [me] As MouseEvent = CType(ev, MouseEvent)
					If [me].popupTrigger Then
						Dim elems As javax.swing.MenuElement() = javax.swing.MenuSelectionManager.defaultManager().selectedPath
						If elems IsNot Nothing AndAlso elems.Length <> 0 Then Return
						Dim c As Object = [me].source
						Dim src As javax.swing.JComponent = Nothing
						If TypeOf c Is javax.swing.JComponent Then
							src = CType(c, javax.swing.JComponent)
						ElseIf TypeOf c Is BasicSplitPaneDivider Then
							' Special case - if user clicks on divider we must
							' invoke popup from the SplitPane
							src = CType(CType(c, BasicSplitPaneDivider).parent, javax.swing.JComponent)
						End If
						If src IsNot Nothing Then
							If src.componentPopupMenu IsNot Nothing Then
								Dim pt As java.awt.Point = src.getPopupLocation([me])
								If pt Is Nothing Then
									pt = [me].point
									pt = javax.swing.SwingUtilities.convertPoint(CType(c, java.awt.Component), pt, src)
								End If
								src.componentPopupMenu.show(src, pt.x, pt.y)
								[me].consume()
							End If
						End If
					End If
				End If
				' Activate a JInternalFrame if necessary. 
				If eventID = MouseEvent.MOUSE_PRESSED Then
					Dim [object] As Object = ev.source
					If Not(TypeOf [object] Is java.awt.Component) Then Return
					Dim component As java.awt.Component = CType([object], java.awt.Component)
					If component IsNot Nothing Then
						Dim parent As java.awt.Component = component
						Do While parent IsNot Nothing AndAlso Not(TypeOf parent Is java.awt.Window)
							If TypeOf parent Is javax.swing.JInternalFrame Then
								' Activate the frame.
								Try
									CType(parent, javax.swing.JInternalFrame).selected = True
								Catch e1 As java.beans.PropertyVetoException
								End Try
							End If
							parent = parent.parent
						Loop
					End If
				End If
			End Sub
		End Class
	End Class

End Namespace
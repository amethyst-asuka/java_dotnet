Imports Microsoft.VisualBasic
Imports System
Imports System.Runtime.CompilerServices
Imports System.Collections.Generic
Imports System.Text
Imports javax.swing
Imports javax.swing.plaf
Imports javax.swing.plaf.basic
Imports sun.awt
Imports sun.security.action
Imports sun.swing
Imports sun.swing.plaf.synth

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
	''' SynthLookAndFeel provides the basis for creating a customized look and
	''' feel. SynthLookAndFeel does not directly provide a look, all painting is
	''' delegated.
	''' You need to either provide a configuration file, by way of the
	''' <seealso cref="#load"/> method, or provide your own <seealso cref="SynthStyleFactory"/>
	''' to <seealso cref="#setStyleFactory"/>. Refer to the
	''' <a href="package-summary.html">package summary</a> for an example of
	''' loading a file, and <seealso cref="javax.swing.plaf.synth.SynthStyleFactory"/> for
	''' an example of providing your own <code>SynthStyleFactory</code> to
	''' <code>setStyleFactory</code>.
	''' <p>
	''' <strong>Warning:</strong>
	''' This class implements <seealso cref="Serializable"/> as a side effect of it
	''' extending <seealso cref="BasicLookAndFeel"/>. It is not intended to be serialized.
	''' An attempt to serialize it will
	''' result in <seealso cref="NotSerializableException"/>.
	''' 
	''' @serial exclude
	''' @since 1.5
	''' @author Scott Violet
	''' </summary>
	Public Class SynthLookAndFeel
		Inherits BasicLookAndFeel

		''' <summary>
		''' Used in a handful of places where we need an empty Insets.
		''' </summary>
		Friend Shared ReadOnly EMPTY_UIRESOURCE_INSETS As Insets = New InsetsUIResource(0, 0, 0, 0)

		''' <summary>
		''' AppContext key to get the current SynthStyleFactory.
		''' </summary>
		Private Shared ReadOnly STYLE_FACTORY_KEY As Object = New StringBuilder("com.sun.java.swing.plaf.gtk.StyleCache")

		''' <summary>
		''' AppContext key to get selectedUI.
		''' </summary>
		Private Shared ReadOnly SELECTED_UI_KEY As Object = New StringBuilder("selectedUI")

		''' <summary>
		''' AppContext key to get selectedUIState.
		''' </summary>
		Private Shared ReadOnly SELECTED_UI_STATE_KEY As Object = New StringBuilder("selectedUIState")

		''' <summary>
		''' The last SynthStyleFactory that was asked for from AppContext
		''' <code>lastContext</code>.
		''' </summary>
		Private Shared lastFactory As SynthStyleFactory
		''' <summary>
		''' AppContext lastLAF came from.
		''' </summary>
		Private Shared lastContext As AppContext

		''' <summary>
		''' SynthStyleFactory for the this SynthLookAndFeel.
		''' </summary>
		Private factory As SynthStyleFactory

		''' <summary>
		''' Map of defaults table entries. This is populated via the load
		''' method.
		''' </summary>
		Private defaultsMap As IDictionary(Of String, Object)

		Private _handler As Handler

		Friend Property Shared selectedUI As ComponentUI
			Get
				Return CType(AppContext.appContext.get(SELECTED_UI_KEY), ComponentUI)
			End Get
		End Property

		''' <summary>
		''' Used by the renderers. For the most part the renderers are implemented
		''' as Labels, which is problematic in so far as they are never selected.
		''' To accommodate this SynthLabelUI checks if the current
		''' UI matches that of <code>selectedUI</code> (which this methods sets), if
		''' it does, then a state as set by this method is returned. This provides
		''' a way for labels to have a state other than selected.
		''' </summary>
		Friend Shared Sub setSelectedUI(ByVal uix As ComponentUI, ByVal selected As Boolean, ByVal focused As Boolean, ByVal enabled As Boolean, ByVal rollover As Boolean)
			Dim ___selectedUIState As Integer = 0

			If selected Then
				___selectedUIState = SynthConstants.SELECTED
				If focused Then ___selectedUIState = ___selectedUIState Or SynthConstants.FOCUSED
			ElseIf rollover AndAlso enabled Then
				___selectedUIState = ___selectedUIState Or SynthConstants.MOUSE_OVER Or SynthConstants.ENABLED
				If focused Then ___selectedUIState = ___selectedUIState Or SynthConstants.FOCUSED
			Else
				If enabled Then
					___selectedUIState = ___selectedUIState Or SynthConstants.ENABLED
					If focused Then ___selectedUIState = ___selectedUIState Or SynthConstants.FOCUSED
				Else
					___selectedUIState = ___selectedUIState Or SynthConstants.DISABLED
				End If
			End If

			Dim context As AppContext = AppContext.appContext

			context.put(SELECTED_UI_KEY, uix)
			context.put(SELECTED_UI_STATE_KEY, Convert.ToInt32(___selectedUIState))
		End Sub

		Friend Property Shared selectedUIState As Integer
			Get
				Dim result As Integer? = CInt(Fix(AppContext.appContext.get(SELECTED_UI_STATE_KEY)))
    
				Return If(result Is Nothing, 0, result)
			End Get
		End Property

		''' <summary>
		''' Clears out the selected UI that was last set in setSelectedUI.
		''' </summary>
		Friend Shared Sub resetSelectedUI()
			AppContext.appContext.remove(SELECTED_UI_KEY)
		End Sub


		''' <summary>
		''' Sets the SynthStyleFactory that the UI classes provided by
		''' synth will use to obtain a SynthStyle.
		''' </summary>
		''' <param name="cache"> SynthStyleFactory the UIs should use. </param>
		Public Shared Property styleFactory As SynthStyleFactory
			Set(ByVal cache As SynthStyleFactory)
				' We assume the setter is called BEFORE the getter has been invoked
				' for a particular AppContext.
				SyncLock GetType(SynthLookAndFeel)
					Dim context As AppContext = AppContext.appContext
					lastFactory = cache
					lastContext = context
					context.put(STYLE_FACTORY_KEY, cache)
				End SyncLock
			End Set
			Get
				SyncLock GetType(SynthLookAndFeel)
					Dim context As AppContext = AppContext.appContext
    
					If lastContext Is context Then Return lastFactory
					lastContext = context
					lastFactory = CType(context.get(STYLE_FACTORY_KEY), SynthStyleFactory)
					Return lastFactory
				End SyncLock
			End Get
		End Property


		''' <summary>
		''' Returns the component state for the specified component. This should
		''' only be used for Components that don't have any special state beyond
		''' that of ENABLED, DISABLED or FOCUSED. For example, buttons shouldn't
		''' call into this method.
		''' </summary>
		Friend Shared Function getComponentState(ByVal c As Component) As Integer
			If c.enabled Then
				If c.focusOwner Then Return SynthUI.ENABLED Or SynthUI.FOCUSED
				Return SynthUI.ENABLED
			End If
			Return SynthUI.DISABLED
		End Function

		''' <summary>
		''' Gets a SynthStyle for the specified region of the specified component.
		''' This is not for general consumption, only custom UIs should call this
		''' method.
		''' </summary>
		''' <param name="c"> JComponent to get the SynthStyle for </param>
		''' <param name="region"> Identifies the region of the specified component </param>
		''' <returns> SynthStyle to use. </returns>
		Public Shared Function getStyle(ByVal c As JComponent, ByVal ___region As Region) As SynthStyle
			Return styleFactory.getStyle(c, ___region)
		End Function

		''' <summary>
		''' Returns true if the Style should be updated in response to the
		''' specified PropertyChangeEvent. This forwards to
		''' <code>shouldUpdateStyleOnAncestorChanged</code> as necessary.
		''' </summary>
		Friend Shared Function shouldUpdateStyle(ByVal [event] As PropertyChangeEvent) As Boolean
			Dim laf As LookAndFeel = UIManager.lookAndFeel
			Return (TypeOf laf Is SynthLookAndFeel AndAlso CType(laf, SynthLookAndFeel).shouldUpdateStyleOnEvent([event]))
		End Function

		''' <summary>
		''' A convience method that will reset the Style of StyleContext if
		''' necessary.
		''' </summary>
		''' <returns> newStyle </returns>
		Friend Shared Function updateStyle(ByVal context As SynthContext, ByVal ui As SynthUI) As SynthStyle
			Dim newStyle As SynthStyle = getStyle(context.component, context.region)
			Dim oldStyle As SynthStyle = context.style

			If newStyle IsNot oldStyle Then
				If oldStyle IsNot Nothing Then oldStyle.uninstallDefaults(context)
				context.style = newStyle
				newStyle.installDefaults(context, ui)
			End If
			Return newStyle
		End Function

		''' <summary>
		''' Updates the style associated with <code>c</code>, and all its children.
		''' This is a lighter version of
		''' <code>SwingUtilities.updateComponentTreeUI</code>.
		''' </summary>
		''' <param name="c"> Component to update style for. </param>
		Public Shared Sub updateStyles(ByVal c As Component)
			If TypeOf c Is JComponent Then
				' Yes, this is hacky. A better solution is to get the UI
				' and cast, but JComponent doesn't expose a getter for the UI
				' (each of the UIs do), making that approach impractical.
				Dim ___name As String = c.name
				c.name = Nothing
				If ___name IsNot Nothing Then c.name = ___name
				CType(c, JComponent).revalidate()
			End If
			Dim children As Component() = Nothing
			If TypeOf c Is JMenu Then
				children = CType(c, JMenu).menuComponents
			ElseIf TypeOf c Is Container Then
				children = CType(c, Container).components
			End If
			If children IsNot Nothing Then
				For Each child As Component In children
					updateStyles(child)
				Next child
			End If
			c.repaint()
		End Sub

		''' <summary>
		''' Returns the Region for the JComponent <code>c</code>.
		''' </summary>
		''' <param name="c"> JComponent to fetch the Region for </param>
		''' <returns> Region corresponding to <code>c</code> </returns>
		Public Shared Function getRegion(ByVal c As JComponent) As Region
			Return Region.getRegion(c)
		End Function

		''' <summary>
		''' A convenience method to return where the foreground should be
		''' painted for the Component identified by the passed in
		''' AbstractSynthContext.
		''' </summary>
		Friend Shared Function getPaintingInsets(ByVal state As SynthContext, ByVal insets As Insets) As Insets
			If state.subregion Then
				insets = state.style.getInsets(state, insets)
			Else
				insets = state.component.getInsets(insets)
			End If
			Return insets
		End Function

		''' <summary>
		''' A convenience method that handles painting of the background.
		''' All SynthUI implementations should override update and invoke
		''' this method.
		''' </summary>
		Friend Shared Sub update(ByVal state As SynthContext, ByVal g As Graphics)
			paintRegion(state, g, Nothing)
		End Sub

		''' <summary>
		''' A convenience method that handles painting of the background for
		''' subregions. All SynthUI's that have subregions should invoke
		''' this method, than paint the foreground.
		''' </summary>
		Friend Shared Sub updateSubregion(ByVal state As SynthContext, ByVal g As Graphics, ByVal bounds As Rectangle)
			paintRegion(state, g, bounds)
		End Sub

		Private Shared Sub paintRegion(ByVal state As SynthContext, ByVal g As Graphics, ByVal bounds As Rectangle)
			Dim c As JComponent = state.component
			Dim ___style As SynthStyle = state.style
			Dim x, y, width, height As Integer

			If bounds Is Nothing Then
				x = 0
				y = 0
				width = c.width
				height = c.height
			Else
				x = bounds.x
				y = bounds.y
				width = bounds.width
				height = bounds.height
			End If

			' Fill in the background, if necessary.
			Dim subregion As Boolean = state.subregion
			If (subregion AndAlso ___style.isOpaque(state)) OrElse ((Not subregion) AndAlso c.opaque) Then
				g.color = ___style.getColor(state, ColorType.BACKGROUND)
				g.fillRect(x, y, width, height)
			End If
		End Sub

		Friend Shared Function isLeftToRight(ByVal c As Component) As Boolean
			Return c.componentOrientation.leftToRight
		End Function

		''' <summary>
		''' Returns the ui that is of type <code>klass</code>, or null if
		''' one can not be found.
		''' </summary>
		Friend Shared Function getUIOfType(ByVal ui As ComponentUI, ByVal klass As Type) As Object
			If klass.IsInstanceOfType(ui) Then Return ui
			Return Nothing
		End Function

		''' <summary>
		''' Creates the Synth look and feel <code>ComponentUI</code> for
		''' the passed in <code>JComponent</code>.
		''' </summary>
		''' <param name="c"> JComponent to create the <code>ComponentUI</code> for </param>
		''' <returns> ComponentUI to use for <code>c</code> </returns>
		Public Shared Function createUI(ByVal c As JComponent) As ComponentUI
			Dim key As String = c.uIClassID.intern()

			If key = "ButtonUI" Then
				Return SynthButtonUI.createUI(c)
			ElseIf key = "CheckBoxUI" Then
				Return SynthCheckBoxUI.createUI(c)
			ElseIf key = "CheckBoxMenuItemUI" Then
				Return SynthCheckBoxMenuItemUI.createUI(c)
			ElseIf key = "ColorChooserUI" Then
				Return SynthColorChooserUI.createUI(c)
			ElseIf key = "ComboBoxUI" Then
				Return SynthComboBoxUI.createUI(c)
			ElseIf key = "DesktopPaneUI" Then
				Return SynthDesktopPaneUI.createUI(c)
			ElseIf key = "DesktopIconUI" Then
				Return SynthDesktopIconUI.createUI(c)
			ElseIf key = "EditorPaneUI" Then
				Return SynthEditorPaneUI.createUI(c)
			ElseIf key = "FileChooserUI" Then
				Return SynthFileChooserUI.createUI(c)
			ElseIf key = "FormattedTextFieldUI" Then
				Return SynthFormattedTextFieldUI.createUI(c)
			ElseIf key = "InternalFrameUI" Then
				Return SynthInternalFrameUI.createUI(c)
			ElseIf key = "LabelUI" Then
				Return SynthLabelUI.createUI(c)
			ElseIf key = "ListUI" Then
				Return SynthListUI.createUI(c)
			ElseIf key = "MenuBarUI" Then
				Return SynthMenuBarUI.createUI(c)
			ElseIf key = "MenuUI" Then
				Return SynthMenuUI.createUI(c)
			ElseIf key = "MenuItemUI" Then
				Return SynthMenuItemUI.createUI(c)
			ElseIf key = "OptionPaneUI" Then
				Return SynthOptionPaneUI.createUI(c)
			ElseIf key = "PanelUI" Then
				Return SynthPanelUI.createUI(c)
			ElseIf key = "PasswordFieldUI" Then
				Return SynthPasswordFieldUI.createUI(c)
			ElseIf key = "PopupMenuSeparatorUI" Then
				Return SynthSeparatorUI.createUI(c)
			ElseIf key = "PopupMenuUI" Then
				Return SynthPopupMenuUI.createUI(c)
			ElseIf key = "ProgressBarUI" Then
				Return SynthProgressBarUI.createUI(c)
			ElseIf key = "RadioButtonUI" Then
				Return SynthRadioButtonUI.createUI(c)
			ElseIf key = "RadioButtonMenuItemUI" Then
				Return SynthRadioButtonMenuItemUI.createUI(c)
			ElseIf key = "RootPaneUI" Then
				Return SynthRootPaneUI.createUI(c)
			ElseIf key = "ScrollBarUI" Then
				Return SynthScrollBarUI.createUI(c)
			ElseIf key = "ScrollPaneUI" Then
				Return SynthScrollPaneUI.createUI(c)
			ElseIf key = "SeparatorUI" Then
				Return SynthSeparatorUI.createUI(c)
			ElseIf key = "SliderUI" Then
				Return SynthSliderUI.createUI(c)
			ElseIf key = "SpinnerUI" Then
				Return SynthSpinnerUI.createUI(c)
			ElseIf key = "SplitPaneUI" Then
				Return SynthSplitPaneUI.createUI(c)
			ElseIf key = "TabbedPaneUI" Then
				Return SynthTabbedPaneUI.createUI(c)
			ElseIf key = "TableUI" Then
				Return SynthTableUI.createUI(c)
			ElseIf key = "TableHeaderUI" Then
				Return SynthTableHeaderUI.createUI(c)
			ElseIf key = "TextAreaUI" Then
				Return SynthTextAreaUI.createUI(c)
			ElseIf key = "TextFieldUI" Then
				Return SynthTextFieldUI.createUI(c)
			ElseIf key = "TextPaneUI" Then
				Return SynthTextPaneUI.createUI(c)
			ElseIf key = "ToggleButtonUI" Then
				Return SynthToggleButtonUI.createUI(c)
			ElseIf key = "ToolBarSeparatorUI" Then
				Return SynthSeparatorUI.createUI(c)
			ElseIf key = "ToolBarUI" Then
				Return SynthToolBarUI.createUI(c)
			ElseIf key = "ToolTipUI" Then
				Return SynthToolTipUI.createUI(c)
			ElseIf key = "TreeUI" Then
				Return SynthTreeUI.createUI(c)
			ElseIf key = "ViewportUI" Then
				Return SynthViewportUI.createUI(c)
			End If
			Return Nothing
		End Function


		''' <summary>
		''' Creates a SynthLookAndFeel.
		''' <p>
		''' For the returned <code>SynthLookAndFeel</code> to be useful you need to
		''' invoke <code>load</code> to specify the set of
		''' <code>SynthStyle</code>s, or invoke <code>setStyleFactory</code>.
		''' </summary>
		''' <seealso cref= #load </seealso>
		''' <seealso cref= #setStyleFactory </seealso>
		Public Sub New()
			factory = New DefaultSynthStyleFactory
			_handler = New Handler(Me)
		End Sub

		''' <summary>
		''' Loads the set of <code>SynthStyle</code>s that will be used by
		''' this <code>SynthLookAndFeel</code>. <code>resourceBase</code> is
		''' used to resolve any path based resources, for example an
		''' <code>Image</code> would be resolved by
		''' <code>resourceBase.getResource(path)</code>. Refer to
		''' <a href="doc-files/synthFileFormat.html">Synth File Format</a>
		''' for more information.
		''' </summary>
		''' <param name="input"> InputStream to load from </param>
		''' <param name="resourceBase"> used to resolve any images or other resources </param>
		''' <exception cref="ParseException"> if there is an error in parsing </exception>
		''' <exception cref="IllegalArgumentException"> if input or resourceBase is <code>null</code> </exception>
		Public Overridable Sub load(ByVal input As InputStream, ByVal resourceBase As Type)
			If resourceBase Is Nothing Then Throw New System.ArgumentException("You must supply a valid resource base Class")

			If defaultsMap Is Nothing Then defaultsMap = New Dictionary(Of String, Object)

			CType(New SynthParser, SynthParser).parse(input, CType(factory, DefaultSynthStyleFactory), Nothing, resourceBase, defaultsMap)
		End Sub

		''' <summary>
		''' Loads the set of <code>SynthStyle</code>s that will be used by
		''' this <code>SynthLookAndFeel</code>. Path based resources are resolved
		''' relatively to the specified <code>URL</code> of the style. For example
		''' an <code>Image</code> would be resolved by
		''' <code>new URL(synthFile, path)</code>. Refer to
		''' <a href="doc-files/synthFileFormat.html">Synth File Format</a> for more
		''' information.
		''' </summary>
		''' <param name="url"> the <code>URL</code> to load the set of
		'''     <code>SynthStyle</code> from </param>
		''' <exception cref="ParseException"> if there is an error in parsing </exception>
		''' <exception cref="IllegalArgumentException"> if synthSet is <code>null</code> </exception>
		''' <exception cref="IOException"> if synthSet cannot be opened as an <code>InputStream</code>
		''' @since 1.6 </exception>
		Public Overridable Sub load(ByVal url As URL)
			If url Is Nothing Then Throw New System.ArgumentException("You must supply a valid Synth set URL")

			If defaultsMap Is Nothing Then defaultsMap = New Dictionary(Of String, Object)

			Dim input As InputStream = url.openStream()
			CType(New SynthParser, SynthParser).parse(input, CType(factory, DefaultSynthStyleFactory), url, Nothing, defaultsMap)
		End Sub

		''' <summary>
		''' Called by UIManager when this look and feel is installed.
		''' </summary>
		Public Overrides Sub initialize()
			MyBase.initialize()
			DefaultLookup.defaultLookup = New SynthDefaultLookup
			styleFactory = factory
			KeyboardFocusManager.currentKeyboardFocusManager.addPropertyChangeListener(_handler)
		End Sub

		''' <summary>
		''' Called by UIManager when this look and feel is uninstalled.
		''' </summary>
		Public Overrides Sub uninitialize()
			KeyboardFocusManager.currentKeyboardFocusManager.removePropertyChangeListener(_handler)
			' We should uninstall the StyleFactory here, but unfortunately
			' there are a handful of things that retain references to the
			' LookAndFeel and expect things to work
			MyBase.uninitialize()
		End Sub

		''' <summary>
		''' Returns the defaults for this SynthLookAndFeel.
		''' </summary>
		''' <returns> Defaults table. </returns>
		Public Property Overrides defaults As UIDefaults
			Get
				Dim table As New UIDefaults(60, 0.75f)
    
				Region.registerUIs(table)
				table.defaultLocale = Locale.default
				table.addResourceBundle("com.sun.swing.internal.plaf.basic.resources.basic")
				table.addResourceBundle("com.sun.swing.internal.plaf.synth.resources.synth")
    
				' SynthTabbedPaneUI supports rollover on tabs, GTK does not
				table("TabbedPane.isTabRollover") = Boolean.TRUE
    
				' These need to be defined for JColorChooser to work.
				table("ColorChooser.swatchesRecentSwatchSize") = New Dimension(10, 10)
				table("ColorChooser.swatchesDefaultRecentColor") = Color.RED
				table("ColorChooser.swatchesSwatchSize") = New Dimension(10, 10)
    
				' These need to be defined for ImageView.
				table("html.pendingImage") = SwingUtilities2.makeIcon(Me.GetType(), GetType(BasicLookAndFeel), "icons/image-delayed.png")
				table("html.missingImage") = SwingUtilities2.makeIcon(Me.GetType(), GetType(BasicLookAndFeel), "icons/image-failed.png")
    
				' These are needed for PopupMenu.
				table("PopupMenu.selectedWindowInputMapBindings") = New Object() { "ESCAPE", "cancel", "DOWN", "selectNext", "KP_DOWN", "selectNext", "UP", "selectPrevious", "KP_UP", "selectPrevious", "LEFT", "selectParent", "KP_LEFT", "selectParent", "RIGHT", "selectChild", "KP_RIGHT", "selectChild", "ENTER", "return", "SPACE", "return" }
				table("PopupMenu.selectedWindowInputMapBindings.RightToLeft") = New Object() { "LEFT", "selectChild", "KP_LEFT", "selectChild", "RIGHT", "selectParent", "KP_RIGHT", "selectParent" }
    
				' enabled antialiasing depending on desktop settings
				flushUnreferenced()
				Dim ___aaTextInfo As Object = aATextInfo
				table(SwingUtilities2.AA_TEXT_PROPERTY_KEY) = ___aaTextInfo
				Dim TempAATextListener As AATextListener = New AATextListener(Me)
    
				If defaultsMap IsNot Nothing Then table.putAll(defaultsMap)
				Return table
			End Get
		End Property

		''' <summary>
		''' Returns true, SynthLookAndFeel is always supported.
		''' </summary>
		''' <returns> true. </returns>
		Public Property Overrides supportedLookAndFeel As Boolean
			Get
				Return True
			End Get
		End Property

		''' <summary>
		''' Returns false, SynthLookAndFeel is not a native look and feel.
		''' </summary>
		''' <returns> false </returns>
		Public Property Overrides nativeLookAndFeel As Boolean
			Get
				Return False
			End Get
		End Property

		''' <summary>
		''' Returns a textual description of SynthLookAndFeel.
		''' </summary>
		''' <returns> textual description of synth. </returns>
		Public Property Overrides description As String
			Get
				Return "Synth look and feel"
			End Get
		End Property

		''' <summary>
		''' Return a short string that identifies this look and feel.
		''' </summary>
		''' <returns> a short string identifying this look and feel. </returns>
		Public Property Overrides name As String
			Get
				Return "Synth look and feel"
			End Get
		End Property

		''' <summary>
		''' Return a string that identifies this look and feel.
		''' </summary>
		''' <returns> a short string identifying this look and feel. </returns>
		Public Property Overrides iD As String
			Get
				Return "Synth"
			End Get
		End Property

		''' <summary>
		''' Returns whether or not the UIs should update their
		''' <code>SynthStyles</code> from the <code>SynthStyleFactory</code>
		''' when the ancestor of the <code>JComponent</code> changes. A subclass
		''' that provided a <code>SynthStyleFactory</code> that based the
		''' return value from <code>getStyle</code> off the containment hierarchy
		''' would override this method to return true.
		''' </summary>
		''' <returns> whether or not the UIs should update their
		''' <code>SynthStyles</code> from the <code>SynthStyleFactory</code>
		''' when the ancestor changed. </returns>
		Public Overridable Function shouldUpdateStyleOnAncestorChanged() As Boolean
			Return False
		End Function

		''' <summary>
		''' Returns whether or not the UIs should update their styles when a
		''' particular event occurs.
		''' </summary>
		''' <param name="ev"> a {@code PropertyChangeEvent} </param>
		''' <returns> whether or not the UIs should update their styles
		''' @since 1.7 </returns>
		Protected Friend Overridable Function shouldUpdateStyleOnEvent(ByVal ev As PropertyChangeEvent) As Boolean
			Dim eName As String = ev.propertyName
			If "name" = eName OrElse "componentOrientation" = eName Then Return True
			If "ancestor" = eName AndAlso ev.newValue IsNot Nothing Then Return shouldUpdateStyleOnAncestorChanged()
			Return False
		End Function

		''' <summary>
		''' Returns the antialiasing information as specified by the host desktop.
		''' Antialiasing might be forced off if the desktop is GNOME and the user
		''' has set his locale to Chinese, Japanese or Korean. This is consistent
		''' with what GTK does. See com.sun.java.swing.plaf.gtk.GtkLookAndFeel
		''' for more information about CJK and antialiased fonts.
		''' </summary>
		''' <returns> the text antialiasing information associated to the desktop </returns>
		Private Property Shared aATextInfo As Object
			Get
				Dim language As String = Locale.default.language
				Dim desktop As String = AccessController.doPrivileged(New GetPropertyAction("sun.desktop"))
    
				Dim isCjkLocale As Boolean = (Locale.CHINESE.language.Equals(language) OrElse Locale.JAPANESE.language.Equals(language) OrElse Locale.KOREAN.language.Equals(language))
				Dim isGnome As Boolean = "gnome".Equals(desktop)
				Dim isLocal As Boolean = SwingUtilities2.localDisplay
    
				Dim aAtAA As Boolean = isLocal AndAlso ((Not isGnome) OrElse (Not isCjkLocale))
    
				Dim ___aaTextInfo As Object = SwingUtilities2.AATextInfo.getAATextInfo(aAtAA)
				Return ___aaTextInfo
			End Get
		End Property

		Private Shared queue As New ReferenceQueue(Of LookAndFeel)

		Private Shared Sub flushUnreferenced()
			Dim aatl As AATextListener
			aatl = CType(queue.poll(), AATextListener)
			Do While aatl IsNot Nothing
				aatl.Dispose()
				aatl = CType(queue.poll(), AATextListener)
			Loop
		End Sub

		Private Class AATextListener
			Inherits WeakReference(Of LookAndFeel)
			Implements PropertyChangeListener

			Private key As String = SunToolkit.DESKTOPFONTHINTS

			Friend Sub New(ByVal laf As LookAndFeel)
				MyBase.New(laf, queue)
				Dim tk As Toolkit = Toolkit.defaultToolkit
				tk.addPropertyChangeListener(key, Me)
			End Sub

			Public Overrides Sub propertyChange(ByVal pce As PropertyChangeEvent)
				Dim defaults As UIDefaults = UIManager.lookAndFeelDefaults
				If defaults.getBoolean("Synth.doNotSetTextAA") Then
					Dispose()
					Return
				End If

				Dim laf As LookAndFeel = get()
				If laf Is Nothing OrElse laf IsNot UIManager.lookAndFeel Then
					Dispose()
					Return
				End If

				Dim aaTextInfo As Object = aATextInfo
				defaults(SwingUtilities2.AA_TEXT_PROPERTY_KEY) = aaTextInfo

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
				updateStyles(window)
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
	'					@Override public void run()
	'					{
	'						updateAllUIs();
	'						setUpdatePending(False);
	'					}
	'				};
					SwingUtilities.invokeLater(uiUpdater)
				End If
			End Sub
		End Class

		Private Sub writeObject(ByVal out As java.io.ObjectOutputStream)
			Throw New NotSerializableException(Me.GetType().name)
		End Sub

		Private Class Handler
			Implements PropertyChangeListener

			Private ReadOnly outerInstance As SynthLookAndFeel

			Public Sub New(ByVal outerInstance As SynthLookAndFeel)
				Me.outerInstance = outerInstance
			End Sub

			Public Overrides Sub propertyChange(ByVal evt As PropertyChangeEvent)
				Dim propertyName As String = evt.propertyName
				Dim newValue As Object = evt.newValue
				Dim oldValue As Object = evt.oldValue

				If "focusOwner" = propertyName Then
					If TypeOf oldValue Is JComponent Then repaintIfBackgroundsDiffer(CType(oldValue, JComponent))

					If TypeOf newValue Is JComponent Then repaintIfBackgroundsDiffer(CType(newValue, JComponent))
				ElseIf "managingFocus" = propertyName Then
					' De-register listener on old keyboard focus manager and
					' register it on the new one.
					Dim manager As KeyboardFocusManager = CType(evt.source, KeyboardFocusManager)
					If newValue.Equals(Boolean.FALSE) Then
						manager.removePropertyChangeListener(outerInstance._handler)
					Else
						manager.addPropertyChangeListener(outerInstance._handler)
					End If
				End If
			End Sub

			''' <summary>
			''' This is a support method that will check if the background colors of
			''' the specified component differ between focused and unfocused states.
			''' If the color differ the component will then repaint itself.
			''' 
			''' @comp the component to check
			''' </summary>
			Private Sub repaintIfBackgroundsDiffer(ByVal comp As JComponent)
				Dim ui As ComponentUI = CType(comp.getClientProperty(SwingUtilities2.COMPONENT_UI_PROPERTY_KEY), ComponentUI)
				If TypeOf ui Is SynthUI Then
					Dim synthUI As SynthUI = CType(ui, SynthUI)
					Dim context As SynthContext = synthUI.getContext(comp)
					Dim style As SynthStyle = context.style
					Dim state As Integer = context.componentState

					' Get the current background color.
					Dim currBG As Color = style.getColor(context, ColorType.BACKGROUND)

					' Get the last background color.
					state = state Xor SynthConstants.FOCUSED
					context.componentState = state
					Dim lastBG As Color = style.getColor(context, ColorType.BACKGROUND)

					' Reset the component state back to original.
					state = state Xor SynthConstants.FOCUSED
					context.componentState = state

					' Repaint the component if the backgrounds differed.
					If currBG IsNot Nothing AndAlso (Not currBG.Equals(lastBG)) Then comp.repaint()
					context.Dispose()
				End If
			End Sub
		End Class
	End Class

End Namespace
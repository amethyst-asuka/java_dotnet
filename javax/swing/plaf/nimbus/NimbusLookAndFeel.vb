Imports System
Imports System.Collections.Generic

'
' * Copyright (c) 2005, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' <p>The NimbusLookAndFeel class.</p>
	''' 
	''' @author Jasper Potts
	''' @author Richard Bair
	''' </summary>
	Public Class NimbusLookAndFeel
		Inherits javax.swing.plaf.synth.SynthLookAndFeel

		''' <summary>
		''' Set of standard region names for UIDefaults Keys </summary>
		Private Shared ReadOnly COMPONENT_KEYS As String() = {"ArrowButton", "Button", "CheckBox", "CheckBoxMenuItem", "ColorChooser", "ComboBox", "DesktopPane", "DesktopIcon", "EditorPane", "FileChooser", "FormattedTextField", "InternalFrame", "InternalFrameTitlePane", "Label", "List", "Menu", "MenuBar", "MenuItem", "OptionPane", "Panel", "PasswordField", "PopupMenu", "PopupMenuSeparator", "ProgressBar", "RadioButton", "RadioButtonMenuItem", "RootPane", "ScrollBar", "ScrollBarTrack", "ScrollBarThumb", "ScrollPane", "Separator", "Slider", "SliderTrack", "SliderThumb", "Spinner", "SplitPane", "TabbedPane", "Table", "TableHeader", "TextArea", "TextField", "TextPane", "ToggleButton", "ToolBar", "ToolTip", "Tree", "Viewport"}

		''' <summary>
		''' A reference to the auto-generated file NimbusDefaults. This file contains
		''' the default mappings and values for the look and feel as specified in the
		''' visual designer.
		''' </summary>
		Private defaults As NimbusDefaults

		''' <summary>
		''' Reference to populated LAD uidefaults
		''' </summary>
		Private uiDefaults As javax.swing.UIDefaults

		Private defaultsListener As New DefaultsListener(Me)

		''' <summary>
		''' Create a new NimbusLookAndFeel.
		''' </summary>
		Public Sub New()
			MyBase.New()
			defaults = New NimbusDefaults
		End Sub

		''' <summary>
		''' Called by UIManager when this look and feel is installed. </summary>
		Public Overrides Sub initialize()
			MyBase.initialize()
			defaults.initialize()
			' create synth style factory
			styleFactoryory(New SynthStyleFactoryAnonymousInnerClassHelper
		End Sub

		Private Class SynthStyleFactoryAnonymousInnerClassHelper
			Inherits javax.swing.plaf.synth.SynthStyleFactory

			Public Overrides Function getStyle(ByVal c As javax.swing.JComponent, ByVal r As javax.swing.plaf.synth.Region) As javax.swing.plaf.synth.SynthStyle
				Return outerInstance.defaults.getStyle(c, r)
			End Function
		End Class


		''' <summary>
		''' Called by UIManager when this look and feel is uninstalled. </summary>
		Public Overrides Sub uninitialize()
			MyBase.uninitialize()
			defaults.uninitialize()
			' clear all cached images to free memory
			ImageCache.instance.flush()
			javax.swing.UIManager.defaults.removePropertyChangeListener(defaultsListener)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Property Overrides defaults As javax.swing.UIDefaults
			Get
				If uiDefaults Is Nothing Then
					' Detect platform
					Dim osName As String = getSystemProperty("os.name")
					Dim isWindows As Boolean = osName IsNot Nothing AndAlso osName.Contains("Windows")
    
					' We need to call super for basic's properties file.
					uiDefaults = MyBase.defaults
					defaults.initializeDefaults(uiDefaults)
    
					' Install Keybindings
					If isWindows Then
						sun.swing.plaf.WindowsKeybindings.installKeybindings(uiDefaults)
					Else
						sun.swing.plaf.GTKKeybindings.installKeybindings(uiDefaults)
					End If
    
					' Add Titled Border
					uiDefaults("TitledBorder.titlePosition") = javax.swing.border.TitledBorder.ABOVE_TOP
					uiDefaults("TitledBorder.border") = New javax.swing.plaf.BorderUIResource(New LoweredBorder)
					uiDefaults("TitledBorder.titleColor") = getDerivedColor("text",0.0f,0.0f,0.23f,0,True)
					uiDefaults("TitledBorder.font") = New NimbusDefaults.DerivedFont("defaultFont", 1f, True, Nothing)
    
					' Choose Dialog button positions
					uiDefaults("OptionPane.isYesLast") = Not isWindows
    
					' Store Table ScrollPane Corner Component
					uiDefaults.put("Table.scrollPaneCornerComponent", New ActiveValueAnonymousInnerClassHelper
    
					' Setup the settings for ToolBarSeparator which is custom
					' installed for Nimbus
					uiDefaults("ToolBarSeparator[Enabled].backgroundPainter") = New ToolBarSeparatorPainter
    
					' Populate UIDefaults with a standard set of properties
					For Each componentKey As String In COMPONENT_KEYS
						Dim key As String = componentKey & ".foreground"
						If Not uiDefaults.ContainsKey(key) Then uiDefaults(key) = New NimbusProperty(Me, componentKey,"textForeground")
						key = componentKey & ".background"
						If Not uiDefaults.ContainsKey(key) Then uiDefaults(key) = New NimbusProperty(Me, componentKey,"background")
						key = componentKey & ".font"
						If Not uiDefaults.ContainsKey(key) Then uiDefaults(key) = New NimbusProperty(Me, componentKey,"font")
						key = componentKey & ".disabledText"
						If Not uiDefaults.ContainsKey(key) Then uiDefaults(key) = New NimbusProperty(Me, componentKey,"Disabled", "textForeground")
						key = componentKey & ".disabled"
						If Not uiDefaults.ContainsKey(key) Then uiDefaults(key) = New NimbusProperty(Me, componentKey,"Disabled", "background")
					Next componentKey
    
					' FileView icon keys are used by some applications, we don't have
					' a computer icon at the moment so using home icon for now
					uiDefaults("FileView.computerIcon") = New LinkProperty(Me, "FileChooser.homeFolderIcon")
					uiDefaults("FileView.directoryIcon") = New LinkProperty(Me, "FileChooser.directoryIcon")
					uiDefaults("FileView.fileIcon") = New LinkProperty(Me, "FileChooser.fileIcon")
					uiDefaults("FileView.floppyDriveIcon") = New LinkProperty(Me, "FileChooser.floppyDriveIcon")
					uiDefaults("FileView.hardDriveIcon") = New LinkProperty(Me, "FileChooser.hardDriveIcon")
				End If
				Return uiDefaults
			End Get
		End Property

		Private Class ActiveValueAnonymousInnerClassHelper
			Implements ActiveValue

			Public Overrides Function createValue(ByVal table As javax.swing.UIDefaults) As Object
				Return New TableScrollPaneCorner
			End Function
		End Class

		''' <summary>
		''' Gets the style associated with the given component and region. This
		''' will never return null. If an appropriate component and region cannot
		''' be determined, then a default style is returned.
		''' </summary>
		''' <param name="c"> a non-null reference to a JComponent </param>
		''' <param name="r"> a non-null reference to the region of the component c </param>
		''' <returns> a non-null reference to a NimbusStyle. </returns>
		Public Shared Function getStyle(ByVal c As javax.swing.JComponent, ByVal r As javax.swing.plaf.synth.Region) As NimbusStyle
			Return CType(javax.swing.plaf.synth.SynthLookAndFeel.getStyle(c, r), NimbusStyle)
		End Function

		''' <summary>
		''' Return a short string that identifies this look and feel. This
		''' String will be the unquoted String "Nimbus".
		''' </summary>
		''' <returns> a short string identifying this look and feel. </returns>
		Public Property Overrides name As String
			Get
				Return "Nimbus"
			End Get
		End Property

		''' <summary>
		''' Return a string that identifies this look and feel. This String will
		''' be the unquoted String "Nimbus".
		''' </summary>
		''' <returns> a short string identifying this look and feel. </returns>
		Public Property Overrides iD As String
			Get
				Return "Nimbus"
			End Get
		End Property

		''' <summary>
		''' Returns a textual description of this look and feel.
		''' </summary>
		''' <returns> textual description of this look and feel. </returns>
		Public Property Overrides description As String
			Get
				Return "Nimbus Look and Feel"
			End Get
		End Property

		''' <summary>
		''' {@inheritDoc} </summary>
		''' <returns> {@code true} </returns>
		Public Overrides Function shouldUpdateStyleOnAncestorChanged() As Boolean
			Return True
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' 
		''' <p>Overridden to return {@code true} when one of the following
		''' properties change:
		''' <ul>
		'''   <li>{@code "Nimbus.Overrides"}
		'''   <li>{@code "Nimbus.Overrides.InheritDefaults"}
		'''   <li>{@code "JComponent.sizeVariant"}
		''' </ul>
		''' 
		''' @since 1.7
		''' </summary>
		Protected Friend Overrides Function shouldUpdateStyleOnEvent(ByVal ev As java.beans.PropertyChangeEvent) As Boolean
			Dim eName As String = ev.propertyName

			' These properties affect style cached inside NimbusDefaults (6860433)
			If "name" = eName OrElse "ancestor" = eName OrElse "Nimbus.Overrides" = eName OrElse "Nimbus.Overrides.InheritDefaults" = eName OrElse "JComponent.sizeVariant" = eName Then

				Dim c As javax.swing.JComponent = CType(ev.source, javax.swing.JComponent)
				defaults.clearOverridesCache(c)
				Return True
			End If

			Return MyBase.shouldUpdateStyleOnEvent(ev)
		End Function

		''' <summary>
		''' <p>Registers a third party component with the NimbusLookAndFeel.</p>
		''' 
		''' <p>Regions represent Components and areas within Components that act as
		''' independent painting areas. Once registered with the NimbusLookAndFeel,
		''' NimbusStyles for these Regions can be retrieved via the
		''' <code>getStyle</code> method.</p>
		''' 
		''' <p>The NimbusLookAndFeel uses a standard naming scheme for entries in the
		''' UIDefaults table. The key for each property, state, painter, and other
		''' default registered in UIDefaults for a specific Region will begin with
		''' the specified <code>prefix</code></p>
		''' 
		''' <p>For example, suppose I had a component named JFoo. Suppose I then registered
		''' this component with the NimbusLookAndFeel in this manner:</p>
		''' 
		''' <pre><code>
		'''     laf.register(NimbusFooUI.FOO_REGION, "Foo");
		''' </code></pre>
		''' 
		''' <p>In this case, I could then register properties for this component with
		''' UIDefaults in the following manner:</p>
		''' 
		''' <pre><code>
		'''     UIManager.put("Foo.background", new ColorUIResource(Color.BLACK));
		'''     UIManager.put("Foo.Enabled.backgroundPainter", new FooBackgroundPainter());
		''' </code></pre>
		''' 
		''' <p>It is also possible to register a named component with Nimbus.
		''' For example, suppose you wanted to style the background of a JPanel
		''' named "MyPanel" differently from other JPanels. You could accomplish this
		''' by doing the following:</p>
		''' 
		''' <pre><code>
		'''     laf.register(Region.PANEL, "\"MyPanel\"");
		'''     UIManager.put("\"MyPanel\".background", new ColorUIResource(Color.RED));
		''' </code></pre>
		''' </summary>
		''' <param name="region"> The Synth Region that is being registered. Such as Button, or
		'''        ScrollBarThumb, or NimbusFooUI.FOO_REGION. </param>
		''' <param name="prefix"> The UIDefault prefix. For example, could be ComboBox, or if
		'''        a named components, "MyComboBox", or even something like
		'''        ToolBar."MyComboBox"."ComboBox.arrowButton" </param>
		Public Overridable Sub register(ByVal region As javax.swing.plaf.synth.Region, ByVal prefix As String)
			defaults.register(region, prefix)
		End Sub

		''' <summary>
		''' Simple utility method that reads system keys.
		''' </summary>
		Private Function getSystemProperty(ByVal key As String) As String
			Return java.security.AccessController.doPrivileged(New sun.security.action.GetPropertyAction(key))
		End Function

		Public Overrides Function getDisabledIcon(ByVal component As javax.swing.JComponent, ByVal icon As javax.swing.Icon) As javax.swing.Icon
			If TypeOf icon Is sun.swing.plaf.synth.SynthIcon Then
				Dim si As sun.swing.plaf.synth.SynthIcon = CType(icon, sun.swing.plaf.synth.SynthIcon)
				Dim img As java.awt.image.BufferedImage = EffectUtils.createCompatibleTranslucentImage(si.iconWidth, si.iconHeight)
				Dim gfx As java.awt.Graphics2D = img.createGraphics()
				si.paintIcon(component, gfx, 0, 0)
				gfx.Dispose()
				Return New sun.swing.ImageIconUIResource(javax.swing.GrayFilter.createDisabledImage(img))
			Else
				Return MyBase.getDisabledIcon(component, icon)
			End If
		End Function

		''' <summary>
		''' Get a derived color, derived colors are shared instances and is color
		''' value will change when its parent UIDefault color changes.
		''' </summary>
		''' <param name="uiDefaultParentName"> The parent UIDefault key </param>
		''' <param name="hOffset">             The hue offset </param>
		''' <param name="sOffset">             The saturation offset </param>
		''' <param name="bOffset">             The brightness offset </param>
		''' <param name="aOffset">             The alpha offset </param>
		''' <param name="uiResource">          True if the derived color should be a
		'''                            UIResource, false if it should not be </param>
		''' <returns> The stored derived color </returns>
		Public Overridable Function getDerivedColor(ByVal uiDefaultParentName As String, ByVal hOffset As Single, ByVal sOffset As Single, ByVal bOffset As Single, ByVal aOffset As Integer, ByVal uiResource As Boolean) As java.awt.Color
			Return defaults.getDerivedColor(uiDefaultParentName, hOffset, sOffset, bOffset, aOffset, uiResource)
		End Function

		''' <summary>
		''' Decodes and returns a color, which is derived from an offset between two
		''' other colors.
		''' </summary>
		''' <param name="color1">   The first color </param>
		''' <param name="color2">   The second color </param>
		''' <param name="midPoint"> The offset between color 1 and color 2, a value of 0.0 is
		'''                 color 1 and 1.0 is color 2; </param>
		''' <param name="uiResource"> True if the derived color should be a UIResource </param>
		''' <returns> The derived color </returns>
		Protected Friend Function getDerivedColor(ByVal color1 As java.awt.Color, ByVal color2 As java.awt.Color, ByVal midPoint As Single, ByVal uiResource As Boolean) As java.awt.Color
			Dim argb As Integer = deriveARGB(color1, color2, midPoint)
			If uiResource Then
				Return New javax.swing.plaf.ColorUIResource(argb)
			Else
				Return New java.awt.Color(argb)
			End If
		End Function

		''' <summary>
		''' Decodes and returns a color, which is derived from a offset between two
		''' other colors.
		''' </summary>
		''' <param name="color1">   The first color </param>
		''' <param name="color2">   The second color </param>
		''' <param name="midPoint"> The offset between color 1 and color 2, a value of 0.0 is
		'''                 color 1 and 1.0 is color 2; </param>
		''' <returns> The derived color, which will be a UIResource </returns>
		Protected Friend Function getDerivedColor(ByVal color1 As java.awt.Color, ByVal color2 As java.awt.Color, ByVal midPoint As Single) As java.awt.Color
			Return getDerivedColor(color1, color2, midPoint, True)
		End Function

		''' <summary>
		''' Package private method which returns either BorderLayout.NORTH,
		''' BorderLayout.SOUTH, BorderLayout.EAST, or BorderLayout.WEST depending
		''' on the location of the toolbar in its parent. The toolbar might be
		''' in PAGE_START, PAGE_END, CENTER, or some other position, but will be
		''' resolved to either NORTH,SOUTH,EAST, or WEST based on where the toolbar
		''' actually IS, with CENTER being NORTH.
		''' 
		''' This code is used to determine where the border line should be drawn
		''' by the custom toolbar states, and also used by NimbusIcon to determine
		''' whether the handle icon needs to be shifted to look correct.
		''' 
		''' Toollbars are unfortunately odd in the way these things are handled,
		''' and so this code exists to unify the logic related to toolbars so it can
		''' be shared among the static files such as NimbusIcon and generated files
		''' such as the ToolBar state classes.
		''' </summary>
		Friend Shared Function resolveToolbarConstraint(ByVal toolbar As javax.swing.JToolBar) As Object
			'NOTE: we don't worry about component orientation or PAGE_END etc
			'because the BasicToolBarUI always uses an absolute position of
			'NORTH/SOUTH/EAST/WEST.
			If toolbar IsNot Nothing Then
				Dim parent As java.awt.Container = toolbar.parent
				If parent IsNot Nothing Then
					Dim m As java.awt.LayoutManager = parent.layout
					If TypeOf m Is java.awt.BorderLayout Then
						Dim b As java.awt.BorderLayout = CType(m, java.awt.BorderLayout)
						Dim con As Object = b.getConstraints(toolbar)
						If con Is SOUTH OrElse con Is EAST OrElse con Is WEST Then Return con
						Return NORTH
					End If
				End If
			End If
			Return NORTH
		End Function

		''' <summary>
		''' Derives the ARGB value for a color based on an offset between two
		''' other colors.
		''' </summary>
		''' <param name="color1">   The first color </param>
		''' <param name="color2">   The second color </param>
		''' <param name="midPoint"> The offset between color 1 and color 2, a value of 0.0 is
		'''                 color 1 and 1.0 is color 2; </param>
		''' <returns> the ARGB value for a new color based on this derivation </returns>
		Friend Shared Function deriveARGB(ByVal color1 As java.awt.Color, ByVal color2 As java.awt.Color, ByVal midPoint As Single) As Integer
			Dim r As Integer = color1.red + Math.Round((color2.red - color1.red) * midPoint)
			Dim g As Integer = color1.green + Math.Round((color2.green - color1.green) * midPoint)
			Dim b As Integer = color1.blue + Math.Round((color2.blue - color1.blue) * midPoint)
			Dim a As Integer = color1.alpha + Math.Round((color2.alpha - color1.alpha) * midPoint)
			Return ((a And &HFF) << 24) Or ((r And &HFF) << 16) Or ((g And &HFF) << 8) Or (b And &HFF)
		End Function

		''' <summary>
		''' Simple Symbolic Link style UIDefalts Property
		''' </summary>
		Private Class LinkProperty
			Implements javax.swing.UIDefaults.ActiveValue, javax.swing.plaf.UIResource

			Private ReadOnly outerInstance As NimbusLookAndFeel

			Private dstPropName As String

			Private Sub New(ByVal outerInstance As NimbusLookAndFeel, ByVal dstPropName As String)
					Me.outerInstance = outerInstance
				Me.dstPropName = dstPropName
			End Sub

			Public Overrides Function createValue(ByVal table As javax.swing.UIDefaults) As Object
				Return javax.swing.UIManager.get(dstPropName)
			End Function
		End Class

		''' <summary>
		''' Nimbus Property that looks up Nimbus keys for standard key names. For
		''' example "Button.background" --> "Button[Enabled].backgound"
		''' </summary>
		Private Class NimbusProperty
			Implements javax.swing.UIDefaults.ActiveValue, javax.swing.plaf.UIResource

			Private ReadOnly outerInstance As NimbusLookAndFeel

			Private prefix As String
			Private state As String = Nothing
			Private suffix As String
			Private isFont As Boolean

			Private Sub New(ByVal outerInstance As NimbusLookAndFeel, ByVal prefix As String, ByVal suffix As String)
					Me.outerInstance = outerInstance
				Me.prefix = prefix
				Me.suffix = suffix
				isFont = "font".Equals(suffix)
			End Sub

			Private Sub New(ByVal outerInstance As NimbusLookAndFeel, ByVal prefix As String, ByVal ___state As String, ByVal suffix As String)
					Me.outerInstance = outerInstance
				Me.New(prefix,suffix)
				Me.state = ___state
			End Sub

			''' <summary>
			''' Creates the value retrieved from the <code>UIDefaults</code> table.
			''' The object is created each time it is accessed.
			''' </summary>
			''' <param name="table"> a <code>UIDefaults</code> table </param>
			''' <returns> the created <code>Object</code> </returns>
			Public Overrides Function createValue(ByVal table As javax.swing.UIDefaults) As Object
				Dim obj As Object = Nothing
				' check specified state
				If state IsNot Nothing Then obj = outerInstance.uiDefaults(prefix & "[" & state & "]." & suffix)
				' check enabled state
				If obj Is Nothing Then obj = outerInstance.uiDefaults(prefix & "[Enabled]." & suffix)
				' check for defaults
				If obj Is Nothing Then
					If isFont Then
						obj = outerInstance.uiDefaults("defaultFont")
					Else
						obj = outerInstance.uiDefaults(suffix)
					End If
				End If
				Return obj
			End Function
		End Class

		Private compiledDefaults As IDictionary(Of String, IDictionary(Of String, Object)) = Nothing
		Private defaultListenerAdded As Boolean = False

		Friend Shared Function parsePrefix(ByVal key As String) As String
			If key Is Nothing Then Return Nothing
			Dim inquotes As Boolean = False
			For i As Integer = 0 To key.Length - 1
				Dim c As Char = key.Chars(i)
				If c = """"c Then
					inquotes = Not inquotes
				ElseIf (c = "["c OrElse c = "."c) AndAlso (Not inquotes) Then
					Return key.Substring(0, i)
				End If
			Next i
			Return Nothing
		End Function

		Friend Overridable Function getDefaultsForPrefix(ByVal prefix As String) As IDictionary(Of String, Object)
			If compiledDefaults Is Nothing Then
				compiledDefaults = New Dictionary(Of String, IDictionary(Of String, Object))
				For Each entry As KeyValuePair(Of Object, Object) In javax.swing.UIManager.defaults
					If TypeOf entry.Key Is String Then addDefault(CStr(entry.Key), entry.Value)
				Next entry
				If Not defaultListenerAdded Then
					javax.swing.UIManager.defaults.addPropertyChangeListener(defaultsListener)
					defaultListenerAdded = True
				End If
			End If
			Return compiledDefaults(prefix)
		End Function

		Private Sub addDefault(ByVal key As String, ByVal value As Object)
			If compiledDefaults Is Nothing Then Return

			Dim prefix As String = parsePrefix(key)
			If prefix IsNot Nothing Then
				Dim keys As IDictionary(Of String, Object) = compiledDefaults(prefix)
				If keys Is Nothing Then
					keys = New Dictionary(Of String, Object)
					compiledDefaults(prefix) = keys
				End If
				keys(key) = value
			End If
		End Sub

		Private Class DefaultsListener
			Implements java.beans.PropertyChangeListener

			Private ReadOnly outerInstance As NimbusLookAndFeel

			Public Sub New(ByVal outerInstance As NimbusLookAndFeel)
				Me.outerInstance = outerInstance
			End Sub

			Public Overrides Sub propertyChange(ByVal ev As java.beans.PropertyChangeEvent)
				Dim key As String = ev.propertyName
				If "UIDefaults".Equals(key) Then
					outerInstance.compiledDefaults = Nothing
				Else
					outerInstance.addDefault(key, ev.newValue)
				End If
			End Sub
		End Class
	End Class

End Namespace
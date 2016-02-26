Imports Microsoft.VisualBasic
Imports System
Imports System.Diagnostics
Imports System.Collections.Generic
Imports javax.swing.plaf.synth.SynthConstants

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
	''' <p>A SynthStyle implementation used by Nimbus. Each Region that has been
	''' registered with the NimbusLookAndFeel will have an associated NimbusStyle.
	''' Third party components that are registered with the NimbusLookAndFeel will
	''' therefore be handed a NimbusStyle from the look and feel from the
	''' #getStyle(JComponent, Region) method.</p>
	''' 
	''' <p>This class properly reads and retrieves values placed in the UIDefaults
	''' according to the standard Nimbus naming conventions. It will create and
	''' retrieve painters, fonts, colors, and other data stored there.</p>
	''' 
	''' <p>NimbusStyle also supports the ability to override settings on a per
	''' component basis. NimbusStyle checks the component's client property map for
	''' "Nimbus.Overrides". If the value associated with this key is an instance of
	''' UIDefaults, then the values in that defaults table will override the standard
	''' Nimbus defaults in UIManager, but for that component instance only.</p>
	''' 
	''' <p>Optionally, you may specify the client property
	''' "Nimbus.Overrides.InheritDefaults". If true, this client property indicates
	''' that the defaults located in UIManager should first be read, and then
	''' replaced with defaults located in the component client properties. If false,
	''' then only the defaults located in the component client property map will
	''' be used. If not specified, it is assumed to be true.</p>
	''' 
	''' <p>You must specify "Nimbus.Overrides" for "Nimbus.Overrides.InheritDefaults"
	''' to have any effect. "Nimbus.Overrides" indicates whether there are any
	''' overrides, while "Nimbus.Overrides.InheritDefaults" indicates whether those
	''' overrides should first be initialized with the defaults from UIManager.</p>
	''' 
	''' <p>The NimbusStyle is reloaded whenever a property change event is fired
	''' for a component for "Nimbus.Overrides" or "Nimbus.Overrides.InheritDefaults".
	''' So for example, setting a new UIDefaults on a component would cause the
	''' style to be reloaded.</p>
	''' 
	''' <p>The values are only read out of UIManager once, and then cached. If
	''' you need to read the values again (for example, if the UI is being reloaded),
	''' then discard this NimbusStyle and read a new one from NimbusLookAndFeel
	''' using NimbusLookAndFeel.getStyle.</p>
	''' 
	''' <p>The primary API of interest in this class for 3rd party component authors
	''' are the three methods which retrieve painters: #getBackgroundPainter,
	''' #getForegroundPainter, and #getBorderPainter.</p>
	''' 
	''' <p>NimbusStyle allows you to specify custom states, or modify the order of
	''' states. Synth (and thus Nimbus) has the concept of a "state". For example,
	''' a JButton might be in the "MOUSE_OVER" state, or the "ENABLED" state, or the
	''' "DISABLED" state. These are all "standard" states which are defined in synth,
	''' and which apply to all synth Regions.</p>
	''' 
	''' <p>Sometimes, however, you need to have a custom state. For example, you
	''' want JButton to render differently if it's parent is a JToolbar. In Nimbus,
	''' you specify these custom states by including a special key in UIDefaults.
	''' The following UIDefaults entries define three states for this button:</p>
	''' 
	''' <pre><code>
	'''     JButton.States = Enabled, Disabled, Toolbar
	'''     JButton[Enabled].backgroundPainter = somePainter
	'''     JButton[Disabled].background = BLUE
	'''     JButton[Toolbar].backgroundPainter = someOtherPaint
	''' </code></pre>
	''' 
	''' <p>As you can see, the <code>JButton.States</code> entry lists the states
	''' that the JButton style will support. You then specify the settings for
	''' each state. If you do not specify the <code>JButton.States</code> entry,
	''' then the standard Synth states will be assumed. If you specify the entry
	''' but the list of states is empty or null, then the standard synth states
	''' will be assumed.</p>
	''' 
	''' @author Richard Bair
	''' @author Jasper Potts
	''' </summary>
	Public NotInheritable Class NimbusStyle
		Inherits javax.swing.plaf.synth.SynthStyle

		' Keys and scales for large/small/mini components, based on Apples sizes 
		Public Const LARGE_KEY As String = "large"
		Public Const SMALL_KEY As String = "small"
		Public Const MINI_KEY As String = "mini"
		Public Const LARGE_SCALE As Double = 1.15
		Public Const SMALL_SCALE As Double = 0.857
		Public Const MINI_SCALE As Double = 0.714

		''' <summary>
		''' Special constant used for performance reasons during the get() method.
		''' If get() runs through all of the search locations and determines that
		''' there is no value, then NULL will be placed into the values map. This way
		''' on subsequent lookups it will simply extract NULL, see it, and return
		''' null rather than continuing the lookup procedure.
		''' </summary>
		Private Shared ReadOnly NULL As Object = ControlChars.NullChar
		''' <summary>
		''' <p>The Color to return from getColorForState if it would otherwise have
		''' returned null.</p>
		''' 
		''' <p>Returning null from getColorForState is a very bad thing, as it causes
		''' the AWT peer for the component to install a SystemColor, which is not a
		''' UIResource. As a result, if <code>null</code> is returned from
		''' getColorForState, then thereafter the color is not updated for other
		''' states or on LAF changes or updates. This DEFAULT_COLOR is used to
		''' ensure that a ColorUIResource is always returned from
		''' getColorForState.</p>
		''' </summary>
		Private Shared ReadOnly DEFAULT_COLOR As java.awt.Color = New javax.swing.plaf.ColorUIResource(java.awt.Color.BLACK)
		''' <summary>
		''' Simple Comparator for ordering the RuntimeStates according to their
		''' rank.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'		private static final java.util.Comparator<RuntimeState> STATE_COMPARATOR = New java.util.Comparator<RuntimeState>()
	'	{
	'			@Override public int compare(RuntimeState a, RuntimeState b)
	'			{
	'				Return a.state - b.state;
	'			}
	'		};
		''' <summary>
		''' The prefix for the component or region that this NimbusStyle
		''' represents. This prefix is used to lookup state in the UIManager.
		''' It should be something like Button or Slider.Thumb or "MyButton" or
		''' ComboBox."ComboBox.arrowButton" or "MyComboBox"."ComboBox.arrowButton"
		''' </summary>
		Private prefix As String
		''' <summary>
		''' The SynthPainter that will be returned from this NimbusStyle. The
		''' SynthPainter returned will be a SynthPainterImpl, which will in turn
		''' delegate back to this NimbusStyle for the proper Painter (not
		''' SynthPainter) to use for painting the foreground, background, or border.
		''' </summary>
		Private painter As javax.swing.plaf.synth.SynthPainter
		''' <summary>
		''' Data structure containing all of the defaults, insets, states, and other
		''' values associated with this style. This instance refers to default
		''' values, and are used when no overrides are discovered in the client
		''' properties of a component. These values are lazily created on first
		''' access.
		''' </summary>
		Private values As Values

		''' <summary>
		''' A temporary CacheKey used to perform lookups. This pattern avoids
		''' creating useless garbage keys, or concatenating strings, etc.
		''' </summary>
		Private tmpKey As New CacheKey("", 0)

		''' <summary>
		''' Some NimbusStyles are created for a specific component only. In Nimbus,
		''' this happens whenever the component has as a client property a
		''' UIDefaults which overrides (or supplements) those defaults found in
		''' UIManager.
		''' </summary>
		Private component As WeakReference(Of javax.swing.JComponent)

		''' <summary>
		''' Create a new NimbusStyle. Only the prefix must be supplied. At the
		''' appropriate time, installDefaults will be called. At that point, all of
		''' the state information will be pulled from UIManager and stored locally
		''' within this style.
		''' </summary>
		''' <param name="prefix"> Something like Button or Slider.Thumb or
		'''        org.jdesktop.swingx.JXStatusBar or ComboBox."ComboBox.arrowButton" </param>
		''' <param name="c"> an optional reference to a component that this NimbusStyle
		'''        should be associated with. This is only used when the component
		'''        has Nimbus overrides registered in its client properties and
		'''        should be null otherwise. </param>
		Friend Sub New(ByVal prefix As String, ByVal c As javax.swing.JComponent)
			If c IsNot Nothing Then Me.component = New WeakReference(Of javax.swing.JComponent)(c)
			Me.prefix = prefix
			Me.painter = New SynthPainterImpl(Me)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' 
		''' Overridden to cause this style to populate itself with data from
		''' UIDefaults, if necessary.
		''' </summary>
		Public Overrides Sub installDefaults(ByVal ctx As javax.swing.plaf.synth.SynthContext)
			validate()

			'delegate to the superclass to install defaults such as background,
			'foreground, font, and opaque onto the swing component.
			MyBase.installDefaults(ctx)
		End Sub

		''' <summary>
		''' Pulls data out of UIDefaults, if it has not done so already, and sets
		''' up the internal state.
		''' </summary>
		Private Sub validate()
			' a non-null values object is the flag we use to determine whether
			' to reparse from UIManager.
			If values IsNot Nothing Then Return

			' reconstruct this NimbusStyle based on the entries in the UIManager
			' and possibly based on any overrides within the component's
			' client properties (assuming such a component exists and contains
			' any Nimbus.Overrides)
			values = New Values

			Dim defaults As IDictionary(Of String, Object) = CType(javax.swing.UIManager.lookAndFeel, NimbusLookAndFeel).getDefaultsForPrefix(prefix)

			' inspect the client properties for the key "Nimbus.Overrides". If the
			' value is an instance of UIDefaults, then these defaults are used
			' in place of, or in addition to, the defaults in UIManager.
			If component IsNot Nothing Then
				' We know component.get() is non-null here, as if the component
				' were GC'ed, we wouldn't be processing its style.
				Dim o As Object = component.get().getClientProperty("Nimbus.Overrides")
				If TypeOf o Is javax.swing.UIDefaults Then
					Dim i As Object = component.get().getClientProperty("Nimbus.Overrides.InheritDefaults")
					Dim inherit As Boolean = If(TypeOf i Is Boolean?, CBool(i), True)
					Dim d As javax.swing.UIDefaults = CType(o, javax.swing.UIDefaults)
					Dim map As New SortedDictionary(Of String, Object)
					For Each obj As Object In d.Keys
						If TypeOf obj Is String Then
							Dim key As String = CStr(obj)
							If key.StartsWith(prefix) Then map(key) = d(key)
						End If
					Next obj
					If inherit Then
'JAVA TO VB CONVERTER TODO TASK: There is no .NET Dictionary equivalent to the Java 'putAll' method:
						defaults.putAll(map)
					Else
						defaults = map
					End If
				End If
			End If

			'a list of the different types of states used by this style. This
			'list may contain only "standard" states (those defined by Synth),
			'or it may contain custom states, or it may contain only "standard"
			'states but list them in a non-standard order.
			Dim states As IList(Of State) = New List(Of State)
			'a map of state name to code
			Dim stateCodes As IDictionary(Of String, Integer?) = New Dictionary(Of String, Integer?)
			'This is a list of runtime "state" context objects. These contain
			'the values associated with each state.
			Dim runtimeStates As IList(Of RuntimeState) = New List(Of RuntimeState)

			'determine whether there are any custom states, or custom state
			'order. If so, then read all those custom states and define the
			'"values" stateTypes to be a non-null array.
			'Otherwise, let the "values" stateTypes be null to indicate that
			'there are no custom states or custom state ordering
			Dim statesString As String = CStr(defaults(prefix & ".States"))
			If statesString IsNot Nothing Then
				Dim s As String() = StringHelperClass.StringSplit(statesString, ",", True)
				For i As Integer = 0 To s.Length - 1
					s(i) = s(i).Trim()
					If Not State.isStandardStateName(s(i)) Then
						'this is a non-standard state name, so look for the
						'custom state associated with it
						Dim stateName As String = prefix & "." & s(i)
						Dim customState As State = CType(defaults(stateName), State)
						If customState IsNot Nothing Then states.Add(customState)
					Else
						states.Add(State.getStandardState(s(i)))
					End If
				Next i

				'if there were any states defined, then set the stateTypes array
				'to be non-null. Otherwise, leave it null (meaning, use the
				'standard synth states).
				If states.Count > 0 Then values.stateTypes = states.ToArray()

				'assign codes for each of the state types
				Dim code As Integer = 1
				For Each ___state As State In states
					stateCodes(___state.name) = code
					code <<= 1
				Next ___state
			Else
				'since there were no custom states defined, setup the list of
				'standard synth states. Note that the "v.stateTypes" is not
				'being set here, indicating that at runtime the state selection
				'routines should use standard synth states instead of custom
				'states. I do need to popuplate this temp list now though, so that
				'the remainder of this method will function as expected.
				states.Add(State.Enabled)
				states.Add(State.MouseOver)
				states.Add(State.Pressed)
				states.Add(State.Disabled)
				states.Add(State.Focused)
				states.Add(State.Selected)
				states.Add(State.Default)

				'assign codes for the states
				stateCodes("Enabled") = ENABLED
				stateCodes("MouseOver") = MOUSE_OVER
				stateCodes("Pressed") = PRESSED
				stateCodes("Disabled") = DISABLED
				stateCodes("Focused") = FOCUSED
				stateCodes("Selected") = SELECTED
				stateCodes("Default") = DEFAULT
			End If

			'Now iterate over all the keys in the defaults table
			For Each key As String In defaults.Keys
				'The key is something like JButton.Enabled.backgroundPainter,
				'or JButton.States, or JButton.background.
				'Remove the "JButton." portion of the key
				Dim temp As String = key.Substring(prefix.Length)
				'if there is a " or : then we skip it because it is a subregion
				'of some kind
				If temp.IndexOf(""""c) <> -1 OrElse temp.IndexOf(":"c) <> -1 Then Continue For
				'remove the separator
				temp = temp.Substring(1)
				'At this point, temp may be any of the following:
				'background
				'[Enabled].background
				'[Enabled+MouseOver].background
				'property.foo

				'parse out the states and the property
				Dim stateString As String = Nothing
				Dim [property] As String = Nothing
				Dim bracketIndex As Integer = temp.IndexOf("]"c)
				If bracketIndex < 0 Then
					'there is not a state string, so property = temp
					[property] = temp
				Else
					stateString = temp.Substring(0, bracketIndex)
					[property] = temp.Substring(bracketIndex + 2)
				End If

				'now that I have the state (if any) and the property, get the
				'value for this property and install it where it belongs
				If stateString Is Nothing Then
					'there was no state, just a property. Check for the custom
					'"contentMargins" property (which is handled specially by
					'Synth/Nimbus). Also check for the property being "States",
					'in which case it is not a real property and should be ignored.
					'otherwise, assume it is a property and install it on the
					'values object
					If "contentMargins".Equals([property]) Then
						values.contentMargins = CType(defaults(key), java.awt.Insets)
					ElseIf "States".Equals([property]) Then
						'ignore
					Else
						values.defaults([property]) = defaults(key)
					End If
				Else
					'it is possible that the developer has a malformed UIDefaults
					'entry, such that something was specified in the place of
					'the State portion of the key but it wasn't a state. In this
					'case, skip will be set to true
					Dim skip As Boolean = False
					'this variable keeps track of the int value associated with
					'the state. See SynthState for details.
					Dim componentState As Integer = 0
					'Multiple states may be specified in the string, such as
					'Enabled+MouseOver
					Dim stateParts As String() = StringHelperClass.StringSplit(stateString, "\+", True)
					'For each state, we need to find the State object associated
					'with it, or skip it if it cannot be found.
					For Each s As String In stateParts
						If stateCodes.ContainsKey(s) Then
							componentState = componentState Or stateCodes(s)
						Else
							'Was not a state. Maybe it was a subregion or something
							'skip it.
							skip = True
							Exit For
						End If
					Next s

					If skip Then Continue For

					'find the RuntimeState for this State
					Dim rs As RuntimeState = Nothing
					For Each s As RuntimeState In runtimeStates
						If s.state = componentState Then
							rs = s
							Exit For
						End If
					Next s

					'couldn't find the runtime state, so create a new one
					If rs Is Nothing Then
						rs = New RuntimeState(Me, componentState, stateString)
						runtimeStates.Add(rs)
					End If

					'check for a couple special properties, such as for the
					'painters. If these are found, then set the specially on
					'the runtime state. Else, it is just a normal property,
					'so put it in the UIDefaults associated with that runtime
					'state
					If "backgroundPainter".Equals([property]) Then
						rs.backgroundPainter = getPainter(defaults, key)
					ElseIf "foregroundPainter".Equals([property]) Then
						rs.foregroundPainter = getPainter(defaults, key)
					ElseIf "borderPainter".Equals([property]) Then
						rs.borderPainter = getPainter(defaults, key)
					Else
						rs.defaults([property]) = defaults(key)
					End If
				End If
			Next key

			'now that I've collected all the runtime states, I'll sort them based
			'on their integer "state" (see SynthState for how this works).
			java.util.Collections.sort(runtimeStates, STATE_COMPARATOR)

			'finally, set the array of runtime states on the values object
			values.states = runtimeStates.ToArray()
		End Sub

		Private Function getPainter(ByVal defaults As IDictionary(Of String, Object), ByVal key As String) As javax.swing.Painter
			Dim p As Object = defaults(key)
			If TypeOf p Is javax.swing.UIDefaults.LazyValue Then p = CType(p, javax.swing.UIDefaults.LazyValue).createValue(javax.swing.UIManager.defaults)
			Return (If(TypeOf p Is javax.swing.Painter, CType(p, javax.swing.Painter), Nothing))
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' 
		''' Overridden to cause this style to populate itself with data from
		''' UIDefaults, if necessary.
		''' </summary>
		Public Overrides Function getInsets(ByVal ctx As javax.swing.plaf.synth.SynthContext, ByVal [in] As java.awt.Insets) As java.awt.Insets
			If [in] Is Nothing Then [in] = New java.awt.Insets(0, 0, 0, 0)

			Dim v As Values = getValues(ctx)

			If v.contentMargins Is Nothing Then
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
					= 0
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
					[in].bottom = [in].top = [in].left = [in].right
				Return [in]
			Else
				[in].bottom = v.contentMargins.bottom
				[in].top = v.contentMargins.top
				[in].left = v.contentMargins.left
				[in].right = v.contentMargins.right
				' Account for scale
				' The key "JComponent.sizeVariant" is used to match Apple's LAF
				Dim scaleKey As String = CStr(ctx.component.getClientProperty("JComponent.sizeVariant"))
				If scaleKey IsNot Nothing Then
					If LARGE_KEY.Equals(scaleKey) Then
						[in].bottom *= LARGE_SCALE
						[in].top *= LARGE_SCALE
						[in].left *= LARGE_SCALE
						[in].right *= LARGE_SCALE
					ElseIf SMALL_KEY.Equals(scaleKey) Then
						[in].bottom *= SMALL_SCALE
						[in].top *= SMALL_SCALE
						[in].left *= SMALL_SCALE
						[in].right *= SMALL_SCALE
					ElseIf MINI_KEY.Equals(scaleKey) Then
						[in].bottom *= MINI_SCALE
						[in].top *= MINI_SCALE
						[in].left *= MINI_SCALE
						[in].right *= MINI_SCALE
					End If
				End If
				Return [in]
			End If
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' 
		''' <p>Overridden to cause this style to populate itself with data from
		''' UIDefaults, if necessary.</p>
		''' 
		''' <p>In addition, NimbusStyle handles ColorTypes slightly differently from
		''' Synth.</p>
		''' <ul>
		'''  <li>ColorType.BACKGROUND will equate to the color stored in UIDefaults
		'''      named "background".</li>
		'''  <li>ColorType.TEXT_BACKGROUND will equate to the color stored in
		'''      UIDefaults named "textBackground".</li>
		'''  <li>ColorType.FOREGROUND will equate to the color stored in UIDefaults
		'''      named "textForeground".</li>
		'''  <li>ColorType.TEXT_FOREGROUND will equate to the color stored in
		'''      UIDefaults named "textForeground".</li>
		''' </ul>
		''' </summary>
		Protected Friend Overrides Function getColorForState(ByVal ctx As javax.swing.plaf.synth.SynthContext, ByVal type As javax.swing.plaf.synth.ColorType) As java.awt.Color
			Dim key As String = Nothing
			If type Is javax.swing.plaf.synth.ColorType.BACKGROUND Then
				key = "background"
			ElseIf type Is javax.swing.plaf.synth.ColorType.FOREGROUND Then
				'map FOREGROUND as TEXT_FOREGROUND
				key = "textForeground"
			ElseIf type Is javax.swing.plaf.synth.ColorType.TEXT_BACKGROUND Then
				key = "textBackground"
			ElseIf type Is javax.swing.plaf.synth.ColorType.TEXT_FOREGROUND Then
				key = "textForeground"
			ElseIf type Is javax.swing.plaf.synth.ColorType.FOCUS Then
				key = "focus"
			ElseIf type IsNot Nothing Then
				key = type.ToString()
			Else
				Return DEFAULT_COLOR
			End If
			Dim c As java.awt.Color = CType([get](ctx, key), java.awt.Color)
			'if all else fails, return a default color (which is a ColorUIResource)
			If c Is Nothing Then c = DEFAULT_COLOR
			Return c
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' 
		''' Overridden to cause this style to populate itself with data from
		''' UIDefaults, if necessary. If a value named "font" is not found in
		''' UIDefaults, then the "defaultFont" font in UIDefaults will be returned
		''' instead.
		''' </summary>
		Protected Friend Overrides Function getFontForState(ByVal ctx As javax.swing.plaf.synth.SynthContext) As java.awt.Font
			Dim f As java.awt.Font = CType([get](ctx, "font"), java.awt.Font)
			If f Is Nothing Then f = javax.swing.UIManager.getFont("defaultFont")

			' Account for scale
			' The key "JComponent.sizeVariant" is used to match Apple's LAF
			Dim scaleKey As String = CStr(ctx.component.getClientProperty("JComponent.sizeVariant"))
			If scaleKey IsNot Nothing Then
				If LARGE_KEY.Equals(scaleKey) Then
					f = f.deriveFont(Math.Round(f.size2D*LARGE_SCALE))
				ElseIf SMALL_KEY.Equals(scaleKey) Then
					f = f.deriveFont(Math.Round(f.size2D*SMALL_SCALE))
				ElseIf MINI_KEY.Equals(scaleKey) Then
					f = f.deriveFont(Math.Round(f.size2D*MINI_SCALE))
				End If
			End If
			Return f
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' 
		''' Returns the SynthPainter for this style, which ends up delegating to
		''' the Painters installed in this style.
		''' </summary>
		Public Overrides Function getPainter(ByVal ctx As javax.swing.plaf.synth.SynthContext) As javax.swing.plaf.synth.SynthPainter
			Return painter
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' 
		''' Overridden to cause this style to populate itself with data from
		''' UIDefaults, if necessary. If opacity is not specified in UI defaults,
		''' then it defaults to being non-opaque.
		''' </summary>
		Public Overrides Function isOpaque(ByVal ctx As javax.swing.plaf.synth.SynthContext) As Boolean
			' Force Table CellRenderers to be opaque
			If "Table.cellRenderer".Equals(ctx.component.name) Then Return True
			Dim ___opaque As Boolean? = CBool([get](ctx, "opaque"))
			Return If(___opaque Is Nothing, False, ___opaque)
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' 
		''' <p>Overridden to cause this style to populate itself with data from
		''' UIDefaults, if necessary.</p>
		''' 
		''' <p>Properties in UIDefaults may be specified in a chained manner. For
		''' example:
		''' <pre>
		''' background
		''' Button.opacity
		''' Button.Enabled.foreground
		''' Button.Enabled+Selected.background
		''' </pre>
		''' 
		''' <p>In this example, suppose you were in the Enabled+Selected state and
		''' searched for "foreground". In this case, we first check for
		''' Button.Enabled+Selected.foreground, but no such color exists. We then
		''' fall back to the next valid state, in this case,
		''' Button.Enabled.foreground, and have a match. So we return it.</p>
		''' 
		''' <p>Again, if we were in the state Enabled and looked for "background", we
		''' wouldn't find it in Button.Enabled, or in Button, but would at the top
		''' level in UIManager. So we return that value.</p>
		''' 
		''' <p>One special note: the "key" passed to this method could be of the form
		''' "background" or "Button.background" where "Button" equals the prefix
		''' passed to the NimbusStyle constructor. In either case, it looks for
		''' "background".</p>
		''' </summary>
		''' <param name="ctx"> </param>
		''' <param name="key"> must not be null </param>
		Public Overrides Function [get](ByVal ctx As javax.swing.plaf.synth.SynthContext, ByVal key As Object) As Object
			Dim v As Values = getValues(ctx)

			' strip off the prefix, if there is one.
			Dim fullKey As String = key.ToString()
			Dim partialKey As String = fullKey.Substring(fullKey.IndexOf(".") + 1)

			Dim obj As Object = Nothing
			Dim xstate As Integer = getExtendedState(ctx, v)

			' check the cache
			tmpKey.init(partialKey, xstate)
			obj = v.cache(tmpKey)
			Dim wasInCache As Boolean = obj IsNot Nothing
			If Not wasInCache Then
				' Search exact matching states and then lesser matching states
				Dim s As RuntimeState = Nothing
				Dim lastIndex As Integer() = {-1}
				s = getNextState(v.states, lastIndex, xstate)
				Do While obj Is Nothing AndAlso s IsNot Nothing
					obj = s.defaults(partialKey)
					s = getNextState(v.states, lastIndex, xstate)
				Loop
				' Search Region Defaults
				If obj Is Nothing AndAlso v.defaults IsNot Nothing Then obj = v.defaults(partialKey)
				' return found object
				' Search UIManager Defaults
				If obj Is Nothing Then obj = javax.swing.UIManager.get(fullKey)
				' Search Synth Defaults for InputMaps
				If obj Is Nothing AndAlso partialKey.Equals("focusInputMap") Then obj = MyBase.get(ctx, fullKey)
				' if all we got was a null, store this fact for later use
				v.cache(New CacheKey(partialKey, xstate)) = If(obj Is Nothing, NULL, obj)
			End If
			' return found object
			Return If(obj Is NULL, Nothing, obj)
		End Function

		''' <summary>
		''' Gets the appropriate background Painter, if there is one, for the state
		''' specified in the given SynthContext. This method does appropriate
		''' fallback searching, as described in #get.
		''' </summary>
		''' <param name="ctx"> The SynthContext. Must not be null. </param>
		''' <returns> The background painter associated for the given state, or null if
		''' none could be found. </returns>
		Public Function getBackgroundPainter(ByVal ctx As javax.swing.plaf.synth.SynthContext) As javax.swing.Painter
			Dim v As Values = getValues(ctx)
			Dim xstate As Integer = getExtendedState(ctx, v)
			Dim p As javax.swing.Painter = Nothing

			' check the cache
			tmpKey.init("backgroundPainter$$instance", xstate)
			p = CType(v.cache(tmpKey), javax.swing.Painter)
			If p IsNot Nothing Then Return p

			' not in cache, so lookup and store in cache
			Dim s As RuntimeState = Nothing
			Dim lastIndex As Integer() = {-1}
			s = getNextState(v.states, lastIndex, xstate)
			Do While s IsNot Nothing
				If s.backgroundPainter IsNot Nothing Then
					p = s.backgroundPainter
					Exit Do
				End If
				s = getNextState(v.states, lastIndex, xstate)
			Loop
			If p Is Nothing Then p = CType([get](ctx, "backgroundPainter"), javax.swing.Painter)
			If p IsNot Nothing Then v.cache(New CacheKey("backgroundPainter$$instance", xstate)) = p
			Return p
		End Function

		''' <summary>
		''' Gets the appropriate foreground Painter, if there is one, for the state
		''' specified in the given SynthContext. This method does appropriate
		''' fallback searching, as described in #get.
		''' </summary>
		''' <param name="ctx"> The SynthContext. Must not be null. </param>
		''' <returns> The foreground painter associated for the given state, or null if
		''' none could be found. </returns>
		Public Function getForegroundPainter(ByVal ctx As javax.swing.plaf.synth.SynthContext) As javax.swing.Painter
			Dim v As Values = getValues(ctx)
			Dim xstate As Integer = getExtendedState(ctx, v)
			Dim p As javax.swing.Painter = Nothing

			' check the cache
			tmpKey.init("foregroundPainter$$instance", xstate)
			p = CType(v.cache(tmpKey), javax.swing.Painter)
			If p IsNot Nothing Then Return p

			' not in cache, so lookup and store in cache
			Dim s As RuntimeState = Nothing
			Dim lastIndex As Integer() = {-1}
			s = getNextState(v.states, lastIndex, xstate)
			Do While s IsNot Nothing
				If s.foregroundPainter IsNot Nothing Then
					p = s.foregroundPainter
					Exit Do
				End If
				s = getNextState(v.states, lastIndex, xstate)
			Loop
			If p Is Nothing Then p = CType([get](ctx, "foregroundPainter"), javax.swing.Painter)
			If p IsNot Nothing Then v.cache(New CacheKey("foregroundPainter$$instance", xstate)) = p
			Return p
		End Function

		''' <summary>
		''' Gets the appropriate border Painter, if there is one, for the state
		''' specified in the given SynthContext. This method does appropriate
		''' fallback searching, as described in #get.
		''' </summary>
		''' <param name="ctx"> The SynthContext. Must not be null. </param>
		''' <returns> The border painter associated for the given state, or null if
		''' none could be found. </returns>
		Public Function getBorderPainter(ByVal ctx As javax.swing.plaf.synth.SynthContext) As javax.swing.Painter
			Dim v As Values = getValues(ctx)
			Dim xstate As Integer = getExtendedState(ctx, v)
			Dim p As javax.swing.Painter = Nothing

			' check the cache
			tmpKey.init("borderPainter$$instance", xstate)
			p = CType(v.cache(tmpKey), javax.swing.Painter)
			If p IsNot Nothing Then Return p

			' not in cache, so lookup and store in cache
			Dim s As RuntimeState = Nothing
			Dim lastIndex As Integer() = {-1}
			s = getNextState(v.states, lastIndex, xstate)
			Do While s IsNot Nothing
				If s.borderPainter IsNot Nothing Then
					p = s.borderPainter
					Exit Do
				End If
				s = getNextState(v.states, lastIndex, xstate)
			Loop
			If p Is Nothing Then p = CType([get](ctx, "borderPainter"), javax.swing.Painter)
			If p IsNot Nothing Then v.cache(New CacheKey("borderPainter$$instance", xstate)) = p
			Return p
		End Function

		''' <summary>
		''' Utility method which returns the proper Values based on the given
		''' SynthContext. Ensures that parsing of the values has occurred, or
		''' reoccurs as necessary.
		''' </summary>
		''' <param name="ctx"> The SynthContext </param>
		''' <returns> a non-null values reference </returns>
		Private Function getValues(ByVal ctx As javax.swing.plaf.synth.SynthContext) As Values
			validate()
			Return values
		End Function

		''' <summary>
		''' Simple utility method that searches the given array of Strings for the
		''' given string. This method is only called from getExtendedState if
		''' the developer has specified a specific state for the component to be
		''' in (ie, has "wedged" the component in that state) by specifying
		''' they client property "Nimbus.State".
		''' </summary>
		''' <param name="names"> a non-null array of strings </param>
		''' <param name="name"> the name to look for in the array </param>
		''' <returns> true or false based on whether the given name is in the array </returns>
		Private Function contains(ByVal names As String(), ByVal name As String) As Boolean
			Debug.Assert(name IsNot Nothing)
			For i As Integer = 0 To names.Length - 1
				If name.Equals(names(i)) Then Return True
			Next i
			Return False
		End Function

		''' <summary>
		''' <p>Gets the extended state for a given synth context. Nimbus supports the
		''' ability to define custom states. The algorithm used for choosing what
		''' style information to use for a given state requires a single integer
		''' bit string where each bit in the integer represents a different state
		''' that the component is in. This method uses the componentState as
		''' reported in the SynthContext, in addition to custom states, to determine
		''' what this extended state is.</p>
		''' 
		''' <p>In addition, this method checks the component in the given context
		''' for a client property called "Nimbus.State". If one exists, then it will
		''' decompose the String associated with that property to determine what
		''' state to return. In this way, the developer can force a component to be
		''' in a specific state, regardless of what the "real" state of the component
		''' is.</p>
		''' 
		''' <p>The string associated with "Nimbus.State" would be of the form:
		''' <pre>Enabled+CustomState+MouseOver</pre></p>
		''' </summary>
		''' <param name="ctx"> </param>
		''' <param name="v">
		''' @return </param>
		Private Function getExtendedState(ByVal ctx As javax.swing.plaf.synth.SynthContext, ByVal v As Values) As Integer
			Dim c As javax.swing.JComponent = ctx.component
			Dim xstate As Integer = 0
			Dim mask As Integer = 1
			'check for the Nimbus.State client property
			'Performance NOTE: getClientProperty ends up inside a synchronized
			'block, so there is some potential for performance issues here, however
			'I'm not certain that there is one on a modern VM.
			Dim [property] As Object = c.getClientProperty("Nimbus.State")
			If [property] IsNot Nothing Then
				Dim stateNames As String = [property].ToString()
				Dim states As String() = StringHelperClass.StringSplit(stateNames, "\+", True)
				If v.stateTypes Is Nothing Then
					' standard states only
					For Each stateStr As String In states
						Dim s As State.StandardState = State.getStandardState(stateStr)
						If s IsNot Nothing Then xstate = xstate Or s.state
					Next stateStr
				Else
					' custom states
					For Each s As State In v.stateTypes
						If contains(states, s.name) Then xstate = xstate Or mask
						mask <<= 1
					Next s
				End If
			Else
				'if there are no custom states defined, then simply return the
				'state that Synth reported
				If v.stateTypes Is Nothing Then Return ctx.componentState

				'there are custom states on this values, so I'll have to iterate
				'over them all and return a custom extended state
				Dim ___state As Integer = ctx.componentState
				For Each s As State In v.stateTypes
					If s.isInState(c, ___state) Then xstate = xstate Or mask
					mask <<= 1
				Next s
			End If
			Return xstate
		End Function

		''' <summary>
		''' <p>Gets the RuntimeState that most closely matches the state in the given
		''' context, but is less specific than the given "lastState". Essentially,
		''' this allows you to search for the next best state.</p>
		''' 
		''' <p>For example, if you had the following three states:
		''' <pre>
		''' Enabled
		''' Enabled+Pressed
		''' Disabled
		''' </pre>
		''' And you wanted to find the state that best represented
		''' ENABLED+PRESSED+FOCUSED and <code>lastState</code> was null (or an
		''' empty array, or an array with a single int with index == -1), then
		''' Enabled+Pressed would be returned. If you then call this method again but
		''' pass the index of Enabled+Pressed as the "lastState", then
		''' Enabled would be returned. If you call this method a third time and pass
		''' the index of Enabled in as the <code>lastState</code>, then null would be
		''' returned.</p>
		''' 
		''' <p>The actual code path for determining the proper state is the same as
		''' in Synth.</p>
		''' </summary>
		''' <param name="ctx"> </param>
		''' <param name="lastState"> a 1 element array, allowing me to do pass-by-reference.
		''' @return </param>
		Private Function getNextState(ByVal states As RuntimeState(), ByVal lastState As Integer(), ByVal xstate As Integer) As RuntimeState
			' Use the StateInfo with the most bits that matches that of state.
			' If there are none, then fallback to
			' the StateInfo with a state of 0, indicating it'll match anything.

			' Consider if we have 3 StateInfos a, b and c with states:
			' SELECTED, SELECTED | ENABLED, 0
			'
			' Input                          Return Value
			' -----                          ------------
			' SELECTED                       a
			' SELECTED | ENABLED             b
			' MOUSE_OVER                     c
			' SELECTED | ENABLED | FOCUSED   b
			' ENABLED                        c

			If states IsNot Nothing AndAlso states.Length > 0 Then
				Dim bestCount As Integer = 0
				Dim bestIndex As Integer = -1
				Dim wildIndex As Integer = -1

				'if xstate is 0, then search for the runtime state with component
				'state of 0. That is, find the exact match and return it.
				If xstate = 0 Then
					For counter As Integer = states.Length - 1 To 0 Step -1
						If states(counter).state = 0 Then
							lastState(0) = counter
							Return states(counter)
						End If
					Next counter
					'an exact match couldn't be found, so there was no match.
					lastState(0) = -1
					Return Nothing
				End If

				'xstate is some value != 0

				'determine from which index to start looking. If lastState[0] is -1
				'then we know to start from the end of the state array. Otherwise,
				'we start at the lastIndex - 1.
				Dim lastStateIndex As Integer = If(lastState Is Nothing OrElse lastState(0) = -1, states.Length, lastState(0))

				For counter As Integer = lastStateIndex - 1 To 0 Step -1
					Dim oState As Integer = states(counter).state

					If oState = 0 Then
						If wildIndex = -1 Then wildIndex = counter
					ElseIf (xstate And oState) = oState Then
						' This is key, we need to make sure all bits of the
						' StateInfo match, otherwise a StateInfo with
						' SELECTED | ENABLED would match ENABLED, which we
						' don't want.

						' This comes from BigInteger.bitCnt
						Dim bitCount As Integer = oState
						bitCount -= CInt(CUInt((&HaaaaaaaaL And bitCount)) >> 1)
						bitCount = (bitCount And &H33333333) + ((CInt(CUInt(bitCount) >> 2)) And &H33333333)
						bitCount = bitCount + (CInt(CUInt(bitCount) >> 4)) And &Hf0f0f0f
						bitCount += CInt(CUInt(bitCount) >> 8)
						bitCount += CInt(CUInt(bitCount) >> 16)
						bitCount = bitCount And &Hff
						If bitCount > bestCount Then
							bestIndex = counter
							bestCount = bitCount
						End If
					End If
				Next counter
				If bestIndex <> -1 Then
					lastState(0) = bestIndex
					Return states(bestIndex)
				End If
				If wildIndex <> -1 Then
					lastState(0) = wildIndex
					Return states(wildIndex)
				End If
			End If
			lastState(0) = -1
			Return Nothing
		End Function

		''' <summary>
		''' Contains values such as the UIDefaults and painters associated with
		''' a state. Whereas <code>State</code> represents a distinct state that a
		''' component can be in (such as Enabled), this class represents the colors,
		''' fonts, painters, etc associated with some state for this
		''' style.
		''' </summary>
		Private NotInheritable Class RuntimeState
			Implements ICloneable

			Private ReadOnly outerInstance As NimbusStyle

			Friend state As Integer
			Friend backgroundPainter As javax.swing.Painter
			Friend foregroundPainter As javax.swing.Painter
			Friend borderPainter As javax.swing.Painter
			Friend stateName As String
			Friend defaults As New javax.swing.UIDefaults(10,.7f)

			Private Sub New(ByVal outerInstance As NimbusStyle, ByVal ___state As Integer, ByVal stateName As String)
					Me.outerInstance = outerInstance
				Me.state = ___state
				Me.stateName = stateName
			End Sub

			Public Overrides Function ToString() As String
				Return stateName
			End Function

			Public Overrides Function clone() As RuntimeState
				Dim ___clone As New RuntimeState(state, stateName)
				___clone.backgroundPainter = backgroundPainter
				___clone.foregroundPainter = foregroundPainter
				___clone.borderPainter = borderPainter
'JAVA TO VB CONVERTER TODO TASK: There is no .NET Dictionary equivalent to the Java 'putAll' method:
				___clone.defaults.putAll(defaults)
				Return ___clone
			End Function
		End Class

		''' <summary>
		''' Essentially a struct of data for a style. A default instance of this
		''' class is used by NimbusStyle. Additional instances exist for each
		''' component that has overrides.
		''' </summary>
		Private NotInheritable Class Values
			''' <summary>
			''' The list of State types. A State represents a type of state, such
			''' as Enabled, Default, WindowFocused, etc. These can be custom states.
			''' </summary>
			Friend stateTypes As State() = Nothing
			''' <summary>
			''' The list of actual runtime state representations. These can represent things such
			''' as Enabled + Focused. Thus, they differ from States in that they contain
			''' several states together, and have associated properties, data, etc.
			''' </summary>
			Friend states As RuntimeState() = Nothing
			''' <summary>
			''' The content margins for this region.
			''' </summary>
			Friend contentMargins As java.awt.Insets
			''' <summary>
			''' Defaults on the region/component level.
			''' </summary>
			Friend defaults As New javax.swing.UIDefaults(10,.7f)
			''' <summary>
			''' Simple cache. After a value has been looked up, it is stored
			''' in this cache for later retrieval. The key is a concatenation of
			''' the property being looked up, two dollar signs, and the extended
			''' state. So for example:
			''' 
			''' foo.bar$$2353
			''' </summary>
			Friend cache As IDictionary(Of CacheKey, Object) = New Dictionary(Of CacheKey, Object)
		End Class

		''' <summary>
		''' This implementation presupposes that key is never null and that
		''' the two keys being checked for equality are never null
		''' </summary>
		Private NotInheritable Class CacheKey
			Private key As String
			Private xstate As Integer

			Friend Sub New(ByVal key As Object, ByVal xstate As Integer)
				init(key, xstate)
			End Sub

			Friend Sub init(ByVal key As Object, ByVal xstate As Integer)
				Me.key = key.ToString()
				Me.xstate = xstate
			End Sub

			Public Overrides Function Equals(ByVal obj As Object) As Boolean
				Dim other As CacheKey = CType(obj, CacheKey)
				If obj Is Nothing Then Return False
				If Me.xstate <> other.xstate Then Return False
				If Not Me.key.Equals(other.key) Then Return False
				Return True
			End Function

			Public Overrides Function GetHashCode() As Integer
				Dim hash As Integer = 3
				hash = 29 * hash + Me.key.GetHashCode()
				hash = 29 * hash + Me.xstate
				Return hash
			End Function
		End Class
	End Class

End Namespace
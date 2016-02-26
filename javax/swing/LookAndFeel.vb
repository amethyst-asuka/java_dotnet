Imports System
Imports javax.swing.text
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

Namespace javax.swing





	''' <summary>
	''' {@code LookAndFeel}, as the name implies, encapsulates a look and
	''' feel. Beyond installing a look and feel most developers never need to
	''' interact directly with {@code LookAndFeel}. In general only developers
	''' creating a custom look and feel need to concern themselves with this class.
	''' <p>
	''' Swing is built upon the foundation that each {@code JComponent}
	''' subclass has an implementation of a specific {@code ComponentUI}
	''' subclass. The {@code ComponentUI} is often referred to as "the ui",
	''' "component ui", or "look and feel delegate". The {@code ComponentUI}
	''' subclass is responsible for providing the look and feel specific
	''' functionality of the component. For example, {@code JTree} requires
	''' an implementation of the {@code ComponentUI} subclass {@code
	''' TreeUI}. The implementation of the specific {@code
	''' ComponentUI} subclass is provided by the {@code LookAndFeel}. Each
	''' {@code JComponent} subclass identifies the {@code ComponentUI}
	''' subclass it requires by way of the {@code JComponent} method {@code
	''' getUIClassID}.
	''' <p>
	''' Each {@code LookAndFeel} implementation must provide
	''' an implementation of the appropriate {@code ComponentUI} subclass by
	''' specifying a value for each of Swing's ui class ids in the {@code
	''' UIDefaults} object returned from {@code getDefaults}. For example,
	''' {@code BasicLookAndFeel} uses {@code BasicTreeUI} as the concrete
	''' implementation for {@code TreeUI}. This is accomplished by {@code
	''' BasicLookAndFeel} providing the key-value pair {@code
	''' "TreeUI"-"javax.swing.plaf.basic.BasicTreeUI"}, in the
	''' {@code UIDefaults} returned from {@code getDefaults}. Refer to
	''' <seealso cref="UIDefaults#getUI(JComponent)"/> for details on how the implementation
	''' of the {@code ComponentUI} subclass is obtained.
	''' <p>
	''' When a {@code LookAndFeel} is installed the {@code UIManager} does
	''' not check that an entry exists for all ui class ids. As such,
	''' random exceptions will occur if the current look and feel has not
	''' provided a value for a particular ui class id and an instance of
	''' the {@code JComponent} subclass is created.
	''' 
	''' <h2>Recommendations for Look and Feels</h2>
	''' 
	''' As noted in {@code UIManager} each {@code LookAndFeel} has the opportunity
	''' to provide a set of defaults that are layered in with developer and
	''' system defaults. Some of Swing's components require the look and feel
	''' to provide a specific set of defaults. These are documented in the
	''' classes that require the specific default.
	''' 
	''' <h3><a name="defaultRecommendation">ComponentUIs and defaults</a></h3>
	''' 
	''' All {@code ComponentUIs} typically need to set various properties
	''' on the {@code JComponent} the {@code ComponentUI} is providing the
	''' look and feel for. This is typically done when the {@code
	''' ComponentUI} is installed on the {@code JComponent}. Setting a
	''' property should only be done if the developer has not set the
	''' property. For non-primitive values it is recommended that the
	''' {@code ComponentUI} only change the property on the {@code
	''' JComponent} if the current value is {@code null} or implements
	''' {@code UIResource}. If the current value is {@code null} or
	''' implements {@code UIResource} it indicates the property has not
	''' been set by the developer, and the ui is free to change it.  For
	''' example, {@code BasicButtonUI.installDefaults} only changes the
	''' font on the {@code JButton} if the return value from {@code
	''' button.getFont()} is {@code null} or implements {@code
	''' UIResource}. On the other hand if {@code button.getFont()} returned
	''' a {@code non-null} value that did not implement {@code UIResource}
	''' then {@code BasicButtonUI.installDefaults} would not change the
	''' {@code JButton}'s font.
	''' <p>
	''' For primitive values, such as {@code opaque}, the method {@code
	''' installProperty} should be invoked.  {@code installProperty} only changes
	''' the corresponding property if the value has not been changed by the
	''' developer.
	''' <p>
	''' {@code ComponentUI} implementations should use the various install methods
	''' provided by this class as they handle the necessary checking and install
	''' the property using the recommended guidelines.
	''' 
	''' <h3><a name="exceptions"></a>Exceptions</h3>
	''' 
	''' All of the install methods provided by {@code LookAndFeel} need to
	''' access the defaults if the value of the property being changed is
	''' {@code null} or a {@code UIResource}. For example, installing the
	''' font does the following:
	''' <pre>
	'''   JComponent c;
	'''   Font font = c.getFont();
	'''   if (font == null || (font instanceof UIResource)) {
	'''       c.setFont(UIManager.getFont("fontKey"));
	'''   }
	''' </pre>
	''' If the font is {@code null} or a {@code UIResource}, the
	''' defaults table is queried with the key {@code fontKey}. All of
	''' {@code UIDefault's} get methods throw a {@code
	''' NullPointerException} if passed in {@code null}. As such, unless
	''' otherwise noted each of the various install methods of {@code
	''' LookAndFeel} throw a {@code NullPointerException} if the current
	''' value is {@code null} or a {@code UIResource} and the supplied
	''' defaults key is {@code null}. In addition, unless otherwise specified
	''' all of the {@code install} methods throw a {@code NullPointerException} if
	''' a {@code null} component is passed in.
	''' 
	''' @author Tom Ball
	''' @author Hans Muller
	''' </summary>
	Public MustInherit Class LookAndFeel

		''' <summary>
		''' Convenience method for setting a component's foreground
		''' and background color properties with values from the
		''' defaults.  The properties are only set if the current
		''' value is either {@code null} or a {@code UIResource}.
		''' </summary>
		''' <param name="c"> component to set the colors on </param>
		''' <param name="defaultBgName"> key for the background </param>
		''' <param name="defaultFgName"> key for the foreground
		''' </param>
		''' <seealso cref= #installColorsAndFont </seealso>
		''' <seealso cref= UIManager#getColor </seealso>
		''' <exception cref="NullPointerException"> as described in
		'''         <a href="#exceptions">exceptions</a> </exception>
		Public Shared Sub installColors(ByVal c As JComponent, ByVal defaultBgName As String, ByVal defaultFgName As String)
			Dim bg As java.awt.Color = c.background
			If bg Is Nothing OrElse TypeOf bg Is UIResource Then c.background = UIManager.getColor(defaultBgName)

			Dim fg As java.awt.Color = c.foreground
			If fg Is Nothing OrElse TypeOf fg Is UIResource Then c.foreground = UIManager.getColor(defaultFgName)
		End Sub


		''' <summary>
		''' Convenience method for setting a component's foreground,
		''' background and font properties with values from the
		''' defaults.  The properties are only set if the current
		''' value is either {@code null} or a {@code UIResource}.
		''' </summary>
		''' <param name="c"> component set to the colors and font on </param>
		''' <param name="defaultBgName"> key for the background </param>
		''' <param name="defaultFgName"> key for the foreground </param>
		''' <param name="defaultFontName"> key for the font </param>
		''' <exception cref="NullPointerException"> as described in
		'''         <a href="#exceptions">exceptions</a>
		''' </exception>
		''' <seealso cref= #installColors </seealso>
		''' <seealso cref= UIManager#getColor </seealso>
		''' <seealso cref= UIManager#getFont </seealso>
		Public Shared Sub installColorsAndFont(ByVal c As JComponent, ByVal defaultBgName As String, ByVal defaultFgName As String, ByVal defaultFontName As String)
			Dim f As java.awt.Font = c.font
			If f Is Nothing OrElse TypeOf f Is UIResource Then c.font = UIManager.getFont(defaultFontName)

			installColors(c, defaultBgName, defaultFgName)
		End Sub


		''' <summary>
		''' Convenience method for setting a component's border property with
		''' a value from the defaults. The border is only set if the border is
		''' {@code null} or an instance of {@code UIResource}.
		''' </summary>
		''' <param name="c"> component to set the border on </param>
		''' <param name="defaultBorderName"> key specifying the border </param>
		''' <exception cref="NullPointerException"> as described in
		'''         <a href="#exceptions">exceptions</a> </exception>
		Public Shared Sub installBorder(ByVal c As JComponent, ByVal defaultBorderName As String)
			Dim b As Border = c.border
			If b Is Nothing OrElse TypeOf b Is UIResource Then c.border = UIManager.getBorder(defaultBorderName)
		End Sub


		''' <summary>
		''' Convenience method for uninstalling a border. If the border of
		''' the component is a {@code UIResource}, it is set to {@code
		''' null}.
		''' </summary>
		''' <param name="c"> component to uninstall the border on </param>
		''' <exception cref="NullPointerException"> if {@code c} is {@code null} </exception>
		Public Shared Sub uninstallBorder(ByVal c As JComponent)
			If TypeOf c.border Is UIResource Then c.border = Nothing
		End Sub

		''' <summary>
		''' Convenience method for installing a property with the specified name
		''' and value on a component if that property has not already been set
		''' by the developer.  This method is intended to be used by
		''' ui delegate instances that need to specify a default value for a
		''' property of primitive type (boolean, int, ..), but do not wish
		''' to override a value set by the client.  Since primitive property
		''' values cannot be wrapped with the {@code UIResource} marker, this method
		''' uses private state to determine whether the property has been set
		''' by the client.
		''' </summary>
		''' <exception cref="IllegalArgumentException"> if the specified property is not
		'''         one which can be set using this method </exception>
		''' <exception cref="ClassCastException"> if the property value has not been set
		'''         by the developer and the type does not match the property's type </exception>
		''' <exception cref="NullPointerException"> if {@code c} is {@code null}, or the
		'''         named property has not been set by the developer and
		'''         {@code propertyValue} is {@code null} </exception>
		''' <param name="c"> target component to set the property on </param>
		''' <param name="propertyName"> name of the property to set </param>
		''' <param name="propertyValue"> value of the property
		''' @since 1.5 </param>
		Public Shared Sub installProperty(ByVal c As JComponent, ByVal propertyName As String, ByVal propertyValue As Object)
			' this is a special case because the JPasswordField's ancestor hierarchy
			' includes a class outside of javax.swing, thus we cannot call setUIProperty
			' directly.
			If sun.awt.SunToolkit.isInstanceOf(c, "javax.swing.JPasswordField") Then
				If Not CType(c, JPasswordField).customSetUIProperty(propertyName, propertyValue) Then c.uIPropertyrty(propertyName, propertyValue)
			Else
				c.uIPropertyrty(propertyName, propertyValue)
			End If
		End Sub

		''' <summary>
		''' Convenience method for building an array of {@code
		''' KeyBindings}. While this method is not deprecated, developers
		''' should instead use {@code ActionMap} and {@code InputMap} for
		''' supplying key bindings.
		''' <p>
		''' This method returns an array of {@code KeyBindings}, one for each
		''' alternating {@code key-action} pair in {@code keyBindingList}.
		''' A {@code key} can either be a {@code String} in the format
		''' specified by the <code>KeyStroke.getKeyStroke</code> method, or
		''' a {@code KeyStroke}. The {@code action} part of the pair is a
		''' {@code String} that corresponds to the name of the {@code
		''' Action}.
		''' <p>
		''' The following example illustrates creating a {@code KeyBinding} array
		''' from six alternating {@code key-action} pairs:
		''' <pre>
		'''  JTextComponent.KeyBinding[] multilineBindings = makeKeyBindings( new Object[] {
		'''          "UP", DefaultEditorKit.upAction,
		'''        "DOWN", DefaultEditorKit.downAction,
		'''     "PAGE_UP", DefaultEditorKit.pageUpAction,
		'''   "PAGE_DOWN", DefaultEditorKit.pageDownAction,
		'''       "ENTER", DefaultEditorKit.insertBreakAction,
		'''         "TAB", DefaultEditorKit.insertTabAction
		'''  });
		''' </pre>
		''' If {@code keyBindingList's} length is odd, the last element is
		''' ignored.
		''' <p>
		''' Supplying a {@code null} value for either the {@code key} or
		''' {@code action} part of the {@code key-action} pair results in
		''' creating a {@code KeyBinding} with the corresponding value
		''' {@code null}. As other parts of Swing's expect {@code non-null} values
		''' in a {@code KeyBinding}, you should avoid supplying {@code null} as
		''' either the {@code key} or {@code action} part of the {@code key-action}
		''' pair.
		''' </summary>
		''' <param name="keyBindingList"> an array of {@code key-action} pairs </param>
		''' <returns> an array of {@code KeyBindings} </returns>
		''' <exception cref="NullPointerException"> if {@code keyBindingList} is {@code null} </exception>
		''' <exception cref="ClassCastException"> if the {@code key} part of the pair is
		'''         not a {@code KeyStroke} or {@code String}, or the
		'''         {@code action} part of the pair is not a {@code String} </exception>
		''' <seealso cref= ActionMap </seealso>
		''' <seealso cref= InputMap </seealso>
		''' <seealso cref= KeyStroke#getKeyStroke </seealso>
		Public Shared Function makeKeyBindings(ByVal keyBindingList As Object()) As JTextComponent.KeyBinding()
			Dim rv As JTextComponent.KeyBinding() = New JTextComponent.KeyBinding(keyBindingList.Length \ 2 - 1){}

			For i As Integer = 0 To rv.Length - 1
				Dim o As Object = keyBindingList(2 * i)
				Dim keystroke As KeyStroke = If(TypeOf o Is KeyStroke, CType(o, KeyStroke), KeyStroke.getKeyStroke(CStr(o)))
				Dim action As String = CStr(keyBindingList(2 * i + 1))
				rv(i) = New JTextComponent.KeyBinding(keystroke, action)
			Next i

			Return rv
		End Function

		''' <summary>
		''' Creates a {@code InputMapUIResource} from <code>keys</code>. This is
		''' a convenience method for creating a new {@code InputMapUIResource},
		''' invoking {@code loadKeyBindings(map, keys)}, and returning the
		''' {@code InputMapUIResource}.
		''' </summary>
		''' <param name="keys"> alternating pairs of {@code keystroke-action key}
		'''        pairs as described in <seealso cref="#loadKeyBindings"/> </param>
		''' <returns> newly created and populated {@code InputMapUIResource} </returns>
		''' <seealso cref= #loadKeyBindings
		''' 
		''' @since 1.3 </seealso>
		Public Shared Function makeInputMap(ByVal keys As Object()) As InputMap
			Dim retMap As InputMap = New InputMapUIResource
			loadKeyBindings(retMap, keys)
			Return retMap
		End Function

		''' <summary>
		''' Creates a {@code ComponentInputMapUIResource} from
		''' <code>keys</code>. This is a convenience method for creating a
		''' new {@code ComponentInputMapUIResource}, invoking {@code
		''' loadKeyBindings(map, keys)}, and returning the {@code
		''' ComponentInputMapUIResource}.
		''' </summary>
		''' <param name="c"> component to create the {@code ComponentInputMapUIResource}
		'''          with </param>
		''' <param name="keys"> alternating pairs of {@code keystroke-action key}
		'''        pairs as described in <seealso cref="#loadKeyBindings"/> </param>
		''' <returns> newly created and populated {@code InputMapUIResource} </returns>
		''' <exception cref="IllegalArgumentException"> if {@code c} is {@code null}
		''' </exception>
		''' <seealso cref= #loadKeyBindings </seealso>
		''' <seealso cref= ComponentInputMapUIResource
		''' 
		''' @since 1.3 </seealso>
		Public Shared Function makeComponentInputMap(ByVal c As JComponent, ByVal keys As Object()) As ComponentInputMap
			Dim retMap As ComponentInputMap = New ComponentInputMapUIResource(c)
			loadKeyBindings(retMap, keys)
			Return retMap
		End Function


		''' <summary>
		''' Populates an {@code InputMap} with the specified bindings.
		''' The bindings are supplied as a list of alternating
		''' {@code keystroke-action key} pairs. The {@code keystroke} is either
		''' an instance of {@code KeyStroke}, or a {@code String}
		''' that identifies the {@code KeyStroke} for the binding. Refer
		''' to {@code KeyStroke.getKeyStroke(String)} for the specific
		''' format. The {@code action key} part of the pair is the key
		''' registered in the {@code InputMap} for the {@code KeyStroke}.
		''' <p>
		''' The following illustrates loading an {@code InputMap} with two
		''' {@code key-action} pairs:
		''' <pre>
		'''   LookAndFeel.loadKeyBindings(inputMap, new Object[] {
		'''     "control X", "cut",
		'''     "control V", "paste"
		'''   });
		''' </pre>
		''' <p>
		''' Supplying a {@code null} list of bindings ({@code keys}) does not
		''' change {@code retMap} in any way.
		''' <p>
		''' Specifying a {@code null} {@code action key} results in
		''' removing the {@code keystroke's} entry from the {@code InputMap}.
		''' A {@code null} {@code keystroke} is ignored.
		''' </summary>
		''' <param name="retMap"> {@code InputMap} to add the {@code key-action}
		'''               pairs to </param>
		''' <param name="keys"> bindings to add to {@code retMap} </param>
		''' <exception cref="NullPointerException"> if {@code keys} is
		'''         {@code non-null}, not empty, and {@code retMap} is
		'''         {@code null}
		''' </exception>
		''' <seealso cref= KeyStroke#getKeyStroke(String) </seealso>
		''' <seealso cref= InputMap
		''' 
		''' @since 1.3 </seealso>
		Public Shared Sub loadKeyBindings(ByVal retMap As InputMap, ByVal keys As Object())
			If keys IsNot Nothing Then
				Dim counter As Integer = 0
				Dim maxCounter As Integer = keys.Length
				Do While counter < maxCounter
					Dim keyStrokeO As Object = keys(counter)
					counter += 1
					Dim ks As KeyStroke = If(TypeOf keyStrokeO Is KeyStroke, CType(keyStrokeO, KeyStroke), KeyStroke.getKeyStroke(CStr(keyStrokeO)))
					retMap.put(ks, keys(counter))
					counter += 1
				Loop
			End If
		End Sub

		''' <summary>
		''' Creates and returns a {@code UIDefault.LazyValue} that loads an
		''' image. The returned value is an implementation of {@code
		''' UIDefaults.LazyValue}. When {@code createValue} is invoked on
		''' the returned object, the image is loaded. If the image is {@code
		''' non-null}, it is then wrapped in an {@code Icon} that implements {@code
		''' UIResource}. The image is loaded using {@code
		''' Class.getResourceAsStream(gifFile)}.
		''' <p>
		''' This method does not check the arguments in any way. It is
		''' strongly recommended that {@code non-null} values are supplied else
		''' exceptions may occur when {@code createValue} is invoked on the
		''' returned object.
		''' </summary>
		''' <param name="baseClass"> {@code Class} used to load the resource </param>
		''' <param name="gifFile"> path to the image to load </param>
		''' <returns> a {@code UIDefaults.LazyValue}; when resolved the
		'''         {@code LazyValue} loads the specified image </returns>
		''' <seealso cref= UIDefaults.LazyValue </seealso>
		''' <seealso cref= Icon </seealso>
		''' <seealso cref= Class#getResourceAsStream(String) </seealso>
		Public Shared Function makeIcon(ByVal baseClass As Type, ByVal gifFile As String) As Object
			Return sun.swing.SwingUtilities2.makeIcon(baseClass, baseClass, gifFile)
		End Function

		''' <summary>
		''' Returns the <code>LayoutStyle</code> for this look
		''' and feel.  This never returns {@code null}.
		''' <p>
		''' You generally don't use the <code>LayoutStyle</code> from
		''' the look and feel, instead use the <code>LayoutStyle</code>
		''' method <code>getInstance</code>.
		''' </summary>
		''' <seealso cref= LayoutStyle#getInstance </seealso>
		''' <returns> the <code>LayoutStyle</code> for this look and feel
		''' @since 1.6 </returns>
		Public Overridable Property layoutStyle As LayoutStyle
			Get
				Return sun.swing.DefaultLayoutStyle.instance
			End Get
		End Property

		''' <summary>
		''' Invoked when the user attempts an invalid operation,
		''' such as pasting into an uneditable <code>JTextField</code>
		''' that has focus. The default implementation beeps. Subclasses
		''' that wish different behavior should override this and provide
		''' the additional feedback.
		''' </summary>
		''' <param name="component"> the <code>Component</code> the error occurred in,
		'''                  may be <code>null</code>
		'''                  indicating the error condition is not directly
		'''                  associated with a <code>Component</code>
		''' @since 1.4 </param>
		Public Overridable Sub provideErrorFeedback(ByVal component As java.awt.Component)
			Dim toolkit As java.awt.Toolkit = Nothing
			If component IsNot Nothing Then
				toolkit = component.toolkit
			Else
				toolkit = java.awt.Toolkit.defaultToolkit
			End If
			toolkit.beep()
		End Sub ' provideErrorFeedback()

		''' <summary>
		''' Returns the value of the specified system desktop property by
		''' invoking <code>Toolkit.getDefaultToolkit().getDesktopProperty()</code>.
		''' If the value of the specified property is {@code null},
		''' {@code fallbackValue} is returned.
		''' </summary>
		''' <param name="systemPropertyName"> the name of the system desktop property being queried </param>
		''' <param name="fallbackValue"> the object to be returned as the value if the system value is null </param>
		''' <returns> the current value of the desktop property
		''' </returns>
		''' <seealso cref= java.awt.Toolkit#getDesktopProperty
		''' 
		''' @since 1.4 </seealso>
		Public Shared Function getDesktopPropertyValue(ByVal systemPropertyName As String, ByVal fallbackValue As Object) As Object
			Dim value As Object = java.awt.Toolkit.defaultToolkit.getDesktopProperty(systemPropertyName)
			If value Is Nothing Then
				Return fallbackValue
			ElseIf TypeOf value Is java.awt.Color Then
				Return New ColorUIResource(CType(value, java.awt.Color))
			ElseIf TypeOf value Is java.awt.Font Then
				Return New FontUIResource(CType(value, java.awt.Font))
			End If
			Return value
		End Function

		''' <summary>
		''' Returns an <code>Icon</code> with a disabled appearance.
		''' This method is used to generate a disabled <code>Icon</code> when
		''' one has not been specified.  For example, if you create a
		''' <code>JButton</code> and only specify an <code>Icon</code> via
		''' <code>setIcon</code> this method will be called to generate the
		''' disabled <code>Icon</code>. If {@code null} is passed as
		''' <code>icon</code> this method returns {@code null}.
		''' <p>
		''' Some look and feels might not render the disabled {@code Icon}, in which
		''' case they will ignore this.
		''' </summary>
		''' <param name="component"> {@code JComponent} that will display the {@code Icon},
		'''         may be {@code null} </param>
		''' <param name="icon"> {@code Icon} to generate the disabled icon from </param>
		''' <returns> disabled {@code Icon}, or {@code null} if a suitable
		'''         {@code Icon} can not be generated
		''' @since 1.5 </returns>
		Public Overridable Function getDisabledIcon(ByVal component As JComponent, ByVal icon As Icon) As Icon
			If TypeOf icon Is ImageIcon Then Return New sun.swing.ImageIconUIResource(GrayFilter.createDisabledImage(CType(icon, ImageIcon).image))
			Return Nothing
		End Function

		''' <summary>
		''' Returns an <code>Icon</code> for use by disabled
		''' components that are also selected. This method is used to generate an
		''' <code>Icon</code> for components that are in both the disabled and
		''' selected states but do not have a specific <code>Icon</code> for this
		''' state.  For example, if you create a <code>JButton</code> and only
		''' specify an <code>Icon</code> via <code>setIcon</code> this method
		''' will be called to generate the disabled and selected
		''' <code>Icon</code>. If {@code null} is passed as <code>icon</code> this
		''' methods returns {@code null}.
		''' <p>
		''' Some look and feels might not render the disabled and selected
		''' {@code Icon}, in which case they will ignore this.
		''' </summary>
		''' <param name="component"> {@code JComponent} that will display the {@code Icon},
		'''        may be {@code null} </param>
		''' <param name="icon"> {@code Icon} to generate disabled and selected icon from </param>
		''' <returns> disabled and selected icon, or {@code null} if a suitable
		'''         {@code Icon} can not be generated.
		''' @since 1.5 </returns>
		Public Overridable Function getDisabledSelectedIcon(ByVal component As JComponent, ByVal icon As Icon) As Icon
			Return getDisabledIcon(component, icon)
		End Function

		''' <summary>
		''' Return a short string that identifies this look and feel, e.g.
		''' "CDE/Motif".  This string should be appropriate for a menu item.
		''' Distinct look and feels should have different names, e.g.
		''' a subclass of MotifLookAndFeel that changes the way a few components
		''' are rendered should be called "CDE/Motif My Way"; something
		''' that would be useful to a user trying to select a L&amp;F from a list
		''' of names.
		''' </summary>
		''' <returns> short identifier for the look and feel </returns>
		Public MustOverride ReadOnly Property name As String


		''' <summary>
		''' Return a string that identifies this look and feel.  This string
		''' will be used by applications/services that want to recognize
		''' well known look and feel implementations.  Presently
		''' the well known names are "Motif", "Windows", "Mac", "Metal".  Note
		''' that a LookAndFeel derived from a well known superclass
		''' that doesn't make any fundamental changes to the look or feel
		''' shouldn't override this method.
		''' </summary>
		''' <returns> identifier for the look and feel </returns>
		Public MustOverride ReadOnly Property iD As String


		''' <summary>
		''' Return a one line description of this look and feel implementation,
		''' e.g. "The CDE/Motif Look and Feel".   This string is intended for
		''' the user, e.g. in the title of a window or in a ToolTip message.
		''' </summary>
		''' <returns> short description for the look and feel </returns>
		Public MustOverride ReadOnly Property description As String


		''' <summary>
		''' Returns {@code true} if the <code>LookAndFeel</code> returned
		''' <code>RootPaneUI</code> instances support providing {@code Window}
		''' decorations in a <code>JRootPane</code>.
		''' <p>
		''' The default implementation returns {@code false}, subclasses that
		''' support {@code Window} decorations should override this and return
		''' {@code true}.
		''' </summary>
		''' <returns> {@code true} if the {@code RootPaneUI} instances created by
		'''         this look and feel support client side decorations </returns>
		''' <seealso cref= JDialog#setDefaultLookAndFeelDecorated </seealso>
		''' <seealso cref= JFrame#setDefaultLookAndFeelDecorated </seealso>
		''' <seealso cref= JRootPane#setWindowDecorationStyle
		''' @since 1.4 </seealso>
		Public Overridable Property supportsWindowDecorations As Boolean
			Get
				Return False
			End Get
		End Property

		''' <summary>
		''' If the underlying platform has a "native" look and feel, and
		''' this is an implementation of it, return {@code true}.  For
		''' example, when the underlying platform is Solaris running CDE
		''' a CDE/Motif look and feel implementation would return {@code
		''' true}.
		''' </summary>
		''' <returns> {@code true} if this look and feel represents the underlying
		'''         platform look and feel </returns>
		Public MustOverride ReadOnly Property nativeLookAndFeel As Boolean


		''' <summary>
		''' Return {@code true} if the underlying platform supports and or permits
		''' this look and feel.  This method returns {@code false} if the look
		''' and feel depends on special resources or legal agreements that
		''' aren't defined for the current platform.
		''' 
		''' </summary>
		''' <returns> {@code true} if this is a supported look and feel </returns>
		''' <seealso cref= UIManager#setLookAndFeel </seealso>
		Public MustOverride ReadOnly Property supportedLookAndFeel As Boolean


		''' <summary>
		''' Initializes the look and feel. While this method is public,
		''' it should only be invoked by the {@code UIManager} when a
		''' look and feel is installed as the current look and feel. This
		''' method is invoked before the {@code UIManager} invokes
		''' {@code getDefaults}. This method is intended to perform any
		''' initialization for the look and feel. Subclasses
		''' should do any one-time setup they need here, rather than
		''' in a static initializer, because look and feel class objects
		''' may be loaded just to discover that {@code isSupportedLookAndFeel()}
		''' returns {@code false}.
		''' </summary>
		''' <seealso cref= #uninitialize </seealso>
		''' <seealso cref= UIManager#setLookAndFeel </seealso>
		Public Overridable Sub initialize()
		End Sub


		''' <summary>
		''' Uninitializes the look and feel. While this method is public,
		''' it should only be invoked by the {@code UIManager} when
		''' the look and feel is uninstalled. For example,
		''' {@code UIManager.setLookAndFeel} invokes this when the look and
		''' feel is changed.
		''' <p>
		''' Subclasses may choose to free up some resources here.
		''' </summary>
		''' <seealso cref= #initialize </seealso>
		''' <seealso cref= UIManager#setLookAndFeel </seealso>
		Public Overridable Sub uninitialize()
		End Sub

		''' <summary>
		''' Returns the look and feel defaults. While this method is public,
		''' it should only be invoked by the {@code UIManager} when the
		''' look and feel is set as the current look and feel and after
		''' {@code initialize} has been invoked.
		''' </summary>
		''' <returns> the look and feel defaults </returns>
		''' <seealso cref= #initialize </seealso>
		''' <seealso cref= #uninitialize </seealso>
		''' <seealso cref= UIManager#setLookAndFeel </seealso>
		Public Overridable Property defaults As UIDefaults
			Get
				Return Nothing
			End Get
		End Property

		''' <summary>
		''' Returns a string that displays and identifies this
		''' object's properties.
		''' </summary>
		''' <returns> a String representation of this object </returns>
		Public Overrides Function ToString() As String
			Return "[" & description & " - " & Me.GetType().name & "]"
		End Function
	End Class

End Namespace
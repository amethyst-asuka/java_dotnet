Imports javax.swing.plaf
Imports javax.swing

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
	''' A concrete implementation of {@code MetalTheme} providing
	''' the original look of the Java Look and Feel, code-named "Steel". Refer
	''' to <seealso cref="MetalLookAndFeel#setCurrentTheme"/> for details on changing
	''' the default theme.
	''' <p>
	''' All colors returned by {@code DefaultMetalTheme} are completely
	''' opaque.
	''' 
	''' <h3><a name="fontStyle"></a>Font Style</h3>
	''' 
	''' {@code DefaultMetalTheme} uses bold fonts for many controls.  To make all
	''' controls (with the exception of the internal frame title bars and
	''' client decorated frame title bars) use plain fonts you can do either of
	''' the following:
	''' <ul>
	''' <li>Set the system property <code>swing.boldMetal</code> to
	'''     <code>false</code>.  For example,
	'''     <code>java&nbsp;-Dswing.boldMetal=false&nbsp;MyApp</code>.
	''' <li>Set the defaults property <code>swing.boldMetal</code> to
	'''     <code>Boolean.FALSE</code>.  For example:
	'''     <code>UIManager.put("swing.boldMetal",&nbsp;Boolean.FALSE);</code>
	''' </ul>
	''' The defaults property <code>swing.boldMetal</code>, if set,
	''' takes precedence over the system property of the same name. After
	''' setting this defaults property you need to re-install
	''' <code>MetalLookAndFeel</code>, as well as update the UI
	''' of any previously created widgets. Otherwise the results are undefined.
	''' The following illustrates how to do this:
	''' <pre>
	'''   // turn off bold fonts
	'''   UIManager.put("swing.boldMetal", Boolean.FALSE);
	''' 
	'''   // re-install the Metal Look and Feel
	'''   UIManager.setLookAndFeel(new MetalLookAndFeel());
	''' 
	'''   // Update the ComponentUIs for all Components. This
	'''   // needs to be invoked for all windows.
	'''   SwingUtilities.updateComponentTreeUI(rootComponent);
	''' </pre>
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
	''' <seealso cref= MetalLookAndFeel </seealso>
	''' <seealso cref= MetalLookAndFeel#setCurrentTheme
	''' 
	''' @author Steve Wilson </seealso>
	Public Class DefaultMetalTheme
		Inherits MetalTheme

		''' <summary>
		''' Whether or not fonts should be plain.  This is only used if
		''' the defaults property 'swing.boldMetal' == "false".
		''' </summary>
		Private Shared ReadOnly PLAIN_FONTS As Boolean

		''' <summary>
		''' Names of the fonts to use.
		''' </summary>
		Private Shared ReadOnly fontNames As String() = { Font.DIALOG,Font.DIALOG,Font.DIALOG,Font.DIALOG,Font.DIALOG,Font.DIALOG }
		''' <summary>
		''' Styles for the fonts.  This is ignored if the defaults property
		''' <code>swing.boldMetal</code> is false, or PLAIN_FONTS is true.
		''' </summary>
		Private Shared ReadOnly fontStyles As Integer() = { Font.BOLD, Font.PLAIN, Font.PLAIN, Font.BOLD, Font.BOLD, Font.PLAIN }
		''' <summary>
		''' Sizes for the fonts.
		''' </summary>
		Private Shared ReadOnly fontSizes As Integer() = { 12, 12, 12, 12, 12, 10 }

		' note the properties listed here can currently be used by people
		' providing runtimes to hint what fonts are good.  For example the bold
		' dialog font looks bad on a Mac, so Apple could use this property to
		' hint at a good font.
		'
		' However, we don't promise to support these forever.  We may move
		' to getting these from the swing.properties file, or elsewhere.
		''' <summary>
		''' System property names used to look up fonts.
		''' </summary>
		Private Shared ReadOnly defaultNames As String() = { "swing.plaf.metal.controlFont", "swing.plaf.metal.systemFont", "swing.plaf.metal.userFont", "swing.plaf.metal.controlFont", "swing.plaf.metal.controlFont", "swing.plaf.metal.smallFont" }

		''' <summary>
		''' Returns the ideal font name for the font identified by key.
		''' </summary>
		Friend Shared Function getDefaultFontName(ByVal key As Integer) As String
			Return fontNames(key)
		End Function

		''' <summary>
		''' Returns the ideal font size for the font identified by key.
		''' </summary>
		Friend Shared Function getDefaultFontSize(ByVal key As Integer) As Integer
			Return fontSizes(key)
		End Function

		''' <summary>
		''' Returns the ideal font style for the font identified by key.
		''' </summary>
		Friend Shared Function getDefaultFontStyle(ByVal key As Integer) As Integer
			If key <> WINDOW_TITLE_FONT Then
				Dim boldMetal As Object = Nothing
				If sun.awt.AppContext.appContext.get(sun.swing.SwingUtilities2.LAF_STATE_KEY) IsNot Nothing Then boldMetal = UIManager.get("swing.boldMetal")
				If boldMetal IsNot Nothing Then
					If Boolean.FALSE.Equals(boldMetal) Then Return Font.PLAIN
				ElseIf PLAIN_FONTS Then
					Return Font.PLAIN
				End If
			End If
			Return fontStyles(key)
		End Function

		''' <summary>
		''' Returns the default used to look up the specified font.
		''' </summary>
		Friend Shared Function getDefaultPropertyName(ByVal key As Integer) As String
			Return defaultNames(key)
		End Function

		Shared Sub New()
			Dim boldProperty As Object = java.security.AccessController.doPrivileged(New sun.security.action.GetPropertyAction("swing.boldMetal"))
			If boldProperty Is Nothing OrElse (Not "false".Equals(boldProperty)) Then
				PLAIN_FONTS = False
			Else
				PLAIN_FONTS = True
			End If
		End Sub

		Private Shared ReadOnly primary1 As New ColorUIResource(102, 102, 153)
		Private Shared ReadOnly primary2 As New ColorUIResource(153, 153, 204)
		Private Shared ReadOnly primary3 As New ColorUIResource(204, 204, 255)
		Private Shared ReadOnly secondary1 As New ColorUIResource(102, 102, 102)
		Private Shared ReadOnly secondary2 As New ColorUIResource(153, 153, 153)
		Private Shared ReadOnly secondary3 As New ColorUIResource(204, 204, 204)

		Private fontDelegate As FontDelegate

		''' <summary>
		''' Returns the name of this theme. This returns {@code "Steel"}.
		''' </summary>
		''' <returns> the name of this theme. </returns>
		Public Property Overrides name As String
			Get
				Return "Steel"
			End Get
		End Property

		''' <summary>
		''' Creates and returns an instance of {@code DefaultMetalTheme}.
		''' </summary>
		Public Sub New()
			install()
		End Sub

		''' <summary>
		''' Returns the primary 1 color. This returns a color with rgb values
		''' of 102, 102, and 153, respectively.
		''' </summary>
		''' <returns> the primary 1 color </returns>
		Protected Friend Property Overrides primary1 As ColorUIResource
			Get
				Return primary1
			End Get
		End Property

		''' <summary>
		''' Returns the primary 2 color. This returns a color with rgb values
		''' of 153, 153, 204, respectively.
		''' </summary>
		''' <returns> the primary 2 color </returns>
		Protected Friend Property Overrides primary2 As ColorUIResource
			Get
				Return primary2
			End Get
		End Property

		''' <summary>
		''' Returns the primary 3 color. This returns a color with rgb values
		''' 204, 204, 255, respectively.
		''' </summary>
		''' <returns> the primary 3 color </returns>
		Protected Friend Property Overrides primary3 As ColorUIResource
			Get
				Return primary3
			End Get
		End Property

		''' <summary>
		''' Returns the secondary 1 color. This returns a color with rgb values
		''' 102, 102, and 102, respectively.
		''' </summary>
		''' <returns> the secondary 1 color </returns>
		Protected Friend Property Overrides secondary1 As ColorUIResource
			Get
				Return secondary1
			End Get
		End Property

		''' <summary>
		''' Returns the secondary 2 color. This returns a color with rgb values
		''' 153, 153, and 153, respectively.
		''' </summary>
		''' <returns> the secondary 2 color </returns>
		Protected Friend Property Overrides secondary2 As ColorUIResource
			Get
				Return secondary2
			End Get
		End Property

		''' <summary>
		''' Returns the secondary 3 color. This returns a color with rgb values
		''' 204, 204, and 204, respectively.
		''' </summary>
		''' <returns> the secondary 3 color </returns>
		Protected Friend Property Overrides secondary3 As ColorUIResource
			Get
				Return secondary3
			End Get
		End Property


		''' <summary>
		''' Returns the control text font. This returns Dialog, 12pt. If
		''' plain fonts have been enabled as described in <a href="#fontStyle">
		''' font style</a>, the font style is plain. Otherwise the font style is
		''' bold.
		''' </summary>
		''' <returns> the control text font </returns>
		Public Property Overrides controlTextFont As FontUIResource
			Get
				Return getFont(CONTROL_TEXT_FONT)
			End Get
		End Property

		''' <summary>
		''' Returns the system text font. This returns Dialog, 12pt, plain.
		''' </summary>
		''' <returns> the system text font </returns>
		Public Property Overrides systemTextFont As FontUIResource
			Get
				Return getFont(SYSTEM_TEXT_FONT)
			End Get
		End Property

		''' <summary>
		''' Returns the user text font. This returns Dialog, 12pt, plain.
		''' </summary>
		''' <returns> the user text font </returns>
		Public Property Overrides userTextFont As FontUIResource
			Get
				Return getFont(USER_TEXT_FONT)
			End Get
		End Property

		''' <summary>
		''' Returns the menu text font. This returns Dialog, 12pt. If
		''' plain fonts have been enabled as described in <a href="#fontStyle">
		''' font style</a>, the font style is plain. Otherwise the font style is
		''' bold.
		''' </summary>
		''' <returns> the menu text font </returns>
		Public Property Overrides menuTextFont As FontUIResource
			Get
				Return getFont(MENU_TEXT_FONT)
			End Get
		End Property

		''' <summary>
		''' Returns the window title font. This returns Dialog, 12pt, bold.
		''' </summary>
		''' <returns> the window title font </returns>
		Public Property Overrides windowTitleFont As FontUIResource
			Get
				Return getFont(WINDOW_TITLE_FONT)
			End Get
		End Property

		''' <summary>
		''' Returns the sub-text font. This returns Dialog, 10pt, plain.
		''' </summary>
		''' <returns> the sub-text font </returns>
		Public Property Overrides subTextFont As FontUIResource
			Get
				Return getFont(SUB_TEXT_FONT)
			End Get
		End Property

		Private Function getFont(ByVal key As Integer) As FontUIResource
			Return fontDelegate.getFont(key)
		End Function

		Friend Overrides Sub install()
			If MetalLookAndFeel.windows AndAlso MetalLookAndFeel.useSystemFonts() Then
				fontDelegate = New WindowsFontDelegate
			Else
				fontDelegate = New FontDelegate
			End If
		End Sub

		''' <summary>
		''' Returns true if this is a theme provided by the core platform.
		''' </summary>
		Friend Property Overrides systemTheme As Boolean
			Get
				Return (Me.GetType() = GetType(DefaultMetalTheme))
			End Get
		End Property

		''' <summary>
		''' FontDelegates add an extra level of indirection to obtaining fonts.
		''' </summary>
		Private Class FontDelegate
			Private Shared defaultMapping As Integer() = { CONTROL_TEXT_FONT, SYSTEM_TEXT_FONT, USER_TEXT_FONT, CONTROL_TEXT_FONT, CONTROL_TEXT_FONT, SUB_TEXT_FONT }
			Friend fonts As FontUIResource()

			' menu and window are mapped to controlFont
			Public Sub New()
				fonts = New FontUIResource(5){}
			End Sub

			Public Overridable Function getFont(ByVal type As Integer) As FontUIResource
				Dim mappedType As Integer = defaultMapping(type)
				If fonts(type) Is Nothing Then
					Dim f As Font = getPrivilegedFont(mappedType)

					If f Is Nothing Then f = New Font(getDefaultFontName(type), getDefaultFontStyle(type), getDefaultFontSize(type))
					fonts(type) = New FontUIResource(f)
				End If
				Return fonts(type)
			End Function

			''' <summary>
			''' This is the same as invoking
			''' <code>Font.getFont(key)</code>, with the exception
			''' that it is wrapped inside a <code>doPrivileged</code> call.
			''' </summary>
			Protected Friend Overridable Function getPrivilegedFont(ByVal key As Integer) As Font
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				Return java.security.AccessController.doPrivileged(New java.security.PrivilegedAction<Font>()
	'			{
	'					public Font run()
	'					{
	'						Return Font.getFont(getDefaultPropertyName(key));
	'					}
	'				}
				   )
			End Function
		End Class

		''' <summary>
		''' The WindowsFontDelegate uses DesktopProperties to obtain fonts.
		''' </summary>
		Private Class WindowsFontDelegate
			Inherits FontDelegate

			Private props As MetalFontDesktopProperty()
			Private checkedPriviledged As Boolean()

			Public Sub New()
				props = New MetalFontDesktopProperty(5){}
				checkedPriviledged = New Boolean(5){}
			End Sub

			Public Overrides Function getFont(ByVal type As Integer) As FontUIResource
				If fonts(type) IsNot Nothing Then Return fonts(type)
				If Not checkedPriviledged(type) Then
					Dim f As Font = getPrivilegedFont(type)

					checkedPriviledged(type) = True
					If f IsNot Nothing Then
						fonts(type) = New FontUIResource(f)
						Return fonts(type)
					End If
				End If
				If props(type) Is Nothing Then props(type) = New MetalFontDesktopProperty(type)
				' While passing null may seem bad, we don't actually use
				' the table and looking it up is rather expensive.
				Return CType(props(type).createValue(Nothing), FontUIResource)
			End Function
		End Class
	End Class

End Namespace
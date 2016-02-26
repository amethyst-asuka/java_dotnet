Imports javax.swing.plaf
Imports javax.swing

'
' * Copyright (c) 1998, 2006, Oracle and/or its affiliates. All rights reserved.
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
	''' {@code MetalTheme} provides the color palette and fonts used by
	''' the Java Look and Feel.
	''' <p>
	''' {@code MetalTheme} is abstract, see {@code DefaultMetalTheme} and
	''' {@code OceanTheme} for concrete implementations.
	''' <p>
	''' {@code MetalLookAndFeel} maintains the current theme that the
	''' the {@code ComponentUI} implementations for metal use. Refer to
	''' {@link MetalLookAndFeel#setCurrentTheme
	''' MetalLookAndFeel.setCurrentTheme(MetalTheme)} for details on changing
	''' the current theme.
	''' <p>
	''' {@code MetalTheme} provides a number of public methods for getting
	''' colors. These methods are implemented in terms of a
	''' handful of protected abstract methods. A subclass need only override
	''' the protected abstract methods ({@code getPrimary1},
	''' {@code getPrimary2}, {@code getPrimary3}, {@code getSecondary1},
	''' {@code getSecondary2}, and {@code getSecondary3}); although a subclass
	''' may override the other public methods for more control over the set of
	''' colors that are used.
	''' <p>
	''' Concrete implementations of {@code MetalTheme} must return {@code non-null}
	''' values from all methods. While the behavior of returning {@code null} is
	''' not specified, returning {@code null} will result in incorrect behavior.
	''' <p>
	''' It is strongly recommended that subclasses return completely opaque colors.
	''' To do otherwise may result in rendering problems, such as visual garbage.
	''' </summary>
	''' <seealso cref= DefaultMetalTheme </seealso>
	''' <seealso cref= OceanTheme </seealso>
	''' <seealso cref= MetalLookAndFeel#setCurrentTheme
	''' 
	''' @author Steve Wilson </seealso>
	Public MustInherit Class MetalTheme

		' Contants identifying the various Fonts that are Theme can support
		Friend Const CONTROL_TEXT_FONT As Integer = 0
		Friend Const SYSTEM_TEXT_FONT As Integer = 1
		Friend Const USER_TEXT_FONT As Integer = 2
		Friend Const MENU_TEXT_FONT As Integer = 3
		Friend Const WINDOW_TITLE_FONT As Integer = 4
		Friend Const SUB_TEXT_FONT As Integer = 5

		Friend Shared white As New ColorUIResource(255, 255, 255)
		Private Shared black As New ColorUIResource(0, 0, 0)

		''' <summary>
		''' Returns the name of this theme.
		''' </summary>
		''' <returns> the name of this theme </returns>
		Public MustOverride ReadOnly Property name As String

		''' <summary>
		''' Returns the primary 1 color.
		''' </summary>
		''' <returns> the primary 1 color </returns>
		Protected Friend MustOverride ReadOnly Property primary1 As ColorUIResource

		''' <summary>
		''' Returns the primary 2 color.
		''' </summary>
		''' <returns> the primary 2 color </returns>
		Protected Friend MustOverride ReadOnly Property primary2 As ColorUIResource

		''' <summary>
		''' Returns the primary 3 color.
		''' </summary>
		''' <returns> the primary 3 color </returns>
		Protected Friend MustOverride ReadOnly Property primary3 As ColorUIResource

		''' <summary>
		''' Returns the secondary 1 color.
		''' </summary>
		''' <returns> the secondary 1 color </returns>
		Protected Friend MustOverride ReadOnly Property secondary1 As ColorUIResource

		''' <summary>
		''' Returns the secondary 2 color.
		''' </summary>
		''' <returns> the secondary 2 color </returns>
		Protected Friend MustOverride ReadOnly Property secondary2 As ColorUIResource

		''' <summary>
		''' Returns the secondary 3 color.
		''' </summary>
		''' <returns> the secondary 3 color </returns>
		Protected Friend MustOverride ReadOnly Property secondary3 As ColorUIResource

		''' <summary>
		''' Returns the control text font.
		''' </summary>
		''' <returns> the control text font </returns>
		Public MustOverride ReadOnly Property controlTextFont As FontUIResource

		''' <summary>
		''' Returns the system text font.
		''' </summary>
		''' <returns> the system text font </returns>
		Public MustOverride ReadOnly Property systemTextFont As FontUIResource

		''' <summary>
		''' Returns the user text font.
		''' </summary>
		''' <returns> the user text font </returns>
		Public MustOverride ReadOnly Property userTextFont As FontUIResource

		''' <summary>
		''' Returns the menu text font.
		''' </summary>
		''' <returns> the menu text font </returns>
		Public MustOverride ReadOnly Property menuTextFont As FontUIResource

		''' <summary>
		''' Returns the window title font.
		''' </summary>
		''' <returns> the window title font </returns>
		Public MustOverride ReadOnly Property windowTitleFont As FontUIResource

		''' <summary>
		''' Returns the sub-text font.
		''' </summary>
		''' <returns> the sub-text font </returns>
		Public MustOverride ReadOnly Property subTextFont As FontUIResource

		''' <summary>
		''' Returns the white color. This returns opaque white
		''' ({@code 0xFFFFFFFF}).
		''' </summary>
		''' <returns> the white color </returns>
		Protected Friend Overridable Property white As ColorUIResource
			Get
				Return white
			End Get
		End Property

		''' <summary>
		''' Returns the black color. This returns opaque black
		''' ({@code 0xFF000000}).
		''' </summary>
		''' <returns> the black color </returns>
		Protected Friend Overridable Property black As ColorUIResource
			Get
				Return black
			End Get
		End Property

		''' <summary>
		''' Returns the focus color. This returns the value of
		''' {@code getPrimary2()}.
		''' </summary>
		''' <returns> the focus color </returns>
		Public Overridable Property focusColor As ColorUIResource
			Get
				Return primary2
			End Get
		End Property

		''' <summary>
		''' Returns the desktop color. This returns the value of
		''' {@code getPrimary2()}.
		''' </summary>
		''' <returns> the desktop color </returns>
		Public Overridable Property desktopColor As ColorUIResource
			Get
				Return primary2
			End Get
		End Property

		''' <summary>
		''' Returns the control color. This returns the value of
		''' {@code getSecondary3()}.
		''' </summary>
		''' <returns> the control color </returns>
		Public Overridable Property control As ColorUIResource
			Get
				Return secondary3
			End Get
		End Property

		''' <summary>
		''' Returns the control shadow color. This returns
		''' the value of {@code getSecondary2()}.
		''' </summary>
		''' <returns> the control shadow color </returns>
		Public Overridable Property controlShadow As ColorUIResource
			Get
				Return secondary2
			End Get
		End Property

		''' <summary>
		''' Returns the control dark shadow color. This returns
		''' the value of {@code getSecondary1()}.
		''' </summary>
		''' <returns> the control dark shadow color </returns>
		Public Overridable Property controlDarkShadow As ColorUIResource
			Get
				Return secondary1
			End Get
		End Property

		''' <summary>
		''' Returns the control info color. This returns
		''' the value of {@code getBlack()}.
		''' </summary>
		''' <returns> the control info color </returns>
		Public Overridable Property controlInfo As ColorUIResource
			Get
				Return black
			End Get
		End Property

		''' <summary>
		''' Returns the control highlight color. This returns
		''' the value of {@code getWhite()}.
		''' </summary>
		''' <returns> the control highlight color </returns>
		Public Overridable Property controlHighlight As ColorUIResource
			Get
				Return white
			End Get
		End Property

		''' <summary>
		''' Returns the control disabled color. This returns
		''' the value of {@code getSecondary2()}.
		''' </summary>
		''' <returns> the control disabled color </returns>
		Public Overridable Property controlDisabled As ColorUIResource
			Get
				Return secondary2
			End Get
		End Property

		''' <summary>
		''' Returns the primary control color. This returns
		''' the value of {@code getPrimary3()}.
		''' </summary>
		''' <returns> the primary control color </returns>
		Public Overridable Property primaryControl As ColorUIResource
			Get
				Return primary3
			End Get
		End Property

		''' <summary>
		''' Returns the primary control shadow color. This returns
		''' the value of {@code getPrimary2()}.
		''' </summary>
		''' <returns> the primary control shadow color </returns>
		Public Overridable Property primaryControlShadow As ColorUIResource
			Get
				Return primary2
			End Get
		End Property
		''' <summary>
		''' Returns the primary control dark shadow color. This
		''' returns the value of {@code getPrimary1()}.
		''' </summary>
		''' <returns> the primary control dark shadow color </returns>
		Public Overridable Property primaryControlDarkShadow As ColorUIResource
			Get
				Return primary1
			End Get
		End Property

		''' <summary>
		''' Returns the primary control info color. This
		''' returns the value of {@code getBlack()}.
		''' </summary>
		''' <returns> the primary control info color </returns>
		Public Overridable Property primaryControlInfo As ColorUIResource
			Get
				Return black
			End Get
		End Property

		''' <summary>
		''' Returns the primary control highlight color. This
		''' returns the value of {@code getWhite()}.
		''' </summary>
		''' <returns> the primary control highlight color </returns>
		Public Overridable Property primaryControlHighlight As ColorUIResource
			Get
				Return white
			End Get
		End Property

		''' <summary>
		''' Returns the system text color. This returns the value of
		''' {@code getBlack()}.
		''' </summary>
		''' <returns> the system text color </returns>
		Public Overridable Property systemTextColor As ColorUIResource
			Get
				Return black
			End Get
		End Property

		''' <summary>
		''' Returns the control text color. This returns the value of
		''' {@code getControlInfo()}.
		''' </summary>
		''' <returns> the control text color </returns>
		Public Overridable Property controlTextColor As ColorUIResource
			Get
				Return controlInfo
			End Get
		End Property

		''' <summary>
		''' Returns the inactive control text color. This returns the value of
		''' {@code getControlDisabled()}.
		''' </summary>
		''' <returns> the inactive control text color </returns>
		Public Overridable Property inactiveControlTextColor As ColorUIResource
			Get
				Return controlDisabled
			End Get
		End Property

		''' <summary>
		''' Returns the inactive system text color. This returns the value of
		''' {@code getSecondary2()}.
		''' </summary>
		''' <returns> the inactive system text color </returns>
		Public Overridable Property inactiveSystemTextColor As ColorUIResource
			Get
				Return secondary2
			End Get
		End Property

		''' <summary>
		''' Returns the user text color. This returns the value of
		''' {@code getBlack()}.
		''' </summary>
		''' <returns> the user text color </returns>
		Public Overridable Property userTextColor As ColorUIResource
			Get
				Return black
			End Get
		End Property

		''' <summary>
		''' Returns the text highlight color. This returns the value of
		''' {@code getPrimary3()}.
		''' </summary>
		''' <returns> the text highlight color </returns>
		Public Overridable Property textHighlightColor As ColorUIResource
			Get
				Return primary3
			End Get
		End Property

		''' <summary>
		''' Returns the highlighted text color. This returns the value of
		''' {@code getControlTextColor()}.
		''' </summary>
		''' <returns> the highlighted text color </returns>
		Public Overridable Property highlightedTextColor As ColorUIResource
			Get
				Return controlTextColor
			End Get
		End Property

		''' <summary>
		''' Returns the window background color. This returns the value of
		''' {@code getWhite()}.
		''' </summary>
		''' <returns> the window background color </returns>
		Public Overridable Property windowBackground As ColorUIResource
			Get
				Return white
			End Get
		End Property

		''' <summary>
		''' Returns the window title background color. This returns the value of
		''' {@code getPrimary3()}.
		''' </summary>
		''' <returns> the window title background color </returns>
		Public Overridable Property windowTitleBackground As ColorUIResource
			Get
				Return primary3
			End Get
		End Property

		''' <summary>
		''' Returns the window title foreground color. This returns the value of
		''' {@code getBlack()}.
		''' </summary>
		''' <returns> the window title foreground color </returns>
		Public Overridable Property windowTitleForeground As ColorUIResource
			Get
				Return black
			End Get
		End Property

		''' <summary>
		''' Returns the window title inactive background color. This
		''' returns the value of {@code getSecondary3()}.
		''' </summary>
		''' <returns> the window title inactive background color </returns>
		Public Overridable Property windowTitleInactiveBackground As ColorUIResource
			Get
				Return secondary3
			End Get
		End Property

		''' <summary>
		''' Returns the window title inactive foreground color. This
		''' returns the value of {@code getBlack()}.
		''' </summary>
		''' <returns> the window title inactive foreground color </returns>
		Public Overridable Property windowTitleInactiveForeground As ColorUIResource
			Get
				Return black
			End Get
		End Property

		''' <summary>
		''' Returns the menu background color. This
		''' returns the value of {@code getSecondary3()}.
		''' </summary>
		''' <returns> the menu background color </returns>
		Public Overridable Property menuBackground As ColorUIResource
			Get
				Return secondary3
			End Get
		End Property

		''' <summary>
		''' Returns the menu foreground color. This
		''' returns the value of {@code getBlack()}.
		''' </summary>
		''' <returns> the menu foreground color </returns>
		Public Overridable Property menuForeground As ColorUIResource
			Get
				Return black
			End Get
		End Property

		''' <summary>
		''' Returns the menu selected background color. This
		''' returns the value of {@code getPrimary2()}.
		''' </summary>
		''' <returns> the menu selected background color </returns>
		Public Overridable Property menuSelectedBackground As ColorUIResource
			Get
				Return primary2
			End Get
		End Property

		''' <summary>
		''' Returns the menu selected foreground color. This
		''' returns the value of {@code getBlack()}.
		''' </summary>
		''' <returns> the menu selected foreground color </returns>
		Public Overridable Property menuSelectedForeground As ColorUIResource
			Get
				Return black
			End Get
		End Property

		''' <summary>
		''' Returns the menu disabled foreground color. This
		''' returns the value of {@code getSecondary2()}.
		''' </summary>
		''' <returns> the menu disabled foreground color </returns>
		Public Overridable Property menuDisabledForeground As ColorUIResource
			Get
				Return secondary2
			End Get
		End Property

		''' <summary>
		''' Returns the separator background color. This
		''' returns the value of {@code getWhite()}.
		''' </summary>
		''' <returns> the separator background color </returns>
		Public Overridable Property separatorBackground As ColorUIResource
			Get
				Return white
			End Get
		End Property

		''' <summary>
		''' Returns the separator foreground color. This
		''' returns the value of {@code getPrimary1()}.
		''' </summary>
		''' <returns> the separator foreground color </returns>
		Public Overridable Property separatorForeground As ColorUIResource
			Get
				Return primary1
			End Get
		End Property

		''' <summary>
		''' Returns the accelerator foreground color. This
		''' returns the value of {@code getPrimary1()}.
		''' </summary>
		''' <returns> the accelerator foreground color </returns>
		Public Overridable Property acceleratorForeground As ColorUIResource
			Get
				Return primary1
			End Get
		End Property

		''' <summary>
		''' Returns the accelerator selected foreground color. This
		''' returns the value of {@code getBlack()}.
		''' </summary>
		''' <returns> the accelerator selected foreground color </returns>
		Public Overridable Property acceleratorSelectedForeground As ColorUIResource
			Get
				Return black
			End Get
		End Property

		''' <summary>
		''' Adds values specific to this theme to the defaults table. This method
		''' is invoked when the look and feel defaults are obtained from
		''' {@code MetalLookAndFeel}.
		''' <p>
		''' This implementation does nothing; it is provided for subclasses
		''' that wish to customize the defaults table.
		''' </summary>
		''' <param name="table"> the {@code UIDefaults} to add the values to
		''' </param>
		''' <seealso cref= MetalLookAndFeel#getDefaults </seealso>
		Public Overridable Sub addCustomEntriesToTable(ByVal table As UIDefaults)
		End Sub

		''' <summary>
		''' This is invoked when a MetalLookAndFeel is installed and about to
		''' start using this theme. When we can add API this should be nuked
		''' in favor of DefaultMetalTheme overriding addCustomEntriesToTable.
		''' </summary>
		Friend Overridable Sub install()
		End Sub

		''' <summary>
		''' Returns true if this is a theme provided by the core platform.
		''' </summary>
		Friend Overridable Property systemTheme As Boolean
			Get
				Return False
			End Get
		End Property
	End Class

End Namespace
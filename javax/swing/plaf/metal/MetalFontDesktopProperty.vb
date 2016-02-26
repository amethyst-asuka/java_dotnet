Imports Microsoft.VisualBasic

'
' * Copyright (c) 2001, 2009, Oracle and/or its affiliates. All rights reserved.
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
	''' DesktopProperty that only uses font height in configuring font. This
	''' is only used on Windows.
	''' 
	''' </summary>
	Friend Class MetalFontDesktopProperty
		Inherits com.sun.java.swing.plaf.windows.DesktopProperty

		''' <summary>
		''' Maps from metal font theme type as defined in MetalTheme
		''' to the corresponding desktop property name.
		''' </summary>
		Private Shared ReadOnly propertyMapping As String() = { "win.ansiVar.font.height", "win.tooltip.font.height", "win.ansiVar.font.height", "win.menu.font.height", "win.frame.captionFont.height", "win.menu.font.height" }

		''' <summary>
		''' Corresponds to a MetalTheme font type.
		''' </summary>
		Private type As Integer


		''' <summary>
		''' Creates a MetalFontDesktopProperty. The key used to lookup the
		''' desktop property is determined from the type of font.
		''' </summary>
		''' <param name="type"> MetalTheme font type. </param>
		Friend Sub New(ByVal type As Integer)
			Me.New(propertyMapping(type), type)
		End Sub

		''' <summary>
		''' Creates a MetalFontDesktopProperty.
		''' </summary>
		''' <param name="key"> Key used in looking up desktop value. </param>
		''' <param name="toolkit"> Toolkit used to fetch property from, can be null
		'''        in which default will be used. </param>
		''' <param name="type"> Type of font being used, corresponds to MetalTheme font
		'''        type. </param>
		Friend Sub New(ByVal key As String, ByVal type As Integer)
			MyBase.New(key, Nothing)
			Me.type = type
		End Sub

		''' <summary>
		''' Overriden to create a Font with the size coming from the desktop
		''' and the style and name coming from DefaultMetalTheme.
		''' </summary>
		Protected Friend Overridable Function configureValue(ByVal value As Object) As Object
			If TypeOf value Is Integer? Then value = New Font(DefaultMetalTheme.getDefaultFontName(type), DefaultMetalTheme.getDefaultFontStyle(type), CInt(Fix(value)))
			Return MyBase.configureValue(value)
		End Function

		''' <summary>
		''' Returns the default font.
		''' </summary>
		Protected Friend Overridable Property defaultValue As Object
			Get
				Return New Font(DefaultMetalTheme.getDefaultFontName(type), DefaultMetalTheme.getDefaultFontStyle(type), DefaultMetalTheme.getDefaultFontSize(type))
			End Get
		End Property
	End Class

End Namespace
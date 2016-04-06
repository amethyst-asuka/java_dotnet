Imports System

'
' * Copyright (c) 1996, 2014, Oracle and/or its affiliates. All rights reserved.
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
Namespace java.awt




	''' <summary>
	''' A class to encapsulate symbolic colors representing the color of
	''' native GUI objects on a system.  For systems which support the dynamic
	''' update of the system colors (when the user changes the colors)
	''' the actual RGB values of these symbolic colors will also change
	''' dynamically.  In order to compare the "current" RGB value of a
	''' <code>SystemColor</code> object with a non-symbolic Color object,
	''' <code>getRGB</code> should be used rather than <code>equals</code>.
	''' <p>
	''' Note that the way in which these system colors are applied to GUI objects
	''' may vary slightly from platform to platform since GUI objects may be
	''' rendered differently on each platform.
	''' <p>
	''' System color values may also be available through the <code>getDesktopProperty</code>
	''' method on <code>java.awt.Toolkit</code>.
	''' </summary>
	''' <seealso cref= Toolkit#getDesktopProperty
	''' 
	''' @author      Carl Quinn
	''' @author      Amy Fowler </seealso>
	<Serializable> _
	Public NotInheritable Class SystemColor
		Inherits Color

	   ''' <summary>
	   ''' The array index for the
	   ''' <seealso cref="#desktop"/> system color. </summary>
	   ''' <seealso cref= SystemColor#desktop </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const DESKTOP As Integer = 0

		''' <summary>
		''' The array index for the
		''' <seealso cref="#activeCaption"/> system color. </summary>
		''' <seealso cref= SystemColor#activeCaption </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const ACTIVE_CAPTION As Integer = 1

		''' <summary>
		''' The array index for the
		''' <seealso cref="#activeCaptionText"/> system color. </summary>
		''' <seealso cref= SystemColor#activeCaptionText </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const ACTIVE_CAPTION_TEXT As Integer = 2

		''' <summary>
		''' The array index for the
		''' <seealso cref="#activeCaptionBorder"/> system color. </summary>
		''' <seealso cref= SystemColor#activeCaptionBorder </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const ACTIVE_CAPTION_BORDER As Integer = 3

		''' <summary>
		''' The array index for the
		''' <seealso cref="#inactiveCaption"/> system color. </summary>
		''' <seealso cref= SystemColor#inactiveCaption </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const INACTIVE_CAPTION As Integer = 4

		''' <summary>
		''' The array index for the
		''' <seealso cref="#inactiveCaptionText"/> system color. </summary>
		''' <seealso cref= SystemColor#inactiveCaptionText </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const INACTIVE_CAPTION_TEXT As Integer = 5

		''' <summary>
		''' The array index for the
		''' <seealso cref="#inactiveCaptionBorder"/> system color. </summary>
		''' <seealso cref= SystemColor#inactiveCaptionBorder </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const INACTIVE_CAPTION_BORDER As Integer = 6

		''' <summary>
		''' The array index for the
		''' <seealso cref="#window"/> system color. </summary>
		''' <seealso cref= SystemColor#window </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const WINDOW As Integer = 7

		''' <summary>
		''' The array index for the
		''' <seealso cref="#windowBorder"/> system color. </summary>
		''' <seealso cref= SystemColor#windowBorder </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const WINDOW_BORDER As Integer = 8

		''' <summary>
		''' The array index for the
		''' <seealso cref="#windowText"/> system color. </summary>
		''' <seealso cref= SystemColor#windowText </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const WINDOW_TEXT As Integer = 9

		''' <summary>
		''' The array index for the
		''' <seealso cref="#menu"/> system color. </summary>
		''' <seealso cref= SystemColor#menu </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const MENU As Integer = 10

		''' <summary>
		''' The array index for the
		''' <seealso cref="#menuText"/> system color. </summary>
		''' <seealso cref= SystemColor#menuText </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const MENU_TEXT As Integer = 11

		''' <summary>
		''' The array index for the
		''' <seealso cref="#text"/> system color. </summary>
		''' <seealso cref= SystemColor#text </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const TEXT As Integer = 12

		''' <summary>
		''' The array index for the
		''' <seealso cref="#textText"/> system color. </summary>
		''' <seealso cref= SystemColor#textText </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const TEXT_TEXT As Integer = 13

		''' <summary>
		''' The array index for the
		''' <seealso cref="#textHighlight"/> system color. </summary>
		''' <seealso cref= SystemColor#textHighlight </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const TEXT_HIGHLIGHT As Integer = 14

		''' <summary>
		''' The array index for the
		''' <seealso cref="#textHighlightText"/> system color. </summary>
		''' <seealso cref= SystemColor#textHighlightText </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const TEXT_HIGHLIGHT_TEXT As Integer = 15

		''' <summary>
		''' The array index for the
		''' <seealso cref="#textInactiveText"/> system color. </summary>
		''' <seealso cref= SystemColor#textInactiveText </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const TEXT_INACTIVE_TEXT As Integer = 16

		''' <summary>
		''' The array index for the
		''' <seealso cref="#control"/> system color. </summary>
		''' <seealso cref= SystemColor#control </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const CONTROL As Integer = 17

		''' <summary>
		''' The array index for the
		''' <seealso cref="#controlText"/> system color. </summary>
		''' <seealso cref= SystemColor#controlText </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const CONTROL_TEXT As Integer = 18

		''' <summary>
		''' The array index for the
		''' <seealso cref="#controlHighlight"/> system color. </summary>
		''' <seealso cref= SystemColor#controlHighlight </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const CONTROL_HIGHLIGHT As Integer = 19

		''' <summary>
		''' The array index for the
		''' <seealso cref="#controlLtHighlight"/> system color. </summary>
		''' <seealso cref= SystemColor#controlLtHighlight </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const CONTROL_LT_HIGHLIGHT As Integer = 20

		''' <summary>
		''' The array index for the
		''' <seealso cref="#controlShadow"/> system color. </summary>
		''' <seealso cref= SystemColor#controlShadow </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const CONTROL_SHADOW As Integer = 21

		''' <summary>
		''' The array index for the
		''' <seealso cref="#controlDkShadow"/> system color. </summary>
		''' <seealso cref= SystemColor#controlDkShadow </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const CONTROL_DK_SHADOW As Integer = 22

		''' <summary>
		''' The array index for the
		''' <seealso cref="#scrollbar"/> system color. </summary>
		''' <seealso cref= SystemColor#scrollbar </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const SCROLLBAR As Integer = 23

		''' <summary>
		''' The array index for the
		''' <seealso cref="#info"/> system color. </summary>
		''' <seealso cref= SystemColor#info </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const INFO As Integer = 24

		''' <summary>
		''' The array index for the
		''' <seealso cref="#infoText"/> system color. </summary>
		''' <seealso cref= SystemColor#infoText </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const INFO_TEXT As Integer = 25

		''' <summary>
		''' The number of system colors in the array.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Const NUM_COLORS As Integer = 26

		''' <summary>
		'''*************************************************************************************** </summary>

	'    
	'     * System colors with default initial values, overwritten by toolkit if
	'     * system values differ and are available.
	'     * Should put array initialization above first field that is using
	'     * SystemColor constructor to initialize.
	'     
		Private Shared systemColors As Integer() = { &HFF005C5CL, &HFF000080L, &HFFFFFFFFL, &HFFC0C0C0L, &HFF808080L, &HFFC0C0C0L, &HFFC0C0C0L, &HFFFFFFFFL, &HFF000000L, &HFF000000L, &HFFC0C0C0L, &HFF000000L, &HFFC0C0C0L, &HFF000000L, &HFF000080L, &HFFFFFFFFL, &HFF808080L, &HFFC0C0C0L, &HFF000000L, &HFFFFFFFFL, &HFFE0E0E0L, &HFF808080L, &HFF000000L, &HFFE0E0E0L, &HFFE0E000L, &HFF000000L }

	   ''' <summary>
	   ''' The color rendered for the background of the desktop.
	   ''' </summary>
		Public Shared ReadOnly desktop_Renamed As New SystemColor(CSByte(DESKTOP))

		''' <summary>
		''' The color rendered for the window-title background of the currently active window.
		''' </summary>
		Public Shared ReadOnly activeCaption As New SystemColor(CSByte(ACTIVE_CAPTION))

		''' <summary>
		''' The color rendered for the window-title text of the currently active window.
		''' </summary>
		Public Shared ReadOnly activeCaptionText As New SystemColor(CSByte(ACTIVE_CAPTION_TEXT))

		''' <summary>
		''' The color rendered for the border around the currently active window.
		''' </summary>
		Public Shared ReadOnly activeCaptionBorder As New SystemColor(CSByte(ACTIVE_CAPTION_BORDER))

		''' <summary>
		''' The color rendered for the window-title background of inactive windows.
		''' </summary>
		Public Shared ReadOnly inactiveCaption As New SystemColor(CSByte(INACTIVE_CAPTION))

		''' <summary>
		''' The color rendered for the window-title text of inactive windows.
		''' </summary>
		Public Shared ReadOnly inactiveCaptionText As New SystemColor(CSByte(INACTIVE_CAPTION_TEXT))

		''' <summary>
		''' The color rendered for the border around inactive windows.
		''' </summary>
		Public Shared ReadOnly inactiveCaptionBorder As New SystemColor(CSByte(INACTIVE_CAPTION_BORDER))

		''' <summary>
		''' The color rendered for the background of interior regions inside windows.
		''' </summary>
		Public Shared ReadOnly window As New SystemColor(CSByte(WINDOW))

		''' <summary>
		''' The color rendered for the border around interior regions inside windows.
		''' </summary>
		Public Shared ReadOnly windowBorder As New SystemColor(CSByte(WINDOW_BORDER))

		''' <summary>
		''' The color rendered for text of interior regions inside windows.
		''' </summary>
		Public Shared ReadOnly windowText As New SystemColor(CSByte(WINDOW_TEXT))

		''' <summary>
		''' The color rendered for the background of menus.
		''' </summary>
		Public Shared ReadOnly menu_Renamed As New SystemColor(CSByte(MENU))

		''' <summary>
		''' The color rendered for the text of menus.
		''' </summary>
		Public Shared ReadOnly menuText As New SystemColor(CSByte(MENU_TEXT))

		''' <summary>
		''' The color rendered for the background of text control objects, such as
		''' textfields and comboboxes.
		''' </summary>
		Public Shared ReadOnly text As New SystemColor(CSByte(TEXT))

		''' <summary>
		''' The color rendered for the text of text control objects, such as textfields
		''' and comboboxes.
		''' </summary>
		Public Shared ReadOnly textText As New SystemColor(CSByte(TEXT_TEXT))

		''' <summary>
		''' The color rendered for the background of selected items, such as in menus,
		''' comboboxes, and text.
		''' </summary>
		Public Shared ReadOnly textHighlight As New SystemColor(CSByte(TEXT_HIGHLIGHT))

		''' <summary>
		''' The color rendered for the text of selected items, such as in menus, comboboxes,
		''' and text.
		''' </summary>
		Public Shared ReadOnly textHighlightText As New SystemColor(CSByte(TEXT_HIGHLIGHT_TEXT))

		''' <summary>
		''' The color rendered for the text of inactive items, such as in menus.
		''' </summary>
		Public Shared ReadOnly textInactiveText As New SystemColor(CSByte(TEXT_INACTIVE_TEXT))

		''' <summary>
		''' The color rendered for the background of control panels and control objects,
		''' such as pushbuttons.
		''' </summary>
		Public Shared ReadOnly control As New SystemColor(CSByte(CONTROL))

		''' <summary>
		''' The color rendered for the text of control panels and control objects,
		''' such as pushbuttons.
		''' </summary>
		Public Shared ReadOnly controlText As New SystemColor(CSByte(CONTROL_TEXT))

		''' <summary>
		''' The color rendered for light areas of 3D control objects, such as pushbuttons.
		''' This color is typically derived from the <code>control</code> background color
		''' to provide a 3D effect.
		''' </summary>
		Public Shared ReadOnly controlHighlight As New SystemColor(CSByte(CONTROL_HIGHLIGHT))

		''' <summary>
		''' The color rendered for highlight areas of 3D control objects, such as pushbuttons.
		''' This color is typically derived from the <code>control</code> background color
		''' to provide a 3D effect.
		''' </summary>
		Public Shared ReadOnly controlLtHighlight As New SystemColor(CSByte(CONTROL_LT_HIGHLIGHT))

		''' <summary>
		''' The color rendered for shadow areas of 3D control objects, such as pushbuttons.
		''' This color is typically derived from the <code>control</code> background color
		''' to provide a 3D effect.
		''' </summary>
		Public Shared ReadOnly controlShadow As New SystemColor(CSByte(CONTROL_SHADOW))

		''' <summary>
		''' The color rendered for dark shadow areas on 3D control objects, such as pushbuttons.
		''' This color is typically derived from the <code>control</code> background color
		''' to provide a 3D effect.
		''' </summary>
		Public Shared ReadOnly controlDkShadow As New SystemColor(CSByte(CONTROL_DK_SHADOW))

		''' <summary>
		''' The color rendered for the background of scrollbars.
		''' </summary>
		Public Shared ReadOnly scrollbar_Renamed As New SystemColor(CSByte(SCROLLBAR))

		''' <summary>
		''' The color rendered for the background of tooltips or spot help.
		''' </summary>
		Public Shared ReadOnly info As New SystemColor(CSByte(INFO))

		''' <summary>
		''' The color rendered for the text of tooltips or spot help.
		''' </summary>
		Public Shared ReadOnly infoText As New SystemColor(CSByte(INFO_TEXT))

	'    
	'     * JDK 1.1 serialVersionUID.
	'     
		Private Const serialVersionUID As Long = 4503142729533789064L

	'    
	'     * An index into either array of SystemColor objects or values.
	'     
		<NonSerialized> _
		Private index As Integer

		Private Shared systemColorObjects As SystemColor() = { SystemColor.desktop_Renamed, SystemColor.activeCaption, SystemColor.activeCaptionText, SystemColor.activeCaptionBorder, SystemColor.inactiveCaption, SystemColor.inactiveCaptionText, SystemColor.inactiveCaptionBorder, SystemColor.window, SystemColor.windowBorder, SystemColor.windowText, SystemColor.menu_Renamed, SystemColor.menuText, SystemColor.text, SystemColor.textText, SystemColor.textHighlight, SystemColor.textHighlightText, SystemColor.textInactiveText, SystemColor.control, SystemColor.controlText, SystemColor.controlHighlight, SystemColor.controlLtHighlight, SystemColor.controlShadow, SystemColor.controlDkShadow, SystemColor.scrollbar_Renamed, SystemColor.info, SystemColor.infoText }

		Shared Sub New()
			sun.awt.AWTAccessor.systemColorAccessor = SystemColor::updateSystemColors
			updateSystemColors()
		End Sub

		''' <summary>
		''' Called from {@code <init>} and toolkit to update the above systemColors cache.
		''' </summary>
		Private Shared Sub updateSystemColors()
			If Not GraphicsEnvironment.headless Then Toolkit.defaultToolkit.loadSystemColors(systemColors)
			For i As Integer = 0 To systemColors.Length - 1
				systemColorObjects(i).value = systemColors(i)
			Next i
		End Sub

		''' <summary>
		''' Creates a symbolic color that represents an indexed entry into system
		''' color cache. Used by above static system colors.
		''' </summary>
		Private Sub New(  index As SByte)
			MyBase.New(systemColors(index))
			Me.index = index
		End Sub

		''' <summary>
		''' Returns a string representation of this <code>Color</code>'s values.
		''' This method is intended to be used only for debugging purposes,
		''' and the content and format of the returned string may vary between
		''' implementations.
		''' The returned string may be empty but may not be <code>null</code>.
		''' </summary>
		''' <returns>  a string representation of this <code>Color</code> </returns>
		Public Overrides Function ToString() As String
			Return Me.GetType().name & "[i=" & (index) & "]"
		End Function

		''' <summary>
		''' The design of the {@code SystemColor} class assumes that
		''' the {@code SystemColor} object instances stored in the
		''' static final fields above are the only instances that can
		''' be used by developers.
		''' This method helps maintain those limits on instantiation
		''' by using the index stored in the value field of the
		''' serialized form of the object to replace the serialized
		''' object with the equivalent static object constant field
		''' of {@code SystemColor}.
		''' See the <seealso cref="#writeReplace"/> method for more information
		''' on the serialized form of these objects. </summary>
		''' <returns> one of the {@code SystemColor} static object
		'''         fields that refers to the same system color. </returns>
		Private Function readResolve() As Object
			' The instances of SystemColor are tightly controlled and
			' only the canonical instances appearing above as static
			' constants are allowed.  The serial form of SystemColor
			' objects stores the color index as the value.  Here we
			' map that index back into the canonical instance.
			Return systemColorObjects(value)
		End Function

		''' <summary>
		''' Returns a specialized version of the {@code SystemColor}
		''' object for writing to the serialized stream.
		''' @serialData
		''' The value field of a serialized {@code SystemColor} object
		''' contains the array index of the system color instead of the
		''' rgb data for the system color.
		''' This index is used by the <seealso cref="#readResolve"/> method to
		''' resolve the deserialized objects back to the original
		''' static constant versions to ensure unique instances of
		''' each {@code SystemColor} object. </summary>
		''' <returns> a proxy {@code SystemColor} object with its value
		'''         replaced by the corresponding system color index. </returns>
		Private Function writeReplace() As Object
			' we put an array index in the SystemColor.value while serialize
			' to keep compatibility.
			Dim color_Renamed As New SystemColor(CSByte(index))
			color_Renamed.value = index
			Return color_Renamed
		End Function
	End Class

End Namespace
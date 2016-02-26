'
' * Copyright (c) 1997, 1998, Oracle and/or its affiliates. All rights reserved.
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
Namespace javax.swing.text.rtf

	''' <summary>
	'''   Class to hold dictionary keys used by the RTF reader/writer.
	'''   These should be moved into StyleConstants.
	''' </summary>
	Friend Class Constants
		''' <summary>
		''' An array of TabStops </summary>
		Friend Const Tabs As String = "tabs"

		''' <summary>
		''' The name of the character set the original RTF file was in </summary>
		Friend Const RTFCharacterSet As String = "rtfCharacterSet"

		''' <summary>
		''' Indicates the domain of a Style </summary>
		Friend Const StyleType As String = "style:type"

		''' <summary>
		''' Value for StyleType indicating a section style </summary>
		Friend Const STSection As String = "section"
		''' <summary>
		''' Value for StyleType indicating a paragraph style </summary>
		Friend Const STParagraph As String = "paragraph"
		''' <summary>
		''' Value for StyleType indicating a character style </summary>
		Friend Const STCharacter As String = "character"

		''' <summary>
		''' The style of the text following this style </summary>
		Friend Const StyleNext As String = "style:nextStyle"

		''' <summary>
		''' Whether the style is additive </summary>
		Friend Const StyleAdditive As String = "style:additive"

		''' <summary>
		''' Whether the style is hidden from the user </summary>
		Friend Const StyleHidden As String = "style:hidden"

		' Miscellaneous character attributes 
		Friend Const Caps As String = "caps"
		Friend Const Deleted As String = "deleted"
		Friend Const Outline As String = "outl"
		Friend Const SmallCaps As String = "scaps"
		Friend Const Shadow As String = "shad"
		Friend Const Strikethrough As String = "strike"
		Friend Const Hidden As String = "v"

		' Miscellaneous document attributes 
		Friend Const PaperWidth As String = "paperw"
		Friend Const PaperHeight As String = "paperh"
		Friend Const MarginLeft As String = "margl"
		Friend Const MarginRight As String = "margr"
		Friend Const MarginTop As String = "margt"
		Friend Const MarginBottom As String = "margb"
		Friend Const GutterWidth As String = "gutter"

		' This is both a document and a paragraph attribute 
		Friend Const WidowControl As String = "widowctrl"
	End Class

End Namespace
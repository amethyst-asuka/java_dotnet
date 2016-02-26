Imports Microsoft.VisualBasic
Imports System

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
Namespace javax.swing.text


	''' <summary>
	''' <p>
	''' A collection of <em>well known</em> or common attribute keys
	''' and methods to apply to an AttributeSet or MutableAttributeSet
	''' to get/set the properties in a typesafe manner.
	''' <p>
	''' The paragraph attributes form the definition of a paragraph to be rendered.
	''' All sizes are specified in points (such as found in postscript), a
	''' device independent measure.
	''' </p>
	''' <p style="text-align:center"><img src="doc-files/paragraph.gif"
	''' alt="Diagram shows SpaceAbove, FirstLineIndent, LeftIndent, RightIndent,
	'''      and SpaceBelow a paragraph."></p>
	''' <p>
	''' 
	''' @author  Timothy Prinzing
	''' </summary>
	Public Class StyleConstants

		''' <summary>
		''' Name of elements used to represent components.
		''' </summary>
		Public Const ComponentElementName As String = "component"

		''' <summary>
		''' Name of elements used to represent icons.
		''' </summary>
		Public Const IconElementName As String = "icon"

		''' <summary>
		''' Attribute name used to name the collection of
		''' attributes.
		''' </summary>
		Public Shared ReadOnly NameAttribute As Object = New StyleConstants("name")

		''' <summary>
		''' Attribute name used to identify the resolving parent
		''' set of attributes, if one is defined.
		''' </summary>
		Public Shared ReadOnly ResolveAttribute As Object = New StyleConstants("resolver")

		''' <summary>
		''' Attribute used to identify the model for embedded
		''' objects that have a model view separation.
		''' </summary>
		Public Shared ReadOnly ModelAttribute As Object = New StyleConstants("model")

		''' <summary>
		''' Returns the string representation.
		''' </summary>
		''' <returns> the string </returns>
		Public Overrides Function ToString() As String
			Return representation
		End Function

		' ---- character constants -----------------------------------

		''' <summary>
		''' Bidirectional level of a character as assigned by the Unicode bidi
		''' algorithm.
		''' </summary>
		Public Shared ReadOnly BidiLevel As Object = New CharacterConstants("bidiLevel")

		''' <summary>
		''' Name of the font family.
		''' </summary>
		Public Shared ReadOnly FontFamily As Object = New FontConstants("family")

		''' <summary>
		''' Name of the font family.
		''' 
		''' @since 1.5
		''' </summary>
		Public Shared ReadOnly Family As Object = FontFamily

		''' <summary>
		''' Name of the font size.
		''' </summary>
		Public Shared ReadOnly FontSize As Object = New FontConstants("size")

		''' <summary>
		''' Name of the font size.
		''' 
		''' @since 1.5
		''' </summary>
		Public Shared ReadOnly Size As Object = FontSize

		''' <summary>
		''' Name of the bold attribute.
		''' </summary>
		Public Shared ReadOnly Bold As Object = New FontConstants("bold")

		''' <summary>
		''' Name of the italic attribute.
		''' </summary>
		Public Shared ReadOnly Italic As Object = New FontConstants("italic")

		''' <summary>
		''' Name of the underline attribute.
		''' </summary>
		Public Shared ReadOnly Underline As Object = New CharacterConstants("underline")

		''' <summary>
		''' Name of the Strikethrough attribute.
		''' </summary>
		Public Shared ReadOnly StrikeThrough As Object = New CharacterConstants("strikethrough")

		''' <summary>
		''' Name of the Superscript attribute.
		''' </summary>
		Public Shared ReadOnly Superscript As Object = New CharacterConstants("superscript")

		''' <summary>
		''' Name of the Subscript attribute.
		''' </summary>
		Public Shared ReadOnly Subscript As Object = New CharacterConstants("subscript")

		''' <summary>
		''' Name of the foreground color attribute.
		''' </summary>
		Public Shared ReadOnly Foreground As Object = New ColorConstants("foreground")

		''' <summary>
		''' Name of the background color attribute.
		''' </summary>
		Public Shared ReadOnly Background As Object = New ColorConstants("background")

		''' <summary>
		''' Name of the component attribute.
		''' </summary>
		Public Shared ReadOnly ComponentAttribute As Object = New CharacterConstants("component")

		''' <summary>
		''' Name of the icon attribute.
		''' </summary>
		Public Shared ReadOnly IconAttribute As Object = New CharacterConstants("icon")

		''' <summary>
		''' Name of the input method composed text attribute. The value of
		''' this attribute is an instance of AttributedString which represents
		''' the composed text.
		''' </summary>
		Public Shared ReadOnly ComposedTextAttribute As Object = New StyleConstants("composed text")

		''' <summary>
		''' The amount of space to indent the first
		''' line of the paragraph.  This value may be negative
		''' to offset in the reverse direction.  The type
		''' is Float and specifies the size of the space
		''' in points.
		''' </summary>
		Public Shared ReadOnly FirstLineIndent As Object = New ParagraphConstants("FirstLineIndent")

		''' <summary>
		''' The amount to indent the left side
		''' of the paragraph.
		''' Type is float and specifies the size in points.
		''' </summary>
		Public Shared ReadOnly LeftIndent As Object = New ParagraphConstants("LeftIndent")

		''' <summary>
		''' The amount to indent the right side
		''' of the paragraph.
		''' Type is float and specifies the size in points.
		''' </summary>
		Public Shared ReadOnly RightIndent As Object = New ParagraphConstants("RightIndent")

		''' <summary>
		''' The amount of space between lines
		''' of the paragraph.
		''' Type is float and specifies the size as a factor of the line height
		''' </summary>
		Public Shared ReadOnly LineSpacing As Object = New ParagraphConstants("LineSpacing")

		''' <summary>
		''' The amount of space above the paragraph.
		''' Type is float and specifies the size in points.
		''' </summary>
		Public Shared ReadOnly SpaceAbove As Object = New ParagraphConstants("SpaceAbove")

		''' <summary>
		''' The amount of space below the paragraph.
		''' Type is float and specifies the size in points.
		''' </summary>
		Public Shared ReadOnly SpaceBelow As Object = New ParagraphConstants("SpaceBelow")

		''' <summary>
		''' Alignment for the paragraph.  The type is
		''' Integer.  Valid values are:
		''' <ul>
		''' <li>ALIGN_LEFT
		''' <li>ALIGN_RIGHT
		''' <li>ALIGN_CENTER
		''' <li>ALIGN_JUSTIFED
		''' </ul>
		''' 
		''' </summary>
		Public Shared ReadOnly Alignment As Object = New ParagraphConstants("Alignment")

		''' <summary>
		''' TabSet for the paragraph, type is a TabSet containing
		''' TabStops.
		''' </summary>
		Public Shared ReadOnly TabSet As Object = New ParagraphConstants("TabSet")

		''' <summary>
		''' Orientation for a paragraph.
		''' </summary>
		Public Shared ReadOnly Orientation As Object = New ParagraphConstants("Orientation")
		''' <summary>
		''' A possible value for paragraph alignment.  This
		''' specifies that the text is aligned to the left
		''' indent and extra whitespace should be placed on
		''' the right.
		''' </summary>
		Public Const ALIGN_LEFT As Integer = 0

		''' <summary>
		''' A possible value for paragraph alignment.  This
		''' specifies that the text is aligned to the center
		''' and extra whitespace should be placed equally on
		''' the left and right.
		''' </summary>
		Public Const ALIGN_CENTER As Integer = 1

		''' <summary>
		''' A possible value for paragraph alignment.  This
		''' specifies that the text is aligned to the right
		''' indent and extra whitespace should be placed on
		''' the left.
		''' </summary>
		Public Const ALIGN_RIGHT As Integer = 2

		''' <summary>
		''' A possible value for paragraph alignment.  This
		''' specifies that extra whitespace should be spread
		''' out through the rows of the paragraph with the
		''' text lined up with the left and right indent
		''' except on the last line which should be aligned
		''' to the left.
		''' </summary>
		Public Const ALIGN_JUSTIFIED As Integer = 3

		' --- character attribute accessors ---------------------------

		''' <summary>
		''' Gets the BidiLevel setting.
		''' </summary>
		''' <param name="a"> the attribute set </param>
		''' <returns> the value </returns>
		Public Shared Function getBidiLevel(ByVal a As AttributeSet) As Integer
			Dim o As Integer? = CInt(Fix(a.getAttribute(BidiLevel)))
			If o IsNot Nothing Then Return o
			Return 0 ' Level 0 is base level (non-embedded) left-to-right
		End Function

		''' <summary>
		''' Sets the BidiLevel.
		''' </summary>
		''' <param name="a"> the attribute set </param>
		''' <param name="o"> the bidi level value </param>
		Public Shared Sub setBidiLevel(ByVal a As MutableAttributeSet, ByVal o As Integer)
			a.addAttribute(BidiLevel, Convert.ToInt32(o))
		End Sub

		''' <summary>
		''' Gets the component setting from the attribute list.
		''' </summary>
		''' <param name="a"> the attribute set </param>
		''' <returns> the component, null if none </returns>
		Public Shared Function getComponent(ByVal a As AttributeSet) As java.awt.Component
			Return CType(a.getAttribute(ComponentAttribute), java.awt.Component)
		End Function

		''' <summary>
		''' Sets the component attribute.
		''' </summary>
		''' <param name="a"> the attribute set </param>
		''' <param name="c"> the component </param>
		Public Shared Sub setComponent(ByVal a As MutableAttributeSet, ByVal c As java.awt.Component)
			a.addAttribute(AbstractDocument.ElementNameAttribute, ComponentElementName)
			a.addAttribute(ComponentAttribute, c)
		End Sub

		''' <summary>
		''' Gets the icon setting from the attribute list.
		''' </summary>
		''' <param name="a"> the attribute set </param>
		''' <returns> the icon, null if none </returns>
		Public Shared Function getIcon(ByVal a As AttributeSet) As javax.swing.Icon
			Return CType(a.getAttribute(IconAttribute), javax.swing.Icon)
		End Function

		''' <summary>
		''' Sets the icon attribute.
		''' </summary>
		''' <param name="a"> the attribute set </param>
		''' <param name="c"> the icon </param>
		Public Shared Sub setIcon(ByVal a As MutableAttributeSet, ByVal c As javax.swing.Icon)
			a.addAttribute(AbstractDocument.ElementNameAttribute, IconElementName)
			a.addAttribute(IconAttribute, c)
		End Sub

		''' <summary>
		''' Gets the font family setting from the attribute list.
		''' </summary>
		''' <param name="a"> the attribute set </param>
		''' <returns> the font family, "Monospaced" as the default </returns>
		Public Shared Function getFontFamily(ByVal a As AttributeSet) As String
			Dim ___family As String = CStr(a.getAttribute(FontFamily))
			If ___family Is Nothing Then ___family = "Monospaced"
			Return ___family
		End Function

		''' <summary>
		''' Sets the font attribute.
		''' </summary>
		''' <param name="a"> the attribute set </param>
		''' <param name="fam"> the font </param>
		Public Shared Sub setFontFamily(ByVal a As MutableAttributeSet, ByVal fam As String)
			a.addAttribute(FontFamily, fam)
		End Sub

		''' <summary>
		''' Gets the font size setting from the attribute list.
		''' </summary>
		''' <param name="a"> the attribute set </param>
		''' <returns> the font size, 12 as the default </returns>
		Public Shared Function getFontSize(ByVal a As AttributeSet) As Integer
			Dim ___size As Integer? = CInt(Fix(a.getAttribute(FontSize)))
			If ___size IsNot Nothing Then Return ___size
			Return 12
		End Function

		''' <summary>
		''' Sets the font size attribute.
		''' </summary>
		''' <param name="a"> the attribute set </param>
		''' <param name="s"> the font size </param>
		Public Shared Sub setFontSize(ByVal a As MutableAttributeSet, ByVal s As Integer)
			a.addAttribute(FontSize, Convert.ToInt32(s))
		End Sub

		''' <summary>
		''' Checks whether the bold attribute is set.
		''' </summary>
		''' <param name="a"> the attribute set </param>
		''' <returns> true if set else false </returns>
		Public Shared Function isBold(ByVal a As AttributeSet) As Boolean
			Dim ___bold As Boolean? = CBool(a.getAttribute(Bold))
			If ___bold IsNot Nothing Then Return ___bold
			Return False
		End Function

		''' <summary>
		''' Sets the bold attribute.
		''' </summary>
		''' <param name="a"> the attribute set </param>
		''' <param name="b"> specifies true/false for setting the attribute </param>
		Public Shared Sub setBold(ByVal a As MutableAttributeSet, ByVal b As Boolean)
			a.addAttribute(Bold, Convert.ToBoolean(b))
		End Sub

		''' <summary>
		''' Checks whether the italic attribute is set.
		''' </summary>
		''' <param name="a"> the attribute set </param>
		''' <returns> true if set else false </returns>
		Public Shared Function isItalic(ByVal a As AttributeSet) As Boolean
			Dim ___italic As Boolean? = CBool(a.getAttribute(Italic))
			If ___italic IsNot Nothing Then Return ___italic
			Return False
		End Function

		''' <summary>
		''' Sets the italic attribute.
		''' </summary>
		''' <param name="a"> the attribute set </param>
		''' <param name="b"> specifies true/false for setting the attribute </param>
		Public Shared Sub setItalic(ByVal a As MutableAttributeSet, ByVal b As Boolean)
			a.addAttribute(Italic, Convert.ToBoolean(b))
		End Sub

		''' <summary>
		''' Checks whether the underline attribute is set.
		''' </summary>
		''' <param name="a"> the attribute set </param>
		''' <returns> true if set else false </returns>
		Public Shared Function isUnderline(ByVal a As AttributeSet) As Boolean
			Dim ___underline As Boolean? = CBool(a.getAttribute(Underline))
			If ___underline IsNot Nothing Then Return ___underline
			Return False
		End Function

		''' <summary>
		''' Checks whether the strikethrough attribute is set.
		''' </summary>
		''' <param name="a"> the attribute set </param>
		''' <returns> true if set else false </returns>
		Public Shared Function isStrikeThrough(ByVal a As AttributeSet) As Boolean
			Dim strike As Boolean? = CBool(a.getAttribute(StrikeThrough))
			If strike IsNot Nothing Then Return strike
			Return False
		End Function


		''' <summary>
		''' Checks whether the superscript attribute is set.
		''' </summary>
		''' <param name="a"> the attribute set </param>
		''' <returns> true if set else false </returns>
		Public Shared Function isSuperscript(ByVal a As AttributeSet) As Boolean
			Dim ___superscript As Boolean? = CBool(a.getAttribute(Superscript))
			If ___superscript IsNot Nothing Then Return ___superscript
			Return False
		End Function


		''' <summary>
		''' Checks whether the subscript attribute is set.
		''' </summary>
		''' <param name="a"> the attribute set </param>
		''' <returns> true if set else false </returns>
		Public Shared Function isSubscript(ByVal a As AttributeSet) As Boolean
			Dim ___subscript As Boolean? = CBool(a.getAttribute(Subscript))
			If ___subscript IsNot Nothing Then Return ___subscript
			Return False
		End Function


		''' <summary>
		''' Sets the underline attribute.
		''' </summary>
		''' <param name="a"> the attribute set </param>
		''' <param name="b"> specifies true/false for setting the attribute </param>
		Public Shared Sub setUnderline(ByVal a As MutableAttributeSet, ByVal b As Boolean)
			a.addAttribute(Underline, Convert.ToBoolean(b))
		End Sub

		''' <summary>
		''' Sets the strikethrough attribute.
		''' </summary>
		''' <param name="a"> the attribute set </param>
		''' <param name="b"> specifies true/false for setting the attribute </param>
		Public Shared Sub setStrikeThrough(ByVal a As MutableAttributeSet, ByVal b As Boolean)
			a.addAttribute(StrikeThrough, Convert.ToBoolean(b))
		End Sub

		''' <summary>
		''' Sets the superscript attribute.
		''' </summary>
		''' <param name="a"> the attribute set </param>
		''' <param name="b"> specifies true/false for setting the attribute </param>
		Public Shared Sub setSuperscript(ByVal a As MutableAttributeSet, ByVal b As Boolean)
			a.addAttribute(Superscript, Convert.ToBoolean(b))
		End Sub

		''' <summary>
		''' Sets the subscript attribute.
		''' </summary>
		''' <param name="a"> the attribute set </param>
		''' <param name="b"> specifies true/false for setting the attribute </param>
		Public Shared Sub setSubscript(ByVal a As MutableAttributeSet, ByVal b As Boolean)
			a.addAttribute(Subscript, Convert.ToBoolean(b))
		End Sub


		''' <summary>
		''' Gets the foreground color setting from the attribute list.
		''' </summary>
		''' <param name="a"> the attribute set </param>
		''' <returns> the color, Color.black as the default </returns>
		Public Shared Function getForeground(ByVal a As AttributeSet) As java.awt.Color
			Dim fg As java.awt.Color = CType(a.getAttribute(Foreground), java.awt.Color)
			If fg Is Nothing Then fg = java.awt.Color.black
			Return fg
		End Function

		''' <summary>
		''' Sets the foreground color.
		''' </summary>
		''' <param name="a"> the attribute set </param>
		''' <param name="fg"> the color </param>
		Public Shared Sub setForeground(ByVal a As MutableAttributeSet, ByVal fg As java.awt.Color)
			a.addAttribute(Foreground, fg)
		End Sub

		''' <summary>
		''' Gets the background color setting from the attribute list.
		''' </summary>
		''' <param name="a"> the attribute set </param>
		''' <returns> the color, Color.black as the default </returns>
		Public Shared Function getBackground(ByVal a As AttributeSet) As java.awt.Color
			Dim fg As java.awt.Color = CType(a.getAttribute(Background), java.awt.Color)
			If fg Is Nothing Then fg = java.awt.Color.black
			Return fg
		End Function

		''' <summary>
		''' Sets the background color.
		''' </summary>
		''' <param name="a"> the attribute set </param>
		''' <param name="fg"> the color </param>
		Public Shared Sub setBackground(ByVal a As MutableAttributeSet, ByVal fg As java.awt.Color)
			a.addAttribute(Background, fg)
		End Sub


		' --- paragraph attribute accessors ----------------------------

		''' <summary>
		''' Gets the first line indent setting.
		''' </summary>
		''' <param name="a"> the attribute set </param>
		''' <returns> the value, 0 if not set </returns>
		Public Shared Function getFirstLineIndent(ByVal a As AttributeSet) As Single
			Dim indent As Single? = CSng(a.getAttribute(FirstLineIndent))
			If indent IsNot Nothing Then Return indent
			Return 0
		End Function

		''' <summary>
		''' Sets the first line indent.
		''' </summary>
		''' <param name="a"> the attribute set </param>
		''' <param name="i"> the value </param>
		Public Shared Sub setFirstLineIndent(ByVal a As MutableAttributeSet, ByVal i As Single)
			a.addAttribute(FirstLineIndent, New Single?(i))
		End Sub

		''' <summary>
		''' Gets the right indent setting.
		''' </summary>
		''' <param name="a"> the attribute set </param>
		''' <returns> the value, 0 if not set </returns>
		Public Shared Function getRightIndent(ByVal a As AttributeSet) As Single
			Dim indent As Single? = CSng(a.getAttribute(RightIndent))
			If indent IsNot Nothing Then Return indent
			Return 0
		End Function

		''' <summary>
		''' Sets right indent.
		''' </summary>
		''' <param name="a"> the attribute set </param>
		''' <param name="i"> the value </param>
		Public Shared Sub setRightIndent(ByVal a As MutableAttributeSet, ByVal i As Single)
			a.addAttribute(RightIndent, New Single?(i))
		End Sub

		''' <summary>
		''' Gets the left indent setting.
		''' </summary>
		''' <param name="a"> the attribute set </param>
		''' <returns> the value, 0 if not set </returns>
		Public Shared Function getLeftIndent(ByVal a As AttributeSet) As Single
			Dim indent As Single? = CSng(a.getAttribute(LeftIndent))
			If indent IsNot Nothing Then Return indent
			Return 0
		End Function

		''' <summary>
		''' Sets left indent.
		''' </summary>
		''' <param name="a"> the attribute set </param>
		''' <param name="i"> the value </param>
		Public Shared Sub setLeftIndent(ByVal a As MutableAttributeSet, ByVal i As Single)
			a.addAttribute(LeftIndent, New Single?(i))
		End Sub

		''' <summary>
		''' Gets the line spacing setting.
		''' </summary>
		''' <param name="a"> the attribute set </param>
		''' <returns> the value, 0 if not set </returns>
		Public Shared Function getLineSpacing(ByVal a As AttributeSet) As Single
			Dim space As Single? = CSng(a.getAttribute(LineSpacing))
			If space IsNot Nothing Then Return space
			Return 0
		End Function

		''' <summary>
		''' Sets line spacing.
		''' </summary>
		''' <param name="a"> the attribute set </param>
		''' <param name="i"> the value </param>
		Public Shared Sub setLineSpacing(ByVal a As MutableAttributeSet, ByVal i As Single)
			a.addAttribute(LineSpacing, New Single?(i))
		End Sub

		''' <summary>
		''' Gets the space above setting.
		''' </summary>
		''' <param name="a"> the attribute set </param>
		''' <returns> the value, 0 if not set </returns>
		Public Shared Function getSpaceAbove(ByVal a As AttributeSet) As Single
			Dim space As Single? = CSng(a.getAttribute(SpaceAbove))
			If space IsNot Nothing Then Return space
			Return 0
		End Function

		''' <summary>
		''' Sets space above.
		''' </summary>
		''' <param name="a"> the attribute set </param>
		''' <param name="i"> the value </param>
		Public Shared Sub setSpaceAbove(ByVal a As MutableAttributeSet, ByVal i As Single)
			a.addAttribute(SpaceAbove, New Single?(i))
		End Sub

		''' <summary>
		''' Gets the space below setting.
		''' </summary>
		''' <param name="a"> the attribute set </param>
		''' <returns> the value, 0 if not set </returns>
		Public Shared Function getSpaceBelow(ByVal a As AttributeSet) As Single
			Dim space As Single? = CSng(a.getAttribute(SpaceBelow))
			If space IsNot Nothing Then Return space
			Return 0
		End Function

		''' <summary>
		''' Sets space below.
		''' </summary>
		''' <param name="a"> the attribute set </param>
		''' <param name="i"> the value </param>
		Public Shared Sub setSpaceBelow(ByVal a As MutableAttributeSet, ByVal i As Single)
			a.addAttribute(SpaceBelow, New Single?(i))
		End Sub

		''' <summary>
		''' Gets the alignment setting.
		''' </summary>
		''' <param name="a"> the attribute set </param>
		''' <returns> the value <code>StyleConstants.ALIGN_LEFT</code> if not set </returns>
		Public Shared Function getAlignment(ByVal a As AttributeSet) As Integer
			Dim align As Integer? = CInt(Fix(a.getAttribute(Alignment)))
			If align IsNot Nothing Then Return align
			Return ALIGN_LEFT
		End Function

		''' <summary>
		''' Sets alignment.
		''' </summary>
		''' <param name="a"> the attribute set </param>
		''' <param name="align"> the alignment value </param>
		Public Shared Sub setAlignment(ByVal a As MutableAttributeSet, ByVal align As Integer)
			a.addAttribute(Alignment, Convert.ToInt32(align))
		End Sub

		''' <summary>
		''' Gets the TabSet.
		''' </summary>
		''' <param name="a"> the attribute set </param>
		''' <returns> the <code>TabSet</code> </returns>
		Public Shared Function getTabSet(ByVal a As AttributeSet) As TabSet
			Dim tabs As TabSet = CType(a.getAttribute(TabSet), TabSet)
			' PENDING: should this return a default?
			Return tabs
		End Function

		''' <summary>
		''' Sets the TabSet.
		''' </summary>
		''' <param name="a"> the attribute set. </param>
		''' <param name="tabs"> the TabSet </param>
		Public Shared Sub setTabSet(ByVal a As MutableAttributeSet, ByVal tabs As TabSet)
			a.addAttribute(TabSet, tabs)
		End Sub

		' --- privates ---------------------------------------------

		Friend Shared keys As Object() = { NameAttribute, ResolveAttribute, BidiLevel, FontFamily, FontSize, Bold, Italic, Underline, StrikeThrough, Superscript, Subscript, Foreground, Background, ComponentAttribute, IconAttribute, FirstLineIndent, LeftIndent, RightIndent, LineSpacing, SpaceAbove, SpaceBelow, Alignment, TabSet, Orientation, ModelAttribute, ComposedTextAttribute }

		Friend Sub New(ByVal representation As String)
			Me.representation = representation
		End Sub

		Private representation As String

		''' <summary>
		''' This is a typesafe enumeration of the <em>well-known</em>
		''' attributes that contribute to a paragraph style.  These are
		''' aliased by the outer class for general presentation.
		''' </summary>
		Public Class ParagraphConstants
			Inherits StyleConstants
			Implements AttributeSet.ParagraphAttribute

			Private Sub New(ByVal representation As String)
				MyBase.New(representation)
			End Sub
		End Class

		''' <summary>
		''' This is a typesafe enumeration of the <em>well-known</em>
		''' attributes that contribute to a character style.  These are
		''' aliased by the outer class for general presentation.
		''' </summary>
		Public Class CharacterConstants
			Inherits StyleConstants
			Implements AttributeSet.CharacterAttribute

			Private Sub New(ByVal representation As String)
				MyBase.New(representation)
			End Sub
		End Class

		''' <summary>
		''' This is a typesafe enumeration of the <em>well-known</em>
		''' attributes that contribute to a color.  These are aliased
		''' by the outer class for general presentation.
		''' </summary>
		Public Class ColorConstants
			Inherits StyleConstants
			Implements AttributeSet.ColorAttribute, AttributeSet.CharacterAttribute

			Private Sub New(ByVal representation As String)
				MyBase.New(representation)
			End Sub
		End Class

		''' <summary>
		''' This is a typesafe enumeration of the <em>well-known</em>
		''' attributes that contribute to a font.  These are aliased
		''' by the outer class for general presentation.
		''' </summary>
		Public Class FontConstants
			Inherits StyleConstants
			Implements AttributeSet.FontAttribute, AttributeSet.CharacterAttribute

			Private Sub New(ByVal representation As String)
				MyBase.New(representation)
			End Sub
		End Class


	End Class

End Namespace
Imports Microsoft.VisualBasic
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports javax.swing.text

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
Namespace javax.swing.text.html

	''' <summary>
	''' Defines a set of
	''' <a href="http://www.w3.org/TR/REC-CSS1">CSS attributes</a>
	''' as a typesafe enumeration.  The HTML View implementations use
	''' CSS attributes to determine how they will render. This also defines
	''' methods to map between CSS/HTML/StyleConstants. Any shorthand
	''' properties, such as font, are mapped to the intrinsic properties.
	''' <p>The following describes the CSS properties that are supported by the
	''' rendering engine:
	''' <ul><li>font-family
	'''   <li>font-style
	'''   <li>font-size (supports relative units)
	'''   <li>font-weight
	'''   <li>font
	'''   <li>color
	'''   <li>background-color (with the exception of transparent)
	'''   <li>background-image
	'''   <li>background-repeat
	'''   <li>background-position
	'''   <li>background
	'''   <li>text-decoration (with the exception of blink and overline)
	'''   <li>vertical-align (only sup and super)
	'''   <li>text-align (justify is treated as center)
	'''   <li>margin-top
	'''   <li>margin-right
	'''   <li>margin-bottom
	'''   <li>margin-left
	'''   <li>margin
	'''   <li>padding-top
	'''   <li>padding-right
	'''   <li>padding-bottom
	'''   <li>padding-left
	'''   <li>padding
	'''   <li>border-top-style
	'''   <li>border-right-style
	'''   <li>border-bottom-style
	'''   <li>border-left-style
	'''   <li>border-style (only supports inset, outset and none)
	'''   <li>border-top-color
	'''   <li>border-right-color
	'''   <li>border-bottom-color
	'''   <li>border-left-color
	'''   <li>border-color
	'''   <li>list-style-image
	'''   <li>list-style-type
	'''   <li>list-style-position
	''' </ul>
	''' The following are modeled, but currently not rendered.
	''' <ul><li>font-variant
	'''   <li>background-attachment (background always treated as scroll)
	'''   <li>word-spacing
	'''   <li>letter-spacing
	'''   <li>text-indent
	'''   <li>text-transform
	'''   <li>line-height
	'''   <li>border-top-width (this is used to indicate if a border should be used)
	'''   <li>border-right-width
	'''   <li>border-bottom-width
	'''   <li>border-left-width
	'''   <li>border-width
	'''   <li>border-top
	'''   <li>border-right
	'''   <li>border-bottom
	'''   <li>border-left
	'''   <li>border
	'''   <li>width
	'''   <li>height
	'''   <li>float
	'''   <li>clear
	'''   <li>display
	'''   <li>white-space
	'''   <li>list-style
	''' </ul>
	''' <p><b>Note: for the time being we do not fully support relative units,
	''' unless noted, so that
	''' p { margin-top: 10% } will be treated as if no margin-top was specified.</b>
	''' 
	''' @author  Timothy Prinzing
	''' @author  Scott Violet </summary>
	''' <seealso cref= StyleSheet </seealso>
	<Serializable> _
	Public Class CSS

		''' <summary>
		''' Definitions to be used as a key on AttributeSet's
		''' that might hold CSS attributes.  Since this is a
		''' closed set (i.e. defined exactly by the specification),
		''' it is final and cannot be extended.
		''' </summary>
		Public NotInheritable Class Attribute

			Private Sub New(ByVal name As String, ByVal defaultValue As String, ByVal inherited As Boolean)
				Me.name = name
				Me.defaultValue = defaultValue
				Me.inherited = inherited
			End Sub

			''' <summary>
			''' The string representation of the attribute.  This
			''' should exactly match the string specified in the
			''' CSS specification.
			''' </summary>
			Public Overrides Function ToString() As String
				Return name
			End Function

			''' <summary>
			''' Fetch the default value for the attribute.
			''' If there is no default value (such as for
			''' composite attributes), null will be returned.
			''' </summary>
			Public Property defaultValue As String
				Get
					Return defaultValue
				End Get
			End Property

			''' <summary>
			''' Indicates if the attribute should be inherited
			''' from the parent or not.
			''' </summary>
			Public Property inherited As Boolean
				Get
					Return inherited
				End Get
			End Property

			Private name As String
			Private defaultValue As String
			Private inherited As Boolean


			Public Shared ReadOnly BACKGROUND As New Attribute("background", Nothing, False)

			Public Shared ReadOnly BACKGROUND_ATTACHMENT As New Attribute("background-attachment", "scroll", False)

			Public Shared ReadOnly BACKGROUND_COLOR As New Attribute("background-color", "transparent", False)

			Public Shared ReadOnly BACKGROUND_IMAGE As New Attribute("background-image", "none", False)

			Public Shared ReadOnly BACKGROUND_POSITION As New Attribute("background-position", Nothing, False)

			Public Shared ReadOnly BACKGROUND_REPEAT As New Attribute("background-repeat", "repeat", False)

			Public Shared ReadOnly BORDER As New Attribute("border", Nothing, False)

			Public Shared ReadOnly BORDER_BOTTOM As New Attribute("border-bottom", Nothing, False)

			Public Shared ReadOnly BORDER_BOTTOM_COLOR As New Attribute("border-bottom-color", Nothing, False)

			Public Shared ReadOnly BORDER_BOTTOM_STYLE As New Attribute("border-bottom-style", "none", False)

			Public Shared ReadOnly BORDER_BOTTOM_WIDTH As New Attribute("border-bottom-width", "medium", False)

			Public Shared ReadOnly BORDER_COLOR As New Attribute("border-color", Nothing, False)

			Public Shared ReadOnly BORDER_LEFT As New Attribute("border-left", Nothing, False)

			Public Shared ReadOnly BORDER_LEFT_COLOR As New Attribute("border-left-color", Nothing, False)

			Public Shared ReadOnly BORDER_LEFT_STYLE As New Attribute("border-left-style", "none", False)

			Public Shared ReadOnly BORDER_LEFT_WIDTH As New Attribute("border-left-width", "medium", False)

			Public Shared ReadOnly BORDER_RIGHT As New Attribute("border-right", Nothing, False)

			Public Shared ReadOnly BORDER_RIGHT_COLOR As New Attribute("border-right-color", Nothing, False)

			Public Shared ReadOnly BORDER_RIGHT_STYLE As New Attribute("border-right-style", "none", False)

			Public Shared ReadOnly BORDER_RIGHT_WIDTH As New Attribute("border-right-width", "medium", False)

			Public Shared ReadOnly BORDER_STYLE As New Attribute("border-style", "none", False)

			Public Shared ReadOnly BORDER_TOP As New Attribute("border-top", Nothing, False)

			Public Shared ReadOnly BORDER_TOP_COLOR As New Attribute("border-top-color", Nothing, False)

			Public Shared ReadOnly BORDER_TOP_STYLE As New Attribute("border-top-style", "none", False)

			Public Shared ReadOnly BORDER_TOP_WIDTH As New Attribute("border-top-width", "medium", False)

			Public Shared ReadOnly BORDER_WIDTH As New Attribute("border-width", "medium", False)

			Public Shared ReadOnly CLEAR As New Attribute("clear", "none", False)

			Public Shared ReadOnly COLOR As New Attribute("color", "black", True)

			Public Shared ReadOnly DISPLAY As New Attribute("display", "block", False)

			Public Shared ReadOnly FLOAT As New Attribute("float", "none", False)

			Public Shared ReadOnly FONT As New Attribute("font", Nothing, True)

			Public Shared ReadOnly FONT_FAMILY As New Attribute("font-family", Nothing, True)

			Public Shared ReadOnly FONT_SIZE As New Attribute("font-size", "medium", True)

			Public Shared ReadOnly FONT_STYLE As New Attribute("font-style", "normal", True)

			Public Shared ReadOnly FONT_VARIANT As New Attribute("font-variant", "normal", True)

			Public Shared ReadOnly FONT_WEIGHT As New Attribute("font-weight", "normal", True)

			Public Shared ReadOnly HEIGHT As New Attribute("height", "auto", False)

			Public Shared ReadOnly LETTER_SPACING As New Attribute("letter-spacing", "normal", True)

			Public Shared ReadOnly LINE_HEIGHT As New Attribute("line-height", "normal", True)

			Public Shared ReadOnly LIST_STYLE As New Attribute("list-style", Nothing, True)

			Public Shared ReadOnly LIST_STYLE_IMAGE As New Attribute("list-style-image", "none", True)

			Public Shared ReadOnly LIST_STYLE_POSITION As New Attribute("list-style-position", "outside", True)

			Public Shared ReadOnly LIST_STYLE_TYPE As New Attribute("list-style-type", "disc", True)

			Public Shared ReadOnly MARGIN As New Attribute("margin", Nothing, False)

			Public Shared ReadOnly MARGIN_BOTTOM As New Attribute("margin-bottom", "0", False)

			Public Shared ReadOnly MARGIN_LEFT As New Attribute("margin-left", "0", False)

			Public Shared ReadOnly MARGIN_RIGHT As New Attribute("margin-right", "0", False)

	'        
	'         * made up css attributes to describe orientation depended
	'         * margins. used for <dir>, <menu>, <ul> etc. see
	'         * 5088268 for more details
	'         
			Friend Shared ReadOnly MARGIN_LEFT_LTR As New Attribute("margin-left-ltr", Integer.ToString(Integer.MIN_VALUE), False)

			Friend Shared ReadOnly MARGIN_LEFT_RTL As New Attribute("margin-left-rtl", Integer.ToString(Integer.MIN_VALUE), False)

			Friend Shared ReadOnly MARGIN_RIGHT_LTR As New Attribute("margin-right-ltr", Integer.ToString(Integer.MIN_VALUE), False)

			Friend Shared ReadOnly MARGIN_RIGHT_RTL As New Attribute("margin-right-rtl", Integer.ToString(Integer.MIN_VALUE), False)


			Public Shared ReadOnly MARGIN_TOP As New Attribute("margin-top", "0", False)

			Public Shared ReadOnly PADDING As New Attribute("padding", Nothing, False)

			Public Shared ReadOnly PADDING_BOTTOM As New Attribute("padding-bottom", "0", False)

			Public Shared ReadOnly PADDING_LEFT As New Attribute("padding-left", "0", False)

			Public Shared ReadOnly PADDING_RIGHT As New Attribute("padding-right", "0", False)

			Public Shared ReadOnly PADDING_TOP As New Attribute("padding-top", "0", False)

			Public Shared ReadOnly TEXT_ALIGN As New Attribute("text-align", Nothing, True)

			Public Shared ReadOnly TEXT_DECORATION As New Attribute("text-decoration", "none", True)

			Public Shared ReadOnly TEXT_INDENT As New Attribute("text-indent", "0", True)

			Public Shared ReadOnly TEXT_TRANSFORM As New Attribute("text-transform", "none", True)

			Public Shared ReadOnly VERTICAL_ALIGN As New Attribute("vertical-align", "baseline", False)

			Public Shared ReadOnly WORD_SPACING As New Attribute("word-spacing", "normal", True)

			Public Shared ReadOnly WHITE_SPACE As New Attribute("white-space", "normal", True)

			Public Shared ReadOnly WIDTH As New Attribute("width", "auto", False)

			'public
	 Friend Shared ReadOnly BORDER_SPACING As New Attribute("border-spacing", "0", True)

			'public
	 Friend Shared ReadOnly CAPTION_SIDE As New Attribute("caption-side", "left", True)

			' All possible CSS attribute keys.
			Friend Shared ReadOnly allAttributes As Attribute() = { BACKGROUND, BACKGROUND_ATTACHMENT, BACKGROUND_COLOR, BACKGROUND_IMAGE, BACKGROUND_POSITION, BACKGROUND_REPEAT, BORDER, BORDER_BOTTOM, BORDER_BOTTOM_WIDTH, BORDER_COLOR, BORDER_LEFT, BORDER_LEFT_WIDTH, BORDER_RIGHT, BORDER_RIGHT_WIDTH, BORDER_STYLE, BORDER_TOP, BORDER_TOP_WIDTH, BORDER_WIDTH, BORDER_TOP_STYLE, BORDER_RIGHT_STYLE, BORDER_BOTTOM_STYLE, BORDER_LEFT_STYLE, BORDER_TOP_COLOR, BORDER_RIGHT_COLOR, BORDER_BOTTOM_COLOR, BORDER_LEFT_COLOR, CLEAR, COLOR, DISPLAY, FLOAT, FONT, FONT_FAMILY, FONT_SIZE, FONT_STYLE, FONT_VARIANT, FONT_WEIGHT, HEIGHT, LETTER_SPACING, LINE_HEIGHT, LIST_STYLE, LIST_STYLE_IMAGE, LIST_STYLE_POSITION, LIST_STYLE_TYPE, MARGIN, MARGIN_BOTTOM, MARGIN_LEFT, MARGIN_RIGHT, MARGIN_TOP, PADDING, PADDING_BOTTOM, PADDING_LEFT, PADDING_RIGHT, PADDING_TOP, TEXT_ALIGN, TEXT_DECORATION, TEXT_INDENT, TEXT_TRANSFORM, VERTICAL_ALIGN, WORD_SPACING, WHITE_SPACE, WIDTH, BORDER_SPACING, CAPTION_SIDE, MARGIN_LEFT_LTR, MARGIN_LEFT_RTL, MARGIN_RIGHT_LTR, MARGIN_RIGHT_RTL }

			Private Shared ReadOnly ALL_MARGINS As Attribute() = { MARGIN_TOP, MARGIN_RIGHT, MARGIN_BOTTOM, MARGIN_LEFT }
			Private Shared ReadOnly ALL_PADDING As Attribute() = { PADDING_TOP, PADDING_RIGHT, PADDING_BOTTOM, PADDING_LEFT }
			Private Shared ReadOnly ALL_BORDER_WIDTHS As Attribute() = { BORDER_TOP_WIDTH, BORDER_RIGHT_WIDTH, BORDER_BOTTOM_WIDTH, BORDER_LEFT_WIDTH }
			Private Shared ReadOnly ALL_BORDER_STYLES As Attribute() = { BORDER_TOP_STYLE, BORDER_RIGHT_STYLE, BORDER_BOTTOM_STYLE, BORDER_LEFT_STYLE }
			Private Shared ReadOnly ALL_BORDER_COLORS As Attribute() = { BORDER_TOP_COLOR, BORDER_RIGHT_COLOR, BORDER_BOTTOM_COLOR, BORDER_LEFT_COLOR }

		End Class

		Friend NotInheritable Class Value

			Private Sub New(ByVal name As String)
				Me.name = name
			End Sub

			''' <summary>
			''' The string representation of the attribute.  This
			''' should exactly match the string specified in the
			''' CSS specification.
			''' </summary>
			Public Overrides Function ToString() As String
				Return name
			End Function

			Friend Shared ReadOnly INHERITED As New Value("inherited")
			Friend Shared ReadOnly NONE As New Value("none")
			Friend Shared ReadOnly HIDDEN As New Value("hidden")
			Friend Shared ReadOnly DOTTED As New Value("dotted")
			Friend Shared ReadOnly DASHED As New Value("dashed")
			Friend Shared ReadOnly SOLID As New Value("solid")
			Friend Shared ReadOnly [DOUBLE] As New Value("double")
			Friend Shared ReadOnly GROOVE As New Value("groove")
			Friend Shared ReadOnly RIDGE As New Value("ridge")
			Friend Shared ReadOnly INSET As New Value("inset")
			Friend Shared ReadOnly OUTSET As New Value("outset")
			' Lists.
			Friend Shared ReadOnly DISC As New Value("disc")
			Friend Shared ReadOnly CIRCLE As New Value("circle")
			Friend Shared ReadOnly SQUARE As New Value("square")
			Friend Shared ReadOnly [DECIMAL] As New Value("decimal")
			Friend Shared ReadOnly LOWER_ROMAN As New Value("lower-roman")
			Friend Shared ReadOnly UPPER_ROMAN As New Value("upper-roman")
			Friend Shared ReadOnly LOWER_ALPHA As New Value("lower-alpha")
			Friend Shared ReadOnly UPPER_ALPHA As New Value("upper-alpha")
			' background-repeat
			Friend Shared ReadOnly BACKGROUND_NO_REPEAT As New Value("no-repeat")
			Friend Shared ReadOnly BACKGROUND_REPEAT As New Value("repeat")
			Friend Shared ReadOnly BACKGROUND_REPEAT_X As New Value("repeat-x")
			Friend Shared ReadOnly BACKGROUND_REPEAT_Y As New Value("repeat-y")
			' background-attachment
			Friend Shared ReadOnly BACKGROUND_SCROLL As New Value("scroll")
			Friend Shared ReadOnly BACKGROUND_FIXED As New Value("fixed")

			Private name As String

			Friend Shared ReadOnly allValues As Value() = { INHERITED, NONE, DOTTED, DASHED, SOLID, [DOUBLE], GROOVE, RIDGE, INSET, OUTSET, DISC, CIRCLE, SQUARE, [DECIMAL], LOWER_ROMAN, UPPER_ROMAN, LOWER_ALPHA, UPPER_ALPHA, BACKGROUND_NO_REPEAT, BACKGROUND_REPEAT, BACKGROUND_REPEAT_X, BACKGROUND_REPEAT_Y, BACKGROUND_FIXED, BACKGROUND_FIXED }
		End Class

		Public Sub New()
			baseFontSize = baseFontSizeIndex + 1
			' setup the css conversion table
			valueConvertor = New Dictionary(Of Object, Object)
			valueConvertor(CSS.Attribute.FONT_SIZE) = New FontSize(Me)
			valueConvertor(CSS.Attribute.FONT_FAMILY) = New FontFamily
			valueConvertor(CSS.Attribute.FONT_WEIGHT) = New FontWeight
			Dim bs As Object = New BorderStyle
			valueConvertor(CSS.Attribute.BORDER_TOP_STYLE) = bs
			valueConvertor(CSS.Attribute.BORDER_RIGHT_STYLE) = bs
			valueConvertor(CSS.Attribute.BORDER_BOTTOM_STYLE) = bs
			valueConvertor(CSS.Attribute.BORDER_LEFT_STYLE) = bs
			Dim cv As Object = New ColorValue
			valueConvertor(CSS.Attribute.COLOR) = cv
			valueConvertor(CSS.Attribute.BACKGROUND_COLOR) = cv
			valueConvertor(CSS.Attribute.BORDER_TOP_COLOR) = cv
			valueConvertor(CSS.Attribute.BORDER_RIGHT_COLOR) = cv
			valueConvertor(CSS.Attribute.BORDER_BOTTOM_COLOR) = cv
			valueConvertor(CSS.Attribute.BORDER_LEFT_COLOR) = cv
			Dim lv As Object = New LengthValue
			valueConvertor(CSS.Attribute.MARGIN_TOP) = lv
			valueConvertor(CSS.Attribute.MARGIN_BOTTOM) = lv
			valueConvertor(CSS.Attribute.MARGIN_LEFT) = lv
			valueConvertor(CSS.Attribute.MARGIN_LEFT_LTR) = lv
			valueConvertor(CSS.Attribute.MARGIN_LEFT_RTL) = lv
			valueConvertor(CSS.Attribute.MARGIN_RIGHT) = lv
			valueConvertor(CSS.Attribute.MARGIN_RIGHT_LTR) = lv
			valueConvertor(CSS.Attribute.MARGIN_RIGHT_RTL) = lv
			valueConvertor(CSS.Attribute.PADDING_TOP) = lv
			valueConvertor(CSS.Attribute.PADDING_BOTTOM) = lv
			valueConvertor(CSS.Attribute.PADDING_LEFT) = lv
			valueConvertor(CSS.Attribute.PADDING_RIGHT) = lv
			Dim bv As Object = New BorderWidthValue(Nothing, 0)
			valueConvertor(CSS.Attribute.BORDER_TOP_WIDTH) = bv
			valueConvertor(CSS.Attribute.BORDER_BOTTOM_WIDTH) = bv
			valueConvertor(CSS.Attribute.BORDER_LEFT_WIDTH) = bv
			valueConvertor(CSS.Attribute.BORDER_RIGHT_WIDTH) = bv
			Dim nlv As Object = New LengthValue(True)
			valueConvertor(CSS.Attribute.TEXT_INDENT) = nlv
			valueConvertor(CSS.Attribute.WIDTH) = lv
			valueConvertor(CSS.Attribute.HEIGHT) = lv
			valueConvertor(CSS.Attribute.BORDER_SPACING) = lv
			Dim sv As Object = New StringValue
			valueConvertor(CSS.Attribute.FONT_STYLE) = sv
			valueConvertor(CSS.Attribute.TEXT_DECORATION) = sv
			valueConvertor(CSS.Attribute.TEXT_ALIGN) = sv
			valueConvertor(CSS.Attribute.VERTICAL_ALIGN) = sv
			Dim valueMapper As Object = New CssValueMapper
			valueConvertor(CSS.Attribute.LIST_STYLE_TYPE) = valueMapper
			valueConvertor(CSS.Attribute.BACKGROUND_IMAGE) = New BackgroundImage
			valueConvertor(CSS.Attribute.BACKGROUND_POSITION) = New BackgroundPosition
			valueConvertor(CSS.Attribute.BACKGROUND_REPEAT) = valueMapper
			valueConvertor(CSS.Attribute.BACKGROUND_ATTACHMENT) = valueMapper
			Dim generic As Object = New CssValue
			Dim n As Integer = CSS.Attribute.allAttributes.Length
			For i As Integer = 0 To n - 1
				Dim key As CSS.Attribute = CSS.Attribute.allAttributes(i)
				If valueConvertor(key) Is Nothing Then valueConvertor(key) = generic
			Next i
		End Sub

		''' <summary>
		''' Sets the base font size. <code>sz</code> is a CSS value, and is
		''' not necessarily the point size. Use getPointSize to determine the
		''' point size corresponding to <code>sz</code>.
		''' </summary>
		Friend Overridable Property baseFontSize As Integer
			Set(ByVal sz As Integer)
				If sz < 1 Then
				  baseFontSize = 0
				ElseIf sz > 7 Then
				  baseFontSize = 7
				Else
				  baseFontSize = sz
				End If
			End Set
			Get
				Return baseFontSize
			End Get
		End Property

		''' <summary>
		''' Sets the base font size from the passed in string.
		''' </summary>
		Friend Overridable Property baseFontSize As String
			Set(ByVal size As String)
				Dim relSize, absSize, diff As Integer
    
				If size IsNot Nothing Then
					If size.StartsWith("+") Then
						relSize = Convert.ToInt32(size.Substring(1))
						baseFontSize = baseFontSize + relSize
					ElseIf size.StartsWith("-") Then
						relSize = -Convert.ToInt32(size.Substring(1))
						baseFontSize = baseFontSize + relSize
					Else
						baseFontSize = Convert.ToInt32(size)
					End If
				End If
			End Set
		End Property


		''' <summary>
		''' Parses the CSS property <code>key</code> with value
		''' <code>value</code> placing the result in <code>att</code>.
		''' </summary>
		Friend Overridable Sub addInternalCSSValue(ByVal attr As MutableAttributeSet, ByVal key As CSS.Attribute, ByVal ___value As String)
			If key Is CSS.Attribute.FONT Then
				ShorthandFontParser.parseShorthandFont(Me, ___value, attr)
			ElseIf key Is CSS.Attribute.BACKGROUND Then
				ShorthandBackgroundParser.parseShorthandBackground(Me, ___value, attr)
			ElseIf key Is CSS.Attribute.MARGIN Then
				ShorthandMarginParser.parseShorthandMargin(Me, ___value, attr, CSS.Attribute.ALL_MARGINS)
			ElseIf key Is CSS.Attribute.PADDING Then
				ShorthandMarginParser.parseShorthandMargin(Me, ___value, attr, CSS.Attribute.ALL_PADDING)
			ElseIf key Is CSS.Attribute.BORDER_WIDTH Then
				ShorthandMarginParser.parseShorthandMargin(Me, ___value, attr, CSS.Attribute.ALL_BORDER_WIDTHS)
			ElseIf key Is CSS.Attribute.BORDER_COLOR Then
				ShorthandMarginParser.parseShorthandMargin(Me, ___value, attr, CSS.Attribute.ALL_BORDER_COLORS)
			ElseIf key Is CSS.Attribute.BORDER_STYLE Then
				ShorthandMarginParser.parseShorthandMargin(Me, ___value, attr, CSS.Attribute.ALL_BORDER_STYLES)
			ElseIf (key Is CSS.Attribute.BORDER) OrElse (key Is CSS.Attribute.BORDER_TOP) OrElse (key Is CSS.Attribute.BORDER_RIGHT) OrElse (key Is CSS.Attribute.BORDER_BOTTOM) OrElse (key Is CSS.Attribute.BORDER_LEFT) Then
				ShorthandBorderParser.parseShorthandBorder(attr, key, ___value)
			Else
				Dim iValue As Object = getInternalCSSValue(key, ___value)
				If iValue IsNot Nothing Then attr.addAttribute(key, iValue)
			End If
		End Sub

		''' <summary>
		''' Gets the internal CSS representation of <code>value</code> which is
		''' a CSS value of the CSS attribute named <code>key</code>. The receiver
		''' should not modify <code>value</code>, and the first <code>count</code>
		''' strings are valid.
		''' </summary>
		Friend Overridable Function getInternalCSSValue(ByVal key As CSS.Attribute, ByVal ___value As String) As Object
			Dim conv As CssValue = CType(valueConvertor(key), CssValue)
			Dim r As Object = conv.parseCssValue(___value)
			Return If(r IsNot Nothing, r, conv.parseCssValue(key.defaultValue))
		End Function

		''' <summary>
		''' Maps from a StyleConstants to a CSS Attribute.
		''' </summary>
		Friend Overridable Function styleConstantsKeyToCSSKey(ByVal sc As StyleConstants) As Attribute
			Return styleConstantToCssMap(sc)
		End Function

		''' <summary>
		''' Maps from a StyleConstants value to a CSS value.
		''' </summary>
		Friend Overridable Function styleConstantsValueToCSSValue(ByVal sc As StyleConstants, ByVal styleValue As Object) As Object
			Dim cssKey As Attribute = styleConstantsKeyToCSSKey(sc)
			If cssKey IsNot Nothing Then
				Dim conv As CssValue = CType(valueConvertor(cssKey), CssValue)
				Return conv.fromStyleConstants(sc, styleValue)
			End If
			Return Nothing
		End Function

		''' <summary>
		''' Converts the passed in CSS value to a StyleConstants value.
		''' <code>key</code> identifies the CSS attribute being mapped.
		''' </summary>
		Friend Overridable Function cssValueToStyleConstantsValue(ByVal key As StyleConstants, ByVal ___value As Object) As Object
			If TypeOf ___value Is CssValue Then Return CType(___value, CssValue).toStyleConstants(key, Nothing)
			Return Nothing
		End Function

		''' <summary>
		''' Returns the font for the values in the passed in AttributeSet.
		''' It is assumed the keys will be CSS.Attribute keys.
		''' <code>sc</code> is the StyleContext that will be messaged to get
		''' the font once the size, name and style have been determined.
		''' </summary>
		Friend Overridable Function getFont(ByVal sc As StyleContext, ByVal a As AttributeSet, ByVal defaultSize As Integer, ByVal ss As StyleSheet) As java.awt.Font
			ss = getStyleSheet(ss)
			Dim size As Integer = getFontSize(a, defaultSize, ss)

	'        
	'         * If the vertical alignment is set to either superscirpt or
	'         * subscript we reduce the font size by 2 points.
	'         
			Dim vAlignV As StringValue = CType(a.getAttribute(CSS.Attribute.VERTICAL_ALIGN), StringValue)
			If (vAlignV IsNot Nothing) Then
				Dim vAlign As String = vAlignV.ToString()
				If (vAlign.IndexOf("sup") >= 0) OrElse (vAlign.IndexOf("sub") >= 0) Then size -= 2
			End If

			Dim familyValue As FontFamily = CType(a.getAttribute(CSS.Attribute.FONT_FAMILY), FontFamily)
			Dim family As String = If(familyValue IsNot Nothing, familyValue.value, java.awt.Font.SANS_SERIF)
			Dim style As Integer = java.awt.Font.PLAIN
			Dim weightValue As FontWeight = CType(a.getAttribute(CSS.Attribute.FONT_WEIGHT), FontWeight)
			If (weightValue IsNot Nothing) AndAlso (weightValue.value > 400) Then style = style Or java.awt.Font.BOLD
			Dim fs As Object = a.getAttribute(CSS.Attribute.FONT_STYLE)
			If (fs IsNot Nothing) AndAlso (fs.ToString().IndexOf("italic") >= 0) Then style = style Or java.awt.Font.ITALIC
			If family.ToUpper() = "monospace".ToUpper() Then family = java.awt.Font.MONOSPACED
			Dim f As java.awt.Font = sc.getFont(family, style, size)
			If f Is Nothing OrElse (f.family.Equals(java.awt.Font.DIALOG) AndAlso (Not family.ToUpper()) = java.awt.Font.DIALOG.ToUpper()) Then
				family = java.awt.Font.SANS_SERIF
				f = sc.getFont(family, style, size)
			End If
			Return f
		End Function

		Friend Shared Function getFontSize(ByVal attr As AttributeSet, ByVal defaultSize As Integer, ByVal ss As StyleSheet) As Integer
			' PENDING(prinz) this is a 1.1 based implementation, need to also
			' have a 1.2 version.
			Dim sizeValue As FontSize = CType(attr.getAttribute(CSS.Attribute.FONT_SIZE), FontSize)

			Return If(sizeValue IsNot Nothing, sizeValue.getValue(attr, ss), defaultSize)
		End Function

		''' <summary>
		''' Takes a set of attributes and turn it into a color
		''' specification.  This might be used to specify things
		''' like brighter, more hue, etc.
		''' This will return null if there is no value for <code>key</code>.
		''' </summary>
		''' <param name="key"> CSS.Attribute identifying where color is stored. </param>
		''' <param name="a"> the set of attributes </param>
		''' <returns> the color </returns>
		Friend Overridable Function getColor(ByVal a As AttributeSet, ByVal key As CSS.Attribute) As java.awt.Color
			Dim cv As ColorValue = CType(a.getAttribute(key), ColorValue)
			If cv IsNot Nothing Then Return cv.value
			Return Nothing
		End Function

		''' <summary>
		''' Returns the size of a font from the passed in string.
		''' </summary>
		''' <param name="size"> CSS string describing font size </param>
		''' <param name="baseFontSize"> size to use for relative units. </param>
		Friend Overridable Function getPointSize(ByVal size As String, ByVal ss As StyleSheet) As Single
			Dim relSize, absSize, diff, index As Integer
			ss = getStyleSheet(ss)
			If size IsNot Nothing Then
				If size.StartsWith("+") Then
					relSize = Convert.ToInt32(size.Substring(1))
					Return getPointSize(baseFontSize + relSize, ss)
				ElseIf size.StartsWith("-") Then
					relSize = -Convert.ToInt32(size.Substring(1))
					Return getPointSize(baseFontSize + relSize, ss)
				Else
					absSize = Convert.ToInt32(size)
					Return getPointSize(absSize, ss)
				End If
			End If
			Return 0
		End Function

		''' <summary>
		''' Returns the length of the attribute in <code>a</code> with
		''' key <code>key</code>.
		''' </summary>
		Friend Overridable Function getLength(ByVal a As AttributeSet, ByVal key As CSS.Attribute, ByVal ss As StyleSheet) As Single
			ss = getStyleSheet(ss)
			Dim lv As LengthValue = CType(a.getAttribute(key), LengthValue)
			Dim isW3CLengthUnits As Boolean = If(ss Is Nothing, False, ss.w3CLengthUnits)
			Dim len As Single = If(lv IsNot Nothing, lv.getValue(isW3CLengthUnits), 0)
			Return len
		End Function

		''' <summary>
		''' Convert a set of HTML attributes to an equivalent
		''' set of CSS attributes.
		''' </summary>
		''' <param name="htmlAttrSet"> AttributeSet containing the HTML attributes. </param>
		''' <returns> AttributeSet containing the corresponding CSS attributes.
		'''        The AttributeSet will be empty if there are no mapping
		'''        CSS attributes. </returns>
		Friend Overridable Function translateHTMLToCSS(ByVal htmlAttrSet As AttributeSet) As AttributeSet
			Dim cssAttrSet As MutableAttributeSet = New SimpleAttributeSet
			Dim elem As Element = CType(htmlAttrSet, Element)
			Dim tag As HTML.Tag = getHTMLTag(htmlAttrSet)
			If (tag Is HTML.Tag.TD) OrElse (tag Is HTML.Tag.TH) Then
				' translate border width into the cells, if it has non-zero value.
				Dim tableAttr As AttributeSet = elem.parentElement.parentElement.attributes

				Dim borderWidth As Integer = getTableBorder(tableAttr)
				If borderWidth > 0 Then translateAttribute(HTML.Attribute.BORDER, "1", cssAttrSet)
				Dim pad As String = CStr(tableAttr.getAttribute(HTML.Attribute.CELLPADDING))
				If pad IsNot Nothing Then
					Dim v As LengthValue = CType(getInternalCSSValue(CSS.Attribute.PADDING_TOP, pad), LengthValue)
					v.span = If(v.span < 0, 0, v.span)
					cssAttrSet.addAttribute(CSS.Attribute.PADDING_TOP, v)
					cssAttrSet.addAttribute(CSS.Attribute.PADDING_BOTTOM, v)
					cssAttrSet.addAttribute(CSS.Attribute.PADDING_LEFT, v)
					cssAttrSet.addAttribute(CSS.Attribute.PADDING_RIGHT, v)
				End If
			End If
			If elem.leaf Then
				translateEmbeddedAttributes(htmlAttrSet, cssAttrSet)
			Else
				translateAttributes(tag, htmlAttrSet, cssAttrSet)
			End If
			If tag Is HTML.Tag.CAPTION Then
	'            
	'             * Navigator uses ALIGN for caption placement and IE uses VALIGN.
	'             
				Dim v As Object = htmlAttrSet.getAttribute(HTML.Attribute.ALIGN)
				If (v IsNot Nothing) AndAlso (v.Equals("top") OrElse v.Equals("bottom")) Then
					cssAttrSet.addAttribute(CSS.Attribute.CAPTION_SIDE, v)
					cssAttrSet.removeAttribute(CSS.Attribute.TEXT_ALIGN)
				Else
					v = htmlAttrSet.getAttribute(HTML.Attribute.VALIGN)
					If v IsNot Nothing Then cssAttrSet.addAttribute(CSS.Attribute.CAPTION_SIDE, v)
				End If
			End If
			Return cssAttrSet
		End Function

		Private Shared Function getTableBorder(ByVal tableAttr As AttributeSet) As Integer
			Dim borderValue As String = CStr(tableAttr.getAttribute(HTML.Attribute.BORDER))

			If borderValue = HTML.NULL_ATTRIBUTE_VALUE OrElse "".Equals(borderValue) Then Return 1

			Try
				Return Convert.ToInt32(borderValue)
			Catch e As NumberFormatException
				Return 0
			End Try
		End Function

		Private Shared ReadOnly attributeMap As New Dictionary(Of String, Attribute)
		Private Shared ReadOnly valueMap As New Dictionary(Of String, Value)

		''' <summary>
		''' The hashtable and the static initalization block below,
		''' set up a mapping from well-known HTML attributes to
		''' CSS attributes.  For the most part, there is a 1-1 mapping
		''' between the two.  However in the case of certain HTML
		''' attributes for example HTML.Attribute.VSPACE or
		''' HTML.Attribute.HSPACE, end up mapping to two CSS.Attribute's.
		''' Therefore, the value associated with each HTML.Attribute.
		''' key ends up being an array of CSS.Attribute.* objects.
		''' </summary>
		Private Shared ReadOnly htmlAttrToCssAttrMap As New Dictionary(Of HTML.Attribute, CSS.Attribute())(20)

		''' <summary>
		''' The hashtable and static initialization that follows sets
		''' up a translation from StyleConstants (i.e. the <em>well known</em>
		''' attributes) to the associated CSS attributes.
		''' </summary>
		Private Shared ReadOnly styleConstantToCssMap As New Dictionary(Of Object, Attribute)(17)
		''' <summary>
		''' Maps from HTML value to a CSS value. Used in internal mapping. </summary>
		Private Shared ReadOnly htmlValueToCssValueMap As New Dictionary(Of String, CSS.Value)(8)
		''' <summary>
		''' Maps from CSS value (string) to internal value. </summary>
		Private Shared ReadOnly cssValueToInternalValueMap As New Dictionary(Of String, CSS.Value)(13)

		Shared Sub New()
			' load the attribute map
			For i As Integer = 0 To Attribute.allAttributes.Length - 1
				attributeMap(Attribute.allAttributes(i).ToString()) = Attribute.allAttributes(i)
			Next i
			' load the value map
			For i As Integer = 0 To Value.allValues.Length - 1
				valueMap(Value.allValues(i).ToString()) = Value.allValues(i)
			Next i

			htmlAttrToCssAttrMap(HTML.Attribute.COLOR) = New CSS.Attribute(){CSS.Attribute.COLOR}
			htmlAttrToCssAttrMap(HTML.Attribute.TEXT) = New CSS.Attribute(){CSS.Attribute.COLOR}
			htmlAttrToCssAttrMap(HTML.Attribute.CLEAR) = New CSS.Attribute(){CSS.Attribute.CLEAR}
			htmlAttrToCssAttrMap(HTML.Attribute.BACKGROUND) = New CSS.Attribute(){CSS.Attribute.BACKGROUND_IMAGE}
			htmlAttrToCssAttrMap(HTML.Attribute.BGCOLOR) = New CSS.Attribute(){CSS.Attribute.BACKGROUND_COLOR}
			htmlAttrToCssAttrMap(HTML.Attribute.WIDTH) = New CSS.Attribute(){CSS.Attribute.WIDTH}
			htmlAttrToCssAttrMap(HTML.Attribute.HEIGHT) = New CSS.Attribute(){CSS.Attribute.HEIGHT}
			htmlAttrToCssAttrMap(HTML.Attribute.BORDER) = New CSS.Attribute(){CSS.Attribute.BORDER_TOP_WIDTH, CSS.Attribute.BORDER_RIGHT_WIDTH, CSS.Attribute.BORDER_BOTTOM_WIDTH, CSS.Attribute.BORDER_LEFT_WIDTH}
			htmlAttrToCssAttrMap(HTML.Attribute.CELLPADDING) = New CSS.Attribute(){CSS.Attribute.PADDING}
			htmlAttrToCssAttrMap(HTML.Attribute.CELLSPACING) = New CSS.Attribute(){CSS.Attribute.BORDER_SPACING}
			htmlAttrToCssAttrMap(HTML.Attribute.MARGINWIDTH) = New CSS.Attribute(){CSS.Attribute.MARGIN_LEFT, CSS.Attribute.MARGIN_RIGHT}
			htmlAttrToCssAttrMap(HTML.Attribute.MARGINHEIGHT) = New CSS.Attribute(){CSS.Attribute.MARGIN_TOP, CSS.Attribute.MARGIN_BOTTOM}
			htmlAttrToCssAttrMap(HTML.Attribute.HSPACE) = New CSS.Attribute(){CSS.Attribute.PADDING_LEFT, CSS.Attribute.PADDING_RIGHT}
			htmlAttrToCssAttrMap(HTML.Attribute.VSPACE) = New CSS.Attribute(){CSS.Attribute.PADDING_BOTTOM, CSS.Attribute.PADDING_TOP}
			htmlAttrToCssAttrMap(HTML.Attribute.FACE) = New CSS.Attribute(){CSS.Attribute.FONT_FAMILY}
			htmlAttrToCssAttrMap(HTML.Attribute.SIZE) = New CSS.Attribute(){CSS.Attribute.FONT_SIZE}
			htmlAttrToCssAttrMap(HTML.Attribute.VALIGN) = New CSS.Attribute(){CSS.Attribute.VERTICAL_ALIGN}
			htmlAttrToCssAttrMap(HTML.Attribute.ALIGN) = New CSS.Attribute(){CSS.Attribute.VERTICAL_ALIGN, CSS.Attribute.TEXT_ALIGN, CSS.Attribute.FLOAT}
			htmlAttrToCssAttrMap(HTML.Attribute.TYPE) = New CSS.Attribute(){CSS.Attribute.LIST_STYLE_TYPE}
			htmlAttrToCssAttrMap(HTML.Attribute.NOWRAP) = New CSS.Attribute(){CSS.Attribute.WHITE_SPACE}

			' initialize StyleConstants mapping
			styleConstantToCssMap(StyleConstants.FontFamily) = CSS.Attribute.FONT_FAMILY
			styleConstantToCssMap(StyleConstants.FontSize) = CSS.Attribute.FONT_SIZE
			styleConstantToCssMap(StyleConstants.Bold) = CSS.Attribute.FONT_WEIGHT
			styleConstantToCssMap(StyleConstants.Italic) = CSS.Attribute.FONT_STYLE
			styleConstantToCssMap(StyleConstants.Underline) = CSS.Attribute.TEXT_DECORATION
			styleConstantToCssMap(StyleConstants.StrikeThrough) = CSS.Attribute.TEXT_DECORATION
			styleConstantToCssMap(StyleConstants.Superscript) = CSS.Attribute.VERTICAL_ALIGN
			styleConstantToCssMap(StyleConstants.Subscript) = CSS.Attribute.VERTICAL_ALIGN
			styleConstantToCssMap(StyleConstants.Foreground) = CSS.Attribute.COLOR
			styleConstantToCssMap(StyleConstants.Background) = CSS.Attribute.BACKGROUND_COLOR
			styleConstantToCssMap(StyleConstants.FirstLineIndent) = CSS.Attribute.TEXT_INDENT
			styleConstantToCssMap(StyleConstants.LeftIndent) = CSS.Attribute.MARGIN_LEFT
			styleConstantToCssMap(StyleConstants.RightIndent) = CSS.Attribute.MARGIN_RIGHT
			styleConstantToCssMap(StyleConstants.SpaceAbove) = CSS.Attribute.MARGIN_TOP
			styleConstantToCssMap(StyleConstants.SpaceBelow) = CSS.Attribute.MARGIN_BOTTOM
			styleConstantToCssMap(StyleConstants.Alignment) = CSS.Attribute.TEXT_ALIGN

			' HTML->CSS
			htmlValueToCssValueMap("disc") = CSS.Value.DISC
			htmlValueToCssValueMap("square") = CSS.Value.SQUARE
			htmlValueToCssValueMap("circle") = CSS.Value.CIRCLE
			htmlValueToCssValueMap("1") = CSS.Value.DECIMAL
			htmlValueToCssValueMap("a") = CSS.Value.LOWER_ALPHA
			htmlValueToCssValueMap("A") = CSS.Value.UPPER_ALPHA
			htmlValueToCssValueMap("i") = CSS.Value.LOWER_ROMAN
			htmlValueToCssValueMap("I") = CSS.Value.UPPER_ROMAN

			' CSS-> internal CSS
			cssValueToInternalValueMap("none") = CSS.Value.NONE
			cssValueToInternalValueMap("disc") = CSS.Value.DISC
			cssValueToInternalValueMap("square") = CSS.Value.SQUARE
			cssValueToInternalValueMap("circle") = CSS.Value.CIRCLE
			cssValueToInternalValueMap("decimal") = CSS.Value.DECIMAL
			cssValueToInternalValueMap("lower-roman") = CSS.Value.LOWER_ROMAN
			cssValueToInternalValueMap("upper-roman") = CSS.Value.UPPER_ROMAN
			cssValueToInternalValueMap("lower-alpha") = CSS.Value.LOWER_ALPHA
			cssValueToInternalValueMap("upper-alpha") = CSS.Value.UPPER_ALPHA
			cssValueToInternalValueMap("repeat") = CSS.Value.BACKGROUND_REPEAT
			cssValueToInternalValueMap("no-repeat") = CSS.Value.BACKGROUND_NO_REPEAT
			cssValueToInternalValueMap("repeat-x") = CSS.Value.BACKGROUND_REPEAT_X
			cssValueToInternalValueMap("repeat-y") = CSS.Value.BACKGROUND_REPEAT_Y
			cssValueToInternalValueMap("scroll") = CSS.Value.BACKGROUND_SCROLL
			cssValueToInternalValueMap("fixed") = CSS.Value.BACKGROUND_FIXED

			' Register all the CSS attribute keys for archival/unarchival
			Dim keys As Object() = CSS.Attribute.allAttributes
			Try
				For Each key As Object In keys
					StyleContext.registerStaticAttributeKey(key)
				Next key
			Catch e As Exception
				e.printStackTrace()
			End Try

			' Register all the CSS Values for archival/unarchival
			keys = CSS.Value.allValues
			Try
				For Each key As Object In keys
					StyleContext.registerStaticAttributeKey(key)
				Next key
			Catch e As Exception
				e.printStackTrace()
			End Try
				lengthMapping.put("pt", New Single?(1f))
				' Not sure about 1.3, determined by experiementation.
				lengthMapping.put("px", New Single?(1.3f))
				lengthMapping.put("mm", New Single?(2.83464f))
				lengthMapping.put("cm", New Single?(28.3464f))
				lengthMapping.put("pc", New Single?(12f))
				lengthMapping.put("in", New Single?(72f))
				Dim res As Integer = 72
				Try
					res = java.awt.Toolkit.defaultToolkit.screenResolution
				Catch e As java.awt.HeadlessException
				End Try
				' mapping according to the CSS2 spec
				w3cLengthMapping.put("pt", New Single?(res/72f))
				w3cLengthMapping.put("px", New Single?(1f))
				w3cLengthMapping.put("mm", New Single?(res/25.4f))
				w3cLengthMapping.put("cm", New Single?(res/2.54f))
				w3cLengthMapping.put("pc", New Single?(res/6f))
				w3cLengthMapping.put("in", New Single?(res))
		End Sub

		''' <summary>
		''' Return the set of all possible CSS attribute keys.
		''' </summary>
		Public Property Shared allAttributeKeys As Attribute()
			Get
				Dim keys As Attribute() = New Attribute(Attribute.allAttributes.Length - 1){}
				Array.Copy(Attribute.allAttributes, 0, keys, 0, Attribute.allAttributes.Length)
				Return keys
			End Get
		End Property

		''' <summary>
		''' Translates a string to a <code>CSS.Attribute</code> object.
		''' This will return <code>null</code> if there is no attribute
		''' by the given name.
		''' </summary>
		''' <param name="name"> the name of the CSS attribute to fetch the
		'''  typesafe enumeration for </param>
		''' <returns> the <code>CSS.Attribute</code> object,
		'''  or <code>null</code> if the string
		'''  doesn't represent a valid attribute key </returns>
		Public Shared Function getAttribute(ByVal name As String) As Attribute
			Return attributeMap(name)
		End Function

		''' <summary>
		''' Translates a string to a <code>CSS.Value</code> object.
		''' This will return <code>null</code> if there is no value
		''' by the given name.
		''' </summary>
		''' <param name="name"> the name of the CSS value to fetch the
		'''  typesafe enumeration for </param>
		''' <returns> the <code>CSS.Value</code> object,
		'''  or <code>null</code> if the string
		'''  doesn't represent a valid CSS value name; this does
		'''  not mean that it doesn't represent a valid CSS value </returns>
		Friend Shared Function getValue(ByVal name As String) As Value
			Return valueMap(name)
		End Function


		'
		' Conversion related methods/classes
		'

		''' <summary>
		''' Returns a URL for the given CSS url string. If relative,
		''' <code>base</code> is used as the parent. If a valid URL can not
		''' be found, this will not throw a MalformedURLException, instead
		''' null will be returned.
		''' </summary>
		Friend Shared Function getURL(ByVal base As java.net.URL, ByVal cssString As String) As java.net.URL
			If cssString Is Nothing Then Return Nothing
			If cssString.StartsWith("url(") AndAlso cssString.EndsWith(")") Then cssString = cssString.Substring(4, cssString.Length - 1 - 4)
			' Absolute first
			Try
				Dim ___url As New java.net.URL(cssString)
				If ___url IsNot Nothing Then Return ___url
			Catch mue As java.net.MalformedURLException
			End Try
			' Then relative
			If base IsNot Nothing Then
				' Relative URL, try from base
				Try
					Dim ___url As New java.net.URL(base, cssString)
					Return ___url
				Catch muee As java.net.MalformedURLException
				End Try
			End If
			Return Nothing
		End Function

		''' <summary>
		''' Converts a type Color to a hex string
		''' in the format "#RRGGBB"
		''' </summary>
		Friend Shared Function colorToHex(ByVal color As java.awt.Color) As String

		  Dim colorstr As String = "#"

		  ' Red
		  Dim str As String = Integer.toHexString(color.red)
		  If str.Length > 2 Then
			str = str.Substring(0, 2)
		  ElseIf str.Length < 2 Then
			colorstr &= "0" & str
		  Else
			colorstr += str
		  End If

		  ' Green
		  str = Integer.toHexString(color.green)
		  If str.Length > 2 Then
			str = str.Substring(0, 2)
		  ElseIf str.Length < 2 Then
			colorstr &= "0" & str
		  Else
			colorstr += str
		  End If

		  ' Blue
		  str = Integer.toHexString(color.blue)
		  If str.Length > 2 Then
			str = str.Substring(0, 2)
		  ElseIf str.Length < 2 Then
			colorstr &= "0" & str
		  Else
			colorstr += str
		  End If

		  Return colorstr
		End Function

		 ''' <summary>
		 ''' Convert a "#FFFFFF" hex string to a Color.
		 ''' If the color specification is bad, an attempt
		 ''' will be made to fix it up.
		 ''' </summary>
		Friend Shared Function hexToColor(ByVal ___value As String) As java.awt.Color
			Dim digits As String
			Dim n As Integer = ___value.Length
			If ___value.StartsWith("#") Then
				digits = ___value.Substring(1, Math.Min(___value.Length, 7) - 1)
			Else
				digits = ___value
			End If
			Dim hstr As String = "0x" & digits
			Dim c As java.awt.Color
			Try
				c = java.awt.Color.decode(hstr)
			Catch nfe As NumberFormatException
				c = Nothing
			End Try
			 Return c
		End Function

		''' <summary>
		''' Convert a color string such as "RED" or "#NNNNNN" or "rgb(r, g, b)"
		''' to a Color.
		''' </summary>
		Friend Shared Function stringToColor(ByVal str As String) As java.awt.Color
		  Dim ___color As java.awt.Color

		  If str Is Nothing Then Return Nothing
		  If str.Length = 0 Then
			___color = java.awt.Color.black
		  ElseIf str.StartsWith("rgb(") Then
			  ___color = parseRGB(str)
		  ElseIf str.Chars(0) = "#"c Then
			___color = hexToColor(str)
		  ElseIf str.ToUpper() = "Black".ToUpper() Then
			___color = hexToColor("#000000")
		  ElseIf str.ToUpper() = "Silver".ToUpper() Then
			___color = hexToColor("#C0C0C0")
		  ElseIf str.ToUpper() = "Gray".ToUpper() Then
			___color = hexToColor("#808080")
		  ElseIf str.ToUpper() = "White".ToUpper() Then
			___color = hexToColor("#FFFFFF")
		  ElseIf str.ToUpper() = "Maroon".ToUpper() Then
			___color = hexToColor("#800000")
		  ElseIf str.ToUpper() = "Red".ToUpper() Then
			___color = hexToColor("#FF0000")
		  ElseIf str.ToUpper() = "Purple".ToUpper() Then
			___color = hexToColor("#800080")
		  ElseIf str.ToUpper() = "Fuchsia".ToUpper() Then
			___color = hexToColor("#FF00FF")
		  ElseIf str.ToUpper() = "Green".ToUpper() Then
			___color = hexToColor("#008000")
		  ElseIf str.ToUpper() = "Lime".ToUpper() Then
			___color = hexToColor("#00FF00")
		  ElseIf str.ToUpper() = "Olive".ToUpper() Then
			___color = hexToColor("#808000")
		  ElseIf str.ToUpper() = "Yellow".ToUpper() Then
			___color = hexToColor("#FFFF00")
		  ElseIf str.ToUpper() = "Navy".ToUpper() Then
			___color = hexToColor("#000080")
		  ElseIf str.ToUpper() = "Blue".ToUpper() Then
			___color = hexToColor("#0000FF")
		  ElseIf str.ToUpper() = "Teal".ToUpper() Then
			___color = hexToColor("#008080")
		  ElseIf str.ToUpper() = "Aqua".ToUpper() Then
			___color = hexToColor("#00FFFF")
		  ElseIf str.ToUpper() = "Orange".ToUpper() Then
			___color = hexToColor("#FF8000")
		  Else
			  ___color = hexToColor(str) ' sometimes get specified without leading #
		  End If
		  Return ___color
		End Function

		''' <summary>
		''' Parses a String in the format <code>rgb(r, g, b)</code> where
		''' each of the Color components is either an integer, or a floating number
		''' with a % after indicating a percentage value of 255. Values are
		''' constrained to fit with 0-255. The resulting Color is returned.
		''' </summary>
		Private Shared Function parseRGB(ByVal [string] As String) As java.awt.Color
			' Find the next numeric char
			Dim index As Integer() = New Integer(0){}

			index(0) = 4
			Dim red As Integer = getColorComponent([string], index)
			Dim green As Integer = getColorComponent([string], index)
			Dim blue As Integer = getColorComponent([string], index)

			Return New java.awt.Color(red, green, blue)
		End Function

		''' <summary>
		''' Returns the next integer value from <code>string</code> starting
		''' at <code>index[0]</code>. The value can either can an integer, or
		''' a percentage (floating number ending with %), in which case it is
		''' multiplied by 255.
		''' </summary>
		Private Shared Function getColorComponent(ByVal [string] As String, ByVal index As Integer()) As Integer
			Dim ___length As Integer = [string].Length
			Dim aChar As Char

			' Skip non-decimal chars
			aChar = [string].Chars(index(0))
			Do While index(0) < ___length AndAlso aChar <> "-"c AndAlso (Not Char.IsDigit(aChar)) AndAlso aChar <> "."c
				index(0) += 1
				aChar = [string].Chars(index(0))
			Loop

			Dim start As Integer = index(0)

			If start < ___length AndAlso [string].Chars(index(0)) = "-"c Then index(0) += 1
			Do While index(0) < ___length AndAlso Char.IsDigit([string].Chars(index(0)))
				index(0) += 1
			Loop
			If index(0) < ___length AndAlso [string].Chars(index(0)) = "."c Then
				' Decimal value
				index(0) += 1
				Do While index(0) < ___length AndAlso Char.IsDigit([string].Chars(index(0)))
					index(0) += 1
				Loop
			End If
			If start <> index(0) Then
				Try
					Dim ___value As Single = Convert.ToSingle([string].Substring(start, index(0) - start))

					If index(0) < ___length AndAlso [string].Chars(index(0)) = "%"c Then
						index(0) += 1
						___value = ___value * 255f / 100f
					End If
					Return Math.Min(255, Math.Max(0, CInt(Fix(___value))))
				Catch nfe As NumberFormatException
					' Treat as 0
				End Try
			End If
			Return 0
		End Function

		Friend Shared Function getIndexOfSize(ByVal pt As Single, ByVal sizeMap As Integer()) As Integer
			For i As Integer = 0 To sizeMap.Length - 1
					If pt <= sizeMap(i) Then Return i + 1
			Next i
			Return sizeMap.Length
		End Function

		Friend Shared Function getIndexOfSize(ByVal pt As Single, ByVal ss As StyleSheet) As Integer
			Dim sizeMap As Integer() = If(ss IsNot Nothing, ss.sizeMap, StyleSheet.sizeMapDefault)
			Return getIndexOfSize(pt, sizeMap)
		End Function


		''' <returns> an array of all the strings in <code>value</code>
		'''         that are separated by whitespace. </returns>
		Friend Shared Function parseStrings(ByVal ___value As String) As String()
			Dim current, last As Integer
			Dim ___length As Integer = If(___value Is Nothing, 0, ___value.Length)
			Dim temp As New List(Of String)(4)

			current = 0
			Do While current < ___length
				' Skip ws
				Do While current < ___length AndAlso Char.IsWhiteSpace(___value.Chars(current))
					current += 1
				Loop
				last = current
				Do While current < ___length AndAlso Not Char.IsWhiteSpace(___value.Chars(current))
					current += 1
				Loop
				If last <> current Then temp.Add(___value.Substring(last, current - last))
				current += 1
			Loop
			Dim retValue As String() = New String(temp.Count - 1){}
			temp.CopyTo(retValue)
			Return retValue
		End Function

		''' <summary>
		''' Return the point size, given a size index. Legal HTML index sizes
		''' are 1-7.
		''' </summary>
		Friend Overridable Function getPointSize(ByVal index As Integer, ByVal ss As StyleSheet) As Single
			ss = getStyleSheet(ss)
			Dim sizeMap As Integer() = If(ss IsNot Nothing, ss.sizeMap, StyleSheet.sizeMapDefault)
			index -= 1
			If index < 0 Then
			  Return sizeMap(0)
			ElseIf index > sizeMap.Length - 1 Then
			  Return sizeMap(sizeMap.Length - 1)
			Else
			  Return sizeMap(index)
			End If
		End Function


		Private Sub translateEmbeddedAttributes(ByVal htmlAttrSet As AttributeSet, ByVal cssAttrSet As MutableAttributeSet)
			Dim keys As System.Collections.IEnumerator = htmlAttrSet.attributeNames
			If htmlAttrSet.getAttribute(StyleConstants.NameAttribute) Is HTML.Tag.HR Then translateAttributes(HTML.Tag.HR, htmlAttrSet, cssAttrSet)
			Do While keys.hasMoreElements()
				Dim key As Object = keys.nextElement()
				If TypeOf key Is HTML.Tag Then
					Dim tag As HTML.Tag = CType(key, HTML.Tag)
					Dim o As Object = htmlAttrSet.getAttribute(tag)
					If o IsNot Nothing AndAlso TypeOf o Is AttributeSet Then translateAttributes(tag, CType(o, AttributeSet), cssAttrSet)
				ElseIf TypeOf key Is CSS.Attribute Then
					cssAttrSet.addAttribute(key, htmlAttrSet.getAttribute(key))
				End If
			Loop
		End Sub

		Private Sub translateAttributes(ByVal tag As HTML.Tag, ByVal htmlAttrSet As AttributeSet, ByVal cssAttrSet As MutableAttributeSet)
			Dim names As System.Collections.IEnumerator = htmlAttrSet.attributeNames
			Do While names.hasMoreElements()
				Dim name As Object = names.nextElement()

				If TypeOf name Is HTML.Attribute Then
					Dim key As HTML.Attribute = CType(name, HTML.Attribute)

	'                
	'                 * HTML.Attribute.ALIGN needs special processing.
	'                 * It can map to to 1 of many(3) possible CSS attributes
	'                 * depending on the nature of the tag the attribute is
	'                 * part off and depending on the value of the attribute.
	'                 
					If key Is HTML.Attribute.ALIGN Then
						Dim htmlAttrValue As String = CStr(htmlAttrSet.getAttribute(HTML.Attribute.ALIGN))
						If htmlAttrValue IsNot Nothing Then
							Dim cssAttr As CSS.Attribute = getCssAlignAttribute(tag, htmlAttrSet)
							If cssAttr IsNot Nothing Then
								Dim o As Object = getCssValue(cssAttr, htmlAttrValue)
								If o IsNot Nothing Then cssAttrSet.addAttribute(cssAttr, o)
							End If
						End If
					Else
						If key Is HTML.Attribute.SIZE AndAlso (Not isHTMLFontTag(tag)) Then
	'                        
	'                         * The html size attribute has a mapping in the CSS world only
	'                         * if it is par of a font or base font tag.
	'                         
						ElseIf tag Is HTML.Tag.TABLE AndAlso key Is HTML.Attribute.BORDER Then
							Dim borderWidth As Integer = getTableBorder(htmlAttrSet)

							If borderWidth > 0 Then translateAttribute(HTML.Attribute.BORDER, Convert.ToString(borderWidth), cssAttrSet)
						Else
							translateAttribute(key, CStr(htmlAttrSet.getAttribute(key)), cssAttrSet)
						End If
					End If
				ElseIf TypeOf name Is CSS.Attribute Then
					cssAttrSet.addAttribute(name, htmlAttrSet.getAttribute(name))
				End If
			Loop
		End Sub

		Private Sub translateAttribute(ByVal key As HTML.Attribute, ByVal htmlAttrValue As String, ByVal cssAttrSet As MutableAttributeSet)
	'        
	'         * In the case of all remaining HTML.Attribute's they
	'         * map to 1 or more CCS.Attribute.
	'         
			Dim cssAttrList As CSS.Attribute() = getCssAttribute(key)

			If cssAttrList Is Nothing OrElse htmlAttrValue Is Nothing Then Return
			For Each cssAttr As Attribute In cssAttrList
				Dim o As Object = getCssValue(cssAttr, htmlAttrValue)
				If o IsNot Nothing Then cssAttrSet.addAttribute(cssAttr, o)
			Next cssAttr
		End Sub

		''' <summary>
		''' Given a CSS.Attribute object and its corresponding HTML.Attribute's
		''' value, this method returns a CssValue object to associate with the
		''' CSS attribute.
		''' </summary>
		''' <param name="the"> CSS.Attribute </param>
		''' <param name="a"> String containing the value associated HTML.Attribtue. </param>
		Friend Overridable Function getCssValue(ByVal cssAttr As CSS.Attribute, ByVal htmlAttrValue As String) As Object
			Dim ___value As CssValue = CType(valueConvertor(cssAttr), CssValue)
			Dim o As Object = ___value.parseHtmlValue(htmlAttrValue)
			Return o
		End Function

		''' <summary>
		''' Maps an HTML.Attribute object to its appropriate CSS.Attributes.
		''' </summary>
		''' <param name="HTML.Attribute"> </param>
		''' <returns> CSS.Attribute[] </returns>
		Private Function getCssAttribute(ByVal hAttr As HTML.Attribute) As CSS.Attribute()
			Return htmlAttrToCssAttrMap(hAttr)
		End Function

		''' <summary>
		''' Maps HTML.Attribute.ALIGN to either:
		'''     CSS.Attribute.TEXT_ALIGN
		'''     CSS.Attribute.FLOAT
		'''     CSS.Attribute.VERTICAL_ALIGN
		''' based on the tag associated with the attribute and the
		''' value of the attribute.
		''' </summary>
		''' <param name="AttributeSet"> containing HTML attributes. </param>
		''' <returns> CSS.Attribute mapping for HTML.Attribute.ALIGN. </returns>
		Private Function getCssAlignAttribute(ByVal tag As HTML.Tag, ByVal htmlAttrSet As AttributeSet) As CSS.Attribute
			Return CSS.Attribute.TEXT_ALIGN
	'
	'        String htmlAttrValue = (String)htmlAttrSet.getAttribute(HTML.Attribute.ALIGN);
	'        CSS.Attribute cssAttr = CSS.Attribute.TEXT_ALIGN;
	'        if (htmlAttrValue != null && htmlAttrSet instanceof Element) {
	'            Element elem = (Element)htmlAttrSet;
	'            if (!elem.isLeaf() && tag.isBlock() && validTextAlignValue(htmlAttrValue)) {
	'                return CSS.Attribute.TEXT_ALIGN;
	'            } else if (isFloater(htmlAttrValue)) {
	'                return CSS.Attribute.FLOAT;
	'            } else if (elem.isLeaf()) {
	'                return CSS.Attribute.VERTICAL_ALIGN;
	'            }
	'        }
	'        return null;
	'        
		End Function

		''' <summary>
		''' Fetches the tag associated with the HTML AttributeSet.
		''' </summary>
		''' <param name="AttributeSet"> containing the HTML attributes. </param>
		''' <returns> HTML.Tag </returns>
		Private Function getHTMLTag(ByVal htmlAttrSet As AttributeSet) As HTML.Tag
			Dim o As Object = htmlAttrSet.getAttribute(StyleConstants.NameAttribute)
			If TypeOf o Is HTML.Tag Then
				Dim tag As HTML.Tag = CType(o, HTML.Tag)
				Return tag
			End If
			Return Nothing
		End Function


		Private Function isHTMLFontTag(ByVal tag As HTML.Tag) As Boolean
			Return (tag IsNot Nothing AndAlso ((tag Is HTML.Tag.FONT) OrElse (tag Is HTML.Tag.BASEFONT)))
		End Function


		Private Function isFloater(ByVal alignValue As String) As Boolean
			Return (alignValue.Equals("left") OrElse alignValue.Equals("right"))
		End Function

		Private Function validTextAlignValue(ByVal alignValue As String) As Boolean
			Return (isFloater(alignValue) OrElse alignValue.Equals("center"))
		End Function

		''' <summary>
		''' Base class to CSS values in the attribute sets.  This
		''' is intended to act as a convertor to/from other attribute
		''' formats.
		''' <p>
		''' The CSS parser uses the parseCssValue method to convert
		''' a string to whatever format is appropriate a given key
		''' (i.e. these convertors are stored in a map using the
		''' CSS.Attribute as a key and the CssValue as the value).
		''' <p>
		''' The HTML to CSS conversion process first converts the
		''' HTML.Attribute to a CSS.Attribute, and then calls
		''' the parseHtmlValue method on the value of the HTML
		''' attribute to produce the corresponding CSS value.
		''' <p>
		''' The StyleConstants to CSS conversion process first
		''' converts the StyleConstants attribute to a
		''' CSS.Attribute, and then calls the fromStyleConstants
		''' method to convert the StyleConstants value to a
		''' CSS value.
		''' <p>
		''' The CSS to StyleConstants conversion process first
		''' converts the StyleConstants attribute to a
		''' CSS.Attribute, and then calls the toStyleConstants
		''' method to convert the CSS value to a StyleConstants
		''' value.
		''' </summary>
		<Serializable> _
		Friend Class CssValue

			''' <summary>
			''' Convert a CSS value string to the internal format
			''' (for fast processing) used in the attribute sets.
			''' The fallback storage for any value that we don't
			''' have a special binary format for is a String.
			''' </summary>
			Friend Overridable Function parseCssValue(ByVal ___value As String) As Object
				Return ___value
			End Function

			''' <summary>
			''' Convert an HTML attribute value to a CSS attribute
			''' value.  If there is no conversion, return null.
			''' This is implemented to simply forward to the CSS
			''' parsing by default (since some of the attribute
			''' values are the same).  If the attribute value
			''' isn't recognized as a CSS value it is generally
			''' returned as null.
			''' </summary>
			Friend Overridable Function parseHtmlValue(ByVal ___value As String) As Object
				Return parseCssValue(___value)
			End Function

			''' <summary>
			''' Converts a <code>StyleConstants</code> attribute value to
			''' a CSS attribute value.  If there is no conversion,
			''' returns <code>null</code>.  By default, there is no conversion.
			''' </summary>
			''' <param name="key"> the <code>StyleConstants</code> attribute </param>
			''' <param name="value"> the value of a <code>StyleConstants</code>
			'''   attribute to be converted </param>
			''' <returns> the CSS value that represents the
			'''   <code>StyleConstants</code> value </returns>
			Friend Overridable Function fromStyleConstants(ByVal key As StyleConstants, ByVal ___value As Object) As Object
				Return Nothing
			End Function

			''' <summary>
			''' Converts a CSS attribute value to a
			''' <code>StyleConstants</code>
			''' value.  If there is no conversion, returns
			''' <code>null</code>.
			''' By default, there is no conversion.
			''' </summary>
			''' <param name="key"> the <code>StyleConstants</code> attribute </param>
			''' <param name="v"> the view containing <code>AttributeSet</code> </param>
			''' <returns> the <code>StyleConstants</code> attribute value that
			'''   represents the CSS attribute value </returns>
			Friend Overridable Function toStyleConstants(ByVal key As StyleConstants, ByVal v As View) As Object
				Return Nothing
			End Function

			''' <summary>
			''' Return the CSS format of the value
			''' </summary>
			Public Overrides Function ToString() As String
				Return svalue
			End Function

			''' <summary>
			''' The value as a string... before conversion to a
			''' binary format.
			''' </summary>
			Friend svalue As String
		End Class

		''' <summary>
		''' By default CSS attributes are represented as simple
		''' strings.  They also have no conversion to/from
		''' StyleConstants by default. This class represents the
		''' value as a string (via the superclass), but
		''' provides StyleConstants conversion support for the
		''' CSS attributes that are held as strings.
		''' </summary>
		Friend Class StringValue
			Inherits CssValue

			''' <summary>
			''' Convert a CSS value string to the internal format
			''' (for fast processing) used in the attribute sets.
			''' This produces a StringValue, so that it can be
			''' used to convert from CSS to StyleConstants values.
			''' </summary>
			Friend Overrides Function parseCssValue(ByVal ___value As String) As Object
				Dim sv As New StringValue
				sv.svalue = ___value
				Return sv
			End Function

			''' <summary>
			''' Converts a <code>StyleConstants</code> attribute value to
			''' a CSS attribute value.  If there is no conversion
			''' returns <code>null</code>.
			''' </summary>
			''' <param name="key"> the <code>StyleConstants</code> attribute </param>
			''' <param name="value"> the value of a <code>StyleConstants</code>
			'''   attribute to be converted </param>
			''' <returns> the CSS value that represents the
			'''   <code>StyleConstants</code> value </returns>
			Friend Overrides Function fromStyleConstants(ByVal key As StyleConstants, ByVal ___value As Object) As Object
				If key Is StyleConstants.Italic Then
					If ___value.Equals(Boolean.TRUE) Then Return parseCssValue("italic")
					Return parseCssValue("")
				ElseIf key Is StyleConstants.Underline Then
					If ___value.Equals(Boolean.TRUE) Then Return parseCssValue("underline")
					Return parseCssValue("")
				ElseIf key Is StyleConstants.Alignment Then
					Dim align As Integer = CInt(Fix(___value))
					Dim ta As String
					Select Case align
					Case StyleConstants.ALIGN_LEFT
						ta = "left"
					Case StyleConstants.ALIGN_RIGHT
						ta = "right"
					Case StyleConstants.ALIGN_CENTER
						ta = "center"
					Case StyleConstants.ALIGN_JUSTIFIED
						ta = "justify"
					Case Else
						ta = "left"
					End Select
					Return parseCssValue(ta)
				ElseIf key Is StyleConstants.StrikeThrough Then
					If ___value.Equals(Boolean.TRUE) Then Return parseCssValue("line-through")
					Return parseCssValue("")
				ElseIf key Is StyleConstants.Superscript Then
					If ___value.Equals(Boolean.TRUE) Then Return parseCssValue("super")
					Return parseCssValue("")
				ElseIf key Is StyleConstants.Subscript Then
					If ___value.Equals(Boolean.TRUE) Then Return parseCssValue("sub")
					Return parseCssValue("")
				End If
				Return Nothing
			End Function

			''' <summary>
			''' Converts a CSS attribute value to a
			''' <code>StyleConstants</code> value.
			''' If there is no conversion, returns <code>null</code>.
			''' By default, there is no conversion.
			''' </summary>
			''' <param name="key"> the <code>StyleConstants</code> attribute </param>
			''' <returns> the <code>StyleConstants</code> attribute value that
			'''   represents the CSS attribute value </returns>
			Friend Overrides Function toStyleConstants(ByVal key As StyleConstants, ByVal v As View) As Object
				If key Is StyleConstants.Italic Then
					If svalue.IndexOf("italic") >= 0 Then Return Boolean.TRUE
					Return Boolean.FALSE
				ElseIf key Is StyleConstants.Underline Then
					If svalue.IndexOf("underline") >= 0 Then Return Boolean.TRUE
					Return Boolean.FALSE
				ElseIf key Is StyleConstants.Alignment Then
					If svalue.Equals("right") Then
						Return New Integer?(StyleConstants.ALIGN_RIGHT)
					ElseIf svalue.Equals("center") Then
						Return New Integer?(StyleConstants.ALIGN_CENTER)
					ElseIf svalue.Equals("justify") Then
						Return New Integer?(StyleConstants.ALIGN_JUSTIFIED)
					End If
					Return New Integer?(StyleConstants.ALIGN_LEFT)
				ElseIf key Is StyleConstants.StrikeThrough Then
					If svalue.IndexOf("line-through") >= 0 Then Return Boolean.TRUE
					Return Boolean.FALSE
				ElseIf key Is StyleConstants.Superscript Then
					If svalue.IndexOf("super") >= 0 Then Return Boolean.TRUE
					Return Boolean.FALSE
				ElseIf key Is StyleConstants.Subscript Then
					If svalue.IndexOf("sub") >= 0 Then Return Boolean.TRUE
					Return Boolean.FALSE
				End If
				Return Nothing
			End Function

			' Used by ViewAttributeSet
			Friend Overridable Property italic As Boolean
				Get
					Return (svalue.IndexOf("italic") <> -1)
				End Get
			End Property

			Friend Overridable Property strike As Boolean
				Get
					Return (svalue.IndexOf("line-through") <> -1)
				End Get
			End Property

			Friend Overridable Property underline As Boolean
				Get
					Return (svalue.IndexOf("underline") <> -1)
				End Get
			End Property

			Friend Overridable Property [sub] As Boolean
				Get
					Return (svalue.IndexOf("sub") <> -1)
				End Get
			End Property

			Friend Overridable Property sup As Boolean
				Get
					Return (svalue.IndexOf("sup") <> -1)
				End Get
			End Property
		End Class

		''' <summary>
		''' Represents a value for the CSS.FONT_SIZE attribute.
		''' The binary format of the value can be one of several
		''' types.  If the type is Float,
		''' the value is specified in terms of point or
		''' percentage, depending upon the ending of the
		''' associated string.
		''' If the type is Integer, the value is specified
		''' in terms of a size index.
		''' </summary>
		Friend Class FontSize
			Inherits CssValue

			Private ReadOnly outerInstance As CSS

			Public Sub New(ByVal outerInstance As CSS)
				Me.outerInstance = outerInstance
			End Sub


			''' <summary>
			''' Returns the size in points.  This is ultimately
			''' what we need for the purpose of creating/fetching
			''' a Font object.
			''' </summary>
			''' <param name="a"> the attribute set the value is being
			'''  requested from.  We may need to walk up the
			'''  resolve hierarchy if it's relative. </param>
			Friend Overridable Function getValue(ByVal a As AttributeSet, ByVal ss As StyleSheet) As Integer
				ss = outerInstance.getStyleSheet(ss)
				If index Then
					' it's an index, translate from size table
					Return Math.Round(outerInstance.getPointSize(CInt(Fix(___value)), ss))
				ElseIf lu Is Nothing Then
					Return Math.Round(___value)
				Else
					If lu.type = 0 Then
						Dim isW3CLengthUnits As Boolean = If(ss Is Nothing, False, ss.w3CLengthUnits)
						Return Math.Round(lu.getValue(isW3CLengthUnits))
					End If
					If a IsNot Nothing Then
						Dim resolveParent As AttributeSet = a.resolveParent

						If resolveParent IsNot Nothing Then
							Dim pValue As Integer = StyleConstants.getFontSize(resolveParent)

							Dim retValue As Single
							If lu.type = 1 OrElse lu.type = 3 Then
								retValue = lu.___value * CSng(pValue)
							Else
								retValue = lu.___value + CSng(pValue)
							End If
							Return Math.Round(retValue)
						End If
					End If
					' a is null, or no resolve parent.
					Return 12
				End If
			End Function

			Friend Overrides Function parseCssValue(ByVal ___value As String) As Object
				Dim fs As New FontSize
				fs.svalue = ___value
				Try
					If ___value.Equals("xx-small") Then
						fs.___value = 1
						fs.index = True
					ElseIf ___value.Equals("x-small") Then
						fs.___value = 2
						fs.index = True
					ElseIf ___value.Equals("small") Then
						fs.___value = 3
						fs.index = True
					ElseIf ___value.Equals("medium") Then
						fs.___value = 4
						fs.index = True
					ElseIf ___value.Equals("large") Then
						fs.___value = 5
						fs.index = True
					ElseIf ___value.Equals("x-large") Then
						fs.___value = 6
						fs.index = True
					ElseIf ___value.Equals("xx-large") Then
						fs.___value = 7
						fs.index = True
					Else
						fs.lu = New LengthUnit(___value, CShort(1), 1f)
					End If
					' relative sizes, larger | smaller (adjust from parent by
					' 1.5 pixels)
					' em, ex refer to parent sizes
					' lengths: pt, mm, cm, pc, in, px
					'          em (font height 3em would be 3 times font height)
					'          ex (height of X)
					' lengths are (+/-) followed by a number and two letter
					' unit identifier
				Catch nfe As NumberFormatException
					fs = Nothing
				End Try
				Return fs
			End Function

			Friend Overrides Function parseHtmlValue(ByVal ___value As String) As Object
				If (___value Is Nothing) OrElse (___value.Length = 0) Then Return Nothing
				Dim fs As New FontSize
				fs.svalue = ___value

				Try
	'                
	'                 * relative sizes in the size attribute are relative
	'                 * to the <basefont>'s size.
	'                 
					Dim baseFontSize As Integer = outerInstance.baseFontSize
					If ___value.Chars(0) = "+"c Then
						Dim relSize As Integer = Convert.ToInt32(___value.Substring(1))
						fs.___value = baseFontSize + relSize
						fs.index = True
					ElseIf ___value.Chars(0) = "-"c Then
						Dim relSize As Integer = -Convert.ToInt32(___value.Substring(1))
						fs.___value = baseFontSize + relSize
						fs.index = True
					Else
						fs.___value = Convert.ToInt32(___value)
						If fs.___value > 7 Then
							fs.___value = 7
						ElseIf fs.___value < 0 Then
							fs.___value = 0
						End If
						fs.index = True
					End If

				Catch nfe As NumberFormatException
					fs = Nothing
				End Try
				Return fs
			End Function

			''' <summary>
			''' Converts a <code>StyleConstants</code> attribute value to
			''' a CSS attribute value.  If there is no conversion
			''' returns <code>null</code>.  By default, there is no conversion.
			''' </summary>
			''' <param name="key"> the <code>StyleConstants</code> attribute </param>
			''' <param name="value"> the value of a <code>StyleConstants</code>
			'''   attribute to be converted </param>
			''' <returns> the CSS value that represents the
			'''   <code>StyleConstants</code> value </returns>
			Friend Overrides Function fromStyleConstants(ByVal key As StyleConstants, ByVal ___value As Object) As Object
				If TypeOf ___value Is Number Then
					Dim fs As New FontSize

					fs.___value = getIndexOfSize(CType(___value, Number), StyleSheet.sizeMapDefault)
					fs.svalue = Convert.ToString(CInt(Fix(fs.___value)))
					fs.index = True
					Return fs
				End If
				Return parseCssValue(___value.ToString())
			End Function

			''' <summary>
			''' Converts a CSS attribute value to a <code>StyleConstants</code>
			''' value.  If there is no conversion, returns <code>null</code>.
			''' By default, there is no conversion.
			''' </summary>
			''' <param name="key"> the <code>StyleConstants</code> attribute </param>
			''' <returns> the <code>StyleConstants</code> attribute value that
			'''   represents the CSS attribute value </returns>
			Friend Overrides Function toStyleConstants(ByVal key As StyleConstants, ByVal v As View) As Object
				If v IsNot Nothing Then Return Convert.ToInt32(getValue(v.attributes, Nothing))
				Return Convert.ToInt32(getValue(Nothing, Nothing))
			End Function

			Friend ___value As Single
			Friend index As Boolean
			Friend lu As LengthUnit
		End Class

		Friend Class FontFamily
			Inherits CssValue

			''' <summary>
			''' Returns the font family to use.
			''' </summary>
			Friend Overridable Property value As String
				Get
					Return family
				End Get
			End Property

			Friend Overrides Function parseCssValue(ByVal ___value As String) As Object
				Dim cIndex As Integer = ___value.IndexOf(","c)
				Dim ff As New FontFamily
				ff.svalue = ___value
				ff.family = Nothing

				If cIndex = -1 Then
					fontNameame(ff, ___value)
				Else
					Dim done As Boolean = False
					Dim lastIndex As Integer
					Dim length As Integer = ___value.Length
					cIndex = 0
					Do While Not done
						' skip ws.
						Do While cIndex < length AndAlso Char.IsWhiteSpace(___value.Chars(cIndex))
							cIndex += 1
						Loop
						' Find next ','
						lastIndex = cIndex
						cIndex = ___value.IndexOf(","c, cIndex)
						If cIndex = -1 Then cIndex = length
						If lastIndex < length Then
							If lastIndex <> cIndex Then
								Dim lastCharIndex As Integer = cIndex
								If cIndex > 0 AndAlso ___value.Chars(cIndex - 1) = " "c Then lastCharIndex -= 1
								fontNameame(ff, ___value.Substring(lastIndex, lastCharIndex - lastIndex))
								done = (ff.family IsNot Nothing)
							End If
							cIndex += 1
						Else
							done = True
						End If
					Loop
				End If
				If ff.family Is Nothing Then ff.family = java.awt.Font.SANS_SERIF
				Return ff
			End Function

			Private Sub setFontName(ByVal ff As FontFamily, ByVal fontName As String)
				ff.family = fontName
			End Sub

			Friend Overrides Function parseHtmlValue(ByVal ___value As String) As Object
				' TBD
				Return parseCssValue(___value)
			End Function

			''' <summary>
			''' Converts a <code>StyleConstants</code> attribute value to
			''' a CSS attribute value.  If there is no conversion
			''' returns <code>null</code>.  By default, there is no conversion.
			''' </summary>
			''' <param name="key"> the <code>StyleConstants</code> attribute </param>
			''' <param name="value"> the value of a <code>StyleConstants</code>
			'''   attribute to be converted </param>
			''' <returns> the CSS value that represents the
			'''   <code>StyleConstants</code> value </returns>
			Friend Overrides Function fromStyleConstants(ByVal key As StyleConstants, ByVal ___value As Object) As Object
				Return parseCssValue(___value.ToString())
			End Function

			''' <summary>
			''' Converts a CSS attribute value to a <code>StyleConstants</code>
			''' value.  If there is no conversion, returns <code>null</code>.
			''' By default, there is no conversion.
			''' </summary>
			''' <param name="key"> the <code>StyleConstants</code> attribute </param>
			''' <returns> the <code>StyleConstants</code> attribute value that
			'''   represents the CSS attribute value </returns>
			Friend Overrides Function toStyleConstants(ByVal key As StyleConstants, ByVal v As View) As Object
				Return family
			End Function

			Friend family As String
		End Class

		Friend Class FontWeight
			Inherits CssValue

			Friend Overridable Property value As Integer
				Get
					Return weight
				End Get
			End Property

			Friend Overrides Function parseCssValue(ByVal ___value As String) As Object
				Dim fw As New FontWeight
				fw.svalue = ___value
				If ___value.Equals("bold") Then
					fw.weight = 700
				ElseIf ___value.Equals("normal") Then
					fw.weight = 400
				Else
					' PENDING(prinz) add support for relative values
					Try
						fw.weight = Convert.ToInt32(___value)
					Catch nfe As NumberFormatException
						fw = Nothing
					End Try
				End If
				Return fw
			End Function

			''' <summary>
			''' Converts a <code>StyleConstants</code> attribute value to
			''' a CSS attribute value.  If there is no conversion
			''' returns <code>null</code>.  By default, there is no conversion.
			''' </summary>
			''' <param name="key"> the <code>StyleConstants</code> attribute </param>
			''' <param name="value"> the value of a <code>StyleConstants</code>
			'''   attribute to be converted </param>
			''' <returns> the CSS value that represents the
			'''   <code>StyleConstants</code> value </returns>
			Friend Overrides Function fromStyleConstants(ByVal key As StyleConstants, ByVal ___value As Object) As Object
				If ___value.Equals(Boolean.TRUE) Then Return parseCssValue("bold")
				Return parseCssValue("normal")
			End Function

			''' <summary>
			''' Converts a CSS attribute value to a <code>StyleConstants</code>
			''' value.  If there is no conversion, returns <code>null</code>.
			''' By default, there is no conversion.
			''' </summary>
			''' <param name="key"> the <code>StyleConstants</code> attribute </param>
			''' <returns> the <code>StyleConstants</code> attribute value that
			'''   represents the CSS attribute value </returns>
			Friend Overrides Function toStyleConstants(ByVal key As StyleConstants, ByVal v As View) As Object
				Return If(weight > 500, Boolean.TRUE, Boolean.FALSE)
			End Function

			Friend Overridable Property bold As Boolean
				Get
					Return (weight > 500)
				End Get
			End Property

			Friend weight As Integer
		End Class

		Friend Class ColorValue
			Inherits CssValue

			''' <summary>
			''' Returns the color to use.
			''' </summary>
			Friend Overridable Property value As java.awt.Color
				Get
					Return c
				End Get
			End Property

			Friend Overrides Function parseCssValue(ByVal ___value As String) As Object

				Dim c As java.awt.Color = stringToColor(___value)
				If c IsNot Nothing Then
					Dim cv As New ColorValue
					cv.svalue = ___value
					cv.c = c
					Return cv
				End If
				Return Nothing
			End Function

			Friend Overrides Function parseHtmlValue(ByVal ___value As String) As Object
				Return parseCssValue(___value)
			End Function

			''' <summary>
			''' Converts a <code>StyleConstants</code> attribute value to
			''' a CSS attribute value.  If there is no conversion
			''' returns <code>null</code>.  By default, there is no conversion.
			''' </summary>
			''' <param name="key"> the <code>StyleConstants</code> attribute </param>
			''' <param name="value"> the value of a <code>StyleConstants</code>
			'''   attribute to be converted </param>
			''' <returns> the CSS value that represents the
			'''   <code>StyleConstants</code> value </returns>
			Friend Overrides Function fromStyleConstants(ByVal key As StyleConstants, ByVal ___value As Object) As Object
				Dim colorValue As New ColorValue
				colorValue.c = CType(___value, java.awt.Color)
				colorValue.svalue = colorToHex(colorValue.c)
				Return colorValue
			End Function

			''' <summary>
			''' Converts a CSS attribute value to a <code>StyleConstants</code>
			''' value.  If there is no conversion, returns <code>null</code>.
			''' By default, there is no conversion.
			''' </summary>
			''' <param name="key"> the <code>StyleConstants</code> attribute </param>
			''' <returns> the <code>StyleConstants</code> attribute value that
			'''   represents the CSS attribute value </returns>
			Friend Overrides Function toStyleConstants(ByVal key As StyleConstants, ByVal v As View) As Object
				Return c
			End Function

			Friend c As java.awt.Color
		End Class

		Friend Class BorderStyle
			Inherits CssValue

			Friend Overridable Property value As CSS.Value
				Get
					Return style
				End Get
			End Property

			Friend Overrides Function parseCssValue(ByVal ___value As String) As Object
				Dim cssv As CSS.Value = CSS.getValue(___value)
				If cssv IsNot Nothing Then
					If (cssv Is CSS.Value.INSET) OrElse (cssv Is CSS.Value.OUTSET) OrElse (cssv Is CSS.Value.NONE) OrElse (cssv Is CSS.Value.DOTTED) OrElse (cssv Is CSS.Value.DASHED) OrElse (cssv Is CSS.Value.SOLID) OrElse (cssv Is CSS.Value.DOUBLE) OrElse (cssv Is CSS.Value.GROOVE) OrElse (cssv Is CSS.Value.RIDGE) Then

						Dim bs As New BorderStyle
						bs.svalue = ___value
						bs.style = cssv
						Return bs
					End If
				End If
				Return Nothing
			End Function

			Private Sub writeObject(ByVal s As java.io.ObjectOutputStream)
				s.defaultWriteObject()
				If style Is Nothing Then
					s.writeObject(Nothing)
				Else
					s.writeObject(style.ToString())
				End If
			End Sub

			Private Sub readObject(ByVal s As ObjectInputStream)
				s.defaultReadObject()
				Dim ___value As Object = s.readObject()
				If ___value IsNot Nothing Then style = CSS.getValue(CStr(___value))
			End Sub

			' CSS.Values are static, don't archive it.
			<NonSerialized> _
			Private style As CSS.Value
		End Class

		Friend Class LengthValue
			Inherits CssValue

			''' <summary>
			''' if this length value may be negative.
			''' </summary>
			Friend mayBeNegative As Boolean

			Friend Sub New()
				Me.New(False)
			End Sub

			Friend Sub New(ByVal mayBeNegative As Boolean)
				Me.mayBeNegative = mayBeNegative
			End Sub

			''' <summary>
			''' Returns the length (span) to use.
			''' </summary>
			Friend Overridable Property value As Single
				Get
					Return getValue(False)
				End Get
			End Property

			Friend Overridable Function getValue(ByVal isW3CLengthUnits As Boolean) As Single
				Return getValue(0, isW3CLengthUnits)
			End Function

			''' <summary>
			''' Returns the length (span) to use. If the value represents
			''' a percentage, it is scaled based on <code>currentValue</code>.
			''' </summary>
			Friend Overridable Function getValue(ByVal currentValue As Single) As Single
				Return getValue(currentValue, False)
			End Function
			Friend Overridable Function getValue(ByVal currentValue As Single, ByVal isW3CLengthUnits As Boolean) As Single
				If percentage Then Return span * currentValue
				Return LengthUnit.getValue(span, units, isW3CLengthUnits)
			End Function

			''' <summary>
			''' Returns true if the length represents a percentage of the
			''' containing box.
			''' </summary>
			Friend Overridable Property percentage As Boolean
				Get
					Return percentage
				End Get
			End Property

			Friend Overrides Function parseCssValue(ByVal ___value As String) As Object
				Dim lv As LengthValue
				Try
					' Assume pixels
					Dim absolute As Single = Convert.ToSingle(___value)
					lv = New LengthValue
					lv.span = absolute
				Catch nfe As NumberFormatException
					' Not pixels, use LengthUnit
					Dim lu As New LengthUnit(___value, LengthUnit.UNINITALIZED_LENGTH, 0)

					' PENDING: currently, we only support absolute values and
					' percentages.
					Select Case lu.type
					Case 0
						' Absolute
						lv = New LengthValue
						lv.span = If(mayBeNegative, lu.___value, Math.Max(0, lu.___value))
						lv.units = lu.units
					Case 1
						' %
						lv = New LengthValue
						lv.span = Math.Max(0, Math.Min(1, lu.___value))
						lv.percentage = True
					Case Else
						Return Nothing
					End Select
				End Try
				lv.svalue = ___value
				Return lv
			End Function

			Friend Overrides Function parseHtmlValue(ByVal ___value As String) As Object
				If ___value.Equals(HTML.NULL_ATTRIBUTE_VALUE) Then ___value = "1"
				Return parseCssValue(___value)
			End Function
			''' <summary>
			''' Converts a <code>StyleConstants</code> attribute value to
			''' a CSS attribute value.  If there is no conversion,
			''' returns <code>null</code>.  By default, there is no conversion.
			''' </summary>
			''' <param name="key"> the <code>StyleConstants</code> attribute </param>
			''' <param name="value"> the value of a <code>StyleConstants</code>
			'''   attribute to be converted </param>
			''' <returns> the CSS value that represents the
			'''   <code>StyleConstants</code> value </returns>
			Friend Overrides Function fromStyleConstants(ByVal key As StyleConstants, ByVal ___value As Object) As Object
				Dim v As New LengthValue
				v.svalue = ___value.ToString()
				v.span = CSng(___value)
				Return v
			End Function

			''' <summary>
			''' Converts a CSS attribute value to a <code>StyleConstants</code>
			''' value.  If there is no conversion, returns <code>null</code>.
			''' By default, there is no conversion.
			''' </summary>
			''' <param name="key"> the <code>StyleConstants</code> attribute </param>
			''' <returns> the <code>StyleConstants</code> attribute value that
			'''   represents the CSS attribute value </returns>
			Friend Overrides Function toStyleConstants(ByVal key As StyleConstants, ByVal v As View) As Object
				Return New Single?(getValue(False))
			End Function

			''' <summary>
			''' If true, span is a percentage value, and that to determine
			''' the length another value needs to be passed in. 
			''' </summary>
			Friend percentage As Boolean
			''' <summary>
			''' Either the absolute value (percentage == false) or
			''' a percentage value. 
			''' </summary>
			Friend span As Single

			Friend units As String = Nothing
		End Class


		''' <summary>
		''' BorderWidthValue is used to model BORDER_XXX_WIDTH and adds support
		''' for the thin/medium/thick values.
		''' </summary>
		Friend Class BorderWidthValue
			Inherits LengthValue

			Friend Sub New(ByVal svalue As String, ByVal index As Integer)
				Me.svalue = svalue
				span = values(index)
				percentage = False
			End Sub

			Friend Overrides Function parseCssValue(ByVal ___value As String) As Object
				If ___value IsNot Nothing Then
					If ___value.Equals("thick") Then
						Return New BorderWidthValue(___value, 2)
					ElseIf ___value.Equals("medium") Then
						Return New BorderWidthValue(___value, 1)
					ElseIf ___value.Equals("thin") Then
						Return New BorderWidthValue(___value, 0)
					End If
				End If
				' Assume its a length.
				Return MyBase.parseCssValue(___value)
			End Function

			Friend Overrides Function parseHtmlValue(ByVal ___value As String) As Object
				If ___value = HTML.NULL_ATTRIBUTE_VALUE Then Return parseCssValue("medium")
				Return parseCssValue(___value)
			End Function

			''' <summary>
			''' Values used to represent border width. </summary>
			Private Shared ReadOnly values As Single() = { 1, 2, 4 }
		End Class


		''' <summary>
		''' Handles uniquing of CSS values, like lists, and background image
		''' repeating.
		''' </summary>
		Friend Class CssValueMapper
			Inherits CssValue

			Friend Overrides Function parseCssValue(ByVal ___value As String) As Object
				Dim retValue As Object = cssValueToInternalValueMap(___value)
				If retValue Is Nothing Then retValue = cssValueToInternalValueMap(___value.ToLower())
				Return retValue
			End Function


			Friend Overrides Function parseHtmlValue(ByVal ___value As String) As Object
				Dim retValue As Object = htmlValueToCssValueMap(___value)
				If retValue Is Nothing Then retValue = htmlValueToCssValueMap(___value.ToLower())
				Return retValue
			End Function
		End Class


		''' <summary>
		''' Used for background images, to represent the position.
		''' </summary>
		Friend Class BackgroundPosition
			Inherits CssValue

			Friend horizontalPosition As Single
			Friend verticalPosition As Single
			' bitmask: bit 0, horizontal relative, bit 1 horizontal relative to
			' font size, 2 vertical relative to size, 3 vertical relative to
			' font size.
			'
			Friend relative As Short

			Friend Overrides Function parseCssValue(ByVal ___value As String) As Object
				' 'top left' and 'left top' both mean the same as '0% 0%'.
				' 'top', 'top center' and 'center top' mean the same as '50% 0%'.
				' 'right top' and 'top right' mean the same as '100% 0%'.
				' 'left', 'left center' and 'center left' mean the same as
				'        '0% 50%'.
				' 'center' and 'center center' mean the same as '50% 50%'.
				' 'right', 'right center' and 'center right' mean the same as
				'        '100% 50%'.
				' 'bottom left' and 'left bottom' mean the same as '0% 100%'.
				' 'bottom', 'bottom center' and 'center bottom' mean the same as
				'        '50% 100%'.
				' 'bottom right' and 'right bottom' mean the same as '100% 100%'.
				Dim strings As String() = CSS.parseStrings(___value)
				Dim count As Integer = strings.Length
				Dim bp As New BackgroundPosition
				bp.relative = 5
				bp.svalue = ___value

				If count > 0 Then
					' bit 0 for vert, 1 hor, 2 for center
					Dim found As Short = 0
					Dim index As Integer = 0
					Do While index < count
						' First, check for keywords
						Dim [string] As String = strings(index)
						index += 1
						If [string].Equals("center") Then
							found = found Or 4
							Continue Do
						Else
							If (found And 1) = 0 Then
								If [string].Equals("top") Then
									found = found Or 1
								ElseIf [string].Equals("bottom") Then
									found = found Or 1
									bp.verticalPosition = 1
									Continue Do
								End If
							End If
							If (found And 2) = 0 Then
								If [string].Equals("left") Then
									found = found Or 2
									bp.horizontalPosition = 0
								ElseIf [string].Equals("right") Then
									found = found Or 2
									bp.horizontalPosition = 1
								End If
							End If
						End If
					Loop
					If found <> 0 Then
						If (found And 1) = 1 Then
							If (found And 2) = 0 Then bp.horizontalPosition =.5f
						ElseIf (found And 2) = 2 Then
							' horiz and no vert.
							bp.verticalPosition =.5f
						Else
							' no horiz, no vert, but center
								bp.verticalPosition =.5f
								bp.horizontalPosition = bp.verticalPosition
						End If
					Else
						' Assume lengths
						Dim lu As New LengthUnit(strings(0), CShort(0), 0f)

						If lu.type = 0 Then
							bp.horizontalPosition = lu.___value
							bp.relative = CShort(Fix(1 Xor bp.relative))
						ElseIf lu.type = 1 Then
							bp.horizontalPosition = lu.___value
						ElseIf lu.type = 3 Then
							bp.horizontalPosition = lu.___value
							bp.relative = CShort(Fix((1 Xor bp.relative) Or 2))
						End If
						If count > 1 Then
							lu = New LengthUnit(strings(1), CShort(0), 0f)

							If lu.type = 0 Then
								bp.verticalPosition = lu.___value
								bp.relative = CShort(Fix(4 Xor bp.relative))
							ElseIf lu.type = 1 Then
								bp.verticalPosition = lu.___value
							ElseIf lu.type = 3 Then
								bp.verticalPosition = lu.___value
								bp.relative = CShort(Fix((4 Xor bp.relative) Or 8))
							End If
						Else
							bp.verticalPosition =.5f
						End If
					End If
				End If
				Return bp
			End Function

			Friend Overridable Property horizontalPositionRelativeToSize As Boolean
				Get
					Return ((relative And 1) = 1)
				End Get
			End Property

			Friend Overridable Property horizontalPositionRelativeToFontSize As Boolean
				Get
					Return ((relative And 2) = 2)
				End Get
			End Property

			Friend Overridable Property horizontalPosition As Single
				Get
					Return horizontalPosition
				End Get
			End Property

			Friend Overridable Property verticalPositionRelativeToSize As Boolean
				Get
					Return ((relative And 4) = 4)
				End Get
			End Property

			Friend Overridable Property verticalPositionRelativeToFontSize As Boolean
				Get
					Return ((relative And 8) = 8)
				End Get
			End Property

			Friend Overridable Property verticalPosition As Single
				Get
					Return verticalPosition
				End Get
			End Property
		End Class


		''' <summary>
		''' Used for BackgroundImages.
		''' </summary>
		Friend Class BackgroundImage
			Inherits CssValue

			Private loadedImage As Boolean
			Private image As javax.swing.ImageIcon

			Friend Overrides Function parseCssValue(ByVal ___value As String) As Object
				Dim retValue As New BackgroundImage
				retValue.svalue = ___value
				Return retValue
			End Function

			Friend Overrides Function parseHtmlValue(ByVal ___value As String) As Object
				Return parseCssValue(___value)
			End Function

			' PENDING: this base is wrong for linked style sheets.
			Friend Overridable Function getImage(ByVal base As java.net.URL) As javax.swing.ImageIcon
				If Not loadedImage Then
					SyncLock Me
						If Not loadedImage Then
							Dim url As java.net.URL = CSS.getURL(base, svalue)
							loadedImage = True
							If url IsNot Nothing Then
								image = New javax.swing.ImageIcon
								Dim tmpImg As java.awt.Image = java.awt.Toolkit.defaultToolkit.createImage(url)
								If tmpImg IsNot Nothing Then image.image = tmpImg
							End If
						End If
					End SyncLock
				End If
				Return image
			End Function
		End Class

		''' <summary>
		''' Parses a length value, this is used internally, and never added
		''' to an AttributeSet or returned to the developer.
		''' </summary>
		<Serializable> _
		Friend Class LengthUnit
			Friend Shared lengthMapping As New Dictionary(Of String, Single?)(6)
			Friend Shared w3cLengthMapping As New Dictionary(Of String, Single?)(6)

			Friend Sub New(ByVal ___value As String, ByVal defaultType As Short, ByVal defaultValue As Single)
				parse(___value, defaultType, defaultValue)
			End Sub

			Friend Overridable Sub parse(ByVal ___value As String, ByVal defaultType As Short, ByVal defaultValue As Single)
				type = defaultType
				Me.___value = defaultValue

				Dim length As Integer = ___value.Length
				If length > 0 AndAlso ___value.Chars(length - 1) = "%"c Then
					Try
						Me.___value = Convert.ToSingle(___value.Substring(0, length - 1)) / 100.0f
						type = 1
					Catch nfe As NumberFormatException
					End Try
				End If
				If length >= 2 Then
					units = ___value.Substring(length - 2, length - (length - 2))
					Dim scale As Single? = lengthMapping(units)
					If scale IsNot Nothing Then
						Try
							Me.___value = Convert.ToSingle(___value.Substring(0, length - 2))
							type = 0
						Catch nfe As NumberFormatException
						End Try
					ElseIf units.Equals("em") OrElse units.Equals("ex") Then
						Try
							Me.___value = Convert.ToSingle(___value.Substring(0, length - 2))
							type = 3
						Catch nfe As NumberFormatException
						End Try
					ElseIf ___value.Equals("larger") Then
						Me.___value = 2f
						type = 2
					ElseIf ___value.Equals("smaller") Then
						Me.___value = -2
						type = 2
					Else
						' treat like points.
						Try
							Me.___value = Convert.ToSingle(___value)
							type = 0
						Catch nfe As NumberFormatException
						End Try
					End If
				ElseIf length > 0 Then
					' treat like points.
					Try
						Me.___value = Convert.ToSingle(___value)
						type = 0
					Catch nfe As NumberFormatException
					End Try
				End If
			End Sub

			Friend Overridable Function getValue(ByVal w3cLengthUnits As Boolean) As Single
				Dim mapping As Dictionary(Of String, Single?) = If(w3cLengthUnits, w3cLengthMapping, lengthMapping)
				Dim scale As Single = 1
				If units IsNot Nothing Then
					Dim scaleFloat As Single? = mapping(units)
					If scaleFloat IsNot Nothing Then scale = scaleFloat
				End If
				Return Me.___value * scale

			End Function

			Friend Shared Function getValue(ByVal ___value As Single, ByVal units As String, ByVal w3cLengthUnits As Boolean?) As Single
				Dim mapping As Dictionary(Of String, Single?) = If(w3cLengthUnits, w3cLengthMapping, lengthMapping)
				Dim scale As Single = 1
				If units IsNot Nothing Then
					Dim scaleFloat As Single? = mapping(units)
					If scaleFloat IsNot Nothing Then scale = scaleFloat
				End If
				Return ___value * scale
			End Function

			Public Overrides Function ToString() As String
				Return type & " " & ___value
			End Function

			' 0 - value indicates real value
			' 1 - % value, value relative to depends upon key.
			'     50% will have a value = .5
			' 2 - add value to parent value.
			' 3 - em/ex relative to font size of element (except for
			'     font-size, which is relative to parent).
			Friend type As Short
			Friend ___value As Single
			Friend units As String = Nothing


			Friend Shared ReadOnly UNINITALIZED_LENGTH As Short = CShort(10)
		End Class


		''' <summary>
		''' Class used to parse font property. The font property is shorthand
		''' for the other font properties. This expands the properties, placing
		''' them in the attributeset.
		''' </summary>
		Friend Class ShorthandFontParser
			''' <summary>
			''' Parses the shorthand font string <code>value</code>, placing the
			''' result in <code>attr</code>.
			''' </summary>
			Friend Shared Sub parseShorthandFont(ByVal css As CSS, ByVal ___value As String, ByVal attr As MutableAttributeSet)
				' font is of the form:
				' [ <font-style> || <font-variant> || <font-weight> ]? <font-size>
				'   [ / <line-height> ]? <font-family>
				Dim strings As String() = CSS.parseStrings(___value)
				Dim count As Integer = strings.Length
				Dim index As Integer = 0
				' bitmask, 1 for style, 2 for variant, 3 for weight
				Dim found As Short = 0
				Dim maxC As Integer = Math.Min(3, count)

				' Check for font-style font-variant font-weight
				Do While index < maxC
					If (found And 1) = 0 AndAlso isFontStyle(strings(index)) Then
						css.addInternalCSSValue(attr, CSS.Attribute.FONT_STYLE, strings(index))
						index += 1
						found = found Or 1
					ElseIf (found And 2) = 0 AndAlso isFontVariant(strings(index)) Then
						css.addInternalCSSValue(attr, CSS.Attribute.FONT_VARIANT, strings(index))
						index += 1
						found = found Or 2
					ElseIf (found And 4) = 0 AndAlso isFontWeight(strings(index)) Then
						css.addInternalCSSValue(attr, CSS.Attribute.FONT_WEIGHT, strings(index))
						index += 1
						found = found Or 4
					ElseIf strings(index).Equals("normal") Then
						index += 1
					Else
						Exit Do
					End If
				Loop
				If (found And 1) = 0 Then css.addInternalCSSValue(attr, CSS.Attribute.FONT_STYLE, "normal")
				If (found And 2) = 0 Then css.addInternalCSSValue(attr, CSS.Attribute.FONT_VARIANT, "normal")
				If (found And 4) = 0 Then css.addInternalCSSValue(attr, CSS.Attribute.FONT_WEIGHT, "normal")

				' string at index should be the font-size
				If index < count Then
					Dim fontSize As String = strings(index)
					Dim slashIndex As Integer = fontSize.IndexOf("/"c)

					If slashIndex <> -1 Then
						fontSize = fontSize.Substring(0, slashIndex)
						strings(index) = strings(index).Substring(slashIndex)
					Else
						index += 1
					End If
					css.addInternalCSSValue(attr, CSS.Attribute.FONT_SIZE, fontSize)
				Else
					css.addInternalCSSValue(attr, CSS.Attribute.FONT_SIZE, "medium")
				End If

				' Check for line height
				If index < count AndAlso strings(index).StartsWith("/") Then
					Dim lineHeight As String = Nothing
					If strings(index).Equals("/") Then
						index += 1
						If index < count Then
							lineHeight = strings(index)
							index += 1
						End If
					Else
						lineHeight = strings(index).Substring(1)
						index += 1
					End If
					' line height
					If lineHeight IsNot Nothing Then
						css.addInternalCSSValue(attr, CSS.Attribute.LINE_HEIGHT, lineHeight)
					Else
						css.addInternalCSSValue(attr, CSS.Attribute.LINE_HEIGHT, "normal")
					End If
				Else
					css.addInternalCSSValue(attr, CSS.Attribute.LINE_HEIGHT, "normal")
				End If

				' remainder of strings are font-family
				If index < count Then
					Dim family As String = strings(index)
					index += 1

					Do While index < count
						family &= " " & strings(index)
						index += 1
					Loop
					css.addInternalCSSValue(attr, CSS.Attribute.FONT_FAMILY, family)
				Else
					css.addInternalCSSValue(attr, CSS.Attribute.FONT_FAMILY, java.awt.Font.SANS_SERIF)
				End If
			End Sub

			Private Shared Function isFontStyle(ByVal [string] As String) As Boolean
				Return ([string].Equals("italic") OrElse [string].Equals("oblique"))
			End Function

			Private Shared Function isFontVariant(ByVal [string] As String) As Boolean
				Return ([string].Equals("small-caps"))
			End Function

			Private Shared Function isFontWeight(ByVal [string] As String) As Boolean
				If [string].Equals("bold") OrElse [string].Equals("bolder") OrElse [string].Equals("italic") OrElse [string].Equals("lighter") Then Return True
				' test for 100-900
				Return ([string].Length = 3 AndAlso [string].Chars(0) >= "1"c AndAlso [string].Chars(0) <= "9"c AndAlso [string].Chars(1) = "0"c AndAlso [string].Chars(2) = "0"c)
			End Function

		End Class


		''' <summary>
		''' Parses the background property into its intrinsic values.
		''' </summary>
		Friend Class ShorthandBackgroundParser
			''' <summary>
			''' Parses the shorthand font string <code>value</code>, placing the
			''' result in <code>attr</code>.
			''' </summary>
			Friend Shared Sub parseShorthandBackground(ByVal css As CSS, ByVal ___value As String, ByVal attr As MutableAttributeSet)
				Dim strings As String() = parseStrings(___value)
				Dim count As Integer = strings.Length
				Dim index As Integer = 0
				' bitmask: 0 for image, 1 repeat, 2 attachment, 3 position,
				'          4 color
				Dim found As Short = 0

				Do While index < count
					Dim [string] As String = strings(index)
					index += 1
					If (found And 1) = 0 AndAlso isImage([string]) Then
						css.addInternalCSSValue(attr, CSS.Attribute.BACKGROUND_IMAGE, [string])
						found = found Or 1
					ElseIf (found And 2) = 0 AndAlso isRepeat([string]) Then
						css.addInternalCSSValue(attr, CSS.Attribute.BACKGROUND_REPEAT, [string])
						found = found Or 2
					ElseIf (found And 4) = 0 AndAlso isAttachment([string]) Then
						css.addInternalCSSValue(attr, CSS.Attribute.BACKGROUND_ATTACHMENT, [string])
						found = found Or 4
					ElseIf (found And 8) = 0 AndAlso isPosition([string]) Then
						If index < count AndAlso isPosition(strings(index)) Then
							css.addInternalCSSValue(attr, CSS.Attribute.BACKGROUND_POSITION, [string] & " " & strings(index))
							index += 1
						Else
							css.addInternalCSSValue(attr, CSS.Attribute.BACKGROUND_POSITION, [string])
						End If
						found = found Or 8
					ElseIf (found And 16) = 0 AndAlso isColor([string]) Then
						css.addInternalCSSValue(attr, CSS.Attribute.BACKGROUND_COLOR, [string])
						found = found Or 16
					End If
				Loop
				If (found And 1) = 0 Then css.addInternalCSSValue(attr, CSS.Attribute.BACKGROUND_IMAGE, Nothing)
				If (found And 2) = 0 Then css.addInternalCSSValue(attr, CSS.Attribute.BACKGROUND_REPEAT, "repeat")
				If (found And 4) = 0 Then css.addInternalCSSValue(attr, CSS.Attribute.BACKGROUND_ATTACHMENT, "scroll")
				If (found And 8) = 0 Then css.addInternalCSSValue(attr, CSS.Attribute.BACKGROUND_POSITION, Nothing)
				' Currently, there is no good way to express this.
	'            
	'            if ((found & 16) == 0) {
	'                css.addInternalCSSValue(attr, CSS.Attribute.BACKGROUND_COLOR,
	'                                        null);
	'            }
	'            
			End Sub

			Friend Shared Function isImage(ByVal [string] As String) As Boolean
				Return ([string].StartsWith("url(") AndAlso [string].EndsWith(")"))
			End Function

			Friend Shared Function isRepeat(ByVal [string] As String) As Boolean
				Return ([string].Equals("repeat-x") OrElse [string].Equals("repeat-y") OrElse [string].Equals("repeat") OrElse [string].Equals("no-repeat"))
			End Function

			Friend Shared Function isAttachment(ByVal [string] As String) As Boolean
				Return ([string].Equals("fixed") OrElse [string].Equals("scroll"))
			End Function

			Friend Shared Function isPosition(ByVal [string] As String) As Boolean
				Return ([string].Equals("top") OrElse [string].Equals("bottom") OrElse [string].Equals("left") OrElse [string].Equals("right") OrElse [string].Equals("center") OrElse ([string].Length > 0 AndAlso Char.IsDigit([string].Chars(0))))
			End Function

			Friend Shared Function isColor(ByVal [string] As String) As Boolean
				Return (CSS.stringToColor([string]) IsNot Nothing)
			End Function
		End Class


		''' <summary>
		''' Used to parser margin and padding.
		''' </summary>
		Friend Class ShorthandMarginParser
			''' <summary>
			''' Parses the shorthand margin/padding/border string
			''' <code>value</code>, placing the result in <code>attr</code>.
			''' <code>names</code> give the 4 instrinsic property names.
			''' </summary>
			Friend Shared Sub parseShorthandMargin(ByVal css As CSS, ByVal ___value As String, ByVal attr As MutableAttributeSet, ByVal names As CSS.Attribute())
				Dim strings As String() = parseStrings(___value)
				Dim count As Integer = strings.Length
				Dim index As Integer = 0
				Select Case count
				Case 0
					' empty string
					Return
				Case 1
					' Identifies all values.
					For counter As Integer = 0 To 3
						css.addInternalCSSValue(attr, names(counter), strings(0))
					Next counter
				Case 2
					' 0 & 2 = strings[0], 1 & 3 = strings[1]
					css.addInternalCSSValue(attr, names(0), strings(0))
					css.addInternalCSSValue(attr, names(2), strings(0))
					css.addInternalCSSValue(attr, names(1), strings(1))
					css.addInternalCSSValue(attr, names(3), strings(1))
				Case 3
					css.addInternalCSSValue(attr, names(0), strings(0))
					css.addInternalCSSValue(attr, names(1), strings(1))
					css.addInternalCSSValue(attr, names(2), strings(2))
					css.addInternalCSSValue(attr, names(3), strings(1))
				Case Else
					For counter As Integer = 0 To 3
						css.addInternalCSSValue(attr, names(counter), strings(counter))
					Next counter
				End Select
			End Sub
		End Class

		Friend Class ShorthandBorderParser
			Friend Shared keys As Attribute() = { Attribute.BORDER_TOP, Attribute.BORDER_RIGHT, Attribute.BORDER_BOTTOM, Attribute.BORDER_LEFT }

			Friend Shared Sub parseShorthandBorder(ByVal attributes As MutableAttributeSet, ByVal key As CSS.Attribute, ByVal ___value As String)
				Dim parts As Object() = New Object(CSSBorder.PARSERS.Length - 1){}
				Dim strings As String() = parseStrings(___value)
				For Each s As String In strings
					Dim valid As Boolean = False
					For i As Integer = 0 To parts.Length - 1
						Dim v As Object = CSSBorder.PARSERS(i).parseCssValue(s)
						If v IsNot Nothing Then
							If parts(i) Is Nothing Then
								parts(i) = v
								valid = True
							End If
							Exit For
						End If
					Next i
					If Not valid Then Return
				Next s

				' Unspecified parts get default values.
				For i As Integer = 0 To parts.Length - 1
					If parts(i) Is Nothing Then parts(i) = CSSBorder.DEFAULTS(i)
				Next i

				' Dispatch collected values to individual properties.
				For i As Integer = 0 To keys.Length - 1
					If (key Is Attribute.BORDER) OrElse (key Is keys(i)) Then
						For k As Integer = 0 To parts.Length - 1
							attributes.addAttribute(CSSBorder.ATTRIBUTES(k)(i), parts(k))
						Next k
					End If
				Next i
			End Sub
		End Class

		''' <summary>
		''' Calculate the requirements needed to tile the requirements
		''' given by the iterator that would be tiled.  The calculation
		''' takes into consideration margin and border spacing.
		''' </summary>
		Friend Shared Function calculateTiledRequirements(ByVal iter As LayoutIterator, ByVal r As javax.swing.SizeRequirements) As javax.swing.SizeRequirements
			Dim minimum As Long = 0
			Dim maximum As Long = 0
			Dim preferred As Long = 0
			Dim lastMargin As Integer = 0
			Dim totalSpacing As Integer = 0
			Dim n As Integer = iter.count
			For i As Integer = 0 To n - 1
				iter.index = i
				Dim margin0 As Integer = lastMargin
				Dim margin1 As Integer = CInt(Fix(iter.leadingCollapseSpan))
				totalSpacing += Math.Max(margin0, margin1)
				preferred += CInt(Fix(iter.getPreferredSpan(0)))
				minimum += iter.getMinimumSpan(0)
				maximum += iter.getMaximumSpan(0)

				lastMargin = CInt(Fix(iter.trailingCollapseSpan))
			Next i
			totalSpacing += lastMargin
			totalSpacing += 2 * iter.borderWidth

			' adjust for the spacing area
			minimum += totalSpacing
			preferred += totalSpacing
			maximum += totalSpacing

			' set return value
			If r Is Nothing Then r = New javax.swing.SizeRequirements
			r.minimum = If(minimum > Integer.MaxValue, Integer.MaxValue, CInt(minimum))
			r.preferred = If(preferred > Integer.MaxValue, Integer.MaxValue, CInt(preferred))
			r.maximum = If(maximum > Integer.MaxValue, Integer.MaxValue, CInt(maximum))
			Return r
		End Function

		''' <summary>
		''' Calculate a tiled layout for the given iterator.
		''' This should be done collapsing the neighboring
		''' margins to be a total of the maximum of the two
		''' neighboring margin areas as described in the CSS spec.
		''' </summary>
		Friend Shared Sub calculateTiledLayout(ByVal iter As LayoutIterator, ByVal targetSpan As Integer)

	'        
	'         * first pass, calculate the preferred sizes, adjustments needed because
	'         * of margin collapsing, and the flexibility to adjust the sizes.
	'         
			Dim preferred As Long = 0
			Dim currentPreferred As Long
			Dim lastMargin As Integer = 0
			Dim totalSpacing As Integer = 0
			Dim n As Integer = iter.count
			Dim adjustmentWeightsCount As Integer = LayoutIterator.WorstAdjustmentWeight + 1
			'max gain we can get adjusting elements with adjustmentWeight <= i
			Dim gain As Long() = New Long(adjustmentWeightsCount - 1){}
			'max loss we can get adjusting elements with adjustmentWeight <= i
			Dim loss As Long() = New Long(adjustmentWeightsCount - 1){}

			For i As Integer = 0 To adjustmentWeightsCount - 1
					loss(i) = 0
					gain(i) = loss(i)
			Next i
			For i As Integer = 0 To n - 1
				iter.index = i
				Dim margin0 As Integer = lastMargin
				Dim margin1 As Integer = CInt(Fix(iter.leadingCollapseSpan))

				iter.offset = Math.Max(margin0, margin1)
				totalSpacing += iter.offset

				currentPreferred = CLng(Fix(iter.getPreferredSpan(targetSpan)))
				iter.span = CInt(currentPreferred)
				preferred += currentPreferred
				gain(iter.adjustmentWeight) += CLng(Fix(iter.getMaximumSpan(targetSpan))) - currentPreferred
				loss(iter.adjustmentWeight) += currentPreferred - CLng(Fix(iter.getMinimumSpan(targetSpan)))
				lastMargin = CInt(Fix(iter.trailingCollapseSpan))
			Next i
			totalSpacing += lastMargin
			totalSpacing += 2 * iter.borderWidth

			For i As Integer = 1 To adjustmentWeightsCount - 1
				gain(i) += gain(i - 1)
				loss(i) += loss(i - 1)
			Next i

	'        
	'         * Second pass, expand or contract by as much as possible to reach
	'         * the target span.  This takes the margin collapsing into account
	'         * prior to adjusting the span.
	'         

			' determine the adjustment to be made
			Dim allocated As Integer = targetSpan - totalSpacing
			Dim desiredAdjustment As Long = allocated - preferred
			Dim adjustmentsArray As Long() = If(desiredAdjustment > 0, gain, loss)
			desiredAdjustment = Math.Abs(desiredAdjustment)
			Dim adjustmentLevel As Integer = 0
			Do While adjustmentLevel <= LayoutIterator.WorstAdjustmentWeight
				' adjustmentsArray[] is sorted. I do not bother about
				' binary search though
				If adjustmentsArray(adjustmentLevel) >= desiredAdjustment Then Exit Do
				adjustmentLevel += 1
			Loop
			Dim adjustmentFactor As Single = 0.0f
			If adjustmentLevel <= LayoutIterator.WorstAdjustmentWeight Then
				desiredAdjustment -= If(adjustmentLevel > 0, adjustmentsArray(adjustmentLevel - 1), 0)
				If desiredAdjustment <> 0 Then
					Dim maximumAdjustment As Single = adjustmentsArray(adjustmentLevel) - (If(adjustmentLevel > 0, adjustmentsArray(adjustmentLevel - 1), 0))
					adjustmentFactor = desiredAdjustment / maximumAdjustment
				End If
			End If
			' make the adjustments
			Dim totalOffset As Integer = CInt(Fix(iter.borderWidth))
			For i As Integer = 0 To n - 1
				iter.index = i
				iter.offset = iter.offset + totalOffset
				If iter.adjustmentWeight < adjustmentLevel Then
					iter.span = CInt(Fix(If(allocated > preferred, Math.Floor(iter.getMaximumSpan(targetSpan)), Math.Ceiling(iter.getMinimumSpan(targetSpan)))))
				ElseIf iter.adjustmentWeight = adjustmentLevel Then
					Dim availableSpan As Integer = If(allocated > preferred, CInt(Fix(iter.getMaximumSpan(targetSpan))) - iter.span, iter.span - CInt(Fix(iter.getMinimumSpan(targetSpan))))
					Dim adj As Integer = CInt(Fix(Math.Floor(adjustmentFactor * availableSpan)))
					iter.span = iter.span + (If(allocated > preferred, adj, -adj))
				End If
				totalOffset = CInt(Fix(Math.Min(CLng(iter.offset) + CLng(iter.span), Integer.MaxValue)))
			Next i

			' while rounding we could lose several pixels.
			Dim roundError As Integer = targetSpan - totalOffset - CInt(Fix(iter.trailingCollapseSpan)) - CInt(Fix(iter.borderWidth))
			Dim adj As Integer = If(roundError > 0, 1, -1)
			roundError *= adj

			Dim canAdjust As Boolean = True
			Do While roundError > 0 AndAlso canAdjust
				' check for infinite loop
				canAdjust = False
				Dim offsetAdjust As Integer = 0
				' try to distribute roundError. one pixel per cell
				For i As Integer = 0 To n - 1
					iter.index = i
					iter.offset = iter.offset + offsetAdjust
					Dim curSpan As Integer = iter.span
					If roundError > 0 Then
						Dim boundGap As Integer = If(adj > 0, CInt(Fix(Math.Floor(iter.getMaximumSpan(targetSpan)))) - curSpan, curSpan - CInt(Fix(Math.Ceiling(iter.getMinimumSpan(targetSpan)))))
						If boundGap >= 1 Then
							canAdjust = True
							iter.span = curSpan + adj
							offsetAdjust += adj
							roundError -= 1
						End If
					End If
				Next i
			Loop
		End Sub

		''' <summary>
		''' An iterator to express the requirements to use when computing
		''' layout.
		''' </summary>
		Friend Interface LayoutIterator

			Property offset As Integer


			Property span As Integer


			ReadOnly Property count As Integer

			WriteOnly Property index As Integer

			Function getMinimumSpan(ByVal parentSpan As Single) As Single

			Function getPreferredSpan(ByVal parentSpan As Single) As Single

			Function getMaximumSpan(ByVal parentSpan As Single) As Single

			ReadOnly Property adjustmentWeight As Integer

			'float getAlignment();

			ReadOnly Property borderWidth As Single

			ReadOnly Property leadingCollapseSpan As Single

			ReadOnly Property trailingCollapseSpan As Single
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'			public static final int WorstAdjustmentWeight = 2;
		End Interface

		'
		' Serialization support
		'

		Private Sub writeObject(ByVal s As java.io.ObjectOutputStream)
			s.defaultWriteObject()

			' Determine what values in valueConvertor need to be written out.
			Dim keys As System.Collections.IEnumerator = valueConvertor.Keys.GetEnumerator()
			s.writeInt(valueConvertor.Count)
			If keys IsNot Nothing Then
				Do While keys.hasMoreElements()
					Dim key As Object = keys.nextElement()
					Dim ___value As Object = valueConvertor(key)
					key = StyleContext.getStaticAttributeKey(key)
					If Not(TypeOf key Is Serializable) AndAlso key Is Nothing Then
						' Should we throw an exception here?
						key = Nothing
						___value = Nothing
					Else
						___value = StyleContext.getStaticAttributeKey(___value)
						If Not(TypeOf ___value Is Serializable) AndAlso ___value Is Nothing Then
							' Should we throw an exception here?
							key = Nothing
							___value = Nothing
						End If
						End If
					s.writeObject(key)
					s.writeObject(___value)
				Loop
			End If
		End Sub

		Private Sub readObject(ByVal s As ObjectInputStream)
			s.defaultReadObject()
			' Reconstruct the hashtable.
			Dim numValues As Integer = s.readInt()
			valueConvertor = New Dictionary(Of Object, Object)(Math.Max(1, numValues))
			Dim tempVar As Boolean = numValues > 0
			numValues -= 1
			Do While tempVar
				Dim key As Object = s.readObject()
				Dim ___value As Object = s.readObject()
				Dim staticKey As Object = StyleContext.getStaticAttribute(key)
				If staticKey IsNot Nothing Then key = staticKey
				Dim staticValue As Object = StyleContext.getStaticAttribute(___value)
				If staticValue IsNot Nothing Then ___value = staticValue
				If key IsNot Nothing AndAlso ___value IsNot Nothing Then valueConvertor(key) = ___value
				tempVar = numValues > 0
				numValues -= 1
			Loop
		End Sub


	'    
	'     * we need StyleSheet for resolving lenght units. (see
	'     * isW3CLengthUnits)
	'     * we can not pass stylesheet for handling relative sizes. (do not
	'     * think changing public API is necessary)
	'     * CSS is not likely to be accessed from more then one thread.
	'     * Having local storage for StyleSheet for resolving relative
	'     * sizes is safe
	'     *
	'     * idk 08/30/2004
	'     
		Private Function getStyleSheet(ByVal ss As StyleSheet) As StyleSheet
			If ss IsNot Nothing Then styleSheet = ss
			Return styleSheet
		End Function
		'
		' Instance variables
		'

		''' <summary>
		''' Maps from CSS key to CssValue. </summary>
		<NonSerialized> _
		Private valueConvertor As Dictionary(Of Object, Object)

		''' <summary>
		''' Size used for relative units. </summary>
		Private baseFontSize As Integer

		<NonSerialized> _
		Private styleSheet As StyleSheet = Nothing

		Friend Shared baseFontSizeIndex As Integer = 3
	End Class

End Namespace
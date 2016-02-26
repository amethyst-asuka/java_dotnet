Imports Microsoft.VisualBasic
Imports javax.swing

'
' * Copyright (c) 2002, 2008, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.swing.plaf.synth



	''' <summary>
	''' Calculates preferred size and layouts synth menu items.
	''' 
	''' All JMenuItems (and JMenus) include enough space for the insets
	''' plus one or more elements.  When we say "label" below, we mean
	''' "icon and/or text."
	''' 
	''' Cases to consider for SynthMenuItemUI (visualized here in a
	''' LTR orientation; the RTL case would be reversed):
	'''                   label
	'''      check icon + label
	'''      check icon + label + accelerator
	'''                   label + accelerator
	''' 
	''' Cases to consider for SynthMenuUI (again visualized here in a
	''' LTR orientation):
	'''                   label + arrow
	''' 
	''' Note that in the above scenarios, accelerator and arrow icon are
	''' mutually exclusive.  This means that if a popup menu contains a mix
	''' of JMenus and JMenuItems, we only need to allow enough space for
	''' max(maxAccelerator, maxArrow), and both accelerators and arrow icons
	''' can occupy the same "column" of space in the menu.
	''' </summary>
	Friend Class SynthMenuItemLayoutHelper
		Inherits sun.swing.MenuItemLayoutHelper

		Public Shared ReadOnly MAX_ACC_OR_ARROW_WIDTH As New sun.swing.StringUIClientPropertyKey("maxAccOrArrowWidth")

		Public Shared ReadOnly LTR_ALIGNMENT_1 As New ColumnAlignment(SwingConstants.LEFT, SwingConstants.LEFT, SwingConstants.LEFT, SwingConstants.RIGHT, SwingConstants.RIGHT)
		Public Shared ReadOnly LTR_ALIGNMENT_2 As New ColumnAlignment(SwingConstants.LEFT, SwingConstants.LEFT, SwingConstants.LEFT, SwingConstants.LEFT, SwingConstants.RIGHT)
		Public Shared ReadOnly RTL_ALIGNMENT_1 As New ColumnAlignment(SwingConstants.RIGHT, SwingConstants.RIGHT, SwingConstants.RIGHT, SwingConstants.LEFT, SwingConstants.LEFT)
		Public Shared ReadOnly RTL_ALIGNMENT_2 As New ColumnAlignment(SwingConstants.RIGHT, SwingConstants.RIGHT, SwingConstants.RIGHT, SwingConstants.RIGHT, SwingConstants.LEFT)

		Private context As SynthContext
		Private accContext As SynthContext
		Private style As SynthStyle
		Private accStyle As SynthStyle
		Private gu As SynthGraphicsUtils
		Private accGu As SynthGraphicsUtils
		Private ___alignAcceleratorText As Boolean
		Private maxAccOrArrowWidth As Integer

		Public Sub New(ByVal context As SynthContext, ByVal accContext As SynthContext, ByVal mi As JMenuItem, ByVal checkIcon As Icon, ByVal arrowIcon As Icon, ByVal viewRect As Rectangle, ByVal gap As Integer, ByVal accDelimiter As String, ByVal isLeftToRight As Boolean, ByVal useCheckAndArrow As Boolean, ByVal propertyPrefix As String)
			Me.context = context
			Me.accContext = accContext
			Me.style = context.style
			Me.accStyle = accContext.style
			Me.gu = style.getGraphicsUtils(context)
			Me.accGu = accStyle.getGraphicsUtils(accContext)
			Me.___alignAcceleratorText = getAlignAcceleratorText(propertyPrefix)
			reset(mi, checkIcon, arrowIcon, viewRect, gap, accDelimiter, isLeftToRight, style.getFont(context), accStyle.getFont(accContext), useCheckAndArrow, propertyPrefix)
			leadingGap = 0
		End Sub

		Private Function getAlignAcceleratorText(ByVal propertyPrefix As String) As Boolean
			Return style.getBoolean(context, propertyPrefix & ".alignAcceleratorText", True)
		End Function

		Protected Friend Overridable Sub calcWidthsAndHeights()
			' iconRect
			If icon IsNot Nothing Then
				iconSize.width = sun.swing.plaf.synth.SynthIcon.getIconWidth(icon, context)
				iconSize.height = sun.swing.plaf.synth.SynthIcon.getIconHeight(icon, context)
			End If

			' accRect
			If Not accText.Equals("") Then
				 accSize.width = accGu.computeStringWidth(accContext, accFontMetrics.font, accFontMetrics, accText)
				accSize.height = accFontMetrics.height
			End If

			' textRect
			If text Is Nothing Then
				text = ""
			ElseIf Not text.Equals("") Then
				If htmlView IsNot Nothing Then
					' Text is HTML
					textSize.width = CInt(Fix(htmlView.getPreferredSpan(javax.swing.text.View.X_AXIS)))
					textSize.height = CInt(Fix(htmlView.getPreferredSpan(javax.swing.text.View.Y_AXIS)))
				Else
					' Text isn't HTML
					textSize.width = gu.computeStringWidth(context, fontMetrics.font, fontMetrics, text)
					textSize.height = fontMetrics.height
				End If
			End If

			If useCheckAndArrow() Then
				' checkIcon
				If checkIcon IsNot Nothing Then
					checkSize.width = sun.swing.plaf.synth.SynthIcon.getIconWidth(checkIcon, context)
					checkSize.height = sun.swing.plaf.synth.SynthIcon.getIconHeight(checkIcon, context)
				End If
				' arrowRect
				If arrowIcon IsNot Nothing Then
					arrowSize.width = sun.swing.plaf.synth.SynthIcon.getIconWidth(arrowIcon, context)
					arrowSize.height = sun.swing.plaf.synth.SynthIcon.getIconHeight(arrowIcon, context)
				End If
			End If

			' labelRect
			If columnLayout Then
				labelSize.width = iconSize.width + textSize.width + gap
				labelSize.height = sun.swing.MenuItemLayoutHelper.max(checkSize.height, iconSize.height, textSize.height, accSize.height, arrowSize.height)
			Else
				Dim textRect As New Rectangle
				Dim iconRect As New Rectangle
				gu.layoutText(context, fontMetrics, text, icon, horizontalAlignment, verticalAlignment, horizontalTextPosition, verticalTextPosition, viewRect, iconRect, textRect, gap)
				textRect.width += leftTextExtraWidth
				Dim labelRect As Rectangle = iconRect.union(textRect)
				labelSize.height = labelRect.height
				labelSize.width = labelRect.width
			End If
		End Sub

		Protected Friend Overridable Sub calcMaxWidths()
			calcMaxWidth(checkSize, MAX_CHECK_WIDTH)
			maxAccOrArrowWidth = calcMaxValue(MAX_ACC_OR_ARROW_WIDTH, arrowSize.width)
			maxAccOrArrowWidth = calcMaxValue(MAX_ACC_OR_ARROW_WIDTH, accSize.width)

			If columnLayout Then
				calcMaxWidth(iconSize, MAX_ICON_WIDTH)
				calcMaxWidth(textSize, MAX_TEXT_WIDTH)
				Dim curGap As Integer = gap
				If (iconSize.maxWidth = 0) OrElse (textSize.maxWidth = 0) Then curGap = 0
				labelSize.maxWidth = calcMaxValue(MAX_LABEL_WIDTH, iconSize.maxWidth + textSize.maxWidth + curGap)
			Else
				' We shouldn't use current icon and text widths
				' in maximal widths calculation for complex layout.
				iconSize.maxWidth = getParentIntProperty(MAX_ICON_WIDTH)
				calcMaxWidth(labelSize, MAX_LABEL_WIDTH)
				' If maxLabelWidth is wider
				' than the widest icon + the widest text + gap,
				' we should update the maximal text witdh
				Dim candidateTextWidth As Integer = labelSize.maxWidth - iconSize.maxWidth
				If iconSize.maxWidth > 0 Then candidateTextWidth -= gap
				textSize.maxWidth = calcMaxValue(MAX_TEXT_WIDTH, candidateTextWidth)
			End If
		End Sub

		Public Overridable Property context As SynthContext
			Get
				Return context
			End Get
		End Property

		Public Overridable Property accContext As SynthContext
			Get
				Return accContext
			End Get
		End Property

		Public Overridable Property style As SynthStyle
			Get
				Return style
			End Get
		End Property

		Public Overridable Property accStyle As SynthStyle
			Get
				Return accStyle
			End Get
		End Property

		Public Overridable Property graphicsUtils As SynthGraphicsUtils
			Get
				Return gu
			End Get
		End Property

		Public Overridable Property accGraphicsUtils As SynthGraphicsUtils
			Get
				Return accGu
			End Get
		End Property

		Public Overridable Function alignAcceleratorText() As Boolean
			Return ___alignAcceleratorText
		End Function

		Public Overridable Property maxAccOrArrowWidth As Integer
			Get
				Return maxAccOrArrowWidth
			End Get
		End Property

		Protected Friend Overridable Sub prepareForLayout(ByVal lr As LayoutResult)
			lr.checkRect.width = checkSize.maxWidth
			' An item can have an arrow or a check icon at once
			If useCheckAndArrow() AndAlso ((Not "".Equals(accText))) Then
				lr.accRect.width = maxAccOrArrowWidth
			Else
				lr.arrowRect.width = maxAccOrArrowWidth
			End If
		End Sub

		Public Overridable Property lTRColumnAlignment As ColumnAlignment
			Get
				If alignAcceleratorText() Then
					Return LTR_ALIGNMENT_2
				Else
					Return LTR_ALIGNMENT_1
				End If
			End Get
		End Property

		Public Overridable Property rTLColumnAlignment As ColumnAlignment
			Get
				If alignAcceleratorText() Then
					Return RTL_ALIGNMENT_2
				Else
					Return RTL_ALIGNMENT_1
				End If
			End Get
		End Property

		Protected Friend Overridable Sub layoutIconAndTextInLabelRect(ByVal lr As LayoutResult)
			lr.textRect = New Rectangle
			lr.iconRect = New Rectangle
			gu.layoutText(context, fontMetrics, text, icon, horizontalAlignment, verticalAlignment, horizontalTextPosition, verticalTextPosition, lr.labelRect, lr.iconRect, lr.textRect, gap)
		End Sub
	End Class

End Namespace
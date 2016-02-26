Imports System
Imports System.Collections.Generic
Imports javax.swing
Imports javax.swing.text
Imports sun.swing.plaf.synth

'
' * Copyright (c) 2002, 2011, Oracle and/or its affiliates. All rights reserved.
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
	''' Wrapper for primitive graphics calls.
	''' 
	''' @since 1.5
	''' @author Scott Violet
	''' </summary>
	Public Class SynthGraphicsUtils
		' These are used in the text painting code to avoid allocating a bunch of
		' garbage.
		Private paintIconR As New Rectangle
		Private paintTextR As New Rectangle
		Private paintViewR As New Rectangle
		Private paintInsets As New Insets(0, 0, 0, 0)

		' These Rectangles/Insets are used in the text size calculation to avoid a
		' a bunch of garbage.
		Private iconR As New Rectangle
		Private textR As New Rectangle
		Private viewR As New Rectangle
		Private viewSizingInsets As New Insets(0, 0, 0, 0)

		''' <summary>
		''' Creates a <code>SynthGraphicsUtils</code>.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Draws a line between the two end points.
		''' </summary>
		''' <param name="context"> Identifies hosting region. </param>
		''' <param name="paintKey"> Identifies the portion of the component being asked
		'''                 to paint, may be null. </param>
		''' <param name="g"> Graphics object to paint to </param>
		''' <param name="x1"> x origin </param>
		''' <param name="y1"> y origin </param>
		''' <param name="x2"> x destination </param>
		''' <param name="y2"> y destination </param>
		Public Overridable Sub drawLine(ByVal context As SynthContext, ByVal paintKey As Object, ByVal g As Graphics, ByVal x1 As Integer, ByVal y1 As Integer, ByVal x2 As Integer, ByVal y2 As Integer)
			g.drawLine(x1, y1, x2, y2)
		End Sub

		''' <summary>
		''' Draws a line between the two end points.
		''' <p>This implementation supports only one line style key,
		''' <code>"dashed"</code>. The <code>"dashed"</code> line style is applied
		''' only to vertical and horizontal lines.
		''' <p>Specifying <code>null</code> or any key different from
		''' <code>"dashed"</code> will draw solid lines.
		''' </summary>
		''' <param name="context"> identifies hosting region </param>
		''' <param name="paintKey"> identifies the portion of the component being asked
		'''                 to paint, may be null </param>
		''' <param name="g"> Graphics object to paint to </param>
		''' <param name="x1"> x origin </param>
		''' <param name="y1"> y origin </param>
		''' <param name="x2"> x destination </param>
		''' <param name="y2"> y destination </param>
		''' <param name="styleKey"> identifies the requested style of the line (e.g. "dashed")
		''' @since 1.6 </param>
		Public Overridable Sub drawLine(ByVal context As SynthContext, ByVal paintKey As Object, ByVal g As Graphics, ByVal x1 As Integer, ByVal y1 As Integer, ByVal x2 As Integer, ByVal y2 As Integer, ByVal styleKey As Object)
			If "dashed".Equals(styleKey) Then
				' draw vertical line
				If x1 = x2 Then
					y1 += (y1 Mod 2)

					For y As Integer = y1 To y2 Step 2
						g.drawLine(x1, y, x2, y)
					Next y
				' draw horizontal line
				ElseIf y1 = y2 Then
					x1 += (x1 Mod 2)

					For x As Integer = x1 To x2 Step 2
						g.drawLine(x, y1, x, y2)
					Next x
				' oblique lines are not supported
				End If
			Else
				drawLine(context, paintKey, g, x1, y1, x2, y2)
			End If
		End Sub

		''' <summary>
		''' Lays out text and an icon returning, by reference, the location to
		''' place the icon and text.
		''' </summary>
		''' <param name="ss"> SynthContext </param>
		''' <param name="fm"> FontMetrics for the Font to use, this may be ignored </param>
		''' <param name="text"> Text to layout </param>
		''' <param name="icon"> Icon to layout </param>
		''' <param name="hAlign"> horizontal alignment </param>
		''' <param name="vAlign"> vertical alignment </param>
		''' <param name="hTextPosition"> horizontal text position </param>
		''' <param name="vTextPosition"> vertical text position </param>
		''' <param name="viewR"> Rectangle to layout text and icon in. </param>
		''' <param name="iconR"> Rectangle to place icon bounds in </param>
		''' <param name="textR"> Rectangle to place text in </param>
		''' <param name="iconTextGap"> gap between icon and text </param>
		Public Overridable Function layoutText(ByVal ss As SynthContext, ByVal fm As FontMetrics, ByVal text As String, ByVal icon As Icon, ByVal hAlign As Integer, ByVal vAlign As Integer, ByVal hTextPosition As Integer, ByVal vTextPosition As Integer, ByVal viewR As Rectangle, ByVal iconR As Rectangle, ByVal textR As Rectangle, ByVal iconTextGap As Integer) As String
			If TypeOf icon Is SynthIcon Then
				Dim wrapper As SynthIconWrapper = SynthIconWrapper.get(CType(icon, SynthIcon), ss)
				Dim formattedText As String = SwingUtilities.layoutCompoundLabel(ss.component, fm, text, wrapper, vAlign, hAlign, vTextPosition, hTextPosition, viewR, iconR, textR, iconTextGap)
				SynthIconWrapper.release(wrapper)
				Return formattedText
			End If
			Return SwingUtilities.layoutCompoundLabel(ss.component, fm, text, icon, vAlign, hAlign, vTextPosition, hTextPosition, viewR, iconR, textR, iconTextGap)
		End Function

		''' <summary>
		''' Returns the size of the passed in string.
		''' </summary>
		''' <param name="ss"> SynthContext </param>
		''' <param name="font"> Font to use </param>
		''' <param name="metrics"> FontMetrics, may be ignored </param>
		''' <param name="text"> Text to get size of. </param>
		Public Overridable Function computeStringWidth(ByVal ss As SynthContext, ByVal font As Font, ByVal metrics As FontMetrics, ByVal text As String) As Integer
			Return sun.swing.SwingUtilities2.stringWidth(ss.component, metrics, text)
		End Function

		''' <summary>
		''' Returns the minimum size needed to properly render an icon and text.
		''' </summary>
		''' <param name="ss"> SynthContext </param>
		''' <param name="font"> Font to use </param>
		''' <param name="text"> Text to layout </param>
		''' <param name="icon"> Icon to layout </param>
		''' <param name="hAlign"> horizontal alignment </param>
		''' <param name="vAlign"> vertical alignment </param>
		''' <param name="hTextPosition"> horizontal text position </param>
		''' <param name="vTextPosition"> vertical text position </param>
		''' <param name="iconTextGap"> gap between icon and text </param>
		''' <param name="mnemonicIndex"> Index into text to render the mnemonic at, -1
		'''        indicates no mnemonic. </param>
		Public Overridable Function getMinimumSize(ByVal ss As SynthContext, ByVal font As Font, ByVal text As String, ByVal icon As Icon, ByVal hAlign As Integer, ByVal vAlign As Integer, ByVal hTextPosition As Integer, ByVal vTextPosition As Integer, ByVal iconTextGap As Integer, ByVal mnemonicIndex As Integer) As Dimension
			Dim c As JComponent = ss.component
			Dim size As Dimension = getPreferredSize(ss, font, text, icon, hAlign, vAlign, hTextPosition, vTextPosition, iconTextGap, mnemonicIndex)
			Dim v As View = CType(c.getClientProperty(javax.swing.plaf.basic.BasicHTML.propertyKey), View)

			If v IsNot Nothing Then size.width -= v.getPreferredSpan(View.X_AXIS) - v.getMinimumSpan(View.X_AXIS)
			Return size
		End Function

		''' <summary>
		''' Returns the maximum size needed to properly render an icon and text.
		''' </summary>
		''' <param name="ss"> SynthContext </param>
		''' <param name="font"> Font to use </param>
		''' <param name="text"> Text to layout </param>
		''' <param name="icon"> Icon to layout </param>
		''' <param name="hAlign"> horizontal alignment </param>
		''' <param name="vAlign"> vertical alignment </param>
		''' <param name="hTextPosition"> horizontal text position </param>
		''' <param name="vTextPosition"> vertical text position </param>
		''' <param name="iconTextGap"> gap between icon and text </param>
		''' <param name="mnemonicIndex"> Index into text to render the mnemonic at, -1
		'''        indicates no mnemonic. </param>
		Public Overridable Function getMaximumSize(ByVal ss As SynthContext, ByVal font As Font, ByVal text As String, ByVal icon As Icon, ByVal hAlign As Integer, ByVal vAlign As Integer, ByVal hTextPosition As Integer, ByVal vTextPosition As Integer, ByVal iconTextGap As Integer, ByVal mnemonicIndex As Integer) As Dimension
			Dim c As JComponent = ss.component
			Dim size As Dimension = getPreferredSize(ss, font, text, icon, hAlign, vAlign, hTextPosition, vTextPosition, iconTextGap, mnemonicIndex)
			Dim v As View = CType(c.getClientProperty(javax.swing.plaf.basic.BasicHTML.propertyKey), View)

			If v IsNot Nothing Then size.width += v.getMaximumSpan(View.X_AXIS) - v.getPreferredSpan(View.X_AXIS)
			Return size
		End Function

		''' <summary>
		''' Returns the maximum height of the the Font from the passed in
		''' SynthContext.
		''' </summary>
		''' <param name="context"> SynthContext used to determine font. </param>
		''' <returns> maximum height of the characters for the font from the passed
		'''         in context. </returns>
		Public Overridable Function getMaximumCharHeight(ByVal context As SynthContext) As Integer
			Dim fm As FontMetrics = context.component.getFontMetrics(context.style.getFont(context))
			Return (fm.ascent + fm.descent)
		End Function

		''' <summary>
		''' Returns the preferred size needed to properly render an icon and text.
		''' </summary>
		''' <param name="ss"> SynthContext </param>
		''' <param name="font"> Font to use </param>
		''' <param name="text"> Text to layout </param>
		''' <param name="icon"> Icon to layout </param>
		''' <param name="hAlign"> horizontal alignment </param>
		''' <param name="vAlign"> vertical alignment </param>
		''' <param name="hTextPosition"> horizontal text position </param>
		''' <param name="vTextPosition"> vertical text position </param>
		''' <param name="iconTextGap"> gap between icon and text </param>
		''' <param name="mnemonicIndex"> Index into text to render the mnemonic at, -1
		'''        indicates no mnemonic. </param>
		Public Overridable Function getPreferredSize(ByVal ss As SynthContext, ByVal font As Font, ByVal text As String, ByVal icon As Icon, ByVal hAlign As Integer, ByVal vAlign As Integer, ByVal hTextPosition As Integer, ByVal vTextPosition As Integer, ByVal iconTextGap As Integer, ByVal mnemonicIndex As Integer) As Dimension
			Dim c As JComponent = ss.component
			Dim insets As Insets = c.getInsets(viewSizingInsets)
			Dim dx As Integer = insets.left + insets.right
			Dim dy As Integer = insets.top + insets.bottom

			If icon Is Nothing AndAlso (text Is Nothing OrElse font Is Nothing) Then
				Return New Dimension(dx, dy)
			ElseIf (text Is Nothing) OrElse ((icon IsNot Nothing) AndAlso (font Is Nothing)) Then
				Return New Dimension(SynthIcon.getIconWidth(icon, ss) + dx, SynthIcon.getIconHeight(icon, ss) + dy)
			Else
				Dim fm As FontMetrics = c.getFontMetrics(font)

					iconR.height = 0
						iconR.width = iconR.height
							iconR.y = iconR.width
							iconR.x = iconR.y
					textR.height = 0
						textR.width = textR.height
							textR.y = textR.width
							textR.x = textR.y
				viewR.x = dx
				viewR.y = dy
					viewR.height = Short.MaxValue
					viewR.width = viewR.height

				layoutText(ss, fm, text, icon, hAlign, vAlign, hTextPosition, vTextPosition, viewR, iconR, textR, iconTextGap)
				Dim x1 As Integer = Math.Min(iconR.x, textR.x)
				Dim x2 As Integer = Math.Max(iconR.x + iconR.width, textR.x + textR.width)
				Dim y1 As Integer = Math.Min(iconR.y, textR.y)
				Dim y2 As Integer = Math.Max(iconR.y + iconR.height, textR.y + textR.height)
				Dim rv As New Dimension(x2 - x1, y2 - y1)

				rv.width += dx
				rv.height += dy
				Return rv
			End If
		End Function

		''' <summary>
		''' Paints text at the specified location. This will not attempt to
		''' render the text as html nor will it offset by the insets of the
		''' component.
		''' </summary>
		''' <param name="ss"> SynthContext </param>
		''' <param name="g"> Graphics used to render string in. </param>
		''' <param name="text"> Text to render </param>
		''' <param name="bounds"> Bounds of the text to be drawn. </param>
		''' <param name="mnemonicIndex"> Index to draw string at. </param>
		Public Overridable Sub paintText(ByVal ss As SynthContext, ByVal g As Graphics, ByVal text As String, ByVal bounds As Rectangle, ByVal mnemonicIndex As Integer)
			paintText(ss, g, text, bounds.x, bounds.y, mnemonicIndex)
		End Sub

		''' <summary>
		''' Paints text at the specified location. This will not attempt to
		''' render the text as html nor will it offset by the insets of the
		''' component.
		''' </summary>
		''' <param name="ss"> SynthContext </param>
		''' <param name="g"> Graphics used to render string in. </param>
		''' <param name="text"> Text to render </param>
		''' <param name="x"> X location to draw text at. </param>
		''' <param name="y"> Upper left corner to draw text at. </param>
		''' <param name="mnemonicIndex"> Index to draw string at. </param>
		Public Overridable Sub paintText(ByVal ss As SynthContext, ByVal g As Graphics, ByVal text As String, ByVal x As Integer, ByVal y As Integer, ByVal mnemonicIndex As Integer)
			If text IsNot Nothing Then
				Dim c As JComponent = ss.component
				Dim fm As FontMetrics = sun.swing.SwingUtilities2.getFontMetrics(c, g)
				y += fm.ascent
				sun.swing.SwingUtilities2.drawStringUnderlineCharAt(c, g, text, mnemonicIndex, x, y)
			End If
		End Sub

		''' <summary>
		''' Paints an icon and text. This will render the text as html, if
		''' necessary, and offset the location by the insets of the component.
		''' </summary>
		''' <param name="ss"> SynthContext </param>
		''' <param name="g"> Graphics to render string and icon into </param>
		''' <param name="text"> Text to layout </param>
		''' <param name="icon"> Icon to layout </param>
		''' <param name="hAlign"> horizontal alignment </param>
		''' <param name="vAlign"> vertical alignment </param>
		''' <param name="hTextPosition"> horizontal text position </param>
		''' <param name="vTextPosition"> vertical text position </param>
		''' <param name="iconTextGap"> gap between icon and text </param>
		''' <param name="mnemonicIndex"> Index into text to render the mnemonic at, -1
		'''        indicates no mnemonic. </param>
		''' <param name="textOffset"> Amount to offset the text when painting </param>
		Public Overridable Sub paintText(ByVal ss As SynthContext, ByVal g As Graphics, ByVal text As String, ByVal icon As Icon, ByVal hAlign As Integer, ByVal vAlign As Integer, ByVal hTextPosition As Integer, ByVal vTextPosition As Integer, ByVal iconTextGap As Integer, ByVal mnemonicIndex As Integer, ByVal textOffset As Integer)
			If (icon Is Nothing) AndAlso (text Is Nothing) Then Return
			Dim c As JComponent = ss.component
			Dim fm As FontMetrics = sun.swing.SwingUtilities2.getFontMetrics(c, g)
			Dim insets As Insets = SynthLookAndFeel.getPaintingInsets(ss, paintInsets)

			paintViewR.x = insets.left
			paintViewR.y = insets.top
			paintViewR.width = c.width - (insets.left + insets.right)
			paintViewR.height = c.height - (insets.top + insets.bottom)

				paintIconR.height = 0
					paintIconR.width = paintIconR.height
						paintIconR.y = paintIconR.width
						paintIconR.x = paintIconR.y
				paintTextR.height = 0
					paintTextR.width = paintTextR.height
						paintTextR.y = paintTextR.width
						paintTextR.x = paintTextR.y

			Dim clippedText As String = layoutText(ss, fm, text, icon, hAlign, vAlign, hTextPosition, vTextPosition, paintViewR, paintIconR, paintTextR, iconTextGap)

			If icon IsNot Nothing Then
				Dim color As Color = g.color

				If ss.style.getBoolean(ss, "TableHeader.alignSorterArrow", False) AndAlso "TableHeader.renderer".Equals(c.name) Then
					paintIconR.x = paintViewR.width - paintIconR.width
				Else
					paintIconR.x += textOffset
				End If
				paintIconR.y += textOffset
				SynthIcon.paintIcon(icon, ss, g, paintIconR.x, paintIconR.y, paintIconR.width, paintIconR.height)
				g.color = color
			End If

			If text IsNot Nothing Then
				Dim v As View = CType(c.getClientProperty(javax.swing.plaf.basic.BasicHTML.propertyKey), View)

				If v IsNot Nothing Then
					v.paint(g, paintTextR)
				Else
					paintTextR.x += textOffset
					paintTextR.y += textOffset

					paintText(ss, g, clippedText, paintTextR, mnemonicIndex)
				End If
			End If
		End Sub


		 ''' <summary>
		 ''' A quick note about how preferred sizes are calculated... Generally
		 ''' speaking, SynthPopupMenuUI will run through the list of its children
		 ''' (from top to bottom) and ask each for its preferred size.  Each menu
		 ''' item will add up the max width of each element (icons, text,
		 ''' accelerator spacing, accelerator text or arrow icon) encountered thus
		 ''' far, so by the time all menu items have been calculated, we will
		 ''' know the maximum (preferred) menu item size for that popup menu.
		 ''' Later when it comes time to paint each menu item, we can use those
		 ''' same accumulated max element sizes in order to layout the item.
		 ''' </summary>
		Friend Shared Function getPreferredMenuItemSize(ByVal context As SynthContext, ByVal accContext As SynthContext, ByVal c As JComponent, ByVal checkIcon As Icon, ByVal arrowIcon As Icon, ByVal defaultTextIconGap As Integer, ByVal acceleratorDelimiter As String, ByVal useCheckAndArrow As Boolean, ByVal propertyPrefix As String) As Dimension

			 Dim mi As JMenuItem = CType(c, JMenuItem)
			 Dim lh As New SynthMenuItemLayoutHelper(context, accContext, mi, checkIcon, arrowIcon, sun.swing.MenuItemLayoutHelper.createMaxRect(), defaultTextIconGap, acceleratorDelimiter, SynthLookAndFeel.isLeftToRight(mi), useCheckAndArrow, propertyPrefix)

			 Dim result As New Dimension

			 ' Calculate the result width
			 Dim gap As Integer = lh.gap
			 result.width = 0
			 sun.swing.MenuItemLayoutHelper.addMaxWidth(lh.checkSize, gap, result)
			 sun.swing.MenuItemLayoutHelper.addMaxWidth(lh.labelSize, gap, result)
			 sun.swing.MenuItemLayoutHelper.addWidth(lh.maxAccOrArrowWidth, 5 * gap, result)
			 ' The last gap is unnecessary
			 result.width -= gap

			 ' Calculate the result height
			 result.height = sun.swing.MenuItemLayoutHelper.max(lh.checkSize.height, lh.labelSize.height, lh.accSize.height, lh.arrowSize.height)

			 ' Take into account menu item insets
			 Dim insets As Insets = lh.menuItem.insets
			 If insets IsNot Nothing Then
				 result.width += insets.left + insets.right
				 result.height += insets.top + insets.bottom
			 End If

			 ' if the width is even, bump it up one. This is critical
			 ' for the focus dash lhne to draw properly
			 If result.width Mod 2 = 0 Then result.width += 1

			 ' if the height is even, bump it up one. This is critical
			 ' for the text to center properly
			 If result.height Mod 2 = 0 Then result.height += 1

			 Return result
		End Function

		Friend Shared Sub applyInsets(ByVal rect As Rectangle, ByVal insets As Insets, ByVal leftToRight As Boolean)
			If insets IsNot Nothing Then
				rect.x += (If(leftToRight, insets.left, insets.right))
				rect.y += insets.top
				rect.width -= (If(leftToRight, insets.right, insets.left)) + rect.x
				rect.height -= (insets.bottom + rect.y)
			End If
		End Sub

		Friend Shared Sub paint(ByVal context As SynthContext, ByVal accContext As SynthContext, ByVal g As Graphics, ByVal checkIcon As Icon, ByVal arrowIcon As Icon, ByVal acceleratorDelimiter As String, ByVal defaultTextIconGap As Integer, ByVal propertyPrefix As String)
			Dim mi As JMenuItem = CType(context.component, JMenuItem)
			Dim style As SynthStyle = context.style
			g.font = style.getFont(context)

			Dim viewRect As New Rectangle(0, 0, mi.width, mi.height)
			Dim leftToRight As Boolean = SynthLookAndFeel.isLeftToRight(mi)
			applyInsets(viewRect, mi.insets, leftToRight)

			Dim lh As New SynthMenuItemLayoutHelper(context, accContext, mi, checkIcon, arrowIcon, viewRect, defaultTextIconGap, acceleratorDelimiter, leftToRight, sun.swing.MenuItemLayoutHelper.useCheckAndArrow(mi), propertyPrefix)
			Dim lr As sun.swing.MenuItemLayoutHelper.LayoutResult = lh.layoutMenuItem()

			paintMenuItem(g, lh, lr)
		End Sub

		Friend Shared Sub paintMenuItem(ByVal g As Graphics, ByVal lh As SynthMenuItemLayoutHelper, ByVal lr As sun.swing.MenuItemLayoutHelper.LayoutResult)
			' Save original graphics font and color
			Dim holdf As Font = g.font
			Dim holdc As Color = g.color

			paintCheckIcon(g, lh, lr)
			paintIcon(g, lh, lr)
			paintText(g, lh, lr)
			paintAccText(g, lh, lr)
			paintArrowIcon(g, lh, lr)

			' Restore original graphics font and color
			g.color = holdc
			g.font = holdf
		End Sub

		Friend Shared Sub paintBackground(ByVal g As Graphics, ByVal lh As SynthMenuItemLayoutHelper)
			paintBackground(lh.context, g, lh.menuItem)
		End Sub

		Friend Shared Sub paintBackground(ByVal context As SynthContext, ByVal g As Graphics, ByVal c As JComponent)
			context.painter.paintMenuItemBackground(context, g, 0, 0, c.width, c.height)
		End Sub

		Friend Shared Sub paintIcon(ByVal g As Graphics, ByVal lh As SynthMenuItemLayoutHelper, ByVal lr As sun.swing.MenuItemLayoutHelper.LayoutResult)
			If lh.icon IsNot Nothing Then
				Dim icon As Icon
				Dim mi As JMenuItem = lh.menuItem
				Dim model As ButtonModel = mi.model
				If Not model.enabled Then
					icon = mi.disabledIcon
				ElseIf model.pressed AndAlso model.armed Then
					icon = mi.pressedIcon
					If icon Is Nothing Then icon = mi.icon
				Else
					icon = mi.icon
				End If

				If icon IsNot Nothing Then
					Dim iconRect As Rectangle = lr.iconRect
					SynthIcon.paintIcon(icon, lh.context, g, iconRect.x, iconRect.y, iconRect.width, iconRect.height)
				End If
			End If
		End Sub

		Friend Shared Sub paintCheckIcon(ByVal g As Graphics, ByVal lh As SynthMenuItemLayoutHelper, ByVal lr As sun.swing.MenuItemLayoutHelper.LayoutResult)
			If lh.checkIcon IsNot Nothing Then
				Dim checkRect As Rectangle = lr.checkRect
				SynthIcon.paintIcon(lh.checkIcon, lh.context, g, checkRect.x, checkRect.y, checkRect.width, checkRect.height)
			End If
		End Sub

		Friend Shared Sub paintAccText(ByVal g As Graphics, ByVal lh As SynthMenuItemLayoutHelper, ByVal lr As sun.swing.MenuItemLayoutHelper.LayoutResult)
			Dim accText As String = lh.accText
			If accText IsNot Nothing AndAlso (Not accText.Equals("")) Then
				g.color = lh.accStyle.getColor(lh.accContext, ColorType.TEXT_FOREGROUND)
				g.font = lh.accStyle.getFont(lh.accContext)
				lh.accGraphicsUtils.paintText(lh.accContext, g, accText, lr.accRect.x, lr.accRect.y, -1)
			End If
		End Sub

		Friend Shared Sub paintText(ByVal g As Graphics, ByVal lh As SynthMenuItemLayoutHelper, ByVal lr As sun.swing.MenuItemLayoutHelper.LayoutResult)
			If Not lh.text.Equals("") Then
				If lh.htmlView IsNot Nothing Then
					' Text is HTML
					lh.htmlView.paint(g, lr.textRect)
				Else
					' Text isn't HTML
					g.color = lh.style.getColor(lh.context, ColorType.TEXT_FOREGROUND)
					g.font = lh.style.getFont(lh.context)
					lh.graphicsUtils.paintText(lh.context, g, lh.text, lr.textRect.x, lr.textRect.y, lh.menuItem.displayedMnemonicIndex)
				End If
			End If
		End Sub

		Friend Shared Sub paintArrowIcon(ByVal g As Graphics, ByVal lh As SynthMenuItemLayoutHelper, ByVal lr As sun.swing.MenuItemLayoutHelper.LayoutResult)
			If lh.arrowIcon IsNot Nothing Then
				Dim arrowRect As Rectangle = lr.arrowRect
				SynthIcon.paintIcon(lh.arrowIcon, lh.context, g, arrowRect.x, arrowRect.y, arrowRect.width, arrowRect.height)
			End If
		End Sub

		''' <summary>
		''' Wraps a SynthIcon around the Icon interface, forwarding calls to
		''' the SynthIcon with a given SynthContext.
		''' </summary>
		Private Class SynthIconWrapper
			Implements Icon

			Private Shared ReadOnly CACHE As IList(Of SynthIconWrapper) = New List(Of SynthIconWrapper)(1)

			Private synthIcon As SynthIcon
			Private context As SynthContext

			Shared Function [get](ByVal icon As SynthIcon, ByVal context As SynthContext) As SynthIconWrapper
				SyncLock CACHE
					Dim size As Integer = CACHE.Count
					If size > 0 Then
						Dim wrapper As SynthIconWrapper = CACHE.Remove(size - 1)
						wrapper.reset(icon, context)
						Return wrapper
					End If
				End SyncLock
				Return New SynthIconWrapper(icon, context)
			End Function

			Shared Sub release(ByVal wrapper As SynthIconWrapper)
				wrapper.reset(Nothing, Nothing)
				SyncLock CACHE
					CACHE.Add(wrapper)
				End SyncLock
			End Sub

			Friend Sub New(ByVal icon As SynthIcon, ByVal context As SynthContext)
				reset(icon, context)
			End Sub

			Friend Overridable Sub reset(ByVal icon As SynthIcon, ByVal context As SynthContext)
				synthIcon = icon
				Me.context = context
			End Sub

			Public Overridable Sub paintIcon(ByVal c As Component, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer)
				' This is a noop as this should only be for sizing calls.
			End Sub

			Public Overridable Property iconWidth As Integer Implements Icon.getIconWidth
				Get
					Return synthIcon.getIconWidth(context)
				End Get
			End Property

			Public Overridable Property iconHeight As Integer Implements Icon.getIconHeight
				Get
					Return synthIcon.getIconHeight(context)
				End Get
			End Property
		End Class
	End Class

End Namespace
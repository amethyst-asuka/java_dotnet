Imports Microsoft.VisualBasic
Imports System
Imports javax.swing
Imports javax.swing.plaf

'
' * Copyright (c) 2002, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' Provides the Synth L&amp;F UI delegate for
	''' <seealso cref="javax.swing.JProgressBar"/>.
	''' 
	''' @author Joshua Outwater
	''' @since 1.7
	''' </summary>
	Public Class SynthProgressBarUI
		Inherits javax.swing.plaf.basic.BasicProgressBarUI
		Implements SynthUI, java.beans.PropertyChangeListener

		Private style As SynthStyle
		Private progressPadding As Integer
		Private rotateText As Boolean ' added for Nimbus LAF
		Private paintOutsideClip As Boolean
		Private tileWhenIndeterminate As Boolean 'whether to tile indeterminate painting
		Private tileWidth As Integer 'the width of each tile

		''' <summary>
		''' Creates a new UI object for the given component.
		''' </summary>
		''' <param name="x"> component to create UI object for </param>
		''' <returns> the UI object </returns>
		Public Shared Function createUI(ByVal x As JComponent) As ComponentUI
			Return New SynthProgressBarUI
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub installListeners()
			MyBase.installListeners()
			progressBar.addPropertyChangeListener(Me)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub uninstallListeners()
			MyBase.uninstallListeners()
			progressBar.removePropertyChangeListener(Me)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub installDefaults()
			updateStyle(progressBar)
		End Sub

		Private Sub updateStyle(ByVal c As JProgressBar)
			Dim ___context As SynthContext = getContext(c, ENABLED)
			Dim oldStyle As SynthStyle = style
			style = SynthLookAndFeel.updateStyle(___context, Me)
			cellLength = style.getInt(___context, "ProgressBar.cellLength", 1)
			cellSpacing = style.getInt(___context, "ProgressBar.cellSpacing", 0)
			progressPadding = style.getInt(___context, "ProgressBar.progressPadding", 0)
			paintOutsideClip = style.getBoolean(___context, "ProgressBar.paintOutsideClip", False)
			rotateText = style.getBoolean(___context, "ProgressBar.rotateText", False)
			tileWhenIndeterminate = style.getBoolean(___context, "ProgressBar.tileWhenIndeterminate", False)
			tileWidth = style.getInt(___context, "ProgressBar.tileWidth", 15)
			' handle scaling for sizeVarients for special case components. The
			' key "JComponent.sizeVariant" scales for large/small/mini
			' components are based on Apples LAF
			Dim scaleKey As String = CStr(progressBar.getClientProperty("JComponent.sizeVariant"))
			If scaleKey IsNot Nothing Then
				If "large".Equals(scaleKey) Then
					tileWidth *= 1.15
				ElseIf "small".Equals(scaleKey) Then
					tileWidth *= 0.857
				ElseIf "mini".Equals(scaleKey) Then
					tileWidth *= 0.784
				End If
			End If
			___context.Dispose()
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub uninstallDefaults()
			Dim ___context As SynthContext = getContext(progressBar, ENABLED)

			style.uninstallDefaults(___context)
			___context.Dispose()
			style = Nothing
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Function getContext(ByVal c As JComponent) As SynthContext
			Return getContext(c, getComponentState(c))
		End Function

		Private Function getContext(ByVal c As JComponent, ByVal state As Integer) As SynthContext
			Return SynthContext.getContext(c, style, state)
		End Function

		Private Function getComponentState(ByVal c As JComponent) As Integer
			Return SynthLookAndFeel.getComponentState(c)
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Function getBaseline(ByVal c As JComponent, ByVal width As Integer, ByVal height As Integer) As Integer
			MyBase.getBaseline(c, width, height)
			If progressBar.stringPainted AndAlso progressBar.orientation = JProgressBar.HORIZONTAL Then
				Dim ___context As SynthContext = getContext(c)
				Dim font As Font = ___context.style.getFont(___context)
				Dim metrics As FontMetrics = progressBar.getFontMetrics(font)
				___context.Dispose()
				Return (height - metrics.ascent - metrics.descent) / 2 + metrics.ascent
			End If
			Return -1
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Function getBox(ByVal r As Rectangle) As Rectangle
			If tileWhenIndeterminate Then
				Return SwingUtilities.calculateInnerArea(progressBar, r)
			Else
				Return MyBase.getBox(r)
			End If
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Property animationIndex As Integer
			Set(ByVal newValue As Integer)
				If paintOutsideClip Then
					If animationIndex = newValue Then Return
					MyBase.animationIndex = newValue
					progressBar.repaint()
				Else
					MyBase.animationIndex = newValue
				End If
			End Set
		End Property

		''' <summary>
		''' Notifies this UI delegate to repaint the specified component.
		''' This method paints the component background, then calls
		''' the <seealso cref="#paint(SynthContext,Graphics)"/> method.
		''' 
		''' <p>In general, this method does not need to be overridden by subclasses.
		''' All Look and Feel rendering code should reside in the {@code paint} method.
		''' </summary>
		''' <param name="g"> the {@code Graphics} object used for painting </param>
		''' <param name="c"> the component being painted </param>
		''' <seealso cref= #paint(SynthContext,Graphics) </seealso>
		Public Overrides Sub update(ByVal g As Graphics, ByVal c As JComponent)
			Dim ___context As SynthContext = getContext(c)

			SynthLookAndFeel.update(___context, g)
			___context.painter.paintProgressBarBackground(___context, g, 0, 0, c.width, c.height, progressBar.orientation)
			paint(___context, g)
			___context.Dispose()
		End Sub

		''' <summary>
		''' Paints the specified component according to the Look and Feel.
		''' <p>This method is not used by Synth Look and Feel.
		''' Painting is handled by the <seealso cref="#paint(SynthContext,Graphics)"/> method.
		''' </summary>
		''' <param name="g"> the {@code Graphics} object used for painting </param>
		''' <param name="c"> the component being painted </param>
		''' <seealso cref= #paint(SynthContext,Graphics) </seealso>
		Public Overrides Sub paint(ByVal g As Graphics, ByVal c As JComponent)
			Dim ___context As SynthContext = getContext(c)

			paint(___context, g)
			___context.Dispose()
		End Sub

		''' <summary>
		''' Paints the specified component.
		''' </summary>
		''' <param name="context"> context for the component being painted </param>
		''' <param name="g"> the {@code Graphics} object used for painting </param>
		''' <seealso cref= #update(Graphics,JComponent) </seealso>
		Protected Friend Overridable Sub paint(ByVal context As SynthContext, ByVal g As Graphics)
			Dim pBar As JProgressBar = CType(context.component, JProgressBar)
			Dim x As Integer = 0, y As Integer = 0, width As Integer = 0, height As Integer = 0
			If Not pBar.indeterminate Then
				Dim pBarInsets As Insets = pBar.insets
				Dim percentComplete As Double = pBar.percentComplete
				If percentComplete <> 0.0 Then
					If pBar.orientation = JProgressBar.HORIZONTAL Then
						x = pBarInsets.left + progressPadding
						y = pBarInsets.top + progressPadding
						width = CInt(Fix(percentComplete * (pBar.width - (pBarInsets.left + progressPadding + pBarInsets.right + progressPadding))))
						height = pBar.height - (pBarInsets.top + progressPadding + pBarInsets.bottom + progressPadding)

						If Not SynthLookAndFeel.isLeftToRight(pBar) Then
							x = pBar.width - pBarInsets.right - width - progressPadding
						End If ' JProgressBar.VERTICAL
					Else
						x = pBarInsets.left + progressPadding
						width = pBar.width - (pBarInsets.left + progressPadding + pBarInsets.right + progressPadding)
						height = CInt(Fix(percentComplete * (pBar.height - (pBarInsets.top + progressPadding + pBarInsets.bottom + progressPadding))))
						y = pBar.height - pBarInsets.bottom - height - progressPadding

						' When the progress bar is vertical we always paint
						' from bottom to top, not matter what the component
						' orientation is.
					End If
				End If
			Else
				boxRect = getBox(boxRect)
				x = boxRect.x + progressPadding
				y = boxRect.y + progressPadding
				width = boxRect.width - progressPadding - progressPadding
				height = boxRect.height - progressPadding - progressPadding
			End If

			'if tiling and indeterminate, then paint the progress bar foreground a
			'bit wider than it should be. Shift as needed to ensure that there is
			'an animated effect
			If tileWhenIndeterminate AndAlso pBar.indeterminate Then
				Dim percentComplete As Double = CDbl(animationIndex) / CDbl(frameCount)
				Dim offset As Integer = CInt(Fix(percentComplete * tileWidth))
				Dim clip As Shape = g.clip
				g.clipRect(x, y, width, height)
				If pBar.orientation = JProgressBar.HORIZONTAL Then
					'paint each tile horizontally
					For i As Integer = x-tileWidth+offset To width Step tileWidth
						context.painter.paintProgressBarForeground(context, g, i, y, tileWidth, height, pBar.orientation)
					Next i 'JProgressBar.VERTICAL
				Else
					'paint each tile vertically
					For i As Integer = y-offset To height+tileWidth - 1 Step tileWidth
						context.painter.paintProgressBarForeground(context, g, x, i, width, tileWidth, pBar.orientation)
					Next i
				End If
				g.clip = clip
			Else
				context.painter.paintProgressBarForeground(context, g, x, y, width, height, pBar.orientation)
			End If

			If pBar.stringPainted Then paintText(context, g, pBar.string)
		End Sub

		''' <summary>
		''' Paints the component's text.
		''' </summary>
		''' <param name="context"> context for the component being painted </param>
		''' <param name="g"> {@code Graphics} object used for painting </param>
		''' <param name="title"> the text to paint </param>
		Protected Friend Overridable Sub paintText(ByVal context As SynthContext, ByVal g As Graphics, ByVal title As String)
			If progressBar.stringPainted Then
				Dim style As SynthStyle = context.style
				Dim font As Font = style.getFont(context)
				Dim fm As FontMetrics = sun.swing.SwingUtilities2.getFontMetrics(progressBar, g, font)
				Dim strLength As Integer = style.getGraphicsUtils(context).computeStringWidth(context, font, fm, title)
				Dim bounds As Rectangle = progressBar.bounds

				If rotateText AndAlso progressBar.orientation = JProgressBar.VERTICAL Then
					Dim g2 As Graphics2D = CType(g, Graphics2D)
					' Calculate the position for the text.
					Dim textPos As Point
					Dim rotation As java.awt.geom.AffineTransform
					If progressBar.componentOrientation.leftToRight Then
						rotation = java.awt.geom.AffineTransform.getRotateInstance(-Math.PI/2)
						textPos = New Point((bounds.width+fm.ascent-fm.descent)/2, (bounds.height+strLength)/2)
					Else
						rotation = java.awt.geom.AffineTransform.getRotateInstance(Math.PI/2)
						textPos = New Point((bounds.width-fm.ascent+fm.descent)/2, (bounds.height-strLength)/2)
					End If

					' Progress bar isn't wide enough for the font.  Don't paint it.
					If textPos.x < 0 Then Return

					' Paint the text.
					font = font.deriveFont(rotation)
					g2.font = font
					g2.color = style.getColor(context, ColorType.TEXT_FOREGROUND)
					style.getGraphicsUtils(context).paintText(context, g, title, textPos.x, textPos.y, -1)
				Else
					' Calculate the bounds for the text.
					Dim textRect As New Rectangle((bounds.width / 2) - (strLength \ 2), (bounds.height - (fm.ascent + fm.descent)) / 2, 0, 0)

					' Progress bar isn't tall enough for the font.  Don't paint it.
					If textRect.y < 0 Then Return

					' Paint the text.
					g.color = style.getColor(context, ColorType.TEXT_FOREGROUND)
					g.font = font
					style.getGraphicsUtils(context).paintText(context, g, title, textRect.x, textRect.y, -1)
				End If
			End If
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Sub paintBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			context.painter.paintProgressBarBorder(context, g, x, y, w, h, progressBar.orientation)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Sub propertyChange(ByVal e As java.beans.PropertyChangeEvent)
			If SynthLookAndFeel.shouldUpdateStyle(e) OrElse "indeterminate".Equals(e.propertyName) Then updateStyle(CType(e.source, JProgressBar))
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Function getPreferredSize(ByVal c As JComponent) As Dimension
			Dim size As Dimension = Nothing
			Dim border As Insets = progressBar.insets
			Dim fontSizer As FontMetrics = progressBar.getFontMetrics(progressBar.font)
			Dim progString As String = progressBar.string
			Dim stringHeight As Integer = fontSizer.height + fontSizer.descent

			If progressBar.orientation = JProgressBar.HORIZONTAL Then
				size = New Dimension(preferredInnerHorizontal)
				If progressBar.stringPainted Then
					' adjust the height if necessary to make room for the string
					If stringHeight > size.height Then size.height = stringHeight

					' adjust the width if necessary to make room for the string
					Dim stringWidth As Integer = sun.swing.SwingUtilities2.stringWidth(progressBar, fontSizer, progString)
					If stringWidth > size.width Then size.width = stringWidth
				End If
			Else
				size = New Dimension(preferredInnerVertical)
				If progressBar.stringPainted Then
					' make sure the width is big enough for the string
					If stringHeight > size.width Then size.width = stringHeight

					' make sure the height is big enough for the string
					Dim stringWidth As Integer = sun.swing.SwingUtilities2.stringWidth(progressBar, fontSizer, progString)
					If stringWidth > size.height Then size.height = stringWidth
				End If
			End If

			' handle scaling for sizeVarients for special case components. The
			' key "JComponent.sizeVariant" scales for large/small/mini
			' components are based on Apples LAF
			Dim scaleKey As String = CStr(progressBar.getClientProperty("JComponent.sizeVariant"))
			If scaleKey IsNot Nothing Then
				If "large".Equals(scaleKey) Then
					size.width *= 1.15f
					size.height *= 1.15f
				ElseIf "small".Equals(scaleKey) Then
					size.width *= 0.90f
					size.height *= 0.90f
				ElseIf "mini".Equals(scaleKey) Then
					size.width *= 0.784f
					size.height *= 0.784f
				End If
			End If

			size.width += border.left + border.right
			size.height += border.top + border.bottom

			Return size
		End Function
	End Class

End Namespace
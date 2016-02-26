Imports System
Imports javax.swing

'
' * Copyright (c) 2005, 2006, Oracle and/or its affiliates. All rights reserved.
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
Namespace javax.swing.plaf.nimbus




	Friend Class SynthPainterImpl
		Inherits javax.swing.plaf.synth.SynthPainter

		Private style As NimbusStyle

		Friend Sub New(ByVal style As NimbusStyle)
			Me.style = style
		End Sub

		''' <summary>
		''' Paint the provided painter using the provided transform at the specified
		''' position and size. Handles if g is a non 2D Graphics by painting via a
		''' BufferedImage.
		''' </summary>
		Private Sub paint(ByVal p As javax.swing.Painter, ByVal ctx As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal transform As java.awt.geom.AffineTransform)
			If p IsNot Nothing Then
				If TypeOf g Is Graphics2D Then
					Dim gfx As Graphics2D = CType(g, Graphics2D)
					If transform IsNot Nothing Then gfx.transform(transform)
					gfx.translate(x, y)
					p.paint(gfx, ctx.component, w, h)
					gfx.translate(-x, -y)
					If transform IsNot Nothing Then
						Try
							gfx.transform(transform.createInverse())
						Catch e As java.awt.geom.NoninvertibleTransformException
							' this should never happen as we are in control of all
							' calls into this method and only ever pass in simple
							' transforms of rotate, flip and translates
							Console.WriteLine(e.ToString())
							Console.Write(e.StackTrace)
						End Try
					End If
				Else
					' use image if we are printing to a Java 1.1 PrintGraphics as
					' it is not a instance of Graphics2D
					Dim img As New java.awt.image.BufferedImage(w,h, java.awt.image.BufferedImage.TYPE_INT_ARGB)
					Dim gfx As Graphics2D = img.createGraphics()
					If transform IsNot Nothing Then gfx.transform(transform)
					p.paint(gfx, ctx.component, w, h)
					gfx.Dispose()
					g.drawImage(img,x,y,Nothing)
					img = Nothing
				End If
			End If
		End Sub

		Private Sub paintBackground(ByVal ctx As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal transform As java.awt.geom.AffineTransform)
			' if the background color of the component is 100% transparent
			' then we should not paint any background graphics. This is a solution
			' for there being no way of turning off Nimbus background painting as
			' basic components are all non-opaque by default.
			Dim c As Component = ctx.component
			Dim bg As Color = If(c IsNot Nothing, c.background, Nothing)
			If bg Is Nothing OrElse bg.alpha > 0 Then
				Dim backgroundPainter As javax.swing.Painter = style.getBackgroundPainter(ctx)
				If backgroundPainter IsNot Nothing Then paint(backgroundPainter, ctx, g, x, y, w, h,transform)
			End If
		End Sub

		Private Sub paintForeground(ByVal ctx As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal transform As java.awt.geom.AffineTransform)
			Dim foregroundPainter As javax.swing.Painter = style.getForegroundPainter(ctx)
			If foregroundPainter IsNot Nothing Then paint(foregroundPainter, ctx, g, x, y, w, h,transform)
		End Sub

		Private Sub paintBorder(ByVal ctx As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal transform As java.awt.geom.AffineTransform)
			Dim borderPainter As javax.swing.Painter = style.getBorderPainter(ctx)
			If borderPainter IsNot Nothing Then paint(borderPainter, ctx, g, x, y, w, h,transform)
		End Sub

		Private Sub paintBackground(ByVal ctx As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
			Dim c As Component = ctx.component
			Dim ltr As Boolean = c.componentOrientation.leftToRight
			' Don't RTL flip JSpliders as they handle it internaly
			If TypeOf ctx.component Is JSlider Then ltr = True

			If orientation = SwingConstants.VERTICAL AndAlso ltr Then
				Dim transform As New java.awt.geom.AffineTransform
				transform.scale(-1, 1)
				transform.rotate(Math.toRadians(90))
				paintBackground(ctx, g, y, x, h, w, transform)
			ElseIf orientation = SwingConstants.VERTICAL Then
				Dim transform As New java.awt.geom.AffineTransform
				transform.rotate(Math.toRadians(90))
				transform.translate(0,-(x+w))
				paintBackground(ctx, g, y, x, h, w, transform)
			ElseIf orientation = SwingConstants.HORIZONTAL AndAlso ltr Then
				paintBackground(ctx, g, x, y, w, h, Nothing)
			Else
				'horizontal and right-to-left orientation
				Dim transform As New java.awt.geom.AffineTransform
				transform.translate(x,y)
				transform.scale(-1, 1)
				transform.translate(-w,0)
				paintBackground(ctx, g, 0, 0, w, h, transform)
			End If
		End Sub

		Private Sub paintBorder(ByVal ctx As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
			Dim c As Component = ctx.component
			Dim ltr As Boolean = c.componentOrientation.leftToRight
			If orientation = SwingConstants.VERTICAL AndAlso ltr Then
				Dim transform As New java.awt.geom.AffineTransform
				transform.scale(-1, 1)
				transform.rotate(Math.toRadians(90))
				paintBorder(ctx, g, y, x, h, w, transform)
			ElseIf orientation = SwingConstants.VERTICAL Then
				Dim transform As New java.awt.geom.AffineTransform
				transform.rotate(Math.toRadians(90))
				transform.translate(0, -(x + w))
				paintBorder(ctx, g, y, 0, h, w, transform)
			ElseIf orientation = SwingConstants.HORIZONTAL AndAlso ltr Then
				paintBorder(ctx, g, x, y, w, h, Nothing)
			Else
				'horizontal and right-to-left orientation
				paintBorder(ctx, g, x, y, w, h, Nothing)
			End If
		End Sub

		Private Sub paintForeground(ByVal ctx As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
			Dim c As Component = ctx.component
			Dim ltr As Boolean = c.componentOrientation.leftToRight
			If orientation = SwingConstants.VERTICAL AndAlso ltr Then
				Dim transform As New java.awt.geom.AffineTransform
				transform.scale(-1, 1)
				transform.rotate(Math.toRadians(90))
				paintForeground(ctx, g, y, x, h, w, transform)
			ElseIf orientation = SwingConstants.VERTICAL Then
				Dim transform As New java.awt.geom.AffineTransform
				transform.rotate(Math.toRadians(90))
				transform.translate(0, -(x + w))
				paintForeground(ctx, g, y, 0, h, w, transform)
			ElseIf orientation = SwingConstants.HORIZONTAL AndAlso ltr Then
				paintForeground(ctx, g, x, y, w, h, Nothing)
			Else
				'horizontal and right-to-left orientation
				paintForeground(ctx, g, x, y, w, h, Nothing)
			End If
		End Sub

		''' <summary>
		''' Paints the background of an arrow button. Arrow buttons are created by
		''' some components, such as <code>JScrollBar</code>.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintArrowButtonBackground(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			If context.component.componentOrientation.leftToRight Then
				paintBackground(context, g, x, y, w, h, Nothing)
			Else
				Dim transform As New java.awt.geom.AffineTransform
				transform.translate(x,y)
				transform.scale(-1, 1)
				transform.translate(-w,0)
				paintBackground(context, g, 0, 0, w, h, transform)
			End If
		End Sub

		''' <summary>
		''' Paints the border of an arrow button. Arrow buttons are created by
		''' some components, such as <code>JScrollBar</code>.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintArrowButtonBorder(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBorder(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the foreground of an arrow button. This method is responsible
		''' for drawing a graphical representation of a direction, typically
		''' an arrow. Arrow buttons are created by
		''' some components, such as <code>JScrollBar</code>
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		''' <param name="direction"> One of SwingConstants.NORTH, SwingConstants.SOUTH
		'''                  SwingConstants.EAST or SwingConstants.WEST </param>
		Public Overridable Sub paintArrowButtonForeground(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal direction As Integer)
			'assume that the painter is arranged with the arrow pointing... LEFT?
			Dim compName As String = context.component.name
			Dim ltr As Boolean = context.component.componentOrientation.leftToRight
			' The hard coding for spinners here needs to be replaced by a more
			' general method for disabling rotation
			If "Spinner.nextButton".Equals(compName) OrElse "Spinner.previousButton".Equals(compName) Then
				If ltr Then
					paintForeground(context, g, x, y, w, h, Nothing)
				Else
					Dim transform As New java.awt.geom.AffineTransform
					transform.translate(w, 0)
					transform.scale(-1, 1)
					paintForeground(context, g, x, y, w, h, transform)
				End If
			ElseIf direction = SwingConstants.WEST Then
				paintForeground(context, g, x, y, w, h, Nothing)
			ElseIf direction = SwingConstants.NORTH Then
				If ltr Then
					Dim transform As New java.awt.geom.AffineTransform
					transform.scale(-1, 1)
					transform.rotate(Math.toRadians(90))
					paintForeground(context, g, y, 0, h, w, transform)
				Else
					Dim transform As New java.awt.geom.AffineTransform
					transform.rotate(Math.toRadians(90))
					transform.translate(0, -(x + w))
					paintForeground(context, g, y, 0, h, w, transform)
				End If
			ElseIf direction = SwingConstants.EAST Then
				Dim transform As New java.awt.geom.AffineTransform
				transform.translate(w, 0)
				transform.scale(-1, 1)
				paintForeground(context, g, x, y, w, h, transform)
			ElseIf direction = SwingConstants.SOUTH Then
				If ltr Then
					Dim transform As New java.awt.geom.AffineTransform
					transform.rotate(Math.toRadians(-90))
					transform.translate(-h, 0)
					paintForeground(context, g, y, x, h, w, transform)
				Else
					Dim transform As New java.awt.geom.AffineTransform
					transform.scale(-1, 1)
					transform.rotate(Math.toRadians(-90))
					transform.translate(-(h+y), -(w+x))
					paintForeground(context, g, y, x, h, w, transform)
				End If
			End If
		End Sub

		''' <summary>
		''' Paints the background of a button.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintButtonBackground(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBackground(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the border of a button.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintButtonBorder(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBorder(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the background of a check box menu item.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintCheckBoxMenuItemBackground(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBackground(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the border of a check box menu item.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintCheckBoxMenuItemBorder(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBorder(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the background of a check box.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintCheckBoxBackground(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBackground(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the border of a check box.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintCheckBoxBorder(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBorder(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the background of a color chooser.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintColorChooserBackground(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBackground(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the border of a color chooser.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintColorChooserBorder(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBorder(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the background of a combo box.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintComboBoxBackground(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			If context.component.componentOrientation.leftToRight Then
				paintBackground(context, g, x, y, w, h, Nothing)
			Else
				Dim transform As New java.awt.geom.AffineTransform
				transform.translate(x,y)
				transform.scale(-1, 1)
				transform.translate(-w,0)
				paintBackground(context, g, 0, 0, w, h, transform)
			End If
		End Sub

		''' <summary>
		''' Paints the border of a combo box.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintComboBoxBorder(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBorder(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the background of a desktop icon.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintDesktopIconBackground(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBackground(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the border of a desktop icon.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintDesktopIconBorder(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBorder(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the background of a desktop pane.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintDesktopPaneBackground(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBackground(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the background of a desktop pane.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintDesktopPaneBorder(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBorder(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the background of an editor pane.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintEditorPaneBackground(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBackground(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the border of an editor pane.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintEditorPaneBorder(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBorder(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the background of a file chooser.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintFileChooserBackground(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBackground(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the border of a file chooser.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintFileChooserBorder(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBorder(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the background of a formatted text field.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintFormattedTextFieldBackground(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			If context.component.componentOrientation.leftToRight Then
				paintBackground(context, g, x, y, w, h, Nothing)
			Else
				Dim transform As New java.awt.geom.AffineTransform
				transform.translate(x,y)
				transform.scale(-1, 1)
				transform.translate(-w,0)
				paintBackground(context, g, 0, 0, w, h, transform)
			End If
		End Sub

		''' <summary>
		''' Paints the border of a formatted text field.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintFormattedTextFieldBorder(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			If context.component.componentOrientation.leftToRight Then
				paintBorder(context, g, x, y, w, h, Nothing)
			Else
				Dim transform As New java.awt.geom.AffineTransform
				transform.translate(x,y)
				transform.scale(-1, 1)
				transform.translate(-w,0)
				paintBorder(context, g, 0, 0, w, h, transform)
			End If
		End Sub

		''' <summary>
		''' Paints the background of an internal frame title pane.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintInternalFrameTitlePaneBackground(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBackground(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the border of an internal frame title pane.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintInternalFrameTitlePaneBorder(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBorder(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the background of an internal frame.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintInternalFrameBackground(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBackground(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the border of an internal frame.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintInternalFrameBorder(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBorder(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the background of a label.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintLabelBackground(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBackground(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the border of a label.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintLabelBorder(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBorder(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the background of a list.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintListBackground(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBackground(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the border of a list.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintListBorder(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBorder(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the background of a menu bar.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintMenuBarBackground(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBackground(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the border of a menu bar.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintMenuBarBorder(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBorder(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the background of a menu item.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintMenuItemBackground(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBackground(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the border of a menu item.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintMenuItemBorder(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBorder(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the background of a menu.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintMenuBackground(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBackground(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the border of a menu.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintMenuBorder(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBorder(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the background of an option pane.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintOptionPaneBackground(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBackground(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the border of an option pane.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintOptionPaneBorder(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBorder(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the background of a panel.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintPanelBackground(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBackground(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the border of a panel.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintPanelBorder(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBorder(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the background of a password field.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintPasswordFieldBackground(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBackground(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the border of a password field.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintPasswordFieldBorder(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBorder(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the background of a popup menu.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintPopupMenuBackground(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBackground(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the border of a popup menu.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintPopupMenuBorder(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBorder(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the background of a progress bar.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintProgressBarBackground(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBackground(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the background of a progress bar. This implementation invokes the
		''' method of the same name without the orientation.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		''' <param name="orientation"> one of <code>JProgressBar.HORIZONTAL</code> or
		'''                    <code>JProgressBar.VERTICAL</code>
		''' @since 1.6 </param>
		Public Overridable Sub paintProgressBarBackground(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
			paintBackground(context, g, x, y, w, h, orientation)
		End Sub

		''' <summary>
		''' Paints the border of a progress bar.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintProgressBarBorder(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBorder(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the border of a progress bar. This implementation invokes the
		''' method of the same name without the orientation.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		''' <param name="orientation"> one of <code>JProgressBar.HORIZONTAL</code> or
		'''                    <code>JProgressBar.VERTICAL</code>
		''' @since 1.6 </param>
		Public Overridable Sub paintProgressBarBorder(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
			paintBorder(context, g, x, y, w, h, orientation)
		End Sub

		''' <summary>
		''' Paints the foreground of a progress bar. is responsible for
		''' providing an indication of the progress of the progress bar.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		''' <param name="orientation"> one of <code>JProgressBar.HORIZONTAL</code> or
		'''                    <code>JProgressBar.VERTICAL</code> </param>
		Public Overridable Sub paintProgressBarForeground(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
			paintForeground(context, g, x, y, w, h, orientation)
		End Sub

		''' <summary>
		''' Paints the background of a radio button menu item.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintRadioButtonMenuItemBackground(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBackground(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the border of a radio button menu item.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintRadioButtonMenuItemBorder(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBorder(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the background of a radio button.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintRadioButtonBackground(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBackground(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the border of a radio button.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintRadioButtonBorder(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBorder(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the background of a root pane.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintRootPaneBackground(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBackground(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the border of a root pane.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintRootPaneBorder(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBorder(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the background of a scrollbar.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintScrollBarBackground(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBackground(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the background of a scrollbar. This implementation invokes the
		''' method of the same name without the orientation.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		''' <param name="orientation"> Orientation of the JScrollBar, one of
		'''                    <code>JScrollBar.HORIZONTAL</code> or
		'''                    <code>JScrollBar.VERTICAL</code>
		''' @since 1.6 </param>
		Public Overridable Sub paintScrollBarBackground(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
			paintBackground(context, g, x, y, w, h, orientation)
		End Sub

		''' <summary>
		''' Paints the border of a scrollbar.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintScrollBarBorder(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBorder(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the border of a scrollbar. This implementation invokes the
		''' method of the same name without the orientation.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		''' <param name="orientation"> Orientation of the JScrollBar, one of
		'''                    <code>JScrollBar.HORIZONTAL</code> or
		'''                    <code>JScrollBar.VERTICAL</code>
		''' @since 1.6 </param>
		Public Overridable Sub paintScrollBarBorder(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
			paintBorder(context, g, x, y, w, h, orientation)
		End Sub

		''' <summary>
		''' Paints the background of the thumb of a scrollbar. The thumb provides
		''' a graphical indication as to how much of the Component is visible in a
		''' <code>JScrollPane</code>.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		''' <param name="orientation"> Orientation of the JScrollBar, one of
		'''                    <code>JScrollBar.HORIZONTAL</code> or
		'''                    <code>JScrollBar.VERTICAL</code> </param>
		Public Overridable Sub paintScrollBarThumbBackground(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
			paintBackground(context, g, x, y, w, h, orientation)
		End Sub

		''' <summary>
		''' Paints the border of the thumb of a scrollbar. The thumb provides
		''' a graphical indication as to how much of the Component is visible in a
		''' <code>JScrollPane</code>.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		''' <param name="orientation"> Orientation of the JScrollBar, one of
		'''                    <code>JScrollBar.HORIZONTAL</code> or
		'''                    <code>JScrollBar.VERTICAL</code> </param>
		Public Overridable Sub paintScrollBarThumbBorder(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
			paintBorder(context, g, x, y, w, h, orientation)
		End Sub

		''' <summary>
		''' Paints the background of the track of a scrollbar. The track contains
		''' the thumb.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintScrollBarTrackBackground(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBackground(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the background of the track of a scrollbar. The track contains
		''' the thumb. This implementation invokes the method of the same name without
		''' the orientation.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		''' <param name="orientation"> Orientation of the JScrollBar, one of
		'''                    <code>JScrollBar.HORIZONTAL</code> or
		'''                    <code>JScrollBar.VERTICAL</code>
		''' @since 1.6 </param>
		Public Overridable Sub paintScrollBarTrackBackground(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
			paintBackground(context, g, x, y, w, h, orientation)
		End Sub

		''' <summary>
		''' Paints the border of the track of a scrollbar. The track contains
		''' the thumb.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintScrollBarTrackBorder(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBorder(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the border of the track of a scrollbar. The track contains
		''' the thumb. This implementation invokes the method of the same name without
		''' the orientation.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		''' <param name="orientation"> Orientation of the JScrollBar, one of
		'''                    <code>JScrollBar.HORIZONTAL</code> or
		'''                    <code>JScrollBar.VERTICAL</code>
		''' @since 1.6 </param>
		Public Overridable Sub paintScrollBarTrackBorder(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
			paintBorder(context, g, x, y, w, h, orientation)
		End Sub

		''' <summary>
		''' Paints the background of a scroll pane.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintScrollPaneBackground(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBackground(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the border of a scroll pane.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintScrollPaneBorder(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBorder(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the background of a separator.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintSeparatorBackground(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBackground(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the background of a separator. This implementation invokes the
		''' method of the same name without the orientation.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		''' <param name="orientation"> One of <code>JSeparator.HORIZONTAL</code> or
		'''                           <code>JSeparator.VERTICAL</code>
		''' @since 1.6 </param>
		Public Overridable Sub paintSeparatorBackground(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
			paintBackground(context, g, x, y, w, h, orientation)
		End Sub

		''' <summary>
		''' Paints the border of a separator.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintSeparatorBorder(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBorder(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the border of a separator. This implementation invokes the
		''' method of the same name without the orientation.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		''' <param name="orientation"> One of <code>JSeparator.HORIZONTAL</code> or
		'''                           <code>JSeparator.VERTICAL</code>
		''' @since 1.6 </param>
		Public Overridable Sub paintSeparatorBorder(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
			paintBorder(context, g, x, y, w, h, orientation)
		End Sub

		''' <summary>
		''' Paints the foreground of a separator.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		''' <param name="orientation"> One of <code>JSeparator.HORIZONTAL</code> or
		'''                           <code>JSeparator.VERTICAL</code> </param>
		Public Overridable Sub paintSeparatorForeground(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
			paintForeground(context, g, x, y, w, h, orientation)
		End Sub

		''' <summary>
		''' Paints the background of a slider.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintSliderBackground(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBackground(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the background of a slider. This implementation invokes the
		''' method of the same name without the orientation.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		''' <param name="orientation"> One of <code>JSlider.HORIZONTAL</code> or
		'''                           <code>JSlider.VERTICAL</code>
		''' @since 1.6 </param>
		Public Overridable Sub paintSliderBackground(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
			paintBackground(context, g, x, y, w, h, orientation)
		End Sub

		''' <summary>
		''' Paints the border of a slider.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintSliderBorder(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBorder(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the border of a slider. This implementation invokes the
		''' method of the same name without the orientation.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		''' <param name="orientation"> One of <code>JSlider.HORIZONTAL</code> or
		'''                           <code>JSlider.VERTICAL</code>
		''' @since 1.6 </param>
		Public Overridable Sub paintSliderBorder(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
			paintBorder(context, g, x, y, w, h, orientation)
		End Sub

		''' <summary>
		''' Paints the background of the thumb of a slider.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		''' <param name="orientation"> One of <code>JSlider.HORIZONTAL</code> or
		'''                           <code>JSlider.VERTICAL</code> </param>
		Public Overridable Sub paintSliderThumbBackground(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
			If context.component.getClientProperty("Slider.paintThumbArrowShape") Is Boolean.TRUE Then
				If orientation = JSlider.HORIZONTAL Then
					orientation = JSlider.VERTICAL
				Else
					orientation = JSlider.HORIZONTAL
				End If
				paintBackground(context, g, x, y, w, h, orientation)
			Else
				paintBackground(context, g, x, y, w, h, orientation)
			End If
		End Sub

		''' <summary>
		''' Paints the border of the thumb of a slider.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		''' <param name="orientation"> One of <code>JSlider.HORIZONTAL</code> or
		'''                           <code>JSlider.VERTICAL</code> </param>
		Public Overridable Sub paintSliderThumbBorder(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
			paintBorder(context, g, x, y, w, h, orientation)
		End Sub

		''' <summary>
		''' Paints the background of the track of a slider.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintSliderTrackBackground(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBackground(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the background of the track of a slider. This implementation invokes
		''' the method of the same name without the orientation.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		''' <param name="orientation"> One of <code>JSlider.HORIZONTAL</code> or
		'''                           <code>JSlider.VERTICAL</code>
		''' @since 1.6 </param>
		Public Overridable Sub paintSliderTrackBackground(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
			paintBackground(context, g, x, y, w, h, orientation)
		End Sub

		''' <summary>
		''' Paints the border of the track of a slider.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintSliderTrackBorder(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBorder(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the border of the track of a slider. This implementation invokes the
		''' method of the same name without the orientation.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		''' <param name="orientation"> One of <code>JSlider.HORIZONTAL</code> or
		'''                           <code>JSlider.VERTICAL</code>
		''' @since 1.6 </param>
		Public Overridable Sub paintSliderTrackBorder(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
			paintBorder(context, g, x, y, w, h, orientation)
		End Sub

		''' <summary>
		''' Paints the background of a spinner.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintSpinnerBackground(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBackground(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the border of a spinner.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintSpinnerBorder(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBorder(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the background of the divider of a split pane.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintSplitPaneDividerBackground(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBackground(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the background of the divider of a split pane. This implementation
		''' invokes the method of the same name without the orientation.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		''' <param name="orientation"> One of <code>JSplitPane.HORIZONTAL_SPLIT</code> or
		'''                           <code>JSplitPane.VERTICAL_SPLIT</code>
		''' @since 1.6 </param>
		Public Overridable Sub paintSplitPaneDividerBackground(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
		   If orientation = JSplitPane.HORIZONTAL_SPLIT Then
				Dim transform As New java.awt.geom.AffineTransform
				transform.scale(-1, 1)
				transform.rotate(Math.toRadians(90))
				paintBackground(context, g, y, x, h, w, transform)
		   Else
				paintBackground(context, g, x, y, w, h, Nothing)
		   End If
		End Sub

		''' <summary>
		''' Paints the foreground of the divider of a split pane.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		''' <param name="orientation"> One of <code>JSplitPane.HORIZONTAL_SPLIT</code> or
		'''                           <code>JSplitPane.VERTICAL_SPLIT</code> </param>
		Public Overridable Sub paintSplitPaneDividerForeground(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
			paintForeground(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the divider, when the user is dragging the divider, of a
		''' split pane.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		''' <param name="orientation"> One of <code>JSplitPane.HORIZONTAL_SPLIT</code> or
		'''                           <code>JSplitPane.VERTICAL_SPLIT</code> </param>
		Public Overridable Sub paintSplitPaneDragDivider(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
			paintBackground(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the background of a split pane.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintSplitPaneBackground(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBackground(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the border of a split pane.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintSplitPaneBorder(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBorder(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the background of a tabbed pane.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintTabbedPaneBackground(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBackground(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the border of a tabbed pane.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintTabbedPaneBorder(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBorder(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the background of the area behind the tabs of a tabbed pane.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintTabbedPaneTabAreaBackground(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBackground(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the background of the area behind the tabs of a tabbed pane.
		''' This implementation invokes the method of the same name without the
		''' orientation.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		''' <param name="orientation"> One of <code>JTabbedPane.TOP</code>,
		'''                    <code>JTabbedPane.LEFT</code>,
		'''                    <code>JTabbedPane.BOTTOM</code>, or
		'''                    <code>JTabbedPane.RIGHT</code>
		''' @since 1.6 </param>
		Public Overridable Sub paintTabbedPaneTabAreaBackground(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
			If orientation = JTabbedPane.LEFT Then
				Dim transform As New java.awt.geom.AffineTransform
				transform.scale(-1, 1)
				transform.rotate(Math.toRadians(90))
				paintBackground(context, g, y, x, h, w, transform)
			ElseIf orientation = JTabbedPane.RIGHT Then
				Dim transform As New java.awt.geom.AffineTransform
				transform.rotate(Math.toRadians(90))
				transform.translate(0, -(x + w))
				paintBackground(context, g, y, 0, h, w, transform)
			ElseIf orientation = JTabbedPane.BOTTOM Then
				Dim transform As New java.awt.geom.AffineTransform
				transform.translate(x,y)
				transform.scale(1, -1)
				transform.translate(0,-h)
				paintBackground(context, g, 0, 0, w, h, transform)
			Else
				paintBackground(context, g, x, y, w, h, Nothing)
			End If
		End Sub

		''' <summary>
		''' Paints the border of the area behind the tabs of a tabbed pane.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintTabbedPaneTabAreaBorder(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBorder(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the border of the area behind the tabs of a tabbed pane. This
		''' implementation invokes the method of the same name without the orientation.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		''' <param name="orientation"> One of <code>JTabbedPane.TOP</code>,
		'''                    <code>JTabbedPane.LEFT</code>,
		'''                    <code>JTabbedPane.BOTTOM</code>, or
		'''                    <code>JTabbedPane.RIGHT</code>
		''' @since 1.6 </param>
		Public Overridable Sub paintTabbedPaneTabAreaBorder(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
			paintBorder(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the background of a tab of a tabbed pane.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		''' <param name="tabIndex"> Index of tab being painted. </param>
		Public Overridable Sub paintTabbedPaneTabBackground(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal tabIndex As Integer)
			paintBackground(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the background of a tab of a tabbed pane. This implementation
		''' invokes the method of the same name without the orientation.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		''' <param name="tabIndex"> Index of tab being painted. </param>
		''' <param name="orientation"> One of <code>JTabbedPane.TOP</code>,
		'''                    <code>JTabbedPane.LEFT</code>,
		'''                    <code>JTabbedPane.BOTTOM</code>, or
		'''                    <code>JTabbedPane.RIGHT</code>
		''' @since 1.6 </param>
		Public Overridable Sub paintTabbedPaneTabBackground(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal tabIndex As Integer, ByVal orientation As Integer)
			If orientation = JTabbedPane.LEFT Then
				Dim transform As New java.awt.geom.AffineTransform
				transform.scale(-1, 1)
				transform.rotate(Math.toRadians(90))
				paintBackground(context, g, y, x, h, w, transform)
			ElseIf orientation = JTabbedPane.RIGHT Then
				Dim transform As New java.awt.geom.AffineTransform
				transform.rotate(Math.toRadians(90))
				transform.translate(0, -(x + w))
				paintBackground(context, g, y, 0, h, w, transform)
			ElseIf orientation = JTabbedPane.BOTTOM Then
				Dim transform As New java.awt.geom.AffineTransform
				transform.translate(x,y)
				transform.scale(1, -1)
				transform.translate(0,-h)
				paintBackground(context, g, 0, 0, w, h, transform)
			Else
				paintBackground(context, g, x, y, w, h, Nothing)
			End If
		End Sub

		''' <summary>
		''' Paints the border of a tab of a tabbed pane.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		''' <param name="tabIndex"> Index of tab being painted. </param>
		Public Overridable Sub paintTabbedPaneTabBorder(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal tabIndex As Integer)
			paintBorder(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the border of a tab of a tabbed pane. This implementation invokes
		''' the method of the same name without the orientation.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		''' <param name="tabIndex"> Index of tab being painted. </param>
		''' <param name="orientation"> One of <code>JTabbedPane.TOP</code>,
		'''                    <code>JTabbedPane.LEFT</code>,
		'''                    <code>JTabbedPane.BOTTOM</code>, or
		'''                    <code>JTabbedPane.RIGHT</code>
		''' @since 1.6 </param>
		Public Overridable Sub paintTabbedPaneTabBorder(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal tabIndex As Integer, ByVal orientation As Integer)
			paintBorder(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the background of the area that contains the content of the
		''' selected tab of a tabbed pane.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintTabbedPaneContentBackground(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBackground(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the border of the area that contains the content of the
		''' selected tab of a tabbed pane.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintTabbedPaneContentBorder(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBorder(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the background of the header of a table.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintTableHeaderBackground(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBackground(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the border of the header of a table.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintTableHeaderBorder(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBorder(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the background of a table.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintTableBackground(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBackground(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the border of a table.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintTableBorder(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBorder(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the background of a text area.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintTextAreaBackground(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBackground(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the border of a text area.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintTextAreaBorder(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBorder(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the background of a text pane.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintTextPaneBackground(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBackground(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the border of a text pane.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintTextPaneBorder(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBorder(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the background of a text field.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintTextFieldBackground(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			If context.component.componentOrientation.leftToRight Then
				paintBackground(context, g, x, y, w, h, Nothing)
			Else
				Dim transform As New java.awt.geom.AffineTransform
				transform.translate(x,y)
				transform.scale(-1, 1)
				transform.translate(-w,0)
				paintBackground(context, g, 0, 0, w, h, transform)
			End If
		End Sub

		''' <summary>
		''' Paints the border of a text field.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintTextFieldBorder(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			If context.component.componentOrientation.leftToRight Then
				paintBorder(context, g, x, y, w, h, Nothing)
			Else
				Dim transform As New java.awt.geom.AffineTransform
				transform.translate(x,y)
				transform.scale(-1, 1)
				transform.translate(-w,0)
				paintBorder(context, g, 0, 0, w, h, transform)
			End If
		End Sub

		''' <summary>
		''' Paints the background of a toggle button.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintToggleButtonBackground(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBackground(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the border of a toggle button.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintToggleButtonBorder(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBorder(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the background of a tool bar.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintToolBarBackground(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBackground(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the background of a tool bar. This implementation invokes the
		''' method of the same name without the orientation.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		''' <param name="orientation"> One of <code>JToolBar.HORIZONTAL</code> or
		'''                           <code>JToolBar.VERTICAL</code>
		''' @since 1.6 </param>
		Public Overridable Sub paintToolBarBackground(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
			paintBackground(context, g, x, y, w, h, orientation)
		End Sub

		''' <summary>
		''' Paints the border of a tool bar.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintToolBarBorder(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBorder(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the border of a tool bar. This implementation invokes the
		''' method of the same name without the orientation.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		''' <param name="orientation"> One of <code>JToolBar.HORIZONTAL</code> or
		'''                           <code>JToolBar.VERTICAL</code>
		''' @since 1.6 </param>
		Public Overridable Sub paintToolBarBorder(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
			paintBorder(context, g, x, y, w, h, orientation)
		End Sub

		''' <summary>
		''' Paints the background of the tool bar's content area.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintToolBarContentBackground(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBackground(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the background of the tool bar's content area. This implementation
		''' invokes the method of the same name without the orientation.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		''' <param name="orientation"> One of <code>JToolBar.HORIZONTAL</code> or
		'''                           <code>JToolBar.VERTICAL</code>
		''' @since 1.6 </param>
		Public Overridable Sub paintToolBarContentBackground(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
			paintBackground(context, g, x, y, w, h, orientation)
		End Sub

		''' <summary>
		''' Paints the border of the content area of a tool bar.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintToolBarContentBorder(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBorder(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the border of the content area of a tool bar. This implementation
		''' invokes the method of the same name without the orientation.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		''' <param name="orientation"> One of <code>JToolBar.HORIZONTAL</code> or
		'''                           <code>JToolBar.VERTICAL</code>
		''' @since 1.6 </param>
		Public Overridable Sub paintToolBarContentBorder(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
			paintBorder(context, g, x, y, w, h, orientation)
		End Sub

		''' <summary>
		''' Paints the background of the window containing the tool bar when it
		''' has been detached from its primary frame.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintToolBarDragWindowBackground(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBackground(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the background of the window containing the tool bar when it
		''' has been detached from its primary frame. This implementation invokes the
		''' method of the same name without the orientation.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		''' <param name="orientation"> One of <code>JToolBar.HORIZONTAL</code> or
		'''                           <code>JToolBar.VERTICAL</code>
		''' @since 1.6 </param>
		Public Overridable Sub paintToolBarDragWindowBackground(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
			paintBackground(context, g, x, y, w, h, orientation)
		End Sub

		''' <summary>
		''' Paints the border of the window containing the tool bar when it
		''' has been detached from it's primary frame.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintToolBarDragWindowBorder(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBorder(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the border of the window containing the tool bar when it
		''' has been detached from it's primary frame. This implementation invokes the
		''' method of the same name without the orientation.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		''' <param name="orientation"> One of <code>JToolBar.HORIZONTAL</code> or
		'''                           <code>JToolBar.VERTICAL</code>
		''' @since 1.6 </param>
		Public Overridable Sub paintToolBarDragWindowBorder(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer, ByVal orientation As Integer)
			paintBorder(context, g, x, y, w, h, orientation)
		End Sub

		''' <summary>
		''' Paints the background of a tool tip.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintToolTipBackground(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBackground(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the border of a tool tip.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintToolTipBorder(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBorder(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the background of a tree.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintTreeBackground(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBackground(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the border of a tree.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintTreeBorder(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBorder(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the background of the row containing a cell in a tree.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintTreeCellBackground(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBackground(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the border of the row containing a cell in a tree.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintTreeCellBorder(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBorder(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the focus indicator for a cell in a tree when it has focus.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintTreeCellFocus(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			'TODO
		End Sub

		''' <summary>
		''' Paints the background of the viewport.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintViewportBackground(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBackground(context, g, x, y, w, h, Nothing)
		End Sub

		''' <summary>
		''' Paints the border of a viewport.
		''' </summary>
		''' <param name="context"> SynthContext identifying the <code>JComponent</code> and
		'''        <code>Region</code> to paint to </param>
		''' <param name="g"> <code>Graphics</code> to paint to </param>
		''' <param name="x"> X coordinate of the area to paint to </param>
		''' <param name="y"> Y coordinate of the area to paint to </param>
		''' <param name="w"> Width of the area to paint to </param>
		''' <param name="h"> Height of the area to paint to </param>
		Public Overridable Sub paintViewportBorder(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			paintBorder(context, g, x, y, w, h, Nothing)
		End Sub
	End Class

End Namespace
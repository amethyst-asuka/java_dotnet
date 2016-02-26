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



	Friend NotInheritable Class TabbedPaneTabAreaPainter
		Inherits AbstractRegionPainter

		'package private integers representing the available states that
		'this painter will paint. These are used when creating a new instance
		'of TabbedPaneTabAreaPainter to determine which region/state is being painted
		'by that instance.
		Friend Const BACKGROUND_ENABLED As Integer = 1
		Friend Const BACKGROUND_DISABLED As Integer = 2
		Friend Const BACKGROUND_ENABLED_MOUSEOVER As Integer = 3
		Friend Const BACKGROUND_ENABLED_PRESSED As Integer = 4


		Private ___state As Integer 'refers to one of the static final ints above
		Private ctx As PaintContext

		'the following 4 variables are reused during the painting code of the layers
		Private path As Path2D = New Path2D.Float
		Private rect As Rectangle2D = New Rectangle2D.Float(0, 0, 0, 0)
		Private roundRect As RoundRectangle2D = New RoundRectangle2D.Float(0, 0, 0, 0, 0, 0)
		Private ellipse As Ellipse2D = New Ellipse2D.Float(0, 0, 0, 0)

		'All Colors used for painting are stored here. Ideally, only those colors being used
		'by a particular instance of TabbedPaneTabAreaPainter would be created. For the moment at least,
		'however, all are created for each instance.
		Private color1 As New Color(255, 200, 0, 255)
		Private color2 As Color = decodeColor("nimbusBase", 0.08801502f, 0.3642857f, -0.4784314f, 0)
		Private color3 As Color = decodeColor("nimbusBase", 5.1498413E-4f, -0.45471883f, 0.31764704f, 0)
		Private color4 As Color = decodeColor("nimbusBase", 5.1498413E-4f, -0.4633005f, 0.3607843f, 0)
		Private color5 As Color = decodeColor("nimbusBase", 0.05468172f, -0.58308274f, 0.19607842f, 0)
		Private color6 As Color = decodeColor("nimbusBase", -0.57865167f, -0.6357143f, -0.54901963f, 0)
		Private color7 As Color = decodeColor("nimbusBase", 5.1498413E-4f, -0.4690476f, 0.39215684f, 0)
		Private color8 As Color = decodeColor("nimbusBase", 5.1498413E-4f, -0.47635174f, 0.4352941f, 0)
		Private color9 As Color = decodeColor("nimbusBase", 0.0f, -0.05401492f, 0.05098039f, 0)
		Private color10 As Color = decodeColor("nimbusBase", 0.0f, -0.09303135f, 0.09411764f, 0)


		'Array of current component colors, updated in each paint call
		Private componentColors As Object()

		Public Sub New(ByVal ctx As PaintContext, ByVal ___state As Integer)
			MyBase.New()
			Me.___state = ___state
			Me.ctx = ctx
		End Sub

		Protected Friend Overrides Sub doPaint(ByVal g As Graphics2D, ByVal c As JComponent, ByVal width As Integer, ByVal height As Integer, ByVal extendedCacheKeys As Object())
			'populate componentColors array with colors calculated in getExtendedCacheKeys call
			componentColors = extendedCacheKeys
			'generate this entire method. Each state/bg/fg/border combo that has
			'been painted gets its own KEY and paint method.
			Select Case ___state
				Case BACKGROUND_ENABLED
					paintBackgroundEnabled(g)
				Case BACKGROUND_DISABLED
					paintBackgroundDisabled(g)
				Case BACKGROUND_ENABLED_MOUSEOVER
					paintBackgroundEnabledAndMouseOver(g)
				Case BACKGROUND_ENABLED_PRESSED
					paintBackgroundEnabledAndPressed(g)

			End Select
		End Sub



		Protected Friend Property NotOverridable Overrides paintContext As PaintContext
			Get
				Return ctx
			End Get
		End Property

		Private Sub paintBackgroundEnabled(ByVal g As Graphics2D)
			rect = decodeRect1()
			g.paint = color1
			g.fill(rect)
			rect = decodeRect2()
			g.paint = decodeGradient1(rect)
			g.fill(rect)

		End Sub

		Private Sub paintBackgroundDisabled(ByVal g As Graphics2D)
			rect = decodeRect2()
			g.paint = decodeGradient2(rect)
			g.fill(rect)

		End Sub

		Private Sub paintBackgroundEnabledAndMouseOver(ByVal g As Graphics2D)
			rect = decodeRect2()
			g.paint = decodeGradient3(rect)
			g.fill(rect)

		End Sub

		Private Sub paintBackgroundEnabledAndPressed(ByVal g As Graphics2D)
			rect = decodeRect2()
			g.paint = decodeGradient4(rect)
			g.fill(rect)

		End Sub



		Private Function decodeRect1() As Rectangle2D
				rect.rectect(decodeX(0.0f), decodeY(1.0f), decodeX(0.0f) - decodeX(0.0f), decodeY(1.0f) - decodeY(1.0f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodeRect2() As Rectangle2D
				rect.rectect(decodeX(0.0f), decodeY(2.1666667f), decodeX(3.0f) - decodeX(0.0f), decodeY(3.0f) - decodeY(2.1666667f)) 'height - width - y - x
			Return rect
		End Function



		Private Function decodeGradient1(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.08387097f,0.09677419f,0.10967742f,0.43709677f,0.7645161f,0.7758064f,0.7870968f }, New Color() { color2, decodeColor(color2,color3,0.5f), color3, decodeColor(color3,color4,0.5f), color4, decodeColor(color4,color2,0.5f), color2})
		End Function

		Private Function decodeGradient2(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.08387097f,0.09677419f,0.10967742f,0.43709677f,0.7645161f,0.7758064f,0.7870968f }, New Color() { color5, decodeColor(color5,color3,0.5f), color3, decodeColor(color3,color4,0.5f), color4, decodeColor(color4,color5,0.5f), color5})
		End Function

		Private Function decodeGradient3(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.08387097f,0.09677419f,0.10967742f,0.43709677f,0.7645161f,0.7758064f,0.7870968f }, New Color() { color6, decodeColor(color6,color7,0.5f), color7, decodeColor(color7,color8,0.5f), color8, decodeColor(color8,color2,0.5f), color2})
		End Function

		Private Function decodeGradient4(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.08387097f,0.09677419f,0.10967742f,0.43709677f,0.7645161f,0.7758064f,0.7870968f }, New Color() { color2, decodeColor(color2,color9,0.5f), color9, decodeColor(color9,color10,0.5f), color10, decodeColor(color10,color2,0.5f), color2})
		End Function


	End Class

End Namespace
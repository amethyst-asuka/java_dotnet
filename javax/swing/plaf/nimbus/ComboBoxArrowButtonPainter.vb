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



	Friend NotInheritable Class ComboBoxArrowButtonPainter
		Inherits AbstractRegionPainter

		'package private integers representing the available states that
		'this painter will paint. These are used when creating a new instance
		'of ComboBoxArrowButtonPainter to determine which region/state is being painted
		'by that instance.
		Friend Const BACKGROUND_DISABLED As Integer = 1
		Friend Const BACKGROUND_ENABLED As Integer = 2
		Friend Const BACKGROUND_ENABLED_MOUSEOVER As Integer = 3
		Friend Const BACKGROUND_ENABLED_PRESSED As Integer = 4
		Friend Const BACKGROUND_DISABLED_EDITABLE As Integer = 5
		Friend Const BACKGROUND_ENABLED_EDITABLE As Integer = 6
		Friend Const BACKGROUND_MOUSEOVER_EDITABLE As Integer = 7
		Friend Const BACKGROUND_PRESSED_EDITABLE As Integer = 8
		Friend Const BACKGROUND_SELECTED_EDITABLE As Integer = 9
		Friend Const FOREGROUND_ENABLED As Integer = 10
		Friend Const FOREGROUND_MOUSEOVER As Integer = 11
		Friend Const FOREGROUND_DISABLED As Integer = 12
		Friend Const FOREGROUND_PRESSED As Integer = 13
		Friend Const FOREGROUND_SELECTED As Integer = 14


		Private state As Integer 'refers to one of the static final ints above
		Private ctx As PaintContext

		'the following 4 variables are reused during the painting code of the layers
		Private path As Path2D = New Path2D.Float
		Private rect As Rectangle2D = New Rectangle2D.Float(0, 0, 0, 0)
		Private roundRect As RoundRectangle2D = New RoundRectangle2D.Float(0, 0, 0, 0, 0, 0)
		Private ellipse As Ellipse2D = New Ellipse2D.Float(0, 0, 0, 0)

		'All Colors used for painting are stored here. Ideally, only those colors being used
		'by a particular instance of ComboBoxArrowButtonPainter would be created. For the moment at least,
		'however, all are created for each instance.
		Private color1 As Color = decodeColor("nimbusBlueGrey", -0.6111111f, -0.110526316f, -0.74509805f, -247)
		Private color2 As Color = decodeColor("nimbusBase", 0.021348298f, -0.56289876f, 0.2588235f, 0)
		Private color3 As Color = decodeColor("nimbusBase", 0.010237217f, -0.55799407f, 0.20784312f, 0)
		Private color4 As New Color(255, 200, 0, 255)
		Private color5 As Color = decodeColor("nimbusBase", 0.021348298f, -0.59223604f, 0.35294116f, 0)
		Private color6 As Color = decodeColor("nimbusBase", 0.02391243f, -0.5774183f, 0.32549018f, 0)
		Private color7 As Color = decodeColor("nimbusBase", 0.021348298f, -0.56722116f, 0.3098039f, 0)
		Private color8 As Color = decodeColor("nimbusBase", 0.021348298f, -0.567841f, 0.31764704f, 0)
		Private color9 As Color = decodeColor("nimbusBlueGrey", -0.6111111f, -0.110526316f, -0.74509805f, -191)
		Private color10 As Color = decodeColor("nimbusBase", 5.1498413E-4f, -0.34585923f, -0.007843137f, 0)
		Private color11 As Color = decodeColor("nimbusBase", 5.1498413E-4f, -0.095173776f, -0.25882354f, 0)
		Private color12 As Color = decodeColor("nimbusBase", 0.004681647f, -0.6197143f, 0.43137252f, 0)
		Private color13 As Color = decodeColor("nimbusBase", 0.0023007393f, -0.46825016f, 0.27058822f, 0)
		Private color14 As Color = decodeColor("nimbusBase", 5.1498413E-4f, -0.43866998f, 0.24705881f, 0)
		Private color15 As Color = decodeColor("nimbusBase", 5.1498413E-4f, -0.4625541f, 0.35686272f, 0)
		Private color16 As Color = decodeColor("nimbusBase", 0.0013483167f, -0.1769987f, -0.12156865f, 0)
		Private color17 As Color = decodeColor("nimbusBase", 0.059279382f, 0.3642857f, -0.43529415f, 0)
		Private color18 As Color = decodeColor("nimbusBase", 0.004681647f, -0.6198413f, 0.43921566f, 0)
		Private color19 As Color = decodeColor("nimbusBase", 0.0023007393f, -0.48084703f, 0.33725488f, 0)
		Private color20 As Color = decodeColor("nimbusBase", 5.1498413E-4f, -0.4555341f, 0.3215686f, 0)
		Private color21 As Color = decodeColor("nimbusBase", 5.1498413E-4f, -0.4757143f, 0.43137252f, 0)
		Private color22 As Color = decodeColor("nimbusBase", -0.57865167f, -0.6357143f, -0.54901963f, 0)
		Private color23 As Color = decodeColor("nimbusBase", -3.528595E-5f, 0.018606722f, -0.23137257f, 0)
		Private color24 As Color = decodeColor("nimbusBase", -4.2033195E-4f, -0.38050595f, 0.20392156f, 0)
		Private color25 As Color = decodeColor("nimbusBase", 7.13408E-4f, -0.064285696f, 0.027450979f, 0)
		Private color26 As Color = decodeColor("nimbusBase", 0.0f, -0.00895375f, 0.007843137f, 0)
		Private color27 As Color = decodeColor("nimbusBase", 8.9377165E-4f, -0.13853917f, 0.14509803f, 0)
		Private color28 As Color = decodeColor("nimbusBase", -0.57865167f, -0.6357143f, -0.37254906f, 0)
		Private color29 As Color = decodeColor("nimbusBase", -0.57865167f, -0.6357143f, -0.5254902f, 0)
		Private color30 As Color = decodeColor("nimbusBase", 0.027408898f, -0.57391655f, 0.1490196f, 0)
		Private color31 As Color = decodeColor("nimbusBase", 0.0f, -0.6357143f, 0.45098037f, 0)


		'Array of current component colors, updated in each paint call
		Private componentColors As Object()

		Public Sub New(ByVal ctx As PaintContext, ByVal ___state As Integer)
			MyBase.New()
			Me.state = ___state
			Me.ctx = ctx
		End Sub

		Protected Friend Overrides Sub doPaint(ByVal g As Graphics2D, ByVal c As JComponent, ByVal width As Integer, ByVal height As Integer, ByVal extendedCacheKeys As Object())
			'populate componentColors array with colors calculated in getExtendedCacheKeys call
			componentColors = extendedCacheKeys
			'generate this entire method. Each state/bg/fg/border combo that has
			'been painted gets its own KEY and paint method.
			Select Case state
				Case BACKGROUND_DISABLED_EDITABLE
					paintBackgroundDisabledAndEditable(g)
				Case BACKGROUND_ENABLED_EDITABLE
					paintBackgroundEnabledAndEditable(g)
				Case BACKGROUND_MOUSEOVER_EDITABLE
					paintBackgroundMouseOverAndEditable(g)
				Case BACKGROUND_PRESSED_EDITABLE
					paintBackgroundPressedAndEditable(g)
				Case BACKGROUND_SELECTED_EDITABLE
					paintBackgroundSelectedAndEditable(g)
				Case FOREGROUND_ENABLED
					paintForegroundEnabled(g)
				Case FOREGROUND_MOUSEOVER
					paintForegroundMouseOver(g)
				Case FOREGROUND_DISABLED
					paintForegroundDisabled(g)
				Case FOREGROUND_PRESSED
					paintForegroundPressed(g)
				Case FOREGROUND_SELECTED
					paintForegroundSelected(g)

			End Select
		End Sub



		Protected Friend Property NotOverridable Overrides paintContext As PaintContext
			Get
				Return ctx
			End Get
		End Property

		Private Sub paintBackgroundDisabledAndEditable(ByVal g As Graphics2D)
			path = decodePath1()
			g.paint = color1
			g.fill(path)
			path = decodePath2()
			g.paint = decodeGradient1(path)
			g.fill(path)
			path = decodePath3()
			g.paint = color4
			g.fill(path)
			path = decodePath4()
			g.paint = decodeGradient2(path)
			g.fill(path)

		End Sub

		Private Sub paintBackgroundEnabledAndEditable(ByVal g As Graphics2D)
			path = decodePath1()
			g.paint = color9
			g.fill(path)
			path = decodePath2()
			g.paint = decodeGradient3(path)
			g.fill(path)
			path = decodePath3()
			g.paint = color4
			g.fill(path)
			path = decodePath4()
			g.paint = decodeGradient4(path)
			g.fill(path)

		End Sub

		Private Sub paintBackgroundMouseOverAndEditable(ByVal g As Graphics2D)
			path = decodePath1()
			g.paint = color9
			g.fill(path)
			path = decodePath2()
			g.paint = decodeGradient5(path)
			g.fill(path)
			path = decodePath3()
			g.paint = color4
			g.fill(path)
			path = decodePath4()
			g.paint = decodeGradient6(path)
			g.fill(path)

		End Sub

		Private Sub paintBackgroundPressedAndEditable(ByVal g As Graphics2D)
			path = decodePath1()
			g.paint = color9
			g.fill(path)
			path = decodePath2()
			g.paint = decodeGradient7(path)
			g.fill(path)
			path = decodePath3()
			g.paint = color4
			g.fill(path)
			path = decodePath4()
			g.paint = decodeGradient8(path)
			g.fill(path)

		End Sub

		Private Sub paintBackgroundSelectedAndEditable(ByVal g As Graphics2D)
			path = decodePath1()
			g.paint = color9
			g.fill(path)
			path = decodePath2()
			g.paint = decodeGradient7(path)
			g.fill(path)
			path = decodePath3()
			g.paint = color4
			g.fill(path)
			path = decodePath4()
			g.paint = decodeGradient8(path)
			g.fill(path)

		End Sub

		Private Sub paintForegroundEnabled(ByVal g As Graphics2D)
			path = decodePath5()
			g.paint = decodeGradient9(path)
			g.fill(path)

		End Sub

		Private Sub paintForegroundMouseOver(ByVal g As Graphics2D)
			path = decodePath6()
			g.paint = decodeGradient9(path)
			g.fill(path)

		End Sub

		Private Sub paintForegroundDisabled(ByVal g As Graphics2D)
			path = decodePath7()
			g.paint = color30
			g.fill(path)

		End Sub

		Private Sub paintForegroundPressed(ByVal g As Graphics2D)
			path = decodePath8()
			g.paint = color31
			g.fill(path)

		End Sub

		Private Sub paintForegroundSelected(ByVal g As Graphics2D)
			path = decodePath7()
			g.paint = color31
			g.fill(path)

		End Sub



		Private Function decodePath1() As Path2D
			path.reset()
			path.moveTo(decodeX(0.0f), decodeY(2.0f))
			path.lineTo(decodeX(2.75f), decodeY(2.0f))
			path.lineTo(decodeX(2.75f), decodeY(2.25f))
			path.curveTo(decodeAnchorX(2.75f, 0.0f), decodeAnchorY(2.25f, 4.0f), decodeAnchorX(2.125f, 3.0f), decodeAnchorY(2.875f, 0.0f), decodeX(2.125f), decodeY(2.875f))
			path.lineTo(decodeX(0.0f), decodeY(2.875f))
			path.lineTo(decodeX(0.0f), decodeY(2.0f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath2() As Path2D
			path.reset()
			path.moveTo(decodeX(0.0f), decodeY(0.25f))
			path.lineTo(decodeX(2.125f), decodeY(0.25f))
			path.curveTo(decodeAnchorX(2.125f, 3.0f), decodeAnchorY(0.25f, 0.0f), decodeAnchorX(2.75f, 0.0f), decodeAnchorY(0.875f, -3.0f), decodeX(2.75f), decodeY(0.875f))
			path.lineTo(decodeX(2.75f), decodeY(2.125f))
			path.curveTo(decodeAnchorX(2.75f, 0.0f), decodeAnchorY(2.125f, 3.0f), decodeAnchorX(2.125f, 3.0f), decodeAnchorY(2.75f, 0.0f), decodeX(2.125f), decodeY(2.75f))
			path.lineTo(decodeX(0.0f), decodeY(2.75f))
			path.lineTo(decodeX(0.0f), decodeY(0.25f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath3() As Path2D
			path.reset()
			path.moveTo(decodeX(0.85294116f), decodeY(2.639706f))
			path.lineTo(decodeX(0.85294116f), decodeY(2.639706f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath4() As Path2D
			path.reset()
			path.moveTo(decodeX(1.0f), decodeY(0.375f))
			path.lineTo(decodeX(2.0f), decodeY(0.375f))
			path.curveTo(decodeAnchorX(2.0f, 4.0f), decodeAnchorY(0.375f, 0.0f), decodeAnchorX(2.625f, 0.0f), decodeAnchorY(1.0f, -4.0f), decodeX(2.625f), decodeY(1.0f))
			path.lineTo(decodeX(2.625f), decodeY(2.0f))
			path.curveTo(decodeAnchorX(2.625f, 0.0f), decodeAnchorY(2.0f, 4.0f), decodeAnchorX(2.0f, 4.0f), decodeAnchorY(2.625f, 0.0f), decodeX(2.0f), decodeY(2.625f))
			path.lineTo(decodeX(1.0f), decodeY(2.625f))
			path.lineTo(decodeX(1.0f), decodeY(0.375f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath5() As Path2D
			path.reset()
			path.moveTo(decodeX(0.9995915f), decodeY(1.3616071f))
			path.lineTo(decodeX(2.0f), decodeY(0.8333333f))
			path.lineTo(decodeX(2.0f), decodeY(1.8571429f))
			path.lineTo(decodeX(0.9995915f), decodeY(1.3616071f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath6() As Path2D
			path.reset()
			path.moveTo(decodeX(1.00625f), decodeY(1.3526785f))
			path.lineTo(decodeX(2.0f), decodeY(0.8333333f))
			path.lineTo(decodeX(2.0f), decodeY(1.8571429f))
			path.lineTo(decodeX(1.00625f), decodeY(1.3526785f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath7() As Path2D
			path.reset()
			path.moveTo(decodeX(1.0117648f), decodeY(1.3616071f))
			path.lineTo(decodeX(2.0f), decodeY(0.8333333f))
			path.lineTo(decodeX(2.0f), decodeY(1.8571429f))
			path.lineTo(decodeX(1.0117648f), decodeY(1.3616071f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath8() As Path2D
			path.reset()
			path.moveTo(decodeX(1.0242647f), decodeY(1.3526785f))
			path.lineTo(decodeX(2.0f), decodeY(0.8333333f))
			path.lineTo(decodeX(2.0f), decodeY(1.8571429f))
			path.lineTo(decodeX(1.0242647f), decodeY(1.3526785f))
			path.closePath()
			Return path
		End Function



		Private Function decodeGradient1(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color2, decodeColor(color2,color3,0.5f), color3})
		End Function

		Private Function decodeGradient2(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.171875f,0.34375f,0.4815341f,0.6193182f,0.8096591f,1.0f }, New Color() { color5, decodeColor(color5,color6,0.5f), color6, decodeColor(color6,color7,0.5f), color7, decodeColor(color7,color8,0.5f), color8})
		End Function

		Private Function decodeGradient3(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color10, decodeColor(color10,color11,0.5f), color11})
		End Function

		Private Function decodeGradient4(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.12299465f,0.44652405f,0.5441176f,0.64171124f,0.8208556f,1.0f }, New Color() { color12, decodeColor(color12,color13,0.5f), color13, decodeColor(color13,color14,0.5f), color14, decodeColor(color14,color15,0.5f), color15})
		End Function

		Private Function decodeGradient5(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color16, decodeColor(color16,color17,0.5f), color17})
		End Function

		Private Function decodeGradient6(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.12299465f,0.44652405f,0.5441176f,0.64171124f,0.81283426f,0.98395723f }, New Color() { color18, decodeColor(color18,color19,0.5f), color19, decodeColor(color19,color20,0.5f), color20, decodeColor(color20,color21,0.5f), color21})
		End Function

		Private Function decodeGradient7(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color22, decodeColor(color22,color23,0.5f), color23})
		End Function

		Private Function decodeGradient8(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.12299465f,0.44652405f,0.5441176f,0.64171124f,0.8208556f,1.0f }, New Color() { color24, decodeColor(color24,color25,0.5f), color25, decodeColor(color25,color26,0.5f), color26, decodeColor(color26,color27,0.5f), color27})
		End Function

		Private Function decodeGradient9(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((1.0f * w) + x, (0.5f * h) + y, (0.0f * w) + x, (0.5f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color28, decodeColor(color28,color29,0.5f), color29})
		End Function


	End Class

End Namespace
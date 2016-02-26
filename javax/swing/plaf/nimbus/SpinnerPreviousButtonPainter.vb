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



	Friend NotInheritable Class SpinnerPreviousButtonPainter
		Inherits AbstractRegionPainter

		'package private integers representing the available states that
		'this painter will paint. These are used when creating a new instance
		'of SpinnerPreviousButtonPainter to determine which region/state is being painted
		'by that instance.
		Friend Const BACKGROUND_DISABLED As Integer = 1
		Friend Const BACKGROUND_ENABLED As Integer = 2
		Friend Const BACKGROUND_FOCUSED As Integer = 3
		Friend Const BACKGROUND_MOUSEOVER_FOCUSED As Integer = 4
		Friend Const BACKGROUND_PRESSED_FOCUSED As Integer = 5
		Friend Const BACKGROUND_MOUSEOVER As Integer = 6
		Friend Const BACKGROUND_PRESSED As Integer = 7
		Friend Const FOREGROUND_DISABLED As Integer = 8
		Friend Const FOREGROUND_ENABLED As Integer = 9
		Friend Const FOREGROUND_FOCUSED As Integer = 10
		Friend Const FOREGROUND_MOUSEOVER_FOCUSED As Integer = 11
		Friend Const FOREGROUND_PRESSED_FOCUSED As Integer = 12
		Friend Const FOREGROUND_MOUSEOVER As Integer = 13
		Friend Const FOREGROUND_PRESSED As Integer = 14


		Private state As Integer 'refers to one of the static final ints above
		Private ctx As PaintContext

		'the following 4 variables are reused during the painting code of the layers
		Private path As Path2D = New Path2D.Float
		Private rect As Rectangle2D = New Rectangle2D.Float(0, 0, 0, 0)
		Private roundRect As RoundRectangle2D = New RoundRectangle2D.Float(0, 0, 0, 0, 0, 0)
		Private ellipse As Ellipse2D = New Ellipse2D.Float(0, 0, 0, 0)

		'All Colors used for painting are stored here. Ideally, only those colors being used
		'by a particular instance of SpinnerPreviousButtonPainter would be created. For the moment at least,
		'however, all are created for each instance.
		Private color1 As Color = decodeColor("nimbusBase", 0.015098333f, -0.5557143f, 0.2352941f, 0)
		Private color2 As Color = decodeColor("nimbusBase", 0.010237217f, -0.55799407f, 0.20784312f, 0)
		Private color3 As Color = decodeColor("nimbusBase", 0.018570602f, -0.5821429f, 0.32941175f, 0)
		Private color4 As Color = decodeColor("nimbusBase", 0.021348298f, -0.56722116f, 0.3098039f, 0)
		Private color5 As Color = decodeColor("nimbusBase", 0.021348298f, -0.567841f, 0.31764704f, 0)
		Private color6 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.0033834577f, -0.30588236f, -148)
		Private color7 As Color = decodeColor("nimbusBase", 5.1498413E-4f, -0.2583558f, -0.13333336f, 0)
		Private color8 As Color = decodeColor("nimbusBase", 5.1498413E-4f, -0.095173776f, -0.25882354f, 0)
		Private color9 As Color = decodeColor("nimbusBase", 0.004681647f, -0.5383692f, 0.33725488f, 0)
		Private color10 As Color = decodeColor("nimbusBase", -0.0017285943f, -0.44453782f, 0.25098038f, 0)
		Private color11 As Color = decodeColor("nimbusBase", 5.1498413E-4f, -0.43866998f, 0.24705881f, 0)
		Private color12 As Color = decodeColor("nimbusBase", 5.1498413E-4f, -0.4625541f, 0.35686272f, 0)
		Private color13 As Color = decodeColor("nimbusFocus", 0.0f, 0.0f, 0.0f, 0)
		Private color14 As Color = decodeColor("nimbusBase", 0.0013483167f, 0.088923395f, -0.2784314f, 0)
		Private color15 As Color = decodeColor("nimbusBase", 0.059279382f, 0.3642857f, -0.43529415f, 0)
		Private color16 As Color = decodeColor("nimbusBase", 0.0010585189f, -0.541452f, 0.4078431f, 0)
		Private color17 As Color = decodeColor("nimbusBase", 0.00254488f, -0.4608264f, 0.32549018f, 0)
		Private color18 As Color = decodeColor("nimbusBase", 5.1498413E-4f, -0.4555341f, 0.3215686f, 0)
		Private color19 As Color = decodeColor("nimbusBase", 5.1498413E-4f, -0.4757143f, 0.43137252f, 0)
		Private color20 As Color = decodeColor("nimbusBase", 0.061133325f, 0.3642857f, -0.427451f, 0)
		Private color21 As Color = decodeColor("nimbusBase", -3.528595E-5f, 0.018606722f, -0.23137257f, 0)
		Private color22 As Color = decodeColor("nimbusBase", 8.354783E-4f, -0.2578073f, 0.12549019f, 0)
		Private color23 As Color = decodeColor("nimbusBase", 8.9377165E-4f, -0.01599598f, 0.007843137f, 0)
		Private color24 As Color = decodeColor("nimbusBase", 0.0f, -0.00895375f, 0.007843137f, 0)
		Private color25 As Color = decodeColor("nimbusBase", 8.9377165E-4f, -0.13853917f, 0.14509803f, 0)
		Private color26 As Color = decodeColor("nimbusBlueGrey", -0.6111111f, -0.110526316f, -0.63529414f, -179)
		Private color27 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.110526316f, 0.25490195f, -186)
		Private color28 As Color = decodeColor("nimbusBase", 0.018570602f, -0.56714284f, 0.1372549f, 0)
		Private color29 As Color = decodeColor("nimbusBase", -0.57865167f, -0.6357143f, -0.54901963f, 0)
		Private color30 As Color = decodeColor("nimbusBase", 0.0f, -0.6357143f, 0.45098037f, 0)


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
				Case BACKGROUND_DISABLED
					paintBackgroundDisabled(g)
				Case BACKGROUND_ENABLED
					paintBackgroundEnabled(g)
				Case BACKGROUND_FOCUSED
					paintBackgroundFocused(g)
				Case BACKGROUND_MOUSEOVER_FOCUSED
					paintBackgroundMouseOverAndFocused(g)
				Case BACKGROUND_PRESSED_FOCUSED
					paintBackgroundPressedAndFocused(g)
				Case BACKGROUND_MOUSEOVER
					paintBackgroundMouseOver(g)
				Case BACKGROUND_PRESSED
					paintBackgroundPressed(g)
				Case FOREGROUND_DISABLED
					paintForegroundDisabled(g)
				Case FOREGROUND_ENABLED
					paintForegroundEnabled(g)
				Case FOREGROUND_FOCUSED
					paintForegroundFocused(g)
				Case FOREGROUND_MOUSEOVER_FOCUSED
					paintForegroundMouseOverAndFocused(g)
				Case FOREGROUND_PRESSED_FOCUSED
					paintForegroundPressedAndFocused(g)
				Case FOREGROUND_MOUSEOVER
					paintForegroundMouseOver(g)
				Case FOREGROUND_PRESSED
					paintForegroundPressed(g)

			End Select
		End Sub



		Protected Friend Property NotOverridable Overrides paintContext As PaintContext
			Get
				Return ctx
			End Get
		End Property

		Private Sub paintBackgroundDisabled(ByVal g As Graphics2D)
			path = decodePath1()
			g.paint = decodeGradient1(path)
			g.fill(path)
			path = decodePath2()
			g.paint = decodeGradient2(path)
			g.fill(path)

		End Sub

		Private Sub paintBackgroundEnabled(ByVal g As Graphics2D)
			path = decodePath3()
			g.paint = color6
			g.fill(path)
			path = decodePath1()
			g.paint = decodeGradient3(path)
			g.fill(path)
			path = decodePath2()
			g.paint = decodeGradient4(path)
			g.fill(path)

		End Sub

		Private Sub paintBackgroundFocused(ByVal g As Graphics2D)
			path = decodePath4()
			g.paint = color13
			g.fill(path)
			path = decodePath1()
			g.paint = decodeGradient3(path)
			g.fill(path)
			path = decodePath2()
			g.paint = decodeGradient4(path)
			g.fill(path)

		End Sub

		Private Sub paintBackgroundMouseOverAndFocused(ByVal g As Graphics2D)
			path = decodePath5()
			g.paint = color13
			g.fill(path)
			path = decodePath6()
			g.paint = decodeGradient5(path)
			g.fill(path)
			path = decodePath7()
			g.paint = decodeGradient6(path)
			g.fill(path)

		End Sub

		Private Sub paintBackgroundPressedAndFocused(ByVal g As Graphics2D)
			path = decodePath4()
			g.paint = color13
			g.fill(path)
			path = decodePath1()
			g.paint = decodeGradient7(path)
			g.fill(path)
			path = decodePath2()
			g.paint = decodeGradient8(path)
			g.fill(path)

		End Sub

		Private Sub paintBackgroundMouseOver(ByVal g As Graphics2D)
			path = decodePath3()
			g.paint = color26
			g.fill(path)
			path = decodePath1()
			g.paint = decodeGradient5(path)
			g.fill(path)
			path = decodePath2()
			g.paint = decodeGradient6(path)
			g.fill(path)

		End Sub

		Private Sub paintBackgroundPressed(ByVal g As Graphics2D)
			path = decodePath8()
			g.paint = color27
			g.fill(path)
			path = decodePath1()
			g.paint = decodeGradient7(path)
			g.fill(path)
			path = decodePath2()
			g.paint = decodeGradient8(path)
			g.fill(path)

		End Sub

		Private Sub paintForegroundDisabled(ByVal g As Graphics2D)
			path = decodePath9()
			g.paint = color28
			g.fill(path)

		End Sub

		Private Sub paintForegroundEnabled(ByVal g As Graphics2D)
			path = decodePath9()
			g.paint = color29
			g.fill(path)

		End Sub

		Private Sub paintForegroundFocused(ByVal g As Graphics2D)
			path = decodePath9()
			g.paint = color29
			g.fill(path)

		End Sub

		Private Sub paintForegroundMouseOverAndFocused(ByVal g As Graphics2D)
			path = decodePath9()
			g.paint = color29
			g.fill(path)

		End Sub

		Private Sub paintForegroundPressedAndFocused(ByVal g As Graphics2D)
			path = decodePath9()
			g.paint = color30
			g.fill(path)

		End Sub

		Private Sub paintForegroundMouseOver(ByVal g As Graphics2D)
			path = decodePath9()
			g.paint = color29
			g.fill(path)

		End Sub

		Private Sub paintForegroundPressed(ByVal g As Graphics2D)
			path = decodePath9()
			g.paint = color30
			g.fill(path)

		End Sub



		Private Function decodePath1() As Path2D
			path.reset()
			path.moveTo(decodeX(0.0f), decodeY(1.0f))
			path.lineTo(decodeX(0.0f), decodeY(2.6666667f))
			path.lineTo(decodeX(2.142857f), decodeY(2.6666667f))
			path.curveTo(decodeAnchorX(2.142857074737549f, 3.0f), decodeAnchorY(2.6666667461395264f, 0.0f), decodeAnchorX(2.7142858505249023f, 0.0f), decodeAnchorY(2.0f, 2.0f), decodeX(2.7142859f), decodeY(2.0f))
			path.lineTo(decodeX(2.7142859f), decodeY(1.0f))
			path.lineTo(decodeX(0.0f), decodeY(1.0f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath2() As Path2D
			path.reset()
			path.moveTo(decodeX(1.0f), decodeY(1.0f))
			path.lineTo(decodeX(1.0f), decodeY(2.5f))
			path.lineTo(decodeX(2.142857f), decodeY(2.5f))
			path.curveTo(decodeAnchorX(2.142857074737549f, 2.0f), decodeAnchorY(2.5f, 0.0f), decodeAnchorX(2.5714285373687744f, 0.0f), decodeAnchorY(2.0f, 1.0f), decodeX(2.5714285f), decodeY(2.0f))
			path.lineTo(decodeX(2.5714285f), decodeY(1.0f))
			path.lineTo(decodeX(1.0f), decodeY(1.0f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath3() As Path2D
			path.reset()
			path.moveTo(decodeX(0.0f), decodeY(2.6666667f))
			path.lineTo(decodeX(0.0f), decodeY(2.8333333f))
			path.lineTo(decodeX(2.0324676f), decodeY(2.8333333f))
			path.curveTo(decodeAnchorX(2.0324676036834717f, 2.1136363636363793f), decodeAnchorY(2.8333332538604736f, 0.0f), decodeAnchorX(2.7142858505249023f, 0.0f), decodeAnchorY(2.0f, 3.0f), decodeX(2.7142859f), decodeY(2.0f))
			path.lineTo(decodeX(0.0f), decodeY(2.6666667f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath4() As Path2D
			path.reset()
			path.moveTo(decodeX(0.0f), decodeY(1.0f))
			path.lineTo(decodeX(0.0f), decodeY(2.8999999f))
			path.lineTo(decodeX(2.2f), decodeY(2.8999999f))
			path.curveTo(decodeAnchorX(2.200000047683716f, 2.9999999999999982f), decodeAnchorY(2.8999998569488525f, 0.0f), decodeAnchorX(2.914285659790039f, 0.0f), decodeAnchorY(2.2333333492279053f, 3.0f), decodeX(2.9142857f), decodeY(2.2333333f))
			path.lineTo(decodeX(2.9142857f), decodeY(1.0f))
			path.lineTo(decodeX(0.0f), decodeY(1.0f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath5() As Path2D
			path.reset()
			path.moveTo(decodeX(0.0f), decodeY(0.0f))
			path.lineTo(decodeX(0.0f), decodeY(2.8999999f))
			path.lineTo(decodeX(2.2f), decodeY(2.8999999f))
			path.curveTo(decodeAnchorX(2.200000047683716f, 2.9999999999999982f), decodeAnchorY(2.8999998569488525f, 0.0f), decodeAnchorX(2.914285659790039f, 0.0f), decodeAnchorY(2.2333333492279053f, 3.0f), decodeX(2.9142857f), decodeY(2.2333333f))
			path.lineTo(decodeX(2.9142857f), decodeY(0.0f))
			path.lineTo(decodeX(0.0f), decodeY(0.0f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath6() As Path2D
			path.reset()
			path.moveTo(decodeX(0.0f), decodeY(0.0f))
			path.lineTo(decodeX(0.0f), decodeY(2.6666667f))
			path.lineTo(decodeX(2.142857f), decodeY(2.6666667f))
			path.curveTo(decodeAnchorX(2.142857074737549f, 3.0f), decodeAnchorY(2.6666667461395264f, 0.0f), decodeAnchorX(2.7142858505249023f, 0.0f), decodeAnchorY(2.0f, 2.0f), decodeX(2.7142859f), decodeY(2.0f))
			path.lineTo(decodeX(2.7142859f), decodeY(0.0f))
			path.lineTo(decodeX(0.0f), decodeY(0.0f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath7() As Path2D
			path.reset()
			path.moveTo(decodeX(1.0f), decodeY(0.0f))
			path.lineTo(decodeX(1.0f), decodeY(2.5f))
			path.lineTo(decodeX(2.142857f), decodeY(2.5f))
			path.curveTo(decodeAnchorX(2.142857074737549f, 2.0f), decodeAnchorY(2.5f, 0.0f), decodeAnchorX(2.5714285373687744f, 0.0f), decodeAnchorY(2.0f, 1.0f), decodeX(2.5714285f), decodeY(2.0f))
			path.lineTo(decodeX(2.5714285f), decodeY(0.0f))
			path.lineTo(decodeX(1.0f), decodeY(0.0f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath8() As Path2D
			path.reset()
			path.moveTo(decodeX(0.0f), decodeY(2.6666667f))
			path.lineTo(decodeX(0.0f), decodeY(2.8333333f))
			path.curveTo(decodeAnchorX(0.0f, 0.0f), decodeAnchorY(2.8333332538604736f, 0.0f), decodeAnchorX(2.0324676036834717f, -2.1136363636363793f), decodeAnchorY(2.8333332538604736f, 0.0f), decodeX(2.0324676f), decodeY(2.8333333f))
			path.curveTo(decodeAnchorX(2.0324676036834717f, 2.1136363636363793f), decodeAnchorY(2.8333332538604736f, 0.0f), decodeAnchorX(2.7142858505249023f, 0.0f), decodeAnchorY(2.0f, 3.0f), decodeX(2.7142859f), decodeY(2.0f))
			path.lineTo(decodeX(0.0f), decodeY(2.6666667f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath9() As Path2D
			path.reset()
			path.moveTo(decodeX(1.0f), decodeY(1.0f))
			path.lineTo(decodeX(1.5045455f), decodeY(1.9943181f))
			path.lineTo(decodeX(2.0f), decodeY(1.0f))
			path.lineTo(decodeX(1.0f), decodeY(1.0f))
			path.closePath()
			Return path
		End Function



		Private Function decodeGradient1(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color1, decodeColor(color1,color2,0.5f), color2})
		End Function

		Private Function decodeGradient2(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.05748663f,0.11497326f,0.55748665f,1.0f }, New Color() { color3, decodeColor(color3,color4,0.5f), color4, decodeColor(color4,color5,0.5f), color5})
		End Function

		Private Function decodeGradient3(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color7, decodeColor(color7,color8,0.5f), color8})
		End Function

		Private Function decodeGradient4(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.05748663f,0.11497326f,0.2419786f,0.36898395f,0.684492f,1.0f }, New Color() { color9, decodeColor(color9,color10,0.5f), color10, decodeColor(color10,color11,0.5f), color11, decodeColor(color11,color12,0.5f), color12})
		End Function

		Private Function decodeGradient5(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color14, decodeColor(color14,color15,0.5f), color15})
		End Function

		Private Function decodeGradient6(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.05748663f,0.11497326f,0.2419786f,0.36898395f,0.684492f,1.0f }, New Color() { color16, decodeColor(color16,color17,0.5f), color17, decodeColor(color17,color18,0.5f), color18, decodeColor(color18,color19,0.5f), color19})
		End Function

		Private Function decodeGradient7(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color20, decodeColor(color20,color21,0.5f), color21})
		End Function

		Private Function decodeGradient8(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.05748663f,0.11497326f,0.2419786f,0.36898395f,0.684492f,1.0f }, New Color() { color22, decodeColor(color22,color23,0.5f), color23, decodeColor(color23,color24,0.5f), color24, decodeColor(color24,color25,0.5f), color25})
		End Function


	End Class

End Namespace
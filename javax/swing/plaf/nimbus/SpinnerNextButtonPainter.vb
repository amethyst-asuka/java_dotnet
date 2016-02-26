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



	Friend NotInheritable Class SpinnerNextButtonPainter
		Inherits AbstractRegionPainter

		'package private integers representing the available states that
		'this painter will paint. These are used when creating a new instance
		'of SpinnerNextButtonPainter to determine which region/state is being painted
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
		'by a particular instance of SpinnerNextButtonPainter would be created. For the moment at least,
		'however, all are created for each instance.
		Private color1 As Color = decodeColor("nimbusBase", 0.021348298f, -0.56289876f, 0.2588235f, 0)
		Private color2 As Color = decodeColor("nimbusBase", 0.010237217f, -0.5607143f, 0.2352941f, 0)
		Private color3 As Color = decodeColor("nimbusBase", 0.021348298f, -0.59223604f, 0.35294116f, 0)
		Private color4 As Color = decodeColor("nimbusBase", 0.016586483f, -0.5723659f, 0.31764704f, 0)
		Private color5 As Color = decodeColor("nimbusBase", 0.021348298f, -0.56182265f, 0.24705881f, 0)
		Private color6 As Color = decodeColor("nimbusBase", 5.1498413E-4f, -0.34585923f, -0.007843137f, 0)
		Private color7 As Color = decodeColor("nimbusBase", 5.1498413E-4f, -0.27207792f, -0.11764708f, 0)
		Private color8 As Color = decodeColor("nimbusBase", 0.004681647f, -0.6197143f, 0.43137252f, 0)
		Private color9 As Color = decodeColor("nimbusBase", -0.0012707114f, -0.5078604f, 0.3098039f, 0)
		Private color10 As Color = decodeColor("nimbusBase", -0.0028941035f, -0.4800539f, 0.28235292f, 0)
		Private color11 As Color = decodeColor("nimbusBase", 0.0023007393f, -0.3622768f, -0.04705882f, 0)
		Private color12 As Color = decodeColor("nimbusFocus", 0.0f, 0.0f, 0.0f, 0)
		Private color13 As Color = decodeColor("nimbusBase", 0.0013483167f, -0.1769987f, -0.12156865f, 0)
		Private color14 As Color = decodeColor("nimbusBase", 0.0013483167f, 0.039961398f, -0.25882354f, 0)
		Private color15 As Color = decodeColor("nimbusBase", 0.004681647f, -0.6198413f, 0.43921566f, 0)
		Private color16 As Color = decodeColor("nimbusBase", -0.0012707114f, -0.51502466f, 0.3607843f, 0)
		Private color17 As Color = decodeColor("nimbusBase", 0.0021564364f, -0.49097747f, 0.34509802f, 0)
		Private color18 As Color = decodeColor("nimbusBase", 5.2034855E-5f, -0.38743842f, 0.019607842f, 0)
		Private color19 As Color = decodeColor("nimbusBase", -0.57865167f, -0.6357143f, -0.54901963f, 0)
		Private color20 As Color = decodeColor("nimbusBase", 0.08801502f, 0.3642857f, -0.454902f, 0)
		Private color21 As Color = decodeColor("nimbusBase", -4.2033195E-4f, -0.38050595f, 0.20392156f, 0)
		Private color22 As Color = decodeColor("nimbusBase", 2.9569864E-4f, -0.15470162f, 0.07058823f, 0)
		Private color23 As Color = decodeColor("nimbusBase", -4.6235323E-4f, -0.09571427f, 0.039215684f, 0)
		Private color24 As Color = decodeColor("nimbusBase", 0.018363237f, 0.18135887f, -0.227451f, 0)
		Private color25 As New Color(255, 200, 0, 255)
		Private color26 As Color = decodeColor("nimbusBase", 0.021348298f, -0.58106947f, 0.16862744f, 0)
		Private color27 As Color = decodeColor("nimbusBase", -0.57865167f, -0.6357143f, -0.043137252f, 0)
		Private color28 As Color = decodeColor("nimbusBase", -0.57865167f, -0.6357143f, -0.24313727f, 0)
		Private color29 As Color = decodeColor("nimbusBase", 0.0f, -0.6357143f, 0.45098037f, 0)


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
			rect = decodeRect1()
			g.paint = color5
			g.fill(rect)

		End Sub

		Private Sub paintBackgroundEnabled(ByVal g As Graphics2D)
			path = decodePath3()
			g.paint = decodeGradient3(path)
			g.fill(path)
			path = decodePath4()
			g.paint = decodeGradient4(path)
			g.fill(path)
			rect = decodeRect1()
			g.paint = color11
			g.fill(rect)

		End Sub

		Private Sub paintBackgroundFocused(ByVal g As Graphics2D)
			path = decodePath5()
			g.paint = color12
			g.fill(path)
			path = decodePath3()
			g.paint = decodeGradient3(path)
			g.fill(path)
			path = decodePath4()
			g.paint = decodeGradient5(path)
			g.fill(path)
			rect = decodeRect1()
			g.paint = color11
			g.fill(rect)

		End Sub

		Private Sub paintBackgroundMouseOverAndFocused(ByVal g As Graphics2D)
			path = decodePath5()
			g.paint = color12
			g.fill(path)
			path = decodePath3()
			g.paint = decodeGradient6(path)
			g.fill(path)
			path = decodePath4()
			g.paint = decodeGradient7(path)
			g.fill(path)
			rect = decodeRect1()
			g.paint = color18
			g.fill(rect)

		End Sub

		Private Sub paintBackgroundPressedAndFocused(ByVal g As Graphics2D)
			path = decodePath5()
			g.paint = color12
			g.fill(path)
			path = decodePath6()
			g.paint = decodeGradient8(path)
			g.fill(path)
			path = decodePath4()
			g.paint = decodeGradient9(path)
			g.fill(path)
			rect = decodeRect1()
			g.paint = color24
			g.fill(rect)

		End Sub

		Private Sub paintBackgroundMouseOver(ByVal g As Graphics2D)
			path = decodePath3()
			g.paint = decodeGradient6(path)
			g.fill(path)
			path = decodePath4()
			g.paint = decodeGradient10(path)
			g.fill(path)
			rect = decodeRect1()
			g.paint = color18
			g.fill(rect)

		End Sub

		Private Sub paintBackgroundPressed(ByVal g As Graphics2D)
			path = decodePath6()
			g.paint = decodeGradient8(path)
			g.fill(path)
			path = decodePath4()
			g.paint = decodeGradient11(path)
			g.fill(path)
			rect = decodeRect1()
			g.paint = color24
			g.fill(rect)
			rect = decodeRect2()
			g.paint = color25
			g.fill(rect)

		End Sub

		Private Sub paintForegroundDisabled(ByVal g As Graphics2D)
			path = decodePath7()
			g.paint = color26
			g.fill(path)

		End Sub

		Private Sub paintForegroundEnabled(ByVal g As Graphics2D)
			path = decodePath7()
			g.paint = decodeGradient12(path)
			g.fill(path)

		End Sub

		Private Sub paintForegroundFocused(ByVal g As Graphics2D)
			path = decodePath8()
			g.paint = decodeGradient12(path)
			g.fill(path)

		End Sub

		Private Sub paintForegroundMouseOverAndFocused(ByVal g As Graphics2D)
			path = decodePath8()
			g.paint = decodeGradient12(path)
			g.fill(path)

		End Sub

		Private Sub paintForegroundPressedAndFocused(ByVal g As Graphics2D)
			path = decodePath9()
			g.paint = color29
			g.fill(path)

		End Sub

		Private Sub paintForegroundMouseOver(ByVal g As Graphics2D)
			path = decodePath7()
			g.paint = decodeGradient12(path)
			g.fill(path)

		End Sub

		Private Sub paintForegroundPressed(ByVal g As Graphics2D)
			path = decodePath9()
			g.paint = color29
			g.fill(path)

		End Sub



		Private Function decodePath1() As Path2D
			path.reset()
			path.moveTo(decodeX(0.0f), decodeY(3.0f))
			path.lineTo(decodeX(0.0f), decodeY(0.2857143f))
			path.curveTo(decodeAnchorX(0.0f, 0.0f), decodeAnchorY(0.2857142984867096f, 0.0f), decodeAnchorX(2.0f, -3.6363636363636402f), decodeAnchorY(0.2857142984867096f, 0.0f), decodeX(2.0f), decodeY(0.2857143f))
			path.curveTo(decodeAnchorX(2.0f, 3.6363636363636402f), decodeAnchorY(0.2857142984867096f, 0.0f), decodeAnchorX(2.7142858505249023f, -0.022727272727273373f), decodeAnchorY(1.0f, -3.749999999999999f), decodeX(2.7142859f), decodeY(1.0f))
			path.curveTo(decodeAnchorX(2.7142858505249023f, 0.022727272727273373f), decodeAnchorY(1.0f, 3.75f), decodeAnchorX(2.7142858505249023f, 0.0f), decodeAnchorY(3.0f, 0.0f), decodeX(2.7142859f), decodeY(3.0f))
			path.lineTo(decodeX(0.0f), decodeY(3.0f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath2() As Path2D
			path.reset()
			path.moveTo(decodeX(1.0f), decodeY(2.0f))
			path.lineTo(decodeX(1.0f), decodeY(0.42857143f))
			path.curveTo(decodeAnchorX(1.0f, 0.0f), decodeAnchorY(0.4285714328289032f, 0.0f), decodeAnchorX(2.0f, -3.0f), decodeAnchorY(0.4285714328289032f, 0.0f), decodeX(2.0f), decodeY(0.42857143f))
			path.curveTo(decodeAnchorX(2.0f, 3.0f), decodeAnchorY(0.4285714328289032f, 0.0f), decodeAnchorX(2.5714285373687744f, 0.0f), decodeAnchorY(1.0f, -2.0f), decodeX(2.5714285f), decodeY(1.0f))
			path.curveTo(decodeAnchorX(2.5714285373687744f, 0.0f), decodeAnchorY(1.0f, 2.0f), decodeAnchorX(2.5714285373687744f, 0.0f), decodeAnchorY(2.0f, 0.0f), decodeX(2.5714285f), decodeY(2.0f))
			path.lineTo(decodeX(1.0f), decodeY(2.0f))
			path.closePath()
			Return path
		End Function

		Private Function decodeRect1() As Rectangle2D
				rect.rectect(decodeX(1.0f), decodeY(2.0f), decodeX(2.5714285f) - decodeX(1.0f), decodeY(3.0f) - decodeY(2.0f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodePath3() As Path2D
			path.reset()
			path.moveTo(decodeX(0.0f), decodeY(3.0f))
			path.lineTo(decodeX(0.0f), decodeY(0.2857143f))
			path.lineTo(decodeX(2.0f), decodeY(0.2857143f))
			path.curveTo(decodeAnchorX(2.0f, 3.6363636363636402f), decodeAnchorY(0.2857142984867096f, 0.0f), decodeAnchorX(2.7142858505249023f, -0.022727272727273373f), decodeAnchorY(1.0f, -3.749999999999999f), decodeX(2.7142859f), decodeY(1.0f))
			path.lineTo(decodeX(2.7142859f), decodeY(3.0f))
			path.lineTo(decodeX(0.0f), decodeY(3.0f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath4() As Path2D
			path.reset()
			path.moveTo(decodeX(1.0f), decodeY(2.0f))
			path.lineTo(decodeX(1.0f), decodeY(0.42857143f))
			path.lineTo(decodeX(2.0f), decodeY(0.42857143f))
			path.curveTo(decodeAnchorX(2.0f, 3.0f), decodeAnchorY(0.4285714328289032f, 0.0f), decodeAnchorX(2.5714285373687744f, 0.0f), decodeAnchorY(1.0f, -2.0f), decodeX(2.5714285f), decodeY(1.0f))
			path.lineTo(decodeX(2.5714285f), decodeY(2.0f))
			path.lineTo(decodeX(1.0f), decodeY(2.0f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath5() As Path2D
			path.reset()
			path.moveTo(decodeX(0.0f), decodeY(3.0f))
			path.lineTo(decodeX(0.0f), decodeY(0.08571429f))
			path.lineTo(decodeX(2.142857f), decodeY(0.08571429f))
			path.curveTo(decodeAnchorX(2.142857074737549f, 3.3999999999999986f), decodeAnchorY(0.08571428805589676f, 0.0f), decodeAnchorX(2.914285659790039f, 0.0f), decodeAnchorY(1.0f, -3.4f), decodeX(2.9142857f), decodeY(1.0f))
			path.lineTo(decodeX(2.9142857f), decodeY(3.0f))
			path.lineTo(decodeX(0.0f), decodeY(3.0f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath6() As Path2D
			path.reset()
			path.moveTo(decodeX(0.0f), decodeY(3.0f))
			path.lineTo(decodeX(0.0f), decodeY(0.2857143f))
			path.lineTo(decodeX(2.0f), decodeY(0.2857143f))
			path.curveTo(decodeAnchorX(2.0f, 3.4545454545454533f), decodeAnchorY(0.2857142984867096f, 0.0f), decodeAnchorX(2.7142858505249023f, -0.022727272727273373f), decodeAnchorY(1.0f, -3.4772727272727266f), decodeX(2.7142859f), decodeY(1.0f))
			path.lineTo(decodeX(2.7142859f), decodeY(3.0f))
			path.lineTo(decodeX(0.0f), decodeY(3.0f))
			path.closePath()
			Return path
		End Function

		Private Function decodeRect2() As Rectangle2D
				rect.rectect(decodeX(0.0f), decodeY(0.0f), decodeX(0.0f) - decodeX(0.0f), decodeY(0.0f) - decodeY(0.0f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodePath7() As Path2D
			path.reset()
			path.moveTo(decodeX(1.0f), decodeY(2.0f))
			path.lineTo(decodeX(1.490909f), decodeY(1.0284091f))
			path.lineTo(decodeX(2.0f), decodeY(2.0f))
			path.lineTo(decodeX(1.0f), decodeY(2.0f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath8() As Path2D
			path.reset()
			path.moveTo(decodeX(1.0f), decodeY(2.0f))
			path.lineTo(decodeX(1.490909f), decodeY(1.3522727f))
			path.lineTo(decodeX(2.0f), decodeY(2.0f))
			path.lineTo(decodeX(1.0f), decodeY(2.0f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath9() As Path2D
			path.reset()
			path.moveTo(decodeX(1.0f), decodeY(2.0f))
			path.lineTo(decodeX(1.5045455f), decodeY(1.0795455f))
			path.lineTo(decodeX(2.0f), decodeY(2.0f))
			path.lineTo(decodeX(1.0f), decodeY(2.0f))
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
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color3, decodeColor(color3,color4,0.5f), color4})
		End Function

		Private Function decodeGradient3(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color6, decodeColor(color6,color7,0.5f), color7})
		End Function

		Private Function decodeGradient4(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.36497328f,0.72994655f,0.8649733f,1.0f }, New Color() { color8, decodeColor(color8,color9,0.5f), color9, decodeColor(color9,color10,0.5f), color10})
		End Function

		Private Function decodeGradient5(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.37566844f,0.7513369f,0.8756684f,1.0f }, New Color() { color8, decodeColor(color8,color9,0.5f), color9, decodeColor(color9,color10,0.5f), color10})
		End Function

		Private Function decodeGradient6(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color13, decodeColor(color13,color14,0.5f), color14})
		End Function

		Private Function decodeGradient7(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.37967914f,0.7593583f,0.87967914f,1.0f }, New Color() { color15, decodeColor(color15,color16,0.5f), color16, decodeColor(color16,color17,0.5f), color17})
		End Function

		Private Function decodeGradient8(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color19, decodeColor(color19,color20,0.5f), color20})
		End Function

		Private Function decodeGradient9(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.37165776f,0.7433155f,0.8716577f,1.0f }, New Color() { color21, decodeColor(color21,color22,0.5f), color22, decodeColor(color22,color23,0.5f), color23})
		End Function

		Private Function decodeGradient10(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.3970588f,0.7941176f,0.89705884f,1.0f }, New Color() { color15, decodeColor(color15,color16,0.5f), color16, decodeColor(color16,color17,0.5f), color17})
		End Function

		Private Function decodeGradient11(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.4318182f,0.8636364f,0.9318182f,1.0f }, New Color() { color21, decodeColor(color21,color22,0.5f), color22, decodeColor(color22,color23,0.5f), color23})
		End Function

		Private Function decodeGradient12(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.48636365f * w) + x, (0.0116959065f * h) + y, (0.4909091f * w) + x, (0.8888889f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color27, decodeColor(color27,color28,0.5f), color28})
		End Function


	End Class

End Namespace
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



	Friend NotInheritable Class CheckBoxPainter
		Inherits AbstractRegionPainter

		'package private integers representing the available states that
		'this painter will paint. These are used when creating a new instance
		'of CheckBoxPainter to determine which region/state is being painted
		'by that instance.
		Friend Const BACKGROUND_DISABLED As Integer = 1
		Friend Const BACKGROUND_ENABLED As Integer = 2
		Friend Const ICON_DISABLED As Integer = 3
		Friend Const ICON_ENABLED As Integer = 4
		Friend Const ICON_FOCUSED As Integer = 5
		Friend Const ICON_MOUSEOVER As Integer = 6
		Friend Const ICON_MOUSEOVER_FOCUSED As Integer = 7
		Friend Const ICON_PRESSED As Integer = 8
		Friend Const ICON_PRESSED_FOCUSED As Integer = 9
		Friend Const ICON_SELECTED As Integer = 10
		Friend Const ICON_SELECTED_FOCUSED As Integer = 11
		Friend Const ICON_PRESSED_SELECTED As Integer = 12
		Friend Const ICON_PRESSED_SELECTED_FOCUSED As Integer = 13
		Friend Const ICON_MOUSEOVER_SELECTED As Integer = 14
		Friend Const ICON_MOUSEOVER_SELECTED_FOCUSED As Integer = 15
		Friend Const ICON_DISABLED_SELECTED As Integer = 16


		Private state As Integer 'refers to one of the static final ints above
		Private ctx As PaintContext

		'the following 4 variables are reused during the painting code of the layers
		Private path As Path2D = New Path2D.Float
		Private rect As Rectangle2D = New Rectangle2D.Float(0, 0, 0, 0)
		Private roundRect As RoundRectangle2D = New RoundRectangle2D.Float(0, 0, 0, 0, 0, 0)
		Private ellipse As Ellipse2D = New Ellipse2D.Float(0, 0, 0, 0)

		'All Colors used for painting are stored here. Ideally, only those colors being used
		'by a particular instance of CheckBoxPainter would be created. For the moment at least,
		'however, all are created for each instance.
		Private color1 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.06766917f, 0.07843137f, 0)
		Private color2 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.06484103f, 0.027450979f, 0)
		Private color3 As Color = decodeColor("nimbusBase", 0.032459438f, -0.60996324f, 0.36470586f, 0)
		Private color4 As Color = decodeColor("nimbusBase", 0.02551502f, -0.5996783f, 0.3215686f, 0)
		Private color5 As Color = decodeColor("nimbusBase", 0.032459438f, -0.59624064f, 0.34509802f, 0)
		Private color6 As Color = decodeColor("nimbusBlueGrey", 0.0f, 0.0f, 0.0f, -89)
		Private color7 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.05356429f, -0.12549019f, 0)
		Private color8 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.015789472f, -0.37254903f, 0)
		Private color9 As Color = decodeColor("nimbusBase", 0.08801502f, -0.63174605f, 0.43921566f, 0)
		Private color10 As Color = decodeColor("nimbusBase", 0.032459438f, -0.5953556f, 0.32549018f, 0)
		Private color11 As Color = decodeColor("nimbusBase", 0.032459438f, -0.59942394f, 0.4235294f, 0)
		Private color12 As Color = decodeColor("nimbusFocus", 0.0f, 0.0f, 0.0f, 0)
		Private color13 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.020974077f, -0.21960783f, 0)
		Private color14 As Color = decodeColor("nimbusBlueGrey", 0.01010108f, 0.08947369f, -0.5294118f, 0)
		Private color15 As Color = decodeColor("nimbusBase", 0.08801502f, -0.6317773f, 0.4470588f, 0)
		Private color16 As Color = decodeColor("nimbusBase", 0.032459438f, -0.5985242f, 0.39999998f, 0)
		Private color17 As Color = decodeColor("nimbusBase", 0.0f, -0.6357143f, 0.45098037f, 0)
		Private color18 As Color = decodeColor("nimbusBlueGrey", 0.055555582f, 0.8894737f, -0.7176471f, 0)
		Private color19 As Color = decodeColor("nimbusBlueGrey", 0.0f, 0.0016232133f, -0.3254902f, 0)
		Private color20 As Color = decodeColor("nimbusBase", 0.027408898f, -0.5847884f, 0.2980392f, 0)
		Private color21 As Color = decodeColor("nimbusBase", 0.029681683f, -0.52701867f, 0.17254901f, 0)
		Private color22 As Color = decodeColor("nimbusBase", 0.029681683f, -0.5376751f, 0.25098038f, 0)
		Private color23 As Color = decodeColor("nimbusBase", 5.1498413E-4f, -0.34585923f, -0.007843137f, 0)
		Private color24 As Color = decodeColor("nimbusBase", 5.1498413E-4f, -0.10238093f, -0.25490198f, 0)
		Private color25 As Color = decodeColor("nimbusBase", 0.004681647f, -0.6197143f, 0.43137252f, 0)
		Private color26 As Color = decodeColor("nimbusBase", 5.1498413E-4f, -0.44153953f, 0.2588235f, 0)
		Private color27 As Color = decodeColor("nimbusBase", 5.1498413E-4f, -0.4602757f, 0.34509802f, 0)
		Private color28 As Color = decodeColor("nimbusBase", -0.57865167f, -0.6357143f, -0.54901963f, 0)
		Private color29 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.110526316f, 0.25490195f, 0)
		Private color30 As Color = decodeColor("nimbusBase", -3.528595E-5f, 0.026785731f, -0.23529413f, 0)
		Private color31 As Color = decodeColor("nimbusBase", -4.2033195E-4f, -0.38050595f, 0.20392156f, 0)
		Private color32 As Color = decodeColor("nimbusBase", -0.0021489263f, -0.2891234f, 0.14117646f, 0)
		Private color33 As Color = decodeColor("nimbusBase", -0.006362498f, -0.016311288f, -0.02352941f, 0)
		Private color34 As Color = decodeColor("nimbusBase", 0.0f, -0.17930403f, 0.21568626f, 0)
		Private color35 As Color = decodeColor("nimbusBase", 0.0013483167f, -0.1769987f, -0.12156865f, 0)
		Private color36 As Color = decodeColor("nimbusBase", 0.05468172f, 0.3642857f, -0.43137258f, 0)
		Private color37 As Color = decodeColor("nimbusBase", 0.004681647f, -0.6198413f, 0.43921566f, 0)
		Private color38 As Color = decodeColor("nimbusBase", 5.1498413E-4f, -0.4555341f, 0.3215686f, 0)
		Private color39 As Color = decodeColor("nimbusBase", 5.1498413E-4f, -0.47377098f, 0.41960782f, 0)
		Private color40 As Color = decodeColor("nimbusBlueGrey", -0.01111114f, -0.03771078f, 0.062745094f, 0)
		Private color41 As Color = decodeColor("nimbusBlueGrey", -0.02222222f, -0.032806106f, 0.011764705f, 0)
		Private color42 As Color = decodeColor("nimbusBase", 0.021348298f, -0.59223604f, 0.35294116f, 0)
		Private color43 As Color = decodeColor("nimbusBase", 0.021348298f, -0.56722116f, 0.3098039f, 0)
		Private color44 As Color = decodeColor("nimbusBase", 0.021348298f, -0.56875f, 0.32941175f, 0)
		Private color45 As Color = decodeColor("nimbusBase", 0.027408898f, -0.5735674f, 0.14509803f, 0)


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
				Case ICON_DISABLED
					painticonDisabled(g)
				Case ICON_ENABLED
					painticonEnabled(g)
				Case ICON_FOCUSED
					painticonFocused(g)
				Case ICON_MOUSEOVER
					painticonMouseOver(g)
				Case ICON_MOUSEOVER_FOCUSED
					painticonMouseOverAndFocused(g)
				Case ICON_PRESSED
					painticonPressed(g)
				Case ICON_PRESSED_FOCUSED
					painticonPressedAndFocused(g)
				Case ICON_SELECTED
					painticonSelected(g)
				Case ICON_SELECTED_FOCUSED
					painticonSelectedAndFocused(g)
				Case ICON_PRESSED_SELECTED
					painticonPressedAndSelected(g)
				Case ICON_PRESSED_SELECTED_FOCUSED
					painticonPressedAndSelectedAndFocused(g)
				Case ICON_MOUSEOVER_SELECTED
					painticonMouseOverAndSelected(g)
				Case ICON_MOUSEOVER_SELECTED_FOCUSED
					painticonMouseOverAndSelectedAndFocused(g)
				Case ICON_DISABLED_SELECTED
					painticonDisabledAndSelected(g)

			End Select
		End Sub



		Protected Friend Property NotOverridable Overrides paintContext As PaintContext
			Get
				Return ctx
			End Get
		End Property

		Private Sub painticonDisabled(ByVal g As Graphics2D)
			roundRect = decodeRoundRect1()
			g.paint = decodeGradient1(roundRect)
			g.fill(roundRect)
			roundRect = decodeRoundRect2()
			g.paint = decodeGradient2(roundRect)
			g.fill(roundRect)

		End Sub

		Private Sub painticonEnabled(ByVal g As Graphics2D)
			roundRect = decodeRoundRect3()
			g.paint = color6
			g.fill(roundRect)
			roundRect = decodeRoundRect1()
			g.paint = decodeGradient3(roundRect)
			g.fill(roundRect)
			roundRect = decodeRoundRect2()
			g.paint = decodeGradient4(roundRect)
			g.fill(roundRect)

		End Sub

		Private Sub painticonFocused(ByVal g As Graphics2D)
			roundRect = decodeRoundRect4()
			g.paint = color12
			g.fill(roundRect)
			roundRect = decodeRoundRect1()
			g.paint = decodeGradient3(roundRect)
			g.fill(roundRect)
			roundRect = decodeRoundRect2()
			g.paint = decodeGradient4(roundRect)
			g.fill(roundRect)

		End Sub

		Private Sub painticonMouseOver(ByVal g As Graphics2D)
			roundRect = decodeRoundRect3()
			g.paint = color6
			g.fill(roundRect)
			roundRect = decodeRoundRect1()
			g.paint = decodeGradient5(roundRect)
			g.fill(roundRect)
			roundRect = decodeRoundRect2()
			g.paint = decodeGradient6(roundRect)
			g.fill(roundRect)

		End Sub

		Private Sub painticonMouseOverAndFocused(ByVal g As Graphics2D)
			roundRect = decodeRoundRect4()
			g.paint = color12
			g.fill(roundRect)
			roundRect = decodeRoundRect1()
			g.paint = decodeGradient5(roundRect)
			g.fill(roundRect)
			roundRect = decodeRoundRect2()
			g.paint = decodeGradient6(roundRect)
			g.fill(roundRect)

		End Sub

		Private Sub painticonPressed(ByVal g As Graphics2D)
			roundRect = decodeRoundRect3()
			g.paint = color6
			g.fill(roundRect)
			roundRect = decodeRoundRect1()
			g.paint = decodeGradient7(roundRect)
			g.fill(roundRect)
			roundRect = decodeRoundRect2()
			g.paint = decodeGradient8(roundRect)
			g.fill(roundRect)

		End Sub

		Private Sub painticonPressedAndFocused(ByVal g As Graphics2D)
			roundRect = decodeRoundRect4()
			g.paint = color12
			g.fill(roundRect)
			roundRect = decodeRoundRect1()
			g.paint = decodeGradient7(roundRect)
			g.fill(roundRect)
			roundRect = decodeRoundRect2()
			g.paint = decodeGradient8(roundRect)
			g.fill(roundRect)

		End Sub

		Private Sub painticonSelected(ByVal g As Graphics2D)
			roundRect = decodeRoundRect3()
			g.paint = color6
			g.fill(roundRect)
			roundRect = decodeRoundRect1()
			g.paint = decodeGradient9(roundRect)
			g.fill(roundRect)
			roundRect = decodeRoundRect2()
			g.paint = decodeGradient10(roundRect)
			g.fill(roundRect)
			path = decodePath1()
			g.paint = color28
			g.fill(path)

		End Sub

		Private Sub painticonSelectedAndFocused(ByVal g As Graphics2D)
			roundRect = decodeRoundRect4()
			g.paint = color12
			g.fill(roundRect)
			roundRect = decodeRoundRect1()
			g.paint = decodeGradient9(roundRect)
			g.fill(roundRect)
			roundRect = decodeRoundRect2()
			g.paint = decodeGradient10(roundRect)
			g.fill(roundRect)
			path = decodePath1()
			g.paint = color28
			g.fill(path)

		End Sub

		Private Sub painticonPressedAndSelected(ByVal g As Graphics2D)
			roundRect = decodeRoundRect3()
			g.paint = color29
			g.fill(roundRect)
			roundRect = decodeRoundRect1()
			g.paint = decodeGradient11(roundRect)
			g.fill(roundRect)
			roundRect = decodeRoundRect2()
			g.paint = decodeGradient12(roundRect)
			g.fill(roundRect)
			path = decodePath1()
			g.paint = color28
			g.fill(path)

		End Sub

		Private Sub painticonPressedAndSelectedAndFocused(ByVal g As Graphics2D)
			roundRect = decodeRoundRect4()
			g.paint = color12
			g.fill(roundRect)
			roundRect = decodeRoundRect1()
			g.paint = decodeGradient11(roundRect)
			g.fill(roundRect)
			roundRect = decodeRoundRect2()
			g.paint = decodeGradient12(roundRect)
			g.fill(roundRect)
			path = decodePath1()
			g.paint = color28
			g.fill(path)

		End Sub

		Private Sub painticonMouseOverAndSelected(ByVal g As Graphics2D)
			roundRect = decodeRoundRect3()
			g.paint = color6
			g.fill(roundRect)
			roundRect = decodeRoundRect1()
			g.paint = decodeGradient13(roundRect)
			g.fill(roundRect)
			roundRect = decodeRoundRect2()
			g.paint = decodeGradient14(roundRect)
			g.fill(roundRect)
			path = decodePath1()
			g.paint = color28
			g.fill(path)

		End Sub

		Private Sub painticonMouseOverAndSelectedAndFocused(ByVal g As Graphics2D)
			roundRect = decodeRoundRect4()
			g.paint = color12
			g.fill(roundRect)
			roundRect = decodeRoundRect1()
			g.paint = decodeGradient13(roundRect)
			g.fill(roundRect)
			roundRect = decodeRoundRect2()
			g.paint = decodeGradient14(roundRect)
			g.fill(roundRect)
			path = decodePath1()
			g.paint = color28
			g.fill(path)

		End Sub

		Private Sub painticonDisabledAndSelected(ByVal g As Graphics2D)
			roundRect = decodeRoundRect1()
			g.paint = decodeGradient15(roundRect)
			g.fill(roundRect)
			roundRect = decodeRoundRect2()
			g.paint = decodeGradient16(roundRect)
			g.fill(roundRect)
			path = decodePath1()
			g.paint = color45
			g.fill(path)

		End Sub



		Private Function decodeRoundRect1() As RoundRectangle2D
			roundRect.roundRectect(decodeX(0.4f), decodeY(0.4f), decodeX(2.6f) - decodeX(0.4f), decodeY(2.6f) - decodeY(0.4f), 3.7058823f, 3.7058823f) 'rounding - height - width - y - x
			Return roundRect
		End Function

		Private Function decodeRoundRect2() As RoundRectangle2D
			roundRect.roundRectect(decodeX(0.6f), decodeY(0.6f), decodeX(2.4f) - decodeX(0.6f), decodeY(2.4f) - decodeY(0.6f), 3.764706f, 3.764706f) 'rounding - height - width - y - x
			Return roundRect
		End Function

		Private Function decodeRoundRect3() As RoundRectangle2D
			roundRect.roundRectect(decodeX(0.4f), decodeY(1.75f), decodeX(2.6f) - decodeX(0.4f), decodeY(2.8f) - decodeY(1.75f), 5.1764708f, 5.1764708f) 'rounding - height - width - y - x
			Return roundRect
		End Function

		Private Function decodeRoundRect4() As RoundRectangle2D
			roundRect.roundRectect(decodeX(0.120000005f), decodeY(0.120000005f), decodeX(2.8799999f) - decodeX(0.120000005f), decodeY(2.8799999f) - decodeY(0.120000005f), 8.0f, 8.0f) 'rounding - height - width - y - x
			Return roundRect
		End Function

		Private Function decodePath1() As Path2D
			path.reset()
			path.moveTo(decodeX(1.0036764f), decodeY(1.382353f))
			path.lineTo(decodeX(1.2536764f), decodeY(1.382353f))
			path.lineTo(decodeX(1.430147f), decodeY(1.757353f))
			path.lineTo(decodeX(1.8235294f), decodeY(0.62352943f))
			path.lineTo(decodeX(2.2f), decodeY(0.61764705f))
			path.lineTo(decodeX(1.492647f), decodeY(2.0058823f))
			path.lineTo(decodeX(1.382353f), decodeY(2.0058823f))
			path.lineTo(decodeX(1.0036764f), decodeY(1.382353f))
			path.closePath()
			Return path
		End Function



		Private Function decodeGradient1(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.25f * w) + x, (0.0f * h) + y, (0.25210086f * w) + x, (0.9957983f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color1, decodeColor(color1,color2,0.5f), color2})
		End Function

		Private Function decodeGradient2(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.25f * w) + x, (0.0f * h) + y, (0.25f * w) + x, (0.997549f * h) + y, New Single() { 0.0f,0.32228917f,0.64457834f,0.82228917f,1.0f }, New Color() { color3, decodeColor(color3,color4,0.5f), color4, decodeColor(color4,color5,0.5f), color5})
		End Function

		Private Function decodeGradient3(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.25f * w) + x, (0.0f * h) + y, (0.25210086f * w) + x, (0.9957983f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color7, decodeColor(color7,color8,0.5f), color8})
		End Function

		Private Function decodeGradient4(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.25f * w) + x, (0.0f * h) + y, (0.25f * w) + x, (0.997549f * h) + y, New Single() { 0.0f,0.32228917f,0.64457834f,0.82228917f,1.0f }, New Color() { color9, decodeColor(color9,color10,0.5f), color10, decodeColor(color10,color11,0.5f), color11})
		End Function

		Private Function decodeGradient5(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.25f * w) + x, (0.0f * h) + y, (0.25210086f * w) + x, (0.9957983f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color13, decodeColor(color13,color14,0.5f), color14})
		End Function

		Private Function decodeGradient6(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.25f * w) + x, (0.0f * h) + y, (0.25f * w) + x, (0.997549f * h) + y, New Single() { 0.0f,0.32228917f,0.64457834f,0.82228917f,1.0f }, New Color() { color15, decodeColor(color15,color16,0.5f), color16, decodeColor(color16,color17,0.5f), color17})
		End Function

		Private Function decodeGradient7(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.25f * w) + x, (0.0f * h) + y, (0.25210086f * w) + x, (0.9957983f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color18, decodeColor(color18,color19,0.5f), color19})
		End Function

		Private Function decodeGradient8(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.25f * w) + x, (0.0f * h) + y, (0.25f * w) + x, (0.997549f * h) + y, New Single() { 0.0f,0.32228917f,0.64457834f,0.82228917f,1.0f }, New Color() { color20, decodeColor(color20,color21,0.5f), color21, decodeColor(color21,color22,0.5f), color22})
		End Function

		Private Function decodeGradient9(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.25f * w) + x, (0.0f * h) + y, (0.25210086f * w) + x, (0.9957983f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color23, decodeColor(color23,color24,0.5f), color24})
		End Function

		Private Function decodeGradient10(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.25f * w) + x, (0.0f * h) + y, (0.25f * w) + x, (0.997549f * h) + y, New Single() { 0.0f,0.32228917f,0.64457834f,0.82228917f,1.0f }, New Color() { color25, decodeColor(color25,color26,0.5f), color26, decodeColor(color26,color27,0.5f), color27})
		End Function

		Private Function decodeGradient11(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.25f * w) + x, (0.0f * h) + y, (0.25210086f * w) + x, (0.9957983f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color28, decodeColor(color28,color30,0.5f), color30})
		End Function

		Private Function decodeGradient12(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.25f * w) + x, (0.0f * h) + y, (0.25f * w) + x, (0.997549f * h) + y, New Single() { 0.0f,0.05775076f,0.11550152f,0.38003993f,0.64457834f,0.82228917f,1.0f }, New Color() { color31, decodeColor(color31,color32,0.5f), color32, decodeColor(color32,color33,0.5f), color33, decodeColor(color33,color34,0.5f), color34})
		End Function

		Private Function decodeGradient13(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.25f * w) + x, (0.0f * h) + y, (0.25210086f * w) + x, (0.9957983f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color35, decodeColor(color35,color36,0.5f), color36})
		End Function

		Private Function decodeGradient14(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.25f * w) + x, (0.0f * h) + y, (0.25f * w) + x, (0.997549f * h) + y, New Single() { 0.0f,0.32228917f,0.64457834f,0.82228917f,1.0f }, New Color() { color37, decodeColor(color37,color38,0.5f), color38, decodeColor(color38,color39,0.5f), color39})
		End Function

		Private Function decodeGradient15(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.25f * w) + x, (0.0f * h) + y, (0.25210086f * w) + x, (0.9957983f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color40, decodeColor(color40,color41,0.5f), color41})
		End Function

		Private Function decodeGradient16(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.25f * w) + x, (0.0f * h) + y, (0.25f * w) + x, (0.997549f * h) + y, New Single() { 0.0f,0.32228917f,0.64457834f,0.82228917f,1.0f }, New Color() { color42, decodeColor(color42,color43,0.5f), color43, decodeColor(color43,color44,0.5f), color44})
		End Function


	End Class

End Namespace
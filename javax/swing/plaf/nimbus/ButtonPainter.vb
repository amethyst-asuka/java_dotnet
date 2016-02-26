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



	Friend NotInheritable Class ButtonPainter
		Inherits AbstractRegionPainter

		'package private integers representing the available states that
		'this painter will paint. These are used when creating a new instance
		'of ButtonPainter to determine which region/state is being painted
		'by that instance.
		Friend Const BACKGROUND_DEFAULT As Integer = 1
		Friend Const BACKGROUND_DEFAULT_FOCUSED As Integer = 2
		Friend Const BACKGROUND_MOUSEOVER_DEFAULT As Integer = 3
		Friend Const BACKGROUND_MOUSEOVER_DEFAULT_FOCUSED As Integer = 4
		Friend Const BACKGROUND_PRESSED_DEFAULT As Integer = 5
		Friend Const BACKGROUND_PRESSED_DEFAULT_FOCUSED As Integer = 6
		Friend Const BACKGROUND_DISABLED As Integer = 7
		Friend Const BACKGROUND_ENABLED As Integer = 8
		Friend Const BACKGROUND_FOCUSED As Integer = 9
		Friend Const BACKGROUND_MOUSEOVER As Integer = 10
		Friend Const BACKGROUND_MOUSEOVER_FOCUSED As Integer = 11
		Friend Const BACKGROUND_PRESSED As Integer = 12
		Friend Const BACKGROUND_PRESSED_FOCUSED As Integer = 13


		Private state As Integer 'refers to one of the static final ints above
		Private ctx As PaintContext

		'the following 4 variables are reused during the painting code of the layers
		Private path As Path2D = New Path2D.Float
		Private rect As Rectangle2D = New Rectangle2D.Float(0, 0, 0, 0)
		Private roundRect As RoundRectangle2D = New RoundRectangle2D.Float(0, 0, 0, 0, 0, 0)
		Private ellipse As Ellipse2D = New Ellipse2D.Float(0, 0, 0, 0)

		'All Colors used for painting are stored here. Ideally, only those colors being used
		'by a particular instance of ButtonPainter would be created. For the moment at least,
		'however, all are created for each instance.
		Private color1 As Color = decodeColor("nimbusBlueGrey", -0.027777791f, -0.06885965f, -0.36862746f, -190)
		Private color2 As Color = decodeColor("nimbusBase", 5.1498413E-4f, -0.34585923f, -0.007843137f, 0)
		Private color3 As Color = decodeColor("nimbusBase", 5.1498413E-4f, -0.095173776f, -0.25882354f, 0)
		Private color4 As Color = decodeColor("nimbusBase", 0.004681647f, -0.6197143f, 0.43137252f, 0)
		Private color5 As Color = decodeColor("nimbusBase", 0.004681647f, -0.5766426f, 0.38039213f, 0)
		Private color6 As Color = decodeColor("nimbusBase", 5.1498413E-4f, -0.43866998f, 0.24705881f, 0)
		Private color7 As Color = decodeColor("nimbusBase", 5.1498413E-4f, -0.46404046f, 0.36470586f, 0)
		Private color8 As Color = decodeColor("nimbusBase", 5.1498413E-4f, -0.47761154f, 0.44313723f, 0)
		Private color9 As Color = decodeColor("nimbusFocus", 0.0f, 0.0f, 0.0f, 0)
		Private color10 As Color = decodeColor("nimbusBase", 0.0013483167f, -0.1769987f, -0.12156865f, 0)
		Private color11 As Color = decodeColor("nimbusBase", 0.059279382f, 0.3642857f, -0.43529415f, 0)
		Private color12 As Color = decodeColor("nimbusBase", 0.004681647f, -0.6198413f, 0.43921566f, 0)
		Private color13 As Color = decodeColor("nimbusBase", -0.0017285943f, -0.5822163f, 0.40392154f, 0)
		Private color14 As Color = decodeColor("nimbusBase", 5.1498413E-4f, -0.4555341f, 0.3215686f, 0)
		Private color15 As Color = decodeColor("nimbusBase", 5.1498413E-4f, -0.47698414f, 0.43921566f, 0)
		Private color16 As Color = decodeColor("nimbusBase", -0.06415892f, -0.5455182f, 0.45098037f, 0)
		Private color17 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.110526316f, 0.25490195f, -95)
		Private color18 As Color = decodeColor("nimbusBase", -0.57865167f, -0.6357143f, -0.54901963f, 0)
		Private color19 As Color = decodeColor("nimbusBase", -3.528595E-5f, 0.018606722f, -0.23137257f, 0)
		Private color20 As Color = decodeColor("nimbusBase", -4.2033195E-4f, -0.38050595f, 0.20392156f, 0)
		Private color21 As Color = decodeColor("nimbusBase", 0.001903832f, -0.29863563f, 0.1490196f, 0)
		Private color22 As Color = decodeColor("nimbusBase", 0.0f, 0.0f, 0.0f, 0)
		Private color23 As Color = decodeColor("nimbusBase", 0.0018727183f, -0.14126986f, 0.15686274f, 0)
		Private color24 As Color = decodeColor("nimbusBase", 8.9377165E-4f, -0.20852983f, 0.2588235f, 0)
		Private color25 As Color = decodeColor("nimbusBlueGrey", -0.027777791f, -0.06885965f, -0.36862746f, -232)
		Private color26 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.06766917f, 0.07843137f, 0)
		Private color27 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.06484103f, 0.027450979f, 0)
		Private color28 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.08477524f, 0.16862744f, 0)
		Private color29 As Color = decodeColor("nimbusBlueGrey", -0.015872955f, -0.080091536f, 0.15686274f, 0)
		Private color30 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.07016757f, 0.12941176f, 0)
		Private color31 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.07052632f, 0.1372549f, 0)
		Private color32 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.070878744f, 0.14509803f, 0)
		Private color33 As Color = decodeColor("nimbusBlueGrey", -0.055555522f, -0.05356429f, -0.12549019f, 0)
		Private color34 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.0147816315f, -0.3764706f, 0)
		Private color35 As Color = decodeColor("nimbusBlueGrey", 0.055555582f, -0.10655806f, 0.24313724f, 0)
		Private color36 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.09823123f, 0.2117647f, 0)
		Private color37 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.0749532f, 0.24705881f, 0)
		Private color38 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.110526316f, 0.25490195f, 0)
		Private color39 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.020974077f, -0.21960783f, 0)
		Private color40 As Color = decodeColor("nimbusBlueGrey", 0.0f, 0.11169591f, -0.53333336f, 0)
		Private color41 As Color = decodeColor("nimbusBlueGrey", 0.055555582f, -0.10658931f, 0.25098038f, 0)
		Private color42 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.098526314f, 0.2352941f, 0)
		Private color43 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.07333623f, 0.20392156f, 0)
		Private color44 As New Color(245, 250, 255, 160)
		Private color45 As Color = decodeColor("nimbusBlueGrey", 0.055555582f, 0.8894737f, -0.7176471f, 0)
		Private color46 As Color = decodeColor("nimbusBlueGrey", 0.0f, 5.847961E-4f, -0.32156864f, 0)
		Private color47 As Color = decodeColor("nimbusBlueGrey", -0.00505054f, -0.05960039f, 0.10196078f, 0)
		Private color48 As Color = decodeColor("nimbusBlueGrey", -0.008547008f, -0.04772438f, 0.06666666f, 0)
		Private color49 As Color = decodeColor("nimbusBlueGrey", -0.0027777553f, -0.0018306673f, -0.02352941f, 0)
		Private color50 As Color = decodeColor("nimbusBlueGrey", -0.0027777553f, -0.0212406f, 0.13333333f, 0)
		Private color51 As Color = decodeColor("nimbusBlueGrey", 0.0055555105f, -0.030845039f, 0.23921567f, 0)


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
				Case BACKGROUND_DEFAULT
					paintBackgroundDefault(g)
				Case BACKGROUND_DEFAULT_FOCUSED
					paintBackgroundDefaultAndFocused(g)
				Case BACKGROUND_MOUSEOVER_DEFAULT
					paintBackgroundMouseOverAndDefault(g)
				Case BACKGROUND_MOUSEOVER_DEFAULT_FOCUSED
					paintBackgroundMouseOverAndDefaultAndFocused(g)
				Case BACKGROUND_PRESSED_DEFAULT
					paintBackgroundPressedAndDefault(g)
				Case BACKGROUND_PRESSED_DEFAULT_FOCUSED
					paintBackgroundPressedAndDefaultAndFocused(g)
				Case BACKGROUND_DISABLED
					paintBackgroundDisabled(g)
				Case BACKGROUND_ENABLED
					paintBackgroundEnabled(g)
				Case BACKGROUND_FOCUSED
					paintBackgroundFocused(g)
				Case BACKGROUND_MOUSEOVER
					paintBackgroundMouseOver(g)
				Case BACKGROUND_MOUSEOVER_FOCUSED
					paintBackgroundMouseOverAndFocused(g)
				Case BACKGROUND_PRESSED
					paintBackgroundPressed(g)
				Case BACKGROUND_PRESSED_FOCUSED
					paintBackgroundPressedAndFocused(g)

			End Select
		End Sub

		Protected Friend Overrides Function getExtendedCacheKeys(ByVal c As JComponent) As Object()
			Dim ___extendedCacheKeys As Object() = Nothing
			Select Case state
				Case BACKGROUND_DEFAULT
					___extendedCacheKeys = New Object() { getComponentColor(c, "background", color4, -0.6197143f, 0.43137252f, 0), getComponentColor(c, "background", color5, -0.5766426f, 0.38039213f, 0), getComponentColor(c, "background", color6, -0.43866998f, 0.24705881f, 0), getComponentColor(c, "background", color7, -0.46404046f, 0.36470586f, 0), getComponentColor(c, "background", color8, -0.47761154f, 0.44313723f, 0)}
				Case BACKGROUND_DEFAULT_FOCUSED
					___extendedCacheKeys = New Object() { getComponentColor(c, "background", color4, -0.6197143f, 0.43137252f, 0), getComponentColor(c, "background", color5, -0.5766426f, 0.38039213f, 0), getComponentColor(c, "background", color6, -0.43866998f, 0.24705881f, 0), getComponentColor(c, "background", color7, -0.46404046f, 0.36470586f, 0), getComponentColor(c, "background", color8, -0.47761154f, 0.44313723f, 0)}
				Case BACKGROUND_MOUSEOVER_DEFAULT
					___extendedCacheKeys = New Object() { getComponentColor(c, "background", color12, -0.6198413f, 0.43921566f, 0), getComponentColor(c, "background", color13, -0.5822163f, 0.40392154f, 0), getComponentColor(c, "background", color14, -0.4555341f, 0.3215686f, 0), getComponentColor(c, "background", color15, -0.47698414f, 0.43921566f, 0), getComponentColor(c, "background", color16, -0.5455182f, 0.45098037f, 0)}
				Case BACKGROUND_MOUSEOVER_DEFAULT_FOCUSED
					___extendedCacheKeys = New Object() { getComponentColor(c, "background", color12, -0.6198413f, 0.43921566f, 0), getComponentColor(c, "background", color13, -0.5822163f, 0.40392154f, 0), getComponentColor(c, "background", color14, -0.4555341f, 0.3215686f, 0), getComponentColor(c, "background", color15, -0.47698414f, 0.43921566f, 0), getComponentColor(c, "background", color16, -0.5455182f, 0.45098037f, 0)}
				Case BACKGROUND_PRESSED_DEFAULT
					___extendedCacheKeys = New Object() { getComponentColor(c, "background", color20, -0.38050595f, 0.20392156f, 0), getComponentColor(c, "background", color21, -0.29863563f, 0.1490196f, 0), getComponentColor(c, "background", color22, 0.0f, 0.0f, 0), getComponentColor(c, "background", color23, -0.14126986f, 0.15686274f, 0), getComponentColor(c, "background", color24, -0.20852983f, 0.2588235f, 0)}
				Case BACKGROUND_PRESSED_DEFAULT_FOCUSED
					___extendedCacheKeys = New Object() { getComponentColor(c, "background", color20, -0.38050595f, 0.20392156f, 0), getComponentColor(c, "background", color21, -0.29863563f, 0.1490196f, 0), getComponentColor(c, "background", color22, 0.0f, 0.0f, 0), getComponentColor(c, "background", color23, -0.14126986f, 0.15686274f, 0), getComponentColor(c, "background", color24, -0.20852983f, 0.2588235f, 0)}
				Case BACKGROUND_ENABLED
					___extendedCacheKeys = New Object() { getComponentColor(c, "background", color35, -0.10655806f, 0.24313724f, 0), getComponentColor(c, "background", color36, -0.09823123f, 0.2117647f, 0), getComponentColor(c, "background", color30, -0.07016757f, 0.12941176f, 0), getComponentColor(c, "background", color37, -0.0749532f, 0.24705881f, 0), getComponentColor(c, "background", color38, -0.110526316f, 0.25490195f, 0)}
				Case BACKGROUND_FOCUSED
					___extendedCacheKeys = New Object() { getComponentColor(c, "background", color35, -0.10655806f, 0.24313724f, 0), getComponentColor(c, "background", color36, -0.09823123f, 0.2117647f, 0), getComponentColor(c, "background", color30, -0.07016757f, 0.12941176f, 0), getComponentColor(c, "background", color37, -0.0749532f, 0.24705881f, 0), getComponentColor(c, "background", color38, -0.110526316f, 0.25490195f, 0)}
				Case BACKGROUND_MOUSEOVER
					___extendedCacheKeys = New Object() { getComponentColor(c, "background", color41, -0.10658931f, 0.25098038f, 0), getComponentColor(c, "background", color42, -0.098526314f, 0.2352941f, 0), getComponentColor(c, "background", color43, -0.07333623f, 0.20392156f, 0), getComponentColor(c, "background", color38, -0.110526316f, 0.25490195f, 0)}
				Case BACKGROUND_MOUSEOVER_FOCUSED
					___extendedCacheKeys = New Object() { getComponentColor(c, "background", color41, -0.10658931f, 0.25098038f, 0), getComponentColor(c, "background", color42, -0.098526314f, 0.2352941f, 0), getComponentColor(c, "background", color43, -0.07333623f, 0.20392156f, 0), getComponentColor(c, "background", color38, -0.110526316f, 0.25490195f, 0)}
				Case BACKGROUND_PRESSED
					___extendedCacheKeys = New Object() { getComponentColor(c, "background", color47, -0.05960039f, 0.10196078f, 0), getComponentColor(c, "background", color48, -0.04772438f, 0.06666666f, 0), getComponentColor(c, "background", color49, -0.0018306673f, -0.02352941f, 0), getComponentColor(c, "background", color50, -0.0212406f, 0.13333333f, 0), getComponentColor(c, "background", color51, -0.030845039f, 0.23921567f, 0)}
				Case BACKGROUND_PRESSED_FOCUSED
					___extendedCacheKeys = New Object() { getComponentColor(c, "background", color47, -0.05960039f, 0.10196078f, 0), getComponentColor(c, "background", color48, -0.04772438f, 0.06666666f, 0), getComponentColor(c, "background", color49, -0.0018306673f, -0.02352941f, 0), getComponentColor(c, "background", color50, -0.0212406f, 0.13333333f, 0), getComponentColor(c, "background", color51, -0.030845039f, 0.23921567f, 0)}
			End Select
			Return ___extendedCacheKeys
		End Function

		Protected Friend Property NotOverridable Overrides paintContext As PaintContext
			Get
				Return ctx
			End Get
		End Property

		Private Sub paintBackgroundDefault(ByVal g As Graphics2D)
			roundRect = decodeRoundRect1()
			g.paint = color1
			g.fill(roundRect)
			roundRect = decodeRoundRect2()
			g.paint = decodeGradient1(roundRect)
			g.fill(roundRect)
			roundRect = decodeRoundRect3()
			g.paint = decodeGradient2(roundRect)
			g.fill(roundRect)

		End Sub

		Private Sub paintBackgroundDefaultAndFocused(ByVal g As Graphics2D)
			roundRect = decodeRoundRect4()
			g.paint = color9
			g.fill(roundRect)
			roundRect = decodeRoundRect2()
			g.paint = decodeGradient1(roundRect)
			g.fill(roundRect)
			roundRect = decodeRoundRect3()
			g.paint = decodeGradient2(roundRect)
			g.fill(roundRect)

		End Sub

		Private Sub paintBackgroundMouseOverAndDefault(ByVal g As Graphics2D)
			roundRect = decodeRoundRect5()
			g.paint = color1
			g.fill(roundRect)
			roundRect = decodeRoundRect2()
			g.paint = decodeGradient3(roundRect)
			g.fill(roundRect)
			roundRect = decodeRoundRect3()
			g.paint = decodeGradient2(roundRect)
			g.fill(roundRect)

		End Sub

		Private Sub paintBackgroundMouseOverAndDefaultAndFocused(ByVal g As Graphics2D)
			roundRect = decodeRoundRect4()
			g.paint = color9
			g.fill(roundRect)
			roundRect = decodeRoundRect2()
			g.paint = decodeGradient3(roundRect)
			g.fill(roundRect)
			roundRect = decodeRoundRect3()
			g.paint = decodeGradient2(roundRect)
			g.fill(roundRect)

		End Sub

		Private Sub paintBackgroundPressedAndDefault(ByVal g As Graphics2D)
			roundRect = decodeRoundRect1()
			g.paint = color17
			g.fill(roundRect)
			roundRect = decodeRoundRect2()
			g.paint = decodeGradient4(roundRect)
			g.fill(roundRect)
			roundRect = decodeRoundRect3()
			g.paint = decodeGradient2(roundRect)
			g.fill(roundRect)

		End Sub

		Private Sub paintBackgroundPressedAndDefaultAndFocused(ByVal g As Graphics2D)
			roundRect = decodeRoundRect4()
			g.paint = color9
			g.fill(roundRect)
			roundRect = decodeRoundRect2()
			g.paint = decodeGradient4(roundRect)
			g.fill(roundRect)
			roundRect = decodeRoundRect3()
			g.paint = decodeGradient2(roundRect)
			g.fill(roundRect)

		End Sub

		Private Sub paintBackgroundDisabled(ByVal g As Graphics2D)
			roundRect = decodeRoundRect1()
			g.paint = color25
			g.fill(roundRect)
			roundRect = decodeRoundRect2()
			g.paint = decodeGradient5(roundRect)
			g.fill(roundRect)
			roundRect = decodeRoundRect3()
			g.paint = decodeGradient6(roundRect)
			g.fill(roundRect)

		End Sub

		Private Sub paintBackgroundEnabled(ByVal g As Graphics2D)
			roundRect = decodeRoundRect1()
			g.paint = color1
			g.fill(roundRect)
			roundRect = decodeRoundRect2()
			g.paint = decodeGradient7(roundRect)
			g.fill(roundRect)
			roundRect = decodeRoundRect3()
			g.paint = decodeGradient2(roundRect)
			g.fill(roundRect)

		End Sub

		Private Sub paintBackgroundFocused(ByVal g As Graphics2D)
			roundRect = decodeRoundRect4()
			g.paint = color9
			g.fill(roundRect)
			roundRect = decodeRoundRect2()
			g.paint = decodeGradient7(roundRect)
			g.fill(roundRect)
			roundRect = decodeRoundRect3()
			g.paint = decodeGradient8(roundRect)
			g.fill(roundRect)

		End Sub

		Private Sub paintBackgroundMouseOver(ByVal g As Graphics2D)
			roundRect = decodeRoundRect1()
			g.paint = color1
			g.fill(roundRect)
			roundRect = decodeRoundRect2()
			g.paint = decodeGradient9(roundRect)
			g.fill(roundRect)
			roundRect = decodeRoundRect3()
			g.paint = decodeGradient10(roundRect)
			g.fill(roundRect)

		End Sub

		Private Sub paintBackgroundMouseOverAndFocused(ByVal g As Graphics2D)
			roundRect = decodeRoundRect4()
			g.paint = color9
			g.fill(roundRect)
			roundRect = decodeRoundRect2()
			g.paint = decodeGradient9(roundRect)
			g.fill(roundRect)
			roundRect = decodeRoundRect3()
			g.paint = decodeGradient10(roundRect)
			g.fill(roundRect)

		End Sub

		Private Sub paintBackgroundPressed(ByVal g As Graphics2D)
			roundRect = decodeRoundRect1()
			g.paint = color44
			g.fill(roundRect)
			roundRect = decodeRoundRect2()
			g.paint = decodeGradient11(roundRect)
			g.fill(roundRect)
			roundRect = decodeRoundRect3()
			g.paint = decodeGradient2(roundRect)
			g.fill(roundRect)

		End Sub

		Private Sub paintBackgroundPressedAndFocused(ByVal g As Graphics2D)
			roundRect = decodeRoundRect4()
			g.paint = color9
			g.fill(roundRect)
			roundRect = decodeRoundRect2()
			g.paint = decodeGradient11(roundRect)
			g.fill(roundRect)
			roundRect = decodeRoundRect3()
			g.paint = decodeGradient2(roundRect)
			g.fill(roundRect)

		End Sub



		Private Function decodeRoundRect1() As RoundRectangle2D
			roundRect.roundRectect(decodeX(0.2857143f), decodeY(0.42857143f), decodeX(2.7142859f) - decodeX(0.2857143f), decodeY(2.857143f) - decodeY(0.42857143f), 12.0f, 12.0f) 'rounding - height - width - y - x
			Return roundRect
		End Function

		Private Function decodeRoundRect2() As RoundRectangle2D
			roundRect.roundRectect(decodeX(0.2857143f), decodeY(0.2857143f), decodeX(2.7142859f) - decodeX(0.2857143f), decodeY(2.7142859f) - decodeY(0.2857143f), 9.0f, 9.0f) 'rounding - height - width - y - x
			Return roundRect
		End Function

		Private Function decodeRoundRect3() As RoundRectangle2D
			roundRect.roundRectect(decodeX(0.42857143f), decodeY(0.42857143f), decodeX(2.5714285f) - decodeX(0.42857143f), decodeY(2.5714285f) - decodeY(0.42857143f), 7.0f, 7.0f) 'rounding - height - width - y - x
			Return roundRect
		End Function

		Private Function decodeRoundRect4() As RoundRectangle2D
			roundRect.roundRectect(decodeX(0.08571429f), decodeY(0.08571429f), decodeX(2.914286f) - decodeX(0.08571429f), decodeY(2.914286f) - decodeY(0.08571429f), 11.0f, 11.0f) 'rounding - height - width - y - x
			Return roundRect
		End Function

		Private Function decodeRoundRect5() As RoundRectangle2D
			roundRect.roundRectect(decodeX(0.2857143f), decodeY(0.42857143f), decodeX(2.7142859f) - decodeX(0.2857143f), decodeY(2.857143f) - decodeY(0.42857143f), 9.0f, 9.0f) 'rounding - height - width - y - x
			Return roundRect
		End Function



		Private Function decodeGradient1(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.05f,0.5f,0.95f }, New Color() { color2, decodeColor(color2,color3,0.5f), color3})
		End Function

		Private Function decodeGradient2(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.024f,0.06f,0.276f,0.6f,0.65f,0.7f,0.856f,0.96f,0.98399997f,1.0f }, New Color() { CType(componentColors(0), Color), decodeColor(CType(componentColors(0), Color),CType(componentColors(1), Color),0.5f), CType(componentColors(1), Color), decodeColor(CType(componentColors(1), Color),CType(componentColors(2), Color),0.5f), CType(componentColors(2), Color), decodeColor(CType(componentColors(2), Color),CType(componentColors(2), Color),0.5f), CType(componentColors(2), Color), decodeColor(CType(componentColors(2), Color),CType(componentColors(3), Color),0.5f), CType(componentColors(3), Color), decodeColor(CType(componentColors(3), Color),CType(componentColors(4), Color),0.5f), CType(componentColors(4), Color)})
		End Function

		Private Function decodeGradient3(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.05f,0.5f,0.95f }, New Color() { color10, decodeColor(color10,color11,0.5f), color11})
		End Function

		Private Function decodeGradient4(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.05f,0.5f,0.95f }, New Color() { color18, decodeColor(color18,color19,0.5f), color19})
		End Function

		Private Function decodeGradient5(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.09f,0.52f,0.95f }, New Color() { color26, decodeColor(color26,color27,0.5f), color27})
		End Function

		Private Function decodeGradient6(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.03f,0.06f,0.33f,0.6f,0.65f,0.7f,0.825f,0.95f,0.975f,1.0f }, New Color() { color28, decodeColor(color28,color29,0.5f), color29, decodeColor(color29,color30,0.5f), color30, decodeColor(color30,color30,0.5f), color30, decodeColor(color30,color31,0.5f), color31, decodeColor(color31,color32,0.5f), color32})
		End Function

		Private Function decodeGradient7(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.09f,0.52f,0.95f }, New Color() { color33, decodeColor(color33,color34,0.5f), color34})
		End Function

		Private Function decodeGradient8(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.03f,0.06f,0.33f,0.6f,0.65f,0.7f,0.825f,0.95f,0.975f,1.0f }, New Color() { CType(componentColors(0), Color), decodeColor(CType(componentColors(0), Color),CType(componentColors(1), Color),0.5f), CType(componentColors(1), Color), decodeColor(CType(componentColors(1), Color),CType(componentColors(2), Color),0.5f), CType(componentColors(2), Color), decodeColor(CType(componentColors(2), Color),CType(componentColors(2), Color),0.5f), CType(componentColors(2), Color), decodeColor(CType(componentColors(2), Color),CType(componentColors(3), Color),0.5f), CType(componentColors(3), Color), decodeColor(CType(componentColors(3), Color),CType(componentColors(4), Color),0.5f), CType(componentColors(4), Color)})
		End Function

		Private Function decodeGradient9(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.09f,0.52f,0.95f }, New Color() { color39, decodeColor(color39,color40,0.5f), color40})
		End Function

		Private Function decodeGradient10(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.024f,0.06f,0.276f,0.6f,0.65f,0.7f,0.856f,0.96f,0.98f,1.0f }, New Color() { CType(componentColors(0), Color), decodeColor(CType(componentColors(0), Color),CType(componentColors(1), Color),0.5f), CType(componentColors(1), Color), decodeColor(CType(componentColors(1), Color),CType(componentColors(2), Color),0.5f), CType(componentColors(2), Color), decodeColor(CType(componentColors(2), Color),CType(componentColors(2), Color),0.5f), CType(componentColors(2), Color), decodeColor(CType(componentColors(2), Color),CType(componentColors(3), Color),0.5f), CType(componentColors(3), Color), decodeColor(CType(componentColors(3), Color),CType(componentColors(3), Color),0.5f), CType(componentColors(3), Color)})
		End Function

		Private Function decodeGradient11(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.05f,0.5f,0.95f }, New Color() { color45, decodeColor(color45,color46,0.5f), color46})
		End Function


	End Class

End Namespace
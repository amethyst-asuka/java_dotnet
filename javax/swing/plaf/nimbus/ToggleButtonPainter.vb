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



	Friend NotInheritable Class ToggleButtonPainter
		Inherits AbstractRegionPainter

		'package private integers representing the available states that
		'this painter will paint. These are used when creating a new instance
		'of ToggleButtonPainter to determine which region/state is being painted
		'by that instance.
		Friend Const BACKGROUND_DISABLED As Integer = 1
		Friend Const BACKGROUND_ENABLED As Integer = 2
		Friend Const BACKGROUND_FOCUSED As Integer = 3
		Friend Const BACKGROUND_MOUSEOVER As Integer = 4
		Friend Const BACKGROUND_MOUSEOVER_FOCUSED As Integer = 5
		Friend Const BACKGROUND_PRESSED As Integer = 6
		Friend Const BACKGROUND_PRESSED_FOCUSED As Integer = 7
		Friend Const BACKGROUND_SELECTED As Integer = 8
		Friend Const BACKGROUND_SELECTED_FOCUSED As Integer = 9
		Friend Const BACKGROUND_PRESSED_SELECTED As Integer = 10
		Friend Const BACKGROUND_PRESSED_SELECTED_FOCUSED As Integer = 11
		Friend Const BACKGROUND_MOUSEOVER_SELECTED As Integer = 12
		Friend Const BACKGROUND_MOUSEOVER_SELECTED_FOCUSED As Integer = 13
		Friend Const BACKGROUND_DISABLED_SELECTED As Integer = 14


		Private ___state As Integer 'refers to one of the static final ints above
		Private ctx As PaintContext

		'the following 4 variables are reused during the painting code of the layers
		Private path As Path2D = New Path2D.Float
		Private rect As Rectangle2D = New Rectangle2D.Float(0, 0, 0, 0)
		Private roundRect As RoundRectangle2D = New RoundRectangle2D.Float(0, 0, 0, 0, 0, 0)
		Private ellipse As Ellipse2D = New Ellipse2D.Float(0, 0, 0, 0)

		'All Colors used for painting are stored here. Ideally, only those colors being used
		'by a particular instance of ToggleButtonPainter would be created. For the moment at least,
		'however, all are created for each instance.
		Private color1 As Color = decodeColor("nimbusBlueGrey", -0.027777791f, -0.06885965f, -0.36862746f, -232)
		Private color2 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.06766917f, 0.07843137f, 0)
		Private color3 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.06484103f, 0.027450979f, 0)
		Private color4 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.08477524f, 0.16862744f, 0)
		Private color5 As Color = decodeColor("nimbusBlueGrey", -0.015872955f, -0.080091536f, 0.15686274f, 0)
		Private color6 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.07016757f, 0.12941176f, 0)
		Private color7 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.07052632f, 0.1372549f, 0)
		Private color8 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.070878744f, 0.14509803f, 0)
		Private color9 As Color = decodeColor("nimbusBlueGrey", -0.027777791f, -0.06885965f, -0.36862746f, -190)
		Private color10 As Color = decodeColor("nimbusBlueGrey", -0.055555522f, -0.05356429f, -0.12549019f, 0)
		Private color11 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.0147816315f, -0.3764706f, 0)
		Private color12 As Color = decodeColor("nimbusBlueGrey", 0.055555582f, -0.10655806f, 0.24313724f, 0)
		Private color13 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.09823123f, 0.2117647f, 0)
		Private color14 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.0749532f, 0.24705881f, 0)
		Private color15 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.110526316f, 0.25490195f, 0)
		Private color16 As Color = decodeColor("nimbusFocus", 0.0f, 0.0f, 0.0f, 0)
		Private color17 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.020974077f, -0.21960783f, 0)
		Private color18 As Color = decodeColor("nimbusBlueGrey", 0.0f, 0.11169591f, -0.53333336f, 0)
		Private color19 As Color = decodeColor("nimbusBlueGrey", 0.055555582f, -0.10658931f, 0.25098038f, 0)
		Private color20 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.098526314f, 0.2352941f, 0)
		Private color21 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.07333623f, 0.20392156f, 0)
		Private color22 As New Color(245, 250, 255, 160)
		Private color23 As Color = decodeColor("nimbusBlueGrey", 0.055555582f, 0.8894737f, -0.7176471f, 0)
		Private color24 As Color = decodeColor("nimbusBlueGrey", 0.0f, 5.847961E-4f, -0.32156864f, 0)
		Private color25 As Color = decodeColor("nimbusBlueGrey", -0.00505054f, -0.05960039f, 0.10196078f, 0)
		Private color26 As Color = decodeColor("nimbusBlueGrey", -0.008547008f, -0.04772438f, 0.06666666f, 0)
		Private color27 As Color = decodeColor("nimbusBlueGrey", -0.0027777553f, -0.0018306673f, -0.02352941f, 0)
		Private color28 As Color = decodeColor("nimbusBlueGrey", -0.0027777553f, -0.0212406f, 0.13333333f, 0)
		Private color29 As Color = decodeColor("nimbusBlueGrey", 0.0055555105f, -0.030845039f, 0.23921567f, 0)
		Private color30 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.110526316f, 0.25490195f, -86)
		Private color31 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.06472479f, -0.23137254f, 0)
		Private color32 As Color = decodeColor("nimbusBlueGrey", 0.007936537f, -0.06959064f, -0.0745098f, 0)
		Private color33 As Color = decodeColor("nimbusBlueGrey", 0.0138888955f, -0.06401469f, -0.07058823f, 0)
		Private color34 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.06530018f, 0.035294116f, 0)
		Private color35 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.06507177f, 0.031372547f, 0)
		Private color36 As Color = decodeColor("nimbusBlueGrey", -0.027777791f, -0.05338346f, -0.47058824f, 0)
		Private color37 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.049301825f, -0.36078432f, 0)
		Private color38 As Color = decodeColor("nimbusBlueGrey", -0.018518567f, -0.03909774f, -0.2509804f, 0)
		Private color39 As Color = decodeColor("nimbusBlueGrey", -0.00505054f, -0.040013492f, -0.13333333f, 0)
		Private color40 As Color = decodeColor("nimbusBlueGrey", 0.01010108f, -0.039558575f, -0.1372549f, 0)
		Private color41 As Color = decodeColor("nimbusBlueGrey", -0.01111114f, -0.060526315f, -0.3529412f, 0)
		Private color42 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.064372465f, -0.2352941f, 0)
		Private color43 As Color = decodeColor("nimbusBlueGrey", -0.006944418f, -0.0595709f, -0.12941176f, 0)
		Private color44 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.061075766f, -0.031372547f, 0)
		Private color45 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.06080256f, -0.035294116f, 0)
		Private color46 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.110526316f, 0.25490195f, -220)
		Private color47 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.066408664f, 0.054901958f, 0)
		Private color48 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.06807348f, 0.086274505f, 0)
		Private color49 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.06924191f, 0.109803915f, 0)


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
				Case BACKGROUND_SELECTED
					paintBackgroundSelected(g)
				Case BACKGROUND_SELECTED_FOCUSED
					paintBackgroundSelectedAndFocused(g)
				Case BACKGROUND_PRESSED_SELECTED
					paintBackgroundPressedAndSelected(g)
				Case BACKGROUND_PRESSED_SELECTED_FOCUSED
					paintBackgroundPressedAndSelectedAndFocused(g)
				Case BACKGROUND_MOUSEOVER_SELECTED
					paintBackgroundMouseOverAndSelected(g)
				Case BACKGROUND_MOUSEOVER_SELECTED_FOCUSED
					paintBackgroundMouseOverAndSelectedAndFocused(g)
				Case BACKGROUND_DISABLED_SELECTED
					paintBackgroundDisabledAndSelected(g)

			End Select
		End Sub

		Protected Friend Overrides Function getExtendedCacheKeys(ByVal c As JComponent) As Object()
			Dim ___extendedCacheKeys As Object() = Nothing
			Select Case ___state
				Case BACKGROUND_ENABLED
					___extendedCacheKeys = New Object() { getComponentColor(c, "background", color12, -0.10655806f, 0.24313724f, 0), getComponentColor(c, "background", color13, -0.09823123f, 0.2117647f, 0), getComponentColor(c, "background", color6, -0.07016757f, 0.12941176f, 0), getComponentColor(c, "background", color14, -0.0749532f, 0.24705881f, 0), getComponentColor(c, "background", color15, -0.110526316f, 0.25490195f, 0)}
				Case BACKGROUND_FOCUSED
					___extendedCacheKeys = New Object() { getComponentColor(c, "background", color12, -0.10655806f, 0.24313724f, 0), getComponentColor(c, "background", color13, -0.09823123f, 0.2117647f, 0), getComponentColor(c, "background", color6, -0.07016757f, 0.12941176f, 0), getComponentColor(c, "background", color14, -0.0749532f, 0.24705881f, 0), getComponentColor(c, "background", color15, -0.110526316f, 0.25490195f, 0)}
				Case BACKGROUND_MOUSEOVER
					___extendedCacheKeys = New Object() { getComponentColor(c, "background", color19, -0.10658931f, 0.25098038f, 0), getComponentColor(c, "background", color20, -0.098526314f, 0.2352941f, 0), getComponentColor(c, "background", color21, -0.07333623f, 0.20392156f, 0), getComponentColor(c, "background", color15, -0.110526316f, 0.25490195f, 0)}
				Case BACKGROUND_MOUSEOVER_FOCUSED
					___extendedCacheKeys = New Object() { getComponentColor(c, "background", color19, -0.10658931f, 0.25098038f, 0), getComponentColor(c, "background", color20, -0.098526314f, 0.2352941f, 0), getComponentColor(c, "background", color21, -0.07333623f, 0.20392156f, 0), getComponentColor(c, "background", color15, -0.110526316f, 0.25490195f, 0)}
				Case BACKGROUND_PRESSED
					___extendedCacheKeys = New Object() { getComponentColor(c, "background", color25, -0.05960039f, 0.10196078f, 0), getComponentColor(c, "background", color26, -0.04772438f, 0.06666666f, 0), getComponentColor(c, "background", color27, -0.0018306673f, -0.02352941f, 0), getComponentColor(c, "background", color28, -0.0212406f, 0.13333333f, 0), getComponentColor(c, "background", color29, -0.030845039f, 0.23921567f, 0)}
				Case BACKGROUND_PRESSED_FOCUSED
					___extendedCacheKeys = New Object() { getComponentColor(c, "background", color25, -0.05960039f, 0.10196078f, 0), getComponentColor(c, "background", color26, -0.04772438f, 0.06666666f, 0), getComponentColor(c, "background", color27, -0.0018306673f, -0.02352941f, 0), getComponentColor(c, "background", color28, -0.0212406f, 0.13333333f, 0), getComponentColor(c, "background", color29, -0.030845039f, 0.23921567f, 0)}
				Case BACKGROUND_SELECTED
					___extendedCacheKeys = New Object() { getComponentColor(c, "background", color33, -0.06401469f, -0.07058823f, 0), getComponentColor(c, "background", color34, -0.06530018f, 0.035294116f, 0), getComponentColor(c, "background", color35, -0.06507177f, 0.031372547f, 0)}
				Case BACKGROUND_SELECTED_FOCUSED
					___extendedCacheKeys = New Object() { getComponentColor(c, "background", color33, -0.06401469f, -0.07058823f, 0), getComponentColor(c, "background", color34, -0.06530018f, 0.035294116f, 0), getComponentColor(c, "background", color35, -0.06507177f, 0.031372547f, 0)}
				Case BACKGROUND_PRESSED_SELECTED
					___extendedCacheKeys = New Object() { getComponentColor(c, "background", color38, -0.03909774f, -0.2509804f, 0), getComponentColor(c, "background", color39, -0.040013492f, -0.13333333f, 0), getComponentColor(c, "background", color40, -0.039558575f, -0.1372549f, 0)}
				Case BACKGROUND_PRESSED_SELECTED_FOCUSED
					___extendedCacheKeys = New Object() { getComponentColor(c, "background", color38, -0.03909774f, -0.2509804f, 0), getComponentColor(c, "background", color39, -0.040013492f, -0.13333333f, 0), getComponentColor(c, "background", color40, -0.039558575f, -0.1372549f, 0)}
				Case BACKGROUND_MOUSEOVER_SELECTED
					___extendedCacheKeys = New Object() { getComponentColor(c, "background", color43, -0.0595709f, -0.12941176f, 0), getComponentColor(c, "background", color44, -0.061075766f, -0.031372547f, 0), getComponentColor(c, "background", color45, -0.06080256f, -0.035294116f, 0)}
				Case BACKGROUND_MOUSEOVER_SELECTED_FOCUSED
					___extendedCacheKeys = New Object() { getComponentColor(c, "background", color43, -0.0595709f, -0.12941176f, 0), getComponentColor(c, "background", color44, -0.061075766f, -0.031372547f, 0), getComponentColor(c, "background", color45, -0.06080256f, -0.035294116f, 0)}
			End Select
			Return ___extendedCacheKeys
		End Function

		Protected Friend Property NotOverridable Overrides paintContext As PaintContext
			Get
				Return ctx
			End Get
		End Property

		Private Sub paintBackgroundDisabled(ByVal g As Graphics2D)
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

		Private Sub paintBackgroundEnabled(ByVal g As Graphics2D)
			roundRect = decodeRoundRect1()
			g.paint = color9
			g.fill(roundRect)
			roundRect = decodeRoundRect2()
			g.paint = decodeGradient3(roundRect)
			g.fill(roundRect)
			roundRect = decodeRoundRect3()
			g.paint = decodeGradient4(roundRect)
			g.fill(roundRect)

		End Sub

		Private Sub paintBackgroundFocused(ByVal g As Graphics2D)
			roundRect = decodeRoundRect4()
			g.paint = color16
			g.fill(roundRect)
			roundRect = decodeRoundRect2()
			g.paint = decodeGradient3(roundRect)
			g.fill(roundRect)
			roundRect = decodeRoundRect3()
			g.paint = decodeGradient5(roundRect)
			g.fill(roundRect)

		End Sub

		Private Sub paintBackgroundMouseOver(ByVal g As Graphics2D)
			roundRect = decodeRoundRect1()
			g.paint = color9
			g.fill(roundRect)
			roundRect = decodeRoundRect2()
			g.paint = decodeGradient6(roundRect)
			g.fill(roundRect)
			roundRect = decodeRoundRect3()
			g.paint = decodeGradient7(roundRect)
			g.fill(roundRect)

		End Sub

		Private Sub paintBackgroundMouseOverAndFocused(ByVal g As Graphics2D)
			roundRect = decodeRoundRect4()
			g.paint = color16
			g.fill(roundRect)
			roundRect = decodeRoundRect2()
			g.paint = decodeGradient6(roundRect)
			g.fill(roundRect)
			roundRect = decodeRoundRect3()
			g.paint = decodeGradient7(roundRect)
			g.fill(roundRect)

		End Sub

		Private Sub paintBackgroundPressed(ByVal g As Graphics2D)
			roundRect = decodeRoundRect1()
			g.paint = color22
			g.fill(roundRect)
			roundRect = decodeRoundRect2()
			g.paint = decodeGradient8(roundRect)
			g.fill(roundRect)
			roundRect = decodeRoundRect3()
			g.paint = decodeGradient4(roundRect)
			g.fill(roundRect)

		End Sub

		Private Sub paintBackgroundPressedAndFocused(ByVal g As Graphics2D)
			roundRect = decodeRoundRect4()
			g.paint = color16
			g.fill(roundRect)
			roundRect = decodeRoundRect2()
			g.paint = decodeGradient8(roundRect)
			g.fill(roundRect)
			roundRect = decodeRoundRect3()
			g.paint = decodeGradient4(roundRect)
			g.fill(roundRect)

		End Sub

		Private Sub paintBackgroundSelected(ByVal g As Graphics2D)
			roundRect = decodeRoundRect5()
			g.paint = color30
			g.fill(roundRect)
			roundRect = decodeRoundRect2()
			g.paint = decodeGradient9(roundRect)
			g.fill(roundRect)
			roundRect = decodeRoundRect3()
			g.paint = decodeGradient10(roundRect)
			g.fill(roundRect)

		End Sub

		Private Sub paintBackgroundSelectedAndFocused(ByVal g As Graphics2D)
			roundRect = decodeRoundRect6()
			g.paint = color16
			g.fill(roundRect)
			roundRect = decodeRoundRect2()
			g.paint = decodeGradient9(roundRect)
			g.fill(roundRect)
			roundRect = decodeRoundRect3()
			g.paint = decodeGradient10(roundRect)
			g.fill(roundRect)

		End Sub

		Private Sub paintBackgroundPressedAndSelected(ByVal g As Graphics2D)
			roundRect = decodeRoundRect5()
			g.paint = color30
			g.fill(roundRect)
			roundRect = decodeRoundRect2()
			g.paint = decodeGradient11(roundRect)
			g.fill(roundRect)
			roundRect = decodeRoundRect3()
			g.paint = decodeGradient10(roundRect)
			g.fill(roundRect)

		End Sub

		Private Sub paintBackgroundPressedAndSelectedAndFocused(ByVal g As Graphics2D)
			roundRect = decodeRoundRect6()
			g.paint = color16
			g.fill(roundRect)
			roundRect = decodeRoundRect2()
			g.paint = decodeGradient11(roundRect)
			g.fill(roundRect)
			roundRect = decodeRoundRect3()
			g.paint = decodeGradient10(roundRect)
			g.fill(roundRect)

		End Sub

		Private Sub paintBackgroundMouseOverAndSelected(ByVal g As Graphics2D)
			roundRect = decodeRoundRect5()
			g.paint = color30
			g.fill(roundRect)
			roundRect = decodeRoundRect2()
			g.paint = decodeGradient12(roundRect)
			g.fill(roundRect)
			roundRect = decodeRoundRect3()
			g.paint = decodeGradient10(roundRect)
			g.fill(roundRect)

		End Sub

		Private Sub paintBackgroundMouseOverAndSelectedAndFocused(ByVal g As Graphics2D)
			roundRect = decodeRoundRect6()
			g.paint = color16
			g.fill(roundRect)
			roundRect = decodeRoundRect2()
			g.paint = decodeGradient12(roundRect)
			g.fill(roundRect)
			roundRect = decodeRoundRect3()
			g.paint = decodeGradient10(roundRect)
			g.fill(roundRect)

		End Sub

		Private Sub paintBackgroundDisabledAndSelected(ByVal g As Graphics2D)
			roundRect = decodeRoundRect5()
			g.paint = color46
			g.fill(roundRect)
			roundRect = decodeRoundRect2()
			g.paint = decodeGradient13(roundRect)
			g.fill(roundRect)
			roundRect = decodeRoundRect3()
			g.paint = decodeGradient14(roundRect)
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

		Private Function decodeRoundRect6() As RoundRectangle2D
			roundRect.roundRectect(decodeX(0.08571429f), decodeY(0.08571429f), decodeX(2.914286f) - decodeX(0.08571429f), decodeY(2.9142857f) - decodeY(0.08571429f), 11.0f, 11.0f) 'rounding - height - width - y - x
			Return roundRect
		End Function



		Private Function decodeGradient1(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.09f,0.52f,0.95f }, New Color() { color2, decodeColor(color2,color3,0.5f), color3})
		End Function

		Private Function decodeGradient2(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.03f,0.06f,0.33f,0.6f,0.65f,0.7f,0.825f,0.95f,0.975f,1.0f }, New Color() { color4, decodeColor(color4,color5,0.5f), color5, decodeColor(color5,color6,0.5f), color6, decodeColor(color6,color6,0.5f), color6, decodeColor(color6,color7,0.5f), color7, decodeColor(color7,color8,0.5f), color8})
		End Function

		Private Function decodeGradient3(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.09f,0.52f,0.95f }, New Color() { color10, decodeColor(color10,color11,0.5f), color11})
		End Function

		Private Function decodeGradient4(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.024f,0.06f,0.276f,0.6f,0.65f,0.7f,0.856f,0.96f,0.98399997f,1.0f }, New Color() { CType(componentColors(0), Color), decodeColor(CType(componentColors(0), Color),CType(componentColors(1), Color),0.5f), CType(componentColors(1), Color), decodeColor(CType(componentColors(1), Color),CType(componentColors(2), Color),0.5f), CType(componentColors(2), Color), decodeColor(CType(componentColors(2), Color),CType(componentColors(2), Color),0.5f), CType(componentColors(2), Color), decodeColor(CType(componentColors(2), Color),CType(componentColors(3), Color),0.5f), CType(componentColors(3), Color), decodeColor(CType(componentColors(3), Color),CType(componentColors(4), Color),0.5f), CType(componentColors(4), Color)})
		End Function

		Private Function decodeGradient5(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.03f,0.06f,0.33f,0.6f,0.65f,0.7f,0.825f,0.95f,0.975f,1.0f }, New Color() { CType(componentColors(0), Color), decodeColor(CType(componentColors(0), Color),CType(componentColors(1), Color),0.5f), CType(componentColors(1), Color), decodeColor(CType(componentColors(1), Color),CType(componentColors(2), Color),0.5f), CType(componentColors(2), Color), decodeColor(CType(componentColors(2), Color),CType(componentColors(2), Color),0.5f), CType(componentColors(2), Color), decodeColor(CType(componentColors(2), Color),CType(componentColors(3), Color),0.5f), CType(componentColors(3), Color), decodeColor(CType(componentColors(3), Color),CType(componentColors(4), Color),0.5f), CType(componentColors(4), Color)})
		End Function

		Private Function decodeGradient6(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.09f,0.52f,0.95f }, New Color() { color17, decodeColor(color17,color18,0.5f), color18})
		End Function

		Private Function decodeGradient7(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.024f,0.06f,0.276f,0.6f,0.65f,0.7f,0.856f,0.96f,0.98f,1.0f }, New Color() { CType(componentColors(0), Color), decodeColor(CType(componentColors(0), Color),CType(componentColors(1), Color),0.5f), CType(componentColors(1), Color), decodeColor(CType(componentColors(1), Color),CType(componentColors(2), Color),0.5f), CType(componentColors(2), Color), decodeColor(CType(componentColors(2), Color),CType(componentColors(2), Color),0.5f), CType(componentColors(2), Color), decodeColor(CType(componentColors(2), Color),CType(componentColors(3), Color),0.5f), CType(componentColors(3), Color), decodeColor(CType(componentColors(3), Color),CType(componentColors(3), Color),0.5f), CType(componentColors(3), Color)})
		End Function

		Private Function decodeGradient8(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.05f,0.5f,0.95f }, New Color() { color23, decodeColor(color23,color24,0.5f), color24})
		End Function

		Private Function decodeGradient9(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color31, decodeColor(color31,color32,0.5f), color32})
		End Function

		Private Function decodeGradient10(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.06684492f,0.13368984f,0.56684494f,1.0f }, New Color() { CType(componentColors(0), Color), decodeColor(CType(componentColors(0), Color),CType(componentColors(1), Color),0.5f), CType(componentColors(1), Color), decodeColor(CType(componentColors(1), Color),CType(componentColors(2), Color),0.5f), CType(componentColors(2), Color)})
		End Function

		Private Function decodeGradient11(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color36, decodeColor(color36,color37,0.5f), color37})
		End Function

		Private Function decodeGradient12(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color41, decodeColor(color41,color42,0.5f), color42})
		End Function

		Private Function decodeGradient13(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color47, decodeColor(color47,color48,0.5f), color48})
		End Function

		Private Function decodeGradient14(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.06684492f,0.13368984f,0.56684494f,1.0f }, New Color() { color48, decodeColor(color48,color49,0.5f), color49, decodeColor(color49,color49,0.5f), color49})
		End Function


	End Class

End Namespace
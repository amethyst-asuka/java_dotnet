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



	Friend NotInheritable Class InternalFrameTitlePaneCloseButtonPainter
		Inherits AbstractRegionPainter

		'package private integers representing the available states that
		'this painter will paint. These are used when creating a new instance
		'of InternalFrameTitlePaneCloseButtonPainter to determine which region/state is being painted
		'by that instance.
		Friend Const BACKGROUND_DISABLED As Integer = 1
		Friend Const BACKGROUND_ENABLED As Integer = 2
		Friend Const BACKGROUND_MOUSEOVER As Integer = 3
		Friend Const BACKGROUND_PRESSED As Integer = 4
		Friend Const BACKGROUND_ENABLED_WINDOWNOTFOCUSED As Integer = 5
		Friend Const BACKGROUND_MOUSEOVER_WINDOWNOTFOCUSED As Integer = 6
		Friend Const BACKGROUND_PRESSED_WINDOWNOTFOCUSED As Integer = 7


		Private state As Integer 'refers to one of the static final ints above
		Private ctx As PaintContext

		'the following 4 variables are reused during the painting code of the layers
		Private path As Path2D = New Path2D.Float
		Private rect As Rectangle2D = New Rectangle2D.Float(0, 0, 0, 0)
		Private roundRect As RoundRectangle2D = New RoundRectangle2D.Float(0, 0, 0, 0, 0, 0)
		Private ellipse As Ellipse2D = New Ellipse2D.Float(0, 0, 0, 0)

		'All Colors used for painting are stored here. Ideally, only those colors being used
		'by a particular instance of InternalFrameTitlePaneCloseButtonPainter would be created. For the moment at least,
		'however, all are created for each instance.
		Private color1 As Color = decodeColor("nimbusRed", 0.5893519f, -0.75736576f, 0.09411764f, 0)
		Private color2 As Color = decodeColor("nimbusRed", 0.5962963f, -0.71005917f, 0.0f, 0)
		Private color3 As Color = decodeColor("nimbusRed", 0.6005698f, -0.7200287f, -0.015686274f, -122)
		Private color4 As Color = decodeColor("nimbusBlueGrey", 0.0055555105f, -0.062449392f, 0.07058823f, 0)
		Private color5 As Color = decodeColor("nimbusBlueGrey", 0.0055555105f, -0.0029994324f, -0.38039216f, -185)
		Private color6 As Color = decodeColor("nimbusRed", -0.014814814f, 0.20118344f, -0.4431373f, 0)
		Private color7 As Color = decodeColor("nimbusRed", -2.7342606E-4f, 0.13829035f, -0.039215684f, 0)
		Private color8 As Color = decodeColor("nimbusRed", 6.890595E-4f, -0.36665577f, 0.11764705f, 0)
		Private color9 As Color = decodeColor("nimbusRed", -0.001021713f, 0.101804554f, -0.031372547f, 0)
		Private color10 As Color = decodeColor("nimbusRed", -2.7342606E-4f, 0.13243341f, -0.035294116f, 0)
		Private color11 As Color = decodeColor("nimbusRed", -2.7342606E-4f, 0.002258718f, 0.06666666f, 0)
		Private color12 As Color = decodeColor("nimbusRed", 0.0056530247f, 0.0040003657f, -0.38431373f, -122)
		Private color13 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.110526316f, 0.25490195f, 0)
		Private color14 As Color = decodeColor("nimbusRed", -0.014814814f, 0.20118344f, -0.3882353f, 0)
		Private color15 As Color = decodeColor("nimbusRed", -0.014814814f, 0.20118344f, -0.13333333f, 0)
		Private color16 As Color = decodeColor("nimbusRed", 6.890595E-4f, -0.38929275f, 0.1607843f, 0)
		Private color17 As Color = decodeColor("nimbusRed", 2.537202E-5f, 0.012294531f, 0.043137252f, 0)
		Private color18 As Color = decodeColor("nimbusRed", -2.7342606E-4f, 0.033585668f, 0.039215684f, 0)
		Private color19 As Color = decodeColor("nimbusRed", -2.7342606E-4f, -0.07198727f, 0.14117646f, 0)
		Private color20 As Color = decodeColor("nimbusRed", -0.014814814f, 0.20118344f, 0.0039215684f, -122)
		Private color21 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.110526316f, 0.25490195f, -140)
		Private color22 As Color = decodeColor("nimbusRed", -0.014814814f, 0.20118344f, -0.49411768f, 0)
		Private color23 As Color = decodeColor("nimbusRed", -0.014814814f, 0.20118344f, -0.20392159f, 0)
		Private color24 As Color = decodeColor("nimbusRed", -0.014814814f, -0.21260965f, 0.019607842f, 0)
		Private color25 As Color = decodeColor("nimbusRed", -0.014814814f, 0.17340565f, -0.09803921f, 0)
		Private color26 As Color = decodeColor("nimbusRed", -0.014814814f, 0.20118344f, -0.10588235f, 0)
		Private color27 As Color = decodeColor("nimbusRed", -0.014814814f, 0.20118344f, -0.04705882f, 0)
		Private color28 As Color = decodeColor("nimbusRed", -0.014814814f, 0.20118344f, -0.31764707f, -122)
		Private color29 As Color = decodeColor("nimbusRed", 0.5962963f, -0.6994788f, -0.07058823f, 0)
		Private color30 As Color = decodeColor("nimbusRed", 0.5962963f, -0.66245294f, -0.23137257f, 0)
		Private color31 As Color = decodeColor("nimbusRed", 0.58518517f, -0.77649516f, 0.21568626f, 0)
		Private color32 As Color = decodeColor("nimbusRed", 0.5962963f, -0.7372781f, 0.10196078f, 0)
		Private color33 As Color = decodeColor("nimbusRed", 0.5962963f, -0.73911506f, 0.12549019f, 0)
		Private color34 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.027957506f, -0.31764707f, 0)


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
				Case BACKGROUND_MOUSEOVER
					paintBackgroundMouseOver(g)
				Case BACKGROUND_PRESSED
					paintBackgroundPressed(g)
				Case BACKGROUND_ENABLED_WINDOWNOTFOCUSED
					paintBackgroundEnabledAndWindowNotFocused(g)
				Case BACKGROUND_MOUSEOVER_WINDOWNOTFOCUSED
					paintBackgroundMouseOverAndWindowNotFocused(g)
				Case BACKGROUND_PRESSED_WINDOWNOTFOCUSED
					paintBackgroundPressedAndWindowNotFocused(g)

			End Select
		End Sub



		Protected Friend Property NotOverridable Overrides paintContext As PaintContext
			Get
				Return ctx
			End Get
		End Property

		Private Sub paintBackgroundDisabled(ByVal g As Graphics2D)
			roundRect = decodeRoundRect1()
			g.paint = decodeGradient1(roundRect)
			g.fill(roundRect)
			path = decodePath1()
			g.paint = color3
			g.fill(path)
			path = decodePath2()
			g.paint = color4
			g.fill(path)

		End Sub

		Private Sub paintBackgroundEnabled(ByVal g As Graphics2D)
			roundRect = decodeRoundRect2()
			g.paint = color5
			g.fill(roundRect)
			roundRect = decodeRoundRect1()
			g.paint = decodeGradient2(roundRect)
			g.fill(roundRect)
			roundRect = decodeRoundRect3()
			g.paint = decodeGradient3(roundRect)
			g.fill(roundRect)
			path = decodePath1()
			g.paint = color12
			g.fill(path)
			path = decodePath2()
			g.paint = color13
			g.fill(path)

		End Sub

		Private Sub paintBackgroundMouseOver(ByVal g As Graphics2D)
			roundRect = decodeRoundRect2()
			g.paint = color5
			g.fill(roundRect)
			roundRect = decodeRoundRect1()
			g.paint = decodeGradient4(roundRect)
			g.fill(roundRect)
			roundRect = decodeRoundRect4()
			g.paint = decodeGradient5(roundRect)
			g.fill(roundRect)
			path = decodePath1()
			g.paint = color20
			g.fill(path)
			path = decodePath2()
			g.paint = color13
			g.fill(path)

		End Sub

		Private Sub paintBackgroundPressed(ByVal g As Graphics2D)
			roundRect = decodeRoundRect2()
			g.paint = color21
			g.fill(roundRect)
			roundRect = decodeRoundRect1()
			g.paint = decodeGradient6(roundRect)
			g.fill(roundRect)
			roundRect = decodeRoundRect3()
			g.paint = decodeGradient7(roundRect)
			g.fill(roundRect)
			path = decodePath1()
			g.paint = color28
			g.fill(path)
			path = decodePath2()
			g.paint = color13
			g.fill(path)

		End Sub

		Private Sub paintBackgroundEnabledAndWindowNotFocused(ByVal g As Graphics2D)
			roundRect = decodeRoundRect1()
			g.paint = decodeGradient8(roundRect)
			g.fill(roundRect)
			roundRect = decodeRoundRect3()
			g.paint = decodeGradient9(roundRect)
			g.fill(roundRect)
			path = decodePath2()
			g.paint = color34
			g.fill(path)

		End Sub

		Private Sub paintBackgroundMouseOverAndWindowNotFocused(ByVal g As Graphics2D)
			roundRect = decodeRoundRect2()
			g.paint = color5
			g.fill(roundRect)
			roundRect = decodeRoundRect1()
			g.paint = decodeGradient4(roundRect)
			g.fill(roundRect)
			roundRect = decodeRoundRect4()
			g.paint = decodeGradient5(roundRect)
			g.fill(roundRect)
			path = decodePath1()
			g.paint = color20
			g.fill(path)
			path = decodePath2()
			g.paint = color13
			g.fill(path)

		End Sub

		Private Sub paintBackgroundPressedAndWindowNotFocused(ByVal g As Graphics2D)
			roundRect = decodeRoundRect2()
			g.paint = color21
			g.fill(roundRect)
			roundRect = decodeRoundRect1()
			g.paint = decodeGradient6(roundRect)
			g.fill(roundRect)
			roundRect = decodeRoundRect3()
			g.paint = decodeGradient7(roundRect)
			g.fill(roundRect)
			path = decodePath1()
			g.paint = color28
			g.fill(path)
			path = decodePath2()
			g.paint = color13
			g.fill(path)

		End Sub



		Private Function decodeRoundRect1() As RoundRectangle2D
			roundRect.roundRectect(decodeX(1.0f), decodeY(1.0f), decodeX(2.0f) - decodeX(1.0f), decodeY(1.9444444f) - decodeY(1.0f), 8.6f, 8.6f) 'rounding - height - width - y - x
			Return roundRect
		End Function

		Private Function decodePath1() As Path2D
			path.reset()
			path.moveTo(decodeX(1.25f), decodeY(1.7373737f))
			path.lineTo(decodeX(1.3002392f), decodeY(1.794192f))
			path.lineTo(decodeX(1.5047847f), decodeY(1.5909091f))
			path.lineTo(decodeX(1.6842105f), decodeY(1.7954545f))
			path.lineTo(decodeX(1.7595694f), decodeY(1.719697f))
			path.lineTo(decodeX(1.5956938f), decodeY(1.5239899f))
			path.lineTo(decodeX(1.7535884f), decodeY(1.3409091f))
			path.lineTo(decodeX(1.6830144f), decodeY(1.2537879f))
			path.lineTo(decodeX(1.5083733f), decodeY(1.4406565f))
			path.lineTo(decodeX(1.3301436f), decodeY(1.2563131f))
			path.lineTo(decodeX(1.257177f), decodeY(1.3320707f))
			path.lineTo(decodeX(1.4270334f), decodeY(1.5252526f))
			path.lineTo(decodeX(1.25f), decodeY(1.7373737f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath2() As Path2D
			path.reset()
			path.moveTo(decodeX(1.257177f), decodeY(1.2828283f))
			path.lineTo(decodeX(1.3217703f), decodeY(1.2133838f))
			path.lineTo(decodeX(1.5f), decodeY(1.4040405f))
			path.lineTo(decodeX(1.673445f), decodeY(1.2108586f))
			path.lineTo(decodeX(1.7440192f), decodeY(1.2853535f))
			path.lineTo(decodeX(1.5669856f), decodeY(1.4709597f))
			path.lineTo(decodeX(1.7488039f), decodeY(1.6527778f))
			path.lineTo(decodeX(1.673445f), decodeY(1.7398989f))
			path.lineTo(decodeX(1.4988039f), decodeY(1.5416667f))
			path.lineTo(decodeX(1.3313397f), decodeY(1.7424242f))
			path.lineTo(decodeX(1.2523923f), decodeY(1.6565657f))
			path.lineTo(decodeX(1.4366028f), decodeY(1.4722222f))
			path.lineTo(decodeX(1.257177f), decodeY(1.2828283f))
			path.closePath()
			Return path
		End Function

		Private Function decodeRoundRect2() As RoundRectangle2D
			roundRect.roundRectect(decodeX(1.0f), decodeY(1.6111112f), decodeX(2.0f) - decodeX(1.0f), decodeY(2.0f) - decodeY(1.6111112f), 6.0f, 6.0f) 'rounding - height - width - y - x
			Return roundRect
		End Function

		Private Function decodeRoundRect3() As RoundRectangle2D
			roundRect.roundRectect(decodeX(1.0526316f), decodeY(1.0530303f), decodeX(1.9473684f) - decodeX(1.0526316f), decodeY(1.8863636f) - decodeY(1.0530303f), 6.75f, 6.75f) 'rounding - height - width - y - x
			Return roundRect
		End Function

		Private Function decodeRoundRect4() As RoundRectangle2D
			roundRect.roundRectect(decodeX(1.0526316f), decodeY(1.0517677f), decodeX(1.9473684f) - decodeX(1.0526316f), decodeY(1.8851011f) - decodeY(1.0517677f), 6.75f, 6.75f) 'rounding - height - width - y - x
			Return roundRect
		End Function



		Private Function decodeGradient1(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.24868421f * w) + x, (0.0014705883f * h) + y, (0.24868421f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color1, decodeColor(color1,color2,0.5f), color2})
		End Function

		Private Function decodeGradient2(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.24868421f * w) + x, (0.0014705883f * h) + y, (0.24868421f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color6, decodeColor(color6,color7,0.5f), color7})
		End Function

		Private Function decodeGradient3(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.25f * w) + x, (0.0f * h) + y, (0.25441176f * w) + x, (1.0016667f * h) + y, New Single() { 0.0f,0.26988637f,0.53977275f,0.5951705f,0.6505682f,0.8252841f,1.0f }, New Color() { color8, decodeColor(color8,color9,0.5f), color9, decodeColor(color9,color10,0.5f), color10, decodeColor(color10,color11,0.5f), color11})
		End Function

		Private Function decodeGradient4(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.24868421f * w) + x, (0.0014705883f * h) + y, (0.24868421f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color14, decodeColor(color14,color15,0.5f), color15})
		End Function

		Private Function decodeGradient5(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.25f * w) + x, (0.0f * h) + y, (0.25441176f * w) + x, (1.0016667f * h) + y, New Single() { 0.0f,0.26988637f,0.53977275f,0.5951705f,0.6505682f,0.81480503f,0.97904193f }, New Color() { color16, decodeColor(color16,color17,0.5f), color17, decodeColor(color17,color18,0.5f), color18, decodeColor(color18,color19,0.5f), color19})
		End Function

		Private Function decodeGradient6(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.24868421f * w) + x, (0.0014705883f * h) + y, (0.24868421f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color22, decodeColor(color22,color23,0.5f), color23})
		End Function

		Private Function decodeGradient7(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.25f * w) + x, (0.0f * h) + y, (0.25441176f * w) + x, (1.0016667f * h) + y, New Single() { 0.0f,0.26988637f,0.53977275f,0.5951705f,0.6505682f,0.81630206f,0.98203593f }, New Color() { color24, decodeColor(color24,color25,0.5f), color25, decodeColor(color25,color26,0.5f), color26, decodeColor(color26,color27,0.5f), color27})
		End Function

		Private Function decodeGradient8(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.24868421f * w) + x, (0.0014705883f * h) + y, (0.24868421f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color29, decodeColor(color29,color30,0.5f), color30})
		End Function

		Private Function decodeGradient9(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.25f * w) + x, (0.0f * h) + y, (0.25441176f * w) + x, (1.0016667f * h) + y, New Single() { 0.0f,0.24101797f,0.48203593f,0.5838324f,0.6856288f,0.8428144f,1.0f }, New Color() { color31, decodeColor(color31,color32,0.5f), color32, decodeColor(color32,color32,0.5f), color32, decodeColor(color32,color33,0.5f), color33})
		End Function


	End Class

End Namespace
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



	Friend NotInheritable Class InternalFrameTitlePaneMenuButtonPainter
		Inherits AbstractRegionPainter

		'package private integers representing the available states that
		'this painter will paint. These are used when creating a new instance
		'of InternalFrameTitlePaneMenuButtonPainter to determine which region/state is being painted
		'by that instance.
		Friend Const ICON_ENABLED As Integer = 1
		Friend Const ICON_DISABLED As Integer = 2
		Friend Const ICON_MOUSEOVER As Integer = 3
		Friend Const ICON_PRESSED As Integer = 4
		Friend Const ICON_ENABLED_WINDOWNOTFOCUSED As Integer = 5
		Friend Const ICON_MOUSEOVER_WINDOWNOTFOCUSED As Integer = 6
		Friend Const ICON_PRESSED_WINDOWNOTFOCUSED As Integer = 7


		Private state As Integer 'refers to one of the static final ints above
		Private ctx As PaintContext

		'the following 4 variables are reused during the painting code of the layers
		Private path As Path2D = New Path2D.Float
		Private rect As Rectangle2D = New Rectangle2D.Float(0, 0, 0, 0)
		Private roundRect As RoundRectangle2D = New RoundRectangle2D.Float(0, 0, 0, 0, 0, 0)
		Private ellipse As Ellipse2D = New Ellipse2D.Float(0, 0, 0, 0)

		'All Colors used for painting are stored here. Ideally, only those colors being used
		'by a particular instance of InternalFrameTitlePaneMenuButtonPainter would be created. For the moment at least,
		'however, all are created for each instance.
		Private color1 As Color = decodeColor("nimbusBlueGrey", 0.0055555105f, -0.0029994324f, -0.38039216f, -185)
		Private color2 As Color = decodeColor("nimbusBase", 0.08801502f, 0.3642857f, -0.5019608f, 0)
		Private color3 As Color = decodeColor("nimbusBase", 0.030543745f, -0.3835404f, -0.09803924f, 0)
		Private color4 As Color = decodeColor("nimbusBase", 0.029191494f, -0.53801316f, 0.13333333f, 0)
		Private color5 As Color = decodeColor("nimbusBase", 0.030543745f, -0.3857143f, -0.09411767f, 0)
		Private color6 As Color = decodeColor("nimbusBase", 0.030543745f, -0.43148893f, 0.007843137f, 0)
		Private color7 As Color = decodeColor("nimbusBase", 0.029191494f, -0.24935067f, -0.20392159f, -132)
		Private color8 As Color = decodeColor("nimbusBase", 0.029191494f, -0.24935067f, -0.20392159f, 0)
		Private color9 As Color = decodeColor("nimbusBase", 0.029191494f, -0.24935067f, -0.20392159f, -123)
		Private color10 As Color = decodeColor("nimbusBase", 0.0f, -0.6357143f, 0.45098037f, 0)
		Private color11 As Color = decodeColor("nimbusBlueGrey", 0.0055555105f, -0.0029994324f, -0.38039216f, -208)
		Private color12 As Color = decodeColor("nimbusBase", 0.02551502f, -0.5942635f, 0.20784312f, 0)
		Private color13 As Color = decodeColor("nimbusBase", 0.032459438f, -0.5490091f, 0.12941176f, 0)
		Private color14 As Color = decodeColor("nimbusBase", 0.032459438f, -0.5469569f, 0.11372548f, 0)
		Private color15 As Color = decodeColor("nimbusBase", 0.032459438f, -0.5760128f, 0.23921567f, 0)
		Private color16 As Color = decodeColor("nimbusBase", 0.08801502f, 0.3642857f, -0.4901961f, 0)
		Private color17 As Color = decodeColor("nimbusBase", 0.032459438f, -0.1857143f, -0.23529413f, 0)
		Private color18 As Color = decodeColor("nimbusBase", 0.029191494f, -0.5438224f, 0.17647058f, 0)
		Private color19 As Color = decodeColor("nimbusBase", 0.030543745f, -0.41929638f, -0.02352941f, 0)
		Private color20 As Color = decodeColor("nimbusBase", 0.030543745f, -0.45559007f, 0.082352936f, 0)
		Private color21 As Color = decodeColor("nimbusBase", 0.03409344f, -0.329408f, -0.11372551f, -132)
		Private color22 As Color = decodeColor("nimbusBase", 0.03409344f, -0.329408f, -0.11372551f, 0)
		Private color23 As Color = decodeColor("nimbusBase", 0.03409344f, -0.329408f, -0.11372551f, -123)
		Private color24 As Color = decodeColor("nimbusBase", -0.57865167f, -0.6357143f, -0.54901963f, 0)
		Private color25 As Color = decodeColor("nimbusBase", 0.031104386f, 0.12354499f, -0.33725494f, 0)
		Private color26 As Color = decodeColor("nimbusBase", 0.032459438f, -0.4592437f, -0.015686274f, 0)
		Private color27 As Color = decodeColor("nimbusBase", 0.029191494f, -0.2579365f, -0.19607845f, 0)
		Private color28 As Color = decodeColor("nimbusBase", 0.03409344f, -0.3149596f, -0.13333336f, 0)
		Private color29 As Color = decodeColor("nimbusBase", 0.029681683f, 0.07857144f, -0.3294118f, -132)
		Private color30 As Color = decodeColor("nimbusBase", 0.029681683f, 0.07857144f, -0.3294118f, 0)
		Private color31 As Color = decodeColor("nimbusBase", 0.029681683f, 0.07857144f, -0.3294118f, -123)
		Private color32 As Color = decodeColor("nimbusBase", 0.032459438f, -0.53637654f, 0.043137252f, 0)
		Private color33 As Color = decodeColor("nimbusBase", 0.032459438f, -0.49935067f, -0.11764708f, 0)
		Private color34 As Color = decodeColor("nimbusBase", 0.021348298f, -0.6133929f, 0.32941175f, 0)
		Private color35 As Color = decodeColor("nimbusBase", 0.042560518f, -0.5804379f, 0.23137254f, 0)
		Private color36 As Color = decodeColor("nimbusBase", 0.032459438f, -0.57417583f, 0.21568626f, 0)
		Private color37 As Color = decodeColor("nimbusBase", 0.027408898f, -0.5784226f, 0.20392156f, -132)
		Private color38 As Color = decodeColor("nimbusBase", 0.042560518f, -0.5665319f, 0.0745098f, 0)
		Private color39 As Color = decodeColor("nimbusBase", 0.036732912f, -0.5642857f, 0.16470587f, -123)
		Private color40 As Color = decodeColor("nimbusBase", 0.021348298f, -0.54480517f, -0.11764708f, 0)


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
				Case ICON_ENABLED
					painticonEnabled(g)
				Case ICON_DISABLED
					painticonDisabled(g)
				Case ICON_MOUSEOVER
					painticonMouseOver(g)
				Case ICON_PRESSED
					painticonPressed(g)
				Case ICON_ENABLED_WINDOWNOTFOCUSED
					painticonEnabledAndWindowNotFocused(g)
				Case ICON_MOUSEOVER_WINDOWNOTFOCUSED
					painticonMouseOverAndWindowNotFocused(g)
				Case ICON_PRESSED_WINDOWNOTFOCUSED
					painticonPressedAndWindowNotFocused(g)

			End Select
		End Sub



		Protected Friend Property NotOverridable Overrides paintContext As PaintContext
			Get
				Return ctx
			End Get
		End Property

		Private Sub painticonEnabled(ByVal g As Graphics2D)
			roundRect = decodeRoundRect1()
			g.paint = color1
			g.fill(roundRect)
			roundRect = decodeRoundRect2()
			g.paint = decodeGradient1(roundRect)
			g.fill(roundRect)
			roundRect = decodeRoundRect3()
			g.paint = decodeGradient2(roundRect)
			g.fill(roundRect)
			path = decodePath1()
			g.paint = decodeGradient3(path)
			g.fill(path)
			path = decodePath2()
			g.paint = color10
			g.fill(path)

		End Sub

		Private Sub painticonDisabled(ByVal g As Graphics2D)
			roundRect = decodeRoundRect1()
			g.paint = color11
			g.fill(roundRect)
			roundRect = decodeRoundRect2()
			g.paint = decodeGradient4(roundRect)
			g.fill(roundRect)
			path = decodePath2()
			g.paint = color15
			g.fill(path)

		End Sub

		Private Sub painticonMouseOver(ByVal g As Graphics2D)
			roundRect = decodeRoundRect1()
			g.paint = color1
			g.fill(roundRect)
			roundRect = decodeRoundRect2()
			g.paint = decodeGradient5(roundRect)
			g.fill(roundRect)
			roundRect = decodeRoundRect3()
			g.paint = decodeGradient6(roundRect)
			g.fill(roundRect)
			path = decodePath1()
			g.paint = decodeGradient7(path)
			g.fill(path)
			path = decodePath2()
			g.paint = color10
			g.fill(path)

		End Sub

		Private Sub painticonPressed(ByVal g As Graphics2D)
			roundRect = decodeRoundRect1()
			g.paint = color1
			g.fill(roundRect)
			roundRect = decodeRoundRect2()
			g.paint = decodeGradient8(roundRect)
			g.fill(roundRect)
			roundRect = decodeRoundRect3()
			g.paint = decodeGradient9(roundRect)
			g.fill(roundRect)
			path = decodePath1()
			g.paint = decodeGradient10(path)
			g.fill(path)
			path = decodePath2()
			g.paint = color10
			g.fill(path)

		End Sub

		Private Sub painticonEnabledAndWindowNotFocused(ByVal g As Graphics2D)
			roundRect = decodeRoundRect2()
			g.paint = decodeGradient11(roundRect)
			g.fill(roundRect)
			roundRect = decodeRoundRect3()
			g.paint = decodeGradient12(roundRect)
			g.fill(roundRect)
			path = decodePath3()
			g.paint = decodeGradient13(path)
			g.fill(path)
			path = decodePath2()
			g.paint = color40
			g.fill(path)

		End Sub

		Private Sub painticonMouseOverAndWindowNotFocused(ByVal g As Graphics2D)
			roundRect = decodeRoundRect1()
			g.paint = color1
			g.fill(roundRect)
			roundRect = decodeRoundRect2()
			g.paint = decodeGradient5(roundRect)
			g.fill(roundRect)
			roundRect = decodeRoundRect3()
			g.paint = decodeGradient6(roundRect)
			g.fill(roundRect)
			path = decodePath1()
			g.paint = decodeGradient7(path)
			g.fill(path)
			path = decodePath2()
			g.paint = color10
			g.fill(path)

		End Sub

		Private Sub painticonPressedAndWindowNotFocused(ByVal g As Graphics2D)
			roundRect = decodeRoundRect1()
			g.paint = color1
			g.fill(roundRect)
			roundRect = decodeRoundRect2()
			g.paint = decodeGradient8(roundRect)
			g.fill(roundRect)
			roundRect = decodeRoundRect3()
			g.paint = decodeGradient9(roundRect)
			g.fill(roundRect)
			path = decodePath1()
			g.paint = decodeGradient10(path)
			g.fill(path)
			path = decodePath2()
			g.paint = color10
			g.fill(path)

		End Sub



		Private Function decodeRoundRect1() As RoundRectangle2D
			roundRect.roundRectect(decodeX(1.0f), decodeY(1.6111112f), decodeX(2.0f) - decodeX(1.0f), decodeY(2.0f) - decodeY(1.6111112f), 6.0f, 6.0f) 'rounding - height - width - y - x
			Return roundRect
		End Function

		Private Function decodeRoundRect2() As RoundRectangle2D
			roundRect.roundRectect(decodeX(1.0f), decodeY(1.0f), decodeX(2.0f) - decodeX(1.0f), decodeY(1.9444444f) - decodeY(1.0f), 8.6f, 8.6f) 'rounding - height - width - y - x
			Return roundRect
		End Function

		Private Function decodeRoundRect3() As RoundRectangle2D
			roundRect.roundRectect(decodeX(1.0526316f), decodeY(1.0555556f), decodeX(1.9473684f) - decodeX(1.0526316f), decodeY(1.8888888f) - decodeY(1.0555556f), 6.75f, 6.75f) 'rounding - height - width - y - x
			Return roundRect
		End Function

		Private Function decodePath1() As Path2D
			path.reset()
			path.moveTo(decodeX(1.3157895f), decodeY(1.4444444f))
			path.lineTo(decodeX(1.6842105f), decodeY(1.4444444f))
			path.lineTo(decodeX(1.5013158f), decodeY(1.7208333f))
			path.lineTo(decodeX(1.3157895f), decodeY(1.4444444f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath2() As Path2D
			path.reset()
			path.moveTo(decodeX(1.3157895f), decodeY(1.3333334f))
			path.lineTo(decodeX(1.6842105f), decodeY(1.3333334f))
			path.lineTo(decodeX(1.5f), decodeY(1.6083333f))
			path.lineTo(decodeX(1.3157895f), decodeY(1.3333334f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath3() As Path2D
			path.reset()
			path.moveTo(decodeX(1.3157895f), decodeY(1.3888888f))
			path.lineTo(decodeX(1.6842105f), decodeY(1.3888888f))
			path.lineTo(decodeX(1.4952153f), decodeY(1.655303f))
			path.lineTo(decodeX(1.3157895f), decodeY(1.3888888f))
			path.closePath()
			Return path
		End Function



		Private Function decodeGradient1(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.24868421f * w) + x, (0.0014705883f * h) + y, (0.24868421f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color2, decodeColor(color2,color3,0.5f), color3})
		End Function

		Private Function decodeGradient2(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.25f * w) + x, (0.0f * h) + y, (0.25441176f * w) + x, (1.0016667f * h) + y, New Single() { 0.0f,0.26988637f,0.53977275f,0.5951705f,0.6505682f,0.8252841f,1.0f }, New Color() { color4, decodeColor(color4,color5,0.5f), color5, decodeColor(color5,color3,0.5f), color3, decodeColor(color3,color6,0.5f), color6})
		End Function

		Private Function decodeGradient3(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.50714284f * w) + x, (0.095f * h) + y, (0.49285713f * w) + x, (0.91f * h) + y, New Single() { 0.0f,0.24289773f,0.48579547f,0.74289775f,1.0f }, New Color() { color7, decodeColor(color7,color8,0.5f), color8, decodeColor(color8,color9,0.5f), color9})
		End Function

		Private Function decodeGradient4(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.24868421f * w) + x, (0.0014705883f * h) + y, (0.24868421f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.31107953f,0.62215906f,0.8110795f,1.0f }, New Color() { color12, decodeColor(color12,color13,0.5f), color13, decodeColor(color13,color14,0.5f), color14})
		End Function

		Private Function decodeGradient5(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.24868421f * w) + x, (0.0014705883f * h) + y, (0.24868421f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color16, decodeColor(color16,color17,0.5f), color17})
		End Function

		Private Function decodeGradient6(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.25f * w) + x, (0.0f * h) + y, (0.25441176f * w) + x, (1.0016667f * h) + y, New Single() { 0.0f,0.26988637f,0.53977275f,0.5951705f,0.6505682f,0.8252841f,1.0f }, New Color() { color18, decodeColor(color18,color19,0.5f), color19, decodeColor(color19,color19,0.5f), color19, decodeColor(color19,color20,0.5f), color20})
		End Function

		Private Function decodeGradient7(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.50714284f * w) + x, (0.095f * h) + y, (0.49285713f * w) + x, (0.91f * h) + y, New Single() { 0.0f,0.24289773f,0.48579547f,0.74289775f,1.0f }, New Color() { color21, decodeColor(color21,color22,0.5f), color22, decodeColor(color22,color23,0.5f), color23})
		End Function

		Private Function decodeGradient8(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.24868421f * w) + x, (0.0014705883f * h) + y, (0.24868421f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color24, decodeColor(color24,color25,0.5f), color25})
		End Function

		Private Function decodeGradient9(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.25f * w) + x, (0.0f * h) + y, (0.25441176f * w) + x, (1.0016667f * h) + y, New Single() { 0.0f,0.26988637f,0.53977275f,0.5951705f,0.6505682f,0.8252841f,1.0f }, New Color() { color26, decodeColor(color26,color27,0.5f), color27, decodeColor(color27,color27,0.5f), color27, decodeColor(color27,color28,0.5f), color28})
		End Function

		Private Function decodeGradient10(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.50714284f * w) + x, (0.095f * h) + y, (0.49285713f * w) + x, (0.91f * h) + y, New Single() { 0.0f,0.24289773f,0.48579547f,0.74289775f,1.0f }, New Color() { color29, decodeColor(color29,color30,0.5f), color30, decodeColor(color30,color31,0.5f), color31})
		End Function

		Private Function decodeGradient11(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.24868421f * w) + x, (0.0014705883f * h) + y, (0.24868421f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color32, decodeColor(color32,color33,0.5f), color33})
		End Function

		Private Function decodeGradient12(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.25f * w) + x, (0.0f * h) + y, (0.25441176f * w) + x, (1.0016667f * h) + y, New Single() { 0.0f,0.26988637f,0.53977275f,0.5951705f,0.6505682f,0.8252841f,1.0f }, New Color() { color34, decodeColor(color34,color35,0.5f), color35, decodeColor(color35,color36,0.5f), color36, decodeColor(color36,color15,0.5f), color15})
		End Function

		Private Function decodeGradient13(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.50714284f * w) + x, (0.095f * h) + y, (0.49285713f * w) + x, (0.91f * h) + y, New Single() { 0.0f,0.24289773f,0.48579547f,0.74289775f,1.0f }, New Color() { color37, decodeColor(color37,color38,0.5f), color38, decodeColor(color38,color39,0.5f), color39})
		End Function


	End Class

End Namespace
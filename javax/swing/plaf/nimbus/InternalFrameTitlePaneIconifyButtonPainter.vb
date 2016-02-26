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



	Friend NotInheritable Class InternalFrameTitlePaneIconifyButtonPainter
		Inherits AbstractRegionPainter

		'package private integers representing the available states that
		'this painter will paint. These are used when creating a new instance
		'of InternalFrameTitlePaneIconifyButtonPainter to determine which region/state is being painted
		'by that instance.
		Friend Const BACKGROUND_ENABLED As Integer = 1
		Friend Const BACKGROUND_DISABLED As Integer = 2
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
		'by a particular instance of InternalFrameTitlePaneIconifyButtonPainter would be created. For the moment at least,
		'however, all are created for each instance.
		Private color1 As Color = decodeColor("nimbusBlueGrey", 0.0055555105f, -0.0029994324f, -0.38039216f, -185)
		Private color2 As Color = decodeColor("nimbusOrange", -0.08377897f, 0.02094239f, -0.40392157f, 0)
		Private color3 As Color = decodeColor("nimbusOrange", 0.0f, 0.0f, 0.0f, 0)
		Private color4 As Color = decodeColor("nimbusOrange", -4.4563413E-4f, -0.48364475f, 0.10588235f, 0)
		Private color5 As Color = decodeColor("nimbusOrange", 0.0f, -0.0050992966f, 0.0039215684f, 0)
		Private color6 As Color = decodeColor("nimbusOrange", 0.0f, -0.12125945f, 0.10588235f, 0)
		Private color7 As Color = decodeColor("nimbusOrange", -0.08377897f, 0.02094239f, -0.40392157f, -106)
		Private color8 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.110526316f, 0.25490195f, 0)
		Private color9 As Color = decodeColor("nimbusOrange", 0.5203877f, -0.9376068f, 0.007843137f, 0)
		Private color10 As Color = decodeColor("nimbusOrange", 0.5273321f, -0.8903002f, -0.086274505f, 0)
		Private color11 As Color = decodeColor("nimbusOrange", 0.5273321f, -0.93313926f, 0.019607842f, 0)
		Private color12 As Color = decodeColor("nimbusOrange", 0.53526866f, -0.8995122f, -0.058823526f, 0)
		Private color13 As Color = decodeColor("nimbusOrange", 0.5233639f, -0.8971863f, -0.07843137f, 0)
		Private color14 As Color = decodeColor("nimbusBlueGrey", -0.0808081f, 0.015910469f, -0.40392157f, -216)
		Private color15 As Color = decodeColor("nimbusBlueGrey", -0.003968239f, -0.03760965f, 0.007843137f, 0)
		Private color16 As New Color(255, 200, 0, 255)
		Private color17 As Color = decodeColor("nimbusOrange", -0.08377897f, 0.02094239f, -0.31764707f, 0)
		Private color18 As Color = decodeColor("nimbusOrange", -0.02758849f, 0.02094239f, -0.062745094f, 0)
		Private color19 As Color = decodeColor("nimbusOrange", -4.4563413E-4f, -0.5074419f, 0.1490196f, 0)
		Private color20 As Color = decodeColor("nimbusOrange", 9.745359E-6f, -0.11175901f, 0.07843137f, 0)
		Private color21 As Color = decodeColor("nimbusOrange", 0.0f, -0.09280169f, 0.07843137f, 0)
		Private color22 As Color = decodeColor("nimbusOrange", 0.0f, -0.19002807f, 0.18039215f, 0)
		Private color23 As Color = decodeColor("nimbusOrange", -0.025772434f, 0.02094239f, 0.05098039f, 0)
		Private color24 As Color = decodeColor("nimbusOrange", -0.08377897f, 0.02094239f, -0.4f, 0)
		Private color25 As Color = decodeColor("nimbusOrange", -0.053104125f, 0.02094239f, -0.109803915f, 0)
		Private color26 As Color = decodeColor("nimbusOrange", -0.017887495f, -0.33726656f, 0.039215684f, 0)
		Private color27 As Color = decodeColor("nimbusOrange", -0.018038228f, 0.02094239f, -0.043137252f, 0)
		Private color28 As Color = decodeColor("nimbusOrange", -0.015844189f, 0.02094239f, -0.027450979f, 0)
		Private color29 As Color = decodeColor("nimbusOrange", -0.010274701f, 0.02094239f, 0.015686274f, 0)
		Private color30 As Color = decodeColor("nimbusOrange", -0.08377897f, 0.02094239f, -0.14509803f, -91)
		Private color31 As Color = decodeColor("nimbusOrange", 0.5273321f, -0.87971985f, -0.15686274f, 0)
		Private color32 As Color = decodeColor("nimbusOrange", 0.5273321f, -0.842694f, -0.31764707f, 0)
		Private color33 As Color = decodeColor("nimbusOrange", 0.516221f, -0.9567362f, 0.12941176f, 0)
		Private color34 As Color = decodeColor("nimbusOrange", 0.5222816f, -0.9229352f, 0.019607842f, 0)
		Private color35 As Color = decodeColor("nimbusOrange", 0.5273321f, -0.91751915f, 0.015686274f, 0)
		Private color36 As Color = decodeColor("nimbusOrange", 0.5273321f, -0.9193561f, 0.039215684f, 0)
		Private color37 As Color = decodeColor("nimbusBlueGrey", -0.01111114f, -0.017933726f, -0.32156864f, 0)


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
				Case BACKGROUND_ENABLED
					paintBackgroundEnabled(g)
				Case BACKGROUND_DISABLED
					paintBackgroundDisabled(g)
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

		Private Sub paintBackgroundEnabled(ByVal g As Graphics2D)
			roundRect = decodeRoundRect1()
			g.paint = color1
			g.fill(roundRect)
			roundRect = decodeRoundRect2()
			g.paint = decodeGradient1(roundRect)
			g.fill(roundRect)
			roundRect = decodeRoundRect3()
			g.paint = decodeGradient2(roundRect)
			g.fill(roundRect)
			rect = decodeRect1()
			g.paint = color7
			g.fill(rect)
			rect = decodeRect2()
			g.paint = color8
			g.fill(rect)

		End Sub

		Private Sub paintBackgroundDisabled(ByVal g As Graphics2D)
			roundRect = decodeRoundRect1()
			g.paint = color1
			g.fill(roundRect)
			roundRect = decodeRoundRect2()
			g.paint = decodeGradient3(roundRect)
			g.fill(roundRect)
			roundRect = decodeRoundRect3()
			g.paint = decodeGradient4(roundRect)
			g.fill(roundRect)
			rect = decodeRect1()
			g.paint = color14
			g.fill(rect)
			rect = decodeRect2()
			g.paint = color15
			g.fill(rect)
			rect = decodeRect3()
			g.paint = color16
			g.fill(rect)
			rect = decodeRect3()
			g.paint = color16
			g.fill(rect)
			rect = decodeRect3()
			g.paint = color16
			g.fill(rect)
			rect = decodeRect3()
			g.paint = color16
			g.fill(rect)
			rect = decodeRect3()
			g.paint = color16
			g.fill(rect)
			rect = decodeRect3()
			g.paint = color16
			g.fill(rect)
			rect = decodeRect3()
			g.paint = color16
			g.fill(rect)

		End Sub

		Private Sub paintBackgroundMouseOver(ByVal g As Graphics2D)
			roundRect = decodeRoundRect1()
			g.paint = color1
			g.fill(roundRect)
			roundRect = decodeRoundRect2()
			g.paint = decodeGradient5(roundRect)
			g.fill(roundRect)
			roundRect = decodeRoundRect3()
			g.paint = decodeGradient6(roundRect)
			g.fill(roundRect)
			rect = decodeRect1()
			g.paint = color23
			g.fill(rect)
			rect = decodeRect2()
			g.paint = color8
			g.fill(rect)
			rect = decodeRect3()
			g.paint = color16
			g.fill(rect)
			rect = decodeRect3()
			g.paint = color16
			g.fill(rect)
			rect = decodeRect3()
			g.paint = color16
			g.fill(rect)
			rect = decodeRect3()
			g.paint = color16
			g.fill(rect)
			rect = decodeRect3()
			g.paint = color16
			g.fill(rect)
			rect = decodeRect3()
			g.paint = color16
			g.fill(rect)
			rect = decodeRect3()
			g.paint = color16
			g.fill(rect)

		End Sub

		Private Sub paintBackgroundPressed(ByVal g As Graphics2D)
			roundRect = decodeRoundRect1()
			g.paint = color1
			g.fill(roundRect)
			roundRect = decodeRoundRect2()
			g.paint = decodeGradient7(roundRect)
			g.fill(roundRect)
			roundRect = decodeRoundRect3()
			g.paint = decodeGradient8(roundRect)
			g.fill(roundRect)
			rect = decodeRect4()
			g.paint = color30
			g.fill(rect)
			rect = decodeRect2()
			g.paint = color8
			g.fill(rect)
			rect = decodeRect3()
			g.paint = color16
			g.fill(rect)
			rect = decodeRect3()
			g.paint = color16
			g.fill(rect)
			rect = decodeRect3()
			g.paint = color16
			g.fill(rect)
			rect = decodeRect3()
			g.paint = color16
			g.fill(rect)
			rect = decodeRect3()
			g.paint = color16
			g.fill(rect)
			rect = decodeRect3()
			g.paint = color16
			g.fill(rect)
			rect = decodeRect3()
			g.paint = color16
			g.fill(rect)

		End Sub

		Private Sub paintBackgroundEnabledAndWindowNotFocused(ByVal g As Graphics2D)
			roundRect = decodeRoundRect1()
			g.paint = color1
			g.fill(roundRect)
			roundRect = decodeRoundRect2()
			g.paint = decodeGradient9(roundRect)
			g.fill(roundRect)
			roundRect = decodeRoundRect3()
			g.paint = decodeGradient10(roundRect)
			g.fill(roundRect)
			rect = decodeRect1()
			g.paint = color14
			g.fill(rect)
			rect = decodeRect2()
			g.paint = color37
			g.fill(rect)
			rect = decodeRect3()
			g.paint = color16
			g.fill(rect)
			rect = decodeRect3()
			g.paint = color16
			g.fill(rect)
			rect = decodeRect3()
			g.paint = color16
			g.fill(rect)
			rect = decodeRect3()
			g.paint = color16
			g.fill(rect)
			rect = decodeRect3()
			g.paint = color16
			g.fill(rect)
			rect = decodeRect3()
			g.paint = color16
			g.fill(rect)
			rect = decodeRect3()
			g.paint = color16
			g.fill(rect)

		End Sub

		Private Sub paintBackgroundMouseOverAndWindowNotFocused(ByVal g As Graphics2D)
			roundRect = decodeRoundRect1()
			g.paint = color1
			g.fill(roundRect)
			roundRect = decodeRoundRect2()
			g.paint = decodeGradient5(roundRect)
			g.fill(roundRect)
			roundRect = decodeRoundRect3()
			g.paint = decodeGradient6(roundRect)
			g.fill(roundRect)
			rect = decodeRect1()
			g.paint = color23
			g.fill(rect)
			rect = decodeRect2()
			g.paint = color8
			g.fill(rect)
			rect = decodeRect3()
			g.paint = color16
			g.fill(rect)
			rect = decodeRect3()
			g.paint = color16
			g.fill(rect)
			rect = decodeRect3()
			g.paint = color16
			g.fill(rect)
			rect = decodeRect3()
			g.paint = color16
			g.fill(rect)
			rect = decodeRect3()
			g.paint = color16
			g.fill(rect)
			rect = decodeRect3()
			g.paint = color16
			g.fill(rect)
			rect = decodeRect3()
			g.paint = color16
			g.fill(rect)

		End Sub

		Private Sub paintBackgroundPressedAndWindowNotFocused(ByVal g As Graphics2D)
			roundRect = decodeRoundRect1()
			g.paint = color1
			g.fill(roundRect)
			roundRect = decodeRoundRect2()
			g.paint = decodeGradient7(roundRect)
			g.fill(roundRect)
			roundRect = decodeRoundRect3()
			g.paint = decodeGradient8(roundRect)
			g.fill(roundRect)
			rect = decodeRect4()
			g.paint = color30
			g.fill(rect)
			rect = decodeRect2()
			g.paint = color8
			g.fill(rect)
			rect = decodeRect3()
			g.paint = color16
			g.fill(rect)
			rect = decodeRect3()
			g.paint = color16
			g.fill(rect)
			rect = decodeRect3()
			g.paint = color16
			g.fill(rect)
			rect = decodeRect3()
			g.paint = color16
			g.fill(rect)
			rect = decodeRect3()
			g.paint = color16
			g.fill(rect)
			rect = decodeRect3()
			g.paint = color16
			g.fill(rect)
			rect = decodeRect3()
			g.paint = color16
			g.fill(rect)

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

		Private Function decodeRect1() As Rectangle2D
				rect.rectect(decodeX(1.25f), decodeY(1.6628788f), decodeX(1.75f) - decodeX(1.25f), decodeY(1.7487373f) - decodeY(1.6628788f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodeRect2() As Rectangle2D
				rect.rectect(decodeX(1.2870814f), decodeY(1.6123737f), decodeX(1.7165072f) - decodeX(1.2870814f), decodeY(1.7222222f) - decodeY(1.6123737f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodeRect3() As Rectangle2D
				rect.rectect(decodeX(1.0f), decodeY(1.0f), decodeX(1.0f) - decodeX(1.0f), decodeY(1.0f) - decodeY(1.0f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodeRect4() As Rectangle2D
				rect.rectect(decodeX(1.25f), decodeY(1.6527778f), decodeX(1.7511961f) - decodeX(1.25f), decodeY(1.7828283f) - decodeY(1.6527778f)) 'height - width - y - x
			Return rect
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
			Return decodeGradient((0.25f * w) + x, (0.0f * h) + y, (0.25441176f * w) + x, (1.0016667f * h) + y, New Single() { 0.0f,0.26988637f,0.53977275f,0.5951705f,0.6505682f,0.8252841f,1.0f }, New Color() { color4, decodeColor(color4,color3,0.5f), color3, decodeColor(color3,color5,0.5f), color5, decodeColor(color5,color6,0.5f), color6})
		End Function

		Private Function decodeGradient3(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.24868421f * w) + x, (0.0014705883f * h) + y, (0.24868421f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color9, decodeColor(color9,color10,0.5f), color10})
		End Function

		Private Function decodeGradient4(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.25f * w) + x, (0.0f * h) + y, (0.25441176f * w) + x, (1.0016667f * h) + y, New Single() { 0.0f,0.26988637f,0.53977275f,0.5951705f,0.6505682f,0.8252841f,1.0f }, New Color() { color11, decodeColor(color11,color12,0.5f), color12, decodeColor(color12,color13,0.5f), color13, decodeColor(color13,color10,0.5f), color10})
		End Function

		Private Function decodeGradient5(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.24868421f * w) + x, (0.0014705883f * h) + y, (0.24868421f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color17, decodeColor(color17,color18,0.5f), color18})
		End Function

		Private Function decodeGradient6(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.25f * w) + x, (0.0f * h) + y, (0.25441176f * w) + x, (1.0016667f * h) + y, New Single() { 0.0f,0.26988637f,0.53977275f,0.5951705f,0.6505682f,0.8252841f,1.0f }, New Color() { color19, decodeColor(color19,color20,0.5f), color20, decodeColor(color20,color21,0.5f), color21, decodeColor(color21,color22,0.5f), color22})
		End Function

		Private Function decodeGradient7(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.24868421f * w) + x, (0.0014705883f * h) + y, (0.24868421f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color24, decodeColor(color24,color25,0.5f), color25})
		End Function

		Private Function decodeGradient8(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.25f * w) + x, (0.0f * h) + y, (0.25441176f * w) + x, (1.0016667f * h) + y, New Single() { 0.0f,0.26988637f,0.53977275f,0.5951705f,0.6505682f,0.8252841f,1.0f }, New Color() { color26, decodeColor(color26,color27,0.5f), color27, decodeColor(color27,color28,0.5f), color28, decodeColor(color28,color29,0.5f), color29})
		End Function

		Private Function decodeGradient9(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.24868421f * w) + x, (0.0014705883f * h) + y, (0.24868421f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color31, decodeColor(color31,color32,0.5f), color32})
		End Function

		Private Function decodeGradient10(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.25f * w) + x, (0.0f * h) + y, (0.25441176f * w) + x, (1.0016667f * h) + y, New Single() { 0.0f,0.26988637f,0.53977275f,0.5951705f,0.6505682f,0.78336793f,0.9161677f }, New Color() { color33, decodeColor(color33,color34,0.5f), color34, decodeColor(color34,color35,0.5f), color35, decodeColor(color35,color36,0.5f), color36})
		End Function


	End Class

End Namespace
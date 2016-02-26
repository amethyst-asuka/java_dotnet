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



	Friend NotInheritable Class SplitPaneDividerPainter
		Inherits AbstractRegionPainter

		'package private integers representing the available states that
		'this painter will paint. These are used when creating a new instance
		'of SplitPaneDividerPainter to determine which region/state is being painted
		'by that instance.
		Friend Const BACKGROUND_ENABLED As Integer = 1
		Friend Const BACKGROUND_FOCUSED As Integer = 2
		Friend Const FOREGROUND_ENABLED As Integer = 3
		Friend Const FOREGROUND_ENABLED_VERTICAL As Integer = 4


		Private state As Integer 'refers to one of the static final ints above
		Private ctx As PaintContext

		'the following 4 variables are reused during the painting code of the layers
		Private path As Path2D = New Path2D.Float
		Private rect As Rectangle2D = New Rectangle2D.Float(0, 0, 0, 0)
		Private roundRect As RoundRectangle2D = New RoundRectangle2D.Float(0, 0, 0, 0, 0, 0)
		Private ellipse As Ellipse2D = New Ellipse2D.Float(0, 0, 0, 0)

		'All Colors used for painting are stored here. Ideally, only those colors being used
		'by a particular instance of SplitPaneDividerPainter would be created. For the moment at least,
		'however, all are created for each instance.
		Private color1 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.017358616f, -0.11372548f, 0)
		Private color2 As Color = decodeColor("nimbusBlueGrey", 0.055555582f, -0.102396235f, 0.21960783f, 0)
		Private color3 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.07016757f, 0.12941176f, 0)
		Private color4 As Color = decodeColor("nimbusFocus", 0.0f, 0.0f, 0.0f, 0)
		Private color5 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.110526316f, 0.25490195f, 0)
		Private color6 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.048026316f, 0.007843137f, 0)
		Private color7 As Color = decodeColor("nimbusBlueGrey", 0.0055555105f, -0.06970999f, 0.21568626f, 0)
		Private color8 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.06704806f, 0.06666666f, 0)
		Private color9 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.019617222f, -0.09803921f, 0)
		Private color10 As Color = decodeColor("nimbusBlueGrey", 0.004273474f, -0.03790062f, -0.043137252f, 0)
		Private color11 As Color = decodeColor("nimbusBlueGrey", -0.111111104f, -0.106573746f, 0.24705881f, 0)
		Private color12 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.049301825f, 0.02352941f, 0)
		Private color13 As Color = decodeColor("nimbusBlueGrey", -0.006944418f, -0.07399663f, 0.11372548f, 0)
		Private color14 As Color = decodeColor("nimbusBlueGrey", -0.018518567f, -0.06998578f, 0.12549019f, 0)
		Private color15 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.050526317f, 0.039215684f, 0)


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
				Case BACKGROUND_FOCUSED
					paintBackgroundFocused(g)
				Case FOREGROUND_ENABLED
					paintForegroundEnabled(g)
				Case FOREGROUND_ENABLED_VERTICAL
					paintForegroundEnabledAndVertical(g)

			End Select
		End Sub



		Protected Friend Property NotOverridable Overrides paintContext As PaintContext
			Get
				Return ctx
			End Get
		End Property

		Private Sub paintBackgroundEnabled(ByVal g As Graphics2D)
			rect = decodeRect1()
			g.paint = decodeGradient1(rect)
			g.fill(rect)

		End Sub

		Private Sub paintBackgroundFocused(ByVal g As Graphics2D)
			rect = decodeRect1()
			g.paint = decodeGradient2(rect)
			g.fill(rect)

		End Sub

		Private Sub paintForegroundEnabled(ByVal g As Graphics2D)
			roundRect = decodeRoundRect1()
			g.paint = decodeGradient3(roundRect)
			g.fill(roundRect)
			roundRect = decodeRoundRect2()
			g.paint = decodeGradient4(roundRect)
			g.fill(roundRect)

		End Sub

		Private Sub paintForegroundEnabledAndVertical(ByVal g As Graphics2D)
			roundRect = decodeRoundRect3()
			g.paint = decodeGradient5(roundRect)
			g.fill(roundRect)
			rect = decodeRect2()
			g.paint = decodeGradient6(rect)
			g.fill(rect)

		End Sub



		Private Function decodeRect1() As Rectangle2D
				rect.rectect(decodeX(1.0f), decodeY(0.0f), decodeX(2.0f) - decodeX(1.0f), decodeY(3.0f) - decodeY(0.0f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodeRoundRect1() As RoundRectangle2D
			roundRect.roundRectect(decodeX(1.05f), decodeY(1.3f), decodeX(1.95f) - decodeX(1.05f), decodeY(1.8f) - decodeY(1.3f), 3.6666667f, 3.6666667f) 'rounding - height - width - y - x
			Return roundRect
		End Function

		Private Function decodeRoundRect2() As RoundRectangle2D
			roundRect.roundRectect(decodeX(1.1f), decodeY(1.4f), decodeX(1.9f) - decodeX(1.1f), decodeY(1.7f) - decodeY(1.4f), 4.0f, 4.0f) 'rounding - height - width - y - x
			Return roundRect
		End Function

		Private Function decodeRoundRect3() As RoundRectangle2D
			roundRect.roundRectect(decodeX(1.3f), decodeY(1.1428572f), decodeX(1.7f) - decodeX(1.3f), decodeY(1.8214285f) - decodeY(1.1428572f), 4.0f, 4.0f) 'rounding - height - width - y - x
			Return roundRect
		End Function

		Private Function decodeRect2() As Rectangle2D
				rect.rectect(decodeX(1.4f), decodeY(1.1785715f), decodeX(1.6f) - decodeX(1.4f), decodeY(1.7678571f) - decodeY(1.1785715f)) 'height - width - y - x
			Return rect
		End Function



		Private Function decodeGradient1(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.058064517f,0.08064516f,0.103225805f,0.116129026f,0.12903225f,0.43387097f,0.7387097f,0.77903223f,0.81935483f,0.85806453f,0.8967742f }, New Color() { color1, decodeColor(color1,color2,0.5f), color2, decodeColor(color2,color3,0.5f), color3, decodeColor(color3,color3,0.5f), color3, decodeColor(color3,color2,0.5f), color2, decodeColor(color2,color1,0.5f), color1})
		End Function

		Private Function decodeGradient2(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.058064517f,0.08064516f,0.103225805f,0.1166129f,0.13f,0.43f,0.73f,0.7746774f,0.81935483f,0.85806453f,0.8967742f }, New Color() { color1, decodeColor(color1,color4,0.5f), color4, decodeColor(color4,color3,0.5f), color3, decodeColor(color3,color3,0.5f), color3, decodeColor(color3,color4,0.5f), color4, decodeColor(color4,color1,0.5f), color1})
		End Function

		Private Function decodeGradient3(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.20645161f,0.5f,0.7935484f }, New Color() { color1, decodeColor(color1,color5,0.5f), color5})
		End Function

		Private Function decodeGradient4(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.090322584f,0.2951613f,0.5f,0.5822581f,0.66451615f }, New Color() { color6, decodeColor(color6,color7,0.5f), color7, decodeColor(color7,color8,0.5f), color8})
		End Function

		Private Function decodeGradient5(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.25f * w) + x, (0.0f * h) + y, (0.75f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.42096773f,0.84193546f,0.8951613f,0.9483871f }, New Color() { color9, decodeColor(color9,color10,0.5f), color10, decodeColor(color10,color11,0.5f), color11})
		End Function

		Private Function decodeGradient6(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.08064516f,0.16129032f,0.5129032f,0.86451614f,0.88548386f,0.90645164f }, New Color() { color12, decodeColor(color12,color13,0.5f), color13, decodeColor(color13,color14,0.5f), color14, decodeColor(color14,color15,0.5f), color15})
		End Function


	End Class

End Namespace
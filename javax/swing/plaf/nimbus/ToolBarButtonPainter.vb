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



	Friend NotInheritable Class ToolBarButtonPainter
		Inherits AbstractRegionPainter

		'package private integers representing the available states that
		'this painter will paint. These are used when creating a new instance
		'of ToolBarButtonPainter to determine which region/state is being painted
		'by that instance.
		Friend Const BACKGROUND_ENABLED As Integer = 1
		Friend Const BACKGROUND_FOCUSED As Integer = 2
		Friend Const BACKGROUND_MOUSEOVER As Integer = 3
		Friend Const BACKGROUND_MOUSEOVER_FOCUSED As Integer = 4
		Friend Const BACKGROUND_PRESSED As Integer = 5
		Friend Const BACKGROUND_PRESSED_FOCUSED As Integer = 6


		Private ___state As Integer 'refers to one of the static final ints above
		Private ctx As PaintContext

		'the following 4 variables are reused during the painting code of the layers
		Private path As Path2D = New Path2D.Float
		Private rect As Rectangle2D = New Rectangle2D.Float(0, 0, 0, 0)
		Private roundRect As RoundRectangle2D = New RoundRectangle2D.Float(0, 0, 0, 0, 0, 0)
		Private ellipse As Ellipse2D = New Ellipse2D.Float(0, 0, 0, 0)

		'All Colors used for painting are stored here. Ideally, only those colors being used
		'by a particular instance of ToolBarButtonPainter would be created. For the moment at least,
		'however, all are created for each instance.
		Private color1 As Color = decodeColor("nimbusFocus", 0.0f, 0.0f, 0.0f, 0)
		Private color2 As Color = decodeColor("nimbusBlueGrey", -0.027777791f, -0.06885965f, -0.36862746f, -153)
		Private color3 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.020974077f, -0.21960783f, 0)
		Private color4 As Color = decodeColor("nimbusBlueGrey", 0.0f, 0.11169591f, -0.53333336f, 0)
		Private color5 As Color = decodeColor("nimbusBlueGrey", 0.055555582f, -0.10658931f, 0.25098038f, 0)
		Private color6 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.098526314f, 0.2352941f, 0)
		Private color7 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.07333623f, 0.20392156f, 0)
		Private color8 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.110526316f, 0.25490195f, 0)
		Private color9 As Color = decodeColor("nimbusBlueGrey", -0.00505054f, -0.05960039f, 0.10196078f, 0)
		Private color10 As Color = decodeColor("nimbusBlueGrey", -0.008547008f, -0.04772438f, 0.06666666f, 0)
		Private color11 As Color = decodeColor("nimbusBlueGrey", -0.0027777553f, -0.0018306673f, -0.02352941f, 0)
		Private color12 As Color = decodeColor("nimbusBlueGrey", -0.0027777553f, -0.0212406f, 0.13333333f, 0)
		Private color13 As Color = decodeColor("nimbusBlueGrey", 0.0055555105f, -0.030845039f, 0.23921567f, 0)


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



		Protected Friend Property NotOverridable Overrides paintContext As PaintContext
			Get
				Return ctx
			End Get
		End Property

		Private Sub paintBackgroundFocused(ByVal g As Graphics2D)
			path = decodePath1()
			g.paint = color1
			g.fill(path)

		End Sub

		Private Sub paintBackgroundMouseOver(ByVal g As Graphics2D)
			roundRect = decodeRoundRect1()
			g.paint = color2
			g.fill(roundRect)
			roundRect = decodeRoundRect2()
			g.paint = decodeGradient1(roundRect)
			g.fill(roundRect)
			roundRect = decodeRoundRect3()
			g.paint = decodeGradient2(roundRect)
			g.fill(roundRect)

		End Sub

		Private Sub paintBackgroundMouseOverAndFocused(ByVal g As Graphics2D)
			roundRect = decodeRoundRect4()
			g.paint = color1
			g.fill(roundRect)
			roundRect = decodeRoundRect2()
			g.paint = decodeGradient1(roundRect)
			g.fill(roundRect)
			roundRect = decodeRoundRect3()
			g.paint = decodeGradient2(roundRect)
			g.fill(roundRect)

		End Sub

		Private Sub paintBackgroundPressed(ByVal g As Graphics2D)
			roundRect = decodeRoundRect1()
			g.paint = color2
			g.fill(roundRect)
			roundRect = decodeRoundRect2()
			g.paint = decodeGradient1(roundRect)
			g.fill(roundRect)
			roundRect = decodeRoundRect3()
			g.paint = decodeGradient3(roundRect)
			g.fill(roundRect)

		End Sub

		Private Sub paintBackgroundPressedAndFocused(ByVal g As Graphics2D)
			roundRect = decodeRoundRect4()
			g.paint = color1
			g.fill(roundRect)
			roundRect = decodeRoundRect2()
			g.paint = decodeGradient1(roundRect)
			g.fill(roundRect)
			roundRect = decodeRoundRect3()
			g.paint = decodeGradient3(roundRect)
			g.fill(roundRect)

		End Sub



		Private Function decodePath1() As Path2D
			path.reset()
			path.moveTo(decodeX(1.4133738f), decodeY(0.120000005f))
			path.lineTo(decodeX(1.9893618f), decodeY(0.120000005f))
			path.curveTo(decodeAnchorX(1.9893617630004883f, 3.0f), decodeAnchorY(0.12000000476837158f, 0.0f), decodeAnchorX(2.8857147693634033f, 0.0f), decodeAnchorY(1.04347825050354f, -3.0f), decodeX(2.8857148f), decodeY(1.0434783f))
			path.lineTo(decodeX(2.9f), decodeY(1.9565217f))
			path.curveTo(decodeAnchorX(2.9000000953674316f, 0.0f), decodeAnchorY(1.95652174949646f, 3.0f), decodeAnchorX(1.9893617630004883f, 3.0f), decodeAnchorY(2.8714287281036377f, 0.0f), decodeX(1.9893618f), decodeY(2.8714287f))
			path.lineTo(decodeX(1.0106384f), decodeY(2.8714287f))
			path.curveTo(decodeAnchorX(1.0106383562088013f, -3.0f), decodeAnchorY(2.8714287281036377f, 0.0f), decodeAnchorX(0.12000000476837158f, 0.0f), decodeAnchorY(1.95652174949646f, 3.0f), decodeX(0.120000005f), decodeY(1.9565217f))
			path.lineTo(decodeX(0.120000005f), decodeY(1.0465839f))
			path.curveTo(decodeAnchorX(0.12000000476837158f, 0.0f), decodeAnchorY(1.046583890914917f, -3.000000000000001f), decodeAnchorX(1.0106383562088013f, -3.0f), decodeAnchorY(0.12000000476837158f, 0.0f), decodeX(1.0106384f), decodeY(0.120000005f))
			path.lineTo(decodeX(1.4148936f), decodeY(0.120000005f))
			path.lineTo(decodeX(1.4148936f), decodeY(0.4857143f))
			path.lineTo(decodeX(1.0106384f), decodeY(0.4857143f))
			path.curveTo(decodeAnchorX(1.0106383562088013f, -1.928571428571427f), decodeAnchorY(0.48571428656578064f, 0.0f), decodeAnchorX(0.4714285731315613f, -0.04427948362011014f), decodeAnchorY(1.040372610092163f, -2.429218094741624f), decodeX(0.47142857f), decodeY(1.0403726f))
			path.lineTo(decodeX(0.47142857f), decodeY(1.9565217f))
			path.curveTo(decodeAnchorX(0.4714285731315613f, 0.0f), decodeAnchorY(1.95652174949646f, 2.2142857142856975f), decodeAnchorX(1.0106383562088013f, -1.7857142857142847f), decodeAnchorY(2.5142855644226074f, 0.0f), decodeX(1.0106384f), decodeY(2.5142856f))
			path.lineTo(decodeX(1.9893618f), decodeY(2.5142856f))
			path.curveTo(decodeAnchorX(1.9893617630004883f, 2.071428571428598f), decodeAnchorY(2.5142855644226074f, 0.0f), decodeAnchorX(2.5f, 0.0f), decodeAnchorY(1.95652174949646f, 2.2142857142857046f), decodeX(2.5f), decodeY(1.9565217f))
			path.lineTo(decodeX(2.5142853f), decodeY(1.0434783f))
			path.curveTo(decodeAnchorX(2.5142853260040283f, 0.0f), decodeAnchorY(1.04347825050354f, -2.1428571428571406f), decodeAnchorX(1.990121603012085f, 2.142857142857167f), decodeAnchorY(0.4714285731315613f, 0.0f), decodeX(1.9901216f), decodeY(0.47142857f))
			path.lineTo(decodeX(1.4148936f), decodeY(0.4857143f))
			path.lineTo(decodeX(1.4133738f), decodeY(0.120000005f))
			path.closePath()
			Return path
		End Function

		Private Function decodeRoundRect1() As RoundRectangle2D
			roundRect.roundRectect(decodeX(0.4f), decodeY(0.6f), decodeX(2.6f) - decodeX(0.4f), decodeY(2.8f) - decodeY(0.6f), 12.0f, 12.0f) 'rounding - height - width - y - x
			Return roundRect
		End Function

		Private Function decodeRoundRect2() As RoundRectangle2D
			roundRect.roundRectect(decodeX(0.4f), decodeY(0.4f), decodeX(2.6f) - decodeX(0.4f), decodeY(2.6f) - decodeY(0.4f), 12.0f, 12.0f) 'rounding - height - width - y - x
			Return roundRect
		End Function

		Private Function decodeRoundRect3() As RoundRectangle2D
			roundRect.roundRectect(decodeX(0.6f), decodeY(0.6f), decodeX(2.4f) - decodeX(0.6f), decodeY(2.4f) - decodeY(0.6f), 9.0f, 9.0f) 'rounding - height - width - y - x
			Return roundRect
		End Function

		Private Function decodeRoundRect4() As RoundRectangle2D
			roundRect.roundRectect(decodeX(0.120000005f), decodeY(0.120000005f), decodeX(2.8800004f) - decodeX(0.120000005f), decodeY(2.8800004f) - decodeY(0.120000005f), 13.0f, 13.0f) 'rounding - height - width - y - x
			Return roundRect
		End Function



		Private Function decodeGradient1(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.09f,0.52f,0.95f }, New Color() { color3, decodeColor(color3,color4,0.5f), color4})
		End Function

		Private Function decodeGradient2(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.03f,0.06f,0.33f,0.6f,0.65f,0.7f,0.825f,0.95f,0.975f,1.0f }, New Color() { color5, decodeColor(color5,color6,0.5f), color6, decodeColor(color6,color7,0.5f), color7, decodeColor(color7,color7,0.5f), color7, decodeColor(color7,color8,0.5f), color8, decodeColor(color8,color8,0.5f), color8})
		End Function

		Private Function decodeGradient3(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.03f,0.06f,0.33f,0.6f,0.65f,0.7f,0.825f,0.95f,0.975f,1.0f }, New Color() { color9, decodeColor(color9,color10,0.5f), color10, decodeColor(color10,color11,0.5f), color11, decodeColor(color11,color11,0.5f), color11, decodeColor(color11,color12,0.5f), color12, decodeColor(color12,color13,0.5f), color13})
		End Function


	End Class

End Namespace
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



	Friend NotInheritable Class ScrollBarThumbPainter
		Inherits AbstractRegionPainter

		'package private integers representing the available states that
		'this painter will paint. These are used when creating a new instance
		'of ScrollBarThumbPainter to determine which region/state is being painted
		'by that instance.
		Friend Const BACKGROUND_DISABLED As Integer = 1
		Friend Const BACKGROUND_ENABLED As Integer = 2
		Friend Const BACKGROUND_FOCUSED As Integer = 3
		Friend Const BACKGROUND_MOUSEOVER As Integer = 4
		Friend Const BACKGROUND_PRESSED As Integer = 5


		Private state As Integer 'refers to one of the static final ints above
		Private ctx As PaintContext

		'the following 4 variables are reused during the painting code of the layers
		Private path As Path2D = New Path2D.Float
		Private rect As Rectangle2D = New Rectangle2D.Float(0, 0, 0, 0)
		Private roundRect As RoundRectangle2D = New RoundRectangle2D.Float(0, 0, 0, 0, 0, 0)
		Private ellipse As Ellipse2D = New Ellipse2D.Float(0, 0, 0, 0)

		'All Colors used for painting are stored here. Ideally, only those colors being used
		'by a particular instance of ScrollBarThumbPainter would be created. For the moment at least,
		'however, all are created for each instance.
		Private color1 As Color = decodeColor("nimbusBase", 5.1498413E-4f, 0.18061227f, -0.35686278f, 0)
		Private color2 As Color = decodeColor("nimbusBase", 5.1498413E-4f, -0.21018237f, -0.18039218f, 0)
		Private color3 As Color = decodeColor("nimbusBase", 7.13408E-4f, -0.53277314f, 0.25098038f, 0)
		Private color4 As Color = decodeColor("nimbusBase", -0.07865167f, -0.6317617f, 0.44313723f, 0)
		Private color5 As Color = decodeColor("nimbusBase", 5.1498413E-4f, -0.44340658f, 0.26666665f, 0)
		Private color6 As Color = decodeColor("nimbusBase", 5.1498413E-4f, -0.4669379f, 0.38039213f, 0)
		Private color7 As Color = decodeColor("nimbusBase", -0.07865167f, -0.56512606f, 0.45098037f, 0)
		Private color8 As Color = decodeColor("nimbusBase", -0.0017285943f, -0.362987f, 0.011764705f, 0)
		Private color9 As Color = decodeColor("nimbusBase", 5.2034855E-5f, -0.41753247f, 0.09803921f, -222)
		Private color10 As New Color(255, 200, 0, 255)
		Private color11 As Color = decodeColor("nimbusBase", -0.0017285943f, -0.362987f, 0.011764705f, -255)
		Private color12 As Color = decodeColor("nimbusBase", 0.010237217f, -0.5621849f, 0.25098038f, 0)
		Private color13 As Color = decodeColor("nimbusBase", 0.08801502f, -0.6317773f, 0.4470588f, 0)
		Private color14 As Color = decodeColor("nimbusBase", 5.1498413E-4f, -0.45950285f, 0.34117645f, 0)
		Private color15 As Color = decodeColor("nimbusBase", -0.0017285943f, -0.48277313f, 0.45098037f, 0)
		Private color16 As Color = decodeColor("nimbusBase", 0.0f, -0.6357143f, 0.45098037f, 0)
		Private color17 As Color = decodeColor("nimbusBase", -0.57865167f, -0.6357143f, -0.54901963f, 0)
		Private color18 As Color = decodeColor("nimbusBase", 0.0013483167f, 0.29021162f, -0.33725494f, 0)
		Private color19 As Color = decodeColor("nimbusBase", 0.002908647f, -0.29012606f, -0.015686274f, 0)
		Private color20 As Color = decodeColor("nimbusBase", -8.738637E-4f, -0.40612245f, 0.21960783f, 0)
		Private color21 As Color = decodeColor("nimbusBase", 0.0f, -0.01765871f, 0.015686274f, 0)
		Private color22 As Color = decodeColor("nimbusBase", 0.0f, -0.12714285f, 0.1372549f, 0)
		Private color23 As Color = decodeColor("nimbusBase", 0.0018727183f, -0.23116884f, 0.31372547f, 0)
		Private color24 As Color = decodeColor("nimbusBase", -8.738637E-4f, -0.3579365f, -0.33725494f, 0)
		Private color25 As Color = decodeColor("nimbusBase", 0.004681647f, -0.3857143f, -0.36078435f, 0)


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
				Case BACKGROUND_MOUSEOVER
					paintBackgroundMouseOver(g)
				Case BACKGROUND_PRESSED
					paintBackgroundPressed(g)

			End Select
		End Sub



		Protected Friend Property NotOverridable Overrides paintContext As PaintContext
			Get
				Return ctx
			End Get
		End Property

		Private Sub paintBackgroundEnabled(ByVal g As Graphics2D)
			path = decodePath1()
			g.paint = decodeGradient1(path)
			g.fill(path)
			path = decodePath2()
			g.paint = decodeGradient2(path)
			g.fill(path)
			path = decodePath3()
			g.paint = decodeGradient3(path)
			g.fill(path)
			path = decodePath4()
			g.paint = color10
			g.fill(path)
			path = decodePath5()
			g.paint = decodeGradient4(path)
			g.fill(path)

		End Sub

		Private Sub paintBackgroundMouseOver(ByVal g As Graphics2D)
			path = decodePath1()
			g.paint = decodeGradient1(path)
			g.fill(path)
			path = decodePath2()
			g.paint = decodeGradient5(path)
			g.fill(path)
			path = decodePath3()
			g.paint = decodeGradient3(path)
			g.fill(path)
			path = decodePath4()
			g.paint = color10
			g.fill(path)
			path = decodePath5()
			g.paint = decodeGradient4(path)
			g.fill(path)

		End Sub

		Private Sub paintBackgroundPressed(ByVal g As Graphics2D)
			path = decodePath1()
			g.paint = decodeGradient6(path)
			g.fill(path)
			path = decodePath2()
			g.paint = decodeGradient7(path)
			g.fill(path)
			path = decodePath3()
			g.paint = decodeGradient8(path)
			g.fill(path)
			path = decodePath4()
			g.paint = color10
			g.fill(path)
			path = decodePath6()
			g.paint = decodeGradient9(path)
			g.fill(path)

		End Sub



		Private Function decodePath1() As Path2D
			path.reset()
			path.moveTo(decodeX(0.0f), decodeY(1.0f))
			path.lineTo(decodeX(0.0f), decodeY(1.0666667f))
			path.curveTo(decodeAnchorX(0.0f, 0.0f), decodeAnchorY(1.0666667222976685f, 6.0f), decodeAnchorX(1.0f, -10.0f), decodeAnchorY(2.0f, 0.0f), decodeX(1.0f), decodeY(2.0f))
			path.lineTo(decodeX(2.0f), decodeY(2.0f))
			path.curveTo(decodeAnchorX(2.0f, 10.0f), decodeAnchorY(2.0f, 0.0f), decodeAnchorX(3.0f, 0.0f), decodeAnchorY(1.0666667222976685f, 6.0f), decodeX(3.0f), decodeY(1.0666667f))
			path.lineTo(decodeX(3.0f), decodeY(1.0f))
			path.lineTo(decodeX(0.0f), decodeY(1.0f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath2() As Path2D
			path.reset()
			path.moveTo(decodeX(0.06666667f), decodeY(1.0f))
			path.lineTo(decodeX(0.06666667f), decodeY(1.0666667f))
			path.curveTo(decodeAnchorX(0.06666667014360428f, -0.045454545454545414f), decodeAnchorY(1.0666667222976685f, 8.45454545454545f), decodeAnchorX(1.0f, -5.863636363636354f), decodeAnchorY(1.933333396911621f, 0.0f), decodeX(1.0f), decodeY(1.9333334f))
			path.lineTo(decodeX(2.0f), decodeY(1.9333334f))
			path.curveTo(decodeAnchorX(2.0f, 5.909090909090935f), decodeAnchorY(1.933333396911621f, -3.552713678800501E-15f), decodeAnchorX(2.933333396911621f, -0.045454545454546746f), decodeAnchorY(1.0666667222976685f, 8.36363636363636f), decodeX(2.9333334f), decodeY(1.0666667f))
			path.lineTo(decodeX(2.9333334f), decodeY(1.0f))
			path.lineTo(decodeX(0.06666667f), decodeY(1.0f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath3() As Path2D
			path.reset()
			path.moveTo(decodeX(0.4f), decodeY(1.0f))
			path.lineTo(decodeX(0.06666667f), decodeY(1.0f))
			path.lineTo(decodeX(0.16060607f), decodeY(1.5090909f))
			path.curveTo(decodeAnchorX(0.16060607135295868f, 0.0f), decodeAnchorY(1.5090909004211426f, 0.0f), decodeAnchorX(0.20000000298023224f, -0.9545454545454564f), decodeAnchorY(1.1363636255264282f, 1.5454545454545472f), decodeX(0.2f), decodeY(1.1363636f))
			path.curveTo(decodeAnchorX(0.20000000298023224f, 0.9545454545454564f), decodeAnchorY(1.1363636255264282f, -1.5454545454545472f), decodeAnchorX(0.4000000059604645f, 0.0f), decodeAnchorY(1.0f, 0.0f), decodeX(0.4f), decodeY(1.0f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath4() As Path2D
			path.reset()
			path.moveTo(decodeX(2.4242425f), decodeY(1.5121212f))
			path.lineTo(decodeX(2.4242425f), decodeY(1.5121212f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath5() As Path2D
			path.reset()
			path.moveTo(decodeX(2.9363637f), decodeY(1.0f))
			path.lineTo(decodeX(2.6030304f), decodeY(1.0f))
			path.curveTo(decodeAnchorX(2.6030304431915283f, 0.0f), decodeAnchorY(1.0f, 0.0f), decodeAnchorX(2.7787880897521973f, -0.6818181818181728f), decodeAnchorY(1.1333333253860474f, -1.227272727272727f), decodeX(2.778788f), decodeY(1.1333333f))
			path.curveTo(decodeAnchorX(2.7787880897521973f, 0.6818181818181728f), decodeAnchorY(1.1333333253860474f, 1.227272727272727f), decodeAnchorX(2.8393938541412354f, 0.0f), decodeAnchorY(1.5060606002807617f, 0.0f), decodeX(2.8393939f), decodeY(1.5060606f))
			path.lineTo(decodeX(2.9363637f), decodeY(1.0f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath6() As Path2D
			path.reset()
			path.moveTo(decodeX(2.9363637f), decodeY(1.0f))
			path.lineTo(decodeX(2.5563636f), decodeY(1.0f))
			path.curveTo(decodeAnchorX(2.556363582611084f, 0.0f), decodeAnchorY(1.0f, 0.0f), decodeAnchorX(2.7587878704071045f, -0.6818181818181728f), decodeAnchorY(1.1399999856948853f, -1.2272727272727266f), decodeX(2.7587879f), decodeY(1.14f))
			path.curveTo(decodeAnchorX(2.7587878704071045f, 0.6818181818181728f), decodeAnchorY(1.1399999856948853f, 1.227272727272727f), decodeAnchorX(2.8393938541412354f, 0.0f), decodeAnchorY(1.5060606002807617f, 0.0f), decodeX(2.8393939f), decodeY(1.5060606f))
			path.lineTo(decodeX(2.9363637f), decodeY(1.0f))
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
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.038922157f,0.0508982f,0.06287425f,0.19610777f,0.32934132f,0.48952097f,0.6497006f,0.8248503f,1.0f }, New Color() { color3, decodeColor(color3,color4,0.5f), color4, decodeColor(color4,color5,0.5f), color5, decodeColor(color5,color6,0.5f), color6, decodeColor(color6,color7,0.5f), color7})
		End Function

		Private Function decodeGradient3(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.06818182f * w) + x, (-0.005952381f * h) + y, (0.3689091f * w) + x, (0.23929171f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color8, decodeColor(color8,color9,0.5f), color9})
		End Function

		Private Function decodeGradient4(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.9409091f * w) + x, (0.035928145f * h) + y, (0.5954546f * w) + x, (0.26347303f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color8, decodeColor(color8,color11,0.5f), color11})
		End Function

		Private Function decodeGradient5(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.038922157f,0.0508982f,0.06287425f,0.19610777f,0.32934132f,0.48952097f,0.6497006f,0.8248503f,1.0f }, New Color() { color12, decodeColor(color12,color13,0.5f), color13, decodeColor(color13,color14,0.5f), color14, decodeColor(color14,color15,0.5f), color15, decodeColor(color15,color16,0.5f), color16})
		End Function

		Private Function decodeGradient6(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color17, decodeColor(color17,color18,0.5f), color18})
		End Function

		Private Function decodeGradient7(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.038922157f,0.0508982f,0.06287425f,0.19610777f,0.32934132f,0.48952097f,0.6497006f,0.8248503f,1.0f }, New Color() { color19, decodeColor(color19,color20,0.5f), color20, decodeColor(color20,color21,0.5f), color21, decodeColor(color21,color22,0.5f), color22, decodeColor(color22,color23,0.5f), color23})
		End Function

		Private Function decodeGradient8(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.06818182f * w) + x, (-0.005952381f * h) + y, (0.3689091f * w) + x, (0.23929171f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color24, decodeColor(color24,color9,0.5f), color9})
		End Function

		Private Function decodeGradient9(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.9409091f * w) + x, (0.035928145f * h) + y, (0.37615633f * w) + x, (0.34910178f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color25, decodeColor(color25,color11,0.5f), color11})
		End Function


	End Class

End Namespace
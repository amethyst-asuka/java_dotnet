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



	Friend NotInheritable Class InternalFramePainter
		Inherits AbstractRegionPainter

		'package private integers representing the available states that
		'this painter will paint. These are used when creating a new instance
		'of InternalFramePainter to determine which region/state is being painted
		'by that instance.
		Friend Const BACKGROUND_ENABLED As Integer = 1
		Friend Const BACKGROUND_ENABLED_WINDOWFOCUSED As Integer = 2


		Private state As Integer 'refers to one of the static final ints above
		Private ctx As PaintContext

		'the following 4 variables are reused during the painting code of the layers
		Private path As Path2D = New Path2D.Float
		Private rect As Rectangle2D = New Rectangle2D.Float(0, 0, 0, 0)
		Private roundRect As RoundRectangle2D = New RoundRectangle2D.Float(0, 0, 0, 0, 0, 0)
		Private ellipse As Ellipse2D = New Ellipse2D.Float(0, 0, 0, 0)

		'All Colors used for painting are stored here. Ideally, only those colors being used
		'by a particular instance of InternalFramePainter would be created. For the moment at least,
		'however, all are created for each instance.
		Private color1 As Color = decodeColor("nimbusBase", 0.032459438f, -0.53637654f, 0.043137252f, 0)
		Private color2 As Color = decodeColor("nimbusBlueGrey", 0.004273474f, -0.039488062f, -0.027450979f, 0)
		Private color3 As Color = decodeColor("nimbusBlueGrey", -0.00505054f, -0.056339122f, 0.05098039f, 0)
		Private color4 As Color = decodeColor("nimbusBlueGrey", -0.01111114f, -0.06357796f, 0.09019607f, 0)
		Private color5 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.023821115f, -0.06666666f, 0)
		Private color6 As Color = decodeColor("control", 0.0f, 0.0f, 0.0f, 0)
		Private color7 As Color = decodeColor("nimbusBlueGrey", -0.006944418f, -0.07399663f, 0.11372548f, 0)
		Private color8 As Color = decodeColor("nimbusBase", 0.02551502f, -0.47885156f, -0.34901965f, 0)
		Private color9 As New Color(255, 200, 0, 255)
		Private color10 As Color = decodeColor("nimbusBase", 0.004681647f, -0.6274498f, 0.39999998f, 0)
		Private color11 As Color = decodeColor("nimbusBase", 0.032459438f, -0.5934608f, 0.2862745f, 0)
		Private color12 As New Color(204, 207, 213, 255)
		Private color13 As Color = decodeColor("nimbusBase", 0.032459438f, -0.55506915f, 0.18039215f, 0)
		Private color14 As Color = decodeColor("nimbusBase", 0.004681647f, -0.52792984f, 0.10588235f, 0)
		Private color15 As Color = decodeColor("nimbusBase", 0.03801495f, -0.4794643f, -0.04705882f, 0)
		Private color16 As Color = decodeColor("nimbusBase", 0.021348298f, -0.61416256f, 0.3607843f, 0)
		Private color17 As Color = decodeColor("nimbusBase", 0.032459438f, -0.5546332f, 0.17647058f, 0)
		Private color18 As New Color(235, 236, 238, 255)


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
				Case BACKGROUND_ENABLED_WINDOWFOCUSED
					paintBackgroundEnabledAndWindowFocused(g)

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
			path = decodePath1()
			g.paint = decodeGradient1(path)
			g.fill(path)
			path = decodePath2()
			g.paint = color3
			g.fill(path)
			path = decodePath3()
			g.paint = color4
			g.fill(path)
			path = decodePath4()
			g.paint = color5
			g.fill(path)
			rect = decodeRect1()
			g.paint = color6
			g.fill(rect)
			rect = decodeRect2()
			g.paint = color7
			g.fill(rect)

		End Sub

		Private Sub paintBackgroundEnabledAndWindowFocused(ByVal g As Graphics2D)
			roundRect = decodeRoundRect2()
			g.paint = color8
			g.fill(roundRect)
			path = decodePath5()
			g.paint = color9
			g.fill(path)
			path = decodePath1()
			g.paint = decodeGradient2(path)
			g.fill(path)
			path = decodePath6()
			g.paint = color12
			g.fill(path)
			path = decodePath7()
			g.paint = color13
			g.fill(path)
			path = decodePath8()
			g.paint = color14
			g.fill(path)
			path = decodePath9()
			g.paint = color15
			g.fill(path)
			rect = decodeRect1()
			g.paint = color6
			g.fill(rect)
			rect = decodeRect3()
			g.paint = color9
			g.fill(rect)
			rect = decodeRect3()
			g.paint = color9
			g.fill(rect)
			rect = decodeRect3()
			g.paint = color9
			g.fill(rect)
			rect = decodeRect4()
			g.paint = decodeGradient3(rect)
			g.fill(rect)
			rect = decodeRect2()
			g.paint = color18
			g.fill(rect)

		End Sub



		Private Function decodeRoundRect1() As RoundRectangle2D
			roundRect.roundRectect(decodeX(0.0f), decodeY(0.0f), decodeX(3.0f) - decodeX(0.0f), decodeY(3.0f) - decodeY(0.0f), 4.6666665f, 4.6666665f) 'rounding - height - width - y - x
			Return roundRect
		End Function

		Private Function decodePath1() As Path2D
			path.reset()
			path.moveTo(decodeX(0.16666667f), decodeY(0.12f))
			path.curveTo(decodeAnchorX(0.1666666716337204f, 0.0f), decodeAnchorY(0.11999999731779099f, -1.0f), decodeAnchorX(0.5f, -1.0f), decodeAnchorY(0.03999999910593033f, 0.0f), decodeX(0.5f), decodeY(0.04f))
			path.curveTo(decodeAnchorX(0.5f, 1.0f), decodeAnchorY(0.03999999910593033f, 0.0f), decodeAnchorX(2.5f, -1.0f), decodeAnchorY(0.03999999910593033f, 0.0f), decodeX(2.5f), decodeY(0.04f))
			path.curveTo(decodeAnchorX(2.5f, 1.0f), decodeAnchorY(0.03999999910593033f, 0.0f), decodeAnchorX(2.8333332538604736f, 0.0f), decodeAnchorY(0.11999999731779099f, -1.0f), decodeX(2.8333333f), decodeY(0.12f))
			path.curveTo(decodeAnchorX(2.8333332538604736f, 0.0f), decodeAnchorY(0.11999999731779099f, 1.0f), decodeAnchorX(2.8333332538604736f, 0.0f), decodeAnchorY(0.9599999785423279f, 0.0f), decodeX(2.8333333f), decodeY(0.96f))
			path.lineTo(decodeX(0.16666667f), decodeY(0.96f))
			path.curveTo(decodeAnchorX(0.1666666716337204f, 0.0f), decodeAnchorY(0.9599999785423279f, 0.0f), decodeAnchorX(0.1666666716337204f, 0.0f), decodeAnchorY(0.11999999731779099f, 1.0f), decodeX(0.16666667f), decodeY(0.12f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath2() As Path2D
			path.reset()
			path.moveTo(decodeX(0.6666667f), decodeY(0.96f))
			path.lineTo(decodeX(0.16666667f), decodeY(0.96f))
			path.curveTo(decodeAnchorX(0.1666666716337204f, 0.0f), decodeAnchorY(0.9599999785423279f, 0.0f), decodeAnchorX(0.1666666716337204f, 0.0f), decodeAnchorY(2.5f, -1.0f), decodeX(0.16666667f), decodeY(2.5f))
			path.curveTo(decodeAnchorX(0.1666666716337204f, 0.0f), decodeAnchorY(2.5f, 1.0f), decodeAnchorX(0.5f, -1.0f), decodeAnchorY(2.8333332538604736f, 0.0f), decodeX(0.5f), decodeY(2.8333333f))
			path.curveTo(decodeAnchorX(0.5f, 1.0f), decodeAnchorY(2.8333332538604736f, 0.0f), decodeAnchorX(2.5f, -1.0f), decodeAnchorY(2.8333332538604736f, 0.0f), decodeX(2.5f), decodeY(2.8333333f))
			path.curveTo(decodeAnchorX(2.5f, 1.0f), decodeAnchorY(2.8333332538604736f, 0.0f), decodeAnchorX(2.8333332538604736f, 0.0f), decodeAnchorY(2.5f, 1.0f), decodeX(2.8333333f), decodeY(2.5f))
			path.curveTo(decodeAnchorX(2.8333332538604736f, 0.0f), decodeAnchorY(2.5f, -1.0f), decodeAnchorX(2.8333332538604736f, 0.0f), decodeAnchorY(0.9599999785423279f, 0.0f), decodeX(2.8333333f), decodeY(0.96f))
			path.lineTo(decodeX(2.3333333f), decodeY(0.96f))
			path.lineTo(decodeX(2.3333333f), decodeY(2.3333333f))
			path.lineTo(decodeX(0.6666667f), decodeY(2.3333333f))
			path.lineTo(decodeX(0.6666667f), decodeY(0.96f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath3() As Path2D
			path.reset()
			path.moveTo(decodeX(0.8333333f), decodeY(0.96f))
			path.lineTo(decodeX(0.6666667f), decodeY(0.96f))
			path.lineTo(decodeX(0.6666667f), decodeY(2.3333333f))
			path.lineTo(decodeX(2.3333333f), decodeY(2.3333333f))
			path.lineTo(decodeX(2.3333333f), decodeY(0.96f))
			path.lineTo(decodeX(2.1666667f), decodeY(0.96f))
			path.lineTo(decodeX(2.1666667f), decodeY(2.1666667f))
			path.lineTo(decodeX(0.8333333f), decodeY(2.1666667f))
			path.lineTo(decodeX(0.8333333f), decodeY(0.96f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath4() As Path2D
			path.reset()
			path.moveTo(decodeX(2.1666667f), decodeY(1.0f))
			path.lineTo(decodeX(1.0f), decodeY(1.0f))
			path.lineTo(decodeX(1.0f), decodeY(2.0f))
			path.lineTo(decodeX(2.0f), decodeY(2.0f))
			path.lineTo(decodeX(2.0f), decodeY(1.0f))
			path.lineTo(decodeX(2.1666667f), decodeY(1.0f))
			path.lineTo(decodeX(2.1666667f), decodeY(2.1666667f))
			path.lineTo(decodeX(0.8333333f), decodeY(2.1666667f))
			path.lineTo(decodeX(0.8333333f), decodeY(0.96f))
			path.lineTo(decodeX(2.1666667f), decodeY(0.96f))
			path.lineTo(decodeX(2.1666667f), decodeY(1.0f))
			path.closePath()
			Return path
		End Function

		Private Function decodeRect1() As Rectangle2D
				rect.rectect(decodeX(1.0f), decodeY(1.0f), decodeX(2.0f) - decodeX(1.0f), decodeY(2.0f) - decodeY(1.0f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodeRect2() As Rectangle2D
				rect.rectect(decodeX(0.33333334f), decodeY(2.6666667f), decodeX(2.6666667f) - decodeX(0.33333334f), decodeY(2.8333333f) - decodeY(2.6666667f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodeRoundRect2() As RoundRectangle2D
			roundRect.roundRectect(decodeX(0.0f), decodeY(0.0f), decodeX(3.0f) - decodeX(0.0f), decodeY(3.0f) - decodeY(0.0f), 4.8333335f, 4.8333335f) 'rounding - height - width - y - x
			Return roundRect
		End Function

		Private Function decodePath5() As Path2D
			path.reset()
			path.moveTo(decodeX(0.16666667f), decodeY(0.08f))
			path.curveTo(decodeAnchorX(0.1666666716337204f, 0.0f), decodeAnchorY(0.07999999821186066f, 1.0f), decodeAnchorX(0.1666666716337204f, 0.0f), decodeAnchorY(0.07999999821186066f, -1.0f), decodeX(0.16666667f), decodeY(0.08f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath6() As Path2D
			path.reset()
			path.moveTo(decodeX(0.5f), decodeY(0.96f))
			path.lineTo(decodeX(0.16666667f), decodeY(0.96f))
			path.curveTo(decodeAnchorX(0.1666666716337204f, 0.0f), decodeAnchorY(0.9599999785423279f, 0.0f), decodeAnchorX(0.1666666716337204f, 0.0f), decodeAnchorY(2.5f, -1.0f), decodeX(0.16666667f), decodeY(2.5f))
			path.curveTo(decodeAnchorX(0.1666666716337204f, 0.0f), decodeAnchorY(2.5f, 1.0f), decodeAnchorX(0.5f, -1.0f), decodeAnchorY(2.8333332538604736f, 0.0f), decodeX(0.5f), decodeY(2.8333333f))
			path.curveTo(decodeAnchorX(0.5f, 1.0f), decodeAnchorY(2.8333332538604736f, 0.0f), decodeAnchorX(2.5f, -1.0f), decodeAnchorY(2.8333332538604736f, 0.0f), decodeX(2.5f), decodeY(2.8333333f))
			path.curveTo(decodeAnchorX(2.5f, 1.0f), decodeAnchorY(2.8333332538604736f, 0.0f), decodeAnchorX(2.8333332538604736f, 0.0f), decodeAnchorY(2.5f, 1.0f), decodeX(2.8333333f), decodeY(2.5f))
			path.curveTo(decodeAnchorX(2.8333332538604736f, 0.0f), decodeAnchorY(2.5f, -1.0f), decodeAnchorX(2.8333332538604736f, 0.0f), decodeAnchorY(0.9599999785423279f, 0.0f), decodeX(2.8333333f), decodeY(0.96f))
			path.lineTo(decodeX(2.5f), decodeY(0.96f))
			path.lineTo(decodeX(2.5f), decodeY(2.5f))
			path.lineTo(decodeX(0.5f), decodeY(2.5f))
			path.lineTo(decodeX(0.5f), decodeY(0.96f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath7() As Path2D
			path.reset()
			path.moveTo(decodeX(0.6666667f), decodeY(0.96f))
			path.lineTo(decodeX(0.33333334f), decodeY(0.96f))
			path.curveTo(decodeAnchorX(0.3333333432674408f, 0.0f), decodeAnchorY(0.9599999785423279f, 0.0f), decodeAnchorX(0.3333333432674408f, 0.0f), decodeAnchorY(2.3333332538604736f, -1.0f), decodeX(0.33333334f), decodeY(2.3333333f))
			path.curveTo(decodeAnchorX(0.3333333432674408f, 0.0f), decodeAnchorY(2.3333332538604736f, 1.0f), decodeAnchorX(0.6666666865348816f, -1.0f), decodeAnchorY(2.6666667461395264f, 0.0f), decodeX(0.6666667f), decodeY(2.6666667f))
			path.curveTo(decodeAnchorX(0.6666666865348816f, 1.0f), decodeAnchorY(2.6666667461395264f, 0.0f), decodeAnchorX(2.3333332538604736f, -1.0f), decodeAnchorY(2.6666667461395264f, 0.0f), decodeX(2.3333333f), decodeY(2.6666667f))
			path.curveTo(decodeAnchorX(2.3333332538604736f, 1.0f), decodeAnchorY(2.6666667461395264f, 0.0f), decodeAnchorX(2.6666667461395264f, 0.0f), decodeAnchorY(2.3333332538604736f, 1.0f), decodeX(2.6666667f), decodeY(2.3333333f))
			path.curveTo(decodeAnchorX(2.6666667461395264f, 0.0f), decodeAnchorY(2.3333332538604736f, -1.0f), decodeAnchorX(2.6666667461395264f, 0.0f), decodeAnchorY(0.9599999785423279f, 0.0f), decodeX(2.6666667f), decodeY(0.96f))
			path.lineTo(decodeX(2.3333333f), decodeY(0.96f))
			path.lineTo(decodeX(2.3333333f), decodeY(2.3333333f))
			path.lineTo(decodeX(0.6666667f), decodeY(2.3333333f))
			path.lineTo(decodeX(0.6666667f), decodeY(0.96f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath8() As Path2D
			path.reset()
			path.moveTo(decodeX(2.3333333f), decodeY(0.96f))
			path.lineTo(decodeX(2.1666667f), decodeY(0.96f))
			path.lineTo(decodeX(2.1666667f), decodeY(2.1666667f))
			path.lineTo(decodeX(0.8333333f), decodeY(2.1666667f))
			path.lineTo(decodeX(0.8333333f), decodeY(0.96f))
			path.lineTo(decodeX(0.6666667f), decodeY(0.96f))
			path.lineTo(decodeX(0.6666667f), decodeY(2.3333333f))
			path.lineTo(decodeX(2.3333333f), decodeY(2.3333333f))
			path.lineTo(decodeX(2.3333333f), decodeY(0.96f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath9() As Path2D
			path.reset()
			path.moveTo(decodeX(0.8333333f), decodeY(1.0f))
			path.lineTo(decodeX(0.8333333f), decodeY(2.1666667f))
			path.lineTo(decodeX(2.1666667f), decodeY(2.1666667f))
			path.lineTo(decodeX(2.1666667f), decodeY(0.96f))
			path.lineTo(decodeX(0.8333333f), decodeY(0.96f))
			path.lineTo(decodeX(0.8333333f), decodeY(1.0f))
			path.lineTo(decodeX(2.0f), decodeY(1.0f))
			path.lineTo(decodeX(2.0f), decodeY(2.0f))
			path.lineTo(decodeX(1.0f), decodeY(2.0f))
			path.lineTo(decodeX(1.0f), decodeY(1.0f))
			path.lineTo(decodeX(0.8333333f), decodeY(1.0f))
			path.closePath()
			Return path
		End Function

		Private Function decodeRect3() As Rectangle2D
				rect.rectect(decodeX(0.0f), decodeY(0.0f), decodeX(0.0f) - decodeX(0.0f), decodeY(0.0f) - decodeY(0.0f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodeRect4() As Rectangle2D
				rect.rectect(decodeX(0.33333334f), decodeY(0.08f), decodeX(2.6666667f) - decodeX(0.33333334f), decodeY(0.96f) - decodeY(0.08f)) 'height - width - y - x
			Return rect
		End Function



		Private Function decodeGradient1(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.3203593f,1.0f }, New Color() { color2, decodeColor(color2,color3,0.5f), color3})
		End Function

		Private Function decodeGradient2(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color10, decodeColor(color10,color11,0.5f), color11})
		End Function

		Private Function decodeGradient3(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.24251497f,1.0f }, New Color() { color16, decodeColor(color16,color17,0.5f), color17})
		End Function


	End Class

End Namespace
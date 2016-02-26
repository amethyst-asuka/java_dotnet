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



	Friend NotInheritable Class ScrollBarButtonPainter
		Inherits AbstractRegionPainter

		'package private integers representing the available states that
		'this painter will paint. These are used when creating a new instance
		'of ScrollBarButtonPainter to determine which region/state is being painted
		'by that instance.
		Friend Const FOREGROUND_ENABLED As Integer = 1
		Friend Const FOREGROUND_DISABLED As Integer = 2
		Friend Const FOREGROUND_MOUSEOVER As Integer = 3
		Friend Const FOREGROUND_PRESSED As Integer = 4


		Private state As Integer 'refers to one of the static final ints above
		Private ctx As PaintContext

		'the following 4 variables are reused during the painting code of the layers
		Private path As Path2D = New Path2D.Float
		Private rect As Rectangle2D = New Rectangle2D.Float(0, 0, 0, 0)
		Private roundRect As RoundRectangle2D = New RoundRectangle2D.Float(0, 0, 0, 0, 0, 0)
		Private ellipse As Ellipse2D = New Ellipse2D.Float(0, 0, 0, 0)

		'All Colors used for painting are stored here. Ideally, only those colors being used
		'by a particular instance of ScrollBarButtonPainter would be created. For the moment at least,
		'however, all are created for each instance.
		Private color1 As New Color(255, 200, 0, 255)
		Private color2 As Color = decodeColor("nimbusBlueGrey", -0.01111114f, -0.07763158f, -0.1490196f, 0)
		Private color3 As Color = decodeColor("nimbusBlueGrey", -0.111111104f, -0.10580933f, 0.086274505f, 0)
		Private color4 As Color = decodeColor("nimbusBlueGrey", -0.027777791f, -0.102261856f, 0.20392156f, 0)
		Private color5 As Color = decodeColor("nimbusBlueGrey", -0.039682567f, -0.079276316f, 0.13333333f, 0)
		Private color6 As Color = decodeColor("nimbusBlueGrey", -0.027777791f, -0.07382907f, 0.109803915f, 0)
		Private color7 As Color = decodeColor("nimbusBlueGrey", -0.039682567f, -0.08241387f, 0.23137254f, 0)
		Private color8 As Color = decodeColor("nimbusBlueGrey", -0.055555522f, -0.08443936f, -0.29411766f, -136)
		Private color9 As Color = decodeColor("nimbusBlueGrey", -0.055555522f, -0.09876161f, 0.25490195f, -178)
		Private color10 As Color = decodeColor("nimbusBlueGrey", 0.055555582f, -0.08878718f, -0.5647059f, 0)
		Private color11 As Color = decodeColor("nimbusBlueGrey", -0.027777791f, -0.080223285f, -0.4862745f, 0)
		Private color12 As Color = decodeColor("nimbusBlueGrey", -0.111111104f, -0.09525914f, -0.23137254f, 0)
		Private color13 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.110526316f, 0.25490195f, -165)
		Private color14 As Color = decodeColor("nimbusBlueGrey", -0.04444444f, -0.080223285f, -0.09803921f, 0)
		Private color15 As Color = decodeColor("nimbusBlueGrey", -0.6111111f, -0.110526316f, 0.10588235f, 0)
		Private color16 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.110526316f, 0.25490195f, 0)
		Private color17 As Color = decodeColor("nimbusBlueGrey", -0.039682567f, -0.081719734f, 0.20784312f, 0)
		Private color18 As Color = decodeColor("nimbusBlueGrey", -0.027777791f, -0.07677104f, 0.18431371f, 0)
		Private color19 As Color = decodeColor("nimbusBlueGrey", -0.04444444f, -0.080223285f, -0.09803921f, -69)
		Private color20 As Color = decodeColor("nimbusBlueGrey", -0.055555522f, -0.09876161f, 0.25490195f, -39)
		Private color21 As Color = decodeColor("nimbusBlueGrey", 0.055555582f, -0.0951417f, -0.49019608f, 0)
		Private color22 As Color = decodeColor("nimbusBlueGrey", -0.027777791f, -0.086996906f, -0.4117647f, 0)
		Private color23 As Color = decodeColor("nimbusBlueGrey", -0.111111104f, -0.09719298f, -0.15686274f, 0)
		Private color24 As Color = decodeColor("nimbusBlueGrey", -0.037037015f, -0.043859646f, -0.21568626f, 0)
		Private color25 As Color = decodeColor("nimbusBlueGrey", -0.06349206f, -0.07309316f, -0.011764705f, 0)
		Private color26 As Color = decodeColor("nimbusBlueGrey", -0.048611104f, -0.07296763f, 0.09019607f, 0)
		Private color27 As Color = decodeColor("nimbusBlueGrey", -0.03535354f, -0.05497076f, 0.031372547f, 0)
		Private color28 As Color = decodeColor("nimbusBlueGrey", -0.034188032f, -0.043168806f, 0.011764705f, 0)
		Private color29 As Color = decodeColor("nimbusBlueGrey", -0.03535354f, -0.0600676f, 0.109803915f, 0)
		Private color30 As Color = decodeColor("nimbusBlueGrey", -0.037037015f, -0.043859646f, -0.21568626f, -44)
		Private color31 As Color = decodeColor("nimbusBlueGrey", -0.6111111f, -0.110526316f, -0.74509805f, 0)


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
				Case FOREGROUND_ENABLED
					paintForegroundEnabled(g)
				Case FOREGROUND_DISABLED
					paintForegroundDisabled(g)
				Case FOREGROUND_MOUSEOVER
					paintForegroundMouseOver(g)
				Case FOREGROUND_PRESSED
					paintForegroundPressed(g)

			End Select
		End Sub



		Protected Friend Property NotOverridable Overrides paintContext As PaintContext
			Get
				Return ctx
			End Get
		End Property

		Private Sub paintForegroundEnabled(ByVal g As Graphics2D)
			path = decodePath1()
			g.paint = color1
			g.fill(path)
			path = decodePath2()
			g.paint = decodeGradient1(path)
			g.fill(path)
			path = decodePath3()
			g.paint = decodeGradient2(path)
			g.fill(path)
			path = decodePath4()
			g.paint = decodeGradient3(path)
			g.fill(path)
			path = decodePath5()
			g.paint = color13
			g.fill(path)

		End Sub

		Private Sub paintForegroundDisabled(ByVal g As Graphics2D)
			path = decodePath1()
			g.paint = color1
			g.fill(path)

		End Sub

		Private Sub paintForegroundMouseOver(ByVal g As Graphics2D)
			path = decodePath1()
			g.paint = color1
			g.fill(path)
			path = decodePath2()
			g.paint = decodeGradient4(path)
			g.fill(path)
			path = decodePath3()
			g.paint = decodeGradient5(path)
			g.fill(path)
			path = decodePath4()
			g.paint = decodeGradient6(path)
			g.fill(path)
			path = decodePath5()
			g.paint = color13
			g.fill(path)

		End Sub

		Private Sub paintForegroundPressed(ByVal g As Graphics2D)
			path = decodePath1()
			g.paint = color1
			g.fill(path)
			path = decodePath2()
			g.paint = decodeGradient7(path)
			g.fill(path)
			path = decodePath3()
			g.paint = decodeGradient8(path)
			g.fill(path)
			path = decodePath4()
			g.paint = color31
			g.fill(path)
			path = decodePath5()
			g.paint = color13
			g.fill(path)

		End Sub



		Private Function decodePath1() As Path2D
			path.reset()
			path.moveTo(decodeX(3.0f), decodeY(3.0f))
			path.lineTo(decodeX(3.0f), decodeY(3.0f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath2() As Path2D
			path.reset()
			path.moveTo(decodeX(0.0f), decodeY(0.0f))
			path.lineTo(decodeX(1.6956522f), decodeY(0.0f))
			path.curveTo(decodeAnchorX(1.6956522464752197f, 0.0f), decodeAnchorY(0.0f, 0.0f), decodeAnchorX(1.6956522464752197f, -0.7058823529411633f), decodeAnchorY(1.307692289352417f, -3.0294117647058822f), decodeX(1.6956522f), decodeY(1.3076923f))
			path.curveTo(decodeAnchorX(1.6956522464752197f, 0.7058823529411633f), decodeAnchorY(1.307692289352417f, 3.0294117647058822f), decodeAnchorX(1.8260869979858398f, -2.0f), decodeAnchorY(1.769230842590332f, -1.9411764705882355f), decodeX(1.826087f), decodeY(1.7692308f))
			path.curveTo(decodeAnchorX(1.8260869979858398f, 2.0f), decodeAnchorY(1.769230842590332f, 1.9411764705882355f), decodeAnchorX(3.0f, 0.0f), decodeAnchorY(2.0f, 0.0f), decodeX(3.0f), decodeY(2.0f))
			path.lineTo(decodeX(3.0f), decodeY(3.0f))
			path.lineTo(decodeX(0.0f), decodeY(3.0f))
			path.lineTo(decodeX(0.0f), decodeY(0.0f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath3() As Path2D
			path.reset()
			path.moveTo(decodeX(0.0f), decodeY(1.0022625f))
			path.lineTo(decodeX(0.9705882f), decodeY(1.0384616f))
			path.lineTo(decodeX(1.0409207f), decodeY(1.0791855f))
			path.lineTo(decodeX(1.0409207f), decodeY(3.0f))
			path.lineTo(decodeX(0.0f), decodeY(3.0f))
			path.lineTo(decodeX(0.0f), decodeY(1.0022625f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath4() As Path2D
			path.reset()
			path.moveTo(decodeX(1.4782609f), decodeY(1.2307693f))
			path.lineTo(decodeX(1.4782609f), decodeY(1.7692308f))
			path.lineTo(decodeX(1.1713555f), decodeY(1.5f))
			path.lineTo(decodeX(1.4782609f), decodeY(1.2307693f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath5() As Path2D
			path.reset()
			path.moveTo(decodeX(1.6713555f), decodeY(1.0769231f))
			path.curveTo(decodeAnchorX(1.6713554859161377f, 0.7352941176470615f), decodeAnchorY(1.076923131942749f, 0.0f), decodeAnchorX(1.718670129776001f, -0.911764705882355f), decodeAnchorY(1.4095022678375244f, -2.2058823529411784f), decodeX(1.7186701f), decodeY(1.4095023f))
			path.curveTo(decodeAnchorX(1.718670129776001f, 0.911764705882355f), decodeAnchorY(1.4095022678375244f, 2.2058823529411784f), decodeAnchorX(1.8439897298812866f, -2.3529411764705905f), decodeAnchorY(1.7941176891326904f, -1.852941176470587f), decodeX(1.8439897f), decodeY(1.7941177f))
			path.curveTo(decodeAnchorX(1.8439897298812866f, 2.3529411764705905f), decodeAnchorY(1.7941176891326904f, 1.852941176470587f), decodeAnchorX(2.5f, 0.0f), decodeAnchorY(2.2352943420410156f, 0.0f), decodeX(2.5f), decodeY(2.2352943f))
			path.lineTo(decodeX(2.3529415f), decodeY(2.8235292f))
			path.curveTo(decodeAnchorX(2.3529415130615234f, 0.0f), decodeAnchorY(2.8235292434692383f, 0.0f), decodeAnchorX(1.818414330482483f, 1.5588235294117645f), decodeAnchorY(1.8438913822174072f, 1.3823529411764675f), decodeX(1.8184143f), decodeY(1.8438914f))
			path.curveTo(decodeAnchorX(1.818414330482483f, -1.5588235294117645f), decodeAnchorY(1.8438913822174072f, -1.3823529411764675f), decodeAnchorX(1.694373369216919f, 0.7941176470588225f), decodeAnchorY(1.4841628074645996f, 1.9999999999999991f), decodeX(1.6943734f), decodeY(1.4841628f))
			path.curveTo(decodeAnchorX(1.694373369216919f, -0.7941176470588225f), decodeAnchorY(1.4841628074645996f, -1.9999999999999991f), decodeAnchorX(1.6713554859161377f, -0.7352941176470598f), decodeAnchorY(1.076923131942749f, 0.0f), decodeX(1.6713555f), decodeY(1.0769231f))
			path.closePath()
			Return path
		End Function



		Private Function decodeGradient1(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.032934133f,0.065868266f,0.089820355f,0.11377245f,0.23053892f,0.3473054f,0.494012f,0.6407186f,0.78443116f,0.92814374f }, New Color() { color2, decodeColor(color2,color3,0.5f), color3, decodeColor(color3,color4,0.5f), color4, decodeColor(color4,color5,0.5f), color5, decodeColor(color5,color6,0.5f), color6, decodeColor(color6,color7,0.5f), color7})
		End Function

		Private Function decodeGradient2(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.0f * w) + x, (0.5f * h) + y, (0.5735294f * w) + x, (0.5f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color8, decodeColor(color8,color9,0.5f), color9})
		End Function

		Private Function decodeGradient3(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.925f * w) + x, (0.9285714f * h) + y, (0.925f * w) + x, (0.004201681f * h) + y, New Single() { 0.0f,0.2964072f,0.5928144f,0.79341316f,0.994012f }, New Color() { color10, decodeColor(color10,color11,0.5f), color11, decodeColor(color11,color12,0.5f), color12})
		End Function

		Private Function decodeGradient4(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.032934133f,0.065868266f,0.089820355f,0.11377245f,0.23053892f,0.3473054f,0.494012f,0.6407186f,0.78443116f,0.92814374f }, New Color() { color14, decodeColor(color14,color15,0.5f), color15, decodeColor(color15,color16,0.5f), color16, decodeColor(color16,color17,0.5f), color17, decodeColor(color17,color18,0.5f), color18, decodeColor(color18,color16,0.5f), color16})
		End Function

		Private Function decodeGradient5(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.0f * w) + x, (0.5f * h) + y, (0.5735294f * w) + x, (0.5f * h) + y, New Single() { 0.19518717f,0.5975936f,1.0f }, New Color() { color19, decodeColor(color19,color20,0.5f), color20})
		End Function

		Private Function decodeGradient6(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.925f * w) + x, (0.9285714f * h) + y, (0.925f * w) + x, (0.004201681f * h) + y, New Single() { 0.0f,0.2964072f,0.5928144f,0.79341316f,0.994012f }, New Color() { color21, decodeColor(color21,color22,0.5f), color22, decodeColor(color22,color23,0.5f), color23})
		End Function

		Private Function decodeGradient7(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.032934133f,0.065868266f,0.089820355f,0.11377245f,0.23053892f,0.3473054f,0.494012f,0.6407186f,0.78443116f,0.92814374f }, New Color() { color24, decodeColor(color24,color25,0.5f), color25, decodeColor(color25,color26,0.5f), color26, decodeColor(color26,color27,0.5f), color27, decodeColor(color27,color28,0.5f), color28, decodeColor(color28,color29,0.5f), color29})
		End Function

		Private Function decodeGradient8(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.0f * w) + x, (0.5f * h) + y, (0.5735294f * w) + x, (0.5f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color30, decodeColor(color30,color9,0.5f), color9})
		End Function


	End Class

End Namespace
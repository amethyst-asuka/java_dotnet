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



	Friend NotInheritable Class OptionPanePainter
		Inherits AbstractRegionPainter

		'package private integers representing the available states that
		'this painter will paint. These are used when creating a new instance
		'of OptionPanePainter to determine which region/state is being painted
		'by that instance.
		Friend Const BACKGROUND_ENABLED As Integer = 1
		Friend Const ERRORICON_ENABLED As Integer = 2
		Friend Const INFORMATIONICON_ENABLED As Integer = 3
		Friend Const QUESTIONICON_ENABLED As Integer = 4
		Friend Const WARNINGICON_ENABLED As Integer = 5


		Private state As Integer 'refers to one of the static final ints above
		Private ctx As PaintContext

		'the following 4 variables are reused during the painting code of the layers
		Private path As Path2D = New Path2D.Float
		Private rect As Rectangle2D = New Rectangle2D.Float(0, 0, 0, 0)
		Private roundRect As RoundRectangle2D = New RoundRectangle2D.Float(0, 0, 0, 0, 0, 0)
		Private ellipse As Ellipse2D = New Ellipse2D.Float(0, 0, 0, 0)

		'All Colors used for painting are stored here. Ideally, only those colors being used
		'by a particular instance of OptionPanePainter would be created. For the moment at least,
		'however, all are created for each instance.
		Private color1 As Color = decodeColor("nimbusRed", -0.014814814f, 0.18384242f, 0.015686274f, 0)
		Private color2 As Color = decodeColor("nimbusRed", -0.014814814f, -0.403261f, 0.21960783f, 0)
		Private color3 As Color = decodeColor("nimbusRed", -0.014814814f, -0.07154381f, 0.11372548f, 0)
		Private color4 As Color = decodeColor("nimbusRed", -0.014814814f, 0.110274374f, 0.07058823f, 0)
		Private color5 As Color = decodeColor("nimbusRed", -0.014814814f, -0.05413574f, 0.2588235f, 0)
		Private color6 As New Color(250, 250, 250, 255)
		Private color7 As Color = decodeColor("nimbusRed", 0.0f, -0.79881656f, 0.33725488f, -187)
		Private color8 As New Color(255, 200, 0, 255)
		Private color9 As Color = decodeColor("nimbusInfoBlue", 0.0f, 0.06231594f, -0.054901958f, 0)
		Private color10 As Color = decodeColor("nimbusInfoBlue", 3.1620264E-4f, 0.07790506f, -0.19215685f, 0)
		Private color11 As Color = decodeColor("nimbusInfoBlue", -8.2296133E-4f, -0.44631243f, 0.19215685f, 0)
		Private color12 As Color = decodeColor("nimbusInfoBlue", 0.0012729168f, -0.0739674f, 0.043137252f, 0)
		Private color13 As Color = decodeColor("nimbusInfoBlue", 8.354187E-4f, -0.14148629f, 0.19999999f, 0)
		Private color14 As Color = decodeColor("nimbusInfoBlue", -0.0014793873f, -0.41456455f, 0.16470587f, 0)
		Private color15 As Color = decodeColor("nimbusInfoBlue", 3.437996E-4f, -0.14726585f, 0.043137252f, 0)
		Private color16 As Color = decodeColor("nimbusInfoBlue", -4.271865E-4f, -0.0055555105f, 0.0f, 0)
		Private color17 As Color = decodeColor("nimbusInfoBlue", 0.0f, 0.0f, 0.0f, 0)
		Private color18 As Color = decodeColor("nimbusInfoBlue", -7.866621E-4f, -0.12728173f, 0.17254901f, 0)
		Private color19 As New Color(115, 120, 126, 255)
		Private color20 As New Color(26, 34, 43, 255)
		Private color21 As New Color(168, 173, 178, 255)
		Private color22 As New Color(101, 109, 118, 255)
		Private color23 As New Color(159, 163, 168, 255)
		Private color24 As New Color(116, 122, 130, 255)
		Private color25 As New Color(96, 104, 112, 255)
		Private color26 As New Color(118, 128, 138, 255)
		Private color27 As New Color(255, 255, 255, 255)
		Private color28 As Color = decodeColor("nimbusAlertYellow", -4.9102306E-4f, 0.1372549f, -0.15294117f, 0)
		Private color29 As Color = decodeColor("nimbusAlertYellow", -0.0015973002f, 0.1372549f, -0.3490196f, 0)
		Private color30 As Color = decodeColor("nimbusAlertYellow", 6.530881E-4f, -0.40784314f, 0.0f, 0)
		Private color31 As Color = decodeColor("nimbusAlertYellow", -3.9456785E-4f, -0.109803915f, 0.0f, 0)
		Private color32 As Color = decodeColor("nimbusAlertYellow", 0.0f, 0.0f, 0.0f, 0)
		Private color33 As Color = decodeColor("nimbusAlertYellow", 0.008085668f, -0.04705882f, 0.0f, 0)
		Private color34 As Color = decodeColor("nimbusAlertYellow", 0.026515156f, -0.18431371f, 0.0f, 0)
		Private color35 As New Color(69, 69, 69, 255)
		Private color36 As New Color(0, 0, 0, 255)
		Private color37 As New Color(16, 16, 16, 255)


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
				Case ERRORICON_ENABLED
					painterrorIconEnabled(g)
				Case INFORMATIONICON_ENABLED
					paintinformationIconEnabled(g)
				Case QUESTIONICON_ENABLED
					paintquestionIconEnabled(g)
				Case WARNINGICON_ENABLED
					paintwarningIconEnabled(g)

			End Select
		End Sub



		Protected Friend Property NotOverridable Overrides paintContext As PaintContext
			Get
				Return ctx
			End Get
		End Property

		Private Sub painterrorIconEnabled(ByVal g As Graphics2D)
			path = decodePath1()
			g.paint = color1
			g.fill(path)
			path = decodePath2()
			g.paint = decodeGradient1(path)
			g.fill(path)
			path = decodePath3()
			g.paint = color6
			g.fill(path)
			ellipse = decodeEllipse1()
			g.paint = color6
			g.fill(ellipse)
			path = decodePath4()
			g.paint = color7
			g.fill(path)

		End Sub

		Private Sub paintinformationIconEnabled(ByVal g As Graphics2D)
			ellipse = decodeEllipse2()
			g.paint = color8
			g.fill(ellipse)
			ellipse = decodeEllipse2()
			g.paint = color8
			g.fill(ellipse)
			ellipse = decodeEllipse2()
			g.paint = color8
			g.fill(ellipse)
			ellipse = decodeEllipse3()
			g.paint = decodeGradient2(ellipse)
			g.fill(ellipse)
			ellipse = decodeEllipse4()
			g.paint = decodeGradient3(ellipse)
			g.fill(ellipse)
			ellipse = decodeEllipse5()
			g.paint = decodeGradient4(ellipse)
			g.fill(ellipse)
			path = decodePath5()
			g.paint = color6
			g.fill(path)
			ellipse = decodeEllipse6()
			g.paint = color6
			g.fill(ellipse)

		End Sub

		Private Sub paintquestionIconEnabled(ByVal g As Graphics2D)
			ellipse = decodeEllipse3()
			g.paint = decodeGradient5(ellipse)
			g.fill(ellipse)
			ellipse = decodeEllipse4()
			g.paint = decodeGradient6(ellipse)
			g.fill(ellipse)
			ellipse = decodeEllipse5()
			g.paint = decodeGradient7(ellipse)
			g.fill(ellipse)
			path = decodePath6()
			g.paint = color27
			g.fill(path)
			ellipse = decodeEllipse1()
			g.paint = color27
			g.fill(ellipse)

		End Sub

		Private Sub paintwarningIconEnabled(ByVal g As Graphics2D)
			rect = decodeRect1()
			g.paint = color8
			g.fill(rect)
			path = decodePath7()
			g.paint = decodeGradient8(path)
			g.fill(path)
			path = decodePath8()
			g.paint = decodeGradient9(path)
			g.fill(path)
			path = decodePath9()
			g.paint = decodeGradient10(path)
			g.fill(path)
			ellipse = decodeEllipse7()
			g.paint = color37
			g.fill(ellipse)

		End Sub



		Private Function decodePath1() As Path2D
			path.reset()
			path.moveTo(decodeX(1.0f), decodeY(1.2708334f))
			path.lineTo(decodeX(1.2708334f), decodeY(1.0f))
			path.lineTo(decodeX(1.6875f), decodeY(1.0f))
			path.lineTo(decodeX(1.9583333f), decodeY(1.2708334f))
			path.lineTo(decodeX(1.9583333f), decodeY(1.6875f))
			path.lineTo(decodeX(1.6875f), decodeY(1.9583333f))
			path.lineTo(decodeX(1.2708334f), decodeY(1.9583333f))
			path.lineTo(decodeX(1.0f), decodeY(1.6875f))
			path.lineTo(decodeX(1.0f), decodeY(1.2708334f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath2() As Path2D
			path.reset()
			path.moveTo(decodeX(1.0208334f), decodeY(1.2916666f))
			path.lineTo(decodeX(1.2916666f), decodeY(1.0208334f))
			path.lineTo(decodeX(1.6666667f), decodeY(1.0208334f))
			path.lineTo(decodeX(1.9375f), decodeY(1.2916666f))
			path.lineTo(decodeX(1.9375f), decodeY(1.6666667f))
			path.lineTo(decodeX(1.6666667f), decodeY(1.9375f))
			path.lineTo(decodeX(1.2916666f), decodeY(1.9375f))
			path.lineTo(decodeX(1.0208334f), decodeY(1.6666667f))
			path.lineTo(decodeX(1.0208334f), decodeY(1.2916666f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath3() As Path2D
			path.reset()
			path.moveTo(decodeX(1.4166666f), decodeY(1.2291666f))
			path.curveTo(decodeAnchorX(1.4166666269302368f, 0.0f), decodeAnchorY(1.2291666269302368f, -2.0f), decodeAnchorX(1.4791666269302368f, -2.0f), decodeAnchorY(1.1666666269302368f, 0.0f), decodeX(1.4791666f), decodeY(1.1666666f))
			path.curveTo(decodeAnchorX(1.4791666269302368f, 2.0f), decodeAnchorY(1.1666666269302368f, 0.0f), decodeAnchorX(1.5416667461395264f, 0.0f), decodeAnchorY(1.2291666269302368f, -2.0f), decodeX(1.5416667f), decodeY(1.2291666f))
			path.curveTo(decodeAnchorX(1.5416667461395264f, 0.0f), decodeAnchorY(1.2291666269302368f, 2.0f), decodeAnchorX(1.5f, 0.0f), decodeAnchorY(1.6041667461395264f, 0.0f), decodeX(1.5f), decodeY(1.6041667f))
			path.lineTo(decodeX(1.4583334f), decodeY(1.6041667f))
			path.curveTo(decodeAnchorX(1.4583333730697632f, 0.0f), decodeAnchorY(1.6041667461395264f, 0.0f), decodeAnchorX(1.4166666269302368f, 0.0f), decodeAnchorY(1.2291666269302368f, 2.0f), decodeX(1.4166666f), decodeY(1.2291666f))
			path.closePath()
			Return path
		End Function

		Private Function decodeEllipse1() As Ellipse2D
			ellipse.frameame(decodeX(1.4166666f), decodeY(1.6666667f), decodeX(1.5416667f) - decodeX(1.4166666f), decodeY(1.7916667f) - decodeY(1.6666667f)) 'height - width - y - x
			Return ellipse
		End Function

		Private Function decodePath4() As Path2D
			path.reset()
			path.moveTo(decodeX(1.0208334f), decodeY(1.2851562f))
			path.lineTo(decodeX(1.2799479f), decodeY(1.0208334f))
			path.lineTo(decodeX(1.6783855f), decodeY(1.0208334f))
			path.lineTo(decodeX(1.9375f), decodeY(1.28125f))
			path.lineTo(decodeX(1.9375f), decodeY(1.6666667f))
			path.lineTo(decodeX(1.6666667f), decodeY(1.9375f))
			path.lineTo(decodeX(1.2851562f), decodeY(1.936198f))
			path.lineTo(decodeX(1.0221354f), decodeY(1.673177f))
			path.lineTo(decodeX(1.0208334f), decodeY(1.5f))
			path.lineTo(decodeX(1.0416666f), decodeY(1.5f))
			path.lineTo(decodeX(1.0416666f), decodeY(1.6666667f))
			path.lineTo(decodeX(1.2916666f), decodeY(1.9166667f))
			path.lineTo(decodeX(1.6666667f), decodeY(1.9166667f))
			path.lineTo(decodeX(1.9166667f), decodeY(1.6666667f))
			path.lineTo(decodeX(1.9166667f), decodeY(1.2916666f))
			path.lineTo(decodeX(1.6666667f), decodeY(1.0416666f))
			path.lineTo(decodeX(1.2916666f), decodeY(1.0416666f))
			path.lineTo(decodeX(1.0416666f), decodeY(1.2916666f))
			path.lineTo(decodeX(1.0416666f), decodeY(1.5f))
			path.lineTo(decodeX(1.0208334f), decodeY(1.5f))
			path.lineTo(decodeX(1.0208334f), decodeY(1.2851562f))
			path.closePath()
			Return path
		End Function

		Private Function decodeEllipse2() As Ellipse2D
			ellipse.frameame(decodeX(1.0f), decodeY(1.0f), decodeX(1.0f) - decodeX(1.0f), decodeY(1.0f) - decodeY(1.0f)) 'height - width - y - x
			Return ellipse
		End Function

		Private Function decodeEllipse3() As Ellipse2D
			ellipse.frameame(decodeX(1.0f), decodeY(1.0f), decodeX(1.9583333f) - decodeX(1.0f), decodeY(1.9583333f) - decodeY(1.0f)) 'height - width - y - x
			Return ellipse
		End Function

		Private Function decodeEllipse4() As Ellipse2D
			ellipse.frameame(decodeX(1.0208334f), decodeY(1.0208334f), decodeX(1.9375f) - decodeX(1.0208334f), decodeY(1.9375f) - decodeY(1.0208334f)) 'height - width - y - x
			Return ellipse
		End Function

		Private Function decodeEllipse5() As Ellipse2D
			ellipse.frameame(decodeX(1.0416666f), decodeY(1.0416666f), decodeX(1.9166667f) - decodeX(1.0416666f), decodeY(1.9166667f) - decodeY(1.0416666f)) 'height - width - y - x
			Return ellipse
		End Function

		Private Function decodePath5() As Path2D
			path.reset()
			path.moveTo(decodeX(1.375f), decodeY(1.375f))
			path.curveTo(decodeAnchorX(1.375f, 2.5f), decodeAnchorY(1.375f, 0.0f), decodeAnchorX(1.5f, -1.1875f), decodeAnchorY(1.375f, 0.0f), decodeX(1.5f), decodeY(1.375f))
			path.curveTo(decodeAnchorX(1.5f, 1.1875f), decodeAnchorY(1.375f, 0.0f), decodeAnchorX(1.5416667461395264f, 0.0f), decodeAnchorY(1.4375f, -2.0f), decodeX(1.5416667f), decodeY(1.4375f))
			path.curveTo(decodeAnchorX(1.5416667461395264f, 0.0f), decodeAnchorY(1.4375f, 2.0f), decodeAnchorX(1.5416667461395264f, 0.0f), decodeAnchorY(1.6875f, 0.0f), decodeX(1.5416667f), decodeY(1.6875f))
			path.curveTo(decodeAnchorX(1.5416667461395264f, 0.0f), decodeAnchorY(1.6875f, 0.0f), decodeAnchorX(1.6028645038604736f, -2.5625f), decodeAnchorY(1.6875f, 0.0625f), decodeX(1.6028645f), decodeY(1.6875f))
			path.curveTo(decodeAnchorX(1.6028645038604736f, 2.5625f), decodeAnchorY(1.6875f, -0.0625f), decodeAnchorX(1.6041667461395264f, 2.5625f), decodeAnchorY(1.7708332538604736f, 0.0f), decodeX(1.6041667f), decodeY(1.7708333f))
			path.curveTo(decodeAnchorX(1.6041667461395264f, -2.5625f), decodeAnchorY(1.7708332538604736f, 0.0f), decodeAnchorX(1.3567708730697632f, 2.5f), decodeAnchorY(1.7708332538604736f, 0.0625f), decodeX(1.3567709f), decodeY(1.7708333f))
			path.curveTo(decodeAnchorX(1.3567708730697632f, -2.5f), decodeAnchorY(1.7708332538604736f, -0.0625f), decodeAnchorX(1.3541666269302368f, -2.4375f), decodeAnchorY(1.6875f, 0.0f), decodeX(1.3541666f), decodeY(1.6875f))
			path.curveTo(decodeAnchorX(1.3541666269302368f, 2.4375f), decodeAnchorY(1.6875f, 0.0f), decodeAnchorX(1.4166666269302368f, 0.0f), decodeAnchorY(1.6875f, 0.0f), decodeX(1.4166666f), decodeY(1.6875f))
			path.lineTo(decodeX(1.4166666f), decodeY(1.4583334f))
			path.curveTo(decodeAnchorX(1.4166666269302368f, 0.0f), decodeAnchorY(1.4583333730697632f, 0.0f), decodeAnchorX(1.375f, 2.75f), decodeAnchorY(1.4583333730697632f, 0.0f), decodeX(1.375f), decodeY(1.4583334f))
			path.curveTo(decodeAnchorX(1.375f, -2.75f), decodeAnchorY(1.4583333730697632f, 0.0f), decodeAnchorX(1.375f, -2.5f), decodeAnchorY(1.375f, 0.0f), decodeX(1.375f), decodeY(1.375f))
			path.closePath()
			Return path
		End Function

		Private Function decodeEllipse6() As Ellipse2D
			ellipse.frameame(decodeX(1.4166666f), decodeY(1.1666666f), decodeX(1.5416667f) - decodeX(1.4166666f), decodeY(1.2916666f) - decodeY(1.1666666f)) 'height - width - y - x
			Return ellipse
		End Function

		Private Function decodePath6() As Path2D
			path.reset()
			path.moveTo(decodeX(1.3125f), decodeY(1.3723959f))
			path.curveTo(decodeAnchorX(1.3125f, 1.5f), decodeAnchorY(1.3723958730697632f, 1.375f), decodeAnchorX(1.3997396230697632f, -0.75f), decodeAnchorY(1.3580728769302368f, 1.1875f), decodeX(1.3997396f), decodeY(1.3580729f))
			path.curveTo(decodeAnchorX(1.3997396230697632f, 0.75f), decodeAnchorY(1.3580728769302368f, -1.1875f), decodeAnchorX(1.46875f, -1.8125f), decodeAnchorY(1.2903646230697632f, 0.0f), decodeX(1.46875f), decodeY(1.2903646f))
			path.curveTo(decodeAnchorX(1.46875f, 1.8125f), decodeAnchorY(1.2903646230697632f, 0.0f), decodeAnchorX(1.53515625f, 0.0f), decodeAnchorY(1.3502603769302368f, -1.5625f), decodeX(1.5351562f), decodeY(1.3502604f))
			path.curveTo(decodeAnchorX(1.53515625f, 0.0f), decodeAnchorY(1.3502603769302368f, 1.5625f), decodeAnchorX(1.4700521230697632f, 1.25f), decodeAnchorY(1.4283853769302368f, -1.1875f), decodeX(1.4700521f), decodeY(1.4283854f))
			path.curveTo(decodeAnchorX(1.4700521230697632f, -1.25f), decodeAnchorY(1.4283853769302368f, 1.1875f), decodeAnchorX(1.41796875f, -0.0625f), decodeAnchorY(1.5442707538604736f, -1.5f), decodeX(1.4179688f), decodeY(1.5442708f))
			path.curveTo(decodeAnchorX(1.41796875f, 0.0625f), decodeAnchorY(1.5442707538604736f, 1.5f), decodeAnchorX(1.4765625f, -1.3125f), decodeAnchorY(1.6028645038604736f, 0.0f), decodeX(1.4765625f), decodeY(1.6028645f))
			path.curveTo(decodeAnchorX(1.4765625f, 1.3125f), decodeAnchorY(1.6028645038604736f, 0.0f), decodeAnchorX(1.5403645038604736f, 0.0f), decodeAnchorY(1.546875f, 1.625f), decodeX(1.5403645f), decodeY(1.546875f))
			path.curveTo(decodeAnchorX(1.5403645038604736f, 0.0f), decodeAnchorY(1.546875f, -1.625f), decodeAnchorX(1.61328125f, -1.1875f), decodeAnchorY(1.46484375f, 1.25f), decodeX(1.6132812f), decodeY(1.4648438f))
			path.curveTo(decodeAnchorX(1.61328125f, 1.1875f), decodeAnchorY(1.46484375f, -1.25f), decodeAnchorX(1.6666667461395264f, 0.0625f), decodeAnchorY(1.3463541269302368f, 3.3125f), decodeX(1.6666667f), decodeY(1.3463541f))
			path.curveTo(decodeAnchorX(1.6666667461395264f, -0.0625f), decodeAnchorY(1.3463541269302368f, -3.3125f), decodeAnchorX(1.4830728769302368f, 6.125f), decodeAnchorY(1.16796875f, -0.0625f), decodeX(1.4830729f), decodeY(1.1679688f))
			path.curveTo(decodeAnchorX(1.4830728769302368f, -6.125f), decodeAnchorY(1.16796875f, 0.0625f), decodeAnchorX(1.3046875f, 0.4375f), decodeAnchorY(1.2890625f, -1.25f), decodeX(1.3046875f), decodeY(1.2890625f))
			path.curveTo(decodeAnchorX(1.3046875f, -0.4375f), decodeAnchorY(1.2890625f, 1.25f), decodeAnchorX(1.3125f, -1.5f), decodeAnchorY(1.3723958730697632f, -1.375f), decodeX(1.3125f), decodeY(1.3723959f))
			path.closePath()
			Return path
		End Function

		Private Function decodeRect1() As Rectangle2D
				rect.rectect(decodeX(1.0f), decodeY(1.0f), decodeX(1.0f) - decodeX(1.0f), decodeY(1.0f) - decodeY(1.0f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodePath7() As Path2D
			path.reset()
			path.moveTo(decodeX(1.5f), decodeY(1.0208334f))
			path.curveTo(decodeAnchorX(1.5f, 2.0f), decodeAnchorY(1.0208333730697632f, 0.0f), decodeAnchorX(1.56640625f, 0.0f), decodeAnchorY(1.08203125f, 0.0f), decodeX(1.5664062f), decodeY(1.0820312f))
			path.lineTo(decodeX(1.9427083f), decodeY(1.779948f))
			path.curveTo(decodeAnchorX(1.9427082538604736f, 0.0f), decodeAnchorY(1.7799479961395264f, 0.0f), decodeAnchorX(1.9752604961395264f, 0.0f), decodeAnchorY(1.8802082538604736f, -2.375f), decodeX(1.9752605f), decodeY(1.8802083f))
			path.curveTo(decodeAnchorX(1.9752604961395264f, 0.0f), decodeAnchorY(1.8802082538604736f, 2.375f), decodeAnchorX(1.9166667461395264f, 0.0f), decodeAnchorY(1.9375f, 0.0f), decodeX(1.9166667f), decodeY(1.9375f))
			path.lineTo(decodeX(1.0833334f), decodeY(1.9375f))
			path.curveTo(decodeAnchorX(1.0833333730697632f, 0.0f), decodeAnchorY(1.9375f, 0.0f), decodeAnchorX(1.0247396230697632f, 0.125f), decodeAnchorY(1.8815104961395264f, 2.25f), decodeX(1.0247396f), decodeY(1.8815105f))
			path.curveTo(decodeAnchorX(1.0247396230697632f, -0.125f), decodeAnchorY(1.8815104961395264f, -2.25f), decodeAnchorX(1.0598958730697632f, 0.0f), decodeAnchorY(1.78125f, 0.0f), decodeX(1.0598959f), decodeY(1.78125f))
			path.lineTo(decodeX(1.4375f), decodeY(1.0833334f))
			path.curveTo(decodeAnchorX(1.4375f, 0.0f), decodeAnchorY(1.0833333730697632f, 0.0f), decodeAnchorX(1.5f, -2.0f), decodeAnchorY(1.0208333730697632f, 0.0f), decodeX(1.5f), decodeY(1.0208334f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath8() As Path2D
			path.reset()
			path.moveTo(decodeX(1.4986979f), decodeY(1.0429688f))
			path.curveTo(decodeAnchorX(1.4986978769302368f, 1.75f), decodeAnchorY(1.04296875f, 0.0f), decodeAnchorX(1.5546875f, 0.0f), decodeAnchorY(1.0950521230697632f, 0.0f), decodeX(1.5546875f), decodeY(1.0950521f))
			path.lineTo(decodeX(1.9322917f), decodeY(1.8007812f))
			path.curveTo(decodeAnchorX(1.9322917461395264f, 0.0f), decodeAnchorY(1.80078125f, 0.0f), decodeAnchorX(1.95703125f, 0.0f), decodeAnchorY(1.875f, -1.4375f), decodeX(1.9570312f), decodeY(1.875f))
			path.curveTo(decodeAnchorX(1.95703125f, 0.0f), decodeAnchorY(1.875f, 1.4375f), decodeAnchorX(1.8841145038604736f, 0.0f), decodeAnchorY(1.9166667461395264f, 0.0f), decodeX(1.8841145f), decodeY(1.9166667f))
			path.lineTo(decodeX(1.1002604f), decodeY(1.9166667f))
			path.curveTo(decodeAnchorX(1.1002603769302368f, 0.0f), decodeAnchorY(1.9166667461395264f, 0.0f), decodeAnchorX(1.0455728769302368f, 0.0625f), decodeAnchorY(1.8723957538604736f, 1.625f), decodeX(1.0455729f), decodeY(1.8723958f))
			path.curveTo(decodeAnchorX(1.0455728769302368f, -0.0625f), decodeAnchorY(1.8723957538604736f, -1.625f), decodeAnchorX(1.0755208730697632f, 0.0f), decodeAnchorY(1.7903645038604736f, 0.0f), decodeX(1.0755209f), decodeY(1.7903645f))
			path.lineTo(decodeX(1.4414062f), decodeY(1.1028646f))
			path.curveTo(decodeAnchorX(1.44140625f, 0.0f), decodeAnchorY(1.1028646230697632f, 0.0f), decodeAnchorX(1.4986978769302368f, -1.75f), decodeAnchorY(1.04296875f, 0.0f), decodeX(1.4986979f), decodeY(1.0429688f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath9() As Path2D
			path.reset()
			path.moveTo(decodeX(1.5f), decodeY(1.2291666f))
			path.curveTo(decodeAnchorX(1.5f, 2.0f), decodeAnchorY(1.2291666269302368f, 0.0f), decodeAnchorX(1.5625f, 0.0f), decodeAnchorY(1.3125f, -2.0f), decodeX(1.5625f), decodeY(1.3125f))
			path.curveTo(decodeAnchorX(1.5625f, 0.0f), decodeAnchorY(1.3125f, 2.0f), decodeAnchorX(1.5f, 1.3125f), decodeAnchorY(1.6666667461395264f, 0.0f), decodeX(1.5f), decodeY(1.6666667f))
			path.curveTo(decodeAnchorX(1.5f, -1.3125f), decodeAnchorY(1.6666667461395264f, 0.0f), decodeAnchorX(1.4375f, 0.0f), decodeAnchorY(1.3125f, 2.0f), decodeX(1.4375f), decodeY(1.3125f))
			path.curveTo(decodeAnchorX(1.4375f, 0.0f), decodeAnchorY(1.3125f, -2.0f), decodeAnchorX(1.5f, -2.0f), decodeAnchorY(1.2291666269302368f, 0.0f), decodeX(1.5f), decodeY(1.2291666f))
			path.closePath()
			Return path
		End Function

		Private Function decodeEllipse7() As Ellipse2D
			ellipse.frameame(decodeX(1.4375f), decodeY(1.7291667f), decodeX(1.5625f) - decodeX(1.4375f), decodeY(1.8541667f) - decodeY(1.7291667f)) 'height - width - y - x
			Return ellipse
		End Function



		Private Function decodeGradient1(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.17258064f,0.3451613f,0.5145161f,0.683871f,0.9f,1.0f }, New Color() { color2, decodeColor(color2,color3,0.5f), color3, decodeColor(color3,color4,0.5f), color4, decodeColor(color4,color5,0.5f), color5})
		End Function

		Private Function decodeGradient2(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color9, decodeColor(color9,color10,0.5f), color10})
		End Function

		Private Function decodeGradient3(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.24143836f,0.48287672f,0.7414384f,1.0f }, New Color() { color11, decodeColor(color11,color12,0.5f), color12, decodeColor(color12,color13,0.5f), color13})
		End Function

		Private Function decodeGradient4(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.14212328f,0.28424656f,0.39212328f,0.5f,0.60958904f,0.7191781f,0.85958904f,1.0f }, New Color() { color14, decodeColor(color14,color15,0.5f), color15, decodeColor(color15,color16,0.5f), color16, decodeColor(color16,color17,0.5f), color17, decodeColor(color17,color18,0.5f), color18})
		End Function

		Private Function decodeGradient5(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color19, decodeColor(color19,color20,0.5f), color20})
		End Function

		Private Function decodeGradient6(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color21, decodeColor(color21,color22,0.5f), color22})
		End Function

		Private Function decodeGradient7(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.15239726f,0.30479452f,0.47945207f,0.6541096f,0.8270548f,1.0f }, New Color() { color23, decodeColor(color23,color24,0.5f), color24, decodeColor(color24,color25,0.5f), color25, decodeColor(color25,color26,0.5f), color26})
		End Function

		Private Function decodeGradient8(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color28, decodeColor(color28,color29,0.5f), color29})
		End Function

		Private Function decodeGradient9(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.1729452f,0.3458904f,0.49315068f,0.64041096f,0.7328767f,0.8253425f,0.9126712f,1.0f }, New Color() { color30, decodeColor(color30,color31,0.5f), color31, decodeColor(color31,color32,0.5f), color32, decodeColor(color32,color33,0.5f), color33, decodeColor(color33,color34,0.5f), color34})
		End Function

		Private Function decodeGradient10(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color35, decodeColor(color35,color36,0.5f), color36})
		End Function


	End Class

End Namespace
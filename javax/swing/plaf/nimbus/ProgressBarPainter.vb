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



	Friend NotInheritable Class ProgressBarPainter
		Inherits AbstractRegionPainter

		'package private integers representing the available states that
		'this painter will paint. These are used when creating a new instance
		'of ProgressBarPainter to determine which region/state is being painted
		'by that instance.
		Friend Const BACKGROUND_ENABLED As Integer = 1
		Friend Const BACKGROUND_DISABLED As Integer = 2
		Friend Const FOREGROUND_ENABLED As Integer = 3
		Friend Const FOREGROUND_ENABLED_FINISHED As Integer = 4
		Friend Const FOREGROUND_ENABLED_INDETERMINATE As Integer = 5
		Friend Const FOREGROUND_DISABLED As Integer = 6
		Friend Const FOREGROUND_DISABLED_FINISHED As Integer = 7
		Friend Const FOREGROUND_DISABLED_INDETERMINATE As Integer = 8


		Private state As Integer 'refers to one of the static final ints above
		Private ctx As PaintContext

		'the following 4 variables are reused during the painting code of the layers
		Private path As Path2D = New Path2D.Float
		Private rect As Rectangle2D = New Rectangle2D.Float(0, 0, 0, 0)
		Private roundRect As RoundRectangle2D = New RoundRectangle2D.Float(0, 0, 0, 0, 0, 0)
		Private ellipse As Ellipse2D = New Ellipse2D.Float(0, 0, 0, 0)

		'All Colors used for painting are stored here. Ideally, only those colors being used
		'by a particular instance of ProgressBarPainter would be created. For the moment at least,
		'however, all are created for each instance.
		Private color1 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.04845735f, -0.17647058f, 0)
		Private color2 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.061345987f, -0.027450979f, 0)
		Private color3 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.110526316f, 0.25490195f, 0)
		Private color4 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.097921275f, 0.18823528f, 0)
		Private color5 As Color = decodeColor("nimbusBlueGrey", 0.0138888955f, -0.0925083f, 0.12549019f, 0)
		Private color6 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.08222443f, 0.086274505f, 0)
		Private color7 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.08477524f, 0.16862744f, 0)
		Private color8 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.086996906f, 0.25490195f, 0)
		Private color9 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.061613273f, -0.02352941f, 0)
		Private color10 As Color = decodeColor("nimbusBlueGrey", -0.01111114f, -0.061265234f, 0.05098039f, 0)
		Private color11 As Color = decodeColor("nimbusBlueGrey", 0.0138888955f, -0.09378991f, 0.19215685f, 0)
		Private color12 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.08455229f, 0.1607843f, 0)
		Private color13 As Color = decodeColor("nimbusBlueGrey", -0.027777791f, -0.08362049f, 0.12941176f, 0)
		Private color14 As Color = decodeColor("nimbusBlueGrey", 0.007936537f, -0.07826825f, 0.10588235f, 0)
		Private color15 As Color = decodeColor("nimbusBlueGrey", 0.007936537f, -0.07982456f, 0.1490196f, 0)
		Private color16 As Color = decodeColor("nimbusBlueGrey", 0.007936537f, -0.08099045f, 0.18431371f, 0)
		Private color17 As Color = decodeColor("nimbusOrange", 0.0f, 0.0f, 0.0f, -156)
		Private color18 As Color = decodeColor("nimbusOrange", -0.015796512f, 0.02094239f, -0.15294117f, 0)
		Private color19 As Color = decodeColor("nimbusOrange", -0.004321605f, 0.02094239f, -0.0745098f, 0)
		Private color20 As Color = decodeColor("nimbusOrange", -0.008021399f, 0.02094239f, -0.10196078f, 0)
		Private color21 As Color = decodeColor("nimbusOrange", -0.011706904f, -0.1790576f, -0.02352941f, 0)
		Private color22 As Color = decodeColor("nimbusOrange", -0.048691254f, 0.02094239f, -0.3019608f, 0)
		Private color23 As Color = decodeColor("nimbusOrange", 0.003940329f, -0.7375322f, 0.17647058f, 0)
		Private color24 As Color = decodeColor("nimbusOrange", 0.005506739f, -0.46764207f, 0.109803915f, 0)
		Private color25 As Color = decodeColor("nimbusOrange", 0.0042127445f, -0.18595415f, 0.04705882f, 0)
		Private color26 As Color = decodeColor("nimbusOrange", 0.0047626942f, 0.02094239f, 0.0039215684f, 0)
		Private color27 As Color = decodeColor("nimbusOrange", 0.0047626942f, -0.15147138f, 0.1607843f, 0)
		Private color28 As Color = decodeColor("nimbusOrange", 0.010665476f, -0.27317524f, 0.25098038f, 0)
		Private color29 As Color = decodeColor("nimbusBlueGrey", -0.54444444f, -0.08748484f, 0.10588235f, 0)
		Private color30 As Color = decodeColor("nimbusOrange", 0.0047626942f, -0.21715283f, 0.23921567f, 0)
		Private color31 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.110526316f, 0.25490195f, -173)
		Private color32 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.110526316f, 0.25490195f, -170)
		Private color33 As Color = decodeColor("nimbusOrange", 0.024554357f, -0.8873145f, 0.10588235f, -156)
		Private color34 As Color = decodeColor("nimbusOrange", -0.023593787f, -0.7963165f, 0.02352941f, 0)
		Private color35 As Color = decodeColor("nimbusOrange", -0.010608241f, -0.7760873f, 0.043137252f, 0)
		Private color36 As Color = decodeColor("nimbusOrange", -0.015402906f, -0.7840576f, 0.035294116f, 0)
		Private color37 As Color = decodeColor("nimbusOrange", -0.017112307f, -0.8091547f, 0.058823526f, 0)
		Private color38 As Color = decodeColor("nimbusOrange", -0.07044564f, -0.844649f, -0.019607842f, 0)
		Private color39 As Color = decodeColor("nimbusOrange", -0.009704903f, -0.9381485f, 0.11372548f, 0)
		Private color40 As Color = decodeColor("nimbusOrange", -4.4563413E-4f, -0.86742973f, 0.09411764f, 0)
		Private color41 As Color = decodeColor("nimbusOrange", -4.4563413E-4f, -0.79896283f, 0.07843137f, 0)
		Private color42 As Color = decodeColor("nimbusOrange", 0.0013274103f, -0.7530961f, 0.06666666f, 0)
		Private color43 As Color = decodeColor("nimbusOrange", 0.0013274103f, -0.7644457f, 0.109803915f, 0)
		Private color44 As Color = decodeColor("nimbusOrange", 0.009244293f, -0.78794646f, 0.13333333f, 0)
		Private color45 As Color = decodeColor("nimbusBlueGrey", -0.015872955f, -0.0803539f, 0.16470587f, 0)
		Private color46 As Color = decodeColor("nimbusBlueGrey", 0.007936537f, -0.07968931f, 0.14509803f, 0)
		Private color47 As Color = decodeColor("nimbusBlueGrey", 0.02222228f, -0.08779904f, 0.11764705f, 0)
		Private color48 As Color = decodeColor("nimbusBlueGrey", 0.0138888955f, -0.075128086f, 0.14117646f, 0)
		Private color49 As Color = decodeColor("nimbusBlueGrey", 0.0138888955f, -0.07604356f, 0.16470587f, 0)
		Private color50 As Color = decodeColor("nimbusOrange", 0.0014062226f, -0.77816474f, 0.12941176f, 0)


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
				Case FOREGROUND_ENABLED
					paintForegroundEnabled(g)
				Case FOREGROUND_ENABLED_FINISHED
					paintForegroundEnabledAndFinished(g)
				Case FOREGROUND_ENABLED_INDETERMINATE
					paintForegroundEnabledAndIndeterminate(g)
				Case FOREGROUND_DISABLED
					paintForegroundDisabled(g)
				Case FOREGROUND_DISABLED_FINISHED
					paintForegroundDisabledAndFinished(g)
				Case FOREGROUND_DISABLED_INDETERMINATE
					paintForegroundDisabledAndIndeterminate(g)

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
			rect = decodeRect2()
			g.paint = decodeGradient2(rect)
			g.fill(rect)

		End Sub

		Private Sub paintBackgroundDisabled(ByVal g As Graphics2D)
			rect = decodeRect1()
			g.paint = decodeGradient3(rect)
			g.fill(rect)
			rect = decodeRect2()
			g.paint = decodeGradient4(rect)
			g.fill(rect)

		End Sub

		Private Sub paintForegroundEnabled(ByVal g As Graphics2D)
			path = decodePath1()
			g.paint = color17
			g.fill(path)
			rect = decodeRect3()
			g.paint = decodeGradient5(rect)
			g.fill(rect)
			rect = decodeRect4()
			g.paint = decodeGradient6(rect)
			g.fill(rect)

		End Sub

		Private Sub paintForegroundEnabledAndFinished(ByVal g As Graphics2D)
			path = decodePath2()
			g.paint = color17
			g.fill(path)
			rect = decodeRect1()
			g.paint = decodeGradient5(rect)
			g.fill(rect)
			rect = decodeRect2()
			g.paint = decodeGradient6(rect)
			g.fill(rect)

		End Sub

		Private Sub paintForegroundEnabledAndIndeterminate(ByVal g As Graphics2D)
			rect = decodeRect5()
			g.paint = decodeGradient7(rect)
			g.fill(rect)
			path = decodePath3()
			g.paint = decodeGradient8(path)
			g.fill(path)
			rect = decodeRect6()
			g.paint = color31
			g.fill(rect)
			rect = decodeRect7()
			g.paint = color32
			g.fill(rect)

		End Sub

		Private Sub paintForegroundDisabled(ByVal g As Graphics2D)
			path = decodePath1()
			g.paint = color33
			g.fill(path)
			rect = decodeRect3()
			g.paint = decodeGradient9(rect)
			g.fill(rect)
			rect = decodeRect4()
			g.paint = decodeGradient10(rect)
			g.fill(rect)

		End Sub

		Private Sub paintForegroundDisabledAndFinished(ByVal g As Graphics2D)
			path = decodePath4()
			g.paint = color33
			g.fill(path)
			rect = decodeRect1()
			g.paint = decodeGradient9(rect)
			g.fill(rect)
			rect = decodeRect2()
			g.paint = decodeGradient10(rect)
			g.fill(rect)

		End Sub

		Private Sub paintForegroundDisabledAndIndeterminate(ByVal g As Graphics2D)
			rect = decodeRect5()
			g.paint = decodeGradient11(rect)
			g.fill(rect)
			path = decodePath5()
			g.paint = decodeGradient12(path)
			g.fill(path)

		End Sub



		Private Function decodeRect1() As Rectangle2D
				rect.rectect(decodeX(0.4f), decodeY(0.4f), decodeX(2.6f) - decodeX(0.4f), decodeY(2.6f) - decodeY(0.4f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodeRect2() As Rectangle2D
				rect.rectect(decodeX(0.6f), decodeY(0.6f), decodeX(2.4f) - decodeX(0.6f), decodeY(2.4f) - decodeY(0.6f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodePath1() As Path2D
			path.reset()
			path.moveTo(decodeX(0.6f), decodeY(0.12666667f))
			path.curveTo(decodeAnchorX(0.6000000238418579f, -2.0f), decodeAnchorY(0.12666666507720947f, 0.0f), decodeAnchorX(0.12666666507720947f, 0.0f), decodeAnchorY(0.6000000238418579f, -2.0f), decodeX(0.12666667f), decodeY(0.6f))
			path.curveTo(decodeAnchorX(0.12666666507720947f, 0.0f), decodeAnchorY(0.6000000238418579f, 2.0f), decodeAnchorX(0.12666666507720947f, 0.0f), decodeAnchorY(2.4000000953674316f, -2.0f), decodeX(0.12666667f), decodeY(2.4f))
			path.curveTo(decodeAnchorX(0.12666666507720947f, 0.0f), decodeAnchorY(2.4000000953674316f, 2.0f), decodeAnchorX(0.6000000238418579f, -2.0f), decodeAnchorY(2.8933334350585938f, 0.0f), decodeX(0.6f), decodeY(2.8933334f))
			path.curveTo(decodeAnchorX(0.6000000238418579f, 2.0f), decodeAnchorY(2.8933334350585938f, 0.0f), decodeAnchorX(3.0f, 0.0f), decodeAnchorY(2.8933334350585938f, 0.0f), decodeX(3.0f), decodeY(2.8933334f))
			path.lineTo(decodeX(3.0f), decodeY(2.6f))
			path.lineTo(decodeX(0.4f), decodeY(2.6f))
			path.lineTo(decodeX(0.4f), decodeY(0.4f))
			path.lineTo(decodeX(3.0f), decodeY(0.4f))
			path.lineTo(decodeX(3.0f), decodeY(0.120000005f))
			path.curveTo(decodeAnchorX(3.0f, 0.0f), decodeAnchorY(0.12000000476837158f, 0.0f), decodeAnchorX(0.6000000238418579f, 2.0f), decodeAnchorY(0.12666666507720947f, 0.0f), decodeX(0.6f), decodeY(0.12666667f))
			path.closePath()
			Return path
		End Function

		Private Function decodeRect3() As Rectangle2D
				rect.rectect(decodeX(0.4f), decodeY(0.4f), decodeX(3.0f) - decodeX(0.4f), decodeY(2.6f) - decodeY(0.4f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodeRect4() As Rectangle2D
				rect.rectect(decodeX(0.6f), decodeY(0.6f), decodeX(2.8f) - decodeX(0.6f), decodeY(2.4f) - decodeY(0.6f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodePath2() As Path2D
			path.reset()
			path.moveTo(decodeX(0.5466667f), decodeY(0.12666667f))
			path.curveTo(decodeAnchorX(0.54666668176651f, -2.000000000000001f), decodeAnchorY(0.12666666507720947f, 0.0f), decodeAnchorX(0.12000000476837158f, 0.0f), decodeAnchorY(0.6066666841506958f, -1.9999999999999998f), decodeX(0.120000005f), decodeY(0.6066667f))
			path.lineTo(decodeX(0.120000005f), decodeY(2.4266667f))
			path.curveTo(decodeAnchorX(0.12000000476837158f, 0.0f), decodeAnchorY(2.426666736602783f, 2.0f), decodeAnchorX(0.5800000429153442f, -2.0f), decodeAnchorY(2.879999876022339f, 0.0f), decodeX(0.58000004f), decodeY(2.8799999f))
			path.lineTo(decodeX(2.4f), decodeY(2.8733335f))
			path.curveTo(decodeAnchorX(2.4000000953674316f, 1.9709292441265305f), decodeAnchorY(2.87333345413208f, 0.019857039365145823f), decodeAnchorX(2.866666793823242f, -0.03333333333333499f), decodeAnchorY(2.433333158493042f, 1.9333333333333869f), decodeX(2.8666668f), decodeY(2.4333332f))
			path.lineTo(decodeX(2.8733335f), decodeY(1.9407407f))
			path.lineTo(decodeX(2.8666668f), decodeY(1.1814815f))
			path.lineTo(decodeX(2.8666668f), decodeY(0.6066667f))
			path.curveTo(decodeAnchorX(2.866666793823242f, 0.0042173304174148996f), decodeAnchorY(0.6066666841506958f, -1.9503377583381705f), decodeAnchorX(2.4599997997283936f, 1.9659460194139413f), decodeAnchorY(0.13333334028720856f, 0.017122267221350018f), decodeX(2.4599998f), decodeY(0.13333334f))
			path.lineTo(decodeX(0.5466667f), decodeY(0.12666667f))
			path.closePath()
			Return path
		End Function

		Private Function decodeRect5() As Rectangle2D
				rect.rectect(decodeX(0.0f), decodeY(0.0f), decodeX(3.0f) - decodeX(0.0f), decodeY(3.0f) - decodeY(0.0f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodePath3() As Path2D
			path.reset()
			path.moveTo(decodeX(0.0f), decodeY(1.3333334f))
			path.curveTo(decodeAnchorX(0.0f, 2.678571428571433f), decodeAnchorY(1.3333333730697632f, 8.881784197001252E-16f), decodeAnchorX(1.3678570985794067f, -6.214285714285715f), decodeAnchorY(0.20714285969734192f, -0.03571428571428292f), decodeX(1.3678571f), decodeY(0.20714286f))
			path.lineTo(decodeX(1.5642858f), decodeY(0.20714286f))
			path.curveTo(decodeAnchorX(1.5642857551574707f, 8.329670329670357f), decodeAnchorY(0.20714285969734192f, 0.002747252747249629f), decodeAnchorX(2.5999999046325684f, -5.2857142857142705f), decodeAnchorY(1.3333333730697632f, 0.03571428571428559f), decodeX(2.6f), decodeY(1.3333334f))
			path.lineTo(decodeX(3.0f), decodeY(1.3333334f))
			path.lineTo(decodeX(3.0f), decodeY(1.6666667f))
			path.lineTo(decodeX(2.6f), decodeY(1.6666667f))
			path.curveTo(decodeAnchorX(2.5999999046325684f, -5.321428571428569f), decodeAnchorY(1.6666667461395264f, 0.0357142857142847f), decodeAnchorX(1.5642857551574707f, 8.983516483516496f), decodeAnchorY(2.799999952316284f, 0.03846153846153122f), decodeX(1.5642858f), decodeY(2.8f))
			path.lineTo(decodeX(1.3892857f), decodeY(2.8f))
			path.curveTo(decodeAnchorX(1.389285683631897f, -6.714285714285704f), decodeAnchorY(2.799999952316284f, 0.0f), decodeAnchorX(0.0f, 2.6071428571428568f), decodeAnchorY(1.6666667461395264f, 0.03571428571428559f), decodeX(0.0f), decodeY(1.6666667f))
			path.lineTo(decodeX(0.0f), decodeY(1.3333334f))
			path.closePath()
			Return path
		End Function

		Private Function decodeRect6() As Rectangle2D
				rect.rectect(decodeX(1.25f), decodeY(0.0f), decodeX(1.3f) - decodeX(1.25f), decodeY(3.0f) - decodeY(0.0f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodeRect7() As Rectangle2D
				rect.rectect(decodeX(1.75f), decodeY(0.0f), decodeX(1.8f) - decodeX(1.75f), decodeY(3.0f) - decodeY(0.0f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodePath4() As Path2D
			path.reset()
			path.moveTo(decodeX(0.59333336f), decodeY(0.120000005f))
			path.curveTo(decodeAnchorX(0.59333336353302f, -1.9999999999999993f), decodeAnchorY(0.12000000476837158f, 0.0f), decodeAnchorX(0.12000000476837158f, 0.0f), decodeAnchorY(0.59333336353302f, -2.000000000000001f), decodeX(0.120000005f), decodeY(0.59333336f))
			path.curveTo(decodeAnchorX(0.12000000476837158f, 0.0f), decodeAnchorY(0.59333336353302f, 1.9999999999999991f), decodeAnchorX(0.12000000476837158f, 0.0f), decodeAnchorY(2.3933331966400146f, -2.0000000000000053f), decodeX(0.120000005f), decodeY(2.3933332f))
			path.curveTo(decodeAnchorX(0.12000000476837158f, 0.0f), decodeAnchorY(2.3933331966400146f, 2.000000000000007f), decodeAnchorX(0.59333336353302f, -1.9999999999999993f), decodeAnchorY(2.8866665363311768f, 0.0f), decodeX(0.59333336f), decodeY(2.8866665f))
			path.curveTo(decodeAnchorX(0.59333336353302f, 2.000000000000003f), decodeAnchorY(2.8866665363311768f, 0.0f), decodeAnchorX(2.700000047683716f, 0.0f), decodeAnchorY(2.879999876022339f, 0.0f), decodeX(2.7f), decodeY(2.8799999f))
			path.lineTo(decodeX(2.8466668f), decodeY(2.6933334f))
			path.lineTo(decodeX(2.8533332f), decodeY(1.6148149f))
			path.lineTo(decodeX(2.86f), decodeY(1.4074074f))
			path.lineTo(decodeX(2.86f), decodeY(0.37333333f))
			path.lineTo(decodeX(2.7599998f), decodeY(0.13333334f))
			path.curveTo(decodeAnchorX(2.7599997520446777f, 0.0f), decodeAnchorY(0.13333334028720856f, 0.0f), decodeAnchorX(0.59333336353302f, 2.000000000000003f), decodeAnchorY(0.12000000476837158f, 0.0f), decodeX(0.59333336f), decodeY(0.120000005f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath5() As Path2D
			path.reset()
			path.moveTo(decodeX(0.0f), decodeY(1.3333334f))
			path.curveTo(decodeAnchorX(0.0f, 2.678571428571433f), decodeAnchorY(1.3333333730697632f, 8.881784197001252E-16f), decodeAnchorX(1.3678570985794067f, -6.357142857142872f), decodeAnchorY(0.20714285969734192f, -0.03571428571428337f), decodeX(1.3678571f), decodeY(0.20714286f))
			path.lineTo(decodeX(1.5642858f), decodeY(0.20714286f))
			path.curveTo(decodeAnchorX(1.5642857551574707f, 3.9999999999999964f), decodeAnchorY(0.20714285969734192f, 0.0f), decodeAnchorX(2.5999999046325684f, -5.2857142857142705f), decodeAnchorY(1.3333333730697632f, 0.03571428571428559f), decodeX(2.6f), decodeY(1.3333334f))
			path.lineTo(decodeX(3.0f), decodeY(1.3333334f))
			path.lineTo(decodeX(3.0f), decodeY(1.6666667f))
			path.lineTo(decodeX(2.6f), decodeY(1.6666667f))
			path.curveTo(decodeAnchorX(2.5999999046325684f, -5.321428571428569f), decodeAnchorY(1.6666667461395264f, 0.0357142857142847f), decodeAnchorX(1.5642857551574707f, 3.999999999999986f), decodeAnchorY(2.799999952316284f, 0.0f), decodeX(1.5642858f), decodeY(2.8f))
			path.lineTo(decodeX(1.3892857f), decodeY(2.8f))
			path.curveTo(decodeAnchorX(1.389285683631897f, -6.571428571428584f), decodeAnchorY(2.799999952316284f, -0.035714285714286476f), decodeAnchorX(0.0f, 2.6071428571428568f), decodeAnchorY(1.6666667461395264f, 0.03571428571428559f), decodeX(0.0f), decodeY(1.6666667f))
			path.lineTo(decodeX(0.0f), decodeY(1.3333334f))
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
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.038709678f,0.05967742f,0.08064516f,0.23709677f,0.3935484f,0.41612905f,0.43870968f,0.67419356f,0.90967745f,0.91451615f,0.91935486f }, New Color() { color3, decodeColor(color3,color4,0.5f), color4, decodeColor(color4,color5,0.5f), color5, decodeColor(color5,color6,0.5f), color6, decodeColor(color6,color7,0.5f), color7, decodeColor(color7,color8,0.5f), color8})
		End Function

		Private Function decodeGradient3(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.05483871f,0.5032258f,0.9516129f }, New Color() { color9, decodeColor(color9,color10,0.5f), color10})
		End Function

		Private Function decodeGradient4(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.038709678f,0.05967742f,0.08064516f,0.23709677f,0.3935484f,0.41612905f,0.43870968f,0.67419356f,0.90967745f,0.91612905f,0.92258066f }, New Color() { color11, decodeColor(color11,color12,0.5f), color12, decodeColor(color12,color13,0.5f), color13, decodeColor(color13,color14,0.5f), color14, decodeColor(color14,color15,0.5f), color15, decodeColor(color15,color16,0.5f), color16})
		End Function

		Private Function decodeGradient5(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.038709678f,0.05483871f,0.07096774f,0.28064516f,0.4903226f,0.6967742f,0.9032258f,0.9241935f,0.9451613f }, New Color() { color18, decodeColor(color18,color19,0.5f), color19, decodeColor(color19,color20,0.5f), color20, decodeColor(color20,color21,0.5f), color21, decodeColor(color21,color22,0.5f), color22})
		End Function

		Private Function decodeGradient6(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.038709678f,0.061290324f,0.08387097f,0.27258065f,0.46129033f,0.4903226f,0.5193548f,0.71774197f,0.91612905f,0.92419356f,0.93225807f }, New Color() { color23, decodeColor(color23,color24,0.5f), color24, decodeColor(color24,color25,0.5f), color25, decodeColor(color25,color26,0.5f), color26, decodeColor(color26,color27,0.5f), color27, decodeColor(color27,color28,0.5f), color28})
		End Function

		Private Function decodeGradient7(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.051612902f,0.06612903f,0.08064516f,0.2935484f,0.5064516f,0.6903226f,0.87419355f,0.88870966f,0.9032258f }, New Color() { color3, decodeColor(color3,color4,0.5f), color4, decodeColor(color4,color29,0.5f), color29, decodeColor(color29,color7,0.5f), color7, decodeColor(color7,color8,0.5f), color8})
		End Function

		Private Function decodeGradient8(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.20645161f,0.41290322f,0.44193548f,0.47096774f,0.7354839f,1.0f }, New Color() { color24, decodeColor(color24,color25,0.5f), color25, decodeColor(color25,color26,0.5f), color26, decodeColor(color26,color30,0.5f), color30})
		End Function

		Private Function decodeGradient9(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.038709678f,0.05483871f,0.07096774f,0.28064516f,0.4903226f,0.6967742f,0.9032258f,0.9241935f,0.9451613f }, New Color() { color34, decodeColor(color34,color35,0.5f), color35, decodeColor(color35,color36,0.5f), color36, decodeColor(color36,color37,0.5f), color37, decodeColor(color37,color38,0.5f), color38})
		End Function

		Private Function decodeGradient10(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.038709678f,0.061290324f,0.08387097f,0.27258065f,0.46129033f,0.4903226f,0.5193548f,0.71774197f,0.91612905f,0.92419356f,0.93225807f }, New Color() { color39, decodeColor(color39,color40,0.5f), color40, decodeColor(color40,color41,0.5f), color41, decodeColor(color41,color42,0.5f), color42, decodeColor(color42,color43,0.5f), color43, decodeColor(color43,color44,0.5f), color44})
		End Function

		Private Function decodeGradient11(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.051612902f,0.06612903f,0.08064516f,0.2935484f,0.5064516f,0.6903226f,0.87419355f,0.88870966f,0.9032258f }, New Color() { color45, decodeColor(color45,color46,0.5f), color46, decodeColor(color46,color47,0.5f), color47, decodeColor(color47,color48,0.5f), color48, decodeColor(color48,color49,0.5f), color49})
		End Function

		Private Function decodeGradient12(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.20645161f,0.41290322f,0.44193548f,0.47096774f,0.7354839f,1.0f }, New Color() { color40, decodeColor(color40,color41,0.5f), color41, decodeColor(color41,color42,0.5f), color42, decodeColor(color42,color50,0.5f), color50})
		End Function


	End Class

End Namespace
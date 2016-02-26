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



	Friend NotInheritable Class RadioButtonPainter
		Inherits AbstractRegionPainter

		'package private integers representing the available states that
		'this painter will paint. These are used when creating a new instance
		'of RadioButtonPainter to determine which region/state is being painted
		'by that instance.
		Friend Const BACKGROUND_DISABLED As Integer = 1
		Friend Const BACKGROUND_ENABLED As Integer = 2
		Friend Const ICON_DISABLED As Integer = 3
		Friend Const ICON_ENABLED As Integer = 4
		Friend Const ICON_FOCUSED As Integer = 5
		Friend Const ICON_MOUSEOVER As Integer = 6
		Friend Const ICON_MOUSEOVER_FOCUSED As Integer = 7
		Friend Const ICON_PRESSED As Integer = 8
		Friend Const ICON_PRESSED_FOCUSED As Integer = 9
		Friend Const ICON_SELECTED As Integer = 10
		Friend Const ICON_SELECTED_FOCUSED As Integer = 11
		Friend Const ICON_PRESSED_SELECTED As Integer = 12
		Friend Const ICON_PRESSED_SELECTED_FOCUSED As Integer = 13
		Friend Const ICON_MOUSEOVER_SELECTED As Integer = 14
		Friend Const ICON_MOUSEOVER_SELECTED_FOCUSED As Integer = 15
		Friend Const ICON_DISABLED_SELECTED As Integer = 16


		Private state As Integer 'refers to one of the static final ints above
		Private ctx As PaintContext

		'the following 4 variables are reused during the painting code of the layers
		Private path As Path2D = New Path2D.Float
		Private rect As Rectangle2D = New Rectangle2D.Float(0, 0, 0, 0)
		Private roundRect As RoundRectangle2D = New RoundRectangle2D.Float(0, 0, 0, 0, 0, 0)
		Private ellipse As Ellipse2D = New Ellipse2D.Float(0, 0, 0, 0)

		'All Colors used for painting are stored here. Ideally, only those colors being used
		'by a particular instance of RadioButtonPainter would be created. For the moment at least,
		'however, all are created for each instance.
		Private color1 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.06766917f, 0.07843137f, 0)
		Private color2 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.06413457f, 0.015686274f, 0)
		Private color3 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.08466425f, 0.16470587f, 0)
		Private color4 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.07016757f, 0.12941176f, 0)
		Private color5 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.070703305f, 0.14117646f, 0)
		Private color6 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.07052632f, 0.1372549f, 0)
		Private color7 As Color = decodeColor("nimbusBlueGrey", 0.0f, 0.0f, 0.0f, -112)
		Private color8 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.053201474f, -0.12941176f, 0)
		Private color9 As Color = decodeColor("nimbusBlueGrey", 0.0f, 0.006356798f, -0.44313726f, 0)
		Private color10 As Color = decodeColor("nimbusBlueGrey", 0.055555582f, -0.10654225f, 0.23921567f, 0)
		Private color11 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.07206477f, 0.17254901f, 0)
		Private color12 As Color = decodeColor("nimbusFocus", 0.0f, 0.0f, 0.0f, 0)
		Private color13 As Color = decodeColor("nimbusBlueGrey", -0.00505054f, -0.027819552f, -0.2235294f, 0)
		Private color14 As Color = decodeColor("nimbusBlueGrey", 0.0f, 0.24241486f, -0.6117647f, 0)
		Private color15 As Color = decodeColor("nimbusBlueGrey", -0.111111104f, -0.10655806f, 0.24313724f, 0)
		Private color16 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.07333623f, 0.20392156f, 0)
		Private color17 As Color = decodeColor("nimbusBlueGrey", 0.08585858f, -0.067389056f, 0.25490195f, 0)
		Private color18 As Color = decodeColor("nimbusBlueGrey", -0.111111104f, -0.10628903f, 0.18039215f, 0)
		Private color19 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.110526316f, 0.25490195f, 0)
		Private color20 As Color = decodeColor("nimbusBlueGrey", 0.055555582f, 0.23947367f, -0.6666667f, 0)
		Private color21 As Color = decodeColor("nimbusBlueGrey", -0.0777778f, -0.06815343f, -0.28235295f, 0)
		Private color22 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.06866585f, 0.09803921f, 0)
		Private color23 As Color = decodeColor("nimbusBlueGrey", -0.0027777553f, -0.0018306673f, -0.02352941f, 0)
		Private color24 As Color = decodeColor("nimbusBlueGrey", 0.002924025f, -0.02047892f, 0.082352936f, 0)
		Private color25 As Color = decodeColor("nimbusBase", 2.9569864E-4f, -0.36035198f, -0.007843137f, 0)
		Private color26 As Color = decodeColor("nimbusBase", 2.9569864E-4f, 0.019458115f, -0.32156867f, 0)
		Private color27 As Color = decodeColor("nimbusBase", 0.004681647f, -0.6195853f, 0.4235294f, 0)
		Private color28 As Color = decodeColor("nimbusBase", 0.004681647f, -0.56704473f, 0.36470586f, 0)
		Private color29 As Color = decodeColor("nimbusBase", 5.1498413E-4f, -0.43866998f, 0.24705881f, 0)
		Private color30 As Color = decodeColor("nimbusBase", 5.1498413E-4f, -0.44879842f, 0.29019606f, 0)
		Private color31 As Color = decodeColor("nimbusBlueGrey", -0.027777791f, -0.07243107f, -0.33333334f, 0)
		Private color32 As Color = decodeColor("nimbusBlueGrey", -0.6111111f, -0.110526316f, -0.74509805f, 0)
		Private color33 As Color = decodeColor("nimbusBlueGrey", -0.027777791f, 0.07129187f, -0.6156863f, 0)
		Private color34 As Color = decodeColor("nimbusBase", -0.57865167f, -0.6357143f, -0.49803925f, 0)
		Private color35 As Color = decodeColor("nimbusBase", 0.0030477047f, -0.1257143f, -0.15686277f, 0)
		Private color36 As Color = decodeColor("nimbusBase", -0.0017285943f, -0.4367347f, 0.21960783f, 0)
		Private color37 As Color = decodeColor("nimbusBase", -0.0010654926f, -0.31349206f, 0.15686274f, 0)
		Private color38 As Color = decodeColor("nimbusBase", 0.0f, 0.0f, 0.0f, 0)
		Private color39 As Color = decodeColor("nimbusBase", 8.05676E-4f, -0.12380952f, 0.109803915f, 0)
		Private color40 As Color = decodeColor("nimbusBlueGrey", -0.027777791f, -0.080223285f, -0.4862745f, 0)
		Private color41 As Color = decodeColor("nimbusBase", -6.374717E-4f, -0.20452163f, -0.12156865f, 0)
		Private color42 As Color = decodeColor("nimbusBase", -0.57865167f, -0.6357143f, -0.5058824f, 0)
		Private color43 As Color = decodeColor("nimbusBase", -0.011985004f, -0.6157143f, 0.43137252f, 0)
		Private color44 As Color = decodeColor("nimbusBase", 0.004681647f, -0.56932425f, 0.3960784f, 0)
		Private color45 As Color = decodeColor("nimbusBase", 5.1498413E-4f, -0.4555341f, 0.3215686f, 0)
		Private color46 As Color = decodeColor("nimbusBase", 5.1498413E-4f, -0.46550155f, 0.372549f, 0)
		Private color47 As Color = decodeColor("nimbusBase", 0.0024294257f, -0.47271872f, 0.34117645f, 0)
		Private color48 As Color = decodeColor("nimbusBase", 0.010237217f, -0.56289876f, 0.2588235f, 0)
		Private color49 As Color = decodeColor("nimbusBase", 0.016586483f, -0.5620301f, 0.19607842f, 0)
		Private color50 As Color = decodeColor("nimbusBase", 0.027408898f, -0.5878882f, 0.35294116f, 0)
		Private color51 As Color = decodeColor("nimbusBase", 0.021348298f, -0.56722116f, 0.3098039f, 0)
		Private color52 As Color = decodeColor("nimbusBase", 0.021348298f, -0.567841f, 0.31764704f, 0)
		Private color53 As Color = decodeColor("nimbusBlueGrey", -0.01111114f, -0.058170296f, 0.0039215684f, 0)
		Private color54 As Color = decodeColor("nimbusBlueGrey", -0.013888836f, -0.04195489f, -0.058823526f, 0)
		Private color55 As Color = decodeColor("nimbusBlueGrey", 0.009259284f, -0.0147816315f, -0.007843137f, 0)


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
				Case ICON_DISABLED
					painticonDisabled(g)
				Case ICON_ENABLED
					painticonEnabled(g)
				Case ICON_FOCUSED
					painticonFocused(g)
				Case ICON_MOUSEOVER
					painticonMouseOver(g)
				Case ICON_MOUSEOVER_FOCUSED
					painticonMouseOverAndFocused(g)
				Case ICON_PRESSED
					painticonPressed(g)
				Case ICON_PRESSED_FOCUSED
					painticonPressedAndFocused(g)
				Case ICON_SELECTED
					painticonSelected(g)
				Case ICON_SELECTED_FOCUSED
					painticonSelectedAndFocused(g)
				Case ICON_PRESSED_SELECTED
					painticonPressedAndSelected(g)
				Case ICON_PRESSED_SELECTED_FOCUSED
					painticonPressedAndSelectedAndFocused(g)
				Case ICON_MOUSEOVER_SELECTED
					painticonMouseOverAndSelected(g)
				Case ICON_MOUSEOVER_SELECTED_FOCUSED
					painticonMouseOverAndSelectedAndFocused(g)
				Case ICON_DISABLED_SELECTED
					painticonDisabledAndSelected(g)

			End Select
		End Sub



		Protected Friend Property NotOverridable Overrides paintContext As PaintContext
			Get
				Return ctx
			End Get
		End Property

		Private Sub painticonDisabled(ByVal g As Graphics2D)
			ellipse = decodeEllipse1()
			g.paint = decodeGradient1(ellipse)
			g.fill(ellipse)
			ellipse = decodeEllipse2()
			g.paint = decodeGradient2(ellipse)
			g.fill(ellipse)

		End Sub

		Private Sub painticonEnabled(ByVal g As Graphics2D)
			ellipse = decodeEllipse3()
			g.paint = color7
			g.fill(ellipse)
			ellipse = decodeEllipse1()
			g.paint = decodeGradient3(ellipse)
			g.fill(ellipse)
			ellipse = decodeEllipse2()
			g.paint = decodeGradient4(ellipse)
			g.fill(ellipse)

		End Sub

		Private Sub painticonFocused(ByVal g As Graphics2D)
			ellipse = decodeEllipse4()
			g.paint = color12
			g.fill(ellipse)
			ellipse = decodeEllipse1()
			g.paint = decodeGradient3(ellipse)
			g.fill(ellipse)
			ellipse = decodeEllipse2()
			g.paint = decodeGradient4(ellipse)
			g.fill(ellipse)

		End Sub

		Private Sub painticonMouseOver(ByVal g As Graphics2D)
			ellipse = decodeEllipse3()
			g.paint = color7
			g.fill(ellipse)
			ellipse = decodeEllipse1()
			g.paint = decodeGradient5(ellipse)
			g.fill(ellipse)
			ellipse = decodeEllipse2()
			g.paint = decodeGradient6(ellipse)
			g.fill(ellipse)

		End Sub

		Private Sub painticonMouseOverAndFocused(ByVal g As Graphics2D)
			ellipse = decodeEllipse4()
			g.paint = color12
			g.fill(ellipse)
			ellipse = decodeEllipse1()
			g.paint = decodeGradient5(ellipse)
			g.fill(ellipse)
			ellipse = decodeEllipse2()
			g.paint = decodeGradient6(ellipse)
			g.fill(ellipse)

		End Sub

		Private Sub painticonPressed(ByVal g As Graphics2D)
			ellipse = decodeEllipse3()
			g.paint = color19
			g.fill(ellipse)
			ellipse = decodeEllipse1()
			g.paint = decodeGradient7(ellipse)
			g.fill(ellipse)
			ellipse = decodeEllipse2()
			g.paint = decodeGradient8(ellipse)
			g.fill(ellipse)

		End Sub

		Private Sub painticonPressedAndFocused(ByVal g As Graphics2D)
			ellipse = decodeEllipse4()
			g.paint = color12
			g.fill(ellipse)
			ellipse = decodeEllipse1()
			g.paint = decodeGradient7(ellipse)
			g.fill(ellipse)
			ellipse = decodeEllipse2()
			g.paint = decodeGradient8(ellipse)
			g.fill(ellipse)

		End Sub

		Private Sub painticonSelected(ByVal g As Graphics2D)
			ellipse = decodeEllipse3()
			g.paint = color7
			g.fill(ellipse)
			ellipse = decodeEllipse1()
			g.paint = decodeGradient9(ellipse)
			g.fill(ellipse)
			ellipse = decodeEllipse2()
			g.paint = decodeGradient10(ellipse)
			g.fill(ellipse)
			ellipse = decodeEllipse5()
			g.paint = decodeGradient11(ellipse)
			g.fill(ellipse)

		End Sub

		Private Sub painticonSelectedAndFocused(ByVal g As Graphics2D)
			ellipse = decodeEllipse4()
			g.paint = color12
			g.fill(ellipse)
			ellipse = decodeEllipse1()
			g.paint = decodeGradient9(ellipse)
			g.fill(ellipse)
			ellipse = decodeEllipse2()
			g.paint = decodeGradient10(ellipse)
			g.fill(ellipse)
			ellipse = decodeEllipse5()
			g.paint = decodeGradient11(ellipse)
			g.fill(ellipse)

		End Sub

		Private Sub painticonPressedAndSelected(ByVal g As Graphics2D)
			ellipse = decodeEllipse3()
			g.paint = color19
			g.fill(ellipse)
			ellipse = decodeEllipse1()
			g.paint = decodeGradient12(ellipse)
			g.fill(ellipse)
			ellipse = decodeEllipse2()
			g.paint = decodeGradient13(ellipse)
			g.fill(ellipse)
			ellipse = decodeEllipse5()
			g.paint = decodeGradient14(ellipse)
			g.fill(ellipse)

		End Sub

		Private Sub painticonPressedAndSelectedAndFocused(ByVal g As Graphics2D)
			ellipse = decodeEllipse4()
			g.paint = color12
			g.fill(ellipse)
			ellipse = decodeEllipse1()
			g.paint = decodeGradient12(ellipse)
			g.fill(ellipse)
			ellipse = decodeEllipse2()
			g.paint = decodeGradient13(ellipse)
			g.fill(ellipse)
			ellipse = decodeEllipse5()
			g.paint = decodeGradient14(ellipse)
			g.fill(ellipse)

		End Sub

		Private Sub painticonMouseOverAndSelected(ByVal g As Graphics2D)
			ellipse = decodeEllipse3()
			g.paint = color7
			g.fill(ellipse)
			ellipse = decodeEllipse1()
			g.paint = decodeGradient15(ellipse)
			g.fill(ellipse)
			ellipse = decodeEllipse2()
			g.paint = decodeGradient16(ellipse)
			g.fill(ellipse)
			ellipse = decodeEllipse5()
			g.paint = decodeGradient11(ellipse)
			g.fill(ellipse)

		End Sub

		Private Sub painticonMouseOverAndSelectedAndFocused(ByVal g As Graphics2D)
			ellipse = decodeEllipse4()
			g.paint = color12
			g.fill(ellipse)
			ellipse = decodeEllipse1()
			g.paint = decodeGradient15(ellipse)
			g.fill(ellipse)
			ellipse = decodeEllipse2()
			g.paint = decodeGradient16(ellipse)
			g.fill(ellipse)
			ellipse = decodeEllipse5()
			g.paint = decodeGradient11(ellipse)
			g.fill(ellipse)

		End Sub

		Private Sub painticonDisabledAndSelected(ByVal g As Graphics2D)
			ellipse = decodeEllipse1()
			g.paint = decodeGradient17(ellipse)
			g.fill(ellipse)
			ellipse = decodeEllipse2()
			g.paint = decodeGradient18(ellipse)
			g.fill(ellipse)
			ellipse = decodeEllipse5()
			g.paint = decodeGradient19(ellipse)
			g.fill(ellipse)

		End Sub



		Private Function decodeEllipse1() As Ellipse2D
			ellipse.frameame(decodeX(0.4f), decodeY(0.4f), decodeX(2.6f) - decodeX(0.4f), decodeY(2.6f) - decodeY(0.4f)) 'height - width - y - x
			Return ellipse
		End Function

		Private Function decodeEllipse2() As Ellipse2D
			ellipse.frameame(decodeX(0.6f), decodeY(0.6f), decodeX(2.4f) - decodeX(0.6f), decodeY(2.4f) - decodeY(0.6f)) 'height - width - y - x
			Return ellipse
		End Function

		Private Function decodeEllipse3() As Ellipse2D
			ellipse.frameame(decodeX(0.4f), decodeY(0.6f), decodeX(2.6f) - decodeX(0.4f), decodeY(2.8f) - decodeY(0.6f)) 'height - width - y - x
			Return ellipse
		End Function

		Private Function decodeEllipse4() As Ellipse2D
			ellipse.frameame(decodeX(0.120000005f), decodeY(0.120000005f), decodeX(2.8799999f) - decodeX(0.120000005f), decodeY(2.8799999f) - decodeY(0.120000005f)) 'height - width - y - x
			Return ellipse
		End Function

		Private Function decodeEllipse5() As Ellipse2D
			ellipse.frameame(decodeX(1.125f), decodeY(1.125f), decodeX(1.875f) - decodeX(1.125f), decodeY(1.875f) - decodeY(1.125f)) 'height - width - y - x
			Return ellipse
		End Function



		Private Function decodeGradient1(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.49789914f * w) + x, (-0.004201681f * h) + y, (0.5f * w) + x, (0.9978992f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color1, decodeColor(color1,color2,0.5f), color2})
		End Function

		Private Function decodeGradient2(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.49754903f * w) + x, (0.004901961f * h) + y, (0.50735295f * w) + x, (1.0f * h) + y, New Single() { 0.06344411f,0.21601209f,0.36858007f,0.54833835f,0.72809666f,0.77492446f,0.82175225f,0.91087615f,1.0f }, New Color() { color3, decodeColor(color3,color4,0.5f), color4, decodeColor(color4,color4,0.5f), color4, decodeColor(color4,color5,0.5f), color5, decodeColor(color5,color6,0.5f), color6})
		End Function

		Private Function decodeGradient3(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.49789914f * w) + x, (-0.004201681f * h) + y, (0.5f * w) + x, (0.9978992f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color8, decodeColor(color8,color9,0.5f), color9})
		End Function

		Private Function decodeGradient4(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.49754903f * w) + x, (0.004901961f * h) + y, (0.50735295f * w) + x, (1.0f * h) + y, New Single() { 0.06344411f,0.25009555f,0.43674698f,0.48042166f,0.52409637f,0.70481926f,0.88554215f }, New Color() { color10, decodeColor(color10,color4,0.5f), color4, decodeColor(color4,color4,0.5f), color4, decodeColor(color4,color11,0.5f), color11})
		End Function

		Private Function decodeGradient5(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.49789914f * w) + x, (-0.004201681f * h) + y, (0.5f * w) + x, (0.9978992f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color13, decodeColor(color13,color14,0.5f), color14})
		End Function

		Private Function decodeGradient6(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.49754903f * w) + x, (0.004901961f * h) + y, (0.50735295f * w) + x, (1.0f * h) + y, New Single() { 0.06344411f,0.21601209f,0.36858007f,0.54833835f,0.72809666f,0.77492446f,0.82175225f,0.91087615f,1.0f }, New Color() { color15, decodeColor(color15,color16,0.5f), color16, decodeColor(color16,color16,0.5f), color16, decodeColor(color16,color17,0.5f), color17, decodeColor(color17,color18,0.5f), color18})
		End Function

		Private Function decodeGradient7(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.49789914f * w) + x, (-0.004201681f * h) + y, (0.5f * w) + x, (0.9978992f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color20, decodeColor(color20,color21,0.5f), color21})
		End Function

		Private Function decodeGradient8(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.49754903f * w) + x, (0.004901961f * h) + y, (0.50735295f * w) + x, (1.0f * h) + y, New Single() { 0.06344411f,0.20792687f,0.35240963f,0.45030123f,0.5481928f,0.748494f,0.9487952f }, New Color() { color22, decodeColor(color22,color23,0.5f), color23, decodeColor(color23,color23,0.5f), color23, decodeColor(color23,color24,0.5f), color24})
		End Function

		Private Function decodeGradient9(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.49789914f * w) + x, (-0.004201681f * h) + y, (0.5f * w) + x, (0.9978992f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color25, decodeColor(color25,color26,0.5f), color26})
		End Function

		Private Function decodeGradient10(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.49754903f * w) + x, (0.004901961f * h) + y, (0.50735295f * w) + x, (1.0f * h) + y, New Single() { 0.0813253f,0.100903615f,0.12048193f,0.28915662f,0.45783132f,0.6159638f,0.77409637f,0.82981926f,0.88554215f }, New Color() { color27, decodeColor(color27,color28,0.5f), color28, decodeColor(color28,color29,0.5f), color29, decodeColor(color29,color29,0.5f), color29, decodeColor(color29,color30,0.5f), color30})
		End Function

		Private Function decodeGradient11(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.50490195f * w) + x, (0.0f * h) + y, (0.49509802f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.23192771f,0.46385542f,0.73192775f,1.0f }, New Color() { color31, decodeColor(color31,color32,0.5f), color32, decodeColor(color32,color33,0.5f), color33})
		End Function

		Private Function decodeGradient12(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.49789914f * w) + x, (-0.004201681f * h) + y, (0.5f * w) + x, (0.9978992f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color34, decodeColor(color34,color26,0.5f), color26})
		End Function

		Private Function decodeGradient13(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.49754903f * w) + x, (0.004901961f * h) + y, (0.50735295f * w) + x, (1.0f * h) + y, New Single() { 0.039156627f,0.07831325f,0.11746988f,0.2876506f,0.45783132f,0.56174695f,0.66566265f,0.7756024f,0.88554215f }, New Color() { color36, decodeColor(color36,color37,0.5f), color37, decodeColor(color37,color38,0.5f), color38, decodeColor(color38,color38,0.5f), color38, decodeColor(color38,color39,0.5f), color39})
		End Function

		Private Function decodeGradient14(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.50490195f * w) + x, (0.0f * h) + y, (0.49509802f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.23192771f,0.46385542f,0.73192775f,1.0f }, New Color() { color40, decodeColor(color40,color32,0.5f), color32, decodeColor(color32,color33,0.5f), color33})
		End Function

		Private Function decodeGradient15(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.49789914f * w) + x, (-0.004201681f * h) + y, (0.5f * w) + x, (0.9978992f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color41, decodeColor(color41,color42,0.5f), color42})
		End Function

		Private Function decodeGradient16(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.49754903f * w) + x, (0.004901961f * h) + y, (0.50735295f * w) + x, (1.0f * h) + y, New Single() { 0.0813253f,0.100903615f,0.12048193f,0.20180723f,0.28313252f,0.49246985f,0.7018072f,0.7560241f,0.810241f,0.84789157f,0.88554215f }, New Color() { color43, decodeColor(color43,color44,0.5f), color44, decodeColor(color44,color45,0.5f), color45, decodeColor(color45,color45,0.5f), color45, decodeColor(color45,color46,0.5f), color46, decodeColor(color46,color47,0.5f), color47})
		End Function

		Private Function decodeGradient17(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.49789914f * w) + x, (-0.004201681f * h) + y, (0.5f * w) + x, (0.9978992f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color48, decodeColor(color48,color49,0.5f), color49})
		End Function

		Private Function decodeGradient18(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.49754903f * w) + x, (0.004901961f * h) + y, (0.50735295f * w) + x, (1.0f * h) + y, New Single() { 0.0813253f,0.2695783f,0.45783132f,0.67168677f,0.88554215f }, New Color() { color50, decodeColor(color50,color51,0.5f), color51, decodeColor(color51,color52,0.5f), color52})
		End Function

		Private Function decodeGradient19(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.50490195f * w) + x, (0.0f * h) + y, (0.49509802f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.23192771f,0.46385542f,0.73192775f,1.0f }, New Color() { color53, decodeColor(color53,color54,0.5f), color54, decodeColor(color54,color55,0.5f), color55})
		End Function


	End Class

End Namespace
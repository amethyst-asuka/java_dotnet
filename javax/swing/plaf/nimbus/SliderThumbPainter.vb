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



	Friend NotInheritable Class SliderThumbPainter
		Inherits AbstractRegionPainter

		'package private integers representing the available states that
		'this painter will paint. These are used when creating a new instance
		'of SliderThumbPainter to determine which region/state is being painted
		'by that instance.
		Friend Const BACKGROUND_DISABLED As Integer = 1
		Friend Const BACKGROUND_ENABLED As Integer = 2
		Friend Const BACKGROUND_FOCUSED As Integer = 3
		Friend Const BACKGROUND_FOCUSED_MOUSEOVER As Integer = 4
		Friend Const BACKGROUND_FOCUSED_PRESSED As Integer = 5
		Friend Const BACKGROUND_MOUSEOVER As Integer = 6
		Friend Const BACKGROUND_PRESSED As Integer = 7
		Friend Const BACKGROUND_ENABLED_ARROWSHAPE As Integer = 8
		Friend Const BACKGROUND_DISABLED_ARROWSHAPE As Integer = 9
		Friend Const BACKGROUND_MOUSEOVER_ARROWSHAPE As Integer = 10
		Friend Const BACKGROUND_PRESSED_ARROWSHAPE As Integer = 11
		Friend Const BACKGROUND_FOCUSED_ARROWSHAPE As Integer = 12
		Friend Const BACKGROUND_FOCUSED_MOUSEOVER_ARROWSHAPE As Integer = 13
		Friend Const BACKGROUND_FOCUSED_PRESSED_ARROWSHAPE As Integer = 14


		Private state As Integer 'refers to one of the static final ints above
		Private ctx As PaintContext

		'the following 4 variables are reused during the painting code of the layers
		Private path As Path2D = New Path2D.Float
		Private rect As Rectangle2D = New Rectangle2D.Float(0, 0, 0, 0)
		Private roundRect As RoundRectangle2D = New RoundRectangle2D.Float(0, 0, 0, 0, 0, 0)
		Private ellipse As Ellipse2D = New Ellipse2D.Float(0, 0, 0, 0)

		'All Colors used for painting are stored here. Ideally, only those colors being used
		'by a particular instance of SliderThumbPainter would be created. For the moment at least,
		'however, all are created for each instance.
		Private color1 As Color = decodeColor("nimbusBase", 0.021348298f, -0.5625436f, 0.25490195f, 0)
		Private color2 As Color = decodeColor("nimbusBase", 0.015098333f, -0.55105823f, 0.19215685f, 0)
		Private color3 As Color = decodeColor("nimbusBase", 0.021348298f, -0.5924243f, 0.35686272f, 0)
		Private color4 As Color = decodeColor("nimbusBase", 0.021348298f, -0.56722116f, 0.3098039f, 0)
		Private color5 As Color = decodeColor("nimbusBase", 0.021348298f, -0.56844974f, 0.32549018f, 0)
		Private color6 As Color = decodeColor("nimbusBlueGrey", -0.003968239f, 0.0014736876f, -0.25490198f, -156)
		Private color7 As Color = decodeColor("nimbusBase", 5.1498413E-4f, -0.34585923f, -0.007843137f, 0)
		Private color8 As Color = decodeColor("nimbusBase", -0.0017285943f, -0.11571431f, -0.25490198f, 0)
		Private color9 As Color = decodeColor("nimbusBase", -0.023096085f, -0.6238095f, 0.43921566f, 0)
		Private color10 As Color = decodeColor("nimbusBase", 5.1498413E-4f, -0.43866998f, 0.24705881f, 0)
		Private color11 As Color = decodeColor("nimbusBase", 5.1498413E-4f, -0.45714286f, 0.32941175f, 0)
		Private color12 As Color = decodeColor("nimbusFocus", 0.0f, 0.0f, 0.0f, 0)
		Private color13 As Color = decodeColor("nimbusBase", -0.0038217902f, -0.15532213f, -0.14901963f, 0)
		Private color14 As Color = decodeColor("nimbusBase", -0.57865167f, -0.6357143f, -0.54509807f, 0)
		Private color15 As Color = decodeColor("nimbusBase", 0.004681647f, -0.62780917f, 0.44313723f, 0)
		Private color16 As Color = decodeColor("nimbusBase", 2.9569864E-4f, -0.4653107f, 0.32549018f, 0)
		Private color17 As Color = decodeColor("nimbusBase", 5.1498413E-4f, -0.4563421f, 0.32549018f, 0)
		Private color18 As Color = decodeColor("nimbusBase", -0.0017285943f, -0.4732143f, 0.39215684f, 0)
		Private color19 As Color = decodeColor("nimbusBase", 0.0015952587f, -0.04875779f, -0.18823531f, 0)
		Private color20 As Color = decodeColor("nimbusBase", 2.9569864E-4f, -0.44943976f, 0.25098038f, 0)
		Private color21 As Color = decodeColor("nimbusBase", 0.0f, 0.0f, 0.0f, 0)
		Private color22 As Color = decodeColor("nimbusBase", 8.9377165E-4f, -0.121094406f, 0.12156862f, 0)
		Private color23 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.110526316f, 0.25490195f, -121)
		Private color24 As New Color(150, 156, 168, 146)
		Private color25 As Color = decodeColor("nimbusBase", -0.0033828616f, -0.40608466f, -0.019607842f, 0)
		Private color26 As Color = decodeColor("nimbusBase", 5.1498413E-4f, -0.17594418f, -0.20784315f, 0)
		Private color27 As Color = decodeColor("nimbusBase", 0.0023007393f, -0.11332625f, -0.28627452f, 0)
		Private color28 As Color = decodeColor("nimbusBase", -0.023096085f, -0.62376213f, 0.4352941f, 0)
		Private color29 As Color = decodeColor("nimbusBase", 0.004681647f, -0.594392f, 0.39999998f, 0)
		Private color30 As Color = decodeColor("nimbusBase", -0.0017285943f, -0.4454704f, 0.25490195f, 0)
		Private color31 As Color = decodeColor("nimbusBase", 5.1498413E-4f, -0.4625541f, 0.35686272f, 0)
		Private color32 As Color = decodeColor("nimbusBase", 5.1498413E-4f, -0.47442397f, 0.4235294f, 0)


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
				Case BACKGROUND_DISABLED
					paintBackgroundDisabled(g)
				Case BACKGROUND_ENABLED
					paintBackgroundEnabled(g)
				Case BACKGROUND_FOCUSED
					paintBackgroundFocused(g)
				Case BACKGROUND_FOCUSED_MOUSEOVER
					paintBackgroundFocusedAndMouseOver(g)
				Case BACKGROUND_FOCUSED_PRESSED
					paintBackgroundFocusedAndPressed(g)
				Case BACKGROUND_MOUSEOVER
					paintBackgroundMouseOver(g)
				Case BACKGROUND_PRESSED
					paintBackgroundPressed(g)
				Case BACKGROUND_ENABLED_ARROWSHAPE
					paintBackgroundEnabledAndArrowShape(g)
				Case BACKGROUND_DISABLED_ARROWSHAPE
					paintBackgroundDisabledAndArrowShape(g)
				Case BACKGROUND_MOUSEOVER_ARROWSHAPE
					paintBackgroundMouseOverAndArrowShape(g)
				Case BACKGROUND_PRESSED_ARROWSHAPE
					paintBackgroundPressedAndArrowShape(g)
				Case BACKGROUND_FOCUSED_ARROWSHAPE
					paintBackgroundFocusedAndArrowShape(g)
				Case BACKGROUND_FOCUSED_MOUSEOVER_ARROWSHAPE
					paintBackgroundFocusedAndMouseOverAndArrowShape(g)
				Case BACKGROUND_FOCUSED_PRESSED_ARROWSHAPE
					paintBackgroundFocusedAndPressedAndArrowShape(g)

			End Select
		End Sub



		Protected Friend Property NotOverridable Overrides paintContext As PaintContext
			Get
				Return ctx
			End Get
		End Property

		Private Sub paintBackgroundDisabled(ByVal g As Graphics2D)
			ellipse = decodeEllipse1()
			g.paint = decodeGradient1(ellipse)
			g.fill(ellipse)
			ellipse = decodeEllipse2()
			g.paint = decodeGradient2(ellipse)
			g.fill(ellipse)

		End Sub

		Private Sub paintBackgroundEnabled(ByVal g As Graphics2D)
			ellipse = decodeEllipse3()
			g.paint = color6
			g.fill(ellipse)
			ellipse = decodeEllipse1()
			g.paint = decodeGradient3(ellipse)
			g.fill(ellipse)
			ellipse = decodeEllipse2()
			g.paint = decodeGradient4(ellipse)
			g.fill(ellipse)

		End Sub

		Private Sub paintBackgroundFocused(ByVal g As Graphics2D)
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

		Private Sub paintBackgroundFocusedAndMouseOver(ByVal g As Graphics2D)
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

		Private Sub paintBackgroundFocusedAndPressed(ByVal g As Graphics2D)
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

		Private Sub paintBackgroundMouseOver(ByVal g As Graphics2D)
			ellipse = decodeEllipse3()
			g.paint = color6
			g.fill(ellipse)
			ellipse = decodeEllipse1()
			g.paint = decodeGradient5(ellipse)
			g.fill(ellipse)
			ellipse = decodeEllipse2()
			g.paint = decodeGradient6(ellipse)
			g.fill(ellipse)

		End Sub

		Private Sub paintBackgroundPressed(ByVal g As Graphics2D)
			ellipse = decodeEllipse3()
			g.paint = color23
			g.fill(ellipse)
			ellipse = decodeEllipse1()
			g.paint = decodeGradient7(ellipse)
			g.fill(ellipse)
			ellipse = decodeEllipse2()
			g.paint = decodeGradient8(ellipse)
			g.fill(ellipse)

		End Sub

		Private Sub paintBackgroundEnabledAndArrowShape(ByVal g As Graphics2D)
			path = decodePath1()
			g.paint = color24
			g.fill(path)
			path = decodePath2()
			g.paint = decodeGradient9(path)
			g.fill(path)
			path = decodePath3()
			g.paint = decodeGradient10(path)
			g.fill(path)

		End Sub

		Private Sub paintBackgroundDisabledAndArrowShape(ByVal g As Graphics2D)
			path = decodePath2()
			g.paint = decodeGradient11(path)
			g.fill(path)
			path = decodePath3()
			g.paint = decodeGradient12(path)
			g.fill(path)

		End Sub

		Private Sub paintBackgroundMouseOverAndArrowShape(ByVal g As Graphics2D)
			path = decodePath1()
			g.paint = color24
			g.fill(path)
			path = decodePath2()
			g.paint = decodeGradient13(path)
			g.fill(path)
			path = decodePath3()
			g.paint = decodeGradient14(path)
			g.fill(path)

		End Sub

		Private Sub paintBackgroundPressedAndArrowShape(ByVal g As Graphics2D)
			path = decodePath1()
			g.paint = color24
			g.fill(path)
			path = decodePath2()
			g.paint = decodeGradient15(path)
			g.fill(path)
			path = decodePath3()
			g.paint = decodeGradient16(path)
			g.fill(path)

		End Sub

		Private Sub paintBackgroundFocusedAndArrowShape(ByVal g As Graphics2D)
			path = decodePath4()
			g.paint = color12
			g.fill(path)
			path = decodePath2()
			g.paint = decodeGradient9(path)
			g.fill(path)
			path = decodePath3()
			g.paint = decodeGradient17(path)
			g.fill(path)

		End Sub

		Private Sub paintBackgroundFocusedAndMouseOverAndArrowShape(ByVal g As Graphics2D)
			path = decodePath4()
			g.paint = color12
			g.fill(path)
			path = decodePath2()
			g.paint = decodeGradient13(path)
			g.fill(path)
			path = decodePath3()
			g.paint = decodeGradient14(path)
			g.fill(path)

		End Sub

		Private Sub paintBackgroundFocusedAndPressedAndArrowShape(ByVal g As Graphics2D)
			path = decodePath4()
			g.paint = color12
			g.fill(path)
			path = decodePath2()
			g.paint = decodeGradient15(path)
			g.fill(path)
			path = decodePath3()
			g.paint = decodeGradient16(path)
			g.fill(path)

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

		Private Function decodePath1() As Path2D
			path.reset()
			path.moveTo(decodeX(0.8166667f), decodeY(0.5007576f))
			path.curveTo(decodeAnchorX(0.8166667222976685f, 1.5643268796105616f), decodeAnchorY(0.5007575750350952f, -0.309751314021121f), decodeAnchorX(2.7925455570220947f, 0.058173584548962154f), decodeAnchorY(1.6116883754730225f, -0.46476349119779314f), decodeX(2.7925456f), decodeY(1.6116884f))
			path.curveTo(decodeAnchorX(2.7925455570220947f, -0.34086855855797005f), decodeAnchorY(1.6116883754730225f, 2.723285191092547f), decodeAnchorX(0.7006363868713379f, 4.56812791706229f), decodeAnchorY(2.7693636417388916f, -0.006014915148298883f), decodeX(0.7006364f), decodeY(2.7693636f))
			path.curveTo(decodeAnchorX(0.7006363868713379f, -3.523395559100149f), decodeAnchorY(2.7693636417388916f, 0.004639302074426865f), decodeAnchorX(0.8166667222976685f, -1.8635255186676325f), decodeAnchorY(0.5007575750350952f, 0.3689954354443423f), decodeX(0.8166667f), decodeY(0.5007576f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath2() As Path2D
			path.reset()
			path.moveTo(decodeX(0.6155303f), decodeY(2.5954547f))
			path.curveTo(decodeAnchorX(0.6155303120613098f, 0.9098089454358838f), decodeAnchorY(2.595454692840576f, 1.3154241785830862f), decodeAnchorX(2.6151516437530518f, 0.014588808096503314f), decodeAnchorY(1.611201286315918f, 0.9295520709665155f), decodeX(2.6151516f), decodeY(1.6112013f))
			path.curveTo(decodeAnchorX(2.6151516437530518f, -0.013655180248463239f), decodeAnchorY(1.611201286315918f, -0.8700642982905453f), decodeAnchorX(0.6092391610145569f, 0.9729934749047704f), decodeAnchorY(0.4071640372276306f, -1.424864396720248f), decodeX(0.60923916f), decodeY(0.40716404f))
			path.curveTo(decodeAnchorX(0.6092391610145569f, -0.7485208875763871f), decodeAnchorY(0.4071640372276306f, 1.0961437978948614f), decodeAnchorX(0.6155303120613098f, -0.7499879392488253f), decodeAnchorY(2.595454692840576f, -1.0843510320300886f), decodeX(0.6155303f), decodeY(2.5954547f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath3() As Path2D
			path.reset()
			path.moveTo(decodeX(0.8055606f), decodeY(0.6009697f))
			path.curveTo(decodeAnchorX(0.8055605888366699f, 0.508208945236218f), decodeAnchorY(0.600969672203064f, -0.8490880998025481f), decodeAnchorX(2.3692727088928223f, 0.0031846066137877216f), decodeAnchorY(1.613116979598999f, -0.6066882577419275f), decodeX(2.3692727f), decodeY(1.613117f))
			path.curveTo(decodeAnchorX(2.3692727088928223f, -0.0038901961210928704f), decodeAnchorY(1.613116979598999f, 0.7411076447438294f), decodeAnchorX(0.7945454716682434f, 0.38709738141524763f), decodeAnchorY(2.393272876739502f, 1.240782009971129f), decodeX(0.7945455f), decodeY(2.3932729f))
			path.curveTo(decodeAnchorX(0.7945454716682434f, -0.3863658307342148f), decodeAnchorY(2.393272876739502f, -1.2384371350947134f), decodeAnchorX(0.8055605888366699f, -0.9951540091537732f), decodeAnchorY(0.600969672203064f, 1.6626496533832493f), decodeX(0.8055606f), decodeY(0.6009697f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath4() As Path2D
			path.reset()
			path.moveTo(decodeX(0.60059524f), decodeY(0.11727543f))
			path.curveTo(decodeAnchorX(0.600595235824585f, 1.5643268796105612f), decodeAnchorY(0.1172754317522049f, -0.3097513140211208f), decodeAnchorX(2.7925455570220947f, 0.004405844009975013f), decodeAnchorY(1.6116883754730225f, -1.1881161542467655f), decodeX(2.7925456f), decodeY(1.6116884f))
			path.curveTo(decodeAnchorX(2.7925455570220947f, -0.007364540661274788f), decodeAnchorY(1.6116883754730225f, 1.9859826422490698f), decodeAnchorX(0.7006363868713379f, 2.7716863466452586f), decodeAnchorY(2.869363784790039f, -0.008974581987587271f), decodeX(0.7006364f), decodeY(2.8693638f))
			path.curveTo(decodeAnchorX(0.7006363868713379f, -3.75489914400509f), decodeAnchorY(2.869363784790039f, 0.012158175929172899f), decodeAnchorX(0.600595235824585f, -1.8635255186676323f), decodeAnchorY(0.1172754317522049f, 0.3689954354443423f), decodeX(0.60059524f), decodeY(0.11727543f))
			path.closePath()
			Return path
		End Function



		Private Function decodeGradient1(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5106101f * w) + x, (-4.553649E-18f * h) + y, (0.49933687f * w) + x, (1.0039787f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color1, decodeColor(color1,color2,0.5f), color2})
		End Function

		Private Function decodeGradient2(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5023511f * w) + x, (0.0015673981f * h) + y, (0.5023511f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.21256684f,0.42513368f,0.71256685f,1.0f }, New Color() { color3, decodeColor(color3,color4,0.5f), color4, decodeColor(color4,color5,0.5f), color5})
		End Function

		Private Function decodeGradient3(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.51f * w) + x, (-4.553649E-18f * h) + y, (0.51f * w) + x, (1.0039787f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color7, decodeColor(color7,color8,0.5f), color8})
		End Function

		Private Function decodeGradient4(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0015673981f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.21256684f,0.42513368f,0.56149733f,0.69786096f,0.8489305f,1.0f }, New Color() { color9, decodeColor(color9,color10,0.5f), color10, decodeColor(color10,color10,0.5f), color10, decodeColor(color10,color11,0.5f), color11})
		End Function

		Private Function decodeGradient5(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5106101f * w) + x, (-4.553649E-18f * h) + y, (0.49933687f * w) + x, (1.0039787f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color13, decodeColor(color13,color14,0.5f), color14})
		End Function

		Private Function decodeGradient6(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5023511f * w) + x, (0.0015673981f * h) + y, (0.5023511f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.21256684f,0.42513368f,0.56149733f,0.69786096f,0.8489305f,1.0f }, New Color() { color15, decodeColor(color15,color16,0.5f), color16, decodeColor(color16,color17,0.5f), color17, decodeColor(color17,color18,0.5f), color18})
		End Function

		Private Function decodeGradient7(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5106101f * w) + x, (-4.553649E-18f * h) + y, (0.49933687f * w) + x, (1.0039787f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color14, decodeColor(color14,color19,0.5f), color19})
		End Function

		Private Function decodeGradient8(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5023511f * w) + x, (0.0015673981f * h) + y, (0.5023511f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.23796791f,0.47593582f,0.5360962f,0.5962567f,0.79812837f,1.0f }, New Color() { color20, decodeColor(color20,color21,0.5f), color21, decodeColor(color21,color21,0.5f), color21, decodeColor(color21,color22,0.5f), color22})
		End Function

		Private Function decodeGradient9(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.24032257f,0.48064515f,0.7403226f,1.0f }, New Color() { color25, decodeColor(color25,color26,0.5f), color26, decodeColor(color26,color27,0.5f), color27})
		End Function

		Private Function decodeGradient10(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.061290324f,0.1016129f,0.14193548f,0.3016129f,0.46129033f,0.5983871f,0.7354839f,0.7935484f,0.8516129f }, New Color() { color28, decodeColor(color28,color29,0.5f), color29, decodeColor(color29,color30,0.5f), color30, decodeColor(color30,color31,0.5f), color31, decodeColor(color31,color32,0.5f), color32})
		End Function

		Private Function decodeGradient11(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color1, decodeColor(color1,color2,0.5f), color2})
		End Function

		Private Function decodeGradient12(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.21256684f,0.42513368f,0.71256685f,1.0f }, New Color() { color3, decodeColor(color3,color4,0.5f), color4, decodeColor(color4,color5,0.5f), color5})
		End Function

		Private Function decodeGradient13(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color13, decodeColor(color13,color14,0.5f), color14})
		End Function

		Private Function decodeGradient14(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.21256684f,0.42513368f,0.56149733f,0.69786096f,0.8489305f,1.0f }, New Color() { color15, decodeColor(color15,color16,0.5f), color16, decodeColor(color16,color17,0.5f), color17, decodeColor(color17,color18,0.5f), color18})
		End Function

		Private Function decodeGradient15(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color14, decodeColor(color14,color19,0.5f), color19})
		End Function

		Private Function decodeGradient16(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.23796791f,0.47593582f,0.5360962f,0.5962567f,0.79812837f,1.0f }, New Color() { color20, decodeColor(color20,color21,0.5f), color21, decodeColor(color21,color21,0.5f), color21, decodeColor(color21,color22,0.5f), color22})
		End Function

		Private Function decodeGradient17(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.4925773f * w) + x, (0.082019866f * h) + y, (0.4925773f * w) + x, (0.91798013f * h) + y, New Single() { 0.061290324f,0.1016129f,0.14193548f,0.3016129f,0.46129033f,0.5983871f,0.7354839f,0.7935484f,0.8516129f }, New Color() { color28, decodeColor(color28,color29,0.5f), color29, decodeColor(color29,color30,0.5f), color30, decodeColor(color30,color31,0.5f), color31, decodeColor(color31,color32,0.5f), color32})
		End Function


	End Class

End Namespace
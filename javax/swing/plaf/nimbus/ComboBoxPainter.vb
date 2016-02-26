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



	Friend NotInheritable Class ComboBoxPainter
		Inherits AbstractRegionPainter

		'package private integers representing the available states that
		'this painter will paint. These are used when creating a new instance
		'of ComboBoxPainter to determine which region/state is being painted
		'by that instance.
		Friend Const BACKGROUND_DISABLED As Integer = 1
		Friend Const BACKGROUND_DISABLED_PRESSED As Integer = 2
		Friend Const BACKGROUND_ENABLED As Integer = 3
		Friend Const BACKGROUND_FOCUSED As Integer = 4
		Friend Const BACKGROUND_MOUSEOVER_FOCUSED As Integer = 5
		Friend Const BACKGROUND_MOUSEOVER As Integer = 6
		Friend Const BACKGROUND_PRESSED_FOCUSED As Integer = 7
		Friend Const BACKGROUND_PRESSED As Integer = 8
		Friend Const BACKGROUND_ENABLED_SELECTED As Integer = 9
		Friend Const BACKGROUND_DISABLED_EDITABLE As Integer = 10
		Friend Const BACKGROUND_ENABLED_EDITABLE As Integer = 11
		Friend Const BACKGROUND_FOCUSED_EDITABLE As Integer = 12
		Friend Const BACKGROUND_MOUSEOVER_EDITABLE As Integer = 13
		Friend Const BACKGROUND_PRESSED_EDITABLE As Integer = 14


		Private state As Integer 'refers to one of the static final ints above
		Private ctx As PaintContext

		'the following 4 variables are reused during the painting code of the layers
		Private path As Path2D = New Path2D.Float
		Private rect As Rectangle2D = New Rectangle2D.Float(0, 0, 0, 0)
		Private roundRect As RoundRectangle2D = New RoundRectangle2D.Float(0, 0, 0, 0, 0, 0)
		Private ellipse As Ellipse2D = New Ellipse2D.Float(0, 0, 0, 0)

		'All Colors used for painting are stored here. Ideally, only those colors being used
		'by a particular instance of ComboBoxPainter would be created. For the moment at least,
		'however, all are created for each instance.
		Private color1 As Color = decodeColor("nimbusBlueGrey", -0.6111111f, -0.110526316f, -0.74509805f, -247)
		Private color2 As Color = decodeColor("nimbusBase", 0.032459438f, -0.5928571f, 0.2745098f, 0)
		Private color3 As Color = decodeColor("nimbusBase", 0.032459438f, -0.590029f, 0.2235294f, 0)
		Private color4 As Color = decodeColor("nimbusBase", 0.032459438f, -0.60996324f, 0.36470586f, 0)
		Private color5 As Color = decodeColor("nimbusBase", 0.040395975f, -0.60474086f, 0.33725488f, 0)
		Private color6 As Color = decodeColor("nimbusBase", 0.032459438f, -0.5953556f, 0.32549018f, 0)
		Private color7 As Color = decodeColor("nimbusBase", 0.032459438f, -0.5957143f, 0.3333333f, 0)
		Private color8 As Color = decodeColor("nimbusBase", 0.021348298f, -0.56289876f, 0.2588235f, 0)
		Private color9 As Color = decodeColor("nimbusBase", 0.010237217f, -0.55799407f, 0.20784312f, 0)
		Private color10 As Color = decodeColor("nimbusBase", 0.021348298f, -0.59223604f, 0.35294116f, 0)
		Private color11 As Color = decodeColor("nimbusBase", 0.02391243f, -0.5774183f, 0.32549018f, 0)
		Private color12 As Color = decodeColor("nimbusBase", 0.021348298f, -0.56722116f, 0.3098039f, 0)
		Private color13 As Color = decodeColor("nimbusBase", 0.021348298f, -0.567841f, 0.31764704f, 0)
		Private color14 As Color = decodeColor("nimbusBlueGrey", 0.0f, 0.0f, -0.22f, -176)
		Private color15 As Color = decodeColor("nimbusBase", 0.032459438f, -0.5787523f, 0.07058823f, 0)
		Private color16 As Color = decodeColor("nimbusBase", 0.032459438f, -0.5399696f, -0.18039218f, 0)
		Private color17 As Color = decodeColor("nimbusBase", 0.08801502f, -0.63174605f, 0.43921566f, 0)
		Private color18 As Color = decodeColor("nimbusBase", 0.040395975f, -0.6054113f, 0.35686272f, 0)
		Private color19 As Color = decodeColor("nimbusBase", 0.032459438f, -0.5998577f, 0.4352941f, 0)
		Private color20 As Color = decodeColor("nimbusBase", 5.1498413E-4f, -0.34585923f, -0.007843137f, 0)
		Private color21 As Color = decodeColor("nimbusBase", 5.1498413E-4f, -0.095173776f, -0.25882354f, 0)
		Private color22 As Color = decodeColor("nimbusBase", 0.004681647f, -0.6197143f, 0.43137252f, 0)
		Private color23 As Color = decodeColor("nimbusBase", -0.0028941035f, -0.4800539f, 0.28235292f, 0)
		Private color24 As Color = decodeColor("nimbusBase", 5.1498413E-4f, -0.43866998f, 0.24705881f, 0)
		Private color25 As Color = decodeColor("nimbusBase", 5.1498413E-4f, -0.4625541f, 0.35686272f, 0)
		Private color26 As Color = decodeColor("nimbusFocus", 0.0f, 0.0f, 0.0f, 0)
		Private color27 As Color = decodeColor("nimbusBase", 0.032459438f, -0.54616207f, -0.02352941f, 0)
		Private color28 As Color = decodeColor("nimbusBase", 0.032459438f, -0.41349208f, -0.33725494f, 0)
		Private color29 As Color = decodeColor("nimbusBase", 0.08801502f, -0.6317773f, 0.4470588f, 0)
		Private color30 As Color = decodeColor("nimbusBase", 0.032459438f, -0.6113241f, 0.41568625f, 0)
		Private color31 As Color = decodeColor("nimbusBase", 0.032459438f, -0.5985242f, 0.39999998f, 0)
		Private color32 As Color = decodeColor("nimbusBase", 0.0f, -0.6357143f, 0.45098037f, 0)
		Private color33 As Color = decodeColor("nimbusBase", 0.0013483167f, -0.1769987f, -0.12156865f, 0)
		Private color34 As Color = decodeColor("nimbusBase", 0.059279382f, 0.3642857f, -0.43529415f, 0)
		Private color35 As Color = decodeColor("nimbusBase", 0.004681647f, -0.6198413f, 0.43921566f, 0)
		Private color36 As Color = decodeColor("nimbusBase", -8.738637E-4f, -0.50527954f, 0.35294116f, 0)
		Private color37 As Color = decodeColor("nimbusBase", 5.1498413E-4f, -0.4555341f, 0.3215686f, 0)
		Private color38 As Color = decodeColor("nimbusBase", 5.1498413E-4f, -0.4757143f, 0.43137252f, 0)
		Private color39 As Color = decodeColor("nimbusBase", 0.08801502f, 0.3642857f, -0.52156866f, 0)
		Private color40 As Color = decodeColor("nimbusBase", 0.032459438f, -0.5246032f, -0.12549022f, 0)
		Private color41 As Color = decodeColor("nimbusBase", 0.027408898f, -0.5847884f, 0.2980392f, 0)
		Private color42 As Color = decodeColor("nimbusBase", 0.026611507f, -0.53623784f, 0.19999999f, 0)
		Private color43 As Color = decodeColor("nimbusBase", 0.029681683f, -0.52701867f, 0.17254901f, 0)
		Private color44 As Color = decodeColor("nimbusBase", 0.03801495f, -0.5456242f, 0.3215686f, 0)
		Private color45 As Color = decodeColor("nimbusBase", -0.57865167f, -0.6357143f, -0.54901963f, 0)
		Private color46 As Color = decodeColor("nimbusBase", -3.528595E-5f, 0.018606722f, -0.23137257f, 0)
		Private color47 As Color = decodeColor("nimbusBase", -4.2033195E-4f, -0.38050595f, 0.20392156f, 0)
		Private color48 As Color = decodeColor("nimbusBase", 4.081726E-4f, -0.12922078f, 0.054901958f, 0)
		Private color49 As Color = decodeColor("nimbusBase", 0.0f, -0.00895375f, 0.007843137f, 0)
		Private color50 As Color = decodeColor("nimbusBase", -0.0015907288f, -0.1436508f, 0.19215685f, 0)
		Private color51 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.110526316f, 0.25490195f, -83)
		Private color52 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.110526316f, 0.25490195f, -88)
		Private color53 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.005263157f, -0.52156866f, -191)


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
				Case BACKGROUND_DISABLED_PRESSED
					paintBackgroundDisabledAndPressed(g)
				Case BACKGROUND_ENABLED
					paintBackgroundEnabled(g)
				Case BACKGROUND_FOCUSED
					paintBackgroundFocused(g)
				Case BACKGROUND_MOUSEOVER_FOCUSED
					paintBackgroundMouseOverAndFocused(g)
				Case BACKGROUND_MOUSEOVER
					paintBackgroundMouseOver(g)
				Case BACKGROUND_PRESSED_FOCUSED
					paintBackgroundPressedAndFocused(g)
				Case BACKGROUND_PRESSED
					paintBackgroundPressed(g)
				Case BACKGROUND_ENABLED_SELECTED
					paintBackgroundEnabledAndSelected(g)
				Case BACKGROUND_DISABLED_EDITABLE
					paintBackgroundDisabledAndEditable(g)
				Case BACKGROUND_ENABLED_EDITABLE
					paintBackgroundEnabledAndEditable(g)
				Case BACKGROUND_FOCUSED_EDITABLE
					paintBackgroundFocusedAndEditable(g)
				Case BACKGROUND_MOUSEOVER_EDITABLE
					paintBackgroundMouseOverAndEditable(g)
				Case BACKGROUND_PRESSED_EDITABLE
					paintBackgroundPressedAndEditable(g)

			End Select
		End Sub

		Protected Friend Overrides Function getExtendedCacheKeys(ByVal c As JComponent) As Object()
			Dim ___extendedCacheKeys As Object() = Nothing
			Select Case state
				Case BACKGROUND_ENABLED
					___extendedCacheKeys = New Object() { getComponentColor(c, "background", color17, -0.63174605f, 0.43921566f, 0), getComponentColor(c, "background", color18, -0.6054113f, 0.35686272f, 0), getComponentColor(c, "background", color6, -0.5953556f, 0.32549018f, 0), getComponentColor(c, "background", color19, -0.5998577f, 0.4352941f, 0), getComponentColor(c, "background", color22, -0.6197143f, 0.43137252f, 0), getComponentColor(c, "background", color23, -0.4800539f, 0.28235292f, 0), getComponentColor(c, "background", color24, -0.43866998f, 0.24705881f, 0), getComponentColor(c, "background", color25, -0.4625541f, 0.35686272f, 0)}
				Case BACKGROUND_FOCUSED
					___extendedCacheKeys = New Object() { getComponentColor(c, "background", color17, -0.63174605f, 0.43921566f, 0), getComponentColor(c, "background", color18, -0.6054113f, 0.35686272f, 0), getComponentColor(c, "background", color6, -0.5953556f, 0.32549018f, 0), getComponentColor(c, "background", color19, -0.5998577f, 0.4352941f, 0), getComponentColor(c, "background", color22, -0.6197143f, 0.43137252f, 0), getComponentColor(c, "background", color23, -0.4800539f, 0.28235292f, 0), getComponentColor(c, "background", color24, -0.43866998f, 0.24705881f, 0), getComponentColor(c, "background", color25, -0.4625541f, 0.35686272f, 0)}
				Case BACKGROUND_MOUSEOVER_FOCUSED
					___extendedCacheKeys = New Object() { getComponentColor(c, "background", color29, -0.6317773f, 0.4470588f, 0), getComponentColor(c, "background", color30, -0.6113241f, 0.41568625f, 0), getComponentColor(c, "background", color31, -0.5985242f, 0.39999998f, 0), getComponentColor(c, "background", color32, -0.6357143f, 0.45098037f, 0), getComponentColor(c, "background", color35, -0.6198413f, 0.43921566f, 0), getComponentColor(c, "background", color36, -0.50527954f, 0.35294116f, 0), getComponentColor(c, "background", color37, -0.4555341f, 0.3215686f, 0), getComponentColor(c, "background", color25, -0.4625541f, 0.35686272f, 0), getComponentColor(c, "background", color38, -0.4757143f, 0.43137252f, 0)}
				Case BACKGROUND_MOUSEOVER
					___extendedCacheKeys = New Object() { getComponentColor(c, "background", color29, -0.6317773f, 0.4470588f, 0), getComponentColor(c, "background", color30, -0.6113241f, 0.41568625f, 0), getComponentColor(c, "background", color31, -0.5985242f, 0.39999998f, 0), getComponentColor(c, "background", color32, -0.6357143f, 0.45098037f, 0), getComponentColor(c, "background", color35, -0.6198413f, 0.43921566f, 0), getComponentColor(c, "background", color36, -0.50527954f, 0.35294116f, 0), getComponentColor(c, "background", color37, -0.4555341f, 0.3215686f, 0), getComponentColor(c, "background", color25, -0.4625541f, 0.35686272f, 0), getComponentColor(c, "background", color38, -0.4757143f, 0.43137252f, 0)}
				Case BACKGROUND_PRESSED_FOCUSED
					___extendedCacheKeys = New Object() { getComponentColor(c, "background", color41, -0.5847884f, 0.2980392f, 0), getComponentColor(c, "background", color42, -0.53623784f, 0.19999999f, 0), getComponentColor(c, "background", color43, -0.52701867f, 0.17254901f, 0), getComponentColor(c, "background", color44, -0.5456242f, 0.3215686f, 0), getComponentColor(c, "background", color47, -0.38050595f, 0.20392156f, 0), getComponentColor(c, "background", color48, -0.12922078f, 0.054901958f, 0), getComponentColor(c, "background", color49, -0.00895375f, 0.007843137f, 0), getComponentColor(c, "background", color50, -0.1436508f, 0.19215685f, 0)}
				Case BACKGROUND_PRESSED
					___extendedCacheKeys = New Object() { getComponentColor(c, "background", color41, -0.5847884f, 0.2980392f, 0), getComponentColor(c, "background", color42, -0.53623784f, 0.19999999f, 0), getComponentColor(c, "background", color43, -0.52701867f, 0.17254901f, 0), getComponentColor(c, "background", color44, -0.5456242f, 0.3215686f, 0), getComponentColor(c, "background", color47, -0.38050595f, 0.20392156f, 0), getComponentColor(c, "background", color48, -0.12922078f, 0.054901958f, 0), getComponentColor(c, "background", color49, -0.00895375f, 0.007843137f, 0), getComponentColor(c, "background", color50, -0.1436508f, 0.19215685f, 0)}
				Case BACKGROUND_ENABLED_SELECTED
					___extendedCacheKeys = New Object() { getComponentColor(c, "background", color41, -0.5847884f, 0.2980392f, 0), getComponentColor(c, "background", color42, -0.53623784f, 0.19999999f, 0), getComponentColor(c, "background", color43, -0.52701867f, 0.17254901f, 0), getComponentColor(c, "background", color44, -0.5456242f, 0.3215686f, 0), getComponentColor(c, "background", color47, -0.38050595f, 0.20392156f, 0), getComponentColor(c, "background", color48, -0.12922078f, 0.054901958f, 0), getComponentColor(c, "background", color49, -0.00895375f, 0.007843137f, 0), getComponentColor(c, "background", color50, -0.1436508f, 0.19215685f, 0)}
			End Select
			Return ___extendedCacheKeys
		End Function

		Protected Friend Property NotOverridable Overrides paintContext As PaintContext
			Get
				Return ctx
			End Get
		End Property

		Private Sub paintBackgroundDisabled(ByVal g As Graphics2D)
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
			g.paint = decodeGradient4(path)
			g.fill(path)

		End Sub

		Private Sub paintBackgroundDisabledAndPressed(ByVal g As Graphics2D)
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
			g.paint = decodeGradient4(path)
			g.fill(path)

		End Sub

		Private Sub paintBackgroundEnabled(ByVal g As Graphics2D)
			path = decodePath1()
			g.paint = color14
			g.fill(path)
			path = decodePath2()
			g.paint = decodeGradient5(path)
			g.fill(path)
			path = decodePath3()
			g.paint = decodeGradient6(path)
			g.fill(path)
			path = decodePath4()
			g.paint = decodeGradient7(path)
			g.fill(path)
			path = decodePath5()
			g.paint = decodeGradient8(path)
			g.fill(path)

		End Sub

		Private Sub paintBackgroundFocused(ByVal g As Graphics2D)
			roundRect = decodeRoundRect1()
			g.paint = color26
			g.fill(roundRect)
			path = decodePath2()
			g.paint = decodeGradient5(path)
			g.fill(path)
			path = decodePath3()
			g.paint = decodeGradient6(path)
			g.fill(path)
			path = decodePath4()
			g.paint = decodeGradient7(path)
			g.fill(path)
			path = decodePath5()
			g.paint = decodeGradient8(path)
			g.fill(path)

		End Sub

		Private Sub paintBackgroundMouseOverAndFocused(ByVal g As Graphics2D)
			roundRect = decodeRoundRect1()
			g.paint = color26
			g.fill(roundRect)
			path = decodePath2()
			g.paint = decodeGradient9(path)
			g.fill(path)
			path = decodePath3()
			g.paint = decodeGradient6(path)
			g.fill(path)
			path = decodePath4()
			g.paint = decodeGradient10(path)
			g.fill(path)
			path = decodePath5()
			g.paint = decodeGradient8(path)
			g.fill(path)

		End Sub

		Private Sub paintBackgroundMouseOver(ByVal g As Graphics2D)
			path = decodePath1()
			g.paint = color14
			g.fill(path)
			path = decodePath2()
			g.paint = decodeGradient9(path)
			g.fill(path)
			path = decodePath3()
			g.paint = decodeGradient6(path)
			g.fill(path)
			path = decodePath4()
			g.paint = decodeGradient10(path)
			g.fill(path)
			path = decodePath5()
			g.paint = decodeGradient8(path)
			g.fill(path)

		End Sub

		Private Sub paintBackgroundPressedAndFocused(ByVal g As Graphics2D)
			roundRect = decodeRoundRect1()
			g.paint = color26
			g.fill(roundRect)
			path = decodePath2()
			g.paint = decodeGradient11(path)
			g.fill(path)
			path = decodePath3()
			g.paint = decodeGradient6(path)
			g.fill(path)
			path = decodePath4()
			g.paint = decodeGradient12(path)
			g.fill(path)
			path = decodePath5()
			g.paint = decodeGradient8(path)
			g.fill(path)

		End Sub

		Private Sub paintBackgroundPressed(ByVal g As Graphics2D)
			path = decodePath1()
			g.paint = color51
			g.fill(path)
			path = decodePath2()
			g.paint = decodeGradient11(path)
			g.fill(path)
			path = decodePath3()
			g.paint = decodeGradient6(path)
			g.fill(path)
			path = decodePath4()
			g.paint = decodeGradient12(path)
			g.fill(path)
			path = decodePath5()
			g.paint = decodeGradient8(path)
			g.fill(path)

		End Sub

		Private Sub paintBackgroundEnabledAndSelected(ByVal g As Graphics2D)
			path = decodePath1()
			g.paint = color52
			g.fill(path)
			path = decodePath2()
			g.paint = decodeGradient11(path)
			g.fill(path)
			path = decodePath3()
			g.paint = decodeGradient6(path)
			g.fill(path)
			path = decodePath4()
			g.paint = decodeGradient12(path)
			g.fill(path)
			path = decodePath5()
			g.paint = decodeGradient8(path)
			g.fill(path)

		End Sub

		Private Sub paintBackgroundDisabledAndEditable(ByVal g As Graphics2D)
			rect = decodeRect1()
			g.paint = color53
			g.fill(rect)

		End Sub

		Private Sub paintBackgroundEnabledAndEditable(ByVal g As Graphics2D)
			rect = decodeRect1()
			g.paint = color53
			g.fill(rect)

		End Sub

		Private Sub paintBackgroundFocusedAndEditable(ByVal g As Graphics2D)
			path = decodePath6()
			g.paint = color26
			g.fill(path)

		End Sub

		Private Sub paintBackgroundMouseOverAndEditable(ByVal g As Graphics2D)
			rect = decodeRect2()
			g.paint = color53
			g.fill(rect)

		End Sub

		Private Sub paintBackgroundPressedAndEditable(ByVal g As Graphics2D)
			rect = decodeRect2()
			g.paint = color53
			g.fill(rect)

		End Sub



		Private Function decodePath1() As Path2D
			path.reset()
			path.moveTo(decodeX(0.22222222f), decodeY(2.0f))
			path.lineTo(decodeX(0.22222222f), decodeY(2.25f))
			path.curveTo(decodeAnchorX(0.2222222238779068f, 0.0f), decodeAnchorY(2.25f, 3.0f), decodeAnchorX(0.7777777910232544f, -3.0f), decodeAnchorY(2.875f, 0.0f), decodeX(0.7777778f), decodeY(2.875f))
			path.lineTo(decodeX(2.631579f), decodeY(2.875f))
			path.curveTo(decodeAnchorX(2.6315789222717285f, 3.0f), decodeAnchorY(2.875f, 0.0f), decodeAnchorX(2.8947367668151855f, 0.0f), decodeAnchorY(2.25f, 3.0f), decodeX(2.8947368f), decodeY(2.25f))
			path.lineTo(decodeX(2.8947368f), decodeY(2.0f))
			path.lineTo(decodeX(0.22222222f), decodeY(2.0f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath2() As Path2D
			path.reset()
			path.moveTo(decodeX(0.22222222f), decodeY(0.875f))
			path.lineTo(decodeX(0.22222222f), decodeY(2.125f))
			path.curveTo(decodeAnchorX(0.2222222238779068f, 0.0f), decodeAnchorY(2.125f, 3.0f), decodeAnchorX(0.7777777910232544f, -3.0f), decodeAnchorY(2.75f, 0.0f), decodeX(0.7777778f), decodeY(2.75f))
			path.lineTo(decodeX(2.0f), decodeY(2.75f))
			path.lineTo(decodeX(2.0f), decodeY(0.25f))
			path.lineTo(decodeX(0.7777778f), decodeY(0.25f))
			path.curveTo(decodeAnchorX(0.7777777910232544f, -3.0f), decodeAnchorY(0.25f, 0.0f), decodeAnchorX(0.2222222238779068f, 0.0f), decodeAnchorY(0.875f, -3.0f), decodeX(0.22222222f), decodeY(0.875f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath3() As Path2D
			path.reset()
			path.moveTo(decodeX(0.8888889f), decodeY(0.375f))
			path.lineTo(decodeX(2.0f), decodeY(0.375f))
			path.lineTo(decodeX(2.0f), decodeY(2.625f))
			path.lineTo(decodeX(0.8888889f), decodeY(2.625f))
			path.curveTo(decodeAnchorX(0.8888888955116272f, -4.0f), decodeAnchorY(2.625f, 0.0f), decodeAnchorX(0.3333333432674408f, 0.0f), decodeAnchorY(2.0f, 4.0f), decodeX(0.33333334f), decodeY(2.0f))
			path.lineTo(decodeX(0.33333334f), decodeY(0.875f))
			path.curveTo(decodeAnchorX(0.3333333432674408f, 0.0f), decodeAnchorY(0.875f, -3.0f), decodeAnchorX(0.8888888955116272f, -4.0f), decodeAnchorY(0.375f, 0.0f), decodeX(0.8888889f), decodeY(0.375f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath4() As Path2D
			path.reset()
			path.moveTo(decodeX(2.0f), decodeY(0.25f))
			path.lineTo(decodeX(2.631579f), decodeY(0.25f))
			path.curveTo(decodeAnchorX(2.6315789222717285f, 3.0f), decodeAnchorY(0.25f, 0.0f), decodeAnchorX(2.8947367668151855f, 0.0f), decodeAnchorY(0.875f, -3.0f), decodeX(2.8947368f), decodeY(0.875f))
			path.lineTo(decodeX(2.8947368f), decodeY(2.125f))
			path.curveTo(decodeAnchorX(2.8947367668151855f, 0.0f), decodeAnchorY(2.125f, 3.0f), decodeAnchorX(2.6315789222717285f, 3.0f), decodeAnchorY(2.75f, 0.0f), decodeX(2.631579f), decodeY(2.75f))
			path.lineTo(decodeX(2.0f), decodeY(2.75f))
			path.lineTo(decodeX(2.0f), decodeY(0.25f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath5() As Path2D
			path.reset()
			path.moveTo(decodeX(2.0131578f), decodeY(0.375f))
			path.lineTo(decodeX(2.5789473f), decodeY(0.375f))
			path.curveTo(decodeAnchorX(2.5789473056793213f, 4.0f), decodeAnchorY(0.375f, 0.0f), decodeAnchorX(2.8421053886413574f, 0.0f), decodeAnchorY(1.0f, -4.0f), decodeX(2.8421054f), decodeY(1.0f))
			path.lineTo(decodeX(2.8421054f), decodeY(2.0f))
			path.curveTo(decodeAnchorX(2.8421053886413574f, 0.0f), decodeAnchorY(2.0f, 4.0f), decodeAnchorX(2.5789473056793213f, 4.0f), decodeAnchorY(2.625f, 0.0f), decodeX(2.5789473f), decodeY(2.625f))
			path.lineTo(decodeX(2.0131578f), decodeY(2.625f))
			path.lineTo(decodeX(2.0131578f), decodeY(0.375f))
			path.closePath()
			Return path
		End Function

		Private Function decodeRoundRect1() As RoundRectangle2D
			roundRect.roundRectect(decodeX(0.06666667f), decodeY(0.075f), decodeX(2.9684212f) - decodeX(0.06666667f), decodeY(2.925f) - decodeY(0.075f), 13.0f, 13.0f) 'rounding - height - width - y - x
			Return roundRect
		End Function

		Private Function decodeRect1() As Rectangle2D
				rect.rectect(decodeX(1.4385965f), decodeY(1.4444444f), decodeX(1.4385965f) - decodeX(1.4385965f), decodeY(1.4444444f) - decodeY(1.4444444f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodePath6() As Path2D
			path.reset()
			path.moveTo(decodeX(0.120000005f), decodeY(0.120000005f))
			path.lineTo(decodeX(1.9954545f), decodeY(0.120000005f))
			path.curveTo(decodeAnchorX(1.9954545497894287f, 3.0f), decodeAnchorY(0.12000000476837158f, 0.0f), decodeAnchorX(2.8799986839294434f, 0.0f), decodeAnchorY(1.0941176414489746f, -2.9999999999999996f), decodeX(2.8799987f), decodeY(1.0941176f))
			path.lineTo(decodeX(2.8799987f), decodeY(1.964706f))
			path.curveTo(decodeAnchorX(2.8799986839294434f, 0.0f), decodeAnchorY(1.9647059440612793f, 3.0f), decodeAnchorX(1.9954545497894287f, 3.0f), decodeAnchorY(2.879999876022339f, 0.0f), decodeX(1.9954545f), decodeY(2.8799999f))
			path.lineTo(decodeX(0.120000005f), decodeY(2.8799999f))
			path.lineTo(decodeX(0.120000005f), decodeY(0.120000005f))
			path.closePath()
			Return path
		End Function

		Private Function decodeRect2() As Rectangle2D
				rect.rectect(decodeX(1.4385965f), decodeY(1.5f), decodeX(1.4385965f) - decodeX(1.4385965f), decodeY(1.5f) - decodeY(1.5f)) 'height - width - y - x
			Return rect
		End Function



		Private Function decodeGradient1(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color2, decodeColor(color2,color3,0.5f), color3})
		End Function

		Private Function decodeGradient2(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.2002841f,0.4005682f,0.5326705f,0.66477275f,0.8323864f,1.0f }, New Color() { color4, decodeColor(color4,color5,0.5f), color5, decodeColor(color5,color6,0.5f), color6, decodeColor(color6,color7,0.5f), color7})
		End Function

		Private Function decodeGradient3(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color8, decodeColor(color8,color9,0.5f), color9})
		End Function

		Private Function decodeGradient4(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.171875f,0.34375f,0.4815341f,0.6193182f,0.8096591f,1.0f }, New Color() { color10, decodeColor(color10,color11,0.5f), color11, decodeColor(color11,color12,0.5f), color12, decodeColor(color12,color13,0.5f), color13})
		End Function

		Private Function decodeGradient5(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color15, decodeColor(color15,color16,0.5f), color16})
		End Function

		Private Function decodeGradient6(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.2002841f,0.4005682f,0.5326705f,0.66477275f,0.8323864f,1.0f }, New Color() { CType(componentColors(0), Color), decodeColor(CType(componentColors(0), Color),CType(componentColors(1), Color),0.5f), CType(componentColors(1), Color), decodeColor(CType(componentColors(1), Color),CType(componentColors(2), Color),0.5f), CType(componentColors(2), Color), decodeColor(CType(componentColors(2), Color),CType(componentColors(3), Color),0.5f), CType(componentColors(3), Color)})
		End Function

		Private Function decodeGradient7(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color20, decodeColor(color20,color21,0.5f), color21})
		End Function

		Private Function decodeGradient8(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.171875f,0.34375f,0.4815341f,0.6193182f,0.8096591f,1.0f }, New Color() { CType(componentColors(4), Color), decodeColor(CType(componentColors(4), Color),CType(componentColors(5), Color),0.5f), CType(componentColors(5), Color), decodeColor(CType(componentColors(5), Color),CType(componentColors(6), Color),0.5f), CType(componentColors(6), Color), decodeColor(CType(componentColors(6), Color),CType(componentColors(7), Color),0.5f), CType(componentColors(7), Color)})
		End Function

		Private Function decodeGradient9(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color27, decodeColor(color27,color28,0.5f), color28})
		End Function

		Private Function decodeGradient10(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color33, decodeColor(color33,color34,0.5f), color34})
		End Function

		Private Function decodeGradient11(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color39, decodeColor(color39,color40,0.5f), color40})
		End Function

		Private Function decodeGradient12(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color45, decodeColor(color45,color46,0.5f), color46})
		End Function


	End Class

End Namespace
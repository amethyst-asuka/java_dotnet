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



	Friend NotInheritable Class InternalFrameTitlePaneMaximizeButtonPainter
		Inherits AbstractRegionPainter

		'package private integers representing the available states that
		'this painter will paint. These are used when creating a new instance
		'of InternalFrameTitlePaneMaximizeButtonPainter to determine which region/state is being painted
		'by that instance.
		Friend Const BACKGROUND_DISABLED_WINDOWMAXIMIZED As Integer = 1
		Friend Const BACKGROUND_ENABLED_WINDOWMAXIMIZED As Integer = 2
		Friend Const BACKGROUND_MOUSEOVER_WINDOWMAXIMIZED As Integer = 3
		Friend Const BACKGROUND_PRESSED_WINDOWMAXIMIZED As Integer = 4
		Friend Const BACKGROUND_ENABLED_WINDOWNOTFOCUSED_WINDOWMAXIMIZED As Integer = 5
		Friend Const BACKGROUND_MOUSEOVER_WINDOWNOTFOCUSED_WINDOWMAXIMIZED As Integer = 6
		Friend Const BACKGROUND_PRESSED_WINDOWNOTFOCUSED_WINDOWMAXIMIZED As Integer = 7
		Friend Const BACKGROUND_DISABLED As Integer = 8
		Friend Const BACKGROUND_ENABLED As Integer = 9
		Friend Const BACKGROUND_MOUSEOVER As Integer = 10
		Friend Const BACKGROUND_PRESSED As Integer = 11
		Friend Const BACKGROUND_ENABLED_WINDOWNOTFOCUSED As Integer = 12
		Friend Const BACKGROUND_MOUSEOVER_WINDOWNOTFOCUSED As Integer = 13
		Friend Const BACKGROUND_PRESSED_WINDOWNOTFOCUSED As Integer = 14


		Private state As Integer 'refers to one of the static final ints above
		Private ctx As PaintContext

		'the following 4 variables are reused during the painting code of the layers
		Private path As Path2D = New Path2D.Float
		Private rect As Rectangle2D = New Rectangle2D.Float(0, 0, 0, 0)
		Private roundRect As RoundRectangle2D = New RoundRectangle2D.Float(0, 0, 0, 0, 0, 0)
		Private ellipse As Ellipse2D = New Ellipse2D.Float(0, 0, 0, 0)

		'All Colors used for painting are stored here. Ideally, only those colors being used
		'by a particular instance of InternalFrameTitlePaneMaximizeButtonPainter would be created. For the moment at least,
		'however, all are created for each instance.
		Private color1 As Color = decodeColor("nimbusGreen", 0.43362403f, -0.6792196f, 0.054901958f, 0)
		Private color2 As Color = decodeColor("nimbusGreen", 0.44056845f, -0.631913f, -0.039215684f, 0)
		Private color3 As Color = decodeColor("nimbusGreen", 0.44056845f, -0.67475206f, 0.06666666f, 0)
		Private color4 As New Color(255, 200, 0, 255)
		Private color5 As Color = decodeColor("nimbusGreen", 0.4355179f, -0.6581704f, -0.011764705f, 0)
		Private color6 As Color = decodeColor("nimbusGreen", 0.44484192f, -0.644647f, -0.031372547f, 0)
		Private color7 As Color = decodeColor("nimbusGreen", 0.44484192f, -0.6480447f, 0.0f, 0)
		Private color8 As Color = decodeColor("nimbusGreen", 0.4366002f, -0.6368381f, -0.04705882f, 0)
		Private color9 As Color = decodeColor("nimbusGreen", 0.44484192f, -0.6423572f, -0.05098039f, 0)
		Private color10 As Color = decodeColor("nimbusBlueGrey", 0.0055555105f, -0.062449392f, 0.07058823f, 0)
		Private color11 As Color = decodeColor("nimbusBlueGrey", -0.008547008f, -0.04174325f, -0.0039215684f, -13)
		Private color12 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.049920253f, 0.031372547f, 0)
		Private color13 As Color = decodeColor("nimbusBlueGrey", 0.0055555105f, -0.0029994324f, -0.38039216f, -185)
		Private color14 As Color = decodeColor("nimbusGreen", 0.1627907f, 0.2793296f, -0.6431373f, 0)
		Private color15 As Color = decodeColor("nimbusGreen", 0.025363803f, 0.2454313f, -0.2392157f, 0)
		Private color16 As Color = decodeColor("nimbusGreen", 0.02642706f, -0.3456704f, -0.011764705f, 0)
		Private color17 As Color = decodeColor("nimbusGreen", 0.025363803f, 0.2373128f, -0.23529413f, 0)
		Private color18 As Color = decodeColor("nimbusGreen", 0.025363803f, 0.0655365f, -0.13333333f, 0)
		Private color19 As Color = decodeColor("nimbusGreen", -0.0087068975f, -0.009330213f, -0.32156864f, 0)
		Private color20 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.110526316f, 0.25490195f, -13)
		Private color21 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.110526316f, 0.25490195f, -33)
		Private color22 As Color = decodeColor("nimbusGreen", 0.1627907f, 0.2793296f, -0.627451f, 0)
		Private color23 As Color = decodeColor("nimbusGreen", 0.04572721f, 0.2793296f, -0.37254903f, 0)
		Private color24 As Color = decodeColor("nimbusGreen", 0.009822637f, -0.34243205f, 0.054901958f, 0)
		Private color25 As Color = decodeColor("nimbusGreen", 0.010559708f, 0.13167858f, -0.11764705f, 0)
		Private color26 As Color = decodeColor("nimbusGreen", 0.010559708f, 0.12599629f, -0.11372548f, 0)
		Private color27 As Color = decodeColor("nimbusGreen", 0.010559708f, 9.2053413E-4f, -0.011764705f, 0)
		Private color28 As Color = decodeColor("nimbusGreen", 0.015249729f, 0.2793296f, -0.22352943f, -49)
		Private color29 As Color = decodeColor("nimbusGreen", 0.01279068f, 0.2793296f, -0.19215685f, 0)
		Private color30 As Color = decodeColor("nimbusGreen", 0.013319805f, 0.2793296f, -0.20784315f, 0)
		Private color31 As Color = decodeColor("nimbusGreen", 0.009604409f, 0.2793296f, -0.16862744f, 0)
		Private color32 As Color = decodeColor("nimbusGreen", 0.011600211f, 0.2793296f, -0.15294117f, 0)
		Private color33 As Color = decodeColor("nimbusGreen", 0.011939123f, 0.2793296f, -0.16470587f, 0)
		Private color34 As Color = decodeColor("nimbusGreen", 0.009506017f, 0.257901f, -0.15294117f, 0)
		Private color35 As Color = decodeColor("nimbusGreen", -0.17054264f, -0.7206704f, -0.7019608f, 0)
		Private color36 As Color = decodeColor("nimbusGreen", 0.07804492f, 0.2793296f, -0.47058827f, 0)
		Private color37 As Color = decodeColor("nimbusGreen", 0.03592503f, -0.23865601f, -0.15686274f, 0)
		Private color38 As Color = decodeColor("nimbusGreen", 0.035979107f, 0.23766291f, -0.3254902f, 0)
		Private color39 As Color = decodeColor("nimbusGreen", 0.03690417f, 0.2793296f, -0.33333334f, 0)
		Private color40 As Color = decodeColor("nimbusGreen", 0.09681849f, 0.2793296f, -0.5137255f, 0)
		Private color41 As Color = decodeColor("nimbusGreen", 0.06535478f, 0.2793296f, -0.44705883f, 0)
		Private color42 As Color = decodeColor("nimbusGreen", 0.0675526f, 0.2793296f, -0.454902f, 0)
		Private color43 As Color = decodeColor("nimbusGreen", 0.060800627f, 0.2793296f, -0.4392157f, 0)
		Private color44 As Color = decodeColor("nimbusGreen", 0.06419912f, 0.2793296f, -0.42352942f, 0)
		Private color45 As Color = decodeColor("nimbusGreen", 0.06375685f, 0.2793296f, -0.43137255f, 0)
		Private color46 As Color = decodeColor("nimbusGreen", 0.048207358f, 0.2793296f, -0.3882353f, 0)
		Private color47 As Color = decodeColor("nimbusGreen", 0.057156876f, 0.2793296f, -0.42352942f, 0)
		Private color48 As Color = decodeColor("nimbusGreen", 0.44056845f, -0.62133265f, -0.109803915f, 0)
		Private color49 As Color = decodeColor("nimbusGreen", 0.44056845f, -0.5843068f, -0.27058825f, 0)
		Private color50 As Color = decodeColor("nimbusGreen", 0.4294573f, -0.698349f, 0.17647058f, 0)
		Private color51 As Color = decodeColor("nimbusGreen", 0.45066953f, -0.665394f, 0.07843137f, 0)
		Private color52 As Color = decodeColor("nimbusGreen", 0.44056845f, -0.65913194f, 0.062745094f, 0)
		Private color53 As Color = decodeColor("nimbusGreen", 0.44056845f, -0.6609689f, 0.086274505f, 0)
		Private color54 As Color = decodeColor("nimbusGreen", 0.44056845f, -0.6578432f, 0.04705882f, 0)
		Private color55 As Color = decodeColor("nimbusGreen", 0.4355179f, -0.6633787f, 0.05098039f, 0)
		Private color56 As Color = decodeColor("nimbusGreen", 0.4355179f, -0.664548f, 0.06666666f, 0)
		Private color57 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.029445238f, -0.30980393f, -13)
		Private color58 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.027957506f, -0.31764707f, -33)
		Private color59 As Color = decodeColor("nimbusGreen", 0.43202144f, -0.64722407f, -0.007843137f, 0)
		Private color60 As Color = decodeColor("nimbusGreen", 0.44056845f, -0.6339652f, -0.02352941f, 0)
		Private color61 As New Color(165, 169, 176, 255)
		Private color62 As Color = decodeColor("nimbusBlueGrey", -0.00505054f, -0.057128258f, 0.062745094f, 0)
		Private color63 As Color = decodeColor("nimbusBlueGrey", -0.003968239f, -0.035257496f, -0.015686274f, 0)
		Private color64 As New Color(64, 88, 0, 255)
		Private color65 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.110526316f, 0.25490195f, 0)
		Private color66 As Color = decodeColor("nimbusBlueGrey", 0.004830897f, -0.00920473f, 0.14509803f, -101)
		Private color67 As Color = decodeColor("nimbusGreen", 0.009564877f, 0.100521624f, -0.109803915f, 0)
		Private color68 As New Color(113, 125, 0, 255)
		Private color69 As Color = decodeColor("nimbusBlueGrey", 0.0025252104f, -0.0067527294f, 0.086274505f, -65)
		Private color70 As Color = decodeColor("nimbusGreen", 0.03129223f, 0.2793296f, -0.27450982f, 0)
		Private color71 As New Color(19, 48, 0, 255)
		Private color72 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.029445238f, -0.30980393f, 0)


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
				Case BACKGROUND_DISABLED_WINDOWMAXIMIZED
					paintBackgroundDisabledAndWindowMaximized(g)
				Case BACKGROUND_ENABLED_WINDOWMAXIMIZED
					paintBackgroundEnabledAndWindowMaximized(g)
				Case BACKGROUND_MOUSEOVER_WINDOWMAXIMIZED
					paintBackgroundMouseOverAndWindowMaximized(g)
				Case BACKGROUND_PRESSED_WINDOWMAXIMIZED
					paintBackgroundPressedAndWindowMaximized(g)
				Case BACKGROUND_ENABLED_WINDOWNOTFOCUSED_WINDOWMAXIMIZED
					paintBackgroundEnabledAndWindowNotFocusedAndWindowMaximized(g)
				Case BACKGROUND_MOUSEOVER_WINDOWNOTFOCUSED_WINDOWMAXIMIZED
					paintBackgroundMouseOverAndWindowNotFocusedAndWindowMaximized(g)
				Case BACKGROUND_PRESSED_WINDOWNOTFOCUSED_WINDOWMAXIMIZED
					paintBackgroundPressedAndWindowNotFocusedAndWindowMaximized(g)
				Case BACKGROUND_DISABLED
					paintBackgroundDisabled(g)
				Case BACKGROUND_ENABLED
					paintBackgroundEnabled(g)
				Case BACKGROUND_MOUSEOVER
					paintBackgroundMouseOver(g)
				Case BACKGROUND_PRESSED
					paintBackgroundPressed(g)
				Case BACKGROUND_ENABLED_WINDOWNOTFOCUSED
					paintBackgroundEnabledAndWindowNotFocused(g)
				Case BACKGROUND_MOUSEOVER_WINDOWNOTFOCUSED
					paintBackgroundMouseOverAndWindowNotFocused(g)
				Case BACKGROUND_PRESSED_WINDOWNOTFOCUSED
					paintBackgroundPressedAndWindowNotFocused(g)

			End Select
		End Sub



		Protected Friend Property NotOverridable Overrides paintContext As PaintContext
			Get
				Return ctx
			End Get
		End Property

		Private Sub paintBackgroundDisabledAndWindowMaximized(ByVal g As Graphics2D)
			roundRect = decodeRoundRect1()
			g.paint = decodeGradient1(roundRect)
			g.fill(roundRect)
			roundRect = decodeRoundRect2()
			g.paint = decodeGradient2(roundRect)
			g.fill(roundRect)
			rect = decodeRect1()
			g.paint = color4
			g.fill(rect)
			rect = decodeRect2()
			g.paint = color5
			g.fill(rect)
			rect = decodeRect3()
			g.paint = color6
			g.fill(rect)
			rect = decodeRect4()
			g.paint = color6
			g.fill(rect)
			rect = decodeRect5()
			g.paint = color7
			g.fill(rect)
			rect = decodeRect6()
			g.paint = color8
			g.fill(rect)
			rect = decodeRect7()
			g.paint = color9
			g.fill(rect)
			rect = decodeRect8()
			g.paint = color7
			g.fill(rect)
			path = decodePath1()
			g.paint = decodeGradient3(path)
			g.fill(path)
			path = decodePath2()
			g.paint = color12
			g.fill(path)

		End Sub

		Private Sub paintBackgroundEnabledAndWindowMaximized(ByVal g As Graphics2D)
			roundRect = decodeRoundRect3()
			g.paint = color13
			g.fill(roundRect)
			roundRect = decodeRoundRect1()
			g.paint = decodeGradient4(roundRect)
			g.fill(roundRect)
			roundRect = decodeRoundRect2()
			g.paint = decodeGradient5(roundRect)
			g.fill(roundRect)
			rect = decodeRect1()
			g.paint = color4
			g.fill(rect)
			rect = decodeRect2()
			g.paint = color19
			g.fill(rect)
			rect = decodeRect3()
			g.paint = color19
			g.fill(rect)
			rect = decodeRect4()
			g.paint = color19
			g.fill(rect)
			rect = decodeRect5()
			g.paint = color19
			g.fill(rect)
			rect = decodeRect9()
			g.paint = color19
			g.fill(rect)
			rect = decodeRect7()
			g.paint = color19
			g.fill(rect)
			rect = decodeRect10()
			g.paint = color19
			g.fill(rect)
			rect = decodeRect8()
			g.paint = color19
			g.fill(rect)
			path = decodePath1()
			g.paint = color20
			g.fill(path)
			path = decodePath2()
			g.paint = color21
			g.fill(path)

		End Sub

		Private Sub paintBackgroundMouseOverAndWindowMaximized(ByVal g As Graphics2D)
			roundRect = decodeRoundRect3()
			g.paint = color13
			g.fill(roundRect)
			roundRect = decodeRoundRect1()
			g.paint = decodeGradient6(roundRect)
			g.fill(roundRect)
			roundRect = decodeRoundRect2()
			g.paint = decodeGradient7(roundRect)
			g.fill(roundRect)
			rect = decodeRect1()
			g.paint = color4
			g.fill(rect)
			rect = decodeRect2()
			g.paint = color28
			g.fill(rect)
			rect = decodeRect3()
			g.paint = color29
			g.fill(rect)
			rect = decodeRect4()
			g.paint = color30
			g.fill(rect)
			rect = decodeRect5()
			g.paint = color31
			g.fill(rect)
			rect = decodeRect9()
			g.paint = color32
			g.fill(rect)
			rect = decodeRect7()
			g.paint = color33
			g.fill(rect)
			rect = decodeRect10()
			g.paint = color34
			g.fill(rect)
			rect = decodeRect8()
			g.paint = color31
			g.fill(rect)
			path = decodePath1()
			g.paint = color20
			g.fill(path)
			path = decodePath2()
			g.paint = color21
			g.fill(path)

		End Sub

		Private Sub paintBackgroundPressedAndWindowMaximized(ByVal g As Graphics2D)
			roundRect = decodeRoundRect3()
			g.paint = color13
			g.fill(roundRect)
			roundRect = decodeRoundRect1()
			g.paint = decodeGradient8(roundRect)
			g.fill(roundRect)
			roundRect = decodeRoundRect2()
			g.paint = decodeGradient9(roundRect)
			g.fill(roundRect)
			rect = decodeRect1()
			g.paint = color4
			g.fill(rect)
			rect = decodeRect2()
			g.paint = color40
			g.fill(rect)
			rect = decodeRect3()
			g.paint = color41
			g.fill(rect)
			rect = decodeRect4()
			g.paint = color42
			g.fill(rect)
			rect = decodeRect5()
			g.paint = color43
			g.fill(rect)
			rect = decodeRect6()
			g.paint = color44
			g.fill(rect)
			rect = decodeRect7()
			g.paint = color45
			g.fill(rect)
			rect = decodeRect10()
			g.paint = color46
			g.fill(rect)
			rect = decodeRect8()
			g.paint = color47
			g.fill(rect)
			path = decodePath1()
			g.paint = color20
			g.fill(path)
			path = decodePath2()
			g.paint = color21
			g.fill(path)

		End Sub

		Private Sub paintBackgroundEnabledAndWindowNotFocusedAndWindowMaximized(ByVal g As Graphics2D)
			roundRect = decodeRoundRect1()
			g.paint = decodeGradient10(roundRect)
			g.fill(roundRect)
			roundRect = decodeRoundRect2()
			g.paint = decodeGradient11(roundRect)
			g.fill(roundRect)
			rect = decodeRect1()
			g.paint = color4
			g.fill(rect)
			rect = decodeRect4()
			g.paint = color54
			g.fill(rect)
			rect = decodeRect5()
			g.paint = color55
			g.fill(rect)
			rect = decodeRect8()
			g.paint = color56
			g.fill(rect)
			path = decodePath1()
			g.paint = color57
			g.fill(path)
			path = decodePath2()
			g.paint = color58
			g.fill(path)

		End Sub

		Private Sub paintBackgroundMouseOverAndWindowNotFocusedAndWindowMaximized(ByVal g As Graphics2D)
			roundRect = decodeRoundRect3()
			g.paint = color13
			g.fill(roundRect)
			roundRect = decodeRoundRect1()
			g.paint = decodeGradient6(roundRect)
			g.fill(roundRect)
			roundRect = decodeRoundRect2()
			g.paint = decodeGradient7(roundRect)
			g.fill(roundRect)
			rect = decodeRect1()
			g.paint = color4
			g.fill(rect)
			rect = decodeRect2()
			g.paint = color28
			g.fill(rect)
			rect = decodeRect3()
			g.paint = color29
			g.fill(rect)
			rect = decodeRect4()
			g.paint = color30
			g.fill(rect)
			rect = decodeRect5()
			g.paint = color31
			g.fill(rect)
			rect = decodeRect9()
			g.paint = color32
			g.fill(rect)
			rect = decodeRect7()
			g.paint = color33
			g.fill(rect)
			rect = decodeRect10()
			g.paint = color34
			g.fill(rect)
			rect = decodeRect8()
			g.paint = color31
			g.fill(rect)
			path = decodePath1()
			g.paint = color20
			g.fill(path)
			path = decodePath2()
			g.paint = color21
			g.fill(path)

		End Sub

		Private Sub paintBackgroundPressedAndWindowNotFocusedAndWindowMaximized(ByVal g As Graphics2D)
			roundRect = decodeRoundRect3()
			g.paint = color13
			g.fill(roundRect)
			roundRect = decodeRoundRect1()
			g.paint = decodeGradient8(roundRect)
			g.fill(roundRect)
			roundRect = decodeRoundRect2()
			g.paint = decodeGradient9(roundRect)
			g.fill(roundRect)
			rect = decodeRect1()
			g.paint = color4
			g.fill(rect)
			rect = decodeRect2()
			g.paint = color40
			g.fill(rect)
			rect = decodeRect3()
			g.paint = color41
			g.fill(rect)
			rect = decodeRect4()
			g.paint = color42
			g.fill(rect)
			rect = decodeRect5()
			g.paint = color43
			g.fill(rect)
			rect = decodeRect6()
			g.paint = color44
			g.fill(rect)
			rect = decodeRect7()
			g.paint = color45
			g.fill(rect)
			rect = decodeRect10()
			g.paint = color46
			g.fill(rect)
			rect = decodeRect8()
			g.paint = color47
			g.fill(rect)
			path = decodePath1()
			g.paint = color20
			g.fill(path)
			path = decodePath2()
			g.paint = color21
			g.fill(path)

		End Sub

		Private Sub paintBackgroundDisabled(ByVal g As Graphics2D)
			roundRect = decodeRoundRect1()
			g.paint = decodeGradient1(roundRect)
			g.fill(roundRect)
			roundRect = decodeRoundRect2()
			g.paint = decodeGradient12(roundRect)
			g.fill(roundRect)
			rect = decodeRect1()
			g.paint = color4
			g.fill(rect)
			path = decodePath3()
			g.paint = color61
			g.fill(path)
			path = decodePath4()
			g.paint = decodeGradient13(path)
			g.fill(path)

		End Sub

		Private Sub paintBackgroundEnabled(ByVal g As Graphics2D)
			roundRect = decodeRoundRect3()
			g.paint = color13
			g.fill(roundRect)
			roundRect = decodeRoundRect1()
			g.paint = decodeGradient4(roundRect)
			g.fill(roundRect)
			roundRect = decodeRoundRect2()
			g.paint = decodeGradient5(roundRect)
			g.fill(roundRect)
			rect = decodeRect1()
			g.paint = color4
			g.fill(rect)
			path = decodePath3()
			g.paint = color64
			g.fill(path)
			path = decodePath4()
			g.paint = color65
			g.fill(path)

		End Sub

		Private Sub paintBackgroundMouseOver(ByVal g As Graphics2D)
			roundRect = decodeRoundRect3()
			g.paint = color66
			g.fill(roundRect)
			roundRect = decodeRoundRect1()
			g.paint = decodeGradient6(roundRect)
			g.fill(roundRect)
			roundRect = decodeRoundRect2()
			g.paint = decodeGradient14(roundRect)
			g.fill(roundRect)
			rect = decodeRect1()
			g.paint = color4
			g.fill(rect)
			path = decodePath3()
			g.paint = color68
			g.fill(path)
			path = decodePath4()
			g.paint = color65
			g.fill(path)

		End Sub

		Private Sub paintBackgroundPressed(ByVal g As Graphics2D)
			roundRect = decodeRoundRect3()
			g.paint = color69
			g.fill(roundRect)
			roundRect = decodeRoundRect1()
			g.paint = decodeGradient8(roundRect)
			g.fill(roundRect)
			roundRect = decodeRoundRect2()
			g.paint = decodeGradient15(roundRect)
			g.fill(roundRect)
			rect = decodeRect1()
			g.paint = color4
			g.fill(rect)
			path = decodePath3()
			g.paint = color71
			g.fill(path)
			path = decodePath4()
			g.paint = color65
			g.fill(path)

		End Sub

		Private Sub paintBackgroundEnabledAndWindowNotFocused(ByVal g As Graphics2D)
			roundRect = decodeRoundRect1()
			g.paint = decodeGradient10(roundRect)
			g.fill(roundRect)
			roundRect = decodeRoundRect2()
			g.paint = decodeGradient16(roundRect)
			g.fill(roundRect)
			rect = decodeRect1()
			g.paint = color4
			g.fill(rect)
			path = decodePath4()
			g.paint = color72
			g.fill(path)

		End Sub

		Private Sub paintBackgroundMouseOverAndWindowNotFocused(ByVal g As Graphics2D)
			roundRect = decodeRoundRect3()
			g.paint = color66
			g.fill(roundRect)
			roundRect = decodeRoundRect1()
			g.paint = decodeGradient6(roundRect)
			g.fill(roundRect)
			roundRect = decodeRoundRect2()
			g.paint = decodeGradient14(roundRect)
			g.fill(roundRect)
			rect = decodeRect1()
			g.paint = color4
			g.fill(rect)
			path = decodePath3()
			g.paint = color68
			g.fill(path)
			path = decodePath4()
			g.paint = color65
			g.fill(path)

		End Sub

		Private Sub paintBackgroundPressedAndWindowNotFocused(ByVal g As Graphics2D)
			roundRect = decodeRoundRect3()
			g.paint = color69
			g.fill(roundRect)
			roundRect = decodeRoundRect1()
			g.paint = decodeGradient8(roundRect)
			g.fill(roundRect)
			roundRect = decodeRoundRect2()
			g.paint = decodeGradient15(roundRect)
			g.fill(roundRect)
			rect = decodeRect1()
			g.paint = color4
			g.fill(rect)
			path = decodePath3()
			g.paint = color71
			g.fill(path)
			path = decodePath4()
			g.paint = color65
			g.fill(path)

		End Sub



		Private Function decodeRoundRect1() As RoundRectangle2D
			roundRect.roundRectect(decodeX(1.0f), decodeY(1.0f), decodeX(2.0f) - decodeX(1.0f), decodeY(1.9444444f) - decodeY(1.0f), 8.6f, 8.6f) 'rounding - height - width - y - x
			Return roundRect
		End Function

		Private Function decodeRoundRect2() As RoundRectangle2D
			roundRect.roundRectect(decodeX(1.0526316f), decodeY(1.0555556f), decodeX(1.9473684f) - decodeX(1.0526316f), decodeY(1.8888888f) - decodeY(1.0555556f), 6.75f, 6.75f) 'rounding - height - width - y - x
			Return roundRect
		End Function

		Private Function decodeRect1() As Rectangle2D
				rect.rectect(decodeX(1.0f), decodeY(1.0f), decodeX(1.0f) - decodeX(1.0f), decodeY(1.0f) - decodeY(1.0f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodeRect2() As Rectangle2D
				rect.rectect(decodeX(1.2165072f), decodeY(1.2790405f), decodeX(1.6746411f) - decodeX(1.2165072f), decodeY(1.3876263f) - decodeY(1.2790405f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodeRect3() As Rectangle2D
				rect.rectect(decodeX(1.2212919f), decodeY(1.6047981f), decodeX(1.270335f) - decodeX(1.2212919f), decodeY(1.3876263f) - decodeY(1.6047981f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodeRect4() As Rectangle2D
				rect.rectect(decodeX(1.2643541f), decodeY(1.5542929f), decodeX(1.6315789f) - decodeX(1.2643541f), decodeY(1.5997474f) - decodeY(1.5542929f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodeRect5() As Rectangle2D
				rect.rectect(decodeX(1.6267943f), decodeY(1.3888888f), decodeX(1.673445f) - decodeX(1.6267943f), decodeY(1.6085858f) - decodeY(1.3888888f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodeRect6() As Rectangle2D
				rect.rectect(decodeX(1.3684211f), decodeY(1.6111112f), decodeX(1.4210527f) - decodeX(1.3684211f), decodeY(1.7777778f) - decodeY(1.6111112f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodeRect7() As Rectangle2D
				rect.rectect(decodeX(1.4389952f), decodeY(1.7209597f), decodeX(1.7882775f) - decodeX(1.4389952f), decodeY(1.7765152f) - decodeY(1.7209597f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodeRect8() As Rectangle2D
				rect.rectect(decodeX(1.5645933f), decodeY(1.4078283f), decodeX(1.7870812f) - decodeX(1.5645933f), decodeY(1.5239899f) - decodeY(1.4078283f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodePath1() As Path2D
			path.reset()
			path.moveTo(decodeX(1.2105263f), decodeY(1.2222222f))
			path.lineTo(decodeX(1.6315789f), decodeY(1.2222222f))
			path.lineTo(decodeX(1.6315789f), decodeY(1.5555556f))
			path.lineTo(decodeX(1.2105263f), decodeY(1.5555556f))
			path.lineTo(decodeX(1.2105263f), decodeY(1.3333334f))
			path.lineTo(decodeX(1.2631578f), decodeY(1.3333334f))
			path.lineTo(decodeX(1.2631578f), decodeY(1.5f))
			path.lineTo(decodeX(1.5789473f), decodeY(1.5f))
			path.lineTo(decodeX(1.5789473f), decodeY(1.3333334f))
			path.lineTo(decodeX(1.2105263f), decodeY(1.3333334f))
			path.lineTo(decodeX(1.2105263f), decodeY(1.2222222f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath2() As Path2D
			path.reset()
			path.moveTo(decodeX(1.6842105f), decodeY(1.3888888f))
			path.lineTo(decodeX(1.6842105f), decodeY(1.5f))
			path.lineTo(decodeX(1.7368422f), decodeY(1.5f))
			path.lineTo(decodeX(1.7368422f), decodeY(1.6666667f))
			path.lineTo(decodeX(1.4210527f), decodeY(1.6666667f))
			path.lineTo(decodeX(1.4210527f), decodeY(1.6111112f))
			path.lineTo(decodeX(1.3684211f), decodeY(1.6111112f))
			path.lineTo(decodeX(1.3684211f), decodeY(1.7222222f))
			path.lineTo(decodeX(1.7894738f), decodeY(1.7222222f))
			path.lineTo(decodeX(1.7894738f), decodeY(1.3888888f))
			path.lineTo(decodeX(1.6842105f), decodeY(1.3888888f))
			path.closePath()
			Return path
		End Function

		Private Function decodeRoundRect3() As RoundRectangle2D
			roundRect.roundRectect(decodeX(1.0f), decodeY(1.6111112f), decodeX(2.0f) - decodeX(1.0f), decodeY(2.0f) - decodeY(1.6111112f), 6.0f, 6.0f) 'rounding - height - width - y - x
			Return roundRect
		End Function

		Private Function decodeRect9() As Rectangle2D
				rect.rectect(decodeX(1.3815789f), decodeY(1.6111112f), decodeX(1.4366028f) - decodeX(1.3815789f), decodeY(1.7739899f) - decodeY(1.6111112f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodeRect10() As Rectangle2D
				rect.rectect(decodeX(1.7918661f), decodeY(1.7752526f), decodeX(1.8349283f) - decodeX(1.7918661f), decodeY(1.4217172f) - decodeY(1.7752526f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodePath3() As Path2D
			path.reset()
			path.moveTo(decodeX(1.1913875f), decodeY(1.2916666f))
			path.lineTo(decodeX(1.1925838f), decodeY(1.7462121f))
			path.lineTo(decodeX(1.8157895f), decodeY(1.7449496f))
			path.lineTo(decodeX(1.819378f), decodeY(1.2916666f))
			path.lineTo(decodeX(1.722488f), decodeY(1.2916666f))
			path.lineTo(decodeX(1.7320573f), decodeY(1.669192f))
			path.lineTo(decodeX(1.2799044f), decodeY(1.6565657f))
			path.lineTo(decodeX(1.284689f), decodeY(1.3863636f))
			path.lineTo(decodeX(1.7260766f), decodeY(1.385101f))
			path.lineTo(decodeX(1.722488f), decodeY(1.2904041f))
			path.lineTo(decodeX(1.1913875f), decodeY(1.2916666f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath4() As Path2D
			path.reset()
			path.moveTo(decodeX(1.2105263f), decodeY(1.2222222f))
			path.lineTo(decodeX(1.2105263f), decodeY(1.7222222f))
			path.lineTo(decodeX(1.7894738f), decodeY(1.7222222f))
			path.lineTo(decodeX(1.7894738f), decodeY(1.3333334f))
			path.lineTo(decodeX(1.7368422f), decodeY(1.3333334f))
			path.lineTo(decodeX(1.7368422f), decodeY(1.6666667f))
			path.lineTo(decodeX(1.2631578f), decodeY(1.6666667f))
			path.lineTo(decodeX(1.2631578f), decodeY(1.3333334f))
			path.lineTo(decodeX(1.7894738f), decodeY(1.3333334f))
			path.lineTo(decodeX(1.7894738f), decodeY(1.2222222f))
			path.lineTo(decodeX(1.2105263f), decodeY(1.2222222f))
			path.closePath()
			Return path
		End Function



		Private Function decodeGradient1(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.24868421f * w) + x, (0.0014705883f * h) + y, (0.24868421f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color1, decodeColor(color1,color2,0.5f), color2})
		End Function

		Private Function decodeGradient2(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.25f * w) + x, (0.0f * h) + y, (0.25441176f * w) + x, (1.0016667f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color3, decodeColor(color3,color2,0.5f), color2})
		End Function

		Private Function decodeGradient3(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color10, decodeColor(color10,color11,0.5f), color11})
		End Function

		Private Function decodeGradient4(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.24868421f * w) + x, (0.0014705883f * h) + y, (0.24868421f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color14, decodeColor(color14,color15,0.5f), color15})
		End Function

		Private Function decodeGradient5(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.25f * w) + x, (0.0f * h) + y, (0.25441176f * w) + x, (1.0016667f * h) + y, New Single() { 0.0f,0.26988637f,0.53977275f,0.5951705f,0.6505682f,0.8252841f,1.0f }, New Color() { color16, decodeColor(color16,color15,0.5f), color15, decodeColor(color15,color17,0.5f), color17, decodeColor(color17,color18,0.5f), color18})
		End Function

		Private Function decodeGradient6(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.24868421f * w) + x, (0.0014705883f * h) + y, (0.24868421f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color22, decodeColor(color22,color23,0.5f), color23})
		End Function

		Private Function decodeGradient7(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.25f * w) + x, (0.0f * h) + y, (0.25441176f * w) + x, (1.0016667f * h) + y, New Single() { 0.0f,0.26988637f,0.53977275f,0.5951705f,0.6505682f,0.8252841f,1.0f }, New Color() { color24, decodeColor(color24,color25,0.5f), color25, decodeColor(color25,color26,0.5f), color26, decodeColor(color26,color27,0.5f), color27})
		End Function

		Private Function decodeGradient8(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.24868421f * w) + x, (0.0014705883f * h) + y, (0.24868421f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color35, decodeColor(color35,color36,0.5f), color36})
		End Function

		Private Function decodeGradient9(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.25f * w) + x, (0.0f * h) + y, (0.25441176f * w) + x, (1.0016667f * h) + y, New Single() { 0.0f,0.26988637f,0.53977275f,0.5951705f,0.6505682f,0.8252841f,1.0f }, New Color() { color37, decodeColor(color37,color38,0.5f), color38, decodeColor(color38,color39,0.5f), color39, decodeColor(color39,color18,0.5f), color18})
		End Function

		Private Function decodeGradient10(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.24868421f * w) + x, (0.0014705883f * h) + y, (0.24868421f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color48, decodeColor(color48,color49,0.5f), color49})
		End Function

		Private Function decodeGradient11(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.25f * w) + x, (0.0f * h) + y, (0.25441176f * w) + x, (1.0016667f * h) + y, New Single() { 0.0f,0.26988637f,0.53977275f,0.5951705f,0.6505682f,0.8252841f,1.0f }, New Color() { color50, decodeColor(color50,color51,0.5f), color51, decodeColor(color51,color52,0.5f), color52, decodeColor(color52,color53,0.5f), color53})
		End Function

		Private Function decodeGradient12(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.25f * w) + x, (0.0f * h) + y, (0.25441176f * w) + x, (1.0016667f * h) + y, New Single() { 0.0f,0.26988637f,0.53977275f,0.6082097f,0.6766467f,0.83832335f,1.0f }, New Color() { color3, decodeColor(color3,color59,0.5f), color59, decodeColor(color59,color60,0.5f), color60, decodeColor(color60,color2,0.5f), color2})
		End Function

		Private Function decodeGradient13(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.26047903f,0.6302395f,1.0f }, New Color() { color62, decodeColor(color62,color63,0.5f), color63})
		End Function

		Private Function decodeGradient14(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.25f * w) + x, (0.0f * h) + y, (0.25441176f * w) + x, (1.0016667f * h) + y, New Single() { 0.0f,0.26988637f,0.53977275f,0.5951705f,0.6505682f,0.8252841f,1.0f }, New Color() { color24, decodeColor(color24,color67,0.5f), color67, decodeColor(color67,color25,0.5f), color25, decodeColor(color25,color27,0.5f), color27})
		End Function

		Private Function decodeGradient15(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.25f * w) + x, (0.0f * h) + y, (0.25441176f * w) + x, (1.0016667f * h) + y, New Single() { 0.0f,0.26988637f,0.53977275f,0.66659296f,0.79341316f,0.8967066f,1.0f }, New Color() { color37, decodeColor(color37,color38,0.5f), color38, decodeColor(color38,color39,0.5f), color39, decodeColor(color39,color70,0.5f), color70})
		End Function

		Private Function decodeGradient16(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.25f * w) + x, (0.0f * h) + y, (0.25441176f * w) + x, (1.0016667f * h) + y, New Single() { 0.0f,0.26988637f,0.53977275f,0.6291678f,0.7185629f,0.8592814f,1.0f }, New Color() { color50, decodeColor(color50,color52,0.5f), color52, decodeColor(color52,color52,0.5f), color52, decodeColor(color52,color53,0.5f), color53})
		End Function


	End Class

End Namespace
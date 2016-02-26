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



	Friend NotInheritable Class TreePainter
		Inherits AbstractRegionPainter

		'package private integers representing the available states that
		'this painter will paint. These are used when creating a new instance
		'of TreePainter to determine which region/state is being painted
		'by that instance.
		Friend Const BACKGROUND_DISABLED As Integer = 1
		Friend Const BACKGROUND_ENABLED As Integer = 2
		Friend Const BACKGROUND_ENABLED_SELECTED As Integer = 3
		Friend Const LEAFICON_ENABLED As Integer = 4
		Friend Const CLOSEDICON_ENABLED As Integer = 5
		Friend Const OPENICON_ENABLED As Integer = 6
		Friend Const COLLAPSEDICON_ENABLED As Integer = 7
		Friend Const COLLAPSEDICON_ENABLED_SELECTED As Integer = 8
		Friend Const EXPANDEDICON_ENABLED As Integer = 9
		Friend Const EXPANDEDICON_ENABLED_SELECTED As Integer = 10


		Private ___state As Integer 'refers to one of the static final ints above
		Private ctx As PaintContext

		'the following 4 variables are reused during the painting code of the layers
		Private path As Path2D = New Path2D.Float
		Private rect As Rectangle2D = New Rectangle2D.Float(0, 0, 0, 0)
		Private roundRect As RoundRectangle2D = New RoundRectangle2D.Float(0, 0, 0, 0, 0, 0)
		Private ellipse As Ellipse2D = New Ellipse2D.Float(0, 0, 0, 0)

		'All Colors used for painting are stored here. Ideally, only those colors being used
		'by a particular instance of TreePainter would be created. For the moment at least,
		'however, all are created for each instance.
		Private color1 As Color = decodeColor("nimbusBlueGrey", 0.007936537f, -0.065654516f, -0.13333333f, 0)
		Private color2 As New Color(97, 98, 102, 255)
		Private color3 As Color = decodeColor("nimbusBlueGrey", -0.032679737f, -0.043332636f, 0.24705881f, 0)
		Private color4 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.110526316f, 0.25490195f, 0)
		Private color5 As Color = decodeColor("nimbusBase", 0.0077680945f, -0.51781034f, 0.3490196f, 0)
		Private color6 As Color = decodeColor("nimbusBase", 0.013940871f, -0.599277f, 0.41960782f, 0)
		Private color7 As Color = decodeColor("nimbusBase", 0.004681647f, -0.4198052f, 0.14117646f, 0)
		Private color8 As Color = decodeColor("nimbusBase", 0.0f, -0.6357143f, 0.45098037f, -127)
		Private color9 As Color = decodeColor("nimbusBlueGrey", 0.0f, 0.0f, -0.21f, -99)
		Private color10 As Color = decodeColor("nimbusBase", 2.9569864E-4f, -0.45978838f, 0.2980392f, 0)
		Private color11 As Color = decodeColor("nimbusBase", 0.0015952587f, -0.34848025f, 0.18823528f, 0)
		Private color12 As Color = decodeColor("nimbusBase", 0.0015952587f, -0.30844158f, 0.09803921f, 0)
		Private color13 As Color = decodeColor("nimbusBase", 0.0015952587f, -0.27329817f, 0.035294116f, 0)
		Private color14 As Color = decodeColor("nimbusBase", 0.004681647f, -0.6198413f, 0.43921566f, 0)
		Private color15 As Color = decodeColor("nimbusBase", 0.0f, -0.6357143f, 0.45098037f, -125)
		Private color16 As Color = decodeColor("nimbusBase", 0.0f, -0.6357143f, 0.45098037f, -50)
		Private color17 As Color = decodeColor("nimbusBase", 0.0f, -0.6357143f, 0.45098037f, -100)
		Private color18 As Color = decodeColor("nimbusBase", 0.0012094378f, -0.23571429f, -0.0784314f, 0)
		Private color19 As Color = decodeColor("nimbusBase", 2.9569864E-4f, -0.115166366f, -0.2627451f, 0)
		Private color20 As Color = decodeColor("nimbusBase", 0.0027436614f, -0.335015f, 0.011764705f, 0)
		Private color21 As Color = decodeColor("nimbusBase", 0.0024294257f, -0.3857143f, 0.031372547f, 0)
		Private color22 As Color = decodeColor("nimbusBase", 0.0018081069f, -0.3595238f, -0.13725492f, 0)
		Private color23 As New Color(255, 200, 0, 255)
		Private color24 As Color = decodeColor("nimbusBase", 0.004681647f, -0.33496243f, -0.027450979f, 0)
		Private color25 As Color = decodeColor("nimbusBase", 0.0019934773f, -0.361378f, -0.10588238f, 0)
		Private color26 As Color = decodeColor("nimbusBlueGrey", -0.6111111f, -0.110526316f, -0.34509805f, 0)


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
				Case LEAFICON_ENABLED
					paintleafIconEnabled(g)
				Case CLOSEDICON_ENABLED
					paintclosedIconEnabled(g)
				Case OPENICON_ENABLED
					paintopenIconEnabled(g)
				Case COLLAPSEDICON_ENABLED
					paintcollapsedIconEnabled(g)
				Case COLLAPSEDICON_ENABLED_SELECTED
					paintcollapsedIconEnabledAndSelected(g)
				Case EXPANDEDICON_ENABLED
					paintexpandedIconEnabled(g)
				Case EXPANDEDICON_ENABLED_SELECTED
					paintexpandedIconEnabledAndSelected(g)

			End Select
		End Sub



		Protected Friend Property NotOverridable Overrides paintContext As PaintContext
			Get
				Return ctx
			End Get
		End Property

		Private Sub paintleafIconEnabled(ByVal g As Graphics2D)
			path = decodePath1()
			g.paint = color1
			g.fill(path)
			rect = decodeRect1()
			g.paint = color2
			g.fill(rect)
			path = decodePath2()
			g.paint = decodeGradient1(path)
			g.fill(path)
			path = decodePath3()
			g.paint = decodeGradient2(path)
			g.fill(path)
			path = decodePath4()
			g.paint = color7
			g.fill(path)
			path = decodePath5()
			g.paint = color8
			g.fill(path)

		End Sub

		Private Sub paintclosedIconEnabled(ByVal g As Graphics2D)
			path = decodePath6()
			g.paint = color9
			g.fill(path)
			path = decodePath7()
			g.paint = decodeGradient3(path)
			g.fill(path)
			path = decodePath8()
			g.paint = decodeGradient4(path)
			g.fill(path)
			rect = decodeRect2()
			g.paint = color15
			g.fill(rect)
			rect = decodeRect3()
			g.paint = color16
			g.fill(rect)
			rect = decodeRect4()
			g.paint = color17
			g.fill(rect)
			path = decodePath9()
			g.paint = decodeGradient5(path)
			g.fill(path)
			path = decodePath10()
			g.paint = decodeGradient6(path)
			g.fill(path)
			path = decodePath11()
			g.paint = color23
			g.fill(path)

		End Sub

		Private Sub paintopenIconEnabled(ByVal g As Graphics2D)
			path = decodePath6()
			g.paint = color9
			g.fill(path)
			path = decodePath12()
			g.paint = decodeGradient3(path)
			g.fill(path)
			path = decodePath13()
			g.paint = decodeGradient4(path)
			g.fill(path)
			rect = decodeRect2()
			g.paint = color15
			g.fill(rect)
			rect = decodeRect3()
			g.paint = color16
			g.fill(rect)
			rect = decodeRect4()
			g.paint = color17
			g.fill(rect)
			path = decodePath14()
			g.paint = decodeGradient5(path)
			g.fill(path)
			path = decodePath15()
			g.paint = decodeGradient7(path)
			g.fill(path)
			path = decodePath11()
			g.paint = color23
			g.fill(path)

		End Sub

		Private Sub paintcollapsedIconEnabled(ByVal g As Graphics2D)
			path = decodePath16()
			g.paint = color26
			g.fill(path)

		End Sub

		Private Sub paintcollapsedIconEnabledAndSelected(ByVal g As Graphics2D)
			path = decodePath16()
			g.paint = color4
			g.fill(path)

		End Sub

		Private Sub paintexpandedIconEnabled(ByVal g As Graphics2D)
			path = decodePath17()
			g.paint = color26
			g.fill(path)

		End Sub

		Private Sub paintexpandedIconEnabledAndSelected(ByVal g As Graphics2D)
			path = decodePath17()
			g.paint = color4
			g.fill(path)

		End Sub



		Private Function decodePath1() As Path2D
			path.reset()
			path.moveTo(decodeX(0.2f), decodeY(0.0f))
			path.lineTo(decodeX(0.2f), decodeY(3.0f))
			path.lineTo(decodeX(0.4f), decodeY(3.0f))
			path.lineTo(decodeX(0.4f), decodeY(0.2f))
			path.lineTo(decodeX(1.9197531f), decodeY(0.2f))
			path.lineTo(decodeX(2.6f), decodeY(0.9f))
			path.lineTo(decodeX(2.6f), decodeY(3.0f))
			path.lineTo(decodeX(2.8f), decodeY(3.0f))
			path.lineTo(decodeX(2.8f), decodeY(0.88888896f))
			path.lineTo(decodeX(1.9537036f), decodeY(0.0f))
			path.lineTo(decodeX(0.2f), decodeY(0.0f))
			path.closePath()
			Return path
		End Function

		Private Function decodeRect1() As Rectangle2D
				rect.rectect(decodeX(0.4f), decodeY(2.8f), decodeX(2.6f) - decodeX(0.4f), decodeY(3.0f) - decodeY(2.8f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodePath2() As Path2D
			path.reset()
			path.moveTo(decodeX(1.8333333f), decodeY(0.2f))
			path.lineTo(decodeX(1.8333333f), decodeY(1.0f))
			path.lineTo(decodeX(2.6f), decodeY(1.0f))
			path.lineTo(decodeX(1.8333333f), decodeY(0.2f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath3() As Path2D
			path.reset()
			path.moveTo(decodeX(1.8333333f), decodeY(0.2f))
			path.lineTo(decodeX(0.4f), decodeY(0.2f))
			path.lineTo(decodeX(0.4f), decodeY(2.8f))
			path.lineTo(decodeX(2.6f), decodeY(2.8f))
			path.lineTo(decodeX(2.6f), decodeY(1.0f))
			path.lineTo(decodeX(1.8333333f), decodeY(1.0f))
			path.lineTo(decodeX(1.8333333f), decodeY(0.2f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath4() As Path2D
			path.reset()
			path.moveTo(decodeX(1.8333333f), decodeY(0.2f))
			path.lineTo(decodeX(1.6234567f), decodeY(0.2f))
			path.lineTo(decodeX(1.6296296f), decodeY(1.2037038f))
			path.lineTo(decodeX(2.6f), decodeY(1.2006173f))
			path.lineTo(decodeX(2.6f), decodeY(1.0f))
			path.lineTo(decodeX(1.8333333f), decodeY(1.0f))
			path.lineTo(decodeX(1.8333333f), decodeY(0.2f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath5() As Path2D
			path.reset()
			path.moveTo(decodeX(1.8333333f), decodeY(0.4f))
			path.lineTo(decodeX(1.8333333f), decodeY(0.2f))
			path.lineTo(decodeX(0.4f), decodeY(0.2f))
			path.lineTo(decodeX(0.4f), decodeY(2.8f))
			path.lineTo(decodeX(2.6f), decodeY(2.8f))
			path.lineTo(decodeX(2.6f), decodeY(1.0f))
			path.lineTo(decodeX(2.4f), decodeY(1.0f))
			path.lineTo(decodeX(2.4f), decodeY(2.6f))
			path.lineTo(decodeX(0.6f), decodeY(2.6f))
			path.lineTo(decodeX(0.6f), decodeY(0.4f))
			path.lineTo(decodeX(1.8333333f), decodeY(0.4f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath6() As Path2D
			path.reset()
			path.moveTo(decodeX(0.0f), decodeY(2.4f))
			path.lineTo(decodeX(0.0f), decodeY(2.6f))
			path.lineTo(decodeX(0.2f), decodeY(3.0f))
			path.lineTo(decodeX(2.6f), decodeY(3.0f))
			path.lineTo(decodeX(2.8f), decodeY(2.6f))
			path.lineTo(decodeX(2.8f), decodeY(2.4f))
			path.lineTo(decodeX(0.0f), decodeY(2.4f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath7() As Path2D
			path.reset()
			path.moveTo(decodeX(0.6f), decodeY(2.6f))
			path.lineTo(decodeX(0.6037037f), decodeY(1.8425925f))
			path.lineTo(decodeX(0.8f), decodeY(1.0f))
			path.lineTo(decodeX(2.8f), decodeY(1.0f))
			path.lineTo(decodeX(2.8f), decodeY(1.3333334f))
			path.lineTo(decodeX(2.6f), decodeY(2.6f))
			path.lineTo(decodeX(0.6f), decodeY(2.6f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath8() As Path2D
			path.reset()
			path.moveTo(decodeX(0.2f), decodeY(2.6f))
			path.lineTo(decodeX(0.4f), decodeY(2.6f))
			path.lineTo(decodeX(0.40833336f), decodeY(1.8645833f))
			path.lineTo(decodeX(0.79583335f), decodeY(0.8f))
			path.lineTo(decodeX(2.4f), decodeY(0.8f))
			path.lineTo(decodeX(2.4f), decodeY(0.6f))
			path.lineTo(decodeX(1.5f), decodeY(0.6f))
			path.lineTo(decodeX(1.3333334f), decodeY(0.4f))
			path.lineTo(decodeX(1.3333334f), decodeY(0.2f))
			path.lineTo(decodeX(0.6f), decodeY(0.2f))
			path.lineTo(decodeX(0.6f), decodeY(0.4f))
			path.lineTo(decodeX(0.4f), decodeY(0.6f))
			path.lineTo(decodeX(0.2f), decodeY(0.6f))
			path.lineTo(decodeX(0.2f), decodeY(2.6f))
			path.closePath()
			Return path
		End Function

		Private Function decodeRect2() As Rectangle2D
				rect.rectect(decodeX(0.2f), decodeY(0.6f), decodeX(0.4f) - decodeX(0.2f), decodeY(0.8f) - decodeY(0.6f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodeRect3() As Rectangle2D
				rect.rectect(decodeX(0.6f), decodeY(0.2f), decodeX(1.3333334f) - decodeX(0.6f), decodeY(0.4f) - decodeY(0.2f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodeRect4() As Rectangle2D
				rect.rectect(decodeX(1.5f), decodeY(0.6f), decodeX(2.4f) - decodeX(1.5f), decodeY(0.8f) - decodeY(0.6f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodePath9() As Path2D
			path.reset()
			path.moveTo(decodeX(3.0f), decodeY(0.8f))
			path.lineTo(decodeX(3.0f), decodeY(1.0f))
			path.lineTo(decodeX(2.4f), decodeY(1.0f))
			path.lineTo(decodeX(2.4f), decodeY(0.6f))
			path.lineTo(decodeX(1.5f), decodeY(0.6f))
			path.lineTo(decodeX(1.3333334f), decodeY(0.4f))
			path.lineTo(decodeX(1.3333334f), decodeY(0.2f))
			path.lineTo(decodeX(0.5888889f), decodeY(0.20370372f))
			path.lineTo(decodeX(0.5962963f), decodeY(0.34814817f))
			path.lineTo(decodeX(0.34814817f), decodeY(0.6f))
			path.lineTo(decodeX(0.2f), decodeY(0.6f))
			path.lineTo(decodeX(0.2f), decodeY(2.6f))
			path.lineTo(decodeX(2.6f), decodeY(2.6f))
			path.lineTo(decodeX(2.6f), decodeY(1.3333334f))
			path.lineTo(decodeX(2.774074f), decodeY(1.1604939f))
			path.lineTo(decodeX(2.8f), decodeY(1.0f))
			path.lineTo(decodeX(3.0f), decodeY(1.0f))
			path.lineTo(decodeX(2.8925927f), decodeY(1.1882716f))
			path.lineTo(decodeX(2.8f), decodeY(1.3333334f))
			path.lineTo(decodeX(2.8f), decodeY(2.6f))
			path.lineTo(decodeX(2.6f), decodeY(2.8f))
			path.lineTo(decodeX(0.2f), decodeY(2.8f))
			path.lineTo(decodeX(0.0f), decodeY(2.6f))
			path.lineTo(decodeX(0.0f), decodeY(0.65185183f))
			path.lineTo(decodeX(0.63703704f), decodeY(0.0f))
			path.lineTo(decodeX(1.3333334f), decodeY(0.0f))
			path.lineTo(decodeX(1.5925925f), decodeY(0.4f))
			path.lineTo(decodeX(2.4f), decodeY(0.4f))
			path.lineTo(decodeX(2.6f), decodeY(0.6f))
			path.lineTo(decodeX(2.6f), decodeY(0.8f))
			path.lineTo(decodeX(3.0f), decodeY(0.8f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath10() As Path2D
			path.reset()
			path.moveTo(decodeX(2.4f), decodeY(1.0f))
			path.lineTo(decodeX(2.4f), decodeY(0.8f))
			path.lineTo(decodeX(0.74814814f), decodeY(0.8f))
			path.lineTo(decodeX(0.4037037f), decodeY(1.8425925f))
			path.lineTo(decodeX(0.4f), decodeY(2.6f))
			path.lineTo(decodeX(0.6f), decodeY(2.6f))
			path.lineTo(decodeX(0.5925926f), decodeY(2.225926f))
			path.lineTo(decodeX(0.916f), decodeY(0.996f))
			path.lineTo(decodeX(2.4f), decodeY(1.0f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath11() As Path2D
			path.reset()
			path.moveTo(decodeX(2.2f), decodeY(2.2f))
			path.lineTo(decodeX(2.2f), decodeY(2.2f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath12() As Path2D
			path.reset()
			path.moveTo(decodeX(0.6f), decodeY(2.6f))
			path.lineTo(decodeX(0.6f), decodeY(2.2f))
			path.lineTo(decodeX(0.8f), decodeY(1.3333334f))
			path.lineTo(decodeX(2.8f), decodeY(1.3333334f))
			path.lineTo(decodeX(2.8f), decodeY(1.6666667f))
			path.lineTo(decodeX(2.6f), decodeY(2.6f))
			path.lineTo(decodeX(0.6f), decodeY(2.6f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath13() As Path2D
			path.reset()
			path.moveTo(decodeX(0.2f), decodeY(2.6f))
			path.lineTo(decodeX(0.4f), decodeY(2.6f))
			path.lineTo(decodeX(0.4f), decodeY(2.0f))
			path.lineTo(decodeX(0.8f), decodeY(1.1666666f))
			path.lineTo(decodeX(2.4f), decodeY(1.1666666f))
			path.lineTo(decodeX(2.4f), decodeY(0.6f))
			path.lineTo(decodeX(1.5f), decodeY(0.6f))
			path.lineTo(decodeX(1.3333334f), decodeY(0.4f))
			path.lineTo(decodeX(1.3333334f), decodeY(0.2f))
			path.lineTo(decodeX(0.6f), decodeY(0.2f))
			path.lineTo(decodeX(0.6f), decodeY(0.4f))
			path.lineTo(decodeX(0.4f), decodeY(0.6f))
			path.lineTo(decodeX(0.2f), decodeY(0.6f))
			path.lineTo(decodeX(0.2f), decodeY(2.6f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath14() As Path2D
			path.reset()
			path.moveTo(decodeX(3.0f), decodeY(1.1666666f))
			path.lineTo(decodeX(3.0f), decodeY(1.3333334f))
			path.lineTo(decodeX(2.4f), decodeY(1.3333334f))
			path.lineTo(decodeX(2.4f), decodeY(0.6f))
			path.lineTo(decodeX(1.5f), decodeY(0.6f))
			path.lineTo(decodeX(1.3333334f), decodeY(0.4f))
			path.lineTo(decodeX(1.3333334f), decodeY(0.2f))
			path.lineTo(decodeX(0.5888889f), decodeY(0.20370372f))
			path.lineTo(decodeX(0.5962963f), decodeY(0.34814817f))
			path.lineTo(decodeX(0.34814817f), decodeY(0.6f))
			path.lineTo(decodeX(0.2f), decodeY(0.6f))
			path.lineTo(decodeX(0.2f), decodeY(2.6f))
			path.lineTo(decodeX(2.6f), decodeY(2.6f))
			path.lineTo(decodeX(2.6f), decodeY(2.0f))
			path.lineTo(decodeX(2.6f), decodeY(1.8333333f))
			path.lineTo(decodeX(2.916f), decodeY(1.3533334f))
			path.lineTo(decodeX(2.98f), decodeY(1.3766667f))
			path.lineTo(decodeX(2.8f), decodeY(1.8333333f))
			path.lineTo(decodeX(2.8f), decodeY(2.0f))
			path.lineTo(decodeX(2.8f), decodeY(2.6f))
			path.lineTo(decodeX(2.6f), decodeY(2.8f))
			path.lineTo(decodeX(0.2f), decodeY(2.8f))
			path.lineTo(decodeX(0.0f), decodeY(2.6f))
			path.lineTo(decodeX(0.0f), decodeY(0.65185183f))
			path.lineTo(decodeX(0.63703704f), decodeY(0.0f))
			path.lineTo(decodeX(1.3333334f), decodeY(0.0f))
			path.lineTo(decodeX(1.5925925f), decodeY(0.4f))
			path.lineTo(decodeX(2.4f), decodeY(0.4f))
			path.lineTo(decodeX(2.6f), decodeY(0.6f))
			path.lineTo(decodeX(2.6f), decodeY(1.1666666f))
			path.lineTo(decodeX(3.0f), decodeY(1.1666666f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath15() As Path2D
			path.reset()
			path.moveTo(decodeX(2.4f), decodeY(1.3333334f))
			path.lineTo(decodeX(2.4f), decodeY(1.1666666f))
			path.lineTo(decodeX(0.74f), decodeY(1.1666666f))
			path.lineTo(decodeX(0.4f), decodeY(2.0f))
			path.lineTo(decodeX(0.4f), decodeY(2.6f))
			path.lineTo(decodeX(0.6f), decodeY(2.6f))
			path.lineTo(decodeX(0.5925926f), decodeY(2.225926f))
			path.lineTo(decodeX(0.8f), decodeY(1.3333334f))
			path.lineTo(decodeX(2.4f), decodeY(1.3333334f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath16() As Path2D
			path.reset()
			path.moveTo(decodeX(0.0f), decodeY(0.0f))
			path.lineTo(decodeX(1.2397541f), decodeY(0.70163935f))
			path.lineTo(decodeX(0.0f), decodeY(3.0f))
			path.lineTo(decodeX(0.0f), decodeY(0.0f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath17() As Path2D
			path.reset()
			path.moveTo(decodeX(0.0f), decodeY(0.0f))
			path.lineTo(decodeX(1.25f), decodeY(0.0f))
			path.lineTo(decodeX(0.70819676f), decodeY(2.9901638f))
			path.lineTo(decodeX(0.0f), decodeY(0.0f))
			path.closePath()
			Return path
		End Function



		Private Function decodeGradient1(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.046296295f * w) + x, (0.9675926f * h) + y, (0.4861111f * w) + x, (0.5324074f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color3, decodeColor(color3,color4,0.5f), color4})
		End Function

		Private Function decodeGradient2(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color5, decodeColor(color5,color6,0.5f), color6})
		End Function

		Private Function decodeGradient3(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.04191617f,0.10329342f,0.16467066f,0.24550897f,0.3263473f,0.6631737f,1.0f }, New Color() { color10, decodeColor(color10,color11,0.5f), color11, decodeColor(color11,color12,0.5f), color12, decodeColor(color12,color13,0.5f), color13})
		End Function

		Private Function decodeGradient4(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color5, decodeColor(color5,color14,0.5f), color14})
		End Function

		Private Function decodeGradient5(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color18, decodeColor(color18,color19,0.5f), color19})
		End Function

		Private Function decodeGradient6(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.12724552f,0.25449103f,0.62724555f,1.0f }, New Color() { color20, decodeColor(color20,color21,0.5f), color21, decodeColor(color21,color22,0.5f), color22})
		End Function

		Private Function decodeGradient7(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color24, decodeColor(color24,color25,0.5f), color25})
		End Function


	End Class

End Namespace
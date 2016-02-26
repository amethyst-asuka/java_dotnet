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



	Friend NotInheritable Class FileChooserPainter
		Inherits AbstractRegionPainter

		'package private integers representing the available states that
		'this painter will paint. These are used when creating a new instance
		'of FileChooserPainter to determine which region/state is being painted
		'by that instance.
		Friend Const BACKGROUND_ENABLED As Integer = 1
		Friend Const FILEICON_ENABLED As Integer = 2
		Friend Const DIRECTORYICON_ENABLED As Integer = 3
		Friend Const UPFOLDERICON_ENABLED As Integer = 4
		Friend Const NEWFOLDERICON_ENABLED As Integer = 5
		Friend Const COMPUTERICON_ENABLED As Integer = 6
		Friend Const HARDDRIVEICON_ENABLED As Integer = 7
		Friend Const FLOPPYDRIVEICON_ENABLED As Integer = 8
		Friend Const HOMEFOLDERICON_ENABLED As Integer = 9
		Friend Const DETAILSVIEWICON_ENABLED As Integer = 10
		Friend Const LISTVIEWICON_ENABLED As Integer = 11


		Private state As Integer 'refers to one of the static final ints above
		Private ctx As PaintContext

		'the following 4 variables are reused during the painting code of the layers
		Private path As Path2D = New Path2D.Float
		Private rect As Rectangle2D = New Rectangle2D.Float(0, 0, 0, 0)
		Private roundRect As RoundRectangle2D = New RoundRectangle2D.Float(0, 0, 0, 0, 0, 0)
		Private ellipse As Ellipse2D = New Ellipse2D.Float(0, 0, 0, 0)

		'All Colors used for painting are stored here. Ideally, only those colors being used
		'by a particular instance of FileChooserPainter would be created. For the moment at least,
		'however, all are created for each instance.
		Private color1 As Color = decodeColor("control", 0.0f, 0.0f, 0.0f, 0)
		Private color2 As Color = decodeColor("nimbusBlueGrey", 0.007936537f, -0.065654516f, -0.13333333f, 0)
		Private color3 As New Color(97, 98, 102, 255)
		Private color4 As Color = decodeColor("nimbusBlueGrey", -0.032679737f, -0.043332636f, 0.24705881f, 0)
		Private color5 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.110526316f, 0.25490195f, 0)
		Private color6 As Color = decodeColor("nimbusBase", 0.0077680945f, -0.51781034f, 0.3490196f, 0)
		Private color7 As Color = decodeColor("nimbusBase", 0.013940871f, -0.599277f, 0.41960782f, 0)
		Private color8 As Color = decodeColor("nimbusBase", 0.004681647f, -0.4198052f, 0.14117646f, 0)
		Private color9 As Color = decodeColor("nimbusBase", 0.0f, -0.6357143f, 0.45098037f, -127)
		Private color10 As Color = decodeColor("nimbusBlueGrey", 0.0f, 0.0f, -0.21f, -99)
		Private color11 As Color = decodeColor("nimbusBase", 2.9569864E-4f, -0.45978838f, 0.2980392f, 0)
		Private color12 As Color = decodeColor("nimbusBase", 0.0015952587f, -0.34848025f, 0.18823528f, 0)
		Private color13 As Color = decodeColor("nimbusBase", 0.0015952587f, -0.30844158f, 0.09803921f, 0)
		Private color14 As Color = decodeColor("nimbusBase", 0.0015952587f, -0.27329817f, 0.035294116f, 0)
		Private color15 As Color = decodeColor("nimbusBase", 0.004681647f, -0.6198413f, 0.43921566f, 0)
		Private color16 As Color = decodeColor("nimbusBase", 0.0f, -0.6357143f, 0.45098037f, -125)
		Private color17 As Color = decodeColor("nimbusBase", 0.0f, -0.6357143f, 0.45098037f, -50)
		Private color18 As Color = decodeColor("nimbusBase", 0.0f, -0.6357143f, 0.45098037f, -100)
		Private color19 As Color = decodeColor("nimbusBase", 0.0012094378f, -0.23571429f, -0.0784314f, 0)
		Private color20 As Color = decodeColor("nimbusBase", 2.9569864E-4f, -0.115166366f, -0.2627451f, 0)
		Private color21 As Color = decodeColor("nimbusBase", 0.0027436614f, -0.335015f, 0.011764705f, 0)
		Private color22 As Color = decodeColor("nimbusBase", 0.0024294257f, -0.3857143f, 0.031372547f, 0)
		Private color23 As Color = decodeColor("nimbusBase", 0.0018081069f, -0.3595238f, -0.13725492f, 0)
		Private color24 As New Color(255, 200, 0, 255)
		Private color25 As Color = decodeColor("nimbusBase", 0.004681647f, -0.44904763f, 0.039215684f, 0)
		Private color26 As Color = decodeColor("nimbusBase", 0.0015952587f, -0.43718487f, -0.015686274f, 0)
		Private color27 As Color = decodeColor("nimbusBase", 2.9569864E-4f, -0.39212453f, -0.24313727f, 0)
		Private color28 As Color = decodeColor("nimbusBase", 0.004681647f, -0.6117143f, 0.43137252f, 0)
		Private color29 As Color = decodeColor("nimbusBase", 0.0012094378f, -0.28015873f, -0.019607842f, 0)
		Private color30 As Color = decodeColor("nimbusBase", 0.00254488f, -0.07049692f, -0.2784314f, 0)
		Private color31 As Color = decodeColor("nimbusBase", 0.0015952587f, -0.28045115f, 0.04705882f, 0)
		Private color32 As Color = decodeColor("nimbusBlueGrey", 0.0f, 5.847961E-4f, -0.21568626f, 0)
		Private color33 As Color = decodeColor("nimbusBase", -0.0061469674f, 0.3642857f, 0.14509803f, 0)
		Private color34 As Color = decodeColor("nimbusBase", 0.0053939223f, 0.3642857f, -0.0901961f, 0)
		Private color35 As Color = decodeColor("nimbusBase", 0.0f, -0.6357143f, 0.45098037f, 0)
		Private color36 As Color = decodeColor("nimbusBase", -0.006044388f, -0.23963585f, 0.45098037f, 0)
		Private color37 As Color = decodeColor("nimbusBase", -0.0063245893f, 0.01592505f, 0.4078431f, 0)
		Private color38 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.110526316f, 0.25490195f, -170)
		Private color39 As Color = decodeColor("nimbusOrange", -0.032758567f, -0.018273294f, 0.25098038f, 0)
		Private color40 As New Color(255, 255, 255, 255)
		Private color41 As New Color(252, 255, 92, 255)
		Private color42 As New Color(253, 191, 4, 255)
		Private color43 As New Color(160, 161, 163, 255)
		Private color44 As New Color(0, 0, 0, 255)
		Private color45 As New Color(239, 241, 243, 255)
		Private color46 As New Color(197, 201, 205, 255)
		Private color47 As New Color(105, 110, 118, 255)
		Private color48 As New Color(63, 67, 72, 255)
		Private color49 As New Color(56, 51, 25, 255)
		Private color50 As New Color(144, 255, 0, 255)
		Private color51 As New Color(243, 245, 246, 255)
		Private color52 As New Color(208, 212, 216, 255)
		Private color53 As New Color(191, 193, 194, 255)
		Private color54 As New Color(170, 172, 175, 255)
		Private color55 As New Color(152, 155, 158, 255)
		Private color56 As New Color(59, 62, 66, 255)
		Private color57 As New Color(46, 46, 46, 255)
		Private color58 As New Color(64, 64, 64, 255)
		Private color59 As New Color(43, 43, 43, 255)
		Private color60 As New Color(164, 179, 206, 255)
		Private color61 As New Color(97, 123, 170, 255)
		Private color62 As New Color(53, 86, 146, 255)
		Private color63 As New Color(48, 82, 144, 255)
		Private color64 As New Color(71, 99, 150, 255)
		Private color65 As New Color(224, 224, 224, 255)
		Private color66 As New Color(232, 232, 232, 255)
		Private color67 As New Color(231, 234, 237, 255)
		Private color68 As New Color(205, 211, 215, 255)
		Private color69 As New Color(149, 153, 156, 54)
		Private color70 As New Color(255, 122, 101, 255)
		Private color71 As New Color(54, 78, 122, 255)
		Private color72 As New Color(51, 60, 70, 255)
		Private color73 As New Color(228, 232, 237, 255)
		Private color74 As New Color(27, 57, 87, 255)
		Private color75 As New Color(75, 109, 137, 255)
		Private color76 As New Color(77, 133, 185, 255)
		Private color77 As New Color(81, 59, 7, 255)
		Private color78 As New Color(97, 74, 18, 255)
		Private color79 As New Color(137, 115, 60, 255)
		Private color80 As New Color(174, 151, 91, 255)
		Private color81 As New Color(114, 92, 13, 255)
		Private color82 As New Color(64, 48, 0, 255)
		Private color83 As New Color(244, 222, 143, 255)
		Private color84 As New Color(160, 161, 162, 255)
		Private color85 As New Color(226, 230, 233, 255)
		Private color86 As New Color(221, 225, 230, 255)
		Private color87 As Color = decodeColor("nimbusBase", 0.004681647f, -0.48756614f, 0.19215685f, 0)
		Private color88 As Color = decodeColor("nimbusBase", 0.004681647f, -0.48399013f, 0.019607842f, 0)
		Private color89 As Color = decodeColor("nimbusBase", -0.0028941035f, -0.5906323f, 0.4078431f, 0)
		Private color90 As Color = decodeColor("nimbusBase", 0.004681647f, -0.51290727f, 0.34509802f, 0)
		Private color91 As Color = decodeColor("nimbusBase", 0.009583652f, -0.5642857f, 0.3843137f, 0)
		Private color92 As Color = decodeColor("nimbusBase", -0.0072231293f, -0.6074885f, 0.4235294f, 0)
		Private color93 As Color = decodeColor("nimbusBase", 7.13408E-4f, -0.52158386f, 0.17254901f, 0)
		Private color94 As Color = decodeColor("nimbusBase", 0.012257397f, -0.5775132f, 0.19215685f, 0)
		Private color95 As Color = decodeColor("nimbusBase", 0.08801502f, -0.6164835f, -0.14117649f, 0)
		Private color96 As Color = decodeColor("nimbusBase", -0.57865167f, -0.6357143f, -0.5019608f, 0)
		Private color97 As Color = decodeColor("nimbusBase", -0.0036516786f, -0.555393f, 0.42745095f, 0)
		Private color98 As Color = decodeColor("nimbusBase", -0.0010654926f, -0.3634138f, 0.2862745f, 0)
		Private color99 As Color = decodeColor("nimbusBase", -0.57865167f, -0.6357143f, -0.29803923f, 0)
		Private color100 As Color = decodeColor("nimbusBase", -0.57865167f, -0.6357143f, 0.12156862f, 0)
		Private color101 As Color = decodeColor("nimbusBase", -0.57865167f, -0.6357143f, -0.54901963f, 0)
		Private color102 As Color = decodeColor("nimbusBase", -0.57865167f, -0.6357143f, -0.48627454f, 0)
		Private color103 As Color = decodeColor("nimbusBase", -0.57865167f, -0.6357143f, -0.007843137f, 0)
		Private color104 As Color = decodeColor("nimbusBase", -0.0028941035f, -0.5408867f, -0.09411767f, 0)
		Private color105 As Color = decodeColor("nimbusBase", -0.011985004f, -0.54721874f, -0.10588238f, 0)
		Private color106 As Color = decodeColor("nimbusBase", -0.0022627711f, -0.4305861f, -0.0901961f, 0)
		Private color107 As Color = decodeColor("nimbusBase", -0.00573498f, -0.447479f, -0.21568629f, 0)
		Private color108 As Color = decodeColor("nimbusBase", 0.004681647f, -0.53271f, 0.36470586f, 0)
		Private color109 As Color = decodeColor("nimbusBase", 0.004681647f, -0.5276062f, -0.11372551f, 0)
		Private color110 As Color = decodeColor("nimbusBase", -8.738637E-4f, -0.5278006f, -0.0039215684f, 0)
		Private color111 As Color = decodeColor("nimbusBase", -0.0028941035f, -0.5338625f, -0.12549022f, 0)
		Private color112 As Color = decodeColor("nimbusBlueGrey", -0.03535354f, -0.008674465f, -0.32156864f, 0)
		Private color113 As Color = decodeColor("nimbusBlueGrey", -0.027777791f, -0.010526314f, -0.3529412f, 0)
		Private color114 As Color = decodeColor("nimbusBase", -0.0028941035f, -0.5234694f, -0.1647059f, 0)
		Private color115 As Color = decodeColor("nimbusBase", 0.004681647f, -0.53401935f, -0.086274534f, 0)
		Private color116 As Color = decodeColor("nimbusBase", 0.004681647f, -0.52077174f, -0.20784315f, 0)
		Private color117 As New Color(108, 114, 120, 255)
		Private color118 As New Color(77, 82, 87, 255)
		Private color119 As Color = decodeColor("nimbusBase", -0.004577577f, -0.52179027f, -0.2392157f, 0)
		Private color120 As Color = decodeColor("nimbusBase", -0.004577577f, -0.547479f, -0.14901963f, 0)
		Private color121 As New Color(186, 186, 186, 50)
		Private color122 As New Color(186, 186, 186, 40)


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
				Case FILEICON_ENABLED
					paintfileIconEnabled(g)
				Case DIRECTORYICON_ENABLED
					paintdirectoryIconEnabled(g)
				Case UPFOLDERICON_ENABLED
					paintupFolderIconEnabled(g)
				Case NEWFOLDERICON_ENABLED
					paintnewFolderIconEnabled(g)
				Case HARDDRIVEICON_ENABLED
					painthardDriveIconEnabled(g)
				Case FLOPPYDRIVEICON_ENABLED
					paintfloppyDriveIconEnabled(g)
				Case HOMEFOLDERICON_ENABLED
					painthomeFolderIconEnabled(g)
				Case DETAILSVIEWICON_ENABLED
					paintdetailsViewIconEnabled(g)
				Case LISTVIEWICON_ENABLED
					paintlistViewIconEnabled(g)

			End Select
		End Sub



		Protected Friend Property NotOverridable Overrides paintContext As PaintContext
			Get
				Return ctx
			End Get
		End Property

		Private Sub paintBackgroundEnabled(ByVal g As Graphics2D)
			rect = decodeRect1()
			g.paint = color1
			g.fill(rect)

		End Sub

		Private Sub paintfileIconEnabled(ByVal g As Graphics2D)
			path = decodePath1()
			g.paint = color2
			g.fill(path)
			rect = decodeRect2()
			g.paint = color3
			g.fill(rect)
			path = decodePath2()
			g.paint = decodeGradient1(path)
			g.fill(path)
			path = decodePath3()
			g.paint = decodeGradient2(path)
			g.fill(path)
			path = decodePath4()
			g.paint = color8
			g.fill(path)
			path = decodePath5()
			g.paint = color9
			g.fill(path)

		End Sub

		Private Sub paintdirectoryIconEnabled(ByVal g As Graphics2D)
			path = decodePath6()
			g.paint = color10
			g.fill(path)
			path = decodePath7()
			g.paint = decodeGradient3(path)
			g.fill(path)
			path = decodePath8()
			g.paint = decodeGradient4(path)
			g.fill(path)
			rect = decodeRect3()
			g.paint = color16
			g.fill(rect)
			rect = decodeRect4()
			g.paint = color17
			g.fill(rect)
			rect = decodeRect5()
			g.paint = color18
			g.fill(rect)
			path = decodePath9()
			g.paint = decodeGradient5(path)
			g.fill(path)
			path = decodePath10()
			g.paint = decodeGradient6(path)
			g.fill(path)
			path = decodePath11()
			g.paint = color24
			g.fill(path)

		End Sub

		Private Sub paintupFolderIconEnabled(ByVal g As Graphics2D)
			path = decodePath12()
			g.paint = decodeGradient7(path)
			g.fill(path)
			path = decodePath13()
			g.paint = decodeGradient8(path)
			g.fill(path)
			path = decodePath14()
			g.paint = decodeGradient9(path)
			g.fill(path)
			path = decodePath15()
			g.paint = decodeGradient10(path)
			g.fill(path)
			path = decodePath16()
			g.paint = color32
			g.fill(path)
			path = decodePath17()
			g.paint = decodeGradient11(path)
			g.fill(path)
			path = decodePath18()
			g.paint = color35
			g.fill(path)
			path = decodePath19()
			g.paint = decodeGradient12(path)
			g.fill(path)

		End Sub

		Private Sub paintnewFolderIconEnabled(ByVal g As Graphics2D)
			path = decodePath6()
			g.paint = color10
			g.fill(path)
			path = decodePath7()
			g.paint = decodeGradient3(path)
			g.fill(path)
			path = decodePath8()
			g.paint = decodeGradient4(path)
			g.fill(path)
			rect = decodeRect3()
			g.paint = color16
			g.fill(rect)
			rect = decodeRect4()
			g.paint = color17
			g.fill(rect)
			rect = decodeRect5()
			g.paint = color18
			g.fill(rect)
			path = decodePath9()
			g.paint = decodeGradient5(path)
			g.fill(path)
			path = decodePath10()
			g.paint = decodeGradient6(path)
			g.fill(path)
			path = decodePath11()
			g.paint = color24
			g.fill(path)
			path = decodePath20()
			g.paint = color38
			g.fill(path)
			path = decodePath21()
			g.paint = color39
			g.fill(path)
			path = decodePath22()
			g.paint = decodeRadial1(path)
			g.fill(path)

		End Sub

		Private Sub painthardDriveIconEnabled(ByVal g As Graphics2D)
			rect = decodeRect6()
			g.paint = color43
			g.fill(rect)
			rect = decodeRect7()
			g.paint = color44
			g.fill(rect)
			rect = decodeRect8()
			g.paint = decodeGradient13(rect)
			g.fill(rect)
			path = decodePath23()
			g.paint = decodeGradient14(path)
			g.fill(path)
			rect = decodeRect9()
			g.paint = color49
			g.fill(rect)
			rect = decodeRect10()
			g.paint = color49
			g.fill(rect)
			ellipse = decodeEllipse1()
			g.paint = color50
			g.fill(ellipse)
			path = decodePath24()
			g.paint = decodeGradient15(path)
			g.fill(path)
			ellipse = decodeEllipse2()
			g.paint = color53
			g.fill(ellipse)
			ellipse = decodeEllipse3()
			g.paint = color53
			g.fill(ellipse)
			ellipse = decodeEllipse4()
			g.paint = color54
			g.fill(ellipse)
			ellipse = decodeEllipse5()
			g.paint = color55
			g.fill(ellipse)
			ellipse = decodeEllipse6()
			g.paint = color55
			g.fill(ellipse)
			ellipse = decodeEllipse7()
			g.paint = color55
			g.fill(ellipse)
			rect = decodeRect11()
			g.paint = color56
			g.fill(rect)
			rect = decodeRect12()
			g.paint = color56
			g.fill(rect)
			rect = decodeRect13()
			g.paint = color56
			g.fill(rect)

		End Sub

		Private Sub paintfloppyDriveIconEnabled(ByVal g As Graphics2D)
			path = decodePath25()
			g.paint = decodeGradient16(path)
			g.fill(path)
			path = decodePath26()
			g.paint = decodeGradient17(path)
			g.fill(path)
			path = decodePath27()
			g.paint = decodeGradient18(path)
			g.fill(path)
			path = decodePath28()
			g.paint = decodeGradient19(path)
			g.fill(path)
			path = decodePath29()
			g.paint = color69
			g.fill(path)
			rect = decodeRect14()
			g.paint = color70
			g.fill(rect)
			rect = decodeRect15()
			g.paint = color40
			g.fill(rect)
			rect = decodeRect16()
			g.paint = color67
			g.fill(rect)
			rect = decodeRect17()
			g.paint = color71
			g.fill(rect)
			rect = decodeRect18()
			g.paint = color44
			g.fill(rect)

		End Sub

		Private Sub painthomeFolderIconEnabled(ByVal g As Graphics2D)
			path = decodePath30()
			g.paint = color72
			g.fill(path)
			path = decodePath31()
			g.paint = color73
			g.fill(path)
			rect = decodeRect19()
			g.paint = decodeGradient20(rect)
			g.fill(rect)
			rect = decodeRect20()
			g.paint = color76
			g.fill(rect)
			path = decodePath32()
			g.paint = decodeGradient21(path)
			g.fill(path)
			rect = decodeRect21()
			g.paint = decodeGradient22(rect)
			g.fill(rect)
			path = decodePath33()
			g.paint = decodeGradient23(path)
			g.fill(path)
			path = decodePath34()
			g.paint = color83
			g.fill(path)
			path = decodePath35()
			g.paint = decodeGradient24(path)
			g.fill(path)
			path = decodePath36()
			g.paint = decodeGradient25(path)
			g.fill(path)

		End Sub

		Private Sub paintdetailsViewIconEnabled(ByVal g As Graphics2D)
			rect = decodeRect22()
			g.paint = decodeGradient26(rect)
			g.fill(rect)
			rect = decodeRect23()
			g.paint = decodeGradient27(rect)
			g.fill(rect)
			rect = decodeRect24()
			g.paint = color93
			g.fill(rect)
			rect = decodeRect5()
			g.paint = color93
			g.fill(rect)
			rect = decodeRect25()
			g.paint = color93
			g.fill(rect)
			rect = decodeRect26()
			g.paint = color94
			g.fill(rect)
			ellipse = decodeEllipse8()
			g.paint = decodeGradient28(ellipse)
			g.fill(ellipse)
			ellipse = decodeEllipse9()
			g.paint = decodeRadial2(ellipse)
			g.fill(ellipse)
			path = decodePath37()
			g.paint = decodeGradient29(path)
			g.fill(path)
			path = decodePath38()
			g.paint = decodeGradient30(path)
			g.fill(path)
			rect = decodeRect27()
			g.paint = color104
			g.fill(rect)
			rect = decodeRect28()
			g.paint = color105
			g.fill(rect)
			rect = decodeRect29()
			g.paint = color106
			g.fill(rect)
			rect = decodeRect30()
			g.paint = color107
			g.fill(rect)

		End Sub

		Private Sub paintlistViewIconEnabled(ByVal g As Graphics2D)
			rect = decodeRect31()
			g.paint = decodeGradient26(rect)
			g.fill(rect)
			rect = decodeRect32()
			g.paint = decodeGradient31(rect)
			g.fill(rect)
			rect = decodeRect33()
			g.paint = color109
			g.fill(rect)
			rect = decodeRect34()
			g.paint = decodeGradient32(rect)
			g.fill(rect)
			rect = decodeRect35()
			g.paint = color111
			g.fill(rect)
			rect = decodeRect36()
			g.paint = color112
			g.fill(rect)
			rect = decodeRect37()
			g.paint = color113
			g.fill(rect)
			rect = decodeRect38()
			g.paint = decodeGradient33(rect)
			g.fill(rect)
			rect = decodeRect39()
			g.paint = color116
			g.fill(rect)
			rect = decodeRect40()
			g.paint = decodeGradient34(rect)
			g.fill(rect)
			rect = decodeRect41()
			g.paint = decodeGradient35(rect)
			g.fill(rect)
			rect = decodeRect42()
			g.paint = color119
			g.fill(rect)
			rect = decodeRect43()
			g.paint = color121
			g.fill(rect)
			rect = decodeRect44()
			g.paint = color121
			g.fill(rect)
			rect = decodeRect45()
			g.paint = color121
			g.fill(rect)
			rect = decodeRect46()
			g.paint = color122
			g.fill(rect)
			rect = decodeRect47()
			g.paint = color121
			g.fill(rect)
			rect = decodeRect48()
			g.paint = color122
			g.fill(rect)
			rect = decodeRect49()
			g.paint = color122
			g.fill(rect)
			rect = decodeRect50()
			g.paint = color121
			g.fill(rect)
			rect = decodeRect51()
			g.paint = color122
			g.fill(rect)
			rect = decodeRect52()
			g.paint = color122
			g.fill(rect)

		End Sub



		Private Function decodeRect1() As Rectangle2D
				rect.rectect(decodeX(1.0f), decodeY(1.0f), decodeX(2.0f) - decodeX(1.0f), decodeY(2.0f) - decodeY(1.0f)) 'height - width - y - x
			Return rect
		End Function

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

		Private Function decodeRect2() As Rectangle2D
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

		Private Function decodeRect3() As Rectangle2D
				rect.rectect(decodeX(0.2f), decodeY(0.6f), decodeX(0.4f) - decodeX(0.2f), decodeY(0.8f) - decodeY(0.6f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodeRect4() As Rectangle2D
				rect.rectect(decodeX(0.6f), decodeY(0.2f), decodeX(1.3333334f) - decodeX(0.6f), decodeY(0.4f) - decodeY(0.2f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodeRect5() As Rectangle2D
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
			path.moveTo(decodeX(0.0f), decodeY(2.8f))
			path.lineTo(decodeX(0.2f), decodeY(3.0f))
			path.lineTo(decodeX(2.6f), decodeY(3.0f))
			path.lineTo(decodeX(2.8f), decodeY(2.8f))
			path.lineTo(decodeX(2.8f), decodeY(1.8333333f))
			path.lineTo(decodeX(3.0f), decodeY(1.3333334f))
			path.lineTo(decodeX(3.0f), decodeY(1.0f))
			path.lineTo(decodeX(1.5f), decodeY(1.0f))
			path.lineTo(decodeX(1.5f), decodeY(0.4f))
			path.lineTo(decodeX(1.3333334f), decodeY(0.2f))
			path.lineTo(decodeX(0.6f), decodeY(0.2f))
			path.lineTo(decodeX(0.4f), decodeY(0.4f))
			path.lineTo(decodeX(0.4f), decodeY(0.6f))
			path.lineTo(decodeX(0.2f), decodeY(0.6f))
			path.lineTo(decodeX(0.0f), decodeY(0.8f))
			path.lineTo(decodeX(0.0f), decodeY(2.8f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath13() As Path2D
			path.reset()
			path.moveTo(decodeX(0.2f), decodeY(2.8f))
			path.lineTo(decodeX(0.2f), decodeY(0.8f))
			path.lineTo(decodeX(0.4f), decodeY(0.8f))
			path.lineTo(decodeX(0.6f), decodeY(0.6f))
			path.lineTo(decodeX(0.6f), decodeY(0.4f))
			path.lineTo(decodeX(1.3333334f), decodeY(0.4f))
			path.lineTo(decodeX(1.3333334f), decodeY(0.6f))
			path.lineTo(decodeX(1.5f), decodeY(0.6f))
			path.lineTo(decodeX(1.5f), decodeY(2.8f))
			path.lineTo(decodeX(0.2f), decodeY(2.8f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath14() As Path2D
			path.reset()
			path.moveTo(decodeX(0.4f), decodeY(2.0f))
			path.lineTo(decodeX(0.6f), decodeY(1.1666666f))
			path.lineTo(decodeX(0.8f), decodeY(1.0f))
			path.lineTo(decodeX(2.8f), decodeY(1.0f))
			path.lineTo(decodeX(2.8f), decodeY(2.8f))
			path.lineTo(decodeX(2.4f), decodeY(3.0f))
			path.lineTo(decodeX(0.4f), decodeY(3.0f))
			path.lineTo(decodeX(0.4f), decodeY(2.0f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath15() As Path2D
			path.reset()
			path.moveTo(decodeX(0.6f), decodeY(2.8f))
			path.lineTo(decodeX(0.6f), decodeY(2.0f))
			path.lineTo(decodeX(0.8f), decodeY(1.1666666f))
			path.lineTo(decodeX(2.8f), decodeY(1.1666666f))
			path.lineTo(decodeX(2.6f), decodeY(2.0f))
			path.lineTo(decodeX(2.6f), decodeY(2.8f))
			path.lineTo(decodeX(0.6f), decodeY(2.8f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath16() As Path2D
			path.reset()
			path.moveTo(decodeX(1.1702899f), decodeY(1.2536231f))
			path.lineTo(decodeX(1.1666666f), decodeY(1.0615941f))
			path.lineTo(decodeX(3.0f), decodeY(1.0978261f))
			path.lineTo(decodeX(2.7782607f), decodeY(1.25f))
			path.lineTo(decodeX(2.3913045f), decodeY(1.3188406f))
			path.lineTo(decodeX(2.3826087f), decodeY(1.7246377f))
			path.lineTo(decodeX(2.173913f), decodeY(1.9347827f))
			path.lineTo(decodeX(1.8695652f), decodeY(1.923913f))
			path.lineTo(decodeX(1.710145f), decodeY(1.7246377f))
			path.lineTo(decodeX(1.710145f), decodeY(1.3115941f))
			path.lineTo(decodeX(1.1702899f), decodeY(1.2536231f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath17() As Path2D
			path.reset()
			path.moveTo(decodeX(1.1666666f), decodeY(1.1666666f))
			path.lineTo(decodeX(1.1666666f), decodeY(0.9130435f))
			path.lineTo(decodeX(1.9456522f), decodeY(0.0f))
			path.lineTo(decodeX(2.0608697f), decodeY(0.0f))
			path.lineTo(decodeX(2.9956522f), decodeY(0.9130435f))
			path.lineTo(decodeX(3.0f), decodeY(1.1666666f))
			path.lineTo(decodeX(2.4f), decodeY(1.1666666f))
			path.lineTo(decodeX(2.4f), decodeY(1.6666667f))
			path.lineTo(decodeX(2.2f), decodeY(1.8333333f))
			path.lineTo(decodeX(1.8333333f), decodeY(1.8333333f))
			path.lineTo(decodeX(1.6666667f), decodeY(1.6666667f))
			path.lineTo(decodeX(1.6666667f), decodeY(1.1666666f))
			path.lineTo(decodeX(1.1666666f), decodeY(1.1666666f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath18() As Path2D
			path.reset()
			path.moveTo(decodeX(1.2717391f), decodeY(0.9956522f))
			path.lineTo(decodeX(1.8333333f), decodeY(1.0f))
			path.lineTo(decodeX(1.8333333f), decodeY(1.6666667f))
			path.lineTo(decodeX(2.2f), decodeY(1.6666667f))
			path.lineTo(decodeX(2.2f), decodeY(1.0f))
			path.lineTo(decodeX(2.8652174f), decodeY(1.0f))
			path.lineTo(decodeX(2.0f), decodeY(0.13043478f))
			path.lineTo(decodeX(1.2717391f), decodeY(0.9956522f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath19() As Path2D
			path.reset()
			path.moveTo(decodeX(1.8333333f), decodeY(1.6666667f))
			path.lineTo(decodeX(1.8333333f), decodeY(1.0f))
			path.lineTo(decodeX(1.3913044f), decodeY(1.0f))
			path.lineTo(decodeX(1.9963768f), decodeY(0.25652176f))
			path.lineTo(decodeX(2.6608696f), decodeY(1.0f))
			path.lineTo(decodeX(2.2f), decodeY(1.0f))
			path.lineTo(decodeX(2.2f), decodeY(1.6666667f))
			path.lineTo(decodeX(1.8333333f), decodeY(1.6666667f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath20() As Path2D
			path.reset()
			path.moveTo(decodeX(0.22692308f), decodeY(0.061538465f))
			path.lineTo(decodeX(0.75384617f), decodeY(0.37692308f))
			path.lineTo(decodeX(0.91923076f), decodeY(0.01923077f))
			path.lineTo(decodeX(1.2532052f), decodeY(0.40769228f))
			path.lineTo(decodeX(1.7115386f), decodeY(0.13846155f))
			path.lineTo(decodeX(1.6923077f), decodeY(0.85f))
			path.lineTo(decodeX(2.169231f), decodeY(0.9115385f))
			path.lineTo(decodeX(1.7852564f), decodeY(1.3333334f))
			path.lineTo(decodeX(1.9166667f), decodeY(1.9679487f))
			path.lineTo(decodeX(1.3685898f), decodeY(1.8301282f))
			path.lineTo(decodeX(1.1314102f), decodeY(2.2115386f))
			path.lineTo(decodeX(0.63076925f), decodeY(1.8205128f))
			path.lineTo(decodeX(0.22692308f), decodeY(1.9262822f))
			path.lineTo(decodeX(0.31153846f), decodeY(1.4871795f))
			path.lineTo(decodeX(0.0f), decodeY(1.1538461f))
			path.lineTo(decodeX(0.38461536f), decodeY(0.68076926f))
			path.lineTo(decodeX(0.22692308f), decodeY(0.061538465f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath21() As Path2D
			path.reset()
			path.moveTo(decodeX(0.23461537f), decodeY(0.33076924f))
			path.lineTo(decodeX(0.32692307f), decodeY(0.21538463f))
			path.lineTo(decodeX(0.9653846f), decodeY(0.74615383f))
			path.lineTo(decodeX(1.0160257f), decodeY(0.01923077f))
			path.lineTo(decodeX(1.1506411f), decodeY(0.01923077f))
			path.lineTo(decodeX(1.2275641f), decodeY(0.72307694f))
			path.lineTo(decodeX(1.6987178f), decodeY(0.20769231f))
			path.lineTo(decodeX(1.8237178f), decodeY(0.37692308f))
			path.lineTo(decodeX(1.3878205f), decodeY(0.94230765f))
			path.lineTo(decodeX(1.9775641f), decodeY(1.0256411f))
			path.lineTo(decodeX(1.9839742f), decodeY(1.1474359f))
			path.lineTo(decodeX(1.4070512f), decodeY(1.2083334f))
			path.lineTo(decodeX(1.7980769f), decodeY(1.7307692f))
			path.lineTo(decodeX(1.7532051f), decodeY(1.8269231f))
			path.lineTo(decodeX(1.2211539f), decodeY(1.3365384f))
			path.lineTo(decodeX(1.1506411f), decodeY(1.9839742f))
			path.lineTo(decodeX(1.0288461f), decodeY(1.9775641f))
			path.lineTo(decodeX(0.95384616f), decodeY(1.3429488f))
			path.lineTo(decodeX(0.28846154f), decodeY(1.8012822f))
			path.lineTo(decodeX(0.20769231f), decodeY(1.7371795f))
			path.lineTo(decodeX(0.75f), decodeY(1.173077f))
			path.lineTo(decodeX(0.011538462f), decodeY(1.1634616f))
			path.lineTo(decodeX(0.015384616f), decodeY(1.0224359f))
			path.lineTo(decodeX(0.79615384f), decodeY(0.94230765f))
			path.lineTo(decodeX(0.23461537f), decodeY(0.33076924f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath22() As Path2D
			path.reset()
			path.moveTo(decodeX(0.58461535f), decodeY(0.6615385f))
			path.lineTo(decodeX(0.68846154f), decodeY(0.56923074f))
			path.lineTo(decodeX(0.9884615f), decodeY(0.80769235f))
			path.lineTo(decodeX(1.0352564f), decodeY(0.43076926f))
			path.lineTo(decodeX(1.1282052f), decodeY(0.43846154f))
			path.lineTo(decodeX(1.1891025f), decodeY(0.80769235f))
			path.lineTo(decodeX(1.4006411f), decodeY(0.59615386f))
			path.lineTo(decodeX(1.4967948f), decodeY(0.70384616f))
			path.lineTo(decodeX(1.3173077f), decodeY(0.9384615f))
			path.lineTo(decodeX(1.625f), decodeY(1.0256411f))
			path.lineTo(decodeX(1.6282051f), decodeY(1.1346154f))
			path.lineTo(decodeX(1.2564102f), decodeY(1.176282f))
			path.lineTo(decodeX(1.4711539f), decodeY(1.3910257f))
			path.lineTo(decodeX(1.4070512f), decodeY(1.4807693f))
			path.lineTo(decodeX(1.1858975f), decodeY(1.2724359f))
			path.lineTo(decodeX(1.1474359f), decodeY(1.6602564f))
			path.lineTo(decodeX(1.0416666f), decodeY(1.6602564f))
			path.lineTo(decodeX(0.9769231f), decodeY(1.2884616f))
			path.lineTo(decodeX(0.6923077f), decodeY(1.5f))
			path.lineTo(decodeX(0.6423077f), decodeY(1.3782052f))
			path.lineTo(decodeX(0.83076924f), decodeY(1.176282f))
			path.lineTo(decodeX(0.46923074f), decodeY(1.1474359f))
			path.lineTo(decodeX(0.48076925f), decodeY(1.0064102f))
			path.lineTo(decodeX(0.8230769f), decodeY(0.98461545f))
			path.lineTo(decodeX(0.58461535f), decodeY(0.6615385f))
			path.closePath()
			Return path
		End Function

		Private Function decodeRect6() As Rectangle2D
				rect.rectect(decodeX(0.2f), decodeY(0.0f), decodeX(2.8f) - decodeX(0.2f), decodeY(2.2f) - decodeY(0.0f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodeRect7() As Rectangle2D
				rect.rectect(decodeX(0.2f), decodeY(2.2f), decodeX(2.8f) - decodeX(0.2f), decodeY(3.0f) - decodeY(2.2f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodeRect8() As Rectangle2D
				rect.rectect(decodeX(0.4f), decodeY(0.2f), decodeX(2.6f) - decodeX(0.4f), decodeY(2.2f) - decodeY(0.2f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodePath23() As Path2D
			path.reset()
			path.moveTo(decodeX(0.4f), decodeY(2.2f))
			path.lineTo(decodeX(0.4f), decodeY(2.8f))
			path.lineTo(decodeX(0.6f), decodeY(2.8f))
			path.lineTo(decodeX(0.6f), decodeY(2.6f))
			path.lineTo(decodeX(2.4f), decodeY(2.6f))
			path.lineTo(decodeX(2.4f), decodeY(2.8f))
			path.lineTo(decodeX(2.6f), decodeY(2.8f))
			path.lineTo(decodeX(2.6f), decodeY(2.2f))
			path.lineTo(decodeX(0.4f), decodeY(2.2f))
			path.closePath()
			Return path
		End Function

		Private Function decodeRect9() As Rectangle2D
				rect.rectect(decodeX(0.6f), decodeY(2.8f), decodeX(1.6666667f) - decodeX(0.6f), decodeY(3.0f) - decodeY(2.8f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodeRect10() As Rectangle2D
				rect.rectect(decodeX(1.8333333f), decodeY(2.8f), decodeX(2.4f) - decodeX(1.8333333f), decodeY(3.0f) - decodeY(2.8f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodeEllipse1() As Ellipse2D
			ellipse.frameame(decodeX(0.6f), decodeY(2.4f), decodeX(0.8f) - decodeX(0.6f), decodeY(2.6f) - decodeY(2.4f)) 'height - width - y - x
			Return ellipse
		End Function

		Private Function decodePath24() As Path2D
			path.reset()
			path.moveTo(decodeX(1.0f), decodeY(0.4f))
			path.curveTo(decodeAnchorX(1.0f, 1.0f), decodeAnchorY(0.4000000059604645f, -1.0f), decodeAnchorX(2.0f, -1.0f), decodeAnchorY(0.4000000059604645f, -1.0f), decodeX(2.0f), decodeY(0.4f))
			path.curveTo(decodeAnchorX(2.0f, 1.0f), decodeAnchorY(0.4000000059604645f, 1.0f), decodeAnchorX(2.200000047683716f, 0.0f), decodeAnchorY(1.0f, -1.0f), decodeX(2.2f), decodeY(1.0f))
			path.curveTo(decodeAnchorX(2.200000047683716f, 0.0f), decodeAnchorY(1.0f, 1.0f), decodeAnchorX(2.200000047683716f, 0.0f), decodeAnchorY(1.5f, -2.0f), decodeX(2.2f), decodeY(1.5f))
			path.curveTo(decodeAnchorX(2.200000047683716f, 0.0f), decodeAnchorY(1.5f, 2.0f), decodeAnchorX(1.6666667461395264f, 1.0f), decodeAnchorY(1.8333332538604736f, 0.0f), decodeX(1.6666667f), decodeY(1.8333333f))
			path.curveTo(decodeAnchorX(1.6666667461395264f, -1.0f), decodeAnchorY(1.8333332538604736f, 0.0f), decodeAnchorX(1.3333333730697632f, 1.0f), decodeAnchorY(1.8333332538604736f, 0.0f), decodeX(1.3333334f), decodeY(1.8333333f))
			path.curveTo(decodeAnchorX(1.3333333730697632f, -1.0f), decodeAnchorY(1.8333332538604736f, 0.0f), decodeAnchorX(0.800000011920929f, 0.0f), decodeAnchorY(1.5f, 2.0f), decodeX(0.8f), decodeY(1.5f))
			path.curveTo(decodeAnchorX(0.800000011920929f, 0.0f), decodeAnchorY(1.5f, -2.0f), decodeAnchorX(0.800000011920929f, 0.0f), decodeAnchorY(1.0f, 1.0f), decodeX(0.8f), decodeY(1.0f))
			path.curveTo(decodeAnchorX(0.800000011920929f, 0.0f), decodeAnchorY(1.0f, -1.0f), decodeAnchorX(1.0f, -1.0f), decodeAnchorY(0.4000000059604645f, 1.0f), decodeX(1.0f), decodeY(0.4f))
			path.closePath()
			Return path
		End Function

		Private Function decodeEllipse2() As Ellipse2D
			ellipse.frameame(decodeX(0.6f), decodeY(0.2f), decodeX(0.8f) - decodeX(0.6f), decodeY(0.4f) - decodeY(0.2f)) 'height - width - y - x
			Return ellipse
		End Function

		Private Function decodeEllipse3() As Ellipse2D
			ellipse.frameame(decodeX(2.2f), decodeY(0.2f), decodeX(2.4f) - decodeX(2.2f), decodeY(0.4f) - decodeY(0.2f)) 'height - width - y - x
			Return ellipse
		End Function

		Private Function decodeEllipse4() As Ellipse2D
			ellipse.frameame(decodeX(2.2f), decodeY(1.0f), decodeX(2.4f) - decodeX(2.2f), decodeY(1.1666666f) - decodeY(1.0f)) 'height - width - y - x
			Return ellipse
		End Function

		Private Function decodeEllipse5() As Ellipse2D
			ellipse.frameame(decodeX(2.2f), decodeY(1.6666667f), decodeX(2.4f) - decodeX(2.2f), decodeY(1.8333333f) - decodeY(1.6666667f)) 'height - width - y - x
			Return ellipse
		End Function

		Private Function decodeEllipse6() As Ellipse2D
			ellipse.frameame(decodeX(0.6f), decodeY(1.6666667f), decodeX(0.8f) - decodeX(0.6f), decodeY(1.8333333f) - decodeY(1.6666667f)) 'height - width - y - x
			Return ellipse
		End Function

		Private Function decodeEllipse7() As Ellipse2D
			ellipse.frameame(decodeX(0.6f), decodeY(1.0f), decodeX(0.8f) - decodeX(0.6f), decodeY(1.1666666f) - decodeY(1.0f)) 'height - width - y - x
			Return ellipse
		End Function

		Private Function decodeRect11() As Rectangle2D
				rect.rectect(decodeX(0.8f), decodeY(2.2f), decodeX(1.0f) - decodeX(0.8f), decodeY(2.6f) - decodeY(2.2f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodeRect12() As Rectangle2D
				rect.rectect(decodeX(1.1666666f), decodeY(2.2f), decodeX(1.3333334f) - decodeX(1.1666666f), decodeY(2.6f) - decodeY(2.2f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodeRect13() As Rectangle2D
				rect.rectect(decodeX(1.5f), decodeY(2.2f), decodeX(1.6666667f) - decodeX(1.5f), decodeY(2.6f) - decodeY(2.2f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodePath25() As Path2D
			path.reset()
			path.moveTo(decodeX(0.0f), decodeY(0.2f))
			path.lineTo(decodeX(0.2f), decodeY(0.0f))
			path.lineTo(decodeX(2.6f), decodeY(0.0f))
			path.lineTo(decodeX(3.0f), decodeY(0.4f))
			path.lineTo(decodeX(3.0f), decodeY(2.8f))
			path.lineTo(decodeX(2.8f), decodeY(3.0f))
			path.lineTo(decodeX(0.2f), decodeY(3.0f))
			path.lineTo(decodeX(0.0f), decodeY(2.8f))
			path.lineTo(decodeX(0.0f), decodeY(0.2f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath26() As Path2D
			path.reset()
			path.moveTo(decodeX(0.2f), decodeY(0.4f))
			path.lineTo(decodeX(0.4f), decodeY(0.2f))
			path.lineTo(decodeX(2.4f), decodeY(0.2f))
			path.lineTo(decodeX(2.8f), decodeY(0.6f))
			path.lineTo(decodeX(2.8f), decodeY(2.8f))
			path.lineTo(decodeX(0.2f), decodeY(2.8f))
			path.lineTo(decodeX(0.2f), decodeY(0.4f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath27() As Path2D
			path.reset()
			path.moveTo(decodeX(0.8f), decodeY(1.6666667f))
			path.lineTo(decodeX(1.0f), decodeY(1.5f))
			path.lineTo(decodeX(2.0f), decodeY(1.5f))
			path.lineTo(decodeX(2.2f), decodeY(1.6666667f))
			path.lineTo(decodeX(2.2f), decodeY(2.6f))
			path.lineTo(decodeX(0.8f), decodeY(2.6f))
			path.lineTo(decodeX(0.8f), decodeY(1.6666667f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath28() As Path2D
			path.reset()
			path.moveTo(decodeX(1.1666666f), decodeY(0.2f))
			path.lineTo(decodeX(1.1666666f), decodeY(1.1666666f))
			path.lineTo(decodeX(2.2f), decodeY(1.1666666f))
			path.lineTo(decodeX(2.2f), decodeY(0.4f))
			path.lineTo(decodeX(2.0f), decodeY(0.4f))
			path.lineTo(decodeX(2.0f), decodeY(1.0f))
			path.lineTo(decodeX(1.6666667f), decodeY(1.0f))
			path.lineTo(decodeX(1.6666667f), decodeY(0.4f))
			path.lineTo(decodeX(2.2f), decodeY(0.4f))
			path.lineTo(decodeX(2.2f), decodeY(0.2f))
			path.lineTo(decodeX(1.1666666f), decodeY(0.2f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath29() As Path2D
			path.reset()
			path.moveTo(decodeX(0.8f), decodeY(0.2f))
			path.lineTo(decodeX(1.0f), decodeY(0.2f))
			path.lineTo(decodeX(1.0f), decodeY(1.0f))
			path.lineTo(decodeX(1.3333334f), decodeY(1.0f))
			path.lineTo(decodeX(1.3333334f), decodeY(0.2f))
			path.lineTo(decodeX(1.5f), decodeY(0.2f))
			path.lineTo(decodeX(1.5f), decodeY(1.0f))
			path.lineTo(decodeX(1.6666667f), decodeY(1.0f))
			path.lineTo(decodeX(1.6666667f), decodeY(1.1666666f))
			path.lineTo(decodeX(0.8f), decodeY(1.1666666f))
			path.lineTo(decodeX(0.8f), decodeY(0.2f))
			path.closePath()
			Return path
		End Function

		Private Function decodeRect14() As Rectangle2D
				rect.rectect(decodeX(0.8f), decodeY(2.6f), decodeX(2.2f) - decodeX(0.8f), decodeY(2.8f) - decodeY(2.6f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodeRect15() As Rectangle2D
				rect.rectect(decodeX(0.36153847f), decodeY(2.3576922f), decodeX(0.63461536f) - decodeX(0.36153847f), decodeY(2.6807692f) - decodeY(2.3576922f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodeRect16() As Rectangle2D
				rect.rectect(decodeX(2.376923f), decodeY(2.3807693f), decodeX(2.6384616f) - decodeX(2.376923f), decodeY(2.6846154f) - decodeY(2.3807693f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodeRect17() As Rectangle2D
				rect.rectect(decodeX(0.4f), decodeY(2.4f), decodeX(0.6f) - decodeX(0.4f), decodeY(2.6f) - decodeY(2.4f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodeRect18() As Rectangle2D
				rect.rectect(decodeX(2.4f), decodeY(2.4f), decodeX(2.6f) - decodeX(2.4f), decodeY(2.6f) - decodeY(2.4f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodePath30() As Path2D
			path.reset()
			path.moveTo(decodeX(0.4f), decodeY(1.5f))
			path.lineTo(decodeX(0.4f), decodeY(2.6f))
			path.lineTo(decodeX(0.6f), decodeY(2.8f))
			path.lineTo(decodeX(2.4f), decodeY(2.8f))
			path.lineTo(decodeX(2.6f), decodeY(2.6f))
			path.lineTo(decodeX(2.6f), decodeY(1.5f))
			path.lineTo(decodeX(0.4f), decodeY(1.5f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath31() As Path2D
			path.reset()
			path.moveTo(decodeX(0.6f), decodeY(1.5f))
			path.lineTo(decodeX(0.6f), decodeY(2.6f))
			path.lineTo(decodeX(2.4f), decodeY(2.6f))
			path.lineTo(decodeX(2.4f), decodeY(1.5f))
			path.lineTo(decodeX(1.5f), decodeY(0.8f))
			path.lineTo(decodeX(0.6f), decodeY(1.5f))
			path.closePath()
			Return path
		End Function

		Private Function decodeRect19() As Rectangle2D
				rect.rectect(decodeX(1.6666667f), decodeY(1.6666667f), decodeX(2.2f) - decodeX(1.6666667f), decodeY(2.2f) - decodeY(1.6666667f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodeRect20() As Rectangle2D
				rect.rectect(decodeX(1.8333333f), decodeY(1.8333333f), decodeX(2.0f) - decodeX(1.8333333f), decodeY(2.0f) - decodeY(1.8333333f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodePath32() As Path2D
			path.reset()
			path.moveTo(decodeX(1.0f), decodeY(2.8f))
			path.lineTo(decodeX(1.5f), decodeY(2.8f))
			path.lineTo(decodeX(1.5f), decodeY(1.8333333f))
			path.lineTo(decodeX(1.3333334f), decodeY(1.6666667f))
			path.lineTo(decodeX(1.1666666f), decodeY(1.6666667f))
			path.lineTo(decodeX(1.0f), decodeY(1.8333333f))
			path.lineTo(decodeX(1.0f), decodeY(2.8f))
			path.closePath()
			Return path
		End Function

		Private Function decodeRect21() As Rectangle2D
				rect.rectect(decodeX(1.1666666f), decodeY(1.8333333f), decodeX(1.3333334f) - decodeX(1.1666666f), decodeY(2.6f) - decodeY(1.8333333f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodePath33() As Path2D
			path.reset()
			path.moveTo(decodeX(0.0f), decodeY(1.3333334f))
			path.lineTo(decodeX(0.0f), decodeY(1.6666667f))
			path.lineTo(decodeX(0.4f), decodeY(1.6666667f))
			path.lineTo(decodeX(1.3974359f), decodeY(0.6f))
			path.lineTo(decodeX(1.596154f), decodeY(0.6f))
			path.lineTo(decodeX(2.6f), decodeY(1.6666667f))
			path.lineTo(decodeX(3.0f), decodeY(1.6666667f))
			path.lineTo(decodeX(3.0f), decodeY(1.3333334f))
			path.lineTo(decodeX(1.6666667f), decodeY(0.0f))
			path.lineTo(decodeX(1.3333334f), decodeY(0.0f))
			path.lineTo(decodeX(0.0f), decodeY(1.3333334f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath34() As Path2D
			path.reset()
			path.moveTo(decodeX(0.2576923f), decodeY(1.3717948f))
			path.lineTo(decodeX(0.2f), decodeY(1.5f))
			path.lineTo(decodeX(0.3230769f), decodeY(1.4711539f))
			path.lineTo(decodeX(1.4006411f), decodeY(0.40384617f))
			path.lineTo(decodeX(1.5929487f), decodeY(0.4f))
			path.lineTo(decodeX(2.6615386f), decodeY(1.4615384f))
			path.lineTo(decodeX(2.8f), decodeY(1.5f))
			path.lineTo(decodeX(2.7461538f), decodeY(1.3653846f))
			path.lineTo(decodeX(1.6089742f), decodeY(0.19615385f))
			path.lineTo(decodeX(1.4070512f), decodeY(0.2f))
			path.lineTo(decodeX(0.2576923f), decodeY(1.3717948f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath35() As Path2D
			path.reset()
			path.moveTo(decodeX(0.6f), decodeY(1.5f))
			path.lineTo(decodeX(1.3333334f), decodeY(0.6f))
			path.lineTo(decodeX(1.5f), decodeY(0.6f))
			path.lineTo(decodeX(1.5f), decodeY(1.1666666f))
			path.lineTo(decodeX(1.0f), decodeY(1.6666667f))
			path.lineTo(decodeX(0.6f), decodeY(1.6666667f))
			path.lineTo(decodeX(0.6f), decodeY(1.5f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath36() As Path2D
			path.reset()
			path.moveTo(decodeX(1.6666667f), decodeY(0.6f))
			path.lineTo(decodeX(1.5f), decodeY(0.6f))
			path.lineTo(decodeX(1.5f), decodeY(1.1666666f))
			path.lineTo(decodeX(2.0f), decodeY(1.6666667f))
			path.lineTo(decodeX(2.4f), decodeY(1.6666667f))
			path.lineTo(decodeX(2.4f), decodeY(1.3333334f))
			path.lineTo(decodeX(1.6666667f), decodeY(0.6f))
			path.closePath()
			Return path
		End Function

		Private Function decodeRect22() As Rectangle2D
				rect.rectect(decodeX(0.2f), decodeY(0.0f), decodeX(3.0f) - decodeX(0.2f), decodeY(2.8f) - decodeY(0.0f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodeRect23() As Rectangle2D
				rect.rectect(decodeX(0.4f), decodeY(0.2f), decodeX(2.8f) - decodeX(0.4f), decodeY(2.6f) - decodeY(0.2f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodeRect24() As Rectangle2D
				rect.rectect(decodeX(1.0f), decodeY(0.6f), decodeX(1.3333334f) - decodeX(1.0f), decodeY(0.8f) - decodeY(0.6f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodeRect25() As Rectangle2D
				rect.rectect(decodeX(1.5f), decodeY(1.3333334f), decodeX(2.4f) - decodeX(1.5f), decodeY(1.5f) - decodeY(1.3333334f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodeRect26() As Rectangle2D
				rect.rectect(decodeX(1.5f), decodeY(2.0f), decodeX(2.4f) - decodeX(1.5f), decodeY(2.2f) - decodeY(2.0f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodeEllipse8() As Ellipse2D
			ellipse.frameame(decodeX(0.6f), decodeY(0.8f), decodeX(2.2f) - decodeX(0.6f), decodeY(2.4f) - decodeY(0.8f)) 'height - width - y - x
			Return ellipse
		End Function

		Private Function decodeEllipse9() As Ellipse2D
			ellipse.frameame(decodeX(0.8f), decodeY(1.0f), decodeX(2.0f) - decodeX(0.8f), decodeY(2.2f) - decodeY(1.0f)) 'height - width - y - x
			Return ellipse
		End Function

		Private Function decodePath37() As Path2D
			path.reset()
			path.moveTo(decodeX(0.0f), decodeY(2.8f))
			path.lineTo(decodeX(0.0f), decodeY(3.0f))
			path.lineTo(decodeX(0.4f), decodeY(3.0f))
			path.lineTo(decodeX(1.0f), decodeY(2.2f))
			path.lineTo(decodeX(0.8f), decodeY(1.8333333f))
			path.lineTo(decodeX(0.0f), decodeY(2.8f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath38() As Path2D
			path.reset()
			path.moveTo(decodeX(0.1826087f), decodeY(2.7217393f))
			path.lineTo(decodeX(0.2826087f), decodeY(2.8217392f))
			path.lineTo(decodeX(1.0181159f), decodeY(2.095652f))
			path.lineTo(decodeX(0.9130435f), decodeY(1.9891305f))
			path.lineTo(decodeX(0.1826087f), decodeY(2.7217393f))
			path.closePath()
			Return path
		End Function

		Private Function decodeRect27() As Rectangle2D
				rect.rectect(decodeX(1.0f), decodeY(1.3333334f), decodeX(1.3333334f) - decodeX(1.0f), decodeY(1.5f) - decodeY(1.3333334f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodeRect28() As Rectangle2D
				rect.rectect(decodeX(1.5f), decodeY(1.3333334f), decodeX(1.8333333f) - decodeX(1.5f), decodeY(1.5f) - decodeY(1.3333334f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodeRect29() As Rectangle2D
				rect.rectect(decodeX(1.5f), decodeY(1.6666667f), decodeX(1.8333333f) - decodeX(1.5f), decodeY(1.8333333f) - decodeY(1.6666667f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodeRect30() As Rectangle2D
				rect.rectect(decodeX(1.0f), decodeY(1.6666667f), decodeX(1.3333334f) - decodeX(1.0f), decodeY(1.8333333f) - decodeY(1.6666667f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodeRect31() As Rectangle2D
				rect.rectect(decodeX(0.0f), decodeY(0.0f), decodeX(3.0f) - decodeX(0.0f), decodeY(2.8f) - decodeY(0.0f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodeRect32() As Rectangle2D
				rect.rectect(decodeX(0.2f), decodeY(0.2f), decodeX(2.8f) - decodeX(0.2f), decodeY(2.6f) - decodeY(0.2f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodeRect33() As Rectangle2D
				rect.rectect(decodeX(0.8f), decodeY(0.6f), decodeX(1.1666666f) - decodeX(0.8f), decodeY(0.8f) - decodeY(0.6f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodeRect34() As Rectangle2D
				rect.rectect(decodeX(1.3333334f), decodeY(0.6f), decodeX(2.2f) - decodeX(1.3333334f), decodeY(0.8f) - decodeY(0.6f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodeRect35() As Rectangle2D
				rect.rectect(decodeX(1.3333334f), decodeY(1.0f), decodeX(2.0f) - decodeX(1.3333334f), decodeY(1.1666666f) - decodeY(1.0f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodeRect36() As Rectangle2D
				rect.rectect(decodeX(0.8f), decodeY(1.0f), decodeX(1.1666666f) - decodeX(0.8f), decodeY(1.1666666f) - decodeY(1.0f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodeRect37() As Rectangle2D
				rect.rectect(decodeX(0.8f), decodeY(1.3333334f), decodeX(1.1666666f) - decodeX(0.8f), decodeY(1.5f) - decodeY(1.3333334f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodeRect38() As Rectangle2D
				rect.rectect(decodeX(1.3333334f), decodeY(1.3333334f), decodeX(2.2f) - decodeX(1.3333334f), decodeY(1.5f) - decodeY(1.3333334f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodeRect39() As Rectangle2D
				rect.rectect(decodeX(0.8f), decodeY(1.6666667f), decodeX(1.1666666f) - decodeX(0.8f), decodeY(1.8333333f) - decodeY(1.6666667f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodeRect40() As Rectangle2D
				rect.rectect(decodeX(1.3333334f), decodeY(1.6666667f), decodeX(2.0f) - decodeX(1.3333334f), decodeY(1.8333333f) - decodeY(1.6666667f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodeRect41() As Rectangle2D
				rect.rectect(decodeX(1.3333334f), decodeY(2.0f), decodeX(2.2f) - decodeX(1.3333334f), decodeY(2.2f) - decodeY(2.0f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodeRect42() As Rectangle2D
				rect.rectect(decodeX(0.8f), decodeY(2.0f), decodeX(1.1666666f) - decodeX(0.8f), decodeY(2.2f) - decodeY(2.0f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodeRect43() As Rectangle2D
				rect.rectect(decodeX(0.8f), decodeY(0.8f), decodeX(1.1666666f) - decodeX(0.8f), decodeY(1.0f) - decodeY(0.8f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodeRect44() As Rectangle2D
				rect.rectect(decodeX(1.3333334f), decodeY(0.8f), decodeX(2.2f) - decodeX(1.3333334f), decodeY(1.0f) - decodeY(0.8f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodeRect45() As Rectangle2D
				rect.rectect(decodeX(0.8f), decodeY(1.1666666f), decodeX(1.1666666f) - decodeX(0.8f), decodeY(1.3333334f) - decodeY(1.1666666f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodeRect46() As Rectangle2D
				rect.rectect(decodeX(1.3333334f), decodeY(1.1666666f), decodeX(2.0f) - decodeX(1.3333334f), decodeY(1.3333334f) - decodeY(1.1666666f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodeRect47() As Rectangle2D
				rect.rectect(decodeX(0.8f), decodeY(1.5f), decodeX(1.1666666f) - decodeX(0.8f), decodeY(1.6666667f) - decodeY(1.5f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodeRect48() As Rectangle2D
				rect.rectect(decodeX(1.3333334f), decodeY(1.5f), decodeX(2.2f) - decodeX(1.3333334f), decodeY(1.6666667f) - decodeY(1.5f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodeRect49() As Rectangle2D
				rect.rectect(decodeX(1.3333334f), decodeY(1.8333333f), decodeX(2.0f) - decodeX(1.3333334f), decodeY(2.0f) - decodeY(1.8333333f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodeRect50() As Rectangle2D
				rect.rectect(decodeX(0.8f), decodeY(1.8333333f), decodeX(1.1666666f) - decodeX(0.8f), decodeY(2.0f) - decodeY(1.8333333f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodeRect51() As Rectangle2D
				rect.rectect(decodeX(0.8f), decodeY(2.2f), decodeX(1.1666666f) - decodeX(0.8f), decodeY(2.4f) - decodeY(2.2f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodeRect52() As Rectangle2D
				rect.rectect(decodeX(1.3333334f), decodeY(2.2f), decodeX(2.2f) - decodeX(1.3333334f), decodeY(2.4f) - decodeY(2.2f)) 'height - width - y - x
			Return rect
		End Function



		Private Function decodeGradient1(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.046296295f * w) + x, (0.9675926f * h) + y, (0.4861111f * w) + x, (0.5324074f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color4, decodeColor(color4,color5,0.5f), color5})
		End Function

		Private Function decodeGradient2(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color6, decodeColor(color6,color7,0.5f), color7})
		End Function

		Private Function decodeGradient3(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.04191617f,0.10329342f,0.16467066f,0.24550897f,0.3263473f,0.6631737f,1.0f }, New Color() { color11, decodeColor(color11,color12,0.5f), color12, decodeColor(color12,color13,0.5f), color13, decodeColor(color13,color14,0.5f), color14})
		End Function

		Private Function decodeGradient4(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color6, decodeColor(color6,color15,0.5f), color15})
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
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.12724552f,0.25449103f,0.62724555f,1.0f }, New Color() { color21, decodeColor(color21,color22,0.5f), color22, decodeColor(color22,color23,0.5f), color23})
		End Function

		Private Function decodeGradient7(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.06392045f,0.1278409f,0.5213069f,0.91477275f }, New Color() { color25, decodeColor(color25,color26,0.5f), color26, decodeColor(color26,color27,0.5f), color27})
		End Function

		Private Function decodeGradient8(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.048295453f,0.09659091f,0.5482955f,1.0f }, New Color() { color28, decodeColor(color28,color6,0.5f), color6, decodeColor(color6,color15,0.5f), color15})
		End Function

		Private Function decodeGradient9(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color29, decodeColor(color29,color30,0.5f), color30})
		End Function

		Private Function decodeGradient10(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.06534091f,0.13068181f,0.3096591f,0.48863637f,0.7443182f,1.0f }, New Color() { color11, decodeColor(color11,color12,0.5f), color12, decodeColor(color12,color31,0.5f), color31, decodeColor(color31,color14,0.5f), color14})
		End Function

		Private Function decodeGradient11(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color33, decodeColor(color33,color34,0.5f), color34})
		End Function

		Private Function decodeGradient12(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color36, decodeColor(color36,color37,0.5f), color37})
		End Function

		Private Function decodeRadial1(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeRadialGradient((0.5f * w) + x, (1.0f * h) + y, 0.53913116f, New Single() { 0.11290322f,0.17419355f,0.23548387f,0.31129032f,0.38709676f,0.47903225f,0.57096773f }, New Color() { color40, decodeColor(color40,color41,0.5f), color41, decodeColor(color41,color41,0.5f), color41, decodeColor(color41,color42,0.5f), color42})
		End Function

		Private Function decodeGradient13(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color45, decodeColor(color45,color46,0.5f), color46})
		End Function

		Private Function decodeGradient14(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color47, decodeColor(color47,color48,0.5f), color48})
		End Function

		Private Function decodeGradient15(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.3983871f,0.7967742f,0.8983871f,1.0f }, New Color() { color51, decodeColor(color51,color52,0.5f), color52, decodeColor(color52,color51,0.5f), color51})
		End Function

		Private Function decodeGradient16(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.061290324f,0.12258065f,0.5016129f,0.88064516f,0.9403226f,1.0f }, New Color() { color57, decodeColor(color57,color58,0.5f), color58, decodeColor(color58,color59,0.5f), color59, decodeColor(color59,color44,0.5f), color44})
		End Function

		Private Function decodeGradient17(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.05f,0.1f,0.19193548f,0.28387097f,0.5209677f,0.7580645f,0.87903225f,1.0f }, New Color() { color60, decodeColor(color60,color61,0.5f), color61, decodeColor(color61,color62,0.5f), color62, decodeColor(color62,color63,0.5f), color63, decodeColor(color63,color64,0.5f), color64})
		End Function

		Private Function decodeGradient18(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.058064517f,0.090322584f,0.12258065f,0.15645161f,0.19032258f,0.22741935f,0.26451612f,0.31290323f,0.36129034f,0.38225806f,0.4032258f,0.4596774f,0.516129f,0.54193544f,0.56774193f,0.61451614f,0.66129035f,0.70645165f,0.7516129f }, New Color() { color65, decodeColor(color65,color40,0.5f), color40, decodeColor(color40,color40,0.5f), color40, decodeColor(color40,color65,0.5f), color65, decodeColor(color65,color65,0.5f), color65, decodeColor(color65,color40,0.5f), color40, decodeColor(color40,color40,0.5f), color40, decodeColor(color40,color66,0.5f), color66, decodeColor(color66,color66,0.5f), color66, decodeColor(color66,color40,0.5f), color40})
		End Function

		Private Function decodeGradient19(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color67, decodeColor(color67,color67,0.5f), color67})
		End Function

		Private Function decodeGradient20(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color74, decodeColor(color74,color75,0.5f), color75})
		End Function

		Private Function decodeGradient21(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color77, decodeColor(color77,color78,0.5f), color78})
		End Function

		Private Function decodeGradient22(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color79, decodeColor(color79,color80,0.5f), color80})
		End Function

		Private Function decodeGradient23(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color81, decodeColor(color81,color82,0.5f), color82})
		End Function

		Private Function decodeGradient24(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.43076923f * w) + x, (0.37820512f * h) + y, (0.7076923f * w) + x, (0.6730769f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color84, decodeColor(color84,color85,0.5f), color85})
		End Function

		Private Function decodeGradient25(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.63076925f * w) + x, (0.3621795f * h) + y, (0.28846154f * w) + x, (0.73397434f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color84, decodeColor(color84,color86,0.5f), color86})
		End Function

		Private Function decodeGradient26(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color87, decodeColor(color87,color88,0.5f), color88})
		End Function

		Private Function decodeGradient27(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.056818184f,0.11363637f,0.34232956f,0.57102275f,0.7855114f,1.0f }, New Color() { color89, decodeColor(color89,color90,0.5f), color90, decodeColor(color90,color91,0.5f), color91, decodeColor(color91,color92,0.5f), color92})
		End Function

		Private Function decodeGradient28(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.25f * w) + x, (0.0f * h) + y, (0.75f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color95, decodeColor(color95,color96,0.5f), color96})
		End Function

		Private Function decodeRadial2(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeRadialGradient((0.49223602f * w) + x, (0.9751553f * h) + y, 0.73615754f, New Single() { 0.0f,0.40625f,1.0f }, New Color() { color97, decodeColor(color97,color98,0.5f), color98})
		End Function

		Private Function decodeGradient29(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.0f * w) + x, (0.0f * h) + y, (1.0f * w) + x, (1.0f * h) + y, New Single() { 0.38352272f,0.4190341f,0.45454547f,0.484375f,0.51420456f }, New Color() { color99, decodeColor(color99,color100,0.5f), color100, decodeColor(color100,color101,0.5f), color101})
		End Function

		Private Function decodeGradient30(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((1.0f * w) + x, (0.0f * h) + y, (0.0f * w) + x, (1.0f * h) + y, New Single() { 0.12215909f,0.16051137f,0.19886364f,0.2627841f,0.32670453f,0.43039775f,0.53409094f }, New Color() { color102, decodeColor(color102,color35,0.5f), color35, decodeColor(color35,color35,0.5f), color35, decodeColor(color35,color103,0.5f), color103})
		End Function

		Private Function decodeGradient31(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.038352273f,0.07670455f,0.24289773f,0.4090909f,0.7045455f,1.0f }, New Color() { color89, decodeColor(color89,color90,0.5f), color90, decodeColor(color90,color108,0.5f), color108, decodeColor(color108,color92,0.5f), color92})
		End Function

		Private Function decodeGradient32(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.0f * w) + x, (0.0f * h) + y, (1.0f * w) + x, (1.0f * h) + y, New Single() { 0.25f,0.33522725f,0.42045453f,0.50142044f,0.5823864f }, New Color() { color109, decodeColor(color109,color110,0.5f), color110, decodeColor(color110,color109,0.5f), color109})
		End Function

		Private Function decodeGradient33(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.25f * w) + x, (0.0f * h) + y, (0.75f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.24147727f,0.48295453f,0.74147725f,1.0f }, New Color() { color114, decodeColor(color114,color115,0.5f), color115, decodeColor(color115,color114,0.5f), color114})
		End Function

		Private Function decodeGradient34(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.0f * w) + x, (0.0f * h) + y, (1.0f * w) + x, (0.0f * h) + y, New Single() { 0.0f,0.21732955f,0.4346591f }, New Color() { color117, decodeColor(color117,color118,0.5f), color118})
		End Function

		Private Function decodeGradient35(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.0f * w) + x, (0.0f * h) + y, (1.0f * w) + x, (0.0f * h) + y, New Single() { 0.0f,0.21448864f,0.42897728f,0.7144886f,1.0f }, New Color() { color119, decodeColor(color119,color120,0.5f), color120, decodeColor(color120,color119,0.5f), color119})
		End Function


	End Class

End Namespace
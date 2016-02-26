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



	Friend NotInheritable Class ScrollBarTrackPainter
		Inherits AbstractRegionPainter

		'package private integers representing the available states that
		'this painter will paint. These are used when creating a new instance
		'of ScrollBarTrackPainter to determine which region/state is being painted
		'by that instance.
		Friend Const BACKGROUND_DISABLED As Integer = 1
		Friend Const BACKGROUND_ENABLED As Integer = 2


		Private state As Integer 'refers to one of the static final ints above
		Private ctx As PaintContext

		'the following 4 variables are reused during the painting code of the layers
		Private path As Path2D = New Path2D.Float
		Private rect As Rectangle2D = New Rectangle2D.Float(0, 0, 0, 0)
		Private roundRect As RoundRectangle2D = New RoundRectangle2D.Float(0, 0, 0, 0, 0, 0)
		Private ellipse As Ellipse2D = New Ellipse2D.Float(0, 0, 0, 0)

		'All Colors used for painting are stored here. Ideally, only those colors being used
		'by a particular instance of ScrollBarTrackPainter would be created. For the moment at least,
		'however, all are created for each instance.
		Private color1 As Color = decodeColor("nimbusBlueGrey", -0.027777791f, -0.10016362f, 0.011764705f, 0)
		Private color2 As Color = decodeColor("nimbusBlueGrey", -0.027777791f, -0.100476064f, 0.035294116f, 0)
		Private color3 As Color = decodeColor("nimbusBlueGrey", 0.055555582f, -0.10606203f, 0.13333333f, 0)
		Private color4 As Color = decodeColor("nimbusBlueGrey", -0.6111111f, -0.110526316f, 0.24705881f, 0)
		Private color5 As Color = decodeColor("nimbusBlueGrey", 0.02222228f, -0.06465475f, -0.31764707f, 0)
		Private color6 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.06766917f, -0.19607842f, 0)
		Private color7 As Color = decodeColor("nimbusBlueGrey", -0.006944418f, -0.0655825f, -0.04705882f, 0)
		Private color8 As Color = decodeColor("nimbusBlueGrey", 0.0138888955f, -0.071117446f, 0.05098039f, 0)
		Private color9 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.07016757f, 0.12941176f, 0)
		Private color10 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.05967886f, -0.5137255f, 0)
		Private color11 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.05967886f, -0.5137255f, -255)
		Private color12 As Color = decodeColor("nimbusBlueGrey", -0.027777791f, -0.07826825f, -0.5019608f, -255)
		Private color13 As Color = decodeColor("nimbusBlueGrey", -0.015872955f, -0.06731644f, -0.109803915f, 0)
		Private color14 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.06924191f, 0.109803915f, 0)
		Private color15 As Color = decodeColor("nimbusBlueGrey", -0.015872955f, -0.06861015f, -0.09019607f, 0)
		Private color16 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.06766917f, 0.07843137f, 0)


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

			End Select
		End Sub



		Protected Friend Property NotOverridable Overrides paintContext As PaintContext
			Get
				Return ctx
			End Get
		End Property

		Private Sub paintBackgroundDisabled(ByVal g As Graphics2D)
			rect = decodeRect1()
			g.paint = decodeGradient1(rect)
			g.fill(rect)

		End Sub

		Private Sub paintBackgroundEnabled(ByVal g As Graphics2D)
			rect = decodeRect1()
			g.paint = decodeGradient2(rect)
			g.fill(rect)
			path = decodePath1()
			g.paint = decodeGradient3(path)
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

		End Sub



		Private Function decodeRect1() As Rectangle2D
				rect.rectect(decodeX(0.0f), decodeY(0.0f), decodeX(3.0f) - decodeX(0.0f), decodeY(3.0f) - decodeY(0.0f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodePath1() As Path2D
			path.reset()
			path.moveTo(decodeX(0.7f), decodeY(0.0f))
			path.lineTo(decodeX(0.0f), decodeY(0.0f))
			path.lineTo(decodeX(0.0f), decodeY(1.2f))
			path.curveTo(decodeAnchorX(0.0f, 0.0f), decodeAnchorY(1.2000000476837158f, 0.0f), decodeAnchorX(0.30000001192092896f, -1.0f), decodeAnchorY(2.200000047683716f, -1.0f), decodeX(0.3f), decodeY(2.2f))
			path.curveTo(decodeAnchorX(0.30000001192092896f, 1.0f), decodeAnchorY(2.200000047683716f, 1.0f), decodeAnchorX(0.6785714030265808f, 0.0f), decodeAnchorY(2.799999952316284f, 0.0f), decodeX(0.6785714f), decodeY(2.8f))
			path.lineTo(decodeX(0.7f), decodeY(0.0f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath2() As Path2D
			path.reset()
			path.moveTo(decodeX(3.0f), decodeY(0.0f))
			path.lineTo(decodeX(2.2222223f), decodeY(0.0f))
			path.lineTo(decodeX(2.2222223f), decodeY(2.8f))
			path.curveTo(decodeAnchorX(2.222222328186035f, 0.0f), decodeAnchorY(2.799999952316284f, 0.0f), decodeAnchorX(2.674603223800659f, -1.0f), decodeAnchorY(2.1857142448425293f, 1.0f), decodeX(2.6746032f), decodeY(2.1857142f))
			path.curveTo(decodeAnchorX(2.674603223800659f, 1.0000000000000036f), decodeAnchorY(2.1857142448425293f, -1.0f), decodeAnchorX(3.0f, 0.0f), decodeAnchorY(1.2000000476837158f, 0.0f), decodeX(3.0f), decodeY(1.2f))
			path.lineTo(decodeX(3.0f), decodeY(0.0f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath3() As Path2D
			path.reset()
			path.moveTo(decodeX(0.11428572f), decodeY(1.3714286f))
			path.curveTo(decodeAnchorX(0.11428572237491608f, 0.7857142857142856f), decodeAnchorY(1.3714286088943481f, -0.571428571428573f), decodeAnchorX(0.4642857015132904f, -1.3571428571428572f), decodeAnchorY(2.0714285373687744f, -1.5714285714285694f), decodeX(0.4642857f), decodeY(2.0714285f))
			path.curveTo(decodeAnchorX(0.4642857015132904f, 1.3571428571428577f), decodeAnchorY(2.0714285373687744f, 1.5714285714285694f), decodeAnchorX(0.8714286088943481f, 0.21428571428571352f), decodeAnchorY(2.7285714149475098f, -1.0f), decodeX(0.8714286f), decodeY(2.7285714f))
			path.curveTo(decodeAnchorX(0.8714286088943481f, -0.21428571428571352f), decodeAnchorY(2.7285714149475098f, 1.0f), decodeAnchorX(0.3571428656578064f, 1.5000000000000004f), decodeAnchorY(2.3142857551574707f, 1.642857142857146f), decodeX(0.35714287f), decodeY(2.3142858f))
			path.curveTo(decodeAnchorX(0.3571428656578064f, -1.5000000000000004f), decodeAnchorY(2.3142857551574707f, -1.642857142857146f), decodeAnchorX(0.11428572237491608f, -0.7857142857142856f), decodeAnchorY(1.3714286088943481f, 0.571428571428573f), decodeX(0.11428572f), decodeY(1.3714286f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath4() As Path2D
			path.reset()
			path.moveTo(decodeX(2.1111112f), decodeY(2.7f))
			path.curveTo(decodeAnchorX(2.1111111640930176f, 0.4285714285714306f), decodeAnchorY(2.700000047683716f, 0.6428571428571388f), decodeAnchorX(2.626984119415283f, -1.571428571428573f), decodeAnchorY(2.200000047683716f, 1.6428571428571388f), decodeX(2.6269841f), decodeY(2.2f))
			path.curveTo(decodeAnchorX(2.626984119415283f, 1.571428571428573f), decodeAnchorY(2.200000047683716f, -1.6428571428571388f), decodeAnchorX(2.8412699699401855f, 0.7142857142857224f), decodeAnchorY(1.3857142925262451f, 0.6428571428571459f), decodeX(2.84127f), decodeY(1.3857143f))
			path.curveTo(decodeAnchorX(2.8412699699401855f, -0.7142857142857224f), decodeAnchorY(1.3857142925262451f, -0.6428571428571459f), decodeAnchorX(2.5238094329833984f, 0.7142857142857117f), decodeAnchorY(2.057142734527588f, -0.8571428571428541f), decodeX(2.5238094f), decodeY(2.0571427f))
			path.curveTo(decodeAnchorX(2.5238094329833984f, -0.7142857142857117f), decodeAnchorY(2.057142734527588f, 0.8571428571428541f), decodeAnchorX(2.1111111640930176f, -0.4285714285714306f), decodeAnchorY(2.700000047683716f, -0.6428571428571388f), decodeX(2.1111112f), decodeY(2.7f))
			path.closePath()
			Return path
		End Function



		Private Function decodeGradient1(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.016129032f,0.038709678f,0.061290324f,0.16091082f,0.26451612f,0.4378071f,0.88387096f }, New Color() { color1, decodeColor(color1,color2,0.5f), color2, decodeColor(color2,color3,0.5f), color3, decodeColor(color3,color4,0.5f), color4})
		End Function

		Private Function decodeGradient2(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.030645162f,0.061290324f,0.09677419f,0.13225806f,0.22096774f,0.30967742f,0.47434634f,0.82258064f }, New Color() { color5, decodeColor(color5,color6,0.5f), color6, decodeColor(color6,color7,0.5f), color7, decodeColor(color7,color8,0.5f), color8, decodeColor(color8,color9,0.5f), color9})
		End Function

		Private Function decodeGradient3(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.0f * w) + x, (0.0f * h) + y, (0.9285714f * w) + x, (0.12244898f * h) + y, New Single() { 0.0f,0.1f,1.0f }, New Color() { color10, decodeColor(color10,color11,0.5f), color11})
		End Function

		Private Function decodeGradient4(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((-0.045918368f * w) + x, (0.18336426f * h) + y, (0.872449f * w) + x, (0.04050711f * h) + y, New Single() { 0.0f,0.87096775f,1.0f }, New Color() { color12, decodeColor(color12,color10,0.5f), color10})
		End Function

		Private Function decodeGradient5(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.12719299f * w) + x, (0.13157894f * h) + y, (0.90789473f * w) + x, (0.877193f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color13, decodeColor(color13,color14,0.5f), color14})
		End Function

		Private Function decodeGradient6(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.86458343f * w) + x, (0.20952381f * h) + y, (0.020833189f * w) + x, (0.95238096f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color15, decodeColor(color15,color16,0.5f), color16})
		End Function


	End Class

End Namespace
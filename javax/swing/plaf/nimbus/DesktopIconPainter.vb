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



	Friend NotInheritable Class DesktopIconPainter
		Inherits AbstractRegionPainter

		'package private integers representing the available states that
		'this painter will paint. These are used when creating a new instance
		'of DesktopIconPainter to determine which region/state is being painted
		'by that instance.
		Friend Const BACKGROUND_ENABLED As Integer = 1


		Private state As Integer 'refers to one of the static final ints above
		Private ctx As PaintContext

		'the following 4 variables are reused during the painting code of the layers
		Private path As Path2D = New Path2D.Float
		Private rect As Rectangle2D = New Rectangle2D.Float(0, 0, 0, 0)
		Private roundRect As RoundRectangle2D = New RoundRectangle2D.Float(0, 0, 0, 0, 0, 0)
		Private ellipse As Ellipse2D = New Ellipse2D.Float(0, 0, 0, 0)

		'All Colors used for painting are stored here. Ideally, only those colors being used
		'by a particular instance of DesktopIconPainter would be created. For the moment at least,
		'however, all are created for each instance.
		Private color1 As Color = decodeColor("nimbusBase", 0.02551502f, -0.47885156f, -0.34901965f, 0)
		Private color2 As Color = decodeColor("nimbusBlueGrey", -0.027777791f, -0.102261856f, 0.20392156f, 0)
		Private color3 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.0682728f, 0.09019607f, 0)
		Private color4 As Color = decodeColor("nimbusBlueGrey", -0.01111114f, -0.088974595f, 0.16470587f, 0)
		Private color5 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.029445238f, -0.019607842f, 0)


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

			End Select
		End Sub



		Protected Friend Property NotOverridable Overrides paintContext As PaintContext
			Get
				Return ctx
			End Get
		End Property

		Private Sub paintBackgroundEnabled(ByVal g As Graphics2D)
			roundRect = decodeRoundRect1()
			g.paint = color1
			g.fill(roundRect)
			roundRect = decodeRoundRect2()
			g.paint = decodeGradient1(roundRect)
			g.fill(roundRect)
			rect = decodeRect1()
			g.paint = decodeGradient2(rect)
			g.fill(rect)

		End Sub



		Private Function decodeRoundRect1() As RoundRectangle2D
			roundRect.roundRectect(decodeX(0.4f), decodeY(0.0f), decodeX(2.8f) - decodeX(0.4f), decodeY(2.6f) - decodeY(0.0f), 4.8333335f, 4.8333335f) 'rounding - height - width - y - x
			Return roundRect
		End Function

		Private Function decodeRoundRect2() As RoundRectangle2D
			roundRect.roundRectect(decodeX(0.6f), decodeY(0.2f), decodeX(2.8f) - decodeX(0.6f), decodeY(2.4f) - decodeY(0.2f), 3.1f, 3.1f) 'rounding - height - width - y - x
			Return roundRect
		End Function

		Private Function decodeRect1() As Rectangle2D
				rect.rectect(decodeX(0.8f), decodeY(0.4f), decodeX(2.4f) - decodeX(0.8f), decodeY(2.2f) - decodeY(0.4f)) 'height - width - y - x
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
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.24f,1.0f }, New Color() { color4, decodeColor(color4,color5,0.5f), color5})
		End Function


	End Class

End Namespace
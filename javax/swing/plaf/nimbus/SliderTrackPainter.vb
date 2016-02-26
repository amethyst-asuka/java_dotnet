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



	Friend NotInheritable Class SliderTrackPainter
		Inherits AbstractRegionPainter

		'package private integers representing the available states that
		'this painter will paint. These are used when creating a new instance
		'of SliderTrackPainter to determine which region/state is being painted
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
		'by a particular instance of SliderTrackPainter would be created. For the moment at least,
		'however, all are created for each instance.
		Private color1 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.110526316f, 0.25490195f, -245)
		Private color2 As Color = decodeColor("nimbusBlueGrey", 0.0055555105f, -0.061265234f, 0.05098039f, 0)
		Private color3 As Color = decodeColor("nimbusBlueGrey", 0.01010108f, -0.059835073f, 0.10588235f, 0)
		Private color4 As Color = decodeColor("nimbusBlueGrey", -0.01111114f, -0.061982628f, 0.062745094f, 0)
		Private color5 As Color = decodeColor("nimbusBlueGrey", -0.00505054f, -0.058639523f, 0.086274505f, 0)
		Private color6 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.110526316f, 0.25490195f, -111)
		Private color7 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.034093194f, -0.12941176f, 0)
		Private color8 As Color = decodeColor("nimbusBlueGrey", 0.01111114f, -0.023821115f, -0.06666666f, 0)
		Private color9 As Color = decodeColor("nimbusBlueGrey", -0.008547008f, -0.03314536f, -0.086274505f, 0)
		Private color10 As Color = decodeColor("nimbusBlueGrey", 0.004273474f, -0.040256046f, -0.019607842f, 0)
		Private color11 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.03626889f, 0.04705882f, 0)


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
			roundRect = decodeRoundRect1()
			g.paint = color1
			g.fill(roundRect)
			roundRect = decodeRoundRect2()
			g.paint = decodeGradient1(roundRect)
			g.fill(roundRect)
			roundRect = decodeRoundRect3()
			g.paint = decodeGradient2(roundRect)
			g.fill(roundRect)

		End Sub

		Private Sub paintBackgroundEnabled(ByVal g As Graphics2D)
			roundRect = decodeRoundRect4()
			g.paint = color6
			g.fill(roundRect)
			roundRect = decodeRoundRect2()
			g.paint = decodeGradient3(roundRect)
			g.fill(roundRect)
			roundRect = decodeRoundRect5()
			g.paint = decodeGradient4(roundRect)
			g.fill(roundRect)

		End Sub



		Private Function decodeRoundRect1() As RoundRectangle2D
			roundRect.roundRectect(decodeX(0.2f), decodeY(1.6f), decodeX(2.8f) - decodeX(0.2f), decodeY(2.8333333f) - decodeY(1.6f), 8.705882f, 8.705882f) 'rounding - height - width - y - x
			Return roundRect
		End Function

		Private Function decodeRoundRect2() As RoundRectangle2D
			roundRect.roundRectect(decodeX(0.0f), decodeY(1.0f), decodeX(3.0f) - decodeX(0.0f), decodeY(2.0f) - decodeY(1.0f), 4.9411764f, 4.9411764f) 'rounding - height - width - y - x
			Return roundRect
		End Function

		Private Function decodeRoundRect3() As RoundRectangle2D
			roundRect.roundRectect(decodeX(0.29411763f), decodeY(1.2f), decodeX(2.7058823f) - decodeX(0.29411763f), decodeY(2.0f) - decodeY(1.2f), 4.0f, 4.0f) 'rounding - height - width - y - x
			Return roundRect
		End Function

		Private Function decodeRoundRect4() As RoundRectangle2D
			roundRect.roundRectect(decodeX(0.2f), decodeY(1.6f), decodeX(2.8f) - decodeX(0.2f), decodeY(2.1666667f) - decodeY(1.6f), 8.705882f, 8.705882f) 'rounding - height - width - y - x
			Return roundRect
		End Function

		Private Function decodeRoundRect5() As RoundRectangle2D
			roundRect.roundRectect(decodeX(0.28823528f), decodeY(1.2f), decodeX(2.7f) - decodeX(0.28823528f), decodeY(2.0f) - decodeY(1.2f), 4.0f, 4.0f) 'rounding - height - width - y - x
			Return roundRect
		End Function



		Private Function decodeGradient1(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.25f * w) + x, (0.07647059f * h) + y, (0.25f * w) + x, (0.9117647f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color2, decodeColor(color2,color3,0.5f), color3})
		End Function

		Private Function decodeGradient2(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.25f * w) + x, (0.0f * h) + y, (0.25f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.13770053f,0.27540106f,0.63770056f,1.0f }, New Color() { color4, decodeColor(color4,color5,0.5f), color5, decodeColor(color5,color3,0.5f), color3})
		End Function

		Private Function decodeGradient3(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.25f * w) + x, (0.07647059f * h) + y, (0.25f * w) + x, (0.9117647f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color7, decodeColor(color7,color8,0.5f), color8})
		End Function

		Private Function decodeGradient4(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.25f * w) + x, (0.0f * h) + y, (0.25f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.13770053f,0.27540106f,0.4906417f,0.7058824f }, New Color() { color9, decodeColor(color9,color10,0.5f), color10, decodeColor(color10,color11,0.5f), color11})
		End Function


	End Class

End Namespace
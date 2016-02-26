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



	Friend NotInheritable Class TableHeaderRendererPainter
		Inherits AbstractRegionPainter

		'package private integers representing the available states that
		'this painter will paint. These are used when creating a new instance
		'of TableHeaderRendererPainter to determine which region/state is being painted
		'by that instance.
		Friend Const BACKGROUND_DISABLED As Integer = 1
		Friend Const BACKGROUND_ENABLED As Integer = 2
		Friend Const BACKGROUND_ENABLED_FOCUSED As Integer = 3
		Friend Const BACKGROUND_MOUSEOVER As Integer = 4
		Friend Const BACKGROUND_PRESSED As Integer = 5
		Friend Const BACKGROUND_ENABLED_SORTED As Integer = 6
		Friend Const BACKGROUND_ENABLED_FOCUSED_SORTED As Integer = 7
		Friend Const BACKGROUND_DISABLED_SORTED As Integer = 8


		Private ___state As Integer 'refers to one of the static final ints above
		Private ctx As PaintContext

		'the following 4 variables are reused during the painting code of the layers
		Private path As Path2D = New Path2D.Float
		Private rect As Rectangle2D = New Rectangle2D.Float(0, 0, 0, 0)
		Private roundRect As RoundRectangle2D = New RoundRectangle2D.Float(0, 0, 0, 0, 0, 0)
		Private ellipse As Ellipse2D = New Ellipse2D.Float(0, 0, 0, 0)

		'All Colors used for painting are stored here. Ideally, only those colors being used
		'by a particular instance of TableHeaderRendererPainter would be created. For the moment at least,
		'however, all are created for each instance.
		Private color1 As Color = decodeColor("nimbusBorder", -0.013888836f, 5.823001E-4f, -0.12941176f, 0)
		Private color2 As Color = decodeColor("nimbusBlueGrey", -0.01111114f, -0.08625447f, 0.062745094f, 0)
		Private color3 As Color = decodeColor("nimbusBlueGrey", -0.013888836f, -0.028334536f, -0.17254901f, 0)
		Private color4 As Color = decodeColor("nimbusBlueGrey", -0.013888836f, -0.029445238f, -0.16470587f, 0)
		Private color5 As Color = decodeColor("nimbusBlueGrey", -0.02020204f, -0.053531498f, 0.011764705f, 0)
		Private color6 As Color = decodeColor("nimbusBlueGrey", 0.055555582f, -0.10655806f, 0.24313724f, 0)
		Private color7 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.08455229f, 0.1607843f, 0)
		Private color8 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.07016757f, 0.12941176f, 0)
		Private color9 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.07466974f, 0.23921567f, 0)
		Private color10 As Color = decodeColor("nimbusFocus", 0.0f, 0.0f, 0.0f, 0)
		Private color11 As Color = decodeColor("nimbusBlueGrey", 0.055555582f, -0.10658931f, 0.25098038f, 0)
		Private color12 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.08613607f, 0.21960783f, 0)
		Private color13 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.07333623f, 0.20392156f, 0)
		Private color14 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.110526316f, 0.25490195f, 0)
		Private color15 As Color = decodeColor("nimbusBlueGrey", -0.00505054f, -0.05960039f, 0.10196078f, 0)
		Private color16 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.017742813f, 0.015686274f, 0)
		Private color17 As Color = decodeColor("nimbusBlueGrey", -0.0027777553f, -0.0018306673f, -0.02352941f, 0)
		Private color18 As Color = decodeColor("nimbusBlueGrey", 0.0055555105f, -0.020436227f, 0.12549019f, 0)
		Private color19 As Color = decodeColor("nimbusBase", -0.023096085f, -0.62376213f, 0.4352941f, 0)
		Private color20 As Color = decodeColor("nimbusBase", -0.0012707114f, -0.50901747f, 0.31764704f, 0)
		Private color21 As Color = decodeColor("nimbusBase", -0.002461195f, -0.47139505f, 0.2862745f, 0)
		Private color22 As Color = decodeColor("nimbusBase", -0.0051222444f, -0.49103343f, 0.372549f, 0)
		Private color23 As Color = decodeColor("nimbusBase", -8.738637E-4f, -0.49872798f, 0.3098039f, 0)
		Private color24 As Color = decodeColor("nimbusBase", -2.2029877E-4f, -0.4916465f, 0.37647057f, 0)


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
				Case BACKGROUND_DISABLED
					paintBackgroundDisabled(g)
				Case BACKGROUND_ENABLED
					paintBackgroundEnabled(g)
				Case BACKGROUND_ENABLED_FOCUSED
					paintBackgroundEnabledAndFocused(g)
				Case BACKGROUND_MOUSEOVER
					paintBackgroundMouseOver(g)
				Case BACKGROUND_PRESSED
					paintBackgroundPressed(g)
				Case BACKGROUND_ENABLED_SORTED
					paintBackgroundEnabledAndSorted(g)
				Case BACKGROUND_ENABLED_FOCUSED_SORTED
					paintBackgroundEnabledAndFocusedAndSorted(g)
				Case BACKGROUND_DISABLED_SORTED
					paintBackgroundDisabledAndSorted(g)

			End Select
		End Sub



		Protected Friend Property NotOverridable Overrides paintContext As PaintContext
			Get
				Return ctx
			End Get
		End Property

		Private Sub paintBackgroundDisabled(ByVal g As Graphics2D)
			rect = decodeRect1()
			g.paint = color1
			g.fill(rect)
			rect = decodeRect2()
			g.paint = decodeGradient1(rect)
			g.fill(rect)
			rect = decodeRect3()
			g.paint = decodeGradient2(rect)
			g.fill(rect)

		End Sub

		Private Sub paintBackgroundEnabled(ByVal g As Graphics2D)
			rect = decodeRect1()
			g.paint = color1
			g.fill(rect)
			rect = decodeRect2()
			g.paint = decodeGradient1(rect)
			g.fill(rect)
			rect = decodeRect3()
			g.paint = decodeGradient2(rect)
			g.fill(rect)

		End Sub

		Private Sub paintBackgroundEnabledAndFocused(ByVal g As Graphics2D)
			rect = decodeRect1()
			g.paint = color1
			g.fill(rect)
			rect = decodeRect2()
			g.paint = decodeGradient1(rect)
			g.fill(rect)
			rect = decodeRect3()
			g.paint = decodeGradient2(rect)
			g.fill(rect)
			path = decodePath1()
			g.paint = color10
			g.fill(path)

		End Sub

		Private Sub paintBackgroundMouseOver(ByVal g As Graphics2D)
			rect = decodeRect1()
			g.paint = color1
			g.fill(rect)
			rect = decodeRect2()
			g.paint = decodeGradient1(rect)
			g.fill(rect)
			rect = decodeRect3()
			g.paint = decodeGradient3(rect)
			g.fill(rect)

		End Sub

		Private Sub paintBackgroundPressed(ByVal g As Graphics2D)
			rect = decodeRect1()
			g.paint = color1
			g.fill(rect)
			rect = decodeRect2()
			g.paint = decodeGradient1(rect)
			g.fill(rect)
			rect = decodeRect3()
			g.paint = decodeGradient4(rect)
			g.fill(rect)

		End Sub

		Private Sub paintBackgroundEnabledAndSorted(ByVal g As Graphics2D)
			rect = decodeRect1()
			g.paint = color1
			g.fill(rect)
			rect = decodeRect2()
			g.paint = decodeGradient1(rect)
			g.fill(rect)
			rect = decodeRect3()
			g.paint = decodeGradient5(rect)
			g.fill(rect)

		End Sub

		Private Sub paintBackgroundEnabledAndFocusedAndSorted(ByVal g As Graphics2D)
			rect = decodeRect1()
			g.paint = color1
			g.fill(rect)
			rect = decodeRect2()
			g.paint = decodeGradient1(rect)
			g.fill(rect)
			rect = decodeRect3()
			g.paint = decodeGradient6(rect)
			g.fill(rect)
			path = decodePath1()
			g.paint = color10
			g.fill(path)

		End Sub

		Private Sub paintBackgroundDisabledAndSorted(ByVal g As Graphics2D)
			rect = decodeRect1()
			g.paint = color1
			g.fill(rect)
			rect = decodeRect2()
			g.paint = decodeGradient1(rect)
			g.fill(rect)
			rect = decodeRect3()
			g.paint = decodeGradient2(rect)
			g.fill(rect)

		End Sub



		Private Function decodeRect1() As Rectangle2D
				rect.rectect(decodeX(0.0f), decodeY(2.8f), decodeX(3.0f) - decodeX(0.0f), decodeY(3.0f) - decodeY(2.8f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodeRect2() As Rectangle2D
				rect.rectect(decodeX(2.8f), decodeY(0.0f), decodeX(3.0f) - decodeX(2.8f), decodeY(2.8f) - decodeY(0.0f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodeRect3() As Rectangle2D
				rect.rectect(decodeX(0.0f), decodeY(0.0f), decodeX(2.8f) - decodeX(0.0f), decodeY(2.8f) - decodeY(0.0f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodePath1() As Path2D
			path.reset()
			path.moveTo(decodeX(0.0f), decodeY(0.0f))
			path.lineTo(decodeX(0.0f), decodeY(3.0f))
			path.lineTo(decodeX(3.0f), decodeY(3.0f))
			path.lineTo(decodeX(3.0f), decodeY(0.0f))
			path.lineTo(decodeX(0.24000001f), decodeY(0.0f))
			path.lineTo(decodeX(0.24000001f), decodeY(0.24000001f))
			path.lineTo(decodeX(2.7599998f), decodeY(0.24000001f))
			path.lineTo(decodeX(2.7599998f), decodeY(2.7599998f))
			path.lineTo(decodeX(0.24000001f), decodeY(2.7599998f))
			path.lineTo(decodeX(0.24000001f), decodeY(0.0f))
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
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.14441223f,0.43703705f,0.59444445f,0.75185186f,0.8759259f,1.0f }, New Color() { color2, decodeColor(color2,color3,0.5f), color3, decodeColor(color3,color4,0.5f), color4, decodeColor(color4,color5,0.5f), color5})
		End Function

		Private Function decodeGradient2(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.07147767f,0.2888889f,0.5490909f,0.7037037f,0.8518518f,1.0f }, New Color() { color6, decodeColor(color6,color7,0.5f), color7, decodeColor(color7,color8,0.5f), color8, decodeColor(color8,color9,0.5f), color9})
		End Function

		Private Function decodeGradient3(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.07147767f,0.2888889f,0.5490909f,0.7037037f,0.7919203f,0.88013697f }, New Color() { color11, decodeColor(color11,color12,0.5f), color12, decodeColor(color12,color13,0.5f), color13, decodeColor(color13,color14,0.5f), color14})
		End Function

		Private Function decodeGradient4(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.07147767f,0.2888889f,0.5490909f,0.7037037f,0.8518518f,1.0f }, New Color() { color15, decodeColor(color15,color16,0.5f), color16, decodeColor(color16,color17,0.5f), color17, decodeColor(color17,color18,0.5f), color18})
		End Function

		Private Function decodeGradient5(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.08049711f,0.32534248f,0.56267816f,0.7037037f,0.83986557f,0.97602737f }, New Color() { color19, decodeColor(color19,color20,0.5f), color20, decodeColor(color20,color21,0.5f), color21, decodeColor(color21,color22,0.5f), color22})
		End Function

		Private Function decodeGradient6(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.07147767f,0.2888889f,0.5490909f,0.7037037f,0.8518518f,1.0f }, New Color() { color19, decodeColor(color19,color23,0.5f), color23, decodeColor(color23,color21,0.5f), color21, decodeColor(color21,color24,0.5f), color24})
		End Function


	End Class

End Namespace
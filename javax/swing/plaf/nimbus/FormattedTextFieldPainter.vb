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



	Friend NotInheritable Class FormattedTextFieldPainter
		Inherits AbstractRegionPainter

		'package private integers representing the available states that
		'this painter will paint. These are used when creating a new instance
		'of FormattedTextFieldPainter to determine which region/state is being painted
		'by that instance.
		Friend Const BACKGROUND_DISABLED As Integer = 1
		Friend Const BACKGROUND_ENABLED As Integer = 2
		Friend Const BACKGROUND_SELECTED As Integer = 3
		Friend Const BORDER_DISABLED As Integer = 4
		Friend Const BORDER_FOCUSED As Integer = 5
		Friend Const BORDER_ENABLED As Integer = 6


		Private state As Integer 'refers to one of the static final ints above
		Private ctx As PaintContext

		'the following 4 variables are reused during the painting code of the layers
		Private path As Path2D = New Path2D.Float
		Private rect As Rectangle2D = New Rectangle2D.Float(0, 0, 0, 0)
		Private roundRect As RoundRectangle2D = New RoundRectangle2D.Float(0, 0, 0, 0, 0, 0)
		Private ellipse As Ellipse2D = New Ellipse2D.Float(0, 0, 0, 0)

		'All Colors used for painting are stored here. Ideally, only those colors being used
		'by a particular instance of FormattedTextFieldPainter would be created. For the moment at least,
		'however, all are created for each instance.
		Private color1 As Color = decodeColor("nimbusBlueGrey", -0.015872955f, -0.07995863f, 0.15294117f, 0)
		Private color2 As Color = decodeColor("nimbusLightBackground", 0.0f, 0.0f, 0.0f, 0)
		Private color3 As Color = decodeColor("nimbusBlueGrey", -0.006944418f, -0.07187897f, 0.06666666f, 0)
		Private color4 As Color = decodeColor("nimbusBlueGrey", 0.007936537f, -0.07826825f, 0.10588235f, 0)
		Private color5 As Color = decodeColor("nimbusBlueGrey", 0.007936537f, -0.07856284f, 0.11372548f, 0)
		Private color6 As Color = decodeColor("nimbusBlueGrey", 0.007936537f, -0.07796818f, 0.09803921f, 0)
		Private color7 As Color = decodeColor("nimbusBlueGrey", -0.027777791f, -0.0965403f, -0.18431371f, 0)
		Private color8 As Color = decodeColor("nimbusBlueGrey", 0.055555582f, -0.1048766f, -0.05098039f, 0)
		Private color9 As Color = decodeColor("nimbusLightBackground", 0.6666667f, 0.004901961f, -0.19999999f, 0)
		Private color10 As Color = decodeColor("nimbusBlueGrey", 0.055555582f, -0.10512091f, -0.019607842f, 0)
		Private color11 As Color = decodeColor("nimbusBlueGrey", 0.055555582f, -0.105344966f, 0.011764705f, 0)
		Private color12 As Color = decodeColor("nimbusFocus", 0.0f, 0.0f, 0.0f, 0)


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
				Case BACKGROUND_SELECTED
					paintBackgroundSelected(g)
				Case BORDER_DISABLED
					paintBorderDisabled(g)
				Case BORDER_FOCUSED
					paintBorderFocused(g)
				Case BORDER_ENABLED
					paintBorderEnabled(g)

			End Select
		End Sub

		Protected Friend Overrides Function getExtendedCacheKeys(ByVal c As JComponent) As Object()
			Dim ___extendedCacheKeys As Object() = Nothing
			Select Case state
				Case BACKGROUND_ENABLED
					___extendedCacheKeys = New Object() { getComponentColor(c, "background", color2, 0.0f, 0.0f, 0)}
				Case BORDER_FOCUSED
					___extendedCacheKeys = New Object() { getComponentColor(c, "background", color9, 0.004901961f, -0.19999999f, 0), getComponentColor(c, "background", color2, 0.0f, 0.0f, 0)}
				Case BORDER_ENABLED
					___extendedCacheKeys = New Object() { getComponentColor(c, "background", color9, 0.004901961f, -0.19999999f, 0), getComponentColor(c, "background", color2, 0.0f, 0.0f, 0)}
			End Select
			Return ___extendedCacheKeys
		End Function

		Protected Friend Property NotOverridable Overrides paintContext As PaintContext
			Get
				Return ctx
			End Get
		End Property

		Private Sub paintBackgroundDisabled(ByVal g As Graphics2D)
			rect = decodeRect1()
			g.paint = color1
			g.fill(rect)

		End Sub

		Private Sub paintBackgroundEnabled(ByVal g As Graphics2D)
			rect = decodeRect1()
			g.paint = CType(componentColors(0), Color)
			g.fill(rect)

		End Sub

		Private Sub paintBackgroundSelected(ByVal g As Graphics2D)
			rect = decodeRect1()
			g.paint = color2
			g.fill(rect)

		End Sub

		Private Sub paintBorderDisabled(ByVal g As Graphics2D)
			rect = decodeRect2()
			g.paint = decodeGradient1(rect)
			g.fill(rect)
			rect = decodeRect3()
			g.paint = decodeGradient2(rect)
			g.fill(rect)
			rect = decodeRect4()
			g.paint = color6
			g.fill(rect)
			rect = decodeRect5()
			g.paint = color4
			g.fill(rect)
			rect = decodeRect6()
			g.paint = color4
			g.fill(rect)

		End Sub

		Private Sub paintBorderFocused(ByVal g As Graphics2D)
			rect = decodeRect7()
			g.paint = decodeGradient3(rect)
			g.fill(rect)
			rect = decodeRect8()
			g.paint = decodeGradient4(rect)
			g.fill(rect)
			rect = decodeRect9()
			g.paint = color10
			g.fill(rect)
			rect = decodeRect10()
			g.paint = color10
			g.fill(rect)
			rect = decodeRect11()
			g.paint = color11
			g.fill(rect)
			path = decodePath1()
			g.paint = color12
			g.fill(path)

		End Sub

		Private Sub paintBorderEnabled(ByVal g As Graphics2D)
			rect = decodeRect7()
			g.paint = decodeGradient5(rect)
			g.fill(rect)
			rect = decodeRect8()
			g.paint = decodeGradient4(rect)
			g.fill(rect)
			rect = decodeRect9()
			g.paint = color10
			g.fill(rect)
			rect = decodeRect10()
			g.paint = color10
			g.fill(rect)
			rect = decodeRect11()
			g.paint = color11
			g.fill(rect)

		End Sub



		Private Function decodeRect1() As Rectangle2D
				rect.rectect(decodeX(0.4f), decodeY(0.4f), decodeX(2.6f) - decodeX(0.4f), decodeY(2.6f) - decodeY(0.4f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodeRect2() As Rectangle2D
				rect.rectect(decodeX(0.6666667f), decodeY(0.4f), decodeX(2.3333333f) - decodeX(0.6666667f), decodeY(1.0f) - decodeY(0.4f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodeRect3() As Rectangle2D
				rect.rectect(decodeX(1.0f), decodeY(0.6f), decodeX(2.0f) - decodeX(1.0f), decodeY(1.0f) - decodeY(0.6f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodeRect4() As Rectangle2D
				rect.rectect(decodeX(0.6666667f), decodeY(1.0f), decodeX(1.0f) - decodeX(0.6666667f), decodeY(2.0f) - decodeY(1.0f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodeRect5() As Rectangle2D
				rect.rectect(decodeX(0.6666667f), decodeY(2.3333333f), decodeX(2.3333333f) - decodeX(0.6666667f), decodeY(2.0f) - decodeY(2.3333333f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodeRect6() As Rectangle2D
				rect.rectect(decodeX(2.0f), decodeY(1.0f), decodeX(2.3333333f) - decodeX(2.0f), decodeY(2.0f) - decodeY(1.0f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodeRect7() As Rectangle2D
				rect.rectect(decodeX(0.4f), decodeY(0.4f), decodeX(2.6f) - decodeX(0.4f), decodeY(1.0f) - decodeY(0.4f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodeRect8() As Rectangle2D
				rect.rectect(decodeX(0.6f), decodeY(0.6f), decodeX(2.4f) - decodeX(0.6f), decodeY(1.0f) - decodeY(0.6f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodeRect9() As Rectangle2D
				rect.rectect(decodeX(0.4f), decodeY(1.0f), decodeX(0.6f) - decodeX(0.4f), decodeY(2.6f) - decodeY(1.0f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodeRect10() As Rectangle2D
				rect.rectect(decodeX(2.4f), decodeY(1.0f), decodeX(2.6f) - decodeX(2.4f), decodeY(2.6f) - decodeY(1.0f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodeRect11() As Rectangle2D
				rect.rectect(decodeX(0.6f), decodeY(2.4f), decodeX(2.4f) - decodeX(0.6f), decodeY(2.6f) - decodeY(2.4f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodePath1() As Path2D
			path.reset()
			path.moveTo(decodeX(0.4f), decodeY(0.4f))
			path.lineTo(decodeX(0.4f), decodeY(2.6f))
			path.lineTo(decodeX(2.6f), decodeY(2.6f))
			path.lineTo(decodeX(2.6f), decodeY(0.4f))
			path.curveTo(decodeAnchorX(2.5999999046325684f, 0.0f), decodeAnchorY(0.4000000059604645f, 0.0f), decodeAnchorX(2.880000352859497f, 0.09999999999999432f), decodeAnchorY(0.4000000059604645f, 0.0f), decodeX(2.8800004f), decodeY(0.4f))
			path.curveTo(decodeAnchorX(2.880000352859497f, 0.09999999999999432f), decodeAnchorY(0.4000000059604645f, 0.0f), decodeAnchorX(2.880000352859497f, 0.0f), decodeAnchorY(2.879999876022339f, 0.0f), decodeX(2.8800004f), decodeY(2.8799999f))
			path.lineTo(decodeX(0.120000005f), decodeY(2.8799999f))
			path.lineTo(decodeX(0.120000005f), decodeY(0.120000005f))
			path.lineTo(decodeX(2.8800004f), decodeY(0.120000005f))
			path.lineTo(decodeX(2.8800004f), decodeY(0.4f))
			path.lineTo(decodeX(0.4f), decodeY(0.4f))
			path.closePath()
			Return path
		End Function



		Private Function decodeGradient1(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color3, decodeColor(color3,color4,0.5f), color4})
		End Function

		Private Function decodeGradient2(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color5, decodeColor(color5,color1,0.5f), color1})
		End Function

		Private Function decodeGradient3(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.25f * w) + x, (0.0f * h) + y, (0.25f * w) + x, (0.1625f * h) + y, New Single() { 0.1f,0.49999997f,0.9f }, New Color() { color7, decodeColor(color7,color8,0.5f), color8})
		End Function

		Private Function decodeGradient4(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.1f,0.49999997f,0.9f }, New Color() { CType(componentColors(0), Color), decodeColor(CType(componentColors(0), Color),CType(componentColors(1), Color),0.5f), CType(componentColors(1), Color)})
		End Function

		Private Function decodeGradient5(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.1f,0.49999997f,0.9f }, New Color() { color7, decodeColor(color7,color8,0.5f), color8})
		End Function


	End Class

End Namespace
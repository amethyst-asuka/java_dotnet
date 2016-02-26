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



	Friend NotInheritable Class MenuBarPainter
		Inherits AbstractRegionPainter

		'package private integers representing the available states that
		'this painter will paint. These are used when creating a new instance
		'of MenuBarPainter to determine which region/state is being painted
		'by that instance.
		Friend Const BACKGROUND_ENABLED As Integer = 1
		Friend Const BORDER_ENABLED As Integer = 2


		Private state As Integer 'refers to one of the static final ints above
		Private ctx As PaintContext

		'the following 4 variables are reused during the painting code of the layers
		Private path As Path2D = New Path2D.Float
		Private rect As Rectangle2D = New Rectangle2D.Float(0, 0, 0, 0)
		Private roundRect As RoundRectangle2D = New RoundRectangle2D.Float(0, 0, 0, 0, 0, 0)
		Private ellipse As Ellipse2D = New Ellipse2D.Float(0, 0, 0, 0)

		'All Colors used for painting are stored here. Ideally, only those colors being used
		'by a particular instance of MenuBarPainter would be created. For the moment at least,
		'however, all are created for each instance.
		Private color1 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.07016757f, 0.12941176f, 0)
		Private color2 As Color = decodeColor("nimbusBlueGrey", -0.027777791f, -0.10255819f, 0.23921567f, 0)
		Private color3 As Color = decodeColor("nimbusBlueGrey", -0.111111104f, -0.10654225f, 0.23921567f, -29)
		Private color4 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.110526316f, 0.25490195f, -255)
		Private color5 As Color = decodeColor("nimbusBorder", 0.0f, 0.0f, 0.0f, 0)


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
				Case BORDER_ENABLED
					paintBorderEnabled(g)

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
			rect = decodeRect2()
			g.paint = decodeGradient1(rect)
			g.fill(rect)

		End Sub

		Private Sub paintBorderEnabled(ByVal g As Graphics2D)
			rect = decodeRect3()
			g.paint = color5
			g.fill(rect)

		End Sub



		Private Function decodeRect1() As Rectangle2D
				rect.rectect(decodeX(1.0f), decodeY(0.0f), decodeX(2.0f) - decodeX(1.0f), decodeY(1.9523809f) - decodeY(0.0f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodeRect2() As Rectangle2D
				rect.rectect(decodeX(1.0f), decodeY(0.0f), decodeX(2.0f) - decodeX(1.0f), decodeY(2.0f) - decodeY(0.0f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodeRect3() As Rectangle2D
				rect.rectect(decodeX(1.0f), decodeY(2.0f), decodeX(2.0f) - decodeX(1.0f), decodeY(3.0f) - decodeY(2.0f)) 'height - width - y - x
			Return rect
		End Function



		Private Function decodeGradient1(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((1.0f * w) + x, (0.0f * h) + y, (1.0f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.015f,0.03f,0.23354445f,0.7569444f }, New Color() { color2, decodeColor(color2,color3,0.5f), color3, decodeColor(color3,color4,0.5f), color4})
		End Function


	End Class

End Namespace
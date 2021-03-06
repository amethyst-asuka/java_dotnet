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



	Friend NotInheritable Class ScrollPanePainter
		Inherits AbstractRegionPainter

		'package private integers representing the available states that
		'this painter will paint. These are used when creating a new instance
		'of ScrollPanePainter to determine which region/state is being painted
		'by that instance.
		Friend Const BACKGROUND_ENABLED As Integer = 1
		Friend Const BORDER_ENABLED_FOCUSED As Integer = 2
		Friend Const BORDER_ENABLED As Integer = 3


		Private state As Integer 'refers to one of the static final ints above
		Private ctx As PaintContext

		'the following 4 variables are reused during the painting code of the layers
		Private path As Path2D = New Path2D.Float
		Private rect As Rectangle2D = New Rectangle2D.Float(0, 0, 0, 0)
		Private roundRect As RoundRectangle2D = New RoundRectangle2D.Float(0, 0, 0, 0, 0, 0)
		Private ellipse As Ellipse2D = New Ellipse2D.Float(0, 0, 0, 0)

		'All Colors used for painting are stored here. Ideally, only those colors being used
		'by a particular instance of ScrollPanePainter would be created. For the moment at least,
		'however, all are created for each instance.
		Private color1 As Color = decodeColor("nimbusBorder", 0.0f, 0.0f, 0.0f, 0)
		Private color2 As Color = decodeColor("nimbusFocus", 0.0f, 0.0f, 0.0f, 0)


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
				Case BORDER_ENABLED_FOCUSED
					paintBorderEnabledAndFocused(g)
				Case BORDER_ENABLED
					paintBorderEnabled(g)

			End Select
		End Sub



		Protected Friend Property NotOverridable Overrides paintContext As PaintContext
			Get
				Return ctx
			End Get
		End Property

		Private Sub paintBorderEnabledAndFocused(ByVal g As Graphics2D)
			rect = decodeRect1()
			g.paint = color1
			g.fill(rect)
			rect = decodeRect2()
			g.paint = color1
			g.fill(rect)
			rect = decodeRect3()
			g.paint = color1
			g.fill(rect)
			rect = decodeRect4()
			g.paint = color1
			g.fill(rect)
			path = decodePath1()
			g.paint = color2
			g.fill(path)

		End Sub

		Private Sub paintBorderEnabled(ByVal g As Graphics2D)
			rect = decodeRect1()
			g.paint = color1
			g.fill(rect)
			rect = decodeRect2()
			g.paint = color1
			g.fill(rect)
			rect = decodeRect3()
			g.paint = color1
			g.fill(rect)
			rect = decodeRect4()
			g.paint = color1
			g.fill(rect)

		End Sub



		Private Function decodeRect1() As Rectangle2D
				rect.rectect(decodeX(0.6f), decodeY(0.4f), decodeX(2.4f) - decodeX(0.6f), decodeY(0.6f) - decodeY(0.4f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodeRect2() As Rectangle2D
				rect.rectect(decodeX(0.4f), decodeY(0.4f), decodeX(0.6f) - decodeX(0.4f), decodeY(2.6f) - decodeY(0.4f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodeRect3() As Rectangle2D
				rect.rectect(decodeX(2.4f), decodeY(0.4f), decodeX(2.6f) - decodeX(2.4f), decodeY(2.6f) - decodeY(0.4f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodeRect4() As Rectangle2D
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




	End Class

End Namespace
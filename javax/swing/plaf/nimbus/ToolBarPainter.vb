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



	Friend NotInheritable Class ToolBarPainter
		Inherits AbstractRegionPainter

		'package private integers representing the available states that
		'this painter will paint. These are used when creating a new instance
		'of ToolBarPainter to determine which region/state is being painted
		'by that instance.
		Friend Const BORDER_NORTH As Integer = 1
		Friend Const BORDER_SOUTH As Integer = 2
		Friend Const BORDER_EAST As Integer = 3
		Friend Const BORDER_WEST As Integer = 4
		Friend Const HANDLEICON_ENABLED As Integer = 5


		Private ___state As Integer 'refers to one of the static final ints above
		Private ctx As PaintContext

		'the following 4 variables are reused during the painting code of the layers
		Private path As Path2D = New Path2D.Float
		Private rect As Rectangle2D = New Rectangle2D.Float(0, 0, 0, 0)
		Private roundRect As RoundRectangle2D = New RoundRectangle2D.Float(0, 0, 0, 0, 0, 0)
		Private ellipse As Ellipse2D = New Ellipse2D.Float(0, 0, 0, 0)

		'All Colors used for painting are stored here. Ideally, only those colors being used
		'by a particular instance of ToolBarPainter would be created. For the moment at least,
		'however, all are created for each instance.
		Private color1 As Color = decodeColor("nimbusBorder", 0.0f, 0.0f, 0.0f, 0)
		Private color2 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.110526316f, 0.25490195f, 0)
		Private color3 As Color = decodeColor("nimbusBlueGrey", -0.006944418f, -0.07399663f, 0.11372548f, 0)
		Private color4 As Color = decodeColor("nimbusBorder", 0.0f, -0.029675633f, 0.109803915f, 0)
		Private color5 As Color = decodeColor("nimbusBlueGrey", -0.008547008f, -0.03494492f, -0.07058823f, 0)


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
				Case BORDER_NORTH
					paintBorderNorth(g)
				Case BORDER_SOUTH
					paintBorderSouth(g)
				Case BORDER_EAST
					paintBorderEast(g)
				Case BORDER_WEST
					paintBorderWest(g)
				Case HANDLEICON_ENABLED
					painthandleIconEnabled(g)

			End Select
		End Sub



		Protected Friend Property NotOverridable Overrides paintContext As PaintContext
			Get
				Return ctx
			End Get
		End Property

		Private Sub paintBorderNorth(ByVal g As Graphics2D)
			rect = decodeRect1()
			g.paint = color1
			g.fill(rect)

		End Sub

		Private Sub paintBorderSouth(ByVal g As Graphics2D)
			rect = decodeRect2()
			g.paint = color1
			g.fill(rect)

		End Sub

		Private Sub paintBorderEast(ByVal g As Graphics2D)
			rect = decodeRect2()
			g.paint = color1
			g.fill(rect)

		End Sub

		Private Sub paintBorderWest(ByVal g As Graphics2D)
			rect = decodeRect1()
			g.paint = color1
			g.fill(rect)

		End Sub

		Private Sub painthandleIconEnabled(ByVal g As Graphics2D)
			rect = decodeRect3()
			g.paint = decodeGradient1(rect)
			g.fill(rect)
			rect = decodeRect4()
			g.paint = color4
			g.fill(rect)
			path = decodePath1()
			g.paint = color5
			g.fill(path)
			path = decodePath2()
			g.paint = color5
			g.fill(path)

		End Sub



		Private Function decodeRect1() As Rectangle2D
				rect.rectect(decodeX(1.0f), decodeY(2.0f), decodeX(2.0f) - decodeX(1.0f), decodeY(3.0f) - decodeY(2.0f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodeRect2() As Rectangle2D
				rect.rectect(decodeX(1.0f), decodeY(0.0f), decodeX(2.0f) - decodeX(1.0f), decodeY(1.0f) - decodeY(0.0f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodeRect3() As Rectangle2D
				rect.rectect(decodeX(0.0f), decodeY(0.0f), decodeX(2.8f) - decodeX(0.0f), decodeY(3.0f) - decodeY(0.0f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodeRect4() As Rectangle2D
				rect.rectect(decodeX(2.8f), decodeY(0.0f), decodeX(3.0f) - decodeX(2.8f), decodeY(3.0f) - decodeY(0.0f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodePath1() As Path2D
			path.reset()
			path.moveTo(decodeX(0.0f), decodeY(0.0f))
			path.lineTo(decodeX(0.0f), decodeY(0.4f))
			path.lineTo(decodeX(0.4f), decodeY(0.0f))
			path.lineTo(decodeX(0.0f), decodeY(0.0f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath2() As Path2D
			path.reset()
			path.moveTo(decodeX(0.0f), decodeY(3.0f))
			path.lineTo(decodeX(0.0f), decodeY(2.6f))
			path.lineTo(decodeX(0.4f), decodeY(3.0f))
			path.lineTo(decodeX(0.0f), decodeY(3.0f))
			path.closePath()
			Return path
		End Function



		Private Function decodeGradient1(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.0f * w) + x, (0.5f * h) + y, (1.0f * w) + x, (0.5f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color2, decodeColor(color2,color3,0.5f), color3})
		End Function


	End Class

End Namespace
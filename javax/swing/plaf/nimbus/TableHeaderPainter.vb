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



	Friend NotInheritable Class TableHeaderPainter
		Inherits AbstractRegionPainter

		'package private integers representing the available states that
		'this painter will paint. These are used when creating a new instance
		'of TableHeaderPainter to determine which region/state is being painted
		'by that instance.
		Friend Const ASCENDINGSORTICON_ENABLED As Integer = 1
		Friend Const DESCENDINGSORTICON_ENABLED As Integer = 2


		Private ___state As Integer 'refers to one of the static final ints above
		Private ctx As PaintContext

		'the following 4 variables are reused during the painting code of the layers
		Private path As Path2D = New Path2D.Float
		Private rect As Rectangle2D = New Rectangle2D.Float(0, 0, 0, 0)
		Private roundRect As RoundRectangle2D = New RoundRectangle2D.Float(0, 0, 0, 0, 0, 0)
		Private ellipse As Ellipse2D = New Ellipse2D.Float(0, 0, 0, 0)

		'All Colors used for painting are stored here. Ideally, only those colors being used
		'by a particular instance of TableHeaderPainter would be created. For the moment at least,
		'however, all are created for each instance.
		Private color1 As Color = decodeColor("nimbusBase", 0.0057927966f, -0.21904764f, 0.15686274f, 0)
		Private color2 As Color = decodeColor("nimbusBase", 0.0038565993f, 0.02012986f, 0.054901958f, 0)


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
				Case ASCENDINGSORTICON_ENABLED
					paintascendingSortIconEnabled(g)
				Case DESCENDINGSORTICON_ENABLED
					paintdescendingSortIconEnabled(g)

			End Select
		End Sub



		Protected Friend Property NotOverridable Overrides paintContext As PaintContext
			Get
				Return ctx
			End Get
		End Property

		Private Sub paintascendingSortIconEnabled(ByVal g As Graphics2D)
			path = decodePath1()
			g.paint = decodeGradient1(path)
			g.fill(path)

		End Sub

		Private Sub paintdescendingSortIconEnabled(ByVal g As Graphics2D)
			path = decodePath2()
			g.paint = decodeGradient1(path)
			g.fill(path)

		End Sub



		Private Function decodePath1() As Path2D
			path.reset()
			path.moveTo(decodeX(1.0f), decodeY(2.0f))
			path.lineTo(decodeX(1.7070175f), decodeY(0.0f))
			path.lineTo(decodeX(3.0f), decodeY(2.0f))
			path.lineTo(decodeX(1.0f), decodeY(2.0f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath2() As Path2D
			path.reset()
			path.moveTo(decodeX(1.0f), decodeY(1.0f))
			path.lineTo(decodeX(2.0f), decodeY(1.0f))
			path.lineTo(decodeX(1.5025063f), decodeY(2.0f))
			path.lineTo(decodeX(1.0f), decodeY(1.0f))
			path.closePath()
			Return path
		End Function



		Private Function decodeGradient1(ByVal s As Shape) As Paint
			Dim bounds As Rectangle2D = s.bounds2D
			Dim x As Single = CSng(bounds.x)
			Dim y As Single = CSng(bounds.y)
			Dim w As Single = CSng(bounds.width)
			Dim h As Single = CSng(bounds.height)
			Return decodeGradient((0.5f * w) + x, (0.0f * h) + y, (0.5f * w) + x, (1.0f * h) + y, New Single() { 0.0f,0.5f,1.0f }, New Color() { color1, decodeColor(color1,color2,0.5f), color2})
		End Function


	End Class

End Namespace
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



	Friend NotInheritable Class TreeCellEditorPainter
		Inherits AbstractRegionPainter

		'package private integers representing the available states that
		'this painter will paint. These are used when creating a new instance
		'of TreeCellEditorPainter to determine which region/state is being painted
		'by that instance.
		Friend Const BACKGROUND_DISABLED As Integer = 1
		Friend Const BACKGROUND_ENABLED As Integer = 2
		Friend Const BACKGROUND_ENABLED_FOCUSED As Integer = 3
		Friend Const BACKGROUND_SELECTED As Integer = 4


		Private ___state As Integer 'refers to one of the static final ints above
		Private ctx As PaintContext

		'the following 4 variables are reused during the painting code of the layers
		Private path As Path2D = New Path2D.Float
		Private rect As Rectangle2D = New Rectangle2D.Float(0, 0, 0, 0)
		Private roundRect As RoundRectangle2D = New RoundRectangle2D.Float(0, 0, 0, 0, 0, 0)
		Private ellipse As Ellipse2D = New Ellipse2D.Float(0, 0, 0, 0)

		'All Colors used for painting are stored here. Ideally, only those colors being used
		'by a particular instance of TreeCellEditorPainter would be created. For the moment at least,
		'however, all are created for each instance.
		Private color1 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.017358616f, -0.11372548f, 0)
		Private color2 As Color = decodeColor("nimbusFocus", 0.0f, 0.0f, 0.0f, 0)


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
				Case BACKGROUND_ENABLED
					paintBackgroundEnabled(g)
				Case BACKGROUND_ENABLED_FOCUSED
					paintBackgroundEnabledAndFocused(g)

			End Select
		End Sub



		Protected Friend Property NotOverridable Overrides paintContext As PaintContext
			Get
				Return ctx
			End Get
		End Property

		Private Sub paintBackgroundEnabled(ByVal g As Graphics2D)
			path = decodePath1()
			g.paint = color1
			g.fill(path)

		End Sub

		Private Sub paintBackgroundEnabledAndFocused(ByVal g As Graphics2D)
			path = decodePath2()
			g.paint = color2
			g.fill(path)

		End Sub



		Private Function decodePath1() As Path2D
			path.reset()
			path.moveTo(decodeX(0.0f), decodeY(0.0f))
			path.lineTo(decodeX(0.0f), decodeY(3.0f))
			path.lineTo(decodeX(3.0f), decodeY(3.0f))
			path.lineTo(decodeX(3.0f), decodeY(0.0f))
			path.lineTo(decodeX(0.2f), decodeY(0.0f))
			path.lineTo(decodeX(0.2f), decodeY(0.2f))
			path.lineTo(decodeX(2.8f), decodeY(0.2f))
			path.lineTo(decodeX(2.8f), decodeY(2.8f))
			path.lineTo(decodeX(0.2f), decodeY(2.8f))
			path.lineTo(decodeX(0.2f), decodeY(0.0f))
			path.lineTo(decodeX(0.0f), decodeY(0.0f))
			path.closePath()
			Return path
		End Function

		Private Function decodePath2() As Path2D
			path.reset()
			path.moveTo(decodeX(0.0f), decodeY(0.0f))
			path.lineTo(decodeX(0.0f), decodeY(3.0f))
			path.lineTo(decodeX(3.0f), decodeY(3.0f))
			path.lineTo(decodeX(3.0f), decodeY(0.0f))
			path.lineTo(decodeX(0.24000001f), decodeY(0.0f))
			path.lineTo(decodeX(0.24000001f), decodeY(0.24000001f))
			path.lineTo(decodeX(2.7600007f), decodeY(0.24000001f))
			path.lineTo(decodeX(2.7600007f), decodeY(2.7599998f))
			path.lineTo(decodeX(0.24000001f), decodeY(2.7599998f))
			path.lineTo(decodeX(0.24000001f), decodeY(0.0f))
			path.lineTo(decodeX(0.0f), decodeY(0.0f))
			path.closePath()
			Return path
		End Function




	End Class

End Namespace
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



	Friend NotInheritable Class CheckBoxMenuItemPainter
		Inherits AbstractRegionPainter

		'package private integers representing the available states that
		'this painter will paint. These are used when creating a new instance
		'of CheckBoxMenuItemPainter to determine which region/state is being painted
		'by that instance.
		Friend Const BACKGROUND_DISABLED As Integer = 1
		Friend Const BACKGROUND_ENABLED As Integer = 2
		Friend Const BACKGROUND_MOUSEOVER As Integer = 3
		Friend Const BACKGROUND_SELECTED_MOUSEOVER As Integer = 4
		Friend Const CHECKICON_DISABLED_SELECTED As Integer = 5
		Friend Const CHECKICON_ENABLED_SELECTED As Integer = 6
		Friend Const CHECKICON_SELECTED_MOUSEOVER As Integer = 7


		Private state As Integer 'refers to one of the static final ints above
		Private ctx As PaintContext

		'the following 4 variables are reused during the painting code of the layers
		Private path As Path2D = New Path2D.Float
		Private rect As Rectangle2D = New Rectangle2D.Float(0, 0, 0, 0)
		Private roundRect As RoundRectangle2D = New RoundRectangle2D.Float(0, 0, 0, 0, 0, 0)
		Private ellipse As Ellipse2D = New Ellipse2D.Float(0, 0, 0, 0)

		'All Colors used for painting are stored here. Ideally, only those colors being used
		'by a particular instance of CheckBoxMenuItemPainter would be created. For the moment at least,
		'however, all are created for each instance.
		Private color1 As Color = decodeColor("nimbusSelection", 0.0f, 0.0f, 0.0f, 0)
		Private color2 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.08983666f, -0.17647058f, 0)
		Private color3 As Color = decodeColor("nimbusBlueGrey", 0.055555582f, -0.096827686f, -0.45882353f, 0)
		Private color4 As Color = decodeColor("nimbusBlueGrey", 0.0f, -0.110526316f, 0.25490195f, 0)


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
				Case BACKGROUND_MOUSEOVER
					paintBackgroundMouseOver(g)
				Case BACKGROUND_SELECTED_MOUSEOVER
					paintBackgroundSelectedAndMouseOver(g)
				Case CHECKICON_DISABLED_SELECTED
					paintcheckIconDisabledAndSelected(g)
				Case CHECKICON_ENABLED_SELECTED
					paintcheckIconEnabledAndSelected(g)
				Case CHECKICON_SELECTED_MOUSEOVER
					paintcheckIconSelectedAndMouseOver(g)

			End Select
		End Sub



		Protected Friend Property NotOverridable Overrides paintContext As PaintContext
			Get
				Return ctx
			End Get
		End Property

		Private Sub paintBackgroundMouseOver(ByVal g As Graphics2D)
			rect = decodeRect1()
			g.paint = color1
			g.fill(rect)

		End Sub

		Private Sub paintBackgroundSelectedAndMouseOver(ByVal g As Graphics2D)
			rect = decodeRect1()
			g.paint = color1
			g.fill(rect)

		End Sub

		Private Sub paintcheckIconDisabledAndSelected(ByVal g As Graphics2D)
			path = decodePath1()
			g.paint = color2
			g.fill(path)

		End Sub

		Private Sub paintcheckIconEnabledAndSelected(ByVal g As Graphics2D)
			path = decodePath1()
			g.paint = color3
			g.fill(path)

		End Sub

		Private Sub paintcheckIconSelectedAndMouseOver(ByVal g As Graphics2D)
			path = decodePath1()
			g.paint = color4
			g.fill(path)

		End Sub



		Private Function decodeRect1() As Rectangle2D
				rect.rectect(decodeX(1.0f), decodeY(1.0f), decodeX(2.0f) - decodeX(1.0f), decodeY(2.0f) - decodeY(1.0f)) 'height - width - y - x
			Return rect
		End Function

		Private Function decodePath1() As Path2D
			path.reset()
			path.moveTo(decodeX(0.0f), decodeY(1.5f))
			path.lineTo(decodeX(0.4292683f), decodeY(1.5f))
			path.lineTo(decodeX(0.7121951f), decodeY(2.4780488f))
			path.lineTo(decodeX(2.5926828f), decodeY(0.0f))
			path.lineTo(decodeX(3.0f), decodeY(0.0f))
			path.lineTo(decodeX(3.0f), decodeY(0.2f))
			path.lineTo(decodeX(2.8317075f), decodeY(0.39512196f))
			path.lineTo(decodeX(0.8f), decodeY(3.0f))
			path.lineTo(decodeX(0.5731707f), decodeY(3.0f))
			path.lineTo(decodeX(0.0f), decodeY(1.5f))
			path.closePath()
			Return path
		End Function




	End Class

End Namespace
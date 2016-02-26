Imports System
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



	''' <summary>
	''' An icon that delegates to a painter.
	''' @author rbair
	''' </summary>
	Friend Class NimbusIcon
		Inherits sun.swing.plaf.synth.SynthIcon

		Private width As Integer
		Private height As Integer
		Private prefix As String
		Private key As String

		Friend Sub New(ByVal prefix As String, ByVal key As String, ByVal w As Integer, ByVal h As Integer)
			Me.width = w
			Me.height = h
			Me.prefix = prefix
			Me.key = key
		End Sub

		Public Overrides Sub paintIcon(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			Dim painter As javax.swing.Painter = Nothing
			If context IsNot Nothing Then painter = CType(context.style.get(context, key), javax.swing.Painter)
			If painter Is Nothing Then painter = CType(UIManager.get(prefix & "[Enabled]." & key), javax.swing.Painter)

			If painter IsNot Nothing AndAlso context IsNot Nothing Then
				Dim c As JComponent = context.component
				Dim rotate As Boolean = False
				Dim flip As Boolean = False
				'translatex and translatey are additional translations that
				'must occur on the graphics context when rendering a toolbar
				'icon
				Dim translatex As Integer = 0
				Dim translatey As Integer = 0
				If TypeOf c Is JToolBar Then
					Dim toolbar As JToolBar = CType(c, JToolBar)
					rotate = toolbar.orientation = JToolBar.VERTICAL
					flip = Not toolbar.componentOrientation.leftToRight
					Dim o As Object = NimbusLookAndFeel.resolveToolbarConstraint(toolbar)
					'we only do the +1 hack for UIResource borders, assuming
					'that the border is probably going to be our border
					If TypeOf toolbar.border Is javax.swing.plaf.UIResource Then
						If o Is BorderLayout.SOUTH Then
							translatey = 1
						ElseIf o Is BorderLayout.EAST Then
							translatex = 1
						End If
					End If
				ElseIf TypeOf c Is JMenu Then
					flip = Not c.componentOrientation.leftToRight
				End If
				If TypeOf g Is Graphics2D Then
					Dim gfx As Graphics2D = CType(g, Graphics2D)
					gfx.translate(x, y)
					gfx.translate(translatex, translatey)
					If rotate Then
						gfx.rotate(Math.toRadians(90))
						gfx.translate(0, -w)
						painter.paint(gfx, context.component, h, w)
						gfx.translate(0, w)
						gfx.rotate(Math.toRadians(-90))
					ElseIf flip Then
						gfx.scale(-1, 1)
						gfx.translate(-w,0)
						painter.paint(gfx, context.component, w, h)
						gfx.translate(w,0)
						gfx.scale(-1, 1)
					Else
						painter.paint(gfx, context.component, w, h)
					End If
					gfx.translate(-translatex, -translatey)
					gfx.translate(-x, -y)
				Else
					' use image if we are printing to a Java 1.1 PrintGraphics as
					' it is not a instance of Graphics2D
					Dim img As New java.awt.image.BufferedImage(w,h, java.awt.image.BufferedImage.TYPE_INT_ARGB)
					Dim gfx As Graphics2D = img.createGraphics()
					If rotate Then
						gfx.rotate(Math.toRadians(90))
						gfx.translate(0, -w)
						painter.paint(gfx, context.component, h, w)
					ElseIf flip Then
						gfx.scale(-1, 1)
						gfx.translate(-w,0)
						painter.paint(gfx, context.component, w, h)
					Else
						painter.paint(gfx, context.component, w, h)
					End If
					gfx.Dispose()
					g.drawImage(img,x,y,Nothing)
					img = Nothing
				End If
			End If
		End Sub

		''' <summary>
		''' Implements the standard Icon interface's paintIcon method as the standard
		''' synth stub passes null for the context and this will cause us to not
		''' paint any thing, so we override here so that we can paint the enabled
		''' state if no synth context is available
		''' </summary>
		Public Overrides Sub paintIcon(ByVal c As Component, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer)
			Dim painter As javax.swing.Painter = CType(UIManager.get(prefix & "[Enabled]." & key), javax.swing.Painter)
			If painter IsNot Nothing Then
				Dim jc As JComponent = If(TypeOf c Is JComponent, CType(c, JComponent), Nothing)
				Dim gfx As Graphics2D = CType(g, Graphics2D)
				gfx.translate(x, y)
				painter.paint(gfx, jc, width, height)
				gfx.translate(-x, -y)
			End If
		End Sub

		Public Overrides Function getIconWidth(ByVal context As javax.swing.plaf.synth.SynthContext) As Integer
			If context Is Nothing Then Return width
			Dim c As JComponent = context.component
			If TypeOf c Is JToolBar AndAlso CType(c, JToolBar).orientation = JToolBar.VERTICAL Then
				'we only do the -1 hack for UIResource borders, assuming
				'that the border is probably going to be our border
				If TypeOf c.border Is javax.swing.plaf.UIResource Then
					Return c.width - 1
				Else
					Return c.width
				End If
			Else
				Return scale(context, width)
			End If
		End Function

		Public Overrides Function getIconHeight(ByVal context As javax.swing.plaf.synth.SynthContext) As Integer
			If context Is Nothing Then Return height
			Dim c As JComponent = context.component
			If TypeOf c Is JToolBar Then
				Dim toolbar As JToolBar = CType(c, JToolBar)
				If toolbar.orientation = JToolBar.HORIZONTAL Then
					'we only do the -1 hack for UIResource borders, assuming
					'that the border is probably going to be our border
					If TypeOf toolbar.border Is javax.swing.plaf.UIResource Then
						Return c.height - 1
					Else
						Return c.height
					End If
				Else
					Return scale(context, width)
				End If
			Else
				Return scale(context, height)
			End If
		End Function

		''' <summary>
		''' Scale a size based on the "JComponent.sizeVariant" client property of the
		''' component that is using this icon
		''' </summary>
		''' <param name="context"> The synthContext to get the component from </param>
		''' <param name="size"> The size to scale </param>
		''' <returns> The scaled size or original if "JComponent.sizeVariant" is not
		'''          set </returns>
		Private Function scale(ByVal context As javax.swing.plaf.synth.SynthContext, ByVal size As Integer) As Integer
			If context Is Nothing OrElse context.component Is Nothing Then Return size
			' The key "JComponent.sizeVariant" is used to match Apple's LAF
			Dim scaleKey As String = CStr(context.component.getClientProperty("JComponent.sizeVariant"))
			If scaleKey IsNot Nothing Then
				If NimbusStyle.LARGE_KEY.Equals(scaleKey) Then
					size *= NimbusStyle.LARGE_SCALE
				ElseIf NimbusStyle.SMALL_KEY.Equals(scaleKey) Then
					size *= NimbusStyle.SMALL_SCALE
				ElseIf NimbusStyle.MINI_KEY.Equals(scaleKey) Then
					' mini is not quite as small for icons as full mini is
					' just too tiny
					size *= NimbusStyle.MINI_SCALE + 0.07
				End If
			End If
			Return size
		End Function
	End Class

End Namespace
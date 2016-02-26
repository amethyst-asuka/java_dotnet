Imports javax.swing
Imports javax.swing.event
Imports javax.swing.border
Imports javax.swing.plaf

'
' * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.swing.plaf.basic


	''' <summary>
	''' Basic L&amp;F for a minimized window on a desktop.
	''' 
	''' @author David Kloba
	''' @author Steve Wilson
	''' @author Rich Schiavi
	''' </summary>
	Public Class BasicDesktopIconUI
		Inherits DesktopIconUI

		Protected Friend desktopIcon As JInternalFrame.JDesktopIcon
		Protected Friend frame As JInternalFrame

		''' <summary>
		''' The title pane component used in the desktop icon.
		''' 
		''' @since 1.5
		''' </summary>
		Protected Friend iconPane As JComponent
		Friend mouseInputListener As MouseInputListener



		Public Shared Function createUI(ByVal c As JComponent) As ComponentUI
			Return New BasicDesktopIconUI
		End Function

		Public Sub New()
		End Sub

		Public Overridable Sub installUI(ByVal c As JComponent)
			desktopIcon = CType(c, JInternalFrame.JDesktopIcon)
			frame = desktopIcon.internalFrame
			installDefaults()
			installComponents()

			' Update icon layout if frame is already iconified
			Dim f As JInternalFrame = desktopIcon.internalFrame
			If f.icon AndAlso f.parent Is Nothing Then
				Dim desktop As JDesktopPane = desktopIcon.desktopPane
				If desktop IsNot Nothing Then
					Dim desktopManager As DesktopManager = desktop.desktopManager
					If TypeOf desktopManager Is DefaultDesktopManager Then desktopManager.iconifyFrame(f)
				End If
			End If

			installListeners()
			JLayeredPane.putLayer(desktopIcon, JLayeredPane.getLayer(frame))
		End Sub

		Public Overridable Sub uninstallUI(ByVal c As JComponent)
			uninstallDefaults()
			uninstallComponents()

			' Force future UI to relayout icon
			Dim f As JInternalFrame = desktopIcon.internalFrame
			If f.icon Then
				Dim desktop As JDesktopPane = desktopIcon.desktopPane
				If desktop IsNot Nothing Then
					Dim desktopManager As DesktopManager = desktop.desktopManager
					If TypeOf desktopManager Is DefaultDesktopManager Then
						' This will cause DefaultDesktopManager to layout the icon
						f.putClientProperty("wasIconOnce", Nothing)
						' Move aside to allow fresh layout of all icons
						desktopIcon.locationion(Integer.MinValue, 0)
					End If
				End If
			End If

			uninstallListeners()
			frame = Nothing
			desktopIcon = Nothing
		End Sub

		Protected Friend Overridable Sub installComponents()
			iconPane = New BasicInternalFrameTitlePane(frame)
			desktopIcon.layout = New BorderLayout
			desktopIcon.add(iconPane, BorderLayout.CENTER)
		End Sub

		Protected Friend Overridable Sub uninstallComponents()
			desktopIcon.remove(iconPane)
			desktopIcon.layout = Nothing
			iconPane = Nothing
		End Sub

		Protected Friend Overridable Sub installListeners()
			mouseInputListener = createMouseInputListener()
			desktopIcon.addMouseMotionListener(mouseInputListener)
			desktopIcon.addMouseListener(mouseInputListener)
		End Sub

		Protected Friend Overridable Sub uninstallListeners()
			desktopIcon.removeMouseMotionListener(mouseInputListener)
			desktopIcon.removeMouseListener(mouseInputListener)
			mouseInputListener = Nothing
		End Sub

		Protected Friend Overridable Sub installDefaults()
			LookAndFeel.installBorder(desktopIcon, "DesktopIcon.border")
			LookAndFeel.installProperty(desktopIcon, "opaque", Boolean.TRUE)
		End Sub

		Protected Friend Overridable Sub uninstallDefaults()
			LookAndFeel.uninstallBorder(desktopIcon)
		End Sub

		Protected Friend Overridable Function createMouseInputListener() As MouseInputListener
			Return New MouseInputHandler(Me)
		End Function

		Public Overridable Function getPreferredSize(ByVal c As JComponent) As Dimension
			Return desktopIcon.layout.preferredLayoutSize(desktopIcon)
		End Function

		Public Overridable Function getMinimumSize(ByVal c As JComponent) As Dimension
			Dim [dim] As New Dimension(iconPane.minimumSize)
			Dim border As Border = frame.border

			If border IsNot Nothing Then [dim].height += border.getBorderInsets(frame).bottom + border.getBorderInsets(frame).top
			Return [dim]
		End Function

		''' <summary>
		''' Desktop icons can not be resized.  Therefore, we should always
		''' return the minimum size of the desktop icon.
		''' </summary>
		''' <seealso cref= #getMinimumSize </seealso>
		Public Overridable Function getMaximumSize(ByVal c As JComponent) As Dimension
			Return iconPane.maximumSize
		End Function

		Public Overridable Function getInsets(ByVal c As JComponent) As Insets
			Dim iframe As JInternalFrame = desktopIcon.internalFrame
			Dim border As Border = iframe.border
			If border IsNot Nothing Then Return border.getBorderInsets(iframe)

			Return New Insets(0,0,0,0)
		End Function

		Public Overridable Sub deiconize()
			Try
				frame.icon = False
			Catch e2 As PropertyVetoException
			End Try
		End Sub

		''' <summary>
		''' Listens for mouse movements and acts on them.
		''' 
		''' This class should be treated as a &quot;protected&quot; inner class.
		''' Instantiate it only within subclasses of {@code BasicDesktopIconUI}.
		''' </summary>
		Public Class MouseInputHandler
			Inherits MouseInputAdapter

			Private ReadOnly outerInstance As BasicDesktopIconUI

			Public Sub New(ByVal outerInstance As BasicDesktopIconUI)
				Me.outerInstance = outerInstance
			End Sub

			' _x & _y are the mousePressed location in absolute coordinate system
			Friend _x, _y As Integer
			' __x & __y are the mousePressed location in source view's coordinate system
			Friend __x, __y As Integer
			Friend startingBounds As Rectangle

			Public Overridable Sub mouseReleased(ByVal e As MouseEvent)
				_x = 0
				_y = 0
				__x = 0
				__y = 0
				startingBounds = Nothing

				Dim d As JDesktopPane
				d = outerInstance.desktopIcon.desktopPane
				If d IsNot Nothing Then
					Dim dm As DesktopManager = d.desktopManager
					dm.endDraggingFrame(outerInstance.desktopIcon)
				End If

			End Sub

			Public Overridable Sub mousePressed(ByVal e As MouseEvent)
				Dim p As Point = SwingUtilities.convertPoint(CType(e.source, Component), e.x, e.y, Nothing)
				__x = e.x
				__y = e.y
				_x = p.x
				_y = p.y
				startingBounds = outerInstance.desktopIcon.bounds

				Dim d As JDesktopPane
				d = outerInstance.desktopIcon.desktopPane
				If d IsNot Nothing Then
					Dim dm As DesktopManager = d.desktopManager
					dm.beginDraggingFrame(outerInstance.desktopIcon)
				End If

				Try
					outerInstance.frame.selected = True
				Catch e1 As PropertyVetoException
				End Try
				If TypeOf outerInstance.desktopIcon.parent Is JLayeredPane Then CType(outerInstance.desktopIcon.parent, JLayeredPane).moveToFront(outerInstance.desktopIcon)

				If e.clickCount > 1 Then
					If outerInstance.frame.iconifiable AndAlso outerInstance.frame.icon Then outerInstance.deiconize()
				End If

			End Sub

			 Public Overridable Sub mouseMoved(ByVal e As MouseEvent)
			 End Sub

			 Public Overridable Sub mouseDragged(ByVal e As MouseEvent)
				Dim p As Point
				Dim newX, newY, newW, newH As Integer
				Dim deltaX As Integer
				Dim deltaY As Integer
				Dim min As Dimension
				Dim max As Dimension
				p = SwingUtilities.convertPoint(CType(e.source, Component), e.x, e.y, Nothing)

					Dim i As Insets = outerInstance.desktopIcon.insets
					Dim pWidth, pHeight As Integer
					pWidth = CType(outerInstance.desktopIcon.parent, JComponent).width
					pHeight = CType(outerInstance.desktopIcon.parent, JComponent).height

					If startingBounds Is Nothing Then Return
					newX = startingBounds.x - (_x - p.x)
					newY = startingBounds.y - (_y - p.y)
					' Make sure we stay in-bounds
					If newX + i.left <= -__x Then newX = -__x - i.left
					If newY + i.top <= -__y Then newY = -__y - i.top
					If newX + __x + i.right > pWidth Then newX = pWidth - __x - i.right
					If newY + __y + i.bottom > pHeight Then newY = pHeight - __y - i.bottom

					Dim d As JDesktopPane
					d = outerInstance.desktopIcon.desktopPane
					If d IsNot Nothing Then
						Dim dm As DesktopManager = d.desktopManager
						dm.dragFrame(outerInstance.desktopIcon, newX, newY)
					Else
						moveAndRepaint(outerInstance.desktopIcon, newX, newY, outerInstance.desktopIcon.width, outerInstance.desktopIcon.height)
					End If
					Return
			 End Sub

			Public Overridable Sub moveAndRepaint(ByVal f As JComponent, ByVal newX As Integer, ByVal newY As Integer, ByVal newWidth As Integer, ByVal newHeight As Integer)
				Dim r As Rectangle = f.bounds
				f.boundsnds(newX, newY, newWidth, newHeight)
				SwingUtilities.computeUnion(newX, newY, newWidth, newHeight, r)
				f.parent.repaint(r.x, r.y, r.width, r.height)
			End Sub
		End Class '/ End MotionListener

	End Class

End Namespace
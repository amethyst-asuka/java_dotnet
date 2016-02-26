Imports System

'
' * Copyright (c) 1997, 2011, Oracle and/or its affiliates. All rights reserved.
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


Namespace javax.swing



	''' <summary>
	''' This is an implementation of the <code>DesktopManager</code>.
	''' It currently implements the basic behaviors for managing
	''' <code>JInternalFrame</code>s in an arbitrary parent.
	''' <code>JInternalFrame</code>s that are not children of a
	''' <code>JDesktop</code> will use this component
	''' to handle their desktop-like actions.
	''' <p>This class provides a policy for the various JInternalFrame methods,
	''' it is not meant to be called directly rather the various JInternalFrame
	''' methods will call into the DesktopManager.</p> </summary>
	''' <seealso cref= JDesktopPane </seealso>
	''' <seealso cref= JInternalFrame
	''' @author David Kloba
	''' @author Steve Wilson </seealso>
	<Serializable> _
	Public Class DefaultDesktopManager
		Implements DesktopManager

		Friend Const HAS_BEEN_ICONIFIED_PROPERTY As String = "wasIconOnce"

		Friend Const DEFAULT_DRAG_MODE As Integer = 0
		Friend Const OUTLINE_DRAG_MODE As Integer = 1
		Friend Const FASTER_DRAG_MODE As Integer = 2

		Friend dragMode As Integer = DEFAULT_DRAG_MODE

		<NonSerialized> _
		Private currentBounds As Rectangle = Nothing
		<NonSerialized> _
		Private desktopGraphics As Graphics = Nothing
		<NonSerialized> _
		Private desktopBounds As Rectangle = Nothing
		<NonSerialized> _
		Private floatingItems As Rectangle() = {}

		''' <summary>
		''' Set to true when the user actually drags a frame vs clicks on it
		''' to start the drag operation.  This is only used when dragging with
		''' FASTER_DRAG_MODE.
		''' </summary>
		<NonSerialized> _
		Private didDrag As Boolean

		''' <summary>
		''' Normally this method will not be called. If it is, it
		''' try to determine the appropriate parent from the desktopIcon of the frame.
		''' Will remove the desktopIcon from its parent if it successfully adds the frame.
		''' </summary>
		Public Overridable Sub openFrame(ByVal f As JInternalFrame) Implements DesktopManager.openFrame
			If f.desktopIcon.parent IsNot Nothing Then
				f.desktopIcon.parent.add(f)
				removeIconFor(f)
			End If
		End Sub

		''' <summary>
		''' Removes the frame, and, if necessary, the
		''' <code>desktopIcon</code>, from its parent. </summary>
		''' <param name="f"> the <code>JInternalFrame</code> to be removed </param>
		Public Overridable Sub closeFrame(ByVal f As JInternalFrame) Implements DesktopManager.closeFrame
			Dim d As JDesktopPane = f.desktopPane
			If d Is Nothing Then Return
			Dim findNext As Boolean = f.selected
			Dim c As Container = f.parent
			Dim nextFrame As JInternalFrame = Nothing
			If findNext Then
				nextFrame = d.getNextFrame(f)
				Try
					f.selected = False
				Catch e2 As java.beans.PropertyVetoException
				End Try
			End If
			If c IsNot Nothing Then
				c.remove(f) ' Removes the focus.
				c.repaint(f.x, f.y, f.width, f.height)
			End If
			removeIconFor(f)
			If f.normalBounds IsNot Nothing Then f.normalBounds = Nothing
			If wasIcon(f) Then wasIconcon(f, Nothing)
			If nextFrame IsNot Nothing Then
				Try
					nextFrame.selected = True
				Catch e2 As java.beans.PropertyVetoException
				End Try
			ElseIf findNext AndAlso d.componentCount = 0 Then
				' It was selected and was the last component on the desktop.
				d.requestFocus()
			End If
		End Sub

		''' <summary>
		''' Resizes the frame to fill its parents bounds. </summary>
		''' <param name="f"> the frame to be resized </param>
		Public Overridable Sub maximizeFrame(ByVal f As JInternalFrame) Implements DesktopManager.maximizeFrame
			If f.icon Then
				Try
					' In turn calls deiconifyFrame in the desktop manager.
					' That method will handle the maximization of the frame.
					f.icon = False
				Catch e2 As java.beans.PropertyVetoException
				End Try
			Else
				f.normalBounds = f.bounds
				Dim desktopBounds As Rectangle = f.parent.bounds
				boundsForFrameame(f, 0, 0, desktopBounds.width, desktopBounds.height)
			End If

			' Set the maximized frame as selected.
			Try
				f.selected = True
			Catch e2 As java.beans.PropertyVetoException
			End Try
		End Sub

		''' <summary>
		''' Restores the frame back to its size and position prior
		''' to a <code>maximizeFrame</code> call. </summary>
		''' <param name="f"> the <code>JInternalFrame</code> to be restored </param>
		Public Overridable Sub minimizeFrame(ByVal f As JInternalFrame) Implements DesktopManager.minimizeFrame
			' If the frame was an icon restore it back to an icon.
			If f.icon Then
				iconifyFrame(f)
				Return
			End If

			If (f.normalBounds) IsNot Nothing Then
				Dim r As Rectangle = f.normalBounds
				f.normalBounds = Nothing
				Try
					f.selected = True
				Catch e2 As java.beans.PropertyVetoException
				End Try
				boundsForFrameame(f, r.x, r.y, r.width, r.height)
			End If
		End Sub

		''' <summary>
		''' Removes the frame from its parent and adds its
		''' <code>desktopIcon</code> to the parent. </summary>
		''' <param name="f"> the <code>JInternalFrame</code> to be iconified </param>
		Public Overridable Sub iconifyFrame(ByVal f As JInternalFrame) Implements DesktopManager.iconifyFrame
			Dim desktopIcon As JInternalFrame.JDesktopIcon
			Dim c As Container = f.parent
			Dim d As JDesktopPane = f.desktopPane
			Dim findNext As Boolean = f.selected
			desktopIcon = f.desktopIcon
			If Not wasIcon(f) Then
				Dim r As Rectangle = getBoundsForIconOf(f)
				desktopIcon.boundsnds(r.x, r.y, r.width, r.height)
				' we must validate the hierarchy to not break the hw/lw mixing
				desktopIcon.revalidate()
				wasIconcon(f, Boolean.TRUE)
			End If

			If c Is Nothing OrElse d Is Nothing Then Return

			If TypeOf c Is JLayeredPane Then
				Dim lp As JLayeredPane = CType(c, JLayeredPane)
				Dim layer As Integer = lp.getLayer(f)
				lp.putLayer(desktopIcon, layer)
			End If

			' If we are maximized we already have the normal bounds recorded
			' don't try to re-record them, otherwise we incorrectly set the
			' normal bounds to maximized state.
			If Not f.maximum Then f.normalBounds = f.bounds
			d.componentOrderCheckingEnabled = False
			c.remove(f)
			c.add(desktopIcon)
			d.componentOrderCheckingEnabled = True
			c.repaint(f.x, f.y, f.width, f.height)
			If findNext Then
				If d.selectFrame(True) Is Nothing Then f.restoreSubcomponentFocus()
			End If
		End Sub

		''' <summary>
		''' Removes the desktopIcon from its parent and adds its frame
		''' to the parent. </summary>
		''' <param name="f"> the <code>JInternalFrame</code> to be de-iconified </param>
		Public Overridable Sub deiconifyFrame(ByVal f As JInternalFrame) Implements DesktopManager.deiconifyFrame
			Dim desktopIcon As JInternalFrame.JDesktopIcon = f.desktopIcon
			Dim c As Container = desktopIcon.parent
			Dim d As JDesktopPane = f.desktopPane
			If c IsNot Nothing AndAlso d IsNot Nothing Then
				c.add(f)
				' If the frame is to be restored to a maximized state make
				' sure it still fills the whole desktop.
				If f.maximum Then
					Dim desktopBounds As Rectangle = c.bounds
					If f.width <> desktopBounds.width OrElse f.height <> desktopBounds.height Then boundsForFrameame(f, 0, 0, desktopBounds.width, desktopBounds.height)
				End If
				removeIconFor(f)
				If f.selected Then
					f.moveToFront()
					f.restoreSubcomponentFocus()
				Else
					Try
						f.selected = True
					Catch e2 As java.beans.PropertyVetoException
					End Try

				End If
			End If
		End Sub

		''' <summary>
		''' This will activate <b>f</b> moving it to the front. It will
		''' set the current active frame's (if any)
		''' <code>IS_SELECTED_PROPERTY</code> to <code>false</code>.
		''' There can be only one active frame across all Layers. </summary>
		''' <param name="f"> the <code>JInternalFrame</code> to be activated </param>
		Public Overridable Sub activateFrame(ByVal f As JInternalFrame) Implements DesktopManager.activateFrame
			Dim p As Container = f.parent
			Dim c As Component()
			Dim d As JDesktopPane = f.desktopPane
			Dim currentlyActiveFrame As JInternalFrame = If(d Is Nothing, Nothing, d.selectedFrame)
			' fix for bug: 4162443
			If p Is Nothing Then
				' If the frame is not in parent, its icon maybe, check it
				p = f.desktopIcon.parent
				If p Is Nothing Then Return
			End If
			' we only need to keep track of the currentActive InternalFrame, if any
			If currentlyActiveFrame Is Nothing Then
			  If d IsNot Nothing Then d.selectedFrame = f
			ElseIf currentlyActiveFrame IsNot f Then
			  ' if not the same frame as the current active
			  ' we deactivate the current
			  If currentlyActiveFrame.selected Then
				Try
				  currentlyActiveFrame.selected = False
				Catch e2 As java.beans.PropertyVetoException
				End Try
			  End If
			  If d IsNot Nothing Then d.selectedFrame = f
			End If
			f.moveToFront()
		End Sub

		' implements javax.swing.DesktopManager
		Public Overridable Sub deactivateFrame(ByVal f As JInternalFrame) Implements DesktopManager.deactivateFrame
		  Dim d As JDesktopPane = f.desktopPane
		  Dim currentlyActiveFrame As JInternalFrame = If(d Is Nothing, Nothing, d.selectedFrame)
		  If currentlyActiveFrame Is f Then d.selectedFrame = Nothing
		End Sub

		' implements javax.swing.DesktopManager
		Public Overridable Sub beginDraggingFrame(ByVal f As JComponent) Implements DesktopManager.beginDraggingFrame
			setupDragMode(f)

			If dragMode = FASTER_DRAG_MODE Then
			  Dim desktop As Component = f.parent
			  floatingItems = findFloatingItems(f)
			  currentBounds = f.bounds
			  If TypeOf desktop Is JComponent Then
				  desktopBounds = CType(desktop, JComponent).visibleRect
			  Else
				  desktopBounds = desktop.bounds
					  desktopBounds.y = 0
					  desktopBounds.x = desktopBounds.y
			  End If
			  desktopGraphics = JComponent.safelyGetGraphics(desktop)
			  CType(f, JInternalFrame).isDragging = True
			  didDrag = False
			End If

		End Sub

		Private Sub setupDragMode(ByVal f As JComponent)
			Dim p As JDesktopPane = getDesktopPane(f)
			Dim parent As Container = f.parent
			dragMode = DEFAULT_DRAG_MODE
			If p IsNot Nothing Then
				Dim mode As String = CStr(p.getClientProperty("JDesktopPane.dragMode"))
				Dim window As Window = SwingUtilities.getWindowAncestor(f)
				If window IsNot Nothing AndAlso (Not com.sun.awt.AWTUtilities.isWindowOpaque(window)) Then
					dragMode = DEFAULT_DRAG_MODE
				ElseIf mode IsNot Nothing AndAlso mode.Equals("outline") Then
					dragMode = OUTLINE_DRAG_MODE
				ElseIf mode IsNot Nothing AndAlso mode.Equals("faster") AndAlso TypeOf f Is JInternalFrame AndAlso CType(f, JInternalFrame).opaque AndAlso (parent Is Nothing OrElse parent.opaque) Then
					dragMode = FASTER_DRAG_MODE
				Else
					If p.dragMode = JDesktopPane.OUTLINE_DRAG_MODE Then
						dragMode = OUTLINE_DRAG_MODE
					ElseIf p.dragMode = JDesktopPane.LIVE_DRAG_MODE AndAlso TypeOf f Is JInternalFrame AndAlso CType(f, JInternalFrame).opaque Then
						dragMode = FASTER_DRAG_MODE
					Else
						dragMode = DEFAULT_DRAG_MODE
					End If
				End If
			End If
		End Sub

		<NonSerialized> _
		Private currentLoc As Point = Nothing

		''' <summary>
		''' Moves the visible location of the frame being dragged
		''' to the location specified.  The means by which this occurs can vary depending
		''' on the dragging algorithm being used.  The actual logical location of the frame
		''' might not change until <code>endDraggingFrame</code> is called.
		''' </summary>
		Public Overridable Sub dragFrame(ByVal f As JComponent, ByVal newX As Integer, ByVal newY As Integer) Implements DesktopManager.dragFrame

			If dragMode = OUTLINE_DRAG_MODE Then
				Dim ___desktopPane As JDesktopPane = getDesktopPane(f)
				If ___desktopPane IsNot Nothing Then
				  Dim g As Graphics = JComponent.safelyGetGraphics(___desktopPane)

				  g.xORMode = Color.white
				  If currentLoc IsNot Nothing Then g.drawRect(currentLoc.x, currentLoc.y, f.width-1, f.height-1)
				  g.drawRect(newX, newY, f.width-1, f.height-1)
	'               Work around for 6635462: XOR mode may cause a SurfaceLost on first use.
	'              * Swing doesn't expect that its XOR drawRect did
	'              * not complete, so believes that on re-entering at
	'              * the next update location, that there is an XOR rect
	'              * to draw out at "currentLoc". But in fact
	'              * its now got a new clean surface without that rect,
	'              * so drawing it "out" in fact draws it on, leaving garbage.
	'              * So only update/set currentLoc if the draw completed.
	'              
				  Dim sData As sun.java2d.SurfaceData = CType(g, sun.java2d.SunGraphics2D).surfaceData

				  If Not sData.surfaceLost Then currentLoc = New Point(newX, newY)

				  g.Dispose()
				End If
			ElseIf dragMode = FASTER_DRAG_MODE Then
				dragFrameFaster(f, newX, newY)
			Else
				boundsForFrameame(f, newX, newY, f.width, f.height)
			End If
		End Sub

		' implements javax.swing.DesktopManager
		Public Overridable Sub endDraggingFrame(ByVal f As JComponent) Implements DesktopManager.endDraggingFrame
			If dragMode = OUTLINE_DRAG_MODE AndAlso currentLoc IsNot Nothing Then
				boundsForFrameame(f, currentLoc.x, currentLoc.y, f.width, f.height)
				currentLoc = Nothing
			ElseIf dragMode = FASTER_DRAG_MODE Then
				currentBounds = Nothing
				If desktopGraphics IsNot Nothing Then
					desktopGraphics.Dispose()
					desktopGraphics = Nothing
				End If
				desktopBounds = Nothing
				CType(f, JInternalFrame).isDragging = False
			End If
		End Sub

		' implements javax.swing.DesktopManager
		Public Overridable Sub beginResizingFrame(ByVal f As JComponent, ByVal direction As Integer) Implements DesktopManager.beginResizingFrame
			setupDragMode(f)
		End Sub

		''' <summary>
		''' Calls <code>setBoundsForFrame</code> with the new values. </summary>
		''' <param name="f"> the component to be resized </param>
		''' <param name="newX"> the new x-coordinate </param>
		''' <param name="newY"> the new y-coordinate </param>
		''' <param name="newWidth"> the new width </param>
		''' <param name="newHeight"> the new height </param>
		Public Overridable Sub resizeFrame(ByVal f As JComponent, ByVal newX As Integer, ByVal newY As Integer, ByVal newWidth As Integer, ByVal newHeight As Integer) Implements DesktopManager.resizeFrame

			If dragMode = DEFAULT_DRAG_MODE OrElse dragMode = FASTER_DRAG_MODE Then
				boundsForFrameame(f, newX, newY, newWidth, newHeight)
			Else
				Dim ___desktopPane As JDesktopPane = getDesktopPane(f)
				If ___desktopPane IsNot Nothing Then
				  Dim g As Graphics = JComponent.safelyGetGraphics(___desktopPane)

				  g.xORMode = Color.white
				  If currentBounds IsNot Nothing Then g.drawRect(currentBounds.x, currentBounds.y, currentBounds.width-1, currentBounds.height-1)
				  g.drawRect(newX, newY, newWidth-1, newHeight-1)

				  ' Work around for 6635462, see comment in dragFrame()
				  Dim sData As sun.java2d.SurfaceData = CType(g, sun.java2d.SunGraphics2D).surfaceData
				  If Not sData.surfaceLost Then currentBounds = New Rectangle(newX, newY, newWidth, newHeight)

				  g.paintModeode()
				  g.Dispose()
				End If
			End If

		End Sub

		' implements javax.swing.DesktopManager
		Public Overridable Sub endResizingFrame(ByVal f As JComponent) Implements DesktopManager.endResizingFrame
			If dragMode = OUTLINE_DRAG_MODE AndAlso currentBounds IsNot Nothing Then
				boundsForFrameame(f, currentBounds.x, currentBounds.y, currentBounds.width, currentBounds.height)
				currentBounds = Nothing
			End If
		End Sub


		''' <summary>
		''' This moves the <code>JComponent</code> and repaints the damaged areas. </summary>
		Public Overridable Sub setBoundsForFrame(ByVal f As JComponent, ByVal newX As Integer, ByVal newY As Integer, ByVal newWidth As Integer, ByVal newHeight As Integer) Implements DesktopManager.setBoundsForFrame
			f.boundsnds(newX, newY, newWidth, newHeight)
			' we must validate the hierarchy to not break the hw/lw mixing
			f.revalidate()
		End Sub

		''' <summary>
		''' Convenience method to remove the desktopIcon of <b>f</b> is necessary. </summary>
		Protected Friend Overridable Sub removeIconFor(ByVal f As JInternalFrame)
			Dim di As JInternalFrame.JDesktopIcon = f.desktopIcon
			Dim c As Container = di.parent
			If c IsNot Nothing Then
				c.remove(di)
				c.repaint(di.x, di.y, di.width, di.height)
			End If
		End Sub

		''' <summary>
		''' The iconifyFrame() code calls this to determine the proper bounds
		''' for the desktopIcon.
		''' </summary>

		Protected Friend Overridable Function getBoundsForIconOf(ByVal f As JInternalFrame) As Rectangle
		  '
		  ' Get the icon for this internal frame and its preferred size
		  '

		  Dim icon As JInternalFrame.JDesktopIcon = f.desktopIcon
		  Dim prefSize As Dimension = icon.preferredSize
		  '
		  ' Get the parent bounds and child components.
		  '

		  Dim c As Container = f.parent
		  If c Is Nothing Then c = f.desktopIcon.parent

		  If c Is Nothing Then Return New Rectangle(0, 0, prefSize.width, prefSize.height)

		  Dim parentBounds As Rectangle = c.bounds
		  Dim components As Component() = c.components


		  '
		  ' Iterate through valid default icon locations and return the
		  ' first one that does not intersect any other icons.
		  '

		  Dim availableRectangle As Rectangle = Nothing
		  Dim currentIcon As JInternalFrame.JDesktopIcon = Nothing

		  Dim x As Integer = 0
		  Dim y As Integer = parentBounds.height - prefSize.height
		  Dim w As Integer = prefSize.width
		  Dim h As Integer = prefSize.height

		  Dim found As Boolean = False

		  Do While Not found

			availableRectangle = New Rectangle(x,y,w,h)

			found = True

			For i As Integer = 0 To components.Length - 1

			  '
			  ' Get the icon for this component
			  '

			  If TypeOf components(i) Is JInternalFrame Then
				currentIcon = CType(components(i), JInternalFrame).desktopIcon
			  ElseIf TypeOf components(i) Is JInternalFrame.JDesktopIcon Then
				currentIcon = CType(components(i), JInternalFrame.JDesktopIcon)
			  Else
	'             found a child that's neither an internal frame nor
	'               an icon. I don't believe this should happen, but at
	'               present it does and causes a null pointer exception.
	'               Even when that gets fixed, this code protects against
	'               the npe. hania 
				Continue For
			  End If

			  '
			  ' If this icon intersects the current location, get next location.
			  '

			  If Not currentIcon.Equals(icon) Then
				If availableRectangle.intersects(currentIcon.bounds) Then
				  found = False
				  Exit For
				End If
			  End If
			Next i

			If currentIcon Is Nothing Then Return availableRectangle

			x += currentIcon.bounds.width

			If x + w > parentBounds.width Then
			  x = 0
			  y -= h
			End If
		  Loop

		  Return (availableRectangle)
		End Function

		''' <summary>
		''' Stores the bounds of the component just before a maximize call. </summary>
		''' <param name="f"> the component about to be resized </param>
		''' <param name="r"> the normal bounds to be saved away </param>
		Protected Friend Overridable Sub setPreviousBounds(ByVal f As JInternalFrame, ByVal r As Rectangle)
			f.normalBounds = r
		End Sub

		''' <summary>
		''' Gets the normal bounds of the component prior to the component
		''' being maximized. </summary>
		''' <param name="f"> the <code>JInternalFrame</code> of interest </param>
		''' <returns> the normal bounds of the component </returns>
		Protected Friend Overridable Function getPreviousBounds(ByVal f As JInternalFrame) As Rectangle
			Return f.normalBounds
		End Function

		''' <summary>
		''' Sets that the component has been iconized and the bounds of the
		''' <code>desktopIcon</code> are valid.
		''' </summary>
		Protected Friend Overridable Sub setWasIcon(ByVal f As JInternalFrame, ByVal value As Boolean?)
			If value IsNot Nothing Then f.putClientProperty(HAS_BEEN_ICONIFIED_PROPERTY, value)
		End Sub

		''' <summary>
		''' Returns <code>true</code> if the component has been iconized
		''' and the bounds of the <code>desktopIcon</code> are valid,
		''' otherwise returns <code>false</code>.
		''' </summary>
		''' <param name="f"> the <code>JInternalFrame</code> of interest </param>
		''' <returns> <code>true</code> if the component has been iconized;
		'''    otherwise returns <code>false</code> </returns>
		Protected Friend Overridable Function wasIcon(ByVal f As JInternalFrame) As Boolean
			Return (f.getClientProperty(HAS_BEEN_ICONIFIED_PROPERTY) Is Boolean.TRUE)
		End Function


		Friend Overridable Function getDesktopPane(ByVal frame As JComponent) As JDesktopPane
			Dim pane As JDesktopPane = Nothing
			Dim c As Component = frame.parent

			' Find the JDesktopPane
			Do While pane Is Nothing
				If TypeOf c Is JDesktopPane Then
					pane = CType(c, JDesktopPane)
				ElseIf c Is Nothing Then
					Exit Do
				Else
					c = c.parent
				End If
			Loop

			Return pane
		End Function


	  ' =========== stuff for faster frame dragging ===================

	   Private Sub dragFrameFaster(ByVal f As JComponent, ByVal newX As Integer, ByVal newY As Integer)

		  Dim ___previousBounds As New Rectangle(currentBounds.x, currentBounds.y, currentBounds.width, currentBounds.height)

	   ' move the frame
		  currentBounds.x = newX
		  currentBounds.y = newY

		  If didDrag Then
			  ' Only initiate cleanup if we have actually done a drag.
			  emergencyCleanup(f)
		  Else
			  didDrag = True
			  ' We reset the danger field as until now we haven't actually
			  ' moved the internal frame so we don't need to initiate repaint.
			  CType(f, JInternalFrame).danger = False
		  End If

		  Dim ___floaterCollision As Boolean = isFloaterCollision(___previousBounds, currentBounds)

		  Dim parent As JComponent = CType(f.parent, JComponent)
		  Dim visBounds As Rectangle = ___previousBounds.intersection(desktopBounds)

		  Dim currentManager As RepaintManager = RepaintManager.currentManager(f)

		  currentManager.beginPaint()
		  Try
			  If Not ___floaterCollision Then currentManager.copyArea(parent, desktopGraphics, visBounds.x, visBounds.y, visBounds.width, visBounds.height, newX - ___previousBounds.x, newY - ___previousBounds.y, True)

			  f.bounds = currentBounds

			  If Not ___floaterCollision Then
				  Dim r As Rectangle = currentBounds
				  currentManager.notifyRepaintPerformed(parent, r.x, r.y, r.width, r.height)
			  End If

			  If ___floaterCollision Then
				  ' since we couldn't blit we just redraw as fast as possible
				  ' the isDragging mucking is to avoid activating emergency
				  ' cleanup
				  CType(f, JInternalFrame).isDragging = False
				  parent.paintImmediately(currentBounds)
				  CType(f, JInternalFrame).isDragging = True
			  End If

			  ' fake out the repaint manager.  We'll take care of everything

			  currentManager.markCompletelyClean(parent)
			  currentManager.markCompletelyClean(f)

			  ' compute the minimal newly exposed area
			  ' if the rects intersect then we use computeDifference.  Otherwise
			  ' we'll repaint the entire previous bounds
			  Dim dirtyRects As Rectangle() = Nothing
			  If ___previousBounds.intersects(currentBounds) Then
				  dirtyRects = SwingUtilities.computeDifference(___previousBounds, currentBounds)
			  Else
				  dirtyRects = New Rectangle(0){}
				  dirtyRects(0) = ___previousBounds
			  End If

			  ' Fix the damage
			  For i As Integer = 0 To dirtyRects.Length - 1
				  parent.paintImmediately(dirtyRects(i))
				  Dim r As Rectangle = dirtyRects(i)
				  currentManager.notifyRepaintPerformed(parent, r.x, r.y, r.width, r.height)
			  Next i

			  ' new areas of blit were exposed
			  If Not(visBounds.Equals(___previousBounds)) Then
				  dirtyRects = SwingUtilities.computeDifference(___previousBounds, desktopBounds)
				  For i As Integer = 0 To dirtyRects.Length - 1
					  dirtyRects(i).x += newX - ___previousBounds.x
					  dirtyRects(i).y += newY - ___previousBounds.y
					  CType(f, JInternalFrame).isDragging = False
					  parent.paintImmediately(dirtyRects(i))
					  CType(f, JInternalFrame).isDragging = True
					  Dim r As Rectangle = dirtyRects(i)
					  currentManager.notifyRepaintPerformed(parent, r.x, r.y, r.width, r.height)
				  Next i

			  End If
		  Finally
			  currentManager.endPaint()
		  End Try

		  ' update window if it's non-opaque
		  Dim topLevel As Window = SwingUtilities.getWindowAncestor(f)
		  Dim tk As Toolkit = Toolkit.defaultToolkit
		  If (Not topLevel.opaque) AndAlso (TypeOf tk Is sun.awt.SunToolkit) AndAlso CType(tk, sun.awt.SunToolkit).needUpdateWindow() Then sun.awt.AWTAccessor.windowAccessor.updateWindow(topLevel)
	   End Sub

	   Private Function isFloaterCollision(ByVal moveFrom As Rectangle, ByVal moveTo As Rectangle) As Boolean
		  If floatingItems.Length = 0 Then Return False

		  For i As Integer = 0 To floatingItems.Length - 1
			 Dim intersectsFrom As Boolean = moveFrom.intersects(floatingItems(i))
			 If intersectsFrom Then Return True
			 Dim intersectsTo As Boolean = moveTo.intersects(floatingItems(i))
			 If intersectsTo Then Return True
		  Next i

		  Return False
	   End Function

	   Private Function findFloatingItems(ByVal f As JComponent) As Rectangle()
		  Dim desktop As Container = f.parent
		  Dim children As Component() = desktop.components
		  Dim i As Integer = 0
		  For i = 0 To children.Length - 1
			 If children(i) Is f Then Exit For
		  Next i
		  ' System.out.println(i);
		  Dim floaters As Rectangle() = New Rectangle(i - 1){}
		  For i = 0 To floaters.Length - 1
			 floaters(i) = children(i).bounds
		  Next i

		  Return floaters
	   End Function

	   ''' <summary>
	   ''' This method is here to clean up problems associated
	   ''' with a race condition which can occur when the full contents
	   ''' of a copyArea's source argument is not available onscreen.
	   ''' This uses brute force to clean up in case of possible damage
	   ''' </summary>
	   Private Sub emergencyCleanup(ByVal f As JComponent)

			If CType(f, JInternalFrame).danger Then CType(f, JInternalFrame).danger = False

	   End Sub


	End Class

End Namespace
Imports System
Imports javax.swing
Imports javax.swing.border
Imports javax.swing.plaf

'
' * Copyright (c) 2002, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.swing.plaf.synth


	''' <summary>
	''' Provides the Synth L&amp;F UI delegate for
	''' <seealso cref="javax.swing.JDesktopPane"/>.
	''' 
	''' @author Joshua Outwater
	''' @author Steve Wilson
	''' @since 1.7
	''' </summary>
	Public Class SynthDesktopPaneUI
		Inherits javax.swing.plaf.basic.BasicDesktopPaneUI
		Implements PropertyChangeListener, SynthUI

		Private style As SynthStyle
		Private taskBar As TaskBar
		Private oldDesktopManager As DesktopManager

		''' <summary>
		''' Creates a new UI object for the given component.
		''' </summary>
		''' <param name="c"> component to create UI object for </param>
		''' <returns> the UI object </returns>
		Public Shared Function createUI(ByVal c As JComponent) As ComponentUI
			Return New SynthDesktopPaneUI
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub installListeners()
			MyBase.installListeners()
			desktop.addPropertyChangeListener(Me)
			If taskBar IsNot Nothing Then
				' Listen for desktop being resized
				desktop.addComponentListener(taskBar)
				' Listen for frames being added to desktop
				desktop.addContainerListener(taskBar)
			End If
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub installDefaults()
			updateStyle(desktop)

			If UIManager.getBoolean("InternalFrame.useTaskBar") Then
				taskBar = New TaskBar

				For Each comp As Component In desktop.components
					Dim desktopIcon As JInternalFrame.JDesktopIcon

					If TypeOf comp Is JInternalFrame.JDesktopIcon Then
						desktopIcon = CType(comp, JInternalFrame.JDesktopIcon)
					ElseIf TypeOf comp Is JInternalFrame Then
						desktopIcon = CType(comp, JInternalFrame).desktopIcon
					Else
						Continue For
					End If
					' Move desktopIcon from desktop to taskBar
					If desktopIcon.parent Is desktop Then desktop.remove(desktopIcon)
					If desktopIcon.parent IsNot taskBar Then
						taskBar.add(desktopIcon)
						desktopIcon.internalFrame.addComponentListener(taskBar)
					End If
				Next comp
				taskBar.background = desktop.background
				desktop.add(taskBar, Convert.ToInt32(JLayeredPane.PALETTE_LAYER + 1))
				If desktop.showing Then taskBar.adjustSize()
			End If
		End Sub

		Private Sub updateStyle(ByVal c As JDesktopPane)
			Dim oldStyle As SynthStyle = style
			Dim ___context As SynthContext = getContext(c, ENABLED)
			style = SynthLookAndFeel.updateStyle(___context, Me)
			If oldStyle IsNot Nothing Then
				uninstallKeyboardActions()
				installKeyboardActions()
			End If
			___context.Dispose()
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub uninstallListeners()
			If taskBar IsNot Nothing Then
				desktop.removeComponentListener(taskBar)
				desktop.removeContainerListener(taskBar)
			End If
			desktop.removePropertyChangeListener(Me)
			MyBase.uninstallListeners()
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub uninstallDefaults()
			Dim ___context As SynthContext = getContext(desktop, ENABLED)

			style.uninstallDefaults(___context)
			___context.Dispose()
			style = Nothing

			If taskBar IsNot Nothing Then
				For Each comp As Component In taskBar.components
					Dim desktopIcon As JInternalFrame.JDesktopIcon = CType(comp, JInternalFrame.JDesktopIcon)
					taskBar.remove(desktopIcon)
					desktopIcon.preferredSize = Nothing
					Dim f As JInternalFrame = desktopIcon.internalFrame
					If f.icon Then desktop.add(desktopIcon)
					f.removeComponentListener(taskBar)
				Next comp
				desktop.remove(taskBar)
				taskBar = Nothing
			End If
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub installDesktopManager()
			If UIManager.getBoolean("InternalFrame.useTaskBar") Then
					oldDesktopManager = desktop.desktopManager
					desktopManager = oldDesktopManager
				If Not(TypeOf desktopManager Is SynthDesktopManager) Then
					desktopManager = New SynthDesktopManager(Me)
					desktop.desktopManager = desktopManager
				End If
			Else
				MyBase.installDesktopManager()
			End If
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub uninstallDesktopManager()
			If oldDesktopManager IsNot Nothing AndAlso Not(TypeOf oldDesktopManager Is UIResource) Then
				desktopManager = desktop.desktopManager
				If desktopManager Is Nothing OrElse TypeOf desktopManager Is UIResource Then desktop.desktopManager = oldDesktopManager
			End If
			oldDesktopManager = Nothing
			MyBase.uninstallDesktopManager()
		End Sub

		Friend Class TaskBar
			Inherits JPanel
			Implements ComponentListener, ContainerListener

			Friend Sub New()
				opaque = True
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				setLayout(New FlowLayout(FlowLayout.LEFT, 0, 0)
	'			{
	'				public void layoutContainer(Container target)
	'				{
	'					' First shrink buttons to fit
	'					Component[] comps = target.getComponents();
	'					int n = comps.length;
	'					if (n > 0)
	'					{
	'						' Start with the largest preferred width
	'						int prefWidth = 0;
	'						for (Component c : comps)
	'						{
	'							c.setPreferredSize(Nothing);
	'							Dimension prefSize = c.getPreferredSize();
	'							if (prefSize.width > prefWidth)
	'							{
	'								prefWidth = prefSize.width;
	'							}
	'						}
	'						' Shrink equally to fit if needed
	'						Insets insets = target.getInsets();
	'						int tw = target.getWidth() - insets.left - insets.right;
	'						int w = Math.min(prefWidth, Math.max(10, tw/n));
	'						for (Component c : comps)
	'						{
	'							Dimension prefSize = c.getPreferredSize();
	'							c.setPreferredSize(New Dimension(w, prefSize.height));
	'						}
	'					}
	'					MyBase.layoutContainer(target);
	'				}
	'			});

				' PENDING: This should be handled by the painter
				borderder(New BevelBorderAnonymousInnerClassHelper
			End Sub

			Private Class BevelBorderAnonymousInnerClassHelper
				Inherits BevelBorder

				Protected Friend Overridable Sub paintRaisedBevel(ByVal c As Component, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
					Dim oldColor As Color = g.color
					g.translate(x, y)
					g.color = getHighlightOuterColor(c)
					g.drawLine(0, 0, 0, h-2)
					g.drawLine(1, 0, w-2, 0)
					g.color = getShadowOuterColor(c)
					g.drawLine(0, h-1, w-1, h-1)
					g.drawLine(w-1, 0, w-1, h-2)
					g.translate(-x, -y)
					g.color = oldColor
				End Sub
			End Class

			Friend Overridable Sub adjustSize()
				Dim desktop As JDesktopPane = CType(parent, JDesktopPane)
				If desktop IsNot Nothing Then
					Dim ___height As Integer = preferredSize.height
					Dim ___insets As Insets = insets
					If ___height = ___insets.top + ___insets.bottom Then
						If height <= ___height Then
							' Initial size, because we have no buttons yet
							___height += 21
						Else
							' We already have a good height
							___height = height
						End If
					End If
					boundsnds(0, desktop.height - ___height, desktop.width, ___height)
					revalidate()
					repaint()
				End If
			End Sub

			' ComponentListener interface

			Public Overridable Sub componentResized(ByVal e As ComponentEvent)
				If TypeOf e.source Is JDesktopPane Then adjustSize()
			End Sub

			Public Overridable Sub componentMoved(ByVal e As ComponentEvent)
			End Sub

			Public Overridable Sub componentShown(ByVal e As ComponentEvent)
				If TypeOf e.source Is JInternalFrame Then adjustSize()
			End Sub

			Public Overridable Sub componentHidden(ByVal e As ComponentEvent)
				If TypeOf e.source Is JInternalFrame Then
					CType(e.source, JInternalFrame).desktopIcon.visible = False
					revalidate()
				End If
			End Sub

			' ContainerListener interface

			Public Overridable Sub componentAdded(ByVal e As ContainerEvent)
				If TypeOf e.child Is JInternalFrame Then
					Dim desktop As JDesktopPane = CType(e.source, JDesktopPane)
					Dim f As JInternalFrame = CType(e.child, JInternalFrame)
					Dim desktopIcon As JInternalFrame.JDesktopIcon = f.desktopIcon
					For Each comp As Component In components
						If comp Is desktopIcon Then Return
					Next comp
					add(desktopIcon)
					f.addComponentListener(Me)
					If componentCount = 1 Then adjustSize()
				End If
			End Sub

			Public Overridable Sub componentRemoved(ByVal e As ContainerEvent)
				If TypeOf e.child Is JInternalFrame Then
					Dim f As JInternalFrame = CType(e.child, JInternalFrame)
					If Not f.icon Then
						' Frame was removed without using setClosed(true)
						remove(f.desktopIcon)
						f.removeComponentListener(Me)
						revalidate()
						repaint()
					End If
				End If
			End Sub
		End Class


		Friend Class SynthDesktopManager
			Inherits DefaultDesktopManager
			Implements UIResource

			Private ReadOnly outerInstance As SynthDesktopPaneUI

			Public Sub New(ByVal outerInstance As SynthDesktopPaneUI)
				Me.outerInstance = outerInstance
			End Sub


			Public Overrides Sub maximizeFrame(ByVal f As JInternalFrame)
				If f.icon Then
					Try
						f.icon = False
					Catch e2 As PropertyVetoException
					End Try
				Else
					f.normalBounds = f.bounds
					Dim desktop As Component = f.parent
					boundsForFrameame(f, 0, 0, desktop.width, desktop.height - outerInstance.taskBar.height)
				End If

				Try
					f.selected = True
				Catch e2 As PropertyVetoException
				End Try
			End Sub

			Public Overrides Sub iconifyFrame(ByVal f As JInternalFrame)
				Dim desktopIcon As JInternalFrame.JDesktopIcon
				Dim c As Container = f.parent
				Dim d As JDesktopPane = f.desktopPane
				Dim findNext As Boolean = f.selected

				If c Is Nothing Then Return

				desktopIcon = f.desktopIcon

				If Not f.maximum Then f.normalBounds = f.bounds
				c.remove(f)
				c.repaint(f.x, f.y, f.width, f.height)
				Try
					f.selected = False
				Catch e2 As PropertyVetoException
				End Try

				' Get topmost of the remaining frames
				If findNext Then
					For Each comp As Component In c.components
						If TypeOf comp Is JInternalFrame Then
							Try
								CType(comp, JInternalFrame).selected = True
							Catch e2 As PropertyVetoException
							End Try
							CType(comp, JInternalFrame).moveToFront()
							Return
						End If
					Next comp
				End If
			End Sub


			Public Overrides Sub deiconifyFrame(ByVal f As JInternalFrame)
				Dim desktopIcon As JInternalFrame.JDesktopIcon = f.desktopIcon
				Dim c As Container = desktopIcon.parent
				If c IsNot Nothing Then
					c = c.parent
					If c IsNot Nothing Then
						c.add(f)
						If f.maximum Then
							Dim w As Integer = c.width
							Dim h As Integer = c.height - outerInstance.taskBar.height
							If f.width <> w OrElse f.height <> h Then boundsForFrameame(f, 0, 0, w, h)
						End If
						If f.selected Then
							f.moveToFront()
						Else
							Try
								f.selected = True
							Catch e2 As PropertyVetoException
							End Try
						End If
					End If
				End If
			End Sub

			Protected Friend Overrides Sub removeIconFor(ByVal f As JInternalFrame)
				MyBase.removeIconFor(f)
				outerInstance.taskBar.validate()
			End Sub

			Public Overrides Sub setBoundsForFrame(ByVal f As JComponent, ByVal newX As Integer, ByVal newY As Integer, ByVal newWidth As Integer, ByVal newHeight As Integer)
				MyBase.boundsForFrameame(f, newX, newY, newWidth, newHeight)
				If outerInstance.taskBar IsNot Nothing AndAlso newY >= outerInstance.taskBar.y Then f.locationion(f.x, outerInstance.taskBar.y-f.insets.top)
			End Sub
		End Class

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Function getContext(ByVal c As JComponent) As SynthContext
			Return getContext(c, getComponentState(c))
		End Function

		Private Function getContext(ByVal c As JComponent, ByVal state As Integer) As SynthContext
			Return SynthContext.getContext(c, style, state)
		End Function

		Private Function getComponentState(ByVal c As JComponent) As Integer
			Return SynthLookAndFeel.getComponentState(c)
		End Function

		''' <summary>
		''' Notifies this UI delegate to repaint the specified component.
		''' This method paints the component background, then calls
		''' the <seealso cref="#paint(SynthContext,Graphics)"/> method.
		''' 
		''' <p>In general, this method does not need to be overridden by subclasses.
		''' All Look and Feel rendering code should reside in the {@code paint} method.
		''' </summary>
		''' <param name="g"> the {@code Graphics} object used for painting </param>
		''' <param name="c"> the component being painted </param>
		''' <seealso cref= #paint(SynthContext,Graphics) </seealso>
		Public Overrides Sub update(ByVal g As Graphics, ByVal c As JComponent)
			Dim ___context As SynthContext = getContext(c)

			SynthLookAndFeel.update(___context, g)
			___context.painter.paintDesktopPaneBackground(___context, g, 0, 0, c.width, c.height)
			paint(___context, g)
			___context.Dispose()
		End Sub

		''' <summary>
		''' Paints the specified component according to the Look and Feel.
		''' <p>This method is not used by Synth Look and Feel.
		''' Painting is handled by the <seealso cref="#paint(SynthContext,Graphics)"/> method.
		''' </summary>
		''' <param name="g"> the {@code Graphics} object used for painting </param>
		''' <param name="c"> the component being painted </param>
		''' <seealso cref= #paint(SynthContext,Graphics) </seealso>
		Public Overrides Sub paint(ByVal g As Graphics, ByVal c As JComponent)
			Dim ___context As SynthContext = getContext(c)

			paint(___context, g)
			___context.Dispose()
		End Sub

		''' <summary>
		''' Paints the specified component. This implementation does nothing.
		''' </summary>
		''' <param name="context"> context for the component being painted </param>
		''' <param name="g"> the {@code Graphics} object used for painting </param>
		''' <seealso cref= #update(Graphics,JComponent) </seealso>
		Protected Friend Overridable Sub paint(ByVal context As SynthContext, ByVal g As Graphics)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Sub paintBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			context.painter.paintDesktopPaneBorder(context, g, x, y, w, h)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Sub propertyChange(ByVal evt As PropertyChangeEvent)
			If SynthLookAndFeel.shouldUpdateStyle(evt) Then updateStyle(CType(evt.source, JDesktopPane))
			If evt.propertyName = "ancestor" AndAlso taskBar IsNot Nothing Then taskBar.adjustSize()
		End Sub
	End Class

End Namespace
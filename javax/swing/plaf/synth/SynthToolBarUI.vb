Imports System

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
	''' <seealso cref="javax.swing.JToolBar"/>.
	''' 
	''' @since 1.7
	''' </summary>
	Public Class SynthToolBarUI
		Inherits javax.swing.plaf.basic.BasicToolBarUI
		Implements java.beans.PropertyChangeListener, SynthUI

		Private handleIcon As javax.swing.Icon = Nothing
		Private contentRect As New java.awt.Rectangle

		Private style As SynthStyle
		Private contentStyle As SynthStyle
		Private dragWindowStyle As SynthStyle

		''' <summary>
		''' Creates a new UI object for the given component.
		''' </summary>
		''' <param name="c"> component to create UI object for </param>
		''' <returns> the UI object </returns>
		Public Shared Function createUI(ByVal c As javax.swing.JComponent) As javax.swing.plaf.ComponentUI
			Return New SynthToolBarUI
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub installDefaults()
			toolBar.layout = createLayout()
			updateStyle(toolBar)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub installListeners()
			MyBase.installListeners()
			toolBar.addPropertyChangeListener(Me)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub uninstallListeners()
			MyBase.uninstallListeners()
			toolBar.removePropertyChangeListener(Me)
		End Sub

		Private Sub updateStyle(ByVal c As javax.swing.JToolBar)
			Dim ___context As SynthContext = getContext(c, Region.TOOL_BAR_CONTENT, Nothing, ENABLED)
			contentStyle = SynthLookAndFeel.updateStyle(___context, Me)
			___context.Dispose()

			___context = getContext(c, Region.TOOL_BAR_DRAG_WINDOW, Nothing, ENABLED)
			dragWindowStyle = SynthLookAndFeel.updateStyle(___context, Me)
			___context.Dispose()

			___context = getContext(c, ENABLED)
			Dim oldStyle As SynthStyle = style

			style = SynthLookAndFeel.updateStyle(___context, Me)
			If oldStyle IsNot style Then
				handleIcon = style.getIcon(___context, "ToolBar.handleIcon")
				If oldStyle IsNot Nothing Then
					uninstallKeyboardActions()
					installKeyboardActions()
				End If
			End If
			___context.Dispose()
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub uninstallDefaults()
			Dim ___context As SynthContext = getContext(toolBar, ENABLED)

			style.uninstallDefaults(___context)
			___context.Dispose()
			style = Nothing

			handleIcon = Nothing

			___context = getContext(toolBar, Region.TOOL_BAR_CONTENT, contentStyle, ENABLED)
			contentStyle.uninstallDefaults(___context)
			___context.Dispose()
			contentStyle = Nothing

			___context = getContext(toolBar, Region.TOOL_BAR_DRAG_WINDOW, dragWindowStyle, ENABLED)
			dragWindowStyle.uninstallDefaults(___context)
			___context.Dispose()
			dragWindowStyle = Nothing

			toolBar.layout = Nothing
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub installComponents()
		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		End Sub
		Protected Friend Overrides Sub uninstallComponents()
		End Sub

		''' <summary>
		''' Creates a {@code LayoutManager} to use with the toolbar.
		''' </summary>
		''' <returns> a {@code LayoutManager} instance </returns>
		Protected Friend Overridable Function createLayout() As java.awt.LayoutManager
			Return New SynthToolBarLayoutManager(Me)
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Function getContext(ByVal c As javax.swing.JComponent) As SynthContext Implements SynthUI.getContext
			Return getContext(c, SynthLookAndFeel.getComponentState(c))
		End Function

		Private Function getContext(ByVal c As javax.swing.JComponent, ByVal state As Integer) As SynthContext
			Return SynthContext.getContext(c, style, state)
		End Function

		Private Function getContext(ByVal c As javax.swing.JComponent, ByVal ___region As Region, ByVal style As SynthStyle) As SynthContext
			Return SynthContext.getContext(c, ___region, style, getComponentState(c, ___region))
		End Function

		Private Function getContext(ByVal c As javax.swing.JComponent, ByVal ___region As Region, ByVal style As SynthStyle, ByVal state As Integer) As SynthContext
			Return SynthContext.getContext(c, ___region, style, state)
		End Function

		Private Function getComponentState(ByVal c As javax.swing.JComponent, ByVal ___region As Region) As Integer
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
		Public Overrides Sub update(ByVal g As java.awt.Graphics, ByVal c As javax.swing.JComponent)
			Dim ___context As SynthContext = getContext(c)

			SynthLookAndFeel.update(___context, g)
			___context.painter.paintToolBarBackground(___context, g, 0, 0, c.width, c.height, toolBar.orientation)
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
		Public Overrides Sub paint(ByVal g As java.awt.Graphics, ByVal c As javax.swing.JComponent)
			Dim ___context As SynthContext = getContext(c)

			paint(___context, g)
			___context.Dispose()
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Sub paintBorder(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer) Implements SynthUI.paintBorder
			context.painter.paintToolBarBorder(context, g, x, y, w, h, toolBar.orientation)
		End Sub

		''' <summary>
		''' This implementation does nothing, because the {@code rollover}
		''' property of the {@code JToolBar} class is not used
		''' in the Synth Look and Feel.
		''' </summary>
		Protected Friend Overrides Property borderToNonRollover As java.awt.Component
			Set(ByVal c As java.awt.Component)
			''' <summary>
			''' This implementation does nothing, because the {@code rollover}
			''' property of the {@code JToolBar} class is not used
			''' in the Synth Look and Feel.
			''' </summary>
			End Set
		End Property
		Protected Friend Overrides Property borderToRollover As java.awt.Component
			Set(ByVal c As java.awt.Component)
			''' <summary>
			''' This implementation does nothing, because the {@code rollover}
			''' property of the {@code JToolBar} class is not used
			''' in the Synth Look and Feel.
			''' </summary>
			End Set
		End Property
		Protected Friend Overrides Property borderToNormal As java.awt.Component
			Set(ByVal c As java.awt.Component)
			End Set
		End Property

		''' <summary>
		''' Paints the toolbar.
		''' </summary>
		''' <param name="context"> context for the component being painted </param>
		''' <param name="g"> the {@code Graphics} object used for painting </param>
		''' <seealso cref= #update(Graphics,JComponent) </seealso>
		Protected Friend Overridable Sub paint(ByVal context As SynthContext, ByVal g As java.awt.Graphics)
			If handleIcon IsNot Nothing AndAlso toolBar.floatable Then
				Dim startX As Integer = If(toolBar.componentOrientation.leftToRight, 0, toolBar.width - sun.swing.plaf.synth.SynthIcon.getIconWidth(handleIcon, context))
				sun.swing.plaf.synth.SynthIcon.paintIcon(handleIcon, context, g, startX, 0, sun.swing.plaf.synth.SynthIcon.getIconWidth(handleIcon, context), sun.swing.plaf.synth.SynthIcon.getIconHeight(handleIcon, context))
			End If

			Dim subcontext As SynthContext = getContext(toolBar, Region.TOOL_BAR_CONTENT, contentStyle)
			paintContent(subcontext, g, contentRect)
			subcontext.Dispose()
		End Sub

		''' <summary>
		''' Paints the toolbar content.
		''' </summary>
		''' <param name="context"> context for the component being painted </param>
		''' <param name="g"> {@code Graphics} object used for painting </param>
		''' <param name="bounds"> bounding box for the toolbar </param>
		Protected Friend Overridable Sub paintContent(ByVal context As SynthContext, ByVal g As java.awt.Graphics, ByVal bounds As java.awt.Rectangle)
			SynthLookAndFeel.updateSubregion(context, g, bounds)
			context.painter.paintToolBarContentBackground(context, g, bounds.x, bounds.y, bounds.width, bounds.height, toolBar.orientation)
			context.painter.paintToolBarContentBorder(context, g, bounds.x, bounds.y, bounds.width, bounds.height, toolBar.orientation)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub paintDragWindow(ByVal g As java.awt.Graphics)
			Dim w As Integer = dragWindow.width
			Dim h As Integer = dragWindow.height
			Dim ___context As SynthContext = getContext(toolBar, Region.TOOL_BAR_DRAG_WINDOW, dragWindowStyle)
			SynthLookAndFeel.updateSubregion(___context, g, New java.awt.Rectangle(0, 0, w, h))
			___context.painter.paintToolBarDragWindowBackground(___context, g, 0, 0, w, h, dragWindow.orientation)
			___context.painter.paintToolBarDragWindowBorder(___context, g, 0, 0, w, h, dragWindow.orientation)
			___context.Dispose()
		End Sub

		'
		' PropertyChangeListener
		'

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Sub propertyChange(ByVal e As java.beans.PropertyChangeEvent)
			If SynthLookAndFeel.shouldUpdateStyle(e) Then updateStyle(CType(e.source, javax.swing.JToolBar))
		End Sub


		Friend Class SynthToolBarLayoutManager
			Implements java.awt.LayoutManager

			Private ReadOnly outerInstance As SynthToolBarUI

			Public Sub New(ByVal outerInstance As SynthToolBarUI)
				Me.outerInstance = outerInstance
			End Sub

			Public Overridable Sub addLayoutComponent(ByVal name As String, ByVal comp As java.awt.Component)
			End Sub

			Public Overridable Sub removeLayoutComponent(ByVal comp As java.awt.Component)
			End Sub

			Public Overridable Function minimumLayoutSize(ByVal parent As java.awt.Container) As java.awt.Dimension
				Dim tb As javax.swing.JToolBar = CType(parent, javax.swing.JToolBar)
				Dim insets As java.awt.Insets = tb.insets
				Dim [dim] As New java.awt.Dimension
				Dim context As SynthContext = outerInstance.getContext(tb)

				If tb.orientation = javax.swing.JToolBar.HORIZONTAL Then
					[dim].width = If(tb.floatable, sun.swing.plaf.synth.SynthIcon.getIconWidth(outerInstance.handleIcon, context), 0)
					Dim compDim As java.awt.Dimension
					For i As Integer = 0 To tb.componentCount - 1
						Dim component As java.awt.Component = tb.getComponent(i)
						If component.visible Then
							compDim = component.minimumSize
							[dim].width += compDim.width
							[dim].height = Math.Max([dim].height, compDim.height)
						End If
					Next i
				Else
					[dim].height = If(tb.floatable, sun.swing.plaf.synth.SynthIcon.getIconHeight(outerInstance.handleIcon, context), 0)
					Dim compDim As java.awt.Dimension
					For i As Integer = 0 To tb.componentCount - 1
						Dim component As java.awt.Component = tb.getComponent(i)
						If component.visible Then
							compDim = component.minimumSize
							[dim].width = Math.Max([dim].width, compDim.width)
							[dim].height += compDim.height
						End If
					Next i
				End If
				[dim].width += insets.left + insets.right
				[dim].height += insets.top + insets.bottom

				context.Dispose()
				Return [dim]
			End Function

			Public Overridable Function preferredLayoutSize(ByVal parent As java.awt.Container) As java.awt.Dimension
				Dim tb As javax.swing.JToolBar = CType(parent, javax.swing.JToolBar)
				Dim insets As java.awt.Insets = tb.insets
				Dim [dim] As New java.awt.Dimension
				Dim context As SynthContext = outerInstance.getContext(tb)

				If tb.orientation = javax.swing.JToolBar.HORIZONTAL Then
					[dim].width = If(tb.floatable, sun.swing.plaf.synth.SynthIcon.getIconWidth(outerInstance.handleIcon, context), 0)
					Dim compDim As java.awt.Dimension
					For i As Integer = 0 To tb.componentCount - 1
						Dim component As java.awt.Component = tb.getComponent(i)
						If component.visible Then
							compDim = component.preferredSize
							[dim].width += compDim.width
							[dim].height = Math.Max([dim].height, compDim.height)
						End If
					Next i
				Else
					[dim].height = If(tb.floatable, sun.swing.plaf.synth.SynthIcon.getIconHeight(outerInstance.handleIcon, context), 0)
					Dim compDim As java.awt.Dimension
					For i As Integer = 0 To tb.componentCount - 1
						Dim component As java.awt.Component = tb.getComponent(i)
						If component.visible Then
							compDim = component.preferredSize
							[dim].width = Math.Max([dim].width, compDim.width)
							[dim].height += compDim.height
						End If
					Next i
				End If
				[dim].width += insets.left + insets.right
				[dim].height += insets.top + insets.bottom

				context.Dispose()
				Return [dim]
			End Function

			Public Overridable Sub layoutContainer(ByVal parent As java.awt.Container)
				Dim tb As javax.swing.JToolBar = CType(parent, javax.swing.JToolBar)
				Dim insets As java.awt.Insets = tb.insets
				Dim ltr As Boolean = tb.componentOrientation.leftToRight
				Dim context As SynthContext = outerInstance.getContext(tb)

				Dim c As java.awt.Component
				Dim d As java.awt.Dimension

				' JToolBar by default uses a somewhat modified BoxLayout as
				' its layout manager. For compatibility reasons, we want to
				' support Box "glue" as a way to move things around on the
				' toolbar. "glue" is represented in BoxLayout as a Box.Filler
				' with a minimum and preferred size of (0,0).
				' So what we do here is find the number of such glue fillers
				' and figure out how much space should be allocated to them.
				Dim glueCount As Integer = 0
				For i As Integer = 0 To tb.componentCount - 1
					If isGlue(tb.getComponent(i)) Then glueCount += 1
				Next i

				If tb.orientation = javax.swing.JToolBar.HORIZONTAL Then
					Dim handleWidth As Integer = If(tb.floatable, sun.swing.plaf.synth.SynthIcon.getIconWidth(outerInstance.handleIcon, context), 0)

					' Note: contentRect does not take insets into account
					' since it is used for determining the bounds that are
					' passed to paintToolBarContentBackground().
					outerInstance.contentRect.x = If(ltr, handleWidth, 0)
					outerInstance.contentRect.y = 0
					outerInstance.contentRect.width = tb.width - handleWidth
					outerInstance.contentRect.height = tb.height

					' However, we do take the insets into account here for
					' the purposes of laying out the toolbar child components.
					Dim x As Integer = If(ltr, handleWidth + insets.left, tb.width - handleWidth - insets.right)
					Dim baseY As Integer = insets.top
					Dim baseH As Integer = tb.height - insets.top - insets.bottom

					' we need to get the minimum width for laying things out
					' so that we can calculate how much empty space needs to
					' be distributed among the "glue", if any
					Dim extraSpacePerGlue As Integer = 0
					If glueCount > 0 Then
						Dim minWidth As Integer = minimumLayoutSize(parent).width
						extraSpacePerGlue = (tb.width - minWidth) \ glueCount
						If extraSpacePerGlue < 0 Then extraSpacePerGlue = 0
					End If

					For i As Integer = 0 To tb.componentCount - 1
						c = tb.getComponent(i)
						If c.visible Then
							d = c.preferredSize
							Dim y, h As Integer
							If d.height >= baseH OrElse TypeOf c Is javax.swing.JSeparator Then
								' Fill available height
								y = baseY
								h = baseH
							Else
								' Center component vertically in the available space
								y = baseY + (baseH \ 2) - (d.height / 2)
								h = d.height
							End If
							'if the component is a "glue" component then add to its
							'width the extraSpacePerGlue it is due
							If isGlue(c) Then d.width += extraSpacePerGlue
							c.boundsnds(If(ltr, x, x - d.width), y, d.width, h)
							x = If(ltr, x + d.width, x - d.width)
						End If
					Next i
				Else
					Dim handleHeight As Integer = If(tb.floatable, sun.swing.plaf.synth.SynthIcon.getIconHeight(outerInstance.handleIcon, context), 0)

					' See notes above regarding the use of insets
					outerInstance.contentRect.x = 0
					outerInstance.contentRect.y = handleHeight
					outerInstance.contentRect.width = tb.width
					outerInstance.contentRect.height = tb.height - handleHeight

					Dim baseX As Integer = insets.left
					Dim baseW As Integer = tb.width - insets.left - insets.right
					Dim y As Integer = handleHeight + insets.top

					' we need to get the minimum height for laying things out
					' so that we can calculate how much empty space needs to
					' be distributed among the "glue", if any
					Dim extraSpacePerGlue As Integer = 0
					If glueCount > 0 Then
						Dim minHeight As Integer = minimumLayoutSize(parent).height
						extraSpacePerGlue = (tb.height - minHeight) \ glueCount
						If extraSpacePerGlue < 0 Then extraSpacePerGlue = 0
					End If

					For i As Integer = 0 To tb.componentCount - 1
						c = tb.getComponent(i)
						If c.visible Then
							d = c.preferredSize
							Dim x, w As Integer
							If d.width >= baseW OrElse TypeOf c Is javax.swing.JSeparator Then
								' Fill available width
								x = baseX
								w = baseW
							Else
								' Center component horizontally in the available space
								x = baseX + (baseW \ 2) - (d.width / 2)
								w = d.width
							End If
							'if the component is a "glue" component then add to its
							'height the extraSpacePerGlue it is due
							If isGlue(c) Then d.height += extraSpacePerGlue
							c.boundsnds(x, y, w, d.height)
							y += d.height
						End If
					Next i
				End If
				context.Dispose()
			End Sub

			Private Function isGlue(ByVal c As java.awt.Component) As Boolean
				If c.visible AndAlso TypeOf c Is javax.swing.Box.Filler Then
					Dim f As javax.swing.Box.Filler = CType(c, javax.swing.Box.Filler)
					Dim min As java.awt.Dimension = f.minimumSize
					Dim pref As java.awt.Dimension = f.preferredSize
					Return min.width = 0 AndAlso min.height = 0 AndAlso pref.width = 0 AndAlso pref.height = 0
				End If
				Return False
			End Function
		End Class
	End Class

End Namespace
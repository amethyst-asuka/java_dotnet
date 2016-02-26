Imports javax.swing
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
	''' <seealso cref="javax.swing.JInternalFrame"/>.
	''' 
	''' @author David Kloba
	''' @author Joshua Outwater
	''' @author Rich Schiavi
	''' @since 1.7
	''' </summary>
	Public Class SynthInternalFrameUI
		Inherits javax.swing.plaf.basic.BasicInternalFrameUI
		Implements SynthUI, PropertyChangeListener

		Private style As SynthStyle

		''' <summary>
		''' Creates a new UI object for the given component.
		''' </summary>
		''' <param name="b"> component to create UI object for </param>
		''' <returns> the UI object </returns>
		Public Shared Function createUI(ByVal b As JComponent) As ComponentUI
			Return New SynthInternalFrameUI(CType(b, JInternalFrame))
		End Function

		Protected Friend Sub New(ByVal b As JInternalFrame)
			MyBase.New(b)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Sub installDefaults()
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
			frame.layout = internalFrameLayout = createLayoutManager()
			updateStyle(frame)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub installListeners()
			MyBase.installListeners()
			frame.addPropertyChangeListener(Me)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub uninstallComponents()
			If TypeOf frame.componentPopupMenu Is UIResource Then frame.componentPopupMenu = Nothing
			MyBase.uninstallComponents()
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub uninstallListeners()
			frame.removePropertyChangeListener(Me)
			MyBase.uninstallListeners()
		End Sub

		Private Sub updateStyle(ByVal c As JComponent)
			Dim ___context As SynthContext = getContext(c, ENABLED)
			Dim oldStyle As SynthStyle = style

			style = SynthLookAndFeel.updateStyle(___context, Me)
			If style IsNot oldStyle Then
				Dim frameIcon As Icon = frame.frameIcon
				If frameIcon Is Nothing OrElse TypeOf frameIcon Is UIResource Then frame.frameIcon = ___context.style.getIcon(___context, "InternalFrame.icon")
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
			Dim ___context As SynthContext = getContext(frame, ENABLED)
			style.uninstallDefaults(___context)
			___context.Dispose()
			style = Nothing
			If frame.layout Is internalFrameLayout Then frame.layout = Nothing

		End Sub

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
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Function createNorthPane(ByVal w As JInternalFrame) As JComponent
			titlePane = New SynthInternalFrameTitlePane(w)
			titlePane.name = "InternalFrame.northPane"
			Return titlePane
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Function createComponentListener() As ComponentListener
			If UIManager.getBoolean("InternalFrame.useTaskBar") Then
				Return New ComponentHandlerAnonymousInnerClassHelper
			Else
				Return MyBase.createComponentListener()
			End If
		End Function

		Private Class ComponentHandlerAnonymousInnerClassHelper
			Inherits ComponentHandler

			Public Overrides Sub componentResized(ByVal e As ComponentEvent)
				If outerInstance.frame IsNot Nothing AndAlso outerInstance.frame.maximum Then
					Dim desktop As JDesktopPane = CType(e.source, JDesktopPane)
					For Each comp As Component In desktop.components
						If TypeOf comp Is SynthDesktopPaneUI.TaskBar Then
							outerInstance.frame.boundsnds(0, 0, desktop.width, desktop.height - comp.height)
							outerInstance.frame.revalidate()
							Exit For
						End If
					Next comp
				End If

				' Update the new parent bounds for next resize, but don't
				' let the super method touch this frame
				Dim f As JInternalFrame = outerInstance.frame
				outerInstance.frame = Nothing
				MyBase.componentResized(e)
				outerInstance.frame = f
			End Sub
		End Class

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
			___context.painter.paintInternalFrameBackground(___context, g, 0, 0, c.width, c.height)
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
			context.painter.paintInternalFrameBorder(context, g, x, y, w, h)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Sub propertyChange(ByVal evt As PropertyChangeEvent)
			Dim oldStyle As SynthStyle = style
			Dim f As JInternalFrame = CType(evt.source, JInternalFrame)
			Dim prop As String = evt.propertyName

			If SynthLookAndFeel.shouldUpdateStyle(evt) Then updateStyle(f)

			If style Is oldStyle AndAlso (prop = JInternalFrame.IS_MAXIMUM_PROPERTY OrElse prop = JInternalFrame.IS_SELECTED_PROPERTY) Then
				' Border (and other defaults) may need to change
				Dim ___context As SynthContext = getContext(f, ENABLED)
				style.uninstallDefaults(___context)
				style.installDefaults(___context, Me)
			End If
		End Sub
	End Class

End Namespace
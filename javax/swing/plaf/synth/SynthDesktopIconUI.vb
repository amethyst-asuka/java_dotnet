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
	''' Provides the Synth L&amp;F UI delegate for a minimized internal frame on a desktop.
	''' 
	''' @author Joshua Outwater
	''' @since 1.7
	''' </summary>
	Public Class SynthDesktopIconUI
		Inherits javax.swing.plaf.basic.BasicDesktopIconUI
		Implements SynthUI, PropertyChangeListener

		Private style As SynthStyle
		Private handler As New Handler(Me)

		''' <summary>
		''' Creates a new UI object for the given component.
		''' </summary>
		''' <param name="c"> component to create UI object for </param>
		''' <returns> the UI object </returns>
		Public Shared Function createUI(ByVal c As JComponent) As ComponentUI
			Return New SynthDesktopIconUI
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub installComponents()
			If UIManager.getBoolean("InternalFrame.useTaskBar") Then
				iconPane = New JToggleButtonAnonymousInnerClassHelper
				ToolTipManager.sharedInstance().registerComponent(iconPane)
				iconPane.font = desktopIcon.font
				iconPane.background = desktopIcon.background
				iconPane.foreground = desktopIcon.foreground
			Else
				iconPane = New SynthInternalFrameTitlePane(frame)
				iconPane.name = "InternalFrame.northPane"
			End If
			desktopIcon.layout = New BorderLayout
			desktopIcon.add(iconPane, BorderLayout.CENTER)
		End Sub

		Private Class JToggleButtonAnonymousInnerClassHelper
			Inherits JToggleButton

			Public Property Overrides toolTipText As String
				Get
					Return text
				End Get
			End Property

			Public Property Overrides componentPopupMenu As JPopupMenu
				Get
					Return outerInstance.frame.componentPopupMenu
				End Get
			End Property
		End Class

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub installListeners()
			MyBase.installListeners()
			desktopIcon.addPropertyChangeListener(Me)

			If TypeOf iconPane Is JToggleButton Then
				frame.addPropertyChangeListener(Me)
				CType(iconPane, JToggleButton).addActionListener(handler)
			End If
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub uninstallListeners()
			If TypeOf iconPane Is JToggleButton Then
				CType(iconPane, JToggleButton).removeActionListener(handler)
				frame.removePropertyChangeListener(Me)
			End If
			desktopIcon.removePropertyChangeListener(Me)
			MyBase.uninstallListeners()
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub installDefaults()
			updateStyle(desktopIcon)
		End Sub

		Private Sub updateStyle(ByVal c As JComponent)
			Dim ___context As SynthContext = getContext(c, ENABLED)
			style = SynthLookAndFeel.updateStyle(___context, Me)
			___context.Dispose()
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub uninstallDefaults()
			Dim ___context As SynthContext = getContext(desktopIcon, ENABLED)
			style.uninstallDefaults(___context)
			___context.Dispose()
			style = Nothing
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
			___context.painter.paintDesktopIconBackground(___context, g, 0, 0, c.width, c.height)
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
			context.painter.paintDesktopIconBorder(context, g, x, y, w, h)
		End Sub

		Public Overridable Sub propertyChange(ByVal evt As PropertyChangeEvent)
			If TypeOf evt.source Is JInternalFrame.JDesktopIcon Then
				If SynthLookAndFeel.shouldUpdateStyle(evt) Then updateStyle(CType(evt.source, JInternalFrame.JDesktopIcon))
			ElseIf TypeOf evt.source Is JInternalFrame Then
				Dim frame As JInternalFrame = CType(evt.source, JInternalFrame)
				If TypeOf iconPane Is JToggleButton Then
					Dim button As JToggleButton = CType(iconPane, JToggleButton)
					Dim prop As String = evt.propertyName
					If prop = "title" Then
						button.text = CStr(evt.newValue)
					ElseIf prop = "frameIcon" Then
						button.icon = CType(evt.newValue, Icon)
					ElseIf prop = JInternalFrame.IS_ICON_PROPERTY OrElse prop = JInternalFrame.IS_SELECTED_PROPERTY Then
						button.selected = (Not frame.icon) AndAlso frame.selected
					End If
				End If
			End If
		End Sub

		Private NotInheritable Class Handler
			Implements ActionListener

			Private ReadOnly outerInstance As SynthDesktopIconUI

			Public Sub New(ByVal outerInstance As SynthDesktopIconUI)
				Me.outerInstance = outerInstance
			End Sub

			Public Sub actionPerformed(ByVal evt As ActionEvent)
				If TypeOf evt.source Is JToggleButton Then
					' Either iconify the frame or deiconify and activate it.
					Dim button As JToggleButton = CType(evt.source, JToggleButton)
					Try
						Dim selected As Boolean = button.selected
						If (Not selected) AndAlso (Not outerInstance.frame.iconifiable) Then
							button.selected = True
						Else
							outerInstance.frame.icon = (Not selected)
							If selected Then outerInstance.frame.selected = True
						End If
					Catch e2 As PropertyVetoException
					End Try
				End If
			End Sub
		End Class
	End Class

End Namespace
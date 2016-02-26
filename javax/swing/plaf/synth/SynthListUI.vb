Imports javax.swing
Imports javax.swing.border
Imports javax.swing.plaf
Imports javax.swing.plaf.basic

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
	''' <seealso cref="javax.swing.JList"/>.
	''' 
	''' @author Scott Violet
	''' @since 1.7
	''' </summary>
	Public Class SynthListUI
		Inherits BasicListUI
		Implements java.beans.PropertyChangeListener, SynthUI

		Private style As SynthStyle
		Private useListColors As Boolean
		Private useUIBorder As Boolean

		''' <summary>
		''' Creates a new UI object for the given component.
		''' </summary>
		''' <param name="list"> component to create UI object for </param>
		''' <returns> the UI object </returns>
		Public Shared Function createUI(ByVal list As JComponent) As ComponentUI
			Return New SynthListUI
		End Function

		''' <summary>
		''' Notifies this UI delegate to repaint the specified component.
		''' This method paints the component background, then calls
		''' the <seealso cref="#paint"/> method.
		''' 
		''' <p>In general, this method does not need to be overridden by subclasses.
		''' All Look and Feel rendering code should reside in the {@code paint} method.
		''' </summary>
		''' <param name="g"> the {@code Graphics} object used for painting </param>
		''' <param name="c"> the component being painted </param>
		''' <seealso cref= #paint </seealso>
		Public Overrides Sub update(ByVal g As Graphics, ByVal c As JComponent)
			Dim ___context As SynthContext = getContext(c)

			SynthLookAndFeel.update(___context, g)
			___context.painter.paintListBackground(___context, g, 0, 0, c.width, c.height)
			___context.Dispose()
			paint(g, c)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Sub paintBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			context.painter.paintListBorder(context, g, x, y, w, h)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub installListeners()
			MyBase.installListeners()
			list.addPropertyChangeListener(Me)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Sub propertyChange(ByVal e As java.beans.PropertyChangeEvent)
			If SynthLookAndFeel.shouldUpdateStyle(e) Then updateStyle(CType(e.source, JList))
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub uninstallListeners()
			MyBase.uninstallListeners()
			list.removePropertyChangeListener(Me)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub installDefaults()
			If list.cellRenderer Is Nothing OrElse (TypeOf list.cellRenderer Is UIResource) Then list.cellRenderer = New SynthListCellRenderer(Me)
			updateStyle(list)
		End Sub

		Private Sub updateStyle(ByVal c As JComponent)
			Dim ___context As SynthContext = getContext(list, ENABLED)
			Dim oldStyle As SynthStyle = style

			style = SynthLookAndFeel.updateStyle(___context, Me)

			If style IsNot oldStyle Then
				___context.componentState = SELECTED
				Dim sbg As Color = list.selectionBackground
				If sbg Is Nothing OrElse TypeOf sbg Is UIResource Then list.selectionBackground = style.getColor(___context, ColorType.TEXT_BACKGROUND)

				Dim sfg As Color = list.selectionForeground
				If sfg Is Nothing OrElse TypeOf sfg Is UIResource Then list.selectionForeground = style.getColor(___context, ColorType.TEXT_FOREGROUND)

				useListColors = style.getBoolean(___context, "List.rendererUseListColors", True)
				useUIBorder = style.getBoolean(___context, "List.rendererUseUIBorder", True)

				Dim height As Integer = style.getInt(___context, "List.cellHeight", -1)
				If height <> -1 Then list.fixedCellHeight = height
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
			MyBase.uninstallDefaults()

			Dim ___context As SynthContext = getContext(list, ENABLED)

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


		Private Class SynthListCellRenderer
			Inherits DefaultListCellRenderer.UIResource

			Private ReadOnly outerInstance As SynthListUI

			Public Sub New(ByVal outerInstance As SynthListUI)
				Me.outerInstance = outerInstance
			End Sub

			Public Property Overrides name As String
				Get
					Return "List.cellRenderer"
				End Get
			End Property

			Public Overrides Property border As Border
				Set(ByVal b As Border)
					If outerInstance.useUIBorder OrElse TypeOf b Is SynthBorder Then MyBase.border = b
				End Set
			End Property

			Public Overrides Function getListCellRendererComponent(ByVal list As JList, ByVal value As Object, ByVal index As Integer, ByVal isSelected As Boolean, ByVal cellHasFocus As Boolean) As Component
				If (Not outerInstance.useListColors) AndAlso (isSelected OrElse cellHasFocus) Then
					SynthLookAndFeel.selectedUIdUI(CType(SynthLookAndFeel.getUIOfType(uI, GetType(SynthLabelUI)), SynthLabelUI), isSelected, cellHasFocus, list.enabled, False)
				Else
					SynthLookAndFeel.resetSelectedUI()
				End If

				MyBase.getListCellRendererComponent(list, value, index, isSelected, cellHasFocus)
				Return Me
			End Function

			Public Overrides Sub paint(ByVal g As Graphics)
				MyBase.paint(g)
				SynthLookAndFeel.resetSelectedUI()
			End Sub
		End Class
	End Class

End Namespace
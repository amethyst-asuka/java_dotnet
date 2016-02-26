Imports javax.swing
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
	''' <seealso cref="javax.swing.JMenu"/>.
	''' 
	''' @author Georges Saab
	''' @author David Karlton
	''' @author Arnaud Weber
	''' @since 1.7
	''' </summary>
	Public Class SynthMenuUI
		Inherits BasicMenuUI
		Implements PropertyChangeListener, SynthUI

		Private style As SynthStyle
		Private accStyle As SynthStyle

		''' <summary>
		''' Creates a new UI object for the given component.
		''' </summary>
		''' <param name="x"> component to create UI object for </param>
		''' <returns> the UI object </returns>
		Public Shared Function createUI(ByVal x As JComponent) As ComponentUI
			Return New SynthMenuUI
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub installDefaults()
			updateStyle(menuItem)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub installListeners()
			MyBase.installListeners()
			menuItem.addPropertyChangeListener(Me)
		End Sub

		Private Sub updateStyle(ByVal mi As JMenuItem)
			Dim oldStyle As SynthStyle = style
			Dim ___context As SynthContext = getContext(mi, ENABLED)

			style = SynthLookAndFeel.updateStyle(___context, Me)
			If oldStyle IsNot style Then
				Dim prefix As String = propertyPrefix
				defaultTextIconGap = style.getInt(___context, prefix & ".textIconGap", 4)
				If menuItem.margin Is Nothing OrElse (TypeOf menuItem.margin Is UIResource) Then
					Dim insets As Insets = CType(style.get(___context, prefix & ".margin"), Insets)

					If insets Is Nothing Then insets = SynthLookAndFeel.EMPTY_UIRESOURCE_INSETS
					menuItem.margin = insets
				End If
				acceleratorDelimiter = style.getString(___context, prefix & ".acceleratorDelimiter", "+")

				If sun.swing.MenuItemLayoutHelper.useCheckAndArrow(menuItem) Then
					checkIcon = style.getIcon(___context, prefix & ".checkIcon")
					arrowIcon = style.getIcon(___context, prefix & ".arrowIcon")
				Else
					' Not needed in this case
					checkIcon = Nothing
					arrowIcon = Nothing
				End If

				CType(menuItem, JMenu).delay = style.getInt(___context, prefix & ".delay", 200)
				If oldStyle IsNot Nothing Then
					uninstallKeyboardActions()
					installKeyboardActions()
				End If
			End If
			___context.Dispose()

			Dim accContext As SynthContext = getContext(mi, Region.MENU_ITEM_ACCELERATOR, ENABLED)

			accStyle = SynthLookAndFeel.updateStyle(accContext, Me)
			accContext.Dispose()
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Sub uninstallUI(ByVal c As JComponent)
			MyBase.uninstallUI(c)
			' Remove values from the parent's Client Properties.
			Dim p As JComponent = sun.swing.MenuItemLayoutHelper.getMenuItemParent(CType(c, JMenuItem))
			If p IsNot Nothing Then p.putClientProperty(SynthMenuItemLayoutHelper.MAX_ACC_OR_ARROW_WIDTH, Nothing)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub uninstallDefaults()
			Dim ___context As SynthContext = getContext(menuItem, ENABLED)
			style.uninstallDefaults(___context)
			___context.Dispose()
			style = Nothing

			Dim accContext As SynthContext = getContext(menuItem, Region.MENU_ITEM_ACCELERATOR, ENABLED)
			accStyle.uninstallDefaults(accContext)
			accContext.Dispose()
			accStyle = Nothing

			MyBase.uninstallDefaults()
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Sub uninstallListeners()
			MyBase.uninstallListeners()
			menuItem.removePropertyChangeListener(Me)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Function getContext(ByVal c As JComponent) As SynthContext
			Return getContext(c, getComponentState(c))
		End Function

		Friend Overridable Function getContext(ByVal c As JComponent, ByVal state As Integer) As SynthContext
			Return SynthContext.getContext(c, style, state)
		End Function

		Friend Overridable Function getContext(ByVal c As JComponent, ByVal ___region As Region) As SynthContext
			Return getContext(c, ___region, getComponentState(c, ___region))
		End Function

		Private Function getContext(ByVal c As JComponent, ByVal ___region As Region, ByVal state As Integer) As SynthContext
			Return SynthContext.getContext(c, ___region, accStyle, state)
		End Function

		Private Function getComponentState(ByVal c As JComponent) As Integer
			Dim state As Integer

			If Not c.enabled Then Return DISABLED
			If menuItem.armed Then
				state = MOUSE_OVER
			Else
				state = SynthLookAndFeel.getComponentState(c)
			End If
			If menuItem.selected Then state = state Or SELECTED
			Return state
		End Function

		Private Function getComponentState(ByVal c As JComponent, ByVal ___region As Region) As Integer
			Return getComponentState(c)
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides Function getPreferredMenuItemSize(ByVal c As JComponent, ByVal checkIcon As Icon, ByVal arrowIcon As Icon, ByVal defaultTextIconGap As Integer) As Dimension
			Dim ___context As SynthContext = getContext(c)
			Dim accContext As SynthContext = getContext(c, Region.MENU_ITEM_ACCELERATOR)
			Dim value As Dimension = SynthGraphicsUtils.getPreferredMenuItemSize(___context, accContext, c, checkIcon, arrowIcon, defaultTextIconGap, acceleratorDelimiter, sun.swing.MenuItemLayoutHelper.useCheckAndArrow(menuItem), propertyPrefix)
			___context.Dispose()
			accContext.Dispose()
			Return value
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
			___context.painter.paintMenuBackground(___context, g, 0, 0, c.width, c.height)
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
			Dim accContext As SynthContext = getContext(menuItem, Region.MENU_ITEM_ACCELERATOR)
			' Refetch the appropriate check indicator for the current state
			Dim prefix As String = propertyPrefix
			Dim checkIcon As Icon = style.getIcon(context, prefix & ".checkIcon")
			Dim arrowIcon As Icon = style.getIcon(context, prefix & ".arrowIcon")
			SynthGraphicsUtils.paint(context, accContext, g, checkIcon, arrowIcon, acceleratorDelimiter, defaultTextIconGap, propertyPrefix)
			accContext.Dispose()
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Sub paintBorder(ByVal context As SynthContext, ByVal g As Graphics, ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer)
			context.painter.paintMenuBorder(context, g, x, y, w, h)
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Sub propertyChange(ByVal e As PropertyChangeEvent)
			If SynthLookAndFeel.shouldUpdateStyle(e) OrElse (e.propertyName.Equals("ancestor") AndAlso UIManager.getBoolean("Menu.useMenuBarForTopLevelMenus")) Then updateStyle(CType(e.source, JMenu))
		End Sub
	End Class

End Namespace
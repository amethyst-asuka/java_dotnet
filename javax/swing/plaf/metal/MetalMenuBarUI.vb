Imports javax.swing
Imports javax.swing.plaf.basic

'
' * Copyright (c) 2003, 2005, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.swing.plaf.metal

	''' <summary>
	''' Metal implementation of <code>MenuBarUI</code>. This class is responsible
	''' for providing the metal look and feel for <code>JMenuBar</code>s.
	''' </summary>
	''' <seealso cref= javax.swing.plaf.MenuBarUI
	''' @since 1.5 </seealso>
	Public Class MetalMenuBarUI
		Inherits BasicMenuBarUI

		''' <summary>
		''' Creates the <code>ComponentUI</code> implementation for the passed
		''' in component.
		''' </summary>
		''' <param name="x"> JComponent to create the ComponentUI implementation for </param>
		''' <returns> ComponentUI implementation for <code>x</code> </returns>
		''' <exception cref="NullPointerException"> if <code>x</code> is null </exception>
		Public Shared Function createUI(ByVal x As JComponent) As javax.swing.plaf.ComponentUI
			If x Is Nothing Then Throw New NullPointerException("Must pass in a non-null component")
			Return New MetalMenuBarUI
		End Function

		''' <summary>
		''' Configures the specified component appropriate for the metal look and
		''' feel.
		''' </summary>
		''' <param name="c"> the component where this UI delegate is being installed </param>
		''' <exception cref="NullPointerException"> if <code>c</code> is null. </exception>
		Public Overrides Sub installUI(ByVal c As JComponent)
			MyBase.installUI(c)
			MetalToolBarUI.register(c)
		End Sub

		''' <summary>
		''' Reverses configuration which was done on the specified component during
		''' <code>installUI</code>.
		''' </summary>
		''' <param name="c"> the component where this UI delegate is being installed </param>
		''' <exception cref="NullPointerException"> if <code>c</code> is null. </exception>
		Public Overrides Sub uninstallUI(ByVal c As JComponent)
			MyBase.uninstallUI(c)
			MetalToolBarUI.unregister(c)
		End Sub

		''' <summary>
		''' If necessary paints the background of the component, then
		''' invokes <code>paint</code>.
		''' </summary>
		''' <param name="g"> Graphics to paint to </param>
		''' <param name="c"> JComponent painting on </param>
		''' <exception cref="NullPointerException"> if <code>g</code> or <code>c</code> is
		'''         null </exception>
		''' <seealso cref= javax.swing.plaf.ComponentUI#update </seealso>
		''' <seealso cref= javax.swing.plaf.ComponentUI#paint
		''' @since 1.5 </seealso>
		Public Overridable Sub update(ByVal g As Graphics, ByVal c As JComponent)
			Dim isOpaque As Boolean = c.opaque
			If g Is Nothing Then Throw New NullPointerException("Graphics must be non-null")
			If isOpaque AndAlso (TypeOf c.background Is javax.swing.plaf.UIResource) AndAlso UIManager.get("MenuBar.gradient") IsNot Nothing Then
				If MetalToolBarUI.doesMenuBarBorderToolBar(CType(c, JMenuBar)) Then
					Dim tb As JToolBar = CType(MetalToolBarUI.findRegisteredComponentOfType(c, GetType(JToolBar)), JToolBar)
					If tb.opaque AndAlso TypeOf tb.background Is javax.swing.plaf.UIResource Then
						MetalUtils.drawGradient(c, g, "MenuBar.gradient", 0, 0, c.width, c.height + tb.height, True)
						paint(g, c)
						Return
					End If
				End If
				MetalUtils.drawGradient(c, g, "MenuBar.gradient", 0, 0, c.width, c.height,True)
				paint(g, c)
			Else
				MyBase.update(g, c)
			End If
		End Sub
	End Class

End Namespace
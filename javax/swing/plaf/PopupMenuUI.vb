'
' * Copyright (c) 1997, 2000, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.swing.plaf


	''' <summary>
	''' Pluggable look and feel interface for JPopupMenu.
	''' 
	''' @author Georges Saab
	''' @author David Karlton
	''' </summary>

	Public MustInherit Class PopupMenuUI
		Inherits ComponentUI

		''' <summary>
		''' @since 1.3
		''' </summary>
		Public Overridable Function isPopupTrigger(ByVal e As java.awt.event.MouseEvent) As Boolean
			Return e.popupTrigger
		End Function

		''' <summary>
		''' Returns the <code>Popup</code> that will be responsible for
		''' displaying the <code>JPopupMenu</code>.
		''' </summary>
		''' <param name="popup"> JPopupMenu requesting Popup </param>
		''' <param name="x">     Screen x location Popup is to be shown at </param>
		''' <param name="y">     Screen y location Popup is to be shown at. </param>
		''' <returns> Popup that will show the JPopupMenu
		''' @since 1.4 </returns>
		Public Overridable Function getPopup(ByVal popup As javax.swing.JPopupMenu, ByVal x As Integer, ByVal y As Integer) As javax.swing.Popup
			Dim ___popupFactory As javax.swing.PopupFactory = javax.swing.PopupFactory.sharedInstance

			Return ___popupFactory.getPopup(popup.invoker, popup, x, y)
		End Function
	End Class

End Namespace
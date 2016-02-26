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

Namespace javax.swing.plaf


	''' <summary>
	''' Pluggable look and feel interface for JComboBox.
	''' 
	''' @author Arnaud Weber
	''' @author Tom Santos
	''' </summary>
	Public MustInherit Class ComboBoxUI
		Inherits ComponentUI

		''' <summary>
		''' Set the visibility of the popup
		''' </summary>
		Public MustOverride Sub setPopupVisible(ByVal c As javax.swing.JComboBox, ByVal v As Boolean)

		''' <summary>
		''' Determine the visibility of the popup
		''' </summary>
		Public MustOverride Function isPopupVisible(ByVal c As javax.swing.JComboBox) As Boolean

		''' <summary>
		''' Determine whether or not the combo box itself is traversable
		''' </summary>
		Public MustOverride Function isFocusTraversable(ByVal c As javax.swing.JComboBox) As Boolean
	End Class

End Namespace
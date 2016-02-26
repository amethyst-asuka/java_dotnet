'
' * Copyright (c) 1997, 1998, Oracle and/or its affiliates. All rights reserved.
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
	''' Pluggable look and feel interface for JOptionPane.
	''' 
	''' @author Scott Violet
	''' </summary>

	Public MustInherit Class OptionPaneUI
		Inherits ComponentUI

		''' <summary>
		''' Requests the component representing the default value to have
		''' focus.
		''' </summary>
		Public MustOverride Sub selectInitialValue(ByVal op As javax.swing.JOptionPane)

		''' <summary>
		''' Returns true if the user has supplied instances of Component for
		''' either the options or message.
		''' </summary>
		Public MustOverride Function containsCustomComponents(ByVal op As javax.swing.JOptionPane) As Boolean
	End Class

End Namespace
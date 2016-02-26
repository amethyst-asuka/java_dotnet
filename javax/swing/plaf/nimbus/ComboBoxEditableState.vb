Imports javax.swing

'
' * Copyright (c) 2005, 2006, Oracle and/or its affiliates. All rights reserved.
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
Namespace javax.swing.plaf.nimbus


	Friend Class ComboBoxEditableState
		Inherits State

		Friend Sub New()
			MyBase.New("Editable")
		End Sub

		Protected Friend Overrides Function isInState(ByVal c As JComponent) As Boolean

			Return TypeOf c Is JComboBox AndAlso CType(c, JComboBox).editable

		End Function
	End Class


End Namespace
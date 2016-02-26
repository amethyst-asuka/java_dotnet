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


	Friend Class ComboBoxArrowButtonEditableState
		Inherits State

		Friend Sub New()
			MyBase.New("Editable")
		End Sub

		Protected Friend Overrides Function isInState(ByVal c As JComponent) As Boolean

									Dim parent As Component = c.parent
									Return TypeOf parent Is JComboBox AndAlso CType(parent, JComboBox).editable
		End Function
	End Class


End Namespace
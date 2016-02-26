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


	Friend Class InternalFrameWindowFocusedState
		Inherits State

		Friend Sub New()
			MyBase.New("WindowFocused")
		End Sub

		Protected Friend Overrides Function isInState(ByVal c As JComponent) As Boolean

							 Return TypeOf c Is JInternalFrame AndAlso CType(c, JInternalFrame).selected
		End Function
	End Class


End Namespace
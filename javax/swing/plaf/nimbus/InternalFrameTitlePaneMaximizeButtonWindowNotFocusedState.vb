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


	Friend Class InternalFrameTitlePaneMaximizeButtonWindowNotFocusedState
		Inherits State

		Friend Sub New()
			MyBase.New("WindowNotFocused")
		End Sub

		Protected Friend Overrides Function isInState(ByVal c As JComponent) As Boolean

								   Dim parent As Component = c
								   Do While parent.parent IsNot Nothing
									   If TypeOf parent Is JInternalFrame Then Exit Do
									   parent = parent.parent
								   Loop
								   If TypeOf parent Is JInternalFrame Then Return Not(CType(parent, JInternalFrame).selected)
								   Return False
		End Function
	End Class


End Namespace
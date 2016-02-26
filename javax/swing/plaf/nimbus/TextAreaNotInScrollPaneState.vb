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


	Friend Class TextAreaNotInScrollPaneState
		Inherits State

		Friend Sub New()
			MyBase.New("NotInScrollPane")
		End Sub

		Protected Friend Overrides Function isInState(ByVal c As JComponent) As Boolean

							  Return Not(TypeOf c.parent Is javax.swing.JViewport)
		End Function
	End Class


End Namespace
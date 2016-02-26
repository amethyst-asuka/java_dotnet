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


	Friend Class TableHeaderRendererSortedState
		Inherits State

		Friend Sub New()
			MyBase.New("Sorted")
		End Sub

		Protected Friend Overrides Function isInState(ByVal c As JComponent) As Boolean

						Dim sortOrder As String = CStr(c.getClientProperty("Table.sortOrder"))
						Return sortOrder IsNot Nothing AndAlso ("ASCENDING".Equals(sortOrder) OrElse "DESCENDING".Equals(sortOrder))
		End Function
	End Class


End Namespace
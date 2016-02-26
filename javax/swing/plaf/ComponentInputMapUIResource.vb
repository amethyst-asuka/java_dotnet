'
' * Copyright (c) 1999, 2006, Oracle and/or its affiliates. All rights reserved.
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
	''' A subclass of javax.swing.ComponentInputMap that implements UIResource.
	''' UI classes which provide a ComponentInputMap should use this class.
	''' 
	''' @author Scott Violet
	''' @since 1.3
	''' </summary>
	Public Class ComponentInputMapUIResource
		Inherits javax.swing.ComponentInputMap
		Implements UIResource

		Public Sub New(ByVal component As javax.swing.JComponent)
			MyBase.New(component)
		End Sub
	End Class

End Namespace
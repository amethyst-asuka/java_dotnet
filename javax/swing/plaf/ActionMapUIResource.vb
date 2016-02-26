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
	''' A subclass of javax.swing.ActionMap that implements UIResource.
	''' UI classes which provide an ActionMap should use this class.
	''' 
	''' @author Scott Violet
	''' @since 1.3
	''' </summary>
	Public Class ActionMapUIResource
		Inherits javax.swing.ActionMap
		Implements UIResource

		Public Sub New()
		End Sub
	End Class

End Namespace
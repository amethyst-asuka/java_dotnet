'
' * Copyright (c) 1996, 2008, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.lang

	''' <summary>
	''' Signals that the class doesn't have a field of a specified name.
	''' 
	''' @author  unascribed
	''' @since   JDK1.1
	''' </summary>
	Public Class NoSuchFieldException
		Inherits ReflectiveOperationException

		Private Shadows Const serialVersionUID As Long = -6143714805279938260L

		''' <summary>
		''' Constructor.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructor with a detail message.
		''' </summary>
		''' <param name="s"> the detail message </param>
		Public Sub New(ByVal s As String)
			MyBase.New(s)
		End Sub
	End Class

End Namespace
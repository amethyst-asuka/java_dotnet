'
' * Copyright (c) 1995, 2008, Oracle and/or its affiliates. All rights reserved.
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
	''' Thrown when the Java Virtual Machine detects a circularity in the
	''' superclass hierarchy of a class being loaded.
	''' 
	''' @author     unascribed
	''' @since      JDK1.0
	''' </summary>
	Public Class ClassCircularityError
		Inherits LinkageError

		Private Shadows Const serialVersionUID As Long = 1054362542914539689L

		''' <summary>
		''' Constructs a {@code ClassCircularityError} with no detail message.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Constructs a {@code ClassCircularityError} with the specified detail
		''' message.
		''' </summary>
		''' <param name="s">
		'''         The detail message </param>
		Public Sub New(ByVal s As String)
			MyBase.New(s)
		End Sub
	End Class

End Namespace
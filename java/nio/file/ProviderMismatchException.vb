'
' * Copyright (c) 2007, 2009, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.nio.file

	''' <summary>
	''' Unchecked exception thrown when an attempt is made to invoke a method on an
	''' object created by one file system provider with a parameter created by a
	''' different file system provider.
	''' </summary>
	Public Class ProviderMismatchException
		Inherits System.ArgumentException

		Friend Shadows Const serialVersionUID As Long = 4990847485741612530L

		''' <summary>
		''' Constructs an instance of this class.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Constructs an instance of this class.
		''' </summary>
		''' <param name="msg">
		'''          the detail message </param>
		Public Sub New(  msg As String)
			MyBase.New(msg)
		End Sub
	End Class

End Namespace
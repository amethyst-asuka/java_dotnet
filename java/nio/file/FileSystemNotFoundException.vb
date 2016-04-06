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
	''' Runtime exception thrown when a file system cannot be found.
	''' </summary>

	Public Class FileSystemNotFoundException
		Inherits RuntimeException

		Friend Shadows Const serialVersionUID As Long = 7999581764446402397L

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
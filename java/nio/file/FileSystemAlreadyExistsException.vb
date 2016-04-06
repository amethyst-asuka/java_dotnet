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
	''' Runtime exception thrown when an attempt is made to create a file system that
	''' already exists.
	''' </summary>

	Public Class FileSystemAlreadyExistsException
		Inherits RuntimeException

		Friend Shadows Const serialVersionUID As Long = -5438419127181131148L

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
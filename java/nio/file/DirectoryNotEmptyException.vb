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
	''' Checked exception thrown when a file system operation fails because a
	''' directory is not empty.
	''' 
	''' @since 1.7
	''' </summary>

	Public Class DirectoryNotEmptyException
		Inherits FileSystemException

		Friend Shadows Const serialVersionUID As Long = 3056667871802779003L

		''' <summary>
		''' Constructs an instance of this class.
		''' </summary>
		''' <param name="dir">
		'''          a string identifying the directory or {@code null} if not known </param>
		Public Sub New(ByVal dir As String)
			MyBase.New(dir)
		End Sub
	End Class

End Namespace
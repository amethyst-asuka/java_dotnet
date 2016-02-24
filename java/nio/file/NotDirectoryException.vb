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
	''' Checked exception thrown when a file system operation, intended for a
	''' directory, fails because the file is not a directory.
	''' 
	''' @since 1.7
	''' </summary>

	Public Class NotDirectoryException
		Inherits FileSystemException

		Private Shadows Const serialVersionUID As Long = -9011457427178200199L

		''' <summary>
		''' Constructs an instance of this class.
		''' </summary>
		''' <param name="file">
		'''          a string identifying the file or {@code null} if not known </param>
		Public Sub New(ByVal file As String)
			MyBase.New(file)
		End Sub
	End Class

End Namespace
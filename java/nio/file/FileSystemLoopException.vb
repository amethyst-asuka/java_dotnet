'
' * Copyright (c) 2010, Oracle and/or its affiliates. All rights reserved.
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
	''' Checked exception thrown when a file system loop, or cycle, is encountered.
	''' 
	''' @since 1.7 </summary>
	''' <seealso cref= Files#walkFileTree </seealso>

	Public Class FileSystemLoopException
		Inherits FileSystemException

		Private Shadows Const serialVersionUID As Long = 4843039591949217617L

		''' <summary>
		''' Constructs an instance of this class.
		''' </summary>
		''' <param name="file">
		'''          a string identifying the file causing the cycle or {@code null} if
		'''          not known </param>
		Public Sub New(ByVal file As String)
			MyBase.New(file)
		End Sub
	End Class

End Namespace
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
	''' Checked exception thrown when a file system operation fails because a file
	''' is not a symbolic link.
	''' 
	''' @since 1.7
	''' </summary>

	Public Class NotLinkException
		Inherits FileSystemException

		Friend Shadows Const serialVersionUID As Long = -388655596416518021L

		''' <summary>
		''' Constructs an instance of this class.
		''' </summary>
		''' <param name="file">
		'''          a string identifying the file or {@code null} if not known </param>
		Public Sub New(ByVal file As String)
			MyBase.New(file)
		End Sub

		''' <summary>
		''' Constructs an instance of this class.
		''' </summary>
		''' <param name="file">
		'''          a string identifying the file or {@code null} if not known </param>
		''' <param name="other">
		'''          a string identifying the other file or {@code null} if not known </param>
		''' <param name="reason">
		'''          a reason message with additional information or {@code null} </param>
		Public Sub New(ByVal file As String, ByVal other As String, ByVal reason As String)
			MyBase.New(file, other, reason)
		End Sub
	End Class

End Namespace
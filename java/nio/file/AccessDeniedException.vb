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
	''' Checked exception thrown when a file system operation is denied, typically
	''' due to a file permission or other access check.
	''' 
	''' <p> This exception is not related to the {@link
	''' java.security.AccessControlException AccessControlException} or {@link
	''' SecurityException} thrown by access controllers or security managers when
	''' access to a file is denied.
	''' 
	''' @since 1.7
	''' </summary>

	Public Class AccessDeniedException
		Inherits FileSystemException

		Private Shadows Const serialVersionUID As Long = 4943049599949219617L

		''' <summary>
		''' Constructs an instance of this class.
		''' </summary>
		''' <param name="file">
		'''          a string identifying the file or {@code null} if not known </param>
		Public Sub New(  file As String)
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
		Public Sub New(  file As String,   other As String,   reason As String)
			MyBase.New(file, other, reason)
		End Sub
	End Class

End Namespace
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
	''' Checked exception thrown when a file cannot be moved as an atomic file system
	''' operation.
	''' 
	''' @since 1.7
	''' </summary>

	Public Class AtomicMoveNotSupportedException
		Inherits FileSystemException

		Friend Shadows Const serialVersionUID As Long = 5402760225333135579L

		''' <summary>
		''' Constructs an instance of this class.
		''' </summary>
		''' <param name="source">
		'''          a string identifying the source file or {@code null} if not known </param>
		''' <param name="target">
		'''          a string identifying the target file or {@code null} if not known </param>
		''' <param name="reason">
		'''          a reason message with additional information </param>
		Public Sub New(  source As String,   target As String,   reason As String)
			MyBase.New(source, target, reason)
		End Sub
	End Class

End Namespace
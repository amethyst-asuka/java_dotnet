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
	''' Unchecked exception thrown when an attempt is made to invoke an operation on
	''' a file and the file system is closed.
	''' </summary>

	Public Class ClosedFileSystemException
		Inherits IllegalStateException

		Friend Shadows Const serialVersionUID As Long = -8158336077256193488L

		''' <summary>
		''' Constructs an instance of this class.
		''' </summary>
		Public Sub New()
		End Sub
	End Class

End Namespace
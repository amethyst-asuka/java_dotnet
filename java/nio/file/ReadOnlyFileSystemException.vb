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
	''' Unchecked exception thrown when an attempt is made to update an object
	''' associated with a <seealso cref="FileSystem#isReadOnly() read-only"/> {@code FileSystem}.
	''' </summary>

	Public Class ReadOnlyFileSystemException
		Inherits UnsupportedOperationException

		Friend Shadows Const serialVersionUID As Long = -6822409595617487197L

		''' <summary>
		''' Constructs an instance of this class.
		''' </summary>
		Public Sub New()
		End Sub
	End Class

End Namespace
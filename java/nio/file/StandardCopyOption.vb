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
	''' Defines the standard copy options.
	''' 
	''' @since 1.7
	''' </summary>

'JAVA TO VB CONVERTER TODO TASK: Enums cannot implement interfaces in .NET:
	Public Enum StandardCopyOption
		''' <summary>
		''' Replace an existing file if it exists.
		''' </summary>
		REPLACE_EXISTING
		''' <summary>
		''' Copy attributes to the new file.
		''' </summary>
		COPY_ATTRIBUTES
		''' <summary>
		''' Move the file as an atomic file system operation.
		''' </summary>
		ATOMIC_MOVE
	End Enum

End Namespace
'
' * Copyright (c) 2007, 2011, Oracle and/or its affiliates. All rights reserved.
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
	''' Defines the options as to how symbolic links are handled.
	''' 
	''' @since 1.7
	''' </summary>

'JAVA TO VB CONVERTER TODO TASK: Enums cannot implement interfaces in .NET:
	Public Enum LinkOption
		''' <summary>
		''' Do not follow symbolic links.
		''' </summary>
		''' <seealso cref= Files#getFileAttributeView(Path,Class,LinkOption[]) </seealso>
		''' <seealso cref= Files#copy </seealso>
		''' <seealso cref= SecureDirectoryStream#newByteChannel </seealso>
		NOFOLLOW_LINKS
	End Enum

End Namespace
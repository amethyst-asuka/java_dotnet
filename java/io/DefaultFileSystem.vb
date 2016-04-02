'
' * Copyright (c) 2012, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.io

	''' 
	''' <summary>
	''' @since 1.8
	''' </summary>
	Friend Class DefaultFileSystem

		''' <summary>
		''' Return the FileSystem object for Windows platform.
		''' </summary>
		PublicShared ReadOnly PropertyfileSystem As FileSystem
			Get
				Return New WinNTFileSystem
			End Get
		End Property
	End Class

End Namespace
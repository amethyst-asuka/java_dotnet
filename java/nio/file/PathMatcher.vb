'
' * Copyright (c) 2007, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' An interface that is implemented by objects that perform match operations on
	''' paths.
	''' 
	''' @since 1.7
	''' </summary>
	''' <seealso cref= FileSystem#getPathMatcher </seealso>
	''' <seealso cref= Files#newDirectoryStream(Path,String) </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	Public Interface PathMatcher
		''' <summary>
		''' Tells if given path matches this matcher's pattern.
		''' </summary>
		''' <param name="path">
		'''          the path to match
		''' </param>
		''' <returns>  {@code true} if, and only if, the path matches this
		'''          matcher's pattern </returns>
		Function matches(ByVal path As Path) As Boolean
	End Interface

End Namespace
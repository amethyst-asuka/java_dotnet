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
	''' The result type of a <seealso cref="FileVisitor FileVisitor"/>.
	''' 
	''' @since 1.7
	''' </summary>
	''' <seealso cref= Files#walkFileTree </seealso>

	Public Enum FileVisitResult
		''' <summary>
		''' Continue. When returned from a {@link FileVisitor#preVisitDirectory
		''' preVisitDirectory} method then the entries in the directory should also
		''' be visited.
		''' </summary>
		[CONTINUE]
		''' <summary>
		''' Terminate.
		''' </summary>
		TERMINATE
		''' <summary>
		''' Continue without visiting the entries in this directory. This result
		''' is only meaningful when returned from the {@link
		''' FileVisitor#preVisitDirectory preVisitDirectory} method; otherwise
		''' this result type is the same as returning <seealso cref="#CONTINUE"/>.
		''' </summary>
		SKIP_SUBTREE
		''' <summary>
		''' Continue without visiting the <em>siblings</em> of this file or directory.
		''' If returned from the {@link FileVisitor#preVisitDirectory
		''' preVisitDirectory} method then the entries in the directory are also
		''' skipped and the <seealso cref="FileVisitor#postVisitDirectory postVisitDirectory"/>
		''' method is not invoked.
		''' </summary>
		SKIP_SIBLINGS
	End Enum

End Namespace
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

Namespace java.nio.file.attribute

	''' <summary>
	''' An attribute view that is a read-only or updatable view of non-opaque
	''' values associated with a file in a filesystem. This interface is extended or
	''' implemented by specific file attribute views that define methods to read
	''' and/or update the attributes of a file.
	''' 
	''' @since 1.7
	''' </summary>
	''' <seealso cref= java.nio.file.Files#getFileAttributeView(Path,Class,java.nio.file.LinkOption[]) </seealso>

	Public Interface FileAttributeView
		Inherits AttributeView

	End Interface

End Namespace
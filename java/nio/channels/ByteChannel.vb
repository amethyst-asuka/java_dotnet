'
' * Copyright (c) 2000, 2001, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.nio.channels



	''' <summary>
	''' A channel that can read and write bytes.  This interface simply unifies
	''' <seealso cref="ReadableByteChannel"/> and <seealso cref="WritableByteChannel"/>; it does not
	''' specify any new operations.
	''' 
	''' @author Mark Reinhold
	''' @author JSR-51 Expert Group
	''' @since 1.4
	''' </summary>

	Public Interface ByteChannel
		Inherits ReadableByteChannel, WritableByteChannel

	End Interface

End Namespace
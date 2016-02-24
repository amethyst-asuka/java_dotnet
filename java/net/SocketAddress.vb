Imports System

'
' * Copyright (c) 2000, 2013, Oracle and/or its affiliates. All rights reserved.
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
Namespace java.net


	''' 
	''' <summary>
	''' This class represents a Socket Address with no protocol attachment.
	''' As an abstract class, it is meant to be subclassed with a specific,
	''' protocol dependent, implementation.
	''' <p>
	''' It provides an immutable object used by sockets for binding, connecting, or
	''' as returned values.
	''' </summary>
	''' <seealso cref= java.net.Socket </seealso>
	''' <seealso cref= java.net.ServerSocket
	''' @since 1.4 </seealso>
	<Serializable> _
	Public MustInherit Class SocketAddress

		Friend Const serialVersionUID As Long = 5215720748342549866L

	End Class

End Namespace
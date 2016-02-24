'
' * Copyright (c) 1995, 2013, Oracle and/or its affiliates. All rights reserved.
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

	''' <summary>
	''' This interface defines a factory for socket implementations. It
	''' is used by the classes {@code Socket} and
	''' {@code ServerSocket} to create actual socket
	''' implementations.
	''' 
	''' @author  Arthur van Hoff </summary>
	''' <seealso cref=     java.net.Socket </seealso>
	''' <seealso cref=     java.net.ServerSocket
	''' @since   JDK1.0 </seealso>
	Public Interface SocketImplFactory
		''' <summary>
		''' Creates a new {@code SocketImpl} instance.
		''' </summary>
		''' <returns>  a new instance of {@code SocketImpl}. </returns>
		''' <seealso cref=     java.net.SocketImpl </seealso>
		Function createSocketImpl() As SocketImpl
	End Interface

End Namespace
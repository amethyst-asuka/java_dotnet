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
	''' This interface defines a factory for {@code URL} stream
	''' protocol handlers.
	''' <p>
	''' It is used by the {@code URL} class to create a
	''' {@code URLStreamHandler} for a specific protocol.
	''' 
	''' @author  Arthur van Hoff </summary>
	''' <seealso cref=     java.net.URL </seealso>
	''' <seealso cref=     java.net.URLStreamHandler
	''' @since   JDK1.0 </seealso>
	Public Interface URLStreamHandlerFactory
		''' <summary>
		''' Creates a new {@code URLStreamHandler} instance with the specified
		''' protocol.
		''' </summary>
		''' <param name="protocol">   the protocol ("{@code ftp}",
		'''                     "{@code http}", "{@code nntp}", etc.). </param>
		''' <returns>  a {@code URLStreamHandler} for the specific protocol. </returns>
		''' <seealso cref=     java.net.URLStreamHandler </seealso>
		Function createURLStreamHandler(ByVal protocol As String) As URLStreamHandler
	End Interface

End Namespace
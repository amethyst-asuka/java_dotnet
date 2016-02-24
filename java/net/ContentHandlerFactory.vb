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
	''' This interface defines a factory for content handlers. An
	''' implementation of this interface should map a MIME type into an
	''' instance of {@code ContentHandler}.
	''' <p>
	''' This interface is used by the {@code URLStreamHandler} class
	''' to create a {@code ContentHandler} for a MIME type.
	''' 
	''' @author  James Gosling </summary>
	''' <seealso cref=     java.net.ContentHandler </seealso>
	''' <seealso cref=     java.net.URLStreamHandler
	''' @since   JDK1.0 </seealso>
	Public Interface ContentHandlerFactory
		''' <summary>
		''' Creates a new {@code ContentHandler} to read an object from
		''' a {@code URLStreamHandler}.
		''' </summary>
		''' <param name="mimetype">   the MIME type for which a content handler is desired.
		''' </param>
		''' <returns>  a new {@code ContentHandler} to read an object from a
		'''          {@code URLStreamHandler}. </returns>
		''' <seealso cref=     java.net.ContentHandler </seealso>
		''' <seealso cref=     java.net.URLStreamHandler </seealso>
		Function createContentHandler(ByVal mimetype As String) As ContentHandler
	End Interface

End Namespace
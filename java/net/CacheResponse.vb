Imports System.Collections.Generic

'
' * Copyright (c) 2003, 2004, Oracle and/or its affiliates. All rights reserved.
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
	''' Represent channels for retrieving resources from the
	''' ResponseCache. Instances of such a class provide an
	''' InputStream that returns the entity body, and also a
	''' getHeaders() method which returns the associated response headers.
	''' 
	''' @author Yingxian Wang
	''' @since 1.5
	''' </summary>
	Public MustInherit Class CacheResponse

		''' <summary>
		''' Returns the response headers as a Map.
		''' </summary>
		''' <returns> An immutable Map from response header field names to
		'''         lists of field values. The status line has null as its
		'''         field name. </returns>
		''' <exception cref="IOException"> if an I/O error occurs
		'''            while getting the response headers </exception>
		Public MustOverride ReadOnly Property headers As IDictionary(Of String, IList(Of String))

		''' <summary>
		''' Returns the response body as an InputStream.
		''' </summary>
		''' <returns> an InputStream from which the response body can
		'''         be accessed </returns>
		''' <exception cref="IOException"> if an I/O error occurs while
		'''         getting the response body </exception>
		Public MustOverride ReadOnly Property body As java.io.InputStream
	End Class

End Namespace
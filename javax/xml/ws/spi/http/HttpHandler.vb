'
' * Copyright (c) 2009, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.xml.ws.spi.http


	''' <summary>
	''' A handler which is invoked to process HTTP requests.
	''' <p>
	''' JAX-WS runtime provides the implementation for this and sets
	''' it using <seealso cref="HttpContext#setHandler(HttpHandler)"/> during
	''' <seealso cref="Endpoint#publish(HttpContext) "/>
	''' 
	''' @author Jitendra Kotamraju
	''' @since JAX-WS 2.2
	''' </summary>
	Public MustInherit Class HttpHandler
		''' <summary>
		''' Handles a given request and generates an appropriate response.
		''' See <seealso cref="HttpExchange"/> for a description of the steps
		''' involved in handling an exchange. Container invokes this method
		''' when it receives an incoming request.
		''' </summary>
		''' <param name="exchange"> the exchange containing the request from the
		'''      client and used to send the response </param>
		''' <exception cref="IOException"> when an I/O error happens during request
		'''      handling </exception>
		Public MustOverride Sub handle(ByVal exchange As HttpExchange)
	End Class

End Namespace
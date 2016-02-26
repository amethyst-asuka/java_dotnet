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
	''' HttpContext represents a mapping between the root URI path of a web
	''' service to a <seealso cref="HttpHandler"/> which is invoked to handle requests
	''' destined for that path on the associated container.
	''' <p>
	''' Container provides the implementation for this and it matches
	''' web service requests to corresponding HttpContext objects.
	''' 
	''' @author Jitendra Kotamraju
	''' @since JAX-WS 2.2
	''' </summary>
	Public MustInherit Class HttpContext

		Protected Friend handler As HttpHandler

		''' <summary>
		''' JAX-WS runtime sets its handler during
		''' <seealso cref="Endpoint#publish(HttpContext)"/> to handle
		''' HTTP requests for this context. Container or its extensions
		''' use this handler to process the requests.
		''' </summary>
		''' <param name="handler"> the handler to set for this context </param>
		Public Overridable Property handler As HttpHandler
			Set(ByVal handler As HttpHandler)
				Me.handler = handler
			End Set
		End Property

		''' <summary>
		''' Returns the path for this context. This path uniquely identifies
		''' an endpoint inside an application and the path is relative to
		''' application's context path. Container should give this
		''' path based on how it matches request URIs to this HttpContext object.
		''' 
		''' <p>
		''' For servlet container, this is typically a url-pattern for an endpoint.
		''' 
		''' <p>
		''' Endpoint's address for this context can be computed as follows:
		''' <pre>
		'''  HttpExchange exch = ...;
		'''  String endpointAddress =
		'''      exch.getScheme() + "://"
		'''      + exch.getLocalAddress().getHostName()
		'''      + ":" + exch.getLocalAddress().getPort()
		'''      + exch.getContextPath() + getPath();
		''' </pre>
		''' </summary>
		''' <returns> this context's path </returns>
		Public MustOverride ReadOnly Property path As String

		''' <summary>
		''' Returns an attribute value for container's configuration
		''' and other data that can be used by jax-ws runtime.
		''' </summary>
		''' <param name="name"> attribute name </param>
		''' <returns> attribute value </returns>
		Public MustOverride Function getAttribute(ByVal name As String) As Object

		''' <summary>
		''' Returns all attribute names for container's configuration
		''' and other data that can be used by jax-ws runtime.
		''' </summary>
		''' <returns> set of all attribute names </returns>
		Public MustOverride ReadOnly Property attributeNames As java.util.Set(Of String)

	End Class

End Namespace
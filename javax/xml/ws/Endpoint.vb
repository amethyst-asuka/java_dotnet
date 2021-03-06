Imports System
Imports System.Collections.Generic

'
' * Copyright (c) 2005, 2011, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.xml.ws



	''' <summary>
	''' A Web service endpoint.
	''' 
	''' <p>Endpoints are created using the static methods defined in this
	''' class. An endpoint is always tied to one <code>Binding</code>
	''' and one implementor, both set at endpoint creation time.
	''' 
	''' <p>An endpoint is either in a published or an unpublished state.
	''' The <code>publish</code> methods can be used to start publishing
	''' an endpoint, at which point it starts accepting incoming requests.
	''' Conversely, the <code>stop</code> method can be used to stop
	''' accepting incoming requests and take the endpoint down.
	''' Once stopped, an endpoint cannot be published again.
	''' 
	''' <p>An <code>Executor</code> may be set on the endpoint in order
	''' to gain better control over the threads used to dispatch incoming
	''' requests. For instance, thread pooling with certain parameters
	''' can be enabled by creating a <code>ThreadPoolExecutor</code> and
	''' registering it with the endpoint.
	''' 
	''' <p>Handler chains can be set using the contained <code>Binding</code>.
	''' 
	''' <p>An endpoint may have a list of metadata documents, such as WSDL
	''' and XMLSchema documents, bound to it. At publishing time, the
	''' JAX-WS implementation will try to reuse as much of that metadata
	''' as possible instead of generating new ones based on the annotations
	''' present on the implementor.
	''' 
	''' @since JAX-WS 2.0
	''' </summary>
	''' <seealso cref= javax.xml.ws.Binding </seealso>
	''' <seealso cref= javax.xml.ws.BindingType </seealso>
	''' <seealso cref= javax.xml.ws.soap.SOAPBinding </seealso>
	''' <seealso cref= java.util.concurrent.Executor
	''' 
	'''  </seealso>
	Public MustInherit Class Endpoint

		''' <summary>
		''' Standard property: name of WSDL service.
		'''  <p>Type: javax.xml.namespace.QName
		''' 
		''' </summary>
		Public Const WSDL_SERVICE As String = "javax.xml.ws.wsdl.service"

		''' <summary>
		''' Standard property: name of WSDL port.
		'''  <p>Type: javax.xml.namespace.QName
		''' 
		''' </summary>
		Public Const WSDL_PORT As String = "javax.xml.ws.wsdl.port"


		''' <summary>
		''' Creates an endpoint with the specified implementor object. If there is
		''' a binding specified via a BindingType annotation then it MUST be used else
		''' a default of SOAP 1.1 / HTTP binding MUST be used.
		''' <p>
		''' The newly created endpoint may be published by calling
		''' one of the <seealso cref="javax.xml.ws.Endpoint#publish(String)"/> and
		''' <seealso cref="javax.xml.ws.Endpoint#publish(Object)"/> methods.
		''' 
		''' </summary>
		''' <param name="implementor"> The endpoint implementor.
		''' </param>
		''' <returns> The newly created endpoint.
		''' 
		'''  </returns>
		Public Shared Function create(ByVal implementor As Object) As Endpoint
			Return create(Nothing, implementor)
		End Function

		''' <summary>
		''' Creates an endpoint with the specified implementor object and web
		''' service features. If there is a binding specified via a BindingType
		''' annotation then it MUST be used else a default of SOAP 1.1 / HTTP
		''' binding MUST be used.
		''' <p>
		''' The newly created endpoint may be published by calling
		''' one of the <seealso cref="javax.xml.ws.Endpoint#publish(String)"/> and
		''' <seealso cref="javax.xml.ws.Endpoint#publish(Object)"/> methods.
		''' 
		''' </summary>
		''' <param name="implementor"> The endpoint implementor. </param>
		''' <param name="features"> A list of WebServiceFeature to configure on the
		'''        endpoint. Supported features not in the <code>features
		'''        </code> parameter will have their default values.
		''' 
		''' </param>
		''' <returns> The newly created endpoint.
		''' @since JAX-WS 2.2
		'''  </returns>
		Public Shared Function create(ByVal implementor As Object, ParamArray ByVal features As WebServiceFeature()) As Endpoint
			Return create(Nothing, implementor, features)
		End Function

		''' <summary>
		''' Creates an endpoint with the specified binding type and
		''' implementor object.
		''' <p>
		''' The newly created endpoint may be published by calling
		''' one of the <seealso cref="javax.xml.ws.Endpoint#publish(String)"/> and
		''' <seealso cref="javax.xml.ws.Endpoint#publish(Object)"/> methods.
		''' </summary>
		''' <param name="bindingId"> A URI specifying the binding to use. If the bindingID is
		''' <code>null</code> and no binding is specified via a BindingType
		''' annotation then a default SOAP 1.1 / HTTP binding MUST be used.
		''' </param>
		''' <param name="implementor"> The endpoint implementor.
		''' </param>
		''' <returns> The newly created endpoint.
		''' 
		'''  </returns>
		Public Shared Function create(ByVal bindingId As String, ByVal implementor As Object) As Endpoint
			Return javax.xml.ws.spi.Provider.provider().createEndpoint(bindingId, implementor)
		End Function

		''' <summary>
		''' Creates an endpoint with the specified binding type,
		''' implementor object, and web service features.
		''' <p>
		''' The newly created endpoint may be published by calling
		''' one of the <seealso cref="javax.xml.ws.Endpoint#publish(String)"/> and
		''' <seealso cref="javax.xml.ws.Endpoint#publish(Object)"/> methods.
		''' </summary>
		''' <param name="bindingId"> A URI specifying the binding to use. If the bindingID is
		''' <code>null</code> and no binding is specified via a BindingType
		''' annotation then a default SOAP 1.1 / HTTP binding MUST be used.
		''' </param>
		''' <param name="implementor"> The endpoint implementor.
		''' </param>
		''' <param name="features"> A list of WebServiceFeature to configure on the
		'''        endpoint. Supported features not in the <code>features
		'''        </code> parameter will have their default values.
		''' </param>
		''' <returns> The newly created endpoint.
		''' @since JAX-WS 2.2 </returns>
		Public Shared Function create(ByVal bindingId As String, ByVal implementor As Object, ParamArray ByVal features As WebServiceFeature()) As Endpoint
			Return javax.xml.ws.spi.Provider.provider().createEndpoint(bindingId, implementor, features)
		End Function

		''' <summary>
		''' Returns the binding for this endpoint.
		''' </summary>
		''' <returns> The binding for this endpoint
		'''  </returns>
		Public MustOverride ReadOnly Property binding As Binding

		''' <summary>
		''' Returns the implementation object for this endpoint.
		''' </summary>
		''' <returns> The implementor for this endpoint
		'''  </returns>
		Public MustOverride ReadOnly Property implementor As Object

		''' <summary>
		''' Publishes this endpoint at the given address.
		''' The necessary server infrastructure will be created and
		''' configured by the JAX-WS implementation using some default configuration.
		''' In order to get more control over the server configuration, please
		''' use the <seealso cref="javax.xml.ws.Endpoint#publish(Object)"/> method instead.
		''' </summary>
		''' <param name="address"> A URI specifying the address to use. The address
		'''        MUST be compatible with the binding specified at the
		'''        time the endpoint was created.
		''' </param>
		''' <exception cref="java.lang.IllegalArgumentException">
		'''          If the provided address URI is not usable
		'''          in conjunction with the endpoint's binding.
		''' </exception>
		''' <exception cref="java.lang.IllegalStateException">
		'''          If the endpoint has been published already or it has been stopped.
		''' </exception>
		''' <exception cref="java.lang.SecurityException">
		'''          If a <code>java.lang.SecurityManger</code>
		'''          is being used and the application doesn't have the
		'''          <code>WebServicePermission("publishEndpoint")</code> permission.
		'''  </exception>
		Public MustOverride Sub publish(ByVal address As String)

		''' <summary>
		''' Creates and publishes an endpoint for the specified implementor
		''' object at the given address.
		''' <p>
		''' The necessary server infrastructure will be created and
		''' configured by the JAX-WS implementation using some default configuration.
		''' 
		''' In order to get more control over the server configuration, please
		''' use the <seealso cref="javax.xml.ws.Endpoint#create(String,Object)"/> and
		''' <seealso cref="javax.xml.ws.Endpoint#publish(Object)"/> methods instead.
		''' </summary>
		''' <param name="address"> A URI specifying the address and transport/protocol
		'''        to use. A http: URI MUST result in the SOAP 1.1/HTTP
		'''        binding being used. Implementations may support other
		'''        URI schemes. </param>
		''' <param name="implementor"> The endpoint implementor.
		''' </param>
		''' <returns> The newly created endpoint.
		''' </returns>
		''' <exception cref="java.lang.SecurityException">
		'''          If a <code>java.lang.SecurityManger</code>
		'''          is being used and the application doesn't have the
		'''          <code>WebServicePermission("publishEndpoint")</code> permission.
		''' 
		'''  </exception>
		Public Shared Function publish(ByVal address As String, ByVal implementor As Object) As Endpoint
			Return javax.xml.ws.spi.Provider.provider().createAndPublishEndpoint(address, implementor)
		End Function

		''' <summary>
		''' Creates and publishes an endpoint for the specified implementor
		''' object at the given address. The created endpoint is configured
		''' with the web service features.
		''' <p>
		''' The necessary server infrastructure will be created and
		''' configured by the JAX-WS implementation using some default configuration.
		''' 
		''' In order to get more control over the server configuration, please
		''' use the <seealso cref="javax.xml.ws.Endpoint#create(String,Object)"/> and
		''' <seealso cref="javax.xml.ws.Endpoint#publish(Object)"/> methods instead.
		''' </summary>
		''' <param name="address"> A URI specifying the address and transport/protocol
		'''        to use. A http: URI MUST result in the SOAP 1.1/HTTP
		'''        binding being used. Implementations may support other
		'''        URI schemes. </param>
		''' <param name="implementor"> The endpoint implementor. </param>
		''' <param name="features"> A list of WebServiceFeature to configure on the
		'''        endpoint. Supported features not in the <code>features
		'''        </code> parameter will have their default values. </param>
		''' <returns> The newly created endpoint.
		''' </returns>
		''' <exception cref="java.lang.SecurityException">
		'''          If a <code>java.lang.SecurityManger</code>
		'''          is being used and the application doesn't have the
		'''          <code>WebServicePermission("publishEndpoint")</code> permission.
		''' @since JAX-WS 2.2 </exception>
		Public Shared Function publish(ByVal address As String, ByVal implementor As Object, ParamArray ByVal features As WebServiceFeature()) As Endpoint
			Return javax.xml.ws.spi.Provider.provider().createAndPublishEndpoint(address, implementor, features)
		End Function


		''' <summary>
		''' Publishes this endpoint at the provided server context.
		''' A server context encapsulates the server infrastructure
		''' and addressing information for a particular transport.
		''' For a call to this method to succeed, the server context
		''' passed as an argument to it MUST be compatible with the
		''' endpoint's binding.
		''' </summary>
		''' <param name="serverContext"> An object representing a server
		'''           context to be used for publishing the endpoint.
		''' </param>
		''' <exception cref="java.lang.IllegalArgumentException">
		'''              If the provided server context is not
		'''              supported by the implementation or turns
		'''              out to be unusable in conjunction with the
		'''              endpoint's binding.
		''' </exception>
		''' <exception cref="java.lang.IllegalStateException">
		'''         If the endpoint has been published already or it has been stopped.
		''' </exception>
		''' <exception cref="java.lang.SecurityException">
		'''          If a <code>java.lang.SecurityManger</code>
		'''          is being used and the application doesn't have the
		'''          <code>WebServicePermission("publishEndpoint")</code> permission.
		'''  </exception>
		Public MustOverride Sub publish(ByVal serverContext As Object)

		''' <summary>
		''' Publishes this endpoint at the provided server context.
		''' A server context encapsulates the server infrastructure
		''' and addressing information for a particular transport.
		''' For a call to this method to succeed, the server context
		''' passed as an argument to it MUST be compatible with the
		''' endpoint's binding.
		''' 
		''' <p>
		''' This is meant for container developers to publish the
		''' the endpoints portably and not intended for the end
		''' developers.
		''' 
		''' </summary>
		''' <param name="serverContext"> An object representing a server
		'''           context to be used for publishing the endpoint.
		''' </param>
		''' <exception cref="java.lang.IllegalArgumentException">
		'''              If the provided server context is not
		'''              supported by the implementation or turns
		'''              out to be unusable in conjunction with the
		'''              endpoint's binding.
		''' </exception>
		''' <exception cref="java.lang.IllegalStateException">
		'''         If the endpoint has been published already or it has been stopped.
		''' </exception>
		''' <exception cref="java.lang.SecurityException">
		'''          If a <code>java.lang.SecurityManger</code>
		'''          is being used and the application doesn't have the
		'''          <code>WebServicePermission("publishEndpoint")</code> permission.
		''' @since JAX-WS 2.2 </exception>
		Public Overridable Sub publish(ByVal serverContext As javax.xml.ws.spi.http.HttpContext)
			Throw New System.NotSupportedException("JAX-WS 2.2 implementation must override this default behaviour.")
		End Sub

		''' <summary>
		''' Stops publishing this endpoint.
		''' 
		''' If the endpoint is not in a published state, this method
		''' has no effect.
		''' 
		''' 
		''' </summary>
		Public MustOverride Sub [stop]()

		''' <summary>
		''' Returns true if the endpoint is in the published state.
		''' </summary>
		''' <returns> <code>true</code> if the endpoint is in the published state.
		'''  </returns>
		Public MustOverride ReadOnly Property published As Boolean

		''' <summary>
		''' Returns a list of metadata documents for the service.
		''' </summary>
		''' <returns> <code>List&lt;javax.xml.transform.Source&gt;</code> A list of metadata documents for the service
		'''  </returns>
		Public MustOverride Property metadata As IList(Of javax.xml.transform.Source)


		''' <summary>
		''' Returns the executor for this <code>Endpoint</code>instance.
		''' 
		''' The executor is used to dispatch an incoming request to
		''' the implementor object.
		''' </summary>
		''' <returns> The <code>java.util.concurrent.Executor</code> to be
		'''         used to dispatch a request.
		''' </returns>
		''' <seealso cref= java.util.concurrent.Executor
		'''  </seealso>
		Public MustOverride Property executor As java.util.concurrent.Executor



		''' <summary>
		''' Returns the property bag for this <code>Endpoint</code> instance.
		''' </summary>
		''' <returns> Map&lt;String,Object&gt; The property bag
		'''         associated with this instance.
		'''  </returns>
		Public MustOverride Property properties As IDictionary(Of String, Object)


		''' <summary>
		''' Returns the <code>EndpointReference</code> associated with
		''' this <code>Endpoint</code> instance.
		''' <p>
		''' If the Binding for this <code>bindingProvider</code> is
		''' either SOAP1.1/HTTP or SOAP1.2/HTTP, then a
		''' <code>W3CEndpointReference</code> MUST be returned.
		''' </summary>
		''' <param name="referenceParameters"> Reference parameters to be associated with the
		''' returned <code>EndpointReference</code> instance. </param>
		''' <returns> EndpointReference of this <code>Endpoint</code> instance.
		''' If the returned <code>EndpointReference</code> is of type
		''' <code>W3CEndpointReference</code> then it MUST contain the
		''' the specified <code>referenceParameters</code>.
		''' </returns>
		''' <exception cref="WebServiceException"> If any error in the creation of
		''' the <code>EndpointReference</code> or if the <code>Endpoint</code> is
		''' not in the published state. </exception>
		''' <exception cref="UnsupportedOperationException"> If this <code>BindingProvider</code>
		''' uses the XML/HTTP binding.
		''' </exception>
		''' <seealso cref= W3CEndpointReference
		''' 
		''' @since JAX-WS 2.1
		'''  </seealso>
		Public MustOverride Function getEndpointReference(ParamArray ByVal referenceParameters As org.w3c.dom.Element()) As EndpointReference


		''' <summary>
		''' Returns the <code>EndpointReference</code> associated with
		''' this <code>Endpoint</code> instance.
		''' </summary>
		''' <param name="clazz"> Specifies the type of EndpointReference  that MUST be returned. </param>
		''' <param name="referenceParameters"> Reference parameters to be associated with the
		''' returned <code>EndpointReference</code> instance. </param>
		''' <returns> EndpointReference of type <code>clazz</code> of this
		''' <code>Endpoint</code> instance.
		''' If the returned <code>EndpointReference</code> is of type
		''' <code>W3CEndpointReference</code> then it MUST contain the
		''' the specified <code>referenceParameters</code>.
		''' </returns>
		''' <exception cref="WebServiceException"> If any error in the creation of
		''' the <code>EndpointReference</code> or if the <code>Endpoint</code> is
		''' not in the published state or if the <code>clazz</code> is not a supported
		''' <code>EndpointReference</code> type. </exception>
		''' <exception cref="UnsupportedOperationException"> If this <code>BindingProvider</code>
		''' uses the XML/HTTP binding.
		''' 
		''' 
		''' @since JAX-WS 2.1
		'''  </exception>
		Public MustOverride Function getEndpointReference(Of T As EndpointReference)(ByVal clazz As Type, ParamArray ByVal referenceParameters As org.w3c.dom.Element()) As T

		''' <summary>
		''' By settng a <code>EndpointContext</code>, JAX-WS runtime knows about
		''' addresses of other endpoints in an application. If multiple endpoints
		''' share different ports of a WSDL, then the multiple port addresses
		''' are patched when the WSDL is accessed.
		''' 
		''' <p>
		''' This needs to be set before publishing the endpoints.
		''' </summary>
		''' <param name="ctxt"> that is shared for multiple endpoints </param>
		''' <exception cref="java.lang.IllegalStateException">
		'''        If the endpoint has been published already or it has been stopped.
		''' 
		''' @since JAX-WS 2.2 </exception>
		Public Overridable Property endpointContext As EndpointContext
			Set(ByVal ctxt As EndpointContext)
				Throw New System.NotSupportedException("JAX-WS 2.2 implementation must override this default behaviour.")
			End Set
		End Property
	End Class

End Namespace
Imports System
Imports System.Collections.Generic

'
' * Copyright (c) 2005, 2010, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.xml.ws.spi



	''' <summary>
	''' Service delegates are used internally by <code>Service</code> objects
	''' to allow pluggability of JAX-WS implementations.
	''' <p>
	''' Every <code>Service</code> object has its own delegate, created using
	''' the <seealso cref="javax.xml.ws.spi.Provider#createServiceDelegate"/> method. A <code>Service</code>
	''' object delegates all of its instance methods to its delegate.
	''' </summary>
	''' <seealso cref= javax.xml.ws.Service </seealso>
	''' <seealso cref= javax.xml.ws.spi.Provider
	''' 
	''' @since JAX-WS 2.0 </seealso>
	Public MustInherit Class ServiceDelegate

		Protected Friend Sub New()
		End Sub

		''' <summary>
		''' The <code>getPort</code> method returns a proxy. A service client
		''' uses this proxy to invoke operations on the target
		''' service endpoint. The <code>serviceEndpointInterface</code>
		''' specifies the service endpoint interface that is supported by
		''' the created dynamic proxy instance.
		''' </summary>
		''' <param name="portName">  Qualified name of the service endpoint in
		'''                  the WSDL service description </param>
		''' <param name="serviceEndpointInterface"> Service endpoint interface
		'''                  supported by the dynamic proxy </param>
		''' <returns> Object Proxy instance that
		'''                supports the specified service endpoint
		'''                interface </returns>
		''' <exception cref="WebServiceException"> This exception is thrown in the
		'''                  following cases:
		'''                  <UL>
		'''                  <LI>If there is an error in creation of
		'''                      the proxy
		'''                  <LI>If there is any missing WSDL metadata
		'''                      as required by this method
		'''                  <LI>If an illegal
		'''                      <code>serviceEndpointInterface</code>
		'''                      or <code>portName</code> is specified
		'''                  </UL> </exception>
		''' <seealso cref= java.lang.reflect.Proxy </seealso>
		''' <seealso cref= java.lang.reflect.InvocationHandler
		'''  </seealso>
		Public MustOverride Function getPort(Of T)(ByVal portName As javax.xml.namespace.QName, ByVal serviceEndpointInterface As Type) As T

		''' <summary>
		''' The <code>getPort</code> method returns a proxy. A service client
		''' uses this proxy to invoke operations on the target
		''' service endpoint. The <code>serviceEndpointInterface</code>
		''' specifies the service endpoint interface that is supported by
		''' the created dynamic proxy instance.
		''' </summary>
		''' <param name="portName">  Qualified name of the service endpoint in
		'''                  the WSDL service description </param>
		''' <param name="serviceEndpointInterface"> Service endpoint interface
		'''                  supported by the dynamic proxy or instance </param>
		''' <param name="features">  A list of WebServiceFeatures to configure on the
		'''                proxy.  Supported features not in the <code>features
		'''                </code> parameter will have their default values. </param>
		''' <returns> Object Proxy instance that
		'''                supports the specified service endpoint
		'''                interface </returns>
		''' <exception cref="WebServiceException"> This exception is thrown in the
		'''                  following cases:
		'''                  <UL>
		'''                  <LI>If there is an error in creation of
		'''                      the proxy
		'''                  <LI>If there is any missing WSDL metadata
		'''                      as required by this method
		'''                  <LI>If an illegal
		'''                      <code>serviceEndpointInterface</code>
		'''                      or <code>portName</code> is specified
		'''                  <LI>If a feature is enabled that is not compatible
		'''                      with this port or is unsupported.
		'''                  </UL> </exception>
		''' <seealso cref= java.lang.reflect.Proxy </seealso>
		''' <seealso cref= java.lang.reflect.InvocationHandler </seealso>
		''' <seealso cref= WebServiceFeature
		''' 
		''' @since JAX-WS 2.1
		'''  </seealso>
		Public MustOverride Function getPort(Of T)(ByVal portName As javax.xml.namespace.QName, ByVal serviceEndpointInterface As Type, ParamArray ByVal features As javax.xml.ws.WebServiceFeature()) As T

		''' <summary>
		''' The <code>getPort</code> method returns a proxy.
		''' The parameter <code>endpointReference</code> specifies the
		''' endpoint that will be invoked by the returned proxy.  If there
		''' are any reference parameters in the
		''' <code>endpointReference</code>, then those reference
		''' parameters MUST appear as SOAP headers, indicating them to be
		''' reference parameters, on all messages sent to the endpoint.
		''' The <code>endpointReference's</code> address MUST be used
		''' for invocations on the endpoint.
		''' The parameter <code>serviceEndpointInterface</code> specifies
		''' the service endpoint interface that is supported by the
		''' returned proxy.
		''' In the implementation of this method, the JAX-WS
		''' runtime system takes the responsibility of selecting a protocol
		''' binding (and a port) and configuring the proxy accordingly from
		''' the WSDL associated with this <code>Service</code> instance or
		''' from the metadata from the <code>endpointReference</code>.
		''' If this <code>Service</code> instance has a WSDL and
		''' the <code>endpointReference</code> metadata
		''' also has a WSDL, then the WSDL from this instance MUST be used.
		''' If this <code>Service</code> instance does not have a WSDL and
		''' the <code>endpointReference</code> does have a WSDL, then the
		''' WSDL from the <code>endpointReference</code> MAY be used.
		''' The returned proxy should not be reconfigured by the client.
		''' If this <code>Service</code> instance has a known proxy
		''' port that matches the information contained in
		''' the WSDL,
		''' then that proxy is returned, otherwise a WebServiceException
		''' is thrown.
		''' <p>
		''' Calling this method has the same behavior as the following
		''' <pre>
		''' <code>port = service.getPort(portName, serviceEndpointInterface);</code>
		''' </pre>
		''' where the <code>portName</code> is retrieved from the
		''' metadata of the <code>endpointReference</code> or from the
		''' <code>serviceEndpointInterface</code> and the WSDL
		''' associated with this <code>Service</code> instance.
		''' </summary>
		''' <param name="endpointReference">  The <code>EndpointReference</code>
		''' for the target service endpoint that will be invoked by the
		''' returned proxy. </param>
		''' <param name="serviceEndpointInterface"> Service endpoint interface. </param>
		''' <param name="features">  A list of <code>WebServiceFeatures</code> to configure on the
		'''                proxy.  Supported features not in the <code>features
		'''                </code> parameter will have their default values. </param>
		''' <returns> Object Proxy instance that supports the
		'''                  specified service endpoint interface. </returns>
		''' <exception cref="WebServiceException">
		'''                  <UL>
		'''                  <LI>If there is an error during creation
		'''                      of the proxy.
		'''                  <LI>If there is any missing WSDL metadata
		'''                      as required by this method.
		'''                  <LI>If the <code>endpointReference</code> metadata does
		'''                      not match the <code>serviceName</code> of this
		'''                      <code>Service</code> instance.
		'''                  <LI>If a <code>portName</code> cannot be extracted
		'''                      from the WSDL or <code>endpointReference</code> metadata.
		'''                  <LI>If an invalid
		'''                      <code>endpointReference</code>
		'''                      is specified.
		'''                  <LI>If an invalid
		'''                      <code>serviceEndpointInterface</code>
		'''                      is specified.
		'''                  <LI>If a feature is enabled that is not compatible
		'''                      with this port or is unsupported.
		'''                  </UL>
		''' 
		''' @since JAX-WS 2.1
		'''  </exception>
		Public MustOverride Function getPort(Of T)(ByVal endpointReference As javax.xml.ws.EndpointReference, ByVal serviceEndpointInterface As Type, ParamArray ByVal features As javax.xml.ws.WebServiceFeature()) As T


		''' <summary>
		''' The <code>getPort</code> method returns a proxy. The parameter
		''' <code>serviceEndpointInterface</code> specifies the service
		''' endpoint interface that is supported by the returned proxy.
		''' In the implementation of this method, the JAX-WS
		''' runtime system takes the responsibility of selecting a protocol
		''' binding (and a port) and configuring the proxy accordingly.
		''' The returned proxy should not be reconfigured by the client.
		''' </summary>
		''' <param name="serviceEndpointInterface"> Service endpoint interface </param>
		''' <returns> Object instance that supports the
		'''                  specified service endpoint interface </returns>
		''' <exception cref="WebServiceException">
		'''                  <UL>
		'''                  <LI>If there is an error during creation
		'''                      of the proxy
		'''                  <LI>If there is any missing WSDL metadata
		'''                      as required by this method
		'''                  <LI>If an illegal
		'''                      <code>serviceEndpointInterface</code>
		'''                      is specified
		'''                  </UL>
		'''  </exception>
		Public MustOverride Function getPort(Of T)(ByVal serviceEndpointInterface As Type) As T


		''' <summary>
		''' The <code>getPort</code> method returns a proxy. The parameter
		''' <code>serviceEndpointInterface</code> specifies the service
		''' endpoint interface that is supported by the returned proxy.
		''' In the implementation of this method, the JAX-WS
		''' runtime system takes the responsibility of selecting a protocol
		''' binding (and a port) and configuring the proxy accordingly.
		''' The returned proxy should not be reconfigured by the client.
		''' </summary>
		''' <param name="serviceEndpointInterface"> Service endpoint interface </param>
		''' <param name="features">  An array of <code>WebServiceFeatures</code> to configure on the
		'''                proxy.  Supported features not in the <code>features
		'''                </code> parameter will have their default values. </param>
		''' <returns> Object instance that supports the
		'''                  specified service endpoint interface </returns>
		''' <exception cref="WebServiceException">
		'''                  <UL>
		'''                  <LI>If there is an error during creation
		'''                      of the proxy
		'''                  <LI>If there is any missing WSDL metadata
		'''                      as required by this method
		'''                  <LI>If an illegal
		'''                      <code>serviceEndpointInterface</code>
		'''                      is specified
		'''                  <LI>If a feature is enabled that is not compatible
		'''                      with this port or is unsupported.
		'''                  </UL>
		''' </exception>
		''' <seealso cref= WebServiceFeature
		''' 
		''' @since JAX-WS 2.1
		'''  </seealso>
		Public MustOverride Function getPort(Of T)(ByVal serviceEndpointInterface As Type, ParamArray ByVal features As javax.xml.ws.WebServiceFeature()) As T


		''' <summary>
		''' Creates a new port for the service. Ports created in this way contain
		''' no WSDL port type information and can only be used for creating
		''' <code>Dispatch</code>instances.
		''' </summary>
		''' <param name="portName">  Qualified name for the target service endpoint </param>
		''' <param name="bindingId"> A URI identifier of a binding. </param>
		''' <param name="endpointAddress"> Address of the target service endpoint as a URI </param>
		''' <exception cref="WebServiceException"> If any error in the creation of
		''' the port
		''' </exception>
		''' <seealso cref= javax.xml.ws.soap.SOAPBinding#SOAP11HTTP_BINDING </seealso>
		''' <seealso cref= javax.xml.ws.soap.SOAPBinding#SOAP12HTTP_BINDING </seealso>
		''' <seealso cref= javax.xml.ws.http.HTTPBinding#HTTP_BINDING
		'''  </seealso>
		Public MustOverride Sub addPort(ByVal portName As javax.xml.namespace.QName, ByVal bindingId As String, ByVal endpointAddress As String)



		''' <summary>
		''' Creates a <code>Dispatch</code> instance for use with objects of
		''' the user's choosing.
		''' </summary>
		''' <param name="portName">  Qualified name for the target service endpoint </param>
		''' <param name="type"> The class of object used for messages or message
		''' payloads. Implementations are required to support
		''' <code>javax.xml.transform.Source</code> and <code>javax.xml.soap.SOAPMessage</code>. </param>
		''' <param name="mode"> Controls whether the created dispatch instance is message
		''' or payload oriented, i.e. whether the user will work with complete
		''' protocol messages or message payloads. E.g. when using the SOAP
		''' protocol, this parameter controls whether the user will work with
		''' SOAP messages or the contents of a SOAP body. Mode MUST be <code>MESSAGE</code>
		''' when type is <code>SOAPMessage</code>.
		''' </param>
		''' <returns> Dispatch instance </returns>
		''' <exception cref="WebServiceException"> If any error in the creation of
		'''                  the <code>Dispatch</code> object </exception>
		''' <seealso cref= javax.xml.transform.Source </seealso>
		''' <seealso cref= javax.xml.soap.SOAPMessage
		'''  </seealso>
		Public MustOverride Function createDispatch(Of T)(ByVal portName As javax.xml.namespace.QName, ByVal type As Type, ByVal mode As javax.xml.ws.Service.Mode) As javax.xml.ws.Dispatch(Of T)

		''' <summary>
		''' Creates a <code>Dispatch</code> instance for use with objects of
		''' the user's choosing.
		''' </summary>
		''' <param name="portName">  Qualified name for the target service endpoint </param>
		''' <param name="type"> The class of object used for messages or message
		''' payloads. Implementations are required to support
		''' <code>javax.xml.transform.Source</code> and <code>javax.xml.soap.SOAPMessage</code>. </param>
		''' <param name="mode"> Controls whether the created dispatch instance is message
		''' or payload oriented, i.e. whether the user will work with complete
		''' protocol messages or message payloads. E.g. when using the SOAP
		''' protocol, this parameter controls whether the user will work with
		''' SOAP messages or the contents of a SOAP body. Mode MUST be <code>MESSAGE</code>
		''' when type is <code>SOAPMessage</code>. </param>
		''' <param name="features">  A list of <code>WebServiceFeatures</code> to configure on the
		'''                proxy.  Supported features not in the <code>features
		'''                </code> parameter will have their default values.
		''' </param>
		''' <returns> Dispatch instance </returns>
		''' <exception cref="WebServiceException"> If any error in the creation of
		'''                  the <code>Dispatch</code> object or if a
		'''                  feature is enabled that is not compatible with
		'''                  this port or is unsupported.
		''' </exception>
		''' <seealso cref= javax.xml.transform.Source </seealso>
		''' <seealso cref= javax.xml.soap.SOAPMessage </seealso>
		''' <seealso cref= WebServiceFeature
		''' 
		''' @since JAX-WS 2.1
		'''  </seealso>
		Public MustOverride Function createDispatch(Of T)(ByVal portName As javax.xml.namespace.QName, ByVal type As Type, ByVal mode As javax.xml.ws.Service.Mode, ParamArray ByVal features As javax.xml.ws.WebServiceFeature()) As javax.xml.ws.Dispatch(Of T)

		''' <summary>
		''' Creates a <code>Dispatch</code> instance for use with objects of
		''' the user's choosing. If there
		''' are any reference parameters in the
		''' <code>endpointReference</code>, then those reference
		''' parameters MUST appear as SOAP headers, indicating them to be
		''' reference parameters, on all messages sent to the endpoint.
		''' The <code>endpointReference's</code> address MUST be used
		''' for invocations on the endpoint.
		''' In the implementation of this method, the JAX-WS
		''' runtime system takes the responsibility of selecting a protocol
		''' binding (and a port) and configuring the dispatch accordingly from
		''' the WSDL associated with this <code>Service</code> instance or
		''' from the metadata from the <code>endpointReference</code>.
		''' If this <code>Service</code> instance has a WSDL and
		''' the <code>endpointReference</code>
		''' also has a WSDL in its metadata, then the WSDL from this instance MUST be used.
		''' If this <code>Service</code> instance does not have a WSDL and
		''' the <code>endpointReference</code> does have a WSDL, then the
		''' WSDL from the <code>endpointReference</code> MAY be used.
		''' An implementation MUST be able to retrieve the <code>portName</code> from the
		''' <code>endpointReference</code> metadata.
		''' <p>
		''' This method behaves the same as calling
		''' <pre>
		''' <code>dispatch = service.createDispatch(portName, type, mode, features);</code>
		''' </pre>
		''' where the <code>portName</code> is retrieved from the
		''' WSDL or <code>EndpointReference</code> metadata.
		''' </summary>
		''' <param name="endpointReference">  The <code>EndpointReference</code>
		''' for the target service endpoint that will be invoked by the
		''' returned <code>Dispatch</code> object. </param>
		''' <param name="type"> The class of object used to messages or message
		''' payloads. Implementations are required to support
		''' <code>javax.xml.transform.Source</code> and <code>javax.xml.soap.SOAPMessage</code>. </param>
		''' <param name="mode"> Controls whether the created dispatch instance is message
		''' or payload oriented, i.e. whether the user will work with complete
		''' protocol messages or message payloads. E.g. when using the SOAP
		''' protocol, this parameter controls whether the user will work with
		''' SOAP messages or the contents of a SOAP body. Mode MUST be <code>MESSAGE</code>
		''' when type is <code>SOAPMessage</code>. </param>
		''' <param name="features">  An array of <code>WebServiceFeatures</code> to configure on the
		'''                proxy.  Supported features not in the <code>features
		'''                </code> parameter will have their default values.
		''' </param>
		''' <returns> Dispatch instance </returns>
		''' <exception cref="WebServiceException">
		'''                  <UL>
		'''                    <LI>If there is any missing WSDL metadata
		'''                      as required by this method.
		'''                    <li>If the <code>endpointReference</code> metadata does
		'''                      not match the <code>serviceName</code> or <code>portName</code>
		'''                      of a WSDL associated
		'''                      with this <code>Service</code> instance.
		'''                    <li>If the <code>portName</code> cannot be determined
		'''                    from the <code>EndpointReference</code> metadata.
		'''                    <li>If any error in the creation of
		'''                     the <code>Dispatch</code> object.
		'''                    <li>If a feature is enabled that is not
		'''                    compatible with this port or is unsupported.
		'''                  </UL>
		''' </exception>
		''' <seealso cref= javax.xml.transform.Source </seealso>
		''' <seealso cref= javax.xml.soap.SOAPMessage </seealso>
		''' <seealso cref= WebServiceFeature
		''' 
		''' @since JAX-WS 2.1
		'''  </seealso>
		Public MustOverride Function createDispatch(Of T)(ByVal endpointReference As javax.xml.ws.EndpointReference, ByVal type As Type, ByVal mode As javax.xml.ws.Service.Mode, ParamArray ByVal features As javax.xml.ws.WebServiceFeature()) As javax.xml.ws.Dispatch(Of T)



		''' <summary>
		''' Creates a <code>Dispatch</code> instance for use with JAXB
		''' generated objects.
		''' </summary>
		''' <param name="portName">  Qualified name for the target service endpoint </param>
		''' <param name="context"> The JAXB context used to marshall and unmarshall
		''' messages or message payloads. </param>
		''' <param name="mode"> Controls whether the created dispatch instance is message
		''' or payload oriented, i.e. whether the user will work with complete
		''' protocol messages or message payloads. E.g. when using the SOAP
		''' protocol, this parameter controls whether the user will work with
		''' SOAP messages or the contents of a SOAP body.
		''' </param>
		''' <returns> Dispatch instance </returns>
		''' <exception cref="WebServiceException"> If any error in the creation of
		'''                  the <code>Dispatch</code> object
		''' </exception>
		''' <seealso cref= javax.xml.bind.JAXBContext
		'''  </seealso>
		Public MustOverride Function createDispatch(ByVal portName As javax.xml.namespace.QName, ByVal context As javax.xml.bind.JAXBContext, ByVal mode As javax.xml.ws.Service.Mode) As javax.xml.ws.Dispatch(Of Object)


		''' <summary>
		''' Creates a <code>Dispatch</code> instance for use with JAXB
		''' generated objects.
		''' </summary>
		''' <param name="portName">  Qualified name for the target service endpoint </param>
		''' <param name="context"> The JAXB context used to marshall and unmarshall
		''' messages or message payloads. </param>
		''' <param name="mode"> Controls whether the created dispatch instance is message
		''' or payload oriented, i.e. whether the user will work with complete
		''' protocol messages or message payloads. E.g. when using the SOAP
		''' protocol, this parameter controls whether the user will work with
		''' SOAP messages or the contents of a SOAP body. </param>
		''' <param name="features">  A list of <code>WebServiceFeatures</code> to configure on the
		'''                proxy.  Supported features not in the <code>features
		'''                </code> parameter will have their default values.
		''' </param>
		''' <returns> Dispatch instance </returns>
		''' <exception cref="WebServiceException"> If any error in the creation of
		'''                  the <code>Dispatch</code> object or if a
		'''                  feature is enabled that is not compatible with
		'''                  this port or is unsupported.
		''' </exception>
		''' <seealso cref= javax.xml.bind.JAXBContext </seealso>
		''' <seealso cref= WebServiceFeature
		''' 
		''' @since JAX-WS 2.1
		'''  </seealso>
		Public MustOverride Function createDispatch(ByVal portName As javax.xml.namespace.QName, ByVal context As javax.xml.bind.JAXBContext, ByVal mode As javax.xml.ws.Service.Mode, ParamArray ByVal features As javax.xml.ws.WebServiceFeature()) As javax.xml.ws.Dispatch(Of Object)

		''' <summary>
		''' Creates a <code>Dispatch</code> instance for use with JAXB
		''' generated objects. If there
		''' are any reference parameters in the
		''' <code>endpointReference</code>, then those reference
		''' parameters MUST appear as SOAP headers, indicating them to be
		''' reference parameters, on all messages sent to the endpoint.
		''' The <code>endpointReference's</code> address MUST be used
		''' for invocations on the endpoint.
		''' In the implementation of this method, the JAX-WS
		''' runtime system takes the responsibility of selecting a protocol
		''' binding (and a port) and configuring the dispatch accordingly from
		''' the WSDL associated with this <code>Service</code> instance or
		''' from the metadata from the <code>endpointReference</code>.
		''' If this <code>Service</code> instance has a WSDL and
		''' the <code>endpointReference</code>
		''' also has a WSDL in its metadata, then the WSDL from this instance
		''' MUST be used.
		''' If this <code>Service</code> instance does not have a WSDL and
		''' the <code>endpointReference</code> does have a WSDL, then the
		''' WSDL from the <code>endpointReference</code> MAY be used.
		''' An implementation MUST be able to retrieve the <code>portName</code> from the
		''' <code>endpointReference</code> metadata.
		''' <p>
		''' This method behavies the same as calling
		''' <pre>
		''' <code>dispatch = service.createDispatch(portName, context, mode, features);</code>
		''' </pre>
		''' where the <code>portName</code> is retrieved from the
		''' WSDL or <code>endpointReference</code> metadata.
		''' </summary>
		''' <param name="endpointReference">  The <code>EndpointReference</code>
		''' for the target service endpoint that will be invoked by the
		''' returned <code>Dispatch</code> object. </param>
		''' <param name="context"> The JAXB context used to marshall and unmarshall
		''' messages or message payloads. </param>
		''' <param name="mode"> Controls whether the created dispatch instance is message
		''' or payload oriented, i.e. whether the user will work with complete
		''' protocol messages or message payloads. E.g. when using the SOAP
		''' protocol, this parameter controls whether the user will work with
		''' SOAP messages or the contents of a SOAP body. </param>
		''' <param name="features">  An array of <code>WebServiceFeatures</code> to configure on the
		'''                proxy.  Supported features not in the <code>features
		'''                </code> parameter will have their default values.
		''' </param>
		''' <returns> Dispatch instance </returns>
		''' <exception cref="WebServiceException">
		'''                  <UL>
		'''                    <li>If there is any missing WSDL metadata
		'''                      as required by this method.
		'''                    <li>If the <code>endpointReference</code> metadata does
		'''                    not match the <code>serviceName</code> or <code>portName</code>
		'''                    of a WSDL associated
		'''                    with this <code>Service</code> instance.
		'''                    <li>If the <code>portName</code> cannot be determined
		'''                    from the <code>EndpointReference</code> metadata.
		'''                    <li>If any error in the creation of
		'''                    the <code>Dispatch</code> object.
		'''                    <li>if a feature is enabled that is not
		'''                    compatible with this port or is unsupported.
		'''                  </UL>
		''' </exception>
		''' <seealso cref= javax.xml.bind.JAXBContext </seealso>
		''' <seealso cref= WebServiceFeature
		''' 
		''' @since JAX-WS 2.1
		'''  </seealso>
		Public MustOverride Function createDispatch(ByVal endpointReference As javax.xml.ws.EndpointReference, ByVal context As javax.xml.bind.JAXBContext, ByVal mode As javax.xml.ws.Service.Mode, ParamArray ByVal features As javax.xml.ws.WebServiceFeature()) As javax.xml.ws.Dispatch(Of Object)


		''' <summary>
		''' Gets the name of this service. </summary>
		''' <returns> Qualified name of this service
		'''  </returns>
		Public MustOverride ReadOnly Property serviceName As javax.xml.namespace.QName

		''' <summary>
		''' Returns an <code>Iterator</code> for the list of
		''' <code>QName</code>s of service endpoints grouped by this
		''' service
		''' </summary>
		''' <returns> Returns <code>java.util.Iterator</code> with elements
		'''         of type <code>javax.xml.namespace.QName</code> </returns>
		''' <exception cref="WebServiceException"> If this Service class does not
		'''         have access to the required WSDL metadata
		'''  </exception>
		Public MustOverride ReadOnly Property ports As IEnumerator(Of javax.xml.namespace.QName)

		''' <summary>
		''' Gets the location of the WSDL document for this Service.
		''' </summary>
		''' <returns> URL for the location of the WSDL document for
		'''         this service
		'''  </returns>
		Public MustOverride ReadOnly Property wSDLDocumentLocation As java.net.URL

		''' <summary>
		''' Returns the configured handler resolver.
		''' </summary>
		''' <returns> HandlerResolver The <code>HandlerResolver</code> being
		'''         used by this <code>Service</code> instance, or <code>null</code>
		'''         if there isn't one.
		'''  </returns>
		Public MustOverride Property handlerResolver As javax.xml.ws.handler.HandlerResolver


		''' <summary>
		''' Returns the executor for this <code>Service</code>instance.
		''' 
		''' The executor is used for all asynchronous invocations that
		''' require callbacks.
		''' </summary>
		''' <returns> The <code>java.util.concurrent.Executor</code> to be
		'''         used to invoke a callback.
		''' </returns>
		''' <seealso cref= java.util.concurrent.Executor
		'''  </seealso>
		Public MustOverride Property executor As java.util.concurrent.Executor


	End Class

End Namespace
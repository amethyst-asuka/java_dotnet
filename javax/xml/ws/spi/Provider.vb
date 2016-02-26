Imports System
Imports System.Collections.Generic
Imports javax.xml.ws

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

Namespace javax.xml.ws.spi



	''' <summary>
	''' Service provider for <code>ServiceDelegate</code> and
	''' <code>Endpoint</code> objects.
	''' <p>
	''' 
	''' @since JAX-WS 2.0
	''' </summary>
	Public MustInherit Class Provider

		''' <summary>
		''' A constant representing the property used to lookup the
		''' name of a <code>Provider</code> implementation
		''' class.
		''' </summary>
		Public Const JAXWSPROVIDER_PROPERTY As String = "javax.xml.ws.spi.Provider"

		''' <summary>
		''' A constant representing the name of the default
		''' <code>Provider</code> implementation class.
		''' 
		''' </summary>
		' Using two strings so that package renaming doesn't change it
		Friend Shared ReadOnly DEFAULT_JAXWSPROVIDER As String = "com.sun" & ".xml.internal.ws.spi.ProviderImpl"

		''' <summary>
		''' Take advantage of Java SE 6's java.util.ServiceLoader API.
		''' Using reflection so that there is no compile-time dependency on SE 6.
		''' </summary>
		Private Shared ReadOnly loadMethod As Method
		Private Shared ReadOnly iteratorMethod As Method
		Shared Sub New()
			Dim tLoadMethod As Method = Nothing
			Dim tIteratorMethod As Method = Nothing
			Try
				Dim clazz As Type = Type.GetType("java.util.ServiceLoader")
				tLoadMethod = clazz.GetMethod("load", GetType(Type))
				tIteratorMethod = clazz.GetMethod("iterator")
			Catch ce As ClassNotFoundException
				' Running on Java SE 5
			Catch ne As NoSuchMethodException
				' Shouldn't happen
			End Try
			loadMethod = tLoadMethod
			iteratorMethod = tIteratorMethod
		End Sub


		''' <summary>
		''' Creates a new instance of Provider
		''' </summary>
		Protected Friend Sub New()
		End Sub

		''' 
		''' <summary>
		''' Creates a new provider object.
		''' <p>
		''' The algorithm used to locate the provider subclass to use consists
		''' of the following steps:
		''' <p>
		''' <ul>
		''' <li>
		'''   If a resource with the name of
		'''   <code>META-INF/services/javax.xml.ws.spi.Provider</code>
		'''   exists, then its first line, if present, is used as the UTF-8 encoded
		'''   name of the implementation class.
		''' </li>
		''' <li>
		'''   If the $java.home/lib/jaxws.properties file exists and it is readable by
		'''   the <code>java.util.Properties.load(InputStream)</code> method and it contains
		'''   an entry whose key is <code>javax.xml.ws.spi.Provider</code>, then the value of
		'''   that entry is used as the name of the implementation class.
		''' </li>
		''' <li>
		'''   If a system property with the name <code>javax.xml.ws.spi.Provider</code>
		'''   is defined, then its value is used as the name of the implementation class.
		''' </li>
		''' <li>
		'''   Finally, a default implementation class name is used.
		''' </li>
		''' </ul>
		''' 
		''' </summary>
		Public Shared Function provider() As Provider
			Try
				Dim ___provider As Object = providerUsingServiceLoader
				If ___provider Is Nothing Then ___provider = FactoryFinder.find(JAXWSPROVIDER_PROPERTY, DEFAULT_JAXWSPROVIDER)
				If Not(TypeOf ___provider Is Provider) Then
					Dim pClass As Type = GetType(Provider)
					Dim classnameAsResource As String = pClass.name.Replace("."c, "/"c) & ".class"
					Dim loader As ClassLoader = pClass.classLoader
					If loader Is Nothing Then loader = ClassLoader.systemClassLoader
					Dim targetTypeURL As java.net.URL = loader.getResource(classnameAsResource)
					Throw New LinkageError("ClassCastException: attempting to cast" & ___provider.GetType().classLoader.getResource(classnameAsResource) & "to" & targetTypeURL.ToString())
				End If
				Return CType(___provider, Provider)
			Catch ex As WebServiceException
				Throw ex
			Catch ex As Exception
				Throw New WebServiceException("Unable to createEndpointReference Provider", ex)
			End Try
		End Function


		Private Property Shared providerUsingServiceLoader As Provider
			Get
				If loadMethod IsNot Nothing Then
					Dim loader As Object
					Try
						loader = loadMethod.invoke(Nothing, GetType(Provider))
					Catch e As Exception
						Throw New WebServiceException("Cannot invoke java.util.ServiceLoader#load()", e)
					End Try
    
					Dim it As IEnumerator(Of Provider)
					Try
						it = CType(iteratorMethod.invoke(loader), IEnumerator(Of Provider))
					Catch e As Exception
						Throw New WebServiceException("Cannot invoke java.util.ServiceLoader#iterator()", e)
					End Try
	'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Return If(it.hasNext(), it.next(), Nothing)
				End If
				Return Nothing
			End Get
		End Property

		''' <summary>
		''' Creates a service delegate object.
		''' <p> </summary>
		''' <param name="wsdlDocumentLocation"> A URL pointing to the WSDL document
		'''        for the service, or <code>null</code> if there isn't one. </param>
		''' <param name="serviceName"> The qualified name of the service. </param>
		''' <param name="serviceClass"> The service class, which MUST be either
		'''        <code>javax.xml.ws.Service</code> or a subclass thereof. </param>
		''' <returns> The newly created service delegate. </returns>
		Public MustOverride Function createServiceDelegate(ByVal wsdlDocumentLocation As java.net.URL, ByVal serviceName As javax.xml.namespace.QName, ByVal serviceClass As Type) As ServiceDelegate

		''' <summary>
		''' Creates a service delegate object.
		''' <p> </summary>
		''' <param name="wsdlDocumentLocation"> A URL pointing to the WSDL document
		'''        for the service, or <code>null</code> if there isn't one. </param>
		''' <param name="serviceName"> The qualified name of the service. </param>
		''' <param name="serviceClass"> The service class, which MUST be either
		'''        <code>javax.xml.ws.Service</code> or a subclass thereof. </param>
		''' <param name="features"> Web Service features that must be configured on
		'''        the service. If the provider doesn't understand a feature,
		'''        it must throw a WebServiceException. </param>
		''' <returns> The newly created service delegate.
		''' 
		''' @since JAX-WS 2.2 </returns>
		Public Overridable Function createServiceDelegate(ByVal wsdlDocumentLocation As java.net.URL, ByVal serviceName As javax.xml.namespace.QName, ByVal serviceClass As Type, ParamArray ByVal features As WebServiceFeature()) As ServiceDelegate
			Throw New System.NotSupportedException("JAX-WS 2.2 implementation must override this default behaviour.")
		End Function


		''' 
		''' <summary>
		''' Creates an endpoint object with the provided binding and implementation
		''' object.
		''' </summary>
		''' <param name="bindingId"> A URI specifying the desired binding (e.g. SOAP/HTTP) </param>
		''' <param name="implementor"> A service implementation object to which
		'''        incoming requests will be dispatched. The corresponding
		'''        class MUST be annotated with all the necessary Web service
		'''        annotations. </param>
		''' <returns> The newly created endpoint. </returns>
		Public MustOverride Function createEndpoint(ByVal bindingId As String, ByVal implementor As Object) As Endpoint


		''' <summary>
		''' Creates and publishes an endpoint object with the specified
		''' address and implementation object.
		''' </summary>
		''' <param name="address"> A URI specifying the address and transport/protocol
		'''        to use. A http: URI MUST result in the SOAP 1.1/HTTP
		'''        binding being used. Implementations may support other
		'''        URI schemes. </param>
		''' <param name="implementor"> A service implementation object to which
		'''        incoming requests will be dispatched. The corresponding
		'''        class MUST be annotated with all the necessary Web service
		'''        annotations. </param>
		''' <returns> The newly created endpoint. </returns>
		Public MustOverride Function createAndPublishEndpoint(ByVal address As String, ByVal implementor As Object) As Endpoint

		''' <summary>
		''' read an EndpointReference from the infoset contained in
		''' <code>eprInfoset</code>.
		''' </summary>
		''' <param name="eprInfoset"> infoset for EndpointReference
		''' </param>
		''' <returns> the <code>EndpointReference</code> unmarshalled from
		''' <code>eprInfoset</code>.  This method never returns <code>null</code>.
		''' </returns>
		''' <exception cref="WebServiceException"> If there is an error creating the
		''' <code>EndpointReference</code> from the specified <code>eprInfoset</code>.
		''' </exception>
		''' <exception cref="NullPointerException"> If the <code>null</code>
		''' <code>eprInfoset</code> value is given.
		''' 
		''' @since JAX-WS 2.1
		'''  </exception>
		Public MustOverride Function readEndpointReference(ByVal eprInfoset As javax.xml.transform.Source) As EndpointReference


		''' <summary>
		''' The getPort method returns a proxy.  If there
		''' are any reference parameters in the
		''' <code>endpointReference</code>, then those reference
		''' parameters MUST appear as SOAP headers, indicating them to be
		''' reference parameters, on all messages sent to the endpoint.
		''' The parameter  <code>serviceEndpointInterface</code> specifies
		''' the service endpoint interface that is supported by the
		''' returned proxy.
		''' The parameter <code>endpointReference</code> specifies the
		''' endpoint that will be invoked by the returned proxy.
		''' In the implementation of this method, the JAX-WS
		''' runtime system takes the responsibility of selecting a protocol
		''' binding (and a port) and configuring the proxy accordingly from
		''' the WSDL metadata of the
		''' <code>serviceEndpointInterface</code> and the <code>EndpointReference</code>.
		''' For this method
		''' to successfully return a proxy, WSDL metadata MUST be available and the
		''' <code>endpointReference</code> MUST contain an implementation understood
		''' <code>serviceName</code> metadata.
		''' 
		''' </summary>
		''' <param name="endpointReference"> the EndpointReference that will
		''' be invoked by the returned proxy. </param>
		''' <param name="serviceEndpointInterface"> Service endpoint interface </param>
		''' <param name="features">  A list of WebServiceFeatures to configure on the
		'''                proxy.  Supported features not in the <code>features
		'''                </code> parameter will have their default values. </param>
		''' <returns> Object Proxy instance that supports the
		'''                  specified service endpoint interface </returns>
		''' <exception cref="WebServiceException">
		'''                  <UL>
		'''                  <LI>If there is an error during creation
		'''                      of the proxy
		'''                  <LI>If there is any missing WSDL metadata
		'''                      as required by this method}
		'''                  <LI>If this
		'''                      <code>endpointReference</code>
		'''                      is illegal
		'''                  <LI>If an illegal
		'''                      <code>serviceEndpointInterface</code>
		'''                      is specified
		'''                  <LI>If a feature is enabled that is not compatible with
		'''                      this port or is unsupported.
		'''                   </UL>
		''' </exception>
		''' <seealso cref= WebServiceFeature
		''' 
		''' @since JAX-WS 2.1
		'''  </seealso>
		Public MustOverride Function getPort(Of T)(ByVal endpointReference As EndpointReference, ByVal serviceEndpointInterface As Type, ParamArray ByVal features As WebServiceFeature()) As T

		''' <summary>
		''' Factory method to create a <code>W3CEndpointReference</code>.
		''' 
		''' <p>
		''' This method can be used to create a <code>W3CEndpointReference</code>
		''' for any endpoint by specifying the <code>address</code> property along
		''' with any other desired properties.  This method
		''' can also be used to create a <code>W3CEndpointReference</code> for
		''' an endpoint that is published by the same Java EE application.
		''' To do so the <code>address</code> property can be provided or this
		''' method can automatically determine the <code>address</code> of
		''' an endpoint that is published by the same Java EE application and is
		''' identified by the <code>serviceName</code> and
		''' <code>portName</code> propeties.  If the <code>address</code> is
		''' <code>null</code> and the <code>serviceName</code> and
		''' <code>portName</code> do not identify an endpoint published by the
		''' same Java EE application, a
		''' <code>javax.lang.IllegalStateException</code> MUST be thrown.
		''' </summary>
		''' <param name="address"> Specifies the address of the target endpoint </param>
		''' <param name="serviceName"> Qualified name of the service in the WSDL. </param>
		''' <param name="portName"> Qualified name of the endpoint in the WSDL. </param>
		''' <param name="metadata"> A list of elements that should be added to the
		''' <code>W3CEndpointReference</code> instances <code>wsa:metadata</code>
		''' element. </param>
		''' <param name="wsdlDocumentLocation"> URL for the WSDL document location for
		''' the service. </param>
		''' <param name="referenceParameters"> Reference parameters to be associated
		''' with the returned <code>EndpointReference</code> instance.
		''' </param>
		''' <returns> the <code>W3CEndpointReference</code> created from
		'''          <code>serviceName</code>, <code>portName</code>,
		'''          <code>metadata</code>, <code>wsdlDocumentLocation</code>
		'''          and <code>referenceParameters</code>. This method
		'''          never returns <code>null</code>.
		''' </returns>
		''' <exception cref="java.lang.IllegalStateException">
		'''     <ul>
		'''        <li>If the <code>address</code>, <code>serviceName</code> and
		'''            <code>portName</code> are all <code>null</code>.
		'''        <li>If the <code>serviceName</code> service is <code>null</code> and the
		'''            <code>portName</code> is NOT <code>null</code>.
		'''        <li>If the <code>address</code> property is <code>null</code> and
		'''            the <code>serviceName</code> and <code>portName</code> do not
		'''            specify a valid endpoint published by the same Java EE
		'''            application.
		'''        <li>If the <code>serviceName</code>is NOT <code>null</code>
		'''             and is not present in the specified WSDL.
		'''        <li>If the <code>portName</code> port is not <code>null</code> and it
		'''             is not present in <code>serviceName</code> service in the WSDL.
		'''        <li>If the <code>wsdlDocumentLocation</code> is NOT <code>null</code>
		'''            and does not represent a valid WSDL.
		'''     </ul> </exception>
		''' <exception cref="WebServiceException"> If an error occurs while creating the
		'''                             <code>W3CEndpointReference</code>.
		''' 
		''' @since JAX-WS 2.1 </exception>
		Public MustOverride Function createW3CEndpointReference(ByVal address As String, ByVal serviceName As javax.xml.namespace.QName, ByVal portName As javax.xml.namespace.QName, ByVal metadata As IList(Of org.w3c.dom.Element), ByVal wsdlDocumentLocation As String, ByVal referenceParameters As IList(Of org.w3c.dom.Element)) As javax.xml.ws.wsaddressing.W3CEndpointReference


		''' <summary>
		''' Factory method to create a <code>W3CEndpointReference</code>.
		''' Using this method, a <code>W3CEndpointReference</code> instance
		''' can be created with extension elements, and attributes.
		''' <code>Provider</code> implementations must override the default
		''' implementation.
		''' 
		''' <p>
		''' This method can be used to create a <code>W3CEndpointReference</code>
		''' for any endpoint by specifying the <code>address</code> property along
		''' with any other desired properties.  This method
		''' can also be used to create a <code>W3CEndpointReference</code> for
		''' an endpoint that is published by the same Java EE application.
		''' To do so the <code>address</code> property can be provided or this
		''' method can automatically determine the <code>address</code> of
		''' an endpoint that is published by the same Java EE application and is
		''' identified by the <code>serviceName</code> and
		''' <code>portName</code> propeties.  If the <code>address</code> is
		''' <code>null</code> and the <code>serviceName</code> and
		''' <code>portName</code> do not identify an endpoint published by the
		''' same Java EE application, a
		''' <code>javax.lang.IllegalStateException</code> MUST be thrown.
		''' </summary>
		''' <param name="address"> Specifies the address of the target endpoint </param>
		''' <param name="interfaceName"> the <code>wsam:InterfaceName</code> element in the
		''' <code>wsa:Metadata</code> element. </param>
		''' <param name="serviceName"> Qualified name of the service in the WSDL. </param>
		''' <param name="portName"> Qualified name of the endpoint in the WSDL. </param>
		''' <param name="metadata"> A list of elements that should be added to the
		''' <code>W3CEndpointReference</code> instances <code>wsa:metadata</code>
		''' element. </param>
		''' <param name="wsdlDocumentLocation"> URL for the WSDL document location for
		''' the service. </param>
		''' <param name="referenceParameters"> Reference parameters to be associated
		''' with the returned <code>EndpointReference</code> instance. </param>
		''' <param name="elements"> extension elements to be associated
		''' with the returned <code>EndpointReference</code> instance. </param>
		''' <param name="attributes"> extension attributes to be associated
		''' with the returned <code>EndpointReference</code> instance.
		''' </param>
		''' <returns> the <code>W3CEndpointReference</code> created from
		'''          <code>serviceName</code>, <code>portName</code>,
		'''          <code>metadata</code>, <code>wsdlDocumentLocation</code>
		'''          and <code>referenceParameters</code>. This method
		'''          never returns <code>null</code>.
		''' </returns>
		''' <exception cref="java.lang.IllegalStateException">
		'''     <ul>
		'''        <li>If the <code>address</code>, <code>serviceName</code> and
		'''            <code>portName</code> are all <code>null</code>.
		'''        <li>If the <code>serviceName</code> service is <code>null</code> and the
		'''            <code>portName</code> is NOT <code>null</code>.
		'''        <li>If the <code>address</code> property is <code>null</code> and
		'''            the <code>serviceName</code> and <code>portName</code> do not
		'''            specify a valid endpoint published by the same Java EE
		'''            application.
		'''        <li>If the <code>serviceName</code>is NOT <code>null</code>
		'''             and is not present in the specified WSDL.
		'''        <li>If the <code>portName</code> port is not <code>null</code> and it
		'''             is not present in <code>serviceName</code> service in the WSDL.
		'''        <li>If the <code>wsdlDocumentLocation</code> is NOT <code>null</code>
		'''            and does not represent a valid WSDL.
		'''        <li>If the <code>wsdlDocumentLocation</code> is NOT <code>null</code> but
		'''            wsdli:wsdlLocation's namespace name cannot be got from the available
		'''            metadata.
		'''     </ul> </exception>
		''' <exception cref="WebServiceException"> If an error occurs while creating the
		'''                             <code>W3CEndpointReference</code>.
		''' @since JAX-WS 2.2 </exception>
		Public Overridable Function createW3CEndpointReference(ByVal address As String, ByVal interfaceName As javax.xml.namespace.QName, ByVal serviceName As javax.xml.namespace.QName, ByVal portName As javax.xml.namespace.QName, ByVal metadata As IList(Of org.w3c.dom.Element), ByVal wsdlDocumentLocation As String, ByVal referenceParameters As IList(Of org.w3c.dom.Element), ByVal elements As IList(Of org.w3c.dom.Element), ByVal attributes As IDictionary(Of javax.xml.namespace.QName, String)) As javax.xml.ws.wsaddressing.W3CEndpointReference
			Throw New System.NotSupportedException("JAX-WS 2.2 implementation must override this default behaviour.")
		End Function

		''' <summary>
		''' Creates and publishes an endpoint object with the specified
		''' address, implementation object and web service features.
		''' <code>Provider</code> implementations must override the
		''' default implementation.
		''' </summary>
		''' <param name="address"> A URI specifying the address and transport/protocol
		'''        to use. A http: URI MUST result in the SOAP 1.1/HTTP
		'''        binding being used. Implementations may support other
		'''        URI schemes. </param>
		''' <param name="implementor"> A service implementation object to which
		'''        incoming requests will be dispatched. The corresponding
		'''        class MUST be annotated with all the necessary Web service
		'''        annotations. </param>
		''' <param name="features"> A list of WebServiceFeatures to configure on the
		'''        endpoint.  Supported features not in the <code>features
		'''        </code> parameter will have their default values. </param>
		''' <returns> The newly created endpoint.
		''' @since JAX-WS 2.2 </returns>
		Public Overridable Function createAndPublishEndpoint(ByVal address As String, ByVal implementor As Object, ParamArray ByVal features As WebServiceFeature()) As Endpoint
			Throw New System.NotSupportedException("JAX-WS 2.2 implementation must override this default behaviour.")
		End Function

		''' <summary>
		''' Creates an endpoint object with the provided binding, implementation
		''' object and web service features. <code>Provider</code> implementations
		''' must override the default implementation.
		''' </summary>
		''' <param name="bindingId"> A URI specifying the desired binding (e.g. SOAP/HTTP) </param>
		''' <param name="implementor"> A service implementation object to which
		'''        incoming requests will be dispatched. The corresponding
		'''        class MUST be annotated with all the necessary Web service
		'''        annotations. </param>
		''' <param name="features"> A list of WebServiceFeatures to configure on the
		'''        endpoint.  Supported features not in the <code>features
		'''        </code> parameter will have their default values. </param>
		''' <returns> The newly created endpoint.
		''' @since JAX-WS 2.2 </returns>
		Public Overridable Function createEndpoint(ByVal bindingId As String, ByVal implementor As Object, ParamArray ByVal features As WebServiceFeature()) As Endpoint
			Throw New System.NotSupportedException("JAX-WS 2.2 implementation must override this default behaviour.")
		End Function

		''' <summary>
		''' Creates an endpoint object with the provided binding, implementation
		''' class, invoker and web service features. Containers typically use
		''' this to create Endpoint objects. <code>Provider</code>
		''' implementations must override the default implementation.
		''' </summary>
		''' <param name="bindingId"> A URI specifying the desired binding (e.g. SOAP/HTTP).
		'''        Can be null. </param>
		''' <param name="implementorClass"> A service implementation class that
		'''        MUST be annotated with all the necessary Web service
		'''        annotations. </param>
		''' <param name="invoker"> that does the actual invocation on the service instance. </param>
		''' <param name="features"> A list of WebServiceFeatures to configure on the
		'''        endpoint.  Supported features not in the <code>features
		'''        </code> parameter will have their default values. </param>
		''' <returns> The newly created endpoint.
		''' @since JAX-WS 2.2 </returns>
		Public Overridable Function createEndpoint(ByVal bindingId As String, ByVal implementorClass As Type, ByVal invoker As Invoker, ParamArray ByVal features As WebServiceFeature()) As Endpoint
			Throw New System.NotSupportedException("JAX-WS 2.2 implementation must override this default behaviour.")
		End Function

	End Class

End Namespace
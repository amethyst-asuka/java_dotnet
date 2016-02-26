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

Namespace javax.xml.ws.wsaddressing





	''' <summary>
	''' This class is used to build <code>W3CEndpointReference</code>
	''' instances. The intended use of this clsss is for
	''' an application component, for example a factory component,
	''' to create an <code>W3CEndpointReference</code> for a
	''' web service endpoint published by the same
	''' Java EE application. It can also be used to create
	''' <code>W3CEndpointReferences</code> for an Java SE based
	''' endpoint by providing the <code>address</code> property.
	''' <p>
	''' When creating a <code>W3CEndpointReference</code> for an
	''' endpoint that is not published by the same Java EE application,
	''' the <code>address</code> property MUST be specified.
	''' <p>
	''' When creating a <code>W3CEndpointReference</code> for an endpoint
	''' published by the same Java EE application, the <code>address</code>
	''' property MAY be <code>null</code> but then the <code>serviceName</code>
	''' and <code>endpointName</code> MUST specify an endpoint published by
	''' the same Java EE application.
	''' <p>
	''' When the <code>wsdlDocumentLocation</code> is specified it MUST refer
	''' to a valid WSDL document and the <code>serviceName</code> and
	''' <code>endpointName</code> (if specified) MUST match a service and port
	''' in the WSDL document.
	''' 
	''' @since JAX-WS 2.1
	''' </summary>
	Public NotInheritable Class W3CEndpointReferenceBuilder
		''' <summary>
		''' Creates a new <code>W3CEndpointReferenceBuilder</code> instance.
		''' </summary>
		Public Sub New()
			referenceParameters = New List(Of org.w3c.dom.Element)
			___metadata = New List(Of org.w3c.dom.Element)
			attributes = New Dictionary(Of javax.xml.namespace.QName, String)
			elements = New List(Of org.w3c.dom.Element)
		End Sub

		''' <summary>
		''' Sets the <code>address</code> to the
		''' <code>W3CEndpointReference</code> instance's
		''' <code>wsa:Address</code>.
		''' <p>
		''' The <code>address</code> MUST be set to a non-<code>null</code>
		''' value when building a <code>W3CEndpointReference</code> for a
		''' web service endpoint that is not published by the same
		''' Java EE application or when running on Java SE.
		''' </summary>
		''' <param name="address"> The address of the endpoint to be targeted
		'''      by the returned <code>W3CEndpointReference</code>.
		''' </param>
		''' <returns> A <code>W3CEndpointReferenceBuilder</code> instance with
		'''   the <code>address</code> set to the <code>wsa:Address</code>. </returns>
		Public Function address(ByVal ___address As String) As W3CEndpointReferenceBuilder
			Me.___address = ___address
			Return Me
		End Function

		''' <summary>
		''' Sets the <code>interfaceName</code> as the
		''' <code>wsam:InterfaceName</code> element in the
		''' <code>wsa:Metadata</code> element.
		''' 
		''' See <a href="http://www.w3.org/TR/2007/REC-ws-addr-metadata-20070904/#refmetadatfromepr">
		''' 2.1 Referencing WSDL Metadata from an EPR</a> for more details.
		''' </summary>
		''' <param name="interfaceName"> The port type name of the endpoint to be targeted
		'''      by the returned <code>W3CEndpointReference</code>.
		''' </param>
		''' <returns> A <code>W3CEndpointReferenceBuilder</code> instance with
		'''   the <code>interfaceName</code> as <code>wsam:InterfaceName</code>
		'''   element added to the <code>wsa:Metadata</code> element </returns>
		Public Function interfaceName(ByVal ___interfaceName As javax.xml.namespace.QName) As W3CEndpointReferenceBuilder
			Me.___interfaceName = ___interfaceName
			Return Me
		End Function

		''' <summary>
		''' Sets the <code>serviceName</code> as the
		''' <code>wsam:ServiceName</code> element in the
		''' <code>wsa:Metadata</code> element.
		''' 
		''' See <a href="http://www.w3.org/TR/2007/REC-ws-addr-metadata-20070904/#refmetadatfromepr">
		''' 2.1 Referencing WSDL Metadata from an EPR</a> for more details.
		''' </summary>
		''' <param name="serviceName"> The service name of the endpoint to be targeted
		'''      by the returned <code>W3CEndpointReference</code>.  This property
		'''      may also be used with the <code>endpointName</code> (portName)
		'''      property to lookup the <code>address</code> of a web service
		'''      endpoint that is published by the same Java EE application.
		''' </param>
		''' <returns> A <code>W3CEndpointReferenceBuilder</code> instance with
		'''   the <code>serviceName</code> as <code>wsam:ServiceName</code>
		'''   element added to the <code>wsa:Metadata</code> element
		'''  </returns>
		Public Function serviceName(ByVal ___serviceName As javax.xml.namespace.QName) As W3CEndpointReferenceBuilder
			Me.___serviceName = ___serviceName
			Return Me
		End Function

		''' <summary>
		''' Sets the <code>endpointName</code> as
		''' <code>wsam:ServiceName/@EndpointName</code> in the
		''' <code>wsa:Metadata</code> element. This method can only be called
		''' after the <seealso cref="#serviceName"/> method has been called.
		''' <p>
		''' See <a href="http://www.w3.org/TR/2007/REC-ws-addr-metadata-20070904/#refmetadatfromepr">
		''' 2.1 Referencing WSDL Metadata from an EPR</a> for more details.
		''' </summary>
		''' <param name="endpointName"> The name of the endpoint to be targeted
		'''      by the returned <code>W3CEndpointReference</code>. The
		'''      <code>endpointName</code> (portName) property may also be
		'''      used with the <code>serviceName</code> property to lookup
		'''      the <code>address</code> of a web service
		'''      endpoint published by the same Java EE application.
		''' </param>
		''' <returns> A <code>W3CEndpointReferenceBuilder</code> instance with
		'''   the <code>endpointName</code> as
		''' <code>wsam:ServiceName/@EndpointName</code> in the
		''' <code>wsa:Metadata</code> element.
		''' </returns>
		''' <exception cref="IllegalStateException">, if the <code>serviceName</code>
		''' has not been set. </exception>
		''' <exception cref="IllegalArgumentException">, if the <code>endpointName</code>'s
		''' Namespace URI doesn't match <code>serviceName</code>'s Namespace URI
		'''  </exception>
		Public Function endpointName(ByVal ___endpointName As javax.xml.namespace.QName) As W3CEndpointReferenceBuilder
			If ___serviceName Is Nothing Then Throw New IllegalStateException("The W3CEndpointReferenceBuilder's serviceName must be set before setting the endpointName: " & ___endpointName)

			Me.___endpointName = ___endpointName
			Return Me
		End Function

		''' <summary>
		''' Sets the <code>wsdlDocumentLocation</code> that will be referenced
		''' as <code>wsa:Metadata/@wsdli:wsdlLocation</code>. The namespace name
		''' for the wsdli:wsdlLocation's value can be taken from the WSDL itself.
		''' 
		''' <p>
		''' See <a href="http://www.w3.org/TR/2007/REC-ws-addr-metadata-20070904/#refmetadatfromepr">
		''' 2.1 Referencing WSDL Metadata from an EPR</a> for more details.
		''' </summary>
		''' <param name="wsdlDocumentLocation"> The location of the WSDL document to
		'''      be referenced in the <code>wsa:Metadata</code> of the
		'''     <code>W3CEndpointReference</code>. </param>
		''' <returns> A <code>W3CEndpointReferenceBuilder</code> instance with
		'''   the <code>wsdlDocumentLocation</code> that is to be referenced. </returns>
		Public Function wsdlDocumentLocation(ByVal ___wsdlDocumentLocation As String) As W3CEndpointReferenceBuilder
			Me.___wsdlDocumentLocation = ___wsdlDocumentLocation
			Return Me
		End Function

		''' <summary>
		''' Adds the <code>referenceParameter</code> to the
		''' <code>W3CEndpointReference</code> instance
		''' <code>wsa:ReferenceParameters</code> element.
		''' </summary>
		''' <param name="referenceParameter"> The element to be added to the
		'''      <code>wsa:ReferenceParameters</code> element.
		''' </param>
		''' <returns> A <code>W3CEndpointReferenceBuilder</code> instance with
		'''   the <code>referenceParameter</code> added to the
		'''   <code>wsa:ReferenceParameters</code> element.
		''' </returns>
		''' <exception cref="java.lang.IllegalArgumentException"> if <code>referenceParameter</code>
		''' is <code>null</code>. </exception>
		Public Function referenceParameter(ByVal ___referenceParameter As org.w3c.dom.Element) As W3CEndpointReferenceBuilder
			If ___referenceParameter Is Nothing Then Throw New System.ArgumentException("The referenceParameter cannot be null.")
			referenceParameters.Add(___referenceParameter)
			Return Me
		End Function

		''' <summary>
		''' Adds the <code>metadataElement</code> to the
		''' <code>W3CEndpointReference</code> instance's
		''' <code>wsa:Metadata</code> element.
		''' </summary>
		''' <param name="metadataElement"> The element to be added to the
		'''      <code>wsa:Metadata</code> element.
		''' </param>
		''' <returns> A <code>W3CEndpointReferenceBuilder</code> instance with
		'''   the <code>metadataElement</code> added to the
		'''    <code>wsa:Metadata</code> element.
		''' </returns>
		''' <exception cref="java.lang.IllegalArgumentException"> if <code>metadataElement</code>
		''' is <code>null</code>. </exception>
		Public Function metadata(ByVal metadataElement As org.w3c.dom.Element) As W3CEndpointReferenceBuilder
			If metadataElement Is Nothing Then Throw New System.ArgumentException("The metadataElement cannot be null.")
			___metadata.Add(metadataElement)
			Return Me
		End Function

		''' <summary>
		''' Adds an extension element to the
		''' <code>W3CEndpointReference</code> instance's
		''' <code>wsa:EndpointReference</code> element.
		''' </summary>
		''' <param name="element"> The extension element to be added to the
		'''   <code>W3CEndpointReference</code> </param>
		''' <returns> A <code>W3CEndpointReferenceBuilder</code> instance with
		'''   the extension <code>element</code> added to the
		'''    <code>W3CEndpointReference</code> instance. </returns>
		''' <exception cref="java.lang.IllegalArgumentException"> if <code>element</code>
		''' is <code>null</code>.
		''' 
		''' @since JAX-WS 2.2 </exception>
		Public Function element(ByVal ___element As org.w3c.dom.Element) As W3CEndpointReferenceBuilder
			If ___element Is Nothing Then Throw New System.ArgumentException("The extension element cannot be null.")
			elements.Add(___element)
			Return Me
		End Function

		''' <summary>
		''' Adds an extension attribute to the
		''' <code>W3CEndpointReference</code> instance's
		''' <code>wsa:EndpointReference</code> element.
		''' </summary>
		''' <param name="name"> The name of the extension attribute to be added to the
		'''   <code>W3CEndpointReference</code> </param>
		''' <param name="value"> extension attribute value </param>
		''' <returns> A <code>W3CEndpointReferenceBuilder</code> instance with
		'''   the extension attribute added to the <code>W3CEndpointReference</code>
		'''   instance. </returns>
		''' <exception cref="java.lang.IllegalArgumentException"> if <code>name</code>
		'''   or <code>value</code> is <code>null</code>.
		''' 
		''' @since JAX-WS 2.2 </exception>
		Public Function attribute(ByVal name As javax.xml.namespace.QName, ByVal value As String) As W3CEndpointReferenceBuilder
			If name Is Nothing OrElse value Is Nothing Then Throw New System.ArgumentException("The extension attribute name or value cannot be null.")
			attributes(name) = value
			Return Me
		End Function

		''' <summary>
		''' Builds a <code>W3CEndpointReference</code> from the accumulated
		''' properties set on this <code>W3CEndpointReferenceBuilder</code>
		''' instance.
		''' <p>
		''' This method can be used to create a <code>W3CEndpointReference</code>
		''' for any endpoint by specifying the <code>address</code> property along
		''' with any other desired properties.  This method
		''' can also be used to create a <code>W3CEndpointReference</code> for
		''' an endpoint that is published by the same Java EE application.
		''' This method can automatically determine the <code>address</code> of
		''' an endpoint published by the same Java EE application that is identified by the
		''' <code>serviceName</code> and
		''' <code>endpointName</code> properties.  If the <code>address</code> is
		''' <code>null</code> and the <code>serviceName</code> and
		''' <code>endpointName</code>
		''' do not identify an endpoint published by the same Java EE application, a
		''' <code>java.lang.IllegalStateException</code> MUST be thrown.
		''' 
		''' </summary>
		''' <returns> <code>W3CEndpointReference</code> from the accumulated
		''' properties set on this <code>W3CEndpointReferenceBuilder</code>
		''' instance. This method never returns <code>null</code>.
		''' </returns>
		''' <exception cref="IllegalStateException">
		'''     <ul>
		'''        <li>If the <code>address</code>, <code>serviceName</code> and
		'''            <code>endpointName</code> are all <code>null</code>.
		'''        <li>If the <code>serviceName</code> service is <code>null</code> and the
		'''            <code>endpointName</code> is NOT <code>null</code>.
		'''        <li>If the <code>address</code> property is <code>null</code> and
		'''            the <code>serviceName</code> and <code>endpointName</code> do not
		'''            specify a valid endpoint published by the same Java EE
		'''            application.
		'''        <li>If the <code>serviceName</code> is NOT <code>null</code>
		'''             and is not present in the specified WSDL.
		'''        <li>If the <code>endpointName</code> port is not <code>null</code> and it
		'''             is not present in <code>serviceName</code> service in the WSDL.
		'''        <li>If the <code>wsdlDocumentLocation</code> is NOT <code>null</code>
		'''            and does not represent a valid WSDL.
		'''     </ul> </exception>
		''' <exception cref="WebServiceException"> If an error occurs while creating the
		'''                             <code>W3CEndpointReference</code>.
		'''  </exception>
		Public Function build() As W3CEndpointReference
			If elements.Count = 0 AndAlso attributes.Count = 0 AndAlso ___interfaceName Is Nothing Then Return javax.xml.ws.spi.Provider.provider().createW3CEndpointReference(___address, ___serviceName, ___endpointName, ___metadata, ___wsdlDocumentLocation, referenceParameters)
			Return javax.xml.ws.spi.Provider.provider().createW3CEndpointReference(___address, ___interfaceName, ___serviceName, ___endpointName, ___metadata, ___wsdlDocumentLocation, referenceParameters, elements, attributes)
		End Function

		Private ___address As String
		Private referenceParameters As IList(Of org.w3c.dom.Element)
		Private ___metadata As IList(Of org.w3c.dom.Element)
		Private ___interfaceName As javax.xml.namespace.QName
		Private ___serviceName As javax.xml.namespace.QName
		Private ___endpointName As javax.xml.namespace.QName
		Private ___wsdlDocumentLocation As String
		Private attributes As IDictionary(Of javax.xml.namespace.QName, String)
		Private elements As IList(Of org.w3c.dom.Element)
	End Class

End Namespace
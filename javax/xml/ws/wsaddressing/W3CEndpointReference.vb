Imports System.Collections.Generic

'
' * Copyright (c) 2005, 2014, Oracle and/or its affiliates. All rights reserved.
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
	''' This class represents a W3C Addressing EndpointReferece which is
	''' a remote reference to a web service endpoint that supports the
	''' W3C WS-Addressing 1.0 - Core Recommendation.
	''' <p>
	''' Developers should use this class in their SEIs if they want to
	''' pass/return endpoint references that represent the W3C WS-Addressing
	''' recommendation.
	''' <p>
	''' JAXB will use the JAXB annotations and bind this class to XML infoset
	''' that is consistent with that defined by WS-Addressing.  See
	''' <a href="http://www.w3.org/TR/2006/REC-ws-addr-core-20060509/">
	''' WS-Addressing</a>
	''' for more information on WS-Addressing EndpointReferences.
	''' 
	''' @since JAX-WS 2.1
	''' </summary>

	' XmlRootElement allows this class to be marshalled on its own
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	Public NotInheritable Class W3CEndpointReference
		Inherits javax.xml.ws.EndpointReference

		Private ReadOnly w3cjc As javax.xml.bind.JAXBContext = w3CJaxbContext

		' should be changed to package private, keeping original modifier to keep backwards compatibility
		Protected Friend Const NS As String = "http://www.w3.org/2005/08/addressing"

		' default constructor forbidden ...
		' should be private, keeping original modifier to keep backwards compatibility
		Protected Friend Sub New()
		End Sub

		''' <summary>
		''' Creates an EPR from infoset representation
		''' </summary>
		''' <param name="source"> A source object containing valid XmlInfoset
		''' instance consistent with the W3C WS-Addressing Core
		''' recommendation.
		''' </param>
		''' <exception cref="WebServiceException">
		'''   If the source does NOT contain a valid W3C WS-Addressing
		'''   EndpointReference. </exception>
		''' <exception cref="NullPointerException">
		'''   If the <code>null</code> <code>source</code> value is given </exception>
		Public Sub New(ByVal source As javax.xml.transform.Source)
			Try
				Dim epr As W3CEndpointReference = w3cjc.createUnmarshaller().unmarshal(source,GetType(W3CEndpointReference)).value
				Me.address = epr.address
				Me.metadata = epr.metadata
				Me.referenceParameters = epr.referenceParameters
				Me.elements = epr.elements
				Me.attributes = epr.attributes
			Catch e As javax.xml.bind.JAXBException
				Throw New javax.xml.ws.WebServiceException("Error unmarshalling W3CEndpointReference ",e)
			Catch e As ClassCastException
				Throw New javax.xml.ws.WebServiceException("Source did not contain W3CEndpointReference", e)
			End Try
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Sub writeTo(ByVal result As javax.xml.transform.Result)
			Try
				Dim marshaller As javax.xml.bind.Marshaller = w3cjc.createMarshaller()
				marshaller.marshal(Me, result)
			Catch e As javax.xml.bind.JAXBException
				Throw New javax.xml.ws.WebServiceException("Error marshalling W3CEndpointReference. ", e)
			End Try
		End Sub

		Private Property Shared w3CJaxbContext As javax.xml.bind.JAXBContext
			Get
				Try
					Return javax.xml.bind.JAXBContext.newInstance(GetType(W3CEndpointReference))
				Catch e As javax.xml.bind.JAXBException
					Throw New javax.xml.ws.WebServiceException("Error creating JAXBContext for W3CEndpointReference. ", e)
				End Try
			End Get
		End Property

		' private but necessary properties for databinding
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Private address As Address
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Private referenceParameters As Elements
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Private metadata As Elements
		' attributes and elements are not private for performance reasons
		' (JAXB can bypass reflection)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend attributes As IDictionary(Of javax.xml.namespace.QName, String)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Friend elements As IList(Of org.w3c.dom.Element)


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Private Class Address
			Protected Friend Sub New()
			End Sub
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Friend uri As String
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Friend attributes As IDictionary(Of javax.xml.namespace.QName, String)
		End Class


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Private Class Elements
			Protected Friend Sub New()
			End Sub
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Friend elements As IList(Of org.w3c.dom.Element)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
			Friend attributes As IDictionary(Of javax.xml.namespace.QName, String)
		End Class

	End Class

End Namespace
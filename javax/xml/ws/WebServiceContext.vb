Imports System

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
	'''  A <code>WebServiceContext</code> makes it possible for
	'''  a web service endpoint implementation class to access
	'''  message context and security information relative to
	'''  a request being served.
	''' 
	'''  Typically a <code>WebServiceContext</code> is injected
	'''  into an endpoint implementation class using the
	'''  <code>Resource</code> annotation.
	''' 
	'''  @since JAX-WS 2.0
	''' </summary>
	'''  <seealso cref= javax.annotation.Resource
	'''  </seealso>
	Public Interface WebServiceContext

		''' <summary>
		''' Returns the <code>MessageContext</code> for the request being served
		''' at the time this method is called. Only properties with
		''' APPLICATION scope will be visible to the application.
		''' </summary>
		''' <returns> MessageContext The message context.
		''' </returns>
		''' <exception cref="IllegalStateException"> This exception is thrown
		'''         if the method is called while no request is
		'''         being serviced.
		''' </exception>
		''' <seealso cref= javax.xml.ws.handler.MessageContext </seealso>
		''' <seealso cref= javax.xml.ws.handler.MessageContext.Scope </seealso>
		''' <seealso cref= java.lang.IllegalStateException
		'''  </seealso>
		ReadOnly Property messageContext As javax.xml.ws.handler.MessageContext

		''' <summary>
		''' Returns the Principal that identifies the sender
		''' of the request currently being serviced. If the
		''' sender has not been authenticated, the method
		''' returns <code>null</code>.
		''' </summary>
		''' <returns> Principal The principal object.
		''' </returns>
		''' <exception cref="IllegalStateException"> This exception is thrown
		'''         if the method is called while no request is
		'''         being serviced.
		''' </exception>
		''' <seealso cref= java.security.Principal </seealso>
		''' <seealso cref= java.lang.IllegalStateException
		'''  </seealso>
		ReadOnly Property userPrincipal As java.security.Principal

		''' <summary>
		''' Returns a boolean indicating whether the
		''' authenticated user is included in the specified
		''' logical role. If the user has not been
		''' authenticated, the method returns <code>false</code>.
		''' </summary>
		''' <param name="role">  A <code>String</code> specifying the name of the role
		''' </param>
		''' <returns> a <code>boolean</code> indicating whether
		''' the sender of the request belongs to a given role
		''' </returns>
		''' <exception cref="IllegalStateException"> This exception is thrown
		'''         if the method is called while no request is
		'''         being serviced.
		'''  </exception>
		Function isUserInRole(ByVal role As String) As Boolean

		''' <summary>
		''' Returns the <code>EndpointReference</code> for this
		''' endpoint.
		''' <p>
		''' If the <seealso cref="Binding"/> for this <code>bindingProvider</code> is
		''' either SOAP1.1/HTTP or SOAP1.2/HTTP, then a
		''' <code>W3CEndpointReference</code> MUST be returned.
		''' </summary>
		''' <param name="referenceParameters"> Reference parameters to be associated with the
		''' returned <code>EndpointReference</code> instance. </param>
		''' <returns> EndpointReference of the endpoint associated with this
		''' <code>WebServiceContext</code>.
		''' If the returned <code>EndpointReference</code> is of type
		''' <code>W3CEndpointReference</code> then it MUST contain the
		''' the specified <code>referenceParameters</code>.
		''' </returns>
		''' <exception cref="IllegalStateException"> This exception is thrown
		'''         if the method is called while no request is
		'''         being serviced.
		''' </exception>
		''' <seealso cref= W3CEndpointReference
		''' 
		''' @since JAX-WS 2.1 </seealso>
		Function getEndpointReference(ParamArray ByVal referenceParameters As org.w3c.dom.Element()) As EndpointReference

		''' <summary>
		''' Returns the <code>EndpointReference</code> associated with
		''' this endpoint.
		''' </summary>
		''' <param name="clazz"> The type of <code>EndpointReference</code> that
		''' MUST be returned. </param>
		''' <param name="referenceParameters"> Reference parameters to be associated with the
		''' returned <code>EndpointReference</code> instance. </param>
		''' <returns> EndpointReference of type <code>clazz</code> of the endpoint
		''' associated with this <code>WebServiceContext</code> instance.
		''' If the returned <code>EndpointReference</code> is of type
		''' <code>W3CEndpointReference</code> then it MUST contain the
		''' the specified <code>referenceParameters</code>.
		''' </returns>
		''' <exception cref="IllegalStateException"> This exception is thrown
		'''         if the method is called while no request is
		'''         being serviced. </exception>
		''' <exception cref="WebServiceException"> If the <code>clazz</code> type of
		''' <code>EndpointReference</code> is not supported.
		''' 
		''' @since JAX-WS 2.1
		'''  </exception>
		 Function getEndpointReference(Of T As EndpointReference)(ByVal clazz As Type, ParamArray ByVal referenceParameters As org.w3c.dom.Element()) As T
	End Interface

End Namespace
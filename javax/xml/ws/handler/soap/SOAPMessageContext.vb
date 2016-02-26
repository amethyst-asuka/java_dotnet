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

Namespace javax.xml.ws.handler.soap


	''' <summary>
	''' The interface <code>SOAPMessageContext</code>
	'''  provides access to the SOAP message for either RPC request or
	'''  response. The <code>javax.xml.soap.SOAPMessage</code> specifies
	'''  the standard Java API for the representation of a SOAP 1.1 message
	'''  with attachments.
	''' </summary>
	'''  <seealso cref= javax.xml.soap.SOAPMessage
	''' 
	'''  @since JAX-WS 2.0
	'''  </seealso>
	Public Interface SOAPMessageContext
		Inherits javax.xml.ws.handler.MessageContext

	  ''' <summary>
	  ''' Gets the <code>SOAPMessage</code> from this message context. Modifications
	  '''  to the returned <code>SOAPMessage</code> change the message in-place, there
	  '''  is no need to subsequently call <code>setMessage</code>.
	  ''' </summary>
	  '''  <returns> Returns the <code>SOAPMessage</code>; returns <code>null</code> if no
	  '''          <code>SOAPMessage</code> is present in this message context
	  '''  </returns>
	  Property message As javax.xml.soap.SOAPMessage


	  ''' <summary>
	  ''' Gets headers that have a particular qualified name from the message in the
	  '''  message context. Note that a SOAP message can contain multiple headers
	  '''  with the same qualified name.
	  ''' </summary>
	  '''  <param name="header"> The XML qualified name of the SOAP header(s). </param>
	  '''  <param name="context"> The JAXBContext that should be used to unmarshall the
	  '''          header </param>
	  '''  <param name="allRoles"> If <code>true</code> then returns headers for all SOAP
	  '''          roles, if <code>false</code> then only returns headers targetted
	  '''          at the roles currently being played by this SOAP node, see
	  '''          <code>getRoles</code>. </param>
	  '''  <returns> An array of unmarshalled headers; returns an empty array if no
	  '''          message is present in this message context or no headers match
	  '''          the supplied qualified name. </returns>
	  '''  <exception cref="WebServiceException"> If an error occurs when using the supplied
	  '''     <code>JAXBContext</code> to unmarshall. The cause of
	  '''     the <code>WebServiceException</code> is the original <code>JAXBException</code>.
	  '''  </exception>
	  Function getHeaders(ByVal header As javax.xml.namespace.QName, ByVal context As javax.xml.bind.JAXBContext, ByVal allRoles As Boolean) As Object()

	  ''' <summary>
	  ''' Gets the SOAP actor roles associated with an execution
	  '''  of the handler chain.
	  '''  Note that SOAP actor roles apply to the SOAP node and
	  '''  are managed using <seealso cref="javax.xml.ws.soap.SOAPBinding#setRoles"/> and
	  '''  <seealso cref="javax.xml.ws.soap.SOAPBinding#getRoles"/>. <code>Handler</code> instances in
	  '''  the handler chain use this information about the SOAP actor
	  '''  roles to process the SOAP header blocks. Note that the
	  '''  SOAP actor roles are invariant during the processing of
	  '''  SOAP message through the handler chain.
	  ''' </summary>
	  '''  <returns> Array of <code>String</code> for SOAP actor roles
	  '''  </returns>
	  ReadOnly Property roles As java.util.Set(Of String)
	End Interface

End Namespace
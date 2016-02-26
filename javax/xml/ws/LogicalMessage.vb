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

Namespace javax.xml.ws


	''' <summary>
	''' The <code>LogicalMessage</code> interface represents a
	'''  protocol agnostic XML message and contains methods that
	'''  provide access to the payload of the message.
	''' 
	'''  @since JAX-WS 2.0
	''' 
	''' </summary>
	Public Interface LogicalMessage

	  ''' <summary>
	  ''' Gets the message payload as an XML source, may be called
	  '''  multiple times on the same LogicalMessage instance, always
	  '''  returns a new <code>Source</code> that may be used to retrieve the entire
	  '''  message payload.
	  ''' 
	  '''  <p>If the returned <code>Source</code> is an instance of
	  '''  <code>DOMSource</code>, then
	  '''  modifications to the encapsulated DOM tree change the message
	  '''  payload in-place, there is no need to susequently call
	  '''  <code>setPayload</code>. Other types of <code>Source</code> provide only
	  '''  read access to the message payload.
	  ''' </summary>
	  '''  <returns> The contained message payload; returns <code>null</code> if no
	  '''          payload is present in this message.
	  '''  </returns>
	  Property payload As javax.xml.transform.Source


	  ''' <summary>
	  ''' Gets the message payload as a JAXB object. Note that there is no
	  '''  connection between the returned object and the message payload,
	  '''  changes to the payload require calling <code>setPayload</code>.
	  ''' </summary>
	  '''  <param name="context"> The JAXBContext that should be used to unmarshall
	  '''          the message payload </param>
	  '''  <returns> The contained message payload; returns <code>null</code> if no
	  '''          payload is present in this message </returns>
	  '''  <exception cref="WebServiceException"> If an error occurs when using a supplied
	  '''     JAXBContext to unmarshall the payload. The cause of
	  '''     the WebServiceException is the original JAXBException.
	  '''  </exception>
	  Function getPayload(ByVal context As javax.xml.bind.JAXBContext) As Object

	  ''' <summary>
	  ''' Sets the message payload
	  ''' </summary>
	  '''  <param name="payload"> message payload </param>
	  '''  <param name="context"> The JAXBContext that should be used to marshall
	  '''          the payload </param>
	  '''  <exception cref="java.lang.UnsupportedOperationException"> If this
	  '''          operation is not supported </exception>
	  '''  <exception cref="WebServiceException"> If an error occurs when using the supplied
	  '''     JAXBContext to marshall the payload. The cause of
	  '''     the WebServiceException is the original JAXBException.
	  '''  </exception>
	  Sub setPayload(ByVal payload As Object, ByVal context As javax.xml.bind.JAXBContext)
	End Interface

End Namespace
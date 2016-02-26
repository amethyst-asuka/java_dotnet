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
	'''  <p>Service endpoints may implement the <code>Provider</code>
	'''  interface as a dynamic alternative to an SEI.
	''' 
	'''  <p>Implementations are required to support <code>Provider&lt;Source&gt;</code>,
	'''  <code>Provider&lt;SOAPMessage&gt;</code> and
	'''  <code>Provider&lt;DataSource&gt;</code>, depending on the binding
	'''  in use and the service mode.
	''' 
	'''  <p>The <code>ServiceMode</code> annotation can be used to control whether
	'''  the <code>Provider</code> instance will receive entire protocol messages
	'''  or just message payloads.
	''' 
	'''  @since JAX-WS 2.0
	''' </summary>
	'''  <seealso cref= javax.xml.transform.Source </seealso>
	'''  <seealso cref= javax.xml.soap.SOAPMessage </seealso>
	'''  <seealso cref= javax.xml.ws.ServiceMode
	'''  </seealso>
	Public Interface Provider(Of T)

	  ''' <summary>
	  ''' Invokes an operation occording to the contents of the request
	  '''  message.
	  ''' </summary>
	  '''  <param name="request"> The request message or message payload. </param>
	  '''  <returns> The response message or message payload. May be <code>null</code> if
	  '''            there is no response. </returns>
	  '''  <exception cref="WebServiceException"> If there is an error processing request.
	  '''          The cause of the <code>WebServiceException</code> may be set to a subclass
	  '''          of <code>ProtocolException</code> to control the protocol level
	  '''          representation of the exception. </exception>
	  '''  <seealso cref= javax.xml.ws.handler.MessageContext </seealso>
	  '''  <seealso cref= javax.xml.ws.ProtocolException
	  '''  </seealso>
	  Function invoke(ByVal request As T) As T
	End Interface

End Namespace
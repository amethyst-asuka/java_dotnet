'
' * Copyright (c) 2005, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.xml.ws.handler


	''' <summary>
	''' The <code>Handler</code> interface
	'''  is the base interface for JAX-WS handlers.
	''' 
	'''  @since JAX-WS 2.0
	''' 
	''' </summary>
	Public Interface Handler(Of C As javax.xml.ws.handler.MessageContext)

	  ''' <summary>
	  ''' The <code>handleMessage</code> method is invoked for normal processing
	  '''  of inbound and outbound messages. Refer to the description of the handler
	  '''  framework in the JAX-WS specification for full details.
	  ''' </summary>
	  '''  <param name="context"> the message context. </param>
	  '''  <returns> An indication of whether handler processing should continue for
	  '''  the current message
	  '''                 <ul>
	  '''                 <li>Return <code>true</code> to continue
	  '''                     processing.</li>
	  '''                 <li>Return <code>false</code> to block
	  '''                     processing.</li>
	  '''                  </ul> </returns>
	  '''  <exception cref="RuntimeException"> Causes the JAX-WS runtime to cease
	  '''    handler processing and generate a fault. </exception>
	  '''  <exception cref="ProtocolException"> Causes the JAX-WS runtime to switch to
	  '''    fault message processing.
	  '''  </exception>
	  Function handleMessage(ByVal context As C) As Boolean

	  ''' <summary>
	  ''' The <code>handleFault</code> method is invoked for fault message
	  '''  processing.  Refer to the description of the handler
	  '''  framework in the JAX-WS specification for full details.
	  ''' </summary>
	  '''  <param name="context"> the message context </param>
	  '''  <returns> An indication of whether handler fault processing should continue
	  '''  for the current message
	  '''                 <ul>
	  '''                 <li>Return <code>true</code> to continue
	  '''                     processing.</li>
	  '''                 <li>Return <code>false</code> to block
	  '''                     processing.</li>
	  '''                  </ul> </returns>
	  '''  <exception cref="RuntimeException"> Causes the JAX-WS runtime to cease
	  '''    handler fault processing and dispatch the fault. </exception>
	  '''  <exception cref="ProtocolException"> Causes the JAX-WS runtime to cease
	  '''    handler fault processing and dispatch the fault.
	  '''  </exception>
	  Function handleFault(ByVal context As C) As Boolean

	  ''' <summary>
	  ''' Called at the conclusion of a message exchange pattern just prior to
	  ''' the JAX-WS runtime dispatching a message, fault or exception.  Refer to
	  ''' the description of the handler
	  ''' framework in the JAX-WS specification for full details.
	  ''' </summary>
	  ''' <param name="context"> the message context
	  '''  </param>
	  Sub close(ByVal context As javax.xml.ws.handler.MessageContext)
	End Interface

End Namespace
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

Namespace javax.xml.ws.handler.soap


	''' <summary>
	''' The <code>SOAPHandler</code> class extends <code>Handler</code>
	'''  to provide typesafety for the message context parameter and add a method
	'''  to obtain access to the headers that may be processed by the handler.
	''' 
	'''  @since JAX-WS 2.0
	''' 
	''' </summary>
	Public Interface SOAPHandler(Of T As SOAPMessageContext)
		Inherits javax.xml.ws.handler.Handler(Of T)

	  ''' <summary>
	  ''' Gets the header blocks that can be processed by this Handler
	  '''  instance.
	  ''' </summary>
	  '''  <returns> Set of <code>QNames</code> of header blocks processed by this
	  '''           handler instance. <code>QName</code> is the qualified
	  '''           name of the outermost element of the Header block.
	  '''  </returns>
	  ReadOnly Property headers As java.util.Set(Of javax.xml.namespace.QName)
	End Interface

End Namespace
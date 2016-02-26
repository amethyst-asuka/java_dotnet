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

Namespace javax.xml.ws.handler

	''' <summary>
	'''  <code>HandlerResolver</code> is an interface implemented
	'''  by an application to get control over the handler chain
	'''  set on proxy/dispatch objects at the time of their creation.
	'''  <p>
	'''  A <code>HandlerResolver</code> may be set on a <code>Service</code>
	'''  using the <code>setHandlerResolver</code> method.
	''' <p>
	'''  When the runtime invokes a <code>HandlerResolver</code>, it will
	'''  pass it a <code>PortInfo</code> object containing information
	'''  about the port that the proxy/dispatch object will be accessing.
	''' </summary>
	'''  <seealso cref= javax.xml.ws.Service#setHandlerResolver
	''' 
	'''  @since JAX-WS 2.0
	'''  </seealso>
	Public Interface HandlerResolver

	  ''' <summary>
	  '''  Gets the handler chain for the specified port.
	  ''' </summary>
	  '''  <param name="portInfo"> Contains information about the port being accessed. </param>
	  '''  <returns> java.util.List&lt;Handler> chain
	  '''  </returns>
	  Function getHandlerChain(ByVal portInfo As PortInfo) As IList(Of Handler)
	End Interface

End Namespace
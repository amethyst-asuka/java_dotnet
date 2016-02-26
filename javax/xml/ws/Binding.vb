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

Namespace javax.xml.ws


	''' <summary>
	''' The <code>Binding</code> interface is the base interface
	'''  for JAX-WS protocol bindings.
	''' 
	'''  @since JAX-WS 2.0
	''' 
	''' </summary>
	Public Interface Binding

	   ''' <summary>
	   ''' Gets a copy of the handler chain for a protocol binding instance.
	   ''' If the returned chain is modified a call to <code>setHandlerChain</code>
	   ''' is required to configure the binding instance with the new chain.
	   ''' </summary>
	   '''  <returns> java.util.List&lt;Handler> Handler chain </returns>
		Property handlerChain As IList(Of javax.xml.ws.handler.Handler)


		''' <summary>
		''' Get the URI for this binding instance.
		''' </summary>
		''' <returns> String The binding identifier for the port.
		'''    Never returns <code>null</code>
		''' 
		''' @since JAX-WS 2.1 </returns>
		ReadOnly Property bindingID As String
	End Interface

End Namespace
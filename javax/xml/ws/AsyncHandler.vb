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
	''' The <code>AsyncHandler</code> interface is implemented by
	''' clients that wish to receive callback notification of the completion of
	''' service endpoint operations invoked asynchronously.
	''' 
	'''  @since JAX-WS 2.0
	''' 
	''' </summary>
	Public Interface AsyncHandler(Of T)

		''' <summary>
		''' Called when the response to an asynchronous operation is available.
		''' </summary>
		''' <param name="res"> The response to the operation invocation.
		''' 
		'''  </param>
		Sub handleResponse(ByVal res As Response(Of T))
	End Interface

End Namespace
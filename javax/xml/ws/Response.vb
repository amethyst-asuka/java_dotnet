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
	''' The <code>Response</code> interface provides methods used to obtain the
	'''  payload and context of a message sent in response to an operation
	'''  invocation.
	''' 
	'''  <p>For asynchronous operation invocations it provides additional methods
	'''  to check the status of the request. The <code>get(...)</code> methods may
	'''  throw the standard
	'''  set of exceptions and their cause may be a <code>RemoteException</code> or a
	'''  <seealso cref="WebServiceException"/> that represents the error that occured during the
	'''  asynchronous method invocation.</p>
	''' 
	'''  @since JAX-WS 2.0
	''' 
	''' </summary>
	Public Interface Response(Of T)
		Inherits java.util.concurrent.Future(Of T)

		''' <summary>
		''' Gets the contained response context.
		''' </summary>
		''' <returns> The contained response context. May be <code>null</code> if a
		''' response is not yet available.
		''' 
		'''  </returns>
		ReadOnly Property context As IDictionary(Of String, Object)
	End Interface

End Namespace
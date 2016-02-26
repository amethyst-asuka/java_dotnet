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
	''' The <code>LogicalMessageContext</code> interface extends
	'''  <code>MessageContext</code> to
	'''  provide access to a the contained message as a protocol neutral
	'''  LogicalMessage
	''' 
	'''  @since JAX-WS 2.0
	''' 
	''' </summary>
	Public Interface LogicalMessageContext
		Inherits MessageContext

	  ''' <summary>
	  ''' Gets the message from this message context
	  ''' </summary>
	  '''  <returns> The contained message; returns <code>null</code> if no
	  '''          message is present in this message context
	  '''  </returns>
	  ReadOnly Property message As javax.xml.ws.LogicalMessage
	End Interface

End Namespace
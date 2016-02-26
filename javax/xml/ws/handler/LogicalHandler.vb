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
	''' The <code>LogicalHandler</code> extends
	'''  Handler to provide typesafety for the message context parameter.
	''' 
	'''  @since JAX-WS 2.0
	''' 
	''' </summary>
	Public Interface LogicalHandler(Of C As LogicalMessageContext)
		Inherits Handler(Of C)

	End Interface

End Namespace
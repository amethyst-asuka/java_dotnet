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
	'''  The <code>PortInfo</code> interface is used by a
	'''  <code>HandlerResolver</code> to query information about
	'''  the port it is being asked to create a handler chain for.
	'''  <p>
	'''  This interface is never implemented by an application,
	'''  only by a JAX-WS implementation.
	''' 
	'''  @since JAX-WS 2.0
	''' 
	''' </summary>
	Public Interface PortInfo

	  ''' <summary>
	  '''  Gets the qualified name of the WSDL service name containing
	  '''  the port being accessed.
	  ''' </summary>
	  '''  <returns> javax.xml.namespace.QName The qualified name of the WSDL service.
	  '''  </returns>
	  ReadOnly Property serviceName As javax.xml.namespace.QName

	  ''' <summary>
	  '''  Gets the qualified name of the WSDL port being accessed.
	  ''' </summary>
	  '''  <returns> javax.xml.namespace.QName The qualified name of the WSDL port.
	  '''  </returns>
	  ReadOnly Property portName As javax.xml.namespace.QName

	  ''' <summary>
	  '''  Gets the URI identifying the binding used by the port being accessed.
	  ''' </summary>
	  '''  <returns> String The binding identifier for the port.
	  ''' </returns>
	  '''  <seealso cref= javax.xml.ws.Binding
	  '''  </seealso>
	  ReadOnly Property bindingID As String

	End Interface

End Namespace
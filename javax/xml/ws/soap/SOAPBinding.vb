'
' * Copyright (c) 2005, 2011, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.xml.ws.soap



	''' <summary>
	''' The <code>SOAPBinding</code> interface is an abstraction for
	'''  the SOAP binding.
	''' 
	'''  @since JAX-WS 2.0
	''' 
	''' </summary>
	Public Interface SOAPBinding
		Inherits javax.xml.ws.Binding

	  ''' <summary>
	  ''' A constant representing the identity of the SOAP 1.1 over HTTP binding.
	  ''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'	  public static final String SOAP11HTTP_BINDING = "http://schemas.xmlsoap.org/wsdl/soap/http";

	  ''' <summary>
	  ''' A constant representing the identity of the SOAP 1.2 over HTTP binding.
	  ''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'	  public static final String SOAP12HTTP_BINDING = "http://www.w3.org/2003/05/soap/bindings/HTTP/";

	  ''' <summary>
	  ''' A constant representing the identity of the SOAP 1.1 over HTTP binding
	  ''' with MTOM enabled by default.
	  ''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'	  public static final String SOAP11HTTP_MTOM_BINDING = "http://schemas.xmlsoap.org/wsdl/soap/http?mtom=true";

	  ''' <summary>
	  ''' A constant representing the identity of the SOAP 1.2 over HTTP binding
	  ''' with MTOM enabled by default.
	  ''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'	  public static final String SOAP12HTTP_MTOM_BINDING = "http://www.w3.org/2003/05/soap/bindings/HTTP/?mtom=true";


	  ''' <summary>
	  ''' Gets the roles played by the SOAP binding instance.
	  ''' </summary>
	  '''  <returns> Set&lt;String> The set of roles played by the binding instance.
	  '''  </returns>
	  Property roles As java.util.Set(Of String)


	  ''' <summary>
	  ''' Returns <code>true</code> if the use of MTOM is enabled.
	  ''' </summary>
	  ''' <returns> <code>true</code> if and only if the use of MTOM is enabled.
	  '''  </returns>

	  Property mTOMEnabled As Boolean


	  ''' <summary>
	  ''' Gets the SAAJ <code>SOAPFactory</code> instance used by this SOAP binding.
	  ''' </summary>
	  ''' <returns> SOAPFactory instance used by this SOAP binding.
	  '''  </returns>
	  ReadOnly Property sOAPFactory As javax.xml.soap.SOAPFactory

	  ''' <summary>
	  ''' Gets the SAAJ <code>MessageFactory</code> instance used by this SOAP binding.
	  ''' </summary>
	  ''' <returns> MessageFactory instance used by this SOAP binding.
	  '''  </returns>
	  ReadOnly Property messageFactory As javax.xml.soap.MessageFactory
	End Interface

End Namespace
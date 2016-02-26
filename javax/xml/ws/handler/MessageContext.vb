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

Namespace javax.xml.ws.handler

	''' <summary>
	''' The interface <code>MessageContext</code> abstracts the message
	''' context that is processed by a handler in the <code>handle</code>
	''' method.
	''' 
	''' <p>The <code>MessageContext</code> interface provides methods to
	''' manage a property set. <code>MessageContext</code> properties
	''' enable handlers in a handler chain to share processing related
	''' state.
	''' 
	''' @since JAX-WS 2.0
	''' </summary>
	Public Interface MessageContext
		Inherits IDictionary(Of String, Object)

		''' <summary>
		''' Standard property: message direction, <code>true</code> for
		''' outbound messages, <code>false</code> for inbound.
		''' <p>Type: boolean
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final String MESSAGE_OUTBOUND_PROPERTY = "javax.xml.ws.handler.message.outbound";

		''' <summary>
		''' Standard property: Map of attachments to a message for the inbound
		''' message, key is  the MIME Content-ID, value is a DataHandler.
		''' <p>Type: java.util.Map&lt;String,DataHandler>
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final String INBOUND_MESSAGE_ATTACHMENTS = "javax.xml.ws.binding.attachments.inbound";

		''' <summary>
		''' Standard property: Map of attachments to a message for the outbound
		''' message, key is the MIME Content-ID, value is a DataHandler.
		''' <p>Type: java.util.Map&lt;String,DataHandler>
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final String OUTBOUND_MESSAGE_ATTACHMENTS = "javax.xml.ws.binding.attachments.outbound";

		''' <summary>
		''' Standard property: input source for WSDL document.
		''' <p>Type: org.xml.sax.InputSource
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final String WSDL_DESCRIPTION = "javax.xml.ws.wsdl.description";

		''' <summary>
		''' Standard property: name of WSDL service.
		''' <p>Type: javax.xml.namespace.QName
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final String WSDL_SERVICE = "javax.xml.ws.wsdl.service";

		''' <summary>
		''' Standard property: name of WSDL port.
		''' <p>Type: javax.xml.namespace.QName
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final String WSDL_PORT = "javax.xml.ws.wsdl.port";

		''' <summary>
		''' Standard property: name of wsdl interface (2.0) or port type (1.1).
		''' <p>Type: javax.xml.namespace.QName
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final String WSDL_INTERFACE = "javax.xml.ws.wsdl.interface";

		''' <summary>
		''' Standard property: name of WSDL operation.
		''' <p>Type: javax.xml.namespace.QName
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final String WSDL_OPERATION = "javax.xml.ws.wsdl.operation";

		''' <summary>
		''' Standard property: HTTP response status code.
		''' <p>Type: java.lang.Integer
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final String HTTP_RESPONSE_CODE = "javax.xml.ws.http.response.code";

		''' <summary>
		''' Standard property: HTTP request headers.
		''' <p>Type: java.util.Map&lt;java.lang.String, java.util.List&lt;java.lang.String>>
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final String HTTP_REQUEST_HEADERS = "javax.xml.ws.http.request.headers";

		''' <summary>
		''' Standard property: HTTP response headers.
		''' <p>Type: java.util.Map&lt;java.lang.String, java.util.List&lt;java.lang.String>>
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final String HTTP_RESPONSE_HEADERS = "javax.xml.ws.http.response.headers";

		''' <summary>
		''' Standard property: HTTP request method.
		''' <p>Type: java.lang.String
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final String HTTP_REQUEST_METHOD = "javax.xml.ws.http.request.method";

		''' <summary>
		''' Standard property: servlet request object.
		''' <p>Type: javax.servlet.http.HttpServletRequest
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final String SERVLET_REQUEST = "javax.xml.ws.servlet.request";

		''' <summary>
		''' Standard property: servlet response object.
		''' <p>Type: javax.servlet.http.HttpServletResponse
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final String SERVLET_RESPONSE = "javax.xml.ws.servlet.response";

		''' <summary>
		''' Standard property: servlet context object.
		''' <p>Type: javax.servlet.ServletContext
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final String SERVLET_CONTEXT = "javax.xml.ws.servlet.context";

		''' <summary>
		''' Standard property: Query string for request.
		''' <p>Type: String
		''' 
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final String QUERY_STRING = "javax.xml.ws.http.request.querystring";

		''' <summary>
		''' Standard property: Request Path Info
		''' <p>Type: String
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final String PATH_INFO = "javax.xml.ws.http.request.pathinfo";

		''' <summary>
		''' Standard property: WS Addressing Reference Parameters.
		''' The list MUST include all SOAP headers marked with the
		''' wsa:IsReferenceParameter="true" attribute.
		''' <p>Type: List&lt;Element>
		''' 
		''' @since JAX-WS 2.1
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final String REFERENCE_PARAMETERS = "javax.xml.ws.reference.parameters";

		''' <summary>
		''' Property scope. Properties scoped as <code>APPLICATION</code> are
		''' visible to handlers,
		''' client applications and service endpoints; properties scoped as
		''' <code>HANDLER</code>
		''' are only normally visible to handlers.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interface inner types are not converted:
'		public enum Scope
	'	{
	'		APPLICATION, HANDLER
	'	};

		''' <summary>
		''' Sets the scope of a property.
		''' </summary>
		''' <param name="name"> Name of the property associated with the
		'''             <code>MessageContext</code> </param>
		''' <param name="scope"> Desired scope of the property </param>
		''' <exception cref="java.lang.IllegalArgumentException"> if an illegal
		'''             property name is specified </exception>
		Sub setScope(ByVal name As String, ByVal scope As Scope)

		''' <summary>
		''' Gets the scope of a property.
		''' </summary>
		''' <param name="name"> Name of the property </param>
		''' <returns> Scope of the property </returns>
		''' <exception cref="java.lang.IllegalArgumentException"> if a non-existant
		'''             property name is specified </exception>
		Function getScope(ByVal name As String) As Scope
	End Interface

End Namespace
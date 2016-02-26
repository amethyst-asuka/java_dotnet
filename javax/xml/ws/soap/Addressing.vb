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

Namespace javax.xml.ws.soap



	''' <summary>
	''' This annotation represents the use of WS-Addressing with either
	''' the SOAP 1.1/HTTP or SOAP 1.2/HTTP binding. Using this annotation
	''' with any other binding is undefined.
	''' <p>
	''' This annotation MUST only be used in conjunction with the
	''' <seealso cref="javax.jws.WebService"/>, <seealso cref="WebServiceProvider"/>,
	'''  and <seealso cref="WebServiceRef"/> annotations.
	''' When used with a <code>javax.jws.WebService</code> annotation, this
	''' annotation MUST only be used on the service endpoint implementation
	''' class.
	''' When used with a <code>WebServiceRef</code> annotation, this annotation
	''' MUST only be used when a proxy instance is created. The injected SEI
	''' proxy, and endpoint MUST honor the values of the <code>Addressing</code>
	''' annotation.
	''' <p>
	''' This annotation's behaviour is defined by the corresponding feature
	''' <seealso cref="AddressingFeature"/>.
	''' 
	''' @since JAX-WS 2.1
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	<AttributeUsage(AttributeTargets.Class Or AttributeTargets.Method Or AttributeTargets.Field, AllowMultiple := False, Inherited := False> _
	Public Class Addressing
		Inherits System.Attribute

		''' <summary>
		''' Specifies if this feature is enabled or disabled. If enabled, it means
		''' the endpoint supports WS-Addressing but does not require its use.
		''' Corresponding
		''' <a href="http://www.w3.org/TR/ws-addr-metadata/#wspolicyaddressing">
		''' 3.1.1 Addressing Assertion</a> must be generated in the generated WSDL.
		''' </summary>
		Boolean enabled() default True

		''' <summary>
		''' If addressing is enabled, this property determines whether the endpoint
		''' requires WS-Addressing. If required is true, the endpoint requires
		''' WS-Addressing and WS-Addressing headers MUST
		''' be present on incoming messages. A corresponding
		''' <a href="http://www.w3.org/TR/ws-addr-metadata/#wspolicyaddressing">
		''' 3.1.1 Addressing Assertion</a> must be generated in the WSDL.
		''' </summary>
		Boolean required() default False

		''' <summary>
		''' If addressing is enabled, this property determines whether endpoint
		''' requires the use of anonymous responses, or non-anonymous responses,
		''' or all.
		''' 
		''' <p>
		''' <seealso cref="Responses#ALL"/> supports all response types and this is the
		''' default value.
		''' 
		''' <p>
		''' <seealso cref="Responses#ANONYMOUS"/> requires the use of only anonymous
		''' responses. It will result into wsam:AnonymousResponses nested assertion
		''' as specified in
		''' <a href="http://www.w3.org/TR/ws-addr-metadata/#wspolicyanonresponses">
		''' 3.1.2 AnonymousResponses Assertion</a> in the generated WSDL.
		''' 
		''' <p>
		''' <seealso cref="Responses#NON_ANONYMOUS"/> requires the use of only non-anonymous
		''' responses. It will result into
		''' wsam:NonAnonymousResponses nested assertion as specified in
		''' <a href="http://www.w3.org/TR/ws-addr-metadata/#wspolicynonanonresponses">
		''' 3.1.3 NonAnonymousResponses Assertion</a> in the generated WSDL.
		''' 
		''' @since JAX-WS 2.2
		''' </summary>
		javax.xml.ws.soap.AddressingFeature.Responses responses() default javax.xml.ws.soap.AddressingFeature.Responses.ALL

	End Class

End Namespace
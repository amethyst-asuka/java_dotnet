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
	''' This feature clarifies the use of the <code>wsdl:binding</code>
	''' in a JAX-WS runtime.
	''' <p>
	''' This annotation MUST only be used in conjunction the
	''' <code>javax.jws.WebService</code>, <seealso cref="WebServiceProvider"/>,
	''' <seealso cref="WebServiceRef"/> annotations.
	''' When used with the <code>javax.jws.WebService</code> annotation this
	''' annotation MUST only be used on the service endpoint implementation
	''' class.
	''' When used with a <code>WebServiceRef</code> annotation, this annotation
	''' MUST only be used when a proxy instance is created. The injected SEI
	''' proxy, and endpoint MUST honor the values of the <code>RespectBinding</code>
	''' annotation.
	''' <p>
	''' 
	''' This annotation's behaviour is defined by the corresponding feature
	''' <seealso cref="RespectBindingFeature"/>.
	''' </summary>
	''' <seealso cref= RespectBindingFeature
	''' 
	''' @since JAX-WS 2.1 </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	<AttributeUsage(AttributeTargets.Class Or AttributeTargets.Method Or AttributeTargets.Field, AllowMultiple := False, Inherited := False> _
	Public Class RespectBinding
		Inherits System.Attribute

		''' <summary>
		''' Specifies if this feature is enabled or disabled.
		''' </summary>
		Boolean enabled() default True
	End Class

End Namespace
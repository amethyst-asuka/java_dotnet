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
	''' This feature represents the use of MTOM with a
	''' web service.
	''' <p>
	''' This annotation MUST only be used in conjunction the
	''' <code>javax.jws.WebService</code>, <seealso cref="WebServiceProvider"/>,
	''' <seealso cref="WebServiceRef"/> annotations.
	''' When used with the <code>javax.jws.WebService</code> annotation this
	''' annotation MUST only be used on the service endpoint implementation
	''' class.
	''' When used with a <code>WebServiceRef</code> annotation, this annotation
	''' MUST only be used when a proxy instance is created. The injected SEI
	''' proxy, and endpoint MUST honor the values of the <code>MTOM</code>
	''' annotation.
	''' <p>
	''' 
	''' This annotation's behaviour is defined by the corresponding feature
	''' <seealso cref="MTOMFeature"/>.
	''' 
	''' @since JAX-WS 2.1
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	<AttributeUsage(AttributeTargets.Class Or AttributeTargets.Method Or AttributeTargets.Field, AllowMultiple := False, Inherited := False> _
	Public Class MTOM
		Inherits System.Attribute

		''' <summary>
		''' Specifies if this feature is enabled or disabled.
		''' </summary>
		Boolean enabled() default True

		''' <summary>
		''' Property for MTOM threshold value. When MTOM is enabled, binary data above this
		''' size in bytes will be XOP encoded or sent as attachment. The value of this property
		''' MUST always be >= 0. Default value is 0.
		''' </summary>
		Integer threshold() default 0
	End Class

End Namespace
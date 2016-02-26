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
	'''  Used to annotate the <code>get<em>PortName</em>()</code>
	'''  methods of a generated service interface.
	''' 
	'''  <p>The information specified in this annotation is sufficient
	'''  to uniquely identify a <code>wsdl:port</code> element
	'''  inside a <code>wsdl:service</code>. The latter is
	'''  determined based on the value of the <code>WebServiceClient</code>
	'''  annotation on the generated service interface itself.
	''' 
	'''  @since JAX-WS 2.0
	''' </summary>
	'''  <seealso cref= javax.xml.ws.WebServiceClient
	'''  </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	<AttributeUsage(AttributeTargets.Method, AllowMultiple := False, Inherited := False> _
	Public Class WebEndpoint
		Inherits System.Attribute

	  ''' <summary>
	  '''  The local name of the endpoint.
	  ''' 
	  ''' </summary>
	  String name() default ""
	End Class

End Namespace
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
	''' Used to annotate methods in the Service Endpoint Interface with the request
	''' wrapper bean to be used at runtime. The default value of the <code>localName</code> is
	''' the <code>operationName</code>, as defined in <code>WebMethod</code> annotation and the
	''' <code>targetNamespace</code> is the target namespace of the SEI.
	''' <p> When starting from Java this annotation is used resolve
	''' overloading conflicts in document literal mode. Only the <code>className</code>
	''' is required in this case.
	''' 
	'''  @since JAX-WS 2.0
	''' 
	''' </summary>

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	<AttributeUsage(AttributeTargets.Method, AllowMultiple := False, Inherited := False> _
	Public Class RequestWrapper
		Inherits System.Attribute

		''' <summary>
		''' Element's local name.
		''' </summary>
		public String localName() default ""

		''' <summary>
		''' Element's namespace name.
		''' </summary>
		public String targetNamespace() default ""

		''' <summary>
		''' Request wrapper bean name.
		''' </summary>
		public String className() default ""

		''' <summary>
		''' wsdl:part name for the wrapper part
		''' 
		''' @since JAX-WS 2.2
		''' </summary>
		public String partName() default ""

	End Class

End Namespace
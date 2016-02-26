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
	''' Used to annotate a Provider implementation class.
	''' 
	''' @since JAX-WS 2.0 </summary>
	''' <seealso cref= javax.xml.ws.Provider </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	<AttributeUsage(AttributeTargets.Class, AllowMultiple := False, Inherited := False> _
	Public Class WebServiceProvider
		Inherits System.Attribute

		''' <summary>
		''' Location of the WSDL description for the service.
		''' </summary>
		String wsdlLocation() default ""

		''' <summary>
		''' Service name.
		''' </summary>
		String serviceName() default ""

		''' <summary>
		''' Target namespace for the service
		''' </summary>
		String targetNamespace() default ""

		''' <summary>
		''' Port name.
		''' </summary>
		String portName() default ""
	End Class

End Namespace
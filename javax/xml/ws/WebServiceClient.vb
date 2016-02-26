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
	'''  Used to annotate a generated service interface.
	''' 
	'''  <p>The information specified in this annotation is sufficient
	'''  to uniquely identify a <code>wsdl:service</code>
	'''  element inside a WSDL document. This <code>wsdl:service</code>
	'''  element represents the Web service for which the generated
	'''  service interface provides a client view.
	''' 
	'''  @since JAX-WS 2.0
	''' 
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	<AttributeUsage(AttributeTargets.Class, AllowMultiple := False, Inherited := False> _
	Public Class WebServiceClient
		Inherits System.Attribute

	  ''' <summary>
	  '''  The local name of the Web service.
	  ''' 
	  ''' </summary>
	  String name() default ""

	  ''' <summary>
	  '''  The namespace for the Web service.
	  ''' 
	  ''' </summary>
	  String targetNamespace() default ""

	  ''' <summary>
	  '''  The location of the WSDL document for the service (a URL).
	  ''' 
	  ''' </summary>
	  String wsdlLocation() default ""
	End Class

End Namespace
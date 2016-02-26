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
	''' Used to annotate service specific exception classes to customize
	''' to the local and namespace name of the fault element and the name
	''' of the fault bean.
	''' 
	'''  @since JAX-WS 2.0
	''' 
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	<AttributeUsage(AttributeTargets.Class, AllowMultiple := False, Inherited := False> _
	Public Class WebFault
		Inherits System.Attribute

	  ''' <summary>
	  '''  Element's local name.
	  ''' 
	  ''' </summary>
	  public String name() default ""

	  ''' <summary>
	  '''  Element's namespace name.
	  ''' 
	  ''' </summary>
	  public String targetNamespace() default ""

	  ''' <summary>
	  '''  Fault bean name.
	  ''' 
	  ''' </summary>
	  public String faultBean() default ""


	  ''' <summary>
	  '''  wsdl:Message's name. Default name is the exception's class name.
	  '''  @since JAX-WS 2.2
	  ''' </summary>
	  public String messageName() default ""

	End Class

End Namespace
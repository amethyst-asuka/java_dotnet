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
	'''  The <code>BindingType</code> annotation is used to
	'''  specify the binding to use for a web service
	'''  endpoint implementation class.
	'''  <p>
	'''  This annotation may be overriden programmatically or via
	'''  deployment descriptors, depending on the platform in use.
	''' 
	'''  @since JAX-WS 2.0
	''' 
	''' 
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	<AttributeUsage(AttributeTargets.Class, AllowMultiple := False, Inherited := False> _
	Public Class BindingType
		Inherits System.Attribute

		 ''' <summary>
		 ''' A binding identifier (a URI).
		 ''' If not specified, the default is the SOAP 1.1 / HTTP binding.
		 ''' <p>
		 ''' See the <code>SOAPBinding</code> and <code>HTTPBinding</code>
		 ''' for the definition of the standard binding identifiers.
		 ''' </summary>
		 ''' <seealso cref= javax.xml.ws.Binding </seealso>
		 ''' <seealso cref= javax.xml.ws.soap.SOAPBinding#SOAP11HTTP_BINDING </seealso>
		 ''' <seealso cref= javax.xml.ws.soap.SOAPBinding#SOAP12HTTP_BINDING </seealso>
		 ''' <seealso cref= javax.xml.ws.http.HTTPBinding#HTTP_BINDING </seealso>
		 String value() default ""
	End Class

End Namespace
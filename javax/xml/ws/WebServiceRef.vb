Imports System

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

Namespace javax.xml.ws


	''' <summary>
	''' The <code>WebServiceRef</code> annotation is used to
	''' define a reference to a web service and
	''' (optionally) an injection target for it.
	''' It can be used to inject both service and proxy
	''' instances. These injected references are not thread safe.
	''' If the references are accessed by multiple threads,
	''' usual synchronization techinques can be used to
	''' support multiple threads.
	''' 
	''' <p>
	''' Web service references are resources in the Java EE 5 sense.
	''' The annotations (for example, <seealso cref="Addressing"/>) annotated with
	''' meta-annotation <seealso cref="WebServiceFeatureAnnotation"/>
	''' can be used in conjunction with <code>WebServiceRef</code>.
	''' The created reference MUST be configured with annotation's web service
	''' feature.
	''' 
	''' <p>
	''' For example, in the code below, the injected
	''' <code>StockQuoteProvider</code> proxy MUST
	''' have WS-Addressing enabled as specifed by the
	''' <seealso cref="Addressing"/>
	''' annotation.
	''' 
	''' <pre><code>
	'''    public class MyClient {
	'''       &#64;Addressing
	'''       &#64;WebServiceRef(StockQuoteService.class)
	'''       private StockQuoteProvider stockQuoteProvider;
	'''       ...
	'''    }
	''' </code></pre>
	''' 
	''' <p>
	''' If a JAX-WS implementation encounters an unsupported or unrecognized
	''' annotation annotated with the <code>WebServiceFeatureAnnotation</code>
	''' that is specified with <code>WebServiceRef</code>, an ERROR MUST be given.
	''' </summary>
	''' <seealso cref= javax.annotation.Resource </seealso>
	''' <seealso cref= WebServiceFeatureAnnotation
	''' 
	''' @since JAX-WS 2.0
	''' 
	'''  </seealso>

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	<AttributeUsage(AttributeTargets.Class Or AttributeTargets.Method Or AttributeTargets.Field, AllowMultiple := False, Inherited := False> _
	Public Class WebServiceRef
		Inherits System.Attribute

		''' <summary>
		''' The JNDI name of the resource.  For field annotations,
		''' the default is the field name.  For method annotations,
		''' the default is the JavaBeans property name corresponding
		''' to the method.  For class annotations, there is no default
		''' and this MUST be specified.
		''' 
		''' The JNDI name can be absolute(with any logical namespace) or relative
		''' to JNDI <code>java:comp/env</code> namespace.
		''' </summary>
		String name() default ""

		''' <summary>
		''' The Java type of the resource.  For field annotations,
		''' the default is the type of the field.  For method annotations,
		''' the default is the type of the JavaBeans property.
		''' For class annotations, there is no default and this MUST be
		''' specified.
		''' </summary>
		Type type() default GetType(Object)

		''' <summary>
		''' A product specific name that this resource should be mapped to.
		''' The name of this resource, as defined by the <code>name</code>
		''' element or defaulted, is a name that is local to the application
		''' component using the resource.  (When a relative JNDI name
		''' is specified, then it's a name in the JNDI
		''' <code>java:comp/env</code> namespace.)  Many application servers
		''' provide a way to map these local names to names of resources
		''' known to the application server.  This mapped name is often a
		''' <i>global</i> JNDI name, but may be a name of any form.
		''' <p>
		''' Application servers are not required to support any particular
		''' form or type of mapped name, nor the ability to use mapped names.
		''' The mapped name is product-dependent and often installation-dependent.
		''' No use of a mapped name is portable.
		''' </summary>
		String mappedName() default ""

		''' <summary>
		''' The service class, alwiays a type extending
		''' <code>javax.xml.ws.Service</code>. This element MUST be specified
		''' whenever the type of the reference is a service endpoint interface.
		''' </summary>
		' 2.1 has Class value() default Object.class;
		' Fixing this raw Class type correctly in 2.2 API. This shouldn't cause
		' any compatibility issues for applications.
		Type value() default GetType(Service)

		''' <summary>
		''' A URL pointing to the WSDL document for the web service.
		''' If not specified, the WSDL location specified by annotations
		''' on the resource type is used instead.
		''' </summary>
		String wsdlLocation() default ""

		''' <summary>
		''' A portable JNDI lookup name that resolves to the target
		''' web service reference.
		''' 
		''' @since JAX-WS 2.2
		''' </summary>
		String lookup() default ""

	End Class

End Namespace
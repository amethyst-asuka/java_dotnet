Imports System

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

Namespace javax.xml.ws.spi


	''' <summary>
	''' Annotation used to identify other annotations
	''' as a <code>WebServiceFeature</code>.
	''' <p>
	''' Each <code>WebServiceFeature</code> annotation annotated with
	''' this annotation MUST contain an
	''' <code>enabled</code> property of type
	''' <code>boolean</code> with a default value of <code>true</code>.
	''' <p>
	''' JAX-WS defines the following
	''' <code>WebServiceFeature</code> annotations (<code>Addressing</code>,
	''' <code>MTOM</code>, <code>RespectBinding</code>), however, an implementation
	''' may define vendors specific annotations for other features.
	''' <p>
	''' Annotations annotated with <code>WebServiceFeatureAnnotation</code> MUST
	''' have the same @Target of <seealso cref="WebServiceRef"/> annotation, so that the resulting
	''' feature annotation can be used in conjunction with the <seealso cref="WebServiceRef"/>
	''' annotation if necessary.
	''' <p>
	''' If a JAX-WS implementation encounters an annotation annotated
	''' with the <code>WebServiceFeatureAnnotation</code> that it does not
	''' recognize/support an error MUST be given.
	''' <p>
	''' </summary>
	''' <seealso cref= Addressing </seealso>
	''' <seealso cref= MTOM </seealso>
	''' <seealso cref= RespectBinding
	''' 
	''' @since JAX-WS 2.1 </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	<AttributeUsage(AttributeTargets.Class, AllowMultiple := False, Inherited := False> _
	Public Class WebServiceFeatureAnnotation
		Inherits System.Attribute

		''' <summary>
		''' Unique identifier for the WebServiceFeature.  This
		''' identifier MUST be unique across all implementations
		''' of JAX-WS.
		''' </summary>
		String id()

		''' <summary>
		''' The <code>WebServiceFeature</code> bean that is associated
		''' with the <code>WebServiceFeature</code> annotation
		''' </summary>
		Type bean()
	End Class

End Namespace
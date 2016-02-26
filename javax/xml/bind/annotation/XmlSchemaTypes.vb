'
' * Copyright (c) 2005, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.xml.bind.annotation


	''' <summary>
	''' <p>
	''' A container for multiple @<seealso cref="XmlSchemaType"/> annotations.
	''' 
	''' <p> Multiple annotations of the same type are not allowed on a program
	''' element. This annotation therefore serves as a container annotation
	''' for multiple &#64;XmlSchemaType annotations as follows:
	''' 
	''' <pre>
	''' &#64;XmlSchemaTypes({ @XmlSchemaType(...), @XmlSchemaType(...) })
	''' </pre>
	''' <p>The <tt>@XmlSchemaTypes</tt> annnotation can be used to
	''' define <seealso cref="XmlSchemaType"/> for different types at the
	''' package level.
	''' 
	''' <p>See "Package Specification" in javax.xml.bind.package javadoc for
	''' additional common information.</p>
	''' 
	''' @author <ul><li>Sekhar Vajjhala, Sun Microsystems, Inc.</li></ul> </summary>
	''' <seealso cref= XmlSchemaType
	''' @since JAXB2.0 </seealso>
'JAVA TO VB CONVERTER TODO TASK: There is no attribute target in .NET corresponding to PACKAGE:
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	<AttributeUsage(<missing>, AllowMultiple := False, Inherited := False> _
	Public Class XmlSchemaTypes
		Inherits System.Attribute

		''' <summary>
		''' Collection of @<seealso cref="XmlSchemaType"/> annotations
		''' </summary>
		XmlSchemaType() value()
	End Class

End Namespace
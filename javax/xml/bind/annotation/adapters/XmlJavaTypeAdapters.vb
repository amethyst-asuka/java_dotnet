'
' * Copyright (c) 2004, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.xml.bind.annotation.adapters


	''' <summary>
	''' <p>
	''' A container for multiple @<seealso cref="XmlJavaTypeAdapter"/> annotations.
	''' 
	''' <p> Multiple annotations of the same type are not allowed on a program
	''' element. This annotation therefore serves as a container annotation
	''' for multiple &#64;XmlJavaTypeAdapter as follows:
	''' 
	''' <pre>
	''' &#64;XmlJavaTypeAdapters ({ @XmlJavaTypeAdapter(...),@XmlJavaTypeAdapter(...) })
	''' </pre>
	''' 
	''' <p>The <tt>@XmlJavaTypeAdapters</tt> annnotation is useful for
	''' defining <seealso cref="XmlJavaTypeAdapter"/> annotations for different types
	''' at the package level.
	''' 
	''' <p>See "Package Specification" in javax.xml.bind.package javadoc for
	''' additional common information.</p>
	''' 
	''' @author <ul><li>Sekhar Vajjhala, Sun Microsystems, Inc.</li></ul> </summary>
	''' <seealso cref= XmlJavaTypeAdapter
	''' @since JAXB2.0 </seealso>
'JAVA TO VB CONVERTER TODO TASK: There is no attribute target in .NET corresponding to PACKAGE:
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	<AttributeUsage(<missing>, AllowMultiple := False, Inherited := False> _
	Public Class XmlJavaTypeAdapters
		Inherits System.Attribute

		''' <summary>
		''' Collection of @<seealso cref="XmlJavaTypeAdapter"/> annotations
		''' </summary>
		XmlJavaTypeAdapter() value()
	End Class

End Namespace
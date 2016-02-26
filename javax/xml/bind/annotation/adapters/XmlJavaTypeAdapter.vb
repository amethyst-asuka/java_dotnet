Imports System

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
	''' Use an adapter that implements <seealso cref="XmlAdapter"/> for custom marshaling.
	''' 
	''' <p> <b> Usage: </b> </p>
	''' 
	''' <p> The <tt>@XmlJavaTypeAdapter</tt> annotation can be used with the
	''' following program elements:
	''' <ul>
	'''   <li> a JavaBean property </li>
	'''   <li> field </li>
	'''   <li> parameter </li>
	'''   <li> package </li>
	'''   <li> from within <seealso cref="XmlJavaTypeAdapters"/> </li>
	''' </ul>
	''' 
	''' <p> When <tt>@XmlJavaTypeAdapter</tt> annotation is defined on a
	''' class, it applies to all references to the class.
	''' <p> When <tt>@XmlJavaTypeAdapter</tt> annotation is defined at the
	''' package level it applies to all references from within the package
	''' to <tt>@XmlJavaTypeAdapter.type()</tt>.
	''' <p> When <tt>@XmlJavaTypeAdapter</tt> annotation is defined on the
	''' field, property or parameter, then the annotation applies to the
	''' field, property or the parameter only.
	''' <p> A <tt>@XmlJavaTypeAdapter</tt> annotation on a field, property
	''' or parameter overrides the <tt>@XmlJavaTypeAdapter</tt> annotation
	''' associated with the class being referenced by the field, property
	''' or parameter.
	''' <p> A <tt>@XmlJavaTypeAdapter</tt> annotation on a class overrides
	''' the <tt>@XmlJavaTypeAdapter</tt> annotation specified at the
	''' package level for that class.
	''' 
	''' <p>This annotation can be used with the following other annotations:
	''' <seealso cref="XmlElement"/>, <seealso cref="XmlAttribute"/>, <seealso cref="XmlElementRef"/>,
	''' <seealso cref="XmlElementRefs"/>, <seealso cref="XmlAnyElement"/>. This can also be
	''' used at the package level with the following annotations:
	''' <seealso cref="XmlAccessorType"/>, <seealso cref="XmlSchema"/>, <seealso cref="XmlSchemaType"/>,
	''' <seealso cref="XmlSchemaTypes"/>.
	''' 
	''' <p><b> Example: </b> See example in <seealso cref="XmlAdapter"/>
	''' 
	''' @author <ul><li>Sekhar Vajjhala, Sun Microsystems Inc.</li> <li> Kohsuke Kawaguchi, Sun Microsystems Inc.</li></ul>
	''' @since JAXB2.0 </summary>
	''' <seealso cref= XmlAdapter </seealso>

'JAVA TO VB CONVERTER TODO TASK: There is no attribute target in .NET corresponding to PACKAGE:
'JAVA TO VB CONVERTER TODO TASK: There is no attribute target in .NET corresponding to FIELD:
'JAVA TO VB CONVERTER TODO TASK: There is no attribute target in .NET corresponding to METHOD:
'JAVA TO VB CONVERTER TODO TASK: There is no attribute target in .NET corresponding to TYPE:
'JAVA TO VB CONVERTER TODO TASK: There is no attribute target in .NET corresponding to PARAMETER:
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	<AttributeUsage(<missing> Or <missing> Or <missing> Or <missing> Or <missing>, AllowMultiple := False, Inherited := False> _
	Public Class XmlJavaTypeAdapter
		Inherits System.Attribute

		''' <summary>
		''' Points to the class that converts a value type to a bound type or vice versa.
		''' See <seealso cref="XmlAdapter"/> for more details.
		''' </summary>
		Type value()

		''' <summary>
		''' If this annotation is used at the package level, then value of
		''' the type() must be specified.
		''' </summary>

		Type type() default GetType(DEFAULT)

		''' <summary>
		''' Used in <seealso cref="XmlJavaTypeAdapter#type()"/> to
		''' signal that the type be inferred from the signature
		''' of the field, property, parameter or the class.
		''' </summary>

		static final class DEFAULT

	End Class

End Namespace
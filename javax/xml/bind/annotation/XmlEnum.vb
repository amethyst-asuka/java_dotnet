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

Namespace javax.xml.bind.annotation


	''' <summary>
	''' <p>
	''' Maps an enum type <seealso cref="Enum"/> to XML representation.
	''' 
	''' <p>This annotation, together with <seealso cref="XmlEnumValue"/> provides a
	''' mapping of enum type to XML representation.
	''' 
	''' <p> <b>Usage</b> </p>
	''' <p>
	''' The <tt>@XmlEnum</tt> annotation can be used with the
	''' following program elements:
	''' <ul>
	'''   <li>enum type</li>
	''' </ul>
	''' 
	''' <p> The usage is subject to the following constraints:
	''' <ul>
	'''   <li> This annotation can be used the following other annotations:
	'''         <seealso cref="XmlType"/>,
	'''         <seealso cref="XmlRootElement"/> </li>
	''' </ul>
	''' <p>See "Package Specification" in javax.xml.bind.package javadoc for
	''' additional common information </p>
	''' 
	''' <p>An enum type is mapped to a schema simple type with enumeration
	''' facets. The schema type is derived from the Java type to which
	''' <tt>@XmlEnum.value()</tt>. Each enum constant <tt>@XmlEnumValue</tt>
	''' must have a valid lexical representation for the type
	''' <tt>@XmlEnum.value()</tt> .
	''' 
	''' <p><b>Examples:</b> See examples in <seealso cref="XmlEnumValue"/>
	''' 
	''' @since JAXB2.0
	''' </summary>

'JAVA TO VB CONVERTER TODO TASK: There is no attribute target in .NET corresponding to TYPE:
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	<AttributeUsage(<missing>, AllowMultiple := False, Inherited := False> _
	Public Class XmlEnum
		Inherits System.Attribute

		''' <summary>
		''' Java type that is mapped to a XML simple type.
		''' 
		''' </summary>
		Type value() default GetType(String)
	End Class

End Namespace
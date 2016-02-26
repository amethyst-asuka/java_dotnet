Imports System

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
	''' Maps a Java type to a simple schema built-in type.
	''' 
	''' <p> <b>Usage</b> </p>
	''' <p>
	''' <tt>@XmlSchemaType</tt> annotation can be used with the following program
	''' elements:
	''' <ul>
	'''   <li> a JavaBean property </li>
	'''   <li> field </li>
	'''   <li> package</li>
	''' </ul>
	''' 
	''' <p> <tt>@XmlSchemaType</tt> annotation defined for Java type
	''' applies to all references to the Java type from a property/field.
	''' A <tt>@XmlSchemaType</tt> annotation specified on the
	''' property/field overrides the <tt>@XmlSchemaType</tt> annotation
	''' specified at the package level.
	''' 
	''' <p> This annotation can be used with the following annotations:
	''' <seealso cref="XmlElement"/>,  <seealso cref="XmlAttribute"/>.
	''' <p>
	''' <b>Example 1: </b> Customize mapping of XMLGregorianCalendar on the
	'''  field.
	''' 
	''' <pre>
	'''     //Example: Code fragment
	'''     public class USPrice {
	'''         &#64;XmlElement
	'''         &#64;XmlSchemaType(name="date")
	'''         public XMLGregorianCalendar date;
	'''     }
	''' 
	'''     &lt;!-- Example: Local XML Schema element -->
	'''     &lt;xs:complexType name="USPrice"/>
	'''       &lt;xs:sequence>
	'''         &lt;xs:element name="date" type="xs:date"/>
	'''       &lt;/sequence>
	'''     &lt;/xs:complexType>
	''' </pre>
	''' 
	''' <p> <b> Example 2: </b> Customize mapping of XMLGregorianCalendar at package
	'''     level </p>
	''' <pre>
	'''     package foo;
	'''     &#64;javax.xml.bind.annotation.XmlSchemaType(
	'''          name="date", type=javax.xml.datatype.XMLGregorianCalendar.class)
	'''     }
	''' </pre>
	''' 
	''' @since JAXB2.0
	''' </summary>

'JAVA TO VB CONVERTER TODO TASK: There is no attribute target in .NET corresponding to FIELD:
'JAVA TO VB CONVERTER TODO TASK: There is no attribute target in .NET corresponding to METHOD:
'JAVA TO VB CONVERTER TODO TASK: There is no attribute target in .NET corresponding to PACKAGE:
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	<AttributeUsage(<missing> Or <missing> Or <missing>, AllowMultiple := False, Inherited := False> _
	Public Class XmlSchemaType
		Inherits System.Attribute

		String name()
		String namespace() default "http://www.w3.org/2001/XMLSchema"
		''' <summary>
		''' If this annotation is used at the package level, then value of
		''' the type() must be specified.
		''' </summary>

		Type type() default GetType(DEFAULT)

		''' <summary>
		''' Used in <seealso cref="XmlSchemaType#type()"/> to
		''' signal that the type be inferred from the signature
		''' of the property.
		''' </summary>

		static final class DEFAULT

	End Class

End Namespace
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
	''' Maps a JavaBean property to a XML attribute.
	''' 
	''' <p> <b>Usage</b> </p>
	''' <p>
	''' The <tt>@XmlAttribute</tt> annotation can be used with the
	''' following program elements:
	''' <ul>
	'''   <li> JavaBean property </li>
	'''   <li> field </li>
	''' </ul>
	''' 
	''' <p> A static final field is mapped to a XML fixed attribute.
	''' 
	''' <p>See "Package Specification" in javax.xml.bind.package javadoc for
	''' additional common information.</p>
	''' 
	''' The usage is subject to the following constraints:
	''' <ul>
	'''   <li> If type of the field or the property is a collection
	'''        type, then the collection item type must be mapped to schema
	'''        simple type.
	''' <pre>
	'''     // Examples
	'''     &#64;XmlAttribute List&lt;Integer> items; //legal
	'''     &#64;XmlAttribute List&lt;Bar> foo; // illegal if Bar does not map to a schema simple type
	''' </pre>
	'''   </li>
	'''   <li> If the type of the field or the property is a non
	'''         collection type, then the type of the property or field
	'''         must map to a simple schema type.
	''' <pre>
	'''     // Examples
	'''     &#64;XmlAttribute int foo; // legal
	'''     &#64;XmlAttribute Foo foo; // illegal if Foo does not map to a schema simple type
	''' </pre>
	'''   </li>
	'''   <li> This annotation can be used with the following annotations:
	'''            <seealso cref="XmlID"/>,
	'''            <seealso cref="XmlIDREF"/>,
	'''            <seealso cref="XmlList"/>,
	'''            <seealso cref="XmlSchemaType"/>,
	'''            <seealso cref="XmlValue"/>,
	'''            <seealso cref="XmlAttachmentRef"/>,
	'''            <seealso cref="XmlMimeType"/>,
	'''            <seealso cref="XmlInlineBinaryData"/>,
	'''            <seealso cref="javax.xml.bind.annotation.adapters.XmlJavaTypeAdapter"/>.</li>
	''' </ul>
	''' </p>
	''' 
	''' <p> <b>Example 1: </b>Map a JavaBean property to an XML attribute.</p>
	''' <pre>
	'''     //Example: Code fragment
	'''     public class USPrice {
	'''         &#64;XmlAttribute
	'''         public java.math.BigDecimal getPrice() {...} ;
	'''         public void setPrice(java.math.BigDecimal ) {...};
	'''     }
	''' 
	'''     &lt;!-- Example: XML Schema fragment -->
	'''     &lt;xs:complexType name="USPrice">
	'''       &lt;xs:sequence>
	'''       &lt;/xs:sequence>
	'''       &lt;xs:attribute name="price" type="xs:decimal"/>
	'''     &lt;/xs:complexType>
	''' </pre>
	''' 
	''' <p> <b>Example 2: </b>Map a JavaBean property to an XML attribute with anonymous type.</p>
	''' See Example 7 in @<seealso cref="XmlType"/>.
	''' 
	''' <p> <b>Example 3: </b>Map a JavaBean collection property to an XML attribute.</p>
	''' <pre>
	'''     // Example: Code fragment
	'''     class Foo {
	'''         ...
	'''         &#64;XmlAttribute List&lt;Integer> items;
	'''     }
	''' 
	'''     &lt;!-- Example: XML Schema fragment -->
	'''     &lt;xs:complexType name="foo">
	'''       ...
	'''       &lt;xs:attribute name="items">
	'''         &lt;xs:simpleType>
	'''           &lt;xs:list itemType="xs:int"/>
	'''         &lt;/xs:simpleType>
	'''     &lt;/xs:complexType>
	''' 
	''' </pre>
	''' @author Sekhar Vajjhala, Sun Microsystems, Inc. </summary>
	''' <seealso cref= XmlType
	''' @since JAXB2.0 </seealso>

'JAVA TO VB CONVERTER TODO TASK: There is no attribute target in .NET corresponding to FIELD:
'JAVA TO VB CONVERTER TODO TASK: There is no attribute target in .NET corresponding to METHOD:
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	<AttributeUsage(<missing> Or <missing>, AllowMultiple := False, Inherited := False> _
	Public Class XmlAttribute
		Inherits System.Attribute

		''' <summary>
		''' Name of the XML Schema attribute. By default, the XML Schema
		''' attribute name is derived from the JavaBean property name.
		''' 
		''' </summary>
		String name() default "##default"

		''' <summary>
		''' Specifies if the XML Schema attribute is optional or
		''' required. If true, then the JavaBean property is mapped to a
		''' XML Schema attribute that is required. Otherwise it is mapped
		''' to a XML Schema attribute that is optional.
		''' 
		''' </summary>
		 Boolean required() default False

		''' <summary>
		''' Specifies the XML target namespace of the XML Schema
		''' attribute.
		''' 
		''' </summary>
		String namespace() default "##default"
	End Class

End Namespace
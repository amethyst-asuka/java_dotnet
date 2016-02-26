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
	''' Maps a JavaBean property to a XML element derived from property name.
	''' 
	''' <p> <b>Usage</b> </p>
	''' <p>
	''' <tt>@XmlElement</tt> annotation can be used with the following program
	''' elements:
	''' <ul>
	'''   <li> a JavaBean property </li>
	'''   <li> non static, non transient field </li>
	'''   <li> within <seealso cref="XmlElements"/>
	''' <p>
	''' 
	''' </ul>
	''' 
	''' The usage is subject to the following constraints:
	''' <ul>
	'''   <li> This annotation can be used with following annotations:
	'''            <seealso cref="XmlID"/>,
	'''            <seealso cref="XmlIDREF"/>,
	'''            <seealso cref="XmlList"/>,
	'''            <seealso cref="XmlSchemaType"/>,
	'''            <seealso cref="XmlValue"/>,
	'''            <seealso cref="XmlAttachmentRef"/>,
	'''            <seealso cref="XmlMimeType"/>,
	'''            <seealso cref="XmlInlineBinaryData"/>,
	'''            <seealso cref="XmlElementWrapper"/>,
	'''            <seealso cref="XmlJavaTypeAdapter"/></li>
	'''   <li> if the type of JavaBean property is a collection type of
	'''        array, an indexed property, or a parameterized list, and
	'''        this annotation is used with <seealso cref="XmlElements"/> then,
	'''        <tt>@XmlElement.type()</tt> must be DEFAULT.class since the
	'''        collection item type is already known. </li>
	''' </ul>
	''' 
	''' <p>
	''' A JavaBean property, when annotated with @XmlElement annotation
	''' is mapped to a local element in the XML Schema complex type to
	''' which the containing class is mapped.
	''' 
	''' <p>
	''' <b>Example 1: </b> Map a public non static non final field to local
	''' element
	''' <pre>
	'''     //Example: Code fragment
	'''     public class USPrice {
	'''         &#64;XmlElement(name="itemprice")
	'''         public java.math.BigDecimal price;
	'''     }
	''' 
	'''     &lt;!-- Example: Local XML Schema element -->
	'''     &lt;xs:complexType name="USPrice"/>
	'''       &lt;xs:sequence>
	'''         &lt;xs:element name="itemprice" type="xs:decimal" minOccurs="0"/>
	'''       &lt;/sequence>
	'''     &lt;/xs:complexType>
	'''   </pre>
	''' <p>
	''' 
	''' <b> Example 2: </b> Map a field to a nillable element.
	'''   <pre>
	''' 
	'''     //Example: Code fragment
	'''     public class USPrice {
	'''         &#64;XmlElement(nillable=true)
	'''         public java.math.BigDecimal price;
	'''     }
	''' 
	'''     &lt;!-- Example: Local XML Schema element -->
	'''     &lt;xs:complexType name="USPrice">
	'''       &lt;xs:sequence>
	'''         &lt;xs:element name="price" type="xs:decimal" nillable="true" minOccurs="0"/>
	'''       &lt;/sequence>
	'''     &lt;/xs:complexType>
	'''   </pre>
	''' <p>
	''' <b> Example 3: </b> Map a field to a nillable, required element.
	'''   <pre>
	''' 
	'''     //Example: Code fragment
	'''     public class USPrice {
	'''         &#64;XmlElement(nillable=true, required=true)
	'''         public java.math.BigDecimal price;
	'''     }
	''' 
	'''     &lt;!-- Example: Local XML Schema element -->
	'''     &lt;xs:complexType name="USPrice">
	'''       &lt;xs:sequence>
	'''         &lt;xs:element name="price" type="xs:decimal" nillable="true" minOccurs="1"/>
	'''       &lt;/sequence>
	'''     &lt;/xs:complexType>
	'''   </pre>
	''' <p>
	''' 
	''' <p> <b>Example 4: </b>Map a JavaBean property to an XML element
	''' with anonymous type.</p>
	''' <p>
	''' See Example 6 in @<seealso cref="XmlType"/>.
	''' 
	''' <p>
	''' @author Sekhar Vajjhala, Sun Microsystems, Inc.
	''' @since JAXB2.0
	''' </summary>

'JAVA TO VB CONVERTER TODO TASK: There is no attribute target in .NET corresponding to FIELD:
'JAVA TO VB CONVERTER TODO TASK: There is no attribute target in .NET corresponding to METHOD:
'JAVA TO VB CONVERTER TODO TASK: There is no attribute target in .NET corresponding to PARAMETER:
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	<AttributeUsage(<missing> Or <missing> Or <missing>, AllowMultiple := False, Inherited := False> _
	Public Class XmlElement
		Inherits System.Attribute

		''' <summary>
		''' Name of the XML Schema element.
		''' <p> If the value is "##default", then element name is derived from the
		''' JavaBean property name.
		''' </summary>
		String name() default "##default"

		''' <summary>
		''' Customize the element declaration to be nillable.
		''' <p>If nillable() is true, then the JavaBean property is
		''' mapped to a XML Schema nillable element declaration.
		''' </summary>
		Boolean nillable() default False

		''' <summary>
		''' Customize the element declaration to be required.
		''' <p>If required() is true, then Javabean property is mapped to
		''' an XML schema element declaration with minOccurs="1".
		''' maxOccurs is "1" for a single valued property and "unbounded"
		''' for a multivalued property.
		''' <p>If required() is false, then the Javabean property is mapped
		''' to XML Schema element declaration with minOccurs="0".
		''' maxOccurs is "1" for a single valued property and "unbounded"
		''' for a multivalued property.
		''' </summary>

		Boolean required() default False

		''' <summary>
		''' XML target namespace of the XML Schema element.
		''' <p>
		''' If the value is "##default", then the namespace is determined
		''' as follows:
		''' <ol>
		'''  <li>
		'''  If the enclosing package has <seealso cref="XmlSchema"/> annotation,
		'''  and its <seealso cref="XmlSchema#elementFormDefault() elementFormDefault"/>
		'''  is <seealso cref="XmlNsForm#QUALIFIED QUALIFIED"/>, then the namespace of
		'''  the enclosing class.
		''' 
		'''  <li>
		'''  Otherwise &#39;&#39; (which produces unqualified element in the default
		'''  namespace.
		''' </ol>
		''' </summary>
		String namespace() default "##default"

		''' <summary>
		''' Default value of this element.
		''' 
		''' <p>
		''' The <pre>'\u0000'</pre> value specified as a default of this annotation element
		''' is used as a poor-man's substitute for null to allow implementations
		''' to recognize the 'no default value' state.
		''' </summary>
		String defaultValue() default ChrW(&H0000).ToString()

		''' <summary>
		''' The Java class being referenced.
		''' </summary>
		Type type() default GetType(DEFAULT)

		''' <summary>
		''' Used in <seealso cref="XmlElement#type()"/> to
		''' signal that the type be inferred from the signature
		''' of the property.
		''' </summary>
		static final class DEFAULT
	End Class

End Namespace
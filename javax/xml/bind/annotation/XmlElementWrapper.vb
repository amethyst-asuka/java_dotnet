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
	''' Generates a wrapper element around XML representation.
	''' 
	''' This is primarily intended to be used to produce a wrapper
	''' XML element around collections. The annotation therefore supports
	''' two forms of serialization shown below.
	''' 
	''' <pre>
	'''    //Example: code fragment
	'''      int[] names;
	''' 
	'''    // XML Serialization Form 1 (Unwrapped collection)
	'''    &lt;names> ... &lt;/names>
	'''    &lt;names> ... &lt;/names>
	''' 
	'''    // XML Serialization Form 2 ( Wrapped collection )
	'''    &lt;wrapperElement>
	'''       &lt;names> value-of-item &lt;/names>
	'''       &lt;names> value-of-item &lt;/names>
	'''       ....
	'''    &lt;/wrapperElement>
	''' </pre>
	''' 
	''' <p> The two serialized XML forms allow a null collection to be
	''' represented either by absence or presence of an element with a
	''' nillable attribute.
	''' 
	''' <p> <b>Usage</b> </p>
	''' <p>
	''' The <tt>@XmlElementWrapper</tt> annotation can be used with the
	''' following program elements:
	''' <ul>
	'''   <li> JavaBean property </li>
	'''   <li> non static, non transient field </li>
	''' </ul>
	''' 
	''' <p>The usage is subject to the following constraints:
	''' <ul>
	'''   <li> The property must be a collection property </li>
	'''   <li> This annotation can be used with the following annotations:
	'''            <seealso cref="XmlElement"/>,
	'''            <seealso cref="XmlElements"/>,
	'''            <seealso cref="XmlElementRef"/>,
	'''            <seealso cref="XmlElementRefs"/>,
	'''            <seealso cref="XmlJavaTypeAdapter"/>.</li>
	''' </ul>
	''' 
	''' <p>See "Package Specification" in javax.xml.bind.package javadoc for
	''' additional common information.</p>
	''' 
	''' @author <ul><li>Kohsuke Kawaguchi, Sun Microsystems, Inc.</li><li>Sekhar Vajjhala, Sun Microsystems, Inc.</li></ul> </summary>
	''' <seealso cref= XmlElement </seealso>
	''' <seealso cref= XmlElements </seealso>
	''' <seealso cref= XmlElementRef </seealso>
	''' <seealso cref= XmlElementRefs
	''' @since JAXB2.0
	'''  </seealso>

'JAVA TO VB CONVERTER TODO TASK: There is no attribute target in .NET corresponding to FIELD:
'JAVA TO VB CONVERTER TODO TASK: There is no attribute target in .NET corresponding to METHOD:
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	<AttributeUsage(<missing> Or <missing>, AllowMultiple := False, Inherited := False> _
	Public Class XmlElementWrapper
		Inherits System.Attribute

		''' <summary>
		''' Name of the XML wrapper element. By default, the XML wrapper
		''' element name is derived from the JavaBean property name.
		''' </summary>
		String name() default "##default"

		''' <summary>
		''' XML target namespace of the XML wrapper element.
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
		'''  Otherwise "" (which produces unqualified element in the default
		'''  namespace.
		''' </ol>
		''' </summary>
		String namespace() default "##default"

		''' <summary>
		''' If true, the absence of the collection is represented by
		''' using <tt>xsi:nil='true'</tt>. Otherwise, it is represented by
		''' the absence of the element.
		''' </summary>
		Boolean nillable() default False

		''' <summary>
		''' Customize the wrapper element declaration to be required.
		''' 
		''' <p>
		''' If required() is true, then the corresponding generated
		''' XML schema element declaration will have <tt>minOccurs="1"</tt>,
		''' to indicate that the wrapper element is always expected.
		''' 
		''' <p>
		''' Note that this only affects the schema generation, and
		''' not the unmarshalling or marshalling capability. This is
		''' simply a mechanism to let users express their application constraints
		''' better.
		''' 
		''' @since JAXB 2.1
		''' </summary>
		Boolean required() default False
	End Class

End Namespace
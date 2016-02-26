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
	''' Maps a JavaBean property to XML infoset representation and/or JAXB element.
	''' 
	''' <p>
	''' This annotation serves as a "catch-all" property while unmarshalling
	''' xml content into a instance of a JAXB annotated class. It typically
	''' annotates a multi-valued JavaBean property, but it can occur on
	''' single value JavaBean property. During unmarshalling, each xml element
	''' that does not match a static &#64;XmlElement or &#64;XmlElementRef
	''' annotation for the other JavaBean properties on the class, is added to this
	''' "catch-all" property.
	''' 
	''' <p>
	''' <h2>Usages:</h2>
	''' <pre>
	''' &#64;XmlAnyElement
	''' public <seealso cref="Element"/>[] others;
	''' 
	''' // Collection of <seealso cref="Element"/> or JAXB elements.
	''' &#64;XmlAnyElement(lax="true")
	''' public <seealso cref="Object"/>[] others;
	''' 
	''' &#64;XmlAnyElement
	''' private List&lt;<seealso cref="Element"/>> nodes;
	''' 
	''' &#64;XmlAnyElement
	''' private <seealso cref="Element"/> node;
	''' </pre>
	''' 
	''' <h2>Restriction usage constraints</h2>
	''' <p>
	''' This annotation is mutually exclusive with
	''' <seealso cref="XmlElement"/>, <seealso cref="XmlAttribute"/>, <seealso cref="XmlValue"/>,
	''' <seealso cref="XmlElements"/>, <seealso cref="XmlID"/>, and <seealso cref="XmlIDREF"/>.
	''' 
	''' <p>
	''' There can be only one <seealso cref="XmlAnyElement"/> annotated JavaBean property
	''' in a class and its super classes.
	''' 
	''' <h2>Relationship to other annotations</h2>
	''' <p>
	''' This annotation can be used with <seealso cref="XmlJavaTypeAdapter"/>, so that users
	''' can map their own data structure to DOM, which in turn can be composed
	''' into XML.
	''' 
	''' <p>
	''' This annotation can be used with <seealso cref="XmlMixed"/> like this:
	''' <pre>
	''' // List of java.lang.String or DOM nodes.
	''' &#64;XmlAnyElement &#64;XmlMixed
	''' List&lt;Object> others;
	''' </pre>
	''' 
	''' 
	''' <h2>Schema To Java example</h2>
	''' 
	''' The following schema would produce the following Java class:
	''' <pre>
	''' &lt;xs:complexType name="foo">
	'''   &lt;xs:sequence>
	'''     &lt;xs:element name="a" type="xs:int" />
	'''     &lt;xs:element name="b" type="xs:int" />
	'''     &lt;xs:any namespace="##other" processContents="lax" minOccurs="0" maxOccurs="unbounded" />
	'''   &lt;/xs:sequence>
	''' &lt;/xs:complexType>
	''' </pre>
	''' 
	''' <pre>
	''' class Foo {
	'''   int a;
	'''   int b;
	'''   &#64;<seealso cref="XmlAnyElement"/>
	'''   List&lt;Element> any;
	''' }
	''' </pre>
	''' 
	''' It can unmarshal instances like
	''' 
	''' <pre>
	''' &lt;foo xmlns:e="extra">
	'''   &lt;a>1</a>
	'''   &lt;e:other />  // this will be bound to DOM, because unmarshalling is orderless
	'''   &lt;b>3</b>
	'''   &lt;e:other />
	'''   &lt;c>5</c>     // this will be bound to DOM, because the annotation doesn't remember namespaces.
	''' &lt;/foo>
	''' </pre>
	''' 
	''' 
	''' 
	''' The following schema would produce the following Java class:
	''' <pre>
	''' &lt;xs:complexType name="bar">
	'''   &lt;xs:complexContent>
	'''   &lt;xs:extension base="foo">
	'''     &lt;xs:sequence>
	'''       &lt;xs:element name="c" type="xs:int" />
	'''       &lt;xs:any namespace="##other" processContents="lax" minOccurs="0" maxOccurs="unbounded" />
	'''     &lt;/xs:sequence>
	'''   &lt;/xs:extension>
	''' &lt;/xs:complexType>
	''' </pre>
	''' 
	''' <pre>
	''' class Bar extends Foo {
	'''   int c;
	'''   // Foo.getAny() also represents wildcard content for type definition bar.
	''' }
	''' </pre>
	''' 
	''' 
	''' It can unmarshal instances like
	''' 
	''' <pre>
	''' &lt;bar xmlns:e="extra">
	'''   &lt;a>1</a>
	'''   &lt;e:other />  // this will be bound to DOM, because unmarshalling is orderless
	'''   &lt;b>3</b>
	'''   &lt;e:other />
	'''   &lt;c>5</c>     // this now goes to Bar.c
	'''   &lt;e:other />  // this will go to Foo.any
	''' &lt;/bar>
	''' </pre>
	''' 
	''' 
	''' 
	''' 
	''' <h2>Using <seealso cref="XmlAnyElement"/> with <seealso cref="XmlElementRef"/></h2>
	''' <p>
	''' The <seealso cref="XmlAnyElement"/> annotation can be used with <seealso cref="XmlElementRef"/>s to
	''' designate additional elements that can participate in the content tree.
	''' 
	''' <p>
	''' The following schema would produce the following Java class:
	''' <pre>
	''' &lt;xs:complexType name="foo">
	'''   &lt;xs:choice maxOccurs="unbounded" minOccurs="0">
	'''     &lt;xs:element name="a" type="xs:int" />
	'''     &lt;xs:element name="b" type="xs:int" />
	'''     &lt;xs:any namespace="##other" processContents="lax" />
	'''   &lt;/xs:choice>
	''' &lt;/xs:complexType>
	''' </pre>
	''' 
	''' <pre>
	''' class Foo {
	'''   &#64;<seealso cref="XmlAnyElement"/>(lax="true")
	'''   &#64;<seealso cref="XmlElementRefs"/>({
	'''     &#64;<seealso cref="XmlElementRef"/>(name="a", type="JAXBElement.class")
	'''     &#64;<seealso cref="XmlElementRef"/>(name="b", type="JAXBElement.class")
	'''   })
	'''   <seealso cref="List"/>&lt;<seealso cref="Object"/>> others;
	''' }
	''' 
	''' &#64;XmlRegistry
	''' class ObjectFactory {
	'''   ...
	'''   &#64;XmlElementDecl(name = "a", namespace = "", scope = Foo.class)
	'''   <seealso cref="JAXBElement"/>&lt;Integer> createFooA( Integer i ) { ... }
	''' 
	'''   &#64;XmlElementDecl(name = "b", namespace = "", scope = Foo.class)
	'''   <seealso cref="JAXBElement"/>&lt;Integer> createFooB( Integer i ) { ... }
	''' </pre>
	''' 
	''' It can unmarshal instances like
	''' 
	''' <pre>
	''' &lt;foo xmlns:e="extra">
	'''   &lt;a>1</a>     // this will unmarshal to a <seealso cref="JAXBElement"/> instance whose value is 1.
	'''   &lt;e:other />  // this will unmarshal to a DOM <seealso cref="Element"/>.
	'''   &lt;b>3</b>     // this will unmarshal to a <seealso cref="JAXBElement"/> instance whose value is 1.
	''' &lt;/foo>
	''' </pre>
	''' 
	''' 
	''' 
	''' 
	''' <h2>W3C XML Schema "lax" wildcard emulation</h2>
	''' The lax element of the annotation enables the emulation of the "lax" wildcard semantics.
	''' For example, when the Java source code is annotated like this:
	''' <pre>
	''' &#64;<seealso cref="XmlRootElement"/>
	''' class Foo {
	'''   &#64;XmlAnyElement(lax=true)
	'''   public <seealso cref="Object"/>[] others;
	''' }
	''' </pre>
	''' then the following document will unmarshal like this:
	''' <pre>
	''' &lt;foo>
	'''   &lt;unknown />
	'''   &lt;foo />
	''' &lt;/foo>
	''' 
	''' Foo foo = unmarshal();
	''' // 1 for 'unknown', another for 'foo'
	''' assert foo.others.length==2;
	''' // 'unknown' unmarshals to a DOM element
	''' assert foo.others[0] instanceof Element;
	''' // because of lax=true, the 'foo' element eagerly
	''' // unmarshals to a Foo object.
	''' assert foo.others[1] instanceof Foo;
	''' </pre>
	''' 
	''' @author Kohsuke Kawaguchi
	''' @since JAXB2.0
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no attribute target in .NET corresponding to FIELD:
'JAVA TO VB CONVERTER TODO TASK: There is no attribute target in .NET corresponding to METHOD:
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	<AttributeUsage(<missing> Or <missing>, AllowMultiple := False, Inherited := False> _
	Public Class XmlAnyElement
		Inherits System.Attribute

		''' <summary>
		''' Controls the unmarshaller behavior when it sees elements
		''' known to the current <seealso cref="JAXBContext"/>.
		''' 
		''' <h3>When false</h3>
		''' <p>
		''' If false, all the elements that match the property will be unmarshalled
		''' to DOM, and the property will only contain DOM elements.
		''' 
		''' <h3>When true</h3>
		''' <p>
		''' If true, when an element matches a property marked with <seealso cref="XmlAnyElement"/>
		''' is known to <seealso cref="JAXBContext"/> (for example, there's a class with
		''' <seealso cref="XmlRootElement"/> that has the same tag name, or there's
		''' <seealso cref="XmlElementDecl"/> that has the same tag name),
		''' the unmarshaller will eagerly unmarshal this element to the JAXB object,
		''' instead of unmarshalling it to DOM. Additionally, if the element is
		''' unknown but it has a known xsi:type, the unmarshaller eagerly unmarshals
		''' the element to a <seealso cref="JAXBElement"/>, with the unknown element name and
		''' the JAXBElement value is set to an instance of the JAXB mapping of the
		''' known xsi:type.
		''' 
		''' <p>
		''' As a result, after the unmarshalling, the property can become heterogeneous;
		''' it can have both DOM nodes and some JAXB objects at the same time.
		''' 
		''' <p>
		''' This can be used to emulate the "lax" wildcard semantics of the W3C XML Schema.
		''' </summary>
		Boolean lax() default False

		''' <summary>
		''' Specifies the <seealso cref="DomHandler"/> which is responsible for actually
		''' converting XML from/to a DOM-like data structure.
		''' </summary>
		Type value() default GetType(W3CDomHandler)
	End Class

End Namespace
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
	''' Maps a factory method to a XML element.
	''' 
	''' <p> <b>Usage</b> </p>
	''' 
	''' The annotation creates a mapping between an XML schema element
	''' declaration and a <i> element factory method </i> that returns a
	''' JAXBElement instance representing the element
	''' declaration. Typically, the element factory method is generated
	''' (and annotated) from a schema into the ObjectFactory class in a
	''' Java package that represents the binding of the element
	''' declaration's target namespace. Thus, while the annotation syntax
	''' allows &#64;XmlElementDecl to be used on any method, semantically
	''' its use is restricted to annotation of element factory method.
	''' 
	''' The usage is subject to the following constraints:
	''' 
	''' <ul>
	'''   <li> The class containing the element factory method annotated
	'''        with &#64;XmlElementDecl must be marked with {@link
	'''        XmlRegistry}. </li>
	'''   <li> The element factory method must take one parameter
	'''        assignable to <seealso cref="Object"/>.</li>
	''' </ul>
	''' 
	''' <p><b>Example 1: </b>Annotation on a factory method
	''' <pre>
	'''     // Example: code fragment
	'''     &#64;XmlRegistry
	'''     class ObjectFactory {
	'''         &#64;XmlElementDecl(name="foo")
	'''         JAXBElement&lt;String> createFoo(String s) { ... }
	'''     }
	''' </pre>
	''' <pre>
	'''     &lt;!-- XML input -->
	'''       &lt;foo>string</foo>
	''' 
	'''     // Example: code fragment corresponding to XML input
	'''     JAXBElement<String> o =
	'''     (JAXBElement<String>)unmarshaller.unmarshal(aboveDocument);
	'''     // print JAXBElement instance to show values
	'''     System.out.println(o.getName());   // prints  "{}foo"
	'''     System.out.println(o.getValue());  // prints  "string"
	'''     System.out.println(o.getValue().getClass()); // prints "java.lang.String"
	''' 
	'''     &lt;!-- Example: XML schema definition -->
	'''     &lt;xs:element name="foo" type="xs:string"/>
	''' </pre>
	''' 
	''' <p><b>Example 2: </b> Element declaration with non local scope
	''' <p>
	''' The following example illustrates the use of scope annotation
	''' parameter in binding of element declaration in schema derived
	''' code.
	''' <p>
	''' The following example may be replaced in a future revision of
	''' this javadoc.
	''' 
	''' <pre>
	'''     &lt;!-- Example: XML schema definition -->
	'''     &lt;xs:schema>
	'''       &lt;xs:complexType name="pea">
	'''         &lt;xs:choice maxOccurs="unbounded">
	'''           &lt;xs:element name="foo" type="xs:string"/>
	'''           &lt;xs:element name="bar" type="xs:string"/>
	'''         &lt;/xs:choice>
	'''       &lt;/xs:complexType>
	'''       &lt;xs:element name="foo" type="xs:int"/>
	'''     &lt;/xs:schema>
	''' </pre>
	''' <pre>
	'''     // Example: expected default binding
	'''     class Pea {
	'''         &#64;XmlElementRefs({
	'''             &#64;XmlElementRef(name="foo",type=JAXBElement.class)
	'''             &#64;XmlElementRef(name="bar",type=JAXBElement.class)
	'''         })
	'''         List&lt;JAXBElement&lt;String>> fooOrBar;
	'''     }
	''' 
	'''     &#64;XmlRegistry
	'''     class ObjectFactory {
	'''         &#64;XmlElementDecl(scope=Pea.class,name="foo")
	'''         JAXBElement<String> createPeaFoo(String s);
	''' 
	'''         &#64;XmlElementDecl(scope=Pea.class,name="bar")
	'''         JAXBElement<String> createPeaBar(String s);
	''' 
	'''         &#64;XmlElementDecl(name="foo")
	'''         JAXBElement<Integer> createFoo(Integer i);
	'''     }
	''' 
	''' </pre>
	''' Without scope createFoo and createPeaFoo would become ambiguous
	''' since both of them map to a XML schema element with the same local
	''' name "foo".
	''' </summary>
	''' <seealso cref= XmlRegistry
	''' @since JAXB 2.0 </seealso>
'JAVA TO VB CONVERTER TODO TASK: There is no attribute target in .NET corresponding to METHOD:
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	<AttributeUsage(<missing>, AllowMultiple := False, Inherited := False> _
	Public Class XmlElementDecl
		Inherits System.Attribute

		''' <summary>
		''' scope of the mapping.
		''' 
		''' <p>
		''' If this is not <seealso cref="XmlElementDecl.GLOBAL"/>, then this element
		''' declaration mapping is only active within the specified class.
		''' </summary>
		Type scope() default GetType(GLOBAL)

		''' <summary>
		''' namespace name of the XML element.
		''' <p>
		''' If the value is "##default", then the value is the namespace
		''' name for the package of the class containing this factory method.
		''' </summary>
		''' <seealso cref= #name() </seealso>
		String namespace() default "##default"

		''' <summary>
		''' local name of the XML element.
		''' 
		''' <p>
		''' <b> Note to reviewers: </b> There is no default name; since
		''' the annotation is on a factory method, it is not clear that the
		''' method name can be derived from the factory method name. </summary>
		''' <seealso cref= #namespace() </seealso>
		String name()

		''' <summary>
		''' namespace name of a substitution group's head XML element.
		''' <p>
		''' This specifies the namespace name of the XML element whose local
		''' name is specified by <tt>substitutionHeadName()</tt>.
		''' <p>
		''' If <tt>susbtitutionHeadName()</tt> is "", then this
		''' value can only be "##default". But the value is ignored since
		''' since this element is not part of susbtitution group when the
		''' value of <tt>susbstitutionHeadName()</tt> is "".
		''' <p>
		''' If <tt>susbtitutionHeadName()</tt> is not "" and the value is
		''' "##default", then the namespace name is the namespace name to
		''' which the package of the containing class, marked with {@link
		''' XmlRegistry }, is mapped.
		''' <p>
		''' If <tt>susbtitutionHeadName()</tt> is not "" and the value is
		''' not "##default", then the value is the namespace name.
		''' </summary>
		''' <seealso cref= #substitutionHeadName() </seealso>
		String substitutionHeadNamespace() default "##default"

		''' <summary>
		''' XML local name of a substitution group's head element.
		''' <p>
		''' If the value is "", then this element is not part of any
		''' substitution group.
		''' </summary>
		''' <seealso cref= #substitutionHeadNamespace() </seealso>
		String substitutionHeadName() default ""

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
		''' Used in <seealso cref="XmlElementDecl#scope()"/> to
		''' signal that the declaration is in the global scope.
		''' </summary>
		public final class GLOBAL
	End Class

End Namespace
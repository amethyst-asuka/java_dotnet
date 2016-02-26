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
	''' Used to map a property to a list simple type.
	''' 
	''' <p><b>Usage</b> </p>
	''' <p>
	''' The <tt>@XmlList</tt> annotation can be used with the
	''' following program elements:
	''' <ul>
	'''   <li> JavaBean property </li>
	'''   <li> field </li>
	''' </ul>
	''' 
	''' <p>
	''' When a collection property is annotated just with @XmlElement,
	''' each item in the collection will be wrapped by an element.
	''' For example,
	''' 
	''' <pre>
	''' &#64;XmlRootElement
	''' class Foo {
	'''     &#64;XmlElement
	'''     List&lt;String> data;
	''' }
	''' </pre>
	''' 
	''' would produce XML like this:
	''' 
	''' <pre>
	''' &lt;foo>
	'''   &lt;data>abc</data>
	'''   &lt;data>def</data>
	''' &lt;/foo>
	''' </pre>
	''' 
	''' &#64;XmlList annotation, on the other hand, allows multiple values to be
	''' represented as whitespace-separated tokens in a single element. For example,
	''' 
	''' <pre>
	''' &#64;XmlRootElement
	''' class Foo {
	'''     &#64;XmlElement
	'''     &#64;XmlList
	'''     List&lt;String> data;
	''' }
	''' </pre>
	''' 
	''' the above code will produce XML like this:
	''' 
	''' <pre>
	''' &lt;foo>
	'''   &lt;data>abc def</data>
	''' &lt;/foo>
	''' </pre>
	''' 
	''' <p>This annotation can be used with the following annotations:
	'''        <seealso cref="XmlElement"/>,
	'''        <seealso cref="XmlAttribute"/>,
	'''        <seealso cref="XmlValue"/>,
	'''        <seealso cref="XmlIDREF"/>.
	'''  <ul>
	'''    <li> The use of <tt>@XmlList</tt> with <seealso cref="XmlValue"/> while
	'''         allowed, is redundant since  <seealso cref="XmlList"/> maps a
	'''         collection type to a simple schema type that derives by
	'''         list just as <seealso cref="XmlValue"/> would. </li>
	''' 
	'''    <li> The use of <tt>@XmlList</tt> with <seealso cref="XmlAttribute"/> while
	'''         allowed, is redundant since  <seealso cref="XmlList"/> maps a
	'''         collection type to a simple schema type that derives by
	'''         list just as <seealso cref="XmlAttribute"/> would. </li>
	'''  </ul>
	''' 
	''' @author <ul><li>Kohsuke Kawaguchi, Sun Microsystems, Inc.</li><li>Sekhar Vajjhala, Sun Microsystems, Inc.</li></ul>
	''' @since JAXB2.0
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no attribute target in .NET corresponding to FIELD:
'JAVA TO VB CONVERTER TODO TASK: There is no attribute target in .NET corresponding to METHOD:
'JAVA TO VB CONVERTER TODO TASK: There is no attribute target in .NET corresponding to PARAMETER:
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	<AttributeUsage(<missing> Or <missing> Or <missing>, AllowMultiple := False, Inherited := False> _
	Public Class XmlList
		Inherits System.Attribute

	End Class

End Namespace
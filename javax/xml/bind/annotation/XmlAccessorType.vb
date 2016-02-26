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
	''' <p> Controls whether fields or Javabean properties are serialized by default. </p>
	''' 
	''' <p> <b> Usage </b> </p>
	''' 
	''' <p> <tt>@XmlAccessorType</tt> annotation can be used with the following program elements:</p>
	''' 
	''' <ul>
	'''   <li> package</li>
	'''   <li> a top level class </li>
	''' </ul>
	''' 
	''' <p> See "Package Specification" in javax.xml.bind.package javadoc for
	''' additional common information.</p>
	''' 
	''' <p>This annotation provides control over the default serialization
	''' of properties and fields in a class.
	''' 
	''' <p>The annotation <tt> @XmlAccessorType </tt> on a package applies to
	''' all classes in the package. The following inheritance
	''' semantics apply:
	''' 
	''' <ul>
	'''   <li> If there is a <tt>@XmlAccessorType</tt> on a class, then it
	'''        is used. </li>
	'''   <li> Otherwise, if a <tt>@XmlAccessorType</tt> exists on one of
	'''        its super classes, then it is inherited.
	'''   <li> Otherwise, the <tt>@XmlAccessorType </tt> on a package is
	'''        inherited.
	''' </ul>
	''' <p> <b> Defaulting Rules: </b> </p>
	''' 
	''' <p>By default, if <tt>@XmlAccessorType </tt> on a package is absent,
	''' then the following package level annotation is assumed.</p>
	''' <pre>
	'''   &#64;XmlAccessorType(XmlAccessType.PUBLIC_MEMBER)
	''' </pre>
	''' <p> By default, if <tt>@XmlAccessorType</tt> on a class is absent,
	''' and none of its super classes is annotated with
	''' <tt>@XmlAccessorType</tt>, then the following default on the class
	''' is assumed: </p>
	''' <pre>
	'''   &#64;XmlAccessorType(XmlAccessType.PUBLIC_MEMBER)
	''' </pre>
	''' <p>This annotation can be used with the following annotations:
	'''    <seealso cref="XmlType"/>, <seealso cref="XmlRootElement"/>, <seealso cref="XmlAccessorOrder"/>,
	'''    <seealso cref="XmlSchema"/>, <seealso cref="XmlSchemaType"/>, <seealso cref="XmlSchemaTypes"/>,
	'''    , <seealso cref="XmlJavaTypeAdapter"/>. It can also be used with the
	'''    following annotations at the package level: <seealso cref="XmlJavaTypeAdapter"/>.
	''' 
	''' @author Sekhar Vajjhala, Sun Microsystems, Inc.
	''' @since JAXB2.0 </summary>
	''' <seealso cref= XmlAccessType </seealso>

'JAVA TO VB CONVERTER TODO TASK: There is no attribute target in .NET corresponding to PACKAGE:
'JAVA TO VB CONVERTER TODO TASK: There is no attribute target in .NET corresponding to TYPE:
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	<AttributeUsage(<missing> Or <missing>, AllowMultiple := False, Inherited := True> _
	Public Class XmlAccessorType
		Inherits System.Attribute

		''' <summary>
		''' Specifies whether fields or properties are serialized.
		''' </summary>
		''' <seealso cref= XmlAccessType </seealso>
		XmlAccessType value() default XmlAccessType.PUBLIC_MEMBER
	End Class

End Namespace
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
	''' <p> Controls the ordering of fields and properties in a class. </p>
	''' 
	''' <h3>Usage </h3>
	''' 
	''' <p> <tt> @XmlAccessorOrder </tt> annotation can be used with the following
	''' program elements:</p>
	''' 
	''' <ul>
	'''   <li> package</li>
	'''   <li> a top level class </li>
	''' </ul>
	''' 
	''' <p> See "Package Specification" in <tt>javax.xml.bind</tt> package javadoc for
	''' additional common information.</p>
	''' 
	''' <p>The effective <seealso cref="XmlAccessOrder"/> on a class is determined
	''' as follows:
	''' 
	''' <ul>
	'''   <li> If there is a <tt>@XmlAccessorOrder</tt> on a class, then
	'''        it is used. </li>
	'''   <li> Otherwise, if a <tt>@XmlAccessorOrder </tt> exists on one of
	'''        its super classes, then it is inherited (by the virtue of
	'''        <seealso cref="Inherited"/>)
	'''   <li> Otherwise, the <tt>@XmlAccessorOrder</tt> on the package
	'''        of the class is used, if it's there.
	'''   <li> Otherwise <seealso cref="XmlAccessOrder#UNDEFINED"/>.
	''' </ul>
	''' 
	''' <p>This annotation can be used with the following annotations:
	'''    <seealso cref="XmlType"/>, <seealso cref="XmlRootElement"/>, <seealso cref="XmlAccessorType"/>,
	'''    <seealso cref="XmlSchema"/>, <seealso cref="XmlSchemaType"/>, <seealso cref="XmlSchemaTypes"/>,
	'''    , <seealso cref="XmlJavaTypeAdapter"/>. It can also be used with the
	'''    following annotations at the package level: <seealso cref="XmlJavaTypeAdapter"/>.
	''' 
	''' @author Sekhar Vajjhala, Sun Microsystems, Inc.
	''' @since JAXB2.0 </summary>
	''' <seealso cref= XmlAccessOrder </seealso>

'JAVA TO VB CONVERTER TODO TASK: There is no attribute target in .NET corresponding to PACKAGE:
'JAVA TO VB CONVERTER TODO TASK: There is no attribute target in .NET corresponding to TYPE:
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	<AttributeUsage(<missing> Or <missing>, AllowMultiple := False, Inherited := True> _
	Public Class XmlAccessorOrder
		Inherits System.Attribute

			XmlAccessOrder value() default XmlAccessOrder.UNDEFINED
	End Class

End Namespace
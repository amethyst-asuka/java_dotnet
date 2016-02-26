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
	''' <p>
	''' Maps a JavaBean property to a map of wildcard attributes.
	''' 
	''' <p> <b>Usage</b> </p>
	''' <p>
	''' The <tt>&#64;XmlAnyAttribute</tt> annotation can be used with the
	''' following program elements:
	''' <ul>
	'''   <li> JavaBean property </li>
	'''   <li> non static, non transient field </li>
	''' </ul>
	''' 
	''' <p>See "Package Specification" in javax.xml.bind.package javadoc for
	''' additional common information.</p>
	''' 
	''' The usage is subject to the following constraints:
	''' <ul>
	'''   <li> At most one field or property in a class can be annotated
	'''        with <tt>&#64;XmlAnyAttribute</tt>.  </li>
	'''   <li> The type of the property or the field must <tt>java.util.Map</tt> </li>
	''' </ul>
	''' 
	''' <p>
	''' While processing attributes to be unmarshalled into a value class,
	''' each attribute that is not statically associated with another
	''' JavaBean property, via <seealso cref="XmlAttribute"/>, is entered into the
	''' wildcard attribute map represented by
	''' <seealso cref="Map"/>&lt;<seealso cref="QName"/>,<seealso cref="Object"/>>. The attribute QName is the
	''' map's key. The key's value is the String value of the attribute.
	''' 
	''' @author Kohsuke Kawaguchi, Sun Microsystems, Inc.
	''' @since JAXB2.0
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no attribute target in .NET corresponding to FIELD:
'JAVA TO VB CONVERTER TODO TASK: There is no attribute target in .NET corresponding to METHOD:
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	<AttributeUsage(<missing> Or <missing>, AllowMultiple := False, Inherited := False> _
	Public Class XmlAnyAttribute
		Inherits System.Attribute

	End Class

End Namespace
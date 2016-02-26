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
	''' Marks a property that refers to classes with <seealso cref="XmlElement"/>
	''' or JAXBElement.
	''' 
	''' <p>
	''' Compared to an element property (property with <seealso cref="XmlElement"/>
	''' annotation), a reference property has a different substitution semantics.
	''' When a sub-class is assigned to a property, an element property produces
	''' the same tag name with @xsi:type, whereas a reference property produces
	''' a different tag name (the tag name that's on the the sub-class.)
	''' 
	''' <p> This annotation can be used with the following annotations:
	''' <seealso cref="XmlJavaTypeAdapter"/>, <seealso cref="XmlElementWrapper"/>.
	''' 
	''' @author <ul><li>Kohsuke Kawaguchi, Sun Microsystems, Inc.</li><li>Sekhar Vajjhala, Sun Microsystems, Inc.</li></ul>
	''' </summary>
	''' <seealso cref= XmlElementWrapper </seealso>
	''' <seealso cref= XmlElementRef
	''' @since JAXB2.0 </seealso>
'JAVA TO VB CONVERTER TODO TASK: There is no attribute target in .NET corresponding to FIELD:
'JAVA TO VB CONVERTER TODO TASK: There is no attribute target in .NET corresponding to METHOD:
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	<AttributeUsage(<missing> Or <missing>, AllowMultiple := False, Inherited := False> _
	Public Class XmlElementRefs
		Inherits System.Attribute

		XmlElementRef() value()
	End Class

End Namespace
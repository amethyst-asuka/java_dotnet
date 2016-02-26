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
	''' Associates the MIME type that controls the XML representation of the property.
	''' 
	''' <p>
	''' This annotation is used in conjunction with datatypes such as
	''' <seealso cref="java.awt.Image"/> or <seealso cref="Source"/> that are bound to base64-encoded binary in XML.
	''' 
	''' <p>
	''' If a property that has this annotation has a sibling property bound to
	''' the xmime:contentType attribute, and if in the instance the property has a value,
	''' the value of the attribute takes precedence and that will control the marshalling.
	''' 
	''' @author Kohsuke Kawaguchi
	''' @since JAXB2.0
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no attribute target in .NET corresponding to FIELD:
'JAVA TO VB CONVERTER TODO TASK: There is no attribute target in .NET corresponding to METHOD:
'JAVA TO VB CONVERTER TODO TASK: There is no attribute target in .NET corresponding to PARAMETER:
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	<AttributeUsage(<missing> Or <missing> Or <missing>, AllowMultiple := False, Inherited := False> _
	Public Class XmlMimeType
		Inherits System.Attribute

		''' <summary>
		''' The textual representation of the MIME type,
		''' such as "image/jpeg" "image/*", "text/xml; charset=iso-8859-1" and so on.
		''' </summary>
		String value()
	End Class

End Namespace
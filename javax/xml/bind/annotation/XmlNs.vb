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
	''' <p>
	''' Associates a namespace prefix with a XML namespace URI.
	''' 
	''' <p><b>Usage</b></p>
	''' <p><tt>@XmlNs</tt> annotation is intended for use from other
	''' program annotations.
	''' 
	''' <p>See "Package Specification" in javax.xml.bind.package javadoc for
	''' additional common information.</p>
	''' 
	''' <p><b>Example:</b>See <tt>XmlSchema</tt> annotation type for an example.
	''' @author Sekhar Vajjhala, Sun Microsystems, Inc.
	''' @since JAXB2.0
	''' </summary>

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	<AttributeUsage(, AllowMultiple := False, Inherited := False> _
	Public Class XmlNs
		Inherits System.Attribute

		''' <summary>
		''' Namespace prefix
		''' </summary>
		String prefix()

		''' <summary>
		''' Namespace URI
		''' </summary>
		String namespaceURI()
	End Class

End Namespace
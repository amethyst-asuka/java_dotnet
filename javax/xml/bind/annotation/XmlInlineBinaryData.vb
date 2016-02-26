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
	''' Disable consideration of XOP encoding for datatypes that are bound to
	''' base64-encoded binary data in XML.
	''' 
	''' <p>
	''' When XOP encoding is enabled as described in <seealso cref="AttachmentMarshaller#isXOPPackage()"/>, this annotation disables datatypes such as <seealso cref="java.awt.Image"/> or <seealso cref="Source"/> or <tt>byte[]</tt> that are bound to base64-encoded binary from being considered for
	''' XOP encoding. If a JAXB property is annotated with this annotation or if
	''' the JAXB property's base type is annotated with this annotation,
	''' neither
	''' <seealso cref="AttachmentMarshaller#addMtomAttachment(DataHandler, String, String)"/>
	''' nor
	''' <seealso cref="AttachmentMarshaller#addMtomAttachment(byte[], int, int, String, String, String)"/> is
	''' ever called for the property. The binary data will always be inlined.
	''' 
	''' @author Joseph Fialli
	''' @since JAXB2.0
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no attribute target in .NET corresponding to FIELD:
'JAVA TO VB CONVERTER TODO TASK: There is no attribute target in .NET corresponding to METHOD:
'JAVA TO VB CONVERTER TODO TASK: There is no attribute target in .NET corresponding to TYPE:
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	<AttributeUsage(<missing> Or <missing> Or <missing>, AllowMultiple := False, Inherited := False> _
	Public Class XmlInlineBinaryData
		Inherits System.Attribute

	End Class

End Namespace
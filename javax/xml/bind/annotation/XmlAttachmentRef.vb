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
	''' Marks a field/property that its XML form is a uri reference to mime content.
	''' The mime content is optimally stored out-of-line as an attachment.
	''' 
	''' A field/property must always map to the <seealso cref="DataHandler"/> class.
	''' 
	''' <h2>Usage</h2>
	''' <pre>
	''' &#64;<seealso cref="XmlRootElement"/>
	''' class Foo {
	'''   &#64;<seealso cref="XmlAttachmentRef"/>
	'''   &#64;<seealso cref="XmlAttribute"/>
	'''   <seealso cref="DataHandler"/> data;
	''' 
	'''   &#64;<seealso cref="XmlAttachmentRef"/>
	'''   &#64;<seealso cref="XmlElement"/>
	'''   <seealso cref="DataHandler"/> body;
	''' }
	''' </pre>
	''' The above code maps to the following XML:
	''' <pre>
	''' &lt;xs:element name="foo" xmlns:ref="http://ws-i.org/profiles/basic/1.1/xsd">
	'''   &lt;xs:complexType>
	'''     &lt;xs:sequence>
	'''       &lt;xs:element name="body" type="ref:swaRef" minOccurs="0" />
	'''     &lt;/xs:sequence>
	'''     &lt;xs:attribute name="data" type="ref:swaRef" use="optional" />
	'''   &lt;/xs:complexType>
	''' &lt;/xs:element>
	''' </pre>
	''' 
	''' <p>
	''' The above binding supports WS-I AP 1.0 <a href="http://www.ws-i.org/Profiles/AttachmentsProfile-1.0-2004-08-24.html#Referencing_Attachments_from_the_SOAP_Envelope">WS-I Attachments Profile Version 1.0.</a>
	''' 
	''' @author Kohsuke Kawaguchi
	''' @since JAXB2.0
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no attribute target in .NET corresponding to FIELD:
'JAVA TO VB CONVERTER TODO TASK: There is no attribute target in .NET corresponding to METHOD:
'JAVA TO VB CONVERTER TODO TASK: There is no attribute target in .NET corresponding to PARAMETER:
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	<AttributeUsage(<missing> Or <missing> Or <missing>, AllowMultiple := False, Inherited := False> _
	Public Class XmlAttachmentRef
		Inherits System.Attribute

	End Class

End Namespace
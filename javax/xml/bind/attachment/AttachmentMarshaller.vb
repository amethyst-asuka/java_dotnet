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

Namespace javax.xml.bind.attachment


	''' <summary>
	''' <p>Enable JAXB marshalling to optimize storage of binary data.</p>
	''' 
	''' <p>This API enables an efficient cooperative creation of optimized
	''' binary data formats between a JAXB marshalling process and a MIME-based package
	''' processor. A JAXB implementation marshals the root body of a MIME-based package,
	''' delegating the creation of referenceable MIME parts to
	''' the MIME-based package processor that implements this abstraction.</p>
	''' 
	''' <p>XOP processing is enabled when <seealso cref="#isXOPPackage()"/> is true.
	'''    See <seealso cref="#addMtomAttachment(DataHandler, String, String)"/> for details.
	''' </p>
	''' 
	''' <p>WS-I Attachment Profile 1.0 is supported by
	''' <seealso cref="#addSwaRefAttachment(DataHandler)"/> being called by the
	''' marshaller for each JAXB property related to
	''' {http://ws-i.org/profiles/basic/1.1/xsd}swaRef.</p>
	''' 
	''' 
	''' @author Marc Hadley
	''' @author Kohsuke Kawaguchi
	''' @author Joseph Fialli
	''' @since JAXB 2.0
	''' </summary>
	''' <seealso cref= Marshaller#setAttachmentMarshaller(AttachmentMarshaller)
	''' </seealso>
	''' <seealso cref= <a href="http://www.w3.org/TR/2005/REC-xop10-20050125/">XML-binary Optimized Packaging</a> </seealso>
	''' <seealso cref= <a href="http://www.ws-i.org/Profiles/AttachmentsProfile-1.0-2004-08-24.html">WS-I Attachments Profile Version 1.0.</a> </seealso>
	Public MustInherit Class AttachmentMarshaller

		''' <summary>
		''' <p>Consider MIME content <code>data</code> for optimized binary storage as an attachment.
		''' 
		''' <p>
		''' This method is called by JAXB marshal process when <seealso cref="#isXOPPackage()"/> is
		''' <code>true</code>, for each element whose datatype is "base64Binary", as described in
		''' Step 3 in
		''' <a href="http://www.w3.org/TR/2005/REC-xop10-20050125/#creating_xop_packages">Creating XOP Packages</a>.
		''' 
		''' <p>
		''' The method implementor determines whether <code>data</code> shall be attached separately
		''' or inlined as base64Binary data. If the implementation chooses to optimize the storage
		''' of the binary data as a MIME part, it is responsible for attaching <code>data</code> to the
		''' MIME-based package, and then assigning an unique content-id, cid, that identifies
		''' the MIME part within the MIME message. This method returns the cid,
		''' which enables the JAXB marshaller to marshal a XOP element that refers to that cid in place
		''' of marshalling the binary data. When the method returns null, the JAXB marshaller
		''' inlines <code>data</code> as base64binary data.
		''' 
		''' <p>
		''' The caller of this method is required to meet the following constraint.
		''' If the element infoset item containing <code>data</code> has the attribute
		''' <code>xmime:contentType</code> or if the JAXB property/field representing
		''' <code>data</code>is annotated with a known MIME type,
		''' <code>data.getContentType()</code> should be set to that MIME type.
		''' 
		''' <p>
		''' The <code>elementNamespace</code> and <code>elementLocalName</code>
		''' parameters provide the
		''' context that contains the binary data. This information could
		''' be used by the MIME-based package processor to determine if the
		''' binary data should be inlined or optimized as an attachment.
		''' </summary>
		''' <param name="data">
		'''       represents the data to be attached. Must be non-null. </param>
		''' <param name="elementNamespace">
		'''      the namespace URI of the element that encloses the base64Binary data.
		'''      Can be empty but never null. </param>
		''' <param name="elementLocalName">
		'''      The local name of the element. Always a non-null valid string.
		''' 
		''' @return
		'''     a valid content-id URI (see <a href="http://www.w3.org/TR/xop10/#RFC2387">RFC 2387</a>) that identifies the attachment containing <code>data</code>.
		'''     Otherwise, null if the attachment was not added and should instead be inlined in the message.
		''' </param>
		''' <seealso cref= <a href="http://www.w3.org/TR/2005/REC-xop10-20050125/">XML-binary Optimized Packaging</a> </seealso>
		''' <seealso cref= <a href="http://www.w3.org/TR/xml-media-types/">Describing Media Content of Binary Data in XML</a> </seealso>
		Public MustOverride Function addMtomAttachment(ByVal data As javax.activation.DataHandler, ByVal elementNamespace As String, ByVal elementLocalName As String) As String

		''' <summary>
		''' <p>Consider binary <code>data</code> for optimized binary storage as an attachment.
		''' 
		''' <p>Since content type is not known, the attachment's MIME content type must be set to "application/octet-stream".</p>
		''' 
		''' <p>
		''' The <code>elementNamespace</code> and <code>elementLocalName</code>
		''' parameters provide the
		''' context that contains the binary data. This information could
		''' be used by the MIME-based package processor to determine if the
		''' binary data should be inlined or optimized as an attachment.
		''' </summary>
		''' <param name="data">
		'''      represents the data to be attached. Must be non-null. The actual data region is
		'''      specified by <tt>(data,offset,length)</tt> tuple.
		''' </param>
		''' <param name="offset">
		'''       The offset within the array of the first byte to be read;
		'''       must be non-negative and no larger than array.length
		''' </param>
		''' <param name="length">
		'''       The number of bytes to be read from the given array;
		'''       must be non-negative and no larger than array.length
		''' </param>
		''' <param name="mimeType">
		'''      If the data has an associated MIME type known to JAXB, that is passed
		'''      as this parameter. If none is known, "application/octet-stream".
		'''      This parameter may never be null.
		''' </param>
		''' <param name="elementNamespace">
		'''      the namespace URI of the element that encloses the base64Binary data.
		'''      Can be empty but never null.
		''' </param>
		''' <param name="elementLocalName">
		'''      The local name of the element. Always a non-null valid string.
		''' </param>
		''' <returns> content-id URI, cid, to the attachment containing
		'''         <code>data</code> or null if data should be inlined.
		''' </returns>
		''' <seealso cref= #addMtomAttachment(DataHandler, String, String) </seealso>
		Public MustOverride Function addMtomAttachment(ByVal data As SByte(), ByVal offset As Integer, ByVal length As Integer, ByVal mimeType As String, ByVal elementNamespace As String, ByVal elementLocalName As String) As String

		''' <summary>
		''' <p>Read-only property that returns true if JAXB marshaller should enable XOP creation.</p>
		''' 
		''' <p>This value must not change during the marshalling process. When this
		''' value is true, the <code>addMtomAttachment(...)</code> method
		''' is invoked when the appropriate binary datatypes are encountered by
		''' the marshal process.</p>
		''' 
		''' <p>Marshaller.marshal() must throw IllegalStateException if this value is <code>true</code>
		''' and the XML content to be marshalled violates Step 1 in
		''' <a href="http://www.w3.org/TR/2005/REC-xop10-20050125/#creating_xop_packages">Creating XOP Pacakges</a>
		''' http://www.w3.org/TR/2005/REC-xop10-20050125/#creating_xop_packages.
		''' <i>"Ensure the Original XML Infoset contains no element information item with a
		''' [namespace name] of "http://www.w3.org/2004/08/xop/include" and a [local name] of Include"</i>
		''' 
		''' <p>When this method returns true and during the marshal process
		''' at least one call to <code>addMtomAttachment(...)</code> returns
		''' a content-id, the MIME-based package processor must label the
		''' root part with the application/xop+xml media type as described in
		''' Step 5 of
		''' <a href="http://www.w3.org/TR/2005/REC-xop10-20050125/#creating_xop_packages">Creating XOP Pacakges</a>.<p>
		''' </summary>
		''' <returns> true when MIME context is a XOP Package. </returns>
		Public Overridable Property xOPPackage As Boolean
			Get
				Return False
			End Get
		End Property

	   ''' <summary>
	   ''' <p>Add MIME <code>data</code> as an attachment and return attachment's content-id, cid.</p>
	   ''' 
	   ''' <p>
	   ''' This method is called by JAXB marshal process for each element/attribute typed as
	   ''' {http://ws-i.org/profiles/basic/1.1/xsd}swaRef. The MIME-based package processor
	   ''' implementing this method is responsible for attaching the specified data to a
	   ''' MIME attachment, and generating a content-id, cid, that uniquely identifies the attachment
	   ''' within the MIME-based package.
	   ''' 
	   ''' <p>Caller inserts the returned content-id, cid, into the XML content being marshalled.</p>
	   ''' </summary>
	   ''' <param name="data">
	   '''       represents the data to be attached. Must be non-null.
	   ''' @return
	   '''       must be a valid URI used as cid. Must satisfy Conformance Requirement R2928 from
	   '''       <a href="http://www.ws-i.org/Profiles/AttachmentsProfile-1.0-2004-08-24.html#Referencing_Attachments_from_the_SOAP_Envelope">WS-I Attachments Profile Version 1.0.</a> </param>
		Public MustOverride Function addSwaRefAttachment(ByVal data As javax.activation.DataHandler) As String
	End Class

End Namespace
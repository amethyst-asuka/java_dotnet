Imports System.Collections

'
' * Copyright (c) 2004, 2012, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.xml.soap



	''' <summary>
	''' A single attachment to a <code>SOAPMessage</code> object. A <code>SOAPMessage</code>
	''' object may contain zero, one, or many <code>AttachmentPart</code> objects.
	''' Each <code>AttachmentPart</code> object consists of two parts,
	''' application-specific content and associated MIME headers. The
	''' MIME headers consists of name/value pairs that can be used to
	''' identify and describe the content.
	''' <p>
	''' An <code>AttachmentPart</code> object must conform to certain standards.
	''' <OL>
	''' <LI>It must conform to <a href="http://www.ietf.org/rfc/rfc2045.txt">
	'''     MIME [RFC2045] standards</a>
	''' <LI>It MUST contain content
	''' <LI>The header portion MUST include the following header:
	'''  <UL>
	'''   <LI><code>Content-Type</code><br>
	'''       This header identifies the type of data in the content of an
	'''       <code>AttachmentPart</code> object and MUST conform to [RFC2045].
	'''       The following is an example of a Content-Type header:
	'''       <PRE>
	'''       Content-Type:  application/xml
	'''       </PRE>
	'''       The following line of code, in which <code>ap</code> is an
	'''       <code>AttachmentPart</code> object, sets the header shown in
	'''       the previous example.
	'''       <PRE>
	'''       ap.setMimeHeader("Content-Type", "application/xml");
	'''       </PRE>
	''' <p>
	'''  </UL>
	''' </OL>
	''' <p>
	''' There are no restrictions on the content portion of an <code>
	''' AttachmentPart</code> object. The content may be anything from a
	''' simple plain text object to a complex XML document or image file.
	''' 
	''' <p>
	''' An <code>AttachmentPart</code> object is created with the method
	''' <code>SOAPMessage.createAttachmentPart</code>. After setting its MIME headers,
	'''  the <code>AttachmentPart</code> object is added to the message
	''' that created it with the method <code>SOAPMessage.addAttachmentPart</code>.
	''' 
	''' <p>
	''' The following code fragment, in which <code>m</code> is a
	''' <code>SOAPMessage</code> object and <code>contentStringl</code> is a
	''' <code>String</code>, creates an instance of <code>AttachmentPart</code>,
	''' sets the <code>AttachmentPart</code> object with some content and
	''' header information, and adds the <code>AttachmentPart</code> object to
	''' the <code>SOAPMessage</code> object.
	''' <PRE>
	'''     AttachmentPart ap1 = m.createAttachmentPart();
	'''     ap1.setContent(contentString1, "text/plain");
	'''     m.addAttachmentPart(ap1);
	''' </PRE>
	''' 
	''' 
	''' <p>
	''' The following code fragment creates and adds a second
	''' <code>AttachmentPart</code> instance to the same message. <code>jpegData</code>
	''' is a binary byte buffer representing the jpeg file.
	''' <PRE>
	'''     AttachmentPart ap2 = m.createAttachmentPart();
	'''     byte[] jpegData =  ...;
	'''     ap2.setContent(new ByteArrayInputStream(jpegData), "image/jpeg");
	'''     m.addAttachmentPart(ap2);
	''' </PRE>
	''' <p>
	''' The <code>getContent</code> method retrieves the contents and header from
	''' an <code>AttachmentPart</code> object. Depending on the
	''' <code>DataContentHandler</code> objects present, the returned
	''' <code>Object</code> can either be a typed Java object corresponding
	''' to the MIME type or an <code>InputStream</code> object that contains the
	''' content as bytes.
	''' <PRE>
	'''     String content1 = ap1.getContent();
	'''     java.io.InputStream content2 = ap2.getContent();
	''' </PRE>
	''' 
	''' The method <code>clearContent</code> removes all the content from an
	''' <code>AttachmentPart</code> object but does not affect its header information.
	''' <PRE>
	'''     ap1.clearContent();
	''' </PRE>
	''' </summary>

	Public MustInherit Class AttachmentPart
		''' <summary>
		''' Returns the number of bytes in this <code>AttachmentPart</code>
		''' object.
		''' </summary>
		''' <returns> the size of this <code>AttachmentPart</code> object in bytes
		'''         or -1 if the size cannot be determined </returns>
		''' <exception cref="SOAPException"> if the content of this attachment is
		'''            corrupted of if there was an exception while trying
		'''            to determine the size. </exception>
		Public MustOverride ReadOnly Property size As Integer

		''' <summary>
		''' Clears out the content of this <code>AttachmentPart</code> object.
		''' The MIME header portion is left untouched.
		''' </summary>
		Public MustOverride Sub clearContent()

		''' <summary>
		''' Gets the content of this <code>AttachmentPart</code> object as a Java
		''' object. The type of the returned Java object depends on (1) the
		''' <code>DataContentHandler</code> object that is used to interpret the bytes
		''' and (2) the <code>Content-Type</code> given in the header.
		''' <p>
		''' For the MIME content types "text/plain", "text/html" and "text/xml", the
		''' <code>DataContentHandler</code> object does the conversions to and
		''' from the Java types corresponding to the MIME types.
		''' For other MIME types,the <code>DataContentHandler</code> object
		''' can return an <code>InputStream</code> object that contains the content data
		''' as raw bytes.
		''' <p>
		''' A SAAJ-compliant implementation must, as a minimum, return a
		''' <code>java.lang.String</code> object corresponding to any content
		''' stream with a <code>Content-Type</code> value of
		''' <code>text/plain</code>, a
		''' <code>javax.xml.transform.stream.StreamSource</code> object corresponding to a
		''' content stream with a <code>Content-Type</code> value of
		''' <code>text/xml</code>, a <code>java.awt.Image</code> object
		''' corresponding to a content stream with a
		''' <code>Content-Type</code> value of <code>image/gif</code> or
		''' <code>image/jpeg</code>.  For those content types that an
		''' installed <code>DataContentHandler</code> object does not understand, the
		''' <code>DataContentHandler</code> object is required to return a
		''' <code>java.io.InputStream</code> object with the raw bytes.
		''' </summary>
		''' <returns> a Java object with the content of this <code>AttachmentPart</code>
		'''         object
		''' </returns>
		''' <exception cref="SOAPException"> if there is no content set into this
		'''            <code>AttachmentPart</code> object or if there was a data
		'''            transformation error </exception>
		Public MustOverride ReadOnly Property content As Object

		''' <summary>
		''' Gets the content of this <code>AttachmentPart</code> object as an
		''' InputStream as if a call had been made to <code>getContent</code> and no
		''' <code>DataContentHandler</code> had been registered for the
		''' <code>content-type</code> of this <code>AttachmentPart</code>.
		''' <p>
		''' Note that reading from the returned InputStream would result in consuming
		''' the data in the stream. It is the responsibility of the caller to reset
		''' the InputStream appropriately before calling a Subsequent API. If a copy
		''' of the raw attachment content is required then the <seealso cref="#getRawContentBytes"/> API
		''' should be used instead.
		''' </summary>
		''' <returns> an <code>InputStream</code> from which the raw data contained by
		'''      the <code>AttachmentPart</code> can be accessed.
		''' </returns>
		''' <exception cref="SOAPException"> if there is no content set into this
		'''      <code>AttachmentPart</code> object or if there was a data
		'''      transformation error.
		''' 
		''' @since SAAJ 1.3 </exception>
		''' <seealso cref= #getRawContentBytes </seealso>
		Public MustOverride ReadOnly Property rawContent As java.io.InputStream

		''' <summary>
		''' Gets the content of this <code>AttachmentPart</code> object as a
		''' byte[] array as if a call had been made to <code>getContent</code> and no
		''' <code>DataContentHandler</code> had been registered for the
		''' <code>content-type</code> of this <code>AttachmentPart</code>.
		''' </summary>
		''' <returns> a <code>byte[]</code> array containing the raw data of the
		'''      <code>AttachmentPart</code>.
		''' </returns>
		''' <exception cref="SOAPException"> if there is no content set into this
		'''      <code>AttachmentPart</code> object or if there was a data
		'''      transformation error.
		''' 
		''' @since SAAJ 1.3 </exception>
		Public MustOverride ReadOnly Property rawContentBytes As SByte()

		''' <summary>
		''' Returns an <code>InputStream</code> which can be used to obtain the
		''' content of <code>AttachmentPart</code>  as Base64 encoded
		''' character data, this method would base64 encode the raw bytes
		''' of the attachment and return.
		''' </summary>
		''' <returns> an <code>InputStream</code> from which the Base64 encoded
		'''       <code>AttachmentPart</code> can be read.
		''' </returns>
		''' <exception cref="SOAPException"> if there is no content set into this
		'''      <code>AttachmentPart</code> object or if there was a data
		'''      transformation error.
		''' 
		''' @since SAAJ 1.3 </exception>
		Public MustOverride ReadOnly Property base64Content As java.io.InputStream

		''' <summary>
		''' Sets the content of this attachment part to that of the given
		''' <code>Object</code> and sets the value of the <code>Content-Type</code>
		''' header to the given type. The type of the
		''' <code>Object</code> should correspond to the value given for the
		''' <code>Content-Type</code>. This depends on the particular
		''' set of <code>DataContentHandler</code> objects in use.
		''' 
		''' </summary>
		''' <param name="object"> the Java object that makes up the content for
		'''               this attachment part </param>
		''' <param name="contentType"> the MIME string that specifies the type of
		'''                  the content
		''' </param>
		''' <exception cref="IllegalArgumentException"> may be thrown if the contentType
		'''            does not match the type of the content object, or if there
		'''            was no <code>DataContentHandler</code> object for this
		'''            content object
		''' </exception>
		''' <seealso cref= #getContent </seealso>
		Public MustOverride Sub setContent(ByVal [object] As Object, ByVal contentType As String)

		''' <summary>
		''' Sets the content of this attachment part to that contained by the
		''' <code>InputStream</code> <code>content</code> and sets the value of the
		''' <code>Content-Type</code> header to the value contained in
		''' <code>contentType</code>.
		''' <P>
		'''  A subsequent call to getSize() may not be an exact measure
		'''  of the content size.
		''' </summary>
		''' <param name="content"> the raw data to add to the attachment part </param>
		''' <param name="contentType"> the value to set into the <code>Content-Type</code>
		''' header
		''' </param>
		''' <exception cref="SOAPException"> if an there is an error in setting the content </exception>
		''' <exception cref="NullPointerException"> if <code>content</code> is null
		''' @since SAAJ 1.3 </exception>
		Public MustOverride Sub setRawContent(ByVal content As java.io.InputStream, ByVal contentType As String)

		''' <summary>
		''' Sets the content of this attachment part to that contained by the
		''' <code>byte[]</code> array <code>content</code> and sets the value of the
		''' <code>Content-Type</code> header to the value contained in
		''' <code>contentType</code>.
		''' </summary>
		''' <param name="content"> the raw data to add to the attachment part </param>
		''' <param name="contentType"> the value to set into the <code>Content-Type</code>
		''' header </param>
		''' <param name="offset"> the offset in the byte array of the content </param>
		''' <param name="len"> the number of bytes that form the content
		''' </param>
		''' <exception cref="SOAPException"> if an there is an error in setting the content
		''' or content is null
		''' @since SAAJ 1.3 </exception>
		Public MustOverride Sub setRawContentBytes(ByVal content As SByte(), ByVal offset As Integer, ByVal len As Integer, ByVal contentType As String)


		''' <summary>
		''' Sets the content of this attachment part from the Base64 source
		''' <code>InputStream</code>  and sets the value of the
		''' <code>Content-Type</code> header to the value contained in
		''' <code>contentType</code>, This method would first decode the base64
		''' input and write the resulting raw bytes to the attachment.
		''' <P>
		'''  A subsequent call to getSize() may not be an exact measure
		'''  of the content size.
		''' </summary>
		''' <param name="content"> the base64 encoded data to add to the attachment part </param>
		''' <param name="contentType"> the value to set into the <code>Content-Type</code>
		''' header
		''' </param>
		''' <exception cref="SOAPException"> if an there is an error in setting the content </exception>
		''' <exception cref="NullPointerException"> if <code>content</code> is null
		''' 
		''' @since SAAJ 1.3 </exception>
		Public MustOverride Sub setBase64Content(ByVal content As java.io.InputStream, ByVal contentType As String)


		''' <summary>
		''' Gets the <code>DataHandler</code> object for this <code>AttachmentPart</code>
		''' object.
		''' </summary>
		''' <returns> the <code>DataHandler</code> object associated with this
		'''         <code>AttachmentPart</code> object
		''' </returns>
		''' <exception cref="SOAPException"> if there is no data in
		''' this <code>AttachmentPart</code> object </exception>
		Public MustOverride Property dataHandler As javax.activation.DataHandler



		''' <summary>
		''' Gets the value of the MIME header whose name is "Content-ID".
		''' </summary>
		''' <returns> a <code>String</code> giving the value of the
		'''          "Content-ID" header or <code>null</code> if there
		'''          is none </returns>
		''' <seealso cref= #setContentId </seealso>
		Public Overridable Property contentId As String
			Get
				Dim values As String() = getMimeHeader("Content-ID")
				If values IsNot Nothing AndAlso values.Length > 0 Then Return values(0)
				Return Nothing
			End Get
			Set(ByVal contentId As String)
				mimeHeaderder("Content-ID", contentId)
			End Set
		End Property

		''' <summary>
		''' Gets the value of the MIME header whose name is "Content-Location".
		''' </summary>
		''' <returns> a <code>String</code> giving the value of the
		'''          "Content-Location" header or <code>null</code> if there
		'''          is none </returns>
		Public Overridable Property contentLocation As String
			Get
				Dim values As String() = getMimeHeader("Content-Location")
				If values IsNot Nothing AndAlso values.Length > 0 Then Return values(0)
				Return Nothing
			End Get
			Set(ByVal contentLocation As String)
				mimeHeaderder("Content-Location", contentLocation)
			End Set
		End Property

		''' <summary>
		''' Gets the value of the MIME header whose name is "Content-Type".
		''' </summary>
		''' <returns> a <code>String</code> giving the value of the
		'''          "Content-Type" header or <code>null</code> if there
		'''          is none </returns>
		Public Overridable Property contentType As String
			Get
				Dim values As String() = getMimeHeader("Content-Type")
				If values IsNot Nothing AndAlso values.Length > 0 Then Return values(0)
				Return Nothing
			End Get
			Set(ByVal contentType As String)
				mimeHeaderder("Content-Type", contentType)
			End Set
		End Property





		''' <summary>
		''' Removes all MIME headers that match the given name.
		''' </summary>
		''' <param name="header"> the string name of the MIME header/s to
		'''               be removed </param>
		Public MustOverride Sub removeMimeHeader(ByVal header As String)

		''' <summary>
		''' Removes all the MIME header entries.
		''' </summary>
		Public MustOverride Sub removeAllMimeHeaders()


		''' <summary>
		''' Gets all the values of the header identified by the given
		''' <code>String</code>.
		''' </summary>
		''' <param name="name"> the name of the header; example: "Content-Type" </param>
		''' <returns> a <code>String</code> array giving the value for the
		'''         specified header </returns>
		''' <seealso cref= #setMimeHeader </seealso>
		Public MustOverride Function getMimeHeader(ByVal name As String) As String()


		''' <summary>
		''' Changes the first header entry that matches the given name
		''' to the given value, adding a new header if no existing header
		''' matches. This method also removes all matching headers but the first. <p>
		''' 
		''' Note that RFC822 headers can only contain US-ASCII characters.
		''' </summary>
		''' <param name="name">    a <code>String</code> giving the name of the header
		'''                  for which to search </param>
		''' <param name="value">   a <code>String</code> giving the value to be set for
		'''                  the header whose name matches the given name
		''' </param>
		''' <exception cref="IllegalArgumentException"> if there was a problem with
		'''            the specified mime header name or value </exception>
		Public MustOverride Sub setMimeHeader(ByVal name As String, ByVal value As String)


		''' <summary>
		''' Adds a MIME header with the specified name and value to this
		''' <code>AttachmentPart</code> object.
		''' <p>
		''' Note that RFC822 headers can contain only US-ASCII characters.
		''' </summary>
		''' <param name="name">    a <code>String</code> giving the name of the header
		'''                  to be added </param>
		''' <param name="value">   a <code>String</code> giving the value of the header
		'''                  to be added
		''' </param>
		''' <exception cref="IllegalArgumentException"> if there was a problem with
		'''            the specified mime header name or value </exception>
		Public MustOverride Sub addMimeHeader(ByVal name As String, ByVal value As String)

		''' <summary>
		''' Retrieves all the headers for this <code>AttachmentPart</code> object
		''' as an iterator over the <code>MimeHeader</code> objects.
		''' </summary>
		''' <returns>  an <code>Iterator</code> object with all of the Mime
		'''          headers for this <code>AttachmentPart</code> object </returns>
		Public MustOverride ReadOnly Property allMimeHeaders As IEnumerator

		''' <summary>
		''' Retrieves all <code>MimeHeader</code> objects that match a name in
		''' the given array.
		''' </summary>
		''' <param name="names"> a <code>String</code> array with the name(s) of the
		'''        MIME headers to be returned </param>
		''' <returns>  all of the MIME headers that match one of the names in the
		'''           given array as an <code>Iterator</code> object </returns>
		Public MustOverride Function getMatchingMimeHeaders(ByVal names As String()) As IEnumerator

		''' <summary>
		''' Retrieves all <code>MimeHeader</code> objects whose name does
		''' not match a name in the given array.
		''' </summary>
		''' <param name="names"> a <code>String</code> array with the name(s) of the
		'''        MIME headers not to be returned </param>
		''' <returns>  all of the MIME headers in this <code>AttachmentPart</code> object
		'''          except those that match one of the names in the
		'''           given array.  The nonmatching MIME headers are returned as an
		'''           <code>Iterator</code> object. </returns>
		Public MustOverride Function getNonMatchingMimeHeaders(ByVal names As String()) As IEnumerator
	End Class

End Namespace
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
	''' The container for the SOAP-specific portion of a <code>SOAPMessage</code>
	''' object. All messages are required to have a SOAP part, so when a
	''' <code>SOAPMessage</code> object is created, it will automatically
	''' have a <code>SOAPPart</code> object.
	''' <P>
	''' A <code>SOAPPart</code> object is a MIME part and has the MIME headers
	''' Content-Id, Content-Location, and Content-Type.  Because the value of
	''' Content-Type must be "text/xml", a <code>SOAPPart</code> object automatically
	''' has a MIME header of Content-Type with its value set to "text/xml".
	''' The value must be "text/xml" because content in the SOAP part of a
	''' message must be in XML format.  Content that is not of type "text/xml"
	''' must be in an <code>AttachmentPart</code> object rather than in the
	''' <code>SOAPPart</code> object.
	''' <P>
	''' When a message is sent, its SOAP part must have the MIME header Content-Type
	''' set to "text/xml". Or, from the other perspective, the SOAP part of any
	''' message that is received must have the MIME header Content-Type with a
	''' value of "text/xml".
	''' <P>
	''' A client can access the <code>SOAPPart</code> object of a
	''' <code>SOAPMessage</code> object by
	''' calling the method <code>SOAPMessage.getSOAPPart</code>. The
	''' following  line of code, in which <code>message</code> is a
	''' <code>SOAPMessage</code> object, retrieves the SOAP part of a message.
	''' <PRE>
	'''   SOAPPart soapPart = message.getSOAPPart();
	''' </PRE>
	''' <P>
	''' A <code>SOAPPart</code> object contains a <code>SOAPEnvelope</code> object,
	''' which in turn contains a <code>SOAPBody</code> object and a
	''' <code>SOAPHeader</code> object.
	''' The <code>SOAPPart</code> method <code>getEnvelope</code> can be used
	''' to retrieve the <code>SOAPEnvelope</code> object.
	''' <P>
	''' </summary>
	Public MustInherit Class SOAPPart
		Implements org.w3c.dom.Document, Node

			Public MustOverride Sub recycleNode() Implements Node.recycleNode
			Public MustOverride Sub detachNode() Implements Node.detachNode
			Public MustOverride Property parentElement As SOAPElement Implements Node.getParentElement
			Public MustOverride Property value Implements Node.setValue As String

		''' <summary>
		''' Gets the <code>SOAPEnvelope</code> object associated with this
		''' <code>SOAPPart</code> object. Once the SOAP envelope is obtained, it
		''' can be used to get its contents.
		''' </summary>
		''' <returns> the <code>SOAPEnvelope</code> object for this
		'''           <code>SOAPPart</code> object </returns>
		''' <exception cref="SOAPException"> if there is a SOAP error </exception>
		Public MustOverride ReadOnly Property envelope As SOAPEnvelope

		''' <summary>
		''' Retrieves the value of the MIME header whose name is "Content-Id".
		''' </summary>
		''' <returns> a <code>String</code> giving the value of the MIME header
		'''         named "Content-Id" </returns>
		''' <seealso cref= #setContentId </seealso>
		Public Overridable Property contentId As String
			Get
				Dim values As String() = getMimeHeader("Content-Id")
				If values IsNot Nothing AndAlso values.Length > 0 Then Return values(0)
				Return Nothing
			End Get
			Set(ByVal contentId As String)
				mimeHeaderder("Content-Id", contentId)
			End Set
		End Property

		''' <summary>
		''' Retrieves the value of the MIME header whose name is "Content-Location".
		''' </summary>
		''' <returns> a <code>String</code> giving the value of the MIME header whose
		'''          name is "Content-Location" </returns>
		''' <seealso cref= #setContentLocation </seealso>
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
		''' Removes all MIME headers that match the given name.
		''' </summary>
		''' <param name="header"> a <code>String</code> giving the name of the MIME header(s) to
		'''               be removed </param>
		Public MustOverride Sub removeMimeHeader(ByVal header As String)

		''' <summary>
		''' Removes all the <code>MimeHeader</code> objects for this
		''' <code>SOAPEnvelope</code> object.
		''' </summary>
		Public MustOverride Sub removeAllMimeHeaders()

		''' <summary>
		''' Gets all the values of the <code>MimeHeader</code> object
		''' in this <code>SOAPPart</code> object that
		''' is identified by the given <code>String</code>.
		''' </summary>
		''' <param name="name"> the name of the header; example: "Content-Type" </param>
		''' <returns> a <code>String</code> array giving all the values for the
		'''         specified header </returns>
		''' <seealso cref= #setMimeHeader </seealso>
		Public MustOverride Function getMimeHeader(ByVal name As String) As String()

		''' <summary>
		''' Changes the first header entry that matches the given header name
		''' so that its value is the given value, adding a new header with the
		''' given name and value if no
		''' existing header is a match. If there is a match, this method clears
		''' all existing values for the first header that matches and sets the
		''' given value instead. If more than one header has
		''' the given name, this method removes all of the matching headers after
		''' the first one.
		''' <P>
		''' Note that RFC822 headers can contain only US-ASCII characters.
		''' </summary>
		''' <param name="name">    a <code>String</code> giving the header name
		'''                  for which to search </param>
		''' <param name="value">   a <code>String</code> giving the value to be set.
		'''                  This value will be substituted for the current value(s)
		'''                  of the first header that is a match if there is one.
		'''                  If there is no match, this value will be the value for
		'''                  a new <code>MimeHeader</code> object.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if there was a problem with
		'''            the specified mime header name or value </exception>
		''' <seealso cref= #getMimeHeader </seealso>
		Public MustOverride Sub setMimeHeader(ByVal name As String, ByVal value As String)

		''' <summary>
		''' Creates a <code>MimeHeader</code> object with the specified
		''' name and value and adds it to this <code>SOAPPart</code> object.
		''' If a <code>MimeHeader</code> with the specified name already
		''' exists, this method adds the specified value to the already
		''' existing value(s).
		''' <P>
		''' Note that RFC822 headers can contain only US-ASCII characters.
		''' </summary>
		''' <param name="name">    a <code>String</code> giving the header name </param>
		''' <param name="value">   a <code>String</code> giving the value to be set
		'''                  or added </param>
		''' <exception cref="IllegalArgumentException"> if there was a problem with
		'''            the specified mime header name or value </exception>
		Public MustOverride Sub addMimeHeader(ByVal name As String, ByVal value As String)

		''' <summary>
		''' Retrieves all the headers for this <code>SOAPPart</code> object
		''' as an iterator over the <code>MimeHeader</code> objects.
		''' </summary>
		''' <returns>  an <code>Iterator</code> object with all of the Mime
		'''          headers for this <code>SOAPPart</code> object </returns>
		Public MustOverride ReadOnly Property allMimeHeaders As IEnumerator

		''' <summary>
		''' Retrieves all <code>MimeHeader</code> objects that match a name in
		''' the given array.
		''' </summary>
		''' <param name="names"> a <code>String</code> array with the name(s) of the
		'''        MIME headers to be returned </param>
		''' <returns>  all of the MIME headers that match one of the names in the
		'''           given array, returned as an <code>Iterator</code> object </returns>
		Public MustOverride Function getMatchingMimeHeaders(ByVal names As String()) As IEnumerator

		''' <summary>
		''' Retrieves all <code>MimeHeader</code> objects whose name does
		''' not match a name in the given array.
		''' </summary>
		''' <param name="names"> a <code>String</code> array with the name(s) of the
		'''        MIME headers not to be returned </param>
		''' <returns>  all of the MIME headers in this <code>SOAPPart</code> object
		'''          except those that match one of the names in the
		'''           given array.  The nonmatching MIME headers are returned as an
		'''           <code>Iterator</code> object. </returns>
		Public MustOverride Function getNonMatchingMimeHeaders(ByVal names As String()) As IEnumerator

		''' <summary>
		''' Sets the content of the <code>SOAPEnvelope</code> object with the data
		''' from the given <code>Source</code> object. This <code>Source</code>
		''' must contain a valid SOAP document.
		''' </summary>
		''' <param name="source"> the <code>javax.xml.transform.Source</code> object with the
		'''        data to be set
		''' </param>
		''' <exception cref="SOAPException"> if there is a problem in setting the source </exception>
		''' <seealso cref= #getContent </seealso>
		Public MustOverride Property content As javax.xml.transform.Source

	End Class

End Namespace
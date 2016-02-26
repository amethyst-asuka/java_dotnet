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
	''' A representation of the SOAP header
	''' element. A SOAP header element consists of XML data that affects
	''' the way the application-specific content is processed by the message
	''' provider. For example, transaction semantics, authentication information,
	''' and so on, can be specified as the content of a <code>SOAPHeader</code>
	''' object.
	''' <P>
	''' A <code>SOAPEnvelope</code> object contains an empty
	''' <code>SOAPHeader</code> object by default. If the <code>SOAPHeader</code>
	''' object, which is optional, is not needed, it can be retrieved and deleted
	''' with the following line of code. The variable <i>se</i> is a
	''' <code>SOAPEnvelope</code> object.
	''' <PRE>
	'''      se.getHeader().detachNode();
	''' </PRE>
	''' 
	''' A <code>SOAPHeader</code> object is created with the <code>SOAPEnvelope</code>
	''' method <code>addHeader</code>. This method, which creates a new header and adds it
	''' to the envelope, may be called only after the existing header has been removed.
	''' 
	''' <PRE>
	'''      se.getHeader().detachNode();
	'''      SOAPHeader sh = se.addHeader();
	''' </PRE>
	''' <P>
	''' A <code>SOAPHeader</code> object can have only <code>SOAPHeaderElement</code>
	''' objects as its immediate children. The method <code>addHeaderElement</code>
	''' creates a new <code>HeaderElement</code> object and adds it to the
	''' <code>SOAPHeader</code> object. In the following line of code, the
	''' argument to the method <code>addHeaderElement</code> is a <code>Name</code>
	''' object that is the name for the new <code>HeaderElement</code> object.
	''' <PRE>
	'''      SOAPHeaderElement shElement = sh.addHeaderElement(name);
	''' </PRE>
	''' </summary>
	''' <seealso cref= SOAPHeaderElement </seealso>
	Public Interface SOAPHeader
		Inherits SOAPElement

		''' <summary>
		''' Creates a new <code>SOAPHeaderElement</code> object initialized with the
		''' specified name and adds it to this <code>SOAPHeader</code> object.
		''' </summary>
		''' <param name="name"> a <code>Name</code> object with the name of the new
		'''        <code>SOAPHeaderElement</code> object </param>
		''' <returns> the new <code>SOAPHeaderElement</code> object that was
		'''          inserted into this <code>SOAPHeader</code> object </returns>
		''' <exception cref="SOAPException"> if a SOAP error occurs </exception>
		''' <seealso cref= SOAPHeader#addHeaderElement(javax.xml.namespace.QName) </seealso>
		Function addHeaderElement(ByVal name As Name) As SOAPHeaderElement

		''' <summary>
		''' Creates a new <code>SOAPHeaderElement</code> object initialized with the
		''' specified qname and adds it to this <code>SOAPHeader</code> object.
		''' </summary>
		''' <param name="qname"> a <code>QName</code> object with the qname of the new
		'''        <code>SOAPHeaderElement</code> object </param>
		''' <returns> the new <code>SOAPHeaderElement</code> object that was
		'''          inserted into this <code>SOAPHeader</code> object </returns>
		''' <exception cref="SOAPException"> if a SOAP error occurs </exception>
		''' <seealso cref= SOAPHeader#addHeaderElement(Name)
		''' @since SAAJ 1.3 </seealso>
		Function addHeaderElement(ByVal qname As javax.xml.namespace.QName) As SOAPHeaderElement

		''' <summary>
		''' Returns an <code>Iterator</code> over all the <code>SOAPHeaderElement</code> objects
		''' in this <code>SOAPHeader</code> object
		''' that have the specified <i>actor</i> and that have a MustUnderstand attribute
		''' whose value is equivalent to <code>true</code>.
		''' <p>
		''' In SOAP 1.2 the <i>env:actor</i> attribute is replaced by the <i>env:role</i>
		''' attribute, but with essentially the same semantics.
		''' </summary>
		''' <param name="actor"> a <code>String</code> giving the URI of the <code>actor</code> / <code>role</code>
		'''        for which to search </param>
		''' <returns> an <code>Iterator</code> object over all the
		'''         <code>SOAPHeaderElement</code> objects that contain the specified
		'''          <code>actor</code> / <code>role</code> and are marked as MustUnderstand </returns>
		''' <seealso cref= #examineHeaderElements </seealso>
		''' <seealso cref= #extractHeaderElements </seealso>
		''' <seealso cref= SOAPConstants#URI_SOAP_ACTOR_NEXT
		''' 
		''' @since SAAJ 1.2 </seealso>
		Function examineMustUnderstandHeaderElements(ByVal actor As String) As IEnumerator

		''' <summary>
		''' Returns an <code>Iterator</code> over all the <code>SOAPHeaderElement</code> objects
		''' in this <code>SOAPHeader</code> object
		''' that have the specified <i>actor</i>.
		''' 
		''' An <i>actor</i> is a global attribute that indicates the intermediate
		''' parties that should process a message before it reaches its ultimate
		''' receiver. An actor receives the message and processes it before sending
		''' it on to the next actor. The default actor is the ultimate intended
		''' recipient for the message, so if no actor attribute is included in a
		''' <code>SOAPHeader</code> object, it is sent to the ultimate receiver
		''' along with the message body.
		''' <p>
		''' In SOAP 1.2 the <i>env:actor</i> attribute is replaced by the <i>env:role</i>
		''' attribute, but with essentially the same semantics.
		''' </summary>
		''' <param name="actor"> a <code>String</code> giving the URI of the <code>actor</code> / <code>role</code>
		'''        for which to search </param>
		''' <returns> an <code>Iterator</code> object over all the
		'''         <code>SOAPHeaderElement</code> objects that contain the specified
		'''          <code>actor</code> / <code>role</code> </returns>
		''' <seealso cref= #extractHeaderElements </seealso>
		''' <seealso cref= SOAPConstants#URI_SOAP_ACTOR_NEXT </seealso>
		Function examineHeaderElements(ByVal actor As String) As IEnumerator

		''' <summary>
		''' Returns an <code>Iterator</code> over all the <code>SOAPHeaderElement</code> objects
		''' in this <code>SOAPHeader</code> object
		''' that have the specified <i>actor</i> and detaches them
		''' from this <code>SOAPHeader</code> object.
		''' <P>
		''' This method allows an actor to process the parts of the
		''' <code>SOAPHeader</code> object that apply to it and to remove
		''' them before passing the message on to the next actor.
		''' <p>
		''' In SOAP 1.2 the <i>env:actor</i> attribute is replaced by the <i>env:role</i>
		''' attribute, but with essentially the same semantics.
		''' </summary>
		''' <param name="actor"> a <code>String</code> giving the URI of the <code>actor</code> / <code>role</code>
		'''        for which to search </param>
		''' <returns> an <code>Iterator</code> object over all the
		'''         <code>SOAPHeaderElement</code> objects that contain the specified
		'''          <code>actor</code> / <code>role</code>
		''' </returns>
		''' <seealso cref= #examineHeaderElements </seealso>
		''' <seealso cref= SOAPConstants#URI_SOAP_ACTOR_NEXT </seealso>
		Function extractHeaderElements(ByVal actor As String) As IEnumerator

		''' <summary>
		''' Creates a new NotUnderstood <code>SOAPHeaderElement</code> object initialized
		''' with the specified name and adds it to this <code>SOAPHeader</code> object.
		''' This operation is supported only by SOAP 1.2.
		''' </summary>
		''' <param name="name"> a <code>QName</code> object with the name of the
		'''        <code>SOAPHeaderElement</code> object that was not understood. </param>
		''' <returns> the new <code>SOAPHeaderElement</code> object that was
		'''          inserted into this <code>SOAPHeader</code> object </returns>
		''' <exception cref="SOAPException"> if a SOAP error occurs. </exception>
		''' <exception cref="UnsupportedOperationException"> if this is a SOAP 1.1 Header.
		''' @since SAAJ 1.3 </exception>
		Function addNotUnderstoodHeaderElement(ByVal name As javax.xml.namespace.QName) As SOAPHeaderElement

		''' <summary>
		''' Creates a new Upgrade <code>SOAPHeaderElement</code> object initialized
		''' with the specified List of supported SOAP URIs and adds it to this
		''' <code>SOAPHeader</code> object.
		''' This operation is supported on both SOAP 1.1 and SOAP 1.2 header.
		''' </summary>
		''' <param name="supportedSOAPURIs"> an <code>Iterator</code> object with the URIs of SOAP
		'''          versions supported. </param>
		''' <returns> the new <code>SOAPHeaderElement</code> object that was
		'''          inserted into this <code>SOAPHeader</code> object </returns>
		''' <exception cref="SOAPException"> if a SOAP error occurs.
		''' @since SAAJ 1.3 </exception>
		Function addUpgradeHeaderElement(ByVal supportedSOAPURIs As IEnumerator) As SOAPHeaderElement

		''' <summary>
		''' Creates a new Upgrade <code>SOAPHeaderElement</code> object initialized
		''' with the specified array of supported SOAP URIs and adds it to this
		''' <code>SOAPHeader</code> object.
		''' This operation is supported on both SOAP 1.1 and SOAP 1.2 header.
		''' </summary>
		''' <param name="supportedSoapUris"> an array of the URIs of SOAP versions supported. </param>
		''' <returns> the new <code>SOAPHeaderElement</code> object that was
		'''          inserted into this <code>SOAPHeader</code> object </returns>
		''' <exception cref="SOAPException"> if a SOAP error occurs.
		''' @since SAAJ 1.3 </exception>
		Function addUpgradeHeaderElement(ByVal supportedSoapUris As String()) As SOAPHeaderElement

		''' <summary>
		''' Creates a new Upgrade <code>SOAPHeaderElement</code> object initialized
		''' with the specified supported SOAP URI and adds it to this
		''' <code>SOAPHeader</code> object.
		''' This operation is supported on both SOAP 1.1 and SOAP 1.2 header.
		''' </summary>
		''' <param name="supportedSoapUri"> the URI of SOAP the version that is supported. </param>
		''' <returns> the new <code>SOAPHeaderElement</code> object that was
		'''          inserted into this <code>SOAPHeader</code> object </returns>
		''' <exception cref="SOAPException"> if a SOAP error occurs.
		''' @since SAAJ 1.3 </exception>
		Function addUpgradeHeaderElement(ByVal supportedSoapUri As String) As SOAPHeaderElement

		''' <summary>
		''' Returns an <code>Iterator</code> over all the <code>SOAPHeaderElement</code> objects
		''' in this <code>SOAPHeader</code> object.
		''' </summary>
		''' <returns> an <code>Iterator</code> object over all the
		'''          <code>SOAPHeaderElement</code> objects contained by this
		'''          <code>SOAPHeader</code> </returns>
		''' <seealso cref= #extractAllHeaderElements
		''' 
		''' @since SAAJ 1.2 </seealso>
		Function examineAllHeaderElements() As IEnumerator

		''' <summary>
		''' Returns an <code>Iterator</code> over all the <code>SOAPHeaderElement</code> objects
		''' in this <code>SOAPHeader</code> object and detaches them
		''' from this <code>SOAPHeader</code> object.
		''' </summary>
		''' <returns> an <code>Iterator</code> object over all the
		'''          <code>SOAPHeaderElement</code> objects contained by this
		'''          <code>SOAPHeader</code>
		''' </returns>
		''' <seealso cref= #examineAllHeaderElements
		''' 
		''' @since SAAJ 1.2 </seealso>
		Function extractAllHeaderElements() As IEnumerator

	End Interface

End Namespace
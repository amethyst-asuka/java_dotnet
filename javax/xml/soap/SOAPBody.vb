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
	''' An object that represents the contents of the SOAP body
	''' element in a SOAP message. A SOAP body element consists of XML data
	''' that affects the way the application-specific content is processed.
	''' <P>
	''' A <code>SOAPBody</code> object contains <code>SOAPBodyElement</code>
	''' objects, which have the content for the SOAP body.
	''' A <code>SOAPFault</code> object, which carries status and/or
	''' error information, is an example of a <code>SOAPBodyElement</code> object.
	''' </summary>
	''' <seealso cref= SOAPFault </seealso>
	Public Interface SOAPBody
		Inherits SOAPElement

		''' <summary>
		''' Creates a new <code>SOAPFault</code> object and adds it to
		''' this <code>SOAPBody</code> object. The new <code>SOAPFault</code> will
		''' have default values set for the mandatory child elements. The type of
		''' the <code>SOAPFault</code> will be a SOAP 1.1 or a SOAP 1.2 <code>SOAPFault</code>
		''' depending on the <code>protocol</code> specified while creating the
		''' <code>MessageFactory</code> instance.
		''' <p>
		''' A <code>SOAPBody</code> may contain at most one <code>SOAPFault</code>
		''' child element.
		''' </summary>
		''' <returns> the new <code>SOAPFault</code> object </returns>
		''' <exception cref="SOAPException"> if there is a SOAP error </exception>
		Function addFault() As SOAPFault


		''' <summary>
		''' Creates a new <code>SOAPFault</code> object and adds it to
		''' this <code>SOAPBody</code> object. The type of the
		''' <code>SOAPFault</code> will be a SOAP 1.1  or a SOAP 1.2
		''' <code>SOAPFault</code> depending on the <code>protocol</code>
		''' specified while creating the <code>MessageFactory</code> instance.
		''' <p>
		''' For SOAP 1.2 the <code>faultCode</code> parameter is the value of the
		''' <i>Fault/Code/Value</i> element  and the <code>faultString</code> parameter
		''' is the value of the <i>Fault/Reason/Text</i> element. For SOAP 1.1
		''' the <code>faultCode</code> parameter is the value of the <code>faultcode</code>
		''' element and the <code>faultString</code> parameter is the value of the <code>faultstring</code>
		''' element.
		''' <p>
		''' A <code>SOAPBody</code> may contain at most one <code>SOAPFault</code>
		''' child element.
		''' </summary>
		''' <param name="faultCode"> a <code>Name</code> object giving the fault
		'''         code to be set; must be one of the fault codes defined in the Version
		'''         of SOAP specification in use </param>
		''' <param name="faultString"> a <code>String</code> giving an explanation of
		'''         the fault </param>
		''' <param name="locale"> a <seealso cref="java.util.Locale"/> object indicating
		'''         the native language of the <code>faultString</code> </param>
		''' <returns> the new <code>SOAPFault</code> object </returns>
		''' <exception cref="SOAPException"> if there is a SOAP error </exception>
		''' <seealso cref= SOAPFault#setFaultCode </seealso>
		''' <seealso cref= SOAPFault#setFaultString
		''' @since SAAJ 1.2 </seealso>
		Function addFault(ByVal faultCode As Name, ByVal faultString As String, ByVal locale As java.util.Locale) As SOAPFault

		''' <summary>
		''' Creates a new <code>SOAPFault</code> object and adds it to this
		''' <code>SOAPBody</code> object. The type of the <code>SOAPFault</code>
		''' will be a SOAP 1.1 or a SOAP 1.2 <code>SOAPFault</code> depending on
		''' the <code>protocol</code> specified while creating the <code>MessageFactory</code>
		''' instance.
		''' <p>
		''' For SOAP 1.2 the <code>faultCode</code> parameter is the value of the
		''' <i>Fault/Code/Value</i> element  and the <code>faultString</code> parameter
		''' is the value of the <i>Fault/Reason/Text</i> element. For SOAP 1.1
		''' the <code>faultCode</code> parameter is the value of the <code>faultcode</code>
		''' element and the <code>faultString</code> parameter is the value of the <code>faultstring</code>
		''' element.
		''' <p>
		''' A <code>SOAPBody</code> may contain at most one <code>SOAPFault</code>
		''' child element.
		''' </summary>
		''' <param name="faultCode">
		'''            a <code>QName</code> object giving the fault code to be
		'''            set; must be one of the fault codes defined in the version
		'''            of SOAP specification in use. </param>
		''' <param name="faultString">
		'''            a <code>String</code> giving an explanation of the fault </param>
		''' <param name="locale">
		'''            a <seealso cref="java.util.Locale Locale"/> object indicating the
		'''            native language of the <code>faultString</code> </param>
		''' <returns> the new <code>SOAPFault</code> object </returns>
		''' <exception cref="SOAPException">
		'''                if there is a SOAP error </exception>
		''' <seealso cref= SOAPFault#setFaultCode </seealso>
		''' <seealso cref= SOAPFault#setFaultString </seealso>
		''' <seealso cref= SOAPBody#addFault(Name faultCode, String faultString, Locale locale)
		''' 
		''' @since SAAJ 1.3 </seealso>
		Function addFault(ByVal faultCode As javax.xml.namespace.QName, ByVal faultString As String, ByVal locale As java.util.Locale) As SOAPFault

		''' <summary>
		''' Creates a new  <code>SOAPFault</code> object and adds it to this
		''' <code>SOAPBody</code> object. The type of the <code>SOAPFault</code>
		''' will be a SOAP 1.1 or a SOAP 1.2 <code>SOAPFault</code> depending on
		''' the <code>protocol</code> specified while creating the <code>MessageFactory</code>
		''' instance.
		''' <p>
		''' For SOAP 1.2 the <code>faultCode</code> parameter is the value of the
		''' <i>Fault/Code/Value</i> element  and the <code>faultString</code> parameter
		''' is the value of the <i>Fault/Reason/Text</i> element. For SOAP 1.1
		''' the <code>faultCode</code> parameter is the value of the <i>faultcode</i>
		''' element and the <code>faultString</code> parameter is the value of the <i>faultstring</i>
		''' element.
		''' <p>
		''' In case of a SOAP 1.2 fault, the default value for the mandatory <code>xml:lang</code>
		''' attribute on the <i>Fault/Reason/Text</i> element will be set to
		''' <code>java.util.Locale.getDefault()</code>
		''' <p>
		''' A <code>SOAPBody</code> may contain at most one <code>SOAPFault</code>
		''' child element.
		''' </summary>
		''' <param name="faultCode">
		'''            a <code>Name</code> object giving the fault code to be set;
		'''            must be one of the fault codes defined in the version of SOAP
		'''            specification in use </param>
		''' <param name="faultString">
		'''            a <code>String</code> giving an explanation of the fault </param>
		''' <returns> the new <code>SOAPFault</code> object </returns>
		''' <exception cref="SOAPException">
		'''                if there is a SOAP error </exception>
		''' <seealso cref= SOAPFault#setFaultCode </seealso>
		''' <seealso cref= SOAPFault#setFaultString
		''' @since SAAJ 1.2 </seealso>
		Function addFault(ByVal faultCode As Name, ByVal faultString As String) As SOAPFault

		''' <summary>
		''' Creates a new <code>SOAPFault</code> object and adds it to this <code>SOAPBody</code>
		''' object. The type of the <code>SOAPFault</code>
		''' will be a SOAP 1.1 or a SOAP 1.2 <code>SOAPFault</code> depending on
		''' the <code>protocol</code> specified while creating the <code>MessageFactory</code>
		''' instance.
		''' <p>
		''' For SOAP 1.2 the <code>faultCode</code> parameter is the value of the
		''' <i>Fault/Code/Value</i> element  and the <code>faultString</code> parameter
		''' is the value of the <i>Fault/Reason/Text</i> element. For SOAP 1.1
		''' the <code>faultCode</code> parameter is the value of the <i>faultcode</i>
		''' element and the <code>faultString</code> parameter is the value of the <i>faultstring</i>
		''' element.
		''' <p>
		''' In case of a SOAP 1.2 fault, the default value for the mandatory <code>xml:lang</code>
		''' attribute on the <i>Fault/Reason/Text</i> element will be set to
		''' <code>java.util.Locale.getDefault()</code>
		''' <p>
		''' A <code>SOAPBody</code> may contain at most one <code>SOAPFault</code>
		''' child element
		''' </summary>
		''' <param name="faultCode">
		'''            a <code>QName</code> object giving the fault code to be
		'''            set; must be one of the fault codes defined in the version
		'''            of  SOAP specification in use </param>
		''' <param name="faultString">
		'''            a <code>String</code> giving an explanation of the fault </param>
		''' <returns> the new <code>SOAPFault</code> object </returns>
		''' <exception cref="SOAPException">
		'''                if there is a SOAP error </exception>
		''' <seealso cref= SOAPFault#setFaultCode </seealso>
		''' <seealso cref= SOAPFault#setFaultString </seealso>
		''' <seealso cref= SOAPBody#addFault(Name faultCode, String faultString)
		''' @since SAAJ 1.3 </seealso>
		Function addFault(ByVal faultCode As javax.xml.namespace.QName, ByVal faultString As String) As SOAPFault

		''' <summary>
		''' Indicates whether a <code>SOAPFault</code> object exists in this
		''' <code>SOAPBody</code> object.
		''' </summary>
		''' <returns> <code>true</code> if a <code>SOAPFault</code> object exists
		'''         in this <code>SOAPBody</code> object; <code>false</code>
		'''         otherwise </returns>
		Function hasFault() As Boolean

		''' <summary>
		''' Returns the <code>SOAPFault</code> object in this <code>SOAPBody</code>
		''' object.
		''' </summary>
		''' <returns> the <code>SOAPFault</code> object in this <code>SOAPBody</code>
		'''         object if present, null otherwise. </returns>
		ReadOnly Property fault As SOAPFault

		''' <summary>
		''' Creates a new <code>SOAPBodyElement</code> object with the specified
		''' name and adds it to this <code>SOAPBody</code> object.
		''' </summary>
		''' <param name="name">
		'''            a <code>Name</code> object with the name for the new <code>SOAPBodyElement</code>
		'''            object </param>
		''' <returns> the new <code>SOAPBodyElement</code> object </returns>
		''' <exception cref="SOAPException">
		'''                if a SOAP error occurs </exception>
		''' <seealso cref= SOAPBody#addBodyElement(javax.xml.namespace.QName) </seealso>
		Function addBodyElement(ByVal name As Name) As SOAPBodyElement


		''' <summary>
		''' Creates a new <code>SOAPBodyElement</code> object with the specified
		''' QName and adds it to this <code>SOAPBody</code> object.
		''' </summary>
		''' <param name="qname">
		'''            a <code>QName</code> object with the qname for the new
		'''            <code>SOAPBodyElement</code> object </param>
		''' <returns> the new <code>SOAPBodyElement</code> object </returns>
		''' <exception cref="SOAPException">
		'''                if a SOAP error occurs </exception>
		''' <seealso cref= SOAPBody#addBodyElement(Name)
		''' @since SAAJ 1.3 </seealso>
		Function addBodyElement(ByVal qname As javax.xml.namespace.QName) As SOAPBodyElement

		''' <summary>
		''' Adds the root node of the DOM <code><seealso cref="org.w3c.dom.Document"/></code>
		''' to this <code>SOAPBody</code> object.
		''' <p>
		''' Calling this method invalidates the <code>document</code> parameter.
		''' The client application should discard all references to this <code>Document</code>
		''' and its contents upon calling <code>addDocument</code>. The behavior
		''' of an application that continues to use such references is undefined.
		''' </summary>
		''' <param name="document">
		'''            the <code>Document</code> object whose root node will be
		'''            added to this <code>SOAPBody</code>. </param>
		''' <returns> the <code>SOAPBodyElement</code> that represents the root node
		'''         that was added. </returns>
		''' <exception cref="SOAPException">
		'''                if the <code>Document</code> cannot be added
		''' @since SAAJ 1.2 </exception>
		Function addDocument(ByVal document As org.w3c.dom.Document) As SOAPBodyElement

		''' <summary>
		''' Creates a new DOM <code><seealso cref="org.w3c.dom.Document"/></code> and sets
		''' the first child of this <code>SOAPBody</code> as it's document
		''' element. The child <code>SOAPElement</code> is removed as part of the
		''' process.
		''' </summary>
		''' <returns> the <code><seealso cref="org.w3c.dom.Document"/></code> representation
		'''         of the <code>SOAPBody</code> content.
		''' </returns>
		''' <exception cref="SOAPException">
		'''                if there is not exactly one child <code>SOAPElement</code> of the <code>
		'''              <code>SOAPBody</code>.
		''' 
		''' @since SAAJ 1.3 </exception>
		Function extractContentAsDocument() As org.w3c.dom.Document
	End Interface

End Namespace
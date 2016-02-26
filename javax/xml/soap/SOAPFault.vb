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
	''' An element in the <code>SOAPBody</code> object that contains
	''' error and/or status information. This information may relate to
	''' errors in the <code>SOAPMessage</code> object or to problems
	''' that are not related to the content in the message itself. Problems
	''' not related to the message itself are generally errors in
	''' processing, such as the inability to communicate with an upstream
	''' server.
	''' <P>
	''' Depending on the <code>protocol</code> specified while creating the
	''' <code>MessageFactory</code> instance,  a <code>SOAPFault</code> has
	''' sub-elements as defined in the SOAP 1.1/SOAP 1.2 specification.
	''' </summary>
	Public Interface SOAPFault
		Inherits SOAPBodyElement

		''' <summary>
		''' Sets this <code>SOAPFault</code> object with the given fault code.
		''' 
		''' <P> Fault codes, which give information about the fault, are defined
		''' in the SOAP 1.1 specification. A fault code is mandatory and must
		''' be of type <code>Name</code>. This method provides a convenient
		''' way to set a fault code. For example,
		''' 
		''' <PRE>
		''' SOAPEnvelope se = ...;
		''' // Create a qualified name in the SOAP namespace with a localName
		''' // of "Client". Note that prefix parameter is optional and is null
		''' // here which causes the implementation to use an appropriate prefix.
		''' Name qname = se.createName("Client", null,
		'''                            SOAPConstants.URI_NS_SOAP_ENVELOPE);
		''' SOAPFault fault = ...;
		''' fault.setFaultCode(qname);
		''' </PRE>
		''' It is preferable to use this method over <seealso cref="#setFaultCode(String)"/>.
		''' </summary>
		''' <param name="faultCodeQName"> a <code>Name</code> object giving the fault
		''' code to be set. It must be namespace qualified. </param>
		''' <seealso cref= #getFaultCodeAsName
		''' </seealso>
		''' <exception cref="SOAPException"> if there was an error in adding the
		'''            <i>faultcode</i> element to the underlying XML tree.
		''' 
		''' @since SAAJ 1.2 </exception>
		Property faultCode As Name

		''' <summary>
		''' Sets this <code>SOAPFault</code> object with the given fault code.
		''' 
		''' It is preferable to use this method over <seealso cref="#setFaultCode(Name)"/>.
		''' </summary>
		''' <param name="faultCodeQName"> a <code>QName</code> object giving the fault
		''' code to be set. It must be namespace qualified. </param>
		''' <seealso cref= #getFaultCodeAsQName
		''' </seealso>
		''' <exception cref="SOAPException"> if there was an error in adding the
		'''            <code>faultcode</code> element to the underlying XML tree.
		''' </exception>
		''' <seealso cref= #setFaultCode(Name) </seealso>
		''' <seealso cref= #getFaultCodeAsQName()
		''' 
		''' @since SAAJ 1.3 </seealso>
		WriteOnly Property faultCode As javax.xml.namespace.QName

		''' <summary>
		''' Sets this <code>SOAPFault</code> object with the give fault code.
		''' <P>
		''' Fault codes, which given information about the fault, are defined in
		''' the SOAP 1.1 specification. This element is mandatory in SOAP 1.1.
		''' Because the fault code is required to be a QName it is preferable to
		''' use the <seealso cref="#setFaultCode(Name)"/> form of this method.
		''' </summary>
		''' <param name="faultCode"> a <code>String</code> giving the fault code to be set.
		'''         It must be of the form "prefix:localName" where the prefix has
		'''         been defined in a namespace declaration. </param>
		''' <seealso cref= #setFaultCode(Name) </seealso>
		''' <seealso cref= #getFaultCode </seealso>
		''' <seealso cref= SOAPElement#addNamespaceDeclaration
		''' </seealso>
		''' <exception cref="SOAPException"> if there was an error in adding the
		'''            <code>faultCode</code> to the underlying XML tree. </exception>
		WriteOnly Property faultCode As String

		''' <summary>
		''' Gets the mandatory SOAP 1.1 fault code for this
		''' <code>SOAPFault</code> object as a SAAJ <code>Name</code> object.
		''' The SOAP 1.1 specification requires the value of the "faultcode"
		''' element to be of type QName. This method returns the content of the
		''' element as a QName in the form of a SAAJ Name object. This method
		''' should be used instead of the <code>getFaultCode</code> method since
		''' it allows applications to easily access the namespace name without
		''' additional parsing.
		''' </summary>
		''' <returns> a <code>Name</code> representing the faultcode </returns>
		''' <seealso cref= #setFaultCode(Name)
		''' 
		''' @since SAAJ 1.2 </seealso>
		ReadOnly Property faultCodeAsName As Name


		''' <summary>
		''' Gets the fault code for this
		''' <code>SOAPFault</code> object as a <code>QName</code> object.
		''' </summary>
		''' <returns> a <code>QName</code> representing the faultcode
		''' </returns>
		''' <seealso cref= #setFaultCode(QName)
		''' 
		''' @since SAAJ 1.3 </seealso>
		ReadOnly Property faultCodeAsQName As javax.xml.namespace.QName

		''' <summary>
		''' Gets the Subcodes for this <code>SOAPFault</code> as an iterator over
		''' <code>QNames</code>.
		''' </summary>
		''' <returns> an <code>Iterator</code> that accesses a sequence of
		'''      <code>QNames</code>. This <code>Iterator</code> should not support
		'''      the optional <code>remove</code> method. The order in which the
		'''      Subcodes are returned reflects the hierarchy of Subcodes present
		'''      in the fault from top to bottom.
		''' </returns>
		''' <exception cref="UnsupportedOperationException"> if this message does not
		'''      support the SOAP 1.2 concept of Subcode.
		''' 
		''' @since SAAJ 1.3 </exception>
		ReadOnly Property faultSubcodes As IEnumerator

		''' <summary>
		''' Removes any Subcodes that may be contained by this
		''' <code>SOAPFault</code>. Subsequent calls to
		''' <code>getFaultSubcodes</code> will return an empty iterator until a call
		''' to <code>appendFaultSubcode</code> is made.
		''' </summary>
		''' <exception cref="UnsupportedOperationException"> if this message does not
		'''      support the SOAP 1.2 concept of Subcode.
		''' 
		''' @since SAAJ 1.3 </exception>
		Sub removeAllFaultSubcodes()

		''' <summary>
		''' Adds a Subcode to the end of the sequence of Subcodes contained by this
		''' <code>SOAPFault</code>. Subcodes, which were introduced in SOAP 1.2, are
		''' represented by a recursive sequence of subelements rooted in the
		''' mandatory Code subelement of a SOAP Fault.
		''' </summary>
		''' <param name="subcode"> a QName containing the Value of the Subcode.
		''' </param>
		''' <exception cref="SOAPException"> if there was an error in setting the Subcode </exception>
		''' <exception cref="UnsupportedOperationException"> if this message does not
		'''      support the SOAP 1.2 concept of Subcode.
		''' 
		''' @since SAAJ 1.3 </exception>
		Sub appendFaultSubcode(ByVal subcode As javax.xml.namespace.QName)


		''' <summary>
		''' Sets this <code>SOAPFault</code> object with the given fault actor.
		''' <P>
		''' The fault actor is the recipient in the message path who caused the
		''' fault to happen.
		''' <P>
		''' If this <code>SOAPFault</code> supports SOAP 1.2 then this call is
		''' equivalent to <seealso cref="#setFaultRole(String)"/>
		''' </summary>
		''' <param name="faultActor"> a <code>String</code> identifying the actor that
		'''        caused this <code>SOAPFault</code> object </param>
		''' <seealso cref= #getFaultActor
		''' </seealso>
		''' <exception cref="SOAPException"> if there was an error in adding the
		'''            <code>faultActor</code> to the underlying XML tree. </exception>
		Property faultActor As String


		''' <summary>
		''' Sets the fault string for this <code>SOAPFault</code> object
		''' to the given string.
		''' <P>
		''' If this
		''' <code>SOAPFault</code> is part of a message that supports SOAP 1.2 then
		''' this call is equivalent to:
		''' <pre>
		'''      addFaultReasonText(faultString, Locale.getDefault());
		''' </pre>
		''' </summary>
		''' <param name="faultString"> a <code>String</code> giving an explanation of
		'''        the fault </param>
		''' <seealso cref= #getFaultString
		''' </seealso>
		''' <exception cref="SOAPException"> if there was an error in adding the
		'''            <code>faultString</code> to the underlying XML tree. </exception>
		Property faultString As String

		''' <summary>
		''' Sets the fault string for this <code>SOAPFault</code> object
		''' to the given string and localized to the given locale.
		''' <P>
		''' If this
		''' <code>SOAPFault</code> is part of a message that supports SOAP 1.2 then
		''' this call is equivalent to:
		''' <pre>
		'''      addFaultReasonText(faultString, locale);
		''' </pre>
		''' </summary>
		''' <param name="faultString"> a <code>String</code> giving an explanation of
		'''         the fault </param>
		''' <param name="locale"> a <seealso cref="java.util.Locale Locale"/> object indicating
		'''         the native language of the <code>faultString</code> </param>
		''' <seealso cref= #getFaultString
		''' </seealso>
		''' <exception cref="SOAPException"> if there was an error in adding the
		'''            <code>faultString</code> to the underlying XML tree.
		''' 
		''' @since SAAJ 1.2 </exception>
		Sub setFaultString(ByVal faultString As String, ByVal locale As java.util.Locale)


		''' <summary>
		''' Gets the locale of the fault string for this <code>SOAPFault</code>
		''' object.
		''' <P>
		''' If this
		''' <code>SOAPFault</code> is part of a message that supports SOAP 1.2 then
		''' this call is equivalent to:
		''' <pre>
		'''    Locale locale = null;
		'''    try {
		'''        locale = (Locale) getFaultReasonLocales().next();
		'''    } catch (SOAPException e) {}
		'''    return locale;
		''' </pre>
		''' </summary>
		''' <returns> a <code>Locale</code> object indicating the native language of
		'''          the fault string or <code>null</code> if no locale was specified </returns>
		''' <seealso cref= #setFaultString(String, Locale)
		''' 
		''' @since SAAJ 1.2 </seealso>
		ReadOnly Property faultStringLocale As java.util.Locale

		''' <summary>
		''' Returns true if this <code>SOAPFault</code> has a <code>Detail</code>
		''' subelement and false otherwise. Equivalent to
		''' <code>(getDetail()!=null)</code>.
		''' </summary>
		''' <returns> true if this <code>SOAPFault</code> has a <code>Detail</code>
		''' subelement and false otherwise.
		''' 
		''' @since SAAJ 1.3 </returns>
		Function hasDetail() As Boolean

		''' <summary>
		''' Returns the optional detail element for this <code>SOAPFault</code>
		''' object.
		''' <P>
		''' A <code>Detail</code> object carries application-specific error
		''' information, the scope of the error information is restricted to
		''' faults in the <code>SOAPBodyElement</code> objects if this is a
		''' SOAP 1.1 Fault.
		''' </summary>
		''' <returns> a <code>Detail</code> object with application-specific
		'''         error information if present, null otherwise </returns>
		ReadOnly Property detail As Detail

		''' <summary>
		''' Creates an optional <code>Detail</code> object and sets it as the
		''' <code>Detail</code> object for this <code>SOAPFault</code>
		''' object.
		''' <P>
		''' It is illegal to add a detail when the fault already
		''' contains a detail. Therefore, this method should be called
		''' only after the existing detail has been removed.
		''' </summary>
		''' <returns> the new <code>Detail</code> object
		''' </returns>
		''' <exception cref="SOAPException"> if this
		'''            <code>SOAPFault</code> object already contains a
		'''            valid <code>Detail</code> object </exception>
		Function addDetail() As Detail

		''' <summary>
		''' Returns an <code>Iterator</code> over a distinct sequence of
		''' <code>Locale</code>s for which there are associated Reason Text items.
		''' Any of these <code>Locale</code>s can be used in a call to
		''' <code>getFaultReasonText</code> in order to obtain a localized version
		''' of the Reason Text string.
		''' </summary>
		''' <returns> an <code>Iterator</code> over a sequence of <code>Locale</code>
		'''      objects for which there are associated Reason Text items.
		''' </returns>
		''' <exception cref="SOAPException"> if there was an error in retrieving
		''' the  fault Reason locales. </exception>
		''' <exception cref="UnsupportedOperationException"> if this message does not
		'''      support the SOAP 1.2 concept of Fault Reason.
		''' 
		''' @since SAAJ 1.3 </exception>
		ReadOnly Property faultReasonLocales As IEnumerator

		''' <summary>
		''' Returns an <code>Iterator</code> over a sequence of
		''' <code>String</code> objects containing all of the Reason Text items for
		''' this <code>SOAPFault</code>.
		''' </summary>
		''' <returns> an <code>Iterator</code> over env:Fault/env:Reason/env:Text items.
		''' </returns>
		''' <exception cref="SOAPException"> if there was an error in retrieving
		''' the  fault Reason texts. </exception>
		''' <exception cref="UnsupportedOperationException"> if this message does not
		'''      support the SOAP 1.2 concept of Fault Reason.
		''' 
		''' @since SAAJ 1.3 </exception>
		ReadOnly Property faultReasonTexts As IEnumerator

		''' <summary>
		''' Returns the Reason Text associated with the given <code>Locale</code>.
		''' If more than one such Reason Text exists the first matching Text is
		''' returned
		''' </summary>
		''' <param name="locale"> -- the <code>Locale</code> for which a localized
		'''      Reason Text is desired
		''' </param>
		''' <returns> the Reason Text associated with <code>locale</code>
		''' </returns>
		''' <seealso cref= #getFaultString
		''' </seealso>
		''' <exception cref="SOAPException"> if there was an error in retrieving
		''' the  fault Reason text for the specified locale . </exception>
		''' <exception cref="UnsupportedOperationException"> if this message does not
		'''      support the SOAP 1.2 concept of Fault Reason.
		''' 
		''' @since SAAJ 1.3 </exception>
		Function getFaultReasonText(ByVal locale As java.util.Locale) As String

		''' <summary>
		''' Appends or replaces a Reason Text item containing the specified
		''' text message and an <i>xml:lang</i> derived from
		''' <code>locale</code>. If a Reason Text item with this
		''' <i>xml:lang</i> already exists its text value will be replaced
		''' with <code>text</code>.
		''' The <code>locale</code> parameter should not be <code>null</code>
		''' <P>
		''' Code sample:
		''' 
		''' <PRE>
		''' SOAPFault fault = ...;
		''' fault.addFaultReasonText("Version Mismatch", Locale.ENGLISH);
		''' </PRE>
		''' </summary>
		''' <param name="text"> -- reason message string </param>
		''' <param name="locale"> -- Locale object representing the locale of the message
		''' </param>
		''' <exception cref="SOAPException"> if there was an error in adding the Reason text
		''' or the <code>locale</code> passed was <code>null</code>. </exception>
		''' <exception cref="UnsupportedOperationException"> if this message does not
		'''      support the SOAP 1.2 concept of Fault Reason.
		''' 
		''' @since SAAJ 1.3 </exception>
		Sub addFaultReasonText(ByVal text As String, ByVal locale As java.util.Locale)

		''' <summary>
		''' Returns the optional Node element value for this
		''' <code>SOAPFault</code> object. The Node element is
		''' optional in SOAP 1.2.
		''' </summary>
		''' <returns> Content of the env:Fault/env:Node element as a String
		''' or <code>null</code> if none
		''' </returns>
		''' <exception cref="UnsupportedOperationException"> if this message does not
		'''      support the SOAP 1.2 concept of Fault Node.
		''' 
		''' @since SAAJ 1.3 </exception>
		Property faultNode As String


		''' <summary>
		''' Returns the optional Role element value for this
		''' <code>SOAPFault</code> object. The Role element is
		''' optional in SOAP 1.2.
		''' </summary>
		''' <returns> Content of the env:Fault/env:Role element as a String
		''' or <code>null</code> if none
		''' </returns>
		''' <exception cref="UnsupportedOperationException"> if this message does not
		'''      support the SOAP 1.2 concept of Fault Role.
		''' 
		''' @since SAAJ 1.3 </exception>
		Property faultRole As String


	End Interface

End Namespace
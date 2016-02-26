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
	''' The definition of constants pertaining to the SOAP protocol.
	''' </summary>
	Public Interface SOAPConstants
		''' <summary>
		''' Used to create <code>MessageFactory</code> instances that create
		''' <code>SOAPMessages</code> whose concrete type is based on the
		''' <code>Content-Type</code> MIME header passed to the
		''' <code>createMessage</code> method. If no <code>Content-Type</code>
		''' header is passed then the <code>createMessage</code> may throw an
		''' <code>IllegalArgumentException</code> or, in the case of the no
		''' argument version of <code>createMessage</code>, an
		''' <code>UnsupportedOperationException</code>.
		''' 
		''' @since  SAAJ 1.3
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final String DYNAMIC_SOAP_PROTOCOL = "Dynamic Protocol";

		''' <summary>
		''' Used to create <code>MessageFactory</code> instances that create
		''' <code>SOAPMessages</code> whose behavior supports the SOAP 1.1  specification.
		''' 
		''' @since  SAAJ 1.3
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final String SOAP_1_1_PROTOCOL = "SOAP 1.1 Protocol";

		''' <summary>
		''' Used to create <code>MessageFactory</code> instances that create
		''' <code>SOAPMessages</code> whose behavior supports the SOAP 1.2
		''' specification
		''' 
		''' @since  SAAJ 1.3
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final String SOAP_1_2_PROTOCOL = "SOAP 1.2 Protocol";

		''' <summary>
		''' The default protocol: SOAP 1.1 for backwards compatibility.
		''' 
		''' @since SAAJ 1.3
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final String DEFAULT_SOAP_PROTOCOL = SOAP_1_1_PROTOCOL;

		''' <summary>
		''' The namespace identifier for the SOAP 1.1 envelope.
		''' @since SAAJ 1.3
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final String URI_NS_SOAP_1_1_ENVELOPE = "http://schemas.xmlsoap.org/soap/envelope/";
		''' <summary>
		''' The namespace identifier for the SOAP 1.2 envelope.
		''' @since SAAJ 1.3
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final String URI_NS_SOAP_1_2_ENVELOPE = "http://www.w3.org/2003/05/soap-envelope";

		''' <summary>
		''' The namespace identifier for the SOAP 1.1 envelope, All SOAPElements in this
		''' namespace are defined by the SOAP 1.1 specification.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final String URI_NS_SOAP_ENVELOPE = URI_NS_SOAP_1_1_ENVELOPE;

		''' <summary>
		''' The namespace identifier for the SOAP 1.1 encoding.
		''' An attribute named <code>encodingStyle</code> in the
		''' <code>URI_NS_SOAP_ENVELOPE</code> namespace and set to the value
		''' <code>URI_NS_SOAP_ENCODING</code> can be added to an element to indicate
		''' that it is encoded using the rules in section 5 of the SOAP 1.1
		''' specification.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final String URI_NS_SOAP_ENCODING = "http://schemas.xmlsoap.org/soap/encoding/";

		''' <summary>
		''' The namespace identifier for the SOAP 1.2 encoding.
		''' @since SAAJ 1.3
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final String URI_NS_SOAP_1_2_ENCODING = "http://www.w3.org/2003/05/soap-encoding";

		''' <summary>
		''' The media type  of the <code>Content-Type</code> MIME header in SOAP 1.1.
		''' @since SAAJ 1.3
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final String SOAP_1_1_CONTENT_TYPE = "text/xml";

		''' <summary>
		''' The media type  of the <code>Content-Type</code> MIME header in SOAP 1.2.
		''' @since SAAJ 1.3
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final String SOAP_1_2_CONTENT_TYPE = "application/soap+xml";

		''' <summary>
		''' The URI identifying the next application processing a SOAP request as the intended
		''' actor for a SOAP 1.1 header entry (see section 4.2.2 of the SOAP 1.1 specification).
		''' <p>
		''' This value can be passed to
		''' <seealso cref="SOAPHeader#examineMustUnderstandHeaderElements(String)"/>,
		''' <seealso cref="SOAPHeader#examineHeaderElements(String)"/> and
		''' <seealso cref="SOAPHeader#extractHeaderElements(String)"/>
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final String URI_SOAP_ACTOR_NEXT = "http://schemas.xmlsoap.org/soap/actor/next";

		''' <summary>
		''' The URI identifying the next application processing a SOAP request as the intended
		''' role for a SOAP 1.2 header entry (see section 2.2 of part 1 of the SOAP 1.2
		''' specification).
		''' @since SAAJ 1.3
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final String URI_SOAP_1_2_ROLE_NEXT = URI_NS_SOAP_1_2_ENVELOPE + "/role/next";

		''' <summary>
		''' The URI specifying the role None in SOAP 1.2.
		''' @since SAAJ 1.3
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final String URI_SOAP_1_2_ROLE_NONE = URI_NS_SOAP_1_2_ENVELOPE + "/role/none";

		''' <summary>
		''' The URI identifying the ultimate receiver of the SOAP 1.2 message.
		''' @since SAAJ 1.3
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final String URI_SOAP_1_2_ROLE_ULTIMATE_RECEIVER = URI_NS_SOAP_1_2_ENVELOPE + "/role/ultimateReceiver";

		''' <summary>
		''' The default namespace prefix for http://www.w3.org/2003/05/soap-envelope
		''' @since SAAJ 1.3
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final String SOAP_ENV_PREFIX = "env";

		''' <summary>
		''' SOAP 1.2 VersionMismatch Fault
		''' @since SAAJ 1.3
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final javax.xml.namespace.QName SOAP_VERSIONMISMATCH_FAULT = New javax.xml.namespace.QName(URI_NS_SOAP_1_2_ENVELOPE, "VersionMismatch", SOAP_ENV_PREFIX);

		''' <summary>
		''' SOAP 1.2 MustUnderstand Fault
		''' @since SAAJ 1.3
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final javax.xml.namespace.QName SOAP_MUSTUNDERSTAND_FAULT = New javax.xml.namespace.QName(URI_NS_SOAP_1_2_ENVELOPE, "MustUnderstand", SOAP_ENV_PREFIX);

		''' <summary>
		''' SOAP 1.2 DataEncodingUnknown Fault
		''' @since SAAJ 1.3
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final javax.xml.namespace.QName SOAP_DATAENCODINGUNKNOWN_FAULT = New javax.xml.namespace.QName(URI_NS_SOAP_1_2_ENVELOPE, "DataEncodingUnknown", SOAP_ENV_PREFIX);

		''' <summary>
		''' SOAP 1.2 Sender Fault
		''' @since SAAJ 1.3
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final javax.xml.namespace.QName SOAP_SENDER_FAULT = New javax.xml.namespace.QName(URI_NS_SOAP_1_2_ENVELOPE, "Sender", SOAP_ENV_PREFIX);

		''' <summary>
		''' SOAP 1.2 Receiver Fault
		''' @since SAAJ 1.3
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final javax.xml.namespace.QName SOAP_RECEIVER_FAULT = New javax.xml.namespace.QName(URI_NS_SOAP_1_2_ENVELOPE, "Receiver", SOAP_ENV_PREFIX);

	End Interface

End Namespace
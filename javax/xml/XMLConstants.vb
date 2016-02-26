'
' * Copyright (c) 2003, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.xml

	''' <summary>
	''' <p>Utility class to contain basic XML values as constants.</p>
	''' 
	''' @author <a href="mailto:Jeff.Suttor@Sun.com">Jeff Suttor</a>
	''' @version $Revision: 1.8 $, $Date: 2010/05/25 16:19:45 $ </summary>
	''' <seealso cref= <a href="http://www.w3.org/TR/xml11/">Extensible Markup Language (XML) 1.1</a> </seealso>
	''' <seealso cref= <a href="http://www.w3.org/TR/REC-xml">Extensible Markup Language (XML) 1.0 (Second Edition)</a> </seealso>
	''' <seealso cref= <a href="http://www.w3.org/XML/xml-V10-2e-errata">XML 1.0 Second Edition Specification Errata</a> </seealso>
	''' <seealso cref= <a href="http://www.w3.org/TR/xml-names11/">Namespaces in XML 1.1</a> </seealso>
	''' <seealso cref= <a href="http://www.w3.org/TR/REC-xml-names">Namespaces in XML</a> </seealso>
	''' <seealso cref= <a href="http://www.w3.org/XML/xml-names-19990114-errata">Namespaces in XML Errata</a> </seealso>
	''' <seealso cref= <a href="http://www.w3.org/TR/xmlschema-1/">XML Schema Part 1: Structures</a>
	''' @since 1.5
	'''  </seealso>

	Public NotInheritable Class XMLConstants

		''' <summary>
		''' <p>Private constructor to prevent instantiation.</p>
		''' </summary>
			Private Sub New()
			End Sub

		''' <summary>
		''' <p>Namespace URI to use to represent that there is no Namespace.</p>
		''' 
		''' <p>Defined by the Namespace specification to be "".</p>
		''' </summary>
		''' <seealso cref= <a href="http://www.w3.org/TR/REC-xml-names/#defaulting">
		''' Namespaces in XML, 5.2 Namespace Defaulting</a> </seealso>
		Public Const NULL_NS_URI As String = ""

		''' <summary>
		''' <p>Prefix to use to represent the default XML Namespace.</p>
		''' 
		''' <p>Defined by the XML specification to be "".</p>
		''' </summary>
		''' <seealso cref= <a
		''' href="http://www.w3.org/TR/REC-xml-names/#ns-qualnames">
		''' Namespaces in XML, 3. Qualified Names</a> </seealso>
		Public Const DEFAULT_NS_PREFIX As String = ""

		''' <summary>
		''' <p>The official XML Namespace name URI.</p>
		''' 
		''' <p>Defined by the XML specification to be
		''' "{@code http://www.w3.org/XML/1998/namespace}".</p>
		''' </summary>
		''' <seealso cref= <a
		''' href="http://www.w3.org/TR/REC-xml-names/#ns-qualnames">
		''' Namespaces in XML, 3. Qualified Names</a> </seealso>
		Public Const XML_NS_URI As String = "http://www.w3.org/XML/1998/namespace"

		''' <summary>
		''' <p>The official XML Namespace prefix.</p>
		''' 
		''' <p>Defined by the XML specification to be "{@code xml}".</p>
		''' </summary>
		''' <seealso cref= <a
		''' href="http://www.w3.org/TR/REC-xml-names/#ns-qualnames">
		''' Namespaces in XML, 3. Qualified Names<</a> </seealso>
		Public Const XML_NS_PREFIX As String = "xml"

		''' <summary>
		''' <p>The official XML attribute used for specifying XML Namespace
		''' declarations, {@link #XMLNS_ATTRIBUTE
		''' XMLConstants.XMLNS_ATTRIBUTE}, Namespace name URI.</p>
		''' 
		''' <p>Defined by the XML specification to be
		''' "{@code http://www.w3.org/2000/xmlns/}".</p>
		''' </summary>
		''' <seealso cref= <a
		''' href="http://www.w3.org/TR/REC-xml-names/#ns-qualnames">
		''' Namespaces in XML, 3. Qualified Names</a> </seealso>
		''' <seealso cref= <a
		''' href="http://www.w3.org/XML/xml-names-19990114-errata">
		''' Namespaces in XML Errata</a> </seealso>
		Public Const XMLNS_ATTRIBUTE_NS_URI As String = "http://www.w3.org/2000/xmlns/"

		''' <summary>
		''' <p>The official XML attribute used for specifying XML Namespace
		''' declarations.</p>
		''' 
		''' <p>It is <strong><em>NOT</em></strong> valid to use as a
		''' prefix.  Defined by the XML specification to be
		''' "{@code xmlns}".</p>
		''' </summary>
		''' <seealso cref= <a
		''' href="http://www.w3.org/TR/REC-xml-names/#ns-qualnames">
		''' Namespaces in XML, 3. Qualified Names</a> </seealso>
		Public Const XMLNS_ATTRIBUTE As String = "xmlns"

		''' <summary>
		''' <p>W3C XML Schema Namespace URI.</p>
		''' 
		''' <p>Defined to be "{@code http://www.w3.org/2001/XMLSchema}".
		''' </summary>
		''' <seealso cref= <a href=
		'''  "http://www.w3.org/TR/xmlschema-1/#Instance_Document_Constructions">
		'''  XML Schema Part 1:
		'''  Structures, 2.6 Schema-Related Markup in Documents Being Validated</a> </seealso>
		Public Const W3C_XML_SCHEMA_NS_URI As String = "http://www.w3.org/2001/XMLSchema"

		''' <summary>
		''' <p>W3C XML Schema Instance Namespace URI.</p>
		''' 
		''' <p>Defined to be "{@code http://www.w3.org/2001/XMLSchema-instance}".</p>
		''' </summary>
		''' <seealso cref= <a href=
		'''  "http://www.w3.org/TR/xmlschema-1/#Instance_Document_Constructions">
		'''  XML Schema Part 1:
		'''  Structures, 2.6 Schema-Related Markup in Documents Being Validated</a> </seealso>
		Public Const W3C_XML_SCHEMA_INSTANCE_NS_URI As String = "http://www.w3.org/2001/XMLSchema-instance"

			''' <summary>
			''' <p>W3C XPath Datatype Namespace URI.</p>
			''' 
			''' <p>Defined to be "{@code http://www.w3.org/2003/11/xpath-datatypes}".</p>
			''' </summary>
			''' <seealso cref= <a href="http://www.w3.org/TR/xpath-datamodel">XQuery 1.0 and XPath 2.0 Data Model</a> </seealso>
			Public Const W3C_XPATH_DATATYPE_NS_URI As String = "http://www.w3.org/2003/11/xpath-datatypes"

		''' <summary>
		''' <p>XML Document Type Declaration Namespace URI as an arbitrary value.</p>
		''' 
		''' <p>Since not formally defined by any existing standard, arbitrarily define to be "{@code http://www.w3.org/TR/REC-xml}".
		''' </summary>
		Public Const XML_DTD_NS_URI As String = "http://www.w3.org/TR/REC-xml"

			''' <summary>
			''' <p>RELAX NG Namespace URI.</p>
			''' 
			''' <p>Defined to be "{@code http://relaxng.org/ns/structure/1.0}".</p>
			''' </summary>
			''' <seealso cref= <a href="http://relaxng.org/spec-20011203.html">RELAX NG Specification</a> </seealso>
			Public Const RELAXNG_NS_URI As String = "http://relaxng.org/ns/structure/1.0"

			''' <summary>
			''' <p>Feature for secure processing.</p>
			''' 
			''' <ul>
			'''   <li>
			'''     {@code true} instructs the implementation to process XML securely.
			'''     This may set limits on XML constructs to avoid conditions such as denial of service attacks.
			'''   </li>
			'''   <li>
			'''     {@code false} instructs the implementation to process XML in accordance with the XML specifications
			'''     ignoring security issues such as limits on XML constructs to avoid conditions such as denial of service attacks.
			'''   </li>
			''' </ul>
			''' </summary>
			Public Const FEATURE_SECURE_PROCESSING As String = "http://javax.xml.XMLConstants/feature/secure-processing"


			''' <summary>
			''' <p>Property: accessExternalDTD</p>
			''' 
			''' <p>
			''' Restrict access to external DTDs and external Entity References to the protocols specified.
			''' If access is denied due to the restriction of this property, a runtime exception that
			''' is specific to the context is thrown. In the case of <seealso cref="javax.xml.parsers.SAXParser"/>
			''' for example, <seealso cref="org.xml.sax.SAXException"/> is thrown.
			''' </p>
			''' 
			''' <p>
			''' <b>Value: </b> a list of protocols separated by comma. A protocol is the scheme portion of a
			''' <seealso cref="java.net.URI"/>, or in the case of the JAR protocol, "jar" plus the scheme portion
			''' separated by colon.
			''' A scheme is defined as:
			''' 
			''' <blockquote>
			''' scheme = alpha *( alpha | digit | "+" | "-" | "." )<br>
			''' where alpha = a-z and A-Z.<br><br>
			''' 
			''' And the JAR protocol:<br>
			''' 
			''' jar[:scheme]<br><br>
			''' 
			''' Protocols including the keyword "jar" are case-insensitive. Any whitespaces as defined by
			''' <seealso cref="java.lang.Character#isSpaceChar "/> in the value will be ignored.
			''' Examples of protocols are file, http, jar:file.
			''' 
			''' </blockquote>
			''' </p>
			''' 
			''' <p>
			''' <b>Default value:</b> The default value is implementation specific and therefore not specified.
			''' The following options are provided for consideration:
			''' <blockquote>
			''' <UL>
			'''     <LI>an empty string to deny all access to external references;</LI>
			'''     <LI>a specific protocol, such as file, to give permission to only the protocol;</LI>
			'''     <LI>the keyword "all" to grant  permission to all protocols.</LI>
			''' </UL><br>
			'''      When FEATURE_SECURE_PROCESSING is enabled,  it is recommended that implementations
			'''      restrict external connections by default, though this may cause problems for applications
			'''      that process XML/XSD/XSL with external references.
			''' </blockquote>
			''' </p>
			''' 
			''' <p>
			''' <b>Granting all access:</b>  the keyword "all" grants permission to all protocols.
			''' </p>
			''' <p>
			''' <b>System Property:</b> The value of this property can be set or overridden by
			''' system property {@code javax.xml.accessExternalDTD}.
			''' </p>
			''' 
			''' <p>
			''' <b>${JAVA_HOME}/lib/jaxp.properties:</b> This configuration file is in standard
			''' <seealso cref="java.util.Properties"/> format. If the file exists and the system property is specified,
			''' its value will be used to override the default of the property.
			''' </p>
			''' 
			''' <p>
			''' 
			''' </p>
			''' @since 1.7
			''' </summary>
			Public Const ACCESS_EXTERNAL_DTD As String = "http://javax.xml.XMLConstants/property/accessExternalDTD"

			''' <summary>
			''' <p>Property: accessExternalSchema</p>
			''' 
			''' <p>
			''' Restrict access to the protocols specified for external reference set by the
			''' schemaLocation attribute, Import and Include element. If access is denied
			''' due to the restriction of this property, a runtime exception that is specific
			''' to the context is thrown. In the case of <seealso cref="javax.xml.validation.SchemaFactory"/>
			''' for example, org.xml.sax.SAXException is thrown.
			''' </p>
			''' <p>
			''' <b>Value:</b> a list of protocols separated by comma. A protocol is the scheme portion of a
			''' <seealso cref="java.net.URI"/>, or in the case of the JAR protocol, "jar" plus the scheme portion
			''' separated by colon.
			''' A scheme is defined as:
			''' 
			''' <blockquote>
			''' scheme = alpha *( alpha | digit | "+" | "-" | "." )<br>
			''' where alpha = a-z and A-Z.<br><br>
			''' 
			''' And the JAR protocol:<br>
			''' 
			''' jar[:scheme]<br><br>
			''' 
			''' Protocols including the keyword "jar" are case-insensitive. Any whitespaces as defined by
			''' <seealso cref="java.lang.Character#isSpaceChar "/> in the value will be ignored.
			''' Examples of protocols are file, http, jar:file.
			''' 
			''' </blockquote>
			''' </p>
			''' 
			''' <p>
			''' <b>Default value:</b> The default value is implementation specific and therefore not specified.
			''' The following options are provided for consideration:
			''' <blockquote>
			''' <UL>
			'''     <LI>an empty string to deny all access to external references;</LI>
			'''     <LI>a specific protocol, such as file, to give permission to only the protocol;</LI>
			'''     <LI>the keyword "all" to grant  permission to all protocols.</LI>
			''' </UL><br>
			'''      When FEATURE_SECURE_PROCESSING is enabled,  it is recommended that implementations
			'''      restrict external connections by default, though this may cause problems for applications
			'''      that process XML/XSD/XSL with external references.
			''' </blockquote>
			''' </p>
			''' <p>
			''' <b>Granting all access:</b>  the keyword "all" grants permission to all protocols.
			''' </p>
			''' 
			''' <p>
			''' <b>System Property:</b> The value of this property can be set or overridden by
			''' system property {@code javax.xml.accessExternalSchema}
			''' </p>
			''' 
			''' <p>
			''' <b>${JAVA_HOME}/lib/jaxp.properties:</b> This configuration file is in standard
			''' java.util.Properties format. If the file exists and the system property is specified,
			''' its value will be used to override the default of the property.
			''' 
			''' @since 1.7
			''' </p>
			''' </summary>
			Public Const ACCESS_EXTERNAL_SCHEMA As String = "http://javax.xml.XMLConstants/property/accessExternalSchema"

			''' <summary>
			''' <p>Property: accessExternalStylesheet</p>
			''' 
			''' <p>
			''' Restrict access to the protocols specified for external references set by the
			''' stylesheet processing instruction, Import and Include element, and document function.
			''' If access is denied due to the restriction of this property, a runtime exception
			''' that is specific to the context is thrown. In the case of constructing new
			''' <seealso cref="javax.xml.transform.Transformer"/> for example,
			''' <seealso cref="javax.xml.transform.TransformerConfigurationException"/>
			''' will be thrown by the <seealso cref="javax.xml.transform.TransformerFactory"/>.
			''' </p>
			''' <p>
			''' <b>Value:</b> a list of protocols separated by comma. A protocol is the scheme portion of a
			''' <seealso cref="java.net.URI"/>, or in the case of the JAR protocol, "jar" plus the scheme portion
			''' separated by colon.
			''' A scheme is defined as:
			''' 
			''' <blockquote>
			''' scheme = alpha *( alpha | digit | "+" | "-" | "." )<br>
			''' where alpha = a-z and A-Z.<br><br>
			''' 
			''' And the JAR protocol:<br>
			''' 
			''' jar[:scheme]<br><br>
			''' 
			''' Protocols including the keyword "jar" are case-insensitive. Any whitespaces as defined by
			''' <seealso cref="java.lang.Character#isSpaceChar "/> in the value will be ignored.
			''' Examples of protocols are file, http, jar:file.
			''' 
			''' </blockquote>
			''' </p>
			''' 
			''' <p>
			''' <b>Default value:</b> The default value is implementation specific and therefore not specified.
			''' The following options are provided for consideration:
			''' <blockquote>
			''' <UL>
			'''     <LI>an empty string to deny all access to external references;</LI>
			'''     <LI>a specific protocol, such as file, to give permission to only the protocol;</LI>
			'''     <LI>the keyword "all" to grant  permission to all protocols.</LI>
			''' </UL><br>
			'''      When FEATURE_SECURE_PROCESSING is enabled,  it is recommended that implementations
			'''      restrict external connections by default, though this may cause problems for applications
			'''      that process XML/XSD/XSL with external references.
			''' </blockquote>
			''' </p>
			''' <p>
			''' <b>Granting all access:</b>  the keyword "all" grants permission to all protocols.
			''' </p>
			''' 
			''' <p>
			''' <b>System Property:</b> The value of this property can be set or overridden by
			''' system property {@code javax.xml.accessExternalStylesheet}
			''' </p>
			''' 
			''' <p>
			''' <b>${JAVA_HOME}/lib/jaxp.properties: </b> This configuration file is in standard
			''' java.util.Properties format. If the file exists and the system property is specified,
			''' its value will be used to override the default of the property.
			''' 
			''' @since 1.7
			''' </summary>
			Public Const ACCESS_EXTERNAL_STYLESHEET As String = "http://javax.xml.XMLConstants/property/accessExternalStylesheet"

	End Class

End Namespace
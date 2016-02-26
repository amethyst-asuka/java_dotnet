Imports System

'
' * Copyright (c) 2003, 2006, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.xml.namespace



	''' <summary>
	''' <p><code>QName</code> represents a <strong>qualified name</strong>
	''' as defined in the XML specifications: <a
	''' href="http://www.w3.org/TR/xmlschema-2/#QName">XML Schema Part2:
	''' Datatypes specification</a>, <a
	''' href="http://www.w3.org/TR/REC-xml-names/#ns-qualnames">Namespaces
	''' in XML</a>, <a
	''' href="http://www.w3.org/XML/xml-names-19990114-errata">Namespaces
	''' in XML Errata</a>.</p>
	''' 
	''' <p>The value of a <code>QName</code> contains a <strong>Namespace
	''' URI</strong>, <strong>local part</strong> and
	''' <strong>prefix</strong>.</p>
	''' 
	''' <p>The prefix is included in <code>QName</code> to retain lexical
	''' information <strong><em>when present</em></strong> in an {@link
	''' javax.xml.transform.Source XML input source}. The prefix is
	''' <strong><em>NOT</em></strong> used in {@link #equals(Object)
	''' QName.equals(Object)} or to compute the {@link #hashCode()
	''' QName.hashCode()}.  Equality and the hash code are defined using
	''' <strong><em>only</em></strong> the Namespace URI and local part.</p>
	''' 
	''' <p>If not specified, the Namespace URI is set to {@link
	''' javax.xml.XMLConstants#NULL_NS_URI XMLConstants.NULL_NS_URI}.
	''' If not specified, the prefix is set to {@link
	''' javax.xml.XMLConstants#DEFAULT_NS_PREFIX
	''' XMLConstants.DEFAULT_NS_PREFIX}.</p>
	''' 
	''' <p><code>QName</code> is immutable.</p>
	''' 
	''' @author <a href="mailto:Jeff.Suttor@Sun.com">Jeff Suttor</a>
	''' @version $Revision: 1.8 $, $Date: 2010/03/18 03:06:17 $ </summary>
	''' <seealso cref= <a href="http://www.w3.org/TR/xmlschema-2/#QName">
	'''   XML Schema Part2: Datatypes specification</a> </seealso>
	''' <seealso cref= <a href="http://www.w3.org/TR/REC-xml-names/#ns-qualnames">
	'''   Namespaces in XML</a> </seealso>
	''' <seealso cref= <a href="http://www.w3.org/XML/xml-names-19990114-errata">
	'''   Namespaces in XML Errata</a>
	''' @since 1.5 </seealso>

	<Serializable> _
	Public Class QName

		''' <summary>
		''' <p>Stream Unique Identifier.</p>
		''' 
		''' <p>Due to a historical defect, QName was released with multiple
		''' serialVersionUID values even though its serialization was the
		''' same.</p>
		''' 
		''' <p>To workaround this issue, serialVersionUID is set with either
		''' a default value or a compatibility value.  To use the
		''' compatiblity value, set the system property:</p>
		''' 
		''' <code>com.sun.xml.namespace.QName.useCompatibleSerialVersionUID=1.0</code>
		''' 
		''' <p>This workaround was inspired by classes in the javax.management
		''' package, e.g. ObjectName, etc.
		''' See CR6267224 for original defect report.</p>
		''' </summary>
		Private Shared ReadOnly serialVersionUID As Long
		''' <summary>
		''' <p>Default <code>serialVersionUID</code> value.</p>
		''' </summary>
		Private Const defaultSerialVersionUID As Long = -9120448754896609940L
		''' <summary>
		''' <p>Compatibility <code>serialVersionUID</code> value.</p>
		''' </summary>
		Private Const compatibleSerialVersionUID As Long = 4418622981026545151L
		''' <summary>
		''' <p>Flag to use default or campatible serialVersionUID.</p>
		''' </summary>
		Private Shared useDefaultSerialVersionUID As Boolean = True
		Shared Sub New()
			Try
				' use a privileged block as reading a system property
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				String valueUseCompatibleSerialVersionUID = (String) java.security.AccessController.doPrivileged(New java.security.PrivilegedAction()
	'			{
	'						public Object run()
	'						{
	'							Return System.getProperty("com.sun.xml.namespace.QName.useCompatibleSerialVersionUID");
	'						}
	'					}
			   )
				useDefaultSerialVersionUID = If(valueUseCompatibleSerialVersionUID IsNot Nothing AndAlso valueUseCompatibleSerialVersionUID.Equals("1.0"), False, True)
			Catch exception As Exception
				' use default if any Exceptions
				useDefaultSerialVersionUID = True
			End Try

			' set serialVersionUID to desired value
			If useDefaultSerialVersionUID Then
				serialVersionUID = defaultSerialVersionUID
			Else
				serialVersionUID = compatibleSerialVersionUID
			End If
		End Sub

		''' <summary>
		''' <p>Namespace URI of this <code>QName</code>.</p>
		''' </summary>
		Private ReadOnly namespaceURI As String

		''' <summary>
		''' <p>local part of this <code>QName</code>.</p>
		''' </summary>
		Private ReadOnly localPart As String

		''' <summary>
		''' <p>prefix of this <code>QName</code>.</p>
		''' </summary>
		Private ReadOnly prefix As String

		''' <summary>
		''' <p><code>QName</code> constructor specifying the Namespace URI
		''' and local part.</p>
		''' 
		''' <p>If the Namespace URI is <code>null</code>, it is set to
		''' {@link javax.xml.XMLConstants#NULL_NS_URI
		''' XMLConstants.NULL_NS_URI}.  This value represents no
		''' explicitly defined Namespace as defined by the <a
		''' href="http://www.w3.org/TR/REC-xml-names/#ns-qualnames">Namespaces
		''' in XML</a> specification.  This action preserves compatible
		''' behavior with QName 1.0.  Explicitly providing the {@link
		''' javax.xml.XMLConstants#NULL_NS_URI
		''' XMLConstants.NULL_NS_URI} value is the preferred coding
		''' style.</p>
		''' 
		''' <p>If the local part is <code>null</code> an
		''' <code>IllegalArgumentException</code> is thrown.
		''' A local part of "" is allowed to preserve
		''' compatible behavior with QName 1.0. </p>
		''' 
		''' <p>When using this constructor, the prefix is set to {@link
		''' javax.xml.XMLConstants#DEFAULT_NS_PREFIX
		''' XMLConstants.DEFAULT_NS_PREFIX}.</p>
		''' 
		''' <p>The Namespace URI is not validated as a
		''' <a href="http://www.ietf.org/rfc/rfc2396.txt">URI reference</a>.
		''' The local part is not validated as a
		''' <a href="http://www.w3.org/TR/REC-xml-names/#NT-NCName">NCName</a>
		''' as specified in <a href="http://www.w3.org/TR/REC-xml-names/">Namespaces
		''' in XML</a>.</p>
		''' </summary>
		''' <param name="namespaceURI"> Namespace URI of the <code>QName</code> </param>
		''' <param name="localPart">    local part of the <code>QName</code>
		''' </param>
		''' <exception cref="IllegalArgumentException"> When <code>localPart</code> is
		'''   <code>null</code>
		''' </exception>
		''' <seealso cref= #QName(String namespaceURI, String localPart, String
		''' prefix) QName(String namespaceURI, String localPart, String
		''' prefix) </seealso>
		Public Sub New(ByVal namespaceURI As String, ByVal localPart As String)
			Me.New(namespaceURI, localPart, javax.xml.XMLConstants.DEFAULT_NS_PREFIX)
		End Sub

		''' <summary>
		''' <p><code>QName</code> constructor specifying the Namespace URI,
		''' local part and prefix.</p>
		''' 
		''' <p>If the Namespace URI is <code>null</code>, it is set to
		''' {@link javax.xml.XMLConstants#NULL_NS_URI
		''' XMLConstants.NULL_NS_URI}.  This value represents no
		''' explicitly defined Namespace as defined by the <a
		''' href="http://www.w3.org/TR/REC-xml-names/#ns-qualnames">Namespaces
		''' in XML</a> specification.  This action preserves compatible
		''' behavior with QName 1.0.  Explicitly providing the {@link
		''' javax.xml.XMLConstants#NULL_NS_URI
		''' XMLConstants.NULL_NS_URI} value is the preferred coding
		''' style.</p>
		''' 
		''' <p>If the local part is <code>null</code> an
		''' <code>IllegalArgumentException</code> is thrown.
		''' A local part of "" is allowed to preserve
		''' compatible behavior with QName 1.0. </p>
		''' 
		''' <p>If the prefix is <code>null</code>, an
		''' <code>IllegalArgumentException</code> is thrown.  Use {@link
		''' javax.xml.XMLConstants#DEFAULT_NS_PREFIX
		''' XMLConstants.DEFAULT_NS_PREFIX} to explicitly indicate that no
		''' prefix is present or the prefix is not relevant.</p>
		''' 
		''' <p>The Namespace URI is not validated as a
		''' <a href="http://www.ietf.org/rfc/rfc2396.txt">URI reference</a>.
		''' The local part and prefix are not validated as a
		''' <a href="http://www.w3.org/TR/REC-xml-names/#NT-NCName">NCName</a>
		''' as specified in <a href="http://www.w3.org/TR/REC-xml-names/">Namespaces
		''' in XML</a>.</p>
		''' </summary>
		''' <param name="namespaceURI"> Namespace URI of the <code>QName</code> </param>
		''' <param name="localPart">    local part of the <code>QName</code> </param>
		''' <param name="prefix">       prefix of the <code>QName</code>
		''' </param>
		''' <exception cref="IllegalArgumentException"> When <code>localPart</code>
		'''   or <code>prefix</code> is <code>null</code> </exception>
		Public Sub New(ByVal namespaceURI As String, ByVal localPart As String, ByVal prefix As String)

			' map null Namespace URI to default
			' to preserve compatibility with QName 1.0
			If namespaceURI Is Nothing Then
				Me.namespaceURI = javax.xml.XMLConstants.NULL_NS_URI
			Else
				Me.namespaceURI = namespaceURI
			End If

			' local part is required.
			' "" is allowed to preserve compatibility with QName 1.0
			If localPart Is Nothing Then Throw New System.ArgumentException("local part cannot be ""null"" when creating a QName")
			Me.localPart = localPart

			' prefix is required
			If prefix Is Nothing Then Throw New System.ArgumentException("prefix cannot be ""null"" when creating a QName")
			Me.prefix = prefix
		End Sub

		''' <summary>
		''' <p><code>QName</code> constructor specifying the local part.</p>
		''' 
		''' <p>If the local part is <code>null</code> an
		''' <code>IllegalArgumentException</code> is thrown.
		''' A local part of "" is allowed to preserve
		''' compatible behavior with QName 1.0. </p>
		''' 
		''' <p>When using this constructor, the Namespace URI is set to
		''' {@link javax.xml.XMLConstants#NULL_NS_URI
		''' XMLConstants.NULL_NS_URI} and the prefix is set to {@link
		''' javax.xml.XMLConstants#DEFAULT_NS_PREFIX
		''' XMLConstants.DEFAULT_NS_PREFIX}.</p>
		''' 
		''' <p><em>In an XML context, all Element and Attribute names exist
		''' in the context of a Namespace.  Making this explicit during the
		''' construction of a <code>QName</code> helps prevent hard to
		''' diagnosis XML validity errors.  The constructors {@link
		''' #QName(String namespaceURI, String localPart) QName(String
		''' namespaceURI, String localPart)} and
		''' <seealso cref="#QName(String namespaceURI, String localPart, String prefix)"/>
		''' are preferred.</em></p>
		''' 
		''' <p>The local part is not validated as a
		''' <a href="http://www.w3.org/TR/REC-xml-names/#NT-NCName">NCName</a>
		''' as specified in <a href="http://www.w3.org/TR/REC-xml-names/">Namespaces
		''' in XML</a>.</p>
		''' </summary>
		''' <param name="localPart"> local part of the <code>QName</code>
		''' </param>
		''' <exception cref="IllegalArgumentException"> When <code>localPart</code> is
		'''   <code>null</code>
		''' </exception>
		''' <seealso cref= #QName(String namespaceURI, String localPart) QName(String
		''' namespaceURI, String localPart) </seealso>
		''' <seealso cref= #QName(String namespaceURI, String localPart, String
		''' prefix) QName(String namespaceURI, String localPart, String
		''' prefix) </seealso>
		Public Sub New(ByVal localPart As String)
			Me.New(javax.xml.XMLConstants.NULL_NS_URI, localPart, javax.xml.XMLConstants.DEFAULT_NS_PREFIX)
		End Sub

		''' <summary>
		''' <p>Get the Namespace URI of this <code>QName</code>.</p>
		''' </summary>
		''' <returns> Namespace URI of this <code>QName</code> </returns>
		Public Overridable Property namespaceURI As String
			Get
				Return namespaceURI
			End Get
		End Property

		''' <summary>
		''' <p>Get the local part of this <code>QName</code>.</p>
		''' </summary>
		'''  <returns> local part of this <code>QName</code> </returns>
		Public Overridable Property localPart As String
			Get
				Return localPart
			End Get
		End Property

		''' <summary>
		''' <p>Get the prefix of this <code>QName</code>.</p>
		''' 
		''' <p>The prefix assigned to a <code>QName</code> might
		''' <strong><em>NOT</em></strong> be valid in a different
		''' context. For example, a <code>QName</code> may be assigned a
		''' prefix in the context of parsing a document but that prefix may
		''' be invalid in the context of a different document.</p>
		''' </summary>
		'''  <returns> prefix of this <code>QName</code> </returns>
		Public Overridable Property prefix As String
			Get
				Return prefix
			End Get
		End Property

		''' <summary>
		''' <p>Test this <code>QName</code> for equality with another
		''' <code>Object</code>.</p>
		''' 
		''' <p>If the <code>Object</code> to be tested is not a
		''' <code>QName</code> or is <code>null</code>, then this method
		''' returns <code>false</code>.</p>
		''' 
		''' <p>Two <code>QName</code>s are considered equal if and only if
		''' both the Namespace URI and local part are equal. This method
		''' uses <code>String.equals()</code> to check equality of the
		''' Namespace URI and local part. The prefix is
		''' <strong><em>NOT</em></strong> used to determine equality.</p>
		''' 
		''' <p>This method satisfies the general contract of {@link
		''' java.lang.Object#equals(Object) Object.equals(Object)}</p>
		''' </summary>
		''' <param name="objectToTest"> the <code>Object</code> to test for
		''' equality with this <code>QName</code> </param>
		''' <returns> <code>true</code> if the given <code>Object</code> is
		''' equal to this <code>QName</code> else <code>false</code> </returns>
		Public NotOverridable Overrides Function Equals(ByVal objectToTest As Object) As Boolean
			If objectToTest Is Me Then Return True

			If objectToTest Is Nothing OrElse Not(TypeOf objectToTest Is QName) Then Return False

			Dim ___qName As QName = CType(objectToTest, QName)

			Return localPart.Equals(___qName.localPart) AndAlso namespaceURI.Equals(___qName.namespaceURI)
		End Function

		''' <summary>
		''' <p>Generate the hash code for this <code>QName</code>.</p>
		''' 
		''' <p>The hash code is calculated using both the Namespace URI and
		''' the local part of the <code>QName</code>.  The prefix is
		''' <strong><em>NOT</em></strong> used to calculate the hash
		''' code.</p>
		''' 
		''' <p>This method satisfies the general contract of {@link
		''' java.lang.Object#hashCode() Object.hashCode()}.</p>
		''' </summary>
		''' <returns> hash code for this <code>QName</code> <code>Object</code> </returns>
		Public NotOverridable Overrides Function GetHashCode() As Integer
			Return namespaceURI.GetHashCode() Xor localPart.GetHashCode()
		End Function

		''' <summary>
		''' <p><code>String</code> representation of this
		''' <code>QName</code>.</p>
		''' 
		''' <p>The commonly accepted way of representing a <code>QName</code>
		''' as a <code>String</code> was
		''' <a href="http://jclark.com/xml/xmlns.htm">defined</a>
		''' by James Clark.  Although this is not a <em>standard</em>
		''' specification, it is in common use, e.g. {@link
		''' javax.xml.transform.Transformer#setParameter(String name, Object value)}.
		''' This implementation represents a <code>QName</code> as:
		''' "{" + Namespace URI + "}" + local part.  If the Namespace URI
		''' <code>.equals(XMLConstants.NULL_NS_URI)</code>, only the
		''' local part is returned.  An appropriate use of this method is
		''' for debugging or logging for human consumption.</p>
		''' 
		''' <p>Note the prefix value is <strong><em>NOT</em></strong>
		''' returned as part of the <code>String</code> representation.</p>
		''' 
		''' <p>This method satisfies the general contract of {@link
		''' java.lang.Object#toString() Object.toString()}.</p>
		''' </summary>
		'''  <returns> <code>String</code> representation of this <code>QName</code> </returns>
		Public Overrides Function ToString() As String
			If namespaceURI.Equals(javax.xml.XMLConstants.NULL_NS_URI) Then
				Return localPart
			Else
				Return "{" & namespaceURI & "}" & localPart
			End If
		End Function

		''' <summary>
		''' <p><code>QName</code> derived from parsing the formatted
		''' <code>String</code>.</p>
		''' 
		''' <p>If the <code>String</code> is <code>null</code> or does not conform to
		''' <seealso cref="#toString() QName.toString()"/> formatting, an
		''' <code>IllegalArgumentException</code> is thrown.</p>
		''' 
		''' <p><em>The <code>String</code> <strong>MUST</strong> be in the
		''' form returned by <seealso cref="#toString() QName.toString()"/>.</em></p>
		''' 
		''' <p>The commonly accepted way of representing a <code>QName</code>
		''' as a <code>String</code> was
		''' <a href="http://jclark.com/xml/xmlns.htm">defined</a>
		''' by James Clark.  Although this is not a <em>standard</em>
		''' specification, it is in common use, e.g. {@link
		''' javax.xml.transform.Transformer#setParameter(String name, Object value)}.
		''' This implementation parses a <code>String</code> formatted
		''' as: "{" + Namespace URI + "}" + local part.  If the Namespace
		''' URI <code>.equals(XMLConstants.NULL_NS_URI)</code>, only the
		''' local part should be provided.</p>
		''' 
		''' <p>The prefix value <strong><em>CANNOT</em></strong> be
		''' represented in the <code>String</code> and will be set to
		''' {@link javax.xml.XMLConstants#DEFAULT_NS_PREFIX
		''' XMLConstants.DEFAULT_NS_PREFIX}.</p>
		''' 
		''' <p>This method does not do full validation of the resulting
		''' <code>QName</code>.
		''' <p>The Namespace URI is not validated as a
		''' <a href="http://www.ietf.org/rfc/rfc2396.txt">URI reference</a>.
		''' The local part is not validated as a
		''' <a href="http://www.w3.org/TR/REC-xml-names/#NT-NCName">NCName</a>
		''' as specified in
		''' <a href="http://www.w3.org/TR/REC-xml-names/">Namespaces in XML</a>.</p>
		''' </summary>
		''' <param name="qNameAsString"> <code>String</code> representation
		''' of the <code>QName</code>
		''' </param>
		''' <exception cref="IllegalArgumentException"> When <code>qNameAsString</code> is
		'''   <code>null</code> or malformed
		''' </exception>
		''' <returns> <code>QName</code> corresponding to the given <code>String</code> </returns>
		''' <seealso cref= #toString() QName.toString() </seealso>
		Public Shared Function valueOf(ByVal qNameAsString As String) As QName

			' null is not valid
			If qNameAsString Is Nothing Then Throw New System.ArgumentException("cannot create QName from ""null"" or """" String")

			' "" local part is valid to preserve compatible behavior with QName 1.0
			If qNameAsString.Length = 0 Then Return New QName(javax.xml.XMLConstants.NULL_NS_URI, qNameAsString, javax.xml.XMLConstants.DEFAULT_NS_PREFIX)

			' local part only?
			If qNameAsString.Chars(0) <> "{"c Then Return New QName(javax.xml.XMLConstants.NULL_NS_URI, qNameAsString, javax.xml.XMLConstants.DEFAULT_NS_PREFIX)

			' Namespace URI improperly specified?
			If qNameAsString.StartsWith("{" & javax.xml.XMLConstants.NULL_NS_URI & "}") Then Throw New System.ArgumentException("Namespace URI .equals(XMLConstants.NULL_NS_URI), " & ".equals(""" & javax.xml.XMLConstants.NULL_NS_URI & """), " & "only the local part, " & """" & qNameAsString.Substring(2 + javax.xml.XMLConstants.NULL_NS_URI.Length) & """, " & "should be provided.")

			' Namespace URI and local part specified
			Dim endOfNamespaceURI As Integer = qNameAsString.IndexOf("}"c)
			If endOfNamespaceURI = -1 Then Throw New System.ArgumentException("cannot create QName from """ & qNameAsString & """, missing closing ""}""")
			Return New QName(qNameAsString.Substring(1, endOfNamespaceURI - 1), qNameAsString.Substring(endOfNamespaceURI + 1), javax.xml.XMLConstants.DEFAULT_NS_PREFIX)
		End Function
	End Class

End Namespace
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

Namespace javax.naming.ldap


	''' <summary>
	''' Requests that the results of a search operation be returned by the LDAP
	''' server in batches of a specified size.
	''' The requestor controls the rate at which batches are returned by the rate
	''' at which it invokes search operations.
	''' <p>
	''' The following code sample shows how the class may be used:
	''' <pre>{@code
	''' 
	'''     // Open an LDAP association
	'''     LdapContext ctx = new InitialLdapContext();
	''' 
	'''     // Activate paged results
	'''     int pageSize = 20; // 20 entries per page
	'''     byte[] cookie = null;
	'''     int total;
	'''     ctx.setRequestControls(new Control[]{
	'''         new PagedResultsControl(pageSize, Control.CRITICAL) });
	''' 
	'''     do {
	'''         // Perform the search
	'''         NamingEnumeration results =
	'''             ctx.search("", "(objectclass=*)", new SearchControls());
	''' 
	'''         // Iterate over a batch of search results
	'''         while (results != null && results.hasMore()) {
	'''             // Display an entry
	'''             SearchResult entry = (SearchResult)results.next();
	'''             System.out.println(entry.getName());
	'''             System.out.println(entry.getAttributes());
	''' 
	'''             // Handle the entry's response controls (if any)
	'''             if (entry instanceof HasControls) {
	'''                 // ((HasControls)entry).getControls();
	'''             }
	'''         }
	'''         // Examine the paged results control response
	'''         Control[] controls = ctx.getResponseControls();
	'''         if (controls != null) {
	'''             for (int i = 0; i < controls.length; i++) {
	'''                 if (controls[i] instanceof PagedResultsResponseControl) {
	'''                     PagedResultsResponseControl prrc =
	'''                         (PagedResultsResponseControl)controls[i];
	'''                     total = prrc.getResultSize();
	'''                     cookie = prrc.getCookie();
	'''                 } else {
	'''                     // Handle other response controls (if any)
	'''                 }
	'''             }
	'''         }
	''' 
	'''         // Re-activate paged results
	'''         ctx.setRequestControls(new Control[]{
	'''             new PagedResultsControl(pageSize, cookie, Control.CRITICAL) });
	'''     } while (cookie != null);
	''' 
	'''     // Close the LDAP association
	'''     ctx.close();
	'''     ...
	''' 
	''' } </pre>
	''' <p>
	''' This class implements the LDAPv3 Control for paged-results as defined in
	''' <a href="http://www.ietf.org/rfc/rfc2696.txt">RFC 2696</a>.
	''' 
	''' The control's value has the following ASN.1 definition:
	''' <pre>{@code
	''' 
	'''     realSearchControlValue ::= SEQUENCE {
	'''         size      INTEGER (0..maxInt),
	'''                           -- requested page size from client
	'''                           -- result set size estimate from server
	'''         cookie    OCTET STRING
	'''     }
	''' 
	''' }</pre>
	''' 
	''' @since 1.5 </summary>
	''' <seealso cref= PagedResultsResponseControl
	''' @author Vincent Ryan </seealso>
	Public NotInheritable Class PagedResultsControl
		Inherits BasicControl

		''' <summary>
		''' The paged-results control's assigned object identifier
		''' is 1.2.840.113556.1.4.319.
		''' </summary>
		Public Const OID As String = "1.2.840.113556.1.4.319"

		Private Shared ReadOnly EMPTY_COOKIE As SByte() = New SByte(){}

		Private Const serialVersionUID As Long = 6684806685736844298L

		''' <summary>
		''' Constructs a control to set the number of entries to be returned per
		''' page of results.
		''' </summary>
		''' <param name="pageSize">        The number of entries to return in a page. </param>
		''' <param name="criticality">     If true then the server must honor the control
		'''                          and return search results as indicated by
		'''                          pageSize or refuse to perform the search.
		'''                          If false, then the server need not honor the
		'''                          control. </param>
		''' <exception cref="IOException">   If an error was encountered while encoding the
		'''                          supplied arguments into a control. </exception>
		Public Sub New(ByVal pageSize As Integer, ByVal criticality As Boolean)

			MyBase.New(OID, criticality, Nothing)
			value = encodedValuelue(pageSize, EMPTY_COOKIE)
		End Sub

		''' <summary>
		''' Constructs a control to set the number of entries to be returned per
		''' page of results. The cookie is provided by the server and may be
		''' obtained from the paged-results response control.
		''' <p>
		''' A sequence of paged-results can be abandoned by setting the pageSize
		''' to zero and setting the cookie to the last cookie received from the
		''' server.
		''' </summary>
		''' <param name="pageSize">        The number of entries to return in a page. </param>
		''' <param name="cookie">          A possibly null server-generated cookie. </param>
		''' <param name="criticality">     If true then the server must honor the control
		'''                          and return search results as indicated by
		'''                          pageSize or refuse to perform the search.
		'''                          If false, then the server need not honor the
		'''                          control. </param>
		''' <exception cref="IOException">   If an error was encountered while encoding the
		'''                          supplied arguments into a control. </exception>
		Public Sub New(ByVal pageSize As Integer, ByVal cookie As SByte(), ByVal criticality As Boolean)

			MyBase.New(OID, criticality, Nothing)
			If cookie Is Nothing Then cookie = EMPTY_COOKIE
			value = encodedValuelue(pageSize, cookie)
		End Sub

	'    
	'     * Encodes the paged-results control's value using ASN.1 BER.
	'     * The result includes the BER tag and length for the control's value but
	'     * does not include the control's object identifier and criticality setting.
	'     *
	'     * @param   pageSize        The number of entries to return in a page.
	'     * @param   cookie          A non-null server-generated cookie.
	'     * @return A possibly null byte array representing the ASN.1 BER encoded
	'     *         value of the LDAP paged-results control.
	'     * @exception IOException If a BER encoding error occurs.
	'     
		Private Function setEncodedValue(ByVal pageSize As Integer, ByVal cookie As SByte()) As SByte()

			' build the ASN.1 encoding
			Dim ber As New com.sun.jndi.ldap.BerEncoder(10 + cookie.Length)

			ber.beginSeq(com.sun.jndi.ldap.Ber.ASN_SEQUENCE Or com.sun.jndi.ldap.Ber.ASN_CONSTRUCTOR)
				ber.encodeInt(pageSize)
				ber.encodeOctetString(cookie, com.sun.jndi.ldap.Ber.ASN_OCTET_STR)
			ber.endSeq()

			Return ber.trimmedBuf
		End Function
	End Class

End Namespace
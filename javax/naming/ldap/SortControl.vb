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
	''' Requests that the results of a search operation be sorted by the LDAP server
	''' before being returned.
	''' The sort criteria are specified using an ordered list of one or more sort
	''' keys, with associated sort parameters.
	''' Search results are sorted at the LDAP server according to the parameters
	''' supplied in the sort control and then returned to the requestor. If sorting
	''' is not supported at the server (and the sort control is marked as critical)
	''' then the search operation is not performed and an error is returned.
	''' <p>
	''' The following code sample shows how the class may be used:
	''' <pre>{@code
	''' 
	'''     // Open an LDAP association
	'''     LdapContext ctx = new InitialLdapContext();
	''' 
	'''     // Activate sorting
	'''     String sortKey = "cn";
	'''     ctx.setRequestControls(new Control[]{
	'''         new SortControl(sortKey, Control.CRITICAL) });
	''' 
	'''     // Perform a search
	'''     NamingEnumeration results =
	'''         ctx.search("", "(objectclass=*)", new SearchControls());
	''' 
	'''     // Iterate over search results
	'''     while (results != null && results.hasMore()) {
	'''         // Display an entry
	'''         SearchResult entry = (SearchResult)results.next();
	'''         System.out.println(entry.getName());
	'''         System.out.println(entry.getAttributes());
	''' 
	'''         // Handle the entry's response controls (if any)
	'''         if (entry instanceof HasControls) {
	'''             // ((HasControls)entry).getControls();
	'''         }
	'''     }
	'''     // Examine the sort control response
	'''     Control[] controls = ctx.getResponseControls();
	'''     if (controls != null) {
	'''         for (int i = 0; i < controls.length; i++) {
	'''             if (controls[i] instanceof SortResponseControl) {
	'''                 SortResponseControl src = (SortResponseControl)controls[i];
	'''                 if (! src.isSorted()) {
	'''                     throw src.getException();
	'''                 }
	'''             } else {
	'''                 // Handle other response controls (if any)
	'''             }
	'''         }
	'''     }
	''' 
	'''     // Close the LDAP association
	'''     ctx.close();
	'''     ...
	''' 
	''' }</pre>
	''' <p>
	''' This class implements the LDAPv3 Request Control for server-side sorting
	''' as defined in
	''' <a href="http://www.ietf.org/rfc/rfc2891.txt">RFC 2891</a>.
	''' 
	''' The control's value has the following ASN.1 definition:
	''' <pre>
	''' 
	'''     SortKeyList ::= SEQUENCE OF SEQUENCE {
	'''         attributeType     AttributeDescription,
	'''         orderingRule  [0] MatchingRuleId OPTIONAL,
	'''         reverseOrder  [1] BOOLEAN DEFAULT FALSE }
	''' 
	''' </pre>
	''' 
	''' @since 1.5 </summary>
	''' <seealso cref= SortKey </seealso>
	''' <seealso cref= SortResponseControl
	''' @author Vincent Ryan </seealso>
	Public NotInheritable Class SortControl
		Inherits BasicControl

		''' <summary>
		''' The server-side sort control's assigned object identifier
		''' is 1.2.840.113556.1.4.473.
		''' </summary>
		Public Const OID As String = "1.2.840.113556.1.4.473"

		Private Const serialVersionUID As Long = -1965961680233330744L

		''' <summary>
		''' Constructs a control to sort on a single attribute in ascending order.
		''' Sorting will be performed using the ordering matching rule defined
		''' for use with the specified attribute.
		''' </summary>
		''' <param name="sortBy">  An attribute ID to sort by. </param>
		''' <param name="criticality">     If true then the server must honor the control
		'''                          and return the search results sorted as
		'''                          requested or refuse to perform the search.
		'''                          If false, then the server need not honor the
		'''                          control. </param>
		''' <exception cref="IOException"> If an error was encountered while encoding the
		'''                        supplied arguments into a control. </exception>
		Public Sub New(ByVal sortBy As String, ByVal criticality As Boolean)

			MyBase.New(OID, criticality, Nothing)
			MyBase.value = encodedValuelue(New SortKey(){ New SortKey(sortBy) })
		End Sub

		''' <summary>
		''' Constructs a control to sort on a list of attributes in ascending order.
		''' Sorting will be performed using the ordering matching rule defined
		''' for use with each of the specified attributes.
		''' </summary>
		''' <param name="sortBy">  A non-null list of attribute IDs to sort by.
		'''                  The list is in order of highest to lowest sort key
		'''                  precedence. </param>
		''' <param name="criticality">     If true then the server must honor the control
		'''                          and return the search results sorted as
		'''                          requested or refuse to perform the search.
		'''                          If false, then the server need not honor the
		'''                          control. </param>
		''' <exception cref="IOException"> If an error was encountered while encoding the
		'''                        supplied arguments into a control. </exception>
		Public Sub New(ByVal sortBy As String(), ByVal criticality As Boolean)

			MyBase.New(OID, criticality, Nothing)
			Dim sortKeys As SortKey() = New SortKey(sortBy.Length - 1){}
			For i As Integer = 0 To sortBy.Length - 1
				sortKeys(i) = New SortKey(sortBy(i))
			Next i
			MyBase.value = encodedValuelue(sortKeys)
		End Sub

		''' <summary>
		''' Constructs a control to sort on a list of sort keys.
		''' Each sort key specifies the sort order and ordering matching rule to use.
		''' </summary>
		''' <param name="sortBy">      A non-null list of keys to sort by.
		'''                      The list is in order of highest to lowest sort key
		'''                      precedence. </param>
		''' <param name="criticality">     If true then the server must honor the control
		'''                          and return the search results sorted as
		'''                          requested or refuse to perform the search.
		'''                          If false, then the server need not honor the
		'''                          control. </param>
		''' <exception cref="IOException"> If an error was encountered while encoding the
		'''                        supplied arguments into a control. </exception>
		Public Sub New(ByVal sortBy As SortKey(), ByVal criticality As Boolean)

			MyBase.New(OID, criticality, Nothing)
			MyBase.value = encodedValuelue(sortBy)
		End Sub

	'    
	'     * Encodes the sort control's value using ASN.1 BER.
	'     * The result includes the BER tag and length for the control's value but
	'     * does not include the control's object identifer and criticality setting.
	'     *
	'     * @param   sortKeys    A non-null list of keys to sort by.
	'     * @return A possibly null byte array representing the ASN.1 BER encoded
	'     *         value of the sort control.
	'     * @exception IOException If a BER encoding error occurs.
	'     
		Private Function setEncodedValue(ByVal sortKeys As SortKey()) As SByte()

			' build the ASN.1 BER encoding
			Dim ber As New com.sun.jndi.ldap.BerEncoder(30 * sortKeys.Length + 10)
			Dim matchingRule As String

			ber.beginSeq(com.sun.jndi.ldap.Ber.ASN_SEQUENCE Or com.sun.jndi.ldap.Ber.ASN_CONSTRUCTOR)

			For i As Integer = 0 To sortKeys.Length - 1
				ber.beginSeq(com.sun.jndi.ldap.Ber.ASN_SEQUENCE Or com.sun.jndi.ldap.Ber.ASN_CONSTRUCTOR)
				ber.encodeString(sortKeys(i).attributeID, True) ' v3

				matchingRule = sortKeys(i).matchingRuleID
				If matchingRule IsNot Nothing Then ber.encodeString(matchingRule, (com.sun.jndi.ldap.Ber.ASN_CONTEXT Or 0), True)
				If Not sortKeys(i).ascending Then ber.encodeBoolean(True, (com.sun.jndi.ldap.Ber.ASN_CONTEXT Or 1))
				ber.endSeq()
			Next i
			ber.endSeq()

			Return ber.trimmedBuf
		End Function
	End Class

End Namespace
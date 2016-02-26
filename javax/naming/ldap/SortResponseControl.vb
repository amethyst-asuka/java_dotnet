Imports javax.naming
Imports javax.naming.directory

'
' * Copyright (c) 2003, Oracle and/or its affiliates. All rights reserved.
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
	''' Indicates whether the requested sort of search results was successful or not.
	''' When the result code indicates success then the results have been sorted as
	''' requested. Otherwise the sort was unsuccessful and additional details
	''' regarding the cause of the error may have been provided by the server.
	''' <p>
	''' The code sample in <seealso cref="SortControl"/> shows how this class may be used.
	''' <p>
	''' This class implements the LDAPv3 Response Control for server-side sorting
	''' as defined in
	''' <a href="http://www.ietf.org/rfc/rfc2891.txt">RFC 2891</a>.
	''' 
	''' The control's value has the following ASN.1 definition:
	''' <pre>
	''' 
	'''     SortResult ::= SEQUENCE {
	'''        sortResult  ENUMERATED {
	'''            success                   (0), -- results are sorted
	'''            operationsError           (1), -- server internal failure
	'''            timeLimitExceeded         (3), -- timelimit reached before
	'''                                           -- sorting was completed
	'''            strongAuthRequired        (8), -- refused to return sorted
	'''                                           -- results via insecure
	'''                                           -- protocol
	'''            adminLimitExceeded       (11), -- too many matching entries
	'''                                           -- for the server to sort
	'''            noSuchAttribute          (16), -- unrecognized attribute
	'''                                           -- type in sort key
	'''            inappropriateMatching    (18), -- unrecognized or inappro-
	'''                                           -- priate matching rule in
	'''                                           -- sort key
	'''            insufficientAccessRights (50), -- refused to return sorted
	'''                                           -- results to this client
	'''            busy                     (51), -- too busy to process
	'''            unwillingToPerform       (53), -- unable to sort
	'''            other                    (80)
	'''            },
	'''      attributeType [0] AttributeType OPTIONAL }
	''' 
	''' </pre>
	''' 
	''' @since 1.5 </summary>
	''' <seealso cref= SortControl
	''' @author Vincent Ryan </seealso>
	Public NotInheritable Class SortResponseControl
		Inherits BasicControl

		''' <summary>
		''' The server-side sort response control's assigned object identifier
		''' is 1.2.840.113556.1.4.474.
		''' </summary>
		Public Const OID As String = "1.2.840.113556.1.4.474"

		Private Const serialVersionUID As Long = 5142939176006310877L

		''' <summary>
		''' The sort result code.
		''' 
		''' @serial
		''' </summary>
		Private _resultCode As Integer = 0

		''' <summary>
		''' The ID of the attribute that caused the sort to fail.
		''' 
		''' @serial
		''' </summary>
		Private badAttrId As String = Nothing

		''' <summary>
		''' Constructs a control to indicate the outcome of a sort request.
		''' </summary>
		''' <param name="id">              The control's object identifier string. </param>
		''' <param name="criticality">     The control's criticality. </param>
		''' <param name="value">           The control's ASN.1 BER encoded value.
		'''                          It is not cloned - any changes to value
		'''                          will affect the contents of the control. </param>
		''' <exception cref="IOException"> if an error is encountered
		'''                          while decoding the control's value. </exception>
		Public Sub New(ByVal id As String, ByVal criticality As Boolean, ByVal value As SByte())

			MyBase.New(id, criticality, value)

			' decode value
			Dim ber As New com.sun.jndi.ldap.BerDecoder(value, 0, value.Length)

			ber.parseSeq(Nothing)
			resultCode = ber.parseEnumeration()
			If (ber.bytesLeft() > 0) AndAlso (ber.peekByte() = com.sun.jndi.ldap.Ber.ASN_CONTEXT) Then badAttrId = ber.parseStringWithTag(com.sun.jndi.ldap.Ber.ASN_CONTEXT, True, Nothing)
		End Sub

		''' <summary>
		''' Determines if the search results have been successfully sorted.
		''' If an error occurred during sorting a NamingException is thrown.
		''' </summary>
		''' <returns>    true if the search results have been sorted. </returns>
	ReadOnly	Public Property sorted As Boolean
			Get
				Return (_resultCode = 0) ' a result code of zero indicates success
			End Get
		End Property

		''' <summary>
		''' Retrieves the LDAP result code of the sort operation.
		''' </summary>
		''' <returns>    The result code. A zero value indicates success. </returns>
	ReadOnly	Public Property resultCode As Integer
			Get
				Return _resultCode
			End Get
		End Property

		''' <summary>
		''' Retrieves the ID of the attribute that caused the sort to fail.
		''' Returns null if no ID was returned by the server.
		''' </summary>
		''' <returns> The possibly null ID of the bad attribute. </returns>
	ReadOnly	Public Property attributeID As String
			Get
				Return badAttrId
			End Get
		End Property

		''' <summary>
		''' Retrieves the NamingException appropriate for the result code.
		''' </summary>
		''' <returns> A NamingException or null if the result code indicates
		'''         success. </returns>
	ReadOnly	Public Property exception As NamingException
			Get
    
				Return com.sun.jndi.ldap.LdapCtx.mapErrorCode(resultCode, Nothing)
			End Get
		End Property
	End Class

End Namespace
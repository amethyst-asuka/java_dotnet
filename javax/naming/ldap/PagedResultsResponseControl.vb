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
	''' Indicates the end of a batch of search results.
	''' Contains an estimate of the total number of entries in the result set
	''' and an opaque cookie. The cookie must be supplied to the next search
	''' operation in order to get the next batch of results.
	''' <p>
	''' The code sample in <seealso cref="PagedResultsControl"/> shows how this class may
	''' be used.
	''' <p>
	''' This class implements the LDAPv3 Response Control for
	''' paged-results as defined in
	''' <a href="http://www.ietf.org/rfc/rfc2696">RFC 2696</a>.
	''' 
	''' The control's value has the following ASN.1 definition:
	''' <pre>
	''' 
	'''     realSearchControlValue ::= SEQUENCE {
	'''         size      INTEGER (0..maxInt),
	'''                           -- requested page size from client
	'''                           -- result set size estimate from server
	'''         cookie    OCTET STRING
	'''     }
	''' 
	''' </pre>
	''' 
	''' @since 1.5 </summary>
	''' <seealso cref= PagedResultsControl
	''' @author Vincent Ryan </seealso>
	Public NotInheritable Class PagedResultsResponseControl
		Inherits BasicControl

		''' <summary>
		''' The paged-results response control's assigned object identifier
		''' is 1.2.840.113556.1.4.319.
		''' </summary>
		Public Const OID As String = "1.2.840.113556.1.4.319"

		Private Const serialVersionUID As Long = -8819778744844514666L

		''' <summary>
		''' An estimate of the number of entries in the search result.
		''' 
		''' @serial
		''' </summary>
		Private resultSize As Integer

		''' <summary>
		''' A server-generated cookie.
		''' 
		''' @serial
		''' </summary>
		Private cookie As SByte()

		''' <summary>
		''' Constructs a paged-results response control.
		''' </summary>
		''' <param name="id">              The control's object identifier string. </param>
		''' <param name="criticality">     The control's criticality. </param>
		''' <param name="value">           The control's ASN.1 BER encoded value.
		'''                          It is not cloned - any changes to value
		'''                          will affect the contents of the control. </param>
		''' <exception cref="IOException">   If an error was encountered while decoding
		'''                          the control's value. </exception>
		Public Sub New(ByVal id As String, ByVal criticality As Boolean, ByVal value As SByte())

			MyBase.New(id, criticality, value)

			' decode value
			Dim ber As New com.sun.jndi.ldap.BerDecoder(value, 0, value.Length)

			ber.parseSeq(Nothing)
			resultSize = ber.parseInt()
			cookie = ber.parseOctetString(com.sun.jndi.ldap.Ber.ASN_OCTET_STR, Nothing)
		End Sub

		''' <summary>
		''' Retrieves (an estimate of) the number of entries in the search result.
		''' </summary>
		''' <returns> The number of entries in the search result, or zero if unknown. </returns>
		Public Property resultSize As Integer
			Get
				Return resultSize
			End Get
		End Property

		''' <summary>
		''' Retrieves the server-generated cookie. Null is returned when there are
		''' no more entries for the server to return.
		''' </summary>
		''' <returns> A possibly null server-generated cookie. It is not cloned - any
		'''         changes to the cookie will update the control's state and thus
		'''         are not recommended. </returns>
		Public Property cookie As SByte()
			Get
				If cookie.Length = 0 Then
					Return Nothing
				Else
					Return cookie
				End If
			End Get
		End Property
	End Class

End Namespace
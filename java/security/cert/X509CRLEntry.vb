Imports System

'
' * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.security.cert



	''' <summary>
	''' <p>Abstract class for a revoked certificate in a CRL (Certificate
	''' Revocation List).
	''' 
	''' The ASN.1 definition for <em>revokedCertificates</em> is:
	''' <pre>
	''' revokedCertificates    SEQUENCE OF SEQUENCE  {
	'''     userCertificate    CertificateSerialNumber,
	'''     revocationDate     ChoiceOfTime,
	'''     crlEntryExtensions Extensions OPTIONAL
	'''                        -- if present, must be v2
	''' }  OPTIONAL
	''' 
	''' CertificateSerialNumber  ::=  INTEGER
	''' 
	''' Extensions  ::=  SEQUENCE SIZE (1..MAX) OF Extension
	''' 
	''' Extension  ::=  SEQUENCE  {
	'''     extnId        OBJECT IDENTIFIER,
	'''     critical      BOOLEAN DEFAULT FALSE,
	'''     extnValue     OCTET STRING
	'''                   -- contains a DER encoding of a value
	'''                   -- of the type registered for use with
	'''                   -- the extnId object identifier value
	''' }
	''' </pre>
	''' </summary>
	''' <seealso cref= X509CRL </seealso>
	''' <seealso cref= X509Extension
	''' 
	''' @author Hemma Prafullchandra </seealso>

	Public MustInherit Class X509CRLEntry
		Implements X509Extension

			Public MustOverride Function getExtensionValue(  oid As String) As SByte() Implements X509Extension.getExtensionValue
			Public MustOverride ReadOnly Property nonCriticalExtensionOIDs As java.util.Set(Of String) Implements X509Extension.getNonCriticalExtensionOIDs
			Public MustOverride ReadOnly Property criticalExtensionOIDs As java.util.Set(Of String) Implements X509Extension.getCriticalExtensionOIDs
			Public MustOverride Function hasUnsupportedCriticalExtension() As Boolean Implements X509Extension.hasUnsupportedCriticalExtension

		''' <summary>
		''' Compares this CRL entry for equality with the given
		''' object. If the {@code other} object is an
		''' {@code instanceof} {@code X509CRLEntry}, then
		''' its encoded form (the inner SEQUENCE) is retrieved and compared
		''' with the encoded form of this CRL entry.
		''' </summary>
		''' <param name="other"> the object to test for equality with this CRL entry. </param>
		''' <returns> true iff the encoded forms of the two CRL entries
		''' match, false otherwise. </returns>
		Public Overrides Function Equals(  other As Object) As Boolean
			If Me Is other Then Return True
			If Not(TypeOf other Is X509CRLEntry) Then Return False
			Try
				Dim thisCRLEntry As SByte() = Me.encoded
				Dim otherCRLEntry As SByte() = CType(other, X509CRLEntry).encoded

				If thisCRLEntry.Length <> otherCRLEntry.Length Then Return False
				For i As Integer = 0 To thisCRLEntry.Length - 1
					 If thisCRLEntry(i) <> otherCRLEntry(i) Then Return False
				Next i
			Catch ce As CRLException
				Return False
			End Try
			Return True
		End Function

		''' <summary>
		''' Returns a hashcode value for this CRL entry from its
		''' encoded form.
		''' </summary>
		''' <returns> the hashcode value. </returns>
		Public Overrides Function GetHashCode() As Integer
			Dim retval As Integer = 0
			Try
				Dim entryData As SByte() = Me.encoded
				For i As Integer = 1 To entryData.Length - 1
					 retval += entryData(i) * i
				Next i

			Catch ce As CRLException
				Return (retval)
			End Try
			Return (retval)
		End Function

		''' <summary>
		''' Returns the ASN.1 DER-encoded form of this CRL Entry,
		''' that is the inner SEQUENCE.
		''' </summary>
		''' <returns> the encoded form of this certificate </returns>
		''' <exception cref="CRLException"> if an encoding error occurs. </exception>
		Public MustOverride ReadOnly Property encoded As SByte()

		''' <summary>
		''' Gets the serial number from this X509CRLEntry,
		''' the <em>userCertificate</em>.
		''' </summary>
		''' <returns> the serial number. </returns>
		Public MustOverride ReadOnly Property serialNumber As System.Numerics.BigInteger

		''' <summary>
		''' Get the issuer of the X509Certificate described by this entry. If
		''' the certificate issuer is also the CRL issuer, this method returns
		''' null.
		''' 
		''' <p>This method is used with indirect CRLs. The default implementation
		''' always returns null. Subclasses that wish to support indirect CRLs
		''' should override it.
		''' </summary>
		''' <returns> the issuer of the X509Certificate described by this entry
		''' or null if it is issued by the CRL issuer.
		''' 
		''' @since 1.5 </returns>
		Public Overridable Property certificateIssuer As javax.security.auth.x500.X500Principal
			Get
				Return Nothing
			End Get
		End Property

		''' <summary>
		''' Gets the revocation date from this X509CRLEntry,
		''' the <em>revocationDate</em>.
		''' </summary>
		''' <returns> the revocation date. </returns>
		Public MustOverride ReadOnly Property revocationDate As DateTime?

		''' <summary>
		''' Returns true if this CRL entry has extensions.
		''' </summary>
		''' <returns> true if this entry has extensions, false otherwise. </returns>
		Public MustOverride Function hasExtensions() As Boolean

		''' <summary>
		''' Returns a string representation of this CRL entry.
		''' </summary>
		''' <returns> a string representation of this CRL entry. </returns>
		Public MustOverride Function ToString() As String

		''' <summary>
		''' Returns the reason the certificate has been revoked, as specified
		''' in the Reason Code extension of this CRL entry.
		''' </summary>
		''' <returns> the reason the certificate has been revoked, or
		'''    {@code null} if this CRL entry does not have
		'''    a Reason Code extension
		''' @since 1.7 </returns>
		Public Overridable Property revocationReason As CRLReason
			Get
				If Not hasExtensions() Then Return Nothing
				Return sun.security.x509.X509CRLEntryImpl.getRevocationReason(Me)
			End Get
		End Property
	End Class

End Namespace
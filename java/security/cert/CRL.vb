'
' * Copyright (c) 1998, 2006, Oracle and/or its affiliates. All rights reserved.
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
	''' This class is an abstraction of certificate revocation lists (CRLs) that
	''' have different formats but important common uses. For example, all CRLs
	''' share the functionality of listing revoked certificates, and can be queried
	''' on whether or not they list a given certificate.
	''' <p>
	''' Specialized CRL types can be defined by subclassing off of this abstract
	''' class.
	''' 
	''' @author Hemma Prafullchandra
	''' 
	''' </summary>
	''' <seealso cref= X509CRL </seealso>
	''' <seealso cref= CertificateFactory
	''' 
	''' @since 1.2 </seealso>

	Public MustInherit Class CRL

		' the CRL type
		Private type As String

		''' <summary>
		''' Creates a CRL of the specified type.
		''' </summary>
		''' <param name="type"> the standard name of the CRL type.
		''' See Appendix A in the <a href=
		''' "../../../../technotes/guides/security/crypto/CryptoSpec.html#AppA">
		''' Java Cryptography Architecture API Specification &amp; Reference </a>
		''' for information about standard CRL types. </param>
		Protected Friend Sub New(ByVal type As String)
			Me.type = type
		End Sub

		''' <summary>
		''' Returns the type of this CRL.
		''' </summary>
		''' <returns> the type of this CRL. </returns>
		Public Property type As String
			Get
				Return Me.type
			End Get
		End Property

		''' <summary>
		''' Returns a string representation of this CRL.
		''' </summary>
		''' <returns> a string representation of this CRL. </returns>
		Public MustOverride Function ToString() As String

		''' <summary>
		''' Checks whether the given certificate is on this CRL.
		''' </summary>
		''' <param name="cert"> the certificate to check for. </param>
		''' <returns> true if the given certificate is on this CRL,
		''' false otherwise. </returns>
		Public MustOverride Function isRevoked(ByVal cert As Certificate) As Boolean
	End Class

End Namespace
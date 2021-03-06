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

Namespace java.security.spec

	''' <summary>
	''' This class represents the ASN.1 encoding of a private key,
	''' encoded according to the ASN.1 type {@code PrivateKeyInfo}.
	''' The {@code PrivateKeyInfo} syntax is defined in the PKCS#8 standard
	''' as follows:
	''' 
	''' <pre>
	''' PrivateKeyInfo ::= SEQUENCE {
	'''   version Version,
	'''   privateKeyAlgorithm PrivateKeyAlgorithmIdentifier,
	'''   privateKey PrivateKey,
	'''   attributes [0] IMPLICIT Attributes OPTIONAL }
	''' 
	''' Version ::= INTEGER
	''' 
	''' PrivateKeyAlgorithmIdentifier ::= AlgorithmIdentifier
	''' 
	''' PrivateKey ::= OCTET STRING
	''' 
	''' Attributes ::= SET OF Attribute
	''' </pre>
	''' 
	''' @author Jan Luehe
	''' 
	''' </summary>
	''' <seealso cref= java.security.Key </seealso>
	''' <seealso cref= java.security.KeyFactory </seealso>
	''' <seealso cref= KeySpec </seealso>
	''' <seealso cref= EncodedKeySpec </seealso>
	''' <seealso cref= X509EncodedKeySpec
	''' 
	''' @since 1.2 </seealso>

	Public Class PKCS8EncodedKeySpec
		Inherits EncodedKeySpec

		''' <summary>
		''' Creates a new PKCS8EncodedKeySpec with the given encoded key.
		''' </summary>
		''' <param name="encodedKey"> the key, which is assumed to be
		''' encoded according to the PKCS #8 standard. The contents of
		''' the array are copied to protect against subsequent modification. </param>
		''' <exception cref="NullPointerException"> if {@code encodedKey}
		''' is null. </exception>
		Public Sub New(  encodedKey As SByte())
			MyBase.New(encodedKey)
		End Sub

		''' <summary>
		''' Returns the key bytes, encoded according to the PKCS #8 standard.
		''' </summary>
		''' <returns> the PKCS #8 encoding of the key. Returns a new array
		''' each time this method is called. </returns>
		Public  Overrides ReadOnly Property  encoded As SByte()
			Get
				Return MyBase.encoded
			End Get
		End Property

		''' <summary>
		''' Returns the name of the encoding format associated with this
		''' key specification.
		''' </summary>
		''' <returns> the string {@code "PKCS#8"}. </returns>
		Public Property NotOverridable Overrides format As String
			Get
				Return "PKCS#8"
			End Get
		End Property
	End Class

End Namespace
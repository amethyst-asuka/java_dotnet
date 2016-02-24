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
	''' This class represents the ASN.1 encoding of a public key,
	''' encoded according to the ASN.1 type {@code SubjectPublicKeyInfo}.
	''' The {@code SubjectPublicKeyInfo} syntax is defined in the X.509
	''' standard as follows:
	''' 
	''' <pre>
	''' SubjectPublicKeyInfo ::= SEQUENCE {
	'''   algorithm AlgorithmIdentifier,
	'''   subjectPublicKey BIT STRING }
	''' </pre>
	''' 
	''' @author Jan Luehe
	''' 
	''' </summary>
	''' <seealso cref= java.security.Key </seealso>
	''' <seealso cref= java.security.KeyFactory </seealso>
	''' <seealso cref= KeySpec </seealso>
	''' <seealso cref= EncodedKeySpec </seealso>
	''' <seealso cref= PKCS8EncodedKeySpec
	''' 
	''' @since 1.2 </seealso>

	Public Class X509EncodedKeySpec
		Inherits EncodedKeySpec

		''' <summary>
		''' Creates a new X509EncodedKeySpec with the given encoded key.
		''' </summary>
		''' <param name="encodedKey"> the key, which is assumed to be
		''' encoded according to the X.509 standard. The contents of the
		''' array are copied to protect against subsequent modification. </param>
		''' <exception cref="NullPointerException"> if {@code encodedKey}
		''' is null. </exception>
		Public Sub New(ByVal encodedKey As SByte())
			MyBase.New(encodedKey)
		End Sub

		''' <summary>
		''' Returns the key bytes, encoded according to the X.509 standard.
		''' </summary>
		''' <returns> the X.509 encoding of the key. Returns a new array
		''' each time this method is called. </returns>
		Public Property Overrides encoded As SByte()
			Get
				Return MyBase.encoded
			End Get
		End Property

		''' <summary>
		''' Returns the name of the encoding format associated with this
		''' key specification.
		''' </summary>
		''' <returns> the string {@code "X.509"}. </returns>
		Public Property NotOverridable Overrides format As String
			Get
				Return "X.509"
			End Get
		End Property
	End Class

End Namespace
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

Namespace java.security.spec


	''' <summary>
	''' This class specifies the set of parameters used with mask generation
	''' function MGF1 in OAEP Padding and RSA-PSS signature scheme, as
	''' defined in the
	''' <a href="http://www.ietf.org/rfc/rfc3447.txt">PKCS #1 v2.1</a>
	''' standard.
	''' 
	''' <p>Its ASN.1 definition in PKCS#1 standard is described below:
	''' <pre>
	''' MGF1Parameters ::= OAEP-PSSDigestAlgorthms
	''' </pre>
	''' where
	''' <pre>
	''' OAEP-PSSDigestAlgorithms    ALGORITHM-IDENTIFIER ::= {
	'''   { OID id-sha1 PARAMETERS NULL   }|
	'''   { OID id-sha224 PARAMETERS NULL   }|
	'''   { OID id-sha256 PARAMETERS NULL }|
	'''   { OID id-sha384 PARAMETERS NULL }|
	'''   { OID id-sha512 PARAMETERS NULL },
	'''   ...  -- Allows for future expansion --
	''' }
	''' </pre> </summary>
	''' <seealso cref= PSSParameterSpec </seealso>
	''' <seealso cref= javax.crypto.spec.OAEPParameterSpec
	''' 
	''' @author Valerie Peng
	''' 
	''' @since 1.5 </seealso>
	Public Class MGF1ParameterSpec
		Implements java.security.spec.AlgorithmParameterSpec

		''' <summary>
		''' The MGF1ParameterSpec which uses "SHA-1" message digest.
		''' </summary>
		Public Shared ReadOnly SHA1 As New MGF1ParameterSpec("SHA-1")
		''' <summary>
		''' The MGF1ParameterSpec which uses "SHA-224" message digest.
		''' </summary>
		Public Shared ReadOnly SHA224 As New MGF1ParameterSpec("SHA-224")
		''' <summary>
		''' The MGF1ParameterSpec which uses "SHA-256" message digest.
		''' </summary>
		Public Shared ReadOnly SHA256 As New MGF1ParameterSpec("SHA-256")
		''' <summary>
		''' The MGF1ParameterSpec which uses "SHA-384" message digest.
		''' </summary>
		Public Shared ReadOnly SHA384 As New MGF1ParameterSpec("SHA-384")
		''' <summary>
		''' The MGF1ParameterSpec which uses SHA-512 message digest.
		''' </summary>
		Public Shared ReadOnly SHA512 As New MGF1ParameterSpec("SHA-512")

		Private mdName As String

		''' <summary>
		''' Constructs a parameter set for mask generation function MGF1
		''' as defined in the PKCS #1 standard.
		''' </summary>
		''' <param name="mdName"> the algorithm name for the message digest
		''' used in this mask generation function MGF1. </param>
		''' <exception cref="NullPointerException"> if {@code mdName} is null. </exception>
		Public Sub New(  mdName As String)
			If mdName Is Nothing Then Throw New NullPointerException("digest algorithm is null")
			Me.mdName = mdName
		End Sub

		''' <summary>
		''' Returns the algorithm name of the message digest used by the mask
		''' generation function.
		''' </summary>
		''' <returns> the algorithm name of the message digest. </returns>
		Public Overridable Property digestAlgorithm As String
			Get
				Return mdName
			End Get
		End Property
	End Class

End Namespace
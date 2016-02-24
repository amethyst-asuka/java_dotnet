'
' * Copyright (c) 2001, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' This class specifies a parameter spec for RSA-PSS signature scheme,
	''' as defined in the
	''' <a href="http://www.ietf.org/rfc/rfc3447.txt">PKCS#1 v2.1</a>
	''' standard.
	''' 
	''' <p>Its ASN.1 definition in PKCS#1 standard is described below:
	''' <pre>
	''' RSASSA-PSS-params ::= SEQUENCE {
	'''   hashAlgorithm      [0] OAEP-PSSDigestAlgorithms  DEFAULT sha1,
	'''   maskGenAlgorithm   [1] PKCS1MGFAlgorithms  DEFAULT mgf1SHA1,
	'''   saltLength         [2] INTEGER  DEFAULT 20,
	'''   trailerField       [3] INTEGER  DEFAULT 1
	''' }
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
	''' 
	''' PKCS1MGFAlgorithms    ALGORITHM-IDENTIFIER ::= {
	'''   { OID id-mgf1 PARAMETERS OAEP-PSSDigestAlgorithms },
	'''   ...  -- Allows for future expansion --
	''' }
	''' </pre>
	''' <p>Note: the PSSParameterSpec.DEFAULT uses the following:
	'''     message digest  -- "SHA-1"
	'''     mask generation function (mgf) -- "MGF1"
	'''     parameters for mgf -- MGF1ParameterSpec.SHA1
	'''     SaltLength   -- 20
	'''     TrailerField -- 1
	''' </summary>
	''' <seealso cref= MGF1ParameterSpec </seealso>
	''' <seealso cref= AlgorithmParameterSpec </seealso>
	''' <seealso cref= java.security.Signature
	''' 
	''' @author Valerie Peng
	''' 
	''' 
	''' @since 1.4 </seealso>

	Public Class PSSParameterSpec
		Implements AlgorithmParameterSpec

		Private mdName As String = "SHA-1"
		Private mgfName As String = "MGF1"
		Private mgfSpec As AlgorithmParameterSpec = java.security.spec.MGF1ParameterSpec.SHA1
		Private saltLen As Integer = 20
		Private trailerField As Integer = 1

		''' <summary>
		''' The PSS parameter set with all default values.
		''' @since 1.5
		''' </summary>
		Public Shared ReadOnly [DEFAULT] As New PSSParameterSpec

		''' <summary>
		''' Constructs a new {@code PSSParameterSpec} as defined in
		''' the PKCS #1 standard using the default values.
		''' </summary>
		Private Sub New()
		End Sub

		''' <summary>
		''' Creates a new {@code PSSParameterSpec} as defined in
		''' the PKCS #1 standard using the specified message digest,
		''' mask generation function, parameters for mask generation
		''' function, salt length, and trailer field values.
		''' </summary>
		''' <param name="mdName"> the algorithm name of the hash function. </param>
		''' <param name="mgfName"> the algorithm name of the mask generation
		''' function. </param>
		''' <param name="mgfSpec"> the parameters for the mask generation
		''' function. If null is specified, null will be returned by
		''' getMGFParameters(). </param>
		''' <param name="saltLen"> the length of salt. </param>
		''' <param name="trailerField"> the value of the trailer field. </param>
		''' <exception cref="NullPointerException"> if {@code mdName},
		''' or {@code mgfName} is null. </exception>
		''' <exception cref="IllegalArgumentException"> if {@code saltLen}
		''' or {@code trailerField} is less than 0.
		''' @since 1.5 </exception>
		Public Sub New(ByVal mdName As String, ByVal mgfName As String, ByVal mgfSpec As AlgorithmParameterSpec, ByVal saltLen As Integer, ByVal trailerField As Integer)
			If mdName Is Nothing Then Throw New NullPointerException("digest algorithm is null")
			If mgfName Is Nothing Then Throw New NullPointerException("mask generation function " & "algorithm is null")
			If saltLen < 0 Then Throw New IllegalArgumentException("negative saltLen value: " & saltLen)
			If trailerField < 0 Then Throw New IllegalArgumentException("negative trailerField: " & trailerField)
			Me.mdName = mdName
			Me.mgfName = mgfName
			Me.mgfSpec = mgfSpec
			Me.saltLen = saltLen
			Me.trailerField = trailerField
		End Sub

		''' <summary>
		''' Creates a new {@code PSSParameterSpec}
		''' using the specified salt length and other default values as
		''' defined in PKCS#1.
		''' </summary>
		''' <param name="saltLen"> the length of salt in bits to be used in PKCS#1
		''' PSS encoding. </param>
		''' <exception cref="IllegalArgumentException"> if {@code saltLen} is
		''' less than 0. </exception>
		Public Sub New(ByVal saltLen As Integer)
			If saltLen < 0 Then Throw New IllegalArgumentException("negative saltLen value: " & saltLen)
			Me.saltLen = saltLen
		End Sub

		''' <summary>
		''' Returns the message digest algorithm name.
		''' </summary>
		''' <returns> the message digest algorithm name.
		''' @since 1.5 </returns>
		Public Overridable Property digestAlgorithm As String
			Get
				Return mdName
			End Get
		End Property

		''' <summary>
		''' Returns the mask generation function algorithm name.
		''' </summary>
		''' <returns> the mask generation function algorithm name.
		''' 
		''' @since 1.5 </returns>
		Public Overridable Property mGFAlgorithm As String
			Get
				Return mgfName
			End Get
		End Property

		''' <summary>
		''' Returns the parameters for the mask generation function.
		''' </summary>
		''' <returns> the parameters for the mask generation function.
		''' @since 1.5 </returns>
		Public Overridable Property mGFParameters As AlgorithmParameterSpec
			Get
				Return mgfSpec
			End Get
		End Property

		''' <summary>
		''' Returns the salt length in bits.
		''' </summary>
		''' <returns> the salt length. </returns>
		Public Overridable Property saltLength As Integer
			Get
				Return saltLen
			End Get
		End Property

		''' <summary>
		''' Returns the value for the trailer field, i.e. bc in PKCS#1 v2.1.
		''' </summary>
		''' <returns> the value for the trailer field, i.e. bc in PKCS#1 v2.1.
		''' @since 1.5 </returns>
		Public Overridable Property trailerField As Integer
			Get
				Return trailerField
			End Get
		End Property
	End Class

End Namespace
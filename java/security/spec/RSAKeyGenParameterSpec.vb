'
' * Copyright (c) 1999, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' This class specifies the set of parameters used to generate an RSA
	''' key pair.
	''' 
	''' @author Jan Luehe
	''' </summary>
	''' <seealso cref= java.security.KeyPairGenerator#initialize(java.security.spec.AlgorithmParameterSpec)
	''' 
	''' @since 1.3 </seealso>

	Public Class RSAKeyGenParameterSpec
		Implements java.security.spec.AlgorithmParameterSpec

		Private keysize As Integer
		Private publicExponent As System.Numerics.BigInteger

		''' <summary>
		''' The public-exponent value F0 = 3.
		''' </summary>
		Public Shared ReadOnly F0 As System.Numerics.BigInteger = System.Numerics.Big java.lang.[Integer].valueOf(3)

		''' <summary>
		''' The public exponent-value F4 = 65537.
		''' </summary>
		Public Shared ReadOnly F4 As System.Numerics.BigInteger = System.Numerics.Big java.lang.[Integer].valueOf(65537)

		''' <summary>
		''' Constructs a new {@code RSAParameterSpec} object from the
		''' given keysize and public-exponent value.
		''' </summary>
		''' <param name="keysize"> the modulus size (specified in number of bits) </param>
		''' <param name="publicExponent"> the public exponent </param>
		Public Sub New(  keysize As Integer,   publicExponent As System.Numerics.BigInteger)
			Me.keysize = keysize
			Me.publicExponent = publicExponent
		End Sub

		''' <summary>
		''' Returns the keysize.
		''' </summary>
		''' <returns> the keysize. </returns>
		Public Overridable Property keysize As Integer
			Get
				Return keysize
			End Get
		End Property

		''' <summary>
		''' Returns the public-exponent value.
		''' </summary>
		''' <returns> the public-exponent value. </returns>
		Public Overridable Property publicExponent As System.Numerics.BigInteger
			Get
				Return publicExponent
			End Get
		End Property
	End Class

End Namespace
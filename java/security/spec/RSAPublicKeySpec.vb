'
' * Copyright (c) 1998, 2001, Oracle and/or its affiliates. All rights reserved.
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
	''' This class specifies an RSA public key.
	''' 
	''' @author Jan Luehe
	''' 
	''' </summary>
	''' <seealso cref= java.security.Key </seealso>
	''' <seealso cref= java.security.KeyFactory </seealso>
	''' <seealso cref= KeySpec </seealso>
	''' <seealso cref= X509EncodedKeySpec </seealso>
	''' <seealso cref= RSAPrivateKeySpec </seealso>
	''' <seealso cref= RSAPrivateCrtKeySpec </seealso>

	Public Class RSAPublicKeySpec
		Implements KeySpec

		Private modulus As System.Numerics.BigInteger
		Private publicExponent As System.Numerics.BigInteger

		''' <summary>
		''' Creates a new RSAPublicKeySpec.
		''' </summary>
		''' <param name="modulus"> the modulus </param>
		''' <param name="publicExponent"> the public exponent </param>
		Public Sub New(  modulus As System.Numerics.BigInteger,   publicExponent As System.Numerics.BigInteger)
			Me.modulus = modulus
			Me.publicExponent = publicExponent
		End Sub

		''' <summary>
		''' Returns the modulus.
		''' </summary>
		''' <returns> the modulus </returns>
		Public Overridable Property modulus As System.Numerics.BigInteger
			Get
				Return Me.modulus
			End Get
		End Property

		''' <summary>
		''' Returns the public exponent.
		''' </summary>
		''' <returns> the public exponent </returns>
		Public Overridable Property publicExponent As System.Numerics.BigInteger
			Get
				Return Me.publicExponent
			End Get
		End Property
	End Class

End Namespace
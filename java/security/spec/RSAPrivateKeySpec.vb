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
	''' This class specifies an RSA private key.
	''' 
	''' @author Jan Luehe
	''' 
	''' </summary>
	''' <seealso cref= java.security.Key </seealso>
	''' <seealso cref= java.security.KeyFactory </seealso>
	''' <seealso cref= KeySpec </seealso>
	''' <seealso cref= PKCS8EncodedKeySpec </seealso>
	''' <seealso cref= RSAPublicKeySpec </seealso>
	''' <seealso cref= RSAPrivateCrtKeySpec </seealso>

	Public Class RSAPrivateKeySpec
		Implements KeySpec

		Private modulus As System.Numerics.BigInteger
		Private privateExponent As System.Numerics.BigInteger

		''' <summary>
		''' Creates a new RSAPrivateKeySpec.
		''' </summary>
		''' <param name="modulus"> the modulus </param>
		''' <param name="privateExponent"> the private exponent </param>
		Public Sub New(ByVal modulus As System.Numerics.BigInteger, ByVal privateExponent As System.Numerics.BigInteger)
			Me.modulus = modulus
			Me.privateExponent = privateExponent
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
		''' Returns the private exponent.
		''' </summary>
		''' <returns> the private exponent </returns>
		Public Overridable Property privateExponent As System.Numerics.BigInteger
			Get
				Return Me.privateExponent
			End Get
		End Property
	End Class

End Namespace
'
' * Copyright (c) 1998, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' This class specifies an RSA private key, as defined in the PKCS#1
	''' standard, using the Chinese Remainder Theorem (CRT) information values for
	''' efficiency.
	''' 
	''' @author Jan Luehe
	''' 
	''' </summary>
	''' <seealso cref= java.security.Key </seealso>
	''' <seealso cref= java.security.KeyFactory </seealso>
	''' <seealso cref= KeySpec </seealso>
	''' <seealso cref= PKCS8EncodedKeySpec </seealso>
	''' <seealso cref= RSAPrivateKeySpec </seealso>
	''' <seealso cref= RSAPublicKeySpec </seealso>

	Public Class RSAPrivateCrtKeySpec
		Inherits RSAPrivateKeySpec

		Private ReadOnly publicExponent As System.Numerics.BigInteger
		Private ReadOnly primeP As System.Numerics.BigInteger
		Private ReadOnly primeQ As System.Numerics.BigInteger
		Private ReadOnly primeExponentP As System.Numerics.BigInteger
		Private ReadOnly primeExponentQ As System.Numerics.BigInteger
		Private ReadOnly crtCoefficient As System.Numerics.BigInteger



	   ''' <summary>
	   ''' Creates a new {@code RSAPrivateCrtKeySpec}
	   ''' given the modulus, publicExponent, privateExponent,
	   ''' primeP, primeQ, primeExponentP, primeExponentQ, and
	   ''' crtCoefficient as defined in PKCS#1.
	   ''' </summary>
	   ''' <param name="modulus"> the modulus n </param>
	   ''' <param name="publicExponent"> the public exponent e </param>
	   ''' <param name="privateExponent"> the private exponent d </param>
	   ''' <param name="primeP"> the prime factor p of n </param>
	   ''' <param name="primeQ"> the prime factor q of n </param>
	   ''' <param name="primeExponentP"> this is d mod (p-1) </param>
	   ''' <param name="primeExponentQ"> this is d mod (q-1) </param>
	   ''' <param name="crtCoefficient"> the Chinese Remainder Theorem
	   ''' coefficient q-1 mod p </param>
		Public Sub New(  modulus As System.Numerics.BigInteger,   publicExponent As System.Numerics.BigInteger,   privateExponent As System.Numerics.BigInteger,   primeP As System.Numerics.BigInteger,   primeQ As System.Numerics.BigInteger,   primeExponentP As System.Numerics.BigInteger,   primeExponentQ As System.Numerics.BigInteger,   crtCoefficient As System.Numerics.BigInteger)
			MyBase.New(modulus, privateExponent)
			Me.publicExponent = publicExponent
			Me.primeP = primeP
			Me.primeQ = primeQ
			Me.primeExponentP = primeExponentP
			Me.primeExponentQ = primeExponentQ
			Me.crtCoefficient = crtCoefficient
		End Sub

		''' <summary>
		''' Returns the public exponent.
		''' </summary>
		''' <returns> the public exponent </returns>
		Public Overridable Property publicExponent As System.Numerics.BigInteger
			Get
				Return Me.publicExponent
			End Get
		End Property

		''' <summary>
		''' Returns the primeP.
		''' </summary>
		''' <returns> the primeP </returns>
		Public Overridable Property primeP As System.Numerics.BigInteger
			Get
				Return Me.primeP
			End Get
		End Property

		''' <summary>
		''' Returns the primeQ.
		''' </summary>
		''' <returns> the primeQ </returns>
		Public Overridable Property primeQ As System.Numerics.BigInteger
			Get
				Return Me.primeQ
			End Get
		End Property

		''' <summary>
		''' Returns the primeExponentP.
		''' </summary>
		''' <returns> the primeExponentP </returns>
		Public Overridable Property primeExponentP As System.Numerics.BigInteger
			Get
				Return Me.primeExponentP
			End Get
		End Property

		''' <summary>
		''' Returns the primeExponentQ.
		''' </summary>
		''' <returns> the primeExponentQ </returns>
		Public Overridable Property primeExponentQ As System.Numerics.BigInteger
			Get
				Return Me.primeExponentQ
			End Get
		End Property

		''' <summary>
		''' Returns the crtCoefficient.
		''' </summary>
		''' <returns> the crtCoefficient </returns>
		Public Overridable Property crtCoefficient As System.Numerics.BigInteger
			Get
				Return Me.crtCoefficient
			End Get
		End Property
	End Class

End Namespace
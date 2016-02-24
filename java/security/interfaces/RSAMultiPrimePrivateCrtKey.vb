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

Namespace java.security.interfaces


	''' <summary>
	''' The interface to an RSA multi-prime private key, as defined in the
	''' PKCS#1 v2.1, using the <i>Chinese Remainder Theorem</i>
	''' (CRT) information values.
	''' 
	''' @author Valerie Peng
	''' 
	''' </summary>
	''' <seealso cref= java.security.spec.RSAPrivateKeySpec </seealso>
	''' <seealso cref= java.security.spec.RSAMultiPrimePrivateCrtKeySpec </seealso>
	''' <seealso cref= RSAPrivateKey </seealso>
	''' <seealso cref= RSAPrivateCrtKey
	''' 
	''' @since 1.4 </seealso>

	Public Interface RSAMultiPrimePrivateCrtKey
		Inherits RSAPrivateKey

		''' <summary>
		''' The type fingerprint that is set to indicate
		''' serialization compatibility with a previous
		''' version of the type.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		static final long serialVersionUID = 618058533534628008L;

		''' <summary>
		''' Returns the public exponent.
		''' </summary>
		''' <returns> the public exponent. </returns>
		ReadOnly Property publicExponent As System.Numerics.BigInteger

		''' <summary>
		''' Returns the primeP.
		''' </summary>
		''' <returns> the primeP. </returns>
		ReadOnly Property primeP As System.Numerics.BigInteger

		''' <summary>
		''' Returns the primeQ.
		''' </summary>
		''' <returns> the primeQ. </returns>
		ReadOnly Property primeQ As System.Numerics.BigInteger

		''' <summary>
		''' Returns the primeExponentP.
		''' </summary>
		''' <returns> the primeExponentP. </returns>
		ReadOnly Property primeExponentP As System.Numerics.BigInteger

		''' <summary>
		''' Returns the primeExponentQ.
		''' </summary>
		''' <returns> the primeExponentQ. </returns>
		ReadOnly Property primeExponentQ As System.Numerics.BigInteger

		''' <summary>
		''' Returns the crtCoefficient.
		''' </summary>
		''' <returns> the crtCoefficient. </returns>
		ReadOnly Property crtCoefficient As System.Numerics.BigInteger

		''' <summary>
		''' Returns the otherPrimeInfo or null if there are only
		''' two prime factors (p and q).
		''' </summary>
		''' <returns> the otherPrimeInfo. </returns>
		ReadOnly Property otherPrimeInfo As java.security.spec.RSAOtherPrimeInfo()
	End Interface

End Namespace
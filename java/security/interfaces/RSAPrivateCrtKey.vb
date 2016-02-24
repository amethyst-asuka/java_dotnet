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

Namespace java.security.interfaces


	''' <summary>
	''' The interface to an RSA private key, as defined in the PKCS#1 standard,
	''' using the <i>Chinese Remainder Theorem</i> (CRT) information values.
	''' 
	''' @author Jan Luehe
	''' 
	''' </summary>
	''' <seealso cref= RSAPrivateKey </seealso>

	Public Interface RSAPrivateCrtKey
		Inherits RSAPrivateKey

		''' <summary>
		''' The type fingerprint that is set to indicate
		''' serialization compatibility with a previous
		''' version of the type.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		static final long serialVersionUID = -5682214253527700368L;

		''' <summary>
		''' Returns the public exponent.
		''' </summary>
		''' <returns> the public exponent </returns>
		ReadOnly Property publicExponent As System.Numerics.BigInteger

		''' <summary>
		''' Returns the primeP.
		''' </summary>
		''' <returns> the primeP </returns>
		ReadOnly Property primeP As System.Numerics.BigInteger

		''' <summary>
		''' Returns the primeQ.
		''' </summary>
		''' <returns> the primeQ </returns>
		ReadOnly Property primeQ As System.Numerics.BigInteger

		''' <summary>
		''' Returns the primeExponentP.
		''' </summary>
		''' <returns> the primeExponentP </returns>
		ReadOnly Property primeExponentP As System.Numerics.BigInteger

		''' <summary>
		''' Returns the primeExponentQ.
		''' </summary>
		''' <returns> the primeExponentQ </returns>
		ReadOnly Property primeExponentQ As System.Numerics.BigInteger

		''' <summary>
		''' Returns the crtCoefficient.
		''' </summary>
		''' <returns> the crtCoefficient </returns>
		ReadOnly Property crtCoefficient As System.Numerics.BigInteger
	End Interface

End Namespace
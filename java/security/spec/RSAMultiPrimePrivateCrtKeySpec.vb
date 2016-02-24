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
	''' This class specifies an RSA multi-prime private key, as defined in the
	''' PKCS#1 v2.1, using the Chinese Remainder Theorem (CRT) information
	''' values for efficiency.
	''' 
	''' @author Valerie Peng
	''' 
	''' </summary>
	''' <seealso cref= java.security.Key </seealso>
	''' <seealso cref= java.security.KeyFactory </seealso>
	''' <seealso cref= KeySpec </seealso>
	''' <seealso cref= PKCS8EncodedKeySpec </seealso>
	''' <seealso cref= RSAPrivateKeySpec </seealso>
	''' <seealso cref= RSAPublicKeySpec </seealso>
	''' <seealso cref= RSAOtherPrimeInfo
	''' 
	''' @since 1.4 </seealso>

	Public Class RSAMultiPrimePrivateCrtKeySpec
		Inherits RSAPrivateKeySpec

		Private ReadOnly publicExponent As System.Numerics.BigInteger
		Private ReadOnly primeP As System.Numerics.BigInteger
		Private ReadOnly primeQ As System.Numerics.BigInteger
		Private ReadOnly primeExponentP As System.Numerics.BigInteger
		Private ReadOnly primeExponentQ As System.Numerics.BigInteger
		Private ReadOnly crtCoefficient As System.Numerics.BigInteger
		Private ReadOnly otherPrimeInfo As RSAOtherPrimeInfo()

	   ''' <summary>
	   ''' Creates a new {@code RSAMultiPrimePrivateCrtKeySpec}
	   ''' given the modulus, publicExponent, privateExponent,
	   ''' primeP, primeQ, primeExponentP, primeExponentQ,
	   ''' crtCoefficient, and otherPrimeInfo as defined in PKCS#1 v2.1.
	   ''' 
	   ''' <p>Note that the contents of {@code otherPrimeInfo}
	   ''' are copied to protect against subsequent modification when
	   ''' constructing this object.
	   ''' </summary>
	   ''' <param name="modulus"> the modulus n. </param>
	   ''' <param name="publicExponent"> the public exponent e. </param>
	   ''' <param name="privateExponent"> the private exponent d. </param>
	   ''' <param name="primeP"> the prime factor p of n. </param>
	   ''' <param name="primeQ"> the prime factor q of n. </param>
	   ''' <param name="primeExponentP"> this is d mod (p-1). </param>
	   ''' <param name="primeExponentQ"> this is d mod (q-1). </param>
	   ''' <param name="crtCoefficient"> the Chinese Remainder Theorem
	   ''' coefficient q-1 mod p. </param>
	   ''' <param name="otherPrimeInfo"> triplets of the rest of primes, null can be
	   ''' specified if there are only two prime factors (p and q). </param>
	   ''' <exception cref="NullPointerException"> if any of the parameters, i.e.
	   ''' {@code modulus},
	   ''' {@code publicExponent}, {@code privateExponent},
	   ''' {@code primeP}, {@code primeQ},
	   ''' {@code primeExponentP}, {@code primeExponentQ},
	   ''' {@code crtCoefficient}, is null. </exception>
	   ''' <exception cref="IllegalArgumentException"> if an empty, i.e. 0-length,
	   ''' {@code otherPrimeInfo} is specified. </exception>
		Public Sub New(ByVal modulus As System.Numerics.BigInteger, ByVal publicExponent As System.Numerics.BigInteger, ByVal privateExponent As System.Numerics.BigInteger, ByVal primeP As System.Numerics.BigInteger, ByVal primeQ As System.Numerics.BigInteger, ByVal primeExponentP As System.Numerics.BigInteger, ByVal primeExponentQ As System.Numerics.BigInteger, ByVal crtCoefficient As System.Numerics.BigInteger, ByVal otherPrimeInfo As RSAOtherPrimeInfo())
			MyBase.New(modulus, privateExponent)
			If modulus Is Nothing Then Throw New NullPointerException("the modulus parameter must be " & "non-null")
			If publicExponent Is Nothing Then Throw New NullPointerException("the publicExponent parameter " & "must be non-null")
			If privateExponent Is Nothing Then Throw New NullPointerException("the privateExponent parameter " & "must be non-null")
			If primeP Is Nothing Then Throw New NullPointerException("the primeP parameter " & "must be non-null")
			If primeQ Is Nothing Then Throw New NullPointerException("the primeQ parameter " & "must be non-null")
			If primeExponentP Is Nothing Then Throw New NullPointerException("the primeExponentP parameter " & "must be non-null")
			If primeExponentQ Is Nothing Then Throw New NullPointerException("the primeExponentQ parameter " & "must be non-null")
			If crtCoefficient Is Nothing Then Throw New NullPointerException("the crtCoefficient parameter " & "must be non-null")
			Me.publicExponent = publicExponent
			Me.primeP = primeP
			Me.primeQ = primeQ
			Me.primeExponentP = primeExponentP
			Me.primeExponentQ = primeExponentQ
			Me.crtCoefficient = crtCoefficient
			If otherPrimeInfo Is Nothing Then
				Me.otherPrimeInfo = Nothing
			ElseIf otherPrimeInfo.Length = 0 Then
				Throw New IllegalArgumentException("the otherPrimeInfo " & "parameter must not be empty")
			Else
				Me.otherPrimeInfo = otherPrimeInfo.clone()
			End If
		End Sub

		''' <summary>
		''' Returns the public exponent.
		''' </summary>
		''' <returns> the public exponent. </returns>
		Public Overridable Property publicExponent As System.Numerics.BigInteger
			Get
				Return Me.publicExponent
			End Get
		End Property

		''' <summary>
		''' Returns the primeP.
		''' </summary>
		''' <returns> the primeP. </returns>
		Public Overridable Property primeP As System.Numerics.BigInteger
			Get
				Return Me.primeP
			End Get
		End Property

		''' <summary>
		''' Returns the primeQ.
		''' </summary>
		''' <returns> the primeQ. </returns>
		Public Overridable Property primeQ As System.Numerics.BigInteger
			Get
				Return Me.primeQ
			End Get
		End Property

		''' <summary>
		''' Returns the primeExponentP.
		''' </summary>
		''' <returns> the primeExponentP. </returns>
		Public Overridable Property primeExponentP As System.Numerics.BigInteger
			Get
				Return Me.primeExponentP
			End Get
		End Property

		''' <summary>
		''' Returns the primeExponentQ.
		''' </summary>
		''' <returns> the primeExponentQ. </returns>
		Public Overridable Property primeExponentQ As System.Numerics.BigInteger
			Get
				Return Me.primeExponentQ
			End Get
		End Property

		''' <summary>
		''' Returns the crtCoefficient.
		''' </summary>
		''' <returns> the crtCoefficient. </returns>
		Public Overridable Property crtCoefficient As System.Numerics.BigInteger
			Get
				Return Me.crtCoefficient
			End Get
		End Property

		''' <summary>
		''' Returns a copy of the otherPrimeInfo or null if there are
		''' only two prime factors (p and q).
		''' </summary>
		''' <returns> the otherPrimeInfo. Returns a new array each
		''' time this method is called. </returns>
		Public Overridable Property otherPrimeInfo As RSAOtherPrimeInfo()
			Get
				If otherPrimeInfo Is Nothing Then Return Nothing
				Return otherPrimeInfo.clone()
			End Get
		End Property
	End Class

End Namespace